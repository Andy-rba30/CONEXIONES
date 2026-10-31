# Resultados de la Fase 13

Fecha: 2026-10-31T09:38:04


## 13-1 git

```text
8a777e3 Fase 13 (add-in 0.13.0): cartela autom├ítica (outline.mode auto) y partir la diagonal para el empalme del Detalle B
?? docs/fases/resultados-fase-13.md

```

## 13-1 build

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 22.51 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 22.5 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 22.5 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:32.42

```

## 13-1 test

```text
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:   311, Omitido:     0, Total:   311, Duraci├│n: 1 s - MotorConexiones.Tests.dll (net10.0)

```

## 13-1 deploy

```text
AVISO: Revit esta abierto (1 proceso(s)). Si la DLL ya estaba cargada, la copia fallara: cierra Revit y repite.
== dotnet build D:\Proyectos C#\CONEXIONES\MotorConexiones.sln -c Release ==
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.43
== MotorConexiones 0.13.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 13-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (27 rutas @api.route)
- copiado tools\conn_tools.py (25 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 13-1 config

```text

    "gusset_side_margin_mm": 30,
    "_comentario_contorno": "Fase 13 (decisiones D2 y D4): outline_mode auto = el arranque escribe gusset.outline.mode 
auto y la cartela se calcula desde las barras (margen gusset_margin_mm al final de cada ranura o placa, 
gusset_side_margin_mm a cada lado de la barra, gusset_beyond_chord_mm mÃ¡s allÃ¡ del cordÃ³n); polygon = la cartela 
rectangular de la Fase 11. Los tres mÃ¡rgenes solo se escriben en el JSON si difieren de los valores por defecto del 
Core (40, 30, 40).",
    "outline_mode": "auto",
  "_comentario_cartela_automatica": "Fase 13: max_auto_gusset_mm es el lado mÃ¡ximo de la caja de la cartela 
automÃ¡tica (outline.mode auto) antes del aviso OUTLINE_AUTO_LARGE (tambiÃ©n avisa si el Ã¡rea supera el doble de la 
de los rectÃ¡ngulos de las barras); auto_outline_max_vertices, cuÃ¡ntos vÃ©rtices como mucho conserva el contorno 
calculado (los giros menores de 3Â° se quitan). Cambiar auto_outline_max_vertices cambia los tokens de las 
especificaciones auto; max_auto_gusset_mm no.",
  "max_auto_gusset_mm": 1200,
  "auto_outline_max_vertices": 12
  "_comentario": "CatÃ¡logo de plantillas (Fase 7). Editable sin recompilar: scripts\\deploy.ps1 lo copia junto al 
add-in. catalog_folder admite %LOCALAPPDATA%; shared_catalog_folder es la copia opcional (catalog\\ del repositorio o 
una carpeta de red): deploy.ps1 copia a la carpeta del usuario las plantillas que falten y 'Guardar en catÃ¡logo' 
ofrece copiar ahÃ. Las claves node_* son de la detecciÃ³n de nudos (Fase 8): node_cluster_mm agrupa los extremos de 
las barras en un nudo, node_axis_max_distance_mm decide si una barra atraviesa el nudo (cordÃ³n) y node_face_reach_mm 
(ronda 8b) es hasta quÃ© distancia del eje del cordÃ³n se admite un extremo cortado en la cara del cordÃ³n; 0 = segÃºn 
el canto del perfil (medio canto de cada barra mÃ¡s node_cluster_mm). batch_single_undo (Fase 9): true = el lote 
(conn_batch_create, botÃ³n Crear N conexiones) es una sola entrada de deshacer (grupo exterior con un grupo por nudo 
anidado); false = plan B, una entrada de deshacer por nudo, por si los grupos anidados no conviven con Advance Steel 
(lo decide el sondeo 20). plan_labels (Fase 10): true = al planificar se pone en la vista una etiqueta pinchable con 
el nÃºmero de cada nudo (pincharla elige la fila en la ventana del plan); false = solo colores y marcadores. 
auto_angle_tolerance_deg (Fase 13, decisiÃ³n D3): tolerancia de Ã¡ngulo con la que casa una plantilla guardada con la 
cartela automÃ¡tica (outline.mode auto), mÃ¡s ancha que angle_tolerance_deg porque la cartela se recalcula con las 
barras reales. plan_ghosts (Fase 10): true = en cada nudo listo se dibuja una cartela fantasma (sÃ³lido transparente 
con el contorno de su cartela, del color de su estado) que Crear sustituye por acero y Descartar quita; false = sin 
fantasmas.",
  "auto_angle_tolerance_deg": 25.0,



```

## 13-2 ping

```text
== conn/ping -> HTTP 200 en 270 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  10,
                 "addin_version":  "0.13.0",
                 "operation":  "ping"
             },
    "data":  {
                 "spec_version":  "1.0",
                 "document":  {
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_workshared":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_modifiable":  false,
                                  "is_family":  false,
                                  "is_read_only":  false
                              },
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "language":  "English_USA",
                               "version_number":  "2027",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39"
                           },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
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
                 "addin_version":  "0.13.0",
                 "has_uidocument":  true
             },
    "ok":  true
}

```

## 13-2 list

```text
== conn/list -> HTTP 200 en 44 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  8,
                 "addin_version":  "0.13.0",
                 "operation":  "list"
             },
    "data":  {
                 "batches":  {

                             },
                 "connections_count":  0,
                 "connections":  [

                                 ],
                 "total_count":  0,
                 "batch_id":  null
             },
    "ok":  true
}

```

## 13-2 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 18 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  5,
                 "addin_version":  "0.13.0",
                 "operation":  "unsplit"
             },
    "data":  {
                 "splits_count":  0,
                 "splits":  [

                            ]
             },
    "ok":  true
}

```

## 13-2 sondeo 12

```text
ERROR: no existe el archivo 'scripts\sondeos\12-estado-barras.py'.

```

## 13-2 sondeo 13

```text
ERROR: no existe el archivo 'scripts\sondeos\13-limpiar-restos.py'.

```

## 13-3 list

```text
== conn/list -> HTTP 200 en 422 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  18,
                 "addin_version":  "0.13.0",
                 "operation":  "list"
             },
    "data":  {
                 "batches":  {

                             },
                 "connections_count":  1,
                 "connections":  [
                                     {
                                         "spec_version":  "1.0",
                                         "created_elements_count":  2,
                                         "created_utc":  "2026-10-31T14:49:23.3383267Z",
                                         "backend":  "advancesteel",
                                         "connection_id":  "036a0e8b-2ebe-47cb-9bf1-beeeec903b9b",
                                         "connection_type":  "direct_weld",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                         "batch_id":  null
                                     }
                                 ],
                 "total_count":  1,
                 "batch_id":  null
             },
    "ok":  true
}

```

## 13-3 delete

```text
== conn/delete -> HTTP 200 en 13 ms ==
{
    "errors":  [
                   {
                       "path":  "connection_id",
                       "code":  "INVALID_REQUEST",
                       "message":  "connection_id es requerido para borrar una conexión.",
                       "hint":  "Usa conn_list para ver las conexiones existentes."
                   }
               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  0,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  null,
    "ok":  false
}
ERROR INVALID_REQUEST: connection_id es requerido para borrar una conexión.

```

## 13-3 log revit_failure

```text
Get-Content : No se encuentra la ruta de acceso 'C:\Users\Andy Bayona 
Antón\AppData\Local\MotorConexiones\logs\motorconexiones-2026-10-31.jsonl' porque no existe.
En línea: 18 Carácter: 34
+ ... _failure" { Get-Content "$env:LOCALAPPDATA\MotorConexiones\logs\motor ...
+                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (C:\Users\Andy B...026-10-31.jsonl:String) [Get-Content], ItemNotFoundEx 
   ception
    + FullyQualifiedErrorId : PathNotFound,Microsoft.PowerShell.Commands.GetContentCommand
 

```

## 13-3 delete

```text
== conn/delete -> HTTP 200 en 545 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  121,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  {
                 "deleted_connection_id":  "036a0e8b-2ebe-47cb-9bf1-beeeec903b9b",
                 "deleted_elements_count":  2,
                 "restored_members_count":  2
             },
    "ok":  true
}

```

## 13-3 log revit_failure

```text

{"ts":"2026-10-31T09:49:23.5062278-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322493","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear EspecificaciÃ³n de arranque (Conectar)"}}
{"ts":"2026-10-31T09:49:23.5075591-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: Fine.","elements":"1322494","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d21
6b","filtered":true,"transaction":"MotorConexiones: Crear EspecificaciÃ³n de arranque (Conectar)"}}



```

## 13-4 sondeo 30

```text
== 30-partir-barra.py -> HTTP 200 en 729 ms ==
Sondeo 30: partir la barra en dos (Fase 13). Documento: HANGAR_PRUEBA_sondeo. Cancelar al final: True

ERROR PROBE_EXCEPTION: TypeError: Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64)
PISTA: Lee el traceback en data.traceback.
TRACEBACK:
Traceback (most recent call last):
  File "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\revit_mcp\conexiones.py", line 354, in _ejecutar_sin_transaccion
    exec(codigo, espacio)
  File "<string>", line 346, in <module>
  File "<string>", line 209, in main
  File "<string>", line 65, in barra
TypeError: Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64)


```

## 13-4 sondeo 12

```text
ERROR: no existe el archivo 'scripts\sondeos\12-estado-barras.py'.

```

## 13-4 sondeo 13

```text
ERROR: no existe el archivo 'scripts\sondeos\13-limpiar-restos.py'.

```

## 13-5 list

```text
== conn/list -> HTTP 200 en 424 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  3,
                 "addin_version":  "0.13.0",
                 "operation":  "list"
             },
    "data":  {
                 "batches":  {

                             },
                 "connections_count":  1,
                 "connections":  [
                                     {
                                         "spec_version":  "1.0",
                                         "created_elements_count":  7,
                                         "created_utc":  "2026-10-31T15:32:23.1343202Z",
                                         "backend":  "advancesteel",
                                         "connection_id":  "546f4856-f2a5-47ec-a600-e2fcf024bebd",
                                         "connection_type":  "gusset_node",
                                         "template_id":  null,
                                         "batch_id":  null
                                     }
                                 ],
                 "total_count":  1,
                 "batch_id":  null
             },
    "ok":  true
}

```

## 13-5 get

```text
== conn/get -> HTTP 200 en 56 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  4,
                 "addin_version":  "0.13.0",
                 "operation":  "get"
             },
    "data":  {
                 "spec_version":  "1.0",
                 "created_elements_count":  7,
                 "spec":  {
                              "spec_version":  "1.0",
                              "members":  [
                                              {
                                                  "role":  "diagonal",
                                                  "end_setback_mm":  180,
                                                  "expected_angle_deg":  43.100000000000001,
                                                  "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                  "attachment":  {
                                                                     "type":  "welded_slot",
                                                                     "weld":  {
                                                                                  "type":  "fillet",
                                                                                  "all_around":  true,
                                                                                  "size_mm":  5
                                                                              },
                                                                     "slot_length_mm":  150
                                                                 },
                                                  "element_id":  1249630,
                                                  "axis_offset_mm":  1.7000000000000000
                                              },
                                              {
                                                  "role":  "diagonal",
                                                  "end_setback_mm":  180,
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                  "attachment":  {
                                                                     "type":  "welded_slot",
                                                                     "weld":  {
                                                                                  "type":  "fillet",
                                                                                  "all_around":  true,
                                                                                  "size_mm":  5
                                                                              },
                                                                     "slot_length_mm":  150
                                                                 },
                                                  "element_id":  1249631,
                                                  "axis_offset_mm":  -1.7000000000000000
                                              },
                                              {
                                                  "role":  "diagonal",
                                                  "end_setback_mm":  180,
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                  "attachment":  {
                                                                     "type":  "welded_slot",
                                                                     "weld":  {
                                                                                  "type":  "fillet",
                                                                                  "all_around":  true,
                                                                                  "size_mm":  5
                                                                              },
                                                                     "slot_length_mm":  150
                                                                 },
                                                  "element_id":  1249636,
                                                  "axis_offset_mm":  -1.7000000000000000
                                              }
                                          ],
                              "source":  {
                                             "drawing":  "Especificación de arranque (Conectar)"
                                         },
                              "node":  {
                                           "eccentricity_mm":  3.3999999999999999,
                                           "element_ids":  [
                                                               1249510,
                                                               1249630,
                                                               1249631,
                                                               1249636
                                                           ]
                                       },
                              "dimension_chains":  [

                                                   ],
                              "chord":  {
                                            "element_id":  1249510,
                                            "profile":  "HSS3X3X1/4",
                                            "continuous":  true
                                        },
                              "gusset":  {
                                             "thickness_mm":  9.5250000000000004,
                                             "chord_interface":  "through_slot",
                                             "height_mm":  610,
                                             "outline":  {
                                                             "mode":  "auto"
                                                         },
                                             "width_mm":  650,
                                             "weld_to_chord":  {
                                                                   "type":  "fillet",
                                                                   "all_around":  true,
                                                                   "size_mm":  5
                                                               },
                                             "thickness_label":  "3/8\""
                                         },
                              "uncertain_fields":  [
                                                       {
                                                           "user_confirmed_value":  "through_slot",
                                                           "path":  "gusset.chord_interface",
                                                           "reason":  "Arranque de Conectar: la unión de la cartela al cordón se supuso through_slot (valores de la plantilla Nudo tipico Detalle D); confírmala o cámbiala en el panel."
                                                       }
                                                   ],
                              "connection_type":  "gusset_node"
                          },
                 "created_element_ids":  [
                                             1322536,
                                             1322537,
                                             1322538,
                                             1322539,
                                             1322540,
                                             1322541,
                                             1322542
                                         ],
                 "created_utc":  "2026-10-31T15:32:23.1343202Z",
                 "backend":  "advancesteel",
                 "connection_id":  "546f4856-f2a5-47ec-a600-e2fcf024bebd",
                 "connection_type":  "gusset_node"
             },
    "ok":  true
}

