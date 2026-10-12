using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Fase 11 (V5): el plano al lado del croquis. Abre una imagen (PNG o JPG) del detalle en un panel con rueda = zoom,
    /// arrastrar = encuadre y Ajustar, como el croquis; un botón la pliega sin olvidarla. El pegado del portapapeles (un
    /// recorte con Win+Mayús+S, también de un PDF abierto) se guarda como PNG en <c>%LOCALAPPDATA%\MotorConexiones\planos\</c>.
    /// La ruta la recuerda la especificación (<c>source.drawing_image</c>), no este control.
    /// </summary>
    public partial class DrawingPane : UserControl
    {
        public const string FolderName = "planos";

        private const double MinScale = 0.05;
        private const double MaxScale = 20.0;

        private Point? _dragStart;
        private double _dragX, _dragY;
        private bool _fitted;

        public DrawingPane()
        {
            InitializeComponent();
            UpdateEmpty();
        }

        /// <summary>Carpeta donde se guardan las imágenes pegadas del portapapeles.</summary>
        public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotorConexiones", FolderName);

        /// <summary>La persona pulsó Ocultar plano.</summary>
        public event EventHandler? HideRequested;

        /// <summary>Ruta de la imagen abierta, o nula.</summary>
        public string? ImagePath { get; private set; }

        public bool HasImage => Picture.Source != null;

        /// <summary>Abre la imagen; falso (con el motivo) si el archivo no existe o no es una imagen legible. Nunca lanza.</summary>
        public bool Load(string? path, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "No se indicó ninguna ruta.";
                return false;
            }
            if (!File.Exists(path))
            {
                error = "El archivo del plano no existe: " + path;
                return false;
            }
            try
            {
                var image = new BitmapImage();
                using (FileStream stream = new FileStream(path!, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                image.Freeze();
                Picture.Source = image;
                ImagePath = path;
                TitleText.Text = Path.GetFileName(path) + " · " + image.PixelWidth + "×" + image.PixelHeight + " · rueda = zoom · arrastrar = encuadre";
                _fitted = false;
                UpdateEmpty();
                Fit();
                return true;
            }
            catch (Exception ex)
            {
                error = "No se pudo abrir la imagen " + path + ": " + ex.Message;
                JsonLineLogger.Write(new { @event = "drawing_pane_failed", file = path, error = ex.ToString() });
                return false;
            }
        }

        /// <summary>Quita la imagen (la ruta la olvida quien la guardó en la especificación).</summary>
        public void Clear()
        {
            Picture.Source = null;
            ImagePath = null;
            TitleText.Text = "Plano: rueda = zoom · arrastrar = encuadre";
            UpdateEmpty();
        }

        /// <summary>Encuadra la imagen entera en el panel.</summary>
        public void Fit()
        {
            if (Picture.Source is not BitmapSource bitmap || Viewport.ActualWidth < 10 || Viewport.ActualHeight < 10) return;
            double w = bitmap.PixelWidth * 96.0 / Math.Max(1.0, bitmap.DpiX);
            double h = bitmap.PixelHeight * 96.0 / Math.Max(1.0, bitmap.DpiY);
            double scale = Clamp(Math.Min(Viewport.ActualWidth / w, Viewport.ActualHeight / h) * 0.96, MinScale, MaxScale);
            Zoom.ScaleX = Zoom.ScaleY = scale;
            Pan.X = (Viewport.ActualWidth - w * scale) / 2.0;
            Pan.Y = (Viewport.ActualHeight - h * scale) / 2.0;
            _fitted = true;
        }

        /// <summary>
        /// Guarda la imagen del portapapeles como PNG en <see cref="Folder"/> (<c>plano-&lt;documento&gt;-&lt;fecha-hora&gt;.png</c>) y
        /// devuelve la ruta; falso con el motivo si no hay imagen (hay texto, o nada) o no se pudo escribir. Nunca lanza.
        /// </summary>
        public static bool TrySaveClipboardImage(string? documentTitle, out string path, out string error)
        {
            path = string.Empty;
            error = string.Empty;
            try
            {
                if (!Clipboard.ContainsImage())
                {
                    error = Clipboard.ContainsText()
                        ? "El portapapeles tiene texto, no una imagen: haz un recorte del plano con Win+Mayús+S y vuelve a pegar."
                        : "El portapapeles no tiene ninguna imagen: haz un recorte del plano con Win+Mayús+S y vuelve a pegar.";
                    return false;
                }
                BitmapSource? image = Clipboard.GetImage();
                if (image == null)
                {
                    error = "La imagen del portapapeles no se pudo leer.";
                    return false;
                }
                Directory.CreateDirectory(Folder);
                string safe = SafeName(documentTitle);
                path = Path.Combine(Folder, "plano-" + safe + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".png");
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    encoder.Save(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                error = "No se pudo guardar la imagen del portapapeles: " + ex.Message;
                JsonLineLogger.Write(new { @event = "drawing_pane_failed", from = "clipboard", error = ex.ToString() });
                return false;
            }
        }

        private static string SafeName(string? title)
        {
            string text = string.IsNullOrWhiteSpace(title) ? "documento" : title!.Trim();
            char[] invalid = Path.GetInvalidFileNameChars();
            var chars = text.Select(c => invalid.Contains(c) || c == ' ' ? '_' : c).ToArray();
            string safe = new string(chars);
            return safe.Length > 40 ? safe.Substring(0, 40) : safe;
        }

        private void UpdateEmpty()
        {
            EmptyText.Visibility = HasImage ? Visibility.Collapsed : Visibility.Visible;
            FitButton.IsEnabled = HasImage;
        }

        private void OnFit(object sender, RoutedEventArgs e) => Fit();

        private void OnHide(object sender, RoutedEventArgs e) => HideRequested?.Invoke(this, EventArgs.Empty);

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_fitted) Fit();
        }

        private void OnWheel(object sender, MouseWheelEventArgs e)
        {
            if (!HasImage) return;
            Point cursor = e.GetPosition(Viewport);
            double factor = Math.Pow(1.15, e.Delta / 120.0);
            double newScale = Clamp(Zoom.ScaleX * factor, MinScale, MaxScale);
            factor = newScale / Zoom.ScaleX;
            Pan.X = cursor.X - (cursor.X - Pan.X) * factor;
            Pan.Y = cursor.Y - (cursor.Y - Pan.Y) * factor;
            Zoom.ScaleX = Zoom.ScaleY = newScale;
            _fitted = true;
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!HasImage || (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Middle)) return;
            _dragStart = e.GetPosition(Viewport);
            _dragX = Pan.X;
            _dragY = Pan.Y;
            Viewport.CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragStart.HasValue) return;
            Point current = e.GetPosition(Viewport);
            Pan.X = _dragX + current.X - _dragStart.Value.X;
            Pan.Y = _dragY + current.Y - _dragStart.Value.Y;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_dragStart.HasValue) return;
            _dragStart = null;
            Viewport.ReleaseMouseCapture();
            e.Handled = true;
        }

        private static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
    }
}
