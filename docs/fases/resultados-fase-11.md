# Resultados de la Fase 11

Fecha: 2026-10-12T09:27:33


## 11-1 git

```text
9074dd2 Fase 11: conectar un nudo sin JSON (bot├│n Conectar, panel por secciones, miniaturas del cat├ílogo, plano al lado), add-in 0.11.0
?? docs/fases/resultados-fase-11.md

```

## 11-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.43 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.43 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.43 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:10.37
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:   238, Omitido:     0, Total:   238, Duraci├│n: 483 ms - MotorConexiones.Tests.dll (net10.0)

```

## 11-1 revit cerrado

```text

```

## 11-1 deploy

```text
== MotorConexiones 0.11.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 11-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 11-1 version de la dll

```text
0.11.0.0

```

## 11-1 choices.json desplegado

```text

  "_comentario": "Fase 11: listas de los desplegables del panel por secciones de la previsualizaciÃ³n y valores por 
defecto de la especificaciÃ³n de arranque del botÃ³n Conectar. Editable sin recompilar: scripts\\deploy.ps1 lo copia a 
config\\ del add-in. Los rÃ³tulos se convierten a mm con las mismas reglas que el validador (3/8\" = 9,525 mm; PL10 = 
10 mm). bolt_diameter_labels: solo se ofrecen los que existen en limits.json (bolts.edge_distance_mm); los demÃ¡s 
diÃ¡metros de limits.json siguen aceptados escribiÃ©ndolos en la pestaÃ±a Avanzado. Un valor del JSON que no estÃ© en 
la lista se enseÃ±a como 'Otro: 9,2 mm' y no se pierde.",
  "bolt_diameter_labels": ["1/2\"", "5/8\"", "3/4\"", "7/8\""],
  "slot_length_mm": 150,
    "gusset_thickness_label": "3/8\"",



```

## 11-2 ping

```text
== conn/ping -> HTTP 200 en 312 ms ==
{
    "meta":  {
                 "duration_ms":  9,
                 "operation":  "ping",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "has_uidocument":  true,
                 "document":  {
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false,
                                  "is_workshared":  false,
                                  "is_modifiable":  false
                              },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
                            },
                 "addin_version":  "0.11.0",
                 "revit":  {
                               "sub_version_number":  "2027.2",
                               "language":  "English_USA",
                               "version_build":  "27.2.0.39",
                               "version_number":  "2027",
                               "version_name":  "Autodesk Revit 2027"
                           },
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
                 "spec_version":  "1.0",
                 "backend":  "advancesteel"
             },
    "errors":  [

               ]
}

```

## 11-2 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 67 ms ==
{
    "meta":  {
                 "duration_ms":  26,
                 "operation":  "catalog_list",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "members_count":  3,
                                       "origin_drawing":  "Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "name":  "Nudo tipico Detalle D",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "connection_type":  "gusset_node",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "errors":  [

               ]
}

```

## 11-2 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 184 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 16 ms | errores=- | avisos=-
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

## 11-2 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 42 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 11-2 sondeo 17 marcadores

```text
== 17-marcas-plan.py -> HTTP 200 en 882 ms ==
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

## 11-2 sondeo 21 etiquetas

```text
== 21-etiquetas-addin.py -> HTTP 200 en 241 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 0
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 41 BMP
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

## 11-2 log de arranque

```text

