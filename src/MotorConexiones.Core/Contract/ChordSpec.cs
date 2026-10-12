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

        /// <summary>
        /// Fase 12, <c>direct_weld</c>: cara del cordón donde llegan las barras en el sistema local: <c>auto</c> (el lado
        /// donde están), <c>+y</c> o <c>-y</c>. Se omite si es nula.
        /// </summary>
        [JsonPropertyName("face")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Face { get; set; }

        /// <summary>
        /// Fase 12, <c>end_plate</c> (empalme): cuánto retrocede este tramo desde el punto de empalme para dejar sitio a su
        /// plancha (espesor de la plancha más media holgura). Se omite si es nulo.
        /// </summary>
        [JsonPropertyName("end_setback_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? EndSetbackMm { get; set; }
    }
}
