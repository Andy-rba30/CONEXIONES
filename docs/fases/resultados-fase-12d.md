# Resultados de la ronda 12d

Fecha: 2026-10-23T12:53:21


## 12d-1 git

```text
42ff200 Cierre de la ronda 12c y ronda 12d (add-in 0.12.3): la 0.12.2 contrastada en Revit (perfil 203,2 × 304,8, WeldLine materializadas en los dos tipos, el 430 en horizontal, puente 38/38), las advertencias de Revit al log (revit_failure, con los elementos en el aviso) y el sondeo 29 (los objetos del modelo de Advance Steel por GetModelObjectIds, la viga por Guid o geometría y BeamShortening: el último intento del corte inclinado)
?? docs/fases/resultados-fase-12d.md

```

## 12d-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.66 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.66 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.66 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:12.71
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   291, Omitido:     0, Total:   291, Duración: 885 ms - MotorConexiones.Tests.dll (net10.0)

```

## 12d-1 revit cerrado

```text

```

## 12d-1 deploy

```text
== MotorConexiones 0.12.3.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 12d-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 12d-1 version de la dll

```text
0.12.3.0

```

## 12d-1 sondeo 29 desplegado

```text

Name                             Length
----                             ------
29-viga-objetos-advance-steel.py  36213



```

## 12d-2 ping

```text
== conn/ping -> HTTP 200 en 321 ms ==
{
    "data":  {
                 "has_uidocument":  true,
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
                 "document":  {
                                  "is_read_only":  false,
                                  "is_family":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_workshared":  false,
                                  "is_modifiable":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo"
                              },
                 "spec_version":  "1.0",
                 "revit":  {
                               "version_build":  "27.2.0.39",
                               "sub_version_number":  "2027.2",
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "language":  "English_USA"
                           },
                 "addin_version":  "0.12.3",
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            }
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "duration_ms":  13,
                 "addin_version":  "0.12.3",
                 "operation":  "ping"
             }
}

```

## 12d-2 list

```text
== conn/list -> HTTP 200 en 59 ms ==
{
    "data":  {
                 "connections_count":  0,
                 "batch_id":  null,
                 "batches":  {

                             },
                 "total_count":  0,
                 "connections":  [

                                 ]
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "duration_ms":  12,
                 "addin_version":  "0.12.3",
                 "operation":  "list"
             }
}

```

## 12d-3 sondeo 29 objetos de Advance Steel

```text
== 29-viga-objetos-advance-steel.py -> HTTP 200 en 3172 ms ==
=== 29-viga-objetos-advance-steel (ronda 12d) ===
   modo: CANCELAR al final (nada queda)
0) Cordon [1245533] (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0); diagonal [1249631] (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9); extremo que llega: fin (-9354.7, -17195.8, 19884.9); angulo con el cordon 44.4; extension actual 69.1996468112 mm
1) Transaccion Autodesk.SteelConnectionsDB.FabricationTransaction; Beam: Autodesk.AdvanceSteel.Modelling.Beam; BeamShortening(eEnd, Double) y WeldLine(Point3d[], Vector3d, Vector3d): si; FilerObject en ASObjectsMgd; eEnd: kMid, kStart, kEnd, kNoEnd
2) Cordon: en el plano 203.2 (medio canto 101.6, giro 90); diagonal ancho 63.5, huella en la cara 90.8 mm, centro (-9409.4, -17195.8, 19831.4)
3) FabricationTransaction abierta. doc.IsModifiable=True
   puente: AddFabricationInformationForRevitElements -> 0
   puente: GetFabricationUniqueID -> dc5e7d5d-2ad6-4211-8300-1747c3e60377
   tipos: DatabaseManager=Autodesk.AdvanceSteel.CADAccess.DatabaseManager; DocumentManager=Autodesk.AdvanceSteel.DocumentManagement.DocumentManager; CADDocumentManager=Autodesk.AdvanceSteel.DocumentManagement.CADDocumentManager; ClassTypeFilter=Autodesk.AdvanceSteel.CADAccess.IClassTypeFilter
   DocumentManager.GetCurrentDatabase() -> Autodesk.AdvanceSteel.CADLink.Database.Database; metodos con ObjectId: GetObjectId(String) -> ObjectId
   CADDocumentManager.GetCurrentDatabase() -> Autodesk.AdvanceSteel.DocumentManagement.OpenDatabase; metodos con ObjectId: getFilerObjectIdFromHandle(String) -> ObjectId; getObjectIdFromHandle(String) -> ObjectId
   CADDocumentManager.GetCurrentDwg() -> Autodesk.AdvanceSteel.CADLink.Database.Database; metodos con ObjectId: GetObjectId(String) -> ObjectId
   abrir con: DatabaseManager.Open(ObjectId, eOpenMode.kNotOpened) (modos: kNotOpened, kNormal, kToShadow)
   abrir con (reserva): FilerObject.GetFilerObject(ObjectId)
   GetModelObjectIds, variantes: GetModelObjectIds(ObjectId[]&) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter, Database) -> Void
   GetModelObjectIds(ObjectId[]&) -> Void -> 5 ObjectId
   GetModelObjectIds(ObjectId[]&, ClassTypeFilter) -> Void: no se llama (falta el filtro o la base de datos)
   GetModelObjectIds(ObjectId[]&, ClassTypeFilter, Database) -> Void: no se llama (falta el filtro o la base de datos)
   GetModelObjectIds(ObjectId[]&) -> Void: tipos recorridos: (no abierto)=5; vigas vistas: 0
   extremos de la barra de Revit en mm: (-11856.5, -17195.8, 17437.3) y (-9354.7, -17195.8, 19884.9) (si las vigas de arriba estan en otro origen o en otra unidad, es lo que hay que casar)
   viga de Advance Steel: NO encontrada (1 lista(s) recorrida(s), 0 viga(s) vista(s) en la ultima)
   WeldLine creada: Thickness=5.0; MainEffectiveThroat=5.0; DoubleEffectiveThroat=5.0; IsClosed=True
   WeldLine.Connect: sin Connect (no hay pieza de Advance Steel)
   WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589
6) Elementos nuevos con la transaccion abierta: 0; extension del extremo ahora: 69.1996468112 mm
7) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False | extension del extremo: 69.1996468112 mm
8) Elementos nuevos tras cerrar: 0 (se esperan 0)
9) RESUMEN: WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589 | WeldLine.Connect: sin Connect (no hay pieza de Advance Steel) | viga AS: NO encontrada (1 lista(s) recorrida(s), 0 viga(s) vista(s) en la ultima)
=== fin 29-viga-objetos-advance-steel ===


```

