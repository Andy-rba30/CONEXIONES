using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Editing
{
    /// <summary>Tipo de dato de un campo editable (decide cómo se interpreta el texto escrito).</summary>
    public enum SpecFieldKind
    {
        Number,
        Integer,
        Text,
        Boolean,
        /// <summary>Lista de números separados por punto y coma: "75; 420; 70".</summary>
        NumberList,
        /// <summary>Lista de puntos [x, y]; se edita en el cuadro de contorno, no en la tabla.</summary>
        Points,
        /// <summary>Valor libre (número, texto o booleano): <c>user_confirmed_value</c>.</summary>
        Any,
    }

    /// <summary>Una fila de la tabla editable: sección, etiqueta en español, ruta JSON y valor como texto.</summary>
    public sealed class SpecField
    {
        public SpecField(string section, string label, string path, string value, SpecFieldKind kind, bool isEditable = true, string? hint = null)
        {
            Section = section;
            Label = label;
            Path = path;
            Value = value;
            Kind = kind;
            IsEditable = isEditable;
            Hint = hint;
        }

        public string Section { get; }
        public string Label { get; }
        public string Path { get; }
        public string Value { get; }
        public SpecFieldKind Kind { get; }
        public bool IsEditable { get; }
        public string? Hint { get; }
    }

    /// <summary>
    /// Edita una especificación trabajando sobre su JSON (no sobre el objeto): así el texto que se valida, el que firma el
    /// <c>validation_token</c> y el que se guarda son el mismo, y los campos que el contrato no conoce se conservan. Las
    /// rutas son las mismas que usan los errores del validador (<c>members[2].attachment.bolts.spacing_mm</c>).
    /// Sin Revit ni WPF: se prueba con xUnit.
    /// </summary>
    public static class SpecEditor
    {
        private static readonly JsonDocumentOptions ParseOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private static readonly JsonSerializerOptions PrettyOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>Filas de la tabla (paso 8 de la guía): cordón, cartela, cada barra, cadenas de cotas y dudas.</summary>
        public static List<SpecField> ListFields(ConnectionSpec spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            var fields = new List<SpecField>();

            const string chord = "Cordón";
            fields.Add(new SpecField(chord, "ElementId", "chord.element_id", Format(spec.Chord?.ElementId), SpecFieldKind.Integer));
            fields.Add(new SpecField(chord, "Perfil", "chord.profile", spec.Chord?.Profile ?? "", SpecFieldKind.Text));
            fields.Add(new SpecField(chord, "Continuo", "chord.continuous", Format(spec.Chord?.Continuous), SpecFieldKind.Boolean));

            const string gusset = "Cartela";
            GussetSpec? g = spec.Gusset;
            fields.Add(new SpecField(gusset, "Espesor (mm)", "gusset.thickness_mm", Format(g?.ThicknessMm), SpecFieldKind.Number));
            fields.Add(new SpecField(gusset, "Rótulo de espesor", "gusset.thickness_label", g?.ThicknessLabel ?? "", SpecFieldKind.Text, true, "Debe coincidir con el espesor: 3/8\" = 9,525 mm"));
            fields.Add(new SpecField(gusset, "Ancho (mm)", "gusset.width_mm", Format(g?.WidthMm), SpecFieldKind.Number));
            fields.Add(new SpecField(gusset, "Alto (mm)", "gusset.height_mm", Format(g?.HeightMm), SpecFieldKind.Number));
            fields.Add(new SpecField(gusset, "Unión al cordón", "gusset.chord_interface", g?.ChordInterface ?? "", SpecFieldKind.Text, true, "through_slot, split_top_bottom o side_lap"));
            fields.Add(new SpecField(gusset, "Soldadura al cordón (mm)", "gusset.weld_to_chord.size_mm", Format(g?.WeldToChord?.SizeMm), SpecFieldKind.Number));
            fields.Add(new SpecField(gusset, "Soldadura todo el contorno", "gusset.weld_to_chord.all_around", Format(g?.WeldToChord?.AllAround), SpecFieldKind.Boolean));
            int pointCount = g?.Outline?.PointsMm?.Count ?? 0;
            fields.Add(new SpecField(gusset, "Contorno", "gusset.outline.points_mm", pointCount + " puntos (se edita en el cuadro de contorno)", SpecFieldKind.Points, false));

            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    MemberSpec m = spec.Members[i];
                    string section = "Barra " + (i + 1) + " · " + (m.Role ?? "barra") + " " + m.ElementId;
                    string p = "members[" + i + "]";
                    AttachmentSpec? a = m.Attachment;
                    bool knife = string.Equals(a?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase);

                    fields.Add(new SpecField(section, "ElementId", p + ".element_id", Format(m.ElementId), SpecFieldKind.Integer));
                    fields.Add(new SpecField(section, "Rol", p + ".role", m.Role ?? "", SpecFieldKind.Text, true, "diagonal o vertical"));
                    fields.Add(new SpecField(section, "Perfil", p + ".profile", m.Profile ?? "", SpecFieldKind.Text));
                    fields.Add(new SpecField(section, "Ángulo en el plano (°)", p + ".expected_angle_deg", Format(m.ExpectedAngleDeg), SpecFieldKind.Number, true, "Solo se compara con el modelo (aviso si difiere más de 1°)"));
                    fields.Add(new SpecField(section, "Retiro (mm)", p + ".end_setback_mm", Format(m.EndSetbackMm), SpecFieldKind.Number));
                    fields.Add(new SpecField(section, "Tipo de unión", p + ".attachment.type", a?.Type ?? "", SpecFieldKind.Text, true, "welded_slot o bolted_knife_plate"));

                    if (!knife)
                    {
                        fields.Add(new SpecField(section, "Largo de ranura (mm)", p + ".attachment.slot_length_mm", Format(a?.SlotLengthMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Soldadura (mm)", p + ".attachment.weld.size_mm", Format(a?.Weld?.SizeMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Soldadura todo el contorno", p + ".attachment.weld.all_around", Format(a?.Weld?.AllAround), SpecFieldKind.Boolean));
                    }
                    else
                    {
                        KnifePlateSpec? pl = a?.Plate;
                        BoltPatternSpec? b = a?.Bolts;
                        fields.Add(new SpecField(section, "Placa: espesor (mm)", p + ".attachment.plate.thickness_mm", Format(pl?.ThicknessMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Placa: rótulo", p + ".attachment.plate.thickness_label", pl?.ThicknessLabel ?? "", SpecFieldKind.Text, true, "PL10 = 10 mm"));
                        fields.Add(new SpecField(section, "Placa: largo (mm)", p + ".attachment.plate.length_mm", Format(pl?.LengthMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Placa: ancho (mm)", p + ".attachment.plate.width_mm", Format(pl?.WidthMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Placa: inserción en la barra (mm)", p + ".attachment.plate.insertion_mm", Format(pl?.InsertionMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Pernos: filas", p + ".attachment.bolts.rows", Format(b?.Rows), SpecFieldKind.Integer));
                        fields.Add(new SpecField(section, "Pernos: columnas", p + ".attachment.bolts.columns", Format(b?.Columns), SpecFieldKind.Integer));
                        fields.Add(new SpecField(section, "Pernos: paso (mm)", p + ".attachment.bolts.spacing_mm", Format(b?.SpacingMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Pernos: borde (mm)", p + ".attachment.bolts.edge_mm", Format(b?.EdgeMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Pernos: primera fila desde el extremo (mm)", p + ".attachment.bolts.first_row_from_plate_end_mm", Format(b?.FirstRowFromPlateEndMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Pernos: diámetro (mm)", p + ".attachment.bolts.diameter_mm", Format(b?.DiameterMm), SpecFieldKind.Number));
                        fields.Add(new SpecField(section, "Pernos: rótulo de diámetro", p + ".attachment.bolts.diameter_label", b?.DiameterLabel ?? "", SpecFieldKind.Text, true, "5/8\" = 15,875 mm"));
                        fields.Add(new SpecField(section, "Soldadura placa-barra (mm)", p + ".attachment.weld_plate_to_member.size_mm", Format(a?.WeldPlateToMember?.SizeMm), SpecFieldKind.Number));
                    }
                }
            }

            if (spec.DimensionChains != null)
            {
                const string chains = "Cadenas de cotas";
                for (int k = 0; k < spec.DimensionChains.Count; k++)
                {
                    DimensionChain c = spec.DimensionChains[k];
                    string label = c.Label ?? ("cadena " + (k + 1));
                    fields.Add(new SpecField(chains, label + ": valores (mm)", "dimension_chains[" + k + "].values_mm", FormatList(c.ValuesMm), SpecFieldKind.NumberList, true, "Separados por punto y coma"));
                    fields.Add(new SpecField(chains, label + ": total esperado (mm)", "dimension_chains[" + k + "].expected_total_mm", Format(c.ExpectedTotalMm), SpecFieldKind.Number));
                }
            }

            if (spec.UncertainFields != null)
            {
                const string doubts = "Dudas";
                for (int j = 0; j < spec.UncertainFields.Count; j++)
                {
                    UncertainField u = spec.UncertainFields[j];
                    fields.Add(new SpecField(doubts, u.Path, "uncertain_fields[" + j + "].user_confirmed_value", FormatAny(u.UserConfirmedValue), SpecFieldKind.Any, true, u.Reason));
                }
            }

            return fields;
        }

        /// <summary>
        /// Escribe <paramref name="text"/> en la ruta indicada y devuelve el JSON nuevo. El tipo del valor sale del valor
        /// que ya había (número, entero, booleano, texto, lista); si no había, se deduce del texto. Texto vacío = null.
        /// Los objetos intermedios que falten (por ejemplo <c>attachment.weld</c>) se crean; los índices de lista no.
        /// </summary>
        public static bool TrySetValue(string rawJson, string path, string text, out string newJson, out string error)
        {
            newJson = rawJson;
            error = string.Empty;
            JsonNode? root;
            try
            {
                root = JsonNode.Parse(rawJson ?? "", null, ParseOptions);
            }
            catch (JsonException ex)
            {
                error = "El JSON actual no se puede leer: " + ex.Message;
                return false;
            }
            if (root is not JsonObject)
            {
                error = "El JSON de la especificación debe ser un objeto.";
                return false;
            }

            if (!TryParsePath(path, out List<PathSegment> segments, out error)) return false;

            if (!TryNavigate(root, segments, createMissingObjects: true, out JsonNode? parent, out PathSegment leaf, out error)) return false;

            JsonNode? existing = GetChild(parent!, leaf);
            JsonNode? value = MakeValue(existing, text ?? "", leaf.Name, out error);
            if (error.Length > 0) return false;

            if (!TrySetChild(parent!, leaf, value, out error)) return false;
            newJson = root.ToJsonString(PrettyOptions);
            return true;
        }

        /// <summary>
        /// Rellena <c>node.element_ids</c> (y <c>chord.element_id</c> si no estaba) con la selección de Revit, sin tocar el
        /// resto del texto: es lo que hace el botón de la cinta cuando el JSON no trae los IDs del nudo.
        /// </summary>
        public static bool TrySetElementIds(string rawJson, IReadOnlyList<long> elementIds, out string newJson, out string error)
        {
            newJson = rawJson;
            error = string.Empty;
            if (elementIds == null || elementIds.Count < 2)
            {
                error = "Hacen falta al menos dos elementos seleccionados (cordón y una barra).";
                return false;
            }

            JsonNode? root;
            try
            {
                root = JsonNode.Parse(rawJson ?? "", null, ParseOptions);
            }
            catch (JsonException ex)
            {
                error = "El JSON actual no se puede leer: " + ex.Message;
                return false;
            }
            if (root is not JsonObject obj)
            {
                error = "El JSON de la especificación debe ser un objeto.";
                return false;
            }

            if (obj["node"] is not JsonObject node)
            {
                node = new JsonObject();
                obj["node"] = node;
            }
            var ids = new JsonArray();
            foreach (long id in elementIds) ids.Add(JsonValue.Create(id));
            node["element_ids"] = ids;

            if (obj["chord"] is not JsonObject chord)
            {
                chord = new JsonObject();
                obj["chord"] = chord;
            }
            Classify(chord["element_id"], out bool hasNumber, out _, out _, out _);
            bool chordMissing = !hasNumber || (chord["element_id"] is JsonValue v && v.TryGetValue<long>(out long current) && current <= 0)
                || (chord["element_id"] is JsonValue v2 && v2.TryGetValue<JsonElement>(out JsonElement el) && el.ValueKind == JsonValueKind.Number && el.TryGetInt64(out long cur2) && cur2 <= 0);
            if (chordMissing)
            {
                chord["element_id"] = JsonValue.Create(elementIds[0]);
                if (chord["continuous"] == null) chord["continuous"] = JsonValue.Create(true);
            }

            newJson = root.ToJsonString(PrettyOptions);
            return true;
        }

        /// <summary>Contorno de la cartela desde texto: un punto por línea, "x; y" (también vale "x, y" o "x y").</summary>
        public static bool TrySetOutline(string rawJson, string pointsText, out string newJson, out string error)
        {
            newJson = rawJson;
            if (!TryParsePoints(pointsText, out List<double[]> points, out error)) return false;
            if (points.Count < 3)
            {
                error = "El contorno necesita al menos 3 puntos (hay " + points.Count + ").";
                return false;
            }

            JsonNode? root;
            try
            {
                root = JsonNode.Parse(rawJson ?? "", null, ParseOptions);
            }
            catch (JsonException ex)
            {
                error = "El JSON actual no se puede leer: " + ex.Message;
                return false;
            }
            if (root is not JsonObject obj)
            {
                error = "El JSON de la especificación debe ser un objeto.";
                return false;
            }

            if (obj["gusset"] is not JsonObject gusset)
            {
                gusset = new JsonObject();
                obj["gusset"] = gusset;
            }
            if (gusset["outline"] is not JsonObject outline)
            {
                outline = new JsonObject();
                gusset["outline"] = outline;
            }
            outline["mode"] = "polygon";
            var array = new JsonArray();
            foreach (double[] point in points)
            {
                array.Add(new JsonArray(JsonValue.Create(point[0]), JsonValue.Create(point[1])));
            }
            outline["points_mm"] = array;
            newJson = root.ToJsonString(PrettyOptions);
            error = string.Empty;
            return true;
        }

        /// <summary>Contorno como texto editable: una línea por punto, "x; y".</summary>
        public static string FormatOutline(GussetOutline? outline)
        {
            if (outline?.PointsMm == null) return string.Empty;
            var sb = new StringBuilder();
            foreach (double[] point in outline.PointsMm)
            {
                if (point == null || point.Length < 2) continue;
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(Format(point[0])).Append("; ").Append(Format(point[1]));
            }
            return sb.ToString();
        }

        /// <summary>JSON con sangría y acentos legibles, para guardar el archivo corregido.</summary>
        public static string ToPrettyJson(string rawJson)
        {
            JsonNode? root = JsonNode.Parse(rawJson ?? "", null, ParseOptions);
            return root == null ? "null" : root.ToJsonString(PrettyOptions);
        }

        /// <summary>Número escrito por una persona: acepta coma o punto decimal.</summary>
        public static bool TryParseNumber(string? text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            string s = text!.Trim();
            if (s.IndexOf(',') >= 0 && s.IndexOf('.') < 0) s = s.Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParsePoints(string? text, out List<double[]> points, out string error)
        {
            points = new List<double[]>();
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(text)) return true;
            string[] lines = text!.Replace("\r", "").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim().Trim('[', ']', '(', ')');
                if (line.Length == 0) continue;
                string[] parts = SplitNumbers(line);
                if (parts.Length != 2 || !TryParseNumber(parts[0], out double x) || !TryParseNumber(parts[1], out double y))
                {
                    error = "Línea " + (i + 1) + " del contorno: se esperaba \"x; y\" y se leyó \"" + lines[i].Trim() + "\".";
                    return false;
                }
                points.Add(new[] { x, y });
            }
            return true;
        }

        // ---- interno ----

        private readonly struct PathSegment
        {
            public PathSegment(string name, int index)
            {
                Name = name;
                Index = index;
            }

            /// <summary>Nombre de propiedad; vacío si el segmento es solo un índice.</summary>
            public string Name { get; }

            /// <summary>Índice de lista o -1.</summary>
            public int Index { get; }

            public bool IsIndex => Index >= 0;
        }

        private static bool TryParsePath(string? path, out List<PathSegment> segments, out string error)
        {
            segments = new List<PathSegment>();
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "La ruta del campo está vacía.";
                return false;
            }

            foreach (string part in path!.Split('.'))
            {
                if (part.Length == 0)
                {
                    error = "La ruta '" + path + "' tiene un segmento vacío.";
                    return false;
                }
                int bracket = part.IndexOf('[');
                string name = bracket < 0 ? part : part.Substring(0, bracket);
                if (name.Length > 0) segments.Add(new PathSegment(name, -1));
                while (bracket >= 0)
                {
                    int close = part.IndexOf(']', bracket);
                    if (close < 0 || !int.TryParse(part.Substring(bracket + 1, close - bracket - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
                    {
                        error = "La ruta '" + path + "' tiene un índice mal escrito en '" + part + "'.";
                        return false;
                    }
                    segments.Add(new PathSegment(string.Empty, index));
                    bracket = part.IndexOf('[', close);
                }
            }
            if (segments.Count == 0)
            {
                error = "La ruta '" + path + "' no tiene campos.";
                return false;
            }
            return true;
        }

        private static bool TryNavigate(JsonNode root, List<PathSegment> segments, bool createMissingObjects, out JsonNode? parent, out PathSegment leaf, out string error)
        {
            error = string.Empty;
            parent = root;
            leaf = segments[segments.Count - 1];
            for (int i = 0; i < segments.Count - 1; i++)
            {
                PathSegment segment = segments[i];
                JsonNode? next = GetChild(parent!, segment);
                if (next == null)
                {
                    bool nextIsObject = !segments[i + 1].IsIndex;
                    if (createMissingObjects && nextIsObject && !segment.IsIndex && parent is JsonObject obj)
                    {
                        next = new JsonObject();
                        obj[segment.Name] = next;
                    }
                    else
                    {
                        error = "No existe '" + Describe(segments, i) + "' en la especificación.";
                        return false;
                    }
                }
                parent = next;
            }
            return true;
        }

        private static string Describe(List<PathSegment> segments, int upTo)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= upTo; i++)
            {
                if (segments[i].IsIndex) sb.Append('[').Append(segments[i].Index).Append(']');
                else
                {
                    if (sb.Length > 0) sb.Append('.');
                    sb.Append(segments[i].Name);
                }
            }
            return sb.ToString();
        }

        private static JsonNode? GetChild(JsonNode parent, PathSegment segment)
        {
            if (segment.IsIndex)
            {
                return parent is JsonArray array && segment.Index < array.Count ? array[segment.Index] : null;
            }
            return parent is JsonObject obj && obj.ContainsKey(segment.Name) ? obj[segment.Name] : null;
        }

        private static bool TrySetChild(JsonNode parent, PathSegment leaf, JsonNode? value, out string error)
        {
            error = string.Empty;
            if (leaf.IsIndex)
            {
                if (parent is not JsonArray array)
                {
                    error = "El campo no es una lista y la ruta termina en un índice.";
                    return false;
                }
                if (leaf.Index >= array.Count)
                {
                    error = "La lista tiene " + array.Count + " elementos; no existe el índice " + leaf.Index + ".";
                    return false;
                }
                array[leaf.Index] = value;
                return true;
            }
            if (parent is not JsonObject obj)
            {
                error = "La ruta apunta dentro de un valor que no es un objeto.";
                return false;
            }
            obj[leaf.Name] = value;
            return true;
        }

        private static JsonNode? MakeValue(JsonNode? existing, string text, string fieldName, out string error)
        {
            error = string.Empty;
            string trimmed = text.Trim();
            if (trimmed.Length == 0) return null;

            // Qué campos son enteros lo decide el contrato, no la forma del valor anterior: si una persona escribe "9" en
            // thickness_mm, el JSON guarda 9 y la siguiente edición ("12,7") debe seguir siendo válida.
            bool integerField = fieldName == "rows" || fieldName == "columns" || fieldName == "element_id" || fieldName == "element_ids" || fieldName == "schema_version";

            if (existing is JsonArray)
            {
                bool integers = integerField;
                var list = new JsonArray();
                foreach (string part in SplitNumbers(trimmed))
                {
                    if (!TryParseNumber(part, out double number))
                    {
                        error = "'" + part + "' no es un número (la lista va separada por punto y coma).";
                        return null;
                    }
                    list.Add(integers && Math.Abs(number - Math.Round(number)) < 1e-9 ? JsonValue.Create((long)Math.Round(number)) : JsonValue.Create(number));
                }
                return list;
            }

            Classify(existing, out bool isNumber, out _, out bool isBool, out bool isString);

            if (isBool || (!isNumber && !isString && IsBooleanText(trimmed)))
            {
                if (!TryParseBoolean(trimmed, out bool flag))
                {
                    error = "'" + trimmed + "' no es un valor sí/no (escribe true o false).";
                    return null;
                }
                return JsonValue.Create(flag);
            }

            if (isNumber || integerField || (!isString && TryParseNumber(trimmed, out _)))
            {
                if (!TryParseNumber(trimmed, out double number))
                {
                    error = "'" + trimmed + "' no es un número (usa coma o punto decimal, sin unidades).";
                    return null;
                }
                if (integerField)
                {
                    if (Math.Abs(number - Math.Round(number)) > 1e-9)
                    {
                        error = "'" + fieldName + "' debe ser un número entero.";
                        return null;
                    }
                    return JsonValue.Create((long)Math.Round(number));
                }
                return JsonValue.Create(number);
            }

            return JsonValue.Create(trimmed);
        }

        private static void Classify(JsonNode? node, out bool isNumber, out bool isInteger, out bool isBool, out bool isString)
        {
            isNumber = isInteger = isBool = isString = false;
            if (node is not JsonValue value) return;
            if (value.TryGetValue<JsonElement>(out JsonElement element))
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Number:
                        isNumber = true;
                        isInteger = element.TryGetInt64(out _) && element.GetRawText().IndexOf('.') < 0 && element.GetRawText().IndexOfAny(new[] { 'e', 'E' }) < 0;
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        isBool = true;
                        break;
                    case JsonValueKind.String:
                        isString = true;
                        break;
                }
                return;
            }
            if (value.TryGetValue<bool>(out _)) { isBool = true; return; }
            if (value.TryGetValue<long>(out _)) { isNumber = true; isInteger = true; return; }
            if (value.TryGetValue<int>(out _)) { isNumber = true; isInteger = true; return; }
            if (value.TryGetValue<double>(out _)) { isNumber = true; return; }
            if (value.TryGetValue<string>(out _)) { isString = true; }
        }

        private static bool IsBooleanText(string text)
        {
            return TryParseBoolean(text, out _);
        }

        private static bool TryParseBoolean(string text, out bool value)
        {
            switch (text.Trim().ToLowerInvariant())
            {
                case "true":
                case "sí":
                case "si":
                case "verdadero":
                    value = true;
                    return true;
                case "false":
                case "no":
                case "falso":
                    value = false;
                    return true;
                default:
                    value = false;
                    return false;
            }
        }

        private static string[] SplitNumbers(string text)
        {
            // Separadores admitidos: punto y coma, barra vertical, espacios y, si no hay coma decimal posible, la coma.
            char[] separators = text.IndexOf(';') >= 0 || text.IndexOf('|') >= 0
                ? new[] { ';', '|' }
                : text.IndexOf('.') >= 0 || text.IndexOf(',') < 0 ? new[] { ',', ' ', '\t' } : new[] { ' ', '\t' };
            return text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
        }

        private static string Format(double? value) => value.HasValue ? value.Value.ToString("0.###", CultureInfo.InvariantCulture) : "";
        private static string Format(long? value) => value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : "";
        private static string Format(int? value) => value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : "";
        private static string Format(bool? value) => value.HasValue ? (value.Value ? "true" : "false") : "";

        private static string FormatList(IEnumerable<double>? values)
        {
            return values == null ? "" : string.Join("; ", values.Select(v => Format(v)));
        }

        private static string FormatAny(object? value)
        {
            switch (value)
            {
                case null:
                    return "";
                case JsonElement element:
                    return element.ValueKind == JsonValueKind.String ? element.GetString() ?? "" : element.ValueKind == JsonValueKind.Null ? "" : element.GetRawText();
                case string s:
                    return s;
                case double d:
                    return Format(d);
                case bool b:
                    return Format(b);
                default:
                    return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
            }
        }
    }
}
