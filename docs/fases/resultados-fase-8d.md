# Resultados de la ronda 8d

Fecha: 2026-10-05T18:34:25


## 8d-1 git

```text
46a2147 Ronda 8d: qué sigue y prompts del instalador y del cierre
?? docs/fases/resultados-fase-8d.md

```

## 8d-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.1 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.1 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.1 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:09.13
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   177, Omitido:     0, Total:   177, Duración: 383 ms - MotorConexiones.Tests.dll (net10.0)

```

## 8d-1 revit cerrado

```text

```

## 8d-1 deploy

```text
== MotorConexiones 0.8.4.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 8d-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (23 rutas @api.route)
- copiado tools\conn_tools.py (21 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 8d-1 version de la dll

```text
0.8.4.0

```

## 8d-2 ping

```text
== conn/ping -> HTTP 200 en 377 ms ==
{
    "data":  {
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "version_build":  "27.2.0.39",
                               "sub_version_number":  "2027.2",
                               "language":  "English_USA",
                               "version_number":  "2027"
                           },
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "spec_version":  "1.0",
                 "addin_version":  "0.8.4",
                 "has_uidocument":  true,
                 "operations":  [
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
                 "backend":  "advancesteel",
                 "document":  {
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_read_only":  false,
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "is_workshared":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
                              }
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  8,
                 "operation":  "ping"
             }
}

```

## 8d-2 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 77 ms ==
{
    "data":  {
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "origin_drawing":  "Detalle D",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "connection_type":  "gusset_node",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "name":  "Nudo tipico Detalle D",
                                       "members_count":  3,
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  29,
                 "operation":  "catalog_list"
             }
}

```

## 8d-2 sondeo 17 marcas

```text
== 17-marcas-plan.py -> HTTP 200 en 929 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322371] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 8d-4 batch_plan por el puente en otra vista

