using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Sketch;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>Una barra tal como la ve el cálculo de la cartela automática (Fase 13): su extremo real en el marco canónico.</summary>
    public sealed class AutoOutlineMember
    {
        public long ElementId { get; set; }

        /// <summary>Dirección unitaria en el plano local, del punto de trabajo hacia fuera.</summary>
        public double Ux { get; set; }
        public double Uy { get; set; }

        /// <summary>Dónde corta el eje de la barra al eje del cordón, desde el punto de trabajo a lo largo de +X (0 si concurre).</summary>
        public double AxisOffsetMm { get; set; }

        /// <summary>Lado del perfil en el plano de la cercha.</summary>
        public double WidthMm { get; set; } = SketchNodeInfo.DefaultMemberWidthMm;

        public double EndSetbackMm { get; set; }

        /// <summary>Verdadero con placa cuchilla atornillada; falso con ranura soldada.</summary>
        public bool IsKnifePlate { get; set; }

        public double SlotLengthMm { get; set; } = 150.0;
        public double PlateLengthMm { get; set; } = 170.0;
        public double PlateWidthMm { get; set; } = 140.0;
        public double PlateInsertionMm { get; set; } = 80.0;
    }

    /// <summary>Lo que necesita <see cref="AutoOutline.Compute"/>: las barras, el cordón y los márgenes (decisión D4 de la Fase 13).</summary>
    public sealed class AutoOutlineInput
    {
        public List<AutoOutlineMember> Members { get; } = new List<AutoOutlineMember>();

        /// <summary>Ancho del cordón en el plano de la cercha.</summary>
        public double ChordWidthMm { get; set; } = SketchNodeInfo.DefaultChordWidthMm;

        /// <summary><c>through_slot</c>, <c>split_top_bottom</c> o <c>side_lap</c>.</summary>
        public string? ChordInterface { get; set; }

        /// <summary>Del final de la ranura (o del extremo libre de la placa cuchilla) al borde de la cartela.</summary>
        public double MarginMm { get; set; } = AutoOutline.DefaultMarginMm;

        /// <summary>Del lado de la barra (o de la placa) al borde de la cartela.</summary>
        public double SideMarginMm { get; set; } = AutoOutline.DefaultSideMarginMm;

        /// <summary>Cuánto asoma la cartela más allá de la cara del cordón.</summary>
        public double BeyondChordMm { get; set; } = AutoOutline.DefaultBeyondChordMm;

        public int MaxVertices { get; set; } = AutoOutline.DefaultMaxVertices;
        public double RoundingMm { get; set; } = AutoOutline.RoundingMm;
        public double MinTurnDeg { get; set; } = AutoOutline.MinTurnDeg;
    }

    /// <summary>El contorno calculado: los puntos en el marco canónico (antihorario), su caja y las medidas que vigilan <c>OUTLINE_AUTO_LARGE</c>.</summary>
    public sealed class AutoOutlineResult
    {
        public AutoOutlineResult(List<Point2D> vertices, double rectanglesAreaMm2)
        {
            Vertices = vertices;
            RectanglesAreaMm2 = rectanglesAreaMm2;
            var polygon = new Polygon2D(vertices);
            AreaMm2 = polygon.Area;
            var (min, max) = polygon.GetBoundingBox();
            MinX = min.X;
            MinY = min.Y;
            MaxX = max.X;
            MaxY = max.Y;
        }

        public List<Point2D> Vertices { get; }
        public int VertexCount => Vertices.Count;
        public double MinX { get; }
        public double MinY { get; }
        public double MaxX { get; }
        public double MaxY { get; }
        public double WidthMm => MaxX - MinX;
        public double HeightMm => MaxY - MinY;
        public double AreaMm2 { get; }

        /// <summary>Suma de las áreas de los rectángulos de las barras y de la franja del cordón (lo que de verdad hace falta cubrir).</summary>
        public double RectanglesAreaMm2 { get; }

        /// <summary>Área del contorno dividida por la de los rectángulos (más de 2 = una barra muy lejos o un margen absurdo).</summary>
        public double AreaRatio => RectanglesAreaMm2 > 1e-6 ? AreaMm2 / RectanglesAreaMm2 : double.PositiveInfinity;

        /// <summary>Caja del contorno como [minX, minY, maxX, maxY].</summary>
        public double[] BoundsMm => new[] { MinX, MinY, MaxX, MaxY };

        /// <summary><c>points_mm</c> listo para el JSON.</summary>
        public List<double[]> PointsMm => Vertices.Select(v => new[] { v.X, v.Y }).ToList();

        public Polygon2D ToPolygon() => new Polygon2D(Vertices);

        /// <summary>Verdadero si la caja supera <paramref name="maxBoxMm"/> o el área es más del doble de los rectángulos (aviso <c>OUTLINE_AUTO_LARGE</c>).</summary>
        public bool IsLarge(double maxBoxMm) => WidthMm > maxBoxMm + 1e-6 || HeightMm > maxBoxMm + 1e-6 || AreaRatio > 2.0 + 1e-9;
    }

    /// <summary>
    /// Fase 13 (decisión D1): la cartela automática (<c>gusset.outline.mode = "auto"</c>). Envuelve los <b>extremos reales</b> de
    /// las barras (retiro, ranura o placa cuchilla, ancho, desvío de eje) y la franja del cordón con los márgenes de
    /// <c>choices.json</c> (D4: 40 / 30 / 40), toma la <b>envolvente convexa</b>, la redondea a 5 mm hacia fuera y la simplifica
    /// (sin giros menores de 3°, como máximo 12 vértices). Es determinista y puro Core: lo mismo en el validador, el croquis, la
    /// fabricación y el simulador. Nunca escribe los puntos en <c>points_mm</c>: eso solo lo hace Convertir en polígono.
    /// </summary>
    public static class AutoOutline
    {
        public const string Mode = "auto";
        public const string PolygonMode = "polygon";
        public const double DefaultMarginMm = 40.0;
        public const double DefaultSideMarginMm = 30.0;
        public const double DefaultBeyondChordMm = 40.0;
        public const int DefaultMaxVertices = 12;
        public const double RoundingMm = 5.0;
        public const double MinTurnDeg = 3.0;

        /// <summary>Verdadero si el contorno pide la cartela automática.</summary>
        public static bool IsAuto(GussetOutline? outline) =>
            outline != null && string.Equals(outline.Mode, Mode, StringComparison.OrdinalIgnoreCase);

        public static bool IsAuto(ConnectionSpec? spec) => IsAuto(spec?.Gusset?.Outline);

        /// <summary>
        /// Los datos del cálculo a partir de la especificación y del nudo leído (<see cref="SketchNodeInfo"/>: direcciones,
        /// desvíos y anchos del modelo, o aproximados por los ángulos del JSON). Los márgenes salen del contorno si los trae
        /// y, si no, de los valores por defecto (los de <c>choices.json</c>).
        /// </summary>
        public static AutoOutlineInput InputFrom(ConnectionSpec spec, SketchNodeInfo nodeInfo, int? maxVertices = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (nodeInfo == null) throw new ArgumentNullException(nameof(nodeInfo));
            GussetOutline? outline = spec.Gusset?.Outline;
            var input = new AutoOutlineInput
            {
                ChordWidthMm = nodeInfo.ChordWidthMm > 0 ? nodeInfo.ChordWidthMm : SketchNodeInfo.DefaultChordWidthMm,
                ChordInterface = spec.Gusset?.ChordInterface,
                MarginMm = Positive(outline?.MarginMm, DefaultMarginMm),
                SideMarginMm = Positive(outline?.SideMarginMm, DefaultSideMarginMm),
                BeyondChordMm = Positive(outline?.BeyondChordMm, DefaultBeyondChordMm),
                MaxVertices = maxVertices.HasValue && maxVertices.Value >= 3 ? maxVertices.Value : DefaultMaxVertices,
            };
            if (spec.Members == null) return input;
            for (int i = 0; i < spec.Members.Count; i++)
            {
                MemberSpec member = spec.Members[i];
                SketchMemberInfo? info = nodeInfo.Find(member.ElementId);
                if (info == null && i < nodeInfo.Members.Count) info = nodeInfo.Members[i];
                if (info == null) info = SketchNodeInfo.FromSpecAngles(spec).Members[i];
                AttachmentSpec? attachment = member.Attachment;
                bool knife = string.Equals(attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase);
                input.Members.Add(new AutoOutlineMember
                {
                    ElementId = member.ElementId,
                    Ux = info.Ux,
                    Uy = info.Uy,
                    AxisOffsetMm = info.AxisOffsetMm,
                    WidthMm = info.WidthMm > 0 ? info.WidthMm : SketchNodeInfo.DefaultMemberWidthMm,
                    EndSetbackMm = Math.Max(0.0, member.EndSetbackMm ?? 0.0),
                    IsKnifePlate = knife,
                    SlotLengthMm = attachment?.SlotLengthMm ?? 150.0,
                    PlateLengthMm = attachment?.Plate?.LengthMm ?? 170.0,
                    PlateWidthMm = attachment?.Plate?.WidthMm ?? 140.0,
                    PlateInsertionMm = attachment?.Plate?.InsertionMm ?? 80.0,
                });
            }
            return input;
        }

        /// <summary>El contorno calculado para una especificación en modo <c>auto</c> (nulo si no lo es o no tiene barras).</summary>
        public static AutoOutlineResult? Resolve(ConnectionSpec spec, IModelFacts? facts, int? maxVertices = null)
        {
            if (spec == null || !IsAuto(spec) || spec.Members == null || spec.Members.Count == 0) return null;
            SketchNodeInfo nodeInfo;
            try
            {
                nodeInfo = SketchNodeInfo.FromModelFacts(spec, facts);
            }
            catch (Exception)
            {
                nodeInfo = SketchNodeInfo.FromSpecAngles(spec);
            }
            return Compute(InputFrom(spec, nodeInfo, maxVertices));
        }

        /// <summary>El contorno calculado con un nudo ya leído (el croquis).</summary>
        public static AutoOutlineResult? Resolve(ConnectionSpec spec, SketchNodeInfo nodeInfo, int? maxVertices = null)
        {
            if (spec == null || !IsAuto(spec) || spec.Members == null || spec.Members.Count == 0) return null;
            return Compute(InputFrom(spec, nodeInfo, maxVertices));
        }

        /// <summary>
        /// Los puntos del contorno con los que se valida, se dibuja y se fabrica: los escritos (<c>polygon</c>) o los
        /// calculados (<c>auto</c>). Lista vacía si no hay contorno.
        /// </summary>
        public static List<double[]> ResolvePoints(ConnectionSpec spec, IModelFacts? facts, int? maxVertices = null)
        {
            if (spec?.Gusset?.Outline == null) return new List<double[]>();
            if (IsAuto(spec))
            {
                return Resolve(spec, facts, maxVertices)?.PointsMm ?? new List<double[]>();
            }
            return (spec.Gusset.Outline.PointsMm ?? new List<double[]>()).Where(p => p != null && p.Length >= 2).ToList();
        }

        /// <summary>El cálculo (sección 3.2 del prompt de la Fase 13), determinista.</summary>
        public static AutoOutlineResult Compute(AutoOutlineInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var points = new List<Point2D>();
            double rectangles = 0.0;
            double minX = double.MaxValue, maxX = double.MinValue;
            int side = 0;
            bool bothSides = false;

            // 1. El rectángulo de cada barra sobre su eje real.
            foreach (AutoOutlineMember member in input.Members)
            {
                double ux = member.Ux, uy = member.Uy;
                double length = Math.Sqrt(ux * ux + uy * uy);
                if (length < 1e-9)
                {
                    ux = 1.0;
                    uy = 0.0;
                }
                else
                {
                    ux /= length;
                    uy /= length;
                }
                double vx = -uy, vy = ux;
                double near, far, half;
                if (member.IsKnifePlate)
                {
                    double exposed = Math.Max(0.0, member.PlateLengthMm - member.PlateInsertionMm);
                    near = member.EndSetbackMm - exposed - input.MarginMm;
                    far = member.EndSetbackMm + input.MarginMm;
                    half = Math.Max(member.WidthMm, member.PlateWidthMm) / 2.0 + input.SideMarginMm;
                }
                else
                {
                    near = member.EndSetbackMm;
                    far = member.EndSetbackMm + member.SlotLengthMm + input.MarginMm;
                    half = member.WidthMm / 2.0 + input.SideMarginMm;
                }
                if (far < near) far = near;
                double cx = member.AxisOffsetMm;
                foreach (double along in new[] { near, far })
                {
                    foreach (double across in new[] { half, -half })
                    {
                        var p = new Point2D(cx + along * ux + across * vx, along * uy + across * vy);
                        points.Add(p);
                        minX = Math.Min(minX, p.X);
                        maxX = Math.Max(maxX, p.X);
                    }
                }
                rectangles += (far - near) * 2.0 * half;
                int memberSide = uy >= 0 ? 1 : -1;
                if (side == 0) side = memberSide;
                else if (side != memberSide) bothSides = true;
            }
            if (points.Count == 0)
            {
                minX = -input.ChordWidthMm;
                maxX = input.ChordWidthMm;
            }

            // 2. La franja del cordón: de extremo a extremo de los rectángulos; a los dos lados con through_slot (y con barras a los
            //    dos lados), solo al lado de las barras con split_top_bottom y side_lap (D1).
            double chordHalf = (input.ChordWidthMm > 0 ? input.ChordWidthMm : SketchNodeInfo.DefaultChordWidthMm) / 2.0;
            double beyond = chordHalf + Math.Max(0.0, input.BeyondChordMm);
            bool through = string.IsNullOrWhiteSpace(input.ChordInterface) || string.Equals(input.ChordInterface, "through_slot", StringComparison.OrdinalIgnoreCase);
            double yTop = through || bothSides || side >= 0 ? beyond : 0.0;
            double yBottom = through || bothSides || side <= 0 ? -beyond : 0.0;
            points.Add(new Point2D(minX, yBottom));
            points.Add(new Point2D(maxX, yBottom));
            points.Add(new Point2D(maxX, yTop));
            points.Add(new Point2D(minX, yTop));
            rectangles += (maxX - minX) * (yTop - yBottom);

            // 3. Envolvente convexa, redondeo a 5 mm hacia fuera y simplificación (sin giros menores de 3°, tope de vértices).
            List<Point2D> hull = Polygon2D.ConvexHull(points);
            hull = Polygon2D.ConvexHull(hull.Select(p => RoundOutward(p, input.RoundingMm)));
            hull = Polygon2D.Simplify(hull, input.MinTurnDeg, input.MaxVertices);
            hull = Polygon2D.ConvexHull(hull.Select(p => RoundOutward(p, input.RoundingMm)));
            if (hull.Count > Math.Max(3, input.MaxVertices)) hull = Polygon2D.Simplify(hull, 0.0, input.MaxVertices);
            hull = Polygon2D.ConvexHull(hull.Select(p => RoundOutward(p, input.RoundingMm)));
            return new AutoOutlineResult(hull, rectangles);
        }

        /// <summary>Redondea una coordenada al múltiplo de <paramref name="stepMm"/> más alejado del origen (hacia fuera).</summary>
        public static double RoundOutward(double value, double stepMm)
        {
            if (stepMm <= 0) return value;
            double units = value / stepMm;
            double rounded = value >= 0 ? Math.Ceiling(units - 1e-9) : Math.Floor(units + 1e-9);
            return Math.Round(rounded * stepMm, 6);
        }

        public static Point2D RoundOutward(Point2D p, double stepMm) => new Point2D(RoundOutward(p.X, stepMm), RoundOutward(p.Y, stepMm));

        private static double Positive(double? value, double fallback) => value.HasValue && value.Value >= 0 && !double.IsNaN(value.Value) ? value.Value : fallback;
    }
}
