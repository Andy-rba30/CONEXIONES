# Resultados de la ronda 13b

Fecha: 2026-10-31T12:07:52


## 13b-1 git

```text
0e65b12 Cierre de la Fase 13 (add-in 0.13.1): la 0.13.0 contrastada en Revit, los tramos sobre la línea exacta y el sondeo 30 v2
?? docs/fases/resultados-fase-13b.md

```

## 13b-1 build

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.52

```

## 13b-1 test

```text
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   314, Omitido:     0, Total:   314, Duración: 744 ms - MotorConexiones.Tests.dll (net10.0)

```

## 13b-1 deploy

```text
== dotnet build D:\Proyectos C#\CONEXIONES\MotorConexiones.sln -c Release ==
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.13
== MotorConexiones 0.13.1.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 13b-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (27 rutas @api.route)
- copiado tools\conn_tools.py (25 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 13b-2 ping

```text
== conn/ping -> HTTP 200 en 351 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "ping",
                 "duration_ms":  17
             },
    "data":  {
                 "has_uidocument":  true,
                 "spec_version":  "1.0",
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "version_build":  "27.2.0.39",
                               "sub_version_number":  "2027.2",
                               "language":  "English_USA",
                               "version_number":  "2027"
                           },
                 "document":  {
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "is_read_only":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_workshared":  false
                              },
                 "backend":  "advancesteel",
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
                                    "split",
                                    "types",
                                    "unsplit",
                                    "update",
                                    "validate"
                                ],
                 "dotnet":  {
                                "load_context":  "Default",
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
                            },
                 "addin_version":  "0.13.1"
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-2 list

```text
== conn/list -> HTTP 200 en 55 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "list",
                 "duration_ms":  7
             },
    "data":  {
                 "batch_id":  null,
                 "total_count":  0,
                 "batches":  {

                             },
                 "connections_count":  0,
                 "connections":  [

                                 ]
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-2 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 23 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "unsplit",
                 "duration_ms":  5
             },
    "data":  {
                 "splits":  [

                            ],
                 "splits_count":  0
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-2 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 186 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 8 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 4 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 13b-2 sondeo 13

```text
== 13-limpiar-fase1.py -> HTTP 200 en 49 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 13b-3 sondeo 30 v2

```text
== 30-partir-barra.py -> HTTP 200 en 39595 ms ==
Sondeo 30: partir la barra en dos (Fase 13). Documento: HANGAR_PRUEBA_sondeo. Cancelar al final: True
1) miembros de la API y parametros ANTES de la diagonal 1249631 (cordon 1245533)
   ElementTransformUtils.CopyElement: existe
   ElementTransformUtils.CopyElements: existe
   (clase no disponible con ese nombre)
   StructuralFramingUtils.DisallowJoinAtEnd: existe
   StructuralFramingUtils.AllowJoinAtEnd: existe
   StructuralFramingUtils.IsJoinAllowedAtEnd: existe
   Document.Create.NewFamilyInstance: existe
   curva: inicio (-11856.5, -17195.8, 17437.3) fin (-9354.7, -17195.8, 19884.9) largo 3500.0 mm
   diagonal 1249631 ANTES: dX 2501.801006 | dY 0.005871 | dZ 2447.650246 mm | en planta 0.000134 grados respecto a X | 0.000096 grados fuera del plano vertical XZ
   diagonal 1249631 ANTES: inicio (-11856.476246, -17195.840724, 17437.279264) fin (-9354.675240, -17195.834852, 19884.929510) mm
   cordon 1245533: dX -10475.000000 | dY 0.024172 | dZ 0.000000 mm | en planta 0.000132 grados respecto a X | 0.000132 grados fuera del plano vertical XZ
   cordon 1245533: inicio (-4180.179050, -17195.756917, 19933.000000) fin (-14655.179050, -17195.732745, 19933.000000) mm
   STRUCTURAL_BEND_DIR_ANGLE = -3.141593
   Y_JUSTIFICATION = 2
   Z_JUSTIFICATION = 1
   Y_OFFSET_VALUE = 0.000000
   Z_OFFSET_VALUE = 0.000000
   START_EXTENSION = 0.000000
   END_EXTENSION = 0.227033
   STRUCTURAL_MATERIAL_PARAM = 1251831
   INSTANCE_REFERENCE_LEVEL_PARAM = 9946
   ELEM_TYPE_PARAM = 1253518
2) punto de corte (SplitPoint.Compute del Core) y su proyeccion sobre la linea real de la diagonal (add-in 0.13.1)
   perfil del cordon: b=203.2 Ht=304.8 giro=90.0 grados
   theta = 44.4 grados | cara a 217.9 mm | primera plancha a 547.9 mm | corte a 557.4 mm del corte de los ejes
   corte de los ejes (-9305.5, -17195.8, 19933.0) | punto de corte (-9704.0, -17195.8, 19543.2) | extremo del lado del cordon: fin
   el punto del marco estaba a 0.000000 mm de la linea real de la diagonal; se usa el proyectado (-9704.0, -17195.8, 19543.2)
3) (a) CopyElement en el sitio + LocationCurve.Curve en los dos elementos
   CopyElement -> 1322482 (FamilyInstance)
   original (tramo corto): (-9704.0, -17195.8, 19543.2) -> (-9354.7, -17195.8, 19884.9) largo 488.7 mm
   original (tramo corto): dX 349.330219 | dY 0.000820 | dZ 341.769067 mm | en planta 0.000134 grados respecto a X | 0.000096 grados fuera del plano vertical XZ
   original (tramo corto): inicio (-9704.005458, -17195.835672, 19543.160443) fin (-9354.675240, -17195.834852, 19884.929510) mm
   original (tramo corto): extension en el corte puesta a 0
   original (tramo corto): DisallowJoinAtEnd(0) ok; IsJoinAllowedAtEnd = False
   copia (tramo largo): (-11856.5, -17195.8, 17437.3) -> (-9704.0, -17195.8, 19543.2) largo 3011.3 mm
   copia (tramo largo): dX 2152.470787 | dY 0.005051 | dZ 2105.881179 mm | en planta 0.000134 grados respecto a X | 0.000096 grados fuera del plano vertical XZ
   copia (tramo largo): inicio (-11856.476246, -17195.840724, 17437.279264) fin (-9704.005458, -17195.835672, 19543.160443) mm
   copia (tramo largo): extension en el corte puesta a 0
   copia (tramo largo): DisallowJoinAtEnd(1) ok; IsJoinAllowedAtEnd = False
   parametros de la copia distintos del original de antes (3): End Level Offset [53.239270 -> 52.117980], IfcGUID [2yvC09P_58MQYC5jkytRlK -> 0PJyCQFHr29QS4VWe8CeTQ], System Length [11.482940 -> 9.879560]
   STRUCTURAL_BEND_DIR_ANGLE: original -3.141593 | copia -3.141593
   Y_JUSTIFICATION: original 2 | copia 2
   Z_JUSTIFICATION: original 1 | copia 1
   Y_OFFSET_VALUE: original 0.000000 | copia 0.000000
   Z_OFFSET_VALUE: original 0.000000 | copia 0.000000
   START_EXTENSION: original 0.000000 | copia 0.000000
   END_EXTENSION: original 0.227033 | copia 0.000000
   STRUCTURAL_MATERIAL_PARAM: original 1251831 | copia 1251831
   INSTANCE_REFERENCE_LEVEL_PARAM: original 9946 | copia 9946
   ELEM_TYPE_PARAM: original 1253518 | copia 1253518
   (a) commit ok
4) (b) no hace falta: (a) funciono (si en el paso 3 hay parametros distintos, el add-in los copia a mano: SPLIT_PARAMETERS_APPROXIMATED)
5) node_info de los dos tramos y del nudo en K (por el puente, si esta)
   node_info [1249631, 1322482L]: ok=False kind=None eccentricity_mm=None chord=None barras=[]
   errores: [{'message': u'Se requieren al menos 2 miembros de armaz\xf3n estructural para inspeccionar el nudo (recibidos: 1).', 'path': 'element_ids', 'hint': "Selecciona los miembros del nudo en Revit o pasa sus IDs en 'element_ids'.", 'code': 'INVALID_REQUEST'}]
   node_info [1245533, 1249631, 1249632]: ok=True kind=node eccentricity_mm=54.9 chord=1245533 barras=[(1245533, 10475, None), (1249631, 488.69999999999999, 484.80000000000001), (1249632, 3546, 483.19999999999999)]
