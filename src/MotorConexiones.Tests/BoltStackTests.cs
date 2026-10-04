using System;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Fase 6b: paquete de pernos de la placa cuchilla (cartela + placa cuchilla en solape sobre la cara +Z) y longitud
    /// de perno por la tabla 7-15 del AISC Manual (config/limits.json).
    /// </summary>
    public class BoltStackTests
    {
        [Fact]
        public void DetalleD_FiveEighthsBolt_GripAndLengthMatchAisc()
        {
            // Cartela 3/8" (9,525) + placa cuchilla PL10: agarre 19,525; 5/8" suma 7/8" (22,225) → 41,75 → 1-3/4" = 44,45.
            var stack = BoltStack.Compute(9.525, 10.0, 15.875, LimitsConfig.Default);

            Assert.Equal(19.525, stack.GripMm, 3);
            Assert.Equal(22.225, stack.LengthAdditionMm, 3);
            Assert.Equal(44.45, stack.BoltLengthMm, 3);
            Assert.Equal(6.35, stack.LengthIncrementMm, 3);
            Assert.True(stack.ProtrusionMm > 0);
        }

        [Fact]
        public void KnifePlate_SitsOnPlusZFaceOfGusset()
        {
            var stack = BoltStack.Compute(9.525, 10.0, 15.875, LimitsConfig.Default);

            // Plano medio de la cuchilla = medio espesor de cartela + medio espesor de cuchilla.
            Assert.Equal(9.7625, stack.KnifePlateZOffsetMm, 4);
            // Cara exterior de la cuchilla (cabeza del perno) y cara −Z de la cartela (tuerca).
            Assert.Equal(14.7625, stack.OuterFaceZMm, 4);
            Assert.Equal(-4.7625, stack.InnerFaceZMm, 4);
            Assert.Equal(stack.GripMm, stack.OuterFaceZMm - stack.InnerFaceZMm, 6);
            Assert.Equal("+z", BoltStack.GussetFace);
        }

        [Theory]
        [InlineData(12.7, 19.525, 17.4625, 38.1)]    // 1/2": 37,0 → 1-1/2"
        [InlineData(19.05, 19.525, 25.4, 50.8)]      // 3/4": 44,9 → 2"
        [InlineData(25.4, 19.525, 31.75, 57.15)]     // 1": 51,3 → 2-1/4"
        public void OtherDiameters_UseTableAndRoundUpToQuarterInch(double diameter, double grip, double expectedAddition, double expectedLength)
        {
            var stack = BoltStack.Compute(9.525, grip - 9.525, diameter, LimitsConfig.Default);
            Assert.Equal(expectedAddition, stack.LengthAdditionMm, 3);
            Assert.Equal(expectedLength, stack.BoltLengthMm, 3);
        }

        [Fact]
        public void RoundUp_IsExactOnMultiplesAndRoundsUpOtherwise()
        {
            Assert.Equal(44.45, BoltStack.RoundUp(44.45, 6.35), 6);
            Assert.Equal(44.45, BoltStack.RoundUp(38.11, 6.35), 6);
            Assert.Equal(50.8, BoltStack.RoundUp(44.46, 6.35), 6);
            Assert.Equal(12.0, BoltStack.RoundUp(12.0, 0.0), 6);
        }

        [Fact]
        public void LengthAddition_InterpolatesBetweenTableRowsAndFallsBackOutside()
        {
            var limits = LimitsConfig.Default;
            // Entre 5/8" (22,225) y 3/4" (25,4): a mitad de camino, 23,8125.
            Assert.Equal(23.8125, limits.GetBoltLengthAddition(17.4625), 3);
            // Fuera de la tabla: 1,4 × d.
            Assert.Equal(1.4 * 40.0, limits.GetBoltLengthAddition(40.0), 3);
        }

        [Fact]
        public void CustomLimits_ChangeTheBoltLengthAndTheHash()
        {
            var custom = LimitsConfig.LoadFromJson("{\"schema_version\":1,\"bolts\":{\"length_addition_mm\":{\"15.875\":30.0},\"length_increment_mm\":5.0}}");
            var stack = BoltStack.Compute(9.525, 10.0, 15.875, custom);

            Assert.Equal(30.0, stack.LengthAdditionMm, 3);
            Assert.Equal(50.0, stack.BoltLengthMm, 3); // 49,525 → 50 (paso 5)
            Assert.NotEqual(LimitsConfig.Default.ComputeHash(), custom.ComputeHash());
        }

        [Fact]
        public void InvalidThicknessOrDiameter_Throws()
        {
            Assert.Throws<ArgumentException>(() => BoltStack.Compute(0.0, 10.0, 15.875, LimitsConfig.Default));
            Assert.Throws<ArgumentException>(() => BoltStack.Compute(9.525, 0.0, 15.875, LimitsConfig.Default));
            Assert.Throws<ArgumentException>(() => BoltStack.Compute(9.525, 10.0, 0.0, LimitsConfig.Default));
        }
    }
}
