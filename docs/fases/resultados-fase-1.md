# Resultados de la Fase 1

Fecha: 2026-09-30T19:25:07


## 2 git

```text
git : Already on 'claude/laughing-pascal-tsxvkt'
En línea: 9 Carácter: 35
+ ...  git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pu ...
+                        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (Already on 'cla...-pascal-tsxvkt':String) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
 
Your branch is up to date with 'origin/claude/laughing-pascal-tsxvkt'.
git : From https://github.com/Andy-rba30/CONEXIONES
En línea: 9 Carácter: 79
+ ... -pascal-tsxvkt; git pull origin claude/laughing-pascal-tsxvkt; git lo ...
+                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (From https://gi...ba30/CONEXIONES:String) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
 
 * branch            claude/laughing-pascal-tsxvkt -> FETCH_HEAD
Already up to date.
43c0197 Fase 1: esqueleto de la solución, puente Bridge.Handle, conn_ping y sondeos de la prueba técnica

```

## 3 dotnet build y test

```text
10.0.401 [C:\Program Files\dotnet\sdk]
  Determinando los proyectos que se van a restaurar...
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\MotorConexiones.Revit.csproj (en 994 ms).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\MotorConexiones.Tests.csproj (en 4.05 s).
  Se ha restaurado D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\MotorConexiones.Core.csproj (en 4.47 s).
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:08.98
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    20, Omitido:     0, Total:    20, Duración: 128 ms - MotorConexiones.Tests.dll (net10.0)

```

## 4 revit cerrado

```text

```

## 5 deploy

```text
.\scripts\deploy.ps1 : No se puede cargar el archivo D:\Proyectos C#\CONEXIONES\scripts\deploy.ps1 porque la ejecución 
de scripts está deshabilitada en este sistema. Para obtener más información, consulta el tema about_Execution_Policies 
en https:/go.microsoft.com/fwlink/?LinkID=135170.
En línea: 9 Carácter: 20
+ Anota "5 deploy" { .\scripts\deploy.ps1 -NoBuild; Get-ChildItem "$env ...
+                    ~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : SecurityError: (:) [], PSSecurityException
    + FullyQualifiedErrorId : UnauthorizedAccess
Get-Content : No se encuentra la ruta de acceso 'C:\Users\Andy Bayona 
Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin' porque no existe.
En línea: 9 Carácter: 166
+ ... me, Length; Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\Moto ...
+                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (C:\Users\Andy B...onexiones.addin:String) [Get-Content], ItemNotFoundEx 
   ception
    + FullyQualifiedErrorId : PathNotFound,Microsoft.PowerShell.Commands.GetContentCommand
 

```

## 5 deploy

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, docs\guide.md
Siguiente paso: abre Revit 2027. Debe aparecer la pestana 'Conexiones'.

FullName                                                                                                        Length
--------                                                                                                        ------
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\config                          
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\docs                            
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Core.dll  16384 
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Core.pdb  12368 
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll 58368 
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.pdb 19856 
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json        467   
C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\docs\guide.md             1281  
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>MotorConexiones</Name>
    <!-- scripts/deploy.ps1 sustituye C:\Users\Andy Bayona AntÃ³n\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll por la ruta absoluta de la DLL desplegada. -->
    <Assembly>C:\Users\Andy Bayona AntÃ³n\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll</Assembly>
    <AddInId>8D2F5C3A-7E4B-4C1D-9A6E-1B2C3D4E5F60</AddInId>
    <FullClassName>MotorConexiones.Revit.App</FullClassName>
    <VendorId>MCNX</VendorId>
    <VendorDescription>MotorConexiones - conexiones de acero desde una especificacion JSON</VendorDescription>
  </AddIn>
</RevitAddIns>



```

## 6 instalar-conn

```text
ERROR: en tools\__init__.py no se encontraron las lineas ancla de document_tools. Anade a mano: from .conn_tools import register_conn_tools / register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)

C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\startup.py:281:        from revit_mcp.conexiones import 
register_conn_routes
C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\startup.py:283:        register_conn_routes(api)
C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\tools\__init__.py:49:    "create_steel_connection", 
"add_plate_or_stiffener", "split_beam", "fix_analytical_alignment",
C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\tools\__init__.py:51:    "family_create_solids", 
"family_lock_faces", "family_set_type_values", "family_add_connectors",



```

## 8 status

```text


api_name        : revit_mcp
health          : healthy
document_title  : HANGAR_PRUEBA
status          : active
revit_available : True




```

## 8 conn ping

```text
== conn/ping -> HTTP 200 en 79 ms ==
{
    "ok":  true,
    "meta":  {
                 "operation":  "ping",
                 "duration_ms":  3,
                 "addin_version":  "0.1.0"
             },
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "spec_version":  "1.0",
                 "backend":  "directshape (camino B, provisional hasta la decisión de la Fase 1)",
                 "operations":  [
                                    "ping",
                                    "probe_delete_b",
                                    "probe_plate_b"
                                ],
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "addin_version":  "0.1.0",
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "language":  "English_USA"
                           },
                 "has_uidocument":  true,
                 "document":  {
                                  "is_workshared":  true,
                                  "title":  "HANGAR_PRUEBA",
                                  "is_read_only":  false,
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA.rvt"
                              }
             }
}

```

## 7 abrir revit y boton

``text
Revit 2027 abierto con D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA.rvt.
Ventana de seguridad detectada: TaskDialog_Security_Unsigned_File_Loading ("The publisher of this add-in could not be verified. What do you want to do?"). Se pulso 'Always Load'.
Pestana 'Conexiones' visible en la cinta con el boton 'Ejecutar especificacion JSON'.
Se pulso el boton y mostro el dialogo:
'MotorConexiones 0.1.0 esta cargado.
Revit 2027 (27.2.0.39).
La ejecucion de especificaciones JSON desde este boton llega en la Fase 3.
Mientras tanto, la IA usa las herramientas conn_* del servidor MCP.'
Captura guardada en docs\fases\capturas\fase1-01-boton.png.
Dialogo cerrado.
pyRevit cargo correctamente con el servidor Routes activo.
``

## 8 status

```text


api_name        : revit_mcp
health          : healthy
document_title  : HANGAR_PRUEBA
status          : active
revit_available : True




