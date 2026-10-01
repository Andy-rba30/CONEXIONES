using System;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>
    /// Segmento de línea 2D entre dos puntos en milímetros.
    /// </summary>
    public readonly struct Segment2D
    {
        public Point2D Start { get; }
        public Point2D End { get; }

        public Segment2D(Point2D start, Point2D end)
        {
            Start = start;
            End = end;
        }

        public double Length => Start.DistanceTo(End);

        /// <summary>
        /// Comprueba si dos segmentos se intersectan.
        /// Si allowEndpoints es false, tocarse en un extremo común no se considera intersección prohibida.
        /// </summary>
        public bool Intersects(Segment2D other, bool allowEndpoints = true)
        {
            double d1 = CrossProduct(other.Start, other.End, Start);
            double d2 = CrossProduct(other.Start, other.End, End);
            double d3 = CrossProduct(Start, End, other.Start);
            double d4 = CrossProduct(Start, End, other.End);

            // Intersección estricta (se cruzan limpiamente en el interior)
            if (((d1 > 1e-9 && d2 < -1e-9) || (d1 < -1e-9 && d2 > 1e-9)) &&
                ((d3 > 1e-9 && d4 < -1e-9) || (d3 < -1e-9 && d4 > 1e-9)))
            {
                return true;
            }

            if (!allowEndpoints)
            {
                // Si no se permiten contactos en extremos, comprobar si algún punto cae sobre el segmento
                if (Math.Abs(d1) <= 1e-9 && OnSegment(other.Start, other.End, Start)) return true;
                if (Math.Abs(d2) <= 1e-9 && OnSegment(other.Start, other.End, End)) return true;
                if (Math.Abs(d3) <= 1e-9 && OnSegment(Start, End, other.Start)) return true;
                if (Math.Abs(d4) <= 1e-9 && OnSegment(Start, End, other.End)) return true;
            }

            return false;
        }

        private static double CrossProduct(Point2D a, Point2D b, Point2D c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private static bool OnSegment(Point2D a, Point2D b, Point2D p)
        {
            return p.X >= Math.Min(a.X, b.X) - 1e-9 && p.X <= Math.Max(a.X, b.X) + 1e-9 &&
                   p.Y >= Math.Min(a.Y, b.Y) - 1e-9 && p.Y <= Math.Max(a.Y, b.Y) + 1e-9;
        }
    }
}
