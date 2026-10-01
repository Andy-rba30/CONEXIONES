using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Resultado de la validación de una especificación de conexión.
    /// </summary>
    public sealed class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<ApiError> Errors { get; } = new List<ApiError>();
        public List<ApiError> Warnings { get; } = new List<ApiError>();
        public string? ValidationToken { get; set; }
    }

    /// <summary>
    /// Validador maestro del contrato v1 con las 10 reglas de validación de la sección 8 del encargo.
    /// </summary>
    public static class SpecValidator
    {
        public static ValidationResult Validate(
            string? rawJson,
            ConnectionSpec? spec,
            IModelFacts? modelFacts = null,
            LimitsConfig? limits = null)
        {
            var result = new ValidationResult();
            limits = limits ?? LimitsConfig.Default;

            // 1. Esquema: JSON Schema estructural (campos obligatorios, tipos, enumeraciones, additionalProperties: false)
            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                var schemaErrors = JsonSchemaValidator.Validate(rawJson);
                result.Errors.AddRange(schemaErrors);
            }

            if (spec == null)
            {
                if (result.Errors.Count == 0)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La especificación no pudo ser deserializada.", "", "Revisa la estructura del JSON."));
                }
                return result;
            }

            // 2. Dudas e incertidumbres explícitas (sección 8.2)
            var unresolvedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (spec.UncertainFields != null)
            {
                for (int i = 0; i < spec.UncertainFields.Count; i++)
                {
                    var u = spec.UncertainFields[i];
                    if (u.UserConfirmedValue == null)
                    {
                        unresolvedPaths.Add(u.Path);
                        result.Errors.Add(new ApiError(
                            ErrorCodes.UnresolvedUncertainty,
                            $"La duda en '{u.Path}' no ha sido confirmada por el usuario: {u.Reason}",
                            $"uncertain_fields[{i}].user_confirmed_value",
                            "Confirma el valor con el usuario y asígnalo en user_confirmed_value antes de validar."));
                    }
                }
            }

            // Validar que ningún campo esencial sea nulo sin estar en uncertain_fields
            if (spec.Chord == null)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "Falta la sección 'chord' obligatoria.", "chord", "Define el cordón principal."));
            }
            if (spec.Gusset == null)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "Falta la sección 'gusset' obligatoria.", "gusset", "Define la cartela."));
            }
            else
            {
                if (string.IsNullOrEmpty(spec.Gusset.ChordInterface) && !unresolvedPaths.Contains("gusset.chord_interface"))
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "El campo 'gusset.chord_interface' es nulo pero no está declarado en uncertain_fields.", "gusset.chord_interface", "Indica el tipo de unión o decláralo en uncertain_fields."));
                }
            }

            // 3. Cadenas de cotas (sección 8.3)
            if (spec.DimensionChains != null)
            {
                for (int i = 0; i < spec.DimensionChains.Count; i++)
                {
                    var chain = spec.DimensionChains[i];
                    double sum = chain.ValuesMm != null ? chain.ValuesMm.Sum() : 0.0;
                    double diff = Math.Abs(sum - chain.ExpectedTotalMm);
                    if (diff > limits.DimensionChainToleranceMm)
                    {
                        result.Errors.Add(new ApiError(
                            ErrorCodes.DimensionChainMismatch,
                            $"La cadena de cotas '{chain.Label}' suma {sum:F1} mm pero se esperaba {chain.ExpectedTotalMm:F1} mm (diferencia {diff:F1} mm > tolerancia {limits.DimensionChainToleranceMm} mm).",
                            $"dimension_chains[{i}].values_mm",
                            $"Ajusta los valores de la cadena para que sumen exactamente {chain.ExpectedTotalMm:F1} mm o corrige expected_total_mm."));
                    }
                }
            }

            // 4. Etiquetas contra números (sección 8.4)
            if (spec.Gusset != null)
            {
                if (!string.IsNullOrWhiteSpace(spec.Gusset.ThicknessLabel) && spec.Gusset.ThicknessMm.HasValue)
                {
                    if (!LabelParser.MatchesNumericValue(spec.Gusset.ThicknessLabel, spec.Gusset.ThicknessMm.Value, limits.LabelValueToleranceMm, out double parsedGussetTh))
                    {
                        result.Errors.Add(new ApiError(
                            ErrorCodes.LabelValueMismatch,
                            $"El rótulo de espesor '{spec.Gusset.ThicknessLabel}' ({parsedGussetTh:F3} mm) no coincide con thickness_mm ({spec.Gusset.ThicknessMm.Value:F3} mm).",
                            "gusset.thickness_label",
                            "Corrige thickness_mm o el rótulo para que coincidan dentro de la tolerancia de 0.05 mm."));
                    }
                }
            }

            if (spec.Members != null)
            {
                for (int m = 0; m < spec.Members.Count; m++)
                {
                    var member = spec.Members[m];
                    var att = member.Attachment;
                    if (att != null)
                    {
                        if (att.Plate != null && !string.IsNullOrWhiteSpace(att.Plate.ThicknessLabel) && att.Plate.ThicknessMm.HasValue)
                        {
                            if (!LabelParser.MatchesNumericValue(att.Plate.ThicknessLabel, att.Plate.ThicknessMm.Value, limits.LabelValueToleranceMm, out double parsedPlTh))
                            {
                                result.Errors.Add(new ApiError(
                                    ErrorCodes.LabelValueMismatch,
                                    $"El rótulo de la placa '{att.Plate.ThicknessLabel}' ({parsedPlTh:F3} mm) no coincide con thickness_mm ({att.Plate.ThicknessMm.Value:F3} mm).",
                                    $"members[{m}].attachment.plate.thickness_label",
                                    "Ajusta thickness_mm o el rótulo para que coincidan."));
                            }
                        }

                        if (att.Bolts != null && !string.IsNullOrWhiteSpace(att.Bolts.DiameterLabel) && att.Bolts.DiameterMm.HasValue)
                        {
                            if (!LabelParser.MatchesNumericValue(att.Bolts.DiameterLabel, att.Bolts.DiameterMm.Value, limits.LabelValueToleranceMm, out double parsedBoltDia))
                            {
                                result.Errors.Add(new ApiError(
                                    ErrorCodes.LabelValueMismatch,
                                    $"El rótulo de pernos '{att.Bolts.DiameterLabel}' ({parsedBoltDia:F3} mm) no coincide con diameter_mm ({att.Bolts.DiameterMm.Value:F3} mm).",
                                    $"members[{m}].attachment.bolts.diameter_label",
                                    "Ajusta diameter_mm o el rótulo de pernos para que coincidan."));
                            }
                        }
                    }
                }
            }

            // 5. Perfiles (sección 8.5)
            if (modelFacts != null)
            {
                if (spec.Chord != null && !string.IsNullOrWhiteSpace(spec.Chord.Profile))
                {
                    var chordFacts = modelFacts.GetMemberFacts(spec.Chord.ElementId);
                    if (chordFacts != null)
                    {
                        if (!ProfileMatcher.Matches(spec.Chord.Profile, chordFacts.TypeName, chordFacts.WidthMm, chordFacts.HeightMm, chordFacts.ThicknessMm))
                        {
                            var suggestions = ProfileMatcher.GetSuggestions(spec.Chord.Profile, modelFacts.GetAvailableProfileNames());
                            string sugText = suggestions.Count > 0 ? $" Sugerencias: {string.Join(", ", suggestions)}." : "";
                            result.Errors.Add(new ApiError(
                                ErrorCodes.ProfileMismatch,
                                $"El perfil del cordón '{spec.Chord.Profile}' no coincide con el tipo del modelo '{chordFacts.TypeName}'.{sugText}",
                                "chord.profile",
                                $"Usa el tipo cargado en el modelo '{chordFacts.TypeName}' o carga la familia adecuada."));
                        }
                    }
                }

                if (spec.Members != null)
                {
                    for (int m = 0; m < spec.Members.Count; m++)
                    {
                        var member = spec.Members[m];
                        if (!string.IsNullOrWhiteSpace(member.Profile))
                        {
                            var memberFacts = modelFacts.GetMemberFacts(member.ElementId);
                            if (memberFacts != null)
                            {
                                if (!ProfileMatcher.Matches(member.Profile, memberFacts.TypeName, memberFacts.WidthMm, memberFacts.HeightMm, memberFacts.ThicknessMm))
                                {
                                    var suggestions = ProfileMatcher.GetSuggestions(member.Profile, modelFacts.GetAvailableProfileNames());
                                    string sugText = suggestions.Count > 0 ? $" Sugerencias: {string.Join(", ", suggestions)}." : "";
                                    result.Errors.Add(new ApiError(
                                        ErrorCodes.ProfileMismatch,
                                        $"El perfil del miembro '{member.Profile}' no coincide con el tipo del modelo '{memberFacts.TypeName}'.{sugText}",
                                        $"members[{m}].profile",
                                        $"Usa el tipo cargado en el modelo '{memberFacts.TypeName}'."));
                                }
                            }
                        }
                    }
                }
            }

            // 6. Ángulos (sección 8.6)
            if (modelFacts != null && spec.Members != null)
            {
                for (int m = 0; m < spec.Members.Count; m++)
                {
                    var member = spec.Members[m];
                    if (member.ExpectedAngleDeg.HasValue)
                    {
                        var memberFacts = modelFacts.GetMemberFacts(member.ElementId);
                        if (memberFacts != null)
                        {
                            double diff = Math.Abs(member.ExpectedAngleDeg.Value - memberFacts.AngleInPlaneDeg);
                            if (diff > limits.AngleToleranceDeg)
                            {
                                result.Warnings.Add(new ApiError(
                                    ErrorCodes.AngleDiffersFromModel,
                                    $"El ángulo del plano ({member.ExpectedAngleDeg.Value:F1}°) difiere del ángulo en el modelo ({memberFacts.AngleInPlaneDeg:F1}°) por {diff:F1}° > {limits.AngleToleranceDeg}°.",
                                    $"members[{m}].expected_angle_deg",
                                    "Verifica la geometría en el modelo o en el plano."));
                            }
                        }
                    }
                }
            }

            // 7. Pernos (sección 8.7)
            if (spec.Members != null)
            {
                for (int m = 0; m < spec.Members.Count; m++)
                {
                    var member = spec.Members[m];
                    var bolts = member.Attachment?.Bolts;
                    var plate = member.Attachment?.Plate;

                    if (bolts != null && bolts.DiameterMm.HasValue)
                    {
                        double dia = bolts.DiameterMm.Value;

                        // Separación mínima AISC (tabla J3.3)
                        double minSpacing = limits.Bolts.MinSpacingFactor * dia;
                        if (bolts.SpacingMm.HasValue && bolts.SpacingMm.Value < minSpacing - 0.01)
                        {
                            result.Errors.Add(new ApiError(
                                ErrorCodes.BoltSpacingTooSmall,
                                $"La separación entre pernos ({bolts.SpacingMm.Value:F1} mm) es menor que el mínimo AISC ({minSpacing:F1} mm = {limits.Bolts.MinSpacingFactor:F3} * d) para pernos de {dia:F2} mm.",
                                $"members[{m}].attachment.bolts.spacing_mm",
                                $"Aumenta spacing_mm a al menos {Math.Ceiling(minSpacing)} mm."));
                        }

                        // Distancia al borde mínima AISC (tabla J3.4)
                        double minEdge = limits.GetMinBoltEdgeDistance(dia);
                        if (bolts.EdgeMm.HasValue && bolts.EdgeMm.Value < minEdge - 0.01)
                        {
                            result.Errors.Add(new ApiError(
                                ErrorCodes.BoltEdgeDistanceTooSmall,
                                $"La distancia al borde ({bolts.EdgeMm.Value:F1} mm) es menor que el mínimo configurado ({minEdge:F1} mm) para pernos de {bolts.DiameterLabel ?? (dia.ToString("F1") + " mm")}.",
                                $"members[{m}].attachment.bolts.edge_mm",
                                $"Revisa la cota en el plano o usa edge_mm >= {minEdge:F0}."));
                        }

                        // Pernos dentro de la placa
                        if (plate != null && plate.LengthMm.HasValue && plate.WidthMm.HasValue &&
                            bolts.Rows.HasValue && bolts.Columns.HasValue && bolts.SpacingMm.HasValue &&
                            bolts.EdgeMm.HasValue && bolts.FirstRowFromPlateEndMm.HasValue)
                        {
                            if (!Geometry2DChecks.CheckBoltsInsidePlate(
                                plate.LengthMm.Value, plate.WidthMm.Value, dia,
                                bolts.Rows.Value, bolts.Columns.Value, bolts.SpacingMm.Value,
                                bolts.EdgeMm.Value, bolts.FirstRowFromPlateEndMm.Value, out string boltDetail))
                            {
                                result.Errors.Add(new ApiError(
                                    ErrorCodes.BoltOutsidePlate,
                                    $"Los pernos quedan fuera de la placa cuchilla: {boltDetail}",
                                    $"members[{m}].attachment.bolts",
                                    "Ajusta las dimensiones de la placa o la distribución de pernos."));
                            }
                        }
                    }
                }
            }

            // 8. Soldaduras (sección 8.8)
            double gussetThickness = spec.Gusset?.ThicknessMm ?? 9.525;
            if (spec.Gusset?.WeldToChord?.SizeMm.HasValue == true)
            {
                double chordTh = 6.35; // HSS3X3X1/4 espesor por defecto si no hay hechos
                if (modelFacts != null && spec.Chord != null)
                {
                    var cf = modelFacts.GetMemberFacts(spec.Chord.ElementId);
                    if (cf != null && cf.ThicknessMm > 0) chordTh = cf.ThicknessMm;
                }

                double thinner = Math.Min(gussetThickness, chordTh);
                double minFillet = limits.GetMinWeldFilletSize(thinner);
                if (spec.Gusset.WeldToChord.SizeMm.Value < minFillet - 0.01)
                {
                    result.Warnings.Add(new ApiError(
                        ErrorCodes.WeldBelowMinimum,
                        $"El tamaño de soldadura al cordón ({spec.Gusset.WeldToChord.SizeMm.Value:F1} mm) es menor que el mínimo AISC Tabla J2.4 ({minFillet:F1} mm) para espesor de {thinner:F1} mm.",
                        "gusset.weld_to_chord.size_mm",
                        $"Aumenta size_mm a al menos {minFillet:F0} mm o confirma con ingeniería."));
                }
            }

            if (spec.Members != null)
            {
                for (int m = 0; m < spec.Members.Count; m++)
                {
                    var member = spec.Members[m];
                    var weld = member.Attachment?.Weld ?? member.Attachment?.WeldPlateToMember;
                    if (weld?.SizeMm.HasValue == true)
                    {
                        double memTh = 4.76; // 3/16" por defecto
                        if (modelFacts != null)
                        {
                            var mf = modelFacts.GetMemberFacts(member.ElementId);
                            if (mf != null && mf.ThicknessMm > 0) memTh = mf.ThicknessMm;
                        }

                        double thinner = Math.Min(gussetThickness, memTh);
                        double minFillet = limits.GetMinWeldFilletSize(thinner);
                        if (weld.SizeMm.Value < minFillet - 0.01)
                        {
                            result.Warnings.Add(new ApiError(
                                ErrorCodes.WeldBelowMinimum,
                                $"El tamaño de soldadura del miembro ({weld.SizeMm.Value:F1} mm) es menor que el mínimo AISC Tabla J2.4 ({minFillet:F1} mm) para espesor de {thinner:F1} mm.",
                                $"members[{m}].attachment.weld.size_mm",
                                $"Aumenta size_mm a al menos {minFillet:F0} mm."));
                        }
                    }
                }
            }

            // 9. Geometría: contorno cerrado, sin cruces, y placas cuchilla dentro de la cartela (sección 8.9)
            Polygon2D? gussetPolygon = null;
            if (spec.Gusset?.Outline?.PointsMm != null)
            {
                gussetPolygon = Polygon2D.FromPointArrays(spec.Gusset.Outline.PointsMm);
                if (!gussetPolygon.IsSimple(out string outlineError))
                {
                    result.Errors.Add(new ApiError(
                        ErrorCodes.OutlineInvalid,
                        $"El contorno de la cartela no es válido: {outlineError}",
                        "gusset.outline.points_mm",
                        "Define un polígono cerrado sin auto-intersecciones de al menos 3 vértices."));
                }
            }

            if (gussetPolygon != null && gussetPolygon.IsSimple(out _) && spec.Members != null)
            {
                for (int m = 0; m < spec.Members.Count; m++)
                {
                    var member = spec.Members[m];
                    var plate = member.Attachment?.Plate;
                    if (plate != null && plate.LengthMm.HasValue && plate.WidthMm.HasValue &&
                        plate.InsertionMm.HasValue && member.EndSetbackMm.HasValue)
                    {
                        double angleDeg = member.ExpectedAngleDeg ?? (modelFacts?.GetMemberFacts(member.ElementId)?.AngleInPlaneDeg ?? 45.0);
                        if (!Geometry2DChecks.CheckKnifePlateInsideGusset(
                            gussetPolygon, angleDeg, member.EndSetbackMm.Value,
                            plate.LengthMm.Value, plate.WidthMm.Value, plate.InsertionMm.Value, out string plateDetail))
                        {
                            result.Errors.Add(new ApiError(
                                ErrorCodes.PlateOutsideGusset,
                                $"La placa cuchilla no queda contenida dentro de la cartela: {plateDetail}",
                                $"members[{m}].attachment.plate",
                                "Aumenta las dimensiones de la cartela o ajusta el retiro y longitud de la placa cuchilla."));
                        }
                    }
                }
            }

            // 10. Modelo: element_ids existen, son armazón estructural y llegan al nudo (sección 8.10)
            if (modelFacts != null && spec.Node?.ElementIds != null)
            {
                foreach (var id in spec.Node.ElementIds)
                {
                    if (!modelFacts.ElementExists(id))
                    {
                        result.Errors.Add(new ApiError(
                            ErrorCodes.ElementNotFound,
                            $"El elemento [{id}] no existe en el modelo de Revit.",
                            "node.element_ids",
                            "Verifica los IDs del nudo seleccionados en Revit."));
                    }
                    else if (!modelFacts.IsStructuralMember(id))
                    {
                        result.Errors.Add(new ApiError(
                            ErrorCodes.ElementNotAMember,
                            $"El elemento [{id}] no pertenece a la categoría de armazón estructural (Structural Framing).",
                            "node.element_ids",
                            "Selecciona únicamente barras de acero estructural."));
                    }
                    else
                    {
                        var mf = modelFacts.GetMemberFacts(id);
                        if (mf != null && !mf.ConnectsToNode)
                        {
                            result.Errors.Add(new ApiError(
                                ErrorCodes.MemberNotAtNode,
                                $"El miembro [{id}] no concurre al nudo de trabajo.",
                                "node.element_ids",
                                "Asegura que todos los miembros seleccionados lleguen al punto de trabajo."));
                        }
                    }
                }
            }

            // 11. Generación del token si no hay errores
            if (result.IsValid)
            {
                result.ValidationToken = ValidationTokenGenerator.GenerateToken(spec, modelFacts, limits);
            }

            return result;
        }
    }
}
