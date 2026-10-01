using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Patrón de pernos en la unión empernada.
    /// </summary>
    public sealed class BoltPatternSpec
    {
        [JsonPropertyName("diameter_mm")]
        public double? DiameterMm { get; set; }

        [JsonPropertyName("diameter_label")]
        public string? DiameterLabel { get; set; }

        [JsonPropertyName("rows")]
        public int? Rows { get; set; }

        [JsonPropertyName("columns")]
        public int? Columns { get; set; }

        [JsonPropertyName("spacing_mm")]
        public double? SpacingMm { get; set; }

        [JsonPropertyName("edge_mm")]
        public double? EdgeMm { get; set; }

        [JsonPropertyName("first_row_from_plate_end_mm")]
        public double? FirstRowFromPlateEndMm { get; set; }

        /// <summary>
        /// Longitud del perno (bajo cabeza) si el plano la indica. Si falta, se calcula del agarre (cartela + placa) más el
        /// suplemento de <c>config/limits.json</c> (ronda 6b).
        /// </summary>
        [JsonPropertyName("length_mm")]
        public double? LengthMm { get; set; }
    }
}
