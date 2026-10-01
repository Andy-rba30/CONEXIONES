using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Analiza rótulos de planos (3/8", PL10, 5/8", etc.) y valida que coincidan con los valores numéricos en mm.
    /// Toda conversión de pulgadas a mm se realiza estrictamente a través de UnitConverter.
    /// </summary>
    public static class LabelParser
    {
        private static readonly Regex FractionWithInchesRegex = new Regex(
            @"^(?:PL\s*)?(?:(\d+)\s*[- ]\s*)?(\d+)\s*/\s*(\d+)\s*(?:""|in)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WholeInchesRegex = new Regex(
            @"^(?:PL\s*)?(\d+(?:\.\d+)?)\s*(?:""|in)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MetricPlateRegex = new Regex(
            @"^(?:PL\s*)?(\d+(?:\.\d+)?)\s*(?:mm)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Intenta interpretar un rótulo de dimensión o espesor y convertirlo a milímetros.
        /// </summary>
        public static bool TryParseLabelToMm(string? label, out double valueMm)
        {
            valueMm = 0.0;
            if (string.IsNullOrWhiteSpace(label))
                return false;

            string trimmed = label!.Trim();

            // 1. Fracción de pulgada: "3/8\"", "PL 3/8\"", "1-1/2\"", "5/8"
            var matchFrac = FractionWithInchesRegex.Match(trimmed);
            if (matchFrac.Success)
            {
                double whole = 0.0;
                if (matchFrac.Groups[1].Success && !string.IsNullOrEmpty(matchFrac.Groups[1].Value))
                {
                    whole = double.Parse(matchFrac.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                double num = double.Parse(matchFrac.Groups[2].Value, CultureInfo.InvariantCulture);
                double den = double.Parse(matchFrac.Groups[3].Value, CultureInfo.InvariantCulture);
                if (den == 0) return false;

                double totalInches = whole + (num / den);
                valueMm = UnitConverter.InchesToMm(totalInches);
                return true;
            }

            // 2. Pulgadas enteras o decimales con comilla: "1\"", "2.5\""
            var matchWholeInches = WholeInchesRegex.Match(trimmed);
            if (matchWholeInches.Success)
            {
                double inches = double.Parse(matchWholeInches.Groups[1].Value, CultureInfo.InvariantCulture);
                valueMm = UnitConverter.InchesToMm(inches);
                return true;
            }

            // 3. Notación métrica: "PL10", "PL 10", "10 mm", "10"
            var matchMetric = MetricPlateRegex.Match(trimmed);
            if (matchMetric.Success)
            {
                valueMm = double.Parse(matchMetric.Groups[1].Value, CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Comprueba si el rótulo coincide con el valor numérico en milímetros dentro de la tolerancia configurada.
        /// </summary>
        public static bool MatchesNumericValue(string? label, double numericValueMm, double toleranceMm, out double parsedMm)
        {
            if (TryParseLabelToMm(label, out parsedMm))
            {
                return Math.Abs(parsedMm - numericValueMm) <= toleranceMm;
            }

            // Si el rótulo no es una dimensión conocida, no se puede contrastar
            return true;
        }
    }
}