```

## 13-6 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 407 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  1,
                 "addin_version":  "0.13.0",
                 "operation":  "catalog_list"
             },
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "members_count":  3,
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "created_utc":  "2026-10-31T15:38:19.4174284Z",
                                       "name":  "Cartela automatica cordon central",
                                       "tags":  [

                                                ],
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\9b1617ff-3e8d-462d-acf6-8735c9761227.json",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "connection_type":  "gusset_node",
                                       "template_id":  "9b1617ff-3e8d-462d-acf6-8735c9761227",
                                       "description":  null
                                   },
                                   {
                                       "members_count":  2,
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "name":  "Nudo K cordon superior",
                                       "tags":  [

                                                ],
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "chord_profile":  "HSS12X8X1/2",
                                       "connection_type":  "direct_weld",
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "description":  null
                                   },
                                   {
                                       "members_count":  3,
                                       "origin_drawing":  "Detalle D",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "connection_type":  "gusset_node",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ],
                 "templates_count":  3
             },
    "ok":  true
}

```

## 13-6 catalog_get

```text
== conn/catalog_get -> HTTP 200 en 90 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  1,
                 "addin_version":  "0.13.0",
                 "operation":  "catalog_get"
             },
    "data":  {
                 "name":  "Cartela automatica cordon central",
                 "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                 "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\9b1617ff-3e8d-462d-acf6-8735c9761227.json",
                 "template":  {
                                  "origin":  {
                                                 "drawing":  "Especificación de arranque (Conectar)",
                                                 "document":  "HANGAR_PRUEBA_sondeo",
                                                 "element_ids":  [
                                                                     1249510,
                                                                     1249630,
                                                                     1249631,
                                                                     1249636
                                                                 ],
                                                 "connection_id":  "546f4856-f2a5-47ec-a600-e2fcf024bebd"
                                             },
                                  "chord_pattern":  {
                                                        "profile_policy":  "warn",
                                                        "profile":  "HSS3X3X1/4",
                                                        "continuous":  true
                                                    },
                                  "catalog_version":  "1.0",
                                  "created_utc":  "2026-10-31T15:38:19.4174284Z",
                                  "name":  "Cartela automatica cordon central",
                                  "tags":  [

                                           ],
                                  "member_pattern":  [
                                                         {
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "side":  "+Y",
                                                             "role":  "diagonal",
                                                             "slot":  0,
                                                             "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "profile_policy":  "warn",
                                                             "angle_deg":  136.91999999999999
                                                         },
                                                         {
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "side":  "+Y",
                                                             "role":  "diagonal",
                                                             "slot":  1,
                                                             "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "profile_policy":  "warn",
                                                             "angle_deg":  44.369999999999997
                                                         },
                                                         {
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "side":  "-Y",
                                                             "role":  "diagonal",
                                                             "slot":  2,
                                                             "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "profile_policy":  "warn",
                                                             "angle_deg":  -135.63000000000000
                                                         }
                                                     ],
                                  "matching":  {
                                                   "angle_tolerance_deg":  25,
                                                   "allow_mirror":  true
                                               },
                                  "connection_type":  "gusset_node",
                                  "template_id":  "9b1617ff-3e8d-462d-acf6-8735c9761227",
                                  "spec_template":  {
                                                        "spec_version":  "1.0",
                                                        "members":  [
                                                                        {
                                                                            "role":  "diagonal",
                                                                            "end_setback_mm":  180,
                                                                            "slot":  0,
                                                                            "expected_angle_deg":  43.100000000000001,
                                                                            "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                            "attachment":  {
                                                                                               "type":  "welded_slot",
                                                                                               "weld":  {
                                                                                                            "type":  "fillet",
                                                                                                            "all_around":  true,
                                                                                                            "size_mm":  5
                                                                                                        },
                                                                                               "slot_length_mm":  150
                                                                                           }
                                                                        },
                                                                        {
                                                                            "role":  "diagonal",
                                                                            "end_setback_mm":  180,
                                                                            "slot":  1,
                                                                            "expected_angle_deg":  44.399999999999999,
                                                                            "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                            "attachment":  {
                                                                                               "type":  "welded_slot",
                                                                                               "weld":  {
                                                                                                            "type":  "fillet",
                                                                                                            "all_around":  true,
                                                                                                            "size_mm":  5
                                                                                                        },
                                                                                               "slot_length_mm":  150
                                                                                           }
                                                                        },
                                                                        {
                                                                            "role":  "diagonal",
                                                                            "end_setback_mm":  180,
                                                                            "slot":  2,
                                                                            "expected_angle_deg":  44.399999999999999,
                                                                            "profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                            "attachment":  {
                                                                                               "type":  "welded_slot",
                                                                                               "weld":  {
                                                                                                            "type":  "fillet",
                                                                                                            "all_around":  true,
                                                                                                            "size_mm":  5
                                                                                                        },
                                                                                               "slot_length_mm":  150
                                                                                           }
                                                                        }
                                                                    ],
                                                        "source":  {
                                                                       "drawing":  "Especificación de arranque (Conectar)"
                                                                   },
                                                        "dimension_chains":  [

                                                                             ],
                                                        "chord":  {
                                                                      "profile":  "HSS3X3X1/4",
                                                                      "continuous":  true
                                                                  },
                                                        "gusset":  {
                                                                       "thickness_mm":  9.5250000000000004,
                                                                       "chord_interface":  "through_slot",
                                                                       "height_mm":  610,
                                                                       "outline":  {
                                                                                       "mode":  "auto"
                                                                                   },
                                                                       "width_mm":  650,
                                                                       "weld_to_chord":  {
                                                                                             "type":  "fillet",
                                                                                             "all_around":  true,
                                                                                             "size_mm":  5
                                                                                         },
                                                                       "thickness_label":  "3/8\""
                                                                   },
                                                        "uncertain_fields":  [

                                                                             ],
                                                        "connection_type":  "gusset_node"
                                                    },
                                  "description":  null
                              },
                 "template_id":  "9b1617ff-3e8d-462d-acf6-8735c9761227"
             },
    "ok":  true
}

```

## 13-6 delete

```text
== conn/delete -> HTTP 200 en 118 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  92,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  {
                 "deleted_connection_id":  "546f4856-f2a5-47ec-a600-e2fcf024bebd",
                 "deleted_elements_count":  7,
                 "restored_members_count":  3
             },
    "ok":  true
}

```

## 13-6 catalog_delete

```text
== conn/catalog_delete -> HTTP 200 en 90 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  1,
                 "addin_version":  "0.13.0",
                 "operation":  "catalog_delete"
             },
    "data":  {
                 "name":  "Cartela automatica cordon central",
                 "deleted_template_id":  "9b1617ff-3e8d-462d-acf6-8735c9761227",
                 "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\9b1617ff-3e8d-462d-acf6-8735c9761227.json"
             },
    "ok":  true
}

```

## 13-7 list

```text
== conn/list -> HTTP 200 en 419 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  4,
                 "addin_version":  "0.13.0",
                 "operation":  "list"
             },
    "data":  {
                 "batches":  {

                             },
                 "connections_count":  3,
                 "connections":  [
                                     {
                                         "spec_version":  "1.0",
                                         "created_elements_count":  8,
                                         "created_utc":  "2026-10-31T15:46:22.1179796Z",
                                         "backend":  "advancesteel",
                                         "connection_id":  "2ef75794-f84a-4a6b-a7d1-44b242239df5",
                                         "connection_type":  "end_plate",
                                         "template_id":  null,
                                         "batch_id":  null
                                     },
                                     {
                                         "spec_version":  "1.0",
                                         "created_elements_count":  8,
                                         "created_utc":  "2026-10-31T15:46:22.3238696Z",
                                         "backend":  "advancesteel",
                                         "connection_id":  "59a3db83-e96b-415f-ac2e-50a4e4f8342d",
                                         "connection_type":  "end_plate",
                                         "template_id":  null,
                                         "batch_id":  null
                                     },
                                     {
                                         "spec_version":  "1.0",
                                         "created_elements_count":  2,
                                         "created_utc":  "2026-10-31T15:46:22.4556055Z",
                                         "backend":  "advancesteel",
                                         "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                                         "connection_type":  "direct_weld",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                         "batch_id":  null
                                     }
                                 ],
                 "total_count":  3,
                 "batch_id":  null
             },
    "ok":  true
}

```

## 13-7 unsplit lista

```text
== conn/unsplit -> HTTP 200 en 31 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  4,
                 "addin_version":  "0.13.0",
                 "operation":  "unsplit"
             },
    "data":  {
                 "splits_count":  2,
                 "splits":  [
                                {
                                    "approximated_parameters":  [

                                                                ],
                                    "batch_id":  null,
                                    "created_utc":  "2026-10-31T15:46:21.5844883Z",
                                    "split_point_mm":  [
                                                           -9652.1000000000004,
                                                           -17195.799999999999,
                                                           19594
                                                       ],
                                    "chord_element_id":  1245533,
                                    "new_element_id":  1322575,
                                    "offset_from_chord_face_mm":  330,
                                    "split_distance_mm":  484.80000000000001,
                                    "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                                    "original_start_mm":  [
                                                              -11856.5,
                                                              -17195.799999999999,
                                                              17437.299999999999
                                                          ],
                                    "element_id":  1249631,
                                    "split_id":  "55bc48d9-20ab-4203-af9e-8d04ca6d38f8",
                                    "mechanism":  "copy_element",
                                    "original_end_mm":  [
                                                            -9354.7000000000007,
                                                            -17195.799999999999,
                                                            19884.900000000001
                                                        ]
                                },
                                {
                                    "approximated_parameters":  [

                                                                ],
                                    "batch_id":  null,
                                    "created_utc":  "2026-10-31T15:46:22.2175987Z",
                                    "split_point_mm":  [
                                                           -8909,
                                                           -17195.799999999999,
                                                           19591.299999999999
                                                       ],
                                    "chord_element_id":  1245533,
                                    "new_element_id":  1322592,
                                    "offset_from_chord_face_mm":  330,
                                    "split_distance_mm":  483.19999999999999,
                                    "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                                    "original_start_mm":  [
                                                              -9277.7999999999993,
                                                              -17195.799999999999,
                                                              19960.099999999999
                                                          ],
                                    "element_id":  1249632,
                                    "split_id":  "32d57fda-daf0-4e9d-8e95-e1eb3e8085f7",
                                    "mechanism":  "copy_element",
                                    "original_end_mm":  [
                                                            -6770.3999999999996,
                                                            -17195.799999999999,
                                                            17452.700000000001
                                                        ]
                                }
                            ]
             },
    "ok":  true
}

```

## 13-7 node_info tramos

```text
== conn/node_info -> HTTP 200 en 46 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  13,
                 "addin_version":  "0.13.0",
                 "operation":  "node_info"
             },
    "data":  {
                 "origin_mm":  [
                                   -9652.1000000000004,
                                   -17195.799999999999,
                                   19594
                               ],
                 "members":  [
                                 {
                                     "split_point_mm":  null,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "angle_in_plane_deg":  0,
                                     "axis_offset_mm":  0,
                                     "node_end":  0,
                                     "angle_to_chord_deg":  0,
                                     "is_chord":  false,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1249631,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  0,
                                     "split_offset_from_chord_face_mm":  null,
                                     "length_mm":  416.10000000000002,
                                     "split_distance_mm":  null,
                                     "slope_deg":  44.369999999999997,
                                     "side":  "+Y",
                                     "axis_cut_mm":  [
                                                         -9652.1000000000004,
                                                         -17195.799999999999,
                                                         19594
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9652.1000000000004,
                                                      -17195.799999999999,
                                                      19594
                                                  ]
                                 },
                                 {
                                     "split_point_mm":  null,
                                     "end_mm":  [
                                                    -9652.1000000000004,
                                                    -17195.799999999999,
                                                    19594
                                                ],
                                     "angle_in_plane_deg":  -180,
                                     "axis_offset_mm":  0,
                                     "node_end":  1,
                                     "angle_to_chord_deg":  0,
                                     "is_chord":  true,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1322575,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  0,
                                     "split_offset_from_chord_face_mm":  null,
                                     "length_mm":  3083.9000000000001,
                                     "split_distance_mm":  null,
                                     "slope_deg":  44.369999999999997,
                                     "side":  "chord",
                                     "axis_cut_mm":  [
                                                         -9652.1000000000004,
                                                         -17195.799999999999,
                                                         19594
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ]
                                 }
                             ],
                 "z_axis":  [
                                2E-06,
                                -1,
                                0
                            ],
                 "axis_distance_mm":  0,
                 "existing_connections":  [
                                              {
                                                  "connection_type":  "end_plate",
                                                  "created_elements_count":  8,
                                                  "created_utc":  "2026-10-31T15:46:22.1179796Z",
                                                  "connection_id":  "2ef75794-f84a-4a6b-a7d1-44b242239df5"
                                              },
                                              {
                                                  "connection_type":  "direct_weld",
                                                  "created_elements_count":  2,
                                                  "created_utc":  "2026-10-31T15:46:22.4556055Z",
                                                  "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320"
                                              }
                                          ],
                 "chord_element_id":  1322575,
                 "eccentricity_mm":  0,
                 "kind":  "splice",
                 "chord_direction_reversed":  false,
                 "y_axis":  [
                                -0.69932899999999998,
                                -2E-06,
                                0.71479999999999999
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "x_axis":  [
                                0.71479999999999999,
                                2E-06,
                                0.69932899999999998
                            ]
             },
    "ok":  true
}

```

