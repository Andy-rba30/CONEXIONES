using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit.Services
{
    /// <summary>
    /// Fase 13 (D9): la cadena de un solo clic de Conectar en un nudo en K con <c>splice_hint</c>: por cada diagonal, partirla a la
    /// distancia del Detalle B (<see cref="SplitService"/>) y crear el empalme de placa de extremo sobre los dos tramos
    /// (<see cref="SpecStarter.ForSplice"/> con las plantillas del catálogo), y al final el nudo en K sobre el tramo corto, vuelto a
    /// validar porque la diagonal cambió de curva. Todo dentro de un <c>TransactionGroup</c> exterior que se asimila (una sola entrada
    /// de deshacer): si cualquier paso falla, se revierte entero y la diagonal sigue entera. El único cuadro es el del botón de la cinta.
    /// </summary>
    public static class SplitChainService
    {
        public static Result CreateWithSplits(UIApplication uiApplication, Document doc, UIDocument? uidoc, PreviewSession session, string operationName)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (!session.CanCreate || session.Spec == null)
            {
                TaskDialog.Show(RibbonCreation.TitleCreate, "La especificación dejó de ser válida: vuelve a abrir la ventana y valida de nuevo.");
                return Result.Cancelled;
            }
            List<SplitTarget> targets = session.SplitTargets;
            if (targets.Count == 0) return RibbonCreation.CreateFromSession(uiApplication, doc, uidoc, session, operationName);

            var warnings = new List<ApiError>();
            string kNodeId = Guid.NewGuid().ToString("D");
            var splits = new List<SplitRecord>();
            var splices = new List<ConnectionRecord>();
            ConnectionRecord kRecord;
            var stopwatch = Stopwatch.StartNew();
            string stage = "preparar";
            PanelChoices choices = session.Choices;
            List<CatalogTemplate> templates = LoadTemplates(warnings);

            using (var outer = new TransactionGroup(doc, "MotorConexiones: Conectar con empalme B"))
            {
                try
                {
                    outer.Start();
                    foreach (SplitTarget target in targets)
                    {
                        // 1. Partir la diagonal (su propio ámbito, anidado; connection_id = el nudo en K, para que conn_delete lo deshaga).
                        stage = "partir " + target.ElementId;
                        SplitPreview preview = SplitService.Preview(doc, target.ElementId, target.ChordElementId, target.OffsetFromChordFaceMm);
                        SplitRecord split = SplitService.Split(doc, uiApplication, preview, kNodeId, null, warnings);
                        splits.Add(split);

                        // 2. El empalme B sobre los dos tramos: el arranque del detalle del perfil (o la plantilla del catálogo que encaje).
                        stage = "empalme B de " + target.ElementId;
                        var facts = new RevitModelFacts(doc);
                        TemplateNode spliceNode = TemplateNode.FromModelFacts(facts, split.ElementId, new[] { split.NewElementId });
                        StarterResult starter = SpecStarter.ForSplice(spliceNode, choices, templates);
                        ModelValidation validation = ValidationService.Validate(doc, starter.SpecJson);
                        if (!validation.IsValid || validation.Spec == null)
                        {
                            ApiError first = validation.Result.Errors.FirstOrDefault() ?? new ApiError(ErrorCodes.InternalError, "sin detalle");
                            throw new CatalogException(first.Code, "El empalme B de los tramos " + split.ElementId + " y " + split.NewElementId + " no valida: " + first.Message, first.Path, first.Hint);
                        }
                        warnings.AddRange(validation.Result.Warnings);
                        ConnectionRecord splice = RibbonCreation.CreateValidated(doc, uiApplication, uidoc, validation.Spec, starter.SpecJson, Guid.NewGuid().ToString("D"), operationName + "_splice_b", warnings);
                        splices.Add(splice);
                    }

                    // 3. El nudo en K sobre el tramo corto: se vuelve a validar (la diagonal cambió de curva y el token con ella).
                    stage = "nudo en K";
                    session.Refresh();
                    if (!session.CanCreate || session.Spec == null)
                    {
                        ApiError first = session.Validation?.Errors.FirstOrDefault() ?? new ApiError(ErrorCodes.InternalError, "sin detalle");
                        throw new CatalogException(first.Code, "El nudo en K ya no valida con la diagonal partida: " + first.Message, first.Path, first.Hint);
                    }
                    kRecord = RibbonCreation.CreateValidated(doc, uiApplication, uidoc, session.Spec, session.RawJson, kNodeId, operationName, warnings);
                    outer.Assimilate();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (outer.HasStarted() && !outer.HasEnded()) outer.RollBack();
                    }
                    catch
                    {
                        // Ya revertido por Revit.
                    }
                    string message = ex is CatalogException catalogError ? catalogError.Error.Message + (catalogError.Error.Hint != null ? "\n\n" + catalogError.Error.Hint : "") : ex.Message;
                    JsonLineLogger.Write(new
                    {
                        @event = "ribbon_split_chain_failed",
                        operation = operationName,
                        stage,
                        targets = targets.Select(t => t.ElementId).ToList(),
                        code = (ex as CatalogException)?.Error.Code,
                        error = ex.ToString(),
                        warnings = warnings.Select(w => w.Code).ToList(),
                        duration_ms = stopwatch.ElapsedMilliseconds,
                    });
                    TaskDialog.Show(RibbonCreation.TitleError,
                        "No se pudo completar la cadena (falló al " + stage + "):\n\n" + message +
                        "\n\nSe deshizo todo: la diagonal sigue entera y no se creó ninguna conexión. Puedes crear el nudo sin la casilla o partir la barra con el botón Partir barra.");
                    return Result.Failed;
                }
            }

            JsonLineLogger.Write(new
            {
                @event = "ribbon_split_chain",
                operation = operationName,
                splits = splits.Select(s => new { s.ElementId, s.NewElementId, s.SplitId, s.Mechanism }).ToList(),
                splice_connection_ids = splices.Select(s => s.ConnectionId).ToList(),
                connection_id = kRecord.ConnectionId,
                created_elements = kRecord.CreatedElementIds.Count + splices.Sum(s => s.CreatedElementIds.Count),
                warnings = warnings.Select(w => w.Code).Distinct().ToList(),
                duration_ms = stopwatch.ElapsedMilliseconds,
            });

            string warningText = warnings.Count == 0 ? "" : "\nAdvertencias: " + string.Join("; ", warnings.Select(w => w.Code).Distinct());
            var dialog = new TaskDialog(RibbonCreation.TitleCreate)
            {
                MainInstruction = "Diagonal partida, empalme B y nudo en K modelados.",
                MainContent =
                    string.Join("\n", splits.Select(s => "Corte: la barra " + s.ElementId + " (tramo corto, conserva el id) y el tramo nuevo " + s.NewElementId + " a " + s.SplitDistanceMm.ToString("0", CultureInfo.InvariantCulture) + " mm del eje del cordón.")) + "\n" +
                    string.Join("\n", splices.Select(s => "Empalme B: conexión " + s.ConnectionId + " (" + s.CreatedElementIds.Count + " elementos).")) + "\n" +
                    "Nudo en K: conexión " + kRecord.ConnectionId + " (" + kRecord.CreatedElementIds.Count + " elementos, " + kRecord.ModifiedMembers.Count + " retiros).\n" +
                    "Todo en una sola entrada de deshacer. Borrar el nudo en K (Conexiones del modelo o conn_delete) deshace también el corte si antes se borra el empalme B." + warningText,
                CommonButtons = TaskDialogCommonButtons.Close,
            };
            dialog.Show();
            return Result.Succeeded;
        }

        private static List<CatalogTemplate> LoadTemplates(List<ApiError> warnings)
        {
            var templates = new List<CatalogTemplate>();
            try
            {
                CatalogStore store = CatalogConfigLoader.OpenStore(CatalogConfigLoader.Load());
                foreach (CatalogEntry entry in store.List(warnings))
                {
                    try
                    {
                        CatalogTemplate? template = store.Get(entry.TemplateId);
                        if (template != null) templates.Add(template);
                    }
                    catch (CatalogException error)
                    {
                        warnings.Add(error.Error);
                    }
                }
            }
            catch (CatalogException error)
            {
                warnings.Add(error.Error);
            }
            return templates;
        }
    }
}
