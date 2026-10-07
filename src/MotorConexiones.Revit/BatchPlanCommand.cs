using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Planificar lote" de la cinta (Fase 8): planifica sobre la selección (o reabre el último plan del documento
    /// si no hay nada seleccionado) y muestra la ventana del plan. Desde el cierre de la ronda 8c la ventana es <b>no
    /// modal</b> (opción B de P10, mejora C7): el comando la abre con <c>Show()</c>, crea el <see cref="PlanEvents"/> que la
    /// conecta con Revit y termina; la ventana se queda abierta mientras la persona orbita y pincha, y todo lo que toca el
    /// modelo pasa por el evento. Si la ventana ya está abierta, el botón la reutiliza: con una selección nueva planifica y
    /// la actualiza; sin selección, solo la trae delante. No crea ninguna conexión.
    /// Fase 10 (mejora C6, <b>selección asistida</b>): basta pinchar una barra; antes de planificar, el add-in busca las que
    /// la tocan (<see cref="BatchPlanner.ExpandSelection"/>) y, si añade alguna, un cuadro (este botón es el único sitio con
    /// diálogos) ofrece planificar con todas, solo con la selección o cancelar. También registra, por si el arranque no pudo,
    /// el manejador de clics de las etiquetas (<see cref="PlanLabels.EnsureHandlerRegistered"/>).
    /// Ronda 10b (0.10.1): el cuadro fija su botón por defecto después de añadir los enlaces de orden (en la 0.10.0 Revit
    /// lanzaba "Corresponding button not found: defaultButton" y el botón no planificaba con una barra seleccionada) y, si el
    /// cuadro fallara igual, se planifica con todas y se dice en la barra de estado en vez de morir.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class BatchPlanCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApplication = commandData.Application;
            UIDocument? uidoc = uiApplication.ActiveUIDocument;
            Document? doc = uidoc?.Document;
            if (doc == null || uidoc == null)
            {
                TaskDialog.Show("MotorConexiones", "Abre un proyecto de Revit con la cercha para planificar un lote.");
                return Result.Cancelled;
            }

            try
            {
                // Solo se puede crear dentro de un comando (contexto válido de la API); se crea una vez por sesión.
                PlanEvents.EnsureCreated();
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_batch_event_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo conectar la ventana del plan con Revit:\n" + ex.Message);
                return Result.Failed;
            }
            // Fase 10: el manejador de clics de las etiquetas (si el arranque no pudo). Nunca lanza; sin él las etiquetas no responden.
            PlanLabels.EnsureHandlerRegistered(out _);

            BatchPlanWindow? open = BatchPlanWindow.Current;
            var selected = uidoc.Selection.GetElementIds().Select(id => id.Value).OrderBy(id => id).ToList();
            if (open != null && open.IsBusy)
            {
                open.Activate();
                TaskDialog.Show("MotorConexiones - Planificar lote", "La ventana del plan está esperando a que Revit termine una acción (pinchar, replanificar…). Termínala y vuelve a pulsar.");
                return Result.Cancelled;
            }

            var warnings = new List<ApiError>();
            BatchPlan? plan;
            try
            {
                if (selected.Count >= 1)
                {
                    // Fase 10 (C6): completar la selección con las barras que la tocan, con la decisión de la persona.
                    List<long> ids = AssistSelection(doc, selected, warnings, out bool cancelled);
                    if (cancelled) return Result.Cancelled;
                    if (ids.Count < 2)
                    {
                        TaskDialog.Show("MotorConexiones - Planificar lote",
                            "Hay una sola barra seleccionada y no encontré ninguna otra que la toque (dentro del plano de la cercha), así que no hay ningún nudo que planificar.\n" +
                            "Selecciona la cercha (los cordones y todas las diagonales y montantes) o pincha una barra que sí forme nudo con otras y vuelve a pulsar Planificar lote.");
                        return Result.Cancelled;
                    }
                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { ElementIds = ids }, warnings);
                }
                else if (open != null)
                {
                    // Sin selección y con la ventana abierta: solo traerla delante (el plan que enseña sigue siendo el último).
                    open.Activate();
                    JsonLineLogger.Write(new { @event = "ribbon_batch_window_activated", plan_id = open.Plan.PlanId });
                    return Result.Succeeded;
                }
                else
                {
                    plan = PlanRegistry.LastFor(doc.Title);
                    if (plan == null)
                    {
                        TaskDialog.Show("MotorConexiones - Planificar lote",
                            "Pincha primero una barra de la cercha (el add-in añade las que la tocan) o selecciona la cercha entera, y vuelve a pulsar Planificar lote.\n" +
                            "Sin selección, el botón reabre el último plan de este documento, y ahora no hay ninguno.");
                        return Result.Cancelled;
                    }
                }
            }
            catch (CatalogException ex)
            {
                TaskDialog.Show("MotorConexiones - Planificar lote", "No se pudo planificar: " + ex.Error.Message + (ex.Error.Hint != null ? "\n" + ex.Error.Hint : ""));
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_batch_plan_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo planificar el lote:\n" + ex.Message);
                return Result.Failed;
            }

            string? status = warnings.Count > 0 ? string.Join(" · ", warnings.Select(w => w.Message)) : null;
            bool isError = warnings.Any(w => w.Code != ErrorCodes.SelectionExpanded);
            try
            {
                PlanSnapshot snapshot = BatchPlanner.SnapshotOf(doc, plan);
                if (open != null)
                {
                    open.Update(snapshot, status, isError);
                    open.Activate();
                    JsonLineLogger.Write(new { @event = "ribbon_batch_window_updated", plan_id = plan.PlanId, selection = selected.Count });
                    return Result.Succeeded;
                }
                var window = new BatchPlanWindow(snapshot, status, isError);
                _ = new WindowInteropHelper(window) { Owner = uiApplication.MainWindowHandle };
                window.Show();
                JsonLineLogger.Write(new { @event = "ribbon_batch_window_opened", plan_id = plan.PlanId, modeless = true, selection = selected.Count });
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_batch_window_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo abrir la ventana del plan:\n" + ex.Message);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// Selección asistida (Fase 10, C6): busca las barras que tocan la selección y, si hay alguna, pregunta en un cuadro
        /// (el único sitio del add-in con diálogos) si se planifica con todas, solo con la selección o nada. Sin barras que
        /// añadir, devuelve la selección tal cual sin preguntar. Si la búsqueda falla, queda en el log y se sigue con la selección.
        /// </summary>
        private static List<long> AssistSelection(Document doc, List<long> selected, List<ApiError> warnings, out bool cancelled)
        {
            cancelled = false;
            SelectionExpansion expansion;
            try
            {
                expansion = BatchPlanner.ExpandSelection(doc, selected, warnings);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_batch_assist_failed", selection = selected.Count, error = ex.ToString() });
                return selected;
            }
            if (expansion.Added.Count == 0)
            {
                if (selected.Count == 1) warnings.Add(new ApiError(ErrorCodes.SelectionExpanded, expansion.SummaryText(), "element_ids"));
                return selected;
            }

            TaskDialogResult choice;
            try
            {
                var dialog = new TaskDialog("MotorConexiones - Planificar lote")
                {
                    MainInstruction = "Selección asistida: " + expansion.Added.Count + (expansion.Added.Count == 1 ? " barra toca" : " barras tocan") + " la selección",
                    MainContent = expansion.SummaryText() + "\n\nSeleccionadas: " + selected.Count + ". Con las añadidas: " + expansion.AllIds.Count + ".",
                    AllowCancellation = true,
                    CommonButtons = TaskDialogCommonButtons.Cancel,
                };
                dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Planificar con las " + expansion.AllIds.Count + " barras",
                    "La selección y las que la tocan dentro del plano de la cercha (cordones que pasan de largo, barras que llegan, tramos del cordón).");
                dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Planificar solo las " + selected.Count + " seleccionadas",
                    "Como hasta ahora: sin añadir nada.");
                // Ronda 10b: el botón por defecto se fija DESPUÉS de añadir los enlaces de orden. En la 0.10.0 iba en el
                // inicializador, antes de AddCommandLink, y Revit lanzaba "Corresponding button not found: defaultButton"
                // (el enlace aún no existía): el botón Planificar lote moría con una barra seleccionada (resultados-fase-10.md).
                dialog.DefaultButton = TaskDialogResult.CommandLink1;
                choice = dialog.Show();
            }
            catch (Exception ex)
            {
                // Si el cuadro falla por lo que sea, la selección asistida no debe dejar sin planificar: se toma la opción por
                // defecto (con todas), se anota en el log y la barra de estado lo dice con el aviso SELECTION_EXPANDED.
                JsonLineLogger.Write(new { @event = "ribbon_batch_assist_dialog_failed", selection = selected.Count, added = expansion.Added.Count, error = ex.ToString() });
                warnings.Add(new ApiError(ErrorCodes.SelectionExpanded,
                    expansion.SummaryText() + " El cuadro para elegir no se pudo abrir (" + ex.Message + "): se planifica con todas; usa Más… > Completar selección o selecciona a mano si no era lo que querías.", "element_ids"));
                JsonLineLogger.Write(new { @event = "ribbon_batch_assist", selection = selected.Count, added = expansion.Added.Count, chords = expansion.ChordCount, members = expansion.MemberCount, splices = expansion.SpliceCount, skipped_out_of_plane = expansion.SkippedOutOfPlane.Count, choice = "all_without_dialog" });
                return expansion.AllIds;
            }
            string chosen;
            List<long> ids;
            switch (choice)
            {
                case TaskDialogResult.CommandLink1:
                    chosen = "all";
                    ids = expansion.AllIds;
                    warnings.Add(new ApiError(ErrorCodes.SelectionExpanded, expansion.SummaryText(), "element_ids"));
                    break;
                case TaskDialogResult.CommandLink2:
                    chosen = "selection";
                    ids = selected;
                    break;
                default:
                    chosen = "cancel";
                    ids = selected;
                    cancelled = true;
                    break;
            }
            JsonLineLogger.Write(new { @event = "ribbon_batch_assist", selection = selected.Count, added = expansion.Added.Count, chords = expansion.ChordCount, members = expansion.MemberCount, splices = expansion.SpliceCount, skipped_out_of_plane = expansion.SkippedOutOfPlane.Count, choice = chosen });
            return ids;
        }
    }
}
