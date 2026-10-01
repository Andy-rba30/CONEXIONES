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

        /// <summary>
        /// Cara de la cartela sobre la que apoya la placa cuchilla, en el sistema local del nudo: <c>"+z"</c> (por defecto)
        /// o <c>"-z"</c>. La placa y la cartela se solapan cara con cara y los pernos atraviesan las dos (ronda 6b).
        /// </summary>
        [JsonPropertyName("gusset_face")]
        public string? GussetFace { get; set; }
    }
}
