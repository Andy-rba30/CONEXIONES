using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                    }
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
    }
}
