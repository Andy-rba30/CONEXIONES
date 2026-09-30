# Fase 0: exploración, herramientas de sondeo y plan

Fecha: 2026-09-30. Rama: `claude/awesome-babbage-q4826b`. Sin código de producción (como pide el encargo).

## 1. Qué se hizo

- Instalado el SDK de .NET 10 en la sesión (10.0.112) y comprobado que NuGet sirve `Nice3point.Revit.Api.RevitAPI`
  y `RevitAPIUI` en versiones 2027.x. Compilado un proyecto de prueba `net10.0-windows` en Linux contra 2027.2.0.
- Clonado `revit-mcp` (commit `8505508`, 2026-09-24) y el módulo `routes` de pyRevit (commit `6c9d3a5`, 2026-09-30).
  Confirmados los diez hechos de la sección 3 del encargo leyendo el código; matices en la sección 4 de este informe.
- Escrito `scripts/revit-exec.ps1` (ejecuta IronPython dentro de Revit por Routes, sin MCP) y probado en la nube
  con PowerShell 7 contra un servidor falso que imita `/execute_code/`.
- Escritos los sondeos `scripts/sondeos/00-version.py`, `01-steel-api.py`, `02-perfiles.py`, `03-llamar-dll.py`
  y `04-ensamblados.py`. Solo leen; ninguno modifica el modelo.
- Escrito `docs/instalacion/fase-0.md` (pasos literales para el instalador) y `.gitignore`.
- Este informe: hechos verificados, plan preliminar, riesgos y preguntas que deben responder los sondeos.

## 2. Qué se probó en la nube y cómo

### 2.1 SDK de .NET 10

El script `dotnet-install.sh` **no se pudo descargar**: el proxy de la sesión rechaza `builds.dotnet.microsoft.com`
(HTTP 403 al `CONNECT`). En su lugar el SDK se instaló desde el archivo de paquetes de Ubuntu 24.04, que sí
está permitido:

```text
$ apt-get install -y dotnet-sdk-10.0      # paquete 10.0.112-0ubuntu1~24.04.1 (noble-updates)
$ dotnet --list-sdks
10.0.112 [/usr/lib/dotnet/sdk]
```

Para sesiones futuras: `apt-get update && apt-get install -y dotnet-sdk-10.0` (unos 2 minutos). PowerShell 7 también
se puede instalar (`packages.microsoft.com` está permitido): `packages-microsoft-prod.deb` + `apt-get install powershell`.

### 2.2 Paquetes NuGet de la API de Revit 2027

`https://api.nuget.org/v3-flatcontainer/<paquete>/index.json` respondió HTTP 200. Versiones 2027 disponibles
(idénticas para `RevitAPI` y `RevitAPIUI`): `2027.0.0-preview.1.20260110`, `2027.0.0`, `2027.0.10`, `2027.0.20`,
`2027.1.0`, `2027.2.0`, `2027.3.0`. Para Revit 2027.2 se usará **2027.2.0**.

