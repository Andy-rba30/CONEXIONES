# Resultados de la ronda 8e

Fecha: 2026-10-05T20:07:45


## 8e-1 git

```text
0c58f1c Ronda 8e: qué sigue y prompts del instalador y del cierre
?? docs/fases/resultados-fase-8e.md

```

## 8e-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 2.1 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 2.1 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 2.09 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:11.48
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   177, Omitido:     0, Total:   177, Duración: 423 ms - MotorConexiones.Tests.dll (net10.0)

```

## 8e-1 revit cerrado

```text

```

## 8e-1 deploy

```text
== MotorConexiones 0.8.5.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 8e-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (23 rutas @api.route)
- copiado tools\conn_tools.py (21 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 8e-1 version de la dll

```text
0.8.5.0

```

## 8e-2 ping

```text
== conn/ping -> HTTP 200 en 406 ms ==
{
    "ok":  true,
    "warnings":  [

                 ],
    "meta":  {
                 "addin_version":  "0.8.5",
                 "operation":  "ping",
                 "duration_ms":  9
             },
    "data":  {
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
                               "language":  "English_USA",
                               "version_build":  "27.2.0.39",
                               "sub_version_number":  "2027.2",
                               "version_number":  "2027"
                           },
                 "spec_version":  "1.0",
                 "addin_version":  "0.8.5",
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "has_uidocument":  true,
                 "backend":  "advancesteel",
                 "document":  {
                                  "is_read_only":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_workshared":  false
                              }
             },
    "errors":  [

               ]
}

```

## 8e-2 sondeo 17 marcas

```text
== 17-marcas-plan.py -> HTTP 200 en 1065 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322481] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 8e-4 sondeo 17 tras descartar

```text
== 17-marcas-plan.py -> HTTP 200 en 1074 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1323083] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 8e-4 batch_plan_discard all

```text
== conn/batch_plan_discard -> HTTP 200 en 70 ms ==
{
    "ok":  true,
    "warnings":  [

                 ],
    "meta":  {
                 "addin_version":  "0.8.5",
                 "operation":  "batch_plan_discard",
                 "duration_ms":  40
             },
    "data":  {
                 "remaining_markers":  0,
                 "discarded_plans":  2,
                 "removed_markers":  0
             },
    "errors":  [

               ]
}

```

## 8e-5 sondeo 19 etiquetas v3

```text
== 19-etiquetas-lienzo.py -> HTTP 200 en 777 ms ==
=== 19-etiquetas-lienzo (v3: BMP 24 bits 32x32, ruta sin tildes, SetVisibility, refresco, dos etiquetas) ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | id=1245519
1) Vista 3D | caja de seccion activa=False | perspectiva=False
2) DB.TemporaryGraphicsManager existe: True
2) DB.InCanvasControlData existe: True
2) miembros de TemporaryGraphicsManager: AddControl, Clear, Dispose, GetAll, GetTemporaryGraphicsManager, IsValidObject, RemoveControl, SetTooltip, SetVisibility, UpdateControl, get_IsValidObject
3) servicio TemporaryGraphicsHandlerService: TemporaryGraphicsHandlerService | servidores registrados: 1
3) UI.ITemporaryGraphicsHandler existe: True
4) BMP A: C:\IA\MotorConexiones-sondeo19\etiqueta-A.bmp | 32x32, 24 bits, 3126 bytes (esperado 3126) | System.Drawing a 24 bits | ruta sin tildes ni espacios: True
4) BMP B: C:\IA\MotorConexiones-sondeo19\etiqueta-B.bmp | 32x32, 24 bits, 3126 bytes (esperado 3126) | System.Drawing a 24 bits | ruta sin tildes ni espacios: True
5) Etiqueta A (N4): pies (-38.9360, -56.4167, 57.1621) = mm (-11867.7, -17195.8, 17423.0)
5) sin etiqueta B (no se pudo calcular el punto: Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64))
6) TemporaryGraphicsManager obtenido: True
6) Control A anadido en la vista 1245519: indice 0 | ImagePath=C:\IA\MotorConexiones-sondeo19\etiqueta-A.bmp | Position pies (-38.9360, -56.4167, 57.1621) = mm (-11867.7, -17195.8, 17423.0)
6) SetVisibility(0, True) llamado
6) SetTooltip(0) puesto
6) GetAll(): 1 control(es) en el documento: 0
7) uidoc.RefreshActiveView() llamado
7) uidoc.UpdateAllOpenViews() llamado
8) Captura exportada: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8e-01-etiqueta-exportada.png (lo normal es que NO ensene las etiquetas: haz la captura a mano, fase8e-01-etiqueta.png)
9) manejador registrado y activo como servidor 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60 de TemporaryGraphicsHandlerService: pincha una etiqueta en Revit; debe salir un cuadro y una linea en C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\log\sondeo19-clics.txt
10) Las etiquetas se quedan puestas (indices 0, vista 1245519) para que la persona las mire y las pinche; despues ejecuta 19b-etiquetas-quitar.py
11) Marcadores DirectShape de plan en el modelo (no los toca este sondeo): 0
=== fin 19-etiquetas-lienzo ===


```