```

## 8 conn ping

```text
== conn/ping -> HTTP 200 en 41 ms ==
{
    "ok":  true,
    "meta":  {
                 "operation":  "ping",
                 "duration_ms":  0,
                 "addin_version":  "0.1.0"
             },
    "errors":  [

               ],
    "warnings":  [

                 ],
    "data":  {
                 "spec_version":  "1.0",
                 "backend":  "directshape (camino B, provisional hasta la decisión de la Fase 1)",
                 "operations":  [
                                    "ping",
                                    "probe_delete_b",
                                    "probe_plate_b"
                                ],
                 "dotnet":  {
                                "assembly_location":  "C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll",
                                "framework":  ".NET 10.0.12",
                                "load_context":  "Default"
                            },
                 "addin_version":  "0.1.0",
                 "revit":  {
                               "version_name":  "Autodesk Revit 2027",
                               "version_number":  "2027",
                               "sub_version_number":  "2027.2",
                               "version_build":  "27.2.0.39",
                               "language":  "English_USA"
                           },
                 "has_uidocument":  true,
                 "document":  {
                                  "is_workshared":  true,
                                  "title":  "HANGAR_PRUEBA",
                                  "is_read_only":  false,
                                  "is_family":  false,
                                  "is_modifiable":  false,
                                  "path":  "D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA.rvt"
                              }
             }
}

```

## 9 sondeo 05 bridge ping

```text
== 05-bridge-ping.py -> HTTP 200 en 1022 ms ==
=== 05-bridge-ping ===
1) DLL desplegada existe: True (C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll)
   manifiesto existe: True (C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin)
2) MotorConexiones.Revit cargado: MotorConexiones.Revit, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   Location: C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll
   AssemblyLoadContext: Default
   MotorConexiones.Core cargado: True
3) Tipo Bridge encontrado: True
   Metodo Handle(String, String, Document, UIDocument): True
4) Handle('ping', '{}') ->
   {"ok":true,"data":{"addin_version":"0.1.0","spec_version":"1.0","backend":"directshape (camino B, provisional hasta la decisión de la Fase 1)","operations":["ping","probe_delete_b","probe_plate_b"],"revit":{"version_number":"2027","version_build":"27.2.0.39","version_name":"Autodesk Revit 2027","sub_version_number":"2027.2","language":"English_USA"},"dotnet":{"framework":".NET 10.0.12","assembly_location":"C:\\Users\\Andy Bayona Antón\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","load_context":"Default"},"document":{"title":"HANGAR_PRUEBA","path":"D:\\IG INGENIERÍA\\Hartree\\HANGAR_PRUEBA.rvt","is_family":false,"is_workshared":true,"is_modifiable":true,"is_read_only":false},"has_uidocument":true},"errors":[],"warnings":[],"meta":{"operation":"ping","duration_ms":0,"addin_version":"0.1.0"}}
4) Handle('no_existe', '{}') ->
   {"ok":false,"data":null,"errors":[{"code":"UNKNOWN_OPERATION","path":null,"message":"La operación 'no_existe' no existe en el add-in.","hint":"Operaciones disponibles: ping, probe_delete_b, probe_plate_b."}],"warnings":[],"meta":{"operation":"no_existe","duration_ms":0,"addin_version":"0.1.0"}}
4) Handle('ping', 'esto no es json') ->
   {"ok":false,"data":null,"errors":[{"code":"INVALID_REQUEST","path":null,"message":"La petición no es JSON válido: 'e' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.","hint":"Envía un objeto JSON, por ejemplo {\"element_ids\": [111, 222]}."}],"warnings":[],"meta":{"operation":"ping","duration_ms":1,"addin_version":"0.1.0"}}
=== fin 05-bridge-ping ===


```

## 9 sondeo 06 steel miembros

```text
== 06-steel-miembros.py -> HTTP 200 en 719 ms ==
=== 06-steel-miembros ===
1) RevitAPISteel.dll: 32 tipos publicos (todos):
   Autodesk.Revit.DB.Steel.AsyncWarnings
   Autodesk.Revit.DB.Steel.ExtRevitDwgHost
   Autodesk.Revit.DB.Steel.FITManager
   Autodesk.Revit.DB.Steel.FramingProfileServiceData
   Autodesk.Revit.DB.Steel.GeomObjectType  [enum]
   Autodesk.Revit.DB.Steel.IFITRunner  [interfaz]
   Autodesk.Revit.DB.Steel.IFramingProfileServer  [interfaz]
   Autodesk.Revit.DB.Steel.ISteelElement  [interfaz]
   Autodesk.Revit.DB.Steel.ISteelModelCallback  [interfaz]
   Autodesk.Revit.DB.Steel.ISteelModelLibrary  [interfaz]
   Autodesk.Revit.DB.Steel.SteelBeamVoidInterval
   Autodesk.Revit.DB.Steel.SteelConnectionUtil
   Autodesk.Revit.DB.Steel.SteelElementGeometryData
   Autodesk.Revit.DB.Steel.SteelElementGeometryHistoryData
   Autodesk.Revit.DB.Steel.SteelElementParamType  [enum]
   Autodesk.Revit.DB.Steel.SteelElementTempGeometryContainer
   Autodesk.Revit.DB.Steel.SteelMarksUtils
   Autodesk.Revit.DB.Steel.SteelModelFlavour  [enum]
   Autodesk.Revit.DB.Steel.SteelModelInfo
   Autodesk.Revit.DB.Steel.SteelModelManager
   Autodesk.Revit.DB.Steel.SteelModelSaveBackup
   Autodesk.Revit.DB.Steel.SteelProxyElement
   Autodesk.Revit.DB.Steel.StructuralCmdUtil
   Autodesk.Revit.DB.Steel.StructuralConnectionBaseUtil
   Autodesk.Revit.DB.Structure.HoleDefinitionServiceData
   Autodesk.Revit.DB.Structure.IHoleDefinitionServer  [interfaz]
   Autodesk.Revit.DB.Structure.IModifyConnectionParametersServer  [interfaz]
   Autodesk.Revit.DB.Structure.IModifyConnectionRangesServer  [interfaz]
   Autodesk.Revit.DB.Structure.ModifyConnectionParametersServiceData
   Autodesk.Revit.DB.Structure.ModifyConnectionRangesServiceData
   Autodesk.Revit.UI.IModelessUI  [interfaz]
   Autodesk.Revit.UI.ModelessUIManager
