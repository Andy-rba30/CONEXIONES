using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Storage
{
    /// <summary>
    /// Registro de parámetros modificados en un elemento estructural para permitir rollback/restauración.
    /// </summary>
    public sealed class ModifiedMemberRecord
    {
        [JsonPropertyName("element_id")]
        public long ElementId { get; set; }

        [JsonPropertyName("end_index")]
        public int EndIndex { get; set; }

        [JsonPropertyName("parameter_name")]
        public string ParameterName { get; set; } = string.Empty;

        [JsonPropertyName("original_value_feet")]
        public double OriginalValueFeet { get; set; }

        [JsonPropertyName("applied_setback_mm")]
        public double AppliedSetbackMm { get; set; }
    }
}