## 12d-3 sondeo 12 tras el sondeo

```text
== 12-fase3-borrar.py -> HTTP 200 en 119 ms ==
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

## 12d-3 sondeo 13 tras el sondeo

```text
== 13-limpiar-fase1.py -> HTTP 200 en 38 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12d-5 list tras el nudo K

```text
== conn/list -> HTTP 200 en 436 ms ==
{
    "data":  {
                 "connections_count":  1,
                 "batch_id":  null,
                 "batches":  {

                             },
                 "total_count":  1,
                 "connections":  [
                                     {
                                         "created_elements_count":  2,
                                         "backend":  "advancesteel",
                                         "connection_id":  "f9f4c051-414f-43bf-8966-5656101263d4",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                         "batch_id":  null,
                                         "spec_version":  "1.0",
                                         "connection_type":  "direct_weld",
                                         "created_utc":  "2026-10-24T14:51:23.3554853Z"
                                     }
                                 ]
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "duration_ms":  7,
                 "addin_version":  "0.12.3",
                 "operation":  "list"
             }
}

```

## 12d-5 delete nudo K

```text
== conn/delete -> HTTP 200 en 552 ms ==
{
    "data":  {
                 "deleted_connection_id":  "f9f4c051-414f-43bf-8966-5656101263d4",
                 "deleted_elements_count":  2,
                 "restored_members_count":  2
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "duration_ms":  145,
                 "addin_version":  "0.12.3",
                 "operation":  "delete"
             }
}

```

## 12d-5 sondeo 12 tras borrar

```text
== 12-fase3-borrar.py -> HTTP 200 en 217 ms ==
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

## 12d-5 sondeo 13 tras borrar

```text
== 13-limpiar-fase1.py -> HTTP 200 en 38 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12d-6 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000          2872



