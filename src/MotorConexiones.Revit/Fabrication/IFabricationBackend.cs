using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Abstracción del backend de fabricación de geometría:
    /// - Camino A: Elementos nativos de acero de Revit / Advance Steel (Plates, Bolts, Welds).
    /// - Camino B: Extrusiones geométricas en DirectShape (reserva garantizada que compila en cualquier entorno).
    /// </summary>
    public interface IFabricationBackend
    {
        /// <summary>Nombre del backend ("advancesteel" o "directshape").</summary>
        string Name { get; }

        /// <summary>Indica si el backend está disponible y listo para operar en la sesión actual de Revit.</summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Crea una placa de espesor <paramref name="thicknessMm"/> centrada en el plano XY del sistema local
        /// con el contorno poligonal <paramref name="outlineMm"/> (mm).
        /// </summary>
        ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, string name);

        /// <summary>
        /// Crea un grupo de pernos de diámetro <paramref name="diameterMm"/> y longitud <paramref name="lengthMm"/>
        /// en las posiciones indicadas en el plano del nudo.
        /// </summary>
        IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name);

        /// <summary>
        /// Crea representaciones de cordones de soldadura de filete.
        /// </summary>
        IList<ElementId> CreateWelds(Document document, NodeFrame frame, IReadOnlyList<WeldLine2D> weldsMm, string name);
    }
}