1b) Miembros de los tipos que pueden abrir el contexto de acero:
  --- Autodesk.Revit.DB.Steel.SteelConnectionUtil (clase, abstracta/estatica)
      base: System.Object
      static StructuralConnectionHandler CreateStructuralConnectionHandler(Document doc, IList`1 inputs, ElementId connectionTypeId, String extEntId, IList`1 additionalPointsInfo)
      static IList`1 GetGeometry(Document doc, Reference reference, Int32 detailLevel)
      static IList`1 GetInputElementIds(StructuralConnectionHandler conn)
      static IList`1 GetInputMemberInfos(StructuralConnectionHandler conn)
      static Boolean IsComponentOfEditedCustomConnection(Document doc, ElementId id)
      static Boolean IsCustomConnectionInEditMode(Document doc, ElementId& id)
      static Boolean IsElementCutByReferencePlane(Element element)
      static Boolean IsIdleBlocked(Document doc)
  --- Autodesk.Revit.DB.Steel.SteelModelInfo (clase)
      base: System.Object
      Void Dispose()
      Boolean GetExtModelChangesIncrement(Int32 nIncrementIdx, IntPtr data, Int32 nDataSize)
      Boolean GetExtModelChangesIncrement(Guid incrementId, IntPtr data, Int32 nDataSize)
      Int32 GetExtModelChangesIncrementSize(Int32 nIncrementIdx)
      Int32 GetExtModelChangesIncrementSize(Guid incrementId)
      Int32 GetExtModelChangesIncrementsCount()
      Void GetExtModelChangesIncrementsIds(IList`1& increments)
      Boolean GetExtModelData(SteelModelFlavour flavour, IntPtr data, Int32 nDataSize)
      Boolean GetExtModelDataHash(SteelModelFlavour flavour, String& hash)
      Int32 GetExtModelDataSize(SteelModelFlavour flavour)
      Boolean GetExtModelLocalChangesIncrement(IntPtr data, Int32 nDataSize)
      Int32 GetExtModelLocalChangesIncrementSize()
      static SteelModelInfo Instance(Document aDoc)
      Boolean PromoteLocalChangesToCentral()
      Void Reset(Document pDoc)
      Void ResetExtModelIncrementalChanges()
      Void ResetLocalIncrementalChanges()
      Void SaveSteelModel(Document pDoc)
      Void SetExtModelData(SteelModelFlavour flavour, IntPtr data, Int32 nDataSize, String datahash)
      Void SetExtModelLocalChangesIncrement(Guid strIncrementId, IntPtr incrementalChangesData, Int32 nIncrementalChangesDataSize)
      Void UpdateSteelModel(Document pDoc)
      props: ExtModelId:String, IsValidObject:Boolean
  --- Autodesk.Revit.DB.Steel.SteelModelManager (clase)
      base: System.Object
      Void AttachSteelGeometryGenerationHistoryToGeometry(Solid solid, Int32 nExtraKey)
      Void CancelTask(Document doc, Guid extUid)
      Void ClearRedoStack(Document doc)
      Void ClearSubElementsCacheForExternalEntities(Document doc, IList`1 idsExtEnts)
      Void ClearUndoStack(Document doc)
      Void ConnectionUpdateRequested(Document doc, String strUid, String strUpdateInfo)
      Void CreateSteelElementsUndoItem(Document doc, Guid uid)
      Void DelayedTaskExecuted(Document doc, Int32 objs, String strMoreInfo)
      Void DetachSteelGeometryGenerationHistoryFromGeometry(Solid solid)
      Void Dispose()
      Void EndSteelGeometryGenerationHistory()
      Void EnqueueTask(Document doc, Guid extUid, String taskDescription, String taskAdditionalInfo)
      Void ExecuteTask(Document doc, Guid extUid)
      ISteelModelLibrary GetExtModelLibrary()
      Int32 GetHistory(IntPtr history, Int32 historyBufferSize)
      Int32 GetHistoryBufferSize()
      static SteelModelManager GetInstance()
      ISteelModelCallback GetModelCallback()
      SteelElementTempGeometryContainer GetSteelElementTempGeometryContainer(Document doc)
      Boolean IsAnySteelTaskSecondaryData(Document doc)
      Boolean IsAwaitModeEnforced()
      Void RebuildSteelSubelementsGraphicsCache(Document doc, IDictionary`2 updateSet)
      Void ReferenceAndAddGeometryObjectToHistory(GeometryObject geometryObject, IList`1 extraKeys)
      Void RegisterExtModelCallback(ISteelModelCallback callback)
      Void RegisterExtModelLibrary(ISteelModelLibrary extModelLibrary)
      Void RequestCacheTransform(Document doc, Guid uid, Transform trf)
      Void RequestGraphicalUpdateForSteelElements(Document doc, ISet`1 idsToUpdate, ISet`1 revitLinkIds2Update)
      Void SetEndFaceTagsForGeometryGeneration(Int32 startFaceTag, Int32 endFaceTag)
      Void SetHistory(IntPtr history, Int32 historyBufferSize)
      Void StartSteelGeometryGenerationHistory()
      Void UnregisterExtModelLibrary()
      props: IsValidObject:Boolean
  --- Autodesk.Revit.DB.Steel.SteelProxyElement (clase)
      base: Autodesk.Revit.DB.Element
      ctor()
      static SteelProxyElement Create(Document aDoc, GeomObjectType type)
      props: GeomType:GeomObjectType, SketchId:ElementId
  --- Autodesk.Revit.DB.Steel.StructuralConnectionBaseUtil (clase, abstracta/estatica)
      base: System.Object
      static Boolean AllowSubpartsAsConnectionInputElements()
      static Boolean EnableCabaggeForSteelElements()
  --- Autodesk.Revit.DB.Steel.SteelElementProperties (clase)
      base: Autodesk.Revit.DB.APIObject
      static IList`1 AddFabricationInformationForRevitElements(Document aDoc, IList`1 elementIds)
      static Guid GetFabricationUniqueID(Document aDoc, Reference reference)
      static Reference GetReference(Document aDoc, Guid guid)
      static SteelElementProperties GetSteelElementProperties(Element pElement)
      props: IsValidObject:Boolean, UniqueID:Guid
2) Autodesk.SteelConnectionsDB.dll: 22 tipos publicos; 3 de interes (max 60):
   Autodesk.SteelConnectionsDB.FabricationOnlyTransaction
   Autodesk.SteelConnectionsDB.FabricationTransaction
   Autodesk.SteelConnectionsDB.SteelConnectionPagesScope
2b) Miembros de los tipos con Transaction/Fabrication/Context/Session:
  --- Autodesk.SteelConnectionsDB.FabricationOnlyTransaction (clase)
      base: System.Object
      Void CancelTransaction()
      Void Commit()
      Void Dispose()
      static Boolean IsWriteTransactionStarted(Document doc)
  --- Autodesk.SteelConnectionsDB.FabricationTransaction (clase)
      base: Autodesk.SteelConnectionsDB.FabricationOnlyTransaction
      ctor(Document doc, Boolean isReadOnly, String strName, Boolean bRevitTransactionAlreadyStarted)
      ctor(Document doc, Boolean isReadOnly, String strName, Boolean bRevitTransactionAlreadyStarted, Boolean bAllowASChangesPropagation)
      ctor(Document doc, Boolean isReadOnly, String strName)
      Void CancelTransaction()
      Void Commit()
      static Void ValidateNewObjects(Document doc, IEnumerable`1 newObjects)
3) Carpeta C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections: existe=True
   DLL (123):
     ASAutoConnection.dll (298 KB)
     ASAutoConnectionMgd.dll (127 KB)
     ASCADAccess.dll (47 KB)
     ASCADInterfaces.dll (157 KB)
     ASCADLinkMgd.dll (71 KB)
     ASConnectionsPreviews.dll (35505 KB)
     ASConnectionsResources.dll (256 KB)
     ASDataObjects.dll (14832 KB)
     ASGTCMapping.dll (96 KB)
     ASGeometryMgd.dll (334 KB)
     ASMgdJoints.dll (201 KB)
     ASModelerMgd.dll (198 KB)
     ASNativeDataObjects.dll (441 KB)
     ASNetRuntime.dll (17 KB)
     ASObjects.dll (5905 KB)
     ASObjectsBase.dll (845 KB)
     ASObjectsDBLayer.dll (27 KB)
     ASObjectsMgd.dll (1401 KB)  [cargado]
     ASProfilesMgd.dll (345 KB)
     ASRepository.dll (185 KB)
     ASRepositoryProxy.dll (9191 KB)
     ASRepositorySQLSrv.dll (3296 KB)
     ASSettings.dll (36 KB)  [cargado]
     ASSettingsMgd.dll (87 KB)
     ASSteelControls.dll (38 KB)
     Accessor.dll (558 KB)
     AecModeler.dll (696 KB)
     AnalyticsBase.dll (52 KB)
     Anchor.dll (782 KB)
     AsConnectionsFramework.dll (2364 KB)
     AsRealDWGLoaderMgd.dll (114 KB)
     AstBCGCBPRO951u140x64.dll (4217 KB)
     AstDocReport.comhost.dll (189 KB)
     AstDocReport.dll (45 KB)
     AstJointsAS51Bitmaps.dll (19092 KB)
     AstStairsAndRailingBitmaps.dll (3623 KB)
     AstorJointBitmapsSteel.dll (6925 KB)
     AstorJointBitmapsSteel1.dll (2600 KB)
     Autodesk.SteelConnections.ASDynamicProfiles.dll (28 KB)
     Autodesk.SteelConnections.ASIFC.dll (196 KB)
     Autodesk.SteelConnections.ASRvtFamilyMapping.dll (24 KB)  [cargado]
     Autodesk.SteelConnections.ASRvtGeomConversions.dll (23 KB)
     Autodesk.SteelConnections.ASRvtModeler.dll (32 KB)
     Autodesk.SteelConnectionsDB.dll (656 KB)  [cargado]
     Autodesk.SteelConnectionsUI.dll (3230 KB)  [cargado]
     AxInterop.ASTCONTROLSLib.dll (69 KB)
     BCGPStyle2007Aqua.dll (1764 KB)
     BCGPStyle2007Luna.dll (1693 KB)
     BCGPStyle2007Obsidian.dll (1694 KB)
     BCGPStyle2007Silver.dll (1693 KB)
     BindingPlates.dll (128 KB)
     Bricks.dll (471 KB)
     ColdRolled.dll (8164 KB)
     ConcreteJoints.dll (554 KB)
     Connections.dll (6859 KB)
     DSCMFC.dll (4199 KB)
     DSCProfileDesignerAccess.dll (57 KB)
     DotNetRootsMgd.dll (64 KB)
     DscATL.dll (135 KB)
     DscAddinManagerBase.dll (49 KB)
     DscBitmaps.dll (53 KB)
     DscDerivedDocumentsBase.dll (162 KB)
     DscExchange.dll (45 KB)
     DscFileFiler.dll (42 KB)
     DscGeomBase.dll (2275 KB)
     DscGeomCOM8x64.dll (480 KB)
     DscMFCBase.dll (25 KB)
     DscODBC.dll (1103 KB)
     DscODBCCOM8x64.dll (141 KB)
     DscProfilesAccess.dll (1393 KB)
     DscProfilesAccessCOM8x64.dll (213 KB)
     DscProjectAccess.dll (78 KB)
     DscRoots.dll (668 KB)
     DscRootsCOM8x64.dll (349 KB)
     DscTable.dll (287 KB)
     DscUnits.dll (130 KB)
     DscUnitsTransform.dll (101 KB)
     DscUtilFacetCOM8x64.dll (120 KB)
     DscUtilFacet_ACIS.dll (114 KB)
     DscUtilFacet_ADESK.dll (534 KB)
     DscUtilFacet_ADESKCAD.dll (144 KB)
     EbInstanceModel.dll (46 KB)
     EntityFramework.SqlServer.dll (578 KB)
     EntityFramework.dll (4864 KB)
     GTCImportExport.dll (981 KB)
     HSSBracing.dll (1492 KB)
     Haunches.dll (1431 KB)
     IFC2X3.dll (537 KB)
     IFCBase.dll (23 KB)
     IFCPlugin.dll (98 KB)
     IFCUnitTests.dll (116 KB)
     InstanceModel.dll (309 KB)
     Interop.ASGTCMappingLib.dll (17 KB)
     Interop.ASTCONTROLSLib.dll (93 KB)
     Interop.AstSTEELAUTOMATIONLib5.dll (673 KB)
     Interop.DSCGEOMCOMLib.dll (57 KB)
     Interop.DSCODBCCOMLib.dll (18 KB)
     Interop.DSCPROFILESACCESSCOMLib.dll (52 KB)
     Interop.DSCRootsCOMLib.dll (62 KB)
     Interop.RTFENGINECOMLib.dll (76 KB)
     JointDesignUtils.dll (126 KB)
     Joints51.dll (5917 KB)
     Joints61.dll (6798 KB)
     JointsUtilities.dll (1409 KB)
     Joist.dll (497 KB)
     MiddleGusset.dll (353 KB)
     MiscJoints.dll (5056 KB)
     ModelerBase.dll (269 KB)
     NSAConfiguration8x64.dll (119 KB)
     NotchConnection.dll (182 KB)
     RTFEngineCOM.dll (502 KB)
     Railing.dll (3203 KB)
     RevitAssemblyResolver.dll (17 KB)  [cargado]
     RvtSteelConnectionsAnalysis.dll (24 KB)
     SchemaModel.dll (963 KB)
     ScriptHost.dll (48 KB)
     SideRailConnBolts.dll (141 KB)
     SqlLocalDbManMgd.dll (68 KB)
     System.Data.Odbc.dll (255 KB)
     System.Data.OleDb.dll (348 KB)
     System.Data.SqlClient.dll (1002 KB)
     ijwhost.dll (134 KB)
     sni.dll (150 KB)
