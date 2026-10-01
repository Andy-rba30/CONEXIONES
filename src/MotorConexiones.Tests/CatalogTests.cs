using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Catálogo de plantillas (Fase 7), sin Revit: el Detalle D confirmado con los hechos del nudo del Hangar
    /// (<see cref="FakeModelFacts"/>) se convierte en plantilla, se vuelve a instanciar sobre el mismo nudo y sobre
    /// nudos reflejados, y la especificación resultante pasa por el validador de siempre.
    /// </summary>
    public class CatalogTests
    {
        private const long Chord = 1249510;
        private static readonly long[] MemberIds = { 1249630, 1249631, 1249636 };

        private static TemplateNode NodeOf(FakeModelFacts facts) => TemplateNode.FromModelFacts(facts, Chord, MemberIds);

        private static (CatalogTemplate template, string rawJson, ConnectionSpec spec, FakeModelFacts facts) BuildDetalleD(string? policy = null)
        {
            var (rawJson, spec) = SketchBuilderTests.LoadConfirmedFixture();
            var facts = new FakeModelFacts();
            var metadata = new TemplateMetadata
            {
                Name = "Nudo típico Detalle D",
                Description = "Cartela PL 3/8\" 565x530 con placa cuchilla",
                Tags = new List<string> { "hangar", "cercha", " HSS " },
                ProfilePolicy = policy,
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
            };
            CatalogTemplate template = TemplateBuilder.Build(rawJson, spec, NodeOf(facts), metadata, CatalogConfig.Default);
            return (template, rawJson, spec, facts);
        }

        /// <summary>El mismo nudo visto en espejo: las barras se reflejan respecto al punto de trabajo (X, Z o los dos).</summary>
        private static FakeModelFacts MirroredFacts(bool mirrorX, bool mirrorZ)
        {
            var facts = new FakeModelFacts();
            Vec3 origin = new Vec3(-11867.7, -17195.8, 17423.0);
            foreach (long id in MemberIds.Concat(new[] { Chord }))
            {
                MemberModelFacts m = facts.Members[id];
                m.CurveStartMm = Reflect(m.CurveStartMm, origin, mirrorX, mirrorZ);
                m.CurveEndMm = Reflect(m.CurveEndMm, origin, mirrorX, mirrorZ);
            }
            SyncAngles(facts);
            return facts;
        }

        /// <summary>Como hace RevitModelFacts: el ángulo con signo de cada barra sale de sus curvas y del marco canónico.</summary>
        private static void SyncAngles(FakeModelFacts facts)
        {
            TemplateNode node = TemplateNode.FromModelFacts(facts, Chord, facts.Members.Keys.Where(id => id != Chord).ToList());
            foreach (TemplateNodeMember member in node.Members)
            {
                facts.Members[member.ElementId].AngleInPlaneDeg = Math.Round(member.AngleDeg, 1);
            }
        }

        private static Vec3 Reflect(Vec3 p, Vec3 origin, bool mirrorX, bool mirrorZ) => new Vec3(
            mirrorX ? 2 * origin.X - p.X : p.X,
            p.Y,
            mirrorZ ? 2 * origin.Z - p.Z : p.Z);

        private static LimitsConfig RepoLimits() => LimitsConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "limits.json")));

        [Fact]
        public void TemplateNode_FromTheHangarFacts_HasCanonicalSignedAngles()
        {
            TemplateNode node = NodeOf(new FakeModelFacts());

            Assert.Equal(Chord, node.ChordElementId);
            Assert.Equal("HSS3X3X1/4", node.ChordTypeName);
            Assert.False(node.Frame.ChordReversed);
            Assert.True(node.Frame.Y.Z > 0.999, "+Y local hacia arriba");
            Assert.Equal(3, node.Members.Count);
            Assert.Equal(45.0, node.Find(1249630)!.AngleDeg, 1);
            Assert.Equal(90.0, node.Find(1249631)!.AngleDeg, 1);
            Assert.Equal(-135.0, node.Find(1249636)!.AngleDeg, 1);
            Assert.Equal("+Y", node.Find(1249630)!.Side);
            Assert.Equal("-Y", node.Find(1249636)!.Side);
            Assert.All(node.Members, m => Assert.True(m.ConnectsToNode));
            Assert.Equal(new[] { Chord, 1249630L, 1249631L, 1249636L }, node.AllElementIds);
        }

        [Fact]
        public void TemplateNode_ChooseChord_PicksTheMostHorizontalMember()
        {
            Assert.Equal(Chord, TemplateNode.ChooseChord(new FakeModelFacts(), new[] { 1249630L, Chord, 1249636L }));
            var error = Assert.Throws<CatalogException>(() => TemplateNode.FromModelFacts(new FakeModelFacts(), Chord, new[] { 4242L }));
            Assert.Equal(ErrorCodes.ElementNotFound, error.Error.Code);
        }

        [Fact]
        public void Build_RemovesIdsWritesThePatternAndRoundTripsThroughJson()
        {
            var (template, _, spec, _) = BuildDetalleD();

            Assert.Equal("Nudo típico Detalle D", template.Name);
            Assert.Equal(new[] { "hangar", "cercha", "HSS" }, template.Tags);
            Assert.Equal("gusset_node", template.ConnectionType);
            Assert.Equal("Detalle D", template.Origin.Drawing);
            Assert.Equal("HANGAR_PRUEBA_sondeo", template.Origin.Document);
            Assert.Equal(new[] { Chord, 1249630L, 1249631L, 1249636L }, template.Origin.ElementIds);
            Assert.Equal("HSS3X3X1/4", template.ChordPattern.Profile);
            Assert.Equal(ProfilePolicy.Warn, template.ChordPattern.ProfilePolicy);
            Assert.Equal(10.0, template.Matching.AngleToleranceDeg);
            Assert.True(template.Matching.AllowMirror);

            Assert.Equal(3, template.MemberPattern.Count);
            Assert.Equal(new[] { 0, 1, 2 }, template.MemberPattern.Select(s => s.Slot));
            Assert.Equal(new[] { 45.0, 90.0, -135.0 }, template.MemberPattern.Select(s => s.AngleDeg));
            Assert.Equal(new[] { "+Y", "+Y", "-Y" }, template.MemberPattern.Select(s => s.Side));
            Assert.Equal(new[] { "diagonal", "vertical", "diagonal" }, template.MemberPattern.Select(s => s.Role));
            Assert.Equal("HSS2-1-2X2-1-2X3-16 64x64", template.MemberPattern[0].ModelTypeName);
            Assert.Equal(spec.Members[0].Profile, template.MemberPattern[0].Profile);

            string json = template.SpecTemplate!.ToJsonString();
            Assert.DoesNotContain("element_id", json);
            Assert.DoesNotContain("\"node\"", json);
            Assert.Contains("\"slot\"", json);
            Assert.Equal(0, template.SpecTemplate["uncertain_fields"]!.AsArray().Count);
            // Los valores confirmados de las dudas siguen en sus campos.
            Assert.Equal("through_slot", template.SpecTemplate["gusset"]!["chord_interface"]!.GetValue<string>());
            Assert.Equal("HSS2-1/2X2-1/2X3/16", template.SpecTemplate["members"]![1]!["profile"]!.GetValue<string>());

            CatalogTemplate? reread = CatalogTemplate.FromJson(template.ToJson());
            Assert.NotNull(reread);
            Assert.Equal(template.TemplateId, reread!.TemplateId);
            Assert.Equal(template.MemberPattern.Select(s => s.AngleDeg), reread.MemberPattern.Select(s => s.AngleDeg));
            Assert.Equal(template.SpecTemplate.ToJsonString(), reread.SpecTemplate!.ToJsonString());
            Assert.Contains("3 barra(s)", template.DescribePattern());
            Assert.Null(CatalogTemplate.FromJson("{\"name\": \"sin spec\"}"));
            Assert.Null(CatalogTemplate.FromJson("esto no es json"));
        }

        [Fact]
        public void Build_WithOpenUncertainties_IsRejected()
        {
            string json = File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("docs", "fixtures", "detalle-D.json")));
            ConnectionSpec spec = ConnectionSpec.FromJson(json)!;
            var error = Assert.Throws<CatalogException>(() =>
                TemplateBuilder.Build(json, spec, NodeOf(new FakeModelFacts()), new TemplateMetadata { Name = "x" }));
            Assert.Equal(ErrorCodes.TemplateHasOpenUncertainties, error.Error.Code);
            Assert.Contains("members[1].profile", error.Error.Message);

            var noName = Assert.Throws<CatalogException>(() =>
                TemplateBuilder.Build(json, spec, NodeOf(new FakeModelFacts()), new TemplateMetadata { Name = "  " }));
            Assert.Equal(ErrorCodes.InvalidRequest, noName.Error.Code);
        }

        [Fact]
        public void RoundTrip_SameNode_InstantiatesTheSameSpecAndValidatesWithAToken()
        {
            var (template, rawJson, original, facts) = BuildDetalleD();
            TemplateNode node = NodeOf(facts);

            TemplateMatch match = TemplateMatcher.Match(template, node);
            Assert.True(match.IsComplete, match.Describe());
            Assert.Equal(TemplateOrientation.Same, match.Orientation);
            Assert.Equal("same", match.OrientationName);
            Assert.All(match.Assignments, a => Assert.True(a.DeviationDeg < 0.05, a.Slot + ": " + a.DeviationDeg));
            Assert.Empty(match.UnassignedMembers);
            Assert.Equal(new long?[] { 1249630, 1249631, 1249636 }, match.Assignments.Select(a => a.ElementId));

            InstantiationResult result = TemplateInstantiator.Instantiate(template, match, node, CatalogConfig.Default);
            Assert.Empty(result.Warnings);
            ConnectionSpec spec = result.Spec;
            Assert.Equal(original.Node.ElementIds, spec.Node.ElementIds);
            Assert.Equal(Chord, spec.Chord!.ElementId);
            Assert.Equal(new[] { 1249630L, 1249631L, 1249636L }, spec.Members.Select(m => m.ElementId));
            Assert.Equal(template.TemplateId, spec.Source!.TemplateId);
            Assert.Null(spec.Source.BatchId);
            Assert.Equal("Detalle D", spec.Source.Drawing);
            // expected_angle_deg = inclinación real respecto al cordón (sin signo): 45, 90 y 45.
            Assert.Equal(new double?[] { 45.0, 90.0, 45.0 }, spec.Members.Select(m => m.ExpectedAngleDeg));
            Assert.Equal(original.Gusset!.Outline!.PointsMm!.Select(p => p[0] + "," + p[1]), spec.Gusset!.Outline!.PointsMm!.Select(p => p[0] + "," + p[1]));
            Assert.Equal(4, spec.DimensionChains.Count);
            Assert.Equal(60.0, spec.Members[2].Attachment!.Bolts!.SpacingMm);
            Assert.Empty(spec.UncertainFields);
            Assert.DoesNotContain("\"slot\"", result.SpecJson);

            ValidationResult validation = SpecValidator.Validate(result.SpecJson, spec, facts, RepoLimits());
            Assert.True(validation.IsValid, string.Join("; ", validation.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Matches("^[0-9a-f]{64}$", validation.ValidationToken);
            Assert.DoesNotContain(validation.Warnings, w => w.Code == ErrorCodes.AngleDiffersFromModel);

            // El esquema acepta source.template_id y la IA puede pasar el JSON tal cual a conn_create.
            Assert.Empty(JsonSchemaValidator.Validate(result.SpecJson));
        }

        [Fact]
        public void MirroredNodes_AreMatchedInTheRightOrientationAndTheOutlineFlips()
        {
            var (template, _, original, _) = BuildDetalleD();
            double[] originalFirst = original.Gusset!.Outline!.PointsMm![0];

            foreach (var (mirrorX, mirrorZ, expected) in new[]
            {
                (true, false, TemplateOrientation.MirrorX),
                (false, true, TemplateOrientation.MirrorY),
                (true, true, TemplateOrientation.Both),
            })
            {
                FakeModelFacts mirroredFacts = MirroredFacts(mirrorX, mirrorZ);
                TemplateNode node = NodeOf(mirroredFacts);
                TemplateMatch match = TemplateMatcher.Match(template, node);
                Assert.True(match.IsComplete, match.Describe());
                Assert.Equal(expected, match.Orientation);
                Assert.All(match.Assignments, a => Assert.True(a.DeviationDeg < 0.05, match.Describe()));
                // La placa cuchilla (ranura 2) sigue en la diagonal inferior 1249636.
                Assert.Equal(1249636L, match.Assignments.Single(a => a.Slot == 2).ElementId);

                InstantiationResult result = TemplateInstantiator.Instantiate(template, match, node, CatalogConfig.Default);
                List<double[]> points = result.Spec.Gusset!.Outline!.PointsMm!;
                Assert.Equal(original.Gusset.Outline.PointsMm.Count, points.Count);
                double sx = mirrorX ? -1 : 1, sy = mirrorZ ? -1 : 1;
                Assert.Contains(points, p => Math.Abs(p[0] - sx * originalFirst[0]) < 1e-9 && Math.Abs(p[1] - sy * originalFirst[1]) < 1e-9);
                Assert.Contains(points, p => Math.Abs(p[0] - sx * 315.0) < 1e-9 && Math.Abs(p[1] - sy * 210.0) < 1e-9);
                // El contorno sigue siendo un polígono simple y la especificación valida contra el nudo reflejado: la placa
                // cuchilla se comprueba donde está la barra (regla 8.9 con el ángulo real) y cae dentro de la cartela reflejada.
                ValidationResult validation = SpecValidator.Validate(result.SpecJson, result.Spec, mirroredFacts, RepoLimits());
                Assert.True(validation.IsValid, expected + ": " + string.Join("; ", validation.Errors.Select(e => e.Code + " " + e.Message)));
            }
        }

        [Fact]
        public void MirroredNode_WithAllowMirrorFalse_DoesNotMatch()
        {
            var (template, _, _, _) = BuildDetalleD();
            TemplateNode mirrored = NodeOf(MirroredFacts(true, false));
            TemplateMatch match = TemplateMatcher.Match(template, mirrored, allowMirror: false);
            Assert.False(match.IsComplete);
            Assert.Equal(TemplateOrientation.Same, match.Orientation);
            // Forzando la orientación correcta sí casa.
            Assert.True(TemplateMatcher.Match(template, mirrored, forced: TemplateOrientation.MirrorX).IsComplete);
        }

        [Fact]
        public void NodeWithAMissingMember_HasNoMatchAndInstantiationExplainsIt()
        {
            var (template, _, _, _) = BuildDetalleD();
            TemplateNode node = TemplateNode.FromModelFacts(new FakeModelFacts(), Chord, new[] { 1249630L, 1249636L });

            List<TemplateMatch> attempts = TemplateMatcher.MatchAll(template, node);
            Assert.Equal(4, attempts.Count);
            TemplateMatch best = TemplateMatcher.Match(template, node);
            Assert.False(best.IsComplete);
            Assert.Equal(2, best.MatchedCount);
            Assert.Equal(new[] { 1 }, best.UnmatchedSlots);

            var error = Assert.Throws<CatalogException>(() => TemplateInstantiator.Instantiate(template, best, node));
            Assert.Equal(ErrorCodes.TemplateNoMatch, error.Error.Code);
            Assert.Contains("ranura 1", error.Error.Message);
        }

        [Fact]
        public void ExtraMember_IsReportedAsUnassigned_AndASlightlyRotatedOneGetsADeviationWarning()
        {
            var (template, _, _, _) = BuildDetalleD();
            var facts = new FakeModelFacts();
            // Giramos la diagonal superior 7° (dentro de la tolerancia de 10°, por encima del aviso de 5°).
            double a = (45.0 + 7.0) * Math.PI / 180.0;
            facts.Members[1249630].CurveEndMm = new Vec3(-11867.7 + 2000 * Math.Cos(a), -17195.8, 17423.0 + 2000 * Math.Sin(a));
            // Y añadimos una barra más que no está en la plantilla.
            facts.Members[1249999] = new MemberModelFacts
            {
                ElementId = 1249999, UniqueId = "extra", TypeName = "HSS2-1-2X2-1-2X3-16 64x64", WidthMm = 63.5, HeightMm = 63.5, ThicknessMm = 4.76,
                CurveStartMm = new Vec3(-11867.7, -17195.8, 17423.0), CurveEndMm = new Vec3(-10453.5, -17195.8, 16008.8), AngleInPlaneDeg = -45.0,
            };
            SyncAngles(facts);
            TemplateNode node = TemplateNode.FromModelFacts(facts, Chord, new[] { 1249630L, 1249631L, 1249636L, 1249999L });

            TemplateMatch match = TemplateMatcher.Match(template, node);
            Assert.True(match.IsComplete, match.Describe());
            Assert.Equal(new[] { 1249999L }, match.UnassignedMembers);
            Assert.Equal(7.0, match.Assignments[0].DeviationDeg, 1);

            InstantiationResult result = TemplateInstantiator.Instantiate(template, match, node, CatalogConfig.Default);
            ApiError warning = Assert.Single(result.Warnings, w => w.Code == ErrorCodes.TemplateAngleDeviation);
            Assert.Equal("members[0].expected_angle_deg", warning.Path);
            Assert.Equal(52.0, result.Spec.Members[0].ExpectedAngleDeg!.Value, 1);
            Assert.DoesNotContain(1249999L, result.Spec.Node.ElementIds);

            // Con 25° ya no casa (tolerancia 10°).
            double b = (45.0 + 25.0) * Math.PI / 180.0;
            facts.Members[1249630].CurveEndMm = new Vec3(-11867.7 + 2000 * Math.Cos(b), -17195.8, 17423.0 + 2000 * Math.Sin(b));
            Assert.False(TemplateMatcher.Match(template, NodeOf(facts)).IsComplete);
        }

        [Fact]
        public void ProfilePolicy_WarnWritesTheModelProfile_RequireKeepsTheTemplateOne()
        {
            var (warnTemplate, _, _, _) = BuildDetalleD();
            var (requireTemplate, _, _, _) = BuildDetalleD(ProfilePolicy.Require);
            var (ignoreTemplate, _, _, _) = BuildDetalleD(ProfilePolicy.Ignore);
            var facts = new FakeModelFacts();
            facts.Members[1249630].TypeName = "HSS3X3X3/16 76x76";
            TemplateNode node = NodeOf(facts);

            InstantiationResult warned = TemplateInstantiator.Instantiate(warnTemplate, TemplateMatcher.Match(warnTemplate, node), node);
            ApiError warning = Assert.Single(warned.Warnings, w => w.Code == ErrorCodes.TemplateProfileDiffers);
            Assert.Equal("members[0].profile", warning.Path);
            Assert.Equal("HSS3X3X3/16 76x76", warned.Spec.Members[0].Profile);
            Assert.Equal("HSS2-1/2X2-1/2X3/16", warned.Spec.Members[1].Profile);
            Assert.True(SpecValidator.Validate(warned.SpecJson, warned.Spec, facts, RepoLimits()).IsValid);

            InstantiationResult required = TemplateInstantiator.Instantiate(requireTemplate, TemplateMatcher.Match(requireTemplate, node), node);
            Assert.Empty(required.Warnings);
            Assert.Equal("HSS2-1/2X2-1/2X3/16", required.Spec.Members[0].Profile);
            ValidationResult validation = SpecValidator.Validate(required.SpecJson, required.Spec, facts, RepoLimits());
            Assert.Contains(validation.Errors, e => e.Code == ErrorCodes.ProfileMismatch && e.Path == "members[0].profile");

            InstantiationResult ignored = TemplateInstantiator.Instantiate(ignoreTemplate, TemplateMatcher.Match(ignoreTemplate, node), node);
            Assert.Empty(ignored.Warnings);
            Assert.Equal("HSS3X3X3/16 76x76", ignored.Spec.Members[0].Profile);
        }

        [Fact]
        public void Store_SavesListsGetsFindsAndDeletesTemplates()
        {
            string folder = Path.Combine(Path.GetTempPath(), "MotorConexiones-catalogo-" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new CatalogStore(folder);
                Assert.Empty(store.List());
                var (template, _, _, _) = BuildDetalleD();

                string file = store.Save(template);
                Assert.True(File.Exists(file));
                Assert.Equal(Path.Combine(store.Folder, template.TemplateId + ".json"), file);

                File.WriteAllText(Path.Combine(folder, "roto.json"), "{ esto no es json");
                var warnings = new List<ApiError>();
                IReadOnlyList<CatalogEntry> entries = store.List(warnings);
                CatalogEntry entry = Assert.Single(entries);
                Assert.Equal(template.TemplateId, entry.TemplateId);
                Assert.Equal("Nudo típico Detalle D", entry.Name);
                Assert.Equal(3, entry.MembersCount);
                Assert.Equal("HSS3X3X1/4", entry.ChordProfile);
                Assert.Equal(new[] { "hangar", "cercha", "HSS" }, entry.Tags);
                ApiError warning = Assert.Single(warnings);
                Assert.Equal(ErrorCodes.TemplateInvalid, warning.Code);
                Assert.Contains("roto.json", warning.Message);

                CatalogTemplate? read = store.Get(template.TemplateId);
                Assert.NotNull(read);
                Assert.Equal(template.SpecTemplate!.ToJsonString(), read!.SpecTemplate!.ToJsonString());
                Assert.NotNull(store.FindByName("nudo TÍPICO detalle d"));
                Assert.Null(store.FindByName("no existe"));
                Assert.Null(store.Get("00000000-0000-0000-0000-000000000000"));
                Assert.Throws<CatalogException>(() => store.Get("../fuera"));

                string copy = CatalogStore.CopyTo(file, Path.Combine(folder, "compartida"));
                Assert.True(File.Exists(copy));

                Assert.True(store.Delete(template.TemplateId, out string deleted));
                Assert.Equal(file, deleted);
                Assert.False(File.Exists(file));
                Assert.False(store.Delete(template.TemplateId, out _));
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
            }
        }

        [Fact]
        public void Config_LoadsTheRepoFileAndFallsBackOnBadValues()
        {
            CatalogConfig config = CatalogConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "catalog.json")));
            Assert.Equal(CatalogConfig.DefaultCatalogFolder, config.CatalogFolder);
            Assert.Equal(10.0, config.AngleToleranceDeg);
            Assert.Equal(5.0, config.AngleDeviationWarningDeg);
            Assert.True(config.AllowMirror);
            Assert.Equal(ProfilePolicy.Warn, config.DefaultProfilePolicy);
            Assert.Equal(10.0, config.NodeClusterMm);
            Assert.False(string.IsNullOrEmpty(config.SharedCatalogFolder));

            CatalogConfig bad = CatalogConfig.LoadFromJson("{\"catalog_folder\": \"\", \"angle_tolerance_deg\": -3, \"default_profile_policy\": \"lo que sea\"}");
            Assert.Equal(CatalogConfig.DefaultCatalogFolder, bad.CatalogFolder);
            Assert.Equal(10.0, bad.AngleToleranceDeg);
            Assert.Equal(ProfilePolicy.Warn, bad.DefaultProfilePolicy);
            Assert.Equal(ProfilePolicy.Require, ProfilePolicy.Normalize(" REQUIRE "));
            Assert.Equal(ProfilePolicy.Warn, ProfilePolicy.Normalize(null));
            Assert.Equal(CatalogConfig.Default.CatalogFolder, CatalogConfig.LoadFromFile("/no/existe.json").CatalogFolder);
        }

        [Fact]
        public void Schema_AcceptsSourceTemplateIdAndBatchId_AndRejectsOtherSourceKeys()
        {
            var (rawJson, _) = SketchBuilderTests.LoadConfirmedFixture();
            string withIds = rawJson.Replace("\"drawing\": \"Detalle D\",", "\"drawing\": \"Detalle D\", \"template_id\": \"abc\", \"batch_id\": null,");
            Assert.NotEqual(rawJson, withIds);
            Assert.Empty(JsonSchemaValidator.Validate(withIds));
            ConnectionSpec spec = ConnectionSpec.FromJson(withIds)!;
            Assert.Equal("abc", spec.Source!.TemplateId);
            Assert.Contains("\"template_id\":\"abc\"", spec.ToJson());

            string unknown = rawJson.Replace("\"drawing\": \"Detalle D\",", "\"drawing\": \"Detalle D\", \"catalog\": \"x\",");
            ApiError error = Assert.Single(JsonSchemaValidator.Validate(unknown));
            Assert.Equal("source.catalog", error.Path);

            // Sin template_id la especificación serializa igual que antes: el token de las especificaciones de siempre no cambia.
            ConnectionSpec plain = ConnectionSpec.FromJson(rawJson)!;
            Assert.DoesNotContain("template_id", plain.ToJson());
        }

        [Fact]
        public void Validator_PlateRule_UsesTheRealMemberAngleWithTheConfiguredTolerance()
        {
            // En los datos del Hangar la diagonal inferior está a −135° y la placa cuchilla termina en el chaflán inferior
            // izquierdo de la cartela: una esquina asoma 1,4 mm. Con la tolerancia de limits.json (2 mm) valida; sin ella, no.
            var (_, spec) = SketchBuilderTests.LoadConfirmedFixture();
            var facts = new FakeModelFacts();
            Assert.True(SpecValidator.Validate(spec.ToJson(), spec, facts, RepoLimits()).IsValid);

            LimitsConfig strict = LimitsConfig.LoadFromJson("{\"plate_outside_gusset_tolerance_mm\": 0.0}");
            ValidationResult result = SpecValidator.Validate(spec.ToJson(), spec, facts, strict);
            ApiError error = Assert.Single(result.Errors, e => e.Code == ErrorCodes.PlateOutsideGusset);
            Assert.Contains("-135.0°", error.Message);
            Assert.NotEqual(RepoLimits().ComputeHash(), strict.ComputeHash());

            // Sin modelo se usa el ángulo escrito (cuadrante +X +Y), como hasta ahora.
            Assert.True(SpecValidator.Validate(spec.ToJson(), spec, null, RepoLimits()).IsValid);
        }

        [Fact]
        public void Validator_AngleRule_ComparesTheInclinationNotTheSign()
        {
            var (_, spec) = SketchBuilderTests.LoadConfirmedFixture();
            var facts = new FakeModelFacts(); // 1249636 está a −135° en el marco canónico; el plano dice 45°.
            ValidationResult ok = SpecValidator.Validate(spec.ToJson(), spec, facts, RepoLimits());
            Assert.DoesNotContain(ok.Warnings, w => w.Code == ErrorCodes.AngleDiffersFromModel);

            spec.Members[2].ExpectedAngleDeg = 60.0;
            ValidationResult warned = SpecValidator.Validate(spec.ToJson(), spec, facts, RepoLimits());
            ApiError warning = Assert.Single(warned.Warnings, w => w.Code == ErrorCodes.AngleDiffersFromModel);
            Assert.Equal("members[2].expected_angle_deg", warning.Path);
            Assert.Contains("-135.0°", warning.Message);
            Assert.Contains("15.0°", warning.Message);
        }
    }
}
