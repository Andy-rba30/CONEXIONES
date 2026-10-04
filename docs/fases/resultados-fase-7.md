# Resultados de la Fase 7

Fecha: 2026-10-04T14:12:49


## 7-1 git

```text
1de3d7f Fase 7: paso 7-0 (fusionar la rama en main) para el instalador y ronda 6d a continuación

```

## 7-2 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.21 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.21 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.21 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\CatalogTests.cs(132,13): warning xUnit2013: Do not use Assert.Equal() to check for collection size. Use Assert.Empty instead. (https://xunit.net/xunit.analyzers/rules/xUnit2013) [D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj]
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.

D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\CatalogTests.cs(132,13): warning xUnit2013: Do not use Assert.Equal() to check for collection size. Use Assert.Empty instead. (https://xunit.net/xunit.analyzers/rules/xUnit2013) [D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj]
    1 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:08.26
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   129, Omitido:     0, Total:   129, Duración: 223 ms - MotorConexiones.Tests.dll (net10.0)

```

## 7-2 revit cerrado

```text

```

## 7-2 deploy

```text
.\scripts\deploy.ps1 : No se puede cargar el archivo D:\Proyectos C#\CONEXIONES\scripts\deploy.ps1 porque la ejecución 
de scripts está deshabilitada en este sistema. Para obtener más información, consulta el tema about_Execution_Policies 
en https:/go.microsoft.com/fwlink/?LinkID=135170.
En línea: 12 Carácter: 22
+ Anota "7-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
+                      ~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : SecurityError: (:) [], PSSecurityException
    + FullyQualifiedErrorId : UnauthorizedAccess

```

## 7-2 instalar-conn

```text
.\mcp\instalar-conn.ps1 : No se puede cargar el archivo D:\Proyectos C#\CONEXIONES\mcp\instalar-conn.ps1 porque la 
ejecución de scripts está deshabilitada en este sistema. Para obtener más información, consulta el tema 
about_Execution_Policies en https:/go.microsoft.com/fwlink/?LinkID=135170.
En línea: 13 Carácter: 29
+ Anota "7-2 instalar-conn" { .\mcp\instalar-conn.ps1 }
+                             ~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : SecurityError: (:) [], PSSecurityException
    + FullyQualifiedErrorId : UnauthorizedAccess

```

## 7-2 catalog.json desplegado

```text
Get-Content : No se encuentra la ruta de acceso 'C:\Users\Andy Bayona 
Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json' porque no existe.
En línea: 14 Carácter: 39
+ ... splegado" { Get-Content -Encoding UTF8 "$env:APPDATA\Autodesk\Revit\A ...
+                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (C:\Users\Andy B...ig\catalog.json:String) [Get-Content], ItemNotFoundEx 
   ception
    + FullyQualifiedErrorId : PathNotFound,Microsoft.PowerShell.Commands.GetContentCommand
 

```

## 7-2 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:01.91
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   129, Omitido:     0, Total:   129, Duración: 184 ms - MotorConexiones.Tests.dll (net10.0)

```

## 7-2 revit cerrado

```text

```

## 7-2 deploy

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 7-2 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (20 rutas @api.route)
- copiado tools\conn_tools.py (18 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 7-2 catalog.json desplegado

```text
{
  "_comentario": "Catálogo de plantillas (Fase 7). Editable sin recompilar: scripts\\deploy.ps1 lo copia junto al add-in. catalog_folder admite %LOCALAPPDATA%; shared_catalog_folder es la copia opcional (catalog\\ del repositorio o una carpeta de red): deploy.ps1 copia a la carpeta del usuario las plantillas que falten y 'Guardar en catálogo' ofrece copiar ahí. Las claves node_* quedan reservadas para la Fase 8.",
  "schema_version": 1,
  "catalog_folder": "%LOCALAPPDATA%\\MotorConexiones\\catalogo",
  "shared_catalog_folder": "D:\\Proyectos C#\\CONEXIONES\\catalog",
  "angle_tolerance_deg": 10.0,
  "angle_deviation_warning_deg": 5.0,
  "allow_mirror": true,
  "default_profile_policy": "warn",
  "node_cluster_mm": 10.0,
  "node_axis_max_distance_mm": 5.0
}

```

## 7-3 ping

```text
== conn/ping -> HTTP 200 en 322 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "document":  {
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "is_workshared":  false,
                                  "is_read_only":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
                              },
                 "addin_version":  "0.1.0",
                 "has_uidocument":  true,
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "version_number":  "2027"
                           },
                 "operations":  [
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
                 "dotnet":  {
                                "load_context":  "Default",
                                "framework":  ".NET 10.0.12",
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
                            },
                 "backend":  "advancesteel",
                 "spec_version":  "1.0"
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "ping",
                 "duration_ms":  8
             }
}

```

## 7-3 list

```text
== conn/list -> HTTP 200 en 39 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "connections":  [

                                 ],
                 "connections_count":  0
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "list",
                 "duration_ms":  8
             }
}

```

## 7-3 node_info

```text
== conn/node_info -> HTTP 200 en 457 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "chord_element_id":  1249510,
                 "existing_connections":  [

                                          ],
                 "y_axis":  [
                                0,
                                0,
                                1
                            ],
                 "origin_mm":  [
                                   -11867.700000000001,
                                   -17195.799999999999,
                                   17423
                               ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "members":  [
                                 {
                                     "end_mm":  [
                                                    -14397.600000000000,
                                                    -17195.799999999999,
                                                    17423
                                                ],
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "start_mm":  [
                                                      -4437.3000000000002,
                                                      -17195.700000000001,
                                                      17423
                                                  ],
                                     "family":  "HSS-Hollow Structural Section",
                                     "length_mm":  9960.2999999999993,
                                     "side":  "chord",
                                     "is_chord":  true,
                                     "structural_type":  "Beam",
                                     "node_end":  1,
                                     "slope_deg":  0,
                                     "type":  "HSS3X3X1/4",
                                     "angle_in_plane_deg":  0,
                                     "element_id":  1249510,
                                     "angle_to_chord_deg":  0
                                 },
                                 {
                                     "end_mm":  [
                                                    -11930.600000000000,
                                                    -17195.799999999999,
                                                    17481.799999999999
                                                ],
                                     "material":  "Material IFC (190-40-140)",
                                     "start_mm":  [
                                                      -14536.799999999999,
                                                      -17195.799999999999,
                                                      19918.799999999999
                                                  ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3568,
                                     "side":  "+Y",
                                     "is_chord":  false,
                                     "structural_type":  "Beam",
                                     "node_end":  1,
                                     "slope_deg":  43.079999999999998,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_in_plane_deg":  136.90000000000001,
                                     "element_id":  1249630,
                                     "angle_to_chord_deg":  43.100000000000001
                                 },
                                 {
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "material":  "Material IFC (190-40-140)",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3500,
                                     "side":  "+Y",
                                     "is_chord":  false,
                                     "structural_type":  "Beam",
                                     "node_end":  0,
                                     "slope_deg":  44.369999999999997,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_in_plane_deg":  44.399999999999999,
                                     "element_id":  1249631,
                                     "angle_to_chord_deg":  44.399999999999999
                                 },
                                 {
                                     "end_mm":  [
                                                    -11904.900000000000,
                                                    -17195.700000000001,
                                                    17389.900000000001
                                                ],
                                     "material":  "Material IFC (190-40-140)",
                                     "start_mm":  [
                                                      -14455.299999999999,
                                                      -17195.700000000001,
                                                      14894.700000000001
                                                  ],
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3567.9000000000001,
                                     "side":  "-Y",
                                     "is_chord":  false,
                                     "structural_type":  "Beam",
                                     "node_end":  1,
                                     "slope_deg":  44.369999999999997,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "element_id":  1249636,
                                     "angle_to_chord_deg":  44.399999999999999
                                 }
                             ],
                 "x_axis":  [
                                1,
                                2E-06,
                                0
                            ],
                 "z_axis":  [
                                2E-06,
                                -1,
                                0
                            ],
                 "axis_distance_mm":  0.080000000000000002,
                 "chord_direction_reversed":  true
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "node_info",
                 "duration_ms":  27
             }
}

```

## 7-3 catalog_list vacio

```text
== conn/catalog_list -> HTTP 200 en 27 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates":  [

                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  0
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_list",
                 "duration_ms":  10
             }
}

```

## 6d-3 sondeo 16

```text
== 16-pernos-agarre.py -> HTTP 200 en 1859 ms ==
=== 16-pernos-agarre ===
Bridge.Handle encontrado: True | version del ensamblado: 0.1.0.0
1) conexiones en el modelo: 1
   connection_id=ebb2f171-5470-432d-b828-6aea655484b3 | backend=advancesteel | elementos creados=9
   cartela 9.525 mm | placa cuchilla 10.0 mm | cara +z | agarre esperado 19.53 mm | paquete Z esperado -4.76 .. 14.76
2) origen (-11867.700000000001, -17195.799999999999, 17423.0) | Z local (normal a la cercha) = (0.0, -1.0, 0.0)
   validate bolt_stacks: [{'bolt_length_mm': 44.450000000000003, 'gusset_face': '+z', 'grip_mm': 19.524999999999999, 'length_source': 'computed_from_grip', 'member_element_id': 1249636}]
3) intervalo Z local (mm) de cada elemento creado:
   [1321345] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321346] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321347] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321348] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321349] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321350] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321351] SteelProxyElement | Plates: z -4.76 .. 4.76 mm (espesor en Z 9.53; 1 solidos, 48 vertices)
        Thickness: 0' - 0 3/8" = 9.53 mm
        Length: 1' - 10 1/4" = 565.0 mm
        Width: 1' - 8 7/8" = 530.0 mm
   [1321352] SteelProxyElement | Plates: z 4.76 .. 14.76 mm (espesor en Z 10.00; 1 solidos, 24 vertices)
        Thickness: 0' - 0 13/32" = 10.0 mm
        Length: 0' - 6 11/16" = 170.0 mm
        Width: 0' - 5 1/2" = 140.0 mm
   [1321353] SteelProxyElement | Bolts: z -29.69 .. 24.69 mm (espesor en Z 54.37; 20 solidos, 496 vertices)
        Diameter:  5/8 inch
        Bolt Length: 0' - 1 3/4" = 44.45 mm
        Grip Length: 0' - 0 25/32" = 19.52 mm
        Number on side 1: 2
        Number on side 2: 2
4) veredicto:
   cartela [1321351]: centrada en el plano de la cercha (-4.76 .. 4.76) OK
   placa cuchilla [1321352]: 4.76 .. 14.76 -> apoya en la cara +z de la cartela OK
   pernos [1321353]: z -29.69 .. 24.69 (largo total 54.37 mm) -> atraviesan cartela + placa OK; sobresalen 24.9 mm por abajo y 9.9 mm por arriba
5) captura de perfil: no se pudo (). Haz la captura a mano (paso de la persona).
=== fin 16-pernos-agarre ===


```

## 7-5 catalog_save

```text
== conn/catalog_save -> HTTP 200 en 505 ms ==
{
    "errors":  [

               ],
    "warnings":  [
                     {
                         "path":  "members[0].expected_angle_deg",
                         "message":  "El ángulo del plano (45.0°, inclinación 45.0° respecto al cordón) difiere del de la barra en el modelo (136.9°, inclinación 43.1°) por 1.9° > 1°.",
                         "code":  "ANGLE_DIFFERS_FROM_MODEL",
                         "hint":  "Verifica la geometría en el modelo o en el plano."
                     },
                     {
                         "path":  "members[1].expected_angle_deg",
                         "message":  "El ángulo del plano (90.0°, inclinación 90.0° respecto al cordón) difiere del de la barra en el modelo (44.4°, inclinación 44.4°) por 45.6° > 1°.",
                         "code":  "ANGLE_DIFFERS_FROM_MODEL",
                         "hint":  "Verifica la geometría en el modelo o en el plano."
                     }
                 ],
    "data":  {
                 "shared_file":  "D:\\Proyectos C#\\CONEXIONES\\catalog\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                 "origin":  {
                                "document":  "HANGAR_PRUEBA_sondeo",
                                "connection_id":  "ebb2f171-5470-432d-b828-6aea655484b3",
                                "element_ids":  [
                                                    1249510,
                                                    1249630,
                                                    1249631,
                                                    1249636
                                                ],
                                "drawing":  "Detalle D"
                            },
                 "chord_profile":  "HSS3X3X1/4",
                 "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                 "matching":  {
                                  "angle_tolerance_deg":  10,
                                  "allow_mirror":  true
                              },
                 "members_count":  3,
                 "member_pattern":  [
                                        {
                                            "profile_policy":  "warn",
                                            "side":  "+Y",
                                            "role":  "diagonal",
                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                            "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                            "angle_deg":  136.91999999999999,
                                            "slot":  0
                                        },
                                        {
                                            "profile_policy":  "warn",
                                            "side":  "+Y",
                                            "role":  "vertical",
                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                            "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                            "angle_deg":  44.369999999999997,
                                            "slot":  1
                                        },
                                        {
                                            "profile_policy":  "warn",
                                            "side":  "-Y",
                                            "role":  "diagonal",
                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                            "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                            "angle_deg":  -135.63000000000000,
                                            "slot":  2
                                        }
                                    ],
                 "name":  "Nudo tipico Detalle D",
                 "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_save",
                 "duration_ms":  59
             }
}

```

## 7-5 catalog_list

```text
== conn/catalog_list -> HTTP 200 en 36 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates":  [
                                   {
                                       "chord_profile":  "HSS3X3X1/4",
                                       "connection_type":  "gusset_node",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · vertical 44,4° +Y · diagonal -135,6° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "members_count":  3,
                                       "created_utc":  "2026-10-04T19:21:59.5163425Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "origin_drawing":  "Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  1
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_list",
                 "duration_ms":  6
             }
}

```

## 7-5 carpeta del catalogo

```text

Name                                      Length
----                                      ------
6abcf116-9b97-485f-b50d-2851ca0018cc.json   5258
6abcf116-9b97-485f-b50d-2851ca0018cc.json   5258



```

## 7-5 catalog_get

```text
== conn/catalog_get -> HTTP 200 en 445 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "template":  {
                                  "origin":  {
                                                 "document":  "HANGAR_PRUEBA_sondeo",
                                                 "connection_id":  "ebb2f171-5470-432d-b828-6aea655484b3",
                                                 "element_ids":  [
                                                                     1249510,
                                                                     1249630,
                                                                     1249631,
                                                                     1249636
                                                                 ],
                                                 "drawing":  "Detalle D"
                                             },
                                  "chord_pattern":  {
                                                        "profile_policy":  "warn",
                                                        "profile":  "HSS3X3X1/4",
                                                        "continuous":  true
                                                    },
                                  "spec_template":  {
                                                        "chord":  {
                                                                      "profile":  "HSS3X3X1/4",
                                                                      "continuous":  true
                                                                  },
                                                        "uncertain_fields":  [

                                                                             ],
                                                        "connection_type":  "gusset_node",
                                                        "gusset":  {
                                                                       "height_mm":  530.0,
                                                                       "chord_interface":  "through_slot",
                                                                       "thickness_label":  "3/8\"",
                                                                       "outline":  {
                                                                                       "mode":  "polygon",
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
                                                                                                     ]
                                                                                   },
                                                                       "weld_to_chord":  {
                                                                                             "size_mm":  5.0,
                                                                                             "type":  "fillet",
                                                                                             "all_around":  true
                                                                                         },
                                                                       "thickness_mm":  9.5250000000000004,
                                                                       "width_mm":  565.0
                                                                   },
                                                        "members":  [
                                                                        {
                                                                            "expected_angle_deg":  45.0,
                                                                            "role":  "diagonal",
                                                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                                                            "end_setback_mm":  180.0,
                                                                            "attachment":  {
                                                                                               "type":  "welded_slot",
                                                                                               "slot_length_mm":  150.0,
                                                                                               "weld":  {
                                                                                                            "size_mm":  5.0,
                                                                                                            "type":  "fillet",
                                                                                                            "all_around":  true
                                                                                                        }
                                                                                           },
                                                                            "slot":  0
                                                                        },
                                                                        {
                                                                            "expected_angle_deg":  90.0,
                                                                            "role":  "vertical",
                                                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                                                            "end_setback_mm":  60.0,
                                                                            "attachment":  {
                                                                                               "type":  "welded_slot",
                                                                                               "slot_length_mm":  150.0,
                                                                                               "weld":  {
                                                                                                            "size_mm":  5.0,
                                                                                                            "type":  "fillet",
                                                                                                            "all_around":  true
                                                                                                        }
                                                                                           },
                                                                            "slot":  1
                                                                        },
                                                                        {
                                                                            "expected_angle_deg":  45.0,
                                                                            "role":  "diagonal",
                                                                            "profile":  "HSS2-1/2X2-1/2X3/16",
                                                                            "end_setback_mm":  260.0,
                                                                            "attachment":  {
                                                                                               "type":  "bolted_knife_plate",
                                                                                               "bolts":  {
                                                                                                             "diameter_label":  "5/8\"",
                                                                                                             "edge_mm":  40.0,
                                                                                                             "first_row_from_plate_end_mm":  40.0,
                                                                                                             "spacing_mm":  60.0,
                                                                                                             "diameter_mm":  15.875,
                                                                                                             "rows":  2,
                                                                                                             "columns":  2
                                                                                                         },
                                                                                               "plate":  {
                                                                                                             "length_mm":  170.0,
                                                                                                             "thickness_label":  "PL10",
                                                                                                             "insertion_mm":  80.0,
                                                                                                             "thickness_mm":  10.0,
                                                                                                             "width_mm":  140.0
                                                                                                         },
                                                                                               "weld_plate_to_member":  {
                                                                                                                            "size_mm":  5.0,
                                                                                                                            "type":  "fillet",
                                                                                                                            "all_around":  true
                                                                                                                        }
                                                                                           },
                                                                            "slot":  2
                                                                        }
                                                                    ],
                                                        "source":  {
                                                                       "scale":  "1/10",
                                                                       "drawing":  "Detalle D"
                                                                   },
                                                        "dimension_chains":  [
                                                                                 {
                                                                                     "expected_total_mm":  565.0,
                                                                                     "values_mm":  [
                                                                                                       75.0,
                                                                                                       420.0,
                                                                                                       70.0
                                                                                                   ],
                                                                                     "label":  "borde superior"
                                                                                 },
                                                                                 {
                                                                                     "expected_total_mm":  565.0,
                                                                                     "values_mm":  [
                                                                                                       125.0,
                                                                                                       90.0,
                                                                                                       350.0
                                                                                                   ],
                                                                                     "label":  "base"
                                                                                 },
                                                                                 {
                                                                                     "expected_total_mm":  530.0,
                                                                                     "values_mm":  [
                                                                                                       70.0,
                                                                                                       250.0,
                                                                                                       210.0
                                                                                                   ],
                                                                                     "label":  "lado derecho"
                                                                                 },
                                                                                 {
                                                                                     "expected_total_mm":  530.0,
                                                                                     "values_mm":  [
                                                                                                       70.0,
                                                                                                       325.0,
                                                                                                       135.0
                                                                                                   ],
                                                                                     "label":  "lado izquierdo"
                                                                                 }
                                                                             ],
                                                        "spec_version":  "1.0"
                                                    },
                                  "connection_type":  "gusset_node",
                                  "matching":  {
                                                   "angle_tolerance_deg":  10,
                                                   "allow_mirror":  true
                                               },
                                  "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                  "tags":  [
                                               "hangar",
                                               "cercha",
                                               "HSS"
                                           ],
                                  "created_utc":  "2026-10-04T19:21:59.5163425Z",
                                  "member_pattern":  [
                                                         {
                                                             "profile_policy":  "warn",
                                                             "side":  "+Y",
                                                             "role":  "diagonal",
                                                             "profile":  "HSS2-1/2X2-1/2X3/16",
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "angle_deg":  136.91999999999999,
                                                             "slot":  0
                                                         },
                                                         {
                                                             "profile_policy":  "warn",
                                                             "side":  "+Y",
                                                             "role":  "vertical",
                                                             "profile":  "HSS2-1/2X2-1/2X3/16",
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "angle_deg":  44.369999999999997,
                                                             "slot":  1
                                                         },
                                                         {
                                                             "profile_policy":  "warn",
                                                             "side":  "-Y",
                                                             "role":  "diagonal",
                                                             "profile":  "HSS2-1/2X2-1/2X3/16",
                                                             "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                             "angle_deg":  -135.63000000000000,
                                                             "slot":  2
                                                         }
                                                     ],
                                  "name":  "Nudo tipico Detalle D",
                                  "catalog_version":  "1.0",
                                  "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                              },
                 "pattern":  "3 barra(s): diagonal 136,9° +Y · vertical 44,4° +Y · diagonal -135,6° -Y",
                 "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                 "name":  "Nudo tipico Detalle D",
                 "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_get",
                 "duration_ms":  1
             }
}

```

## 7-5 catalog_apply mismo nudo

```text
== conn/catalog_apply -> HTTP 200 en 98 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "calculated_values":  {
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
                                           "axis_distance_mm":  0.080000000000000002,
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ],
                                           "chord_direction_reversed":  true
                                       },
                 "errors_count":  0,
                 "node":  {
                              "element_ids":  [
                                                  1249510,
                                                  1249630,
                                                  1249631,
                                                  1249636
                                              ],
                              "chord_direction_reversed":  true,
                              "chord_element_id":  1249510
                          },
                 "spec":  {
                              "chord":  {
                                            "element_id":  1249510,
                                            "profile":  "HSS3X3X1/4",
                                            "continuous":  true
                                        },
                              "uncertain_fields":  [

                                                   ],
                              "node":  {
                                           "element_ids":  [
                                                               1249510,
                                                               1249630,
                                                               1249631,
                                                               1249636
                                                           ]
                                       },
                              "connection_type":  "gusset_node",
                              "gusset":  {
                                             "height_mm":  530.0,
                                             "chord_interface":  "through_slot",
                                             "thickness_label":  "3/8\"",
                                             "outline":  {
                                                             "mode":  "polygon",
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
                                                                           ]
                                                         },
                                             "weld_to_chord":  {
                                                                   "size_mm":  5.0,
                                                                   "type":  "fillet",
                                                                   "all_around":  true
                                                               },
                                             "thickness_mm":  9.5250000000000004,
                                             "width_mm":  565.0
                                         },
                              "members":  [
                                              {
                                                  "expected_angle_deg":  43.100000000000001,
                                                  "role":  "diagonal",
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "end_setback_mm":  180.0,
                                                  "attachment":  {
                                                                     "type":  "welded_slot",
                                                                     "slot_length_mm":  150.0,
                                                                     "weld":  {
                                                                                  "size_mm":  5.0,
                                                                                  "type":  "fillet",
                                                                                  "all_around":  true
                                                                              }
                                                                 },
                                                  "element_id":  1249630
                                              },
                                              {
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "role":  "vertical",
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "end_setback_mm":  60.0,
                                                  "attachment":  {
                                                                     "type":  "welded_slot",
                                                                     "slot_length_mm":  150.0,
                                                                     "weld":  {
                                                                                  "size_mm":  5.0,
                                                                                  "type":  "fillet",
                                                                                  "all_around":  true
                                                                              }
                                                                 },
                                                  "element_id":  1249631
                                              },
                                              {
                                                  "expected_angle_deg":  44.399999999999999,
                                                  "role":  "diagonal",
                                                  "profile":  "HSS2-1/2X2-1/2X3/16",
                                                  "end_setback_mm":  260.0,
                                                  "attachment":  {
                                                                     "type":  "bolted_knife_plate",
                                                                     "bolts":  {
                                                                                   "diameter_label":  "5/8\"",
                                                                                   "edge_mm":  40.0,
                                                                                   "first_row_from_plate_end_mm":  40.0,
                                                                                   "spacing_mm":  60.0,
                                                                                   "diameter_mm":  15.875,
                                                                                   "rows":  2,
                                                                                   "columns":  2
                                                                               },
                                                                     "plate":  {
                                                                                   "length_mm":  170.0,
                                                                                   "thickness_label":  "PL10",
                                                                                   "insertion_mm":  80.0,
                                                                                   "thickness_mm":  10.0,
                                                                                   "width_mm":  140.0
                                                                               },
                                                                     "weld_plate_to_member":  {
                                                                                                  "size_mm":  5.0,
                                                                                                  "type":  "fillet",
                                                                                                  "all_around":  true
                                                                                              }
                                                                 },
                                                  "element_id":  1249636
                                              }
                                          ],
                              "source":  {
                                             "scale":  "1/10",
                                             "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                             "drawing":  "Detalle D"
                                         },
                              "dimension_chains":  [
                                                       {
                                                           "expected_total_mm":  565.0,
                                                           "values_mm":  [
                                                                             75.0,
                                                                             420.0,
                                                                             70.0
                                                                         ],
                                                           "label":  "borde superior"
                                                       },
                                                       {
                                                           "expected_total_mm":  565.0,
                                                           "values_mm":  [
                                                                             125.0,
                                                                             90.0,
                                                                             350.0
                                                                         ],
                                                           "label":  "base"
                                                       },
                                                       {
                                                           "expected_total_mm":  530.0,
                                                           "values_mm":  [
                                                                             70.0,
                                                                             250.0,
                                                                             210.0
                                                                         ],
                                                           "label":  "lado derecho"
                                                       },
                                                       {
                                                           "expected_total_mm":  530.0,
                                                           "values_mm":  [
                                                                             70.0,
                                                                             325.0,
                                                                             135.0
                                                                         ],
                                                           "label":  "lado izquierdo"
                                                       }
                                                   ],
                              "spec_version":  "1.0"
                          },
                 "name":  "Nudo tipico Detalle D",
                 "is_valid":  true,
                 "bolt_stacks":  [
                                     {
                                         "bolt_length_mm":  44.450000000000003,
                                         "gusset_face":  "+z",
                                         "grip_mm":  19.524999999999999,
                                         "length_source":  "computed_from_grip",
                                         "member_element_id":  1249636
                                     }
                                 ],
                 "validation_token":  "855ede381e0e13a3cedbe44f4b367c6eeab16057b78a0dcfe2879b116e1340a3",
                 "match":  {
                               "max_deviation_deg":  0,
                               "unmatched_slots":  [

                                                   ],
                               "unassigned_members":  [

                                                      ],
                               "assignments":  [
                                                   {
                                                       "profile_policy":  "warn",
                                                       "side":  "+Y",
                                                       "role":  "diagonal",
                                                       "deviation_deg":  0,
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "model_angle_deg":  136.91999999999999,
                                                       "template_angle_deg":  136.91999999999999,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "element_id":  1249630,
                                                       "slot":  0
                                                   },
                                                   {
                                                       "profile_policy":  "warn",
                                                       "side":  "+Y",
                                                       "role":  "vertical",
                                                       "deviation_deg":  0,
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "model_angle_deg":  44.369999999999997,
                                                       "template_angle_deg":  44.369999999999997,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "element_id":  1249631,
                                                       "slot":  1
                                                   },
                                                   {
                                                       "profile_policy":  "warn",
                                                       "side":  "-Y",
                                                       "role":  "diagonal",
                                                       "deviation_deg":  0,
                                                       "template_profile":  "HSS2-1/2X2-1/2X3/16",
                                                       "model_angle_deg":  -135.63000000000000,
                                                       "template_angle_deg":  -135.63000000000000,
                                                       "model_type_name":  "HSS2-1-2X2-1-2X3-16 64x64",
                                                       "element_id":  1249636,
                                                       "slot":  2
                                                   }
                                               ],
                               "orientation":  "same",
                               "description":  "same: ranura 0 (diagonal 136.9°) → barra 1249630 (136.9°, desvío 0.0°); ranura 1 (vertical 44.4°) → barra 1249631 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1249636 (-135.6°, desvío 0.0°)",
                               "is_complete":  true,
                               "matched_count":  3,
                               "score_deg":  0.01
                           },
                 "warnings_count":  0,
                 "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_apply",
                 "duration_ms":  58
             }
}

```

## 7-5 delete conexion

```text
== conn/delete -> HTTP 200 en 170 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "deleted_elements_count":  9,
                 "restored_members_count":  3,
                 "deleted_connection_id":  "ebb2f171-5470-432d-b828-6aea655484b3"
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "delete",
                 "duration_ms":  150
             }
}

```

## 7-5 conn_list

```text
== conn/list -> HTTP 200 en 835 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "connections":  [

                                 ],
                 "connections_count":  0
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "list",
                 "duration_ms":  6
             }
}

```

## 7-8 puente en 8000

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         14928



```

## 7-8 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.1.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"errors":[],"warnings":[],"data":{"document":{"title":"HANGAR_PRUEBA_sondeo","is_family":false,"is_modifiable":false,"is_workshared":false,"is_read_only":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"},"addin_version":"0.1.0","has_uidocument":true,"revit":{"version_name":"Autodesk Revit 2027","language":"English_USA","sub_version_number":"2027.2","version_build":"27.2.0.39","version_number":"2027"},"operations":["catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"dotnet":{"load_context":"Default","framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"},"backend":"advancesteel","spec_version":"1.0"},"ok":true,"meta":{"addin_version":"0.1.0","operation":"ping","duration_ms":2}}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 12839 caracteres
Cuerpo:
{"errors":[],"warnings":[],"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si de ...
======================================================================
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
Cuerpo:
{"errors":[],"warnings":[],"data":{"connection_types":[{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"ok":true,"meta":{"addin_version":"0.1.0","operation":"types","duration_ms":1}}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"errors":[],"warnings":[],"data":{"example":{"chord":{"element_id":1249510,"profile":"HSS3X3X1/4","continuous":true},"uncertain_fields":[],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"connection_type":"gusset_node","gusset":{"height_mm":530.0,"chord_interface":"through_slot","thickness_label":"3/8\"","outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"thickness_mm":9.5250000000000004,"width_mm":565.0},"members":[{"expected_angle_deg":45.0,"role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"element_id":1249630}],"source":{"scale":"1/10","drawing":"Detalle D"},"dimension_chains":[{"expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0],"label":"borde superior"}],"spec_version":"1.0"},"json_schema":{"title":"GussetNodeConnectionSpec","properties":{"chord":{"required":["element_id","continuous"],"type":"object","additionalProperties":false,"properties":{"element_id":{"type":"integer"},"profile":{"type":["string","null"]},"continuous":{"type":"boolean"}}},"uncertain_fields":{"type":"array","items":{"required":["path","reason"],"type":"object","additionalProperties":false,"properties":{"path":{"type":"string"},"reason":{"type":"string"},"user_confirmed_value" ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"path":"type","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","code":"UNKNOWN_OPERATION","hint":"Tipos disponibles: gusset_node"}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"schema","duration_ms":11}}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"errors": [], "warnings": [], "data": {"suggestions": [], "query": "HSS2-1/2X2-1/2X3/16", "matched_count": 1, "matches": [{"exact_match": false, "type_name": "HSS2-1-2X2-1-2X3-16 64x64", "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "total_profiles_in_model": 29}, "ok": true, "meta": {"addin_version": "0.1.0", "operation": "find_profile", "duration_ms": 8}}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cord�n=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423]
Cuerpo:
{"errors": [], "warnings": [], "data": {"chord_element_id": 1249510, "existing_connections": [{"created_elements_count": 9, "connection_id": "c3c7373c-6c68-49be-bfbf-9ead1e12b279", "connection_type": "gusset_node", "created_utc": "2026-10-04T19:27:42.5751045Z"}], "y_axis": [0, 0, 1], "origin_mm": [-11867.700000000001, -17195.799999999999, 17423], "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)", "members": [{"end_mm": [-14397.600000000000, -17195.799999999999, 17423], "material": "Steel ASTM A500, Grade B, Rectangular and Square", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "family": "HSS-Hollow Structural Section", "length_mm": 9960.2999999999993, "side": "chord", "is_chord": true, "structural_type": "Beam", "node_end": 1, "slope_deg": 0, "type": "HSS3X3X1/4", "angle_in_plane_deg": 0, "element_id": 1249510, "angle_to_chord_deg": 0}, {"end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "material": "Material IFC (190-40-140)", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "family": "HSS2-1-2X2-1-2X3-16 64x64", "length_mm": 3568, "side": "+Y", "is_chord": false, "structural_type": "Beam", "node_end": 1, "slope_deg": 43.079999999999998, "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle_in_plane_deg": 136.90000000000001, "element_id": 1249630, "angle_to_chord_deg": 43.100000000000001}, {"end_mm": [-9354.7000000000007, -17195.799999999999,  ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=bca1ce7b8e68...
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"members[1].expected_angle_deg","message":"El \u00e1ngulo del plano (90.0\u00b0, inclinaci\u00f3n 90.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (44.4\u00b0, inclinaci\u00f3n 44.4\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"data":{"errors_count":0,"is_valid":true,"bolt_stacks":[{"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip","member_element_id":1249636}],"validation_token":"bca1ce7b8e68625d1ff025c5ce450d8cdea333024c272919e6c26fb4d7782ca8","warnings_count":2,"calculated_values":{"frame_z":[0,-1,0],"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"frame_x":[1,0,0],"chord_direction_reversed":true}},"ok":true,"meta":{"addin_version":"0.1.0","operation":"validate","duration_ms":14}}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"path":"dimension_chains[0].values_mm","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","code":"DIMENSION_CHAIN_MISMATCH","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm."}],"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"members[1].expected_angle_deg","message":"El \u00e1ngulo del plano (90.0\u00b0, inclinaci\u00f3n 90.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (44.4\u00b0, inclinaci\u00f3n 44.4\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"validate","duration_ms":10}}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"errors":[{"path":"uncertain_fields[0].user_confirmed_value","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."},{"path":"uncertain_fields[1].user_confirmed_value","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","code":"UNRESOLVED_UNCERTAINTY","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar."}],"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"members[1].expected_angle_deg","message":"El \u00e1ngulo del plano (90.0\u00b0, inclinaci\u00f3n 90.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (44.4\u00b0, inclinaci\u00f3n 44.4\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"validate","duration_ms":8}}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"chord_element_id": 1249510, "weld_lines": 6, "connection_type": "gusset_node", "knife_plates": 1, "working_point_mm": [-11867.7, -17195.8, 17423], "first_member_element_id": 1249630, "backend": "advancesteel", "members_modified": 3, "dry_run": true, "gusset_plates": 1, "bolts": 4}
Cuerpo:
{"errors": [], "warnings": [], "data": {"members_to_modify": [{"current_end_distance_mm": 86.200000000000003, "role": "diagonal", "new_extension_mm": -93.799999999999997, "end": "end", "profile": "HSS2-1-2X2-1-2X3-16 64x64", "setback_mm": 180, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249630}, {"current_end_distance_mm": 18, "role": "vertical", "new_extension_mm": -42, "end": "start", "profile": "HSS2-1-2X2-1-2X3-16 64x64", "setback_mm": 60, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249631}, {"current_end_distance_mm": 49.799999999999997, "role": "diagonal", "new_extension_mm": -210.19999999999999, "end": "end", "profile": "HSS2-1-2X2-1-2X3-16 64x64", "setback_mm": 260, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249636}], "elements_to_create": [{"height_mm": 530, "chord_interface": "through_slot", "kind": "gusset_plate", "thickness_label": "3/8\"", "thickness_mm": 9.5250000000000004, "vertices_count": 8, "width_mm": 565}, {"weld_size_mm": 5, "for_member_id": 1249630, "kind": "welded_slot_interface", "slot_length_mm": 150}, {"weld_size_mm": 5, "for_member_id": 1249631, "kind": "welded_slot_interface", "slot_length_mm": 150}, {"for_member_id": 1249636, "gusset_face": "+z", "length_mm": 170, "kind": "knife_plate", "insertion_mm": 80, "thickness_mm": 10, "width_mm": 14 ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"errors":[{"path":"validation_token","message":"validation_token es obligatorio para crear una conexi\u00f3n.","code":"VALIDATION_TOKEN_INVALID","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token."}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"create","duration_ms":0}}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=2
Cuerpo:
{"errors": [], "warnings": [], "data": {"connections": [{"template_id": "6abcf116-9b97-485f-b50d-2851ca0018cc", "connection_type": "gusset_node", "connection_id": "c3c7373c-6c68-49be-bfbf-9ead1e12b279", "created_utc": "2026-10-04T19:27:42.5751045Z", "backend": "advancesteel", "spec_version": "1.0", "created_elements_count": 9}, {"template_id": "6abcf116-9b97-485f-b50d-2851ca0018cc", "connection_type": "gusset_node", "connection_id": "039107a8-107a-4fd8-a877-f09c33b921fd", "created_utc": "2026-10-04T19:28:54.7511016Z", "backend": "advancesteel", "spec_version": "1.0", "created_elements_count": 9}], "connections_count": 2}, "ok": true, "meta": {"addin_version": "0.1.0", "operation": "list", "duration_ms": 4}}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"connection_id","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo."}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"get","duration_ms":4}}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"connection_id","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","code":"ELEMENT_NOT_FOUND","hint":"Verifica los IDs disponibles con conn_list."}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"delete","duration_ms":4}}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"errors":[{"path":null,"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","code":"UNKNOWN_OPERATION","hint":"Operaciones disponibles: catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate."}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"no_existe","duration_ms":0}}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=1 carpeta=C:\Users\Andy Bayona Ant�n\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"errors":[],"warnings":[],"data":{"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates":[{"chord_profile":"HSS3X3X1/4","connection_type":"gusset_node","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 vertical 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","origin_document":"HANGAR_PRUEBA_sondeo","tags":["hangar","cercha","HSS"],"members_count":3,"created_utc":"2026-10-04T19:21:59.5163425Z","name":"Nudo tipico Detalle D","origin_drawing":"Detalle D","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc"}],"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates_count":1},"ok":true,"meta":{"addin_version":"0.1.0","operation":"catalog_list","duration_ms":2}}
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL', 'ANGLE_DIFFERS_FROM_MODEL'], template_id=3833d4ba-276f-4dfc-9e52-366440743f73 barras=3 archivo=C:\Users\Andy Bayona Ant�n\AppData\Local\MotorConexiones\catalogo\3833d4ba-276f-4dfc-9e52-366440743f73.json
Cuerpo:
{"errors":[],"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."},{"path":"members[1].expected_angle_deg","message":"El \u00e1ngulo del plano (90.0\u00b0, inclinaci\u00f3n 90.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (44.4\u00b0, inclinaci\u00f3n 44.4\u00b0) por 45.6\u00b0 > 1\u00b0.","code":"ANGLE_DIFFERS_FROM_MODEL","hint":"Verifica la geometr\u00eda en el modelo o en el plano."}],"data":{"shared_file":null,"origin":{"document":"HANGAR_PRUEBA_sondeo","connection_id":null,"element_ids":[1249510,1249630,1249631,1249636],"drawing":"Detalle D"},"chord_profile":"HSS3X3X1/4","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\3833d4ba-276f-4dfc-9e52-366440743f73.json","matching":{"angle_tolerance_deg":10,"allow_mirror":true},"members_count":3,"member_pattern":[{"profile_policy":"warn","side":"+Y","role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","angle_deg":136.91999999999999,"slot":0},{"profile_policy":"warn","side":"+Y","role":"vertical","profile":"HSS2-1/2X2-1/2X3/16","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","angle_deg":44.369999999999997,"slot":1},{"profile_policy" ...
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patr�n=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"errors":[],"warnings":[],"data":{"template":{"origin":{"document":"HANGAR_PRUEBA_sondeo","connection_id":null,"element_ids":[1249510,1249630,1249631,1249636],"drawing":"Detalle D"},"chord_pattern":{"profile_policy":"warn","profile":"HSS3X3X1/4","continuous":true},"spec_template":{"chord":{"profile":"HSS3X3X1/4","continuous":true},"uncertain_fields":[],"connection_type":"gusset_node","gusset":{"height_mm":530.0,"chord_interface":"through_slot","thickness_label":"3/8\"","outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"thickness_mm":9.5250000000000004,"width_mm":565.0},"members":[{"expected_angle_deg":45.0,"role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"slot":0},{"expected_angle_deg":90.0,"role":"vertical","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"slot":1},{"expected_angle_deg":45.0,"role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":260.0,"attachment":{"type":"bolted_knife_plate","bolts":{"diameter_label":"5/8\"","edge_mm":40.0,"first_row_from_plate_end_mm":40.0,"diameter_mm":15.875,"spacing_mm":60.0,"rows":2,"columns" ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientaci�n same  [OK]  HTTP 200, ok=True, orientaci�n=same desv�o_m�x=0 token=ebdbbc61fc8c...
Cuerpo:
{"errors":[],"warnings":[],"data":{"calculated_values":{"frame_z":[0,-1,0],"origin_mm":[-11867.700000000001,-17195.799999999999,17423],"axis_distance_mm":0.080000000000000002,"frame_y":[0,0,1],"frame_x":[1,0,0],"chord_direction_reversed":true},"errors_count":0,"node":{"element_ids":[1249510,1249630,1249631,1249636],"chord_direction_reversed":true,"chord_element_id":1249510},"spec":{"chord":{"element_id":1249510,"profile":"HSS3X3X1/4","continuous":true},"uncertain_fields":[],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"connection_type":"gusset_node","gusset":{"height_mm":530.0,"chord_interface":"through_slot","thickness_label":"3/8\"","outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"weld_to_chord":{"size_mm":5.0,"type":"fillet","all_around":true},"thickness_mm":9.5250000000000004,"width_mm":565.0},"members":[{"expected_angle_deg":43.100000000000001,"role":"diagonal","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":180.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"element_id":1249630},{"expected_angle_deg":44.399999999999999,"role":"vertical","profile":"HSS2-1/2X2-1/2X3/16","end_setback_mm":60.0,"attachment":{"type":"welded_slot","slot_length_mm":150.0,"weld":{"size_mm":5.0,"type":"fillet","all_around":true}},"element_id":1249631},{"expected_angle_deg":44.399999999999999,"role":"di ...
======================================================================
22. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"errors":[{"path":"template_id","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","code":"TEMPLATE_NOT_FOUND","hint":"Usa conn_catalog_list para ver las plantillas disponibles."}],"warnings":[],"data":null,"ok":false,"meta":{"addin_version":"0.1.0","operation":"catalog_apply","duration_ms":1}}
======================================================================
23. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 3833d4ba-276f-4dfc-9e52-366440743f73
Cuerpo:
{"errors":[],"warnings":[],"data":{"name":"PRUEBA probar_conexiones","deleted_template_id":"3833d4ba-276f-4dfc-9e52-366440743f73","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\3833d4ba-276f-4dfc-9e52-366440743f73.json"},"ok":true,"meta":{"addin_version":"0.1.0","operation":"catalog_delete","duration_ms":3}}
======================================================================
24. tools/list por el puente trae las 18 herramientas conn_*  [OK]  HTTP 200, herramientas=84 conn_*=18
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply
======================================================================
25. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.1.0
Cuerpo:
{
  "errors": [],
  "warnings": [],
  "data": {
    "document": {
      "title": "HANGAR_PRUEBA_sondeo",
      "is_family": false,
      "is_modifiable": false,
      "is_workshared": false,
      "is_read_only": false,
      "path": "D:\\IG INGENIER�A\\Hartree\\HANGAR_PRUEBA_sondeo.rvt"
    },
    "addin_version": "0.1.0",
    "has_uidocument": true,
    "revit": {
      "version_name": "Autodesk Revit 2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39",
      "version_number": "2027"
    },
    "operations": [
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
    "dotnet": {
      "load_context": "Default",
      "framework": ".NET 10.0.12",
      "assembly_location": "C:\\Users\\Andy Bayona Ant�n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"
    },
    "backend": "advancesteel",
    "spec_version": "1.0"
  },
  "ok": true,
  "meta": {
    "addin_version": "0.1.0",
    "operation": "ping",
    "duration_ms": 2
  }
}
======================================================================
Resultado: 25/25 pruebas correctas

```

## 7-9 sondeo 12 restos de conexiones

```text
== 12-fase3-borrar.py -> HTTP 200 en 745 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 7 ms | errores=- | avisos=-
    conexiones en el modelo: 2
--- get: ok=True en 6 ms | errores=- | avisos=-
--- delete: ok=True en 119 ms | errores=- | avisos=-
    {"deleted_elements_count": 9, "restored_members_count": 3, "deleted_connection_id": "c3c7373c-6c68-49be-bfbf-9ead1e12b279"}
    miembro 1249633: extension inicio -186.129221494 -> 0.0 mm | fin 0.0 -> 0.0 mm
    miembro 1249637: extension inicio -302.056415201 -> 0.0 mm | fin 0.0 -> 0.0 mm
    miembro 1249632: extension inicio 0.0 -> 0.0 mm | fin -17.9300489113 -> 0.0 mm
--- get: ok=True en 11 ms | errores=- | avisos=-
--- delete: ok=True en 91 ms | errores=- | avisos=REVIT_WARNING
    aviso REVIT_WARNING: No se pudo abrir la FabricationTransaction de Advance Steel: InvalidOperationException: cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution. Toda la conexión se crea con DirectShape.
    {"deleted_elements_count": 9, "restored_members_count": 3, "deleted_connection_id": "039107a8-107a-4fd8-a877-f09c33b921fd"}
    miembro 1249626: extension inicio 0.0 -> 0.0 mm | fin -164.673243358 -> 0.0 mm
    miembro 1249627: extension inicio -39.2597250340 -> 0.0 mm | fin 0.0 -> 0.0 mm
    miembro 1249638: extension inicio 0.0 -> 0.0 mm | fin -280.740249594 -> 0.0 mm
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 7-9 sondeo 13 restos de acero

```text
== 13-limpiar-fase1.py -> HTTP 200 en 83 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 7-9 log del dia

```text

{"ts":"2026-10-04T14:14:29.4453894-05:00","record":{"event":"ribbon_panel_created","tab":"ARBA","panel":"MotorConexione
s","tab_already_existed":true,"create_tab_error":"ArgumentException: The tab with the input name exists 
already.\r\nParameter name: tabName"}}
{"ts":"2026-10-04T14:14:29.4509506-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","r
evit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-10-04T14:16:53.2444878-05:00","record":{"event":"handle","operation":"catalog_list","request_summary":"{}",
"ok":true,"error_codes":[],"warning_codes":[],"duration_ms":10}}
{"ts":"2026-10-04T14:18:23.2994038-05:00","record":{"event":"ribbon_preview_opened","file":"D:\\Proyectos 
C#\\CONEXIONES\\docs\\fixtures\\detalle-D-confirmado.json","is_valid":true,"errors":[],"sketch_pieces":49}}
{"ts":"2026-10-04T14:20:00.0680395-05:00","record":{"event":"ribbon_create","operation":"run_spec_ribbon","file":"D:\\P
royectos C#\\CONEXIONES\\docs\\fixtures\\detalle-D-confirmado.json","template_id":null,"connection_id":"ebb2f171-5470-4
32d-b828-6aea655484b3","created_elements":9,"modified_members":3,"backend":"advancesteel","validation_token_prefix":"bc
a1ce7b8e68625d","warnings":["REVIT_WARNING","REVIT_WARNING","REVIT_WARNING"],"duration_ms":1107}}
{"ts":"2026-10-04T14:21:59.5261352-05:00","record":{"event":"catalog_save","template_id":"6abcf116-9b97-485f-b50d-2851c
a0018cc","name":"Nudo tipico Detalle D","file":"C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo
\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","shared_file":"D:\\Proyectos C#\\CONEXIONES\\catalog\\6abcf116-9b97-485f-b
50d-2851ca0018cc.json","from_connection":"ebb2f171-5470-432d-b828-6aea655484b3"}}
{"ts":"2026-10-04T14:21:59.5394489-05:00","record":{"event":"handle","operation":"catalog_save","request_summary":"{\"c
onnection_id\": \"ebb2f171-5470-432d-b828-6aea655484b3\", \"description\": \"Cartela PL 3/8 565x530 con diagonales 
ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8\", \"tags\": [\"hangar\", \"cercha...","ok":true,"error_cod
es":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":59}}
{"ts":"2026-10-04T14:21:59.7372177-05:00","record":{"event":"handle","operation":"catalog_list","request_summary":"{}",
"ok":true,"error_codes":[],"warning_codes":[],"duration_ms":6}}
{"ts":"2026-10-04T14:22:13.1136391-05:00","record":{"event":"handle","operation":"catalog_get","request_summary":"{\"te
mplate_id\": \"6abcf116-9b97-485f-b50d-2851ca0018cc\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-10-04T14:22:13.3437405-05:00","record":{"event":"catalog_apply","template_id":"6abcf116-9b97-485f-b50d-2851
ca0018cc","element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":true,"error_codes":[]}}
{"ts":"2026-10-04T14:22:13.3609999-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"6abcf116-9b97-485f-b50d-2851ca0018cc\", \"element_ids\": [1249510, 1249630, 1249631, 1249636], 
\"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":58}}
{"ts":"2026-10-04T14:23:48.8025974-05:00","record":{"event":"ribbon_catalog_window","document":"HANGAR_PRUEBA_sondeo","
applied_template_id":null,"create_requested":false}}
{"ts":"2026-10-04T14:24:02.6173315-05:00","record":{"event":"ribbon_catalog_apply","template_id":"6abcf116-9b97-485f-b5
0d-2851ca0018cc","element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":true,"error_codes":[]}
}
{"ts":"2026-10-04T14:24:52.8563412-05:00","record":{"event":"ribbon_catalog_apply","template_id":"6abcf116-9b97-485f-b5
0d-2851ca0018cc","element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":true,"error_codes":[]}
}
{"ts":"2026-10-04T14:25:07.7172346-05:00","record":{"event":"ribbon_preview_saved","path":"C:\\Users\\Andy Bayona 
Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo tipico Detalle D-nudo-1249510.json"}}
{"ts":"2026-10-04T14:25:38.8338724-05:00","record":{"event":"ribbon_catalog_window","document":"HANGAR_PRUEBA_sondeo","
applied_template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","create_requested":true}}
{"ts":"2026-10-04T14:25:39.0203763-05:00","record":{"event":"ribbon_create","operation":"catalog_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo tipico Detalle D-nudo-1249510.json","template_id":"
6abcf116-9b97-485f-b50d-2851ca0018cc","connection_id":"f721ff1f-2bd4-4ac5-9536-2ca495c34a5f","created_elements":9,"modi
fied_members":3,"backend":"advancesteel","validation_token_prefix":"855ede381e0e13a3","warnings":[],"duration_ms":177}}
{"ts":"2026-10-04T14:26:24.2058528-05:00","record":{"event":"ribbon_delete","connection_id":"f721ff1f-2bd4-4ac5-9536-2c
a495c34a5f","deleted_elements":9,"restored_members":3,"duration_ms":74,"warnings":[]}}
{"ts":"2026-10-04T14:26:25.5268849-05:00","record":{"event":"ribbon_connections_window","document":"HANGAR_PRUEBA_sonde
o","deleted":1}}
{"ts":"2026-10-04T14:27:00.5064270-05:00","record":{"event":"ribbon_catalog_apply","template_id":"6abcf116-9b97-485f-b5
0d-2851ca0018cc","element_ids":[1249510,1249632,1249633,1249637],"orientation":"mirror_x","is_valid":true,"error_codes"
:[]}}
{"ts":"2026-10-04T14:27:42.5071387-05:00","record":{"event":"ribbon_catalog_window","document":"HANGAR_PRUEBA_sondeo","
applied_template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","create_requested":true}}
{"ts":"2026-10-04T14:27:42.7002607-05:00","record":{"event":"ribbon_create","operation":"catalog_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo tipico Detalle D-nudo-1249510.json","template_id":"
6abcf116-9b97-485f-b50d-2851ca0018cc","connection_id":"c3c7373c-6c68-49be-bfbf-9ead1e12b279","created_elements":9,"modi
fied_members":3,"backend":"advancesteel","validation_token_prefix":"b911166cb2c6bd9c","warnings":[],"duration_ms":183}}
{"ts":"2026-10-04T14:28:24.8611878-05:00","record":{"event":"ribbon_connections_window","document":"HANGAR_PRUEBA_sonde
o","deleted":0}}
{"ts":"2026-10-04T14:28:27.9694848-05:00","record":{"event":"ribbon_catalog_apply_failed","template_id":"6abcf116-9b97-
485f-b50d-2851ca0018cc","element_ids":[1249509,1249626,1249627],"code":"TEMPLATE_NO_MATCH","error":"La plantilla 'Nudo 
tipico Detalle D' no casa con el nudo: mirror_x: ranura 0 (diagonal 43.1°) → barra 1249627 (44.4°, desvío 1.3°); 
ranura 1 (vertical 135.6°) → barra 1249626 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) sin barra"}}
{"ts":"2026-10-04T14:28:31.7232770-05:00","record":{"event":"ribbon_catalog_apply_failed","template_id":"6abcf116-9b97-
485f-b50d-2851ca0018cc","element_ids":[1249509,1249626,1249627],"code":"TEMPLATE_NO_MATCH","error":"La plantilla 'Nudo 
tipico Detalle D' no casa con el nudo: mirror_x: ranura 0 (diagonal 43.1°) → barra 1249627 (44.4°, desvío 1.3°); 
ranura 1 (vertical 135.6°) → barra 1249626 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) sin barra"}}
{"ts":"2026-10-04T14:28:33.0624552-05:00","record":{"event":"ribbon_catalog_apply_failed","template_id":"6abcf116-9b97-
485f-b50d-2851ca0018cc","element_ids":[1249509,1249626,1249627],"code":"TEMPLATE_NO_MATCH","error":"La plantilla 'Nudo 
tipico Detalle D' no casa con el nudo: mirror_x: ranura 0 (diagonal 43.1°) → barra 1249627 (44.4°, desvío 1.3°); 
ranura 1 (vertical 135.6°) → barra 1249626 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) sin barra"}}
{"ts":"2026-10-04T14:28:33.2929437-05:00","record":{"event":"ribbon_catalog_apply_failed","template_id":"6abcf116-9b97-
485f-b50d-2851ca0018cc","element_ids":[1249509,1249626,1249627],"code":"TEMPLATE_NO_MATCH","error":"La plantilla 'Nudo 
tipico Detalle D' no casa con el nudo: mirror_x: ranura 0 (diagonal 43.1°) → barra 1249627 (44.4°, desvío 1.3°); 
ranura 1 (vertical 135.6°) → barra 1249626 (135.6°, desvío 0.1°); ranura 2 (diagonal -44.4°) sin barra"}}
{"ts":"2026-10-04T14:28:36.7658855-05:00","record":{"event":"ribbon_catalog_window","document":"HANGAR_PRUEBA_sondeo","
applied_template_id":null,"create_requested":false}}
{"ts":"2026-10-04T14:28:45.9064540-05:00","record":{"event":"ribbon_catalog_apply","template_id":"6abcf116-9b97-485f-b5
0d-2851ca0018cc","element_ids":[1249509,1249626,1249627,1249638],"orientation":"same","is_valid":true,"error_codes":[]}
}
{"ts":"2026-10-04T14:28:54.6479861-05:00","record":{"event":"ribbon_catalog_window","document":"HANGAR_PRUEBA_sondeo","
applied_template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","create_requested":true}}
{"ts":"2026-10-04T14:28:54.9291090-05:00","record":{"event":"ribbon_create","operation":"catalog_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo tipico Detalle D-nudo-1249509.json","template_id":"
6abcf116-9b97-485f-b50d-2851ca0018cc","connection_id":"039107a8-107a-4fd8-a877-f09c33b921fd","created_elements":9,"modi
fied_members":3,"backend":"advancesteel","validation_token_prefix":"8012da9bf29234ba","warnings":[],"duration_ms":268}}
{"ts":"2026-10-04T14:29:45.8806477-05:00","record":{"event":"handle","operation":"catalog_list","request_summary":"{}",
"ok":true,"error_codes":[],"warning_codes":[],"duration_ms":2}}
{"ts":"2026-10-04T14:29:45.9408538-05:00","record":{"event":"catalog_save","template_id":"3833d4ba-276f-4dfc-9e52-36644
0743f73","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catal
ogo\\3833d4ba-276f-4dfc-9e52-366440743f73.json","shared_file":null,"from_connection":null}}
{"ts":"2026-10-04T14:29:45.9413418-05:00","record":{"event":"handle","operation":"catalog_save","request_summary":"{\"s
pec\": {\"chord\": {\"element_id\": 1249510, \"profile\": \"HSS3X3X1/4\", \"continuous\": true}, \"uncertain_fields\": 
[{\"path\": \"members[1].profile\", \"reason\": \"La etiqueta del montante está cortada en la imag...","ok":true,"error
_codes":[],"warning_codes":["ANGLE_DIFFERS_FROM_MODEL","ANGLE_DIFFERS_FROM_MODEL"],"duration_ms":20}}
{"ts":"2026-10-04T14:29:45.9952835-05:00","record":{"event":"handle","operation":"catalog_get","request_summary":"{\"te
mplate_id\": \"3833d4ba-276f-4dfc-9e52-366440743f73\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-10-04T14:29:46.0407770-05:00","record":{"event":"catalog_apply","template_id":"3833d4ba-276f-4dfc-9e52-3664
40743f73","element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":true,"error_codes":[]}}
{"ts":"2026-10-04T14:29:46.0414114-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"3833d4ba-276f-4dfc-9e52-366440743f73\", \"element_ids\": [1249510, 1249630, 1249631, 1249636], 
\"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":17}}
{"ts":"2026-10-04T14:29:46.0775411-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"00000000-0000-0000-0000-000000000000\", \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":false,"error_codes":["TEMPLATE_NOT_FOUND"],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-10-04T14:29:46.1167132-05:00","record":{"event":"catalog_delete","template_id":"3833d4ba-276f-4dfc-9e52-366
440743f73","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona 
Antón\\AppData\\Local\\MotorConexiones\\catalogo\\3833d4ba-276f-4dfc-9e52-366440743f73.json"}}
{"ts":"2026-10-04T14:29:46.1185937-05:00","record":{"event":"handle","operation":"catalog_delete","request_summary":"{\
"template_id\": 
\"3833d4ba-276f-4dfc-9e52-366440743f73\"}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}



```

## 7-9 catalogo final

```text
== conn/catalog_list -> HTTP 200 en 75 ms ==
{
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates":  [
                                   {
                                       "chord_profile":  "HSS3X3X1/4",
                                       "connection_type":  "gusset_node",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · vertical 44,4° +Y · diagonal -135,6° -Y",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "members_count":  3,
                                       "created_utc":  "2026-10-04T19:21:59.5163425Z",
                                       "name":  "Nudo tipico Detalle D",
                                       "origin_drawing":  "Detalle D",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc"
                                   }
                               ],
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates_count":  1
             },
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "operation":  "catalog_list",
                 "duration_ms":  2
             }
}

```
