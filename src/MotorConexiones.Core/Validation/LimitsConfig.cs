using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Configuración de límites y tolerancias (AISC 360 y geométricas) leídas desde config/limits.json.
    /// Editable sin recompilar el add-in.
    /// </summary>
    public sealed class LimitsConfig
    {
        [JsonPropertyName("schema_version")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("dimension_chain_tolerance_mm")]
        public double DimensionChainToleranceMm { get; set; } = 1.0;

        [JsonPropertyName("label_value_tolerance_mm")]
        public double LabelValueToleranceMm { get; set; } = 0.05;

        [JsonPropertyName("angle_tolerance_deg")]
        public double AngleToleranceDeg { get; set; } = 1.0;

        [JsonPropertyName("node_axis_max_distance_mm")]
        public double NodeAxisMaxDistanceMm { get; set; } = 5.0;

        [JsonPropertyName("bolts")]
        public BoltLimits Bolts { get; set; } = new BoltLimits();

        [JsonPropertyName("welds")]
        public WeldLimits Welds { get; set; } = new WeldLimits();

        public sealed class BoltLimits
        {
            [JsonPropertyName("min_spacing_factor")]
            public double MinSpacingFactor { get; set; } = 2.667;

            [JsonPropertyName("edge_distance_mm")]
            public Dictionary<string, double> EdgeDistanceMm { get; set; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// Lo que se suma al agarre (espesores que atraviesa el perno) para obtener la longitud del perno, por diámetro
            /// en mm: tuerca, arandela y rosca sobrante (RCSC, tabla C-2.1, pernos A325/A490 de cabeza hexagonal pesada).
            /// Clave "default" = factor sobre el diámetro cuando el diámetro no está en la tabla. Ronda 6b.
            /// </summary>
            [JsonPropertyName("length_addition_mm")]
            public Dictionary<string, double> LengthAdditionMm { get; set; } = DefaultLengthAdditions();

            /// <summary>Las longitudes de perno se redondean hacia arriba a múltiplos de este valor (1/4" = 6,35 mm).</summary>
            [JsonPropertyName("length_increment_mm")]
            public double LengthIncrementMm { get; set; } = 6.35;

            internal static Dictionary<string, double> DefaultLengthAdditions() => new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["12.7"] = 17.46,     // 1/2"  + 11/16"
                ["15.875"] = 22.23,   // 5/8"  + 7/8"
                ["19.05"] = 25.4,     // 3/4"  + 1"
                ["22.225"] = 28.58,   // 7/8"  + 1 1/8"
                ["25.4"] = 31.75,     // 1"    + 1 1/4"
                ["28.575"] = 38.1,    // 1 1/8" + 1 1/2"
                ["31.75"] = 41.28,    // 1 1/4" + 1 5/8"
                ["default"] = 1.4,    // factor sobre el diámetro para diámetros fuera de la tabla (métricos)
            };
        }

        public sealed class WeldLimits
        {
            [JsonPropertyName("min_fillet_mm")]
            public Dictionary<string, double> MinFilletMm { get; set; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Instancia con valores por defecto basados en AISC 360-16 / 360-22.</summary>
        public static LimitsConfig Default { get; } = CreateDefault();

        private static LimitsConfig CreateDefault()
        {
            var config = new LimitsConfig
            {
                SchemaVersion = 1,
                DimensionChainToleranceMm = 1.0,
                LabelValueToleranceMm = 0.05,
                AngleToleranceDeg = 1.0,
                NodeAxisMaxDistanceMm = 5.0,
                Bolts = new BoltLimits
                {
                    MinSpacingFactor = 2.667,
                    EdgeDistanceMm = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["12.7"] = 19.0,
                        ["15.875"] = 22.0,
                        ["19.05"] = 25.0,
                        ["22.225"] = 28.0,
                        ["25.4"] = 32.0,
                        ["28.575"] = 38.0,
                        ["31.75"] = 42.0
                    },
                    LengthAdditionMm = BoltLimits.DefaultLengthAdditions(),
                    LengthIncrementMm = 6.35
                },
                Welds = new WeldLimits
                {
                    MinFilletMm = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["6.0"] = 3.0,
                        ["13.0"] = 5.0,
                        ["19.0"] = 6.0,
                        ["default"] = 8.0
                    }
                }
            };
            return config;
        }

        public static LimitsConfig LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Default;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var parsed = JsonSerializer.Deserialize<LimitsConfig>(json, options);
            return parsed ?? Default;
        }

        public static LimitsConfig LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return Default;

            try
            {
                string json = File.ReadAllText(filePath);
                return LoadFromJson(json);
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// Obtiene la distancia mínima al borde según AISC 360 Tabla J3.4 para un diámetro dado en mm.
        /// </summary>
        public double GetMinBoltEdgeDistance(double diameterMm)
        {
            if (Bolts.EdgeDistanceMm != null && Bolts.EdgeDistanceMm.Count > 0)
            {
                // Buscar coincidencia exacta o cercana dentro de 0.5 mm
                double bestDiff = double.MaxValue;
                double bestVal = 0.0;
                bool found = false;

                foreach (var kvp in Bolts.EdgeDistanceMm)
                {
                    if (double.TryParse(kvp.Key, NumberStyles.Float, CultureInfo.InvariantCulture, out double tableDia))
                    {
                        double diff = Math.Abs(tableDia - diameterMm);
                        if (diff < bestDiff && diff <= 0.5)
                        {
                            bestDiff = diff;
                            bestVal = kvp.Value;
                            found = true;
                        }
                    }
                }

                if (found)
                    return bestVal;
            }

            // Regla general AISC: para diámetros mayores a 1-1/4" (31.75 mm), 1.25 * d
            if (diameterMm > 31.75)
                return 1.25 * diameterMm;

            // Fallback por defecto según tabla J3.4
            if (diameterMm <= 13.0) return 19.0;
            if (diameterMm <= 16.0) return 22.0;
            if (diameterMm <= 20.0) return 25.0;
            if (diameterMm <= 23.0) return 28.0;
            if (diameterMm <= 26.0) return 32.0;
            if (diameterMm <= 29.0) return 38.0;
            return 42.0;
        }

        /// <summary>
        /// Cuánto se suma al agarre para obtener la longitud del perno (tuerca + arandela + rosca sobrante), según la
        /// tabla <c>bolts.length_addition_mm</c>; fuera de la tabla, <c>default</c> × diámetro (1,4·d si no hay default).
        /// </summary>
        public double GetBoltLengthAdditionMm(double diameterMm)
        {
            double factor = 1.4;
            if (Bolts.LengthAdditionMm != null && Bolts.LengthAdditionMm.Count > 0)
            {
                double bestDiff = double.MaxValue;
                double bestVal = 0.0;
                bool found = false;
                foreach (var kvp in Bolts.LengthAdditionMm)
                {
                    if (kvp.Key.Equals("default", StringComparison.OrdinalIgnoreCase))
                    {
                        factor = kvp.Value;
                        continue;
                    }
                    if (double.TryParse(kvp.Key, NumberStyles.Float, CultureInfo.InvariantCulture, out double tableDia))
                    {
                        double diff = Math.Abs(tableDia - diameterMm);
                        if (diff < bestDiff && diff <= 0.5)
                        {
                            bestDiff = diff;
                            bestVal = kvp.Value;
                            found = true;
                        }
                    }
                }
                if (found) return bestVal;
            }
            return factor * diameterMm;
        }

        /// <summary>
        /// Longitud de perno para un agarre dado: agarre + suplemento del diámetro, redondeado hacia arriba al múltiplo de
        /// <c>bolts.length_increment_mm</c> (1/4"). Para el Detalle D (agarre 9,525 + 10 = 19,525 mm, Ø5/8") da 44,45 mm (1 3/4").
        /// </summary>
        public double ComputeBoltLengthMm(double gripMm, double diameterMm)
        {
            double raw = gripMm + GetBoltLengthAdditionMm(diameterMm);
            double increment = Bolts.LengthIncrementMm > 0.01 ? Bolts.LengthIncrementMm : 6.35;
            return Math.Ceiling(raw / increment - 1e-9) * increment;
        }

        /// <summary>
        /// Obtiene el tamaño mínimo de soldadura de filete según AISC 360 Tabla J2.4 para el espesor menor de las partes unidas en mm.
        /// </summary>
        public double GetMinWeldFilletSize(double thinnerThicknessMm)
        {
            if (Welds.MinFilletMm != null && Welds.MinFilletMm.Count > 0)
            {
                // Ordenar umbrales numéricos
                var thresholds = new List<KeyValuePair<double, double>>();
                double defaultVal = 8.0;

                foreach (var kvp in Welds.MinFilletMm)
                {
                    if (double.TryParse(kvp.Key, NumberStyles.Float, CultureInfo.InvariantCulture, out double th))
                    {
                        thresholds.Add(new KeyValuePair<double, double>(th, kvp.Value));
                    }
                    else if (kvp.Key.Equals("default", StringComparison.OrdinalIgnoreCase))
                    {
                        defaultVal = kvp.Value;
                    }
                }

                thresholds.Sort((a, b) => a.Key.CompareTo(b.Key));

                foreach (var th in thresholds)
                {
                    if (thinnerThicknessMm <= th.Key + 0.001)
                        return th.Value;
                }

                return defaultVal;
            }

            // Fallback directo AISC 360 Tabla J2.4:
            // <= 6 mm: 3 mm
            // 6 < t <= 13 mm: 5 mm
            // 13 < t <= 19 mm: 6 mm
            // > 19 mm: 8 mm
            if (thinnerThicknessMm <= 6.0) return 3.0;
            if (thinnerThicknessMm <= 13.0) return 5.0;
            if (thinnerThicknessMm <= 19.0) return 6.0;
            return 8.0;
        }

        /// <summary>
        /// Calcula un hash determinista SHA-256 de los límites y tolerancias configurados.
        /// </summary>
        public string ComputeHash()
        {
            var sb = new StringBuilder();
            sb.Append(SchemaVersion.ToString(CultureInfo.InvariantCulture)).Append('|')
              .Append(DimensionChainToleranceMm.ToString("0.000", CultureInfo.InvariantCulture)).Append('|')
              .Append(LabelValueToleranceMm.ToString("0.000", CultureInfo.InvariantCulture)).Append('|')
              .Append(AngleToleranceDeg.ToString("0.000", CultureInfo.InvariantCulture)).Append('|')
              .Append(NodeAxisMaxDistanceMm.ToString("0.000", CultureInfo.InvariantCulture)).Append('|')
              .Append(Bolts.MinSpacingFactor.ToString("0.000", CultureInfo.InvariantCulture)).Append('|');

            if (Bolts.EdgeDistanceMm != null)
            {
                foreach (var kv in Bolts.EdgeDistanceMm.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append(kv.Key).Append(':').Append(kv.Value.ToString("0.000", CultureInfo.InvariantCulture)).Append(';');
                }
            }
            sb.Append('|');
            if (Bolts.LengthAdditionMm != null)
            {
                foreach (var kv in Bolts.LengthAdditionMm.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append(kv.Key).Append(':').Append(kv.Value.ToString("0.000", CultureInfo.InvariantCulture)).Append(';');
                }
            }
            sb.Append('|').Append(Bolts.LengthIncrementMm.ToString("0.000", CultureInfo.InvariantCulture)).Append('|');
            if (Welds.MinFilletMm != null)
            {
                foreach (var kv in Welds.MinFilletMm.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append(kv.Key).Append(':').Append(kv.Value.ToString("0.000", CultureInfo.InvariantCulture)).Append(';');
                }
            }

            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] hash = sha.ComputeHash(bytes);
            var hex = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) hex.Append(b.ToString("x2"));
            return hex.ToString();
        }
    }
}