4) ASGeometryMgd.dll: existe=True
   cargado ahora: True
   tipos publicos: 86; espacios: -=47, Autodesk.AdvanceSteel.Geometry=39
  --- Autodesk.AdvanceSteel.Geometry.Point3d (clase)
      base: System.Object
      ctor(AstGeomPoint3d* pt)
      ctor(Point3d source)
      ctor(Double x, Double y, Double z)
      ctor()
      Boolean IsEqualTo(Point3d ptOther)
      Boolean IsEqualTo(Point3d ptOther, Tol tol)
      Point3d Set(Double x, Double y, Double z)
      Point3d TransformBy(Matrix3d matrix3d)
      props: Native:AstGeomPoint3d*, kOrigin:Point3d, x:Double, y:Double, z:Double
  --- Autodesk.AdvanceSteel.Geometry.Vector3d (clase)
      base: System.Object
      ctor(AstGeomVector3d* ntvVec)
      ctor(Double x, Double y, Double z)
      ctor(Vector3d source)
      ctor()
      Double GetAngleOnPlane(Plane pPlane)
      Double GetAngleTo(Plane pPlane)
      Double GetAngleTo(Vector3d vOther, Vector3d vReference)
      Double GetAngleTo(Vector3d vOther)
      UInt32 GetLargestElement()
      Double GetLength()
      Double GetLengthSqrd()
      Vector3d GetNormal()
      ... (mas miembros)
      props: Native:AstGeomVector3d*, kIdentity:Vector3d, kXAxis:Vector3d, kYAxis:Vector3d, kZAxis:Vector3d, x:Double, y:Double, z:Double
  --- Autodesk.AdvanceSteel.Geometry.Plane (clase)
      base: System.Object
      ctor(AstGeomPlane* ntvPlane)
      ctor(Double a, Double b, Double c, Double d)
      ctor(Point3d ptOrigin, Vector3d vAxisU, Vector3d vAxisV)
      ctor(Point3d ptU, Point3d ptOrigin, Point3d ptV)
      ctor(Point3d ptOrigin, Vector3d vNormal)
      ctor(Plane source)
      ctor()
      Point3d GetClosestPointTo(Point3d point)
      Void GetCoordSystem(Point3d& origin, Vector3d& xAxis, Vector3d& yAxis, Vector3d& zAxis)
      Double GetSignedDistanceTo(Point3d ptTo)
      Boolean IsCoplanarTo(Plane other, Tol tol)
      Boolean IsEqualTo(Plane plane, Tol tol)
      ... (mas miembros)
      props: Native:AstGeomPlane*, Normal:Vector3d, Origin:Point3d, kXYPlane:Plane, kYZPlane:Plane, kZXPlane:Plane
  --- Autodesk.AdvanceSteel.Geometry.Matrix3d (clase)
      base: System.Object
      ctor(AstGeomMatrix3d* matrix)
      ctor(Matrix3d source)
      ctor()
      static Matrix3d GetAlignCoordSys(Point3d ptFromOrigin, Vector3d vFromXAxis, Vector3d vFromYAxis, Vector3d vFromZAxis, Point3d ptToOrigin, Vector3d vToXAxis, Vector3d vToYAxis, Vector3d vToZAxis)
      Void GetCoordSystem(Point3d& ptOrigin, Vector3d& vXAxis, Vector3d& vYAxis, Vector3d& vZAxis)
      Double GetDet()
      Matrix3d GetInverse()
      static Matrix3d GetMirroring(Line3d lineMirror)
      static Matrix3d GetMirroring(Point3d ptMirror)
      static Matrix3d GetMirroring(Plane planeMirror)
      static Matrix3d GetPlaneToWorld(Plane plane)
      static Matrix3d GetPlaneToWorld(Vector3d vNormal)
      ... (mas miembros)
      props: Native:AstGeomMatrix3d*, Values:Double[][], kIdentity:Matrix3d
