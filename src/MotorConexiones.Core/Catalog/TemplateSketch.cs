using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>
    /// Fase 11 (V4): el croquis de una plantilla <b>sin modelo</b>, para las miniaturas del catálogo. Instancia
    /// <c>spec_template</c> con IDs ficticios (cordón 1, barras 2, 3…) y construye el <see cref="SketchNodeInfo"/> con los
    /// ángulos con signo y los perfiles del <c>member_pattern</c>; el dibujo es el mismo <see cref="GussetNodeSketch"/> que
    /// pinta la previsualización. El PNG lo escribe el add-in (WPF); aquí solo las primitivas, que se prueban en la nube.
    /// </summary>
    public static class TemplateSketch
    {
        /// <summary>ID ficticio del cordón en la especificación instanciada para el croquis.</summary>
        public const long FakeChordId = 1;

        public static Sketch.Sketch Build(CatalogTemplate template, LimitsConfig? limits = null)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            ConnectionSpec spec = Instantiate(template, out _);
            SketchNodeInfo nodeInfo = NodeInfoOf(template, spec);
            return SketchBuilder.Build(spec, nodeInfo, null, limits);
        }

        /// <summary>
        /// La especificación de la plantilla con IDs ficticios: cordón <see cref="FakeChordId"/> y barras 2, 3… en el orden
        /// de las ranuras. Lanza <see cref="CatalogException"/> (<c>TEMPLATE_INVALID</c>) si falta <c>spec_template</c>.
        /// </summary>
        public static ConnectionSpec Instantiate(CatalogTemplate template, out string json)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (template.SpecTemplate == null)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La plantilla '" + template.Name + "' no tiene spec_template.", "template_id");
            }
            JsonObject spec = TemplateJson.Clone(template.SpecTemplate);
            if (spec["chord"] is not JsonObject chord)
            {
                chord = new JsonObject();
                spec["chord"] = chord;
            }
            chord["element_id"] = FakeChordId;
            if (!string.IsNullOrWhiteSpace(template.ChordPattern?.Profile) && chord["profile"] == null) chord["profile"] = template.ChordPattern!.Profile;

            var ids = new JsonArray { FakeChordId };
            if (spec["members"] is JsonArray members)
            {
                var ordered = members.OfType<JsonObject>()
                    .Select((m, i) => (Member: m, Slot: m["slot"] is JsonValue v && v.TryGetValue(out int s) ? s : i))
                    .OrderBy(t => t.Slot)
                    .ToList();
                var rebuilt = new JsonArray();
                long next = FakeChordId + 1;
                foreach (var (member, _) in ordered)
                {
                    JsonObject copy = TemplateJson.Clone(member);
                    copy.Remove("slot");
                    copy["element_id"] = next;
                    ids.Add(next);
                    rebuilt.Add(copy);
                    next++;
                }
                spec["members"] = rebuilt;
            }
            spec["node"] = new JsonObject { ["element_ids"] = ids };
            spec["uncertain_fields"] = new JsonArray();
            json = TemplateJson.Pretty(spec);
            ConnectionSpec? parsed = ConnectionSpec.FromJson(json);
            if (parsed == null)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "La especificación de la plantilla '" + template.Name + "' quedó vacía.", "template_id");
            }
            return parsed;
        }

        /// <summary>Los datos del nudo para el croquis a partir del patrón: dirección por el ángulo con signo de cada ranura y ancho por el perfil.</summary>
        public static SketchNodeInfo NodeInfoOf(CatalogTemplate template, ConnectionSpec spec)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            var slots = template.MemberPattern.OrderBy(s => s.Slot).ToList();
            var members = new List<SketchMemberInfo>();
            for (int i = 0; i < spec.Members.Count; i++)
            {
                MemberSpec member = spec.Members[i];
                MemberPatternSlot? slot = i < slots.Count ? slots[i] : null;
                double angle = slot?.AngleDeg ?? member.ExpectedAngleDeg ?? 45.0;
                double radians = UnitConverter.DegreesToRadians(angle);
                double width = WidthOf(slot?.ModelTypeName) ?? WidthOf(slot?.Profile) ?? WidthOf(member.Profile) ?? SketchNodeInfo.DefaultMemberWidthMm;
                members.Add(new SketchMemberInfo(member.ElementId, Math.Cos(radians), Math.Sin(radians), width,
                    slot?.ModelTypeName ?? slot?.Profile ?? member.Profile, NodeFrame.NormalizeDeg(angle), isApproximate: false));
            }
            double chordWidth = WidthOf(template.ChordPattern?.Profile) ?? WidthOf(spec.Chord?.Profile) ?? SketchNodeInfo.DefaultChordWidthMm;
            return new SketchNodeInfo(chordWidth, template.ChordPattern?.Profile ?? spec.Chord?.Profile, members, null, null);
        }

        /// <summary>Ancho en el plano de un perfil HSS por su designación ("HSS3X3X1/4" → 76,2 mm); nulo si no se reconoce.</summary>
        public static double? WidthOf(string? profile)
        {
            if (string.IsNullOrWhiteSpace(profile)) return null;
            if (!ProfileMatcher.TryParseHssDimensions(profile!, out double widthInches, out _, out _) || widthInches <= 0) return null;
            return UnitConverter.InchesToMm(widthInches);
        }
    }
}
