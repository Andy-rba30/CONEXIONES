using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>
    /// Croquis 2D de un nudo <c>gusset_node</c> en el plano de la cercha, en el sistema local del nudo (origen en el
    /// punto de trabajo, X a lo largo del cordón, Y perpendicular en el plano; sección 7 del encargo), todo en mm.
    /// Usa las mismas funciones de <see cref="ConnectionGeometry"/> que la creación (placa cuchilla, pernos,
    /// soldaduras), así que lo que se dibuja es lo que se crea. Sin Revit: se prueba con xUnit.
    /// </summary>
    public static class GussetNodeSketch
    {
        /// <summary>HSS3X3: ancho del cordón del Detalle D cuando el modelo no da el parámetro.</summary>
        public const double DefaultChordWidthMm = 76.2;

        /// <summary>HSS2-1/2X2-1/2: ancho de las barras del Detalle D cuando el modelo no da el parámetro.</summary>
        public const double DefaultMemberWidthMm = 63.5;

        /// <summary>Separación de la primera cota respecto al borde que mide.</summary>
        public const double DimensionGapMm = 40.0;

        /// <summary>Separación entre cotas apiladas.</summary>
        public const double DimensionStepMm = 45.0;

        private const double WorkPointCrossMm = 15.0;

        public static SketchModel Build(ConnectionSpec spec, SketchNodeInput? node)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            node = node ?? SketchNodeInput.Schematic(spec);

            var model = new SketchModel { IsSchematic = node.IsSchematic };
            model.Notes.AddRange(node.Notes);

            // 1. Alcance del dibujo: la cartela manda; las barras se dibujan hasta pasar su unión.
            List<Point2D> outline = OutlinePoints(spec.Gusset);
            double reach = 300.0;
            foreach (Point2D p in outline)
            {
                reach = Math.Max(reach, Math.Max(Math.Abs(p.X), Math.Abs(p.Y)));
            }
            double memberLength = reach * 1.35;
            if (spec.Members != null)
            {
                foreach (MemberSpec m in spec.Members)
                {
                    memberLength = Math.Max(memberLength, MemberDrawLength(m));
                }
            }
            double chordHalfLength = reach * 1.6 + node.ChordWidthMm;

            // 2. Cordón: eje, cuerpo y rótulo.
            double chordHalf = node.ChordWidthMm / 2.0;
            model.Lines.Add(new SketchLine(new Point2D(-chordHalfLength, 0), new Point2D(chordHalfLength, 0), SketchLineKind.Axis, "chord"));
            model.Polygons.Add(new SketchPolygon(new[]
            {
                new Point2D(-chordHalfLength, -chordHalf),
                new Point2D(chordHalfLength, -chordHalf),
                new Point2D(chordHalfLength, chordHalf),
                new Point2D(-chordHalfLength, chordHalf),
            }, SketchPolygonKind.Member, "chord", node.ChordProfile));
            model.Labels.Add(new SketchLabel(new Point2D(-chordHalfLength + 20.0, -chordHalf - 14.0),
                "Cordón " + (spec.Chord?.Profile ?? node.ChordProfile ?? ""), "chord.profile"));

            // 3. Punto de trabajo.
            model.Lines.Add(new SketchLine(new Point2D(-WorkPointCrossMm, 0), new Point2D(WorkPointCrossMm, 0), SketchLineKind.WorkPoint, "node"));
            model.Lines.Add(new SketchLine(new Point2D(0, -WorkPointCrossMm), new Point2D(0, WorkPointCrossMm), SketchLineKind.WorkPoint, "node"));
            model.Labels.Add(new SketchLabel(new Point2D(WorkPointCrossMm + 4.0, -WorkPointCrossMm - 4.0), "PT", "node"));

            // 4. Cartela: contorno, cotas de ancho y alto, espesor como etiqueta.
            if (spec.Gusset != null)
            {
                DrawGusset(model, spec.Gusset, outline);
            }

            // 5. Barras: cuerpo retirado, eje, retiro, unión (ranura soldada o placa cuchilla empernada), soldaduras.
            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    MemberSpec member = spec.Members[i];
                    SketchMemberInput? input = node.Find(member.ElementId);
                    if (input == null && i < node.Members.Count) input = node.Members[i];
                    if (input == null)
                    {
                        input = SketchNodeInput.Schematic(spec).Members.Count > i
                            ? SketchNodeInput.Schematic(spec).Members[i]
                            : new SketchMemberInput(member.ElementId, 1, 1, DefaultMemberWidthMm, member.Profile, fromModel: false);
                        model.Notes.Add("La barra " + member.ElementId + " no tiene datos del nudo: dirección esquemática.");
                    }
                    DrawMember(model, spec, member, i, input, memberLength);
                }
            }

            model.ComputeBounds();
            return model;
        }

        /// <summary>Largo con el que se dibuja una barra: hasta pasar la unión y dejar sitio al rótulo.</summary>
        public static double MemberDrawLength(MemberSpec member)
        {
            double setback = member.EndSetbackMm.GetValueOrDefault(0.0);
            double joint = 0.0;
            AttachmentSpec? a = member.Attachment;
            if (a != null)
            {
                if (IsBolted(a) && a.Plate != null)
                {
                    joint = Math.Max(a.Plate.InsertionMm.GetValueOrDefault(80.0), a.Plate.LengthMm.GetValueOrDefault(170.0));
                }
                else
                {
                    joint = a.SlotLengthMm.GetValueOrDefault(150.0);
                }
            }
            return setback + joint + 160.0;
        }

        /// <summary>Texto del espesor: <c>PL 3/8" (9,5 mm)</c>, <c>PL10 (10,0 mm)</c> o <c>PL 9,5 mm</c> si no hay etiqueta.</summary>
        public static string ThicknessText(string? label, double? thicknessMm)
        {
            string mm = thicknessMm.HasValue ? SketchFormat.Mm(thicknessMm.Value) + " mm" : "sin espesor";
            if (string.IsNullOrWhiteSpace(label)) return "PL " + mm;
            string l = label!.Trim();
            return (l.StartsWith("PL", StringComparison.OrdinalIgnoreCase) ? l : "PL " + l) + " (" + mm + ")";
        }

        private static List<Point2D> OutlinePoints(GussetSpec? gusset)
        {
            var points = new List<Point2D>();
            if (gusset == null) return points;
            foreach (BoltPosition p in ConnectionGeometry.GetGussetOutline(gusset))
            {
                points.Add(new Point2D(p.X, p.Y));
            }
            return points;
        }

        private static void DrawGusset(SketchModel model, GussetSpec gusset, List<Point2D> outline)
        {
            if (outline.Count < 3)
            {
                model.Notes.Add("La cartela no tiene contorno (gusset.outline.points_mm con al menos 3 puntos): no se dibuja.");
                model.Labels.Add(new SketchLabel(new Point2D(0, 60), "Cartela sin contorno", "gusset.outline.points_mm", emphasized: true));
                return;
            }

            model.Polygons.Add(new SketchPolygon(outline, SketchPolygonKind.Gusset, "gusset.outline.points_mm", gusset.ThicknessLabel));

            double minX = double.PositiveInfinity, minY = double.PositiveInfinity, maxX = double.NegativeInfinity, maxY = double.NegativeInfinity;
            foreach (Point2D p in outline)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            // Ancho por encima del borde superior y alto a la derecha del borde derecho.
            model.Dimensions.Add(new SketchDimension(new Point2D(minX, maxY), new Point2D(maxX, maxY), DimensionGapMm, maxX - minX, "gusset.width_mm"));
            model.Dimensions.Add(new SketchDimension(new Point2D(maxX, maxY), new Point2D(maxX, minY), DimensionGapMm, maxY - minY, "gusset.height_mm"));

            model.Labels.Add(new SketchLabel(new Point2D(minX + 25.0, maxY - 35.0), ThicknessText(gusset.ThicknessLabel, gusset.ThicknessMm), "gusset.thickness_mm", emphasized: true));

            if (gusset.WidthMm.HasValue && Math.Abs(gusset.WidthMm.Value - (maxX - minX)) > 0.5)
            {
                model.Notes.Add("gusset.width_mm (" + SketchFormat.Mm(gusset.WidthMm.Value) + ") no coincide con el ancho del contorno (" + SketchFormat.Mm(maxX - minX) + ").");
            }
            if (gusset.HeightMm.HasValue && Math.Abs(gusset.HeightMm.Value - (maxY - minY)) > 0.5)
            {
                model.Notes.Add("gusset.height_mm (" + SketchFormat.Mm(gusset.HeightMm.Value) + ") no coincide con el alto del contorno (" + SketchFormat.Mm(maxY - minY) + ").");
            }
        }

        private static void DrawMember(SketchModel model, ConnectionSpec spec, MemberSpec member, int index, SketchMemberInput input, double drawLength)
        {
            string basePath = "members[" + index + "]";
            double ux = input.Ux, uy = input.Uy;
            double vx = -uy, vy = ux;
            double half = input.WidthMm / 2.0;
            double setback = member.EndSetbackMm.GetValueOrDefault(0.0);

            Point2D P(double along, double across) => new Point2D(along * ux + across * vx, along * uy + across * vy);

            // Eje desde el punto de trabajo hacia fuera.
            model.Lines.Add(new SketchLine(P(0, 0), P(drawLength, 0), SketchLineKind.Axis, basePath));

            // Cuerpo real de la barra: empieza en el retiro (extremo tras el setback).
            model.Polygons.Add(new SketchPolygon(new[]
            {
                P(setback, -half), P(setback, half), P(drawLength, half), P(drawLength, -half),
            }, SketchPolygonKind.Member, basePath, input.Profile));

            // Retiro: del punto de trabajo al extremo, a la derecha del eje (lado -v).
            if (setback > 0.0)
            {
                model.Dimensions.Add(new SketchDimension(P(0, 0), P(setback, 0), -(half + DimensionGapMm), setback, basePath + ".end_setback_mm", "retiro"));
            }

            // Rótulo: rol, perfil y ángulo con el cordón (del modelo o esquemático).
            string role = RoleText(member.Role);
            string angle = SketchFormat.Degrees(input.AngleToChordDeg) + (input.FromModel ? "" : " (plano)");
            model.Labels.Add(new SketchLabel(P(drawLength - 10.0, half + 16.0), role + " " + (member.Profile ?? input.Profile ?? "") + " · " + angle, basePath + ".profile"));

            AttachmentSpec? attachment = member.Attachment;
            if (attachment == null) return;

            if (IsBolted(attachment))
            {
                DrawKnifePlate(model, member, attachment, basePath, ux, uy, setback, half, P);
            }
            else
            {
                double slotLength = attachment.SlotLengthMm.GetValueOrDefault(150.0);
                double gussetHalfThickness = spec.Gusset?.ThicknessMm.GetValueOrDefault(9.525) / 2.0 ?? 9.525 / 2.0;
                string slotPath = basePath + ".attachment.slot_length_mm";
                // Ranura en el HSS (oculta): dos aristas al espesor de la cartela y el fondo.
                model.Lines.Add(new SketchLine(P(setback, gussetHalfThickness), P(setback + slotLength, gussetHalfThickness), SketchLineKind.Hidden, slotPath));
                model.Lines.Add(new SketchLine(P(setback, -gussetHalfThickness), P(setback + slotLength, -gussetHalfThickness), SketchLineKind.Hidden, slotPath));
                model.Lines.Add(new SketchLine(P(setback + slotLength, -gussetHalfThickness), P(setback + slotLength, gussetHalfThickness), SketchLineKind.Hidden, slotPath));
                // Largo de la ranura, a la izquierda del eje (lado +v).
                model.Dimensions.Add(new SketchDimension(P(setback, 0), P(setback + slotLength, 0), half + DimensionGapMm, slotLength, slotPath, "ranura"));
            }

            // Soldaduras: las mismas líneas que crea el add-in.
            foreach (WeldLine2D weld in ConnectionGeometry.ComputeWeldLines(ux, uy, setback, member))
            {
                model.Lines.Add(new SketchLine(new Point2D(weld.Start.X, weld.Start.Y), new Point2D(weld.End.X, weld.End.Y),
                    SketchLineKind.Weld, basePath + ".attachment", SketchFormat.Mm(weld.SizeMm)));
            }
        }

        private static void DrawKnifePlate(SketchModel model, MemberSpec member, AttachmentSpec attachment, string basePath,
            double ux, double uy, double setback, double memberHalf, Func<double, double, Point2D> P)
        {
            KnifePlateSpec? plate = attachment.Plate;
            if (plate == null)
            {
                model.Notes.Add("La barra " + member.ElementId + " es bolted_knife_plate sin attachment.plate: no se dibuja la placa.");
                return;
            }

            string platePath = basePath + ".attachment.plate";
            double length = plate.LengthMm.GetValueOrDefault(170.0);
            double width = plate.WidthMm.GetValueOrDefault(140.0);
            double insertion = plate.InsertionMm.GetValueOrDefault(80.0);
            double plateHalf = width / 2.0;
            double freeEnd = setback - (length - insertion);
            double insertedEnd = setback + insertion;

            var corners = new List<Point2D>();
            foreach (BoltPosition c in ConnectionGeometry.ComputeKnifePlateCorners(ux, uy, setback, plate))
            {
                corners.Add(new Point2D(c.X, c.Y));
            }
            model.Polygons.Add(new SketchPolygon(corners, SketchPolygonKind.KnifePlate, platePath, plate.ThicknessLabel));
            model.Labels.Add(new SketchLabel(P((freeEnd + insertedEnd) / 2.0, -plateHalf + 14.0), ThicknessText(plate.ThicknessLabel, plate.ThicknessMm), platePath + ".thickness_mm", emphasized: true));

            // Ranura del HSS para la placa (oculta), del extremo retirado hasta la inserción.
            double plateHalfThickness = plate.ThicknessMm.GetValueOrDefault(10.0) / 2.0;
            model.Lines.Add(new SketchLine(P(setback, plateHalfThickness), P(insertedEnd, plateHalfThickness), SketchLineKind.Hidden, platePath + ".insertion_mm"));
            model.Lines.Add(new SketchLine(P(setback, -plateHalfThickness), P(insertedEnd, -plateHalfThickness), SketchLineKind.Hidden, platePath + ".insertion_mm"));

            // Inserción: a la derecha del eje (lado -v), por fuera de la cota del retiro.
            model.Dimensions.Add(new SketchDimension(P(setback, 0), P(insertedEnd, 0), -(Math.Max(memberHalf, plateHalf) + DimensionGapMm + DimensionStepMm), insertion, platePath + ".insertion_mm", "inserción"));

            // Largo de la placa, a la izquierda del eje (lado +v), segunda fila.
            model.Dimensions.Add(new SketchDimension(P(freeEnd, 0), P(freeEnd + length, 0), plateHalf + DimensionGapMm + DimensionStepMm, length, platePath + ".length_mm", "placa"));

            // Ancho de la placa, en el extremo libre, segunda fila.
            model.Dimensions.Add(new SketchDimension(P(freeEnd, -plateHalf), P(freeEnd, plateHalf), DimensionGapMm + DimensionStepMm, width, platePath + ".width_mm", "placa"));

            BoltPatternSpec? bolts = attachment.Bolts;
            if (bolts == null)
            {
                model.Notes.Add("La barra " + member.ElementId + " no tiene attachment.bolts: no se dibujan pernos.");
                return;
            }

            string boltsPath = basePath + ".attachment.bolts";
            double diameter = bolts.DiameterMm.GetValueOrDefault(15.875);
            foreach (BoltPosition b in ConnectionGeometry.ComputeBoltPositions(ux, uy, setback, plate, bolts))
            {
                model.Circles.Add(new SketchCircle(new Point2D(b.X, b.Y), diameter / 2.0, SketchCircleKind.Bolt, boltsPath));
            }

            int rows = Math.Max(1, bolts.Rows.GetValueOrDefault(1));
            int cols = Math.Max(1, bolts.Columns.GetValueOrDefault(1));
            double spacing = bolts.SpacingMm.GetValueOrDefault(60.0);
            double firstRow = bolts.FirstRowFromPlateEndMm.GetValueOrDefault(40.0);

            // Cadena a lo largo de la barra (lado +v, primera fila): borde libre → 1.ª fila → última fila → fin de la placa.
            double alongOffset = plateHalf + DimensionGapMm;
            double firstRowAt = freeEnd + firstRow;
            double lastRowAt = firstRowAt + spacing * (rows - 1);
            model.Dimensions.Add(new SketchDimension(P(freeEnd, 0), P(firstRowAt, 0), alongOffset, firstRow, boltsPath + ".first_row_from_plate_end_mm"));
            if (rows > 1)
            {
                model.Dimensions.Add(new SketchDimension(P(firstRowAt, 0), P(lastRowAt, 0), alongOffset, lastRowAt - firstRowAt, boltsPath + ".spacing_mm"));
            }
            double remainder = freeEnd + length - lastRowAt;
            if (remainder > 0.05)
            {
                model.Dimensions.Add(new SketchDimension(P(lastRowAt, 0), P(freeEnd + length, 0), alongOffset, remainder, platePath + ".length_mm"));
            }

            // Cadena transversal en el extremo libre (primera fila): borde → pernos → borde.
            double span = spacing * (cols - 1);
            double edge = (width - span) / 2.0;
            double acrossOffset = DimensionGapMm;
            model.Dimensions.Add(new SketchDimension(P(freeEnd, -plateHalf), P(freeEnd, -plateHalf + edge), acrossOffset, edge, boltsPath + ".edge_mm"));
            if (cols > 1)
            {
                model.Dimensions.Add(new SketchDimension(P(freeEnd, -plateHalf + edge), P(freeEnd, plateHalf - edge), acrossOffset, span, boltsPath + ".spacing_mm"));
            }
            model.Dimensions.Add(new SketchDimension(P(freeEnd, plateHalf - edge), P(freeEnd, plateHalf), acrossOffset, edge, boltsPath + ".edge_mm"));

            string diameterText = string.IsNullOrWhiteSpace(bolts.DiameterLabel) ? "Ø" + SketchFormat.Mm(diameter) : (rows * cols) + " Ø" + bolts.DiameterLabel + " (" + SketchFormat.Mm(diameter) + " mm)";
            model.Labels.Add(new SketchLabel(P(freeEnd + length / 2.0, plateHalf - 14.0), diameterText, boltsPath + ".diameter_mm"));

            if (bolts.EdgeMm.HasValue && Math.Abs(bolts.EdgeMm.Value - edge) > 0.5)
            {
                model.Notes.Add("bolts.edge_mm de la barra " + member.ElementId + " (" + SketchFormat.Mm(bolts.EdgeMm.Value) + ") no coincide con la distancia dibujada al borde (" + SketchFormat.Mm(edge) + ").");
            }
        }

        private static bool IsBolted(AttachmentSpec attachment)
        {
            return string.Equals(attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase);
        }

        private static string RoleText(string? role)
        {
            if (string.Equals(role, "diagonal", StringComparison.OrdinalIgnoreCase)) return "Diagonal";
            if (string.Equals(role, "vertical", StringComparison.OrdinalIgnoreCase)) return "Montante";
            if (string.Equals(role, "chord", StringComparison.OrdinalIgnoreCase)) return "Cordón";
            return string.IsNullOrWhiteSpace(role) ? "Barra" : role!;
        }
    }
}
