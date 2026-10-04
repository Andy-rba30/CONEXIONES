using System;
using System.Collections.Generic;
using System.Globalization;
using MotorConexiones.Core.Geometry2D;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>Capa de una línea del croquis; la ventana elige trazo y color por capa.</summary>
    public enum SketchLineKind
    {
        /// <summary>Eje de una barra (trazo y punto).</summary>
        Axis,
        /// <summary>Arista visible.</summary>
        Outline,
        /// <summary>Arista oculta (ranura dentro del HSS): trazo discontinuo.</summary>
        Hidden,
        /// <summary>Marca de soldadura.</summary>
        Weld,
        /// <summary>Cruz del punto de trabajo.</summary>
        WorkPoint,
    }

    /// <summary>Capa de un polígono del croquis.</summary>
    public enum SketchPolygonKind
    {
        /// <summary>Cuerpo del cordón o de una barra (ancho del perfil).</summary>
        Member,
        /// <summary>Cartela.</summary>
        Gusset,
        /// <summary>Placa cuchilla.</summary>
        KnifePlate,
    }

    /// <summary>Capa de un círculo del croquis.</summary>
    public enum SketchCircleKind
    {
        /// <summary>Perno (radio = diámetro / 2).</summary>
        Bolt,
    }

    /// <summary>Segmento en mm, sistema local del nudo (X a lo largo del cordón, Y perpendicular en el plano).</summary>
    public sealed class SketchLine
    {
        public SketchLine(Point2D start, Point2D end, SketchLineKind kind, string? path = null, string? label = null)
        {
            Start = start;
            End = end;
            Kind = kind;
            Path = path;
            Label = label;
        }

        public Point2D Start { get; }
        public Point2D End { get; }
        public SketchLineKind Kind { get; }

        /// <summary>Ruta JSON del campo que representa (por ejemplo <c>members[0].attachment.slot_length_mm</c>), si lo hay.</summary>
        public string? Path { get; }

        /// <summary>Texto corto junto a la línea (tamaño de soldadura, etc.).</summary>
        public string? Label { get; }
    }

    /// <summary>Polígono cerrado en mm.</summary>
    public sealed class SketchPolygon
    {
        public SketchPolygon(IReadOnlyList<Point2D> points, SketchPolygonKind kind, string? path = null, string? label = null)
        {
            Points = points ?? throw new ArgumentNullException(nameof(points));
            Kind = kind;
            Path = path;
            Label = label;
        }

        public IReadOnlyList<Point2D> Points { get; }
        public SketchPolygonKind Kind { get; }
        public string? Path { get; }
        public string? Label { get; }
    }

    /// <summary>Círculo en mm.</summary>
    public sealed class SketchCircle
    {
        public SketchCircle(Point2D center, double radiusMm, SketchCircleKind kind, string? path = null)
        {
            Center = center;
            RadiusMm = radiusMm;
            Kind = kind;
            Path = path;
        }

        public Point2D Center { get; }
        public double RadiusMm { get; }
        public SketchCircleKind Kind { get; }
        public string? Path { get; }
    }

    /// <summary>
    /// Cota lineal entre dos puntos. La línea de cota se dibuja desplazada <see cref="OffsetMm"/> hacia la izquierda
    /// de la dirección <see cref="Start"/> → <see cref="End"/> (normal izquierda = (-dy, dx)); un desplazamiento negativo
    /// la pone a la derecha. El texto ya viene formateado en mm con una cifra decimal.
    /// </summary>
    public sealed class SketchDimension
    {
        public SketchDimension(Point2D start, Point2D end, double offsetMm, double valueMm, string? path = null, string? prefix = null)
        {
            Start = start;
            End = end;
            OffsetMm = offsetMm;
            ValueMm = valueMm;
            Path = path;
            Text = (string.IsNullOrEmpty(prefix) ? "" : prefix + " ") + SketchFormat.Mm(valueMm);
        }

        public Point2D Start { get; }
        public Point2D End { get; }
        public double OffsetMm { get; }

        /// <summary>Valor medido, en mm (sin redondear).</summary>
        public double ValueMm { get; }

        /// <summary>Texto de la cota, por ejemplo <c>565,0</c> o <c>ranura 150,0</c>.</summary>
        public string Text { get; }

        /// <summary>Ruta JSON del campo que mide, si lo hay.</summary>
        public string? Path { get; }

        /// <summary>Longitud real entre los dos puntos (para comprobar que la cota mide lo que dice).</summary>
        public double MeasuredLengthMm => Start.DistanceTo(End);
    }

    /// <summary>Texto suelto (espesor de una placa, rol y perfil de una barra, "PT").</summary>
    public sealed class SketchLabel
    {
        public SketchLabel(Point2D position, string text, string? path = null, bool emphasized = false)
        {
            Position = position;
            Text = text ?? "";
            Path = path;
            Emphasized = emphasized;
        }

        public Point2D Position { get; }
        public string Text { get; }
        public string? Path { get; }
        public bool Emphasized { get; }
    }

    /// <summary>Croquis completo: listas de primitivas en mm y su caja envolvente. La ventana solo lo dibuja.</summary>
    public sealed class SketchModel
    {
        public List<SketchLine> Lines { get; } = new List<SketchLine>();
        public List<SketchPolygon> Polygons { get; } = new List<SketchPolygon>();
        public List<SketchCircle> Circles { get; } = new List<SketchCircle>();
        public List<SketchDimension> Dimensions { get; } = new List<SketchDimension>();
        public List<SketchLabel> Labels { get; } = new List<SketchLabel>();

        /// <summary>Avisos del propio croquis (datos que faltaban y se dibujaron de forma esquemática).</summary>
        public List<string> Notes { get; } = new List<string>();

        /// <summary>Verdadero cuando las direcciones de las barras no salieron del modelo sino de los ángulos del plano.</summary>
        public bool IsSchematic { get; set; }

        public Point2D BoundsMin { get; private set; }
        public Point2D BoundsMax { get; private set; }
        public bool IsEmpty => Lines.Count == 0 && Polygons.Count == 0 && Circles.Count == 0;

        /// <summary>Recalcula la caja envolvente con todas las primitivas (incluidas las líneas de cota desplazadas).</summary>
        public void ComputeBounds()
        {
            double minX = double.PositiveInfinity, minY = double.PositiveInfinity;
            double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity;

            void Include(Point2D p)
            {
                if (p.X < minX) minX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.X > maxX) maxX = p.X;
                if (p.Y > maxY) maxY = p.Y;
            }

            foreach (var l in Lines) { Include(l.Start); Include(l.End); }
            foreach (var poly in Polygons) foreach (var p in poly.Points) Include(p);
            foreach (var c in Circles)
            {
                Include(new Point2D(c.Center.X - c.RadiusMm, c.Center.Y - c.RadiusMm));
                Include(new Point2D(c.Center.X + c.RadiusMm, c.Center.Y + c.RadiusMm));
            }
            foreach (var d in Dimensions)
            {
                Include(d.Start);
                Include(d.End);
                var (a, b) = d.DimensionLine();
                Include(a);
                Include(b);
            }
            foreach (var lab in Labels) Include(lab.Position);

            if (double.IsInfinity(minX))
            {
                minX = minY = -100;
                maxX = maxY = 100;
            }
            BoundsMin = new Point2D(minX, minY);
            BoundsMax = new Point2D(maxX, maxY);
        }
    }

    /// <summary>Formato de los textos del croquis: mm con una cifra decimal y coma decimal (como en el plano).</summary>
    public static class SketchFormat
    {
        public static string Mm(double valueMm)
        {
            return valueMm.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',');
        }

        public static string Degrees(double degrees)
        {
            return degrees.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',') + "°";
        }
    }

    public static class SketchDimensionExtensions
    {
        /// <summary>Extremos de la línea de cota desplazada (en mm).</summary>
        public static (Point2D A, Point2D B) DimensionLine(this SketchDimension d)
        {
            double dx = d.End.X - d.Start.X;
            double dy = d.End.Y - d.Start.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-9) return (d.Start, d.End);
            double nx = -dy / len;
            double ny = dx / len;
            var a = new Point2D(d.Start.X + nx * d.OffsetMm, d.Start.Y + ny * d.OffsetMm);
            var b = new Point2D(d.End.X + nx * d.OffsetMm, d.End.Y + ny * d.OffsetMm);
            return (a, b);
        }
    }
}
