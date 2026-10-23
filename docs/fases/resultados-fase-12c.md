# Resultados de la ronda 12c

Fecha: 2026-10-23T11:40:28


## 12c-1 git

```text
77f9416 Cierre de la ronda 12b y ronda 12c (add-in 0.12.2): el perfil del cord├│n le├¡do con Ht, b y el giro de la secci├│n (203,2 en el plano ├ù 304,8 perpendicular), soldaduras WeldLine de Advance Steel con reserva DirectShape, sondeo 28 para la viga de Advance Steel y el corte inclinado
?? docs/fases/resultados-fase-12c.md

```

## 12c-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.12 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.12 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.12 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilaci├│n correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:10.96
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patr├│n especificado.

Correctas! - Con error:     0, Superado:   291, Omitido:     0, Total:   291, Duraci├│n: 717 ms - MotorConexiones.Tests.dll (net10.0)

```

## 12c-1 revit cerrado

```text

```

## 12c-1 deploy

```text
== MotorConexiones 0.12.2.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 12c-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 12c-1 version de la dll

```text
0.12.2.0

```

## 12c-1 sondeo 28 desplegado

```text

Name                     Length
----                     ------
28-viga-advance-steel.py  34505



```

## 12c-2 ping

```text
== conn/ping -> HTTP 200 en 272 ms ==
{
    "data":  {
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39"
                           },
                 "addin_version":  "0.12.2",
                 "document":  {
                                  "is_workshared":  false,
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "is_modifiable":  false,
                                  "is_family":  false,
                                  "is_read_only":  false
                              },
                 "has_uidocument":  true,
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
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "spec_version":  "1.0",
                 "backend":  "advancesteel"
             },
    "meta":  {
                 "duration_ms":  14,
                 "operation":  "ping",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-2 list

```text
== conn/list -> HTTP 200 en 40 ms ==
{
    "data":  {
                 "connections_count":  0,
                 "total_count":  0,
                 "connections":  [

                                 ],
                 "batch_id":  null,
                 "batches":  {

                             }
             },
    "meta":  {
                 "duration_ms":  8,
                 "operation":  "list",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-2 node_info nudo en K

```text
== conn/node_info -> HTTP 200 en 82 ms ==
{
    "data":  {
                 "members":  [
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "section_rotation_deg":  90,
                                     "end_gap_mm":  0,
                                     "slope_deg":  0,
                                     "width_mm":  203.19999999999999,
                                     "length_mm":  10475,
                                     "type":  "HSS12X8X1/2",
                                     "axis_offset_mm":  0,
                                     "height_mm":  304.80000000000001,
                                     "angle_in_plane_deg":  -180,
                                     "is_chord":  true,
                                     "angle_to_chord_deg":  0,
                                     "wall_mm":  11.810000000000000,
                                     "structural_type":  "Beam",
                                     "reaches_node":  true,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "node_end":  0,
                                     "side":  "chord",
                                     "family":  "HSS-Hollow Structural Section",
                                     "element_id":  1245533,
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ]
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "section_rotation_deg":  -180,
                                     "end_gap_mm":  90.400000000000006,
                                     "slope_deg":  44.369999999999997,
                                     "width_mm":  63.5,
                                     "length_mm":  3500,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "axis_offset_mm":  -27.399999999999999,
                                     "height_mm":  63.5,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "is_chord":  false,
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "wall_mm":  0,
                                     "structural_type":  "Beam",
                                     "reaches_node":  true,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "node_end":  1,
                                     "side":  "-Y",
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "element_id":  1249631,
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ]
                                 },
                                 {
                                     "material":  "Material IFC (190-40-140)",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "section_rotation_deg":  -180,
                                     "end_gap_mm":  27.100000000000001,
                                     "slope_deg":  45,
                                     "width_mm":  63.5,
                                     "length_mm":  3546,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "axis_offset_mm":  27.399999999999999,
                                     "height_mm":  63.5,
                                     "angle_in_plane_deg":  -45,
                                     "is_chord":  false,
                                     "angle_to_chord_deg":  45,
                                     "wall_mm":  0,
                                     "structural_type":  "Beam",
                                     "reaches_node":  true,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "node_end":  0,
                                     "side":  "-Y",
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "element_id":  1249632,
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ]
                                 }
                             ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "kind":  "node",
                 "chord_direction_reversed":  true,
                 "eccentricity_mm":  54.899999999999999,
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "axis_distance_mm":  0.089999999999999997,
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "existing_connections":  [

                                          ],
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "chord_element_id":  1245533
             },
    "meta":  {
                 "duration_ms":  61,
                 "operation":  "node_info",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-2 node_info empalme C

```text
== conn/node_info -> HTTP 200 en 21 ms ==
{
    "data":  {
                 "members":  [
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "start_mm":  [
                                                      -4181,
                                                      17204.200000000001,
                                                      19933
                                                  ],
                                     "section_rotation_deg":  90,
                                     "end_gap_mm":  0.40000000000000002,
                                     "slope_deg":  0,
                                     "width_mm":  203.19999999999999,
                                     "length_mm":  10475,
                                     "type":  "HSS12X8X1/2",
                                     "axis_offset_mm":  0,
                                     "height_mm":  304.80000000000001,
                                     "angle_in_plane_deg":  -180,
                                     "is_chord":  false,
                                     "angle_to_chord_deg":  0,
                                     "wall_mm":  11.810000000000000,
                                     "structural_type":  "Beam",
                                     "reaches_node":  true,
                                     "end_mm":  [
                                                    -14656,
                                                    17204.299999999999,
                                                    19933
                                                ],
                                     "node_end":  0,
                                     "side":  "-Y",
                                     "family":  "HSS-Hollow Structural Section",
                                     "element_id":  1245531,
                                     "axis_cut_mm":  [
                                                         -4180.5,
                                                         17204.200000000001,
                                                         19933
                                                     ]
                                 },
                                 {
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "start_mm":  [
                                                      6069.8999999999996,
                                                      17204.200000000001,
                                                      19933
                                                  ],
                                     "section_rotation_deg":  90,
                                     "end_gap_mm":  0,
                                     "slope_deg":  0,
                                     "width_mm":  203.19999999999999,
                                     "length_mm":  10250,
                                     "type":  "HSS12X8X1/2",
                                     "axis_offset_mm":  0,
                                     "height_mm":  304.80000000000001,
                                     "angle_in_plane_deg":  0,
                                     "is_chord":  true,
                                     "angle_to_chord_deg":  0,
                                     "wall_mm":  11.810000000000000,
                                     "structural_type":  "Beam",
                                     "reaches_node":  true,
                                     "end_mm":  [
                                                    -4180.1000000000004,
                                                    17204.200000000001,
                                                    19933
                                                ],
                                     "node_end":  1,
                                     "side":  "chord",
                                     "family":  "HSS-Hollow Structural Section",
                                     "element_id":  1245530,
                                     "axis_cut_mm":  [
                                                         -4180.5,
                                                         17204.200000000001,
                                                         19933
                                                     ]
                                 }
                             ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                0
                            ],
                 "kind":  "splice",
                 "chord_direction_reversed":  true,
                 "eccentricity_mm":  0,
                 "origin_mm":  [
                                   -4180.5,
                                   17204.200000000001,
                                   19933
                               ],
                 "y_axis":  [
                                0,
                                0,
                                1
                            ],
                 "axis_distance_mm":  0,
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "existing_connections":  [

                                          ],
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "chord_element_id":  1245530
             },
    "meta":  {
                 "duration_ms":  5,
                 "operation":  "node_info",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-2 validate fixture K

```text
== conn/validate -> HTTP 200 en 120 ms ==
{
    "data":  {
                 "bolt_stacks":  [

                                 ],
                 "is_valid":  true,
                 "calculated_values":  {
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "kind":  "node",
                                           "chord_direction_reversed":  true,
                                           "eccentricity_mm":  54.899999999999999,
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "origin_mm":  [
                                                             -9278.1000000000004,
                                                             -17195.700000000001,
                                                             19933
                                                         ],
                                           "axis_distance_mm":  0.089999999999999997,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ]
                                       },
                 "errors_count":  0,
                 "validation_token":  "d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9",
                 "warnings_count":  2
             },
    "meta":  {
                 "duration_ms":  78,
                 "operation":  "validate",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [
                     {
                         "code":  "MEMBER_ECCENTRIC",
                         "message":  "El eje del miembro [1249631] corta el cordón a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "path":  "members[0].axis_offset_mm"
                     },
                     {
                         "code":  "MEMBER_ECCENTRIC",
                         "message":  "El eje del miembro [1249632] corta el cordón a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "path":  "members[1].axis_offset_mm"
                     }
                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-2 validate fixture C

```text
== conn/validate -> HTTP 200 en 56 ms ==
{
    "data":  {
                 "bolt_stacks":  [
                                     {
                                         "member_element_id":  1245531,
                                         "bolt_length_mm":  50.799999999999997,
                                         "bolts":  12,
                                         "plates":  2,
                                         "gap_mm":  0,
                                         "grip_mm":  25.399999999999999,
                                         "length_source":  "computed_from_grip"
                                     }
                                 ],
                 "is_valid":  true,
                 "calculated_values":  {
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "kind":  "splice",
                                           "chord_direction_reversed":  true,
                                           "eccentricity_mm":  0,
                                           "frame_z":  [
                                                           0,
                                                           -1,
                                                           0
                                                       ],
                                           "origin_mm":  [
                                                             -4180.5,
                                                             17204.200000000001,
                                                             19933
                                                         ],
                                           "axis_distance_mm":  0,
                                           "frame_x":  [
                                                           1,
                                                           0,
                                                           0
                                                       ]
                                       },
                 "errors_count":  0,
                 "validation_token":  "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8",
                 "warnings_count":  0
             },
    "meta":  {
                 "duration_ms":  41,
                 "operation":  "validate",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-3 sondeo 28 viga de Advance Steel

```text
== 28-viga-advance-steel.py -> HTTP 200 en 3160 ms ==
=== 28-viga-advance-steel (ronda 12c) ===
   modo: CANCELAR al final (nada queda)
0) Cordon [1245533] (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0); diagonal [1249631] (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9); extremo que llega: fin (-9354.7, -17195.8, 19884.9); angulo con el cordon 44.4; extension actual 69.1996468112 mm
1) SteelElementProperties (RevitAPI), metodos estaticos:
   AddFabricationInformationForRevitElements(Document, IList`1) -> IList`1
   GetFabricationUniqueID(Document, Reference) -> Guid
   GetReference(Document, Guid) -> Reference
   GetSteelElementProperties(Element) -> SteelElementProperties
2) Transaccion Autodesk.SteelConnectionsDB.FabricationTransaction; BeamShortening(eEnd, Double) y WeldLine(Point3d[], Vector3d, Vector3d): si; FilerObject en ASObjectsMgd; eEnd: kMid, kStart, kEnd, kNoEnd
3) Cordon: ancho nominal 203.2, canto nominal 304.8, Cross-Section Rotation 90 -> en el plano 203.2 (medio canto 101.6); diagonal ancho 63.5, huella en la cara 90.8 mm, centro de la huella (-9409.4, -17195.8, 19831.4)
4) FabricationTransaction abierta. doc.IsModifiable=True
   ensamblados de acero cargados: ASCADLinkMgd, ASGeometryMgd, ASModelerMgd, ASObjectsMgd, ASSettingsMgd, AsRealDWGLoaderMgd, Autodesk.SteelConnections.ASRvtFamilyMapping, Autodesk.SteelConnections.ASRvtGeomConversions, Autodesk.SteelConnections.ASRvtModeler, Autodesk.SteelConnectionsDB, Autodesk.SteelConnectionsDB.resources, Autodesk.SteelConnectionsUI, DotNetRoots, DotNetRootsMgd, RevitAPISteel
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.LengthConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.SpecificParametersVisibilityConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.AngleConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.ParamUnitConverter: 2 estaticos: get_CurrentDocument() -> Document; set_CurrentDocument(Document) -> Void
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.DynamicUnitConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.InverseBooleanConverter: 0 estaticos: 
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.DocumentManagement.DocumentManager: 9 estaticos: DestroyDatabase(Database) -> Int32; GetAllDocuments() -> List`1; GetCurrentDatabase() -> Database; GetCurrentDocument() -> Document; LockCurrentDocument() -> Boolean; LockDocument(Document) -> Boolean; SetCurrentDocument(Document) -> Void; UnlockCurrentDocument() -> Boolean; UnlockDocument(Document) -> Boolean
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.CADAccess.DatabaseManager: 13 estaticos: Erase(IEnumerable`1) -> Void; ExportToACIS(ObjectId[], String) -> Void; GetFilerObjectId(ObjectId) -> ObjectId; GetFilerObjectId(ObjectId, Boolean) -> ObjectId; GetModelObjectIds(ObjectId[]&) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter, Database) -> Void; GetReprId(FilerObject) -> ObjectId; Open(ObjectId) -> FilerObject; Open(ObjectId, eOpenMode) -> FilerObject; Open(ObjectId, eOpenMode, eObjectType) -> FilerObject; WasObjectReplaced(String, String&) -> Boolean
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.DocumentManagement.CADDocumentManager: 3 estaticos: CurrentUCS() -> Matrix3d; GetCurrentDatabase() -> OpenDatabase; GetCurrentDwg() -> Database
   tipo ASCADLinkMgd.Autodesk.AdvanceSteel.CADLink.InteropServices.CADLinkInteropServices: 2 estaticos: ManagedString(Char*) -> String; NativeBuffer(String) -> Char*
   tipo ASCADLinkMgd.Autodesk.AdvanceSteel.CADLink.Database.ObjectId: 3 estaticos: get_kNull() -> ObjectId; op_Equality(ObjectId, ObjectId) -> Boolean; op_Inequality(ObjectId, ObjectId) -> Boolean
   tipo ASCADLinkMgd.AcDbObjectId: 0 estaticos: 
   tipo ASCADLinkMgd.AstDbObjectId: 0 estaticos: 
   puente: AddFabricationInformationForRevitElements -> 0
   puente: GetFabricationUniqueID -> 0c6957d7-153f-47ae-9be0-c329c57f0ec3
   puente: ASObjectsMgd.FilerObject.GetFilerObjectByHandle(String) -> None
   NO encontrada por ningun camino (ver los intentos y los tipos de arriba)
   WeldLine creada: Thickness=5.0; MainEffectiveThroat=5.0; DoubleEffectiveThroat=5.0; IsClosed=True
   WeldLine.Connect: sin Connect (no hay pieza de Advance Steel)
   WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589; IsClosed=True
   WeldPoint: WriteToDb OK
7) Elementos nuevos con la transaccion abierta: 0; extension del extremo ahora: 69.1996468112 mm
8) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False | extension del extremo: 69.1996468112 mm
9) Elementos nuevos tras cerrar: 0 (se esperan 0)
10) RESUMEN: WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589; IsClosed=True | WeldLine.Connect: sin Connect (no hay pieza de Advance Steel) | WeldPoint: WriteToDb OK | WeldPoint.Connect: sin Connect (no hay pieza de Advance Steel) | viga AS: NO encontrada por ningun camino (ver los intentos y los tipos de arriba)
=== fin 28-viga-advance-steel ===


```

## 12c-3 sondeo 12 tras el sondeo

```text
== 12-fase3-borrar.py -> HTTP 200 en 90 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 5 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 10 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 12c-3 sondeo 13 tras el sondeo

```text
== 13-limpiar-fase1.py -> HTTP 200 en 35 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```


## 12c-4 sondeo 28 con commit

```text
== 28-viga-advance-steel.py -> HTTP 200 en 4538 ms ==
=== 28-viga-advance-steel (ronda 12c) ===
   modo: COMMIT al final (existe C:\Users\ANDYBA~1\AppData\Local\Temp\b936a55a-d4cc-4ad0-9f87-0b4d72950c47\sondeo28-commit.txt): captura y Ctrl+Z
0) Cordon [1245533] (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0); diagonal [1249631] (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9); extremo que llega: fin (-9354.7, -17195.8, 19884.9); angulo con el cordon 44.4; extension actual 69.1996468112 mm
1) SteelElementProperties (RevitAPI), metodos estaticos:
   AddFabricationInformationForRevitElements(Document, IList`1) -> IList`1
   GetFabricationUniqueID(Document, Reference) -> Guid
   GetReference(Document, Guid) -> Reference
   GetSteelElementProperties(Element) -> SteelElementProperties
2) Transaccion Autodesk.SteelConnectionsDB.FabricationTransaction; BeamShortening(eEnd, Double) y WeldLine(Point3d[], Vector3d, Vector3d): si; FilerObject en ASObjectsMgd; eEnd: kMid, kStart, kEnd, kNoEnd
3) Cordon: ancho nominal 203.2, canto nominal 304.8, Cross-Section Rotation 90 -> en el plano 203.2 (medio canto 101.6); diagonal ancho 63.5, huella en la cara 90.8 mm, centro de la huella (-9409.4, -17195.8, 19831.4)
4) FabricationTransaction abierta. doc.IsModifiable=True
   ensamblados de acero cargados: ASCADLinkMgd, ASGeometryMgd, ASModelerMgd, ASObjectsMgd, ASProfilesMgd, ASSettingsMgd, AsRealDWGLoaderMgd, Autodesk.SteelConnections.ASDynamicProfiles, Autodesk.SteelConnections.ASRvtFamilyMapping, Autodesk.SteelConnections.ASRvtGeomConversions, Autodesk.SteelConnections.ASRvtModeler, Autodesk.SteelConnectionsDB, Autodesk.SteelConnectionsDB.resources, Autodesk.SteelConnectionsUI, DotNetRoots, DotNetRootsMgd, RevitAPISteel
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.LengthConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.SpecificParametersVisibilityConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.AngleConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.ParamUnitConverter: 2 estaticos: get_CurrentDocument() -> Document; set_CurrentDocument(Document) -> Void
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.DynamicUnitConverter: 0 estaticos: 
   tipo Autodesk.SteelConnectionsUI.Autodesk.SteelConnectionsUI.GUI.InverseBooleanConverter: 0 estaticos: 
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.DocumentManagement.DocumentManager: 9 estaticos: DestroyDatabase(Database) -> Int32; GetAllDocuments() -> List`1; GetCurrentDatabase() -> Database; GetCurrentDocument() -> Document; LockCurrentDocument() -> Boolean; LockDocument(Document) -> Boolean; SetCurrentDocument(Document) -> Void; UnlockCurrentDocument() -> Boolean; UnlockDocument(Document) -> Boolean
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.CADAccess.DatabaseManager: 13 estaticos: Erase(IEnumerable`1) -> Void; ExportToACIS(ObjectId[], String) -> Void; GetFilerObjectId(ObjectId) -> ObjectId; GetFilerObjectId(ObjectId, Boolean) -> ObjectId; GetModelObjectIds(ObjectId[]&) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter) -> Void; GetModelObjectIds(ObjectId[]&, ClassTypeFilter, Database) -> Void; GetReprId(FilerObject) -> ObjectId; Open(ObjectId) -> FilerObject; Open(ObjectId, eOpenMode) -> FilerObject; Open(ObjectId, eOpenMode, eObjectType) -> FilerObject; WasObjectReplaced(String, String&) -> Boolean
   tipo ASObjectsMgd.Autodesk.AdvanceSteel.DocumentManagement.CADDocumentManager: 3 estaticos: CurrentUCS() -> Matrix3d; GetCurrentDatabase() -> OpenDatabase; GetCurrentDwg() -> Database
   tipo ASCADLinkMgd.Autodesk.AdvanceSteel.CADLink.InteropServices.CADLinkInteropServices: 2 estaticos: ManagedString(Char*) -> String; NativeBuffer(String) -> Char*
   tipo ASCADLinkMgd.Autodesk.AdvanceSteel.CADLink.Database.ObjectId: 3 estaticos: get_kNull() -> ObjectId; op_Equality(ObjectId, ObjectId) -> Boolean; op_Inequality(ObjectId, ObjectId) -> Boolean
   tipo ASCADLinkMgd.AcDbObjectId: 0 estaticos: 
   tipo ASCADLinkMgd.AstDbObjectId: 0 estaticos: 
   puente: AddFabricationInformationForRevitElements -> 0
   puente: GetFabricationUniqueID -> 1acf653c-2bf5-4c48-976c-90cea5861720
   puente: ASObjectsMgd.FilerObject.GetFilerObjectByHandle(String) -> None
   NO encontrada por ningun camino (ver los intentos y los tipos de arriba)
   WeldLine creada: Thickness=5.0; MainEffectiveThroat=5.0; DoubleEffectiveThroat=5.0; IsClosed=True
   WeldLine.Connect: sin Connect (no hay pieza de Advance Steel)
   WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589; IsClosed=True
   WeldPoint: WriteToDb OK