## 8e-5 sondeo 19b quitar

```text
ERROR: no se pudo hablar con Revit en http://127.0.0.1:48884/revit_mcp/conn/dev_exec/ (Se canceló una tarea.). Comprueba que Revit esta abierto, que pyRevit tiene Routes activo y que la extension revit-mcp esta cargada.

```

## 8e-6 sondeo 17 marcadores

```text
ERROR: no se pudo hablar con Revit en http://127.0.0.1:48884/revit_mcp/conn/dev_exec/ (Se canceló una tarea.). Comprueba que Revit esta abierto, que pyRevit tiene Routes activo y que la extension revit-mcp esta cargada.

```

## 8e-6 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 274972 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 10 ms | errores=- | avisos=-
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

## 8e-6 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 172 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 8e-7 probar_conexiones --puente

```text
======================================================================
Conexión con Revit  [FALLO]  no se pudo hablar con http://127.0.0.1:48884/revit_mcp: [WinError 10061] No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión. ¿Revit abierto con pyRevit Routes activo?
======================================================================
Resultado: 0/1 pruebas correctas

```

## 8e-7 log del dia

```text

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
{"ts":"2026-10-05T20:06:45.0544049-05:00","record":{"event":"ribbon_batch_window","plan_id":"ff066e37-c4b1-448d-8c0c-95
7239aac032","action":"closed","discarded":true}}
{"ts":"2026-10-05T20:09:15.3156080-05:00","record":{"event":"startup","addin_version":"0.8.5","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-05T20:16:08.5480096-05:00","record":{"event":"batch_plan","plan_id":"861504d6-12ab-4356-b44f-40a97cd6948
e","replan":false,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:16:08.8495776-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"861504d6-12ab-4356-
b44f-40a97cd6948e","modeless":true,"selection":64}}
{"ts":"2026-10-05T20:19:12.7327284-05:00","record":{"event":"ribbon_batch_pick","plan_id":"861504d6-12ab-4356-b44f-40a9
7cd6948e","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:21:14.3257003-05:00","record":{"event":"ribbon_batch_picked","plan_id":"861504d6-12ab-4356-b44f-40
a97cd6948e","action":"Cordón…","node":"N9","result":"cancelado"}}
{"ts":"2026-10-05T20:24:40.1321454-05:00","record":{"event":"ribbon_batch_window","plan_id":"861504d6-12ab-4356-b44f-40
a97cd6948e","action":"closed","discarded":false}}
{"ts":"2026-10-05T20:25:15.6996768-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":false,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untyp
ed":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_mem
ber\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:25:15.9018713-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"af5990a6-4f17-4753-
aee8-f8249c771ee3","modeless":true,"selection":64}}
{"ts":"2026-10-05T20:27:01.4188396-05:00","record":{"event":"ribbon_batch_show","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","node":"N9"}}
{"ts":"2026-10-05T20:27:10.4478791-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:27:44.0824495-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Cordón…","node":"N9","result":"1245531"}}
{"ts":"2026-10-05T20:27:44.5544748-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245531},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:27:44.5554403-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1245531},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:27:52.3155343-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:27:55.9888964-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Cordón…","node":"N9","result":"1251056"}}
{"ts":"2026-10-05T20:27:56.5737149-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1251056},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:27:56.5742851-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1251056},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:28:01.2487256-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":24,"untype
d":18,"ready":16,"excluded":1},"marked":true,"overrides":"{\"exclude\":[\"N9\"],\"add_node\":{},\"chord\":{\"N9\":12510
56},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":f
alse}"}}
{"ts":"2026-10-05T20:28:01.2495428-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":24,"untyped":18,"ready":16,"excluded":1},"overrides":"{\"exclude\":[\"N9\"],\"add_nod
e\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:28:05.1854609-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1251056},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:28:05.1860507-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{}
,\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":fals
e}"}}
{"ts":"2026-10-05T20:28:19.1794502-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:28:20.9862451-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Cordón…","node":"N9","result":"1245530"}}
{"ts":"2026-10-05T20:28:21.4514866-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245530},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:28:21.4520728-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1245530},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:28:46.9212700-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1251049},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:28:46.9219987-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1251049},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:28:53.8530069-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1251056},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:28:53.8536546-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1251056},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:29:04.5237279-05:00","record":{"event":"ribbon_batch_show","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","node":"N9"}}
{"ts":"2026-10-05T20:29:12.7911393-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:29:22.3688594-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Cordón…","node":"N9","result":"1245531"}}
{"ts":"2026-10-05T20:29:22.8787309-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245531},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:29:22.8793025-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1245531},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:29:56.6229073-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Cordón…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:29:58.3859708-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Cordón…","node":"N9","result":"1245531"}}
{"ts":"2026-10-05T20:29:58.8790838-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245531},\"template\":{}
,\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:29:58.8796052-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\
"N9\":1245531},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_e
xisting\":false}"}}
{"ts":"2026-10-05T20:32:19.3240611-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Barras…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:32:40.8926435-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Barras…","node":"N9","result":"1251056"}}
{"ts":"2026-10-05T20:32:41.4278779-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245531},\"template\":{}
,\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":fals
e}"}}
{"ts":"2026-10-05T20:32:41.4284424-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{}
,\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"replace
_existing\":false}"}}
{"ts":"2026-10-05T20:32:51.6300423-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Barras…","node":"N9","view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:32:55.9159388-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Barras…","node":"N9","result":"1251056"}}
{"ts":"2026-10-05T20:32:56.5115997-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"ready":16},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{\"N9\":1245531},\"template\":{}
,\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":fals
e}"}}
{"ts":"2026-10-05T20:32:56.5120748-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"ready":16},"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{}
,\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"replace
_existing\":false}"}}
{"ts":"2026-10-05T20:33:14.6707293-05:00","record":{"event":"ribbon_batch_pick","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","action":"Añadir nudo…","node":null,"view":1245519,"window_hidden":true,"revit_activated":true}}
{"ts":"2026-10-05T20:33:25.3476897-05:00","record":{"event":"ribbon_batch_picked","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"Añadir nudo…","node":null,"result":"cancelado"}}
{"ts":"2026-10-05T20:34:19.3287537-05:00","record":{"event":"ribbon_batch_show","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","node":"N11"}}
{"ts":"2026-10-05T20:34:24.3528088-05:00","record":{"event":"ribbon_batch_show","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","node":"N11"}}
{"ts":"2026-10-05T20:34:29.4898089-05:00","record":{"event":"ribbon_batch_show","plan_id":"af5990a6-4f17-4753-aee8-f824
9c771ee3","node":"N11"}}
{"ts":"2026-10-05T20:36:16.9030659-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"excluded":1,"ready":15},"marked":true,"overrides":"{\"exclude\":[\"N4\"],\"add_node\":{},\"chord\":{\"N9\":12455
31},\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:36:16.9037477-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"excluded":1,"ready":15},"overrides":"{\"exclude\":[\"N4\"],\"add_nod
e\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:36:19.1295048-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"excluded":1,"ready":15},"marked":true,"overrides":"{\"exclude\":[\"N4\"],\"add_node\":{},\"chord\":{\"N9\":12455
31},\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:36:19.1300422-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"excluded":1,"ready":15},"overrides":"{\"exclude\":[],\"add_node\":{}
,\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_ex
isting\":false}"}}
{"ts":"2026-10-05T20:36:24.0624570-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"excluded":1,"ready":15},"marked":true,"overrides":"{\"exclude\":[\"N4\"],\"add_node\":{},\"chord\":{\"N9\":12455
31},\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:36:24.0630789-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"excluded":1,"ready":15},"overrides":"{\"exclude\":[],\"add_node\":{}
,\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_ex
isting\":false}"}}
{"ts":"2026-10-05T20:36:36.0863980-05:00","record":{"event":"batch_plan","plan_id":"af5990a6-4f17-4753-aee8-f8249c771ee
3","replan":true,"element_ids":64,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"no_match":25,"untype
d":18,"excluded":1,"ready":15},"marked":true,"overrides":"{\"exclude\":[\"N4\"],\"add_node\":{},\"chord\":{\"N9\":12455
31},\"template\":{},\"remove_member\":{},\"add_member\":{\"N9\":[1251056]},\"merge\":[],\"split\":{},\"spec\":{},\"repl
ace_existing\":false}"}}
{"ts":"2026-10-05T20:36:36.0870104-05:00","record":{"event":"ribbon_batch_replan","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","summary":{"no_match":25,"untyped":18,"excluded":1,"ready":15},"overrides":"{\"exclude\":[],\"add_node\":{}
,\"chord\":{},\"template\":{},\"remove_member\":{},\"add_member\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_ex
isting\":false}"}}
{"ts":"2026-10-05T20:37:56.5060005-05:00","record":{"event":"ribbon_batch_window","plan_id":"af5990a6-4f17-4753-aee8-f8
249c771ee3","action":"closed","discarded":false}}
{"ts":"2026-10-05T20:38:12.7252969-05:00","record":{"event":"batch_plan","plan_id":"d532d975-5a4c-473a-ac03-f7ee603ad4e
5","replan":false,"element_ids":7,"templates":["6abcf116-9b97-485f-b50d-2851ca0018cc"],"summary":{"untyped":8,"ready":2
},"marked":true,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_me
mber\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-05T20:38:12.8060786-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"d532d975-5a4c-473a-
ac03-f7ee603ad4e5","modeless":true,"selection":7}}
{"ts":"2026-10-05T20:38:34.6441272-05:00","record":{"event":"batch_plan_discard","plan_id":"d532d975-5a4c-473a-ac03-f7e
e603ad4e5","from":"window","removed_marks":9,"other_plans_unmarked":0,"orphan_markers":0,"remaining_markers":0}}
{"ts":"2026-10-05T20:38:34.6461544-05:00","record":{"event":"ribbon_batch_discard","plan_id":"d532d975-5a4c-473a-ac03-f
7ee603ad4e5","result":"9 marca(s) quitadas; quedan 0 marcadores en el documento.","warnings":0,"closes_window":true}}
{"ts":"2026-10-05T20:38:34.6603898-05:00","record":{"event":"ribbon_batch_window","plan_id":"d532d975-5a4c-473a-ac03-f7
ee603ad4e5","action":"closed","discarded":true}}
{"ts":"2026-10-05T20:42:21.3047612-05:00","record":{"event":"batch_plan_discard_all","markers":0,"orphans":0}}
{"ts":"2026-10-05T20:42:21.3126703-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"all\": true}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":40}}



```

