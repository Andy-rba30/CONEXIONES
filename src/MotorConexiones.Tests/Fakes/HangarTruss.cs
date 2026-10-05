using System.Collections.Generic;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// La cercha real del Hangar (53 barras seleccionadas en el paso 8-4 de <c>docs/fases/resultados-fase-8.md</c>), reconstruida
    /// a partir de los puntos de trabajo del plan del PC: cada barra aparecía como extremo suelto (<c>untyped</c>) en sus dos
    /// nudos, así que sus dos extremos de <c>LocationCurve</c> están ahí con 0,1 mm. Es la prueba de la ronda 8b: las diagonales
    /// terminan en la cara del cordón (a 15–85 mm de su eje), no en su eje, y la agrupación a 10 mm no las juntaba.
    /// <list type="bullet">
    /// <item>Cordón central en Z = 17423 en cinco tramos (1249510, 1249509, 1249511, 1249515 y 1249516); entre X = 16063 y
    /// X = 47327 no se seleccionó cordón, y tampoco los cordones superior (Z ≈ 19916) ni inferior (Z ≈ 14955).</item>
    /// <item>Diagonales a ±44,4° entre los tres niveles; en el cordón central se juntan de tres en tres (dos arriba y una
    /// abajo, como el Detalle D, o una arriba y dos abajo: el nudo en espejo).</item>
    /// <item>Tipos: los que devolvió el plan; las barras marcadas "tipo supuesto" no salieron en ningún nudo con marco y se
    /// les asigna el tipo de su familia (cordón HSS3X3X1/4 de 76,2 mm; diagonal HSS 2-1/2 de 63,5 mm).</item>
    /// </list>
    /// </summary>
    public static class HangarTruss
    {
        public const string ChordType = SyntheticTruss.ChordType;
        public const string DiagonalType = SyntheticTruss.DiagonalType;
        public const string BigChordType = "HSS4X4X3-16 102x102";

        /// <summary>Cordón del Detalle D y sus tres diagonales (las de <c>docs/fixtures/detalle-D.json</c>).</summary>
        public const long DetalleDChord = 1249510;
        public const long DetalleDUpLeft = 1249630;
        public const long DetalleDUpRight = 1249631;
        public const long DetalleDLower = 1249636;

        /// <summary>El nudo siguiente del mismo cordón (el simétrico que la Fase 7 creó en espejo): 1249632, 1249633 y 1249637.</summary>
        public const long MirrorUpLeft = 1249632;
        public const long MirrorUpRight = 1249633;
        public const long MirrorLower = 1249637;

        public static double DepthOf(string type) => type == BigChordType ? 101.6 : type == ChordType ? SyntheticTruss.ChordDepth : SyntheticTruss.DiagonalDepth;

        private static DetectorBar Bar(long id, double x0, double y0, double z0, double x1, double y1, double z1, string type) =>
            new DetectorBar(id, new Vec3(x0, y0, z0), new Vec3(x1, y1, z1), type, DepthOf(type));

        public static List<DetectorBar> Bars() => new List<DetectorBar>
        {
            Bar(1249630, -14536.8, -17195.8, 19918.8, -11930.6, -17195.8, 17481.8, DiagonalType) /* tipo supuesto */,
            Bar(1249636, -14455.3, -17195.7, 14894.7, -11904.9, -17195.7, 17389.9, DiagonalType) /* tipo supuesto */,
            Bar(1249510, -14397.6, -17195.8, 17423.0, -4437.3, -17195.7, 17423.0, ChordType),
            Bar(1249631, -11856.5, -17195.8, 17437.3, -9354.7, -17195.8, 19884.9, DiagonalType) /* tipo supuesto */,
            Bar(1249632, -9277.8, -17195.8, 19960.1, -6770.4, -17195.8, 17452.7, DiagonalType),
            Bar(1249637, -6770.4, -17195.8, 17452.7, -4220.3, -17195.7, 14957.2, DiagonalType),
            Bar(1249633, -6745.0, -17195.8, 17418.7, -4197.1, -17195.8, 19916.4, DiagonalType),
            Bar(1249626, -4163.2, -17195.7, 19916.4, -1632.3, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1249638, -4141.0, -17195.7, 14958.5, -1606.5, -17195.7, 17437.5, DiagonalType),
            Bar(1249509, -4022.5, -17195.7, 17423.0, 5812.7, -17195.7, 17423.0, ChordType) /* tipo supuesto */,
            Bar(1249627, -1606.5, -17195.7, 17437.5, 927.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1249628, 961.8, -17195.7, 19916.4, 3493.2, -17195.7, 17434.3, DiagonalType),
            Bar(1249639, 3477.2, -17195.7, 17450.0, 6024.8, -17195.7, 14952.1, DiagonalType),
            Bar(1249629, 3519.0, -17195.7, 17436.9, 6052.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250263, 6086.8, -17195.7, 19916.4, 8617.7, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1250267, 6109.0, -17195.7, 14958.5, 8643.5, -17195.7, 17437.5, DiagonalType),
            Bar(1249511, 6227.5, -17195.7, 17423.0, 16062.7, -17195.7, 17423.0, ChordType) /* tipo supuesto */,
            Bar(1250264, 8643.5, -17195.7, 17437.5, 11177.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250265, 11211.8, -17195.7, 19916.4, 13743.2, -17195.7, 17434.3, DiagonalType),
            Bar(1250268, 13727.2, -17195.7, 17450.0, 16274.8, -17195.7, 14952.1, DiagonalType),
            Bar(1250266, 13769.0, -17195.7, 17436.9, 16302.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250269, 16336.8, -17195.7, 19916.4, 18867.7, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1250273, 16359.0, -17195.7, 14958.5, 18893.5, -17195.7, 17437.5, DiagonalType),
            Bar(1250270, 18893.5, -17195.7, 17437.5, 21427.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250271, 21461.8, -17195.7, 19916.4, 23993.2, -17195.7, 17434.3, DiagonalType),
            Bar(1250274, 23977.2, -17195.7, 17450.0, 26524.8, -17195.7, 14952.1, DiagonalType),
            Bar(1250272, 24019.0, -17195.7, 17436.9, 26552.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250275, 26586.8, -17195.7, 19916.4, 29117.7, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1250279, 26609.0, -17195.7, 14958.5, 29143.5, -17195.7, 17437.5, DiagonalType),
            Bar(1250276, 29143.5, -17195.7, 17437.5, 31677.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250277, 31711.8, -17195.7, 19916.4, 34243.2, -17195.7, 17434.3, DiagonalType),
            Bar(1250280, 34227.2, -17195.7, 17450.0, 36774.8, -17195.7, 14952.1, DiagonalType),
            Bar(1250278, 34269.0, -17195.7, 17436.9, 36802.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250281, 36836.8, -17195.7, 19916.4, 39367.7, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1250285, 36859.0, -17195.7, 14958.5, 39393.5, -17195.7, 17437.5, DiagonalType),
            Bar(1250282, 39393.5, -17195.7, 17437.5, 41927.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250283, 41961.8, -17195.7, 19916.4, 44493.2, -17195.7, 17434.3, DiagonalType),
            Bar(1250286, 44477.2, -17195.7, 17450.0, 47024.8, -17195.7, 14952.1, DiagonalType),
            Bar(1250284, 44519.0, -17195.7, 17436.9, 47052.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250287, 47086.8, -17195.7, 19916.4, 49617.7, -17195.7, 17433.7, DiagonalType) /* tipo supuesto */,
            Bar(1250291, 47109.0, -17195.7, 14958.5, 49643.5, -17195.7, 17437.5, DiagonalType),
            Bar(1249515, 47327.1, -17195.6, 17423.0, 57319.9, -17195.6, 17423.0, ChordType) /* tipo supuesto */,
            Bar(1250288, 49643.5, -17195.7, 17437.5, 52177.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250289, 52211.8, -17195.7, 19916.4, 54743.2, -17195.7, 17434.3, DiagonalType),
            Bar(1250292, 54727.2, -17195.7, 17450.0, 57274.8, -17195.7, 14952.1, DiagonalType),
            Bar(1250290, 54769.0, -17195.7, 17436.9, 57302.9, -17195.7, 19916.5, DiagonalType) /* tipo supuesto */,
            Bar(1250293, 57213.5, -17195.8, 19918.8, 59819.6, -17195.8, 17481.8, DiagonalType) /* tipo supuesto */,
            Bar(1250297, 57294.9, -17195.7, 14894.7, 59845.3, -17195.7, 17389.9, DiagonalType) /* tipo supuesto */,
            Bar(1249516, 57319.9, -17195.6, 17423.0, 67437.8, -17195.6, 17423.0, BigChordType),
            Bar(1250294, 59893.7, -17195.8, 17437.3, 62395.5, -17195.8, 19884.9, DiagonalType) /* tipo supuesto */,
            Bar(1250295, 62472.4, -17195.8, 19960.1, 64979.9, -17195.8, 17452.7, DiagonalType),
            Bar(1250298, 64979.9, -17195.8, 17452.7, 67529.9, -17195.7, 14957.2, DiagonalType),
            Bar(1250296, 65005.3, -17195.8, 17418.7, 67553.1, -17195.8, 19916.4, DiagonalType)
        };

        public static SyntheticTrussFacts Facts() => new SyntheticTrussFacts(Bars());
    }
}
