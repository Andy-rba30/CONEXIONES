# Resultados de la Fase 12

Fecha: 2026-10-21T11:38:41


## 12-1 git

```text
43658ba Fase 12: informe, instalaci├│n con los sondeos 22, 23 y 24, resumen, README, gu├¡a, CLAUDE.md y propuestas al d├¡a (add-in 0.12.0)
?? docs/fases/resultados-fase-12.md

```

## 12-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos est├ín actualizados para la restauraci├│n.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.76
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:   268, Omitido:     0, Total:   268, Duraci├│n: 502 ms - MotorConexiones.Tests.dll (net10.0)

```

## 12-1 revit cerrado

```text

```

## 12-1 deploy

```text
== MotorConexiones 0.12.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 12-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 12-1 version de la dll

```text
0.12.0.0

```

## 12-1 limits.json desplegado

```text

  "_comentario_excentricidad": "Fase 12 (opciÃ³n A): hasta node_axis_max_distance_mm la barra concurre; hasta 
node_axis_max_offset_mm su eje puede cortar el cordÃ³n lejos del punto de trabajo con el aviso MEMBER_ECCENTRIC (nudo 
en K con excentricidad); por encima, MEMBER_NOT_AT_NODE. 0 = medio canto del cordÃ³n leÃdo del modelo (152,4 mm en un 
HSS12X8); un nÃºmero fija el lÃmite en mm.",
  "node_axis_max_offset_mm": 0,
  "_comentario_placa_extremo": "Fase 12: mÃnimos propios de la placa de extremo (end_plate). min_bolt_to_tube_mm es la 
distancia mÃnima entre la cara del tubo y el eje de cada perno (media tuerca mÃ¡s arandela); min_hole_clearance_mm, 
cuÃ¡nto mayor que el perno debe ser el agujero (Ã˜18 para 5/8\", Ã˜13/16\" para 3/4\").",
    "min_bolt_to_tube_mm": 20.0,
    "min_hole_clearance_mm": 1.0



```

## 12-1 choices.json desplegado

```text

  "_comentario_fase12": "Fase 12: hole_labels son los agujeros de las placas de extremo (Ã˜18 para 5/8\", Ã˜13/16\" 
para 3/4\"); diagonal_splice_offset_mm es la distancia de la cara del cordÃ³n a las planchas del empalme de la 
diagonal (Detalle B), medida a lo largo de la diagonal (decisiones P6 y D6: 330; se dibuja y se escribe en 
splice_hint, no se fabrica en esta fase).",
  "hole_labels": ["Ã˜18", "Ã˜13/16\"", "Ã˜15/16\"", "Ã˜21"],
  "diagonal_splice_offset_mm": 330,
    "_comentario_direct_weld": "Fase 12 (decisiÃ³n D7): el arranque del nudo en K soldado: filete de 5 mm en todo el 
contorno, sin holgura entre el corte y la cara; splice_hint true = cada diagonal lleva la marca del empalme Detalle B 
a diagonal_splice_offset_mm.",
    "direct_weld": { "weld_size_mm": 5, "weld_all_around": true, "end_gap_mm": 0, "splice_hint": true, 
"splice_detail": "B" },
    "_comentario_end_plate": "Fase 12 (decisiones D2, D3 y D8): el arranque del empalme con placa de extremo. 
by_profile: el detalle del plano por perfil del cordÃ³n (C para HSS12X8X1/2, I para HSS3X3X1/4, B para 
HSS2-1/2X2-1/2X3/16); las claves se comparan con la regla de perfiles del validador. default: el Detalle I escalado 
para cualquier otro perfil (esquina 60, borde 25, un perno por cara a canto + 2 Ã— bolt_to_tube_mm, plancha redondeada 
a round_mm). gap_mm: holgura entre las dos planchas (0 = en contacto). El retiro de cada tramo lo calcula Conectar: 
espesor mÃ¡s media holgura menos el hueco que el tramo ya deja en el modelo.",
      "by_profile": {



```

## 12-2 ping

```text
== conn/ping -> HTTP 200 en 316 ms ==
{
    "data":  {
                 "addin_version":  "0.12.0",
                 "operations":  [
                                    "batch_create",
                                    "batch_delete",
                                    "batch_plan",
                                    "batch_plan_discard",
                                    "batch_plan_get",
                                    "catalog_apply",
                                    "catalog_delete",
                                    "catalog_get",
                                    "catalog_list",
                                    "catalog_save",
                                    "create",
                                    "delete",
                                    "find_profile",
                                    "get",
                                    "guide",
                                    "list",
                                    "node_info",
                                    "ping",
                                    "preview",
                                    "schema",
                                    "types",
                                    "update",
                                    "validate"
                                ],
                 "document":  {
                                  "is_workshared":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false
                              },
                 "dotnet":  {
                                "load_context":  "Default",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12"
                            },
                 "revit":  {
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "language":  "English_USA"
                           },
                 "has_uidocument":  true,
                 "backend":  "advancesteel",
                 "spec_version":  "1.0"
             },
    "meta":  {
                 "duration_ms":  8,
                 "operation":  "ping",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-2 types

```text
== conn/types -> HTTP 200 en 54 ms ==
{
    "data":  {
                 "connection_types":  [
                                          {
                                              "type_name":  "direct_weld",
                                              "description":  "Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cordón y se suelda en todo el contorno, sin cartela ni placas (los nudos del cordón superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cordón."
                                          },
                                          {
                                              "type_name":  "end_plate",
                                              "description":  "Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones)."
                                          },
                                          {
                                              "type_name":  "gusset_node",
                                              "description":  "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."
                                          }
                                      ]
             },
    "meta":  {
                 "duration_ms":  2,
                 "operation":  "types",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-2 schema end_plate

```text
== conn/schema -> HTTP 200 en 38 ms ==
{
    "data":  {
                 "json_schema":  {
                                     "required":  [
                                                      "spec_version",
                                                      "connection_type",
                                                      "node",
                                                      "chord",
                                                      "members",
                                                      "plate",
                                                      "bolts"
                                                  ],
                                     "additionalProperties":  false,
                                     "title":  "EndPlateConnectionSpec",
                                     "type":  "object",
                                     "definitions":  {
                                                         "row":  {
                                                                     "description":  "Una fila de pernos en esa cara de la plancha: cuántos, a qué paso y a qué distancia del borde (25 en los Detalles B, C e I).",
                                                                     "type":  [
                                                                                  "object",
                                                                                  "null"
                                                                              ],
                                                                     "additionalProperties":  false,
                                                                     "properties":  {
                                                                                        "count":  {
                                                                                                      "type":  "integer",
                                                                                                      "minimum":  0
                                                                                                  },
                                                                                        "edge_mm":  {
                                                                                                        "type":  [
                                                                                                                     "number",
                                                                                                                     "null"
                                                                                                                 ],
                                                                                                        "minimum":  0.0
                                                                                                    },
                                                                                        "spacing_mm":  {
                                                                                                           "type":  [
                                                                                                                        "number",
                                                                                                                        "null"
                                                                                                                    ],
                                                                                                           "minimum":  0.0
                                                                                                       }
                                                                                    }
                                                                 }
                                                     },
                                     "properties":  {
                                                        "members":  {
                                                                        "type":  "array",
                                                                        "minItems":  1,
                                                                        "items":  {
                                                                                      "type":  "object",
                                                                                      "additionalProperties":  false,
                                                                                      "required":  [
                                                                                                       "element_id",
                                                                                                       "role",
                                                                                                       "end_setback_mm"
                                                                                                   ],
                                                                                      "properties":  {
                                                                                                         "profile":  {
                                                                                                                         "type":  [
                                                                                                                                      "string",
                                                                                                                                      "null"
                                                                                                                                  ]
                                                                                                                     },
                                                                                                         "role":  {
                                                                                                                      "type":  "string",
                                                                                                                      "enum":  [
                                                                                                                                   "chord"
                                                                                                                               ]
                                                                                                                  },
                                                                                                         "element_id":  {
                                                                                                                            "type":  "integer"
                                                                                                                        },
                                                                                                         "expected_angle_deg":  {
                                                                                                                                    "type":  "number"
                                                                                                                                },
                                                                                                         "end_setback_mm":  {
                                                                                                                                "type":  "number",
                                                                                                                                "minimum":  0.0
                                                                                                                            }
                                                                                                     }
                                                                                  },
                                                                        "maxItems":  1
                                                                    },
                                                        "source":  {
                                                                       "type":  "object",
                                                                       "additionalProperties":  false,
                                                                       "properties":  {
                                                                                          "scale":  {
                                                                                                        "type":  "string"
                                                                                                    },
                                                                                          "drawing_image":  {
                                                                                                                "type":  [
                                                                                                                             "string",
                                                                                                                             "null"
                                                                                                                         ]
                                                                                                            },
                                                                                          "drawing":  {
                                                                                                          "type":  "string"
                                                                                                      },
                                                                                          "batch_id":  {
                                                                                                           "type":  [
                                                                                                                        "string",
                                                                                                                        "null"
                                                                                                                    ]
                                                                                                       },
                                                                                          "template_id":  {
                                                                                                              "type":  [
                                                                                                                           "string",
                                                                                                                           "null"
                                                                                                                       ]
                                                                                                          }
                                                                                      }
                                                                   },
                                                        "connection_type":  {
                                                                                "type":  "string",
                                                                                "enum":  [
                                                                                             "end_plate"
                                                                                         ]
                                                                            },
                                                        "node":  {
                                                                     "type":  "object",
                                                                     "additionalProperties":  false,
                                                                     "required":  [
                                                                                      "element_ids"
                                                                                  ],
                                                                     "properties":  {
                                                                                        "element_ids":  {
                                                                                                            "type":  "array",
                                                                                                            "items":  {
                                                                                                                          "type":  "integer"
                                                                                                                      },
                                                                                                            "minItems":  2
                                                                                                        },
                                                                                        "eccentricity_mm":  {
                                                                                                                "type":  [
                                                                                                                             "number",
                                                                                                                             "null"
                                                                                                                         ]
                                                                                                            }
                                                                                    }
                                                                 },
                                                        "chord":  {
                                                                      "type":  "object",
                                                                      "additionalProperties":  false,
                                                                      "required":  [
                                                                                       "element_id",
                                                                                       "continuous"
                                                                                   ],
                                                                      "properties":  {
                                                                                         "end_setback_mm":  {
                                                                                                                "description":  "Cuánto retrocede el primer tramo desde el punto de empalme (espesor de su plancha más media holgura).",
                                                                                                                "type":  [
                                                                                                                             "number",
                                                                                                                             "null"
                                                                                                                         ],
                                                                                                                "minimum":  0.0
                                                                                                            },
                                                                                         "element_id":  {
                                                                                                            "type":  "integer"
                                                                                                        },
                                                                                         "profile":  {
                                                                                                         "type":  [
                                                                                                                      "string",
                                                                                                                      "null"
                                                                                                                  ]
                                                                                                     },
                                                                                         "continuous":  {
                                                                                                            "type":  "boolean"
                                                                                                        }
                                                                                     }
                                                                  },
                                                        "plate":  {
                                                                      "type":  "object",
                                                                      "additionalProperties":  false,
                                                                      "required":  [
                                                                                       "thickness_mm",
                                                                                       "width_mm",
                                                                                       "height_mm"
                                                                                   ],
                                                                      "properties":  {
                                                                                         "width_mm":  {
                                                                                                          "type":  "number",
                                                                                                          "minimum":  10.0
                                                                                                      },
                                                                                         "thickness_label":  {
                                                                                                                 "type":  "string"
                                                                                                             },
                                                                                         "weld":  {
                                                                                                      "type":  "object",
                                                                                                      "additionalProperties":  false,
                                                                                                      "required":  [
                                                                                                                       "type",
                                                                                                                       "size_mm"
                                                                                                                   ],
                                                                                                      "properties":  {
                                                                                                                         "type":  {
                                                                                                                                      "type":  "string",
                                                                                                                                      "enum":  [
                                                                                                                                                   "fillet"
                                                                                                                                               ]
                                                                                                                                  },
                                                                                                                         "size_mm":  {
                                                                                                                                         "type":  "number",
                                                                                                                                         "minimum":  1.0
                                                                                                                                     },
                                                                                                                         "all_around":  {
                                                                                                                                            "type":  "boolean"
                                                                                                                                        }
                                                                                                                     }
                                                                                                  },
                                                                                         "corner_cut_mm":  {
                                                                                                               "description":  "Cateto de la esquina recortada a 45° (60 en los Detalles B, C e I); 0 = rectángulo.",
                                                                                                               "type":  [
                                                                                                                            "number",
                                                                                                                            "null"
                                                                                                                        ],
                                                                                                               "minimum":  0.0
                                                                                                           },
                                                                                         "height_mm":  {
                                                                                                           "type":  "number",
                                                                                                           "minimum":  10.0
                                                                                                       },
                                                                                         "thickness_mm":  {
                                                                                                              "type":  "number",
                                                                                                              "minimum":  1.0
                                                                                                          }
                                                                                     }
                                                                  },
                                                        "gap_mm":  {
                                                                       "description":  "Holgura entre las dos planchas; 0 = en contacto.",
                                                                       "type":  [
                                                                                    "number",
                                                                                    "null"
                                                                                ],
                                                                       "minimum":  0.0
                                                                   },
                                                        "spec_version":  {
                                                                             "type":  "string",
                                                                             "enum":  [
                                                                                          "1.0"
                                                                                      ]
                                                                         },
                                                        "kind":  {
                                                                     "type":  [
                                                                                  "string",
                                                                                  "null"
                                                                              ],
                                                                     "enum":  [
                                                                                  "splice",
                                                                                  null
                                                                              ]
                                                                 },
                                                        "bolts":  {
                                                                      "type":  "object",
                                                                      "additionalProperties":  false,
                                                                      "required":  [
                                                                                       "diameter_mm",
                                                                                       "faces"
                                                                                   ],
                                                                      "properties":  {
                                                                                         "hole_label":  {
                                                                                                            "type":  [
                                                                                                                         "string",
                                                                                                                         "null"
                                                                                                                     ]
                                                                                                        },
                                                                                         "diameter_label":  {
                                                                                                                "type":  "string"
                                                                                                            },
                                                                                         "length_mm":  {
                                                                                                           "description":  "Si falta, dos espesores + holgura + suplemento de limits.json.",
                                                                                                           "type":  [
                                                                                                                        "number",
                                                                                                                        "null"
                                                                                                                    ],
                                                                                                           "minimum":  1.0
                                                                                                       },
                                                                                         "faces":  {
                                                                                                       "type":  "object",
                                                                                                       "additionalProperties":  false,
                                                                                                       "properties":  {
                                                                                                                          "bottom":  {
                                                                                                                                         "$ref":  "#/definitions/row"
                                                                                                                                     },
                                                                                                                          "right":  {
                                                                                                                                        "$ref":  "#/definitions/row"
                                                                                                                                    },
                                                                                                                          "top":  {
                                                                                                                                      "$ref":  "#/definitions/row"
                                                                                                                                  },
                                                                                                                          "left":  {
                                                                                                                                       "$ref":  "#/definitions/row"
                                                                                                                                   }
                                                                                                                      }
                                                                                                   },
                                                                                         "diameter_mm":  {
                                                                                                             "type":  "number",
                                                                                                             "minimum":  1.0
                                                                                                         },
                                                                                         "nuts":  {
                                                                                                      "type":  [
                                                                                                                   "string",
                                                                                                                   "null"
                                                                                                               ],
                                                                                                      "enum":  [
                                                                                                                   "both_sides",
                                                                                                                   "one_side",
                                                                                                                   null
                                                                                                               ]
                                                                                                  },
                                                                                         "hole_mm":  {
                                                                                                         "type":  [
                                                                                                                      "number",
                                                                                                                      "null"
                                                                                                                  ],
                                                                                                         "minimum":  1.0
                                                                                                     }
                                                                                     }
                                                                  },
                                                        "dimension_chains":  {
                                                                                 "type":  "array",
                                                                                 "items":  {
                                                                                               "type":  "object",
                                                                                               "additionalProperties":  false,
                                                                                               "required":  [
                                                                                                                "values_mm",
                                                                                                                "expected_total_mm"
                                                                                                            ],
                                                                                               "properties":  {
                                                                                                                  "expected_total_mm":  {
                                                                                                                                            "type":  "number",
                                                                                                                                            "minimum":  0.0
                                                                                                                                        },
                                                                                                                  "label":  {
                                                                                                                                "type":  "string"
                                                                                                                            },
                                                                                                                  "values_mm":  {
                                                                                                                                    "type":  "array",
                                                                                                                                    "items":  {
                                                                                                                                                  "type":  "number"
                                                                                                                                              },
                                                                                                                                    "minItems":  1
                                                                                                                                }
                                                                                                              }
                                                                                           }
                                                                             },
                                                        "uncertain_fields":  {
                                                                                 "type":  "array",
                                                                                 "items":  {
                                                                                               "type":  "object",
                                                                                               "additionalProperties":  false,
                                                                                               "required":  [
                                                                                                                "path",
                                                                                                                "reason"
                                                                                                            ],
                                                                                               "properties":  {
                                                                                                                  "reason":  {
                                                                                                                                 "type":  "string"
                                                                                                                             },
                                                                                                                  "user_confirmed_value":  {

                                                                                                                                           },
                                                                                                                  "path":  {
                                                                                                                               "type":  "string"
                                                                                                                           }
                                                                                                              }
                                                                                           }
                                                                             }
                                                    },
                                     "$schema":  "http://json-schema.org/draft-07/schema#",
                                     "description":  "Placa de extremo (Fase 12): dos planchas iguales, perpendiculares al eje, soldadas a cada tramo y unidas por pernos alrededor del tubo con tuerca a ambos lados. kind = splice: chord es el primer tramo y members[0] el segundo."
                                 },
                 "description":  "Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).",
                 "connection_type":  "end_plate",
                 "example":  {
                                 "members":  [
                                                 {
                                                     "profile":  "HSS12X8X1/2",
                                                     "role":  "chord",
                                                     "element_id":  1245530,
                                                     "expected_angle_deg":  0.0,
                                                     "end_setback_mm":  12.699999999999999
                                                 }
                                             ],
                                 "source":  {
                                                "drawing":  "Detalle C (empalme del cordón superior)",
                                                "scale":  "1/10"
                                            },
                                 "connection_type":  "end_plate",
                                 "node":  {
                                              "element_ids":  [
                                                                  1245531,
                                                                  1245530
                                                              ]
                                          },
                                 "chord":  {
                                               "end_setback_mm":  12.699999999999999,
                                               "element_id":  1245531,
                                               "profile":  "HSS12X8X1/2",
                                               "continuous":  false
                                           },
                                 "plate":  {
                                               "width_mm":  430.0,
                                               "thickness_label":  "1/2\"",
                                               "weld":  {
                                                            "type":  "fillet",
                                                            "size_mm":  6.0,
                                                            "all_around":  true
                                                        },
                                               "corner_cut_mm":  60.0,
                                               "height_mm":  330.0,
                                               "thickness_mm":  12.699999999999999
                                           },
                                 "gap_mm":  0.0,
                                 "spec_version":  "1.0",
                                 "kind":  "splice",
                                 "bolts":  {
                                               "hole_label":  "13/16\"",
                                               "diameter_label":  "3/4\"",
                                               "faces":  {
                                                             "bottom":  {
                                                                            "count":  4,
                                                                            "edge_mm":  25.0,
                                                                            "spacing_mm":  90.0
                                                                        },
                                                             "right":  {
                                                                           "count":  2,
                                                                           "edge_mm":  25.0,
                                                                           "spacing_mm":  80.0
                                                                       },
                                                             "top":  {
                                                                         "count":  4,
                                                                         "edge_mm":  25.0,
                                                                         "spacing_mm":  90.0
                                                                     },
                                                             "left":  {
                                                                          "count":  2,
                                                                          "edge_mm":  25.0,
                                                                          "spacing_mm":  80.0
                                                                      }
                                                         },
                                               "diameter_mm":  19.050000000000001,
                                               "nuts":  "both_sides",
                                               "hole_mm":  20.638000000000002
                                           },
                                 "dimension_chains":  [
                                                          {
                                                              "expected_total_mm":  430.0,
                                                              "label":  "ancho de la plancha",
                                                              "values_mm":  [
                                                                                60.0,
                                                                                310.0,
                                                                                60.0
                                                                            ]
                                                          },
                                                          {
                                                              "expected_total_mm":  310.0,
                                                              "label":  "pernos de arriba",
                                                              "values_mm":  [
                                                                                20.0,
                                                                                90.0,
                                                                                90.0,
                                                                                90.0,
                                                                                20.0
                                                                            ]
                                                          }
                                                      ],
                                 "uncertain_fields":  [

                                                      ]
                             }
             },
    "meta":  {
                 "duration_ms":  2,
                 "operation":  "schema",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-2 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 55 ms ==
{
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "origin_drawing":  "Detalle D",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "connection_type":  "gusset_node",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "members_count":  3,
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ]
             },
    "meta":  {
                 "duration_ms":  36,
                 "operation":  "catalog_list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-2 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 171 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 26 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 12-2 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 38 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12-2 sondeo 17 marcadores

```text
== 17-marcas-plan.py -> HTTP 200 en 758 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322481] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17-exportada.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 12-2 sondeo 21 etiquetas

```text
== 21-etiquetas-addin.py -> HTTP 200 en 264 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 0
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 46 BMP
   etiqueta-ambar-11-sel.bmp (3126 bytes)
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
   etiqueta-ambar-39.bmp (3126 bytes)
   etiqueta-ambar-4-sel.bmp (3126 bytes)
   etiqueta-ambar-4.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 0 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 12-2 log de arranque

```text

{"ts":"2026-10-21T09:59:41.4905665-05:00","record":{"event":"ribbon_panel_created","tab":"ARBA","panel":"MotorConexione
s","tab_already_existed":true,"create_tab_error":"ArgumentException: The tab with the input name exists 
already.\r\nParameter name: tabName"}}
{"ts":"2026-10-21T09:59:41.5043283-05:00","record":{"event":"startup","addin_version":"0.11.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-21T11:41:21.0682765-05:00","record":{"event":"ribbon_panel_created","tab":"ARBA","panel":"MotorConexione
s","tab_already_existed":true,"create_tab_error":"ArgumentException: The tab with the input name exists 
already.\r\nParameter name: tabName"}}
{"ts":"2026-10-21T11:41:21.0863197-05:00","record":{"event":"startup","addin_version":"0.12.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}



```

## 12-3 sondeo 22 corte plano

```text
== 22-corte-plano.py -> HTTP 200 en 781 ms ==
=== 22-corte-plano (Fase 12) ===
1) Cordon [1245533] HSS12X8X1/2: (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0)
1) Diagonal [1249631] HSS2-1-2X2-1-2X3-16 64x64: (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9)
2) Extremo que llega: fin ((-9354.7, -17195.8, 19884.9)); corte del eje con el eje del cordon: (-9305.5, -17195.7, 19933.0); hueco extremo-eje: 68.7 mm
2) Extension actual del extremo: 69.1996468112 mm | Cut Length: 3569.19964681 mm
3) Caras planas del cordon: 10; cara a la que llega la diagonal: normal (-0.0, -0.0, -304.8) | punto del eje en la cara (-9213.8, -17195.7, 20022.8) | a 128.4 mm del corte (medio canto esperado 101,6 en HSS12X8)
3) Reference de la cara: si
4) StructuralFramingUtils, metodos estaticos: AllowJoinAtEnd, CanFlipEnds, CanSetEndReference, DisallowJoinAtEnd, FlipEnds, GetEndReference, IsEndReferenceValid, IsJoinAllowedAtEnd, RemoveEndReference, SetEndReference
   CanSetEndReference(FamilyInstance familyInstance, Int32 end) -> Boolean
   IsEndReferenceValid(FamilyInstance familyInstance, Int32 end, Reference pick) -> Boolean
   RemoveEndReference(FamilyInstance familyInstance, Int32 end) -> Void
   GetEndReference(FamilyInstance familyInstance, Int32 end) -> Reference
   SetEndReference(FamilyInstance familyInstance, Int32 end, Reference pick) -> Void
   DisallowJoinAtEnd(FamilyInstance familyInstance, Int32 end) -> Void
   AllowJoinAtEnd(FamilyInstance familyInstance, Int32 end) -> Void
   IsJoinAllowedAtEnd(FamilyInstance familyInstance, Int32 end) -> Boolean
5) CanSetEndReference(diagonal, 1) = False
5) SetEndReference ERROR Exception: The input familyInstance is not of a structural framing category or is concrete or is not joined at given end and cannot have an end reference set.
Parameter name: end
Traceback (most recent call last):
  File "<string>", line 201, in <module>
