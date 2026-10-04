# Resultados de la Fase 8

Fecha: 2026-10-04T18:25:36


## 8-1 git

```text
4952f3a Fase 8: fusión con el cierre de la ronda 7b (README)
?? docs/fases/capturas/fase8-02-cinta.png
?? docs/fases/resultados-fase-8.md

```

## 8-2 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:02.24
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   150, Omitido:     0, Total:   150, Duración: 247 ms - MotorConexiones.Tests.dll (net10.0)

```

## 8-2 revit cerrado

```text

```

## 8-2 deploy

```text
== MotorConexiones 0.8.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 8-2 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (23 rutas @api.route)
- copiado tools\conn_tools.py (21 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 8-2 version de la dll

```text
0.8.0.0

```

## 8-3 ping

```text
== conn/ping -> HTTP 200 en 340 ms ==
{
    "data":  {
                 "spec_version":  "1.0",
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
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "sub_version_number":  "2027.2",
                               "language":  "English_USA",
                               "version_number":  "2027",
                               "version_build":  "27.2.0.39"
                           },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
                            },
                 "has_uidocument":  true,
                 "document":  {
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_workshared":  false,
                                  "is_modifiable":  false,
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_read_only":  false
                              },
                 "backend":  "advancesteel",
                 "addin_version":  "0.8.0"
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "operation":  "ping",
                 "duration_ms":  9,
                 "addin_version":  "0.8.0"
             },
    "ok":  true
}

```

## 8-3 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 85 ms ==
{
    "data":  {
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "origin_drawing":  "Detalle D",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "operation":  "catalog_list",
                 "duration_ms":  31,
                 "addin_version":  "0.8.0"
             },
    "ok":  true
}

```

## 8-3 sondeo 17 marcas

```text
== 17-marcas-plan.py -> HTTP 200 en 375 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0

ERROR PROBE_EXCEPTION: AttributeError: Name
PISTA: Lee el traceback en data.traceback.
TRACEBACK:
Traceback (most recent call last):
  File "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\revit_mcp\conexiones.py", line 346, in _ejecutar_sin_transaccion
    exec(codigo, espacio)
  File "<string>", line 67, in <module>
AttributeError: Name


```

## 8-4 batch_plan

```text
== conn/batch_plan -> HTTP 200 en 795 ms ==
{
    "data":  {
                 "marks":  {
                               "marker_element_ids":  [
                                                          1321375,
                                                          1321376,
                                                          1321377,
                                                          1321378
                                                      ],
                               "element_count":  8
                           },
                 "is_marked":  true,
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "ready_count":  0,
                 "nodes":  [
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249630
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14536.799999999999,
                                                         -17195.799999999999,
                                                         19918.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249630
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N1",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249636
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14455.299999999999,
                                                         -17195.700000000001,
                                                         14894.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249636
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N2",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14397.600000000000,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249510
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N3",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249630
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11930.600000000000,
                                                         -17195.799999999999,
                                                         17481.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249630
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N4",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249636
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11904.900000000000,
                                                         -17195.700000000001,
                                                         17389.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249636
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N5",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249631
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11856.5,
                                                         -17195.799999999999,
                                                         17437.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249631
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N6",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249631
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -9354.7000000000007,
                                                         -17195.799999999999,
                                                         19884.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249631
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N7",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249632
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -9277.7999999999993,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249632
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N8",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249637,
                                                       1249632
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1249632,
                                                       "reaches_node":  true,
                                                       "angle_deg":  179.40000000000001,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ],
                                   "signature":  "1 +Y (179)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "rojo",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249637,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     230,
                                                     25,
                                                     75
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1249632"
                                                ],
                                   "member_element_ids":  [
                                                              1249632
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321375,
                                   "name":  "N9",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510,
                                                       1249633
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1249633,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         -6745,
                                                         -17195.799999999999,
                                                         17418.700000000001
                                                     ],
                                   "signature":  "1 +Y (44)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249510
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "verde",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249510,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     60,
                                                     180,
                                                     75
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) → barra 1249633 (44.4°, desvío 0.1°); ranura 2 (diagonal -135.6°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) → barra 1249633 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1249633",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) → barra 1249633 (44.4°, desvío 0.1°)"
                                                ],
                                   "member_element_ids":  [
                                                              1249633
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321376,
                                   "name":  "N10",
                                   "chord_type_name":  "HSS3X3X1/4",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4437.3000000000002,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249510
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N11",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249637
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4220.3000000000002,
                                                         -17195.700000000001,
                                                         14957.200000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249637
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N12",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249633
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4197.1000000000004,
                                                         -17195.799999999999,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249633
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N13",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249626
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4163.1999999999998,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249626
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N14",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249638
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4141,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249638
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N15",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249509
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4022.5,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249509
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N16",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249626
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -1632.3000000000000,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249626
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N17",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249638,
                                                       1249627
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -1606.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249638,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249627
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N18",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249627
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         927.89999999999998,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249627
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N19",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249628
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         961.79999999999995,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249628
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N20",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249628,
                                                       1249639
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3477.1999999999998,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249628
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249628,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249639
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N21",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249639,
                                                       1249628
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3493.1999999999998,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249639
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249639,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249628
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N22",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249629
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3519,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249629
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N23",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249509
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         5812.6999999999998,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249509
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N24",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249639
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6024.8000000000002,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249639
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N25",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249629
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6052.8999999999996,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249629
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N26",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250263
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6086.8000000000002,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250263
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N27",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250267
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6109,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250267
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N28",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249511
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6227.5,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249511
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N29",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250263
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         8617.7000000000007,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250263
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N30",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250267,
                                                       1250264
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         8643.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250267,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250264
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N31",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250264
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         11177.900000000000,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250264
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N32",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250265
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         11211.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250265
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N33",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250265,
                                                       1250268
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13727.200000000001,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250265
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250265,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250268
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N34",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250268,
                                                       1250265
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13743.200000000001,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250268
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250268,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250265
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N35",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250266
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13769,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250266
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N36",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249511
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16062.700000000001,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249511
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N37",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250268
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16274.799999999999,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250268
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N38",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250266
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16302.900000000000,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250266
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N39",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250269
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16336.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250269
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N40",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250273
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16359,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250273
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N41",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250269
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         18867.700000000001,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250269
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N42",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250273,
                                                       1250270
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         18893.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250273,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250270
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N43",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250270
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         21427.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250270
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N44",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250271
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         21461.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250271
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N45",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250271,
                                                       1250274
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         23977.200000000001,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250271
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250271,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250274
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N46",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250274,
                                                       1250271
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         23993.200000000001,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250274
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250274,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250271
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N47",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250272
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         24019,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250272
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N48",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250274
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26524.799999999999,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250274
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N49",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250272
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26552.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250272
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N50",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250275
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26586.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250275
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N51",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250279
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26609,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250279
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N52",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250275
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         29117.700000000001,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250275
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N53",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250279,
                                                       1250276
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         29143.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250279,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250276
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N54",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250276
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         31677.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250276
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N55",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250277
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         31711.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250277
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N56",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250277,
                                                       1250280
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34227.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250277
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250277,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250280
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N57",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250280,
                                                       1250277
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34243.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250280
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250280,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250277
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N58",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250278
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34269,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250278
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N59",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250280
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36774.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250280
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N60",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250278
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36802.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250278
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N61",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250281
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36836.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250281
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N62",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250285
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36859,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250285
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N63",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250281
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         39367.699999999997,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250281
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N64",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250285,
                                                       1250282
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         39393.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250285,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250282
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N65",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250282
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         41927.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250282
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N66",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250283
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         41961.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250283
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N67",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250283,
                                                       1250286
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44477.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250283
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250283,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250286
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N68",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250286,
                                                       1250283
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44493.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250286
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250286,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250283
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N69",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250284
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44519,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250284
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N70",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250286
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47024.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250286
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N71",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250284
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47052.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250284
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N72",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250287
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47086.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250287
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N73",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250291
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47109,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250291
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N74",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249515
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47327.099999999999,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249515
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N75",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250287
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         49617.699999999997,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250287
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N76",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250291,
                                                       1250288
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         49643.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250291,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250288
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N77",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250288
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         52177.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250288
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N78",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250289
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         52211.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250289
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N79",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250289,
                                                       1250292
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54727.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250289
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250289,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250292
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N80",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250292,
                                                       1250289
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54743.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250292
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250292,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250289
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N81",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250290
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54769,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250290
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N82",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250293
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57213.5,
                                                         -17195.799999999999,
                                                         19918.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250293
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N83",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250292
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57274.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250292
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N84",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250297
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57294.900000000001,
                                                         -17195.700000000001,
                                                         14894.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250297
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N85",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250290
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57302.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250290
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N86",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516,
                                                       1249515
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57319.900000000001,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249516,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249515
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N87",
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250293
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59819.599999999999,
                                                         -17195.799999999999,
                                                         17481.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250293
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N88",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250297
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59845.300000000003,
                                                         -17195.700000000001,
                                                         17389.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250297
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N89",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250294
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59893.699999999997,
                                                         -17195.799999999999,
                                                         17437.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250294
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N90",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250294
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         62395.5,
                                                         -17195.799999999999,
                                                         19884.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250294
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N91",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250295
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         62472.400000000001,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250295
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N92",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250298,
                                                       1250295
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1250295,
                                                       "reaches_node":  true,
                                                       "angle_deg":  179.40000000000001,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         64979.900000000001,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ],
                                   "signature":  "1 +Y (179)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "azul",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250298,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     0,
                                                     130,
                                                     200
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1250295"
                                                ],
                                   "member_element_ids":  [
                                                              1250295
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321377,
                                   "name":  "N93",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516,
                                                       1250296
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1250296,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         65005.300000000003,
                                                         -17195.799999999999,
                                                         17418.700000000001
                                                     ],
                                   "signature":  "1 +Y (44)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249516
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "naranja",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249516,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     245,
                                                     130,
                                                     48
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) → barra 1250296 (44.4°, desvío 0.1°); ranura 2 (diagonal -135.6°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) → barra 1250296 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1250296",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) → barra 1250296 (44.4°, desvío 0.1°)"
                                                ],
                                   "member_element_ids":  [
                                                              1250296
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321378,
                                   "name":  "N94",
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67437.800000000003,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249516
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N95",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250298
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67529.899999999994,
                                                         -17195.700000000001,
                                                         14957.200000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250298
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N96",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250296
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67553.100000000006,
                                                         -17195.799999999999,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250296
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N97",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               }
                           ],
                 "selection_count":  53,
                 "updated_utc":  "2026-10-04T23:40:20.2525259Z",
                 "plan_id":  "01fd5226-37b8-4e8e-a82a-defa5895aa60",
                 "summary":  {
                                 "no_match":  4,
                                 "untyped":  93
                             },
                 "overrides":  {
                                   "exclude":  [

                                               ],
                                   "add_member":  {

                                                  },
                                   "IsEmpty":  true,
                                   "remove_member":  {

                                                     },
                                   "chord":  {

                                             },
                                   "split":  {

                                             },
                                   "add_node":  {

                                                },
                                   "spec":  {

                                            },
                                   "merge":  [

                                             ],
                                   "template":  {

                                                },
                                   "replace_existing":  false
                               },
                 "templates":  {
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "marked_view_id":  1245519,
                 "created_utc":  "2026-10-04T23:40:20.2521346Z",
                 "description":  "97 nudo(s): 93 untyped, 4 no_match.",
                 "unused_element_ids":  [
                                            1249509,
                                            1249511,
                                            1249515,
                                            1249626,
                                            1249627,
                                            1249628,
                                            1249629,
                                            1249630,
                                            1249631,
                                            1249636,
                                            1249638,
                                            1249639,
                                            1250263,
                                            1250264,
                                            1250265,
                                            1250266,
                                            1250267,
                                            1250268,
                                            1250269,
                                            1250270,
                                            1250271,
                                            1250272,
                                            1250273,
                                            1250274,
                                            1250275,
                                            1250276,
                                            1250277,
                                            1250278,
                                            1250279,
                                            1250280,
                                            1250281,
                                            1250282,
                                            1250283,
                                            1250284,
                                            1250285,
                                            1250286,
                                            1250287,
                                            1250288,
                                            1250289,
                                            1250290,
                                            1250291,
                                            1250292,
                                            1250293,
                                            1250294,
                                            1250297
                                        ]
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "operation":  "batch_plan",
                 "duration_ms":  289,
                 "addin_version":  "0.8.0"
             },
    "ok":  true
}

