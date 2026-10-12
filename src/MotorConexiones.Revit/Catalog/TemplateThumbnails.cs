using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit.Catalog
{
    /// <summary>
    /// Fase 11 (V4): la miniatura de cada plantilla, un PNG de 240×180 <b>junto a su archivo</b> (<c>&lt;template_id&gt;.png</c>
    /// en la carpeta del catálogo), pintado con el mismo <see cref="SketchCanvas"/> de la previsualización (sin cotas ni
    /// etiquetas) a partir del croquis que calcula el Core sin modelo (<see cref="TemplateSketch"/>). Se rehace si falta o si
    /// el <c>.json</c> es más nuevo. Un fallo al dibujar deja un hueco gris con el nombre y una línea en el log
    /// (<c>thumbnail_failed</c>): nunca impide abrir una ventana. Solo con el hilo de WPF (ventanas de la cinta).
    /// </summary>
    public static class TemplateThumbnails
    {
        public const int WidthPx = 240;
        public const int HeightPx = 180;

        /// <summary>Ruta del PNG de una plantilla en su carpeta (exista o no). Nunca pisa un .json: la extensión es distinta.</summary>
        public static string PathFor(CatalogTemplate template, string folder) => Path.Combine(folder, template.TemplateId + ".png");

        public static string PathFor(string templateId, string folder) => Path.Combine(folder, templateId + ".png");

        /// <summary>
        /// Deja escrito el PNG de la plantilla (si falta o está más viejo que su .json) y devuelve su ruta; nulo si no se pudo
        /// dibujar (queda en el log). <paramref name="templateFile"/> es el .json de la plantilla, para comparar fechas.
        /// </summary>
        public static string? EnsurePng(CatalogTemplate template, string folder, LimitsConfig? limits = null, string? templateFile = null)
        {
            if (template == null || string.IsNullOrWhiteSpace(folder)) return null;
            string png = PathFor(template, folder);
            try
            {
                string json = templateFile ?? Path.Combine(folder, template.TemplateId + ".json");
                if (File.Exists(png) && (!File.Exists(json) || File.GetLastWriteTimeUtc(png) >= File.GetLastWriteTimeUtc(json)))
                {
                    return png;
                }
                Directory.CreateDirectory(folder);
                BitmapSource bitmap = Render(template, limits);
                if (IsBlank(bitmap))
                {
                    JsonLineLogger.Write(new { @event = "thumbnail_failed", template_id = template.TemplateId, name = template.Name, error = "el PNG salió en blanco (RenderTargetBitmap fuera de pantalla)" });
                    return null;
                }
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (FileStream stream = new FileStream(png, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    encoder.Save(stream);
                }
                JsonLineLogger.Write(new { @event = "thumbnail_written", template_id = template.TemplateId, name = template.Name, file = png });
                return png;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "thumbnail_failed", template_id = template.TemplateId, name = template.Name, file = png, error = ex.ToString() });
                return null;
            }
        }

        /// <summary>Los PNG de todas las plantillas de la carpeta; devuelve id → ruta (o nulo si falló). Nunca lanza.</summary>
        public static Dictionary<string, string?> EnsureAll(CatalogStore store, IEnumerable<CatalogEntry> entries, LimitsConfig? limits = null)
        {
            var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            if (store == null || entries == null) return result;
            foreach (CatalogEntry entry in entries)
            {
                try
                {
                    CatalogTemplate? template = store.Get(entry.TemplateId);
                    result[entry.TemplateId] = template == null ? null : EnsurePng(template, store.Folder, limits, entry.File);
                }
                catch (Exception ex)
                {
                    JsonLineLogger.Write(new { @event = "thumbnail_failed", template_id = entry.TemplateId, error = ex.ToString() });
                    result[entry.TemplateId] = null;
                }
            }
            return result;
        }

        /// <summary>Pinta el croquis de la plantilla (sin cotas) en un lienzo fuera de pantalla de 240×180.</summary>
        public static BitmapSource Render(CatalogTemplate template, LimitsConfig? limits = null)
        {
            Core.Sketch.Sketch sketch = TemplateSketch.Build(template, limits ?? LimitsConfig.Default);
            var canvas = new SketchCanvas { ShowAnnotations = false, Width = WidthPx, Height = HeightPx };
            // Sin árbol visual: hay que medir y colocar a mano antes de pintar (riesgo anotado en el prompt de la fase).
            canvas.Measure(new Size(WidthPx, HeightPx));
            canvas.Arrange(new Rect(0, 0, WidthPx, HeightPx));
            canvas.Sketch = sketch;
            canvas.FitTo(WidthPx, HeightPx, 40);
            canvas.UpdateLayout();
            var bitmap = new RenderTargetBitmap(WidthPx, HeightPx, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(canvas);
            bitmap.Freeze();
            return bitmap;
        }

        /// <summary>Verdadero si todos los píxeles son blancos (o transparentes): el dibujo no llegó al mapa de bits.</summary>
        public static bool IsBlank(BitmapSource bitmap)
        {
            if (bitmap == null) return true;
            int stride = bitmap.PixelWidth * 4;
            var pixels = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixels, stride, 0);
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i], g = pixels[i + 1], r = pixels[i + 2], a = pixels[i + 3];
                if (a != 0 && (r < 250 || g < 250 || b < 250)) return false;
            }
            return true;
        }

        /// <summary>Carga un PNG como imagen de WPF (congelada, sin bloquear el archivo); nulo si no existe o no se puede leer.</summary>
        public static BitmapSource? Load(string? pngPath)
        {
            if (string.IsNullOrEmpty(pngPath) || !File.Exists(pngPath)) return null;
            try
            {
                var image = new BitmapImage();
                using (FileStream stream = new FileStream(pngPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "thumbnail_load_failed", file = pngPath, error = ex.Message });
                return null;
            }
        }

        /// <summary>La miniatura de una plantilla si su PNG existe; si no, el hueco gris con el nombre.</summary>
        public static ImageSource ImageOrPlaceholder(string? pngPath, string name) => Load(pngPath) ?? Placeholder(name);

        /// <summary>Hueco gris con el nombre de la plantilla (cuando el PNG no se pudo dibujar).</summary>
        public static ImageSource Placeholder(string name)
        {
            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)), new Pen(Brushes.Gray, 1), new Rect(0, 0, WidthPx, HeightPx));
                var text = new FormattedText(string.IsNullOrWhiteSpace(name) ? "sin miniatura" : name.Trim(), System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight, new Typeface("Segoe UI"), 13, Brushes.DimGray, 1.0) { MaxTextWidth = WidthPx - 20, MaxTextHeight = HeightPx - 20, TextAlignment = TextAlignment.Center };
                dc.DrawText(text, new Point(10, (HeightPx - text.Height) / 2.0));
            }
            var bitmap = new RenderTargetBitmap(WidthPx, HeightPx, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }

        /// <summary>Copia el PNG de una plantilla junto a su .json en otra carpeta (la compartida), si existe; devuelve la copia o nulo.</summary>
        public static string? CopyAlongside(string? pngPath, string? targetFolder)
        {
            if (string.IsNullOrEmpty(pngPath) || string.IsNullOrEmpty(targetFolder) || !File.Exists(pngPath)) return null;
            try
            {
                Directory.CreateDirectory(targetFolder!);
                string target = Path.Combine(targetFolder!, Path.GetFileName(pngPath));
                File.Copy(pngPath!, target, overwrite: true);
                return target;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "thumbnail_copy_failed", file = pngPath, folder = targetFolder, error = ex.Message });
                return null;
            }
        }
    }
}
