# Resultados de la ronda 13c

Fecha: 2026-10-31T12:35:46


## 13c-1 git

```text
143a1eb Cierre de la ronda 13b (add-in 0.13.2): la advertencia del corte al log, las uniones restauradas al deshacer y el sondeo 30 v3
?? docs/fases/resultados-fase-13c.md

```

## 13c-1 build

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 193 ms).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 193 ms).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 193 ms).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:09.14

```

## 13c-1 test

```text
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   314, Omitido:     0, Total:   314, Duración: 1 s - MotorConexiones.Tests.dll (net10.0)

```

## 13c-1 deploy

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

Tiempo transcurrido 00:00:01.25
== MotorConexiones 0.13.2.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 13c-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (27 rutas @api.route)
- copiado tools\conn_tools.py (25 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 13c-2 ping

```text
== conn/ping -> HTTP 200 en 370 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "has_uidocument":  true,
                 "spec_version":  "1.0",
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
                 "addin_version":  "0.13.2",
                 "document":  {
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "is_workshared":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false
                              },
                 "revit":  {
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "language":  "English_USA",
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027"
                           },
                 "dotnet":  {
                                "load_context":  "Default",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12"
                            }
             },
    "meta":  {
                 "operation":  "ping",
                 "addin_version":  "0.13.2",
                 "duration_ms":  17
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-2 list

```text
== conn/list -> HTTP 200 en 48 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "total_count":  0,
                 "batch_id":  null,
                 "batches":  {

                             },
                 "connections_count":  0,
                 "connections":  [

                                 ]
             },
    "meta":  {
                 "operation":  "list",
                 "addin_version":  "0.13.2",
                 "duration_ms":  8
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-2 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 19 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  0,
                 "splits":  [

                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  5
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-2 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 179 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 7 ms | errores=- | avisos=-
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

## 13c-2 sondeo 13

```text
== 13-limpiar-fase1.py -> HTTP 200 en 46 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 13c-2 node_info nudo K antes de nada

```text
== conn/node_info -> HTTP 200 en 107 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "members":  [
                                 {
                                     "length_mm":  10475,
                                     "angle_in_plane_deg":  -180,
                                     "wall_mm":  11.810000000000000,
                                     "is_chord":  true,
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "slope_deg":  0,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  0,
                                     "angle_to_chord_deg":  0,
                                     "side":  "chord",
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "split_distance_mm":  null,
                                     "width_mm":  203.19999999999999,
                                     "split_point_mm":  null,
                                     "split_offset_from_chord_face_mm":  null,
                                     "type":  "HSS12X8X1/2",
                                     "node_end":  0,
                                     "element_id":  1245533,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  90,
                                     "height_mm":  304.80000000000001
                                 },
                                 {
                                     "length_mm":  3500,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "slope_deg":  44.369999999999997,
                                     "end_gap_mm":  90.400000000000006,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  -27.399999999999999,
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  484.80000000000001,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -9652.1000000000004,
                                                            -17195.700000000001,
                                                            19594
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  1,
                                     "element_id":  1249631,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 },
                                 {
                                     "length_mm":  3546,
                                     "angle_in_plane_deg":  -45,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "slope_deg":  45,
                                     "end_gap_mm":  27.100000000000001,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  27.399999999999999,
                                     "angle_to_chord_deg":  45,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  483.19999999999999,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -8909,
                                                            -17195.700000000001,
                                                            19591.299999999999
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  0,
                                     "element_id":  1249632,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 }
                             ],
                 "kind":  "node",
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "chord_element_id":  1245533,
                 "chord_direction_reversed":  true,
                 "existing_connections":  [

                                          ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "eccentricity_mm":  54.899999999999999,
                 "axis_distance_mm":  0.089999999999999997
             },
    "meta":  {
                 "operation":  "node_info",
                 "addin_version":  "0.13.2",
                 "duration_ms":  80
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-3 sondeo 30 v3

```text
== 30-partir-barra.py -> HTTP 200 en 7818 ms ==
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
   diagonal 1249631 ANTES: uniones automaticas permitidas: inicio=False fin=False
   cordon 1245533: uniones automaticas permitidas: inicio=False fin=False
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
   theta = 44.4 grados | cara a 145.3 mm | primera plancha a 475.3 mm | corte a 484.8 mm del corte de los ejes
   corte de los ejes (-9305.5, -17195.8, 19933.0) | punto de corte (-9652.1, -17195.8, 19594.0) | extremo del lado del cordon: fin
   el punto del marco estaba a 0.000000 mm de la linea real de la diagonal; se usa el proyectado (-9652.1, -17195.8, 19594.0)
3) (a) CopyElement en el sitio + LocationCurve.Curve en los dos elementos
   CopyElement -> 1322482 (FamilyInstance)
   original (tramo corto): (-9652.1, -17195.8, 19594.0) -> (-9354.7, -17195.8, 19884.9) largo 416.1 mm
   original (tramo corto): dX 297.406341 | dY 0.000698 | dZ 290.969067 mm | en planta 0.000134 grados respecto a X | 0.000096 grados fuera del plano vertical XZ
   original (tramo corto): inicio (-9652.081581, -17195.835550, 19593.960443) fin (-9354.675240, -17195.834852, 19884.929510) mm
   original (tramo corto): extension en el corte puesta a 0
   original (tramo corto): DisallowJoinAtEnd(0) ok; IsJoinAllowedAtEnd = False
   copia (tramo largo): (-11856.5, -17195.8, 17437.3) -> (-9652.1, -17195.8, 19594.0) largo 3083.9 mm
   copia (tramo largo): dX 2204.394665 | dY 0.005173 | dZ 2156.681179 mm | en planta 0.000134 grados respecto a X | 0.000096 grados fuera del plano vertical XZ
   copia (tramo largo): inicio (-11856.476246, -17195.840724, 17437.279264) fin (-9652.081581, -17195.835550, 19593.960443) mm
   copia (tramo largo): extension en el corte puesta a 0
   copia (tramo largo): DisallowJoinAtEnd(1) ok; IsJoinAllowedAtEnd = False
   parametros de la copia distintos del original de antes (3): End Level Offset [53.239270 -> 52.284647], IfcGUID [2yvC09P_58MQYC5jkytRlK -> 235WiF$on62vAiJGNcBiiK], System Length [11.482940 -> 10.117883]
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
   node_info [1249631, 1322482]: ok=True kind=splice eccentricity_mm=0 chord=1322482 barras=[(1249631, 416.10000000000002, None), (1322482, 3083.9000000000001, None)]
   node_info [1245533, 1249631, 1249632]: ok=True kind=node eccentricity_mm=54.9 chord=1245533 barras=[(1245533, 10475, None), (1249631, 416.10000000000002, 484.80000000000001), (1249632, 3546, 483.19999999999999)]
6) deshacer: borrar la copia y devolver la curva al original
   original tras deshacer: (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9) largo 3500.0 mm (antes 3500.0); diferencia 0.00 mm
   original tras deshacer (el sondeo no restaura las uniones; el add-in 0.13.2 si): uniones automaticas permitidas: inicio=False fin=False
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

