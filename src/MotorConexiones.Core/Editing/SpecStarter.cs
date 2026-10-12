using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Editing
{
    /// <summary>Lo que devuelve <see cref="SpecStarter.FromNode"/>: la especificación de arranque y de dónde salieron sus valores.</summary>
    public sealed class StarterResult
    {
        public StarterResult(string specJson, ConnectionSpec spec, CatalogTemplate? defaultsTemplate, string defaultsSource, double[] gussetBoundsMm)
        {
            SpecJson = specJson;
            Spec = spec;
            DefaultsTemplate = defaultsTemplate;
            DefaultsSource = defaultsSource;
            GussetBoundsMm = gussetBoundsMm;
        }

        /// <summary>JSON con sangría, listo para la previsualización y <c>conn_validate</c>.</summary>
        public string SpecJson { get; }
        public ConnectionSpec Spec { get; }

        /// <summary>La plantilla de la que se tomaron los valores (decisión D10), o nula si fueron los de <c>choices.json</c>.</summary>
        public CatalogTemplate? DefaultsTemplate { get; }

        /// <summary>"valores de la plantilla Nudo tipico Detalle D" o "valores por defecto de choices.json".</summary>
        public string DefaultsSource { get; }

        /// <summary>[minX, minY, maxX, maxY] de la cartela rectangular, en mm del marco canónico.</summary>
        public double[] GussetBoundsMm { get; }

        /// <summary>Texto de cabecera: qué es y de dónde salieron los valores.</summary>
        public string HeaderText => "Especificación de arranque con cartela rectangular (" + DefaultsSource + "). Es un punto de partida, no la cartela automática: rellena el panel o pide el Encargo para IA.";
    }

    /// <summary>
    /// Fase 11 (decisión D2): cuando ninguna plantilla encaja, el botón Conectar abre igual la previsualización con una
    /// especificación válida en esquema escrita desde el nudo leído del modelo: los IDs y perfiles reales, una unión
    /// <b>soldada en ranura</b> por barra y una <b>cartela rectangular</b> en el marco canónico que envuelve los extremos
    /// recortados de las barras (retiro + ranura + margen, redondeado a 5 mm) y el ancho del cordón. Los valores por
    /// defecto salen <b>primero</b> de la plantilla más reciente con el mismo perfil de cordón (D10) y, si no hay, de
    /// <c>choices.json</c>. La unión al cordón va como duda confirmada. No es la cartela automática (Fase 12):
    /// <c>conn_validate</c> manda igual.
    /// </summary>
    public static class SpecStarter
    {
        /// <summary><c>source.drawing</c> de toda especificación de arranque.</summary>
        public const string DrawingName = "Especificación de arranque (Conectar)";

        /// <summary>Redondeo de la cartela rectangular.</summary>
        public const double RoundingMm = 5.0;

        /// <summary>A partir de esta inclinación respecto al cordón la barra se llama "vertical" (montante).</summary>
        public const double VerticalFromDeg = 80.0;

        public static StarterResult FromNode(TemplateNode node, PanelChoices? choices, IReadOnlyList<CatalogTemplate>? templates = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            choices ??= PanelChoices.Default;
            StarterDefaults defaults = choices.Starter ?? new StarterDefaults();
            if (node.Members.Count == 0)
            {
                throw new CatalogException(ErrorCodes.NodeNeedsTwoMembers, "El nudo no tiene barras: no hay nada que conectar.", "element_ids",
                    "Selecciona el cordón y las barras que llegan al nudo.");
            }

            // 1. Valores por defecto: la plantilla más reciente con el mismo perfil de cordón, o choices.json.
            CatalogTemplate? template = FindDefaultsTemplate(node, templates);
            StarterValues values = template != null ? StarterValues.FromTemplate(template, defaults) : StarterValues.FromDefaults(defaults);
            string source = template != null ? "valores de la plantilla " + template.Name : "valores por defecto de choices.json";

            // 2. Barras: rol por la inclinación, perfil del modelo, soldada en ranura.
            var membersArray = new JsonArray();
            var ids = new JsonArray { node.ChordElementId };
            var ends = new List<(double X, double Y)>();
            foreach (TemplateNodeMember member in node.Members)
            {
                ids.Add(member.ElementId);
                double inclination = NodeFrame.AngleToChordDeg(member.AngleDeg);
                string role = inclination >= VerticalFromDeg ? "vertical" : "diagonal";
                membersArray.Add(new JsonObject
                {
                    ["element_id"] = member.ElementId,
                    ["role"] = role,
                    ["profile"] = string.IsNullOrWhiteSpace(member.TypeName) ? null : member.TypeName,
                    ["end_setback_mm"] = values.EndSetbackMm,
                    ["expected_angle_deg"] = Math.Round(inclination, 1),
                    ["attachment"] = new JsonObject
                    {
                        ["type"] = "welded_slot",
                        ["slot_length_mm"] = values.SlotLengthMm,
                        ["weld"] = new JsonObject { ["type"] = "fillet", ["size_mm"] = values.WeldSizeMm, ["all_around"] = values.WeldAllAround },
                    },
                });

                // Extremo recortado de la barra: del retiro al final de la ranura más el margen, con el ancho de la barra.
                double half = (member.WidthMm > 0 ? member.WidthMm : SketchNodeInfo.DefaultMemberWidthMm) / 2.0;
                double near = values.EndSetbackMm;
                double far = values.EndSetbackMm + values.SlotLengthMm + defaults.GussetMarginMm;
                double vx = -member.Uy, vy = member.Ux;
                foreach (double along in new[] { near, far })
                {
                    ends.Add((along * member.Ux + half * vx, along * member.Uy + half * vy));
                    ends.Add((along * member.Ux - half * vx, along * member.Uy - half * vy));
                }
            }

            // 3. Cartela rectangular: envuelve los extremos y el cordón, redondeada a 5 mm.
            double chordHalf = (node.ChordWidthMm > 0 ? node.ChordWidthMm : SketchNodeInfo.DefaultChordWidthMm) / 2.0;
            double beyond = chordHalf + defaults.GussetBeyondChordMm;
            double minX = Math.Min(ends.Min(p => p.X), -beyond);
            double maxX = Math.Max(ends.Max(p => p.X), beyond);
            double minY = Math.Min(ends.Min(p => p.Y), -beyond);
            double maxY = Math.Max(ends.Max(p => p.Y), beyond);
            minX = Math.Floor(minX / RoundingMm) * RoundingMm;
            minY = Math.Floor(minY / RoundingMm) * RoundingMm;
            maxX = Math.Ceiling(maxX / RoundingMm) * RoundingMm;
            maxY = Math.Ceiling(maxY / RoundingMm) * RoundingMm;
            var outline = new JsonArray
            {
                new JsonArray { minX, minY }, new JsonArray { maxX, minY }, new JsonArray { maxX, maxY }, new JsonArray { minX, maxY },
            };

            var spec = new JsonObject
            {
                ["spec_version"] = "1.0",
                ["connection_type"] = "gusset_node",
                ["source"] = new JsonObject { ["drawing"] = DrawingName },
                ["node"] = new JsonObject { ["element_ids"] = ids },
                ["chord"] = new JsonObject
                {
                    ["element_id"] = node.ChordElementId,
                    ["profile"] = string.IsNullOrWhiteSpace(node.ChordTypeName) ? null : node.ChordTypeName,
                    ["continuous"] = true,
                },
                ["gusset"] = new JsonObject
                {
                    ["thickness_mm"] = Math.Round(values.GussetThicknessMm, 3),
                    ["thickness_label"] = values.GussetThicknessLabel,
                    ["width_mm"] = maxX - minX,
                    ["height_mm"] = maxY - minY,
                    ["outline"] = new JsonObject { ["mode"] = "polygon", ["points_mm"] = outline },
                    ["chord_interface"] = values.ChordInterface,
                    ["weld_to_chord"] = new JsonObject { ["type"] = "fillet", ["size_mm"] = values.ChordWeldSizeMm, ["all_around"] = values.WeldAllAround },
                },
                ["members"] = membersArray,
                ["dimension_chains"] = new JsonArray(),
                ["uncertain_fields"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["path"] = "gusset.chord_interface",
                        ["reason"] = "Arranque de Conectar: la unión de la cartela al cordón se supuso " + values.ChordInterface + " (" + source + "); confírmala o cámbiala en el panel.",
                        ["user_confirmed_value"] = values.ChordInterface,
                    },
                },
            };

            string json = spec.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            ConnectionSpec parsed = ConnectionSpec.FromJson(json) ?? throw new InvalidOperationException("La especificación de arranque quedó vacía.");
            return new StarterResult(json, parsed, template, source, new[] { minX, minY, maxX, maxY });
        }

        /// <summary>La plantilla más reciente cuyo cordón es del mismo perfil que el del nudo (D10), o nula.</summary>
        public static CatalogTemplate? FindDefaultsTemplate(TemplateNode node, IReadOnlyList<CatalogTemplate>? templates)
        {
            if (node == null || templates == null || templates.Count == 0) return null;
            return templates
                .Where(t => t?.SpecTemplate != null && SameChordProfile(t.ChordPattern?.Profile, node.ChordTypeName))
                .OrderByDescending(t => t.CreatedUtc ?? string.Empty, StringComparer.Ordinal)
                .FirstOrDefault();
        }

        private static bool SameChordProfile(string? templateProfile, string? modelTypeName)
        {
            if (string.IsNullOrWhiteSpace(templateProfile) || string.IsNullOrWhiteSpace(modelTypeName)) return false;
            return ProfileMatcher.Matches(templateProfile, modelTypeName);
        }

        /// <summary>Los valores del arranque, de una plantilla o de <c>choices.json</c>.</summary>
        private sealed class StarterValues
        {
            public double GussetThicknessMm;
            public string GussetThicknessLabel = "3/8\"";
            public string ChordInterface = "through_slot";
            public double ChordWeldSizeMm = 5;
            public double WeldSizeMm = 5;
            public bool WeldAllAround = true;
            public double SlotLengthMm = 150;
            public double EndSetbackMm = 60;

            public static StarterValues FromDefaults(StarterDefaults d) => new StarterValues
            {
                GussetThicknessMm = d.GussetThicknessMm,
                GussetThicknessLabel = d.GussetThicknessLabel,
                ChordInterface = string.IsNullOrWhiteSpace(d.ChordInterface) ? "through_slot" : d.ChordInterface,
                ChordWeldSizeMm = d.WeldSizeMm,
                WeldSizeMm = d.WeldSizeMm,
                WeldAllAround = d.WeldAllAround,
                SlotLengthMm = d.SlotLengthMm,
                EndSetbackMm = d.EndSetbackMm,
            };

            /// <summary>Espesor, unión al cordón y soldadura de la cartela de la plantilla; ranura, retiro y soldadura de su primera barra soldada en ranura.</summary>
            public static StarterValues FromTemplate(CatalogTemplate template, StarterDefaults d)
            {
                StarterValues v = FromDefaults(d);
                ConnectionSpec? spec = null;
                try
                {
                    spec = ConnectionSpec.FromJson(template.SpecTemplate!.ToJsonString());
                }
                catch (Exception)
                {
                    // Una plantilla ilegible no impide el arranque: quedan los valores por defecto.
                }
                if (spec == null) return v;
                if (spec.Gusset != null)
                {
                    if (spec.Gusset.ThicknessMm.HasValue && spec.Gusset.ThicknessMm.Value > 0)
                    {
                        v.GussetThicknessMm = spec.Gusset.ThicknessMm.Value;
                        v.GussetThicknessLabel = string.IsNullOrWhiteSpace(spec.Gusset.ThicknessLabel)
                            ? spec.Gusset.ThicknessMm.Value.ToString("0.###", CultureInfo.InvariantCulture) + " mm"
                            : spec.Gusset.ThicknessLabel!;
                    }
                    if (!string.IsNullOrWhiteSpace(spec.Gusset.ChordInterface)) v.ChordInterface = spec.Gusset.ChordInterface!;
                    if (spec.Gusset.WeldToChord?.SizeMm is double chordWeld && chordWeld > 0) v.ChordWeldSizeMm = chordWeld;
                }
                MemberSpec? welded = spec.Members?.FirstOrDefault(m => string.Equals(m.Attachment?.Type, "welded_slot", StringComparison.OrdinalIgnoreCase));
                if (welded != null)
                {
                    if (welded.Attachment!.SlotLengthMm is double slot && slot > 0) v.SlotLengthMm = slot;
                    if (welded.EndSetbackMm is double setback && setback >= 0) v.EndSetbackMm = setback;
                    if (welded.Attachment.Weld?.SizeMm is double weld && weld > 0) v.WeldSizeMm = weld;
                    if (welded.Attachment.Weld != null) v.WeldAllAround = welded.Attachment.Weld.AllAround;
                }
                return v;
            }
        }
    }
}