```text
== conn/batch_plan -> HTTP 200 en 2072 ms ==
{
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  958,
                 "operation":  "batch_plan"
             },
    "ok":  true,
    "data":  {
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "updated_utc":  "2026-10-06T00:04:39.4796383Z",
                 "hidden_text":  "8 sin cordón, 18 barras sueltas",
                 "unused_element_ids":  [

                                        ],
                 "templates":  {
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "marked_view_id":  1245519,
                 "description":  "59 nudo(s): 25 no_match, 18 untyped, 16 ready.",
                 "is_marked":  true,
                 "selection_count":  64,
                 "created_utc":  "2026-10-06T00:04:39.4790232Z",
                 "summary_text":  "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 con el cordón sin seleccionar. Ocultos: 8 sin cordón, 18 barras sueltas.",
                 "nodes":  [
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251053",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251053 (-43.1°, desvío 1.3°)",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) → barra 1251053 (-43.1°, desvío 1.3°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251053 (-43.1°, desvío 0.0°); ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-43)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245531). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N1].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N1",
                                   "work_point_mm":  [
                                                         -14551.900000000000,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  20.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -43.100000000000001,
                                                       "element_id":  1251053,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245531,
                                                       1251053
                                                   ],
                                   "member_element_ids":  [
                                                              1251053
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245531,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N2",
                                   "work_point_mm":  [
                                                         -14455.299999999999,
                                                         17204.299999999999,
                                                         14894.700000000001
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1251059
                                                   ],
                                   "member_element_ids":  [
                                                              1251059
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N3",
                                   "work_point_mm":  [
                                                         -14397.600000000000,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250933
                                                   ],
                                   "member_element_ids":  [
                                                              1250933
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  0,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (137, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250933 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "ab4fdf2fb585aa1317fa387e3983109431db36e462cdd34355325bbdf56899e7",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N4",
                                   "work_point_mm":  [
                                                         -11870,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  84.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  136.90000000000001,
                                                       "element_id":  1251053,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251054,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  48.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251059,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322401,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  136.91999999999999,
                                                                         "element_id":  1251053,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251054,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251059,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251053 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1251054 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251059 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  0.01,
                                                 "max_deviation_deg":  0
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250933,
                                                       1251053,
                                                       1251054,
                                                       1251059
                                                   ],
                                   "member_element_ids":  [
                                                              1251053,
                                                              1251054,
                                                              1251059
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250933,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250933
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251054 (-135.6°, desvío 0.0°); barras sobrantes: 1251055",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251055 (-45.0°, desvío 0.6°); barras sobrantes: 1251054",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251054 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251055 (-45.0°, desvío 0.6°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251055 (-45.0°, desvío 1.9°); ranura 1 (diagonal -135.6°) → barra 1251054 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -45)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N5",
                                   "work_point_mm":  [
                                                         -9278.1000000000004,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  90.400000000000006,
                                                       "reaches_node":  false,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251054,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  27.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -45,
                                                       "element_id":  1251055,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322402,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -45°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245531,
                                                       1251054,
                                                       1251055
                                                   ],
                                   "member_element_ids":  [
                                                              1251054,
                                                              1251055
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245531,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245531
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3500000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (135, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250933 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "359c66fda80e065b965235bdf3f74107c8720b2d1e3a66217ff7935a471f7121",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N6",
                                   "work_point_mm":  [
                                                         -6740.3999999999996,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135,
                                                       "element_id":  1251055,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  6.2000000000000002,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251056,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251060,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322403,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3500000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.430000000000000,
                                                                         "element_id":  1251056,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.63,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135,
                                                                         "element_id":  1251055,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.01,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.380000000000003,
                                                                         "element_id":  1251060,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251056 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) → barra 1251055 (135.0°, desvío 0.6°); ranura 2 (diagonal -44.4°) → barra 1251060 (-44.4°, desvío 0.0°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.9900000000000000,
                                                 "max_deviation_deg":  1.3500000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250933,
                                                       1251055,
                                                       1251056,
                                                       1251060
                                                   ],
                                   "member_element_ids":  [
                                                              1251055,
                                                              1251056,
                                                              1251060
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250933,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250933
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N7",
                                   "work_point_mm":  [
                                                         -4437.3000000000002,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250933
                                                   ],
                                   "member_element_ids":  [
                                                              1250933
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251060"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251061). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N8].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N8",
                                   "work_point_mm":  [
                                                         -4181.3999999999996,
                                                         17204.200000000001,
                                                         14919.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251060,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251061,
                                                       1251060
                                                   ],
                                   "member_element_ids":  [
                                                              1251060
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251061,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251056 (-135.6°, desvío 0.1°); barras sobrantes: 1245531, 1251049",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251049 (-44.4°, desvío 0.1°); barras sobrantes: 1245531, 1251056",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251056 (-135.6°, desvío 1.4°); ranura 1 (diagonal -44.4°) → barra 1251049 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245531",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251049 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251056 (-135.6°, desvío 0.1°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245531"
                                                ],
                                   "signature":  "3 -Y (-180, -44, -136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245530). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N9].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N9",
                                   "work_point_mm":  [
                                                         -4180.1999999999998,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.80000000000000004,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245531,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251049,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251056,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322404,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245530,
                                                       1245531,
                                                       1251049,
                                                       1251056
                                                   ],
                                   "member_element_ids":  [
                                                              1245531,
                                                              1251049,
                                                              1251056
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N10",
                                   "work_point_mm":  [
                                                         -4022.5,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250932
                                                   ],
                                   "member_element_ids":  [
                                                              1250932
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250932 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "4713c9d37559ef4216927354b1b1e516bdaa74c61a89d39c0fb8c9afa006a24b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N11",
                                   "work_point_mm":  [
                                                         -1621.4000000000001,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251049,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251050,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251061,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322405,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251049,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251050,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251061,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251049 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251050 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251061 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250932,
                                                       1251049,
                                                       1251050,
                                                       1251061
                                                   ],
                                   "member_element_ids":  [
                                                              1251049,
                                                              1251050,
                                                              1251061
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250932,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250932
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251050 (-135.6°, desvío 0.0°); barras sobrantes: 1251051",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251051 (-44.4°, desvío 0.1°); barras sobrantes: 1251050",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251050 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251051 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251051 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251050 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N12",
                                   "work_point_mm":  [
                                                         944.79999999999995,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251050,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251051,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322406,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245530,
                                                       1251050,
                                                       1251051
                                                   ],
                                   "member_element_ids":  [
                                                              1251050,
                                                              1251051
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245530
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250932 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "b22907acc24764d7e808e8e007aedae1c102cf3af123b25591daf3ce02bbbe4b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N13",
                                   "work_point_mm":  [
                                                         3504.8000000000002,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251051,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251052,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251062,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322407,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251052,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251051,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251062,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251052 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251051 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251062 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250932,
                                                       1251051,
                                                       1251052,
                                                       1251062
                                                   ],
                                   "member_element_ids":  [
                                                              1251051,
                                                              1251052,
                                                              1251062
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250932,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250932
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N14",
                                   "work_point_mm":  [
                                                         5812.6999999999998,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250932
                                                   ],
                                   "member_element_ids":  [
                                                              1250932
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251062"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251694). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N15].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N15",
                                   "work_point_mm":  [
                                                         6063.6000000000004,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251062,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251694,
                                                       1251062
                                                   ],
                                   "member_element_ids":  [
                                                              1251062
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251694,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251052 (-135.6°, desvío 0.0°); barras sobrantes: 1245534, 1251690",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251690 (-44.4°, desvío 0.1°); barras sobrantes: 1245534, 1251052",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251052 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251690 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245534",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251690 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251052 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245534"
                                                ],
                                   "signature":  "3 -Y (-0, -136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245530). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N16].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N16",
                                   "work_point_mm":  [
                                                         6069.8000000000002,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245534,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251052,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251690,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322408,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245530,
                                                       1245534,
                                                       1251052,
                                                       1251690
                                                   ],
                                   "member_element_ids":  [
                                                              1245534,
                                                              1251052,
                                                              1251690
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N17",
                                   "work_point_mm":  [
                                                         6227.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250934
                                                   ],
                                   "member_element_ids":  [
                                                              1250934
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250934 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "8fb605d8822d33eabe98697e2f05f3a77466b2e41b7bd40100bb1bd7f848a22a",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N18",
                                   "work_point_mm":  [
                                                         8628.6000000000004,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251690,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251691,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251694,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322409,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251690,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251691,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251694,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251690 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251691 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251694 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250934,
                                                       1251690,
                                                       1251691,
                                                       1251694
                                                   ],
                                   "member_element_ids":  [
                                                              1251690,
                                                              1251691,
                                                              1251694
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250934,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250934
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251691 (-135.6°, desvío 0.0°); barras sobrantes: 1251692",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251692 (-44.4°, desvío 0.1°); barras sobrantes: 1251691",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251691 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251692 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251692 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251691 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N19",
                                   "work_point_mm":  [
                                                         11194.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251691,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251692,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322410,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245534,
                                                       1251691,
                                                       1251692
                                                   ],
                                   "member_element_ids":  [
                                                              1251691,
                                                              1251692
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245534,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245534
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250934 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "eb8cefa9a4642391c96669cd4a0570a5030a8274924367c4371b0ac5d45a707f",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N20",
                                   "work_point_mm":  [
                                                         13754.799999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251692,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251693,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251695,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322411,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251693,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251692,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251695,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251693 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251692 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251695 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250934,
                                                       1251692,
                                                       1251693,
                                                       1251695
                                                   ],
                                   "member_element_ids":  [
                                                              1251692,
                                                              1251693,
                                                              1251695
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250934,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250934
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N21",
                                   "work_point_mm":  [
                                                         16062.700000000001,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250934
                                                   ],
                                   "member_element_ids":  [
                                                              1250934
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251695"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251700). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N22].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N22",
                                   "work_point_mm":  [
                                                         16313.600000000000,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251695,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251700,
                                                       1251695
                                                   ],
                                   "member_element_ids":  [
                                                              1251695
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251700,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251693 (-135.6°, desvío 0.0°); barras sobrantes: 1245536, 1251696",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251696 (-44.4°, desvío 0.1°); barras sobrantes: 1245536, 1251693",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251693 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251696 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245536",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251696 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251693 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245536"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245534). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N23].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N23",
                                   "work_point_mm":  [
                                                         16319.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245536,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251693,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251696,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322412,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245534,
                                                       1245536,
                                                       1251693,
                                                       1251696
                                                   ],
                                   "member_element_ids":  [
                                                              1245536,
                                                              1251693,
                                                              1251696
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245534,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N24",
                                   "work_point_mm":  [
                                                         16477.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250935
                                                   ],
                                   "member_element_ids":  [
                                                              1250935
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250935 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "2b7da029d3a15b4c9ef3ae05336d450ca09e3d5fb1d3be43e2ca66f01f701277",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N25",
                                   "work_point_mm":  [
                                                         18878.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251696,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251697,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251700,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322413,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251696,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251697,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251700,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251696 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251697 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251700 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250935,
                                                       1251696,
                                                       1251697,
                                                       1251700
                                                   ],
                                   "member_element_ids":  [
                                                              1251696,
                                                              1251697,
                                                              1251700
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250935,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250935
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251697 (-135.6°, desvío 0.0°); barras sobrantes: 1251698",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251698 (-44.4°, desvío 0.1°); barras sobrantes: 1251697",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251697 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251698 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251698 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251697 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N26",
                                   "work_point_mm":  [
                                                         21444.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251697,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251698,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322414,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245536,
                                                       1251697,
                                                       1251698
                                                   ],
                                   "member_element_ids":  [
                                                              1251697,
                                                              1251698
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245536,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245536
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250935 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "126328a6eb7ed1103bf1525e16422912c22099a8ada146875e76780af69effbc",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N27",
                                   "work_point_mm":  [
                                                         24004.799999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251698,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251699,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251701,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322415,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251699,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251698,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251701,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251699 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251698 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251701 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250935,
                                                       1251698,
                                                       1251699,
                                                       1251701
                                                   ],
                                   "member_element_ids":  [
                                                              1251698,
                                                              1251699,
                                                              1251701
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250935,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250935
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N28",
                                   "work_point_mm":  [
                                                         26312.700000000001,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250935
                                                   ],
                                   "member_element_ids":  [
                                                              1250935
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251701"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251706). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N29].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N29",
                                   "work_point_mm":  [
                                                         26563.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251701,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251706,
                                                       1251701
                                                   ],
                                   "member_element_ids":  [
                                                              1251701
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251706,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251699 (-135.6°, desvío 0.0°); barras sobrantes: 1245536, 1251702",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251702 (-44.4°, desvío 0.1°); barras sobrantes: 1245536, 1251699",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251699 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251702 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245536",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251702 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251699 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245536"
                                                ],
                                   "signature":  "3 -Y (-180, -136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245538). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N30].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N30",
                                   "work_point_mm":  [
                                                         26569.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245536,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251699,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251702,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322416,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245538,
                                                       1245536,
                                                       1251699,
                                                       1251702
                                                   ],
                                   "member_element_ids":  [
                                                              1245536,
                                                              1251699,
                                                              1251702
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N31",
                                   "work_point_mm":  [
                                                         26727.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250936
                                                   ],
                                   "member_element_ids":  [
                                                              1250936
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250936 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "3ff1e9700d483dac4493d74c88d9c9b2f89cc2338c670bc91a7eadedc5c17fad",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N32",
                                   "work_point_mm":  [
                                                         29128.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251702,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251703,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251706,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322417,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251702,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251703,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251706,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251702 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251703 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251706 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250936,
                                                       1251702,
                                                       1251703,
                                                       1251706
                                                   ],
                                   "member_element_ids":  [
                                                              1251702,
                                                              1251703,
                                                              1251706
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250936,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250936
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251703 (-135.6°, desvío 0.0°); barras sobrantes: 1251704",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251704 (-44.4°, desvío 0.1°); barras sobrantes: 1251703",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251703 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251704 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251704 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251703 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N33",
                                   "work_point_mm":  [
                                                         31694.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251703,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251704,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322418,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245538,
                                                       1251703,
                                                       1251704
                                                   ],
                                   "member_element_ids":  [
                                                              1251703,
                                                              1251704
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245538
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250936 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "b49b4a0ac33fb4bad9f2722bf488c5e2bce2f309ddce60de428961c1082c31fa",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N34",
                                   "work_point_mm":  [
                                                         34254.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251704,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251705,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251707,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322419,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251705,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251704,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251707,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251705 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251704 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251707 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250936,
                                                       1251704,
                                                       1251705,
                                                       1251707
                                                   ],
                                   "member_element_ids":  [
                                                              1251704,
                                                              1251705,
                                                              1251707
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250936,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250936
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N35",
                                   "work_point_mm":  [
                                                         36562.699999999997,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250936
                                                   ],
                                   "member_element_ids":  [
                                                              1250936
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251707"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251712). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N36].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N36",
                                   "work_point_mm":  [
                                                         36813.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251707,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251712,
                                                       1251707
                                                   ],
                                   "member_element_ids":  [
                                                              1251707
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251712,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251705 (-135.6°, desvío 0.0°); barras sobrantes: 1245540, 1251708",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251708 (-44.4°, desvío 0.1°); barras sobrantes: 1245540, 1251705",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251705 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251708 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245540",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251708 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251705 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245540"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245538). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N37].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N37",
                                   "work_point_mm":  [
                                                         36819.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245540,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251705,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251708,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322420,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245538,
                                                       1245540,
                                                       1251705,
                                                       1251708
                                                   ],
                                   "member_element_ids":  [
                                                              1245540,
                                                              1251705,
                                                              1251708
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N38",
                                   "work_point_mm":  [
                                                         36977.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250937
                                                   ],
                                   "member_element_ids":  [
                                                              1250937
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250937 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "42aa816c83946ed0842d0c01e3363b0f4266d8fa5cdd03c91e728e296939eae5",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N39",
                                   "work_point_mm":  [
                                                         39378.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251708,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251709,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251712,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322421,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251708,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251709,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251712,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251708 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251709 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251712 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250937,
                                                       1251708,
                                                       1251709,
                                                       1251712
                                                   ],
                                   "member_element_ids":  [
                                                              1251708,
                                                              1251709,
                                                              1251712
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250937,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250937
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251709 (-135.6°, desvío 0.0°); barras sobrantes: 1251710",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251710 (-44.4°, desvío 0.1°); barras sobrantes: 1251709",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251709 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251710 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251710 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251709 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N40",
                                   "work_point_mm":  [
                                                         41944.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251709,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251710,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322422,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245540,
                                                       1251709,
                                                       1251710
                                                   ],
                                   "member_element_ids":  [
                                                              1251709,
                                                              1251710
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245540,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245540
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250937 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "355b505a6437decfe5b13ef7e59e6d93e311e6753308eb49a459709abadcea21",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N41",
                                   "work_point_mm":  [
                                                         44504.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251710,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251711,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251713,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322423,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251711,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251710,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251713,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251711 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251710 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251713 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250937,
                                                       1251710,
                                                       1251711,
                                                       1251713
                                                   ],
                                   "member_element_ids":  [
                                                              1251710,
                                                              1251711,
                                                              1251713
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250937,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250937
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N42",
                                   "work_point_mm":  [
                                                         46812.699999999997,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250937
                                                   ],
                                   "member_element_ids":  [
                                                              1250937
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251713"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251718). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N43].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N43",
                                   "work_point_mm":  [
                                                         47063.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251713,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251718,
                                                       1251713
                                                   ],
                                   "member_element_ids":  [
                                                              1251713
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251718,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251711 (-135.6°, desvío 0.0°); barras sobrantes: 1245542, 1251714",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251714 (-44.4°, desvío 0.1°); barras sobrantes: 1245542, 1251711",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251711 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251714 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251714 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251711 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245542"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245540). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N44].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N44",
                                   "work_point_mm":  [
                                                         47069.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245542,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251711,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251714,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322424,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245540,
                                                       1245542,
                                                       1251711,
                                                       1251714
                                                   ],
                                   "member_element_ids":  [
                                                              1245542,
                                                              1251711,
                                                              1251714
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245540,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N45",
                                   "work_point_mm":  [
                                                         47327.099999999999,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250938
                                                   ],
                                   "member_element_ids":  [
                                                              1250938
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250938 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "2d549181b6403f944f2e009171a90a9bd241407485039fa9aa1baf0b954e34ff",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N46",
                                   "work_point_mm":  [
                                                         49628.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251714,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251715,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251718,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322425,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251714,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251715,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251718,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251714 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251715 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251718 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250938,
                                                       1251714,
                                                       1251715,
                                                       1251718
                                                   ],
                                   "member_element_ids":  [
                                                              1251714,
                                                              1251715,
                                                              1251718
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250938,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250938
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251715 (-135.6°, desvío 0.0°); barras sobrantes: 1251716",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251716 (-44.4°, desvío 0.1°); barras sobrantes: 1251715",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251715 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251716 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251716 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251715 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N47",
                                   "work_point_mm":  [
                                                         52194.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251715,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251716,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322426,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245542,
                                                       1251715,
                                                       1251716
                                                   ],
                                   "member_element_ids":  [
                                                              1251715,
                                                              1251716
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245542,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245542
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250938 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "25156839ceb806ddafc3484c3f80aca0a96ae63510cf36e816cbb33c01ca3f1b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N48",
                                   "work_point_mm":  [
                                                         54754.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251716,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251717,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251719,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322427,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251717,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251716,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251719,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251717 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251716 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251719 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250938,
                                                       1251716,
                                                       1251717,
                                                       1251719
                                                   ],
                                   "member_element_ids":  [
                                                              1251716,
                                                              1251717,
                                                              1251719
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250938,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250938
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251720",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251720 (-43.1°, desvío 1.3°)",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) → barra 1251720 (-43.1°, desvío 1.3°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251720 (-43.1°, desvío 0.0°); ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-43)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N49",
                                   "work_point_mm":  [
                                                         57198.300000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  20.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -43.100000000000001,
                                                       "element_id":  1251720,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322428,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (1 barra, ángulos -43.1°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245542,
                                                       1251720
                                                   ],
                                   "member_element_ids":  [
                                                              1251720
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245542,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245542
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251719"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251724). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N50].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N50",
                                   "work_point_mm":  [
                                                         57314.199999999997,
                                                         17204.200000000001,
                                                         14913.5
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  55.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251719,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251724,
                                                       1251719
                                                   ],
                                   "member_element_ids":  [
                                                              1251719
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251724,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N51",
                                   "work_point_mm":  [
                                                         57319.900000000001,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250939,
                                                       1250938
                                                   ],
                                   "member_element_ids":  [
                                                              1250938
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251717 (-135.6°, desvío 0.0°); barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1245542, 1251717",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251717 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) → barra 1251717 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245542"
                                                ],
                                   "signature":  "2 -Y (-180, -136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245544). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N52].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N52",
                                   "work_point_mm":  [
                                                         57319.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245542,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251717,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322429,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245544,
                                                       1245542,
                                                       1251717
                                                   ],
                                   "member_element_ids":  [
                                                              1245542,
                                                              1251717
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  0,
                                   "spec":  null,
                                   "color_name":  "verde",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (137, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  "17ddac3d89c33a36bc4d9c28bbe57820659e278e5933a436278e24a417d59aa8",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "● Listo",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N53",
                                   "work_point_mm":  [
                                                         59880.300000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  84.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  136.90000000000001,
                                                       "element_id":  1251720,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251721,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  48.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251724,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322430,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  136.91999999999999,
                                                                         "element_id":  1251720,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251721,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251724,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251720 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1251721 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251724 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  0.01,
                                                 "max_deviation_deg":  0
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "—",
                                   "element_ids":  [
                                                       1250939,
                                                       1251720,
                                                       1251721,
                                                       1251724
                                                   ],
                                   "member_element_ids":  [
                                                              1251720,
                                                              1251721,
                                                              1251724
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250939
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251721 (-135.6°, desvío 0.0°); barras sobrantes: 1251722",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251722 (-45.0°, desvío 0.6°); barras sobrantes: 1251721",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251721 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251722 (-45.0°, desvío 0.6°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251722 (-45.0°, desvío 1.9°); ranura 1 (diagonal -135.6°) → barra 1251721 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -45)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N54",
                                   "work_point_mm":  [
                                                         62472.099999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  90.400000000000006,
                                                       "reaches_node":  false,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251721,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  27.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -45,
                                                       "element_id":  1251722,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322431,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -45°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245544,
                                                       1251721,
                                                       1251722
                                                   ],
                                   "member_element_ids":  [
                                                              1251721,
                                                              1251722
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245544
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3500000000000001,
                                   "spec":  null,
                                   "color_name":  "verde",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (135, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  true,
                                   "validation_token":  "3455521071d423efdfec66bad4eefe67308de41f1a84e6f452f5630e413a67a1",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "● Listo",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N55",
                                   "work_point_mm":  [
                                                         65009.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135,
                                                       "element_id":  1251722,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  6.2999999999999998,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251723,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251725,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322432,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3500000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.430000000000000,
                                                                         "element_id":  1251723,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.63,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135,
                                                                         "element_id":  1251722,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.01,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.380000000000003,
                                                                         "element_id":  1251725,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251723 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) → barra 1251722 (135.0°, desvío 0.6°); ranura 2 (diagonal -44.4°) → barra 1251725 (-44.4°, desvío 0.0°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.9900000000000000,
                                                 "max_deviation_deg":  1.3500000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "—",
                                   "element_ids":  [
                                                       1250939,
                                                       1251722,
                                                       1251723,
                                                       1251725
                                                   ],
                                   "member_element_ids":  [
                                                              1251722,
                                                              1251723,
                                                              1251725
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250939
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N56",
                                   "work_point_mm":  [
                                                         67437.800000000003,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250939
                                                   ],
                                   "member_element_ids":  [
                                                              1250939
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N57",
                                   "work_point_mm":  [
                                                         67529.899999999994,
                                                         17204.200000000001,
                                                         14957.200000000001
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1251725
                                                   ],
                                   "member_element_ids":  [
                                                              1251725
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251723 (-135.6°, desvío 0.1°)",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251723",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251723 (-135.6°, desvío 1.4°); ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) → barra 1251723 (-135.6°, desvío 0.1°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N58",
                                   "work_point_mm":  [
                                                         67570,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251723,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322433,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (1 barra, ángulos -135.6°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245544,
                                                       1251723
                                                   ],
                                   "member_element_ids":  [
                                                              1251723
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245544
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N59",
                                   "work_point_mm":  [
                                                         67794.899999999994,
                                                         17204.099999999999,
                                                         19933
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1245544
                                                   ],
                                   "member_element_ids":  [
                                                              1245544
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               }
                           ],
                 "plan_id":  "6d218fa3-03ca-4c47-95d9-6da3a84540b5",
                 "summary":  {
                                 "no_match":  25,
                                 "ready":  16,
                                 "untyped":  18
                             },
                 "ready_count":  16,
                 "visible_count":  33,
                 "marks":  {
                               "marker_element_ids":  [
                                                          1322401,
                                                          1322402,
                                                          1322403,
                                                          1322404,
                                                          1322405,
                                                          1322406,
                                                          1322407,
                                                          1322408,
                                                          1322409,
                                                          1322410,
                                                          1322411,
                                                          1322412,
                                                          1322413,
                                                          1322414,
                                                          1322415,
                                                          1322416,
                                                          1322417,
                                                          1322418,
                                                          1322419,
                                                          1322420,
                                                          1322421,
                                                          1322422,
                                                          1322423,
                                                          1322424,
                                                          1322425,
                                                          1322426,
                                                          1322427,
                                                          1322428,
                                                          1322429,
                                                          1322430,
                                                          1322431,
                                                          1322432,
                                                          1322433
                                                      ],
                               "element_count":  64
                           },
                 "overrides":  {
                                   "add_member":  {

                                                  },
                                   "merge":  [

                                             ],
                                   "spec":  {

                                            },
                                   "exclude":  [

                                               ],
                                   "add_node":  {

                                                },
                                   "chord":  {

                                             },
                                   "remove_member":  {

                                                     },
                                   "template":  {

                                                },
                                   "replace_existing":  false,
                                   "split":  {

                                             }
                               }
             },
    "errors":  [

               ],
    "warnings":  [

                 ]
}

```