## 13-7 sondeo 12

```text
ERROR: no existe el archivo 'scripts\sondeos\12-estado-barras.py'.

```

## 13-7 delete nudo K primero (se espera aviso SPLIT_IN_USE)

```text
== conn/delete -> HTTP 200 en 133 ms ==
{
    "errors":  [

               ],
    "warnings":  [
                     {
                         "path":  "element_id",
                         "code":  "SPLIT_IN_USE",
                         "message":  "El corte de la barra 1249632 pedido por la conexión 9127b34a-b4ec-41b3-afd8-3fb94f748320 se queda: El corte de la barra 1249632 (tramos 1249632 y 1322592) lo usan las conexiones 59a3db83-e96b-415f-ac2e-50a4e4f8342d (end_plate): no se deshace.",
                         "hint":  "Borra antes esas conexiones (conn_delete o Borrar) y vuelve a conn_unsplit."
                     },
                     {
                         "path":  "element_id",
                         "code":  "SPLIT_IN_USE",
                         "message":  "El corte de la barra 1249631 pedido por la conexión 9127b34a-b4ec-41b3-afd8-3fb94f748320 se queda: El corte de la barra 1249631 (tramos 1249631 y 1322575) lo usan las conexiones 2ef75794-f84a-4a6b-a7d1-44b242239df5 (end_plate): no se deshace.",
                         "hint":  "Borra antes esas conexiones (conn_delete o Borrar) y vuelve a conn_unsplit."
                     }
                 ],
    "meta":  {
                 "duration_ms":  119,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  {
                 "deleted_connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                 "deleted_elements_count":  2,
                 "restored_members_count":  2
             },
    "ok":  true
}

```

## 13-7 delete empalmes B

```text
== conn/delete -> HTTP 200 en 147 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  100,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  {
                 "deleted_connection_id":  "2ef75794-f84a-4a6b-a7d1-44b242239df5",
                 "deleted_elements_count":  8,
                 "restored_members_count":  2
             },
    "ok":  true
}
== conn/delete -> HTTP 200 en 190 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  94,
                 "addin_version":  "0.13.0",
                 "operation":  "delete"
             },
    "data":  {
                 "deleted_connection_id":  "59a3db83-e96b-415f-ac2e-50a4e4f8342d",
                 "deleted_elements_count":  8,
                 "restored_members_count":  2
             },
    "ok":  true
}

```

## 13-7 unsplit restantes

```text
== conn/unsplit -> HTTP 200 en 78 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  67,
                 "addin_version":  "0.13.0",
                 "operation":  "unsplit"
             },
    "data":  {
                 "complete":  true,
                 "unsplit":  {
                                 "approximated_parameters":  [

                                                             ],
                                 "batch_id":  null,
                                 "created_utc":  "2026-10-31T15:46:21.5844883Z",
                                 "split_point_mm":  [
                                                        -9652.1000000000004,
                                                        -17195.799999999999,
                                                        19594
                                                    ],
                                 "chord_element_id":  1245533,
                                 "new_element_id":  1322575,
                                 "offset_from_chord_face_mm":  330,
                                 "split_distance_mm":  484.80000000000001,
                                 "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                                 "original_start_mm":  [
                                                           -11856.5,
                                                           -17195.799999999999,
                                                           17437.299999999999
                                                       ],
                                 "element_id":  1249631,
                                 "split_id":  "55bc48d9-20ab-4203-af9e-8d04ca6d38f8",
                                 "mechanism":  "copy_element",
                                 "original_end_mm":  [
                                                         -9354.7000000000007,
                                                         -17195.799999999999,
                                                         19884.900000000001
                                                     ]
                             },
                 "description":  "Corte deshecho: el tramo 1322575 se borró y la barra 1249631 recuperó su curva."
             },
    "ok":  true
}
== conn/unsplit -> HTTP 200 en 100 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  47,
                 "addin_version":  "0.13.0",
                 "operation":  "unsplit"
             },
    "data":  {
                 "complete":  true,
                 "unsplit":  {
                                 "approximated_parameters":  [

                                                             ],
                                 "batch_id":  null,
                                 "created_utc":  "2026-10-31T15:46:22.2175987Z",
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "chord_element_id":  1245533,
                                 "new_element_id":  1322592,
                                 "offset_from_chord_face_mm":  330,
                                 "split_distance_mm":  483.19999999999999,
                                 "connection_id":  "9127b34a-b4ec-41b3-afd8-3fb94f748320",
                                 "original_start_mm":  [
                                                           -9277.7999999999993,
                                                           -17195.799999999999,
                                                           19960.099999999999
                                                       ],
                                 "element_id":  1249632,
                                 "split_id":  "32d57fda-daf0-4e9d-8e95-e1eb3e8085f7",
                                 "mechanism":  "copy_element",
                                 "original_end_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ]
                             },
                 "description":  "Corte deshecho: el tramo 1322592 se borró y la barra 1249632 recuperó su curva."
             },
    "ok":  true
}

```

## 13-7 sondeo 12 final

```text
ERROR: no existe el archivo 'scripts\sondeos\12-estado-barras.py'.

```

## 13-8 split dry_run

```text
== conn/split -> HTTP 200 en 420 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  9,
                 "addin_version":  "0.13.0",
                 "operation":  "split"
             },
    "data":  {
                 "preview":  {
                                 "chord_type":  "HSS12X8X1/2",
                                 "long_segment_mm":  3024.4000000000001,
                                 "face_distance_mm":  143.69999999999999,
                                 "member_type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                 "gap_mm":  0,
                                 "theta_deg":  45,
                                 "chord_element_id":  1245533,
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "offset_from_chord_face_mm":  330,
                                 "first_plate_distance_mm":  473.69999999999999,
                                 "split_distance_mm":  483.19999999999999,
                                 "plate_thickness_mm":  9.5250000000000004,
                                 "cut_point_mm":  [
                                                      -9250.7000000000007,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                 "element_id":  1249632,
                                 "short_segment_mm":  521.60000000000002
                             },
                 "dry_run":  true,
                 "description":  "La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cordón 1245533 a 45,0°: la cara del cordón queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo),",
                 "already_split":  null
             },
    "ok":  true
}

```

## 13-8 split

```text
== conn/split -> HTTP 200 en 135 ms ==
{
    "errors":  [

               ],
    "warnings":  [
                     {
                         "path":  null,
                         "code":  "REVIT_WARNING",
                         "message":  "Advertencia de Revit: Beam or Brace is slightly off axis and may cause inaccuracies. [elementos: 1322620]",
                         "hint":  null
                     }
                 ],
    "meta":  {
                 "duration_ms":  115,
                 "addin_version":  "0.13.0",
                 "operation":  "split"
             },
    "data":  {
                 "preview":  {
                                 "chord_type":  "HSS12X8X1/2",
                                 "long_segment_mm":  3024.4000000000001,
                                 "face_distance_mm":  143.69999999999999,
                                 "member_type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                 "gap_mm":  0,
                                 "theta_deg":  45,
                                 "chord_element_id":  1245533,
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "offset_from_chord_face_mm":  330,
                                 "first_plate_distance_mm":  473.69999999999999,
                                 "split_distance_mm":  483.19999999999999,
                                 "plate_thickness_mm":  9.5250000000000004,
                                 "cut_point_mm":  [
                                                      -9250.7000000000007,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                 "element_id":  1249632,
                                 "short_segment_mm":  521.60000000000002
                             },
                 "split":  {
                               "approximated_parameters":  [

                                                           ],
                               "batch_id":  null,
                               "created_utc":  "2026-10-31T15:50:24.6139035Z",
                               "split_point_mm":  [
                                                      -8909,
                                                      -17195.799999999999,
                                                      19591.299999999999
                                                  ],
                               "chord_element_id":  1245533,
                               "new_element_id":  1322620,
                               "offset_from_chord_face_mm":  330,
                               "split_distance_mm":  483.19999999999999,
                               "connection_id":  null,
                               "original_start_mm":  [
                                                         -9277.7999999999993,
                                                         -17195.799999999999,
                                                         19960.099999999999
                                                     ],
                               "element_id":  1249632,
                               "split_id":  "02116c8a-c904-4cbf-8f5a-a3f4d60e3513",
                               "mechanism":  "copy_element",
                               "original_end_mm":  [
                                                       -6770.3999999999996,
                                                       -17195.799999999999,
                                                       17452.700000000001
                                                   ]
                           },
                 "description":  "La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cordón 1245533 a 45,0°: la cara del cordón queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo), Hecho: tramo nuevo 1322620.",
                 "next":  "conn_get_node_info con [1249632, 1322620] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte."
             },
    "ok":  true
}

```

## 13-8 node_info tramos

```text
== conn/node_info -> HTTP 200 en 19 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  6,
                 "addin_version":  "0.13.0",
                 "operation":  "node_info"
             },
    "data":  {
                 "origin_mm":  [
                                   -8909,
                                   -17195.799999999999,
                                   19591.299999999999
                               ],
                 "members":  [
                                 {
                                     "split_point_mm":  null,
                                     "end_mm":  [
                                                    -8909,
                                                    -17195.799999999999,
                                                    19591.299999999999
                                                ],
                                     "angle_in_plane_deg":  -180,
                                     "axis_offset_mm":  0,
                                     "node_end":  1,
                                     "angle_to_chord_deg":  0,
                                     "is_chord":  false,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1249632,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  0,
                                     "split_offset_from_chord_face_mm":  null,
                                     "length_mm":  521.60000000000002,
                                     "split_distance_mm":  null,
                                     "slope_deg":  45,
                                     "side":  "-Y",
                                     "axis_cut_mm":  [
                                                         -8909,
                                                         -17195.799999999999,
                                                         19591.299999999999
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ]
                                 },
                                 {
                                     "split_point_mm":  null,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "angle_in_plane_deg":  0,
                                     "axis_offset_mm":  0,
                                     "node_end":  0,
                                     "angle_to_chord_deg":  0,
                                     "is_chord":  true,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1322620,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  0,
                                     "split_offset_from_chord_face_mm":  null,
                                     "length_mm":  3024.4000000000001,
                                     "split_distance_mm":  null,
                                     "slope_deg":  45,
                                     "side":  "chord",
                                     "axis_cut_mm":  [
                                                         -8909,
                                                         -17195.799999999999,
                                                         19591.299999999999
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -8909,
                                                      -17195.799999999999,
                                                      19591.299999999999
                                                  ]
                                 }
                             ],
                 "z_axis":  [
                                2E-06,
                                -1,
                                0
                            ],
                 "axis_distance_mm":  0,
                 "existing_connections":  [

                                          ],
                 "chord_element_id":  1322620,
                 "eccentricity_mm":  0,
                 "kind":  "splice",
                 "chord_direction_reversed":  false,
                 "y_axis":  [
                                0.70710700000000004,
                                2E-06,
                                0.70710700000000004
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "x_axis":  [
                                0.70710700000000004,
                                2E-06,
                                -0.70710700000000004
                            ]
             },
    "ok":  true
}

```

## 13-8 node_info nudo K

```text
== conn/node_info -> HTTP 200 en 38 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  12,
                 "addin_version":  "0.13.0",
                 "operation":  "node_info"
             },
    "data":  {
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "members":  [
                                 {
                                     "split_point_mm":  null,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "angle_in_plane_deg":  -180,
                                     "axis_offset_mm":  0,
                                     "node_end":  0,
                                     "angle_to_chord_deg":  0,
                                     "is_chord":  true,
                                     "reaches_node":  true,
                                     "width_mm":  203.19999999999999,
                                     "family":  "HSS-Hollow Structural Section",
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "structural_type":  "Beam",
                                     "element_id":  1245533,
                                     "height_mm":  304.80000000000001,
                                     "end_gap_mm":  0,
                                     "split_offset_from_chord_face_mm":  null,
                                     "length_mm":  10475,
                                     "split_distance_mm":  null,
                                     "slope_deg":  0,
                                     "side":  "chord",
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "wall_mm":  11.810000000000000,
                                     "section_rotation_deg":  90,
                                     "type":  "HSS12X8X1/2",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ]
                                 },
                                 {
                                     "split_point_mm":  [
                                                            -9652.1000000000004,
                                                            -17195.700000000001,
                                                            19594
                                                        ],
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "axis_offset_mm":  -27.399999999999999,
                                     "node_end":  1,
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "is_chord":  false,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1249631,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  90.400000000000006,
                                     "split_offset_from_chord_face_mm":  330,
                                     "length_mm":  3500,
                                     "split_distance_mm":  484.80000000000001,
                                     "slope_deg":  44.369999999999997,
                                     "side":  "-Y",
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ]
                                 },
                                 {
                                     "split_point_mm":  [
                                                            -8909,
                                                            -17195.700000000001,
                                                            19591.299999999999
                                                        ],
                                     "end_mm":  [
                                                    -8909,
                                                    -17195.799999999999,
                                                    19591.299999999999
                                                ],
                                     "angle_in_plane_deg":  -45,
                                     "axis_offset_mm":  27.399999999999999,
                                     "node_end":  0,
                                     "angle_to_chord_deg":  45,
                                     "is_chord":  false,
                                     "reaches_node":  true,
                                     "width_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "material":  "Material IFC (190-40-140)",
                                     "structural_type":  "Beam",
                                     "element_id":  1249632,
                                     "height_mm":  63.5,
                                     "end_gap_mm":  27.100000000000001,
                                     "split_offset_from_chord_face_mm":  330,
                                     "length_mm":  521.60000000000002,
                                     "split_distance_mm":  483.19999999999999,
                                     "slope_deg":  45,
                                     "side":  "-Y",
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "wall_mm":  0,
                                     "section_rotation_deg":  -180,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ]
                                 }
                             ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "axis_distance_mm":  0.089999999999999997,
                 "existing_connections":  [

                                          ],
                 "chord_element_id":  1245533,
                 "eccentricity_mm":  54.899999999999999,
                 "kind":  "node",
                 "chord_direction_reversed":  true,
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ]
             },
    "ok":  true
}

```

