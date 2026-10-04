using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MotorConexiones.Core.Sketch;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Doble clic sobre una cota del croquis: la cota y el punto (píxeles del control) donde se hizo.</summary>
    public sealed class DimensionActivatedEventArgs : EventArgs
    {
        public DimensionActivatedEventArgs(SketchDimension dimension, Point position)
        {
            Dimension = dimension;
            Position = position;
        }

        public SketchDimension Dimension { get; }
        public Point Position { get; }
    }

    /// <summary>
    /// Dibuja un <see cref="Sketch"/> (mm, sistema local del nudo, Y hacia arriba) con zoom por rueda, encuadre con el
    /// botón central (o arrastrando) y ajuste a la ventana. El factor mm → píxel es propio del control, no una conversión
    /// de unidades; la geometría y los textos vienen hechos de Core. Ronda 6b: el doble clic sobre el texto o la línea de
    /// una cota lanza <see cref="DimensionActivated"/> para editar su valor en sitio; el cursor pasa a mano encima de ellas.
    /// </summary>
    public sealed class SketchCanvas : FrameworkElement
    {
        private const double MinScale = 0.02;
        private const double MaxScale = 50.0;
        private const double DimensionHitTolerancePx = 7.0;

        /// <summary>Dónde quedó dibujada cada cota (píxeles), para el doble clic.</summary>
        private readonly struct DimensionHit
        {
            public DimensionHit(SketchDimension dimension, Point lineStart, Point lineEnd, Point textCenter, double angleDeg, double textWidth, double textHeight)
            {
                Dimension = dimension;
                LineStart = lineStart;
                LineEnd = lineEnd;
                TextCenter = textCenter;
                AngleDeg = angleDeg;
                TextWidth = textWidth;
                TextHeight = textHeight;
            }

            public SketchDimension Dimension { get; }
            public Point LineStart { get; }
            public Point LineEnd { get; }
            public Point TextCenter { get; }
            public double AngleDeg { get; }
            public double TextWidth { get; }
            public double TextHeight { get; }
        }

        private readonly List<DimensionHit> _dimensionHits = new List<DimensionHit>();

        private static readonly Typeface TextTypeface = new Typeface("Segoe UI");
        private static readonly Brush Paper = Brushes.White;
        private static readonly Brush TextBrush = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
        private static readonly Brush DimensionBrush = new SolidColorBrush(Color.FromRgb(0x1F, 0x5F, 0x9F));
        private static readonly Brush HighlightBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0x6C, 0x00));
        private static readonly Brush GussetFill = new SolidColorBrush(Color.FromArgb(0x2A, 0x3A, 0x7B, 0xD5));
        private static readonly Brush PlateFill = new SolidColorBrush(Color.FromArgb(0x55, 0xF2, 0xA3, 0x3C));
        private static readonly Brush SlotFill = Brushes.White;
        private static readonly Brush LabelBackground = new SolidColorBrush(Color.FromArgb(0xC8, 0xFF, 0xFF, 0xFF));

        private readonly Pen _axisPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80)), 1.0) { DashStyle = new DashStyle(new[] { 8.0, 3.0, 2.0, 3.0 }, 0) });
        private readonly Pen _chordPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30)), 1.6));
        private readonly Pen _memberPen = Frozen(new Pen(Brushes.Black, 1.6));
        private readonly Pen _gussetPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x1D, 0x4E, 0x9A)), 2.2));
        private readonly Pen _platePen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0xB3, 0x5C, 0x00)), 1.6));
        private readonly Pen _slotPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x70, 0x70, 0x70)), 1.0));
        private readonly Pen _boltPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)), 1.3));
        private readonly Pen _weldPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0xC8, 0x1E, 0x1E)), 3.2));
        private readonly Pen _workPointPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x1E, 0x8E, 0x3E)), 1.6));
        private readonly Pen _dimensionPen = Frozen(new Pen(DimensionBrush, 1.0));
        private readonly Pen _highlightPen = Frozen(new Pen(HighlightBrush, 2.2));

        private Sketch? _sketch;
        private string? _highlightPath;
        private double _scale = 1.0;
        private Point _origin = new Point(0, 0);
        private bool _hasView;
        private Point? _dragStart;
        private Point _dragOrigin;

        public SketchCanvas()
        {
            ClipToBounds = true;
            Focusable = true;
            SizeChanged += (_, _) =>
            {
                if (!_hasView) Fit();
                InvalidateVisual();
            };
        }

        /// <summary>Croquis a dibujar. Al cambiarlo se conserva el encuadre (editar un valor no mueve la vista).</summary>
        public Sketch? Sketch
        {
            get => _sketch;
            set
            {
                _sketch = value;
                if (!_hasView) Fit();
                InvalidateVisual();
            }
        }

        /// <summary>Ruta JSON de la fila seleccionada en la tabla: su cota o etiqueta se resalta en naranja.</summary>
        public string? HighlightPath
        {
            get => _highlightPath;
            set
            {
                _highlightPath = value;
                InvalidateVisual();
            }
        }

        /// <summary>Píxeles por milímetro actuales (informativo).</summary>
        public double Scale => _scale;

        /// <summary>Doble clic con el botón izquierdo sobre una cota (su texto o su línea).</summary>
        public event EventHandler<DimensionActivatedEventArgs>? DimensionActivated;

        /// <summary>Cota dibujada bajo el punto (píxeles del control), o nula.</summary>
        public SketchDimension? HitTestDimension(Point point)
        {
            // De la última a la primera: la que se dibujó encima gana.
            for (int i = _dimensionHits.Count - 1; i >= 0; i--)
            {
                DimensionHit hit = _dimensionHits[i];
                if (IsInsideText(hit, point) || DistanceToSegment(point, hit.LineStart, hit.LineEnd) <= DimensionHitTolerancePx)
                {
                    return hit.Dimension;
                }
            }
            return null;
        }

        private static bool IsInsideText(DimensionHit hit, Point point)
        {
            // Al sistema del texto: origen en el centro de la línea de cota, X a lo largo de la línea. El texto se dibuja
            // justo "encima" de la línea (lado −Y en pantalla), con un pequeño margen para el dedo.
            double radians = -hit.AngleDeg * Math.PI / 180.0;
            double dx = point.X - hit.TextCenter.X;
            double dy = point.Y - hit.TextCenter.Y;
            double localX = dx * Math.Cos(radians) - dy * Math.Sin(radians);
            double localY = dx * Math.Sin(radians) + dy * Math.Cos(radians);
            return Math.Abs(localX) <= hit.TextWidth / 2.0 + 6.0 && localY >= -hit.TextHeight - 8.0 && localY <= 4.0;
        }

        private static double DistanceToSegment(Point p, Point a, Point b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double length2 = dx * dx + dy * dy;
            if (length2 < 1e-9) return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));
            double t = Math.Max(0.0, Math.Min(1.0, ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / length2));
            double qx = a.X + t * dx;
            double qy = a.Y + t * dy;
            return Math.Sqrt((p.X - qx) * (p.X - qx) + (p.Y - qy) * (p.Y - qy));
        }

        /// <summary>Encuadra todo el croquis en el control con un margen.</summary>
        public void Fit()
        {
            if (ActualWidth < 10 || ActualHeight < 10) return;
            SketchBounds bounds = _sketch?.GetBounds() ?? SketchBounds.Empty;
            if (bounds.IsEmpty) bounds = new SketchBounds(-400, -400, 400, 400);
            bounds = bounds.Inflate(60);
            double sx = ActualWidth / Math.Max(1.0, bounds.Width);
            double sy = ActualHeight / Math.Max(1.0, bounds.Height);
            _scale = Clamp(Math.Min(sx, sy) * 0.95, MinScale, MaxScale);
            double centerX = (bounds.MinX + bounds.MaxX) / 2.0;
            double centerY = (bounds.MinY + bounds.MaxY) / 2.0;
            _origin = new Point(ActualWidth / 2.0 - centerX * _scale, ActualHeight / 2.0 + centerY * _scale);
            _hasView = true;
            InvalidateVisual();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            Point cursor = e.GetPosition(this);
            double factor = Math.Pow(1.15, e.Delta / 120.0);
            double newScale = Clamp(_scale * factor, MinScale, MaxScale);
            factor = newScale / _scale;
            _origin = new Point(cursor.X - (cursor.X - _origin.X) * factor, cursor.Y - (cursor.Y - _origin.Y) * factor);
            _scale = newScale;
            _hasView = true;
            InvalidateVisual();
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                // Doble clic: si cae sobre una cota, se edita; no se inicia encuadre.
                Point position = e.GetPosition(this);
                SketchDimension? dimension = HitTestDimension(position);
                if (dimension != null)
                {
                    _dragStart = null;
                    if (IsMouseCaptured) ReleaseMouseCapture();
                    e.Handled = true;
                    DimensionActivated?.Invoke(this, new DimensionActivatedEventArgs(dimension, position));
                    return;
                }
            }
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                _dragStart = e.GetPosition(this);
                _dragOrigin = _origin;
                CaptureMouse();
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point current = e.GetPosition(this);
            if (_dragStart.HasValue)
            {
                _origin = new Point(_dragOrigin.X + current.X - _dragStart.Value.X, _dragOrigin.Y + current.Y - _dragStart.Value.Y);
                _hasView = true;
                InvalidateVisual();
                return;
            }
            Cursor = HitTestDimension(current) != null ? Cursors.Hand : Cursors.Arrow;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (_dragStart.HasValue && (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left))
            {
                _dragStart = null;
                ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            _dragStart = null;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Paper, null, new Rect(0, 0, ActualWidth, ActualHeight));
            _dimensionHits.Clear();
            if (_sketch == null) return;
            double dip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            foreach (SketchPolygon polygon in _sketch.Polygons)
            {
                DrawPolygon(dc, polygon);
            }
            foreach (SketchLine line in _sketch.Lines)
            {
                dc.DrawLine(PenFor(line.Kind), ToScreen(line.Start), ToScreen(line.End));
            }
            foreach (SketchCircle circle in _sketch.Circles)
            {
                Point center = ToScreen(circle.Center);
                double r = Math.Max(1.5, circle.RadiusMm * _scale);
                dc.DrawEllipse(null, _boltPen, center, r, r);
                double cross = r * 1.5;
                dc.DrawLine(_boltPen, new Point(center.X - cross, center.Y), new Point(center.X + cross, center.Y));
                dc.DrawLine(_boltPen, new Point(center.X, center.Y - cross), new Point(center.X, center.Y + cross));
            }
            foreach (SketchDimension dimension in _sketch.Dimensions)
            {
                DrawDimension(dc, dimension, dip);
            }
            foreach (SketchLabel label in _sketch.Labels)
            {
                DrawLabel(dc, label, dip);
            }
        }

        private void DrawPolygon(DrawingContext dc, SketchPolygon polygon)
        {
            if (polygon.Points.Count < 2) return;
            var geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(ToScreen(polygon.Points[0]), polygon.IsClosed, polygon.IsClosed);
                for (int i = 1; i < polygon.Points.Count; i++)
                {
                    context.LineTo(ToScreen(polygon.Points[i]), true, false);
                }
            }
            geometry.Freeze();
            Brush? fill = polygon.IsClosed ? FillFor(polygon.Kind) : null;
            dc.DrawGeometry(fill, PenFor(polygon.Kind), geometry);
        }

        private void DrawDimension(DrawingContext dc, SketchDimension dimension, double dip)
        {
            bool highlighted = _highlightPath != null && dimension.Path == _highlightPath;
            Pen pen = highlighted ? _highlightPen : _dimensionPen;
            Brush brush = highlighted ? HighlightBrush : DimensionBrush;

            var (lineStart, lineEnd) = dimension.GetDimensionLine();
            Point a = ToScreen(dimension.Start);
            Point b = ToScreen(dimension.End);
            Point la = ToScreen(lineStart);
            Point lb = ToScreen(lineEnd);

            // Líneas de referencia y línea de cota.
            dc.DrawLine(pen, a, Extend(a, la, 4));
            dc.DrawLine(pen, b, Extend(b, lb, 4));
            dc.DrawLine(pen, la, lb);

            // Marcas oblicuas en los extremos.
            double dx = lb.X - la.X;
            double dy = lb.Y - la.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length < 1e-6) return;
            double ux = dx / length;
            double uy = dy / length;
            double tick = 5.0;
            dc.DrawLine(pen, new Point(la.X - (ux - uy) * tick * 0.7, la.Y - (uy + ux) * tick * 0.7), new Point(la.X + (ux - uy) * tick * 0.7, la.Y + (uy + ux) * tick * 0.7));
            dc.DrawLine(pen, new Point(lb.X - (ux - uy) * tick * 0.7, lb.Y - (uy + ux) * tick * 0.7), new Point(lb.X + (ux - uy) * tick * 0.7, lb.Y + (uy + ux) * tick * 0.7));

            // Texto a lo largo de la línea, siempre legible de izquierda a derecha.
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            if (angle > 90.0 || angle <= -90.0) angle += 180.0;
            var text = new FormattedText(dimension.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeface, 11.5, brush, dip);
            var mid = new Point((la.X + lb.X) / 2.0, (la.Y + lb.Y) / 2.0);
            dc.PushTransform(new RotateTransform(angle, mid.X, mid.Y));
            var origin = new Point(mid.X - text.Width / 2.0, mid.Y - text.Height - 2.0);
            dc.DrawRectangle(LabelBackground, null, new Rect(origin.X - 2, origin.Y, text.Width + 4, text.Height));
            dc.DrawText(text, origin);
            dc.Pop();

            _dimensionHits.Add(new DimensionHit(dimension, la, lb, mid, angle, text.Width, text.Height));
        }

        private void DrawLabel(DrawingContext dc, SketchLabel label, double dip)
        {
            bool highlighted = _highlightPath != null && label.Path == _highlightPath;
            var text = new FormattedText(label.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeface, 11.5,
                highlighted ? HighlightBrush : TextBrush, dip);
            Point p = ToScreen(label.Position);
            double x = label.Anchor == 1 ? p.X - text.Width / 2.0 : label.Anchor == 2 ? p.X - text.Width : p.X;
            var origin = new Point(x, p.Y - text.Height / 2.0);
            dc.DrawRectangle(LabelBackground, highlighted ? _highlightPen : null, new Rect(origin.X - 2, origin.Y - 1, text.Width + 4, text.Height + 2));
            dc.DrawText(text, origin);
        }

        private Pen PenFor(SketchKind kind)
        {
            switch (kind)
            {
                case SketchKind.Axis: return _axisPen;
                case SketchKind.ChordEdge: return _chordPen;
                case SketchKind.MemberOutline: return _memberPen;
                case SketchKind.Gusset: return _gussetPen;
                case SketchKind.KnifePlate: return _platePen;
                case SketchKind.Slot: return _slotPen;
                case SketchKind.Bolt: return _boltPen;
                case SketchKind.Weld: return _weldPen;
                case SketchKind.WorkPoint: return _workPointPen;
                case SketchKind.Dimension: return _dimensionPen;
                default: return _memberPen;
            }
        }

        private static Brush? FillFor(SketchKind kind)
        {
            switch (kind)
            {
                case SketchKind.Gusset: return GussetFill;
                case SketchKind.KnifePlate: return PlateFill;
                case SketchKind.Slot: return SlotFill;
                default: return null;
            }
        }

        private Point ToScreen(SketchPoint p) => new Point(_origin.X + p.X * _scale, _origin.Y - p.Y * _scale);

        private static Point Extend(Point from, Point to, double extraPixels)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length < 1e-6) return to;
            return new Point(to.X + dx / length * extraPixels, to.Y + dy / length * extraPixels);
        }

        private static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

        private static Pen Frozen(Pen pen)
        {
            pen.Freeze();
            return pen;
        }
    }
}