## 8d-4 batch_plan_get con la ventana abierta

```text
== conn/batch_plan_get -> HTTP 200 en 562 ms ==
{
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  1,
                 "operation":  "batch_plan_get"
             },
    "ok":  true,
    "data":  {
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "updated_utc":  "2026-10-06T00:04:39.4796383Z",
                 "hidden_text":  "8 sin cordón, 18 barras sueltas",
                 "unused_element_ids":  [

                                        ],
                 "templates":  {
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "marked_view_id":  1245519,
                 "description":  "59 nudo(s): 25 no_match, 18 untyped, 16 ready.",
                 "is_marked":  true,
                 "selection_count":  64,
                 "created_utc":  "2026-10-06T00:04:39.4790232Z",
                 "summary_text":  "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 con el cordón sin seleccionar. Ocultos: 8 sin cordón, 18 barras sueltas.",
                 "nodes":  [
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251053",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251053 (-43.1°, desvío 1.3°)",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) → barra 1251053 (-43.1°, desvío 1.3°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251053 (-43.1°, desvío 0.0°); ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-43)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245531). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N1].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N1",
                                   "work_point_mm":  [
                                                         -14551.900000000000,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  20.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -43.100000000000001,
                                                       "element_id":  1251053,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245531,
                                                       1251053
                                                   ],
                                   "member_element_ids":  [
                                                              1251053
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245531,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N2",
                                   "work_point_mm":  [
                                                         -14455.299999999999,
                                                         17204.299999999999,
                                                         14894.700000000001
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1251059
                                                   ],
                                   "member_element_ids":  [
                                                              1251059
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N3",
                                   "work_point_mm":  [
                                                         -14397.600000000000,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250933
                                                   ],
                                   "member_element_ids":  [
                                                              1250933
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  0,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (137, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250933 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "ab4fdf2fb585aa1317fa387e3983109431db36e462cdd34355325bbdf56899e7",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N4",
                                   "work_point_mm":  [
                                                         -11870,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  84.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  136.90000000000001,
                                                       "element_id":  1251053,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251054,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  48.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251059,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322401,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  136.91999999999999,
                                                                         "element_id":  1251053,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251054,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251059,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251053 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1251054 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251059 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  0.01,
                                                 "max_deviation_deg":  0
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250933,
                                                       1251053,
                                                       1251054,
                                                       1251059
                                                   ],
                                   "member_element_ids":  [
                                                              1251053,
                                                              1251054,
                                                              1251059
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250933,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250933
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251054 (-135.6°, desvío 0.0°); barras sobrantes: 1251055",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251055 (-45.0°, desvío 0.6°); barras sobrantes: 1251054",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251054 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251055 (-45.0°, desvío 0.6°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251055 (-45.0°, desvío 1.9°); ranura 1 (diagonal -135.6°) → barra 1251054 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -45)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N5",
                                   "work_point_mm":  [
                                                         -9278.1000000000004,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  90.400000000000006,
                                                       "reaches_node":  false,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251054,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  27.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -45,
                                                       "element_id":  1251055,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322402,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -45°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245531,
                                                       1251054,
                                                       1251055
                                                   ],
                                   "member_element_ids":  [
                                                              1251054,
                                                              1251055
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245531,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245531
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3500000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (135, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250933 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "359c66fda80e065b965235bdf3f74107c8720b2d1e3a66217ff7935a471f7121",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N6",
                                   "work_point_mm":  [
                                                         -6740.3999999999996,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135,
                                                       "element_id":  1251055,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  6.2000000000000002,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251056,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251060,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322403,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3500000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.430000000000000,
                                                                         "element_id":  1251056,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.63,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135,
                                                                         "element_id":  1251055,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.01,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.380000000000003,
                                                                         "element_id":  1251060,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251056 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) → barra 1251055 (135.0°, desvío 0.6°); ranura 2 (diagonal -44.4°) → barra 1251060 (-44.4°, desvío 0.0°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.9900000000000000,
                                                 "max_deviation_deg":  1.3500000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250933,
                                                       1251055,
                                                       1251056,
                                                       1251060
                                                   ],
                                   "member_element_ids":  [
                                                              1251055,
                                                              1251056,
                                                              1251060
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250933,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250933
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N7",
                                   "work_point_mm":  [
                                                         -4437.3000000000002,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250933
                                                   ],
                                   "member_element_ids":  [
                                                              1250933
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251060",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251060"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251061). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N8].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N8",
                                   "work_point_mm":  [
                                                         -4181.3999999999996,
                                                         17204.200000000001,
                                                         14919.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251060,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251061,
                                                       1251060
                                                   ],
                                   "member_element_ids":  [
                                                              1251060
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251061,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251056 (-135.6°, desvío 0.1°); barras sobrantes: 1245531, 1251049",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251049 (-44.4°, desvío 0.1°); barras sobrantes: 1245531, 1251056",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251056 (-135.6°, desvío 1.4°); ranura 1 (diagonal -44.4°) → barra 1251049 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245531",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251049 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251056 (-135.6°, desvío 0.1°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245531"
                                                ],
                                   "signature":  "3 -Y (-180, -44, -136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245530). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N9].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N9",
                                   "work_point_mm":  [
                                                         -4180.1999999999998,
                                                         17204.299999999999,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.80000000000000004,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245531,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251049,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251056,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322404,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245530,
                                                       1245531,
                                                       1251049,
                                                       1251056
                                                   ],
                                   "member_element_ids":  [
                                                              1245531,
                                                              1251049,
                                                              1251056
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N10",
                                   "work_point_mm":  [
                                                         -4022.5,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250932
                                                   ],
                                   "member_element_ids":  [
                                                              1250932
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250932 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "4713c9d37559ef4216927354b1b1e516bdaa74c61a89d39c0fb8c9afa006a24b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N11",
                                   "work_point_mm":  [
                                                         -1621.4000000000001,
                                                         17204.299999999999,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251049,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251050,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251061,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322405,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251049,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251050,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251061,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251049 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251050 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251061 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250932,
                                                       1251049,
                                                       1251050,
                                                       1251061
                                                   ],
                                   "member_element_ids":  [
                                                              1251049,
                                                              1251050,
                                                              1251061
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250932,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250932
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251050 (-135.6°, desvío 0.0°); barras sobrantes: 1251051",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251051 (-44.4°, desvío 0.1°); barras sobrantes: 1251050",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251050 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251051 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251051 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251050 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N12",
                                   "work_point_mm":  [
                                                         944.79999999999995,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251050,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251051,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322406,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245530,
                                                       1251050,
                                                       1251051
                                                   ],
                                   "member_element_ids":  [
                                                              1251050,
                                                              1251051
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245530
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250932 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "b22907acc24764d7e808e8e007aedae1c102cf3af123b25591daf3ce02bbbe4b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N13",
                                   "work_point_mm":  [
                                                         3504.8000000000002,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251051,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251052,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251062,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322407,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251052,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251051,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251062,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251052 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251051 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251062 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250932,
                                                       1251051,
                                                       1251052,
                                                       1251062
                                                   ],
                                   "member_element_ids":  [
                                                              1251051,
                                                              1251052,
                                                              1251062
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250932,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250932
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N14",
                                   "work_point_mm":  [
                                                         5812.6999999999998,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250932
                                                   ],
                                   "member_element_ids":  [
                                                              1250932
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251062",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251062"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251694). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N15].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N15",
                                   "work_point_mm":  [
                                                         6063.6000000000004,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251062,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251694,
                                                       1251062
                                                   ],
                                   "member_element_ids":  [
                                                              1251062
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251694,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251052 (-135.6°, desvío 0.0°); barras sobrantes: 1245534, 1251690",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251690 (-44.4°, desvío 0.1°); barras sobrantes: 1245534, 1251052",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251052 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251690 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245534",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251690 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251052 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245534"
                                                ],
                                   "signature":  "3 -Y (-0, -136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245530). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N16].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N16",
                                   "work_point_mm":  [
                                                         6069.8000000000002,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245534,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251052,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251690,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322408,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245530,
                                                       1245534,
                                                       1251052,
                                                       1251690
                                                   ],
                                   "member_element_ids":  [
                                                              1245534,
                                                              1251052,
                                                              1251690
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245530,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N17",
                                   "work_point_mm":  [
                                                         6227.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250934
                                                   ],
                                   "member_element_ids":  [
                                                              1250934
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250934 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "8fb605d8822d33eabe98697e2f05f3a77466b2e41b7bd40100bb1bd7f848a22a",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N18",
                                   "work_point_mm":  [
                                                         8628.6000000000004,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251690,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251691,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251694,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322409,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251690,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251691,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251694,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251690 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251691 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251694 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250934,
                                                       1251690,
                                                       1251691,
                                                       1251694
                                                   ],
                                   "member_element_ids":  [
                                                              1251690,
                                                              1251691,
                                                              1251694
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250934,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250934
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251691 (-135.6°, desvío 0.0°); barras sobrantes: 1251692",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251692 (-44.4°, desvío 0.1°); barras sobrantes: 1251691",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251691 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251692 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251692 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251691 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N19",
                                   "work_point_mm":  [
                                                         11194.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251691,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251692,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322410,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245534,
                                                       1251691,
                                                       1251692
                                                   ],
                                   "member_element_ids":  [
                                                              1251691,
                                                              1251692
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245534,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245534
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250934 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "eb8cefa9a4642391c96669cd4a0570a5030a8274924367c4371b0ac5d45a707f",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N20",
                                   "work_point_mm":  [
                                                         13754.799999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251692,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251693,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251695,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322411,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251693,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251692,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251695,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251693 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251692 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251695 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250934,
                                                       1251692,
                                                       1251693,
                                                       1251695
                                                   ],
                                   "member_element_ids":  [
                                                              1251692,
                                                              1251693,
                                                              1251695
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250934,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250934
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N21",
                                   "work_point_mm":  [
                                                         16062.700000000001,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250934
                                                   ],
                                   "member_element_ids":  [
                                                              1250934
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251695",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251695"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251700). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N22].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N22",
                                   "work_point_mm":  [
                                                         16313.600000000000,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251695,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251700,
                                                       1251695
                                                   ],
                                   "member_element_ids":  [
                                                              1251695
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251700,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251693 (-135.6°, desvío 0.0°); barras sobrantes: 1245536, 1251696",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251696 (-44.4°, desvío 0.1°); barras sobrantes: 1245536, 1251693",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251693 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251696 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245536",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251696 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251693 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245536"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245534). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N23].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N23",
                                   "work_point_mm":  [
                                                         16319.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245536,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251693,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251696,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322412,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245534,
                                                       1245536,
                                                       1251693,
                                                       1251696
                                                   ],
                                   "member_element_ids":  [
                                                              1245536,
                                                              1251693,
                                                              1251696
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245534,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N24",
                                   "work_point_mm":  [
                                                         16477.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250935
                                                   ],
                                   "member_element_ids":  [
                                                              1250935
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250935 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "2b7da029d3a15b4c9ef3ae05336d450ca09e3d5fb1d3be43e2ca66f01f701277",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N25",
                                   "work_point_mm":  [
                                                         18878.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251696,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251697,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251700,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322413,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251696,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251697,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251700,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251696 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251697 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251700 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250935,
                                                       1251696,
                                                       1251697,
                                                       1251700
                                                   ],
                                   "member_element_ids":  [
                                                              1251696,
                                                              1251697,
                                                              1251700
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250935,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250935
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251697 (-135.6°, desvío 0.0°); barras sobrantes: 1251698",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251698 (-44.4°, desvío 0.1°); barras sobrantes: 1251697",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251697 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251698 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251698 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251697 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N26",
                                   "work_point_mm":  [
                                                         21444.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251697,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251698,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322414,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245536,
                                                       1251697,
                                                       1251698
                                                   ],
                                   "member_element_ids":  [
                                                              1251697,
                                                              1251698
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245536,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245536
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250935 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "126328a6eb7ed1103bf1525e16422912c22099a8ada146875e76780af69effbc",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N27",
                                   "work_point_mm":  [
                                                         24004.799999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251698,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251699,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251701,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322415,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251699,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251698,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251701,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251699 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251698 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251701 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250935,
                                                       1251698,
                                                       1251699,
                                                       1251701
                                                   ],
                                   "member_element_ids":  [
                                                              1251698,
                                                              1251699,
                                                              1251701
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250935,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250935
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N28",
                                   "work_point_mm":  [
                                                         26312.700000000001,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250935
                                                   ],
                                   "member_element_ids":  [
                                                              1250935
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251701",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251701"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251706). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N29].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N29",
                                   "work_point_mm":  [
                                                         26563.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251701,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251706,
                                                       1251701
                                                   ],
                                   "member_element_ids":  [
                                                              1251701
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251706,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251699 (-135.6°, desvío 0.0°); barras sobrantes: 1245536, 1251702",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251702 (-44.4°, desvío 0.1°); barras sobrantes: 1245536, 1251699",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251699 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251702 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245536",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251702 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251699 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245536"
                                                ],
                                   "signature":  "3 -Y (-180, -136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245538). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N30].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N30",
                                   "work_point_mm":  [
                                                         26569.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245536,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251699,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251702,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322416,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245538,
                                                       1245536,
                                                       1251699,
                                                       1251702
                                                   ],
                                   "member_element_ids":  [
                                                              1245536,
                                                              1251699,
                                                              1251702
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N31",
                                   "work_point_mm":  [
                                                         26727.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250936
                                                   ],
                                   "member_element_ids":  [
                                                              1250936
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250936 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "3ff1e9700d483dac4493d74c88d9c9b2f89cc2338c670bc91a7eadedc5c17fad",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N32",
                                   "work_point_mm":  [
                                                         29128.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251702,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251703,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251706,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322417,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251702,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251703,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251706,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251702 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251703 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251706 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250936,
                                                       1251702,
                                                       1251703,
                                                       1251706
                                                   ],
                                   "member_element_ids":  [
                                                              1251702,
                                                              1251703,
                                                              1251706
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250936,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250936
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251703 (-135.6°, desvío 0.0°); barras sobrantes: 1251704",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251704 (-44.4°, desvío 0.1°); barras sobrantes: 1251703",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251703 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251704 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251704 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251703 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N33",
                                   "work_point_mm":  [
                                                         31694.799999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251703,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251704,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322418,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245538,
                                                       1251703,
                                                       1251704
                                                   ],
                                   "member_element_ids":  [
                                                              1251703,
                                                              1251704
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245538
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250936 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "b49b4a0ac33fb4bad9f2722bf488c5e2bce2f309ddce60de428961c1082c31fa",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N34",
                                   "work_point_mm":  [
                                                         34254.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251704,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251705,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251707,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322419,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251705,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251704,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251707,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251705 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251704 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251707 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250936,
                                                       1251704,
                                                       1251705,
                                                       1251707
                                                   ],
                                   "member_element_ids":  [
                                                              1251704,
                                                              1251705,
                                                              1251707
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250936,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250936
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N35",
                                   "work_point_mm":  [
                                                         36562.699999999997,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250936
                                                   ],
                                   "member_element_ids":  [
                                                              1250936
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251707",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251707"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251712). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N36].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N36",
                                   "work_point_mm":  [
                                                         36813.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251707,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251712,
                                                       1251707
                                                   ],
                                   "member_element_ids":  [
                                                              1251707
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251712,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251705 (-135.6°, desvío 0.0°); barras sobrantes: 1245540, 1251708",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251708 (-44.4°, desvío 0.1°); barras sobrantes: 1245540, 1251705",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251705 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251708 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245540",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251708 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251705 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245540"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245538). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N37].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N37",
                                   "work_point_mm":  [
                                                         36819.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245540,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251705,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251708,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322420,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245538,
                                                       1245540,
                                                       1251705,
                                                       1251708
                                                   ],
                                   "member_element_ids":  [
                                                              1245540,
                                                              1251705,
                                                              1251708
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245538,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N38",
                                   "work_point_mm":  [
                                                         36977.5,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250937
                                                   ],
                                   "member_element_ids":  [
                                                              1250937
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250937 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "42aa816c83946ed0842d0c01e3363b0f4266d8fa5cdd03c91e728e296939eae5",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N39",
                                   "work_point_mm":  [
                                                         39378.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251708,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251709,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251712,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322421,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251708,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251709,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251712,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251708 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251709 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251712 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250937,
                                                       1251708,
                                                       1251709,
                                                       1251712
                                                   ],
                                   "member_element_ids":  [
                                                              1251708,
                                                              1251709,
                                                              1251712
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250937,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250937
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251709 (-135.6°, desvío 0.0°); barras sobrantes: 1251710",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251710 (-44.4°, desvío 0.1°); barras sobrantes: 1251709",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251709 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251710 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251710 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251709 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N40",
                                   "work_point_mm":  [
                                                         41944.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251709,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251710,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322422,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245540,
                                                       1251709,
                                                       1251710
                                                   ],
                                   "member_element_ids":  [
                                                              1251709,
                                                              1251710
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245540,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245540
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250937 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "355b505a6437decfe5b13ef7e59e6d93e311e6753308eb49a459709abadcea21",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N41",
                                   "work_point_mm":  [
                                                         44504.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251710,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251711,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251713,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322423,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251711,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251710,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251713,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251711 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251710 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251713 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250937,
                                                       1251710,
                                                       1251711,
                                                       1251713
                                                   ],
                                   "member_element_ids":  [
                                                              1251710,
                                                              1251711,
                                                              1251713
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250937,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250937
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N42",
                                   "work_point_mm":  [
                                                         46812.699999999997,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250937
                                                   ],
                                   "member_element_ids":  [
                                                              1250937
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251713",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251713"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251718). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N43].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N43",
                                   "work_point_mm":  [
                                                         47063.599999999999,
                                                         17204.200000000001,
                                                         14914.100000000000
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  54.299999999999997,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251713,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251718,
                                                       1251713
                                                   ],
                                   "member_element_ids":  [
                                                              1251713
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251718,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251711 (-135.6°, desvío 0.0°); barras sobrantes: 1245542, 1251714",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251714 (-44.4°, desvío 0.1°); barras sobrantes: 1245542, 1251711",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251711 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251714 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251714 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251711 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245542"
                                                ],
                                   "signature":  "1 +Y (0) · 2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245540). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N44].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N44",
                                   "work_point_mm":  [
                                                         47069.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  0,
                                                       "element_id":  1245542,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251711,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251714,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322424,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245540,
                                                       1245542,
                                                       1251711,
                                                       1251714
                                                   ],
                                   "member_element_ids":  [
                                                              1245542,
                                                              1251711,
                                                              1251714
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245540,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N45",
                                   "work_point_mm":  [
                                                         47327.099999999999,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250938
                                                   ],
                                   "member_element_ids":  [
                                                              1250938
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  1.3700000000000001,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250938 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  "2d549181b6403f944f2e009171a90a9bd241407485039fa9aa1baf0b954e34ff",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N46",
                                   "work_point_mm":  [
                                                         49628.599999999999,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  15.300000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251714,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251715,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  20.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251718,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322425,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3700000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.55000000000001,
                                                                         "element_id":  1251714,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251715,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251718,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251714 (135.6°, desvío 1.4°); ranura 1 (diagonal 44.4°) → barra 1251715 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251718 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.3799999999999999,
                                                 "max_deviation_deg":  1.3700000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250938,
                                                       1251714,
                                                       1251715,
                                                       1251718
                                                   ],
                                   "member_element_ids":  [
                                                              1251714,
                                                              1251715,
                                                              1251718
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250938,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250938
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251715 (-135.6°, desvío 0.0°); barras sobrantes: 1251716",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251716 (-44.4°, desvío 0.1°); barras sobrantes: 1251715",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251715 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251716 (-44.4°, desvío 0.1°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251716 (-44.4°, desvío 1.4°); ranura 1 (diagonal -135.6°) → barra 1251715 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -44)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N47",
                                   "work_point_mm":  [
                                                         52194.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251715,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251716,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322426,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -44.4°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245542,
                                                       1251715,
                                                       1251716
                                                   ],
                                   "member_element_ids":  [
                                                              1251715,
                                                              1251716
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245542,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245542
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3000000000000000,
                                   "spec":  null,
                                   "color_name":  "ambar",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (136, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "El perfil de el cordón 1250938 en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.",
                                                        "hint":  "Sigue si el cambio de perfil es correcto para este nudo; si no, corrige el modelo o usa otra plantilla.",
                                                        "path":  "chord.profile",
                                                        "code":  "TEMPLATE_PROFILE_DIFFERS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "warnings_count":  1,
                                   "is_mirrored":  true,
                                   "validation_token":  "25156839ceb806ddafc3484c3f80aca0a96ae63510cf36e816cbb33c01ca3f1b",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N48",
                                   "work_point_mm":  [
                                                         54754.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  16.199999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135.59999999999999,
                                                       "element_id":  1251716,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.899999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251717,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  38.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251719,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322427,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3000000000000000,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.380000000000003,
                                                                         "element_id":  1251717,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135.56000000000000,
                                                                         "element_id":  1251716,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.070000000000000007,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.439999999999998,
                                                                         "element_id":  1251719,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251717 (44.4°, desvío 1.3°); ranura 1 (diagonal 135.6°) → barra 1251716 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) → barra 1251719 (-44.4°, desvío 0.1°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.4299999999999999,
                                                 "max_deviation_deg":  1.3000000000000000
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres",
                                   "element_ids":  [
                                                       1250938,
                                                       1251716,
                                                       1251717,
                                                       1251719
                                                   ],
                                   "member_element_ids":  [
                                                              1251716,
                                                              1251717,
                                                              1251719
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250938,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250938
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251720",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251720 (-43.1°, desvío 1.3°)",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) → barra 1251720 (-43.1°, desvío 1.3°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251720 (-43.1°, desvío 0.0°); ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-43)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N49",
                                   "work_point_mm":  [
                                                         57198.300000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  20.800000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -43.100000000000001,
                                                       "element_id":  1251720,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322428,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (1 barra, ángulos -43.1°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245542,
                                                       1251720
                                                   ],
                                   "member_element_ids":  [
                                                              1251720
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245542,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245542
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1251719",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1251719"
                                                ],
                                   "signature":  "1 +Y (91)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1251724). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N50].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N50",
                                   "work_point_mm":  [
                                                         57314.199999999997,
                                                         17204.200000000001,
                                                         14913.5
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  55.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  91.200000000000003,
                                                       "element_id":  1251719,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   }
                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1251724,
                                                       1251719
                                                   ],
                                   "member_element_ids":  [
                                                              1251719
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1251724,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N51",
                                   "work_point_mm":  [
                                                         57319.900000000001,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250939,
                                                       1250938
                                                   ],
                                   "member_element_ids":  [
                                                              1250938
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251717 (-135.6°, desvío 0.0°); barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1245542, 1251717",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251717 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1245542",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) → barra 1251717 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1245542"
                                                ],
                                   "signature":  "2 -Y (-180, -136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [
                                                    {
                                                        "message":  "Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan (1245544). Si es un extremo de cercha está bien; si falta el cordón en la selección, añádelo y replanifica.",
                                                        "hint":  "overrides.chord fija el cordón a mano.",
                                                        "path":  "nodes[N52].chord_element_id",
                                                        "code":  "NODE_CHORD_NOT_CONTINUOUS"
                                                    }
                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  1,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Falta el cordón",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N52",
                                   "work_point_mm":  [
                                                         57319.800000000003,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  0.10000000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -180,
                                                       "element_id":  1245542,
                                                       "type_name":  "HSS12X8X1/2",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  23.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251717,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322429,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  true,
                                   "advice":  "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…",
                                   "element_ids":  [
                                                       1245544,
                                                       1245542,
                                                       1251717
                                                   ],
                                   "member_element_ids":  [
                                                              1245542,
                                                              1251717
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  0,
                                   "spec":  null,
                                   "color_name":  "verde",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (137, 44) · 1 -Y (-136)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  "17ddac3d89c33a36bc4d9c28bbe57820659e278e5933a436278e24a417d59aa8",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "● Listo",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N53",
                                   "work_point_mm":  [
                                                         59880.300000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  84.5,
                                                       "reaches_node":  true,
                                                       "angle_deg":  136.90000000000001,
                                                       "element_id":  1251720,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  19.600000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251721,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  48.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251724,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322430,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  136.91999999999999,
                                                                         "element_id":  1251720,
                                                                         "template_angle_deg":  136.91999999999999,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.369999999999997,
                                                                         "element_id":  1251721,
                                                                         "template_angle_deg":  44.369999999999997,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "element_id":  1251724,
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "same: ranura 0 (diagonal 136.9°) → barra 1251720 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1251721 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1251724 (-135.6°, desvío 0.0°)",
                                                 "orientation":  "same",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  0.01,
                                                 "max_deviation_deg":  0
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "—",
                                   "element_ids":  [
                                                       1250939,
                                                       1251720,
                                                       1251721,
                                                       1251724
                                                   ],
                                   "member_element_ids":  [
                                                              1251720,
                                                              1251721,
                                                              1251724
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250939
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "same",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251721 (-135.6°, desvío 0.0°); barras sobrantes: 1251722",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) → barra 1251722 (-45.0°, desvío 0.6°); barras sobrantes: 1251721",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251721 (-135.6°, desvío 1.3°); ranura 1 (diagonal -44.4°) → barra 1251722 (-45.0°, desvío 0.6°); ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) → barra 1251722 (-45.0°, desvío 1.9°); ranura 1 (diagonal -135.6°) → barra 1251721 (-135.6°, desvío 0.0°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "2 -Y (-136, -45)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N54",
                                   "work_point_mm":  [
                                                         62472.099999999999,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  90.400000000000006,
                                                       "reaches_node":  false,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251721,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  27.100000000000001,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -45,
                                                       "element_id":  1251722,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322431,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (2 barras, ángulos -135.6°, -45°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245544,
                                                       1251721,
                                                       1251722
                                                   ],
                                   "member_element_ids":  [
                                                              1251721,
                                                              1251722
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245544
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  1.3500000000000001,
                                   "spec":  null,
                                   "color_name":  "verde",
                                   "attempts":  [

                                                ],
                                   "signature":  "2 +Y (135, 44) · 1 -Y (-44)",
                                   "status":  "ready",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS3X3X1-4 76x76",
                                   "warnings_count":  0,
                                   "is_mirrored":  true,
                                   "validation_token":  "3455521071d423efdfec66bad4eefe67308de41f1a84e6f452f5630e413a67a1",
                                   "template_name":  "Nudo tipico Detalle D",
                                   "existing_connection_id":  null,
                                   "status_text":  "● Listo",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N55",
                                   "work_point_mm":  [
                                                         65009.800000000003,
                                                         17204.200000000001,
                                                         17423
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  135,
                                                       "element_id":  1251722,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  6.2999999999999998,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "element_id":  1251723,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "+Y"
                                                   },
                                                   {
                                                       "end_gap_mm":  42.200000000000003,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -44.399999999999999,
                                                       "element_id":  1251725,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322432,
                                   "match":  {
                                                 "unassigned_members":  [

                                                                        ],
                                                 "assignments":  [
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  0,
                                                                         "deviation_deg":  1.3500000000000001,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  44.430000000000000,
                                                                         "element_id":  1251723,
                                                                         "template_angle_deg":  43.079999999999998,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  1,
                                                                         "deviation_deg":  0.63,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  135,
                                                                         "element_id":  1251722,
                                                                         "template_angle_deg":  135.63000000000000,
                                                                         "side":  "+Y"
                                                                     },
                                                                     {
                                                                         "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                                         "profile_policy":  "warn",
                                                                         "role":  "diagonal",
                                                                         "slot":  2,
                                                                         "deviation_deg":  0.01,
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "model_angle_deg":  -44.380000000000003,
                                                                         "element_id":  1251725,
                                                                         "template_angle_deg":  -44.369999999999997,
                                                                         "side":  "-Y"
                                                                     }
                                                                 ],
                                                 "description":  "mirror_x: ranura 0 (diagonal 43.1°) → barra 1251723 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) → barra 1251722 (135.0°, desvío 0.6°); ranura 2 (diagonal -44.4°) → barra 1251725 (-44.4°, desvío 0.0°)",
                                                 "orientation":  "mirror_x",
                                                 "matched_count":  3,
                                                 "is_complete":  true,
                                                 "score_deg":  1.9900000000000000,
                                                 "max_deviation_deg":  1.3500000000000001
                                             },
                                   "is_valid":  true,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "—",
                                   "element_ids":  [
                                                       1250939,
                                                       1251722,
                                                       1251723,
                                                       1251725
                                                   ],
                                   "member_element_ids":  [
                                                              1251722,
                                                              1251723,
                                                              1251725
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1250939,
                                   "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                   "through_element_ids":  [
                                                               1250939
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  "mirror_x",
                                   "has_spec_override":  false,
                                   "status_detail":  null
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N56",
                                   "work_point_mm":  [
                                                         67437.800000000003,
                                                         17204.099999999999,
                                                         17423
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1250939
                                                   ],
                                   "member_element_ids":  [
                                                              1250939
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N57",
                                   "work_point_mm":  [
                                                         67529.899999999994,
                                                         17204.200000000001,
                                                         14957.200000000001
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1251725
                                                   ],
                                   "member_element_ids":  [
                                                              1251725
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  "rojo",
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) → barra 1251723 (-135.6°, desvío 0.1°)",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1251723",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) → barra 1251723 (-135.6°, desvío 1.4°); ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) → barra 1251723 (-135.6°, desvío 0.1°); ranura 2 (diagonal 44.4°) sin barra"
                                                ],
                                   "signature":  "1 -Y (-136)",
                                   "status":  "no_match",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "✖ Sin plantilla que encaje",
                                   "color_rgb":  [
                                                     214,
                                                     45,
                                                     45
                                                 ],
                                   "errors":  [

                                              ],
                                   "name":  "N58",
                                   "work_point_mm":  [
                                                         67570,
                                                         17204.200000000001,
                                                         19933
                                                     ],
                                   "members":  [
                                                   {
                                                       "end_gap_mm":  23.699999999999999,
                                                       "reaches_node":  true,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1251723,
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "side":  "-Y"
                                                   }
                                               ],
                                   "marker_element_id":  1322433,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  true,
                                   "is_marked":  true,
                                   "advice":  "Ninguna plantilla encaja (1 barra, ángulos -135.6°): crea esa típica o excluye",
                                   "element_ids":  [
                                                       1245544,
                                                       1251723
                                                   ],
                                   "member_element_ids":  [
                                                              1251723
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  1245544,
                                   "template_id":  null,
                                   "through_element_ids":  [
                                                               1245544
                                                           ],
                                   "visible_by_default":  true,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo."
                               },
                               {
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "color_name":  null,
                                   "attempts":  [

                                                ],
                                   "signature":  "1 barra(s) sin marco",
                                   "status":  "untyped",
                                   "replaces_existing":  false,
                                   "warnings":  [

                                                ],
                                   "chord_type_name":  null,
                                   "warnings_count":  0,
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "template_name":  null,
                                   "existing_connection_id":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "errors":  [

                                              ],
                                   "name":  "N59",
                                   "work_point_mm":  [
                                                         67794.899999999994,
                                                         17204.099999999999,
                                                         19933
                                                     ],
                                   "members":  [

                                               ],
                                   "marker_element_id":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_continuous":  false,
                                   "is_marked":  false,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "element_ids":  [
                                                       1245544
                                                   ],
                                   "member_element_ids":  [
                                                              1245544
                                                          ],
                                   "errors_count":  0,
                                   "chord_element_id":  0,
                                   "template_id":  null,
                                   "through_element_ids":  [

                                                           ],
                                   "visible_by_default":  false,
                                   "is_manual":  false,
                                   "orientation":  null,
                                   "has_spec_override":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                               }
                           ],
                 "plan_id":  "6d218fa3-03ca-4c47-95d9-6da3a84540b5",
                 "summary":  {
                                 "no_match":  25,
                                 "ready":  16,
                                 "untyped":  18
                             },
                 "ready_count":  16,
                 "visible_count":  33,
                 "marks":  {
                               "marker_element_ids":  [
                                                          1322401,
                                                          1322402,
                                                          1322403,
                                                          1322404,
                                                          1322405,
                                                          1322406,
                                                          1322407,
                                                          1322408,
                                                          1322409,
                                                          1322410,
                                                          1322411,
                                                          1322412,
                                                          1322413,
                                                          1322414,
                                                          1322415,
                                                          1322416,
                                                          1322417,
                                                          1322418,
                                                          1322419,
                                                          1322420,
                                                          1322421,
                                                          1322422,
                                                          1322423,
                                                          1322424,
                                                          1322425,
                                                          1322426,
                                                          1322427,
                                                          1322428,
                                                          1322429,
                                                          1322430,
                                                          1322431,
                                                          1322432,
                                                          1322433
                                                      ],
                               "element_count":  64
                           },
                 "overrides":  {
                                   "add_member":  {

                                                  },
                                   "merge":  [

                                             ],
                                   "spec":  {

                                            },
                                   "exclude":  [

                                               ],
                                   "add_node":  {

                                                },
                                   "chord":  {

                                             },
                                   "remove_member":  {

                                                     },
                                   "template":  {

                                                },
                                   "replace_existing":  false,
                                   "split":  {

                                             }
                               }
             },
    "errors":  [

               ],
    "warnings":  [

                 ]
}

```

