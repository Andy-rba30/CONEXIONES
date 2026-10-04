using System;
using System.Collections.Generic;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Qué representa cada trazo del croquis. La ventana elige color y grosor por este valor; Core no sabe de píxeles.
    /// </summary>
    public enum SketchKind
    {
        /// <summary>Eje del cordón o de una barra (línea de centro, discontinua).</summary>
        Axis,
        /// <summary>Borde del cordón.</summary>
        ChordEdge,
        /// <summary>Contorno visible de una barra (bordes y cara del extremo tras el retiro).</summary>
        MemberOutline,
        /// <summary>Contorno de la cartela.</summary>
        Gusset,
        /// <summary>Placa cuchilla.</summary>
        KnifePlate,
        /// <summary>Ranura del HSS por donde pasa la cartela.</summary>
        Slot,
        /// <summary>Perno (círculo con su diámetro).</summary>
        Bolt,
        /// <summary>Línea de soldadura.</summary>
        Weld,
        /// <summary>Marca del punto de trabajo (origen del sistema local).</summary>
        WorkPoint,
        /// <summary>Cota.</summary>
        Dimension,
        /// <summary>Texto informativo (perfil, espesor, soldadura, leyenda).</summary>
        Label,
    }

    /// <summary>Qué mide una cota. Las pruebas comprueban los valores por este campo.</summary>
    public enum DimensionKind
    {
        GussetWidth,
        GussetHeight,
        MemberSetback,
        SlotLength,
        PlateLength,
        PlateWidth,
        BoltSpacing,
        BoltEdge,
        BoltFirstRow,
    }

    /// <summary>Punto del croquis en el sistema local del nudo, en mm.</summary>
    public readonly struct SketchPoint
    {
        public SketchPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public double DistanceTo(SketchPoint other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static SketchPoint operator +(SketchPoint a, SketchPoint b) => new SketchPoint(a.X + b.X, a.Y + b.Y);
        public static SketchPoint operator -(SketchPoint a, SketchPoint b) => new SketchPoint(a.X - b.X, a.Y - b.Y);
        public static SketchPoint operator *(SketchPoint a, double k) => new SketchPoint(a.X * k, a.Y * k);

        public override string ToString() => string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0:0.#}, {1:0.#})", X, Y);
    }

    /// <summary>Segmento recto.</summary>
    public sealed class SketchLine
    {
        public SketchLine(SketchPoint start, SketchPoint end, SketchKind kind)
        {
            Start = start;
            End = end;
            Kind = kind;
        }

        public SketchPoint Start { get; }
        public SketchPoint End { get; }
        public SketchKind Kind { get; }
    }

    /// <summary>Polilínea; cerrada si <see cref="IsClosed"/> (cartela, placa, ranura) o abierta (cuerpo de una barra).</summary>
    public sealed class SketchPolygon
    {
        public SketchPolygon(IReadOnlyList<SketchPoint> points, SketchKind kind, bool isClosed = true)
        {
            Points = points ?? throw new ArgumentNullException(nameof(points));
            Kind = kind;
            IsClosed = isClosed;
        }

        public IReadOnlyList<SketchPoint> Points { get; }
        public SketchKind Kind { get; }
        public bool IsClosed { get; }
    }

    /// <summary>Círculo (pernos).</summary>
    public sealed class SketchCircle
    {
        public SketchCircle(SketchPoint center, double radiusMm, SketchKind kind)
        {
            Center = center;
            RadiusMm = radiusMm;
            Kind = kind;
        }

        public SketchPoint Center { get; }
        public double RadiusMm { get; }
        public SketchKind Kind { get; }
    }

    /// <summary>
    /// Cota lineal: mide de <see cref="Start"/> a <see cref="End"/>. La línea de cota va desplazada
    /// <see cref="LineOffsetMm"/> en la normal izquierda del segmento (normal = (-dy, dx) normalizada); las líneas de
    /// referencia unen los puntos medidos con la línea de cota. <see cref="Text"/> es el texto que se dibuja
    /// (mm con una cifra decimal) y <see cref="Path"/> la ruta JSON del campo al que corresponde, si hay uno.
    /// </summary>
    public sealed class SketchDimension
    {
        public SketchDimension(SketchPoint start, SketchPoint end, double lineOffsetMm, double valueMm, string text, DimensionKind kind, string? path)
        {
            Start = start;
            End = end;
            LineOffsetMm = lineOffsetMm;
            ValueMm = valueMm;
            Text = text ?? string.Empty;
            Kind = kind;
            Path = path;
        }

        public SketchPoint Start { get; }
        public SketchPoint End { get; }
        public double LineOffsetMm { get; }
        public double ValueMm { get; }
        public string Text { get; }
        public DimensionKind Kind { get; }
        public string? Path { get; }

        /// <summary>Extremos de la línea de cota (ya desplazada).</summary>
        public (SketchPoint Start, SketchPoint End) GetDimensionLine()
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length < 1e-9) return (Start, End);
            var normal = new SketchPoint(-dy / length, dx / length);
            return (Start + normal * LineOffsetMm, End + normal * LineOffsetMm);
        }
    }

    /// <summary>Texto anclado a un punto. <see cref="Anchor"/>: 0 = izquierda, 1 = centrado, 2 = derecha.</summary>
    public sealed class SketchLabel
    {
        public SketchLabel(SketchPoint position, string text, SketchKind kind = SketchKind.Label, int anchor = 0, string? path = null)
        {
            Position = position;
            Text = text ?? string.Empty;
            Kind = kind;
            Anchor = anchor;
            Path = path;
        }

        public SketchPoint Position { get; }
        public string Text { get; }
        public SketchKind Kind { get; }
        public int Anchor { get; }
        public string? Path { get; }
    }

    /// <summary>Caja envolvente en mm.</summary>
    public readonly struct SketchBounds
    {
        public SketchBounds(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public double MinX { get; }
        public double MinY { get; }
        public double MaxX { get; }
        public double MaxY { get; }
        public double Width => MaxX - MinX;
        public double Height => MaxY - MinY;
        public bool IsEmpty => MaxX < MinX || MaxY < MinY;

        public static SketchBounds Empty => new SketchBounds(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

        public SketchBounds Include(SketchPoint p) => new SketchBounds(
            Math.Min(MinX, p.X), Math.Min(MinY, p.Y), Math.Max(MaxX, p.X), Math.Max(MaxY, p.Y));

        public SketchBounds Inflate(double marginMm) => IsEmpty ? this : new SketchBounds(MinX - marginMm, MinY - marginMm, MaxX + marginMm, MaxY + marginMm);
    }

    /// <summary>
    /// Croquis 2D completo del nudo en el sistema local (mm). Solo datos: la ventana lo dibuja, Core lo construye y
    /// las pruebas lo comprueban sin Revit ni WPF.
    /// </summary>
    public sealed class Sketch
    {
        public List<SketchLine> Lines { get; } = new List<SketchLine>();
        public List<SketchPolygon> Polygons { get; } = new List<SketchPolygon>();
        public List<SketchCircle> Circles { get; } = new List<SketchCircle>();
        public List<SketchDimension> Dimensions { get; } = new List<SketchDimension>();
        public List<SketchLabel> Labels { get; } = new List<SketchLabel>();

        /// <summary>Avisos del propio croquis (por ejemplo, direcciones aproximadas sin modelo).</summary>
        public List<string> Notes { get; } = new List<string>();

        public int PieceCount => Lines.Count + Polygons.Count + Circles.Count + Dimensions.Count + Labels.Count;

        /// <summary>Caja envolvente de todo lo dibujado (incluidas las líneas de cota; los textos no se miden).</summary>
        public SketchBounds GetBounds()
        {
            SketchBounds bounds = SketchBounds.Empty;
            foreach (var line in Lines)
            {
                bounds = bounds.Include(line.Start).Include(line.End);
            }
            foreach (var polygon in Polygons)
            {
                foreach (var point in polygon.Points) bounds = bounds.Include(point);
            }
            foreach (var circle in Circles)
            {
                bounds = bounds.Include(new SketchPoint(circle.Center.X - circle.RadiusMm, circle.Center.Y - circle.RadiusMm));
                bounds = bounds.Include(new SketchPoint(circle.Center.X + circle.RadiusMm, circle.Center.Y + circle.RadiusMm));
            }
            foreach (var dimension in Dimensions)
            {
                var (start, end) = dimension.GetDimensionLine();
                bounds = bounds.Include(dimension.Start).Include(dimension.End).Include(start).Include(end);
            }
            foreach (var label in Labels)
            {
                bounds = bounds.Include(label.Position);
            }
            return bounds;
        }
    }
}
