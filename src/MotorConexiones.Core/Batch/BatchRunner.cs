using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Lo que devuelve el creador de un nudo cuando sale bien.</summary>
    public sealed class BatchNodeOutcome
    {
        public BatchNodeOutcome(string connectionId, int elementsCount, IEnumerable<ApiError>? warnings = null, bool updated = false)
        {
            ConnectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));
            ElementsCount = elementsCount;
            Updated = updated;
            if (warnings != null) Warnings.AddRange(warnings);
        }

        public string ConnectionId { get; }
        public int ElementsCount { get; }
        public bool Updated { get; }
        public List<ApiError> Warnings { get; } = new List<ApiError>();
    }

    /// <summary>
    /// Crea <b>un</b> nudo: en Revit, <c>BatchCreator</c> (token contra el modelo, <c>OperationScope</c>, geometría, registro);
    /// en las pruebas, una imitación. Lanza si falla; la excepción es el motivo del fallo (una <see cref="CatalogException"/>
    /// conserva su código).
    /// </summary>
    public delegate BatchNodeOutcome NodeCreator(PlanNode node, BatchCreateItem item);

    /// <summary>
    /// Qué nudos se crean, en qué orden y con qué comprobaciones (Fase 9): lo que no depende de Revit. Recorre la petición
    /// nudo a nudo, salta los que no se pueden crear (con su motivo), rechaza un token que no sea el del plan, llama al
    /// creador y anota el resultado en el informe y en el plan (estados <c>created</c> / <c>failed</c>). Con
    /// <c>stop_on_error</c>, el primer fallo detiene el lote y los nudos ya creados vuelven a <c>ready</c> (quien llama
    /// revierte el grupo exterior). Se prueba en la nube con un creador simulado.
    /// </summary>
    public static class BatchRunner
    {
        /// <summary>Estados desde los que se puede crear: listo, o fallido en un intento anterior (conserva token y especificación).</summary>
        public static bool CanBeCreated(PlanNode node) =>
            node != null && (node.Status == NodeStatus.Ready || node.Status == NodeStatus.Failed) && !string.IsNullOrEmpty(node.ValidationToken) && node.Spec != null;

        /// <summary>Motivo en español por el que un nudo no entra en el lote (nulo si puede crearse).</summary>
        public static string? WhyNotCreatable(PlanNode node, BatchPlan? plan = null)
        {
            if (node == null) return "el nudo no existe en el plan";
            if (node.Status == NodeStatus.Created) return "ya creada en este lote" + (node.CreatedConnectionId != null ? " (conexión " + node.CreatedConnectionId + ")" : "");
            if (CanBeCreated(node)) return null;
            if (node.Status == NodeStatus.Ready) return "listo pero sin token o sin especificación: replanifica";
            return "no está listo: " + PlanAdvice.StatusText(node) + " · " + PlanAdvice.Advice(node, plan);
        }

        /// <param name="plan">El plan en memoria; sus nudos cambian de estado.</param>
        /// <param name="request">Qué nudos y con qué tokens.</param>
        /// <param name="creator">Crea un nudo (Revit) o lo simula (pruebas).</param>
        /// <param name="undoEntries"><c>one</c> si hay grupo exterior (quien llama lo revierte con <c>stop_on_error</c>), <c>per_node</c> si no.</param>
        public static BatchReport Run(BatchPlan plan, BatchCreateRequest request, NodeCreator creator, string undoEntries = BatchReport.UndoOne)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (creator == null) throw new ArgumentNullException(nameof(creator));

            var total = Stopwatch.StartNew();
            var report = new BatchReport
            {
                BatchId = plan.PlanId,
                PlanId = plan.PlanId,
                Operation = BatchReport.OperationCreate,
                Document = plan.Document,
                StopOnError = request.StopOnError,
                UndoEntries = undoEntries,
            };
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var createdNow = new List<PlanNode>();
            bool stopped = false;

            foreach (BatchCreateItem item in request.Items)
            {
                var result = new BatchNodeResult { Node = item.Node };
                report.Nodes.Add(result);
                if (stopped)
                {
                    result.Reason = "no se intentó: el lote se detuvo en " + report.StoppedAt;
                    continue;
                }
                PlanNode? node = plan.Find(item.Node);
                if (node == null)
                {
                    result.Reason = "el nudo no existe en el plan";
                    result.Errors.Add(new ApiError(ErrorCodes.InvalidRequest, "El plan " + plan.PlanId + " no tiene ningún nudo llamado '" + item.Node + "'.", "nodes",
                        "Usa los nombres de nodes[].name: " + string.Join(", ", plan.Nodes.Select(n => n.Name)) + "."));
                    continue;
                }
                result.TemplateName = node.TemplateName;
                result.Orientation = node.Orientation;
                if (!seen.Add(node.Name))
                {
                    result.Reason = "repetido en la petición";
                    continue;
                }
                string? why = WhyNotCreatable(node, plan);
                if (why != null)
                {
                    result.Reason = why;
                    continue;
                }
                if (item.Spec == null && !string.Equals(item.ValidationToken, node.ValidationToken, StringComparison.OrdinalIgnoreCase))
                {
                    result.Outcome = BatchOutcome.Failed;
                    result.Reason = "token distinto del plan";
                    var error = new ApiError(ErrorCodes.ValidationTokenInvalid,
                        "El validation_token de " + node.Name + " no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo).",
                        "nodes[" + node.Name + "].validation_token",
                        "Relee el plan con conn_batch_plan_get y pasa el validation_token actual de " + node.Name + ".");
                    result.Errors.Add(error);
                    MarkFailed(node, error);
                    if (request.StopOnError)
                    {
                        stopped = true;
                        report.StoppedAt = node.Name;
                    }
                    continue;
                }

                var clock = Stopwatch.StartNew();
                try
                {
                    BatchNodeOutcome outcome = creator(node, item);
                    clock.Stop();
                    result.DurationMs = clock.ElapsedMilliseconds;
                    result.ConnectionId = outcome.ConnectionId;
                    result.ElementsCount = outcome.ElementsCount;
                    result.Warnings.AddRange(outcome.Warnings);
                    bool warned = node.Warnings.Count > 0 || outcome.Warnings.Count > 0;
                    result.Outcome = outcome.Updated || node.ReplacesExisting ? BatchOutcome.Updated : warned ? BatchOutcome.CreatedWithWarnings : BatchOutcome.Created;
                    node.Status = NodeStatus.Created;
                    node.CreatedConnectionId = outcome.ConnectionId;
                    node.Errors.RemoveAll(e => e.Path != null && e.Path.StartsWith("batch", StringComparison.Ordinal));
                    node.StatusDetail = (outcome.Updated || node.ReplacesExisting ? "Rehecha" : "Creada") + " en este lote: conexión " + outcome.ConnectionId + " (" + outcome.ElementsCount + " elementos).";
                    createdNow.Add(node);
                }
                catch (Exception ex)
                {
                    clock.Stop();
                    result.DurationMs = clock.ElapsedMilliseconds;
                    result.Outcome = BatchOutcome.Failed;
                    ApiError error = ex is CatalogException catalogError
                        ? catalogError.Error
                        : new ApiError(ErrorCodes.FabricationFailed, "Fallo al modelar " + node.Name + ": " + ex.Message, "nodes[" + node.Name + "]",
                            "El nudo se revirtió solo; los demás siguen. Mira el registro y el detalle del nudo.");
                    result.Reason = error.Code;
                    result.Errors.Add(error);
                    MarkFailed(node, error);
                    if (request.StopOnError)
                    {
                        stopped = true;
                        report.StoppedAt = node.Name;
                    }
                }
            }

            if (stopped)
            {
                report.Stopped = true;
                if (undoEntries == BatchReport.UndoOne)
                {
                    // Quien llama revierte el grupo exterior: en el plan, los creados en esta pasada vuelven a listos.
                    foreach (PlanNode node in createdNow)
                    {
                        BatchNodeResult? result = report.Find(node.Name);
                        if (result != null) result.Outcome = BatchOutcome.RolledBack;
                        RestoreReady(node);
                    }
                }
                // Sin grupo exterior (plan B, una entrada de deshacer por nudo) no hay nada que revertir: los creados antes
                // del fallo se quedan creados y el informe lo dice.
            }

            total.Stop();
            report.Finish(total.ElapsedMilliseconds);
            plan.LastReport = report;
            plan.UpdatedUtc = DateTime.UtcNow.ToString("o");
            return report;
        }

        /// <summary>
        /// Después de <c>batch_delete</c>: los nudos del plan cuya conexión se borró vuelven a <c>ready</c> con su token (la
        /// geometría que el token resume no cambió: <c>conn_delete</c> restauró las barras) y el informe queda en el plan.
        /// Devuelve cuántos nudos volvieron a listos.
        /// </summary>
        public static int ApplyDeletion(BatchPlan plan, IEnumerable<string> deletedConnectionIds, BatchReport? report = null)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var deleted = new HashSet<string>(deletedConnectionIds ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            int restored = 0;
            foreach (PlanNode node in plan.Nodes)
            {
                bool created = node.Status == NodeStatus.Created && node.CreatedConnectionId != null && deleted.Contains(node.CreatedConnectionId);
                bool already = node.Status == NodeStatus.AlreadyConnected && node.ExistingConnectionId != null && deleted.Contains(node.ExistingConnectionId);
                if (created)
                {
                    RestoreReady(node);
                    restored++;
                }
                else if (already)
                {
                    node.StatusDetail = "Su conexión " + node.ExistingConnectionId + " se borró con el lote: replanifica para volver a planificarlo.";
                    node.ExistingConnectionId = null;
                    node.ExistingBatchId = null;
                }
            }
            if (report != null) plan.LastReport = report;
            plan.UpdatedUtc = DateTime.UtcNow.ToString("o");
            return restored;
        }

        private static void MarkFailed(PlanNode node, ApiError error)
        {
            node.Status = NodeStatus.Failed;
            node.CreatedConnectionId = null;
            node.Errors.RemoveAll(e => e.Path != null && e.Path.StartsWith("batch", StringComparison.Ordinal));
            node.Errors.Add(new ApiError(error.Code, error.Message, "batch:" + (error.Path ?? ""), error.Hint));
            node.StatusDetail = "Falló al crear (" + error.Code + "): " + BatchReport.Short(error.Message) + ".";
        }

        private static void RestoreReady(PlanNode node)
        {
            node.Status = NodeStatus.Ready;
            node.CreatedConnectionId = null;
            node.Errors.RemoveAll(e => e.Path != null && e.Path.StartsWith("batch", StringComparison.Ordinal));
            node.StatusDetail = null;
        }
    }
}
