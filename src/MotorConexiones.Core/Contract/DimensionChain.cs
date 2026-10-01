using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Cadena de cotas leída del plano para verificación de cuadre geométrico.
    /// </summary>
    public sealed class DimensionChain
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("values_mm")]
        public List<double> ValuesMm { get; set; } = new List<double>();

        [JsonPropertyName("expected_total_mm")]
        public double ExpectedTotalMm { get; set; }
    }
}
