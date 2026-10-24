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

        /// <summary>
        /// Fase 13: la envolvente convexa de un conjunto de puntos (cadena monótona de Andrew), en sentido antihorario y sin
        /// puntos repetidos ni colineales. Con menos de 3 puntos distintos devuelve los que haya.
        /// </summary>
        public static List<Point2D> ConvexHull(IEnumerable<Point2D> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            var sorted = points
                .GroupBy(p => (Math.Round(p.X, 6), Math.Round(p.Y, 6)))
                .Select(g => g.First())
                .OrderBy(p => p.X).ThenBy(p => p.Y)
                .ToList();
            if (sorted.Count < 3) return sorted;

            static double Cross(Point2D o, Point2D a, Point2D b) => (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);

            var lower = new List<Point2D>();
            foreach (Point2D p in sorted)
            {
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 1e-9) lower.RemoveAt(lower.Count - 1);
                lower.Add(p);
            }
            var upper = new List<Point2D>();
            for (int i = sorted.Count - 1; i >= 0; i--)
            {
                Point2D p = sorted[i];
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 1e-9) upper.RemoveAt(upper.Count - 1);
                upper.Add(p);
            }
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        /// <summary>
        /// Fase 13: quita los vértices casi colineales (giro menor que <paramref name="minTurnDeg"/>) y, mientras haya más de
        /// <paramref name="maxVertices"/>, sustituye la arista más corta (el chaflán) por la esquina en que se cortarían sus
        /// aristas vecinas. Pensado para un polígono convexo en sentido antihorario; devuelve una lista nueva.
        /// </summary>
        public static List<Point2D> Simplify(IReadOnlyList<Point2D> vertices, double minTurnDeg, int maxVertices)
        {
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            var list = vertices.ToList();
            bool removed = true;
            while (removed && list.Count > 3)
            {
                removed = false;
                for (int i = 0; i < list.Count && list.Count > 3; i++)
                {
                    Point2D prev = list[(i - 1 + list.Count) % list.Count];
                    Point2D next = list[(i + 1) % list.Count];
                    if (TurnDeg(prev, list[i], next) < minTurnDeg)
                    {
                        list.RemoveAt(i);
                        removed = true;
                        i--;
                    }
                }
            }
            while (list.Count > Math.Max(3, maxVertices))
            {
                int shortest = 0;
                double best = double.MaxValue;
                for (int i = 0; i < list.Count; i++)
                {
                    double length = list[i].DistanceTo(list[(i + 1) % list.Count]);
                    if (length < best)
                    {
                        best = length;
                        shortest = i;
                    }
                }
                int j = (shortest + 1) % list.Count;
                Point2D a0 = list[(shortest - 1 + list.Count) % list.Count], a1 = list[shortest];
                Point2D b0 = list[j], b1 = list[(j + 1) % list.Count];
                if (!TryIntersectLines(a0, a1, b0, b1, out Point2D corner))
                {
                    // Aristas vecinas paralelas: el chaflán es un lado entero; se quita el vértice más cercano al siguiente.
                    list.RemoveAt(j);
                    continue;
                }
                list[shortest] = corner;
                list.RemoveAt(j);
            }
            return list;
        }

        /// <summary>Giro (grados) del recorrido a → b → c en b (0 = colineal).</summary>
        public static double TurnDeg(Point2D a, Point2D b, Point2D c)
        {
            double ux = b.X - a.X, uy = b.Y - a.Y, vx = c.X - b.X, vy = c.Y - b.Y;
            double lu = Math.Sqrt(ux * ux + uy * uy), lv = Math.Sqrt(vx * vx + vy * vy);
            if (lu < 1e-9 || lv < 1e-9) return 0.0;
            double cos = Math.Max(-1.0, Math.Min(1.0, (ux * vx + uy * vy) / (lu * lv)));
            return Math.Acos(cos) * 180.0 / Math.PI;
        }

        /// <summary>Corte de las rectas a0→a1 y b0→b1; falso si son paralelas.</summary>
        public static bool TryIntersectLines(Point2D a0, Point2D a1, Point2D b0, Point2D b1, out Point2D point)
        {
            double dx1 = a1.X - a0.X, dy1 = a1.Y - a0.Y, dx2 = b1.X - b0.X, dy2 = b1.Y - b0.Y;
            double denominator = dx1 * dy2 - dy1 * dx2;
            if (Math.Abs(denominator) < 1e-9)
            {
                point = a1;
                return false;
            }
            double t = ((b0.X - a0.X) * dy2 - (b0.Y - a0.Y) * dx2) / denominator;
            point = new Point2D(a0.X + t * dx1, a0.Y + t * dy1);
            return true;
        }
    }
}
