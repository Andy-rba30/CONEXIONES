using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Una barra proyectada en el alzado (mm).</summary>
    public sealed class TrussMapSegment
    {
        public TrussMapSegment(long elementId, double x0, double y0, double x1, double y1, string? typeName, bool isChord)
        {
            ElementId = elementId;
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            TypeName = typeName;
            IsChord = isChord;
        }

        public long ElementId { get; }
        public double X0 { get; }
        public double Y0 { get; }
        public double X1 { get; }
        public double Y1 { get; }
        public string? TypeName { get; }

        /// <summary>Verdadero si es el cordón de algún nudo del plan (se dibuja más gruesa).</summary>
        public bool IsChord { get; }

        public double LengthMm => Math.Sqrt((X1 - X0) * (X1 - X0) + (Y1 - Y0) * (Y1 - Y0));
    }

    /// <summary>Un nudo proyectado en el alzado, con lo que el mapa necesita para dibujarlo y explicarlo.</summary>
    public sealed class TrussMapNode
    {
        public TrussMapNode(PlanNode node, double x, double y)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Name = node.Name;
            X = x;
            Y = y;
            Status = node.Status;
            StatusText = PlanAdvice.StatusText(node);
            ColorName = PlanAdvice.ColorName(node);
            VisibleByDefault = PlanAdvice.VisibleByDefault(node);
            IsMirrored = node.IsMirrored;
            TemplateName = node.TemplateName;
            Label = PlanAdvice.MapLabel(node);
            Number = ParseNumber(node.Name);
        }

        public string Name { get; }
        public double X { get; }
        public double Y { get; }
        public string Status { get; }
        public string StatusText { get; }
        public string ColorName { get; }
        public bool VisibleByDefault { get; }
        public bool IsMirrored { get; }
        public string? TemplateName { get; }

        /// <summary>"N4 · Listo · Nudo tipico Detalle D · igual": el globo al pasar el ratón.</summary>
        public string Label { get; }

        /// <summary>El número del nombre (4 en "N4"; "N5-2" → 5), para escribirlo dentro del círculo.</summary>
        public string Number { get; }

        private static string ParseNumber(string name)
        {
            string digits = new string((name ?? "").SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
            return digits.Length > 0 ? digits : name ?? "";
        }
    }

    /// <summary>
    /// Ronda 8c (V1): el alzado de la cercha para dibujarlo en la ventana del plan. A partir de los ejes de las barras y de
    /// los nudos del plan elige el plano de la cercha (el de mínimos cuadrados de los extremos de las barras, que es exacto
    /// cuando son coplanares), proyecta cada barra a un segmento 2D en mm y cada nudo a un punto con su nombre, estado,
    /// espejo y plantilla, y devuelve el rectángulo envolvente. Geometría pura, sin Revit: el eje X del mapa sigue la
    /// cercha (hacia +X global, o +Y si la cercha va en Y) y el Y del mapa apunta hacia arriba (+Z global). Las coordenadas
    /// son las globales proyectadas: en la cercha del Hangar (Y constante) el mapa es X–Z del modelo tal cual.
    /// </summary>
    public sealed class TrussMap
    {
        /// <summary>Distancia al plano a partir de la cual los ejes ya no se consideran coplanares.</summary>
        public const double CoplanarToleranceMm = 1.0;

        private TrussMap(Vec3 origin, Vec3 axisU, Vec3 axisV, Vec3 normal)
        {
            Origin = origin;
            AxisU = axisU;
            AxisV = axisV;
            Normal = normal;
        }

        /// <summary>Centro de los extremos de las barras (el plano pasa por aquí).</summary>
        public Vec3 Origin { get; }

        /// <summary>Eje X del mapa (unitario, en mm del modelo).</summary>
        public Vec3 AxisU { get; }

        /// <summary>Eje Y del mapa (unitario, hacia arriba).</summary>
        public Vec3 AxisV { get; }

        /// <summary>Normal del plano de la cercha.</summary>
        public Vec3 Normal { get; }

        /// <summary>Verdadero si todos los extremos están a menos de <see cref="CoplanarToleranceMm"/> del plano.</summary>
        public bool IsCoplanar { get; private set; }

        /// <summary>Lo más que se aparta un extremo de barra del plano elegido.</summary>
        public double MaxPlaneDistanceMm { get; private set; }

        public List<TrussMapSegment> Segments { get; } = new List<TrussMapSegment>();
        public List<TrussMapNode> Nodes { get; } = new List<TrussMapNode>();

        public double MinX { get; private set; } = double.PositiveInfinity;
        public double MinY { get; private set; } = double.PositiveInfinity;
        public double MaxX { get; private set; } = double.NegativeInfinity;
        public double MaxY { get; private set; } = double.NegativeInfinity;

        public bool IsEmpty => Segments.Count == 0 && Nodes.Count == 0;
        public double Width => IsEmpty ? 0.0 : MaxX - MinX;
        public double Height => IsEmpty ? 0.0 : MaxY - MinY;

        public int VisibleCount => Nodes.Count(n => n.VisibleByDefault);
        public int HiddenCount => Nodes.Count - VisibleCount;

        public TrussMapNode? Find(string name) => Nodes.FirstOrDefault(n => string.Equals(n.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>Coordenadas de un punto del modelo en el mapa (mm).</summary>
        public (double X, double Y) Project(Vec3 point) => (point.Dot(AxisU), point.Dot(AxisV));

        /// <summary>Construye el mapa de un plan con las barras del modelo (la selección y las que mencionan las correcciones).</summary>
        public static TrussMap Build(BatchPlan plan, IReadOnlyList<DetectorBar> bars)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            bars ??= new List<DetectorBar>();
            var points = new List<Vec3>(bars.Count * 2);
            foreach (DetectorBar bar in bars)
            {
                points.Add(bar.StartMm);
                points.Add(bar.EndMm);
            }
            if (points.Count == 0)
            {
                foreach (PlanNode node in plan.Nodes) points.Add(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
            }

            Vec3 origin = Vec3.Zero;
            foreach (Vec3 p in points) origin += p;
            if (points.Count > 0) origin = origin * (1.0 / points.Count);

            Vec3 normal = FitNormal(points, origin);
            Vec3 axisV = Perpendicular(Vec3.UnitZ, normal);
            if (axisV.Length < 1e-9) axisV = Perpendicular(Vec3.UnitY, normal);
            if (axisV.Length < 1e-9) axisV = Perpendicular(Vec3.UnitX, normal);
            axisV = axisV.Normalized();
            Vec3 axisU = axisV.Cross(normal).Normalized();
            double alongX = axisU.Dot(Vec3.UnitX);
            if (alongX < -1e-9 || (Math.Abs(alongX) <= 1e-9 && axisU.Dot(Vec3.UnitY) < 0)) axisU = -axisU;

            var map = new TrussMap(origin, axisU, axisV, normal);
            double maxDistance = 0.0;
            foreach (Vec3 p in points) maxDistance = Math.Max(maxDistance, Math.Abs((p - origin).Dot(normal)));
            map.MaxPlaneDistanceMm = Math.Round(maxDistance, 3);
            map.IsCoplanar = maxDistance <= CoplanarToleranceMm;

            var chords = new HashSet<long>(plan.Nodes.Where(n => n.ChordElementId != 0 && n.Status != NodeStatus.Untyped).Select(n => n.ChordElementId));
            foreach (DetectorBar bar in bars)
            {
                var (x0, y0) = map.Project(bar.StartMm);
                var (x1, y1) = map.Project(bar.EndMm);
                map.Segments.Add(new TrussMapSegment(bar.ElementId, Round(x0), Round(y0), Round(x1), Round(y1), bar.TypeName, chords.Contains(bar.ElementId)));
                map.Include(x0, y0);
                map.Include(x1, y1);
            }
            foreach (PlanNode node in plan.Nodes)
            {
                var (x, y) = map.Project(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
                map.Nodes.Add(new TrussMapNode(node, Round(x), Round(y)));
                map.Include(x, y);
            }
            return map;
        }

        private void Include(double x, double y)
        {
            if (x < MinX) MinX = x;
            if (x > MaxX) MaxX = x;
            if (y < MinY) MinY = y;
            if (y > MaxY) MaxY = y;
        }

        private static double Round(double value) => Math.Round(value, 1);

        private static Vec3 Perpendicular(Vec3 vector, Vec3 normal) => vector - normal * vector.Dot(normal);

        /// <summary>
        /// Normal del plano de mínimos cuadrados: el autovector del autovalor más pequeño de la matriz de covarianza de los
        /// puntos. Con los puntos en un plano es exacto; con una sola barra (puntos alineados) se toma el plano vertical que
        /// contiene la recta; sin puntos, el alzado X–Z.
        /// </summary>
        private static Vec3 FitNormal(List<Vec3> points, Vec3 origin)
        {
            if (points.Count < 2) return Vec3.UnitY;
            var m = new double[3, 3];
            foreach (Vec3 p in points)
            {
                Vec3 d = p - origin;
                m[0, 0] += d.X * d.X; m[0, 1] += d.X * d.Y; m[0, 2] += d.X * d.Z;
                m[1, 1] += d.Y * d.Y; m[1, 2] += d.Y * d.Z;
                m[2, 2] += d.Z * d.Z;
            }
            m[1, 0] = m[0, 1];
            m[2, 0] = m[0, 2];
            m[2, 1] = m[1, 2];
            double scale = m[0, 0] + m[1, 1] + m[2, 2];
            if (scale < 1e-12) return Vec3.UnitY;

            double[] values = Jacobi(m, out double[,] vectors);
            int smallest = 0;
            int largest = 0;
            for (int i = 1; i < 3; i++)
            {
                if (values[i] < values[smallest]) smallest = i;
                if (values[i] > values[largest]) largest = i;
            }
            int middle = 3 - smallest - largest;
            if (middle < 0 || middle > 2 || middle == smallest) middle = (smallest + 1) % 3;
            Vec3 normal = new Vec3(vectors[0, smallest], vectors[1, smallest], vectors[2, smallest]);
            // Puntos alineados (una sola barra o barras colineales): dos autovalores nulos; el plano vertical que contiene la recta.
            if (values[middle] <= 1e-9 * scale)
            {
                Vec3 direction = new Vec3(vectors[0, largest], vectors[1, largest], vectors[2, largest]).Normalized();
                Vec3 candidate = direction.Cross(Vec3.UnitZ);
                normal = candidate.Length > 1e-9 ? candidate : direction.Cross(Vec3.UnitX);
            }
            normal = normal.Normalized();
            // Normal con sentido estable (hacia +Y, o +X, o +Z): el mapa no depende del orden de las barras.
            if (normal.Y < -1e-9 || (Math.Abs(normal.Y) <= 1e-9 && (normal.X < -1e-9 || (Math.Abs(normal.X) <= 1e-9 && normal.Z < 0)))) normal = -normal;
            return normal;
        }

        /// <summary>Autovalores y autovectores (columnas) de una matriz simétrica 3×3 por rotaciones de Jacobi.</summary>
        private static double[] Jacobi(double[,] a, out double[,] v)
        {
            v = new double[3, 3];
            for (int i = 0; i < 3; i++) v[i, i] = 1.0;
            for (int sweep = 0; sweep < 60; sweep++)
            {
                double off = a[0, 1] * a[0, 1] + a[0, 2] * a[0, 2] + a[1, 2] * a[1, 2];
                double diag = a[0, 0] * a[0, 0] + a[1, 1] * a[1, 1] + a[2, 2] * a[2, 2];
                if (off <= 1e-24 * Math.Max(diag, 1e-300)) break;
                for (int p = 0; p < 2; p++)
                {
                    for (int q = p + 1; q < 3; q++)
                    {
                        double apq = a[p, q];
                        if (Math.Abs(apq) < 1e-300) continue;
                        double theta = (a[q, q] - a[p, p]) / (2.0 * apq);
                        double t = theta == 0.0 ? 1.0 : Math.Sign(theta) / (Math.Abs(theta) + Math.Sqrt(theta * theta + 1.0));
                        double c = 1.0 / Math.Sqrt(t * t + 1.0);
                        double s = t * c;
                        for (int k = 0; k < 3; k++)
                        {
                            double akp = a[k, p];
                            double akq = a[k, q];
                            a[k, p] = c * akp - s * akq;
                            a[k, q] = s * akp + c * akq;
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            double apk = a[p, k];
                            double aqk = a[q, k];
                            a[p, k] = c * apk - s * aqk;
                            a[q, k] = s * apk + c * aqk;
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            double vkp = v[k, p];
                            double vkq = v[k, q];
                            v[k, p] = c * vkp - s * vkq;
                            v[k, q] = s * vkp + c * vkq;
                        }
                    }
                }
            }
            return new[] { a[0, 0], a[1, 1], a[2, 2] };
        }
    }
}
