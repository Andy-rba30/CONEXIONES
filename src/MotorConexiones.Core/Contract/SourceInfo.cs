using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Información sobre el plano o detalle de origen. Desde la Fase 7 también la trazabilidad del catálogo:
    /// <c>template_id</c> (plantilla de la que salió la especificación) y <c>batch_id</c> (reservado para la Fase 9).
    /// Los dos se omiten al serializar cuando son nulos, para que las especificaciones de siempre firmen el mismo token.
    /// </summary>
    public sealed class SourceInfo
    {
        [JsonPropertyName("drawing")]
        public string? Drawing { get; set; }

        [JsonPropertyName("scale")]
        public string? Scale { get; set; }

        [JsonPropertyName("template_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TemplateId { get; set; }

        [JsonPropertyName("batch_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BatchId { get; set; }
    }
}