## 8d-4 sondeo 17 tras descartar desde la ventana

```text
== 17-marcas-plan.py -> HTTP 200 en 1054 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322434] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 8d-4 batch_plan_discard all

```text
== conn/batch_plan_discard -> HTTP 200 en 52 ms ==
{
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  22,
                 "operation":  "batch_plan_discard"
             },
    "ok":  true,
    "data":  {
                 "removed_markers":  0,
                 "discarded_plans":  0,
                 "remaining_markers":  0
             },
    "errors":  [

               ],
    "warnings":  [

                 ]
}

```

## 8d-5 catalogo vacio

```text

```

## 8d-5 catalogo restaurado

```text
== conn/catalog_list -> HTTP 200 en 427 ms ==
{
    "meta":  {
                 "addin_version":  "0.8.4",
                 "duration_ms":  4,
                 "operation":  "catalog_list"
             },
    "ok":  true,
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "members_count":  3,
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_drawing":  "Detalle D",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "name":  "Nudo tipico Detalle D",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "connection_type":  "gusset_node",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ]
                                   }
                               ]
             },
    "errors":  [

               ],
    "warnings":  [

                 ]
}

```

## 8d-6 sondeo 19 etiquetas v2

```text
== 19-etiquetas-lienzo.py -> HTTP 200 en 852 ms ==
=== 19-etiquetas-lienzo (v2: BMP y busqueda del manejador) ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | id=1245519
2) DB.TemporaryGraphicsManager existe: True
2) DB.InCanvasControlData existe: True
3) tipo Autodesk.Revit.DB.InCanvasControlData (clase): Dispose, ImagePath, IsValidObject, Position, get_ImagePath, get_IsValidObject, get_Position, set_ImagePath, set_Position
3) tipo Autodesk.Revit.DB.TemporaryGraphicsManager (clase): AddControl, Clear, Dispose, GetAll, GetTemporaryGraphicsManager, IsValidObject, RemoveControl, SetTooltip, SetVisibility, UpdateControl, get_IsValidObject
3) tipo Autodesk.Revit.UI.ITemporaryGraphicsHandler (interfaz): OnClick
3) tipo Autodesk.Revit.UI.TemporaryGraphicsCommandData (clase): Dispose, Document, Index, IsValidObject, get_Document, get_Index, get_IsValidObject
4) servicios externos integrados: 55 en total; con Temporary/Canvas en el nombre: TemporaryGraphicsHandlerService
4) servicio TemporaryGraphicsHandlerService: TemporaryGraphicsHandlerService (MultiServerService) | servidores registrados: 1
5) BMP generado: c:\users\andy bayona antón\appdata\local\temp\34737693-e5b4-4d14-8719-4c01b05cb191\motorconexiones-etiqueta-N4.bmp (9270 bytes)
6) TemporaryGraphicsManager obtenido: True
6) InCanvasControlData: ImagePath=c:\users\andy bayona antón\appdata\local\temp\34737693-e5b4-4d14-8719-4c01b05cb191\motorconexiones-etiqueta-N4.bmp | Position=(-11867.7, -17195.8, 17423.0) mm
7) Control anadido en la vista 1245519: indice 0
7) SetTooltip puesto
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8d-01-etiqueta.png (si la etiqueta no sale en la exportacion, mirala en pantalla y haz una captura a mano)
9) manejador registrado como servidor 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60 del servicio TemporaryGraphicsHandlerService (interfaz Autodesk.Revit.UI.ITemporaryGraphicsHandler): pincha la etiqueta en Revit; debe salir un cuadro y una linea en C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\log\sondeo19-clics.txt
10) La etiqueta se queda puesta (indice 0, vista 1245519) para que la persona la pinche; despues ejecuta 19b-etiquetas-quitar.py
11) Marcadores DirectShape de plan en el modelo (no los toca este sondeo): 0
=== fin 19-etiquetas-lienzo ===


