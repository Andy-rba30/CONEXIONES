using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Crear y borrar el lote (Fase 9, sección 3.5 de la propuesta). <b>Crear</b>: cada nudo es una operación completa, la
    /// misma que <c>conn_create</c> (token comprobado contra el modelo, <see cref="OperationScope"/> con su
    /// <c>TransactionGroup</c>, geometría, adopción de los elementos de Advance Steel y registro), <b>anidada</b> en un grupo
    /// exterior del lote que se asimila al final: en Revit el lote es una sola entrada de deshacer, y el nudo que falla se
    /// revierte solo sin tirar los demás (P9, opción a). Con <c>stop_on_error</c> el exterior se revierte entero. Si
    /// <c>config\catalog.json</c> lleva <c>batch_single_undo: false</c> (plan B de la propuesta 4.1, por si el sondeo 20
    /// dijera que los grupos anidados no conviven con Advance Steel) no hay grupo exterior: una entrada de deshacer por nudo.
    /// Qué nudos se crean y en qué orden lo decide <see cref="BatchRunner"/> (Core, probado sin Revit); aquí solo va lo que
    /// necesita el modelo. Las marcas de los nudos creados se quitan al terminar (el acero las sustituye); las de los demás
    /// siguen. <b>Borrar</b>: todas las conexiones cuyo <c>source.batch_id</c> es el del plan, una a una con las garantías de
    /// <c>conn_delete</c>, en un solo grupo.
    /// </summary>
    public static class BatchCreator
    {
        /// <summary>Crea el lote. Devuelve el informe; el plan queda actualizado (estados <c>created</c> / <c>failed</c>, <c>last_report</c>).</summary>
        public static BatchReport Create(Document document, UIApplication? uiApplication, BatchPlan plan, BatchCreateRequest request, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (request == null) throw new ArgumentNullException(nameof(request));
            warnings ??= new List<ApiError>();
            if (!string.IsNullOrEmpty(plan.Document) && !string.Equals(plan.Document, document.Title, StringComparison.OrdinalIgnoreCase))
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "El plan " + plan.PlanId + " es del documento '" + plan.Document + "' y el documento activo es '" + document.Title + "'.", "plan_id",
                    "Activa el documento del plan o vuelve a planificar en este.");
            }

            bool singleUndo = CatalogConfigLoader.Load().BatchSingleUndo;
            var stopwatch = Stopwatch.StartNew();
            TransactionGroup? outer = null;
            string undoEntries = BatchReport.UndoOne;
            if (singleUndo)
            {
                try
                {
                    outer = new TransactionGroup(document, "MotorConexiones: batch_create " + plan.PlanId);
                    outer.Start();
                }
                catch (Exception ex)
                {
                    outer?.Dispose();
                    outer = null;
                    undoEntries = BatchReport.UndoPerNode;
                    warnings.Add(new ApiError(ErrorCodes.BatchUndoSplit, "No se pudo abrir el grupo exterior del lote (" + ex.Message + "): cada nudo quedará como una entrada de deshacer propia.",
                        hint: "Es el plan B de la propuesta (batch_single_undo: false). Anótalo en el informe de la fase."));
                }
            }
            else
            {
                undoEntries = BatchReport.UndoPerNode;
            }

            var created = new List<PlanNode>();
            BatchReport report;
            try
            {
                report = BatchRunner.Run(plan, request, (node, item) =>
                {
                    BatchNodeOutcome outcome = CreateOne(document, uiApplication, plan, node, item);
                    created.Add(node);
                    return outcome;
                }, undoEntries);

                if (outer != null)
                {
                    if (report.Stopped)
                    {
                        // stop_on_error: el grupo exterior se revierte entero; BatchRunner ya devolvió a listos los creados.
                        outer.RollBack();
                        JsonLineLogger.Write(new { @event = "batch_create_rolled_back", plan_id = plan.PlanId, stopped_at = report.StoppedAt });
                    }
                    else
                    {
                        RemoveMarksOfCreated(document, uiApplication, plan, created, warnings);
                        outer.Assimilate();
                        RemoveLabelsOfCreated(document, plan, created, warnings);
                    }
                }
                else if (!report.Stopped)
                {
                    RemoveMarksOfCreated(document, uiApplication, plan, created, warnings);
                    RemoveLabelsOfCreated(document, plan, created, warnings);
                }
            }
            catch
            {
                if (outer != null && outer.HasStarted() && !outer.HasEnded()) outer.RollBack();
                throw;
            }
            finally
            {
                outer?.Dispose();
            }

            stopwatch.Stop();
            report.Finish(stopwatch.ElapsedMilliseconds);
            foreach (BatchNodeResult failed in report.Nodes.Where(n => n.Outcome == BatchOutcome.Failed))
            {
                warnings.Add(new ApiError(ErrorCodes.BatchNodeFailed, failed.Describe(), "nodes[" + failed.Node + "]",
                    "Ese nudo se revirtió solo; en el plan queda failed con su especificación y su token para reintentar."));
            }
            PlanRegistry.Put(plan);
            JsonLineLogger.Write(new
            {
                @event = "batch_create",
                plan_id = plan.PlanId,
                requested = request.Items.Count,
                created = report.CreatedCount,
                updated = report.UpdatedCount,
                failed = report.FailedCount,
                skipped = report.SkippedCount,
                rolled_back = report.RolledBackCount,
                stop_on_error = request.StopOnError,
                stopped = report.Stopped,
                undo_entries = report.UndoEntries,
                connection_ids = report.ConnectionIds,
                duration_ms = report.DurationMs,
                summary = report.SummaryText,
            });
            return report;
        }

        /// <summary>
        /// Un nudo: lo mismo que <c>conn_create</c> (o <c>conn_update</c> si rehace una conexión), en su propio
        /// <see cref="OperationScope"/>. Lanza si algo falla; el ámbito ya ha revertido el grupo del nudo.
        /// </summary>
        private static BatchNodeOutcome CreateOne(Document document, UIApplication? uiApplication, BatchPlan plan, PlanNode node, BatchCreateItem item)
        {
            string rawSpecJson = item.Spec != null ? item.Spec.ToJsonString(BatchPlan.PrettyOptions) : node.SpecJson;
            ConnectionSpec spec;
            try
            {
                spec = ConnectionSpec.FromJson(rawSpecJson) ?? throw new InvalidOperationException("especificación vacía");
            }
            catch (Exception ex)
            {
                throw new CatalogException(ErrorCodes.SchemaInvalid, "La especificación de " + node.Name + " no se puede leer: " + ex.Message, "nodes[" + node.Name + "].spec", "Replanifica o quita la edición del nudo.");
            }

            // El token se comprueba contra el modelo como en conn_create: barras, documento y limits.json de ahora.
            var facts = new RevitModelFacts(document);
            LimitsConfig limits = LimitsConfigLoader.Load();
            string expected = ValidationTokenGenerator.GenerateToken(spec, facts, limits);
            if (!string.Equals(item.ValidationToken.Trim(), expected.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new CatalogException(ErrorCodes.ValidationTokenInvalid,
                    "El validation_token de " + node.Name + " no vale contra el modelo de ahora: alguna barra, el documento o limits.json cambiaron desde que se planificó.",
                    "nodes[" + node.Name + "].validation_token", "Replanifica (conn_batch_plan con el mismo plan_id) y vuelve a crear con los tokens nuevos.");
            }

            bool update = node.ReplacesExisting && !string.IsNullOrEmpty(node.ExistingConnectionId)
                          && ConnectionStorageManager.GetConnection(document, node.ExistingConnectionId!) != null;
            string connectionId = update ? node.ExistingConnectionId! : Guid.NewGuid().ToString("D");
            var nodeWarnings = new List<ApiError>();
            ConnectionRecord record;
            HashSet<long> snapshot = ConnectionCreationService.Snapshot(document);
            using (var scope = new OperationScope(document, uiApplication, update ? "update" : "create", connectionId, nodeWarnings))
            {
                using (Transaction tx = scope.StartTransaction(document, "MotorConexiones: " + (update ? "Rehacer " : "Crear ") + node.Name + " (" + (spec.Source?.Drawing ?? node.TemplateName ?? "lote") + ")"))
                {
                    record = update
                        ? ConnectionCreationService.UpdateConnection(document, uiApplication?.ActiveUIDocument, connectionId, spec, rawSpecJson, nodeWarnings)
                        : ConnectionCreationService.CreateConnection(document, uiApplication?.ActiveUIDocument, spec, rawSpecJson, connectionId, nodeWarnings);
                    scope.CommitOrThrow(tx);
                }
                using (Transaction adopt = scope.StartTransaction(document, "MotorConexiones: registrar elementos"))
                {
                    ConnectionCreationService.AdoptNewElements(document, record, snapshot, nodeWarnings);
                    scope.CommitOrThrow(adopt);
                }
                scope.Commit();
            }
            JsonLineLogger.Write(new { @event = update ? "batch_node_updated" : "batch_node_created", plan_id = plan.PlanId, node = node.Name, connection_id = record.ConnectionId, elements = record.CreatedElementIds.Count, warnings = nodeWarnings.Select(w => w.Code).ToList() });
            return new BatchNodeOutcome(record.ConnectionId, record.CreatedElementIds.Count, nodeWarnings.Where(w => w.Code != ErrorCodes.RevitWarning), update);
        }

        /// <summary>Las marcas (color y marcador) de los nudos creados se quitan: el acero las sustituye. Las de los demás siguen.</summary>
        private static void RemoveMarksOfCreated(Document document, UIApplication? uiApplication, BatchPlan plan, List<PlanNode> created, List<ApiError> warnings)
        {
            if (created.Count == 0 || !plan.IsMarked) return;
            PlanMarkState saved = PlanMarks.Capture(plan);
            try
            {
                using (var scope = new OperationScope(document, uiApplication, "batch_marks", plan.PlanId, warnings))
                {
                    using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: quitar marcas de los nudos creados"))
                    {
                        PlanMarks.RemoveNodes(document, plan, created, warnings);
                        scope.CommitOrThrow(clean);
                    }
                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                saved.Restore();
                warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Las conexiones se crearon, pero no se pudieron quitar las marcas de los nudos creados: " + ex.Message,
                    hint: "Descartar plan (o conn_batch_plan_discard) las quita."));
            }
        }

        /// <summary>Fase 10 (V3): las etiquetas del lienzo de los nudos creados se quitan con sus marcas (fuera de las transacciones). Nunca lanza.</summary>
        private static void RemoveLabelsOfCreated(Document document, BatchPlan plan, List<PlanNode> created, List<ApiError> warnings)
        {
            if (created.Count == 0 || !plan.HasLabels) return;
            try
            {
                PlanLabels.RemoveNodes(document, plan, created, warnings);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "labels_remove_failed", plan_id = plan.PlanId, error = ex.ToString() });
            }
        }

        /// <summary>
        /// Borra todas las conexiones del lote (<c>source.batch_id</c> = <paramref name="batchId"/>), una a una con las
        /// garantías de <c>conn_delete</c>, en un solo grupo (una entrada de deshacer). Una conexión que no se pueda borrar se
        /// anota y se sigue. Si el plan sigue en memoria, sus nudos creados vuelven a <c>ready</c>.
        /// </summary>
        public static BatchReport DeleteBatch(Document document, UIApplication? uiApplication, string batchId, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(batchId)) throw new CatalogException(ErrorCodes.InvalidRequest, "Falta batch_id (el plan_id del plan que creó el lote).", "batch_id", "conn_list dice el batch_id de cada conexión.");
            warnings ??= new List<ApiError>();
            BatchPlan? plan = PlanRegistry.Get(batchId);
            var stopwatch = Stopwatch.StartNew();
            var report = new BatchReport
            {
                BatchId = batchId.Trim(),
                PlanId = plan?.PlanId,
                Operation = BatchReport.OperationDelete,
                Document = document.Title,
            };
            List<ConnectionRecord> records = ConnectionsOfBatch(document, batchId);
            if (records.Count == 0)
            {
                warnings.Add(new ApiError(ErrorCodes.BatchEmpty, "No hay ninguna conexión con batch_id '" + batchId + "' en el modelo: nada que borrar.", "batch_id",
                    "conn_list enseña el batch_id de cada conexión (data.batches cuenta por lote)."));
                stopwatch.Stop();
                report.Finish(stopwatch.ElapsedMilliseconds);
                if (plan != null) BatchRunner.ApplyDeletion(plan, Array.Empty<string>(), report);
                return report;
            }

            using (var scope = new OperationScope(document, uiApplication, "batch_delete", batchId, warnings))
            {
                foreach (ConnectionRecord record in records)
                {
                    var result = new BatchNodeResult { ConnectionId = record.ConnectionId, Node = NodeNameOf(plan, record.ConnectionId) };
                    report.Nodes.Add(result);
                    var clock = Stopwatch.StartNew();
                    var itemWarnings = new List<ApiError>();
                    Transaction tx = scope.StartTransaction(document, "MotorConexiones: Borrar " + record.ConnectionId);
                    try
                    {
                        bool ok = ConnectionCreationService.DeleteConnection(document, record.ConnectionId, itemWarnings, out ConnectionRecord? deleted);
                        if (!ok) throw new InvalidOperationException("Revit no encontró la conexión " + record.ConnectionId + " al borrarla.");
                        scope.CommitOrThrow(tx);
                        result.Outcome = BatchOutcome.Deleted;
                        result.ElementsCount = deleted?.CreatedElementIds.Count ?? record.CreatedElementIds.Count;
                        result.RestoredMembersCount = deleted?.ModifiedMembers.Count ?? record.ModifiedMembers.Count;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (tx.HasStarted() && !tx.HasEnded()) tx.RollBack();
                        }
                        catch
                        {
                            // Ya revertida por Revit.
                        }
                        result.Outcome = BatchOutcome.Failed;
                        result.Errors.Add(new ApiError(ErrorCodes.InternalError, "No se pudo borrar la conexión " + record.ConnectionId + ": " + ex.Message, "connection_id",
                            "Las demás se borraron; prueba conn_delete con ese connection_id."));
                    }
                    finally
                    {
                        tx.Dispose();
                        result.Warnings.AddRange(itemWarnings.Where(w => w.Code != ErrorCodes.RevitWarning));
                        clock.Stop();
                        result.DurationMs = clock.ElapsedMilliseconds;
                    }
                }
                scope.Commit();
            }

            stopwatch.Stop();
            report.Finish(stopwatch.ElapsedMilliseconds);
            if (plan != null)
            {
                BatchRunner.ApplyDeletion(plan, report.Nodes.Where(n => n.Outcome == BatchOutcome.Deleted).Select(n => n.ConnectionId!), report);
                PlanRegistry.Put(plan);
            }
            JsonLineLogger.Write(new
            {
                @event = "batch_delete",
                batch_id = batchId,
                plan_in_memory = plan != null,
                deleted = report.DeletedCount,
                failed = report.FailedCount,
                elements = report.DeletedElementsCount,
                restored_members = report.RestoredMembersCount,
                duration_ms = report.DurationMs,
                summary = report.SummaryText,
            });
            return report;
        }

        /// <summary>Las conexiones del modelo cuyo <c>source.batch_id</c> es el indicado.</summary>
        public static List<ConnectionRecord> ConnectionsOfBatch(Document document, string batchId)
        {
            var records = new List<ConnectionRecord>();
            foreach (ConnectionRecord record in ConnectionStorageManager.ListConnections(document))
            {
                if (string.Equals(BatchIdOf(record), batchId.Trim(), StringComparison.OrdinalIgnoreCase)) records.Add(record);
            }
            return records;
        }

        /// <summary><c>source.batch_id</c> de una conexión guardada, o nulo.</summary>
        public static string? BatchIdOf(ConnectionRecord record)
        {
            try
            {
                string? id = ConnectionSpec.FromJson(record.SpecJson)?.Source?.BatchId;
                return string.IsNullOrWhiteSpace(id) ? null : id;
            }
            catch
            {
                return null;
            }
        }

        private static string? NodeNameOf(BatchPlan? plan, string connectionId)
        {
            if (plan == null) return null;
            PlanNode? node = plan.Nodes.FirstOrDefault(n => string.Equals(n.CreatedConnectionId, connectionId, StringComparison.OrdinalIgnoreCase))
                             ?? plan.Nodes.FirstOrDefault(n => string.Equals(n.ExistingConnectionId, connectionId, StringComparison.OrdinalIgnoreCase));
            if (node != null) return node.Name;
            return plan.LastReport?.Nodes.FirstOrDefault(n => string.Equals(n.ConnectionId, connectionId, StringComparison.OrdinalIgnoreCase))?.Node;
        }

        /// <summary>Los datos del informe para la respuesta del puente y para la ventana.</summary>
        public static object ReportToData(BatchReport report, BatchPlan? plan, bool includeSpecs)
        {
            return new
            {
                batch_id = report.BatchId,
                plan_id = report.PlanId,
                operation = report.Operation,
                document = report.Document,
                started_utc = report.StartedUtc,
                finished_utc = report.FinishedUtc,
                duration_ms = report.DurationMs,
                stop_on_error = report.StopOnError,
                stopped = report.Stopped,
                stopped_at = report.StoppedAt,
                undo_entries = report.UndoEntries,
                created_count = report.CreatedCount,
                updated_count = report.UpdatedCount,
                with_warnings_count = report.WithWarningsCount,
                failed_count = report.FailedCount,
                skipped_count = report.SkippedCount,
                rolled_back_count = report.RolledBackCount,
                deleted_count = report.DeletedCount,
                deleted_elements_count = report.DeletedElementsCount,
                restored_members_count = report.RestoredMembersCount,
                connection_ids = report.ConnectionIds,
                summary_text = report.SummaryText,
                nodes = report.Nodes.Select(n => new
                {
                    node = n.Node,
                    outcome = n.Outcome,
                    connection_id = n.ConnectionId,
                    template_name = n.TemplateName,
                    orientation = n.Orientation,
                    elements_count = n.ElementsCount,
                    restored_members_count = n.RestoredMembersCount,
                    duration_ms = n.DurationMs,
                    reason = n.Reason,
                    description = n.Describe(),
                    errors = n.Errors,
                    warnings = n.Warnings,
                    spec = includeSpecs && plan != null && n.Node != null && plan.Find(n.Node)?.Spec is { } spec ? (object)System.Text.Json.JsonDocument.Parse(spec.ToJsonString()).RootElement.Clone() : null,
                }).ToList(),
                plan_summary = plan?.Summary(),
                plan_summary_text = plan == null ? null : PlanAdvice.SummaryText(plan),
                plan_creatable_count = plan?.CreatableCount,
            };
        }
    }
}
