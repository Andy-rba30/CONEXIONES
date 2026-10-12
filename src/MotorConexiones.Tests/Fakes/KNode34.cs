using System;
using System.Collections.Generic;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// Fase 12: el nudo en K sintético del cordón superior del Hangar con 34 mm de excentricidad, con los IDs reales del paso
    /// 11-5 (cordón 1245533 HSS12X8X1/2 en Z = 19933; diagonales HSS 2-1/2 1249631, que llega desde abajo a la izquierda, y
    /// 1249632, desde abajo a la derecha). Los ejes de las diagonales cortan al del cordón en X = −9295 y X = −9261: el punto de
    /// trabajo (opción A) queda en X = −9278 y cada diagonal lleva un desvío de ∓17 mm. Las diagonales terminan en la cara del
    /// cordón (101,6 mm bajo su eje: el 8" del HSS12X8 queda en el plano de la cercha).
    /// </summary>
    public static class KNode34
    {
        public const double Y = -17195.8;
        public const double ChordZ = 19933.0;
        public const long Chord = 1245533;
        public const long LeftDiagonal = 1249631;
        public const long RightDiagonal = 1249632;
        public const string ChordType = "HSS12X8X1/2";
        public const string DiagonalType = SyntheticTruss.DiagonalType;

        /// <summary>Punto de trabajo esperado (mitad entre los dos cortes).</summary>
        public static readonly Vec3 WorkPoint = new Vec3(-9278.0, Y, ChordZ);

        public const double EccentricityMm = 34.0;
        public const double HalfDepthMm = 101.6;

        /// <summary>Las tres barras con la excentricidad indicada (34 por defecto; 0 = concurrentes).</summary>
        public static List<DetectorBar> Bars(double eccentricityMm = EccentricityMm)
        {
            double half = eccentricityMm / 2.0;
            double leftCut = -9278.0 - half;
            double rightCut = -9278.0 + half;
            // 1249631 baja hacia la izquierda a 44,4° de inclinación (−135,6° con signo); 1249632 baja hacia la derecha a 45°.
            double ul = Math.Cos(44.4 * Math.PI / 180.0), vl = Math.Sin(44.4 * Math.PI / 180.0);
            double ur = Math.Cos(45.0 * Math.PI / 180.0), vr = Math.Sin(45.0 * Math.PI / 180.0);
            double faceLeft = HalfDepthMm / vl, faceRight = HalfDepthMm / vr;
            return new List<DetectorBar>
            {
                new DetectorBar(Chord, P(-15000, ChordZ), P(-3000, ChordZ), ChordType, 304.8),
                new DetectorBar(LeftDiagonal, P(leftCut - ul * 3500, ChordZ - vl * 3500), P(leftCut - ul * faceLeft, ChordZ - HalfDepthMm), DiagonalType, SyntheticTruss.DiagonalDepth),
                new DetectorBar(RightDiagonal, P(rightCut + ur * faceRight, ChordZ - HalfDepthMm), P(rightCut + ur * 3500, ChordZ - vr * 3500), DiagonalType, SyntheticTruss.DiagonalDepth),
            };
        }

        public static Vec3 P(double x, double z) => new Vec3(x, Y, z);

        public static SyntheticTrussFacts Facts(double eccentricityMm = EccentricityMm) => new SyntheticTrussFacts(Bars(eccentricityMm));
    }
}
