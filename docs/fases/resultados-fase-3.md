# Resultados de la Fase 3

Fecha: 2026-09-30T21:51:41


## 2 git

```text
git : Already on 'claude/laughing-pascal-tsxvkt'
En D:\Proyectos C#\CONEXIONES\scripts\ejecutar-fase3-pasos1-5.ps1: 13 Carácter: 35
+ ...  git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pu ...
+                        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (Already on 'cla...-pascal-tsxvkt':String) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
 
Your branch is up to date with 'origin/claude/laughing-pascal-tsxvkt'.
git : From https://github.com/Andy-rba30/CONEXIONES
En D:\Proyectos C#\CONEXIONES\scripts\ejecutar-fase3-pasos1-5.ps1: 13 Carácter: 79
+ ... cal-tsxvkt; git pull --no-rebase origin claude/laughing-pascal-tsxvkt ...
+                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (From https://gi...ba30/CONEXIONES:String) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
 
 * branch            claude/laughing-pascal-tsxvkt -> FETCH_HEAD
Already up to date.
e823648 Merge remote-tracking branch 'origin/claude/laughing-pascal-tsxvkt'

```

## 3 dotnet build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:04.56
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    46, Omitido:     0, Total:    46, Duración: 158 ms - MotorConexiones.Tests.dll (net10.0)

```

## 4 revit cerrado

```text

```

## 5 deploy

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, docs\guide.md
Siguiente paso: abre Revit 2027. Debe aparecer la pestana 'Conexiones'.

```

## 6 ping

```text
== conn/ping -> HTTP 200 en 52 ms ==
{
    "ok":  true,
    "errors":  [

               ],
    "data":  {
                 "spec_version":  "1.0",
                 "dotnet":  {
                                "load_context":  "Default",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12"
                            },
                 "addin_version":  "0.1.0",
                 "backend":  "advancesteel",
                 "has_uidocument":  true,
                 "revit":  {
                               "version_build":  "27.2.0.39",
                               "sub_version_number":  "2027.2",
                               "version_number":  "2027",
                               "version_name":  "Autodesk Revit 2027",
                               "language":  "English_USA"
                           },
                 "document":  {
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_read_only":  false,
                                  "is_workshared":  false,
                                  "is_family":  false
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
                                    "probe_delete_b",
                                    "probe_plate_b",
                                    "schema",
                                    "types",
                                    "update",
                                    "validate"
                                ]
             },
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "ping",
                 "duration_ms":  2
             },
    "warnings":  [

                 ]
}

```

## 7 sondeo 13 limpiar

```text
== 13-limpiar-fase1.py -> HTTP 200 en 4043 ms ==
=== 13-limpiar-fase1 ===
   [1321328] SteelProxyElement | Plates
   [1321344] SteelProxyElement | Bolts
1) Elementos de acero sueltos encontrados: 2
2) Borrados 2 elementos. Commit OK
3) Copia guardada
=== fin 13-limpiar-fase1 ===


```

## 8 sondeo 11 crear

