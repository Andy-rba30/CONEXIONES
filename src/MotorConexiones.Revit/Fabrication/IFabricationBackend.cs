using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>Posición de un perno en el plano de la placa (mm, sistema local del nudo).</summary>
    public readonly struct BoltPosition
    {
        public BoltPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }
    }

    /// <summary>
    /// Lo que el add-in necesita de la geometría, independiente del camino elegido (A: fabricación de acero de
    /// Revit / Advance Steel; B: DirectShape o familias propias). La Fase 1 implementa placa y grupo de pernos con B;
    /// la Fase 3 completa soldaduras, cortes y borrado con el camino que decida la prueba técnica.
    /// </summary>
    public interface IFabricationBackend
    {
        /// <summary>Nombre corto que devuelve <c>conn_ping</c> en <c>backend</c>.</summary>
        string Name { get; }

        /// <summary>
        /// Crea una placa de espesor <paramref name="thicknessMm"/> centrada en el plano XY del sistema local
        /// (Z de -t/2 a +t/2) con el contorno <paramref name="outlineMm"/> (polígono cerrado implícito, mm).
        /// </summary>
        ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, string name);

        /// <summary>Crea un perno por posición: cilindro de diámetro <paramref name="diameterMm"/> y longitud <paramref name="lengthMm"/> centrado en el plano de la placa.</summary>
        IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name);
    }
}
