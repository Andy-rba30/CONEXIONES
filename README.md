# MotorConexiones

Add-in de Autodesk Revit 2027 en C# y herramientas MCP (`conn_*`) para crear conexiones de acero (cartelas,
placas cuchilla, pernos, soldaduras y retiros de barras) a partir de una especificación JSON leída de un plano.
Lo maneja una IA a través del servidor MCP `revit-mcp` (repositorio aparte, Python + pyRevit).

Estado (2026-10-04): Fases 0 a 5 cerradas y probadas en el PC, incluida la prueba de punta a punta desde Antigravity
(`docs/fases/fase-5.md`, secciones 6 y 7, y `docs/fases/resultados-fase-5.md`). Fase 6 (ventana de previsualización 2D,
borrado desde la cinta y panel en la pestaña ARBA) probada en el PC en la ronda 6b con el add-in 0.2.0
(`docs/fases/fase-6.md`, sección 6); la corrección 6b (pernos a través del paquete cartela + placa cuchilla y edición de
cotas con doble clic, add-in 0.2.1) está **pendiente de la ronda `docs/instalacion/fase-6b.md`**.

---

## 1. Qué es MotorConexiones

Un sistema para modelar en Revit el nudo de una cercha tal como lo dibuja el plano de fabricación, sin inventar nada.
La IA lee el detalle, escribe una especificación JSON (`gusset_node`, contrato v1), la valida con el add-in y, con la
confirmación del usuario, la crea. También se puede usar sin IA desde la cinta de Revit: panel **Conexiones** de la
pestaña **ARBA** (la de los add-ins de la persona; si no se puede, pestaña **Conexiones** aparte), con los botones
**Ejecutar especificación JSON** (ventana con croquis 2D acotado, tabla editable y validación; sección 8) y **Conexiones
del modelo** (lista y borrado sin la IA; sección 9).

Lo que garantiza el add-in y dónde está probado:

