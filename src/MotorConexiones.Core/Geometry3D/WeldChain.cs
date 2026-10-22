using System;
using System.Collections.Generic;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Ronda 12c: los segmentos de soldadura de un marco encadenados en polilíneas, que es lo que entiende
    /// <c>WeldLine(Point3d[] polyLineWCS, Vector3d xvec, Vector3d yvec)</c> de Advance Steel (sondeo 26): el contorno de la
    /// huella del tubo en la plancha de extremo es una cadena cerrada de cuatro lados; las dos líneas a los costados de la
    /// ranura de una barra soldada a la cartela son dos cadenas abiertas; la huella de una barra cortada contra la cara del
    /// cordón, una. El tamaño del filete de la cadena es el mayor de sus segmentos.
    /// </summary>
    public sealed class WeldChain
    {
        private WeldChain(List<BoltPosition> points, List<WeldLine2D> segments, bool isClosed, double sizeMm)
        {
            Points = points;
            Segments = segments;
            IsClosed = isClosed;
            SizeMm = sizeMm;
        }

        /// <summary>Vértices de la polilínea en mm (el primero no se repite al final aunque esté cerrada).</summary>
        public IReadOnlyList<BoltPosition> Points { get; }

        /// <summary>Los segmentos originales que forman la cadena, en orden (para la reserva segmento a segmento).</summary>
        public IReadOnlyList<WeldLine2D> Segments { get; }

        /// <summary>True si el último segmento vuelve al principio (tres o más vértices).</summary>
        public bool IsClosed { get; }

        /// <summary>Tamaño del filete (mm): el mayor de los segmentos de la cadena.</summary>
        public double SizeMm { get; }

        /// <summary>Encadena los segmentos cuyo final coincide con el principio del siguiente (tolerancia en mm); descarta los de menos de 1 mm.</summary>
        public static List<WeldChain> Build(IReadOnlyList<WeldLine2D>? segments, double toleranceMm = 0.5)
        {
            var chains = new List<WeldChain>();
            if (segments == null || segments.Count == 0) return chains;
            var remaining = new List<WeldLine2D>();
            foreach (WeldLine2D segment in segments)
            {
                if (Distance(segment.Start, segment.End) >= 1.0) remaining.Add(segment);
            }
            while (remaining.Count > 0)
            {
                WeldLine2D first = remaining[0];
                remaining.RemoveAt(0);
                var points = new List<BoltPosition> { first.Start, first.End };
                var used = new List<WeldLine2D> { first };
                double size = first.SizeMm;
                bool extended = true;
                while (extended && remaining.Count > 0)
                {
                    extended = false;
                    for (int i = 0; i < remaining.Count; i++)
                    {
                        WeldLine2D candidate = remaining[i];
                        if (Distance(points[points.Count - 1], candidate.Start) <= toleranceMm)
                        {
                            points.Add(candidate.End);
                        }
                        else if (Distance(points[points.Count - 1], candidate.End) <= toleranceMm)
                        {
                            points.Add(candidate.Start);
                        }
                        else
                        {
                            continue;
                        }
                        used.Add(candidate);
                        size = Math.Max(size, candidate.SizeMm);
                        remaining.RemoveAt(i);
                        extended = true;
                        break;
                    }
                }
                bool closed = points.Count >= 4 && Distance(points[0], points[points.Count - 1]) <= toleranceMm;
                if (closed) points.RemoveAt(points.Count - 1);
                chains.Add(new WeldChain(points, used, closed, size));
            }
            return chains;
        }

        private static double Distance(BoltPosition a, BoltPosition b)
        {
            double dx = a.X - b.X, dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
