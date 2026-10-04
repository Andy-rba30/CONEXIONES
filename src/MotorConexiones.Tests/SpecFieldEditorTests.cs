using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>La tabla editable de la ventana: filas que salen de la especificación y textos que vuelven a ella.</summary>
    public class SpecFieldEditorTests
    {
        private static LimitsConfig Limits() => LimitsConfig.LoadFromFile(Fixtures.RepoPath("config", "limits.json"));

        private static ValidationResult Validate(ConnectionSpec spec) => SpecValidator.Validate(spec.ToJson(), spec, new FakeModelFacts(), Limits());

        [Fact]
        public void Catalog_DetalleD_HasTheRowsOfTheSummaryTable()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            var node = SketchNodeInput.FromModelFacts(spec, new FakeModelFacts());
            var rows = SpecFieldCatalog.Build(spec, node);

            string Value(string path) => rows.Single(r => r.Path == path).Value;

            Assert.Equal("9,525", Value("gusset.thickness_mm"));
            Assert.Equal("3/8\"", Value("gusset.thickness_label"));
            Assert.Equal("565", Value("gusset.width_mm"));
            Assert.Equal("through_slot", Value("gusset.chord_interface"));
            Assert.Equal("-175; 280", Value("gusset.outline.points_mm[0]"));
            Assert.Equal("HSS3X3X1/4", Value("chord.profile"));
            Assert.Equal("sí", Value("chord.continuous"));
            Assert.Equal("180", Value("members[0].end_setback_mm"));
            Assert.Equal("150", Value("members[0].attachment.slot_length_mm"));
            Assert.Equal("welded_slot", Value("members[0].attachment.type"));
            Assert.Equal("bolted_knife_plate", Value("members[2].attachment.type"));
            Assert.Equal("60", Value("members[2].attachment.bolts.spacing_mm"));
            Assert.Equal("2", Value("members[2].attachment.bolts.rows"));
            Assert.Equal("PL10", Value("members[2].attachment.plate.thickness_label"));
            Assert.Equal("75; 420; 70", Value("dimension_chains[0].values_mm"));
            Assert.Equal("565", Value("dimension_chains[0].expected_total_mm"));
            Assert.Equal("HSS2-1/2X2-1/2X3/16", Value("uncertain_fields[0].user_confirmed_value"));
            Assert.Equal("90", Value("members[1].angle_model"));

            // Cada barra solo muestra las filas de su tipo de unión.
            Assert.DoesNotContain(rows, r => r.Path == "members[2].attachment.slot_length_mm");
            Assert.DoesNotContain(rows, r => r.Path == "members[0].attachment.plate.thickness_mm");
            Assert.True(rows.Single(r => r.Path == "uncertain_fields[0].path").IsReadOnly);
            Assert.Equal(SpecFieldCatalog.ChordInterfaceOptions, rows.Single(r => r.Path == "gusset.chord_interface").Options);
            Assert.All(rows, r => Assert.False(string.IsNullOrWhiteSpace(r.Group)));
        }

        [Fact]
        public void Apply_GussetThickness_12_7_UpdatesLabelAndKeepsValidationGreen()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            string tokenBefore = Validate(spec).ValidationToken!;

            SpecEditResult result = SpecFieldEditor.Apply(spec, "gusset.thickness_mm", "12,7");

            Assert.True(result.Ok, result.Error);
            Assert.Equal(12.7, spec.Gusset!.ThicknessMm!.Value, 6);
            Assert.Equal("1/2\"", spec.Gusset.ThicknessLabel);
            Assert.Contains("gusset.thickness_label", result.Note);

            ValidationResult after = Validate(spec);
            Assert.True(after.IsValid, string.Join("; ", after.Errors.Select(e => e.Code + ": " + e.Message)));
            Assert.DoesNotContain(after.Errors, e => e.Code == ErrorCodes.LabelValueMismatch);
            Assert.NotNull(after.ValidationToken);
            Assert.NotEqual(tokenBefore, after.ValidationToken);
        }

        [Fact]
        public void Apply_SameThickness_DoesNotTouchTheLabel()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            SpecEditResult result = SpecFieldEditor.Apply(spec, "gusset.thickness_mm", "9.525");
            Assert.True(result.Ok);
            Assert.Null(result.Note);
            Assert.Equal("3/8\"", spec.Gusset!.ThicknessLabel);
        }

        [Fact]
        public void Apply_BoltSpacing_10_GivesBoltSpacingErrorAndNoToken()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            Assert.True(SpecFieldEditor.Apply(spec, "members[2].attachment.bolts.spacing_mm", "10").Ok);

            ValidationResult result = Validate(spec);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.BoltSpacingTooSmall && e.Path!.Contains("members[2]"));
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void Apply_InvalidTexts_FailWithSpanishMessageAndLeaveTheSpecUntouched()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();

            SpecEditResult number = SpecFieldEditor.Apply(spec, "gusset.width_mm", "abc");
            Assert.False(number.Ok);
            Assert.Contains("no se entiende", number.Error);
            Assert.Equal(565.0, spec.Gusset!.WidthMm!.Value, 6);

            SpecEditResult choice = SpecFieldEditor.Apply(spec, "gusset.chord_interface", "pegado");
            Assert.False(choice.Ok);
            Assert.Contains("through_slot", choice.Error);
            Assert.Equal("through_slot", spec.Gusset.ChordInterface);

            SpecEditResult boolean = SpecFieldEditor.Apply(spec, "chord.continuous", "quizá");
            Assert.False(boolean.Ok);

            SpecEditResult integer = SpecFieldEditor.Apply(spec, "members[2].attachment.bolts.rows", "2,5");
            Assert.False(integer.Ok);
            Assert.Equal(2, spec.Members[2].Attachment!.Bolts!.Rows);

            SpecEditResult unknown = SpecFieldEditor.Apply(spec, "gusset.color", "rojo");
            Assert.False(unknown.Ok);
            Assert.Contains("no se edita", unknown.Error);

            SpecEditResult missing = SpecFieldEditor.Apply(spec, "members[7].profile", "HSS");
            Assert.False(missing.Ok);
        }

        [Fact]
        public void Apply_OutlinePoint_DimensionChain_AndUncertainty()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();

            Assert.True(SpecFieldEditor.Apply(spec, "gusset.outline.points_mm[1]", "245; 290").Ok);
            Assert.Equal(245.0, spec.Gusset!.Outline!.PointsMm![1][0], 6);
            Assert.Equal(290.0, spec.Gusset.Outline.PointsMm[1][1], 6);
            Assert.False(SpecFieldEditor.Apply(spec, "gusset.outline.points_mm[1]", "245").Ok);

            Assert.True(SpecFieldEditor.Apply(spec, "dimension_chains[0].values_mm", "75; 402; 70").Ok);
            Assert.Equal(new[] { 75.0, 402.0, 70.0 }, spec.DimensionChains[0].ValuesMm);
            ValidationResult chain = Validate(spec);
            Assert.Contains(chain.Errors, e => e.Code == ErrorCodes.DimensionChainMismatch);

            Assert.True(SpecFieldEditor.Apply(spec, "dimension_chains[0].values_mm", "75; 420; 70").Ok);
            Assert.True(SpecFieldEditor.Apply(spec, "uncertain_fields[0].user_confirmed_value", "").Ok);
            Assert.Null(spec.UncertainFields[0].UserConfirmedValue);
            ValidationResult unresolved = Validate(spec);
            Assert.Contains(unresolved.Errors, e => e.Code == ErrorCodes.UnresolvedUncertainty);
            Assert.Null(unresolved.ValidationToken);

            Assert.True(SpecFieldEditor.Apply(spec, "uncertain_fields[0].user_confirmed_value", "HSS2-1/2X2-1/2X3/16").Ok);
            Assert.Equal("HSS2-1/2X2-1/2X3/16", spec.UncertainFields[0].UserConfirmedValue);
        }

        [Fact]
        public void Apply_AttachmentType_ToBolted_CreatesEmptyPlateAndBoltsRows()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            SpecEditResult result = SpecFieldEditor.Apply(spec, "members[0].attachment.type", "bolted_knife_plate");

            Assert.True(result.Ok);
            Assert.NotNull(result.Note);
            AttachmentSpec attachment = spec.Members[0].Attachment!;
            Assert.Equal("bolted_knife_plate", attachment.Type);
            Assert.NotNull(attachment.Plate);
            Assert.NotNull(attachment.Bolts);
            Assert.Null(attachment.Plate!.ThicknessMm);

            var rows = SpecFieldCatalog.Build(spec);
            Assert.Contains(rows, r => r.Path == "members[0].attachment.plate.thickness_mm" && r.Value == "");
            Assert.DoesNotContain(rows, r => r.Path == "members[0].attachment.slot_length_mm");

            // Hasta rellenar la placa, el validador lo dice campo por campo y no hay token.
            ValidationResult validation = Validate(spec);
            Assert.False(validation.IsValid);
            Assert.Null(validation.ValidationToken);
            Assert.Contains(validation.Errors, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "members[0].attachment.plate.thickness_mm");
            Assert.Contains(validation.Errors, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "members[0].attachment.bolts.spacing_mm");
        }

        [Fact]
        public void Validator_RequiresNestedFieldsUnlessDeclaredUncertain()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            Assert.True(SpecFieldEditor.Apply(spec, "members[0].attachment.slot_length_mm", "").Ok);
            Assert.Null(spec.Members[0].Attachment!.SlotLengthMm);

            ValidationResult missing = Validate(spec);
            Assert.Contains(missing.Errors, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "members[0].attachment.slot_length_mm");

            spec.UncertainFields.Add(new UncertainField { Path = "members[0].attachment.slot_length_mm", Reason = "No se lee en el plano", UserConfirmedValue = null });
            ValidationResult declared = Validate(spec);
            Assert.DoesNotContain(declared.Errors, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "members[0].attachment.slot_length_mm");
            Assert.Contains(declared.Errors, e => e.Code == ErrorCodes.UnresolvedUncertainty);
        }

        [Fact]
        public void Apply_BoltDiameter_UpdatesLabel()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            SpecEditResult result = SpecFieldEditor.Apply(spec, "members[2].attachment.bolts.diameter_mm", "19,05");
            Assert.True(result.Ok);
            Assert.Equal("3/4\"", spec.Members[2].Attachment!.Bolts!.DiameterLabel);
            Assert.Contains("diameter_label", result.Note);
        }

        [Fact]
        public void EditedSpec_RoundTripsThroughIndentedJson()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            Assert.True(SpecFieldEditor.Apply(spec, "gusset.thickness_mm", "12,7").Ok);
            Assert.True(SpecFieldEditor.Apply(spec, "members[1].profile", "HSS2-1/2X2-1/2X3/16").Ok);

            string indented = spec.ToJson(true);
            Assert.Contains("\n", indented);
            Assert.Contains("\"thickness_label\": \"1/2\\\"\"", indented);

            ConnectionSpec again = ConnectionSpec.FromJson(indented)!;
            Assert.Equal(12.7, again.Gusset!.ThicknessMm!.Value, 6);
            Assert.Equal("1/2\"", again.Gusset.ThicknessLabel);
            Assert.Equal(spec.ToJson(), again.ToJson());
        }

        [Theory]
        [InlineData("9,525", 9.525)]
        [InlineData("9.525", 9.525)]
        [InlineData(" 565 ", 565.0)]
        [InlineData("1.234,5", 1234.5)]
        [InlineData("-175", -175.0)]
        public void Parser_AcceptsCommaAndDot(string text, double expected)
        {
            Assert.True(SpecValueParser.TryParseNumber(text, out double value));
            Assert.Equal(expected, value, 6);
        }

        [Fact]
        public void Parser_FormatsBackWithComma()
        {
            Assert.Equal("9,525", SpecValueParser.FormatNumber(9.525));
            Assert.Equal("565", SpecValueParser.FormatNumber(565.0));
            Assert.Equal("", SpecValueParser.FormatNumber(null));
            Assert.Equal("75; 420; 70", SpecValueParser.FormatNumberList(new[] { 75.0, 420.0, 70.0 }));
            Assert.True(SpecValueParser.TryParseBoolean("Sí", out bool yes) && yes);
            Assert.True(SpecValueParser.TryParseBoolean("no", out bool no) && !no);
        }
    }
}