```

## 8-5 batch_plan_get N

```text
== conn/batch_plan_get -> HTTP 200 en 377 ms ==
{
    "data":  null,
    "warnings":  [

                 ],
    "errors":  [
                   {
                       "code":  "PLAN_NOT_FOUND",
                       "hint":  "Planifica con conn_batch_plan (la selección de la cercha y, si quieres, template_ids).",
                       "path":  "plan_id",
                       "message":  "No hay ningún plan con plan_id '35920' en memoria."
                   }
               ],
    "meta":  {
                 "operation":  "batch_plan_get",
                 "duration_ms":  0,
                 "addin_version":  "0.8.0"
             },
    "ok":  false
}
ERROR PLAN_NOT_FOUND: No hay ningún plan con plan_id '35920' en memoria.

```

## 8-5 replan excluir

```text
== conn/batch_plan -> HTTP 200 en 23 ms ==
{
    "data":  null,
    "warnings":  [

                 ],
    "errors":  [
                   {
                       "code":  "PLAN_NOT_FOUND",
                       "hint":  "Vuelve a planificar sin plan_id con la selección de la cercha.",
                       "path":  "plan_id",
                       "message":  "No hay ningún plan con plan_id '35920' en memoria (se descartó o Revit se reinició)."
                   }
               ],
    "meta":  {
                 "operation":  "batch_plan",
                 "duration_ms":  2,
                 "addin_version":  "0.8.0"
             },
    "ok":  false
}
ERROR PLAN_NOT_FOUND: No hay ningún plan con plan_id '35920' en memoria (se descartó o Revit se reinició).

