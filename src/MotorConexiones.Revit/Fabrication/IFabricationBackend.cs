using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Sesión de fabricación: todo lo que se crea o borra para UNA conexión ocurre dentro de una sola sesión.
    /// En el camino A es una única <c>FabricationTransaction</c> de Advance Steel (la Fase 1 midió 49–132 s por
    /// transacción, así que no puede haber una por elemento). En el camino B no hace nada.
    /// Si no se llama a <see cref="Complete"/> antes de <see cref="IDisposable.Dispose"/>, la sesión se cancela.
    /// </summary>
    public interface IFabricationSession : IDisposable
    {
        /// <summary>Confirma la sesión (Commit de la transacción de fabricación).</summary>
        void Complete();
    }

    /// <summary>
    /// Abstracción del backend de fabricación de geometría:
    /// - Camino A: elementos nativos de acero de Revit / Advance Steel (Plates, Bolts).
    /// - Camino B: extrusiones en DirectShape (reserva que compila en cualquier entorno).
    /// </summary>
    public interface IFabricationBackend
    {
        /// <summary>Nombre del backend ("advancesteel" o "directshape").</summary>
        string Name { get; }

        /// <summary>Indica si el backend está disponible y listo para operar en la sesión actual de Revit.</summary>
        bool IsAvailable { get; }

        /// <summary>Abre la sesión de fabricación de una conexión. Llamar siempre dentro de la Transaction de Revit de la operación.</summary>
        IFabricationSession BeginSession(Document document, string name);

        /// <summary>
        /// Crea una placa de espesor <paramref name="thicknessMm"/> centrada en el plano XY del sistema local
        /// con el contorno poligonal <paramref name="outlineMm"/> (mm).
        /// </summary>
        ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, string name);

        /// <summary>Pernos sueltos en las posiciones indicadas (lo usa la prueba técnica de la Fase 1).</summary>
        IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name);

        /// <summary>Patrón rectangular de pernos (filas, columnas y paso reales), que es lo que entiende Advance Steel.</summary>
        IList<ElementId> CreateBoltPattern(Document document, NodeFrame frame, BoltGrid grid, double diameterMm, double lengthMm, string name);

        /// <summary>Crea representaciones de cordones de soldadura de filete.</summary>
        IList<ElementId> CreateWelds(Document document, NodeFrame frame, IReadOnlyList<WeldLine2D> weldsMm, string name);

        /// <summary>Borra elementos creados por este backend (dentro de la sesión en el camino A).</summary>
        void DeleteElements(Document document, ICollection<ElementId> elementIds);
    }
}
