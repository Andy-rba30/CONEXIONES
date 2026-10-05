using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MotorConexiones.Core.Batch;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Clic o doble clic sobre un nudo del mapa.</summary>
    public sealed class MapNodeEventArgs : EventArgs
    {
        public MapNodeEventArgs(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    /// <summary>
    /// Ronda 8c (V1): dibuja el alzado de la cercha (<see cref="TrussMap"/>, mm) con las barras en gris, los nudos como
    /// círculos del color de su estado con el número dentro y los ocultos como círculos vacíos pequeños (solo con
    /// <see cref="ShowHidden"/>). Misma técnica que <see cref="SketchCanvas"/>: rueda = zoom, arrastrar = encuadre,
    /// <see cref="Fit"/> = ver todo. Clic en un círculo = <see cref="NodeClicked"/> (seleccionar la fila), doble clic =
    /// <see cref="NodeActivated"/> (Ver en Revit); al pasar el ratón se dibuja un globo "N4 · Listo · Detalle D · igual".
    /// </summary>
    public sealed class TrussMapCanvas : FrameworkElement
    {
        private const double MinScale = 0.0005;
        private const double MaxScale = 4.0;
        private const double NodeRadiusPx = 11.0;
        private const double HiddenRadiusPx = 4.5;
        private const double ClickTolerancePx = 4.0;

        private static readonly Typeface NumberTypeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
        private static readonly Typeface LabelTypeface = new Typeface("Segoe UI");
        private static readonly Brush Paper = Brushes.White;
        private static readonly Brush DarkText = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
        private static readonly Brush LabelBackground = new SolidColorBrush(Color.FromArgb(0xE6, 0xFF, 0xFF, 0xF0));
        private static readonly Brush HintBrush = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
        private static readonly Pen BarPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x9A, 0x9A, 0x9A)), 1.2));
        private static readonly Pen ChordPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50)), 2.2));
        private static readonly Pen HiddenPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0xA8, 0xA8, 0xA8)), 1.0));
        private static readonly Pen RingPen = Frozen(new Pen(Brushes.White, 1.2));
        private static readonly Pen SelectedPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0xE0, 0x6C, 0x00)), 2.6));
        private static readonly Pen HoverPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)), 1.4));
        private static readonly Pen LabelPen = Frozen(new Pen(new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88)), 1.0));

        private TrussMap? _map;
        private bool _showHidden;
        private string? _selected;
        private string? _hover;
        private double _scale = 0.01;
        private Point _origin = new Point(0, 0);
        private bool _hasView;
        private Point? _dragStart;
        private Point _dragOrigin;
        private bool _dragged;

        public TrussMapCanvas()
        {
            ClipToBounds = true;
            Focusable = true;
            SizeChanged += (_, _) =>
            {
                if (!_hasView) Fit();
                InvalidateVisual();
            };
        }

        /// <summary>El alzado a dibujar. Al cambiarlo (replanificar) se conserva el encuadre si ya había uno.</summary>
        public TrussMap? Map
        {
            get => _map;
            set
            {
                _map = value;
                _hover = null;
                if (!_hasView) Fit();
                InvalidateVisual();
            }
        }

        /// <summary>Dibujar también los nudos ocultos por defecto (círculos vacíos pequeños).</summary>
        public bool ShowHidden
        {
            get => _showHidden;
            set
            {
                _showHidden = value;
                InvalidateVisual();
            }
        }

        /// <summary>Nudo resaltado (la fila elegida en la tabla).</summary>
        public string? SelectedNode
        {
            get => _selected;
            set
            {
                _selected = value;
                InvalidateVisual();
            }
        }

        /// <summary>Clic con el botón izquierdo sobre un nudo.</summary>
        public event EventHandler<MapNodeEventArgs>? NodeClicked;

        /// <summary>Doble clic sobre un nudo.</summary>
        public event EventHandler<MapNodeEventArgs>? NodeActivated;

        /// <summary>Encuadra toda la cercha con un margen.</summary>
        public void Fit()
        {
            if (ActualWidth < 10 || ActualHeight < 10) return;
            double minX = -5000, minY = -2000, maxX = 5000, maxY = 2000;
            if (_map != null && !_map.IsEmpty)
            {
                minX = _map.MinX;
                minY = _map.MinY;
                maxX = _map.MaxX;
                maxY = _map.MaxY;
            }
            double width = Math.Max(1.0, maxX - minX);
            double height = Math.Max(1.0, maxY - minY);
            double marginX = Math.Max(width * 0.06, 400.0);
            double marginY = Math.Max(height * 0.12, 400.0);
            double sx = ActualWidth / (width + 2 * marginX);
            double sy = ActualHeight / (height + 2 * marginY);
            _scale = Clamp(Math.Min(sx, sy), MinScale, MaxScale);
            double centerX = (minX + maxX) / 2.0;
            double centerY = (minY + maxY) / 2.0;
            _origin = new Point(ActualWidth / 2.0 - centerX * _scale, ActualHeight / 2.0 + centerY * _scale);
            _hasView = true;
            InvalidateVisual();
        }

        /// <summary>Nudo dibujado bajo el punto (píxeles del control), o nulo. Los ocultos solo cuentan si se dibujan.</summary>
        public TrussMapNode? HitTest(Point point)
        {
            if (_map == null) return null;
            TrussMapNode? best = null;
            double bestDistance = double.MaxValue;
            foreach (TrussMapNode node in _map.Nodes)
            {
                if (!node.VisibleByDefault && !_showHidden) continue;
                Point center = ToScreen(node.X, node.Y);
                double radius = node.VisibleByDefault ? NodeRadiusPx : HiddenRadiusPx;
                double dx = point.X - center.X;
                double dy = point.Y - center.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance <= radius + 3.0 && distance < bestDistance)
                {
                    best = node;
                    bestDistance = distance;
                }
            }
            return best;
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
            Point position = e.GetPosition(this);
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                TrussMapNode? node = HitTest(position);
                _dragStart = null;
                if (IsMouseCaptured) ReleaseMouseCapture();
                if (node != null)
                {
                    e.Handled = true;
                    NodeActivated?.Invoke(this, new MapNodeEventArgs(node.Name));
                }
                return;
            }
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                _dragStart = position;
                _dragOrigin = _origin;
                _dragged = false;
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
                double dx = current.X - _dragStart.Value.X;
                double dy = current.Y - _dragStart.Value.Y;
                if (Math.Abs(dx) > ClickTolerancePx || Math.Abs(dy) > ClickTolerancePx) _dragged = true;
                if (_dragged)
                {
                    _origin = new Point(_dragOrigin.X + dx, _dragOrigin.Y + dy);
                    _hasView = true;
                    InvalidateVisual();
                }
                return;
            }
            string? hover = HitTest(current)?.Name;
            Cursor = hover != null ? Cursors.Hand : Cursors.Arrow;
            if (!string.Equals(hover, _hover, StringComparison.Ordinal))
            {
                _hover = hover;
                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_dragStart.HasValue || (e.ChangedButton != MouseButton.Middle && e.ChangedButton != MouseButton.Left)) return;
            bool wasClick = !_dragged && e.ChangedButton == MouseButton.Left;
            _dragStart = null;
            ReleaseMouseCapture();
            e.Handled = true;
            if (wasClick)
            {
                TrussMapNode? node = HitTest(e.GetPosition(this));
                if (node != null)
                {
                    _selected = node.Name;
                    InvalidateVisual();
                    NodeClicked?.Invoke(this, new MapNodeEventArgs(node.Name));
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hover != null)
            {
                _hover = null;
                InvalidateVisual();
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
            double dip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            if (_map == null || _map.IsEmpty)
            {
                var hint = new FormattedText("Sin mapa: el plan no tiene barras legibles.", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, LabelTypeface, 12.0, HintBrush, dip);
                dc.DrawText(hint, new Point(10, 8));
                return;
            }

            foreach (TrussMapSegment segment in _map.Segments)
            {
                dc.DrawLine(segment.IsChord ? ChordPen : BarPen, ToScreen(segment.X0, segment.Y0), ToScreen(segment.X1, segment.Y1));
            }

            // Primero los ocultos (debajo), luego los visibles, y el elegido encima de todo.
            if (_showHidden)
            {
                foreach (TrussMapNode node in _map.Nodes.Where(n => !n.VisibleByDefault))
                {
                    Point center = ToScreen(node.X, node.Y);
                    dc.DrawEllipse(Paper, HiddenPen, center, HiddenRadiusPx, HiddenRadiusPx);
                }
            }
            foreach (TrussMapNode node in _map.Nodes.Where(n => n.VisibleByDefault && !IsSelected(n)))
            {
                DrawNode(dc, node, dip, false);
            }
            TrussMapNode? selected = _selected == null ? null : _map.Find(_selected);
            if (selected != null && (selected.VisibleByDefault || _showHidden)) DrawNode(dc, selected, dip, true);

            TrussMapNode? hover = _hover == null ? null : _map.Find(_hover);
            if (hover != null) DrawLabel(dc, hover, dip);
        }

        private bool IsSelected(TrussMapNode node) => _selected != null && string.Equals(node.Name, _selected, StringComparison.OrdinalIgnoreCase);

        private void DrawNode(DrawingContext dc, TrussMapNode node, double dip, bool selected)
        {
            Point center = ToScreen(node.X, node.Y);
            var (r, g, b) = PlanAdvice.Rgb(node.ColorName);
            var fill = new SolidColorBrush(Color.FromRgb(r, g, b));
            fill.Freeze();
            double radius = node.VisibleByDefault ? NodeRadiusPx : HiddenRadiusPx + 2.0;
            if (selected) dc.DrawEllipse(null, SelectedPen, center, radius + 3.0, radius + 3.0);
            dc.DrawEllipse(fill, _hover != null && string.Equals(_hover, node.Name, StringComparison.OrdinalIgnoreCase) ? HoverPen : RingPen, center, radius, radius);
            if (!node.VisibleByDefault) return;
            Brush textBrush = node.ColorName == PlanAdvice.Amber ? DarkText : Brushes.White;
            double size = node.Number.Length > 2 ? 8.5 : 10.0;
            var text = new FormattedText(node.Number, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, NumberTypeface, size, textBrush, dip);
            dc.DrawText(text, new Point(center.X - text.Width / 2.0, center.Y - text.Height / 2.0));
        }

        private void DrawLabel(DrawingContext dc, TrussMapNode node, double dip)
        {
            Point center = ToScreen(node.X, node.Y);
            var text = new FormattedText(node.Label, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, LabelTypeface, 11.5, DarkText, dip);
            double x = center.X + NodeRadiusPx + 8.0;
            double y = center.Y - text.Height / 2.0;
            if (x + text.Width + 10.0 > ActualWidth) x = center.X - NodeRadiusPx - 8.0 - text.Width;
            if (y < 2.0) y = 2.0;
            if (y + text.Height + 4.0 > ActualHeight) y = ActualHeight - text.Height - 4.0;
            dc.DrawRoundedRectangle(LabelBackground, LabelPen, new Rect(x - 5.0, y - 2.0, text.Width + 10.0, text.Height + 4.0), 3.0, 3.0);
            dc.DrawText(text, new Point(x, y));
        }

        private Point ToScreen(double x, double y) => new Point(_origin.X + x * _scale, _origin.Y - y * _scale);

        private static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

        private static Pen Frozen(Pen pen)
        {
            pen.Freeze();
            return pen;
        }
    }
}
