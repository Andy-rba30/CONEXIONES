using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Iconos de los botones de la cinta dibujados en código (DrawingVisual + RenderTargetBitmap), sin archivos PNG:
    /// la misma mecánica que usan los add-ins de la pestaña ARBA. Si algo falla devuelve null y el botón sale sin icono.
    /// </summary>
    internal static class RibbonIcons
    {
        private static readonly Brush Steel = new SolidColorBrush(Color.FromRgb(0x4A, 0x5A, 0x6A));
        private static readonly Brush Gusset = new SolidColorBrush(Color.FromRgb(0x1F, 0x4E, 0x9A));
        private static readonly Brush Paper = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        private static readonly Brush Red = new SolidColorBrush(Color.FromRgb(0xC0, 0x1E, 0x1E));

        /// <summary>Cartela azul con cordón, una diagonal y cuatro pernos.</summary>
        public static ImageSource? RunSpec(int size)
        {
            return Render(size, (dc, s) =>
            {
                double u = s / 32.0;
                var gusset = new StreamGeometry();
                using (StreamGeometryContext ctx = gusset.Open())
                {
                    ctx.BeginFigure(new Point(5 * u, 8 * u), true, true);
                    ctx.LineTo(new Point(27 * u, 8 * u), true, false);
                    ctx.LineTo(new Point(27 * u, 22 * u), true, false);
                    ctx.LineTo(new Point(17 * u, 29 * u), true, false);
                    ctx.LineTo(new Point(5 * u, 29 * u), true, false);
                }
                gusset.Freeze();
                dc.DrawGeometry(Gusset, new Pen(Steel, 1.0 * u), gusset);
                dc.DrawRectangle(Steel, null, new Rect(0, 2 * u, s, 5 * u));
                var pen = new Pen(Steel, 4.5 * u) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                dc.DrawLine(pen, new Point(7 * u, 27 * u), new Point(24 * u, 10 * u));
                double r = 1.8 * u;
                foreach (var p in new[] { new Point(10, 13), new Point(14, 13), new Point(10, 18), new Point(14, 18) })
                {
                    dc.DrawEllipse(Paper, null, new Point(p.X * u, p.Y * u), r, r);
                }
            });
        }

        /// <summary>Lista de conexiones con una marca roja de borrado.</summary>
        public static ImageSource? ModelConnections(int size)
        {
            return Render(size, (dc, s) =>
            {
                double u = s / 32.0;
                dc.DrawRoundedRectangle(Paper, new Pen(Steel, 1.5 * u), new Rect(3 * u, 3 * u, 26 * u, 26 * u), 2 * u, 2 * u);
                var line = new Pen(Steel, 2.2 * u) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                for (int i = 0; i < 3; i++)
                {
                    double y = (9 + i * 7) * u;
                    dc.DrawRectangle(Gusset, null, new Rect(7 * u, y - 1.8 * u, 4 * u, 3.6 * u));
                    dc.DrawLine(line, new Point(13 * u, y), new Point(i == 2 ? 18 * u : 24 * u, y));
                }
                var cross = new Pen(Red, 3.0 * u) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                dc.DrawLine(cross, new Point(20 * u, 19 * u), new Point(27 * u, 26 * u));
                dc.DrawLine(cross, new Point(27 * u, 19 * u), new Point(20 * u, 26 * u));
            });
        }

        private static ImageSource? Render(int size, Action<DrawingContext, double> draw)
        {
            try
            {
                var visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    draw(dc, size);
                }
                var bitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(visual);
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}
