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
    /// <summary>Utilidades de las pruebas de la Fase 13: el Detalle D en modo auto, el nudo girado y las plantillas auto.</summary>
    internal static class Fase13Fixtures
    {
        /// <summary>El Detalle D confirmado con la cartela automática (outline.mode auto, sin puntos).</summary>
        public static (string Json, ConnectionSpec Spec) DetalleDAuto()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetOutlineMode(json, "auto", null, out string autoJson, out string error), error);
            return (autoJson, ConnectionSpec.FromJson(autoJson)!);
        }

        /// <summary>Los hechos del Hangar con las tres barras giradas <paramref name="degrees"/> alrededor del punto de trabajo, en el plano de la cercha.</summary>
        public static FakeModelFacts RotatedFacts(double degrees)
        {
            var facts = new FakeModelFacts();
            Vec3 origin = new Vec3(-11867.7, -17195.8, 17423.0);
            double rad = degrees * Math.PI / 180.0;
            foreach (long id in new[] { 1249630L, 1249631L, 1249636L })
            {
                MemberModelFacts m = facts.Members[id];
                m.CurveStartMm = Rotate(m.CurveStartMm, origin, rad);
                m.CurveEndMm = Rotate(m.CurveEndMm, origin, rad);
            }
            SyncAngles(facts);
            return facts;
        }

        public static void SyncAngles(FakeModelFacts facts)
        {
            TemplateNode node = TemplateNode.FromModelFacts(facts, 1249510, facts.Members.Keys.Where(id => id != 1249510).ToList());
            foreach (TemplateNodeMember member in node.Members) facts.Members[member.ElementId].AngleInPlaneDeg = Math.Round(member.AngleDeg, 1);
        }

        private static Vec3 Rotate(Vec3 p, Vec3 origin, double rad)
        {
            double dx = p.X - origin.X, dz = p.Z - origin.Z;
            return new Vec3(origin.X + dx * Math.Cos(rad) - dz * Math.Sin(rad), p.Y, origin.Z + dx * Math.Sin(rad) + dz * Math.Cos(rad));
        }

        /// <summary>La plantilla oficial del Detalle D con el contorno en modo auto (sin puntos), como la guardaría la Fase 13.</summary>
        public static CatalogTemplate OfficialTemplateAsAuto(string name = "Nudo tipico Detalle D (auto)")
        {
            CatalogTemplate official = BatchPlanTests.OfficialDetalleDTemplate();
            JsonObject spec = (JsonObject)JsonNode.Parse(official.SpecTemplate!.ToJsonString())!;
            spec["gusset"]!["outline"] = new JsonObject { ["mode"] = "auto" };
            official.SpecTemplate = spec;
            official.Name = name;
            official.TemplateId = "auto-" + official.TemplateId;
            official.Matching.AngleToleranceDeg = CatalogConfig.Default.AutoAngleToleranceDeg;
            return CatalogTemplate.FromJson(official.ToJson())!;
        }
    }

    /// <summary>Fase 13 (sección 3.2 y pruebas de la sección 5): la cartela automática.</summary>
    public class AutoOutlineTests
    {
        private static readonly LimitsConfig Limits = Fase11Fixtures.RepoLimits();

        [Fact]
        public void DetalleD_AutoOutline_ContainsTheSlotsAndTheKnifePlate_AndLooksLikeThePlano()
        {
            var (json, spec) = Fase13Fixtures.DetalleDAuto();
            var facts = new FakeModelFacts();
            Assert.Empty(JsonSchemaValidator.Validate(json));
            AutoOutlineResult auto = AutoOutline.Resolve(spec, facts)!;
            Assert.InRange(auto.VertexCount, 6, 12);
            Polygon2D polygon = auto.ToPolygon();
            Assert.True(polygon.IsSimple(out string reason), reason);
            Assert.True(polygon.SignedArea() > 0, "antihorario");
            Assert.All(auto.PointsMm.SelectMany(p => p), v => Assert.Equal(0.0, Math.Abs(v % 5.0), 6));
            // La caja queda a menos del 15 % de la del plano (565 × 530).
            Assert.InRange(auto.WidthMm, 565 * 0.85, 565 * 1.15);
            Assert.InRange(auto.HeightMm, 530 * 0.85, 530 * 1.15);
            // Las ranuras de las dos diagonales soldadas quedan dentro (del retiro al final de la ranura, con el ancho de la barra).
            foreach (MemberSpec member in spec.Members.Where(m => m.Attachment!.Type == "welded_slot"))
            {
                MemberModelFacts mf = facts.Members[member.ElementId];
                var (ux, uy) = ValidationCommon.DirectionOf(mf.AngleInPlaneDeg);
                double half = mf.WidthMm / 2.0;
                foreach (double along in new[] { member.EndSetbackMm!.Value, member.EndSetbackMm.Value + member.Attachment!.SlotLengthMm!.Value })
                {
                    Assert.True(polygon.ContainsPoint(new Point2D(along * ux - half * uy, along * uy + half * ux)), "barra " + member.ElementId + " a " + along);
                    Assert.True(polygon.ContainsPoint(new Point2D(along * ux + half * uy, along * uy - half * ux)), "barra " + member.ElementId + " a " + along);
                }
            }
            // Y la placa cuchilla de la tercera, con la misma comprobación del validador; sus pernos caben en la placa.
            MemberSpec knife = spec.Members.Single(m => m.Attachment!.Type == "bolted_knife_plate");
            KnifePlateSpec plate = knife.Attachment!.Plate!;
            Assert.True(Geometry2DChecks.CheckKnifePlateInsideGusset(polygon, facts.Members[knife.ElementId].AngleInPlaneDeg, knife.EndSetbackMm!.Value,
                plate.LengthMm!.Value, plate.WidthMm!.Value, plate.InsertionMm!.Value, out string detail, Limits.PlateOutsideGussetToleranceMm), detail);
            BoltPatternSpec bolts = knife.Attachment.Bolts!;
            Assert.True(Geometry2DChecks.CheckBoltsInsidePlate(plate.LengthMm.Value, plate.WidthMm.Value, bolts.DiameterMm!.Value, bolts.Rows!.Value, bolts.Columns!.Value,
                bolts.SpacingMm!.Value, bolts.EdgeMm!.Value, bolts.FirstRowFromPlateEndMm!.Value, out string boltDetail), boltDetail);
            Assert.True(auto.AreaRatio < 2.0, "área " + auto.AreaRatio);

            // El validador de siempre: válido, con el contorno calculado y sin aviso de tamaño.
            ValidationResult result = SpecValidator.Validate(json, spec, facts, Limits);
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.NotNull(result.AutoOutline);
            Assert.Equal(auto.PointsMm.Count, result.AutoOutline!.PointsMm.Count);
            Assert.DoesNotContain(result.Warnings, w => w.Code == ErrorCodes.OutlineAutoLarge);
            Assert.DoesNotContain(result.Errors, e => e.Code == ErrorCodes.PlateOutsideGusset || e.Code == ErrorCodes.OutlineInvalid);
            Assert.Matches("^[0-9a-f]{64}$", result.ValidationToken);
            // El token del auto no es el del polígono (es otra especificación), y es determinista.
            var (polygonJson, polygonSpec) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.NotEqual(SpecValidator.Validate(polygonJson, polygonSpec, facts, Limits).ValidationToken, result.ValidationToken);
            Assert.Equal(result.ValidationToken, SpecValidator.Validate(json, spec, facts, Limits).ValidationToken);
            // Ancho y alto escritos (565 × 530) no cuadran con la caja calculada: aviso, no error (D1).
            Assert.Equal(2, result.Warnings.Count(w => w.Code == ErrorCodes.LabelValueMismatch && (w.Path == "gusset.width_mm" || w.Path == "gusset.height_mm")));
        }

        [Fact]
        public void Hangar8b_TheSixteenNodes_ValidateWithTheAutoTemplate()
        {
            CatalogTemplate auto = Fase13Fixtures.OfficialTemplateAsAuto();
            Assert.Equal("auto", auto.SpecTemplate!["gusset"]!["outline"]!["mode"]!.GetValue<string>());
            Assert.Null(auto.SpecTemplate["gusset"]!["outline"]!["points_mm"]);
            PlanRequest request = BatchPlanTests.Hangar8bRequest();
            request.Templates = new List<CatalogTemplate> { auto };
            BatchPlan plan = PlanBuilder.Build(request);
            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            Assert.Equal(16, ready.Count);
            Assert.False(plan.Summary().ContainsKey(NodeStatus.Invalid), string.Join(" | ", plan.Nodes.Where(n => n.Status == NodeStatus.Invalid).Select(n => n.Name + ": " + string.Join(", ", n.Errors.Select(e => e.Code + " " + e.Message)))));
            Assert.All(ready, n => Assert.Equal("auto", n.Spec!["gusset"]!["outline"]!["mode"]!.GetValue<string>()));
            Assert.All(ready, n => Assert.Null(n.Spec!["gusset"]!["outline"]!["points_mm"]));
            Assert.All(ready, n => Assert.DoesNotContain(n.Warnings, w => w.Code == ErrorCodes.OutlineAutoLarge));
            Assert.Equal(8, ready.Count(n => n.IsMirrored));
        }

        [Fact]
        public void RotatedNode_ValidatesWithAuto_WhereTheFixedPolygonFails()
        {
            var (polygonJson, polygonSpec) = SketchBuilderTests.LoadConfirmedFixture();
            var (autoJson, autoSpec) = Fase13Fixtures.DetalleDAuto();
            foreach (double degrees in new[] { 15.0, -15.0 })
            {
                FakeModelFacts rotated = Fase13Fixtures.RotatedFacts(degrees);
                ValidationResult fixedPolygon = SpecValidator.Validate(polygonJson, polygonSpec, rotated, Limits);
                ValidationResult automatic = SpecValidator.Validate(autoJson, autoSpec, rotated, Limits);
                Assert.True(automatic.IsValid, degrees + "°: " + string.Join("; ", automatic.Errors.Select(e => e.Code + " " + e.Message)));
                Assert.DoesNotContain(automatic.Warnings, w => w.Code == ErrorCodes.OutlineAutoLarge);
                if (degrees > 0)
                {
                    // Control: el polígono fijo del plano no cubre la placa cuchilla a +15°.
                    Assert.Contains(fixedPolygon.Errors, e => e.Code == ErrorCodes.PlateOutsideGusset);
                }
            }
        }

        [Fact]
        public void KNode55_AutoGusset_EnvelopesBothRealEnds()
        {
            TemplateNode node = Fase12Fixtures.KNode();
            StarterResult starter = SpecStarter.FromNode(node, Fase11Fixtures.RepoChoices(), null);
            Assert.True(starter.IsAutoOutline);
            var facts = KNode55.Facts();
            AutoOutlineResult auto = AutoOutline.Resolve(starter.Spec, facts)!;
            Polygon2D polygon = auto.ToPolygon();
            foreach (TemplateNodeMember member in node.Members)
            {
                Assert.True(Math.Abs(member.AxisOffsetMm) > 20, "desvío");
                double half = member.WidthMm / 2.0;
                double far = 60.0 + 150.0;
                Assert.True(polygon.ContainsPoint(new Point2D(member.AxisOffsetMm + far * member.Ux - half * member.Uy, far * member.Uy + half * member.Ux)), "barra " + member.ElementId);
                Assert.True(polygon.ContainsPoint(new Point2D(member.AxisOffsetMm + far * member.Ux + half * member.Uy, far * member.Uy - half * member.Ux)), "barra " + member.ElementId);
            }
            Assert.True(auto.MaxY >= 101.6 + 40 - 1e-6 && auto.MinY <= -(101.6 + 40) + 1e-6, "franja del cordón ±141,6 con through_slot");
        }

        [Fact]
        public void Mirror_GivesTheReflectedOutline()
        {
            var (_, spec) = Fase13Fixtures.DetalleDAuto();
            AutoOutlineResult same = AutoOutline.Resolve(spec, new FakeModelFacts())!;
            AutoOutlineResult mirrorX = AutoOutline.Resolve(spec, CatalogTests.MirroredFacts(true, false))!;
            AutoOutlineResult mirrorY = AutoOutline.Resolve(spec, CatalogTests.MirroredFacts(false, true))!;
            Assert.Equal(same.VertexCount, mirrorX.VertexCount);
            Assert.Equal(Set(same.PointsMm.Select(p => (-p[0], p[1]))), Set(mirrorX.PointsMm.Select(p => (p[0], p[1]))));
            Assert.Equal(Set(same.PointsMm.Select(p => (p[0], -p[1]))), Set(mirrorY.PointsMm.Select(p => (p[0], p[1]))));
        }

        private static string Set(IEnumerable<(double X, double Y)> points) =>
            string.Join(";", points.Select(p => Math.Round(p.X, 1).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "," + Math.Round(p.Y, 1).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)).OrderBy(s => s, StringComparer.Ordinal));

        [Fact]
        public void Margins_ZeroDoesNotBreak_AndAnAbsurdOneWarnsLarge()
        {
            var (json, _) = Fase13Fixtures.DetalleDAuto();
            var facts = new FakeModelFacts();
            Assert.True(SpecEditor.TrySetValue(json, "gusset.outline.margin_mm", "0", out string zero, out string error), error);
            Assert.True(SpecEditor.TrySetValue(zero, "gusset.outline.side_margin_mm", "0", out zero, out error), error);
            ValidationResult tight = SpecValidator.Validate(zero, ConnectionSpec.FromJson(zero), facts, Limits);
            Assert.Empty(JsonSchemaValidator.Validate(zero));
            Assert.DoesNotContain(tight.Errors, e => e.Code == ErrorCodes.OutlineInvalid);
            Assert.NotNull(tight.AutoOutline);
            Assert.True(tight.AutoOutline!.WidthMm < 600);

            Assert.True(SpecEditor.TrySetValue(json, "gusset.outline.beyond_chord_mm", "1500", out string huge, out error), error);
            ValidationResult large = SpecValidator.Validate(huge, ConnectionSpec.FromJson(huge), facts, Limits);
            Assert.True(large.IsValid);
            Assert.Contains(large.Warnings, w => w.Code == ErrorCodes.OutlineAutoLarge);
            Assert.True(large.AutoOutline!.HeightMm > Limits.MaxAutoGussetMm);
            Assert.Equal(1200.0, Limits.MaxAutoGussetMm);
            Assert.Equal(12, Limits.AutoOutlineMaxVertices);
            Assert.Equal(LimitsConfig.Default.ComputeHash(), Limits.ComputeHash());
        }

        [Fact]
        public void AutoWithPoints_IsSchemaInvalid_AndNegativeMarginsToo()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            string broken = json.Replace("\"mode\": \"polygon\"", "\"mode\": \"auto\"");
            Assert.Contains(JsonSchemaValidator.Validate(broken), e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "gusset.outline.points_mm");
            Assert.Contains(SpecValidator.Validate(broken, ConnectionSpec.FromJson(broken), new FakeModelFacts(), Limits).Errors, e => e.Code == ErrorCodes.SchemaInvalid);
            var (autoJson, _) = Fase13Fixtures.DetalleDAuto();
            string negative = autoJson.Replace("\"mode\": \"auto\"", "\"mode\": \"auto\", \"margin_mm\": -5");
            Assert.Contains(JsonSchemaValidator.Validate(negative), e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "gusset.outline.margin_mm");
            // El contrato solo añade: el ejemplo y el esquema del tipo siguen válidos.
            Assert.Contains("beyond_chord_mm", GussetNodeType.Instance.GetSchemaJson());
        }

        [Fact]
        public void Sketch_DrawsTheAutoOutline_AndTheTemplateThumbnailToo()
        {
            var (_, spec) = Fase13Fixtures.DetalleDAuto();
            var facts = new FakeModelFacts();
            SketchNodeInfo info = SketchNodeInfo.FromModelFacts(spec, facts);
            Sketch sketch = SketchBuilder.Build(spec, info, null, Limits);
            AutoOutlineResult auto = AutoOutline.Resolve(spec, facts)!;
            SketchPolygon gusset = sketch.Polygons.Single(p => p.Kind == SketchKind.Gusset);
            Assert.Equal(auto.VertexCount, gusset.Points.Count);
            Assert.Contains(sketch.Labels, l => l.Text.Contains("cartela automática"));
            Assert.DoesNotContain(sketch.Notes, n => n.Contains("no tiene contorno"));
            Sketch thumb = TemplateSketch.Build(Fase13Fixtures.OfficialTemplateAsAuto());
            Assert.Single(thumb.Polygons, p => p.Kind == SketchKind.Gusset && p.Points.Count >= 6);
            Assert.Contains(thumb.Labels, l => l.Text.Contains("cartela automática"));
        }

        [Fact]
        public void Polygon2D_ConvexHullAndSimplify_BehaveAsExpected()
        {
            var points = new[] { new Point2D(0, 0), new Point2D(10, 0), new Point2D(10, 10), new Point2D(0, 10), new Point2D(5, 5), new Point2D(5, 0), new Point2D(10, 0) };
            List<Point2D> hull = Polygon2D.ConvexHull(points);
            Assert.Equal(4, hull.Count);
            Assert.True(new Polygon2D(hull).SignedArea() > 0);
            var octagon = Octagon.Points(100, 100, 20).Select(p => new Point2D(p.X, p.Y)).ToList();
            List<Point2D> fewer = Polygon2D.Simplify(octagon, 3.0, 4);
            Assert.Equal(4, fewer.Count);
            Assert.Equal(50.0, fewer.Max(p => p.X), 1e-6);
            Assert.Equal(15.0, AutoOutline.RoundOutward(12.3, 5));
            Assert.Equal(-15.0, AutoOutline.RoundOutward(-12.3, 5));
            Assert.Equal(10.0, AutoOutline.RoundOutward(10.0, 5));
        }
    }

    /// <summary>Regresión de la Fase 13: el camino polygon da los mismos tokens que la 0.12.4 (tokens volcados de ese código).</summary>
    public class Fase13RegressionTests
    {
        private static readonly Dictionary<string, string> Tokens0124 = new Dictionary<string, string>
        {
            ["detalle-D"] = "e1cc88e5b90544e9abf4f2824bd65644e3030fede3dfc47aa89ce9bae3f3a0ee",
            ["detalle-C"] = "cdc3725ac0f19fd18d10ffe1b4679b17a8d800e0a9a7198758fd5c95cee21028",
            ["nudo-K"] = "0f8de9dc616c3dc34e44061a80d5eb2cd416c2f6dbd046e4c15bca57a295d3a7",
            ["detalle-B"] = "9df6f95d4beac96d27c7db9ed2d057484c8c9859406efeabe65f49a66e142908",
            ["detalle-I"] = "560f05bcde78f4d75ff1aec12bb198ac6c53c4941f53d9fe52b0cf2fb4465057",
            ["8b/1250933/1251053,1251054,1251059"] = "8facd1d54fb375db4c8adce7636420bd9aca7c0fed02e6cadc809b2087059cf6",
            ["8b/1250933/1251055,1251056,1251060"] = "3b303b9939ff420fd28842613164c2478b0a3e2a03c4d50e8f3815e5019cdad7",
            ["8b/1250932/1251049,1251050,1251061"] = "7352b66a44812b8888284807b90207686a4f529fdcf19f764a569b5e6bc44094",
            ["8b/1250932/1251051,1251052,1251062"] = "3b015c51dc474a9fd16f0c624ff2a194db8ff6397ee44df0ef38320287598d65",
            ["8b/1250934/1251690,1251691,1251694"] = "4b029bcc411a789a87b86dcdae958ca2f17c7f5d8be48d3fa8cfe18ecb0063eb",
            ["8b/1250934/1251692,1251693,1251695"] = "8363b321860bd6c075e02ee1029fb392b015da076d65e64e7a58046e8fb79392",
            ["8b/1250935/1251696,1251697,1251700"] = "da592867f7ccd9025332cc1b9d426e8dcc34eee664a0af90a334a655c591fe91",
            ["8b/1250935/1251698,1251699,1251701"] = "8f5a6503ad7afc7da5100180392518164b13c2154c79de513ba8747cf6b9adf5",
            ["8b/1250936/1251702,1251703,1251706"] = "32020e1919f4f396c1b2a71abeb8ed1a71ed8af007883ca9f4c5f03f5e0ddf5a",
            ["8b/1250936/1251704,1251705,1251707"] = "55c9ca6cf4ed4919978cd7073654f75465c530a4ef0ba5b42761c365410e70ce",
            ["8b/1250937/1251708,1251709,1251712"] = "5dfb11227ccd48b8a9357ce8f2582d25baa1c24ceb7705cae1b4c9476150cc56",
            ["8b/1250937/1251710,1251711,1251713"] = "62ad001f0c3e4bdec764b4a538e1b4ebbcf2d36d080d0f58de0641efda0939ad",
            ["8b/1250938/1251714,1251715,1251718"] = "612e7f61a54a2c9dc98f8316a8c4f21ec7fc8d1e82f660bc7f09d742ca7dfd2a",
            ["8b/1250938/1251716,1251717,1251719"] = "1e5184d8e01d2cb81390976297e98b6b0432a0ed9c67ed5423f59c6357c2f804",
            ["8b/1250939/1251720,1251721,1251724"] = "a2106336342520ddf387daed20762f815cf0386fd5b71333ccaa4b99d2e95443",
            ["8b/1250939/1251722,1251723,1251725"] = "21c28efac7e56ba1d4d642f066ddad077517b9a90c0b97a7def17e9d14482a38",
        };

        [Fact]
        public void PolygonPath_KeepsTheTokensOf0124()
        {
            LimitsConfig limits = Fase11Fixtures.RepoLimits();
            var (json, spec) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.Equal(Tokens0124["detalle-D"], SpecValidator.Validate(json, spec, new FakeModelFacts(), limits).ValidationToken);
            var (cj, cs) = Fase12Fixtures.Load("detalle-C");
            Assert.Equal(Tokens0124["detalle-C"], Fase12Fixtures.Validate(cj, cs, HangarTruss8c.Facts()).ValidationToken);
            var (kj, ks) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            Assert.Equal(Tokens0124["nudo-K"], Fase12Fixtures.Validate(kj, ks, KNode55.Facts()).ValidationToken);
            foreach (string name in new[] { "detalle-B", "detalle-I" })
            {
                var (j, s) = Fase12Fixtures.Load(name);
                Assert.Equal(Tokens0124[name], ValidationTokenGenerator.GenerateToken(s, null, limits));
            }
            PlanRequest request = BatchPlanTests.Hangar8bRequest();
            request.PlanId = "regresion-fase-13";
            BatchPlan plan = PlanBuilder.Build(request);
            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            Assert.Equal(16, ready.Count);
            foreach (PlanNode node in ready)
            {
                Assert.Equal(Tokens0124["8b/" + node.ChordElementId + "/" + string.Join(",", node.MemberElementIds.OrderBy(i => i))], node.ValidationToken);
            }
            // Y la cercha de la 8c con los pasos de corte: los nudos en K conservan su token (el corte no cambia la especificación).
            // (el token firma source.batch_id: los dos planes llevan el mismo plan_id para poder compararlos)
            PlanRequest before = Fase12Fixtures.Request8c(splitDiagonals: false);
            before.PlanId = "regresion-8c-fase-13";
            PlanRequest after = Fase12Fixtures.Request8c(splitDiagonals: true);
            after.PlanId = "regresion-8c-fase-13";
            BatchPlan fase12 = PlanBuilder.Build(before);
            BatchPlan fase13 = PlanBuilder.Build(after);
            foreach (PlanNode node in fase12.Nodes.Where(n => n.Status == NodeStatus.Ready))
            {
                Assert.Equal(node.ValidationToken, fase13.Find(node.Name)!.ValidationToken);
            }
        }
    }

    /// <summary>Fase 13: el editor del contorno (Contorno: Automático / Polígono y Convertir en polígono).</summary>
    public class SpecEditorOutlineTests
    {
        [Fact]
        public void SwitchingToAuto_RemovesThePoints_AndConvertingBack_WritesTheCalculatedOnes()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            var facts = new FakeModelFacts();
            Assert.True(SpecEditor.TrySetValue(json, "gusset.outline.mode", "auto", out string autoJson, out string error), error);
            ConnectionSpec auto = ConnectionSpec.FromJson(autoJson)!;
            Assert.Equal("auto", auto.Gusset!.Outline!.Mode);
            Assert.Null(auto.Gusset.Outline.PointsMm);
            Assert.True(SpecEditor.IsAutoOutlineJson(autoJson));
            List<SpecField> fields = SpecEditor.ListFields(auto);
            Assert.Equal("auto", fields.Single(f => f.Path == "gusset.outline.mode").Value);
            Assert.False(fields.Single(f => f.Path == "gusset.outline.points_mm").IsEditable);
            Assert.Equal(string.Empty, SpecEditor.FormatOutline(auto.Gusset.Outline));
            // En auto las cotas no se editan ni los puntos.
            Assert.False(SpecEditor.TrySetGussetSize(autoJson, true, 600, out _, out string sizeError));
            Assert.Contains("Convertir en polígono", sizeError);
            Assert.False(SpecEditor.TrySetValue(autoJson, "gusset.outline.points_mm", "0; 0", out _, out string pointsError));
            Assert.Contains("automática", pointsError);
            // Volver a polygon sin puntos no vale; Convertir en polígono escribe los calculados y el ancho y alto de su caja.
            Assert.False(SpecEditor.TrySetValue(autoJson, "gusset.outline.mode", "polygon", out _, out string noPoints));
            Assert.Contains("Convertir en polígono", noPoints);
            ValidationResult validation = SpecValidator.Validate(autoJson, auto, facts, Fase11Fixtures.RepoLimits());
            Assert.True(SpecEditor.ConvertAutoToPolygon(autoJson, validation.AutoOutline!.PointsMm, out string polygonJson, out error), error);
            ConnectionSpec polygon = ConnectionSpec.FromJson(polygonJson)!;
            Assert.Equal("polygon", polygon.Gusset!.Outline!.Mode);
            Assert.Equal(validation.AutoOutline.VertexCount, polygon.Gusset.Outline.PointsMm!.Count);
            Assert.Equal(validation.AutoOutline.WidthMm, polygon.Gusset.WidthMm!.Value, 1e-6);
            Assert.Equal(validation.AutoOutline.HeightMm, polygon.Gusset.HeightMm!.Value, 1e-6);
            Assert.False(SpecEditor.IsAutoOutlineJson(polygonJson));
            // Y desde ahí se edita como siempre: la cota estira el contorno; el token cambia respecto al auto.
            Assert.True(SpecEditor.TrySetGussetSize(polygonJson, true, 700, out string wider, out error), error);
            Assert.Equal(700.0, ConnectionSpec.FromJson(wider)!.Gusset!.WidthMm!.Value, 1e-6);
            ValidationResult converted = SpecValidator.Validate(polygonJson, polygon, facts, Fase11Fixtures.RepoLimits());
            Assert.True(converted.IsValid, string.Join("; ", converted.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Null(converted.AutoOutline);
            Assert.NotEqual(validation.ValidationToken, converted.ValidationToken);
            Assert.False(SpecEditor.TrySetOutlineMode(autoJson, "redondo", null, out _, out string bad));
            Assert.Contains("'auto'", bad);
        }
    }

    /// <summary>Fase 13 (D3): plantillas guardadas con la cartela automática.</summary>
    public class TemplateAutoTests
    {
        [Fact]
        public void ATemplateSavedFromAuto_KeepsAuto_MatchesAt12Degrees_AndValidatesRecalculated()
        {
            var (json, spec) = Fase13Fixtures.DetalleDAuto();
            var facts = new FakeModelFacts();
            TemplateNode node = Fase11Fixtures.HangarNode(facts);
            CatalogTemplate template = TemplateBuilder.Build(json, spec, node, new TemplateMetadata { Name = "Detalle D automático" }, CatalogConfig.Default);
            Assert.Equal("auto", template.SpecTemplate!["gusset"]!["outline"]!["mode"]!.GetValue<string>());
            Assert.Null(template.SpecTemplate["gusset"]!["outline"]!["points_mm"]);
            Assert.Equal(25.0, template.Matching.AngleToleranceDeg);
            // La del Detalle D en polygon sigue con los 10° del catálogo.
            var (pj, ps) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.Equal(10.0, TemplateBuilder.Build(pj, ps, node, new TemplateMetadata { Name = "Detalle D" }, CatalogConfig.Default).Matching.AngleToleranceDeg);

            CatalogTemplate copy = CatalogTemplate.FromJson(template.ToJson())!;
            FakeModelFacts rotated = Fase13Fixtures.RotatedFacts(12.0);
            TemplateNode rotatedNode = Fase11Fixtures.HangarNode(rotated);
            TemplateMatch match = TemplateMatcher.Match(copy, rotatedNode);
            Assert.True(match.IsComplete, match.Describe());
            Assert.InRange(match.MaxDeviationDeg, 11.5, 12.5);
            Assert.False(TemplateMatcher.Match(copy, rotatedNode, angleToleranceDeg: 10.0).IsComplete);
            InstantiationResult instance = TemplateInstantiator.Instantiate(copy, match, rotatedNode, CatalogConfig.Default);
            Assert.Equal("auto", instance.Spec.Gusset!.Outline!.Mode);
            Assert.Null(instance.Spec.Gusset.Outline.PointsMm);
            Assert.Contains(instance.Warnings, w => w.Code == ErrorCodes.TemplateAngleDeviation);
            ValidationResult result = SpecValidator.Validate(instance.SpecJson, instance.Spec, rotated, Fase11Fixtures.RepoLimits());
            Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.NotNull(result.AutoOutline);
            // Conectar la propone (TemplateChooser) y la miniatura se dibuja.
            TemplateProposal proposal = TemplateChooser.Propose(rotatedNode, new[] { copy, BatchPlanTests.OfficialDetalleDTemplate() }, CatalogConfig.Default);
            Assert.Equal(copy.TemplateId, proposal.Chosen!.TemplateId);
            Assert.Single(proposal.Candidates);
            Assert.Contains(TemplateSketch.Build(copy).Polygons, p => p.Kind == SketchKind.Gusset);
            // config/catalog.json del repositorio trae la tolerancia de las plantillas auto.
            CatalogConfig repo = CatalogConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "catalog.json")));
            Assert.Equal(25.0, repo.AutoAngleToleranceDeg);
        }
    }

    /// <summary>Fase 13 (D7): dónde se parte la diagonal.</summary>
    public class SplitPointTests
    {
        [Fact]
        public void KNode55_SplitsAt485FromTheChordAxis_WithTheFirstPlateAt330FromTheFace()
        {
            TemplateNode node = Fase12Fixtures.KNode();
            var facts = KNode55.Facts();
            MemberModelFacts chord = facts.GetMemberFacts(KNode55.Chord)!;
            Assert.Equal(203.2, chord.WidthMm, 0.1);
            var results = new List<SplitPointResult>();
            foreach (long id in new[] { KNode55.LeftDiagonal, KNode55.RightDiagonal })
            {
                SplitPointResult result = SplitPoint.FromAxes(id, node.Frame, chord, facts.GetMemberFacts(id)!, 330.0, 9.525, 0.0);
                results.Add(result);
                Assert.InRange(result.ThetaDeg, 44.0, 45.1);
                Assert.InRange(result.FaceDistanceMm, 143.0, 145.5);
                Assert.Equal(result.FaceDistanceMm + 330.0, result.FirstPlateDistanceMm, 1e-6);
                Assert.Equal(result.FirstPlateDistanceMm + 9.525, result.SplitDistanceMm, 1e-6);
                Assert.InRange(result.SplitDistanceMm, 480.0, 490.0);
                // El punto de corte está sobre el eje de la barra, a la distancia dicha de su corte con el eje del cordón.
                MemberModelFacts bar = facts.GetMemberFacts(id)!;
                Assert.True(NodeReach.AxisDistanceMm(bar.CurveStartMm, bar.CurveEndMm, result.SplitPointMm) < 0.5);
                Assert.Equal(result.SplitDistanceMm, result.CutPointMm.DistanceTo(result.SplitPointMm), 1e-6);
                var (shortStart, shortEnd, longStart, longEnd) = SplitPoint.Segments(bar, result);
                Assert.InRange(shortStart.DistanceTo(shortEnd), 335.0, 345.0); // del extremo en la cara (145 del eje) al corte (485)
                Assert.True(longStart.DistanceTo(longEnd) > 2500);
                Assert.True(shortEnd.DistanceTo(longStart) < 1e-6 || shortStart.DistanceTo(longEnd) < 1e-6);
            }
            // Cada diagonal tiene su propio corte (los desvíos de ∓27,45 separan los puntos).
            Assert.True(results[0].CutPointMm.DistanceTo(results[1].CutPointMm) > 50);
            Assert.True(results[0].SplitPointMm.DistanceTo(results[1].SplitPointMm) > 600);
        }

        [Fact]
        public void Vertical_HasTheFaceAtHalfTheChord_AndParallel_Throws()
        {
            NodeFrame frame = NodeFrame.FromAxes(Vec3.Zero, Vec3.UnitX, Vec3.UnitZ, Vec3.UnitY * -1);
            SplitPointResult vertical = SplitPoint.Compute(1, frame, 0.0, 1.0, 0.0, 203.2, 330.0, 9.525, 0.0);
            Assert.Equal(101.6, vertical.FaceDistanceMm, 1e-6);
            Assert.Equal(90.0, vertical.ThetaDeg, 1e-6);
            Assert.Equal(441.125, vertical.SplitDistanceMm, 1e-6);
            Assert.Equal(441.125, vertical.SplitPointMm.Z, 1e-6);
            SplitPointResult withGap = SplitPoint.Compute(1, frame, 0.0, 1.0, 0.0, 203.2, 330.0, 9.525, 4.0);
            Assert.Equal(443.125, withGap.SplitDistanceMm, 1e-6);
            Assert.Throws<NodeGeometryException>(() => SplitPoint.Compute(1, frame, 1.0, 0.0, 0.0, 203.2, 330.0, 9.525, 0.0));
        }

        [Fact]
        public void FromSpec_UsesTheSpliceHint_AndABarWithoutHintHasNoPoint()
        {
            var (_, spec) = Fase12Fixtures.Load("nudo-K-cordon-superior");
            var facts = KNode55.Facts();
            MemberSpec left = spec.Members.Single(m => m.ElementId == KNode55.LeftDiagonal);
            Assert.NotNull(left.SpliceHint);
            SplitPointResult result = SplitPoint.FromSpec(spec, left, facts)!;
            Assert.InRange(result.SplitDistanceMm, 480.0, 490.0);
            MemberSpec copy = new MemberSpec { ElementId = KNode55.RightDiagonal };
            Assert.Null(SplitPoint.FromSpec(spec, copy, facts));
            // Una barra más corta que el corte no se puede partir.
            MemberModelFacts bar = facts.GetMemberFacts(KNode55.LeftDiagonal)!;
            var shortBar = new MemberModelFacts { ElementId = bar.ElementId, CurveStartMm = bar.CurveEndMm + (bar.CurveStartMm - bar.CurveEndMm).Normalized() * 300.0, CurveEndMm = bar.CurveEndMm, TypeName = bar.TypeName, WidthMm = bar.WidthMm, HeightMm = bar.HeightMm };
            Assert.Throws<NodeGeometryException>(() => SplitPoint.Segments(shortBar, result));
        }
    }

    /// <summary>Fase 13 (D5): los pasos de corte y los empalmes B en el plan y en el lote.</summary>
    public class Fase13PlanTests
    {
        [Fact]
        public void Plan_HangarTruss8c_AddsTheSplitStepsAndThePredictedSplices()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            Assert.Equal(41, plan.ReadyCount); // los mismos 41 de la Fase 12
            var kNodes = plan.Nodes.Where(n => n.Status == NodeStatus.Ready && n.ConnectionType == "direct_weld").ToList();
            Assert.Equal(17, kNodes.Count);
            Assert.All(kNodes, n => Assert.True(n.SplitSteps.Count >= 1 && n.SplitSteps.Count <= 2, n.Name));
            Assert.All(kNodes.SelectMany(n => n.SplitSteps), s => Assert.True(s.CanSplit, s.Describe()));
            Assert.All(kNodes.SelectMany(n => n.SplitSteps), s => Assert.InRange(s.SplitDistanceMm, 478.0, 492.0));
            Assert.All(kNodes.SelectMany(n => n.SplitSteps), s => Assert.Equal(330.0, s.OffsetFromChordFaceMm));
            Assert.All(kNodes.SelectMany(n => n.SplitSteps), s => Assert.Equal("B", s.Detail));
            Assert.Equal(31, kNodes.Sum(n => n.SplitSteps.Count));
            Assert.Equal(31, plan.SplitStepCount);
            var predicted = plan.Nodes.Where(n => n.Status == NodeStatus.AfterSplit).ToList();
            Assert.Equal(31, predicted.Count);
            Assert.Equal(31, plan.AfterSplitCount);
            Assert.All(predicted, n => Assert.True(n.IsSplice && n.IsStarter && n.ConnectionType == "end_plate"));
            Assert.All(predicted, n => Assert.Equal("arranque end_plate (Detalle B)", n.TemplateName));
            Assert.All(predicted, n => Assert.Empty(n.Errors));
            Assert.All(predicted, n => Assert.Equal("splice", n.Spec!["kind"]!.GetValue<string>()));
            Assert.All(predicted, n => Assert.Equal(185.0, n.Spec!["plate"]!["width_mm"]!.GetValue<double>()));
            Assert.All(predicted, n => Assert.Null(n.ValidationToken));
            Assert.All(predicted, n => Assert.False(BatchRunner.CanBeCreated(n)));
            Assert.All(predicted, n => Assert.False(n.CanBeMarked));
            // N5 (el nudo en K con excentricidad) tiene dos cortes y sus dos empalmes justo detrás; N9 los tiene detrás de N9E.
            PlanNode n5 = plan.Find("N5")!;
            Assert.Equal(2, n5.SplitSteps.Count);
            Assert.Equal(new[] { "N5B", "N5B2" }, n5.SplitSteps.Select(s => s.SpliceNode).ToArray());
            Assert.Equal(plan.Nodes.IndexOf(n5) + 1, plan.Nodes.IndexOf(plan.Find("N5B")!));
            Assert.Equal(plan.Nodes.IndexOf(n5) + 2, plan.Nodes.IndexOf(plan.Find("N5B2")!));
            PlanNode n9 = plan.Find("N9")!;
            Assert.Equal(plan.Nodes.IndexOf(n9) + 1, plan.Nodes.IndexOf(plan.Find("N9E")!));
            Assert.Equal(plan.Nodes.IndexOf(n9) + 2, plan.Nodes.IndexOf(plan.Find("N9B")!));
            Assert.Equal("N5", plan.Find("N5B")!.SplitOfNode);
            Assert.Contains("Partir " + n5.SplitSteps[0].ElementId + " a 330 (Detalle B)", n5.SplitSteps[0].Describe());
            Assert.Equal("● Empalme B (tras partir)", PlanAdvice.StatusText(plan.Find("N5B")!, plan));
            Assert.Contains("se parten 31 diagonales a 330 (Detalle B) y se crean 31 empalmes B", PlanAdvice.SummaryText(plan));
            Assert.Contains("se crea con el corte de N5", BatchRunner.WhyNotCreatable(plan.Find("N5B")!, plan));
            // Las barras de los empalmes previstos no cuentan como sueltas y los nombres ordenan N5, N5B, N5B2 antes de N6.
            Assert.DoesNotContain(plan.UnusedElementIds, id => id < 0);
            Assert.True(NodeDetector.NameNumber("N5") < NodeDetector.NameNumber("N5E") && NodeDetector.NameNumber("N5E") < NodeDetector.NameNumber("N5B") && NodeDetector.NameNumber("N5B") < NodeDetector.NameNumber("N5B2") && NodeDetector.NameNumber("N5B2") < NodeDetector.NameNumber("N6"));
        }

        [Fact]
        public void StepsFor_OrdersCutsThenSplicesBThenN9EThenN9()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            PlanNode n9 = plan.Find("N9")!;
            PlanNode n9e = plan.Find("N9E")!;
            PlanNode n12 = plan.Find("N12")!;
            var request = new BatchCreateRequest(plan.PlanId, new[]
            {
                new BatchCreateItem("N12", n12.ValidationToken!),
                new BatchCreateItem("N9", n9.ValidationToken!),
                new BatchCreateItem("N9E", n9e.ValidationToken!),
            });
            List<BatchStep> steps = BatchRunner.StepsFor(plan, request);
            Assert.Equal(new[] { "split", "split", "splice_b", "splice_b", "node", "split", "split", "splice_b", "splice_b", "node", "node" }, steps.Select(s => s.Kind).ToArray());
            Assert.Equal(new[] { "N12 · partir " + n12.SplitSteps[0].ElementId, "N12 · partir " + n12.SplitSteps[1].ElementId, "N12B", "N12B2", "N12",
                "N9 · partir " + n9.SplitSteps[0].ElementId, "N9 · partir " + n9.SplitSteps[1].ElementId, "N9B", "N9B2", "N9E", "N9" }, steps.Select(s => s.NodeName).ToArray());
        }

        [Fact]
        public void Run_SplitsCreatesTheSplicesAndThenTheKNode_AndUndoesEverythingWithStopOnError()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            PlanNode n9 = plan.Find("N9")!;
            PlanNode n9e = plan.Find("N9E")!;
            PlanNode other = plan.Nodes.First(n => n.Status == NodeStatus.Ready && !n.AtSplice && !n.IsSplice && n.SplitSteps.Count == 0);
            var request = new BatchCreateRequest(plan.PlanId, new[]
            {
                new BatchCreateItem(other.Name, other.ValidationToken!),
                new BatchCreateItem("N9", n9.ValidationToken!),
                new BatchCreateItem("N9E", n9e.ValidationToken!),
            });
            var order = new List<string>();
            long nextId = 9000;
            var handlers = new BatchStepHandlers
            {
                Split = (k, step) => { order.Add("partir " + step.ElementId); return new SplitOutcome(++nextId, "split-" + nextId); },
                CreateSplice = (splice, step, split) => { order.Add(splice.Name); return new BatchNodeOutcome("conn-" + splice.Name, 8); },
            };
            BatchReport report = BatchRunner.Run(plan, request, (node, item) => { order.Add(node.Name); return new BatchNodeOutcome("conn-" + node.Name, 3); }, BatchReport.UndoOne, handlers);
            Assert.Equal(new[] { other.Name, "partir " + n9.SplitSteps[0].ElementId, "partir " + n9.SplitSteps[1].ElementId, "N9B", "N9B2", "N9E", "N9" }, order.ToArray());
            Assert.Equal(2, report.SplitCount);
            Assert.Equal(5, report.CreatedCount);
            Assert.Equal(NodeStatus.Created, plan.Find("N9B")!.Status);
            Assert.Equal(9001, plan.Find("N9B")!.MemberElementIds.Single());
            Assert.Equal(9001, n9.SplitSteps[0].NewElementId);
            Assert.Equal("split-9001", n9.SplitSteps[0].SplitId);
            Assert.True(n9.SplitSteps.All(s => s.IsDone));
            Assert.Equal(NodeStatus.Created, n9.Status);
            Assert.Contains("2 diagonales partidas (Detalle B)", report.SummaryText);
            Assert.Contains(report.Nodes, r => r.Kind == "split" && r.NewElementId == 9001 && r.Outcome == BatchOutcome.Split);
            Assert.Contains(report.Nodes, r => r.Node == "N9B" && r.Kind == "splice_b" && r.Outcome == BatchOutcome.Created);
            Assert.True(plan.HasBatchConnections);
            // Borrar el lote: los empalmes B vuelven a after_split y los cortes a no hechos.
            int restored = BatchRunner.ApplyDeletion(plan, new[] { "conn-N9", "conn-N9E", "conn-N9B", "conn-N9B2", "conn-" + other.Name }, null, n9.SplitSteps.Select(s => s.ElementId));
            Assert.Equal(5, restored);
            Assert.Equal(NodeStatus.AfterSplit, plan.Find("N9B")!.Status);
            Assert.Equal(NodeStatus.Ready, n9.Status);
            Assert.All(n9.SplitSteps, s => Assert.False(s.IsDone));

            // Si el empalme B falla con stop_on_error y grupo exterior, todo se revierte (D9 en el lote): el nudo en K queda fallido.
            var failing = new BatchStepHandlers
            {
                Split = (k, step) => new SplitOutcome(++nextId, "split-" + nextId),
                CreateSplice = (splice, step, split) => throw new InvalidOperationException("Advance Steel no creó la plancha"),
            };
            var stopRequest = new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem("N9", n9.ValidationToken!), new BatchCreateItem(other.Name, other.ValidationToken!) }, stopOnError: true);
            BatchReport stopped = BatchRunner.Run(plan, stopRequest, (node, item) => new BatchNodeOutcome("conn-" + node.Name, 3), BatchReport.UndoOne, failing);
            Assert.True(stopped.Stopped);
            Assert.Equal("N9B", stopped.StoppedAt);
            Assert.Equal(NodeStatus.Failed, n9.Status);
            Assert.Contains(n9.Errors, e => e.Code == ErrorCodes.FabricationFailed);
            Assert.All(n9.SplitSteps, s => Assert.False(s.IsDone));
            Assert.Contains(stopped.Nodes, r => r.Kind == "split" && r.Outcome == BatchOutcome.RolledBack);
            Assert.Equal(NodeStatus.Ready, other.Status);
            Assert.Contains(stopped.Nodes, r => r.Node == other.Name && r.Outcome == BatchOutcome.Skipped);

            // Sin stop_on_error: el corte que falla deja el nudo en K fallido con el motivo y los demás siguen.
            BatchPlan plan2 = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            PlanNode k2 = plan2.Find("N12")!;
            PlanNode other2 = plan2.Find(other.Name)!;
            var failingSplit = new BatchStepHandlers
            {
                Split = (k, step) => throw new CatalogException(ErrorCodes.SplitFailed, "Revit no copió el elemento"),
                CreateSplice = (splice, step, split) => new BatchNodeOutcome("x", 1),
            };
            BatchReport partial = BatchRunner.Run(plan2, new BatchCreateRequest(plan2.PlanId, new[] { new BatchCreateItem("N12", k2.ValidationToken!), new BatchCreateItem(other.Name, other2.ValidationToken!) }),
                (node, item) => new BatchNodeOutcome("conn-" + node.Name, 3), BatchReport.UndoPerNode, failingSplit);
            Assert.False(partial.Stopped);
            Assert.Equal(NodeStatus.Failed, k2.Status);
            Assert.Equal(NodeStatus.Created, other2.Status);
            Assert.Contains(partial.Nodes, r => r.Node == "N12" && r.Outcome == BatchOutcome.Failed && r.Reason!.Contains("corte"));
            Assert.Contains(partial.Nodes, r => r.Node == "N12B" && r.Outcome == BatchOutcome.Skipped);
            Assert.Equal(2, partial.FailedCount); // el corte y el nudo en K que no se creó sin él
        }

        [Fact]
        public void Run_WithoutASplitter_SkipsTheStepsAndCreatesTheKNodeLikeFase12()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            PlanNode n12 = plan.Find("N12")!;
            var request = new BatchCreateRequest(plan.PlanId, new[] { new BatchCreateItem("N12", n12.ValidationToken!) });
            BatchReport report = BatchRunner.Run(plan, request, (node, item) => new BatchNodeOutcome("conn-" + node.Name, 2), BatchReport.UndoPerNode);
            Assert.Equal(NodeStatus.Created, n12.Status);
            Assert.Equal(0, report.SplitCount);
            Assert.Equal(1, report.CreatedCount);
            Assert.Equal(4, report.SkippedCount);
            Assert.All(report.Nodes.Where(r => r.Kind != "node"), r => Assert.Contains("sin partir", r.Reason));
            Assert.Equal(NodeStatus.AfterSplit, plan.Find("N12B")!.Status);
            // Con split_members: false también se saltan, aunque haya quien parta.
            BatchPlan plan2 = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: true));
            var noSplit = new BatchCreateRequest(plan2.PlanId, new[] { new BatchCreateItem("N12", plan2.Find("N12")!.ValidationToken!) }) { SplitMembers = false };
            var handlers = new BatchStepHandlers { Split = (k, s) => throw new InvalidOperationException("no debería"), CreateSplice = (a, b, c) => throw new InvalidOperationException("no debería") };
            BatchReport report2 = BatchRunner.Run(plan2, noSplit, (node, item) => new BatchNodeOutcome("conn", 2), BatchReport.UndoPerNode, handlers);
            Assert.Equal(1, report2.CreatedCount);
            Assert.All(report2.Nodes.Where(r => r.Kind != "node"), r => Assert.Contains("split_members", r.Reason));
            // La petición JSON admite split_members.
            using var doc = System.Text.Json.JsonDocument.Parse("{\"plan_id\":\"p\",\"nodes\":[{\"node\":\"N1\",\"validation_token\":\"t\"}],\"split_members\":false}");
            Assert.False(BatchCreateRequest.FromJson(doc.RootElement).SplitMembers);
        }

        [Fact]
        public void Plan_WithoutSplitDiagonals_IsTheFase12Plan()
        {
            BatchPlan plan = PlanBuilder.Build(Fase12Fixtures.Request8c(splitDiagonals: false));
            Assert.Equal(0, plan.AfterSplitCount);
            Assert.All(plan.Nodes, n => Assert.Empty(n.SplitSteps));
            Assert.DoesNotContain("se parten", PlanAdvice.SummaryText(plan));
        }
    }
}