4) ASObjectsMgd.dll: existe=True
   ya estaba cargado
   tipos publicos: 265; espacios: -=5, ASObjectsAPI=10, Autodesk.AdvanceSteel.ApplicabilityRanges=20, Autodesk.AdvanceSteel.Application=1, Autodesk.AdvanceSteel.Arrangement=8, Autodesk.AdvanceSteel.BuildingStructure=12, Autodesk.AdvanceSteel.CADAccess=14, Autodesk.AdvanceSteel.Connection=4, Autodesk.AdvanceSteel.ConstructionHelper=7, Autodesk.AdvanceSteel.ConstructionTypes=53, Autodesk.AdvanceSteel.Contours=6, Autodesk.AdvanceSteel.DerivedDocuments=3
   tipos de interes (39, max 60):
     ASObjectsAPI.FilerObject+eContourTypes
     Autodesk.AdvanceSteel.CADAccess.FilerObject+eContourTypes
     Autodesk.AdvanceSteel.Connection.ConnectionFeature
     Autodesk.AdvanceSteel.Connection.ConnectionHoleFeature
     Autodesk.AdvanceSteel.ConstructionTypes.ConstructionFeatureRule
     Autodesk.AdvanceSteel.ConstructionTypes.ContourConstructionObject
     Autodesk.AdvanceSteel.ConstructionTypes.FeatureObject
     Autodesk.AdvanceSteel.ConstructionTypes.FeatureObject+eFeatureType
     Autodesk.AdvanceSteel.Modelling.AnchorPattern
     Autodesk.AdvanceSteel.Modelling.AnchorPattern+eAnchorType
     Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch
     Autodesk.AdvanceSteel.Modelling.BoltPattern
     Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate
     Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.FoldedPlate
     Autodesk.AdvanceSteel.Modelling.InfinitMidScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.InfinitRectScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.Plate
     Autodesk.AdvanceSteel.Modelling.PlateBase
     Autodesk.AdvanceSteel.Modelling.PlateContourNotch
     Autodesk.AdvanceSteel.Modelling.PlateFeat
     Autodesk.AdvanceSteel.Modelling.PlateFeatContour
     Autodesk.AdvanceSteel.Modelling.PlateFeatEdge
     Autodesk.AdvanceSteel.Modelling.PlateFeatShortening
     Autodesk.AdvanceSteel.Modelling.PlateFeatVertFillet
     Autodesk.AdvanceSteel.Modelling.PlateFeatVertex
     Autodesk.AdvanceSteel.Modelling.PlateFeatWeldFillet
     Autodesk.AdvanceSteel.Modelling.PlateFeatWeldPreparation
     Autodesk.AdvanceSteel.Modelling.PlateFold
     Autodesk.AdvanceSteel.Modelling.PlateFoldRelation
     Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern
     Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern+eScrewBoltType
     Autodesk.AdvanceSteel.Modelling.WeldLine
     Autodesk.AdvanceSteel.Modelling.WeldPattern
     Autodesk.AdvanceSteel.Modelling.WeldPattern+eWeldSurface
     Autodesk.AdvanceSteel.Modelling.WeldPattern+eWeldType
     Autodesk.AdvanceSteel.Modelling.WeldPoint
  --- Autodesk.AdvanceSteel.Modelling.Plate (clase)
      base: Autodesk.AdvanceSteel.Modelling.PlateBase
      ctor(Plane plane, Point3d[] arrVerticesWCS)
      ctor(Plane plane, Point3d[] arrVerticesWCS, Double dThickness)
      ctor(Plane plane, Point3d ptCenter, Double dRadius)
      ctor(Plane plane, Point3d ptCenter)
      ctor(Plane plane, Point3d ptCenter, Double dLength, Double dWidth)
      Void Accept(IVisitor visitor)
      static Plate Create(Polyline3d poly)
      Void GetEdgeAndVertex(Point3d ptPick, Int32& iEdge, Int32& iVertex)
      Void GetNumberOfConnectedHoles(Int32& nNumHoles, Int32& nNumBoltHoles)
      Void GetSurfaceAtZPos(Double dZPos, Plane& plane)
      Boolean IsKindOf(eObjectType objType)
      Boolean MoveBaseContourEdgeTo(Int32 index, Point3d ptPointNew)
      Boolean SetBaseContourVertex(Int32 iVertex, Point3d ptVertexNew)
      Void SetLengthAndWidth(Double dLength, Double dWidth, Int32 bIgnoreRaster)
      Void SetPolygonContour(Point3d[] arrVerticesWCS)
      Boolean ShrinkBy(Double dGap)
      eObjectType Type()
  --- Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern (clase)
      base: Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern
      ctor(Point3d ptRef, Point3d ptRef2, Vector3d vX, Vector3d vY)
      Void Accept(IVisitor visitor)
      Boolean IsKindOf(eObjectType objType)
      eObjectType Type()
      Void setHeight(Double dLength, Int32 nDirection)
      Void setLength(Double dLength, Int32 nDirection)
      props: Height:Double, Length:Double, MidpointOnLowerRight:Point3d, MidpointOnUpperLeft:Point3d, Wx:Double, Wy:Double
  --- Autodesk.AdvanceSteel.Modelling.InfinitRectScrewBoltPattern (clase)
      base: Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern
      ctor(Point3d ptRef, Vector3d vX, Vector3d vY)
      Void Accept(IVisitor visitor)
      Boolean IsKindOf(eObjectType objType)
      eObjectType Type()
      props: MidpointOnLowerLeft:Point3d, MidpointOnUpperRight:Point3d, Wx:Double, Wy:Double
  --- Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern (clase)
      base: Autodesk.AdvanceSteel.Modelling.BoltPattern
      Void Accept(IVisitor visitor)
      HoleDefinition GetHoleDefinition(Int32 index)
      Double GetWeight()
      Boolean IsKindOf(eObjectType objType)
      Void SetHoleDefinition(Int32 index, HoleDefinition holeDef)
      eObjectType Type()
      Void setHoleTolerance(Double dTol, Boolean bForwardToAllHoles)
      props: Annotation:String, AssemblyLocation:eAssemblyLocation, BindingLength:Double, BindingLengthAddition:Double, BoltAssembly:String, BoltHeadDiameter:Double, BoltHeadHeight:Double, BoltHeadNumEdges:Int32, BottomToolDiameter:Double, BottomToolHeight:Double, Grade:String, HoleTolerance:Double, IgnoreMaxGap:Boolean, MaxBottomDiameter:Double, MaxTopDiameter:Double, NutDiameter:Double, NutHeight:Double, ScrewBoltType:eScrewBoltType, ScrewDiameter:Double, ScrewLength:Double, Standard:String, SumBottomHeight:Double, SumBottomSetHeight:Double, SumTopHeight:Double, SumTopSetHeight:Double, TopToolDiameter:Double, TopToolHeight:Double
  --- Autodesk.AdvanceSteel.Modelling.BoltPattern (clase)
      base: Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement
      Void Accept(IVisitor visitor)
      Void Connect(FilerObject[] elems, eAssemblyLocation location)
      Void GetHoles(ConnectionHoleFeature[]& arrHoles)
      Void GetMidpoints(Point3d[]& ptsMid)
      Void GetUsedMidpoints(FilerObject[] arrRelatedObjects, Point3d[]& ptsMid)
      Boolean IsKindOf(eObjectType objType)
      Void MoveRefPoint(Vector3d vOffset)
      Void RemoveConnectedObjects(FilerObject[] elems)
      Void SetCS(Matrix3d csNew)
      eObjectType Type()
      props: BoltNormal:Vector3d, Center:Point3d, IsInverted:Boolean, Normal:Vector3d, NumberOfScrews:Int32, RefPoint:Point3d, XDirection:Vector3d, YDirection:Vector3d
  --- Autodesk.AdvanceSteel.CADAccess.FilerObject (clase)
      base: System.Object
      Void Accept(IVisitor visitor)
      Void DelFromDb()
      Void Dispose()
      Boolean Equals(Object obj)
      Boolean Equals(FilerObject obj)
      FilerObject* GetFilerObject()
      static FilerObject GetFilerObject(FilerObject* pFiler)
      static FilerObject GetFilerObject(ObjectId obId)
      static FilerObject GetFilerObjectByHandle(String obHandle)
      Int32 GetHashCode()
      ObjectId GetObjectId()
      Guid GetUniqueId()
      Boolean IsKindOf(eObjectType objType)
      Void ResetBodyCache()
      Int32 SetLayer(ObjectId newVal, Boolean doSubents)
      Int32 SetLayer(String strNewVal, Boolean doSubents)
      eObjectType Type()
      ObjectId WriteToDb()
      Byte[] getXData(String xdataName)
      Void markChanged(Boolean bPropagateChanges)
      Void removeXData(String xdataName)
      Void setXData(String xdataName, Byte[] data)
      props: ASId:Int64, Handle:String, Layer:String, SkipEraseEnvironment:Boolean
