using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Especificación de la placa cuchilla soldada dentro de la ranura del miembro y empernada a la cartela.
    /// </summary>
    public sealed class KnifePlateSpec
    {
        [JsonPropertyName("thickness_mm")]
        public double? ThicknessMm { get; set; }

        [JsonPropertyName("thickness_label")]
        public string? ThicknessLabel { get; set; }

        [JsonPropertyName("length_mm")]
        public double? LengthMm { get; set; }

        [JsonPropertyName("width_mm")]
        public double? WidthMm { get; set; }

        [JsonPropertyName("insertion_mm")]
        public double? InsertionMm { get; set; }
    }
}
