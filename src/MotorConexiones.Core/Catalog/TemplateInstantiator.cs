using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Especificación instanciada a partir de una plantilla, lista para <c>conn_validate</c>.</summary>
    public sealed class InstantiationResult
    {
        public InstantiationResult(string specJson, ConnectionSpec spec, TemplateMatch match, List<ApiError> warnings)
        {
            SpecJson = specJson;
            Spec = spec;
            Match = match;
            Warnings = warnings;
        }

        /// <summary>JSON con sangría, con los IDs reales, el contorno transformado y <c>source.template_id</c>.</summary>
        public string SpecJson { get; }
        public ConnectionSpec Spec { get; }
        public TemplateMatch Match { get; }

        /// <summary><c>TEMPLATE_ANGLE_DEVIATION</c> y <c>TEMPLATE_PROFILE_DIFFERS</c>; nunca bloquean.</summary>
        public List<ApiError> Warnings { get; }
    }

    /// <summary>
    /// Plantilla + asignación → especificación con IDs reales (sección 3.3, paso 4 de la propuesta): barras en el orden
    /// de las ranuras, contorno de la cartela transformado según la orientación, <c>expected_angle_deg</c> = inclinación
    /// real de cada barra (así <c>conn_validate</c> no avisa por el ángulo), perfil según <c>profile_policy</c> y
    /// <c>source.template_id</c>. La especificación resultante pasa después por <c>conn_validate</c> normal.
    /// </summary>
    public static class TemplateInstantiator
    {
        public static InstantiationResult Instantiate(CatalogTemplate template, TemplateMatch match, TemplateNode node, CatalogConfig? config = null, string? batchId = null)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (node == null) throw new ArgumentNullException(nameof(node));
            config ??= CatalogConfig.Default;
            if (template.SpecTemplate == null)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La plantilla '" + template.Name + "' no tiene spec_template.", "template_id");
            }
            if (!match.IsComplete)
            {
                throw new CatalogException(ErrorCodes.TemplateNoMatch,
                    "La plantilla '" + template.Name + "' no casa con el nudo: " + match.Describe(), "element_ids",
                    "Comprueba que seleccionaste el cordón y todas las barras del nudo, o usa otra plantilla.");
            }

            var warnings = new List<ApiError>();
            JsonObject spec = TemplateJson.Clone(template.SpecTemplate);

            // 1. Cordón.
            if (spec["chord"] is not JsonObject chord)
            {
                chord = new JsonObject();
                spec["chord"] = chord;
            }
            chord["element_id"] = node.ChordElementId;
            string chordPolicy = ProfilePolicy.Normalize(template.ChordPattern?.ProfilePolicy, ProfilePolicy.Warn);
            ApplyProfilePolicy(chord, template.ChordPattern?.Profile, node.ChordTypeName, chordPolicy, "chord.profile", "el cordón " + node.ChordElementId, warnings);

            // 2. Barras en el orden de las ranuras.
            if (spec["members"] is not JsonArray templateMembers)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La plantilla no tiene 'members'.", "template_id");
            }
            var bySlot = new Dictionary<int, JsonObject>();
            for (int i = 0; i < templateMembers.Count; i++)
            {
                if (templateMembers[i] is JsonObject memberObject)
                {
                    int slot = memberObject["slot"] is JsonValue slotValue && slotValue.TryGetValue(out int s) ? s : i;
                    bySlot[slot] = memberObject;
                }
            }

            var newMembers = new JsonArray();
            var memberIds = new List<long>();
            int index = 0;
            foreach (SlotAssignment assignment in match.Assignments.OrderBy(a => a.Slot))
            {
                if (!bySlot.TryGetValue(assignment.Slot, out JsonObject? source))
                {
                    throw new CatalogException(ErrorCodes.TemplateInvalid, "La plantilla no tiene la barra de la ranura " + assignment.Slot + ".", "template_id");
                }
                TemplateNodeMember member = assignment.Member!;
                JsonObject copy = TemplateJson.Clone(source);
                copy.Remove("slot");
                copy["element_id"] = member.ElementId;
                copy["expected_angle_deg"] = Math.Round(NodeFrame.AngleToChordDeg(member.AngleDeg), 1);
                string path = "members[" + index + "]";
                ApplyProfilePolicy(copy, assignment.TemplateProfile, member.TypeName, ProfilePolicy.Normalize(assignment.ProfilePolicy, ProfilePolicy.Warn),
                    path + ".profile", "la barra " + member.ElementId, warnings);
                if (assignment.DeviationDeg > config.AngleDeviationWarningDeg + 1e-9)
                {
                    warnings.Add(new ApiError(ErrorCodes.TemplateAngleDeviation,
                        string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "La barra {0} llega a {1:0.0}° y la plantilla la tenía a {2:0.0}° (desvío {3:0.0}° > {4:0.0}°).",
                            member.ElementId, member.AngleDeg, assignment.TemplateAngleDeg, assignment.DeviationDeg, config.AngleDeviationWarningDeg),
                        path + ".expected_angle_deg",
                        "La unión sigue el ángulo real de la barra; revisa en la previsualización que la cartela la cubra."));
                }
                newMembers.Add(copy);
                memberIds.Add(member.ElementId);
                index++;
            }
            spec["members"] = newMembers;

            // 3. Nudo.
            var nodeIds = new JsonArray { node.ChordElementId };
            foreach (long id in memberIds) nodeIds.Add(id);
            spec["node"] = new JsonObject { ["element_ids"] = nodeIds };

            // 4. Contorno de la cartela en la orientación casada.
            if (match.Orientation != TemplateOrientation.Same && spec["gusset"] is JsonObject gusset
                && gusset["outline"] is JsonObject outline && outline["points_mm"] is JsonArray points)
            {
                var transformed = new List<JsonNode?>();
                foreach (JsonNode? point in points)
                {
                    if (point is JsonArray pair && pair.Count >= 2 && pair[0] is JsonValue xv && pair[1] is JsonValue yv
                        && xv.TryGetValue(out double x) && yv.TryGetValue(out double y))
                    {
                        var (tx, ty) = TemplateMatcher.TransformPoint(x, y, match.Orientation);
                        transformed.Add(new JsonArray { tx, ty });
                    }
                    else
                    {
                        transformed.Add(point == null ? null : JsonNode.Parse(point.ToJsonString()));
                    }
                }
                // Una reflexión invierte el sentido de giro del contorno; se recorre al revés para conservarlo.
                if (match.Orientation != TemplateOrientation.Both) transformed.Reverse();
                outline["points_mm"] = new JsonArray(transformed.ToArray());
            }

            // 5. Trazabilidad y dudas.
            if (spec["source"] is not JsonObject sourceObject)
            {
                sourceObject = new JsonObject();
                spec["source"] = sourceObject;
            }
            sourceObject["template_id"] = template.TemplateId;
            if (!string.IsNullOrWhiteSpace(batchId)) sourceObject["batch_id"] = batchId;
            else sourceObject.Remove("batch_id");
            spec["uncertain_fields"] = new JsonArray();

            string json = TemplateJson.Pretty(spec);
            ConnectionSpec? parsed;
            try
            {
                parsed = ConnectionSpec.FromJson(json);
            }
            catch (Exception error)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La especificación instanciada no se puede leer: " + error.Message, "template_id");
            }
            if (parsed == null)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La especificación instanciada quedó vacía.", "template_id");
            }
            return new InstantiationResult(json, parsed, match, warnings);
        }

        private static void ApplyProfilePolicy(JsonObject target, string? templateProfile, string? modelTypeName, string policy, string path, string what, List<ApiError> warnings)
        {
            bool matches = string.IsNullOrWhiteSpace(templateProfile) || string.IsNullOrWhiteSpace(modelTypeName)
                || ProfileMatcher.Matches(templateProfile!, modelTypeName!, 0, 0, 0);
            if (policy == ProfilePolicy.Require)
            {
                if (!string.IsNullOrWhiteSpace(templateProfile)) target["profile"] = templateProfile;
                return;
            }
            if (!matches && policy == ProfilePolicy.Warn)
            {
                warnings.Add(new ApiError(ErrorCodes.TemplateProfileDiffers,
                    "El perfil de " + what + " en el modelo es '" + modelTypeName + "' y la plantilla esperaba '" + templateProfile + "': se escribe el del modelo.",
                    path, "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla."));
            }
            if (!string.IsNullOrWhiteSpace(modelTypeName) && !matches)
            {
                target["profile"] = modelTypeName;
            }
            else if (!string.IsNullOrWhiteSpace(templateProfile))
            {
                target["profile"] = templateProfile;
            }
            else if (!string.IsNullOrWhiteSpace(modelTypeName))
            {
                target["profile"] = modelTypeName;
            }
        }
    }
}
