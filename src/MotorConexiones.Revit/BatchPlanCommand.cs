using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
    /// si no hay nada seleccionado) y muestra la ventana del plan. Las acciones que necesitan la vista de Revit (Ver en
    /// Revit, pinchar cordón o barras, añadir nudo) cierran la ventana, se hacen aquí y la ventana se vuelve a abrir:
    /// es la opción A de la propuesta (ventana modal), sin <c>ExternalEvent</c>. No crea ninguna conexión.
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

            var warnings = new List<ApiError>();
            BatchPlan? plan;
            var selected = uidoc.Selection.GetElementIds().Select(id => id.Value).OrderBy(id => id).ToList();
            try
            {
                if (selected.Count >= 2)
                {
                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { ElementIds = selected }, warnings);
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
            string? selectNode = null;
            while (true)
            {
                BatchPlanWindow window;
                try
                {
                    window = new BatchPlanWindow(doc, uiApplication, plan, status) { SelectNodeOnLoad = selectNode };
                    _ = new WindowInteropHelper(window) { Owner = uiApplication.MainWindowHandle };
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    JsonLineLogger.Write(new { @event = "ribbon_batch_window_failed", error = ex.ToString() });
                    TaskDialog.Show("MotorConexiones - Error", "No se pudo abrir la ventana del plan:\n" + ex.Message);
                    return Result.Failed;
                }
                plan = window.Plan;
                JsonLineLogger.Write(new { @event = "ribbon_batch_window", plan_id = plan.PlanId, action = window.Action.ToString(), node = window.ActionNodeName, discarded = window.Discarded });
                if (window.Discarded || window.Action == PlanWindowAction.None) return Result.Succeeded;

                status = null;
                selectNode = window.ActionNodeName;
                PlanNode? node = window.ActionNodeName != null ? plan.Find(window.ActionNodeName) : null;
                try
                {
                    switch (window.Action)
                    {
                        case PlanWindowAction.ShowInRevit:
                            if (node != null)
                            {
                                var ids = node.ElementIds.Select(id => new ElementId(id)).Where(id => doc.GetElement(id) != null).ToList();
                                if (node.MarkerElementId.HasValue && doc.GetElement(new ElementId(node.MarkerElementId.Value)) != null) ids.Add(new ElementId(node.MarkerElementId.Value));
                                uidoc.Selection.SetElementIds(ids);
                                uidoc.ShowElements(ids);
                                TaskDialog.Show("MotorConexiones - " + node.Name, node.Describe() + "\n\nMira el nudo en la vista (puedes orbitar) y pulsa Cerrar para volver al plan.");
                                status = "Nudo " + node.Name + " mostrado en Revit.";
                            }
                            break;
                        case PlanWindowAction.PickChord:
                            if (node != null)
                            {
                                Reference? picked = Pick(uidoc, "Pincha el cordón del nudo " + node.Name + " (Esc para cancelar)");
                                if (picked != null)
                                {
                                    var delta = new BatchOverrides();
                                    delta.Chord[node.Name] = picked.ElementId.Value;
                                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { PlanId = plan.PlanId, Overrides = delta }, warnings = new List<ApiError>());
                                    status = "Cordón de " + node.Name + ": " + picked.ElementId.Value + ".";
                                }
                                else status = "Sin cambios (elección cancelada).";
                            }
                            break;
                        case PlanWindowAction.PickMembersToAdd:
                            if (node != null)
                            {
                                var picked = PickMany(uidoc, "Pincha las barras que faltan en el nudo " + node.Name + " y pulsa Finalizar (Esc para cancelar)");
                                if (picked.Count > 0)
                                {
                                    var delta = new BatchOverrides();
                                    delta.AddMember[node.Name] = picked;
                                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { PlanId = plan.PlanId, Overrides = delta }, warnings = new List<ApiError>());
                                    status = "Añadidas a " + node.Name + ": " + string.Join(", ", picked) + ".";
                                }
                                else status = "Sin cambios (elección cancelada).";
                            }
                            break;
                        case PlanWindowAction.PickNewNode:
                            {
                                var picked = PickMany(uidoc, "Pincha el cordón y las barras del nudo nuevo y pulsa Finalizar (Esc para cancelar)");
                                if (picked.Count >= 2)
                                {
                                    string name = NodeDetector.NextName(plan.Nodes.Select(n => n.Name));
                                    var delta = new BatchOverrides();
                                    delta.AddNode[name] = picked;
                                    plan = BatchPlanner.Plan(doc, uiApplication, new BatchPlanInput { PlanId = plan.PlanId, Overrides = delta }, warnings = new List<ApiError>());
                                    selectNode = name;
                                    status = "Nudo " + name + " añadido con " + picked.Count + " barras.";
                                }
                                else status = picked.Count == 0 ? "Sin cambios (elección cancelada)." : "Un nudo necesita al menos 2 barras: no se añadió nada.";
                            }
                            break;
                    }
                }
                catch (CatalogException ex)
                {
                    status = "No se pudo aplicar la corrección: " + ex.Error.Message;
                }
                catch (Exception ex)
                {
                    JsonLineLogger.Write(new { @event = "ribbon_batch_action_failed", action = window.Action.ToString(), error = ex.ToString() });
                    status = "No se pudo aplicar la corrección: " + ex.Message;
                }
                if (warnings.Count > 0) status = (status ?? "") + " " + string.Join(" · ", warnings.Select(w => w.Message));
            }
        }

        private static Reference? Pick(UIDocument uidoc, string prompt)
        {
            try
            {
                return uidoc.Selection.PickObject(ObjectType.Element, new FramingFilter(), prompt);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return null;
            }
        }

        private static List<long> PickMany(UIDocument uidoc, string prompt)
        {
            try
            {
                return uidoc.Selection.PickObjects(ObjectType.Element, new FramingFilter(), prompt).Select(r => r.ElementId.Value).Distinct().ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return new List<long>();
            }
        }

        /// <summary>Solo barras de armazón estructural con eje.</summary>
        private sealed class FramingFilter : ISelectionFilter
        {
            private static readonly long FramingCategory = new ElementId(BuiltInCategory.OST_StructuralFraming).Value;

            public bool AllowElement(Element elem) =>
                elem is FamilyInstance fi && fi.Category != null && fi.Category.Id.Value == FramingCategory && fi.Location is LocationCurve;

            public bool AllowReference(Reference reference, XYZ position) => true;
        }
    }
}
