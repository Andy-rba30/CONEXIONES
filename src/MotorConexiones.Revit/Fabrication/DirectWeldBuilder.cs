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
    /// mecanismo nuevo de la fase y lo deciden los sondeos: el 22 descartó <c>SetEndReference</c> (Revit no une sin unión
    /// previa) y el 25 (ronda 12b) no pudo probar <c>BeamShortening</c> de Advance Steel porque no encontró la viga de
    /// Advance Steel de la barra de Revit (el sondeo 28 de la ronda 12c la busca en todos los ensamblados). Hasta entonces la
    /// reserva es acortar la barra por su eje (<see cref="MemberModifier.ApplySetback"/>, medido desde el corte de la barra con
    /// el eje del cordón, no desde el punto de trabajo: opción A) lo justo para que su esquina más cercana no entre en el
    /// cordón, con el aviso <c>END_CUT_APPROXIMATED</c>. La soldadura va sobre la huella en la cara (<c>CreateWelds</c>: desde
    /// la 12c, <c>WeldLine</c> de Advance Steel con reserva DirectShape).
    /// </summary>
    public static class DirectWeldBuilder
    {
        public static void Build(Document document, ConnectionSpec spec, ResolvedNode node, IFabricationBackend backend, LimitsConfig limits,
            List<ElementId> createdIds, List<ModifiedMemberRecord> modifiedMembers, List<ApiError> warnings)
        {
            if (spec.Members == null || spec.Members.Count == 0) return;
            NodeFrame frame = node.Frame;
            MemberModelFacts? chordFacts = new RevitModelFacts(document, frame).GetMemberFacts(node.Chord.Id);
            // Medio canto del cordón en el plano de la cercha (WidthMm, orientado desde la ronda 12c: el 8" del HSS12X8 girado 90°).
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
                // La huella de la barra sobre la cara se mide con su lado en el plano de la cercha (WidthMm; iguales en un cuadrado).
                double width = memberFacts != null && memberFacts.WidthMm > 0 ? memberFacts.WidthMm : 63.5;
                DirectWeldEnd end = DirectWeldGeometry.ComputeEnd(ux, uy, offset, chordHalf, width, memberSpec.EndGapMm ?? 0.0);

                // Reserva (el corte inclinado espera al sondeo 28): retiro por el eje desde el corte propio de la barra.
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
                    "El corte inclinado contra la cara del cordón no está disponible todavía (sondeos 22 y 25): cada barra se acortó por su eje hasta dejar su esquina fuera del cordón, con el retiro medido desde su propio corte con el eje del cordón.",
                    "members", "Informativo: el sondeo 28 de docs/instalacion/fase-12c.md busca la viga de Advance Steel y prueba BeamShortening; mientras, la soldadura va sobre la huella en la cara."));
            }
        }

        /// <summary>
        /// Medio canto del cordón en el plano de la cercha (mm): el lado en el plano (<see cref="MemberModelFacts.WidthMm"/>,
        /// orientado por <see cref="SectionOrientation"/> desde la ronda 12c; antes, el menor de los dos); 38,1 si el modelo no lo dice.
        /// </summary>
        public static double ChordHalfDepthMm(MemberModelFacts? chord)
        {
            if (chord == null) return 38.1;
            double inPlane = chord.WidthMm > 0 ? chord.WidthMm : chord.HeightMm;
            return inPlane > 0 ? inPlane / 2.0 : 38.1;
        }
    }
}
