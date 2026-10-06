using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana del plan de lote. Desde el cierre de la ronda 8c es <b>no modal</b> (opción B de la decisión P10, mejora C7
    /// de la propuesta): se queda abierta a un lado mientras la persona orbita y pincha en Revit. Como fuera de un comando
    /// Revit no admite su API, la ventana no toca el modelo por su cuenta: todo lo que replanifica, marca, descarta, encuadra
    /// (<b>Ver en Revit</b>), pide pinchar (cordón, barras, nudo nuevo) o abre la previsualización y el catálogo pasa por
    /// <see cref="PlanEvents"/> (un <c>ExternalEvent</c>); mientras Revit trabaja los botones se apagan y la barra de estado
    /// lo dice. Cada acción devuelve un <see cref="PlanSnapshot"/> (plan, mapa y tipos de barra leídos en contexto válido) y
    /// la ventana solo pinta. La ventana de previsualización (Editar nudo) y el catálogo siguen siendo modales, dentro del
    /// evento. Hay una sola ventana del plan por sesión de Revit (<see cref="Current"/>): el botón de la cinta la reutiliza.
    /// Ronda 8c: cabecera con la decisión (<see cref="PlanAdvice.SummaryText"/>), mapa de la cercha (<see cref="TrussMapCanvas"/>),
    /// tabla solo con los nudos de verdad, estados en español, columna <b>Qué hacer</b>, cuatro botones, <b>Más…</b> y menú
    /// de clic derecho. Los textos salen del Core (<see cref="PlanAdvice"/>). No crea nada.
    /// Cierre de la ronda 8d: ninguna excepción de la ventana llega a Revit (<see cref="Guard"/> en cada manejador y una red
    /// en el despachador), pinchar en Revit se hace con la ventana <b>oculta</b> y la ventana principal de Revit activada
    /// (<see cref="PickHidingWindow{T}"/>, con una línea en el log antes y otra después), y <b>Descartar plan</b> cierra la
    /// ventana al terminar.
    /// Fase 9: <b>Crear N conexiones</b> (confirmación, <see cref="BatchCreator.Create"/> por el evento, informe por nudo en
    /// la cabecera, la tabla y la barra de estado), <b>Borrar el lote</b> (<see cref="BatchCreator.DeleteBatch"/> y
    /// replanificar) y los botones de la columna <b>Qué hacer</b> (mejora C3: <see cref="PlanAdvice.Actions"/>, cada uno hace
    /// lo mismo que la entrada del menú de clic derecho).
    /// </summary>
    public partial class BatchPlanWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x33, 0x33, 0x33));
        private static readonly Brush BusyBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1F, 0x4E, 0x9A));
        private const string ErrorCodesRevitWarning = "REVIT_WARNING";

        private static BatchPlanWindow? _current;

        private readonly ObservableCollection<NodeRow> _rows = new ObservableCollection<NodeRow>();
        private TrussMap? _map;
        private IReadOnlyDictionary<long, string> _typeNames;
        private bool _showHidden;
        private bool _busy;
        private bool _closed;
        private bool _closeWhenFree;

        public BatchPlanWindow(PlanSnapshot snapshot, string? status = null)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            Plan = snapshot.Plan;
            _map = snapshot.Map;
            _typeNames = snapshot.TypeNames;
            InitializeComponent();
            NodesGrid.ItemsSource = _rows;
            LegendText.Text = PlanAdvice.Legend;
            MapCanvas.NodeClicked += (_, e) => Guard("clic en el mapa", () => SelectRow(e.Name, showIfHidden: true));
            MapCanvas.NodeActivated += (_, e) => Guard("doble clic en el mapa", () =>
            {
                SelectRow(e.Name, showIfHidden: true);
                if (SelectedNode is PlanNode node && node.ElementIds.Count > 0) ShowInRevit(node.Name);
            });
            Loaded += (_, _) => Guard("abrir la ventana", () =>
            {
                Refresh(status);
                MapCanvas.Fit();
            });
            Closing += OnClosing;
            Closed += (_, _) =>
            {
                _closed = true;
                Dispatcher.UnhandledException -= OnDispatcherException;
                if (ReferenceEquals(_current, this)) _current = null;
                JsonLineLogger.Write(new { @event = "ribbon_batch_window", plan_id = Plan.PlanId, action = "closed", discarded = Discarded });
            };
            // Cierre de la ronda 8d: la ventana es no modal, así que una excepción que se escape de un manejador llega al
            // despachador de Revit y Revit se cierra con "fatal error" (es lo que pasó con Cordón… en la 8d). Cada manejador
            // va dentro de Guard y, por si algo se escapa (pintado, enlaces), esta red recoge solo las excepciones de esta interfaz.
            Dispatcher.UnhandledException += OnDispatcherException;
            _current = this;
        }

        /// <summary>La ventana del plan abierta en esta sesión de Revit, o nula.</summary>
        public static BatchPlanWindow? Current => _current;

        /// <summary>El plan actual (se sustituye en cada replanificación).</summary>
        public BatchPlan Plan { get; private set; }

        /// <summary>Verdadero si la persona descartó el plan (marcas quitadas).</summary>
        public bool Discarded { get; private set; }

        /// <summary>Verdadero mientras Revit ejecuta una acción pedida desde aquí (replanificar, pinchar, encuadrar…).</summary>
        public bool IsBusy => _busy;

        /// <summary>
        /// Sustituye el plan que enseña la ventana (desde el botón de la cinta, al planificar otra selección con la ventana
        /// abierta, o desde una acción del evento). Solo pinta: el modelo ya se leyó en contexto válido.
        /// </summary>
        public void Update(PlanSnapshot snapshot, string? status, bool isError = false)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (_closed) return;
            Plan = snapshot.Plan;
            _map = snapshot.Map;
            _typeNames = snapshot.TypeNames;
            Refresh(status, isError);
        }

        // ---- red de seguridad: ninguna excepción de la ventana llega a Revit (cierre de la ronda 8d) ----

        /// <summary>
        /// Ejecuta un manejador de la ventana capturando cualquier excepción: queda en el log (<c>ribbon_batch_window_error</c>)
        /// y en la barra de estado, y la ventana sigue. Con la ventana no modal, una excepción sin capturar en un manejador
        /// llega al despachador de Revit y Revit se cierra con "fatal error" (Cordón… en la ronda 8d).
        /// </summary>
        private void Guard(string action, Action work)
        {
            try
            {
                work();
            }
            catch (Exception ex)
            {
                ReportWindowError(action, ex);
            }
        }

        private void ReportWindowError(string action, Exception ex)
        {
            JsonLineLogger.Write(new { @event = "ribbon_batch_window_error", plan_id = Plan.PlanId, action, error = ex.ToString() });
            try
            {
                SetStatus("Error en la ventana (" + action + "): " + ex.Message, ErrorBrush);
            }
            catch (Exception)
            {
                // La barra de estado ya no está (ventana cerrándose): el log lo tiene.
            }
        }

        /// <summary>Red del despachador: solo recoge las excepciones que vienen de la interfaz de este add-in; las demás son de Revit.</summary>
        private void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (_closed || e.Handled) return;
            if (e.Exception.ToString().IndexOf("MotorConexiones.Revit.UI", StringComparison.Ordinal) < 0) return;
            e.Handled = true;
            ReportWindowError("despachador", e.Exception);
        }

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
            else if (Plan.LastReport != null && (Plan.CreatedCount > 0 || Plan.FailedCount > 0)) SetStatus(Plan.LastReport.SummaryText, Plan.FailedCount > 0 ? ErrorBrush : OkBrush);
            else if (Plan.CreatableCount > 0) SetStatus(Plan.CreatableCount + " nudo(s) listos con token. Pulsa " + PlanAdvice.CreateButtonText(Plan) + " para crearlos: cada nudo por separado (si uno falla, los demás se quedan) y una sola entrada de deshacer (Ctrl+Z).", InfoBrush);
            else SetStatus("Ningún nudo que crear ahora. La ventana se queda abierta: orbita y pincha en Revit cuando quieras.", InfoBrush);
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
            MapCanvas.Map = _map;
            MapCanvas.ShowHidden = _showHidden;
            MapCanvas.SelectedNode = SelectedRow?.Name;
        }

        private void SetStatus(string text, Brush brush)
        {
            if (_closed) return;
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
            bool free = !_busy;
            ReplanButton.IsEnabled = free;
            MoreButton.IsEnabled = free;
            CatalogButton.IsEnabled = free;
            ShowButton.IsEnabled = free && selected && node!.ElementIds.Count > 0;
            EditButton.IsEnabled = free && selected && node!.Spec != null;
            // Fase 9: Crear N conexiones (listos y fallidos con token) y Borrar el lote (si el plan creó algo).
            CreateButton.Content = PlanAdvice.CreateButtonText(Plan);
            CreateButton.IsEnabled = free && Plan.CreatableCount > 0;
            CreateButton.ToolTip = Plan.CreatableCount > 0
                ? "Crea las conexiones de los " + Plan.CreatableCount + " nudo(s) listos con los tokens del plan: cada nudo por separado (si uno falla, los demás se quedan y la tabla dice por qué) y una sola entrada de deshacer (Ctrl+Z)."
                : "No hay ningún nudo listo que crear: corrige los nudos en rojo, incluye los excluidos o replanifica.";
            bool hasBatch = Plan.HasBatchConnections;
            DeleteBatchButton.Visibility = hasBatch ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            DeleteBatchButton.IsEnabled = free && hasBatch;
            MenuDeleteBatch.IsEnabled = free && hasBatch;
            // Ronda 8b: en el PC no se pudo editar nada porque ningún nudo salió ready; el botón dice por qué está en gris.
            EditButton.ToolTip = _busy
                ? "Espera: Revit está con la acción anterior."
                : EditButton.IsEnabled
                    ? "Abre la ventana de previsualización con la especificación de este nudo; lo que cambies sustituye a la plantilla solo aquí."
                    : "Editar nudo solo se activa con nudos que tienen especificación (listos o que no validan)"
                      + (node != null ? ": " + node.Name + " está " + PlanAdvice.StatusWord(node).ToLowerInvariant() + "." : ".");
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => Guard("elegir un nudo", () => DoSelectionChanged(sender, e));

        private void DoSelectionChanged(object sender, SelectionChangedEventArgs e)
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
            text.Append(PlanAdvice.MapLabel(node, Plan));
            if (node.StatusDetail != null) text.Append(" · ").Append(node.StatusDetail);
            text.AppendLine();
            text.Append("Qué hacer: ").Append(PlanAdvice.Advice(node, Plan)).AppendLine();
            if (node.CreatedConnectionId != null) text.Append("Conexión creada por el lote: ").Append(node.CreatedConnectionId).AppendLine();
            if (node.ExistingConnectionId != null) text.Append("Conexión existente: ").Append(node.ExistingConnectionId).Append(node.ExistingBatchId != null ? " (lote " + node.ExistingBatchId + ")" : "").AppendLine();
            BatchNodeResult? result = Plan.LastReport?.Find(node.Name);
            if (result != null) text.Append("Último lote: ").Append(result.Describe()).AppendLine();
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

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e) => Guard("doble clic en la tabla", () => DoRowDoubleClick(sender, e));

        private void DoRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedNode?.Spec != null) OnEditNode(sender, e);
        }

        private void OnGridRightClick(object sender, MouseButtonEventArgs e) => Guard("clic derecho", () => DoGridRightClick(sender, e));

        private void DoGridRightClick(object sender, MouseButtonEventArgs e)
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

        private void OnRowMenuOpened(object sender, RoutedEventArgs e) => Guard("abrir el menú", () => DoRowMenuOpened(sender, e));

        private void DoRowMenuOpened(object sender, RoutedEventArgs e)
        {
            PlanNode? node = SelectedNode;
            bool selected = node != null && !_busy;
            MenuShow.IsEnabled = selected && node!.ElementIds.Count > 0;
            MenuEdit.IsEnabled = selected && node!.Spec != null;
            MenuClearEdit.IsEnabled = selected && node!.HasSpecOverride;
            MenuExclude.IsEnabled = selected;
            MenuExclude.Header = node != null && node.Status == NodeStatus.Excluded ? "Incluir" : "Excluir";
            MenuChord.IsEnabled = selected && node!.ElementIds.Count > 0;
            MenuMembers.IsEnabled = selected;
            MenuTemplate.IsEnabled = selected && node!.Status != NodeStatus.Excluded;
        }

        private void OnToggleHidden(object sender, RoutedEventArgs e) => Guard("Mostrar ocultos", () => DoToggleHidden(sender, e));

        private void DoToggleHidden(object sender, RoutedEventArgs e)
        {
            _showHidden = HiddenToggle.IsChecked == true;
            string? selected = SelectedRow?.Name;
            RebuildRows();
            UpdateHiddenControls();
            MapCanvas.ShowHidden = _showHidden;
            if (selected != null) SelectRow(selected, showIfHidden: false);
            JsonLineLogger.Write(new { @event = "ribbon_batch_show_hidden", plan_id = Plan.PlanId, show = _showHidden });
        }

        private void OnFitMap(object sender, RoutedEventArgs e) => Guard("Ajustar", () => DoFitMap(sender, e));

        private void DoFitMap(object sender, RoutedEventArgs e)
        {
            MapCanvas.Fit();
        }

        private void OnMore(object sender, RoutedEventArgs e) => Guard("Más…", () => DoMore(sender, e));

        private void DoMore(object sender, RoutedEventArgs e)
        {
            MoreMenu.PlacementTarget = MoreButton;
            MoreMenu.Placement = PlacementMode.Top;
            MoreMenu.IsOpen = true;
        }

        // ---- el puente con Revit: todo lo que toca el modelo pasa por aquí ----

        /// <summary>
        /// Encola <paramref name="work"/> en <see cref="PlanEvents"/> y apaga los botones hasta que Revit lo ejecute. El
        /// trabajo corre en el hilo de Revit en contexto válido (es el mismo hilo de la ventana, así que puede pintar). Un
        /// fallo se enseña en la barra de estado con <paramref name="errorPrefix"/> y queda en el log; la ventana sigue.
        /// </summary>
        private void RunInRevit(string busyText, string errorPrefix, Action<UIApplication> work)
        {
            if (_closed) return;
            if (_busy)
            {
                SetStatus("Revit todavía está con la acción anterior; espera a que termine.", InfoBrush);
                return;
            }
            SetBusy(true, busyText);
            bool accepted = PlanEvents.Run(app =>
            {
                try
                {
                    if (!_closed) work(app);
                }
                catch (CatalogException ex)
                {
                    SetStatus(errorPrefix + ": " + ex.Error.Message + (ex.Error.Hint != null ? " " + ex.Error.Hint : ""), ErrorBrush);
                    JsonLineLogger.Write(new { @event = "ribbon_batch_action_failed", plan_id = Plan.PlanId, action = busyText, code = ex.Error.Code, error = ex.Error.Message });
                }
                catch (Exception ex)
                {
                    SetStatus(errorPrefix + ": " + ex.Message, ErrorBrush);
                    JsonLineLogger.Write(new { @event = "ribbon_batch_action_failed", plan_id = Plan.PlanId, action = busyText, error = ex.ToString() });
                }
                finally
                {
                    SetBusy(false, null);
                    // Descartar plan pide cerrar al terminar. Close() dentro del trabajo no valía: OnClosing lo cancelaba
                    // porque la ventana seguía ocupada (ronda 8d, anotación 2 de la persona).
                    if (_closeWhenFree && !_closed) Close();
                }
            }, out string? reason);
            if (!accepted)
            {
                SetBusy(false, null);
                SetStatus(reason ?? "Revit no aceptó la petición.", ErrorBrush);
            }
        }

        private void SetBusy(bool busy, string? text)
        {
            if (_closed) return;
            _busy = busy;
            UpdateButtons();
            Cursor = busy ? Cursors.AppStarting : null;
            if (busy && text != null) SetStatus("⏳ " + text, BusyBrush);
        }

        /// <summary>El documento activo, que tiene que ser el del plan (la persona pudo cambiar de documento con la ventana abierta).</summary>
        private Document DocumentOf(UIApplication app)
        {
            Document? doc = app.ActiveUIDocument?.Document;
            if (doc == null) throw new InvalidOperationException("No hay ningún documento activo en Revit.");
            if (!string.IsNullOrEmpty(Plan.Document) && !string.Equals(doc.Title, Plan.Document, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("El documento activo es '" + doc.Title + "' y el plan es de '" + Plan.Document + "': vuelve a ese documento y repite.");
            }
            return doc;
        }

        /// <summary>Replanifica en contexto válido y refresca la ventana. Lo llaman todas las correcciones.</summary>
        private void ReplanNow(Document doc, UIApplication app, BatchOverrides delta, string? status)
        {
            var warnings = new List<ApiError>();
            BatchPlan plan = BatchPlanner.Plan(doc, app, new BatchPlanInput { PlanId = Plan.PlanId, Overrides = delta }, warnings);
            string text = status ?? "Replanificado.";
            var important = warnings.Where(w => w.Code != ErrorCodesRevitWarning).ToList();
            if (important.Count > 0) text += " " + string.Join(" · ", important.Select(w => w.Message));
            if (PlanAdvice.IsCatalogEmpty(plan)) text += " " + PlanAdvice.CatalogEmptyWarning().Message;
            JsonLineLogger.Write(new { @event = "ribbon_batch_replan", plan_id = plan.PlanId, summary = plan.Summary(), overrides = delta.ToJson() });
            Update(BatchPlanner.SnapshotOf(doc, plan), text, important.Count > 0 || PlanAdvice.IsCatalogEmpty(plan));
        }

        private void Replan(BatchOverrides delta, string? status)
        {
            RunInRevit("Replanificando en Revit…", "No se pudo replanificar", app => ReplanNow(DocumentOf(app), app, delta, status));
        }

        // ---- Ver en Revit y pinchar (antes cerraban la ventana; ahora no) ----

        private void ShowInRevit(string nodeName)
        {
            RunInRevit("Encuadrando " + nodeName + " en Revit…", "No se pudo encuadrar " + nodeName, app =>
            {
                DocumentOf(app);
                PlanNode node = Plan.Find(nodeName) ?? throw new InvalidOperationException("El plan ya no tiene el nudo " + nodeName + ".");
                string text = PlanZoom.ShowNode(app.ActiveUIDocument!, node);
                JsonLineLogger.Write(new { @event = "ribbon_batch_show", plan_id = Plan.PlanId, node = nodeName });
                SelectRow(nodeName, showIfHidden: true);
                SetStatus(text, OkBrush);
            });
        }

        private void OnShowInRevit(object sender, RoutedEventArgs e) => Guard("Ver en Revit", () => DoShowInRevit(sender, e));

        private void DoShowInRevit(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is PlanNode node && node.ElementIds.Count > 0) ShowInRevit(node.Name);
        }

        /// <summary>
        /// Pincha en Revit con la ventana del plan <b>oculta</b> (cierre de la ronda 8d): antes de <c>PickObject</c> se esconde la
        /// ventana y se activa la ventana principal de Revit, se pincha, y después la ventana vuelve delante. Cualquier
        /// excepción del pinchado se captura y queda en el log, con una línea antes (<c>ribbon_batch_pick</c>) y otra después
        /// (<c>ribbon_batch_picked</c> o <c>ribbon_batch_pick_failed</c>), para saber dónde se quedó Revit si vuelve a cerrarse.
        /// </summary>
        private T PickHidingWindow<T>(UIApplication app, string action, string? nodeName, Func<UIDocument, T> pick, Func<T, object?> describe)
        {
            UIDocument uidoc = app.ActiveUIDocument ?? throw new InvalidOperationException("No hay ningún documento activo en Revit.");
            long viewId = uidoc.ActiveView?.Id.Value ?? 0;
            bool activated = false;
            try
            {
                Hide();
                activated = RevitMainWindow.Activate(app);
                JsonLineLogger.Write(new { @event = "ribbon_batch_pick", plan_id = Plan.PlanId, action, node = nodeName, view = viewId, window_hidden = true, revit_activated = activated });
                T result = pick(uidoc);
                JsonLineLogger.Write(new { @event = "ribbon_batch_picked", plan_id = Plan.PlanId, action, node = nodeName, result = describe(result) });
                return result;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_batch_pick_failed", plan_id = Plan.PlanId, action, node = nodeName, revit_activated = activated, error = ex.ToString() });
                throw new InvalidOperationException("No se pudo pinchar en Revit (" + ex.GetType().Name + "): " + ex.Message, ex);
            }
            finally
            {
                if (!_closed)
                {
                    Show();
                    Activate();
                }
            }
        }

        private void PickChord(string nodeName)
        {
            RunInRevit("Pincha en Revit el cordón de " + nodeName + " (Esc cancela)…", "No se pudo elegir el cordón", app =>
            {
                Document doc = DocumentOf(app);
                Reference? picked = PickHidingWindow(app, "Cordón…", nodeName,
                    uidoc => PlanPicker.PickOne(uidoc, "Pincha el cordón del nudo " + nodeName + " (Esc para cancelar)"),
                    r => r == null ? "cancelado" : r.ElementId.Value.ToString(CultureInfo.InvariantCulture));
                if (picked == null)
                {
                    SetStatus("Sin cambios (elección cancelada).", InfoBrush);
                    return;
                }
                var delta = new BatchOverrides();
                delta.Chord[nodeName] = picked.ElementId.Value;
                ReplanNow(doc, app, delta, "Cordón de " + nodeName + ": " + picked.ElementId.Value + ".");
            });
        }

        private void PickMembersToAdd(string nodeName)
        {
            RunInRevit("Pincha en Revit las barras que faltan en " + nodeName + " y pulsa Finalizar (Esc cancela)…", "No se pudieron añadir barras", app =>
            {
                Document doc = DocumentOf(app);
                List<long> picked = PickHidingWindow(app, "Barras…", nodeName,
                    uidoc => PlanPicker.PickMany(uidoc, "Pincha las barras que faltan en el nudo " + nodeName + " y pulsa Finalizar (Esc para cancelar)"),
                    ids => ids.Count == 0 ? "cancelado" : string.Join(",", ids));
                if (picked.Count == 0)
                {
                    SetStatus("Sin cambios (elección cancelada).", InfoBrush);
                    return;
                }
                var delta = new BatchOverrides();
                delta.AddMember[nodeName] = picked;
                ReplanNow(doc, app, delta, "Añadidas a " + nodeName + ": " + string.Join(", ", picked) + ".");
            });
        }

        private void OnAddNode(object sender, RoutedEventArgs e) => Guard("Añadir nudo…", () => DoAddNode(sender, e));

        private void DoAddNode(object sender, RoutedEventArgs e)
        {
            RunInRevit("Pincha en Revit el cordón y las barras del nudo nuevo y pulsa Finalizar (Esc cancela)…", "No se pudo añadir el nudo", app =>
            {
                Document doc = DocumentOf(app);
                List<long> picked = PickHidingWindow(app, "Añadir nudo…", null,
                    uidoc => PlanPicker.PickMany(uidoc, "Pincha el cordón y las barras del nudo nuevo y pulsa Finalizar (Esc para cancelar)"),
                    ids => ids.Count == 0 ? "cancelado" : string.Join(",", ids));
                if (picked.Count == 0)
                {
                    SetStatus("Sin cambios (elección cancelada).", InfoBrush);
                    return;
                }
                if (picked.Count < 2)
                {
                    SetStatus("Un nudo necesita al menos 2 barras: no se añadió nada.", ErrorBrush);
                    return;
                }
                string name = NodeDetector.NextName(Plan.Nodes.Select(n => n.Name));
                var delta = new BatchOverrides();
                delta.AddNode[name] = picked;
                ReplanNow(doc, app, delta, "Nudo " + name + " añadido con " + picked.Count + " barras.");
                SelectRow(name, showIfHidden: true);
            });
        }

        // ---- correcciones desde la ventana (replanifican por el evento) ----

        private void OnReplan(object sender, RoutedEventArgs e) => Guard("Replanificar", () => DoReplan(sender, e));

        private void DoReplan(object sender, RoutedEventArgs e)
        {
            Replan(new BatchOverrides(), "Replanificado con la misma selección y las correcciones acumuladas.");
        }

        private void OnToggleExclude(object sender, RoutedEventArgs e) => Guard("Excluir/Incluir", () => DoToggleExclude(sender, e));

        private void DoToggleExclude(object sender, RoutedEventArgs e)
        {
            if (SelectedNode is not PlanNode node) return;
            ToggleExclude(node);
        }

        private void ToggleExclude(PlanNode node)
        {
            var delta = new BatchOverrides();
            if (node.Status == NodeStatus.Excluded) delta.Include.Add(node.Name);
            else delta.Exclude.Add(node.Name);
            Replan(delta, node.Status == NodeStatus.Excluded ? node.Name + " vuelve al plan." : node.Name + " excluido (en gris en el modelo).");
        }

        // ---- Fase 9: botones de la columna Qué hacer (mejora C3) ----

        private void OnAdviceButton(object sender, RoutedEventArgs e) => Guard("botón de Qué hacer", () => DoAdviceButton(sender, e));

        private void DoAdviceButton(object sender, RoutedEventArgs e)
        {
            if (_busy || (sender as Button)?.Tag is not AdviceAction action) return;
            SelectRow(action.Node, showIfHidden: true);
            PlanNode? node = Plan.Find(action.Node);
            if (node == null) return;
            JsonLineLogger.Write(new { @event = "ribbon_batch_advice_action", plan_id = Plan.PlanId, node = node.Name, action = action.Key });
            switch (action.Key)
            {
                case PlanAction.Exclude:
                case PlanAction.Include:
                    ToggleExclude(node);
                    break;
                case PlanAction.IncludeReplace:
                    Replan(new BatchOverrides { ReplaceExisting = true }, "Rehacer existentes activado: los nudos con conexión se planifican para rehacerla con el lote (" + node.Name + " incluido).");
                    break;
                case PlanAction.Chord:
                    DoChord(sender, e);
                    break;
                case PlanAction.Members:
                    DoMembers(sender, e);
                    break;
                case PlanAction.Template:
                    DoTemplate(sender, e);
                    break;
                case PlanAction.Edit:
                    DoEditNode(sender, e);
                    break;
                case PlanAction.Show:
                    if (node.ElementIds.Count > 0) ShowInRevit(node.Name);
                    break;
                case PlanAction.Replan:
                    DoReplan(sender, e);
                    break;
                case PlanAction.Catalog:
                    DoOpenCatalog(sender, e);
                    break;
                default:
                    SetStatus("Acción desconocida: " + action.Key, ErrorBrush);
                    break;
            }
        }

        // ---- Fase 9: crear el lote y borrarlo ----

        private void OnCreateBatch(object sender, RoutedEventArgs e) => Guard("Crear N conexiones", () => DoCreateBatch(sender, e));

        private void DoCreateBatch(object sender, RoutedEventArgs e)
        {
            if (_busy) return;
            int count = Plan.CreatableCount;
            if (count == 0)
            {
                SetStatus("No hay ningún nudo listo que crear.", InfoBrush);
                return;
            }
            MessageBoxResult confirm = MessageBox.Show(this,
                PlanAdvice.SummaryText(Plan) + "\n\n"
                + "Se crearán " + count + " conexión(es) con los tokens del plan, cada nudo por separado: si uno falla, los demás se quedan creados y la tabla dice cuál falló y por qué. "
                + "Todo el lote es una sola entrada de deshacer (Ctrl+Z lo deshace entero). Las marcas de los nudos creados se quitan.\n\n"
                + "Puede tardar varios minutos (cada nudo abre su sesión de Advance Steel). ¿Crear ahora?",
                "MotorConexiones - " + PlanAdvice.CreateButtonText(Plan), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (confirm != MessageBoxResult.Yes) return;
            RunInRevit("Creando " + count + " conexión(es) en Revit (puede tardar varios minutos)…", "No se pudo crear el lote", app =>
            {
                Document doc = DocumentOf(app);
                var warnings = new List<ApiError>();
                BatchCreateRequest request = BatchCreateRequest.ForPlan(Plan);
                BatchReport report = BatchCreator.Create(doc, app, Plan, request, warnings);
                var important = warnings.Where(w => w.Code != ErrorCodesRevitWarning && w.Code != ErrorCodes.BatchNodeFailed).ToList();
                string text = report.SummaryText + (important.Count > 0 ? " " + string.Join(" · ", important.Select(w => w.Message)) : "");
                JsonLineLogger.Write(new { @event = "ribbon_batch_create", plan_id = Plan.PlanId, requested = request.Items.Count, created = report.CreatedCount, failed = report.FailedCount, skipped = report.SkippedCount, undo_entries = report.UndoEntries, duration_ms = report.DurationMs });
                Update(BatchPlanner.SnapshotOf(doc, Plan), text, report.FailedCount > 0 || report.Stopped || important.Count > 0);
            });
        }

        private void OnDeleteBatch(object sender, RoutedEventArgs e) => Guard("Borrar el lote", () => DoDeleteBatch(sender, e));

        private void DoDeleteBatch(object sender, RoutedEventArgs e)
        {
            if (_busy) return;
            int known = Math.Max(Plan.CreatedCount + Plan.CreatedInBatchCount, Plan.LastReport != null && !Plan.LastReport.IsDelete ? Plan.LastReport.ConnectionIds.Count : 0);
            MessageBoxResult confirm = MessageBox.Show(this,
                "¿Borrar todas las conexiones creadas por este plan (lote " + Plan.PlanId.Substring(0, 8) + "…; según el plan, " + known + ")?\n\n"
                + "Se borra solo lo que creó el add-in (cartelas, placas, pernos, soldaduras y registros) y las barras recuperan su extensión original. "
                + "Una sola entrada de deshacer. Después la ventana replanifica.",
                "MotorConexiones - Borrar el lote", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (confirm != MessageBoxResult.Yes) return;
            RunInRevit("Borrando las conexiones del lote en Revit…", "No se pudo borrar el lote", app =>
            {
                Document doc = DocumentOf(app);
                var warnings = new List<ApiError>();
                BatchReport report = BatchCreator.DeleteBatch(doc, app, Plan.PlanId, warnings);
                JsonLineLogger.Write(new { @event = "ribbon_batch_delete", plan_id = Plan.PlanId, deleted = report.DeletedCount, failed = report.FailedCount, duration_ms = report.DurationMs });
                string text = report.SummaryText + (warnings.Any(w => w.Code == ErrorCodes.BatchEmpty) ? " No había ninguna conexión de este lote en el modelo." : "") + " Replanificado.";
                ReplanNow(doc, app, new BatchOverrides(), text);
            });
        }

        private void OnChord(object sender, RoutedEventArgs e) => Guard("Cordón…", () => DoChord(sender, e));

        private void DoChord(object sender, RoutedEventArgs e)
        {
            if (_busy || SelectedNode is not PlanNode node) return;
            var items = node.ElementIds.Concat(node.ThroughElementIds).Distinct()
                .Select(id => new ChoiceItem(id + (id == node.ChordElementId ? "  (cordón actual)" : "") + TypeOf(id), id, id == node.ChordElementId)).ToList();
            var dialog = new ChooseDialog("cordón de " + node.Name, "Elige la barra que hace de cordón en " + node.Name + " (o pínchala en Revit).", items, false, true) { Owner = this };
            if (dialog.ShowDialog() != true) return;
            if (dialog.PickRequested)
            {
                PickChord(node.Name);
                return;
            }
            if (dialog.Chosen.FirstOrDefault()?.Tag is long chordId)
            {
                var delta = new BatchOverrides();
                delta.Chord[node.Name] = chordId;
                Replan(delta, "Cordón de " + node.Name + ": " + chordId + ".");
            }
        }

        private void OnMembers(object sender, RoutedEventArgs e) => Guard("Barras…", () => DoMembers(sender, e));

        private void DoMembers(object sender, RoutedEventArgs e)
        {
            if (_busy || SelectedNode is not PlanNode node) return;
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
                PickMembersToAdd(node.Name);
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

        private void OnTemplate(object sender, RoutedEventArgs e) => Guard("Plantilla…", () => DoTemplate(sender, e));

        private void DoTemplate(object sender, RoutedEventArgs e)
        {
            if (_busy || SelectedNode is not PlanNode node) return;
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
            string nodeName = node.Name;
            if (choice == "auto")
            {
                // Olvidar la plantilla fijada: se quita de las correcciones acumuladas (en contexto válido, justo antes de replanificar).
                RunInRevit("Replanificando en Revit…", "No se pudo replanificar", app =>
                {
                    Document doc = DocumentOf(app);
                    Plan.Overrides.Template.Remove(nodeName);
                    ReplanNow(doc, app, new BatchOverrides(), "Plantilla automática para " + nodeName + ".");
                });
                return;
            }
            var delta = new BatchOverrides();
            delta.Template[nodeName] = choice == "none" ? null : choice;
            Replan(delta, choice == "none" ? nodeName + " sin plantilla." : "Plantilla de " + nodeName + " fijada.");
        }

        private void OnEditNode(object sender, RoutedEventArgs e) => Guard("Editar nudo", () => DoEditNode(sender, e));

        private void DoEditNode(object sender, RoutedEventArgs e)
        {
            if (_busy || SelectedNode is not PlanNode selected || selected.Spec == null) return;
            string nodeName = selected.Name;
            // La previsualización sigue siendo modal, pero dentro del evento: valida contra el modelo (contexto válido).
            RunInRevit("Editando " + nodeName + " en la ventana de previsualización…", "No se pudo editar " + nodeName, app =>
            {
                Document doc = DocumentOf(app);
                PlanNode node = Plan.Find(nodeName) ?? throw new InvalidOperationException("El plan ya no tiene el nudo " + nodeName + ".");
                if (node.Spec == null) throw new InvalidOperationException(nodeName + " ya no tiene especificación.");
                string original = node.SpecJson;
                string virtualPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MotorConexiones",
                    "plan-" + Plan.PlanId.Substring(0, 8) + "-" + node.Name + ".json");
                string title = "Nudo " + node.Name + " del plan (" + (node.TemplateName ?? "editado") + (node.Orientation != null ? ", " + (node.IsMirrored ? "en espejo" : "igual") : "") + "). Crear se hace con el botón Crear N conexiones";
                var session = new PreviewSession(doc, app.ActiveUIDocument, virtualPath, original, isVirtualFile: true, title: title);
                var preview = new PreviewWindow(session) { Owner = this };
                preview.ShowDialog();
                if (string.Equals(session.RawJson, original, StringComparison.Ordinal))
                {
                    SetStatus(preview.CreateRequested ? "Crear un nudo suelto no está en esta ventana: pulsa Crear N conexiones para crear el lote." : "Sin cambios en " + node.Name + ".", InfoBrush);
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
                ReplanNow(doc, app, delta, node.Name + " con especificación editada a mano" + (preview.CreateRequested ? " (se creará con Crear N conexiones)" : "") + ".");
            });
        }

        private static JsonObject TemplateJsonParse(string json)
        {
            JsonNode? node = JsonNode.Parse(json, null, new System.Text.Json.JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = System.Text.Json.JsonCommentHandling.Skip });
            return node as JsonObject ?? throw new InvalidOperationException("no es un objeto JSON");
        }

        private void OnClearEdit(object sender, RoutedEventArgs e) => Guard("Quitar edición", () => DoClearEdit(sender, e));

        private void DoClearEdit(object sender, RoutedEventArgs e)
        {
            if (_busy || SelectedNode is not PlanNode node || !node.HasSpecOverride) return;
            string nodeName = node.Name;
            RunInRevit("Replanificando en Revit…", "No se pudo quitar la edición", app =>
            {
                Document doc = DocumentOf(app);
                Plan.Overrides.ClearSpec(nodeName);
                ReplanNow(doc, app, new BatchOverrides(), nodeName + " vuelve a la especificación de la plantilla.");
            });
        }

        /// <summary>Nombre del tipo de una barra, leído con el plan (la ventana no consulta el modelo).</summary>
        private string TypeOf(long id) => _typeNames.TryGetValue(id, out string? name) && !string.IsNullOrEmpty(name) ? "  " + name : string.Empty;

        // ---- catálogo vacío (C9) ----

        private void OnOpenCatalog(object sender, RoutedEventArgs e) => Guard("Abrir catálogo", () => DoOpenCatalog(sender, e));

        private void DoOpenCatalog(object sender, RoutedEventArgs e)
        {
            RunInRevit("Catálogo abierto…", "No se pudo abrir el catálogo", app =>
            {
                Document doc = DocumentOf(app);
                var catalog = new CatalogWindow(doc, app, pickOnly: false) { Owner = this };
                catalog.ShowDialog();
                JsonLineLogger.Write(new { @event = "ribbon_batch_catalog_opened", plan_id = Plan.PlanId, create_requested = catalog.PendingSession != null });
                if (catalog.PendingSession != null)
                {
                    SetStatus("Crear desde el catálogo se hace con el botón Catálogo de la cinta (esta ventana no crea nada). Guarda primero una plantilla y replanifica.", ErrorBrush);
                    return;
                }
                ReplanNow(doc, app, new BatchOverrides(), "Catálogo cerrado: replanificado con las plantillas que haya ahora.");
            });
        }

        // ---- plan entero ----

        private void OnSavePlan(object sender, RoutedEventArgs e) => Guard("Guardar plan JSON", () => DoSavePlan(sender, e));

        private void DoSavePlan(object sender, RoutedEventArgs e)
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

        private void OnDiscard(object sender, RoutedEventArgs e) => Guard("Descartar plan", () => DoDiscard(sender, e));

        private void DoDiscard(object sender, RoutedEventArgs e)
        {
            if (_busy) return;
            MessageBoxResult confirm = MessageBox.Show(this,
                "¿Descartar el plan y quitar todas las marcas del modelo?\n\nSe restauran los colores de las barras y se borran todos los marcadores (cubos y rombos), también los de otros planes. No se toca ninguna conexión. Para volver a planificar, selecciona la cercha y pulsa Planificar lote.",
                "MotorConexiones - Descartar plan", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (confirm != MessageBoxResult.Yes) return;
            RunInRevit("Descartando el plan y quitando las marcas…", "No se pudo descartar el plan", app =>
            {
                Document doc = DocumentOf(app);
                var warnings = new List<ApiError>();
                DiscardResult result = BatchPlanner.DiscardAndClean(doc, app, Plan, warnings);
                Discarded = true;
                _closeWhenFree = true;
                JsonLineLogger.Write(new { @event = "ribbon_batch_discard", plan_id = Plan.PlanId, result = result.Describe(), warnings = warnings.Count, closes_window = true });
            });
        }

        private void OnClose(object sender, RoutedEventArgs e) => Guard("Cerrar", () => DoClose(sender, e));

        private void DoClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (!_busy) return;
            // Con una elección en marcha en Revit (pinchar) o una ventana modal abierta, cerrar dejaría el evento a medias.
            e.Cancel = true;
            SetStatus("Termina primero la acción en Revit (Esc cancela una elección) y después cierra.", InfoBrush);
        }

        /// <summary>Fila de la tabla (solo lectura): todo en español y sin tokens ni IDs (eso va en el detalle).</summary>
        public sealed class NodeRow
        {
            public NodeRow(PlanNode node, BatchPlan plan)
            {
                Name = node.Name;
                StatusText = PlanAdvice.StatusText(node, plan);
                ColorName = PlanAdvice.ColorName(node);
                var (r, g, b) = PlanAdvice.Rgb(ColorName);
                var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                brush.Freeze();
                StatusBrush = brush;
                Mirror = PlanAdvice.MirrorText(node);
                Template = node.TemplateName ?? node.TemplateId ?? "";
                Deviation = node.MaxDeviationDeg.HasValue ? node.MaxDeviationDeg.Value.ToString("0.0", CultureInfo.InvariantCulture) + "°" : "";
                Advice = PlanAdvice.Advice(node, plan);
                Actions = PlanAdvice.Actions(node, plan).Select(a => new AdviceAction(node.Name, a)).ToList();
                IsHidden = !PlanAdvice.VisibleByDefault(node);
                Detail = PlanAdvice.MapLabel(node, plan) + (node.StatusDetail != null ? "\n" + node.StatusDetail : "")
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
            public List<AdviceAction> Actions { get; }
            public bool IsHidden { get; }
            public string Detail { get; }
        }

        /// <summary>Un botón de la columna Qué hacer (Fase 9, C3): la acción del Core y el nudo al que se aplica.</summary>
        public sealed class AdviceAction
        {
            public AdviceAction(string node, PlanAction action)
            {
                Node = node;
                Key = action.Key;
                Label = action.Label;
            }

            public string Node { get; }
            public string Key { get; }
            public string Label { get; }
            public string ToolTip => Label + " " + Node + " (lo mismo que la entrada del menú de clic derecho).";
        }
    }
}
