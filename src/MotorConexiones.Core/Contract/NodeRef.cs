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
    }
}
