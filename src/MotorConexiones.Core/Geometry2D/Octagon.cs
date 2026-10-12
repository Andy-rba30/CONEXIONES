using System;
using System.Collections.Generic;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>
    /// Fase 12: el contorno de una plancha rectangular con las esquinas recortadas a 45° (los Detalles B, C e I), centrada en
    /// (0, 0) del plano de la plancha: ancho a lo largo de X, alto a lo largo de Y. Con cateto 0 es un rectángulo de cuatro
    /// vértices; con cateto mayor que la mitad del lado menor se recorta hasta la mitad (la plancha no se "cruza").
    /// </summary>
    public static class Octagon
    {
        /// <summary>Vértices en sentido antihorario empezando por el borde inferior (izquierda a derecha).</summary>
        public static List<BoltPosition> Points(double widthMm, double heightMm, double cornerCutMm)
        {
            double w = Math.Max(widthMm, 1.0) / 2.0;
            double h = Math.Max(heightMm, 1.0) / 2.0;
            double c = Math.Max(0.0, Math.Min(cornerCutMm, Math.Min(w, h)));
            if (c < 1e-9)
            {
                return new List<BoltPosition>
                {
                    new BoltPosition(-w, -h), new BoltPosition(w, -h), new BoltPosition(w, h), new BoltPosition(-w, h),
                };
            }
            return new List<BoltPosition>
            {
                new BoltPosition(-w + c, -h), new BoltPosition(w - c, -h),
                new BoltPosition(w, -h + c), new BoltPosition(w, h - c),
                new BoltPosition(w - c, h), new BoltPosition(-w + c, h),
                new BoltPosition(-w, h - c), new BoltPosition(-w, -h + c),
            };
        }

        /// <summary>Lo mismo como polígono 2D (para contener puntos y medir distancias al borde).</summary>
        public static Polygon2D Polygon(double widthMm, double heightMm, double cornerCutMm)
        {
            var points = new List<double[]>();
            foreach (BoltPosition p in Points(widthMm, heightMm, cornerCutMm)) points.Add(new[] { p.X, p.Y });
            return Polygon2D.FromPointArrays(points);
        }
    }
}
