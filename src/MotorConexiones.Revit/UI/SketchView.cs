using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Sketch;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Cota o rótulo del croquis sobre el que la persona hizo doble clic (Fase 6b: edición desde el dibujo).</summary>
    public sealed class SketchHitEventArgs : EventArgs
    {
        public SketchHitEventArgs(string path, string text, Point position)
        {
            Path = path;
            Text = text;
            Position = position;
        }

        /// <summary>Ruta JSON del campo que mide la cota (la misma de la tabla).</summary>
        public string Path { get; }

        /// <summary>Texto dibujado (por ejemplo <c>placa 170,0</c>).</summary>
        public string Text { get; }

        /// <summary>Posición del clic, en píxeles del control.</summary>
        public Point Position { get; }
    }

    /// <summary>
    /// Dibuja un <see cref="SketchModel"/> (mm, sistema local del nudo: X a la derecha, Y hacia arriba) en WPF.
    /// Zoom con la rueda alrededor del cursor, encuadre arrastrando con el botón central y <see cref="Fit"/> para
    /// ajustar. El factor mm → píxel es solo de pantalla: no es una conversión de unidades del contrato.
    /// Doble clic sobre el texto de una cota o de un rótulo con ruta JSON: dispara <see cref="HitActivated"/> para que la
    /// ventana abra el editor de ese valor (Fase 6b).
    /// </summary>
    public sealed class SketchView : FrameworkElement
    {
        /// <summary>Zona de pantalla ocupada por un texto editable (se rellena en cada OnRender).</summary>
        private sealed class HitTarget
        {
            public HitTarget(Rect bounds, string path, string text)
            {
                Bounds = bounds;
                Path = path;
                Text = text;
            }

            public Rect Bounds { get; }
            public string Path { get; }
            public string Text { get; }
        }

        private const double HitToleranceEmPixels = 6.0;
        private readonly List<HitTarget> _hitTargets = new List<HitTarget>();
        private HitTarget? _hover;

        /// <summary>Doble clic sobre una cota o un rótulo editable.</summary>
        public event EventHandler<SketchHitEventArgs>? HitActivated;

        private static readonly Brush PaperBrush = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xF7));
        private static readonly Pen AxisPen = MakePen(Color.FromRgb(0x70, 0x70, 0x70), 1.0, new DoubleCollection { 8, 3, 2, 3 });
        private static readonly Pen MemberPen = MakePen(Color.FromRgb(0x30, 0x30, 0x30), 1.4, null);
        private static readonly Brush MemberFill = new SolidColorBrush(Color.FromArgb(0x22, 0x60, 0x60, 0x60));
        private static readonly Pen GussetPen = MakePen(Color.FromRgb(0x1F, 0x4E, 0x9A), 2.0, null);
        private static readonly Brush GussetFill = new SolidColorBrush(Color.FromArgb(0x33, 0x4A, 0x90, 0xE2));
        private static readonly Pen KnifePen = MakePen(Color.FromRgb(0x9A, 0x4E, 0x1F), 1.8, null);
        private static readonly Brush KnifeFill = new SolidColorBrush(Color.FromArgb(0x44, 0xE2, 0x9A, 0x4A));
        private static readonly Pen HiddenPen = MakePen(Color.FromRgb(0x50, 0x50, 0x50), 1.0, new DoubleCollection { 4, 3 });
        private static readonly Pen WeldPen = MakePen(Color.FromRgb(0xC0, 0x1E, 0x1E), 2.5, null);
        private static readonly Pen WorkPointPen = MakePen(Color.FromRgb(0xC0, 0x1E, 0x1E), 1.2, null);
        private static readonly Pen BoltPen = MakePen(Color.FromRgb(0x20, 0x20, 0x20), 1.2, null);
        private static readonly Brush BoltFill = new SolidColorBrush(Color.FromArgb(0x55, 0x20, 0x20, 0x20));
        private static readonly Pen DimensionPen = MakePen(Color.FromRgb(0x00, 0x7A, 0x33), 0.9, null);
        private static readonly Brush DimensionBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x5E, 0x27));
        private static readonly Brush LabelBrush = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20));
        private static readonly Brush EmphasisBrush = new SolidColorBrush(Color.FromRgb(0x1F, 0x4E, 0x9A));
        private static readonly Typeface TextFace = new Typeface("Segoe UI");
        private static readonly Brush HoverFill = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xD5, 0x4F));
        private static readonly Pen HoverPen = MakePen(Color.FromRgb(0xE0, 0x8A, 0x2E), 1.0, null);

        private SketchModel? _model;
        private double _scale = 1.0;      // píxeles por mm
        private Point _origin = new Point(0, 0); // posición en píxeles del punto (0, 0) mm
        private bool _fitPending = true;
        private Point? _panStart;
        private Point _panOrigin;

        static SketchView()
        {
            FocusableProperty.OverrideMetadata(typeof(SketchView), new FrameworkPropertyMetadata(true));
            ClipToBoundsProperty.OverrideMetadata(typeof(SketchView), new FrameworkPropertyMetadata(true));
        }

        public SketchView()
        {
            SizeChanged += (_, _) =>
            {
                if (_fitPending) Fit();
                else InvalidateVisual();
            };
        }

        /// <summary>Croquis a dibujar. Cambiarlo conserva el zoom y el encuadre (la cartela no salta al editar un valor).</summary>
        public SketchModel? Model
        {
            get => _model;
            set
            {
                bool first = _model == null;
                _model = value;
                if (first) _fitPending = true;
                if (_fitPending && ActualWidth > 0 && ActualHeight > 0) Fit();
                else InvalidateVisual();
            }
        }

        /// <summary>Escala actual en píxeles por mm (solo informativa).</summary>
        public double PixelsPerMm => _scale;

        /// <summary>Encuadra todo el croquis con un margen.</summary>
        public void Fit()
        {
            if (_model == null || ActualWidth <= 0 || ActualHeight <= 0)
            {
                _fitPending = true;
                InvalidateVisual();
                return;
            }
            _fitPending = false;
            _model.ComputeBounds();
            double widthMm = Math.Max(1.0, _model.BoundsMax.X - _model.BoundsMin.X);
            double heightMm = Math.Max(1.0, _model.BoundsMax.Y - _model.BoundsMin.Y);
            const double margin = 36.0;
            _scale = Math.Min((ActualWidth - 2 * margin) / widthMm, (ActualHeight - 2 * margin) / heightMm);
            if (_scale <= 0 || double.IsNaN(_scale) || double.IsInfinity(_scale)) _scale = 0.5;
            double centerX = (_model.BoundsMin.X + _model.BoundsMax.X) / 2.0;
            double centerY = (_model.BoundsMin.Y + _model.BoundsMax.Y) / 2.0;
            _origin = new Point(ActualWidth / 2.0 - centerX * _scale, ActualHeight / 2.0 + centerY * _scale);
            InvalidateVisual();
        }

        private Point ToPixel(Point2D p) => new Point(_origin.X + p.X * _scale, _origin.Y - p.Y * _scale);

        private Point ToPixel(double x, double y) => new Point(_origin.X + x * _scale, _origin.Y - y * _scale);

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_model == null) return;
            double factor = e.Delta > 0 ? 1.15 : 1.0 / 1.15;
            double newScale = Math.Max(0.02, Math.Min(50.0, _scale * factor));
            factor = newScale / _scale;
            Point cursor = e.GetPosition(this);
            // El punto bajo el cursor se queda quieto.
            _origin = new Point(cursor.X - (cursor.X - _origin.X) * factor, cursor.Y - (cursor.Y - _origin.Y) * factor);
            _scale = newScale;
            _fitPending = false;
            InvalidateVisual();
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Middle)
            {
                _panStart = e.GetPosition(this);
                _panOrigin = _origin;
                CaptureMouse();
                Cursor = Cursors.SizeAll;
                e.Handled = true;
                return;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                Focus();
                if (e.ClickCount == 2)
                {
                    Point position = e.GetPosition(this);
                    HitTarget? target = FindHit(position);
                    if (target != null)
                    {
                        HitActivated?.Invoke(this, new SketchHitEventArgs(target.Path, target.Text, position));
                        e.Handled = true;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_panStart.HasValue && e.MiddleButton == MouseButtonState.Pressed)
            {
                Point now = e.GetPosition(this);
                _origin = new Point(_panOrigin.X + (now.X - _panStart.Value.X), _panOrigin.Y + (now.Y - _panStart.Value.Y));
                _fitPending = false;
                InvalidateVisual();
                return;
            }
            // Mano sobre los textos editables; se redibuja solo cuando cambia el objetivo (para resaltarlo).
            HitTarget? hover = FindHit(e.GetPosition(this));
            if (!ReferenceEquals(hover, _hover))
            {
                _hover = hover;
                Cursor = hover != null ? Cursors.Hand : Cursors.Arrow;
                ToolTip = hover != null ? "Doble clic para editar " + hover.Path : null;
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hover != null)
            {
                _hover = null;
                Cursor = Cursors.Arrow;
                InvalidateVisual();
            }
        }

        /// <summary>Texto editable más cercano al punto (dentro de su caja ampliada unos píxeles), o null.</summary>
        private HitTarget? FindHit(Point position)
        {
            HitTarget? best = null;
            double bestDistance = double.MaxValue;
            foreach (HitTarget target in _hitTargets)
            {
                Rect bounds = target.Bounds;
                bounds.Inflate(HitToleranceEmPixels, HitToleranceEmPixels);
                if (!bounds.Contains(position)) continue;
                Point center = new Point(target.Bounds.X + target.Bounds.Width / 2, target.Bounds.Y + target.Bounds.Height / 2);
                double distance = (center - position).Length;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = target;
                }
            }
            return best;
        }

        /// <summary>Rutas que la ventana sabe editar (las de la tabla); si no se fija, cualquier ruta cuenta.</summary>
        public Func<string, bool>? IsEditablePath { get; set; }

        private void RegisterHit(Rect bounds, string? path, string text)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (IsEditablePath != null && !IsEditablePath(path!)) return;
            _hitTargets.Add(new HitTarget(bounds, path!, text));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Middle && _panStart.HasValue)
            {
                _panStart = null;
                ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            _hitTargets.Clear();
            dc.DrawRectangle(PaperBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));
            if (_model == null)
            {
                DrawText(dc, "Sin croquis", new Point(12, 12), 13, LabelBrush, false);
                return;
            }

            // Orden: cuerpos de barras, cartela, placa cuchilla, ocultas, ejes, soldaduras, pernos, cotas, textos.
            foreach (SketchPolygon poly in _model.Polygons)
            {
                if (poly.Kind == SketchPolygonKind.Member) DrawPolygon(dc, poly, MemberFill, MemberPen);
            }
            foreach (SketchPolygon poly in _model.Polygons)
            {
                if (poly.Kind == SketchPolygonKind.Gusset) DrawPolygon(dc, poly, GussetFill, GussetPen);
            }
            foreach (SketchPolygon poly in _model.Polygons)
            {
                if (poly.Kind == SketchPolygonKind.KnifePlate) DrawPolygon(dc, poly, KnifeFill, KnifePen);
            }
            foreach (SketchLine line in _model.Lines)
            {
                Pen pen = PenFor(line.Kind);
                dc.DrawLine(pen, ToPixel(line.Start), ToPixel(line.End));
                if (line.Kind == SketchLineKind.Weld)
                {
                    DrawWeldTicks(dc, line);
                }
            }
            foreach (SketchCircle circle in _model.Circles)
            {
                double r = Math.Max(2.0, circle.RadiusMm * _scale);
                Point c = ToPixel(circle.Center);
                dc.DrawEllipse(BoltFill, BoltPen, c, r, r);
                dc.DrawLine(BoltPen, new Point(c.X - r * 0.7, c.Y - r * 0.7), new Point(c.X + r * 0.7, c.Y + r * 0.7));
                dc.DrawLine(BoltPen, new Point(c.X - r * 0.7, c.Y + r * 0.7), new Point(c.X + r * 0.7, c.Y - r * 0.7));
            }
            foreach (SketchDimension dim in _model.Dimensions)
            {
                DrawDimension(dc, dim);
            }
            foreach (SketchLabel label in _model.Labels)
            {
                Point at = ToPixel(label.Position);
                FormattedText text = MakeText(label.Text, label.Emphasized ? 12.5 : 11.0, label.Emphasized ? EmphasisBrush : LabelBrush, label.Emphasized);
                var bounds = new Rect(at.X, at.Y, text.Width, text.Height);
                if (_hover != null && _hover.Path == label.Path && _hover.Bounds == bounds) DrawHoverBox(dc, bounds);
                dc.DrawText(text, at);
                RegisterHit(bounds, label.Path, label.Text);
            }

            DrawAxesIndicator(dc);
            string scaleText = "Escala " + (_scale * 10.0).ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',') + " px/cm · rueda: zoom · botón central: encuadre · doble clic en una cota: editar";
            DrawText(dc, scaleText, new Point(8, ActualHeight - 20), 10.0, DimensionBrush, false);
        }

        private static Pen PenFor(SketchLineKind kind)
        {
            switch (kind)
            {
                case SketchLineKind.Axis: return AxisPen;
                case SketchLineKind.Hidden: return HiddenPen;
                case SketchLineKind.Weld: return WeldPen;
                case SketchLineKind.WorkPoint: return WorkPointPen;
                default: return MemberPen;
            }
        }

        private void DrawPolygon(DrawingContext dc, SketchPolygon poly, Brush fill, Pen pen)
        {
            if (poly.Points.Count < 2) return;
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(ToPixel(poly.Points[0]), true, true);
                for (int i = 1; i < poly.Points.Count; i++)
                {
                    ctx.LineTo(ToPixel(poly.Points[i]), true, false);
                }
            }
            geometry.Freeze();
            dc.DrawGeometry(fill, pen, geometry);
        }

        private void DrawWeldTicks(DrawingContext dc, SketchLine line)
        {
            Point a = ToPixel(line.Start);
            Point b = ToPixel(line.End);
            double dx = b.X - a.X, dy = b.Y - a.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 4) return;
            double ux = dx / len, uy = dy / len;
            double nx = -uy, ny = ux;
            const double step = 7.0;
            for (double t = step / 2; t < len; t += step)
            {
                var p = new Point(a.X + ux * t, a.Y + uy * t);
                dc.DrawLine(WeldPen, new Point(p.X - nx * 3 - ux * 2, p.Y - ny * 3 - uy * 2), new Point(p.X + nx * 3 + ux * 2, p.Y + ny * 3 + uy * 2));
            }
            if (!string.IsNullOrEmpty(line.Label))
            {
                var mid = new Point((a.X + b.X) / 2 + nx * 9, (a.Y + b.Y) / 2 + ny * 9);
                DrawText(dc, "▷ " + line.Label, mid, 9.5, WeldPen.Brush, false);
            }
        }

        private void DrawDimension(DrawingContext dc, SketchDimension dim)
        {
            var (a, b) = dim.DimensionLine();
            Point pa = ToPixel(a), pb = ToPixel(b);
            Point sa = ToPixel(dim.Start), sb = ToPixel(dim.End);

            // Líneas de referencia (desde el punto medido hasta un poco más allá de la línea de cota).
            dc.DrawLine(DimensionPen, sa, Extend(sa, pa, 4));
            dc.DrawLine(DimensionPen, sb, Extend(sb, pb, 4));
            dc.DrawLine(DimensionPen, pa, pb);

            double dx = pb.X - pa.X, dy = pb.Y - pa.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-6) return;
            double ux = dx / len, uy = dy / len;
            double nx = -uy, ny = ux;
            // Marcas oblicuas de arquitectura en los extremos.
            const double tick = 4.0;
            dc.DrawLine(DimensionPen, new Point(pa.X - tick * (ux + nx) * 0.7, pa.Y - tick * (uy + ny) * 0.7), new Point(pa.X + tick * (ux + nx) * 0.7, pa.Y + tick * (uy + ny) * 0.7));
            dc.DrawLine(DimensionPen, new Point(pb.X - tick * (ux + nx) * 0.7, pb.Y - tick * (uy + ny) * 0.7), new Point(pb.X + tick * (ux + nx) * 0.7, pb.Y + tick * (uy + ny) * 0.7));

            // Texto centrado, girado con la línea de cota y siempre legible (nunca cabeza abajo), a su izquierda.
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            double side = 1.0;
            if (angle > 90.0 || angle <= -90.0)
            {
                angle += 180.0;
                side = -1.0;
            }
            bool hovered = _hover != null && dim.Path != null && _hover.Path == dim.Path && _hover.Text == dim.Text;
            var formatted = MakeText(dim.Text, 10.5, hovered ? EmphasisBrush : DimensionBrush, hovered);
            var mid = new Point((pa.X + pb.X) / 2, (pa.Y + pb.Y) / 2);
            // Normal "arriba" del texto en pantalla: en WPF el eje Y crece hacia abajo, así que la izquierda de la
            // dirección (ux, uy) en pantalla es (uy, -ux).
            double lx = uy * side, ly = -ux * side;
            var anchor = new Point(mid.X + lx * 3, mid.Y + ly * 3);
            // Zona de clic: el texto centrado en su ancla, con media altura de texto hacia dentro de la cota; el
            // rectángulo no gira (basta para acertar con el doble clic en cotas de hasta 45° de inclinación).
            double hitHalf = Math.Max(formatted.Width, formatted.Height) / 2.0;
            var textCenter = new Point(anchor.X + lx * formatted.Height / 2.0, anchor.Y + ly * formatted.Height / 2.0);
            var hitBounds = new Rect(textCenter.X - hitHalf, textCenter.Y - formatted.Height / 2.0 - 2, hitHalf * 2, formatted.Height + 4);
            if (hovered) DrawHoverBox(dc, new Rect(textCenter.X - formatted.Width / 2.0 - 2, textCenter.Y - formatted.Height / 2.0 - 2, formatted.Width + 4, formatted.Height + 4));
            dc.PushTransform(new RotateTransform(angle, anchor.X, anchor.Y));
            dc.DrawText(formatted, new Point(anchor.X - formatted.Width / 2, anchor.Y - formatted.Height));
            dc.Pop();
            RegisterHit(hitBounds, dim.Path, dim.Text);
        }

        private static void DrawHoverBox(DrawingContext dc, Rect bounds)
        {
            dc.DrawRoundedRectangle(HoverFill, HoverPen, bounds, 3, 3);
        }

        private static Point Extend(Point from, Point to, double extraPixels)
        {
            double dx = to.X - from.X, dy = to.Y - from.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-6) return to;
            return new Point(to.X + dx / len * extraPixels, to.Y + dy / len * extraPixels);
        }

        private void DrawAxesIndicator(DrawingContext dc)
        {
            var o = new Point(ActualWidth - 70, ActualHeight - 30);
            dc.DrawLine(WorkPointPen, o, new Point(o.X + 40, o.Y));
            dc.DrawLine(WorkPointPen, o, new Point(o.X, o.Y - 40));
            DrawText(dc, "X (cordón)", new Point(o.X + 14, o.Y - 2), 9.5, LabelBrush, false);
            DrawText(dc, "Y", new Point(o.X + 4, o.Y - 46), 9.5, LabelBrush, false);
        }

        private static FormattedText MakeText(string text, double size, Brush brush, bool bold)
        {
            var face = bold ? new Typeface(TextFace.FontFamily, FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal) : TextFace;
            return new FormattedText(text ?? "", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, size, brush, 1.0);
        }

        private static void DrawText(DrawingContext dc, string text, Point position, double size, Brush brush, bool bold)
        {
            dc.DrawText(MakeText(text, size, brush, bold), position);
        }

        private static Pen MakePen(Color color, double thickness, DoubleCollection? dashes)
        {
            var pen = new Pen(new SolidColorBrush(color), thickness);
            if (dashes != null) pen.DashStyle = new DashStyle(dashes, 0);
            pen.Freeze();
            return pen;
        }
    }
}
