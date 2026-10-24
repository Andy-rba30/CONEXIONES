using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Types;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Utilidades de las pruebas de la Fase 12: los fixtures B, C, I y K, sus hechos y la validación con el marco único.</summary>
    internal static class Fase12Fixtures
    {
        public static (string Json, ConnectionSpec Spec) Load(string name)
        {
            string json = File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("docs", "fixtures", name + ".json")));
            return (json, ConnectionSpec.FromJson(json) ?? throw new InvalidOperationException("El fixture " + name + " quedó vacío."));
        }

        /// <summary>Dos tramos colineales de un perfil que se tocan en X = 5000 (Z = 17423), con los IDs indicados.</summary>
        public static List<DetectorBar> CollinearBars(long a, long b, string type, double depthMm, double gapMm = 0.0, double kinkDeg = 0.0)
        {
            double z = 17423.0;
            double rad = kinkDeg * Math.PI / 180.0;
            return new List<DetectorBar>
            {
                new DetectorBar(a, SyntheticTruss.P(0, z), SyntheticTruss.P(5000 - gapMm / 2.0, z), type, depthMm),
                new DetectorBar(b, SyntheticTruss.P(5000 + gapMm / 2.0, z), SyntheticTruss.P(5000 + 5000 * Math.Cos(rad), z + 5000 * Math.Sin(rad)), type, depthMm),
            };
        }

        /// <summary>Valida como lo hace el add-in: el marco único de <see cref="SpecFrame"/> y los hechos con ese marco.</summary>
        public static ValidationResult Validate(string json, ConnectionSpec spec, SyntheticTrussFacts facts, LimitsConfig? limits = null)
        {
            NodeFrame frame = SpecFrame.Compute(spec, facts);
            return SpecValidator.Validate(json, spec, facts.WithFrame(frame), limits ?? Fase11Fixtures.RepoLimits());
        }

        public static ValidationResult Validate(ConnectionSpec spec, SyntheticTrussFacts facts, LimitsConfig? limits = null) => Validate(spec.ToJson(), spec, facts, limits);

        public static TemplateNode KNode(double eccentricityMm = KNode55.EccentricityMm) =>
            TemplateNode.FromModelFacts(KNode55.Facts(eccentricityMm), KNode55.Chord, new[] { KNode55.LeftDiagonal, KNode55.RightDiagonal });

        /// <summary>El plan de la cercha de la 8c. <paramref name="splitDiagonals"/> falso = el plan de la Fase 12 (sin los pasos de corte de la Fase 13).</summary>
        public static PlanRequest Request8c(bool starters = true, IEnumerable<CatalogTemplate>? templates = null, bool splitDiagonals = false)
        {
            var bars = HangarTruss8c.Bars();
            var facts = new SyntheticTrussFacts(bars);
            return new PlanRequest(facts, BatchPlanTests.ValidatorFor(facts, BatchPlanTests.Limits()))
            {
                SelectionIds = bars.Select(b => b.ElementId).ToList(),
                Templates = (templates ?? new[] { BatchPlanTests.OfficialDetalleDTemplate() }).ToList(),
                Overrides = new BatchOverrides(),
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
                Choices = Fase11Fixtures.RepoChoices(),
                UseStarters = starters,
                SplitDiagonals = splitDiagonals,
                Limits = BatchPlanTests.Limits(),
            };
        }

        /// <summary>Una plantilla direct_weld guardada desde el fixture del nudo en K sobre el nudo sintético de 54,9 mm.</summary>
        public static CatalogTemplate KTemplate(string name = "Nudo K cordón superior")
        {
            var (json, spec) = Load("nudo-K-cordon-superior");
            return TemplateBuilder.Build(json, spec, KNode(), new TemplateMetadata { Name = name, DocumentTitle = "HANGAR_PRUEBA_sondeo" }, CatalogConfig.Default);
        }
    }

    /// <summary>Fase 12, opción A: el punto de trabajo a mitad entre los cortes, los desvíos y los dos umbrales de la validación.</summary>
    public class EccentricityTests
    {
        [Fact]
        public void Frame_WithTwoEccentricDiagonals_PutsTheWorkPointHalfwayAndMeasuresTheOffsets()
        {
            TemplateNode node = Fase12Fixtures.KNode();
            Assert.Equal(KNode55.WorkPoint.X, node.Frame.Origin.X, 0.2);
            Assert.Equal(KNode55.ChordZ, node.Frame.Origin.Z, 0.2);
            Assert.Equal(54.9, node.EccentricityMm, 0.2);
            Assert.Equal(-27.45, node.Find(KNode55.LeftDiagonal)!.AxisOffsetMm, 0.2);
            Assert.Equal(27.45, node.Find(KNode55.RightDiagonal)!.AxisOffsetMm, 0.2);
            Assert.Equal(-135.6, node.Find(KNode55.LeftDiagonal)!.AngleDeg, 0.2);
            Assert.Equal(-45.0, node.Find(KNode55.RightDiagonal)!.AngleDeg, 0.2);
            Assert.All(node.Members, m => Assert.True(m.ConnectsToNode));
            Assert.True(node.AllMembersOneSide);
            Assert.True(node.ChordContinuous);
            Assert.Equal(203.2, node.ChordWidthMm, 0.1);
            Assert.Equal(304.8, node.ChordDepthMm, 0.1);

            // Con las barras concurrentes el marco es el de siempre (sin excentricidad ni desvíos).
            TemplateNode concurrent = Fase12Fixtures.KNode(0.0);
            Assert.Equal(0.0, concurrent.EccentricityMm, 1e-6);
            Assert.All(concurrent.Members, m => Assert.Equal(0.0, m.AxisOffsetMm, 1e-6));
            Assert.Equal(KNode55.WorkPoint.X, concurrent.Frame.Origin.X, 0.2);
        }

        [Fact]
        public void Validation_UpToHalfTheChordDepth_WarnsAndAboveItFails()
        {
            // Una cartela sobre el nudo en K de 54,9 mm: aviso MEMBER_ECCENTRIC por barra y ningún error.
            StarterResult gusset = SpecStarter.FromNode(Fase12Fixtures.KNode(), Fase11Fixtures.RepoChoices());
            Assert.Equal(54.9, gusset.Spec.Node.EccentricityMm!.Value, 0.2);
            Assert.Equal(-27.45, gusset.Spec.Members[0].AxisOffsetMm!.Value, 0.2);
            ValidationResult result = Fase12Fixtures.Validate(gusset.SpecJson, gusset.Spec, KNode55.Facts());
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Equal(2, result.Warnings.Count(w => w.Code == ErrorCodes.MemberEccentric));
            Assert.Contains(result.Warnings, w => w.Code == ErrorCodes.MemberEccentric && w.Message.Contains("152.4") && w.Message.Contains("54.9"));
            Assert.All(result.Warnings.Where(w => w.Code == ErrorCodes.MemberEccentric), w => Assert.StartsWith("members[", w.Path));

            // Con node_axis_max_offset_mm = 10 el mismo nudo da MEMBER_NOT_AT_NODE (los ejes pasan a unos 19 mm del punto).
            LimitsConfig tight = Fase11Fixtures.RepoLimits();
            tight.NodeAxisMaxOffsetMm = 10.0;
            ValidationResult rejected = Fase12Fixtures.Validate(gusset.SpecJson, gusset.Spec, KNode55.Facts(), tight);
            Assert.Contains(rejected.Errors, e => e.Code == ErrorCodes.MemberNotAtNode);
            Assert.DoesNotContain(rejected.Warnings, w => w.Code == ErrorCodes.MemberEccentric);
            Assert.NotEqual(Fase11Fixtures.RepoLimits().ComputeHash(), tight.ComputeHash());

            // Con 460 mm de excentricidad los ejes pasan a más de medio canto (152,4 mm): error, como antes.
            TemplateNode far = Fase12Fixtures.KNode(460.0);
            StarterResult farGusset = SpecStarter.FromNode(far, Fase11Fixtures.RepoChoices());
            ValidationResult farResult = Fase12Fixtures.Validate(farGusset.SpecJson, farGusset.Spec, KNode55.Facts(460.0));
            Assert.Contains(farResult.Errors, e => e.Code == ErrorCodes.MemberNotAtNode && e.Message.Contains("máximo con aviso"));
        }

        [Fact]
        public void Limits_WithTheDefaults_KeepTheSameHashAsBefore()
        {
            // Las claves nuevas (node_axis_max_offset_mm = 0, end_plate con los mínimos) no entran en el hash: los tokens ya emitidos siguen valiendo.
            LimitsConfig repo = Fase11Fixtures.RepoLimits();
            LimitsConfig bare = LimitsConfig.LoadFromJson(File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("config", "limits.json")))
                .Replace("\"node_axis_max_offset_mm\": 0,", "").Replace("\"min_bolt_to_tube_mm\": 20.0,", "\"min_bolt_to_tube_mm\": 20.0,"));
            Assert.Equal(0.0, repo.NodeAxisMaxOffsetMm);
            Assert.Equal(20.0, repo.EndPlate.MinBoltToTubeMm);
            Assert.Equal(1.0, repo.EndPlate.MinHoleClearanceMm);
            Assert.Equal(152.4, repo.GetMaxAxisOffsetMm(304.8), 1e-6);
            Assert.Equal(40.0, repo.GetMaxAxisOffsetMm(0.0), 1e-6);
            Assert.Equal(bare.ComputeHash(), repo.ComputeHash());
        }

        [Fact]
        public void Regression_TheSixteenNodesOfTheHangar8b_KeepTheirTokensWithTheTopChordSelected()
        {
            // La cercha de la 8c es la de la 8b más el cordón superior: los 16 nudos del Detalle D dan la misma especificación y el
            // mismo token (el punto de trabajo con barras concurrentes no cambia).
            // El token firma también source.batch_id: los dos planes llevan el mismo plan_id para poder compararlos.
            PlanRequest beforeRequest = BatchPlanTests.Hangar8bRequest();
            beforeRequest.PlanId = "regresion-fase-12";
            PlanRequest afterRequest = Fase12Fixtures.Request8c();
            afterRequest.PlanId = "regresion-fase-12";
            BatchPlan before = PlanBuilder.Build(beforeRequest);
            BatchPlan after = PlanBuilder.Build(afterRequest);
            var readyBefore = before.Nodes.Where(n => n.Status == NodeStatus.Ready).ToDictionary(n => n.ChordElementId + "/" + string.Join(",", n.MemberElementIds.OrderBy(i => i)));
            var detalleD = after.Nodes.Where(n => n.Status == NodeStatus.Ready && !n.IsStarter).ToList();
            Assert.Equal(16, detalleD.Count);
            foreach (PlanNode node in detalleD)
            {
                PlanNode old = readyBefore[node.ChordElementId + "/" + string.Join(",", node.MemberElementIds.OrderBy(i => i))];
                Assert.Equal(old.ValidationToken, node.ValidationToken);
                Assert.Equal(old.WorkPointMm, node.WorkPointMm);
            }

            // El Detalle D del fixture confirmado: el token de hoy sobre los hechos del Hangar es el mismo con y sin la Fase 12
            // (la especificación no lleva claves nuevas y los hechos concurren).
            var (json, spec) = SketchBuilderTests.LoadConfirmedFixture();
            ValidationResult result = SpecValidator.Validate(json, spec, new FakeModelFacts(), Fase11Fixtures.RepoLimits());
            Assert.True(result.IsValid);
            Assert.DoesNotContain(result.Warnings, w => w.Code == ErrorCodes.MemberEccentric);
        }

        [Fact]
        public void Geometry_EachMemberIsMeasuredFromItsOwnCut()
        {
            // El croquis de la cartela sobre el nudo de 54,9 mm arranca cada barra desde su corte (±27,45 mm), no desde el origen.
            StarterResult gusset = SpecStarter.FromNode(Fase12Fixtures.KNode(), Fase11Fixtures.RepoChoices());
            SketchNodeInfo info = SketchNodeInfo.FromModelFacts(gusset.Spec, KNode55.Facts());
            Assert.Equal(54.9, info.EccentricityMm, 0.2);
            Assert.Equal(-27.45, info.Find(KNode55.LeftDiagonal)!.AxisOffsetMm, 0.2);
            Assert.Equal(27.45, info.Find(KNode55.RightDiagonal)!.AxisOffsetMm, 0.2);
            Sketch eccentric = SketchBuilder.Build(gusset.Spec, info);
            Assert.Contains(eccentric.Labels, l => l.Text.Contains("desvío"));

            // Lo mismo dibujado con cada barra sola (un nudo con una diagonal cuyo corte es el origen): los extremos coinciden
            // trasladados 27,45 mm, como manda la opción A.
            var leftOnly = new SyntheticTrussFacts(KNode55.Bars().Where(b => b.ElementId != KNode55.RightDiagonal).ToList());
            TemplateNode leftNode = TemplateNode.FromModelFacts(leftOnly, KNode55.Chord, new[] { KNode55.LeftDiagonal });
            Assert.Equal(0.0, leftNode.Find(KNode55.LeftDiagonal)!.AxisOffsetMm, 1e-6);
            Assert.Equal(KNode55.WorkPoint.X - 27.45, leftNode.Frame.Origin.X, 0.2);
            DirectWeldEnd alone = DirectWeldGeometry.ComputeEnd(leftNode.Members[0].Ux, leftNode.Members[0].Uy, 0.0, 101.6, 63.5, 0.0);
            DirectWeldEnd shifted = DirectWeldGeometry.ComputeEnd(info.Find(KNode55.LeftDiagonal)!.Ux, info.Find(KNode55.LeftDiagonal)!.Uy, -27.45, 101.6, 63.5, 0.0);
            Assert.Equal(alone.FacePoint.X - 27.45, shifted.FacePoint.X, 0.3);
            Assert.Equal(alone.FacePoint.Y, shifted.FacePoint.Y, 0.3);
            Assert.Equal(alone.SetbackMm, shifted.SetbackMm, 0.3);
        }

        [Fact]
        public void Schema_AcceptsTheNewKeysAndStillRejectsUnknownOnes()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.Empty(JsonSchemaValidator.Validate(json));
            string withKeys = json.Replace("\"element_ids\": [1249510, 1249630, 1249631, 1249636]", "\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"eccentricity_mm\": 3.4")
                .Replace("\"element_id\": 1249630,", "\"element_id\": 1249630, \"axis_offset_mm\": -1.7,");
            Assert.NotEqual(json, withKeys);
            Assert.Empty(JsonSchemaValidator.Validate(withKeys));
            Assert.Empty(JsonSchemaValidator.Validate(json.Replace("\"element_ids\": [1249510, 1249630, 1249631, 1249636]", "\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"eccentricity_mm\": null")));
            Assert.Contains(JsonSchemaValidator.Validate(json.Replace("\"element_ids\": [1249510, 1249630, 1249631, 1249636]", "\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"eccentricity\": 3.4")), e => e.Code == ErrorCodes.SchemaInvalid);
            ConnectionSpec spec = ConnectionSpec.FromJson(withKeys)!;
            Assert.Equal(3.4, spec.Node.EccentricityMm);
            Assert.Equal(-1.7, spec.Members[0].AxisOffsetMm);
            Assert.Contains("\"axis_offset_mm\"", spec.ToJson());
            // Sin las claves no se escriben (los tokens de las especificaciones de siempre no cambian).
            Assert.DoesNotContain("axis_offset_mm", ConnectionSpec.FromJson(json)!.ToJson());
            Assert.DoesNotContain("eccentricity_mm", ConnectionSpec.FromJson(json)!.ToJson());
        }
    }

    /// <summary>Fase 12: el tipo direct_weld (nudo en K soldado).</summary>
    public class DirectWeldTests
    {
        [Fact]
        public void Fixture_PassesTheSchemaAndValidatesWithOnlyTheEccentricityWarning()
        {
            var (json, spec) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            Assert.Empty(JsonSchemaValidator.Validate(json));
            Assert.True(spec.IsType("direct_weld"));
            ValidationResult result = Fase12Fixtures.Validate(json, spec, KNode55.Facts());
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Equal(2, result.Warnings.Count);
            Assert.All(result.Warnings, w => Assert.Equal(ErrorCodes.MemberEccentric, w.Code));
            Assert.Matches("^[0-9a-f]{64}$", result.ValidationToken);
            Assert.Equal("direct_weld", DirectWeldType.Instance.Name);
            Assert.Empty(JsonSchemaValidator.Validate(DirectWeldType.Instance.GetExampleJson()));
        }

        [Fact]
        public void Validation_RejectsTheObviousMistakes()
        {
            var (json, spec) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            spec.Members[0].Weld!.SizeMm = 2.0;
            Assert.Contains(Fase12Fixtures.Validate(spec, KNode55.Facts()).Warnings, w => w.Code == ErrorCodes.WeldBelowMinimum && w.Path == "members[0].weld.size_mm");

            string negative = json.Replace("\"end_gap_mm\": 0.0", "\"end_gap_mm\": -1.0");
            Assert.Contains(JsonSchemaValidator.Validate(negative), e => e.Code == ErrorCodes.SchemaInvalid && (e.Path ?? "").Contains("end_gap_mm"));
            Assert.Contains(SpecValidator.Validate(negative, ConnectionSpec.FromJson(negative), null, null).Errors, e => e.Code == ErrorCodes.SchemaInvalid);

            string badCut = json.Replace("\"to_chord_face\"", "\"mitre\"");
            Assert.Contains(JsonSchemaValidator.Validate(badCut), e => (e.Path ?? "").Contains("end_cut"));
            string noWeld = json.Replace("\"weld\": { \"type\": \"fillet\", \"size_mm\": 5.0, \"all_around\": true },\n      \"splice_hint\": { \"detail\": \"B\", \"offset_from_chord_face_mm\": 330.0 }\n    },\n    {", "\"splice_hint\": { \"detail\": \"B\", \"offset_from_chord_face_mm\": 330.0 }\n    },\n    {");
            Assert.NotEqual(json, noWeld);
            Assert.Contains(JsonSchemaValidator.Validate(noWeld), e => e.Path == "members[0].weld");

            // Un tramo que no está en node.element_ids.
            spec = ConnectionSpec.FromJson(json)!;
            spec.Members[1].ChordElementId = 999;
            Assert.Contains(Fase12Fixtures.Validate(spec, KNode55.Facts()).Errors, e => e.Code == ErrorCodes.ElementNotFound && e.Path == "members[1].chord_element_id");
        }

        [Fact]
        public void Validation_WarnsWhenAMemberIsWiderThanTheFaceOrTwoFootprintsOverlap()
        {
            // Cordón HSS8X4X1/2 (cara de 203 mm) con una diagonal HSS10X10X1/2 (254 mm): más ancha que la cara.
            double z = 19933.0;
            var wide = new List<DetectorBar>
            {
                new DetectorBar(1, SyntheticTruss.P(0, z), SyntheticTruss.P(10000, z), "HSS8X4X1/2", 203.2),
                new DetectorBar(2, SyntheticTruss.P(5000 - 2000, z - 2000 - 50.8), SyntheticTruss.P(5000 - 50.8, z - 50.8), "HSS10X10X1/2", 254.0),
            };
            var wideFacts = new SyntheticTrussFacts(wide);
            StarterResult wideStarter = SpecStarter.ForKNode(TemplateNode.FromModelFacts(wideFacts, 1, new long[] { 2 }), Fase11Fixtures.RepoChoices());
            ValidationResult wideResult = Fase12Fixtures.Validate(wideStarter.SpecJson, wideStarter.Spec, wideFacts);
            Assert.Contains(wideResult.Warnings, w => w.Code == ErrorCodes.MemberWiderThanChordFace);

            // Dos diagonales a 78° que concurren: sus huellas (63,5 / sin 78° = 64,9 mm, centradas a ±21,6 mm) se pisan sobre la cara.
            double u = Math.Cos(78 * Math.PI / 180.0), v = Math.Sin(78 * Math.PI / 180.0);
            var steep = new List<DetectorBar>
            {
                new DetectorBar(1, SyntheticTruss.P(0, z), SyntheticTruss.P(10000, z), KNode55.ChordType, 304.8),
                SyntheticTruss.Bar(2, SyntheticTruss.P(5000 - u * 2000, z - v * 2000), SyntheticTruss.P(5000 - u * 110, z - v * 110), SyntheticTruss.DiagonalType),
                SyntheticTruss.Bar(3, SyntheticTruss.P(5000 + u * 110, z - v * 110), SyntheticTruss.P(5000 + u * 2000, z - v * 2000), SyntheticTruss.DiagonalType),
            };
            var steepFacts = new SyntheticTrussFacts(steep);
            StarterResult steepStarter = SpecStarter.ForKNode(TemplateNode.FromModelFacts(steepFacts, 1, new long[] { 2, 3 }), Fase11Fixtures.RepoChoices());
            ValidationResult steepResult = Fase12Fixtures.Validate(steepStarter.SpecJson, steepStarter.Spec, steepFacts);
            Assert.Contains(steepResult.Warnings, w => w.Code == ErrorCodes.MembersOverlapOnFace);
            Assert.True(steepResult.IsValid, string.Join("; ", steepResult.Errors.Select(e => e.Code)));
            // Las de 45° del nudo de 54,9 mm no se pisan (quedan a unos 200 mm sobre la cara).
            var (_, kSpec) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            Assert.DoesNotContain(Fase12Fixtures.Validate(kSpec, KNode55.Facts()).Warnings, w => w.Code == ErrorCodes.MembersOverlapOnFace);
        }

        [Fact]
        public void Sketch_DrawsTheChordTheCutMembersTheGapAndTheSpliceMark()
        {
            var (_, spec) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            SketchNodeInfo info = SketchNodeInfo.FromModelFacts(spec, KNode55.Facts());
            Assert.False(info.IsApproximate);
            Assert.Equal(304.8, info.ChordDepthMm, 0.1);
            Sketch sketch = SketchBuilder.Build(spec, info);
            Assert.DoesNotContain(sketch.Notes, n => n.Contains("aproximad"));
            Assert.True(sketch.Lines.Count(l => l.Kind == SketchKind.ChordEdge) >= 2);
            Assert.Equal(2, sketch.Polygons.Count(p => p.Kind == SketchKind.MemberOutline));
            Assert.Equal(2, sketch.Dimensions.Count(d => d.Path != null && d.Path.EndsWith(".splice_hint.offset_from_chord_face_mm") && Math.Abs(d.ValueMm - 330.0) < 0.01));
            Assert.Contains(sketch.Dimensions, d => d.Path == "node.eccentricity_mm" && Math.Abs(d.ValueMm - 54.9) < 0.5);
            Assert.Contains(sketch.Dimensions, d => d.Text.Contains("entre huellas"));
            Assert.Equal(2, sketch.Dimensions.Count(d => d.Path != null && d.Path.EndsWith(".end_gap_mm")));
            Assert.Contains(sketch.Labels, l => l.Text.Contains("corte contra la cara"));
            Assert.Contains(sketch.Lines, l => l.Kind == SketchKind.Weld);
            // Sin modelo: el croquis aproximado sigue saliendo (las dos barras por debajo del cordón).
            Sketch approximate = SketchBuilder.Build(spec, SketchNodeInfo.FromSpecAngles(spec));
            Assert.True(approximate.Notes.Count > 0);
            Assert.Equal(2, approximate.Polygons.Count(p => p.Kind == SketchKind.MemberOutline));
        }

        [Fact]
        public void Starter_ForKNode_WritesAValidSpecWithTheChoicesValues()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            StarterResult starter = SpecStarter.ForKNode(Fase12Fixtures.KNode(), choices);
            Assert.Equal("direct_weld", starter.ConnectionType);
            Assert.Equal("arranque direct_weld", starter.TemplateName);
            Assert.Null(starter.DetailName);
            Assert.Contains("soldadura directa", starter.HeaderText);
            Assert.Empty(JsonSchemaValidator.Validate(starter.SpecJson));
            Assert.Equal(54.9, starter.Spec.Node.EccentricityMm!.Value, 0.2);
            Assert.Equal("auto", starter.Spec.Chord!.Face);
            Assert.True(starter.Spec.Chord.Continuous);
            Assert.Equal(2, starter.Spec.Members.Count);
            foreach (MemberSpec member in starter.Spec.Members)
            {
                Assert.Equal("diagonal", member.Role);
                Assert.Equal("to_chord_face", member.EndCut);
                Assert.Equal(0.0, member.EndGapMm);
                Assert.Equal(5.0, member.Weld!.SizeMm);
                Assert.True(member.Weld.AllAround);
                Assert.Equal("B", member.SpliceHint!.Detail);
                Assert.Equal(330.0, member.SpliceHint.OffsetFromChordFaceMm);
                Assert.Null(member.ChordElementId);
            }
            Assert.Equal(-27.45, starter.Spec.Members[0].AxisOffsetMm!.Value, 0.2);
            Assert.Equal(27.45, starter.Spec.Members[1].AxisOffsetMm!.Value, 0.2);
            ValidationResult result = Fase12Fixtures.Validate(starter.SpecJson, starter.Spec, KNode55.Facts());
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Equal(2, result.Warnings.Count(w => w.Code == ErrorCodes.MemberEccentric));

            // La regla D4: el nudo en K es direct_weld; con barras a los dos lados, cartela; un empalme, end_plate.
            Assert.Equal("direct_weld", SpecStarter.TypeFor(Fase12Fixtures.KNode()));
            Assert.Equal("gusset_node", SpecStarter.TypeFor(Fase11Fixtures.HangarNode()));
            var spliceFacts = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(1, 2, "HSS3X3X1/4", 76.2));
            Assert.Equal("end_plate", SpecStarter.TypeFor(TemplateNode.FromModelFacts(spliceFacts, 1, new long[] { 2 })));
            // Con una plantilla direct_weld del mismo perfil de cordón mandan sus valores (D10): soldadura 6.
            CatalogTemplate template = Fase12Fixtures.KTemplate();
            template.SpecTemplate!["members"]![0]!["weld"]!["size_mm"] = 6.0;
            StarterResult fromTemplate = SpecStarter.ForKNode(Fase12Fixtures.KNode(), choices, new[] { template });
            Assert.Equal(6.0, fromTemplate.Spec.Members[0].Weld!.SizeMm);
            Assert.Same(template, fromTemplate.DefaultsTemplate);
            // Y el desplegable Tipo: el mismo nudo como cartela o como soldadura.
            Assert.Equal("gusset_node", SpecStarter.ForType("gusset_node", Fase12Fixtures.KNode(), choices).ConnectionType);
            Assert.Throws<CatalogException>(() => SpecStarter.ForType("end_plate", Fase12Fixtures.KNode(), choices));
        }

        [Fact]
        public void Starter_OnAKNodeOverASplice_SendsEachDiagonalToItsOwnSegment()
        {
            // El primer empalme de la 8c (N9): el nudo en K sobre los tramos 1245531 y 1245530 con las diagonales 1251056 y 1251049.
            var facts = HangarTruss8c.Facts();
            List<DetectedNode> nodes = NodeDetector.Detect(HangarTruss8c.Bars());
            DetectedNode k = nodes.Single(n => n.AtSplice && n.MemberElementIds.Contains(HangarTruss8c.FirstSpliceDiagonalLeft));
            TemplateNode node = TemplateNode.FromModelFacts(facts, k.ChordElementId, k.MemberElementIds, k.SpliceElementId, k.ChordContinuous);
            Assert.False(node.ChordContinuous);
            Assert.Equal(k.SpliceElementId, node.SpliceElementId);
            Assert.Equal(4, node.AllElementIds.Count);
            TemplateNodeMember toOther = node.Members.Single(m => m.ChordElementId.HasValue);
            TemplateNodeMember toChord = node.Members.Single(m => !m.ChordElementId.HasValue);
            // La diagonal que baja hacia +X llega al tramo de la derecha (1245530); la que baja hacia −X, al de la izquierda (1245531).
            long rightSegment = HangarTruss8c.FirstSpliceRight, leftSegment = HangarTruss8c.FirstSpliceLeft;
            if (node.ChordElementId == leftSegment)
            {
                Assert.Equal(rightSegment, toOther.ChordElementId);
                Assert.Equal(HangarTruss8c.FirstSpliceDiagonalRight, toOther.ElementId);
            }
            else
            {
                Assert.Equal(leftSegment, toOther.ChordElementId);
                Assert.Equal(HangarTruss8c.FirstSpliceDiagonalLeft, toOther.ElementId);
            }
            StarterResult starter = SpecStarter.ForKNode(node, Fase11Fixtures.RepoChoices());
            Assert.False(starter.Spec.Chord!.Continuous);
            Assert.Contains(starter.Spec.Node.ElementIds, id => id == k.SpliceElementId!.Value);
            Assert.Equal(toOther.ChordElementId, starter.Spec.Members.Single(m => m.ElementId == toOther.ElementId).ChordElementId);
            Assert.Null(starter.Spec.Members.Single(m => m.ElementId == toChord.ElementId).ChordElementId);
            ValidationResult result = Fase12Fixtures.Validate(starter.SpecJson, starter.Spec, facts);
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
        }
    }

    /// <summary>Fase 12: el tipo end_plate (placa de extremo, empalme).</summary>
    public class EndPlateTests
    {
        private static SyntheticTrussFacts FactsForB() => new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(9101, 9102, "HSS2-1/2X2-1/2X3/16", 63.5));
        private static SyntheticTrussFacts FactsForI(string type = "HSS3X3X1/4", double depth = 76.2) => new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(9201, 9202, type, depth));

        [Fact]
        public void TheThreeFixtures_PassTheSchemaAndValidateWithoutErrors()
        {
            foreach (var (name, facts) in new (string, SyntheticTrussFacts)[] { ("detalle-B", FactsForB()), ("detalle-C", HangarTruss8c.Facts()), ("detalle-I", FactsForI()) })
            {
                var (json, spec) = Fase12Fixtures.Load(name);
                Assert.Empty(JsonSchemaValidator.Validate(json));
                Assert.Equal("end_plate", spec.ConnectionType);
                Assert.Equal("splice", spec.Kind);
                Assert.Equal(60.0, spec.Plate!.CornerCutMm);
                Assert.Equal("both_sides", spec.Bolts!.Nuts);
                Assert.EndsWith(name + ".png", spec.Source!.DrawingImage);
                Assert.True(File.Exists(SketchBuilderTests.FindRepoFile(spec.Source.DrawingImage!)), name + ".png");
                ValidationResult result = Fase12Fixtures.Validate(json, spec, facts);
                Assert.True(result.IsValid, name + ": " + string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Path + " " + e.Message)));
                Assert.Empty(result.Warnings);
                Assert.Matches("^[0-9a-f]{64}$", result.ValidationToken);
            }
            Assert.Equal("end_plate", EndPlateType.Instance.Name);
            Assert.Empty(JsonSchemaValidator.Validate(EndPlateType.Instance.GetExampleJson()));
            Assert.Equal(3, JsonSchemaValidator.KnownConnectionTypes.Length);
            Assert.NotEqual(JsonSchemaValidator.GetEndPlateSchemaJson(), JsonSchemaValidator.GetDirectWeldSchemaJson());
            Assert.NotEqual(JsonSchemaValidator.GetGussetNodeSchemaJson(), JsonSchemaValidator.GetEndPlateSchemaJson());
        }

        [Fact]
        public void DetalleC_HasTwelveBoltsAroundTheTubeAndTheBoltLengthOfTwoPlates()
        {
            var (_, spec) = Fase12Fixtures.Load("detalle-C");
            List<BoltRow> rows = BoltRing.Rows(spec.Plate, spec.Bolts);
            Assert.Equal(4, rows.Count);
            Assert.Equal(12, BoltRing.Count(spec.Bolts));
            List<BoltPosition> bolts = BoltRing.Positions(spec.Plate, spec.Bolts);
            Assert.Equal(12, bolts.Count);
            double[] xTop = bolts.Where(b => Math.Abs(b.Y - 140.0) < 0.01).Select(b => Math.Round(b.X, 3)).OrderBy(x => x).ToArray();
            Assert.Equal(new[] { -135.0, -45.0, 45.0, 135.0 }, xTop);
            Assert.Equal(4, bolts.Count(b => Math.Abs(b.Y + 140.0) < 0.01));
            double[] ySide = bolts.Where(b => Math.Abs(b.X - 190.0) < 0.01).Select(b => Math.Round(b.Y, 3)).OrderBy(y => y).ToArray();
            Assert.Equal(new[] { -40.0, 40.0 }, ySide);
            Assert.Equal(2, bolts.Count(b => Math.Abs(b.X + 190.0) < 0.01));
            // Ninguno a menos de 25 del borde (polígono con las esquinas recortadas) ni a menos de 20 de la cara del tubo (305 × 203).
            Polygon2D octagon = Octagon.Polygon(430, 330, 60);
            Assert.Equal(8, octagon.Vertices.Count);
            var tube = new TubeFootprint(304.8, 203.2);
            foreach (BoltPosition b in bolts)
            {
                var p = new Point2D(b.X, b.Y);
                Assert.True(octagon.ContainsPoint(p));
                Assert.True(Geometry2DChecks.DistanceToBoundary(octagon, p) >= 25.0 - 0.01);
                Assert.True(tube.DistanceTo(b.X, b.Y) >= 20.0);
                Assert.InRange(tube.DistanceTo(b.X, b.Y), 37.0, 40.0);
            }
            LimitsConfig limits = Fase11Fixtures.RepoLimits();
            BoltStack stack = BoltStack.ForTwoPlates(12.7, 12.7, 0.0, spec.Bolts, limits);
            Assert.Equal(25.4, stack.GripMm, 1e-6);
            Assert.Equal(25.4, stack.LengthAdditionMm, 1e-6);
            Assert.Equal(50.8, stack.BoltLengthMm, 1e-6); // 2 × 12,7 + 25,4 = 50,8 = 8 × 1/4"
            Assert.Equal(50.8, limits.ComputeBoltLengthMm(25.4, 19.05), 1e-6);
            // Una filas por cara como patrón rectangular de N × 1 para Advance Steel.
            BoltRow top = rows.Single(r => r.Face == "top");
            Assert.Equal(4, top.Count);
            Assert.True(top.IsHorizontal);
            Assert.Equal(4, top.ToGrid().Positions.Count);
        }

        [Fact]
        public void DetallesBAndI_HaveFourBoltsAtTheRightDistance()
        {
            var (_, b) = Fase12Fixtures.Load("detalle-B");
            List<BoltPosition> boltsB = BoltRing.Positions(b.Plate, b.Bolts);
            Assert.Equal(4, boltsB.Count);
            Assert.Equal(135.0, boltsB.Max(p => p.Y) - boltsB.Min(p => p.Y), 1e-6);
            Assert.Equal(135.0, boltsB.Max(p => p.X) - boltsB.Min(p => p.X), 1e-6);
            Assert.Equal(8, Octagon.Points(185, 185, 60).Count);
            var (_, i) = Fase12Fixtures.Load("detalle-I");
            List<BoltPosition> boltsI = BoltRing.Positions(i.Plate, i.Bolts);
            Assert.Equal(150.0, boltsI.Max(p => p.Y) - boltsI.Min(p => p.Y), 1e-6);
            Assert.Equal(4, Octagon.Points(200, 200, 0).Count);
            Assert.Single(i.UncertainFields);
            Assert.NotNull(i.UncertainFields[0].UserConfirmedValue);
        }

        [Fact]
        public void Validation_RejectsTheObviousMistakes()
        {
            LimitsConfig limits = Fase11Fixtures.RepoLimits();
            // Borde de 10 en la fila de arriba del C.
            var (_, c) = Fase12Fixtures.Load("detalle-C");
            c.Bolts!.Faces!.Top!.EdgeMm = 10.0;
            Assert.Contains(Fase12Fixtures.Validate(c, HangarTruss8c.Facts()).Errors, e => e.Code == ErrorCodes.BoltEdgeDistanceTooSmall && e.Path == "bolts.faces.top.edge_mm");
            // El I sobre un tubo HSS4X4 (101,6): los pernos quedan a 24,2 de la cara: pasa; con min_bolt_to_tube_mm = 30, no.
            var (_, i) = Fase12Fixtures.Load("detalle-I");
            i.Chord!.Profile = "HSS4X4X3/16";
            i.Members[0].Profile = "HSS4X4X3/16";
            SyntheticTrussFacts big = FactsForI("HSS4X4X3/16", 101.6);
            ValidationResult ok = Fase12Fixtures.Validate(i, big, limits);
            Assert.True(ok.IsValid, string.Join("; ", ok.Errors.Select(e => e.Code + " " + e.Message)));
            LimitsConfig strict = Fase11Fixtures.RepoLimits();
            strict.EndPlate.MinBoltToTubeMm = 30.0;
            Assert.Contains(Fase12Fixtures.Validate(i, big, strict).Errors, e => e.Code == ErrorCodes.BoltTooCloseToTube);
            Assert.NotEqual(limits.ComputeHash(), strict.ComputeHash());
            // Agujero de 15 para un perno de 15,875.
            var (_, b) = Fase12Fixtures.Load("detalle-B");
            b.Bolts!.HoleMm = 15.0;
            b.Bolts.HoleLabel = null;
            Assert.Contains(Fase12Fixtures.Validate(b, FactsForB()).Errors, e => e.Code == ErrorCodes.BoltHoleTooSmall);
            // Rótulo que no cuadra con el agujero.
            var (_, b2) = Fase12Fixtures.Load("detalle-B");
            b2.Bolts!.HoleLabel = "Ø21";
            Assert.Contains(Fase12Fixtures.Validate(b2, FactsForB()).Errors, e => e.Code == ErrorCodes.LabelValueMismatch && e.Path == "bolts.hole_label");
            // Plancha más pequeña que el tubo.
            var (_, b3) = Fase12Fixtures.Load("detalle-B");
            b3.Plate!.WidthMm = 60.0;
            Assert.Contains(Fase12Fixtures.Validate(b3, FactsForB()).Errors, e => e.Code == ErrorCodes.PlateSmallerThanTube);
            // Retiro que no deja sitio a la plancha.
            var (_, b4) = Fase12Fixtures.Load("detalle-B");
            b4.Chord!.EndSetbackMm = 4.0;
            Assert.Contains(Fase12Fixtures.Validate(b4, FactsForB()).Errors, e => e.Code == ErrorCodes.SpliceSetbackTooSmall && e.Path == "chord.end_setback_mm");
            // Si los tramos ya dejan 6 mm de hueco en el modelo, el retiro necesario baja (D8).
            var gapped = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(9101, 9102, "HSS2-1/2X2-1/2X3/16", 63.5, gapMm: 12.0));
            Assert.True(Fase12Fixtures.Validate(b4, gapped).IsValid);
            // Tramos a 8°: no es un empalme.
            var kinked = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(9101, 9102, "HSS2-1/2X2-1/2X3/16", 63.5, kinkDeg: 8.0));
            var (_, b5) = Fase12Fixtures.Load("detalle-B");
            Assert.Contains(Fase12Fixtures.Validate(b5, kinked).Errors, e => e.Code == ErrorCodes.SpliceNotCollinear);
            // Perfiles distintos en los dos tramos: aviso, no error.
            var mixed = new SyntheticTrussFacts(new List<DetectorBar>
            {
                new DetectorBar(9201, SyntheticTruss.P(0, 17423), SyntheticTruss.P(5000, 17423), "HSS4X4X3-16 102x102", 101.6),
                new DetectorBar(9202, SyntheticTruss.P(5000, 17423), SyntheticTruss.P(10000, 17423), "HSS3X3X1/4", 76.2),
            });
            var (_, i2) = Fase12Fixtures.Load("detalle-I");
            i2.Chord!.Profile = "HSS4X4X3-16 102x102";
            i2.Plate!.WidthMm = 230;
            i2.Plate.HeightMm = 230;
            ValidationResult mixedResult = Fase12Fixtures.Validate(i2, mixed);
            Assert.Contains(mixedResult.Warnings, w => w.Code == ErrorCodes.SpliceProfilesDiffer);
            // Longitud de perno declarada demasiado corta: aviso.
            var (_, c2) = Fase12Fixtures.Load("detalle-C");
            c2.Bolts!.LengthMm = 30.0;
            Assert.Contains(Fase12Fixtures.Validate(c2, HangarTruss8c.Facts()).Warnings, w => w.Code == ErrorCodes.BoltLengthTooShort);
        }

        [Fact]
        public void Sketch_DrawsThePlateTheTubeTheBoltsAndTheDimensionsWithTheirPaths()
        {
            var (_, spec) = Fase12Fixtures.Load("detalle-C");
            SketchNodeInfo info = SketchNodeInfo.FromModelFacts(spec, HangarTruss8c.Facts());
            Assert.False(info.IsApproximate);
            Sketch sketch = SketchBuilder.Build(spec, info);
            Assert.Equal(12, sketch.Circles.Count(c => c.Kind == SketchKind.Bolt));
            Assert.Contains(sketch.Polygons, p => p.Kind == SketchKind.Gusset && p.Points.Count == 8);
            Assert.Contains(sketch.Dimensions, d => d.Path == "plate.width_mm" && Math.Abs(d.ValueMm - 430) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "plate.height_mm" && Math.Abs(d.ValueMm - 330) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "plate.corner_cut_mm" && Math.Abs(d.ValueMm - 60) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "bolts.faces.top.spacing_mm" && Math.Abs(d.ValueMm - 90) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "bolts.faces.left.spacing_mm" && Math.Abs(d.ValueMm - 80) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "bolts.faces.top.edge_mm" && Math.Abs(d.ValueMm - 25) < 0.01);
            Assert.Contains(sketch.Dimensions, d => d.Path == "chord.end_setback_mm");
            Assert.Contains(sketch.Dimensions, d => d.Path == "members[0].end_setback_mm");
            Assert.Contains(sketch.Labels, l => l.Text.Contains("50,8") || l.Text.Contains("50.8"));
            // Sin modelo (miniatura del catálogo): también.
            Sketch approximate = SketchBuilder.Build(spec, SketchNodeInfo.FromSpecAngles(spec));
            Assert.Equal(12, approximate.Circles.Count(c => c.Kind == SketchKind.Bolt));
        }

        [Fact]
        public void Starter_ForSplice_PicksTheDetailOfTheProfileOrScalesTheI()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            // HSS12X8X1/2 → Detalle C (430 × 330, 12 pernos de 3/4").
            var c = HangarTruss8c.Facts();
            TemplateNode spliceC = TemplateNode.FromModelFacts(c, HangarTruss8c.FirstSpliceLeft, new[] { HangarTruss8c.FirstSpliceRight });
            Assert.True(spliceC.IsSplice);
            Assert.False(spliceC.ChordContinuous);
            StarterResult starterC = SpecStarter.ForSplice(spliceC, choices);
            Assert.Equal("end_plate", starterC.ConnectionType);
            Assert.Equal("C", starterC.DetailName);
            Assert.Equal("arranque end_plate (Detalle C)", starterC.TemplateName);
            Assert.Equal(430.0, starterC.Spec.Plate!.WidthMm);
            Assert.Equal(330.0, starterC.Spec.Plate.HeightMm);
            Assert.Equal(12, BoltRing.Count(starterC.Spec.Bolts));
            Assert.Equal(19.05, starterC.Spec.Bolts!.DiameterMm!.Value, 1e-6);
            Assert.Equal(20.638, starterC.Spec.Bolts.HoleMm!.Value, 1e-3);
            Assert.Equal("splice", starterC.Spec.Kind);
            Assert.Equal(0.0, starterC.Spec.GapMm);
            Assert.Equal(12.7, starterC.Spec.Chord!.EndSetbackMm!.Value, 0.1);
            Assert.Equal(12.7, starterC.Spec.Members[0].EndSetbackMm!.Value, 0.1);
            Assert.Equal("chord", starterC.Spec.Members[0].Role);
            Assert.Empty(JsonSchemaValidator.Validate(starterC.SpecJson));
            ValidationResult result = Fase12Fixtures.Validate(starterC.SpecJson, starterC.Spec, c);
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));

            // HSS3X3X1/4 → I; HSS2-1/2X2-1/2X3/16 → B; HSS4X4X3/16 → I escalado (230 × 230, un perno por cara).
            var i = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(1, 2, "HSS3X3X1/4", 76.2));
            Assert.Equal("I", SpecStarter.ForSplice(TemplateNode.FromModelFacts(i, 1, new long[] { 2 }), choices).DetailName);
            var b = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(1, 2, "HSS2-1/2X2-1/2X3/16", 63.5));
            StarterResult starterB = SpecStarter.ForSplice(TemplateNode.FromModelFacts(b, 1, new long[] { 2 }), choices);
            Assert.Equal("B", starterB.DetailName);
            Assert.Equal(185.0, starterB.Spec.Plate!.WidthMm);
            Assert.True(Fase12Fixtures.Validate(starterB.SpecJson, starterB.Spec, b).IsValid);
            var big = new SyntheticTrussFacts(Fase12Fixtures.CollinearBars(1, 2, "HSS4X4X3/16", 101.6));
            StarterResult scaled = SpecStarter.ForSplice(TemplateNode.FromModelFacts(big, 1, new long[] { 2 }), choices);
            Assert.Equal("I escalado", scaled.DetailName);
            Assert.Equal(230.0, scaled.Spec.Plate!.WidthMm);
            Assert.Equal(230.0, scaled.Spec.Plate.HeightMm);
            Assert.Equal(4, BoltRing.Count(scaled.Spec.Bolts));
            ValidationResult scaledResult = Fase12Fixtures.Validate(scaled.SpecJson, scaled.Spec, big);
            Assert.True(scaledResult.IsValid, string.Join("; ", scaledResult.Errors.Select(e => e.Code + " " + e.Message)));
            // El escalado reproduce el C en tamaño de plancha para un HSS12X8 (305 + 124 → 430; 203 + 124 → 330).
            EndPlateDetailDefaults c2 = choices.Starter.EndPlate.Default.Scale(304.8, 203.2);
            Assert.Equal(430.0, c2.WidthMm);
            Assert.Equal(330.0, c2.HeightMm);
            // Un nudo que no es empalme no admite la placa de extremo.
            Assert.Throws<CatalogException>(() => SpecStarter.ForSplice(Fase12Fixtures.KNode(), choices));
        }

        [Fact]
        public void Panel_ListsTheSectionsOfEachTypeAndWritesTheHoleWithItsLabel()
        {
            var (jsonC, c) = Fase12Fixtures.Load("detalle-C");
            List<SpecField> fields = SpecEditor.ListFields(c);
            var sections = fields.Select(f => f.Section).Distinct().ToList();
            Assert.Equal(new[] { "Tramo 1", "Tramo 2", "Plancha", "Pernos", "Empalme", "Cadenas de cotas", "Origen" }, sections.ToArray());
            Assert.Contains(fields, f => f.Path == "bolts.faces.top.count" && f.Value == "4");
            Assert.Contains(fields, f => f.Path == "bolts.faces.right.spacing_mm" && f.Value.StartsWith("80"));
            Assert.Contains(fields, f => f.Path == "plate.corner_cut_mm" && f.Label.Contains("Esquina recortada"));
            Assert.Contains(fields, f => f.Path == "gap_mm");
            Assert.DoesNotContain(fields, f => f.Section == "Cartela");
            Assert.True(SpecEditor.TrySetLabeledValue(jsonC, "bolts.hole_mm", "bolts.hole_label", "Ø18", out string newJson, out string error), error);
            ConnectionSpec edited = ConnectionSpec.FromJson(newJson)!;
            Assert.Equal(18.0, edited.Bolts!.HoleMm);
            Assert.Equal("Ø18", edited.Bolts.HoleLabel);
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            Assert.Equal(4, choices.HoleOptions().Count);
            Assert.Equal(20.638, choices.HoleOptions().Single(o => o.Label == "Ø13/16\"").ValueMm, 1e-3);

            var (_, k) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            List<SpecField> kFields = SpecEditor.ListFields(k);
            Assert.Contains(kFields, f => f.Section == "Cordón" && f.Path == "chord.face");
            Assert.Contains(kFields, f => f.Section == "Cordón" && f.Path == "node.eccentricity_mm" && !f.IsEditable);
            Assert.Contains(kFields, f => f.Section.StartsWith("Barra 1") && f.Path == "members[0].axis_offset_mm" && !f.IsEditable);
            Assert.Contains(kFields, f => f.Path == "members[0].splice_hint.offset_from_chord_face_mm" && f.Value.StartsWith("330"));
            Assert.Contains(kFields, f => f.Path == "members[1].end_gap_mm");
            Assert.DoesNotContain(kFields, f => f.Path.Contains("attachment"));
            // Una cartela sobre un nudo excéntrico enseña el desvío (solo lectura); la de siempre, no.
            StarterResult gusset = SpecStarter.FromNode(Fase12Fixtures.KNode(), choices);
            Assert.Contains(SpecEditor.ListFields(gusset.Spec), f => f.Path == "members[0].axis_offset_mm" && !f.IsEditable);
            var (_, d) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.DoesNotContain(SpecEditor.ListFields(d), f => f.Path.EndsWith("axis_offset_mm"));
        }
    }

    /// <summary>Fase 12: la detección de la clase splice y el plan de lote con los arranques por tipo.</summary>
    public class Fase12PlanTests
    {
        [Fact]
        public void Detector_TwoCollinearSegmentsThatMeet_AreASpliceNotALooseBar()
        {
            List<DetectedNode> nodes = NodeDetector.Detect(Fase12Fixtures.CollinearBars(1, 2, "HSS3X3X1/4", 76.2));
            DetectedNode splice = nodes.Single(n => n.Kind == NodeKind.Splice);
            Assert.Equal(NodeStatus.Detected, splice.Status);
            Assert.False(splice.ChordContinuous);
            Assert.Equal(new long[] { 1, 2 }, splice.AllElementIds.OrderBy(i => i).ToArray());
            Assert.Equal(5000.0, splice.WorkPointMm.X, 0.01);
            Assert.NotNull(splice.Frame);
            Assert.Equal(1.0, splice.Frame!.X.X, 1e-9);
            Assert.Equal(1.0, splice.Frame.Y.Z, 1e-9);
            Assert.Single(splice.Members);
            Assert.True(Math.Abs(splice.Members[0].AngleDeg) < 0.01 || Math.Abs(Math.Abs(splice.Members[0].AngleDeg) - 180.0) < 0.01);
            Assert.Equal(2, nodes.Count(n => n.Status == NodeStatus.Untyped)); // los dos extremos lejanos
            Assert.Equal("N1E", NodeDetector.NextName(new[] { "N1" }) == "N2" ? "N1E" : "");
            Assert.True(NodeDetector.NameNumber("N9E") > NodeDetector.NameNumber("N9") && NodeDetector.NameNumber("N9E") < NodeDetector.NameNumber("N9-2"));
            Assert.True(NodeDetector.NameNumber("N9-2") < NodeDetector.NameNumber("N10"));
        }

        [Fact]
        public void Detector_HangarTruss8c_FindsTheKNodesTheSevenSplicesAndTheirCompanions()
        {
            List<DetectedNode> nodes = NodeDetector.Detect(HangarTruss8c.Bars());
            var splices = nodes.Where(n => n.Kind == NodeKind.Splice).ToList();
            Assert.Equal(8, splices.Count); // los 7 del cordón superior y el 1250938/1250939 del cordón central
            var topSplices = splices.Where(n => n.WorkPointMm.Z > 19000).OrderBy(n => n.WorkPointMm.X).ToList();
            Assert.Equal(7, topSplices.Count);
            for (int i = 0; i < 7; i++) Assert.Equal(HangarTruss8c.SpliceX[i], topSplices[i].WorkPointMm.X, 0.6);
            Assert.All(topSplices, s => Assert.NotNull(s.Companion));
            Assert.All(topSplices, s => Assert.Equal(s.Companion!.Name + "E", s.Name));
            Assert.All(topSplices, s => Assert.Equal(nodes.IndexOf(s.Companion!) + 1, nodes.IndexOf(s)));
            var kAtSplice = nodes.Where(n => n.AtSplice).ToList();
            Assert.Equal(7, kAtSplice.Count);
            Assert.Equal(6, kAtSplice.Count(n => n.MemberElementIds.Count == 2));
            Assert.Single(kAtSplice, n => n.MemberElementIds.Count == 1); // N52: una sola diagonal (1251717)
            Assert.All(kAtSplice, n => Assert.All(n.Members, m => Assert.True(m.ReachesNode)));
            // Los 8 nudos en K con el cordón atravesando: 6 con dos diagonales concurrentes y 2 (N5 y N54) con unos 55 mm de excentricidad.
            var kThrough = nodes.Where(n => n.ChordContinuous && n.WorkPointMm.Z > 19000 && n.MemberElementIds.Count == 2).ToList();
            Assert.Equal(8, kThrough.Count);
            Assert.Equal(6, kThrough.Count(n => n.EccentricityMm < 1.0));
            var eccentric = kThrough.Where(n => n.EccentricityMm >= 1.0).ToList();
            Assert.Equal(2, eccentric.Count);
            Assert.All(eccentric, n => Assert.InRange(n.EccentricityMm, 50.0, 60.0));
            DetectedNode n5 = eccentric.Single(n => n.MemberElementIds.Contains(HangarTruss8c.EccentricKLeft));
            Assert.Equal(HangarTruss8c.EccentricKChord, n5.ChordElementId);
            Assert.Equal(-9278.1, n5.WorkPointMm.X, 1.0);
            Assert.All(n5.Members, m => Assert.True(m.ReachesNode));
            Assert.All(n5.Members, m => Assert.InRange(Math.Abs(m.AxisOffsetMm), 25.0, 30.0));
            // N49 y N58: una diagonal sobre un tramo que atraviesa.
            Assert.Equal(2, nodes.Count(n => n.ChordContinuous && n.WorkPointMm.Z > 19000 && n.MemberElementIds.Count == 1));
            // Nada más cambia respecto a la 8b: los 16 nudos del cordón central siguen ahí.
            Assert.Equal(16, nodes.Count(n => n.ChordContinuous && n.MemberElementIds.Count == 3));
        }

        [Fact]
        public void Plan_HangarTruss8c_CreatesTheKNodesWithDirectWeldAndTheSplicesWithDetalleC()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c());
            Dictionary<string, int> summary = plan.Summary();
            // 16 del Detalle D + 17 soldaduras directas + 7 empalmes del cordón superior (Detalle C) + el empalme del cordón central
            // 1250938/1250939 (Detalle I, por el perfil HSS3X3 del tramo elegido como cordón).
            Assert.Equal(41, summary[NodeStatus.Ready]);
            Assert.False(summary.ContainsKey(NodeStatus.Invalid), string.Join(" | ", plan.Nodes.Where(n => n.Status == NodeStatus.Invalid).Select(n => n.Name + ": " + string.Join(", ", n.Errors.Select(e => e.Code + " " + e.Message)))));
            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            Assert.Equal(16, ready.Count(n => !n.IsStarter));
            var welds = ready.Where(n => n.ConnectionType == "direct_weld").ToList();
            var splices = ready.Where(n => n.ConnectionType == "end_plate").ToList();
            Assert.Equal(17, welds.Count); // 8 nudos en K con el cordón atravesando, 7 sobre empalme, N49 y N58 con una barra
            Assert.Equal(8, splices.Count);
            Assert.All(welds, n => Assert.Equal("arranque direct_weld", n.TemplateName));
            Assert.Equal(7, splices.Count(n => n.TemplateName == "arranque end_plate (Detalle C)"));
            Assert.Single(splices, n => n.TemplateName == "arranque end_plate (Detalle I)" && n.ElementIds.Contains(HangarTruss8b.SpliceLeft));
            Assert.All(splices, n => Assert.True(n.IsSplice));
            Assert.Equal(7, splices.Count(n => n.Name.EndsWith("E")));
            Assert.All(ready, n => Assert.Matches("^[0-9a-f]{64}$", n.ValidationToken!));
            Assert.Equal(41, ready.Select(n => n.ValidationToken).Distinct().Count());
            Assert.All(ready, n => Assert.Equal(plan.PlanId, n.Spec!["source"]!["batch_id"]!.GetValue<string>()));
            Assert.Equal(7, welds.Count(n => n.AtSplice));
            Assert.Equal(7, welds.Count(n => n.Warnings.Any(w => w.Code == ErrorCodes.NodeAtSplice)));
            var eccentricWelds = welds.Where(n => n.Warnings.Any(w => w.Code == ErrorCodes.MemberEccentric)).ToList();
            Assert.True(eccentricWelds.Count == 2, string.Join(" | ", eccentricWelds.Select(n => n.Name + " ecc=" + n.EccentricityMm + " " + string.Join(";", n.Warnings.Where(w => w.Code == ErrorCodes.MemberEccentric).Select(w => w.Message)))));
            Assert.Equal(3, welds.Count(n => n.MemberElementIds.Count == 1)); // N49, N58 y el nudo en K de una diagonal sobre el último empalme
            Assert.Equal(2, welds.Count(n => n.MemberElementIds.Count == 1 && n.ChordContinuous));
            Assert.DoesNotContain(welds, n => n.Warnings.Any(w => w.Code == ErrorCodes.NodeChordNotContinuous));
            // Cada nudo en K sobre un empalme tiene su empalme justo detrás y una diagonal al otro tramo.
            foreach (PlanNode k in welds.Where(n => n.AtSplice))
            {
                PlanNode splice = plan.Find(k.Name + "E")!;
                Assert.Equal(NodeStatus.Ready, splice.Status);
                Assert.Contains(k.SpliceElementId!.Value, splice.ElementIds);
                Assert.Contains(k.ChordElementId, splice.ElementIds);
                Assert.Equal(plan.Nodes.IndexOf(k) + 1, plan.Nodes.IndexOf(splice));
                Assert.Equal(k.MemberElementIds.Count == 2 ? 1 : k.Members.Count(m => m.ChordElementId.HasValue), k.Members.Count(m => m.ChordElementId.HasValue));
                Assert.Equal(k.SpliceElementId, k.Spec!["members"]!.AsArray().Select(m => m!["chord_element_id"]?.GetValue<long>()).FirstOrDefault(id => id.HasValue));
            }
            // La cabecera cuenta las plantillas y los arranques aparte.
            string header = PlanAdvice.SummaryText(plan);
            Assert.StartsWith("Se crearán 41 conexiones: 16 con Nudo tipico Detalle D (8 iguales, 8 en espejo), 17 soldaduras directas (arranque), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I). 14 avisan de perfil distinto. 2 avisan de excentricidad. 7 nudos en K están sobre un empalme.", header);
            Assert.Contains("Ocultos:", header);
            // Textos y botones del nudo en K y del empalme.
            PlanNode k5 = welds.Single(n => n.MemberElementIds.Contains(HangarTruss8c.EccentricKLeft));
            Assert.Equal("▲ Listo con aviso (arranque)", PlanAdvice.StatusText(k5, plan)); // Fase 13 (D2): el arranque se ve en el estado
            Assert.Contains("Sin plantilla: se creará con el arranque direct_weld (soldadura directa)", PlanAdvice.Advice(k5, plan));
            Assert.Contains("excentricidad", PlanAdvice.Advice(k5, plan));
            Assert.Contains(PlanAction.Edit, PlanAdvice.Actions(k5, plan).Select(a => a.Key));
            PlanNode firstSplice = splices.Where(n => n.WorkPointMm[2] > 19000).OrderBy(n => n.WorkPointMm[0]).First();
            Assert.Contains("empalme", PlanAdvice.MapLabel(firstSplice, plan));
            Assert.Equal(PlanAdvice.Green, PlanAdvice.ColorName(firstSplice));
            // El mapa: el empalme con su clase.
            TrussMap map = TrussMap.Build(plan, HangarTruss8c.Bars());
            Assert.Equal(8, map.Nodes.Count(n => n.IsSplice));
            Assert.Equal(64, map.Segments.Count);
            // El JSON del plan lleva kind, is_starter, connection_type, splice_element_id, eccentricity_mm y axis_offset_mm y vuelve igual.
            string json = plan.ToJson();
            Assert.Contains("\"kind\": \"splice\"", json);
            Assert.Contains("\"is_starter\": true", json);
            Assert.Contains("\"connection_type\": \"direct_weld\"", json);
            Assert.Contains("\"splice_element_id\":", json);
            BatchPlan copy = BatchPlan.FromJson(json)!;
            Assert.Equal(8, copy.Nodes.Count(n => n.IsSplice));
            Assert.Equal(k5.EccentricityMm, copy.Find(k5.Name)!.EccentricityMm);
            Assert.Equal(k5.Members[0].AxisOffsetMm, copy.Find(k5.Name)!.Members[0].AxisOffsetMm);
        }

        [Fact]
        public void Plan_HangarTruss8c_WithADirectWeldTemplate_UsesItInsteadOfTheStarter()
        {
            CatalogTemplate k = Fase12Fixtures.KTemplate();
            Assert.Equal("direct_weld", k.ConnectionType);
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(templates: new[] { BatchPlanTests.OfficialDetalleDTemplate(), k }));
            var welds = plan.Nodes.Where(n => n.Status == NodeStatus.Ready && n.ConnectionType == "direct_weld").ToList();
            Assert.Equal(17, welds.Count);
            // Los 14 nudos con dos diagonales (8 con el cordón atravesando y 6 sobre empalme) casan con la plantilla (same o mirror_x,
            // dentro de la tolerancia de 10°); los tres de una barra (N49, N58 y el K sobre el último empalme) no la llenan y siguen
            // con el arranque.
            Assert.Equal(14, welds.Count(n => n.TemplateId == k.TemplateId));
            Assert.Equal(3, welds.Count(n => n.IsStarter));
            Assert.All(welds.Where(n => n.TemplateId == k.TemplateId), n => Assert.Equal(k.Name, n.TemplateName));
            Assert.All(welds.Where(n => n.TemplateId == k.TemplateId), n => Assert.Contains(n.Orientation, new[] { "same", "mirror_x" }));
            Assert.Equal(8, plan.Nodes.Count(n => n.Status == NodeStatus.Ready && n.ConnectionType == "end_plate"));
            Assert.StartsWith("Se crearán 41 conexiones: 30 con 2 plantillas (Nudo tipico Detalle D: 16, Nudo K cordón superior: 14)", PlanAdvice.SummaryText(plan));
            // La plantilla no lleva la excentricidad ni los desvíos (son del nudo); instanciada sobre el nudo de 54,9 mm, valida con el aviso.
            Assert.DoesNotContain("axis_offset_mm", k.SpecTemplate!.ToJsonString());
            Assert.DoesNotContain("eccentricity_mm", k.SpecTemplate.ToJsonString());
            TemplateProposal proposal = TemplateChooser.Propose(Fase12Fixtures.KNode(), new[] { k });
            Assert.True(proposal.HasChoice);
            InstantiationResult instance = TemplateInstantiator.Instantiate(k, proposal.Chosen!.Match, Fase12Fixtures.KNode(), CatalogConfig.Default);
            ValidationResult result = Fase12Fixtures.Validate(instance.SpecJson, instance.Spec, KNode55.Facts());
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Equal(2, result.Warnings.Count(w => w.Code == ErrorCodes.MemberEccentric));
            // La miniatura de la plantilla sin modelo.
            Sketch thumb = TemplateSketch.Build(k);
            Assert.Equal(2, thumb.Polygons.Count(p => p.Kind == SketchKind.MemberOutline));
        }

        [Fact]
        public void Plan_WithoutStartersOrTemplates_SaysWhyEachTypeIsMissing()
        {
            PlanRequest request = Fase12Fixtures.Request8c(starters: false);
            BatchPlan plan = PlanBuilder.Build(request);
            Assert.Equal(16, plan.ReadyCount);
            var splices = plan.Nodes.Where(n => n.IsSplice).ToList();
            Assert.Equal(8, splices.Count);
            Assert.All(splices, n => Assert.Equal(NodeStatus.NoMatch, n.Status));
            Assert.All(splices, n => Assert.Contains("end_plate", n.StatusDetail));
            Assert.Equal(8, PlanAdvice.SummaryText(plan).Contains("8 empalmes del cordón (sin plantilla)") ? 8 : 0);
            PlanNode k = plan.Nodes.First(n => n.AtSplice);
            Assert.Equal(NodeStatus.NoMatch, k.Status);
            Assert.False(PlanAdvice.ChordMissing(k));
            Assert.Equal("✖ Sin plantilla que encaje", PlanAdvice.StatusText(k, plan));
        }

        [Fact]
        public void Plan_SyntheticTruss_WithStarters_WeldsEverySingleSidedNode()
        {
            PlanRequest request = BatchPlanTests.Request();
            request.UseStarters = true;
            request.SplitDiagonals = false; // el plan de la Fase 12 (los pasos de corte de la Fase 13 se prueban en Fase13Tests)
            request.Choices = Fase11Fixtures.RepoChoices();
            BatchPlan plan = PlanBuilder.Build(request);
            Dictionary<string, int> summary = plan.Summary();
            // 2 del Detalle D más 13 arranques: los 12 nudos de una diagonal de los cordones superior e inferior y el de X = 7000
            // (dos diagonales arriba, la de abajo no llegó): todos con las barras a un solo lado.
            Assert.Equal(15, summary[NodeStatus.Ready]);
            Assert.False(summary.ContainsKey(NodeStatus.NoMatch));
            Assert.Equal(13, plan.Nodes.Count(n => n.IsStarter));
            Assert.All(plan.Nodes.Where(n => n.IsStarter), n => Assert.Equal("direct_weld", n.ConnectionType));
            Assert.StartsWith("Se crearán 15 conexiones: 2 con Nudo típico Detalle D (1 igual, 1 en espejo), 13 soldaduras directas (arranque).", PlanAdvice.SummaryText(plan));
            // El nudo de X = 7000 avisa de que las huellas se pisan (dos diagonales a 45° sobre un HSS3X3 de 76 mm).
            PlanNode n22 = plan.Find("N22")!;
            Assert.True(n22.IsStarter);
            Assert.Contains(n22.Warnings, w => w.Code == ErrorCodes.MembersOverlapOnFace);
            // Con el catálogo vacío los arranques siguen: ya no es "ningún nudo listo". Fase 13 (D2): los dos nudos del Detalle D
            // también se arrancan, con la cartela automática, y la cabecera los cuenta aparte.
            request.Templates = new List<CatalogTemplate>();
            BatchPlan empty = PlanBuilder.Build(request);
            Assert.Equal(15, empty.ReadyCount);
            Assert.False(empty.Summary().ContainsKey(NodeStatus.NoMatch));
            var autoGussets = empty.Nodes.Where(n => n.IsStarter && n.ConnectionType == "gusset_node").ToList();
            Assert.Equal(2, autoGussets.Count);
            Assert.All(autoGussets, n => Assert.Equal("auto", n.Spec!["gusset"]!["outline"]!["mode"]!.GetValue<string>()));
            Assert.All(autoGussets, n => Assert.Contains(n.Warnings, w => w.Code == ErrorCodes.TemplateNoMatch));
            Assert.All(autoGussets, n => Assert.Equal("▲ Listo con aviso (arranque)", PlanAdvice.StatusText(n, empty)));
            Assert.Contains("2 cartelas automáticas (arranque)", PlanAdvice.SummaryText(empty));
            Assert.True(PlanAdvice.IsCatalogEmpty(empty));
        }

        [Fact]
        public void Plan_HangarTruss8b_WithStarters_SplicesTheCentralChordWithTheScaledI()
        {
            PlanRequest request = BatchPlanTests.Hangar8bRequest();
            request.UseStarters = true;
            request.Choices = Fase11Fixtures.RepoChoices();
            BatchPlan plan = PlanBuilder.Build(request);
            Assert.Equal(17, plan.ReadyCount);
            PlanNode splice = plan.Nodes.Single(n => n.IsSplice);
            Assert.Equal(NodeStatus.Ready, splice.Status);
            // El tramo elegido como cordón es el HSS3X3 (1250939, el más largo): el Detalle I tal cual; el otro tramo es HSS4X4 y avisa.
            Assert.Equal("arranque end_plate (Detalle I)", splice.TemplateName);
            Assert.Equal(HangarTruss8b.SmallChord, splice.ChordElementId);
            Assert.Contains(splice.Warnings, w => w.Code == ErrorCodes.SpliceProfilesDiffer);
            Assert.Equal(200.0, splice.Spec!["plate"]!["width_mm"]!.GetValue<double>());
            Assert.Contains("1 empalme (arranque Detalle I)", PlanAdvice.SummaryText(plan));
            Assert.Equal(20, plan.Nodes.Count(n => n.Status == NodeStatus.NoMatch)); // las parejas sin cordón no tienen arranque
        }

        [Fact]
        public void BatchRunner_CreatesTheSpliceBeforeItsKNode()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c());
            PlanNode k = plan.Nodes.First(n => n.AtSplice && n.Status == NodeStatus.Ready);
            PlanNode splice = plan.Find(k.Name + "E")!;
            PlanNode other = plan.Nodes.First(n => n.Status == NodeStatus.Ready && !n.AtSplice && !n.IsSplice);
            var request = new BatchCreateRequest(plan.PlanId, new[]
            {
                new BatchCreateItem(other.Name, other.ValidationToken!),
                new BatchCreateItem(k.Name, k.ValidationToken!),
                new BatchCreateItem(splice.Name, splice.ValidationToken!),
            });
            var order = new List<string>();
            BatchReport report = BatchRunner.Run(plan, request, (node, item) =>
            {
                order.Add(node.Name);
                if (node.Name == splice.Name) throw new InvalidOperationException("falla el empalme");
                return new BatchNodeOutcome("conn-" + node.Name, 3);
            }, BatchReport.UndoPerNode);
            Assert.Equal(new[] { other.Name, splice.Name, k.Name }, order.ToArray());
            Assert.Equal(NodeStatus.Failed, splice.Status);
            Assert.Equal(NodeStatus.Created, k.Status); // el fallo de uno no toca al otro
            Assert.Equal(NodeStatus.Created, other.Status);
            Assert.Equal(1, report.FailedCount);
            var ordered = BatchRunner.OrderForCreation(new[] { new BatchCreateItem("N12", "a"), new BatchCreateItem("N9", "b"), new BatchCreateItem("N9E", "c") });
            Assert.Equal(new[] { "N12", "N9E", "N9" }, ordered.Select(i => i.Node).ToArray());
        }

        [Fact]
        public void Catalog_SavesAndAppliesAnEndPlateTemplate()
        {
            var facts = HangarTruss8c.Facts();
            TemplateNode splice = TemplateNode.FromModelFacts(facts, HangarTruss8c.FirstSpliceLeft, new[] { HangarTruss8c.FirstSpliceRight });
            var (json, spec) = Fase12Fixtures.Load("detalle-C");
            CatalogTemplate template = TemplateBuilder.Build(json, spec, splice, new TemplateMetadata { Name = "Empalme Detalle C" }, CatalogConfig.Default);
            Assert.Equal("end_plate", template.ConnectionType);
            Assert.Single(template.MemberPattern);
            Assert.Equal("HSS12X8X1/2", template.ChordPattern.Profile);
            Assert.False(template.ChordPattern.Continuous);
            // Ida y vuelta por JSON y casado sobre otro empalme del mismo cordón (el segundo: 1245530 / 1245534).
            CatalogTemplate copy = CatalogTemplate.FromJson(template.ToJson())!;
            TemplateNode second = TemplateNode.FromModelFacts(facts, HangarTruss8c.TopChordIds[1], new[] { HangarTruss8c.TopChordIds[2] });
            Assert.Single(TemplateChooser.ForNode(new[] { copy, BatchPlanTests.OfficialDetalleDTemplate() }, second));
            Assert.Empty(TemplateChooser.ForNode(new[] { copy }, Fase12Fixtures.KNode()));
            TemplateProposal proposal = TemplateChooser.Propose(second, new[] { copy });
            Assert.True(proposal.HasChoice);
            InstantiationResult instance = TemplateInstantiator.Instantiate(copy, proposal.Chosen!.Match, second, CatalogConfig.Default, "lote-1");
            Assert.Equal(HangarTruss8c.TopChordIds[1], instance.Spec.Chord!.ElementId);
            Assert.Equal(HangarTruss8c.TopChordIds[2], instance.Spec.Members[0].ElementId);
            Assert.Equal("splice", instance.Spec.Kind);
            ValidationResult result = Fase12Fixtures.Validate(instance.SpecJson, instance.Spec, facts);
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            // La miniatura sin modelo y el plan con la plantilla en vez del arranque.
            Sketch thumb = TemplateSketch.Build(copy);
            Assert.Equal(12, thumb.Circles.Count(c => c.Kind == SketchKind.Bolt));
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(templates: new[] { BatchPlanTests.OfficialDetalleDTemplate(), copy }));
            var splices = plan.Nodes.Where(n => n.IsSplice && n.WorkPointMm[2] > 19000).ToList();
            Assert.Equal(7, splices.Count);
            Assert.All(splices, n => Assert.Equal(NodeStatus.Ready, n.Status));
            Assert.All(splices, n => Assert.Equal(copy.TemplateId, n.TemplateId));
            Assert.All(splices, n => Assert.False(n.IsStarter));
            // Un empalme de otro perfil (el del cordón central, HSS4X4/HSS3X3) no casa con ella: arranque del Detalle I.
            PlanNode central = plan.Nodes.Single(n => n.IsSplice && n.WorkPointMm[2] < 19000);
            Assert.True(central.IsStarter);
            Assert.Equal("arranque end_plate (Detalle I)", central.TemplateName);
        }

        [Fact]
        public void PanelChoices_ReadTheFase12KeysFromTheRepoFile()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            Assert.Equal(330.0, choices.DiagonalSpliceOffsetMm);
            Assert.Equal(new[] { "Ø18", "Ø13/16\"", "Ø15/16\"", "Ø21" }, choices.HoleLabels.ToArray());
            Assert.Equal(4, choices.HoleOptions().Count);
            Assert.Equal(5.0, choices.Starter.DirectWeld.WeldSizeMm);
            Assert.True(choices.Starter.DirectWeld.SpliceHint);
            Assert.Equal(3, choices.Starter.EndPlate.ByProfile.Count);
            Assert.Equal("C", choices.Starter.EndPlate.Find("HSS12X8X1/2")!.Detail);
            Assert.Equal("I", choices.Starter.EndPlate.Find("HSS3X3X1-4 76x76")!.Detail);
            Assert.Equal("B", choices.Starter.EndPlate.Find("HSS2-1-2X2-1-2X3-16 64x64")!.Detail);
            Assert.Null(choices.Starter.EndPlate.Find("HSS4X4X3/16"));
            Assert.Equal(37.0, choices.Starter.EndPlate.Default.BoltToTubeMm);
            foreach (EndPlateDetailDefaults detail in choices.Starter.EndPlate.ByProfile.Values)
            {
                Assert.True(detail.ThicknessMm > 0);
                Assert.True(detail.DiameterMm > 0);
                Assert.True(detail.HoleMm > detail.DiameterMm);
            }
            // Sin archivo, los embebidos dicen lo mismo.
            PanelChoices embedded = PanelChoices.Default;
            Assert.Equal(330.0, embedded.DiagonalSpliceOffsetMm);
            Assert.Equal(3, embedded.Starter.EndPlate.ByProfile.Count);
            Assert.Equal(4, embedded.HoleLabels.Count);
            // Los rótulos de agujero con Ø se leen.
            Assert.True(LabelParser.TryParseLabelToMm("Ø13/16\"", out double hole));
            Assert.Equal(20.638, hole, 1e-3);
            Assert.True(LabelParser.TryParseLabelToMm("Ø18", out double metric));
            Assert.Equal(18.0, metric);
        }

        [Fact]
        public void Types_AreThreeAndEachOneValidatesItsExample()
        {
            ConnectionTypeRegistry.Clear();
            ConnectionTypeRegistry.Register(GussetNodeType.Instance);
            ConnectionTypeRegistry.Register(DirectWeldType.Instance);
            ConnectionTypeRegistry.Register(EndPlateType.Instance);
            Assert.Equal(new[] { "direct_weld", "end_plate", "gusset_node" }, ConnectionTypeRegistry.All.Select(t => t.Name).ToArray());
            foreach (IConnectionType type in ConnectionTypeRegistry.All)
            {
                string example = type.GetExampleJson();
                Assert.Empty(JsonSchemaValidator.Validate(example));
                ConnectionSpec spec = ConnectionSpec.FromJson(example)!;
                Assert.Equal(type.Name, spec.ConnectionType);
                ValidationResult result = SpecValidator.Validate(example, spec, null, null);
                Assert.DoesNotContain(result.Errors, e => e.Code == ErrorCodes.SchemaInvalid);
                Assert.Contains("\"connection_type\"", type.GetSchemaJson());
                Assert.True(type is ISketchProvider);
            }
            ConnectionTypeRegistry.Clear();
            ConnectionTypeRegistry.Register(GussetNodeType.Instance);
        }
    }
}