{"ts":"2026-10-12T10:10:20.6019705-05:00","record":{"event":"ribbon_panel_created","tab":"ARBA","panel":"MotorConexione
s","tab_already_existed":true,"create_tab_error":"ArgumentException: The tab with the input name exists 
already.\r\nParameter name: tabName"}}
{"ts":"2026-10-12T10:10:20.6193478-05:00","record":{"event":"startup","addin_version":"0.11.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}



```

## 11-3 list tras crear

```text
== conn/list -> HTTP 200 en 434 ms ==
{
    "meta":  {
                 "duration_ms":  6,
                 "operation":  "list",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "batch_id":  null,
                 "connections":  [
                                     {
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                         "batch_id":  null,
                                         "created_elements_count":  9,
                                         "connection_id":  "fea87f0f-5231-4f0b-a4ae-c093980de1f4",
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-12T15:31:54.1458642Z",
                                         "connection_type":  "gusset_node",
                                         "backend":  "advancesteel"
                                     }
                                 ],
                 "connections_count":  1,
                 "total_count":  1,
                 "batches":  {

                             }
             },
    "errors":  [

               ]
}

```

## 11-5 list tras el arranque

```text
== conn/list -> HTTP 200 en 434 ms ==
{
    "meta":  {
                 "duration_ms":  5,
                 "operation":  "list",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "batch_id":  null,
                 "connections":  [
                                     {
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                         "batch_id":  null,
                                         "created_elements_count":  9,
                                         "connection_id":  "fea87f0f-5231-4f0b-a4ae-c093980de1f4",
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-12T15:31:54.1458642Z",
                                         "connection_type":  "gusset_node",
                                         "backend":  "advancesteel"
                                     }
                                 ],
                 "connections_count":  1,
                 "total_count":  1,
                 "batches":  {

                             }
             },
    "errors":  [

               ]
}

```

## 11-6 catalogo dir

```text

Name                                      Length LastWriteTime      
----                                      ------ -------------      
6abcf116-9b97-485f-b50d-2851ca0018cc.json   5258 4/10/2026 17:01:50 
6abcf116-9b97-485f-b50d-2851ca0018cc.png   10338 12/10/2026 10:28:55



```

## 11-7 label_clicked

```text

{"ts":"2026-10-12T10:55:20.1248547-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:55:20.1254601-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:55:20.5098771-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:55:20.5105044-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}



```

## 11-7 sondeo 17 tras descartar

```text
== 17-marcas-plan.py -> HTTP 200 en 822 ms ==
=== 17-marcas-plan ===
1) Vista activa: Section 2 (Section) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322661] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1321647; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17-exportada.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 11-7 sondeo 21 tras descartar

```text
== 21-etiquetas-addin.py -> HTTP 200 en 18398 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: Section 2 (Section)
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

## 11-8 catalog_apply sin template_id

```text
== conn/catalog_apply -> HTTP 200 en 490 ms ==
{
    "meta":  {
                 "duration_ms":  42,
                 "operation":  "catalog_apply",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                 "is_valid":  true,
                 "bolt_stacks":  [
                                     {
                                         "grip_mm":  19.524999999999999,
                                         "bolt_length_mm":  44.450000000000003,
                                         "gusset_face":  "+z",
                                         "member_element_id":  1249636,
                                         "length_source":  "computed_from_grip"
                                     }
                                 ],
                 "spec":  {
                              "dimension_chains":  [
                                                       {
                                                           "values_mm":  [
                                                                             75.0,
                                                                             420.0,
                                                                             70.0
                                                                         ],
                                                           "label":  "borde superior",
                                                           "expected_total_mm":  565.0
                                                       },
                                                       {
                                                           "values_mm":  [
                                                                             125.0,
                                                                             90.0,
                                                                             350.0
                                                                         ],
                                                           "label":  "base",
                                                           "expected_total_mm":  565.0
                                                       },
                                                       {
                                                           "values_mm":  [
                                                                             70.0,
                                                                             250.0,
                                                                             210.0
                                                                         ],
                                                           "label":  "lado derecho",
                                                           "expected_total_mm":  530.0
                                                       },
                                                       {
                                                           "values_mm":  [
                                                                             70.0,
                                                                             325.0,
                                                                             135.0
                                                                         ],
                                                           "label":  "lado izquierdo",
                                                           "expected_total_mm":  530.0
                                                       }
                                                   ],
                              "source":  {
                                             "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                             "drawing":  "Detalle D",
                                             "scale":  "1/10"
                                         },
                              "members":  [
                                              {
                                                  "end_setback_mm":  180.0,
                                                  "expected_angle_deg":  43.100000000000001,
                                                  "element_id":  1249630,
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "role":  "diagonal",
                                                  "attachment":  {
                                                                     "slot_length_mm":  150.0,
                                                                     "weld":  {
                                                                                  "all_around":  true,
                                                                                  "size_mm":  5.0,
                                                                                  "type":  "fillet"
                                                                              },
                                                                     "type":  "welded_slot"
                                                                 }
                                              },
                                              {
                                                  "end_setback_mm":  60.0,
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "element_id":  1249631,
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "role":  "diagonal",
                                                  "attachment":  {
                                                                     "slot_length_mm":  150.0,
                                                                     "weld":  {
                                                                                  "all_around":  true,
                                                                                  "size_mm":  5.0,
                                                                                  "type":  "fillet"
                                                                              },
                                                                     "type":  "welded_slot"
                                                                 }
                                              },
                                              {
                                                  "end_setback_mm":  260.0,
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "element_id":  1249636,
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "role":  "diagonal",
                                                  "attachment":  {
                                                                     "plate":  {
                                                                                   "thickness_label":  "PL10",
                                                                                   "thickness_mm":  10.0,
                                                                                   "length_mm":  170.0,
                                                                                   "insertion_mm":  80.0,
                                                                                   "width_mm":  140.0
                                                                               },
                                                                     "bolts":  {
                                                                                   "columns":  2,
                                                                                   "rows":  2,
                                                                                   "edge_mm":  40.0,
                                                                                   "first_row_from_plate_end_mm":  40.0,
                                                                                   "spacing_mm":  60.0,
                                                                                   "diameter_label":  "5/8\"",
                                                                                   "diameter_mm":  15.875
                                                                               },
                                                                     "weld_plate_to_member":  {
                                                                                                  "all_around":  true,
                                                                                                  "size_mm":  5.0,
                                                                                                  "type":  "fillet"
                                                                                              },
                                                                     "type":  "bolted_knife_plate"
                                                                 }
                                              }
                                          ],
                              "chord":  {
                                            "element_id":  1249510,
                                            "profile":  "HSS3X3X1/4",
                                            "continuous":  true
                                        },
                              "gusset":  {
                                             "thickness_label":  "3/8\"",
                                             "thickness_mm":  9.5250000000000004,
                                             "chord_interface":  "through_slot",
                                             "weld_to_chord":  {
                                                                   "all_around":  true,
                                                                   "size_mm":  5.0,
                                                                   "type":  "fillet"
                                                               },
                                             "width_mm":  565.0,
                                             "height_mm":  530.0,
                                             "outline":  {
                                                             "points_mm":  [
                                                                               [
                                                                                   -175.0,
                                                                                   280.0
                                                                               ],
                                                                               [
                                                                                   245.0,
                                                                                   280.0
                                                                               ],
                                                                               [
                                                                                   315.0,
                                                                                   210.0
                                                                               ],
                                                                               [
                                                                                   315.0,
                                                                                   -40.0
                                                                               ],
                                                                               [
                                                                                   -35.0,
                                                                                   -250.0
                                                                               ],
                                                                               [
                                                                                   -125.0,
                                                                                   -250.0
                                                                               ],
                                                                               [
                                                                                   -250.0,
                                                                                   -115.0
                                                                               ],
                                                                               [
                                                                                   -250.0,
                                                                                   210.0
                                                                               ]
                                                                           ],
                                                             "mode":  "polygon"
                                                         }
                                         },
                              "node":  {
                                           "element_ids":  [
                                                               1249510,
                                                               1249630,
                                                               1249631,
                                                               1249636
                                                           ]
                                       },
                              "spec_version":  "1.0",
                              "connection_type":  "gusset_node",
                              "uncertain_fields":  [

                                                   ]
                          },
                 "name":  "Nudo tipico Detalle D",
                 "validation_token":  "f7e43ac3c6aa513e8763a6eb1966b6c75ba81b2dcc97853665c2bbbb5b25cd2e",
                 "proposal":  {
                                  "candidates":  [
                                                     {
                                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                                         "max_deviation_deg":  0,
                                                         "score_deg":  0.01,
                                                         "orientation":  "same",
                                                         "name":  "Nudo tipico Detalle D",
                                                         "is_mirrored":  false,
                                                         "description":  "Nudo tipico Detalle D · igual · desvío máx. 0,0°"
                                                     }
                                                 ],
                                  "summary_text":  "Plantilla propuesta: Nudo tipico Detalle D · igual · desvío máx. 0,0°. Otras que encajan: ninguna.",
                                  "member_angles_deg":  [
                                                            136.90000000000001,
                                                            44.399999999999999,
                                                            -135.59999999999999
                                                        ],
                                  "attempts":  [

                                               ],
                                  "chosen":  {
                                                 "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                                 "max_deviation_deg":  0,
                                                 "score_deg":  0.01,
                                                 "orientation":  "same",
                                                 "name":  "Nudo tipico Detalle D",
                                                 "is_mirrored":  false,
                                                 "description":  "Nudo tipico Detalle D · igual · desvío máx. 0,0°"
                                             },
                                  "templates_count":  1
                              },
                 "errors_count":  0,
                 "calculated_values":  {
                                           "axis_distance_mm":  0.080000000000000002,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ],
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "origin_mm":  [
                                                             -11867.700000000001,
                                                             -17195.799999999999,
                                                             17423
                                                         ],
                                           "chord_direction_reversed":  true,
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ]
                                       },
                 "node":  {
                              "chord_direction_reversed":  true,
                              "chord_element_id":  1249510,
                              "element_ids":  [
                                                  1249510,
                                                  1249630,
                                                  1249631,
                                                  1249636
                                              ]
                          },
                 "warnings_count":  0,
                 "match":  {
                               "max_deviation_deg":  0,
                               "score_deg":  0.01,
                               "matched_count":  3,
                               "assignments":  [
                                                   {
                                                       "template_angle_deg":  136.91999999999999,
                                                       "profile_policy":  "warn",
                                                       "element_id":  1249630,
                                                       "side":  "+Y",
                                                       "slot":  0,
                                                       "model_angle_deg":  136.91999999999999,
                                                       "deviation_deg":  0,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "role":  "diagonal"
                                                   },
                                                   {
                                                       "template_angle_deg":  44.369999999999997,
                                                       "profile_policy":  "warn",
                                                       "element_id":  1249631,
                                                       "side":  "+Y",
                                                       "slot":  1,
                                                       "model_angle_deg":  44.369999999999997,
                                                       "deviation_deg":  0,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "role":  "diagonal"
                                                   },
                                                   {
                                                       "template_angle_deg":  -135.63000000000000,
                                                       "profile_policy":  "warn",
                                                       "element_id":  1249636,
                                                       "side":  "-Y",
                                                       "slot":  2,
                                                       "model_angle_deg":  -135.63000000000000,
                                                       "deviation_deg":  0,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "role":  "diagonal"
                                                   }
                                               ],
                               "orientation":  "same",
                               "is_complete":  true,
                               "unmatched_slots":  [

                                                   ],
                               "description":  "same: ranura 0 (diagonal 136.9°) → barra 1249630 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1249631 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1249636 (-135.6°, desvío 0.0°)",
                               "unassigned_members":  [

                                                      ]
                           }
             },
    "errors":  [

               ]
}

```

## 11-8 validate con drawing_image

```text
== conn/validate -> HTTP 200 en 65 ms ==
{
    "meta":  {
                 "duration_ms":  24,
                 "operation":  "validate",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [
                     {
                         "path":  "members[0].expected_angle_deg",
                         "hint":  "Verifica la geometría en el modelo o en el plano.",
                         "message":  "El ángulo del plano (45.0°, inclinación 45.0° respecto al cordón) difiere del de la barra en el modelo (136.9°, inclinación 43.1°) por 1.9° > 1°.",
                         "code":  "ANGLE_DIFFERS_FROM_MODEL"
                     }
                 ],
    "data":  {
                 "is_valid":  true,
                 "bolt_stacks":  [
                                     {
                                         "grip_mm":  19.524999999999999,
                                         "bolt_length_mm":  44.450000000000003,
                                         "gusset_face":  "+z",
                                         "member_element_id":  1249636,
                                         "length_source":  "computed_from_grip"
                                     }
                                 ],
                 "validation_token":  "3df92b37e6c5593682453ebbe44bbf8e41b4a3b2d14a0772a7a12b02d60f38a2",
                 "errors_count":  0,
                 "calculated_values":  {
                                           "axis_distance_mm":  0.080000000000000002,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ],
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "origin_mm":  [
                                                             -11867.700000000001,
                                                             -17195.799999999999,
                                                             17423
                                                         ],
                                           "chord_direction_reversed":  true,
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ]
                                       },
                 "warnings_count":  1
             },
    "errors":  [

               ]
}