```text
== 11-fase3-crear.py -> HTTP 200 en 1945 ms ==
=== 11-fase3-crear ===
Bridge.Handle encontrado: True | version del ensamblado: 0.1.0.0
--- ping: ok=True en 6 ms | errores=- | avisos=-
    backend=advancesteel | revit=27.2.0.39 | documento=HANGAR_PRUEBA_sondeo
--- guide: ok=True en 4 ms | errores=- | avisos=-
    guia: 1266 caracteres
--- types: ok=True en 11 ms | errores=- | avisos=-
    tipos: [{"type_name": "gusset_node", "description": "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]
--- schema: ok=True en 10 ms | errores=- | avisos=-
    claves de data: connection_type, description, example, json_schema
--- find_profile: ok=True en 26 ms | errores=- | avisos=-
    {"matched_count": 1, "matches": [{"family_name": "HSS2-1-2X2-1-2X3-16 64x64", "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false}], "query": "HSS2-1/2X2-1/2X3/16", "total_profiles_in_model": 29, "suggestions": []}
--- node_info: ok=True en 32 ms | errores=- | avisos=-
    {"axis_distance_mm": 0.080000000000000002, "z_axis": [-1.9999999999999999e-06, 1, 0], "members": [{"start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "length_mm": 9960.2999999999993, "element_id": 1249510, "angle_in_plane_deg": 180, "is_chord": true, "structural_type": "Beam", "family": "HSS-Hollow Structural Section", "slope_deg": 0, "material": "Steel ASTM A500, Grade B, Rectangular and Square", "node_end": 1, "type": "HSS3X3X1/4"}, {"start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "length_mm": 3568, "element_id": 1249630, "angle_in_plane_deg": 43.100000000000001, "is_chord": false, "structural_type": "Beam", "family": "HSS2-1-2X2-1-2X3-16 64x64", "slope_deg": 43.079999999999998, "material": "Material IFC (190-40-1 ...
--- validate: ok=True en 104 ms | errores=- | avisos=ANGLE_DIFFERS_FROM_MODEL, ANGLE_DIFFERS_FROM_MODEL
    aviso ANGLE_DIFFERS_FROM_MODEL: El ángulo del plano (45.0°) difiere del ángulo en el modelo (43.1°) por 1.9° > 1°.
    aviso ANGLE_DIFFERS_FROM_MODEL: El ángulo del plano (90.0°) difiere del ángulo en el modelo (135.6°) por 45.6° > 1°.
    is_valid=True | token=22e4e1d7cb5febd08b290c7c8817e78c83125b70645ed52916ae8dd6825f59ea
    calculados: {"frame_y": [0, 0, 1], "axis_distance_mm": 0.080000000000000002, "frame_x": [-1, 0, 0], "origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "frame_z": [0, 1, 0]}
--- preview: ok=True en 21 ms | errores=- | avisos=-
    {"knife_plates": 1, "members_modified": 3, "gusset_plates": 1, "first_member_element_id": 1249630, "working_point_mm": [-11867.700000000001, -17195.799999999999, 17423], "connection_type": "gusset_node", "backend": "advancesteel", "dry_run": true, "bolts": 4, "weld_lines": 6, "chord_element_id": 1249510}
    retiro: {"current_end_distance_mm": 86.200000000000003, "setback_mm": 180, "end": "end", "element_id": 1249630, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -93.7999999999999 ...
    retiro: {"current_end_distance_mm": 18, "setback_mm": 60, "end": "start", "element_id": 1249631, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "vertical", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -42}
    retiro: {"current_end_distance_mm": 49.799999999999997, "setback_mm": 260, "end": "end", "element_id": 1249636, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "role": "diagonal", "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "new_extension_mm": -210.199999999999 ...
--- create: ok=True en 1134 ms | errores=- | avisos=REVIT_WARNING, REVIT_WARNING, REVIT_WARNING, REVIT_WARNING, REVIT_WARNING, REVIT_WARNING
    aviso REVIT_WARNING: Advance Steel no creó ningún elemento para la placa 'Cartela Detalle D'; se crea con DirectShape.
    aviso REVIT_WARNING: Advance Steel no creó ningún elemento para la placa 'Placa cuchilla miembro 1249636'; se crea con DirectShape.
    aviso REVIT_WARNING: Advance Steel no creó ningún elemento para los pernos 'Pernos miembro 1249636'; se crean con DirectShape.
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    aviso REVIT_WARNING: Advertencia de Revit: The created elements are only visible in Detail Level: Fine.
    connection_id=70234718-7dde-47b5-baf6-32f8a1e7fd09 | elementos creados=12 | ids=[1321345, 1321346, 1321347, 1321348, 1321349, 1321350, 1321351, 1321352, 1321353, 1321354, 1321355, 1321356]
--- list: ok=True en 9 ms | errores=- | avisos=-
    {"connections": [{"created_elements_count": 12, "connection_id": "70234718-7dde-47b5-baf6-32f8a1e7fd09", "spec_version": "1.0", "created_utc": "2026-10-01T03:08:00.3141709Z", "connection_type": "gusset_node"}], "connections_count": 1}
--- get: ok=True en 9 ms | errores=- | avisos=-
    backend=None | elementos=12 | creado=2026-10-01T03:08:00.3141709Z
    [1321345] DirectShape | Structural Connections
    [1321346] DirectShape | Structural Connections
    [1321347] DirectShape | Structural Connections
    [1321348] DirectShape | Structural Connections
    [1321349] DirectShape | Structural Connections
    [1321350] DirectShape | Structural Connections
    [1321351] DirectShape | Structural Connections
    [1321352] DirectShape | Structural Connections
    [1321353] DirectShape | Structural Connections
    [1321354] DirectShape | Structural Connections
    [1321355] DirectShape | Structural Connections
    [1321356] DirectShape | Structural Connections
CONEXION CREADA: 70234718-7dde-47b5-baf6-32f8a1e7fd09. Haz la captura y despues ejecuta el sondeo 12 para borrarla.
=== fin 11-fase3-crear ===


```

