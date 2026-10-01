using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Especificación del cordón en el nudo de cercha.
    /// </summary>
    public sealed class ChordSpec
    {
        [JsonPropertyName("element_id")]
        public long ElementId { get; set; }

        [JsonPropertyName("profile")]
        public string? Profile { get; set; }

        [JsonPropertyName("continuous")]
        public bool Continuous { get; set; } = true;
    }
}