7) Elementos nuevos con la transaccion abierta: 0; extension del extremo ahora: 69.1996468112 mm
8) Commit() hecho (existe C:\Users\ANDYBA~1\AppData\Local\Temp\b936a55a-d4cc-4ad0-9f87-0b4d72950c47\sondeo28-commit.txt): mira la diagonal y la soldadura, captura y deshaz con Ctrl+Z (o cierra sin guardar).
   doc.IsModifiable tras la operacion: False | extension del extremo: 69.1996468112 mm
9) Elementos nuevos tras cerrar: 2 (con Commit: una WeldLine materializada seria 1 de categoria Structural Connections)
   nuevo 1322481: Welds | SteelProxyElement
   nuevo 1322482: Welds | SteelProxyElement
10) RESUMEN: WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.602743589; IsClosed=True | WeldLine.Connect: sin Connect (no hay pieza de Advance Steel) | WeldPoint: WriteToDb OK | WeldPoint.Connect: sin Connect (no hay pieza de Advance Steel) | viga AS: NO encontrada por ningun camino (ver los intentos y los tipos de arriba)
=== fin 28-viga-advance-steel ===

```

## 12c-4 sondeo 12 tras deshacer

```text
== 12-fase3-borrar.py -> HTTP 200 en 494 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 5 ms | errores=- | avisos=-
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

