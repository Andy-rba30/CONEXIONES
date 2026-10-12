using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Dónde termina una barra soldada directamente a la cara del cordón (Fase 12), en el plano local del nudo (mm).</summary>
    public sealed class DirectWeldEnd
    {
        public DirectWeldEnd(BoltPosition cut, BoltPosition facePoint, double faceY, BoltPosition footprintStart, BoltPosition footprintEnd, double setbackMm, double axisSetbackMm)
        {
            Cut = cut;
            FacePoint = facePoint;
            FaceY = faceY;
            FootprintStart = footprintStart;
            FootprintEnd = footprintEnd;
            SetbackMm = setbackMm;
            AxisSetbackMm = axisSetbackMm;
        }

        /// <summary>Corte del eje de la barra con el eje del cordón (el punto de trabajo desplazado <c>axis_offset_mm</c>).</summary>
        public BoltPosition Cut { get; }
        /// <summary>Punto donde el eje de la barra atraviesa la cara del cordón.</summary>
        public BoltPosition FacePoint { get; }
        /// <summary>Y local de la cara (±medio canto del cordón).</summary>
        public double FaceY { get; }
        /// <summary>Extremos de la huella de la barra sobre la cara (donde va la soldadura).</summary>
        public BoltPosition FootprintStart { get; }
        public BoltPosition FootprintEnd { get; }
        /// <summary>Distancia del corte del eje al punto de la cara, a lo largo de la barra, más la holgura: el retiro ideal (corte inclinado).</summary>
        public double SetbackMm { get; }
        /// <summary>
        /// Retiro por el eje (la reserva si no hay corte inclinado): deja la esquina más cercana de la barra fuera del cordón,
        /// así la barra no penetra en él aunque su extremo sea perpendicular a su eje.
        /// </summary>
        public double AxisSetbackMm { get; }
        public double FootprintLengthMm => FootprintStart.X < FootprintEnd.X ? FootprintEnd.X - FootprintStart.X : FootprintStart.X - FootprintEnd.X;
    }

    /// <summary>
    /// Fase 12: la geometría de <c>direct_weld</c>. La barra sale de su corte con el eje del cordón (desplazado
    /// <c>axis_offset_mm</c> del punto de trabajo) con dirección (ux, uy) y termina en la cara del cordón del lado al que
    /// va (y = ±medio canto). La huella sobre la cara mide ancho / |uy|; la soldadura es una línea sobre la cara a lo largo
    /// de la huella (todo el contorno se representa así en el plano).
    /// </summary>
    public static class DirectWeldGeometry
    {
        public static DirectWeldEnd ComputeEnd(double ux, double uy, double axisOffsetMm, double chordHalfDepthMm, double memberWidthMm, double endGapMm)
        {
            double absUy = Math.Max(Math.Abs(uy), 1e-6);
            double sign = uy >= 0 ? 1.0 : -1.0;
            double faceY = sign * chordHalfDepthMm;
            double t = chordHalfDepthMm / absUy; // distancia del corte a la cara a lo largo de la barra
            double setback = t + Math.Max(0.0, endGapMm);
            var cut = new BoltPosition(axisOffsetMm, 0.0);
            var facePoint = new BoltPosition(axisOffsetMm + ux * setback, sign * (chordHalfDepthMm + Math.Max(0.0, endGapMm) * absUy));
            double halfFootprint = (memberWidthMm / 2.0) / absUy;
            var a = new BoltPosition(axisOffsetMm + ux * t - halfFootprint, faceY);
            var b = new BoltPosition(axisOffsetMm + ux * t + halfFootprint, faceY);
            // Retiro por el eje: la esquina más cercana a la cara queda justo en la cara (más la holgura).
            double axisSetback = t + (memberWidthMm / 2.0) * Math.Abs(ux) / absUy + Math.Max(0.0, endGapMm);
            return new DirectWeldEnd(cut, facePoint, faceY, a, b, setback, axisSetback);
        }

        /// <summary>La línea de soldadura de una barra sobre la cara del cordón (para el backend y el croquis).</summary>
        public static WeldLine2D WeldLine(DirectWeldEnd end, double sizeMm) => new WeldLine2D(end.FootprintStart, end.FootprintEnd, sizeMm);

        /// <summary>Verdadero si las huellas de dos barras sobre la misma cara se solapan (más de <paramref name="toleranceMm"/>).</summary>
        public static bool FootprintsOverlap(DirectWeldEnd a, DirectWeldEnd b, double toleranceMm = 1.0)
        {
            if (Math.Abs(a.FaceY - b.FaceY) > 1e-6) return false;
            double aMin = Math.Min(a.FootprintStart.X, a.FootprintEnd.X), aMax = Math.Max(a.FootprintStart.X, a.FootprintEnd.X);
            double bMin = Math.Min(b.FootprintStart.X, b.FootprintEnd.X), bMax = Math.Max(b.FootprintStart.X, b.FootprintEnd.X);
            return Math.Min(aMax, bMax) - Math.Max(aMin, bMin) > toleranceMm;
        }

        /// <summary>Hueco entre dos huellas vecinas sobre la cara (negativo si se solapan).</summary>
        public static double FootprintGapMm(DirectWeldEnd a, DirectWeldEnd b)
        {
            double aMin = Math.Min(a.FootprintStart.X, a.FootprintEnd.X), aMax = Math.Max(a.FootprintStart.X, a.FootprintEnd.X);
            double bMin = Math.Min(b.FootprintStart.X, b.FootprintEnd.X), bMax = Math.Max(b.FootprintStart.X, b.FootprintEnd.X);
            return Math.Max(aMin, bMin) - Math.Min(aMax, bMax);
        }
    }
}
