using System.Collections.Generic;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// La cercha que la persona seleccionó en la ronda 8b (56 barras, paso 8b-4 de <c>docs/fases/resultados-fase-8b.md</c>): la
    /// cercha gemela del Hangar en Y = +17204 (la de <see cref="HangarTruss"/> está en Y = −17196), con el **cordón central
    /// entero** en ocho tramos (siete HSS4X4 de 101,6 mm y el último HSS3X3 de 76,2 mm) y sus 48 diagonales HSS 2-1/2 de
    /// 63,5 mm; sin cordones superior ni inferior. A diferencia de <see cref="HangarTruss"/> (reconstruida de los puntos de
    /// trabajo), aquí los ejes son los extremos reales de la <c>LocationCurve</c> que imprimió el sondeo 18 en Revit (0,1 mm),
    /// y los tipos y cantos son los que leyó <c>RevitModelFacts</c>. En el PC, <c>batch_plan</c> con la plantilla oficial dio
    /// 59 nudos: 16 <c>ready</c> (8 <c>same</c> y 8 <c>mirror_x</c>), 20 <c>no_match</c> (parejas en K de los cordones superior
    /// e inferior, sin cordón en la selección) y 23 <c>untyped</c> (14 extremos de tramos de cordón, el empalme 1250938/1250939
    /// y 8 extremos lejanos de diagonales), en 953 ms. Las pruebas comprueban que la nube reproduce exactamente eso.
    /// </summary>
    public static class HangarTruss8b
    {
        public const string BigChordType = HangarTruss.BigChordType;
        public const string SmallChordType = "HSS3X3X1-4 76x76";
        public const string DiagonalType = SyntheticTruss.DiagonalType;

        /// <summary>El gemelo del Detalle D (N4 en el PC): cordón HSS4X4 1250933 con 1251053 (136,9°), 1251054 (44,4°) y 1251059 (−135,6°).</summary>
        public const long DetalleDChord = 1250933;
        public const long DetalleDUpLeft = 1251053;
        public const long DetalleDUpRight = 1251054;
        public const long DetalleDLower = 1251059;

        /// <summary>El nudo siguiente del mismo cordón (N7 en el PC, <c>mirror_x</c>): 1251055 (135°), 1251056 (44,4°) y 1251060 (−44,4°).</summary>
        public const long MirrorUpLeft = 1251055;
        public const long MirrorUpRight = 1251056;
        public const long MirrorLower = 1251060;

        /// <summary>El único tramo HSS3X3 del cordón central (N53 y N56 en el PC: sin aviso de perfil).</summary>
        public const long SmallChord = 1250939;

        /// <summary>El empalme del cordón central (N52 en el PC: dos barras paralelas, <c>untyped</c>).</summary>
        public const long SpliceLeft = 1250938;

        public static double DepthOf(string type) => type == BigChordType ? 101.6 : type == SmallChordType ? 76.2 : SyntheticTruss.DiagonalDepth;

        private static DetectorBar Bar(long id, double x0, double y0, double z0, double x1, double y1, double z1, string type) =>
            new DetectorBar(id, new Vec3(x0, y0, z0), new Vec3(x1, y1, z1), type, DepthOf(type));

        /// <summary>Las 56 barras en el orden del sondeo 18 (cordones 1250932…1250939 y diagonales 1251049…1251725).</summary>
        public static List<DetectorBar> Bars() => new List<DetectorBar>
        {
            Bar(1250932, 5812.7, 17204.2, 17423.0, -4022.5, 17204.3, 17423.0, BigChordType),
            Bar(1250933, -4437.3, 17204.3, 17423.0, -14397.6, 17204.3, 17423.0, BigChordType), /* extensiones -3.0 / -1.7 */
            Bar(1250934, 16062.7, 17204.2, 17423.0, 6227.5, 17204.2, 17423.0, BigChordType),
            Bar(1250935, 26312.7, 17204.2, 17423.0, 16477.5, 17204.2, 17423.0, BigChordType),
            Bar(1250936, 36562.7, 17204.2, 17423.0, 26727.5, 17204.2, 17423.0, BigChordType),
            Bar(1250937, 46812.7, 17204.1, 17423.0, 36977.5, 17204.2, 17423.0, BigChordType),
            Bar(1250938, 57319.9, 17204.1, 17423.0, 47327.1, 17204.1, 17423.0, BigChordType),
            Bar(1250939, 67437.8, 17204.1, 17423.0, 57319.9, 17204.1, 17423.0, SmallChordType),
            Bar(1251049, -4163.2, 17204.3, 19916.4, -1632.3, 17204.3, 17433.7, DiagonalType),
            Bar(1251050, -1606.5, 17204.3, 17437.5, 927.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251051, 961.8, 17204.3, 19916.4, 3493.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251052, 3519.0, 17204.2, 17436.9, 6052.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251053, -14536.8, 17204.4, 19918.8, -11930.6, 17204.4, 17481.8, DiagonalType), /* extensiones 0.0 / 68.6 */
            Bar(1251054, -11856.5, 17204.4, 17437.3, -9354.7, 17204.4, 19884.9, DiagonalType), /* extensiones 0.0 / 69.2 */
            Bar(1251055, -9277.8, 17204.4, 19960.1, -6770.4, 17204.3, 17452.7, DiagonalType),
            Bar(1251056, -6745.0, 17204.3, 17418.7, -4197.1, 17204.3, 19916.4, DiagonalType),
            Bar(1251059, -14455.3, 17204.3, 14894.7, -11904.9, 17204.3, 17389.9, DiagonalType),
            Bar(1251060, -6770.4, 17204.2, 17452.7, -4220.3, 17204.2, 14957.2, DiagonalType),
            Bar(1251061, -4141.0, 17204.2, 14958.5, -1606.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251062, 3477.2, 17204.2, 17450.0, 6024.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251690, 6086.8, 17204.3, 19916.4, 8617.7, 17204.3, 17433.7, DiagonalType),
            Bar(1251691, 8643.5, 17204.3, 17437.5, 11177.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251692, 11211.8, 17204.3, 19916.4, 13743.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251693, 13769.0, 17204.2, 17436.9, 16302.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251694, 6109.0, 17204.2, 14958.5, 8643.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251695, 13727.2, 17204.2, 17450.0, 16274.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251696, 16336.8, 17204.3, 19916.4, 18867.7, 17204.3, 17433.7, DiagonalType),
            Bar(1251697, 18893.5, 17204.3, 17437.5, 21427.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251698, 21461.8, 17204.3, 19916.4, 23993.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251699, 24019.0, 17204.2, 17436.9, 26552.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251700, 16359.0, 17204.2, 14958.5, 18893.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251701, 23977.2, 17204.2, 17450.0, 26524.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251702, 26586.8, 17204.3, 19916.4, 29117.7, 17204.3, 17433.7, DiagonalType),
            Bar(1251703, 29143.5, 17204.3, 17437.5, 31677.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251704, 31711.8, 17204.3, 19916.4, 34243.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251705, 34269.0, 17204.2, 17436.9, 36802.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251706, 26609.0, 17204.2, 14958.5, 29143.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251707, 34227.2, 17204.2, 17450.0, 36774.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251708, 36836.8, 17204.3, 19916.4, 39367.7, 17204.3, 17433.7, DiagonalType),
            Bar(1251709, 39393.5, 17204.3, 17437.5, 41927.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251710, 41961.8, 17204.3, 19916.4, 44493.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251711, 44519.0, 17204.2, 17436.9, 47052.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251712, 36859.0, 17204.2, 14958.5, 39393.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251713, 44477.2, 17204.2, 17450.0, 47024.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251714, 47086.8, 17204.3, 19916.4, 49617.7, 17204.3, 17433.7, DiagonalType),
            Bar(1251715, 49643.5, 17204.3, 17437.5, 52177.9, 17204.3, 19916.5, DiagonalType),
            Bar(1251716, 52211.8, 17204.3, 19916.4, 54743.2, 17204.2, 17434.3, DiagonalType),
            Bar(1251717, 54769.0, 17204.2, 17436.9, 57302.9, 17204.2, 19916.5, DiagonalType),
            Bar(1251718, 47109.0, 17204.2, 14958.5, 49643.5, 17204.2, 17437.5, DiagonalType),
            Bar(1251719, 54727.2, 17204.2, 17450.0, 57274.8, 17204.2, 14952.1, DiagonalType),
            Bar(1251720, 57213.5, 17204.4, 19918.8, 59819.6, 17204.4, 17481.8, DiagonalType), /* extensiones 0.0 / 68.6 */
            Bar(1251721, 59893.7, 17204.4, 17437.3, 62395.5, 17204.4, 19884.9, DiagonalType), /* extensiones 0.0 / 69.2 */
            Bar(1251722, 62472.4, 17204.4, 19960.1, 64979.9, 17204.3, 17452.7, DiagonalType),
            Bar(1251723, 65005.3, 17204.3, 17418.7, 67553.1, 17204.3, 19916.4, DiagonalType),
            Bar(1251724, 57294.9, 17204.3, 14894.7, 59845.3, 17204.3, 17389.9, DiagonalType),
            Bar(1251725, 64979.9, 17204.2, 17452.7, 67529.9, 17204.2, 14957.2, DiagonalType),
        };

        public static SyntheticTrussFacts Facts() => new SyntheticTrussFacts(Bars());
    }
}
