using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Lo que el panel dice a la ventana cuando aplicó un cambio: la ruta editada y una nota para la barra de estado.</summary>
    public sealed class PanelEditEventArgs : EventArgs
    {
        public PanelEditEventArgs(string path, string note)
        {
            Path = path;
            Note = note;
        }

        public string Path { get; }
        public string Note { get; }
    }

    /// <summary>
    /// Fase 11 (decisión D4): el panel por secciones de la previsualización (Cartela, Cordón, Barra 1-2-3), construido a
    /// partir de <see cref="SpecEditor.ListFields"/> (la misma lista que llena la tabla de Avanzado). Cada campo pinta el
    /// control que le toca: desplegable para espesor, diámetro, soldadura, tipo de unión, unión al cordón, cara de la
    /// cartela y rol; casilla para los sí/no; número para el resto; solo lectura para los IDs y el perfil del modelo. Todo
    /// cambio pasa por <see cref="PreviewSession"/> (una sola fuente de verdad, el JSON); la ventana refresca el croquis y la
    /// validación como hoy. Un desplegable con rótulo escribe <c>*_mm</c> y <c>*_label</c> a la vez. Los errores de
    /// validación con <c>path</c> se enseñan junto al campo. Las palabras son las de la decisión D6.
    /// </summary>
    public partial class SpecPanel : UserControl
    {
        private static readonly Brush ErrorBorder = new SolidColorBrush(Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush ReadOnlyText = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));

        /// <summary>Un campo pintado: su control, el texto de error y cómo volver a leer su valor.</summary>
        private sealed class FieldControl
        {
            public FieldControl(SpecField field, FrameworkElement control, TextBlock error, Action<SpecField> refresh)
            {
                Field = field;
                Control = control;
                Error = error;
                Refresh = refresh;
            }

            public SpecField Field { get; set; }
            public FrameworkElement Control { get; }
            public TextBlock Error { get; }
            public Action<SpecField> Refresh { get; }
            public Brush? NormalBorder { get; set; }
        }

        private readonly Dictionary<string, FieldControl> _controls = new Dictionary<string, FieldControl>(StringComparer.Ordinal);
        private readonly Dictionary<string, bool> _expanded = new Dictionary<string, bool>(StringComparer.Ordinal);
        private readonly List<string> _builtPaths = new List<string>();
        private PreviewSession? _session;
        private bool _refreshing;

        public SpecPanel()
        {
            InitializeComponent();
        }

        /// <summary>Un campo tomó el foco: la ventana resalta su cota en el croquis.</summary>
        public event EventHandler<string>? FieldFocused;

        /// <summary>Un cambio se escribió en el JSON: la ventana redibuja, valida y refresca.</summary>
        public event EventHandler<PanelEditEventArgs>? EditApplied;

        /// <summary>Un cambio no se pudo aplicar (texto del error).</summary>
        public event EventHandler<string>? EditRejected;

        public event EventHandler? ChangeTemplateRequested;
        public event EventHandler? BriefRequested;
        public event EventHandler? OpenCatalogRequested;

        /// <summary>Secciones que no van al panel (están en Avanzado).</summary>
        private static bool IsAdvancedSection(string section) => section == "Cadenas de cotas" || section == "Dudas" || section == "Origen";

        public void Bind(PreviewSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            Rebuild();
        }

        /// <summary>
        /// Relee los campos de la sesión: si son las mismas rutas, actualiza los valores en sitio (sin perder el foco ni el
        /// desplegado de las secciones); si cambiaron (por ejemplo, otra unión), reconstruye.
        /// </summary>
        public void Refresh()
        {
            if (_session == null) return;
            List<SpecField> fields = _session.Fields.Where(f => !IsAdvancedSection(f.Section)).ToList();
            var paths = fields.Where(IsPainted).Select(f => f.Path).ToList();
            if (!paths.SequenceEqual(_builtPaths, StringComparer.Ordinal))
            {
                Rebuild();
                return;
            }
            _refreshing = true;
            try
            {
                foreach (SpecField field in fields)
                {
                    if (_controls.TryGetValue(field.Path, out FieldControl? control))
                    {
                        control.Field = field;
                        control.Refresh(field);
                    }
                }
            }
            finally
            {
                _refreshing = false;
            }
        }

        /// <summary>Enseña junto a cada campo los errores del validador que traen su ruta (o la de su rótulo).</summary>
        public void ShowIssues(Core.Validation.ValidationResult? validation)
        {
            foreach (FieldControl control in _controls.Values)
            {
                control.Error.Text = string.Empty;
                control.Error.Visibility = Visibility.Collapsed;
                if (control.Control is Control c && control.NormalBorder != null) c.BorderBrush = control.NormalBorder;
            }
            if (validation == null) return;
            foreach (ApiError error in validation.Errors)
            {
                if (string.IsNullOrEmpty(error.Path)) continue;
                string path = error.Path!;
                if (!_controls.TryGetValue(path, out FieldControl? target) && path.EndsWith("_label", StringComparison.Ordinal))
                {
                    _controls.TryGetValue(path.Substring(0, path.Length - "_label".Length) + "_mm", out target);
                }
                if (target == null) continue;
                target.Error.Text = target.Error.Text.Length == 0 ? error.Code + ": " + error.Message : target.Error.Text + "\n" + error.Code + ": " + error.Message;
                target.Error.Visibility = Visibility.Visible;
                if (target.Control is Control control)
                {
                    target.NormalBorder ??= control.BorderBrush;
                    control.BorderBrush = ErrorBorder;
                }
            }
        }

        /// <summary>La cabecera: texto, miniatura (V4) y qué botones se ven. Con texto nulo la cabecera se oculta.</summary>
        public void SetHeader(string? text, string? warning, ImageSource? thumbnail, bool showChange, bool showBrief, bool showCatalog)
        {
            HeaderBorder.Visibility = string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;
            HeaderText.Text = text ?? string.Empty;
            HeaderWarningText.Text = warning ?? string.Empty;
            HeaderWarningText.Visibility = string.IsNullOrWhiteSpace(warning) ? Visibility.Collapsed : Visibility.Visible;
            HeaderImage.Source = thumbnail;
            HeaderImageBorder.Visibility = thumbnail != null ? Visibility.Visible : Visibility.Collapsed;
            ChangeButton.Visibility = showChange ? Visibility.Visible : Visibility.Collapsed;
            BriefButton.Visibility = showBrief ? Visibility.Visible : Visibility.Collapsed;
            CatalogButton.Visibility = showCatalog ? Visibility.Visible : Visibility.Collapsed;
            HeaderBorder.Background = showCatalog || showBrief ? new SolidColorBrush(Color.FromRgb(0xFF, 0xF1, 0xE0)) : new SolidColorBrush(Color.FromRgb(0xEA, 0xF3, 0xFF));
            HeaderBorder.BorderBrush = showCatalog || showBrief ? new SolidColorBrush(Color.FromRgb(0xE0, 0xA0, 0x60)) : new SolidColorBrush(Color.FromRgb(0x9B, 0xBD, 0xE8));
        }

        /// <summary>Lleva el foco al control de una ruta (al elegir una cota en el croquis), si está en el panel.</summary>
        public bool FocusPath(string? path)
        {
            if (path == null || !_controls.TryGetValue(path, out FieldControl? control)) return false;
            ExpandSectionOf(control.Control);
            control.Control.BringIntoView();
            return true;
        }

        private static void ExpandSectionOf(FrameworkElement element)
        {
            DependencyObject? current = element;
            while (current != null)
            {
                if (current is Expander expander)
                {
                    expander.IsExpanded = true;
                    return;
                }
                current = VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
            }
        }

        // ---- construcción ----

        private static bool IsPainted(SpecField field)
        {
            if (field.Kind == SpecFieldKind.Points) return false;
            // Los rótulos se funden con su valor en mm en un solo desplegable (Fase 12: también el agujero de la placa de extremo).
            return !field.Path.EndsWith(".thickness_label", StringComparison.Ordinal) && !field.Path.EndsWith(".diameter_label", StringComparison.Ordinal)
                   && !field.Path.EndsWith(".hole_label", StringComparison.Ordinal);
        }

        private void Rebuild()
        {
            if (_session == null) return;
            _refreshing = true;
            try
            {
                Sections.Children.Clear();
                _controls.Clear();
                _builtPaths.Clear();
                List<SpecField> fields = _session.Fields.Where(f => !IsAdvancedSection(f.Section)).ToList();
                foreach (string section in OrderedSections(fields))
                {
                    List<SpecField> sectionFields = fields.Where(f => f.Section == section).ToList();
                    Sections.Children.Add(BuildSection(section, sectionFields));
                }
                if (fields.Count == 0)
                {
                    Sections.Children.Add(new TextBlock { Text = "El JSON no se puede leer: corrígelo en Avanzado (o en el archivo y pulsa Recargar).", Foreground = ErrorBorder, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(4) });
                }
            }
            finally
            {
                _refreshing = false;
            }
        }

        /// <summary>
        /// Cartela, Cordón y después las barras (decisión D4), en el orden de la especificación. Fase 12: en una placa de extremo,
        /// Plancha, Pernos, Tramo 1, Tramo 2 y Empalme; en un nudo soldado, Cordón y las barras.
        /// </summary>
        private static List<string> OrderedSections(List<SpecField> fields)
        {
            var sections = fields.Select(f => f.Section).Distinct().ToList();
            var ordered = new List<string>();
            foreach (string first in new[] { "Plancha", "Pernos", "Tramo 1", "Tramo 2", "Empalme", "Cartela", "Cordón" })
            {
                if (sections.Contains(first)) ordered.Add(first);
            }
            ordered.AddRange(sections.Where(s => !ordered.Contains(s)));
            return ordered;
        }

        private Expander BuildSection(string section, List<SpecField> fields)
        {
            var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 120 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star), MinWidth = 150 });
            int row = 0;
            foreach (SpecField field in fields)
            {
                if (!IsPainted(field)) continue;
                FieldControl control = BuildField(field);
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var label = new TextBlock { Text = LabelOf(field), Style = (Style)FindResource("FieldLabel"), ToolTip = field.Hint != null ? field.Hint + "  ·  " + field.Path : field.Path };
                Grid.SetRow(label, row);
                Grid.SetColumn(label, 0);
                Grid.SetRow(control.Control, row);
                Grid.SetColumn(control.Control, 1);
                Grid.SetRow(control.Error, row + 1);
                Grid.SetColumn(control.Error, 0);
                Grid.SetColumnSpan(control.Error, 2);
                grid.Children.Add(label);
                grid.Children.Add(control.Control);
                grid.Children.Add(control.Error);
                _controls[field.Path] = control;
                _builtPaths.Add(field.Path);
                row += 2;
            }
            bool expanded = !_expanded.TryGetValue(section, out bool remembered) || remembered;
            var expander = new Expander { Header = new TextBlock { Text = SectionTitle(section), FontWeight = FontWeights.SemiBold }, Content = grid, IsExpanded = expanded };
            expander.Expanded += (_, _) => _expanded[section] = true;
            expander.Collapsed += (_, _) => _expanded[section] = false;
            return expander;
        }

        /// <summary>"Barra 1 · diagonal 1249630" → "Barra 1 · diagonal · +45,0° · 1249630" si el nudo está leído.</summary>
        private string SectionTitle(string section)
        {
            if (_session?.Node == null || !section.StartsWith("Barra ", StringComparison.Ordinal)) return section;
            string[] parts = section.Split(' ');
            if (parts.Length < 2 || !long.TryParse(parts[parts.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long id)) return section;
            TemplateNodeMember? member = _session.Node.Find(id);
            if (member == null) return section;
            int dot = section.IndexOf('·');
            string role = dot >= 0 ? section.Substring(dot + 1).Trim() : "barra";
            role = role.Replace(parts[parts.Length - 1], string.Empty).Trim();
            return parts[0] + " " + parts[1] + " · " + role + " · " + TemplateChooser.SignedDegrees(member.AngleDeg) + " · " + id;
        }

        private static string LabelOf(SpecField field)
        {
            string path = field.Path;
            if (path.EndsWith(".thickness_mm", StringComparison.Ordinal)) return path.Contains(".plate.") ? "Placa: espesor" : "Espesor";
            if (path.EndsWith(".bolts.diameter_mm", StringComparison.Ordinal)) return "Pernos: Ø";
            if (path.EndsWith(".attachment.type", StringComparison.Ordinal)) return "Unión";
            if (path.EndsWith(".end_setback_mm", StringComparison.Ordinal)) return "Retiro (mm)";
            if (path.EndsWith(".hole_mm", StringComparison.Ordinal)) return "Agujero Ø";
            if (path == "bolts.diameter_mm") return "Pernos: Ø";
            if (path == "plate.thickness_mm") return "Espesor";
            if (path.EndsWith(".attachment.slot_length_mm", StringComparison.Ordinal)) return "Ranura (mm)";
            if (path.EndsWith(".gusset_face", StringComparison.Ordinal)) return "Placa: cara de la cartela";
            if (path == "gusset.chord_interface") return "Unión al cordón";
            return field.Label;
        }

        private FieldControl BuildField(SpecField field)
        {
            var error = new TextBlock { Style = (Style)FindResource("FieldError"), Visibility = Visibility.Collapsed };
            string path = field.Path;
            PanelChoices choices = _session!.Choices;

            if (path.EndsWith(".thickness_mm", StringComparison.Ordinal))
            {
                string labelPath = path.Substring(0, path.Length - "_mm".Length) + "_label";
                return LabeledCombo(field, error, choices.ThicknessOptions(), labelPath, "Espesor");
            }
            if (path.EndsWith(".bolts.diameter_mm", StringComparison.Ordinal))
            {
                string labelPath = path.Substring(0, path.Length - "_mm".Length) + "_label";
                return LabeledCombo(field, error, choices.BoltDiameterOptions(), labelPath, "Diámetro de perno");
            }
            if (path.EndsWith(".size_mm", StringComparison.Ordinal) && path.Contains("weld"))
            {
                return NumberCombo(field, error, choices.WeldOptions());
            }
            // Fase 12: la placa de extremo y el nudo soldado.
            if (path.EndsWith(".hole_mm", StringComparison.Ordinal))
            {
                string labelPath = path.Substring(0, path.Length - "_mm".Length) + "_label";
                return LabeledCombo(field, error, choices.HoleOptions(), labelPath, "Agujero");
            }
            if (path == "bolts.nuts")
            {
                return NamedCombo(field, error, new List<NamedChoice>
                {
                    new NamedChoice { Value = "both_sides", Text = "tuerca a ambos lados (atraviesan las dos planchas)" },
                    new NamedChoice { Value = "one_side", Text = "tuerca a un lado" },
                }, null, emptyMeans: "both_sides");
            }
            if (path == "chord.face")
            {
                return NamedCombo(field, error, new List<NamedChoice>
                {
                    new NamedChoice { Value = "auto", Text = "auto (la cara donde están las barras)" },
                    new NamedChoice { Value = "+y", Text = "+y (cara de arriba en el plano de la cercha)" },
                    new NamedChoice { Value = "-y", Text = "-y (cara de abajo)" },
                }, null, emptyMeans: "auto");
            }
            if (path.EndsWith(".end_cut", StringComparison.Ordinal))
            {
                return NamedCombo(field, error, new List<NamedChoice> { new NamedChoice { Value = "to_chord_face", Text = "cortada contra la cara del cordón" } }, null, emptyMeans: "to_chord_face");
            }
            if (path.EndsWith(".splice_hint.detail", StringComparison.Ordinal))
            {
                return NamedCombo(field, error, new List<NamedChoice> { new NamedChoice { Value = "B", Text = "B: empalme de la diagonal (la casilla Partir la diagonal lo parte y lo crea con el nudo)" } }, null, emptyMeans: "B");
            }
            if (path.EndsWith(".attachment.type", StringComparison.Ordinal))
            {
                int memberIndex = MemberIndexOf(path);
                return NamedCombo(field, error, choices.AttachmentTypes, value =>
                {
                    if (!_session.TrySetAttachmentType(memberIndex, value, out string err)) return (false, err);
                    return (true, "Barra " + (memberIndex + 1) + ": unión " + TextOf(choices.AttachmentTypes, value) + (value == "bolted_knife_plate" ? " (placa y pernos con los valores por defecto, o los de antes si ya los tenía)." : " (ranura y soldadura por defecto, o las de antes si ya las tenía)."));
                });
            }
            // Fase 13: el contorno de la cartela, automático (calculado desde las barras) o polígono (puntos fijos).
            if (path == "gusset.outline.mode")
            {
                return NamedCombo(field, error, new List<NamedChoice>
                {
                    new NamedChoice { Value = Core.Geometry2D.AutoOutline.Mode, Text = "Automático: se calcula desde las barras (margen de choices.json)" },
                    new NamedChoice { Value = Core.Geometry2D.AutoOutline.PolygonMode, Text = "Polígono: puntos fijos (convierte los calculados y deja editarlos)" },
                }, value =>
                {
                    if (!_session.TrySetOutlineMode(value, out string err)) return (false, err);
                    return (true, value == Core.Geometry2D.AutoOutline.Mode
                        ? "Contorno automático: los puntos se calculan desde las barras; ancho y alto salen de la caja calculada."
                        : "Contorno convertido en polígono: los puntos calculados quedan fijos y se editan en Avanzado (cuadro de contorno) o estirando las cotas.");
                }, emptyMeans: Core.Geometry2D.AutoOutline.PolygonMode);
            }
            if (path == "gusset.chord_interface") return NamedCombo(field, error, choices.ChordInterfaces, null);
            if (path.EndsWith(".plate.gusset_face", StringComparison.Ordinal)) return NamedCombo(field, error, choices.GussetFaces, null, emptyMeans: "+z");
            if (path.EndsWith(".role", StringComparison.Ordinal)) return NamedCombo(field, error, choices.Roles, null);
            if (field.Kind == SpecFieldKind.Boolean) return CheckBoxField(field, error);
            bool readOnly = !field.IsEditable || path.EndsWith(".element_id", StringComparison.Ordinal) || path.EndsWith(".profile", StringComparison.Ordinal);
            if (readOnly) return ReadOnlyField(field, error);
            return TextField(field, error);
        }

        private static int MemberIndexOf(string path)
        {
            int open = path.IndexOf('[');
            int close = path.IndexOf(']');
            return open >= 0 && close > open && int.TryParse(path.Substring(open + 1, close - open - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out int index) ? index : -1;
        }

        private static string TextOf(IEnumerable<NamedChoice> choices, string value) => choices.FirstOrDefault(c => string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase))?.Text ?? value;

        private void Hook(FrameworkElement control, string path)
        {
            control.GotKeyboardFocus += (_, _) => FieldFocused?.Invoke(this, path);
            control.ToolTip ??= path;
        }

        /// <summary>Desplegable de rótulo + mm (espesores, diámetros): escribe los dos campos a la vez; "Otro: x mm" no cambia nada.</summary>
        private FieldControl LabeledCombo(SpecField field, TextBlock error, List<ChoiceOption> options, string labelPath, string what)
        {
            var combo = new ComboBox { Margin = new Thickness(0, 2, 0, 2) };
            void Fill(SpecField current)
            {
                combo.Items.Clear();
                double? value = SpecEditor.TryParseNumber(current.Value, out double v) ? v : (double?)null;
                ChoiceOption? selected = PanelChoices.OptionFor(options, value, _session!.Limits.LabelValueToleranceMm);
                foreach (ChoiceOption option in options) combo.Items.Add(option);
                if (selected != null && selected.IsOther) combo.Items.Add(selected);
                combo.SelectedItem = selected == null ? null : combo.Items.OfType<ChoiceOption>().FirstOrDefault(o => o.IsOther == selected.IsOther && Math.Abs(o.ValueMm - selected.ValueMm) < 1e-9);
            }
            Fill(field);
            combo.SelectionChanged += (_, _) =>
            {
                if (_refreshing || combo.SelectedItem is not ChoiceOption option || option.IsOther) return;
                if (!_session!.TrySetLabeledValue(field.Path, labelPath, option.Label, out string err))
                {
                    EditRejected?.Invoke(this, err);
                    return;
                }
                EditApplied?.Invoke(this, new PanelEditEventArgs(field.Path, what + ": " + option.Text + " (se escribieron " + field.Path + " y el rótulo a la vez)."));
            };
            Hook(combo, field.Path);
            return new FieldControl(field, combo, error, Fill);
        }

        /// <summary>Desplegable de números (soldaduras): escribe el valor en mm.</summary>
        private FieldControl NumberCombo(SpecField field, TextBlock error, List<ChoiceOption> options)
        {
            var combo = new ComboBox { Margin = new Thickness(0, 2, 0, 2) };
            void Fill(SpecField current)
            {
                combo.Items.Clear();
                double? value = SpecEditor.TryParseNumber(current.Value, out double v) ? v : (double?)null;
                ChoiceOption? selected = PanelChoices.OptionFor(options, value, 0.05);
                foreach (ChoiceOption option in options) combo.Items.Add(option);
                if (selected != null && selected.IsOther) combo.Items.Add(selected);
                combo.SelectedItem = selected == null ? null : combo.Items.OfType<ChoiceOption>().FirstOrDefault(o => o.IsOther == selected.IsOther && Math.Abs(o.ValueMm - selected.ValueMm) < 1e-9);
            }
            Fill(field);
            combo.SelectionChanged += (_, _) =>
            {
                if (_refreshing || combo.SelectedItem is not ChoiceOption option || option.IsOther) return;
                Apply(field, option.ValueMm.ToString("0.###", CultureInfo.InvariantCulture), "Soldadura: " + option.Text + ".");
            };
            Hook(combo, field.Path);
            return new FieldControl(field, combo, error, Fill);
        }

        /// <summary>Desplegable de valores con nombre (unión, unión al cordón, cara, rol). <paramref name="apply"/> nulo = escribir el valor tal cual.</summary>
        private FieldControl NamedCombo(SpecField field, TextBlock error, List<NamedChoice> choices, Func<string, (bool Ok, string Message)>? apply, string? emptyMeans = null)
        {
            var combo = new ComboBox { Margin = new Thickness(0, 2, 0, 2) };
            void Fill(SpecField current)
            {
                combo.Items.Clear();
                string value = string.IsNullOrWhiteSpace(current.Value) ? emptyMeans ?? string.Empty : current.Value.Trim();
                foreach (NamedChoice choice in choices) combo.Items.Add(choice);
                NamedChoice? selected = choices.FirstOrDefault(c => string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
                if (selected == null && value.Length > 0)
                {
                    selected = new NamedChoice { Value = value, Text = "Otro: " + value };
                    combo.Items.Add(selected);
                }
                combo.SelectedItem = selected;
            }
            Fill(field);
            combo.SelectionChanged += (_, _) =>
            {
                if (_refreshing || combo.SelectedItem is not NamedChoice choice) return;
                if (string.Equals(choice.Value, string.IsNullOrWhiteSpace(field.Value) ? emptyMeans : field.Value.Trim(), StringComparison.OrdinalIgnoreCase)) return;
                if (apply != null)
                {
                    var (ok, message) = apply(choice.Value);
                    if (!ok) EditRejected?.Invoke(this, message);
                    else EditApplied?.Invoke(this, new PanelEditEventArgs(field.Path, message));
                    return;
                }
                Apply(field, choice.Value, LabelOf(field) + ": " + choice.Text + ".");
            };
            Hook(combo, field.Path);
            return new FieldControl(field, combo, error, Fill);
        }

        private FieldControl CheckBoxField(SpecField field, TextBlock error)
        {
            var box = new CheckBox { Margin = new Thickness(0, 4, 0, 4), VerticalAlignment = VerticalAlignment.Center };
            void Fill(SpecField current) => box.IsChecked = string.Equals(current.Value, "true", StringComparison.OrdinalIgnoreCase);
            Fill(field);
            box.Click += (_, _) =>
            {
                if (_refreshing) return;
                bool checkedNow = box.IsChecked == true;
                Apply(field, checkedNow ? "true" : "false", field.Label + ": " + (checkedNow ? "sí" : "no") + ".");
            };
            Hook(box, field.Path);
            return new FieldControl(field, box, error, Fill);
        }

        private FieldControl ReadOnlyField(SpecField field, TextBlock error)
        {
            var text = new TextBlock { Margin = new Thickness(2, 4, 0, 4), Foreground = ReadOnlyText, TextTrimming = TextTrimming.CharacterEllipsis, ToolTip = "Del modelo (solo lectura aquí; se puede cambiar en Avanzado)" };
            void Fill(SpecField current) => text.Text = string.IsNullOrEmpty(current.Value) ? "—" : current.Value + (current.Path.EndsWith(".profile", StringComparison.Ordinal) ? "  (del modelo)" : "");
            Fill(field);
            return new FieldControl(field, text, error, Fill);
        }

        /// <summary>Número o texto libre: aplica con Enter o al salir del campo si cambió; Esc vuelve al valor de antes.</summary>
        private FieldControl TextField(SpecField field, TextBlock error)
        {
            var box = new TextBox { Margin = new Thickness(0, 2, 0, 2), MinWidth = 60 };
            string original = field.Value;
            void Fill(SpecField current)
            {
                original = current.Value;
                box.Text = current.Value;
            }
            Fill(field);
            void Commit()
            {
                if (_refreshing) return;
                string text = (box.Text ?? string.Empty).Trim();
                if (string.Equals(text, original, StringComparison.Ordinal)) return;
                if (field.Kind == SpecFieldKind.Number || field.Kind == SpecFieldKind.Integer)
                {
                    if (text.Length > 0 && !SpecEditor.TryParseNumber(text, out _))
                    {
                        EditRejected?.Invoke(this, "'" + text + "' no es un número (usa coma o punto decimal, sin unidades).");
                        box.Text = original;
                        return;
                    }
                }
                string before = original;
                original = text;
                if (!Apply(field, text, LabelOf(field) + ": " + (before.Length == 0 ? "vacío" : before) + " → " + (text.Length == 0 ? "vacío" : text) + "."))
                {
                    original = before;
                    box.Text = before;
                }
            }
            box.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    e.Handled = true;
                    Commit();
                }
                else if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    box.Text = original;
                }
            };
            box.LostKeyboardFocus += (_, _) => Commit();
            Hook(box, field.Path);
            return new FieldControl(field, box, error, Fill);
        }

        private bool Apply(SpecField field, string text, string note)
        {
            if (_session == null) return false;
            if (!_session.TrySetField(field.Path, text, out string err))
            {
                EditRejected?.Invoke(this, "No se aplicó el cambio en '" + LabelOf(field) + "': " + err);
                return false;
            }
            EditApplied?.Invoke(this, new PanelEditEventArgs(field.Path, note));
            return true;
        }

        private void OnChangeTemplate(object sender, RoutedEventArgs e) => ChangeTemplateRequested?.Invoke(this, EventArgs.Empty);

        private void OnBrief(object sender, RoutedEventArgs e) => BriefRequested?.Invoke(this, EventArgs.Empty);

        private void OnOpenCatalog(object sender, RoutedEventArgs e) => OpenCatalogRequested?.Invoke(this, EventArgs.Empty);
    }
}
