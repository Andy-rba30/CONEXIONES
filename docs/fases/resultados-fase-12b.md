# Resultados de la ronda 12b

Fecha: 2026-10-21T15:09:38


## 12b-1 git

```text
777cd0f Cierre de la Fase 12 y ronda 12b (add-in 0.12.1): la 0.12.0 contrastada en Revit, validate con kind y eccentricity_mm, excentricidad real 54,9 (KNode55, fixture K, simulador, guía) y los sondeos 25, 26 y 27 (corte con BeamShortening, WeldLine, perfil del cordón)
?? docs/fases/resultados-fase-12b.md

```

## 12b-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 1.59 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 1.6 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 1.6 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:11.74
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   268, Omitido:     0, Total:   268, Duración: 752 ms - MotorConexiones.Tests.dll (net10.0)

```

## 12b-1 revit cerrado

```text

```

## 12b-1 deploy

```text
== MotorConexiones 0.12.1.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, config\catalog.json, config\choices.json, docs\guide.md
Catalogo:    C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 1; miniaturas .png copiadas: 0)
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 12b-1 instalar-conn

```text
== MotorConexiones: archivos conn_* instalados en C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension ==
- copiado revit_mcp\conexiones.py (25 rutas @api.route)
- copiado tools\conn_tools.py (23 herramientas @mcp.tool)
- startup.py: ya tenia register_conn_routes
- tools\__init__.py: ya tenia register_conn_tools
Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha.

```

## 12b-1 version de la dll

```text
0.12.1.0

```

## 12b-1 sondeos nuevos desplegados

```text

Name                      Length
----                      ------
25-corte-advance-steel.py  22909
26-soldadura-weldline.py   21914
27-perfil-cordon.py         9173



```

## 12b-2 ping

