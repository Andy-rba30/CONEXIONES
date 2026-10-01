using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Especificación de soldadura (de filete).
    /// </summary>
    public sealed class WeldSpec
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "fillet";

        [JsonPropertyName("size_mm")]
        public double? SizeMm { get; set; }

        [JsonPropertyName("all_around")]
        public bool AllAround { get; set; } = true;
    }
}
