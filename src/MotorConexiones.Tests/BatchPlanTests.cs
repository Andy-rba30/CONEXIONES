using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// El plan de lote (Fase 8) sin Revit: la plantilla del Detalle D sobre la cercha sintética, con las correcciones de
    /// la sección 3.4 de la propuesta y la validación de siempre (<see cref="SpecValidator"/>) nudo a nudo.
    /// </summary>
    public class BatchPlanTests
    {
        private static readonly Regex Token = new Regex("^[0-9a-f]{64}$");

        private static readonly Lazy<CatalogTemplate> Template = new Lazy<CatalogTemplate>(BuildDetalleDTemplate);

        private static CatalogTemplate DetalleDTemplate() => Template.Value;

        private static CatalogTemplate BuildDetalleDTemplate()
        {
            var (rawJson, spec) = SketchBuilderTests.LoadConfirmedFixture();
            var facts = new FakeModelFacts();
            TemplateNode node = TemplateNode.FromModelFacts(facts, 1249510, new long[] { 1249630, 1249631, 1249636 });
            var metadata = new TemplateMetadata { Name = "Nudo típico Detalle D", DocumentTitle = "HANGAR_PRUEBA_sondeo" };
            return TemplateBuilder.Build(rawJson, spec, node, metadata, CatalogConfig.Default);
        }

        internal static LimitsConfig Limits() => LimitsConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "limits.json")));

        /// <summary>Como ValidationService en Revit: marco del nudo de la especificación, hechos con ese marco y el validador.</summary>
        internal static PlanValidator ValidatorFor(SyntheticTrussFacts facts, LimitsConfig limits) => (json, spec) =>
        {
            try
            {
                var chord = facts.GetMemberFacts(spec.Chord!.ElementId)!;
                var first = facts.GetMemberFacts(spec.Members[0].ElementId)!;
                NodeFrame frame = NodeFrame.Compute(chord.CurveStartMm, chord.CurveEndMm, first.CurveStartMm, first.CurveEndMm);
                ValidationResult result = SpecValidator.Validate(json, spec, facts.WithFrame(frame), limits);
                return new PlanValidation(result.IsValid, result.ValidationToken, result.Errors, result.Warnings);
            }
            catch (NodeGeometryException error)
            {
                return new PlanValidation(false, null, new[] { new ApiError(error.Code, error.Message, "node", error.Hint) }, null);
            }
        };

        internal static PlanRequest Request(BatchOverrides? overrides = null, IEnumerable<DetectorBar>? bars = null)
        {
            var allBars = (bars ?? SyntheticTruss.Bars()).ToList();
            var facts = SyntheticTruss.Facts(allBars);
            var request = new PlanRequest(facts, ValidatorFor(facts, Limits()))
            {
                SelectionIds = allBars.Select(b => b.ElementId).ToList(),
                Templates = new List<CatalogTemplate> { DetalleDTemplate() },
                Overrides = overrides ?? new BatchOverrides(),
                DocumentTitle = "Cercha sintética",
            };
            return request;
        }

        internal static BatchOverrides Overrides(string json) => BatchOverrides.FromJson(JsonDocument.Parse(json).RootElement.Clone());

        [Fact]
        public void Build_PlansTheSyntheticTrussWithTheDetalleDTemplate()
        {
            BatchPlan plan = PlanBuilder.Build(Request());

            Assert.Equal(30, plan.Nodes.Count);
            Dictionary<string, int> summary = plan.Summary();
            Assert.Equal(2, summary[NodeStatus.Ready]);
            Assert.Equal(1, summary[NodeStatus.AmbiguousChord]);
            Assert.Equal(1, summary[NodeStatus.Offset]);
            Assert.Equal(13, summary[NodeStatus.Untyped]);
            Assert.Equal(13, summary[NodeStatus.NoMatch]);
            Assert.Contains("2 ready", plan.Describe());

            PlanNode left = plan.Find("N6")!;
            Assert.Equal(NodeStatus.Ready, left.Status);
            Assert.Equal("same", left.Orientation);
            Assert.False(left.IsMirrored);
            Assert.Equal(0.0, left.MaxDeviationDeg!.Value, 1);
            Assert.Matches(Token, left.ValidationToken!);
            Assert.Equal("Nudo típico Detalle D", left.TemplateName);
            Assert.Equal(plan.PlanId, left.Spec!["source"]!["batch_id"]!.GetValue<string>());
            Assert.Equal(left.TemplateId, left.Spec["source"]!["template_id"]!.GetValue<string>());
            Assert.Equal(SyntheticTruss.CentralChord, left.Spec["chord"]!["element_id"]!.GetValue<long>());
            // Las barras van en el orden de las ranuras: 135° (arriba izquierda), 45° (arriba derecha), −135° (abajo, placa cuchilla).
            var memberIds = left.Spec["members"]!.AsArray().Select(m => m!["element_id"]!.GetValue<long>()).ToArray();
            Assert.Equal(new[] { SyntheticTruss.UpLeft(0), SyntheticTruss.UpRight(0), SyntheticTruss.Lower(0) }, memberIds);
            Assert.Equal("bolted_knife_plate", left.Spec["members"]![2]!["attachment"]!["type"]!.GetValue<string>());
            Assert.Empty(left.Errors);

            PlanNode right = plan.Find("N18")!;
            Assert.Equal(NodeStatus.Ready, right.Status);
            Assert.Equal("mirror_x", right.Orientation);
            Assert.True(right.IsMirrored);
            Assert.Matches(Token, right.ValidationToken!);
            Assert.NotEqual(left.ValidationToken, right.ValidationToken);
            Assert.Equal(SyntheticTruss.Lower(2), right.Spec!["members"]![2]!["element_id"]!.GetValue<long>());

            PlanNode ambiguous = plan.Find("N11")!;
            Assert.Equal(NodeStatus.AmbiguousChord, ambiguous.Status);
            Assert.Null(ambiguous.ValidationToken);
            Assert.Equal(2, ambiguous.ThroughElementIds.Count);

            PlanNode twoBars = plan.Find("N22")!;
            Assert.Equal(NodeStatus.NoMatch, twoBars.Status);
            Assert.Equal(2, twoBars.MemberElementIds.Count);
            Assert.Equal(4, twoBars.Attempts.Count);
            Assert.All(twoBars.Attempts, a => Assert.Contains("sin barra", a));

            // Colores distintos (paleta de la Fase 8) para los nudos marcables: desde la ronda 8c son los visibles por defecto
            // (ni barras sueltas ni parejas sin cordón); el add-in los sustituye por el color del estado al construir el plan.
            var marked = plan.Nodes.Where(n => n.CanBeMarked).ToList();
            Assert.Equal(plan.Nodes.Count(PlanAdvice.VisibleByDefault), marked.Count);
            Assert.DoesNotContain(marked, n => n.Status == NodeStatus.Untyped);
            Assert.Equal(marked.Count, marked.Select(n => n.ColorIndex).Distinct().Count());
            Assert.All(marked, n => Assert.False(string.IsNullOrEmpty(n.ColorName)));
            Assert.Contains(SyntheticTruss.LooseBar, plan.UnusedElementIds);
            Assert.DoesNotContain(SyntheticTruss.CentralChord, plan.UnusedElementIds);
            Assert.Single(plan.Templates);
        }

        [Fact]
        public void Overrides_ChordExcludeAndNoTemplate()
        {
            var overrides = Overrides("{\"chord\": {\"N11\": 100}, \"exclude\": [\"N6\"], \"template\": {\"N18\": null}}");
            BatchPlan plan = PlanBuilder.Build(Request(overrides));

            PlanNode resolved = plan.Find("N11")!;
            Assert.Equal(NodeStatus.Ready, resolved.Status);
            Assert.Equal("same", resolved.Orientation);
            Assert.Equal(SyntheticTruss.CentralChord, resolved.ChordElementId);
            Assert.Matches(Token, resolved.ValidationToken!);

            PlanNode excluded = plan.Find("N6")!;
            Assert.Equal(NodeStatus.Excluded, excluded.Status);
            Assert.Null(excluded.Spec);
            // Ronda 8c: un nudo excluido sigue siendo un nudo de verdad: se ve en la tabla y se marca en gris en el modelo.
            Assert.True(excluded.CanBeMarked);
            Assert.Equal(PlanAdvice.Gray, PlanAdvice.ColorName(excluded));

            PlanNode noTemplate = plan.Find("N18")!;
            Assert.Equal(NodeStatus.NoMatch, noTemplate.Status);
            Assert.Contains("template: null", noTemplate.StatusDetail);
            Assert.Equal(1, plan.ReadyCount);
            Assert.Equal(overrides.Exclude, plan.Overrides.Exclude);
        }

        [Fact]
        public void Overrides_AddAndRemoveMembers()
        {
            // La diagonal corta (40 mm) se añade a mano al nudo de X = 7000: casa en espejo y, como su eje pasa por el
            // punto de trabajo (es un retiro a lo largo de la barra, no un desfase), la regla de NodeReach la da por llegada.
            long shortDiagonal = SyntheticTruss.Lower(3);
            var overrides = Overrides("{\"add_member\": {\"N22\": [" + shortDiagonal + "]}, \"remove_member\": {\"N6\": [" + SyntheticTruss.UpRight(0) + "]}}");
            BatchPlan plan = PlanBuilder.Build(Request(overrides));

            PlanNode added = plan.Find("N22")!;
            Assert.Equal(3, added.MemberElementIds.Count);
            Assert.Equal("mirror_x", added.Orientation);
            Assert.Equal(NodeStatus.Ready, added.Status);
            Assert.True(added.Members.Single(m => m.ElementId == shortDiagonal).ReachesNode);
            Assert.Matches(Token, added.ValidationToken!);

            PlanNode reduced = plan.Find("N6")!;
            Assert.Equal(2, reduced.MemberElementIds.Count);
            Assert.Equal(NodeStatus.NoMatch, reduced.Status);
        }

        [Fact]
        public void Overrides_MergeSplitAndAddNode()
        {
            BatchPlan first = PlanBuilder.Build(Request());
            string looseEnd = first.Nodes.Single(n => n.Status == NodeStatus.Untyped && n.WorkPointMm[0] > 7050 && n.WorkPointMm[0] < 7100).Name;

            var overrides = Overrides("{\"merge\": [[\"N22\", \"" + looseEnd + "\"]], \"split\": {\"N6\": [[100, " + SyntheticTruss.UpLeft(0) + ", " + SyntheticTruss.UpRight(0) + "], [" + SyntheticTruss.Lower(0) + "]]}, "
                                      + "\"add_node\": {\"N18\": [100, " + SyntheticTruss.UpLeft(2) + ", " + SyntheticTruss.UpRight(2) + ", " + SyntheticTruss.Lower(2) + "]}}");
            BatchPlan plan = PlanBuilder.Build(Request(overrides));

            Assert.Equal(30, plan.Nodes.Count); // merge quita uno, split añade uno, add_node sustituye a N18
            PlanNode merged = plan.Find("N22")!;
            Assert.True(merged.IsManual);
            Assert.Equal(3, merged.MemberElementIds.Count);
            Assert.Equal("mirror_x", merged.Orientation);
            Assert.Equal(NodeStatus.Ready, merged.Status);
            Assert.Null(plan.Find(looseEnd));

            PlanNode splitMain = plan.Find("N6")!;
            Assert.Equal(2, splitMain.MemberElementIds.Count);
            Assert.Equal(NodeStatus.NoMatch, splitMain.Status);
            // El segundo grupo solo lleva la diagonal inferior, pero el cordón que atraviesa el punto se reconoce solo.
            PlanNode splitRest = plan.Find("N6-2")!;
            Assert.Equal(NodeStatus.NoMatch, splitRest.Status);
            Assert.Equal(SyntheticTruss.CentralChord, splitRest.ChordElementId);
            Assert.Equal(new[] { SyntheticTruss.Lower(0) }, splitRest.MemberElementIds);
            Assert.Equal(plan.Nodes.IndexOf(splitMain) + 1, plan.Nodes.IndexOf(splitRest));

            PlanNode manual = plan.Find("N18")!;
            Assert.True(manual.IsManual);
            Assert.Equal(NodeStatus.Ready, manual.Status);
            Assert.Equal("mirror_x", manual.Orientation);
            Assert.Equal(SyntheticTruss.CentralChord, manual.ChordElementId);
            Assert.True(manual.ChordContinuous);
        }

        [Fact]
        public void Overrides_SpecPerNode_ReplacesTheTemplateInstanceAndIsValidated()
        {
            BatchPlan first = PlanBuilder.Build(Request());
            JsonObject edited = (JsonObject)JsonNode.Parse(first.Find("N6")!.SpecJson)!;
            edited["dimension_chains"]![0]!["values_mm"]![1] = 402;

            var overrides = new BatchOverrides();
            overrides.Spec["N6"] = edited;
            BatchPlan plan = PlanBuilder.Build(Request(overrides));

            PlanNode node = plan.Find("N6")!;
            Assert.True(node.HasSpecOverride);
            Assert.Equal(NodeStatus.Invalid, node.Status);
            Assert.Contains(node.Errors, e => e.Code == ErrorCodes.DimensionChainMismatch);
            Assert.Equal(plan.PlanId, node.Spec!["source"]!["batch_id"]!.GetValue<string>());
            Assert.Equal(first.Find("N6")!.TemplateId, node.TemplateId);

            // Corregida de nuevo, vuelve a estar lista con otro token (la especificación cambió y lleva otro batch_id).
            edited["dimension_chains"]![0]!["values_mm"]![1] = 420;
            BatchPlan again = PlanBuilder.Build(Request(overrides));
            Assert.Equal(NodeStatus.Ready, again.Find("N6")!.Status);
            Assert.Matches(Token, again.Find("N6")!.ValidationToken!);
        }

        [Fact]
        public void AlreadyConnected_IsSkippedUnlessReplaceExisting()
        {
            PlanRequest request = Request();
            request.ConnectedMembers[SyntheticTruss.UpLeft(0)] = "conn-1";
            BatchPlan plan = PlanBuilder.Build(request);
            PlanNode node = plan.Find("N6")!;
            Assert.Equal(NodeStatus.AlreadyConnected, node.Status);
            Assert.Equal("conn-1", node.ExistingConnectionId);
            Assert.Null(node.ValidationToken);
            // Ronda 8c: se marca en gris ("no se crea"), como los excluidos.
            Assert.True(node.CanBeMarked);
            Assert.Equal(PlanAdvice.Gray, PlanAdvice.ColorName(node));

            PlanRequest replace = Request(Overrides("{\"replace_existing\": true}"));
            replace.ConnectedMembers[SyntheticTruss.UpLeft(0)] = "conn-1";
            PlanNode replaced = PlanBuilder.Build(replace).Find("N6")!;
            Assert.Equal(NodeStatus.Ready, replaced.Status);
            Assert.True(replaced.ReplacesExisting);
            Assert.Equal("conn-1", replaced.ExistingConnectionId);
        }

        [Fact]
        public void Replan_KeepsThePlanIdAndTheNames()
        {
            BatchPlan first = PlanBuilder.Build(Request());
            PlanRequest second = Request(Overrides("{\"exclude\": [\"N22\"]}"));
            second.PlanId = first.PlanId;
            second.CreatedUtc = first.CreatedUtc;
            BatchPlan plan = PlanBuilder.Build(second);
            Assert.Equal(first.PlanId, plan.PlanId);
            Assert.Equal(first.CreatedUtc, plan.CreatedUtc);
            Assert.Equal(first.Nodes.Select(n => n.Name), plan.Nodes.Select(n => n.Name));
            Assert.Equal(first.Find("N6")!.ValidationToken, plan.Find("N6")!.ValidationToken);
            Assert.Equal(NodeStatus.Excluded, plan.Find("N22")!.Status);
        }

        [Fact]
        public void Plan_RoundTripsThroughJson()
        {
            BatchPlan plan = PlanBuilder.Build(Request(Overrides("{\"chord\": {\"N11\": 100}}")));
            string json = plan.ToJson();
            BatchPlan? back = BatchPlan.FromJson(json);
            Assert.NotNull(back);
            Assert.Equal(plan.PlanId, back!.PlanId);
            Assert.Equal(plan.Nodes.Count, back.Nodes.Count);
            Assert.Equal(plan.Find("N6")!.ValidationToken, back.Find("N6")!.ValidationToken);
            Assert.Equal(plan.Find("N6")!.SpecJson, back.Find("N6")!.SpecJson);
            Assert.Equal(100, back.Overrides.Chord["N11"]);
            Assert.Equal(plan.Summary(), back.Summary());
            Assert.Null(BatchPlan.FromJson("no es json"));
            Assert.Contains("\"validation_token\"", json);
            Assert.Contains("\"color_name\"", json);
        }

        [Fact]
        public void Overrides_ParseMergeAndReject()
        {
            BatchOverrides parsed = Overrides("{\"exclude\": [\"N3\", \"N7\"], \"add_node\": {\"N11\": [1, 2, 3]}, \"chord\": {\"N4\": 1249510}, "
                                              + "\"template\": {\"N9\": \"abc\", \"N2\": null}, \"remove_member\": {\"N2\": [9]}, \"add_member\": {\"N2\": [10]}, "
                                              + "\"merge\": [[\"N5\", \"N6\"]], \"split\": {\"N5\": [[1], [2, 3]]}, \"spec\": {\"N4\": {\"spec_version\": \"1.0\"}}, \"replace_existing\": true}");
            Assert.Equal(new[] { "N3", "N7" }, parsed.Exclude);
            Assert.Equal(new long[] { 1, 2, 3 }, parsed.AddNode["N11"]);
            Assert.Equal(1249510, parsed.Chord["N4"]);
            Assert.Equal("abc", parsed.Template["N9"]);
            Assert.Null(parsed.Template["N2"]);
            Assert.Equal(new long[] { 9 }, parsed.RemoveMember["N2"]);
            Assert.Equal(new long[] { 10 }, parsed.AddMember["N2"]);
            Assert.Single(parsed.Merge);
            Assert.Equal(2, parsed.Split["N5"].Count);
            Assert.Equal("1.0", parsed.Spec["N4"]["spec_version"]!.GetValue<string>());
            Assert.True(parsed.ReplaceExisting);
            Assert.False(parsed.IsEmpty);
            Assert.Equal(new long[] { 1, 2, 3, 10, 1249510 }, parsed.ReferencedElementIds().Distinct().OrderBy(i => i).ToArray());

            BatchOverrides merged = parsed.MergeWith(Overrides("{\"include\": [\"N3\"], \"chord\": {\"N4\": null}, \"add_member\": {\"N2\": [9]}, \"exclude\": [\"N8\"]}"));
            Assert.Equal(new[] { "N7", "N8" }, merged.Exclude);
            Assert.False(merged.Chord.ContainsKey("N4"));
            Assert.Equal(new long[] { 10, 9 }, merged.AddMember["N2"]);
            Assert.Empty(merged.RemoveMember["N2"]);
            Assert.True(merged.ReplaceExisting);

            CatalogException unknown = Assert.Throws<CatalogException>(() => Overrides("{\"excluir\": [\"N1\"]}"));
            Assert.Equal(ErrorCodes.InvalidRequest, unknown.Error.Code);
            Assert.Throws<CatalogException>(() => Overrides("{\"chord\": {\"N1\": \"x\"}}"));
            Assert.Throws<CatalogException>(() => Overrides("[]"));
            Assert.True(Overrides("null").IsEmpty);
            Assert.Contains("\"replace_existing\":true", parsed.ToJson());
        }

        [Fact]
        public void Overrides_UnknownNodeNames_AreReportedAsPlanWarnings()
        {
            BatchPlan plan = PlanBuilder.Build(Request(Overrides("{\"chord\": {\"N99\": 100}, \"merge\": [[\"N1\", \"N98\"]], \"add_node\": {\"N50\": [999999]}}")));
            // chord N99, merge N98, add_node con un ID inexistente (ELEMENT_NOT_FOUND) y add_node sin barras legibles.
            Assert.Equal(4, plan.Warnings.Count(w => w.Code == ErrorCodes.InvalidRequest || w.Code == ErrorCodes.ElementNotFound));
            Assert.Contains(plan.Warnings, w => w.Message.Contains("N99"));
            Assert.Contains(plan.Warnings, w => w.Message.Contains("N98"));
            Assert.Contains(plan.Warnings, w => w.Message.Contains("999999"));
        }

        [Fact]
        public void NoTemplates_GivesNoMatchWithAnExplanation()
        {
            PlanRequest request = Request();
            request.Templates.Clear();
            BatchPlan plan = PlanBuilder.Build(request);
            Assert.Equal(0, plan.ReadyCount);
            Assert.Contains("No hay plantillas", plan.Find("N6")!.StatusDetail);
        }

        // ---- Ronda 8b: la cercha real del Hangar con la plantilla oficial del catálogo ----

        internal static CatalogTemplate OfficialDetalleDTemplate()
        {
            string json = File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("catalog", "6abcf116-9b97-485f-b50d-2851ca0018cc.json")));
            return CatalogTemplate.FromJson(json) ?? throw new InvalidOperationException("La plantilla oficial no se pudo leer.");
        }

        internal static PlanRequest Hangar8bRequest()
        {
            var bars = HangarTruss8b.Bars();
            var facts = new SyntheticTrussFacts(bars);
            return new PlanRequest(facts, ValidatorFor(facts, Limits()))
            {
                SelectionIds = bars.Select(b => b.ElementId).ToList(),
                Templates = new List<CatalogTemplate> { OfficialDetalleDTemplate() },
                Overrides = new BatchOverrides(),
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
            };
        }

        internal static PlanRequest HangarRequest(BatchOverrides? overrides = null)
        {
            var bars = HangarTruss.Bars();
            var facts = new SyntheticTrussFacts(bars);
            return new PlanRequest(facts, ValidatorFor(facts, Limits()))
            {
                SelectionIds = bars.Select(b => b.ElementId).ToList(),
                Templates = new List<CatalogTemplate> { OfficialDetalleDTemplate() },
                Overrides = overrides ?? new BatchOverrides(),
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
            };
        }

        [Fact]
        public void Build_PlansTheHangarTrussFromThePcResultsWithTheOfficialTemplate()
        {
            BatchPlan plan = PlanBuilder.Build(HangarRequest());
            Dictionary<string, int> summary = plan.Summary();

            // En el PC (0.8.0) salieron 97 nudos, 93 untyped y 0 ready. Con el corte de los ejes: 53 nudos y los 10 del cordón
            // central casan con la plantilla oficial (cinco como el Detalle D y cinco en espejo) y validan.
            Assert.Equal(53, plan.Nodes.Count);
            Assert.Equal(10, summary[NodeStatus.Ready]);
            Assert.Equal(17, summary[NodeStatus.Untyped]); // 16 extremos sueltos y el empalme 1249515/1249516
            Assert.Equal(26, summary[NodeStatus.NoMatch]);
            Assert.False(summary.ContainsKey(NodeStatus.Invalid));
            Assert.False(summary.ContainsKey(NodeStatus.Offset));

            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            Assert.Equal(5, ready.Count(n => n.Orientation == "same"));
            Assert.Equal(5, ready.Count(n => n.Orientation == "mirror_x"));
            Assert.All(ready, n => Assert.Matches(Token, n.ValidationToken!));
            Assert.All(ready, n => Assert.Equal("Nudo tipico Detalle D", n.TemplateName));
            Assert.All(ready, n => Assert.True(n.ChordContinuous));
            Assert.All(ready, n => Assert.InRange(n.MaxDeviationDeg!.Value, 0.0, 2.0));
            Assert.Equal(10, ready.Select(n => n.ValidationToken).Distinct().Count());

            // El Detalle D: same, desvío ~0 (la plantilla se midió en este nudo), token y la cuchilla en 1249636.
            PlanNode detalleD = ready.Single(n => n.ChordElementId == HangarTruss.DetalleDChord && n.MemberElementIds.Contains(HangarTruss.DetalleDUpLeft));
            Assert.Equal("same", detalleD.Orientation);
            Assert.Equal(0.0, detalleD.MaxDeviationDeg!.Value, 0.2);
            Assert.Equal(plan.PlanId, detalleD.Spec!["source"]!["batch_id"]!.GetValue<string>());
            Assert.Equal(HangarTruss.DetalleDLower, detalleD.Spec["members"]![2]!["element_id"]!.GetValue<long>());
            Assert.Equal(84.5, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDUpLeft).EndGapMm, 1.0);
            Assert.Equal(-11870.0, detalleD.WorkPointMm[0], 1.0);

            // El simétrico del mismo cordón (el que la Fase 7 creó en espejo): mirror_x con la cuchilla en 1249637.
            PlanNode mirror = ready.Single(n => n.MemberElementIds.Contains(HangarTruss.MirrorUpLeft));
            Assert.Equal("mirror_x", mirror.Orientation);
            Assert.Equal(HangarTruss.MirrorLower, mirror.Spec!["members"]![2]!["element_id"]!.GetValue<long>());

            // Donde no se seleccionó el cordón, los tríos y las parejas avisan de que ninguna barra atraviesa el nudo.
            var withoutChord = plan.Nodes.Where(n => n.Status == NodeStatus.NoMatch).ToList();
            Assert.Equal(26, withoutChord.Count);
            Assert.All(withoutChord, n => Assert.False(n.ChordContinuous));
            Assert.All(withoutChord, n => Assert.Contains(n.Warnings, w => w.Code == ErrorCodes.NodeChordNotContinuous));
            Assert.Equal(6, withoutChord.Count(n => n.ElementIds.Count == 3));
            Assert.Equal(20, withoutChord.Count(n => n.ElementIds.Count == 2));
            Assert.DoesNotContain(plan.Nodes.Where(n => n.ChordContinuous), n => n.Warnings.Any(w => w.Code == ErrorCodes.NodeChordNotContinuous));

            // JSON: end_gap_mm viaja en cada barra.
            Assert.Contains("\"end_gap_mm\"", plan.ToJson());
            Assert.Equal(detalleD.Members.Count, BatchPlan.FromJson(plan.ToJson())!.Find(detalleD.Name)!.Members.Count(m => m.EndGapMm > 0));
        }

        [Fact]
        public void Overrides_AddNodeOnTheHangarTruss_UsesTheCutWithTheChordAsWorkPoint()
        {
            // add_node con las cuatro barras del Detalle D: el punto de trabajo sale del corte con el cordón, que atraviesa.
            var overrides = Overrides("{\"add_node\": {\"ND\": [" + HangarTruss.DetalleDChord + ", " + HangarTruss.DetalleDUpLeft + ", "
                                      + HangarTruss.DetalleDUpRight + ", " + HangarTruss.DetalleDLower + "]}}");
            BatchPlan plan = PlanBuilder.Build(HangarRequest(overrides));
            PlanNode manual = plan.Find("ND")!;
            Assert.True(manual.IsManual);
            Assert.True(manual.ChordContinuous);
            Assert.Equal(HangarTruss.DetalleDChord, manual.ChordElementId);
            Assert.Equal(-11870.0, manual.WorkPointMm[0], 1.0);
            Assert.Equal(17423.0, manual.WorkPointMm[2], 0.5);
            Assert.Equal(NodeStatus.Ready, manual.Status);
            Assert.Equal("same", manual.Orientation);
            // El nudo detectado con las mismas barras sigue en el plan (misma selección) y queda ready también.
            Assert.Equal(11, plan.Summary()[NodeStatus.Ready]);
        }

        [Fact]
        public void Build_PlansTheHangarTruss8bExactlyLikeThePc()
        {
            // El plan del paso 8b-4 del PC con la plantilla oficial: 59 nudos, 16 ready (8 same y 8 mirror_x), 20 no_match (las
            // parejas en K sin cordón, con NODE_CHORD_NOT_CONTINUOUS), 23 untyped y 0 invalid; TEMPLATE_PROFILE_DIFFERS en los 14
            // nudos de los tramos HSS4X4 (la plantilla esperaba HSS3X3X1/4) y no en los dos del tramo HSS3X3.
            BatchPlan plan = PlanBuilder.Build(Hangar8bRequest());
            Dictionary<string, int> summary = plan.Summary();
            Assert.Equal(59, plan.Nodes.Count);
            Assert.Equal(16, summary[NodeStatus.Ready]);
            Assert.Equal(20, summary[NodeStatus.NoMatch]);
            Assert.Equal(23, summary[NodeStatus.Untyped]);
            Assert.False(summary.ContainsKey(NodeStatus.Invalid));
            Assert.Empty(plan.UnusedElementIds);

            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            Assert.Equal(8, ready.Count(n => n.Orientation == "same"));
            Assert.Equal(8, ready.Count(n => n.Orientation == "mirror_x"));
            Assert.All(ready, n => Assert.True(n.ChordContinuous));
            Assert.All(ready, n => Assert.Matches(Token, n.ValidationToken!));
            Assert.Equal(16, ready.Select(n => n.ValidationToken).Distinct().Count());
            Assert.All(ready, n => Assert.InRange(n.MaxDeviationDeg!.Value, 0.0, 2.0));
            Assert.All(ready, n => Assert.Equal(plan.PlanId, n.Spec!["source"]!["batch_id"]!.GetValue<string>()));
            Assert.Equal(14, ready.Count(n => n.Warnings.Any(w => w.Code == ErrorCodes.TemplateProfileDiffers)));
            Assert.All(ready.Where(n => n.ChordElementId == HangarTruss8b.SmallChord), n => Assert.Empty(n.Warnings));
            Assert.All(ready.Where(n => n.ChordElementId != HangarTruss8b.SmallChord), n => Assert.Equal(HangarTruss8b.BigChordType, n.Spec!["chord"]!["profile"]!.GetValue<string>()));

            // N4: same con desvío 0 (la plantilla se midió en el Detalle D, que es su gemelo), cuchilla en 1251059 y end_gap_mm
            // hasta el punto de trabajo (84,5 mm; el sondeo 18 mide 86,2 hasta el corte propio de la barra).
            PlanNode n4 = plan.Find("N4")!;
            Assert.Equal(NodeStatus.Ready, n4.Status);
            Assert.Equal("same", n4.Orientation);
            Assert.Equal(0.0, n4.MaxDeviationDeg!.Value, 0.2);
            Assert.Equal(HangarTruss8b.DetalleDChord, n4.ChordElementId);
            Assert.Equal(HangarTruss8b.DetalleDLower, n4.Spec!["members"]![2]!["element_id"]!.GetValue<long>());
            Assert.Equal(84.5, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDUpLeft).EndGapMm, 1.0);
            Assert.Equal("mirror_x", plan.Find("N7")!.Orientation);
            Assert.Equal(HangarTruss8b.MirrorLower, plan.Find("N7")!.Spec!["members"]![2]!["element_id"]!.GetValue<long>());

            // Las 20 parejas en K: dos barras, sin cordón que atraviese, con el aviso; el empalme 1250938/1250939 es untyped.
            var pairs = plan.Nodes.Where(n => n.Status == NodeStatus.NoMatch).ToList();
            Assert.All(pairs, n => Assert.Equal(2, n.ElementIds.Count));
            Assert.All(pairs, n => Assert.False(n.ChordContinuous));
            Assert.All(pairs, n => Assert.Contains(n.Warnings, w => w.Code == ErrorCodes.NodeChordNotContinuous));
            Assert.Equal(NodeStatus.Untyped, plan.Nodes.Single(n => n.ElementIds.Contains(HangarTruss8b.SpliceLeft) && n.ElementIds.Contains(HangarTruss8b.SmallChord)).Status);
        }

        [Fact]
        public void Build_OnlyTheFourBarsOfTheDetalleD_PlansTheNodeReady()
        {
            // Lo que hacen las pruebas 22 y 23 de probar_conexiones.py --puente en Revit: planificar solo las cuatro barras del
            // fixture (cordón 1249510 y sus tres diagonales, que terminan en la cara del cordón). En la Fase 8 (0.8.0) dieron 8
            // nudos untyped; con el corte de los ejes deben dar un nudo ready (el cordón atraviesa) y los otros cinco extremos
            // sueltos (cuatro barras tienen ocho extremos; tres forman el nudo).
            var four = new[] { HangarTruss.DetalleDChord, HangarTruss.DetalleDUpLeft, HangarTruss.DetalleDUpRight, HangarTruss.DetalleDLower };
            var bars = HangarTruss.Bars().Where(b => four.Contains(b.ElementId)).ToList();
            var facts = new SyntheticTrussFacts(bars);
            BatchPlan plan = PlanBuilder.Build(new PlanRequest(facts, ValidatorFor(facts, Limits()))
            {
                SelectionIds = bars.Select(b => b.ElementId).ToList(),
                Templates = new List<CatalogTemplate> { OfficialDetalleDTemplate() },
                Overrides = new BatchOverrides(),
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
            });
            Assert.Equal(6, plan.Nodes.Count);
            Assert.Equal(1, plan.Summary()[NodeStatus.Ready]);
            Assert.Equal(5, plan.Summary()[NodeStatus.Untyped]);
            PlanNode node = plan.Nodes.Single(n => n.Status == NodeStatus.Ready);
            Assert.Equal(HangarTruss.DetalleDChord, node.ChordElementId);
            Assert.True(node.ChordContinuous);
            Assert.Equal("same", node.Orientation);
            Assert.Equal(3, node.Members.Count);
            Assert.Matches(Token, node.ValidationToken!);
            // Los nombres van por la X global: tres extremos sueltos quedan a la izquierda del nudo, que es N4, no N1 (por eso la
            // prueba 22 de probar_conexiones.py no puede buscar "N1" a mano en Revit; el simulador, con un solo nudo, sí da N1).
            Assert.Equal("N4", node.Name);
            Assert.Equal(new[] { "N1", "N2", "N3" }, plan.Nodes.Take(3).Select(n => n.Name));
        }

        [Fact]
        public void Overrides_ToJson_HasNoHelperKeysAndCanBeSentBackAsARequest()
        {
            // Cierre de la Fase 8: el overrides de la respuesta del PC traía "IsEmpty": true; devuelto tal cual en una petición
            // (lo natural para la IA) daba INVALID_REQUEST por clave desconocida.
            BatchOverrides overrides = Overrides("{\"exclude\": [\"N4\"], \"chord\": {\"N9\": 1250933}, \"template\": {\"N7\": null}}");
            string json = overrides.ToJson();
            Assert.DoesNotContain("IsEmpty", json);
            Assert.DoesNotContain("isEmpty", json);
            BatchOverrides back = Overrides(json);
            Assert.Equal(new[] { "N4" }, back.Exclude);
            Assert.Equal(1250933, back.Chord["N9"]);
            Assert.True(back.Template.ContainsKey("N7"));
            Assert.False(back.IsEmpty);

            string empty = new BatchOverrides().ToJson();
            Assert.DoesNotContain("IsEmpty", empty);
            Assert.True(Overrides(empty).IsEmpty);
        }
    }
}
