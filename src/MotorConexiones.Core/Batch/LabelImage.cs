using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MotorConexiones.Core.Batch
{
    /// <summary>
    /// La imagen de una etiqueta del plan en la vista de Revit (Fase 10, mejora V3): un BMP de <b>24 bits</b> de 32×32
    /// escrito a mano (lo que el sondeo 19 v3 y v4 demostró que Revit pinta en el lienzo; el de 32 bits de la v2 no se vio),
    /// con un círculo del color del estado del nudo (los mismos RGB de <see cref="PlanAdvice"/>), un anillo blanco y la cifra
    /// del nudo en blanco con una fuente de píxeles de 5×7 (hasta tres cifras). La versión <b>resaltada</b> (etiqueta
    /// pinchada o fila elegida en la ventana) es la inversa: círculo blanco, anillo grueso y cifra del color del estado. Sin
    /// <c>System.Drawing</c>: se prueba en la nube byte a byte.
    /// </summary>
    public static class LabelImage
    {
        public const int Size = 32;

        /// <summary>Tamaño del archivo: cabecera de 54 bytes más 32 filas de 96 bytes (sin relleno: 96 es múltiplo de 4).</summary>
        public const int FileSize = 54 + Size * Size * 3;

        private static readonly (byte R, byte G, byte B) White = (255, 255, 255);

        private static readonly Dictionary<char, string[]> Font = new Dictionary<char, string[]>
        {
            ['0'] = new[] { "01110", "10001", "10011", "10101", "11001", "10001", "01110" },
            ['1'] = new[] { "00100", "01100", "00100", "00100", "00100", "00100", "01110" },
            ['2'] = new[] { "01110", "10001", "00001", "00010", "00100", "01000", "11111" },
            ['3'] = new[] { "11111", "00010", "00100", "00010", "00001", "10001", "01110" },
            ['4'] = new[] { "00010", "00110", "01010", "10010", "11111", "00010", "00010" },
            ['5'] = new[] { "11111", "10000", "11110", "00001", "00001", "10001", "01110" },
            ['6'] = new[] { "00110", "01000", "10000", "11110", "10001", "10001", "01110" },
            ['7'] = new[] { "11111", "00001", "00010", "00100", "01000", "01000", "01000" },
            ['8'] = new[] { "01110", "10001", "10001", "01110", "10001", "10001", "01110" },
            ['9'] = new[] { "01110", "10001", "10001", "01111", "00001", "00010", "01100" },
            ['-'] = new[] { "00000", "00000", "00000", "11111", "00000", "00000", "00000" },
            ['?'] = new[] { "01110", "10001", "00001", "00010", "00100", "00000", "00100" },
        };

        /// <summary>La cifra que lleva la etiqueta: "N4" → "4", "N12" → "12", "N5-2" → "5"; sin cifras, el nombre recortado a tres caracteres.</summary>
        public static string NumberOf(string nodeName)
        {
            string name = (nodeName ?? string.Empty).Trim();
            int start = 0;
            while (start < name.Length && !char.IsDigit(name[start])) start++;
            string digits = new string(name.Skip(start).TakeWhile(char.IsDigit).ToArray());
            if (digits.Length > 0) return digits.Length > 3 ? digits.Substring(digits.Length - 3) : digits;
            string fallback = name.StartsWith("N", StringComparison.OrdinalIgnoreCase) && name.Length > 1 ? name.Substring(1) : name;
            if (fallback.Length == 0) fallback = "?";
            return fallback.Length > 3 ? fallback.Substring(0, 3) : fallback;
        }

        /// <summary>Nombre de archivo estable: <c>etiqueta-verde-4.bmp</c>, <c>etiqueta-verde-4-sel.bmp</c> (resaltada).</summary>
        public static string FileName(string number, string colorName, bool highlighted) =>
            "etiqueta-" + Safe(colorName) + "-" + Safe(number) + (highlighted ? "-sel" : "") + ".bmp";

        /// <summary>La etiqueta de un nudo: círculo del color del estado y cifra en blanco; resaltada = al revés.</summary>
        public static byte[] ForNode(string number, string colorName, bool highlighted)
        {
            var color = PlanAdvice.Rgb(colorName);
            return highlighted
                ? Bmp24(number, White, color, color, ringWidth: 3)
                : Bmp24(number, color, White, White, ringWidth: 1);
        }

        /// <summary>Escribe la etiqueta en <paramref name="folder"/> si no existe (o si está rota) y devuelve su ruta.</summary>
        public static string EnsureFile(string folder, string number, string colorName, bool highlighted)
        {
            if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("Hace falta la carpeta de las etiquetas.", nameof(folder));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, FileName(number, colorName, highlighted));
            if (!File.Exists(path) || new FileInfo(path).Length != FileSize) File.WriteAllBytes(path, ForNode(number, colorName, highlighted));
            return path;
        }

        /// <summary>
        /// BMP de 24 bits de 32×32: fondo blanco, círculo de <paramref name="fill"/>, anillo de <paramref name="ring"/> de
        /// <paramref name="ringWidth"/> píxeles y el texto centrado en <paramref name="text"/> color. Píxeles fila a fila
        /// de abajo arriba, como manda el formato (BGR).
        /// </summary>
        public static byte[] Bmp24(string text, (byte R, byte G, byte B) fill, (byte R, byte G, byte B) ring, (byte R, byte G, byte B) textColor, int ringWidth)
        {
            var pixels = new (byte R, byte G, byte B)[Size, Size];
            double center = (Size - 1) / 2.0;
            double radius = Size / 2.0 - 1.5;
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - center, dy = y - center;
                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    if (distance > radius) pixels[y, x] = White;
                    else if (distance > radius - ringWidth) pixels[y, x] = ring;
                    else pixels[y, x] = fill;
                }
            }
            DrawText(pixels, text ?? string.Empty, textColor);

            var bytes = new byte[FileSize];
            int rowBytes = Size * 3;
            bytes[0] = (byte)'B';
            bytes[1] = (byte)'M';
            WriteInt(bytes, 2, FileSize);
            WriteInt(bytes, 10, 54);
            WriteInt(bytes, 14, 40);
            WriteInt(bytes, 18, Size);
            WriteInt(bytes, 22, Size);
            bytes[26] = 1;
            bytes[28] = 24;
            WriteInt(bytes, 34, rowBytes * Size);
            WriteInt(bytes, 38, 2835);
            WriteInt(bytes, 42, 2835);
            int offset = 54;
            for (int y = Size - 1; y >= 0; y--)
            {
                for (int x = 0; x < Size; x++)
                {
                    var p = pixels[y, x];
                    bytes[offset++] = p.B;
                    bytes[offset++] = p.G;
                    bytes[offset++] = p.R;
                }
            }
            return bytes;
        }

        /// <summary>Lee el píxel (x, y) de un BMP escrito por <see cref="Bmp24"/> (fila 0 arriba), para las pruebas.</summary>
        public static (byte R, byte G, byte B) PixelAt(byte[] bmp, int x, int y)
        {
            if (bmp == null || bmp.Length != FileSize) throw new ArgumentException("No es una etiqueta de 32×32 y 24 bits.", nameof(bmp));
            int row = Size - 1 - y;
            int offset = 54 + row * Size * 3 + x * 3;
            return (bmp[offset + 2], bmp[offset + 1], bmp[offset]);
        }

        private static void DrawText((byte R, byte G, byte B)[,] pixels, string text, (byte R, byte G, byte B) color)
        {
            string shown = text.Length > 3 ? text.Substring(0, 3) : text;
            if (shown.Length == 0) return;
            int scale = shown.Length <= 2 ? 2 : 1;
            int gap = scale;
            int glyphWidth = 5 * scale, glyphHeight = 7 * scale;
            int totalWidth = shown.Length * glyphWidth + (shown.Length - 1) * gap;
            int left = (Size - totalWidth) / 2;
            int top = (Size - glyphHeight) / 2;
            for (int i = 0; i < shown.Length; i++)
            {
                string[] glyph = Font.TryGetValue(shown[i], out string[]? g) ? g : Font['?'];
                int originX = left + i * (glyphWidth + gap);
                for (int row = 0; row < 7; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        if (glyph[row][col] != '1') continue;
                        for (int sy = 0; sy < scale; sy++)
                        {
                            for (int sx = 0; sx < scale; sx++)
                            {
                                int x = originX + col * scale + sx;
                                int y = top + row * scale + sy;
                                if (x >= 0 && x < Size && y >= 0 && y < Size) pixels[y, x] = color;
                            }
                        }
                    }
                }
            }
        }

        private static void WriteInt(byte[] bytes, int offset, int value)
        {
            bytes[offset] = (byte)(value & 0xFF);
            bytes[offset + 1] = (byte)((value >> 8) & 0xFF);
            bytes[offset + 2] = (byte)((value >> 16) & 0xFF);
            bytes[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        private static string Safe(string text)
        {
            var clean = new string((text ?? string.Empty).Select(c => char.IsLetterOrDigit(c) && c < 128 ? char.ToLowerInvariant(c) : '-').ToArray()).Trim('-');
            return clean.Length == 0 ? "x" : clean;
        }

        /// <summary>Texto para el log: "32x32, 24 bits, 3126 bytes".</summary>
        public static string Describe(byte[] bmp) =>
            Size + "x" + Size + ", 24 bits, " + (bmp?.Length ?? 0).ToString(CultureInfo.InvariantCulture) + " bytes";
    }
}
