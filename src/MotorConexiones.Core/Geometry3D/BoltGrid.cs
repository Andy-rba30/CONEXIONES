using System.Collections.Generic;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Patrón rectangular de pernos en el plano del nudo (mm, sistema local). Lleva tanto las posiciones
    /// individuales (camino B: un cilindro por perno) como la descripción del patrón (camino A:
    /// <c>FinitRectScrewBoltPattern(ptRef, ptRef2, vX, vY)</c> con <c>Nx</c>, <c>Ny</c>, <c>Dx</c>, <c>Dy</c>,
    /// firmas reales del sondeo 10 de la Fase 1).
    /// </summary>
    public sealed class BoltGrid
    {
        public BoltGrid(IReadOnlyList<BoltPosition> positions, BoltPosition firstCorner, BoltPosition oppositeCorner,
            double ux, double uy, double vx, double vy, int countAlong, int countAcross, double spacingMm)
        {
            Positions = positions;
            FirstCorner = firstCorner;
            OppositeCorner = oppositeCorner;
            Ux = ux;
            Uy = uy;
            Vx = vx;
            Vy = vy;
            CountAlong = countAlong;
            CountAcross = countAcross;
            SpacingMm = spacingMm;
        }

        /// <summary>Centro de cada perno (mm).</summary>
        public IReadOnlyList<BoltPosition> Positions { get; }

        /// <summary>Primer perno del patrón (fila 0, columna 0).</summary>
        public BoltPosition FirstCorner { get; }

        /// <summary>Último perno del patrón (esquina opuesta).</summary>
        public BoltPosition OppositeCorner { get; }

        /// <summary>Dirección unitaria "a lo largo del miembro" (eje X del patrón).</summary>
        public double Ux { get; }
        public double Uy { get; }

        /// <summary>Dirección unitaria transversal (eje Y del patrón).</summary>
        public double Vx { get; }
        public double Vy { get; }

        /// <summary>Pernos a lo largo (filas) y a lo ancho (columnas).</summary>
        public int CountAlong { get; }
        public int CountAcross { get; }

        /// <summary>Paso entre pernos en ambas direcciones.</summary>
        public double SpacingMm { get; }

        public int Count => Positions.Count;
    }
}
