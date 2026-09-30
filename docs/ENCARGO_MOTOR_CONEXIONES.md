# Encargo definitivo: MotorConexiones (add-in de Revit en C#) + herramientas `conn_*` para revit-mcp

> Este documento es el prompt que Claude Code ejecuta por fases. La sección 0 es para el humano
> (cómo lanzarlo). Desde la sección 1 en adelante habla Claude Code.

---

## 0. Cómo usar este encargo (para el humano)

**Dónde vive cada cosa**

| Pieza | Repositorio | Carpeta local (ejemplo) |
|---|---|---|
| Add-in MotorConexiones (C#) | `Andy-rba30/CONEXIONES` | `C:\dev\CONEXIONES` |
| Servidor MCP existente | `Andy-rba30/revit-mcp` | `C:\dev\revit-mcp` |
| Este encargo | `CONEXIONES/docs/ENCARGO_MOTOR_CONEXIONES.md` | — |
| Reglas permanentes | `CONEXIONES/CLAUDE.md` | — |

**Requisitos de la máquina donde corre Claude Code** (tiene que ser la misma máquina donde está Revit):

1. Windows 10/11 con **Revit 2027.2** instalado (ya lo tienes) y un modelo de prueba `.rvt` con una cercha de HSS.
2. pyRevit instalado en una versión con soporte para Revit 2027, con el servidor Routes activado y la extensión `revit-mcp` cargada (ya lo tienes).
3. `uv` instalado y el MCP funcionando (`uv run main.py` en la carpeta de revit-mcp).
4. **SDK de .NET 10** instalado (`dotnet --list-sdks` muestra una versión 10.x). Revit 2027 corre sobre .NET 10; el add-in se compila para `net10.0-windows`.
5. Claude Code instalado (`claude --version` responde).
6. Ambos repositorios clonados en la misma carpeta padre.

**Conectar el MCP a Claude Code** (una sola vez, desde `C:\dev\CONEXIONES`):

```powershell
claude mcp add revit -- uv run --directory C:\dev\revit-mcp main.py
```

**Lanzar una fase** (una sesión de Claude Code por fase, siempre desde `C:\dev\CONEXIONES`):

```powershell
claude --add-dir C:\dev\revit-mcp
```

Y en la sesión pegas el prompt de arranque de esa fase (Anexo A). Cuando la fase termina, revisas el
informe `docs/fases/fase-N.md`, pruebas lo que dice, y si estás conforme lanzas la fase siguiente.
Ese es tu "visto bueno": no hace falta escribir nada más.

**Completa antes de empezar** (sección 2): las rutas reales de las carpetas y del modelo de prueba.

---

## 1. Quién soy y cómo quiero que trabajes

- Soy principiante programando. Explícame en español sencillo, con pasos exactos: qué archivo, qué botón, qué comando.
- Trabajas por fases (sección 12). Cada sesión de Claude Code ejecuta **una sola fase**. No empieces la siguiente aunque termines pronto.
- Al terminar una fase: el proyecto compila, las pruebas pasan, haces commit en cada repositorio tocado, y escribes `docs/fases/fase-N.md` con qué hiciste, cómo lo pruebo en Revit paso a paso, qué quedó pendiente y qué dudas tienes. El mensaje final del chat es un resumen corto que remite a ese archivo.
- En la Fase 0 no escribes código de producción: solo exploras, pruebas cosas pequeñas y entregas un plan.
- Tienes el servidor MCP `revit` conectado en la sesión. Úsalo: para inspeccionar el modelo, para probar llamadas a la API de Revit con `execute_revit_code` antes de escribirlas en C#, y para probar de verdad las herramientas `conn_*` cuando existan.
- **No informes como probado nada que no hayas ejecutado.** Si algo necesita Revit y no pudiste ejecutarlo, escribe "NO PROBADO" y por qué.
- Si no puedes verificar una clase o método de la API de Revit, no lo inventes: haz una prueba pequeña primero (con `execute_revit_code` o compilando un fragmento) y construye encima solo cuando funcione. En C#, "compila" es la prueba mínima de que el miembro existe.
- No modifiques ni borres herramientas, rutas ni archivos existentes del repositorio revit-mcp. Solo añades archivos nuevos y las líneas de registro que indica la sección 8. Si crees que hace falta tocar algo existente, dilo en el informe y espera a la siguiente fase.
- Cuando necesites que yo haga algo en Revit (reiniciar Revit, recargar pyRevit, seleccionar elementos, abrir un modelo), escríbelo como paso numerado en el informe. No te quedes esperando en el chat.
- Nombres de código, clases, archivos de código y claves JSON en inglés. Mensajes al usuario, errores, comentarios importantes, informes y documentación en español.
- Commits con mensaje claro en español. No abras pull requests. No hagas push a ramas que no sean la de trabajo.

## 2. Entorno

- Carpeta del add-in (este repositorio): **[COMPLETAR, p. ej. C:\dev\CONEXIONES]**
- Carpeta del servidor MCP: **[COMPLETAR, p. ej. C:\dev\revit-mcp]**
- Versión de Revit: **2027.2** (instalado y funcionando con pyRevit y el MCP). Runtime: .NET 10. La Fase 0 solo lo confirma con `doc.Application.VersionNumber` y `VersionBuild`.
- Modelo de prueba: **[COMPLETAR ruta del .rvt]**. Contiene una cercha con cordón y diagonales HSS.
- Rama de trabajo en CONEXIONES: `main`. Rama de trabajo en revit-mcp: `feature/conn-tools`.

Comandos que uso para comprobar tu trabajo (deben funcionar desde la raíz de CONEXIONES):

```powershell
dotnet build MotorConexiones.sln -c Release
dotnet test
.\scripts\deploy.ps1        # copia el add-in a la carpeta Addins de mi versión de Revit
```

## 3. Contexto: qué es el MCP que ya tengo y cómo encaja el add-in

Lo siguiente está verificado leyendo el repositorio revit-mcp. Confírmalo en la Fase 0 y corrige lo que haya cambiado.

**Arquitectura del MCP existente**

```
Claude ──stdio──> main.py (CPython 3.11+, SDK mcp 2.2, MCPServer)
                   └── tools/*.py: una función por herramienta; llama a revit_get / revit_post
                        ──HTTP 127.0.0.1:48884/revit_mcp/<ruta>/ (token de sesión)──>
                             pyRevit Routes dentro de Revit (IronPython 2.7)
                              └── startup.py registra rutas; revit_mcp/*.py las implementa
                                   └── Revit API (los manejadores abren Transactions: corren en contexto de API)
```

Hechos que condicionan el diseño:

1. Los manejadores de rutas (`@api.route(...)` + `@requiere_token`) reciben por nombre `doc`, `uidoc` y `request`, y **ya corren en contexto de la API de Revit** (pyRevit los ejecuta mediante ExternalEvent y espera a Revit). Por eso el add-in **no necesita** su propio despachador de ExternalEvent ni cola de peticiones: el punto de entrada C# se llama desde el manejador IronPython y puede abrir transacciones directamente.
2. Toda ruta exige el token de sesión (`revit_mcp/seguridad.py`). Las rutas nuevas llevan `@requiere_token` justo debajo de `@api.route`. No hay que inventar otro mecanismo de seguridad.
3. `revit_post` (main.py) devuelve el JSON como dict solo si el HTTP es 200; con otro código devuelve un texto `"Error: <código> - <cuerpo>"`. Por eso las rutas `conn_*` responden **siempre 200** con el sobre común de la sección 9, también cuando hay errores de validación. Se reservan 401 (token), 503 (sin documento) y 500 (excepción no controlada).
4. `tools/utils.py:format_response` aplana los dicts a texto "Clave: valor". Las herramientas `conn_*` **no lo usan**: devuelven `json.dumps(respuesta, ensure_ascii=False, indent=2)` para que la IA reciba el JSON íntegro.
5. `revit_mcp/utils.py:sanitize_string` convierte acentos en `?`. Los mensajes en español del add-in no deben pasar por ahí: el manejador IronPython hace `json.loads` de lo que devuelve C# y lo entrega tal cual con `routes.make_response(data=...)`.
6. El tiempo de espera por defecto de `revit_post` es 30 s. `conn_create`, `conn_update` y `conn_preview` lo pasan explícitamente (`timeout=180.0`).
7. `revit_mcp/utils.py:suppress_warnings(t)` ya muestra cómo evitar diálogos modales con `IFailuresPreprocessor`. El add-in implementa lo mismo en C# (sección 4) y además se suscribe a `UIApplication.DialogBoxShowing` mientras atiende una petición.
8. Ya existen herramientas que puedes reutilizar en vez de duplicar: `get_selected_elements`, `get_element_properties`, `list_families`, `get_revit_view` (imagen de una vista), `execute_revit_code` (IronPython con `doc`, `DB`, `revit`, `clr`, `System`; todo dentro de un `TransactionGroup` que Revit deshace como una sola entrada).
9. Añadir una herramienta al MCP son 2 archivos + 2 líneas de registro: `revit_mcp/<modulo>.py` (IronPython) registrado en `startup.py`, y `tools/<modulo>_tools.py` (CPython) registrado en `tools/__init__.py`. Sigue exactamente ese patrón.
10. Revit 2027: en IronPython usa `get_element_id_value` y `make_element_id` de `revit_mcp/utils.py` (en 2027 `DB.ElementId(<int>)` a secas falla por ambigüedad de sobrecargas; hay que pasar `System.Int64`). En C# usa `ElementId.Value` (long) y nunca `IntegerValue`. Comprueba en la Fase 0 que ninguna API que vayas a usar esté entre las retiradas en 2027 (el README del MCP lista las conocidas).

**Cómo encaja el add-in (decisión tomada; la Fase 1 la confirma con una prueba)**

- MotorConexiones es un add-in normal de Revit (manifiesto `.addin` + DLL en la carpeta Addins). Revit lo carga al arrancar. Aporta el botón de la cinta y un punto de entrada estático:
  `MotorConexiones.Revit.Bridge.Handle(string operation, string requestJson, Document doc, UIDocument uidoc) -> string responseJson`.
- El módulo nuevo `revit_mcp/conexiones.py` es un adaptador delgado: localiza el ensamblado `MotorConexiones.Revit` ya cargado en el proceso de Revit (primero `AppDomain.CurrentDomain.GetAssemblies()` por nombre; si no está, `clr.AddReferenceToFileAndPath` con la ruta de la DLL desplegada), llama a `Bridge.Handle` y devuelve el JSON. Cero lógica de negocio en Python.
- Si en la Fase 1 esa llamada no funciona en mi versión de Revit (por ejemplo por el aislamiento de ensamblados de add-ins que Revit puede aplicar en 2027), el plan B es que el add-in levante su propio `HttpListener` en `127.0.0.1:48885`, reutilizando el mismo archivo de token, y que `tools/conn_tools.py` hable con ese puerto. Documenta en el informe cuál de los dos caminos quedó.

## 4. Arquitectura del add-in

```
CONEXIONES/
├── MotorConexiones.sln
├── src/
│   ├── MotorConexiones.Core/        netstandard2.0, sin referencias a Revit
│   │   ├── Contract/                modelos del contrato (ConnectionSpec, GussetNodeSpec, ...)
│   │   ├── Schema/                  generación del JSON Schema por tipo + ejemplo lleno
│   │   ├── Units/                   mm y grados <-> unidades internas (UN SOLO archivo)
│   │   ├── Geometry2D/              polígonos, patrones de pernos, comprobaciones planas
│   │   ├── Validation/              reglas, códigos de error, carga de config/limits.json
│   │   └── Types/                   un archivo por tipo de conexión (IConnectionType)
│   ├── MotorConexiones.Revit/       add-in: net48 y/o net8.0-windows según la versión de Revit
│   │   ├── App.cs                   IExternalApplication: cinta, botón "Ejecutar especificación JSON"
│   │   ├── Bridge.cs                punto de entrada estático para el MCP
│   │   ├── Node/                    inspección del nudo, sistema local, perfiles
│   │   ├── Fabrication/             IFabricationBackend + implementación elegida en Fase 1
│   │   ├── Storage/                 Extensible Storage en DataStorage
│   │   ├── Transactions/            TransactionGroup + IFailuresPreprocessor + DialogBoxShowing
│   │   └── Logging/                 JSON por línea en %LOCALAPPDATA%\MotorConexiones\log\
│   └── MotorConexiones.Tests/       xUnit, solo Core; fixture Detalle D
├── config/limits.json               mínimos AISC (pernos, soldaduras) editables sin recompilar
├── docs/guide.md                    contenido de conn_get_guide, editable sin recompilar
├── docs/fixtures/detalle-D.json     especificación del caso de prueba
├── docs/fixtures/detalle-D.png      imagen del detalle (si la tengo)
├── docs/fases/                      informes de cada fase
└── scripts/deploy.ps1               compila y copia DLL + .addin + config + guide a Addins\<versión>
```

Detalles técnicos:

- **Referencias a la API de Revit:** primero intenta los paquetes NuGet `Nice3point.Revit.Api.RevitAPI` y `Nice3point.Revit.Api.RevitAPIUI` en su versión 2027.x (comprueba que existen). Si no existen, referencia `RevitAPI.dll` y `RevitAPIUI.dll` de `C:\Program Files\Autodesk\Revit 2027\` con `Private=false`. Para la API de acero, candidatos a verificar: `RevitAPISteel.dll` y los ensamblados de Advance Steel (`ASMgd.dll`, `ASObjectsMgd.dll`, ...) de la misma carpeta.
- **Versión de .NET:** Revit 2027 → un único target `net10.0-windows`. Nada de multi-target ni de .NET Framework.
- **Transacciones:** una operación = un `TransactionGroup` con nombre `MotorConexiones: <operación> <connection_id>`. Dentro, las `Transaction` que hagan falta. Ante cualquier error: rollback del grupo completo (atómico). `FailureHandlingOptions` con `SetForcedModalHandling(false)` y un `IFailuresPreprocessor` que guarda las advertencias en la respuesta y hace rollback si hay errores. Mientras se atiende una petición del MCP, `DialogBoxShowing` cancela cualquier diálogo y lo anota como advertencia `REVIT_DIALOG_SUPPRESSED`.
- **Ocupado:** si `doc.IsModifiable` (ya hay una transacción abierta) o `doc.IsReadOnly`, responder `REVIT_BUSY` con sugerencia, sin intentar nada.
- **Unidades:** el contrato usa mm y grados. La conversión a pies y radianes vive en un único archivo (`Units/UnitConverter.cs`) y se usa desde ahí. Prohibido multiplicar por 304.8 en otro sitio.
- **Almacenamiento:** por cada conexión, un `DataStorage` con un `Schema` de Extensible Storage (GUID fijo, nombre `MotorConexiones.Connection`, versión 1) que guarda: `connection_id` (GUID), `spec_version`, `connection_type`, la especificación JSON completa, la lista de IDs creados y, para cada miembro modificado, sus valores originales (por ejemplo, retiros de extremo) para restaurarlos al borrar.
- **Registro:** cada llamada a `Bridge.Handle` escribe una línea JSON con fecha, operación, resumen de entrada, `ok`, códigos de error y duración en ms.
- **Botón de la cinta:** pestaña "Conexiones", botón "Ejecutar especificación JSON": abre un diálogo de archivo, lee el JSON, valida, muestra un resumen en un `TaskDialog` y, si el usuario acepta, crea. Es el único sitio del add-in donde se permite una ventana.

## 5. Principios de diseño

1. **Determinista:** misma especificación + mismo modelo = mismo resultado.
2. **Contrato versionado:** `spec_version` y un JSON Schema por tipo de conexión, consultable desde MCP. La IA nunca adivina campos.
3. **Herramientas pequeñas y componibles:** descubrir, inspeccionar, validar, previsualizar, crear, modificar, borrar.
4. **Validar antes de crear es obligatorio:** `conn_create` exige un `validation_token` que solo entrega `conn_validate` sin errores. El token es `sha256` de: la especificación en JSON canónico (claves ordenadas, sin espacios) + `ProjectInformation.UniqueId` del documento + por cada `element_id` implicado, ordenados: `UniqueId`, nombre del tipo y extremos de su curva de ubicación redondeados a 0,1 mm. Caduca a los 30 minutos. Si algo cambia, el token deja de servir con error `VALIDATION_TOKEN_INVALID`.
5. **Errores accionables:** código estable, ruta del campo, mensaje en español y sugerencia.
6. **Incertidumbre explícita:** `uncertain_fields`. Mientras haya dudas sin `user_confirmed_value`, no hay token.
7. **IDs estables:** `connection_id` guardado en el modelo.
8. **Atómico:** o se crea todo o no se crea nada.
9. **Sin ventanas** en las rutas que usa la IA.
10. **Límite de responsabilidad:** el add-in modela lo que dice el plano. No diseña ni verifica resistencias.

## 6. Cómo crear la geometría: decidir con una prueba (Fase 1)

Dos caminos detrás de la interfaz `IFabricationBackend` (crear placa, crear grupo de pernos, crear soldadura, recortar miembro, borrar lo creado):

- **A. Elementos de fabricación de acero de Revit** (placas, pernos, soldaduras y cortes de "Conexiones de acero"). Puntos de partida a verificar, no a dar por hechos: espacio de nombres `Autodesk.Revit.DB.Steel`, `SteelElementProperties`, `FabricationTransaction`, `StructuralConnectionHandler`, los ensamblados de Advance Steel que Revit instala y los ejemplos de acero del SDK de Revit.
- **B. Familias paramétricas propias** (placa genérica, perno) colocadas con transformaciones 3D. Las familias `.rfa` se generan o se guardan en `families/`.

Prefiero A si funciona en mi versión. La prueba mínima de la Fase 1: crear una placa y un grupo de 4 pernos en un nudo del modelo de prueba, primero explorando con `execute_revit_code` y después desde C#. Muéstrame el resultado (usa `get_revit_view` para adjuntar una imagen en el informe) y recomiéndame un camino.

## 7. Contrato v1: `gusset_node`

Nudo de cercha con cartela y cordón.

**Sistema de coordenadas local del nudo (decisión cerrada):**

- X: vector unitario del eje del cordón, del inicio al fin de su curva de ubicación.
- Z: normal al plano de la cercha = `X × d` normalizado, donde `d` es la dirección del primer miembro de `members`; se invierte el signo para que Z tenga componente global Z positiva (si es cero, componente global Y positiva).
- Y = Z × X.
- Origen: punto de trabajo = punto medio del segmento más corto entre el eje del cordón y el eje del primer miembro. Si esa distancia supera 5 mm → error `NODE_AXES_NOT_INTERSECTING` con la distancia medida.
- Todas las posiciones del contrato van en ese sistema, en mm. El add-in lo calcula a partir del modelo y `conn_get_node_info` lo devuelve para que la IA lo vea.

**Tipos de unión de cada miembro (`attachment`) en v1:**

- `welded_slot`: el HSS va ranurado en su extremo, abraza la cartela y se suelda.
- `bolted_knife_plate`: una placa (cuchilla) va soldada dentro de la ranura del HSS y se emperna a la cartela.

**Borrador del contrato.** Mejóralo, pero conserva las ideas. Los IDs y los números que no aparecen en el Detalle D son ilustrativos y la lista de vértices está abreviada:

```json
{
  "spec_version": "1.0",
  "connection_type": "gusset_node",
  "source": { "drawing": "Detalle D", "scale": "1/10" },
  "node": { "element_ids": [111, 222, 333, 444, 555] },
  "chord": { "element_id": 111, "profile": "HSS3X3X1/4", "continuous": true },
  "gusset": {
    "thickness_mm": 9.525,
    "thickness_label": "3/8\"",
    "width_mm": 565,
    "height_mm": 530,
    "outline": { "mode": "polygon", "points_mm": [[-250, 280], [245, 280], [315, 210], [315, -250]] },
    "chord_interface": "through_slot",
    "weld_to_chord": { "type": "fillet", "size_mm": 5, "all_around": true }
  },
  "members": [
    {
      "element_id": 222,
      "role": "diagonal",
      "profile": "HSS2-1/2X2-1/2X3/16",
      "end_setback_mm": 180,
      "attachment": {
        "type": "welded_slot",
        "slot_length_mm": 150,
        "weld": { "type": "fillet", "size_mm": 5, "all_around": true }
      }
    },
    {
      "element_id": 333,
      "role": "vertical",
      "profile": null,
      "end_setback_mm": 60,
      "attachment": {
        "type": "welded_slot",
        "slot_length_mm": 150,
        "weld": { "type": "fillet", "size_mm": 5, "all_around": true }
      }
    },
    {
      "element_id": 555,
      "role": "diagonal",
      "profile": "HSS2-1/2X2-1/2X3/16",
      "end_setback_mm": 260,
      "attachment": {
        "type": "bolted_knife_plate",
        "plate": {
          "thickness_mm": 10,
          "thickness_label": "PL10",
          "length_mm": 170,
          "width_mm": 140,
          "insertion_mm": 80
        },
        "bolts": {
          "diameter_mm": 15.875,
          "diameter_label": "5/8\"",
          "rows": 2,
          "columns": 2,
          "spacing_mm": 60,
          "edge_mm": 40,
          "first_row_from_plate_end_mm": 40
        },
        "weld_plate_to_member": { "type": "fillet", "size_mm": 5, "all_around": true }
      }
    }
  ],
  "dimension_chains": [
    { "label": "borde superior", "values_mm": [75, 420, 70], "expected_total_mm": 565 },
    { "label": "lado derecho", "values_mm": [70, 250, 210], "expected_total_mm": 530 }
  ],
  "uncertain_fields": [
    {
      "path": "members[1].profile",
      "reason": "La etiqueta del montante está cortada en la imagen",
      "user_confirmed_value": null
    },
    {
      "path": "gusset.chord_interface",
      "reason": "El dibujo no muestra con claridad cómo se une la cartela al cordón",
      "user_confirmed_value": null
    }
  ]
}
```

**Reglas del contrato:**

- Los campos numéricos van siempre en mm. Los campos `*_label` guardan el texto del plano (por ejemplo `3/8"`) y el validador comprueba que coincidan con el número (9.525 mm) con tolerancia de 0,05 mm.
- Un campo puede ser `null` solo si su ruta está en `uncertain_fields`.
- `outline.mode`: en v1 solo `polygon` (vértices en el sistema local, en mm, contorno cerrado implícito). `auto` queda para v2, pero el esquema ya lo reserva.
- `chord_interface`: `through_slot`, `split_top_bottom` o `side_lap`, con el significado del encargo original.
- La posición de la placa cuchilla y de sus pernos no puede quedar implícita: `insertion_mm` es la parte de la placa dentro de la ranura del HSS, y `first_row_from_plate_end_mm` se mide desde el extremo libre de la placa (el que apoya en la cartela). Si al leer el plano no encaja, va a `uncertain_fields`.
- La arquitectura debe permitir agregar tipos de conexión nuevos como archivos nuevos en `Core/Types/`, sin tocar el núcleo. Cada tipo registra su nombre, su esquema, su ejemplo y sus validaciones propias.

## 8. Validaciones v1

Todas devuelven errores o advertencias con código, ruta, mensaje en español y sugerencia. Los mínimos numéricos van en `config/limits.json`, que yo puedo editar sin recompilar.

1. **Esquema:** campos obligatorios, tipos y rangos → `SCHEMA_INVALID`.
2. **Dudas:** `uncertain_fields` sin `user_confirmed_value` → `UNRESOLVED_UNCERTAINTY`.
3. **Cadenas de cotas:** cada `dimension_chains` suma su total con tolerancia de 1 mm → `DIMENSION_CHAIN_MISMATCH`.
4. **Etiquetas contra números** → `LABEL_VALUE_MISMATCH`.
5. **Perfiles:** el perfil escrito coincide con el nombre del tipo del elemento en el modelo → `PROFILE_MISMATCH`. Si el tipo no existe, sugerir los tres más parecidos.
6. **Ángulos:** si la especificación trae ángulos leídos del plano, compararlos con los del modelo; advertencia `ANGLE_DIFFERS_FROM_MODEL` si difieren más de 1°.
7. **Pernos:** distancia mínima al borde y separación mínima según AISC 360 (tablas J3.3 y J3.4) leídas de la configuración → `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL`; todos los pernos dentro de su placa → `BOLT_OUTSIDE_PLATE`.
8. **Soldaduras:** tamaño mínimo de filete según el espesor más delgado (AISC 360, tabla J2.4), como advertencia `WELD_BELOW_MINIMUM`.
9. **Geometría:** contorno cerrado y sin cruces → `OUTLINE_INVALID`; placas cuchilla dentro de la cartela → `PLATE_OUTSIDE_GUSSET`; sin choques con miembros que no forman parte de la unión → `CLASH_WITH_FOREIGN_MEMBER`.
10. **Modelo:** todos los `element_ids` existen, son armazón estructural y llegan al nudo → `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE`.

## 9. Herramientas MCP (prefijo `conn_`)

La descripción (docstring) de cada herramienta es el manual de la IA: cuándo usarla, qué se necesita antes, qué devuelve y errores comunes. Respuestas cortas, sin volcar datos enormes.

| Herramienta | Qué hace | Ruta en Revit |
|---|---|---|
| `conn_ping` | Comprueba que el add-in está cargado y devuelve su versión, la versión de Revit y el backend activo. | `GET /conn/ping/` |
| `conn_get_guide` | Devuelve `docs/guide.md` (sección 11). La IA la llama al empezar. | `GET /conn/guide/` |
| `conn_list_types` | Tipos de conexión disponibles y cuándo usar cada uno. | `GET /conn/types/` |
| `conn_get_schema` | JSON Schema de un tipo más un ejemplo lleno. | `GET /conn/schema/<type>` |
| `conn_get_node_info` | Dados unos IDs (o la selección actual si no se pasan): punto de trabajo, sistema local, miembros con perfil, material, ángulo en el plano, extremo que llega al nudo y conexiones existentes. | `POST /conn/node_info/` |
| `conn_find_profile` | Busca tipos de perfil cargados que coincidan con un nombre como `HSS2-1/2X2-1/2X3/16`. | `POST /conn/find_profile/` |
| `conn_validate` | Valida la especificación. Devuelve errores, advertencias, valores calculados y el `validation_token`. | `POST /conn/validate/` |
| `conn_preview` | Simulación en texto: lista lo que se crearía y lo que se modificaría, sin guardar nada. Sin imagen en v1 (para ver el resultado, la IA usa `get_revit_view` después de crear). | `POST /conn/preview/` |
| `conn_create` | Crea la conexión. Exige `validation_token`. Devuelve `connection_id` y los IDs creados. | `POST /conn/create/` |
| `conn_list` | Lista las conexiones creadas por el add-in. | `GET /conn/list/` |
| `conn_get` | Devuelve la especificación guardada de una conexión. | `GET /conn/get/<connection_id>` |
| `conn_update` | Reemplaza una conexión de forma atómica conservando su `connection_id`. Exige token. | `POST /conn/update/` |
| `conn_delete` | Borra una conexión y restaura los miembros modificados. Nunca borra elementos que el add-in no creó. | `POST /conn/delete/` |

`conn_get_selection` no se implementa: ya existe `get_selected_elements`, y `conn_get_node_info` sin IDs usa la selección.

**Reglas de implementación en el repositorio revit-mcp** (rama `feature/conn-tools`):

- Archivos nuevos: `revit_mcp/conexiones.py` (IronPython 2.7: rutas `/conn/...`, `@requiere_token`, adaptador a `Bridge.Handle`) y `tools/conn_tools.py` (CPython: una función `@mcp.tool()` por fila de la tabla, devuelve el JSON tal cual).
- Líneas nuevas: una importación y una llamada `register_conn_routes(api)` en `startup.py`; una importación y una llamada `register_conn_tools(...)` en `tools/__init__.py`. Nada más se toca.
- Cada ruta responde 200 con el sobre de la sección 10. Si el add-in no está cargado: 200 con `ok:false` y error `ADDIN_NOT_LOADED` con la sugerencia de instalarlo y reiniciar Revit.
- Añade a `CONTRATO.md` una sección nueva al final con las rutas `/conn/...` (añadir, no reescribir).
- Añade `pruebas/probar_conexiones.py` al estilo de `pruebas/probar_revit.py`: ping, guía, esquema, validar el Detalle D con dudas confirmadas (sin errores), validar con 420→402 (`DIMENSION_CHAIN_MISMATCH`).

## 10. Formato de respuesta común

```json
{
  "ok": false,
  "data": null,
  "errors": [
    {
      "code": "BOLT_EDGE_DISTANCE_TOO_SMALL",
      "path": "members[2].attachment.bolts.edge_mm",
      "message": "La distancia al borde (15 mm) es menor que el mínimo configurado (22 mm) para pernos de 5/8\".",
      "hint": "Revisa la cota en el plano o usa edge_mm >= 22."
    }
  ],
  "warnings": [],
  "meta": { "operation": "validate", "duration_ms": 41, "addin_version": "0.1.0" }
}
```

## 11. Guía para la IA (contenido de `docs/guide.md`, servido por `conn_get_guide`)

1. Llama a `conn_ping`. Si el add-in no está cargado, díselo al usuario y para.
2. Pide al usuario que seleccione en Revit los miembros del nudo y llama a `conn_get_node_info`.
3. Lee el detalle: rótulos de perfiles, placas, pernos y soldaduras. En planos de acero las cotas van en mm salvo que se indique otra cosa. "TIP." significa típico; el círculo en el símbolo de soldadura significa soldadura en todo el contorno.
4. Los ángulos y las posiciones salen del modelo, no del dibujo.
5. Transcribe cada cadena de cotas completa en `dimension_chains`, con el total que debería dar.
6. Nunca inventes un dato ilegible o ausente: va en `uncertain_fields` con el motivo.
7. Llama a `conn_validate`. Corrige lo que sea error tuyo; si el error depende del plano, pregunta al usuario.
8. Muestra al usuario un resumen corto (tabla con lo leído y las dudas) y espera su confirmación explícita.
9. Llama a `conn_preview`, luego a `conn_create` con el token, e informa el `connection_id`. Si quieres enseñar el resultado, usa `get_revit_view`.

## 12. Caso de prueba: "Detalle D" (ESC. 1/10)

Si tengo la imagen, está en `docs/fixtures/detalle-D.png`. Estos son sus datos. Úsalo como fixture en las pruebas del Core (`docs/fixtures/detalle-D.json`) y como prueba de punta a punta en la Fase 5.

- Cordón horizontal continuo: HSS3X3X1/4".
- Cartela: PL 3/8" x 530 mm de alto x 565 mm de ancho, con las esquinas superiores recortadas y el borde inferior derecho inclinado.
- Dos diagonales superiores HSS2-1/2X2-1/2X3/16", ranuradas y soldadas a la cartela.
- Montante vertical ranurado y soldado. Su perfil no es legible (solo se ve "HSS1..."): va a `uncertain_fields`.
- Diagonal inferior HSS2-1/2X2-1/2X3/16" con placa cuchilla PL10 (10 mm), 170 mm a lo largo del miembro y 4 pernos Ø5/8" en 2x2.
  - En el ancho de la placa se lee 40 + 60 + 40 = 140 mm.
  - A lo largo del miembro se leen 40, 60 y 43 mm, además de 38 mm cerca del extremo. Su referencia exacta no es segura: si no cierra, va a `uncertain_fields`.
- Soldadura típica: filete de 5 mm en todo el contorno.
- Cadenas de cotas que deben cerrar:
  - Borde superior: 75 + 420 + 70 = 565
  - Base: 125 + 90 + 350 = 565
  - Lado derecho: 70 + 250 + 210 = 530
  - Lado izquierdo: 70 + 325 + 135 = 530
- Cómo se une la cartela al cordón no se ve con certeza: `chord_interface` va a `uncertain_fields`.

**Pruebas mínimas del Core (xUnit):**

- La especificación correcta, con las dudas confirmadas, valida sin errores.
- Cambiar 420 por 402 da `DIMENSION_CHAIN_MISMATCH`.
- Poner bordes de 15 mm en los pernos da `BOLT_EDGE_DISTANCE_TOO_SMALL`.
- Poner `thickness_label` "3/8\"" con `thickness_mm` 12 da `LABEL_VALUE_MISMATCH`.
- Dejar la duda del montante sin confirmar impide obtener el token.
- El JSON Schema generado acepta el fixture y rechaza un campo desconocido.
- La conversión de unidades es reversible (mm → pies → mm) con error menor de 0,001 mm.

## 13. Fases y definición de hecho

Cada fase termina con: compila, pruebas en verde, commit en cada repositorio tocado, `docs/fases/fase-N.md` escrito, y resumen corto en el chat.

**Fase 0. Exploración y plan (sin código de producción).**
Usando el MCP y el sistema de archivos: confirmar Revit 2027.2 y el SDK de .NET 10 (`execute_revit_code` con `doc.Application.VersionNumber` y `VersionBuild`; `dotnet --list-sdks`), si la API de acero está disponible (prueba en IronPython: `clr.AddReference` a los ensamblados candidatos y listar unas cuantas clases), perfiles HSS cargados en el modelo de prueba, cómo pyRevit ejecuta los manejadores (confirma el hecho 1 de la sección 3), y si un manejador IronPython puede invocar un método estático de una DLL cargada en Revit (prueba con cualquier DLL ya cargada, por ejemplo `RevitAPI`). Entrega `docs/fases/fase-0.md` con: hechos verificados, plan ajustado, riesgos y preguntas concretas para mí.

**Fase 1. Prueba técnica.**
Esqueleto de la solución (`Core`, `Revit`, `Tests`), `scripts/deploy.ps1`, `conn_ping` de punta a punta (Claude → MCP → IronPython → `Bridge.Handle` → respuesta), y la placa con 4 pernos en un nudo del modelo de prueba con el camino A; si A no funciona, con B. Entrega: decisión A o B con evidencia (imagen con `get_revit_view`), y qué DLL o familias hacen falta.

**Fase 2. Core.**
Contrato, esquema, unidades, validaciones, `config/limits.json`, fixture Detalle D y todas las pruebas de la sección 12. Se prueba con `dotnet test`; no necesita Revit.

**Fase 3. Add-in.**
Inspección del nudo y sistema local, `IFabricationBackend` completo para `gusset_node`, almacenamiento, borrar y actualizar, registro, transacciones sin ventanas, y el botón "Ejecutar especificación JSON". Prueba en Revit con el JSON del Detalle D desde el botón.

**Fase 4. MCP.**
`revit_mcp/conexiones.py`, `tools/conn_tools.py`, registros, `docs/guide.md`, sección nueva en `CONTRATO.md`, `pruebas/probar_conexiones.py`. Prueba llamando a cada herramienta `conn_*` desde la propia sesión.

**Fase 5. Punta a punta y documentación.**
Con la imagen o los datos del Detalle D, seguir la guía completa desde el agente hasta `conn_create`, después `conn_delete` y comprobar que los miembros vuelven a su estado. README en español con: instalación paso a paso, cómo actualizar el add-in, cómo editar `limits.json` y `guide.md`, y cómo agregar un tipo de conexión nuevo.

## 14. Fuera de alcance en v1

- Diseño o verificación de resistencia.
- Otros tipos de conexión (placa base, viga-columna, empalmes). La arquitectura queda lista para agregarlos.
- Lectura automática de planos PDF completos.
- Imagen en `conn_preview` y `outline.mode = "auto"`.

---

## Anexo A. Prompts de arranque por fase

Pega uno por sesión. No cambies nada más.

**Fase 0**
```
Lee CLAUDE.md y docs/ENCARGO_MOTOR_CONEXIONES.md completos. Ejecuta SOLO la Fase 0 (sección 13).
No escribas código de producción. Usa el MCP `revit` para verificar lo que el encargo pide verificar.
Termina con docs/fases/fase-0.md, un commit y un resumen corto.
```

**Fase 1**
```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md y docs/fases/fase-0.md. Ejecuta SOLO la Fase 1.
Antes de usar cualquier clase de la API de acero, pruébala con execute_revit_code o compilando.
Termina con docs/fases/fase-1.md, commits en los dos repositorios y un resumen corto.
```

**Fase 2**
```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md y docs/fases/fase-1.md. Ejecuta SOLO la Fase 2.
Debe terminar con `dotnet build` y `dotnet test` en verde. Termina con docs/fases/fase-2.md, commit y resumen corto.
```

**Fase 3**
```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md y docs/fases/fase-2.md. Ejecuta SOLO la Fase 3.
Cuando necesites que reinicie Revit, dímelo en el informe como paso numerado. Termina con docs/fases/fase-3.md, commit y resumen corto.
```

**Fase 4**
```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md y docs/fases/fase-3.md. Ejecuta SOLO la Fase 4.
Solo añades archivos nuevos y las líneas de registro indicadas en la sección 9 del encargo dentro de revit-mcp.
Termina con docs/fases/fase-4.md, commits en los dos repositorios y un resumen corto.
```

**Fase 5**
```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md y docs/fases/fase-4.md. Ejecuta SOLO la Fase 5.
Termina con docs/fases/fase-5.md, README.md, commit y resumen corto.
```

## Anexo B. Qué debe contener cada `docs/fases/fase-N.md`

1. Qué se hizo (lista corta).
2. Qué se probó y cómo, con el resultado literal (comandos y salidas resumidas). Lo no probado, marcado "NO PROBADO" y por qué.
3. Cómo lo pruebo yo en Revit, paso a paso, con archivo, botón y comando.
4. Decisiones tomadas y por qué.
5. Pendientes, riesgos y preguntas para mí.
