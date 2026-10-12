using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Fase 12: croquis del nudo en K soldado (<c>direct_weld</c>) en el plano de la cercha: el cordón con su canto real,
    /// cada barra desde su propio corte con el eje del cordón (opción A) cortada contra la cara del cordón, la soldadura
    /// sobre la cara, la cota de la excentricidad entre los cortes, la holgura si la hay y la marca del empalme Detalle B
    /// (<c>splice_hint</c>) a su distancia de la cara. Usa la misma geometría que la creación (<see cref="DirectWeldGeometry"/>).
    /// </summary>
    public static class DirectWeldSketch
    {
        public const double DimensionGapMm = 25.0;
        public const double ChordMarginMm = 250.0;
        public const double MinMemberLengthMm = 520.0;

        public static Sketch Build(ConnectionSpec spec, SketchNodeInfo nodeInfo, LimitsConfig? limits = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (nodeInfo == null) throw new ArgumentNullException(nameof(nodeInfo));
            var sketch = new Sketch();
            double chordHalf = nodeInfo.ChordWidthMm / 2.0;
            var ends = new List<(int Index, MemberSpec Member, SketchMemberInfo Info, DirectWeldEnd End)>();
            double minX = -200, maxX = 200;
            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    MemberSpec member = spec.Members[i];
                    SketchMemberInfo? info = nodeInfo.Find(member.ElementId) ?? (i < nodeInfo.Members.Count ? nodeInfo.Members[i] : null);
                    if (info == null)
                    {
                        info = SketchNodeInfo.FromSpecAngles(spec).Members[i];
                        sketch.Notes.Add("La barra " + member.ElementId + " no está en los datos del nudo: dirección aproximada.");
                    }
                    DirectWeldEnd end = DirectWeldGeometry.ComputeEnd(info.Ux, info.Uy, info.AxisOffsetMm, chordHalf, info.WidthMm, member.EndGapMm ?? 0.0);
                    ends.Add((i, member, info, end));
                    double reach = Math.Max(end.SetbackMm + 300.0, MinMemberLengthMm);
                    minX = Math.Min(minX, info.AxisOffsetMm + info.Ux * reach - info.WidthMm);
                    maxX = Math.Max(maxX, info.AxisOffsetMm + info.Ux * reach + info.WidthMm);
                }
            }
            // 1. Cordón: eje, bordes y perfil.
            double chordX0 = minX - ChordMarginMm;
            double chordX1 = maxX + ChordMarginMm;
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, 0), new SketchPoint(chordX1, 0), SketchKind.Axis));
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, chordHalf), new SketchPoint(chordX1, chordHalf), SketchKind.ChordEdge));
            sketch.Lines.Add(new SketchLine(new SketchPoint(chordX0, -chordHalf), new SketchPoint(chordX1, -chordHalf), SketchKind.ChordEdge));
            string chordText = "cordón · " + (spec.Chord?.Profile ?? nodeInfo.ChordTypeName ?? "perfil sin definir") + (spec.Chord != null && !spec.Chord.Continuous ? " · empalmado en el nudo" : "");
            sketch.Labels.Add(new SketchLabel(new SketchPoint(chordX1, chordHalf + 12), chordText, SketchKind.Label, 2, "chord.profile"));
            // 2. Punto de trabajo.
            sketch.Lines.Add(new SketchLine(new SketchPoint(-20, 0), new SketchPoint(20, 0), SketchKind.WorkPoint));
            sketch.Lines.Add(new SketchLine(new SketchPoint(0, -20), new SketchPoint(0, 20), SketchKind.WorkPoint));
            // 3. Barras cortadas contra la cara, soldadura y marca del empalme.
            double lowest = -chordHalf;
            double highest = chordHalf;
            foreach (var (index, member, info, end) in ends)
            {
                string path = "members[" + index + "]";
                var c = new SketchPoint(info.AxisOffsetMm, 0.0);
                var u = new SketchPoint(info.Ux, info.Uy);
                var v = new SketchPoint(-info.Uy, info.Ux);
                double half = info.WidthMm / 2.0;
                double length = Math.Max(end.SetbackMm + 300.0, MinMemberLengthMm);
                double absUy = Math.Max(Math.Abs(info.Uy), 1e-6);
                // Las dos aristas de la barra terminan en la cara (y = faceY): cada una a su distancia a lo largo de la barra.
                double tEdgePlus = (end.FaceY - (c.Y + v.Y * half)) / (u.Y == 0 ? 1e-6 : u.Y);
                double tEdgeMinus = (end.FaceY - (c.Y - v.Y * half)) / (u.Y == 0 ? 1e-6 : u.Y);
                double gap = member.EndGapMm ?? 0.0;
                double gapAlong = gap / absUy;
                SketchPoint plusEnd = c + v * half + u * (tEdgePlus + gapAlong);
                SketchPoint minusEnd = c - v * half + u * (tEdgeMinus + gapAlong);
                sketch.Lines.Add(new SketchLine(c, c + u * length, SketchKind.Axis));
                sketch.Polygons.Add(new SketchPolygon(new List<SketchPoint>
                {
                    c + u * length + v * half, plusEnd, minusEnd, c + u * length - v * half,
                }, SketchKind.MemberOutline, isClosed: false));
                // Soldadura sobre la cara, a lo largo de la huella (lo que crea el add-in).
                sketch.Lines.Add(new SketchLine(new SketchPoint(end.FootprintStart.X, end.FootprintStart.Y), new SketchPoint(end.FootprintEnd.X, end.FootprintEnd.Y), SketchKind.Weld));
                WeldSpec? weld = member.Weld;
                string weldText = weld != null ? SketchText.Weld(weld.SizeMm, weld.AllAround) : "soldadura sin definir";
                double sideSign = info.Uy >= 0 ? 1.0 : -1.0;
                sketch.Labels.Add(new SketchLabel(new SketchPoint(end.FacePoint.X, end.FaceY + sideSign * (0.6 * DimensionGapMm)) + v * 0, weldText, SketchKind.Label, 1, path + ".weld.size_mm"));
                // Retiro desde el corte del eje hasta la cara (lo que mide la creación).
                sketch.Dimensions.Add(new SketchDimension(c, c + u * end.SetbackMm, half + DimensionGapMm, end.SetbackMm,
                    SketchText.Mm(end.SetbackMm) + (gap > 0 ? " (holgura " + SketchText.Mm(gap) + ")" : ""), DimensionKind.MemberSetback, path + ".end_gap_mm"));
                // Corte del eje de la barra con el del cordón.
                if (Math.Abs(info.AxisOffsetMm) > 0.05)
                {
                    sketch.Lines.Add(new SketchLine(new SketchPoint(info.AxisOffsetMm, -12), new SketchPoint(info.AxisOffsetMm, 12), SketchKind.WorkPoint));
                }
                // Etiqueta de la barra.
                string role = member.Role ?? "barra";
                string profile = member.Profile ?? info.TypeName ?? "perfil sin definir";
                string memberText = role + " " + member.ElementId + " · " + profile + " · " + SketchText.Degrees(info.AngleInPlaneDeg) + " · corte contra la cara";
                if (member.ChordElementId.HasValue && member.ChordElementId.Value > 0 && spec.Chord != null && member.ChordElementId.Value != spec.Chord.ElementId) memberText += " · al tramo " + member.ChordElementId.Value;
                if (info.IsApproximate) memberText += " (dirección aproximada)";
                sketch.Labels.Add(new SketchLabel(c + u * (length + 15), memberText, SketchKind.Label, info.Ux >= 0 ? 0 : 2, path + ".profile"));
                // Marca del empalme Detalle B (D6): una línea discontinua a través de la barra, a su distancia de la cara.
                if (member.SpliceHint?.OffsetFromChordFaceMm is double hint && hint > 0)
                {
                    double along = end.SetbackMm + hint;
                    SketchPoint a = c + u * along + v * (half + 20);
                    SketchPoint b = c + u * along - v * (half + 20);
                    sketch.Lines.Add(new SketchLine(a, b, SketchKind.Slot));
                    sketch.Dimensions.Add(new SketchDimension(c + u * end.SetbackMm, c + u * along, -(half + DimensionGapMm), hint,
                        SketchText.Mm(hint) + " · empalme " + (member.SpliceHint.Detail ?? "B"), DimensionKind.SlotLength, path + ".splice_hint.offset_from_chord_face_mm"));
                    length = Math.Max(length, along + 120);
                }
                SketchPoint far = c + u * length;
                lowest = Math.Min(lowest, far.Y - half);
                highest = Math.Max(highest, far.Y + half);
            }
            // 4. Excentricidad: cota entre los cortes extremos sobre el eje del cordón.
            if (ends.Count >= 2)
            {
                double minOffset = double.MaxValue, maxOffset = double.MinValue;
                foreach (var e in ends)
                {
                    minOffset = Math.Min(minOffset, e.Info.AxisOffsetMm);
                    maxOffset = Math.Max(maxOffset, e.Info.AxisOffsetMm);
                }
                double eccentricity = maxOffset - minOffset;
                if (eccentricity > 0.05)
                {
                    double declared = spec.Node?.EccentricityMm ?? eccentricity;
                    string text = SketchText.Mm(declared) + " excentricidad";
                    if (Math.Abs(declared - eccentricity) > 0.5) text += " (medida " + SketchText.Mm(eccentricity) + ")";
                    sketch.Dimensions.Add(new SketchDimension(new SketchPoint(minOffset, 0), new SketchPoint(maxOffset, 0), chordHalf + 1.6 * DimensionGapMm, eccentricity,
                        text, DimensionKind.BoltSpacing, "node.eccentricity_mm"));
                }
                // Hueco entre las huellas sobre la cara (el "gap" del nudo en K).
                for (int a = 0; a < ends.Count; a++)
                {
                    for (int b = a + 1; b < ends.Count; b++)
                    {
                        if (Math.Abs(ends[a].End.FaceY - ends[b].End.FaceY) > 1e-6) continue;
                        double gapOnFace = DirectWeldGeometry.FootprintGapMm(ends[a].End, ends[b].End);
                        double faceY = ends[a].End.FaceY;
                        double left = Math.Min(Math.Max(ends[a].End.FootprintStart.X, ends[a].End.FootprintEnd.X), Math.Max(ends[b].End.FootprintStart.X, ends[b].End.FootprintEnd.X));
                        double right = Math.Max(Math.Min(ends[a].End.FootprintStart.X, ends[a].End.FootprintEnd.X), Math.Min(ends[b].End.FootprintStart.X, ends[b].End.FootprintEnd.X));
                        if (gapOnFace > 0.5)
                        {
                            sketch.Dimensions.Add(new SketchDimension(new SketchPoint(left, faceY), new SketchPoint(right, faceY), (faceY >= 0 ? 1 : -1) * 0.8 * DimensionGapMm, gapOnFace,
                                SketchText.Mm(gapOnFace) + " entre huellas", DimensionKind.BoltEdge, null));
                        }
                    }
                }
            }
            // 5. Leyenda.
            string legend = "Sistema local del nudo: X = eje del cordón, Y en el plano de la cercha · nudo soldado (sin cartela): cada barra cortada contra la cara del cordón";
            if (nodeInfo.LocalYInGlobal.HasValue)
            {
                Vec3 y = nodeInfo.LocalYInGlobal.Value;
                legend += string.Format(System.Globalization.CultureInfo.InvariantCulture, " · +Y local = global ({0:0.##}, {1:0.##}, {2:0.##})", y.X, y.Y, y.Z);
            }
            sketch.Labels.Add(new SketchLabel(new SketchPoint(chordX0, lowest - 4.4 * DimensionGapMm), legend, SketchKind.Label, 0));
            if (nodeInfo.Note != null) sketch.Notes.Add(nodeInfo.Note);
            return sketch;
        }
    }
}
