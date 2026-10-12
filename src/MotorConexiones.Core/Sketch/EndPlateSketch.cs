using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Fase 12: croquis de la placa de extremo (<c>end_plate</c>, empalme). A la izquierda, el alzado de la plancha mirando
    /// al eje de la barra: el octógono con las esquinas recortadas, la huella del tubo y los pernos por cara, con las cotas
    /// de ancho, alto, esquina, paso y borde (cada cota con su ruta JSON para el doble clic). A la derecha, el perfil del
    /// empalme: los dos tramos, las dos planchas, la holgura y el texto del paquete de pernos (agarre y longitud).
    /// </summary>
    public static class EndPlateSketch
    {
        public const double DimensionGapMm = 25.0;
        /// <summary>Separación entre el alzado de la plancha y el perfil del empalme.</summary>
        public const double ProfileGapMm = 160.0;
        public const double StubLengthMm = 260.0;

        public static Sketch Build(ConnectionSpec spec, SketchNodeInfo nodeInfo, LimitsConfig? limits = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (nodeInfo == null) throw new ArgumentNullException(nameof(nodeInfo));
            limits ??= LimitsConfig.Default;
            var sketch = new Sketch();
            EndPlateSpec? plate = spec.Plate;
            RingBoltSpec? bolts = spec.Bolts;
            double w = plate?.WidthMm ?? 200.0;
            double h = plate?.HeightMm ?? 200.0;
            double corner = Math.Max(0.0, plate?.CornerCutMm ?? 0.0);
            double t = plate?.ThicknessMm ?? 9.525;
            double gap = Math.Max(0.0, spec.GapMm ?? 0.0);
            // Huella del tubo: canto mayor a lo ancho, menor a lo alto (lo mismo que el validador).
            double tubeW = Math.Max(nodeInfo.ChordWidthMm, nodeInfo.ChordDepthMm);
            double tubeH = Math.Min(nodeInfo.ChordWidthMm, nodeInfo.ChordDepthMm);
            // 1. Alzado de la plancha.
            var points = new List<SketchPoint>();
            foreach (BoltPosition p in Octagon.Points(w, h, corner)) points.Add(new SketchPoint(p.X, p.Y));
            sketch.Polygons.Add(new SketchPolygon(points, SketchKind.Gusset));
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint>
            {
                new SketchPoint(-tubeW / 2, -tubeH / 2), new SketchPoint(tubeW / 2, -tubeH / 2), new SketchPoint(tubeW / 2, tubeH / 2), new SketchPoint(-tubeW / 2, tubeH / 2),
            }, SketchKind.ChordEdge));
            sketch.Lines.Add(new SketchLine(new SketchPoint(-20, 0), new SketchPoint(20, 0), SketchKind.WorkPoint));
            sketch.Lines.Add(new SketchLine(new SketchPoint(0, -20), new SketchPoint(0, 20), SketchKind.WorkPoint));
            sketch.Labels.Add(new SketchLabel(new SketchPoint(0, tubeH / 2 + 10), "plancha " + SketchText.Thickness(plate?.ThicknessLabel, plate?.ThicknessMm) + " · " + SketchText.Mm(w) + " × " + SketchText.Mm(h), SketchKind.Label, 1, "plate.thickness_mm"));
            sketch.Labels.Add(new SketchLabel(new SketchPoint(0, -tubeH / 2 - 14), "tubo " + (spec.Chord?.Profile ?? nodeInfo.ChordTypeName ?? "perfil sin definir"), SketchKind.Label, 1, "chord.profile"));
            // Cotas: ancho (abajo), alto (derecha), esquina (arriba a la izquierda).
            sketch.Dimensions.Add(new SketchDimension(new SketchPoint(-w / 2, -h / 2), new SketchPoint(w / 2, -h / 2), -2.4 * DimensionGapMm, w, SketchText.Mm(w), DimensionKind.GussetWidth, "plate.width_mm"));
            sketch.Dimensions.Add(new SketchDimension(new SketchPoint(w / 2, -h / 2), new SketchPoint(w / 2, h / 2), -2.4 * DimensionGapMm, h, SketchText.Mm(h), DimensionKind.GussetHeight, "plate.height_mm"));
            if (corner > 0.05)
            {
                sketch.Dimensions.Add(new SketchDimension(new SketchPoint(-w / 2, h / 2), new SketchPoint(-w / 2 + corner, h / 2), -1.2 * DimensionGapMm, corner, SketchText.Mm(corner) + " esquina", DimensionKind.PlateWidth, "plate.corner_cut_mm"));
            }
            // 2. Pernos por cara.
            double radius = (bolts?.DiameterMm ?? 15.875) / 2.0;
            double holeRadius = bolts?.HoleMm is double hole && hole > 0 ? hole / 2.0 : 0.0;
            List<BoltRow> rows = BoltRing.Rows(plate, bolts);
            int count = 0;
            foreach (BoltRow row in rows)
            {
                string path = "bolts.faces." + row.Face;
                foreach (BoltPosition p in row.Positions)
                {
                    sketch.Circles.Add(new SketchCircle(new SketchPoint(p.X, p.Y), radius, SketchKind.Bolt));
                    if (holeRadius > radius) sketch.Circles.Add(new SketchCircle(new SketchPoint(p.X, p.Y), holeRadius, SketchKind.Slot));
                    count++;
                }
                if (row.Positions.Count == 0) continue;
                BoltPosition first = row.Positions[0];
                // Borde: del eje de la fila al borde de la plancha, en la primera posición.
                SketchPoint edgeFrom = new SketchPoint(first.X, first.Y);
                SketchPoint edgeTo = row.Face == "top" ? new SketchPoint(first.X, h / 2)
                    : row.Face == "bottom" ? new SketchPoint(first.X, -h / 2)
                    : row.Face == "left" ? new SketchPoint(-w / 2, first.Y)
                    : new SketchPoint(w / 2, first.Y);
                double edgeOffset = row.IsHorizontal ? (row.Face == "top" ? 1.0 : -1.0) * (Math.Abs(first.X) + 1.2 * DimensionGapMm) * 0 + (row.Face == "top" ? -1 : 1) * (w / 2 - Math.Abs(first.X) + 1.2 * DimensionGapMm)
                                                     : (row.Face == "right" ? -1 : 1) * (h / 2 - Math.Abs(first.Y) + 1.2 * DimensionGapMm);
                sketch.Dimensions.Add(new SketchDimension(edgeFrom, edgeTo, edgeOffset, row.EdgeMm, SketchText.Mm(row.EdgeMm), DimensionKind.BoltEdge, path + ".edge_mm"));
                if (row.Positions.Count >= 2)
                {
                    BoltPosition second = row.Positions[1];
                    double spacingOffset = row.IsHorizontal ? (row.Face == "top" ? 1.0 : -1.0) * (h / 2 - Math.Abs(first.Y) + 1.2 * DimensionGapMm)
                                                            : (row.Face == "right" ? 1.0 : -1.0) * (w / 2 - Math.Abs(first.X) + 1.2 * DimensionGapMm);
                    sketch.Dimensions.Add(new SketchDimension(new SketchPoint(first.X, first.Y), new SketchPoint(second.X, second.Y), spacingOffset, row.SpacingMm, SketchText.Mm(row.SpacingMm), DimensionKind.BoltSpacing, path + ".spacing_mm"));
                }
            }
            BoltStack stack = BoltStack.ForTwoPlates(t, t, gap, bolts, limits);
            string diameter = !string.IsNullOrWhiteSpace(bolts?.DiameterLabel) ? "Ø" + bolts!.DiameterLabel!.Trim() : "Ø" + SketchText.Mm(bolts?.DiameterMm ?? 15.875) + " mm";
            string holeText = bolts?.HoleLabel != null ? " en Ø" + bolts.HoleLabel : bolts?.HoleMm is double hm ? " en Ø" + SketchText.Mm(hm) : "";
            string nuts = string.Equals(bolts?.Nuts, "one_side", StringComparison.OrdinalIgnoreCase) ? "tuerca a un lado" : "tuerca a ambos lados";
            sketch.Labels.Add(new SketchLabel(new SketchPoint(0, -h / 2 - 3.6 * DimensionGapMm),
                count + " pernos " + diameter + holeText + " · " + nuts + " · agarre " + SketchText.Mm(stack.GripMm) + " mm (2 × " + SketchText.Mm(t) + (gap > 0 ? " + holgura " + SketchText.Mm(gap) : "") + ") · L " + SketchText.Mm(stack.BoltLengthMm) + " mm" + (stack.LengthFromSpec ? " (del plano)" : ""),
                SketchKind.Label, 1, "bolts.length_mm"));
            // 3. Perfil del empalme a la derecha: eje horizontal, dos tramos, dos planchas y la holgura.
            double x0 = w / 2 + ProfileGapMm;
            double halfTube = tubeH / 2.0;
            double splice = x0 + StubLengthMm + t + gap / 2.0;
            double plateHalfH = h / 2.0;
            sketch.Lines.Add(new SketchLine(new SketchPoint(x0 - 40, 0), new SketchPoint(splice * 2 - x0 + 40, 0), SketchKind.Axis));
            // Tramo 1 (chord): termina en la cara interior de su plancha.
            double stubEnd1 = splice - gap / 2.0 - t;
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint> { new SketchPoint(x0, halfTube), new SketchPoint(stubEnd1, halfTube), new SketchPoint(stubEnd1, -halfTube), new SketchPoint(x0, -halfTube) }, SketchKind.MemberOutline, isClosed: false));
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint> { new SketchPoint(stubEnd1, -plateHalfH), new SketchPoint(stubEnd1 + t, -plateHalfH), new SketchPoint(stubEnd1 + t, plateHalfH), new SketchPoint(stubEnd1, plateHalfH) }, SketchKind.KnifePlate));
            // Tramo 2 (members[0]).
            double stubStart2 = splice + gap / 2.0 + t;
            double x1 = stubStart2 + StubLengthMm;
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint> { new SketchPoint(x1, halfTube), new SketchPoint(stubStart2, halfTube), new SketchPoint(stubStart2, -halfTube), new SketchPoint(x1, -halfTube) }, SketchKind.MemberOutline, isClosed: false));
            sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint> { new SketchPoint(stubStart2 - t, -plateHalfH), new SketchPoint(stubStart2, -plateHalfH), new SketchPoint(stubStart2, plateHalfH), new SketchPoint(stubStart2 - t, plateHalfH) }, SketchKind.KnifePlate));
            // Soldaduras plancha-tubo y pernos vistos de perfil (en las filas de arriba y abajo).
            sketch.Lines.Add(new SketchLine(new SketchPoint(stubEnd1, halfTube), new SketchPoint(stubEnd1, halfTube + 8), SketchKind.Weld));
            sketch.Lines.Add(new SketchLine(new SketchPoint(stubEnd1, -halfTube), new SketchPoint(stubEnd1, -halfTube - 8), SketchKind.Weld));
            sketch.Lines.Add(new SketchLine(new SketchPoint(stubStart2, halfTube), new SketchPoint(stubStart2, halfTube + 8), SketchKind.Weld));
            sketch.Lines.Add(new SketchLine(new SketchPoint(stubStart2, -halfTube), new SketchPoint(stubStart2, -halfTube - 8), SketchKind.Weld));
            foreach (BoltRow row in rows)
            {
                if (row.Positions.Count == 0) continue;
                double y = row.IsHorizontal ? row.Positions[0].Y : row.Positions[0].X;
                if (!row.IsHorizontal) continue;
                sketch.Lines.Add(new SketchLine(new SketchPoint(splice - stack.BoltLengthMm / 2.0, y), new SketchPoint(splice + stack.BoltLengthMm / 2.0, y), SketchKind.Bolt));
            }
            // Punto de empalme y retiros.
            sketch.Lines.Add(new SketchLine(new SketchPoint(splice, -plateHalfH - 10), new SketchPoint(splice, plateHalfH + 10), SketchKind.WorkPoint));
            double setback1 = spec.Chord?.EndSetbackMm ?? (t + gap / 2.0);
            double setback2 = spec.Members != null && spec.Members.Count > 0 ? spec.Members[0].EndSetbackMm ?? (t + gap / 2.0) : t + gap / 2.0;
            sketch.Dimensions.Add(new SketchDimension(new SketchPoint(splice - setback1, -plateHalfH), new SketchPoint(splice, -plateHalfH), -1.2 * DimensionGapMm, setback1, SketchText.Mm(setback1) + " retiro", DimensionKind.MemberSetback, "chord.end_setback_mm"));
            sketch.Dimensions.Add(new SketchDimension(new SketchPoint(splice, -plateHalfH), new SketchPoint(splice + setback2, -plateHalfH), -1.2 * DimensionGapMm, setback2, SketchText.Mm(setback2) + " retiro", DimensionKind.MemberSetback, "members[0].end_setback_mm"));
            if (gap > 0.05)
            {
                sketch.Dimensions.Add(new SketchDimension(new SketchPoint(splice - gap / 2.0, plateHalfH), new SketchPoint(splice + gap / 2.0, plateHalfH), 1.2 * DimensionGapMm, gap, SketchText.Mm(gap) + " holgura", DimensionKind.BoltEdge, "gap_mm"));
            }
            sketch.Labels.Add(new SketchLabel(new SketchPoint(x0, halfTube + 12), "tramo 1 · " + (spec.Chord?.ElementId.ToString() ?? "?"), SketchKind.Label, 0, "chord.element_id"));
            sketch.Labels.Add(new SketchLabel(new SketchPoint(x1, halfTube + 12), "tramo 2 · " + (spec.Members != null && spec.Members.Count > 0 ? spec.Members[0].ElementId.ToString() : "?"), SketchKind.Label, 2, "members[0].element_id"));
            WeldSpec? weld = plate?.Weld;
            sketch.Labels.Add(new SketchLabel(new SketchPoint(splice, plateHalfH + 2.4 * DimensionGapMm), "plancha al tubo: " + (weld != null ? SketchText.Weld(weld.SizeMm, weld.AllAround) : "soldadura sin definir"), SketchKind.Label, 1, "plate.weld.size_mm"));
            sketch.Labels.Add(new SketchLabel(new SketchPoint(-w / 2, -h / 2 - 5.4 * DimensionGapMm),
                "Placa de extremo (empalme): a la izquierda, la plancha vista en el eje de la barra (X a lo ancho, Y a lo alto); a la derecha, el perfil del empalme", SketchKind.Label, 0));
            if (nodeInfo.Note != null) sketch.Notes.Add(nodeInfo.Note);
            return sketch;
        }
    }
}