```

## 8-5 replan incluir

```text
== conn/batch_plan -> HTTP 200 en 17 ms ==
{
    "data":  null,
    "warnings":  [

                 ],
    "errors":  [
                   {
                       "code":  "PLAN_NOT_FOUND",
                       "hint":  "Vuelve a planificar sin plan_id con la selección de la cercha.",
                       "path":  "plan_id",
                       "message":  "No hay ningún plan con plan_id '35920' en memoria (se descartó o Revit se reinició)."
                   }
               ],
    "meta":  {
                 "operation":  "batch_plan",
                 "duration_ms":  0,
                 "addin_version":  "0.8.0"
             },
    "ok":  false
}
ERROR PLAN_NOT_FOUND: No hay ningún plan con plan_id '35920' en memoria (se descartó o Revit se reinició).

```

## 8-7 batch_plan_get tras descartar

```text
== conn/batch_plan_get -> HTTP 200 en 514 ms ==
{
    "data":  {
                 "marks":  {
                               "marker_element_ids":  [
                                                          1321375,
                                                          1321376,
                                                          1321377,
                                                          1321378
                                                      ],
                               "element_count":  8
                           },
                 "is_marked":  true,
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "ready_count":  0,
                 "nodes":  [
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249630
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14536.799999999999,
                                                         -17195.799999999999,
                                                         19918.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249630
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N1",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249636
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14455.299999999999,
                                                         -17195.700000000001,
                                                         14894.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249636
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N2",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -14397.600000000000,
                                                         -17195.799999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249510
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N3",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249630
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11930.600000000000,
                                                         -17195.799999999999,
                                                         17481.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249630
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N4",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249636
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11904.900000000000,
                                                         -17195.700000000001,
                                                         17389.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249636
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N5",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249631
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -11856.5,
                                                         -17195.799999999999,
                                                         17437.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249631
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N6",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249631
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -9354.7000000000007,
                                                         -17195.799999999999,
                                                         19884.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249631
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N7",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249632
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -9277.7999999999993,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249632
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N8",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249637,
                                                       1249632
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1249632,
                                                       "reaches_node":  true,
                                                       "angle_deg":  179.40000000000001,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ],
                                   "signature":  "1 +Y (179)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "rojo",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249637,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     230,
                                                     25,
                                                     75
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1249632",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1249632"
                                                ],
                                   "member_element_ids":  [
                                                              1249632
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321375,
                                   "name":  "N9",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510,
                                                       1249633
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1249633,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         -6745,
                                                         -17195.799999999999,
                                                         17418.700000000001
                                                     ],
                                   "signature":  "1 +Y (44)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249510
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "verde",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249510,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     60,
                                                     180,
                                                     75
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) → barra 1249633 (44.4°, desvío 0.1°); ranura 2 (diagonal -135.6°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) → barra 1249633 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1249633",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) → barra 1249633 (44.4°, desvío 0.1°)"
                                                ],
                                   "member_element_ids":  [
                                                              1249633
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321376,
                                   "name":  "N10",
                                   "chord_type_name":  "HSS3X3X1/4",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249510
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4437.3000000000002,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249510
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N11",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249637
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4220.3000000000002,
                                                         -17195.700000000001,
                                                         14957.200000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249637
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N12",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249633
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4197.1000000000004,
                                                         -17195.799999999999,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249633
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N13",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249626
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4163.1999999999998,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249626
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N14",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249638
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4141,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249638
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N15",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249509
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -4022.5,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249509
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N16",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249626
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -1632.3000000000000,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249626
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N17",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249638,
                                                       1249627
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         -1606.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249638,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249627
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N18",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249627
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         927.89999999999998,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249627
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N19",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249628
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         961.79999999999995,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249628
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N20",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249628,
                                                       1249639
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3477.1999999999998,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249628
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249628,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249639
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N21",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249639,
                                                       1249628
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3493.1999999999998,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249639
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249639,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249628
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N22",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249629
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         3519,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249629
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N23",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249509
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         5812.6999999999998,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249509
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N24",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249639
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6024.8000000000002,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249639
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N25",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249629
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6052.8999999999996,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249629
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N26",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250263
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6086.8000000000002,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250263
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N27",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250267
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6109,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250267
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N28",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249511
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         6227.5,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249511
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N29",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250263
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         8617.7000000000007,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250263
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N30",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250267,
                                                       1250264
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         8643.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250267,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250264
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N31",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250264
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         11177.900000000000,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250264
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N32",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250265
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         11211.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250265
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N33",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250265,
                                                       1250268
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13727.200000000001,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250265
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250265,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250268
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N34",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250268,
                                                       1250265
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13743.200000000001,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250268
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250268,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250265
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N35",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250266
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         13769,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250266
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N36",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249511
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16062.700000000001,
                                                         -17195.700000000001,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249511
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N37",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250268
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16274.799999999999,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250268
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N38",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250266
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16302.900000000000,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250266
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N39",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250269
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16336.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250269
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N40",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250273
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         16359,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250273
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N41",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250269
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         18867.700000000001,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250269
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N42",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250273,
                                                       1250270
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         18893.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250273,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250270
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N43",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250270
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         21427.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250270
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N44",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250271
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         21461.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250271
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N45",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250271,
                                                       1250274
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         23977.200000000001,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250271
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250271,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250274
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N46",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250274,
                                                       1250271
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         23993.200000000001,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250274
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250274,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250271
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N47",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250272
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         24019,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250272
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N48",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250274
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26524.799999999999,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250274
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N49",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250272
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26552.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250272
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N50",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250275
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26586.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250275
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N51",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250279
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         26609,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250279
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N52",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250275
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         29117.700000000001,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250275
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N53",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250279,
                                                       1250276
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         29143.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250279,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250276
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N54",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250276
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         31677.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250276
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N55",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250277
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         31711.799999999999,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250277
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N56",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250277,
                                                       1250280
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34227.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250277
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250277,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250280
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N57",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250280,
                                                       1250277
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34243.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250280
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250280,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250277
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N58",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250278
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         34269,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250278
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N59",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250280
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36774.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250280
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N60",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250278
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36802.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250278
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N61",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250281
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36836.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250281
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N62",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250285
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         36859,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250285
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N63",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250281
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         39367.699999999997,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250281
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N64",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250285,
                                                       1250282
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         39393.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250285,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250282
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N65",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250282
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         41927.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250282
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N66",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250283
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         41961.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250283
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N67",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250283,
                                                       1250286
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44477.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250283
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250283,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250286
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N68",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250286,
                                                       1250283
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44493.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250286
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250286,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250283
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N69",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250284
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         44519,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250284
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N70",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250286
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47024.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250286
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N71",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250284
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47052.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250284
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N72",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250287
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47086.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250287
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N73",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250291
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47109,
                                                         -17195.700000000001,
                                                         14958.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250291
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N74",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249515
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         47327.099999999999,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249515
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N75",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250287
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         49617.699999999997,
                                                         -17195.700000000001,
                                                         17433.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250287
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N76",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250291,
                                                       1250288
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         49643.5,
                                                         -17195.700000000001,
                                                         17437.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250291,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250288
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N77",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250288
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         52177.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250288
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N78",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250289
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         52211.800000000003,
                                                         -17195.700000000001,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250289
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N79",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250289,
                                                       1250292
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54727.199999999997,
                                                         -17195.700000000001,
                                                         17450
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250289
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250289,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250292
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N80",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250292,
                                                       1250289
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54743.199999999997,
                                                         -17195.700000000001,
                                                         17434.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1250292
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250292,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250289
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N81",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250290
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         54769,
                                                         -17195.700000000001,
                                                         17436.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250290
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N82",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250293
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57213.5,
                                                         -17195.799999999999,
                                                         19918.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250293
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N83",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250292
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57274.800000000003,
                                                         -17195.700000000001,
                                                         14952.100000000000
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250292
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N84",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250297
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57294.900000000001,
                                                         -17195.700000000001,
                                                         14894.700000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250297
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N85",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250290
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57302.900000000001,
                                                         -17195.700000000001,
                                                         19916.5
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250290
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N86",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516,
                                                       1249515
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         57319.900000000001,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Todas las barras son paralelas al cordón: no definen el plano de la cercha.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249516,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249515
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N87",
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250293
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59819.599999999999,
                                                         -17195.799999999999,
                                                         17481.799999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250293
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N88",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250297
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59845.300000000003,
                                                         -17195.700000000001,
                                                         17389.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250297
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N89",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250294
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         59893.699999999997,
                                                         -17195.799999999999,
                                                         17437.299999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250294
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N90",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250294
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         62395.5,
                                                         -17195.799999999999,
                                                         19884.900000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250294
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N91",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250295
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         62472.400000000001,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250295
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N92",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250298,
                                                       1250295
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1250295,
                                                       "reaches_node":  true,
                                                       "angle_deg":  179.40000000000001,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         64979.900000000001,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ],
                                   "signature":  "1 +Y (179)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "azul",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1250298,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     0,
                                                     130,
                                                     200
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) sin barra; ranura 2 (diagonal -135.6°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) sin barra; ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1250295",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) sin barra; barras sobrantes: 1250295"
                                                ],
                                   "member_element_ids":  [
                                                              1250295
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321377,
                                   "name":  "N93",
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516,
                                                       1250296
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [
                                                   {
                                                       "element_id":  1250296,
                                                       "reaches_node":  true,
                                                       "angle_deg":  44.399999999999999,
                                                       "side":  "+Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64"
                                                   }
                                               ],
                                   "work_point_mm":  [
                                                         65005.300000000003,
                                                         -17195.799999999999,
                                                         17418.700000000001
                                                     ],
                                   "signature":  "1 +Y (44)",
                                   "is_manual":  false,
                                   "status_detail":  "Ninguna plantilla casa todas sus ranuras con las barras del nudo.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [
                                                               1249516
                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  "naranja",
                                   "warnings_count":  0,
                                   "is_marked":  true,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  1249516,
                                   "status":  "no_match",
                                   "color_rgb":  [
                                                     245,
                                                     130,
                                                     48
                                                 ],
                                   "attempts":  [
                                                    "Nudo tipico Detalle D → same: ranura 0 (diagonal 136.9°) sin barra; ranura 1 (diagonal 44.4°) → barra 1250296 (44.4°, desvío 0.1°); ranura 2 (diagonal -135.6°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_x: ranura 0 (diagonal 43.1°) → barra 1250296 (44.4°, desvío 1.4°); ranura 1 (diagonal 135.6°) sin barra; ranura 2 (diagonal -44.4°) sin barra",
                                                    "Nudo tipico Detalle D → mirror_y: ranura 0 (diagonal -136.9°) sin barra; ranura 1 (diagonal -44.4°) sin barra; ranura 2 (diagonal 135.6°) sin barra; barras sobrantes: 1250296",
                                                    "Nudo tipico Detalle D → both: ranura 0 (diagonal -43.1°) sin barra; ranura 1 (diagonal -135.6°) sin barra; ranura 2 (diagonal 44.4°) → barra 1250296 (44.4°, desvío 0.1°)"
                                                ],
                                   "member_element_ids":  [
                                                              1250296
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  1321378,
                                   "name":  "N94",
                                   "chord_type_name":  "HSS4X4X3-16 102x102",
                                   "spec":  null,
                                   "chord_continuous":  true
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1249516
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67437.800000000003,
                                                         -17195.599999999999,
                                                         17423
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1249516
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N95",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250298
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67529.899999999994,
                                                         -17195.700000000001,
                                                         14957.200000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250298
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N96",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               },
                               {
                                   "template_name":  null,
                                   "errors":  [

                                              ],
                                   "has_spec_override":  false,
                                   "element_ids":  [
                                                       1250296
                                                   ],
                                   "max_deviation_deg":  null,
                                   "members":  [

                                               ],
                                   "work_point_mm":  [
                                                         67553.100000000006,
                                                         -17195.799999999999,
                                                         19916.400000000001
                                                     ],
                                   "signature":  "1 barra(s) sin marco",
                                   "is_manual":  false,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "replaces_existing":  false,
                                   "through_element_ids":  [

                                                           ],
                                   "validation_token":  null,
                                   "warnings":  [

                                                ],
                                   "is_mirrored":  false,
                                   "color_name":  null,
                                   "warnings_count":  0,
                                   "is_marked":  false,
                                   "errors_count":  0,
                                   "orientation":  null,
                                   "match":  null,
                                   "is_valid":  false,
                                   "chord_element_id":  0,
                                   "status":  "untyped",
                                   "color_rgb":  null,
                                   "attempts":  [

                                                ],
                                   "member_element_ids":  [
                                                              1250296
                                                          ],
                                   "template_id":  null,
                                   "existing_connection_id":  null,
                                   "marker_element_id":  null,
                                   "name":  "N97",
                                   "chord_type_name":  null,
                                   "spec":  null,
                                   "chord_continuous":  false
                               }
                           ],
                 "selection_count":  53,
                 "updated_utc":  "2026-10-04T23:40:20.2525259Z",
                 "plan_id":  "01fd5226-37b8-4e8e-a82a-defa5895aa60",
                 "summary":  {
                                 "no_match":  4,
                                 "untyped":  93
                             },
                 "overrides":  {
                                   "exclude":  [

                                               ],
                                   "add_member":  {

                                                  },
                                   "IsEmpty":  true,
                                   "remove_member":  {

                                                     },
                                   "chord":  {

                                             },
                                   "split":  {

                                             },
                                   "add_node":  {

                                                },
                                   "spec":  {

                                            },
                                   "merge":  [

                                             ],
                                   "template":  {

                                                },
                                   "replace_existing":  false
                               },
                 "templates":  {
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "marked_view_id":  1245519,
                 "created_utc":  "2026-10-04T23:40:20.2521346Z",
                 "description":  "97 nudo(s): 93 untyped, 4 no_match.",
                 "unused_element_ids":  [
                                            1249509,
                                            1249511,
                                            1249515,
                                            1249626,
                                            1249627,
                                            1249628,
                                            1249629,
                                            1249630,
                                            1249631,
                                            1249636,
                                            1249638,
                                            1249639,
                                            1250263,
                                            1250264,
                                            1250265,
                                            1250266,
                                            1250267,
                                            1250268,
                                            1250269,
                                            1250270,
                                            1250271,
                                            1250272,
                                            1250273,
                                            1250274,
                                            1250275,
                                            1250276,
                                            1250277,
                                            1250278,
                                            1250279,
                                            1250280,
                                            1250281,
                                            1250282,
                                            1250283,
                                            1250284,
                                            1250285,
                                            1250286,
                                            1250287,
                                            1250288,
                                            1250289,
                                            1250290,
                                            1250291,
                                            1250292,
                                            1250293,
                                            1250294,
                                            1250297
                                        ]
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "operation":  "batch_plan_get",
                 "duration_ms":  0,
                 "addin_version":  "0.8.0"
             },
    "ok":  true
}

