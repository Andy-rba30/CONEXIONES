# Resultados de la ronda 10b

Fecha: 2026-10-11T18:24:27


## 10b-1 git

```text
ab38d13 Cierre de la Fase 10: resultados contrastados y ronda 10b (0.10.1): el cuadro de la selecci├│n asistida ya se abre, instalaci├│n solo de lo no probado
?? docs/fases/resultados-fase-10b.md

```

## 10b-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 921 ms).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 921 ms).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 921 ms).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:14.57
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:   217, Omitido:     0, Total:   217, Duraci├│n: 627 ms - MotorConexiones.Tests.dll (net10.0)

```

## 10b-1 revit cerrado

```text

```

## 10b-1 deploy

```text
== MotorConexiones 0.10.1.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 10b-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 10b-1 version de la dll

```text
0.10.1.0

```

## 10b-2 ping

```text
== conn/ping -> HTTP 200 en 376 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "ping",
                 "duration_ms":  9,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "addin_version":  "0.10.1",
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
                                    "types",
                                    "update",
                                    "validate"
                                ],
                 "revit":  {
                               "version_build":  "27.2.0.39",
                               "version_number":  "2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_name":  "Autodesk Revit 2027"
                           },
                 "document":  {
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_family":  false,
                                  "is_read_only":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_workshared":  false
                              },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
                            }
             },
    "warnings":  [

                 ]
}

```

## 10b-2 catalog_list antes

```text
== conn/catalog_list -> HTTP 200 en 88 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "catalog_list",
                 "duration_ms":  46,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_drawing":  "Detalle D",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ]
                                   },
                                   {
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\023b17e5-3f5b-42da-9efc-5ce0dcc1547a.json",
                                       "created_utc":  "2026-10-06T17:18:11.7663608Z",
                                       "name":  "PRUEBA probar_conexiones",
                                       "template_id":  "023b17e5-3f5b-42da-9efc-5ce0dcc1547a",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_drawing":  "Detalle D",
                                       "description":  null,
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "prueba"
                                                ]
                                   }
                               ],
                 "templates_count":  2,
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "warnings":  [

                 ]
}

```

## 10b-2 catalog_delete de la plantilla sobrante

```text
== conn/catalog_delete -> HTTP 200 en 18 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "catalog_delete",
                 "duration_ms":  4,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "name":  "PRUEBA probar_conexiones",
                 "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\023b17e5-3f5b-42da-9efc-5ce0dcc1547a.json",
                 "deleted_template_id":  "023b17e5-3f5b-42da-9efc-5ce0dcc1547a"
             },
    "warnings":  [

                 ]
}

```

## 10b-2 catalog_list despues

```text
== conn/catalog_list -> HTTP 200 en 26 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "catalog_list",
                 "duration_ms":  12,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_drawing":  "Detalle D",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ]
                                   }
                               ],
                 "templates_count":  1,
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "warnings":  [

                 ]
}

```

## 10b-2 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 179 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 14 ms | errores=- | avisos=-
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

## 10b-2 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 41 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 10b-2 sondeo 21 etiquetas antes

```text
== 21-etiquetas-addin.py -> HTTP 200 en 318 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 0
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 33 BMP
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
   etiqueta-ambar-39.bmp (3126 bytes)
   etiqueta-ambar-4.bmp (3126 bytes)
   etiqueta-ambar-41.bmp (3126 bytes)
   etiqueta-ambar-46.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 0 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 10b-2 log de arranque

```text

{"ts":"2026-10-11T18:27:00.4500730-05:00","record":{"event":"label_handler_registered","server":"3f6c1b2e-7a8d-4c5b-9e2
1-0d4f5a6b7c8d","already_registered":false,"service":"TemporaryGraphicsHandlerService","activation":"multi:2"}}
{"ts":"2026-10-11T18:27:00.4544575-05:00","record":{"event":"startup","addin_version":"0.10.1","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}



```

## 10b-5 sondeo 21 tras descartar

```text
== 21-etiquetas-addin.py -> HTTP 200 en 454 ms ==
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

## 10b-5 sondeo 17 tras descartar

```text
== 17-marcas-plan.py -> HTTP 200 en 712 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322638] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 10b-5 ping antes del puente

```text
== conn/ping -> HTTP 200 en 71 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "ping",
                 "duration_ms":  2,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "addin_version":  "0.10.1",
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
                                    "types",
                                    "update",
                                    "validate"
                                ],
                 "revit":  {
                               "version_build":  "27.2.0.39",
                               "version_number":  "2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_name":  "Autodesk Revit 2027"
                           },
                 "document":  {
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_family":  false,
                                  "is_read_only":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_workshared":  false
                              },
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
                            }
             },
    "warnings":  [

                 ]
}