Exception: The input familyInstance is not of a structural framing category or is concrete or is not joined at given end and cannot have an end reference set.
Parameter name: end

5) Transaction deshecha (RollBack). Extension ahora: 69.1996468112 mm
6) Tipos de ASObjectsMgd que recortan vigas: Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch, Autodesk.AdvanceSteel.Modelling.BeamNotch, Autodesk.AdvanceSteel.Modelling.BeamNotch+eBeamNotchCornerType, Autodesk.AdvanceSteel.Modelling.BeamNotch2Ortho, Autodesk.AdvanceSteel.Modelling.BeamNotchEx, Autodesk.AdvanceSteel.Modelling.BeamShortening, Autodesk.AdvanceSteel.Modelling.PlateFeatShortening
   BeamMultiContourNotch(Beam pBeam, eEnd end, Point3d centerWCS, Vector3d normalWCS, Vector3d xvec, Double radius)
   BeamMultiContourNotch(Beam pBeam, eEnd end, Point3d centerWCS, Vector3d normalWCS, Vector3d xvec, Double length, Double width)
   BeamMultiContourNotch(Beam pBeam, eEnd end, Polyline3d contourWCS, Vector3d normalWCS, Vector3d xvec)
   BeamMultiContourNotch(Beam pBeam, eEnd end, Point3d[] contourWCS, Vector3d normalWCS, Vector3d xvec)
   BeamMultiContourNotch()
   propiedades de BeamMultiContourNotch: ASId, BoringOut, CS, CenterPoint, ClipType, CompoundReprMode, ContourType, DrivingPassiveConstructionObject, End, Gap, GeomExtents, Handle, HasDrivingPassiveConstructionObject, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, Length, LengthIncrement, Normal, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, Offset, PureRole, RadIncrement, Radius, Recipe, ReprMode, Role, RoleDescription, SkipEraseEnvironment, UseGap, VertexInformation, Width
   propiedades de BeamNotch: ASId, CS, CenterPoint, CompoundReprMode, CornerRadius, CornerType, DrivingPassiveConstructionObject, End, GeomExtents, Handle, HasDrivingPassiveConstructionObject, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, PureRole, Recipe, ReferenceDepth, ReferenceLength, ReprMode, Role, RoleDescription, Side, SkipEraseEnvironment, UseGap
   propiedades de eBeamNotchCornerType: 
   BeamNotch2Ortho(eEnd end, eSide side, Double length, Double depth)
   BeamNotch2Ortho()
   propiedades de BeamNotch2Ortho: ASId, CS, CenterPoint, CompoundReprMode, CornerRadius, CornerType, DrivingPassiveConstructionObject, End, GeomExtents, Handle, HasDrivingPassiveConstructionObject, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, PureRole, Recipe, ReferenceDepth, ReferenceLength, ReprMode, Role, RoleDescription, Side, SkipEraseEnvironment, UseGap
   BeamNotchEx(eEnd end, eSide side, Double length, Double depth)
   BeamNotchEx()
   propiedades de BeamNotchEx: ASId, AxisAngle, CS, CenterPoint, CompoundReprMode, CornerRadius, CornerType, DrivingPassiveConstructionObject, End, GeomExtents, Handle, HasDrivingPassiveConstructionObject, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, PureRole, Recipe, ReferenceDepth, ReferenceLength, ReprMode, Role, RoleDescription, Side, SkipEraseEnvironment, UseGap, XAngle, XRotation, ZAngle
   BeamShortening(eEnd end, Double length)
   BeamShortening()
   propiedades de BeamShortening: ASId, AngleOnY, AngleOnZ, CS, CenterPoint, CompoundReprMode, CutStraight, CutStraightOffset, CutStraightRelativeOffset, CutStraightType, DrivingPassiveConstructionObject, End, GeomExtents, Handle, HasDrivingPassiveConstructionObject, InsLength, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, PureRole, Recipe, ReprMode, Role, RoleDescription, SkipEraseEnvironment, UseGap
   PlateFeatShortening(Plane plDef, Double dSuckDistance)
   PlateFeatShortening()
   propiedades de PlateFeatShortening: ASId, Angle, CS, CenterPoint, CompoundReprMode, CutPlane, Distance, DrivingPassiveConstructionObject, GeomExtents, Handle, HasDrivingPassiveConstructionObject, IsCutStraight, IsFromFitter, IsGeometryNotEditable, JointTransferID, Layer, NumberOfDrivenConObj, NumberOfReprModes, ObjectIndex, PureRole, Recipe, ReprMode, Role, RoleDescription, SkipEraseEnvironment, StraightCutOffset, StraightPlane, SuctionDistance, UseGap
6) No se crea ninguno (sin firma conocida): copia esta lista en resultados-fase-12.md para la ronda 12b.
=== fin 22-corte-plano ===


```

## 12-3 sondeo 23 pernos dos planchas

```text
== 23-pernos-dos-planchas.py -> HTTP 200 en 1108 ms ==
=== 23-pernos-dos-planchas (Fase 12) ===
1) Transaccion: Autodesk.SteelConnectionsDB.FabricationTransaction; CancelTransaction: True
2) Plate y FinitRectScrewBoltPattern listos. Propiedades del patron: ASId, Annotation, Assembly, AssemblyLocation, AssemblyUsedForNumbering, BindingLength, BindingLengthAddition, BoltAssembly, BoltHeadDiameter, BoltHeadHeight, BoltHeadNumEdges, BoltNormal, BottomToolDiameter, BottomToolHeight, CS, Center, CenterPoint, Coating, CoatingDescription, CoatingUsedForNumbering, CompoundReprMode, DennotationUsedForNumbering, Denotation, DrivingPassiveConstructionObject, Dx, Dy, ExplicitQuantity, GeomExtents, Grade, Handle, HasDrivingPassiveConstructionObject, Height, HoleTolerance, HolesUsedForNumbering, IgnoreMaxGap, IsAttachedPart, IsGeometryNotEditable, IsInverted, IsMainPart, ItemNumber, ItemNumberUsedForNumbering, JointTransferID, Layer, Length, MainPartNumber, MainPartPrefix, MainPartUsedForBOM, MainPartUsedForCollisionCheck, MainPartUsedForNumbering, Material, MaterialDescription, MaterialUsedForNumbering, MaxBottomDiameter, MaxTopDiameter, MidpointOnLower
3) Propiedades que pueden fijar el conjunto del perno (tuerca, arandela, norma): Assembly, AssemblyLocation, AssemblyUsedForNumbering, BoltAssembly, BoltHeadDiameter, BoltHeadHeight, BoltHeadNumEdges, BoltNormal, Grade, IsInverted, Material, MaterialDescription, MaterialUsedForNumbering, NumberOfScrews, NutDiameter, NutHeight, ScrewBoltType, ScrewDiameter, ScrewLength, Standard, SumBottomSetHeight, SumTopSetHeight
3) Metodos con Nut/Washer/Assembly/Set/Connect/Invert/Head: Connect, GetConnectedElements, GetConnectedObjects, GetConnection, RemoveConnectedObjects, SetCS, SetHoleDefinition, SetLayer, SetLevel, SetNextVisibleReprMode, SetUniqueId, SetUserAttribute, SetUserAttributeUsedForNumbering, SetWorkingPlaneLinkInfo, UpdateYourselfAsConnectionMeans, get_Assembly, get_AssemblyLocation, get_AssemblyUsedForNumbering, get_BoltAssembly, get_BoltHeadDiameter, get_BoltHeadHeight, get_BoltHeadNumEdges, get_IsInverted, get_NutDiameter, get_NutHeight, get_SumBottomSetHeight, get_SumTopSetHeight, set_Assembly, set_AssemblyLocation, set_AssemblyUsedForNumbering, set_BoltAssembly, set_IsInverted
   get_NutHeight() -> Double
   get_NutDiameter() -> Double
   Connect(FilerObject[] elems, eAssemblyLocation location) -> Void
3) Tipos de ASObjectsMgd con Nut/Washer/BoltSet/ScrewSet/Assembly: eAssemblyLocation
4) Tramo [1245531]: punto de empalme (-14656.0, 17204.3, 19933.0); Z (-304.8, 0.0, -0.0); Y (-0.0, 0.0, 304.8); X (-0.0, -304.8, 0.0)
4) Pila de dos planchas: agarre 19.05 mm, longitud del perno 44.45 mm (suplemento 5/8" 22.23 redondeado a 6.35)
5) FabricationTransaction abierta. doc.IsModifiable=True
   plancha A (lado del tramo) escrita en z local +4.763 mm
   plancha B (otro tramo) escrita en z local -4.763 mm
   patron 4 x 1: Nx=4; Ny=1; Dx=50.0; Dy=50.0; ScrewDiameter=15.875; BindingLength=19.05; ScrewLength=44.45
   NumberOfScrews antes de escribir: 4
   fila 4 x 1: WriteToDb OK; NumberOfScrews=4; ScrewLength=44.45; BindingLength=19.05
   Connect(FilerObject[], eAssemblyLocation)
   eAssemblyLocation valores: kOnSite, kSiteDrill, kInShop, kWrong, kUnknown
   Connect: OK con kOnSite
   pernos sueltos: 4/4 escritos
6) Elementos nuevos con la transaccion abierta: 0
7) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False
8) Elementos nuevos tras cerrar: 0 (se esperan 0 con CancelTransaction; si no, sondeo 13)
9) RESUMEN: Connect: OK con kOnSite | fila 4x1: WriteToDb OK; NumberOfScrews=4; ScrewLength=44.45; BindingLength=19.05 | planchas: 2 planchas de 200.0 x 200.0 x 9.525 con gap 0.0: WriteToDb OK | sueltos 1x1: 4/4 escritos
=== fin 23-pernos-dos-planchas ===


```

## 12-3 sondeo 24 soldadura contorno

```text
== 24-soldadura-contorno.py -> HTTP 200 en 402 ms ==
=== 24-soldadura-contorno (Fase 12) ===
1) Transaccion: Autodesk.SteelConnectionsDB.FabricationTransaction
2) Tipos con Weld en ASObjectsMgd: WeldPattern, WeldPoint, PlateFeatWeldPreparation, PlateFeatWeldFillet, WeldLine
   --- Autodesk.AdvanceSteel.Modelling.WeldPattern
   propiedades: ASId, AdditionalData, Assembly, AssemblyLocation, AssemblyUsedForNumbering, CS, CenterPoint, Coating, CoatingDescription, CoatingUsedForNumbering, CompoundReprMode, DennotationUsedForNumbering, Denotation, DoubleEffectiveThroat, DoublePreparationDepth, DoubleRootOpening, DoubleWeldText, DrivingPassiveConstructionObject, ExplicitQuantity, GeomExtents, Handle, HasDrivingPassiveConstructionObject, HolesUsedForNumbering, IsAttachedPart, IsClosed, IsGeometryNotEditable, IsMainPart, ItemNumber, ItemNumberUsedForNumbering, JointTransferID, Layer, Length, MainEffectiveThroat, MainPartNumber, MainPartPrefix, MainPartUsedForBOM, MainPartUsedForCollisionCheck, MainPartUsedForNumbering, MainPreparationDepth, MainRootOpening, MainWeldText, Material, MaterialDescription, MaterialUsedForNumbering, Note, NoteUsedForNumbering, NumberOfDrivenConObj, NumberOfHoles, NumberOfReprModes, Pitch, Prefix, Prelimi
   enum AssemblyLocation (eAssemblyLocation): kOnSite, kSiteDrill, kInShop, kWrong, kUnknown
   metodos propios: Accept, AddDrivenConObj, AddFeature, AdjustGeometryToWorkingPlanes, Connect, CopyFeaturesFrom, CreateACIS, DelAllFeatures, DelFeature, DelFromDb, DelReference, Dispose, EmptyWorkingPlaneLinkInfo, Equals, ExportToACIS, GetBalancepoint, GetCenterPoint, GetConnectedElements, GetConnectedObjects, GetConnection, GetDSTVName, GetDrivenConObj, GetExternalName, GetExternalNames, GetFaceShape, GetFeatures, GetFilerObject, GetHashCode, GetLevel, GetMainPartPositionNumber, GetModeler, GetModelerBodyList, GetName, GetNextVisibleReprMode, GetNumberOfGeometryIDs, GetNumberingStatus, GetObjectId, GetQuantityInModel, GetSeamExecuting, GetSeamThickness, GetSinglePartPositionNumber, GetUniqueId, GetUserAttrib
   --- Autodesk.AdvanceSteel.Modelling.WeldPoint
   ctor WeldPoint(Point3d origin, Vector3d xvec, Vector3d yvec)
   propiedades: ASId, AdditionalData, Assembly, AssemblyLocation, AssemblyUsedForNumbering, CS, CenterPoint, Coating, CoatingDescription, CoatingUsedForNumbering, CompoundReprMode, DennotationUsedForNumbering, Denotation, DoubleEffectiveThroat, DoublePreparationDepth, DoubleRootOpening, DoubleWeldText, DrivingPassiveConstructionObject, ExplicitQuantity, GeomExtents, Handle, HasDrivingPassiveConstructionObject, HolesUsedForNumbering, IsAttachedPart, IsClosed, IsGeometryNotEditable, IsMainPart, ItemNumber, ItemNumberUsedForNumbering, JointTransferID, Layer, Length, MainEffectiveThroat, MainPartNumber, MainPartPrefix, MainPartUsedForBOM, MainPartUsedForCollisionCheck, MainPartUsedForNumbering, MainPreparationDepth, MainRootOpening, MainWeldText, Material, MaterialDescription, MaterialUsedForNumbering, Note, NoteUsedForNumbering, NumberOfDrivenConObj, NumberOfHoles, NumberOfReprModes, Pitch, Prefix, Prelimi
   enum AssemblyLocation (eAssemblyLocation): kOnSite, kSiteDrill, kInShop, kWrong, kUnknown
   metodos propios: Accept, AddDrivenConObj, AddFeature, AdjustGeometryToWorkingPlanes, Connect, CopyFeaturesFrom, CreateACIS, DelAllFeatures, DelFeature, DelFromDb, DelReference, Dispose, EmptyWorkingPlaneLinkInfo, Equals, ExportToACIS, GetBalancepoint, GetCenterPoint, GetConnectedElements, GetConnectedObjects, GetConnection, GetDSTVName, GetDrivenConObj, GetExternalName, GetExternalNames, GetFaceShape, GetFeatures, GetFilerObject, GetHashCode, GetLevel, GetMainPartPositionNumber, GetModeler, GetModelerBodyList, GetName, GetNextVisibleReprMode, GetNumberOfGeometryIDs, GetNumberingStatus, GetObjectId, GetQuantityInModel, GetSeamExecuting, GetSeamThickness, GetSinglePartPositionNumber, GetUniqueId, GetUserAttrib
   --- Autodesk.AdvanceSteel.Modelling.WeldLine
   ctor WeldLine(Point3d[] polyLineWCS, Vector3d xvec, Vector3d yvec)
   propiedades: ASId, AdditionalData, Assembly, AssemblyLocation, AssemblyUsedForNumbering, CS, CenterPoint, Coating, CoatingDescription, CoatingUsedForNumbering, CompoundReprMode, DennotationUsedForNumbering, Denotation, DoubleEffectiveThroat, DoublePreparationDepth, DoubleRootOpening, DoubleWeldText, DrivingPassiveConstructionObject, ExplicitQuantity, GeomExtents, Handle, HasDrivingPassiveConstructionObject, HolesUsedForNumbering, IsAttachedPart, IsClosed, IsGeometryNotEditable, IsMainPart, ItemNumber, ItemNumberUsedForNumbering, JointTransferID, Layer, Length, MainEffectiveThroat, MainPartNumber, MainPartPrefix, MainPartUsedForBOM, MainPartUsedForCollisionCheck, MainPartUsedForNumbering, MainPreparationDepth, MainRootOpening, MainWeldText, Material, MaterialDescription, MaterialUsedForNumbering, Note, NoteUsedForNumbering, NumberOfDrivenConObj, NumberOfHoles, NumberOfReprModes, Pitch, Prefix, Prelimi
   enum AssemblyLocation (eAssemblyLocation): kOnSite, kSiteDrill, kInShop, kWrong, kUnknown
   metodos propios: Accept, AddDrivenConObj, AddFeature, AdjustGeometryToWorkingPlanes, Connect, CopyFeaturesFrom, CreateACIS, DelAllFeatures, DelFeature, DelFromDb, DelReference, Dispose, EmptyWorkingPlaneLinkInfo, Equals, ExportToACIS, GetBalancepoint, GetCenterPoint, GetConnectedElements, GetConnectedObjects, GetConnection, GetDSTVName, GetDrivenConObj, GetExternalName, GetExternalNames, GetFaceShape, GetFeatures, GetFilerObject, GetHashCode, GetLevel, GetMainPartPositionNumber, GetModeler, GetModelerBodyList, GetName, GetNextVisibleReprMode, GetNumberOfGeometryIDs, GetNumberingStatus, GetObjectId, GetQuantityInModel, GetSeamExecuting, GetSeamThickness, GetSinglePartPositionNumber, GetUniqueId, GetUserAttrib
   enum Autodesk.AdvanceSteel.Modelling.WeldPattern+eWeldType: kFlangeButtWelding, kIWeld, kVWeld, kHVWeld, kYWeld, kHYWeld, kUWeld, kHUWeld, kTWeld, kPerforationWeld, kSpotWeld, kLineWeld, kSquareEdgeWeld, kHalfSquareEdgeWeld, kFrontFlushWeld, kProfileWeld, kSlantWeld, kSeamWeld, kResufaceWeld, kCounterWeld, kFlangeButtWeldingJointWithCounterWeld, kDIWeld, kVWeldWithCounterWeld, kXWeld, kKWeld, kDYWeld, kKWebWeld, kDUWeld, kDHUWeld, kVUWeld, kDoubleFilletWeld
   enum Autodesk.AdvanceSteel.Modelling.WeldPattern+eSeamPosition: kUpper, kLower
   enum Autodesk.AdvanceSteel.Modelling.WeldPattern+eWeldSurface: kUndefined, kFlat, kConvex, kConcav
   enum Autodesk.AdvanceSteel.Modelling.WeldPattern+eExecType: kUndefined, kRootElaborated, kSeamSmoothed, kSeamTransitionsWrought, kAddendumUsed, kPadUsed
