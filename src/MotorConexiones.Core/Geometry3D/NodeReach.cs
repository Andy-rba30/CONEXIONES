using System;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Regla de "la barra llega al nudo" (validación 8.10, <c>MEMBER_NOT_AT_NODE</c>), en Core para poder probarla con
    /// las coordenadas reales del modelo de prueba. Una barra llega al nudo si su eje (recta) pasa a
    /// <paramref name="axisToleranceMm"/> o menos del punto de trabajo y, además, o bien el punto de trabajo cae dentro del
    /// tramo de la barra (cordón continuo que atraviesa el nudo), o bien el extremo más cercano queda a
    /// <paramref name="endToleranceMm"/> o menos (diagonales y montantes con retiro).
    /// </summary>
    public static class NodeReach
    {
        public const double DefaultAxisToleranceMm = 5.0;
        public const double DefaultEndToleranceMm = 500.0;

        public static bool MemberReachesNode(Vec3 startMm, Vec3 endMm, Vec3 workPointMm,
            double axisToleranceMm = DefaultAxisToleranceMm, double endToleranceMm = DefaultEndToleranceMm)
        {
            Vec3 v = endMm - startMm;
            double length = v.Length;
            if (length < 1e-9) return startMm.DistanceTo(workPointMm) <= axisToleranceMm;

            Vec3 u = v * (1.0 / length);
            Vec3 w = workPointMm - startMm;
            double projection = w.Dot(u);
            Vec3 closestOnLine = startMm + u * projection;
            double axisDistance = workPointMm.DistanceTo(closestOnLine);
            if (axisDistance > axisToleranceMm) return false;

            // Proyección dentro del tramo (cordón que atraviesa el nudo) o a menos de endToleranceMm de un extremo.
            return projection >= -endToleranceMm && projection <= length + endToleranceMm;
        }

        /// <summary>Distancia del punto de trabajo a la recta del eje (no al segmento).</summary>
        public static double AxisDistanceMm(Vec3 startMm, Vec3 endMm, Vec3 workPointMm)
        {
            Vec3 v = endMm - startMm;
            double length = v.Length;
            if (length < 1e-9) return startMm.DistanceTo(workPointMm);
            Vec3 u = v * (1.0 / length);
            Vec3 w = workPointMm - startMm;
            return workPointMm.DistanceTo(startMm + u * w.Dot(u));
        }
    }
}
