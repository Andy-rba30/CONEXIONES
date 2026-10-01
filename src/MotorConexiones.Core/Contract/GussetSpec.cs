using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Especificación de la cartela principal del nudo.
    /// </summary>
    public sealed class GussetSpec
    {
        [JsonPropertyName("thickness_mm")]
        public double? ThicknessMm { get; set; }

        [JsonPropertyName("thickness_label")]
        public string? ThicknessLabel { get; set; }

        [JsonPropertyName("width_mm")]
        public double? WidthMm { get; set; }

        [JsonPropertyName("height_mm")]
        public double? HeightMm { get; set; }

        [JsonPropertyName("outline")]
        public GussetOutline? Outline { get; set; }

        [JsonPropertyName("chord_interface")]
        public string? ChordInterface { get; set; }

        [JsonPropertyName("weld_to_chord")]
        public WeldSpec? WeldToChord { get; set; }
    }
}