2) WeldLine con constructor utilizable: NO (copia la lista de arriba); WeldPoint(Point3d): False
3) Barra [1249631] HSS2-1-2X2-1-2X3-16 64x64: extremo (-9354.7, -17195.8, 19884.9), eje (217.9, 0.0, 213.2), huella 63.5 x 63.5 mm
4) FabricationTransaction abierta. doc.IsModifiable=True
   escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)
5) Elementos nuevos con la transaccion abierta: 0
6) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False
7) Elementos nuevos tras cerrar: 0 (se esperan 0; si no, sondeo 13)
8) RESUMEN: WeldLine: sin constructor utilizable (ver la lista del paso 2) | plancha de apoyo: escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)
=== fin 24-soldadura-contorno ===


```

## 12-3 sondeo 12 tras los sondeos

```text
== 12-fase3-borrar.py -> HTTP 200 en 109 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 12-3 sondeo 13 tras los sondeos

```text
== 13-limpiar-fase1.py -> HTTP 200 en 39 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12-4 node_info nudo en K

```text
== conn/node_info -> HTTP 200 en 449 ms ==
{
    "data":  {
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "members":  [
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "axis_offset_mm":  0,
                                     "wall_mm":  11.810000000000000,
                                     "element_id":  1245533,
                                     "length_mm":  10475,
                                     "height_mm":  203.19999999999999,
                                     "node_end":  0,
                                     "angle_in_plane_deg":  -180,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "width_mm":  203.19999999999999,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "slope_deg":  0,
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "side":  "chord",
                                     "reaches_node":  true,
                                     "is_chord":  true,
                                     "type":  "HSS12X8X1/2",
                                     "angle_to_chord_deg":  0
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "axis_offset_mm":  -27.399999999999999,
                                     "wall_mm":  0,
                                     "element_id":  1249631,
                                     "length_mm":  3500,
                                     "height_mm":  63.5,
                                     "node_end":  1,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "width_mm":  63.5,
                                     "end_gap_mm":  90.400000000000006,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "slope_deg":  44.369999999999997,
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "side":  "-Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_to_chord_deg":  44.399999999999999
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "axis_offset_mm":  27.399999999999999,
                                     "wall_mm":  0,
                                     "element_id":  1249632,
                                     "length_mm":  3546,
                                     "height_mm":  63.5,
                                     "node_end":  0,
                                     "angle_in_plane_deg":  -45,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "width_mm":  63.5,
                                     "end_gap_mm":  27.100000000000001,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "slope_deg":  45,
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "side":  "-Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_to_chord_deg":  45
                                 }
                             ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "chord_element_id":  1245533,
                 "eccentricity_mm":  54.899999999999999,
                 "axis_distance_mm":  0.089999999999999997,
                 "existing_connections":  [

                                          ],
                 "kind":  "node",
                 "chord_direction_reversed":  true,
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)"
             },
    "meta":  {
                 "duration_ms":  34,
                 "operation":  "node_info",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-4 node_info empalme

```text
== conn/node_info -> HTTP 200 en 20 ms ==
{
    "data":  {
                 "origin_mm":  [
                                   -4180.5,
                                   17204.200000000001,
                                   19933
                               ],
                 "members":  [
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "axis_offset_mm":  0,
                                     "wall_mm":  11.810000000000000,
                                     "element_id":  1245531,
                                     "length_mm":  10475,
                                     "height_mm":  203.19999999999999,
                                     "node_end":  0,
                                     "angle_in_plane_deg":  -180,
                                     "end_mm":  [
                                                    -14656,
                                                    17204.299999999999,
                                                    19933
                                                ],
                                     "width_mm":  203.19999999999999,
                                     "end_gap_mm":  0.40000000000000002,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -4180.5,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "slope_deg":  0,
                                     "start_mm":  [
                                                      -4181,
                                                      17204.200000000001,
                                                      19933
                                                  ],
                                     "side":  "-Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS12X8X1/2",
                                     "angle_to_chord_deg":  0
                                 },
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "axis_offset_mm":  0,
                                     "wall_mm":  11.810000000000000,
                                     "element_id":  1245530,
                                     "length_mm":  10250,
                                     "height_mm":  203.19999999999999,
                                     "node_end":  1,
                                     "angle_in_plane_deg":  0,
                                     "end_mm":  [
                                                    -4180.1000000000004,
                                                    17204.200000000001,
                                                    19933
                                                ],
                                     "width_mm":  203.19999999999999,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -4180.5,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "slope_deg":  0,
                                     "start_mm":  [
                                                      6069.8999999999996,
                                                      17204.200000000001,
                                                      19933
                                                  ],
                                     "side":  "chord",
                                     "reaches_node":  true,
                                     "is_chord":  true,
                                     "type":  "HSS12X8X1/2",
                                     "angle_to_chord_deg":  0
                                 }
                             ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                0
                            ],
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "chord_element_id":  1245530,
                 "eccentricity_mm":  0,
                 "axis_distance_mm":  0,
                 "existing_connections":  [

                                          ],
                 "kind":  "splice",
                 "chord_direction_reversed":  true,
                 "y_axis":  [
                                0,
                                0,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)"
             },
    "meta":  {
                 "duration_ms":  5,
                 "operation":  "node_info",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-4 node_info Detalle D

```text
== conn/node_info -> HTTP 200 en 22 ms ==
{
    "data":  {
                 "origin_mm":  [
                                   -11869.400000000000,
                                   -17195.799999999999,
                                   17423
                               ],
                 "members":  [
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "axis_offset_mm":  0,
                                     "wall_mm":  5.9199999999999999,
                                     "element_id":  1249510,
                                     "length_mm":  9960.2999999999993,
                                     "height_mm":  76.200000000000003,
                                     "node_end":  1,
                                     "angle_in_plane_deg":  0,
                                     "end_mm":  [
                                                    -14397.600000000000,
                                                    -17195.799999999999,
                                                    17423
                                                ],
                                     "width_mm":  76.200000000000003,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -11869.400000000000,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "slope_deg":  0,
                                     "start_mm":  [
                                                      -4437.3000000000002,
                                                      -17195.700000000001,
                                                      17423
                                                  ],
                                     "side":  "chord",
                                     "reaches_node":  true,
                                     "is_chord":  true,
                                     "type":  "HSS3X3X1/4",
                                     "angle_to_chord_deg":  0
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "axis_offset_mm":  1.7000000000000000,
                                     "wall_mm":  0,
                                     "element_id":  1249630,
                                     "length_mm":  3568,
                                     "height_mm":  63.5,
                                     "node_end":  1,
                                     "angle_in_plane_deg":  136.90000000000001,
                                     "end_mm":  [
                                                    -11930.600000000000,
                                                    -17195.799999999999,
                                                    17481.799999999999
                                                ],
                                     "width_mm":  63.5,
                                     "end_gap_mm":  84.900000000000006,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -11867.700000000001,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "slope_deg":  43.079999999999998,
                                     "start_mm":  [
                                                      -14536.799999999999,
                                                      -17195.799999999999,
                                                      19918.799999999999
                                                  ],
                                     "side":  "+Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_to_chord_deg":  43.100000000000001
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "axis_offset_mm":  -1.7000000000000000,
                                     "wall_mm":  0,
                                     "element_id":  1249631,
                                     "length_mm":  3500,
                                     "height_mm":  63.5,
                                     "node_end":  0,
                                     "angle_in_plane_deg":  44.399999999999999,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "width_mm":  63.5,
                                     "end_gap_mm":  19.300000000000001,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -11871.100000000000,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "slope_deg":  44.369999999999997,
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "side":  "+Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_to_chord_deg":  44.399999999999999
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "axis_offset_mm":  -1.7000000000000000,
                                     "wall_mm":  0,
                                     "element_id":  1249636,
                                     "length_mm":  3567.9000000000001,
                                     "height_mm":  63.5,
                                     "node_end":  1,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "end_mm":  [
                                                    -11904.900000000000,
                                                    -17195.700000000001,
                                                    17389.900000000001
                                                ],
                                     "width_mm":  63.5,
                                     "end_gap_mm":  48.600000000000001,
                                     "structural_type":  "Beam",
                                     "axis_cut_mm":  [
                                                         -11871.100000000000,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "slope_deg":  44.369999999999997,
                                     "start_mm":  [
                                                      -14455.299999999999,
                                                      -17195.700000000001,
                                                      14894.700000000001
                                                  ],
                                     "side":  "-Y",
                                     "reaches_node":  true,
                                     "is_chord":  false,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_to_chord_deg":  44.399999999999999
                                 }
                             ],
                 "z_axis":  [
                                2E-06,
                                -1,
                                0
                            ],
                 "x_axis":  [
                                1,
                                2E-06,
                                0
                            ],
                 "chord_element_id":  1249510,
                 "eccentricity_mm":  3.3999999999999999,
                 "axis_distance_mm":  0.080000000000000002,
                 "existing_connections":  [

                                          ],
                 "kind":  "node",
                 "chord_direction_reversed":  true,
                 "y_axis":  [
                                0,
                                0,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)"
             },
    "meta":  {
                 "duration_ms":  6,
                 "operation":  "node_info",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-5 list tras el nudo en K

```text
== conn/list -> HTTP 200 en 419 ms ==
{
    "data":  {
                 "connections":  [
                                     {
                                         "connection_id":  "5b5fd89b-9161-4560-92d7-4209da8a377e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T16:47:18.3273069Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     }
                                 ],
                 "batches":  {

                             },
                 "batch_id":  null,
                 "connections_count":  1,
                 "total_count":  1
             },
    "meta":  {
                 "duration_ms":  4,
                 "operation":  "list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-5 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 33 ms ==
{
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  2,
                 "templates":  [
                                   {
                                       "name":  "Nudo K cordon superior",
                                       "tags":  [

                                                ],
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "connection_type":  "direct_weld",
                                       "chord_profile":  "HSS12X8X1/2",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "members_count":  2,
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  null
                                   },
                                   {
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "origin_drawing":  "Detalle D",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "connection_type":  "gusset_node",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "members_count":  3,
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ]
             },
    "meta":  {
                 "duration_ms":  1,
                 "operation":  "catalog_list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-6 list tras el empalme

```text
== conn/list -> HTTP 200 en 405 ms ==
{
    "data":  {
                 "connections":  [
                                     {
                                         "connection_id":  "5b5fd89b-9161-4560-92d7-4209da8a377e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T16:47:18.3273069Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "f370914e-94a4-441f-8bdd-c4b038d3c309",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T16:54:34.1395681Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     }
                                 ],
                 "batches":  {

                             },
                 "batch_id":  null,
                 "connections_count":  2,
                 "total_count":  2
             },
    "meta":  {
                 "duration_ms":  4,
                 "operation":  "list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-6 log del empalme

```text

{"ts":"2026-10-21T11:47:18.2682695-05:00","record":{"event":"direct_weld_member","element_id":1249631,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-21T11:47:18.2835324-05:00","record":{"event":"direct_weld_member","element_id":1249632,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-21T11:54:33.9574719-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245530","vertices":8,"thickness_mm":12.7,"offset_mm":6.35,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T11:54:33.9598628-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245531","vertices":8,"thickness_mm":12.7,"offset_mm":-6.35,"plane_z_mm":-12.7,"units":"mm"}}
{"ts":"2026-10-21T11:54:33.9649976-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T11:54:33.9687369-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T11:54:33.9712058-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T11:54:33.9730206-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T11:54:33.9845378-05:00","record":{"event":"end_plate_built","first_segment":1245530,"second_segment":
1245531,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.3,12.3],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.307565366741405E-06,-0]}}



```

## 12-8 list tras el lote

```text
== conn/list -> HTTP 200 en 424 ms ==
{
    "data":  {
                 "connections":  [
                                     {
                                         "connection_id":  "5b5fd89b-9161-4560-92d7-4209da8a377e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T16:47:18.3273069Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "f370914e-94a4-441f-8bdd-c4b038d3c309",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T16:54:34.1395681Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "7776b004-6456-49b0-9412-732739e7ffa2",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:04.8345846Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "1eb4f785-902a-4afa-b4db-a37072912483",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.0148473Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "b8dbcd43-f16d-4223-9f05-0b8f6bb17265",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:05.1844600Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "9adebe50-be0d-460c-80f0-e205364b358e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:05.3944245Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "4cde5f9a-12da-4896-a5b3-86c78c0f2aa6",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.5227898Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "994e38c0-f8ba-48a9-8413-25fa7fc8772d",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:05.6596395Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "509c7537-6a6c-42ed-bd99-d1609c3ab93d",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.8172818Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "2fe4c11f-b38d-473e-bdad-28461f3cd082",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:05.9797078Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "83a64c13-2981-40a5-8654-f91f8c5028a3",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.1578784Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "dca546c9-2a86-48c6-9cde-3ebad26956e3",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:06.3172126Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "6fd5f8f5-b3e3-4397-a975-91708b520f93",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.4703540Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "ff4b8802-d659-429d-9bc7-8c869d31b743",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:06.5918868Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "549598f9-d868-4e8d-8eb9-8a3e30b309ed",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:06.7585380Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "18028706-6c16-40e3-8683-1cdfeb8de515",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.9614880Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "8a553d43-1b20-40f7-b170-d56b1f608eda",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.1161568Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "87927705-79a6-485f-b323-90c2e2f1d46d",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:07.2403044Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "e00a1db0-20ef-4b89-9dc2-6e4a711520c6",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.3843092Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "f5043651-6388-4a9d-902e-ad7626b794bd",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:07.5648113Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "ab9a5c5d-a663-4fbb-8178-e1e91f9707fb",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:07.7509562Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "19257157-e88f-489a-9bcf-11022a150a1c",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.8838815Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "ead78189-a064-41ff-88a9-dd048dc1d8ae",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.0505508Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "fc9f689f-e692-473f-aa98-e495a6065f2b",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.1666504Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "bd60566a-21ff-42a2-9011-a52b900fe49f",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:08.2991691Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "ca7a53cd-d955-4af9-bb6d-bfb90250f929",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.4703544Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "81a54a73-e722-4910-86a9-5d816de53c30",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.5896796Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "73e65525-985a-4da6-9336-35f7f0bd0f09",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.7360938Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "50272955-17e8-4bf0-9aab-86437c276b86",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.8640707Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "09af65a5-2143-45ee-a90c-1ff23f6fc0a5",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.0077233Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "d1363ed3-8780-44b6-a4e3-6a7dddd76425",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.1605167Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "cd5b87b1-a9d8-4197-8bef-321f13f78824",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:09.2885765Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "1f68d407-5c99-4b93-9b88-69f432ab1774",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.4248833Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "b14f334e-1301-4b82-98a4-815a0fd67edb",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:09.5561453Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "f3bdf907-6874-4139-ab66-aee7cb0c5fb0",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.6644157Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "04df2997-4f17-455f-a22a-1378cf7b1719",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.7707784Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "d076e320-b6e0-498e-a116-5494fccedc0c",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.9789596Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "a7419d94-00fc-49a4-b64d-bb853194fe0b",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.1416344Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "96418f83-1daf-428b-b94a-f204a821c173",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:10.2712878Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "23960cf8-00a2-4839-8bf8-ea8f6d759da0",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.3982602Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "7235620e-0970-4510-9f4c-4234b5b40a3e",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:10.5217924Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "487354a9-d777-47e1-86fd-508df18873de",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.6777728Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     }
                                 ],
                 "batches":  {
                                 "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c":  40
                             },
                 "batch_id":  null,
                 "connections_count":  42,
                 "total_count":  42
             },
    "meta":  {
                 "duration_ms":  6,
                 "operation":  "list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-8 sondeo 17 tras el lote

```text
== 17-marcas-plan.py -> HTTP 200 en 1053 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 1
   [1323006] N5 | dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c:N5 | N5 · MotorConexiones plan dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c; view=1245519; ids=1245533,1249631,1249632; already_connected; gris
3) Barra de prueba: [1249630] HSS2-1-2X2-1-2X3-16 64x64 | centro mm (-13233.7, -17195.8, 18700.3)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249630]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1323511] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249630 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17-exportada.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 12-8 sondeo 21 tras el lote

```text
== 21-etiquetas-addin.py -> HTTP 200 en 237 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 1: 10
   control 10: visible=? ('TemporaryGraphicsManager' object has no attribute 'IsVisible')
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 75 BMP
   etiqueta-ambar-11-sel.bmp (3126 bytes)
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-16.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-23.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-30.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 1 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 12-8 log del lote

```text

{"ts":"2026-10-21T11:47:18.2682695-05:00","record":{"event":"direct_weld_member","element_id":1249631,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-21T11:47:18.2835324-05:00","record":{"event":"direct_weld_member","element_id":1249632,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-21T11:54:33.9845378-05:00","record":{"event":"end_plate_built","first_segment":1245530,"second_segment":
1245531,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.3,12.3],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.307565366741405E-06,-0]}}
{"ts":"2026-10-21T12:05:05.1686105-05:00","record":{"event":"end_plate_built","first_segment":1245533,"second_segment":
1245532,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[-0.9999999999973376,2.3075653684491078E-06,0]}}
{"ts":"2026-10-21T12:05:05.3523450-05:00","record":{"event":"direct_weld_member","element_id":1249633,"axis_offset_mm":
0,"setback_ideal_mm":145.13,"axis_setback_mm":177.52,"face_y_mm":-101.6,"footprint_mm":90.71,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:05.3548685-05:00","record":{"event":"direct_weld_member","element_id":1249626,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:05.6487634-05:00","record":{"event":"direct_weld_member","element_id":1249627,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:05.6508918-05:00","record":{"event":"direct_weld_member","element_id":1249628,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:05.9652723-05:00","record":{"event":"end_plate_built","first_segment":1245532,"second_segment":
1245535,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[-0.9999999999973376,2.3075653642569363E-06,0]}}
{"ts":"2026-10-21T12:05:06.1453167-05:00","record":{"event":"direct_weld_member","element_id":1249629,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.1476316-05:00","record":{"event":"direct_weld_member","element_id":1250263,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.4587760-05:00","record":{"event":"direct_weld_member","element_id":1250264,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.4610897-05:00","record":{"event":"direct_weld_member","element_id":1250265,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.7425071-05:00","record":{"event":"end_plate_built","first_segment":1245537,"second_segment":
1245535,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653639020113E-06,-0]}}
{"ts":"2026-10-21T12:05:06.9479232-05:00","record":{"event":"direct_weld_member","element_id":1250266,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.9506282-05:00","record":{"event":"direct_weld_member","element_id":1250269,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.2288646-05:00","record":{"event":"direct_weld_member","element_id":1250270,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.2310178-05:00","record":{"event":"direct_weld_member","element_id":1250271,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.5493496-05:00","record":{"event":"end_plate_built","first_segment":1245539,"second_segment":
1245537,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.307565366741405E-06,-0]}}
{"ts":"2026-10-21T12:05:07.7375921-05:00","record":{"event":"direct_weld_member","element_id":1250272,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.7402927-05:00","record":{"event":"direct_weld_member","element_id":1250275,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.0387983-05:00","record":{"event":"direct_weld_member","element_id":1250276,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.0410683-05:00","record":{"event":"direct_weld_member","element_id":1250277,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.2859956-05:00","record":{"event":"end_plate_built","first_segment":1245539,"second_segment":
1245541,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[-0.9999999999973376,2.307565366741405E-06,0]}}
{"ts":"2026-10-21T12:05:08.4585273-05:00","record":{"event":"direct_weld_member","element_id":1250278,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.4608755-05:00","record":{"event":"direct_weld_member","element_id":1250281,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.6968767-05:00","record":{"event":"direct_weld_member","element_id":1250282,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.6992252-05:00","record":{"event":"direct_weld_member","element_id":1250283,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.9944958-05:00","record":{"event":"end_plate_built","first_segment":1245543,"second_segment":
1245541,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653667414034E-06,-0]}}
{"ts":"2026-10-21T12:05:09.1490200-05:00","record":{"event":"direct_weld_member","element_id":1250284,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.1512023-05:00","record":{"event":"direct_weld_member","element_id":1250287,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.4150214-05:00","record":{"event":"direct_weld_member","element_id":1250288,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.4169162-05:00","record":{"event":"direct_weld_member","element_id":1250289,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.6558851-05:00","record":{"event":"direct_weld_member","element_id":1250293,"axis_offset_mm":
0,"setback_ideal_mm":148.75,"axis_setback_mm":182.71,"face_y_mm":-101.6,"footprint_mm":92.97,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.7583861-05:00","record":{"event":"end_plate_built","first_segment":1249516,"second_segment":
1249515,"plate_mm":[230,230,9.525],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[9.6,9.6],"rows":4,"bolts":4,"bolt_lengt
h_mm":44.449999999999996,"tube_footprint_mm":[101.6,101.6],"plate_frame_z":[0.9999999999973377,2.3075653758460542E-06,-
0]}}
{"ts":"2026-10-21T12:05:09.9646791-05:00","record":{"event":"end_plate_built","first_segment":1245545,"second_segment":
1245543,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653684491095E-06,-0]}}
{"ts":"2026-10-21T12:05:10.1323623-05:00","record":{"event":"direct_weld_member","element_id":1250290,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":1245543
}}
{"ts":"2026-10-21T12:05:10.3868275-05:00","record":{"event":"direct_weld_member","element_id":1250294,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-21T12:05:10.3889241-05:00","record":{"event":"direct_weld_member","element_id":1250295,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-21T12:05:10.6675188-05:00","record":{"event":"direct_weld_member","element_id":1250296,"axis_offset_mm":
0,"setback_ideal_mm":145.13,"axis_setback_mm":177.52,"face_y_mm":-101.6,"footprint_mm":90.71,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:11.0653658-05:00","record":{"event":"batch_create","plan_id":"dd6cf93f-76d6-4fe0-81d1-2defaaf4d
a7c","requested":40,"created":40,"updated":0,"failed":0,"skipped":0,"rolled_back":0,"stop_on_error":false,"stopped":fal
se,"undo_entries":"one","connection_ids":["7776b004-6456-49b0-9412-732739e7ffa2","1eb4f785-902a-4afa-b4db-a37072912483"
,"b8dbcd43-f16d-4223-9f05-0b8f6bb17265","9adebe50-be0d-460c-80f0-e205364b358e","4cde5f9a-12da-4896-a5b3-86c78c0f2aa6","
994e38c0-f8ba-48a9-8413-25fa7fc8772d","509c7537-6a6c-42ed-bd99-d1609c3ab93d","2fe4c11f-b38d-473e-bdad-28461f3cd082","83
a64c13-2981-40a5-8654-f91f8c5028a3","dca546c9-2a86-48c6-9cde-3ebad26956e3","6fd5f8f5-b3e3-4397-a975-91708b520f93","ff4b
8802-d659-429d-9bc7-8c869d31b743","549598f9-d868-4e8d-8eb9-8a3e30b309ed","18028706-6c16-40e3-8683-1cdfeb8de515","8a553d
43-1b20-40f7-b170-d56b1f608eda","87927705-79a6-485f-b323-90c2e2f1d46d","e00a1db0-20ef-4b89-9dc2-6e4a711520c6","f5043651
-6388-4a9d-902e-ad7626b794bd","ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","19257157-e88f-489a-9bcf-11022a150a1c","ead78189-a
064-41ff-88a9-dd048dc1d8ae","fc9f689f-e692-473f-aa98-e495a6065f2b","bd60566a-21ff-42a2-9011-a52b900fe49f","ca7a53cd-d95
5-4af9-bb6d-bfb90250f929","81a54a73-e722-4910-86a9-5d816de53c30","73e65525-985a-4da6-9336-35f7f0bd0f09","50272955-17e8-
4bf0-9aab-86437c276b86","09af65a5-2143-45ee-a90c-1ff23f6fc0a5","d1363ed3-8780-44b6-a4e3-6a7dddd76425","cd5b87b1-a9d8-41
97-8bef-321f13f78824","1f68d407-5c99-4b93-9b88-69f432ab1774","b14f334e-1301-4b82-98a4-815a0fd67edb","f3bdf907-6874-4139
-ab66-aee7cb0c5fb0","04df2997-4f17-455f-a22a-1378cf7b1719","d076e320-b6e0-498e-a116-5494fccedc0c","a7419d94-00fc-49a4-b
64d-bb853194fe0b","96418f83-1daf-428b-b94a-f204a821c173","23960cf8-00a2-4839-8bf8-ea8f6d759da0","7235620e-0970-4510-9f4
c-4234b5b40a3e","487354a9-d777-47e1-86fd-508df18873de"],"duration_ms":6282,"summary":"Lote dd6cf93f: 40 conexiones 
creadas (38 con aviso). Una sola entrada de deshacer (Ctrl+Z). 6.3 s."}}
{"ts":"2026-10-21T12:05:11.0673475-05:00","record":{"event":"ribbon_batch_create","plan_id":"dd6cf93f-76d6-4fe0-81d1-2d
efaaf4da7c","requested":40,"created":40,"failed":0,"skipped":0,"undo_entries":"one","duration_ms":6282}}



```

## 12-9 validate fixture C

```text
== conn/validate -> HTTP 200 en 433 ms ==
{
    "data":  {
                 "errors_count":  0,
                 "warnings_count":  0,
                 "bolt_stacks":  [
                                     {
                                         "member_element_id":  1245531,
                                         "bolt_length_mm":  50.799999999999997,
                                         "length_source":  "computed_from_grip",
                                         "plates":  2,
                                         "grip_mm":  25.399999999999999,
                                         "gap_mm":  0,
                                         "bolts":  12
                                     }
                                 ],
                 "validation_token":  "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8",
                 "is_valid":  true,
                 "calculated_values":  {
                                           "origin_mm":  [
                                                             -4180.5,
                                                             17204.200000000001,
                                                             19933
                                                         ],
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "axis_distance_mm":  0,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ],
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "chord_direction_reversed":  true
                                       }
             },
    "meta":  {
                 "duration_ms":  9,
                 "operation":  "validate",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 validate fixture K

```text
== conn/validate -> HTTP 200 en 21 ms ==
{
    "data":  {
                 "errors_count":  0,
                 "warnings_count":  2,
                 "bolt_stacks":  [

                                 ],
                 "validation_token":  "c3a605114cd7ef2d32bb9e785be1e7399ebd63fda4740e74f4ada2b72e577713",
                 "is_valid":  true,
                 "calculated_values":  {
                                           "origin_mm":  [
                                                             -9278.1000000000004,
                                                             -17195.700000000001,
                                                             19933
                                                         ],
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "axis_distance_mm":  0.089999999999999997,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ],
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "chord_direction_reversed":  true
                                       }
             },
    "meta":  {
                 "duration_ms":  7,
                 "operation":  "validate",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [
                     {
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "message":  "El eje del miembro [1249631] corta el cordón a -27.4 mm del punto de trabajo (excentricidad del nudo 34.0 mm admitida hasta 101.6 mm). El JSON declara axis_offset_mm = -17.0 y el modelo mide -27.4. Cada barra se recorta y se suelda sobre su propio eje.",
                         "code":  "MEMBER_ECCENTRIC",
                         "path":  "members[0].axis_offset_mm"
                     },
                     {
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "message":  "El eje del miembro [1249632] corta el cordón a 27.4 mm del punto de trabajo (excentricidad del nudo 34.0 mm admitida hasta 101.6 mm). El JSON declara axis_offset_mm = 17.0 y el modelo mide 27.4. Cada barra se recorta y se suelda sobre su propio eje.",
                         "code":  "MEMBER_ECCENTRIC",
                         "path":  "members[1].axis_offset_mm"
                     }
                 ]
}

```

## 12-9 batch_plan N9 (4 elementos, mark false)

```text
== conn/batch_plan -> HTTP 200 en 84 ms ==
{
    "data":  {
                 "updated_utc":  "2026-10-21T17:08:09.4499260Z",
                 "selection_expansion":  null,
                 "created_count":  0,
                 "marked_view_id":  null,
                 "hidden_text":  "4 barras sueltas",
                 "plan_id":  "50f869f9-2b1f-4cda-8b42-d8f390b5568a",
                 "nodes":  [
                               {
                                   "color_name":  null,
                                   "eccentricity_mm":  0,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "max_deviation_deg":  null,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  null,
                                   "status":  "untyped",
                                   "advice":  "No es un nudo: nada que hacer",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1245531
                                                          ],
                                   "template_name":  null,
                                   "attempts":  [

                                                ],
                                   "kind":  "node",
                                   "template_id":  null,
                                   "orientation":  null,
                                   "existing_connection_id":  null,
                                   "visible_by_default":  false,
                                   "warnings":  [

                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N1",
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "chord_element_id":  0,
                                   "match":  null,
                                   "errors_count":  0,
                                   "splice_element_id":  null,
                                   "chord_type_name":  null,
                                   "validation_token":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1245531
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [

                                               ],
                                   "connection_type":  null,
                                   "is_valid":  false,
                                   "members":  [

                                               ],
                                   "spec":  null,
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         -14656,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "warnings_count":  0,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               },
                               {
                                   "color_name":  null,
                                   "eccentricity_mm":  0,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "max_deviation_deg":  null,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  null,
                                   "status":  "untyped",
                                   "advice":  "No es un nudo: nada que hacer",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1251056
                                                          ],
                                   "template_name":  null,
                                   "attempts":  [

                                                ],
                                   "kind":  "node",
                                   "template_id":  null,
                                   "orientation":  null,
                                   "existing_connection_id":  null,
                                   "visible_by_default":  false,
                                   "warnings":  [

                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N2",
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "chord_element_id":  0,
                                   "match":  null,
                                   "errors_count":  0,
                                   "splice_element_id":  null,
                                   "chord_type_name":  null,
                                   "validation_token":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1251056
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [

                                               ],
                                   "connection_type":  null,
                                   "is_valid":  false,
                                   "members":  [

                                               ],
                                   "spec":  null,
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         -6745,
                                                         17204.299999999999,
                                                         17418.700000000001
                                                     ],
                                   "warnings_count":  0,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               },
                               {
                                   "color_name":  "ambar",
                                   "eccentricity_mm":  0,
                                   "status_detail":  null,
                                   "max_deviation_deg":  0.55000000000000004,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "status":  "ready",
                                   "advice":  "Sobre el empalme N3E: cada diagonal se suelda al tramo al que llega; se crea después del empalme",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1251049,
                                                              1251056
                                                          ],
                                   "template_name":  "Nudo K cordon superior",
                                   "attempts":  [

                                                ],
                                   "kind":  "node",
                                   "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                   "orientation":  "same",
                                   "existing_connection_id":  null,
                                   "visible_by_default":  true,
                                   "warnings":  [
                                                    {
                                                        "hint":  "No hay que corregir nada; el empalme es el nudo N3E.",
                                                        "message":  "El nudo en K está sobre el empalme de los tramos 1245530 y 1245531: cada diagonal se suelda al tramo al que llega su eje y la conexión se crea después del empalme (N3E).",
                                                        "code":  "NODE_AT_SPLICE",
                                                        "path":  "nodes[N3].splice_element_id"
                                                    }
                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N3",
                                   "status_text":  "▲ Listo con aviso",
                                   "chord_element_id":  1245530,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "matched_count":  2,
                                                 "orientation":  "same",
                                                 "score_deg":  0.60999999999999999,
                                                 "assignments":  [
                                                                     {
                                                                         "model_angle_deg":  -135.56999999999999,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "side":  "-Y",
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251056,
                                                                         "deviation_deg":  0.059999999999999998,
                                                                         "template_profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "slot":  0
                                                                     },
                                                                     {
                                                                         "model_angle_deg":  -44.450000000000003,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "side":  "-Y",
                                                                         "template_angle_deg":  -45,
                                                                         "element_id":  1251049,
                                                                         "deviation_deg":  0.55000000000000004,
                                                                         "template_profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "slot":  1
                                                                     }
                                                                 ],
                                                 "is_complete":  true,
                                                 "max_deviation_deg":  0.55000000000000004,
                                                 "description":  "same: ranura 0 (diagonal -135.6°) → barra 1251056 (-135.6°, desvío 0.1°); ranura 1 (diagonal -45.0°) → barra 1251049 (-44.4°, desvío 0.6°)"
                                             },
                                   "errors_count":  0,
                                   "splice_element_id":  1245531,
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "validation_token":  "23afd1a7e57cf02c73a8f3e4f262f4a7a0b51c2990c3a00c8649da4a0aa82232",
                                   "signature":  "2 -Y (-44, -136)",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1245530,
                                                       1245531,
                                                       1251049,
                                                       1251056
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [
                                                   {
                                                       "key":  "exclude",
                                                       "label":  "Excluir"
                                                   }
                                               ],
                                   "connection_type":  "direct_weld",
                                   "is_valid":  true,
                                   "members":  [
                                                   {
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  0,
                                                       "angle_deg":  -44.399999999999999,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  null,
                                                       "side":  "-Y",
                                                       "end_gap_mm":  23.800000000000001,
                                                       "element_id":  1251049
                                                   },
                                                   {
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  0,
                                                       "angle_deg":  -135.59999999999999,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  1245531,
                                                       "side":  "-Y",
                                                       "end_gap_mm":  23.699999999999999,
                                                       "element_id":  1251056
                                                   }
                                               ],
                                   "spec":  {
                                                "members":  [
                                                                {
                                                                    "splice_hint":  {
                                                                                        "detail":  "B",
                                                                                        "offset_from_chord_face_mm":  330
                                                                                    },
                                                                    "weld":  {
                                                                                 "type":  "fillet",
                                                                                 "size_mm":  5,
                                                                                 "all_around":  true
                                                                             },
                                                                    "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                    "end_gap_mm":  0,
                                                                    "role":  "diagonal",
                                                                    "element_id":  1251056,
                                                                    "end_cut":  "to_chord_face",
                                                                    "expected_angle_deg":  44.399999999999999
                                                                },
                                                                {
                                                                    "splice_hint":  {
                                                                                        "detail":  "B",
                                                                                        "offset_from_chord_face_mm":  330
                                                                                    },
                                                                    "weld":  {
                                                                                 "type":  "fillet",
                                                                                 "size_mm":  5,
                                                                                 "all_around":  true
                                                                             },
                                                                    "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                    "end_gap_mm":  0,
                                                                    "role":  "diagonal",
                                                                    "element_id":  1251049,
                                                                    "end_cut":  "to_chord_face",
                                                                    "expected_angle_deg":  44.399999999999999
                                                                }
                                                            ],
                                                "source":  {
                                                               "drawing":  "Especificación de arranque (Conectar)",
                                                               "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                                               "batch_id":  "50f869f9-2b1f-4cda-8b42-d8f390b5568a"
                                                           },
                                                "connection_type":  "direct_weld",
                                                "node":  {
                                                             "element_ids":  [
                                                                                 1245530,
                                                                                 1251056,
                                                                                 1251049
                                                                             ]
                                                         },
                                                "chord":  {
                                                              "face":  "auto",
                                                              "profile":  "HSS12X8X1/2",
                                                              "continuous":  true,
                                                              "element_id":  1245530
                                                          },
                                                "uncertain_fields":  [

                                                                     ],
                                                "spec_version":  "1.0",
                                                "dimension_chains":  [

                                                                     ]
                                            },
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         -4180.1999999999998,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "warnings_count":  1,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               },
                               {
                                   "color_name":  "gris",
                                   "eccentricity_mm":  0,
                                   "status_detail":  "Ya tiene la conexión f370914e-94a4-441f-8bdd-c4b038d3c309 (se salta; replace_existing: true para rehacerla).",
                                   "max_deviation_deg":  null,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  [
                                                     140,
                                                     140,
                                                     140
                                                 ],
                                   "status":  "already_connected",
                                   "advice":  "Se salta; Incluir (rehacer) replanifica con 'rehacer existentes' para rehacerla con el lote",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1245531
                                                          ],
                                   "template_name":  null,
                                   "attempts":  [

                                                ],
                                   "kind":  "splice",
                                   "template_id":  null,
                                   "orientation":  null,
                                   "existing_connection_id":  "f370914e-94a4-441f-8bdd-c4b038d3c309",
                                   "visible_by_default":  true,
                                   "warnings":  [

                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N3E",
                                   "status_text":  "◌ Ya tiene conexión",
                                   "chord_element_id":  1245530,
                                   "match":  null,
                                   "errors_count":  0,
                                   "splice_element_id":  null,
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "validation_token":  null,
                                   "signature":  "1 -Y (-180)",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1245530,
                                                       1245531
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [
                                                   {
                                                       "key":  "include_replace",
                                                       "label":  "Incluir (rehacer)"
                                                   }
                                               ],
                                   "connection_type":  null,
                                   "is_valid":  false,
                                   "members":  [
                                                   {
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  0,
                                                       "angle_deg":  -180,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "chord_element_id":  null,
                                                       "side":  "-Y",
                                                       "end_gap_mm":  0.40000000000000002,
                                                       "element_id":  1245531
                                                   }
                                               ],
                                   "spec":  null,
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         -4180.5,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "warnings_count":  0,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               },
                               {
                                   "color_name":  null,
                                   "eccentricity_mm":  0,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "max_deviation_deg":  null,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  null,
                                   "status":  "untyped",
                                   "advice":  "No es un nudo: nada que hacer",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1251049
                                                          ],
                                   "template_name":  null,
                                   "attempts":  [

                                                ],
                                   "kind":  "node",
                                   "template_id":  null,
                                   "orientation":  null,
                                   "existing_connection_id":  null,
                                   "visible_by_default":  false,
                                   "warnings":  [

                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N4",
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "chord_element_id":  0,
                                   "match":  null,
                                   "errors_count":  0,
                                   "splice_element_id":  null,
                                   "chord_type_name":  null,
                                   "validation_token":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1251049
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [

                                               ],
                                   "connection_type":  null,
                                   "is_valid":  false,
                                   "members":  [

                                               ],
                                   "spec":  null,
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         -1632.3000000000000,
                                                         17204.299999999999,
                                                         17433.700000000001
                                                     ],
                                   "warnings_count":  0,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               },
                               {
                                   "color_name":  null,
                                   "eccentricity_mm":  0,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "max_deviation_deg":  null,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "color_rgb":  null,
                                   "status":  "untyped",
                                   "advice":  "No es un nudo: nada que hacer",
                                   "is_manual":  false,
                                   "member_element_ids":  [
                                                              1245530
                                                          ],
                                   "template_name":  null,
                                   "attempts":  [

                                                ],
                                   "kind":  "node",
                                   "template_id":  null,
                                   "orientation":  null,
                                   "existing_connection_id":  null,
                                   "visible_by_default":  false,
                                   "warnings":  [

                                                ],
                                   "ghost_element_id":  null,
                                   "is_marked":  false,
                                   "marker_element_id":  null,
                                   "name":  "N5",
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "chord_element_id":  0,
                                   "match":  null,
                                   "errors_count":  0,
                                   "splice_element_id":  null,
                                   "chord_type_name":  null,
                                   "validation_token":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "existing_batch_id":  null,
                                   "starter":  false,
                                   "element_ids":  [
                                                       1245530
                                                   ],
                                   "through_element_ids":  [

                                                           ],
                                   "actions":  [

                                               ],
                                   "connection_type":  null,
                                   "is_valid":  false,
                                   "members":  [

                                               ],
                                   "spec":  null,
                                   "replaces_existing":  false,
                                   "label_index":  null,
                                   "work_point_mm":  [
                                                         6069.8999999999996,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "warnings_count":  0,
                                   "chord_continuous":  false,
                                   "is_mirrored":  false,
                                   "has_spec_override":  false
                               }
                           ],
                 "visible_count":  2,
                 "summary_text":  "Se creará 1 conexión con Nudo K cordon superior (1 igual, 0 en espejo). 1 nudo en K está sobre un empalme. 1 ya tiene conexión. Ocultos: 4 barras sueltas.",
                 "created_in_batch_count":  0,
                 "is_marked":  false,
                 "marks":  {
                               "element_count":  0,
                               "ghost_count":  0,
                               "marker_element_ids":  [

                                                      ]
                           },
                 "creatable_count":  1,
                 "unused_element_ids":  [

                                        ],
                 "description":  "6 nudo(s): 4 untyped, 1 already_connected, 1 ready.",
                 "ready_count":  1,
                 "has_batch_connections":  false,
                 "last_report":  null,
                 "label_indices":  {

                                   },
                 "label_view_id":  null,
                 "created_utc":  "2026-10-21T17:08:09.4499032Z",
                 "summary":  {
                                 "untyped":  4,
                                 "ready":  1,
                                 "already_connected":  1
                             },
                 "templates":  {
                                   "91adac27-d196-4c98-88db-6579aa991be4":  "Nudo K cordon superior",
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "labels":  {
                                "count":  0,
                                "view_id":  null
                            },
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "failed_count":  0,
                 "selection_count":  4,
                 "overrides":  {
                                   "exclude":  [

                                               ],
                                   "merge":  [

                                             ],
                                   "spec":  {

                                            },
                                   "add_node":  {

                                                },
                                   "replace_existing":  false,
                                   "add_member":  {

                                                  },
                                   "chord":  {

                                             },
                                   "template":  {

                                                },
                                   "split":  {

                                             },
                                   "remove_member":  {

                                                     }
                               }
             },
    "meta":  {
                 "duration_ms":  34,
                 "operation":  "batch_plan",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 list antes de borrar

```text
== conn/list -> HTTP 200 en 23 ms ==
{
    "data":  {
                 "connections":  [
                                     {
                                         "connection_id":  "5b5fd89b-9161-4560-92d7-4209da8a377e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T16:47:18.3273069Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "f370914e-94a4-441f-8bdd-c4b038d3c309",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T16:54:34.1395681Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "7776b004-6456-49b0-9412-732739e7ffa2",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:04.8345846Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "1eb4f785-902a-4afa-b4db-a37072912483",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.0148473Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "b8dbcd43-f16d-4223-9f05-0b8f6bb17265",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:05.1844600Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "9adebe50-be0d-460c-80f0-e205364b358e",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:05.3944245Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "4cde5f9a-12da-4896-a5b3-86c78c0f2aa6",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.5227898Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "994e38c0-f8ba-48a9-8413-25fa7fc8772d",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:05.6596395Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "509c7537-6a6c-42ed-bd99-d1609c3ab93d",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:05.8172818Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "2fe4c11f-b38d-473e-bdad-28461f3cd082",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:05.9797078Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "83a64c13-2981-40a5-8654-f91f8c5028a3",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.1578784Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "dca546c9-2a86-48c6-9cde-3ebad26956e3",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:06.3172126Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "6fd5f8f5-b3e3-4397-a975-91708b520f93",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.4703540Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "ff4b8802-d659-429d-9bc7-8c869d31b743",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:06.5918868Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "549598f9-d868-4e8d-8eb9-8a3e30b309ed",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:06.7585380Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "18028706-6c16-40e3-8683-1cdfeb8de515",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:06.9614880Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "8a553d43-1b20-40f7-b170-d56b1f608eda",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.1161568Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "87927705-79a6-485f-b323-90c2e2f1d46d",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:07.2403044Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "e00a1db0-20ef-4b89-9dc2-6e4a711520c6",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.3843092Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "f5043651-6388-4a9d-902e-ad7626b794bd",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:07.5648113Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "ab9a5c5d-a663-4fbb-8178-e1e91f9707fb",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:07.7509562Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "19257157-e88f-489a-9bcf-11022a150a1c",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:07.8838815Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "ead78189-a064-41ff-88a9-dd048dc1d8ae",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.0505508Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "fc9f689f-e692-473f-aa98-e495a6065f2b",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.1666504Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "bd60566a-21ff-42a2-9011-a52b900fe49f",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:08.2991691Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "ca7a53cd-d955-4af9-bb6d-bfb90250f929",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.4703544Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "81a54a73-e722-4910-86a9-5d816de53c30",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.5896796Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "73e65525-985a-4da6-9336-35f7f0bd0f09",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:08.7360938Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "50272955-17e8-4bf0-9aab-86437c276b86",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:08.8640707Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "09af65a5-2143-45ee-a90c-1ff23f6fc0a5",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.0077233Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "d1363ed3-8780-44b6-a4e3-6a7dddd76425",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.1605167Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "cd5b87b1-a9d8-4197-8bef-321f13f78824",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:09.2885765Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "1f68d407-5c99-4b93-9b88-69f432ab1774",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.4248833Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "b14f334e-1301-4b82-98a4-815a0fd67edb",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:09.5561453Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "f3bdf907-6874-4139-ab66-aee7cb0c5fb0",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:09.6644157Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "04df2997-4f17-455f-a22a-1378cf7b1719",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.7707784Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "d076e320-b6e0-498e-a116-5494fccedc0c",
                                         "created_elements_count":  14,
                                         "connection_type":  "end_plate",
                                         "created_utc":  "2026-10-21T17:05:09.9789596Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "a7419d94-00fc-49a4-b64d-bb853194fe0b",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.1416344Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     },
                                     {
                                         "connection_id":  "96418f83-1daf-428b-b94a-f204a821c173",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:10.2712878Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "23960cf8-00a2-4839-8bf8-ea8f6d759da0",
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.3982602Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     },
                                     {
                                         "connection_id":  "7235620e-0970-4510-9f4c-4234b5b40a3e",
                                         "created_elements_count":  9,
                                         "connection_type":  "gusset_node",
                                         "created_utc":  "2026-10-21T17:05:10.5217924Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                     },
                                     {
                                         "connection_id":  "487354a9-d777-47e1-86fd-508df18873de",
                                         "created_elements_count":  1,
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-21T17:05:10.6777728Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                                         "spec_version":  "1.0",
                                         "template_id":  null
                                     }
                                 ],
                 "batches":  {
                                 "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c":  40
                             },
                 "batch_id":  null,
                 "connections_count":  42,
                 "total_count":  42
             },
    "meta":  {
                 "duration_ms":  5,
                 "operation":  "list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 batch_delete

```text
== conn/batch_delete -> HTTP 200 en 3799 ms ==
{
    "data":  {
                 "stop_on_error":  false,
                 "plan_creatable_count":  40,
                 "deleted_elements_count":  285,
                 "duration_ms":  3722,
                 "rolled_back_count":  0,
                 "created_count":  0,
                 "updated_count":  0,
                 "plan_summary":  {
                                      "no_match":  8,
                                      "untyped":  17,
                                      "ready":  40,
                                      "already_connected":  1
                                  },
                 "stopped":  false,
                 "deleted_count":  40,
                 "plan_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                 "connection_ids":  [

                                    ],
                 "batch_id":  "dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c",
                 "nodes":  [
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "7776b004-6456-49b0-9412-732739e7ffa2",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N4",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  158,
                                   "description":  "N4: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "1eb4f785-902a-4afa-b4db-a37072912483",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N6",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  132,
                                   "description":  "N6: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "b8dbcd43-f16d-4223-9f05-0b8f6bb17265",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N9E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  108,
                                   "description":  "N9E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "9adebe50-be0d-460c-80f0-e205364b358e",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N9",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  75,
                                   "description":  "N9: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "4cde5f9a-12da-4896-a5b3-86c78c0f2aa6",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N11",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  102,
                                   "description":  "N11: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "994e38c0-f8ba-48a9-8413-25fa7fc8772d",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N12",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  82,
                                   "description":  "N12: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "509c7537-6a6c-42ed-bd99-d1609c3ab93d",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N13",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  91,
                                   "description":  "N13: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "2fe4c11f-b38d-473e-bdad-28461f3cd082",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N16E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  107,
                                   "description":  "N16E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "83a64c13-2981-40a5-8654-f91f8c5028a3",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N16",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  92,
                                   "description":  "N16: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "dca546c9-2a86-48c6-9cde-3ebad26956e3",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N18",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  94,
                                   "description":  "N18: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "6fd5f8f5-b3e3-4397-a975-91708b520f93",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N19",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  77,
                                   "description":  "N19: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "ff4b8802-d659-429d-9bc7-8c869d31b743",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N20",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  91,
                                   "description":  "N20: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "549598f9-d868-4e8d-8eb9-8a3e30b309ed",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N23E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  105,
                                   "description":  "N23E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "18028706-6c16-40e3-8683-1cdfeb8de515",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N23",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  88,
                                   "description":  "N23: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "8a553d43-1b20-40f7-b170-d56b1f608eda",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N25",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  86,
                                   "description":  "N25: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "87927705-79a6-485f-b323-90c2e2f1d46d",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N26",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  82,
                                   "description":  "N26: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "e00a1db0-20ef-4b89-9dc2-6e4a711520c6",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N27",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  88,
                                   "description":  "N27: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "f5043651-6388-4a9d-902e-ad7626b794bd",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N30E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  111,
                                   "description":  "N30E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "ab9a5c5d-a663-4fbb-8178-e1e91f9707fb",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N30",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  80,
                                   "description":  "N30: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "19257157-e88f-489a-9bcf-11022a150a1c",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N32",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  85,
                                   "description":  "N32: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "ead78189-a064-41ff-88a9-dd048dc1d8ae",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N33",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  80,
                                   "description":  "N33: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "fc9f689f-e692-473f-aa98-e495a6065f2b",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N34",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  83,
                                   "description":  "N34: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "bd60566a-21ff-42a2-9011-a52b900fe49f",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N37E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  75,
                                   "description":  "N37E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "ca7a53cd-d955-4af9-bb6d-bfb90250f929",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N37",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  97,
                                   "description":  "N37: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "81a54a73-e722-4910-86a9-5d816de53c30",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N39",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  99,
                                   "description":  "N39: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "73e65525-985a-4da6-9336-35f7f0bd0f09",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N40",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  95,
                                   "description":  "N40: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "50272955-17e8-4bf0-9aab-86437c276b86",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N41",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  105,
                                   "description":  "N41: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "09af65a5-2143-45ee-a90c-1ff23f6fc0a5",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N44E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  120,
                                   "description":  "N44E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "d1363ed3-8780-44b6-a4e3-6a7dddd76425",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N44",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  82,
                                   "description":  "N44: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "cd5b87b1-a9d8-4197-8bef-321f13f78824",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N46",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  105,
                                   "description":  "N46: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "1f68d407-5c99-4b93-9b88-69f432ab1774",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N47",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  83,
                                   "description":  "N47: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "b14f334e-1301-4b82-98a4-815a0fd67edb",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N48",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  91,
                                   "description":  "N48: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "f3bdf907-6874-4139-ab66-aee7cb0c5fb0",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N49",
                                   "restored_members_count":  1,
                                   "elements_count":  1,
                                   "duration_ms":  81,
                                   "description":  "N49: borrada (1 elementos, 1 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "04df2997-4f17-455f-a22a-1378cf7b1719",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N51",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  76,
                                   "description":  "N51: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "d076e320-b6e0-498e-a116-5494fccedc0c",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N52E",
                                   "restored_members_count":  2,
                                   "elements_count":  14,
                                   "duration_ms":  73,
                                   "description":  "N52E: borrada (14 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "a7419d94-00fc-49a4-b64d-bb853194fe0b",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N52",
                                   "restored_members_count":  1,
                                   "elements_count":  1,
                                   "duration_ms":  59,
                                   "description":  "N52: borrada (1 elementos, 1 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "96418f83-1daf-428b-b94a-f204a821c173",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N53",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  92,
                                   "description":  "N53: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "23960cf8-00a2-4839-8bf8-ea8f6d759da0",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N54",
                                   "restored_members_count":  2,
                                   "elements_count":  2,
                                   "duration_ms":  104,
                                   "description":  "N54: borrada (2 elementos, 2 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "7235620e-0970-4510-9f4c-4234b5b40a3e",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N55",
                                   "restored_members_count":  3,
                                   "elements_count":  9,
                                   "duration_ms":  93,
                                   "description":  "N55: borrada (9 elementos, 3 barras restauradas)"
                               },
                               {
                                   "outcome":  "deleted",
                                   "orientation":  null,
                                   "template_name":  null,
                                   "connection_id":  "487354a9-d777-47e1-86fd-508df18873de",
                                   "reason":  null,
                                   "errors":  [

                                              ],
                                   "warnings":  [

                                                ],
                                   "spec":  null,
                                   "node":  "N58",
                                   "restored_members_count":  1,
                                   "elements_count":  1,
                                   "duration_ms":  59,
                                   "description":  "N58: borrada (1 elementos, 1 barras restauradas)"
                               }
                           ],
                 "summary_text":  "Lote dd6cf93f: 40 conexiones borradas (285 elementos, 93 barras restauradas). Una sola entrada de deshacer (Ctrl+Z). 3.7 s.",
                 "restored_members_count":  93,
                 "started_utc":  "2026-10-21T17:08:45.1982562Z",
                 "operation":  "batch_delete",
                 "stopped_at":  null,
                 "with_warnings_count":  0,
                 "undo_entries":  "one",
                 "finished_utc":  "2026-10-21T17:08:48.9210731Z",
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "failed_count":  0,
                 "skipped_count":  0,
                 "plan_summary_text":  "Se crearán 40 conexiones: 29 con 2 plantillas (Nudo tipico Detalle D: 16, Nudo K cordon superior: 13) (21 iguales, 8 en espejo), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I escalado), 3 soldaduras directas (arranque). 14 avisan de perfil distinto. 1 avisa de excentricidad. 7 nudos en K están sobre un empalme. 1 ya tiene conexión. Ocultos: 8 sin cordón, 17 barras sueltas."
             },
    "meta":  {
                 "duration_ms":  3736,
                 "operation":  "batch_delete",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 delete nudo K

```text
== conn/delete -> HTTP 200 en 346 ms ==
{
    "data":  {
                 "deleted_connection_id":  "5b5fd89b-9161-4560-92d7-4209da8a377e",
                 "deleted_elements_count":  2,
                 "restored_members_count":  2
             },
    "meta":  {
                 "duration_ms":  86,
                 "operation":  "delete",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [
                     {
                         "hint":  null,
                         "message":  "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Los elementos se borran directamente (Document.Delete) y las barras se restauran igual.",
                         "code":  "REVIT_WARNING",
                         "path":  null
                     }
                 ]
}

```

## 12-9 delete empalme

```text
== conn/delete -> HTTP 200 en 125 ms ==
{
    "data":  {
                 "deleted_connection_id":  "f370914e-94a4-441f-8bdd-c4b038d3c309",
                 "deleted_elements_count":  14,
                 "restored_members_count":  2
             },
    "meta":  {
                 "duration_ms":  91,
                 "operation":  "delete",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [
                     {
                         "hint":  null,
                         "message":  "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Los elementos se borran directamente (Document.Delete) y las barras se restauran igual.",
                         "code":  "REVIT_WARNING",
                         "path":  null
                     }
                 ]
}

```

## 12-9 sondeo 12 restos

```text
== 12-fase3-borrar.py -> HTTP 200 en 112 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 12-9 sondeo 13 restos

```text
== 13-limpiar-fase1.py -> HTTP 200 en 59 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12-9 sondeo 17 final

```text
== 17-marcas-plan.py -> HTTP 200 en 18258 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 1
   [1323006] N5 | dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c:N5 | N5 · MotorConexiones plan dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c; view=1245519; ids=1245533,1249631,1249632; already_connected; gris
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1323554] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: fallo (Internal error)
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 12-9 sondeo 21 final

```text
== 21-etiquetas-addin.py -> HTTP 200 en 613 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 1: 10
   control 10: visible=? ('TemporaryGraphicsManager' object has no attribute 'IsVisible')
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 75 BMP
   etiqueta-ambar-11-sel.bmp (3126 bytes)
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-16.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-23.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-30.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 1 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 12-9 ping antes del puente

```text
== conn/ping -> HTTP 200 en 594 ms ==
{
    "data":  {
                 "addin_version":  "0.12.0",
                 "operations":  [
                                    "batch_create",
                                    "batch_delete",
                                    "batch_plan",
                                    "batch_plan_discard",
                                    "batch_plan_get",
                                    "catalog_apply",
                                    "catalog_delete",
                                    "catalog_get",
                                    "catalog_list",
                                    "catalog_save",
                                    "create",
                                    "delete",
                                    "find_profile",
                                    "get",
                                    "guide",
                                    "list",
                                    "node_info",
                                    "ping",
                                    "preview",
                                    "schema",
                                    "types",
                                    "update",
                                    "validate"
                                ],
                 "document":  {
                                  "is_workshared":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false
                              },
                 "dotnet":  {
                                "load_context":  "Default",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12"
                            },
                 "revit":  {
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "language":  "English_USA"
                           },
                 "has_uidocument":  true,
                 "backend":  "advancesteel",
                 "spec_version":  "1.0"
             },
    "meta":  {
                 "duration_ms":  2,
                 "operation":  "ping",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         44200



```

## 12-9 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.12.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"data":{"addin_version":"0.12.0","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"document":{"is_workshared":false,"title":"HANGAR_PRUEBA_sondeo","is_family":false,"is_modifiable":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_read_only":false},"dotnet":{"load_context":"Default","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12"},"revit":{"sub_version_number":"2027.2","version_build":"27.2.0.39","version_name":"Autodesk Revit 2027","version_number":"2027","language":"English_USA"},"has_uidocument":true,"backend":"advancesteel","spec_version":"1.0"},"meta":{"duration_ms":6,"operation":"ping","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 31658 caracteres
Cuerpo:
{"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"data":{"connection_types":[{"type_name":"direct_weld","description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n."},{"type_name":"end_plate","description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones)."},{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"meta":{"duration_ms":0,"operation":"types","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"data":{"json_schema":{"required":["spec_version","connection_type","node","chord","gusset","members"],"additionalProperties":false,"title":"GussetNodeConnectionSpec","type":"object","properties":{"members":{"type":"array","items":{"type":"object","additionalProperties":false,"required":["element_id","role","end_setback_mm","attachment"],"properties":{"attachment":{"type":"object","additionalProperties":false,"required":["type"],"properties":{"weld":{"type":"object","additionalProperties":false,"required":["type","size_mm"],"properties":{"type":{"type":"string","enum":["fillet"]},"size_mm":{"type":"number","minimum":1.0},"all_around":{"type":"boolean"}}},"type":{"type":"string","enum":["welded_slot","bolted_knife_plate"]},"weld_plate_to_member":{"type":"object","additionalProperties":false,"required":["type","size_mm"],"properties":{"type":{"type":"string","enum":["fillet"]},"size_mm":{"type":"number","minimum":1.0},"all_around":{"type":"boolean"}}},"plate":{"type":"object","additionalProperties":false,"required":["thickness_mm","length_mm","width_mm","insertion_mm"],"properties":{"insertion_mm":{"type":"number","minimum":0.0},"width_mm":{"type":"number","minimum":1.0},"thickness_label":{"type":"string"},"length_mm":{"type":"number","minimum":1.0},"thickness_mm":{"type":"number","minimum":1.0},"gusset_face":{"description":"Cara de la cartela (sistema local del nudo) sobre la que apoya la placa cuchilla; por defecto +z. La placa se solapa con la cartela y los pernos atraviesa ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"schema","addin_version":"0.12.0"},"errors":[{"hint":"Tipos disponibles: direct_weld, end_plate, gusset_node","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","code":"UNKNOWN_OPERATION","path":"type"}],"ok":false,"warnings":[]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"data": {"matched_count": 1, "total_profiles_in_model": 29, "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "suggestions": [], "query": "HSS2-1/2X2-1/2X3/16"}, "meta": {"duration_ms": 9, "operation": "find_profile", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"data": {"origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "members": [{"material": "Steel ASTM A500, Grade B, Rectangular and Square", "axis_offset_mm": 0, "wall_mm": 5.9199999999999999, "element_id": 1249510, "length_mm": 9960.2999999999993, "height_mm": 76.200000000000003, "node_end": 1, "angle_in_plane_deg": 0, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "width_mm": 76.200000000000003, "end_gap_mm": 0, "structural_type": "Beam", "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "family": "HSS-Hollow Structural Section", "slope_deg": 0, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "side": "chord", "reaches_node": true, "is_chord": true, "type": "HSS3X3X1/4", "angle_to_chord_deg": 0}, {"material": "Material IFC (190-40-140)", "axis_offset_mm": 1.7000000000000000, "wall_mm": 0, "element_id": 1249630, "length_mm": 3568, "height_mm": 63.5, "node_end": 1, "angle_in_plane_deg": 136.90000000000001, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "width_mm": 63.5, "end_gap_mm": 84.900000000000006, "structural_type": "Beam", "axis_cut_mm": [-11867.700000000001, -17195.799999999999, 17423], "family": "HSS2-1-2X2-1-2X3-16 64x64", "slope_deg": 43.079999999999998, "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "side": "+Y", "reaches_node": true, "is_chord": false, "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle_to_chord_deg": 43.100000000000001}, {"material": "Material I ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"data":{"errors_count":0,"warnings_count":1,"bolt_stacks":[{"member_element_id":1249636,"bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip","grip_mm":19.524999999999999,"gusset_face":"+z"}],"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","is_valid":true,"calculated_values":{"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"frame_x":[1,0,0],"frame_y":[0,0,1],"chord_direction_reversed":true}},"meta":{"duration_ms":21,"operation":"validate","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[{"hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg"}]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"meta":{"duration_ms":15,"operation":"validate","addin_version":"0.12.0"},"errors":[{"hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","code":"DIMENSION_CHAIN_MISMATCH","path":"dimension_chains[0].values_mm"}],"ok":false,"warnings":[{"hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"meta":{"duration_ms":26,"operation":"validate","addin_version":"0.12.0"},"errors":[{"hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[0].user_confirmed_value"},{"hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[1].user_confirmed_value"}],"ok":false,"warnings":[{"hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"knife_plates": 1, "first_member_element_id": 1249630, "chord_element_id": 1249510, "weld_lines": 6, "working_point_mm": [-11869.4, -17195.8, 17423], "members_modified": 3, "gusset_plates": 1, "connection_type": "gusset_node", "backend": "advancesteel", "bolts": 4, "dry_run": true}
Cuerpo:
{"data": {"members_to_modify": [{"new_extension_mm": -95.099999999999994, "end": "end", "current_end_distance_mm": 84.900000000000006, "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249630}, {"new_extension_mm": -40.799999999999997, "end": "start", "current_end_distance_mm": 19.199999999999999, "setback_mm": 60, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249631}, {"new_extension_mm": -211.40000000000001, "end": "end", "current_end_distance_mm": 48.600000000000001, "setback_mm": 260, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249636}], "summary": {"knife_plates": 1, "first_member_element_id": 1249630, "chord_element_id": 1249510, "weld_lines": 6, "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "members_modified": 3, "gusset_plates": 1, "connection_type": "gusset_node", "backend": "advancesteel", "bolts": 4, "dry_run": true}, "elements_to_create": [{"width_mm": 565, "thickness_label": "3/8\"", "chord_interface": "through_slot", "vertices_count": 8, "height_mm": 530, "thickness_mm": 9.5250000000000004, "kind": "gusset_plate"}, {"kind": "welded_slot_interfa ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"data":null,"meta":{"duration_ms":1,"operation":"create","addin_version":"0.12.0"},"errors":[{"hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","message":"validation_token es obligatorio para crear una conexi\u00f3n.","code":"VALIDATION_TOKEN_INVALID","path":"validation_token"}],"ok":false,"warnings":[]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"data": {"connections": [], "batches": {}, "batch_id": null, "connections_count": 0, "total_count": 0}, "meta": {"duration_ms": 4, "operation": "list", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":6,"operation":"get","addin_version":"0.12.0"},"errors":[{"hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","path":"connection_id"}],"ok":false,"warnings":[]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":5,"operation":"delete","addin_version":"0.12.0"},"errors":[{"hint":"Verifica los IDs disponibles con conn_list.","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","path":"connection_id"}],"ok":false,"warnings":[]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"no_existe","addin_version":"0.12.0"},"errors":[{"hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","code":"UNKNOWN_OPERATION","path":null}],"ok":false,"warnings":[]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"data":{"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates_count":2,"templates":[{"name":"Nudo K cordon superior","tags":[],"origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","connection_type":"direct_weld","chord_profile":"HSS12X8X1/2","created_utc":"2026-10-21T16:49:07.9104459Z","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","members_count":2,"template_id":"91adac27-d196-4c98-88db-6579aa991be4","origin_document":"HANGAR_PRUEBA_sondeo","description":null},{"name":"Nudo tipico Detalle D","tags":["hangar","cercha","HSS"],"origin_drawing":"Detalle D","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","connection_type":"gusset_node","chord_profile":"HSS3X3X1/4","created_utc":"2026-10-04T22:01:50.5585233Z","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","members_count":3,"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","origin_document":"HANGAR_PRUEBA_sondeo","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"}]},"meta":{"duration_ms":8,"operation":"catalog_list","addin_version":"0.12.0"}, ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=b7461a9f-3537-47f3-a6a2-e0ba0a639b71 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\b7461a9f-3537-47f3-a6a2-e0ba0a639b71.json
Cuerpo:
{"data":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"name":"PRUEBA probar_conexiones","chord_profile":"HSS3X3X1/4","origin":{"drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo","connection_id":null},"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\b7461a9f-3537-47f3-a6a2-e0ba0a639b71.json","members_count":3,"shared_file":null,"template_id":"b7461a9f-3537-47f3-a6a2-e0ba0a639b71","member_pattern":[{"angle_deg":136.91999999999999,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","side":"+Y","role":"diagonal","slot":0},{"angle_deg":44.369999999999997,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","side":"+Y","role":"diagonal","slot":1},{"angle_deg":-135.63000000000000,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","side":"-Y","role":"diagonal","slot":2}]},"meta":{"duration_ms":45,"operation":"catalog_save","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[{"hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg"}]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\b7461a9f-3537-47f3-a6a2-e0ba0a639b71.json","template":{"spec_template":{"members":[{"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","slot":0,"expected_angle_deg":45.0,"end_setback_mm":180.0},{"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","slot":1,"expected_angle_deg":45.0,"end_setback_mm":60.0},{"attachment":{"plate":{"insertion_mm":80.0,"width_mm":140.0,"thickness_label":"PL10","length_mm":170.0,"thickness_mm":10.0},"bolts":{"diameter_label":"5/8\"","columns":2,"edge_mm":40.0,"diameter_mm":15.875,"spacing_mm":60.0,"rows":2,"first_row_from_plate_end_mm":40.0},"type":"bolted_knife_plate","weld_plate_to_member":{"type":"fillet","size_mm":5.0,"all_around":true}},"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","slot":2,"expected_angle_deg":45.0,"end_setback_mm":260.0}],"gusset":{"width_mm":565.0,"thickness_label":"3/8\"","weld_to_chord":{"type":"fillet","size_mm":5.0,"all_around":true},"chord_interface":"through_slot","outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250. ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=924b95a91843...
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","match":{"unassigned_members":[],"matched_count":3,"orientation":"same","score_deg":0.01,"assignments":[{"model_angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":136.91999999999999,"element_id":1249630,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":0},{"model_angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":44.369999999999997,"element_id":1249631,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":1},{"model_angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"-Y","template_angle_deg":-135.63000000000000,"element_id":1249636,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":2}],"unmatched_slots":[],"is_complete":true,"max_deviation_deg":0,"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0)"},"errors_count":0,"warnings_count":0,"spec":{"members":[{"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"profile":"HSS2-1 ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","match":{"unassigned_members":[],"matched_count":3,"orientation":"same","score_deg":0.01,"assignments":[{"model_angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":136.91999999999999,"element_id":1249630,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":0},{"model_angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":44.369999999999997,"element_id":1249631,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":1},{"model_angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"-Y","template_angle_deg":-135.63000000000000,"element_id":1249636,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":2}],"unmatched_slots":[],"is_complete":true,"max_deviation_deg":0,"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0)"},"errors_count":0,"warnings_count":0,"spec":{"members":[{"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"profile":"HSS2-1 ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=f49dce56-56c3-460a-826b-f3dddbed0a42 nudos=6 resumen={'untyped': 5, 'ready': 1} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"data":{"updated_utc":"2026-10-21T17:11:22.4010364Z","selection_expansion":null,"created_count":0,"marked_view_id":null,"hidden_text":"5 barras sueltas","plan_id":"f49dce56-56c3-460a-826b-f3dddbed0a42","nodes":[{"color_name":null,"eccentricity_mm":0,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","max_deviation_deg":null,"created_connection_id":null,"errors":[],"color_rgb":null,"status":"untyped","advice":"No es un nudo: nada que hacer","is_manual":false,"member_element_ids":[1249630],"template_name":null,"attempts":[],"kind":"node","template_id":null,"orientation":null,"existing_connection_id":null,"visible_by_default":false,"warnings":[],"ghost_element_id":null,"is_marked":false,"marker_element_id":null,"name":"N1","status_text":"\u25cb Barra suelta (no es nudo)","chord_element_id":0,"match":null,"errors_count":0,"splice_element_id":null,"chord_type_name":null,"validation_token":null,"signature":"1 barra(s) sin marco","existing_batch_id":null,"starter":false,"element_ids":[1249630],"through_element_ids":[],"actions":[],"connection_type":null,"is_valid":false,"members":[],"spec":null,"replaces_existing":false,"label_index":null,"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"warnings_count":0,"chord_continuous":false,"is_mirrored":false,"has_spec_override":false},{"color_name":null,"eccentricity_mm":0,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","max_deviation_deg":n ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=228067ab1cdd...
Cuerpo:
{"data":{"node":{"color_name":"verde","eccentricity_mm":3.3999999999999999,"status_detail":null,"max_deviation_deg":0,"created_connection_id":null,"errors":[],"color_rgb":[46,160,67],"status":"ready","advice":"\u2014","is_manual":false,"member_element_ids":[1249630,1249631,1249636],"template_name":"PRUEBA probar_conexiones","attempts":[],"kind":"node","template_id":"b7461a9f-3537-47f3-a6a2-e0ba0a639b71","orientation":"same","existing_connection_id":null,"visible_by_default":true,"warnings":[],"ghost_element_id":null,"is_marked":false,"marker_element_id":null,"name":"N4","status_text":"\u25cf Listo","chord_element_id":1249510,"match":{"unassigned_members":[],"matched_count":3,"orientation":"same","score_deg":0.01,"assignments":[{"model_angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":136.91999999999999,"element_id":1249630,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":0},{"model_angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"+Y","template_angle_deg":44.369999999999997,"element_id":1249631,"deviation_deg":0,"template_profile":"HSS2-1/2X2-1/2X3/16","slot":1},{"model_angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","role":"diagonal","side":"-Y","template_angle_deg":-135.63000000000000,"element_id":1249636,"deviation_deg":0,"templat ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote f49dce56: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)).
Cuerpo:
{"data":{"stop_on_error":false,"plan_creatable_count":1,"deleted_elements_count":0,"duration_ms":0,"rolled_back_count":0,"created_count":0,"updated_count":0,"plan_summary":{"untyped":5,"failed":1},"stopped":false,"deleted_count":0,"plan_id":"f49dce56-56c3-460a-826b-f3dddbed0a42","connection_ids":[],"batch_id":"f49dce56-56c3-460a-826b-f3dddbed0a42","nodes":[{"outcome":"failed","orientation":"same","template_name":"PRUEBA probar_conexiones","connection_id":null,"reason":"token distinto del plan","errors":[{"hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","code":"VALIDATION_TOKEN_INVALID","path":"nodes[N4].validation_token"}],"warnings":[],"spec":null,"node":"N4","restored_members_count":0,"elements_count":0,"duration_ms":0,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))"}],"summary_text":"Lote f49dce56: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)).","restored_members_count":0,"started_utc":"2026-10-21T17:11:22.6634899Z","operation":"batch_create","stopped_at":null,"with_warnings_count":0,"undo_entries":"one","finished_utc":"2026-10-21T17:11:22.6641215Z","document":"HAN ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"data":{"stop_on_error":false,"plan_creatable_count":null,"deleted_elements_count":0,"duration_ms":7,"rolled_back_count":0,"created_count":0,"updated_count":0,"plan_summary":null,"stopped":false,"deleted_count":0,"plan_id":null,"connection_ids":[],"batch_id":"00000000-0000-0000-0000-000000000000","nodes":[],"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","restored_members_count":0,"started_utc":"2026-10-21T17:11:22.7036794Z","operation":"batch_delete","stopped_at":null,"with_warnings_count":0,"undo_entries":"one","finished_utc":"2026-10-21T17:11:22.7106880Z","document":"HANGAR_PRUEBA_sondeo","failed_count":0,"skipped_count":0,"plan_summary_text":null},"meta":{"duration_ms":7,"operation":"batch_delete","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[{"hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","code":"BATCH_EMPTY","path":"batch_id"}]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado f49dce56-56c3-460a-826b-f3dddbed0a42
Cuerpo:
{"data": {"removed_labels": 0, "remaining_plans": 1, "remaining_labels": 0, "discarded_plan_id": "f49dce56-56c3-460a-826b-f3dddbed0a42", "remaining_markers": 0, "removed_marks": 0}, "meta": {"duration_ms": 18, "operation": "batch_plan_discard", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"catalog_apply","addin_version":"0.12.0"},"errors":[{"hint":"Usa conn_catalog_list para ver las plantillas disponibles.","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","code":"TEMPLATE_NOT_FOUND","path":"template_id"}],"ok":false,"warnings":[]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada b7461a9f-3537-47f3-a6a2-e0ba0a639b71
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\b7461a9f-3537-47f3-a6a2-e0ba0a639b71.json","deleted_template_id":"b7461a9f-3537-47f3-a6a2-e0ba0a639b71"},"meta":{"duration_ms":2,"operation":"catalog_delete","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"data":{"updated_utc":"2026-10-21T17:11:24.2620945Z","selection_expansion":{"chord_count":7,"member_count":42,"splice_count":14,"added_count":63,"rounds":11,"skipped_out_of_plane":[],"requested_count":1,"ignored_element_ids":[],"summary_text":"Se a\u00f1adieron 63 barras que tocan la selecci\u00f3n: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cord\u00f3n.","added_element_ids":[1249630,1249631,1249632,1249633,1249636,1249637,1245533,1245532,1249626,1249638,1245535,1249627,1249628,1249629,1250263,1249509,1245537,1250264,1250265,1250266,1250269,1249639,1249511,1250267,1245539,1250270,1250271,1250272,1250275,1250268,1249512,1250273,1245541,1250276,1250277,1250278,1250281,1250274,1249513,1250279,1245543,1250282,1250283,1250284,1250287,1250280,1249514,1250285,1245545,1250288,1250289,1250290,1250293,1250286,1249515,1250291,1250294,1250295,1250296,1250292,1249516,1250297,1250298],"limit_reached":false},"created_count":0,"marked_view_id":null,"hidden_text":"8 sin cord\u00f3n, 17 barras sueltas","plan_id":"046d10c8-cee9-48ac-a39f-b96c707e407e","nodes":[{"color_name":null,"eccentricity_mm":0,"status_detail":"Ninguna plantilla casa todas sus ranuras con las barras del nudo.","max_deviation_deg":null,"created_connection_id":null,"errors":[],"color_rgb":null,"status":"no_match","advice":"Falta el cord\u00f3n en la selecci\u00f3n: selecci\u00f3nalo y replanifica, o Cord\u00f3n\u2026","is_manual":false,"member_element_ids":[1249630],"template_name":null,"attempts":["N ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 046d10c8-cee9-48ac-a39f-b96c707e407e etiquetas quitadas=0
Cuerpo:
{"data": {"removed_labels": 0, "remaining_plans": 1, "remaining_labels": 0, "discarded_plan_id": "046d10c8-cee9-48ac-a39f-b96c707e407e", "remaining_markers": 0, "removed_marks": 0}, "meta": {"duration_ms": 16, "operation": "batch_plan_discard", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [FALLO]  HTTP 200, ok=True, kind=None excentricidad=None avisos=[]
Cuerpo:
{"data": {"errors_count": 0, "warnings_count": 0, "bolt_stacks": [{"member_element_id": 1245531, "bolt_length_mm": 50.799999999999997, "length_source": "computed_from_grip", "plates": 2, "grip_mm": 25.399999999999999, "gap_mm": 0, "bolts": 12}], "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8", "is_valid": true, "calculated_values": {"origin_mm": [-4180.5, 17204.200000000001, 19933], "frame_z": [0, -1, 0], "axis_distance_mm": 0, "frame_x": [1, 0, 0], "frame_y": [0, 0, 1], "chord_direction_reversed": true}}, "meta": {"duration_ms": 9, "operation": "validate", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [FALLO]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=None excentricidad=None avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"data":{"errors_count":0,"warnings_count":2,"bolt_stacks":[],"validation_token":"c3a605114cd7ef2d32bb9e785be1e7399ebd63fda4740e74f4ada2b72e577713","is_valid":true,"calculated_values":{"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"frame_z":[0,-1,0],"axis_distance_mm":0.089999999999999997,"frame_x":[1,0,0],"frame_y":[0,0,1],"chord_direction_reversed":true}},"meta":{"duration_ms":12,"operation":"validate","addin_version":"0.12.0"},"errors":[],"ok":true,"warnings":[{"hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 34.0 mm admitida hasta 101.6 mm). El JSON declara axis_offset_mm = -17.0 y el modelo mide -27.4. Cada barra se recorta y se suelda sobre su propio eje.","code":"MEMBER_ECCENTRIC","path":"members[0].axis_offset_mm"},{"hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 34.0 mm admitida hasta 101.6 mm). El JSON declara axis_offset_mm = 17.0 y el modelo mide 27.4. Cada barra se recorta y se suelda sobre su propio eje.","code":"MEMBER_ECCENTRIC","path":"members[1].axis_offset_mm"}]}
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"data":{"updated_utc":"2026-10-21T17:11:26.0500189Z","selection_expansion":null,"created_count":0,"marked_view_id":null,"hidden_text":"2 barras sueltas","plan_id":"12fba806-b4d5-45f5-9d77-c31a3aa2dd07","nodes":[{"color_name":null,"eccentricity_mm":0,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","max_deviation_deg":null,"created_connection_id":null,"errors":[],"color_rgb":null,"status":"untyped","advice":"No es un nudo: nada que hacer","is_manual":false,"member_element_ids":[1245531],"template_name":null,"attempts":[],"kind":"node","template_id":null,"orientation":null,"existing_connection_id":null,"visible_by_default":false,"warnings":[],"ghost_element_id":null,"is_marked":false,"marker_element_id":null,"name":"N1","status_text":"\u25cb Barra suelta (no es nudo)","chord_element_id":0,"match":null,"errors_count":0,"splice_element_id":null,"chord_type_name":null,"validation_token":null,"signature":"1 barra(s) sin marco","existing_batch_id":null,"starter":false,"element_ids":[1245531],"through_element_ids":[],"actions":[],"connection_type":null,"is_valid":false,"members":[],"spec":null,"replaces_existing":false,"label_index":null,"work_point_mm":[-14656,17204.299999999999,19933],"warnings_count":0,"chord_continuous":false,"is_mirrored":false,"has_spec_override":false},{"color_name":"verde","eccentricity_mm":0,"status_detail":"Sin plantilla: arranque end_plate (Detalle C) (Detalle C del plano para HSS12X8X1/2 (choices.json)).","max_deviati ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado 12fba806-b4d5-45f5-9d77-c31a3aa2dd07
Cuerpo:
{"data": {"removed_labels": 0, "remaining_plans": 1, "remaining_labels": 0, "discarded_plan_id": "12fba806-b4d5-45f5-9d77-c31a3aa2dd07", "remaining_markers": 0, "removed_marks": 0}, "meta": {"duration_ms": 15, "operation": "batch_plan_discard", "addin_version": "0.12.0"}, "errors": [], "ok": true, "warnings": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.12.0
Cuerpo:
{
  "data": {
    "addin_version": "0.12.0",
    "operations": [
      "batch_create",
      "batch_delete",
      "batch_plan",
      "batch_plan_discard",
      "batch_plan_get",
      "catalog_apply",
      "catalog_delete",
      "catalog_get",
      "catalog_list",
      "catalog_save",
      "create",
      "delete",
      "find_profile",
      "get",
      "guide",
      "list",
      "node_info",
      "ping",
      "preview",
      "schema",
      "types",
      "update",
      "validate"
    ],
    "document": {
      "is_workshared": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "is_family": false,
      "is_modifiable": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_read_only": false
    },
    "dotnet": {
      "load_context": "Default",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12"
    },
    "revit": {
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39",
      "version_name": "Autodesk Revit 2027",
      "version_number": "2027",
      "language": "English_USA"
    },
    "has_uidocument": true,
    "backend": "advancesteel",
    "spec_version": "1.0"
  },
  "meta": {
    "duration_ms": 3,
    "operation": "ping",
    "addin_version": "0.12.0"
  },
  "errors": [],
  "ok": true,
  "warnings": []
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "data": {
    "name": "PRUEBA probar_conexiones",
    "match": {
      "unassigned_members": [],
      "matched_count": 3,
      "orientation": "same",
      "score_deg": 0.01,
      "assignments": [
        {
          "model_angle_deg": 136.92,
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "profile_policy": "warn",
          "role": "diagonal",
          "side": "+Y",
          "template_angle_deg": 136.92,
          "element_id": 1249630,
          "deviation_deg": 0,
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "slot": 0
        },
        {
          "model_angle_deg": 44.37,
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "profile_policy": "warn",
          "role": "diagonal",
          "side": "+Y",
          "template_angle_deg": 44.37,
          "element_id": 1249631,
          "deviation_deg": 0,
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "slot": 1
        },
        {
          "model_angle_deg": -135.63,
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "profile_policy": "warn",
          "role": "diagonal",
          "side": "-Y",
          "template_angle_deg": -135.63,
          "element_id": 1249636,
          "deviation_deg": 0,
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "slot": 2
        }
      ],
      "unmatched_slots": [],
      "is_complete": true,
      "max_deviation_deg": 0,
      "description": "same: ranura 0 (diagonal 136.9°) → ...
======================================================================
Resultado: 36/38 pruebas correctas

```

## 12-9 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 501 ms ==
{
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  2,
                 "templates":  [
                                   {
                                       "name":  "Nudo K cordon superior",
                                       "tags":  [

                                                ],
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "connection_type":  "direct_weld",
                                       "chord_profile":  "HSS12X8X1/2",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "members_count":  2,
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  null
                                   },
                                   {
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "origin_drawing":  "Detalle D",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "connection_type":  "gusset_node",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "members_count":  3,
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ]
             },
    "meta":  {
                 "duration_ms":  1,
                 "operation":  "catalog_list",
                 "addin_version":  "0.12.0"
             },
    "errors":  [

               ],
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 12-9 log del dia

```text

{"ts":"2026-10-21T12:05:06.7313706-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:06.7328858-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:06.7340945-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:06.7353054-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:06.7425071-05:00","record":{"event":"end_plate_built","first_segment":1245537,"second_segment":
1245535,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653639020113E-06,-0]}}
{"ts":"2026-10-21T12:05:06.7579457-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 549598f9-d868-4e8d-8eb9-8a3e30b309ed","pending":6,"new_elements":["1323245:Structural 
Connections:DirectShape","1323246:Structural Connections:DirectShape","1323247:Structural 
Connections:DirectShape","1323248:Structural Connections:DirectShape","1323249:Structural 
Connections:DirectShape","1323250:Structural Connections:DirectShape","1323251:Structural 
Connections:DirectShape","1323252:Structural Connections:DirectShape","1323253:Plates:SteelProxyElement","1323254:Plate
s:SteelProxyElement","1323255:Bolts:SteelProxyElement","1323256:Bolts:SteelProxyElement","1323257:Bolts:SteelProxyEleme
nt","1323258:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:06.9453044-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 18028706-6c16-40e3-8683-1cdfeb8de515","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:06.9479232-05:00","record":{"event":"direct_weld_member","element_id":1250266,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.9506282-05:00","record":{"event":"direct_weld_member","element_id":1250269,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:06.9609308-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 18028706-6c16-40e3-8683-1cdfeb8de515","pending":0,"new_elements":["1323262:Structural 
Connections:DirectShape","1323263:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.0650894-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 8a553d43-1b20-40f7-b170-d56b1f608eda","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.0670983-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.0705510-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250273","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.0732261-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125027
3","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.1155718-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 8a553d43-1b20-40f7-b170-d56b1f608eda","pending":3,"new_elements":["1323266:Structural 
Connections:DirectShape","1323267:Structural Connections:DirectShape","1323268:Structural 
Connections:DirectShape","1323269:Structural Connections:DirectShape","1323270:Structural 
Connections:DirectShape","1323271:Structural Connections:DirectShape","1323272:Plates:SteelProxyElement","1323273:Plate
s:SteelProxyElement","1323274:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.2264544-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 87927705-79a6-485f-b323-90c2e2f1d46d","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.2288646-05:00","record":{"event":"direct_weld_member","element_id":1250270,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.2310178-05:00","record":{"event":"direct_weld_member","element_id":1250271,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.2398728-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 87927705-79a6-485f-b323-90c2e2f1d46d","pending":0,"new_elements":["1323277:Structural 
Connections:DirectShape","1323278:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.3585262-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear e00a1db0-20ef-4b89-9dc2-6e4a711520c6","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.3606877-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.3651964-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250274","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.3682517-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125027
4","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.3836986-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear e00a1db0-20ef-4b89-9dc2-6e4a711520c6","pending":3,"new_elements":["1323282:Structural 
Connections:DirectShape","1323283:Structural Connections:DirectShape","1323284:Structural 
Connections:DirectShape","1323285:Structural Connections:DirectShape","1323286:Structural 
Connections:DirectShape","1323287:Structural Connections:DirectShape","1323288:Plates:SteelProxyElement","1323289:Plate
s:SteelProxyElement","1323290:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.5354122-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear f5043651-6388-4a9d-902e-ad7626b794bd","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.5375613-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245539","vertices":8,"thickness_mm":12.7,"offset_mm":6.35,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.5381759-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245537","vertices":8,"thickness_mm":12.7,"offset_mm":-6.35,"plane_z_mm":-12.7,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.5397472-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.5413115-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.5423396-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.5433860-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.5493496-05:00","record":{"event":"end_plate_built","first_segment":1245539,"second_segment":
1245537,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.307565366741405E-06,-0]}}
{"ts":"2026-10-21T12:05:07.5642550-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear f5043651-6388-4a9d-902e-ad7626b794bd","pending":6,"new_elements":["1323294:Structural 
Connections:DirectShape","1323295:Structural Connections:DirectShape","1323296:Structural 
Connections:DirectShape","1323297:Structural Connections:DirectShape","1323298:Structural 
Connections:DirectShape","1323299:Structural Connections:DirectShape","1323300:Structural 
Connections:DirectShape","1323301:Structural Connections:DirectShape","1323302:Plates:SteelProxyElement","1323303:Plate
s:SteelProxyElement","1323304:Bolts:SteelProxyElement","1323305:Bolts:SteelProxyElement","1323306:Bolts:SteelProxyEleme
nt","1323307:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.7348519-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.7375921-05:00","record":{"event":"direct_weld_member","element_id":1250272,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.7402927-05:00","record":{"event":"direct_weld_member","element_id":1250275,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:07.7503953-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","pending":0,"new_elements":["1323311:Structural 
Connections:DirectShape","1323312:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.8587746-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 19257157-e88f-489a-9bcf-11022a150a1c","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:07.8608181-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.8643038-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250279","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:07.8674417-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125027
9","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:07.8833388-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 19257157-e88f-489a-9bcf-11022a150a1c","pending":3,"new_elements":["1323316:Structural 
Connections:DirectShape","1323317:Structural Connections:DirectShape","1323318:Structural 
Connections:DirectShape","1323319:Structural Connections:DirectShape","1323320:Structural 
Connections:DirectShape","1323321:Structural Connections:DirectShape","1323322:Plates:SteelProxyElement","1323323:Plate
s:SteelProxyElement","1323324:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.0363021-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear ead78189-a064-41ff-88a9-dd048dc1d8ae","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.0387983-05:00","record":{"event":"direct_weld_member","element_id":1250276,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.0410683-05:00","record":{"event":"direct_weld_member","element_id":1250277,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.0501049-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear ead78189-a064-41ff-88a9-dd048dc1d8ae","pending":0,"new_elements":["1323328:Structural 
Connections:DirectShape","1323329:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.1452059-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear fc9f689f-e692-473f-aa98-e495a6065f2b","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.1469704-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.1501080-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250280","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.1527468-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125028
0","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.1661317-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear fc9f689f-e692-473f-aa98-e495a6065f2b","pending":3,"new_elements":["1323332:Structural 
Connections:DirectShape","1323333:Structural Connections:DirectShape","1323334:Structural 
Connections:DirectShape","1323335:Structural Connections:DirectShape","1323336:Structural 
Connections:DirectShape","1323337:Structural Connections:DirectShape","1323338:Plates:SteelProxyElement","1323339:Plate
s:SteelProxyElement","1323340:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.2733550-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear bd60566a-21ff-42a2-9011-a52b900fe49f","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.2750981-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245539","vertices":8,"thickness_mm":12.7,"offset_mm":6.35,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.2756134-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245541","vertices":8,"thickness_mm":12.7,"offset_mm":-6.35,"plane_z_mm":-12.7,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.2770746-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.2784727-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.2793724-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.2803418-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.2859956-05:00","record":{"event":"end_plate_built","first_segment":1245539,"second_segment":
1245541,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[-0.9999999999973376,2.307565366741405E-06,0]}}
{"ts":"2026-10-21T12:05:08.2987026-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear bd60566a-21ff-42a2-9011-a52b900fe49f","pending":6,"new_elements":["1323343:Structural 
Connections:DirectShape","1323344:Structural Connections:DirectShape","1323345:Structural 
Connections:DirectShape","1323346:Structural Connections:DirectShape","1323347:Structural 
Connections:DirectShape","1323348:Structural Connections:DirectShape","1323349:Structural 
Connections:DirectShape","1323350:Structural Connections:DirectShape","1323351:Plates:SteelProxyElement","1323352:Plate
s:SteelProxyElement","1323353:Bolts:SteelProxyElement","1323354:Bolts:SteelProxyElement","1323355:Bolts:SteelProxyEleme
nt","1323356:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.4558457-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear ca7a53cd-d955-4af9-bb6d-bfb90250f929","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.4585273-05:00","record":{"event":"direct_weld_member","element_id":1250278,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.4608755-05:00","record":{"event":"direct_weld_member","element_id":1250281,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.4698868-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear ca7a53cd-d955-4af9-bb6d-bfb90250f929","pending":0,"new_elements":["1323359:Structural 
Connections:DirectShape","1323360:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.5668872-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 81a54a73-e722-4910-86a9-5d816de53c30","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.5694214-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.5730206-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250285","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.5757460-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125028
5","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.5891424-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 81a54a73-e722-4910-86a9-5d816de53c30","pending":3,"new_elements":["1323363:Structural 
Connections:DirectShape","1323364:Structural Connections:DirectShape","1323365:Structural 
Connections:DirectShape","1323366:Structural Connections:DirectShape","1323367:Structural 
Connections:DirectShape","1323368:Structural Connections:DirectShape","1323369:Plates:SteelProxyElement","1323370:Plate
s:SteelProxyElement","1323371:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.6945098-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 73e65525-985a-4da6-9336-35f7f0bd0f09","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.6968767-05:00","record":{"event":"direct_weld_member","element_id":1250282,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.6992252-05:00","record":{"event":"direct_weld_member","element_id":1250283,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:08.7355294-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 73e65525-985a-4da6-9336-35f7f0bd0f09","pending":0,"new_elements":["1323374:Structural 
Connections:DirectShape","1323375:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.8429474-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 50272955-17e8-4bf0-9aab-86437c276b86","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.8447569-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.8481781-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250286","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.8507344-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125028
6","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.8635489-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 50272955-17e8-4bf0-9aab-86437c276b86","pending":3,"new_elements":["1323379:Structural 
Connections:DirectShape","1323380:Structural Connections:DirectShape","1323381:Structural 
Connections:DirectShape","1323382:Structural Connections:DirectShape","1323383:Structural 
Connections:DirectShape","1323384:Structural Connections:DirectShape","1323385:Plates:SteelProxyElement","1323386:Plate
s:SteelProxyElement","1323387:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.9815958-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 09af65a5-2143-45ee-a90c-1ff23f6fc0a5","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:08.9834357-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245543","vertices":8,"thickness_mm":12.7,"offset_mm":6.35,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.9839590-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245541","vertices":8,"thickness_mm":12.7,"offset_mm":-6.35,"plane_z_mm":-12.7,"units":"mm"}}
{"ts":"2026-10-21T12:05:08.9853990-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.9869295-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.9880759-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.9891030-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:08.9944958-05:00","record":{"event":"end_plate_built","first_segment":1245543,"second_segment":
1245541,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653667414034E-06,-0]}}
{"ts":"2026-10-21T12:05:09.0072858-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 09af65a5-2143-45ee-a90c-1ff23f6fc0a5","pending":6,"new_elements":["1323391:Structural 
Connections:DirectShape","1323392:Structural Connections:DirectShape","1323393:Structural 
Connections:DirectShape","1323394:Structural Connections:DirectShape","1323395:Structural 
Connections:DirectShape","1323396:Structural Connections:DirectShape","1323397:Structural 
Connections:DirectShape","1323398:Structural Connections:DirectShape","1323399:Plates:SteelProxyElement","1323400:Plate
s:SteelProxyElement","1323401:Bolts:SteelProxyElement","1323402:Bolts:SteelProxyElement","1323403:Bolts:SteelProxyEleme
nt","1323404:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.1464753-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear d1363ed3-8780-44b6-a4e3-6a7dddd76425","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.1490200-05:00","record":{"event":"direct_weld_member","element_id":1250284,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.1512023-05:00","record":{"event":"direct_weld_member","element_id":1250287,"axis_offset_mm":
0,"setback_ideal_mm":145.09,"axis_setback_mm":177.45,"face_y_mm":-101.6,"footprint_mm":90.68,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.1600941-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear d1363ed3-8780-44b6-a4e3-6a7dddd76425","pending":0,"new_elements":["1323407:Structural 
Connections:DirectShape","1323408:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.2666488-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear cd5b87b1-a9d8-4197-8bef-321f13f78824","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.2683835-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.2718687-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250291","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.2744470-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125029
1","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.2880848-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear cd5b87b1-a9d8-4197-8bef-321f13f78824","pending":3,"new_elements":["1323412:Structural 
Connections:DirectShape","1323413:Structural Connections:DirectShape","1323414:Structural 
Connections:DirectShape","1323415:Structural Connections:DirectShape","1323416:Structural 
Connections:DirectShape","1323417:Structural Connections:DirectShape","1323418:Plates:SteelProxyElement","1323419:Plate
s:SteelProxyElement","1323420:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.4128338-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 1f68d407-5c99-4b93-9b88-69f432ab1774","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.4150214-05:00","record":{"event":"direct_weld_member","element_id":1250288,"axis_offset_mm":
0,"setback_ideal_mm":145.3,"axis_setback_mm":177.76,"face_y_mm":-101.6,"footprint_mm":90.81,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.4169162-05:00","record":{"event":"direct_weld_member","element_id":1250289,"axis_offset_mm":
0,"setback_ideal_mm":145.12,"axis_setback_mm":177.5,"face_y_mm":-101.6,"footprint_mm":90.7,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.4245503-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 1f68d407-5c99-4b93-9b88-69f432ab1774","pending":0,"new_elements":["1323424:Structural 
Connections:DirectShape","1323425:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.5348704-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear b14f334e-1301-4b82-98a4-815a0fd67edb","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.5369514-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.5400294-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250292","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.5425235-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125029
2","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.5556873-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear b14f334e-1301-4b82-98a4-815a0fd67edb","pending":3,"new_elements":["1323428:Structural 
Connections:DirectShape","1323429:Structural Connections:DirectShape","1323430:Structural 
Connections:DirectShape","1323431:Structural Connections:DirectShape","1323432:Structural 
Connections:DirectShape","1323433:Structural Connections:DirectShape","1323434:Plates:SteelProxyElement","1323435:Plate
s:SteelProxyElement","1323436:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.6537633-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear f3bdf907-6874-4139-ab66-aee7cb0c5fb0","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.6558851-05:00","record":{"event":"direct_weld_member","element_id":1250293,"axis_offset_mm":
0,"setback_ideal_mm":148.75,"axis_setback_mm":182.71,"face_y_mm":-101.6,"footprint_mm":92.97,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:09.6640273-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear f3bdf907-6874-4139-ab66-aee7cb0c5fb0","pending":0,"new_elements":["1323439:Structural 
Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.7449969-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 04df2997-4f17-455f-a22a-1378cf7b1719","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.7464366-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1249516","vertices":8,"thickness_mm":9.525,"offset_mm":4.7625,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.7469688-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1249515","vertices":8,"thickness_mm":9.525,"offset_mm":-4.7625,"plane_z_mm":-9.525,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.7492028-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=1","Ny=1","Dx=0","Dy=0","ScrewDiameter=15.875","BindingLength=19.05","ScrewLength=44.449999999999
996"],"count":1,"units":"mm","grip_mm":19.05,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_mm":
14.287500000000001,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.7504903-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=1","Ny=1","Dx=0","Dy=0","ScrewDiameter=15.875","BindingLength=19.05","ScrewLength=44.449999999
999996"],"count":1,"units":"mm","grip_mm":19.05,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.287500000000001,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.7517376-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=1","Ny=1","Dx=0","Dy=0","ScrewDiameter=15.875","BindingLength=19.05","ScrewLength=44.44999999999
9996"],"count":1,"units":"mm","grip_mm":19.05,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_mm"
:14.287500000000001,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.7531997-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=1","Ny=1","Dx=0","Dy=0","ScrewDiameter=15.875","BindingLength=19.05","ScrewLength=44.4499999999
99996"],"count":1,"units":"mm","grip_mm":19.05,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_mm
":14.287500000000001,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.7583861-05:00","record":{"event":"end_plate_built","first_segment":1249516,"second_segment":
1249515,"plate_mm":[230,230,9.525],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[9.6,9.6],"rows":4,"bolts":4,"bolt_lengt
h_mm":44.449999999999996,"tube_footprint_mm":[101.6,101.6],"plate_frame_z":[0.9999999999973377,2.3075653758460542E-06,-
0]}}
{"ts":"2026-10-21T12:05:09.7703634-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 04df2997-4f17-455f-a22a-1378cf7b1719","pending":6,"new_elements":["1323442:Structural 
Connections:DirectShape","1323443:Structural Connections:DirectShape","1323444:Structural 
Connections:DirectShape","1323445:Structural Connections:DirectShape","1323446:Structural 
Connections:DirectShape","1323447:Structural Connections:DirectShape","1323448:Structural 
Connections:DirectShape","1323449:Structural Connections:DirectShape","1323450:Plates:SteelProxyElement","1323451:Plate
s:SteelProxyElement","1323452:Bolts:SteelProxyElement","1323453:Bolts:SteelProxyElement","1323454:Bolts:SteelProxyEleme
nt","1323455:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.9487721-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear d076e320-b6e0-498e-a116-5494fccedc0c","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:09.9506789-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245545","vertices":8,"thickness_mm":12.7,"offset_mm":6.35,"plane_z_mm":0,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.9511904-05:00","record":{"event":"advance_steel_plate_written","name":"Plancha de extremo 
tramo 1245543","vertices":8,"thickness_mm":12.7,"offset_mm":-6.35,"plane_z_mm":-12.7,"units":"mm"}}
{"ts":"2026-10-21T12:05:09.9526980-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila t
op","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count":
4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_min
_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.9541339-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila b
ottom","properties":["Nx=4","Ny=1","Dx=90","Dy=90","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"coun
t":4,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_
min_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.9551149-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila l
eft","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count"
:2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_mi
n_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.9565758-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos empalme fila r
ight","properties":["Nx=2","Ny=1","Dx=80","Dy=80","ScrewDiameter=19.05","BindingLength=25.4","ScrewLength=50.8"],"count
":2,"units":"mm","grip_mm":25.4,"bolt_length_mm":50.8,"length_from_spec":false,"plane_z_mm":19.049999999999997,"stack_m
in_z_mm":-6.35,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:09.9646791-05:00","record":{"event":"end_plate_built","first_segment":1245545,"second_segment":
1245543,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.7,12.7],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[203.2,203.2],"plate_frame_z":[0.9999999999973376,-2.3075653684491095E-06,-0]}}
{"ts":"2026-10-21T12:05:09.9784595-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear d076e320-b6e0-498e-a116-5494fccedc0c","pending":6,"new_elements":["1323459:Structural 
Connections:DirectShape","1323460:Structural Connections:DirectShape","1323461:Structural 
Connections:DirectShape","1323462:Structural Connections:DirectShape","1323463:Structural 
Connections:DirectShape","1323464:Structural Connections:DirectShape","1323465:Structural 
Connections:DirectShape","1323466:Structural Connections:DirectShape","1323467:Plates:SteelProxyElement","1323468:Plate
s:SteelProxyElement","1323469:Bolts:SteelProxyElement","1323470:Bolts:SteelProxyElement","1323471:Bolts:SteelProxyEleme
nt","1323472:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.1298770-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear a7419d94-00fc-49a4-b64d-bb853194fe0b","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.1323623-05:00","record":{"event":"direct_weld_member","element_id":1250290,"axis_offset_mm":
0,"setback_ideal_mm":145.27,"axis_setback_mm":177.71,"face_y_mm":-101.6,"footprint_mm":90.79,"chord_element_id":1245543
}}
{"ts":"2026-10-21T12:05:10.1412324-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear a7419d94-00fc-49a4-b64d-bb853194fe0b","pending":0,"new_elements":["1323476:Structural 
Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.2525399-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 96418f83-1daf-428b-b94a-f204a821c173","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.2543471-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:10.2570106-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250297","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:10.2592936-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125029
7","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:10.2708702-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 96418f83-1daf-428b-b94a-f204a821c173","pending":3,"new_elements":["1323479:Structural 
Connections:DirectShape","1323480:Structural Connections:DirectShape","1323481:Structural 
Connections:DirectShape","1323482:Structural Connections:DirectShape","1323483:Structural 
Connections:DirectShape","1323484:Structural Connections:DirectShape","1323485:Plates:SteelProxyElement","1323486:Plate
s:SteelProxyElement","1323487:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.3844058-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 23960cf8-00a2-4839-8bf8-ea8f6d759da0","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.3868275-05:00","record":{"event":"direct_weld_member","element_id":1250294,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-21T12:05:10.3889241-05:00","record":{"event":"direct_weld_member","element_id":1250295,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-21T12:05:10.3977984-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 23960cf8-00a2-4839-8bf8-ea8f6d759da0","pending":0,"new_elements":["1323491:Structural 
Connections:DirectShape","1323492:Structural Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.5036783-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 7235620e-0970-4510-9f4c-4234b5b40a3e","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.5052931-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"plane_z_mm":-4.7625,"units":"mm"}}
{"ts":"2026-10-21T12:05:10.5079122-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1250298","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"plane_z_mm":4.762499999999999,"units":"mm"}}
{"ts":"2026-10-21T12:05:10.5101095-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 125029
8","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}
{"ts":"2026-10-21T12:05:10.5213946-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 7235620e-0970-4510-9f4c-4234b5b40a3e","pending":3,"new_elements":["1323496:Structural 
Connections:DirectShape","1323497:Structural Connections:DirectShape","1323498:Structural 
Connections:DirectShape","1323499:Structural Connections:DirectShape","1323500:Structural 
Connections:DirectShape","1323501:Structural Connections:DirectShape","1323502:Plates:SteelProxyElement","1323503:Plate
s:SteelProxyElement","1323504:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.6647649-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 487354a9-d777-47e1-86fd-508df18873de","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:10.6675188-05:00","record":{"event":"direct_weld_member","element_id":1250296,"axis_offset_mm":
0,"setback_ideal_mm":145.13,"axis_setback_mm":177.52,"face_y_mm":-101.6,"footprint_mm":90.71,"chord_element_id":null}}
{"ts":"2026-10-21T12:05:10.6773324-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 487354a9-d777-47e1-86fd-508df18873de","pending":0,"new_elements":["1323508:Structural 
Connections:DirectShape"],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:05:11.0653658-05:00","record":{"event":"batch_create","plan_id":"dd6cf93f-76d6-4fe0-81d1-2defaaf4d
a7c","requested":40,"created":40,"updated":0,"failed":0,"skipped":0,"rolled_back":0,"stop_on_error":false,"stopped":fal
se,"undo_entries":"one","connection_ids":["7776b004-6456-49b0-9412-732739e7ffa2","1eb4f785-902a-4afa-b4db-a37072912483"
,"b8dbcd43-f16d-4223-9f05-0b8f6bb17265","9adebe50-be0d-460c-80f0-e205364b358e","4cde5f9a-12da-4896-a5b3-86c78c0f2aa6","
994e38c0-f8ba-48a9-8413-25fa7fc8772d","509c7537-6a6c-42ed-bd99-d1609c3ab93d","2fe4c11f-b38d-473e-bdad-28461f3cd082","83
a64c13-2981-40a5-8654-f91f8c5028a3","dca546c9-2a86-48c6-9cde-3ebad26956e3","6fd5f8f5-b3e3-4397-a975-91708b520f93","ff4b
8802-d659-429d-9bc7-8c869d31b743","549598f9-d868-4e8d-8eb9-8a3e30b309ed","18028706-6c16-40e3-8683-1cdfeb8de515","8a553d
43-1b20-40f7-b170-d56b1f608eda","87927705-79a6-485f-b323-90c2e2f1d46d","e00a1db0-20ef-4b89-9dc2-6e4a711520c6","f5043651
-6388-4a9d-902e-ad7626b794bd","ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","19257157-e88f-489a-9bcf-11022a150a1c","ead78189-a
064-41ff-88a9-dd048dc1d8ae","fc9f689f-e692-473f-aa98-e495a6065f2b","bd60566a-21ff-42a2-9011-a52b900fe49f","ca7a53cd-d95
5-4af9-bb6d-bfb90250f929","81a54a73-e722-4910-86a9-5d816de53c30","73e65525-985a-4da6-9336-35f7f0bd0f09","50272955-17e8-
4bf0-9aab-86437c276b86","09af65a5-2143-45ee-a90c-1ff23f6fc0a5","d1363ed3-8780-44b6-a4e3-6a7dddd76425","cd5b87b1-a9d8-41
97-8bef-321f13f78824","1f68d407-5c99-4b93-9b88-69f432ab1774","b14f334e-1301-4b82-98a4-815a0fd67edb","f3bdf907-6874-4139
-ab66-aee7cb0c5fb0","04df2997-4f17-455f-a22a-1378cf7b1719","d076e320-b6e0-498e-a116-5494fccedc0c","a7419d94-00fc-49a4-b
64d-bb853194fe0b","96418f83-1daf-428b-b94a-f204a821c173","23960cf8-00a2-4839-8bf8-ea8f6d759da0","7235620e-0970-4510-9f4
c-4234b5b40a3e","487354a9-d777-47e1-86fd-508df18873de"],"duration_ms":6282,"summary":"Lote dd6cf93f: 40 conexiones 
creadas (38 con aviso). Una sola entrada de deshacer (Ctrl+Z). 6.3 s."}}
{"ts":"2026-10-21T12:05:11.0673475-05:00","record":{"event":"ribbon_batch_create","plan_id":"dd6cf93f-76d6-4fe0-81d1-2d
efaaf4da7c","requested":40,"created":40,"failed":0,"skipped":0,"undo_entries":"one","duration_ms":6282}}
{"ts":"2026-10-21T12:08:45.2211213-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
borrar 7776b004-6456-49b0-9412-732739e7ffa2","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-21T12:08:45.2558014-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
borrar 7776b004-6456-49b0-9412-732739e7ffa2","pending":0,"new_elements":[],"is_modifiable_after":true}}
{"ts":"2026-10-21T12:08:45.4009706-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 1eb4f785-902a-4afa-b4db-a37072912483","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.5152726-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar b8dbcd43-f16d-4223-9f05-0b8f6bb17265","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.6203795-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 9adebe50-be0d-460c-80f0-e205364b358e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.6981443-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 4cde5f9a-12da-4896-a5b3-86c78c0f2aa6","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.7995014-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 994e38c0-f8ba-48a9-8413-25fa7fc8772d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.8882318-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 509c7537-6a6c-42ed-bd99-d1609c3ab93d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.9739702-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 2fe4c11f-b38d-473e-bdad-28461f3cd082","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.0824215-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 83a64c13-2981-40a5-8654-f91f8c5028a3","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.1774600-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar dca546c9-2a86-48c6-9cde-3ebad26956e3","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.2680738-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 6fd5f8f5-b3e3-4397-a975-91708b520f93","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.3451854-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ff4b8802-d659-429d-9bc7-8c869d31b743","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.4395462-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 549598f9-d868-4e8d-8eb9-8a3e30b309ed","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.5452417-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 18028706-6c16-40e3-8683-1cdfeb8de515","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.6309134-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 8a553d43-1b20-40f7-b170-d56b1f608eda","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.7184764-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 87927705-79a6-485f-b323-90c2e2f1d46d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.8027190-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar e00a1db0-20ef-4b89-9dc2-6e4a711520c6","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.8890117-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f5043651-6388-4a9d-902e-ad7626b794bd","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.9995490-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:47.0799511-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 19257157-e88f-489a-9bcf-11022a150a1c","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.1718078-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ead78189-a064-41ff-88a9-dd048dc1d8ae","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.2473366-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar fc9f689f-e692-473f-aa98-e495a6065f2b","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.3418649-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar bd60566a-21ff-42a2-9011-a52b900fe49f","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.4194534-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ca7a53cd-d955-4af9-bb6d-bfb90250f929","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.5132144-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 81a54a73-e722-4910-86a9-5d816de53c30","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.6157912-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 73e65525-985a-4da6-9336-35f7f0bd0f09","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.7061815-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 50272955-17e8-4bf0-9aab-86437c276b86","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.8123429-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 09af65a5-2143-45ee-a90c-1ff23f6fc0a5","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.9284223-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar d1363ed3-8780-44b6-a4e3-6a7dddd76425","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.0098414-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar cd5b87b1-a9d8-4197-8bef-321f13f78824","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.1136766-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 1f68d407-5c99-4b93-9b88-69f432ab1774","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.1969608-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar b14f334e-1301-4b82-98a4-815a0fd67edb","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.2878142-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f3bdf907-6874-4139-ab66-aee7cb0c5fb0","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.3687677-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 04df2997-4f17-455f-a22a-1378cf7b1719","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.4451411-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar d076e320-b6e0-498e-a116-5494fccedc0c","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.5201958-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar a7419d94-00fc-49a4-b64d-bb853194fe0b","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.5779530-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 96418f83-1daf-428b-b94a-f204a821c173","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.6744256-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 23960cf8-00a2-4839-8bf8-ea8f6d759da0","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.7763419-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 7235620e-0970-4510-9f4c-4234b5b40a3e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.8717675-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 487354a9-d777-47e1-86fd-508df18873de","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.9233958-05:00","record":{"event":"batch_delete","batch_id":"dd6cf93f-76d6-4fe0-81d1-2defaaf4
da7c","plan_in_memory":true,"deleted":40,"failed":0,"elements":285,"restored_members":93,"duration_ms":3722,"summary":"
Lote dd6cf93f: 40 conexiones borradas (285 elementos, 93 barras restauradas). Una sola entrada de deshacer (Ctrl+Z). 
3.7 s."}}
{"ts":"2026-10-21T12:08:48.9373806-05:00","record":{"event":"handle","operation":"batch_delete","request_summary":"{\"b
atch_id\": 
\"dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3736}}
{"ts":"2026-10-21T12:08:49.2994624-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 5b5fd89b-9161-4560-92d7-4209da8a377e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:49.4783276-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f370914e-94a4-441f-8bdd-c4b038d3c309","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:11:22.1164798-05:00","record":{"event":"catalog_save","template_id":"b7461a9f-3537-47f3-a6a2-e0ba0
a639b71","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catal
ogo\\b7461a9f-3537-47f3-a6a2-e0ba0a639b71.json","shared_file":null,"from_connection":null}}
{"ts":"2026-10-21T12:11:22.1247346-05:00","record":{"event":"handle","operation":"catalog_save","request_summary":"{\"n
ame\": \"PRUEBA probar_conexiones\", \"tags\": [\"prueba\"], \"overwrite\": true, \"spec\": {\"members\": 
[{\"attachment\": {\"type\": \"welded_slot\", \"slot_length_mm\": 150.0, \"weld\": {\"type\": \"fillet\", \"size_mm\": 
5...","ok":true,"error_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":45}}
{"ts":"2026-10-21T12:11:22.6647777-05:00","record":{"event":"batch_create","plan_id":"f49dce56-56c3-460a-826b-f3dddbed0
a42","requested":1,"created":0,"updated":0,"failed":1,"skipped":0,"rolled_back":0,"stop_on_error":false,"stopped":false
,"undo_entries":"one","connection_ids":[],"duration_ms":0,"summary":"Lote f49dce56: 0 conexiones creadas, 1 falló (N4: 
VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es 
de otro nudo))."}}
{"ts":"2026-10-21T12:11:22.6655449-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"node\": \"N4\", \"validation_token\": 
\"0000000000000000000000000000000000000000000000000000000000000000\"}], \"plan_id\": \"f49dce56-56c3-460a-826b-f3dddbed
0a42\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAILED"],"duration_ms":4}}
{"ts":"2026-10-21T12:11:22.7108137-05:00","record":{"event":"handle","operation":"batch_delete","request_summary":"{\"b
atch_id\": 
\"00000000-0000-0000-0000-000000000000\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_EMPTY"],"duration_ms":7}}
{"ts":"2026-10-21T12:11:26.9399289-05:00","record":{"event":"catalog_save","template_id":"f6d9d174-a970-4d8f-b87a-67eb9
4abd3bd","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catal
ogo\\f6d9d174-a970-4d8f-b87a-67eb94abd3bd.json","shared_file":null,"from_connection":null}}
{"ts":"2026-10-21T12:11:26.9406336-05:00","record":{"event":"handle","operation":"catalog_save","request_summary":"{\"n
ame\": \"PRUEBA probar_conexiones\", \"tags\": [\"prueba\"], \"overwrite\": true, \"spec\": {\"members\": 
[{\"attachment\": {\"type\": \"welded_slot\", \"slot_length_mm\": 150.0, \"weld\": {\"type\": \"fillet\", \"size_mm\": 
5...","ok":true,"error_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":24}}



```

## 12-10 git status antes del commit

```text
?? docs/fases/capturas/fase12-03-nudo-k.png
?? docs/fases/capturas/fase12-04-nudo-k-creado.png
?? docs/fases/capturas/fase12-04-nudo-k-dialogo.png
?? docs/fases/capturas/fase12-05-empalme.png
?? docs/fases/capturas/fase12-06-empalme-creado.png
?? docs/fases/capturas/fase12-06-empalme-dialogo.png
?? docs/fases/capturas/fase12-07-plan.png
?? docs/fases/capturas/fase12-08-lote-detalle.png
?? docs/fases/capturas/fase12-08-lote.png
?? docs/fases/resultados-fase-12.md

```
