using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Un tipo de conexión que sabe dibujarse en 2D. <see cref="SketchBuilder"/> lo busca en el registro de tipos; un
    /// tipo nuevo aporta su croquis implementando esta interfaz además de <see cref="Types.IConnectionType"/>.
    /// </summary>
    public interface ISketchProvider
    {
        SketchModel BuildSketch(ConnectionSpec spec, SketchNodeInput node);
    }
}
