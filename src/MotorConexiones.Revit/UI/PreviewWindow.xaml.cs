using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using SpecValidationResult = MotorConexiones.Core.Validation.ValidationResult;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana "Previsualización de conexión" del botón de la cinta (el único camino del add-in con ventanas).
    /// Izquierda: croquis 2D con cotas. Derecha: tabla editable y contorno. Abajo: errores y avisos del validador,
    /// token abreviado y botones Recargar, Guardar JSON, Validar, Crear y Cancelar. No modifica el modelo: si la persona
    /// pulsa Crear, <see cref="CreateRequested"/> queda en verdadero y el comando crea al cerrarse la ventana.
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        private readonly PreviewSession _session;
        private readonly ObservableCollection<FieldRow> _rows = new ObservableCollection<FieldRow>();
        private readonly ObservableCollection<IssueRow> _issues = new ObservableCollection<IssueRow>();
        private bool _refreshing;
        private SketchDimension? _editingDimension;
        private bool _closingDimensionEditor;

        public PreviewWindow(PreviewSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            InitializeComponent();
            FieldsGrid.ItemsSource = _rows;
            IssuesList.ItemsSource = _issues;
            FileText.Text = _session.IsVirtualFile
                ? (_session.Title ?? "Plantilla del catálogo aplicada") + "  ·  Guardar JSON escribe en " + _session.FilePath
                : "Archivo: " + _session.FilePath;
            ReloadButton.IsEnabled = !_session.IsVirtualFile;
            if (_session.IsVirtualFile) ReloadButton.ToolTip = "No hay archivo que recargar: la especificación salió del catálogo.";
            Canvas.DimensionActivated += OnDimensionActivated;
            Loaded += (_, _) =>
            {
                RefreshAll();
                Canvas.Fit();
            };
        }

        /// <summary>Verdadero si la persona pulsó Crear con la validación en verde.</summary>
        public bool CreateRequested { get; private set; }

        private void RefreshAll(string? statusOverride = null, bool statusIsError = false)
        {
            _refreshing = true;
            try
            {
                _rows.Clear();
                foreach (SpecField field in _session.Fields)
                {
                    _rows.Add(new FieldRow(field));
                }
                OutlineBox.Text = _session.OutlineText;
                SummaryText.Text = _session.Summary();
                Canvas.Sketch = _session.Sketch;
                SketchNotesText.Text = _session.Sketch != null && _session.Sketch.Notes.Count > 0
                    ? string.Join(" · ", _session.Sketch.Notes)
                    : string.Empty;
                UpdateStatus(statusOverride, statusIsError);
            }
            finally
            {
                _refreshing = false;
            }
        }

        private void UpdateStatus(string? statusOverride = null, bool statusIsError = false)
        {
            SpecValidationResult? validation = _session.Validation;
            _issues.Clear();
            if (validation != null)
            {
                foreach (ApiError error in validation.Errors) _issues.Add(new IssueRow("Error", error));
                foreach (ApiError warning in validation.Warnings) _issues.Add(new IssueRow("Aviso", warning));
            }

            int errors = validation?.Errors.Count ?? 0;
            int warnings = validation?.Warnings.Count ?? 0;
            if (statusOverride != null)
            {
                StatusText.Text = statusOverride;
                StatusText.Foreground = statusIsError ? ErrorBrush : InfoBrush;
            }
            else if (_session.CanCreate)
            {
                StatusText.Text = "Validación correcta" + (warnings > 0 ? " con " + warnings + " aviso(s). Revísalos antes de crear." : ". Puedes crear la conexión.");
                StatusText.Foreground = OkBrush;
            }
            else
            {
                StatusText.Text = errors + " error(es) y " + warnings + " aviso(s). Corrige los errores en la tabla (o en el archivo y pulsa Recargar) para poder crear.";
                StatusText.Foreground = ErrorBrush;
            }

            TokenText.Text = "validation_token: " + _session.TokenShort;
            CreateButton.IsEnabled = _session.CanCreate;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is FieldRow row && !row.IsEditable)
            {
                e.Cancel = true;
            }
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_refreshing || e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not FieldRow row) return;
            string text = (e.EditingElement as TextBox)?.Text ?? row.Value;
            if (string.Equals(text, row.OriginalValue, StringComparison.Ordinal)) return;

            // Fuera del evento de edición para que el DataGrid termine su propio ciclo antes de reconstruir las filas.
            Dispatcher.BeginInvoke(new Action(() => ApplyEdit(row, text)));
        }

        private bool ApplyEdit(FieldRow row, string text, string? statusNote = null)
        {
            if (!_session.TrySetField(row.Path, text, out string error))
            {
                row.Value = row.OriginalValue;
                UpdateStatus("No se aplicó el cambio en '" + row.Label + "': " + error, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_edit_rejected", path = row.Path, value = text, error });
                return false;
            }
            JsonLineLogger.Write(new { @event = "ribbon_preview_edit", path = row.Path, value = text, is_valid = _session.CanCreate });
            RefreshAll(statusNote);
            Canvas.HighlightPath = row.Path;
            SelectRow(row.Path);
            return true;
        }

        private void SelectRow(string? path)
        {
            if (path == null) return;
            FieldRow? row = _rows.FirstOrDefault(r => r.Path == path);
            if (row == null) return;
            FieldsGrid.SelectedItem = row;
            FieldsGrid.ScrollIntoView(row);
        }

        private void OnFieldSelected(object sender, SelectionChangedEventArgs e)
        {
            Canvas.HighlightPath = (FieldsGrid.SelectedItem as FieldRow)?.Path;
        }

        // ---- edición de una cota en el croquis (ronda 6b) ----

        private static bool IsGussetSize(SketchDimension dimension) =>
            dimension.Kind == DimensionKind.GussetWidth || dimension.Kind == DimensionKind.GussetHeight;

        private void OnDimensionActivated(object? sender, DimensionActivatedEventArgs e)
        {
            SketchDimension dimension = e.Dimension;
            if (string.IsNullOrEmpty(dimension.Path))
            {
                UpdateStatus("Esta cota no corresponde a ningún campo del JSON: no se puede editar desde el croquis.", statusIsError: true);
                return;
            }

            FieldRow? row = _rows.FirstOrDefault(r => r.Path == dimension.Path);
            bool gussetSize = IsGussetSize(dimension);
            if (!gussetSize && row == null)
            {
                UpdateStatus("La cota apunta a '" + dimension.Path + "', que no está en la tabla: edítalo en el archivo y pulsa Recargar.", statusIsError: true);
                return;
            }

            _editingDimension = dimension;
            string label = gussetSize
                ? (dimension.Kind == DimensionKind.GussetWidth ? "Ancho de la cartela (estira el contorno en X)" : "Alto de la cartela (estira el contorno en Y)")
                : row!.Label;
            DimensionEditorLabel.Text = label + "  ·  " + dimension.Path;
            // Para la cartela se parte de lo medido en el contorno (es lo que dice la cota); para el resto, del valor de la tabla.
            DimensionEditorBox.Text = gussetSize ? SpecEditor.FormatNumber(dimension.ValueMm) : row!.Value;

            double x = Math.Max(0.0, Math.Min(e.Position.X + 14.0, Math.Max(0.0, Canvas.ActualWidth - 300.0)));
            double y = Math.Max(0.0, Math.Min(e.Position.Y - 70.0, Math.Max(0.0, Canvas.ActualHeight - 90.0)));
            DimensionEditor.Margin = new Thickness(x, y, 0, 0);
            DimensionEditor.Visibility = Visibility.Visible;
            Canvas.HighlightPath = dimension.Path;
            SelectRow(dimension.Path);
            DimensionEditorBox.Focus();
            DimensionEditorBox.SelectAll();
            UpdateStatus("Editando la cota '" + label + "': escribe el valor nuevo en mm y pulsa Enter (Esc cancela).");
        }

        private void OnDimensionEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true;
                CommitDimensionEdit();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CloseDimensionEditor();
                UpdateStatus("Edición de la cota cancelada: no se cambió nada.");
            }
        }

        private void OnDimensionEditorLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Clic fuera del cuadro = cancelar (igual que Esc). Si lo estamos cerrando nosotros, no hay nada que hacer.
            if (_closingDimensionEditor || DimensionEditor.Visibility != Visibility.Visible) return;
            CloseDimensionEditor();
        }

        private void CloseDimensionEditor()
        {
            _closingDimensionEditor = true;
            try
            {
                DimensionEditor.Visibility = Visibility.Collapsed;
                _editingDimension = null;
                Canvas.Focus();
            }
            finally
            {
                _closingDimensionEditor = false;
            }
        }

        private void CommitDimensionEdit()
        {
            SketchDimension? dimension = _editingDimension;
            string text = DimensionEditorBox.Text ?? string.Empty;
            CloseDimensionEditor();
            if (dimension == null || string.IsNullOrEmpty(dimension.Path)) return;

            if (IsGussetSize(dimension))
            {
                bool width = dimension.Kind == DimensionKind.GussetWidth;
                string what = width ? "ancho" : "alto";
                if (!SpecEditor.TryParseNumber(text, out double value))
                {
                    UpdateStatus("'" + text + "' no es un número (usa coma o punto decimal, sin unidades).", statusIsError: true);
                    return;
                }
                if (Math.Abs(value - dimension.ValueMm) < 0.005) return;
                if (!_session.TrySetGussetSize(width, value, out string error))
                {
                    UpdateStatus("No se aplicó el " + what + " de la cartela: " + error, statusIsError: true);
                    JsonLineLogger.Write(new { @event = "ribbon_preview_dimension_rejected", path = dimension.Path, value = text, error });
                    return;
                }
                JsonLineLogger.Write(new { @event = "ribbon_preview_dimension_edit", path = dimension.Path, from_mm = dimension.ValueMm, to_mm = value, is_valid = _session.CanCreate });
                RefreshAll(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "Cartela: {0} {1} → {2} mm. El contorno se ha estirado en {3} alrededor del punto de trabajo (mira el cuadro del contorno) y {4} = {2}. Revisa las cadenas de cotas si las tenías del plano.",
                    what, SketchText.Mm(dimension.ValueMm), SketchText.Mm(value), width ? "X" : "Y", width ? "width_mm" : "height_mm"));
                Canvas.HighlightPath = dimension.Path;
                SelectRow(dimension.Path);
                return;
            }

            FieldRow? row = _rows.FirstOrDefault(r => r.Path == dimension.Path);
            if (row == null)
            {
                UpdateStatus("La cota apunta a '" + dimension.Path + "', que no está en la tabla.", statusIsError: true);
                return;
            }
            if (string.Equals(text.Trim(), row.OriginalValue, StringComparison.Ordinal)) return;
            ApplyEdit(row, text, "Cota '" + row.Label + "': " + row.OriginalValue + " → " + text.Trim() + " mm. Croquis redibujado y validación repetida.");
        }

        private void OnApplyOutline(object sender, RoutedEventArgs e)
        {
            if (!_session.TrySetOutline(OutlineBox.Text, out string error))
            {
                UpdateStatus("El contorno no se aplicó: " + error, statusIsError: true);
                return;
            }
            RefreshAll();
            Canvas.HighlightPath = "gusset.width_mm";
        }

        private void OnFit(object sender, RoutedEventArgs e)
        {
            Canvas.Fit();
        }

        private void OnReload(object sender, RoutedEventArgs e)
        {
            try
            {
                _session.Reload();
                RefreshAll("Archivo recargado del disco: " + _session.FilePath);
                Canvas.Fit();
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo recargar el archivo: " + ex.Message, statusIsError: true);
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = _session.Save();
                UpdateStatus("JSON guardado en " + path + " (el original no se ha tocado).");
                JsonLineLogger.Write(new { @event = "ribbon_preview_saved", path });
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo guardar el JSON: " + ex.Message, statusIsError: true);
            }
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            _session.Refresh();
            RefreshAll();
        }

        // ---- catálogo de plantillas (Fase 7) ----

        /// <summary>Elige una plantilla y la aplica a las barras del nudo actual: sustituye el JSON de la ventana, nada se crea.</summary>
        private void OnOpenFromCatalog(object sender, RoutedEventArgs e)
        {
            if (_session.Spec == null)
            {
                UpdateStatus("El JSON actual no se puede leer: corrígelo (o pulsa Recargar) antes de aplicar una plantilla.", statusIsError: true);
                return;
            }

            var picker = new CatalogWindow(_session.Document, _session.UIDocument?.Application, pickOnly: true) { Owner = this };
            if (picker.ShowDialog() != true || picker.SelectedTemplate == null) return;
            CatalogTemplate template = picker.SelectedTemplate;

            var ids = new List<long>();
            long? chordId = _session.Spec.Chord != null && _session.Spec.Chord.ElementId > 0 ? _session.Spec.Chord.ElementId : (long?)null;
            if (chordId.HasValue) ids.Add(chordId.Value);
            if (_session.Spec.Members != null) ids.AddRange(_session.Spec.Members.Select(m => m.ElementId).Where(id => id > 0));
            if (_session.Spec.Node?.ElementIds != null) ids.AddRange(_session.Spec.Node.ElementIds.Where(id => id > 0));

            try
            {
                CatalogConfig config = CatalogConfigLoader.Load();
                CatalogApplyResult result = CatalogService.Apply(_session.Document, template, ids.Distinct().ToList(), chordId, null, config);
                _session.SetJson(result.Instantiation.SpecJson);
                string extra = result.Instantiation.Warnings.Count == 0 ? "" : " Avisos: " + string.Join(", ", result.Instantiation.Warnings.Select(w => w.Code).Distinct()) + ".";
                RefreshAll(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "Plantilla '{0}' aplicada al nudo en orientación {1} (desvío máximo {2:0.0}°). {3}{4}",
                    template.Name, result.Match.OrientationName, result.Match.MaxDeviationDeg,
                    _session.CanCreate ? "Validación correcta: puedes crear." : "Revisa los errores antes de crear.", extra));
                Canvas.Fit();
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_apply", template_id = template.TemplateId, orientation = result.Match.OrientationName, is_valid = _session.CanCreate });
            }
            catch (CatalogException ex)
            {
                UpdateStatus("No se pudo aplicar la plantilla '" + template.Name + "': " + ex.Error.Message, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_apply_failed", template_id = template.TemplateId, code = ex.Error.Code, error = ex.Error.Message });
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo aplicar la plantilla: " + ex.Message, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_apply_failed", template_id = template.TemplateId, error = ex.ToString() });
            }
        }

        /// <summary>Guarda la especificación actual (validada) como plantilla con nombre, sin los IDs del nudo.</summary>
        private void OnSaveToCatalog(object sender, RoutedEventArgs e)
        {
            if (_session.Spec == null || !_session.CanCreate)
            {
                UpdateStatus("Para guardar una plantilla la validación debe estar en verde (sin errores, con token): una plantilla no puede arrastrar errores a cada nudo.", statusIsError: true);
                return;
            }

            CatalogConfig config = CatalogConfigLoader.Load();
            var dialog = new SaveTemplateDialog(config, _session.Spec.Source?.Drawing ?? "Nudo típico", null) { Owner = this };
            if (dialog.ShowDialog() != true) return;

            try
            {
                if (!CatalogService.TryBuildFromSpec(_session.Document, _session.RawJson, _session.Spec, dialog.Metadata, config, out CatalogTemplate? template, out ModelValidation validation) || template == null)
                {
                    UpdateStatus("La especificación no valida contra el modelo: " + string.Join("; ", validation.Result.Errors.Select(err => err.Code + " " + err.Message)), statusIsError: true);
                    return;
                }
                CatalogStore store = CatalogConfigLoader.OpenStore(config);
                string? file = CatalogWindow.SaveAskingToOverwrite(store, template, dialog.Overwrite, CatalogConfigLoader.ResolveSharedFolder(config), dialog.CopyToShared, out string? sharedFile);
                if (file == null)
                {
                    UpdateStatus("No se guardó la plantilla.");
                    return;
                }
                UpdateStatus("Plantilla '" + template.Name + "' guardada en " + file + (sharedFile != null ? " y copiada a " + sharedFile : "") + ".");
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_save", template_id = template.TemplateId, name = template.Name, file, shared_file = sharedFile });
            }
            catch (CatalogException ex)
            {
                UpdateStatus("No se pudo guardar la plantilla: " + ex.Error.Message, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_save_failed", code = ex.Error.Code, error = ex.Error.Message });
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo guardar la plantilla: " + ex.Message, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_catalog_save_failed", error = ex.ToString() });
            }
        }

        private void OnCreate(object sender, RoutedEventArgs e)
        {
            // Se vuelve a validar justo antes de crear: el token que se usa es el recién calculado.
            _session.Refresh();
            RefreshAll();
            if (!_session.CanCreate) return;
            CreateRequested = true;
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CreateRequested = false;
            DialogResult = false;
            Close();
        }

        /// <summary>Fila de la tabla; <see cref="Value"/> es lo único que se edita.</summary>
        public sealed class FieldRow : INotifyPropertyChanged
        {
            private string _value;

            public FieldRow(SpecField field)
            {
                Section = field.Section;
                Label = field.Hint != null ? field.Label + "  (" + field.Hint + ")" : field.Label;
                Path = field.Path;
                IsEditable = field.IsEditable;
                OriginalValue = field.Value;
                _value = field.Value;
            }

            public string Section { get; }
            public string Label { get; }
            public string Path { get; }
            public bool IsEditable { get; }
            public string OriginalValue { get; }

            public string Value
            {
                get => _value;
                set
                {
                    if (_value == value) return;
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        /// <summary>Fila de la lista de errores y avisos (los mismos que ve la IA).</summary>
        public sealed class IssueRow
        {
            public IssueRow(string severity, ApiError error)
            {
                Severity = severity;
                Code = error.Code;
                Path = error.Path ?? string.Empty;
                Message = error.Message;
                Hint = error.Hint ?? string.Empty;
            }

            public string Severity { get; }
            public string Code { get; }
            public string Path { get; }
            public string Message { get; }
            public string Hint { get; }
        }
    }
}
