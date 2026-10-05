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
                if (selected.Count >= 2)
                {
                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { ElementIds = selected }, warnings);
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
                            "Selecciona primero las barras de la cercha (los cordones y todas las diagonales y montantes) y vuelve a pulsar Planificar lote.\n" +
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
            try
            {
                PlanSnapshot snapshot = BatchPlanner.SnapshotOf(doc, plan);
                if (open != null)
                {
                    open.Update(snapshot, status, warnings.Count > 0);
                    open.Activate();
                    JsonLineLogger.Write(new { @event = "ribbon_batch_window_updated", plan_id = plan.PlanId, selection = selected.Count });
                    return Result.Succeeded;
                }
                var window = new BatchPlanWindow(snapshot, status);
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
    }
}