```

## 10b-5 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         35744



```

## 10b-5 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.10.1 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"ping","duration_ms":9,"addin_version":"0.10.1"},"data":{"addin_version":"0.10.1","has_uidocument":true,"spec_version":"1.0","backend":"advancesteel","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"revit":{"version_build":"27.2.0.39","version_number":"2027","language":"English_USA","sub_version_number":"2027.2","version_name":"Autodesk Revit 2027"},"document":{"is_modifiable":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_family":false,"is_read_only":false,"title":"HANGAR_PRUEBA_sondeo","is_workshared":false},"dotnet":{"framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","load_context":"Default"}},"warnings":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 23845 caracteres
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"guide","duration_ms":7,"addin_version":"0.10.1"},"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\ ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"types","duration_ms":1,"addin_version":"0.10.1"},"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"warnings":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"schema","duration_ms":1,"addin_version":"0.10.1"},"data":{"description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","example":{"source":{"scale":"1/10","drawing":"Detalle D"},"spec_version":"1.0","uncertain_fields":[],"connection_type":"gusset_node","chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"gusset":{"width_mm":565.0,"chord_interface":"through_slot","thickness_label":"3/8\"","weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"height_mm":530.0,"thickness_mm":9.5250000000000004},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"role":"diagonal","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"expected_angle_deg":45.0}],"dimension_chains":[{"expected_total_mm":565.0,"label":"borde superior","values_mm":[75.0,420.0,70.0]}]},"connection_type":"gusset_node","json_schema":{"additionalProperties":false,"type":"object","required":["spec_version","connection_type","node","chord","gusset","members"],"$schema":"http://json-schema.org/draft-07/schema#","properties":{"source":{"additionalPr ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"ok":false,"errors":[{"code":"UNKNOWN_OPERATION","path":"type","hint":"Tipos disponibles: gusset_node","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado."}],"meta":{"operation":"schema","duration_ms":0,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "find_profile", "duration_ms": 7, "addin_version": "0.10.1"}, "data": {"matched_count": 1, "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "query": "HSS2-1/2X2-1/2X3/16", "total_profiles_in_model": 29, "suggestions": []}, "warnings": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "node_info", "duration_ms": 17, "addin_version": "0.10.1"}, "data": {"frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "z_axis": [1.9999999999999999e-06, -1, 0], "x_axis": [1, 1.9999999999999999e-06, 0], "axis_distance_mm": 0.080000000000000002, "origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "y_axis": [0, 0, 1], "chord_direction_reversed": true, "chord_element_id": 1249510, "existing_connections": [], "members": [{"structural_type": "Beam", "element_id": 1249510, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "length_mm": 9960.2999999999993, "family": "HSS-Hollow Structural Section", "type": "HSS3X3X1/4", "node_end": 1, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "material": "Steel ASTM A500, Grade B, Rectangular and Square", "angle_in_plane_deg": 0, "is_chord": true, "side": "chord", "angle_to_chord_deg": 0, "slope_deg": 0}, {"structural_type": "Beam", "element_id": 1249630, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "length_mm": 3568, "family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "node_end": 1, "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "material": "Material IFC (190-40-140)", "angle_in_plane_deg": 136.90000000000001, "is_chord": false, "side": "+Y", "angle_to_chord_deg": 43.100000000000001, "slope_deg": 43. ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"validate","duration_ms":16,"addin_version":"0.10.1"},"data":{"warnings_count":1,"is_valid":true,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","calculated_values":{"frame_y":[0,0,1],"frame_z":[0,-1,0],"axis_distance_mm":0.080000000000000002,"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"chord_direction_reversed":true,"frame_x":[1,0,0]},"bolt_stacks":[{"length_source":"computed_from_grip","gusset_face":"+z","grip_mm":19.524999999999999,"bolt_length_mm":44.450000000000003,"member_element_id":1249636}],"errors_count":0},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"ok":false,"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","path":"dimension_chains[0].values_mm","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm)."}],"meta":{"operation":"validate","duration_ms":8,"addin_version":"0.10.1"},"data":null,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"ok":false,"errors":[{"code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[0].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen"},{"code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[1].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n"}],"meta":{"operation":"validate","duration_ms":7,"addin_version":"0.10.1"},"data":null,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"knife_plates": 1, "connection_type": "gusset_node", "gusset_plates": 1, "weld_lines": 6, "first_member_element_id": 1249630, "backend": "advancesteel", "members_modified": 3, "dry_run": true, "working_point_mm": [-11867.7, -17195.8, 17423], "chord_element_id": 1249510, "bolts": 4}
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "preview", "duration_ms": 10, "addin_version": "0.10.1"}, "data": {"elements_to_create": [{"width_mm": 565, "chord_interface": "through_slot", "thickness_label": "3/8\"", "vertices_count": 8, "kind": "gusset_plate", "height_mm": 530, "thickness_mm": 9.5250000000000004}, {"weld_size_mm": 5, "kind": "welded_slot_interface", "slot_length_mm": 150, "for_member_id": 1249630}, {"weld_size_mm": 5, "kind": "welded_slot_interface", "slot_length_mm": 150, "for_member_id": 1249631}, {"width_mm": 140, "gusset_face": "+z", "length_mm": 170, "kind": "knife_plate", "insertion_mm": 80, "thickness_mm": 10, "for_member_id": 1249636, "offset_from_gusset_plane_mm": 9.7620000000000005}, {"edge_mm": 40, "length_source": "computed_from_grip", "columns": 2, "rows": 2, "length_mm": 44.450000000000003, "kind": "bolt_group", "count": 4, "grip_mm": 19.524999999999999, "spacing_mm": 60, "for_member_id": 1249636, "diameter_mm": 15.875}], "members_to_modify": [{"element_id": 1249630, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "new_extension_mm": -93.799999999999997, "end": "end", "role": "diagonal", "setback_mm": 180, "current_end_distance_mm": 86.200000000000003, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"}, {"element_id": 1249631, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "new_extension_mm": -42, "end": "start", "role": "diagonal", "setback_mm": 60, "current_end_distance_mm": 18, "action": "Fijar Start/End E ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"ok":false,"errors":[{"code":"VALIDATION_TOKEN_INVALID","path":"validation_token","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","message":"validation_token es obligatorio para crear una conexi\u00f3n."}],"meta":{"operation":"create","duration_ms":0,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "list", "duration_ms": 3, "addin_version": "0.10.1"}, "data": {"batches": {}, "total_count": 0, "connections_count": 0, "batch_id": null, "connections": []}, "warnings": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"ok":false,"errors":[{"code":"ELEMENT_NOT_FOUND","path":"connection_id","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'."}],"meta":{"operation":"get","duration_ms":4,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"ok":false,"errors":[{"code":"ELEMENT_NOT_FOUND","path":"connection_id","hint":"Verifica los IDs disponibles con conn_list.","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'."}],"meta":{"operation":"delete","duration_ms":4,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"ok":false,"errors":[{"code":"UNKNOWN_OPERATION","path":null,"hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","message":"La operaci\u00f3n 'no_existe' no existe en el add-in."}],"meta":{"operation":"no_existe","duration_ms":0,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"catalog_list","duration_ms":2,"addin_version":"0.10.1"},"data":{"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates":[{"connection_type":"gusset_node","members_count":3,"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","created_utc":"2026-10-04T22:01:50.5585233Z","name":"Nudo tipico Detalle D","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","origin_drawing":"Detalle D","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","chord_profile":"HSS3X3X1/4","origin_document":"HANGAR_PRUEBA_sondeo","tags":["hangar","cercha","HSS"]}],"templates_count":1,"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog"},"warnings":[]}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=a2730872-39b8-4e8a-b62a-8279b3e9b47e barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\a2730872-39b8-4e8a-b62a-8279b3e9b47e.json
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"catalog_save","duration_ms":35,"addin_version":"0.10.1"},"data":{"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\a2730872-39b8-4e8a-b62a-8279b3e9b47e.json","members_count":3,"member_pattern":[{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":0,"angle_deg":136.91999999999999,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":1,"angle_deg":44.369999999999997,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"profile":"HSS2-1/2X2-1/2X3/16","profile_policy":"warn","role":"diagonal","slot":2,"angle_deg":-135.63000000000000,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y"}],"name":"PRUEBA probar_conexiones","template_id":"a2730872-39b8-4e8a-b62a-8279b3e9b47e","shared_file":null,"chord_profile":"HSS3X3X1/4","origin":{"document":"HANGAR_PRUEBA_sondeo","connection_id":null,"element_ids":[1249510,1249630,1249631,1249636],"drawing":"Detalle D"},"matching":{"angle_tolerance_deg":10,"allow_mirror":true}},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0."}]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"catalog_get","duration_ms":1,"addin_version":"0.10.1"},"data":{"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\a2730872-39b8-4e8a-b62a-8279b3e9b47e.json","template":{"connection_type":"gusset_node","chord_pattern":{"continuous":true,"profile_policy":"warn","profile":"HSS3X3X1/4"},"created_utc":"2026-10-11T23:38:15.3906858Z","spec_template":{"source":{"scale":"1/10","drawing":"Detalle D"},"uncertain_fields":[],"spec_version":"1.0","connection_type":"gusset_node","chord":{"continuous":true,"profile":"HSS3X3X1/4"},"gusset":{"width_mm":565.0,"chord_interface":"through_slot","thickness_label":"3/8\"","weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"height_mm":530.0,"thickness_mm":9.5250000000000004},"members":[{"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"role":"diagonal","slot":0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"expected_angle_deg":45.0},{"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"role":"diagonal","slot":1,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"expected_angle_deg":45.0},{"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":260.0,"role":"diagonal ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=43c2685eab66...
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"catalog_apply","duration_ms":18,"addin_version":"0.10.1"},"data":{"spec":{"source":{"template_id":"a2730872-39b8-4e8a-b62a-8279b3e9b47e","scale":"1/10","drawing":"Detalle D"},"uncertain_fields":[],"spec_version":"1.0","connection_type":"gusset_node","chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"gusset":{"width_mm":565.0,"chord_interface":"through_slot","thickness_label":"3/8\"","weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"outline":{"points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]],"mode":"polygon"},"height_mm":530.0,"thickness_mm":9.5250000000000004},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"role":"diagonal","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"expected_angle_deg":43.100000000000001},{"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"role":"diagonal","attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"expected_angle_deg":44.399999999999999},{"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":260.0,"role":"diagonal","attachment":{"type":"bolted_knife_plate","bolts":{"edge_mm":40.0,"diameter_label":"5/8\"","columns": ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=c0413a80-1fd0-4b79-90cc-1a51cb0c5bf9 nudos=6 resumen={'untyped': 5, 'ready': 1} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"batch_plan","duration_ms":32,"addin_version":"0.10.1"},"data":{"has_batch_connections":false,"templates":{"a2730872-39b8-4e8a-b62a-8279b3e9b47e":"PRUEBA probar_conexiones"},"last_report":null,"created_utc":"2026-10-11T23:38:15.5504344Z","label_view_id":null,"nodes":[{"color_rgb":null,"has_spec_override":false,"max_deviation_deg":null,"members":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","created_connection_id":null,"is_mirrored":false,"is_manual":false,"chord_continuous":false,"signature":"1 barra(s) sin marco","template_name":null,"visible_by_default":false,"validation_token":null,"color_name":null,"element_ids":[1249630],"through_element_ids":[],"spec":null,"existing_batch_id":null,"template_id":null,"attempts":[],"warnings_count":0,"replaces_existing":false,"ghost_element_id":null,"status_text":"\u25cb Barra suelta (no es nudo)","actions":[],"match":null,"errors_count":0,"errors":[],"status":"untyped","marker_element_id":null,"orientation":null,"chord_element_id":0,"warnings":[],"name":"N1","work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"is_valid":false,"advice":"No es un nudo: nada que hacer","member_element_ids":[1249630],"existing_connection_id":null,"chord_type_name":null,"label_index":null,"is_marked":false},{"color_rgb":null,"has_spec_override":false,"max_deviation_deg":null,"members":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empa ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=0caf07f45f94...
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"batch_plan_get","duration_ms":1,"addin_version":"0.10.1"},"data":{"node":{"color_rgb":[46,160,67],"has_spec_override":false,"max_deviation_deg":0,"members":[{"element_id":1249630,"end_gap_mm":84.5,"reaches_node":true,"angle_deg":136.90000000000001,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"element_id":1249631,"end_gap_mm":19.600000000000001,"reaches_node":true,"angle_deg":44.399999999999999,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y"},{"element_id":1249636,"end_gap_mm":48.200000000000003,"reaches_node":true,"angle_deg":-135.59999999999999,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y"}],"status_detail":null,"created_connection_id":null,"is_mirrored":false,"is_manual":false,"chord_continuous":true,"signature":"2 +Y (137, 44) \u00b7 1 -Y (-136)","template_name":"PRUEBA probar_conexiones","visible_by_default":true,"validation_token":"0caf07f45f94e60226d11c941866de5f091893e0be57327ee639de6523f131b7","color_name":"verde","element_ids":[1249510,1249630,1249631,1249636],"through_element_ids":[1249510],"spec":{"source":{"template_id":"a2730872-39b8-4e8a-b62a-8279b3e9b47e","batch_id":"c0413a80-1fd0-4b79-90cc-1a51cb0c5bf9","scale":"1/10","drawing":"Detalle D"},"uncertain_fields":[],"spec_version":"1.0","connection_type":"gusset_node","chord":{"continuous":true,"profile":"HSS3X3X1/4","element_id":1249510},"gusset":{"width_mm":565.0,"chord_interface":"through_slot","thickness_label":"3/8\"","weld_to_chord":{"size_mm":5. ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote c0413a80: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"batch_create","duration_ms":12,"addin_version":"0.10.1"},"data":{"connection_ids":[],"stop_on_error":false,"nodes":[{"spec":null,"elements_count":0,"reason":"token distinto del plan","errors":[{"code":"VALIDATION_TOKEN_INVALID","path":"nodes[N4].validation_token","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)."}],"connection_id":null,"orientation":"same","duration_ms":0,"restored_members_count":0,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","node":"N4","template_name":"PRUEBA probar_conexiones","warnings":[],"outcome":"failed"}],"plan_creatable_count":1,"stopped":false,"failed_count":1,"summary_text":"Lote c0413a80: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","started_utc":"2026-10-11T23:38:15.6910953Z","plan_id":"c0413a80-1fd0-4b79-90cc-1a51cb0c5bf9","skipped_count":0,"updated_count":0,"document":"HANGAR_PRUEBA_sondeo","restored_members_count":0,"plan_summary":{"untyped":5,"failed":1},"deleted_count":0,"deleted_elements_count":0,"finished_utc":"2026-10-11T23:38:15.6923330Z","with_warnings_count":0,"stopped_at": ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"batch_delete","duration_ms":10,"addin_version":"0.10.1"},"data":{"connection_ids":[],"stop_on_error":false,"nodes":[],"plan_creatable_count":null,"stopped":false,"failed_count":0,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","started_utc":"2026-10-11T23:38:15.7393495Z","plan_id":null,"skipped_count":0,"updated_count":0,"document":"HANGAR_PRUEBA_sondeo","restored_members_count":0,"plan_summary":null,"deleted_count":0,"deleted_elements_count":0,"finished_utc":"2026-10-11T23:38:15.7432204Z","with_warnings_count":0,"stopped_at":null,"batch_id":"00000000-0000-0000-0000-000000000000","operation":"batch_delete","created_count":0,"rolled_back_count":0,"undo_entries":"one","duration_ms":3,"plan_summary_text":null},"warnings":[{"code":"BATCH_EMPTY","path":"batch_id","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar."}]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado c0413a80-1fd0-4b79-90cc-1a51cb0c5bf9
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "batch_plan_discard", "duration_ms": 14, "addin_version": "0.10.1"}, "data": {"removed_marks": 0, "remaining_plans": 0, "remaining_markers": 0, "remaining_labels": 0, "discarded_plan_id": "c0413a80-1fd0-4b79-90cc-1a51cb0c5bf9", "removed_labels": 0}, "warnings": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"ok":false,"errors":[{"code":"TEMPLATE_NOT_FOUND","path":"template_id","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo."}],"meta":{"operation":"catalog_apply","duration_ms":0,"addin_version":"0.10.1"},"data":null,"warnings":[]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada a2730872-39b8-4e8a-b62a-8279b3e9b47e
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"catalog_delete","duration_ms":7,"addin_version":"0.10.1"},"data":{"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\a2730872-39b8-4e8a-b62a-8279b3e9b47e.json","deleted_template_id":"a2730872-39b8-4e8a-b62a-8279b3e9b47e"},"warnings":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"ok":true,"errors":[],"meta":{"operation":"batch_plan","duration_ms":939,"addin_version":"0.10.1"},"data":{"has_batch_connections":false,"templates":{"6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D"},"last_report":null,"created_utc":"2026-10-11T23:38:16.5021608Z","label_view_id":null,"nodes":[{"color_rgb":null,"has_spec_override":false,"max_deviation_deg":null,"members":[{"element_id":1249630,"end_gap_mm":20.800000000000001,"reaches_node":true,"angle_deg":-43.100000000000001,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y"}],"status_detail":"Ninguna plantilla casa todas sus ranuras con las barras del nudo.","created_connection_id":null,"is_mirrored":false,"is_manual":false,"chord_continuous":false,"signature":"1 -Y (-43)","template_name":null,"visible_by_default":false,"validation_token":null,"color_name":null,"element_ids":[1245533,1249630],"through_element_ids":[],"spec":null,"existing_batch_id":null,"template_id":null,"attempts":["Nudo tipico Detalle D \u2192 same: ranura 0 (diagonal 136.9\u00b0) sin barra; ranura 1 (diagonal 44.4\u00b0) sin barra; ranura 2 (diagonal -135.6\u00b0) sin barra; barras sobrantes: 1249630","Nudo tipico Detalle D \u2192 mirror_x: ranura 0 (diagonal 43.1\u00b0) sin barra; ranura 1 (diagonal 135.6\u00b0) sin barra; ranura 2 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0)","Nudo tipico Detalle D \u2192 mirror_y: ranura 0 (diagonal -136.9\u00b0) sin barra; ranura 1 (diagonal -44.4\u00b0) \u2192 barr ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 7434e6ee-3bdc-485f-aa5b-a17ed02f0956 etiquetas quitadas=0
Cuerpo:
{"ok": true, "errors": [], "meta": {"operation": "batch_plan_discard", "duration_ms": 10, "addin_version": "0.10.1"}, "data": {"removed_marks": 0, "remaining_plans": 0, "remaining_markers": 0, "remaining_labels": 0, "discarded_plan_id": "7434e6ee-3bdc-485f-aa5b-a17ed02f0956", "removed_labels": 0}, "warnings": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.10.1
Cuerpo:
{
  "ok": true,
  "errors": [],
  "meta": {
    "operation": "ping",
    "duration_ms": 2,
    "addin_version": "0.10.1"
  },
  "data": {
    "addin_version": "0.10.1",
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
      "types",
      "update",
      "validate"
    ],
    "revit": {
      "version_build": "27.2.0.39",
      "version_number": "2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_name": "Autodesk Revit 2027"
    },
    "document": {
      "is_modifiable": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_family": false,
      "is_read_only": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "is_workshared": false
    },
    "dotnet": {
      "framework": ".NET 10.0.12",
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "load_context": "Default"
    }
  },
  "warnings": []
}
======================================================================
Resultado: 32/32 pruebas correctas

```

## 10b-6 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 417 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "meta":  {
                 "operation":  "catalog_list",
                 "duration_ms":  2,
                 "addin_version":  "0.10.1"
             },
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "origin_drawing":  "Detalle D",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ]
                                   }
                               ],
                 "templates_count":  1,
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog"
             },
    "warnings":  [

                 ]
}

```

## 10b-6 sondeo 12 restos

```text
== 12-fase3-borrar.py -> HTTP 200 en 98 ms ==
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

## 10b-6 sondeo 13 restos

```text
== 13-limpiar-fase1.py -> HTTP 200 en 33 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 10b-6 sondeo 21 final

```text
== 21-etiquetas-addin.py -> HTTP 200 en 54 ms ==
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

## 10b-6 log del dia

```text

{"ts":"2026-10-11T18:27:00.4544575-05:00","record":{"event":"startup","addin_version":"0.10.1","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-11T18:29:22.4019728-05:00","record":{"event":"selection_expanded","requested":1,"candidates":964,"added"
:63,"chords":8,"members":46,"splices":9,"skipped_out_of_plane":0,"ignored":0,"rounds":11,"limit_reached":false,"duratio
n_ms":607,"summary":"Se añadieron 63 barras que tocan la selección: 8 cordones que pasan de largo, 46 barras que 
llegan y 9 tramos de cordón."}}
{"ts":"2026-10-11T18:30:00.3611859-05:00","record":{"event":"ribbon_batch_assist","selection":1,"added":63,"chords":8,"
members":46,"splices":9,"skipped_out_of_plane":0,"choice":"all"}}
{"ts":"2026-10-11T18:30:01.4298820-05:00","record":{"event":"labels_applied","plan_id":"1a92a865-adff-4521-8fb5-e48cb8d
a89ff","view":1245519,"count":33,"folder":"C:\\Users\\Public\\MotorConexiones\\etiquetas","total_in_document":33}}
{"ts":"2026-10-11T18:30:01.7416512-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","modeless":true,"selection":1}}
{"ts":"2026-10-11T18:31:02.4756213-05:00","record":{"event":"selection_expanded","requested":1,"candidates":964,"added"
:63,"chords":7,"members":47,"splices":9,"skipped_out_of_plane":0,"ignored":0,"rounds":10,"limit_reached":false,"duratio
n_ms":532,"summary":"Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 47 barras que 
llegan y 9 tramos de cordón."}}
{"ts":"2026-10-11T18:31:12.8537933-05:00","record":{"event":"ribbon_batch_assist","selection":1,"added":63,"chords":7,"
members":47,"splices":9,"skipped_out_of_plane":0,"choice":"cancel"}}
{"ts":"2026-10-11T18:31:27.1933450-05:00","record":{"event":"selection_expanded","requested":1,"candidates":964,"added"
:63,"chords":7,"members":47,"splices":9,"skipped_out_of_plane":0,"ignored":0,"rounds":10,"limit_reached":false,"duratio
n_ms":501,"summary":"Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 47 barras que 
llegan y 9 tramos de cordón."}}
{"ts":"2026-10-11T18:31:28.2836278-05:00","record":{"event":"ribbon_batch_assist","selection":1,"added":63,"chords":7,"
members":47,"splices":9,"skipped_out_of_plane":0,"choice":"selection"}}
{"ts":"2026-10-11T18:32:01.2925074-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":true,"window_open":true}}
{"ts":"2026-10-11T18:32:01.3003602-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:01.7935967-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:01.7942481-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:03.3557071-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:03.3562578-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:04.1314670-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:04.1320563-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:04.5856445-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:04.5862471-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:06.3816597-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:06.3822846-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:23.6540501-05:00","record":{"event":"label_clicked","index":0,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N4","status":"ready","highlighted":false,"window_open":true}}
{"ts":"2026-10-11T18:32:23.6549710-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N4","status":"ready"}}
{"ts":"2026-10-11T18:32:45.0153515-05:00","record":{"event":"label_clicked","index":1,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N5","status":"no_match","highlighted":true,"window_open":true
}}
{"ts":"2026-10-11T18:32:45.0184224-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N5","status":"no_match"}}
{"ts":"2026-10-11T18:32:46.0665188-05:00","record":{"event":"label_clicked","index":2,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N6","status":"ready","highlighted":true,"window_open":true}}
{"ts":"2026-10-11T18:32:46.0684138-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N6","status":"ready"}}
{"ts":"2026-10-11T18:32:46.4362528-05:00","record":{"event":"label_clicked","index":3,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N9","status":"no_match","highlighted":true,"window_open":true
}}
{"ts":"2026-10-11T18:32:46.4377413-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N9","status":"no_match"}}
{"ts":"2026-10-11T18:32:47.2273830-05:00","record":{"event":"label_clicked","index":4,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N11","status":"ready","highlighted":true,"window_open":true}}
{"ts":"2026-10-11T18:32:47.2289678-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N11","status":"ready"}}
{"ts":"2026-10-11T18:33:14.1289967-05:00","record":{"event":"label_clicked","index":1,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N5","status":"no_match","highlighted":true,"window_open":true
}}
{"ts":"2026-10-11T18:33:14.1370270-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N5","status":"no_match"}}
{"ts":"2026-10-11T18:33:14.6054685-05:00","record":{"event":"label_clicked","index":1,"document":"HANGAR_PRUEBA_sondeo"
,"plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N5","status":"no_match","highlighted":false,"window_open":tru
e}}
{"ts":"2026-10-11T18:33:14.6061571-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N5","status":"no_match"}}
{"ts":"2026-10-11T18:33:17.4949626-05:00","record":{"event":"ribbon_batch_window_activated","plan_id":"1a92a865-adff-45
21-8fb5-e48cb8da89ff"}}
{"ts":"2026-10-11T18:33:19.5166511-05:00","record":{"event":"ribbon_batch_window_activated","plan_id":"1a92a865-adff-45
21-8fb5-e48cb8da89ff"}}
{"ts":"2026-10-11T18:33:50.4709122-05:00","record":{"event":"ribbon_batch_window","plan_id":"1a92a865-adff-4521-8fb5-e4
8cb8da89ff","action":"closed","discarded":false}}
{"ts":"2026-10-11T18:34:03.9490173-05:00","record":{"event":"label_clicked","index":11,"document":"HANGAR_PRUEBA_sondeo
","plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N23","status":"no_match","highlighted":true,"window_open":fa
lse}}
{"ts":"2026-10-11T18:34:06.5417751-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","modeless":true,"selection":0}}
{"ts":"2026-10-11T18:34:11.3150560-05:00","record":{"event":"ribbon_batch_window","plan_id":"1a92a865-adff-4521-8fb5-e4
8cb8da89ff","action":"closed","discarded":false}}
{"ts":"2026-10-11T18:34:43.0314777-05:00","record":{"event":"ribbon_batch_window_opened","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","modeless":true,"selection":0}}
{"ts":"2026-10-11T18:34:49.5685849-05:00","record":{"event":"label_clicked","index":19,"document":"HANGAR_PRUEBA_sondeo
","plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N37","status":"no_match","highlighted":true,"window_open":tr
ue}}
{"ts":"2026-10-11T18:34:49.5825138-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N37","status":"no_match"}}
{"ts":"2026-10-11T18:34:49.9272538-05:00","record":{"event":"label_clicked","index":19,"document":"HANGAR_PRUEBA_sondeo
","plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N37","status":"no_match","highlighted":false,"window_open":t
rue}}
{"ts":"2026-10-11T18:34:49.9277893-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N37","status":"no_match"}}
{"ts":"2026-10-11T18:34:56.5142887-05:00","record":{"event":"label_clicked","index":21,"document":"HANGAR_PRUEBA_sondeo
","plan_id":"1a92a865-adff-4521-8fb5-e48cb8da89ff","node":"N40","status":"no_match","highlighted":true,"window_open":tr
ue}}
{"ts":"2026-10-11T18:34:56.5163385-05:00","record":{"event":"ribbon_batch_label_clicked","plan_id":"1a92a865-adff-4521-
8fb5-e48cb8da89ff","node":"N40","status":"no_match"}}
{"ts":"2026-10-11T18:35:07.3586827-05:00","record":{"event":"labels_cleared","before":33,"after":0}}
{"ts":"2026-10-11T18:35:07.5139640-05:00","record":{"event":"ribbon_batch_discard","plan_id":"1a92a865-adff-4521-8fb5-e
48cb8da89ff","result":"113 marca(s) quitadas, 33 etiqueta(s) quitadas; quedan 0 marcadores y 0 etiquetas en el 
documento.","warnings":0,"closes_window":true}}
{"ts":"2026-10-11T18:35:07.5268447-05:00","record":{"event":"ribbon_batch_window","plan_id":"1a92a865-adff-4521-8fb5-e4
8cb8da89ff","action":"closed","discarded":true}}
{"ts":"2026-10-11T18:38:16.4946408-05:00","record":{"event":"selection_expanded","requested":1,"candidates":964,"added"
:63,"chords":7,"members":42,"splices":14,"skipped_out_of_plane":0,"ignored":0,"rounds":11,"limit_reached":false,"durati
on_ms":619,"summary":"Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que 
llegan y 14 tramos de cordón."}}
{"ts":"2026-10-11T18:38:16.8154628-05:00","record":{"event":"handle","operation":"batch_plan","request_summary":"{\"ele
ment_ids\": [1249510], \"expand_selection\": true, \"mark\": 
false}","ok":true,"error_codes":[],"warning_codes":["SELECTION_EXPANDED"],"duration_ms":939}}



```

## 10b-7 git status antes del commit

```text
 M docs/fases/capturas/fase8-01-sondeo17.png
?? docs/fases/capturas/fase10b-01-seleccion-asistida.png
?? docs/fases/capturas/fase10b-02-ventana.png
?? docs/fases/capturas/fase10b-03-etiquetas.png
?? docs/fases/capturas/fase10b-04-etiqueta-pinchada.png
?? docs/fases/capturas/fase10b-05-solo-una-barra.png
?? docs/fases/resultados-fase-10b.md

```