4) ASDocumentMgd.dll: existe=False
4) ASCADLinkMgd.dll: existe=True
   ya estaba cargado
   tipos publicos: 9; espacios: -=6, Autodesk.AdvanceSteel.CADLink.Database=2, Autodesk.AdvanceSteel.CADLink.InteropServices=1
   tipos con Transaction/Document/Session/Lock/Manager (3, max 30):
     AcDbDatabase
     AcDbObjectId
     AstDbObjectId
4) ASDatabaseMgd.dll: existe=False
5) SDK de Revit 2027:
   no encontrado en las rutas habituales (opcional: descargar 'Revit 2027 SDK' de Autodesk Developer Network)
=== fin 06-steel-miembros ===


```

## 10 probar_conexiones

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  (esperado 401)  [OK]
Cuerpo:
{"error": "token ausente o incorrecto"}
======================================================================
2. GET /conn/ping/ con token -> 200  (esperado 200)  [OK]  addin_version=0.1.0 revit=27.2.0.39
Cuerpo:
{"ok":true,"meta":{"operation":"ping","duration_ms":0,"addin_version":"0.1.0"},"errors":[],"warnings":[],"data":{"spec_version":"1.0","backend":"directshape (camino B, provisional hasta la decisi\u00f3n de la Fase 1)","operations":["ping","probe_delete_b","probe_plate_b"],"dotnet":{"assembly_location":"C:\\Users\\Andy Bayona Ant\u00f3n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll","framework":".NET 10.0.12","load_context":"Default"},"addin_version":"0.1.0","revit":{"version_name":"Autodesk Revit 2027","version_number":"2027","sub_version_number":"2027.2","version_build":"27.2.0.39","language":"English_USA"},"has_uidocument":true,"document":{"is_workshared":true,"title":"HANGAR_PRUEBA","is_read_only":false,"is_family":false,"is_modifiable":false,"path":"D:\\IG INGENIER\u00cdA\\Hartree\\HANGAR_PRUEBA.rvt"}}}
======================================================================
3. POST /conn/op/no_existe/ -> 200  (esperado 200)  [OK]  c�digos: UNKNOWN_OPERATION
Cuerpo:
{"ok":false,"meta":{"operation":"no_existe","duration_ms":0,"addin_version":"0.1.0"},"errors":[{"hint":"Operaciones disponibles: ping, probe_delete_b, probe_plate_b.","path":null,"message":"La operaci\u00f3n 'no_existe' no existe en el add-in.","code":"UNKNOWN_OPERATION"}],"warnings":[],"data":null}
======================================================================
4. POST /conn/dev_exec/ print('hola') -> 200  (esperado 200)  [OK]  output='hola'
Cuerpo:
{"errors": [], "data": {"output": "hola\n", "description": "prueba"}, "meta": {"operation": "dev_exec", "duration_ms": 16, "addin_version": null}, "warnings": [], "ok": true}
======================================================================
Resultado: 4/4 pruebas correctas

```