```

## 12d-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.12.3 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"data":{"has_uidocument":true,"backend":"advancesteel","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"document":{"is_read_only":false,"is_family":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_workshared":false,"is_modifiable":false,"title":"HANGAR_PRUEBA_sondeo"},"spec_version":"1.0","revit":{"version_build":"27.2.0.39","sub_version_number":"2027.2","version_name":"Autodesk Revit 2027","version_number":"2027","language":"English_USA"},"addin_version":"0.12.3","dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12","load_context":"Default"}},"ok":true,"warnings":[],"errors":[],"meta":{"duration_ms":3,"addin_version":"0.12.3","operation":"ping"}}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 32693 caracteres
Cuerpo:
{"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"data":{"connection_types":[{"type_name":"direct_weld","description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n."},{"type_name":"end_plate","description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones)."},{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"ok":true,"warnings":[],"errors":[],"meta":{"duration_ms":11,"addin_version":"0.12.3","operation":"types"}}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"data":{"connection_type":"gusset_node","json_schema":{"type":"object","$schema":"http://json-schema.org/draft-07/schema#","additionalProperties":false,"title":"GussetNodeConnectionSpec","required":["spec_version","connection_type","node","chord","gusset","members"],"properties":{"uncertain_fields":{"type":"array","items":{"required":["path","reason"],"type":"object","properties":{"reason":{"type":"string"},"user_confirmed_value":{},"path":{"type":"string"}},"additionalProperties":false}},"dimension_chains":{"type":"array","items":{"required":["values_mm","expected_total_mm"],"type":"object","properties":{"label":{"type":"string"},"expected_total_mm":{"type":"number","minimum":0.0},"values_mm":{"type":"array","items":{"type":"number"},"minItems":1}},"additionalProperties":false}},"chord":{"required":["element_id","continuous"],"type":"object","properties":{"profile":{"type":["string","null"]},"continuous":{"type":"boolean"},"element_id":{"type":"integer"}},"additionalProperties":false},"spec_version":{"type":"string","enum":["1.0"]},"connection_type":{"type":"string","enum":["gusset_node"]},"source":{"type":"object","properties":{"template_id":{"type":["string","null"],"description":"Plantilla del cat\u00e1logo de la que sali\u00f3 esta especificaci\u00f3n (conn_catalog_apply); la escribe el add-in."},"batch_id":{"type":["string","null"],"description":"Lote al que pertenece la conexi\u00f3n (reservado para conn_batch_create, Fase 9)."},"scale":{"type":"string"},"drawing":{"t ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"UNKNOWN_OPERATION","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","path":"type","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node"}],"meta":{"duration_ms":0,"addin_version":"0.12.3","operation":"schema"}}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"data": {"query": "HSS2-1/2X2-1/2X3/16", "matches": [{"exact_match": false, "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "matched_count": 1, "suggestions": [], "total_profiles_in_model": 29}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 8, "addin_version": "0.12.3", "operation": "find_profile"}}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"data": {"origin_mm": [-11869.400000000000, -17195.799999999999, 17423], "y_axis": [0, 0, 1], "z_axis": [1.9999999999999999e-06, -1, 0], "chord_direction_reversed": true, "existing_connections": [], "chord_element_id": 1249510, "eccentricity_mm": 3.3999999999999999, "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "x_axis": [1, 1.9999999999999999e-06, 0], "kind": "node", "members": [{"angle_to_chord_deg": 0, "side": "chord", "width_mm": 76.200000000000003, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "height_mm": 76.200000000000003, "is_chord": true, "node_end": 1, "section_rotation_deg": 180, "angle_in_plane_deg": 0, "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "end_gap_mm": 0, "element_id": 1249510, "wall_mm": 5.9199999999999999, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "slope_deg": 0, "length_mm": 9960.2999999999993, "family": "HSS-Hollow Structural Section", "structural_type": "Beam", "axis_offset_mm": 0, "type": "HSS3X3X1/4", "reaches_node": true, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423]}, {"angle_to_chord_deg": 43.100000000000001, "side": "+Y", "width_mm": 63.5, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "height_mm": 63.5, "is_chord": false, "node_end": 1, "section_rotation_deg": -180, "angle_in_plane_deg": 136.90000000000001, "axis_cut_mm": [-11867.700000000001, -17195.79999999999 ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"data":{"errors_count":0,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","calculated_values":{"frame_y":[0,0,1],"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"chord_direction_reversed":true,"frame_x":[1,0,0],"eccentricity_mm":3.3999999999999999,"frame_z":[0,-1,0],"kind":"node","axis_distance_mm":0.080000000000000002},"bolt_stacks":[{"grip_mm":19.524999999999999,"gusset_face":"+z","length_source":"computed_from_grip","member_element_id":1249636,"bolt_length_mm":44.450000000000003}],"warnings_count":1,"is_valid":true},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"errors":[],"meta":{"duration_ms":46,"addin_version":"0.12.3","operation":"validate"}}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","path":"dimension_chains[0].values_mm","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm."}],"meta":{"duration_ms":14,"addin_version":"0.12.3","operation":"validate"}}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"ok":false,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"errors":[{"code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","path":"uncertain_fields[0].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."},{"code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","path":"uncertain_fields[1].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."}],"meta":{"duration_ms":9,"addin_version":"0.12.3","operation":"validate"}}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"dry_run": true, "backend": "advancesteel", "gusset_plates": 1, "weld_lines": 6, "members_modified": 3, "bolts": 4, "chord_element_id": 1249510, "connection_type": "gusset_node", "working_point_mm": [-11869.4, -17195.8, 17423], "first_member_element_id": 1249630, "knife_plates": 1}
Cuerpo:
{"data": {"elements_to_create": [{"chord_interface": "through_slot", "thickness_label": "3/8\"", "thickness_mm": 9.5250000000000004, "vertices_count": 8, "kind": "gusset_plate", "height_mm": 530, "width_mm": 565}, {"kind": "welded_slot_interface", "slot_length_mm": 150, "weld_size_mm": 5, "for_member_id": 1249630}, {"kind": "welded_slot_interface", "slot_length_mm": 150, "weld_size_mm": 5, "for_member_id": 1249631}, {"gusset_face": "+z", "length_mm": 170, "offset_from_gusset_plane_mm": 9.7620000000000005, "thickness_mm": 10, "for_member_id": 1249636, "insertion_mm": 80, "kind": "knife_plate", "width_mm": 140}, {"count": 4, "rows": 2, "grip_mm": 19.524999999999999, "length_source": "computed_from_grip", "length_mm": 44.450000000000003, "diameter_mm": 15.875, "for_member_id": 1249636, "spacing_mm": 60, "columns": 2, "kind": "bolt_group", "edge_mm": 40}], "members_to_modify": [{"role": "diagonal", "current_end_distance_mm": 84.900000000000006, "element_id": 1249630, "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "end": "end", "new_extension_mm": -95.099999999999994, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"}, {"role": "diagonal", "current_end_distance_mm": 19.199999999999999, "element_id": 1249631, "setback_mm": 60, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "end": "start", "new_extension_mm": -40.799999999999997, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"}, {" ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"VALIDATION_TOKEN_INVALID","message":"validation_token es obligatorio para crear una conexi\u00f3n.","path":"validation_token","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token."}],"meta":{"duration_ms":0,"addin_version":"0.12.3","operation":"create"}}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"data": {"connections_count": 0, "batch_id": null, "batches": {}, "total_count": 0, "connections": []}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 4, "addin_version": "0.12.3", "operation": "list"}}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo."}],"meta":{"duration_ms":4,"addin_version":"0.12.3","operation":"get"}}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","path":"connection_id","hint":"Verifica los IDs disponibles con conn_list."}],"meta":{"duration_ms":4,"addin_version":"0.12.3","operation":"delete"}}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"UNKNOWN_OPERATION","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","path":null,"hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate."}],"meta":{"duration_ms":0,"addin_version":"0.12.3","operation":"no_existe"}}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"data":{"templates":[{"created_utc":"2026-10-21T16:49:07.9104459Z","origin_document":"HANGAR_PRUEBA_sondeo","template_id":"91adac27-d196-4c98-88db-6579aa991be4","pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","description":null,"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","connection_type":"direct_weld","members_count":2,"name":"Nudo K cordon superior","tags":[],"chord_profile":"HSS12X8X1/2"},{"created_utc":"2026-10-04T22:01:50.5585233Z","origin_document":"HANGAR_PRUEBA_sondeo","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","origin_drawing":"Detalle D","connection_type":"gusset_node","members_count":3,"name":"Nudo tipico Detalle D","tags":["hangar","cercha","HSS"],"chord_profile":"HSS3X3X1/4"}],"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates_count":2,"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo"},"ok":true,"warnings":[],"errors":[],"meta":{"duration_ms":14,"addin_version": ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=eacee8af-562a-478d-86cc-4e5336990cbb barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\eacee8af-562a-478d-86cc-4e5336990cbb.json
Cuerpo:
{"data":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","origin":{"connection_id":null,"document":"HANGAR_PRUEBA_sondeo","drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636]},"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\eacee8af-562a-478d-86cc-4e5336990cbb.json","shared_file":null,"member_pattern":[{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","slot":0,"angle_deg":136.91999999999999},{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","slot":1,"angle_deg":44.369999999999997},{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","slot":2,"angle_deg":-135.63000000000000}],"members_count":3,"name":"PRUEBA probar_conexiones","chord_profile":"HSS3X3X1/4"},"ok":true,"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"errors":[],"meta":{"duration_ms":34,"addin_version":"0.12.3","operation":"catalog_save"}}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"data":{"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\eacee8af-562a-478d-86cc-4e5336990cbb.json","template":{"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","origin":{"connection_id":null,"document":"HANGAR_PRUEBA_sondeo","drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636]},"description":null,"catalog_version":"1.0","member_pattern":[{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","slot":0,"angle_deg":136.91999999999999},{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"+Y","slot":1,"angle_deg":44.369999999999997},{"role":"diagonal","profile_policy":"warn","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","side":"-Y","slot":2,"angle_deg":-135.63000000000000}],"connection_type":"gusset_node","spec_template":{"uncertain_fields":[],"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125.0,90.0,350.0]},{"label":"lado derecho","expected_total_mm":530.0,"values_mm":[70.0,250.0,210.0]},{"label":"lado izquierdo" ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=93f71e1020d9...
Cuerpo:
{"data":{"errors_count":0,"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","spec":{"uncertain_fields":[],"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125.0,90.0,350.0]},{"label":"lado derecho","expected_total_mm":530.0,"values_mm":[70.0,250.0,210.0]},{"label":"lado izquierdo","expected_total_mm":530.0,"values_mm":[70.0,325.0,135.0]}],"chord":{"profile":"HSS3X3X1/4","continuous":true,"element_id":1249510},"spec_version":"1.0","connection_type":"gusset_node","source":{"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","drawing":"Detalle D","scale":"1/10"},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"role":"diagonal","element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"expected_angle_deg":43.100000000000001},{"role":"diagonal","element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"expected_angle_deg":44.399999999999999},{"role":"diagonal","element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":260.0,"attachment":{"type":"bolted_knife_plate","plate":{"thickness_label":"PL10","length_mm":170.0,"thickness_mm":10.0,"insertion_mm":80.0,"width_mm":140.0},"we ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"data":{"errors_count":0,"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","spec":{"uncertain_fields":[],"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125.0,90.0,350.0]},{"label":"lado derecho","expected_total_mm":530.0,"values_mm":[70.0,250.0,210.0]},{"label":"lado izquierdo","expected_total_mm":530.0,"values_mm":[70.0,325.0,135.0]}],"chord":{"profile":"HSS3X3X1/4","continuous":true,"element_id":1249510},"spec_version":"1.0","connection_type":"gusset_node","source":{"template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","drawing":"Detalle D","scale":"1/10"},"node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"role":"diagonal","element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"expected_angle_deg":43.100000000000001},{"role":"diagonal","element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"type":"fillet","size_mm":5.0,"all_around":true}},"expected_angle_deg":44.399999999999999},{"role":"diagonal","element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":260.0,"attachment":{"type":"bolted_knife_plate","plate":{"thickness_label":"PL10","length_mm":170.0,"thickness_mm":10.0,"insertion_mm":80.0,"width_mm":140.0},"we ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=50d50d28-a329-44c2-8181-32a06dd0d458 nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"data":{"templates":{"eacee8af-562a-478d-86cc-4e5336990cbb":"PRUEBA probar_conexiones"},"document":"HANGAR_PRUEBA_sondeo","has_batch_connections":false,"overrides":{"replace_existing":false,"exclude":[],"spec":{},"add_node":{},"merge":[],"chord":{},"remove_member":{},"template":{},"add_member":{},"split":{}},"nodes":[{"existing_connection_id":null,"connection_type":null,"template_name":null,"validation_token":null,"chord_type_name":null,"existing_batch_id":null,"starter":false,"status_text":"\u25cb Barra suelta (no es nudo)","actions":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","visible_by_default":false,"status":"untyped","members":[],"orientation":null,"spec":null,"element_ids":[1249630],"match":null,"ghost_element_id":null,"warnings":[],"has_spec_override":false,"signature":"1 barra(s) sin marco","is_mirrored":false,"color_name":null,"advice":"No es un nudo: nada que hacer","through_element_ids":[],"splice_element_id":null,"chord_continuous":false,"created_connection_id":null,"errors_count":0,"color_rgb":null,"is_marked":false,"marker_element_id":null,"is_valid":false,"kind":"node","name":"N1","is_manual":false,"eccentricity_mm":0,"max_deviation_deg":null,"label_index":null,"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"warnings_count":0,"replaces_existing":false,"member_element_ids":[1249630],"errors":[],"chord_element_id":0,"template_id":null,"attempts":[]},{"existing_connection_id":null,"connec ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=e3a204a20e70...
Cuerpo:
{"data":{"node":{"existing_connection_id":null,"connection_type":"gusset_node","template_name":"PRUEBA probar_conexiones","validation_token":"e3a204a20e70514b3f4241daf8fce81cfbdfcbe063a15780342293aad7757739","chord_type_name":"HSS3X3X1/4","existing_batch_id":null,"starter":false,"status_text":"\u25cf Listo","actions":[],"status_detail":null,"visible_by_default":true,"status":"ready","members":[{"reaches_node":true,"axis_offset_mm":1.7000000000000000,"element_id":1249630,"side":"+Y","chord_element_id":null,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","end_gap_mm":84.900000000000006,"angle_deg":136.90000000000001},{"reaches_node":true,"axis_offset_mm":-1.7000000000000000,"element_id":1249631,"side":"+Y","chord_element_id":null,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","end_gap_mm":19.300000000000001,"angle_deg":44.399999999999999},{"reaches_node":true,"axis_offset_mm":-1.7000000000000000,"element_id":1249636,"side":"-Y","chord_element_id":null,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","end_gap_mm":48.600000000000001,"angle_deg":-135.59999999999999}],"orientation":"same","spec":{"uncertain_fields":[],"dimension_chains":[{"label":"borde superior","expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0]},{"label":"base","expected_total_mm":565.0,"values_mm":[125.0,90.0,350.0]},{"label":"lado derecho","expected_total_mm":530.0,"values_mm":[70.0,250.0,210.0]},{"label":"lado izquierdo","expected_total_mm":530.0,"values_mm":[70.0,325.0,135.0]}],"chord":{"profile":"HSS3X3X1/4","continuous":true, ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote 50d50d28: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"data":{"document":"HANGAR_PRUEBA_sondeo","connection_ids":[],"restored_members_count":0,"nodes":[{"outcome":"failed","connection_id":null,"orientation":"same","restored_members_count":0,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","template_name":"PRUEBA probar_conexiones","spec":null,"elements_count":0,"reason":"token distinto del plan","warnings":[],"node":"N4","duration_ms":0,"errors":[{"code":"VALIDATION_TOKEN_INVALID","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","path":"nodes[N4].validation_token","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4."}]}],"deleted_count":0,"updated_count":0,"stopped_at":null,"deleted_elements_count":0,"stop_on_error":false,"duration_ms":2,"stopped":false,"summary_text":"Lote 50d50d28: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","plan_creatable_count":1,"undo_entries":"one","batch_id":"50d50d28-a329-44c2-8181-32a06dd0d458","started_utc":"2026-10-24T14:55:29.7483944Z","plan_summary":{"untyped":5,"failed":1},"plan_id":"50d50d28-a329-44c2-8181-32a06dd0d458","operation":"batch_create","created_count":0,"skipped_count":0,"failed_count":1,"with_warnings_count":0,"plan_sum ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"data":{"document":"HANGAR_PRUEBA_sondeo","connection_ids":[],"restored_members_count":0,"nodes":[],"deleted_count":0,"updated_count":0,"stopped_at":null,"deleted_elements_count":0,"stop_on_error":false,"duration_ms":4,"stopped":false,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","plan_creatable_count":null,"undo_entries":"one","batch_id":"00000000-0000-0000-0000-000000000000","started_utc":"2026-10-24T14:55:29.8077300Z","plan_summary":null,"plan_id":null,"operation":"batch_delete","created_count":0,"skipped_count":0,"failed_count":0,"with_warnings_count":0,"plan_summary_text":null,"finished_utc":"2026-10-24T14:55:29.8121601Z","rolled_back_count":0},"ok":true,"warnings":[{"code":"BATCH_EMPTY","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","path":"batch_id","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote)."}],"errors":[],"meta":{"duration_ms":5,"addin_version":"0.12.3","operation":"batch_delete"}}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado 50d50d28-a329-44c2-8181-32a06dd0d458
Cuerpo:
{"data": {"remaining_labels": 0, "remaining_markers": 0, "removed_marks": 0, "removed_labels": 0, "discarded_plan_id": "50d50d28-a329-44c2-8181-32a06dd0d458", "remaining_plans": 0}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 35, "addin_version": "0.12.3", "operation": "batch_plan_discard"}}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"data":null,"ok":false,"warnings":[],"errors":[{"code":"TEMPLATE_NOT_FOUND","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","path":"template_id","hint":"Usa conn_catalog_list para ver las plantillas disponibles."}],"meta":{"duration_ms":0,"addin_version":"0.12.3","operation":"catalog_apply"}}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada eacee8af-562a-478d-86cc-4e5336990cbb
Cuerpo:
{"data":{"deleted_template_id":"eacee8af-562a-478d-86cc-4e5336990cbb","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\eacee8af-562a-478d-86cc-4e5336990cbb.json"},"ok":true,"warnings":[],"errors":[],"meta":{"duration_ms":2,"addin_version":"0.12.3","operation":"catalog_delete"}}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"data":{"templates":{"91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior","6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D"},"document":"HANGAR_PRUEBA_sondeo","has_batch_connections":false,"overrides":{"replace_existing":false,"exclude":[],"spec":{},"add_node":{},"merge":[],"chord":{},"remove_member":{},"template":{},"add_member":{},"split":{}},"nodes":[{"existing_connection_id":null,"connection_type":null,"template_name":null,"validation_token":null,"chord_type_name":"HSS12X8X1/2","existing_batch_id":null,"starter":false,"status_text":"\u2716 Falta el cord\u00f3n","actions":[{"key":"chord","label":"Cord\u00f3n\u2026"},{"key":"members","label":"Barras\u2026"}],"status_detail":"Ninguna plantilla casa todas sus ranuras con las barras del nudo.","visible_by_default":false,"status":"no_match","members":[{"reaches_node":true,"axis_offset_mm":0,"element_id":1249630,"side":"-Y","chord_element_id":null,"type_name":"HSS2-1-2X2-1-2X3-16 64x64","end_gap_mm":20.800000000000001,"angle_deg":-43.100000000000001}],"orientation":null,"spec":null,"element_ids":[1245533,1249630],"match":null,"ghost_element_id":null,"warnings":[{"code":"NODE_CHORD_NOT_CONTINUOUS","message":"Ninguna barra atraviesa el nudo: el cord\u00f3n es la m\u00e1s horizontal de las que llegan (1245533). Si es un extremo de cercha est\u00e1 bien; si falta el cord\u00f3n en la selecci\u00f3n, a\u00f1\u00e1delo y replanifica.","path":"nodes[N1].chord_element_id","hint":"overrides.chord fija el cord\u00 ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado 81b94c49-04a2-40ee-8a17-324396fa06ec etiquetas quitadas=0
Cuerpo:
{"data": {"remaining_labels": 0, "remaining_markers": 0, "removed_marks": 0, "removed_labels": 0, "discarded_plan_id": "81b94c49-04a2-40ee-8a17-324396fa06ec", "remaining_plans": 0}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 12, "addin_version": "0.12.3", "operation": "batch_plan_discard"}}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"data": {"errors_count": 0, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8", "calculated_values": {"frame_y": [0, 0, 1], "origin_mm": [-4180.5, 17204.200000000001, 19933], "chord_direction_reversed": true, "frame_x": [1, 0, 0], "eccentricity_mm": 0, "frame_z": [0, -1, 0], "kind": "splice", "axis_distance_mm": 0}, "bolt_stacks": [{"gap_mm": 0, "grip_mm": 25.399999999999999, "plates": 2, "length_source": "computed_from_grip", "member_element_id": 1245531, "bolts": 12, "bolt_length_mm": 50.799999999999997}], "warnings_count": 0, "is_valid": true}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 14, "addin_version": "0.12.3", "operation": "validate"}}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"data":{"errors_count":0,"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9","calculated_values":{"frame_y":[0,0,1],"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"chord_direction_reversed":true,"frame_x":[1,0,0],"eccentricity_mm":54.899999999999999,"frame_z":[0,-1,0],"kind":"node","axis_distance_mm":0.089999999999999997},"bolt_stacks":[],"warnings_count":2,"is_valid":true},"ok":true,"warnings":[{"code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","path":"members[0].axis_offset_mm","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE."},{"code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","path":"members[1].axis_offset_mm","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE."}],"errors":[],"meta":{"duration_ms":9,"addin_version":"0.12.3","operation":"validate"}}
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"data":{"templates":{"91adac27-d196-4c98-88db-6579aa991be4":"Nudo K cordon superior","6abcf116-9b97-485f-b50d-2851ca0018cc":"Nudo tipico Detalle D"},"document":"HANGAR_PRUEBA_sondeo","has_batch_connections":false,"overrides":{"replace_existing":false,"exclude":[],"spec":{},"add_node":{},"merge":[],"chord":{},"remove_member":{},"template":{},"add_member":{},"split":{}},"nodes":[{"existing_connection_id":null,"connection_type":null,"template_name":null,"validation_token":null,"chord_type_name":null,"existing_batch_id":null,"starter":false,"status_text":"\u25cb Barra suelta (no es nudo)","actions":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","visible_by_default":false,"status":"untyped","members":[],"orientation":null,"spec":null,"element_ids":[1245531],"match":null,"ghost_element_id":null,"warnings":[],"has_spec_override":false,"signature":"1 barra(s) sin marco","is_mirrored":false,"color_name":null,"advice":"No es un nudo: nada que hacer","through_element_ids":[],"splice_element_id":null,"chord_continuous":false,"created_connection_id":null,"errors_count":0,"color_rgb":null,"is_marked":false,"marker_element_id":null,"is_valid":false,"kind":"node","name":"N1","is_manual":false,"eccentricity_mm":0,"max_deviation_deg":null,"label_index":null,"work_point_mm":[-14656,17204.299999999999,19933],"warnings_count":0,"replaces_existing":false,"member_element_ids":[1245531],"errors":[],"chord_element_id":0,"template_id":null,"attempts":[]},{"ex ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado f81d43df-ee00-4b61-a48d-61e524acb264
Cuerpo:
{"data": {"remaining_labels": 0, "remaining_markers": 0, "removed_marks": 0, "removed_labels": 0, "discarded_plan_id": "f81d43df-ee00-4b61-a48d-61e524acb264", "remaining_plans": 0}, "ok": true, "warnings": [], "errors": [], "meta": {"duration_ms": 13, "addin_version": "0.12.3", "operation": "batch_plan_discard"}}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.12.3
Cuerpo:
{
  "data": {
    "has_uidocument": true,
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
    "document": {
      "is_read_only": false,
      "is_family": false,
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_workshared": false,
      "is_modifiable": false,
      "title": "HANGAR_PRUEBA_sondeo"
    },
    "spec_version": "1.0",
    "revit": {
      "version_build": "27.2.0.39",
      "sub_version_number": "2027.2",
      "version_name": "Autodesk Revit 2027",
      "version_number": "2027",
      "language": "English_USA"
    },
    "addin_version": "0.12.3",
    "dotnet": {
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12",
      "load_context": "Default"
    }
  },
  "ok": true,
  "warnings": [],
  "errors": [],
  "meta": {
    "duration_ms": 3,
    "addin_version": "0.12.3",
    "operation": "ping"
  }
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "data": {
    "errors_count": 0,
    "template_id": "1a8af134-f04d-4978-b137-44dfe0e160c1",
    "spec": {
      "uncertain_fields": [],
      "dimension_chains": [
        {
          "label": "borde superior",
          "expected_total_mm": 565.0,
          "values_mm": [
            75.0,
            420.0,
            70.0
          ]
        },
        {
          "label": "base",
          "expected_total_mm": 565.0,
          "values_mm": [
            125.0,
            90.0,
            350.0
          ]
        },
        {
          "label": "lado derecho",
          "expected_total_mm": 530.0,
          "values_mm": [
            70.0,
            250.0,
            210.0
          ]
        },
        {
          "label": "lado izquierdo",
          "expected_total_mm": 530.0,
          "values_mm": [
            70.0,
            325.0,
            135.0
          ]
        }
      ],
      "chord": {
        "profile": "HSS3X3X1/4",
        "continuous": true,
        "element_id": 1249510
      },
      "spec_version": "1.0",
      "connection_type": "gusset_node",
      "source": {
        "template_id": "1a8af134-f04d-4978-b137-44dfe0e160c1",
        "drawing": "Detalle D",
        "scale": "1/10"
      },
      "node": {
        "element_ids": [
          1249510,
          1249630,
          1249631,
          1249636
        ]
      },
      "members": [
        {
          "role": "diagonal",
          "element_id": 1249630,
          "profile": "HSS2 ...
======================================================================
Resultado: 38/38 pruebas correctas

```

