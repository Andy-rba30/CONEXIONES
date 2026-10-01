# Fase 1: prueba técnica

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt` (avanzada por *fast-forward* desde
`claude/awesome-babbage-q4826b`, que es donde quedó la Fase 0; no hubo conflictos). Versión del add-in: 0.1.0.

## 1. Qué se hizo

- **Plan cerrado con los sondeos de la Fase 0** (sección 4 de este informe): puente por reflexión confirmado como
  camino principal; esquema de Extensible Storage sin punto; la decisión A/B queda para la prueba de escritura de
  esta fase, con las reglas de seguridad del instalador.
- **Esqueleto de la solución** `MotorConexiones.sln` con tres proyectos:
  - `src/MotorConexiones.Core` (netstandard2.0): sobre de respuesta común (`Contract/ApiResponse`, `ApiError`,
    `ApiMeta`, `JsonOptions`), `Units/UnitConverter.cs` (único sitio de conversión mm↔pies y grados↔radianes),
    `Geometry3D/Vec3` y `Geometry3D/NodeFrame` (sistema local del nudo de la sección 7, con los errores
    `NODE_AXES_NOT_INTERSECTING` y `NODE_AXES_PARALLEL`), `Types/IConnectionType` + `ConnectionTypeRegistry`,
    `Validation/ErrorCodes`.
  - `src/MotorConexiones.Revit` (net10.0-windows, paquetes `Nice3point.Revit.Api.*` 2027.2.0): `App.cs` (pestaña
    "Conexiones" y botón "Ejecutar especificación JSON", por ahora solo informativo), `Bridge.Handle(string, string,
    Document, UIDocument)`, operaciones `ping`, `probe_plate_b` (placa + 4 pernos con DirectShape) y `probe_delete_b`,
    `Transactions/OperationScope` (TransactionGroup + `IFailuresPreprocessor` + `DialogBoxShowing`, todo sin ventanas),
    `Node/NodeInspector` (lee miembros, elige cordón, calcula el sistema local), `Fabrication/IFabricationBackend` +
    `DirectShapeBackend`, `Logging/JsonLineLogger` (JSON por línea en `%LOCALAPPDATA%\MotorConexiones\log\`).
  - `src/MotorConexiones.Tests` (xUnit, net10.0): 20 pruebas del Core.
- `scripts/deploy.ps1` (compila y copia a `%APPDATA%\Autodesk\Revit\Addins\2027\`), `scripts/conn-call.ps1`
  (llama a `/conn/ping/` y `/conn/op/<operación>/`) y `scripts/revit-exec.ps1 -SinTransaccion` (ejecuta IronPython
  sin la transacción envolvente de `/execute_code/`).
- `mcp/revit_mcp/conexiones.py` (rutas `/conn/ping/`, `/conn/op/<operation>/` y la ruta de desarrollo
  `/conn/dev_exec/`), `mcp/tools/conn_tools.py` (herramienta `conn_ping`), `mcp/instalar-conn.ps1` (idempotente) y
  `mcp/pruebas/probar_conexiones.py` (4 pruebas de punta a punta por HTTP con token).
- Sondeos `05-bridge-ping.py` (puente por reflexión desde `/execute_code/`), `06-steel-miembros.py` (volcado completo
  de `RevitAPISteel.dll`, `Autodesk.SteelConnectionsDB.dll`, `AddIns\SteelConnections\` y los ensamblados `AS*Mgd`),
  `07-nudo-seleccion.py` (nudo seleccionado y sistema local), `08-guardar-copia.py` (copia `_sondeo.rvt`),
  `09-placa-camino-a.py` (placa de Advance Steel, guardada por el contexto de acero) y `10-pernos-camino-a.py`.
- `config/limits.json` (esqueleto; la Fase 2 lo rellena) y `docs/guide.md` (sección 11 del encargo).
- `docs/instalacion/fase-1.md` con 20 pasos literales y 4 capturas, y este informe.

## 2. Qué se probó en la nube y cómo

### 2.1 Compilación y pruebas

```text
$ dotnet build MotorConexiones.sln -c Release
  MotorConexiones.Core  -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
