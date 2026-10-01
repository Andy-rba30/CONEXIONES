using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>
    /// Polígono plano cerrado en milímetros. Se asume cerrado implícitamente entre el último vértice y el primero.
    /// </summary>
    public sealed class Polygon2D
    {
        public IReadOnlyList<Point2D> Vertices { get; }

        public Polygon2D(IEnumerable<Point2D> vertices)
        {
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            var list = vertices.ToList();
            // Si el último coincide exactamente con el primero, remover el duplicado para mantener la lista canónica
            if (list.Count > 1 && list[list.Count - 1].Equals(list[0]))
            {
                list.RemoveAt(list.Count - 1);
            }
            Vertices = list;
        }

        public static Polygon2D FromPointArrays(IEnumerable<double[]> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            var pts = new List<Point2D>();
            foreach (var p in points)
            {
                if (p != null && p.Length >= 2)
                {
                    pts.Add(new Point2D(p[0], p[1]));
                }
            }
            return new Polygon2D(pts);
        }

        /// <summary>
        /// Área con signo usando la fórmula de Shoelace (positiva si los vértices están en sentido antihorario).
        /// </summary>
        public double SignedArea()
        {
            int n = Vertices.Count;
            if (n < 3) return 0.0;

            double area = 0.0;
            for (int i = 0; i < n; i++)
            {
                var p1 = Vertices[i];
                var p2 = Vertices[(i + 1) % n];
                area += (p1.X * p2.Y - p2.X * p1.Y);
            }
            return area * 0.5;
        }

        public double Area => Math.Abs(SignedArea());

        /// <summary>
        /// Comprueba si el polígono es simple (no auto-intersecante, con al menos 3 vértices y área positiva).
        /// </summary>
        public bool IsSimple(out string errorReason)
        {
            errorReason = string.Empty;
            int n = Vertices.Count;
            if (n < 3)
            {
                errorReason = $"El polígono tiene {n} vértices; requiere al menos 3.";
                return false;
            }

            if (Area < 1e-4)
            {
                errorReason = "El área del polígono es prácticamente nula o degenerada.";
                return false;
            }

            var edges = new Segment2D[n];
            for (int i = 0; i < n; i++)
            {
                edges[i] = new Segment2D(Vertices[i], Vertices[(i + 1) % n]);
            }

            // Comprobar que no haya cruces entre aristas no consecutivas
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    // Aristas consecutivas comparten un vértice legítimo; las no consecutivas no pueden tocarse
                    bool areConsecutive = (j == i + 1) || (i == 0 && j == n - 1);
                    if (areConsecutive)
                    {
                        // Si son consecutivas, no pueden solaparse colinealmente en dirección opuesta
                        continue;
                    }

                    if (edges[i].Intersects(edges[j], allowEndpoints: false))
                    {
                        errorReason = $"Las aristas {i} y {j} se cruzan o solapan (auto-intersección del contorno).";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Comprueba si un punto dado cae dentro del polígono (o en su borde) usando ray casting.
        /// </summary>
        public bool ContainsPoint(Point2D point, bool countBoundary = true)
        {
            int n = Vertices.Count;
            if (n < 3) return false;

            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var vi = Vertices[i];
                var vj = Vertices[j];

                // Comprobación de borde
                if (countBoundary)
                {
                    var seg = new Segment2D(vj, vi);
                    double cross = (vi.X - vj.X) * (point.Y - vj.Y) - (vi.Y - vj.Y) * (point.X - vj.X);
                    if (Math.Abs(cross) <= 1e-6)
                    {
                        if (point.X >= Math.Min(vj.X, vi.X) - 1e-6 && point.X <= Math.Max(vj.X, vi.X) + 1e-6 &&
                            point.Y >= Math.Min(vj.Y, vi.Y) - 1e-6 && point.Y <= Math.Max(vj.Y, vi.Y) + 1e-6)
                        {
                            return true;
                        }
                    }
                }

                // Ray casting horizontal hacia +X
                if (((vi.Y > point.Y) != (vj.Y > point.Y)) &&
                    (point.X < (vj.X - vi.X) * (point.Y - vi.Y) / (vj.Y - vi.Y) + vi.X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Obtiene la caja envolvente (bounding box) del polígono en mm.
        /// </summary>
        public (Point2D Min, Point2D Max) GetBoundingBox()
        {
            if (Vertices.Count == 0)
                return (new Point2D(0, 0), new Point2D(0, 0));

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var v in Vertices)
            {
                if (v.X < minX) minX = v.X;
                if (v.Y < minY) minY = v.Y;
                if (v.X > maxX) maxX = v.X;
                if (v.Y > maxY) maxY = v.Y;
            }

            return (new Point2D(minX, minY), new Point2D(maxX, maxY));
        }
    }
}
