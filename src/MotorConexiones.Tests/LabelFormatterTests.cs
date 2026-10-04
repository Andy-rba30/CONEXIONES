using MotorConexiones.Core.Validation;
using Xunit;

namespace MotorConexiones.Tests
{
    public class LabelFormatterTests
    {
        [Theory]
        [InlineData(9.525, "3/8\"")]
        [InlineData(12.7, "1/2\"")]
        [InlineData(15.875, "5/8\"")]
        [InlineData(19.05, "3/4\"")]
        [InlineData(25.4, "1\"")]
        [InlineData(38.1, "1-1/2\"")]
        [InlineData(4.7625, "3/16\"")]
        public void TryInchFraction_RecognisesExactFractions(double mm, string expected)
        {
            Assert.True(LabelFormatter.TryInchFraction(mm, out string label));
            Assert.Equal(expected, label);
        }

        [Theory]
        [InlineData(10.0)]
        [InlineData(12.5)]
        [InlineData(20.0)]
        [InlineData(0.0)]
        [InlineData(-5.0)]
        public void TryInchFraction_RejectsMetricValues(double mm)
        {
            Assert.False(LabelFormatter.TryInchFraction(mm, out _));
        }

        [Theory]
        [InlineData(12.7, "3/8\"", "1/2\"")]
        [InlineData(12.7, "PL10", "PL 1/2\"")]
        [InlineData(10.0, "3/8\"", "PL10")]
        [InlineData(12.5, "PL10", "PL12.5")]
        [InlineData(9.525, null, "3/8\"")]
        public void Thickness_KeepsTheStyleOfThePreviousLabel(double mm, string? previous, string expected)
        {
            Assert.Equal(expected, LabelFormatter.Thickness(mm, previous));
        }

        [Theory]
        [InlineData(15.875, "5/8\"")]
        [InlineData(19.05, "3/4\"")]
        [InlineData(20.0, "20 mm")]
        [InlineData(22.5, "22.5 mm")]
        public void Diameter_IsInchOrMillimetres(double mm, string expected)
        {
            Assert.Equal(expected, LabelFormatter.Diameter(mm));
        }

        [Theory]
        [InlineData(9.525)]
        [InlineData(12.7)]
        [InlineData(10.0)]
        [InlineData(12.5)]
        [InlineData(15.875)]
        [InlineData(20.0)]
        [InlineData(38.1)]
        public void GeneratedLabels_AreReadBackByLabelParser(double mm)
        {
            string thickness = LabelFormatter.Thickness(mm, null);
            Assert.True(LabelParser.TryParseLabelToMm(thickness, out double parsedThickness), thickness);
            Assert.InRange(parsedThickness, mm - LabelFormatter.InchToleranceMm, mm + LabelFormatter.InchToleranceMm);

            string diameter = LabelFormatter.Diameter(mm);
            Assert.True(LabelParser.TryParseLabelToMm(diameter, out double parsedDiameter), diameter);
            Assert.InRange(parsedDiameter, mm - LabelFormatter.InchToleranceMm, mm + LabelFormatter.InchToleranceMm);
        }
    }
}
