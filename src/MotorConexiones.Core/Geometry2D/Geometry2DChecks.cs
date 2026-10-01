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
            out string detail)
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
                if (!gussetOutline.ContainsPoint(corners[i]))
                {
                    detail = $"La esquina {i + 1} de la placa cuchilla en ({corners[i].X:F1}, {corners[i].Y:F1}) mm queda fuera del contorno de la cartela.";
                    return false;
                }
            }

            return true;
        }
    }
}
