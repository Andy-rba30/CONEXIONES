using MotorConexiones.Core.Units;
using Xunit;

namespace MotorConexiones.Tests
{
    public class UnitConverterTests
    {
        [Theory]
        [InlineData(0.0)]
        [InlineData(1.0)]
        [InlineData(9.525)]
        [InlineData(565.0)]
        [InlineData(12345.678)]
        [InlineData(-250.0)]
        public void MmToFeetAndBack_IsReversibleBelowOneMicron(double mm)
        {
            double feet = UnitConverter.MmToFeet(mm);
            double back = UnitConverter.FeetToMm(feet);
            Assert.True(System.Math.Abs(back - mm) < 0.001, $"{mm} mm -> {feet} ft -> {back} mm");
        }

        [Fact]
        public void OneFootIs304Point8Millimeters()
        {
            Assert.Equal(304.8, UnitConverter.FeetToMm(1.0), 10);
            Assert.Equal(1.0, UnitConverter.MmToFeet(304.8), 10);
        }

        [Fact]
        public void DegreesAndRadiansAreReversible()
        {
            Assert.Equal(System.Math.PI / 2, UnitConverter.DegreesToRadians(90), 12);
            Assert.Equal(45.0, UnitConverter.RadiansToDegrees(UnitConverter.DegreesToRadians(45.0)), 12);
        }

        [Fact]
        public void RoundMm_RoundsToTenthOfMillimeter()
        {
            Assert.Equal(9.5, UnitConverter.RoundMm(9.525));
            Assert.Equal(9.6, UnitConverter.RoundMm(9.55));
            Assert.Equal(-0.1, UnitConverter.RoundMm(-0.05));
        }
    }
}