| Paquete `Nice3point.Revit.Api.*` | Versión 2027 | Uso previsto |
|---|---|---|
| `RevitAPI`, `RevitAPIUI` | sí (hasta 2027.3.0) | add-in, compila en la nube |
| `RevitAPIIFC`, `AdWindows`, `RevitAddInUtility` | sí | no hacen falta (RevitAddInUtility solo en el sondeo 03) |
| `RevitAPISteel`, `ASMgd`, `ASObjectsMgd` | **no existen (HTTP 404)** | API de acero y Advance Steel: solo desde `C:\Program Files\Autodesk\Revit 2027\` en el PC |

Restauración y compilación reales en Linux de un proyecto de prueba (en el borrador de la sesión, no en el repositorio):

```xml
<TargetFramework>net10.0-windows</TargetFramework>
<EnableWindowsTargeting>true</EnableWindowsTargeting>
<UseWPF>true</UseWPF>
<PackageReference Include="Nice3point.Revit.Api.RevitAPI" Version="2027.2.0" />
<PackageReference Include="Nice3point.Revit.Api.RevitAPIUI" Version="2027.2.0" />
```

```text
Restored ApiCheck.csproj (in 3.34 sec).      # RevitAPI.dll 27.2.0.0, carpeta ref/net10.0-windows7.0
Build succeeded.  0 Warning(s)  0 Error(s)   # también descargó Microsoft.WindowsDesktop.App.Ref (WPF) sin problema
```

El archivo de prueba usa, y por tanto **existen en la API 2027** (el compilador lo garantiza):
`ElementId.Value` (long) y `new ElementId(long)`; `Document.IsModifiable`, `IsReadOnly`, `ProjectInformation.UniqueId`,
`Document.GetDocumentVersion(Document)`; `TransactionGroup`/`Transaction` con `FailureHandlingOptions.SetForcedModalHandling(false)`,
`SetClearAfterRollback(true)`, `SetFailuresPreprocessor(IFailuresPreprocessor)`, `FailuresAccessor.GetFailureMessages()`,
`DeleteAllWarnings()`; `UIApplication.DialogBoxShowing` con `DialogBoxShowingEventArgs.DialogId` y `OverrideResult(int)`;
`UnitUtils.ConvertToInternalUnits/ConvertFromInternalUnits(double, ForgeTypeId)` con `UnitTypeId.Millimeters` y `Degrees`,
`UnitFormatUtils.Format`; `LocationCurve.Curve`, `Curve.GetEndPoint`, `XYZ.CrossProduct/Normalize/DistanceTo`;
`FamilyInstance.StructuralType`, `Symbol.FamilyName`, `StructuralMaterialId`, `BuiltInParameter.START_EXTENSION/END_EXTENSION`;
`FamilySymbol.GetStructuralSection()` con `StructuralSectionShape` y `StructuralSectionRectangleHSS.Width/Height/WallNominalThickness`;
Extensible Storage completo (`SchemaBuilder`, `SetSchemaName`, `AddSimpleField`, `Finish`, `Schema.Lookup`, `DataStorage.Create`,
`Entity.Set/Get<string>`, `SetEntity/GetEntity`); `Autodesk.Revit.DB.Structure.StructuralConnectionHandler` y
`StructuralConnectionHandlerType`; `Solid.IntersectWithCurve`, `BooleanOperationsUtils.ExecuteBooleanOperation`,
`ElementIntersectsElementFilter`; `UIControlledApplication.CreateRibbonTab/CreateRibbonPanel`, `PushButtonData`, `TaskDialog.Show`,
`UIDocument.Selection.GetElementIds()`, `UIApplication.GetRibbonPanels(string)`.

Además se volcaron por metadatos las firmas de `StructuralConnectionHandler` (sección 5.3) y se comprobó que en
`RevitAPI.dll` 2027 el espacio `Autodesk.Revit.DB.Steel` contiene **un solo tipo**, `SteelElementProperties`
(`GetSteelElementProperties(Element)`, `AddFabricationInformationForRevitElements(Document, IList<ElementId>)`,
`GetFabricationUniqueID`, `GetReference`). El resto de la API de acero (placas, pernos, soldaduras, `FabricationTransaction`...)
no está en `RevitAPI.dll`: tiene que venir de `RevitAPISteel.dll` o de los ensamblados de Advance Steel. Lo confirma el sondeo 01.

### 2.3 Lectura de `revit-mcp` y de pyRevit

Archivos leídos enteros: `startup.py`, `main.py`, `revit_mcp/seguridad.py`, `revit_mcp/utils.py`, `revit_mcp/code_execution.py`,
`revit_mcp/status.py`, `tools/__init__.py`, `tools/utils.py`, `tools/status_tools.py`, `tools/code_execution_tools.py`,
`CONTRATO.md`, `README.md`, `pruebas/probar_revit.py`; y de pyRevit `pyrevitlib/pyrevit/routes/server/{handler,server,base}.py`,
`routes/api.py`, `routes/__init__.py`. Resultado en la sección 4.

### 2.4 `scripts/revit-exec.ps1`

Probado con PowerShell **7.6.6 en Linux** contra un servidor local falso que imita `/revit_mcp/execute_code/`
(token, 200 con `output`, 401, 500 con `traceback` y `hints`). Sin errores de sintaxis (`Parser::ParseFile`).

| Caso | Resultado |
|---|---|
| Sondeo válido, token correcto | `== 00-version.py -> HTTP 200 en 85 ms ==` + salida con acentos intactos, código de salida 0 |
| Token incorrecto | `HTTP 401` + `ERROR 401: token ausente o incorrecto...`, salida 1 |
| El código lanza excepción | `HTTP 500` + `ERROR`, `TIPO`, `TRACEBACK`, `PISTAS`, salida 1 |
| `-Json` | cuerpo JSON íntegro, salida 0 |
| Revit apagado (puerto sin servidor) | `ERROR: no se pudo hablar con Revit en ... (Connection refused ...)`, salida 1 |
| Archivo inexistente / sin archivo de token | mensaje claro, salida 1 |

**NO PROBADO en Windows PowerShell 5.1** (no hay Windows en la sesión). El script evita `Invoke-WebRequest` y usa
`System.Net.Http.HttpClient`, que existe en 5.1 y 7, y lleva BOM UTF-8 para que 5.1 lea bien los acentos.
El instalador lo prueba de verdad en el paso 5 de `docs/instalacion/fase-0.md`.

### 2.5 Sondeos `scripts/sondeos/*.py`

Solo se pudo comprobar la sintaxis con CPython 3 (`python3 -m py_compile`, correcto). Están escritos para IronPython 2.7
(`from __future__ import print_function`, sin acentos en los textos para evitar los problemas de codificación de
IronPython 2.7 que documenta el propio pyRevit). **PENDIENTE DE INSTALADOR**: paso 5 de `docs/instalacion/fase-0.md`.

## 3. Qué debo mirar cuando el instalador termine

1. Paso 3: `dotnet --list-sdks` debe mostrar una línea `10.x`. Si no, instala el SDK de .NET 10 antes de la Fase 1
   (https://dotnet.microsoft.com/download/dotnet/10.0, "SDK", instalador x64).
2. Paso 4: `status: active` y el título del modelo de la cercha.
3. Paso 5: cada sondeo empieza por `== NN-nombre.py -> HTTP 200` y termina en `=== fin NN-... ===`. Un `HTTP 500` con
   `TRACEBACK` también es información útil: no lo repitas más de dos veces, envíamelo tal cual.
4. Sondeo 00: `VersionNumber: 2027`, `Runtime .NET: .NET 10.x`, la línea `Ruta .rvt:` (necesito esa ruta para el encargo) y
   `Armazon estructural (instancias)` mayor que cero.
5. Sondeo 02: los tipos HSS del cordón y las diagonales con `forma=RectangleHSS` (o similar) y medidas en mm. Anota con
   qué nombres exactos aparecen (por ejemplo `HSS3X3X1/4`) porque son los que irán en `profile` de la especificación.
6. Sondeo 03: las líneas `2a)`, `2b)` y `3)` deben acabar en valores, no en `ERROR`. Si `2a` da `3.2808...` y `3)` lista
   productos de Revit instalados, el mecanismo de `Bridge.Handle` por reflexión funciona.
7. Sondeo 04: si la pestaña `Steel`/`Acero` tiene paneles y aparecen archivos `*Steel*.dll` o `AS*Mgd.dll`, el camino A
   (fabricación de acero) es posible. Si no, en la Fase 1 iremos directos al camino B.
8. Guarda el archivo `docs/fases/resultados-fase-0.md` en la rama (paso 7) o pega su contenido en el prompt de la Fase 1.

## 4. Hechos de la sección 3 del encargo, verificados en el código

| # | Hecho | Estado | Dónde se ve | Matiz encontrado |
|---|---|---|---|---|
| 1 | Los manejadores reciben `doc`, `uidoc`, `request` por nombre y corren en contexto de la API (ExternalEvent) | **Confirmado** | pyRevit `routes/server/server.py` (`EVENT_HNDLR = UI.ExternalEvent.Create(REQUEST_HNDLR)`, `_call_host_event_sync`) y `handler.py` (`RequestHandler(UI.IExternalEventHandler)`, `prepare_handler_kwargs`, `filter_kwargs`) | **Solo si el manejador declara `doc`, `uidoc` o `uiapp`** (`wants_api_context`). Un manejador sin esos argumentos corre en el hilo HTTP, fuera del contexto de la API. Todas las rutas `conn_*`, incluida `conn_ping`, declararán `doc` y `uidoc`. Hay un único `RequestHandler` compartido: las peticiones deben ir de una en una (el MCP ya lo hace). `uiapp` también se puede pedir por nombre. |
| 2 | Toda ruta exige token; `@requiere_token` bajo `@api.route` | Confirmado | `revit_mcp/seguridad.py`; `startup.py` genera 64 hex y lo escribe en `%LOCALAPPDATA%\RevitMcp\token` | El decorador regenera una función con los mismos nombres de parámetros, así que no rompe el hecho 1. Quita la clave `token` del cuerpo antes de llamar al manejador. Las cabeceras HTTP no llegan al manejador (`base.Request._headers` vacío). |
| 3 | `revit_post` devuelve dict solo con HTTP 200; si no, texto `Error: <código> - <cuerpo>` | Confirmado | `main.py: _revit_call` | Firma: `revit_post(endpoint, data, ctx=None, timeout=30.0, params=None)`; `timeout` se pasa como argumento con nombre. Ante 401 relee el token y reintenta una vez. |
| 4 | `format_response` aplana los dicts | Confirmado | `tools/utils.py` | Devuelve `output`, `message`, `result` o `data` sueltos y el resto como "Clave: valor". `conn_*` no lo usará. |
| 5 | `sanitize_string` convierte acentos en `?` | Confirmado | `revit_mcp/utils.py` (`encode('ascii','replace')`) | pyRevit serializa las respuestas con `json.dumps(ensure_ascii=True)`: los acentos viajan como `\uXXXX` y llegan intactos. Basta con no pasar los textos por `sanitize_string`/`normalize_string`. |
| 6 | Tiempo de espera por defecto 30 s | Confirmado | `main.py: _revit_call(timeout=30.0)`; `revit_image` usa 60 s | `conn_create/update/preview` pasarán `timeout=180.0`. |
| 7 | `suppress_warnings(t)` evita diálogos con `IFailuresPreprocessor` | Confirmado | `revit_mcp/utils.py: _FailureSwallower` | Usa `SetForcedModalHandling(False)` + `SetClearAfterRollback(True)`; borra avisos y hace rollback ante errores. Mismo patrón para C#. |
| 8 | Herramientas ya existentes que no hay que duplicar | Confirmado | `README.md` (48 herramientas) y `tools/*.py` | `execute_revit_code` expone `doc`, `DB`, `revit`, `clr`, `System` y `print`; `uidoc` sale de `revit.uidoc`. Todo corre dentro de `TransactionGroup` "IA: ..." **y de una `Transaction` ya abierta**: el código no puede abrir otra (ver riesgo R4). |
| 9 | Herramienta nueva = 2 archivos + 2 líneas de registro | Confirmado | `startup.py: register_routes()`, `tools/__init__.py: register_tools()` | Los módulos IronPython importan con `from seguridad import requiere_token` y `from utils import ...` (sin prefijo `revit_mcp.`). `conexiones.py` seguirá ese patrón. Las herramientas CPython importan `Context` de `mcp.server.mcpserver` (SDK mcp 2.x, `pyproject.toml`: `mcp[cli]>=2.2,<3`). |
| 10 | `get_element_id_value` / `make_element_id`; `.Value`, nunca `IntegerValue` | Confirmado | `revit_mcp/utils.py`, `LLM.txt` | El README lista las API retiradas en 2027 (importación AXM/FormIt, miembros de `Mechanical.Zone`, creación de armaduras heredada, propiedades de `EnergyDataSettings`): ninguna afecta al plan. El último commit del MCP carga explícitamente ensamblados de ACL en `startup.py`, señal de que pyRevit corre IronPython sobre .NET 10 en 2027. |

Detalles extra útiles del contrato de `/execute_code/` (usados por `revit-exec.ps1`): POST con `Content-Type: application/json`,
cuerpo `{"code", "description", "token"}`; éxito `200 {"status":"success","output",...}`; excepción `500 {"status":"error",
"error","error_type","traceback","hints"?,"partial_output"?,"open_transaction"?}`; sin `code` `400`; sin token `401`.
La descripción se recorta a 60 caracteres y da nombre a la entrada de deshacer.

## 5. Plan preliminar (se cierra al empezar la Fase 1 con los resultados de los sondeos)

### 5.1 Compilación en la nube y en el PC

- `MotorConexiones.Core` (netstandard2.0) y `MotorConexiones.Tests` (net10.0, xUnit): compilan y se prueban en la nube.
- `MotorConexiones.Revit` (`net10.0-windows`, `EnableWindowsTargeting`, `UseWPF` para los iconos de la cinta) con los paquetes
  `Nice3point.Revit.Api.RevitAPI/RevitAPIUI` 2027.2.0: **compila en la nube** (demostrado en 2.2).
- Todo lo que dependa de `RevitAPISteel.dll` o de Advance Steel **no compila en la nube** (no está en NuGet). Si el camino A
  gana en la Fase 1, ese código irá en un proyecto aparte (`MotorConexiones.Revit.Steel`) que referencia las DLL de
  `C:\Program Files\Autodesk\Revit 2027\` con `Private=false` y se compila solo en el PC con `deploy.ps1`; el add-in lo carga
  por reflexión detrás de `IFabricationBackend`. La nube seguirá compilando Core, Tests y Revit.
- JSON: `System.Text.Json` (forma parte del runtime .NET 10 de Revit 2027; en Core, paquete NuGet para netstandard2.0).
  Validación de esquema con un validador propio y pequeño (obligatorios, tipos, rangos, `additionalProperties`) para no
  meter en Revit una librería más; se decide en la Fase 2.

### 5.2 Puente MCP → add-in (`Bridge.Handle`)

1. `revit_mcp/conexiones.py` busca `MotorConexiones.Revit` en `AppDomain.CurrentDomain.GetAssemblies()`, obtiene
   `MotorConexiones.Revit.Bridge` con `GetType` y `Handle` con `GetMethod(nombre, Type[])` (String, String, Document, UIDocument) e invoca.
   El sondeo 03 prueba exactamente esa mecánica con `RevitAPI` (`UnitUtils.ConvertToInternalUnits`, `Document.GetDocumentVersion`).
2. Si la DLL no está cargada: `clr.AddReferenceToFileAndPath` con la ruta desplegada. El sondeo 03 lo prueba con `RevitAddInUtility.dll`.
3. Si Revit 2027 aísla los add-ins en un `AssemblyLoadContext` propio y eso rompe la identidad de tipos, plan B del encargo:
   `HttpListener` en `127.0.0.1:48885` con el mismo token. El sondeo 03 (apartado 4) muestra el contexto de carga de `RevitAPI`,
   de pyRevit y de los add-ins, que es lo que decide.
4. Todos los manejadores `conn_*` declaran `doc` y `uidoc` para correr en contexto de la API (matiz del hecho 1).

### 5.3 Geometría: camino A frente a camino B

- **A (fabricación de acero)**: en `RevitAPI.dll` solo hay `SteelElementProperties` y las clases `StructuralConnectionHandler`
  (`Create(doc, ids, typeId)`, `Create(doc, ids, typeName)`, `CreateGenericConnection(doc, ids)`, `GetConnectedElementIds()`,
  `GetOrigin()`, `IsDetailed()`) y `StructuralConnectionHandlerType` (`FindGenericConnectionType(doc)`,
  `CreateDefaultStructuralConnectionHandlerType(doc)`, `IsGeneric/IsCustom/IsDetailed`). Las placas, pernos y soldaduras
  reales son objetos de Advance Steel (`RevitAPISteel.dll`, `AS*Mgd.dll`): los sondeos 01 y 04 dicen si esas DLL existen y
  qué clases traen. La prueba mínima de la Fase 1 (placa + 4 pernos) se hará primero en IronPython con esas DLL.
- **B (familias propias)**: compila entero en la nube. Variante a valorar en la Fase 1: `DirectShape` para placas y pernos
  (sólidos por extrusión, sin archivos `.rfa` ni plantillas de familia; categoría "Conexiones estructurales"), y los retiros de
  extremo con `START_EXTENSION`/`END_EXTENSION` de cada barra. La ranura del HSS solo sería representable con A o con una
  familia de vacío; en B quedaría documentada como limitación.
- Recomendación provisional: A si el sondeo 04 muestra la pestaña Steel y las DLL, y la prueba de la Fase 1 crea la placa
  sin diálogos; si no, B con `DirectShape`.

### 5.4 Almacenamiento, transacciones y registro (Fase 3)

- Extensible Storage: `SchemaBuilder` tiene `AcceptableName(string)`; el sondeo 03 (apartado 5) dice si Revit acepta
  `MotorConexiones.Connection` con punto o hay que usar `MotorConexionesConnection`.
- `TransactionGroup` "MotorConexiones: <operación> <connection_id>" + `IFailuresPreprocessor` + `DialogBoxShowing`
  (`OverrideResult`), todo compilado ya contra 2027.
- `Bridge.Handle` escribe una línea JSON por llamada en `%LOCALAPPDATA%\MotorConexiones\log\`.

### 5.5 Fases siguientes (sin cambios respecto al encargo)

Fase 1 prueba técnica (esqueleto, `deploy.ps1`, `instalar-conn.ps1`, `conn_ping` de punta a punta, placa + 4 pernos);
Fase 2 Core con fixture Detalle D y pruebas; Fase 3 add-in; Fase 4 MCP; Fase 5 punta a punta y README.

## 6. Decisiones tomadas y por qué

- **SDK por `apt` y no por `dotnet-install.sh`**: el proxy bloquea los servidores de descarga de Microsoft; el archivo de
  Ubuntu está permitido y trae 10.0.112. Mismo SDK mayor (10) que el PC.
- **Versión de paquetes 2027.2.0**: coincide con Revit 2027.2 del PC.
- **`revit-exec.ps1` con `HttpClient` y BOM UTF-8**: para que funcione igual en Windows PowerShell 5.1 y en PowerShell 7,
  y para no depender de `Invoke-WebRequest` (que en 5.1 lanza excepción con 4xx/5xx y pierde el cuerpo).
- **Sondeos sin acentos y con `print_function`**: IronPython 2.7 puede fallar con textos no ASCII (lo documenta el propio
  pyRevit en `handler.py`); con `print_function` la salida pasa por el `print` capturado de `/execute_code/`.
- **Sondeos solo de lectura**: aunque `/execute_code/` deshace en caso de error, no quiero tocar el modelo hasta la Fase 1.
- **Comprobación de la API por compilación**: en vez de fiarme de memoria, los miembros que usará el plan se compilaron
  contra 2027.2.0 (sección 2.2). Lo que no se pudo compilar (API de acero) queda en los sondeos.
- **Sin `deploy.ps1` ni `instalar-conn.ps1` todavía**: la sección 13 los sitúa en la Fase 1 y aún no hay solución que desplegar.

## 7. Pendientes, riesgos y preguntas

### 7.1 Preguntas que deben responder los sondeos

| Sondeo | Pregunta |
|---|---|
| 00 | ¿Revit 2027.2 sobre .NET 10? ¿Qué IronPython y pyRevit? ¿Idioma de Revit? ¿Ruta del `.rvt` de prueba y cuántas barras tiene? |
| 01 | ¿Existe `RevitAPISteel.dll`? ¿Y `AS*Mgd.dll`? ¿Se cargan con `clr.AddReferenceToFileAndPath`? ¿Qué clases de placas/pernos/soldaduras exponen? |
| 02 | ¿Cómo se llaman exactamente los tipos HSS cargados y cuáles usa la cercha? ¿Traen sección estructural con medidas? ¿Qué retiros de extremo tienen las barras? |
| 03 | ¿Funciona buscar un ensamblado por nombre e invocar un método estático por reflexión con un `Document`? ¿Y cargar una DLL por ruta? ¿En qué `AssemblyLoadContext` viven RevitAPI, pyRevit y los add-ins? ¿Acepta Revit un nombre de esquema con punto? |
| 04 | ¿Hay pestaña Steel/Acero? ¿Qué complementos de acero hay instalados? ¿Qué tipos de conexión estructural tiene el documento? |

### 7.2 Riesgos

- **R1. API de acero no compilable en la nube.** Mitigación: prototipos en IronPython por sondeo y proyecto aparte compilado
  solo en el PC (5.1). Si el usuario prefiere que todo compile en la nube, el camino B es la alternativa segura.
- **R2. Aislamiento de add-ins en Revit 2027.** Si el sondeo 03 muestra que pyRevit y los add-ins viven en contextos de
  carga distintos y la reflexión falla con `Document`, se aplica el plan B (`HttpListener` 48885).
- **R3. Advance Steel puede exigir sus propias transacciones** (`FabricationTransaction` o similar) que no admiten
  anidarse en la `Transaction` que ya abre `/execute_code/`. Mitigación en la Fase 1: si ocurre, la prueba se ejecutará desde
  un botón de pyRevit o una ruta nueva sin transacción envolvente.
- **R4. Los sondeos corren dentro de una `Transaction` abierta**: por eso no abren otra ni crean elementos.
- **R5. Windows PowerShell 5.1 no probado**: si `revit-exec.ps1` falla en el PC, el error literal del paso 5 dirá por qué y se
  corrige en la Fase 1 (alternativa preparada: `Invoke-RestMethod` con `-SkipHttpErrorCheck` en PowerShell 7).
- **R6. Idioma de la interfaz**: los nombres de pestañas y categorías cambian con el idioma; el sondeo 00 lo muestra y el
  add-in usará solo `BuiltInCategory` y no nombres traducidos.

### 7.3 Preguntas para ti

1. ¿Cuál es la ruta del `.rvt` de prueba? (Sección 2 del encargo tiene `[COMPLETAR]`; el sondeo 00 la imprime.)
2. ¿Tienes la imagen del Detalle D para `docs/fixtures/detalle-D.png`? Si la tienes, súbela antes de la Fase 2.
3. Si el camino A falla, ¿prefieres familias `.rfa` (como dice el encargo) o `DirectShape` sin archivos de familia (5.3)?
4. ¿Tu PC tiene ya el SDK de .NET 10? (Lo dirá el paso 3 del instalador; sin él no hay Fase 1 en el PC.)

## 8. Conclusiones con los resultados del PC (`docs/fases/resultados-fase-0.md`, commit `d0e442e`)

Los cinco sondeos respondieron `HTTP 200` sin ninguna ventana en Revit. `revit-exec.ps1` funcionó en el PC
(el instalador tuvo que renormalizar finales de línea del clon; por eso se añade `.gitattributes` y el script ya
convierte CRLF a LF antes de enviar el código).

| Pregunta | Respuesta del PC | Consecuencia para el plan |
|---|---|---|
| Entorno | Revit 2027.2 (27.2.0.39), .NET 10.0.12, IronPython 2.7.12 sobre .NET 10, pyRevit 6.5.3, idioma English_USA, SDK .NET 10.0.401 en el PC | Todo lo previsto vale. El PC puede compilar. |
| Modelo | `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA.rvt`, unidades en metros, 1064 barras, 145 pilares, 0 cerchas (categoría), 6 elementos de conexión estructural, 0 `StructuralConnectionHandler` | La cercha está hecha de barras sueltas (armazón estructural), no de un elemento cercha: encaja con `conn_get_node_info` por IDs. |
| Puente `Bridge.Handle` | Reflexión con `Document` funciona; `RevitAPI`, `pyRevitLoader` y los add-ins del usuario viven en el `AssemblyLoadContext` **Default**; solo "Revit Assistant" usa un `AddInLoadContext` aislado | **Camino principal confirmado**: `conexiones.py` localizará `MotorConexiones.Revit` en el AppDomain y llamará a `Bridge.Handle` por reflexión. El plan B (`HttpListener`) queda solo como reserva. |
| Extensible Storage | `AcceptableName("MotorConexiones.Connection") = False`; sin punto, `True` | El esquema se llamará `MotorConexionesConnection`. |
| API de acero | `RevitAPISteel.dll` cargado (24 tipos en `Autodesk.Revit.DB.Steel`: `SteelConnectionUtil`, `SteelModelManager`, `SteelProxyElement`, `StructuralConnectionBaseUtil`...; **no hay** tipos de placa, perno ni soldadura, ni `FabricationTransaction`). El módulo Steel Connections está cargado (`Autodesk.SteelConnectionsDB/UI.dll` en `Revit 2027\AddIns\SteelConnections\`). Los ensamblados de Advance Steel (`ASMgd`, `ASObjectsMgd`) **no aparecen** cargados ni en la carpeta raíz de Revit. El documento tiene 7 tipos de conexión (`Generic Connection`, `Shear plate`...). La comprobación de la pestaña `Steel` con `GetRibbonPanels` no es concluyente: ese método solo ve pestañas creadas por API. | El camino A sigue abierto pero **sin evidencia todavía de una API de placas y pernos**. Primer trabajo de la Fase 1: sondeo que liste `C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\` y pruebe a cargar `ASMgd.dll`/`ASObjectsMgd.dll` desde ahí; si no existen o no exponen placas y pernos, se va al camino B (`DirectShape`). |
| Perfiles | Dos familias distintas de HSS: `HSS-Hollow Structural Section` (tipo `HSS3X3X1/4`, 12 instancias, sección `GeneralH`; tipo `HSS2-1/2X2-1/2X3/16` sin instancias y sin sección) y las familias `HSS2-1-2X2-1-2X3-16 64x64` (436 instancias, `RectangleHSS` 63,5×63,5×4,76 mm) y `HSS3X3X1-4 76x76` (152 instancias, 76,2×76,2×6,35 mm). Retiros de extremo 0. | La validación `PROFILE_MISMATCH` no puede comparar nombres tal cual: el plano dice `HSS2-1/2X2-1/2X3/16` y el modelo `HSS2-1-2X2-1-2X3-16 64x64`. La Fase 2 comparará la designación AISC normalizada (`/`→`-`, sin sufijo de medidas) **y** las medidas de `GetStructuralSection()` contra las pulgadas de la etiqueta, con las tres sugerencias más parecidas. `conn_get_node_info` devolverá familia, tipo y medidas de la sección. |
| Sondeos dentro de `/execute_code/` | Correcto y rápido (menos de 1,2 s cada uno) | Se mantiene como vía de prueba para la Fase 1. |

Pendientes que hereda la Fase 1: (1) sondeo de `AddIns\SteelConnections\` y de los ensamblados `AS*`; (2) elegir los IDs
del nudo del Detalle D en el modelo (pedir al usuario que lo seleccione y usar `get_selected_elements`); (3) decidir A o B con la
prueba de placa + 4 pernos.
