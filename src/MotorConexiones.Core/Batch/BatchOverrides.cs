using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>
    /// Correcciones manuales del plan (sección 3.4 de la propuesta): las mismas para el MCP (<c>overrides</c> de
    /// <c>conn_batch_plan</c>) y para la ventana de la cinta. Se acumulan en el plan y se aplican en cada replanificación.
    /// Los nombres de nudo (<c>N1</c>…) son los del plan anterior; como la detección es determinista para la misma
    /// selección, se mantienen entre llamadas.
    /// </summary>
    public sealed class BatchOverrides
    {
        /// <summary>Nudos que no se tocan.</summary>
        [JsonPropertyName("exclude")]
        public List<string> Exclude { get; set; } = new List<string>();

        /// <summary>Nudos añadidos a mano: nombre → IDs de sus barras (cordón incluido).</summary>
        [JsonPropertyName("add_node")]
        public Dictionary<string, List<long>> AddNode { get; set; } = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Cordón de un nudo: nombre → ID de la barra.</summary>
        [JsonPropertyName("chord")]
        public Dictionary<string, long> Chord { get; set; } = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Plantilla de un nudo: nombre → <c>template_id</c>; nulo = sin plantilla (no se le aplica nada).</summary>
        [JsonPropertyName("template")]
        public Dictionary<string, string?> Template { get; set; } = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Barras que no son del nudo: nombre → IDs.</summary>
        [JsonPropertyName("remove_member")]
        public Dictionary<string, List<long>> RemoveMember { get; set; } = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Barras que faltaban en el nudo: nombre → IDs.</summary>
        [JsonPropertyName("add_member")]
        public Dictionary<string, List<long>> AddMember { get; set; } = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Grupos de nudos que son el mismo nudo: cada lista se funde en el primero.</summary>
        [JsonPropertyName("merge")]
        public List<List<string>> Merge { get; set; } = new List<List<string>>();

        /// <summary>Nudo que son varios: nombre → grupos de IDs (el primero conserva el nombre, los demás N5-2, N5-3…).</summary>
        [JsonPropertyName("split")]
        public Dictionary<string, List<List<long>>> Split { get; set; } = new Dictionary<string, List<List<long>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Especificación editada a mano para un nudo (sustituye a la instancia de la plantilla solo ahí).</summary>
        [JsonPropertyName("spec")]
        public Dictionary<string, JsonObject> Spec { get; set; } = new Dictionary<string, JsonObject>(StringComparer.OrdinalIgnoreCase);

        /// <summary>P8: los nudos que ya tienen conexión se saltan (<c>false</c>) o se planifican para rehacerlos (<c>true</c>).</summary>
        [JsonPropertyName("replace_existing")]
        public bool ReplaceExisting { get; set; }

        /// <summary>Sin ninguna corrección. No se serializa (cierre de la Fase 8: se colaba como clave <c>IsEmpty</c> en el <c>overrides</c> de la respuesta y, devuelto en una petición, daba <c>INVALID_REQUEST</c>).</summary>
        [JsonIgnore]
        public bool IsEmpty =>
            Exclude.Count == 0 && AddNode.Count == 0 && Chord.Count == 0 && Template.Count == 0 && RemoveMember.Count == 0
            && AddMember.Count == 0 && Merge.Count == 0 && Split.Count == 0 && Spec.Count == 0 && !ReplaceExisting;

        /// <summary>
        /// Lee las correcciones de un objeto JSON (<c>overrides</c> de la petición). Claves desconocidas → <c>INVALID_REQUEST</c>.
        /// Además de las claves anteriores admite <c>include</c> (quita nudos de <c>exclude</c>) y, en <c>chord</c>, un
        /// valor nulo o 0 para olvidar la corrección.
        /// </summary>
        public static BatchOverrides FromJson(JsonElement element)
        {
            var result = new BatchOverrides();
            if (element.ValueKind == JsonValueKind.Undefined || element.ValueKind == JsonValueKind.Null) return result;
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "overrides debe ser un objeto JSON.", "overrides",
                    "Ejemplo: {\"exclude\": [\"N3\"], \"chord\": {\"N4\": 1249510}}.");
            }
            foreach (JsonProperty property in element.EnumerateObject())
            {
                string key = property.Name.ToLowerInvariant();
                JsonElement value = property.Value;
                switch (key)
                {
                    case "exclude":
                        result.Exclude = Names(value, key);
                        break;
                    case "include":
                        result.Include = Names(value, key);
                        break;
                    case "add_node":
                        result.AddNode = IdsByName(value, key);
                        break;
                    case "chord":
                        foreach (JsonProperty p in Object(value, key).EnumerateObject())
                        {
                            if (p.Value.ValueKind == JsonValueKind.Number && p.Value.TryGetInt64(out long id)) result.Chord[p.Name] = id;
                            else if (p.Value.ValueKind == JsonValueKind.Null) result.Chord[p.Name] = 0;
                            else throw Bad(key + "." + p.Name, "un ID de barra (número)");
                        }
                        break;
                    case "template":
                        foreach (JsonProperty p in Object(value, key).EnumerateObject())
                        {
                            if (p.Value.ValueKind == JsonValueKind.String) result.Template[p.Name] = p.Value.GetString();
                            else if (p.Value.ValueKind == JsonValueKind.Null) result.Template[p.Name] = null;
                            else throw Bad(key + "." + p.Name, "un template_id (texto) o null");
                        }
                        break;
                    case "remove_member":
                        result.RemoveMember = IdsByName(value, key);
                        break;
                    case "add_member":
                        result.AddMember = IdsByName(value, key);
                        break;
                    case "merge":
                        if (value.ValueKind != JsonValueKind.Array) throw Bad(key, "una lista de listas de nombres");
                        foreach (JsonElement group in value.EnumerateArray()) result.Merge.Add(Names(group, key));
                        break;
                    case "split":
                        foreach (JsonProperty p in Object(value, key).EnumerateObject())
                        {
                            if (p.Value.ValueKind != JsonValueKind.Array) throw Bad(key + "." + p.Name, "una lista de listas de IDs");
                            result.Split[p.Name] = p.Value.EnumerateArray().Select(g => Ids(g, key + "." + p.Name)).ToList();
                        }
                        break;
                    case "spec":
                        foreach (JsonProperty p in Object(value, key).EnumerateObject())
                        {
                            if (p.Value.ValueKind == JsonValueKind.Null) continue;
                            if (p.Value.ValueKind != JsonValueKind.Object) throw Bad(key + "." + p.Name, "la especificación como objeto JSON");
                            result.Spec[p.Name] = TemplateJson.ParseObject(p.Value.GetRawText(), "overrides.spec." + p.Name);
                        }
                        break;
                    case "replace_existing":
                        if (value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False) throw Bad(key, "true o false");
                        result.ReplaceExisting = value.ValueKind == JsonValueKind.True;
                        break;
                    default:
                        throw new CatalogException(ErrorCodes.InvalidRequest, "overrides." + property.Name + " no es una corrección conocida.", "overrides." + property.Name,
                            "Claves admitidas: exclude, include, add_node, chord, template, remove_member, add_member, merge, split, spec, replace_existing.");
                }
            }
            return result;
        }

        /// <summary>Nudos que vuelven a incluirse (solo al fusionar correcciones; no se guarda).</summary>
        [JsonIgnore]
        public List<string> Include { get; set; } = new List<string>();

        /// <summary>Acumula otras correcciones sobre estas (las nuevas mandan por clave) y devuelve el resultado.</summary>
        public BatchOverrides MergeWith(BatchOverrides? other)
        {
            if (other == null) return Clone();
            BatchOverrides merged = Clone();
            foreach (string name in other.Exclude) if (!merged.Exclude.Contains(name, StringComparer.OrdinalIgnoreCase)) merged.Exclude.Add(name);
            foreach (string name in other.Include) merged.Exclude.RemoveAll(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
            foreach (var pair in other.AddNode) merged.AddNode[pair.Key] = pair.Value.ToList();
            foreach (var pair in other.Chord)
            {
                if (pair.Value == 0) merged.Chord.Remove(pair.Key);
                else merged.Chord[pair.Key] = pair.Value;
            }
            foreach (var pair in other.Template) merged.Template[pair.Key] = pair.Value;
            foreach (var pair in other.RemoveMember) merged.RemoveMember[pair.Key] = Union(merged.RemoveMember, pair.Key, pair.Value);
            foreach (var pair in other.AddMember) merged.AddMember[pair.Key] = Union(merged.AddMember, pair.Key, pair.Value);
            // Una barra añadida deja de estar quitada y al revés.
            foreach (var pair in other.AddMember) if (merged.RemoveMember.TryGetValue(pair.Key, out var removed)) removed.RemoveAll(pair.Value.Contains);
            foreach (var pair in other.RemoveMember) if (merged.AddMember.TryGetValue(pair.Key, out var added)) added.RemoveAll(pair.Value.Contains);
            foreach (List<string> group in other.Merge) if (group.Count > 1) merged.Merge.Add(group.ToList());
            foreach (var pair in other.Split) merged.Split[pair.Key] = pair.Value.Select(g => g.ToList()).ToList();
            foreach (var pair in other.Spec) merged.Spec[pair.Key] = TemplateJson.Clone(pair.Value);
            if (other.ReplaceExisting) merged.ReplaceExisting = true;
            return merged;
        }

        /// <summary>Olvida la especificación editada de un nudo (vuelve a la instancia de la plantilla).</summary>
        public void ClearSpec(string name) => Spec.Remove(name);

        public BatchOverrides Clone()
        {
            var copy = new BatchOverrides
            {
                Exclude = Exclude.ToList(),
                Include = Include.ToList(),
                Merge = Merge.Select(g => g.ToList()).ToList(),
                ReplaceExisting = ReplaceExisting,
            };
            foreach (var pair in AddNode) copy.AddNode[pair.Key] = pair.Value.ToList();
            foreach (var pair in Chord) copy.Chord[pair.Key] = pair.Value;
            foreach (var pair in Template) copy.Template[pair.Key] = pair.Value;
            foreach (var pair in RemoveMember) copy.RemoveMember[pair.Key] = pair.Value.ToList();
            foreach (var pair in AddMember) copy.AddMember[pair.Key] = pair.Value.ToList();
            foreach (var pair in Split) copy.Split[pair.Key] = pair.Value.Select(g => g.ToList()).ToList();
            foreach (var pair in Spec) copy.Spec[pair.Key] = TemplateJson.Clone(pair.Value);
            return copy;
        }

        /// <summary>IDs que las correcciones mencionan y que pueden no estar en la selección (hay que leerlos del modelo).</summary>
        public IEnumerable<long> ReferencedElementIds()
        {
            foreach (var pair in AddNode) foreach (long id in pair.Value) yield return id;
            foreach (var pair in AddMember) foreach (long id in pair.Value) yield return id;
            foreach (var pair in Chord) if (pair.Value != 0) yield return pair.Value;
            foreach (var pair in Split) foreach (var group in pair.Value) foreach (long id in group) yield return id;
        }

        public string ToJson() => JsonSerializer.Serialize(this, SerializerOptions);

        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };

        private static List<long> Union(Dictionary<string, List<long>> map, string key, List<long> more)
        {
            List<long> list = map.TryGetValue(key, out var existing) ? existing.ToList() : new List<long>();
            foreach (long id in more) if (!list.Contains(id)) list.Add(id);
            return list;
        }

        private static CatalogException Bad(string path, string expected) =>
            new CatalogException(ErrorCodes.InvalidRequest, "overrides." + path + " debe ser " + expected + ".", "overrides." + path);

        private static JsonElement Object(JsonElement value, string key)
        {
            if (value.ValueKind != JsonValueKind.Object) throw Bad(key, "un objeto {nombre de nudo: valor}");
            return value;
        }

        private static List<string> Names(JsonElement value, string key)
        {
            if (value.ValueKind != JsonValueKind.Array) throw Bad(key, "una lista de nombres de nudo (\"N1\", \"N2\"...)");
            var names = new List<string>();
            foreach (JsonElement item in value.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.String) throw Bad(key, "una lista de nombres de nudo (\"N1\", \"N2\"...)");
                string name = (item.GetString() ?? "").Trim();
                if (name.Length > 0) names.Add(name);
            }
            return names;
        }

        private static List<long> Ids(JsonElement value, string key)
        {
            if (value.ValueKind != JsonValueKind.Array) throw Bad(key, "una lista de IDs de barra (números)");
            var ids = new List<long>();
            foreach (JsonElement item in value.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Number || !item.TryGetInt64(out long id)) throw Bad(key, "una lista de IDs de barra (números)");
                if (!ids.Contains(id)) ids.Add(id);
            }
            return ids;
        }

        private static Dictionary<string, List<long>> IdsByName(JsonElement value, string key)
        {
            var map = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
            foreach (JsonProperty p in Object(value, key).EnumerateObject()) map[p.Name] = Ids(p.Value, key + "." + p.Name);
            return map;
        }
    }
}