## 12d-6 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 415 ms ==
{
    "data":  {
                 "templates":  [
                                   {
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "description":  null,
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "connection_type":  "direct_weld",
                                       "members_count":  2,
                                       "name":  "Nudo K cordon superior",
                                       "tags":  [

                                                ],
                                       "chord_profile":  "HSS12X8X1/2"
                                   },
                                   {
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "origin_drawing":  "Detalle D",
                                       "connection_type":  "gusset_node",
                                       "members_count":  3,
                                       "name":  "Nudo tipico Detalle D",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "chord_profile":  "HSS3X3X1/4"
                                   }
                               ],
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates_count":  2,
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo"
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ],
    "meta":  {
                 "duration_ms":  0,
                 "addin_version":  "0.12.3",
                 "operation":  "catalog_list"
             }
}

```

## 12d-6 log del dia

```text

{"ts":"2026-10-24T09:42:24.0842756-05:00","record":{"event":"startup","addin_version":"0.12.3","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-24T09:51:23.2630268-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear f9f4c051-414f-43bf-8966-5656101263d4","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-24T09:51:23.2764233-05:00","record":{"event":"direct_weld_member","element_id":1249631,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-24T09:51:23.2904731-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura directa 
barra 1249631","points":2,"closed":false,"size_mm":5,"properties":["Thickness=5","MainEffectiveThroat=5","DoubleEffecti
veThroat=5","IsClosed=False"],"connected_to":[],"connect":"sin Connect (ninguna plancha de Advance Steel bajo la 
soldadura)","units":"mm"}}
{"ts":"2026-10-24T09:51:23.2931707-05:00","record":{"event":"direct_weld_member","element_id":1249632,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-24T09:51:23.2947141-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura directa 
barra 1249632","points":2,"closed":false,"size_mm":5,"properties":["Thickness=5","MainEffectiveThroat=5","DoubleEffecti
veThroat=5","IsClosed=False"],"connected_to":[],"connect":"sin Connect (ninguna plancha de Advance Steel bajo la 
soldadura)","units":"mm"}}
{"ts":"2026-10-24T09:51:23.3527301-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear f9f4c051-414f-43bf-8966-5656101263d4","pending":2,"pending_welds":2,"missing":0,"new_elements":["1322501:Welds:St
eelProxyElement","1322502:Welds:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-24T09:51:23.5476055-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: 
Fine.","elements":"1322501","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d216b","transaction":"MotorConexiones: Crear 
Especificación de arranque (Conectar)"}}
{"ts":"2026-10-24T09:51:23.5490209-05:00","record":{"event":"revit_failure","severity":"Warning","text":"The created 
elements are only visible in Detail Level: 
Fine.","elements":"1322502","definition":"73c984d7-da5a-4cd2-b83b-1c2c345d216b","transaction":"MotorConexiones: Crear 
Especificación de arranque (Conectar)"}}
{"ts":"2026-10-24T09:51:23.5791110-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo K cordon superior-nudo-1245533.json","template_id":
"91adac27-d196-4c98-88db-6579aa991be4","connection_id":"f9f4c051-414f-43bf-8966-5656101263d4","created_elements":2,"mod
ified_members":2,"backend":"advancesteel","validation_token_prefix":"d2a7fa77bea6c69e","warnings":["END_CUT_APPROXIMATE
D","REVIT_WARNING","REVIT_WARNING"],"duration_ms":344}}
{"ts":"2026-10-24T09:52:46.4061108-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
borrar f9f4c051-414f-43bf-8966-5656101263d4","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-24T09:52:46.4457692-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
borrar f9f4c051-414f-43bf-8966-5656101263d4","pending":0,"pending_welds":0,"missing":0,"new_elements":[],"is_modifiable
_after":true}}
{"ts":"2026-10-24T09:55:29.7605933-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"validation_token\": \"0000000000000000000000000000000000000000000000000000000000000000\", \"node\": 
\"N4\"}], \"plan_id\": \"50d50d28-a329-44c2-8181-32a06dd0d458\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NO
DE_FAILED"],"duration_ms":13}}



```

## 12d-7 git status antes del commit

```text
?? docs/fases/resultados-fase-12d.md

```
