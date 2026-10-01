using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Especificación de un miembro que concurre al nudo (diagonal o montante).
    /// </summary>
    public sealed class MemberSpec
    {
        [JsonPropertyName("element_id")]
        public long ElementId { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("profile")]
        public string? Profile { get; set; }

        [JsonPropertyName("end_setback_mm")]
        public double? EndSetbackMm { get; set; }

        [JsonPropertyName("expected_angle_deg")]
        public double? ExpectedAngleDeg { get; set; }

        [JsonPropertyName("attachment")]
        public AttachmentSpec? Attachment { get; set; }
    }
}