6) deshacer: borrar la copia y devolver la curva al original
   original tras deshacer: (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9) largo 3500.0 mm (antes 3500.0); diferencia 0.00 mm
   STRUCTURAL_BEND_DIR_ANGLE: ahora -3.141593 | antes -3.141593
   Y_JUSTIFICATION: ahora 2 | antes 2
   Z_JUSTIFICATION: ahora 1 | antes 1
   Y_OFFSET_VALUE: ahora 0.000000 | antes 0.000000
   Z_OFFSET_VALUE: ahora 0.000000 | antes 0.000000
   START_EXTENSION: ahora 0.000000 | antes 0.000000
   END_EXTENSION: ahora 0.227033 | antes 0.227033
7) elementos nuevos con el grupo abierto: []
   grupo de transacciones REVERTIDO (nada queda en el modelo)
   elementos nuevos tras cerrar: []
Fin del sondeo 30.


```

## 13b-3 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 80 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "unsplit",
                 "duration_ms":  4
             },
    "data":  {
                 "splits":  [

                            ],
                 "splits_count":  0
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-4 split dry_run

```text
== conn/split -> HTTP 200 en 464 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "split",
                 "duration_ms":  21
             },
    "data":  {
                 "dry_run":  true,
                 "description":  "La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cordón 1245533 a 45,0°: la cara del cordón queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo),",
                 "preview":  {
                                 "theta_deg":  45,
                                 "plate_thickness_mm":  9.5250000000000004,
                                 "short_segment_mm":  521.60000000000002,
                                 "chord_element_id":  1245533,
                                 "long_segment_mm":  3024.4000000000001,
                                 "first_plate_distance_mm":  473.69999999999999,
                                 "offset_from_chord_face_mm":  330,
                                 "chord_type":  "HSS12X8X1/2",
                                 "cut_point_mm":  [
                                                      -9250.7000000000007,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                 "face_distance_mm":  143.69999999999999,
                                 "element_id":  1249632,
                                 "split_distance_mm":  483.19999999999999,
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "member_type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                 "gap_mm":  0
                             },
                 "already_split":  null
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-4 split

```text
== conn/split -> HTTP 200 en 248 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "split",
                 "duration_ms":  220
             },
    "data":  {
                 "split":  {
                               "split_id":  "b67f7557-8564-4cbc-8906-b34b31fbe6f0",
                               "batch_id":  null,
                               "new_element_id":  1322488,
                               "chord_element_id":  1245533,
                               "original_start_mm":  [
                                                         -9277.7999999999993,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                               "connection_id":  null,
                               "offset_from_chord_face_mm":  330,
                               "original_end_mm":  [
                                                       -6770.3999999999996,
                                                       -17195.799999999999,
                                                       17452.700000000001
                                                   ],
                               "created_utc":  "2026-10-31T17:11:06.6237030Z",
                               "approximated_parameters":  [

                                                           ],
                               "mechanism":  "copy_element",
                               "element_id":  1249632,
                               "split_point_mm":  [
                                                      -8909,
                                                      -17195.799999999999,
                                                      19591.299999999999
                                                  ],
                               "split_distance_mm":  483.19999999999999
                           },
                 "description":  "La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cordón 1245533 a 45,0°: la cara del cordón queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo), Hecho: tramo nuevo 1322488.",
                 "next":  "conn_get_node_info con [1249632, 1322488] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.",
                 "preview":  {
                                 "theta_deg":  45,
                                 "plate_thickness_mm":  9.5250000000000004,
                                 "short_segment_mm":  521.60000000000002,
                                 "chord_element_id":  1245533,
                                 "long_segment_mm":  3024.4000000000001,
                                 "first_plate_distance_mm":  473.69999999999999,
                                 "offset_from_chord_face_mm":  330,
                                 "chord_type":  "HSS12X8X1/2",
                                 "cut_point_mm":  [
                                                      -9250.7000000000007,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                 "face_distance_mm":  143.69999999999999,
                                 "element_id":  1249632,
                                 "split_distance_mm":  483.19999999999999,
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "member_type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                 "gap_mm":  0
                             }
             },
    "warnings":  [
                     {
                         "message":  "Advertencia de Revit: Beam or Brace is slightly off axis and may cause inaccuracies. [elementos: 1322488]",
                         "path":  null,
                         "hint":  null,
                         "code":  "REVIT_WARNING"
                     }
                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-4 log revit_failure del corte

```text

{"ts":"2026-10-31T11:05:32.0266673-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322767","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}
{"ts":"2026-10-31T11:07:15.3911477-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322773","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}
{"ts":"2026-10-31T11:07:37.7662750-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322779","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}
{"ts":"2026-10-31T12:11:06.7924228-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322488","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}



```

## 13b-4 registro del corte

```text

```

## 13b-4 node_info tramos

```text
== conn/node_info -> HTTP 500 en 36 ms ==
{
    "exception":  {
                      "message":  "Server error (ValueError): Expecting object: line 1 column 102 (char 101)\n  File \"C:\\Program Files\\pyRevit-Master\\pyrevitlib\\pyrevit\\routes\\server\\server.py\", line 161, in _handle_route\n    request = self._prepare_request(route, path, method, query_string)\n  File \"C:\\Program Files\\pyRevit-Master\\pyrevitlib\\pyrevit\\routes\\server\\server.py\", line 89, in _prepare_request\n    data = json.loads(data)\n  File \"json\\__init__.py\", line 339, in loads\n  File \"json\\decoder.py\", line 364, in decode\n  File \"json\\decoder.py\", line 380, in raw_decode\n  File \"json\\scanner.py\", line 37, in _scan_once\n  File \"json\\decoder.py\", line 194, in JSONObject\n  File \"json\\scanner.py\", line 40, in _scan_once\n  File \"json\\decoder.py\", line 251, in JSONArray\n\n",
                      "source":  "pyrevit.routes"
                  }
}

```

## 13b-4 unsplit

```text
== conn/unsplit -> HTTP 200 en 27 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "unsplit",
                 "duration_ms":  4
             },
    "data":  {
                 "splits":  [
                                {
                                    "split_id":  "b67f7557-8564-4cbc-8906-b34b31fbe6f0",
                                    "batch_id":  null,
                                    "new_element_id":  1322488,
                                    "chord_element_id":  1245533,
                                    "original_start_mm":  [
                                                              -9277.7999999999993,
                                                              -17195.799999999999,
                                                              19960.099999999999
                                                          ],
                                    "connection_id":  null,
                                    "offset_from_chord_face_mm":  330,
                                    "original_end_mm":  [
                                                            -6770.3999999999996,
                                                            -17195.799999999999,
                                                            17452.700000000001
                                                        ],
                                    "created_utc":  "2026-10-31T17:11:06.6237030Z",
                                    "approximated_parameters":  [

                                                                ],
                                    "mechanism":  "copy_element",
                                    "element_id":  1249632,
                                    "split_point_mm":  [
                                                           -8909,
                                                           -17195.799999999999,
                                                           19591.299999999999
                                                       ],
                                    "split_distance_mm":  483.19999999999999
                                }
                            ],
                 "splits_count":  1
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-4 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 54 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "unsplit",
                 "duration_ms":  4
             },
    "data":  {
                 "splits":  [
                                {
                                    "split_id":  "b67f7557-8564-4cbc-8906-b34b31fbe6f0",
                                    "batch_id":  null,
                                    "new_element_id":  1322488,
                                    "chord_element_id":  1245533,
                                    "original_start_mm":  [
                                                              -9277.7999999999993,
                                                              -17195.799999999999,
                                                              19960.099999999999
                                                          ],
                                    "connection_id":  null,
                                    "offset_from_chord_face_mm":  330,
                                    "original_end_mm":  [
                                                            -6770.3999999999996,
                                                            -17195.799999999999,
                                                            17452.700000000001
                                                        ],
                                    "created_utc":  "2026-10-31T17:11:06.6237030Z",
                                    "approximated_parameters":  [

                                                                ],
                                    "mechanism":  "copy_element",
                                    "element_id":  1249632,
                                    "split_point_mm":  [
                                                           -8909,
                                                           -17195.799999999999,
                                                           19591.299999999999
                                                       ],
                                    "split_distance_mm":  483.19999999999999
                                }
                            ],
                 "splits_count":  1
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-4 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 128 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
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

## 13b-5 list

```text
== conn/list -> HTTP 200 en 425 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "list",
                 "duration_ms":  21
             },
    "data":  {
                 "batch_id":  null,
                 "total_count":  2,
                 "batches":  {

                             },
                 "connections_count":  2,
                 "connections":  [
                                     {
                                         "spec_version":  "1.0",
                                         "batch_id":  null,
                                         "connection_id":  "ff08f5d6-2031-4dd3-9619-57de3a506e23",
                                         "template_id":  null,
                                         "backend":  "advancesteel",
                                         "created_elements_count":  8,
                                         "created_utc":  "2026-10-31T17:15:10.4603511Z",
                                         "connection_type":  "end_plate"
                                     },
                                     {
                                         "spec_version":  "1.0",
                                         "batch_id":  null,
                                         "connection_id":  "0f03b68e-7b47-4bd6-911e-111bc60d18f0",
                                         "template_id":  null,
                                         "backend":  "advancesteel",
                                         "created_elements_count":  1,
                                         "created_utc":  "2026-10-31T17:15:10.6611240Z",
                                         "connection_type":  "direct_weld"
                                     }
                                 ]
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-5 delete empalmes B

```text
== conn/delete -> HTTP 200 en 155 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "delete",
                 "duration_ms":  142
             },
    "data":  {
                 "deleted_connection_id":  "ff08f5d6-2031-4dd3-9619-57de3a506e23",
                 "deleted_elements_count":  8,
                 "restored_members_count":  2
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-5 delete nudo K (deshace los cortes)

```text
== conn/delete -> HTTP 200 en 110 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "delete",
                 "duration_ms":  101
             },
    "data":  {
                 "deleted_connection_id":  "0f03b68e-7b47-4bd6-911e-111bc60d18f0",
                 "deleted_elements_count":  1,
                 "restored_members_count":  1
             },
    "warnings":  [
                     {
                         "message":  "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Los elementos se borran directamente (Document.Delete) y las barras se restauran igual.",
                         "path":  null,
                         "hint":  null,
                         "code":  "REVIT_WARNING"
                     }
                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-5 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 207 ms ==
{
    "meta":  {
                 "addin_version":  "0.13.1",
                 "operation":  "unsplit",
                 "duration_ms":  4
             },
    "data":  {
                 "splits":  [

                            ],
                 "splits_count":  0
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true
}

```

## 13b-5 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 118 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 7 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 13b-6 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         37592



```

## 13b-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.13.1 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"ping","duration_ms":3},"data":{"has_uidocument":true,"spec_version":"1.0","revit":{"version_name":"Autodesk Revit 2027","version_build":"27.2.0.39","sub_version_number":"2027.2","language":"English_USA","version_number":"2027"},"document":{"is_family":false,"is_modifiable":false,"is_read_only":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","title":"HANGAR_PRUEBA_sondeo","is_workshared":false},"backend":"advancesteel","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","split","types","unsplit","update","validate"],"dotnet":{"load_context":"Default","framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"},"addin_version":"0.13.1"},"warnings":[],"errors":[],"ok":true}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 36647 caracteres
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"guide","duration_ms":7},"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligat ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"types","duration_ms":1},"data":{"connection_types":[{"description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n.","type_name":"direct_weld"},{"description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).","type_name":"end_plate"},{"description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","type_name":"gusset_node"}]},"warnings":[],"errors":[],"ok":true}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"schema","duration_ms":1},"data":{"example":{"uncertain_fields":[],"spec_version":"1.0","gusset":{"thickness_mm":9.5250000000000004,"width_mm":565.0,"thickness_label":"3/8\"","height_mm":530.0,"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"chord_interface":"through_slot"},"chord":{"profile":"HSS3X3X1/4","element_id":1249510,"continuous":true},"source":{"scale":"1/10","drawing":"Detalle D"},"connection_type":"gusset_node","members":[{"expected_angle_deg":45.0,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"role":"diagonal","attachment":{"slot_length_mm":150.0,"type":"welded_slot","weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"element_id":1249630}],"dimension_chains":[{"label":"borde superior","values_mm":[75.0,420.0,70.0],"expected_total_mm":565.0}],"node":{"element_ids":[1249510,1249630,1249631,1249636]}},"json_schema":{"type":"object","additionalProperties":false,"$schema":"http://json-schema.org/draft-07/schema#","properties":{"uncertain_fields":{"items":{"additionalProperties":false,"type":"object","required":["path","reason"],"properties":{"user_confirmed_value":{},"reason":{"type":"string"},"path":{"type":"string"}}},"type":"array"},"spec_version":{"type":"string","enum":["1.0"]},"gusset":{"additionalProperties":false,"type":"o ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"schema","duration_ms":0},"data":null,"warnings":[],"errors":[{"message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","path":"type","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node","code":"UNKNOWN_OPERATION"}],"ok":false}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "find_profile", "duration_ms": 5}, "data": {"query": "HSS2-1/2X2-1/2X3/16", "total_profiles_in_model": 29, "suggestions": [], "matched_count": 1, "matches": [{"family_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "type_name": "HSS2-1-2X2-1-2X3-16 64x64"}]}, "warnings": [], "errors": [], "ok": true}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "node_info", "duration_ms": 16}, "data": {"existing_connections": [], "y_axis": [0, 0, 1], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "chord_element_id": 1249510, "axis_distance_mm": 0.080000000000000002, "origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "eccentricity_mm": 3.3999999999999999, "chord_direction_reversed": true, "members": [{"end_gap_mm": 0, "axis_offset_mm": 0, "section_rotation_deg": 180, "type": "HSS3X3X1/4", "height_mm": 76.200000000000003, "length_mm": 9960.2999999999993, "structural_type": "Beam", "element_id": 1249510, "node_end": 1, "width_mm": 76.200000000000003, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "side": "chord", "angle_in_plane_deg": 0, "split_distance_mm": null, "split_offset_from_chord_face_mm": null, "slope_deg": 0, "is_chord": true, "angle_to_chord_deg": 0, "split_point_mm": null, "reaches_node": true, "wall_mm": 5.9199999999999999, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "family": "HSS-Hollow Structural Section", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423]}, {"end_gap_mm": 84.900000000000006, "axis_offset_mm": 1.7000000000000000, "section_rotation_deg": -180, "type": "HSS2-1-2X2-1-2X3-16 64x64", "height_mm": 63.5, "length_mm": 3568, "structural_type": "Beam", "element_ ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":22},"data":{"bolt_stacks":[{"member_element_id":1249636,"gusset_face":"+z","length_source":"computed_from_grip","grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003}],"errors_count":0,"is_valid":true,"calculated_values":{"outline_mode":null,"frame_x":[1,0,0],"outline_width_mm":null,"outline_points_mm":null,"outline_height_mm":null,"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"eccentricity_mm":3.3999999999999999,"chord_direction_reversed":true,"frame_y":[0,0,1],"outline_vertices":null,"kind":"node"},"warnings_count":1,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1"},"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[],"ok":true}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":9},"data":null,"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[{"message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","path":"dimension_chains[0].values_mm","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","code":"DIMENSION_CHAIN_MISMATCH"}],"ok":false}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":8},"data":null,"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[{"message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","path":"uncertain_fields[0].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","code":"UNRESOLVED_UNCERTAINTY"},{"message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","path":"uncertain_fields[1].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","code":"UNRESOLVED_UNCERTAINTY"}],"ok":false}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"working_point_mm": [-11869.4, -17195.8, 17423], "dry_run": true, "chord_element_id": 1249510, "weld_lines": 6, "gusset_plates": 1, "backend": "advancesteel", "members_modified": 3, "knife_plates": 1, "connection_type": "gusset_node", "first_member_element_id": 1249630, "bolts": 4}
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "preview", "duration_ms": 6}, "data": {"calculated_values": {"outline_mode": null, "frame_x": [1, 0, 0], "outline_width_mm": null, "outline_points_mm": null, "outline_height_mm": null, "frame_z": [0, -1, 0], "axis_distance_mm": 0.080000000000000002, "origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "eccentricity_mm": 3.3999999999999999, "chord_direction_reversed": true, "frame_y": [0, 0, 1], "outline_vertices": null, "kind": "node"}, "summary": {"working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "dry_run": true, "chord_element_id": 1249510, "weld_lines": 6, "gusset_plates": 1, "backend": "advancesteel", "members_modified": 3, "knife_plates": 1, "connection_type": "gusset_node", "first_member_element_id": 1249630, "bolts": 4}, "members_to_modify": [{"new_extension_mm": -95.099999999999994, "current_end_distance_mm": 84.900000000000006, "end": "end", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "element_id": 1249630}, {"new_extension_mm": -40.799999999999997, "current_end_distance_mm": 19.199999999999999, "end": "start", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "setback_mm": 60, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "element_id": 1249631}, {"new_extension_mm": -211.40000000000001, "curr ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"create","duration_ms":0},"data":null,"warnings":[],"errors":[{"message":"validation_token es obligatorio para crear una conexi\u00f3n.","path":"validation_token","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","code":"VALIDATION_TOKEN_INVALID"}],"ok":false}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "list", "duration_ms": 3}, "data": {"batch_id": null, "total_count": 0, "batches": {}, "connections_count": 0, "connections": []}, "warnings": [], "errors": [], "ok": true}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"get","duration_ms":4},"data":null,"warnings":[],"errors":[{"message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","code":"ELEMENT_NOT_FOUND"}],"ok":false}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"delete","duration_ms":3},"data":null,"warnings":[],"errors":[{"message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id","hint":"Verifica los IDs disponibles con conn_list.","code":"ELEMENT_NOT_FOUND"}],"ok":false}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"no_existe","duration_ms":0},"data":null,"warnings":[],"errors":[{"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","path":null,"hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, split, types, unsplit, update, validate.","code":"UNKNOWN_OPERATION"}],"ok":false}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_list","duration_ms":2},"data":{"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates_count":2,"templates":[{"origin_document":"HANGAR_PRUEBA_sondeo","description":null,"template_id":"91adac27-d196-4c98-88db-6579aa991be4","pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","created_utc":"2026-10-21T16:49:07.9104459Z","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","name":"Nudo K cordon superior","members_count":2,"tags":[],"connection_type":"direct_weld","chord_profile":"HSS12X8X1/2"},{"origin_document":"HANGAR_PRUEBA_sondeo","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","origin_drawing":"Detalle D","created_utc":"2026-10-04T22:01:50.5585233Z","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","name":"Nudo tipico Detalle D","members_count":3,"tags":["hangar","cercha","HSS"],"connection_type":"gusset_node","chord_profile":"HSS3X3X1/4"}]}, ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=648b91fd-f886-4e56-b242-e0300121b9ea barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\648b91fd-f886-4e56-b242-e0300121b9ea.json
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_save","duration_ms":28},"data":{"origin":{"document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636],"connection_id":null,"drawing":"Detalle D"},"member_pattern":[{"side":"+Y","slot":0,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":136.91999999999999},{"side":"+Y","slot":1,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":44.369999999999997},{"side":"-Y","slot":2,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":-135.63000000000000}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"template_id":"648b91fd-f886-4e56-b242-e0300121b9ea","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\648b91fd-f886-4e56-b242-e0300121b9ea.json","shared_file":null,"name":"PRUEBA probar_conexiones","members_count":3,"chord_profile":"HSS3X3X1/4"},"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[],"ok":true}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_get","duration_ms":1},"data":{"template":{"chord_pattern":{"profile":"HSS3X3X1/4","profile_policy":"warn","continuous":true},"origin":{"document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636],"connection_id":null,"drawing":"Detalle D"},"member_pattern":[{"side":"+Y","slot":0,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":136.91999999999999},{"side":"+Y","slot":1,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":44.369999999999997},{"side":"-Y","slot":2,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","profile_policy":"warn","angle_deg":-135.63000000000000}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"description":null,"template_id":"648b91fd-f886-4e56-b242-e0300121b9ea","spec_template":{"spec_version":"1.0","gusset":{"thickness_mm":9.5250000000000004,"width_mm":565.0,"thickness_label":"3/8\"","height_mm":530.0,"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"chord_interface":"through_slot"},"chord":{"profile":"HSS3X3X1/4","continuous":true},"source":{"scale":"1/10","drawing":"Detalle D"},"connect ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=cc0ddbcf05ae...
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_apply","duration_ms":33},"data":{"validation_token":"cc0ddbcf05ae89d8a29b6d29e3d83902df8eb790aa5d6505a4360deef6fd4444","bolt_stacks":[{"member_element_id":1249636,"gusset_face":"+z","length_source":"computed_from_grip","grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003}],"is_valid":true,"errors_count":0,"proposal":null,"calculated_values":{"outline_mode":null,"frame_x":[1,0,0],"outline_width_mm":null,"outline_points_mm":null,"outline_height_mm":null,"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"eccentricity_mm":3.3999999999999999,"chord_direction_reversed":true,"frame_y":[0,0,1],"outline_vertices":null,"kind":"node"},"template_id":"648b91fd-f886-4e56-b242-e0300121b9ea","match":{"max_deviation_deg":0,"score_deg":0.01,"matched_count":3,"assignments":[{"side":"+Y","template_profile":"HSS2-1/2X2-1/2X3/16","template_angle_deg":136.91999999999999,"slot":0,"deviation_deg":0,"model_angle_deg":136.91999999999999,"role":"diagonal","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249630,"profile_policy":"warn"},{"side":"+Y","template_profile":"HSS2-1/2X2-1/2X3/16","template_angle_deg":44.369999999999997,"slot":1,"deviation_deg":0,"model_angle_deg":44.369999999999997,"role":"diagonal","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249631,"profile_policy":"warn"},{"side":"-Y","template_profile":"HSS2-1/2X2-1/2X3/16","templat ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_apply","duration_ms":27},"data":{"validation_token":"cc0ddbcf05ae89d8a29b6d29e3d83902df8eb790aa5d6505a4360deef6fd4444","bolt_stacks":[{"member_element_id":1249636,"gusset_face":"+z","length_source":"computed_from_grip","grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003}],"is_valid":true,"errors_count":0,"proposal":{"candidates":[{"max_deviation_deg":0,"is_mirrored":false,"score_deg":0.01,"description":"PRUEBA probar_conexiones \u00b7 igual \u00b7 desv\u00edo m\u00e1x. 0,0\u00b0","template_id":"648b91fd-f886-4e56-b242-e0300121b9ea","name":"PRUEBA probar_conexiones","orientation":"same"},{"max_deviation_deg":0,"is_mirrored":false,"score_deg":0.01,"description":"Nudo tipico Detalle D \u00b7 igual \u00b7 desv\u00edo m\u00e1x. 0,0\u00b0","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico Detalle D","orientation":"same"}],"summary_text":"Plantilla propuesta: PRUEBA probar_conexiones \u00b7 igual \u00b7 desv\u00edo m\u00e1x. 0,0\u00b0. Otras que encajan: Nudo tipico Detalle D (igual, 0,0\u00b0).","attempts":["Nudo K cordon superior \u2192 same: ranura 0 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal -45.0\u00b0) sin barra; barras sobrantes: 1249630, 1249631","Nudo K cordon superior \u2192 mirror_x: ranura 0 (diagonal -44.4\u00b0) sin barra; ranura 1 (diagonal -135.0\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.6\u00b0); barras ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=8fc94091-017b-4b54-a4c8-9977b92f93fb nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_plan","duration_ms":76},"data":{"split_step_count":0,"selection_count":4,"hidden_text":"5 barras sueltas","overrides":{"add_node":{},"split":{},"replace_existing":false,"exclude":[],"template":{},"chord":{},"add_member":{},"merge":[],"remove_member":{},"spec":{}},"updated_utc":"2026-10-31T17:17:48.1199654Z","plan_id":"8fc94091-017b-4b54-a4c8-9977b92f93fb","summary_text":"Se crear\u00e1 1 conexi\u00f3n con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.","failed_count":0,"has_batch_connections":false,"description":"6 nudo(s): 5 untyped, 1 ready.","nodes":[{"errors":[],"existing_batch_id":null,"split_steps":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"validation_token":null,"is_mirrored":false,"match":null,"element_ids":[1249630],"color_name":null,"chord_continuous":false,"spec":null,"is_valid":false,"splice_element_id":null,"advice":"No es un nudo: nada que hacer","kind":"node","color_rgb":null,"members":[],"split_of_node":null,"visible_by_default":false,"status_text":"\u25cb Barra suelta (no es nudo)","warnings":[],"through_element_ids":[],"connection_type":null,"marker_element_id":null,"template_id":null,"chord_element_id":0,"chord_type_name":null,"errors_count":0,"starter":false,"label_index":null,"is_marked":false,"has_spec_override":false,"warnings_count":0,"created_connec ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=04c50a618992...
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_plan_get","duration_ms":1},"data":{"node":{"errors":[],"existing_batch_id":null,"split_steps":[],"status_detail":null,"work_point_mm":[-11869.400000000000,-17195.799999999999,17423],"validation_token":"04c50a618992cb845ba6d952f16c647eac131071865806dc83272a23be990d6d","is_mirrored":false,"match":{"max_deviation_deg":0,"score_deg":0.01,"matched_count":3,"assignments":[{"side":"+Y","template_profile":"HSS2-1/2X2-1/2X3/16","template_angle_deg":136.91999999999999,"slot":0,"deviation_deg":0,"model_angle_deg":136.91999999999999,"role":"diagonal","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249630,"profile_policy":"warn"},{"side":"+Y","template_profile":"HSS2-1/2X2-1/2X3/16","template_angle_deg":44.369999999999997,"slot":1,"deviation_deg":0,"model_angle_deg":44.369999999999997,"role":"diagonal","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249631,"profile_policy":"warn"},{"side":"-Y","template_profile":"HSS2-1/2X2-1/2X3/16","template_angle_deg":-135.63000000000000,"slot":2,"deviation_deg":0,"model_angle_deg":-135.63000000000000,"role":"diagonal","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249636,"profile_policy":"warn"}],"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, d ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote 8fc94091: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_create","duration_ms":13},"data":{"restored_members_count":0,"started_utc":"2026-10-31T17:17:48.2892379Z","skipped_count":0,"with_warnings_count":0,"duration_ms":4,"plan_id":"8fc94091-017b-4b54-a4c8-9977b92f93fb","summary_text":"Lote 8fc94091: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","rolled_back_count":0,"failed_count":1,"finished_utc":"2026-10-31T17:17:48.2921026Z","deleted_count":0,"nodes":[{"kind":"node","split_id":null,"new_element_id":null,"duration_ms":0,"connection_id":null,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","outcome":"failed","elements_count":0,"template_name":"PRUEBA probar_conexiones","restored_members_count":0,"reason":"token distinto del plan","orientation":"same","warnings":[],"element_id":null,"spec":null,"errors":[{"message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","path":"nodes[N4].validation_token","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","code":"VALIDATION_TOKEN_INVALID"}],"node":"N4"}],"plan_creatable_count":1,"created_count":0,"plan_summary_text":"Creadas 0 conexiones, 1 fall\u00f3. Pulsa Crear otra vez para rei ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_delete","duration_ms":12},"data":{"restored_members_count":0,"started_utc":"2026-10-31T17:17:48.3370702Z","skipped_count":0,"with_warnings_count":0,"duration_ms":10,"plan_id":null,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","rolled_back_count":0,"failed_count":0,"finished_utc":"2026-10-31T17:17:48.3476511Z","deleted_count":0,"nodes":[],"plan_creatable_count":null,"created_count":0,"plan_summary_text":null,"batch_id":"00000000-0000-0000-0000-000000000000","updated_count":0,"connection_ids":[],"stop_on_error":false,"split_count":0,"stopped_at":null,"document":"HANGAR_PRUEBA_sondeo","undo_entries":"one","unsplit_count":0,"stopped":false,"plan_summary":null,"operation":"batch_delete","deleted_elements_count":0},"warnings":[{"message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","path":"batch_id","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","code":"BATCH_EMPTY"}],"errors":[],"ok":true}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado 8fc94091-017b-4b54-a4c8-9977b92f93fb
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "batch_plan_discard", "duration_ms": 22}, "data": {"discarded_plan_id": "8fc94091-017b-4b54-a4c8-9977b92f93fb", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "warnings": [], "errors": [], "ok": true}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_apply","duration_ms":0},"data":null,"warnings":[],"errors":[{"message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","path":"template_id","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","code":"TEMPLATE_NOT_FOUND"}],"ok":false}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 648b91fd-f886-4e56-b242-e0300121b9ea
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"catalog_delete","duration_ms":2},"data":{"deleted_template_id":"648b91fd-f886-4e56-b242-e0300121b9ea","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\648b91fd-f886-4e56-b242-e0300121b9ea.json"},"warnings":[],"errors":[],"ok":true}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_plan","duration_ms":1214},"data":{"split_step_count":31,"selection_count":64,"hidden_text":"8 sin cord\u00f3n, 17 barras sueltas","overrides":{"add_node":{},"split":{},"replace_existing":false,"exclude":[],"template":{},"chord":{},"add_member":{},"merge":[],"remove_member":{},"spec":{}},"updated_utc":"2026-10-31T17:17:49.1119729Z","plan_id":"3dda5f6f-472b-4d99-986c-5ab61138e67b","summary_text":"Se crear\u00e1n 41 conexiones: 30 con 2 plantillas (Nudo tipico Detalle D: 16, Nudo K cordon superior: 14) (22 iguales, 8 en espejo), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I escalado), 3 soldaduras directas (arranque). 14 avisan de perfil distinto. 2 avisan de excentricidad. 7 nudos en K est\u00e1n sobre un empalme. Adem\u00e1s se parten 31 diagonales a 330 (Detalle B) y se crean 31 empalmes B antes de sus nudos en K. Ocultos: 8 sin cord\u00f3n, 17 barras sueltas.","failed_count":0,"has_batch_connections":false,"description":"97 nudo(s): 41 ready, 31 after_split, 17 untyped, 8 no_match.","nodes":[{"errors":[],"existing_batch_id":null,"split_steps":[],"status_detail":"Ninguna plantilla casa todas sus ranuras con las barras del nudo.","work_point_mm":[-14551.900000000000,-17195.799999999999,19933],"validation_token":null,"is_mirrored":false,"match":null,"element_ids":[1245533,1249630],"color_name":null,"chord_continuous":false,"spec":null,"is_valid":false,"splice_element_id":null,"advice":"Falta el cord\u00f3n en ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 3dda5f6f-472b-4d99-986c-5ab61138e67b etiquetas quitadas=0
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "batch_plan_discard", "duration_ms": 11}, "data": {"discarded_plan_id": "3dda5f6f-472b-4d99-986c-5ab61138e67b", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "warnings": [], "errors": [], "ok": true}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "validate", "duration_ms": 5}, "data": {"bolt_stacks": [{"member_element_id": 1245531, "length_source": "computed_from_grip", "plates": 2, "grip_mm": 25.399999999999999, "bolt_length_mm": 50.799999999999997, "bolts": 12, "gap_mm": 0}], "errors_count": 0, "is_valid": true, "calculated_values": {"outline_mode": null, "frame_x": [1, 0, 0], "outline_width_mm": null, "outline_points_mm": null, "outline_height_mm": null, "frame_z": [0, -1, 0], "axis_distance_mm": 0, "origin_mm": [-4180.5, 17204.200000000001, 19933], "eccentricity_mm": 0, "chord_direction_reversed": true, "frame_y": [0, 0, 1], "outline_vertices": null, "kind": "splice"}, "warnings_count": 0, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8"}, "warnings": [], "errors": [], "ok": true}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":7},"data":{"bolt_stacks":[],"errors_count":0,"is_valid":true,"calculated_values":{"outline_mode":null,"frame_x":[1,0,0],"outline_width_mm":null,"outline_points_mm":null,"outline_height_mm":null,"frame_z":[0,-1,0],"axis_distance_mm":0.089999999999999997,"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"eccentricity_mm":54.899999999999999,"chord_direction_reversed":true,"frame_y":[0,0,1],"outline_vertices":null,"kind":"node"},"warnings_count":2,"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9"},"warnings":[{"message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","path":"members[0].axis_offset_mm","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","code":"MEMBER_ECCENTRIC"},{"message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","path":"members[1].axis_offset_mm","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","code":"MEMBER_ECCENTRIC"}],"errors":[],"o ...
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo (arranque) arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_plan","duration_ms":15},"data":{"split_step_count":0,"selection_count":2,"hidden_text":"2 barras sueltas","overrides":{"add_node":{},"split":{},"replace_existing":false,"exclude":[],"template":{},"chord":{},"add_member":{},"merge":[],"remove_member":{},"spec":{}},"updated_utc":"2026-10-31T17:17:50.2031297Z","plan_id":"2bf20b8a-0647-407f-b31e-52620d66b64b","summary_text":"Se crear\u00e1 1 conexi\u00f3n: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.","failed_count":0,"has_batch_connections":false,"description":"3 nudo(s): 2 untyped, 1 ready.","nodes":[{"errors":[],"existing_batch_id":null,"split_steps":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14656,17204.299999999999,19933],"validation_token":null,"is_mirrored":false,"match":null,"element_ids":[1245531],"color_name":null,"chord_continuous":false,"spec":null,"is_valid":false,"splice_element_id":null,"advice":"No es un nudo: nada que hacer","kind":"node","color_rgb":null,"members":[],"split_of_node":null,"visible_by_default":false,"status_text":"\u25cb Barra suelta (no es nudo)","warnings":[],"through_element_ids":[],"connection_type":null,"marker_element_id":null,"template_id":null,"chord_element_id":0,"chord_type_name":null,"errors_count":0,"starter":false,"label_index":null,"is_marked":false,"has_spec_override":false,"warnings_count":0,"created_connection_id":null,"name":"N1","status":"untyped","i ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado 2bf20b8a-0647-407f-b31e-52620d66b64b
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "batch_plan_discard", "duration_ms": 11}, "data": {"discarded_plan_id": "2bf20b8a-0647-407f-b31e-52620d66b64b", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "warnings": [], "errors": [], "ok": true}
======================================================================
31. POST /conn/validate/ del Detalle D con gusset.outline.mode auto (sin puntos) -> válido, calculated_values.outline_points_mm y la caja (Fase 13)  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH'], 8 vértices, caja 540 × 585 mm, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":27},"data":{"bolt_stacks":[{"member_element_id":1249636,"gusset_face":"+z","length_source":"computed_from_grip","grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003}],"errors_count":0,"is_valid":true,"calculated_values":{"outline_mode":"auto","frame_x":[1,0,0],"outline_width_mm":540,"outline_points_mm":[[-315,-80],[-290,-140],[-150,-285],[225,-80],[225,135],[135,220],[-230,300],[-315,210]],"outline_height_mm":585,"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"eccentricity_mm":3.3999999999999999,"chord_direction_reversed":true,"frame_y":[0,0,1],"outline_vertices":8,"kind":"node"},"warnings_count":3,"validation_token":"3cecf67b7243f8fcd0e05e709b87c59e6a3a0792568b7fed2c1afbc1ba607c39"},"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"},{"message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","path":"gusset.width_mm","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00e9jalo: es informativo con outline.mode auto.","code ...
======================================================================
31b. POST /conn/preview/ con outline.mode auto -> gusset_plate con outline_mode auto y los mismos puntos que validate (Fase 13)  [OK]  HTTP 200, ok=True, cartela con 8 vértices (los mismos que validate: True)
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "preview", "duration_ms": 6}, "data": {"calculated_values": {"outline_mode": "auto", "frame_x": [1, 0, 0], "outline_width_mm": 540, "outline_points_mm": [[-315, -80], [-290, -140], [-150, -285], [225, -80], [225, 135], [135, 220], [-230, 300], [-315, 210]], "outline_height_mm": 585, "frame_z": [0, -1, 0], "axis_distance_mm": 0.080000000000000002, "origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "eccentricity_mm": 3.3999999999999999, "chord_direction_reversed": true, "frame_y": [0, 0, 1], "outline_vertices": 8, "kind": "node"}, "summary": {"working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "dry_run": true, "chord_element_id": 1249510, "weld_lines": 6, "gusset_plates": 1, "backend": "advancesteel", "members_modified": 3, "knife_plates": 1, "connection_type": "gusset_node", "first_member_element_id": 1249630, "bolts": 4}, "members_to_modify": [{"new_extension_mm": -95.099999999999994, "current_end_distance_mm": 84.900000000000006, "end": "end", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "element_id": 1249630}, {"new_extension_mm": -40.799999999999997, "current_end_distance_mm": 19.199999999999999, "end": "start", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "setback_mm": 60, "profile": "HSS2-1-2X2-1-2X3-16 64x ...
======================================================================
31c. POST /conn/validate/ con outline.mode auto Y points_mm -> SCHEMA_INVALID (Fase 13)  [OK]  HTTP 200, ok=False, errores=['SCHEMA_INVALID'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"validate","duration_ms":10},"data":null,"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"},{"message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","path":"gusset.width_mm","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00e9jalo: es informativo con outline.mode auto.","code":"LABEL_VALUE_MISMATCH"},{"message":"gusset.height_mm (530.0 mm) no coincide con el alto de la cartela autom\u00e1tica calculada (585.0 mm): manda el calculado.","path":"gusset.height_mm","hint":"Escribe el alto calculado (calculated_values.outline_height_mm) o d\u00e9jalo: es informativo con outline.mode auto.","code":"LABEL_VALUE_MISMATCH"}],"errors":[{"message":"outline.mode \"auto\" no lleva points_mm: el contorno lo calcula el add-in (usa polygon si quieres fijar los puntos).","path":"gusset.outline.points_mm","hint":"Quita points_mm (o pon null) para la cartela autom\u00e1tica, o cambia mode a \"polygon\".","code":"SCHEMA_INVALID"}],"ok":false}
======================================================================
32. POST /conn/split/ dry_run de la diagonal 1249632 contra el cordón 1245533 -> el punto del Detalle B (≈ 485 mm) sin tocar nada (Fase 13, D7)  [OK]  HTTP 200, ok=True, θ=45 cara=143.7 primera plancha=473.7 corte=483.2 mm (corto None, largo None)
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"split","duration_ms":6},"data":{"dry_run":true,"description":"La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cord\u00f3n 1245533 a 45,0\u00b0: la cara del cord\u00f3n queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo),","preview":{"theta_deg":45,"plate_thickness_mm":9.5250000000000004,"short_segment_mm":521.60000000000002,"chord_element_id":1245533,"long_segment_mm":3024.4000000000001,"first_plate_distance_mm":473.69999999999999,"offset_from_chord_face_mm":330,"chord_type":"HSS12X8X1/2","cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"face_distance_mm":143.69999999999999,"element_id":1249632,"split_distance_mm":483.19999999999999,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"member_type":"HSS2-1-2X2-1-2X3-16 64x64","gap_mm":0},"already_split":null},"warnings":[],"errors":[],"ok":true}
======================================================================
32b. POST /conn/node_info/ del nudo en K -> split_point_mm y split_distance_mm por diagonal (Fase 13)  [OK]  HTTP 200, ok=True, [(1245533, None), (1249631, 484.8), (1249632, 483.2)]
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "node_info", "duration_ms": 8}, "data": {"existing_connections": [], "y_axis": [0, -5.0000000000000004e-06, 1], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "chord_element_id": 1245533, "axis_distance_mm": 0.089999999999999997, "origin_mm": [-9278.1000000000004, -17195.700000000001, 19933], "eccentricity_mm": 54.899999999999999, "chord_direction_reversed": true, "members": [{"end_gap_mm": 0, "axis_offset_mm": 0, "section_rotation_deg": 90, "type": "HSS12X8X1/2", "height_mm": 304.80000000000001, "length_mm": 10475, "structural_type": "Beam", "element_id": 1245533, "node_end": 0, "width_mm": 203.19999999999999, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "side": "chord", "angle_in_plane_deg": -180, "split_distance_mm": null, "split_offset_from_chord_face_mm": null, "slope_deg": 0, "is_chord": true, "angle_to_chord_deg": 0, "split_point_mm": null, "reaches_node": true, "wall_mm": 11.810000000000000, "end_mm": [-14655.200000000001, -17195.700000000001, 19933], "axis_cut_mm": [-9278.1000000000004, -17195.700000000001, 19933], "family": "HSS-Hollow Structural Section", "start_mm": [-4180.1999999999998, -17195.799999999999, 19933]}, {"end_gap_mm": 90.400000000000006, "axis_offset_mm": -27.399999999999999, "section_rotation_deg": -180, "type": "HSS2-1-2X2-1-2X3-16 64x64", "height_mm": 63.5, "length_mm": 3500, "structural_type": "Beam ...
======================================================================
32c. POST /conn/split/ REAL de la diagonal 1249632 -> split_id y new_element_id (ESCRIBE: se deshace en la 32e) (Fase 13)  [OK]  HTTP 200, ok=True, avisos=['REVIT_WARNING'], split_id=c4bf59ea-f64b-47f1-b804-13f2128d72e0 tramo nuevo=1322532 mecanismo=copy_element avisos=['REVIT_WARNING']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"split","duration_ms":100},"data":{"split":{"split_id":"c4bf59ea-f64b-47f1-b804-13f2128d72e0","batch_id":null,"new_element_id":1322532,"chord_element_id":1245533,"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"connection_id":null,"offset_from_chord_face_mm":330,"original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001],"created_utc":"2026-10-31T17:17:50.5141611Z","approximated_parameters":[],"mechanism":"copy_element","element_id":1249632,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"split_distance_mm":483.19999999999999},"description":"La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cord\u00f3n 1245533 a 45,0\u00b0: la cara del cord\u00f3n queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo), Hecho: tramo nuevo 1322532.","next":"conn_get_node_info con [1249632, 1322532] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.","preview":{"theta_deg":45,"plate_thickness_mm":9.5250000000000004,"short_segment_mm":521.60000000000002,"chord_element_id":1245533,"long_segment_mm":3024.4000000000001,"first_plate_distance_mm":473.69999999999999,"offset_from_chord_face_mm":330,"chord_type":"HSS12X8X1/2","cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"face_distance_mm":143.69999999999999,"element_id":1249632,"s ...
======================================================================
32d. POST /conn/node_info/ de los dos tramos -> kind splice (el empalme B se puede aplicar) (Fase 13)  [OK]  HTTP 200, ok=True, kind=splice barras=[1249632, 1322532]
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "node_info", "duration_ms": 6}, "data": {"existing_connections": [], "y_axis": [0.70710700000000004, 1.9999999999999999e-06, 0.70710700000000004], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "chord_element_id": 1322532, "axis_distance_mm": 0, "origin_mm": [-8909, -17195.799999999999, 19591.299999999999], "eccentricity_mm": 0, "chord_direction_reversed": false, "members": [{"end_gap_mm": 0, "axis_offset_mm": 0, "section_rotation_deg": -180, "type": "HSS2-1-2X2-1-2X3-16 64x64", "height_mm": 63.5, "length_mm": 521.60000000000002, "structural_type": "Beam", "element_id": 1249632, "node_end": 1, "width_mm": 63.5, "material": "Material IFC (190-40-140)", "side": "-Y", "angle_in_plane_deg": -180, "split_distance_mm": null, "split_offset_from_chord_face_mm": null, "slope_deg": 45, "is_chord": false, "angle_to_chord_deg": 0, "split_point_mm": null, "reaches_node": true, "wall_mm": 0, "end_mm": [-8909, -17195.799999999999, 19591.299999999999], "axis_cut_mm": [-8909, -17195.799999999999, 19591.299999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-9277.7999999999993, -17195.799999999999, 19960.099999999999]}, {"end_gap_mm": 0, "axis_offset_mm": 0, "section_rotation_deg": -180, "type": "HSS2-1-2X2-1-2X3-16 64x64", "height_mm": 63.5, "length_mm": 3024.4000000000001, "structural_type": "Beam", "element_id": 1322532, "node_end": 0, "width_mm": 63 ...
======================================================================
32e. POST /conn/unsplit/ por split_id -> deshecho, la diagonal vuelve a ser una (Fase 13, D8)  [OK]  HTTP 200, ok=True, Corte deshecho: el tramo 1322532 se borró y la barra 1249632 recuperó su curva. avisos=[]
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"unsplit","duration_ms":48},"data":{"complete":true,"unsplit":{"split_id":"c4bf59ea-f64b-47f1-b804-13f2128d72e0","batch_id":null,"new_element_id":1322532,"chord_element_id":1245533,"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"connection_id":null,"offset_from_chord_face_mm":330,"original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001],"created_utc":"2026-10-31T17:17:50.5141611Z","approximated_parameters":[],"mechanism":"copy_element","element_id":1249632,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"split_distance_mm":483.19999999999999},"description":"Corte deshecho: el tramo 1322532 se borr\u00f3 y la barra 1249632 recuper\u00f3 su curva."},"warnings":[],"errors":[],"ok":true}
======================================================================
32f. POST /conn/unsplit/ de la barra ya entera -> SPLIT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['SPLIT_NOT_FOUND']
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"unsplit","duration_ms":3},"data":null,"warnings":[],"errors":[{"message":"La barra 1249632 no est\u00e1 partida por el add-in (ning\u00fan corte la tiene como tramo).","path":"element_id","hint":"conn_unsplit sin par\u00e1metros lista los cortes del modelo.","code":"SPLIT_NOT_FOUND"}],"ok":false}
======================================================================
33. POST /conn/batch/plan/ (mark:false) del nudo en K -> split_steps por diagonal y los empalmes B previstos (after_split) detrás (Fase 13, D5)  [OK]  HTTP 200, ok=True, pasos=[(1249631, 484.8), (1249632, 483.2)] previstos=['● Empalme B (tras partir)', '● Empalme B (tras partir)'] | Se creará 1 conexión con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Además se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.
Cuerpo:
{"meta":{"addin_version":"0.13.1","operation":"batch_plan","duration_ms":32},"data":{"split_step_count":2,"selection_count":3,"hidden_text":"4 barras sueltas","overrides":{"add_node":{},"split":{},"replace_existing":false,"exclude":[],"template":{},"chord":{},"add_member":{},"merge":[],"remove_member":{},"spec":{}},"updated_utc":"2026-10-31T17:17:50.8273563Z","plan_id":"1ee66339-2a98-4d45-8ac8-0fd2bf94bcdb","summary_text":"Se crear\u00e1 1 conexi\u00f3n con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Adem\u00e1s se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.","failed_count":0,"has_batch_connections":false,"description":"7 nudo(s): 4 untyped, 2 after_split, 1 ready.","nodes":[{"errors":[],"existing_batch_id":null,"split_steps":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14655.200000000001,-17195.700000000001,19933],"validation_token":null,"is_mirrored":false,"match":null,"element_ids":[1245533],"color_name":null,"chord_continuous":false,"spec":null,"is_valid":false,"splice_element_id":null,"advice":"No es un nudo: nada que hacer","kind":"node","color_rgb":null,"members":[],"split_of_node":null,"visible_by_default":false,"status_text":"\u25cb Barra suelta (no es nudo)","warnings":[],"through_element_ids":[],"connection_type":null,"marker_element_id":null,"template_id":null,"chord_element_id":0,"chord_type_name":null,"e ...
======================================================================
33b. POST /conn/batch/plan/discard/ del plan del nudo en K -> descartado  [OK]  HTTP 200, ok=True, descartado
Cuerpo:
{"meta": {"addin_version": "0.13.1", "operation": "batch_plan_discard", "duration_ms": 12}, "data": {"discarded_plan_id": "1ee66339-2a98-4d45-8ac8-0fd2bf94bcdb", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "warnings": [], "errors": [], "ok": true}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=91 conn_*=25
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete, conn_split_member, conn_unsplit
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.13.1
Cuerpo:
{
  "meta": {
    "addin_version": "0.13.1",
    "operation": "ping",
    "duration_ms": 2
  },
  "data": {
    "has_uidocument": true,
    "spec_version": "1.0",
    "revit": {
      "version_name": "Autodesk Revit 2027",
      "version_build": "27.2.0.39",
      "sub_version_number": "2027.2",
      "language": "English_USA",
      "version_number": "2027"
    },
    "document": {
      "is_family": false,
      "is_modifiable": false,
      "is_read_only": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "title": "HANGAR_PRUEBA_sondeo",
      "is_workshared": false
    },
    "backend": "advancesteel",
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
      "split",
      "types",
      "unsplit",
      "update",
      "validate"
    ],
    "dotnet": {
      "load_context": "Default",
      "framework": ".NET 10.0.12",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
    },
    "addin_version": "0.13.1"
  },
  "warnings": [],
  "errors": [],
  "ok": true
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "meta": {
    "addin_version": "0.13.1",
    "operation": "catalog_apply",
    "duration_ms": 20
  },
  "data": {
    "validation_token": "406941cf88fcb49d7e049af6f316cccf6a624bc3e446accb8e008c62cb40f65c",
    "bolt_stacks": [
      {
        "member_element_id": 1249636,
        "gusset_face": "+z",
        "length_source": "computed_from_grip",
        "grip_mm": 19.525,
        "bolt_length_mm": 44.45
      }
    ],
    "is_valid": true,
    "errors_count": 0,
    "proposal": {
      "candidates": [
        {
          "max_deviation_deg": 0,
          "is_mirrored": false,
          "score_deg": 0.01,
          "description": "PRUEBA probar_conexiones · igual · desvío máx. 0,0°",
          "template_id": "d5946838-72a2-4bf1-ad44-447f6b3e45da",
          "name": "PRUEBA probar_conexiones",
          "orientation": "same"
        },
        {
          "max_deviation_deg": 0,
          "is_mirrored": false,
          "score_deg": 0.01,
          "description": "Nudo tipico Detalle D · igual · desvío máx. 0,0°",
          "template_id": "6abcf116-9b97-485f-b50d-2851ca0018cc",
          "name": "Nudo tipico Detalle D",
          "orientation": "same"
        }
      ],
      "summary_text": "Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igual, 0,0°).",
      "attempts": [
        "Nudo K cordon superior → same: ranura 0 (diagonal -135.6°) → barra 1249636 (-135.6°, desvío 0.0°); ranura 1 (diagonal -45.0° ...
======================================================================
Resultado: 49/49 pruebas correctas

```

## 13b-6 log failed

```text

{"ts":"2026-10-31T11:03:20.9243754-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"validation_token\": \"0000000000000000000000000000000000000000000000000000000000000000\", \"node\": 
\"N4\"}], \"plan_id\": \"263b7e4c-3d82-44ef-b4c9-44053440da9e\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NO
DE_FAILED"],"duration_ms":20}}
{"ts":"2026-10-31T11:05:29.7925532-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"validation_token\": \"0000000000000000000000000000000000000000000000000000000000000000\", \"node\": 
\"N4\"}], \"plan_id\": \"d0d4b74f-1308-4205-b334-6f889c33962f\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NO
DE_FAILED"],"duration_ms":1}}
{"ts":"2026-10-31T11:07:13.1974864-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"validation_token\": \"0000000000000000000000000000000000000000000000000000000000000000\", \"node\": 
\"N4\"}], \"plan_id\": \"3ca6e2c8-0ee2-4db5-b1cf-ea5275f5930c\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NO
DE_FAILED"],"duration_ms":0}}
{"ts":"2026-10-31T11:07:35.4196025-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"validation_token\": \"0000000000000000000000000000000000000000000000000000000000000000\", \"node\": 
\"N4\"}], \"plan_id\": \"fd9979c8-9f6f-40f0-a201-c4232b5ca24a\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NO
DE_FAILED"],"duration_ms":1}}
{"ts":"2026-10-31T12:15:51.6503354-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 0f03b68e-7b47-4bd6-911e-111bc60d18f0","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at System.RuntimeMethodHandle.InvokeMethod(ObjectHandleOnStack target, Void** 
arguments, ObjectHandleOnStack sig, BOOL isConstructor, ObjectHandleOnStack result)\r\n   at 
System.Reflection.MethodBaseInvoker.InterpretedInvoke_Constructor(Object obj, IntPtr* args)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeDirectByRefWithFewArgs(Object obj, Span`1 copyOfArgs, BindingFlags 
invokeAttr)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeDirectByRefWithFewArgs(Object obj, Span`1 copyOfArgs, BindingFlags 
invokeAttr)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] parameters)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 419"}}
{"ts":"2026-10-31T12:17:48.3017851-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"p
lan_id\": \"8fc94091-017b-4b54-a4c8-9977b92f93fb\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"000000000000
0000000000000000000000000000000000000000000000000000\"}]}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAIL
ED"],"duration_ms":13}}



```

## 13b-6 log cortes

```text

{"ts":"2026-10-31T12:11:06.8074441-05:00","record":{"event":"split_member","element_id":1249632,"chord_element_id":1245
533,"new_element_id":1322488,"split_id":"b67f7557-8564-4cbc-8906-b34b31fbe6f0","mechanism":"copy_element","duration_ms"
:205}}
{"ts":"2026-10-31T12:12:14.3712818-05:00","record":{"event":"member_unsplit","element_id":1249632,"new_element_id":1322
488,"problems":[]}}
{"ts":"2026-10-31T12:12:14.4129804-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
488,"split_id":"b67f7557-8564-4cbc-8906-b34b31fbe6f0","complete":true}}
{"ts":"2026-10-31T12:15:09.7520349-05:00","record":{"event":"member_split","element_id":1249631,"new_element_id":132251
0,"mechanism":"copy_element","split_point_mm":[-9652.081580707123,-17195.790726741696,19593.960443147163],"split_distan
ce_mm":484.807,"short_mm":416.1,"long_mm":3083.9,"approximated":[]}}
{"ts":"2026-10-31T12:15:09.7931108-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322510","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249631"}}
{"ts":"2026-10-31T12:15:10.5991299-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322515","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.5996190-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322516","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.5999152-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322517","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.6001934-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322518","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.6004790-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322519","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.6007873-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322520","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.6010614-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322521","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.6013030-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322522","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar) · Detalle B"}}
{"ts":"2026-10-31T12:15:10.7052265-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322526","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar)"}}
{"ts":"2026-10-31T12:15:51.6827212-05:00","record":{"event":"member_unsplit","element_id":1249631,"new_element_id":1322
510,"problems":[]}}
{"ts":"2026-10-31T12:17:50.5482022-05:00","record":{"event":"member_split","element_id":1249632,"new_element_id":132253
2,"mechanism":"copy_element","split_point_mm":[-8908.9765873183,-17195.789149833254,19591.319569910534],"split_distance
_mm":483.209,"short_mm":521.6,"long_mm":3024.4,"approximated":[]}}
{"ts":"2026-10-31T12:17:50.5890906-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322532","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}
{"ts":"2026-10-31T12:17:50.5975393-05:00","record":{"event":"split_member","element_id":1249632,"chord_element_id":1245
533,"new_element_id":1322532,"split_id":"c4bf59ea-f64b-47f1-b804-13f2128d72e0","mechanism":"copy_element","duration_ms"
:90}}
{"ts":"2026-10-31T12:17:50.7204072-05:00","record":{"event":"member_unsplit","element_id":1249632,"new_element_id":1322
532,"problems":[]}}
{"ts":"2026-10-31T12:17:50.7562184-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
532,"split_id":"c4bf59ea-f64b-47f1-b804-13f2128d72e0","complete":true}}



```
