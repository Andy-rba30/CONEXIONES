using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Un tipo de conexión (<see cref="Types.IConnectionType"/>) que sabe dibujarse en 2D implementa esta interfaz.
    /// <see cref="SketchBuilder"/> la busca por <c>connection_type</c>; un tipo nuevo aporta su croquis sin tocar el núcleo.
    /// </summary>
    public interface ISketchProvider
    {
        Sketch BuildSketch(ConnectionSpec spec, SketchNodeInfo nodeInfo);
    }
}
