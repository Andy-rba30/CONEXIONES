using System;
using System.IO;
using MotorConexiones.Core.Batch;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Fase 10, mejora V3: la etiqueta del lienzo es un BMP de 24 bits de 32×32 escrito a mano, como el del sondeo 19 v3/v4.</summary>
    public class LabelImageTests
    {
        private static readonly (byte R, byte G, byte B) Green = PlanAdvice.Rgb(PlanAdvice.Green);
        private static readonly (byte R, byte G, byte B) White = (255, 255, 255);

        [Fact]
        public void ForNode_WritesA24BitBmpOf32By32WithTheStateColorAndAWhiteDigit()
        {
            byte[] bmp = LabelImage.ForNode("4", PlanAdvice.Green, highlighted: false);

            Assert.Equal(LabelImage.FileSize, bmp.Length);
            Assert.Equal(3126, bmp.Length);
            Assert.Equal((byte)'B', bmp[0]);
            Assert.Equal((byte)'M', bmp[1]);
            Assert.Equal(3126, BitConverter.ToInt32(bmp, 2));
            Assert.Equal(54, BitConverter.ToInt32(bmp, 10));
            Assert.Equal(32, BitConverter.ToInt32(bmp, 18));
            Assert.Equal(32, BitConverter.ToInt32(bmp, 22));
            Assert.Equal(1, BitConverter.ToInt16(bmp, 26));
            Assert.Equal(24, BitConverter.ToInt16(bmp, 28));
            Assert.Equal(32 * 32 * 3, BitConverter.ToInt32(bmp, 34));

            Assert.Equal(White, LabelImage.PixelAt(bmp, 0, 0));        // fuera del círculo
            Assert.Equal(White, LabelImage.PixelAt(bmp, 31, 31));
            Assert.Equal(White, LabelImage.PixelAt(bmp, 15, 2));       // anillo blanco
            Assert.Equal(Green, LabelImage.PixelAt(bmp, 4, 15));       // relleno verde
            Assert.Equal(White, LabelImage.PixelAt(bmp, 15, 17));      // la barra horizontal del 4, en blanco
            Assert.Equal(Green, LabelImage.PixelAt(bmp, 12, 10));      // esquina superior izquierda del glifo del 4: sin tinta
        }

        [Fact]
        public void ForNode_HighlightedIsInverted()
        {
            byte[] bmp = LabelImage.ForNode("4", PlanAdvice.Green, highlighted: true);
            Assert.Equal(White, LabelImage.PixelAt(bmp, 8, 15));       // relleno blanco (a 7,5 px del centro)
            Assert.Equal(Green, LabelImage.PixelAt(bmp, 4, 15));       // el anillo grueso (3 px) llega hasta 11,5 px del centro
            Assert.Equal(Green, LabelImage.PixelAt(bmp, 15, 17));      // cifra verde
            Assert.Equal(Green, LabelImage.PixelAt(bmp, 15, 2));       // anillo grueso verde
            Assert.Equal(White, LabelImage.PixelAt(bmp, 0, 0));
        }

        [Fact]
        public void ForNode_DrawsTwoAndThreeDigits()
        {
            byte[] two = LabelImage.ForNode("16", PlanAdvice.Amber, false);
            // "16" a escala 2: 22 px de ancho desde x = 5; el 1 es la columna central del primer glifo (x = 9..10).
            Assert.Equal(White, LabelImage.PixelAt(two, 9, 15));
            Assert.Equal(PlanAdvice.Rgb(PlanAdvice.Amber), LabelImage.PixelAt(two, 5, 15));
            byte[] three = LabelImage.ForNode("123", PlanAdvice.Red, false);
            Assert.Equal(LabelImage.FileSize, three.Length);
            // A escala 1 el texto ocupa 17 px desde x = 7; la cifra 1 pinta su columna central en x = 9 a media altura (y = 15).
            Assert.Equal(White, LabelImage.PixelAt(three, 9, 15));
            Assert.Equal(PlanAdvice.Rgb(PlanAdvice.Red), LabelImage.PixelAt(three, 7, 15));
        }

        [Theory]
        [InlineData("N4", "4")]
        [InlineData("N12", "12")]
        [InlineData("N5-2", "5")]
        [InlineData("n7", "7")]
        [InlineData("N1234", "234")]
        [InlineData("Nabc", "abc")]
        [InlineData("", "?")]
        public void NumberOf_TakesTheDigitsOfTheNodeName(string name, string expected)
        {
            Assert.Equal(expected, LabelImage.NumberOf(name));
        }

        [Fact]
        public void FileName_IsStableAndSafe()
        {
            Assert.Equal("etiqueta-verde-4.bmp", LabelImage.FileName("4", PlanAdvice.Green, false));
            Assert.Equal("etiqueta-ambar-16-sel.bmp", LabelImage.FileName("16", PlanAdvice.Amber, true));
            Assert.Equal("etiqueta-x-5-2.bmp", LabelImage.FileName("5/2", "", false));
        }

        [Fact]
        public void EnsureFile_WritesOnceAndReusesTheFile()
        {
            string folder = Path.Combine(Path.GetTempPath(), "motorconexiones-etiquetas-" + Guid.NewGuid().ToString("N"));
            try
            {
                string path = LabelImage.EnsureFile(folder, "4", PlanAdvice.Green, false);
                Assert.True(File.Exists(path));
                Assert.Equal(LabelImage.FileSize, new FileInfo(path).Length);
                DateTime written = File.GetLastWriteTimeUtc(path);
                string again = LabelImage.EnsureFile(folder, "4", PlanAdvice.Green, false);
                Assert.Equal(path, again);
                Assert.Equal(written, File.GetLastWriteTimeUtc(path));
                File.WriteAllBytes(path, new byte[] { 1, 2, 3 });
                LabelImage.EnsureFile(folder, "4", PlanAdvice.Green, false);
                Assert.Equal(LabelImage.FileSize, new FileInfo(path).Length);
                Assert.Equal("32x32, 24 bits, 3126 bytes", LabelImage.Describe(File.ReadAllBytes(path)));
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }
    }
}
