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

        /// <summary>La unión a la cartela (<c>gusset_node</c>). Fase 12: se omite si es nula (los otros tipos no la llevan).</summary>
        [JsonPropertyName("attachment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AttachmentSpec? Attachment { get; set; }

        /// <summary>
        /// Fase 12 (opción A): dónde corta el eje de esta barra al eje del cordón, medido desde el punto de trabajo a lo
        /// largo de +X del marco canónico, con signo (0 cuando concurre). Informativo: el validador lo mide del modelo.
        /// </summary>
        [JsonPropertyName("axis_offset_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? AxisOffsetMm { get; set; }

        /// <summary>
        /// Fase 12, <c>direct_weld</c> en un empalme (decisión D5): el tramo del cordón al que llega el eje de esta barra
        /// cuando no es <c>chord.element_id</c>.
        /// </summary>
        [JsonPropertyName("chord_element_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? ChordElementId { get; set; }

        /// <summary>Fase 12, <c>direct_weld</c>: cómo termina la barra. En esta fase solo <c>to_chord_face</c>.</summary>
        [JsonPropertyName("end_cut")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? EndCut { get; set; }

        /// <summary>Fase 12, <c>direct_weld</c>: holgura entre el corte de la barra y la cara del cordón (0 por defecto).</summary>
        [JsonPropertyName("end_gap_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? EndGapMm { get; set; }

        /// <summary>Fase 12, <c>direct_weld</c>: la soldadura de la barra a la cara del cordón (filete, todo el contorno).</summary>
        [JsonPropertyName("weld")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WeldSpec? Weld { get; set; }

        /// <summary>
        /// Fase 12 (decisión D6): la marca del empalme de la diagonal (Detalle B) a una distancia de la cara del cordón. Solo
        /// se dibuja y se escribe; no se fabrica sobre una barra continua (Fase 13).
        /// </summary>
        [JsonPropertyName("splice_hint")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SpliceHintSpec? SpliceHint { get; set; }
    }
}
