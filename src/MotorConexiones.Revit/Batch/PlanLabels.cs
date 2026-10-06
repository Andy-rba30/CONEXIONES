using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Etiquetas con número en la vista (Fase 10, mejora V3 de <c>docs/propuestas/flujo-intuitivo.md</c>), sobre lo que el
    /// sondeo 19 v4 demostró en el PC: un control con imagen en el lienzo (<see cref="TemporaryGraphicsManager"/> +
    /// <see cref="InCanvasControlData"/>, BMP de 24 bits de 32×32 en una ruta sin tildes ni espacios) en el punto de trabajo de
    /// cada nudo visible del plan, con el número y el color de su estado (<see cref="LabelImage"/>), un globo
    /// (<c>SetTooltip</c>) y un manejador de clics (<see cref="PlanLabelHandler"/>) que <b>nunca abre un cuadro</b>: cambia la
    /// etiqueta pinchada por su versión resaltada (<c>UpdateControl</c>), elige esa fila en la ventana del plan y lo escribe en
    /// su barra de estado. Las etiquetas no son elementos del modelo (no pasan por transacciones ni por Deshacer y se van al
    /// cerrar el documento): se quitan aquí con <c>RemoveControl</c> al replanificar, al crear el lote (las de los creados) y
    /// al descartar; <see cref="Clear"/> deja el documento sin ninguna. Todo se llama en contexto válido de la API (comando,
    /// evento externo, ruta de pyRevit o el propio clic). Nada de aquí lanza hacia fuera: los fallos van al log y a los avisos.
    /// </summary>
    public static class PlanLabels
    {
        /// <summary>Identificador fijo del servidor de clics (el sondeo 19 usó otro GUID: los dos pueden convivir).</summary>
        public static readonly Guid ServerId = new Guid("3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d");

        public const string ServerName = "MotorConexiones: etiquetas del plan";
        public const string VendorId = "ARBA";

        private static readonly object Gate = new object();
        private static readonly Dictionary<string, string> Highlighted = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static string? _folder;
        private static bool _handlerRegistered;

        /// <summary>
        /// Carpeta de los BMP: la primera en la que se puede escribir, sin tildes ni espacios (el sondeo 19 v3 necesitó una ruta
        /// así): <c>%LOCALAPPDATA%\MotorConexiones\etiquetas</c>, <c>%PUBLIC%\MotorConexiones\etiquetas</c>,
        /// <c>C:\MotorConexiones\etiquetas</c> o la temporal. Se decide una vez y queda en el log (<c>label_folder</c>).
        /// </summary>
        public static string Folder
        {
            get
            {
                lock (Gate)
                {
                    if (_folder != null) return _folder;
                    var candidates = new List<string>();
                    try { candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotorConexiones", "etiquetas")); } catch { }
                    try
                    {
                        string? publicFolder = Environment.GetEnvironmentVariable("PUBLIC");
                        if (!string.IsNullOrEmpty(publicFolder)) candidates.Add(Path.Combine(publicFolder, "MotorConexiones", "etiquetas"));
                    }
                    catch { }
                    candidates.Add(@"C:\MotorConexiones\etiquetas");
                    try { candidates.Add(Path.Combine(Path.GetTempPath(), "MotorConexiones-etiquetas")); } catch { }
                    foreach (string candidate in candidates)
                    {
                        if (!IsPlainPath(candidate)) continue;
                        try
                        {
                            Directory.CreateDirectory(candidate);
                            string probe = Path.Combine(candidate, "escritura.tmp");
                            File.WriteAllText(probe, "ok");
                            File.Delete(probe);
                            _folder = candidate;
                            break;
                        }
                        catch
                        {
                            // Siguiente candidata.
                        }
                    }
                    _folder ??= candidates.Last();
                    JsonLineLogger.Write(new { @event = "label_folder", folder = _folder, plain = IsPlainPath(_folder) });
                    return _folder;
                }
            }
        }

        /// <summary>Solo ASCII, sin espacios: lo que Revit aceptó en el sondeo 19.</summary>
        public static bool IsPlainPath(string path) => !string.IsNullOrEmpty(path) && path.All(c => c < 128 && c != ' ');

        // ---- poner y quitar ----

        /// <summary>
        /// Pone una etiqueta por nudo visible (<see cref="PlanNode.CanBeMarked"/>) en la vista marcada del plan (o la activa) y
        /// anota en el plan la vista y los índices. Devuelve cuántas puso; si algo falla, aviso <c>PLAN_LABELS_SKIPPED</c>.
        /// </summary>
        public static int Apply(Document document, UIDocument? uidoc, BatchPlan plan, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            warnings ??= new List<ApiError>();
            Remove(document, plan, warnings);
            View? view = plan.MarkedViewId.HasValue ? document.GetElement(new ElementId(plan.MarkedViewId.Value)) as View : document.ActiveView;
            if (view == null || !view.IsValidObject || view.IsTemplate)
            {
                warnings.Add(new ApiError(ErrorCodes.PlanLabelsSkipped, "La vista del plan no admite etiquetas en el lienzo: el plan sigue con colores y marcadores.",
                    hint: "Abre una vista 3D o de estructura y replanifica para ver las etiquetas."));
                return 0;
            }
            int count = 0;
            try
            {
                TemporaryGraphicsManager manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
                string folder = Folder;
                foreach (PlanNode node in plan.Nodes.Where(n => n.CanBeMarked))
                {
                    string number = LabelImage.NumberOf(node.Name);
                    string color = PlanAdvice.ColorName(node);
                    string path = LabelImage.EnsureFile(folder, number, color, highlighted: false);
                    XYZ position = RevitGeometry.ToFeet(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
                    int index = manager.AddControl(new InCanvasControlData(path, position), view.Id);
                    node.LabelIndex = index;
                    plan.LabelIndices[node.Name] = index;
                    count++;
                    try
                    {
                        manager.SetTooltip(index, PlanAdvice.MapLabel(node, plan) + " · pincha para elegirlo en la ventana del plan");
                    }
                    catch (Exception)
                    {
                        // El globo es decorativo.
                    }
                }
                plan.LabelViewId = count > 0 ? view.Id.Value : (long?)null;
                lock (Gate) Highlighted.Remove(plan.PlanId);
                Refresh(document, uidoc);
                JsonLineLogger.Write(new { @event = "labels_applied", plan_id = plan.PlanId, view = view.Id.Value, count, folder, total_in_document = Count(document) });
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "labels_failed", plan_id = plan.PlanId, count, error = ex.ToString() });
                warnings.Add(new ApiError(ErrorCodes.PlanLabelsSkipped, "No se pudieron poner las etiquetas en la vista (" + ex.GetType().Name + ": " + ex.Message + "): el plan sigue con colores y marcadores.",
                    hint: "Mira la línea labels_failed del registro; config\\catalog.json plan_labels: false las desactiva."));
                try
                {
                    Remove(document, plan, warnings);
                }
                catch (Exception)
                {
                    // Ya está anotado.
                }
            }
            return count;
        }

        /// <summary>Quita todas las etiquetas del plan (solo las que <c>GetAll()</c> dice que existen). Devuelve cuántas quitó.</summary>
        public static int Remove(Document document, BatchPlan plan, List<ApiError> warnings)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            int removed = RemoveIndices(document, plan.LabelIndices.Values.ToList(), plan.PlanId, warnings);
            plan.LabelIndices.Clear();
            plan.LabelViewId = null;
            foreach (PlanNode node in plan.Nodes) node.LabelIndex = null;
            lock (Gate) Highlighted.Remove(plan.PlanId);
            return removed;
        }

        /// <summary>Quita las etiquetas de unos nudos (los creados por el lote); las de los demás siguen.</summary>
        public static int RemoveNodes(Document document, BatchPlan plan, IEnumerable<PlanNode> nodes, List<ApiError> warnings)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var chosen = (nodes ?? Enumerable.Empty<PlanNode>()).Where(n => n.LabelIndex.HasValue).ToList();
            if (chosen.Count == 0) return 0;
            int removed = RemoveIndices(document, chosen.Select(n => n.LabelIndex!.Value).ToList(), plan.PlanId, warnings);
            foreach (PlanNode node in chosen)
            {
                plan.LabelIndices.Remove(node.Name);
                node.LabelIndex = null;
                lock (Gate)
                {
                    if (Highlighted.TryGetValue(plan.PlanId, out string? name) && string.Equals(name, node.Name, StringComparison.OrdinalIgnoreCase)) Highlighted.Remove(plan.PlanId);
                }
            }
            if (plan.LabelIndices.Count == 0) plan.LabelViewId = null;
            return removed;
        }

        /// <summary>Quita <b>todas</b> las etiquetas del lienzo del documento (de cualquier plan, también los olvidados). Devuelve cuántas había.</summary>
        public static int Clear(Document document, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            int before = Count(document);
            try
            {
                TemporaryGraphicsManager manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
                manager.Clear();
                foreach (BatchPlan plan in PlanRegistry.All().Where(p => string.Equals(p.Document, document.Title, StringComparison.OrdinalIgnoreCase)))
                {
                    plan.LabelIndices.Clear();
                    plan.LabelViewId = null;
                    foreach (PlanNode node in plan.Nodes) node.LabelIndex = null;
                    lock (Gate) Highlighted.Remove(plan.PlanId);
                }
                Refresh(document, null);
                JsonLineLogger.Write(new { @event = "labels_cleared", before, after = Count(document) });
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "labels_clear_failed", error = ex.ToString() });
                warnings?.Add(new ApiError(ErrorCodes.PlanLabelsSkipped, "No se pudieron quitar todas las etiquetas del lienzo: " + ex.Message, hint: "Se van solas al cerrar el documento."));
            }
            return before;
        }

        /// <summary>Cuántos controles hay en el lienzo del documento (de quien sea); 0 si no se puede leer.</summary>
        public static int Count(Document document)
        {
            try
            {
                return TemporaryGraphicsManager.GetTemporaryGraphicsManager(document).GetAll().Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static int RemoveIndices(Document document, List<int> indices, string planId, List<ApiError> warnings)
        {
            if (indices.Count == 0) return 0;
            int removed = 0;
            try
            {
                TemporaryGraphicsManager manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
                var present = new HashSet<int>(manager.GetAll());
                foreach (int index in indices.Distinct())
                {
                    if (!present.Contains(index)) continue;
                    try
                    {
                        manager.RemoveControl(index);
                        removed++;
                    }
                    catch (Exception ex)
                    {
                        warnings?.Add(new ApiError(ErrorCodes.RevitWarning, "No se pudo quitar la etiqueta " + index + " del plan " + planId + ": " + ex.Message));
                    }
                }
                Refresh(document, null);
                JsonLineLogger.Write(new { @event = "labels_removed", plan_id = planId, requested = indices.Count, removed, remaining_in_document = Count(document) });
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "labels_remove_failed", plan_id = planId, error = ex.ToString() });
                warnings?.Add(new ApiError(ErrorCodes.PlanLabelsSkipped, "No se pudieron quitar las etiquetas del plan " + planId + ": " + ex.Message, hint: "Se van solas al cerrar el documento."));
            }
            return removed;
        }

        // ---- resaltar ----

        /// <summary>El nudo cuya etiqueta está resaltada en este plan, o nulo.</summary>
        public static string? HighlightedNode(BatchPlan plan)
        {
            lock (Gate) return plan != null && Highlighted.TryGetValue(plan.PlanId, out string? name) ? name : null;
        }

        /// <summary>
        /// Resalta la etiqueta de <paramref name="nodeName"/> (versión invertida) y devuelve a su aspecto la que estaba resaltada.
        /// Con <paramref name="nodeName"/> nulo solo quita el resaltado. Devuelve verdadero si cambió algo. No lanza.
        /// </summary>
        public static bool Highlight(Document document, BatchPlan plan, string? nodeName, List<ApiError>? warnings = null)
        {
            if (document == null || plan == null || plan.LabelIndices.Count == 0) return false;
            string? previous = HighlightedNode(plan);
            if (string.Equals(previous, nodeName, StringComparison.OrdinalIgnoreCase)) return false;
            try
            {
                TemporaryGraphicsManager manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
                var present = new HashSet<int>(manager.GetAll());
                bool changed = false;
                if (previous != null && plan.Find(previous) is PlanNode old && old.LabelIndex.HasValue && present.Contains(old.LabelIndex.Value))
                {
                    manager.UpdateControl(old.LabelIndex.Value, DataFor(old, highlighted: false));
                    changed = true;
                }
                lock (Gate) Highlighted.Remove(plan.PlanId);
                if (nodeName != null && plan.Find(nodeName) is PlanNode node && node.LabelIndex.HasValue && present.Contains(node.LabelIndex.Value))
                {
                    manager.UpdateControl(node.LabelIndex.Value, DataFor(node, highlighted: true));
                    lock (Gate) Highlighted[plan.PlanId] = node.Name;
                    changed = true;
                }
                if (changed) Refresh(document, null);
                return changed;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "label_highlight_failed", plan_id = plan.PlanId, node = nodeName, error = ex.ToString() });
                warnings?.Add(new ApiError(ErrorCodes.PlanLabelsSkipped, "No se pudo resaltar la etiqueta de " + nodeName + ": " + ex.Message));
                return false;
            }
        }

        private static InCanvasControlData DataFor(PlanNode node, bool highlighted)
        {
            string path = LabelImage.EnsureFile(Folder, LabelImage.NumberOf(node.Name), PlanAdvice.ColorName(node), highlighted);
            XYZ position = RevitGeometry.ToFeet(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
            return new InCanvasControlData(path, position);
        }

        /// <summary>El plan (de este documento) y el nudo cuya etiqueta tiene ese índice, o nulos.</summary>
        public static (BatchPlan? Plan, PlanNode? Node) FindByIndex(string? documentTitle, int index)
        {
            foreach (BatchPlan plan in PlanRegistry.All())
            {
                if (documentTitle != null && !string.Equals(plan.Document, documentTitle, StringComparison.OrdinalIgnoreCase)) continue;
                PlanNode? node = plan.Nodes.FirstOrDefault(n => n.LabelIndex.HasValue && n.LabelIndex.Value == index);
                if (node != null) return (plan, node);
            }
            return (null, null);
        }

        // ---- el clic ----

        /// <summary>
        /// Lo que hace el manejador al pinchar una etiqueta (sin ningún cuadro): busca el plan y el nudo por el índice, resalta
        /// la etiqueta, lo anota en el log y avisa a la ventana del plan si está abierta con ese plan (elige la fila y escribe en
        /// la barra de estado). Devuelve el nudo pinchado o nulo si el índice no es de ningún plan en memoria.
        /// </summary>
        public static PlanNode? OnClick(Document document, int index)
        {
            var (plan, node) = FindByIndex(document?.Title, index);
            if (plan == null || node == null)
            {
                JsonLineLogger.Write(new { @event = "label_clicked", index, document = document?.Title, plan_id = (string?)null, node = (string?)null, known = false });
                return null;
            }
            bool highlighted = Highlight(document!, plan, node.Name);
            JsonLineLogger.Write(new { @event = "label_clicked", index, document = document!.Title, plan_id = plan.PlanId, node = node.Name, status = node.Status, highlighted, window_open = UI.BatchPlanWindow.Current != null });
            try
            {
                UI.BatchPlanWindow.Current?.OnLabelClicked(plan.PlanId, node.Name);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "label_click_window_failed", plan_id = plan.PlanId, node = node.Name, error = ex.ToString() });
            }
            return node;
        }

        // ---- el manejador de clics ----

        /// <summary>
        /// Registra el manejador de clics (<see cref="PlanLabelHandler"/>) en el servicio <c>TemporaryGraphicsHandlerService</c>
        /// y lo deja activo sin desactivar a los demás servidores. Solo en contexto válido de la API (arranque del add-in o
        /// comando). Devuelve falso, con el motivo, si no se pudo; sin manejador las etiquetas se ven pero no responden al clic.
        /// </summary>
        public static bool EnsureHandlerRegistered(out string? error)
        {
            error = null;
            lock (Gate)
            {
                if (_handlerRegistered) return true;
            }
            try
            {
                ExternalService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService);
                bool registered = service.GetRegisteredServerIds().Contains(ServerId);
                if (!registered) service.AddServer(new PlanLabelHandler());
                string activation;
                if (service is MultiServerService multi)
                {
                    IList<Guid> active = multi.GetActiveServerIds();
                    if (!active.Contains(ServerId))
                    {
                        var ids = new List<Guid>(active) { ServerId };
                        multi.SetActiveServers(ids);
                    }
                    activation = "multi:" + multi.GetActiveServerIds().Count;
                }
                else if (service is SingleServerService single)
                {
                    single.SetActiveServer(ServerId);
                    activation = "single";
                }
                else
                {
                    activation = "unknown:" + service.GetType().Name;
                }
                lock (Gate) _handlerRegistered = true;
                JsonLineLogger.Write(new { @event = "label_handler_registered", server = ServerId, already_registered = registered, service = service.Name, activation });
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetType().Name + ": " + ex.Message;
                JsonLineLogger.Write(new { @event = "label_handler_failed", error = ex.ToString() });
                return false;
            }
        }

        private static void Refresh(Document document, UIDocument? uidoc)
        {
            try
            {
                (uidoc ?? new UIDocument(document)).RefreshActiveView();
            }
            catch (Exception)
            {
                // El refresco es cortesía: la etiqueta se pinta igual al siguiente redibujado.
            }
        }
    }

    /// <summary>
    /// El servidor que recibe los clics de las etiquetas del lienzo (Fase 10, sobre el sondeo 19 v4): <b>nunca abre un
    /// cuadro</b> (el <c>TaskDialog</c> de la v2 era modal y dejó a Revit sin atender a pyRevit un cuarto de hora) y nunca deja
    /// escapar una excepción al hilo de Revit. Todo lo que hace está en <see cref="PlanLabels.OnClick"/>.
    /// </summary>
    public sealed class PlanLabelHandler : ITemporaryGraphicsHandler
    {
        public Guid GetServerId() => PlanLabels.ServerId;

        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService;

        public string GetName() => PlanLabels.ServerName;

        public string GetVendorId() => PlanLabels.VendorId;

        public string GetDescription() => "Clic en una etiqueta del plan de lote: resalta la etiqueta y elige el nudo en la ventana del plan. Sin cuadros.";

        public void OnClick(TemporaryGraphicsCommandData data)
        {
            try
            {
                if (data == null) return;
                PlanLabels.OnClick(data.Document, data.Index);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "label_click_failed", error = ex.ToString() });
            }
        }
    }
}