## 8e-8 git status antes del commit

```text
 M docs/fases/capturas/fase8-01-sondeo17.png
A  docs/fases/capturas/fase8e-01-etiqueta-clic.png
A  docs/fases/capturas/fase8e-01-etiqueta.png
A  docs/fases/capturas/fase8e-02-cordon-n9.png
?? docs/fases/capturas/fase8e-01-etiqueta-exportada.png
?? docs/fases/resultados-fase-8e.md

```

## 8e-7b ping

```text
== conn/ping -> HTTP 200 en 297 ms ==
{
    "data":  {
                 "addin_version":  "0.8.5",
                 "revit":  {
                               "version_number":  "2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "version_name":  "Autodesk Revit 2027"
                           },
                 "has_uidocument":  true,
                 "backend":  "advancesteel",
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
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
                 "spec_version":  "1.0",
                 "document":  {
                                  "is_read_only":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_modifiable":  false,
                                  "is_workshared":  false,
                                  "is_family":  false
                              }
             },
    "ok":  true,
    "warnings":  [

                 ],
    "meta":  {
                 "addin_version":  "0.8.5",
                 "operation":  "ping",
                 "duration_ms":  9
             },
    "errors":  [

               ]
}

```

## 8e-7b sondeo 19b quitar (repetido)