## 10 sondeo 12 borrar

```text
== 12-fase3-borrar.py -> HTTP 200 en 593 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones en el modelo: 1
--- get: ok=True en 5 ms | errores=- | avisos=-
--- delete: ok=True en 82 ms | errores=- | avisos=-
    {"deleted_elements_count": 12, "restored_members_count": 3, "deleted_connection_id": "70234718-7dde-47b5-baf6-32f8a1e7fd09"}
    miembro 1249636: extension inicio 0.0 -> 0.0 mm | fin -210.210261645 -> 0.0 mm
    miembro 1249630: extension inicio 0.0 -> 0.0 mm | fin -93.8379623742 -> 68.6404746190 mm
    miembro 1249631: extension inicio -41.9844021063 -> 0.0 mm | fin 69.1996468112 -> 69.1996468112 mm
--- list: ok=True en 4 ms | errores=- | avisos=-
    conexiones tras borrar: 0
=== fin 12-fase3-borrar ===


```

## 11 sondeo 12 borrar tras boton

```text
== 12-fase3-borrar.py -> HTTP 200 en 607 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 6 ms | errores=- | avisos=-
    conexiones en el modelo: 1
--- get: ok=True en 7 ms | errores=- | avisos=-
--- delete: ok=True en 66 ms | errores=- | avisos=-
    {"deleted_elements_count": 12, "restored_members_count": 3, "deleted_connection_id": "c0deed3c-ee5f-40e5-aa25-870047c24ab0"}
    miembro 1249636: extension inicio 0.0 -> 0.0 mm | fin -210.210261645 -> 0.0 mm
    miembro 1249630: extension inicio 0.0 -> 0.0 mm | fin -93.8379623742 -> 68.6404746190 mm
    miembro 1249631: extension inicio -41.9844021063 -> 0.0 mm | fin 69.1996468112 -> 69.1996468112 mm
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones tras borrar: 0
=== fin 12-fase3-borrar ===


```

## 12 log add-in

