# Resultados de la Fase 5

Fecha: 2026-10-01T00:46:02


## A-1 git

```text
14da08d Fase 5: limpieza de probe_*, README corregido, sondeo 14 e instrucciones de punta a punta

```

## A-2 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.74
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    51, Omitido:     0, Total:    51, Duración: 149 ms - MotorConexiones.Tests.dll (net10.0)

```

## A-3 revit cerrado

```text

```

## A-3 deploy e instalar-conn

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, docs\guide.md
Siguiente paso: abre Revit 2027. Debe aparecer la pestana 'Conexiones'.
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (15 rutas @api.route)
- copiado tools\conn_tools.py (13 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## A-4 ping

```text
== conn/ping -> HTTP 200 en 46 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "ok":  true,
    "meta":  {
                 "operation":  "ping",
                 "addin_version":  "0.1.0",
                 "duration_ms":  2
             },
    "data":  {
                 "addin_version":  "0.1.0",
                 "backend":  "advancesteel",
                 "operations":  [
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
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "has_uidocument":  true,
                 "revit":  {
                               "version_number":  "2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "version_name":  "Autodesk Revit 2027"
                           },
                 "spec_version":  "1.0",
                 "document":  {
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_read_only":  false,
                                  "is_workshared":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
                              }
             }
}

```

## A-5 puente en 8000

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         16728



```