## 12c-4 sondeo 13 tras deshacer

```text
== 13-limpiar-fase1.py -> HTTP 200 en 35 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12c-5 list tras el nudo K

```text
== conn/list -> HTTP 200 en 422 ms ==
{
    "data":  {
                 "connections_count":  1,
                 "total_count":  1,
                 "connections":  [
                                     {
                                         "backend":  "advancesteel",
                                         "created_elements_count":  2,
                                         "created_utc":  "2026-10-23T17:16:58.3257033Z",
                                         "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                         "connection_id":  "56652883-6a1d-44da-a01e-80e0b591d292",
                                         "spec_version":  "1.0",
                                         "batch_id":  null,
                                         "connection_type":  "direct_weld"
                                     }
                                 ],
                 "batch_id":  null,
                 "batches":  {

                             }
             },
    "meta":  {
                 "duration_ms":  7,
                 "operation":  "list",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-5 delete nudo K

```text
== conn/delete -> HTTP 200 en 563 ms ==
{
    "data":  {
                 "deleted_connection_id":  "56652883-6a1d-44da-a01e-80e0b591d292",
                 "restored_members_count":  2,
                 "deleted_elements_count":  2
             },
    "meta":  {
                 "duration_ms":  139,
                 "operation":  "delete",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-5 list tras el empalme

```text
== conn/list -> HTTP 200 en 415 ms ==
{
    "data":  {
                 "connections_count":  1,
                 "total_count":  1,
                 "connections":  [
                                     {
                                         "backend":  "advancesteel",
                                         "created_elements_count":  8,
                                         "created_utc":  "2026-10-23T17:19:41.9300666Z",
                                         "template_id":  null,
                                         "connection_id":  "0caa4a6d-79ad-45aa-a899-2b82749b4c11",
                                         "spec_version":  "1.0",
                                         "batch_id":  null,
                                         "connection_type":  "end_plate"
                                     }
                                 ],
                 "batch_id":  null,
                 "batches":  {

                             }
             },
    "meta":  {
                 "duration_ms":  4,
                 "operation":  "list",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-5 delete empalme

```text
== conn/delete -> HTTP 200 en 521 ms ==
{
    "data":  {
                 "deleted_connection_id":  "0caa4a6d-79ad-45aa-a899-2b82749b4c11",
                 "restored_members_count":  2,
                 "deleted_elements_count":  8
             },
    "meta":  {
                 "duration_ms":  115,
                 "operation":  "delete",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-5 sondeo 12 tras borrar

```text
== 12-fase3-borrar.py -> HTTP 200 en 108 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 5 ms | errores=- | avisos=-
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

## 12c-5 sondeo 13 tras borrar

```text
== 13-limpiar-fase1.py -> HTTP 200 en 44 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12c-6 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         38240



```

## 12c-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.12.2 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"data":{"revit":{"version_name":"Autodesk Revit 2027","version_number":"2027","language":"English_USA","sub_version_number":"2027.2","version_build":"27.2.0.39"},"addin_version":"0.12.2","document":{"is_workshared":false,"title":"HANGAR_PRUEBA_sondeo","path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","is_modifiable":false,"is_family":false,"is_read_only":false},"has_uidocument":true,"operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12","load_context":"Default"},"spec_version":"1.0","backend":"advancesteel"},"meta":{"duration_ms":3,"operation":"ping","addin_version":"0.12.2"},"warnings":[],"ok":true,"errors":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 32026 caracteres
Cuerpo:
{"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obligatorio, en este orden\n\n1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"data":{"connection_types":[{"description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n.","type_name":"direct_weld"},{"description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).","type_name":"end_plate"},{"description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","type_name":"gusset_node"}]},"meta":{"duration_ms":1,"operation":"types","addin_version":"0.12.2"},"warnings":[],"ok":true,"errors":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"data":{"connection_type":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.","example":{"chord":{"element_id":1249510,"continuous":true,"profile":"HSS3X3X1/4"},"source":{"drawing":"Detalle D","scale":"1/10"},"members":[{"expected_angle_deg":45.0,"end_setback_mm":180.0,"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"weld":{"type":"fillet","size_mm":5.0,"all_around":true},"type":"welded_slot","slot_length_mm":150.0},"role":"diagonal"}],"uncertain_fields":[],"dimension_chains":[{"expected_total_mm":565.0,"values_mm":[75.0,420.0,70.0],"label":"borde superior"}],"spec_version":"1.0","node":{"element_ids":[1249510,1249630,1249631,1249636]},"gusset":{"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"height_mm":530.0,"width_mm":565.0,"thickness_mm":9.5250000000000004,"chord_interface":"through_slot","weld_to_chord":{"type":"fillet","size_mm":5.0,"all_around":true},"thickness_label":"3/8\""},"connection_type":"gusset_node"},"json_schema":{"additionalProperties":false,"title":"GussetNodeConnectionSpec","type":"object","$schema":"http://json-schema.org/draft-07/schema#","required":["spec_version","connection_type","node","chord","gusset","members"],"properties":{"chord":{"type":"object","required":["element_id","continuous"],"properties":{"el ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"schema","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"UNKNOWN_OPERATION","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node","path":"type"}]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"data": {"total_profiles_in_model": 29, "matched_count": 1, "query": "HSS2-1/2X2-1/2X3/16", "matches": [{"family_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "type_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "suggestions": []}, "meta": {"duration_ms": 8, "operation": "find_profile", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"data": {"members": [{"material": "Steel ASTM A500, Grade B, Rectangular and Square", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "section_rotation_deg": 180, "end_gap_mm": 0, "slope_deg": 0, "width_mm": 76.200000000000003, "length_mm": 9960.2999999999993, "type": "HSS3X3X1/4", "axis_offset_mm": 0, "height_mm": 76.200000000000003, "angle_in_plane_deg": 0, "is_chord": true, "angle_to_chord_deg": 0, "wall_mm": 5.9199999999999999, "structural_type": "Beam", "reaches_node": true, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "node_end": 1, "side": "chord", "family": "HSS-Hollow Structural Section", "element_id": 1249510, "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423]}, {"material": "Material IFC (190-40-140)", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "section_rotation_deg": -180, "end_gap_mm": 84.900000000000006, "slope_deg": 43.079999999999998, "width_mm": 63.5, "length_mm": 3568, "type": "HSS2-1-2X2-1-2X3-16 64x64", "axis_offset_mm": 1.7000000000000000, "height_mm": 63.5, "angle_in_plane_deg": 136.90000000000001, "is_chord": false, "angle_to_chord_deg": 43.100000000000001, "wall_mm": 0, "structural_type": "Beam", "reaches_node": true, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "node_end": 1, "side": "+Y", "family": "HSS2-1-2X2-1-2X3-16 64x64", "element_id": 1249630, "axis_cut_mm": [-11867.700000000001, -17195.799999999999, 17423]}, {"material": "Material IFC (1 ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"data":{"bolt_stacks":[{"member_element_id":1249636,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip"}],"is_valid":true,"calculated_values":{"frame_y":[0,0,1],"kind":"node","chord_direction_reversed":true,"eccentricity_mm":3.3999999999999999,"frame_z":[0,-1,0],"origin_mm":[-11869.400000000000,-17195.799999999999,17423],"axis_distance_mm":0.080000000000000002,"frame_x":[1,0,0]},"errors_count":0,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","warnings_count":1},"meta":{"duration_ms":29,"operation":"validate","addin_version":"0.12.2"},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg"}],"ok":true,"errors":[]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"meta":{"duration_ms":9,"operation":"validate","addin_version":"0.12.2"},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg"}],"ok":false,"errors":[{"code":"DIMENSION_CHAIN_MISMATCH","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","path":"dimension_chains[0].values_mm"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"data":null,"meta":{"duration_ms":11,"operation":"validate","addin_version":"0.12.2"},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg"}],"ok":false,"errors":[{"code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[0].user_confirmed_value"},{"code":"UNRESOLVED_UNCERTAINTY","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","path":"uncertain_fields[1].user_confirmed_value"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"backend": "advancesteel", "dry_run": true, "working_point_mm": [-11869.4, -17195.8, 17423], "members_modified": 3, "bolts": 4, "first_member_element_id": 1249630, "gusset_plates": 1, "knife_plates": 1, "weld_lines": 6, "chord_element_id": 1249510, "connection_type": "gusset_node"}
Cuerpo:
{"data": {"elements_to_create": [{"vertices_count": 8, "height_mm": 530, "kind": "gusset_plate", "width_mm": 565, "thickness_mm": 9.5250000000000004, "chord_interface": "through_slot", "thickness_label": "3/8\""}, {"weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface", "for_member_id": 1249630}, {"weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface", "for_member_id": 1249631}, {"kind": "knife_plate", "width_mm": 140, "gusset_face": "+z", "offset_from_gusset_plane_mm": 9.7620000000000005, "thickness_mm": 10, "length_mm": 170, "insertion_mm": 80, "for_member_id": 1249636}, {"count": 4, "spacing_mm": 60, "rows": 2, "kind": "bolt_group", "diameter_mm": 15.875, "edge_mm": 40, "length_mm": 44.450000000000003, "columns": 2, "grip_mm": 19.524999999999999, "length_source": "computed_from_grip", "for_member_id": 1249636}], "members_to_modify": [{"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 84.900000000000006, "element_id": 1249630, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "setback_mm": 180, "role": "diagonal", "end": "end", "new_extension_mm": -95.099999999999994}, {"action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "current_end_distance_mm": 19.199999999999999, "element_id": 1249631, "profile": "HSS2-1-2X2-1-2X3-16 64x64", "setback_mm": 60, "role": "diagonal", "end": "start", "new_extension_mm": -40.799999999999997}, {" ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"create","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"VALIDATION_TOKEN_INVALID","message":"validation_token es obligatorio para crear una conexi\u00f3n.","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","path":"validation_token"}]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"data": {"connections_count": 0, "total_count": 0, "connections": [], "batch_id": null, "batches": {}}, "meta": {"duration_ms": 4, "operation": "list", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":4,"operation":"get","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","path":"connection_id"}]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":4,"operation":"delete","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"ELEMENT_NOT_FOUND","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Verifica los IDs disponibles con conn_list.","path":"connection_id"}]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"no_existe","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"UNKNOWN_OPERATION","message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","path":null}]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"data":{"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","templates":[{"origin_document":"HANGAR_PRUEBA_sondeo","chord_profile":"HSS12X8X1/2","name":"Nudo K cordon superior","pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","created_utc":"2026-10-21T16:49:07.9104459Z","template_id":"91adac27-d196-4c98-88db-6579aa991be4","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","description":null,"tags":[],"members_count":2,"origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","connection_type":"direct_weld"},{"origin_document":"HANGAR_PRUEBA_sondeo","chord_profile":"HSS3X3X1/4","name":"Nudo tipico Detalle D","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","created_utc":"2026-10-04T22:01:50.5585233Z","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","tags":["hangar","cercha","HSS"],"members_count":3,"origin_drawing":"Detalle D","connection_type":"gusset_node"}],"shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates_count":2},"meta":{"duration_ms":23,"operation":"catalog_list","addin_version":"0.12.2"} ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=18421ee8-c2bf-43f7-a613-4f4ad488126f barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\18421ee8-c2bf-43f7-a613-4f4ad488126f.json
Cuerpo:
{"data":{"chord_profile":"HSS3X3X1/4","name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\18421ee8-c2bf-43f7-a613-4f4ad488126f.json","member_pattern":[{"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":0,"profile_policy":"warn","role":"diagonal","angle_deg":136.91999999999999},{"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":1,"profile_policy":"warn","role":"diagonal","angle_deg":44.369999999999997},{"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":2,"profile_policy":"warn","role":"diagonal","angle_deg":-135.63000000000000}],"template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f","matching":{"allow_mirror":true,"angle_tolerance_deg":10},"origin":{"connection_id":null,"drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo"},"members_count":3,"shared_file":null},"meta":{"duration_ms":31,"operation":"catalog_save","addin_version":"0.12.2"},"warnings":[{"code":"ANGLE_DIFFERS_FROM_MODEL","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","path":"members[0].expected_angle_deg"}],"ok":true,"errors":[]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\18421ee8-c2bf-43f7-a613-4f4ad488126f.json","pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f","template":{"catalog_version":"1.0","name":"PRUEBA probar_conexiones","created_utc":"2026-10-23T17:27:11.7950148Z","template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f","chord_pattern":{"continuous":true,"profile":"HSS3X3X1/4","profile_policy":"warn"},"description":null,"member_pattern":[{"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":0,"profile_policy":"warn","role":"diagonal","angle_deg":136.91999999999999},{"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":1,"profile_policy":"warn","role":"diagonal","angle_deg":44.369999999999997},{"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile":"HSS2-1/2X2-1/2X3/16","slot":2,"profile_policy":"warn","role":"diagonal","angle_deg":-135.63000000000000}],"matching":{"allow_mirror":true,"angle_tolerance_deg":10},"tags":["prueba"],"origin":{"connection_id":null,"drawing":"Detalle D","element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo"},"spec_template":{"chord":{"continuous":true,"profile":"HSS3X3X1/4"},"source":{"drawing":"Detalle D","scale":"1/10"},"members":[{"expected_ang ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=5873498f1d14...
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","match":{"assignments":[{"model_angle_deg":136.91999999999999,"template_angle_deg":136.91999999999999,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249630,"deviation_deg":0,"slot":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"model_angle_deg":44.369999999999997,"template_angle_deg":44.369999999999997,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249631,"deviation_deg":0,"slot":1,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"model_angle_deg":-135.63000000000000,"template_angle_deg":-135.63000000000000,"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249636,"deviation_deg":0,"slot":2,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"}],"matched_count":3,"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0)","unmatched_slots":[],"score_deg":0.01,"orientation":"same","unassigned_members":[],"is_complete":true,"max_deviation_deg":0},"bolt_stacks":[{"member_element_id":1249636,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip"}],"template_id":"18421ee ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","match":{"assignments":[{"model_angle_deg":136.91999999999999,"template_angle_deg":136.91999999999999,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249630,"deviation_deg":0,"slot":0,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"model_angle_deg":44.369999999999997,"template_angle_deg":44.369999999999997,"side":"+Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249631,"deviation_deg":0,"slot":1,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"},{"model_angle_deg":-135.63000000000000,"template_angle_deg":-135.63000000000000,"side":"-Y","model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","element_id":1249636,"deviation_deg":0,"slot":2,"profile_policy":"warn","template_profile":"HSS2-1/2X2-1/2X3/16","role":"diagonal"}],"matched_count":3,"description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 barra 1249631 (44.4\u00b0, desv\u00edo 0.0\u00b0); ranura 2 (diagonal -135.6\u00b0) \u2192 barra 1249636 (-135.6\u00b0, desv\u00edo 0.0\u00b0)","unmatched_slots":[],"score_deg":0.01,"orientation":"same","unassigned_members":[],"is_complete":true,"max_deviation_deg":0},"bolt_stacks":[{"member_element_id":1249636,"bolt_length_mm":44.450000000000003,"gusset_face":"+z","grip_mm":19.524999999999999,"length_source":"computed_from_grip"}],"template_id":"18421ee ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=22c89cba-8f39-42e3-a1f1-e42ebd1b31ca nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"data":{"hidden_text":"5 barras sueltas","nodes":[{"template_id":null,"is_valid":false,"is_mirrored":false,"attempts":[],"warnings":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","errors_count":0,"existing_connection_id":null,"label_index":null,"chord_continuous":false,"chord_element_id":0,"warnings_count":0,"errors":[],"existing_batch_id":null,"member_element_ids":[1249630],"is_marked":false,"has_spec_override":false,"orientation":null,"status":"untyped","signature":"1 barra(s) sin marco","element_ids":[1249630],"created_connection_id":null,"spec":null,"name":"N1","members":[],"kind":"node","max_deviation_deg":null,"status_text":"\u25cb Barra suelta (no es nudo)","starter":false,"through_element_ids":[],"replaces_existing":false,"chord_type_name":null,"validation_token":null,"template_name":null,"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"advice":"No es un nudo: nada que hacer","splice_element_id":null,"ghost_element_id":null,"connection_type":null,"actions":[],"match":null,"is_manual":false,"visible_by_default":false,"color_name":null,"eccentricity_mm":0,"color_rgb":null,"marker_element_id":null},{"template_id":null,"is_valid":false,"is_mirrored":false,"attempts":[],"warnings":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","errors_count":0,"existing_connection_id":null,"label_index":null,"chord_continuous":false,"chord_element_id":0,"warnings_count":0,"erro ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=08a3e3d82996...
Cuerpo:
{"data":{"node":{"template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f","is_valid":true,"is_mirrored":false,"attempts":[],"warnings":[],"status_detail":null,"errors_count":0,"existing_connection_id":null,"label_index":null,"chord_continuous":true,"chord_element_id":1249510,"warnings_count":0,"errors":[],"existing_batch_id":null,"member_element_ids":[1249630,1249631,1249636],"is_marked":false,"has_spec_override":false,"orientation":"same","status":"ready","signature":"2 +Y (137, 44) \u00b7 1 -Y (-136)","element_ids":[1249510,1249630,1249631,1249636],"created_connection_id":null,"spec":{"chord":{"continuous":true,"element_id":1249510,"profile":"HSS3X3X1/4"},"source":{"template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f","drawing":"Detalle D","scale":"1/10","batch_id":"22c89cba-8f39-42e3-a1f1-e42ebd1b31ca"},"members":[{"expected_angle_deg":43.100000000000001,"end_setback_mm":180.0,"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"weld":{"type":"fillet","size_mm":5.0,"all_around":true},"type":"welded_slot","slot_length_mm":150.0},"role":"diagonal"},{"expected_angle_deg":44.399999999999999,"end_setback_mm":60.0,"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"weld":{"type":"fillet","size_mm":5.0,"all_around":true},"type":"welded_slot","slot_length_mm":150.0},"role":"diagonal"},{"expected_angle_deg":44.399999999999999,"end_setback_mm":260.0,"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","attachment":{"plate":{"width_mm":140.0,"thickness_mm":1 ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote 22c89cba: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"data":{"nodes":[{"duration_ms":0,"outcome":"failed","description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","reason":"token distinto del plan","restored_members_count":0,"spec":null,"template_name":"PRUEBA probar_conexiones","connection_id":null,"orientation":"same","warnings":[],"errors":[{"code":"VALIDATION_TOKEN_INVALID","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","path":"nodes[N4].validation_token"}],"node":"N4","elements_count":0}],"failed_count":1,"document":"HANGAR_PRUEBA_sondeo","plan_id":"22c89cba-8f39-42e3-a1f1-e42ebd1b31ca","created_count":0,"operation":"batch_create","started_utc":"2026-10-23T17:27:12.2040411Z","stopped":false,"batch_id":"22c89cba-8f39-42e3-a1f1-e42ebd1b31ca","with_warnings_count":0,"skipped_count":0,"stop_on_error":false,"plan_summary_text":"Creadas 0 conexiones, 1 fall\u00f3. Pulsa Crear otra vez para reintentar la fallida. Ocultos: 5 barras sueltas.","stopped_at":null,"deleted_elements_count":0,"undo_entries":"one","plan_creatable_count":1,"duration_ms":3,"rolled_back_count":0,"updated_count":0,"connection_ids":[],"restored_members_count":0,"plan_summary":{"untyped":5,"failed":1},"finished_utc":"2026-10-23T17:27:12.2058629Z","deleted_count":0,"summary_text":"Lote 22 ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"data":{"nodes":[],"failed_count":0,"document":"HANGAR_PRUEBA_sondeo","plan_id":null,"created_count":0,"operation":"batch_delete","started_utc":"2026-10-23T17:27:12.2538386Z","stopped":false,"batch_id":"00000000-0000-0000-0000-000000000000","with_warnings_count":0,"skipped_count":0,"stop_on_error":false,"plan_summary_text":null,"stopped_at":null,"deleted_elements_count":0,"undo_entries":"one","plan_creatable_count":null,"duration_ms":4,"rolled_back_count":0,"updated_count":0,"connection_ids":[],"restored_members_count":0,"plan_summary":null,"finished_utc":"2026-10-23T17:27:12.2585102Z","deleted_count":0,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s."},"meta":{"duration_ms":6,"operation":"batch_delete","addin_version":"0.12.2"},"warnings":[{"code":"BATCH_EMPTY","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","path":"batch_id"}],"ok":true,"errors":[]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado 22c89cba-8f39-42e3-a1f1-e42ebd1b31ca
Cuerpo:
{"data": {"removed_labels": 0, "remaining_markers": 0, "remaining_plans": 0, "remaining_labels": 0, "discarded_plan_id": "22c89cba-8f39-42e3-a1f1-e42ebd1b31ca", "removed_marks": 0}, "meta": {"duration_ms": 32, "operation": "batch_plan_discard", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"data":null,"meta":{"duration_ms":0,"operation":"catalog_apply","addin_version":"0.12.2"},"warnings":[],"ok":false,"errors":[{"code":"TEMPLATE_NOT_FOUND","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","path":"template_id"}]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 18421ee8-c2bf-43f7-a613-4f4ad488126f
Cuerpo:
{"data":{"name":"PRUEBA probar_conexiones","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\18421ee8-c2bf-43f7-a613-4f4ad488126f.json","deleted_template_id":"18421ee8-c2bf-43f7-a613-4f4ad488126f"},"meta":{"duration_ms":3,"operation":"catalog_delete","addin_version":"0.12.2"},"warnings":[],"ok":true,"errors":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"data":{"hidden_text":"8 sin cord\u00f3n, 17 barras sueltas","nodes":[{"template_id":null,"is_valid":false,"is_mirrored":false,"attempts":["Nudo K cordon superior \u2192 same: ranura 0 (diagonal -135.6\u00b0) sin barra; ranura 1 (diagonal -45.0\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.9\u00b0)","Nudo K cordon superior \u2192 mirror_x: ranura 0 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0); ranura 1 (diagonal -135.0\u00b0) sin barra","Nudo K cordon superior \u2192 mirror_y: ranura 0 (diagonal 135.6\u00b0) sin barra; ranura 1 (diagonal 45.0\u00b0) sin barra; barras sobrantes: 1249630","Nudo K cordon superior \u2192 both: ranura 0 (diagonal 44.4\u00b0) sin barra; ranura 1 (diagonal 135.0\u00b0) sin barra; barras sobrantes: 1249630","Nudo tipico Detalle D \u2192 same: ranura 0 (diagonal 136.9\u00b0) sin barra; ranura 1 (diagonal 44.4\u00b0) sin barra; ranura 2 (diagonal -135.6\u00b0) sin barra; barras sobrantes: 1249630","Nudo tipico Detalle D \u2192 mirror_x: ranura 0 (diagonal 43.1\u00b0) sin barra; ranura 1 (diagonal 135.6\u00b0) sin barra; ranura 2 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0)","Nudo tipico Detalle D \u2192 mirror_y: ranura 0 (diagonal -136.9\u00b0) sin barra; ranura 1 (diagonal -44.4\u00b0) \u2192 barra 1249630 (-43.1\u00b0, desv\u00edo 1.3\u00b0); ranura 2 (diagonal 135.6\u00b0) sin barra","Nudo tipico Detalle D \u2192 both: ranura 0 (diagonal -43.1\u00b0) \u2192 barra 124 ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado add46342-cbac-4763-9063-83227110c5df etiquetas quitadas=0
Cuerpo:
{"data": {"removed_labels": 0, "remaining_markers": 0, "remaining_plans": 0, "remaining_labels": 0, "discarded_plan_id": "add46342-cbac-4763-9063-83227110c5df", "removed_marks": 0}, "meta": {"duration_ms": 12, "operation": "batch_plan_discard", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"data": {"bolt_stacks": [{"member_element_id": 1245531, "bolt_length_mm": 50.799999999999997, "bolts": 12, "plates": 2, "gap_mm": 0, "grip_mm": 25.399999999999999, "length_source": "computed_from_grip"}], "is_valid": true, "calculated_values": {"frame_y": [0, 0, 1], "kind": "splice", "chord_direction_reversed": true, "eccentricity_mm": 0, "frame_z": [0, -1, 0], "origin_mm": [-4180.5, 17204.200000000001, 19933], "axis_distance_mm": 0, "frame_x": [1, 0, 0]}, "errors_count": 0, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8", "warnings_count": 0}, "meta": {"duration_ms": 6, "operation": "validate", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"data":{"bolt_stacks":[],"is_valid":true,"calculated_values":{"frame_y":[0,0,1],"kind":"node","chord_direction_reversed":true,"eccentricity_mm":54.899999999999999,"frame_z":[0,-1,0],"origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"axis_distance_mm":0.089999999999999997,"frame_x":[1,0,0]},"errors_count":0,"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9","warnings_count":2},"meta":{"duration_ms":8,"operation":"validate","addin_version":"0.12.2"},"warnings":[{"code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","path":"members[0].axis_offset_mm"},{"code":"MEMBER_ECCENTRIC","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 152.4 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","path":"members[1].axis_offset_mm"}],"ok":true,"errors":[]}
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"data":{"hidden_text":"2 barras sueltas","nodes":[{"template_id":null,"is_valid":false,"is_mirrored":false,"attempts":[],"warnings":[],"status_detail":"Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.","errors_count":0,"existing_connection_id":null,"label_index":null,"chord_continuous":false,"chord_element_id":0,"warnings_count":0,"errors":[],"existing_batch_id":null,"member_element_ids":[1245531],"is_marked":false,"has_spec_override":false,"orientation":null,"status":"untyped","signature":"1 barra(s) sin marco","element_ids":[1245531],"created_connection_id":null,"spec":null,"name":"N1","members":[],"kind":"node","max_deviation_deg":null,"status_text":"\u25cb Barra suelta (no es nudo)","starter":false,"through_element_ids":[],"replaces_existing":false,"chord_type_name":null,"validation_token":null,"template_name":null,"work_point_mm":[-14656,17204.299999999999,19933],"advice":"No es un nudo: nada que hacer","splice_element_id":null,"ghost_element_id":null,"connection_type":null,"actions":[],"match":null,"is_manual":false,"visible_by_default":false,"color_name":null,"eccentricity_mm":0,"color_rgb":null,"marker_element_id":null},{"template_id":null,"is_valid":true,"is_mirrored":false,"attempts":[],"warnings":[],"status_detail":"Sin plantilla: arranque end_plate (Detalle C) (Detalle C del plano para HSS12X8X1/2 (choices.json)).","errors_count":0,"existing_connection_id":null,"label_index":null,"chord_continuous":false,"chord_element_id":1245530,"warnings_cou ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado 7a5e7971-428e-4381-ab98-eef191b1cdd8
Cuerpo:
{"data": {"removed_labels": 0, "remaining_markers": 0, "remaining_plans": 0, "remaining_labels": 0, "discarded_plan_id": "7a5e7971-428e-4381-ab98-eef191b1cdd8", "removed_marks": 0}, "meta": {"duration_ms": 13, "operation": "batch_plan_discard", "addin_version": "0.12.2"}, "warnings": [], "ok": true, "errors": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.12.2
Cuerpo:
{
  "data": {
    "revit": {
      "version_name": "Autodesk Revit 2027",
      "version_number": "2027",
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_build": "27.2.0.39"
    },
    "addin_version": "0.12.2",
    "document": {
      "is_workshared": false,
      "title": "HANGAR_PRUEBA_sondeo",
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "is_modifiable": false,
      "is_family": false,
      "is_read_only": false
    },
    "has_uidocument": true,
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
    "dotnet": {
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "framework": ".NET 10.0.12",
      "load_context": "Default"
    },
    "spec_version": "1.0",
    "backend": "advancesteel"
  },
  "meta": {
    "duration_ms": 3,
    "operation": "ping",
    "addin_version": "0.12.2"
  },
  "warnings": [],
  "ok": true,
  "errors": []
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "data": {
    "name": "PRUEBA probar_conexiones",
    "match": {
      "assignments": [
        {
          "model_angle_deg": 136.92,
          "template_angle_deg": 136.92,
          "side": "+Y",
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "element_id": 1249630,
          "deviation_deg": 0,
          "slot": 0,
          "profile_policy": "warn",
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "role": "diagonal"
        },
        {
          "model_angle_deg": 44.37,
          "template_angle_deg": 44.37,
          "side": "+Y",
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "element_id": 1249631,
          "deviation_deg": 0,
          "slot": 1,
          "profile_policy": "warn",
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "role": "diagonal"
        },
        {
          "model_angle_deg": -135.63,
          "template_angle_deg": -135.63,
          "side": "-Y",
          "model_type_name": "HSS2-1-2X2-1-2X3-16 64x64",
          "element_id": 1249636,
          "deviation_deg": 0,
          "slot": 2,
          "profile_policy": "warn",
          "template_profile": "HSS2-1/2X2-1/2X3/16",
          "role": "diagonal"
        }
      ],
      "matched_count": 3,
      "description": "same: ranura 0 (diagonal 136.9°) → barra 1249630 (136.9°, desvío 0.0°); ranura 1 (diagonal 44.4°) → barra 1249631 (44.4°, desvío 0.0°); ranura 2 (diagonal -135.6°) → barra 1249636 (-135.6°, desvío 0.0°)",
  ...
======================================================================
Resultado: 38/38 pruebas correctas

```

## 12c-6 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 417 ms ==
{
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "templates":  [
                                   {
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "chord_profile":  "HSS12X8X1/2",
                                       "name":  "Nudo K cordon superior",
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "description":  null,
                                       "tags":  [

                                                ],
                                       "members_count":  2,
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "connection_type":  "direct_weld"
                                   },
                                   {
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "name":  "Nudo tipico Detalle D",
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "members_count":  3,
                                       "origin_drawing":  "Detalle D",
                                       "connection_type":  "gusset_node"
                                   }
                               ],
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates_count":  2
             },
    "meta":  {
                 "duration_ms":  1,
                 "operation":  "catalog_list",
                 "addin_version":  "0.12.2"
             },
    "warnings":  [

                 ],
    "ok":  true,
    "errors":  [

               ]
}

```

## 12c-6 log del dia

```text

{"ts":"2026-10-23T11:44:02.4035332-05:00","record":{"event":"startup","addin_version":"0.12.2","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-23T11:59:01.7080174-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:16:58.2480716-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:16:58.2593506-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 56652883-6a1d-44da-a01e-80e0b591d292","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-23T12:16:58.2735418-05:00","record":{"event":"direct_weld_member","element_id":1249631,"axis_offset_mm":
-27.44,"setback_ideal_mm":145.28,"axis_setback_mm":177.73,"face_y_mm":-101.6,"footprint_mm":90.8,"chord_element_id":nul
l}}
{"ts":"2026-10-23T12:16:58.2820525-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura directa 
barra 1249631","points":2,"closed":false,"size_mm":5,"properties":["Thickness=5","MainEffectiveThroat=5","DoubleEffecti
veThroat=5","IsClosed=False"],"connected_to":[],"connect":"sin Connect (ninguna plancha de Advance Steel bajo la 
soldadura)","units":"mm"}}
{"ts":"2026-10-23T12:16:58.2849354-05:00","record":{"event":"direct_weld_member","element_id":1249632,"axis_offset_mm":
27.44,"setback_ideal_mm":143.68,"axis_setback_mm":175.43,"face_y_mm":-101.6,"footprint_mm":89.8,"chord_element_id":null
}}
{"ts":"2026-10-23T12:16:58.2862187-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura directa 
barra 1249632","points":2,"closed":false,"size_mm":5,"properties":["Thickness=5","MainEffectiveThroat=5","DoubleEffecti
veThroat=5","IsClosed=False"],"connected_to":[],"connect":"sin Connect (ninguna plancha de Advance Steel bajo la 
soldadura)","units":"mm"}}
{"ts":"2026-10-23T12:16:58.3225912-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 56652883-6a1d-44da-a01e-80e0b591d292","pending":2,"pending_welds":2,"missing":0,"new_elements":["1322804:Welds:St
eelProxyElement","1322805:Welds:SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-23T12:16:58.4639948-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\Nudo K cordon superior-nudo-1245533.json","template_id":
"91adac27-d196-4c98-88db-6579aa991be4","connection_id":"56652883-6a1d-44da-a01e-80e0b591d292","created_elements":2,"mod
ified_members":2,"backend":"advancesteel","validation_token_prefix":"d2a7fa77bea6c69e","warnings":["END_CUT_APPROXIMATE
D","REVIT_WARNING","REVIT_WARNING"],"duration_ms":238}}
{"ts":"2026-10-23T12:18:09.5896111-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:18:09.6136491-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
borrar 56652883-6a1d-44da-a01e-80e0b591d292","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-23T12:18:09.6371479-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
borrar 56652883-6a1d-44da-a01e-80e0b591d292","pending":0,"pending_welds":0,"missing":0,"new_elements":[],"is_modifiable
_after":true}}
{"ts":"2026-10-23T12:19:41.4685202-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:19:41.4773291-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
crear 0caa4a6d-79ad-45aa-a899-2b82749b4c11","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-23T12:19:41.8373307-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura plancha 
tramo 1245530","points":4,"closed":true,"size_mm":6,"properties":["Thickness=6","MainEffectiveThroat=6","DoubleEffectiv
eThroat=6","IsClosed=True"],"connected_to":["Plancha de extremo tramo 1245530"],"connect":"Connect con 1 pieza(s), 
kInShop","units":"mm"}}
{"ts":"2026-10-23T12:19:41.8391433-05:00","record":{"event":"advance_steel_weld_written","name":"Soldadura plancha 
tramo 1245531","points":4,"closed":true,"size_mm":6,"properties":["Thickness=6","MainEffectiveThroat=6","DoubleEffectiv
eThroat=6","IsClosed=True"],"connected_to":["Plancha de extremo tramo 1245531"],"connect":"Connect con 1 pieza(s), 
kInShop","units":"mm"}}
{"ts":"2026-10-23T12:19:41.8396795-05:00","record":{"event":"end_plate_built","first_segment":1245530,"second_segment":
1245531,"plate_mm":[430,330,12.7],"corner_cut_mm":60,"gap_mm":0,"setbacks_mm":[12.3,12.3],"rows":4,"bolts":12,"bolt_len
gth_mm":50.8,"tube_footprint_mm":[304.8,203.2],"plate_frame_z":[0.9999999999973376,-2.307565366741405E-06,-0]}}
{"ts":"2026-10-23T12:19:41.9294146-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
crear 0caa4a6d-79ad-45aa-a899-2b82749b4c11","pending":8,"pending_welds":2,"missing":0,"new_elements":["1322877:Plates:S
teelProxyElement","1322878:Plates:SteelProxyElement","1322879:Bolts:SteelProxyElement","1322880:Bolts:SteelProxyElement
","1322881:Bolts:SteelProxyElement","1322882:Bolts:SteelProxyElement","1322883:Welds:SteelProxyElement","1322884:Welds:
SteelProxyElement"],"is_modifiable_after":true}}
{"ts":"2026-10-23T12:19:42.1880956-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\conectar-empalme-1245530.json","template_id":null,"conne
ction_id":"0caa4a6d-79ad-45aa-a899-2b82749b4c11","created_elements":8,"modified_members":2,"backend":"advancesteel","va
lidation_token_prefix":"d8b12345edf9348a","warnings":["BOLT_NUTS_APPROXIMATED","REVIT_WARNING","REVIT_WARNING","REVIT_W
ARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING","REVIT_WARNING"],"duration_ms":722}}
{"ts":"2026-10-23T12:21:30.0049637-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:21:30.0145606-05:00","record":{"event":"fabrication_transaction_open","name":"MotorConexiones: 
borrar 0caa4a6d-79ad-45aa-a899-2b82749b4c11","revit_transaction_started":true,"is_modifiable_after":true}}
{"ts":"2026-10-23T12:21:30.0317525-05:00","record":{"event":"fabrication_transaction_commit","name":"MotorConexiones: 
borrar 0caa4a6d-79ad-45aa-a899-2b82749b4c11","pending":0,"pending_welds":0,"missing":0,"new_elements":[],"is_modifiable
_after":true}}
{"ts":"2026-10-23T12:27:11.0589435-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:27:11.5298942-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}
{"ts":"2026-10-23T12:27:12.2270064-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"node\": \"N4\", \"validation_token\": 
\"0000000000000000000000000000000000000000000000000000000000000000\"}], \"plan_id\": \"22c89cba-8f39-42e3-a1f1-e42ebd1b
31ca\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAILED"],"duration_ms":14}}
{"ts":"2026-10-23T12:27:15.0262056-05:00","record":{"event":"advance_steel_weld_support","weld_line":true,"connect":tru
e,"location":"kInShop"}}



```

## 12c-7 git status antes del commit

```text
?? docs/fases/capturas/fase12c-01-soldadura-k.png
?? docs/fases/capturas/fase12c-02-empalme-frente.png
?? docs/fases/capturas/fase12c-03-sondeo28.png
?? docs/fases/resultados-fase-12c.md

```
