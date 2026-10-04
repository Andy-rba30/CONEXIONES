using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Un tipo de conexión (<see cref="Types.IConnectionType"/>) que sabe dibujarse en 2D implementa esta interfaz.
    /// <see cref="SketchBuilder"/> la busca por <c>connection_type</c>; un tipo nuevo aporta su croquis sin tocar el núcleo.
    /// <paramref name="limits"/> es <c>config/limits.json</c> (o los valores por defecto si es nulo): el croquis lo usa para
    /// mostrar lo mismo que se crearía (por ejemplo, la longitud de los pernos calculada del agarre).
    /// </summary>
    public interface ISketchProvider
    {
        Sketch BuildSketch(ConnectionSpec spec, SketchNodeInfo nodeInfo, LimitsConfig? limits = null);
    }
}
