using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Schema
{
    /// <summary>
    /// Generador y validador de JSON Schema propio sin dependencias externas (solo System.Text.Json).
    /// Valida campos obligatorios, tipos, rangos, enumeraciones y rechaza propiedades no definidas (additionalProperties: false).
    /// </summary>
    public static class JsonSchemaValidator
    {
        private static readonly HashSet<string> AllowedRootProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "spec_version", "connection_type", "source", "node", "chord", "gusset", "members", "dimension_chains", "uncertain_fields"
        };

        private static readonly HashSet<string> AllowedSourceProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "drawing", "scale"
        };

        private static readonly HashSet<string> AllowedNodeProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_ids"
        };

        private static readonly HashSet<string> AllowedChordProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_id", "profile", "continuous"
        };

        private static readonly HashSet<string> AllowedGussetProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "thickness_mm", "thickness_label", "width_mm", "height_mm", "outline", "chord_interface", "weld_to_chord"
        };

        private static readonly HashSet<string> AllowedOutlineProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "mode", "points_mm"
        };

        private static readonly HashSet<string> AllowedWeldProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "type", "size_mm", "all_around"
        };

        private static readonly HashSet<string> AllowedMemberProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_id", "role", "profile", "end_setback_mm", "expected_angle_deg", "attachment"
        };

        private static readonly HashSet<string> AllowedAttachmentProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "type", "slot_length_mm", "weld", "plate", "bolts", "weld_plate_to_member"
        };

        private static readonly HashSet<string> AllowedKnifePlateProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "thickness_mm", "thickness_label", "length_mm", "width_mm", "insertion_mm"
        };

        private static readonly HashSet<string> AllowedBoltPatternProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "diameter_mm", "diameter_label", "rows", "columns", "spacing_mm", "edge_mm", "first_row_from_plate_end_mm"
        };

        private static readonly HashSet<string> AllowedDimensionChainProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "label", "values_mm", "expected_total_mm"
        };

        private static readonly HashSet<string> AllowedUncertainFieldProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "path", "reason", "user_confirmed_value"
        };

        /// <summary>
        /// Devuelve el JSON Schema oficial en formato Draft-07 para gusset_node.
        /// </summary>
        public static string GetGussetNodeSchemaJson()
        {
            return @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""title"": ""GussetNodeConnectionSpec"",
  ""type"": ""object"",
  ""required"": [""spec_version"", ""connection_type"", ""node"", ""chord"", ""gusset"", ""members""],
  ""additionalProperties"": false,
  ""properties"": {
    ""spec_version"": { ""type"": ""string"", ""enum"": [""1.0""] },
    ""connection_type"": { ""type"": ""string"", ""enum"": [""gusset_node""] },
    ""source"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""drawing"": { ""type"": ""string"" },
        ""scale"": { ""type"": ""string"" }
      }
    },
    ""node"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""required"": [""element_ids""],
      ""properties"": {
        ""element_ids"": {
          ""type"": ""array"",
          ""items"": { ""type"": ""integer"" },
          ""minItems"": 2
        }
      }
    },
    ""chord"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""required"": [""element_id"", ""continuous""],
      ""properties"": {
        ""element_id"": { ""type"": ""integer"" },
        ""profile"": { ""type"": [""string"", ""null""] },
        ""continuous"": { ""type"": ""boolean"" }
      }
    },
    ""gusset"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""required"": [""thickness_mm"", ""width_mm"", ""height_mm"", ""outline""],
      ""properties"": {
        ""thickness_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
        ""thickness_label"": { ""type"": ""string"" },
        ""width_mm"": { ""type"": ""number"", ""minimum"": 10.0 },
        ""height_mm"": { ""type"": ""number"", ""minimum"": 10.0 },
        ""outline"": {
          ""type"": ""object"",
          ""additionalProperties"": false,
          ""required"": [""mode"", ""points_mm""],
          ""properties"": {
            ""mode"": { ""type"": ""string"", ""enum"": [""polygon"", ""auto""] },
            ""points_mm"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""array"",
                ""items"": { ""type"": ""number"" },
                ""minItems"": 2,
                ""maxItems"": 2
              },
              ""minItems"": 3
            }
          }
        },
        ""chord_interface"": { ""type"": [""string"", ""null""], ""enum"": [""through_slot"", ""split_top_bottom"", ""side_lap"", null] },
        ""weld_to_chord"": {
          ""type"": ""object"",
          ""additionalProperties"": false,
          ""required"": [""type"", ""size_mm""],
          ""properties"": {
            ""type"": { ""type"": ""string"", ""enum"": [""fillet""] },
            ""size_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
            ""all_around"": { ""type"": ""boolean"" }
          }
        }
      }
    },
    ""members"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""element_id"", ""role"", ""end_setback_mm"", ""attachment""],
        ""properties"": {
          ""element_id"": { ""type"": ""integer"" },
          ""role"": { ""type"": ""string"", ""enum"": [""diagonal"", ""vertical"", ""chord""] },
          ""profile"": { ""type"": [""string"", ""null""] },
          ""end_setback_mm"": { ""type"": ""number"", ""minimum"": 0.0 },
          ""expected_angle_deg"": { ""type"": ""number"" },
          ""attachment"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""required"": [""type""],
            ""properties"": {
              ""type"": { ""type"": ""string"", ""enum"": [""welded_slot"", ""bolted_knife_plate""] },
              ""slot_length_mm"": { ""type"": ""number"", ""minimum"": 0.0 },
              ""weld"": {
                ""type"": ""object"",
                ""additionalProperties"": false,
                ""required"": [""type"", ""size_mm""],
                ""properties"": {
                  ""type"": { ""type"": ""string"", ""enum"": [""fillet""] },
                  ""size_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""all_around"": { ""type"": ""boolean"" }
                }
              },
              ""plate"": {
                ""type"": ""object"",
                ""additionalProperties"": false,
                ""required"": [""thickness_mm"", ""length_mm"", ""width_mm"", ""insertion_mm""],
                ""properties"": {
                  ""thickness_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""thickness_label"": { ""type"": ""string"" },
                  ""length_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""width_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""insertion_mm"": { ""type"": ""number"", ""minimum"": 0.0 }
                }
              },
              ""bolts"": {
                ""type"": ""object"",
                ""additionalProperties"": false,
                ""required"": [""diameter_mm"", ""rows"", ""columns"", ""spacing_mm"", ""edge_mm"", ""first_row_from_plate_end_mm""],
                ""properties"": {
                  ""diameter_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""diameter_label"": { ""type"": ""string"" },
                  ""rows"": { ""type"": ""integer"", ""minimum"": 1 },
                  ""columns"": { ""type"": ""integer"", ""minimum"": 1 },
                  ""spacing_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""edge_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""first_row_from_plate_end_mm"": { ""type"": ""number"", ""minimum"": 0.0 }
                }
              },
              ""weld_plate_to_member"": {
                ""type"": ""object"",
                ""additionalProperties"": false,
                ""required"": [""type"", ""size_mm""],
                ""properties"": {
                  ""type"": { ""type"": ""string"", ""enum"": [""fillet""] },
                  ""size_mm"": { ""type"": ""number"", ""minimum"": 1.0 },
                  ""all_around"": { ""type"": ""boolean"" }
                }
              }
            }
          }
        }
      }
    },
    ""dimension_chains"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""values_mm"", ""expected_total_mm""],
        ""properties"": {
          ""label"": { ""type"": ""string"" },
          ""values_mm"": {
            ""type"": ""array"",
            ""items"": { ""type"": ""number"" },
            ""minItems"": 1
          },
          ""expected_total_mm"": { ""type"": ""number"", ""minimum"": 0.0 }
        }
      }
    },
    ""uncertain_fields"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""path"", ""reason""],
        ""properties"": {
          ""path"": { ""type"": ""string"" },
          ""reason"": { ""type"": ""string"" },
          ""user_confirmed_value"": {}
        }
      }
    }
  }
}";
        }

        /// <summary>
        /// Valida la estructura del JSON contra el esquema propio y reporta errores SCHEMA_INVALID.
        /// </summary>
        public static List<ApiError> Validate(string? json)
        {
            var errors = new List<ApiError>();
            if (string.IsNullOrWhiteSpace(json))
            {
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "El JSON está vacío o nulo.", "", "Envía una especificación JSON válida."));
                return errors;
            }

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json!);
            }
            catch (JsonException ex)
            {
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"Error de sintaxis JSON: {ex.Message}", "", "Corrige la sintaxis JSON."));
                return errors;
            }

            using (doc)
            {
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La raíz del documento debe ser un objeto JSON.", "", "Envía un objeto JSON {...}."));
                    return errors;
                }

                // 1. Validar propiedades desconocidas en la raíz (additionalProperties: false)
                ValidateProperties(root, AllowedRootProperties, "", errors);

                // 2. Comprobar campos obligatorios de raíz
                string[] requiredRoot = { "spec_version", "connection_type", "node", "chord", "gusset", "members" };
                foreach (var req in requiredRoot)
                {
                    if (!root.TryGetProperty(req, out _))
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"Falta la propiedad obligatoria '{req}' en la raíz.", req, $"Incluye el campo '{req}'."));
                    }
                }

                // 3. spec_version
                if (root.TryGetProperty("spec_version", out var specVer))
                {
                    if (specVer.ValueKind != JsonValueKind.String || specVer.GetString() != "1.0")
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "spec_version debe ser \"1.0\".", "spec_version", "Usa \"1.0\"."));
                    }
                }

                // 4. connection_type
                if (root.TryGetProperty("connection_type", out var connType))
                {
                    if (connType.ValueKind != JsonValueKind.String || connType.GetString() != "gusset_node")
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "connection_type debe ser \"gusset_node\".", "connection_type", "Usa \"gusset_node\"."));
                    }
                }

                // 5. source
                if (root.TryGetProperty("source", out var src) && src.ValueKind == JsonValueKind.Object)
                {
                    ValidateProperties(src, AllowedSourceProperties, "source", errors);
                }

                // 6. node
                if (root.TryGetProperty("node", out var node))
                {
                    if (node.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "node debe ser un objeto.", "node", "Define node como objeto {...}."));
                    }
                    else
                    {
                        ValidateProperties(node, AllowedNodeProperties, "node", errors);
                        if (!node.TryGetProperty("element_ids", out var elIds) || elIds.ValueKind != JsonValueKind.Array)
                        {
                            errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "node.element_ids debe ser un arreglo de enteros.", "node.element_ids", "Define element_ids como [id1, id2...]."));
                        }
                    }
                }

                // 7. chord
                if (root.TryGetProperty("chord", out var chord))
                {
                    if (chord.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord debe ser un objeto.", "chord", "Define chord como objeto {...}."));
                    }
                    else
                    {
                        ValidateProperties(chord, AllowedChordProperties, "chord", errors);
                        if (!chord.TryGetProperty("element_id", out var chordId) || chordId.ValueKind != JsonValueKind.Number)
                        {
                            errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord.element_id es obligatorio y numérico.", "chord.element_id", "Indica el element_id del cordón."));
                        }
                    }
                }

                // 8. gusset
                if (root.TryGetProperty("gusset", out var gusset))
                {
                    if (gusset.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "gusset debe ser un objeto.", "gusset", "Define gusset como objeto {...}."));
                    }
                    else
                    {
                        ValidateProperties(gusset, AllowedGussetProperties, "gusset", errors);

                        if (gusset.TryGetProperty("outline", out var outline))
                        {
                            if (outline.ValueKind != JsonValueKind.Object)
                            {
                                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "gusset.outline debe ser un objeto.", "gusset.outline", "Define outline con mode y points_mm."));
                            }
                            else
                            {
                                ValidateProperties(outline, AllowedOutlineProperties, "gusset.outline", errors);
                                if (outline.TryGetProperty("mode", out var mode) && mode.GetString() != "polygon" && mode.GetString() != "auto")
                                {
                                    errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "outline.mode debe ser \"polygon\" o \"auto\".", "gusset.outline.mode", "Usa \"polygon\"."));
                                }
                            }
                        }

                        if (gusset.TryGetProperty("weld_to_chord", out var wChord) && wChord.ValueKind == JsonValueKind.Object)
                        {
                            ValidateProperties(wChord, AllowedWeldProperties, "gusset.weld_to_chord", errors);
                        }
                    }
                }

                // 9. members
                if (root.TryGetProperty("members", out var members))
                {
                    if (members.ValueKind != JsonValueKind.Array)
                    {
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members debe ser un arreglo.", "members", "Define members como arreglo [...]."));
                    }
                    else
                    {
                        int mIndex = 0;
                        foreach (var m in members.EnumerateArray())
                        {
                            string mPath = $"members[{mIndex}]";
                            if (m.ValueKind != JsonValueKind.Object)
                            {
                                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"{mPath} debe ser un objeto.", mPath, ""));
                            }
                            else
                            {
                                ValidateProperties(m, AllowedMemberProperties, mPath, errors);

                                if (m.TryGetProperty("attachment", out var att) && att.ValueKind == JsonValueKind.Object)
                                {
                                    string attPath = $"{mPath}.attachment";
                                    ValidateProperties(att, AllowedAttachmentProperties, attPath, errors);

                                    if (att.TryGetProperty("weld", out var w) && w.ValueKind == JsonValueKind.Object)
                                    {
                                        ValidateProperties(w, AllowedWeldProperties, $"{attPath}.weld", errors);
                                    }
                                    if (att.TryGetProperty("plate", out var pl) && pl.ValueKind == JsonValueKind.Object)
                                    {
                                        ValidateProperties(pl, AllowedKnifePlateProperties, $"{attPath}.plate", errors);
                                    }
                                    if (att.TryGetProperty("bolts", out var b) && b.ValueKind == JsonValueKind.Object)
                                    {
                                        ValidateProperties(b, AllowedBoltPatternProperties, $"{attPath}.bolts", errors);
                                    }
                                    if (att.TryGetProperty("weld_plate_to_member", out var wpm) && wpm.ValueKind == JsonValueKind.Object)
                                    {
                                        ValidateProperties(wpm, AllowedWeldProperties, $"{attPath}.weld_plate_to_member", errors);
                                    }
                                }
                            }
                            mIndex++;
                        }
                    }
                }

                // 10. dimension_chains
                if (root.TryGetProperty("dimension_chains", out var dChains) && dChains.ValueKind == JsonValueKind.Array)
                {
                    int cIndex = 0;
                    foreach (var c in dChains.EnumerateArray())
                    {
                        string cPath = $"dimension_chains[{cIndex}]";
                        if (c.ValueKind == JsonValueKind.Object)
                        {
                            ValidateProperties(c, AllowedDimensionChainProperties, cPath, errors);
                        }
                        cIndex++;
                    }
                }

                // 11. uncertain_fields
                if (root.TryGetProperty("uncertain_fields", out var uFields) && uFields.ValueKind == JsonValueKind.Array)
                {
                    int uIndex = 0;
                    foreach (var u in uFields.EnumerateArray())
                    {
                        string uPath = $"uncertain_fields[{uIndex}]";
                        if (u.ValueKind == JsonValueKind.Object)
                        {
                            ValidateProperties(u, AllowedUncertainFieldProperties, uPath, errors);
                        }
                        uIndex++;
                    }
                }
            }

            return errors;
        }

        private static void ValidateProperties(JsonElement element, HashSet<string> allowedProps, string basePath, List<ApiError> errors)
        {
            if (element.ValueKind != JsonValueKind.Object) return;

            foreach (var prop in element.EnumerateObject())
            {
                if (!allowedProps.Contains(prop.Name))
                {
                    string fullPath = string.IsNullOrEmpty(basePath) ? prop.Name : $"{basePath}.{prop.Name}";
                    errors.Add(new ApiError(
                        ErrorCodes.SchemaInvalid,
                        $"Propiedad no permitida o desconocida '{prop.Name}'.",
                        fullPath,
                        $"Elimina o corrige la clave '{prop.Name}'. Propiedades permitidas: {string.Join(", ", allowedProps.OrderBy(x => x))}"));
                }
            }
        }
    }
}
