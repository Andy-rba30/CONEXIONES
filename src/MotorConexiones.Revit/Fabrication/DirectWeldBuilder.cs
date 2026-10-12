using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Fase 12: la fabricación de <c>direct_weld</c> (el nudo en K soldado): cada barra se corta contra la cara del cordón y
    /// se suelda en todo el contorno; sin cartela, sin placas, sin pernos. El corte plano inclinado del extremo es el único
    /// mecanismo nuevo de la fase y lo decide el <b>sondeo 22</b>: hasta que diga qué miembro de la API lo hace, la reserva es
    /// acortar la barra por su eje (<see cref="MemberModifier.ApplySetback"/>, medido desde el corte de la barra con el eje
    /// del cordón, no desde el punto de trabajo: opción A) lo justo para que su esquina más cercana no entre en el cordón,
    /// con el aviso <c>END_CUT_APPROXIMATED</c>. La soldadura se representa sobre la huella en la cara (<c>CreateWelds</c>).
    /// </summary>
    public static class DirectWeldBuilder
    {
        public static void Build(Document document, ConnectionSpec spec, ResolvedNode node, IFabricationBackend backend, LimitsConfig limits,
            List<ElementId> createdIds, List<ModifiedMemberRecord> modifiedMembers, List<ApiError> warnings)
        {
            if (spec.Members == null || spec.Members.Count == 0) return;
            NodeFrame frame = node.Frame;
            MemberModelFacts? chordFacts = new RevitModelFacts(document, frame).GetMemberFacts(node.Chord.Id);
            // Medio canto del cordón en el plano de la cercha: el lado menor del perfil (el 8" del HSS12X8; iguales en un cuadrado).
            double chordHalf = ChordHalfDepthMm(chordFacts);
            bool approximated = false;
            foreach (MemberSpec memberSpec in spec.Members)
            {
                MemberInfo? info = node.Find(memberSpec.ElementId);
                if (info == null) continue;
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, info.Start, info.End, frame.Origin);
                double offset = NodeInspector.AxisOffsetMm(frame, info);
                Vec3 cut = frame.Origin + frame.X * offset;
                MemberModelFacts? memberFacts = new RevitModelFacts(document, frame).GetMemberFacts(info.Id);
                double width = memberFacts != null && memberFacts.WidthMm > 0 ? Math.Min(memberFacts.WidthMm, memberFacts.HeightMm > 0 ? memberFacts.HeightMm : memberFacts.WidthMm) : 63.5;
                DirectWeldEnd end = DirectWeldGeometry.ComputeEnd(ux, uy, offset, chordHalf, width, memberSpec.EndGapMm ?? 0.0);

                // Reserva (sondeo 22 pendiente): retiro por el eje desde el corte propio de la barra.
                ModifiedMemberRecord? modified = MemberModifier.ApplySetback(document, info.Instance, cut, end.AxisSetbackMm);
                if (modified != null)
                {
                    modifiedMembers.Add(modified);
                    approximated = true;
                }
                JsonLineLogger.Write(new
                {
                    @event = "direct_weld_member",
                    element_id = info.Id,
                    axis_offset_mm = Math.Round(offset, 2),
                    setback_ideal_mm = Math.Round(end.SetbackMm, 2),
                    axis_setback_mm = Math.Round(end.AxisSetbackMm, 2),
                    face_y_mm = Math.Round(end.FaceY, 2),
                    footprint_mm = Math.Round(end.FootprintLengthMm, 2),
                    chord_element_id = memberSpec.ChordElementId,
                });

                double weldSize = memberSpec.Weld?.SizeMm ?? 5.0;
                var lines = new List<WeldLine2D> { DirectWeldGeometry.WeldLine(end, weldSize) };
                createdIds.AddRange(backend.CreateWelds(document, frame, lines, "Soldadura directa barra " + memberSpec.ElementId));
            }
            if (approximated)
            {
                warnings.Add(new ApiError(ErrorCodes.EndCutApproximated,
                    "El corte inclinado contra la cara del cordón no está disponible todavía (sondeo 22): cada barra se acortó por su eje hasta dejar su esquina fuera del cordón, con el retiro medido desde su propio corte con el eje del cordón.",
                    "members", "Informativo: el sondeo 22 de docs/instalacion/fase-12.md decide el corte plano; mientras, la soldadura va sobre la huella en la cara."));
            }
        }

        /// <summary>Medio canto del cordón en el plano de la cercha (mm): el lado menor del perfil; 38,1 si el modelo no lo dice.</summary>
        public static double ChordHalfDepthMm(MemberModelFacts? chord)
        {
            if (chord == null) return 38.1;
            double a = chord.WidthMm, b = chord.HeightMm;
            double inPlane = a > 0 && b > 0 ? Math.Min(a, b) : Math.Max(a, b);
            return inPlane > 0 ? inPlane / 2.0 : 38.1;
        }
    }
}
