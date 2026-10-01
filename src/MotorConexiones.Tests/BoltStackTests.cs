using System.IO;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Ronda 6b: los pernos de la placa cuchilla atraviesan cartela + placa (agarre real) y su longitud sale del agarre más
    /// el suplemento de <c>config/limits.json</c>, o de <c>bolts.length_mm</c> si el plano la trae. Antes la placa se creaba
    /// en el plano de la cartela y los pernos medían 45 mm fijos (capturas fase6-05 y fase5-03).
    /// </summary>
    public class BoltStackTests
    {
        private static KnifePlateSpec DetalleDPlate(string? face = null) =>
            new KnifePlateSpec { ThicknessMm = 10.0, ThicknessLabel = "PL10", LengthMm = 170, WidthMm = 140, InsertionMm = 80, GussetFace = face };

        private static BoltPatternSpec DetalleDBolts(double? lengthMm = null) =>
            new BoltPatternSpec { DiameterMm = 15.875, DiameterLabel = "5/8\"", Rows = 2, Columns = 2, SpacingMm = 60, EdgeMm = 40, FirstRowFromPlateEndMm = 40, LengthMm = lengthMm };

        private static ValidationResult Validate(string json) =>
            SpecValidator.Validate(json, ConnectionSpec.FromJson(json), new FakeModelFacts(), LimitsConfig.Default);

        [Fact]
        public void DetalleD_GripIsGussetPlusPlate_AndLengthComesFromTheTable()
        {
            BoltStack stack = BoltStack.Compute(9.525, DetalleDPlate(), DetalleDBolts(), LimitsConfig.Default);

            Assert.Equal(19.525, stack.GripMm, 6);
            Assert.Equal(1, stack.Side);
            Assert.Equal("+z", stack.FaceLabel);
            // La placa apoya en la cara +Z de la cartela: su plano medio queda a (9,525 + 10)/2 del plano de la cercha.
            Assert.Equal(9.7625, stack.PlateOffsetMm, 6);
            Assert.Equal(-4.7625, stack.StackMinMm, 6);
            Assert.Equal(14.7625, stack.StackMaxMm, 6);
            Assert.Equal(stack.GripMm, stack.StackMaxMm - stack.StackMinMm, 6);
            // Ø5/8": 19,525 + 22,23 = 41,755 → siguiente múltiplo de 1/4" = 44,45 mm (1 3/4"). Ya no son 45 mm fijos.
            Assert.Equal(22.23, stack.LengthAdditionMm, 6);
            Assert.Equal(44.45, stack.BoltLengthMm, 6);
            Assert.False(stack.LengthFromSpec);
            Assert.True(stack.BoltLengthMm >= stack.MinimumLengthMm);
        }

        [Fact]
        public void NegativeFace_PutsThePlateOnTheOtherSide()
        {
            BoltStack stack = BoltStack.Compute(9.525, DetalleDPlate("-z"), DetalleDBolts(), LimitsConfig.Default);

            Assert.Equal(-1, stack.Side);
            Assert.Equal("-z", stack.FaceLabel);
            Assert.Equal(-9.7625, stack.PlateOffsetMm, 6);
            Assert.Equal(-14.7625, stack.StackMinMm, 6);
            Assert.Equal(4.7625, stack.StackMaxMm, 6);
            Assert.Equal(19.525, stack.GripMm, 6);
        }

        [Fact]
        public void LengthFromTheDrawing_IsUsedAsIs()
        {
            BoltStack stack = BoltStack.Compute(9.525, DetalleDPlate(), DetalleDBolts(50.8), LimitsConfig.Default);
            Assert.Equal(50.8, stack.BoltLengthMm, 6);
            Assert.True(stack.LengthFromSpec);
        }

        [Theory]
        [InlineData("+z", 1, true)]
        [InlineData("-z", -1, true)]
        [InlineData("", 1, true)]
        [InlineData(null, 1, true)]
        [InlineData(" -Z ", -1, true)]
        [InlineData("lado", 1, false)]
        public void ParseSide_ReadsTheContractValues(string? face, int expectedSide, bool expectedRecognized)
        {
            int side = BoltStack.ParseSide(face, out bool recognized);
            Assert.Equal(expectedSide, side);
            Assert.Equal(expectedRecognized, recognized);
        }

        [Fact]
        public void Limits_BoltLength_RoundsUpToQuarterInch_AndFallsBackToFactor()
        {
            LimitsConfig limits = LimitsConfig.Default;
            Assert.Equal(44.45, limits.ComputeBoltLengthMm(19.525, 15.875), 6);
            // 25 + 22,23 = 47,23 → 50,8 (2").
            Assert.Equal(50.8, limits.ComputeBoltLengthMm(25.0, 15.875), 6);
            // Diámetro fuera de la tabla (M20): 1,4 × 20 = 28 de suplemento; 30 + 28 = 58 → 63,5.
            Assert.Equal(28.0, limits.GetBoltLengthAdditionMm(20.0), 6);
            Assert.Equal(63.5, limits.ComputeBoltLengthMm(30.0, 20.0), 6);
            Assert.Equal(17.46, limits.GetBoltLengthAdditionMm(12.7), 6);

            // El hash de los límites (parte del token) cambia si se edita la tabla de longitudes.
            string before = limits.ComputeHash();
            var edited = LimitsConfig.LoadFromJson("{\"bolts\": {\"length_increment_mm\": 5.0}}");
            Assert.Equal(5.0, edited.Bolts.LengthIncrementMm);
            Assert.NotEqual(before, edited.ComputeHash());
            // Un limits.json sin la tabla (anterior a la ronda 6b) sigue funcionando con los valores por defecto.
            var legacy = LimitsConfig.LoadFromJson("{\"bolts\": {\"min_spacing_factor\": 2.667}}");
            Assert.Equal(44.45, legacy.ComputeBoltLengthMm(19.525, 15.875), 6);
        }

        [Fact]
        public void RepoLimitsJson_CarriesTheLengthTable()
        {
            var limits = LimitsConfig.LoadFromFile(SketchBuilderTests.FindRepoFile(Path.Combine("config", "limits.json")));
            Assert.Equal(22.23, limits.Bolts.LengthAdditionMm["15.875"], 6);
            Assert.Equal(6.35, limits.Bolts.LengthIncrementMm, 6);
            Assert.Equal(44.45, limits.ComputeBoltLengthMm(19.525, 15.875), 6);
        }

        [Fact]
        public void GussetFace_IsAcceptedByTheSchema_AndOtherTextIsRejected()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.plate.gusset_face", "-z", out string withFace, out string error), error);
            ValidationResult ok = Validate(withFace);
            Assert.DoesNotContain(ok.Errors, e => e.Code == ErrorCodes.SchemaInvalid);
            Assert.Equal("-z", ConnectionSpec.FromJson(withFace)!.Members[2].Attachment!.Plate!.GussetFace);
            Assert.NotNull(ok.ValidationToken);

            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.plate.gusset_face", "lado", out string wrong, out error), error);
            ValidationResult bad = Validate(wrong);
            Assert.Contains(bad.Errors, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "members[2].attachment.plate.gusset_face");
            Assert.Null(bad.ValidationToken);

            // La tabla de la ventana ofrece los dos campos nuevos.
            var fields = SpecEditor.ListFields(ConnectionSpec.FromJson(withFace)!);
            Assert.Equal("-z", fields.Single(f => f.Path == "members[2].attachment.plate.gusset_face").Value);
            Assert.Equal("", fields.Single(f => f.Path == "members[2].attachment.bolts.length_mm").Value);
        }

        [Fact]
        public void ShortBoltLength_IsAWarning_NotAnError()
        {
            var (json, _) = SketchBuilderTests.LoadConfirmedFixture();
            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.bolts.length_mm", "30", out string shortBolt, out string error), error);
            ValidationResult warned = Validate(shortBolt);
            ApiError warning = Assert.Single(warned.Warnings, w => w.Code == ErrorCodes.BoltLengthTooShort);
            Assert.Equal("members[2].attachment.bolts.length_mm", warning.Path);
            Assert.Contains("19.5", warning.Message);
            Assert.True(warned.IsValid);
            Assert.NotNull(warned.ValidationToken);

            Assert.True(SpecEditor.TrySetValue(json, "members[2].attachment.bolts.length_mm", "50", out string longBolt, out error), error);
            Assert.DoesNotContain(Validate(longBolt).Warnings, w => w.Code == ErrorCodes.BoltLengthTooShort);

            // Sin length_mm no hay nada que avisar: se calcula del agarre.
            Assert.DoesNotContain(Validate(json).Warnings, w => w.Code == ErrorCodes.BoltLengthTooShort);
        }
    }
}
