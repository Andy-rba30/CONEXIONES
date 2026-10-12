using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Fase 12: la fabricación de <c>end_plate</c> (empalme con placa de extremo): cada tramo se retira hasta dejar sitio a su
    /// plancha (<see cref="MemberModifier.ApplySetback"/> desde el punto de empalme), las dos planchas octogonales se crean
    /// perpendiculares al eje de la barra con <c>CreatePlate</c> (el marco de la plancha lleva Z a lo largo de la barra, X
    /// perpendicular al plano de la cercha —donde va el canto mayor del tubo, el 12" del Detalle C— e Y vertical), los
    /// pernos van <b>por cara</b> como un patrón rectangular de N × 1 cada fila (<c>CreateBoltPattern</c>, lo decide el sondeo
    /// 23: si Advance Steel rechaza el patrón de una fila, el backend cae a DirectShape) atravesando la pila de dos planchas
    /// (<see cref="BoltStack.ForTwoPlates"/>), y la soldadura de cada plancha al tubo se representa en el contorno de la huella.
    /// La tuerca a ambos lados no está en la API: perno estándar con el aviso <c>BOLT_NUTS_APPROXIMATED</c> (decisión D9).
    /// </summary>
    public static class EndPlateBuilder
    {
        public static void Build(Document document, ConnectionSpec spec, ResolvedNode node, IFabricationBackend backend, LimitsConfig limits,
            List<ElementId> createdIds, List<ModifiedMemberRecord> modifiedMembers, List<ApiError> warnings)
        {
            if (spec.Plate == null || spec.Bolts == null || spec.Members == null || spec.Members.Count == 0) return;
            NodeFrame frame = node.Frame; // colineal: X a lo largo de la barra, origen en el punto de empalme
            Vec3 splicePoint = frame.Origin;
            MemberInfo first = node.Chord;
            MemberInfo? second = node.Find(spec.Members[0].ElementId);
            if (second == null) return;

            double t = spec.Plate.ThicknessMm ?? 9.525;
            double gap = Math.Max(0.0, spec.GapMm ?? 0.0);
            double w = spec.Plate.WidthMm ?? 200.0;
            double h = spec.Plate.HeightMm ?? w;
            double corner = Math.Max(0.0, spec.Plate.CornerCutMm ?? 0.0);

            // 1. Retiros de los dos tramos desde el punto de empalme.
            double setbackA = spec.Chord?.EndSetbackMm ?? (t + gap / 2.0);
            double setbackB = spec.Members[0].EndSetbackMm ?? (t + gap / 2.0);
            ModifiedMemberRecord? modifiedA = MemberModifier.ApplySetback(document, first.Instance, splicePoint, setbackA);
            if (modifiedA != null) modifiedMembers.Add(modifiedA);
            ModifiedMemberRecord? modifiedB = MemberModifier.ApplySetback(document, second.Instance, splicePoint, setbackB);
            if (modifiedB != null) modifiedMembers.Add(modifiedB);

            // 2. El marco de la plancha: Z a lo largo de la barra (hacia el lado del primer tramo), X perpendicular al plano de la
            //    cercha (el canto mayor del tubo), Y vertical. Es un marco a derechas: X × Y = Z.
            Vec3 chordFar = first.Start.DistanceTo(splicePoint) >= first.End.DistanceTo(splicePoint) ? first.Start : first.End;
            double sideA = Math.Sign((chordFar - splicePoint).Dot(frame.X));
            if (sideA == 0) sideA = 1;
            Vec3 z = frame.X * sideA;
            Vec3 y = frame.Y;
            Vec3 x = y.Cross(z);
            NodeFrame plateFrame = NodeFrame.FromAxes(splicePoint, x, y, z);
            List<BoltPosition> outline = Octagon.Points(w, h, corner);

            // 3. Las dos planchas: la del primer tramo hacia +Z (su lado), la del segundo hacia −Z, con la holgura en el medio.
            double offsetA = gap / 2.0 + t / 2.0;
            ElementId plateA = backend.CreatePlate(document, plateFrame, outline, t, offsetA, "Plancha de extremo tramo " + first.Id);
            if (plateA != ElementId.InvalidElementId) createdIds.Add(plateA);
            ElementId plateB = backend.CreatePlate(document, plateFrame, outline, t, -offsetA, "Plancha de extremo tramo " + second.Id);
            if (plateB != ElementId.InvalidElementId) createdIds.Add(plateB);

            // 4. Pernos por cara: cada fila es un patrón de N × 1 que atraviesa las dos planchas; el plano del patrón se pone en la cara
            //    exterior de la pila (StackMaxMm, como en la cartela): la pila de ForTwoPlates va de −t/2 a t/2 + holgura + t, así
            //    que el marco de los pernos se desplaza −(t + holgura)/2 para que la pila quede centrada en el punto de empalme.
            BoltStack stack = BoltStack.ForTwoPlates(t, t, gap, spec.Bolts, limits);
            NodeFrame boltFrame = NodeFrame.FromAxes(splicePoint - z * ((t + gap) / 2.0), x, y, z);
            double diameter = spec.Bolts.DiameterMm ?? 15.875;
            int rows = 0, bolts = 0;
            foreach (BoltRow row in BoltRing.Rows(spec.Plate, spec.Bolts))
            {
                if (row.Count == 0) continue;
                BoltGrid grid = row.ToGrid();
                createdIds.AddRange(backend.CreateBoltPattern(document, boltFrame, grid, diameter, stack, "Pernos empalme fila " + row.Face));
                rows++;
                bolts += row.Count;
            }
            if (bolts > 0 && string.Equals(spec.Bolts.Nuts ?? "both_sides", "both_sides", StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add(new ApiError(ErrorCodes.BoltNutsApproximated,
                    "La tuerca a ambos lados no está en la API de Advance Steel que usa el add-in: los " + bolts + " pernos se crean como pernos estándar (cabeza y tuerca) con la longitud de la pila de dos planchas (" + stack.BoltLengthMm.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " mm).",
                    "bolts.nuts", "Informativo (decisión D9): el sondeo 23 de docs/instalacion/fase-12.md dice si hay otra opción."));
            }

            // 5. Soldadura de cada plancha al tubo: la huella del tubo (canto mayor a lo ancho) sobre la cara interior de cada plancha.
            double weldSize = spec.Plate.Weld?.SizeMm ?? 5.0;
            TubeFootprint tube = EndPlateValidator.Footprint(new RevitModelFacts(document, frame).GetMemberFacts(first.Id), new RevitModelFacts(document, frame).GetMemberFacts(second.Id), spec);
            double hw = tube.WidthMm / 2.0, hh = tube.HeightMm / 2.0;
            var contour = new List<WeldLine2D>
            {
                new WeldLine2D(new BoltPosition(-hw, -hh), new BoltPosition(hw, -hh), weldSize),
                new WeldLine2D(new BoltPosition(hw, -hh), new BoltPosition(hw, hh), weldSize),
                new WeldLine2D(new BoltPosition(hw, hh), new BoltPosition(-hw, hh), weldSize),
                new WeldLine2D(new BoltPosition(-hw, hh), new BoltPosition(-hw, -hh), weldSize),
            };
            createdIds.AddRange(backend.CreateWelds(document, NodeFrame.FromAxes(splicePoint + z * (gap / 2.0 + t), x, y, z), contour, "Soldadura plancha tramo " + first.Id));
            createdIds.AddRange(backend.CreateWelds(document, NodeFrame.FromAxes(splicePoint - z * (gap / 2.0 + t), x, y, z), contour, "Soldadura plancha tramo " + second.Id));

            JsonLineLogger.Write(new
            {
                @event = "end_plate_built",
                first_segment = first.Id,
                second_segment = second.Id,
                plate_mm = new[] { w, h, t },
                corner_cut_mm = corner,
                gap_mm = gap,
                setbacks_mm = new[] { setbackA, setbackB },
                rows,
                bolts,
                bolt_length_mm = stack.BoltLengthMm,
                tube_footprint_mm = new[] { tube.WidthMm, tube.HeightMm },
                plate_frame_z = new[] { z.X, z.Y, z.Z },
            });
        }
    }
}
