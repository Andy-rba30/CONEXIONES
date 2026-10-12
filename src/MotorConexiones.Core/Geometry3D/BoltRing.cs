using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Una fila de pernos de una cara de la plancha, ya convertida en posiciones (mm, plano de la plancha).</summary>
    public sealed class BoltRow
    {
        public BoltRow(string face, IReadOnlyList<BoltPosition> positions, double spacingMm, double edgeMm, bool horizontal)
        {
            Face = face;
            Positions = positions;
            SpacingMm = spacingMm;
            EdgeMm = edgeMm;
            IsHorizontal = horizontal;
        }

        /// <summary><c>top</c>, <c>bottom</c>, <c>left</c> o <c>right</c>.</summary>
        public string Face { get; }
        public IReadOnlyList<BoltPosition> Positions { get; }
        public double SpacingMm { get; }
        /// <summary>Distancia del borde de la plancha al eje de la fila.</summary>
        public double EdgeMm { get; }
        /// <summary>Verdadero en las filas de arriba y abajo (los pernos se reparten a lo largo de X).</summary>
        public bool IsHorizontal { get; }
        public int Count => Positions.Count;

        /// <summary>El patrón rectangular de una fila (N × 1) para el backend de Advance Steel.</summary>
        public BoltGrid ToGrid()
        {
            BoltPosition first = Positions.Count > 0 ? Positions[0] : new BoltPosition(0, 0);
            BoltPosition last = Positions.Count > 0 ? Positions[Positions.Count - 1] : first;
            double ux = IsHorizontal ? 1.0 : 0.0;
            double uy = IsHorizontal ? 0.0 : 1.0;
            return new BoltGrid(Positions, first, last, ux, uy, -uy, ux, Positions.Count, 1, SpacingMm);
        }
    }

    /// <summary>
    /// Fase 12: los pernos de una placa de extremo <b>alrededor del tubo</b> (Detalles B, C e I): una fila por cara de la
    /// plancha, cada una con su cantidad, su paso y su distancia al borde. Las posiciones van en el plano de la plancha
    /// centrada en (0, 0): X a lo ancho, Y a lo alto. Arriba y abajo los pernos se reparten a lo largo de X centrados;
    /// izquierda y derecha, a lo largo de Y. Con un perno por cara (B e I) queda centrado en ella.
    /// </summary>
    public static class BoltRing
    {
        public static List<BoltRow> Rows(EndPlateSpec? plate, RingBoltSpec? bolts)
        {
            var rows = new List<BoltRow>();
            if (plate == null || bolts?.Faces == null) return rows;
            double w = plate.WidthMm ?? 0.0;
            double h = plate.HeightMm ?? 0.0;
            if (w <= 0 || h <= 0) return rows;
            Add(rows, "top", bolts.Faces.Top, horizontal: true, line: h / 2.0, sign: 1.0);
            Add(rows, "bottom", bolts.Faces.Bottom, horizontal: true, line: h / 2.0, sign: -1.0);
            Add(rows, "left", bolts.Faces.Left, horizontal: false, line: w / 2.0, sign: -1.0);
            Add(rows, "right", bolts.Faces.Right, horizontal: false, line: w / 2.0, sign: 1.0);
            return rows;
        }

        /// <summary>Todas las posiciones de todas las filas.</summary>
        public static List<BoltPosition> Positions(EndPlateSpec? plate, RingBoltSpec? bolts)
        {
            var all = new List<BoltPosition>();
            foreach (BoltRow row in Rows(plate, bolts)) all.AddRange(row.Positions);
            return all;
        }

        public static int Count(RingBoltSpec? bolts)
        {
            if (bolts?.Faces == null) return 0;
            return Math.Max(0, bolts.Faces.Top?.Count ?? 0) + Math.Max(0, bolts.Faces.Bottom?.Count ?? 0)
                 + Math.Max(0, bolts.Faces.Left?.Count ?? 0) + Math.Max(0, bolts.Faces.Right?.Count ?? 0);
        }

        private static void Add(List<BoltRow> rows, string face, BoltRowSpec? row, bool horizontal, double line, double sign)
        {
            int count = row?.Count ?? 0;
            if (row == null || count <= 0) return;
            double spacing = row.SpacingMm ?? 0.0;
            double edge = row.EdgeMm ?? 0.0;
            double along0 = -(count - 1) * spacing / 2.0;
            double across = sign * (line - edge);
            var positions = new List<BoltPosition>(count);
            for (int i = 0; i < count; i++)
            {
                double along = along0 + i * spacing;
                positions.Add(horizontal ? new BoltPosition(along, across) : new BoltPosition(across, along));
            }
            rows.Add(new BoltRow(face, positions, spacing, edge, horizontal));
        }
    }
}