## 13-8 unsplit

```text
== conn/unsplit -> HTTP 200 en 91 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  61,
                 "addin_version":  "0.13.0",
                 "operation":  "unsplit"
             },
    "data":  {
                 "complete":  true,
                 "unsplit":  {
                                 "approximated_parameters":  [

                                                             ],
                                 "batch_id":  null,
                                 "created_utc":  "2026-10-31T15:50:24.6139035Z",
                                 "split_point_mm":  [
                                                        -8909,
                                                        -17195.799999999999,
                                                        19591.299999999999
                                                    ],
                                 "chord_element_id":  1245533,
                                 "new_element_id":  1322620,
                                 "offset_from_chord_face_mm":  330,
                                 "split_distance_mm":  483.19999999999999,
                                 "connection_id":  null,
                                 "original_start_mm":  [
                                                           -9277.7999999999993,
                                                           -17195.799999999999,
                                                           19960.099999999999
                                                       ],
                                 "element_id":  1249632,
                                 "split_id":  "02116c8a-c904-4cbf-8f5a-a3f4d60e3513",
                                 "mechanism":  "copy_element",
                                 "original_end_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ]
                             },
                 "description":  "Corte deshecho: el tramo 1322620 se borró y la barra 1249632 recuperó su curva."
             },
    "ok":  true
}

```

## 13-8 sondeo 12

```text
ERROR: no existe el archivo 'scripts\sondeos\12-estado-barras.py'.

```

## 13-8 sondeo 13

```text
ERROR: no existe el archivo 'scripts\sondeos\13-limpiar-restos.py'.

```

## 13-9 batch_plan

```text
== conn/batch_plan -> HTTP 200 en 493 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  37,
                 "addin_version":  "0.13.0",
                 "operation":  "batch_plan"
             },
    "data":  {
                 "summary":  {
                                 "ready":  1,
                                 "after_split":  2,
                                 "untyped":  4
                             },
                 "created_count":  0,
                 "selection_expansion":  null,
                 "summary_text":  "Se creará 1 conexión con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Además se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.",
                 "hidden_text":  "4 barras sueltas",
                 "visible_count":  3,
                 "has_batch_connections":  false,
                 "is_marked":  false,
                 "failed_count":  0,
                 "marks":  {
                               "marker_element_ids":  [

                                                      ],
                               "element_count":  0,
                               "ghost_count":  0
                           },
                 "split_step_count":  2,
                 "unused_element_ids":  [

                                        ],
                 "templates":  {
                                   "91adac27-d196-4c98-88db-6579aa991be4":  "Nudo K cordon superior",
                                   "6abcf116-9b97-485f-b50d-2851ca0018cc":  "Nudo tipico Detalle D"
                               },
                 "overrides":  {
                                   "add_node":  {

                                                },
                                   "replace_existing":  false,
                                   "spec":  {

                                            },
                                   "split":  {

                                             },
                                   "chord":  {

                                             },
                                   "add_member":  {

                                                  },
                                   "exclude":  [

                                               ],
                                   "template":  {

                                                },
                                   "remove_member":  {

                                                     },
                                   "merge":  [

                                             ]
                               },
                 "label_view_id":  null,
                 "last_report":  null,
                 "plan_id":  "23090463-9082-4b89-bf9e-b519f327f885",
                 "created_utc":  "2026-10-31T16:02:59.2181913Z",
                 "labels":  {
                                "view_id":  null,
                                "count":  0
                            },
                 "ready_count":  1,
                 "marked_view_id":  null,
                 "selection_count":  3,
                 "created_in_batch_count":  0,
                 "description":  "7 nudo(s): 4 untyped, 2 after_split, 1 ready.",
                 "updated_utc":  "2026-10-31T16:02:59.2182137Z",
                 "after_split_count":  2,
                 "creatable_count":  1,
                 "document":  "HANGAR_PRUEBA_sondeo",
                 "label_indices":  {

                                   },
                 "nodes":  [
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  false,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  null,
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "connection_type":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "chord_type_name":  null,
                                   "visible_by_default":  false,
                                   "chord_element_id":  0,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "untyped",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1245533
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  null,
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              1245533
                                                          ],
                                   "split_of_node":  null,
                                   "kind":  "node",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  false,
                                   "name":  "N1",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [

                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "work_point_mm":  [
                                                         -14655.200000000001,
                                                         -17195.700000000001,
                                                         19933
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  false,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  null,
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "connection_type":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "chord_type_name":  null,
                                   "visible_by_default":  false,
                                   "chord_element_id":  0,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "untyped",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1249631
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  null,
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              1249631
                                                          ],
                                   "split_of_node":  null,
                                   "kind":  "node",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  false,
                                   "name":  "N2",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [

                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "work_point_mm":  [
                                                         -11856.5,
                                                         -17195.799999999999,
                                                         17437.299999999999
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  true,
                                   "existing_connection_id":  null,
                                   "warnings":  [
                                                    {
                                                        "path":  "members[0].axis_offset_mm",
                                                        "code":  "MEMBER_ECCENTRIC",
                                                        "message":  "El eje del miembro [1249631] corta el cordón a -27.4 mm del punto de trabajo (excentricidad admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                                                        "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE."
                                                    },
                                                    {
                                                        "path":  "members[1].axis_offset_mm",
                                                        "code":  "MEMBER_ECCENTRIC",
                                                        "message":  "El eje del miembro [1249632] corta el cordón a 27.4 mm del punto de trabajo (excentricidad admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                                                        "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE."
                                                    }
                                                ],
                                   "through_element_ids":  [
                                                               1245533
                                                           ],
                                   "split_steps":  [
                                                       {
                                                           "splice_node":  "N3B",
                                                           "face_distance_mm":  145.30000000000001,
                                                           "detail":  "B",
                                                           "split_point_mm":  [
                                                                                  -9652.1000000000004,
                                                                                  -17195.700000000001,
                                                                                  19594
                                                                              ],
                                                           "chord_element_id":  1245533,
                                                           "theta_deg":  44.399999999999999,
                                                           "new_element_id":  null,
                                                           "offset_from_chord_face_mm":  330,
                                                           "split_distance_mm":  484.80000000000001,
                                                           "error":  null,
                                                           "element_id":  1249631,
                                                           "split_id":  null,
                                                           "description":  "Partir 1249631 a 330 (Detalle B): corte a 485 mm del eje del cordón por el eje de la barra"
                                                       },
                                                       {
                                                           "splice_node":  "N3B2",
                                                           "face_distance_mm":  143.69999999999999,
                                                           "detail":  "B",
                                                           "split_point_mm":  [
                                                                                  -8909,
                                                                                  -17195.700000000001,
                                                                                  19591.299999999999
                                                                              ],
                                                           "chord_element_id":  1245533,
                                                           "theta_deg":  45,
                                                           "new_element_id":  null,
                                                           "offset_from_chord_face_mm":  330,
                                                           "split_distance_mm":  483.19999999999999,
                                                           "error":  null,
                                                           "element_id":  1249632,
                                                           "split_id":  null,
                                                           "description":  "Partir 1249632 a 330 (Detalle B): corte a 483 mm del eje del cordón por el eje de la barra"
                                                       }
                                                   ],
                                   "color_name":  "ambar",
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  true,
                                   "match":  {
                                                 "assignments":  [
                                                                     {
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "side":  "-Y",
                                                                         "role":  "diagonal",
                                                                         "template_angle_deg":  -135.63000000000000,
                                                                         "slot":  0,
                                                                         "model_angle_deg":  -135.63000000000000,
                                                                         "deviation_deg":  0,
                                                                         "profile_policy":  "warn",
                                                                         "template_profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "element_id":  1249631
                                                                     },
                                                                     {
                                                                         "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "side":  "-Y",
                                                                         "role":  "diagonal",
                                                                         "template_angle_deg":  -45,
                                                                         "slot":  1,
                                                                         "model_angle_deg":  -45,
                                                                         "deviation_deg":  0,
                                                                         "profile_policy":  "warn",
                                                                         "template_profile":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                                         "element_id":  1249632
                                                                     }
                                                                 ],
                                                 "is_complete":  true,
                                                 "score_deg":  0,
                                                 "max_deviation_deg":  0,
                                                 "orientation":  "same",
                                                 "unassigned_members":  [

                                                                        ],
                                                 "matched_count":  2,
                                                 "description":  "same: ranura 0 (diagonal -135.6°) → barra 1249631 (-135.6°, desvío 0.0°); ranura 1 (diagonal -45.0°) → barra 1249632 (-45.0°, desvío 0.0°)"
                                             },
                                   "advice":  "Nudo en K con excentricidad de 54,9 mm: se crea igual, cada barra recortada sobre su propio eje",
                                   "connection_type":  "direct_weld",
                                   "status_text":  "▲ Listo con aviso",
                                   "color_rgb":  [
                                                     240,
                                                     160,
                                                     0
                                                 ],
                                   "chord_type_name":  "HSS12X8X1/2",
                                   "visible_by_default":  true,
                                   "chord_element_id":  1245533,
                                   "eccentricity_mm":  54.899999999999999,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "ready",
                                   "orientation":  "same",
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1245533,
                                                       1249631,
                                                       1249632
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "2 -Y (-136, -45)",
                                   "max_deviation_deg":  0,
                                   "spec":  null,
                                   "template_name":  "Nudo K cordon superior",
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                   "member_element_ids":  [
                                                              1249631,
                                                              1249632
                                                          ],
                                   "split_of_node":  null,
                                   "kind":  "node",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  false,
                                   "name":  "N3",
                                   "warnings_count":  2,
                                   "actions":  [
                                                   {
                                                       "label":  "Excluir",
                                                       "key":  "exclude"
                                                   }
                                               ],
                                   "members":  [
                                                   {
                                                       "side":  "-Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  null,
                                                       "end_gap_mm":  90.400000000000006,
                                                       "angle_deg":  -135.59999999999999,
                                                       "element_id":  1249631,
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  -27.399999999999999
                                                   },
                                                   {
                                                       "side":  "-Y",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  null,
                                                       "end_gap_mm":  27.100000000000001,
                                                       "angle_deg":  -45,
                                                       "element_id":  1249632,
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  27.399999999999999
                                                   }
                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  "24c18194ca7c256635c7cf12287d33896e202b813919af96f4782169ae088503",
                                   "status_detail":  null,
                                   "work_point_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  true,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  "verde",
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "Se crea solo, después de partir la diagonal (paso previo de N3); nada que hacer",
                                   "connection_type":  "end_plate",
                                   "status_text":  "● Empalme B (tras partir)",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "visible_by_default":  true,
                                   "chord_element_id":  1249631,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "after_split",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1249631,
                                                       -1249631
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "empalme previsto de la diagonal 1249631",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  "arranque end_plate (Detalle B)",
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              -1249631
                                                          ],
                                   "split_of_node":  "N3",
                                   "kind":  "splice",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  true,
                                   "name":  "N3B",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [
                                                   {
                                                       "side":  "chord",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  null,
                                                       "end_gap_mm":  0,
                                                       "angle_deg":  0,
                                                       "element_id":  -1249631,
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  0
                                                   }
                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Empalme B de la diagonal 1249631: se crea después de partirla a 330 (el tramo largo saldrá con un id nuevo; -1249631 es provisional). Detalle B del plano para HSS2-1-2X2-1-2X3-16 64x64 (choices.json).",
                                   "work_point_mm":  [
                                                         -9652.1000000000004,
                                                         -17195.700000000001,
                                                         19594
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  true,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  "verde",
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "Se crea solo, después de partir la diagonal (paso previo de N3); nada que hacer",
                                   "connection_type":  "end_plate",
                                   "status_text":  "● Empalme B (tras partir)",
                                   "color_rgb":  [
                                                     46,
                                                     160,
                                                     67
                                                 ],
                                   "chord_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                   "visible_by_default":  true,
                                   "chord_element_id":  1249632,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "after_split",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1249632,
                                                       -1249632
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "empalme previsto de la diagonal 1249632",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  "arranque end_plate (Detalle B)",
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              -1249632
                                                          ],
                                   "split_of_node":  "N3",
                                   "kind":  "splice",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  true,
                                   "name":  "N3B2",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [
                                                   {
                                                       "side":  "chord",
                                                       "type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "chord_element_id":  null,
                                                       "end_gap_mm":  0,
                                                       "angle_deg":  0,
                                                       "element_id":  -1249632,
                                                       "reaches_node":  true,
                                                       "axis_offset_mm":  0
                                                   }
                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Empalme B de la diagonal 1249632: se crea después de partirla a 330 (el tramo largo saldrá con un id nuevo; -1249632 es provisional). Detalle B del plano para HSS2-1-2X2-1-2X3-16 64x64 (choices.json).",
                                   "work_point_mm":  [
                                                         -8909,
                                                         -17195.700000000001,
                                                         19591.299999999999
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  false,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  null,
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "connection_type":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "chord_type_name":  null,
                                   "visible_by_default":  false,
                                   "chord_element_id":  0,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "untyped",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1249632
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  null,
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              1249632
                                                          ],
                                   "split_of_node":  null,
                                   "kind":  "node",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  false,
                                   "name":  "N4",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [

                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "work_point_mm":  [
                                                         -6770.3999999999996,
                                                         -17195.799999999999,
                                                         17452.700000000001
                                                     ]
                               },
                               {
                                   "splice_element_id":  null,
                                   "is_valid":  false,
                                   "existing_connection_id":  null,
                                   "warnings":  [

                                                ],
                                   "through_element_ids":  [

                                                           ],
                                   "split_steps":  [

                                                   ],
                                   "color_name":  null,
                                   "ghost_element_id":  null,
                                   "attempts":  [

                                                ],
                                   "replaces_existing":  false,
                                   "is_marked":  false,
                                   "chord_continuous":  false,
                                   "match":  null,
                                   "advice":  "No es un nudo: nada que hacer",
                                   "connection_type":  null,
                                   "status_text":  "○ Barra suelta (no es nudo)",
                                   "color_rgb":  null,
                                   "chord_type_name":  null,
                                   "visible_by_default":  false,
                                   "chord_element_id":  0,
                                   "eccentricity_mm":  0,
                                   "created_connection_id":  null,
                                   "errors":  [

                                              ],
                                   "status":  "untyped",
                                   "orientation":  null,
                                   "errors_count":  0,
                                   "element_ids":  [
                                                       1245533
                                                   ],
                                   "existing_batch_id":  null,
                                   "signature":  "1 barra(s) sin marco",
                                   "max_deviation_deg":  null,
                                   "spec":  null,
                                   "template_name":  null,
                                   "label_index":  null,
                                   "is_manual":  false,
                                   "template_id":  null,
                                   "member_element_ids":  [
                                                              1245533
                                                          ],
                                   "split_of_node":  null,
                                   "kind":  "node",
                                   "has_spec_override":  false,
                                   "marker_element_id":  null,
                                   "starter":  false,
                                   "name":  "N5",
                                   "warnings_count":  0,
                                   "actions":  [

                                               ],
                                   "members":  [

                                               ],
                                   "is_mirrored":  false,
                                   "validation_token":  null,
                                   "status_detail":  "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.",
                                   "work_point_mm":  [
                                                         -4180.1999999999998,
                                                         -17195.799999999999,
                                                         19933
                                                     ]
                               }
                           ]
             },
    "ok":  true
}

```

