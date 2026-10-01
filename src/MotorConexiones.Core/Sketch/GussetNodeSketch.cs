using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Croquis 2D del nudo de cercha con cartela (<c>gusset_node</c>) en el plano de la cercha y en el sistema local del
    /// nudo (origen en el punto de trabajo, X a lo largo del cordón, Y perpendicular en el plano). Dibuja eje y ancho del
    /// cordón y de cada barra, contorno de la cartela, ranuras, placas cuchilla, pernos, retiros y marcas de soldadura, y
    /// acota ancho y alto de la cartela, retiro de cada barra, largo de ranura, placa cuchilla y pernos (paso, borde y
    /// primera fila). Usa la misma geometría que la creación (<see cref="ConnectionGeometry"/>): lo que se ve es lo que se crea.
    /// </summary>
    public static class GussetNodeSketch
    {
        /// <summary>Espesor supuesto de la cartela cuando la especificación no lo trae (3/8").</summary>
        public const double DefaultGussetThicknessMm = 9.525;

        /// <summary>Separación entre el borde de una pieza y su línea de cota.</summary>
        public const double DimensionGapMm = 25.0;

        /// <summary>Cuánto sobresale el cordón por cada lado de la cartela.</summary>
        public const double ChordMarginMm = 150.0;

        /// <summary>Largo mínimo dibujado de cada barra desde el punto de trabajo.</summary>
        public const double MinMemberLengthMm = 480.0;

        private static readonly SketchPoint Origin = new SketchPoint(0, 0);

        public static Sketch Build(ConnectionSpec spec, SketchNodeInfo nodeInfo)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (nodeInfo == null) throw new ArgumentNullException(nameof(nodeInfo));

            var sketch = new Sketch();
            double chordHalf = nodeInfo.ChordWidthMm / 2.0;
            double gussetThickness = spec.Gusset?.ThicknessMm ?? DefaultGussetThicknessMm;

            // 1. Cartela: contorno y caja envolvente.
            SketchBounds gussetBounds = DrawGusset(sketch, spec.Gusset, chordHalf);

            // 2. Cordón: eje, bordes y perfil.
            double chordX0 = gussetBounds.MinX - ChordMarginMm;
            double chordX1 = gussetBounds.MaxX + ChordMarginMm;
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, 0), new SketchPoint(chordX1, 0), SketchKind.Axis));
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, chordHalf), new SketchPoint(chordX1, chordHalf), SketchKind.ChordEdge));
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, -chordHalf), new SketchPoint(chordX1, -chordHalf), SketchKind.ChordEdge));
            string chordText = "cordón · " + (spec.Chord?.Profile ?? nodeInfo.ChordTypeName ?? "perfil sin definir");
            sketch.Labels.Add(new SketchLabel(new SketchPoint(chordX1, chordHalf + 12), chordText, SketchKind.Label, 2, "chord.profile"));

            // 3. Punto de trabajo.
            sketch.Lines.Add(new SketchLine(new SketchPoint(-20, 0), new SketchPoint(20, 0), SketchKind.WorkPoint));
            sketch.Lines.Add(new SketchLine(new SketchPoint(0, -20), new SketchPoint(0, 20), SketchKind.WorkPoint));

            // 4. Unión de la cartela al cordón y su soldadura.
            if (spec.Gusset != null)
            {
                string interfaceText = "unión al cordón: " + (spec.Gusset.ChordInterface ?? "sin definir");
                if (spec.Gusset.WeldToChord != null)
                {
                    interfaceText += " · " + SketchText.Weld(spec.Gusset.WeldToChord.SizeMm, spec.Gusset.WeldToChord.AllAround);
                }
                sketch.Labels.Add(new SketchLabel(new SketchPoint(chordX0, -chordHalf - 14), interfaceText, SketchKind.Label, 0, "gusset.chord_interface"));
            }

            // 5. Barras.
            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    DrawMember(sketch, spec, i, nodeInfo, gussetThickness);
                }
            }

            // 6. Leyenda del sistema local.
            string legend = "Sistema local del nudo: X = eje del cordón, Y en el plano de la cercha";
            if (nodeInfo.LocalYInGlobal.HasValue)
            {
                Vec3 y = nodeInfo.LocalYInGlobal.Value;
                legend += string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    " · +Y local = global ({0:0.##}, {1:0.##}, {2:0.##})", y.X, y.Y, y.Z);
            }
            sketch.Labels.Add(new SketchLabel(new SketchPoint(chordX0, gussetBounds.MinY - 4.4 * DimensionGapMm), legend, SketchKind.Label, 0));

            if (nodeInfo.Note != null) sketch.Notes.Add(nodeInfo.Note);
            return sketch;
        }

        private static SketchBounds DrawGusset(Sketch sketch, GussetSpec? gusset, double chordHalf)
        {
            var points = new List<SketchPoint>();
            if (gusset?.Outline?.PointsMm != null)
            {
                foreach (double[] point in gusset.Outline.PointsMm)
                {
                    if (point != null && point.Length >= 2) points.Add(new SketchPoint(point[0], point[1]));
                }
            }

            if (points.Count < 3)
            {
                double w = gusset?.WidthMm ?? 300.0;
                double h = gusset?.HeightMm ?? 300.0;
                points = new List<SketchPoint>
                {
                    new SketchPoint(-w / 2, -h / 2), new SketchPoint(w / 2, -h / 2), new SketchPoint(w / 2, h / 2), new SketchPoint(-w / 2, h / 2),
                };
                sketch.Notes.Add("La cartela no tiene contorno (outline.points_mm): se dibuja un rectángulo de ancho × alto centrado en el punto de trabajo.");
            }

            sketch.Polygons.Add(new SketchPolygon(points, SketchKind.Gusset));

            SketchBounds bounds = SketchBounds.Empty;
            double sumX = 0, sumY = 0;
            foreach (var p in points)
            {
                bounds = bounds.Include(p);
                sumX += p.X;
                sumY += p.Y;
            }

            // Cotas de ancho y alto: miden la caja envolvente del contorno (lo mismo que mide Advance Steel) y avisan si
            // width_mm / height_mm de la especificación dicen otra cosa.
            double width = bounds.Width;
            double height = bounds.Height;
            sketch.Dimensions.Add(new SketchDimension(
                new SketchPoint(bounds.MinX, bounds.MinY), new SketchPoint(bounds.MaxX, bounds.MinY),
                -2.4 * DimensionGapMm, width, WithSpecValue(width, gusset?.WidthMm, "width_mm"), DimensionKind.GussetWidth, "gusset.width_mm"));
            sketch.Dimensions.Add(new SketchDimension(
                new SketchPoint(bounds.MaxX, bounds.MinY), new SketchPoint(bounds.MaxX, bounds.MaxY),
                -2.4 * DimensionGapMm, height, WithSpecValue(height, gusset?.HeightMm, "height_mm"), DimensionKind.GussetHeight, "gusset.height_mm"));

            // Etiqueta de espesor dentro de la cartela, por encima del cordón si hay sitio.
            double labelX = sumX / points.Count;
            double labelY = bounds.MaxY > chordHalf + 60 ? (bounds.MaxY + chordHalf) / 2.0 : sumY / points.Count;
            sketch.Labels.Add(new SketchLabel(new SketchPoint(labelX, labelY),
                "cartela " + SketchText.Thickness(gusset?.ThicknessLabel, gusset?.ThicknessMm), SketchKind.Label, 1, "gusset.thickness_mm"));

            return bounds;
        }

        private static string WithSpecValue(double measured, double? declared, string field)
        {
            string text = SketchText.Mm(measured);
            if (declared.HasValue && Math.Abs(declared.Value - measured) > 0.5)
            {
                text += " (" + field + " = " + SketchText.Mm(declared.Value) + ")";
            }
            return text;
        }

        private static void DrawMember(Sketch sketch, ConnectionSpec spec, int index, SketchNodeInfo nodeInfo, double gussetThickness)
        {
            MemberSpec member = spec.Members[index];
            SketchMemberInfo? info = nodeInfo.Find(member.ElementId);
            if (info == null && index < nodeInfo.Members.Count) info = nodeInfo.Members[index];
            if (info == null)
            {
                info = SketchNodeInfo.FromSpecAngles(spec).Members[index];
                sketch.Notes.Add("La barra " + member.ElementId + " no está en los datos del nudo: dirección aproximada.");
            }

            string path = "members[" + index + "]";
            var u = new SketchPoint(info.Ux, info.Uy);
            var v = new SketchPoint(-info.Uy, info.Ux);
            double half = info.WidthMm / 2.0;
            double setback = member.EndSetbackMm ?? 0.0;
            AttachmentSpec? attachment = member.Attachment;
            bool knife = string.Equals(attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase);
            KnifePlateSpec? plate = knife ? attachment?.Plate : null;
            double plateHalf = plate != null ? (plate.WidthMm ?? 140.0) / 2.0 : 0.0;
            double sideHalf = Math.Max(half, plateHalf);

            double reach = knife
                ? setback + (plate?.InsertionMm ?? 80.0) + 220.0
                : setback + (attachment?.SlotLengthMm ?? 150.0) + 220.0;
            double length = Math.Max(reach, MinMemberLengthMm);

            // Eje y cuerpo (abierto por el extremo lejano; la cara cercana es el extremo real tras el retiro).
            sketch.Lines.Add(new SketchLine(Origin, u * length, SketchKind.Axis));
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint>
            {
                u * length + v * half, u * setback + v * half, u * setback - v * half, u * length - v * half,
            }, SketchKind.MemberOutline, isClosed: false));

            // Retiro: del punto de trabajo al extremo real de la barra.
            if (setback > 0)
            {
                sketch.Dimensions.Add(new SketchDimension(Origin, u * setback, sideHalf + DimensionGapMm, setback,
                    SketchText.Mm(setback), DimensionKind.MemberSetback, path + ".end_setback_mm"));
            }

            // Etiqueta de la barra en el extremo lejano.
            string role = member.Role ?? "barra";
            string profile = member.Profile ?? info.TypeName ?? "perfil sin definir";
            string memberText = role + " " + member.ElementId + " · " + profile + " · " + SketchText.Degrees(info.AngleInPlaneDeg);
            if (info.IsApproximate) memberText += " (dirección aproximada)";
            sketch.Labels.Add(new SketchLabel(u * (length + 15), memberText, SketchKind.Label, info.Ux >= 0 ? 0 : 2, path + ".profile"));

            if (!knife)
            {
                DrawWeldedSlot(sketch, member, index, u, v, half, sideHalf, setback, gussetThickness);
            }
            else if (plate != null)
            {
                DrawKnifePlate(sketch, member, index, info, u, v, sideHalf, plateHalf, setback, plate);
            }
            else
            {
                sketch.Notes.Add("La barra " + member.ElementId + " es bolted_knife_plate sin 'plate': no se dibuja la placa.");
            }
        }

        private static void DrawWeldedSlot(Sketch sketch, MemberSpec member, int index, SketchPoint u, SketchPoint v,
            double half, double sideHalf, double setback, double gussetThickness)
        {
            string path = "members[" + index + "].attachment";
            double slotLength = member.Attachment?.SlotLengthMm ?? 150.0;
            double t = gussetThickness / 2.0;
            double slotEnd = setback + slotLength;

            // Ranura del HSS por donde pasa la cartela (ancho = espesor de la cartela).
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint>
            {
                u * setback + v * t, u * slotEnd + v * t, u * slotEnd - v * t, u * setback - v * t,
            }, SketchKind.Slot));

            // Soldaduras a lo largo de la ranura, en los bordes de la barra (misma posición que crea el add-in).
            sketch.Lines.Add(new SketchLine(u * setback + v * half, u * slotEnd + v * half, SketchKind.Weld));
            sketch.Lines.Add(new SketchLine(u * setback - v * half, u * slotEnd - v * half, SketchKind.Weld));

            sketch.Dimensions.Add(new SketchDimension(u * setback, u * slotEnd, -(sideHalf + DimensionGapMm), slotLength,
                SketchText.Mm(slotLength), DimensionKind.SlotLength, path + ".slot_length_mm"));

            WeldSpec? weld = member.Attachment?.Weld;
            string weldText = weld != null ? SketchText.Weld(weld.SizeMm, weld.AllAround) : "soldadura sin definir";
            sketch.Labels.Add(new SketchLabel(u * (setback + slotLength / 2.0) + v * (half + DimensionGapMm), weldText, SketchKind.Label, 1, path + ".weld.size_mm"));
        }

        private static void DrawKnifePlate(Sketch sketch, MemberSpec member, int index, SketchMemberInfo info, SketchPoint u, SketchPoint v,
            double sideHalf, double plateHalf, double setback, KnifePlateSpec plate)
        {
            string path = "members[" + index + "].attachment";
            double plateLength = plate.LengthMm ?? 170.0;
            double insertion = plate.InsertionMm ?? 80.0;
            double distStart = setback - (plateLength - insertion);
            double distEnd = setback + insertion;

            // Placa cuchilla: los mismos vértices que se crean en el modelo.
            var corners = ConnectionGeometry.ComputeKnifePlateCorners(info.Ux, info.Uy, setback, plate);
            var platePoints = new List<SketchPoint>(corners.Count);
            foreach (BoltPosition corner in corners) platePoints.Add(new SketchPoint(corner.X, corner.Y));
            sketch.Polygons.Add(new SketchPolygon(platePoints, SketchKind.KnifePlate));
            sketch.Labels.Add(new SketchLabel(u * (distStart + plateLength / 2.0) - v * (plateHalf + 0.5 * DimensionGapMm),
                "placa cuchilla " + SketchText.Thickness(plate.ThicknessLabel, plate.ThicknessMm), SketchKind.Label, 1, path + ".plate.thickness_mm"));

            // Soldadura placa-barra a lo largo de la inserción.
            sketch.Lines.Add(new SketchLine(u * setback + v * plateHalf, u * distEnd + v * plateHalf, SketchKind.Weld));
            sketch.Lines.Add(new SketchLine(u * setback - v * plateHalf, u * distEnd - v * plateHalf, SketchKind.Weld));
            WeldSpec? weld = member.Attachment?.WeldPlateToMember;
            string weldText = weld != null ? SketchText.Weld(weld.SizeMm, weld.AllAround) : "soldadura placa-barra sin definir";
            sketch.Labels.Add(new SketchLabel(u * (setback + insertion / 2.0) + v * (plateHalf + 0.6 * DimensionGapMm), weldText, SketchKind.Label, 1, path + ".weld_plate_to_member.size_mm"));

            // Cotas de la placa: largo (lado −v, pegado a la placa) y ancho (en el extremo libre, hacia el punto de trabajo).
            sketch.Dimensions.Add(new SketchDimension(u * distStart, u * distEnd, -(sideHalf + DimensionGapMm), plateLength,
                SketchText.Mm(plateLength), DimensionKind.PlateLength, path + ".plate.length_mm"));
            double widthLineOffset = 1.8 * DimensionGapMm; // normal izquierda de v = −u: positivo = hacia el punto de trabajo
            sketch.Dimensions.Add(new SketchDimension(u * distStart - v * plateHalf, u * distStart + v * plateHalf, widthLineOffset, 2 * plateHalf,
                SketchText.Mm(2 * plateHalf), DimensionKind.PlateWidth, path + ".plate.width_mm"));

            // Pernos.
            BoltPatternSpec? bolts = member.Attachment?.Bolts;
            if (bolts == null) return;

            var positions = ConnectionGeometry.ComputeBoltPositions(info.Ux, info.Uy, setback, plate, bolts);
            double radius = (bolts.DiameterMm ?? 15.875) / 2.0;
            foreach (BoltPosition position in positions)
            {
                sketch.Circles.Add(new SketchCircle(new SketchPoint(position.X, position.Y), radius, SketchKind.Bolt));
            }

            int rows = Math.Max(1, bolts.Rows ?? 1);
            int cols = Math.Max(1, bolts.Columns ?? 1);
            double spacing = bolts.SpacingMm ?? 60.0;
            double firstRow = bolts.FirstRowFromPlateEndMm ?? 40.0;
            double firstRowDist = distStart + firstRow;
            double acrossLast = cols == 1 ? 0.0 : (cols - 1) * spacing / 2.0;

            // Primera fila desde el extremo libre (lado −v, por fuera de la cota de largo).
            sketch.Dimensions.Add(new SketchDimension(u * distStart, u * firstRowDist, -(sideHalf + 2.4 * DimensionGapMm), firstRow,
                SketchText.Mm(firstRow), DimensionKind.BoltFirstRow, path + ".bolts.first_row_from_plate_end_mm"));

            // Paso: entre las dos primeras filas (a lo largo) o, con una sola fila, entre las dos primeras columnas.
            if (positions.Count >= 2)
            {
                if (rows >= 2)
                {
                    var p0 = positions[0];
                    var p1 = positions[cols];
                    double across0 = -acrossLast; // columna 0
                    double target = sideHalf + 2.4 * DimensionGapMm; // lado +v, por fuera de la cota de retiro
                    sketch.Dimensions.Add(new SketchDimension(new SketchPoint(p0.X, p0.Y), new SketchPoint(p1.X, p1.Y), target - across0, spacing,
                        SketchText.Mm(spacing), DimensionKind.BoltSpacing, path + ".bolts.spacing_mm"));
                }
                else
                {
                    var p0 = positions[0];
                    var p1 = positions[1];
                    double along = firstRowDist;
                    double lineAlong = distStart - 3.4 * DimensionGapMm; // más allá de la cota de ancho
                    sketch.Dimensions.Add(new SketchDimension(new SketchPoint(p0.X, p0.Y), new SketchPoint(p1.X, p1.Y), along - lineAlong, spacing,
                        SketchText.Mm(spacing), DimensionKind.BoltSpacing, path + ".bolts.spacing_mm"));
                }
            }

            // Distancia al borde: del perno de la última columna al borde +v de la placa, en la primera fila.
            if (bolts.EdgeMm.HasValue || cols >= 1)
            {
                var last = positions[cols - 1];
                var boltPoint = new SketchPoint(last.X, last.Y);
                var edgePoint = u * firstRowDist + v * plateHalf;
                double measured = boltPoint.DistanceTo(edgePoint);
                double declared = bolts.EdgeMm ?? measured;
                string text = SketchText.Mm(declared);
                if (Math.Abs(declared - measured) > 0.5) text += " (medido " + SketchText.Mm(measured) + ")";
                double lineAlong = distStart - 3.4 * DimensionGapMm;
                sketch.Dimensions.Add(new SketchDimension(boltPoint, edgePoint, firstRowDist - lineAlong, declared,
                    text, DimensionKind.BoltEdge, path + ".bolts.edge_mm"));
            }
        }
    }
}
