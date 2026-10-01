using System;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Editor de campos por ruta JSON (Fase 6): lo que la tabla de la ventana escribe en la especificación, probado
    /// con el fixture confirmado del Detalle D y el validador real.
    /// </summary>
    public class SpecEditorTests
    {
        private static ValidationResult Validate(string json)
        {
            return SpecValidator.Validate(json, ConnectionSpec.FromJson(json), new FakeModelFacts(), LimitsConfig.Default);
        }

        [Fact]
        public void ListFields_CoversStep8Table()
        {
            var (_, spec) = SketchBuilderTests.LoadConfirmedFixture();
            var fields = SpecEditor.ListFields(spec);

            SpecField thickness = fields.Single(f => f.Path == "gusset.thickness_mm");
            Assert.Equal("Cartela", thickness.Section);
            Assert.Equal("9.525", thickness.Value);
            Assert.Equal(SpecFieldKind.Number, thickness.Kind);

            Assert.Equal("HSS3X3X1/4", fields.Single(f => f.Path == "chord.profile").Value);
            Assert.Equal("60", fields.Single(f => f.Path == "members[2].attachment.bolts.spacing_mm").Value);
            Assert.Equal("2", fields.Single(f => f.Path == "members[2].attachment.bolts.rows").Value);
            Assert.Equal("150", fields.Single(f => f.Path == "members[0].attachment.slot_length_mm").Value);
            Assert.Equal("75; 420; 70", fields.Single(f => f.Path == "dimension_chains[0].values_mm").Value);
            Assert.Equal("HSS2-1/2X2-1/2X3/16", fields.Single(f => f.Path == "uncertain_fields[0].user_confirmed_value").Value);
            Assert.Equal("through_slot", fields.Single(f => f.Path == "uncertain_fields[1].user_confirmed_value").Value);
            Assert.False(fields.Single(f => f.Path == "gusset.outline.points_mm").IsEditable);

            // Una sección por barra, con su rol e id, y las ranuras soldadas no muestran campos de pernos.
            Assert.Equal(3, fields.Select(f => f.Section).Distinct().Count(s => s.StartsWith("Barra ", StringComparison.Ordinal)));
            Assert.DoesNotContain(fields, f => f.Path.StartsWith("members[0].attachment.bolts", StringComparison.Ordinal));
            Assert.Contains(fields, f => f.Path == "members[2].attachment.plate.insertion_mm");
        }

        [Fact]
        public void SetThickness_WithoutLabel_GivesLabelMismatch_AndWithLabel_IsValidWithNewToken()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            string originalToken = Validate(json).ValidationToken!;

            Assert.True(SpecEditor.TrySetValue(json, "gusset.thickness_mm", "12,7", out string json2, out string error), error);
            Assert.Equal(12.7, ConnectionSpec.FromJson(json2)!.Gusset!.ThicknessMm);
            ValidationResult mismatch = Validate(json2);
            Assert.Contains(mismatch.Errors, e => e.Code == ErrorCodes.LabelValueMismatch && e.Path == "gusset.thickness_label");

            Assert.True(SpecEditor.TrySetValue(json2, "gusset.thickness_label", "1/2\"", out string json3, out error), error);
            ValidationResult ok = Validate(json3);
            Assert.True(ok.IsValid, string.Join("; ", ok.Errors.Select(e => e.Code)));
            Assert.NotNull(ok.ValidationToken);
            Assert.NotEqual(originalToken, ok.ValidationToken);
            Assert.Equal(64, ok.ValidationToken!.Length);
        }

        [Fact]
        public void SetBoltSpacing10_GivesBoltSpacingTooSmall()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.bolts.spacing_mm", "10", out string json2, out string error), error);
            ValidationResult result = Validate(json2);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.BoltSpacingTooSmall && e.Path == "members[2].attachment.bolts.spacing_mm");
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void SetDimensionChain_402_GivesDimensionChainMismatch()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "dimension_chains[0].values_mm", "75; 402; 70", out string json2, out string error), error);
            var spec = ConnectionSpec.FromJson(json2)!;
            Assert.Equal(new[] { 75.0, 402.0, 70.0 }, spec.DimensionChains[0].ValuesMm);
            Assert.Contains(Validate(json2).Errors, e => e.Code == ErrorCodes.DimensionChainMismatch);
        }

        [Fact]
        public void SetUncertainValue_ToEmpty_BlocksTheToken_AndBackAgain()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "uncertain_fields[0].user_confirmed_value", "", out string json2, out string error), error);
            Assert.Contains(Validate(json2).Errors, e => e.Code == ErrorCodes.UnresolvedUncertainty);

            Assert.True(SpecEditor.TrySetValue(json2, "uncertain_fields[0].user_confirmed_value", "HSS2-1/2X2-1/2X3/16", out string json3, out error), error);
            Assert.True(Validate(json3).IsValid);
            // El valor confirmado vuelve como texto, no como número.
            Assert.Equal("HSS2-1/2X2-1/2X3/16", SpecEditor.ListFields(ConnectionSpec.FromJson(json3)!).Single(f => f.Path == "uncertain_fields[0].user_confirmed_value").Value);
        }

        [Fact]
        public void InvalidTexts_AreRejectedWithoutChangingTheJson()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();

            Assert.False(SpecEditor.TrySetValue(json, "gusset.thickness_mm", "abc", out string same, out string error));
            Assert.Equal(json, same);
            Assert.Contains("no es un número", error);

            Assert.False(SpecEditor.TrySetValue(json, "members[2].attachment.bolts.rows", "2,5", out _, out error));
            Assert.Contains("entero", error);

            Assert.False(SpecEditor.TrySetValue(json, "members[7].profile", "HSS", out _, out error));
            Assert.Contains("members[7]", error);

            Assert.False(SpecEditor.TrySetValue(json, "", "1", out _, out error));
            Assert.False(SpecEditor.TrySetValue(json, "gusset.outline.points_mm[", "1", out _, out error));
        }

        [Fact]
        public void IntegersAndBooleansKeepTheirJsonType()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.bolts.rows", "3", out string json2, out string error), error);
            Assert.Contains("\"rows\": 3", json2);
            Assert.Equal(3, ConnectionSpec.FromJson(json2)!.Members[2].Attachment!.Bolts!.Rows);

            Assert.True(SpecEditor.TrySetValue(json2, "chord.continuous", "no", out string json3, out error), error);
            Assert.False(ConnectionSpec.FromJson(json3)!.Chord!.Continuous);
            Assert.Contains("\"continuous\": false", json3);

            // El esquema sigue aceptando el JSON editado (sin campos desconocidos ni tipos cambiados).
            Assert.DoesNotContain(Validate(json3).Errors, e => e.Code == ErrorCodes.SchemaInvalid);
        }

        [Fact]
        public void MissingIntermediateObjects_AreCreated()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            // El miembro 0 no tiene weld_plate_to_member: se crea el objeto y el campo.
            Assert.True(SpecEditor.TrySetValue(json, "members[0].attachment.weld_plate_to_member.size_mm", "6", out string json2, out string error), error);
            Assert.Equal(6.0, ConnectionSpec.FromJson(json2)!.Members[0].Attachment!.WeldPlateToMember!.SizeMm);
        }

        [Fact]
        public void Outline_RoundTripsThroughText()
        {
            var (json, spec) = SketchBuilderTests.LoadConfirmedFixture();
            string text = SpecEditor.FormatOutline(spec.Gusset!.Outline);
            Assert.Equal(8, text.Split('\n').Length);
            Assert.StartsWith("-175; 280", text);

            string edited = text.Replace("-175; 280", "-180; 280");
            Assert.True(SpecEditor.TrySetOutline(json, edited, out string json2, out string error), error);
            var spec2 = ConnectionSpec.FromJson(json2)!;
            Assert.Equal(-180.0, spec2.Gusset!.Outline!.PointsMm![0][0]);
            Assert.Equal("polygon", spec2.Gusset.Outline.Mode);
            Assert.True(Validate(json2).IsValid);

            Assert.False(SpecEditor.TrySetOutline(json, "1; 2\n3; 4", out _, out error));
            Assert.Contains("al menos 3", error);
            Assert.False(SpecEditor.TrySetOutline(json, "1; 2\n3; 4\nx; 6", out _, out error));
            Assert.Contains("Línea 3", error);
        }

        [Fact]
        public void PrettyJson_KeepsTheSameTokenAsTheOriginal()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            string pretty = SpecEditor.ToPrettyJson(json);
            Assert.Contains("\n", pretty);
            Assert.Equal(ValidationTokenGenerator.ToCanonicalJson(json), ValidationTokenGenerator.ToCanonicalJson(pretty));
            Assert.Equal(Validate(json).ValidationToken, Validate(pretty).ValidationToken);
            // Los acentos se guardan tal cual, no como \uXXXX.
            Assert.Contains("está cortada", pretty);
        }

        [Fact]
        public void SetElementIds_FillsNodeAndChordWithoutTouchingTheRest()
        {
            string json = "{ \"spec_version\": \"1.0\", \"connection_type\": \"gusset_node\", \"node\": { \"element_ids\": [] }, \"chord\": { \"element_id\": 0, \"profile\": \"HSS3X3X1/4\" }, \"gusset\": { \"thickness_mm\": 9.525 }, \"members\": [] }";
            Assert.True(SpecEditor.TrySetElementIds(json, new long[] { 1249510, 1249630 }, out string json2, out string error), error);
            var spec = ConnectionSpec.FromJson(json2)!;
            Assert.Equal(new long[] { 1249510, 1249630 }, spec.Node.ElementIds);
            Assert.Equal(1249510, spec.Chord!.ElementId);
            Assert.Equal("HSS3X3X1/4", spec.Chord.Profile);
            Assert.Equal(9.525, spec.Gusset!.ThicknessMm);
            // Los miembros sin expected_angle_deg no reciben un null que el esquema rechazaría.
            Assert.DoesNotContain("expected_angle_deg", json2);

            // Con cordón ya indicado, no se cambia.
            var (fixture, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetElementIds(fixture, new long[] { 5, 6 }, out string json3, out error), error);
            Assert.Equal(1249510, ConnectionSpec.FromJson(json3)!.Chord!.ElementId);
            Assert.Equal(new long[] { 5, 6 }, ConnectionSpec.FromJson(json3)!.Node.ElementIds);

            Assert.False(SpecEditor.TrySetElementIds(fixture, new long[] { 5 }, out _, out error));
            Assert.Contains("dos elementos", error);
        }

        [Fact]
        public void SetGussetSize_StretchesTheOutlineAboutTheWorkPoint()
        {
            // Ronda 6b: doble clic en la cota de ancho (565) y escribir 575 estira el contorno en X alrededor de x = 0.
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetGussetSize(json, width: true, 575.0, out string wider, out string error), error);
            ConnectionSpec spec = ConnectionSpec.FromJson(wider)!;
            Assert.Equal(575.0, spec.Gusset!.WidthMm);
            Assert.Equal(530.0, spec.Gusset.HeightMm);
            var points = spec.Gusset.Outline!.PointsMm!;
            Assert.Equal(8, points.Count);
            Assert.Equal(-254.42, points.Min(p => p[0]), 2);
            Assert.Equal(320.58, points.Max(p => p[0]), 2);
            Assert.Equal(575.0, points.Max(p => p[0]) - points.Min(p => p[0]), 1);
            // Las Y no cambian.
            Assert.Equal(280.0, points.Max(p => p[1]), 6);
            Assert.Equal(-250.0, points.Min(p => p[1]), 6);

            // El croquis mide el ancho nuevo y la tabla lo muestra.
            var sketch = Core.Sketch.SketchBuilder.Build(spec, Core.Sketch.SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));
            Assert.Equal("575,0", sketch.Dimensions.Single(d => d.Kind == Core.Sketch.DimensionKind.GussetWidth).Text);
            Assert.Equal("575", SpecEditor.ListFields(spec).Single(f => f.Path == "gusset.width_mm").Value);

            // Alto 530 → 500 estira en Y; el esquema sigue aceptando el JSON y hay token.
            Assert.True(SpecEditor.TrySetGussetSize(wider, width: false, 500.0, out string shorter, out error), error);
            ConnectionSpec spec2 = ConnectionSpec.FromJson(shorter)!;
            Assert.Equal(500.0, spec2.Gusset!.HeightMm);
            Assert.Equal(264.15, spec2.Gusset.Outline!.PointsMm!.Max(p => p[1]), 2);
            Assert.Equal(-235.85, spec2.Gusset.Outline.PointsMm!.Min(p => p[1]), 2);
            Assert.NotNull(Validate(shorter).ValidationToken);

            // Valores imposibles se rechazan sin tocar el JSON.
            Assert.False(SpecEditor.TrySetGussetSize(json, width: true, 0.0, out string unchanged, out error));
            Assert.Equal(json, unchanged);
            Assert.Contains("positivo", error);
        }

        [Fact]
        public void IntegerLookingValue_DoesNotTurnTheFieldIntoAnInteger()
        {
            // Visto en el PC (Fase 6, paso 6-5): tras escribir "9" en thickness_mm, "12,7" se rechazaba como "debe ser entero".
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "gusset.thickness_mm", "9", out string json2, out string error), error);
            Assert.True(SpecEditor.TrySetValue(json2, "gusset.thickness_mm", "12,7", out string json3, out error), error);
            Assert.Equal(12.7, ConnectionSpec.FromJson(json3)!.Gusset!.ThicknessMm);

            // Las listas de cotas tampoco se vuelven enteras.
            Assert.True(SpecEditor.TrySetValue(json3, "dimension_chains[0].values_mm", "75,5; 419,5; 70", out string json4, out error), error);
            Assert.Equal(new[] { 75.5, 419.5, 70.0 }, ConnectionSpec.FromJson(json4)!.DimensionChains[0].ValuesMm);

            // Y los enteros del contrato siguen siendo enteros.
            Assert.False(SpecEditor.TrySetValue(json4, "members[2].attachment.bolts.rows", "2.5", out _, out error));
            Assert.True(SpecEditor.TrySetValue(json4, "node.element_ids", "1249510; 1249630", out string json5, out error), error);
            Assert.Contains("1249510,", json5.Replace(" ", "").Replace("\n", "").Replace("\r", ""));
        }

        [Theory]
        [InlineData("12,7", 12.7)]
        [InlineData("12.7", 12.7)]
        [InlineData(" 565 ", 565.0)]
        [InlineData("-250", -250.0)]
        public void TryParseNumber_AcceptsCommaAndDot(string text, double expected)
        {
            Assert.True(SpecEditor.TryParseNumber(text, out double value));
            Assert.Equal(expected, value, 9);
        }
    }
}
