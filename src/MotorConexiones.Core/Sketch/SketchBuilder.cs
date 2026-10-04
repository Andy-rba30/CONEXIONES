using System;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Types;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Punto de entrada del croquis: elige el dibujo según <c>connection_type</c> (el tipo registrado que implemente
    /// <see cref="ISketchProvider"/>; <c>gusset_node</c> siempre tiene croquis aunque el registro esté vacío).
    /// </summary>
    public static class SketchBuilder
    {
        public static SketchModel Build(ConnectionSpec spec, SketchNodeInput? node)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            node = node ?? SketchNodeInput.Schematic(spec);

            string typeName = spec.ConnectionType ?? "";
            IConnectionType? type = ConnectionTypeRegistry.Find(typeName);
            if (type is ISketchProvider provider)
            {
                return provider.BuildSketch(spec, node);
            }
            if (string.Equals(typeName, GussetNodeType.Instance.Name, StringComparison.Ordinal))
            {
                // gusset_node siempre tiene croquis, esté o no registrado (el botón de la cinta no pasa por Bridge).
                return GussetNodeSketch.Build(spec, node);
            }

            var empty = new SketchModel { IsSchematic = node.IsSchematic };
            string text = string.IsNullOrEmpty(typeName)
                ? "La especificación no indica connection_type: no hay croquis."
                : "El tipo de conexión '" + typeName + "' no aporta croquis.";
            empty.Notes.Add(text);
            empty.Labels.Add(new SketchLabel(new Point2D(0, 0), text, "connection_type", emphasized: true));
            empty.ComputeBounds();
            return empty;
        }
    }
}