## 13 sondeo 07 nudo

```text
== 07-nudo-seleccion.py -> HTTP 200 en 985 ms ==
=== 07-nudo-seleccion ===
1) Elementos seleccionados: 4
2) Miembros de armazon estructural con eje: 4
   [2390473] HSS3X3X1-4 76x76 : HSS3X3X1-4 76x76 | (236570.5, -41238.5, 17423.0) -> (226452.6, -41238.5, 17423.0) mm | L=10118 mm | pendiente=0.0 grados
   [2391296] HSS2-1-2X2-1-2X3-16 64x64 : HSS2-1-2X2-1-2X3-16 64x64 | (231605.2, -41238.3, 19960.1) -> (234112.6, -41238.3, 17452.7) mm | L=3546 mm | pendiente=45.0 grados
   [2391297] HSS2-1-2X2-1-2X3-16 64x64 : HSS2-1-2X2-1-2X3-16 64x64 | (234138.0, -41238.3, 17418.7) -> (236685.9, -41238.3, 19916.4) mm | L=3568 mm | pendiente=44.4 grados
   [2391299] HSS2-1-2X2-1-2X3-16 64x64 : HSS2-1-2X2-1-2X3-16 64x64 | (234112.6, -41238.4, 17452.7) -> (236662.6, -41238.4, 14957.2) mm | L=3568 mm | pendiente=44.4 grados
3) Cordon elegido (mas horizontal, mas largo): [2390473] HSS3X3X1-4 76x76
   Primer miembro (define el plano): [2391296] HSS2-1-2X2-1-2X3-16 64x64
4) Sistema local del nudo:
   origen (punto de trabajo) mm: (234142.3, -41238.4, 17423.0)
   X (eje cordon):  (-1.000000, 0.000002, 0.000000)
   Y:               (-0.000000, -0.000000, -1.000000)
   Z (normal plano):(-0.000002, -1.000000, 0.000000)
   distancia entre ejes: 0.24 mm (OK <= 5 mm)
   miembro [2391297] dista 0.24 mm del eje del cordon
   miembro [2391299] dista 0.15 mm del eje del cordon
5) Orden lista para la prueba del camino B (copiar y pegar en PowerShell):
   .\scripts\conn-call.ps1 -Operation probe_plate_b -Body '{"element_ids":[2390473,2391296,2391297,2391299],"chord_element_id":2390473}'
=== fin 07-nudo-seleccion ===


```

## 14 sondeo 08 copia

```text
== 08-guardar-copia.py -> HTTP 200 en 488 ms ==
=== 08-guardar-copia ===
Documento: HANGAR_PRUEBA
Ruta actual: D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA.rvt
ERROR: el modelo es de trabajo compartido (IsWorkshared=True). Haz la copia a mano: Archivo > Guardar como > Proyecto, nombre HANGAR_PRUEBA_sondeo.rvt, y sigue con esa copia abierta.
=== fin 08-guardar-copia ===


```

## 15 camino B probe_plate_b

```text
== conn/probe_plate_b -> HTTP 200 en 459 ms ==
{
    "data":  null,
    "warnings":  [

                 ],
    "errors":  [
                   {
                       "code":  "ELEMENT_NOT_FOUND",
                       "path":  "element_ids[0]",
                       "message":  "No existe ningún elemento con id 2390473.",
                       "hint":  "Selecciona los miembros en Revit y usa get_selected_elements para leer sus IDs."
                   }
               ],
    "ok":  false,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "duration_ms":  4,
                 "operation":  "probe_plate_b"
             }
}
ERROR ELEMENT_NOT_FOUND: No existe ningún elemento con id 2390473.

```

## 15 camino B probe_plate_b