## A-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.1.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"errors":[],"warnings":[],"ok":true,"meta":{"operation":"ping","addin_version":"0.1.0","duration_ms":3},"data":{"addin_version":"0.1.0","backend":"advancesteel","operations":["create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12","load_context":"Default"},"has_uidocument":true,"revit":{"version_number":"2027","language":"English_USA","sub_version_number":"2027.2","version_build":"27.2.0.39","version_name":"Autodesk Revit 2027"},"spec_version":"1.0","document":{"is_family":false,"title":"HANGAR_PRUEBA_sondeo","is_read_only":false,"is_workshared":false,"is_modifiable":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"}}}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 8855 caracteres
Cuerpo:
{"errors":[],"warnings":[],"ok":true,"meta":{"operation":"guide","addin_version":"0.1.0","duration_ms":10},"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna a ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"errors":[],"warnings":[],"ok":true,"meta":{"operation":"types","addin_version":"0.1.0","duration_ms":1},"data":{"connection_types":[{"description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","type_name":"gusset_node"}]}}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"errors":[],"warnings":[],"ok":true,"meta":{"operation":"schema","addin_version":"0.1.0","duration_ms":1},"data":{"connection_type":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","example":{"chord":{"profile":"HSS3X3X1/4","continuous":true,"element_id":1249510},"uncertain_fields":[],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"end_setback_mm":180.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"type":"welded_slot","slot_length_mm":150.0},"element_id":1249630,"role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":45.0}],"connection_type":"gusset_node","dimension_chains":[{"expected_total_mm":565.0,"label":"borde superior","values_mm":[75.0,420.0,70.0]}],"spec_version":"1.0","source":{"scale":"1/10","drawing":"Detalle D"},"gusset":{"height_mm":530.0,"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"all_around":true,"type":"fillet"},"width_mm":565.0,"thickness_mm":9.5250000000000004,"chord_interface":"through_slot","thickness_label":"3/8\""}},"json_schema":{"properties":{"chord":{"additionalProperties":false,"properties":{"profile":{"type":["string","null"]},"continuous":{"type":"boolean"},"element_id":{"type":"integer"}},"type":"object","required":["elem ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","code":"UNKNOWN_OPERATION","path":"type","hint":"Tipos disponibles: gusset_node"}],"warnings":[],"ok":false,"meta":{"operation":"schema","addin_version":"0.1.0","duration_ms":1},"data":null}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"errors": [], "warnings": [], "ok": true, "meta": {"operation": "find_profile", "addin_version": "0.1.0", "duration_ms": 26}, "data": {"suggestions": [], "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "total_profiles_in_model": 29, "query": "HSS2-1/2X2-1/2X3/16", "matched_count": 1}}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cord�n=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"errors": [], "warnings": [], "ok": true, "meta": {"operation": "node_info", "addin_version": "0.1.0", "duration_ms": 36}, "data": {"origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "y_axis": [0, 0, 1], "members": [{"length_mm": 9960.2999999999993, "angle_in_plane_deg": 180, "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "type": "HSS3X3X1/4", "is_chord": true, "element_id": 1249510, "slope_deg": 0, "family": "HSS-Hollow Structural Section", "node_end": 1, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "structural_type": "Beam"}, {"length_mm": 3568, "angle_in_plane_deg": 43.100000000000001, "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "type": "HSS2-1-2X2-1-2X3-16 64x64", "is_chord": false, "element_id": 1249630, "slope_deg": 43.079999999999998, "family": "HSS2-1-2X2-1-2X3-16 64x64", "node_end": 1, "material": "Material IFC (190-40-140)", "structural_type": "Beam"}, {"length_mm": 3500, "angle_in_plane_deg": 135.59999999999999, "start_mm": [-11856.5, -17195.799999999999, 17437.299999999999], "end_mm": [-9354.7000000000007, -17195.799999999999, 19884.900000000001], "type": "HSS2-1-2X2-1-2X3-16 64x64", "is_chord": false, "element_id": 1249631, "slope_deg": 44.369999999999997, "family": "HSS2-1-2X2-1-2X3-16 64x64", "node_end": 0, "material": "Material IFC (190-40-140)", " ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=176744f3890f...
Cuerpo:
{"errors":[],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0) difiere del \u00e1ngulo en el modelo (43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"message":"El \u00e1ngulo del plano (90.0\u00b0) difiere del \u00e1ngulo en el modelo (135.6\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[1].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"ok":true,"meta":{"operation":"validate","addin_version":"0.1.0","duration_ms":135},"data":{"is_valid":true,"errors_count":0,"calculated_values":{"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"frame_z":[0,1,0],"frame_y":[0,0,1],"frame_x":[-1,0,0],"axis_distance_mm":0.080000000000000002},"validation_token":"176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c","warnings_count":2}}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","code":"DIMENSION_CHAIN_MISMATCH","path":"dimension_chains[0].values_mm","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm."}],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0) difiere del \u00e1ngulo en el modelo (43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"message":"El \u00e1ngulo del plano (90.0\u00b0) difiere del \u00e1ngulo en el modelo (135.6\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[1].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"ok":false,"meta":{"operation":"validate","addin_version":"0.1.0","duration_ms":13},"data":null}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[0].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."},{"message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","code":"UNRESOLVED_UNCERTAINTY","path":"uncertain_fields[1].user_confirmed_value","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."}],"warnings":[{"message":"El \u00e1ngulo del plano (45.0\u00b0) difiere del \u00e1ngulo en el modelo (43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[0].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"message":"El \u00e1ngulo del plano (90.0\u00b0) difiere del \u00e1ngulo en el modelo (135.6\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","path":"members[1].expected_angle_deg","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"ok":false,"meta":{"operation":"validate","addin_version":"0.1.0","duration_ms":12},"data":null}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"backend": "advancesteel", "weld_lines": 6, "knife_plates": 1, "connection_type": "gusset_node", "first_member_element_id": 1249630, "bolts": 4, "members_modified": 3, "chord_element_id": 1249510, "working_point_mm": [-11867.7, -17195.8, 17423], "dry_run": true, "gusset_plates": 1}
Cuerpo:
{"errors": [], "warnings": [], "ok": true, "meta": {"operation": "preview", "addin_version": "0.1.0", "duration_ms": 7}, "data": {"summary": {"backend": "advancesteel", "weld_lines": 6, "knife_plates": 1, "connection_type": "gusset_node", "first_member_element_id": 1249630, "bolts": 4, "members_modified": 3, "chord_element_id": 1249510, "working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "dry_run": true, "gusset_plates": 1}, "members_to_modify": [{"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -93.799999999999997, "current_end_distance_mm": 86.200000000000003, "element_id": 1249630, "end": "end", "setback_mm": 180, "role": "diagonal", "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -42, "current_end_distance_mm": 18, "element_id": 1249631, "end": "start", "setback_mm": 60, "role": "vertical", "profile": "HSS2-1-2X2-1-2X3-16 64x64"}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -210.19999999999999, "current_end_distance_mm": 49.799999999999997, "element_id": 1249636, "end": "end", "setback_mm": 260, "role": "diagonal", "profile": "HSS2-1-2X2-1-2X3-16 64x64"}], "elements_to_create": [{"kind": "gusset_plate", "vertices_count": 8, "height_mm": 530, "width_mm": 565, "thickness_mm": 9.5250000000000004, "chor ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"errors":[{"message":"validation_token es obligatorio para crear una conexi\u00f3n.","code":"VALIDATION_TOKEN_INVALID","path":"validation_token","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token."}],"warnings":[],"ok":false,"meta":{"operation":"create","addin_version":"0.1.0","duration_ms":0},"data":null}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"errors": [], "warnings": [], "ok": true, "meta": {"operation": "list", "addin_version": "0.1.0", "duration_ms": 8}, "data": {"connections": [], "connections_count": 0}}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","path":"connection_id","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo."}],"warnings":[],"ok":false,"meta":{"operation":"get","addin_version":"0.1.0","duration_ms":6},"data":null}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","path":"connection_id","hint":"Verifica los IDs disponibles con conn_list."}],"warnings":[],"ok":false,"meta":{"operation":"delete","addin_version":"0.1.0","duration_ms":5},"data":null}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","code":"UNKNOWN_OPERATION","path":null,"hint":"Operaciones disponibles: create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate."}],"warnings":[],"ok":false,"meta":{"operation":"no_existe","addin_version":"0.1.0","duration_ms":0},"data":null}
======================================================================
18. tools/list por el puente trae las 13 herramientas conn_*  [OK]  HTTP 200, herramientas=79 conn_*=13
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete
======================================================================
19. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.1.0
Cuerpo:
{
  "errors": [],
  "warnings": [],
  "ok": true,
  "meta": {
    "operation": "ping",
    "addin_version": "0.1.0",
    "duration_ms": 2
  },
  "data": {
    "addin_version": "0.1.0",
    "backend": "advancesteel",
    "operations": [
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
    "dotnet": {
      "assembly_location": "C:\\Users\\Andy Bayona Ant�n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12",
      "load_context": "Default"
    },
    "has_uidocument": true,
    "revit": {
      "version_number": "2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39",
      "version_name": "Autodesk Revit 2027"
    },
    "spec_version": "1.0",
    "document": {
      "is_family": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "is_read_only": false,
      "is_workshared": false,
      "is_modifiable": false,
      "path": "D:\\IG INGENIER�A\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
    }
  }
}
======================================================================
Resultado: 19/19 pruebas correctas

```

## A-7 sondeo 14 token y limits

```text
== 14-fase5-token-limits.py -> HTTP 200 en 690 ms ==
=== 14-fase5-token-limits ===
Bridge.Handle encontrado: True | Revit.dll 0.1.0.0 | Core.dll 0.1.0.0
DLL desplegada: C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll
--- ping: ok=True en 8 ms | errores=- | avisos=-
    operations: create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate
[OK] ping ok y sin operaciones probe_*
[OK] ping con 13 operaciones: 13
[OK] documento de ping es la copia _sondeo: HANGAR_PRUEBA_sondeo
[OK] Core desplegado tiene LimitsConfig.ComputeHash (Fase 5)
    limits.json desplegado: C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json (existe=True)
    hash desplegado=617a195a2089a70d559e9ba25e2f92eaf81848e09d5956447dbb1a79e4eefb56
    hash repositorio=617a195a2089a70d559e9ba25e2f92eaf81848e09d5956447dbb1a79e4eefb56
[OK] limits.json desplegado existe y tiene el mismo hash que el del repositorio
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones antes: 0
    barra 1249510: extension inicio/fin (mm) = (-3.0259999999999998, -1.7330000000000001)
    barra 1249630: extension inicio/fin (mm) = (0.0, 68.640000000000001)
    barra 1249631: extension inicio/fin (mm) = (0.0, 69.200000000000003)
    barra 1249636: extension inicio/fin (mm) = (0.0, 0.0)
--- validate: ok=True en 25 ms | errores=- | avisos=ANGLE_DIFFERS_FROM_MODEL, ANGLE_DIFFERS_FROM_MODEL
--- validate: ok=True en 14 ms | errores=- | avisos=ANGLE_DIFFERS_FROM_MODEL, ANGLE_DIFFERS_FROM_MODEL
    token 1: 176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c
    token 2: 176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c
[OK] validate del fixture confirmado: is_valid con token de 64 hex
[OK] validar dos veces da el mismo token (determinista)
--- create: ok=False en 6 ms | errores=VALIDATION_TOKEN_INVALID | avisos=-
    ERROR VALIDATION_TOKEN_INVALID [validation_token]: El validation_token no es válido o el modelo ha cambiado desde que se realizó la validación.
[OK] create con token alterado -> VALIDATION_TOKEN_INVALID: VALIDATION_TOKEN_INVALID
    token con otro limits.json: 97c6795c0a8eea9786d8935b5883fed8390096aaa359f7d2c987f3dbf523f037
    token con el limits.json desplegado: 176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c
[OK] el token cambia al cambiar limits.json
[OK] token de validate == token calculado con el limits.json desplegado
--- create: ok=False en 11 ms | errores=VALIDATION_TOKEN_INVALID | avisos=-
    ERROR VALIDATION_TOKEN_INVALID [validation_token]: El validation_token no es válido o el modelo ha cambiado desde que se realizó la validación.
[OK] create con token de otro limits.json -> VALIDATION_TOKEN_INVALID: VALIDATION_TOKEN_INVALID
--- list: ok=True en 4 ms | errores=- | avisos=-
[OK] conn_list igual que antes (nada creado): 0 -> 0
[OK] extensiones de las barras sin cambios: {1249510: (-3.0259999999999998, -1.7330000000000001), 1249630: (0.0, 68.640000000000001), 1249631: (0.0, 69.200000000000003), 1249636: (0.0, 0.0)}
[OK] documento sin transaccion abierta
RESULTADO: 14/14 comprobaciones correctas
=== fin 14-fase5-token-limits ===


```

## A-8 log add-in

```text

{"ts":"2026-10-01T00:50:58.6021268-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\
": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":true,"error
_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":135}}
{"ts":"2026-10-01T00:50:58.6441698-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\
": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":false,"erro
r_codes":["DIMENSION_CHAIN_MISMATCH"],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration
_ms":13}}
{"ts":"2026-10-01T00:50:58.7041933-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\
": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":false,"erro
r_codes":["UNRESOLVED_UNCERTAINTY","UNRESOLVED_UNCERTAINTY"],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS
_FROM_MODEL"],"duration_ms":12}}
{"ts":"2026-10-01T00:50:58.7898494-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\":
 {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la 
imag...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-01T00:51:08.8779591-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\
": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante est\\u00e1 cortada en la...","ok":true,"erro
r_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":16}}
{"ts":"2026-10-01T00:51:08.8925288-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\
": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante est\\u00e1 cortada en la...","ok":true,"erro
r_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":10}}
{"ts":"2026-10-01T00:51:08.9136438-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"validat
ion_token\": \"176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb090\", \"spec\": {\"chord\": {\"profile\": 
\"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-10-01T00:51:08.9379209-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"validat
ion_token\": \"97c6795c0a8eea9786d8935b5883fed8390096aaa359f7d2c987f3dbf523f037\", \"spec\": {\"chord\": {\"profile\": 
\"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": 
[{\"...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":3}}



```

## B-3 sondeo 12 extensiones tras conn_delete

```text
== 12-fase3-borrar.py -> HTTP 200 en 527 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 7 ms | errores=- | avisos=-
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


## B-1 Antigravity (punta a punta)

================================================================================
PRUEBA DE PUNTA A PUNTA: ADD-IN MotorConexiones CON HERRAMIENTAS MCP conn_*
Modelo: HANGAR_PRUEBA_sondeo.rvt
Especificacion: docs/fixtures/detalle-D-confirmado.json (Detalle D)
================================================================================

[PASO 1] conn_ping
- Herramienta: conn_ping
- ok: true
- errors: []
- warnings: []
- duracion: 4 ms
- Datos clave:
    * backend: "advancesteel"
    * addin_version: "0.1.0"
    * spec_version: "1.0"
    * revit_build: "27.2.0.39" (Revit 2027.2 / .NET 10.0.12)
    * document.title: "HANGAR_PRUEBA_sondeo"
    * operations (13): create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate

[PASO 2] conn_get_guide
- Herramienta: conn_get_guide
- ok: true
- errors: []
- warnings: []
- duracion: 1 ms
- Datos clave: Guia recuperada con exito (flujo obligatorio y reglas de seguridad descritas).

[PASO 3] conn_list_types
- Herramienta: conn_list_types
- ok: true
- errors: []
- warnings: []
- duracion: 0 ms
- Datos clave:
    * connection_types: ["gusset_node"] (descripcion: nudo de cercha con cartela plana y cordón continuo).

[PASO 4] conn_get_node_info
- Herramienta: conn_get_node_info
- Argumentos: element_ids=[1249510, 1249630, 1249631, 1249636], chord_element_id=1249510
- ok: true
- errors: []
- warnings: []
- duracion: 7 ms
- Datos clave:
    * origin_mm: [-11867.7, -17195.8, 17423.0]
    * axis_distance_mm: 0.08
    * existing_connections: []
    * Miembros identificados:
        - 1249510 (Cordon): HSS3X3X1/4, angle_in_plane_deg: 180.0
        - 1249630 (Barra):  HSS2-1-2X2-1-2X3-16 64x64, angle_in_plane_deg: 43.1
        - 1249631 (Barra):  HSS2-1-2X2-1-2X3-16 64x64, angle_in_plane_deg: 135.6
        - 1249636 (Barra):  HSS2-1-2X2-1-2X3-16 64x64, angle_in_plane_deg: 44.4

[PASO 5] conn_get_schema
- Herramienta: conn_get_schema
- Argumentos: connection_type="gusset_node"
- ok: true
- errors: []
- warnings: []
- duracion: 0 ms
- Datos clave:
    * Claves en data: ["connection_type", "description", "example", "json_schema"]

[PASO 6] conn_validate
- Herramienta: conn_validate
- Argumentos: spec=docs/fixtures/detalle-D-confirmado.json
- ok: true
- errors: [] (0 errores)
- warnings: [ANGLE_DIFFERS_FROM_MODEL, ANGLE_DIFFERS_FROM_MODEL]
    * members[0].expected_angle_deg: plano 45.0 vs modelo 43.1 (dif 1.9 > 1)
    * members[1].expected_angle_deg: plano 90.0 vs modelo 135.6 (dif 45.6 > 1)
- duracion: 17 ms
- Datos clave:
    * is_valid: true
    * validation_token: 176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c (64 caracteres hex)
    * calculated_values: origin_mm=[-11867.7, -17195.8, 17423.0], axis_distance_mm=0.08, frame_x=[-1, 0, 0], frame_y=[0, 0, 1], frame_z=[0, 1, 0]

[PASO 7] conn_preview
- Herramienta: conn_preview
- Argumentos: misma spec
- ok: true
- errors: []
- warnings: []
- duracion: 2 ms
- Datos clave:
    * summary:
        - backend: "advancesteel"
        - connection_type: "gusset_node"
        - gusset_plates: 1
        - knife_plates: 1
        - bolts: 4
        - weld_lines: 6
        - members_modified: 3
        - working_point_mm: [-11867.7, -17195.8, 17423.0]
        - dry_run: true
    * members_to_modify:
        - 1249630 (diagonal): setback 180.0 mm -> new_extension_mm: -93.8 mm
        - 1249631 (vertical): setback 60.0 mm  -> new_extension_mm: -42.0 mm
        - 1249636 (diagonal): setback 260.0 mm -> new_extension_mm: -210.2 mm

[PASO 8] Confirmacion explicita de creacion
- Respuesta usuario: "SI, CREA"

[PASO 9] conn_create
- Herramienta: conn_create
- Argumentos: spec=Detalle D, validation_token="176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c"
- ok: true
- errors: []
- warnings: [REVIT_WARNING, REVIT_WARNING, REVIT_WARNING] ("The created elements are only visible in Detail Level: Fine.")
- duracion: 2191 ms
- Datos clave:
    * connection_id: "c62a7ed8-6653-4a35-8b0e-ed6f116aaf22"
    * backend: "advancesteel"
    * created_elements_count: 9
    * created_element_ids: [1321392, 1321393, 1321394, 1321395, 1321396, 1321397, 1321398, 1321399, 1321400]
    * desglose de elementos creados (segun log fabrication_transaction_commit):
        - 1321392..1321397: DirectShape | Structural Connections (6 lineas de soldadura)
        - 1321398: SteelProxyElement | Plates (Cartela Detalle D)
        - 1321399: SteelProxyElement | Plates (Placa cuchilla miembro 1249636)
        - 1321400: SteelProxyElement | Bolts (Patron de 4 pernos)

[PASO 10] conn_list y conn_get
- Herramienta: conn_list
    * ok: true, errors: [], warnings: [], duracion: 8 ms
    * connections_count: 1
- Herramienta: conn_get
    * ok: true, errors: [], warnings: [], duracion: 7 ms
    * connection_id: "c62a7ed8-6653-4a35-8b0e-ed6f116aaf22"
    * backend: "advancesteel"
    * created_elements_count: 9
    * created_utc: "2026-10-01T06:05:32.4049153Z"

[PASO 11] Confirmacion explicita de borrado
- Respuesta usuario: "SI, BORRA"

[PASO 12] conn_delete y conn_list final
- Herramienta: conn_delete
    * Argumentos: connection_id="c62a7ed8-6653-4a35-8b0e-ed6f116aaf22"
    * ok: true
    * errors: []
    * warnings: []
    * duracion: 127 ms
    * deleted_connection_id: "c62a7ed8-6653-4a35-8b0e-ed6f116aaf22"
    * deleted_elements_count: 9
    * restored_members_count: 3 (longitudes originales de barras restituidas)
- Herramienta: conn_list
    * ok: true
    * errors: []
    * warnings: []
    * duracion: 5 ms
    * connections_count: 0
    * connections: []

================================================================================
FIN DE LA PRUEBA E2E: Todos los pasos completados exitosamente.
================================================================================

## B-2 comprobación a ojo

NO: con detalle Fino, Sombreado y las categorías Placas, Pernos y Conexiones estructurales
activas, solo se ven los retiros de las 3 barras en el nudo.
Seleccionar por ID 1321398 (Plates): se selecciona; Steel ASTM A36, Thickness 0' 0",
Length 0' 0 5/64", Width 0' 0 3/64", Weight 0.00 lbm.
Seleccionar por ID 1321400 (Bolts): se selecciona; A325, grado 10.9, Diameter vacío,
Bolt Length 0' 0 1/256", Length on side 1/2 0' 0 1/128", Intermediate distance 0' 0 1/128",
Number on side 1/2 = 2/2.
Diagnóstico: Advance Steel espera milímetros y el add-in le pasa pies; todas las medidas
llegan divididas entre 304,8 (60 mm -> 0,197 mm = 1/128"). Las piezas existen pero son
diminutas y quedan cerca del origen del modelo, no en el nudo.
Captura: docs/fases/capturas/fase-5-B2-nudo-fino.png

# Ronda 5b: Advance Steel en milímetros

Fecha: 2026-10-01T08:27:34


## 5b-1 git

```text
19e34f5 Merge branch 'claude/laughing-pascal-tsxvkt' of https://github.com/Andy-rba30/CONEXIONES into claude/laughing-pascal-tsxvkt

```

## 5b-2 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos est├ín actualizados para la restauraci├│n.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:04.72
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:    51, Omitido:     0, Total:    51, Duraci├│n: 170 ms - MotorConexiones.Tests.dll (net10.0)

```

## 5b-2 revit cerrado

```text

```

## 5b-2 deploy

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, docs\guide.md
Siguiente paso: abre Revit 2027. Debe aparecer la pestana 'Conexiones'.

```

## 5b-3 ping

```text
== conn/ping -> HTTP 200 en 290 ms ==
{
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true,
    "meta":  {
                 "operation":  "ping",
                 "addin_version":  "0.1.0",
                 "duration_ms":  7
             },
    "data":  {
                 "backend":  "advancesteel",
                 "spec_version":  "1.0",
                 "addin_version":  "0.1.0",
                 "dotnet":  {
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default"
                            },
                 "operations":  [
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
                                  "is_workshared":  false,
                                  "is_modifiable":  false,
                                  "is_family":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
                              },
                 "has_uidocument":  true,
                 "revit":  {
                               "version_build":  "27.2.0.39",
                               "language":  "English_USA",
                               "version_number":  "2027",
                               "sub_version_number":  "2027.2",
                               "version_name":  "Autodesk Revit 2027"
                           }
             }
}

```

## 5b-4 sondeo 11 crear y medir

```text
== 11-fase3-crear.py -> HTTP 200 en 2128 ms ==
=== 11-fase3-crear ===
Bridge.Handle encontrado: True | version del ensamblado: 0.1.0.0
--- ping: ok=True en 5 ms | errores=- | avisos=-
    backend=advancesteel | revit=27.2.0.39 | documento=HANGAR_PRUEBA_sondeo
--- guide: ok=True en 14 ms | errores=- | avisos=-
    guia: 8855 caracteres
--- types: ok=True en 3 ms | errores=- | avisos=-
    tipos: [{"type_name": "gusset_node", "description": "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]
--- schema: ok=True en 9 ms | errores=- | avisos=-
    claves de data: connection_type, description, example, json_schema
--- find_profile: ok=True en 28 ms | errores=- | avisos=-
    {"suggestions": [], "matched_count": 1, "total_profiles_in_model": 29, "query": "HSS2-1/2X2-1/2X3/16", "matches": [{"family_name": "HSS2-1-2X2-1-2X3-16 64x64", "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}]}
--- node_info: ok=True en 40 ms | errores=- | avisos=-
    {"origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "members": [{"material": "Steel ASTM A500, Grade B, Rectangular and Square", "type": "HSS3X3X1/4", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "family": "HSS-Hollow Structural Section", "structural_type": "Beam", "element_id": 1249510, "angle_in_plane_deg": 180, "length_mm": 9960.2999999999993, "is_chord": true, "slope_deg": 0, "node_end": 1}, {"material": "Material IFC (190-40-140)", "type": "HSS2-1-2X2-1-2X3-16 64x64", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "structural_type": "Beam", "element_id": 1249630, "angle_in_plane_deg": 43.100000000000001, "length_mm": 3568, "is_chord": false, "slope_deg" ...
--- validate: ok=True en 130 ms | errores=- | avisos=ANGLE_DIFFERS_FROM_MODEL, ANGLE_DIFFERS_FROM_MODEL
    aviso ANGLE_DIFFERS_FROM_MODEL: El ángulo del plano (45.0°) difiere del ángulo en el modelo (43.1°) por 1.9° > 1°.
    aviso ANGLE_DIFFERS_FROM_MODEL: El ángulo del plano (90.0°) difiere del ángulo en el modelo (135.6°) por 45.6° > 1°.
    is_valid=True | token=176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb09c
    calculados: {"origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "frame_z": [0, 1, 0], "frame_y": [0, 0, 1], "axis_distance_mm": 0.080000000000000002, "frame_x": [-1, 0, 0]}
--- preview: ok=True en 16 ms | errores=- | avisos=-
    {"backend": "advancesteel", "working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "chord_element_id": 1249510, "first_member_element_id": 1249630, "gusset_plates": 1, "bolts": 4, "connection_type": "gusset_node", "weld_lines": 6, "dry_run": true, "knife_plates": 1, "members_modified": 3}
    retiro: {"end": "end", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -93.799999999999997, "current_end_distance_mm": 86.200000000000003, "element_id": 1249630, "setback_mm": 180, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagona ...
    retiro: {"end": "start", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -42, "current_end_distance_mm": 18, "element_id": 1249631, "setback_mm": 60, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "vertical"}
    retiro: {"end": "end", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -210.19999999999999, "current_end_distance_mm": 49.799999999999997, "element_id": 1249636, "setback_mm": 260, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagona ...
--- create: ok=True en 1176 ms | errores=- | avisos=REVIT_WARNING, REVIT_WARNING, REVIT_WARNING
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    connection_id=dadbdfa2-f041-4630-b97f-d4e4695942ea | elementos creados=9 | ids=[1321345, 1321346, 1321347, 1321348, 1321349, 1321350, 1321351, 1321352, 1321353]
--- list: ok=True en 13 ms | errores=- | avisos=-
    {"connections_count": 1, "connections": [{"connection_id": "dadbdfa2-f041-4630-b97f-d4e4695942ea", "backend": "advancesteel", "spec_version": "1.0", "created_utc": "2026-10-01T13:30:06.0054796Z", "connection_type": "gusset_node", "created_elements_count": 9}]}
--- get: ok=True en 8 ms | errores=- | avisos=-
    backend=advancesteel | elementos=9 | creado=2026-10-01T13:30:06.0054796Z
    [1321345] DirectShape | Structural Connections
    [1321346] DirectShape | Structural Connections
    [1321347] DirectShape | Structural Connections
    [1321348] DirectShape | Structural Connections
    [1321349] DirectShape | Structural Connections
    [1321350] DirectShape | Structural Connections
    [1321351] SteelProxyElement | Plates
        Thickness: 0' - 0 3/8" = 9.53 mm
        Length: 1' - 10 1/4" = 565.0 mm
        Width: 1' - 8 7/8" = 530.0 mm
    [1321352] SteelProxyElement | Plates
        Thickness: 0' - 0 13/32" = 10.0 mm
        Length: 0' - 6 11/16" = 170.0 mm
        Width: 0' - 5 1/2" = 140.0 mm
    [1321353] SteelProxyElement | Bolts
        Diameter:  5/8 inch
        Bolt Length: 0' - 1 25/32" = 45.0 mm
        Grip Length: 0' - 3 5/32" = 80.0 mm
        Length on side 1: 0' - 2 3/8" = 60.0 mm
        Length on side 2: 0' - 2 3/8" = 60.0 mm
        Intermediate distance on side 1: 0' - 2 3/8" = 60.0 mm
        Intermediate distance on side 2: 0' - 2 3/8" = 60.0 mm
        Number on side 1: 2
        Number on side 2: 2
        Standard: A325
        Grade: 10.9
    ESPERADO (Detalle D): cartela Thickness 9.5 mm, Length/Width ~565 x 530 mm; placa cuchilla 10 x 170 x 140 mm;
    pernos Diameter 15.9 mm (5/8"), Length on side / Intermediate distance 60 mm, Number on side 2 y 2.
    Si salen ~300 veces mas pequenas (2 mm, 0.2 mm, 0) las unidades siguen llegando en pies.
CONEXION CREADA: dadbdfa2-f041-4630-b97f-d4e4695942ea. Haz la captura y despues ejecuta el sondeo 12 para borrarla.
=== fin 11-fase3-crear ===


```

## 5b-5 captura exportada

```text
== capturar-nudo.py -> HTTP 200 en 1804 ms ==
captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase3-captura.png (OK)


```

## 5b-6 sondeo 12 borrar

```text
== 12-fase3-borrar.py -> HTTP 200 en 669 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 9 ms | errores=- | avisos=-
    conexiones en el modelo: 1
--- get: ok=True en 6 ms | errores=- | avisos=-
--- delete: ok=True en 153 ms | errores=- | avisos=-
    {"restored_members_count": 3, "deleted_elements_count": 9, "deleted_connection_id": "dadbdfa2-f041-4630-b97f-d4e4695942ea"}
    miembro 1249636: extension inicio 0.0 -> 0.0 mm | fin -210.210261645 -> 0.0 mm
    miembro 1249630: extension inicio 0.0 -> 0.0 mm | fin -93.8379623742 -> 68.6404746190 mm
    miembro 1249631: extension inicio -41.9844021063 -> 0.0 mm | fin 69.1996468112 -> 69.1996468112 mm
--- list: ok=True en 4 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 5b-6 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 44 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 5b-7 log

```text
{"ts":"2026-10-01T00:05:14.1940803-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"source\": {\"drawing\": \"Detalle D\", \"scale\": \"1/10\"}, \"gusset\": {\"weld_to_chord\": {\"all_around\": true, \"size_mm\": 5.0, \"type\": \"fillet\"}, \"thickness_label\": \"3/8\\\"\", \"chord_interface\": \"throu...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-01T00:05:14.2915640-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"00000000-0000-0000-0000-000000000000\"}","ok":false,"error_codes":["ELEMENT_NOT_FOUND"],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-10-01T00:19:58.9650765-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"source\": {\"drawing\": \"Detalle D\", \"scale\": \"1/10\"}, \"gusset\": {\"weld_to_chord\": {\"all_around\": true, \"size_mm\": 5.0, \"type\": \"fillet\"}, \"thickness_label\": \"3/8\\\"\", \"chord_interface\": \"throu...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-01T00:19:59.0675822-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"00000000-0000-0000-0000-000000000000\"}","ok":false,"error_codes":["ELEMENT_NOT_FOUND"],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-10-01T00:50:58.7898494-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": [{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-01T00:50:58.9170166-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"00000000-0000-0000-0000-000000000000\"}","ok":false,"error_codes":["ELEMENT_NOT_FOUND"],"warning_codes":[],"duration_ms":5}}
{"ts":"2026-10-01T00:51:08.9136438-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"validation_token\": \"176744f3890f8650d7f3d46261467e26f0865e043a62d1a86ea29eb1083cb090\", \"spec\": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": [{\"...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-10-01T00:51:08.9379209-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"validation_token\": \"97c6795c0a8eea9786d8935b5883fed8390096aaa359f7d2c987f3dbf523f037\", \"spec\": {\"chord\": {\"profile\": \"HSS3X3X1/4\", \"continuous\": true, \"element_id\": 1249510}, \"uncertain_fields\": [{\"...","ok":false,"error_codes":["VALIDATION_TOKEN_INVALID"],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-10-01T01:05:30.6666913-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: crear c62a7ed8-6653-4a35-8b0e-ed6f116aaf22","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-01T01:05:31.5823326-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle D","vertices":8,"thickness_mm":9.525}}
{"ts":"2026-10-01T01:05:31.6232253-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla miembro 1249636","vertices":4,"thickness_mm":10}}
{"ts":"2026-10-01T01:05:32.0926993-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 1249636","properties":["Nx=2","Ny=2","Dx=0.19685039370078738","Dy=0.19685039370078738","ScrewDiameter=0.05208333333333333","ScrewLength=0.14763779527559054"],"count":4}}
{"ts":"2026-10-01T01:05:32.4011921-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: crear c62a7ed8-6653-4a35-8b0e-ed6f116aaf22","pending":3,"new_elements":["1321392:Structural Connections:DirectShape","1321393:Structural Connections:DirectShape","1321394:Structural Connections:DirectShape","1321395:Structural Connections:DirectShape","1321396:Structural Connections:DirectShape","1321397:Structural Connections:DirectShape","1321398:Plates:SteelProxyElement","1321399:Plates:SteelProxyElement","1321400:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-01T01:05:32.7805693-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"chord\": {\"continuous\": true, \"profile\": \"HSS3X3X1/4\", \"element_id\": 1249510}, \"uncertain_fields\": [{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":true,"error_codes":[],"warning_codes":["REVIT_WARNING","REVIT_WARNING","REVIT_WARNING"],"duration_ms":2191}}
{"ts":"2026-10-01T01:21:23.2193996-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: borrar c62a7ed8-6653-4a35-8b0e-ed6f116aaf22","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-01T01:21:23.2434134-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: borrar c62a7ed8-6653-4a35-8b0e-ed6f116aaf22","pending":0,"new_elements":[],"is_modifiable_after":true}}
{"ts":"2026-10-01T01:21:23.3170844-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"c62a7ed8-6653-4a35-8b0e-ed6f116aaf22\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":127}}
{"ts":"2026-10-01T08:30:05.2939918-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: crear dadbdfa2-f041-4630-b97f-d4e4695942ea","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-01T08:30:05.3560743-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle D","vertices":8,"thickness_mm":9.525,"units":"mm"}}
{"ts":"2026-10-01T08:30:05.3893481-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla miembro 1249636","vertices":4,"thickness_mm":10,"units":"mm"}}
{"ts":"2026-10-01T08:30:05.8161996-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 1249636","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","ScrewLength=45"],"count":4,"units":"mm"}}
{"ts":"2026-10-01T08:30:06.0008587-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: crear dadbdfa2-f041-4630-b97f-d4e4695942ea","pending":3,"new_elements":["1321345:Structural Connections:DirectShape","1321346:Structural Connections:DirectShape","1321347:Structural Connections:DirectShape","1321348:Structural Connections:DirectShape","1321349:Structural Connections:DirectShape","1321350:Structural Connections:DirectShape","1321351:Plates:SteelProxyElement","1321352:Plates:SteelProxyElement","1321353:Bolts:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-01T08:30:06.4040516-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"gusset\": {\"height_mm\": 530.0, \"outline\": {\"mode\": \"polygon\", \"points_mm\": [[-175.0, 280.0], [245.0, 280.0], [315.0, 210.0], [315.0, -40.0], [-35.0, -250.0], [-125.0, -250.0], [-250.0, -115....","ok":true,"error_codes":[],"warning_codes":["REVIT_WARNING","REVIT_WARNING","REVIT_WARNING"],"duration_ms":1163}}
{"ts":"2026-10-01T08:35:12.6451060-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: borrar dadbdfa2-f041-4630-b97f-d4e4695942ea","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-01T08:35:12.6813025-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: borrar dadbdfa2-f041-4630-b97f-d4e4695942ea","pending":0,"new_elements":[],"is_modifiable_after":true}}
{"ts":"2026-10-01T08:35:12.7615649-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"dadbdfa2-f041-4630-b97f-d4e4695942ea\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":150}}
```