```text
== 19b-etiquetas-quitar.py -> HTTP 200 en 298 ms ==
=== 19b-etiquetas-quitar ===
1) clics anotados en C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\log\sondeo19-clics.txt:
clic en una etiqueta: Index=0 | Document=HANGAR_PRUEBA_sondeo
clic en una etiqueta: Index=0 | Document=HANGAR_PRUEBA_sondeo
clic en una etiqueta: Index=0 | Document=HANGAR_PRUEBA_sondeo
clic en una etiqueta: Index=0 | Document=HANGAR_PRUEBA_sondeo
2) estado del sondeo 19: control(es) 0 en la vista 1245519
2) GetAll() antes de quitar: 0 control(es)
3) RemoveControl(0) fallo: index is out of range of TemporaryGraphicsManager managed objects, or the indexed object has been removed from the document.
Parameter name: index
4) TemporaryGraphicsManager.Clear() llamado
4) GetAll() despues de quitar: 0 control(es) (debe ser 0)
4) vista refrescada (RefreshActiveView + UpdateAllOpenViews)
5) el servidor de prueba no esta registrado en TemporaryGraphicsHandlerService (registrados: 1)
6) Marcadores DirectShape de plan en el modelo (no los toca este sondeo): 0
=== fin 19b-etiquetas-quitar ===


```

