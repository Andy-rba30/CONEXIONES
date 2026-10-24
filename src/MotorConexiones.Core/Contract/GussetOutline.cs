using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Contorno geométrico de la cartela. <c>polygon</c> trae los puntos (<c>points_mm</c>, marco canónico, antihorario);
    /// <c>auto</c> (Fase 13) no los trae: el Core los calcula desde los extremos reales de las barras y el cordón
    /// (<c>Geometry2D/AutoOutline</c>) con los márgenes opcionales de aquí o los de <c>choices.json</c>.
    /// </summary>
    public sealed class GussetOutline
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "polygon";

        [JsonPropertyName("points_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<double[]>? PointsMm { get; set; }

        /// <summary>Fase 13 (<c>auto</c>): del final de la ranura o del extremo libre de la placa cuchilla al borde (40 por defecto).</summary>
        [JsonPropertyName("margin_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? MarginMm { get; set; }

        /// <summary>Fase 13 (<c>auto</c>): del lado de la barra (o de la placa) al borde (30 por defecto).</summary>
        [JsonPropertyName("side_margin_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? SideMarginMm { get; set; }

        /// <summary>Fase 13 (<c>auto</c>): cuánto asoma la cartela más allá de la cara del cordón (40 por defecto).</summary>
        [JsonPropertyName("beyond_chord_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? BeyondChordMm { get; set; }
    }
}
