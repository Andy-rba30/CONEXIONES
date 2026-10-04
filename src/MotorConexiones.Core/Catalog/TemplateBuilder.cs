using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Lo que la persona (o la IA) dice de la plantilla al guardarla.</summary>
    public sealed class TemplateMetadata
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>Si se indica, se reutiliza (sobrescribir una plantilla existente); si no, GUID nuevo.</summary>
        public string? TemplateId { get; set; }

        /// <summary><c>warn</c>, <c>require</c> o <c>ignore</c>; nulo = el de <c>config/catalog.json</c>.</summary>
        public string? ProfilePolicy { get; set; }

        public double? AngleToleranceDeg { get; set; }
        public bool? AllowMirror { get; set; }

        public string? DocumentTitle { get; set; }
        public string? ConnectionId { get; set; }
    }

    /// <summary>
    /// Especificación + nudo → plantilla (sección 2.1 de la propuesta): quita los IDs, mide los ángulos reales de las
    /// barras en el marco canónico y escribe el patrón. La especificación debe estar sin dudas abiertas; el valor
    /// confirmado de cada duda queda escrito en su campo.
    /// </summary>
    public static class TemplateBuilder
    {
        public static CatalogTemplate Build(string rawSpecJson, ConnectionSpec spec, TemplateNode node, TemplateMetadata metadata, CatalogConfig? config = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            config ??= CatalogConfig.Default;

            string name = (metadata.Name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "La plantilla necesita un nombre.", "name",
                    "Pasa 'name' con un nombre corto y reconocible, por ejemplo 'Nudo típico cordón inferior'.");
            }
            if (spec.Chord == null || spec.Members == null || spec.Members.Count == 0)
            {
                throw new CatalogException(ErrorCodes.TemplateSpecInvalid, "La especificación necesita 'chord' y al menos una barra en 'members'.", "spec",
                    "Guarda como plantilla una especificación completa y validada.");
            }
            if (spec.Chord.ElementId != node.ChordElementId)
            {
                throw new CatalogException(ErrorCodes.TemplateSpecInvalid,
                    "El cordón de la especificación (" + spec.Chord.ElementId + ") no es el del nudo leído (" + node.ChordElementId + ").", "chord.element_id");
            }

            var open = new List<string>();
            foreach (UncertainField doubt in spec.UncertainFields ?? new List<UncertainField>())
            {
                if (TemplateJson.ToNode(doubt.UserConfirmedValue) == null) open.Add(doubt.Path);
            }
            if (open.Count > 0)
            {
                throw new CatalogException(ErrorCodes.TemplateHasOpenUncertainties,
                    "La especificación tiene dudas sin confirmar (" + string.Join(", ", open) + "): una plantilla no puede arrastrarlas a cada nudo.",
                    "uncertain_fields", "Confirma cada duda con el usuario (user_confirmed_value y el campo) y vuelve a guardar.");
            }

            JsonObject template = TemplateJson.ParseObject(rawSpecJson, "La especificación");

            // 1. Dudas confirmadas: el valor confirmado queda en su campo si este seguía vacío; la lista se vacía.
            foreach (UncertainField doubt in spec.UncertainFields ?? new List<UncertainField>())
            {
                JsonNode? current = TemplateJson.Get(template, doubt.Path);
                if (current == null)
                {
                    TemplateJson.TrySet(template, doubt.Path, TemplateJson.ToNode(doubt.UserConfirmedValue));
                }
            }
            template["uncertain_fields"] = new JsonArray();

            // 2. Fuera los IDs del nudo.
            template.Remove("node");
            if (template["chord"] is JsonObject chordObject) chordObject.Remove("element_id");

            // 3. Barras: slot en vez de element_id, y el patrón con el ángulo real.
            string policy = ProfilePolicy.Normalize(metadata.ProfilePolicy, ProfilePolicy.Normalize(config.DefaultProfilePolicy));
            var pattern = new List<MemberPatternSlot>();
            if (template["members"] is not JsonArray membersArray || membersArray.Count != spec.Members.Count)
            {
                throw new CatalogException(ErrorCodes.TemplateSpecInvalid, "La lista 'members' del JSON no coincide con la especificación leída.", "members");
            }
            for (int i = 0; i < spec.Members.Count; i++)
            {
                MemberSpec member = spec.Members[i];
                TemplateNodeMember? real = node.Find(member.ElementId);
                if (real == null)
                {
                    throw new CatalogException(ErrorCodes.TemplateSpecInvalid,
                        "La barra " + member.ElementId + " (members[" + i + "]) no está en el nudo leído del modelo.", "members[" + i + "].element_id",
                        "Guarda la plantilla desde una conexión creada o desde una especificación que valide contra el modelo abierto.");
                }
                if (membersArray[i] is not JsonObject memberObject)
                {
                    throw new CatalogException(ErrorCodes.TemplateSpecInvalid, "members[" + i + "] no es un objeto.", "members[" + i + "]");
                }
                memberObject.Remove("element_id");
                memberObject["slot"] = i;
                pattern.Add(new MemberPatternSlot
                {
                    Slot = i,
                    Role = member.Role,
                    AngleDeg = Math.Round(real.AngleDeg, 2),
                    Side = real.Side,
                    Profile = member.Profile,
                    ModelTypeName = real.TypeName,
                    ProfilePolicy = policy,
                });
            }

            // 4. Trazabilidad: la plantilla no hereda el template_id de otra plantilla.
            if (template["source"] is JsonObject sourceObject)
            {
                sourceObject.Remove("template_id");
                sourceObject.Remove("batch_id");
            }

            var result = new CatalogTemplate
            {
                TemplateId = string.IsNullOrWhiteSpace(metadata.TemplateId) ? Guid.NewGuid().ToString("D") : metadata.TemplateId!.Trim(),
                Name = name,
                Description = string.IsNullOrWhiteSpace(metadata.Description) ? null : metadata.Description!.Trim(),
                ConnectionType = spec.ConnectionType ?? "gusset_node",
                Tags = (metadata.Tags ?? new List<string>()).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                CreatedUtc = DateTime.UtcNow.ToString("o"),
                Origin = new TemplateOrigin
                {
                    ConnectionId = metadata.ConnectionId,
                    Document = metadata.DocumentTitle,
                    Drawing = spec.Source?.Drawing,
                    ElementIds = new List<long> { node.ChordElementId }.Concat(spec.Members.Select(m => m.ElementId)).ToList(),
                },
                ChordPattern = new ChordPattern
                {
                    Profile = spec.Chord.Profile ?? node.ChordTypeName,
                    Continuous = spec.Chord.Continuous,
                    ProfilePolicy = policy,
                },
                MemberPattern = pattern,
                Matching = new MatchingOptions
                {
                    AngleToleranceDeg = metadata.AngleToleranceDeg ?? config.AngleToleranceDeg,
                    AllowMirror = metadata.AllowMirror ?? config.AllowMirror,
                },
                SpecTemplate = template,
            };
            return result;
        }
    }
}
