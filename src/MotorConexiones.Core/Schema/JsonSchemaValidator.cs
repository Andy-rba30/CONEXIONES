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
            "drawing", "scale", "template_id", "batch_id", "drawing_image"
        };

        private static readonly HashSet<string> AllowedNodeProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_ids", "eccentricity_mm"
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
            "element_id", "role", "profile", "end_setback_mm", "expected_angle_deg", "attachment", "axis_offset_mm"
        };
        // ---- Fase 12: direct_weld ----
        private static readonly HashSet<string> AllowedDirectWeldRootProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "spec_version", "connection_type", "source", "node", "chord", "members", "dimension_chains", "uncertain_fields"
        };
        private static readonly HashSet<string> AllowedDirectWeldChordProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_id", "profile", "continuous", "face"
        };
        private static readonly HashSet<string> AllowedDirectWeldMemberProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            // end_setback_mm se admite (nulo o informativo) para que una especificación reserializada por el add-in pase el esquema.
            "element_id", "role", "profile", "expected_angle_deg", "end_setback_mm", "axis_offset_mm", "chord_element_id", "end_cut", "end_gap_mm", "weld", "splice_hint"
        };
        private static readonly HashSet<string> AllowedSpliceHintProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "detail", "offset_from_chord_face_mm"
        };
        // ---- Fase 12: end_plate ----
        private static readonly HashSet<string> AllowedEndPlateRootProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "spec_version", "connection_type", "kind", "source", "node", "chord", "members", "plate", "bolts", "gap_mm", "dimension_chains", "uncertain_fields"
        };
        private static readonly HashSet<string> AllowedEndPlateChordProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_id", "profile", "continuous", "end_setback_mm"
        };
        private static readonly HashSet<string> AllowedEndPlateMemberProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "element_id", "role", "profile", "end_setback_mm", "expected_angle_deg"
        };
        private static readonly HashSet<string> AllowedEndPlatePlateProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "thickness_mm", "thickness_label", "width_mm", "height_mm", "corner_cut_mm", "weld"
        };
        private static readonly HashSet<string> AllowedRingBoltProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "diameter_mm", "diameter_label", "hole_mm", "hole_label", "nuts", "length_mm", "faces"
        };
        private static readonly HashSet<string> AllowedBoltFacesProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "top", "bottom", "left", "right"
        };
        private static readonly HashSet<string> AllowedBoltRowProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "count", "spacing_mm", "edge_mm"
        };
        /// <summary>Tipos de conexión que entiende el esquema (Fase 12: tres).</summary>
        public static readonly string[] KnownConnectionTypes = { "gusset_node", "direct_weld", "end_plate" };

        private static readonly HashSet<string> AllowedAttachmentProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "type", "slot_length_mm", "weld", "plate", "bolts", "weld_plate_to_member"
        };

        private static readonly HashSet<string> AllowedKnifePlateProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "thickness_mm", "thickness_label", "length_mm", "width_mm", "insertion_mm", "gusset_face"
        };

        private static readonly HashSet<string> AllowedBoltPatternProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "diameter_mm", "diameter_label", "rows", "columns", "spacing_mm", "edge_mm", "first_row_from_plate_end_mm", "length_mm"
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
        ""scale"": { ""type"": ""string"" },
        ""template_id"": { ""type"": [""string"", ""null""], ""description"": ""Plantilla del catálogo de la que salió esta especificación (conn_catalog_apply); la escribe el add-in."" },
        ""batch_id"": { ""type"": [""string"", ""null""], ""description"": ""Lote al que pertenece la conexión (reservado para conn_batch_create, Fase 9)."" },
        ""drawing_image"": { ""type"": [""string"", ""null""], ""description"": ""Fase 11: ruta del plano como imagen (PNG o JPG) que la previsualización abre al lado del croquis; opcional, la escribe la ventana."" }
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
        },
        ""eccentricity_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Fase 12: distancia a lo largo del cordón entre los cortes extremos de los ejes de las barras con el eje del cordón (nudo en K con excentricidad); informativa, la mide el add-in."" }
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
          ""axis_offset_mm"": { ""type"": [""number"", ""null""], ""description"": ""Fase 12: dónde corta el eje de la barra al del cordón, medido desde el punto de trabajo a lo largo de +X (con signo; 0 si concurre). Informativo: el validador lo mide del modelo y avisa MEMBER_ECCENTRIC hasta medio canto del cordón."" },
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
                  ""insertion_mm"": { ""type"": ""number"", ""minimum"": 0.0 },
                  ""gusset_face"": { ""type"": ""string"", ""enum"": [""+z"", ""-z""], ""description"": ""Cara de la cartela (sistema local del nudo) sobre la que apoya la placa cuchilla; por defecto +z. La placa se solapa con la cartela y los pernos atraviesan las dos."" }
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
                  ""first_row_from_plate_end_mm"": { ""type"": ""number"", ""minimum"": 0.0 },
                  ""length_mm"": { ""type"": ""number"", ""minimum"": 1.0, ""description"": ""Longitud del perno bajo cabeza si el plano la indica; si falta se calcula del agarre (cartela + placa) más el suplemento de limits.json."" }
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

                // Fase 12: cada tipo tiene su esquema; gusset_node sigue siendo el camino de siempre.
                string? connectionType = root.TryGetProperty("connection_type", out var typeElement) && typeElement.ValueKind == JsonValueKind.String ? typeElement.GetString() : null;
                if (connectionType == "direct_weld")
                {
                    ValidateDirectWeld(root, errors);
                    return errors;
                }
                if (connectionType == "end_plate")
                {
                    ValidateEndPlate(root, errors);
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
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "connection_type debe ser \"gusset_node\", \"direct_weld\" o \"end_plate\".", "connection_type", "Usa uno de los tipos de conn_list_types."));
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
                                        if (pl.TryGetProperty("gusset_face", out var face) && face.ValueKind != JsonValueKind.Null
                                            && (face.ValueKind != JsonValueKind.String || (face.GetString() != "+z" && face.GetString() != "-z")))
                                        {
                                            errors.Add(new ApiError(ErrorCodes.SchemaInvalid,
                                                "plate.gusset_face debe ser \"+z\" o \"-z\" (cara de la cartela sobre la que apoya la placa cuchilla).",
                                                $"{attPath}.plate.gusset_face", "Usa \"+z\" (por defecto) o \"-z\" según el sistema local del nudo; si el plano no lo dice, ponlo en uncertain_fields."));
                                        }
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

        // ------------------------------------------------------------------ Fase 12: direct_weld
        private static void ValidateDirectWeld(JsonElement root, List<ApiError> errors)
        {
            ValidateProperties(root, AllowedDirectWeldRootProperties, "", errors);
            foreach (var req in new[] { "spec_version", "connection_type", "node", "chord", "members" })
            {
                if (!root.TryGetProperty(req, out _)) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"Falta la propiedad obligatoria '{req}' en la raíz.", req, $"Incluye el campo '{req}'."));
            }
            CheckSpecVersion(root, errors);
            if (root.TryGetProperty("source", out var src) && src.ValueKind == JsonValueKind.Object) ValidateProperties(src, AllowedSourceProperties, "source", errors);
            ValidateNodeObject(root, errors, minimumIds: 2);
            if (root.TryGetProperty("chord", out var chord))
            {
                if (chord.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord debe ser un objeto.", "chord", "Define chord como objeto {...}."));
                else
                {
                    ValidateProperties(chord, AllowedDirectWeldChordProperties, "chord", errors);
                    if (!chord.TryGetProperty("element_id", out var chordId) || chordId.ValueKind != JsonValueKind.Number)
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord.element_id es obligatorio y numérico.", "chord.element_id", "Indica el element_id del cordón."));
                    if (chord.TryGetProperty("face", out var face) && face.ValueKind != JsonValueKind.Null && (face.ValueKind != JsonValueKind.String || (face.GetString() != "auto" && face.GetString() != "+y" && face.GetString() != "-y")))
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord.face debe ser \"auto\", \"+y\" o \"-y\".", "chord.face", "Usa \"auto\": la cara donde están las barras."));
                }
            }
            if (root.TryGetProperty("members", out var members))
            {
                if (members.ValueKind != JsonValueKind.Array) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members debe ser un arreglo.", "members", "Define members como arreglo [...]."));
                else
                {
                    int index = 0;
                    if (members.GetArrayLength() == 0) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members necesita al menos una barra.", "members", "Añade las diagonales que se sueldan al cordón."));
                    foreach (var m in members.EnumerateArray())
                    {
                        string path = $"members[{index}]";
                        if (m.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"{path} debe ser un objeto.", path, ""));
                        else
                        {
                            ValidateProperties(m, AllowedDirectWeldMemberProperties, path, errors);
                            foreach (var req in new[] { "element_id", "role", "weld" })
                            {
                                if (!m.TryGetProperty(req, out _)) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"Falta '{req}' en {path}.", path + "." + req, $"Incluye '{req}'."));
                            }
                            if (m.TryGetProperty("end_cut", out var cut) && cut.ValueKind != JsonValueKind.Null && (cut.ValueKind != JsonValueKind.String || cut.GetString() != "to_chord_face"))
                                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "end_cut solo admite \"to_chord_face\" en esta fase.", path + ".end_cut", "Usa \"to_chord_face\"."));
                            if (m.TryGetProperty("end_gap_mm", out var gap) && gap.ValueKind == JsonValueKind.Number && gap.GetDouble() < 0)
                                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "end_gap_mm no puede ser negativo.", path + ".end_gap_mm", "Usa 0 (sin holgura) o un valor positivo."));
                            if (m.TryGetProperty("weld", out var weld) && weld.ValueKind == JsonValueKind.Object) ValidateProperties(weld, AllowedWeldProperties, path + ".weld", errors);
                            if (m.TryGetProperty("splice_hint", out var hint) && hint.ValueKind == JsonValueKind.Object) ValidateProperties(hint, AllowedSpliceHintProperties, path + ".splice_hint", errors);
                        }
                        index++;
                    }
                }
            }
            ValidateChainsAndDoubts(root, errors);
        }
        // ------------------------------------------------------------------ Fase 12: end_plate
        private static void ValidateEndPlate(JsonElement root, List<ApiError> errors)
        {
            ValidateProperties(root, AllowedEndPlateRootProperties, "", errors);
            foreach (var req in new[] { "spec_version", "connection_type", "node", "chord", "members", "plate", "bolts" })
            {
                if (!root.TryGetProperty(req, out _)) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"Falta la propiedad obligatoria '{req}' en la raíz.", req, $"Incluye el campo '{req}'."));
            }
            CheckSpecVersion(root, errors);
            if (root.TryGetProperty("kind", out var kind) && kind.ValueKind != JsonValueKind.Null && (kind.ValueKind != JsonValueKind.String || kind.GetString() != "splice"))
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "kind solo admite \"splice\" en esta fase (to_support queda para otra).", "kind", "Usa \"splice\"."));
            if (root.TryGetProperty("source", out var src) && src.ValueKind == JsonValueKind.Object) ValidateProperties(src, AllowedSourceProperties, "source", errors);
            ValidateNodeObject(root, errors, minimumIds: 2);
            if (root.TryGetProperty("chord", out var chord))
            {
                if (chord.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord debe ser un objeto (el primer tramo del empalme).", "chord", "Define chord como objeto {...}."));
                else
                {
                    ValidateProperties(chord, AllowedEndPlateChordProperties, "chord", errors);
                    if (!chord.TryGetProperty("element_id", out var chordId) || chordId.ValueKind != JsonValueKind.Number)
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "chord.element_id es obligatorio y numérico.", "chord.element_id", "Indica el element_id del primer tramo."));
                }
            }
            if (root.TryGetProperty("members", out var members))
            {
                if (members.ValueKind != JsonValueKind.Array || members.GetArrayLength() != 1)
                    errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members debe tener exactamente una barra: el segundo tramo del empalme.", "members", "Pon el otro tramo en members[0] con role \"chord\"."));
                else
                {
                    var m = members[0];
                    if (m.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members[0] debe ser un objeto.", "members[0]", ""));
                    else
                    {
                        ValidateProperties(m, AllowedEndPlateMemberProperties, "members[0]", errors);
                        if (!m.TryGetProperty("element_id", out var id) || id.ValueKind != JsonValueKind.Number)
                            errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "members[0].element_id es obligatorio y numérico.", "members[0].element_id", "Indica el element_id del segundo tramo."));
                    }
                }
            }
            if (root.TryGetProperty("plate", out var plate))
            {
                if (plate.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "plate debe ser un objeto.", "plate", "Define plate con thickness_mm, width_mm y height_mm."));
                else
                {
                    ValidateProperties(plate, AllowedEndPlatePlateProperties, "plate", errors);
                    foreach (var req in new[] { "thickness_mm", "width_mm", "height_mm" })
                    {
                        if (!plate.TryGetProperty(req, out var v) || v.ValueKind != JsonValueKind.Number || v.GetDouble() <= 0)
                            errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"plate.{req} es obligatorio y positivo.", "plate." + req, "Lee la medida de la plancha en el plano."));
                    }
                    if (plate.TryGetProperty("corner_cut_mm", out var cc) && cc.ValueKind == JsonValueKind.Number && cc.GetDouble() < 0)
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "plate.corner_cut_mm no puede ser negativo.", "plate.corner_cut_mm", "0 = rectángulo."));
                    if (plate.TryGetProperty("weld", out var weld) && weld.ValueKind == JsonValueKind.Object) ValidateProperties(weld, AllowedWeldProperties, "plate.weld", errors);
                }
            }
            if (root.TryGetProperty("bolts", out var bolts))
            {
                if (bolts.ValueKind != JsonValueKind.Object) errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "bolts debe ser un objeto.", "bolts", "Define bolts con diameter_mm y faces."));
                else
                {
                    ValidateProperties(bolts, AllowedRingBoltProperties, "bolts", errors);
                    if (!bolts.TryGetProperty("diameter_mm", out var d) || d.ValueKind != JsonValueKind.Number || d.GetDouble() <= 0)
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "bolts.diameter_mm es obligatorio y positivo.", "bolts.diameter_mm", "Lee el diámetro del perno en el plano."));
                    if (bolts.TryGetProperty("nuts", out var nuts) && nuts.ValueKind != JsonValueKind.Null && (nuts.ValueKind != JsonValueKind.String || (nuts.GetString() != "both_sides" && nuts.GetString() != "one_side")))
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "bolts.nuts debe ser \"both_sides\" o \"one_side\".", "bolts.nuts", "Los Detalles B, C e I llevan tuerca a ambos lados."));
                    if (!bolts.TryGetProperty("faces", out var faces) || faces.ValueKind != JsonValueKind.Object)
                        errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "bolts.faces es obligatorio: las filas por cara (top, bottom, left, right).", "bolts.faces", "Pon count, spacing_mm y edge_mm por cara."));
                    else
                    {
                        ValidateProperties(faces, AllowedBoltFacesProperties, "bolts.faces", errors);
                        foreach (var name in AllowedBoltFacesProperties)
                        {
                            if (faces.TryGetProperty(name, out var row) && row.ValueKind == JsonValueKind.Object) ValidateProperties(row, AllowedBoltRowProperties, "bolts.faces." + name, errors);
                        }
                    }
                }
            }
            if (root.TryGetProperty("gap_mm", out var gap) && gap.ValueKind == JsonValueKind.Number && gap.GetDouble() < 0)
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "gap_mm no puede ser negativo.", "gap_mm", "0 = planchas en contacto."));
            ValidateChainsAndDoubts(root, errors);
        }
        private static void CheckSpecVersion(JsonElement root, List<ApiError> errors)
        {
            if (root.TryGetProperty("spec_version", out var specVer) && (specVer.ValueKind != JsonValueKind.String || specVer.GetString() != "1.0"))
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "spec_version debe ser \"1.0\".", "spec_version", "Usa \"1.0\"."));
        }
        private static void ValidateNodeObject(JsonElement root, List<ApiError> errors, int minimumIds)
        {
            if (!root.TryGetProperty("node", out var node)) return;
            if (node.ValueKind != JsonValueKind.Object)
            {
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "node debe ser un objeto.", "node", "Define node como objeto {...}."));
                return;
            }
            ValidateProperties(node, AllowedNodeProperties, "node", errors);
            if (!node.TryGetProperty("element_ids", out var elIds) || elIds.ValueKind != JsonValueKind.Array || elIds.GetArrayLength() < minimumIds)
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, $"node.element_ids debe ser un arreglo de al menos {minimumIds} enteros.", "node.element_ids", "Define element_ids como [id1, id2...]."));
            if (node.TryGetProperty("eccentricity_mm", out var ecc) && ecc.ValueKind == JsonValueKind.Number && ecc.GetDouble() < 0)
                errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "node.eccentricity_mm no puede ser negativo.", "node.eccentricity_mm", "Es una distancia: 0 o positiva."));
        }
        private static void ValidateChainsAndDoubts(JsonElement root, List<ApiError> errors)
        {
            if (root.TryGetProperty("dimension_chains", out var dChains) && dChains.ValueKind == JsonValueKind.Array)
            {
                int cIndex = 0;
                foreach (var c in dChains.EnumerateArray())
                {
                    if (c.ValueKind == JsonValueKind.Object) ValidateProperties(c, AllowedDimensionChainProperties, $"dimension_chains[{cIndex}]", errors);
                    cIndex++;
                }
            }
            if (root.TryGetProperty("uncertain_fields", out var uFields) && uFields.ValueKind == JsonValueKind.Array)
            {
                int uIndex = 0;
                foreach (var u in uFields.EnumerateArray())
                {
                    if (u.ValueKind == JsonValueKind.Object) ValidateProperties(u, AllowedUncertainFieldProperties, $"uncertain_fields[{uIndex}]", errors);
                    uIndex++;
                }
            }
        }
        /// <summary>Fase 12: JSON Schema (Draft-07) de <c>direct_weld</c>.</summary>
        public static string GetDirectWeldSchemaJson()
        {
            return @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""title"": ""DirectWeldConnectionSpec"",
  ""description"": ""Nudo en K soldado (Fase 12): cada diagonal se corta contra la cara del cordón y se suelda en todo el contorno. Sin cartela, sin placas, sin pernos. Admite la excentricidad del nudo (node.eccentricity_mm, members[].axis_offset_mm)."",
  ""type"": ""object"",
  ""required"": [""spec_version"", ""connection_type"", ""node"", ""chord"", ""members""],
  ""additionalProperties"": false,
  ""properties"": {
    ""spec_version"": { ""type"": ""string"", ""enum"": [""1.0""] },
    ""connection_type"": { ""type"": ""string"", ""enum"": [""direct_weld""] },
    ""source"": { ""type"": ""object"", ""additionalProperties"": false, ""properties"": { ""drawing"": { ""type"": ""string"" }, ""scale"": { ""type"": ""string"" }, ""template_id"": { ""type"": [""string"", ""null""] }, ""batch_id"": { ""type"": [""string"", ""null""] }, ""drawing_image"": { ""type"": [""string"", ""null""] } } },
    ""node"": {
      ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_ids""],
      ""properties"": {
        ""element_ids"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" }, ""minItems"": 2 },
        ""eccentricity_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Distancia a lo largo del cordón entre los cortes extremos de los ejes de las barras (34 mm en el cordón superior del Hangar); informativa."" }
      }
    },
    ""chord"": {
      ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_id"", ""continuous""],
      ""properties"": {
        ""element_id"": { ""type"": ""integer"" },
        ""profile"": { ""type"": [""string"", ""null""] },
        ""continuous"": { ""type"": ""boolean"", ""description"": ""false cuando el cordón está empalmado en el nudo (cada diagonal se suelda al tramo al que llega: members[].chord_element_id)."" },
        ""face"": { ""type"": [""string"", ""null""], ""enum"": [""auto"", ""+y"", ""-y"", null], ""description"": ""Cara del cordón donde llegan las barras; auto = el lado donde están."" }
      }
    },
    ""members"": {
      ""type"": ""array"", ""minItems"": 1,
      ""items"": {
        ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_id"", ""role"", ""weld""],
        ""properties"": {
          ""element_id"": { ""type"": ""integer"" },
          ""role"": { ""type"": ""string"", ""enum"": [""diagonal"", ""vertical""] },
          ""profile"": { ""type"": [""string"", ""null""] },
          ""expected_angle_deg"": { ""type"": ""number"" },
          ""axis_offset_mm"": { ""type"": [""number"", ""null""], ""description"": ""Dónde corta el eje de la barra al del cordón desde el punto de trabajo (con signo a lo largo de +X)."" },
          ""chord_element_id"": { ""type"": [""integer"", ""null""], ""description"": ""El tramo del cordón al que llega esta barra cuando no es chord.element_id (nudo en un empalme)."" },
          ""end_cut"": { ""type"": [""string"", ""null""], ""enum"": [""to_chord_face"", null] },
          ""end_gap_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Holgura entre el corte y la cara del cordón (0 por defecto)."" },
          ""weld"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""type"", ""size_mm""], ""properties"": { ""type"": { ""type"": ""string"", ""enum"": [""fillet""] }, ""size_mm"": { ""type"": ""number"", ""minimum"": 1.0 }, ""all_around"": { ""type"": ""boolean"" } } },
          ""splice_hint"": { ""type"": [""object"", ""null""], ""additionalProperties"": false, ""properties"": { ""detail"": { ""type"": [""string"", ""null""] }, ""offset_from_chord_face_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0 } }, ""description"": ""Marca del empalme de la diagonal (Detalle B) a esa distancia de la cara del cordón; solo se dibuja."" }
        }
      }
    },
    ""dimension_chains"": { ""type"": ""array"", ""items"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""values_mm"", ""expected_total_mm""], ""properties"": { ""label"": { ""type"": ""string"" }, ""values_mm"": { ""type"": ""array"", ""items"": { ""type"": ""number"" }, ""minItems"": 1 }, ""expected_total_mm"": { ""type"": ""number"", ""minimum"": 0.0 } } } },
    ""uncertain_fields"": { ""type"": ""array"", ""items"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""path"", ""reason""], ""properties"": { ""path"": { ""type"": ""string"" }, ""reason"": { ""type"": ""string"" }, ""user_confirmed_value"": {} } } }
  }
}";
        }
        /// <summary>Fase 12: JSON Schema (Draft-07) de <c>end_plate</c> (empalme con dos planchas cara a cara).</summary>
        public static string GetEndPlateSchemaJson()
        {
            return @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""title"": ""EndPlateConnectionSpec"",
  ""description"": ""Placa de extremo (Fase 12): dos planchas iguales, perpendiculares al eje, soldadas a cada tramo y unidas por pernos alrededor del tubo con tuerca a ambos lados. kind = splice: chord es el primer tramo y members[0] el segundo."",
  ""type"": ""object"",
  ""required"": [""spec_version"", ""connection_type"", ""node"", ""chord"", ""members"", ""plate"", ""bolts""],
  ""additionalProperties"": false,
  ""properties"": {
    ""spec_version"": { ""type"": ""string"", ""enum"": [""1.0""] },
    ""connection_type"": { ""type"": ""string"", ""enum"": [""end_plate""] },
    ""kind"": { ""type"": [""string"", ""null""], ""enum"": [""splice"", null] },
    ""source"": { ""type"": ""object"", ""additionalProperties"": false, ""properties"": { ""drawing"": { ""type"": ""string"" }, ""scale"": { ""type"": ""string"" }, ""template_id"": { ""type"": [""string"", ""null""] }, ""batch_id"": { ""type"": [""string"", ""null""] }, ""drawing_image"": { ""type"": [""string"", ""null""] } } },
    ""node"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_ids""], ""properties"": { ""element_ids"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" }, ""minItems"": 2 }, ""eccentricity_mm"": { ""type"": [""number"", ""null""] } } },
    ""chord"": {
      ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_id"", ""continuous""],
      ""properties"": { ""element_id"": { ""type"": ""integer"" }, ""profile"": { ""type"": [""string"", ""null""] }, ""continuous"": { ""type"": ""boolean"" }, ""end_setback_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Cuánto retrocede el primer tramo desde el punto de empalme (espesor de su plancha más media holgura)."" } }
    },
    ""members"": {
      ""type"": ""array"", ""minItems"": 1, ""maxItems"": 1,
      ""items"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""element_id"", ""role"", ""end_setback_mm""], ""properties"": { ""element_id"": { ""type"": ""integer"" }, ""role"": { ""type"": ""string"", ""enum"": [""chord""] }, ""profile"": { ""type"": [""string"", ""null""] }, ""end_setback_mm"": { ""type"": ""number"", ""minimum"": 0.0 }, ""expected_angle_deg"": { ""type"": ""number"" } } }
    },
    ""plate"": {
      ""type"": ""object"", ""additionalProperties"": false, ""required"": [""thickness_mm"", ""width_mm"", ""height_mm""],
      ""properties"": { ""thickness_mm"": { ""type"": ""number"", ""minimum"": 1.0 }, ""thickness_label"": { ""type"": ""string"" }, ""width_mm"": { ""type"": ""number"", ""minimum"": 10.0 }, ""height_mm"": { ""type"": ""number"", ""minimum"": 10.0 }, ""corner_cut_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Cateto de la esquina recortada a 45° (60 en los Detalles B, C e I); 0 = rectángulo."" }, ""weld"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""type"", ""size_mm""], ""properties"": { ""type"": { ""type"": ""string"", ""enum"": [""fillet""] }, ""size_mm"": { ""type"": ""number"", ""minimum"": 1.0 }, ""all_around"": { ""type"": ""boolean"" } } } }
    },
    ""bolts"": {
      ""type"": ""object"", ""additionalProperties"": false, ""required"": [""diameter_mm"", ""faces""],
      ""properties"": {
        ""diameter_mm"": { ""type"": ""number"", ""minimum"": 1.0 }, ""diameter_label"": { ""type"": ""string"" },
        ""hole_mm"": { ""type"": [""number"", ""null""], ""minimum"": 1.0 }, ""hole_label"": { ""type"": [""string"", ""null""] },
        ""nuts"": { ""type"": [""string"", ""null""], ""enum"": [""both_sides"", ""one_side"", null] },
        ""length_mm"": { ""type"": [""number"", ""null""], ""minimum"": 1.0, ""description"": ""Si falta, dos espesores + holgura + suplemento de limits.json."" },
        ""faces"": {
          ""type"": ""object"", ""additionalProperties"": false,
          ""properties"": {
            ""top"": { ""$ref"": ""#/definitions/row"" }, ""bottom"": { ""$ref"": ""#/definitions/row"" }, ""left"": { ""$ref"": ""#/definitions/row"" }, ""right"": { ""$ref"": ""#/definitions/row"" }
          }
        }
      }
    },
    ""gap_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0, ""description"": ""Holgura entre las dos planchas; 0 = en contacto."" },
    ""dimension_chains"": { ""type"": ""array"", ""items"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""values_mm"", ""expected_total_mm""], ""properties"": { ""label"": { ""type"": ""string"" }, ""values_mm"": { ""type"": ""array"", ""items"": { ""type"": ""number"" }, ""minItems"": 1 }, ""expected_total_mm"": { ""type"": ""number"", ""minimum"": 0.0 } } } },
    ""uncertain_fields"": { ""type"": ""array"", ""items"": { ""type"": ""object"", ""additionalProperties"": false, ""required"": [""path"", ""reason""], ""properties"": { ""path"": { ""type"": ""string"" }, ""reason"": { ""type"": ""string"" }, ""user_confirmed_value"": {} } } }
  },
  ""definitions"": {
    ""row"": { ""type"": [""object"", ""null""], ""additionalProperties"": false, ""properties"": { ""count"": { ""type"": ""integer"", ""minimum"": 0 }, ""spacing_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0 }, ""edge_mm"": { ""type"": [""number"", ""null""], ""minimum"": 0.0 } }, ""description"": ""Una fila de pernos en esa cara de la plancha: cuántos, a qué paso y a qué distancia del borde (25 en los Detalles B, C e I)."" }
  }
}";
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
