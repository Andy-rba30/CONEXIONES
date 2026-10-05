using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Lo que la ventana pide al comando con las ventanas cerradas (acciones que necesitan la vista de Revit).</summary>
    public enum PlanWindowAction
    {
        None,
        /// <summary>Seleccionar y hacer zoom al nudo; después reabrir la ventana.</summary>
        ShowInRevit,
        /// <summary>Pinchar el cordón de un nudo.</summary>
        PickChord,
        /// <summary>Pinchar barras que faltan en un nudo.</summary>
        PickMembersToAdd,
        /// <summary>Pinchar las barras de un nudo nuevo.</summary>
        PickNewNode,
    }

    /// <summary>
    /// Ventana del plan de lote (opción A de la propuesta, decisión P10: modal). Ronda 8c: se entiende sin leer el README:
    /// cabecera con la decisión (<see cref="PlanAdvice.SummaryText"/>), mapa de la cercha con un círculo por nudo del color
    /// de su estado (<see cref="TrussMapCanvas"/>), tabla solo con los nudos de verdad (las barras sueltas y las parejas sin
    /// cordón van ocultas, con contador y <b>Mostrar ocultos</b>), estados en español con icono y color, columna
    /// <b>Qué hacer</b>, cuatro botones (Replanificar, Editar nudo, Ver en Revit, Cerrar) y el resto en el menú de clic
    /// derecho del nudo y en <b>Más…</b>. Los textos salen del Core (<see cref="PlanAdvice"/>), así que la IA y la ventana
    /// dicen lo mismo. Ver en Revit y pinchar cierran la ventana y el comando la vuelve a abrir (<see cref="Action"/>).
    /// No crea nada.
    /// </summary>
    public partial class BatchPlanWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x33, 0x33, 0x33));

        private readonly Document _document;
        private readonly UIApplication? _uiApplication;
        private readonly ObservableCollection<NodeRow> _rows = new ObservableCollection<NodeRow>();
        private bool _showHidden;

        public BatchPlanWindow(Document document, UIApplication? uiApplication, BatchPlan plan, string? status = null)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _uiApplication = uiApplication;
            Plan = plan ?? throw new ArgumentNullException(nameof(plan));
            InitializeComponent();
            NodesGrid.ItemsSource = _rows;
            LegendText.Text = PlanAdvice.Legend;
            MapCanvas.NodeClicked += (_, e) => SelectRow(e.Name, showIfHidden: true);
            MapCanvas.NodeActivated += (_, e) =>
            {
                SelectRow(e.Name, showIfHidden: true);
                if (SelectedNode is PlanNode node && node.ElementIds.Count > 0) RequestAction(PlanWindowAction.ShowInRevit, node.Name);
            };
            Loaded += (_, _) =>
            {
                Refresh(status);
                MapCanvas.Fit();
                if (!string.IsNullOrEmpty(SelectNodeOnLoad)) SelectRow(SelectNodeOnLoad!, showIfHidden: true);
            };
        }

        /// <summary>El plan actual (se sustituye en cada replanificación).</summary>
        public BatchPlan Plan { get; private set; }

        /// <summary>Acción pedida al comando al cerrarse (ninguna si se cerró sin más).</summary>
        public PlanWindowAction Action { get; private set; } = PlanWindowAction.None;

        /// <summary>Nudo al que se refiere <see cref="Action"/>.</summary>
        public string? ActionNodeName { get; private set; }

        /// <summary>Verdadero si la persona descartó el plan (marcas quitadas).</summary>
        public bool Discarded { get; private set; }

        /// <summary>Nudo que se selecciona al abrir (al volver de una acción en Revit).</summary>
        public string? SelectNodeOnLoad { get; set; }

        // ---- cabecera, mapa y tabla ----

        private void Refresh(string? status = null, bool isError = false)
        {
            string? selected = SelectedRow?.Name;
            RebuildRows();
            SummaryText.Text = PlanAdvice.SummaryText(Plan);
            PlanInfoText.Text = "Plan " + Plan.PlanId.Substring(0, 8) + "… · " + Plan.SelectionIds.Count + " barras seleccionadas · "
                + (Plan.Templates.Count == 0 ? "sin plantillas en el catálogo" : "plantillas: " + string.Join(", ", Plan.Templates.Values))
                + (Plan.IsMarked ? " · marcas puestas en la vista" : " · sin marcas en el modelo");
            UpdateHiddenControls();
            CatalogButton.Visibility = PlanAdvice.IsCatalogEmpty(Plan) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            RefreshMap();

            if (status != null) SetStatus(status, isError ? ErrorBrush : OkBrush);
            else if (PlanAdvice.IsCatalogEmpty(Plan)) SetStatus(PlanAdvice.CatalogEmptyWarning().Message, ErrorBrush);
            else if (Plan.Warnings.Count > 0) SetStatus(string.Join(" · ", Plan.Warnings.Select(w => w.Message)), ErrorBrush);
            else SetStatus(Plan.ReadyCount + " nudo(s) listos con token. Crear el lote llega en la Fase 9.", InfoBrush);
            if (selected != null) SelectRow(selected, showIfHidden: false);
            UpdateButtons();
        }

        private void RebuildRows()
        {
            _rows.Clear();
            foreach (PlanNode node in Plan.Nodes.Where(PlanAdvice.VisibleByDefault)) _rows.Add(new NodeRow(node, Plan));
            if (_showHidden)
            {
                foreach (PlanNode node in Plan.Nodes.Where(n => !PlanAdvice.VisibleByDefault(n))) _rows.Add(new NodeRow(node, Plan));
            }
        }

        private void UpdateHiddenControls()
        {
            int hidden = Plan.Nodes.Count(n => !PlanAdvice.VisibleByDefault(n));
            string text = PlanAdvice.HiddenText(Plan);
            HiddenText.Text = hidden == 0
                ? "Todos los nudos de la selección son nudos de verdad."
                : "Ocultos: " + text + " (no son nudos: extremos sueltos y parejas de barras sin cordón que pase de largo).";
            HiddenToggle.Visibility = hidden == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            HiddenToggle.IsChecked = _showHidden;
            HiddenToggle.Content = (_showHidden ? "Ocultar" : "Mostrar ocultos") + " (" + hidden + ")";
        }

        private void RefreshMap()
        {
            try
            {
                MapCanvas.Map = BatchPlanner.MapOf(_document, Plan);
            }
            catch (Exception ex)
            {
                MapCanvas.Map = null;
                JsonLineLogger.Write(new { @event = "ribbon_batch_map_failed", plan_id = Plan.PlanId, error = ex.ToString() });
            }
            MapCanvas.ShowHidden = _showHidden;
            MapCanvas.SelectedNode = SelectedRow?.Name;
        }

        private void SetStatus(string text, Brush brush)
        {
            StatusText.Text = text;
            StatusText.Foreground = brush;
        }

        private NodeRow? SelectedRow => NodesGrid.SelectedItem as NodeRow;

        private PlanNode? SelectedNode => SelectedRow == null ? null : Plan.Find(SelectedRow.Name);

        private void SelectRow(string name, bool showIfHidden)
        {
            NodeRow? row = _rows.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            if (row == null && showIfHidden && !_showHidden && Plan.Find(name) is PlanNode hiddenNode && !PlanAdvice.VisibleByDefault(hiddenNode))
            {
                _showHidden = true;
                RebuildRows();
                UpdateHiddenControls();
                MapCanvas.ShowHidden = true;
                row = _rows.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            }
            if (row == null) return;
            NodesGrid.SelectedItem = row;
            NodesGrid.ScrollIntoView(row);
        }

        private void UpdateButtons()
        {
            PlanNode? node = SelectedNode;
            bool selected = node != null;
            ShowButton.IsEnabled = selected && node!.ElementIds.Count > 0;
            EditButton.IsEnabled = selected && node!.Spec != null;
            // Ronda 8b: en el PC no se pudo editar nada porque ningún nudo salió ready; el botón dice por qué está en gris.
            EditButton.ToolTip = EditButton.IsEnabled
                ? "Abre la ventana de previsualización con la especificación de este nudo; lo que cambies sustituye a la plantilla solo aquí."
                : "Editar nudo solo se activa con nudos que tienen especificación (listos o que no validan)"
                  + (node != null ? ": " + node.Name + " está " + PlanAdvice.StatusWord(node).ToLowerInvariant() + "." : ".");
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
            PlanNode? node = SelectedNode;
            MapCanvas.SelectedNode = node?.Name;
            if (node == null)
            {
                DetailText.Text = "Elige un nudo (en la tabla o en el mapa) para ver sus barras, su casado, sus avisos y sus errores. Clic derecho en un nudo: excluir, cordón, barras, plantilla.";
                return;
            }
            DetailText.Text = DetailOf(node);
        }

        private string DetailOf(PlanNode node)
        {
            var text = new StringBuilder();
            text.Append(PlanAdvice.MapLabel(node));
            if (node.StatusDetail != null) text.Append(" · ").Append(node.StatusDetail);
            text.AppendLine();
            text.Append("Qué hacer: ").Append(PlanAdvice.Advice(node, Plan)).AppendLine();
            text.Append("Estado interno: ").Append(node.Status).Append(node.Orientation != null ? " " + node.Orientation : "");
            text.Append(" · punto de trabajo (mm): ").Append(string.Join(", ", node.WorkPointMm.Select(v => v.ToString("0.0", CultureInfo.InvariantCulture))));
            if (node.ChordElementId != 0)
            {
                text.Append(" · cordón ").Append(node.ChordElementId).Append(node.ChordTypeName != null ? " " + node.ChordTypeName : "").Append(node.ChordContinuous ? " (atraviesa)" : " (llega, no pasa de largo)");
            }
            if (node.ThroughElementIds.Count > 1) text.Append(" · atraviesan: ").Append(string.Join(", ", node.ThroughElementIds));
            text.AppendLine();
            text.Append("Barras: ").Append(node.Members.Count == 0
                ? string.Join(", ", node.MemberElementIds)
                : string.Join(" · ", node.Members.Select(m => m.ElementId + " (" + m.AngleDeg.ToString("0.0", CultureInfo.InvariantCulture) + "° " + m.Side
                    + (m.EndGapMm > 0.5 ? ", se queda a " + m.EndGapMm.ToString("0.0", CultureInfo.InvariantCulture) + " mm del eje" : "")
                    + (m.ReachesNode ? "" : ", no llega") + ")")));
            text.AppendLine();
            if (node.TemplateName != null) text.Append("Plantilla: ").Append(node.TemplateName).Append(node.Orientation != null ? " (" + node.Orientation + (node.IsMirrored ? ", en espejo" : "") + ")" : "").AppendLine();
            if (node.Match?["description"] is JsonValue description) text.Append("Casado: ").Append(description.ToString()).AppendLine();
            foreach (string attempt in node.Attempts.Take(4)) text.Append("Intento: ").Append(attempt).AppendLine();
            if (node.HasSpecOverride) text.AppendLine("Especificación editada a mano (clic derecho > Quitar edición vuelve a la plantilla).");
            foreach (ApiError error in node.Errors) text.Append("ERROR ").Append(error.Code).Append(' ').Append(error.Path).Append(": ").Append(error.Message).AppendLine();
            foreach (ApiError warning in node.Warnings) text.Append("aviso ").Append(warning.Code).Append(' ').Append(warning.Path).Append(": ").Append(warning.Message).AppendLine();
            if (node.ValidationToken != null) text.Append("Validación correcta · token ").Append(node.ValidationToken.Substring(0, 16)).Append('…');
            return text.ToString().TrimEnd();
        }

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedNode?.Spec != null) OnEditNode(sender, e);
        }

        private void OnGridRightClick(object sender, MouseButtonEventArgs e)
        {
            // Clic derecho sobre una fila: se elige antes de abrir el menú, para que las acciones vayan a ese nudo.
            DependencyObject? source = e.OriginalSource as DependencyObject;
            while (source != null && source is not DataGridRow && source is not DataGrid) source = VisualTreeHelper.GetParent(source);
            if (source is DataGridRow row && row.Item is NodeRow item)
            {
                NodesGrid.SelectedItem = item;
                NodesGrid.ScrollIntoView(item);
            }
        }

        private void OnRowMenuOpened(object sender, RoutedEventArgs e)
        {
            PlanNode? node = SelectedNode;
            bool selected = node != null;
            MenuShow.IsEnabled = selected && node!.ElementIds.Count > 0;
            MenuEdit.IsEnabled = selected && node!.Spec != null;
            MenuClearEdit.IsEnabled = selected && node!.HasSpecOverride;
            MenuExclude.IsEnabled = selected;
            MenuExclude.Header = node != null && node.Status == NodeStatus.Excluded ? "Incluir" : "Excluir";
            MenuChord.IsEnabled = selected && node!.ElementIds.Count > 0;
            MenuMembers.IsEnabled = selected;
            MenuTemplate.IsEnabled = selected && node!.Status != NodeStatus.Excluded;
        }

        private void OnToggleHidden(object sender, RoutedEventArgs e)
        {
            _showHidden = HiddenToggle.IsChecked == true;
            string? selected = SelectedRow?.Name;
            RebuildRows();
            UpdateHiddenControls();
            MapCanvas.ShowHidden = _showHidden;
            if (selected != null) SelectRow(selected, showIfHidden: false);
            JsonLineLogger.Write(new { @event = "ribbon_batch_show_hidden", plan_id = Plan.PlanId, show = _showHidden });
        }

        private void OnFitMap(object sender, RoutedEventArgs e)
        {
            MapCanvas.Fit();
        }

        private void OnMore(object sender, RoutedEventArgs e)
        {
            MoreMenu.PlacementTarget = MoreButton;
            MoreMenu.Placement = PlacementMode.Top;
            MoreMenu.IsOpen = true;
        }

        // ---- acciones que necesitan Revit (cierran la ventana) ----

        private void RequestAction(PlanWindowAction action, string? nodeName)
        {
            Action = action;
            ActionNodeName = nodeName;
            DialogResult = true;
            Close();
        }

        private void OnShowInRevit(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is PlanNode node && node.ElementIds.Count > 0) RequestAction(PlanWindowAction.ShowInRevit, node.Name);
        }

        private void OnAddNode(object sender, RoutedEventArgs e)
        {
            RequestAction(PlanWindowAction.PickNewNode, null);
        }

        // ---- correcciones que replanifican aquí mismo ----

        private void Replan(BatchOverrides delta, string? status)
        {
            var warnings = new List<ApiError>();
            try
            {
                Plan = BatchPlanner.Plan(_document, _uiApplication, new BatchPlanInput { PlanId = Plan.PlanId, Overrides = delta }, warnings);
            }
            catch (CatalogException ex)
            {
                SetStatus("No se pudo replanificar: " + ex.Error.Message + (ex.Error.Hint != null ? " " + ex.Error.Hint : ""), ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_batch_replan_failed", plan_id = Plan.PlanId, code = ex.Error.Code, error = ex.Error.Message });
                return;
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo replanificar: " + ex.Message, ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_batch_replan_failed", plan_id = Plan.PlanId, error = ex.ToString() });
                return;
            }
            string text = status ?? "Replanificado.";
            var important = warnings.Where(w => w.Code != ErrorCodesRevitWarning).ToList();
            if (important.Count > 0) text += " " + string.Join(" · ", important.Select(w => w.Message));
            if (PlanAdvice.IsCatalogEmpty(Plan)) text += " " + PlanAdvice.CatalogEmptyWarning().Message;
            JsonLineLogger.Write(new { @event = "ribbon_batch_replan", plan_id = Plan.PlanId, summary = Plan.Summary(), overrides = delta.ToJson() });
            Refresh(text, important.Count > 0 || PlanAdvice.IsCatalogEmpty(Plan));
        }

        private const string ErrorCodesRevitWarning = "REVIT_WARNING";

        private void OnReplan(object sender, RoutedEventArgs e)
        {
            Replan(new BatchOverrides(), "Replanificado con la misma selección y las correcciones acumuladas.");
        }

        private void OnToggleExclude(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node) return;
            var delta = new BatchOverrides();
            if (node.Status == NodeStatus.Excluded) delta.Include.Add(node.Name);
            else delta.Exclude.Add(node.Name);
            Replan(delta, node.Status == NodeStatus.Excluded ? node.Name + " vuelve al plan." : node.Name + " excluido (en gris en el modelo).");
        }

        private void OnChord(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node) return;
            var items = node.ElementIds.Concat(node.ThroughElementIds).Distinct()
                .Select(id => new ChoiceItem(id + (id == node.ChordElementId ? "  (cordón actual)" : "") + TypeOf(id), id, id == node.ChordElementId)).ToList();
            var dialog = new ChooseDialog("cordón de " + node.Name, "Elige la barra que hace de cordón en " + node.Name + " (o pínchala en Revit).", items, false, true) { Owner = this };
            if (dialog.ShowDialog() != true) return;
            if (dialog.PickRequested)
            {
                RequestAction(PlanWindowAction.PickChord, node.Name);
                return;
            }
            if (dialog.Chosen.FirstOrDefault()?.Tag is long chordId)
            {
                var delta = new BatchOverrides();
                delta.Chord[node.Name] = chordId;
                Replan(delta, "Cordón de " + node.Name + ": " + chordId + ".");
            }
        }

        private void OnMembers(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node) return;
            var inNode = node.MemberElementIds.ToList();
            var others = Plan.SelectionIds.Where(id => !inNode.Contains(id) && id != node.ChordElementId).OrderBy(id => id).ToList();
            var items = inNode.Select(id => new ChoiceItem(id + "  (en el nudo)" + TypeOf(id), id, true))
                .Concat(others.Select(id => new ChoiceItem(id + "  (otra barra de la selección)" + TypeOf(id), id, false))).ToList();
            var dialog = new ChooseDialog("barras de " + node.Name,
                "Marca las barras que forman el nudo " + node.Name + " (sin el cordón " + node.ChordElementId + "): desmarca las que no son del nudo, marca las que faltan, o pincha en Revit las que faltan.",
                items, true, true) { Owner = this };
            if (dialog.ShowDialog() != true) return;
            if (dialog.PickRequested)
            {
                RequestAction(PlanWindowAction.PickMembersToAdd, node.Name);
                return;
            }
            var chosen = dialog.Chosen.Select(c => (long)c.Tag!).ToList();
            var delta = new BatchOverrides();
            var removed = inNode.Where(id => !chosen.Contains(id)).ToList();
            var added = chosen.Where(id => !inNode.Contains(id)).ToList();
            if (removed.Count > 0) delta.RemoveMember[node.Name] = removed;
            if (added.Count > 0) delta.AddMember[node.Name] = added;
            if (delta.IsEmpty)
            {
                SetStatus("Sin cambios en las barras de " + node.Name + ".", InfoBrush);
                return;
            }
            Replan(delta, "Barras de " + node.Name + ": " + (removed.Count > 0 ? "quitadas " + string.Join(", ", removed) + " " : "") + (added.Count > 0 ? "añadidas " + string.Join(", ", added) : ""));
        }

        private void OnTemplate(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node) return;
            bool forced = Plan.Overrides.Template.ContainsKey(node.Name);
            var items = new List<ChoiceItem>
            {
                new ChoiceItem("(automática: la que mejor case)", "auto", !forced),
                new ChoiceItem("(sin plantilla: no tocar este nudo)", "none", forced && Plan.Overrides.Template[node.Name] == null),
            };
            items.AddRange(Plan.Templates.Select(t => new ChoiceItem(t.Value + "  [" + t.Key.Substring(0, 8) + "…]", t.Key, forced && string.Equals(Plan.Overrides.Template[node.Name], t.Key, StringComparison.OrdinalIgnoreCase))));
            var dialog = new ChooseDialog("plantilla de " + node.Name, "Plantilla para " + node.Name + " (solo las que entraron en el plan).", items, false, false) { Owner = this };
            if (dialog.ShowDialog() != true || dialog.Chosen.Count == 0) return;
            string choice = (string)dialog.Chosen[0].Tag!;
            var delta = new BatchOverrides();
            if (choice == "auto")
            {
                // Olvidar la plantilla fijada: se quita de las correcciones acumuladas y se replanifica.
                Plan.Overrides.Template.Remove(node.Name);
                Replan(delta, "Plantilla automática para " + node.Name + ".");
                return;
            }
            delta.Template[node.Name] = choice == "none" ? null : choice;
            Replan(delta, choice == "none" ? node.Name + " sin plantilla." : "Plantilla de " + node.Name + " fijada.");
        }

        private void OnEditNode(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node || node.Spec == null) return;
            string original = node.SpecJson;
            string virtualPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MotorConexiones",
                "plan-" + Plan.PlanId.Substring(0, 8) + "-" + node.Name + ".json");
            string title = "Nudo " + node.Name + " del plan (" + (node.TemplateName ?? "editado") + (node.Orientation != null ? ", " + (node.IsMirrored ? "en espejo" : "igual") : "") + "). Crear se hace con el lote (Fase 9)";
            var session = new PreviewSession(_document, _uiApplication?.ActiveUIDocument, virtualPath, original, isVirtualFile: true, title: title);
            var preview = new PreviewWindow(session) { Owner = this };
            preview.ShowDialog();
            if (string.Equals(session.RawJson, original, StringComparison.Ordinal))
            {
                SetStatus(preview.CreateRequested ? "Crear un nudo suelto no está en esta ventana: el lote se crea en la Fase 9." : "Sin cambios en " + node.Name + ".", InfoBrush);
                return;
            }
            JsonObject edited;
            try
            {
                edited = TemplateJsonParse(session.RawJson);
            }
            catch (Exception ex)
            {
                SetStatus("La especificación editada no es JSON válido: " + ex.Message, ErrorBrush);
                return;
            }
            var delta = new BatchOverrides();
            delta.Spec[node.Name] = edited;
            Replan(delta, node.Name + " con especificación editada a mano" + (preview.CreateRequested ? " (crear llega con el lote, Fase 9)" : "") + ".");
        }

        private static JsonObject TemplateJsonParse(string json)
        {
            JsonNode? node = JsonNode.Parse(json, null, new System.Text.Json.JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = System.Text.Json.JsonCommentHandling.Skip });
            return node as JsonObject ?? throw new InvalidOperationException("no es un objeto JSON");
        }

        private void OnClearEdit(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node || !node.HasSpecOverride) return;
            Plan.Overrides.ClearSpec(node.Name);
            Replan(new BatchOverrides(), node.Name + " vuelve a la especificación de la plantilla.");
        }

        private string TypeOf(long id)
        {
            try
            {
                Element? element = _document.GetElement(new ElementId(id));
                if (element is FamilyInstance fi && fi.Symbol != null) return "  " + fi.Symbol.Name;
            }
            catch
            {
                // Solo decorativo.
            }
            return string.Empty;
        }

        // ---- catálogo vacío (C9) ----

        private void OnOpenCatalog(object sender, RoutedEventArgs e)
        {
            try
            {
                var catalog = new CatalogWindow(_document, _uiApplication, pickOnly: false) { Owner = this };
                catalog.ShowDialog();
                JsonLineLogger.Write(new { @event = "ribbon_batch_catalog_opened", plan_id = Plan.PlanId, create_requested = catalog.PendingSession != null });
                if (catalog.PendingSession != null)
                {
                    SetStatus("Crear desde el catálogo se hace con el botón Catálogo de la cinta (esta ventana no crea nada). Guarda primero una plantilla y replanifica.", ErrorBrush);
                    return;
                }
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo abrir el catálogo: " + ex.Message, ErrorBrush);
                return;
            }
            Replan(new BatchOverrides(), "Catálogo cerrado: replanificado con las plantillas que haya ahora.");
        }

        // ---- plan entero ----

        private void OnSavePlan(object sender, RoutedEventArgs e)
        {
            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MotorConexiones");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, "plan-" + Plan.PlanId.Substring(0, 8) + ".json");
                File.WriteAllText(path, Plan.ToJson(), new UTF8Encoding(false));
                JsonLineLogger.Write(new { @event = "ribbon_batch_plan_saved", plan_id = Plan.PlanId, file = path });
                SetStatus("Plan guardado en " + path, OkBrush);
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo guardar el plan: " + ex.Message, ErrorBrush);
            }
        }

        private void OnDiscard(object sender, RoutedEventArgs e)
        {
            var confirm = new TaskDialog("MotorConexiones - Descartar plan")
            {
                MainInstruction = "¿Descartar el plan y quitar las marcas del modelo?",
                MainContent = "Se restauran los colores de las barras y se borran los marcadores. No se toca ninguna conexión. Para volver a planificar, selecciona la cercha y pulsa Planificar lote.",
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
                DefaultButton = TaskDialogResult.No,
            };
            if (confirm.Show() != TaskDialogResult.Yes) return;
            var warnings = new List<ApiError>();
            try
            {
                BatchPlanner.Discard(_document, _uiApplication, Plan, warnings);
                Discarded = true;
                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo descartar el plan: " + ex.Message, ErrorBrush);
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>Fila de la tabla (solo lectura): todo en español y sin tokens ni IDs (eso va en el detalle).</summary>
        public sealed class NodeRow
        {
            public NodeRow(PlanNode node, BatchPlan plan)
            {
                Name = node.Name;
                StatusText = PlanAdvice.StatusText(node);
                ColorName = PlanAdvice.ColorName(node);
                var (r, g, b) = PlanAdvice.Rgb(ColorName);
                var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                brush.Freeze();
                StatusBrush = brush;
                Mirror = PlanAdvice.MirrorText(node);
                Template = node.TemplateName ?? node.TemplateId ?? "";
                Deviation = node.MaxDeviationDeg.HasValue ? node.MaxDeviationDeg.Value.ToString("0.0", CultureInfo.InvariantCulture) + "°" : "";
                Advice = PlanAdvice.Advice(node, plan);
                IsHidden = !PlanAdvice.VisibleByDefault(node);
                Detail = PlanAdvice.MapLabel(node) + (node.StatusDetail != null ? "\n" + node.StatusDetail : "")
                         + (node.Warnings.Count > 0 ? "\n" + node.Warnings.Count + " aviso(s)" : "") + (node.Errors.Count > 0 ? "\n" + node.Errors.Count + " error(es)" : "");
            }

            public string Name { get; }
            public string StatusText { get; }
            public string ColorName { get; }
            public Brush StatusBrush { get; }
            public string Mirror { get; }
            public string Template { get; }
            public string Deviation { get; }
            public string Advice { get; }
            public bool IsHidden { get; }
            public string Detail { get; }
        }
    }
}
