using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Identificadores de los elementos del modelo de Revit que concurren en el nudo.
    /// </summary>
    public sealed class NodeRef
    {
        [JsonPropertyName("element_ids")]
        public List<long> ElementIds { get; set; } = new List<long>();

        /// <summary>
        /// Fase 12 (opción A): excentricidad del nudo en mm, informativa: distancia a lo largo del cordón entre los cortes
        /// extremos de los ejes de las barras con el eje del cordón (0 cuando concurren). La escribe <c>conn_get_node_info</c>
        /// y Conectar; el validador la mide del modelo. Se omite al serializar cuando es nula, para que las especificaciones
        /// de siempre firmen el mismo token.
        /// </summary>
        [JsonPropertyName("eccentricity_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? EccentricityMm { get; set; }
    }
}
