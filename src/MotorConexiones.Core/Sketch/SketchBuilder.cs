using System;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Types;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Punto de entrada del croquis: <c>Build(spec, nodeInfo)</c> devuelve las primitivas en mm a partir de la
    /// especificación y de los datos del nudo. El dibujo lo aporta el tipo de conexión (<see cref="ISketchProvider"/>);
    /// si el tipo no sabe dibujarse, devuelve un croquis vacío con una nota, nunca lanza.
    /// </summary>
    public static class SketchBuilder
    {
        public static Sketch Build(ConnectionSpec spec, SketchNodeInfo nodeInfo, IConnectionType? type = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (nodeInfo == null) throw new ArgumentNullException(nameof(nodeInfo));

            string typeName = spec.ConnectionType ?? string.Empty;
            type ??= ConnectionTypeRegistry.Find(typeName);
            // gusset_node siempre tiene croquis, aunque el registro no esté cargado o tenga otra implementación del tipo.
            if (type is not ISketchProvider && string.Equals(typeName, GussetNodeType.Instance.Name, StringComparison.Ordinal))
            {
                type = GussetNodeType.Instance;
            }

            if (type is ISketchProvider provider)
            {
                Sketch sketch = provider.BuildSketch(spec, nodeInfo);
                if (nodeInfo.Note != null && !sketch.Notes.Contains(nodeInfo.Note)) sketch.Notes.Add(nodeInfo.Note);
                return sketch;
            }

            var empty = new Sketch();
            string message = string.IsNullOrEmpty(typeName)
                ? "La especificación no indica connection_type: no hay croquis."
                : "El tipo de conexión '" + typeName + "' no aporta croquis.";
            empty.Notes.Add(message);
            empty.Labels.Add(new SketchLabel(new SketchPoint(0, 0), message, SketchKind.Label, 1));
            return empty;
        }
    }
}