```

## 8-7 batch_plan_discard all

```text
== conn/batch_plan_discard -> HTTP 200 en 92 ms ==
{
    "data":  {
                 "remaining_markers":  0,
                 "discarded_plans":  1,
                 "removed_markers":  0
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "operation":  "batch_plan_discard",
                 "duration_ms":  72,
                 "addin_version":  "0.8.0"
             },
    "ok":  true
}

```

## 8-7 sondeo 17 marcadores

```text
== 17-marcas-plan.py -> HTTP 200 en 98 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0

ERROR PROBE_EXCEPTION: AttributeError: Name
PISTA: Lee el traceback en data.traceback.
TRACEBACK:
Traceback (most recent call last):
  File "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\revit_mcp\conexiones.py", line 346, in _ejecutar_sin_transaccion
    exec(codigo, espacio)
  File "<string>", line 67, in <module>
AttributeError: Name


```

## 8-7 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 173 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 11 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 8-7 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 53 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 8-7 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.8.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"data":{"spec_version":"1.0","operations":["batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"revit":{"version_name":"Autodesk Revit 2027","sub_version_number":"2027.2","language":"English_USA","version_number":"2027","version_build":"27.2.0.39"},"dotnet":{"framework":".NET 10.0.12","load_context":"Default","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"},"has_uidocument":true,"document":{"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_workshared":false,"is_modifiable":false,"is_family":false,"title":"HANGAR_PRUEBA_sondeo","is_read_only":false},"backend":"advancesteel","addin_version":"0.8.0"},"warnings":[],"errors":[],"meta":{"operation":"ping","duration_ms":3,"addin_version":"0.8.0"},"ok":true}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 15842 caracteres
Cuerpo:
{"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"warnings":[],"errors":[],"meta":{"operation":"types","duration_ms":1,"addin_version":"0.8.0"},"ok":true}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"data":{"connection_type":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","example":{"uncertain_fields":[],"connection_type":"gusset_node","spec_version":"1.0","node":{"element_ids":[1249510,1249630,1249631,1249636]},"chord":{"continuous":true,"element_id":1249510,"profile":"HSS3X3X1/4"},"source":{"drawing":"Detalle D","scale":"1/10"},"dimension_chains":[{"expected_total_mm":565.0,"label":"borde superior","values_mm":[75.0,420.0,70.0]}],"members":[{"element_id":1249630,"attachment":{"type":"welded_slot","weld":{"type":"fillet","size_mm":5.0,"all_around":true},"slot_length_mm":150.0},"end_setback_mm":180.0,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","expected_angle_deg":45.0}],"gusset":{"width_mm":565.0,"height_mm":530.0,"weld_to_chord":{"type":"fillet","size_mm":5.0,"all_around":true},"chord_interface":"through_slot","thickness_mm":9.5250000000000004,"thickness_label":"3/8\"","outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]}}},"json_schema":{"$schema":"http://json-schema.org/draft-07/schema#","required":["spec_version","connection_type","node","chord","gusset","members"],"title":"GussetNodeConnectionSpec","properties":{"uncertain_fields":{"items":{"properties":{"reason":{"type":"string"},"path":{"type":"string"},"user_confirmed_value":{}},"typ ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"UNKNOWN_OPERATION","hint":"Tipos disponibles: gusset_node","path":"type","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado."}],"meta":{"operation":"schema","duration_ms":0,"addin_version":"0.8.0"},"ok":false}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"data": {"matched_count": 1, "query": "HSS2-1/2X2-1/2X3/16", "suggestions": [], "matches": [{"exact_match": false, "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "total_profiles_in_model": 29}, "warnings": [], "errors": [], "meta": {"operation": "find_profile", "duration_ms": 15, "addin_version": "0.8.0"}, "ok": true}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cord�n=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"data": {"chord_element_id": 1249510, "origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "x_axis": [1, 1.9999999999999999e-06, 0], "axis_distance_mm": 0.080000000000000002, "existing_connections": [], "chord_direction_reversed": true, "members": [{"element_id": 1249510, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "angle_in_plane_deg": 0, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "angle_to_chord_deg": 0, "is_chord": true, "length_mm": 9960.2999999999993, "node_end": 1, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "structural_type": "Beam", "slope_deg": 0, "type": "HSS3X3X1/4", "side": "chord", "family": "HSS-Hollow Structural Section"}, {"element_id": 1249630, "material": "Material IFC (190-40-140)", "angle_in_plane_deg": 136.90000000000001, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "angle_to_chord_deg": 43.100000000000001, "is_chord": false, "length_mm": 3568, "node_end": 1, "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "structural_type": "Beam", "slope_deg": 43.079999999999998, "type": "HSS2-1-2X2-1-2X3-16 64x64", "side": "+Y", "family": "HSS2-1-2X2-1-2X3-16 64x64"}, {"element_id": 1249631, "material": "Material IFC (190-40-140)", "angle_in_plane_deg": 44.399999999999999, "end_mm": [-9354.7000000000007, -17195.79999999 ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"data":{"warnings_count":1,"bolt_stacks":[{"length_source":"computed_from_grip","gusset_face":"+z","grip_mm":19.524999999999999,"member_element_id":1249636,"bolt_length_mm":44.450000000000003}],"errors_count":0,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","calculated_values":{"frame_y":[0,0,1],"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"frame_x":[1,0,0],"chord_direction_reversed":true},"is_valid":true},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}],"errors":[],"meta":{"operation":"validate","duration_ms":126,"addin_version":"0.8.0"},"ok":true}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}],"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","path":"dimension_chains[0].values_mm","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm)."}],"meta":{"operation":"validate","duration_ms":29,"addin_version":"0.8.0"},"ok":false}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}],"errors":[{"code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[0].user_confirmed_value","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen"},{"code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[1].user_confirmed_value","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n"}],"meta":{"operation":"validate","duration_ms":11,"addin_version":"0.8.0"},"ok":false}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"chord_element_id": 1249510, "first_member_element_id": 1249630, "connection_type": "gusset_node", "members_modified": 3, "weld_lines": 6, "backend": "advancesteel", "knife_plates": 1, "bolts": 4, "gusset_plates": 1, "dry_run": true, "working_point_mm": [-11867.7, -17195.8, 17423]}
Cuerpo:
{"data": {"elements_to_create": [{"kind": "gusset_plate", "vertices_count": 8, "width_mm": 565, "height_mm": 530, "chord_interface": "through_slot", "thickness_mm": 9.5250000000000004, "thickness_label": "3/8\""}, {"kind": "welded_slot_interface", "for_member_id": 1249630, "weld_size_mm": 5, "slot_length_mm": 150}, {"kind": "welded_slot_interface", "for_member_id": 1249631, "weld_size_mm": 5, "slot_length_mm": 150}, {"kind": "knife_plate", "for_member_id": 1249636, "offset_from_gusset_plane_mm": 9.7620000000000005, "width_mm": 140, "length_mm": 170, "insertion_mm": 80, "gusset_face": "+z", "thickness_mm": 10}, {"kind": "bolt_group", "for_member_id": 1249636, "columns": 2, "length_mm": 44.450000000000003, "diameter_mm": 15.875, "length_source": "computed_from_grip", "rows": 2, "grip_mm": 19.524999999999999, "spacing_mm": 60, "edge_mm": 40, "count": 4}], "members_to_modify": [{"element_id": 1249630, "end": "end", "current_end_distance_mm": 86.200000000000003, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "setback_mm": 180, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -93.799999999999997}, {"element_id": 1249631, "end": "start", "current_end_distance_mm": 18, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "setback_mm": 60, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -42}, {"element_id": 1249636, "end": "en ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"VALIDATION_TOKEN_INVALID","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","path":"validation_token","message":"validation_token es obligatorio para crear una conexi\u00f3n."}],"meta":{"operation":"create","duration_ms":1,"addin_version":"0.8.0"},"ok":false}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"data": {"connections": [], "connections_count": 0}, "warnings": [], "errors": [], "meta": {"operation": "list", "duration_ms": 4, "addin_version": "0.8.0"}, "ok": true}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"ELEMENT_NOT_FOUND","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","path":"connection_id","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'."}],"meta":{"operation":"get","duration_ms":5,"addin_version":"0.8.0"},"ok":false}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"ELEMENT_NOT_FOUND","hint":"Verifica los IDs disponibles con conn_list.","path":"connection_id","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'."}],"meta":{"operation":"delete","duration_ms":5,"addin_version":"0.8.0"},"ok":false}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"UNKNOWN_OPERATION","hint":"Operaciones disponibles: batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","path":null,"message":"La operaci\u00f3n 'no_existe' no existe en el add-in."}],"meta":{"operation":"no_existe","duration_ms":0,"addin_version":"0.8.0"},"ok":false}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Ant�n\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"data":{"templates_count":1,"templates":[{"connection_type":"gusset_node","members_count":3,"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","origin_document":"HANGAR_PRUEBA_sondeo","origin_drawing":"Detalle D","chord_profile":"HSS3X3X1/4","name":"Nudo tipico Detalle D","tags":["hangar","cercha","HSS"],"created_utc":"2026-10-04T22:01:50.5585233Z","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc"}],"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog"},"warnings":[],"errors":[],"meta":{"operation":"catalog_list","duration_ms":2,"addin_version":"0.8.0"},"ok":true}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f barras=3 archivo=C:\Users\Andy Bayona Ant�n\AppData\Local\MotorConexiones\catalogo\d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f.json
Cuerpo:
{"data":{"origin":{"connection_id":null,"drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo"},"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f.json","members_count":3,"member_pattern":[{"profile_policy":"warn","angle_deg":136.91999999999999,"profile":"HSS2-1/2X2-1/2X3/16","slot":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"+Y"},{"profile_policy":"warn","angle_deg":44.369999999999997,"profile":"HSS2-1/2X2-1/2X3/16","slot":1,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"+Y"},{"profile_policy":"warn","angle_deg":-135.63000000000000,"profile":"HSS2-1/2X2-1/2X3/16","slot":2,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"-Y"}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"shared_file":null,"chord_profile":"HSS3X3X1/4","name":"PRUEBA probar_conexiones","template_id":"d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f"},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}],"errors":[],"meta":{"operation":"catalog_save","duration_ms":47,"addin_version":"0.8.0"},"ok":true}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patr�n=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"data":{"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f.json","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","name":"PRUEBA probar_conexiones","template":{"origin":{"connection_id":null,"drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo"},"connection_type":"gusset_node","catalog_version":"1.0","member_pattern":[{"profile_policy":"warn","angle_deg":136.91999999999999,"profile":"HSS2-1/2X2-1/2X3/16","slot":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"+Y"},{"profile_policy":"warn","angle_deg":44.369999999999997,"profile":"HSS2-1/2X2-1/2X3/16","slot":1,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"+Y"},{"profile_policy":"warn","angle_deg":-135.63000000000000,"profile":"HSS2-1/2X2-1/2X3/16","slot":2,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","side":"-Y"}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"name":"PRUEBA probar_conexiones","tags":["prueba"],"spec_template":{"uncertain_fields":[],"connection_type":"gusset_node","spec_version":"1.0","chord":{"continuous":true,"profile":"HSS3X3X1/4"},"source":{"drawing":"Detalle D","scale":"1/10"},"dimension_chains":[{"expected_total_mm":565.0,"label":"borde superior","values_mm":[75.0,420.0,70.0]},{"expected_total_mm":565.0,"label":"base","values_mm":[125.0,90.0,35 ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientaci�n same  [OK]  HTTP 200, ok=True, orientaci�n=same desv�o_m�x=0 token=3a2f14df8e76...
Cuerpo:
{"data":{"is_valid":true,"warnings_count":0,"bolt_stacks":[{"length_source":"computed_from_grip","gusset_face":"+z","grip_mm":19.524999999999999,"member_element_id":1249636,"bolt_length_mm":44.450000000000003}],"node":{"chord_element_id":1249510,"element_ids":[1249510,1249630,1249631,1249636],"chord_direction_reversed":true},"errors_count":0,"name":"PRUEBA probar_conexiones","validation_token":"3a2f14df8e762567a9fc10eb54f6e038ca2cf5f6695c93e275687a1bece85953","spec":{"uncertain_fields":[],"connection_type":"gusset_node","spec_version":"1.0","node":{"element_ids":[1249510,1249630,1249631,1249636]},"chord":{"continuous":true,"element_id":1249510,"profile":"HSS3X3X1/4"},"source":{"drawing":"Detalle D","scale":"1/10","template_id":"d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f"},"dimension_chains":[{"expected_total_mm":565.0,"label":"borde superior","values_mm":[75.0,420.0,70.0]},{"expected_total_mm":565.0,"label":"base","values_mm":[125.0,90.0,350.0]},{"expected_total_mm":530.0,"label":"lado derecho","values_mm":[70.0,250.0,210.0]},{"expected_total_mm":530.0,"label":"lado izquierdo","values_mm":[70.0,325.0,135.0]}],"members":[{"element_id":1249630,"attachment":{"type":"welded_slot","weld":{"type":"fillet","size_mm":5.0,"all_around":true},"slot_length_mm":150.0},"end_setback_mm":180.0,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","expected_angle_deg":43.100000000000001},{"element_id":1249631,"attachment":{"type":"welded_slot","weld":{"type":"fillet","size_mm":5.0,"all_around":true},"s ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, N1 ready con token  [FALLO]  HTTP 200, ok=True, plan_id=ff16712f-5382-4bd6-8e02-7cb7351ce03d nudos=8 resumen={'untyped': 8} N1=untyped None
Cuerpo:
{"data": {"marks": {"marker_element_ids": [], "element_count": 0}, "is_marked": false, "document": "HANGAR_PRUEBA_sondeo", "ready_count": 0, "nodes": [{"template_name": null, "errors": [], "has_spec_override": false, "element_ids": [1249630], "max_deviation_deg": null, "members": [], "work_point_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "signature": "1 barra(s) sin marco", "is_manual": false, "status_detail": "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.", "replaces_existing": false, "through_element_ids": [], "validation_token": null, "warnings": [], "is_mirrored": false, "color_name": null, "warnings_count": 0, "is_marked": false, "errors_count": 0, "orientation": null, "match": null, "is_valid": false, "chord_element_id": 0, "status": "untyped", "color_rgb": null, "attempts": [], "member_element_ids": [1249630], "template_id": null, "existing_connection_id": null, "marker_element_id": null, "name": "N1", "chord_type_name": null, "spec": null, "chord_continuous": false}, {"template_name": null, "errors": [], "has_spec_override": false, "element_ids": [1249636], "max_deviation_deg": null, "members": [], "work_point_mm": [-14455.299999999999, -17195.700000000001, 14894.700000000001], "signature": "1 barra(s) sin marco", "is_manual": false, "status_detail": "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.", "replaces_existing": false, "through_element_ids": [], "validation_token": null, "warnings": [], "is_m ...
======================================================================
23. POST /conn/batch/plan/get/ node N1 -> el nudo con su token  [FALLO]  HTTP 200, ok=True, N1 untyped token=...
Cuerpo:
{"data": {"node": {"template_name": null, "errors": [], "has_spec_override": false, "element_ids": [1249630], "max_deviation_deg": null, "members": [], "work_point_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "signature": "1 barra(s) sin marco", "is_manual": false, "status_detail": "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.", "replaces_existing": false, "through_element_ids": [], "validation_token": null, "warnings": [], "is_mirrored": false, "color_name": null, "warnings_count": 0, "is_marked": false, "errors_count": 0, "orientation": null, "match": null, "is_valid": false, "chord_element_id": 0, "status": "untyped", "color_rgb": null, "attempts": [], "member_element_ids": [1249630], "template_id": null, "existing_connection_id": null, "marker_element_id": null, "name": "N1", "chord_type_name": null, "spec": null, "chord_continuous": false}, "plan_id": "ff16712f-5382-4bd6-8e02-7cb7351ce03d"}, "warnings": [], "errors": [], "meta": {"operation": "batch_plan_get", "duration_ms": 0, "addin_version": "0.8.0"}, "ok": true}
======================================================================
24. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado ff16712f-5382-4bd6-8e02-7cb7351ce03d
Cuerpo:
{"data": {"discarded_plan_id": "ff16712f-5382-4bd6-8e02-7cb7351ce03d", "remaining_plans": 0, "removed_marks": 0, "remaining_markers": 0}, "warnings": [], "errors": [], "meta": {"operation": "batch_plan_discard", "duration_ms": 16, "addin_version": "0.8.0"}, "ok": true}
======================================================================
25. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"data":null,"warnings":[],"errors":[{"code":"TEMPLATE_NOT_FOUND","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","path":"template_id","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo."}],"meta":{"operation":"catalog_apply","duration_ms":1,"addin_version":"0.8.0"},"ok":false}
======================================================================
26. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","deleted_template_id":"d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f.json"},"warnings":[],"errors":[],"meta":{"operation":"catalog_delete","duration_ms":3,"addin_version":"0.8.0"},"ok":true}
======================================================================
27. tools/list por el puente trae las 21 herramientas conn_*  [OK]  HTTP 200, herramientas=87 conn_*=21
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard
======================================================================
28. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.8.0
Cuerpo:
{
  "data": {
    "spec_version": "1.0",
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
    ],
    "revit": {
      "version_name": "Autodesk Revit 2027",
      "sub_version_number": "2027.2",
      "language": "English_USA",
      "version_number": "2027",
      "version_build": "27.2.0.39"
    },
    "dotnet": {
      "framework": ".NET 10.0.12",
      "load_context": "Default",
      "assembly_location": "C:\\Users\\Andy Bayona Ant�n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
    },
    "has_uidocument": true,
    "document": {
      "path": "D:\\IG INGENIER�A\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_workshared": false,
      "is_modifiable": false,
      "is_family": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "is_read_only": false
    },
    "backend": "advancesteel",
    "addin_version": "0.8.0"
  },
  "warnings": [],
  "errors": [],
  "meta": {
    "operation": "ping",
    "duration_ms": 2,
    "addin_version": "0.8.0"
  },
  "ok": true
}
======================================================================
Resultado: 26/28 pruebas correctas

```