## 13-9 discard

```text
== conn/batch_plan_discard -> HTTP 200 en 26 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  14,
                 "addin_version":  "0.13.0",
                 "operation":  "batch_plan_discard"
             },
    "data":  {
                 "remaining_plans":  1,
                 "discarded_plan_id":  "4b507595-6dc0-42ef-9f8b-5a5ae85b3b6f",
                 "remaining_markers":  0,
                 "removed_marks":  0,
                 "remaining_labels":  0,
                 "removed_labels":  0
             },
    "ok":  true
}

```

## 13-10 probar_conexiones

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.13.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":2,"addin_version":"0.13.0","operation":"ping"},"data":{"spec_version":"1.0","document":{"title":"HANGAR_PRUEBA_sondeo","is_workshared":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_modifiable":false,"is_family":false,"is_read_only":false},"revit":{"version_name":"Autodesk Revit 2027","language":"English_USA","version_number":"2027","sub_version_number":"2027.2","version_build":"27.2.0.39"},"dotnet":{"framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","load_context":"Default"},"backend":"advancesteel","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","split","types","unsplit","update","validate"],"addin_version":"0.13.0","has_uidocument":true},"ok":true}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 36647 caracteres
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":7,"addin_version":"0.13.0","operation":"guide"},"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventan ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"types"},"data":{"connection_types":[{"type_name":"direct_weld","description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n."},{"type_name":"end_plate","description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones)."},{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"ok":true}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"schema"},"data":{"json_schema":{"properties":{"spec_version":{"type":"string","enum":["1.0"]},"members":{"type":"array","items":{"type":"object","additionalProperties":false,"required":["element_id","role","end_setback_mm","attachment"],"properties":{"role":{"type":"string","enum":["diagonal","vertical","chord"]},"end_setback_mm":{"type":"number","minimum":0.0},"expected_angle_deg":{"type":"number"},"profile":{"type":["string","null"]},"attachment":{"type":"object","additionalProperties":false,"required":["type"],"properties":{"bolts":{"type":"object","additionalProperties":false,"required":["diameter_mm","rows","columns","spacing_mm","edge_mm","first_row_from_plate_end_mm"],"properties":{"first_row_from_plate_end_mm":{"type":"number","minimum":0.0},"edge_mm":{"type":"number","minimum":1.0},"rows":{"type":"integer","minimum":1},"columns":{"type":"integer","minimum":1},"diameter_label":{"type":"string"},"diameter_mm":{"type":"number","minimum":1.0},"spacing_mm":{"type":"number","minimum":1.0},"length_mm":{"type":"number","minimum":1.0,"description":"Longitud del perno bajo cabeza si el plano la indica; si falta se calcula del agarre (cartela + placa) m\u00e1s el suplemento de limits.json."}}},"weld_plate_to_member":{"type":"object","additionalProperties":false,"required":["type","size_mm"],"properties":{"type":{"type":"string","enum":["fillet"]},"all_around":{"type":"boolean"},"size_mm":{" ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"path":"type","code":"UNKNOWN_OPERATION","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node"}],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"schema"},"data":null,"ok":false}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 2, "addin_version": "0.13.0", "operation": "find_profile"}, "data": {"suggestions": [], "query": "HSS2-1/2X2-1/2X3/16", "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "total_profiles_in_model": 29, "matched_count": 1}, "ok": true}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 7, "addin_version": "0.13.0", "operation": "node_info"}, "data": {"origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "members": [{"split_point_mm": null, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "angle_in_plane_deg": 0, "axis_offset_mm": 0, "node_end": 1, "angle_to_chord_deg": 0, "is_chord": true, "reaches_node": true, "width_mm": 76.200000000000003, "family": "HSS-Hollow Structural Section", "material": "Steel ASTM A500, Grade B, Rectangular and Square", "structural_type": "Beam", "element_id": 1249510, "height_mm": 76.200000000000003, "end_gap_mm": 0, "split_offset_from_chord_face_mm": null, "length_mm": 9960.2999999999993, "split_distance_mm": null, "slope_deg": 0, "side": "chord", "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "wall_mm": 5.9199999999999999, "section_rotation_deg": 180, "type": "HSS3X3X1/4", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423]}, {"split_point_mm": [-12156.400000000000, -17195.799999999999, 17693], "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "angle_in_plane_deg": 136.90000000000001, "axis_offset_mm": 1.7000000000000000, "node_end": 1, "angle_to_chord_deg": 43.100000000000001, "is_chord": false, "reaches_node": true, "width_mm": 63.5, "family": "HSS2-1-2X2-1-2X3-16 64x64", "material": "Material IFC (190-40-140)", "structural_type": "Beam", "element_id": 1249630, "height_mm": 63.5, "end_gap_mm": 84. ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"meta":{"duration_ms":9,"addin_version":"0.13.0","operation":"validate"},"data":{"warnings_count":1,"errors_count":0,"is_valid":true,"bolt_stacks":[{"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip","member_element_id":1249636}],"calculated_values":{"outline_vertices":null,"frame_x":[1,0,0],"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"axis_distance_mm":0.080000000000000002,"outline_mode":null,"outline_width_mm":null,"frame_y":[0,0,1],"eccentricity_mm":3.3999999999999999,"frame_z":[0,-1,0],"kind":"node","outline_points_mm":null,"chord_direction_reversed":true,"outline_height_mm":null},"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1"},"ok":true}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"path":"dimension_chains[0].values_mm","code":"DIMENSION_CHAIN_MISMATCH","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm."}],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"meta":{"duration_ms":7,"addin_version":"0.13.0","operation":"validate"},"data":null,"ok":false}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"path":"uncertain_fields[0].user_confirmed_value","code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."},{"path":"uncertain_fields[1].user_confirmed_value","code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."}],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"meta":{"duration_ms":7,"addin_version":"0.13.0","operation":"validate"},"data":null,"ok":false}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"weld_lines": 6, "working_point_mm": [-11869.4, -17195.8, 17423], "backend": "advancesteel", "members_modified": 3, "chord_element_id": 1249510, "first_member_element_id": 1249630, "knife_plates": 1, "connection_type": "gusset_node", "gusset_plates": 1, "dry_run": true, "bolts": 4}
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 3, "addin_version": "0.13.0", "operation": "preview"}, "data": {"members_to_modify": [{"role": "diagonal", "setback_mm": 180, "end": "end", "new_extension_mm": -95.099999999999994, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 84.900000000000006, "element_id": 1249630}, {"role": "diagonal", "setback_mm": 60, "end": "start", "new_extension_mm": -40.799999999999997, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 19.199999999999999, "element_id": 1249631}, {"role": "diagonal", "setback_mm": 260, "end": "end", "new_extension_mm": -211.40000000000001, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 48.600000000000001, "element_id": 1249636}], "summary": {"weld_lines": 6, "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "backend": "advancesteel", "members_modified": 3, "chord_element_id": 1249510, "first_member_element_id": 1249630, "knife_plates": 1, "connection_type": "gusset_node", "gusset_plates": 1, "dry_run": true, "bolts": 4}, "calculated_values": {"outline_vertices": null, "frame_x": [1, 0, 0], "origin_mm": [-11869.400000000000, -17195.799999999999 ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"errors":[{"path":"validation_token","code":"VALIDATION_TOKEN_INVALID","message":"validation_token es obligatorio para crear una conexi\u00f3n.","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token."}],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"create"},"data":null,"ok":false}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 3, "addin_version": "0.13.0", "operation": "list"}, "data": {"batches": {}, "connections_count": 0, "connections": [], "total_count": 0, "batch_id": null}, "ok": true}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"connection_id","code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo."}],"warnings":[],"meta":{"duration_ms":4,"addin_version":"0.13.0","operation":"get"},"data":null,"ok":false}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"connection_id","code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Verifica los IDs disponibles con conn_list."}],"warnings":[],"meta":{"duration_ms":9,"addin_version":"0.13.0","operation":"delete"},"data":null,"ok":false}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"path":null,"code":"UNKNOWN_OPERATION","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, split, types, unsplit, update, validate."}],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"no_existe"},"data":null,"ok":false}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"catalog_list"},"data":{"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates":[{"members_count":2,"origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","origin_document":"HANGAR_PRUEBA_sondeo","created_utc":"2026-10-21T16:49:07.9104459Z","name":"Nudo K cordon superior","tags":[],"pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","chord_profile":"HSS12X8X1/2","connection_type":"direct_weld","template_id":"91adac27-d196-4c98-88db-6579aa991be4","description":null},{"members_count":3,"origin_drawing":"Detalle D","origin_document":"HANGAR_PRUEBA_sondeo","created_utc":"2026-10-04T22:01:50.5585233Z","name":"Nudo tipico Detalle D","tags":["hangar","cercha","HSS"],"pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","chord_profile":"HSS3X3X1/4","connection_type":"gusset_node","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/ ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=2b644c11-b47e-4949-a397-79d37bb71541 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\2b644c11-b47e-4949-a397-79d37bb71541.json
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"meta":{"duration_ms":15,"addin_version":"0.13.0","operation":"catalog_save"},"data":{"members_count":3,"origin":{"drawing":"Detalle D","document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636],"connection_id":null},"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\2b644c11-b47e-4949-a397-79d37bb71541.json","shared_file":null,"member_pattern":[{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","slot":0,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":136.91999999999999},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","slot":1,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":44.369999999999997},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","role":"diagonal","slot":2,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":-135.63000000000000}],"matching":{"angle_tolerance_deg":10,"allow_mirror":true},"chord_profile":"HSS3X3X1/4","template_id":"2b644c11-b47e-4949-a397-79d37bb71541"},"ok":true}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"catalog_get"},"data":{"name":"PRUEBA probar_conexiones","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\2b644c11-b47e-4949-a397-79d37bb71541.json","template":{"origin":{"drawing":"Detalle D","document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636],"connection_id":null},"chord_pattern":{"profile_policy":"warn","profile":"HSS3X3X1/4","continuous":true},"catalog_version":"1.0","created_utc":"2026-10-31T16:07:35.1363008Z","name":"PRUEBA probar_conexiones","tags":["prueba"],"member_pattern":[{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","slot":0,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":136.91999999999999},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","slot":1,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":44.369999999999997},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","role":"diagonal","slot":2,"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","angle_deg":-135.63000000000000}],"matching":{"angle_tolerance_deg":10,"allow_mirror":true},"connection_type":"gusset_node","template_id":"2b644c11-b47e-4949-a397-79d37bb71541","spec_template":{"spec_version":"1.0","members":[{"role":"diagonal","end_setbac ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=a76ad11273ba...
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":19,"addin_version":"0.13.0","operation":"catalog_apply"},"data":{"is_valid":true,"spec":{"spec_version":"1.0","members":[{"role":"diagonal","end_setback_mm":180.0,"expected_angle_deg":43.100000000000001,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","weld":{"type":"fillet","all_around":true,"size_mm":5.0},"slot_length_mm":150.0},"element_id":1249630},{"role":"diagonal","end_setback_mm":60.0,"expected_angle_deg":44.399999999999999,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","weld":{"type":"fillet","all_around":true,"size_mm":5.0},"slot_length_mm":150.0},"element_id":1249631},{"role":"diagonal","end_setback_mm":260.0,"expected_angle_deg":44.399999999999999,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"bolted_knife_plate","bolts":{"first_row_from_plate_end_mm":40.0,"edge_mm":40.0,"rows":2,"columns":2,"diameter_label":"5/8\"","diameter_mm":15.875,"spacing_mm":60.0},"plate":{"insertion_mm":80.0,"thickness_mm":10.0,"width_mm":140.0,"thickness_label":"PL10","length_mm":170.0},"weld_plate_to_member":{"type":"fillet","all_around":true,"size_mm":5.0}},"element_id":1249636}],"source":{"drawing":"Detalle D","template_id":"2b644c11-b47e-4949-a397-79d37bb71541","scale":"1/10"},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125 ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":17,"addin_version":"0.13.0","operation":"catalog_apply"},"data":{"is_valid":true,"spec":{"spec_version":"1.0","members":[{"role":"diagonal","end_setback_mm":180.0,"expected_angle_deg":43.100000000000001,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","weld":{"type":"fillet","all_around":true,"size_mm":5.0},"slot_length_mm":150.0},"element_id":1249630},{"role":"diagonal","end_setback_mm":60.0,"expected_angle_deg":44.399999999999999,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","weld":{"type":"fillet","all_around":true,"size_mm":5.0},"slot_length_mm":150.0},"element_id":1249631},{"role":"diagonal","end_setback_mm":260.0,"expected_angle_deg":44.399999999999999,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"bolted_knife_plate","bolts":{"first_row_from_plate_end_mm":40.0,"edge_mm":40.0,"rows":2,"columns":2,"diameter_label":"5/8\"","diameter_mm":15.875,"spacing_mm":60.0},"plate":{"insertion_mm":80.0,"thickness_mm":10.0,"width_mm":140.0,"thickness_label":"PL10","length_mm":170.0},"weld_plate_to_member":{"type":"fillet","all_around":true,"size_mm":5.0}},"element_id":1249636}],"source":{"drawing":"Detalle D","template_id":"2b644c11-b47e-4949-a397-79d37bb71541","scale":"1/10"},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125 ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=fd9979c8-9f6f-40f0-a201-c4232b5ca24a nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":23,"addin_version":"0.13.0","operation":"batch_plan"},"data":{"summary":{"ready":1,"untyped":5},"created_count":0,"selection_expansion":null,"summary_text":"Se crear\u00e1 1 conexi\u00f3n con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.","hidden_text":"5 barras sueltas","visible_count":1,"has_batch_connections":false,"is_marked":false,"failed_count":0,"marks":{"marker_element_ids":[],"element_count":0,"ghost_count":0},"split_step_count":0,"unused_element_ids":[],"templates":{"2b644c11-b47e-4949-a397-79d37bb71541":"PRUEBA probar_conexiones"},"overrides":{"add_node":{},"replace_existing":false,"spec":{},"split":{},"chord":{},"add_member":{},"exclude":[],"template":{},"remove_member":{},"merge":[]},"label_view_id":null,"last_report":null,"plan_id":"fd9979c8-9f6f-40f0-a201-c4232b5ca24a","created_utc":"2026-10-31T16:07:35.3052438Z","labels":{"view_id":null,"count":0},"ready_count":1,"marked_view_id":null,"selection_count":4,"created_in_batch_count":0,"description":"6 nudo(s): 5 untyped, 1 ready.","updated_utc":"2026-10-31T16:07:35.3053080Z","after_split_count":0,"creatable_count":1,"document":"HANGAR_PRUEBA_sondeo","label_indices":{},"nodes":[{"splice_element_id":null,"is_valid":false,"existing_connection_id":null,"warnings":[],"through_element_ids":[],"split_steps":[],"color_name":null,"ghost_element_id":null,"attempts":[],"replaces_existing":false,"is_marked":false,"chord_continuous":false,"match":nu ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=8b54301ec364...
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"batch_plan_get"},"data":{"node":{"splice_element_id":null,"is_valid":true,"existing_connection_id":null,"warnings":[],"through_element_ids":[1249510],"split_steps":[],"color_name":"verde","ghost_element_id":null,"attempts":[],"replaces_existing":false,"is_marked":false,"chord_continuous":true,"match":{"assignments":[{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","template_angle_deg":136.91999999999999,"slot":0,"model_angle_deg":136.91999999999999,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","element_id":1249630},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","role":"diagonal","template_angle_deg":44.369999999999997,"slot":1,"model_angle_deg":44.369999999999997,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","element_id":1249631},{"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","role":"diagonal","template_angle_deg":-135.63000000000000,"slot":2,"model_angle_deg":-135.63000000000000,"deviation_deg":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","element_id":1249636}],"is_complete":true,"score_deg":0.01,"max_deviation_deg":0,"orientation":"same","unassigned_members":[],"matched_count":3,"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 ( ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote fd9979c8: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)).
Cuerpo:
{"errors":[],"warnings":[{"path":"nodes[N4]","code":"BATCH_NODE_FAILED","message":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","hint":"Ese nudo se revirti\u00f3 solo; en el plan queda failed con su especificaci\u00f3n y su token para reintentar."}],"meta":{"duration_ms":1,"addin_version":"0.13.0","operation":"batch_create"},"data":{"created_count":0,"rolled_back_count":0,"plan_creatable_count":1,"unsplit_count":0,"updated_count":0,"summary_text":"Lote fd9979c8: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)).","failed_count":1,"deleted_elements_count":0,"stopped":false,"undo_entries":"one","plan_summary":{"failed":1,"untyped":5},"batch_id":"fd9979c8-9f6f-40f0-a201-c4232b5ca24a","with_warnings_count":0,"deleted_count":0,"plan_id":"fd9979c8-9f6f-40f0-a201-c4232b5ca24a","duration_ms":0,"skipped_count":0,"restored_members_count":0,"finished_utc":"2026-10-31T16:07:35.4190311Z","plan_summary_text":"Creadas 0 conexiones, 1 fall\u00f3. Pulsa Crear otra vez para reintentar la fallida. Ocultos: 5 barras sueltas.","started_utc":"2026-10-31T16:07:35.4189937Z","stop_on_error":false,"document":"HANGAR_PRUEBA_sondeo","operation":"batch_create","split_count":0,"stopped_at":null,"nodes":[{"errors":[{"path":"nodes[N4].validation_token","code":"VALIDATION_TO ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"errors":[],"warnings":[{"path":"batch_id","code":"BATCH_EMPTY","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote)."}],"meta":{"duration_ms":6,"addin_version":"0.13.0","operation":"batch_delete"},"data":{"created_count":0,"rolled_back_count":0,"plan_creatable_count":null,"unsplit_count":0,"updated_count":0,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","failed_count":0,"deleted_elements_count":0,"stopped":false,"undo_entries":"one","plan_summary":null,"batch_id":"00000000-0000-0000-0000-000000000000","with_warnings_count":0,"deleted_count":0,"plan_id":null,"duration_ms":6,"skipped_count":0,"restored_members_count":0,"finished_utc":"2026-10-31T16:07:35.4562090Z","plan_summary_text":null,"started_utc":"2026-10-31T16:07:35.4495843Z","stop_on_error":false,"document":"HANGAR_PRUEBA_sondeo","operation":"batch_delete","split_count":0,"stopped_at":null,"nodes":[],"connection_ids":[]},"ok":true}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado fd9979c8-9f6f-40f0-a201-c4232b5ca24a
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 14, "addin_version": "0.13.0", "operation": "batch_plan_discard"}, "data": {"remaining_plans": 1, "discarded_plan_id": "fd9979c8-9f6f-40f0-a201-c4232b5ca24a", "remaining_markers": 0, "removed_marks": 0, "remaining_labels": 0, "removed_labels": 0}, "ok": true}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"template_id","code":"TEMPLATE_NOT_FOUND","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","hint":"Usa conn_catalog_list para ver las plantillas disponibles."}],"warnings":[],"meta":{"duration_ms":0,"addin_version":"0.13.0","operation":"catalog_apply"},"data":null,"ok":false}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 2b644c11-b47e-4949-a397-79d37bb71541
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":1,"addin_version":"0.13.0","operation":"catalog_delete"},"data":{"name":"PRUEBA probar_conexiones","deleted_template_id":"2b644c11-b47e-4949-a397-79d37bb71541","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\2b644c11-b47e-4949-a397-79d37bb71541.json"},"ok":true}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"errors":[],"warnings":[{"path":"element_ids","code":"SELECTION_EXPANDED","message":"Se a\u00f1adieron 63 barras que tocan la selecci\u00f3n: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cord\u00f3n.","hint":"El plan se calcul\u00f3 con la selecci\u00f3n ampliada (selection_count las cuenta; data.selection_expansion dice cu\u00e1les); sin expand_selection se planifica solo lo seleccionado."}],"meta":{"duration_ms":1152,"addin_version":"0.13.0","operation":"batch_plan"},"data":{"summary":{"ready":41,"after_split":31,"no_match":8,"untyped":17},"created_count":0,"selection_expansion":{"splice_count":14,"requested_count":1,"limit_reached":false,"added_element_ids":[1249630,1249631,1249632,1249633,1249636,1249637,1245533,1245532,1249626,1249638,1245535,1249627,1249628,1249629,1250263,1249509,1245537,1250264,1250265,1250266,1250269,1249639,1249511,1250267,1245539,1250270,1250271,1250272,1250275,1250268,1249512,1250273,1245541,1250276,1250277,1250278,1250281,1250274,1249513,1250279,1245543,1250282,1250283,1250284,1250287,1250280,1249514,1250285,1245545,1250288,1250289,1250290,1250293,1250286,1249515,1250291,1250294,1250295,1250296,1250292,1249516,1250297,1250298],"member_count":42,"rounds":11,"chord_count":7,"summary_text":"Se a\u00f1adieron 63 barras que tocan la selecci\u00f3n: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cord\u00f3n.","skipped_out_of_plane":[],"ignored_element_ids":[],"added_count":63},"summary_text":"Se crear\u00e1n 4 ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 179216b5-3523-4c5f-afaa-7aa2a42b163d etiquetas quitadas=0
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 11, "addin_version": "0.13.0", "operation": "batch_plan_discard"}, "data": {"remaining_plans": 1, "discarded_plan_id": "179216b5-3523-4c5f-afaa-7aa2a42b163d", "remaining_markers": 0, "removed_marks": 0, "remaining_labels": 0, "removed_labels": 0}, "ok": true}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 5, "addin_version": "0.13.0", "operation": "validate"}, "data": {"warnings_count": 0, "errors_count": 0, "is_valid": true, "bolt_stacks": [{"bolt_length_mm": 50.799999999999997, "gap_mm": 0, "plates": 2, "grip_mm": 25.399999999999999, "member_element_id": 1245531, "length_source": "computed_from_grip", "bolts": 12}], "calculated_values": {"outline_vertices": null, "frame_x": [1, 0, 0], "origin_mm": [-4180.5, 17204.200000000001, 19933], "axis_distance_mm": 0, "outline_mode": null, "outline_width_mm": null, "frame_y": [0, 0, 1], "eccentricity_mm": 0, "frame_z": [0, -1, 0], "kind": "splice", "outline_points_mm": null, "chord_direction_reversed": true, "outline_height_mm": null}, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8"}, "ok": true}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].axis_offset_mm","code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE."},{"path":"members[1].axis_offset_mm","code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE."}],"meta":{"duration_ms":7,"addin_version":"0.13.0","operation":"validate"},"data":{"warnings_count":2,"errors_count":0,"is_valid":true,"bolt_stacks":[],"calculated_values":{"outline_vertices":null,"frame_x":[1,0,0],"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"axis_distance_mm":0.089999999999999997,"outline_mode":null,"outline_width_mm":null,"frame_y":[0,0,1],"eccentricity_mm":54.899999999999999,"frame_z":[0,-1,0],"kind":"node","outline_points_mm":null,"chord_direction_reversed":true,"outline_height_mm":null},"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9"},"o ...
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo (arranque) arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":21,"addin_version":"0.13.0","operation":"batch_plan"},"data":{"summary":{"ready":1,"untyped":2},"created_count":0,"selection_expansion":null,"summary_text":"Se crear\u00e1 1 conexi\u00f3n: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.","hidden_text":"2 barras sueltas","visible_count":1,"has_batch_connections":false,"is_marked":false,"failed_count":0,"marks":{"marker_element_ids":[],"element_count":0,"ghost_count":0},"split_step_count":0,"unused_element_ids":[],"templates":{"91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior","6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D"},"overrides":{"add_node":{},"replace_existing":false,"spec":{},"split":{},"chord":{},"add_member":{},"exclude":[],"template":{},"remove_member":{},"merge":[]},"label_view_id":null,"last_report":null,"plan_id":"31fa7e95-1a23-435f-ab9a-9bcc21dbf739","created_utc":"2026-10-31T16:07:37.3508919Z","labels":{"view_id":null,"count":0},"ready_count":1,"marked_view_id":null,"selection_count":2,"created_in_batch_count":0,"description":"3 nudo(s): 2 untyped, 1 ready.","updated_utc":"2026-10-31T16:07:37.3509153Z","after_split_count":0,"creatable_count":1,"document":"HANGAR_PRUEBA_sondeo","label_indices":{},"nodes":[{"splice_element_id":null,"is_valid":false,"existing_connection_id":null,"warnings":[],"through_element_ids":[],"split_steps":[],"color_name":null,"ghost_element_id":null,"attempts":[],"replaces_existing":false,"is_marked": ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado 31fa7e95-1a23-435f-ab9a-9bcc21dbf739
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 10, "addin_version": "0.13.0", "operation": "batch_plan_discard"}, "data": {"remaining_plans": 1, "discarded_plan_id": "31fa7e95-1a23-435f-ab9a-9bcc21dbf739", "remaining_markers": 0, "removed_marks": 0, "remaining_labels": 0, "removed_labels": 0}, "ok": true}
======================================================================
31. POST /conn/validate/ del Detalle D con gusset.outline.mode auto (sin puntos) -> válido, calculated_values.outline_points_mm y la caja (Fase 13)  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH'], 8 vértices, caja 540 × 585 mm, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"gusset.width_mm","code":"LABEL_VALUE_MISMATCH","message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00e9jalo: es informativo con outline.mode auto."},{"path":"gusset.height_mm","code":"LABEL_VALUE_MISMATCH","message":"gusset.height_mm (530.0 mm) no coincide con el alto de la cartela autom\u00e1tica calculada (585.0 mm): manda el calculado.","hint":"Escribe el alto calculado (calculated_values.outline_height_mm) o d\u00e9jalo: es informativo con outline.mode auto."}],"meta":{"duration_ms":12,"addin_version":"0.13.0","operation":"validate"},"data":{"warnings_count":3,"errors_count":0,"is_valid":true,"bolt_stacks":[{"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip","member_element_id":1249636}],"calculated_values":{"outline_vertices":8,"frame_x":[1,0,0],"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"axis_distance_mm":0.080000000000000002,"outline_mode":"auto","outl ...
======================================================================
31b. POST /conn/preview/ con outline.mode auto -> gusset_plate con outline_mode auto y los mismos puntos que validate (Fase 13)  [OK]  HTTP 200, ok=True, cartela con 8 vértices (los mismos que validate: True)
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 11, "addin_version": "0.13.0", "operation": "preview"}, "data": {"members_to_modify": [{"role": "diagonal", "setback_mm": 180, "end": "end", "new_extension_mm": -95.099999999999994, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 84.900000000000006, "element_id": 1249630}, {"role": "diagonal", "setback_mm": 60, "end": "start", "new_extension_mm": -40.799999999999997, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 19.199999999999999, "element_id": 1249631}, {"role": "diagonal", "setback_mm": 260, "end": "end", "new_extension_mm": -211.40000000000001, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 48.600000000000001, "element_id": 1249636}], "summary": {"weld_lines": 6, "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "backend": "advancesteel", "members_modified": 3, "chord_element_id": 1249510, "first_member_element_id": 1249630, "knife_plates": 1, "connection_type": "gusset_node", "gusset_plates": 1, "dry_run": true, "bolts": 4}, "calculated_values": {"outline_vertices": 8, "frame_x": [1, 0, 0], "origin_mm": [-11869.400000000000, -17195.799999999999,  ...
======================================================================
31c. POST /conn/validate/ con outline.mode auto Y points_mm -> SCHEMA_INVALID (Fase 13)  [OK]  HTTP 200, ok=False, errores=['SCHEMA_INVALID'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'LABEL_VALUE_MISMATCH', 'LABEL_VALUE_MISMATCH']
Cuerpo:
{"errors":[{"path":"gusset.outline.points_mm","code":"SCHEMA_INVALID","message":"outline.mode \"auto\" no lleva points_mm: el contorno lo calcula el add-in (usa polygon si quieres fijar los puntos).","hint":"Quita points_mm (o pon null) para la cartela autom\u00e1tica, o cambia mode a \"polygon\"."}],"warnings":[{"path":"members[0].expected_angle_deg","code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"gusset.width_mm","code":"LABEL_VALUE_MISMATCH","message":"gusset.width_mm (565.0 mm) no coincide con el ancho de la cartela autom\u00e1tica calculada (540.0 mm): manda el calculado.","hint":"Escribe el ancho calculado (calculated_values.outline_width_mm) o d\u00e9jalo: es informativo con outline.mode auto."},{"path":"gusset.height_mm","code":"LABEL_VALUE_MISMATCH","message":"gusset.height_mm (530.0 mm) no coincide con el alto de la cartela autom\u00e1tica calculada (585.0 mm): manda el calculado.","hint":"Escribe el alto calculado (calculated_values.outline_height_mm) o d\u00e9jalo: es informativo con outline.mode auto."}],"meta":{"duration_ms":16,"addin_version":"0.13.0","operation":"validate"},"data":null,"ok":false}
======================================================================
32. POST /conn/split/ dry_run de la diagonal 1249632 contra el cordón 1245533 -> el punto del Detalle B (≈ 485 mm) sin tocar nada (Fase 13, D7)  [OK]  HTTP 200, ok=True, θ=45 cara=143.7 primera plancha=473.7 corte=483.2 mm (corto None, largo None)
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":7,"addin_version":"0.13.0","operation":"split"},"data":{"preview":{"chord_type":"HSS12X8X1/2","long_segment_mm":3024.4000000000001,"face_distance_mm":143.69999999999999,"member_type":"HSS2-1-2X2-1-2X3-16 64x64","gap_mm":0,"theta_deg":45,"chord_element_id":1245533,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"offset_from_chord_face_mm":330,"first_plate_distance_mm":473.69999999999999,"split_distance_mm":483.19999999999999,"plate_thickness_mm":9.5250000000000004,"cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"element_id":1249632,"short_segment_mm":521.60000000000002},"dry_run":true,"description":"La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cord\u00f3n 1245533 a 45,0\u00b0: la cara del cord\u00f3n queda a 144 mm por su eje, la primera plancha a 474 y el corte a 483 mm (tramo corto 522 mm con el id original, tramo largo 3024 mm nuevo),","already_split":null},"ok":true}
======================================================================
32b. POST /conn/node_info/ del nudo en K -> split_point_mm y split_distance_mm por diagonal (Fase 13)  [OK]  HTTP 200, ok=True, [(1245533, None), (1249631, 484.8), (1249632, 483.2)]
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 8, "addin_version": "0.13.0", "operation": "node_info"}, "data": {"origin_mm": [-9278.1000000000004, -17195.700000000001, 19933], "members": [{"split_point_mm": null, "end_mm": [-14655.200000000001, -17195.700000000001, 19933], "angle_in_plane_deg": -180, "axis_offset_mm": 0, "node_end": 0, "angle_to_chord_deg": 0, "is_chord": true, "reaches_node": true, "width_mm": 203.19999999999999, "family": "HSS-Hollow Structural Section", "material": "Steel ASTM A500, Grade B, Rectangular and Square", "structural_type": "Beam", "element_id": 1245533, "height_mm": 304.80000000000001, "end_gap_mm": 0, "split_offset_from_chord_face_mm": null, "length_mm": 10475, "split_distance_mm": null, "slope_deg": 0, "side": "chord", "axis_cut_mm": [-9278.1000000000004, -17195.700000000001, 19933], "wall_mm": 11.810000000000000, "section_rotation_deg": 90, "type": "HSS12X8X1/2", "start_mm": [-4180.1999999999998, -17195.799999999999, 19933]}, {"split_point_mm": [-9652.1000000000004, -17195.700000000001, 19594], "end_mm": [-9354.7000000000007, -17195.799999999999, 19884.900000000001], "angle_in_plane_deg": -135.59999999999999, "axis_offset_mm": -27.399999999999999, "node_end": 1, "angle_to_chord_deg": 44.399999999999999, "is_chord": false, "reaches_node": true, "width_mm": 63.5, "family": "HSS2-1-2X2-1-2X3-16 64x64", "material": "Material IFC (190-40-140)", "structural_type": "Beam", "element_id": 1249631, "height_mm": 63.5, "end_gap_mm": 90.40000000 ...
======================================================================
32c. POST /conn/split/ REAL de la diagonal 1249632 -> split_id y new_element_id (ESCRIBE: se deshace en la 32e) (Fase 13)  [OK]  HTTP 200, ok=True, avisos=['REVIT_WARNING'], split_id=7c22df3b-775f-4883-86e2-d8226d1deec7 tramo nuevo=1322779 mecanismo=copy_element avisos=['REVIT_WARNING']
Cuerpo:
{"errors":[],"warnings":[{"path":null,"code":"REVIT_WARNING","message":"Advertencia de Revit: Beam or Brace is slightly off axis and may cause inaccuracies. [elementos: 1322779]","hint":null}],"meta":{"duration_ms":99,"addin_version":"0.13.0","operation":"split"},"data":{"preview":{"chord_type":"HSS12X8X1/2","long_segment_mm":3024.4000000000001,"face_distance_mm":143.69999999999999,"member_type":"HSS2-1-2X2-1-2X3-16 64x64","gap_mm":0,"theta_deg":45,"chord_element_id":1245533,"split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"offset_from_chord_face_mm":330,"first_plate_distance_mm":473.69999999999999,"split_distance_mm":483.19999999999999,"plate_thickness_mm":9.5250000000000004,"cut_point_mm":[-9250.7000000000007,-17195.799999999999,19933],"element_id":1249632,"short_segment_mm":521.60000000000002},"split":{"approximated_parameters":[],"batch_id":null,"created_utc":"2026-10-31T16:07:37.6958878Z","split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"chord_element_id":1245533,"new_element_id":1322779,"offset_from_chord_face_mm":330,"split_distance_mm":483.19999999999999,"connection_id":null,"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"element_id":1249632,"split_id":"7c22df3b-775f-4883-86e2-d8226d1deec7","mechanism":"copy_element","original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001]},"description":"La barra 1249632 (HSS2-1-2X2-1-2X3-16 64x64) llega al cord\u00f3n 1245533 a 45,0\u00b0: la cara  ...
======================================================================
32d. POST /conn/node_info/ de los dos tramos -> kind splice (el empalme B se puede aplicar) (Fase 13)  [OK]  HTTP 200, ok=True, kind=splice barras=[1249632, 1322779]
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 9, "addin_version": "0.13.0", "operation": "node_info"}, "data": {"origin_mm": [-8909, -17195.799999999999, 19591.299999999999], "members": [{"split_point_mm": null, "end_mm": [-8909, -17195.799999999999, 19591.299999999999], "angle_in_plane_deg": -180, "axis_offset_mm": 0, "node_end": 1, "angle_to_chord_deg": 0, "is_chord": false, "reaches_node": true, "width_mm": 63.5, "family": "HSS2-1-2X2-1-2X3-16 64x64", "material": "Material IFC (190-40-140)", "structural_type": "Beam", "element_id": 1249632, "height_mm": 63.5, "end_gap_mm": 0, "split_offset_from_chord_face_mm": null, "length_mm": 521.60000000000002, "split_distance_mm": null, "slope_deg": 45, "side": "-Y", "axis_cut_mm": [-8909, -17195.799999999999, 19591.299999999999], "wall_mm": 0, "section_rotation_deg": -180, "type": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-9277.7999999999993, -17195.799999999999, 19960.099999999999]}, {"split_point_mm": null, "end_mm": [-6770.3999999999996, -17195.799999999999, 17452.700000000001], "angle_in_plane_deg": 0, "axis_offset_mm": 0, "node_end": 0, "angle_to_chord_deg": 0, "is_chord": true, "reaches_node": true, "width_mm": 63.5, "family": "HSS2-1-2X2-1-2X3-16 64x64", "material": "Material IFC (190-40-140)", "structural_type": "Beam", "element_id": 1322779, "height_mm": 63.5, "end_gap_mm": 0, "split_offset_from_chord_face_mm": null, "length_mm": 3024.4000000000001, "split_distance_mm": null, "slope_deg": 45, "side": "chord", "axis_ ...
======================================================================
32e. POST /conn/unsplit/ por split_id -> deshecho, la diagonal vuelve a ser una (Fase 13, D8)  [OK]  HTTP 200, ok=True, Corte deshecho: el tramo 1322779 se borró y la barra 1249632 recuperó su curva. avisos=[]
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":61,"addin_version":"0.13.0","operation":"unsplit"},"data":{"complete":true,"unsplit":{"approximated_parameters":[],"batch_id":null,"created_utc":"2026-10-31T16:07:37.6958878Z","split_point_mm":[-8909,-17195.799999999999,19591.299999999999],"chord_element_id":1245533,"new_element_id":1322779,"offset_from_chord_face_mm":330,"split_distance_mm":483.19999999999999,"connection_id":null,"original_start_mm":[-9277.7999999999993,-17195.799999999999,19960.099999999999],"element_id":1249632,"split_id":"7c22df3b-775f-4883-86e2-d8226d1deec7","mechanism":"copy_element","original_end_mm":[-6770.3999999999996,-17195.799999999999,17452.700000000001]},"description":"Corte deshecho: el tramo 1322779 se borr\u00f3 y la barra 1249632 recuper\u00f3 su curva."},"ok":true}
======================================================================
32f. POST /conn/unsplit/ de la barra ya entera -> SPLIT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['SPLIT_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"element_id","code":"SPLIT_NOT_FOUND","message":"La barra 1249632 no est\u00e1 partida por el add-in (ning\u00fan corte la tiene como tramo).","hint":"conn_unsplit sin par\u00e1metros lista los cortes del modelo."}],"warnings":[],"meta":{"duration_ms":4,"addin_version":"0.13.0","operation":"unsplit"},"data":null,"ok":false}
======================================================================
33. POST /conn/batch/plan/ (mark:false) del nudo en K -> split_steps por diagonal y los empalmes B previstos (after_split) detrás (Fase 13, D5)  [OK]  HTTP 200, ok=True, pasos=[(1249631, 484.8), (1249632, 483.2)] previstos=['● Empalme B (tras partir)', '● Empalme B (tras partir)'] | Se creará 1 conexión con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Además se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.
Cuerpo:
{"errors":[],"warnings":[],"meta":{"duration_ms":25,"addin_version":"0.13.0","operation":"batch_plan"},"data":{"summary":{"ready":1,"after_split":2,"untyped":4},"created_count":0,"selection_expansion":null,"summary_text":"Se crear\u00e1 1 conexi\u00f3n con Nudo K cordon superior (1 igual, 0 en espejo). 1 avisa de excentricidad. Adem\u00e1s se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K. Ocultos: 4 barras sueltas.","hidden_text":"4 barras sueltas","visible_count":3,"has_batch_connections":false,"is_marked":false,"failed_count":0,"marks":{"marker_element_ids":[],"element_count":0,"ghost_count":0},"split_step_count":2,"unused_element_ids":[],"templates":{"91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior","6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D"},"overrides":{"add_node":{},"replace_existing":false,"spec":{},"split":{},"chord":{},"add_member":{},"exclude":[],"template":{},"remove_member":{},"merge":[]},"label_view_id":null,"last_report":null,"plan_id":"53f245ea-3764-4817-9685-88944afbdcf8","created_utc":"2026-10-31T16:07:38.0848606Z","labels":{"view_id":null,"count":0},"ready_count":1,"marked_view_id":null,"selection_count":3,"created_in_batch_count":0,"description":"7 nudo(s): 4 untyped, 2 after_split, 1 ready.","updated_utc":"2026-10-31T16:07:38.0848841Z","after_split_count":2,"creatable_count":1,"document":"HANGAR_PRUEBA_sondeo","label_indices":{},"nodes":[{"splice_element_id":null,"is_valid":false,"ex ...
======================================================================
33b. POST /conn/batch/plan/discard/ del plan del nudo en K -> descartado  [OK]  HTTP 200, ok=True, descartado
Cuerpo:
{"errors": [], "warnings": [], "meta": {"duration_ms": 20, "addin_version": "0.13.0", "operation": "batch_plan_discard"}, "data": {"remaining_plans": 1, "discarded_plan_id": "53f245ea-3764-4817-9685-88944afbdcf8", "remaining_markers": 0, "removed_marks": 0, "remaining_labels": 0, "removed_labels": 0}, "ok": true}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=91 conn_*=25
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete, conn_split_member, conn_unsplit
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.13.0
Cuerpo:
{
  "errors": [],
  "warnings": [],
  "meta": {
    "duration_ms": 2,
    "addin_version": "0.13.0",
    "operation": "ping"
  },
  "data": {
    "spec_version": "1.0",
    "document": {
      "title": "HANGAR_PRUEBA_sondeo",
      "is_workshared": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_modifiable": false,
      "is_family": false,
      "is_read_only": false
    },
    "revit": {
      "version_name": "Autodesk Revit 2027",
      "language": "English_USA",
      "version_number": "2027",
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39"
    },
    "dotnet": {
      "framework": ".NET 10.0.12",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "load_context": "Default"
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
    "addin_version": "0.13.0",
    "has_uidocument": true
  },
  "ok": true
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "errors": [],
  "warnings": [],
  "meta": {
    "duration_ms": 17,
    "addin_version": "0.13.0",
    "operation": "catalog_apply"
  },
  "data": {
    "is_valid": true,
    "spec": {
      "spec_version": "1.0",
      "members": [
        {
          "role": "diagonal",
          "end_setback_mm": 180.0,
          "expected_angle_deg": 43.1,
          "profile": "HSS2-1/2X2-1/2X3/16",
          "attachment": {
            "type": "welded_slot",
            "weld": {
              "type": "fillet",
              "all_around": true,
              "size_mm": 5.0
            },
            "slot_length_mm": 150.0
          },
          "element_id": 1249630
        },
        {
          "role": "diagonal",
          "end_setback_mm": 60.0,
          "expected_angle_deg": 44.4,
          "profile": "HSS2-1/2X2-1/2X3/16",
          "attachment": {
            "type": "welded_slot",
            "weld": {
              "type": "fillet",
              "all_around": true,
              "size_mm": 5.0
            },
            "slot_length_mm": 150.0
          },
          "element_id": 1249631
        },
        {
          "role": "diagonal",
          "end_setback_mm": 260.0,
          "expected_angle_deg": 44.4,
          "profile": "HSS2-1/2X2-1/2X3/16",
          "attachment": {
            "type": "bolted_knife_plate",
            "bolts": {
              "first_row_from_plate_end_mm": 40.0,
              "edge_mm": 40.0,
              "rows": 2,
              "columns": 2 ...
======================================================================
Resultado: 49/49 pruebas correctas

```

## 13-10 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 408 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "meta":  {
                 "duration_ms":  1,
                 "addin_version":  "0.13.0",
                 "operation":  "catalog_list"
             },
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "members_count":  2,
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "name":  "Nudo K cordon superior",
                                       "tags":  [

                                                ],
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "chord_profile":  "HSS12X8X1/2",
                                       "connection_type":  "direct_weld",
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "description":  null
                                   },
                                   {
                                       "members_count":  3,
                                       "origin_drawing":  "Detalle D",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "connection_type":  "gusset_node",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ],
                 "templates_count":  2
             },
    "ok":  true
}

```

## 13-10 log failed

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



```

## 13-10 log fase 13

```text

{"ts":"2026-10-31T11:07:15.4021888-05:00","record":{"event":"split_member","element_id":1249632,"chord_element_id":1245
533,"new_element_id":1322773,"split_id":"7bc207c7-b742-48aa-a7bf-580c59f6c29d","mechanism":"copy_element","duration_ms"
:95}}
{"ts":"2026-10-31T11:07:15.4027403-05:00","record":{"event":"handle","operation":"split","request_summary":"{\"element_
id\": 1249632, \"chord_element_id\": 
1245533}","ok":true,"error_codes":[],"warning_codes":["REVIT_WARNING"],"duration_ms":105}}
{"ts":"2026-10-31T11:07:15.5246692-05:00","record":{"event":"member_unsplit","element_id":1249632,"new_element_id":1322
773,"problems":[]}}
{"ts":"2026-10-31T11:07:15.5681776-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
773,"split_id":"7bc207c7-b742-48aa-a7bf-580c59f6c29d","complete":true}}
{"ts":"2026-10-31T11:07:15.5687146-05:00","record":{"event":"handle","operation":"unsplit","request_summary":"{\"split_
id\": \"7bc207c7-b742-48aa-a7bf-580c59f6c29d\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":56}}
{"ts":"2026-10-31T11:07:15.6315641-05:00","record":{"event":"handle","operation":"unsplit","request_summary":"{\"elemen
t_id\": 1249632}","ok":false,"error_codes":["SPLIT_NOT_FOUND"],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-10-31T11:07:15.6997906-05:00","record":{"event":"batch_plan","plan_id":"6952890b-e898-4505-ae79-a13a8ecf938
b","replan":false,"element_ids":3,"expanded":null,"templates":["91adac27-d196-4c98-88db-6579aa991be4","6abcf116-9b97-48
5f-b50d-2851ca0018cc"],"summary":{"untyped":4,"ready":1,"after_split":2},"marked":false,"labels":0,"ghosts":0,"override
s":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"
split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-31T11:07:35.3212559-05:00","record":{"event":"batch_plan","plan_id":"fd9979c8-9f6f-40f0-a201-c4232b5ca24
a","replan":false,"element_ids":4,"expanded":null,"templates":["2b644c11-b47e-4949-a397-79d37bb71541"],"summary":{"unty
ped":5,"ready":1},"marked":false,"labels":0,"ghosts":0,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"temp
late\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-31T11:07:35.4190508-05:00","record":{"event":"batch_create","plan_id":"fd9979c8-9f6f-40f0-a201-c4232b5ca
24a","requested":1,"created":0,"updated":0,"split":0,"failed":1,"skipped":0,"rolled_back":0,"stop_on_error":false,"stop
ped":false,"undo_entries":"one","connection_ids":[],"duration_ms":0,"summary":"Lote fd9979c8: 0 conexiones creadas, 1 
fallÃ³ (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificÃ³ (o 
el token es de otro nudo))."}}
{"ts":"2026-10-31T11:07:36.7280931-05:00","record":{"event":"batch_plan","plan_id":"179216b5-3523-4c5f-afaa-7aa2a42b163
d","replan":false,"element_ids":64,"expanded":63,"templates":["91adac27-d196-4c98-88db-6579aa991be4","6abcf116-9b97-485
f-b50d-2851ca0018cc"],"summary":{"no_match":8,"untyped":17,"ready":41,"after_split":31},"marked":false,"labels":0,"ghos
ts":0,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},
\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-31T11:07:37.3651736-05:00","record":{"event":"batch_plan","plan_id":"31fa7e95-1a23-435f-ab9a-9bcc21dbf73
9","replan":false,"element_ids":2,"expanded":null,"templates":["91adac27-d196-4c98-88db-6579aa991be4","6abcf116-9b97-48
5f-b50d-2851ca0018cc"],"summary":{"untyped":2,"ready":1},"marked":false,"labels":0,"ghosts":0,"overrides":"{\"exclude\"
:[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spe
c\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-31T11:07:37.6180398-05:00","record":{"event":"handle","operation":"split","request_summary":"{\"dry_run\
": true, \"element_id\": 1249632, \"chord_element_id\": 
1245533}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":7}}
{"ts":"2026-10-31T11:07:37.7155486-05:00","record":{"event":"member_split","element_id":1249632,"new_element_id":132277
9,"mechanism":"copy_element","split_point_mm":[-8908.9765873183,-17195.789149833254,19591.319569910534],"split_distance
_mm":483.209,"short_mm":521.6,"long_mm":3024.4,"approximated":[]}}
{"ts":"2026-10-31T11:07:37.7730213-05:00","record":{"event":"split_member","element_id":1249632,"chord_element_id":1245
533,"new_element_id":1322779,"split_id":"7c22df3b-775f-4883-86e2-d8226d1deec7","mechanism":"copy_element","duration_ms"
:84}}
{"ts":"2026-10-31T11:07:37.7736770-05:00","record":{"event":"handle","operation":"split","request_summary":"{\"element_
id\": 1249632, \"chord_element_id\": 
1245533}","ok":true,"error_codes":[],"warning_codes":["REVIT_WARNING"],"duration_ms":99}}
{"ts":"2026-10-31T11:07:37.8977838-05:00","record":{"event":"member_unsplit","element_id":1249632,"new_element_id":1322
779,"problems":[]}}
{"ts":"2026-10-31T11:07:37.9358243-05:00","record":{"event":"unsplit_member","element_id":1249632,"new_element_id":1322
779,"split_id":"7c22df3b-775f-4883-86e2-d8226d1deec7","complete":true}}
{"ts":"2026-10-31T11:07:37.9455106-05:00","record":{"event":"handle","operation":"unsplit","request_summary":"{\"split_
id\": \"7c22df3b-775f-4883-86e2-d8226d1deec7\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":61}}
{"ts":"2026-10-31T11:07:38.0099736-05:00","record":{"event":"handle","operation":"unsplit","request_summary":"{\"elemen
t_id\": 1249632}","ok":false,"error_codes":["SPLIT_NOT_FOUND"],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-10-31T11:07:38.1029913-05:00","record":{"event":"batch_plan","plan_id":"53f245ea-3764-4817-9685-88944afbdcf
8","replan":false,"element_ids":3,"expanded":null,"templates":["91adac27-d196-4c98-88db-6579aa991be4","6abcf116-9b97-48
5f-b50d-2851ca0018cc"],"summary":{"untyped":4,"ready":1,"after_split":2},"marked":false,"labels":0,"ghosts":0,"override
s":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"
split\":{},\"spec\":{},\"replace_existing\":false}"}}



```
