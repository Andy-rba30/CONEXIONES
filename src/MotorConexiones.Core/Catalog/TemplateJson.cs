using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Utilidades JSON del catálogo: rutas <c>a.b[0].c</c> sobre <see cref="JsonNode"/>, copias y serialización.</summary>
    internal static class TemplateJson
    {
        private static readonly JsonDocumentOptions ParseOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private static readonly JsonNodeOptions NodeOptions = new JsonNodeOptions { PropertyNameCaseInsensitive = false };

        private static readonly JsonSerializerOptions PrettyOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public static JsonObject ParseObject(string json, string what)
        {
            JsonNode? node;
            try
            {
                node = JsonNode.Parse(json, NodeOptions, ParseOptions);
            }
            catch (JsonException error)
            {
                throw new CatalogException(Validation.ErrorCodes.TemplateSpecInvalid, what + " no es JSON válido: " + error.Message, "spec", "Corrige la sintaxis JSON.");
            }
            if (node is not JsonObject obj)
            {
                throw new CatalogException(Validation.ErrorCodes.TemplateSpecInvalid, what + " debe ser un objeto JSON.", "spec", "Envía un objeto {...}.");
            }
            return obj;
        }

        public static JsonObject Clone(JsonObject source) => (JsonObject)JsonNode.Parse(source.ToJsonString(), NodeOptions, ParseOptions)!;

        public static string Pretty(JsonNode node) => node.ToJsonString(PrettyOptions);

        /// <summary>Convierte un valor leído del contrato (<see cref="JsonElement"/>, número, texto...) en un nodo.</summary>
        public static JsonNode? ToNode(object? value)
        {
            if (value == null) return null;
            if (value is JsonElement element)
            {
                return element.ValueKind == JsonValueKind.Null ? null : JsonNode.Parse(element.GetRawText(), NodeOptions, ParseOptions);
            }
            return JsonSerializer.SerializeToNode(value);
        }

        /// <summary>Devuelve el nodo de una ruta o nulo si no existe (sin crear nada).</summary>
        public static JsonNode? Get(JsonNode root, string path)
        {
            if (!TryParsePath(path, out List<PathSegment> segments)) return null;
            JsonNode? current = root;
            foreach (PathSegment segment in segments)
            {
                current = Child(current, segment);
                if (current == null) return null;
            }
            return current;
        }

        /// <summary>
        /// Escribe un valor en una ruta creando los objetos intermedios que falten (no los índices de lista).
        /// Devuelve falso si la ruta no se puede recorrer.
        /// </summary>
        public static bool TrySet(JsonObject root, string path, JsonNode? value)
        {
            if (!TryParsePath(path, out List<PathSegment> segments)) return false;
            JsonNode? parent = root;
            for (int i = 0; i < segments.Count - 1; i++)
            {
                PathSegment segment = segments[i];
                JsonNode? next = Child(parent, segment);
                if (next == null)
                {
                    if (segment.IsIndex || segments[i + 1].IsIndex || parent is not JsonObject obj) return false;
                    next = new JsonObject();
                    obj[segment.Name] = next;
                }
                parent = next;
            }
            PathSegment leaf = segments[segments.Count - 1];
            if (leaf.IsIndex)
            {
                if (parent is not JsonArray array || leaf.Index >= array.Count) return false;
                array[leaf.Index] = value;
                return true;
            }
            if (parent is not JsonObject target) return false;
            target[leaf.Name] = value;
            return true;
        }

        private static JsonNode? Child(JsonNode? parent, PathSegment segment)
        {
            if (parent == null) return null;
            if (segment.IsIndex)
            {
                return parent is JsonArray array && segment.Index < array.Count ? array[segment.Index] : null;
            }
            return parent is JsonObject obj && obj.TryGetPropertyValue(segment.Name, out JsonNode? child) ? child : null;
        }

        private readonly struct PathSegment
        {
            public PathSegment(string name, int index)
            {
                Name = name;
                Index = index;
            }

            public string Name { get; }
            public int Index { get; }
            public bool IsIndex => Index >= 0;
        }

        private static bool TryParsePath(string? path, out List<PathSegment> segments)
        {
            segments = new List<PathSegment>();
            if (string.IsNullOrWhiteSpace(path)) return false;
            foreach (string part in path!.Split('.'))
            {
                if (part.Length == 0) return false;
                int bracket = part.IndexOf('[');
                string name = bracket < 0 ? part : part.Substring(0, bracket);
                if (name.Length > 0) segments.Add(new PathSegment(name, -1));
                while (bracket >= 0)
                {
                    int close = part.IndexOf(']', bracket);
                    if (close < 0 || !int.TryParse(part.Substring(bracket + 1, close - bracket - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out int index)) return false;
                    segments.Add(new PathSegment(string.Empty, index));
                    bracket = part.IndexOf('[', close);
                }
            }
            return segments.Count > 0;
        }
    }
}
