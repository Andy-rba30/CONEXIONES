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
    /// Fase 12: las reglas que comparten los tres tipos de conexión (dudas, cadenas de cotas, rótulos, perfiles, ángulos y
    /// "la barra llega al nudo" con la excentricidad de la opción A), extraídas de <see cref="SpecValidator"/> para que
    /// <see cref="DirectWeldValidator"/> y <see cref="EndPlateValidator"/> digan lo mismo que el gusset.
    /// </summary>
    public static class ValidationCommon
    {
        /// <summary>Regla 8.2: cada duda sin confirmar es un error; devuelve las rutas sin confirmar.</summary>
        public static HashSet<string> CheckUncertainFields(ConnectionSpec spec, ValidationResult result)
        {
            var unresolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (spec.UncertainFields == null) return unresolved;
            for (int i = 0; i < spec.UncertainFields.Count; i++)
            {
                UncertainField u = spec.UncertainFields[i];
                if (u.UserConfirmedValue != null) continue;
                unresolved.Add(u.Path);
                result.Errors.Add(new ApiError(ErrorCodes.UnresolvedUncertainty,
                    $"La duda en '{u.Path}' no ha sido confirmada por el usuario: {u.Reason}",
                    $"uncertain_fields[{i}].user_confirmed_value",
                    "Confirma el valor con el usuario y asígnalo en user_confirmed_value antes de validar."));
            }
            return unresolved;
        }

        /// <summary>Regla 8.3: cada cadena de cotas suma su total.</summary>
        public static void CheckDimensionChains(ConnectionSpec spec, LimitsConfig limits, ValidationResult result)
        {
            if (spec.DimensionChains == null) return;
            for (int i = 0; i < spec.DimensionChains.Count; i++)
            {
                DimensionChain chain = spec.DimensionChains[i];
                double sum = chain.ValuesMm != null ? chain.ValuesMm.Sum() : 0.0;
                double diff = Math.Abs(sum - chain.ExpectedTotalMm);
                if (diff > limits.DimensionChainToleranceMm)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.DimensionChainMismatch,
                        $"La cadena de cotas '{chain.Label}' suma {sum:F1} mm pero se esperaba {chain.ExpectedTotalMm:F1} mm (diferencia {diff:F1} mm > tolerancia {limits.DimensionChainToleranceMm} mm).",
                        $"dimension_chains[{i}].values_mm",
                        $"Ajusta los valores de la cadena para que sumen exactamente {chain.ExpectedTotalMm:F1} mm o corrige expected_total_mm."));
                }
            }
        }

        /// <summary>Regla 8.4: un rótulo (<c>3/8"</c>, <c>PL10</c>, <c>Ø13/16"</c>) coincide con su valor en mm.</summary>
        public static void CheckLabel(string? label, double? valueMm, string what, string labelPath, LimitsConfig limits, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(label) || !valueMm.HasValue) return;
            if (!LabelParser.MatchesNumericValue(label, valueMm.Value, limits.LabelValueToleranceMm, out double parsed))
            {
                result.Errors.Add(new ApiError(ErrorCodes.LabelValueMismatch,
                    $"El rótulo de {what} '{label}' ({parsed:F3} mm) no coincide con el valor ({valueMm.Value:F3} mm).",
                    labelPath, "Corrige el número o el rótulo para que coincidan dentro de la tolerancia de 0.05 mm."));
            }
        }

        /// <summary>Regla 8.5: el perfil escrito es el del modelo.</summary>
        public static void CheckProfile(long elementId, string? profile, string path, string what, IModelFacts? facts, ValidationResult result)
        {
            if (facts == null || string.IsNullOrWhiteSpace(profile)) return;
            MemberModelFacts? mf = facts.GetMemberFacts(elementId);
            if (mf == null) return;
            if (ProfileMatcher.Matches(profile, mf.TypeName, mf.WidthMm, mf.HeightMm, mf.ThicknessMm)) return;
            var suggestions = ProfileMatcher.GetSuggestions(profile, facts.GetAvailableProfileNames());
            string sugText = suggestions.Count > 0 ? $" Sugerencias: {string.Join(", ", suggestions)}." : "";
            result.Errors.Add(new ApiError(ErrorCodes.ProfileMismatch,
                $"El perfil de {what} '{profile}' no coincide con el tipo del modelo '{mf.TypeName}'.{sugText}",
                path, $"Usa el tipo cargado en el modelo '{mf.TypeName}'."));
        }

        /// <summary>Regla 8.6: la inclinación del plano frente a la del modelo (sin signo).</summary>
        public static void CheckAngle(MemberSpec member, int index, IModelFacts? facts, LimitsConfig limits, ValidationResult result)
        {
            if (facts == null || !member.ExpectedAngleDeg.HasValue) return;
            MemberModelFacts? mf = facts.GetMemberFacts(member.ElementId);
            if (mf == null) return;
            double expected = NodeFrame.AngleToChordDeg(member.ExpectedAngleDeg.Value);
            double model = NodeFrame.AngleToChordDeg(mf.AngleInPlaneDeg);
            double diff = Math.Abs(expected - model);
            if (diff > limits.AngleToleranceDeg)
            {
                result.Warnings.Add(new ApiError(ErrorCodes.AngleDiffersFromModel,
                    $"El ángulo del plano ({member.ExpectedAngleDeg.Value:F1}°, inclinación {expected:F1}° respecto al cordón) difiere del de la barra en el modelo ({mf.AngleInPlaneDeg:F1}°, inclinación {model:F1}°) por {diff:F1}° > {limits.AngleToleranceDeg}°.",
                    $"members[{index}].expected_angle_deg", "Verifica la geometría en el modelo o en el plano."));
            }
        }

        /// <summary>Regla 8.8: un filete por debajo del mínimo AISC para el espesor más delgado avisa.</summary>
        public static void CheckWeld(WeldSpec? weld, double thinnerMm, string path, string what, LimitsConfig limits, ValidationResult result)
        {
            if (weld?.SizeMm.HasValue != true) return;
            double minFillet = limits.GetMinWeldFilletSize(thinnerMm);
            if (weld.SizeMm!.Value < minFillet - 0.01)
            {
                result.Warnings.Add(new ApiError(ErrorCodes.WeldBelowMinimum,
                    $"El tamaño de soldadura de {what} ({weld.SizeMm.Value:F1} mm) es menor que el mínimo AISC Tabla J2.4 ({minFillet:F1} mm) para espesor de {thinnerMm:F1} mm.",
                    path, $"Aumenta size_mm a al menos {minFillet:F0} mm o confirma con ingeniería."));
            }
        }

        /// <summary>Canto del cordón en mm según los hechos del modelo (el mayor de ancho y alto); 0 si no se conoce.</summary>
        public static double ChordDepthMm(ConnectionSpec spec, IModelFacts? facts)
        {
            if (facts == null || spec.Chord == null) return 0.0;
            MemberModelFacts? cf = facts.GetMemberFacts(spec.Chord.ElementId);
            return cf == null ? 0.0 : Math.Max(cf.WidthMm, cf.HeightMm);
        }

        /// <summary>
        /// Regla 8.10 con la opción A (Fase 12): cada <c>element_id</c> existe, es armazón estructural y llega al nudo. Si su
        /// eje no pasa por el punto de trabajo pero corta el cordón a menos de <see cref="LimitsConfig.GetMaxAxisOffsetMm"/>
        /// (medio canto del cordón) y su extremo está cerca, es un aviso <c>MEMBER_ECCENTRIC</c> con los mm; si no, el error
        /// <c>MEMBER_NOT_AT_NODE</c> de siempre. Si el JSON declara <c>axis_offset_mm</c> y el modelo mide otra cosa, el
        /// aviso lo dice.
        /// </summary>
        public static void CheckElementsReachNode(ConnectionSpec spec, IModelFacts? facts, LimitsConfig limits, ValidationResult result)
        {
            if (facts == null || spec.Node?.ElementIds == null) return;
            double maxOffset = limits.GetMaxAxisOffsetMm(ChordDepthMm(spec, facts));
            foreach (long id in spec.Node.ElementIds)
            {
                if (!facts.ElementExists(id))
                {
                    result.Errors.Add(new ApiError(ErrorCodes.ElementNotFound, $"El elemento [{id}] no existe en el modelo de Revit.", "node.element_ids", "Verifica los IDs del nudo seleccionados en Revit."));
                    continue;
                }
                if (!facts.IsStructuralMember(id))
                {
                    result.Errors.Add(new ApiError(ErrorCodes.ElementNotAMember, $"El elemento [{id}] no pertenece a la categoría de armazón estructural (Structural Framing).", "node.element_ids", "Selecciona únicamente barras de acero estructural."));
                    continue;
                }
                MemberModelFacts? mf = facts.GetMemberFacts(id);
                if (mf == null || mf.ConnectsToNode) continue;
                bool eccentric = mf.AxisDistanceMm.HasValue && mf.AxisDistanceMm.Value <= maxOffset + 1e-6
                                 && (mf.EndGapMm ?? 0.0) <= NodeReach.DefaultEndToleranceMm;
                if (!eccentric)
                {
                    string measured = mf.AxisDistanceMm.HasValue ? $" Su eje pasa a {mf.AxisDistanceMm.Value:F1} mm del punto de trabajo (máximo con aviso: {maxOffset:F1} mm)." : "";
                    result.Errors.Add(new ApiError(ErrorCodes.MemberNotAtNode,
                        $"El miembro [{id}] no concurre al nudo de trabajo.{measured}",
                        "node.element_ids", "Asegura que todos los miembros seleccionados lleguen al punto de trabajo (hasta medio canto del cordón se admite con aviso)."));
                    continue;
                }
                MemberSpec? declared = spec.Members?.FirstOrDefault(m => m.ElementId == id);
                int index = declared != null ? spec.Members!.IndexOf(declared) : -1;
                string declaredText = "";
                if (declared?.AxisOffsetMm.HasValue == true && mf.AxisOffsetMm.HasValue && Math.Abs(declared.AxisOffsetMm.Value - mf.AxisOffsetMm.Value) > limits.DimensionChainToleranceMm)
                {
                    declaredText = $" El JSON declara axis_offset_mm = {declared.AxisOffsetMm.Value.ToString("0.0", CultureInfo.InvariantCulture)} y el modelo mide {mf.AxisOffsetMm.Value.ToString("0.0", CultureInfo.InvariantCulture)}.";
                }
                string offset = mf.AxisOffsetMm.HasValue ? mf.AxisOffsetMm.Value.ToString("0.0", CultureInfo.InvariantCulture) : (mf.AxisDistanceMm ?? 0.0).ToString("0.0", CultureInfo.InvariantCulture);
                string eccentricity = spec.Node.EccentricityMm.HasValue ? $" (excentricidad del nudo {spec.Node.EccentricityMm.Value.ToString("0.0", CultureInfo.InvariantCulture)} mm" : " (excentricidad";
                result.Warnings.Add(new ApiError(ErrorCodes.MemberEccentric,
                    $"El eje del miembro [{id}] corta el cordón a {offset} mm del punto de trabajo{eccentricity} admitida hasta {maxOffset.ToString("0.0", CultureInfo.InvariantCulture)} mm).{declaredText} Cada barra se recorta y se suelda sobre su propio eje.",
                    index >= 0 ? $"members[{index}].axis_offset_mm" : "node.element_ids",
                    "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE."));
            }
        }

        /// <summary>Dirección unitaria (ux, uy) en el plano local a partir del ángulo con signo del modelo.</summary>
        public static (double Ux, double Uy) DirectionOf(double angleInPlaneDeg)
        {
            double radians = angleInPlaneDeg * Math.PI / 180.0;
            return (Math.Cos(radians), Math.Sin(radians));
        }
    }
}