## 8-7 log del dia

```text

{"ts":"2026-10-04T18:26:45.3220122-05:00","record":{"event":"startup","addin_version":"0.8.0","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-04T18:40:20.4925307-05:00","record":{"event":"batch_plan","plan_id":"01fd5226-37b8-4e8e-a82a-defa5895aa6
0","replan":false,"element_ids":53,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":93,"no_mat
ch":4},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"a
dd_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false,\"IsEmpty\":true}"}}
{"ts":"2026-10-04T18:40:20.5175311-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"tem
plate_ids\": [\"6abcf116-9b97-485f-b50d-2851ca0018cc\"], \"include_specs\": 
false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":289}}
{"ts":"2026-10-04T18:44:41.6131215-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"node\": \"N9\", \"plan_id\": 
\"35920\"}","ok":false,"error_codes":["PLAN_NOT_FOUND"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-04T18:44:41.7294966-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"pla
n_id\": \"35920\", \"include_specs\": false, \"overrides\": {\"exclude\": 
[\"N9\"]}}","ok":false,"error_codes":["PLAN_NOT_FOUND"],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-10-04T18:44:41.8094932-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"pla
n_id\": \"35920\", \"include_specs\": false, \"overrides\": {\"include\": 
[\"N9\"]}}","ok":false,"error_codes":["PLAN_NOT_FOUND"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-04T18:45:50.3458657-05:00","record":{"event":"batch_plan","plan_id":"0e900ec3-a06c-4656-a547-00e9a5b52b1
f","replan":false,"element_ids":53,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":93,"no_mat
ch":4},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"a
dd_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false,\"IsEmpty\":true}"}}
{"ts":"2026-10-04T18:47:08.0554089-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"ShowInRevit","node":"N10","discarded":false}}
{"ts":"2026-10-04T18:47:31.8442627-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"ShowInRevit","node":"N10","discarded":false}}
{"ts":"2026-10-04T18:48:49.4101285-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickChord","node":"N12","discarded":false}}
{"ts":"2026-10-04T18:49:02.6570914-05:00","record":{"event":"batch_plan","plan_id":"0e900ec3-a06c-4656-a547-00e9a5b52b1
f","replan":true,"element_ids":53,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":92,"no_matc
h":5},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N12\":1249631},\"template\":{},\"remove_m
ember\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false,\"IsEmpty\":false}"}}
{"ts":"2026-10-04T18:49:17.5367384-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickNewNode","node":null,"discarded":false}}
{"ts":"2026-10-04T18:49:51.4187183-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickNewNode","node":null,"discarded":false}}
{"ts":"2026-10-04T18:50:18.8421444-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"ShowInRevit","node":"N93","discarded":false}}
{"ts":"2026-10-04T18:50:30.1236461-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickChord","node":"N13","discarded":false}}
{"ts":"2026-10-04T18:50:33.0718692-05:00","record":{"event":"batch_plan","plan_id":"0e900ec3-a06c-4656-a547-00e9a5b52b1
f","replan":true,"element_ids":53,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":91,"no_matc
h":6},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N12\":1249631,\"N13\":1249515},\"template
\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false,\"IsEmpty
\":false}"}}
{"ts":"2026-10-04T18:50:38.4833519-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickNewNode","node":null,"discarded":false}}
{"ts":"2026-10-04T18:50:45.5212788-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"PickNewNode","node":null,"discarded":false}}
{"ts":"2026-10-04T18:50:47.7970632-05:00","record":{"event":"batch_plan","plan_id":"0e900ec3-a06c-4656-a547-00e9a5b52b1
f","replan":true,"element_ids":53,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":92,"no_matc
h":6},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{\"N98\":[1250294,1250297]},\"chord\":{\"N12\":1249631,\"
N13\":1249515},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false,\"IsEmpty\":false}"}}
{"ts":"2026-10-04T18:50:49.7391250-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"ShowInRevit","node":"N98","discarded":false}}
{"ts":"2026-10-04T18:51:09.9826963-05:00","record":{"event":"batch_plan_discard","plan_id":"0e900ec3-a06c-4656-a547-00e
9a5b52b1f","removed_marks":16}}
{"ts":"2026-10-04T18:51:10.0083769-05:00","record":{"event":"ribbon_batch_window","plan_id":"0e900ec3-a06c-4656-a547-00
e9a5b52b1f","action":"None","node":null,"discarded":true}}
{"ts":"2026-10-04T18:52:10.3961747-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{}
","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-04T18:52:10.7786249-05:00","record":{"event":"batch_plan_discard_all","markers":0}}
{"ts":"2026-10-04T18:52:10.7920041-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"all\": true}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":72}}
{"ts":"2026-10-04T18:52:34.2077465-05:00","record":{"event":"batch_plan","plan_id":"ff16712f-5382-4bd6-8e02-7cb7351ce03
d","replan":false,"element_ids":4,"templates":["d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f"],"summary":{"untyped":8},"marked"
:false,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{}
,\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false,\"IsEmpty\":true}"}}
{"ts":"2026-10-04T18:52:34.2085374-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"ele
ment_ids\": [1249510, 1249630, 1249631, 1249636], \"template_ids\": [\"d3c3cf46-f5c9-45cc-8bf3-5bb73e72171f\"], 
\"mark\": false}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":10}}
{"ts":"2026-10-04T18:52:34.2286129-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"node\": \"N1\", \"plan_id\": 
\"ff16712f-5382-4bd6-8e02-7cb7351ce03d\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-04T18:52:34.2767847-05:00","record":{"event":"batch_plan_discard","plan_id":"ff16712f-5382-4bd6-8e02-7cb
7351ce03d","removed_marks":0}}
{"ts":"2026-10-04T18:52:34.2843969-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"plan_id\": 
\"ff16712f-5382-4bd6-8e02-7cb7351ce03d\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":16}}



```

## 8-8 git status antes del commit

```text
?? docs/fases/capturas/fase8-02-cinta.png
?? docs/fases/capturas/fase8-03-marcas.png
?? docs/fases/capturas/fase8-04-marcador-espejo.png
?? docs/fases/capturas/fase8-05-ventana-plan.png
?? docs/fases/resultados-fase-8.md

```