```

## 11-8 list antes de borrar

```text
== conn/list -> HTTP 200 en 36 ms ==
{
    "meta":  {
                 "duration_ms":  8,
                 "operation":  "list",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "batch_id":  null,
                 "connections":  [
                                     {
                                         "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                         "batch_id":  null,
                                         "created_elements_count":  9,
                                         "connection_id":  "fea87f0f-5231-4f0b-a4ae-c093980de1f4",
                                         "spec_version":  "1.0",
                                         "created_utc":  "2026-10-12T15:31:54.1458642Z",
                                         "connection_type":  "gusset_node",
                                         "backend":  "advancesteel"
                                     }
                                 ],
                 "connections_count":  1,
                 "total_count":  1,
                 "batches":  {

                             }
             },
    "errors":  [

               ]
}

```

## 11-8 delete 1

```text
== conn/delete -> HTTP 200 en 564 ms ==
{
    "meta":  {
                 "duration_ms":  148,
                 "operation":  "delete",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "deleted_connection_id":  "fea87f0f-5231-4f0b-a4ae-c093980de1f4",
                 "deleted_elements_count":  9,
                 "restored_members_count":  3
             },
    "errors":  [

               ]
}

```

## 11-8 delete 2

```text
== conn/delete -> HTTP 200 en 14 ms ==
{
    "meta":  {
                 "duration_ms":  0,
                 "operation":  "delete",
                 "addin_version":  "0.11.0"
             },
    "ok":  false,
    "warnings":  [

                 ],
    "data":  null,
    "errors":  [
                   {
                       "path":  "connection_id",
                       "hint":  "Usa conn_list para ver las conexiones existentes.",
                       "message":  "connection_id es requerido para borrar una conexión.",
                       "code":  "INVALID_REQUEST"
                   }
               ]
}
ERROR INVALID_REQUEST: connection_id es requerido para borrar una conexión.

```

## 11-8 sondeo 12 restos

```text
== 12-fase3-borrar.py -> HTTP 200 en 183 ms ==
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

## 11-8 sondeo 13 restos

```text
== 13-limpiar-fase1.py -> HTTP 200 en 116 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 11-8 sondeo 17 final

```text
== 17-marcas-plan.py -> HTTP 200 en 11184 ms ==
=== 17-marcas-plan ===
1) Vista activa: Section 2 (Section) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322662] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1321647; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: fallo (Internal error)
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 11-8 sondeo 21 final

