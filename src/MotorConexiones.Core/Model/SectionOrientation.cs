using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Model
{
    /// <summary>
    /// Ronda 12c: la designación AISC de un tubo HSS leída en mm. <c>HSS12X8X1/2</c> es <b>canto</b> (el primer número: el
    /// "Ht" o "d" de la familia de Revit) × <b>ancho</b> (el segundo: "b") × pared; en un tubo cuadrado los dos primeros
    /// coinciden. Se usa cuando el tipo de Revit no trae los parámetros de sección: el sondeo 27 (ronda 12b) vio que el
    /// HSS12X8X1/2 del Hangar guarda "Ht" = 304,8, "b" = 203,2 y "t" = 11,81 con nombre corto y ningún
    /// <c>STRUCTURAL_SECTION_COMMON_*</c>, y que el add-in, al no encontrar el canto, lo infería del "8" de la designación.
    /// </summary>
    public static class HssDesignation
    {
        private static readonly Regex MetricRegex = new Regex(@"(\d+(?:\.\d+)?)\s*[xX]\s*(\d+(?:\.\d+)?)(?:\s*[xX]\s*(\d+(?:\.\d+)?))?\s*\]?\s*$", RegexOptions.Compiled);

        /// <summary>Canto, ancho y pared en mm de una designación HSS (fracciones AISC o, si no las hay, el sufijo métrico "64x64").</summary>
        public static bool TryParseMm(string? typeName, out double depthMm, out double widthMm, out double thicknessMm)
        {
            depthMm = 0.0;
            widthMm = 0.0;
            thicknessMm = 0.0;
            if (string.IsNullOrWhiteSpace(typeName)) return false;
            if (ProfileMatcher.TryParseHssDimensions(typeName!, out double d, out double w, out double t) && d > 0 && w > 0)
            {
                depthMm = UnitConverter.InchesToMm(d);
                widthMm = UnitConverter.InchesToMm(w);
                thicknessMm = t > 0 ? UnitConverter.InchesToMm(t) : 0.0;
                return true;
            }
            if (typeName!.IndexOf("HSS", StringComparison.OrdinalIgnoreCase) < 0) return false;
            Match metric = MetricRegex.Match(typeName);
            if (!metric.Success) return false;
            if (!double.TryParse(metric.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out depthMm) ||
                !double.TryParse(metric.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out widthMm) || depthMm <= 0 || widthMm <= 0)
            {
                depthMm = 0.0;
                widthMm = 0.0;
                return false;
            }
            if (metric.Groups[3].Success) double.TryParse(metric.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out thicknessMm);
            return true;
        }
    }

    /// <summary>
    /// Ronda 12c: qué lado de la sección queda <b>en</b> el plano de la cercha y cuál <b>perpendicular</b> a él. Revit pone
    /// el canto nominal ("Ht", "d") en el plano vertical de la barra cuando <i>Cross-Section Rotation</i>
    /// (<c>STRUCTURAL_BEND_DIR_ANGLE</c>) vale 0 y lo gira alrededor del eje con ese ángulo: a ±90° el canto pasa a ser
    /// perpendicular al plano y el ancho ("b") queda en el plano; a 0° y ±180° no cambia nada. Es lo que midió el sondeo
    /// 27 en el cordón superior del Hangar (HSS12X8X1/2 girado 90°: en el plano 203,2, perpendicular 304,8). Vale para
    /// cerchas en un plano vertical, que son las de este proyecto; un ángulo que no sea múltiplo de 90° se redondea al
    /// más cercano. El resultado es lo que <see cref="MemberModelFacts.WidthMm"/> (en el plano) y
    /// <see cref="MemberModelFacts.HeightMm"/> (perpendicular) prometen.
    /// </summary>
    public static class SectionOrientation
    {
        /// <summary>(lado en el plano de la cercha, lado perpendicular al plano) a partir del canto y el ancho nominales y el giro.</summary>
        public static (double InPlaneMm, double OutOfPlaneMm) Oriented(double nominalDepthMm, double nominalWidthMm, double rotationDeg)
        {
            return IsRotated(rotationDeg) ? (nominalWidthMm, nominalDepthMm) : (nominalDepthMm, nominalWidthMm);
        }

        /// <summary>True cuando el giro deja el canto nominal perpendicular al plano (más cerca de ±90° que de 0° o 180°).</summary>
        public static bool IsRotated(double rotationDeg)
        {
            if (double.IsNaN(rotationDeg) || double.IsInfinity(rotationDeg)) return false;
            double rad = rotationDeg * Math.PI / 180.0;
            return Math.Abs(Math.Sin(rad)) > Math.Abs(Math.Cos(rad));
        }
    }
}
