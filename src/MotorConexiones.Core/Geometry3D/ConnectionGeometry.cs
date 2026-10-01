using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Algoritmos geométricos para ubicar cartelas, placas cuchilla, pernos y soldaduras
    /// en el sistema de coordenadas local del nudo (plano XY, en mm).
    /// Sin dependencias de Revit, probado en la suite de pruebas unitarias.
    /// </summary>
    public static class ConnectionGeometry
    {
        /// <summary>
        /// Convierte la lista de pares de coordenadas [x, y] del contorno de la cartela en <see cref="BoltPosition"/>.
        /// </summary>
        public static List<BoltPosition> GetGussetOutline(GussetSpec gusset)
        {
            var outline = new List<BoltPosition>();
            if (gusset?.Outline?.PointsMm == null) return outline;

            foreach (var pt in gusset.Outline.PointsMm)
            {
                if (pt != null && pt.Length >= 2)
                {
                    outline.Add(new BoltPosition(pt[0], pt[1]));
                }
            }
            return outline;
        }

        /// <summary>
        /// Calcula la dirección unitaria 2D del miembro proyectada en el plano local XY del nudo,
        /// orientada desde el punto de trabajo hacia el extremo exterior de la barra.
        /// </summary>
        public static (double Ux, double Uy) GetMemberDirection2D(NodeFrame frame, Vec3 curveStartMm, Vec3 curveEndMm, Vec3 workPointMm)
        {
            double dStart = curveStartMm.DistanceTo(workPointMm);
            double dEnd = curveEndMm.DistanceTo(workPointMm);

            Vec3 memberDir = (curveEndMm - curveStartMm).Normalized();
            Vec3 outwardDir = dStart <= dEnd ? memberDir : memberDir * -1.0;

            // Proyectar en los ejes X e Y del marco del nudo
            double ux = outwardDir.Dot(frame.X);
            double uy = outwardDir.Dot(frame.Y);
            double len = Math.Sqrt(ux * ux + uy * uy);
            if (len < 1e-6)
            {
                return (1.0, 0.0);
            }
            return (ux / len, uy / len);
        }

        /// <summary>
        /// Calcula los 4 vértices de la placa cuchilla en el plano local XY del nudo (mm).
        /// La placa se extiende desde el extremo libre hacia adentro de la ranura del miembro HSS.
        /// </summary>
        public static List<BoltPosition> ComputeKnifePlateCorners(
            double ux, double uy,
            double endSetbackMm,
            KnifePlateSpec plate)
        {
            if (plate == null) return new List<BoltPosition>();

            double lengthMm = plate.LengthMm.GetValueOrDefault(170.0);
            double widthMm = plate.WidthMm.GetValueOrDefault(140.0);
            double insertionMm = plate.InsertionMm.GetValueOrDefault(80.0);

            // Vector perpendicular en el plano: (-uy, ux)
            double vx = -uy;
            double vy = ux;

            // Posición a lo largo del miembro
            double distStart = endSetbackMm - (lengthMm - insertionMm);
            double distEnd = endSetbackMm + insertionMm;

            double halfWidth = widthMm / 2.0;

            // 4 vértices en orden cíclico:
            var corners = new List<BoltPosition>(4)
            {
                new BoltPosition(distStart * ux - halfWidth * vx, distStart * uy - halfWidth * vy),
                new BoltPosition(distStart * ux + halfWidth * vx, distStart * uy + halfWidth * vy),
                new BoltPosition(distEnd * ux + halfWidth * vx, distEnd * uy + halfWidth * vy),
                new BoltPosition(distEnd * ux - halfWidth * vx, distEnd * uy - halfWidth * vy)
            };

            return corners;
        }

        /// <summary>
        /// Calcula las coordenadas (x, y) de cada perno en el plano local XY del nudo (mm).
        /// </summary>
        public static List<BoltPosition> ComputeBoltPositions(
            double ux, double uy,
            double endSetbackMm,
            KnifePlateSpec plate,
            BoltPatternSpec bolts)
        {
            var positions = new List<BoltPosition>();
            if (plate == null || bolts == null) return positions;

            double lengthMm = plate.LengthMm.GetValueOrDefault(170.0);
            double insertionMm = plate.InsertionMm.GetValueOrDefault(80.0);
            double firstRowFromEnd = bolts.FirstRowFromPlateEndMm.GetValueOrDefault(40.0);
            double spacingMm = bolts.SpacingMm.GetValueOrDefault(60.0);

            int rows = Math.Max(1, bolts.Rows.GetValueOrDefault(1));
            int cols = Math.Max(1, bolts.Columns.GetValueOrDefault(1));

            double vx = -uy;
            double vy = ux;

            double distStart = endSetbackMm - (lengthMm - insertionMm);
            double firstRowDist = distStart + firstRowFromEnd;

            for (int r = 0; r < rows; r++)
            {
                double alongDist = firstRowDist + r * spacingMm;

                for (int c = 0; c < cols; c++)
                {
                    double acrossOffset;
                    if (cols == 1)
                    {
                        acrossOffset = 0.0;
                    }
                    else
                    {
                        double totalSpan = (cols - 1) * spacingMm;
                        acrossOffset = -totalSpan / 2.0 + c * spacingMm;
                    }

                    double px = alongDist * ux + acrossOffset * vx;
                    double py = alongDist * uy + acrossOffset * vy;
                    positions.Add(new BoltPosition(px, py));
                }
            }

            return positions;
        }

        /// <summary>
        /// Patrón rectangular completo (posiciones + descripción para Advance Steel) de los pernos de una placa cuchilla.
        /// Las filas avanzan a lo largo del miembro (eje X del patrón) y las columnas en transversal (eje Y).
        /// </summary>
        public static BoltGrid ComputeBoltGrid(double ux, double uy, double endSetbackMm, KnifePlateSpec plate, BoltPatternSpec bolts)
        {
            var positions = ComputeBoltPositions(ux, uy, endSetbackMm, plate, bolts);
            int rows = Math.Max(1, bolts?.Rows.GetValueOrDefault(1) ?? 1);
            int cols = Math.Max(1, bolts?.Columns.GetValueOrDefault(1) ?? 1);
            double spacing = bolts?.SpacingMm.GetValueOrDefault(60.0) ?? 60.0;
            BoltPosition first = positions.Count > 0 ? positions[0] : new BoltPosition(0, 0);
            BoltPosition last = positions.Count > 0 ? positions[positions.Count - 1] : first;
            return new BoltGrid(positions, first, last, ux, uy, -uy, ux, rows, cols, spacing);
        }

        /// <summary>
        /// Calcula las líneas de soldadura representativas de las uniones de cada miembro.
        /// </summary>
        public static List<WeldLine2D> ComputeWeldLines(
            double ux, double uy,
            double endSetbackMm,
            MemberSpec member)
        {
            var welds = new List<WeldLine2D>();
            if (member?.Attachment == null) return welds;

            double vx = -uy;
            double vy = ux;

            if (string.Equals(member.Attachment.Type, "welded_slot", StringComparison.OrdinalIgnoreCase))
            {
                double slotLen = member.Attachment.SlotLengthMm.GetValueOrDefault(150.0);
                double weldSize = member.Attachment.Weld?.SizeMm ?? 5.0;
                double halfHss = 32.0; // semiancho aproximado para HSS de 64 mm

                // Dos líneas de soldadura a los costados de la ranura
                double x1 = endSetbackMm;
                double x2 = endSetbackMm + slotLen;

                welds.Add(new WeldLine2D(
                    new BoltPosition(x1 * ux + halfHss * vx, x1 * uy + halfHss * vy),
                    new BoltPosition(x2 * ux + halfHss * vx, x2 * uy + halfHss * vy),
                    weldSize));

                welds.Add(new WeldLine2D(
                    new BoltPosition(x1 * ux - halfHss * vx, x1 * uy - halfHss * vy),
                    new BoltPosition(x2 * ux - halfHss * vx, x2 * uy - halfHss * vy),
                    weldSize));
            }
            else if (string.Equals(member.Attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
            {
                if (member.Attachment.Plate != null && member.Attachment.WeldPlateToMember != null)
                {
                    double weldSize = member.Attachment.WeldPlateToMember.SizeMm.GetValueOrDefault(5.0);
                    double insertionMm = member.Attachment.Plate.InsertionMm.GetValueOrDefault(80.0);
                    double widthMm = member.Attachment.Plate.WidthMm.GetValueOrDefault(140.0);
                    double xSlotStart = endSetbackMm;
                    double xSlotEnd = endSetbackMm + insertionMm;
                    double halfW = widthMm / 2.0;

                    welds.Add(new WeldLine2D(
                        new BoltPosition(xSlotStart * ux + halfW * vx, xSlotStart * uy + halfW * vy),
                        new BoltPosition(xSlotEnd * ux + halfW * vx, xSlotEnd * uy + halfW * vy),
                        weldSize));

                    welds.Add(new WeldLine2D(
                        new BoltPosition(xSlotStart * ux - halfW * vx, xSlotStart * uy - halfW * vy),
                        new BoltPosition(xSlotEnd * ux - halfW * vx, xSlotEnd * uy - halfW * vy),
                        weldSize));
                }
            }

            return welds;
        }
    }
}
