using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Utilidades compartidas por las pruebas de la Fase 11: el nudo del Hangar y las plantillas del Detalle D.</summary>
    internal static class Fase11Fixtures
    {
        public const long Chord = 1249510;
        public static readonly long[] MemberIds = { 1249630, 1249631, 1249636 };

        public static TemplateNode HangarNode(FakeModelFacts? facts = null) => TemplateNode.FromModelFacts(facts ?? new FakeModelFacts(), Chord, MemberIds);

        /// <summary>La plantilla oficial del repositorio (catalog\6abcf116-….json), medida en el modelo real (136,9° / 44,4° / −135,6°).</summary>
        public static CatalogTemplate RepoTemplate()
        {
            string file = Directory.EnumerateFiles(Path.GetDirectoryName(SketchBuilderTests.FindRepoFile(Path.Combine("catalog", "6abcf116-9b97-485f-b50d-2851ca0018cc.json")))!, "*.json").First();
            return CatalogTemplate.FromJson(File.ReadAllText(file)) ?? throw new InvalidOperationException("La plantilla del repositorio no se pudo leer.");
        }

        /// <summary>Una plantilla construida desde el fixture confirmado sobre el nudo del Hangar (ángulos exactos 135 / 45 / −135).</summary>
        public static CatalogTemplate ExactTemplate(string name = "Detalle D exacto", Func<string, string>? editJson = null, string? createdUtc = null)
        {
            var (rawJson, _) = SketchBuilderTests.LoadConfirmedFixture();
            if (editJson != null) rawJson = editJson(rawJson);
            ConnectionSpec spec = ConnectionSpec.FromJson(rawJson)!;
            CatalogTemplate template = TemplateBuilder.Build(rawJson, spec, HangarNode(), new TemplateMetadata { Name = name, DocumentTitle = "HANGAR_PRUEBA_sondeo" }, CatalogConfig.Default);
            if (createdUtc != null) template.CreatedUtc = createdUtc;
            return template;
        }

        public static LimitsConfig RepoLimits() => LimitsConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "limits.json")));

        public static PanelChoices RepoChoices() => PanelChoices.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "choices.json")));
    }

    /// <summary>Fase 11: casar todas las plantillas y proponer la mejor (botón Conectar, Cambiar… y conn_catalog_apply sin template_id).</summary>
    public class TemplateChooserTests
    {
        [Fact]
        public void Propose_WithTheDetalleDTemplate_ChoosesItInSameOrientation()
        {
            TemplateProposal proposal = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new[] { Fase11Fixtures.ExactTemplate() });
            Assert.True(proposal.HasChoice);
            Assert.Equal(TemplateOrientation.Same, proposal.Chosen!.Match.Orientation);
            Assert.True(proposal.Chosen.MaxDeviationDeg < 0.05, proposal.Chosen.MaxDeviationDeg.ToString());
            Assert.StartsWith("Plantilla propuesta: Detalle D exacto · igual · desvío máx. 0,0°. Otras que encajan: ninguna.", proposal.SummaryText);
            Assert.Empty(proposal.Attempts);
            Assert.Equal(1, proposal.TemplatesCount);
            Assert.Equal(new[] { 135.0, 45.0, -135.0 }, proposal.MemberAnglesDeg);
        }

        [Fact]
        public void Propose_WithAMirroredNode_ChoosesMirrorX()
        {
            TemplateNode mirrored = Fase11Fixtures.HangarNode(CatalogTests.MirroredFacts(true, false));
            TemplateProposal proposal = TemplateChooser.Propose(mirrored, new[] { Fase11Fixtures.ExactTemplate() });
            Assert.True(proposal.HasChoice);
            Assert.Equal("mirror_x", proposal.Chosen!.OrientationName);
            Assert.True(proposal.Chosen.IsMirrored);
            Assert.Contains("en espejo (mirror_x)", proposal.SummaryText);
        }

        [Fact]
        public void Propose_WithTwoFittingTemplates_TheSmallerDeviationWinsAndTheOtherIsACandidate()
        {
            CatalogTemplate exact = Fase11Fixtures.ExactTemplate();
            CatalogTemplate repo = Fase11Fixtures.RepoTemplate();
            // La del repositorio va primera en la lista: aun así gana la exacta por menor desvío.
            TemplateProposal proposal = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new[] { repo, exact });
            Assert.Equal(2, proposal.Candidates.Count);
            Assert.Same(exact, proposal.Chosen!.Template);
            Assert.Same(repo, proposal.Candidates[1].Template);
            Assert.True(proposal.Candidates[1].MaxDeviationDeg > 1.0 && proposal.Candidates[1].MaxDeviationDeg < 2.5, proposal.Candidates[1].MaxDeviationDeg.ToString());
            Assert.Contains("Otras que encajan: Nudo tipico Detalle D (igual, ", proposal.SummaryText);
            Assert.NotNull(proposal.Find(repo.TemplateId));
            Assert.Null(proposal.Find("no-existe"));
        }

        [Fact]
        public void Propose_WithEqualDeviation_TheMostRecentTemplateWins()
        {
            CatalogTemplate older = Fase11Fixtures.ExactTemplate("Vieja", null, "2026-01-01T00:00:00.0000000Z");
            CatalogTemplate newer = Fase11Fixtures.ExactTemplate("Nueva", null, "2026-10-01T00:00:00.0000000Z");
            TemplateProposal proposal = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new[] { older, newer });
            Assert.Equal("Nueva", proposal.Chosen!.Name);
        }

        [Fact]
        public void Propose_WithATwoBarTemplateAndAThreeBarNode_HasNoChoiceAndExplainsWhy()
        {
            // Plantilla de 2 barras: se quita la tercera (placa cuchilla) del fixture confirmado.
            CatalogTemplate twoBars = Fase11Fixtures.ExactTemplate("Dos barras", json =>
            {
                var root = (JsonObject)JsonNode.Parse(json)!;
                ((JsonArray)root["members"]!).RemoveAt(2);
                ((JsonArray)root["node"]!["element_ids"]!).RemoveAt(3);
                return root.ToJsonString();
            });
            TemplateProposal proposal = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new[] { twoBars });
            Assert.False(proposal.HasChoice);
            Assert.Null(proposal.Chosen);
            Assert.Equal(4, proposal.Attempts.Count);
            Assert.All(proposal.Attempts, a => Assert.StartsWith("Dos barras → ", a));
            Assert.StartsWith("Ninguna de las 1 plantilla encaja con 3 barras a +135,0°, +45,0° y −135,0°: Dos barras → sobran barras 1249636", proposal.SummaryText);

            // El plan de lote conserva la regla de la Fase 8: la barra sobrante no impide el casado (requireAllMembers: false).
            TemplateProposal lenient = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new[] { twoBars }, null, requireAllMembers: false);
            Assert.True(lenient.HasChoice);
            Assert.Equal(new[] { 1249636L }, lenient.Chosen!.Match.UnassignedMembers);
        }

        [Fact]
        public void Propose_WithAnEmptyCatalog_SaysSo()
        {
            TemplateProposal proposal = TemplateChooser.Propose(Fase11Fixtures.HangarNode(), new List<CatalogTemplate>());
            Assert.True(proposal.IsCatalogEmpty);
            Assert.False(proposal.HasChoice);
            Assert.Empty(proposal.Candidates);
            Assert.Contains("El catálogo no tiene ninguna plantilla", proposal.SummaryText);
        }
    }

    /// <summary>Fase 11 (decisión D2): la especificación de arranque del botón Conectar cuando ninguna plantilla encaja.</summary>
    public class SpecStarterTests
    {
        [Fact]
        public void FromNode_WithoutTemplates_WritesAValidStarterFromChoicesJson()
        {
            var facts = new FakeModelFacts();
            TemplateNode node = Fase11Fixtures.HangarNode(facts);
            StarterResult result = SpecStarter.FromNode(node, Fase11Fixtures.RepoChoices(), null);

            Assert.Null(result.DefaultsTemplate);
            Assert.Equal("valores por defecto de choices.json", result.DefaultsSource);
            Assert.Empty(JsonSchemaValidator.Validate(result.SpecJson));
            ConnectionSpec spec = result.Spec;
            Assert.Equal(SpecStarter.DrawingName, spec.Source!.Drawing);
            Assert.Equal(new[] { 1249510L, 1249630L, 1249631L, 1249636L }, spec.Node.ElementIds);
            Assert.Equal(1249510, spec.Chord!.ElementId);
            Assert.Equal("HSS3X3X1/4", spec.Chord.Profile);
            Assert.Equal(new[] { 1249630L, 1249631L, 1249636L }, spec.Members.Select(m => m.ElementId));
            Assert.All(spec.Members, m => Assert.Equal("HSS2-1-2X2-1-2X3-16 64x64", m.Profile));
            Assert.All(spec.Members, m => Assert.Equal("welded_slot", m.Attachment!.Type));
            Assert.All(spec.Members, m => Assert.Equal("diagonal", m.Role));
            Assert.All(spec.Members, m => Assert.Equal(60.0, m.EndSetbackMm));
            Assert.All(spec.Members, m => Assert.Equal(150.0, m.Attachment!.SlotLengthMm));
            Assert.All(spec.Members, m => Assert.Equal(5.0, m.Attachment!.Weld!.SizeMm));
            Assert.All(spec.Members, m => Assert.Equal(45.0, m.ExpectedAngleDeg));
            Assert.Equal(9.525, spec.Gusset!.ThicknessMm);
            Assert.Equal("3/8\"", spec.Gusset.ThicknessLabel);
            Assert.Equal("through_slot", spec.Gusset.ChordInterface);
            Assert.Equal(5.0, spec.Gusset.WeldToChord!.SizeMm);

            // Fase 13: la cartela es automática (outline.mode auto, sin puntos escritos); el contorno calculado (múltiplos de 5)
            // contiene los extremos recortados de las tres barras.
            Assert.Equal("auto", spec.Gusset.Outline!.Mode);
            Assert.Null(spec.Gusset.Outline.PointsMm);
            Assert.True(result.IsAutoOutline);
            Assert.Contains("cartela automática", result.HeaderText);
            AutoOutlineResult auto = AutoOutline.Resolve(spec, facts)!;
            Assert.All(auto.PointsMm.SelectMany(p => p), v => Assert.Equal(0.0, Math.Abs(v % 5.0)));
            Assert.Equal(spec.Gusset.WidthMm, result.GussetBoundsMm[2] - result.GussetBoundsMm[0]);
            Assert.Equal(spec.Gusset.HeightMm, result.GussetBoundsMm[3] - result.GussetBoundsMm[1]);
            Assert.Equal(auto.WidthMm, spec.Gusset.WidthMm!.Value, 1e-6);
            var polygon = auto.ToPolygon();
            Assert.True(polygon.IsSimple(out string reason), reason);
            foreach (TemplateNodeMember member in node.Members)
            {
                double half = member.WidthMm / 2.0;
                foreach (double along in new[] { 60.0, 60.0 + 150.0 })
                {
                    Assert.True(polygon.ContainsPoint(new Point2D(along * member.Ux - half * member.Uy, along * member.Uy + half * member.Ux)), "barra " + member.ElementId);
                    Assert.True(polygon.ContainsPoint(new Point2D(along * member.Ux + half * member.Uy, along * member.Uy - half * member.Ux)), "barra " + member.ElementId);
                }
            }
            // Y el cordón: la cartela asoma por los dos lados (±(38,1 + 40) redondeado).
            Assert.True(result.GussetBoundsMm[1] <= -75 && result.GussetBoundsMm[3] >= 75, string.Join(", ", result.GussetBoundsMm));

            // Con starter.outline_mode "polygon" en choices.json sigue saliendo el rectángulo de la Fase 11 (4 puntos, múltiplos de 5).
            PanelChoices rectangular = Fase11Fixtures.RepoChoices();
            rectangular.Starter.OutlineMode = "polygon";
            StarterResult rect = SpecStarter.FromNode(node, rectangular, null);
            Assert.Equal("polygon", rect.Spec.Gusset!.Outline!.Mode);
            Assert.Equal(4, rect.Spec.Gusset.Outline.PointsMm!.Count);
            Assert.All(rect.Spec.Gusset.Outline.PointsMm.SelectMany(p => p), v => Assert.Equal(0.0, Math.Abs(v % 5.0)));
            Assert.Contains("cartela rectangular", rect.HeaderText);

            UncertainField doubt = Assert.Single(spec.UncertainFields);
            Assert.Equal("gusset.chord_interface", doubt.Path);
            Assert.Equal("through_slot", doubt.UserConfirmedValue?.ToString());
            Assert.Contains("arranque", result.HeaderText);

            // Pasa el validador de siempre con los hechos del Hangar (los errores de geometría dependen del modelo real).
            ValidationResult validation = SpecValidator.Validate(result.SpecJson, spec, facts, Fase11Fixtures.RepoLimits());
            Assert.True(validation.IsValid, string.Join("; ", validation.Errors.Select(e => e.Code + " " + e.Message)));
            Assert.Matches("^[0-9a-f]{64}$", validation.ValidationToken);
        }

        [Fact]
        public void FromNode_WithATemplateOfTheSameChord_TakesItsValues()
        {
            // El Detalle D: cartela 3/8", soldaduras 5, la primera barra soldada tiene retiro 180 y ranura 150, unión through_slot.
            // Se cambia el espesor para distinguirlo de choices.json.
            CatalogTemplate template = Fase11Fixtures.ExactTemplate("Detalle D 1/2", json => json.Replace("\"thickness_mm\": 9.525,", "\"thickness_mm\": 12.7,").Replace("\"thickness_label\": \"3/8\\\"\",", "\"thickness_label\": \"1/2\\\"\","));
            StarterResult result = SpecStarter.FromNode(Fase11Fixtures.HangarNode(), PanelChoices.Default, new[] { template });
            Assert.Same(template, result.DefaultsTemplate);
            Assert.Equal("valores de la plantilla Detalle D 1/2", result.DefaultsSource);
            Assert.Equal(12.7, result.Spec.Gusset!.ThicknessMm);
            Assert.Equal("1/2\"", result.Spec.Gusset.ThicknessLabel);
            Assert.All(result.Spec.Members, m => Assert.Equal(180.0, m.EndSetbackMm));
            Assert.All(result.Spec.Members, m => Assert.Equal(150.0, m.Attachment!.SlotLengthMm));
            Assert.Equal("through_slot", result.Spec.Gusset.ChordInterface);
            Assert.Empty(JsonSchemaValidator.Validate(result.SpecJson));
        }

        [Fact]
        public void FromNode_WithTwoTemplatesOfTheSameChord_TheMostRecentWins_AndAnotherChordIsIgnored()
        {
            CatalogTemplate older = Fase11Fixtures.ExactTemplate("Vieja", null, "2026-01-01T00:00:00.0000000Z");
            CatalogTemplate newer = Fase11Fixtures.ExactTemplate("Nueva", json => json.Replace("\"slot_length_mm\": 150.0", "\"slot_length_mm\": 200.0"), "2026-10-01T00:00:00.0000000Z");
            CatalogTemplate otherChord = Fase11Fixtures.ExactTemplate("Otro cordón", null, "2026-12-01T00:00:00.0000000Z");
            otherChord.ChordPattern.Profile = "HSS4X4X3/8";
            StarterResult result = SpecStarter.FromNode(Fase11Fixtures.HangarNode(), PanelChoices.Default, new[] { older, otherChord, newer });
            Assert.Same(newer, result.DefaultsTemplate);
            Assert.All(result.Spec.Members, m => Assert.Equal(200.0, m.Attachment!.SlotLengthMm));

            StarterResult none = SpecStarter.FromNode(Fase11Fixtures.HangarNode(), PanelChoices.Default, new[] { otherChord });
            Assert.Null(none.DefaultsTemplate);
        }

        [Fact]
        public void FromNode_WithAVerticalMember_CallsItVertical()
        {
            var facts = new FakeModelFacts();
            // La barra 1249631 pasa a ser un montante: vertical desde el punto de trabajo.
            facts.Members[1249631].CurveEndMm = new Core.Geometry3D.Vec3(-11867.7, -17195.8, 19423.0);
            StarterResult result = SpecStarter.FromNode(Fase11Fixtures.HangarNode(facts), PanelChoices.Default, null);
            Assert.Equal("vertical", result.Spec.Members.Single(m => m.ElementId == 1249631).Role);
            Assert.Equal(90.0, result.Spec.Members.Single(m => m.ElementId == 1249631).ExpectedAngleDeg);
        }
    }

    /// <summary>Fase 11: las listas de los desplegables (config/choices.json) y la conversión rótulo ↔ mm.</summary>
    public class PanelChoicesTests
    {
        [Fact]
        public void RepoFile_LoadsAndEveryLabelConvertsToMm()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            Assert.EndsWith("choices.json", choices.Source);
            Assert.Equal(13, choices.ThicknessLabels.Count);
            Assert.Equal(13, choices.ThicknessOptions().Count);
            Assert.Equal(new[] { "1/2\"", "5/8\"", "3/4\"", "7/8\"" }, choices.BoltDiameterLabels);
            Assert.Equal(new[] { 3.0, 4.0, 5.0, 6.0, 8.0, 10.0 }, choices.WeldSizesMm);
            Assert.Equal(150.0, choices.SlotLengthMm);
            Assert.Equal(60.0, choices.EndSetbackMm);
            Assert.Equal(150.0, choices.Starter.SlotLengthMm);
            Assert.Equal("soldada en ranura", choices.AttachmentTypes.Single(a => a.Value == "welded_slot").Text);
            Assert.Equal("placa cuchilla atornillada", choices.AttachmentTypes.Single(a => a.Value == "bolted_knife_plate").Text);
            Assert.Equal(3, choices.ChordInterfaces.Count);
            Assert.Equal(9.525, choices.Starter.GussetThicknessMm);
            Assert.Equal(10.0, choices.Starter.KnifePlate.ThicknessMm);
            Assert.Equal(15.875, choices.Starter.Bolts.DiameterMm);
            Assert.Equal("3/8\" · 9,5 mm", choices.ThicknessOptions().Single(o => o.Label == "3/8\"").Text);
            Assert.Equal("PL10 · 10,0 mm", choices.ThicknessOptions().Single(o => o.Label == "PL10").Text);
            Assert.Equal("5 mm", choices.WeldOptions().Single(o => o.ValueMm == 5.0).Text);
        }

        [Fact]
        public void BoltDiameters_ExistInLimitsJson_AndTheOthersAreStillShownAsOther()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            Assert.Equal(0, choices.FilterBoltDiameters(Fase11Fixtures.RepoLimits()));
            Assert.Empty(choices.DiscardedBoltLabels);
            List<ChoiceOption> options = choices.BoltDiameterOptions();
            Assert.Equal(new[] { 12.7, 15.875, 19.05, 22.225 }, options.Select(o => o.ValueMm));

            // 25,4 está en limits.json pero no en la lista: se enseña como "Otro" sin perderse.
            ChoiceOption other = PanelChoices.OptionFor(options, 25.4)!;
            Assert.True(other.IsOther);
            Assert.Equal("Otro: 25,4 mm", other.Text);
            Assert.Equal(25.4, other.ValueMm);

            // Un diámetro que no está en limits.json se descarta de la lista con su nombre.
            choices.BoltDiameterLabels.Add("2\"");
            Assert.Equal(1, choices.FilterBoltDiameters(Fase11Fixtures.RepoLimits()));
            Assert.Equal(new[] { "2\"" }, choices.DiscardedBoltLabels);
            Assert.Equal(4, choices.BoltDiameterLabels.Count);
        }

        [Fact]
        public void OptionFor_FindsTheLabelWithinTolerance_OrReturnsOther()
        {
            List<ChoiceOption> thickness = PanelChoices.Default.ThicknessOptions();
            Assert.Equal("3/8\"", PanelChoices.OptionFor(thickness, 9.525)!.Label);
            Assert.Equal("3/8\"", PanelChoices.OptionFor(thickness, 9.5)!.Label);
            ChoiceOption other = PanelChoices.OptionFor(thickness, 9.2)!;
            Assert.True(other.IsOther);
            Assert.Equal("Otro: 9,2 mm", other.Text);
            Assert.Null(PanelChoices.OptionFor(thickness, null));
            Assert.Null(PanelChoices.FindOption(thickness, 9.2));
        }

        [Fact]
        public void WithoutFile_TheEmbeddedListsAreNotEmpty_AndBadJsonFallsBack()
        {
            PanelChoices missing = PanelChoices.LoadFromFile(Path.Combine(Path.GetTempPath(), "no-existe-" + Guid.NewGuid() + ".json"));
            Assert.Equal("embebidas", missing.Source);
            Assert.NotEmpty(missing.ThicknessOptions());
            Assert.NotEmpty(missing.BoltDiameterOptions());
            Assert.NotEmpty(missing.WeldOptions());
            Assert.NotEmpty(missing.AttachmentTypes);

            PanelChoices bad = PanelChoices.LoadFromJson("{ esto no es json");
            Assert.NotEmpty(bad.ThicknessOptions());
            Assert.StartsWith("embebidas", bad.Source);

            // Listas vacías en el archivo se rellenan con las embebidas; los valores nuevos se respetan.
            PanelChoices partial = PanelChoices.LoadFromJson("{ \"thickness_labels\": [], \"weld_sizes_mm\": [4, 6], \"slot_length_mm\": 120 }");
            Assert.NotEmpty(partial.ThicknessOptions());
            Assert.Equal(new[] { 4.0, 6.0 }, partial.WeldSizesMm);
            Assert.Equal(120.0, partial.Starter.SlotLengthMm);
        }
    }

    /// <summary>Fase 11 (V4): el croquis de una plantilla sin modelo, para las miniaturas.</summary>
    public class TemplateSketchTests
    {
        [Fact]
        public void Build_FromTheRepoTemplate_HasChordThreeBarsGussetPlateAndFourBolts()
        {
            CatalogTemplate template = Fase11Fixtures.RepoTemplate();
            Sketch sketch = TemplateSketch.Build(template, Fase11Fixtures.RepoLimits());

            Assert.Single(sketch.Polygons, p => p.Kind == SketchKind.Gusset);
            Assert.Equal(3, sketch.Polygons.Count(p => p.Kind == SketchKind.MemberOutline));
            Assert.Single(sketch.Polygons, p => p.Kind == SketchKind.KnifePlate);
            Assert.Equal(2, sketch.Polygons.Count(p => p.Kind == SketchKind.Slot));
            Assert.Equal(4, sketch.Circles.Count(c => c.Kind == SketchKind.Bolt));
            Assert.Equal(2, sketch.Lines.Count(l => l.Kind == SketchKind.ChordEdge));
            Assert.False(sketch.GetBounds().IsEmpty);
            Assert.True(sketch.GetBounds().Width > 565 && sketch.GetBounds().Height > 530);
            // Sin modelo no hay nota de "dirección aproximada": las direcciones salen del patrón.
            Assert.DoesNotContain(sketch.Notes, n => n.Contains("aproximad"));
            Assert.Equal(565.0, Math.Round(sketch.Dimensions.Single(d => d.Kind == DimensionKind.GussetWidth).ValueMm, 1));
        }

        [Fact]
        public void NodeInfoOf_UsesThePatternAnglesAndTheProfileWidths()
        {
            CatalogTemplate template = Fase11Fixtures.RepoTemplate();
            ConnectionSpec spec = TemplateSketch.Instantiate(template, out string json);
            Assert.Equal(TemplateSketch.FakeChordId, spec.Chord!.ElementId);
            Assert.Equal(new[] { 2L, 3L, 4L }, spec.Members.Select(m => m.ElementId));
            Assert.Equal(new[] { 1L, 2L, 3L, 4L }, spec.Node.ElementIds);
            Assert.DoesNotContain("\"slot\"", json);
            Assert.Empty(JsonSchemaValidator.Validate(json));

            SketchNodeInfo info = TemplateSketch.NodeInfoOf(template, spec);
            Assert.Equal(new[] { 136.92, 44.37, -135.63 }, info.Members.Select(m => Math.Round(m.AngleInPlaneDeg, 2)));
            Assert.All(info.Members, m => Assert.Equal(63.5, m.WidthMm, 3));
            Assert.Equal(76.2, info.ChordWidthMm, 3);
            Assert.False(info.IsApproximate);
            Assert.Null(TemplateSketch.WidthOf("W12X26"));
            Assert.Null(TemplateSketch.WidthOf(null));
        }
    }

    /// <summary>Fase 11: lo nuevo del editor (desplegables que escriben los mm y el rótulo, cambio de tipo de unión, origen).</summary>
    public class SpecEditorFase11Tests
    {
        private static ValidationResult Validate(string json) => SpecValidator.Validate(json, ConnectionSpec.FromJson(json), new FakeModelFacts(), LimitsConfig.Default);

        [Fact]
        public void TrySetLabeledValue_WritesMmAndLabelTogether_WithoutLabelMismatch()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetLabeledValue(json, "gusset.thickness_mm", "gusset.thickness_label", "1/2\"", out string json2, out string error), error);
            ConnectionSpec spec = ConnectionSpec.FromJson(json2)!;
            Assert.Equal(12.7, spec.Gusset!.ThicknessMm);
            Assert.Equal("1/2\"", spec.Gusset.ThicknessLabel);
            Assert.DoesNotContain(Validate(json2).Errors, e => e.Code == ErrorCodes.LabelValueMismatch);

            Assert.True(SpecEditor.TrySetLabeledValue(json2, "members[2].attachment.bolts.diameter_mm", "members[2].attachment.bolts.diameter_label", "3/4\"", out string json3, out error), error);
            spec = ConnectionSpec.FromJson(json3)!;
            Assert.Equal(19.05, spec.Members[2].Attachment!.Bolts!.DiameterMm);
            Assert.Equal("3/4\"", spec.Members[2].Attachment!.Bolts!.DiameterLabel);
            Assert.True(Validate(json3).IsValid, string.Join("; ", Validate(json3).Errors.Select(e => e.Code)));

            Assert.False(SpecEditor.TrySetLabeledValue(json3, "gusset.thickness_mm", "gusset.thickness_label", "grueso", out string unchanged, out error));
            Assert.Equal(json3, unchanged);
            Assert.Contains("grueso", error);
        }

        [Fact]
        public void TrySetAttachmentType_CreatesTheMissingBlock_AndRestoresTheOldOneOnTheWayBack()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            StarterDefaults defaults = Fase11Fixtures.RepoChoices().Starter;
            // Ranura distinta para comprobar que vuelve exactamente la de antes.
            Assert.True(SpecEditor.TrySetValue(json, "members[0].attachment.slot_length_mm", "123", out json, out string error), error);

            Assert.True(SpecEditor.TrySetAttachmentType(json, 0, "bolted_knife_plate", defaults, null, out string knife, out JsonObject? previous, out error), error);
            Assert.NotNull(previous);
            Assert.Equal("welded_slot", previous!["type"]!.ToString());
            ConnectionSpec spec = ConnectionSpec.FromJson(knife)!;
            AttachmentSpec attachment = spec.Members[0].Attachment!;
            Assert.Equal("bolted_knife_plate", attachment.Type);
            Assert.Null(attachment.SlotLengthMm);
            Assert.Equal(10.0, attachment.Plate!.ThicknessMm);
            Assert.Equal("PL10", attachment.Plate.ThicknessLabel);
            Assert.Equal(170.0, attachment.Plate.LengthMm);
            Assert.Equal("+z", attachment.Plate.GussetFace);
            Assert.Equal(15.875, attachment.Bolts!.DiameterMm);
            Assert.Equal("5/8\"", attachment.Bolts.DiameterLabel);
            Assert.Equal(2, attachment.Bolts.Rows);
            Assert.Equal(40.0, attachment.Bolts.EdgeMm);
            Assert.Equal(5.0, attachment.WeldPlateToMember!.SizeMm);
            Assert.Empty(JsonSchemaValidator.Validate(knife));
            Assert.Contains(SpecEditor.ListFields(spec), f => f.Path == "members[0].attachment.bolts.rows");

            // Mismo tipo otra vez: no cambia nada.
            Assert.True(SpecEditor.TrySetAttachmentType(knife, 0, "bolted_knife_plate", defaults, null, out string same, out JsonObject? none, out error), error);
            Assert.Equal(knife, same);
            Assert.Null(none);

            // Vuelta a soldada en ranura con el bloque guardado: la ranura de 123 vuelve.
            Assert.True(SpecEditor.TrySetAttachmentType(knife, 0, "welded_slot", defaults, previous, out string back, out JsonObject? knifeBlock, out error), error);
            Assert.Equal("bolted_knife_plate", knifeBlock!["type"]!.ToString());
            Assert.Equal(123.0, ConnectionSpec.FromJson(back)!.Members[0].Attachment!.SlotLengthMm);

            // Sin bloque guardado, los valores por defecto (ranura 150).
            Assert.True(SpecEditor.TrySetAttachmentType(knife, 0, "welded_slot", defaults, null, out string fresh, out _, out error), error);
            Assert.Equal(150.0, ConnectionSpec.FromJson(fresh)!.Members[0].Attachment!.SlotLengthMm);

            Assert.False(SpecEditor.TrySetAttachmentType(json, 7, "welded_slot", defaults, null, out _, out _, out error));
            Assert.Contains("members[7]", error);
            Assert.False(SpecEditor.TrySetAttachmentType(json, 0, "pegada", defaults, null, out _, out _, out error));
        }

        [Fact]
        public void ListFields_ShowsTheOriginWithTheDrawingImage()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "source.drawing_image", "C:\\planos\\detalle-D.png", out string withImage, out string error), error);
            ConnectionSpec spec = ConnectionSpec.FromJson(withImage)!;
            Assert.Equal("C:\\planos\\detalle-D.png", spec.Source!.DrawingImage);
            SpecField field = SpecEditor.ListFields(spec).Single(f => f.Path == "source.drawing_image");
            Assert.Equal("Origen", field.Section);
            Assert.Equal("C:\\planos\\detalle-D.png", field.Value);
            Assert.Equal("Detalle D", SpecEditor.ListFields(spec).Single(f => f.Path == "source.drawing").Value);
            Assert.Empty(JsonSchemaValidator.Validate(withImage));
        }
    }

    /// <summary>Fase 11 (V5): la clave nueva del contrato, source.drawing_image, en el esquema y en el catálogo.</summary>
    public class DrawingImageTests
    {
        [Fact]
        public void Schema_AcceptsDrawingImageWithPathNullOrAbsent_AndStillRejectsUnknownSourceKeys()
        {
            var (rawJson, _) = SketchBuilderTests.LoadConfirmedFixture();
            string withPath = rawJson.Replace("\"drawing\": \"Detalle D\",", "\"drawing\": \"Detalle D\", \"drawing_image\": \"C:\\\\planos\\\\detalle-D.png\",");
            Assert.NotEqual(rawJson, withPath);
            Assert.Empty(JsonSchemaValidator.Validate(withPath));
            Assert.Equal("C:\\planos\\detalle-D.png", ConnectionSpec.FromJson(withPath)!.Source!.DrawingImage);

            string withNull = rawJson.Replace("\"drawing\": \"Detalle D\",", "\"drawing\": \"Detalle D\", \"drawing_image\": null,");
            Assert.Empty(JsonSchemaValidator.Validate(withNull));
            Assert.Null(ConnectionSpec.FromJson(withNull)!.Source!.DrawingImage);
            Assert.Empty(JsonSchemaValidator.Validate(rawJson));

            string unknown = rawJson.Replace("\"drawing\": \"Detalle D\",", "\"drawing\": \"Detalle D\", \"drawing_pdf\": \"x\",");
            ApiError error = Assert.Single(JsonSchemaValidator.Validate(unknown));
            Assert.Equal(ErrorCodes.SchemaInvalid, error.Code);
            Assert.Equal("source.drawing_pdf", error.Path);

            // Sin la clave, la especificación serializa igual que antes (el token de siempre no cambia).
            Assert.DoesNotContain("drawing_image", ConnectionSpec.FromJson(rawJson)!.ToJson());
            // El validador no exige que el archivo exista.
            ValidationResult validation = SpecValidator.Validate(withPath, ConnectionSpec.FromJson(withPath), new FakeModelFacts(), LimitsConfig.Default);
            Assert.True(validation.IsValid, string.Join("; ", validation.Errors.Select(e => e.Code)));
        }

        [Fact]
        public void Catalog_KeepsTheDrawingImageInOrigin_AndGivesItBackWhenApplied()
        {
            var (rawJson, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(rawJson, "source.drawing_image", "C:\\planos\\detalle-D.png", out string withImage, out string error), error);
            ConnectionSpec spec = ConnectionSpec.FromJson(withImage)!;
            TemplateNode node = Fase11Fixtures.HangarNode();
            CatalogTemplate template = TemplateBuilder.Build(withImage, spec, node, new TemplateMetadata { Name = "Con plano" }, CatalogConfig.Default);
            Assert.Equal("C:\\planos\\detalle-D.png", template.Origin.DrawingImage);
            // spec_template no la lleva (va en origin, como el resto de la trazabilidad).
            Assert.Null(template.SpecTemplate!["source"]!["drawing_image"]);

            CatalogTemplate reread = CatalogTemplate.FromJson(template.ToJson())!;
            Assert.Equal("C:\\planos\\detalle-D.png", reread.Origin.DrawingImage);

            string folder = Path.Combine(Path.GetTempPath(), "mc-fase11-" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new CatalogStore(folder);
                store.Save(reread);
                CatalogTemplate loaded = store.Get(reread.TemplateId)!;
                Assert.Equal("C:\\planos\\detalle-D.png", loaded.Origin.DrawingImage);
                InstantiationResult applied = TemplateInstantiator.Instantiate(loaded, TemplateMatcher.Match(loaded, node), node, CatalogConfig.Default);
                Assert.Equal("C:\\planos\\detalle-D.png", applied.Spec.Source!.DrawingImage);
                Assert.Empty(JsonSchemaValidator.Validate(applied.SpecJson));
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }

            // Una plantilla sin plano no escribe la clave.
            CatalogTemplate plain = Fase11Fixtures.ExactTemplate();
            Assert.Null(plain.Origin.DrawingImage);
            Assert.DoesNotContain("drawing_image", plain.ToJson());
            InstantiationResult appliedPlain = TemplateInstantiator.Instantiate(plain, TemplateMatcher.Match(plain, node), node, CatalogConfig.Default);
            Assert.Null(appliedPlain.Spec.Source!.DrawingImage);
        }
    }
}
