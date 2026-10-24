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
        public StarterResult(string specJson, ConnectionSpec spec, CatalogTemplate? defaultsTemplate, string defaultsSource, double[] gussetBoundsMm, string? detailName = null)
        {
            SpecJson = specJson;
            Spec = spec;
            DefaultsTemplate = defaultsTemplate;
            DefaultsSource = defaultsSource;
            GussetBoundsMm = gussetBoundsMm;
            DetailName = detailName;
        }

        /// <summary>Fase 12: <c>gusset_node</c>, <c>direct_weld</c> o <c>end_plate</c>.</summary>
        public string ConnectionType => Spec.ConnectionType ?? "gusset_node";

        /// <summary>Fase 12: el detalle del plano que se aplicó en un <c>end_plate</c> ("C", "I", "B", "I escalado"); nulo en los otros tipos.</summary>
        public string? DetailName { get; }

        /// <summary>
        /// Fase 12: el nombre con el que el plan de lote cuenta el arranque en vez de una plantilla: "arranque direct_weld",
        /// "arranque end_plate (Detalle C)", "arranque gusset_node".
        /// </summary>
        public string TemplateName => SpecStarter.StarterName(ConnectionType, DetailName);

        /// <summary>JSON con sangría, listo para la previsualización y <c>conn_validate</c>.</summary>
        public string SpecJson { get; }
        public ConnectionSpec Spec { get; }

        /// <summary>La plantilla de la que se tomaron los valores (decisión D10), o nula si fueron los de <c>choices.json</c>.</summary>
        public CatalogTemplate? DefaultsTemplate { get; }

        /// <summary>"valores de la plantilla Nudo tipico Detalle D" o "valores por defecto de choices.json".</summary>
        public string DefaultsSource { get; }

        /// <summary>[minX, minY, maxX, maxY] de la cartela (Fase 13: la caja del contorno automático calculado), en mm del marco canónico.</summary>
        public double[] GussetBoundsMm { get; }

        /// <summary>Fase 13: verdadero si la cartela del arranque es automática (<c>outline.mode = "auto"</c>).</summary>
        public bool IsAutoOutline => Geometry2D.AutoOutline.IsAuto(Spec);

        /// <summary>Texto de cabecera: qué es y de dónde salieron los valores.</summary>
        public string HeaderText
        {
            get
            {
                switch (ConnectionType)
                {
                    case "direct_weld":
                        return "Especificación de arranque de soldadura directa: cada barra cortada contra la cara del cordón y soldada en todo el contorno (" + DefaultsSource + "). Es un punto de partida: revisa la soldadura y la marca del empalme en el panel.";
                    case "end_plate":
                        return "Especificación de arranque de placa de extremo (empalme) con el Detalle " + (DetailName ?? "?") + " (" + DefaultsSource + "). Es un punto de partida: comprueba la plancha y los pernos por cara en el panel.";
                    default:
                        return IsAutoOutline
                            ? "Especificación de arranque con cartela automática (" + DefaultsSource + "): el contorno se calcula desde las barras y el cordón (outline.mode auto) y sigue sus ángulos. Es un punto de partida: revisa el panel (Contorno: Automático / Polígono) o pide el Encargo para IA."
                            : "Especificación de arranque con cartela rectangular (" + DefaultsSource + "). Es un punto de partida, no la cartela automática: rellena el panel o pide el Encargo para IA.";
                }
            }
        }
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

        /// <summary>Nombre del arranque para el plan: "arranque direct_weld", "arranque end_plate (Detalle C)".</summary>
        public static string StarterName(string connectionType, string? detailName) =>
            "arranque " + (connectionType ?? "gusset_node") + (string.IsNullOrWhiteSpace(detailName) ? "" : " (Detalle " + detailName + ")");

        /// <summary>
        /// Fase 12 (decisión D4): el tipo de arranque que le toca a un nudo leído del modelo: <c>end_plate</c> si es un empalme
        /// (dos tramos colineales); <c>direct_weld</c> si todas las barras están a un lado del cordón y el cordón atraviesa o el
        /// nudo está sobre un empalme (nudo en K, también con una sola barra); <c>gusset_node</c> (cartela rectangular) si hay
        /// barras a los dos lados. Nulo cuando no hay regla (una sola barra con el cordón terminando en el nudo sin empalme:
        /// falta el cordón en la selección).
        /// </summary>
        public static string? TypeFor(TemplateNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.IsSplice) return "end_plate";
            if (node.Members.Count == 0) return null;
            if (node.AllMembersOneSide)
            {
                return node.ChordContinuous || node.SpliceElementId.HasValue ? "direct_weld" : null;
            }
            return "gusset_node";
        }

        /// <summary>El arranque que le toca al nudo según <see cref="TypeFor"/>, o nulo si no hay regla.</summary>
        public static StarterResult? Choose(TemplateNode node, PanelChoices? choices, IReadOnlyList<CatalogTemplate>? templates = null)
        {
            string? type = TypeFor(node);
            return type == null ? null : ForType(type, node, choices, templates);
        }

        /// <summary>El arranque de un tipo concreto (el desplegable Tipo de Cambiar…). Lanza <see cref="CatalogException"/> si el tipo no existe o el nudo no sirve.</summary>
        public static StarterResult ForType(string connectionType, TemplateNode node, PanelChoices? choices, IReadOnlyList<CatalogTemplate>? templates = null)
        {
            switch ((connectionType ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "gusset_node": return FromNode(node, choices, templates);
                case "direct_weld": return ForKNode(node, choices, templates);
                case "end_plate": return ForSplice(node, choices, templates);
                default:
                    throw new CatalogException(ErrorCodes.InvalidRequest, "No hay arranque para el tipo '" + connectionType + "'.", "connection_type",
                        "Usa gusset_node, direct_weld o end_plate.");
            }
        }

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
            if (node.IsSplice)
            {
                throw new CatalogException(ErrorCodes.NodeAxesParallel, "Un empalme (dos tramos colineales) no lleva cartela: usa el arranque end_plate.", "element_ids",
                    "Elige placa de extremo en el desplegable Tipo.");
            }

            // 1. Valores por defecto: la plantilla más reciente con el mismo perfil de cordón, o choices.json.
            CatalogTemplate? template = FindDefaultsTemplate(node, templates);
            StarterValues values = template != null ? StarterValues.FromTemplate(template, defaults) : StarterValues.FromDefaults(defaults);
            string source = template != null ? "valores de la plantilla " + template.Name : "valores por defecto de choices.json";

            // 2. Barras: rol por la inclinación, perfil del modelo, soldada en ranura.
            var membersArray = new JsonArray();
            var ids = new JsonArray();
            foreach (long id in node.AllElementIds) ids.Add(id);
            var ends = new List<(double X, double Y)>();
            foreach (TemplateNodeMember member in node.Members)
            {
                double inclination = NodeFrame.AngleToChordDeg(member.AngleDeg);
                string role = inclination >= VerticalFromDeg ? "vertical" : "diagonal";
                var memberObject = new JsonObject
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
                };
                // Fase 12: el desvío del eje solo se escribe cuando lo hay (así los nudos concurrentes dan el mismo JSON que antes).
                if (Math.Abs(member.AxisOffsetMm) >= 0.05) memberObject["axis_offset_mm"] = Math.Round(member.AxisOffsetMm, 1);
                membersArray.Add(memberObject);

                // Extremo recortado de la barra: del retiro al final de la ranura más el margen, con el ancho de la barra, desde su propio corte.
                double half = (member.WidthMm > 0 ? member.WidthMm : SketchNodeInfo.DefaultMemberWidthMm) / 2.0;
                double near = values.EndSetbackMm;
                double far = values.EndSetbackMm + values.SlotLengthMm + defaults.GussetMarginMm;
                double vx = -member.Uy, vy = member.Ux;
                foreach (double along in new[] { near, far })
                {
                    ends.Add((member.AxisOffsetMm + along * member.Ux + half * vx, along * member.Uy + half * vy));
                    ends.Add((member.AxisOffsetMm + along * member.Ux - half * vx, along * member.Uy - half * vy));
                }
            }

            // 3. Cartela. Fase 13: automática (outline.mode auto, calculada por AutoOutline desde los extremos reales: envolvente
            //    convexa redondeada a 5 mm); con starter.outline_mode "polygon" en choices.json, el rectángulo de la Fase 11.
            double chordHalf = (node.ChordWidthMm > 0 ? node.ChordWidthMm : SketchNodeInfo.DefaultChordWidthMm) / 2.0;
            double minX, minY, maxX, maxY;
            JsonObject outlineObject;
            if (defaults.UsesAutoOutline)
            {
                var input = new Geometry2D.AutoOutlineInput
                {
                    ChordWidthMm = chordHalf * 2.0,
                    ChordInterface = values.ChordInterface,
                    MarginMm = defaults.GussetMarginMm,
                    SideMarginMm = defaults.GussetSideMarginMm,
                    BeyondChordMm = defaults.GussetBeyondChordMm,
                };
                foreach (TemplateNodeMember member in node.Members)
                {
                    input.Members.Add(new Geometry2D.AutoOutlineMember
                    {
                        ElementId = member.ElementId,
                        Ux = member.Ux,
                        Uy = member.Uy,
                        AxisOffsetMm = member.AxisOffsetMm,
                        WidthMm = member.WidthMm > 0 ? member.WidthMm : SketchNodeInfo.DefaultMemberWidthMm,
                        EndSetbackMm = values.EndSetbackMm,
                        IsKnifePlate = false,
                        SlotLengthMm = values.SlotLengthMm,
                    });
                }
                Geometry2D.AutoOutlineResult auto = Geometry2D.AutoOutline.Compute(input);
                minX = auto.MinX;
                minY = auto.MinY;
                maxX = auto.MaxX;
                maxY = auto.MaxY;
                outlineObject = new JsonObject { ["mode"] = "auto" };
                // Los márgenes solo se escriben cuando choices.json se aparta de los valores por defecto del Core: así el JSON (y
                // el token) de un nudo cualquiera no depende del archivo del PC, y los de siempre quedan limpios.
                if (Math.Abs(defaults.GussetMarginMm - Geometry2D.AutoOutline.DefaultMarginMm) > 1e-6) outlineObject["margin_mm"] = defaults.GussetMarginMm;
                if (Math.Abs(defaults.GussetSideMarginMm - Geometry2D.AutoOutline.DefaultSideMarginMm) > 1e-6) outlineObject["side_margin_mm"] = defaults.GussetSideMarginMm;
                if (Math.Abs(defaults.GussetBeyondChordMm - Geometry2D.AutoOutline.DefaultBeyondChordMm) > 1e-6) outlineObject["beyond_chord_mm"] = defaults.GussetBeyondChordMm;
            }
            else
            {
                double beyond = chordHalf + defaults.GussetBeyondChordMm;
                minX = Math.Min(ends.Min(p => p.X), -beyond);
                maxX = Math.Max(ends.Max(p => p.X), beyond);
                minY = Math.Min(ends.Min(p => p.Y), -beyond);
                maxY = Math.Max(ends.Max(p => p.Y), beyond);
                minX = Math.Floor(minX / RoundingMm) * RoundingMm;
                minY = Math.Floor(minY / RoundingMm) * RoundingMm;
                maxX = Math.Ceiling(maxX / RoundingMm) * RoundingMm;
                maxY = Math.Ceiling(maxY / RoundingMm) * RoundingMm;
                outlineObject = new JsonObject
                {
                    ["mode"] = "polygon",
                    ["points_mm"] = new JsonArray { new JsonArray { minX, minY }, new JsonArray { maxX, minY }, new JsonArray { maxX, maxY }, new JsonArray { minX, maxY } },
                };
            }

            var nodeObject = new JsonObject { ["element_ids"] = ids };
            if (node.EccentricityMm >= 0.05) nodeObject["eccentricity_mm"] = Math.Round(node.EccentricityMm, 1);
            var spec = new JsonObject
            {
                ["spec_version"] = "1.0",
                ["connection_type"] = "gusset_node",
                ["source"] = new JsonObject { ["drawing"] = DrawingName },
                ["node"] = nodeObject,
                ["chord"] = new JsonObject
                {
                    ["element_id"] = node.ChordElementId,
                    ["profile"] = string.IsNullOrWhiteSpace(node.ChordTypeName) ? null : node.ChordTypeName,
                    ["continuous"] = node.ChordContinuous,
                },
                ["gusset"] = new JsonObject
                {
                    ["thickness_mm"] = Math.Round(values.GussetThicknessMm, 3),
                    ["thickness_label"] = values.GussetThicknessLabel,
                    ["width_mm"] = Math.Round(maxX - minX, 3),
                    ["height_mm"] = Math.Round(maxY - minY, 3),
                    ["outline"] = outlineObject,
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

        /// <summary>
        /// Fase 12 (decisiones P3, D4, D5, D6 y D7): el arranque <c>direct_weld</c> de un nudo en K: cada barra cortada contra la
        /// cara del cordón (<c>end_cut: to_chord_face</c>, holgura de <c>choices.json</c>) y soldada en todo el contorno; el nudo
        /// lleva su excentricidad y cada barra su desvío; sobre un empalme, la barra que llega al otro tramo lo dice en
        /// <c>chord_element_id</c>; cada diagonal lleva la marca del empalme Detalle B a <c>diagonal_splice_offset_mm</c>. La
        /// soldadura sale de la plantilla <c>direct_weld</c> más reciente con el mismo perfil de cordón (D10) o de <c>choices.json</c>.
        /// </summary>
        public static StarterResult ForKNode(TemplateNode node, PanelChoices? choices, IReadOnlyList<CatalogTemplate>? templates = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            choices ??= PanelChoices.Default;
            DirectWeldDefaults d = choices.Starter?.DirectWeld ?? new DirectWeldDefaults();
            if (node.Members.Count == 0)
            {
                throw new CatalogException(ErrorCodes.NodeNeedsTwoMembers, "El nudo no tiene barras: no hay nada que soldar.", "element_ids",
                    "Selecciona el cordón y las barras que llegan al nudo.");
            }
            if (node.IsSplice)
            {
                throw new CatalogException(ErrorCodes.NodeAxesParallel, "Un empalme (dos tramos colineales) no es un nudo soldado: usa el arranque end_plate.", "element_ids",
                    "Elige placa de extremo en el desplegable Tipo.");
            }
            CatalogTemplate? template = FindDefaultsTemplate(node, templates, "direct_weld");
            double weldSize = d.WeldSizeMm > 0 ? d.WeldSizeMm : 5;
            bool allAround = d.WeldAllAround;
            double endGap = Math.Max(0.0, d.EndGapMm);
            string source = "valores por defecto de choices.json";
            if (template != null)
            {
                ConnectionSpec? templateSpec = null;
                try
                {
                    templateSpec = ConnectionSpec.FromJson(template.SpecTemplate!.ToJsonString());
                }
                catch (Exception)
                {
                    // Una plantilla ilegible no impide el arranque.
                }
                MemberSpec? first = templateSpec?.Members?.FirstOrDefault(m => m.Weld != null);
                if (first != null)
                {
                    if (first.Weld!.SizeMm is double size && size > 0) weldSize = size;
                    allAround = first.Weld.AllAround;
                    if (first.EndGapMm is double gap && gap >= 0) endGap = gap;
                    source = "valores de la plantilla " + template.Name;
                }
                else
                {
                    template = null;
                }
            }

            var ids = new JsonArray();
            foreach (long id in node.AllElementIds) ids.Add(id);
            var nodeObject = new JsonObject { ["element_ids"] = ids };
            if (node.EccentricityMm >= 0.05) nodeObject["eccentricity_mm"] = Math.Round(node.EccentricityMm, 1);

            var membersArray = new JsonArray();
            foreach (TemplateNodeMember member in node.Members)
            {
                double inclination = NodeFrame.AngleToChordDeg(member.AngleDeg);
                string role = inclination >= VerticalFromDeg ? "vertical" : "diagonal";
                var memberObject = new JsonObject
                {
                    ["element_id"] = member.ElementId,
                    ["role"] = role,
                    ["profile"] = string.IsNullOrWhiteSpace(member.TypeName) ? null : member.TypeName,
                    ["expected_angle_deg"] = Math.Round(inclination, 1),
                };
                if (Math.Abs(member.AxisOffsetMm) >= 0.05) memberObject["axis_offset_mm"] = Math.Round(member.AxisOffsetMm, 1);
                if (member.ChordElementId.HasValue && member.ChordElementId.Value > 0) memberObject["chord_element_id"] = member.ChordElementId.Value;
                memberObject["end_cut"] = "to_chord_face";
                memberObject["end_gap_mm"] = endGap;
                memberObject["weld"] = new JsonObject { ["type"] = "fillet", ["size_mm"] = weldSize, ["all_around"] = allAround };
                if (d.SpliceHint && role == "diagonal" && choices.DiagonalSpliceOffsetMm > 0)
                {
                    memberObject["splice_hint"] = new JsonObject
                    {
                        ["detail"] = string.IsNullOrWhiteSpace(d.SpliceDetail) ? "B" : d.SpliceDetail,
                        ["offset_from_chord_face_mm"] = choices.DiagonalSpliceOffsetMm,
                    };
                }
                membersArray.Add(memberObject);
            }

            var spec = new JsonObject
            {
                ["spec_version"] = "1.0",
                ["connection_type"] = "direct_weld",
                ["source"] = new JsonObject { ["drawing"] = DrawingName },
                ["node"] = nodeObject,
                ["chord"] = new JsonObject
                {
                    ["element_id"] = node.ChordElementId,
                    ["profile"] = string.IsNullOrWhiteSpace(node.ChordTypeName) ? null : node.ChordTypeName,
                    ["continuous"] = node.ChordContinuous,
                    ["face"] = "auto",
                },
                ["members"] = membersArray,
                ["dimension_chains"] = new JsonArray(),
                ["uncertain_fields"] = new JsonArray(),
            };
            string json = Pretty(spec);
            ConnectionSpec parsed = ConnectionSpec.FromJson(json) ?? throw new InvalidOperationException("La especificación de arranque quedó vacía.");
            return new StarterResult(json, parsed, template, source, new double[0]);
        }

        /// <summary>
        /// Fase 12 (decisiones D2, D3 y D8): el arranque <c>end_plate</c> de un empalme: el detalle del plano del perfil del
        /// cordón (C para HSS12X8X1/2, I para HSS3X3X1/4, B para HSS2-1/2X2-1/2X3/16) o el I escalado al tubo; dos planchas en
        /// contacto (<c>gap_mm</c> de <c>choices.json</c>); el retiro de cada tramo = espesor más media holgura menos el hueco
        /// que el tramo ya deja en el modelo (nunca negativo).
        /// </summary>
        public static StarterResult ForSplice(TemplateNode node, PanelChoices? choices, IReadOnlyList<CatalogTemplate>? templates = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            choices ??= PanelChoices.Default;
            EndPlateDefaults e = choices.Starter?.EndPlate ?? new EndPlateDefaults();
            if (!node.IsSplice || node.Members.Count != 1)
            {
                throw new CatalogException(ErrorCodes.SpliceNotCollinear, "La placa de extremo de esta fase empalma dos tramos colineales: el nudo leído no lo es.", "element_ids",
                    "Selecciona los dos tramos del cordón que se juntan (sin diagonales) o usa otro tipo.");
            }
            TemplateNodeMember other = node.Members[0];
            // Ronda 12c: a lo ancho de la plancha (perpendicular al plano de la cercha) va el lado perpendicular del tubo (DepthMm) y a
            // lo alto el lado en el plano (WidthMm), como en el validador y el croquis; ya no se adivina con el mayor y el menor.
            double tubeWidth = Math.Max(node.ChordDepthMm, other.DepthMm);
            double tubeHeight = Math.Max(node.ChordWidthMm, other.WidthMm);
            if (tubeWidth <= 0) tubeWidth = SketchNodeInfo.DefaultChordWidthMm;
            if (tubeHeight <= 0) tubeHeight = tubeWidth;
            EndPlateDetailDefaults? byProfile = e.Find(node.ChordTypeName);
            EndPlateDetailDefaults detail = byProfile ?? (e.Default ?? new EndPlateScaledDefaults()).Scale(tubeWidth, tubeHeight);
            string source = byProfile != null ? "Detalle " + detail.Detail + " del plano para " + node.ChordTypeName + " (choices.json)" : "Detalle I escalado al tubo (choices.json)";
            double gap = Math.Max(0.0, e.GapMm);
            double thickness = detail.ThicknessMm;
            double needed = thickness + gap / 2.0;
            // Redondeado hacia arriba a 0,1 mm: nunca por debajo de lo que la pila necesita (D8).
            double chordSetback = Math.Max(0.0, Math.Ceiling((needed - node.ChordEndGapMm) * 10.0 - 1e-6) / 10.0);
            double otherSetback = Math.Max(0.0, Math.Ceiling((needed - other.EndGapMm) * 10.0 - 1e-6) / 10.0);

            JsonObject Row(BoltRowDefaults? row)
            {
                row ??= new BoltRowDefaults { Count = 0 };
                var o = new JsonObject { ["count"] = Math.Max(0, row.Count), ["edge_mm"] = row.EdgeMm };
                if (row.Count > 1) o["spacing_mm"] = row.SpacingMm;
                return o;
            }
            BoltFacesDefaults faces = detail.Faces ?? new BoltFacesDefaults();
            var ids = new JsonArray { node.ChordElementId, other.ElementId };
            var spec = new JsonObject
            {
                ["spec_version"] = "1.0",
                ["connection_type"] = "end_plate",
                ["kind"] = "splice",
                ["source"] = new JsonObject { ["drawing"] = DrawingName + (byProfile != null ? " · Detalle " + detail.Detail : "") },
                ["node"] = new JsonObject { ["element_ids"] = ids },
                ["chord"] = new JsonObject
                {
                    ["element_id"] = node.ChordElementId,
                    ["profile"] = string.IsNullOrWhiteSpace(node.ChordTypeName) ? null : node.ChordTypeName,
                    ["continuous"] = false,
                    ["end_setback_mm"] = chordSetback,
                },
                ["members"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["element_id"] = other.ElementId,
                        ["role"] = "chord",
                        ["profile"] = string.IsNullOrWhiteSpace(other.TypeName) ? null : other.TypeName,
                        ["end_setback_mm"] = otherSetback,
                        ["expected_angle_deg"] = 0.0,
                    },
                },
                ["plate"] = new JsonObject
                {
                    ["thickness_mm"] = Math.Round(thickness, 3),
                    ["thickness_label"] = detail.ThicknessLabel,
                    ["width_mm"] = detail.WidthMm,
                    ["height_mm"] = detail.HeightMm,
                    ["corner_cut_mm"] = detail.CornerCutMm,
                    ["weld"] = new JsonObject { ["type"] = "fillet", ["size_mm"] = detail.WeldSizeMm > 0 ? detail.WeldSizeMm : 5, ["all_around"] = true },
                },
                ["bolts"] = new JsonObject
                {
                    ["diameter_mm"] = Math.Round(detail.DiameterMm, 3),
                    ["diameter_label"] = detail.DiameterLabel,
                    ["hole_mm"] = Math.Round(detail.HoleMm, 3),
                    ["hole_label"] = detail.HoleLabel,
                    ["nuts"] = "both_sides",
                    ["faces"] = new JsonObject
                    {
                        ["top"] = Row(faces.Top),
                        ["bottom"] = Row(faces.Bottom),
                        ["left"] = Row(faces.Left),
                        ["right"] = Row(faces.Right),
                    },
                },
                ["gap_mm"] = gap,
                ["dimension_chains"] = new JsonArray(),
                ["uncertain_fields"] = new JsonArray(),
            };
            string json = Pretty(spec);
            ConnectionSpec parsed = ConnectionSpec.FromJson(json) ?? throw new InvalidOperationException("La especificación de arranque quedó vacía.");
            return new StarterResult(json, parsed, null, source, new double[0], detail.Detail);
        }

        private static string Pretty(JsonObject spec) =>
            spec.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

        /// <summary>La plantilla más reciente cuyo cordón es del mismo perfil que el del nudo (D10), o nula. Fase 12: del tipo indicado (por defecto, cartela).</summary>
        public static CatalogTemplate? FindDefaultsTemplate(TemplateNode node, IReadOnlyList<CatalogTemplate>? templates, string connectionType = "gusset_node")
        {
            if (node == null || templates == null || templates.Count == 0) return null;
            return templates
                .Where(t => t?.SpecTemplate != null && string.Equals(t.ConnectionType ?? "gusset_node", connectionType, StringComparison.OrdinalIgnoreCase)
                            && SameChordProfile(t.ChordPattern?.Profile, node.ChordTypeName))
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
