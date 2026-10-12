using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// Fase 12: la cercha que la persona planificó en la ronda 8c (64 barras, <c>docs/fases/resultados-fase-8c.md</c>, paso 8c-3):
    /// las 56 barras de <see cref="HangarTruss8b"/> (cordón central en ocho tramos y 48 diagonales) más los <b>ocho tramos del
    /// cordón superior</b> HSS12X8X1/2 en Z = 19933, reconstruidos de los puntos de trabajo del plan: los tramos se juntan en los
    /// siete empalmes (N9, N16, N23, N30, N37, N44 y N52 del PC: X = −4180,2; 6069,8; 16319,8; 26569,8; 36819,8; 47069,8 y 57319,8)
    /// y los extremos están en X = −14551,9 (N1) y 67794,9 (N59). En el PC (0.8.3, sin la Fase 12) salieron 59 nudos: 16 ready, 25
    /// no_match (los 8 nudos en K del cordón superior, los 7 empalmes con sus diagonales, N49 y N58 con una diagonal y 8 parejas sin
    /// cordón) y 18 untyped. Con la Fase 12 los nudos en K llevan soldadura directa y los empalmes el Detalle C.
    /// </summary>
    public static class HangarTruss8c
    {
        public const string TopChordType = "HSS12X8X1/2";
        public const double TopChordZ = 19933.0;

        /// <summary>Los ocho tramos del cordón superior, de izquierda a derecha.</summary>
        public static readonly long[] TopChordIds = { 1245531, 1245530, 1245534, 1245536, 1245538, 1245540, 1245542, 1245544 };

        /// <summary>X donde se juntan los tramos (los siete empalmes).</summary>
        public static readonly double[] SpliceX = { -4180.2, 6069.8, 16319.8, 26569.8, 36819.8, 47069.8, 57319.8 };

        public const double LeftEndX = -14551.9;
        public const double RightEndX = 67794.9;

        /// <summary>El nudo en K del paso 8c con excentricidad (N5 en el PC): cordón 1245531 con 1251054 y 1251055.</summary>
        public const long EccentricKChord = 1245531;
        public const long EccentricKLeft = 1251054;
        public const long EccentricKRight = 1251055;

        /// <summary>El primer empalme con nudo en K (N9 en el PC): tramos 1245531 y 1245530, diagonales 1251056 y 1251049.</summary>
        public const long FirstSpliceLeft = 1245531;
        public const long FirstSpliceRight = 1245530;
        public const long FirstSpliceDiagonalLeft = 1251056;
        public const long FirstSpliceDiagonalRight = 1251049;

        public static List<DetectorBar> TopChord()
        {
            var bars = new List<DetectorBar>();
            double[] xs = new[] { LeftEndX }.Concat(SpliceX).Concat(new[] { RightEndX }).ToArray();
            double[] ys = { 17204.3, 17204.3, 17204.2, 17204.2, 17204.2, 17204.2, 17204.2, 17204.1 };
            for (int i = 0; i < TopChordIds.Length; i++)
            {
                bars.Add(new DetectorBar(TopChordIds[i], new Vec3(xs[i], ys[i], TopChordZ), new Vec3(xs[i + 1], ys[i], TopChordZ), TopChordType, 304.8));
            }
            return bars;
        }

        /// <summary>Las 64 barras: las 56 de la 8b y los 8 tramos del cordón superior.</summary>
        public static List<DetectorBar> Bars() => HangarTruss8b.Bars().Concat(TopChord()).ToList();

        public static SyntheticTrussFacts Facts() => new SyntheticTrussFacts(Bars());
    }
}
