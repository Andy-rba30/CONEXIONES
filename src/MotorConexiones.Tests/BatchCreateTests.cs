using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Fase 9: crear el lote sin Revit. La petición de <c>conn_batch_create</c>, el recorrido nudo a nudo de
    /// <see cref="BatchRunner"/> con un creador simulado (en Revit, <c>BatchCreator</c>), el informe por nudo, los estados
    /// <c>created</c> / <c>failed</c> del plan, <c>stop_on_error</c>, el borrado del lote y "ya creada en este lote".
    /// </summary>
    public class BatchCreateTests
    {
        private static BatchPlan Hangar8bPlan() => PlanBuilder.Build(BatchPlanTests.Hangar8bRequest());

        private static BatchCreateRequest Parse(string json) => BatchCreateRequest.FromJson(JsonDocument.Parse(json).RootElement.Clone());

        private static CatalogException Rejects(string json) => Assert.Throws<CatalogException>(() => Parse(json));

        /// <summary>Creador simulado: devuelve un id por nudo, nueve elementos y, para los nombres indicados, falla.</summary>
        private sealed class FakeCreator
        {
            public List<string> Called { get; } = new List<string>();
            public HashSet<string> Failing { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public Func<PlanNode, Exception>? ErrorFor { get; set; }

            public BatchNodeOutcome Create(PlanNode node, BatchCreateItem item)
            {
                Called.Add(node.Name);
                if (Failing.Contains(node.Name))
                {
                    throw ErrorFor != null ? ErrorFor(node) : new InvalidOperationException("Advance Steel no pudo escribir la placa de " + node.Name + ".");
                }
                var warnings = new List<ApiError> { new ApiError(ErrorCodes.RevitWarning, "Advertencia de Revit: solo visible en nivel de detalle Fino.") };
                return new BatchNodeOutcome("conn-" + node.Name.ToLowerInvariant(), 9, warnings, updated: node.ReplacesExisting);
            }
        }

        [Fact]
        public void Request_ParsesNodesWithTokensAndRejectsWhatIsMissing()
        {
            BatchCreateRequest request = Parse("{\"plan_id\": \"plan-1\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"abc\"}, {\"node\": \"N7\", \"validation_token\": \"def\", \"spec\": {\"a\": 1}}], \"stop_on_error\": true}");
            Assert.Equal("plan-1", request.PlanId);
            Assert.Equal(2, request.Items.Count);
            Assert.Equal("N4", request.Items[0].Node);
            Assert.Equal("abc", request.Items[0].ValidationToken);
            Assert.Null(request.Items[0].Spec);
            Assert.NotNull(request.Items[1].Spec);
            Assert.True(request.StopOnError);
            Assert.False(request.IncludeSpecs);

            Assert.Equal(ErrorCodes.InvalidRequest, Rejects("{\"nodes\": [{\"node\": \"N4\", \"validation_token\": \"abc\"}]}").Error.Code);
            Assert.Equal("plan_id", Rejects("{\"nodes\": [{\"node\": \"N4\", \"validation_token\": \"abc\"}]}").Error.Path);
            Assert.Equal("nodes", Rejects("{\"plan_id\": \"plan-1\"}").Error.Path);
            Assert.Equal("nodes", Rejects("{\"plan_id\": \"plan-1\", \"nodes\": []}").Error.Path);
            // Nombres sueltos: sin token no se crea nada.
            CatalogException bare = Rejects("{\"plan_id\": \"plan-1\", \"nodes\": [\"N4\"]}");
            Assert.Equal(ErrorCodes.InvalidRequest, bare.Error.Code);
            Assert.Contains("validation_token", bare.Error.Message);
            Assert.Equal("nodes[1].validation_token", Rejects("{\"plan_id\": \"plan-1\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"abc\"}, {\"node\": \"N7\"}]}").Error.Path);
            Assert.Equal("nodes[0].node", Rejects("{\"plan_id\": \"plan-1\", \"nodes\": [{\"validation_token\": \"abc\"}]}").Error.Path);
            Assert.Equal("nudos", Rejects("{\"plan_id\": \"plan-1\", \"nudos\": []}").Error.Path);
            Assert.Equal(ErrorCodes.InvalidRequest, Assert.Throws<CatalogException>(() => BatchCreateRequest.FromJson(JsonDocument.Parse("[]").RootElement.Clone())).Error.Code);
        }

        [Fact]
        public void Request_ForPlan_TakesTheCreatableNodesWithTheirTokens()
        {
            BatchPlan plan = Hangar8bPlan();
            BatchCreateRequest request = BatchCreateRequest.ForPlan(plan);
            Assert.Equal(plan.PlanId, request.PlanId);
            Assert.Equal(16, request.Items.Count);
            Assert.All(request.Items, i => Assert.Equal(plan.Find(i.Node)!.ValidationToken, i.ValidationToken));
            Assert.False(request.StopOnError);
            Assert.Equal("Crear 16 conexiones", PlanAdvice.CreateButtonText(plan));
        }

        [Fact]
        public void Run_CreatesTheSixteenReadyNodesOfTheHangarTruss8b()
        {
            BatchPlan plan = Hangar8bPlan();
            var creator = new FakeCreator();
            BatchReport report = BatchRunner.Run(plan, BatchCreateRequest.ForPlan(plan), creator.Create);

            Assert.Equal(16, creator.Called.Count);
            Assert.Equal(16, report.CreatedCount);
            Assert.Equal(0, report.FailedCount);
            Assert.Equal(0, report.SkippedCount);
            Assert.Equal(0, report.UpdatedCount);
            Assert.False(report.Stopped);
            Assert.Equal(BatchReport.UndoOne, report.UndoEntries);
            Assert.Equal(plan.PlanId, report.BatchId);
            Assert.Equal(16, report.ConnectionIds.Distinct().Count());
            // 14 nudos tenían el aviso de perfil (cordón HSS4X4) y todos reciben un aviso de Revit al crear: todos "con aviso".
            Assert.Equal(16, report.WithWarningsCount);
            Assert.Equal(14, report.Nodes.Count(n => n.Outcome == BatchOutcome.CreatedWithWarnings && plan.Find(n.Node!)!.Warnings.Any(w => w.Code == ErrorCodes.TemplateProfileDiffers)));
            Assert.All(report.Nodes, n => Assert.True(n.IsCreated));
            Assert.All(report.Nodes, n => Assert.Equal(9, n.ElementsCount));
            Assert.StartsWith("Lote " + plan.PlanId.Substring(0, 8) + ": 16 conexiones creadas (16 con aviso).", report.SummaryText);
            Assert.Contains("Una sola entrada de deshacer (Ctrl+Z).", report.SummaryText);

            // El plan: los 16 pasan a created con su conexión; la cabecera y los textos lo dicen; las marcas siguen (las quita el add-in).
            Assert.Equal(16, plan.CreatedCount);
            Assert.Equal(0, plan.ReadyCount);
            Assert.Equal(0, plan.CreatableCount);
            Assert.Same(report, plan.LastReport);
            Assert.True(plan.HasBatchConnections);
            PlanNode n4 = plan.Find("N4")!;
            Assert.Equal(NodeStatus.Created, n4.Status);
            Assert.Equal("conn-n4", n4.CreatedConnectionId);
            Assert.Equal("✔ Creada con aviso", PlanAdvice.StatusText(n4, plan));
            Assert.Equal(PlanAdvice.Amber, PlanAdvice.ColorName(n4));
            Assert.StartsWith("Creada: conexión conn-n4 · Ver en Revit; Borrar el lote la quita · se creó con el aviso del plan", PlanAdvice.Advice(n4, plan));
            Assert.Equal(new[] { PlanAction.Show }, PlanAdvice.Actions(n4, plan).Select(a => a.Key).ToArray());
            PlanNode small = plan.Nodes.First(n => n.Status == NodeStatus.Created && n.ChordElementId == HangarTruss8b.SmallChord);
            Assert.Equal("✔ Creada", PlanAdvice.StatusText(small, plan));
            Assert.Equal(PlanAdvice.Green, PlanAdvice.ColorName(small));
            Assert.True(PlanAdvice.VisibleByDefault(n4));
            Assert.Equal("Creadas 16 conexiones (14 con aviso). Ocultos: 20 sin cordón, 23 barras sueltas.", PlanAdvice.SummaryText(plan));
            Assert.Equal("Crear 0 conexiones", PlanAdvice.CreateButtonText(plan));
        }

        [Fact]
        public void Run_AFailedNodeIsRolledBackAloneAndTheRestAreCreated()
        {
            BatchPlan plan = Hangar8bPlan();
            var creator = new FakeCreator();
            creator.Failing.Add("N7");
            BatchReport report = BatchRunner.Run(plan, BatchCreateRequest.ForPlan(plan), creator.Create);

            Assert.Equal(16, creator.Called.Count);
            Assert.Equal(15, report.CreatedCount);
            Assert.Equal(1, report.FailedCount);
            Assert.False(report.Stopped);
            BatchNodeResult failed = report.Find("N7")!;
            Assert.Equal(BatchOutcome.Failed, failed.Outcome);
            Assert.Equal(ErrorCodes.FabricationFailed, failed.Errors.Single().Code);
            Assert.Contains("Advance Steel no pudo escribir la placa de N7", failed.Errors.Single().Message);
            Assert.Equal("N7: falló (FABRICATION_FAILED: Fallo al modelar N7: Advance Steel no pudo escribir la placa de N7)", failed.Describe());
            Assert.Contains("15 conexiones creadas (15 con aviso), 1 falló (N7: FABRICATION_FAILED: Fallo al modelar N7", report.SummaryText);

            PlanNode n7 = plan.Find("N7")!;
            Assert.Equal(NodeStatus.Failed, n7.Status);
            Assert.Null(n7.CreatedConnectionId);
            Assert.NotNull(n7.ValidationToken);
            Assert.NotNull(n7.Spec);
            Assert.Equal("✖ Falló al crear", PlanAdvice.StatusText(n7, plan));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(n7));
            Assert.StartsWith("Falló al crear (FABRICATION_FAILED: Fallo al modelar N7", PlanAdvice.Advice(n7, plan));
            Assert.EndsWith("corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye", PlanAdvice.Advice(n7, plan));
            Assert.Equal(new[] { PlanAction.Show, PlanAction.Exclude }, PlanAdvice.Actions(n7, plan).Select(a => a.Key).ToArray());
            Assert.StartsWith("Falló al crear (FABRICATION_FAILED): ", n7.StatusDetail);
            Assert.Equal(15, plan.CreatedCount);
            Assert.Equal(1, plan.FailedCount);
            Assert.Equal(1, plan.CreatableCount);
            Assert.Equal("Crear 1 conexión (1 reintento)", PlanAdvice.CreateButtonText(plan));
            Assert.StartsWith("Creadas 15 conexiones (13 con aviso), 1 falló. Pulsa Crear otra vez para reintentar la fallida.", PlanAdvice.SummaryText(plan));

            // Reintento: solo el fallido; ahora sale bien y el plan queda entero.
            creator.Failing.Clear();
            creator.Called.Clear();
            BatchReport retry = BatchRunner.Run(plan, BatchCreateRequest.ForPlan(plan), creator.Create);
            Assert.Equal(new[] { "N7" }, creator.Called);
            Assert.Equal(1, retry.CreatedCount);
            Assert.Equal(NodeStatus.Created, n7.Status);
            Assert.DoesNotContain(n7.Errors, e => (e.Path ?? "").StartsWith("batch"));
            Assert.Equal(16, plan.CreatedCount);
        }

        [Fact]
        public void Run_StopOnError_RevertsEverythingAndSkipsTheRest()
        {
            BatchPlan plan = Hangar8bPlan();
            BatchCreateRequest request = BatchCreateRequest.ForPlan(plan, stopOnError: true);
            string third = request.Items[2].Node;
            var creator = new FakeCreator();
            creator.Failing.Add(third);
            BatchReport report = BatchRunner.Run(plan, request, creator.Create);

            Assert.Equal(3, creator.Called.Count);
            Assert.True(report.Stopped);
            Assert.Equal(third, report.StoppedAt);
            Assert.Equal(0, report.CreatedCount);
            Assert.Equal(2, report.RolledBackCount);
            Assert.Equal(1, report.FailedCount);
            Assert.Equal(13, report.SkippedCount);
            Assert.All(report.Nodes.Skip(3), n => Assert.Equal("no se intentó: el lote se detuvo en " + third, n.Reason));
            Assert.Empty(report.ConnectionIds);
            Assert.StartsWith("Lote " + plan.PlanId.Substring(0, 8) + ": se detuvo en " + third + " y se revirtió todo (stop_on_error): 2 conexiones revertidas, 1 fallida (", report.SummaryText);
            Assert.DoesNotContain("Una sola entrada de deshacer", report.SummaryText);
            // En el plan: los dos creados vuelven a listos (el grupo exterior se revierte), el fallido queda failed.
            Assert.Equal(0, plan.CreatedCount);
            Assert.Equal(15, plan.ReadyCount);
            Assert.Equal(NodeStatus.Failed, plan.Find(third)!.Status);
            Assert.Null(plan.Find(request.Items[0].Node)!.CreatedConnectionId);
        }

        [Fact]
        public void Run_StopOnError_WithoutOuterGroup_KeepsWhatWasCreatedBefore()
        {
            // Plan B (batch_single_undo: false): no hay grupo exterior que revertir, así que lo creado antes del fallo se queda.
            BatchPlan plan = Hangar8bPlan();
            BatchCreateRequest request = BatchCreateRequest.ForPlan(plan, stopOnError: true);
            string third = request.Items[2].Node;
            var creator = new FakeCreator();
            creator.Failing.Add(third);
            BatchReport report = BatchRunner.Run(plan, request, creator.Create, BatchReport.UndoPerNode);

            Assert.True(report.Stopped);
            Assert.Equal(BatchReport.UndoPerNode, report.UndoEntries);
            Assert.Equal(2, report.CreatedCount);
            Assert.Equal(0, report.RolledBackCount);
            Assert.Equal(1, report.FailedCount);
            Assert.Equal(13, report.SkippedCount);
            Assert.Equal(2, plan.CreatedCount);
            Assert.Equal(2, report.ConnectionIds.Count);
            Assert.Contains("se detuvo en " + third + " (stop_on_error): 2 conexiones creadas antes se quedan (una entrada de deshacer por nudo), 1 fallida", report.SummaryText);
        }

        [Fact]
        public void Run_SkipsWhatCannotBeCreatedAndRejectsATokenThatIsNotThePlansOne()
        {
            BatchPlan plan = Hangar8bPlan();
            PlanNode n4 = plan.Find("N4")!;
            PlanNode n7 = plan.Find("N7")!;
            PlanNode pair = plan.Nodes.First(n => n.Status == NodeStatus.NoMatch);
            PlanNode loose = plan.Nodes.First(n => n.Status == NodeStatus.Untyped);
            var items = new List<BatchCreateItem>
            {
                new BatchCreateItem("N4", n4.ValidationToken!),
                new BatchCreateItem("N7", "0000000000000000000000000000000000000000000000000000000000000000"),
                new BatchCreateItem(pair.Name, "x"),
                new BatchCreateItem(loose.Name, "x"),
                new BatchCreateItem("N999", "x"),
                new BatchCreateItem("N4", n4.ValidationToken!),
            };
            var creator = new FakeCreator();
            BatchReport report = BatchRunner.Run(plan, new BatchCreateRequest(plan.PlanId, items), creator.Create);

            Assert.Equal(new[] { "N4" }, creator.Called);
            Assert.Equal(1, report.CreatedCount);
            Assert.Equal(1, report.FailedCount);
            Assert.Equal(4, report.SkippedCount);
            BatchNodeResult badToken = report.Nodes[1];
            Assert.Equal(BatchOutcome.Failed, badToken.Outcome);
            Assert.Equal(ErrorCodes.ValidationTokenInvalid, badToken.Errors.Single().Code);
            Assert.Equal("token distinto del plan", badToken.Reason);
            Assert.Equal(NodeStatus.Failed, n7.Status);
            Assert.Contains("replanifica (el modelo o el plan cambiaron)", PlanAdvice.Advice(n7, plan));
            Assert.StartsWith("no está listo: ✖ ", report.Nodes[2].Reason);
            Assert.StartsWith("no está listo: ○ Barra suelta", report.Nodes[3].Reason);
            Assert.Equal("el nudo no existe en el plan", report.Nodes[4].Reason);
            Assert.Equal(ErrorCodes.InvalidRequest, report.Nodes[4].Errors.Single().Code);
            Assert.Equal("repetido en la petición", report.Nodes[5].Reason);
            Assert.Equal(NodeStatus.Created, n4.Status);

            // Pedir otra vez el creado: se salta "ya creada en este lote".
            BatchReport again = BatchRunner.Run(plan, new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem("N4", n4.ValidationToken!) }), creator.Create);
            Assert.Equal(1, again.SkippedCount);
            Assert.Equal("ya creada en este lote (conexión conn-n4)", again.Nodes[0].Reason);
            Assert.Equal(new[] { "N4" }, creator.Called);

            // Un spec propio en la petición no exige el token del plan (lo comprueba el creador contra el modelo).
            PlanNode other = plan.Nodes.First(n => n.Status == NodeStatus.Ready);
            BatchReport withSpec = BatchRunner.Run(plan, new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem(other.Name, "otro-token", other.Spec) }), creator.Create);
            Assert.Equal(1, withSpec.CreatedCount);
            Assert.Equal(NodeStatus.Created, other.Status);
        }

        [Fact]
        public void Run_ReplaceExisting_ReportsUpdatedAndACatalogExceptionKeepsItsCode()
        {
            PlanRequest request = BatchPlanTests.Request(BatchPlanTests.Overrides("{\"replace_existing\": true}"));
            request.ExistingConnections.Add(new ExistingConnection("conn-1", null, SyntheticTruss.CentralChord, new[] { SyntheticTruss.UpLeft(0) }));
            BatchPlan plan = PlanBuilder.Build(request);
            PlanNode n6 = plan.Find("N6")!;
            Assert.True(n6.ReplacesExisting);
            var creator = new FakeCreator();
            BatchReport report = BatchRunner.Run(plan, new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem("N6", n6.ValidationToken!) }), creator.Create);
            Assert.Equal(BatchOutcome.Updated, report.Nodes[0].Outcome);
            Assert.Equal(1, report.UpdatedCount);
            Assert.Equal(0, report.CreatedCount);
            Assert.Contains("1 rehecha", report.SummaryText);
            Assert.Equal("N6: rehecha (conexión conn-n6, 9 elementos)", report.Nodes[0].Describe());
            Assert.Equal(NodeStatus.Created, n6.Status);
            Assert.Equal("✔ Creada", PlanAdvice.StatusText(n6, plan));
            Assert.StartsWith("Rehecha en este lote", n6.StatusDetail);

            // Un fallo con código propio (el token no vale contra el modelo) conserva el código en el informe y en el nudo.
            PlanNode n18 = plan.Find("N18")!;
            creator.Failing.Add("N18");
            creator.ErrorFor = node => new CatalogException(ErrorCodes.ValidationTokenInvalid, "El modelo cambió desde que se planificó " + node.Name + ".", "validation_token", "Replanifica.");
            BatchReport failed = BatchRunner.Run(plan, new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem("N18", n18.ValidationToken!) }), creator.Create);
            Assert.Equal(ErrorCodes.ValidationTokenInvalid, failed.Nodes[0].Errors.Single().Code);
            Assert.Equal(NodeStatus.Failed, n18.Status);
            Assert.Equal("batch:validation_token", n18.Errors.Last().Path);
        }

        [Fact]
        public void Report_RoundTripsThroughJsonInsideThePlan()
        {
            BatchPlan plan = Hangar8bPlan();
            var creator = new FakeCreator();
            creator.Failing.Add("N4");
            BatchReport report = BatchRunner.Run(plan, BatchCreateRequest.ForPlan(plan), creator.Create);

            BatchReport? copy = BatchReport.FromJson(report.ToJson());
            Assert.NotNull(copy);
            Assert.Equal(report.BatchId, copy!.BatchId);
            Assert.Equal(16, copy.Nodes.Count);
            Assert.Equal(15, copy.CreatedCount);
            Assert.Equal(1, copy.FailedCount);
            Assert.Equal(report.SummaryText, copy.SummaryText);
            Assert.Equal(report.ConnectionIds, copy.ConnectionIds);
            Assert.Null(BatchReport.FromJson("no es json"));

            BatchPlan? planCopy = BatchPlan.FromJson(plan.ToJson());
            Assert.NotNull(planCopy!.LastReport);
            Assert.Equal(15, planCopy.LastReport!.CreatedCount);
            Assert.Equal(NodeStatus.Created, planCopy.Find("N7")!.Status);
            Assert.Equal("conn-n7", planCopy.Find("N7")!.CreatedConnectionId);
            Assert.Equal(NodeStatus.Failed, planCopy.Find("N4")!.Status);
            Assert.Contains("\"created_count\": 15", report.ToJson());
            Assert.Contains("\"summary_text\"", report.ToJson());
        }

        [Fact]
        public void ApplyDeletion_ReturnsTheCreatedNodesToReadyAndKeepsTheDeleteReport()
        {
            BatchPlan plan = Hangar8bPlan();
            var creator = new FakeCreator();
            BatchReport created = BatchRunner.Run(plan, BatchCreateRequest.ForPlan(plan), creator.Create);
            Assert.Equal(16, plan.CreatedCount);

            var deleteReport = new BatchReport { BatchId = plan.PlanId, Operation = BatchReport.OperationDelete };
            foreach (string id in created.ConnectionIds.Take(10))
            {
                deleteReport.Nodes.Add(new BatchNodeResult { ConnectionId = id, Outcome = BatchOutcome.Deleted, ElementsCount = 9, RestoredMembersCount = 3 });
            }
            deleteReport.Nodes.Add(new BatchNodeResult { ConnectionId = created.ConnectionIds[10], Outcome = BatchOutcome.Failed, Errors = { new ApiError(ErrorCodes.InternalError, "Revit no pudo borrar.") } });
            int restored = BatchRunner.ApplyDeletion(plan, deleteReport.Nodes.Where(n => n.Outcome == BatchOutcome.Deleted).Select(n => n.ConnectionId!), deleteReport);

            Assert.Equal(10, restored);
            Assert.Equal(10, plan.ReadyCount);
            Assert.Equal(6, plan.CreatedCount);
            Assert.Same(deleteReport, plan.LastReport);
            Assert.True(deleteReport.IsDelete);
            Assert.Equal(10, deleteReport.DeletedCount);
            Assert.Equal(90, deleteReport.DeletedElementsCount);
            Assert.Equal(30, deleteReport.RestoredMembersCount);
            Assert.StartsWith("Lote " + plan.PlanId.Substring(0, 8) + ": 10 conexiones borradas (90 elementos, 30 barras restauradas), 1 no se pudo borrar (", deleteReport.SummaryText);
            Assert.Contains("Una sola entrada de deshacer (Ctrl+Z).", deleteReport.SummaryText);
            PlanNode ready = plan.Nodes.First(n => n.Status == NodeStatus.Ready);
            Assert.Null(ready.CreatedConnectionId);
            Assert.NotNull(ready.ValidationToken);
            Assert.Equal("Crear 10 conexiones", PlanAdvice.CreateButtonText(plan));
            Assert.StartsWith("Creadas 6 conexiones", PlanAdvice.SummaryText(plan));
            Assert.Contains("Quedan 10 listas sin crear.", PlanAdvice.SummaryText(plan));
        }

        [Fact]
        public void Replan_AfterTheBatch_SaysAlreadyCreatedInThisBatch()
        {
            // Tras crear el lote, replanificar (mismo plan_id) encuentra las conexiones en el modelo: los nudos salen
            // already_connected, pero con el texto "Ya creada en este lote" y el botón de borrar el lote, no "Ya tiene conexión".
            BatchPlan first = Hangar8bPlan();
            var creator = new FakeCreator();
            BatchReport report = BatchRunner.Run(first, BatchCreateRequest.ForPlan(first), creator.Create);

            PlanRequest replan = BatchPlanTests.Hangar8bRequest();
            replan.PlanId = first.PlanId;
            replan.CreatedUtc = first.CreatedUtc;
            replan.LastReport = first.LastReport;
            foreach (PlanNode node in first.Nodes.Where(n => n.Status == NodeStatus.Created))
            {
                replan.ExistingConnections.Add(new ExistingConnection(node.CreatedConnectionId!, first.PlanId, node.ChordElementId, node.MemberElementIds));
            }
            // Una conexión de otro lote sobre una pareja en K (su "cordón" es la diagonal menos inclinada).
            PlanNode pair = first.Nodes.First(n => n.Status == NodeStatus.NoMatch);
            replan.ExistingConnections.Add(new ExistingConnection("conn-otra", "otro-plan", pair.ChordElementId, pair.MemberElementIds));
            BatchPlan plan = PlanBuilder.Build(replan);

            Assert.Equal(first.PlanId, plan.PlanId);
            Assert.Same(report, plan.LastReport);
            Assert.Equal(16, plan.CreatedInBatchCount);
            Assert.True(plan.HasBatchConnections);
            Assert.Equal(0, plan.ReadyCount);
            // Los extremos sueltos de las diagonales creadas siguen siendo barras sueltas (ocultas), no nudos "ya conectados".
            Assert.Equal(23, plan.Nodes.Count(n => n.Status == NodeStatus.Untyped));
            Assert.Equal(17, plan.Nodes.Count(n => n.Status == NodeStatus.AlreadyConnected));
            PlanNode n4 = plan.Find("N4")!;
            Assert.Equal(NodeStatus.AlreadyConnected, n4.Status);
            Assert.Equal("conn-n4", n4.ExistingConnectionId);
            Assert.Equal(first.PlanId, n4.ExistingBatchId);
            Assert.True(n4.IsCreatedInBatch(plan.PlanId));
            Assert.Equal("◌ Ya creada en este lote", PlanAdvice.StatusText(n4, plan));
            Assert.Equal("◌ Ya tiene conexión", PlanAdvice.StatusText(n4));
            Assert.StartsWith("Creada en este lote (conexión conn-n4): Borrar el lote la quita", PlanAdvice.Advice(n4, plan));
            Assert.Equal(new[] { PlanAction.Show, PlanAction.IncludeReplace }, PlanAdvice.Actions(n4, plan).Select(a => a.Key).ToArray());
            Assert.StartsWith("Creada en este lote: conexión conn-n4", n4.StatusDetail);
            Assert.StartsWith("Nada que crear: 16 conexiones ya creadas en este lote (Borrar el lote las quita).", PlanAdvice.SummaryText(plan));

            PlanNode other = plan.Nodes.First(n => n.ExistingConnectionId == "conn-otra");
            Assert.Equal("otro-plan", other.ExistingBatchId);
            Assert.False(other.IsCreatedInBatch(plan.PlanId));
            Assert.Equal("Ya tiene conexión", PlanAdvice.StatusWord(other, plan));
            Assert.Equal(new[] { PlanAction.IncludeReplace }, PlanAdvice.Actions(other, plan).Select(a => a.Key).ToArray());

            // Al borrar el lote, ApplyDeletion limpia la conexión existente de los nudos del plan replanificado.
            int restored = BatchRunner.ApplyDeletion(plan, report.ConnectionIds);
            Assert.Equal(0, restored);
            Assert.Null(n4.ExistingConnectionId);
            Assert.Null(n4.ExistingBatchId);
            Assert.Equal(0, plan.CreatedInBatchCount);
            Assert.Contains("replanifica", n4.StatusDetail);
        }
    }
}
