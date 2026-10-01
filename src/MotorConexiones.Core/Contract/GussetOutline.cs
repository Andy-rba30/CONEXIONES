using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Contorno geométrico de la cartela. En v1 solo mode="polygon".
    /// </summary>
    public sealed class GussetOutline
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "polygon";

        [JsonPropertyName("points_mm")]
        public List<double[]>? PointsMm { get; set; }
    }
}