```text
== 21-etiquetas-addin.py -> HTTP 200 en 139 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: Section 2 (Section)
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

## 11-8 ping antes del puente

```text
== conn/ping -> HTTP 200 en 15 ms ==
{
    "meta":  {
                 "duration_ms":  2,
                 "operation":  "ping",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "has_uidocument":  true,
                 "document":  {
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false,
                                  "is_workshared":  false,
                                  "is_modifiable":  false
                              },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
                            },
                 "addin_version":  "0.11.0",
                 "revit":  {
                               "sub_version_number":  "2027.2",
                               "language":  "English_USA",
                               "version_build":  "27.2.0.39",
                               "version_number":  "2027",
                               "version_name":  "Autodesk Revit 2027"
                           },
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
                 "spec_version":  "1.0",
                 "backend":  "advancesteel"
             },
    "errors":  [

               ]
}

```

## 11-8 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         35908



```

## 11-8 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.11.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"meta":{"duration_ms":2,"operation":"ping","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"has_uidocument":true,"document":{"is_family":false,"title":"HANGAR_PRUEBA_sondeo","path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_read_only":false,"is_workshared":false,"is_modifiable":false},"dotnet":{"framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","load_context":"Default"},"addin_version":"0.11.0","revit":{"sub_version_number":"2027.2","language":"English_USA","version_build":"27.2.0.39","version_number":"2027","version_name":"Autodesk Revit 2027"},"operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"spec_version":"1.0","backend":"advancesteel"},"errors":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 24783 caracteres
Cuerpo:
{"meta":{"duration_ms":12,"operation":"guide","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventana ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"meta":{"duration_ms":1,"operation":"types","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"errors":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"meta":{"duration_ms":1,"operation":"schema","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"json_schema":{"title":"GussetNodeConnectionSpec","$schema":"http://json-schema.org/draft-07/schema#","additionalProperties":false,"required":["spec_version","connection_type","node","chord","gusset","members"],"type":"object","properties":{"dimension_chains":{"items":{"properties":{"values_mm":{"items":{"type":"number"},"minItems":1,"type":"array"},"label":{"type":"string"},"expected_total_mm":{"minimum":0.0,"type":"number"}},"additionalProperties":false,"required":["values_mm","expected_total_mm"],"type":"object"},"type":"array"},"source":{"properties":{"template_id":{"description":"Plantilla del cat\u00e1logo de la que sali\u00f3 esta especificaci\u00f3n (conn_catalog_apply); la escribe el add-in.","type":["string","null"]},"batch_id":{"description":"Lote al que pertenece la conexi\u00f3n (reservado para conn_batch_create, Fase 9).","type":["string","null"]},"drawing_image":{"description":"Fase 11: ruta del plano como imagen (PNG o JPG) que la previsualizaci\u00f3n abre al lado del croquis; opcional, la escribe la ventana.","type":["string","null"]},"drawing":{"type":"string"},"scale":{"type":"string"}},"additionalProperties":false,"type":"object"},"members":{"items":{"properties":{"end_setback_mm":{"minimum":0.0,"type":"number"},"expected_angle_deg":{"type":"number"},"element_id":{"type":"integer"},"profile":{"type":["string","null"]},"role":{"enum":["diagonal","vertica ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"schema","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":"type","hint":"Tipos disponibles: gusset_node","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","code":"UNKNOWN_OPERATION"}]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"meta": {"duration_ms": 16, "operation": "find_profile", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"total_profiles_in_model": 29, "matched_count": 1, "suggestions": [], "query": "HSS2-1/2X2-1/2X3/16", "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}]}, "errors": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"meta": {"duration_ms": 8, "operation": "node_info", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"x_axis": [1, 1.9999999999999999e-06, 0], "axis_distance_mm": 0.080000000000000002, "existing_connections": [], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "z_axis": [1.9999999999999999e-06, -1, 0], "chord_element_id": 1249510, "y_axis": [0, 0, 1], "members": [{"angle_to_chord_deg": 0, "slope_deg": 0, "element_id": 1249510, "structural_type": "Beam", "node_end": 1, "type": "HSS3X3X1/4", "length_mm": 9960.2999999999993, "angle_in_plane_deg": 0, "side": "chord", "is_chord": true, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "family": "HSS-Hollow Structural Section", "material": "Steel ASTM A500, Grade B, Rectangular and Square", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423]}, {"angle_to_chord_deg": 43.100000000000001, "slope_deg": 43.079999999999998, "element_id": 1249630, "structural_type": "Beam", "node_end": 1, "type": "HSS2-1-2X2-1-2X3-16 64x64", "length_mm": 3568, "angle_in_plane_deg": 136.90000000000001, "side": "+Y", "is_chord": false, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "material": "Material IFC (190-40-140)", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999]}, {"angle_to_chord_deg": 44.399999999999999, "slope_deg": 44.369999999999997, "el ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"meta":{"duration_ms":10,"operation":"validate","addin_version":"0.11.0"},"ok":true,"warnings":[{"path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"data":{"is_valid":true,"bolt_stacks":[{"grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","member_element_id":1249636,"length_source":"computed_from_grip"}],"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","errors_count":0,"calculated_values":{"axis_distance_mm":0.080000000000000002,"frame_x":[1,0,0],"frame_z":[0,-1,0],"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"chord_direction_reversed":true,"frame_y":[0,0,1]},"warnings_count":1},"errors":[]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"duration_ms":7,"operation":"validate","addin_version":"0.11.0"},"ok":false,"warnings":[{"path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"data":null,"errors":[{"path":"dimension_chains[0].values_mm","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","code":"DIMENSION_CHAIN_MISMATCH"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"duration_ms":7,"operation":"validate","addin_version":"0.11.0"},"ok":false,"warnings":[{"path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"data":null,"errors":[{"path":"uncertain_fields[0].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","code":"UNRESOLVED_UNCERTAINTY"},{"path":"uncertain_fields[1].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","code":"UNRESOLVED_UNCERTAINTY"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"dry_run": true, "bolts": 4, "chord_element_id": 1249510, "weld_lines": 6, "gusset_plates": 1, "first_member_element_id": 1249630, "members_modified": 3, "knife_plates": 1, "backend": "advancesteel", "working_point_mm": [-11867.7, -17195.8, 17423], "connection_type": "gusset_node"}
Cuerpo:
{"meta": {"duration_ms": 5, "operation": "preview", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"summary": {"dry_run": true, "bolts": 4, "chord_element_id": 1249510, "weld_lines": 6, "gusset_plates": 1, "first_member_element_id": 1249630, "members_modified": 3, "knife_plates": 1, "backend": "advancesteel", "working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "connection_type": "gusset_node"}, "elements_to_create": [{"kind": "gusset_plate", "vertices_count": 8, "thickness_label": "3/8\"", "thickness_mm": 9.5250000000000004, "chord_interface": "through_slot", "width_mm": 565, "height_mm": 530}, {"for_member_id": 1249630, "slot_length_mm": 150, "kind": "welded_slot_interface", "weld_size_mm": 5}, {"for_member_id": 1249631, "slot_length_mm": 150, "kind": "welded_slot_interface", "weld_size_mm": 5}, {"kind": "knife_plate", "offset_from_gusset_plane_mm": 9.7620000000000005, "thickness_mm": 10, "length_mm": 170, "insertion_mm": 80, "for_member_id": 1249636, "width_mm": 140, "gusset_face": "+z"}, {"columns": 2, "grip_mm": 19.524999999999999, "rows": 2, "count": 4, "kind": "bolt_group", "edge_mm": 40, "spacing_mm": 60, "length_mm": 44.450000000000003, "for_member_id": 1249636, "length_source": "computed_from_grip", "diameter_mm": 15.875}], "members_to_modify": [{"current_end_distance_mm": 86.200000000000003, "new_extension_mm": -93.799999999999997, "element_id": 1249630, "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "action": "Fijar  ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"create","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":"validation_token","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","message":"validation_token es obligatorio para crear una conexi\u00f3n.","code":"VALIDATION_TOKEN_INVALID"}]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"meta": {"duration_ms": 3, "operation": "list", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"batch_id": null, "connections": [], "connections_count": 0, "total_count": 0, "batches": {}}, "errors": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":4,"operation":"get","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":"connection_id","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND"}]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":3,"operation":"delete","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":"connection_id","hint":"Verifica los IDs disponibles con conn_list.","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND"}]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"no_existe","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":null,"hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","code":"UNKNOWN_OPERATION"}]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"meta":{"duration_ms":0,"operation":"catalog_list","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"templates_count":1,"templates":[{"members_count":3,"origin_drawing":"Detalle D","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","tags":["hangar","cercha","HSS"],"name":"Nudo tipico Detalle D","chord_profile":"HSS3X3X1/4","origin_document":"HANGAR_PRUEBA_sondeo","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","created_utc":"2026-10-04T22:01:50.5585233Z","connection_type":"gusset_node","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"}],"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog"},"errors":[]}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=f6f3cea4-6970-4c51-b6c3-3e8494d70d93 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\f6f3cea4-6970-4c51-b6c3-3e8494d70d93.json
Cuerpo:
{"meta":{"duration_ms":24,"operation":"catalog_save","addin_version":"0.11.0"},"ok":true,"warnings":[{"path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"data":{"members_count":3,"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","origin":{"drawing":"Detalle D","document":"HANGAR_PRUEBA_sondeo","element_ids":[1249510,1249630,1249631,1249636],"connection_id":null},"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\f6f3cea4-6970-4c51-b6c3-3e8494d70d93.json","chord_profile":"HSS3X3X1/4","shared_file":null,"member_pattern":[{"profile_policy":"warn","side":"+Y","slot":0,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","angle_deg":136.91999999999999},{"profile_policy":"warn","side":"+Y","slot":1,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","angle_deg":44.369999999999997},{"profile_policy":"warn","side":"-Y","slot":2,"profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","role":"diagonal","angle_deg":-135.63000000000000}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10}},"errors":[]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"meta":{"duration_ms":1,"operation":"catalog_get","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","template":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"spec_template":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"label":"borde superior","expected_total_mm":565.0},{"values_mm":[125.0,90.0,350.0],"label":"base","expected_total_mm":565.0},{"values_mm":[70.0,250.0,210.0],"label":"lado derecho","expected_total_mm":530.0},{"values_mm":[70.0,325.0,135.0],"label":"lado izquierdo","expected_total_mm":530.0}],"source":{"drawing":"Detalle D","scale":"1/10"},"members":[{"end_setback_mm":180.0,"expected_angle_deg":45.0,"slot":0,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":60.0,"expected_angle_deg":45.0,"slot":1,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":260.0,"expected_angle_deg":45.0,"slot":2,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"plate":{"thickness_label":"PL10","thickness_mm":10.0,"length_mm":170.0,"insertion_mm":80.0,"width_mm":140.0},"type":"bolted_knife_plate","bolts":{"columns":2,"rows":2,"edge_mm":40.0,"first_row_from_plate_end_mm":40.0,"spacing_mm":60.0,"diameter_label":"5/8\"","diameter_mm" ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=3a794083f2b9...
Cuerpo:
{"meta":{"duration_ms":12,"operation":"catalog_apply","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","is_valid":true,"bolt_stacks":[{"grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","member_element_id":1249636,"length_source":"computed_from_grip"}],"spec":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"label":"borde superior","expected_total_mm":565.0},{"values_mm":[125.0,90.0,350.0],"label":"base","expected_total_mm":565.0},{"values_mm":[70.0,250.0,210.0],"label":"lado derecho","expected_total_mm":530.0},{"values_mm":[70.0,325.0,135.0],"label":"lado izquierdo","expected_total_mm":530.0}],"source":{"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","drawing":"Detalle D","scale":"1/10"},"members":[{"end_setback_mm":180.0,"expected_angle_deg":43.100000000000001,"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":60.0,"expected_angle_deg":44.399999999999999,"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":260.0,"expected_angle_deg":44.399999999999999,"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"plate":{"thickness_label":"PL10","t ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"meta":{"duration_ms":15,"operation":"catalog_apply","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","is_valid":true,"bolt_stacks":[{"grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","member_element_id":1249636,"length_source":"computed_from_grip"}],"spec":{"dimension_chains":[{"values_mm":[75.0,420.0,70.0],"label":"borde superior","expected_total_mm":565.0},{"values_mm":[125.0,90.0,350.0],"label":"base","expected_total_mm":565.0},{"values_mm":[70.0,250.0,210.0],"label":"lado derecho","expected_total_mm":530.0},{"values_mm":[70.0,325.0,135.0],"label":"lado izquierdo","expected_total_mm":530.0}],"source":{"template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93","drawing":"Detalle D","scale":"1/10"},"members":[{"end_setback_mm":180.0,"expected_angle_deg":43.100000000000001,"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":60.0,"expected_angle_deg":44.399999999999999,"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"slot_length_mm":150.0,"weld":{"all_around":true,"size_mm":5.0,"type":"fillet"},"type":"welded_slot"}},{"end_setback_mm":260.0,"expected_angle_deg":44.399999999999999,"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal","attachment":{"plate":{"thickness_label":"PL10","t ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=df7d3bbc-c0fd-407f-ac01-ff994bf17e0a nudos=6 resumen={'untyped': 5, 'ready': 1} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"meta":{"duration_ms":26,"operation":"batch_plan","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"created_utc":"2026-10-12T16:03:55.0672054Z","summary":{"untyped":5,"ready":1},"is_marked":false,"label_view_id":null,"overrides":{"merge":[],"remove_member":{},"template":{},"split":{},"spec":{},"add_node":{},"add_member":{},"chord":{},"exclude":[],"replace_existing":false},"document":"HANGAR_PRUEBA_sondeo","plan_id":"df7d3bbc-c0fd-407f-ac01-ff994bf17e0a","label_indices":{},"last_report":null,"description":"6 nudo(s): 5 untyped, 1 ready.","summary_text":"Se crear\u00e1 1 conexi\u00f3n con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.","creatable_count":1,"nodes":[{"marker_element_id":null,"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"chord_type_name":null,"chord_continuous":false,"members":[],"match":null,"is_marked":false,"element_ids":[1249630],"attempts":[],"is_valid":false,"errors":[],"errors_count":0,"max_deviation_deg":null,"ghost_element_id":null,"color_name":null,"created_connection_id":null,"orientation":null,"is_manual":false,"status_text":"\u25cb Barra suelta (no es nudo)","through_element_ids":[],"chord_element_id":0,"existing_batch_id":null,"template_name":null,"status":"untyped","warnings":[],"is_mirrored":false,"spec":null,"member_element_ids":[1249630],"color_rgb":null,"label_index":null,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","advice":"No es un  ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=dbb894c06dee...
Cuerpo:
{"meta":{"duration_ms":0,"operation":"batch_plan_get","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"plan_id":"df7d3bbc-c0fd-407f-ac01-ff994bf17e0a","node":{"marker_element_id":null,"work_point_mm":[-11870,-17195.799999999999,17423],"chord_type_name":"HSS3X3X1/4","chord_continuous":true,"members":[{"element_id":1249630,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","end_gap_mm":84.5,"angle_deg":136.90000000000001,"reaches_node":true},{"element_id":1249631,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","end_gap_mm":19.600000000000001,"angle_deg":44.399999999999999,"reaches_node":true},{"element_id":1249636,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","end_gap_mm":48.200000000000003,"angle_deg":-135.59999999999999,"reaches_node":true}],"match":{"max_deviation_deg":0,"score_deg":0.01,"matched_count":3,"assignments":[{"template_angle_deg":136.91999999999999,"profile_policy":"warn","element_id":1249630,"side":"+Y","slot":0,"model_angle_deg":136.91999999999999,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"template_angle_deg":44.369999999999997,"profile_policy":"warn","element_id":1249631,"side":"+Y","slot":1,"model_angle_deg":44.369999999999997,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"template_angle_deg":-135.63000000000000,"profile_policy":"warn","element_id":1249636,"side":"-Y","slot":2,"m ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote df7d3bbc: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"meta":{"duration_ms":12,"operation":"batch_create","addin_version":"0.11.0"},"ok":true,"warnings":[{"path":"nodes[N4]","hint":"Ese nudo se revirti\u00f3 solo; en el plan queda failed con su especificaci\u00f3n y su token para reintentar.","message":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","code":"BATCH_NODE_FAILED"}],"data":{"updated_count":0,"restored_members_count":0,"rolled_back_count":0,"document":"HANGAR_PRUEBA_sondeo","plan_id":"df7d3bbc-c0fd-407f-ac01-ff994bf17e0a","stopped":false,"skipped_count":0,"connection_ids":[],"summary_text":"Lote df7d3bbc: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","nodes":[{"template_name":"PRUEBA probar_conexiones","warnings":[],"orientation":"same","reason":"token distinto del plan","spec":null,"restored_members_count":0,"outcome":"failed","elements_count":0,"connection_id":null,"node":"N4","errors":[{"path":"nodes[N4].validation_token","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","code":"VALIDATION_TOKEN_INVALID"}],"duration_ms":0,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el  ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"meta":{"duration_ms":5,"operation":"batch_delete","addin_version":"0.11.0"},"ok":true,"warnings":[{"path":"batch_id","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","code":"BATCH_EMPTY"}],"data":{"updated_count":0,"restored_members_count":0,"rolled_back_count":0,"document":"HANGAR_PRUEBA_sondeo","plan_id":null,"stopped":false,"skipped_count":0,"connection_ids":[],"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","nodes":[],"undo_entries":"one","failed_count":0,"plan_summary_text":null,"duration_ms":3,"batch_id":"00000000-0000-0000-0000-000000000000","started_utc":"2026-10-12T16:03:55.5353843Z","deleted_count":0,"with_warnings_count":0,"operation":"batch_delete","finished_utc":"2026-10-12T16:03:55.5393384Z","deleted_elements_count":0,"stop_on_error":false,"created_count":0,"stopped_at":null,"plan_summary":null,"plan_creatable_count":null},"errors":[]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado df7d3bbc-c0fd-407f-ac01-ff994bf17e0a
Cuerpo:
{"meta": {"duration_ms": 14, "operation": "batch_plan_discard", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"discarded_plan_id": "df7d3bbc-c0fd-407f-ac01-ff994bf17e0a", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "errors": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"catalog_apply","addin_version":"0.11.0"},"ok":false,"warnings":[],"data":null,"errors":[{"path":"template_id","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","code":"TEMPLATE_NOT_FOUND"}]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada f6f3cea4-6970-4c51-b6c3-3e8494d70d93
Cuerpo:
{"meta":{"duration_ms":1,"operation":"catalog_delete","addin_version":"0.11.0"},"ok":true,"warnings":[],"data":{"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\f6f3cea4-6970-4c51-b6c3-3e8494d70d93.json","name":"PRUEBA probar_conexiones","deleted_template_id":"f6f3cea4-6970-4c51-b6c3-3e8494d70d93"},"errors":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"meta":{"duration_ms":821,"operation":"batch_plan","addin_version":"0.11.0"},"ok":true,"warnings":[{"path":"element_ids","hint":"El plan se calcul\u00f3 con la selecci\u00f3n ampliada (selection_count las cuenta; data.selection_expansion dice cu\u00e1les); sin expand_selection se planifica solo lo seleccionado.","message":"Se a\u00f1adieron 63 barras que tocan la selecci\u00f3n: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cord\u00f3n.","code":"SELECTION_EXPANDED"}],"data":{"created_utc":"2026-10-12T16:03:57.0324202Z","summary":{"untyped":18,"no_match":25,"ready":16},"is_marked":false,"label_view_id":null,"overrides":{"merge":[],"remove_member":{},"template":{},"split":{},"spec":{},"add_node":{},"add_member":{},"chord":{},"exclude":[],"replace_existing":false},"document":"HANGAR_PRUEBA_sondeo","plan_id":"7191cf49-a241-43be-8c78-60aa3ef5d342","label_indices":{},"last_report":null,"description":"59 nudo(s): 25 no_match, 18 untyped, 16 ready.","summary_text":"Se crear\u00e1n 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 empalmes del cord\u00f3n (sin plantilla). Ocultos: 8 sin cord\u00f3n, 18 barras sueltas.","creatable_count":16,"nodes":[{"marker_element_id":null,"work_point_mm":[-14551.900000000000,-17195.799999999999,19933],"chord_type_name":"HSS12X8X1/2","chord_continuous":false,"members":[{"element_id":1249630,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","end_gap_mm ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 7191cf49-a241-43be-8c78-60aa3ef5d342 etiquetas quitadas=0
Cuerpo:
{"meta": {"duration_ms": 25, "operation": "batch_plan_discard", "addin_version": "0.11.0"}, "ok": true, "warnings": [], "data": {"discarded_plan_id": "7191cf49-a241-43be-8c78-60aa3ef5d342", "remaining_plans": 0, "remaining_labels": 0, "removed_labels": 0, "removed_marks": 0, "remaining_markers": 0}, "errors": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.11.0
Cuerpo:
{
  "meta": {
    "duration_ms": 2,
    "operation": "ping",
    "addin_version": "0.11.0"
  },
  "ok": true,
  "warnings": [],
  "data": {
    "has_uidocument": true,
    "document": {
      "is_family": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_read_only": false,
      "is_workshared": false,
      "is_modifiable": false
    },
    "dotnet": {
      "framework": ".NET 10.0.12",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "load_context": "Default"
    },
    "addin_version": "0.11.0",
    "revit": {
      "sub_version_number": "2027.2",
      "language": "English_USA",
      "version_build": "27.2.0.39",
      "version_number": "2027",
      "version_name": "Autodesk Revit 2027"
    },
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
    "spec_version": "1.0",
    "backend": "advancesteel"
  },
  "errors": []
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "meta": {
    "duration_ms": 17,
    "operation": "catalog_apply",
    "addin_version": "0.11.0"
  },
  "ok": true,
  "warnings": [],
  "data": {
    "template_id": "8bc468db-5f15-466c-afb3-41284067f4fa",
    "is_valid": true,
    "bolt_stacks": [
      {
        "grip_mm": 19.525,
        "bolt_length_mm": 44.45,
        "gusset_face": "+z",
        "member_element_id": 1249636,
        "length_source": "computed_from_grip"
      }
    ],
    "spec": {
      "dimension_chains": [
        {
          "values_mm": [
            75.0,
            420.0,
            70.0
          ],
          "label": "borde superior",
          "expected_total_mm": 565.0
        },
        {
          "values_mm": [
            125.0,
            90.0,
            350.0
          ],
          "label": "base",
          "expected_total_mm": 565.0
        },
        {
          "values_mm": [
            70.0,
            250.0,
            210.0
          ],
          "label": "lado derecho",
          "expected_total_mm": 530.0
        },
        {
          "values_mm": [
            70.0,
            325.0,
            135.0
          ],
          "label": "lado izquierdo",
          "expected_total_mm": 530.0
        }
      ],
      "source": {
        "template_id": "8bc468db-5f15-466c-afb3-41284067f4fa",
        "drawing": "Detalle D",
        "scale": "1/10"
      },
      "members": [
        {
          "end_setback_mm": 180.0,
          "expected_angle_deg": 43.1,
          "elem ...
======================================================================
Resultado: 34/34 pruebas correctas

```

## 11-8 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 423 ms ==
{
    "meta":  {
                 "duration_ms":  0,
                 "operation":  "catalog_list",
                 "addin_version":  "0.11.0"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "data":  {
                 "templates_count":  1,
                 "templates":  [
                                   {
                                       "members_count":  3,
                                       "origin_drawing":  "Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "name":  "Nudo tipico Detalle D",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "connection_type":  "gusset_node",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "errors":  [

               ]
}

```

## 11-8 log del dia

```text

{"ts":"2026-10-12T10:10:20.6193478-05:00","record":{"event":"startup","addin_version":"0.11.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-12T10:28:55.4999840-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":3,"chord_element_id":1249510,"templates":1,"candidates":1,"proposal":{"template_id":"6abcf1
16-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico Detalle D","orientation":"same","max_deviation_deg":0},"starter":fa
lse,"mode":"template","is_valid":true,"errors":[],"duration_ms":156}}
{"ts":"2026-10-12T10:28:55.8024633-05:00","record":{"event":"thumbnail_written","template_id":"6abcf116-9b97-485f-b50d-
2851ca0018cc","name":"Nudo tipico Detalle D","file":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.png"}}
{"ts":"2026-10-12T10:29:45.9867312-05:00","record":{"event":"ribbon_preview_edit","path":"gusset.thickness_mm","from":"
panel","is_valid":true}}
{"ts":"2026-10-12T10:30:11.0725989-05:00","record":{"event":"ribbon_preview_edit","path":"members[2].attachment.bolts.d
iameter_mm","from":"panel","is_valid":true}}
{"ts":"2026-10-12T10:30:24.9511536-05:00","record":{"event":"ribbon_preview_edit","path":"members[0].attachment.type","
from":"panel","is_valid":true}}
{"ts":"2026-10-12T10:30:28.4826312-05:00","record":{"event":"ribbon_preview_edit","path":"members[0].attachment.type","
from":"panel","is_valid":true}}
{"ts":"2026-10-12T10:30:30.3327785-05:00","record":{"event":"ribbon_preview_edit","path":"members[0].attachment.type","
from":"panel","is_valid":true}}
{"ts":"2026-10-12T10:30:40.4847093-05:00","record":{"event":"ribbon_preview_edit","path":"members[0].attachment.type","
from":"panel","is_valid":true}}
{"ts":"2026-10-12T10:31:54.4300580-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo tipico Detalle D-nudo-1249510.json","template_id":"
6abcf116-9b97-485f-b50d-2851ca0018cc","connection_id":"fea87f0f-5231-4f0b-a4ae-c093980de1f4","created_elements":9,"modi
fied_members":3,"backend":"advancesteel","validation_token_prefix":"6b75cf4fec4fea90","warnings":[],"duration_ms":963}}
{"ts":"2026-10-12T10:32:30.4570099-05:00","record":{"event":"ribbon_connections_window","document":"HANGAR_PRUEBA_sonde
o","deleted":0}}
{"ts":"2026-10-12T10:39:53.4759944-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":3,"chord_element_id":1249510,"templates":1,"candidates":1,"proposal":{"template_id":"6abcf1
16-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico Detalle D","orientation":"mirror_x","max_deviation_deg":1.35},"star
ter":false,"mode":"template","is_valid":true,"errors":[],"duration_ms":26}}
{"ts":"2026-10-12T10:41:46.7607505-05:00","record":{"event":"drawing_pane_opened","file":"D:\\Proyectos 
C#\\CONEXIONES\\docs\\fixtures\\detalle-D.png","from":"archivo"}}
{"ts":"2026-10-12T10:43:55.6527574-05:00","record":{"event":"drawing_pane_opened","file":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Local\\MotorConexiones\\planos\\plano-HANGAR_PRUEBA_sondeo-20261012-104355.png","from":"portapapeles"}}
{"ts":"2026-10-12T10:44:07.2621342-05:00","record":{"event":"ribbon_connect_cancelled","selection":4,"reason":"window_c
losed","mode":"template"}}
{"ts":"2026-10-12T10:45:59.1641724-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":3,"ignored":0,"members":2,"chord_element_id":1245533,"templates":1,"candidates":0,"proposal":null,"starter":true,"m
ode":"starter","is_valid":false,"errors":["MEMBER_NOT_AT_NODE"],"duration_ms":26}}
{"ts":"2026-10-12T10:46:58.0546757-05:00","record":{"event":"ribbon_connect_cancelled","selection":3,"reason":"window_c
losed","mode":"starter"}}
{"ts":"2026-10-12T10:53:49.8321215-05:00","record":{"event":"ribbon_batch_assist","selection":1,"added":63,"chords":9,"
members":38,"splices":16,"skipped_out_of_plane":0,"choice":"all"}}
{"ts":"2026-10-12T10:54:45.6579261-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":true,"highlighted"
:true,"window_open":true}}
{"ts":"2026-10-12T10:54:45.6644913-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:54:46.4154097-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:54:46.4160778-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:54:56.2357818-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:54:56.2364652-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:55:19.5844892-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:55:19.5851624-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:55:20.1248547-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:55:20.1254601-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:55:20.5098771-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"d6978e07-5d6e-45af-97a8-e7330b8a67b9","node":"N4","status":"already_connected","changed":false,"highlighted
":true,"window_open":true}}
{"ts":"2026-10-12T10:55:20.5105044-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"d6978e07-5d6e-45af-
97a8-e7330b8a67b9","node":"N4","status":"already_connected"}}
{"ts":"2026-10-12T10:56:33.8802726-05:00","record":{"event":"ribbon_batch_assist","selection":5,"added":60,"chords":7,"
members":39,"splices":14,"skipped_out_of_plane":0,"choice":"cancel"}}
{"ts":"2026-10-12T10:59:21.0554994-05:00","record":{"event":"catalog_apply","template_id":"6abcf116-9b97-485f-b50d-2851
ca0018cc","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-12T10:59:21.0711798-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":42}}
{"ts":"2026-10-12T11:03:54.9485120-05:00","record":{"event":"catalog_apply","template_id":"f6f3cea4-6970-4c51-b6c3-3e84
94d70d93","chosen_automatically":false,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":
true,"error_codes":[]}}
{"ts":"2026-10-12T11:03:54.9489429-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"f6f3cea4-6970-4c51-b6c3-3e8494d70d93\", \"chord_element_id\": 1249510, \"element_ids\": [1249510, 
1249630, 1249631, 1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":12}}
{"ts":"2026-10-12T11:03:55.0131032-05:00","record":{"event":"catalog_apply","template_id":"f6f3cea4-6970-4c51-b6c3-3e84
94d70d93","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-12T11:03:55.0136262-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":15}}
{"ts":"2026-10-12T11:03:55.4962958-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"node\": \"N4\", \"validation_token\": 
\"0000000000000000000000000000000000000000000000000000000000000000\"}], \"plan_id\": \"df7d3bbc-c0fd-407f-ac01-ff994bf1
7e0a\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAILED"],"duration_ms":12}}
{"ts":"2026-10-12T11:03:56.4371777-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"00000000-0000-0000-0000-000000000000\", \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":false,"error_codes":["TEMPLATE_NOT_FOUND"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-12T11:03:58.7583973-05:00","record":{"event":"catalog_apply","template_id":"8bc468db-5f15-466c-afb3-4128
4067f4fa","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-12T11:03:58.7590145-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":17}}



```

## 11-9 git status antes del commit

```text
?? docs/fases/capturas/fase11-01-conectar.png
?? docs/fases/capturas/fase11-02-panel.png
?? docs/fases/capturas/fase11-03-plano.png
?? docs/fases/capturas/fase11-04-arranque.png
?? docs/fases/capturas/fase11-05-catalogo.png
?? docs/fases/capturas/fase11-06-titulo.png
?? docs/fases/resultados-fase-11.md

```
