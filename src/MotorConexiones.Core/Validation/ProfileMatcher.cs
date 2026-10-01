using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Compara designaciones de perfiles AISC normalizadas y dimensiones de sección,
    /// y genera sugerencias de los perfiles más cercanos si no coinciden.
    /// </summary>
    public static class ProfileMatcher
    {
        private static readonly Regex MetricSuffixRegex = new Regex(
            @"\s*\[?\d+\s*[xX]\s*\d+(?:\s*[xX]\s*\d+)?\]?$",
            RegexOptions.Compiled);

        /// <summary>
        /// Comprueba si el nombre de perfil especificado en el contrato coincide con el tipo del modelo de Revit.
        /// Ejemplo: "HSS2-1/2X2-1/2X3/16" coincide con "HSS2-1-2X2-1-2X3-16 64x64".
        /// </summary>
        public static bool Matches(string? specProfile, string? modelTypeName, double? modelWidthMm = null, double? modelHeightMm = null, double? modelThicknessMm = null)
        {
            if (string.IsNullOrWhiteSpace(specProfile) || string.IsNullOrWhiteSpace(modelTypeName))
                return false;

            // 1. Coincidencia exacta tras quitar espacios y mayúsculas
            string normSpec = NormalizeDesignation(specProfile!);
            string normModel = NormalizeDesignation(modelTypeName!);

            if (string.Equals(normSpec, normModel, StringComparison.OrdinalIgnoreCase))
                return true;

            // 2. Coincidencia por componentes normalizados
            if (TryParseHssDimensions(specProfile!, out double sW, out double sH, out double sT) &&
                TryParseHssDimensions(modelTypeName!, out double mW, out double mH, out double mT))
            {
                if (Math.Abs(sW - mW) <= 0.01 && Math.Abs(sH - mH) <= 0.01 && Math.Abs(sT - mT) <= 0.01)
                    return true;
            }

            // 3. Comparación contra dimensiones reales del modelo si se aportan
            if (modelWidthMm.HasValue && modelHeightMm.HasValue && modelThicknessMm.HasValue &&
                TryParseHssDimensions(specProfile!, out double specWInches, out double specHInches, out double specTInches))
            {
                double specWMm = UnitConverter.InchesToMm(specWInches);
                double specHMm = UnitConverter.InchesToMm(specHInches);
                double specTMm = UnitConverter.InchesToMm(specTInches);

                if (Math.Abs(specWMm - modelWidthMm.Value) <= 1.5 &&
                    Math.Abs(specHMm - modelHeightMm.Value) <= 1.5 &&
                    Math.Abs(specTMm - modelThicknessMm.Value) <= 0.5)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Normaliza la designación convirtiendo guiones en barras para fracciones (p.ej. 2-1-2 -> 2-1/2, 3-16 -> 3/16),
        /// eliminando etiquetas métricas adicionales y espacios.
        /// </summary>
        public static string NormalizeDesignation(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            // Eliminar sufijo métrico como " 64x64" o "[76x76]"
            string cleaned = MetricSuffixRegex.Replace(raw.Trim(), "");

            // Quitar espacios
            cleaned = cleaned.Replace(" ", "").ToUpperInvariant();

            // Reemplazar patrones de fracciones con guión por barras estándar:
            // "2-1-2" -> "2-1/2"
            cleaned = Regex.Replace(cleaned, @"(\d+)-(\d+)-(\d+)", "$1-$2/$3");
            // "3-16" -> "3/16"
            cleaned = Regex.Replace(cleaned, @"([X\-])(\d+)-(\d+)", "$1$2/$3");

            return cleaned;
        }

        /// <summary>
        /// Extrae ancho, alto y espesor en pulgadas de una designación tipo HSS (p.ej. HSS3X3X1/4 o HSS2-1-2X2-1-2X3-16).
        /// </summary>
        public static bool TryParseHssDimensions(string designation, out double widthInches, out double heightInches, out double thicknessInches)
        {
            widthInches = 0;
            heightInches = 0;
            thicknessInches = 0;

            if (string.IsNullOrWhiteSpace(designation)) return false;

            string normalized = NormalizeDesignation(designation);
            var match = Regex.Match(normalized, @"^HSS(?:\s*ROUND\s*)?([0-9\.\-/]+)X([0-9\.\-/]+)X([0-9\.\-/]+)$");
            if (!match.Success)
            {
                // Intentar sin prefijo HSS
                match = Regex.Match(normalized, @"([0-9\.\-/]+)X([0-9\.\-/]+)X([0-9\.\-/]+)$");
                if (!match.Success) return false;
            }

            if (TryParseFraction(match.Groups[1].Value, out widthInches) &&
                TryParseFraction(match.Groups[2].Value, out heightInches) &&
                TryParseFraction(match.Groups[3].Value, out thicknessInches))
            {
                return true;
            }

            return false;
        }

        private static bool TryParseFraction(string text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            // Caso "2-1/2" o "2 1/2"
            var parts = text.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double whole))
                {
                    if (TryParseSimpleFraction(parts[1], out double frac))
                    {
                        value = whole + frac;
                        return true;
                    }
                }
                return false;
            }

            if (parts.Length == 1)
            {
                if (parts[0].Contains("/"))
                {
                    return TryParseSimpleFraction(parts[0], out value);
                }
                return double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            }

            return false;
        }

        private static bool TryParseSimpleFraction(string text, out double value)
        {
            value = 0;
            var fracParts = text.Split('/');
            if (fracParts.Length == 2 &&
                double.TryParse(fracParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double num) &&
                double.TryParse(fracParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double den) &&
                den != 0)
            {
                value = num / den;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtiene hasta 3 sugerencias de perfiles cargados más similares a la designación buscada.
        /// </summary>
        public static IReadOnlyList<string> GetSuggestions(string? requestedProfile, IEnumerable<string>? availableProfiles, int count = 3)
        {
            if (string.IsNullOrWhiteSpace(requestedProfile) || availableProfiles == null)
                return Array.Empty<string>();

            string normRequested = NormalizeDesignation(requestedProfile!);

            var scored = new List<(string Name, int Distance)>();
            foreach (var profile in availableProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile)) continue;
                string normCandidate = NormalizeDesignation(profile);
                int dist = ComputeLevenshteinDistance(normRequested, normCandidate);
                scored.Add((profile, dist));
            }

            return scored
                .OrderBy(s => s.Distance)
                .ThenBy(s => s.Name, StringComparer.Ordinal)
                .Take(count)
                .Select(s => s.Name)
                .ToList();
        }

        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