| Garantía | Qué significa | Probado en |
|---|---|---|
| Validación obligatoria | `conn_create` y `conn_update` exigen el `validation_token` (SHA-256) que solo entrega `conn_validate` sin errores. Cubre la especificación, el documento y las barras del nudo. | `docs/fases/resultados-fase-3.md` y `resultados-fase-4.md` (`create` sin token → `VALIDATION_TOKEN_INVALID`; token determinista: el mismo en las Fases 3 y 4) |
| Backend nativo Advance Steel | Cartela y placa cuchilla como `SteelProxyElement` de categoría *Plates*, pernos como *Bolts*, **con las medidas del contrato** (Advance Steel trabaja en mm; el add-in convierte desde los pies de Revit). Las soldaduras van como `DirectShape` (reserva prevista en v1) y, si Advance Steel no está disponible, todo sale por `DirectShape`. | `resultados-fase-5.md`, ronda 5b, `5b-4` (cartela 565 × 530 × 9,53 mm, cuchilla 170 × 140 × 10 mm, pernos 5/8" a 60 mm) y capturas `fase5-03/04` |
| Atómico y sin ventanas | Cada operación es un `TransactionGroup`; ante un error, rollback completo. Los diálogos de Revit se cancelan y quedan como avisos en la respuesta. | `resultados-fase-3.md` y `resultados-fase-4.md` ("sin ventanas ni cierres de Revit"; avisos `REVIT_WARNING` en `create`) |
| Reversible | `conn_delete` borra solo lo que creó el add-in y devuelve a las barras sus extensiones originales (guardadas en Extensible Storage). | `resultados-fase-3.md` y `resultados-fase-5.md` (`B-3` y `5b-6`: `extension ... -> 0.0 mm`, `conexiones tras borrar: 0`, `Elementos de acero sueltos encontrados: 0`) |
| Cliente de IA | Las 13 herramientas `conn_*` se ven y funcionan desde Antigravity por el puente HTTP del puerto 8000. | `resultados-fase-4.md`, sección "4-9 cliente de IA" |
| Punta a punta desde la IA | Desde Antigravity: `conn_ping` → guía → tipos → `node_info` → esquema → `validate` → `preview` → confirmación literal → `create` (9 elementos, 2,2 s) → `list` → `get` → confirmación literal → `delete` → `list` = 0, sin ventanas ni cierres de Revit. | `resultados-fase-5.md`, sección "B-1 Antigravity (punta a punta)" |
| Token ligado a `limits.json` | El `validation_token` incluye el hash del `limits.json` desplegado: con un `limits.json` distinto o un token alterado, `create` responde `VALIDATION_TOKEN_INVALID` y el modelo no cambia. | `resultados-fase-5.md`, `A-7` (sondeo 14, 14/14) |

Lo que **no** hace: no diseña ni verifica resistencias; no lee planos PDF completos; v1 solo conoce `gusset_node`.

---

## 2. Estructura del repositorio

```text
CONEXIONES/
├── CLAUDE.md                        Reglas permanentes del repositorio
├── MotorConexiones.sln
├── src/
│   ├── MotorConexiones.Core/        netstandard2.0, sin referencias a Revit
│   │   ├── AddinInfo.cs             Versión del add-in (0.2.0) y spec_version (1.0)
│   │   ├── Contract/                ConnectionSpec, ChordSpec, GussetSpec, GussetOutline, MemberSpec, AttachmentSpec,
│   │   │                            KnifePlateSpec, BoltPatternSpec, WeldSpec, DimensionChain, UncertainField, SourceInfo,
│   │   │                            NodeRef, ApiResponse/ApiError/ApiMeta (sobre común), JsonOptions
│   │   ├── Editing/                 SpecFieldCatalog (filas de la tabla editable), SpecFieldEditor (aplica un texto a una
│   │   │                            ruta JSON), SpecValueParser, SpecField (Fase 6)
│   │   ├── Geometry2D/              Point2D, Segment2D, Polygon2D, Geometry2DChecks (contorno, cruces, pernos en placa)
│   │   ├── Geometry3D/              Vec3, NodeFrame (sistema local del nudo), NodeReach, ConnectionGeometry, BoltGrid,
│   │   │                            BoltPosition, WeldLine2D
│   │   ├── Model/IModelFacts.cs     Lo que el validador necesita del modelo, sin depender de Revit
│   │   ├── Schema/JsonSchemaValidator.cs   JSON Schema de gusset_node y su comprobación
│   │   ├── Sketch/                  Croquis 2D en mm (Fase 6): SketchPrimitives, SketchNodeInput (direcciones y anchos
│   │   │                            desde IModelFacts o esquemático), GussetNodeSketch, SketchBuilder, ISketchProvider
│   │   ├── Storage/                 ConnectionRecord, ModifiedMemberRecord (lo que se guarda por conexión)
│   │   ├── Types/                   IConnectionType, ConnectionTypeRegistry, GussetNodeType
│   │   ├── Units/UnitConverter.cs   ÚNICO sitio donde se convierten mm y grados a pies y radianes
│   │   └── Validation/              SpecValidator (las 10 reglas), ValidationTokenGenerator, LimitsConfig,
│   │                                LabelParser, LabelFormatter (etiquetas desde mm), ProfileMatcher, ErrorCodes
│   ├── MotorConexiones.Revit/       net10.0-windows, add-in de Revit 2027
│   │   ├── App.cs                   IExternalApplication: panel "Conexiones" en la pestaña "ARBA" (reserva: pestaña
│   │   │                            "Conexiones") con los botones "Ejecutar especificación JSON" y "Conexiones del modelo"
│   │   ├── RunSpecCommand.cs        Botón 1: archivo JSON → ventana de previsualización → crear (Fase 6)
│   │   ├── ModelConnectionsCommand.cs   Botón 2: lista y borra conexiones del modelo (Fase 6)
│   │   ├── UI/                      Ventanas WPF, las únicas del add-in: PreviewWindow (croquis + tabla + validación),
│   │   │                            SketchView (dibuja el SketchModel), PreviewSession, ConnectionsWindow, RibbonIcons
│   │   ├── Bridge.cs                Bridge.Handle(operation, requestJson, doc, uidoc): punto de entrada del MCP
│   │   ├── LimitsConfigLoader.cs    Lee config\limits.json de la carpeta del add-in desplegado
│   │   ├── Fabrication/             IFabricationBackend, AdvanceSteelBackend, DirectShapeBackend, BackendFactory,
│   │   │                            MemberModifier (retiros de extremo)
│   │   ├── Node/                    NodeInspector, RevitGeometry, RevitModelFacts
│   │   ├── Operations/              IOperation + una clase por operación: Ping, Guide, Types, Schema, NodeInfo,
│   │   │                            FindProfile, Validate, Preview, Create, List, Get, Update, Delete (13)
│   │   ├── Services/ConnectionCreationService.cs   Crea la conexión completa y adopta los elementos de acero
│   │   ├── Storage/ConnectionStorageManager.cs     Extensible Storage: esquema MotorConexionesConnection (GUID fijo, v1)
│   │   ├── Transactions/OperationScope.cs          TransactionGroup + IFailuresPreprocessor + DialogBoxShowing
│   │   └── Logging/JsonLineLogger.cs               Una línea JSON por llamada en %LOCALAPPDATA%\MotorConexiones\log\
│   └── MotorConexiones.Tests/       xUnit (123 pruebas), solo Core, con el fixture del Detalle D
├── config/limits.json               Tolerancias y mínimos AISC 360 (J3.3, J3.4, J2.4), editable sin recompilar
├── docs/
│   ├── ENCARGO_MOTOR_CONEXIONES.md  El encargo completo, por fases
│   ├── guide.md                     Guía para la IA (la devuelve conn_get_guide), editable sin recompilar
│   ├── fixtures/                    detalle-D.json (con dudas), detalle-D-confirmado.json (dudas resueltas),
│   │                                detalle-D.png, cercha-vista-general.png
│   ├── fases/                       fase-N.md (informe de cada fase), resultados-fase-N.md (salidas del PC), capturas/
│   ├── prompts/                     prompt y alcance de cada fase posterior al encargo (fase-6.md)
│   └── instalacion/                 Instrucciones literales para el agente instalador, una por fase
├── mcp/                             Archivos nuevos para la extensión revit-mcp (no se toca lo existente)
│   ├── revit_mcp/conexiones.py      Adaptador IronPython 2.7: 15 rutas /conn/... -> Bridge.Handle
│   ├── tools/conn_tools.py          13 herramientas @mcp.tool() conn_* (CPython, SDK mcp 2.x)
│   ├── CONTRATO-conn.md             Contrato de las rutas /conn/ (para pegar al final de CONTRATO.md de revit-mcp)
│   ├── instalar-conn.ps1            Copia los dos archivos a la extensión y añade las líneas de registro (idempotente)
│   └── pruebas/                     probar_conexiones.py (17 pruebas + 2 con --puente) y simulador_revit.py (solo nube)
└── scripts/
    ├── deploy.ps1                   Compila en Release y copia DLL, .addin, config\limits.json y docs\guide.md a Addins\2027
    ├── revit-exec.ps1               Ejecuta un sondeo IronPython dentro de Revit (por /execute_code/ o -SinTransaccion)
    ├── conn-call.ps1                Llama a una operación del add-in por HTTP (ping o /conn/op/<operación>/)
    └── sondeos/                     00 a 15 y capturar-nudo.py (scripts de sondeo para el instalador)
```

---

## 3. Requisitos previos (PC con Revit)

1. **Autodesk Revit 2027.2** (runtime .NET 10). En el PC de prueba `conn_ping` responde `backend: advancesteel`: los módulos
   de Advance Steel que instala Revit están disponibles. Si no lo estuvieran, el add-in usa `DirectShape`.
2. **SDK de .NET 10** (`dotnet --list-sdks` debe mostrar `10.x`): el instalador compila el add-in; los binarios no se suben.
3. **pyRevit** con soporte para 2027 y el servicio **Routes** activo (puerto 48884).
4. **Extensión revit-mcp** desplegada en `C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension`, con su `.venv`
   (Python 3.13 en el PC, `mcp` 2.2 y `httpx`) y `uv` en `%USERPROFILE%\.local\bin\uv.exe`.
5. **Git**, para traer la rama al PC.

---

## 4. Instalación paso a paso

Todos los comandos van en PowerShell desde la raíz del repositorio (`D:\Proyectos C#\CONEXIONES`; la ruta lleva
espacio y `#`, siempre entre comillas).

### Paso 1: compilar y pasar las pruebas

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
dotnet build MotorConexiones.sln -c Release
dotnet test MotorConexiones.sln -c Release --no-build
```

Se espera `0 Errores` y `Superado: 114`.

### Paso 2: desplegar el add-in (con Revit cerrado)

`deploy.ps1` compila en Release (salvo con `-NoBuild`) y copia `MotorConexiones.Core.dll`, `MotorConexiones.Revit.dll`,
`config\limits.json` y `docs\guide.md` a `%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\`, más el manifiesto
`MotorConexiones.addin`. Revit bloquea la DLL mientras está abierto, así que ciérralo antes.

```powershell
.\scripts\deploy.ps1
```

Se espera `== MotorConexiones 0.2.0.0 desplegado en Revit 2027 ==`.

### Paso 3: instalar las rutas y herramientas del MCP en la extensión

```powershell
.\mcp\instalar-conn.ps1
```

Copia `mcp\revit_mcp\conexiones.py` y `mcp\tools\conn_tools.py` a la extensión y añade, si faltan, las dos líneas de
registro en `startup.py` y las dos en `tools\__init__.py`. Es idempotente: se puede repetir. Se espera
`(15 rutas @api.route)` y `(13 herramientas @mcp.tool)`.

### Paso 4: abrir Revit y comprobar el add-in

1. Abre **Revit 2027** con tu modelo (para las pruebas, la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
   nunca el original). Si Revit pregunta por el add-in sin firmar, pulsa *Always Load*.
2. En la pestaña **ARBA** debe aparecer el panel **Conexiones** con los botones **Ejecutar especificación JSON** y
   **Conexiones del modelo**. Si ARBA no existe o falla, el panel sale en una pestaña **Conexiones** y el log del add-in
   tiene una línea `ribbon_arba_failed` (sondeo `scripts\sondeos\15-cinta-arba.py` lista pestañas y paneles).
3. Espera a que pyRevit cargue (unos 20 s; pyRevit solo lee `conexiones.py` al arrancar Revit) y comprueba:

   ```powershell
   .\scripts\conn-call.ps1 -Operation ping
   ```

   Se espera `ok: true`, `addin_version: 0.2.0`, `backend: advancesteel` y, en `operations`, las 13 operaciones.

### Paso 5: arrancar el puente MCP (puerto 8000)

El puente `main.py` de la extensión traduce las llamadas del cliente de IA a HTTP contra Revit. Se arranca a mano con
`C:\IA\iniciar_servidor_revit.bat`, que ejecuta en la carpeta de la extensión:

```powershell
cd "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
& "$env:USERPROFILE\.local\bin\uv.exe" run main.py --streamable-http
```

Deja esa ventana abierta: escucha en `http://127.0.0.1:8000/mcp`. Cada vez que reinstales `conn_tools.py` hay que
reiniciar el puente.

### Paso 6: conectar el cliente de IA

- **Antigravity (configuración probada, `resultados-fase-4.md` "4-9")**. Archivo
  `%USERPROFILE%\.gemini\config\mcp_config.json`:

  ```json
  {
    "mcpServers": {
      "revit": {
        "serverUrl": "http://localhost:8000/mcp"
      }
    }
  }
  ```

  Orden que funciona: abrir Revit con el modelo → arrancar el puente con `C:\IA\iniciar_servidor_revit.bat` → en
  Antigravity, recargar (o reiniciar) el servidor `revit` para que vuelva a pedir la lista de herramientas → deben
  aparecer las 13 `conn_*` (79 herramientas en total en el PC).

- **Claude Desktop (NO PROBADA en este proyecto)**. Claude Desktop lanza `main.py` como subproceso por stdio, no por
  HTTP. La configuración siguiente está escrita a partir de la documentación general de MCP y no se ha ejecutado:
  archivo `%APPDATA%\Claude\claude_desktop_config.json`:

  ```json
  {
    "mcpServers": {
      "revit": {
        "command": "C:\\Users\\<Usuario>\\.local\\bin\\uv.exe",
        "args": ["run", "--directory", "C:\\IA\\pyrevit-ext\\mcp-server-for-revit-python.extension", "main.py"]
      }
    }
  }
  ```

  Si la usas, anota en `docs/fases/resultados-fase-5.md` si funcionó.

---

## 5. Cómo actualizar el add-in

### Si cambió código C# (`src/`)

1. Cierra Revit (tiene la DLL bloqueada).
2. Compila, pasa las pruebas y despliega:

   ```powershell
   dotnet build MotorConexiones.sln -c Release
   dotnet test MotorConexiones.sln -c Release --no-build
   .\scripts\deploy.ps1 -NoBuild
   ```

3. Vuelve a abrir Revit.

### Si cambiaron los archivos del MCP (`mcp/`)

1. Cierra Revit y ejecuta `.\mcp\instalar-conn.ps1` (pyRevit solo recarga `conexiones.py` al arrancar Revit).
2. Vuelve a abrir Revit.
3. Cierra la ventana del puente y vuelve a arrancarlo (`C:\IA\iniciar_servidor_revit.bat`), y recarga el servidor
   `revit` en el cliente de IA para que vea las herramientas nuevas.

### Si solo cambió `config/limits.json` o `docs/guide.md`

Basta `.\scripts\deploy.ps1 -NoBuild` (copia los dos archivos). Para `guide.md` ni siquiera hace falta reiniciar Revit;
para `limits.json`, ver la sección 6.

---

## 6. Cómo editar `limits.json` y `guide.md` sin recompilar

### `config/limits.json` (tolerancias y mínimos AISC)

El add-in lo lee en cada `validate`, `create` y `update` desde la carpeta desplegada
(`%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json`). Edita el del repositorio y vuelve a
desplegar con `.\scripts\deploy.ps1 -NoBuild`.

```json
{
  "schema_version": 1,
  "dimension_chain_tolerance_mm": 1.0,
  "label_value_tolerance_mm": 0.05,
  "angle_tolerance_deg": 1.0,
  "node_axis_max_distance_mm": 5.0,
  "bolts": {
    "min_spacing_factor": 2.667,
    "edge_distance_mm": { "12.7": 19.0, "15.875": 22.0, "19.05": 25.0, "22.225": 28.0, "25.4": 32.0, "28.575": 38.0, "31.75": 42.0 }
  },
  "welds": {
    "min_fillet_mm": { "6.0": 3.0, "13.0": 5.0, "19.0": 6.0, "default": 8.0 }
  }
}
```

- `dimension_chain_tolerance_mm`: cuánto puede desviarse la suma de una cadena de cotas de su total.
- `label_value_tolerance_mm`: diferencia admitida entre un rótulo (`3/8"`) y su valor en mm (9.525).
- `bolts.edge_distance_mm`: distancia mínima al borde por diámetro (AISC 360, tabla J3.4); `min_spacing_factor` por `d`
  (J3.3).
- `welds.min_fillet_mm`: filete mínimo según el espesor más delgado (J2.4); `default` para espesores mayores.
- El hash SHA-256 de estos valores entra en el `validation_token`: si editas el archivo entre `conn_validate` y
  `conn_create`, el token deja de valer (`VALIDATION_TOKEN_INVALID`) y hay que volver a validar. La clave `_comentario`
  no cuenta.

### `docs/guide.md` (guía para la IA)

Es lo que devuelve `conn_get_guide`. El add-in lo lee del disco en cada llamada, así que cualquier cambio desplegado
surte efecto de inmediato, sin reiniciar Revit. Contiene el flujo obligatorio de 10 pasos, cómo leer un detalle de
acero, el sistema local, qué hacer con cada error y las reglas de seguridad.

---

## 7. Cómo agregar un tipo de conexión nuevo

Un tipo de conexión es una clase en `src/MotorConexiones.Core/Types/` que implementa la interfaz real
`IConnectionType` (`src/MotorConexiones.Core/Types/IConnectionType.cs`):

```csharp
public interface IConnectionType
{
    string Name { get; }          // nombre estable que va en connection_type, p. ej. "base_plate"
    string Description { get; }   // descripción corta en español (la ve la IA en conn_list_types)
    string GetSchemaJson();       // JSON Schema del tipo
    string GetExampleJson();      // ejemplo JSON completo y válido
}
```

`GussetNodeType.cs` es el modelo a seguir: un `Instance` estático, las cuatro propiedades y un método `Validate` propio
que llama a `SpecValidator`. Pasos para añadir, por ejemplo, `base_plate`:

1. **Core: el tipo.** `src/MotorConexiones.Core/Types/BasePlateType.cs`:

   ```csharp
   public sealed class BasePlateType : IConnectionType
   {
       public static BasePlateType Instance { get; } = new BasePlateType();
       public string Name => "base_plate";
       public string Description => "Placa base con pernos de anclaje para columna HSS o W.";
       public string GetSchemaJson() => JsonSchemaValidator.GetBasePlateSchemaJson();
       public string GetExampleJson() => "{ ... }";
   }
   ```

2. **Core: contrato, esquema y validación.** Sus clases de contrato en `Contract/`, el esquema en
   `Schema/JsonSchemaValidator.cs`, las reglas propias en `Validation/` y las pruebas xUnit en
   `src/MotorConexiones.Tests/` con un fixture en `docs/fixtures/`.

3. **Registro.** En el constructor estático de `src/MotorConexiones.Revit/Bridge.cs`, junto al tipo existente:

   ```csharp
   if (ConnectionTypeRegistry.Find("gusset_node") == null)
   {
       ConnectionTypeRegistry.Register(GussetNodeType.Instance);
   }
   ConnectionTypeRegistry.Register(BasePlateType.Instance);
   ```

   Con solo esto, `conn_list_types` y `conn_get_schema` ya lo devuelven (leen el registro).

4. **Revit: operaciones y modelado.** En v1 `ValidateOperation`, `PreviewOperation`, `CreateOperation` y
   `UpdateOperation` llaman directamente a `SpecValidator` y a `ConnectionCreationService`, que son de `gusset_node`.
   Para un segundo tipo hay que despachar por `connection_type` dentro de esas operaciones hacia el validador y el
   servicio de creación del tipo nuevo, e implementar su geometría en `Fabrication/` (Advance Steel y la reserva
   `DirectShape`). El almacenamiento (`ConnectionStorageManager`), el `OperationScope` y el registro se reutilizan tal cual.

5. **Guía.** Describe el tipo nuevo en `docs/guide.md` para que la IA sepa cuándo usarlo.

Si el tipo nuevo quiere verse en la ventana de previsualización (sección 8), además implementa
`MotorConexiones.Core.Sketch.ISketchProvider` (`BuildSketch(spec, node)` devuelve un `SketchModel` con líneas, polígonos,
círculos, cotas y etiquetas en mm, en el sistema local del nudo); `SketchBuilder` lo busca en el registro. Si no lo
implementa, la ventana muestra "El tipo de conexión '…' no aporta croquis" y la tabla sigue funcionando.

---

## 8. Previsualizar y corregir antes de crear (desde la cinta, sin la IA)

El botón **ARBA > Conexiones > Ejecutar especificación JSON** abre un selector de archivo y después la ventana
**Previsualización de conexión** (Fase 6). Estado: programada y compilada en la nube, **PENDIENTE DE INSTALADOR**
(`docs/instalacion/fase-6.md`).

- **Izquierda, croquis 2D** del nudo en el plano de la cercha, en el sistema local del nudo: origen en el punto de
  trabajo (`PT`), X a lo largo del cordón, Y perpendicular en el plano. Dibuja el cordón y cada barra con su ancho real
  (leído del modelo) y su retiro, la cartela, la placa cuchilla, los pernos, las ranuras (ocultas) y las marcas de
  soldadura. Cotas en mm con una cifra decimal: ancho y alto de la cartela, espesor como etiqueta (`PL 3/8" (9,5 mm)`),
  retiro de cada barra, largo de la ranura, placa cuchilla (largo, ancho, inserción) y pernos (primera fila, paso, borde).
  Rueda del ratón: zoom; botón central: encuadre; **Ajustar**: encuadra todo. Si alguna barra no está en el modelo, el
  croquis sale esquemático con los ángulos del plano y lo avisa en naranja; también avisa si en ese nudo el eje Y local
  apunta hacia abajo (el croquis se ve girado 180° respecto a la vista real).
  **Doble clic sobre una cota o un rótulo** (desde la 0.2.1): al pasar el ratón la cota se resalta y el cursor es una
  mano; el doble clic selecciona su fila en la tabla y abre un editor junto al cursor con el campo, su ruta JSON y el
  valor. Intro aplica (misma validación y mismo registro que la tabla), Esc cancela. Bajo el croquis, una línea dice el
  agarre y la longitud de los pernos de cada placa cuchilla (ver abajo).
- **Derecha, tabla editable** con los mismos datos del resumen que da la IA antes de crear: origen, cordón, cartela
  (espesor, etiqueta, ancho, alto, unión al cordón, soldadura), contorno de la cartela (un vértice por fila, `x; y`),
  cada barra (rol, perfil, ángulo del plano y del modelo, retiro, tipo de unión y sus medidas: ranura y soldadura, o
  placa cuchilla, pernos y soldadura), cadenas de cotas y dudas (`user_confirmed_value`). Escribe el valor y pulsa Intro:
  sirve coma o punto decimal, listas con punto y coma (`75; 420; 70`), `sí`/`no`. Cada cambio redibuja el croquis y
  vuelve a validar. Si cambias `thickness_mm` o `diameter_mm` y la etiqueta deja de coincidir, la etiqueta se actualiza
  sola (`12,7` → `1/2"`, `10` → `PL10`, `20` → `20 mm`) y la línea de estado lo dice. Las filas grises son datos leídos
  del modelo y no se editan.
- **Abajo, validación**: los mismos errores y avisos que ve la IA (código, campo, mensaje, sugerencia), el token
  abreviado y los botones **Recargar** (vuelve a leer el archivo del disco, para correcciones hechas desde el chat),
  **Guardar JSON** (escribe la copia `<nombre>-corregido.json` junto al original, nunca encima), **Validar**, **Crear**
  (solo con 0 errores y token) y **Cancelar**. **Crear** cierra la ventana y modela la conexión igual que la IA
  (`ConnectionCreationService`, una operación atómica, registro en el modelo) y termina con un diálogo que muestra el
  `connection_id`.

Las rutas `conn_*` que usa la IA no cambian ni muestran ventanas: la ventana vive solo en el camino del botón.

**Pernos de la placa cuchilla (desde la 0.2.1).** La placa cuchilla se apoya sobre la cara +Z de la cartela (solape) y los
pernos atraviesan las dos placas: cabeza sobre la placa cuchilla, tuerca por la otra cara de la cartela. La longitud del
perno no es fija: agarre (cartela + placa cuchilla) más el suplemento de la tabla 7-15 del AISC Manual por diámetro
(`bolts.length_addition_mm` en `config/limits.json`), redondeado hacia arriba a 1/4" (`length_increment_mm`). Para el
Detalle D (3/8" + PL10, pernos 5/8"): 19,5 + 22,2 → **44,45 mm (1-3/4")**. `conn_preview` lo devuelve en `bolt_stacks`.
Estado: **PENDIENTE DE INSTALADOR** (`docs/instalacion/fase-6b.md`, paso 6b-5).

---

## 9. Borrar desde la cinta

El botón **ARBA > Conexiones > Conexiones del modelo** lista las conexiones creadas por el add-in en el documento activo
(id, tipo, plano, fecha, elementos creados, barras y backend; lo mismo que `conn_list`). Selecciona una y pulsa **Borrar
seleccionada**: tras confirmar, borra solo los elementos que creó el add-in y devuelve a las barras sus extensiones
originales (lo mismo que `conn_delete`, con el mismo `TransactionGroup`: si algo falla, no cambia nada). Nunca borra
elementos ajenos. Estado: **PENDIENTE DE INSTALADOR** (`docs/instalacion/fase-6.md`, paso 6-8).

---

## 10. Guion de prueba de punta a punta con la IA

El guion completo, con el prompt literal para pegar en Antigravity, está en `docs/instalacion/fase-5.md`, parte B.
Resumen del ciclo sobre la copia `HANGAR_PRUEBA_sondeo.rvt` y el fixture `docs/fixtures/detalle-D-confirmado.json`:

1. `conn_ping`: `ok: true`, `backend: advancesteel`, `document.title: HANGAR_PRUEBA_sondeo`.
2. `conn_get_guide` y `conn_list_types`.
3. `conn_get_node_info` con `[1249510, 1249630, 1249631, 1249636]` (cordón `1249510`).
4. `conn_get_schema` de `gusset_node`.
5. `conn_validate` con el fixture: `is_valid: true`, dos avisos `ANGLE_DIFFERS_FROM_MODEL`, token de 64 hex.
6. `conn_preview`: 1 cartela, 1 placa cuchilla, 4 pernos, 6 soldaduras, 3 barras retiradas.
7. **Confirmación explícita del usuario** y `conn_create` con la especificación y el token: `connection_id` y 9 IDs.
8. `conn_list` (1 conexión) y `conn_get` (especificación guardada). Mirar el nudo en Revit: placas y pernos de Advance
   Steel a la vista.
9. **Confirmación explícita** y `conn_delete`: `conn_list` vuelve a 0 y las barras recuperan sus extensiones.

Reglas para la IA durante la prueba: nunca `conn_create` ni `conn_delete` sin confirmación; nunca `execute_revit_code`
sobre lo que creó el add-in; si `conn_create` no responde, no repetirlo: `conn_list` dice si quedó creada.

---

## 11. Cómo probar sin Revit

Lo que se puede ejecutar en cualquier máquina (Linux, macOS o Windows) sin Revit ni pyRevit:

```bash
dotnet build MotorConexiones.sln -c Release          # Core, Revit y Tests (0 avisos)
dotnet test MotorConexiones.sln -c Release --no-build  # 123 pruebas del Core
python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py
```

**`mcp/pruebas/simulador_revit.py`** (solo para desarrollo; nada lo copia a la extensión) carga el `conexiones.py` real
con módulos `pyrevit`, `clr` y `System` simulados, sustituye `Bridge.Handle` por una imitación en Python del add-in
(mismas operaciones y códigos de error, datos del nudo del Detalle D) y sirve las rutas `/revit_mcp/conn/...` en el
puerto 48884 con token, igual que pyRevit Routes. Con `--extension <clon de revit-mcp>` usa el `seguridad.py` real.

```bash
# 1) Autocomprobación en proceso: 15 rutas, token, dev_exec, acentos (27 comprobaciones)
python3 mcp/pruebas/simulador_revit.py --autocomprobar

# 2) Servir el simulador y pasarle el script de pruebas (17 pruebas)
python3 mcp/pruebas/simulador_revit.py &        # escribe el token en el archivo literal "%LOCALAPPDATA%\RevitMcp\token"
python3 mcp/pruebas/probar_conexiones.py        #   de la carpeta actual, que es el que abre el script en Linux

# 3) Con el puente real (19 pruebas): clon de revit-mcp con conn_tools.py copiado y registrado en tools/__init__.py,
#    un venv con "mcp[cli]>=2.2,<3" y httpx, y "python main.py --streamable-http" en la carpeta del clon
python3 mcp/pruebas/probar_conexiones.py --puente
```

Lo que prueba de verdad: el adaptador, la forma de las peticiones, el script de pruebas, las 13 herramientas y el puente.
Lo que **no** prueba: nada de lo que pasa dentro de Revit (geometría, Advance Steel, almacenamiento). Eso solo lo prueba
el instalador en el PC con `docs/instalacion/fase-N.md`.

---

## 12. Errores más comunes

| Código | Significado | Qué hacer |
|---|---|---|
| `VALIDATION_TOKEN_INVALID` | Falta el token o no coincide: la especificación, el documento, las barras del nudo o `limits.json` cambiaron desde `conn_validate`. No caduca por tiempo. | Volver a llamar a `conn_validate` con la misma especificación y usar el token nuevo. |
| `UNRESOLVED_UNCERTAINTY` | Hay entradas en `uncertain_fields` sin `user_confirmed_value`. | Preguntar al usuario y poner el valor confirmado. |
| `SCHEMA_INVALID` | El JSON no cumple el esquema, o un campo obligatorio (espesor de la cartela, medidas de la placa cuchilla o de los pernos, largo de la ranura, retiro) está vacío sin estar declarado en `uncertain_fields`. | Rellenar el campo que indica `path` (en la ventana, la fila correspondiente) o declararlo como duda. |
| `DIMENSION_CHAIN_MISMATCH` | Una cadena de cotas no suma su total (tolerancia de `limits.json`). | Releer el plano o llevar la cota a `uncertain_fields`. |
| `LABEL_VALUE_MISMATCH` | Un rótulo (`3/8"`, `PL10`) no coincide con su valor en mm. | Corregir el número o el rótulo. |
| `PROFILE_MISMATCH` | El perfil escrito no coincide con el tipo del elemento; la respuesta sugiere los más parecidos. | Usar `conn_find_profile` y corregir. |
| `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL`, `BOLT_OUTSIDE_PLATE` | Pernos fuera de los mínimos AISC o fuera de la placa. | Revisar la cota en el plano o los mínimos de `limits.json`. |
| `NODE_AXES_NOT_INTERSECTING` | Los ejes del cordón y del primer miembro distan más de 5 mm. | Avisar al usuario: hay que ajustar el modelo. |
| `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE` | Un ID no existe, no es armazón estructural o no llega al nudo. | Volver a `conn_get_node_info` con la selección correcta. |
| `REVIT_BUSY` | Hay una orden abierta en Revit o el documento es de solo lectura. | Cerrar la orden en Revit y repetir. |
| `UNKNOWN_OPERATION` | Operación o tipo de conexión no registrado (también lo da `conn_get_schema` con un tipo inexistente). | Ver `operations` en `conn_ping` o `conn_list_types`. |
| `ADDIN_NOT_LOADED`, `NO_DOCUMENT` | El add-in no está en Revit, o no hay documento abierto. | `scripts\deploy.ps1` con Revit cerrado; abrir un modelo. |
| `REVIT_UNREACHABLE`, `CONN_ROUTE_NOT_FOUND`, `REVIT_RESTARTED`, `REVIT_TIMEOUT` | Los genera el puente: Revit cerrado, `conexiones.py` sin instalar, token cambiado o tiempo agotado. | Abrir Revit; `mcp\instalar-conn.ps1`; reintentar; comprobar con `conn_list`. |

La tabla completa, con `path`, `message` y `hint`, está en `mcp/CONTRATO-conn.md` y en `docs/guide.md`.

---

## 13. Licencia

No se ha definido una licencia. Uso interno del autor del repositorio.
