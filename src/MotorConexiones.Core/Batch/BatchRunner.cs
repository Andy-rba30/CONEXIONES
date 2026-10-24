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

    /// <summary>Fase 13: lo que devuelve quien parte una diagonal: el tramo largo nuevo y el registro del corte.</summary>
    public sealed class SplitOutcome
    {
        public SplitOutcome(long newElementId, string splitId, IEnumerable<ApiError>? warnings = null)
        {
            NewElementId = newElementId;
            SplitId = splitId ?? throw new ArgumentNullException(nameof(splitId));
            if (warnings != null) Warnings.AddRange(warnings);
        }

        public long NewElementId { get; }
        public string SplitId { get; }
        public List<ApiError> Warnings { get; } = new List<ApiError>();
    }

    /// <summary>
    /// Crea <b>un</b> nudo: en Revit, <c>BatchCreator</c> (token contra el modelo, <c>OperationScope</c>, geometría, registro);
    /// en las pruebas, una imitación. Lanza si falla; la excepción es el motivo del fallo (una <see cref="CatalogException"/>
    /// conserva su código).
    /// </summary>
    public delegate BatchNodeOutcome NodeCreator(PlanNode node, BatchCreateItem item);

    /// <summary>Fase 13: parte una diagonal (paso previo de un nudo en K). Lanza si falla.</summary>
    public delegate SplitOutcome NodeSplitter(PlanNode kNode, PlanSplitStep step);

    /// <summary>Fase 13: crea el empalme B previsto sobre los dos tramos recién partidos (el original y <paramref name="split"/>). Lanza si falla.</summary>
    public delegate BatchNodeOutcome SpliceCreator(PlanNode spliceNode, PlanSplitStep step, SplitOutcome split);

    /// <summary>Fase 13: quien sabe partir barras y crear el empalme B en Revit (o lo imita en las pruebas). Sin él, los pasos de corte se saltan.</summary>
    /// <summary>
    /// Fase 13: tras partir una diagonal, el token del nudo en K (calculado con la barra entera) ya no vale contra el modelo: quien
    /// crea el lote vuelve a validar la especificación que se va a crear (<c>item.Spec</c> o la del plan) y devuelve el token nuevo.
    /// Lanza <see cref="CatalogException"/> si ya no valida.
    /// </summary>
    public delegate string NodeRevalidator(PlanNode kNode, BatchCreateItem item);

    public sealed class BatchStepHandlers
    {
        public NodeSplitter? Split { get; set; }
        public SpliceCreator? CreateSplice { get; set; }
        /// <summary>Opcional: sin él, el nudo en K se crea con el token del plan (vale solo si quien crea no comprueba la geometría).</summary>
        public NodeRevalidator? Revalidate { get; set; }

        public bool CanSplit => Split != null && CreateSplice != null;
    }

    /// <summary>Fase 13: un paso del lote en el orden en que se ejecuta: un corte, un empalme B o un nudo.</summary>
    public sealed class BatchStep
    {
        public const string KindSplit = "split";
        public const string KindSpliceB = "splice_b";
        public const string KindNode = "node";

        public BatchStep(string kind, BatchCreateItem item, PlanSplitStep? splitStep = null, string? nodeName = null)
        {
            Kind = kind;
            Item = item;
            SplitStep = splitStep;
            NodeName = nodeName ?? item.Node;
        }

        public string Kind { get; }

        /// <summary>El nudo de la petición al que pertenece el paso (el nudo en K en los cortes y los empalmes B).</summary>
        public BatchCreateItem Item { get; }

        public PlanSplitStep? SplitStep { get; }

        /// <summary>El nombre que sale en el informe: "N5", "N5B" o "N5 · partir 1249631".</summary>
        public string NodeName { get; }

        public override string ToString() => NodeName;
    }

    /// <summary>
    /// Qué nudos se crean, en qué orden y con qué comprobaciones (Fase 9): lo que no depende de Revit. Recorre la petición
    /// nudo a nudo, salta los que no se pueden crear (con su motivo), rechaza un token que no sea el del plan, llama al
    /// creador y anota el resultado en el informe y en el plan (estados <c>created</c> / <c>failed</c>). Con
    /// <c>stop_on_error</c>, el primer fallo detiene el lote y los nudos ya creados vuelven a <c>ready</c> (quien llama
    /// revierte el grupo exterior). Fase 13 (D5): un nudo en K con pasos de corte parte antes sus diagonales y crea los empalmes
    /// B previstos (cortes → empalmes B → <c>N9E</c> → <c>N9</c>); sin quien parta (<see cref="BatchStepHandlers"/>) o con
    /// <c>split_members: false</c>, los pasos se saltan y el nudo en K se crea como en la Fase 12. Se prueba en la nube.
    /// </summary>
    public static class BatchRunner
    {
        /// <summary>Estados desde los que se puede crear: listo, o fallido en un intento anterior (conserva token y especificación).</summary>
        public static bool CanBeCreated(PlanNode node) =>
            node != null && (node.Status == NodeStatus.Ready || node.Status == NodeStatus.Failed) && !string.IsNullOrEmpty(node.ValidationToken) && node.Spec != null && !node.IsAfterSplit;

        /// <summary>Motivo en español por el que un nudo no entra en el lote (nulo si puede crearse).</summary>
        public static string? WhyNotCreatable(PlanNode node, BatchPlan? plan = null)
        {
            if (node == null) return "el nudo no existe en el plan";
            if (node.Status == NodeStatus.Created) return "ya creada en este lote" + (node.CreatedConnectionId != null ? " (conexión " + node.CreatedConnectionId + ")" : "");
            if (node.IsAfterSplit) return "es el empalme B previsto: se crea con el corte de " + (node.SplitOfNode ?? "su nudo en K") + ", no por separado";
            if (CanBeCreated(node)) return null;
            if (node.Status == NodeStatus.Ready) return "listo pero sin token o sin especificación: replanifica";
            return "no está listo: " + PlanAdvice.StatusText(node) + " · " + PlanAdvice.Advice(node, plan);
        }

        /// <param name="plan">El plan en memoria; sus nudos cambian de estado.</param>
        /// <param name="request">Qué nudos y con qué tokens.</param>
        /// <param name="creator">Crea un nudo (Revit) o lo simula (pruebas).</param>
        /// <param name="undoEntries"><c>one</c> si hay grupo exterior (quien llama lo revierte con <c>stop_on_error</c>), <c>per_node</c> si no.</param>
        /// <param name="handlers">Fase 13: quien parte las diagonales y crea los empalmes B; nulo = los pasos de corte se saltan.</param>
        public static BatchReport Run(BatchPlan plan, BatchCreateRequest request, NodeCreator creator, string undoEntries = BatchReport.UndoOne, BatchStepHandlers? handlers = null)
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
            var splitNow = new List<PlanSplitStep>();
            // Fase 13: los nudos en K cuyo token se renovó tras partir su diagonal (nombre → token nuevo) y el token que tenían.
            var retokened = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var originalTokens = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            bool stopped = false;
            // Los nudos que ya se saltaron (no existen, repetidos, no creables): sus pasos de corte tampoco se intentan.
            var skippedNodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool canSplit = request.SplitMembers && handlers != null && handlers.CanSplit;

            foreach (BatchStep step in StepsFor(plan, request))
            {
                if (step.Kind == BatchStep.KindNode)
                {
                    RunNode(plan, request, creator, report, step.Item, seen, createdNow, skippedNodes, retokened, ref stopped);
                    continue;
                }
                // Fase 13: un corte o un empalme B. Antes, el nudo en K al que pertenecen tiene que poder crearse (si no, el paso se salta con el motivo).
                var result = new BatchNodeResult { Node = step.NodeName, Kind = step.Kind, ElementId = step.SplitStep?.ElementId };
                report.Nodes.Add(result);
                if (stopped)
                {
                    result.Reason = "no se intentó: el lote se detuvo en " + report.StoppedAt;
                    continue;
                }
                PlanNode? kNode = plan.Find(step.Item.Node);
                PlanSplitStep planStep = step.SplitStep!;
                if (kNode == null || skippedNodes.Contains(step.Item.Node))
                {
                    result.Reason = "no se intentó: el nudo " + step.Item.Node + " no entra en el lote";
                    continue;
                }
                string? why = WhyNotCreatable(kNode, plan);
                if (why != null && kNode.Status != NodeStatus.Created)
                {
                    result.Reason = "no se intentó: " + why;
                    skippedNodes.Add(kNode.Name);
                    continue;
                }
                if (step.Item.Spec == null && !string.Equals(step.Item.ValidationToken, kNode.ValidationToken, StringComparison.OrdinalIgnoreCase))
                {
                    result.Reason = "no se intentó: el token de " + kNode.Name + " no es el del plan";
                    continue;
                }
                if (!canSplit)
                {
                    result.Reason = request.SplitMembers ? "sin partir: quien crea el lote no parte barras (la diagonal sigue entera; Conectar o Partir barra la parten)" : "sin partir: split_members es false";
                    continue;
                }
                if (!planStep.CanSplit)
                {
                    result.Reason = "no se puede partir: " + (planStep.Error ?? "sin punto de corte");
                    continue;
                }
                PlanNode? spliceNode = plan.Find(planStep.SpliceNode);
                var clock = Stopwatch.StartNew();
                try
                {
                    if (step.Kind == BatchStep.KindSplit)
                    {
                        if (planStep.IsDone)
                        {
                            result.Reason = "ya partida (tramo nuevo " + planStep.NewElementId + ")";
                            continue;
                        }
                        SplitOutcome outcome = handlers!.Split!(kNode, planStep);
                        clock.Stop();
                        result.DurationMs = clock.ElapsedMilliseconds;
                        result.Outcome = BatchOutcome.Split;
                        result.NewElementId = outcome.NewElementId;
                        result.SplitId = outcome.SplitId;
                        result.ElementsCount = 1;
                        result.Warnings.AddRange(outcome.Warnings);
                        planStep.NewElementId = outcome.NewElementId;
                        planStep.SplitId = outcome.SplitId;
                        splitNow.Add(planStep);
                        if (handlers.Revalidate != null)
                        {
                            // La diagonal cambió de curva: el token del nudo en K se renueva contra el modelo de ahora.
                            string token = handlers.Revalidate(kNode, step.Item);
                            if (!originalTokens.ContainsKey(kNode.Name)) originalTokens[kNode.Name] = kNode.ValidationToken;
                            retokened[kNode.Name] = token;
                            if (step.Item.Spec == null) kNode.ValidationToken = token;
                        }
                    }
                    else
                    {
                        if (!planStep.IsDone)
                        {
                            result.Reason = "no se intentó: la diagonal " + planStep.ElementId + " no se partió";
                            continue;
                        }
                        if (spliceNode == null)
                        {
                            result.Reason = "el plan no tiene el empalme previsto " + planStep.SpliceNode;
                            continue;
                        }
                        if (spliceNode.Status == NodeStatus.Created)
                        {
                            result.Reason = "ya creada en este lote (conexión " + spliceNode.CreatedConnectionId + ")";
                            continue;
                        }
                        BatchNodeOutcome outcome = handlers!.CreateSplice!(spliceNode, planStep, new SplitOutcome(planStep.NewElementId!.Value, planStep.SplitId ?? string.Empty));
                        clock.Stop();
                        result.DurationMs = clock.ElapsedMilliseconds;
                        result.ConnectionId = outcome.ConnectionId;
                        result.ElementsCount = outcome.ElementsCount;
                        result.TemplateName = spliceNode.TemplateName;
                        result.Warnings.AddRange(outcome.Warnings);
                        result.Outcome = outcome.Warnings.Count > 0 || spliceNode.Warnings.Count > 0 ? BatchOutcome.CreatedWithWarnings : BatchOutcome.Created;
                        spliceNode.Status = NodeStatus.Created;
                        spliceNode.CreatedConnectionId = outcome.ConnectionId;
                        spliceNode.MemberElementIds = new List<long> { planStep.NewElementId.Value };
                        spliceNode.ElementIds = new List<long> { planStep.ElementId, planStep.NewElementId.Value };
                        spliceNode.StatusDetail = "Creada en este lote tras partir " + planStep.ElementId + ": conexión " + outcome.ConnectionId + " (" + outcome.ElementsCount + " elementos; tramo nuevo " + planStep.NewElementId + ").";
                        createdNow.Add(spliceNode);
                    }
                }
                catch (Exception ex)
                {
                    clock.Stop();
                    result.DurationMs = clock.ElapsedMilliseconds;
                    result.Outcome = BatchOutcome.Failed;
                    ApiError error = ex is CatalogException catalogError
                        ? catalogError.Error
                        : new ApiError(step.Kind == BatchStep.KindSplit ? ErrorCodes.SplitFailed : ErrorCodes.FabricationFailed,
                            (step.Kind == BatchStep.KindSplit ? "Fallo al partir la diagonal " + planStep.ElementId : "Fallo al modelar el empalme " + planStep.SpliceNode) + ": " + ex.Message,
                            "nodes[" + kNode.Name + "].split_steps",
                            "El paso se revirtió solo; el nudo en K " + kNode.Name + " no se crea en esta pasada.");
                    result.Reason = error.Code;
                    result.Errors.Add(error);
                    // El nudo en K no se crea sin su corte (D5): queda fallido con el motivo y se reintenta con Crear otra vez.
                    MarkFailed(kNode, error);
                    skippedNodes.Add(kNode.Name);
                    if (spliceNode != null && spliceNode.Status != NodeStatus.Created)
                    {
                        spliceNode.Errors.RemoveAll(e => e.Path != null && e.Path.StartsWith("batch", StringComparison.Ordinal));
                        spliceNode.Errors.Add(new ApiError(error.Code, error.Message, "batch:" + (error.Path ?? ""), error.Hint));
                    }
                    if (request.StopOnError)
                    {
                        stopped = true;
                        report.StoppedAt = step.NodeName;
                    }
                }
            }

            if (stopped)
            {
                report.Stopped = true;
                if (undoEntries == BatchReport.UndoOne)
                {
                    // Quien llama revierte el grupo exterior: en el plan, los creados en esta pasada vuelven a listos (y los cortes, a no hechos).
                    foreach (PlanNode node in createdNow)
                    {
                        BatchNodeResult? result = report.Find(node.Name);
                        if (result != null) result.Outcome = BatchOutcome.RolledBack;
                        RestoreReady(node);
                    }
                    foreach (PlanSplitStep step in splitNow)
                    {
                        BatchNodeResult? result = report.Nodes.FirstOrDefault(r => r.Kind == BatchStep.KindSplit && r.ElementId == step.ElementId && r.Outcome == BatchOutcome.Split);
                        if (result != null) result.Outcome = BatchOutcome.RolledBack;
                        step.NewElementId = null;
                        step.SplitId = null;
                    }
                    foreach (KeyValuePair<string, string?> pair in originalTokens)
                    {
                        PlanNode? kNode = plan.Find(pair.Key);
                        if (kNode != null && kNode.Status != NodeStatus.Created) kNode.ValidationToken = pair.Value;
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

        private static void RunNode(BatchPlan plan, BatchCreateRequest request, NodeCreator creator, BatchReport report, BatchCreateItem item,
            HashSet<string> seen, List<PlanNode> createdNow, HashSet<string> skippedNodes, IReadOnlyDictionary<string, string> retokened, ref bool stopped)
        {
            var result = new BatchNodeResult { Node = item.Node };
            report.Nodes.Add(result);
            if (stopped)
            {
                result.Reason = "no se intentó: el lote se detuvo en " + report.StoppedAt;
                return;
            }
            PlanNode? node = plan.Find(item.Node);
            if (node == null)
            {
                result.Reason = "el nudo no existe en el plan";
                result.Errors.Add(new ApiError(ErrorCodes.InvalidRequest, "El plan " + plan.PlanId + " no tiene ningún nudo llamado '" + item.Node + "'.", "nodes",
                    "Usa los nombres de nodes[].name: " + string.Join(", ", plan.Nodes.Select(n => n.Name)) + "."));
                skippedNodes.Add(item.Node);
                return;
            }
            result.TemplateName = node.TemplateName;
            result.Orientation = node.Orientation;
            if (!seen.Add(node.Name))
            {
                result.Reason = "repetido en la petición";
                return;
            }
            if (skippedNodes.Contains(node.Name) && node.Status == NodeStatus.Failed)
            {
                // Fase 13: su corte falló en esta pasada: el nudo en K no se crea sin él.
                result.Outcome = BatchOutcome.Failed;
                result.Reason = "no se creó: falló su corte de diagonal";
                result.Errors.AddRange(node.Errors.Where(e => (e.Path ?? "").StartsWith("batch", StringComparison.Ordinal)));
                return;
            }
            string? why = WhyNotCreatable(node, plan);
            if (why != null)
            {
                result.Reason = why;
                skippedNodes.Add(node.Name);
                return;
            }
            // Fase 13: si su diagonal se partió en esta pasada, el token del plan ya se comprobó en el paso de corte y el nuevo es el que vale.
            bool retokenedNow = retokened.TryGetValue(node.Name, out string? renewedToken);
            if (retokenedNow) item = new BatchCreateItem(item.Node, renewedToken!, item.Spec);
            if (item.Spec == null && !retokenedNow && !string.Equals(item.ValidationToken, node.ValidationToken, StringComparison.OrdinalIgnoreCase))
            {
                result.Outcome = BatchOutcome.Failed;
                result.Reason = "token distinto del plan";
                var error = new ApiError(ErrorCodes.ValidationTokenInvalid,
                    "El validation_token de " + node.Name + " no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo).",
                    "nodes[" + node.Name + "].validation_token",
                    "Relee el plan con conn_batch_plan_get y pasa el validation_token actual de " + node.Name + ".");
                result.Errors.Add(error);
                MarkFailed(node, error);
                skippedNodes.Add(node.Name);
                if (request.StopOnError)
                {
                    stopped = true;
                    report.StoppedAt = node.Name;
                }
                return;
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

        /// <summary>
        /// Fase 12 (D5): los dos nudos de un mismo punto se crean en orden fijo: primero el empalme (<c>N9E</c>: retira los tramos
        /// y pone las planchas) y después el nudo en K (<c>N9</c>: recorta las diagonales contra la cara del tramo que les toca).
        /// El resto conserva el orden de la petición.
        /// </summary>
        public static List<BatchCreateItem> OrderForCreation(IEnumerable<BatchCreateItem> items)
        {
            var list = (items ?? Enumerable.Empty<BatchCreateItem>()).ToList();
            var ordered = new List<BatchCreateItem>();
            var advanced = new HashSet<BatchCreateItem>(); // los empalmes que ya se adelantaron delante de su nudo en K
            foreach (BatchCreateItem item in list)
            {
                if (advanced.Contains(item)) continue;
                string name = item.Node ?? string.Empty;
                if (!name.EndsWith("E", StringComparison.OrdinalIgnoreCase))
                {
                    BatchCreateItem? companion = list.FirstOrDefault(i => !advanced.Contains(i) && !ordered.Contains(i) && string.Equals(i.Node, name + "E", StringComparison.OrdinalIgnoreCase));
                    if (companion != null)
                    {
                        ordered.Add(companion);
                        advanced.Add(companion);
                    }
                }
                ordered.Add(item);
            }
            return ordered;
        }

        /// <summary>
        /// Fase 13 (D5): los pasos del lote en el orden de ejecución: por cada nudo en K con cortes, primero sus cortes, después sus
        /// empalmes B, después (si está en la petición) su empalme del cordón <c>NxE</c> y por último el nudo en K
        /// (cortes → empalmes B → N9E → N9). Los demás nudos conservan el orden de <see cref="OrderForCreation"/>.
        /// </summary>
        public static List<BatchStep> StepsFor(BatchPlan plan, BatchCreateRequest request)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (request == null) throw new ArgumentNullException(nameof(request));
            var steps = new List<BatchStep>();
            var emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<BatchCreateItem> ordered = OrderForCreation(request.Items);

            void EmitSplits(BatchCreateItem kItem)
            {
                if (!emitted.Add(kItem.Node)) return;
                PlanNode? k = plan.Find(kItem.Node);
                if (k == null || k.SplitSteps.Count == 0) return;
                foreach (PlanSplitStep split in k.SplitSteps) steps.Add(new BatchStep(BatchStep.KindSplit, kItem, split, k.Name + " · partir " + split.ElementId));
                foreach (PlanSplitStep split in k.SplitSteps) steps.Add(new BatchStep(BatchStep.KindSpliceB, kItem, split, split.SpliceNode));
            }

            foreach (BatchCreateItem item in ordered)
            {
                string name = item.Node ?? string.Empty;
                if (name.EndsWith("E", StringComparison.OrdinalIgnoreCase))
                {
                    // El empalme del cordón: antes van los cortes y los empalmes B de su nudo en K, si está en la petición.
                    string kName = name.Substring(0, name.Length - 1);
                    BatchCreateItem? kItem = ordered.FirstOrDefault(i => string.Equals(i.Node, kName, StringComparison.OrdinalIgnoreCase));
                    if (kItem != null) EmitSplits(kItem);
                }
                else
                {
                    EmitSplits(item);
                }
                steps.Add(new BatchStep(BatchStep.KindNode, item));
            }
            return steps;
        }

        /// <summary>
        /// Después de <c>batch_delete</c>: los nudos del plan cuya conexión se borró vuelven a <c>ready</c> con su token (la
        /// geometría que el token resume no cambió: <c>conn_delete</c> restauró las barras) y el informe queda en el plan. Fase 13:
        /// los empalmes B borrados vuelven a <c>after_split</c> y los cortes deshechos (<paramref name="unsplitElementIds"/>) a no hechos.
        /// Devuelve cuántos nudos volvieron a listos.
        /// </summary>
        public static int ApplyDeletion(BatchPlan plan, IEnumerable<string> deletedConnectionIds, BatchReport? report = null, IEnumerable<long>? unsplitElementIds = null)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var deleted = new HashSet<string>(deletedConnectionIds ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            var unsplit = new HashSet<long>(unsplitElementIds ?? Enumerable.Empty<long>());
            int restored = 0;
            foreach (PlanNode node in plan.Nodes)
            {
                bool created = node.Status == NodeStatus.Created && node.CreatedConnectionId != null && deleted.Contains(node.CreatedConnectionId);
                bool already = node.Status == NodeStatus.AlreadyConnected && node.ExistingConnectionId != null && deleted.Contains(node.ExistingConnectionId);
                if (created)
                {
                    if (node.IsAfterSplit)
                    {
                        node.Status = NodeStatus.AfterSplit;
                        node.CreatedConnectionId = null;
                        node.Errors.RemoveAll(e => e.Path != null && e.Path.StartsWith("batch", StringComparison.Ordinal));
                        node.StatusDetail = "Su conexión se borró con el lote: se vuelve a crear con el corte de " + (node.SplitOfNode ?? "su nudo en K") + ".";
                    }
                    else
                    {
                        RestoreReady(node);
                    }
                    restored++;
                }
                else if (already)
                {
                    node.StatusDetail = "Su conexión " + node.ExistingConnectionId + " se borró con el lote: replanifica para volver a planificarlo.";
                    node.ExistingConnectionId = null;
                    node.ExistingBatchId = null;
                }
                foreach (PlanSplitStep step in node.SplitSteps.Where(s => s.IsDone && unsplit.Contains(s.ElementId)))
                {
                    step.NewElementId = null;
                    step.SplitId = null;
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