```

## 8d-6 sondeo 19b quitar

```text
== 19b-etiquetas-quitar.py -> HTTP 200 en 499 ms ==
=== 19b-etiquetas-quitar ===
1) ningun clic anotado (C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\log\sondeo19-clics.txt no existe): o no se pincho la etiqueta, o el manejador no se registro, o Revit no avisa de los clics
2) estado del sondeo 19: control 0 en la vista 1245519
3) Control 0 quitado
4) TemporaryGraphicsManager.Clear() llamado: no queda ningun control
5) servidor de prueba 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60 desactivado en TemporaryGraphicsHandlerService (sigue registrado hasta reiniciar Revit)
6) Marcadores DirectShape de plan en el modelo (no los toca este sondeo): 0
=== fin 19b-etiquetas-quitar ===


```

## 8d-7 sondeo 17 marcadores

```text
== 17-marcas-plan.py -> HTTP 200 en 766 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322548] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 8d-7 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 257 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 9 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 12 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 8d-7 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 49 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 8d-8 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.8.4 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":18,"operation":"ping"},"ok":true,"data":{"document":{"is_workshared":false,"is_read_only":false,"title":"HANGAR_PRUEBA_sondeo","is_modifiable":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_family":false},"dotnet":{"framework":".NET 10.0.12","load_context":"Default","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"},"addin_version":"0.8.4","spec_version":"1.0","revit":{"version_number":"2027","language":"English_USA","sub_version_number":"2027.2","version_name":"Autodesk Revit 2027","version_build":"27.2.0.39"},"has_uidocument":true,"backend":"advancesteel","operations":["batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"]},"errors":[],"warnings":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 19152 caracteres
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":9,"operation":"guide"},"ok":true,"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Fluj ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":1,"operation":"types"},"ok":true,"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"errors":[],"warnings":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":1,"operation":"schema"},"ok":true,"data":{"json_schema":{"type":"object","additionalProperties":false,"title":"GussetNodeConnectionSpec","$schema":"http://json-schema.org/draft-07/schema#","required":["spec_version","connection_type","node","chord","gusset","members"],"properties":{"dimension_chains":{"items":{"properties":{"values_mm":{"items":{"type":"number"},"type":"array","minItems":1},"label":{"type":"string"},"expected_total_mm":{"type":"number","minimum":0.0}},"additionalProperties":false,"type":"object","required":["values_mm","expected_total_mm"]},"type":"array"},"node":{"properties":{"element_ids":{"items":{"type":"integer"},"type":"array","minItems":2}},"additionalProperties":false,"type":"object","required":["element_ids"]},"spec_version":{"enum":["1.0"],"type":"string"},"chord":{"properties":{"continuous":{"type":"boolean"},"profile":{"type":["string","null"]},"element_id":{"type":"integer"}},"additionalProperties":false,"type":"object","required":["element_id","continuous"]},"uncertain_fields":{"items":{"properties":{"user_confirmed_value":{},"reason":{"type":"string"},"path":{"type":"string"}},"additionalProperties":false,"type":"object","required":["path","reason"]},"type":"array"},"gusset":{"properties":{"chord_interface":{"enum":["through_slot","split_top_bottom","side_lap",null],"type":["string","null"]},"weld_to_chord":{"properties":{"size_mm":{"type":"number","minimum":1.0},"type":{"enum":["fillet"],"type":" ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":0,"operation":"schema"},"ok":false,"data":null,"errors":[{"message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","hint":"Tipos disponibles: gusset_node","path":"type","code":"UNKNOWN_OPERATION"}],"warnings":[]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"meta": {"addin_version": "0.8.4", "duration_ms": 8, "operation": "find_profile"}, "ok": true, "data": {"query": "HSS2-1/2X2-1/2X3/16", "total_profiles_in_model": 29, "matched_count": 1, "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}], "suggestions": []}, "errors": [], "warnings": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"meta": {"addin_version": "0.8.4", "duration_ms": 19, "operation": "node_info"}, "ok": true, "data": {"chord_direction_reversed": true, "existing_connections": [], "x_axis": [1, 1.9999999999999999e-06, 0], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "z_axis": [1.9999999999999999e-06, -1, 0], "y_axis": [0, 0, 1], "axis_distance_mm": 0.080000000000000002, "chord_element_id": 1249510, "origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "members": [{"node_end": 1, "type": "HSS3X3X1/4", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "family": "HSS-Hollow Structural Section", "structural_type": "Beam", "angle_in_plane_deg": 0, "is_chord": true, "length_mm": 9960.2999999999993, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "element_id": 1249510, "angle_to_chord_deg": 0, "slope_deg": 0, "side": "chord"}, {"node_end": 1, "type": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "structural_type": "Beam", "angle_in_plane_deg": 136.90000000000001, "is_chord": false, "length_mm": 3568, "material": "Material IFC (190-40-140)", "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "element_id": 1249630, "angle_to_chord_deg": 43.100000000000001, "slope_deg": 43.079999999999998, "side": "+Y" ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":31,"operation":"validate"},"ok":true,"data":{"warnings_count":1,"is_valid":true,"bolt_stacks":[{"gusset_face":"+z","bolt_length_mm":44.450000000000003,"grip_mm":19.524999999999999,"length_source":"computed_from_grip","member_element_id":1249636}],"calculated_values":{"chord_direction_reversed":true,"frame_x":[1,0,0],"frame_y":[0,0,1],"axis_distance_mm":0.080000000000000002,"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"frame_z":[0,-1,0]},"errors_count":0,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1"},"errors":[],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL"}]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":13,"operation":"validate"},"ok":false,"data":null,"errors":[{"message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","path":"dimension_chains[0].values_mm","code":"DIMENSION_CHAIN_MISMATCH"}],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":13,"operation":"validate"},"ok":false,"data":null,"errors":[{"message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[0].user_confirmed_value","code":"UNRESOLVED_UNCERTAINTY"},{"message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[1].user_confirmed_value","code":"UNRESOLVED_UNCERTAINTY"}],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"gusset_plates": 1, "weld_lines": 6, "working_point_mm": [-11867.7, -17195.8, 17423], "members_modified": 3, "first_member_element_id": 1249630, "chord_element_id": 1249510, "connection_type": "gusset_node", "backend": "advancesteel", "bolts": 4, "knife_plates": 1, "dry_run": true}
Cuerpo:
{"meta": {"addin_version": "0.8.4", "duration_ms": 16, "operation": "preview"}, "ok": true, "data": {"elements_to_create": [{"chord_interface": "through_slot", "vertices_count": 8, "width_mm": 565, "kind": "gusset_plate", "thickness_label": "3/8\"", "height_mm": 530, "thickness_mm": 9.5250000000000004}, {"kind": "welded_slot_interface", "for_member_id": 1249630, "weld_size_mm": 5, "slot_length_mm": 150}, {"kind": "welded_slot_interface", "for_member_id": 1249631, "weld_size_mm": 5, "slot_length_mm": 150}, {"for_member_id": 1249636, "gusset_face": "+z", "insertion_mm": 80, "offset_from_gusset_plane_mm": 9.7620000000000005, "width_mm": 140, "kind": "knife_plate", "length_mm": 170, "thickness_mm": 10}, {"rows": 2, "columns": 2, "diameter_mm": 15.875, "grip_mm": 19.524999999999999, "count": 4, "kind": "bolt_group", "length_mm": 44.450000000000003, "edge_mm": 40, "length_source": "computed_from_grip", "spacing_mm": 60, "for_member_id": 1249636}], "summary": {"gusset_plates": 1, "weld_lines": 6, "working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "members_modified": 3, "first_member_element_id": 1249630, "chord_element_id": 1249510, "connection_type": "gusset_node", "backend": "advancesteel", "bolts": 4, "knife_plates": 1, "dry_run": true}, "members_to_modify": [{"current_end_distance_mm": 86.200000000000003, "end": "end", "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "new_extension_mm": -93.799999999999997, "element_id": 1249630, "setback_mm": 180, ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":0,"operation":"create"},"ok":false,"data":null,"errors":[{"message":"validation_token es obligatorio para crear una conexi\u00f3n.","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","path":"validation_token","code":"VALIDATION_TOKEN_INVALID"}],"warnings":[]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"meta": {"addin_version": "0.8.4", "duration_ms": 4, "operation": "list"}, "ok": true, "data": {"connections_count": 0, "connections": []}, "errors": [], "warnings": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":5,"operation":"get"},"ok":false,"data":null,"errors":[{"message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","path":"connection_id","code":"ELEMENT_NOT_FOUND"}],"warnings":[]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":5,"operation":"delete"},"ok":false,"data":null,"errors":[{"message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Verifica los IDs disponibles con conn_list.","path":"connection_id","code":"ELEMENT_NOT_FOUND"}],"warnings":[]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":0,"operation":"no_existe"},"ok":false,"data":null,"errors":[{"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","path":null,"code":"UNKNOWN_OPERATION"}],"warnings":[]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":2,"operation":"catalog_list"},"ok":true,"data":{"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates_count":1,"templates":[{"origin_document":"HANGAR_PRUEBA_sondeo","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","members_count":3,"pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","origin_drawing":"Detalle D","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","name":"Nudo tipico Detalle D","created_utc":"2026-10-04T22:01:50.5585233Z","connection_type":"gusset_node","chord_profile":"HSS3X3X1/4","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","tags":["hangar","cercha","HSS"]}]},"errors":[],"warnings":[]}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=eac51671-95ee-4651-96c0-6ff2da536714 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\eac51671-95ee-4651-96c0-6ff2da536714.json
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":37,"operation":"catalog_save"},"ok":true,"data":{"members_count":3,"template_id":"eac51671-95ee-4651-96c0-6ff2da536714","shared_file":null,"member_pattern":[{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":0,"angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":1,"angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":2,"angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y"}],"name":"PRUEBA probar_conexiones","matching":{"allow_mirror":true,"angle_tolerance_deg":10},"origin":{"connection_id":null,"drawing":"Detalle D","document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636]},"chord_profile":"HSS3X3X1/4","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\eac51671-95ee-4651-96c0-6ff2da536714.json"},"errors":[],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL"}]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":1,"operation":"catalog_get"},"ok":true,"data":{"template_id":"eac51671-95ee-4651-96c0-6ff2da536714","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","name":"PRUEBA probar_conexiones","template":{"template_id":"eac51671-95ee-4651-96c0-6ff2da536714","member_pattern":[{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":0,"angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":1,"angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":2,"angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y"}],"description":null,"name":"PRUEBA probar_conexiones","matching":{"allow_mirror":true,"angle_tolerance_deg":10},"created_utc":"2026-10-06T00:21:57.5623125Z","origin":{"connection_id":null,"drawing":"Detalle D","document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636]},"spec_template":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"label":"borde superior","expected_total_mm":565.0},{"values_mm":[125.0,90.0,350.0],"label":"base","expected_total_mm":565.0},{"values_mm":[70.0,250.0,210.0],"label":"lado derecho","expected_total_mm":530.0},{"values_mm":[70.0,325.0,135.0],"l ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=956d018bddb8...
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":32,"operation":"catalog_apply"},"ok":true,"data":{"match":{"unassigned_members":[],"unmatched_slots":[],"assignments":[{"template_profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":0,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","model_angle_deg":136.91999999999999,"element_id":1249630,"template_angle_deg":136.91999999999999,"side":"+Y"},{"template_profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":1,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","model_angle_deg":44.369999999999997,"element_id":1249631,"template_angle_deg":44.369999999999997,"side":"+Y"},{"template_profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":2,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","model_angle_deg":-135.63000000000000,"element_id":1249636,"template_angle_deg":-135.63000000000000,"side":"-Y"}],"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0)","orientation":"same","matched_count":3,"is_complete":true,"score_deg":0.01,"max_deviation_deg":0},"template_id":"eac51671-95ee-4651-96c0-6ff2da536714","warnings_count":0,"spec":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"lab ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420 nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":35,"operation":"batch_plan"},"ok":true,"data":{"document":"HANGAR_PRUEBA_sondeo","updated_utc":"2026-10-06T00:21:57.7137538Z","hidden_text":"5 barras sueltas","unused_element_ids":[],"templates":{"eac51671-95ee-4651-96c0-6ff2da536714":"PRUEBA probar_conexiones"},"marked_view_id":null,"description":"6 nudo(s): 5 untyped, 1 ready.","is_marked":false,"selection_count":4,"created_utc":"2026-10-06T00:21:57.7137038Z","summary_text":"Se crear\u00e1 1 conexi\u00f3n con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.","nodes":[{"max_deviation_deg":null,"spec":null,"color_name":null,"attempts":[],"signature":"1 barra(s) sin marco","status":"untyped","replaces_existing":false,"warnings":[],"chord_type_name":null,"warnings_count":0,"is_mirrored":false,"validation_token":null,"template_name":null,"existing_connection_id":null,"status_text":"\u25cb Barra suelta (no es nudo)","color_rgb":null,"errors":[],"name":"N1","work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"members":[],"marker_element_id":null,"match":null,"is_valid":false,"chord_continuous":false,"is_marked":false,"advice":"No es un nudo: nada que hacer","element_ids":[1249630],"member_element_ids":[1249630],"errors_count":0,"chord_element_id":0,"template_id":null,"through_element_ids":[],"visible_by_default":false,"is_manual":false,"orientation":null,"has_spec_override":false,"status_detail":"Una sola barra llega y ninguna atravi ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=6e728f9696dc...
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":0,"operation":"batch_plan_get"},"ok":true,"data":{"plan_id":"f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420","node":{"max_deviation_deg":0,"spec":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"label":"borde superior","expected_total_mm":565.0},{"values_mm":[125.0,90.0,350.0],"label":"base","expected_total_mm":565.0},{"values_mm":[70.0,250.0,210.0],"label":"lado derecho","expected_total_mm":530.0},{"values_mm":[70.0,325.0,135.0],"label":"lado izquierdo","expected_total_mm":530.0}],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"spec_version":"1.0","chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"uncertain_fields":[],"gusset":{"chord_interface":"through_slot","weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"width_mm":565.0,"thickness_label":"3/8\"","height_mm":530.0,"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"thickness_mm":9.5250000000000004},"connection_type":"gusset_node","source":{"template_id":"eac51671-95ee-4651-96c0-6ff2da536714","drawing":"Detalle D","batch_id":"f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420","scale":"1/10"},"members":[{"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"weld":{"size_mm":5.0,"type":"fillet","all_around":true},"type":"welded_slot","slot_length_mm":150.0},"expected_angle_deg":43.100000000000001,"role":"diagonal","end_setback_mm":180.0,"element_ ...
======================================================================
24. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420
Cuerpo:
{"meta": {"addin_version": "0.8.4", "duration_ms": 15, "operation": "batch_plan_discard"}, "ok": true, "data": {"discarded_plan_id": "f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420", "removed_marks": 0, "remaining_plans": 0, "remaining_markers": 0}, "errors": [], "warnings": []}
======================================================================
25. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":1,"operation":"catalog_apply"},"ok":false,"data":null,"errors":[{"message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","path":"template_id","code":"TEMPLATE_NOT_FOUND"}],"warnings":[]}
======================================================================
26. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada eac51671-95ee-4651-96c0-6ff2da536714
Cuerpo:
{"meta":{"addin_version":"0.8.4","duration_ms":3,"operation":"catalog_delete"},"ok":true,"data":{"deleted_template_id":"eac51671-95ee-4651-96c0-6ff2da536714","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\eac51671-95ee-4651-96c0-6ff2da536714.json"},"errors":[],"warnings":[]}
======================================================================
27. tools/list por el puente trae las 21 herramientas conn_*  [OK]  HTTP 200, herramientas=87 conn_*=21
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard
======================================================================
28. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.8.4
Cuerpo:
{
  "meta": {
    "addin_version": "0.8.4",
    "duration_ms": 2,
    "operation": "ping"
  },
  "ok": true,
  "data": {
    "document": {
      "is_workshared": false,
      "is_read_only": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "is_modifiable": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_family": false
    },
    "dotnet": {
      "framework": ".NET 10.0.12",
      "load_context": "Default",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
    },
    "addin_version": "0.8.4",
    "spec_version": "1.0",
    "revit": {
      "version_number": "2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_name": "Autodesk Revit 2027",
      "version_build": "27.2.0.39"
    },
    "has_uidocument": true,
    "backend": "advancesteel",
    "operations": [
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
    ]
  },
  "errors": [],
  "warnings": []
}
======================================================================
Resultado: 28/28 pruebas correctas

```

## 8d-8 log del dia

```text

{"ts":"2026-10-05T17:32:09.2946650-05:00","record":{"event":"ribbon_batch_window","plan_id":"d50224e0-e138-4924-bbf1-53
88c0f129d3","action":"ShowInRevit","node":"N55","discarded":false}}
{"ts":"2026-10-05T17:32:16.9334050-05:00","record":{"event":"ribbon_batch_window","plan_id":"d50224e0-e138-4924-bbf1-53
88c0f129d3","action":"ShowInRevit","node":"N55","discarded":false}}
{"ts":"2026-10-05T17:32:19.9109735-05:00","record":{"event":"ribbon_batch_window","plan_id":"d50224e0-e138-4924-bbf1-53
88c0f129d3","action":"ShowInRevit","node":"N55","discarded":false}}
{"ts":"2026-10-05T17:32:54.0610188-05:00","record":{"event":"ribbon_batch_window_failed","error":"System.InvalidOperati
onException: Can only change SelectedItems collection in multiple selection modes. Use SelectedItem in single select 
modes.\r\n   at System.Windows.Controls.Primitives.Selector.OnSelectedItemsCollectionChanged(Object sender, 
NotifyCollectionChangedEventArgs e)\r\n   at 
System.Collections.ObjectModel.ObservableCollection`1.OnCollectionChanged(NotifyCollectionChangedEventArgs e)\r\n   at 
System.Windows.Controls.SelectedItemCollection.InsertItem(Int32 index, Object item)\r\n   at 
System.Collections.ObjectModel.Collection`1.System.Collections.IList.Add(Object value)\r\n   at 
MotorConexiones.Revit.UI.ChooseDialog..ctor(String title, String prompt, IEnumerable`1 items, Boolean multiSelect, 
Boolean allowPick) in D:\\Proyectos C#\\CONEXIONES\\src\\MotorConexiones.Revit\\UI\\ChooseDialog.xaml.cs:line 40\r\n   
at MotorConexiones.Revit.UI.BatchPlanWindow.OnChord(Object sender, RoutedEventArgs e) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\UI\\BatchPlanWindow.xaml.cs:line 362\r\n   at 
System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n   at 
System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)\r\n   at 
System.Windows.Controls.MenuItem.InvokeClickAfterRender(Object arg)\r\n   at 
System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)\r\n   at 
System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, 
Delegate catchHandler)\r\n   at System.Windows.Threading.DispatcherOperation.InvokeImpl()\r\n   at 
MS.Internal.CulturePreservingExecutionContext.CallbackWrapper(Object obj)\r\n   at 
System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object 
state)\r\n--- End of stack trace from previous location ---\r\n   at 
System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object 
state)\r\n   at System.Windows.Threading.DispatcherOperation.Invoke()\r\n   at 
System.Windows.Threading.Dispatcher.ProcessQueue()\r\n   at System.Windows.Threading.Dispatcher.WndProcHook(IntPtr 
hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n   at 
System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)\r\n   at 
System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, 
Delegate catchHandler)\r\n   at MS.Win32.HwndSubclass.SubclassWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr 
lParam)\r\n   at MS.Win32.UnsafeNativeMethods.DispatchMessage(MSG& msg)\r\n   at 
System.Windows.Threading.Dispatcher.PushFrameImpl(DispatcherFrame frame)\r\n   at 
System.Windows.Window.ShowHelper(Object booleanBox)\r\n   at System.Windows.Window.Show()\r\n   at 
System.Windows.Window.ShowDialog()\r\n   at MotorConexiones.Revit.BatchPlanCommand.Execute(ExternalCommandData 
commandData, String& message, ElementSet elements) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\BatchPlanCommand.cs:line 81"}}
{"ts":"2026-10-05T17:33:01.9015114-05:00","record":{"event":"ribbon_batch_show_hidden","plan_id":"d50224e0-e138-4924-bb
f1-5388c0f129d3","show":true}}
{"ts":"2026-10-05T17:33:10.0268268-05:00","record":{"event":"ribbon_batch_show_hidden","plan_id":"d50224e0-e138-4924-bb
f1-5388c0f129d3","show":false}}
{"ts":"2026-10-05T17:33:23.9002898-05:00","record":{"event":"ribbon_batch_replan_failed","plan_id":"d50224e0-e138-4924-
bbf1-5388c0f129d3","error":"Autodesk.Revit.Exceptions.InvalidOperationException: Invalid call to Revit API! Revit is 
currently not within an API context.\r\n   at Autodesk.Revit.UI.UIApplication.add_DialogBoxShowing(EventHandler`1 
handler)\r\n   at MotorConexiones.Revit.Transactions.OperationScope..ctor(Document document, UIApplication 
uiApplication, String operation, String connectionId, List`1 warnings) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Transactions\\OperationScope.cs:line 35\r\n   at 
MotorConexiones.Revit.Batch.BatchPlanner.Plan(Document document, UIApplication uiApplication, BatchPlanInput input, 
List`1 warnings) in D:\\Proyectos C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Batch\\BatchPlanner.cs:line 105\r\n   at 
MotorConexiones.Revit.UI.BatchPlanWindow.Replan(BatchOverrides delta, String status) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\UI\\BatchPlanWindow.xaml.cs:line 319"}}
{"ts":"2026-10-05T17:33:26.2757519-05:00","record":{"event":"ribbon_batch_plan_saved","plan_id":"d50224e0-e138-4924-bbf
1-5388c0f129d3","file":"C:\\Users\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\plan-d50224e0.json"}}
{"ts":"2026-10-05T17:35:07.2576723-05:00","record":{"event":"batch_plan","plan_id":"59b9ab15-67f8-4fc6-86da-90f29527e54
4","replan":false,"element_ids":65,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:35:14.1865851-05:00","record":{"event":"batch_plan_discard","plan_id":"59b9ab15-67f8-4fc6-86da-90f
29527e544","removed_marks":97}}
{"ts":"2026-10-05T17:35:14.2195293-05:00","record":{"event":"ribbon_batch_window","plan_id":"59b9ab15-67f8-4fc6-86da-90
f29527e544","action":"None","node":null,"discarded":true}}
{"ts":"2026-10-05T17:39:37.1791122-05:00","record":{"event":"batch_plan","plan_id":"ddaaba1a-837c-4406-ac6d-9d95a4c7905
2","replan":false,"element_ids":60,"templates":[],"summary":{"no_match":38,"untyped":20},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:39:42.1461781-05:00","record":{"event":"ribbon_batch_window","plan_id":"ddaaba1a-837c-4406-ac6d-9d
95a4c79052","action":"None","node":null,"discarded":false}}
{"ts":"2026-10-05T17:39:59.7994400-05:00","record":{"event":"batch_plan","plan_id":"0a1ca205-f9ef-4e56-9fb5-987bea5b674
f","replan":false,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:40:41.8173965-05:00","record":{"event":"ribbon_batch_catalog_opened","plan_id":"0a1ca205-f9ef-4e56
-9fb5-987bea5b674f","create_requested":false}}
{"ts":"2026-10-05T17:40:42.0533931-05:00","record":{"event":"batch_plan","plan_id":"0a1ca205-f9ef-4e56-9fb5-987bea5b674
f","replan":true,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"e
xclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":
{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:40:42.0541827-05:00","record":{"event":"ribbon_batch_replan","plan_id":"0a1ca205-f9ef-4e56-9fb5-98
7bea5b674f","summary":{"no_match":41,"untyped":18},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template
\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:41:05.3174546-05:00","record":{"event":"ribbon_batch_catalog_opened","plan_id":"0a1ca205-f9ef-4e56
-9fb5-987bea5b674f","create_requested":false}}
{"ts":"2026-10-05T17:41:05.5433467-05:00","record":{"event":"batch_plan","plan_id":"0a1ca205-f9ef-4e56-9fb5-987bea5b674
f","replan":true,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"e
xclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":
{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:41:05.5439565-05:00","record":{"event":"ribbon_batch_replan","plan_id":"0a1ca205-f9ef-4e56-9fb5-98
7bea5b674f","summary":{"no_match":41,"untyped":18},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template
\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:41:07.2907825-05:00","record":{"event":"ribbon_batch_window","plan_id":"0a1ca205-f9ef-4e56-9fb5-98
7bea5b674f","action":"None","node":null,"discarded":false}}
{"ts":"2026-10-05T17:41:22.1735142-05:00","record":{"event":"batch_plan","plan_id":"4ab6ee0f-01d3-45c7-ba1c-20270a2166a
e","replan":false,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:41:27.0011262-05:00","record":{"event":"ribbon_batch_window","plan_id":"4ab6ee0f-01d3-45c7-ba1c-20
270a2166ae","action":"None","node":null,"discarded":false}}
{"ts":"2026-10-05T17:41:51.3156743-05:00","record":{"event":"batch_plan","plan_id":"80b5b34a-a720-4fce-afc9-84f9de2da21
d","replan":false,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:48:56.8971899-05:00","record":{"event":"ribbon_batch_window","plan_id":"80b5b34a-a720-4fce-afc9-84
f9de2da21d","action":"None","node":null,"discarded":false}}
{"ts":"2026-10-05T17:49:15.7873103-05:00","record":{"event":"batch_plan","plan_id":"21a69b9e-da32-49c7-89d5-3efb9ebdc35
5","replan":false,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:49:15.7884261-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"inc
lude_specs\": 
false}","ok":true,"error_codes":[],"warning_codes":["PLAN_MARKS_REPLACED","CATALOG_EMPTY"],"duration_ms":291}}
{"ts":"2026-10-05T17:49:33.8884077-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{}
","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-05T17:49:34.5099853-05:00","record":{"event":"batch_plan_discard_all","markers":66,"orphans":0}}
{"ts":"2026-10-05T17:49:34.5221437-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"all\": true}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":298}}
{"ts":"2026-10-05T17:50:44.0258188-05:00","record":{"event":"batch_plan","plan_id":"dfdb8ae0-ae4a-4689-9a62-f751a060f5c
e","replan":false,"element_ids":4,"templates":["1371b61d-0746-44b8-858b-a79bba55c471"],"summary":{"untyped":5,"ready":1
},"marked":false,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_m
ember\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:50:44.0267231-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"ele
ment_ids\": [1249510, 1249630, 1249631, 1249636], \"template_ids\": [\"1371b61d-0746-44b8-858b-a79bba55c471\"], 
\"mark\": false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":40}}
{"ts":"2026-10-05T17:50:56.3193593-05:00","record":{"event":"batch_plan","plan_id":"8073d3ab-b714-465d-bf2e-df574d992c1
0","replan":false,"element_ids":4,"templates":["1371b61d-0746-44b8-858b-a79bba55c471"],"summary":{"untyped":5,"ready":1
},"marked":false,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_m
ember\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T17:50:56.3201841-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"ele
ment_ids\": [1249510, 1249630, 1249631, 1249636], \"template_ids\": [\"1371b61d-0746-44b8-858b-a79bba55c471\"], 
\"mark\": false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":32}}
{"ts":"2026-10-05T17:50:56.3748895-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"plan_id\": \"8073d3ab-b714-465d-bf2e-df574d992c10\", \"node\": 
\"N4\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-05T17:50:56.4448124-05:00","record":{"event":"batch_plan_discard","plan_id":"8073d3ab-b714-465d-bf2e-df5
74d992c10","removed_marks":0}}
{"ts":"2026-10-05T17:50:56.4517465-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"plan_id\": 
\"8073d3ab-b714-465d-bf2e-df574d992c10\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":16}}
{"ts":"2026-10-05T18:47:36.7447336-05:00","record":{"event":"startup","addin_version":"0.8.4","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-05T18:50:59.3815905-05:00","record":{"event":"batch_plan","plan_id":"667930ea-9925-4f36-b4d8-d486c50847f
c","replan":false,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T18:50:59.6269698-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"667930ea-9925-4f36-
b4d8-d486c50847fc","modeless":true,"selection":64}}
{"ts":"2026-10-05T18:51:40.6101535-05:00","record":{"event":"ribbon_batch_show_hidden","plan_id":"667930ea-9925-4f36-b4
d8-d486c50847fc","show":true}}
{"ts":"2026-10-05T18:51:41.4224707-05:00","record":{"event":"ribbon_batch_show_hidden","plan_id":"667930ea-9925-4f36-b4
d8-d486c50847fc","show":false}}
{"ts":"2026-10-05T18:51:45.4632609-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N54"}}
{"ts":"2026-10-05T18:53:45.8384690-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N54"}}
{"ts":"2026-10-05T18:53:52.1625022-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N25"}}
{"ts":"2026-10-05T18:53:56.4546005-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N6"}}
{"ts":"2026-10-05T18:53:58.0456508-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N9"}}
{"ts":"2026-10-05T18:54:38.4230037-05:00","record":{"event":"ribbon_batch_show","plan_id":"667930ea-9925-4f36-b4d8-d486
c50847fc","node":"N9"}}
{"ts":"2026-10-05T18:54:55.3362914-05:00","record":{"event":"ribbon_batch_window","plan_id":"667930ea-9925-4f36-b4d8-d4
86c50847fc","action":"closed","discarded":false}}
{"ts":"2026-10-05T18:55:19.0014348-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"667930ea-9925-4f36-
b4d8-d486c50847fc","modeless":true,"selection":0}}
{"ts":"2026-10-05T18:57:46.3149148-05:00","record":{"event":"startup","addin_version":"0.8.4","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-05T18:59:27.5822584-05:00","record":{"event":"batch_plan","plan_id":"e676902d-11e2-4d32-bb56-40aeef5243a
c","replan":false,"element_ids":56,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":23,"ready"
:16,"no_match":20},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T18:59:27.8617408-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"e676902d-11e2-4d32-
bb56-40aeef5243ac","modeless":true,"selection":56}}
{"ts":"2026-10-05T18:59:34.1162659-05:00","record":{"event":"ribbon_batch_window","plan_id":"e676902d-11e2-4d32-bb56-40
aeef5243ac","action":"closed","discarded":false}}
{"ts":"2026-10-05T18:59:53.3436691-05:00","record":{"event":"batch_plan","plan_id":"67936ac9-bca9-4fd9-a28b-dd1d8f680ac
6","replan":false,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T18:59:53.5580581-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"67936ac9-bca9-4fd9-
a28b-dd1d8f680ac6","modeless":true,"selection":64}}
{"ts":"2026-10-05T19:01:40.9994850-05:00","record":{"event":"startup","addin_version":"0.8.4","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-05T19:04:40.3514379-05:00","record":{"event":"batch_plan","plan_id":"6d218fa3-03ca-4c47-95d9-6da3a84540b
5","replan":false,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T19:04:40.3933559-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"inc
lude_specs\": false, \"template_ids\": 
[\"6abcf116-9b97-485f-b50d-2851ca0018cc\"]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":958}}
{"ts":"2026-10-05T19:05:56.8417181-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"6d218fa3-03ca-4c47-
95d9-6da3a84540b5","modeless":true,"selection":0}}
{"ts":"2026-10-05T19:07:09.8500107-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"include_specs\": false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-10-05T19:08:38.5747252-05:00","record":{"event":"batch_plan_discard","plan_id":"6d218fa3-03ca-4c47-95d9-6da
3a84540b5","from":"window","removed_marks":97,"other_plans_unmarked":0,"orphan_markers":0,"remaining_markers":0}}
{"ts":"2026-10-05T19:08:38.5767753-05:00","record":{"event":"ribbon_batch_discard","plan_id":"6d218fa3-03ca-4c47-95d9-6
da3a84540b5","result":"97 marca(s) quitadas; quedan 0 marcadores en el documento.","warnings":0}}
{"ts":"2026-10-05T19:09:11.2159094-05:00","record":{"event":"ribbon_batch_window","plan_id":"6d218fa3-03ca-4c47-95d9-6d
a3a84540b5","action":"closed","discarded":true}}
{"ts":"2026-10-05T19:10:02.5783947-05:00","record":{"event":"batch_plan_discard_all","markers":0,"orphans":0}}
{"ts":"2026-10-05T19:10:02.5868352-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"all\": true}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":22}}
{"ts":"2026-10-05T19:13:25.1981284-05:00","record":{"event":"batch_plan","plan_id":"ff066e37-c4b1-448d-8c0c-957239aac03
2","replan":false,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"
exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\"
:{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T19:13:25.3833748-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"ff066e37-c4b1-448d-
8c0c-957239aac032","modeless":true,"selection":64}}
{"ts":"2026-10-05T19:14:05.7681708-05:00","record":{"event":"ribbon_batch_catalog_opened","plan_id":"ff066e37-c4b1-448d
-8c0c-957239aac032","create_requested":false}}
{"ts":"2026-10-05T19:14:06.0063470-05:00","record":{"event":"batch_plan","plan_id":"ff066e37-c4b1-448d-8c0c-957239aac03
2","replan":true,"element_ids":64,"templates":[],"summary":{"no_match":41,"untyped":18},"marked":true,"overrides":"{\"e
xclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":
{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T19:14:06.0070684-05:00","record":{"event":"ribbon_batch_replan","plan_id":"ff066e37-c4b1-448d-8c0c-95
7239aac032","summary":{"no_match":41,"untyped":18},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template
\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T19:14:09.8422558-05:00","record":{"event":"batch_plan_discard","plan_id":"ff066e37-c4b1-448d-8c0c-957
239aac032","from":"window","removed_marks":97,"other_plans_unmarked":0,"orphan_markers":0,"remaining_markers":0}}
{"ts":"2026-10-05T19:14:09.8427744-05:00","record":{"event":"ribbon_batch_discard","plan_id":"ff066e37-c4b1-448d-8c0c-9
57239aac032","result":"97 marca(s) quitadas; quedan 0 marcadores en el documento.","warnings":0}}
{"ts":"2026-10-05T19:16:27.6733578-05:00","record":{"event":"ribbon_batch_show","plan_id":"ff066e37-c4b1-448d-8c0c-9572
39aac032","node":"N4"}}
{"ts":"2026-10-05T19:17:32.7547534-05:00","record":{"event":"ribbon_batch_show","plan_id":"ff066e37-c4b1-448d-8c0c-9572
39aac032","node":"N4"}}
{"ts":"2026-10-05T19:21:57.7405654-05:00","record":{"event":"batch_plan","plan_id":"f21be0fd-ebb1-4a4c-aee5-db6c7f1c642
0","replan":false,"element_ids":4,"templates":["eac51671-95ee-4651-96c0-6ff2da536714"],"summary":{"untyped":5,"ready":1
},"marked":false,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_m
ember\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T19:21:57.7414635-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"tem
plate_ids\": [\"eac51671-95ee-4651-96c0-6ff2da536714\"], \"element_ids\": [1249510, 1249630, 1249631, 1249636], 
\"mark\": false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":35}}
{"ts":"2026-10-05T19:21:57.7868118-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"plan_id\": \"f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420\", \"node\": 
\"N4\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-05T19:21:57.8476847-05:00","record":{"event":"batch_plan_discard","plan_id":"f21be0fd-ebb1-4a4c-aee5-db6
c7f1c6420","removed_marks":0}}
{"ts":"2026-10-05T19:21:57.8551792-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"plan_id\": 
\"f21be0fd-ebb1-4a4c-aee5-db6c7f1c6420\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":15}}



```

## 8d-9 git status antes del commit

```text
 M docs/fases/capturas/fase8-01-sondeo17.png
A  docs/fases/capturas/fase8d-01-etiqueta.png
A  docs/fases/capturas/fase8d-02-ventana-abierta.png
A  docs/fases/capturas/fase8d-crash-revit.png
?? docs/fases/resultados-fase-8d.md

```
