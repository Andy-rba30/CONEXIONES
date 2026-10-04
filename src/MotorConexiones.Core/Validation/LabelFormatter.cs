using System;
using System.Globalization;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Escribe etiquetas de plano a partir de un valor en mm, con el formato inverso al que lee <see cref="LabelParser"/>:
    /// fracción de pulgada (<c>3/8"</c>, <c>1-1/2"</c>) si el valor es una fracción exacta de 1/64" con tolerancia
    /// de 0,05 mm, y si no, métrico (<c>PL12.5</c> para espesores, <c>20 mm</c> para diámetros). La ventana lo usa
    /// para mantener <c>thickness_label</c> y <c>diameter_label</c> coherentes cuando se edita el número.
    /// </summary>
    public static class LabelFormatter
    {
        /// <summary>Tolerancia con la que un valor en mm se considera una fracción de pulgada (la misma que LABEL_VALUE_MISMATCH).</summary>
        public const double InchToleranceMm = 0.05;

        /// <summary>Intenta expresar el valor como fracción de pulgada en sesentaicuatroavos, reducida.</summary>
        public static bool TryInchFraction(double valueMm, out string label)
        {
            label = "";
            if (valueMm <= 0 || double.IsNaN(valueMm) || double.IsInfinity(valueMm)) return false;

            double inches = UnitConverter.MmToInches(valueMm);
            long sixtyFourths = (long)Math.Round(inches * 64.0, MidpointRounding.AwayFromZero);
            if (sixtyFourths <= 0) return false;
            double candidateMm = UnitConverter.InchesToMm(sixtyFourths / 64.0);
            if (Math.Abs(candidateMm - valueMm) > InchToleranceMm) return false;

            long whole = sixtyFourths / 64;
            long numerator = sixtyFourths % 64;
            long denominator = 64;
            while (numerator != 0 && numerator % 2 == 0)
            {
                numerator /= 2;
                denominator /= 2;
            }

            if (numerator == 0)
            {
                label = whole.ToString(CultureInfo.InvariantCulture) + "\"";
            }
            else if (whole > 0)
            {
                label = whole.ToString(CultureInfo.InvariantCulture) + "-" + numerator.ToString(CultureInfo.InvariantCulture) + "/" + denominator.ToString(CultureInfo.InvariantCulture) + "\"";
            }
            else
            {
                label = numerator.ToString(CultureInfo.InvariantCulture) + "/" + denominator.ToString(CultureInfo.InvariantCulture) + "\"";
            }
            return true;
        }

        /// <summary>
        /// Etiqueta de espesor de placa: fracción de pulgada si lo es (conservando el prefijo <c>PL</c> si la etiqueta
        /// anterior lo llevaba), o <c>PL&lt;mm&gt;</c> (<c>PL10</c>, <c>PL12.5</c>).
        /// </summary>
        public static string Thickness(double valueMm, string? previousLabel)
        {
            bool hadPlPrefix = !string.IsNullOrWhiteSpace(previousLabel) && previousLabel!.Trim().StartsWith("PL", StringComparison.OrdinalIgnoreCase);
            if (TryInchFraction(valueMm, out string fraction))
            {
                return hadPlPrefix ? "PL " + fraction : fraction;
            }
            return "PL" + valueMm.ToString("0.##", CultureInfo.InvariantCulture);
        }

        /// <summary>Etiqueta de diámetro de perno: fracción de pulgada (<c>5/8"</c>) o <c>&lt;mm&gt; mm</c> (<c>20 mm</c>).</summary>
        public static string Diameter(double valueMm)
        {
            if (TryInchFraction(valueMm, out string fraction)) return fraction;
            return valueMm.ToString("0.##", CultureInfo.InvariantCulture) + " mm";
        }
    }
}