```text
== conn/ping -> HTTP 200 en 332 ms ==
{
    "meta":  {
                 "duration_ms":  10,
                 "operation":  "ping",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "spec_version":  "1.0",
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
                 "has_uidocument":  true,
                 "backend":  "advancesteel",
                 "document":  {
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
                                  "title":  "HANGAR_PRUEBA_sondeo",
                                  "is_read_only":  false,
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "is_workshared":  false
                              },
                 "addin_version":  "0.12.1",
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "load_context":  "Default",
                                "framework":  ".NET 10.0.12"
                            },
                 "revit":  {
                               "language":  "English_USA",
                               "sub_version_number":  "2027.2",
                               "version_number":  "2027",
                               "version_name":  "Autodesk Revit 2027",
                               "version_build":  "27.2.0.39"
                           }
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-2 list

```text
== conn/list -> HTTP 200 en 56 ms ==
{
    "meta":  {
                 "duration_ms":  9,
                 "operation":  "list",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "connections_count":  0,
                 "batches":  {

                             },
                 "connections":  [

                                 ],
                 "total_count":  0,
                 "batch_id":  null
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-2 sondeo 17 antes

```text
== 17-marcas-plan.py -> HTTP 200 en 1796 ms ==
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

## 12b-2 batch_plan_discard del plan viejo

```text
== conn/batch_plan_discard -> HTTP 200 en 506 ms ==
{
    "meta":  {
                 "duration_ms":  83,
                 "operation":  "batch_plan_discard",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "removed_labels":  0,
                 "remaining_labels":  0,
                 "remaining_markers":  0,
                 "discarded_plans":  0,
                 "removed_markers":  0
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-2 sondeo 17 despues

```text
== 17-marcas-plan.py -> HTTP 200 en 1430 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1249510] HSS3X3X1/4 | centro mm (-9417.5, -17195.8, 17423.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1249510]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322486] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17-exportada.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 12b-2 sondeo 21 despues

```text
== 21-etiquetas-addin.py -> HTTP 200 en 392 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 0
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 75 BMP
   etiqueta-ambar-11-sel.bmp (3126 bytes)
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-16.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-23.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-30.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 0 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 12b-3 sondeo 27 perfil del cordon

```text
== 27-perfil-cordon.py -> HTTP 500 en 1449 ms ==
ERROR: NameError: name 'uidoc' is not defined
TIPO: NameError
SALIDA PARCIAL:
=== 27-perfil-cordon (ronda 12b) ===

TRACEBACK:
Traceback (most recent call last):
  File "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\revit_mcp\code_execution.py", line 357, in cuerpo
    exec(code_to_execute, namespace)
  File "<string>", line 120, in <module>
NameError: name 'uidoc' is not defined


```

## 12b-3 sondeo 27 perfil del cordon (SinTransaccion)

```text
== 27-perfil-cordon.py -> HTTP 200 en 499 ms ==
=== 27-perfil-cordon (ronda 12b) ===
0) Documento HANGAR_PRUEBA_sondeo | barras: 1245533, 1249631, 1249510

1) Barra [1245533] familia 'HSS-Hollow Structural Section' tipo 'HSS12X8X1/2'
   eje: (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0)
   TIPO (seccion):
     STRUCTURAL_SECTION_SHAPE: 0 | Not Defined
     STRUCTURAL_SECTION_COMMON_WIDTH: (no lo tiene)
     STRUCTURAL_SECTION_COMMON_HEIGHT: (no lo tiene)
     STRUCTURAL_SECTION_HSS_OUTERFILLET: (no lo tiene)
     STRUCTURAL_SECTION_COMMON_NOMINAL_WEIGHT: (no lo tiene)
     con nombre corto: Ht = 304.80 mm; b = 203.20 mm; t = 11.81 mm
     cadena de RevitModelFacts: ancho = COMMON_WIDTH o b/Width/B/Ancho; alto = COMMON_HEIGHT o d/h/H/Height/Alto
   EJEMPLAR (giro y justificacion):
     STRUCTURAL_BEND_DIR_ANGLE: 90.00 grados
     Y_JUSTIFICATION: 2 | Origin
     Z_JUSTIFICATION: 1 | Center
     Y_OFFSET_VALUE: 0.00 mm
     Z_OFFSET_VALUE: 0.00 mm
     STRUCTURAL_BEAM_ORIENTATION: 0 | Normal
     START_EXTENSION: 0.00 mm
     END_EXTENSION: 0.00 mm
     'Cross-Section Rotation' = 90.00 grados
     'y Justification' = 2 | Origin
     'z Justification' = 1 | Center
     'y Offset Value' = 0.00 mm
     'z Offset Value' = 0.00 mm
   SOLIDO: canto EN EL PLANO de la cercha 203.2 mm | ancho PERPENDICULAR al plano 304.8 mm | caras 18 | eje (-304.8, 0.0, 0.0) | en plano (0.0, 0.0, -304.8) | fuera (0.0, 304.8, 0.0)

2) Barra [1249631] familia 'HSS2-1-2X2-1-2X3-16 64x64' tipo 'HSS2-1-2X2-1-2X3-16 64x64'
   eje: (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9)
   TIPO (seccion):
     STRUCTURAL_SECTION_SHAPE: 14 | Rectangular HSS
     STRUCTURAL_SECTION_COMMON_WIDTH: 0.2083 (crudo) | 2.500"
     STRUCTURAL_SECTION_COMMON_HEIGHT: 0.2083 (crudo) | 2.500"
     STRUCTURAL_SECTION_HSS_OUTERFILLET: 0.0000 (crudo) | 0.000"
     STRUCTURAL_SECTION_COMMON_NOMINAL_WEIGHT: 0.0000 (crudo) | 0.00 lbf/ft
     con nombre corto: Height = 0.2083 (crudo) | 2.500"; Width = 0.2083 (crudo) | 2.500"
     cadena de RevitModelFacts: ancho = COMMON_WIDTH o b/Width/B/Ancho; alto = COMMON_HEIGHT o d/h/H/Height/Alto
   EJEMPLAR (giro y justificacion):
     STRUCTURAL_BEND_DIR_ANGLE: -180.00 grados
     Y_JUSTIFICATION: 2 | Origin
     Z_JUSTIFICATION: 1 | Center
     Y_OFFSET_VALUE: 0.00 mm
     Z_OFFSET_VALUE: 0.00 mm
     STRUCTURAL_BEAM_ORIENTATION: 0 | Normal
     START_EXTENSION: 0.00 mm
     END_EXTENSION: 69.20 mm
     'Cross-Section Rotation' = -180.00 grados
     'y Justification' = 2 | Origin
     'z Justification' = 1 | Center
     'y Offset Value' = 0.00 mm
     'z Offset Value' = 0.00 mm
   SOLIDO: canto EN EL PLANO de la cercha 63.5 mm | ancho PERPENDICULAR al plano 63.5 mm | caras 10 | eje (217.9, 0.0, 213.2) | en plano (-213.2, 0.0, 217.9) | fuera (-0.0, 304.8, -0.0)

3) Barra [1249510] familia 'HSS-Hollow Structural Section' tipo 'HSS3X3X1/4'
   eje: (-4437.3, -17195.7, 17423.0) -> (-14397.6, -17195.8, 17423.0)
   TIPO (seccion):
     STRUCTURAL_SECTION_SHAPE: 0 | Not Defined
     STRUCTURAL_SECTION_COMMON_WIDTH: (no lo tiene)
     STRUCTURAL_SECTION_COMMON_HEIGHT: (no lo tiene)
     STRUCTURAL_SECTION_HSS_OUTERFILLET: (no lo tiene)
     STRUCTURAL_SECTION_COMMON_NOMINAL_WEIGHT: (no lo tiene)
     con nombre corto: Ht = 76.20 mm; b = 76.20 mm; t = 5.92 mm
     cadena de RevitModelFacts: ancho = COMMON_WIDTH o b/Width/B/Ancho; alto = COMMON_HEIGHT o d/h/H/Height/Alto
   EJEMPLAR (giro y justificacion):
     STRUCTURAL_BEND_DIR_ANGLE: 180.00 grados
     Y_JUSTIFICATION: 2 | Origin
     Z_JUSTIFICATION: 1 | Center
     Y_OFFSET_VALUE: 0.00 mm
     Z_OFFSET_VALUE: 0.00 mm
     STRUCTURAL_BEAM_ORIENTATION: 0 | Normal
     START_EXTENSION: -3.03 mm
     END_EXTENSION: -1.73 mm
     'Cross-Section Rotation' = 180.00 grados
     'y Justification' = 2 | Origin
     'z Justification' = 1 | Center
     'y Offset Value' = 0.00 mm
     'z Offset Value' = 0.00 mm
   SOLIDO: canto EN EL PLANO de la cercha 76.2 mm | ancho PERPENDICULAR al plano 76.2 mm | caras 18 | eje (-304.8, -0.0, 0.0) | en plano (-0.0, 0.0, -304.8) | fuera (-0.0, 304.8, 0.0)

4) Como leerlo: si el solido del 1245533 da canto en el plano ~203 y ancho perpendicular ~305, el 8" esta en el plano de la cercha (lo que supuso la Fase 12) y el add-in debe tomar como 'ancho' (perpendicular al plano) el numero del tipo que valga 304,8 y como 'canto' el de 203,2, girados si Cross-Section Rotation no es 0. Copia este bloque entero en resultados-fase-12b.md.
=== fin 27-perfil-cordon ===


```

## 12b-3 sondeo 26 soldadura WeldLine

```text
== 26-soldadura-weldline.py -> HTTP 200 en 2303 ms ==
=== 26-soldadura-weldline (ronda 12b) ===
   modo: CANCELAR al final (nada queda)
1) Transaccion Autodesk.SteelConnectionsDB.FabricationTransaction; WeldLine(Point3d[], Vector3d, Vector3d): True; WeldPoint(Point3d, Vector3d, Vector3d): True; FilerObject en ASObjectsMgd
   WeldLine, propiedades escribibles: AdditionalData, Assembly, AssemblyLocation, AssemblyUsedForNumbering, Coating, CoatingUsedForNumbering, CompoundReprMode, DennotationUsedForNumbering, Denotation, DoubleEffectiveThroat, DoublePreparationDepth, DoubleRootOpening, DoubleWeldText, DrivingPassiveConstructionObject, ExplicitQuantity, HolesUsedForNumbering, IsClosed, IsMainPart, ItemNumber, ItemNumberUsedForNumbering, JointTransferID, Layer, MainEffectiveThroat, MainPartNumber, MainPartPrefix, MainPartUsedForBOM, MainPartUsedForCollisionCheck, MainPartUsedForNumbering, MainPreparationDepth, MainRootOpening, MainWeldText, Material, MaterialUsedForNumbering, Note, NoteUsedForNumbering, Pitch, Prefix, PreliminaryPartNumber, PreliminaryPartPrefix, ReprMode, Role, RoleUsedForNumbering, SeamDistance, SinglePartNumber, SinglePartPrefix, SinglePartUsedForBOM, SinglePartUsedForCollisionCheck, SinglePartUsedForNumbering, SingleSeamLengt
   enum escribible AssemblyLocation (eAssemblyLocation): kOnSite, kSiteDrill, kInShop, kWrong, kUnknown
2) Barra [1249631] HSS2-1-2X2-1-2X3-16 64x64: extremo (-9354.7, -17195.8, 19884.9), eje (217.9, 0.0, 213.2), huella 63.5 x 63.5 mm
3) FabricationTransaction abierta. doc.IsModifiable=True
   puente: AddFabricationInformationForRevitElements -> 0
   puente: GetFabricationUniqueID -> 445dc2d4-a73f-439a-b741-5f3d0aaa7cd1
   barra de Advance Steel: NO encontrada (ver los intentos)
   escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)
   WeldLine creada: DoubleEffectiveThroat=5.0; IsClosed=True; MainEffectiveThroat=5.0; Thickness=5.0
   WeldLine.Connect: OK con 1 pieza(s), kInShop
   WeldLine: WriteToDb OK (contorno cerrado de 64 x 64, 5.0 mm); Length=254.0; IsClosed=True
   WeldPoint creado: DoubleEffectiveThroat=5.0; IsClosed=True; MainEffectiveThroat=5.0; Thickness=5.0
   WeldPoint: WriteToDb OK
4) Elementos nuevos con la transaccion abierta: 0
5) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False
6) Elementos nuevos tras cerrar: 0 (se esperan 0)
7) RESUMEN: WeldLine: WriteToDb OK (contorno cerrado de 64 x 64, 5.0 mm); Length=254.0; IsClosed=True | WeldLine.Connect: OK con 1 pieza(s), kInShop | WeldPoint: WriteToDb OK | WeldPoint.Connect: OK con 1 pieza(s), kInShop | barra AS: NO encontrada (ver los intentos) | plancha de apoyo: escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)
=== fin 26-soldadura-weldline ===


```

## 12b-3 sondeo 25 corte Advance Steel

```text
== 25-corte-advance-steel.py -> HTTP 200 en 893 ms ==
=== 25-corte-advance-steel (ronda 12b) ===
   modo: CANCELAR al final (nada queda)
1) Cordon [1245533] (-4180.2, -17195.8, 19933.0) -> (-14655.2, -17195.7, 19933.0); diagonal [1249631] (-11856.5, -17195.8, 17437.3) -> (-9354.7, -17195.8, 19884.9); extremo que llega: fin ((-9354.7, -17195.8, 19884.9)); angulo con el cordon 44.4 grados; extension actual 69.1996468112 mm
2) Transaccion Autodesk.SteelConnectionsDB.FabricationTransaction; BeamShortening(eEnd, Double): si; BeamMultiContourNotch(Beam, eEnd, Point3d[], Vector3d, Vector3d): True; eEnd (Autodesk.AdvanceSteel.Modelling.Beam+eEnd): kMid, kStart, kEnd, kNoEnd
   BeamShortening, propiedades escribibles: AngleOnY, AngleOnZ, CS, CompoundReprMode, CutStraight, CutStraightOffset, CutStraightRelativeOffset, CutStraightType, DrivingPassiveConstructionObject, End, InsLength, JointTransferID, Layer, ObjectIndex, ReprMode, Role, SkipEraseEnvironment, UseGap
   enum End (eEnd): kMid, kStart, kEnd, kNoEnd
3) FabricationTransaction abierta. doc.IsModifiable=True
   puente: AddFabricationInformationForRevitElements -> 0
   puente: GetFabricationUniqueID -> 19b0eff1-50ed-42cc-a31c-d2e89d8dfbf3
   NO encontrada: sin viga de Advance Steel no hay corte por esta via (copia los intentos del puente)
4) Elementos nuevos con la transaccion abierta: 0; extension del extremo ahora: 69.1996468112 mm
5) CancelTransaction() hecho: nada debe quedar en la copia.
   doc.IsModifiable tras la operacion: False | extension del extremo: 69.1996468112 mm
6) Elementos nuevos tras cerrar: 0 (se esperan 0)
7) RESUMEN: viga AS: NO encontrada: sin viga de Advance Steel no hay corte por esta via (copia los intentos del puente)
=== fin 25-corte-advance-steel ===


```

## 12b-3 sondeo 12 tras los sondeos

```text
== 12-fase3-borrar.py -> HTTP 200 en 133 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 12 ms | errores=- | avisos=-
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

## 12b-3 sondeo 13 tras los sondeos

```text
== 13-limpiar-fase1.py -> HTTP 200 en 44 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```

## 12b-6 validate fixture K

```text
== conn/validate -> HTTP 200 en 469 ms ==
{
    "meta":  {
                 "duration_ms":  14,
                 "operation":  "validate",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "warnings_count":  2,
                 "validation_token":  "d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9",
                 "is_valid":  true,
                 "bolt_stacks":  [

                                 ],
                 "calculated_values":  {
                                           "chord_direction_reversed":  true,
                                           "kind":  "node",
                                           "origin_mm":  [
                                                             -9278.1000000000004,
                                                             -17195.700000000001,
                                                             19933
                                                         ],
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
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "eccentricity_mm":  54.899999999999999,
                                           "axis_distance_mm":  0.089999999999999997
                                       },
                 "errors_count":  0
             },
    "ok":  true,
    "warnings":  [
                     {
                         "path":  "members[0].axis_offset_mm",
                         "message":  "El eje del miembro [1249631] corta el cordón a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 101.6 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "code":  "MEMBER_ECCENTRIC"
                     },
                     {
                         "path":  "members[1].axis_offset_mm",
                         "message":  "El eje del miembro [1249632] corta el cordón a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 101.6 mm). Cada barra se recorta y se suelda sobre su propio eje.",
                         "hint":  "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE.",
                         "code":  "MEMBER_ECCENTRIC"
                     }
                 ],
    "errors":  [

               ]
}

```

## 12b-6 validate fixture C

```text
== conn/validate -> HTTP 200 en 37 ms ==
{
    "meta":  {
                 "duration_ms":  8,
                 "operation":  "validate",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "warnings_count":  0,
                 "validation_token":  "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8",
                 "is_valid":  true,
                 "bolt_stacks":  [
                                     {
                                         "bolt_length_mm":  50.799999999999997,
                                         "grip_mm":  25.399999999999999,
                                         "member_element_id":  1245531,
                                         "bolts":  12,
                                         "length_source":  "computed_from_grip",
                                         "plates":  2,
                                         "gap_mm":  0
                                     }
                                 ],
                 "calculated_values":  {
                                           "chord_direction_reversed":  true,
                                           "kind":  "splice",
                                           "origin_mm":  [
                                                             -4180.5,
                                                             17204.200000000001,
                                                             19933
                                                         ],
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
                                           "frame_y":  [
                                                           0,
                                                           0,
                                                           1
                                                       ],
                                           "eccentricity_mm":  0,
                                           "axis_distance_mm":  0
                                       },
                 "errors_count":  0
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-6 node_info nudo en K

```text
== conn/node_info -> HTTP 200 en 43 ms ==
{
    "meta":  {
                 "duration_ms":  23,
                 "operation":  "node_info",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "members":  [
                                 {
                                     "slope_deg":  0,
                                     "type":  "HSS12X8X1/2",
                                     "length_mm":  10475,
                                     "end_mm":  [
                                                    -14655.200000000001,
                                                    -17195.700000000001,
                                                    19933
                                                ],
                                     "end_gap_mm":  0,
                                     "wall_mm":  11.810000000000000,
                                     "reaches_node":  true,
                                     "angle_in_plane_deg":  -180,
                                     "width_mm":  203.19999999999999,
                                     "element_id":  1245533,
                                     "node_end":  0,
                                     "axis_cut_mm":  [
                                                         -9278.1000000000004,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "angle_to_chord_deg":  0,
                                     "side":  "chord",
                                     "structural_type":  "Beam",
                                     "start_mm":  [
                                                      -4180.1999999999998,
                                                      -17195.799999999999,
                                                      19933
                                                  ],
                                     "material":  "Steel ASTM A500, Grade B, Rectangular and Square",
                                     "is_chord":  true,
                                     "axis_offset_mm":  0,
                                     "height_mm":  203.19999999999999,
                                     "family":  "HSS-Hollow Structural Section"
                                 },
                                 {
                                     "slope_deg":  44.369999999999997,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3500,
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "end_gap_mm":  90.400000000000006,
                                     "wall_mm":  0,
                                     "reaches_node":  true,
                                     "angle_in_plane_deg":  -135.59999999999999,
                                     "width_mm":  63.5,
                                     "element_id":  1249631,
                                     "node_end":  1,
                                     "axis_cut_mm":  [
                                                         -9305.5,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "angle_to_chord_deg":  44.399999999999999,
                                     "side":  "-Y",
                                     "structural_type":  "Beam",
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "material":  "Material IFC (190-40-140)",
                                     "is_chord":  false,
                                     "axis_offset_mm":  -27.399999999999999,
                                     "height_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64"
                                 },
                                 {
                                     "slope_deg":  45,
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3546,
                                     "end_mm":  [
                                                    -6770.3999999999996,
                                                    -17195.799999999999,
                                                    17452.700000000001
                                                ],
                                     "end_gap_mm":  27.100000000000001,
                                     "wall_mm":  0,
                                     "reaches_node":  true,
                                     "angle_in_plane_deg":  -45,
                                     "width_mm":  63.5,
                                     "element_id":  1249632,
                                     "node_end":  0,
                                     "axis_cut_mm":  [
                                                         -9250.7000000000007,
                                                         -17195.700000000001,
                                                         19933
                                                     ],
                                     "angle_to_chord_deg":  45,
                                     "side":  "-Y",
                                     "structural_type":  "Beam",
                                     "start_mm":  [
                                                      -9277.7999999999993,
                                                      -17195.799999999999,
                                                      19960.099999999999
                                                  ],
                                     "material":  "Material IFC (190-40-140)",
                                     "is_chord":  false,
                                     "axis_offset_mm":  27.399999999999999,
                                     "height_mm":  63.5,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64"
                                 }
                             ],
                 "frame_rule":  "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
                 "chord_element_id":  1245533,
                 "chord_direction_reversed":  true,
                 "y_axis":  [
                                0,
                                -5E-06,
                                1
                            ],
                 "kind":  "node",
                 "origin_mm":  [
                                   -9278.1000000000004,
                                   -17195.700000000001,
                                   19933
                               ],
                 "z_axis":  [
                                -2E-06,
                                -1,
                                -5E-06
                            ],
                 "existing_connections":  [

                                          ],
                 "x_axis":  [
                                1,
                                -2E-06,
                                0
                            ],
                 "eccentricity_mm":  54.899999999999999,
                 "axis_distance_mm":  0.089999999999999997
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-6 list

```text
== conn/list -> HTTP 200 en 20 ms ==
{
    "meta":  {
                 "duration_ms":  5,
                 "operation":  "list",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "connections_count":  0,
                 "batches":  {

                             },
                 "connections":  [

                                 ],
                 "total_count":  0,
                 "batch_id":  null
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-6 sondeo 17 final

```text
== 17-marcas-plan.py -> HTTP 200 en 731 ms ==
=== 17-marcas-plan ===
1) Vista activa: {3D} (ThreeD) | plantilla=False | admite overrides=True
2) Marcadores de plan (ApplicationId MotorConexiones.Plan) en el modelo: 0
3) Barra de prueba: [1245532] HSS12X8X1/2 | centro mm (944.8, -17195.8, 19933.0)
4) Patron solido: [20] <Solid fill>
5) Override puesto en [1245532]: color leido (230, 25, 75) | grosor 10 | patron superficie 20
6) DirectShape admite Modelos genericos: True
6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0
7) Marcador creado: [1322683] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1245532 | caja mm 160 x 160 x 160
8) Captura: D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase8-01-sondeo17-exportada.png
9) Tras limpiar: color valido=False | marcador existe=False
10) TransactionGroup deshecho: el modelo queda como estaba.
=== fin 17-marcas-plan ===


```

## 12b-6 sondeo 21 final

```text
== 21-etiquetas-addin.py -> HTTP 200 en 224 ms ==
=== 21-etiquetas-addin (Fase 10) ===
1) Documento: HANGAR_PRUEBA_sondeo | vista activa: {3D} (ThreeD)
2) Controles en el lienzo (GetAll): 0
3) Servicio TemporaryGraphicsHandlerService: 2 servidor(es) registrados, activos: 70cd437f-c0eb-4274-8b6a-2269ae2d54bf, 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d
3) Servidor del add-in 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: registrado=True | activo=True
3) Servidor del sondeo 19 7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60: registrado=False | activo=False
   70cd437f-c0eb-4274-8b6a-2269ae2d54bf: RIA Temporary Graphics Server (ADSK)
   3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d: MotorConexiones: etiquetas del plan (ARBA)
4) Carpeta de etiquetas: C:\Users\Public\MotorConexiones\etiquetas (sin tildes ni espacios: True) | 77 BMP
   etiqueta-ambar-11-sel.bmp (3126 bytes)
   etiqueta-ambar-11.bmp (3126 bytes)
   etiqueta-ambar-13.bmp (3126 bytes)
   etiqueta-ambar-16.bmp (3126 bytes)
   etiqueta-ambar-18.bmp (3126 bytes)
   etiqueta-ambar-20.bmp (3126 bytes)
   etiqueta-ambar-23.bmp (3126 bytes)
   etiqueta-ambar-25.bmp (3126 bytes)
   etiqueta-ambar-27.bmp (3126 bytes)
   etiqueta-ambar-30.bmp (3126 bytes)
   etiqueta-ambar-32.bmp (3126 bytes)
   etiqueta-ambar-34.bmp (3126 bytes)
5) Marcadores de plan en el modelo: 0 (de ellos, cartelas fantasma: 0)
6) Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.
=== fin 21-etiquetas-addin ===


```

## 12b-6 servidor 8000 escuchando

```text

LocalAddress LocalPort OwningProcess
------------ --------- -------------
127.0.0.1         8000         36764



```

## 12b-6 probar_conexiones --puente

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.12.1 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
Cuerpo:
{"meta":{"duration_ms":3,"operation":"ping","addin_version":"0.12.1"},"data":{"spec_version":"1.0","operations":["batch_create","batch_delete","batch_plan","batch_plan_discard","batch_plan_get","catalog_apply","catalog_delete","catalog_get","catalog_list","catalog_save","create","delete","find_profile","get","guide","list","node_info","ping","preview","schema","types","update","validate"],"has_uidocument":true,"backend":"advancesteel","document":{"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt","title":"HANGAR_PRUEBA_sondeo","is_read_only":false,"is_family":false,"is_modifiable":false,"is_workshared":false},"addin_version":"0.12.1","dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","load_context":"Default","framework":".NET 10.0.12"},"revit":{"language":"English_USA","sub_version_number":"2027.2","version_number":"2027","version_name":"Autodesk Revit 2027","version_build":"27.2.0.39"}},"ok":true,"warnings":[],"errors":[]}
======================================================================
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 31680 caracteres
Cuerpo:
{"meta":{"duration_ms":16,"operation":"guide","addin_version":"0.12.1"},"data":{"guide_markdown":"# Gu\u00eda para la IA: crear conexiones de acero con MotorConexiones\n\nEsta gu\u00eda la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el\nadd-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la secci\u00f3n 11 del encargo.\n\n## 0. Qu\u00e9 hace el add-in y qu\u00e9 no\n\n- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro\n  de las barras. Usa Advance Steel si est\u00e1 disponible (placas y pernos nativos, categor\u00edas Plates/Bolts) y, si no,\n  s\u00f3lidos DirectShape de reserva. `conn_ping` dice cu\u00e1l (`backend`).\n- No dise\u00f1a ni verifica resistencias: si el usuario pregunta si la conexi\u00f3n \"aguanta\", dile que eso no lo hace el add-in.\n- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.\n- v1 solo sabe crear `gusset_node` (nudo con cartela, cord\u00f3n HSS continuo y diagonales/montantes HSS ranurados y\n  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no est\u00e1n en v1.\n- Todas las operaciones de escritura son at\u00f3micas (o se crea todo o nada) y quedan como una sola entrada de deshacer en\n  Revit (`MotorConexiones: <operaci\u00f3n> <id>`). Ninguna abre ventanas.\n\n## 1. Flujo obliga ...
======================================================================
4. GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)  [OK]  HTTP 200, ok=True, tipos=['direct_weld', 'end_plate', 'gusset_node']
Cuerpo:
{"meta":{"duration_ms":1,"operation":"types","addin_version":"0.12.1"},"data":{"connection_types":[{"type_name":"direct_weld","description":"Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cord\u00f3n y se suelda en todo el contorno, sin cartela ni placas (los nudos del cord\u00f3n superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cord\u00f3n."},{"type_name":"end_plate","description":"Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones)."},{"type_name":"gusset_node","description":"Nudo de cercha con cartela plana, cord\u00f3n continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada."}]},"ok":true,"warnings":[],"errors":[]}
======================================================================
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=1
Cuerpo:
{"meta":{"duration_ms":1,"operation":"schema","addin_version":"0.12.1"},"data":{"example":{"uncertain_fields":[],"node":{"element_ids":[1249510,1249630,1249631,1249636]},"spec_version":"1.0","members":[{"role":"diagonal","end_setback_mm":180.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":45.0}],"connection_type":"gusset_node","source":{"scale":"1/10","drawing":"Detalle D"},"chord":{"element_id":1249510,"profile":"HSS3X3X1/4","continuous":true},"gusset":{"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280.0],[315.0,210.0],[315.0,-40.0],[-35.0,-250.0],[-125.0,-250.0],[-250.0,-115.0],[-250.0,210.0]]},"chord_interface":"through_slot","thickness_label":"3/8\"","weld_to_chord":{"size_mm":5.0,"all_around":true,"type":"fillet"},"height_mm":530.0,"width_mm":565.0,"thickness_mm":9.5250000000000004},"dimension_chains":[{"label":"borde superior","values_mm":[75.0,420.0,70.0],"expected_total_mm":565.0}]},"json_schema":{"title":"GussetNodeConnectionSpec","required":["spec_version","connection_type","node","chord","gusset","members"],"additionalProperties":false,"properties":{"uncertain_fields":{"items":{"additionalProperties":false,"required":["path","reason"],"properties":{"path":{"type":"string"},"user_confirmed_value":{},"reason":{"type":"string"}},"type":"object"},"type":"array"},"node":{"additionalProperties":false,"required":[ ...
======================================================================
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"schema","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":"type","message":"El tipo de conexi\u00f3n 'no_existe' no est\u00e1 registrado.","hint":"Tipos disponibles: direct_weld, end_plate, gusset_node","code":"UNKNOWN_OPERATION"}]}
======================================================================
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64'] sugerencias=[]
Cuerpo:
{"meta": {"duration_ms": 12, "operation": "find_profile", "addin_version": "0.12.1"}, "data": {"query": "HSS2-1/2X2-1/2X3/16", "matches": [{"type_name": "HSS2-1-2X2-1-2X3-16 64x64", "exact_match": false, "family_name": "HSS2-1-2X2-1-2X3-16 64x64"}], "suggestions": [], "matched_count": 1, "total_profiles_in_model": 29}, "ok": true, "warnings": [], "errors": []}
======================================================================
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11869.4, -17195.8, 17423]
Cuerpo:
{"meta": {"duration_ms": 10, "operation": "node_info", "addin_version": "0.12.1"}, "data": {"members": [{"slope_deg": 0, "type": "HSS3X3X1/4", "length_mm": 9960.2999999999993, "end_mm": [-14397.600000000000, -17195.799999999999, 17423], "end_gap_mm": 0, "wall_mm": 5.9199999999999999, "reaches_node": true, "angle_in_plane_deg": 0, "width_mm": 76.200000000000003, "element_id": 1249510, "node_end": 1, "axis_cut_mm": [-11869.400000000000, -17195.799999999999, 17423], "angle_to_chord_deg": 0, "side": "chord", "structural_type": "Beam", "start_mm": [-4437.3000000000002, -17195.700000000001, 17423], "material": "Steel ASTM A500, Grade B, Rectangular and Square", "is_chord": true, "axis_offset_mm": 0, "height_mm": 76.200000000000003, "family": "HSS-Hollow Structural Section"}, {"slope_deg": 43.079999999999998, "type": "HSS2-1-2X2-1-2X3-16 64x64", "length_mm": 3568, "end_mm": [-11930.600000000000, -17195.799999999999, 17481.799999999999], "end_gap_mm": 84.900000000000006, "wall_mm": 0, "reaches_node": true, "angle_in_plane_deg": 136.90000000000001, "width_mm": 63.5, "element_id": 1249630, "node_end": 1, "axis_cut_mm": [-11867.700000000001, -17195.799999999999, 17423], "angle_to_chord_deg": 43.100000000000001, "side": "+Y", "structural_type": "Beam", "start_mm": [-14536.799999999999, -17195.799999999999, 19918.799999999999], "material": "Material IFC (190-40-140)", "is_chord": false, "axis_offset_mm": 1.7000000000000000, "height_mm": 63.5, "family": "HSS2-1-2X2-1-2X3-16 64x64"}, {"slop ...
======================================================================
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], is_valid=True token=f50f27b4fa18...
Cuerpo:
{"meta":{"duration_ms":13,"operation":"validate","addin_version":"0.12.1"},"data":{"warnings_count":1,"validation_token":"f50f27b4fa18a976f6cc72415d40268f4fcecdecb349573aefd79f045d10a4b1","is_valid":true,"bolt_stacks":[{"bolt_length_mm":44.450000000000003,"grip_mm":19.524999999999999,"member_element_id":1249636,"length_source":"computed_from_grip","gusset_face":"+z"}],"calculated_values":{"chord_direction_reversed":true,"kind":"node","origin_mm":[-11869.400000000000,-17195.799999999999,17423],"frame_x":[1,0,0],"frame_z":[0,-1,0],"frame_y":[0,0,1],"eccentricity_mm":3.3999999999999999,"axis_distance_mm":0.080000000000000002},"errors_count":0},"ok":true,"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[]}
======================================================================
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"duration_ms":13,"operation":"validate","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[{"path":"dimension_chains[0].values_mm","message":"La cadena de cotas 'borde superior' suma 547.0 mm pero se esperaba 565.0 mm (diferencia 18.0 mm > tolerancia 1 mm).","hint":"Ajusta los valores de la cadena para que sumen exactamente 565.0 mm o corrige expected_total_mm.","code":"DIMENSION_CHAIN_MISMATCH"}]}
======================================================================
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False, errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], avisos=['ANGLE_DIFFERS_FROM_MODEL'], sin token
Cuerpo:
{"meta":{"duration_ms":9,"operation":"validate","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[{"path":"uncertain_fields[0].user_confirmed_value","message":"La duda en 'members[1].profile' no ha sido confirmada por el usuario: La etiqueta del montante est\u00e1 cortada en la imagen","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","code":"UNRESOLVED_UNCERTAINTY"},{"path":"uncertain_fields[1].user_confirmed_value","message":"La duda en 'gusset.chord_interface' no ha sido confirmada por el usuario: El dibujo no muestra con claridad c\u00f3mo se une la cartela al cord\u00f3n","hint":"Confirma el valor con el usuario y as\u00edgnalo en user_confirmed_value antes de validar.","code":"UNRESOLVED_UNCERTAINTY"}]}
======================================================================
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"knife_plates": 1, "first_member_element_id": 1249630, "dry_run": true, "connection_type": "gusset_node", "weld_lines": 6, "chord_element_id": 1249510, "bolts": 4, "backend": "advancesteel", "working_point_mm": [-11869.4, -17195.8, 17423], "members_modified": 3, "gusset_plates": 1}
Cuerpo:
{"meta": {"duration_ms": 7, "operation": "preview", "addin_version": "0.12.1"}, "data": {"summary": {"knife_plates": 1, "first_member_element_id": 1249630, "dry_run": true, "connection_type": "gusset_node", "weld_lines": 6, "chord_element_id": 1249510, "bolts": 4, "backend": "advancesteel", "working_point_mm": [-11869.400000000000, -17195.799999999999, 17423], "members_modified": 3, "gusset_plates": 1}, "elements_to_create": [{"chord_interface": "through_slot", "kind": "gusset_plate", "thickness_label": "3/8\"", "height_mm": 530, "width_mm": 565, "thickness_mm": 9.5250000000000004, "vertices_count": 8}, {"for_member_id": 1249630, "weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface"}, {"for_member_id": 1249631, "weld_size_mm": 5, "slot_length_mm": 150, "kind": "welded_slot_interface"}, {"insertion_mm": 80, "offset_from_gusset_plane_mm": 9.7620000000000005, "length_mm": 170, "kind": "knife_plate", "width_mm": 140, "for_member_id": 1249636, "thickness_mm": 10, "gusset_face": "+z"}, {"edge_mm": 40, "grip_mm": 19.524999999999999, "rows": 2, "diameter_mm": 15.875, "count": 4, "length_mm": 44.450000000000003, "length_source": "computed_from_grip", "kind": "bolt_group", "spacing_mm": 60, "for_member_id": 1249636, "columns": 2}], "members_to_modify": [{"role": "diagonal", "setback_mm": 180, "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo", "element_id": 1249630, "new_extension_mm": -95.099999999999994, "end": "end", ...
======================================================================
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False, errores=['VALIDATION_TOKEN_INVALID']
Cuerpo:
{"meta":{"duration_ms":1,"operation":"create","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":"validation_token","message":"validation_token es obligatorio para crear una conexi\u00f3n.","hint":"Llama primero a conn_validate para validar la especificaci\u00f3n y obtener el token.","code":"VALIDATION_TOKEN_INVALID"}]}
======================================================================
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
Cuerpo:
{"meta": {"duration_ms": 4, "operation": "list", "addin_version": "0.12.1"}, "data": {"connections_count": 0, "batches": {}, "connections": [], "total_count": 0, "batch_id": null}, "ok": true, "warnings": [], "errors": []}
======================================================================
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":7,"operation":"get","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":"connection_id","message":"No se encontr\u00f3 ninguna conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Usa conn_list para verificar las conexiones guardadas en el modelo.","code":"ELEMENT_NOT_FOUND"}]}
======================================================================
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['ELEMENT_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":5,"operation":"delete","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":"connection_id","message":"No se encontr\u00f3 la conexi\u00f3n con ID '00000000-0000-0000-0000-000000000000'.","hint":"Verifica los IDs disponibles con conn_list.","code":"ELEMENT_NOT_FOUND"}]}
======================================================================
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"no_existe","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":null,"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: batch_create, batch_delete, batch_plan, batch_plan_discard, batch_plan_get, catalog_apply, catalog_delete, catalog_get, catalog_list, catalog_save, create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate.","code":"UNKNOWN_OPERATION"}]}
======================================================================
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=2 carpeta=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo
Cuerpo:
{"meta":{"duration_ms":3,"operation":"catalog_list","addin_version":"0.12.1"},"data":{"catalog_folder":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo","shared_catalog_folder":"D:\\Proyectos C#\\CONEXIONES\\catalog","templates_count":2,"templates":[{"tags":[],"pattern":"2 barra(s): diagonal -135,6\u00b0 -Y \u00b7 diagonal -45,0\u00b0 -Y","name":"Nudo K cordon superior","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json","connection_type":"direct_weld","template_id":"91adac27-d196-4c98-88db-6579aa991be4","description":null,"chord_profile":"HSS12X8X1/2","created_utc":"2026-10-21T16:49:07.9104459Z","origin_drawing":"Especificaci\u00f3n de arranque (Conectar)","origin_document":"HANGAR_PRUEBA_sondeo","members_count":2},{"tags":["hangar","cercha","HSS"],"pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","name":"Nudo tipico Detalle D","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json","connection_type":"gusset_node","template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","chord_profile":"HSS3X3X1/4","created_utc":"2026-10-04T22:01:50.5585233Z","origin_drawing":"Detalle D","origin_document":"HANGAR_PRUEBA_sondeo","members_count":3}]}, ...
======================================================================
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  HTTP 200, ok=True, avisos=['ANGLE_DIFFERS_FROM_MODEL'], template_id=7e8b3ede-559f-4778-bd45-de0ce859096b barras=3 archivo=C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\catalogo\7e8b3ede-559f-4778-bd45-de0ce859096b.json
Cuerpo:
{"meta":{"duration_ms":40,"operation":"catalog_save","addin_version":"0.12.1"},"data":{"origin":{"element_ids":[1249510,1249630,1249631,1249636],"document":"HANGAR_PRUEBA_sondeo","connection_id":null,"drawing":"Detalle D"},"member_pattern":[{"role":"diagonal","slot":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","angle_deg":136.91999999999999,"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16"},{"role":"diagonal","slot":1,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","angle_deg":44.369999999999997,"side":"+Y","profile":"HSS2-1/2X2-1/2X3/16"},{"role":"diagonal","slot":2,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","angle_deg":-135.63000000000000,"side":"-Y","profile":"HSS2-1/2X2-1/2X3/16"}],"file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\7e8b3ede-559f-4778-bd45-de0ce859096b.json","name":"PRUEBA probar_conexiones","shared_file":null,"template_id":"7e8b3ede-559f-4778-bd45-de0ce859096b","chord_profile":"HSS3X3X1/4","members_count":3,"matching":{"angle_tolerance_deg":10,"allow_mirror":true}},"ok":true,"warnings":[{"path":"members[0].expected_angle_deg","message":"El \u00e1ngulo del plano (45.0\u00b0, inclinaci\u00f3n 45.0\u00b0 respecto al cord\u00f3n) difiere del de la barra en el modelo (136.9\u00b0, inclinaci\u00f3n 43.1\u00b0) por 1.9\u00b0 > 1\u00b0.","hint":"Verifica la geometr\u00eda en el modelo o en el plano.","code":"ANGLE_DIFFERS_FROM_MODEL"}],"errors":[]}
======================================================================
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  HTTP 200, ok=True, nombre=PRUEBA probar_conexiones patrón=[(0, 136.92, '+Y'), (1, 44.37, '+Y'), (2, -135.63, '-Y')]
Cuerpo:
{"meta":{"duration_ms":1,"operation":"catalog_get","addin_version":"0.12.1"},"data":{"pattern":"3 barra(s): diagonal 136,9\u00b0 +Y \u00b7 diagonal 44,4\u00b0 +Y \u00b7 diagonal -135,6\u00b0 -Y","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\7e8b3ede-559f-4778-bd45-de0ce859096b.json","name":"PRUEBA probar_conexiones","template_id":"7e8b3ede-559f-4778-bd45-de0ce859096b","template":{"tags":["prueba"],"matching":{"angle_tolerance_deg":10,"allow_mirror":true},"spec_template":{"uncertain_fields":[],"spec_version":"1.0","members":[{"role":"diagonal","slot":0,"end_setback_mm":180.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":45.0},{"role":"diagonal","slot":1,"end_setback_mm":60.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":45.0},{"role":"diagonal","slot":2,"end_setback_mm":260.0,"attachment":{"plate":{"insertion_mm":80.0,"length_mm":170.0,"thickness_label":"PL10","width_mm":140.0,"thickness_mm":10.0},"bolts":{"edge_mm":40.0,"first_row_from_plate_end_mm":40.0,"rows":2,"diameter_mm":15.875,"diameter_label":"5/8\"","spacing_mm":60.0,"columns":2},"weld_plate_to_member":{"size_mm":5.0,"all_around":true,"type":"fillet"},"type":"bolted_knife_plate"},"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":45.0 ...
======================================================================
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  HTTP 200, ok=True, orientación=same desvío_máx=0 token=e3b47172cd52...
Cuerpo:
{"meta":{"duration_ms":25,"operation":"catalog_apply","addin_version":"0.12.1"},"data":{"spec":{"uncertain_fields":[],"spec_version":"1.0","node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"role":"diagonal","end_setback_mm":180.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":43.100000000000001},{"role":"diagonal","end_setback_mm":60.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":44.399999999999999},{"role":"diagonal","end_setback_mm":260.0,"attachment":{"plate":{"insertion_mm":80.0,"length_mm":170.0,"thickness_label":"PL10","width_mm":140.0,"thickness_mm":10.0},"bolts":{"edge_mm":40.0,"first_row_from_plate_end_mm":40.0,"rows":2,"diameter_mm":15.875,"diameter_label":"5/8\"","spacing_mm":60.0,"columns":2},"weld_plate_to_member":{"size_mm":5.0,"all_around":true,"type":"fillet"},"type":"bolted_knife_plate"},"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":44.399999999999999}],"connection_type":"gusset_node","source":{"template_id":"7e8b3ede-559f-4778-bd45-de0ce859096b","scale":"1/10","drawing":"Detalle D"},"chord":{"profile":"HSS3X3X1/4","element_id":1249510,"continuous":true},"gusset":{"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280. ...
======================================================================
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones candidatas=2 | Plantilla propuesta: PRUEBA probar_conexiones · igual · desvío máx. 0,0°. Otras que encajan: Nudo tipico Detalle D (igua
Cuerpo:
{"meta":{"duration_ms":25,"operation":"catalog_apply","addin_version":"0.12.1"},"data":{"spec":{"uncertain_fields":[],"spec_version":"1.0","node":{"element_ids":[1249510,1249630,1249631,1249636]},"members":[{"role":"diagonal","end_setback_mm":180.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"element_id":1249630,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":43.100000000000001},{"role":"diagonal","end_setback_mm":60.0,"attachment":{"weld":{"size_mm":5.0,"all_around":true,"type":"fillet"},"slot_length_mm":150.0,"type":"welded_slot"},"element_id":1249631,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":44.399999999999999},{"role":"diagonal","end_setback_mm":260.0,"attachment":{"plate":{"insertion_mm":80.0,"length_mm":170.0,"thickness_label":"PL10","width_mm":140.0,"thickness_mm":10.0},"bolts":{"edge_mm":40.0,"first_row_from_plate_end_mm":40.0,"rows":2,"diameter_mm":15.875,"diameter_label":"5/8\"","spacing_mm":60.0,"columns":2},"weld_plate_to_member":{"size_mm":5.0,"all_around":true,"type":"fillet"},"type":"bolted_knife_plate"},"element_id":1249636,"profile":"HSS2-1/2X2-1/2X3/16","expected_angle_deg":44.399999999999999}],"connection_type":"gusset_node","source":{"template_id":"7e8b3ede-559f-4778-bd45-de0ce859096b","scale":"1/10","drawing":"Detalle D"},"chord":{"profile":"HSS3X3X1/4","element_id":1249510,"continuous":true},"gusset":{"outline":{"mode":"polygon","points_mm":[[-175.0,280.0],[245.0,280. ...
======================================================================
22. POST /conn/batch/plan/ (mark:false) -> plan_id, el nudo del fixture ready con token  [OK]  HTTP 200, ok=True, plan_id=f2b67a1a-0d79-4a52-8fd5-8a27c022c9d1 nudos=6 resumen={'ready': 1, 'untyped': 5} nudo=N4 ready same | ● Listo | verde | Se creará 1 conexión con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.
Cuerpo:
{"meta":{"duration_ms":36,"operation":"batch_plan","addin_version":"0.12.1"},"data":{"hidden_text":"5 barras sueltas","marked_view_id":null,"creatable_count":1,"description":"6 nudo(s): 5 untyped, 1 ready.","overrides":{"replace_existing":false,"spec":{},"add_node":{},"exclude":[],"merge":[],"template":{},"add_member":{},"remove_member":{},"chord":{},"split":{}},"selection_count":4,"summary_text":"Se crear\u00e1 1 conexi\u00f3n con PRUEBA probar_conexiones (1 igual, 0 en espejo). Ocultos: 5 barras sueltas.","updated_utc":"2026-10-21T20:38:45.5334020Z","nodes":[{"warnings_count":0,"is_manual":false,"template_name":null,"is_mirrored":false,"kind":"node","existing_connection_id":null,"signature":"1 barra(s) sin marco","errors_count":0,"marker_element_id":null,"replaces_existing":false,"validation_token":null,"status_text":"\u25cb Barra suelta (no es nudo)","splice_element_id":null,"match":null,"actions":[],"errors":[],"element_ids":[1249630],"orientation":null,"status":"untyped","chord_type_name":null,"existing_batch_id":null,"through_element_ids":[],"member_element_ids":[1249630],"spec":null,"is_valid":false,"max_deviation_deg":null,"attempts":[],"warnings":[],"chord_continuous":false,"work_point_mm":[-14536.799999999999,-17195.799999999999,19918.799999999999],"connection_type":null,"advice":"No es un nudo: nada que hacer","created_connection_id":null,"has_spec_override":false,"color_name":null,"is_marked":false,"label_index":null,"ghost_element_id":null,"name":"N1","visible_by ...
======================================================================
23. POST /conn/batch/plan/get/ node <nudo del fixture> -> el nudo con su token  [OK]  HTTP 200, ok=True, N4 ready ● Listo token=3be5e4d6d247...
Cuerpo:
{"meta":{"duration_ms":0,"operation":"batch_plan_get","addin_version":"0.12.1"},"data":{"node":{"warnings_count":0,"is_manual":false,"template_name":"PRUEBA probar_conexiones","is_mirrored":false,"kind":"node","existing_connection_id":null,"signature":"2 +Y (137, 44) \u00b7 1 -Y (-136)","errors_count":0,"marker_element_id":null,"replaces_existing":false,"validation_token":"3be5e4d6d2478600af102e8e4d35aee45e91cf47a6b44b466deddb6ab7578376","status_text":"\u25cf Listo","splice_element_id":null,"match":{"assignments":[{"role":"diagonal","slot":0,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","template_angle_deg":136.91999999999999,"element_id":1249630,"side":"+Y","model_angle_deg":136.91999999999999,"template_profile":"HSS2-1/2X2-1/2X3/16"},{"role":"diagonal","slot":1,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","template_angle_deg":44.369999999999997,"element_id":1249631,"side":"+Y","model_angle_deg":44.369999999999997,"template_profile":"HSS2-1/2X2-1/2X3/16"},{"role":"diagonal","slot":2,"deviation_deg":0,"model_type_name":"HSS2-1-2X2-1-2X3-16 64x64","profile_policy":"warn","template_angle_deg":-135.63000000000000,"element_id":1249636,"side":"-Y","model_angle_deg":-135.63000000000000,"template_profile":"HSS2-1/2X2-1/2X3/16"}],"orientation":"same","description":"same: ranura 0 (diagonal 136.9\u00b0) \u2192 barra 1249630 (136.9\u00b0, desv\u00edo 0.0\u00b0); ranura 1 (diagonal 44.4\u00b0) \u2192 ba ...
======================================================================
24. POST /conn/batch/create/ con un token FALSO -> el nudo falla (VALIDATION_TOKEN_INVALID), nada creado  [OK]  HTTP 200, ok=True, avisos=['BATCH_NODE_FAILED'], N4: falló (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)) | Lote f2b67a1a: 0 conexiones creadas, 1 falló (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo)). 0 s.
Cuerpo:
{"meta":{"duration_ms":15,"operation":"batch_create","addin_version":"0.12.1"},"data":{"restored_members_count":0,"updated_count":0,"with_warnings_count":0,"plan_summary":{"failed":1,"untyped":5},"summary_text":"Lote f2b67a1a: 0 conexiones creadas, 1 fall\u00f3 (N4: VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo)). 0 s.","stop_on_error":false,"nodes":[{"errors":[{"path":"nodes[N4].validation_token","message":"El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo).","hint":"Relee el plan con conn_batch_plan_get y pasa el validation_token actual de N4.","code":"VALIDATION_TOKEN_INVALID"}],"spec":null,"node":"N4","orientation":"same","reason":"token distinto del plan","restored_members_count":0,"description":"N4: fall\u00f3 (VALIDATION_TOKEN_INVALID: El validation_token de N4 no es el que tiene el plan: el plan se replanific\u00f3 (o el token es de otro nudo))","template_name":"PRUEBA probar_conexiones","duration_ms":0,"warnings":[],"outcome":"failed","connection_id":null,"elements_count":0}],"connection_ids":[],"rolled_back_count":0,"plan_summary_text":"Creadas 0 conexiones, 1 fall\u00f3. Pulsa Crear otra vez para reintentar la fallida. Ocultos: 5 barras sueltas.","finished_utc":"2026-10-21T20:38:45.6567531Z","plan_creatable_count":1,"operation":"batch_create","duration_ms":3,"deleted_count":0,"document":"HANGAR_PRUEBA_sondeo","started ...
======================================================================
25. POST /conn/batch/delete/ lote inexistente -> deleted_count 0 y aviso BATCH_EMPTY  [OK]  HTTP 200, ok=True, avisos=['BATCH_EMPTY'], borradas=0 avisos=['BATCH_EMPTY']
Cuerpo:
{"meta":{"duration_ms":6,"operation":"batch_delete","addin_version":"0.12.1"},"data":{"restored_members_count":0,"updated_count":0,"with_warnings_count":0,"plan_summary":null,"summary_text":"Lote 00000000: 0 conexiones borradas (0 elementos, 0 barras restauradas). 0 s.","stop_on_error":false,"nodes":[],"connection_ids":[],"rolled_back_count":0,"plan_summary_text":null,"finished_utc":"2026-10-21T20:38:45.7005843Z","plan_creatable_count":null,"operation":"batch_delete","duration_ms":5,"deleted_count":0,"document":"HANGAR_PRUEBA_sondeo","started_utc":"2026-10-21T20:38:45.6954180Z","undo_entries":"one","stopped_at":null,"failed_count":0,"batch_id":"00000000-0000-0000-0000-000000000000","plan_id":null,"deleted_elements_count":0,"skipped_count":0,"stopped":false,"created_count":0},"ok":true,"warnings":[{"path":"batch_id","message":"No hay ninguna conexi\u00f3n con batch_id '00000000-0000-0000-0000-000000000000' en el modelo: nada que borrar.","hint":"conn_list ense\u00f1a el batch_id de cada conexi\u00f3n (data.batches cuenta por lote).","code":"BATCH_EMPTY"}],"errors":[]}
======================================================================
26. POST /conn/batch/plan/discard/ -> descartado  [OK]  HTTP 200, ok=True, descartado f2b67a1a-0d79-4a52-8fd5-8a27c022c9d1
Cuerpo:
{"meta": {"duration_ms": 18, "operation": "batch_plan_discard", "addin_version": "0.12.1"}, "data": {"removed_labels": 0, "remaining_labels": 0, "remaining_plans": 0, "remaining_markers": 0, "removed_marks": 0, "discarded_plan_id": "f2b67a1a-0d79-4a52-8fd5-8a27c022c9d1"}, "ok": true, "warnings": [], "errors": []}
======================================================================
27. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]  HTTP 200, ok=False, errores=['TEMPLATE_NOT_FOUND']
Cuerpo:
{"meta":{"duration_ms":0,"operation":"catalog_apply","addin_version":"0.12.1"},"data":null,"ok":false,"warnings":[],"errors":[{"path":"template_id","message":"No existe la plantilla '00000000-0000-0000-0000-000000000000' en C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo.","hint":"Usa conn_catalog_list para ver las plantillas disponibles.","code":"TEMPLATE_NOT_FOUND"}]}
======================================================================
28. POST /conn/catalog/delete/ -> borrada  [OK]  HTTP 200, ok=True, borrada 7e8b3ede-559f-4778-bd45-de0ce859096b
Cuerpo:
{"meta":{"duration_ms":2,"operation":"catalog_delete","addin_version":"0.12.1"},"data":{"deleted_template_id":"7e8b3ede-559f-4778-bd45-de0ce859096b","file":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Local\\MotorConexiones\\catalogo\\7e8b3ede-559f-4778-bd45-de0ce859096b.json","name":"PRUEBA probar_conexiones"},"ok":true,"warnings":[],"errors":[]}
======================================================================
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED'], añadidas=63 (7 cordones, 42 barras, 14 tramos) selection_count=64 avisos=['SELECTION_EXPANDED'] | Se añadieron 63 barras que tocan la selección: 7 cordones que pasan de largo, 42 barras que llegan y 14 tramos de cordón.
Cuerpo:
{"meta":{"duration_ms":1454,"operation":"batch_plan","addin_version":"0.12.1"},"data":{"hidden_text":"8 sin cord\u00f3n, 17 barras sueltas","marked_view_id":null,"creatable_count":41,"description":"66 nudo(s): 41 ready, 17 untyped, 8 no_match.","overrides":{"replace_existing":false,"spec":{},"add_node":{},"exclude":[],"merge":[],"template":{},"add_member":{},"remove_member":{},"chord":{},"split":{}},"selection_count":64,"summary_text":"Se crear\u00e1n 41 conexiones: 30 con 2 plantillas (Nudo tipico Detalle D: 16, Nudo K cordon superior: 14) (22 iguales, 8 en espejo), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I escalado), 3 soldaduras directas (arranque). 14 avisan de perfil distinto. 2 avisan de excentricidad. 7 nudos en K est\u00e1n sobre un empalme. Ocultos: 8 sin cord\u00f3n, 17 barras sueltas.","updated_utc":"2026-10-21T20:38:46.6937683Z","nodes":[{"warnings_count":1,"is_manual":false,"template_name":null,"is_mirrored":false,"kind":"node","existing_connection_id":null,"signature":"1 -Y (-43)","errors_count":0,"marker_element_id":null,"replaces_existing":false,"validation_token":null,"status_text":"\u2716 Falta el cord\u00f3n","splice_element_id":null,"match":null,"actions":[{"label":"Cord\u00f3n\u2026","key":"chord"},{"label":"Barras\u2026","key":"members"}],"errors":[],"element_ids":[1245533,1249630],"orientation":null,"status":"no_match","chord_type_name":"HSS12X8X1/2","existing_batch_id":null,"through_element_ids":[],"member_element_ids":[1249630],"s ...
======================================================================
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado b8b40854-fc88-4220-8789-d1a9fdb2d9fe etiquetas quitadas=0
Cuerpo:
{"meta": {"duration_ms": 15, "operation": "batch_plan_discard", "addin_version": "0.12.1"}, "data": {"removed_labels": 0, "remaining_labels": 0, "remaining_plans": 0, "remaining_markers": 0, "removed_marks": 0, "discarded_plan_id": "b8b40854-fc88-4220-8789-d1a9fdb2d9fe"}, "ok": true, "warnings": [], "errors": []}
======================================================================
30b. POST /conn/validate/ del fixture detalle-C.json (end_plate, tramos 1245531 y 1245530) -> token y kind splice (Fase 12)  [OK]  HTTP 200, ok=True, kind=splice excentricidad=0 avisos=[]
Cuerpo:
{"meta": {"duration_ms": 16, "operation": "validate", "addin_version": "0.12.1"}, "data": {"warnings_count": 0, "validation_token": "c9b5ec64e95974d3306c40ee8b3d2bb7d5a17c2c2c574778d2348456972b11b8", "is_valid": true, "bolt_stacks": [{"bolt_length_mm": 50.799999999999997, "grip_mm": 25.399999999999999, "member_element_id": 1245531, "bolts": 12, "length_source": "computed_from_grip", "plates": 2, "gap_mm": 0}], "calculated_values": {"chord_direction_reversed": true, "kind": "splice", "origin_mm": [-4180.5, 17204.200000000001, 19933], "frame_x": [1, 0, 0], "frame_z": [0, -1, 0], "frame_y": [0, 0, 1], "eccentricity_mm": 0, "axis_distance_mm": 0}, "errors_count": 0}, "ok": true, "warnings": [], "errors": []}
======================================================================
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]  HTTP 200, ok=True, avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC'], kind=node excentricidad=54.9 avisos=['MEMBER_ECCENTRIC', 'MEMBER_ECCENTRIC']
Cuerpo:
{"meta":{"duration_ms":10,"operation":"validate","addin_version":"0.12.1"},"data":{"warnings_count":2,"validation_token":"d7260509d679cef58edaeb25530fda54fda72ec09887d5de359854bcf45680b9","is_valid":true,"bolt_stacks":[],"calculated_values":{"chord_direction_reversed":true,"kind":"node","origin_mm":[-9278.1000000000004,-17195.700000000001,19933],"frame_x":[1,0,0],"frame_z":[0,-1,0],"frame_y":[0,0,1],"eccentricity_mm":54.899999999999999,"axis_distance_mm":0.089999999999999997},"errors_count":0},"ok":true,"warnings":[{"path":"members[0].axis_offset_mm","message":"El eje del miembro [1249631] corta el cord\u00f3n a -27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 101.6 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","code":"MEMBER_ECCENTRIC"},{"path":"members[1].axis_offset_mm","message":"El eje del miembro [1249632] corta el cord\u00f3n a 27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta 101.6 mm). Cada barra se recorta y se suelda sobre su propio eje.","hint":"Es un nudo en K con excentricidad (opci\u00f3n A): no hay que corregir nada; por encima de medio canto del cord\u00f3n ser\u00eda MEMBER_NOT_AT_NODE.","code":"MEMBER_ECCENTRIC"}],"errors":[]}
======================================================================
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]  HTTP 200, ok=True, nudos=3 N2 splice ● Listo arranque end_plate (Detalle C) | Se creará 1 conexión: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.
Cuerpo:
{"meta":{"duration_ms":20,"operation":"batch_plan","addin_version":"0.12.1"},"data":{"hidden_text":"2 barras sueltas","marked_view_id":null,"creatable_count":1,"description":"3 nudo(s): 2 untyped, 1 ready.","overrides":{"replace_existing":false,"spec":{},"add_node":{},"exclude":[],"merge":[],"template":{},"add_member":{},"remove_member":{},"chord":{},"split":{}},"selection_count":2,"summary_text":"Se crear\u00e1 1 conexi\u00f3n: 1 empalme (arranque Detalle C). Ocultos: 2 barras sueltas.","updated_utc":"2026-10-21T20:38:47.7982490Z","nodes":[{"warnings_count":0,"is_manual":false,"template_name":null,"is_mirrored":false,"kind":"node","existing_connection_id":null,"signature":"1 barra(s) sin marco","errors_count":0,"marker_element_id":null,"replaces_existing":false,"validation_token":null,"status_text":"\u25cb Barra suelta (no es nudo)","splice_element_id":null,"match":null,"actions":[],"errors":[],"element_ids":[1245531],"orientation":null,"status":"untyped","chord_type_name":null,"existing_batch_id":null,"through_element_ids":[],"member_element_ids":[1245531],"spec":null,"is_valid":false,"max_deviation_deg":null,"attempts":[],"warnings":[],"chord_continuous":false,"work_point_mm":[-14656,17204.299999999999,19933],"connection_type":null,"advice":"No es un nudo: nada que hacer","created_connection_id":null,"has_spec_override":false,"color_name":null,"is_marked":false,"label_index":null,"ghost_element_id":null,"name":"N1","visible_by_default":false,"color_rgb":null,"template_id": ...
======================================================================
30e. POST /conn/batch/plan/discard/ del plan del empalme -> descartado  [OK]  HTTP 200, ok=True, descartado 308795f6-1d0f-4d43-9a1d-7e1064331204
Cuerpo:
{"meta": {"duration_ms": 19, "operation": "batch_plan_discard", "addin_version": "0.12.1"}, "data": {"removed_labels": 0, "remaining_labels": 0, "remaining_plans": 0, "remaining_markers": 0, "removed_marks": 0, "discarded_plan_id": "308795f6-1d0f-4d43-9a1d-7e1064331204"}, "ok": true, "warnings": [], "errors": []}
======================================================================
31. tools/list por el puente trae las 23 herramientas conn_*  [OK]  HTTP 200, herramientas=89 conn_*=23
Cuerpo:
conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate, conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete, conn_catalog_list, conn_catalog_get, conn_catalog_save, conn_catalog_delete, conn_catalog_apply, conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard, conn_batch_create, conn_batch_delete
======================================================================
32. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.12.1
Cuerpo:
{
  "meta": {
    "duration_ms": 3,
    "operation": "ping",
    "addin_version": "0.12.1"
  },
  "data": {
    "spec_version": "1.0",
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
    "has_uidocument": true,
    "backend": "advancesteel",
    "document": {
      "path": "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA_sondeo.rvt",
      "title": "HANGAR_PRUEBA_sondeo",
      "is_read_only": false,
      "is_family": false,
      "is_modifiable": false,
      "is_workshared": false
    },
    "addin_version": "0.12.1",
    "dotnet": {
      "assembly_location": "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
      "load_context": "Default",
      "framework": ".NET 10.0.12"
    },
    "revit": {
      "language": "English_USA",
      "sub_version_number": "2027.2",
      "version_number": "2027",
      "version_name": "Autodesk Revit 2027",
      "version_build": "27.2.0.39"
    }
  },
  "ok": true,
  "warnings": [],
  "errors": []
}
======================================================================
32b. tools/call conn_catalog_apply sin template_id por el puente -> proposal.chosen (Fase 11)  [OK]  HTTP 200, ok=True elegida=PRUEBA probar_conexiones orientación=same
Cuerpo:
{
  "meta": {
    "duration_ms": 31,
    "operation": "catalog_apply",
    "addin_version": "0.12.1"
  },
  "data": {
    "spec": {
      "uncertain_fields": [],
      "spec_version": "1.0",
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
          "end_setback_mm": 180.0,
          "attachment": {
            "weld": {
              "size_mm": 5.0,
              "all_around": true,
              "type": "fillet"
            },
            "slot_length_mm": 150.0,
            "type": "welded_slot"
          },
          "element_id": 1249630,
          "profile": "HSS2-1/2X2-1/2X3/16",
          "expected_angle_deg": 43.1
        },
        {
          "role": "diagonal",
          "end_setback_mm": 60.0,
          "attachment": {
            "weld": {
              "size_mm": 5.0,
              "all_around": true,
              "type": "fillet"
            },
            "slot_length_mm": 150.0,
            "type": "welded_slot"
          },
          "element_id": 1249631,
          "profile": "HSS2-1/2X2-1/2X3/16",
          "expected_angle_deg": 44.4
        },
        {
          "role": "diagonal",
          "end_setback_mm": 260.0,
          "attachment": {
            "plate": {
              "insertion_mm": 80.0,
              "length_mm": 170.0,
              "thickness_label": "PL10",
              "width_mm": 140.0,
     ...
======================================================================
Resultado: 38/38 pruebas correctas

```

## 12b-6 catalog_list final

```text
== conn/catalog_list -> HTTP 200 en 430 ms ==
{
    "meta":  {
                 "duration_ms":  1,
                 "operation":  "catalog_list",
                 "addin_version":  "0.12.1"
             },
    "data":  {
                 "catalog_folder":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo",
                 "shared_catalog_folder":  "D:\\Proyectos C#\\CONEXIONES\\catalog",
                 "templates_count":  2,
                 "templates":  [
                                   {
                                       "tags":  [

                                                ],
                                       "pattern":  "2 barra(s): diagonal -135,6° -Y · diagonal -45,0° -Y",
                                       "name":  "Nudo K cordon superior",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\91adac27-d196-4c98-88db-6579aa991be4.json",
                                       "connection_type":  "direct_weld",
                                       "template_id":  "91adac27-d196-4c98-88db-6579aa991be4",
                                       "description":  null,
                                       "chord_profile":  "HSS12X8X1/2",
                                       "created_utc":  "2026-10-21T16:49:07.9104459Z",
                                       "origin_drawing":  "Especificación de arranque (Conectar)",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "members_count":  2
                                   },
                                   {
                                       "tags":  [
                                                    "hangar",
                                                    "cercha",
                                                    "HSS"
                                                ],
                                       "pattern":  "3 barra(s): diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y",
                                       "name":  "Nudo tipico Detalle D",
                                       "file":  "C:\\Users\\Andy Bayona Antón\\AppData\\Local\\MotorConexiones\\catalogo\\6abcf116-9b97-485f-b50d-2851ca0018cc.json",
                                       "connection_type":  "gusset_node",
                                       "template_id":  "6abcf116-9b97-485f-b50d-2851ca0018cc",
                                       "description":  "Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8",
                                       "chord_profile":  "HSS3X3X1/4",
                                       "created_utc":  "2026-10-04T22:01:50.5585233Z",
                                       "origin_drawing":  "Detalle D",
                                       "origin_document":  "HANGAR_PRUEBA_sondeo",
                                       "members_count":  3
                                   }
                               ]
             },
    "ok":  true,
    "warnings":  [

                 ],
    "errors":  [

               ]
}

```

## 12b-6 log del dia

```text

{"ts":"2026-10-21T09:38:21.6448149-05:00","record":{"event":"startup","addin_version":"0.11.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-21T09:59:41.5043283-05:00","record":{"event":"startup","addin_version":"0.11.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-21T11:41:21.0863197-05:00","record":{"event":"startup","addin_version":"0.12.0","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-21T11:45:51.7836629-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":3,"ignored":0,"members":2,"chord_element_id":1245533,"kind":"node","splice_element_id":null,"eccentricity_mm":54.9,
"templates":1,"candidates":0,"proposal":null,"starter":true,"connection_type":"direct_weld","mode":"starter","is_valid"
:true,"errors":[],"duration_ms":168}}
{"ts":"2026-10-21T11:47:18.5180689-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\conectar-nudo-1245533.json","template_id":null,"connecti
on_id":"5b5fd89b-9161-4560-92d7-4209da8a377e","created_elements":2,"modified_members":2,"backend":"advancesteel","valid
ation_token_prefix":"56ad1471db77e40c","warnings":["END_CUT_APPROXIMATED"],"duration_ms":290}}
{"ts":"2026-10-21T11:48:29.4177613-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":3,"ignored":0,"members":2,"chord_element_id":1245533,"kind":"node","splice_element_id":null,"eccentricity_mm":54.9,
"templates":1,"candidates":0,"proposal":null,"starter":true,"connection_type":"direct_weld","mode":"starter","is_valid"
:true,"errors":[],"duration_ms":26}}
{"ts":"2026-10-21T11:48:46.0867661-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter"}}
{"ts":"2026-10-21T11:48:47.9240298-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":3,"ignored":0,"members":2,"chord_element_id":1245533,"kind":"node","splice_element_id":null,"eccentricity_mm":54.9,
"templates":1,"candidates":0,"proposal":null,"starter":true,"connection_type":"direct_weld","mode":"starter","is_valid"
:true,"errors":[],"duration_ms":21}}
{"ts":"2026-10-21T11:49:09.6963634-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter"}}
{"ts":"2026-10-21T11:52:01.9476212-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":2,"ignored":0,"members":1,"chord_element_id":1245530,"kind":"splice","splice_element_id":null,"eccentricity_mm":0,"
templates":0,"candidates":0,"proposal":null,"starter":true,"connection_type":"end_plate","mode":"starter_empty_catalog"
,"is_valid":true,"errors":[],"duration_ms":38}}
{"ts":"2026-10-21T11:54:34.4461809-05:00","record":{"event":"ribbon_create","operation":"connect_ribbon","file":"C:\\Us
ers\\Andy Bayona Antón\\OneDrive\\Documentos\\MotorConexiones\\conectar-empalme-1245530.json","template_id":null,"conne
ction_id":"f370914e-94a4-441f-8bdd-c4b038d3c309","created_elements":14,"modified_members":2,"backend":"advancesteel","v
alidation_token_prefix":"d8b12345edf9348a","warnings":["BOLT_NUTS_APPROXIMATED"],"duration_ms":507}}
{"ts":"2026-10-21T11:57:15.3965481-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[]}}
{"ts":"2026-10-21T11:57:15.3979116-05:00","record":{"event":"ribbon_connect_cancelled","selection":1,"reason":"pick_can
celled"}}
{"ts":"2026-10-21T11:57:36.1053278-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[1249631,1
249636,1249510]}}
{"ts":"2026-10-21T11:57:36.1473258-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":13213}}
{"ts":"2026-10-21T11:57:36.5332785-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T11:58:19.0563513-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[1249510,1
249631,1249636,1249565]}}
{"ts":"2026-10-21T11:58:19.0844812-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":37259}}
{"ts":"2026-10-21T11:58:19.4459787-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T11:58:26.7430160-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[1249631,1
249510,1249636]}}
{"ts":"2026-10-21T11:58:26.7666940-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":2596}}
{"ts":"2026-10-21T11:58:28.4923905-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T11:58:33.0616701-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[1249631]}
}
{"ts":"2026-10-21T11:58:33.0808220-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":1,"chord_element_id":1249630,"kind":"node","splice_element_id":null,"eccentricity_mm":0,"te
mplates":2,"candidates":0,"proposal":null,"starter":true,"connection_type":"direct_weld","mode":"starter","is_valid":tr
ue,"errors":[],"duration_ms":1368}}
{"ts":"2026-10-21T11:58:34.9423113-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249630,"reas
on":"window_closed","mode":"starter"}}
{"ts":"2026-10-21T12:00:51.6788028-05:00","record":{"event":"ribbon_connect_truss","selection":10,"nodes":5}}
{"ts":"2026-10-21T12:01:52.0217633-05:00","record":{"event":"ribbon_connect_truss","selection":10,"nodes":5}}
{"ts":"2026-10-21T12:02:20.7422203-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":2,"members":1,"chord_element_id":1250207,"kind":"node","splice_element_id":null,"eccentricity_mm":0,"te
mplates":2,"candidates":0,"proposal":null,"starter":true,"connection_type":"direct_weld","mode":"starter","is_valid":fa
lse,"errors":["MEMBER_NOT_AT_NODE"],"duration_ms":15}}
{"ts":"2026-10-21T12:02:25.6333999-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1250207,"reas
on":"window_closed","mode":"starter"}}
{"ts":"2026-10-21T12:02:32.0777383-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":2,"chord_element_id":1249637,"kind":"node","splice_element_id":null,"eccentricity_mm":42.1,
"templates":2,"candidates":0,"proposal":null,"starter":true,"connection_type":"gusset_node","mode":"starter","is_valid"
:false,"errors":["MEMBER_NOT_AT_NODE"],"duration_ms":18}}
{"ts":"2026-10-21T12:02:34.1878234-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249637,"reas
on":"window_closed","mode":"starter"}}
{"ts":"2026-10-21T12:02:46.9144254-05:00","record":{"event":"ribbon_connect_truss","selection":10,"nodes":5}}
{"ts":"2026-10-21T12:03:55.5385627-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[]}}
{"ts":"2026-10-21T12:03:55.5390120-05:00","record":{"event":"ribbon_connect_cancelled","selection":1,"reason":"pick_can
celled"}}
{"ts":"2026-10-21T12:08:45.4009706-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 1eb4f785-902a-4afa-b4db-a37072912483","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.5152726-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar b8dbcd43-f16d-4223-9f05-0b8f6bb17265","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.6203795-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 9adebe50-be0d-460c-80f0-e205364b358e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.6981443-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 4cde5f9a-12da-4896-a5b3-86c78c0f2aa6","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.7995014-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 994e38c0-f8ba-48a9-8413-25fa7fc8772d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.8882318-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 509c7537-6a6c-42ed-bd99-d1609c3ab93d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:45.9739702-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 2fe4c11f-b38d-473e-bdad-28461f3cd082","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.0824215-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 83a64c13-2981-40a5-8654-f91f8c5028a3","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.1774600-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar dca546c9-2a86-48c6-9cde-3ebad26956e3","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.2680738-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 6fd5f8f5-b3e3-4397-a975-91708b520f93","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.3451854-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ff4b8802-d659-429d-9bc7-8c869d31b743","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.4395462-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 549598f9-d868-4e8d-8eb9-8a3e30b309ed","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.5452417-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 18028706-6c16-40e3-8683-1cdfeb8de515","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.6309134-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 8a553d43-1b20-40f7-b170-d56b1f608eda","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.7184764-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 87927705-79a6-485f-b323-90c2e2f1d46d","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.8027190-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar e00a1db0-20ef-4b89-9dc2-6e4a711520c6","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationOnlyTransaction..ctor(Document doc, Boolean isReadOnly, Boolean 
bAskRevit2UpdateGraphics, Boolean bAllowASChangesPropagation)\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.8890117-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f5043651-6388-4a9d-902e-ad7626b794bd","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:46.9995490-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ab9a5c5d-a663-4fbb-8178-e1e91f9707fb","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at System.Reflection.ConstructorInfo.Invoke(Object[] 
parameters)\r\n   at MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend 
owner, Document document, String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 280"}}
{"ts":"2026-10-21T12:08:47.0799511-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 19257157-e88f-489a-9bcf-11022a150a1c","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.1718078-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ead78189-a064-41ff-88a9-dd048dc1d8ae","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.2473366-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar fc9f689f-e692-473f-aa98-e495a6065f2b","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.3418649-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar bd60566a-21ff-42a2-9011-a52b900fe49f","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.4194534-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar ca7a53cd-d955-4af9-bb6d-bfb90250f929","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.5132144-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 81a54a73-e722-4910-86a9-5d816de53c30","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.6157912-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 73e65525-985a-4da6-9336-35f7f0bd0f09","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.7061815-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 50272955-17e8-4bf0-9aab-86437c276b86","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.8123429-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 09af65a5-2143-45ee-a90c-1ff23f6fc0a5","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:47.9284223-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar d1363ed3-8780-44b6-a4e3-6a7dddd76425","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.0098414-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar cd5b87b1-a9d8-4197-8bef-321f13f78824","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.1136766-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 1f68d407-5c99-4b93-9b88-69f432ab1774","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.1969608-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar b14f334e-1301-4b82-98a4-815a0fd67edb","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.2878142-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f3bdf907-6874-4139-ab66-aee7cb0c5fb0","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.3687677-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 04df2997-4f17-455f-a22a-1378cf7b1719","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.4451411-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar d076e320-b6e0-498e-a116-5494fccedc0c","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.5201958-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar a7419d94-00fc-49a4-b64d-bb853194fe0b","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.5779530-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 96418f83-1daf-428b-b94a-f204a821c173","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.6744256-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 23960cf8-00a2-4839-8bf8-ea8f6d759da0","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.7763419-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 7235620e-0970-4510-9f4c-4234b5b40a3e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:48.8717675-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 487354a9-d777-47e1-86fd-508df18873de","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:49.2994624-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar 5b5fd89b-9161-4560-92d7-4209da8a377e","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:08:49.4783276-05:00","record":{"event":"fabrication_transaction_failed","name":"MotorConexiones: 
borrar f370914e-94a4-441f-8bdd-c4b038d3c309","for_deletion":true,"error":"System.Reflection.TargetInvocationException: 
Exception has been thrown by the target of an invocation.\r\n ---> System.InvalidOperationException: cannot start a 
fabrication transaction while asynchronous fabrication tasks are queued for execution\r\n   at 
Autodesk.SteelConnectionsDB.FabricationTransaction..ctor(Document doc, Boolean isReadOnly, String strName, Boolean 
bRevitTransactionAlreadyStarted)\r\n   at InvokeStub_FabricationTransaction..ctor(Object, Span`1)\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   --- End of inner exception stack trace ---\r\n   at 
System.Reflection.MethodBaseInvoker.InvokeWithFewArgs(Object obj, BindingFlags invokeAttr, Binder binder, Object[] 
parameters, CultureInfo culture)\r\n   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, 
Binder binder, Object[] parameters, CultureInfo culture)\r\n   at 
MotorConexiones.Revit.Fabrication.AdvanceSteelBackend.SteelSession..ctor(AdvanceSteelBackend owner, Document document, 
String name, Boolean forDeletion) in D:\\Proyectos 
C#\\CONEXIONES\\src\\MotorConexiones.Revit\\Fabrication\\AdvanceSteelBackend.cs:line 262"}}
{"ts":"2026-10-21T12:11:22.2663780-05:00","record":{"event":"catalog_apply","template_id":"b7461a9f-3537-47f3-a6a2-e0ba
0a639b71","chosen_automatically":false,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":
true,"error_codes":[]}}
{"ts":"2026-10-21T12:11:22.2767456-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"template_id\": \"b7461a9f-3537-47f3-a6a2-e0ba0a639b71\", \"element_ids\": [1249510, 
1249630, 1249631, 1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":61}}
{"ts":"2026-10-21T12:11:22.3557277-05:00","record":{"event":"catalog_apply","template_id":"b7461a9f-3537-47f3-a6a2-e0ba
0a639b71","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-21T12:11:22.3600135-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":56}}
{"ts":"2026-10-21T12:11:22.6655449-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"n
odes\": [{\"node\": \"N4\", \"validation_token\": 
\"0000000000000000000000000000000000000000000000000000000000000000\"}], \"plan_id\": \"f49dce56-56c3-460a-826b-f3dddbed
0a42\"}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAILED"],"duration_ms":4}}
{"ts":"2026-10-21T12:11:22.8522541-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
template_id\": \"00000000-0000-0000-0000-000000000000\", \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":false,"error_codes":["TEMPLATE_NOT_FOUND"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-21T12:11:27.2455408-05:00","record":{"event":"catalog_apply","template_id":"f6d9d174-a970-4d8f-b87a-67eb
94abd3bd","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-21T12:11:27.2460701-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
chord_element_id\": 1249510, \"element_ids\": [1249510, 1249630, 1249631, 
1249636]}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":24}}
{"ts":"2026-10-21T15:13:48.8091646-05:00","record":{"event":"startup","addin_version":"0.12.1","revit_version":"2027","
revit_build":"27.2.0.39","ribbon_tab":"ARBA","assembly":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revi
t\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","label_handler":true,"label_handler_error":null}}
{"ts":"2026-10-21T15:24:14.5604696-05:00","record":{"event":"ribbon_connect_pick","already":1249631,"picked":[1249630,1
249510,1249636]}}
{"ts":"2026-10-21T15:24:14.7920683-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":33072}}
{"ts":"2026-10-21T15:24:18.5650521-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T15:30:11.0878506-05:00","record":{"event":"ribbon_connect_truss","selection":9,"nodes":5}}
{"ts":"2026-10-21T15:30:24.7849735-05:00","record":{"event":"ribbon_connect_truss","selection":9,"nodes":5}}
{"ts":"2026-10-21T15:31:13.0786886-05:00","record":{"event":"ribbon_connect_pick","already":1249630,"picked":[1249631,1
249510,1249636]}}
{"ts":"2026-10-21T15:31:13.1078337-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":1,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":5664}}
{"ts":"2026-10-21T15:31:15.0680423-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T15:31:21.6017582-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":3,"chord_element_id":1249510,"kind":"node","splice_element_id":null,"eccentricity_mm":3.4,"
templates":2,"candidates":1,"proposal":{"template_id":"6abcf116-9b97-485f-b50d-2851ca0018cc","name":"Nudo tipico 
Detalle D","orientation":"same","max_deviation_deg":0},"starter":false,"connection_type":"gusset_node","mode":"template
","is_valid":true,"errors":[],"duration_ms":27}}
{"ts":"2026-10-21T15:31:23.8583638-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1249510,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T15:32:22.5639613-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":1,"chord_element_id":1245533,"kind":"splice","splice_element_id":null,"eccentricity_mm":0,"
templates":0,"candidates":0,"proposal":null,"starter":true,"connection_type":"end_plate","mode":"starter_empty_catalog"
,"is_valid":true,"errors":[],"duration_ms":28}}
{"ts":"2026-10-21T15:33:55.6160077-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter_empty_catalog"}}
{"ts":"2026-10-21T15:34:13.0936847-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":2,"chord_element_id":1245533,"kind":"node","splice_element_id":1245532,"eccentricity_mm":0,
"templates":2,"candidates":1,"proposal":{"template_id":"91adac27-d196-4c98-88db-6579aa991be4","name":"Nudo K cordon sup
erior","orientation":"same","max_deviation_deg":0.55},"starter":false,"connection_type":"direct_weld","mode":"template"
,"is_valid":true,"errors":[],"duration_ms":110558}}
{"ts":"2026-10-21T15:34:32.5314143-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T15:34:35.0119268-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":1,"chord_element_id":1245533,"kind":"splice","splice_element_id":null,"eccentricity_mm":0,"
templates":0,"candidates":0,"proposal":null,"starter":true,"connection_type":"end_plate","mode":"starter_empty_catalog"
,"is_valid":true,"errors":[],"duration_ms":24}}
{"ts":"2026-10-21T15:34:38.8880240-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter_empty_catalog"}}
{"ts":"2026-10-21T15:34:49.7243334-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":1,"chord_element_id":1245533,"kind":"splice","splice_element_id":null,"eccentricity_mm":0,"
templates":0,"candidates":0,"proposal":null,"starter":true,"connection_type":"end_plate","mode":"starter_empty_catalog"
,"is_valid":true,"errors":[],"duration_ms":27}}
{"ts":"2026-10-21T15:34:53.2038866-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter_empty_catalog"}}
{"ts":"2026-10-21T15:34:54.3269125-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":2,"chord_element_id":1245533,"kind":"node","splice_element_id":1245532,"eccentricity_mm":0,
"templates":2,"candidates":1,"proposal":{"template_id":"91adac27-d196-4c98-88db-6579aa991be4","name":"Nudo K cordon sup
erior","orientation":"same","max_deviation_deg":0.55},"starter":false,"connection_type":"direct_weld","mode":"template"
,"is_valid":true,"errors":[],"duration_ms":4629}}
{"ts":"2026-10-21T15:34:58.7867892-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"template"}}
{"ts":"2026-10-21T15:35:16.9060844-05:00","record":{"event":"ribbon_connect","document":"HANGAR_PRUEBA_sondeo","selecti
on":4,"ignored":0,"members":1,"chord_element_id":1245533,"kind":"splice","splice_element_id":null,"eccentricity_mm":0,"
templates":0,"candidates":0,"proposal":null,"starter":true,"connection_type":"end_plate","mode":"starter_empty_catalog"
,"is_valid":true,"errors":[],"duration_ms":24}}
{"ts":"2026-10-21T15:36:33.3526464-05:00","record":{"event":"ribbon_connect_cancelled","chord_element_id":1245533,"reas
on":"window_closed","mode":"starter_empty_catalog"}}
{"ts":"2026-10-21T15:38:45.4132597-05:00","record":{"event":"catalog_apply","template_id":"7e8b3ede-559f-4778-bd45-de0c
e859096b","chosen_automatically":false,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":
true,"error_codes":[]}}
{"ts":"2026-10-21T15:38:45.4204562-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
element_ids\": [1249510, 1249630, 1249631, 1249636], \"template_id\": \"7e8b3ede-559f-4778-bd45-de0ce859096b\", 
\"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":25}}
{"ts":"2026-10-21T15:38:45.4784976-05:00","record":{"event":"catalog_apply","template_id":"7e8b3ede-559f-4778-bd45-de0c
e859096b","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-21T15:38:45.4820445-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 
1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":25}}
{"ts":"2026-10-21T15:38:45.6689768-05:00","record":{"event":"handle","operation":"batch_create","request_summary":"{\"p
lan_id\": \"f2b67a1a-0d79-4a52-8fd5-8a27c022c9d1\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"000000000000
0000000000000000000000000000000000000000000000000000\"}]}","ok":true,"error_codes":[],"warning_codes":["BATCH_NODE_FAIL
ED"],"duration_ms":15}}
{"ts":"2026-10-21T15:38:45.8013944-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
element_ids\": [1249510, 1249630, 1249631, 1249636], \"template_id\": \"00000000-0000-0000-0000-000000000000\"}","ok":f
alse,"error_codes":["TEMPLATE_NOT_FOUND"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-10-21T15:38:48.9636884-05:00","record":{"event":"catalog_apply","template_id":"6907c00e-0f72-458b-89fd-2bbf
50c9683b","chosen_automatically":true,"element_ids":[1249510,1249630,1249631,1249636],"orientation":"same","is_valid":t
rue,"error_codes":[]}}
{"ts":"2026-10-21T15:38:48.9644755-05:00","record":{"event":"handle","operation":"catalog_apply","request_summary":"{\"
element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 
1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":31}}



```

## 12b-7 git status antes del commit

```text
?? docs/fases/capturas/fase12b-03-una-barra.png
?? docs/fases/capturas/fase12b-04-cuadro.png
?? docs/fases/capturas/fase12b-empalme-n9.png
?? docs/fases/capturas/fase12b-error-borde.png
?? docs/fases/capturas/fase8-01-sondeo17-exportada.png
?? docs/fases/resultados-fase-12b.md

```
