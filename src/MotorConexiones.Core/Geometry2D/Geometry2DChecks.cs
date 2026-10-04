using System;
using System.Collections.Generic;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>
    /// Comprobaciones geométricas planas (2D): pernos dentro de la placa cuchilla y placa dentro de la cartela.
    /// </summary>
    public static class Geometry2DChecks
    {
        /// <summary>
        /// Comprueba si todos los pernos de un patrón caben dentro de las dimensiones de su placa cuchilla.
        /// </summary>
        public static bool CheckBoltsInsidePlate(
            double plateLengthMm,
            double plateWidthMm,
            double boltDiameterMm,
            int rows,
            int columns,
            double spacingMm,
            double edgeMm,
            double firstRowFromEndMm,
            out string detail)
        {
            detail = string.Empty;

            if (plateLengthMm <= 0 || plateWidthMm <= 0)
            {
                detail = "Dimensiones de placa no válidas.";
                return false;
            }

            if (rows <= 0 || columns <= 0)
            {
                detail = "Número de filas o columnas de pernos no válido.";
                return false;
            }

            // A lo largo de la longitud:
            // Fila 0 está en firstRowFromEndMm
            // Última fila está en firstRowFromEndMm + (rows - 1) * spacingMm
            double lastRowMm = firstRowFromEndMm + (rows - 1) * spacingMm;
            if (firstRowFromEndMm < 0 || lastRowMm > plateLengthMm)
            {
                detail = $"El grupo de pernos a lo largo de la placa (de {firstRowFromEndMm:F1} a {lastRowMm:F1} mm) excede la longitud de la placa ({plateLengthMm:F1} mm).";
                return false;
            }

            // A lo ancho de la placa:
            // Ancho requerido por columnas con su separación y borde:
            double requiredWidthMm = (columns - 1) * spacingMm + 2.0 * edgeMm;
            if (requiredWidthMm > plateWidthMm + 0.05)
            {
                detail = $"El ancho requerido por el grupo de pernos ({requiredWidthMm:F1} mm) excede el ancho de la placa ({plateWidthMm:F1} mm).";
                return false;
            }

            // Distancia del último perno al extremo interior de la placa
            double endEdgeMm = plateLengthMm - lastRowMm;
            if (endEdgeMm < 0)
            {
                detail = $"El último perno queda fuera del extremo de la placa por {-endEdgeMm:F1} mm.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Comprueba si la porción expuesta de la placa cuchilla (la que apoya en la cartela) cae dentro del contorno poligonal de la cartela.
        /// </summary>
        public static bool CheckKnifePlateInsideGusset(
            Polygon2D gussetOutline,
            double memberAngleDeg,
            double endSetbackMm,
            double plateLengthMm,
            double plateWidthMm,
            double insertionMm,
            out string detail,
            double toleranceMm = 0.0)
        {
            detail = string.Empty;
            if (gussetOutline == null || gussetOutline.Vertices.Count < 3)
            {
                detail = "El contorno de la cartela no es válido.";
                return false;
            }

            // Porción expuesta que solapa con la cartela
            double exposedLengthMm = plateLengthMm - insertionMm;
            if (exposedLengthMm <= 0)
            {
                detail = "La placa cuchilla está totalmente insertada dentro del miembro o su longitud expuesta es <= 0.";
                return false;
            }

            // Vector director a lo largo del miembro (desde el punto de trabajo hacia el miembro)
            double rad = memberAngleDeg * Math.PI / 180.0;
            double ux = Math.Cos(rad);
            double uy = Math.Sin(rad);

            // Vector perpendicular en el plano
            double vx = -uy;
            double vy = ux;

            // Extremo libre de la placa: a (endSetbackMm - exposedLengthMm) del punto de trabajo
            double tStart = endSetbackMm - exposedLengthMm;
            double tEnd = endSetbackMm;

            double halfW = plateWidthMm * 0.5;

            // 4 esquinas de la placa expuesta
            Point2D[] corners = new Point2D[]
            {
                new Point2D(tStart * ux + halfW * vx, tStart * uy + halfW * vy),
                new Point2D(tStart * ux - halfW * vx, tStart * uy - halfW * vy),
                new Point2D(tEnd * ux + halfW * vx,   tEnd * uy + halfW * vy),
                new Point2D(tEnd * ux - halfW * vx,   tEnd * uy - halfW * vy)
            };

            for (int i = 0; i < corners.Length; i++)
            {
                if (gussetOutline.ContainsPoint(corners[i])) continue;
                // Fase 7: una esquina que asoma menos de la tolerancia (limits.json) se admite: la placa cuchilla del
                // Detalle D termina justo en el chaflán de la cartela y su esquina asoma unas décimas de milímetro.
                double outside = DistanceToBoundary(gussetOutline, corners[i]);
                if (toleranceMm > 0 && outside <= toleranceMm + 1e-9) continue;
                detail = $"La esquina {i + 1} de la placa cuchilla en ({corners[i].X:F1}, {corners[i].Y:F1}) mm queda fuera del contorno de la cartela por {outside:F1} mm (barra a {memberAngleDeg:F1}°).";
                return false;
            }

            return true;
        }

        /// <summary>Distancia de un punto al borde del polígono (mínimo a sus aristas).</summary>
        public static double DistanceToBoundary(Polygon2D polygon, Point2D point)
        {
            int n = polygon.Vertices.Count;
            double best = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                Point2D a = polygon.Vertices[i];
                Point2D b = polygon.Vertices[(i + 1) % n];
                double dx = b.X - a.X, dy = b.Y - a.Y;
                double length2 = dx * dx + dy * dy;
                double t = length2 < 1e-12 ? 0.0 : Math.Max(0.0, Math.Min(1.0, ((point.X - a.X) * dx + (point.Y - a.Y) * dy) / length2));
                double px = a.X + t * dx, py = a.Y + t * dy;
                double distance = Math.Sqrt((point.X - px) * (point.X - px) + (point.Y - py) * (point.Y - py));
                if (distance < best) best = distance;
            }
            return best;
        }
    }
}