```text
{"ts":"2026-09-30T19:28:21.5703037-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:30:17.8900380-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-09-30T19:35:04.5251598-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6045938-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6059620-05:00","record":{"event":"handle","operation":"no_existe","request_summary":"{}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6081407-05:00","record":{"event":"handle","operation":"ping","request_summary":"esto no es json","ok":false,"error_codes":["INVALID_REQUEST"],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T19:35:34.4497643-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:34.4908316-05:00","record":{"event":"handle","operation":"no_existe","request_summary":"{}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:41:05.2426378-05:00","record":{"event":"shutdown"}}
{"ts":"2026-09-30T19:41:41.3538541-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:54:06.4529495-05:00","record":{"event":"shutdown"}}
{"ts":"2026-09-30T19:54:45.4481079-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:56:19.6737205-05:00","record":{"event":"handle","operation":"probe_plate_b","request_summary":"{\"element_ids\": [2390473, 2391296, 2391297, 2391299], \"chord_element_id\": 2390473}","ok":false,"error_codes":["ELEMENT_NOT_FOUND"],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T19:57:06.6580668-05:00","record":{"event":"handle","operation":"probe_plate_b","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":130}}
{"ts":"2026-09-30T19:58:24.9436829-05:00","record":{"event":"handle","operation":"probe_delete_b","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":42}}
{"ts":"2026-09-30T20:22:13.1565306-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T20:35:07.0242956-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T20:42:29.2475668-05:00","record":{"event":"shutdown"}}
{"ts":"2026-09-30T21:53:01.6106038-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T21:53:39.1289223-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":9}}
{"ts":"2026-09-30T21:54:00.0047157-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T21:54:52.2528069-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T21:54:52.2789794-05:00","record":{"event":"handle","operation":"guide","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":9}}
{"ts":"2026-09-30T21:54:52.2827850-05:00","record":{"event":"handle","operation":"types","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T21:54:52.2894315-05:00","record":{"event":"handle","operation":"schema","request_summary":"{\"type\": \"gusset_node\"}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T21:54:52.3281731-05:00","record":{"event":"handle","operation":"find_profile","request_summary":"{\"query\": \"HSS2-1/2X2-1/2X3/16\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":30}}
{"ts":"2026-09-30T21:54:52.3733058-05:00","record":{"event":"handle","operation":"node_info","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":28}}
{"ts":"2026-09-30T21:54:52.4974344-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":false,"error_codes":["MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE"],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":101}}
{"ts":"2026-09-30T21:54:52.5167782-05:00","record":{"event":"handle","operation":"preview","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":5}}
{"ts":"2026-09-30T21:55:33.0059231-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T21:55:33.0232263-05:00","record":{"event":"handle","operation":"guide","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:55:33.0241680-05:00","record":{"event":"handle","operation":"types","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:55:33.0264643-05:00","record":{"event":"handle","operation":"schema","request_summary":"{\"type\": \"gusset_node\"}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:55:33.0311121-05:00","record":{"event":"handle","operation":"find_profile","request_summary":"{\"query\": \"HSS2-1/2X2-1/2X3/16\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-09-30T21:55:33.0381182-05:00","record":{"event":"handle","operation":"node_info","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T21:55:33.0642569-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":false,"error_codes":["MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE"],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":11}}
{"ts":"2026-09-30T21:55:33.0747031-05:00","record":{"event":"handle","operation":"preview","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T21:58:04.7511520-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T21:58:04.7647470-05:00","record":{"event":"handle","operation":"guide","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:58:04.7658498-05:00","record":{"event":"handle","operation":"types","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:58:04.7679376-05:00","record":{"event":"handle","operation":"schema","request_summary":"{\"type\": \"gusset_node\"}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T21:58:04.7731084-05:00","record":{"event":"handle","operation":"find_profile","request_summary":"{\"query\": \"HSS2-1/2X2-1/2X3/16\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-09-30T21:58:04.7878198-05:00","record":{"event":"handle","operation":"node_info","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T21:58:04.8033583-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":false,"error_codes":["MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE","MEMBER_NOT_AT_NODE"],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":11}}
{"ts":"2026-09-30T21:58:04.8132126-05:00","record":{"event":"handle","operation":"preview","request_summary":"{\"spec\": {\"dimension_chains\": [{\"values_mm\": [75.0, 420.0, 70.0], \"expected_total_mm\": 565.0, \"label\": \"borde superior\"}, {\"values_mm\": [125.0, 90.0, 350.0], \"expected_total_mm\": 565.0, \"label\": \"base...","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T22:06:34.2381715-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T22:07:13.7811116-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":10}}
{"ts":"2026-09-30T22:07:59.2506584-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T22:07:59.2707811-05:00","record":{"event":"handle","operation":"guide","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-09-30T22:07:59.2743600-05:00","record":{"event":"handle","operation":"types","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T22:07:59.2912208-05:00","record":{"event":"handle","operation":"schema","request_summary":"{\"type\": \"gusset_node\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T22:07:59.3226315-05:00","record":{"event":"handle","operation":"find_profile","request_summary":"{\"query\": \"HSS2-1/2X2-1/2X3/16\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":23}}
{"ts":"2026-09-30T22:07:59.3620764-05:00","record":{"event":"handle","operation":"node_info","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":28}}
{"ts":"2026-09-30T22:07:59.4738678-05:00","record":{"event":"handle","operation":"validate","request_summary":"{\"spec\": {\"node\": {\"element_ids\": [1249510, 1249630, 1249631, 1249636]}, \"source\": {\"drawing\": \"Detalle D\", \"scale\": \"1/10\"}, \"members\": [{\"element_id\": 1249630, \"profile\": \"HSS2-1/2X2-1/2X3/16\", \"end...","ok":true,"error_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":91}}
{"ts":"2026-09-30T22:07:59.4966968-05:00","record":{"event":"handle","operation":"preview","request_summary":"{\"spec\": {\"node\": {\"element_ids\": [1249510, 1249630, 1249631, 1249636]}, \"source\": {\"drawing\": \"Detalle D\", \"scale\": \"1/10\"}, \"members\": [{\"element_id\": 1249630, \"profile\": \"HSS2-1/2X2-1/2X3/16\", \"end...","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":5}}
{"ts":"2026-09-30T22:07:59.5294053-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: crear 70234718-7dde-47b5-baf6-32f8a1e7fd09","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-09-30T22:08:00.1078041-05:00","record":{"event":"advance_steel_bolts","name":"Pernos miembro 1249636","properties":["Nx=2","Ny=2","Dx=0.19685039370078738","Dy=0.19685039370078738","ScrewDiameter=0.05208333333333333","ScrewLength=0.14763779527559054"],"element_ids":[]}}
{"ts":"2026-09-30T22:08:00.6326429-05:00","record":{"event":"handle","operation":"create","request_summary":"{\"spec\": {\"node\": {\"element_ids\": [1249510, 1249630, 1249631, 1249636]}, \"source\": {\"drawing\": \"Detalle D\", \"scale\": \"1/10\"}, \"members\": [{\"element_id\": 1249630, \"profile\": \"HSS2-1/2X2-1/2X3/16\", \"end...","ok":true,"error_codes":[],"warning_codes":["REVIT_WARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING"],"duration_ms":1124}}
{"ts":"2026-09-30T22:08:00.6443240-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":7}}
{"ts":"2026-09-30T22:08:00.6527581-05:00","record":{"event":"handle","operation":"get","request_summary":"{\"connection_id\": \"70234718-7dde-47b5-baf6-32f8a1e7fd09\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":6}}
{"ts":"2026-09-30T22:12:11.2308497-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T22:12:11.2528299-05:00","record":{"event":"handle","operation":"get","request_summary":"{\"connection_id\": \"70234718-7dde-47b5-baf6-32f8a1e7fd09\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-09-30T22:12:11.2721249-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: borrar 70234718-7dde-47b5-baf6-32f8a1e7fd09","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-09-30T22:12:11.3410262-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"70234718-7dde-47b5-baf6-32f8a1e7fd09\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":80}}
{"ts":"2026-09-30T22:12:11.3482832-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T22:17:11.6749236-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: crear c0deed3c-ee5f-40e5-aa25-870047c24ab0","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-09-30T22:17:11.7353608-05:00","record":{"event":"advance_steel_bolts","name":"Pernos miembro 1249636","properties":["Nx=2","Ny=2","Dx=0.19685039370078738","Dy=0.19685039370078738","ScrewDiameter=0.05208333333333333","ScrewLength=0.14763779527559054"],"element_ids":[]}}
{"ts":"2026-09-30T22:19:46.8424803-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T22:21:07.9479743-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T22:21:07.9748508-05:00","record":{"event":"handle","operation":"get","request_summary":"{\"connection_id\": \"c0deed3c-ee5f-40e5-aa25-870047c24ab0\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T22:21:07.9909894-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: borrar c0deed3c-ee5f-40e5-aa25-870047c24ab0","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-09-30T22:21:08.0437158-05:00","record":{"event":"handle","operation":"delete","request_summary":"{\"connection_id\": \"c0deed3c-ee5f-40e5-aa25-870047c24ab0\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":65}}
{"ts":"2026-09-30T22:21:08.0503776-05:00","record":{"event":"handle","operation":"list","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":4}}

```
