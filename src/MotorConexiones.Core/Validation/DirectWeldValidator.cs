using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Fase 12: las reglas de <c>direct_weld</c> (el nudo en K soldado): dudas y cadenas como siempre, perfiles y ángulos
    /// contra el modelo, soldadura mínima por el espesor de pared, la barra llega al nudo (con la excentricidad de la opción
    /// A), <c>chord_element_id</c> de cada barra dentro del nudo, la huella cortada cabe en la cara del cordón
    /// (<c>MEMBER_WIDER_THAN_CHORD_FACE</c>) y dos huellas de la misma cara no se pisan (<c>MEMBERS_OVERLAP_ON_FACE</c>).
    /// </summary>
    public static class DirectWeldValidator
    {
        public const string TypeName = "direct_weld";
        /// <summary>Espesor de pared supuesto cuando el modelo no lo dice (3/16").</summary>
        public const double DefaultWallMm = 4.76;

        public static void Apply(ConnectionSpec spec, IModelFacts? facts, LimitsConfig limits, ValidationResult result)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            limits ??= LimitsConfig.Default;
            ValidationCommon.CheckUncertainFields(spec, result);
            if (spec.Chord == null)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "Falta la sección 'chord' obligatoria.", "chord", "Define el cordón al que se sueldan las barras."));
            }
            if (spec.Members == null || spec.Members.Count == 0)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "direct_weld necesita al menos una barra en 'members'.", "members", "Añade las diagonales que llegan al cordón."));
                return;
            }
            ValidationCommon.CheckDimensionChains(spec, limits, result);
            if (spec.Chord != null) ValidationCommon.CheckProfile(spec.Chord.ElementId, spec.Chord.Profile, "chord.profile", "el cordón", facts, result);
            MemberModelFacts? chordFacts = facts != null && spec.Chord != null ? facts.GetMemberFacts(spec.Chord.ElementId) : null;
            double chordWall = chordFacts != null && chordFacts.ThicknessMm > 0 ? chordFacts.ThicknessMm : 6.35;
            double chordHalf = chordFacts != null && chordFacts.WidthMm > 0 ? chordFacts.WidthMm / 2.0 : 0.0;
            double faceWidth = chordFacts != null && chordFacts.HeightMm > 0 ? chordFacts.HeightMm : 0.0;
            var ends = new List<(int Index, MemberSpec Member, DirectWeldEnd End)>();
            var nodeIds = new HashSet<long>(spec.Node?.ElementIds ?? new List<long>());
            for (int i = 0; i < spec.Members.Count; i++)
            {
                MemberSpec member = spec.Members[i];
                string path = "members[" + i + "]";
                if (member.Weld == null)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La barra " + member.ElementId + " no tiene 'weld': la soldadura a la cara del cordón es obligatoria.", path + ".weld", "Añade weld {type: fillet, size_mm, all_around: true}."));
                }
                if (member.EndGapMm.HasValue && member.EndGapMm.Value < 0)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "end_gap_mm no puede ser negativo.", path + ".end_gap_mm", "Usa 0 o un valor positivo."));
                }
                if (member.ChordElementId.HasValue && member.ChordElementId.Value > 0 && nodeIds.Count > 0 && !nodeIds.Contains(member.ChordElementId.Value))
                {
                    result.Errors.Add(new ApiError(ErrorCodes.ElementNotFound, "El tramo " + member.ChordElementId.Value + " al que llega la barra " + member.ElementId + " no está en node.element_ids.", path + ".chord_element_id", "Incluye los dos tramos del cordón en node.element_ids o quita chord_element_id."));
                }
                ValidationCommon.CheckProfile(member.ElementId, member.Profile, path + ".profile", "la barra " + member.ElementId, facts, result);
                ValidationCommon.CheckAngle(member, i, facts, limits, result);
                MemberModelFacts? mf = facts?.GetMemberFacts(member.ElementId);
                double wall = mf != null && mf.ThicknessMm > 0 ? mf.ThicknessMm : DefaultWallMm;
                ValidationCommon.CheckWeld(member.Weld, Math.Min(wall, chordWall), path + ".weld.size_mm", "la barra " + member.ElementId, limits, result);
                if (mf != null && chordHalf > 0)
                {
                    var (ux, uy) = ValidationCommon.DirectionOf(mf.AngleInPlaneDeg);
                    double width = mf.WidthMm > 0 ? mf.WidthMm : 63.5;
                    double depth = mf.HeightMm > 0 ? mf.HeightMm : width;
                    if (member.EndGapMm.HasValue && member.EndGapMm.Value >= depth)
                    {
                        result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"end_gap_mm ({member.EndGapMm.Value:F1} mm) es mayor que el canto de la barra ({depth:F1} mm).", path + ".end_gap_mm", "La holgura entre el corte y la cara es de milímetros, no de centímetros."));
                    }
                    double offset = mf.AxisOffsetMm ?? member.AxisOffsetMm ?? 0.0;
                    DirectWeldEnd end = DirectWeldGeometry.ComputeEnd(ux, uy, offset, chordHalf, width, member.EndGapMm ?? 0.0);
                    ends.Add((i, member, end));
                    if (faceWidth > 0 && depth > faceWidth + 0.5)
                    {
                        result.Warnings.Add(new ApiError(ErrorCodes.MemberWiderThanChordFace,
                            $"La barra {member.ElementId} ({depth:F1} mm de canto) es más ancha que la cara del cordón ({faceWidth:F1} mm): la soldadura no cabe entera en la cara.",
                            path + ".profile", "Revisa el perfil o la orientación del cordón; no bloquea."));
                    }
                }
            }
            for (int a = 0; a < ends.Count; a++)
            {
                for (int b = a + 1; b < ends.Count; b++)
                {
                    if (!DirectWeldGeometry.FootprintsOverlap(ends[a].End, ends[b].End)) continue;
                    double overlap = -DirectWeldGeometry.FootprintGapMm(ends[a].End, ends[b].End);
                    result.Warnings.Add(new ApiError(ErrorCodes.MembersOverlapOnFace,
                        $"Las huellas de las barras {ends[a].Member.ElementId} y {ends[b].Member.ElementId} se pisan {overlap.ToString("0.0", CultureInfo.InvariantCulture)} mm sobre la cara del cordón.",
                        "members[" + ends[b].Index + "]", "En el modelo las barras se cruzan antes de llegar a la cara: separa las diagonales o acepta la soldadura compartida."));
                }
            }
            ValidationCommon.CheckElementsReachNode(spec, facts, limits, result);
        }
    }
}
