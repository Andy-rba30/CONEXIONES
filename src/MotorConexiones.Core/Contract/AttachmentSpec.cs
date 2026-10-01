using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Tipo de unión de un miembro a la cartela (welded_slot o bolted_knife_plate).
    /// </summary>
    public sealed class AttachmentSpec
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "welded_slot";

        [JsonPropertyName("slot_length_mm")]
        public double? SlotLengthMm { get; set; }

        [JsonPropertyName("weld")]
        public WeldSpec? Weld { get; set; }

        [JsonPropertyName("plate")]
        public KnifePlateSpec? Plate { get; set; }

        [JsonPropertyName("bolts")]
        public BoltPatternSpec? Bolts { get; set; }

        [JsonPropertyName("weld_plate_to_member")]
        public WeldSpec? WeldPlateToMember { get; set; }
    }
}