```text
== conn/probe_plate_b -> HTTP 200 en 553 ms ==
{
    "data":  {
                 "chord":  {
                               "end_mm":  [
                                              -14397.600000000000,
                                              -17195.799999999999,
                                              17423
                                          ],
                               "slope_deg":  0,
                               "start_mm":  [
                                                -4437.3000000000002,
                                                -17195.700000000001,
                                                17423
                                            ],
                               "type":  "HSS3X3X1/4",
                               "length_mm":  9960.2999999999993,
                               "family":  "HSS-Hollow Structural Section",
                               "element_id":  1249510,
                               "structural_type":  "Beam"
                           },
                 "frame":  {
                               "y_axis":  [
                                              0,
                                              0,
                                              -1
                                          ],
                               "x_axis":  [
                                              -1,
                                              -2E-06,
                                              0
                                          ],
                               "z_axis":  [
                                              2E-06,
                                              -1,
                                              0
                                          ],
                               "axis_distance_mm":  0.080000000000000002,
                               "origin_mm":  [
                                                 -11867.700000000001,
                                                 -17195.799999999999,
                                                 17423
                                             ]
                           },
                 "undo_entry":  "MotorConexiones: probe_plate_b 20260930-195706",
                 "probe_id":  "20260930-195706",
                 "bolts":  {
                               "element_ids":  [
                                                   1321305,
                                                   1321306,
                                                   1321307,
                                                   1321308
                                               ],
                               "diameter_mm":  15.875,
                               "spacing_mm":  60,
                               "length_mm":  40
                           },
                 "members":  [
                                 {
                                     "end_mm":  [
                                                    -11930.600000000000,
                                                    -17195.799999999999,
                                                    17481.799999999999
                                                ],
                                     "slope_deg":  43.079999999999998,
                                     "start_mm":  [
                                                      -14536.799999999999,
                                                      -17195.799999999999,
                                                      19918.799999999999
                                                  ],
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3568,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "element_id":  1249630,
                                     "structural_type":  "Beam"
                                 },
                                 {
                                     "end_mm":  [
                                                    -9354.7000000000007,
                                                    -17195.799999999999,
                                                    19884.900000000001
                                                ],
                                     "slope_deg":  44.369999999999997,
                                     "start_mm":  [
                                                      -11856.5,
                                                      -17195.799999999999,
                                                      17437.299999999999
                                                  ],
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3500,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "element_id":  1249631,
                                     "structural_type":  "Beam"
                                 },
                                 {
                                     "end_mm":  [
                                                    -11904.900000000000,
                                                    -17195.700000000001,
                                                    17389.900000000001
                                                ],
                                     "slope_deg":  44.369999999999997,
                                     "start_mm":  [
                                                      -14455.299999999999,
                                                      -17195.700000000001,
                                                      14894.700000000001
                                                  ],
                                     "type":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "length_mm":  3567.9000000000001,
                                     "family":  "HSS2-1-2X2-1-2X3-16 64x64",
                                     "element_id":  1249636,
                                     "structural_type":  "Beam"
                                 }
                             ],
                 "plate":  {
                               "offset_x_mm":  0,
                               "height_mm":  200,
                               "width_mm":  200,
                               "thickness_mm":  10,
                               "element_id":  1321304
                           },
                 "backend":  "directshape"
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "duration_ms":  130,
                 "operation":  "probe_plate_b"
             }
}

```

## 15 camino B probe_delete_b

```text
== conn/probe_delete_b -> HTTP 200 en 458 ms ==
{
    "data":  {
                 "element_ids":  [
                                     1321304,
                                     1321305,
                                     1321306,
                                     1321307,
                                     1321308
                                 ],
                 "deleted":  5
             },
    "warnings":  [

                 ],
    "errors":  [

               ],
    "ok":  true,
    "meta":  {
                 "addin_version":  "0.1.0",
                 "duration_ms":  42,
                 "operation":  "probe_delete_b"
             }
}

```

## 16 camino A sondeo 09 placa

```text
== 09-placa-camino-a.py -> HTTP 200 en 4465 ms ==
=== 09-placa-camino-a ===
1) Tipos *Transaction* de la API de acero: Autodesk.SteelConnectionsDB.FabricationOnlyTransaction, Autodesk.SteelConnectionsDB.FabricationTransaction
   FabricationTransaction(Document, Boolean, String, Boolean)
   FabricationTransaction(Document, Boolean, String, Boolean, Boolean)
   FabricationTransaction(Document, Boolean, String)
   se usara Autodesk.SteelConnectionsDB.FabricationTransaction(Document, Boolean, String)
   Commit(): True; RollBack(): False; Dispose(): True
2) Point3d=True Vector3d=True Plane=True Plate=True
   ctores: Point3d(d,d,d)=True Vector3d(d,d,d)=True Plane(Point3d,Vector3d)=True Plate(Plane,Point3d[],Double)=True WriteToDb=True
3) Nudo: cordon [1249510], primer miembro [1249630], origen (-11867.7, -17195.8, 17423.0) mm, dist ejes 0.08 mm
   esquinas (mm): (-11767.7, -17195.8, 17523.0); (-11967.7, -17195.8, 17523.0); (-11967.7, -17195.8, 17323.0); (-11767.7, -17195.8, 17323.0)
   unidades pasadas a Advance Steel: pies
4) FabricationTransaction abierta. doc.IsModifiable=True
   ERROR ValueError: Object of type 'Autodesk.AdvanceSteel.Geometry.Plane' cannot be converted to type 'Autodesk.AdvanceSteel.Geometry.Plane'.
   doc.IsModifiable tras la operacion: False
5) Elementos nuevos en el documento: 0
=== fin 09-placa-camino-a ===


```

## 17 log add-in

```text


    Directorio: C:\Users\Andy Bayona Antón\AppData\Local\MotorConexiones\log


Mode                 LastWriteTime         Length Name                                                                 
----                 -------------         ------ ----                                                                 
-a----        30/09/2026     19:58           3054 motorconexiones-20260930.jsonl                                       
{"ts":"2026-09-30T19:28:21.5703037-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona AntÃ³n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:30:17.8900380-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":3}}
{"ts":"2026-09-30T19:35:04.5251598-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6045938-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6059620-05:00","record":{"event":"handle","operation":"no_existe","request_summary":"{}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:19.6081407-05:00","record":{"event":"handle","operation":"ping","request_summary":"esto no es json","ok":false,"error_codes":["INVALID_REQUEST"],"warning_codes":[],"duration_ms":1}}
{"ts":"2026-09-30T19:35:34.4497643-05:00","record":{"event":"handle","operation":"ping","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:35:34.4908316-05:00","record":{"event":"handle","operation":"no_existe","request_summary":"{}","ok":false,"error_codes":["UNKNOWN_OPERATION"],"warning_codes":[],"duration_ms":0}}
{"ts":"2026-09-30T19:41:05.2426378-05:00","record":{"event":"shutdown"}}
{"ts":"2026-09-30T19:41:41.3538541-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona AntÃ³n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:54:06.4529495-05:00","record":{"event":"shutdown"}}
{"ts":"2026-09-30T19:54:45.4481079-05:00","record":{"event":"startup","addin_version":"0.1.0","revit_version":"2027","revit_build":"27.2.0.39","assembly":"C:\\Users\\Andy Bayona AntÃ³n\\AppData\\Roaming\\Autodesk\\Revit\\Addins\\2027\\MotorConexiones\\MotorConexiones.Revit.dll"}}
{"ts":"2026-09-30T19:56:19.6737205-05:00","record":{"event":"handle","operation":"probe_plate_b","request_summary":"{\"element_ids\": [2390473, 2391296, 2391297, 2391299], \"chord_element_id\": 2390473}","ok":false,"error_codes":["ELEMENT_NOT_FOUND"],"warning_codes":[],"duration_ms":4}}
{"ts":"2026-09-30T19:57:06.6580668-05:00","record":{"event":"handle","operation":"probe_plate_b","request_summary":"{\"element_ids\": [1249510, 1249630, 1249631, 1249636], \"chord_element_id\": 1249510}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":130}}
{"ts":"2026-09-30T19:58:24.9436829-05:00","record":{"event":"handle","operation":"probe_delete_b","request_summary":"{}","ok":true,"error_codes":[],"warning_codes":[],"duration_ms":42}}



```