## 13c-3 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 79 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  0,
                 "splits":  [

                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  3
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 split

```text
== conn/split -> HTTP 200 en 624 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "next":  "conn_get_node_info con [1249632, 1322488] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.",
                 "split":  {
                               "split_distance_mm":  483.19999999999999,
                               "original_end_mm":  [
                                                       -6770.3999999999996,
                                                       -17195.799999999999,
                                                       17452.700000000001
                                                   ],
                               "new_element_id":  1322488,
                               "original_join_allowed":  [
                                                             false,
                                                             false
                                                         ],
                               "element_id":  1249632,
                               "original_start_feet":  [
                                                           -30.438972704533334,
                                                           -56.416780678368951,
                                                           65.486029733016920
                                                       ],
                               "mechanism":  "copy_element",
                               "approximated_parameters":  [

                                                           ],
                               "batch_id":  null,
                               "chord_element_id":  1245533,
                               "created_utc":  "2026-10-31T17:38:15.5580653Z",
                               "connection_id":  null,
                               "original_end_feet":  [
                                                         -22.212491925579190,
                                                         -56.416761369748379,
                                                         57.259548954040092
                                                     ],
                               "split_id":  "155ce03c-4733-4809-b711-a1ef85275396",
                               "split_point_mm":  [
                                                      -8909,
                                                      -17195.799999999999,
                                                      19591.299999999999
                                                  ],
                               "original_start_mm":  [
                                                         -9277.7999999999993,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                               "offset_from_chord_face_mm":  330
                           },
                 "preview":  {
                                 "split_distance_mm":  483.19999999999999,
                                 "cut_point_mm":  [
                                                      -9250.7000000000007,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                 "element_id":  1249632,
                                 "chord_element_id":  1245533,
                                 "theta_deg":  45,
                                 "plate_thickness_mm":  9.5250000000000004,
                                 "member_type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                 "gap_mm":  0,
                                 "chord_type":  "HSS12X8X1/2",
                                 "short_segment_mm":  521.60000000000002,
                                 "long_segment_mm":  3024.4000000000001,
                                 "face_distance_mm":  143.69999999999999,
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "offset_from_chord_face_mm":  330,
                                 "first_plate_distance_mm":  473.69999999999999
                             },
                 "description":  "La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cordón 1245533 a 45,0°: la cara del cordón queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo), Hecho: tramo nuevo 1322488."
             },
    "meta":  {
                 "operation":  "split",
                 "addin_version":  "0.13.2",
                 "duration_ms":  187
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 log revit_failure del corte

```text

{"ts":"2026-10-31T12:15:10.7052265-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322526","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear Especificación de arranque (Conectar)"}}
{"ts":"2026-10-31T12:17:50.5890906-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322532","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":false,"transaction":"MotorConexiones: Partir barra 1249632"}}
{"ts":"2026-10-31T12:38:15.6773642-05:00","record":{"event":"revit_failure","severity":"Warning","text":"Beam or Brace 
is slightly off axis and may cause inaccuracies.","elements":"1322488","definition":"7cba70c0-2107-421e-9fe6-d2a2e05ead
62","filtered":true,"transaction":"MotorConexiones: Partir barra 1249632"}}



```

## 13c-4 registro del corte

```text
== conn/unsplit -> HTTP 200 en 22 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  1,
                 "splits":  [
                                {
                                    "split_distance_mm":  483.19999999999999,
                                    "original_end_mm":  [
                                                            -6770.3999999999996,
                                                            -17195.799999999999,
                                                            17452.700000000001
                                                        ],
                                    "new_element_id":  1322488,
                                    "original_join_allowed":  [
                                                                  false,
                                                                  false
                                                              ],
                                    "element_id":  1249632,
                                    "original_start_feet":  [
                                                                -30.438972704533334,
                                                                -56.416780678368951,
                                                                65.486029733016920
                                                            ],
                                    "mechanism":  "copy_element",
                                    "approximated_parameters":  [

                                                                ],
                                    "batch_id":  null,
                                    "chord_element_id":  1245533,
                                    "created_utc":  "2026-10-31T17:38:15.5580653Z",
                                    "connection_id":  null,
                                    "original_end_feet":  [
                                                              -22.212491925579190,
                                                              -56.416761369748379,
                                                              57.259548954040092
                                                          ],
                                    "split_id":  "155ce03c-4733-4809-b711-a1ef85275396",
                                    "split_point_mm":  [
                                                           -8909,
                                                           -17195.799999999999,
                                                           19591.299999999999
                                                       ],
                                    "original_start_mm":  [
                                                              -9277.7999999999993,
                                                              -17195.799999999999,
                                                              19960.099999999999
                                                          ],
                                    "offset_from_chord_face_mm":  330
                                }
                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  9
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 node_info tramos

```text
== conn/node_info -> HTTP 200 en 414 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "x_axis":  [
                                0.70710700000000004,
                                2E-06,
                                -0.70710700000000004
                            ],
                 "members":  [
                                 {
                                     "length_mm":  521.60000000000002,
                                     "angle_in_plane_deg":  -180,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -8909,
                                                         -17195.799999999999,
                                                         19591.299999999999
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "slope_deg":  45,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  0,
                                     "angle_to_chord_deg":  0,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  null,
                                     "width_mm":  63.5,
                                     "split_point_mm":  null,
                                     "split_offset_from_chord_face_mm":  null,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  1,
                                     "element_id":  1249632,
                                     "end_mm":  [
                                                    -8909,
                                                    -17195.799999999999,
                                                    19591.299999999999
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 },
                                 {
                                     "length_mm":  3024.4000000000001,
                                     "angle_in_plane_deg":  0,
                                     "wall_mm":  0,
                                     "is_chord":  true,
                                     "axis_cut_mm":  [
                                                         -8909,
                                                         -17195.799999999999,
                                                         19591.299999999999
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -8909,
                                                      -17195.799999999999,
                                                      19591.299999999999
                                                  ],
                                     "slope_deg":  45,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  0,
                                     "angle_to_chord_deg":  0,
                                     "side":  "chord",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  null,
                                     "width_mm":  63.5,
                                     "split_point_mm":  null,
                                     "split_offset_from_chord_face_mm":  null,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  0,
                                     "element_id":  1322488,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 }
                             ],
                 "kind":  "splice",
                 "y_axis":  [
                                0.70710700000000004,
                                2E-06,
                                0.70710700000000004
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "chord_element_id":  1322488,
                 "chord_direction_reversed":  false,
                 "existing_connections":  [

                                          ],
                 "z_axis":  [
                                2E-06,
                                -1,
                                0
                            ],
                 "origin_mm":  [
                                   -8909,
                                   -17195.799999999999,
                                   19591.299999999999
                               ],
                 "eccentricity_mm":  0,
                 "axis_distance_mm":  0
             },
    "meta":  {
                 "operation":  "node_info",
                 "addin_version":  "0.13.2",
                 "duration_ms":  6
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 unsplit

```text
== conn/unsplit -> HTTP 200 en 79 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "unsplit":  {
                                 "split_distance_mm":  483.19999999999999,
                                 "original_end_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ],
                                 "new_element_id":  1322488,
                                 "original_join_allowed":  [
                                                               false,
                                                               false
                                                           ],
                                 "element_id":  1249632,
                                 "original_start_feet":  [
                                                             -30.438972704533334,
                                                             -56.416780678368951,
                                                             65.486029733016920
                                                         ],
                                 "mechanism":  "copy_element",
                                 "approximated_parameters":  [

                                                             ],
                                 "batch_id":  null,
                                 "chord_element_id":  1245533,
                                 "created_utc":  "2026-10-31T17:38:15.5580653Z",
                                 "connection_id":  null,
                                 "original_end_feet":  [
                                                           -22.212491925579190,
                                                           -56.416761369748379,
                                                           57.259548954040092
                                                       ],
                                 "split_id":  "155ce03c-4733-4809-b711-a1ef85275396",
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "original_start_mm":  [
                                                           -9277.7999999999993,
                                                           -17195.799999999999,
                                                           19960.099999999999
                                                       ],
                                 "offset_from_chord_face_mm":  330
                             },
                 "description":  "Corte deshecho: el tramo 1322488 se borró y la barra 1249632 recuperó su curva.",
                 "complete":  true
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  63
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 34 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  0,
                 "splits":  [

                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  3
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 node_info nudo K tras deshacer

```text
== conn/node_info -> HTTP 200 en 23 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "members":  [
                                 {
                                     "length_mm":  10475,
                                     "angle_in_plane_deg":  -180,
                                     "wall_mm":  11.810000000000000,
                                     "is_chord":  true,
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "slope_deg":  0,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  0,
                                     "angle_to_chord_deg":  0,
                                     "side":  "chord",
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "split_distance_mm":  null,
                                     "width_mm":  203.19999999999999,
                                     "split_point_mm":  null,
                                     "split_offset_from_chord_face_mm":  null,
                                     "type":  "HSS12X8X1/2",
                                     "node_end":  0,
                                     "element_id":  1245533,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  90,
                                     "height_mm":  304.80000000000001
                                 },
                                 {
                                     "length_mm":  3500,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "slope_deg":  44.369999999999997,
                                     "end_gap_mm":  90.400000000000006,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  -27.399999999999999,
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  484.80000000000001,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -9652.1000000000004,
                                                            -17195.700000000001,
                                                            19594
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  1,
                                     "element_id":  1249631,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 },
                                 {
                                     "length_mm":  3546,
                                     "angle_in_plane_deg":  -45,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "slope_deg":  45,
                                     "end_gap_mm":  27.100000000000001,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  27.399999999999999,
                                     "angle_to_chord_deg":  45,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  483.19999999999999,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -8909,
                                                            -17195.700000000001,
                                                            19591.299999999999
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  0,
                                     "element_id":  1249632,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 }
                             ],
                 "kind":  "node",
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "chord_element_id":  1245533,
                 "chord_direction_reversed":  true,
                 "existing_connections":  [

                                          ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "eccentricity_mm":  54.899999999999999,
                 "axis_distance_mm":  0.089999999999999997
             },
    "meta":  {
                 "operation":  "node_info",
                 "addin_version":  "0.13.2",
                 "duration_ms":  12
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-4 log cortes

```text

{"ts":"2026-10-31T12:17:50.7562184-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
532,"split_id":"c4bf59ea-f64b-47f1-b804-13f2128d72e0","complete":true}}
{"ts":"2026-10-31T12:38:15.6044851-05:00","record":{"event":"member_split","element_id":1249632,"new_element_id":132248
8,"mechanism":"copy_element","split_point_mm":[-8908.9765873183,-17195.789149833254,19591.319569910534],"split_distance
_mm":483.209,"short_mm":521.6,"long_mm":3024.4,"approximated":[]}}
{"ts":"2026-10-31T12:38:29.7780148-05:00","record":{"event":"member_unsplit","element_id":1249632,"new_element_id":1322
488,"problems":[]}}
{"ts":"2026-10-31T12:38:29.8240345-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
488,"split_id":"155ce03c-4733-4809-b711-a1ef85275396","complete":true}}



```

## 13c-5 list

```text
== conn/list -> HTTP 200 en 414 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "total_count":  3,
                 "batch_id":  null,
                 "batches":  {

                             },
                 "connections_count":  3,
                 "connections":  [
                                     {
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-31T17:39:39.2444932Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "created_elements_count":  8,
                                         "connection_type":  "end_plate",
                                         "connection_id":  "f4717f01-d1c8-461b-9394-c40c3c260115",
                                         "template_id":  null
                                     },
                                     {
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-31T17:39:39.5847069Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "created_elements_count":  8,
                                         "connection_type":  "end_plate",
                                         "connection_id":  "31d0b406-52fd-4183-b95c-cc56140e5c5d",
                                         "template_id":  null
                                     },
                                     {
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-31T17:39:39.7231158Z",
                                         "backend":  "advancesteel",
                                         "batch_id":  null,
                                         "created_elements_count":  2,
                                         "connection_type":  "direct_weld",
                                         "connection_id":  "62e5c502-b059-4b5e-b1c0-c16735bd6fd9",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4"
                                     }
                                 ]
             },
    "meta":  {
                 "operation":  "list",
                 "addin_version":  "0.13.2",
                 "duration_ms":  4
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-5 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 20 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  2,
                 "splits":  [
                                {
                                    "split_distance_mm":  484.80000000000001,
                                    "original_end_mm":  [
                                                            -9354.7000000000007,
                                                            -17195.799999999999,
                                                            19884.900000000001
                                                        ],
                                    "new_element_id":  1322506,
                                    "original_join_allowed":  [
                                                                  false,
                                                                  false
                                                              ],
                                    "element_id":  1249631,
                                    "original_start_feet":  [
                                                                -38.899200280803477,
                                                                -56.416800274500808,
                                                                57.208921471030742
                                                            ],
                                    "mechanism":  "copy_element",
                                    "approximated_parameters":  [

                                                                ],
                                    "batch_id":  null,
                                    "chord_element_id":  1245533,
                                    "created_utc":  "2026-10-31T17:39:38.5022632Z",
                                    "connection_id":  "62e5c502-b059-4b5e-b1c0-c16735bd6fd9",
                                    "original_end_feet":  [
                                                              -30.691191730840792,
                                                              -56.416781011659708,
                                                              65.239270046781542
                                                          ],
                                    "split_id":  "346924b4-c046-4de5-b01f-efe6fe335d47",
                                    "split_point_mm":  [
                                                           -9652.1000000000004,
                                                           -17195.799999999999,
                                                           19594
                                                       ],
                                    "original_start_mm":  [
                                                              -11856.5,
                                                              -17195.799999999999,
                                                              17437.299999999999
                                                          ],
                                    "offset_from_chord_face_mm":  330
                                },
                                {
                                    "split_distance_mm":  483.19999999999999,
                                    "original_end_mm":  [
                                                            -6770.3999999999996,
                                                            -17195.799999999999,
                                                            17452.700000000001
                                                        ],
                                    "new_element_id":  1322523,
                                    "original_join_allowed":  [
                                                                  false,
                                                                  false
                                                              ],
                                    "element_id":  1249632,
                                    "original_start_feet":  [
                                                                -30.438972704533338,
                                                                -56.416780678368951,
                                                                65.486029733016920
                                                            ],
                                    "mechanism":  "copy_element",
                                    "approximated_parameters":  [

                                                                ],
                                    "batch_id":  null,
                                    "chord_element_id":  1245533,
                                    "created_utc":  "2026-10-31T17:39:39.4491422Z",
                                    "connection_id":  "62e5c502-b059-4b5e-b1c0-c16735bd6fd9",
                                    "original_end_feet":  [
                                                              -22.212491925579190,
                                                              -56.416761369748379,
                                                              57.259548954040092
                                                          ],
                                    "split_id":  "f840ca2e-c61f-41a9-8f74-54c1fd00e202",
                                    "split_point_mm":  [
                                                           -8909,
                                                           -17195.799999999999,
                                                           19591.299999999999
                                                       ],
                                    "original_start_mm":  [
                                                              -9277.7999999999993,
                                                              -17195.799999999999,
                                                              19960.099999999999
                                                          ],
                                    "offset_from_chord_face_mm":  330
                                }
                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  3
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-5 delete empalme B 1

```text
== conn/delete -> HTTP 200 en 611 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "restored_members_count":  2,
                 "deleted_elements_count":  8,
                 "deleted_connection_id":  "f4717f01-d1c8-461b-9394-c40c3c260115"
             },
    "meta":  {
                 "operation":  "delete",
                 "addin_version":  "0.13.2",
                 "duration_ms":  188
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-5 delete empalme B 2

```text
== conn/delete -> HTTP 200 en 1076 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "restored_members_count":  2,
                 "deleted_elements_count":  8,
                 "deleted_connection_id":  "31d0b406-52fd-4183-b95c-cc56140e5c5d"
             },
    "meta":  {
                 "operation":  "delete",
                 "addin_version":  "0.13.2",
                 "duration_ms":  124
             },
    "ok":  true,
    "warnings":  [
                     {
                         "code":  "REVIT_WARNING",
                         "path":  null,
                         "message":  "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Los elementos se borran directamente (Document.Delete) y las barras se restauran igual.",
                         "hint":  null
                     }
                 ]
}

```

## 13c-5 delete nudo K (deshace los cortes)

```text
== conn/delete -> HTTP 200 en 307 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "restored_members_count":  2,
                 "deleted_elements_count":  2,
                 "deleted_connection_id":  "62e5c502-b059-4b5e-b1c0-c16735bd6fd9"
             },
    "meta":  {
                 "operation":  "delete",
                 "addin_version":  "0.13.2",
                 "duration_ms":  116
             },
    "ok":  true,
    "warnings":  [
                     {
                         "code":  "REVIT_WARNING",
                         "path":  null,
                         "message":  "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Los elementos se borran directamente (Document.Delete) y las barras se restauran igual.",
                         "hint":  null
                     }
                 ]
}

```

## 13c-5 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 288 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "splits_count":  0,
                 "splits":  [

                            ]
             },
    "meta":  {
                 "operation":  "unsplit",
                 "addin_version":  "0.13.2",
                 "duration_ms":  4
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-5 node_info nudo K final

```text
== conn/node_info -> HTTP 200 en 89 ms ==
{
    "errors":  [

               ],
    "data":  {
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "members":  [
                                 {
                                     "length_mm":  10475,
                                     "angle_in_plane_deg":  -180,
                                     "wall_mm":  11.810000000000000,
                                     "is_chord":  true,
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "slope_deg":  0,
                                     "end_gap_mm":  0,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  0,
                                     "angle_to_chord_deg":  0,
                                     "side":  "chord",
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "split_distance_mm":  null,
                                     "width_mm":  203.19999999999999,
                                     "split_point_mm":  null,
                                     "split_offset_from_chord_face_mm":  null,
                                     "type":  "HSS12X8X1/2",
                                     "node_end":  0,
                                     "element_id":  1245533,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  90,
                                     "height_mm":  304.80000000000001
                                 },
                                 {
                                     "length_mm":  3500,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "slope_deg":  44.369999999999997,
                                     "end_gap_mm":  90.400000000000006,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  -27.399999999999999,
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  484.80000000000001,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -9652.1000000000004,
                                                            -17195.700000000001,
                                                            19594
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  1,
                                     "element_id":  1249631,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 },
                                 {
                                     "length_mm":  3546,
                                     "angle_in_plane_deg":  -45,
                                     "wall_mm":  0,
                                     "is_chord":  false,
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "slope_deg":  45,
                                     "end_gap_mm":  27.100000000000001,
                                     "structural_type":  "Beam",
                                     "axis_offset_mm":  27.399999999999999,
                                     "angle_to_chord_deg":  45,
                                     "side":  "-Y",
                                     "material":  "Material IFC (190-40-140)",
                                     "split_distance_mm":  483.19999999999999,
                                     "width_mm":  63.5,
                                     "split_point_mm":  [
                                                            -8909,
                                                            -17195.700000000001,
                                                            19591.299999999999
                                                        ],
                                     "split_offset_from_chord_face_mm":  330,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "node_end":  0,
                                     "element_id":  1249632,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "reaches_node":  true,
                                     "section_rotation_deg":  -180,
                                     "height_mm":  63.5
                                 }
                             ],
                 "kind":  "node",
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "chord_element_id":  1245533,
                 "chord_direction_reversed":  true,
                 "existing_connections":  [

                                          ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "eccentricity_mm":  54.899999999999999,
                 "axis_distance_mm":  0.089999999999999997
             },
    "meta":  {
                 "operation":  "node_info",
                 "addin_version":  "0.13.2",
                 "duration_ms":  9
             },
    "ok":  true,
    "warnings":  [

                 ]
}

```

## 13c-5 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 615 ms ==
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

## 13c-6 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         14424



```

## 13c-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.13.2 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"errors":[],"data":{"has_uidocument":true,"spec_version":"1.0","backend":"advancesteel","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","split","types","unsplit","update","validate"],"addin_version":"0.13.2","document":{"is_family":false,"is_modifiable":false,"is_workshared":false,"title":"HANGAR_PRUEBA_sondeo","path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_read_only":false},"revit":{"sub_version_number":"2027.2","version_build":"27.2.0.39","language":"English_USA","version_name":"Autodesk Revit 2027","version_number":"2027"},"dotnet":{"load_context":"Default","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12"}},"meta":{"operation":"ping","addin_version":"0.13.2","duration_ms":3},"ok":true,"warnings":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 37015 caracteres
Cuerpo:
{"errors":[],"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_ ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"errors":[],"data":{"connection_types":[{"description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n.","type_name":"direct_weld"},{"description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).","type_name":"end_plate"},{"description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","type_name":"gusset_node"}]},"meta":{"operation":"types","addin_version":"0.13.2","duration_ms":1},"ok":true,"warnings":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"errors":[],"data":{"json_schema":{"type":"object","additionalProperties":false,"$schema":"http://json-schema.org/draft-07/schema#","title":"GussetNodeConnectionSpec","required":["spec_version","connection_type","node","chord","gusset","members"],"properties":{"members":{"type":"array","items":{"type":"object","additionalProperties":false,"properties":{"role":{"type":"string","enum":["diagonal","vertical","chord"]},"expected_angle_deg":{"type":"number"},"element_id":{"type":"integer"},"axis_offset_mm":{"type":["number","null"],"description":"Fase 12: d\u00f3nde corta el eje de la barra al del cord\u00f3n, medido desde el punto de trabajo a lo largo de +X (con signo; 0 si concurre). Informativo: el validador lo mide del modelo y avisa MEMBER_ECCENTRIC hasta medio canto del cord\u00f3n."},"attachment":{"type":"object","additionalProperties":false,"properties":{"weld_plate_to_member":{"type":"object","additionalProperties":false,"properties":{"type":{"type":"string","enum":["fillet"]},"size_mm":{"type":"number","minimum":1.0},"all_around":{"type":"boolean"}},"required":["type","size_mm"]},"type":{"type":"string","enum":["welded_slot","bolted_knife_plate"]},"weld":{"type":"object","additionalProperties":false,"properties":{"type":{"type":"string","enum":["fillet"]},"size_mm":{"type":"number","minimum":1.0},"all_around":{"type":"boolean"}},"required":["type","size_mm"]},"bolts":{"type":"object","additionalProperties":false,"properties":{"rows":{"type":"integer","minimum":1},"colu ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"code":"UNKNOWN_OPERATION","path":"type","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node"}],"data":null,"meta":{"operation":"schema","addin_version":"0.13.2","duration_ms":0},"ok":false,"warnings":[]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"errors": [], "data": {"query": "HSS2-1/2X2-1/2X3/16", "matched_count": 1, "matches": [{"family_name": "HSS2-1-2X2-1-2X3-16 64x64", "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}], "total_profiles_in_model": 29, "suggestions": []}, "meta": {"operation": "find_profile", "addin_version": "0.13.2", "duration_ms": 16}, "ok": true, "warnings": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"errors": [], "data": {"x_axis": [1, 1.9999999999999999e-06, 0], "members": [{"length_mm": 9960.2999999999993, "angle_in_plane_deg": 0, "wall_mm": 5.9199999999999999, "is_chord": true, "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "family": "HSS-Hollow Structural Section", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "slope_deg": 0, "end_gap_mm": 0, "structural_type": "Beam", "axis_offset_mm": 0, "angle_to_chord_deg": 0, "side": "chord", "material": "Steel ASTM A500, Grade B, Rectangular and Square", "split_distance_mm": null, "width_mm": 76.200000000000003, "split_point_mm": null, "split_offset_from_chord_face_mm": null, "type": "HSS3X3X1/4", "node_end": 1, "element_id": 1249510, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "reaches_node": true, "section_rotation_deg": 180, "height_mm": 76.200000000000003}, {"length_mm": 3568, "angle_in_plane_deg": 136.90000000000001, "wall_mm": 0, "is_chord": false, "axis_cut_mm": [-11867.700000000001, -17195.799999999999, 17423], "family": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "slope_deg": 43.079999999999998, "end_gap_mm": 84.900000000000006, "structural_type": "Beam", "axis_offset_mm": 1.7000000000000000, "angle_to_chord_deg": 43.100000000000001, "side": "+Y", "material": "Material IFC (190-40-140)", "split_distance_mm": 395.30000000000001, "width_mm": 63.5, "split_point_mm": [-12156.400000000000, -17195.799999999999,  ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"errors":[],"data":{"calculated_values":{"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"kind":"node","outline_mode":null,"outline_vertices":null,"outline_height_mm":null,"outline_width_mm":null,"frame_z":[0,-1,0],"chord_direction_reversed":true,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_x":[1,0,0],"eccentricity_mm":3.3999999999999999,"outline_points_mm":null},"bolt_stacks":[{"gusset_face":"+z","bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip","grip_mm":19.524999999999999,"member_element_id":1249636}],"warnings_count":1,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","errors_count":0,"is_valid":true},"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":50},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","path":"dimension_chains[0].values_mm","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm."}],"data":null,"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":8},"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[0].user_confirmed_value","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."},{"code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[1].user_confirmed_value","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."}],"data":null,"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":10},"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"first_member_element_id": 1249630, "dry_run": true, "working_point_mm": [-11869.4, -17195.8, 17423], "bolts": 4, "backend": "advancesteel", "gusset_plates": 1, "chord_element_id": 1249510, "knife_plates": 1, "connection_type": "gusset_node", "weld_lines": 6, "members_modified": 3}
Cuerpo:
{"errors": [], "data": {"summary": {"first_member_element_id": 1249630, "dry_run": true, "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "bolts": 4, "backend": "advancesteel", "gusset_plates": 1, "chord_element_id": 1249510, "knife_plates": 1, "connection_type": "gusset_node", "weld_lines": 6, "members_modified": 3}, "members_to_modify": [{"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -95.099999999999994, "role": "diagonal", "setback_mm": 180, "element_id": 1249630, "end": "end", "current_end_distance_mm": 84.900000000000006, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -40.799999999999997, "role": "diagonal", "setback_mm": 60, "element_id": 1249631, "end": "start", "current_end_distance_mm": 19.199999999999999, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -211.40000000000001, "role": "diagonal", "setback_mm": 260, "element_id": 1249636, "end": "end", "current_end_distance_mm": 48.600000000000001, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}], "calculated_values": {"axis_distance_mm": 0.080000000000000002, "frame_y": [0, 0, 1], "kind": "node", "outline_mode": null, "outline_vertices": null, "outline_height_mm": null, "outline_width_mm": null, "frame_z": [0, - ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"errors":[{"code":"VALIDATION_TOKEN_INVALID","path":"validation_token","message":"validation_token es obligatorio para crear una conexi\u00f3n.","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token."}],"data":null,"meta":{"operation":"create","addin_version":"0.13.2","duration_ms":0},"ok":false,"warnings":[]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"errors": [], "data": {"total_count": 0, "batch_id": null, "batches": {}, "connections_count": 0, "connections": []}, "meta": {"operation": "list", "addin_version": "0.13.2", "duration_ms": 4}, "ok": true, "warnings": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"code":"ELEMENT_NOT_FOUND","path":"connection_id","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo."}],"data":null,"meta":{"operation":"get","addin_version":"0.13.2","duration_ms":13},"ok":false,"warnings":[]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"code":"ELEMENT_NOT_FOUND","path":"connection_id","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Verifica los IDs disponibles con conn_list."}],"data":null,"meta":{"operation":"delete","addin_version":"0.13.2","duration_ms":4},"ok":false,"warnings":[]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"code":"UNKNOWN_OPERATION","path":null,"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, split, types, unsplit, update, validate."}],"data":null,"meta":{"operation":"no_existe","addin_version":"0.13.2","duration_ms":0},"ok":false,"warnings":[]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"errors":[],"data":{"templates_count":2,"templates":[{"tags":[],"name":"Nudo K cordon superior","members_count":2,"created_utc":"2026-10-21T16:49:07.9104459Z","template_id":"91adac27-d196-4c98-88db-6579aa991be4","connection_type":"direct_weld","description":null,"pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","chord_profile":"HSS12X8X1/2","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","origin_document":"HANGAR_PRUEBA_sondeo","origin_drawing":"Especificaci\u00f3n de arranque (Conectar)"},{"tags":["hangar","cercha","HSS"],"name":"Nudo tipico Detalle D","members_count":3,"created_utc":"2026-10-04T22:01:50.5585233Z","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","connection_type":"gusset_node","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","chord_profile":"HSS3X3X1/4","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","origin_document":"HANGAR_PRUEBA_sondeo","origin_drawing":"Detalle D"}],"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo"},"meta":{"operation":"catalog_list","addin_version":"0.13.2","dura ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=494c02aa-cf7a-46fb-b8a5-4d44333f32d1 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\494c02aa-cf7a-46fb-b8a5-4d44333f32d1.json
Cuerpo:
{"errors":[],"data":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"origin":{"element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo","drawing":"Detalle D","connection_id":null},"name":"PRUEBA probar_conexiones","shared_file":null,"members_count":3,"template_id":"494c02aa-cf7a-46fb-b8a5-4d44333f32d1","member_pattern":[{"role":"diagonal","angle_deg":136.91999999999999,"profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","side":"+Y","slot":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","angle_deg":44.369999999999997,"profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","side":"+Y","slot":1,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","angle_deg":-135.63000000000000,"profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","side":"-Y","slot":2,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"}],"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\494c02aa-cf7a-46fb-b8a5-4d44333f32d1.json","chord_profile":"HSS3X3X1/4"},"meta":{"operation":"catalog_save","addin_version":"0.13.2","duration_ms":31},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"errors":[],"data":{"name":"PRUEBA probar_conexiones","template_id":"494c02aa-cf7a-46fb-b8a5-4d44333f32d1","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\494c02aa-cf7a-46fb-b8a5-4d44333f32d1.json","template":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"tags":["prueba"],"origin":{"element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo","drawing":"Detalle D","connection_id":null},"name":"PRUEBA probar_conexiones","spec_template":{"members":[{"role":"diagonal","expected_angle_deg":45.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"end_setback_mm":180.0,"profile":"HSS2-1/2X2-1/2X3/16","slot":0},{"role":"diagonal","expected_angle_deg":45.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"end_setback_mm":60.0,"profile":"HSS2-1/2X2-1/2X3/16","slot":1},{"role":"diagonal","expected_angle_deg":45.0,"attachment":{"type":"bolted_knife_plate","bolts":{"rows":2,"columns":2,"diameter_label":"5/8\"","diameter_mm":15.875,"first_row_from_plate_end_mm":40.0,"edge_mm":40.0,"spacing_mm":60.0},"plate":{"length_mm":170.0,"thickness_mm":10.0,"width_mm":140.0,"thickness_label":"PL10","insertion_mm":80.0},"weld_plate_to_member":{"type":"fillet","size_mm":5.0,"all_around":true}}, ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=583d8561b645...
Cuerpo:
{"errors":[],"data":{"calculated_values":{"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"kind":"node","outline_mode":null,"outline_vertices":null,"outline_height_mm":null,"outline_width_mm":null,"frame_z":[0,-1,0],"chord_direction_reversed":true,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_x":[1,0,0],"eccentricity_mm":3.3999999999999999,"outline_points_mm":null},"name":"PRUEBA probar_conexiones","bolt_stacks":[{"gusset_face":"+z","bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip","grip_mm":19.524999999999999,"member_element_id":1249636}],"proposal":null,"template_id":"494c02aa-cf7a-46fb-b8a5-4d44333f32d1","match":{"assignments":[{"role":"diagonal","element_id":1249630,"template_angle_deg":136.91999999999999,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":136.91999999999999,"slot":0,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","element_id":1249631,"template_angle_deg":44.369999999999997,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":44.369999999999997,"slot":1,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","element_id":1249636,"template_angle_deg":-135.63000000000000,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":-135.63000000000000,"slot":2,"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"}], ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"errors":[],"data":{"calculated_values":{"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"kind":"node","outline_mode":null,"outline_vertices":null,"outline_height_mm":null,"outline_width_mm":null,"frame_z":[0,-1,0],"chord_direction_reversed":true,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_x":[1,0,0],"eccentricity_mm":3.3999999999999999,"outline_points_mm":null},"name":"PRUEBA probar_conexiones","bolt_stacks":[{"gusset_face":"+z","bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip","grip_mm":19.524999999999999,"member_element_id":1249636}],"proposal":{"attempts":["Nudo K cordon superior \u2192 same: ranura 0 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal -45.0\u00b0) sin barra; barras sobrantes: 1249630, 1249631","Nudo K cordon superior \u2192 mirror_x: ranura 0 (diagonal -44.4\u00b0) sin barra; ranura 1 (diagonal -135.0\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.6\u00b0); barras sobrantes: 1249630, 1249631","Nudo K cordon superior \u2192 mirror_y: ranura 0 (diagonal 135.6\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 1.3\u00b0); ranura 1 (diagonal 45.0\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.6\u00b0); barras sobrantes: 1249636","Nudo K cordon superior \u2192 both: ranura 0 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 135.0\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 1 ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d nudos=6 resumen={'untyped': 5, 'ready': 1} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"errors":[],"data":{"templates":{"494c02aa-cf7a-46fb-b8a5-4d44333f32d1":"PRUEBA probar_conexiones"},"has_batch_connections":false,"document":"HANGAR_PRUEBA_sondeo","labels":{"view_id":null,"count":0},"nodes":[{"color_rgb":null,"max_deviation_deg":null,"attempts":[],"warnings_count":0,"validation_token":null,"kind":"node","split_steps":[],"label_index":null,"match":null,"created_connection_id":null,"starter":false,"replaces_existing":false,"marker_element_id":null,"signature":"1 barra(s) sin marco","advice":"No es un nudo: nada que hacer","split_of_node":null,"is_valid":false,"template_name":null,"orientation":null,"visible_by_default":false,"eccentricity_mm":0,"is_mirrored":false,"chord_continuous":false,"actions":[],"chord_type_name":null,"name":"N1","color_name":null,"errors":[],"chord_element_id":0,"ghost_element_id":null,"through_element_ids":[],"errors_count":0,"has_spec_override":false,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"member_element_ids":[1249630],"status_text":"\u25cb Barra suelta (no es nudo)","template_id":null,"spec":null,"splice_element_id":null,"members":[],"status":"untyped","warnings":[],"element_ids":[1249630],"existing_connection_id":null,"is_manual":false,"connection_type":null,"existing_batch_id":null,"is_marked":false},{"color_rgb":null,"max_deviation_deg":null,"attempts":[],"warnings_count":0,"validation_token":null,"kind":"no ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=f9623089a973...
Cuerpo:
{"errors":[],"data":{"plan_id":"ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d","node":{"color_rgb":[46,160,67],"max_deviation_deg":0,"attempts":[],"warnings_count":0,"validation_token":"f9623089a9736e57205121b0f76fa3cb2ae1773eb9bb831b93cafbc1b1efd1bc","kind":"node","split_steps":[],"label_index":null,"match":{"assignments":[{"role":"diagonal","element_id":1249630,"template_angle_deg":136.91999999999999,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":136.91999999999999,"slot":0,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","element_id":1249631,"template_angle_deg":44.369999999999997,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":44.369999999999997,"slot":1,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"role":"diagonal","element_id":1249636,"template_angle_deg":-135.63000000000000,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","model_angle_deg":-135.63000000000000,"slot":2,"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"}],"matched_count":3,"is_complete":true,"score_deg":0.01,"orientation":"same","unassigned_members":[],"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u0 ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote ab4b620d: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"errors":[],"data":{"duration_ms":4,"deleted_elements_count":0,"deleted_count":0,"operation":"batch_create","with_warnings_count":0,"document":"HANGAR_PRUEBA_sondeo","split_count":0,"started_utc":"2026-10-31T17:42:09.1031187Z","nodes":[{"kind":"node","new_element_id":null,"errors":[{"code":"VALIDATION_TOKEN_INVALID","path":"nodes[N4].validation_token","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4."}],"element_id":null,"duration_ms":0,"outcome":"failed","connection_id":null,"orientation":"same","restored_members_count":0,"elements_count":0,"split_id":null,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","node":"N4","template_name":"PRUEBA probar_conexiones","spec":null,"warnings":[],"reason":"token distinto del plan"}],"updated_count":0,"unsplit_count":0,"plan_id":"ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d","failed_count":1,"finished_utc":"2026-10-31T17:42:09.1059076Z","summary_text":"Lote ab4b620d: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","batch_id":"ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d","stopped_at":null,"undo_entries":"one","plan_summary":{"untyped":5,"fail ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"errors":[],"data":{"duration_ms":8,"deleted_elements_count":0,"deleted_count":0,"operation":"batch_delete","with_warnings_count":0,"document":"HANGAR_PRUEBA_sondeo","split_count":0,"started_utc":"2026-10-31T17:42:09.1381514Z","nodes":[],"updated_count":0,"unsplit_count":0,"plan_id":null,"failed_count":0,"finished_utc":"2026-10-31T17:42:09.1463779Z","summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","batch_id":"00000000-0000-0000-0000-000000000000","stopped_at":null,"undo_entries":"one","plan_summary":null,"stopped":false,"plan_summary_text":null,"stop_on_error":false,"connection_ids":[],"skipped_count":0,"rolled_back_count":0,"created_count":0,"plan_creatable_count":null,"restored_members_count":0},"meta":{"operation":"batch_delete","addin_version":"0.13.2","duration_ms":9},"ok":true,"warnings":[{"code":"BATCH_EMPTY","path":"batch_id","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote)."}]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d
Cuerpo:
{"errors": [], "data": {"remaining_labels": 0, "discarded_plan_id": "ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d", "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0, "remaining_plans": 0}, "meta": {"operation": "batch_plan_discard", "addin_version": "0.13.2", "duration_ms": 28}, "ok": true, "warnings": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"errors":[{"code":"TEMPLATE_NOT_FOUND","path":"template_id","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","hint":"Usa conn_catalog_list para ver las plantillas disponibles."}],"data":null,"meta":{"operation":"catalog_apply","addin_version":"0.13.2","duration_ms":0},"ok":false,"warnings":[]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 494c02aa-cf7a-46fb-b8a5-4d44333f32d1
Cuerpo:
{"errors":[],"data":{"deleted_template_id":"494c02aa-cf7a-46fb-b8a5-4d44333f32d1","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\494c02aa-cf7a-46fb-b8a5-4d44333f32d1.json"},"meta":{"operation":"catalog_delete","addin_version":"0.13.2","duration_ms":2},"ok":true,"warnings":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"errors":[],"data":{"templates":{"6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D","91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior"},"has_batch_connections":false,"document":"HANGAR_PRUEBA_sondeo","labels":{"view_id":null,"count":0},"nodes":[{"color_rgb":null,"max_deviation_deg":null,"attempts":["Nudo K cordon superior \u2192 same: ranura 0 (diagonal -135.6\u00b0) sin barra; ranura 1 (diagonal -45.0\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.9\u00b0)","Nudo K cordon superior \u2192 mirror_x: ranura 0 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0); ranura 1 (diagonal -135.0\u00b0) sin barra","Nudo K cordon superior \u2192 mirror_y: ranura 0 (diagonal 135.6\u00b0) sin barra; ranura 1 (diagonal 45.0\u00b0) sin barra; barras sobrantes: 1249630","Nudo K cordon superior \u2192 both: ranura 0 (diagonal 44.4\u00b0) sin barra; ranura 1 (diagonal 135.0\u00b0) sin barra; barras sobrantes: 1249630","Nudo tipico Detalle D \u2192 same: ranura 0 (diagonal 136.9\u00b0) sin barra; ranura 1 (diagonal 44.4\u00b0) sin barra; ranura 2 (diagonal -135.6\u00b0) sin barra; barras sobrantes: 1249630","Nudo tipico Detalle D \u2192 mirror_x: ranura 0 (diagonal 43.1\u00b0) sin barra; ranura 1 (diagonal 135.6\u00b0) sin barra; ranura 2 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0)","Nudo tipico Detalle D \u2192 mirror_y: ranura 0 (diagonal -136.9\u00b0) sin barra; ranura 1 (diagonal -44.4\u00b0)  ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado d03f6673-6d1a-4ea7-90b3-2b4f3d9ed9a9 etiquetas quitadas=0
Cuerpo:
{"errors": [], "data": {"remaining_labels": 0, "discarded_plan_id": "d03f6673-6d1a-4ea7-90b3-2b4f3d9ed9a9", "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0, "remaining_plans": 0}, "meta": {"operation": "batch_plan_discard", "addin_version": "0.13.2", "duration_ms": 11}, "ok": true, "warnings": []}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"errors": [], "data": {"calculated_values": {"axis_distance_mm": 0, "frame_y": [0, 0, 1], "kind": "splice", "outline_mode": null, "outline_vertices": null, "outline_height_mm": null, "outline_width_mm": null, "frame_z": [0, -1, 0], "chord_direction_reversed": true, "origin_mm": [-4180.5, 17204.200000000001, 19933], "frame_x": [1, 0, 0], "eccentricity_mm": 0, "outline_points_mm": null}, "bolt_stacks": [{"bolt_length_mm": 50.799999999999997, "bolts": 12, "length_source": "computed_from_grip", "plates": 2, "gap_mm": 0, "member_element_id": 1245531, "grip_mm": 25.399999999999999}], "warnings_count": 0, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8", "errors_count": 0, "is_valid": true}, "meta": {"operation": "validate", "addin_version": "0.13.2", "duration_ms": 6}, "ok": true, "warnings": []}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"errors":[],"data":{"calculated_values":{"axis_distance_mm":0.089999999999999997,"frame_y":[0,0,1],"kind":"node","outline_mode":null,"outline_vertices":null,"outline_height_mm":null,"outline_width_mm":null,"frame_z":[0,-1,0],"chord_direction_reversed":true,"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"frame_x":[1,0,0],"eccentricity_mm":54.899999999999999,"outline_points_mm":null},"bolt_stacks":[],"warnings_count":2,"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9","errors_count":0,"is_valid":true},"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":7},"ok":true,"warnings":[{"code":"MEMBER_ECCENTRIC","path":"members[0].axis_offset_mm","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE."},{"code":"MEMBER_ECCENTRIC","path":"members[1].axis_offset_mm","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_N ...
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo (arranque) arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"errors":[],"data":{"templates":{"6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D","91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior"},"has_batch_connections":false,"document":"HANGAR_PRUEBA_sondeo","labels":{"view_id":null,"count":0},"nodes":[{"color_rgb":null,"max_deviation_deg":null,"attempts":[],"warnings_count":0,"validation_token":null,"kind":"node","split_steps":[],"label_index":null,"match":null,"created_connection_id":null,"starter":false,"replaces_existing":false,"marker_element_id":null,"signature":"1 barra(s) sin marco","advice":"No es un nudo: nada que hacer","split_of_node":null,"is_valid":false,"template_name":null,"orientation":null,"visible_by_default":false,"eccentricity_mm":0,"is_mirrored":false,"chord_continuous":false,"actions":[],"chord_type_name":null,"name":"N1","color_name":null,"errors":[],"chord_element_id":0,"ghost_element_id":null,"through_element_ids":[],"errors_count":0,"has_spec_override":false,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14656,17204.299999999999,19933],"member_element_ids":[1245531],"status_text":"\u25cb Barra suelta (no es nudo)","template_id":null,"spec":null,"splice_element_id":null,"members":[],"status":"untyped","warnings":[],"element_ids":[1245531],"existing_connection_id":null,"is_manual":false,"connection_type":null,"existing_batch_id":null,"is_marked":false},{"color_rgb":[46,160,67],"max_deviation_deg":null,"attempts":[],"warnings_co ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado ce300dd6-df5e-4790-aee2-f593caf53612
Cuerpo:
{"errors": [], "data": {"remaining_labels": 0, "discarded_plan_id": "ce300dd6-df5e-4790-aee2-f593caf53612", "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0, "remaining_plans": 0}, "meta": {"operation": "batch_plan_discard", "addin_version": "0.13.2", "duration_ms": 12}, "ok": true, "warnings": []}
======================================================================
31. POST /conn/validate/ del Detalle D con gusset.outline.mode auto (sin puntos) -> válido, calculated_values.outline_points_mm y la caja (Fase 13)  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH'], 8 vértices, caja 540 × 585 mm, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"errors":[],"data":{"calculated_values":{"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"kind":"node","outline_mode":"auto","outline_vertices":8,"outline_height_mm":585,"outline_width_mm":540,"frame_z":[0,-1,0],"chord_direction_reversed":true,"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_x":[1,0,0],"eccentricity_mm":3.3999999999999999,"outline_points_mm":[[-315,-80],[-290,-140],[-150,-285],[225,-80],[225,135],[135,220],[-230,300],[-315,210]]},"bolt_stacks":[{"gusset_face":"+z","bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip","grip_mm":19.524999999999999,"member_element_id":1249636}],"warnings_count":3,"validation_token":"3cecf67b7243f8fcd0e05e709b87c59e6a3a0792568b7fed2c1afbc1ba607c39","errors_count":0,"is_valid":true},"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":26},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"code":"LABEL_VALUE_MISMATCH","path":"gusset.width_mm","message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00 ...
======================================================================
31b. POST /conn/preview/ con outline.mode auto -> gusset_plate con outline_mode auto y los mismos puntos que validate (Fase 13)  [OK]  HTTP 200, ok=True, cartela con 8 vértices (los mismos que validate: True)
Cuerpo:
{"errors": [], "data": {"summary": {"first_member_element_id": 1249630, "dry_run": true, "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "bolts": 4, "backend": "advancesteel", "gusset_plates": 1, "chord_element_id": 1249510, "knife_plates": 1, "connection_type": "gusset_node", "weld_lines": 6, "members_modified": 3}, "members_to_modify": [{"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -95.099999999999994, "role": "diagonal", "setback_mm": 180, "element_id": 1249630, "end": "end", "current_end_distance_mm": 84.900000000000006, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -40.799999999999997, "role": "diagonal", "setback_mm": 60, "element_id": 1249631, "end": "start", "current_end_distance_mm": 19.199999999999999, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -211.40000000000001, "role": "diagonal", "setback_mm": 260, "element_id": 1249636, "end": "end", "current_end_distance_mm": 48.600000000000001, "profile": "HSS2-1-2X2-1-2X3-16 64x64"}], "calculated_values": {"axis_distance_mm": 0.080000000000000002, "frame_y": [0, 0, 1], "kind": "node", "outline_mode": "auto", "outline_vertices": 8, "outline_height_mm": 585, "outline_width_mm": 540, "frame_z": [0, -1,  ...
======================================================================
31c. POST /conn/validate/ con outline.mode auto Y points_mm -> SCHEMA_INVALID (Fase 13)  [OK]  HTTP 200, ok=False, errores=['SCHEMA_INVALID'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"errors":[{"code":"SCHEMA_INVALID","path":"gusset.outline.points_mm","message":"outline.mode \"auto\" no lleva points_mm: el contorno lo calcula el add-in (usa polygon si quieres fijar los puntos).","hint":"Quita points_mm (o pon null) para la cartela autom\u00e1tica, o cambia mode a \"polygon\"."}],"data":null,"meta":{"operation":"validate","addin_version":"0.13.2","duration_ms":10},"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"code":"LABEL_VALUE_MISMATCH","path":"gusset.width_mm","message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00e9jalo: es informativo con outline.mode auto."},{"code":"LABEL_VALUE_MISMATCH","path":"gusset.height_mm","message":"gusset.height_mm (530.0 mm) no coincide con el alto de la cartela autom\u00e1tica calculada (585.0 mm): manda el calculado.","hint":"Escribe el alto calculado (calculated_values.outline_height_mm) o d\u00e9jalo: es informativo con outline.mode auto."}]}
======================================================================
32. POST /conn/split/ dry_run de la diagonal 1249632 contra el cordón 1245533 -> el punto del Detalle B (≈ 485 mm) sin tocar nada (Fase 13, D7)  [OK]  HTTP 200, ok=True, θ=45 cara=143.7 primera plancha=473.7 corte=483.2 mm (corto None, largo None)
Cuerpo:
{"errors":[],"data":{"dry_run":true,"preview":{"split_distance_mm":483.19999999999999,"cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"element_id":1249632,"chord_element_id":1245533,"theta_deg":45,"plate_thickness_mm":9.5250000000000004,"member_type":"HSS2-1-2X2-1-2X3-16 64x64","gap_mm":0,"chord_type":"HSS12X8X1/2","short_segment_mm":521.60000000000002,"long_segment_mm":3024.4000000000001,"face_distance_mm":143.69999999999999,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"offset_from_chord_face_mm":330,"first_plate_distance_mm":473.69999999999999},"description":"La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cord\u00f3n 1245533 a 45,0\u00b0: la cara del cord\u00f3n queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo),","already_split":null},"meta":{"operation":"split","addin_version":"0.13.2","duration_ms":7},"ok":true,"warnings":[]}
======================================================================
32b. POST /conn/node_info/ del nudo en K -> split_point_mm y split_distance_mm por diagonal (Fase 13)  [OK]  HTTP 200, ok=True, [(1245533, None), (1249631, 484.8), (1249632, 483.2)]
Cuerpo:
{"errors": [], "data": {"x_axis": [1, -1.9999999999999999e-06, 0], "members": [{"length_mm": 10475, "angle_in_plane_deg": -180, "wall_mm": 11.810000000000000, "is_chord": true, "axis_cut_mm": [-9278.1000000000004, -17195.700000000001, 19933], "family": "HSS-Hollow Structural Section", "start_mm": [-4180.1999999999998, -17195.799999999999, 19933], "slope_deg": 0, "end_gap_mm": 0, "structural_type": "Beam", "axis_offset_mm": 0, "angle_to_chord_deg": 0, "side": "chord", "material": "Steel ASTM A500, Grade B, Rectangular and Square", "split_distance_mm": null, "width_mm": 203.19999999999999, "split_point_mm": null, "split_offset_from_chord_face_mm": null, "type": "HSS12X8X1/2", "node_end": 0, "element_id": 1245533, "end_mm": [-14655.200000000001, -17195.700000000001, 19933], "reaches_node": true, "section_rotation_deg": 90, "height_mm": 304.80000000000001}, {"length_mm": 3500, "angle_in_plane_deg": -135.59999999999999, "wall_mm": 0, "is_chord": false, "axis_cut_mm": [-9305.5, -17195.700000000001, 19933], "family": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-11856.5, -17195.799999999999, 17437.299999999999], "slope_deg": 44.369999999999997, "end_gap_mm": 90.400000000000006, "structural_type": "Beam", "axis_offset_mm": -27.399999999999999, "angle_to_chord_deg": 44.399999999999999, "side": "-Y", "material": "Material IFC (190-40-140)", "split_distance_mm": 484.80000000000001, "width_mm": 63.5, "split_point_mm": [-9652.1000000000004, -17195.700000000001, 19594], "split_offset_from_cho ...
======================================================================
32c. POST /conn/split/ REAL de la diagonal 1249632 -> split_id y new_element_id (ESCRIBE: se deshace en la 32e) (Fase 13)  [OK]  HTTP 200, ok=True, split_id=9b28eea7-a315-4f3c-9353-eb7ac01d0a3b tramo nuevo=1322549 mecanismo=copy_element avisos=[]
Cuerpo:
{"errors":[],"data":{"next":"conn_get_node_info con [1249632, 1322549] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.","split":{"split_distance_mm":483.19999999999999,"original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001],"new_element_id":1322549,"original_join_allowed":[false,false],"element_id":1249632,"original_start_feet":[-30.438972704533338,-56.416780678368951,65.486029733016920],"mechanism":"copy_element","approximated_parameters":[],"batch_id":null,"chord_element_id":1245533,"created_utc":"2026-10-31T17:42:11.4262144Z","connection_id":null,"original_end_feet":[-22.212491925579190,-56.416761369748379,57.259548954040092],"split_id":"9b28eea7-a315-4f3c-9353-eb7ac01d0a3b","split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"offset_from_chord_face_mm":330},"preview":{"split_distance_mm":483.19999999999999,"cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"element_id":1249632,"chord_element_id":1245533,"theta_deg":45,"plate_thickness_mm":9.5250000000000004,"member_type":"HSS2-1-2X2-1-2X3-16 64x64","gap_mm":0,"chord_type":"HSS12X8X1/2","short_segment_mm":521.60000000000002,"long_segment_mm":3024.4000000000001,"face_distance_mm":143.69999999999999,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"offset_from_chord_face_mm":330,"first_plate_distance_mm":473.69999999999999} ...
======================================================================
32d. POST /conn/node_info/ de los dos tramos -> kind splice (el empalme B se puede aplicar) (Fase 13)  [OK]  HTTP 200, ok=True, kind=splice barras=[1249632, 1322549]
Cuerpo:
{"errors": [], "data": {"x_axis": [0.70710700000000004, 1.9999999999999999e-06, -0.70710700000000004], "members": [{"length_mm": 521.60000000000002, "angle_in_plane_deg": -180, "wall_mm": 0, "is_chord": false, "axis_cut_mm": [-8909, -17195.799999999999, 19591.299999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-9277.7999999999993, -17195.799999999999, 19960.099999999999], "slope_deg": 45, "end_gap_mm": 0, "structural_type": "Beam", "axis_offset_mm": 0, "angle_to_chord_deg": 0, "side": "-Y", "material": "Material IFC (190-40-140)", "split_distance_mm": null, "width_mm": 63.5, "split_point_mm": null, "split_offset_from_chord_face_mm": null, "type": "HSS2-1-2X2-1-2X3-16 64x64", "node_end": 1, "element_id": 1249632, "end_mm": [-8909, -17195.799999999999, 19591.299999999999], "reaches_node": true, "section_rotation_deg": -180, "height_mm": 63.5}, {"length_mm": 3024.4000000000001, "angle_in_plane_deg": 0, "wall_mm": 0, "is_chord": true, "axis_cut_mm": [-8909, -17195.799999999999, 19591.299999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-8909, -17195.799999999999, 19591.299999999999], "slope_deg": 45, "end_gap_mm": 0, "structural_type": "Beam", "axis_offset_mm": 0, "angle_to_chord_deg": 0, "side": "chord", "material": "Material IFC (190-40-140)", "split_distance_mm": null, "width_mm": 63.5, "split_point_mm": null, "split_offset_from_chord_face_mm": null, "type": "HSS2-1-2X2-1-2X3-16 64x64", "node_end": 0, "element_id": 1322549, "end_mm": [-6770.3999999 ...
======================================================================
32e. POST /conn/unsplit/ por split_id -> deshecho, la diagonal vuelve a ser una (Fase 13, D8)  [OK]  HTTP 200, ok=True, Corte deshecho: el tramo 1322549 se borró y la barra 1249632 recuperó su curva. avisos=[]
Cuerpo:
{"errors":[],"data":{"unsplit":{"split_distance_mm":483.19999999999999,"original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001],"new_element_id":1322549,"original_join_allowed":[false,false],"element_id":1249632,"original_start_feet":[-30.438972704533338,-56.416780678368951,65.486029733016920],"mechanism":"copy_element","approximated_parameters":[],"batch_id":null,"chord_element_id":1245533,"created_utc":"2026-10-31T17:42:11.4262144Z","connection_id":null,"original_end_feet":[-22.212491925579190,-56.416761369748379,57.259548954040092],"split_id":"9b28eea7-a315-4f3c-9353-eb7ac01d0a3b","split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"offset_from_chord_face_mm":330},"description":"Corte deshecho: el tramo 1322549 se borr\u00f3 y la barra 1249632 recuper\u00f3 su curva.","complete":true},"meta":{"operation":"unsplit","addin_version":"0.13.2","duration_ms":42},"ok":true,"warnings":[]}
======================================================================
32f. POST /conn/unsplit/ de la barra ya entera -> SPLIT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['SPLIT_NOT_FOUND']
Cuerpo:
{"errors":[{"code":"SPLIT_NOT_FOUND","path":"element_id","message":"La barra 1249632 no est\u00e1 partida por el add-in (ning\u00fan corte la tiene como tramo).","hint":"conn_unsplit sin par\u00e1metros lista los cortes del modelo."}],"data":null,"meta":{"operation":"unsplit","addin_version":"0.13.2","duration_ms":3},"ok":false,"warnings":[]}
======================================================================
33. POST /conn/batch/plan/ (mark:false) del nudo en K -> split_steps por diagonal y los empalmes B previstos (after_split) detrás (Fase 13, D5)  [OK]  HTTP 200, ok=True, pasos=[(1249631, 484.8), (1249632, 483.2)] previstos=['● Empalme B (tras partir)', '● Empalme B (tras partir)'] | Se creará 1 conexión con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Además se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.
Cuerpo:
{"errors":[],"data":{"templates":{"6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D","91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior"},"has_batch_connections":false,"document":"HANGAR_PRUEBA_sondeo","labels":{"view_id":null,"count":0},"nodes":[{"color_rgb":null,"max_deviation_deg":null,"attempts":[],"warnings_count":0,"validation_token":null,"kind":"node","split_steps":[],"label_index":null,"match":null,"created_connection_id":null,"starter":false,"replaces_existing":false,"marker_element_id":null,"signature":"1 barra(s) sin marco","advice":"No es un nudo: nada que hacer","split_of_node":null,"is_valid":false,"template_name":null,"orientation":null,"visible_by_default":false,"eccentricity_mm":0,"is_mirrored":false,"chord_continuous":false,"actions":[],"chord_type_name":null,"name":"N1","color_name":null,"errors":[],"chord_element_id":0,"ghost_element_id":null,"through_element_ids":[],"errors_count":0,"has_spec_override":false,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","work_point_mm":[-14655.200000000001,-17195.700000000001,19933],"member_element_ids":[1245533],"status_text":"\u25cb Barra suelta (no es nudo)","template_id":null,"spec":null,"splice_element_id":null,"members":[],"status":"untyped","warnings":[],"element_ids":[1245533],"existing_connection_id":null,"is_manual":false,"connection_type":null,"existing_batch_id":null,"is_marked":false},{"color_rgb":null,"max_deviation_deg":null,"attempts":[],"warn ...
======================================================================
33b. POST /conn/batch/plan/discard/ del plan del nudo en K -> descartado  [OK]  HTTP 200, ok=True, descartado
Cuerpo:
{"errors": [], "data": {"remaining_labels": 0, "discarded_plan_id": "fac8d22f-c446-433c-a737-5d4f2edf9b55", "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0, "remaining_plans": 0}, "meta": {"operation": "batch_plan_discard", "addin_version": "0.13.2", "duration_ms": 11}, "ok": true, "warnings": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=91 conn_*=25
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete, conn_split_member, conn_unsplit
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.13.2
Cuerpo:
{
  "errors": [],
  "data": {
    "has_uidocument": true,
    "spec_version": "1.0",
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
    "addin_version": "0.13.2",
    "document": {
      "is_family": false,
      "is_modifiable": false,
      "is_workshared": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_read_only": false
    },
    "revit": {
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39",
      "language": "English_USA",
      "version_name": "Autodesk Revit 2027",
      "version_number": "2027"
    },
    "dotnet": {
      "load_context": "Default",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12"
    }
  },
  "meta": {
    "operation": "ping",
    "addin_version": "0.13.2",
    "duration_ms": 2
  },
  "ok": true,
  "warnings": []
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "errors": [],
  "data": {
    "calculated_values": {
      "axis_distance_mm": 0.08,
      "frame_y": [
        0,
        0,
        1
      ],
      "kind": "node",
      "outline_mode": null,
      "outline_vertices": null,
      "outline_height_mm": null,
      "outline_width_mm": null,
      "frame_z": [
        0,
        -1,
        0
      ],
      "chord_direction_reversed": true,
      "origin_mm": [
        -11869.4,
        -17195.8,
        17423
      ],
      "frame_x": [
        1,
        0,
        0
      ],
      "eccentricity_mm": 3.4,
      "outline_points_mm": null
    },
    "name": "PRUEBA probar_conexiones",
    "bolt_stacks": [
      {
        "gusset_face": "+z",
        "bolt_length_mm": 44.45,
        "length_source": "computed_from_grip",
        "grip_mm": 19.525,
        "member_element_id": 1249636
      }
    ],
    "proposal": {
      "attempts": [
        "Nudo K cordon superior → same: ranura 0 (diagonal -135.6°) → barra 1249636 (-135.6°, desvío 0.0°); ranura 1 (diagonal -45.0°) sin barra; barras sobrantes: 1249630, 1249631",
        "Nudo K cordon superior → mirror_x: ranura 0 (diagonal -44.4°) sin barra; ranura 1 (diagonal -135.0°) → barra 1249636 (-135.6°, desvío 0.6°); barras sobrantes: 1249630, 1249631",
        "Nudo K cordon superior → mirror_y: ranura 0 (diagonal 135.6°) → barra 1249630 (136.9°, desvío 1.3°); ranura 1 (diagonal 45.0°) → barra 1249631 (44.4°, desvío 0.6°); barras sobrantes: 1249636",
        "Nudo K cordon supe ...
======================================================================
Resultado: 49/49 pruebas correctas

```

## 13c-6 log failed

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
{"ts":"2026-10-31T12:40:27.6466874-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 31d0b406-52fd-4183-b95c-cc56140e5c5d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
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
{"ts":"2026-10-31T12:40:28.0092508-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 62e5c502-b059-4b5e-b1c0-c16735bd6fd9","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
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
{"ts":"2026-10-31T12:42:09.1160577-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"p
lan_id\": \"ab4b620d-1a4c-4360-85d3-4c7ec65b2a6d\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"000000000000
0000000000000000000000000000000000000000000000000000\"}]}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAIL
ED"],"duration_ms":14}}



```
