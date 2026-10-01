using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Información sobre el plano o detalle de origen.
    /// </summary>
    public sealed class SourceInfo
    {
        [JsonPropertyName("drawing")]
        public string? Drawing { get; set; }

        [JsonPropertyName("scale")]
        public string? Scale { get; set; }
    }
}