## 8e-7b probar_conexiones --puente (repetido)

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.8.5 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"data":{"addin_version":"0.8.5","revit":{"version_number":"2027","language":"English_USA","sub_version_number":"2027.2","version_build":"27.2.0.39","version_name":"Autodesk Revit 2027"},"has_uidocument":true,"backend":"advancesteel","dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12","load_context":"Default"},"operations":["batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"spec_version":"1.0","document":{"is_read_only":false,"title":"HANGAR_PRUEBA_sondeo","path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_modifiable":false,"is_workshared":false,"is_family":false}},"ok":true,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"ping","duration_ms":3},"errors":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 19183 caracteres
Cuerpo:
{"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"ok":true,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"types","duration_ms":1},"errors":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"data":{"json_schema":{"type":"object","required":["spec_version","connection_type","node","chord","gusset","members"],"additionalProperties":false,"title":"GussetNodeConnectionSpec","$schema":"http://json-schema.org/draft-07/schema#","properties":{"chord":{"type":"object","additionalProperties":false,"required":["element_id","continuous"],"properties":{"continuous":{"type":"boolean"},"profile":{"type":["string","null"]},"element_id":{"type":"integer"}}},"dimension_chains":{"type":"array","items":{"type":"object","additionalProperties":false,"required":["values_mm","expected_total_mm"],"properties":{"label":{"type":"string"},"values_mm":{"type":"array","minItems":1,"items":{"type":"number"}},"expected_total_mm":{"type":"number","minimum":0.0}}}},"node":{"type":"object","additionalProperties":false,"required":["element_ids"],"properties":{"element_ids":{"type":"array","minItems":2,"items":{"type":"integer"}}}},"gusset":{"type":"object","additionalProperties":false,"required":["thickness_mm","width_mm","height_mm","outline"],"properties":{"thickness_mm":{"type":"number","minimum":1.0},"weld_to_chord":{"type":"object","additionalProperties":false,"required":["type","size_mm"],"properties":{"type":{"type":"string","enum":["fillet"]},"size_mm":{"type":"number","minimum":1.0},"all_around":{"type":"boolean"}}},"thickness_label":{"type":"string"},"chord_interface":{"type":["string","null"],"enum":["through_slot","split_top_bottom","side_lap",null]},"width_mm":{"type":"number","minim ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"schema","duration_ms":0},"errors":[{"code":"UNKNOWN_OPERATION","hint":"Tipos disponibles: gusset_node","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","path":"type"}]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"data": {"query": "HSS2-1/2X2-1/2X3/16", "suggestions": [], "total_profiles_in_model": 29, "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}], "matched_count": 1}, "ok": true, "warnings": [], "meta": {"addin_version": "0.8.5", "operation": "find_profile", "duration_ms": 37}, "errors": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"data": {"members": [{"type": "HSS3X3X1/4", "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "angle_in_plane_deg": 0, "angle_to_chord_deg": 0, "side": "chord", "slope_deg": 0, "length_mm": 9960.2999999999993, "structural_type": "Beam", "node_end": 1, "element_id": 1249510, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "is_chord": true, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "family": "HSS-Hollow Structural Section"}, {"type": "HSS2-1-2X2-1-2X3-16 64x64", "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "angle_in_plane_deg": 136.90000000000001, "angle_to_chord_deg": 43.100000000000001, "side": "+Y", "slope_deg": 43.079999999999998, "length_mm": 3568, "structural_type": "Beam", "node_end": 1, "element_id": 1249630, "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "is_chord": false, "material": "Material IFC (190-40-140)", "family": "HSS2-1-2X2-1-2X3-16 64x64"}, {"type": "HSS2-1-2X2-1-2X3-16 64x64", "end_mm": [-9354.7000000000007, -17195.799999999999, 19884.900000000001], "angle_in_plane_deg": 44.399999999999999, "angle_to_chord_deg": 44.399999999999999, "side": "+Y", "slope_deg": 44.369999999999997, "length_mm": 3500, "structural_type": "Beam", "node_end": 0, "element_id": 1249631, "start_mm": [-11856.5, -17195.799999999999, 17437.299999999999], "is_chord": false, "material": "Material IFC (190-40-140)", "family": "HSS2-1-2X2-1-2X3-16 64x64"}, {"type": "HSS2-1-2X2-1-2X3-16 ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"data":{"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","calculated_values":{"frame_z":[0,-1,0],"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"frame_x":[1,0,0],"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"chord_direction_reversed":true},"warnings_count":1,"is_valid":true,"bolt_stacks":[{"grip_mm":19.524999999999999,"gusset_face":"+z","member_element_id":1249636,"bolt_length_mm":44.450000000000003,"length_source":"computed_from_grip"}],"errors_count":0},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg"}],"meta":{"addin_version":"0.8.5","operation":"validate","duration_ms":128},"errors":[]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg"}],"meta":{"addin_version":"0.8.5","operation":"validate","duration_ms":14},"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","path":"dimension_chains[0].values_mm"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg"}],"meta":{"addin_version":"0.8.5","operation":"validate","duration_ms":9},"errors":[{"code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","path":"uncertain_fields[0].user_confirmed_value"},{"code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","path":"uncertain_fields[1].user_confirmed_value"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"working_point_mm": [-11867.7, -17195.8, 17423], "weld_lines": 6, "dry_run": true, "knife_plates": 1, "chord_element_id": 1249510, "gusset_plates": 1, "backend": "advancesteel", "bolts": 4, "members_modified": 3, "connection_type": "gusset_node", "first_member_element_id": 1249630}
Cuerpo:
{"data": {"summary": {"working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "weld_lines": 6, "dry_run": true, "knife_plates": 1, "chord_element_id": 1249510, "gusset_plates": 1, "backend": "advancesteel", "bolts": 4, "members_modified": 3, "connection_type": "gusset_node", "first_member_element_id": 1249630}, "elements_to_create": [{"thickness_mm": 9.5250000000000004, "thickness_label": "3/8\"", "chord_interface": "through_slot", "width_mm": 565, "kind": "gusset_plate", "height_mm": 530, "vertices_count": 8}, {"for_member_id": 1249630, "weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface"}, {"for_member_id": 1249631, "weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface"}, {"for_member_id": 1249636, "gusset_face": "+z", "thickness_mm": 10, "length_mm": 170, "width_mm": 140, "offset_from_gusset_plane_mm": 9.7620000000000005, "kind": "knife_plate", "insertion_mm": 80}, {"for_member_id": 1249636, "grip_mm": 19.524999999999999, "length_mm": 44.450000000000003, "spacing_mm": 60, "kind": "bolt_group", "columns": 2, "count": 4, "edge_mm": 40, "rows": 2, "length_source": "computed_from_grip", "diameter_mm": 15.875}], "members_to_modify": [{"profile": "HSS2-1-2X2-1-2X3-16 64x64", "element_id": 1249630, "role": "diagonal", "end": "end", "new_extension_mm": -93.799999999999997, "setback_mm": 180, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 86.20000000 ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"create","duration_ms":1},"errors":[{"code":"VALIDATION_TOKEN_INVALID","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","message":"validation_token es obligatorio para crear una conexi\u00f3n.","path":"validation_token"}]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"data": {"connections_count": 0, "connections": []}, "ok": true, "warnings": [], "meta": {"addin_version": "0.8.5", "operation": "list", "duration_ms": 4}, "errors": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"get","duration_ms":6},"errors":[{"code":"ELEMENT_NOT_FOUND","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id"}]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"delete","duration_ms":4},"errors":[{"code":"ELEMENT_NOT_FOUND","hint":"Verifica los IDs disponibles con conn_list.","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id"}]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"no_existe","duration_ms":0},"errors":[{"code":"UNKNOWN_OPERATION","hint":"Operaciones disponibles: batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","path":null}]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"data":{"templates_count":1,"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates":[{"chord_profile":"HSS3X3X1/4","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","members_count":3,"created_utc":"2026-10-04T22:01:50.5585233Z","origin_drawing":"Detalle D","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","name":"Nudo tipico Detalle D","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","connection_type":"gusset_node","tags":["hangar","cercha","HSS"],"origin_document":"HANGAR_PRUEBA_sondeo"}]},"ok":true,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"catalog_list","duration_ms":23},"errors":[]}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=b83034f6-408e-407f-bbb3-ad74e77095d5 barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\b83034f6-408e-407f-bbb3-ad74e77095d5.json
Cuerpo:
{"data":{"origin":{"element_ids":[1249510,1249630,1249631,1249636],"drawing":"Detalle D","connection_id":null,"document":"HANGAR_PRUEBA_sondeo"},"chord_profile":"HSS3X3X1/4","template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5","member_pattern":[{"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16","slot":0,"angle_deg":136.91999999999999,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16","slot":1,"angle_deg":44.369999999999997,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"side":"-Y","profile":"HSS2-1/2X2-1/2X3/16","slot":2,"angle_deg":-135.63000000000000,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"}],"members_count":3,"shared_file":null,"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\b83034f6-408e-407f-bbb3-ad74e77095d5.json","name":"PRUEBA probar_conexiones"},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg"}],"meta":{"addin_version":"0.8.5","operation":"catalog_save","duration_ms":40},"errors":[]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"data":{"template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5","template":{"origin":{"element_ids":[1249510,1249630,1249631,1249636],"drawing":"Detalle D","connection_id":null,"document":"HANGAR_PRUEBA_sondeo"},"member_pattern":[{"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16","slot":0,"angle_deg":136.91999999999999,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16","slot":1,"angle_deg":44.369999999999997,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"},{"side":"-Y","profile":"HSS2-1/2X2-1/2X3/16","slot":2,"angle_deg":-135.63000000000000,"role":"diagonal","profile_policy":"warn","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64"}],"template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5","created_utc":"2026-10-06T12:56:25.5371719Z","matching":{"allow_mirror":true,"angle_tolerance_deg":10},"description":null,"catalog_version":"1.0","name":"PRUEBA probar_conexiones","chord_pattern":{"continuous":true,"profile":"HSS3X3X1/4","profile_policy":"warn"},"connection_type":"gusset_node","tags":["prueba"],"spec_template":{"chord":{"continuous":true,"profile":"HSS3X3X1/4"},"dimension_chains":[{"label":"borde superior","values_mm":[75.0,420.0,70.0],"expected_total_mm":565.0},{"label":"base","values_mm":[125.0,90.0,350.0],"expected_total_mm":565.0},{"label":"lado derecho","values_mm":[70.0,250.0,210.0],"expected_total_mm":530.0},{"label":"lado izquierdo","values_mm":[70.0,325.0,135.0 ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=473c07648def...
Cuerpo:
{"data":{"template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5","spec":{"chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"dimension_chains":[{"label":"borde superior","values_mm":[75.0,420.0,70.0],"expected_total_mm":565.0},{"label":"base","values_mm":[125.0,90.0,350.0],"expected_total_mm":565.0},{"label":"lado derecho","values_mm":[70.0,250.0,210.0],"expected_total_mm":530.0},{"label":"lado izquierdo","values_mm":[70.0,325.0,135.0],"expected_total_mm":530.0}],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"gusset":{"thickness_mm":9.5250000000000004,"weld_to_chord":{"type":"fillet","size_mm":5.0,"all_around":true},"thickness_label":"3/8\"","chord_interface":"through_slot","width_mm":565.0,"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"height_mm":530.0},"uncertain_fields":[],"source":{"drawing":"Detalle D","scale":"1/10","template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5"},"connection_type":"gusset_node","spec_version":"1.0","members":[{"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"role":"diagonal","element_id":1249630,"expected_angle_deg":43.100000000000001,"end_setback_mm":180.0},{"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"r ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=0c48107f-7b57-4f4f-9ee2-91ac35ebaa04 nudos=6 resumen={'untyped': 5, 'ready': 1} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"data":{"unused_element_ids":[],"templates":{"b83034f6-408e-407f-bbb3-ad74e77095d5":"PRUEBA probar_conexiones"},"plan_id":"0c48107f-7b57-4f4f-9ee2-91ac35ebaa04","marks":{"element_count":0,"marker_element_ids":[]},"is_marked":false,"created_utc":"2026-10-06T12:56:25.7442102Z","marked_view_id":null,"updated_utc":"2026-10-06T12:56:25.7444385Z","summary":{"untyped":5,"ready":1},"nodes":[{"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"status":"untyped","is_mirrored":false,"spec":null,"advice":"No es un nudo: nada que hacer","color_rgb":null,"members":[],"errors_count":0,"errors":[],"warnings_count":0,"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","is_manual":false,"element_ids":[1249630],"name":"N1","has_spec_override":false,"template_name":null,"chord_continuous":false,"chord_element_id":0,"max_deviation_deg":null,"visible_by_default":false,"warnings":[],"is_valid":false,"template_id":null,"color_name":null,"attempts":[],"is_marked":false,"match":null,"orientation":null,"validation_token":null,"marker_element_id":null,"chord_type_name":null,"existing_connection_id":null,"member_element_ids":[1249630],"replaces_existing":false,"signature":"1 barra(s) sin marco","through_element_ids":[],"status_text":"\u25cb Barra suelta (no es nudo)"},{"work_point_mm":[-14455.299999999999,-17195.700000000001,14894.700000000001],"status":"untyped","is_mirrored":false,"spec":null,"advice":"No es un nudo: nada que hacer","color_rg ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=75bfd64eb09c...
Cuerpo:
{"data":{"node":{"work_point_mm":[-11870,-17195.799999999999,17423],"status":"ready","is_mirrored":false,"spec":{"chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"dimension_chains":[{"label":"borde superior","values_mm":[75.0,420.0,70.0],"expected_total_mm":565.0},{"label":"base","values_mm":[125.0,90.0,350.0],"expected_total_mm":565.0},{"label":"lado derecho","values_mm":[70.0,250.0,210.0],"expected_total_mm":530.0},{"label":"lado izquierdo","values_mm":[70.0,325.0,135.0],"expected_total_mm":530.0}],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"gusset":{"thickness_mm":9.5250000000000004,"weld_to_chord":{"type":"fillet","size_mm":5.0,"all_around":true},"thickness_label":"3/8\"","chord_interface":"through_slot","width_mm":565.0,"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"height_mm":530.0},"uncertain_fields":[],"source":{"batch_id":"0c48107f-7b57-4f4f-9ee2-91ac35ebaa04","drawing":"Detalle D","scale":"1/10","template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5"},"connection_type":"gusset_node","spec_version":"1.0","members":[{"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"role":"diagonal","element_id":1249630,"expected_angle_deg":43.100000000000001,"end_setback_mm":180.0},{"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"type":"wel ...
======================================================================
24. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado 0c48107f-7b57-4f4f-9ee2-91ac35ebaa04
Cuerpo:
{"data": {"remaining_plans": 0, "discarded_plan_id": "0c48107f-7b57-4f4f-9ee2-91ac35ebaa04", "removed_marks": 0, "remaining_markers": 0}, "ok": true, "warnings": [], "meta": {"addin_version": "0.8.5", "operation": "batch_plan_discard", "duration_ms": 38}, "errors": []}
======================================================================
25. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"catalog_apply","duration_ms":1},"errors":[{"code":"TEMPLATE_NOT_FOUND","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","path":"template_id"}]}
======================================================================
26. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada b83034f6-408e-407f-bbb3-ad74e77095d5
Cuerpo:
{"data":{"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\b83034f6-408e-407f-bbb3-ad74e77095d5.json","deleted_template_id":"b83034f6-408e-407f-bbb3-ad74e77095d5","name":"PRUEBA probar_conexiones"},"ok":true,"warnings":[],"meta":{"addin_version":"0.8.5","operation":"catalog_delete","duration_ms":3},"errors":[]}
======================================================================
27. tools/list por el puente trae las 21 herramientas conn_*  [OK]  HTTP 200, herramientas=87 conn_*=21
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard
======================================================================
28. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.8.5
Cuerpo:
{
  "data": {
    "addin_version": "0.8.5",
    "revit": {
      "version_number": "2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39",
      "version_name": "Autodesk Revit 2027"
    },
    "has_uidocument": true,
    "backend": "advancesteel",
    "dotnet": {
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12",
      "load_context": "Default"
    },
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
    "spec_version": "1.0",
    "document": {
      "is_read_only": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_modifiable": false,
      "is_workshared": false,
      "is_family": false
    }
  },
  "ok": true,
  "warnings": [],
  "meta": {
    "addin_version": "0.8.5",
    "operation": "ping",
    "duration_ms": 2
  },
  "errors": []
}
======================================================================
Resultado: 28/28 pruebas correctas

```

## 8e-7b log del dia (repetido)

```text

{"ts":"2026-10-06T07:25:46.8480821-05:00","record":{"event":"startup","addin_version":"0.8.5","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-06T07:54:47.5460909-05:00","record":{"event":"startup","addin_version":"0.8.5","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-06T07:56:25.8099911-05:00","record":{"event":"batch_plan","plan_id":"0c48107f-7b57-4f4f-9ee2-91ac35ebaa0
4","replan":false,"element_ids":4,"templates":["b83034f6-408e-407f-bbb3-ad74e77095d5"],"summary":{"untyped":5,"ready":1
},"marked":false,"overrides":"{\"exclude\":[],\"add_node\":{},\"chord\":{},\"template\":{},\"remove_member\":{},\"add_m
ember\":{},\"merge\":[],\"split\":{},\"spec\":{},\"replace_existing\":false}"}}
{"ts":"2026-10-06T07:56:25.8345044-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"ele
ment_ids\": [1249510, 1249630, 1249631, 1249636], \"mark\": false, \"template_ids\": 
[\"b83034f6-408e-407f-bbb3-ad74e77095d5\"]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":109}}
{"ts":"2026-10-06T07:56:25.8876533-05:00","record":{"event":"handle","operation":"batch_plan_get","request_summary":"{\
"node\": \"N4\", \"plan_id\": 
\"0c48107f-7b57-4f4f-9ee2-91ac35ebaa04\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-06T07:56:25.9553634-05:00","record":{"event":"batch_plan_discard","plan_id":"0c48107f-7b57-4f4f-9ee2-91a
c35ebaa04","removed_marks":0}}
{"ts":"2026-10-06T07:56:25.9621869-05:00","record":{"event":"handle","operation":"batch_plan_discard","request_summary"
:"{\"plan_id\": 
\"0c48107f-7b57-4f4f-9ee2-91ac35ebaa04\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":38}}



```