Build succeeded.  0 Warning(s)  0 Error(s)          (TreatWarningsAsErrors activado en los tres proyectos)

$ dotnet test -c Release
Passed!  - Failed: 0, Passed: 20, Skipped: 0, Total: 20
```

El proyecto `MotorConexiones.Revit` **compila en la nube contra la API 2027.2.0**, así que todos los miembros de la API
que usa existen: `DirectShape.CreateElement/IsValidCategoryId/SetShape/ApplicationId/ApplicationDataId`,
`GeometryCreationUtilities.CreateExtrusionGeometry`, `CurveLoop.Create`, `Line.CreateBound`, `Arc.Create(center, r, a0, a1, x, y)`,
`Transform.Identity` con `Origin/BasisX/BasisY/BasisZ`, `TransactionGroup.Assimilate/RollBack`, `FailureHandlingOptions`,
`UIApplication.DialogBoxShowing`, `TaskDialogShowingEventArgs.Message`, `MessageBoxShowingEventArgs.Message`,
`DialogBoxShowingEventArgs.OverrideResult`, `FamilyInstance.StructuralType`, `Document.Delete(ICollection<ElementId>)`,
`Application.Language/VersionBuild/SubVersionNumber`, `UIDocument.Application`. La salida son solo dos DLL
(`MotorConexiones.Revit.dll` y `MotorConexiones.Core.dll`); `System.Text.Json` lo pone el runtime .NET 10 de Revit
(se referencia la misma versión 10.0.12 que mostró el sondeo 00).

Pruebas del Core (20): conversión mm→pies→mm reversible con error < 0,001 mm, 1 pie = 304,8 mm, grados↔radianes,
redondeo a 0,1 mm; sistema local del nudo (cercha vertical, cercha horizontal, cordón invertido, ejes a 12 mm →
`NODE_AXES_NOT_INTERSECTING` con la distancia en el mensaje, ejes a 4 mm → origen en el punto medio, ejes paralelos,
ida y vuelta local↔global); sobre de respuesta (forma exacta de la sección 10, acentos sin escapar, `snake_case`);
registro de tipos de conexión.

### 2.2 Scripts de PowerShell (PowerShell 7.6.6 en Linux, contra un servidor falso que imita Routes)

Sintaxis comprobada con `Parser::ParseFile` en los cuatro scripts. Casos ejecutados:

| Script | Caso | Resultado |
|---|---|---|
| `conn-call.ps1 -Operation ping` | GET con token | `HTTP 200`, JSON con sangría y acentos legibles, salida 0 |
| `conn-call.ps1 -Operation probe_plate_b -Body '{"element_ids":[1,2],"chord_element_id":1}'` | POST con token insertado en el cuerpo | `ok: true`, el servidor recibió el cuerpo intacto, salida 0 |
| `conn-call.ps1 -Operation nada` | operación desconocida | imprime `ERROR UNKNOWN_OPERATION: ...`, salida 1 |
| `conn-call.ps1` sin archivo de token | | mensaje claro, salida 1 |
| `revit-exec.ps1 -File ... -SinTransaccion` | éxito | imprime `data.output`, salida 0 |
| `revit-exec.ps1 -File ... -SinTransaccion` | excepción | salida parcial + `ERROR PROBE_EXCEPTION` + `PISTA` + `TRACEBACK`, salida 1 |
| `revit-exec.ps1 -File ...` | `/execute_code/` como antes | sin cambios de comportamiento |
| `mcp/instalar-conn.ps1 -Extension <clon de revit-mcp>` | dos ejecuciones seguidas | la primera copia 2 archivos e inserta 2+2 líneas (`diff` comprobado); la segunda no cambia nada; `startup.py` y `tools/__init__.py` modificados compilan |

**NO PROBADO en Windows PowerShell 5.1** (no hay Windows en la sesión); los scripts usan las mismas construcciones que
`revit-exec.ps1`, que sí funcionó en el PC en la Fase 0. `deploy.ps1` solo se pudo comprobar en sintaxis: necesita
`%APPDATA%` de Windows (**PENDIENTE DE INSTALADOR**, paso 5).

### 2.3 Archivos del MCP

- `conexiones.py`: lógica probada en CPython 3 con módulos de pyRevit simulados (`pyrevit`, `seguridad`, `clr`,
  `System`): registra las tres rutas; sin add-in responde 200 con `ADDIN_NOT_LOADED`; la ruta genérica entrega el
  nombre de operación y el cuerpo; `dev_exec` captura `print`, devuelve traceback y código `PROBE_EXCEPTION`, deshace
  las transacciones que dejó vivas el código, avisa `OPEN_TRANSACTION` y restaura `sys.stdout`. **La ejecución real en
  IronPython 2.7 dentro de Revit es PENDIENTE DE INSTALADOR** (pasos 8, 10, 13-16).
- `conn_tools.py`: registrado con el SDK `mcp` 2.x real (`MCPServer`) desde el `tools/__init__.py` modificado por el
  instalador: 49 herramientas (48 existentes + `conn_ping`); `call_tool("conn_ping")` llama a `GET /conn/ping/` con
  `timeout=15.0` y devuelve el JSON íntegro con acentos. Probado con `mcp[cli]` 2.x instalado en la sesión.
- `probar_conexiones.py`: `Resultado: 4/4 pruebas correctas` contra el servidor falso (401 sin token, ping, operación
  desconocida, `dev_exec`). Contra Revit: **PENDIENTE DE INSTALADOR** (paso 10).

### 2.4 Sondeos

Sintaxis comprobada con `python3 -m py_compile` (CPython 3; son IronPython 2.7 sin acentos, como los de la Fase 0).
Todo lo que hacen dentro de Revit es **PENDIENTE DE INSTALADOR** (pasos 9, 13, 14, 16). El sistema local que calculan
07 y 09 en Python reproduce la fórmula de `NodeFrame.cs`, que sí está probada.

## 3. Qué debo mirar cuando el instalador termine

1. Paso 3: `Build succeeded` y `Passed! Failed: 0` en tu PC (misma salida que en la nube).
2. Paso 7: la pestaña **Conexiones** y el diálogo del botón (captura `fase1-01-boton.png`). Si Revit avisó de
   "add-in sin firmar", es normal en esta fase.
3. Paso 8: `conn_ping` con `ok: true`, `addin_version: 0.1.0`, `dotnet.load_context` (espero `Default`) y
   `document.title: HANGAR_PRUEBA`. **Esto es la prueba de `conn_ping` de punta a punta** (HTTP → Routes → IronPython →
   `Bridge.Handle` → respuesta); el paso 10 la repite desde CPython y el 11, opcional, desde la herramienta MCP.
4. Paso 9, sondeo 06: es el que decide el camino A. Busca en su salida (a) algún tipo `*Transaction*` en
   `RevitAPISteel` o `Autodesk.SteelConnectionsDB` y su constructor; (b) la lista de DLL de `AddIns\SteelConnections\`;
   (c) los constructores de `Autodesk.AdvanceSteel.Modelling.Plate` y de `FinitRectScrewBoltPattern`.
5. Paso 13: el sondeo 07 debe elegir como cordón el HSS3X3 horizontal y dar `distancia entre ejes` ≤ 5 mm. Si da más,
   las barras del modelo no llegan al eje del cordón: me interesa saberlo ya (afecta al contrato de la Fase 3).
6. Paso 15 (captura `fase1-02-camino-b.png`): placa cuadrada 200 × 200 × 10 mm en el plano de la cercha, centrada en
   el nudo, con 4 cilindros de ⌀15,9 mm a 60 mm entre ejes atravesándola. Si la placa aparece "de canto" o fuera del
   plano, el sistema local está mal orientado y lo corrijo con las coordenadas que devuelve la operación.
7. Paso 16: cuál de los tres desenlaces ocurrió (parada sin crear, placa creada, Revit cerrado) y la captura
   `fase1-03-camino-a.png` si la hubo. Con la caja del elemento sabré si Advance Steel esperaba pies o mm.
8. Paso 17: el registro debe tener una línea `startup` y una `handle` por llamada con `ok`, códigos y duración.

## 4. Decisiones tomadas y por qué (plan cerrado con los resultados de la Fase 0)

| Resultado de la Fase 0 | Decisión en la Fase 1 |
|---|---|
| Reflexión con `Document` funciona; RevitAPI, pyRevit y los add-ins viven en el `AssemblyLoadContext` Default | **Puente por reflexión como camino principal** (`conexiones.py` → `Bridge.Handle`). El plan B (`HttpListener` 48885) no se implementa; queda en reserva. `conn_ping` devuelve `dotnet.load_context` para confirmarlo desde dentro del add-in. |
| `/execute_code/` abre siempre `TransactionGroup` + `Transaction` | Las pruebas que abren sus propias transacciones (SaveAs, API de acero) no pueden ir por ahí: se añade la ruta de desarrollo `/conn/dev_exec/` (sin transacción envolvente) y `revit-exec.ps1 -SinTransaccion`. Está protegida por el mismo token; no da más poder que `execute_code`. En la Fase 4 decidiré si se deja o se retira. |
| Revit se cerró de golpe al crear un `Plate` fuera del contexto de acero (8.1 de fase-0.md) | Reglas de seguridad aplicadas tal cual: el sondeo 09 **solo** crea si encuentra y abre un tipo `*Transaction*` de la API de acero y las firmas esperadas; todo sobre la copia `_sondeo.rvt`; el camino B se ejecuta **antes** que el A; el 10 solo si el 09 creó la placa. |
| `FabricationTransaction` no está entre los 20 tipos "de interés" del sondeo 01 (y el patrón incluía `Fabrication`) | No se da por hecho ningún nombre: el sondeo 06 vuelca **todos** los tipos de `RevitAPISteel.dll` y de `Autodesk.SteelConnectionsDB.dll` con sus constructores, y el 09 elige el tipo de transacción por búsqueda. |
| `AcceptableName("MotorConexiones.Connection") = False` | El esquema de Extensible Storage se llamará `MotorConexionesConnection` (Fase 3). |
| Dos familias HSS distintas con nombres `HSS2-1-2X2-1-2X3-16 64x64` | `MemberInfo` ya devuelve familia, tipo y medidas; `PROFILE_MISMATCH` comparará designación normalizada y medidas (Fase 2). |
| Cercha hecha de barras sueltas, sin elemento cercha | `NodeInspector` trabaja por IDs y, sin IDs, por la selección; elige el cordón como el miembro más horizontal (o `chord_element_id`). |
| Modelo en metros, ejes en pies | Toda conversión pasa por `UnitConverter`; `RevitGeometry.ToMm/ToFeet` es el único paso entre `XYZ` y `Vec3`. |

Otras decisiones:

- **Camino B = `DirectShape`** (no familias `.rfa`): compila entero en la nube, no necesita archivos de familia y
  no puede tumbar Revit. Categoría Conexiones estructurales, con retroceso a Modelos genéricos y advertencia
  `CATEGORY_FALLBACK` si Revit no la admite para DirectShape.
- **"La misma prueba desde C#" para el camino A no se escribió**: habría que inventar las firmas de Advance Steel y
  `RevitAPISteel` que la nube no puede compilar (CLAUDE.md lo prohíbe). La prueba A va en IronPython (sondeos 09 y 10),
  cuyo volcado por reflexión (06) dará las firmas reales para escribirla en C# en la Fase 3 si A gana.
- **Unidades de Advance Steel desconocidas**: el sondeo 09 pasa las coordenadas en pies (unidad interna de Revit) y
  mide después la caja del elemento creado; con eso se sabe si esperaba mm.
- **Botón de la cinta ya creado** pero solo informativo (único diálogo permitido); la lectura del JSON llega en la Fase 3.
- **Registro de operaciones con diccionario estático** en `Bridge` (`ping`, `probe_plate_b`, `probe_delete_b`); las
  operaciones reales (`validate`, `create`...) se añaden como clases nuevas sin tocar el puente.
- **Versión de `System.Text.Json` 10.0.12** en Core: la misma que trae el runtime del PC, para que Revit no cargue otra copia.

## 5. Pendientes, riesgos y preguntas

### 5.1 Decisión A o B: PENDIENTE DE INSTALADOR

Esta sesión no puede cerrar la decisión: no tiene Revit. Queda preparada para cerrarse con la salida de los pasos 9,
15 y 16 de `docs/instalacion/fase-1.md`, con esta regla:

- **A** si el sondeo 09 crea la placa con un tipo de transacción de la API de acero, sin diálogos y sin cerrar Revit,
  y el 06/10 muestran una API de pernos utilizable. DLL necesarias: `RevitAPISteel.dll` (raíz de Revit) y
  `ASObjectsMgd.dll` + `ASGeometryMgd.dll` (y las que diga el 06) de `AddIns\SteelConnections\`, referenciadas con
  `Private=false` desde un proyecto `MotorConexiones.Revit.Steel` que solo compila en el PC.
- **B** en cualquier otro caso. No necesita DLL ni familias: ya funciona con lo que compila la nube. Limitación
  documentada: la ranura del HSS no se representa (solo retiros de extremo y sólidos).
- Recomendación provisional: **B**, porque es lo único verificado y lo que la nube puede compilar; A solo si la
  evidencia del PC es limpia.

### 5.2 Riesgos

- **R1. Windows PowerShell 5.1** no probado (como en la Fase 0; `revit-exec.ps1` sí funcionó allí).
- **R2. Revit puede cerrarse en el sondeo 09 o 10** aunque se abra el contexto de acero: por eso van sobre la copia,
  después del camino B y con guardado previo. Si ocurre, el camino A queda descartado para v1.
- **R3. `DirectShape` en la categoría Conexiones estructurales** puede no estar permitido; hay retroceso automático.
- **R4. Add-in sin firmar**: Revit pregunta al cargar. En el PC se acepta "Always Load"; para distribuir habría que firmar.
- **R5. `Transform.OfPoint` con ejes del nudo**: si el sistema local saliera mal orientado en el modelo real, la captura
  del paso 15 lo mostrará; las pruebas del Core cubren los casos de cercha vertical y horizontal.
- **R6. Ruta `/conn/dev_exec/`**: ejecuta código arbitrario, igual que `execute_code`, con el mismo token. No abre
  ninguna superficie nueva, pero conviene decidir en la Fase 4 si se mantiene.

### 5.3 Preguntas para ti

1. Si el camino A funciona pero exige un proyecto que solo compila en tu PC, ¿lo prefieres igualmente a B
   (que compila entero en la nube)? Mi recomendación provisional es B salvo evidencia muy limpia de A.
2. ¿Quieres que la ruta de desarrollo `/conn/dev_exec/` se quede instalada después de la Fase 1?
3. Para la Fase 3: ¿la placa de prueba del paso 15 se ve donde esperabas (plano de la cercha, centrada en el nudo)?
   Si el plano de la cercha no coincide con el del Detalle D, dímelo con la captura.

## 6. Conclusiones con los resultados del PC (primera ronda, 2026-09-30)

El instalador ejecutó los 20 pasos; la salida está en `docs/fases/resultados-fase-1.md` (la sube él con el paso 19)
y la pegó en el chat. Revit **no se cerró de golpe** en ningún paso.

| Prueba | Resultado en el PC | Consecuencia |
|---|---|---|
| Compilación y pruebas en el PC (SDK 10.0.401) | `Compilación correcta, 0 errores`; 20/20 pruebas | Igual que en la nube. |
| `deploy.ps1` | Desplegó la 0.1.0 (el primer intento falló por la política de ejecución: el instalador no había aplicado el paso 1 en esa ventana) | Las instrucciones de la segunda ronda repiten `Set-ExecutionPolicy` en cada bloque. |
| Add-in en Revit | Ventana "Security - Unsigned Add-In" → *Always Load*; pestaña Conexiones y botón funcionando (`fase1-01-boton.png`) | Firmar la DLL queda como pendiente de distribución, no de v1. |
| **`conn_ping` de punta a punta** | `conn-call.ps1`: `ok:true`, `addin_version 0.1.0`, Revit 27.2.0.39, `dotnet.load_context: Default`; sondeo 05: `Bridge.Handle` por reflexión responde a `ping`, `no_existe` e JSON inválido; `probar_conexiones.py`: **4/4** | **Puente por reflexión confirmado y cerrado.** El plan B (`HttpListener`) se descarta. |
| `instalar-conn.ps1` | `startup.py` correcto; en `tools\__init__.py` **no encontró las anclas** (`document_tools`): la extensión del PC no es idéntica a ninguna rama de `revit-mcp` en GitHub (tiene una lista de nombres de herramientas) | Corregido: ahora ancla en la última línea `from .xxx import register_xxx` y la última llamada `register_xxx(` (probado con una variante con CRLF y llamada multilínea). Si vuelve a fallar, la segunda ronda devuelve el archivo entero. La herramienta MCP `conn_ping` sigue **PENDIENTE DE INSTALADOR**. |
| Sondeo 06 (API de acero) | `RevitAPISteel.dll`: 32 tipos, ninguno abre el contexto de acero. **`Autodesk.SteelConnectionsDB.FabricationTransaction(Document, Boolean isReadOnly, String, [Boolean bRevitTransactionAlreadyStarted])`** con `Commit()`, `CancelTransaction()`, `Dispose()` y `FabricationOnlyTransaction.IsWriteTransactionStarted(doc)`. `ASObjectsMgd.dll` ya cargado por Revit: `Plate(Plane, Point3d[], Double)`, `Plate(Plane, Point3d ptCenter, Double dLength, Double dWidth)`, `FinitRectScrewBoltPattern(Point3d, Point3d, Vector3d, Vector3d)` con `Wx/Wy/Length/Height`, `ScrewBoltPattern.ScrewDiameter/ScrewLength/Standard/Grade`, `BoltPattern.Connect(FilerObject[], eAssemblyLocation)`, `WeldPattern/WeldLine/WeldPoint`, `PlateFeatContour/PlateFeatVertFillet`, `BeamMultiContourNotch`; `FilerObject.WriteToDb()/DelFromDb()/Handle`. `ASGeometryMgd.dll` **no** estaba cargado (lo cargó el sondeo por ruta). Sin SDK de Revit en el PC. | La API del camino A **existe y está identificada** (tipos y firmas reales, ya no de memoria). |
| Sondeo 07 (nudo) | Cordón HSS3X3 horizontal, tres diagonales a 45°, distancia entre ejes 0,15–0,24 mm | El modelo encaja con el contrato (regla de 5 mm). |
| Sondeo 08 (copia) | El modelo es de **trabajo compartido** (`IsWorkshared=True`, y Revit avisa de que `HANGAR_PRUEBA.rvt` es una copia del central `HANGAR.rvt`); la copia se hizo a mano y **los ElementId cambiaron** (2390473 → 1249510) | `probe_plate_b` lo detectó con `ELEMENT_NOT_FOUND` y el instalador releyó los IDs con el sondeo 07. Regla para la Fase 3: los IDs de una especificación no sobreviven a un "Guardar como" de un modelo compartido; el `validation_token` ya lo cubre (`UniqueId`). |
| **Camino B** (`probe_plate_b`) | `ok:true`: placa 200×200×10 (id 1321304) y 4 pernos (1321305–1321308) en 130 ms, sin advertencias ni diálogos; `probe_delete_b` borró los 5. Captura `fase1-02-camino-b.png` (la sube el instalador). | **El camino B funciona de punta a punta.** |
| **Camino A** (sondeo 09) | `FabricationTransaction(doc, False, "Sondeo placa A")` se abrió (`IsModifiable=True`) **sin cerrar Revit**; el constructor de `Plate` falló con `ValueError: Object of type 'Autodesk.AdvanceSteel.Geometry.Plane' cannot be converted to type 'Autodesk.AdvanceSteel.Geometry.Plane'`; la transacción se cerró limpia (`IsModifiable=False`), 0 elementos nuevos. | No es un fallo de la API: `clr.AddReferenceToFileAndPath` cargó `ASGeometryMgd.dll` en **otro `AssemblyLoadContext`** y Revit resolvió su propia copia al construir la `Plate` (dos tipos `Plane` con el mismo nombre). Corregido en 09 y 10: los tipos de geometría se toman de los **parámetros del constructor** de `Plate`/`FinitRectScrewBoltPattern` (mismo contexto que `ASObjectsMgd`), y se usa `CancelTransaction()` (no existe `RollBack`). |

Correcciones hechas con estos resultados (ya en la rama):

- `NodeFrame`: la componente Z de la normal se considera nula por debajo de 1e-4 (antes 1e-9). En el modelo real el cordón
  trae un desvío de 2e-6 en Y y la normal salía `(0, -1, 0)` en vez de `(0, +1, 0)`. Prueba nueva con las coordenadas
  reales del nudo (21/21). El mismo umbral va en los sondeos 07 y 09.
- `instalar-conn.ps1`: anclas genéricas en `tools\__init__.py`.
- `deploy.ps1`: manifiesto con BOM (PowerShell 5.1 mostraba `AntÃ³n`; Revit lo leía bien) y comentario limpio.
- Sondeos 09 y 10: tipos de Advance Steel desde el constructor; `CancelTransaction`; en 10 se fijan `Wx`, `Wy` y `ScrewDiameter`.

**Decisión A/B: sigue abierta, pero el camino A pasó de "sin evidencia" a "API identificada y contexto de acero
abierto sin tumbar Revit".** Hace falta una segunda ronda corta (`docs/instalacion/fase-1b.md`: solo los sondeos 09 y 10
sobre la copia) para ver si la placa se crea y en qué unidades. Si se crea: **A** para placas, pernos y soldaduras
(DLL necesarias: `Autodesk.SteelConnectionsDB.dll`, `ASObjectsMgd.dll`, `ASGeometryMgd.dll` de `AddIns\SteelConnections\`,
referenciadas con `Private=false` desde un proyecto que solo compila en el PC), con B como reserva. Si no: **B**, ya probado.

## 7. Decisión cerrada con la segunda ronda (2026-09-30, `docs/fases/resultados-fase-1.md` secciones 1b-*)

| Prueba | Resultado en el PC |
|---|---|
| `instalar-conn.ps1` corregido | `tools\__init__.py: anadido register_conn_tools(...)` (líneas 194 y 200 de la versión del PC). Falta solo reiniciar el puente `main.py` para ver `conn_ping` como herramienta MCP. |
| Sondeo 07 con la tolerancia nueva | Normal del nudo `(0, +1, 0)`: la corrección de `NodeFrame` es la buena. |
| **Sondeo 09, camino A** | `FabricationTransaction` abierta, `Plate(Plane, Point3d[], 10 mm)` construida con los tipos del contexto `Default`, `WriteToDb() OK`, `Commit() OK`, **1 elemento nuevo: `SteelProxyElement` de la categoría Plates**, copia guardada. Sin ventanas, **Revit no se cerró**. Captura `fase1-03-camino-a.png`: placa cuadrada en el plano de la cercha, centrada en el nudo, de unas 2,5 veces el ancho del cordón HSS3X3 (76 mm): **las coordenadas en pies son las correctas**. |
| Sondeo 10, pernos | No llegó a intentarlo: `get_BoundingBox(None)` de un `SteelProxyElement` devuelve `null` y el sondeo lo usaba para situar el patrón. Volcado útil: `FinitRectScrewBoltPattern` tiene `Nx`, `Ny`, `Dx`, `Dy`, `ScrewDiameter`, `ScrewLength`, `NumberOfScrews`, `Connect(...)`. Reescrito para usar el sistema local del nudo (ronda 1c). |
| Tiempo | El sondeo 09 tardó **132 s** (la primera operación de acero en un documento inicializa el modelo de Advance Steel); el 10, 0,5 s hasta el fallo. |

### Decisión: **camino A** (fabricación de acero de Revit / Advance Steel) para placas, pernos y soldaduras

Evidencia: la placa se crea de forma programática dentro de `Autodesk.SteelConnectionsDB.FabricationTransaction`, sin
diálogos y sin tumbar Revit, y aparece como elemento nativo de Conexiones de acero (categoría Plates), que es lo que
pide el encargo ("prefiero A si funciona"). Lo que queda por confirmar en la ronda 1c (sondeo 10, cinco minutos) es
solo el patrón de pernos; usa el mismo mecanismo (`WriteToDb` dentro de la misma transacción) y las firmas reales
del volcado. Si el patrón fallara, los pernos irían por B (`DirectShape`) y las placas por A: la interfaz
`IFabricationBackend` admite mezclar.

**DLL que hacen falta** (todas en `C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\`, ninguna en NuGet):
`Autodesk.SteelConnectionsDB.dll` (transacción), `ASObjectsMgd.dll` (placas, pernos, soldaduras, cortes) y
`ASGeometryMgd.dll` (Point3d, Vector3d, Plane, Matrix3d). Se referencian con `Private=false` desde un proyecto nuevo
`MotorConexiones.Revit.Steel` que **solo compila en el PC** (`deploy.ps1` lo detecta por la existencia de la carpeta);
el add-in lo carga por reflexión detrás de `IFabricationBackend`, con `DirectShapeBackend` (B) como reserva si falta.
Reglas que salen de la prueba y van a la Fase 3:

1. Coordenadas a Advance Steel en **pies** (unidades internas de Revit), nunca en mm.
2. Los tipos de geometría se toman siempre del contexto en que Revit cargó `ASObjectsMgd` (nunca `LoadFile` por ruta).
3. `FabricationTransaction(doc, isReadOnly:false, nombre)` + `Commit()`; ante error `CancelTransaction()` y `Dispose()`.
   Hay una sobrecarga con `bRevitTransactionAlreadyStarted` para anidarla dentro de nuestro `TransactionGroup`: probar en la Fase 3.
4. La primera operación de acero de un documento puede tardar más de dos minutos: `conn_create` ya usa 180 s de espera;
   `conn_ping` o `conn_get_node_info` deberían calentar el modelo de acero (abrir y cerrar una transacción de solo lectura).
5. Los `SteelProxyElement` no tienen `BoundingBox`: para medir hay que usar `SteelElementProperties`/geometría de Advance Steel.
6. Los ElementId cambian al guardar como copia un modelo de trabajo compartido; el `validation_token` usa `UniqueId`.

Pendiente de la Fase 1 (no bloquea la Fase 2, que es solo Core): ronda 1c con el sondeo 10 reescrito
(`docs/instalacion/fase-1c.md`) y reinicio del puente `main.py` para ver `conn_ping` en el cliente de IA.
