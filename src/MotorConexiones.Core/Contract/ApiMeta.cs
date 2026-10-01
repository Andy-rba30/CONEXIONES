using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>Bloque <c>meta</c> del sobre de respuesta (sección 10 del encargo).</summary>
    public sealed class ApiMeta
    {
        [JsonPropertyName("operation")]
        public string Operation { get; set; } = "";

        [JsonPropertyName("duration_ms")]
        public long DurationMs { get; set; }

        [JsonPropertyName("addin_version")]
        public string AddinVersion { get; set; } = AddinInfo.Version;
    }
}
