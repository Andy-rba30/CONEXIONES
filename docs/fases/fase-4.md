# Fase 4: MCP (rutas `/conn/` con nombre, herramientas `conn_*`, guía, contrato y pruebas)

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt`. Add-in 0.1.0 (sin cambios en C#); adaptador y herramientas 0.4.0.

---

## 1. Qué se hizo

- **`mcp/revit_mcp/conexiones.py`** (IronPython 2.7, dentro de Revit): a las tres rutas de la Fase 1 (`/conn/ping/`,
  `/conn/op/<operation>/`, `/conn/dev_exec/`) se añaden las **12 rutas con nombre** de la sección 9 del encargo:
  `GET /conn/guide/`, `GET /conn/types/`, `GET /conn/schema/<type>`, `POST /conn/node_info/`, `POST /conn/find_profile/`,
  `POST /conn/validate/`, `POST /conn/preview/`, `POST /conn/create/`, `GET /conn/list/`, `GET /conn/get/<connection_id>`,
  `POST /conn/update/`, `POST /conn/delete/`. Cada una declara `doc` y `uidoc` (contexto de la API), lleva `@requiere_token`
  y llama a `Bridge.Handle` con el nombre de operación y el cuerpo JSON; todas responden 200 con el sobre común. Sigue sin
  haber lógica de negocio en Python (15 rutas en total).
- **`mcp/tools/conn_tools.py`** (CPython, SDK mcp 2.x): **13 herramientas** `conn_ping`, `conn_get_guide`, `conn_list_types`,
  `conn_get_schema`, `conn_get_node_info`, `conn_find_profile`, `conn_validate`, `conn_preview`, `conn_create`, `conn_list`,
  `conn_get`, `conn_update`, `conn_delete`. Docstrings en español que sirven de manual (cuándo usarla, qué hace falta antes,
  qué devuelve, errores comunes y qué hacer). Devuelven el sobre íntegro con `json.dumps(ensure_ascii=False, indent=2)`;
  `timeout=180.0` en `create`, `update`, `delete` y `preview`. `spec` se acepta como objeto o como texto JSON; los
  argumentos obligatorios vacíos se rechazan en el puente con `INVALID_REQUEST` sin llamar a Revit. Los textos de error de
  `revit_get`/`revit_post` (Revit cerrado, 404 por ruta no instalada, token cambiado, 5xx, tiempo de espera) se convierten
  en códigos con pista: `REVIT_UNREACHABLE`, `CONN_ROUTE_NOT_FOUND`, `REVIT_RESTARTED`, `CONN_ROUTE_EXCEPTION`, `REVIT_TIMEOUT`.
- **`mcp/instalar-conn.ps1`**: sin cambios de comportamiento (ya funcionaba en el PC desde la Fase 1); ahora imprime cuántas
  rutas `@api.route` y herramientas `@mcp.tool()` tiene cada archivo copiado, para que la salida del instalador demuestre
  qué versión quedó instalada.
- **`docs/guide.md`** reescrita como guía real para la IA (la devuelve `conn_get_guide`; `deploy.ps1` la copia junto al add-in
  y el add-in la lee en cada llamada, así que se edita sin recompilar): qué hace y qué no el add-in, el flujo obligatorio de
  10 pasos (sección 11 del encargo), cómo leer un detalle de acero (rótulos en pulgadas, `TIP.`, símbolo de soldadura, pernos),
  el sistema local y el significado de cada posición, una tabla "código de error → qué hacer" y las reglas de seguridad
  (nunca crear sin previsualizar y confirmar; no repetir `create` a ciegas; no tocar lo creado con `execute_revit_code`).
- **`mcp/CONTRATO-conn.md`**: la sección "Rutas `/conn/`" para pegar al final de `CONTRATO.md` de revit-mcp: qué hay detrás,
  seguridad, códigos HTTP y por qué siempre 200, el sobre común, tabla de las 15 rutas con cuerpo y `data`, códigos de error
  del adaptador, del puente y del add-in, tabla de las 13 herramientas con tiempos de espera, deshacer y pruebas.
- **`mcp/pruebas/probar_conexiones.py`** reescrito: 17 pruebas contra Revit (401 sin token; `ping`; guía; tipos; esquema de
  `gusset_node` y de un tipo inexistente; `find_profile`; `node_info` del nudo del fixture; `validate` del Detalle D con dudas
  confirmadas → token de 64 hex; `validate` con 420→402 → `DIMENSION_CHAIN_MISMATCH` sin token; `validate` de `detalle-D.json`
  con dudas sin confirmar → `UNRESOLVED_UNCERTAINTY`; `preview`; `create` sin token → `VALIDATION_TOKEN_INVALID`; `list`; `get`
  y `delete` de un ID inexistente → `ELEMENT_NOT_FOUND`; `op/no_existe` → `UNKNOWN_OPERATION`) y, con `--puente`, 2 más contra
  el puente MCP en `http://127.0.0.1:8000/mcp` (`tools/list` debe traer las 13 `conn_*`; `tools/call conn_ping` → `ok:true`).
  **No crea ni borra nada en el modelo.** Termina con `Resultado: N/N pruebas correctas` y código de salida.
- **`mcp/pruebas/simulador_revit.py`** (nuevo, solo para la nube): carga el `conexiones.py` real con módulos `pyrevit`, `clr`,
  `System` y `StringIO` simulados y el `seguridad.py` real de la extensión, sustituye `Bridge.Handle` por una imitación en
  Python del add-in (mismas operaciones y códigos) y sirve `/revit_mcp/conn/...` en 48884 con token. Con `--autocomprobar`
  verifica en proceso que las 15 rutas llegan al puente con la operación y el cuerpo correctos. Permite ejecutar en Linux el
  script de pruebas y el puente `main.py` real. El instalador **no** lo copia a ningún sitio.
- `docs/instalacion/fase-4.md` (11 pasos) y este informe.

Sin cambios en `src/` (C#), `config/limits.json`, `scripts/` ni fixtures.

## 2. Qué se probó en la nube y cómo

Herramientas de la sesión: SDK .NET 10.0.112 (`apt`, como en la Fase 0; `dotnet-install.sh` sigue bloqueado por el proxy),
PowerShell 7.6.6 (`packages.microsoft.com`), venv con `mcp[cli]==2.2.0` y `httpx 0.28.1` (las mismas versiones que la
extensión del PC: `pyproject.toml` pide `mcp[cli]>=2.2,<3`), y un clon de solo lectura de `revit-mcp` (commit `8505508`).

### 2.1 Compilación y pruebas del Core (sin cambios en C#, para dejar constancia)

```text
$ dotnet build MotorConexiones.sln -c Release
  MotorConexiones.Core  -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
Build succeeded.  0 Warning(s)  0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build
Passed!  - Failed: 0, Passed: 50, Skipped: 0, Total: 50
```

### 2.2 `conexiones.py` en proceso (`simulador_revit.py --autocomprobar --extension <clon>`)

Con el `seguridad.py` **real** del clon de revit-mcp (su decorador `requiere_token` genera la función envuelta con `exec`,
igual que en IronPython) y `pyrevit.routes` simulado:

```text
Autocomprobación de conexiones.py (seguridad.py real de .../revit-mcp/revit_mcp/seguridad.py)
  [OK] 15 rutas registradas
  [OK] GET /conn/ping/ -> Bridge.Handle('ping')            ... (las 13 rutas con nombre y /conn/op/<operation>/:
  [OK] GET /conn/schema/gusset_node -> Bridge.Handle('schema')  cuerpo={"type": "gusset_node"}
  [OK] GET /conn/get/abc-123 -> Bridge.Handle('get')        cuerpo={"connection_id": "abc-123"}
  [OK] POST /conn/create/ -> Bridge.Handle('create')        cuerpo={"spec": {...}, "validation_token": "x"}   ...)
  [OK] GET /conn/ping/ sin token -> 401        [OK] POST /conn/validate/ sin token -> 401     [OK] token incorrecto -> 401
  [OK] POST /conn/dev_exec/ print('hola')      [OK] dev_exec excepción -> PROBE_EXCEPTION      [OK] GET /conn/nada/ -> 404
  [OK] GET /conn/ping/ sin add-in -> 200 ADDIN_NOT_LOADED
  [OK] POST /conn/validate/ sin documento -> NO_DOCUMENT    [OK] GET /conn/guide/ sin documento -> ok:true
Autocomprobación: 24/24 correctas
```

`python3 -m py_compile` en los cuatro archivos Python y búsqueda de construcciones exclusivas de Python 3 en
`conexiones.py` (f-strings, `:=`, `async`, `nonlocal`, `print(`...): ninguna. **La ejecución real en IronPython 2.7 dentro
de Revit es PENDIENTE DE INSTALADOR** (pasos 4-5 a 4-8); las rutas nuevas usan exactamente las mismas construcciones que
las tres que ya corren en el PC desde la Fase 1 y el mismo patrón de parámetro de ruta (`<nombre>` pasado por nombre) que
`/get_view/<view_name>` de la extensión.

### 2.3 `probar_conexiones.py` contra el simulador (puerto 48884, token de archivo)

```text
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.1.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 8855 caracteres
4. GET /conn/types/  [OK]  tipos=['gusset_node']
5. GET /conn/schema/gusset_node  [OK]  claves de data=['connection_type', 'description', 'example', 'json_schema'], ejemplo.members=3
6. GET /conn/schema/no_existe -> ok:false  [OK]  errores=['UNKNOWN_OPERATION']
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  coincidencias=['HSS2-1-2X2-1-2X3-16 64x64']
8. POST /conn/node_info/ 4 miembros  [OK]  cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423.0]
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  avisos=['ANGLE_DIFFERS_FROM_MODEL', ...], is_valid=True token=551074966166...
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  ok=False, errores=['DIMENSION_CHAIN_MISMATCH'], sin token
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  errores=['UNRESOLVED_UNCERTAINTY', 'UNRESOLVED_UNCERTAINTY'], sin token
12. POST /conn/preview/ Detalle D  [OK]  resumen={... "gusset_plates": 1, "knife_plates": 1, "bolts": 4, "weld_lines": 6, "members_modified": 3, "dry_run": true}
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]
14. GET /conn/list/  [OK]  conexiones en el modelo=0
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]
Resultado: 17/17 pruebas correctas
```

Las respuestas del simulador imitan al add-in; **contra el add-in real es PENDIENTE DE INSTALADOR (paso 4-6)**. Lo que esta
ejecución sí prueba de verdad: el script, la forma de las peticiones (token en cuerpo o en `?token=`, `spec` envuelta,
parámetros de ruta codificados) y la lectura de los fixtures.

### 2.4 Puente MCP real (`main.py --streamable-http` del clon con `instalar-conn.ps1` aplicado, SDK mcp 2.2.0)

```text
18. tools/list por el puente trae las 13 herramientas conn_*  [OK]  HTTP 200, herramientas=61 conn_*=13
    conn_ping, conn_get_guide, conn_list_types, conn_get_schema, conn_get_node_info, conn_find_profile, conn_validate,
    conn_preview, conn_create, conn_list, conn_get, conn_update, conn_delete
19. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.1.0
Resultado: 19/19 pruebas correctas
```

Además, en proceso (importando `main.py` del clon modificado y llamando a `mcp.call_tool` contra el simulador): 61 herramientas
registradas (48 existentes + 13), y 23 llamadas comprobadas: cada herramienta con argumentos válidos; `conn_get_schema` de un
tipo inexistente → `ok:false`; `conn_get_node_info` sin IDs (selección); `conn_find_profile` con `query` vacía →
`INVALID_REQUEST` sin llamar a Revit; `conn_validate` con `spec` como objeto y como texto JSON (ambas `ok:true`) y con texto
que no es JSON → `INVALID_REQUEST`; `conn_create` sin token → `INVALID_REQUEST`, con token falso → `VALIDATION_TOKEN_INVALID`,
con el token de `conn_validate` → `ok:true`; después `conn_list` (1), `conn_get`, `conn_update`, `conn_delete` (ok) y
`conn_delete` repetido → `ELEMENT_NOT_FOUND`; y los acentos llegan sin escapar (`especificación`, ningún `\u00`).
`Herramientas: 23/23 correctas`.

### 2.5 `instalar-conn.ps1` (PowerShell 7.6.6 en Linux, sobre una copia del clon)

Sintaxis comprobada con `Parser::ParseFile`. Primera ejecución: copia los dos archivos e inserta las 2+2 líneas (`diff`
idéntico al de la Fase 1: `register_conn_routes(api)` antes de `logger.info("All MCP routes registered successfully")`, y el
import y la llamada al final de `register_tools()`); crea `startup.py.bak-conn` y `tools\__init__.py.bak-conn`. Segunda
ejecución: "ya tenia" en ambos, sin cambios. Salida nueva:

```text
- copiado revit_mcp\conexiones.py (15 rutas @api.route)
- copiado tools\conn_tools.py (13 herramientas @mcp.tool)
```

**NO PROBADO en Windows PowerShell 5.1** en esta sesión; en el PC ya corrió en las Fases 1 y 3 y lo único nuevo son dos
`[regex]::Matches` sobre el texto leído.

### 2.6 Lo que solo puede probarse en el PC (PENDIENTE DE INSTALADOR)

| Qué | Paso de `docs/instalacion/fase-4.md` |
|---|---|
| Que pyRevit registre las 15 rutas en IronPython 2.7 y que `<connection_type>` / `<connection_id>` lleguen por nombre | 4-5 (`/conn/guide/` por `Invoke-RestMethod`) y 4-6 (pruebas 5, 6 y 15) |
| Que el add-in real responda a las 17 pruebas con los datos del nudo del fixture | 4-6 |
| Que el puente `main.py` del PC (su `.venv`) cargue `conn_tools.py` y exponga las 13 herramientas | 4-7 |
| Que el cliente de IA vea y use las herramientas | 4-9 (lo hace la persona) |

## 3. Qué debo mirar yo cuando el instalador termine

1. Paso 4-4: `(15 rutas @api.route)` y `(13 herramientas @mcp.tool)`. Si dice menos, no se copió la versión de esta fase.
2. Paso 4-5: `backend: advancesteel` y las primeras líneas de la guía nueva ("Guía para la IA: crear conexiones de acero...").
   Un 404 ahí significa que pyRevit no recargó `conexiones.py`.
3. Paso 4-6: `Resultado: 17/17`. Las pruebas 9 y 12 son las que hablan con el modelo de verdad: el token debe tener 64
   caracteres y el resumen de `preview` debe decir `gusset_plates: 1, knife_plates: 1, bolts: 4, weld_lines: 6,
   members_modified: 3` (como en la Fase 3). Si la 7 u 8 fallan, el modelo abierto no es la copia `_sondeo`.
4. Paso 4-7: `19/19` y `herramientas=61 conn_*=13`. Si la 18 falla por "no se pudo conectar", el puerto 8000 estaba ocupado o
   el `.venv` no tiene `mcp` 2.x: el log del puente lo dice.
5. Paso 4-9: en tu cliente de IA deben aparecer las 13 herramientas `conn_*` y, al pedirle `conn_ping` + `conn_get_guide`,
   debe resumir la guía nueva (si resume la antigua de 9 líneas, el add-in no se volvió a desplegar).

## 4. Decisiones tomadas y por qué

- **Siempre HTTP 200 con el sobre común, también sin documento (`NO_DOCUMENT`) en vez del 503 que reserva el encargo.**
  `revit_post` de `main.py` solo devuelve el `dict` con 200; con 503 la IA recibiría el texto `Error: 503 - ...` y perdería
  código, ruta, mensaje y pista. Queda documentado en `CONTRATO-conn.md`. 401 y 500 (adaptador) se conservan.
- **Parámetro de ruta `<connection_type>` en vez de `<type>`**: pyRevit lo pasa como argumento con ese nombre y `type` es un
  nombre reservado de Python. La URL es la misma que dice el encargo (`/conn/schema/gusset_node`).
- **Trece funciones explícitas, no generadas en bucle**: `requiere_token` lee los nombres de los argumentos con `getargspec`
  y los regenera con `exec`; con funciones explícitas se ve de un vistazo qué recibe cada ruta y no hay sorpresas en IronPython.
- **`spec` como objeto o como texto JSON** en `conn_validate`, `conn_preview`, `conn_create` y `conn_update`: algunos clientes
  de IA envían los objetos grandes como cadena; el puente lo entiende y, si no es JSON, responde `INVALID_REQUEST` sin molestar
  a Revit. Lo mismo para `connection_id`, `query` y `validation_token` vacíos.
- **Tiempos de espera**: 15 s `ping`; 60 s lecturas y `validate` (en el PC tardó 0,1 s); 180 s `preview`, `create`, `update`
  y `delete` (el encargo fija 180 para las tres primeras; `delete` también abre la sesión de acero, así que va igual).
- **Se mantienen `/conn/op/<operation>/` y `/conn/dev_exec/`** (pregunta 2 de la Fase 1, sin respuesta): las usan
  `conn-call.ps1`, `revit-exec.ps1 -SinTransaccion` y los sondeos 11-13 del instalador. No tienen herramienta MCP, así que
  la IA no las ve, y no dan más poder que `/execute_code/` (mismo token). Si prefieres retirarlas en la Fase 5, es borrar
  dos funciones.
- **El simulador se queda en el repositorio** (`mcp/pruebas/simulador_revit.py`): es lo que permite a la Fase 5 y a cualquier
  corrección futura probar el lado MCP en la nube sin esperar al instalador. Está marcado como "solo nube" y nada lo copia
  a la extensión.
- **No se toca el C#**: la fase no lo pide y las 13 operaciones ya respondían en el PC. Dos cosas que vi y dejo anotadas
  abajo (caducidad del token y operaciones `probe_*`).

## 5. Pendientes, riesgos y preguntas

### Pendientes y riesgos

- **R1. IronPython 2.7**: lo nuevo de `conexiones.py` solo se ha ejecutado en CPython 3 con módulos simulados. Riesgo bajo
  (mismas construcciones que la Fase 1), pero el paso 4-5 lo decide.
- **R2. El puerto 8000 del puente**: la prueba `--puente` necesita un `main.py --streamable-http` propio; si el cliente de IA
  ya tiene uno en marcha en modo HTTP, choca. Las instrucciones lo prevén (cerrar el cliente antes, reabrirlo al final).
- **R3. Caducidad del `validation_token` a los 30 minutos (sección 5.4 del encargo): no está implementada en el add-in**
  (lo comprobé en `ValidationTokenGenerator` y `SpecValidator`: el token es un SHA-256 determinista sin fecha). Hoy el token
  deja de valer si cambia la especificación o el modelo, que es la protección importante. Las docstrings y la guía no
  prometen los 30 minutos. Propongo decidirlo en la Fase 5 (añadir una marca de tiempo al token obliga a tocar Core y
  sus pruebas).
- **R4. Las operaciones `probe_plate_b` y `probe_delete_b` de la Fase 1 siguen registradas en `Bridge`** (se ven en
  `operations` de `conn_ping`). No tienen ruta con nombre ni herramienta, pero son alcanzables por `/conn/op/`. Propongo
  retirarlas en la Fase 5 junto con la decisión sobre `dev_exec`.
- **R5. `conn_get_schema` de un tipo inexistente responde `UNKNOWN_OPERATION`** (código elegido por `SchemaOperation` en la
  Fase 3); sería más claro un `UNKNOWN_CONNECTION_TYPE`. Cambio pequeño en C# para la Fase 5 si lo quieres.
- **R6. Tamaño de las respuestas**: `conn_get_schema` devuelve el esquema completo más el ejemplo (unos 10 KB) y
  `conn_get_node_info` unos 2 KB por nudo. Es lo que pide el encargo; si tu cliente de IA se queja del tamaño, se puede
  añadir un argumento `only_example`.
- **Windows PowerShell 5.1** no probado en la nube (como en todas las fases).

### Preguntas para ti

1. ¿Quieres que la Fase 5 implemente la caducidad de 30 minutos del token (R3) o te basta con la invalidación por cambio?
2. ¿Retiro en la Fase 5 las rutas de desarrollo (`/conn/op/`, `/conn/dev_exec/`) y las operaciones `probe_*` (R4), o
   prefieres conservarlas para los sondeos del instalador?
3. ¿Con qué cliente de IA vas a hacer la prueba de punta a punta de la Fase 5 (Claude Desktop, Claude Code en el PC...)?
   Me sirve para escribir el guion con la configuración exacta del servidor MCP.

## 6. Primera ronda en el PC (2026-09-30, `docs/fases/resultados-fase-4.md`) y corrección

**Lo que funcionó**: `deploy.ps1` e `instalar-conn.ps1` (`15 rutas @api.route`, `13 herramientas @mcp.tool`); pyRevit
registró las rutas nuevas en IronPython 2.7 y los parámetros de ruta llegan por nombre (`/conn/schema/gusset_node`,
`/conn/schema/no_existe`, `/conn/get/<id>` respondieron lo esperado); `ping`, guía nueva (8855 caracteres), tipos, esquema,
`find_profile`, `node_info` del nudo real, `list`, `get`, `delete` de un ID inexistente y `op/no_existe`: **12/17**. El
puente real del PC cargó `conn_tools.py` (`herramientas=79 conn_*=13`, el PC tiene más herramientas que el clon público)
y `tools/call conn_ping` devolvió `ok:true`: **14/19**. Sin ventanas ni cierres de Revit. Las dos pruebas del puente pasan.

**Lo que falló**: las cinco pruebas que envían una especificación (9 a 13: `validate` ×3, `preview`, `create`) con el
mismo error, generado por el **adaptador** (`addin_version: null`, no hay línea en el registro del add-in):

```text
INVALID_REQUEST: No se pudo serializar la petición: 'unknown' codec can't decode byte 0xe1 in position 28:
Unable to translate bytes [E1] at index 28 from specified code page to Unicode.
```

Causa: `llamar_bridge` hacía `json.dumps(data)` (con `ensure_ascii=True`, el valor por defecto). En IronPython 2.7 el
codificador JSON puro de Python llama a `s.decode('utf-8')` sobre cualquier texto con caracteres > 127, y pyRevit ya había
decodificado bien el cuerpo HTTP: el carácter 28 de `"La etiqueta del montante está cortada en la imagen"` (`uncertain_fields`
del fixture) es `á` (U+00E1), que al "decodificar" se trata como el byte `E1` suelto, inválido en UTF-8. En la Fase 3 el
sondeo 11 no lo sufrió porque leía el fixture del disco con la codificación por defecto de IronPython (los dos bytes UTF-8
de `á` llegaban como dos caracteres y el truco de `.decode` los recomponía por casualidad).

Corrección (este commit, solo `mcp/revit_mcp/conexiones.py`): `_a_json` serializa con `json.dumps(..., ensure_ascii=False)`
(ese camino no decodifica nada: el texto llega a C# como `System.String` y `System.Text.Json` lo lee tal cual) y, si aun así
lanzara, cae a `_json_ascii`, un serializador mínimo propio que escapa a mano los no ASCII como `\uXXXX` (con par sustituto
para los emoji). Probado en la nube con el simulador: autocomprobación **27/27** (incluye un cuerpo con `á`, `—`, `😀`,
comillas y barras que llega intacto al puente; `_json_ascii` da solo ASCII y `json.loads` devuelve el mismo objeto; y con
`json.dumps` forzado a fallar como en IronPython, `llamar_bridge` usa la reserva y el puente recibe lo mismo),
`probar_conexiones.py` **17/17** y las 23 llamadas de herramientas. **En IronPython es PENDIENTE DE INSTALADOR:
`docs/instalacion/fase-4b.md`** (ronda corta: reinstalar `conexiones.py` con Revit cerrado y repetir 4-6 y 4-7).

Dos detalles más de la primera ronda: el instalador tuvo que parar dos procesos `main.py` que ocupaban el puerto 8000 (el
puente del cliente de IA), así que al terminar la segunda ronda hay que reabrir ese cliente; y `data.example` del esquema
real trae un solo miembro (el simulador devolvía tres): sin efecto en las pruebas.

## 7. Segunda ronda (2026-09-30, `resultados-fase-4.md` "Segunda ronda"): **Fase 4 terminada en el PC**

Con `conexiones.py` corregido (`ensure_ascii=False` presente en la copia instalada), sobre `HANGAR_PRUEBA_sondeo.rvt`,
sin ventanas ni cierres de Revit:

- `probar_conexiones.py`: **17/17**. Las cinco que fallaban pasan ahora contra el add-in real: `validate` del Detalle D
  confirmado → `is_valid: true`, token `22e4e1d7cb5f…` (el mismo que emitió el sondeo 11 en la Fase 3: el token es
  determinista), dos advertencias `ANGLE_DIFFERS_FROM_MODEL`; 420→402 → `DIMENSION_CHAIN_MISMATCH` ("suma 547.0 mm pero
  se esperaba 565.0 mm"); dudas sin confirmar → dos `UNRESOLVED_UNCERTAINTY` con el motivo acentuado intacto ("está
  cortada"); `preview` → cartela, placa cuchilla, 4 pernos, 6 soldaduras, 3 retiros; `create` sin token →
  `VALIDATION_TOKEN_INVALID` sin tocar el modelo (`list` sigue en 0).
- Con el puente real del PC (`main.py --streamable-http`, 79 herramientas, 13 `conn_*`): **19/19**.
- El registro del add-in muestra ahora `validate`, `preview` y `create` con esos mismos códigos y 8–103 ms por llamada.

Estado final de la Fase 4: las 15 rutas `/conn/` y las 13 herramientas `conn_*` instaladas y probadas de punta a punta
en el PC (HTTP → pyRevit Routes → IronPython → `Bridge.Handle` → add-in, y cliente JSON-RPC → `main.py` → Revit).
Queda **para la persona** el paso 4-9 (reabrir el cliente de IA y pedirle `conn_ping` + `conn_get_guide`), que es
también el arranque natural de la Fase 5, y las tres preguntas de la sección 5.

### 7.1 Cliente de IA: Antigravity (paso 4-9, primer intento)

Antigravity no lanza `main.py` como subproceso: su `mcp_config.json` (`%USERPROFILE%\.gemini\config\mcp_config.json`)
define el servidor `revit` como `{"serverUrl": "http://localhost:8000/mcp"}`, y el puente lo arranca a mano
`C:\IA\iniciar_servidor_revit.bat` (`uv run main.py --streamable-http` en la carpeta de la extensión). En el primer
intento no había nada en el puerto 8000 (el instalador paró el puente en la primera ronda) ni Revit abierto, y la
caché de herramientas de Antigravity (`%USERPROFILE%\.gemini\antigravity\mcp\revit`) era del 30/09 a las 09:29, anterior a
la instalación: 0 herramientas `conn_*` de 66. No es un fallo del MCP (el mismo `main.py --streamable-http` sirvió
79 herramientas con 13 `conn_*` en el paso 4-7). Secuencia correcta para Antigravity: abrir Revit con el modelo →
ejecutar `iniciar_servidor_revit.bat` y dejar la ventana abierta → recargar el servidor `revit` en Antigravity (o
reiniciarlo) para que vuelva a pedir `tools/list` → usar las herramientas. Esta es la configuración que usará el guion
de la Fase 5 (respuesta a la pregunta 3 de la sección 5: cliente Antigravity, transporte HTTP en el puerto 8000).

### 7.2 Paso 4-9 completado desde Antigravity (2026-10-01, `resultados-fase-4.md` "4-9 cliente de IA")

Con Revit abierto, el puente arrancado con `iniciar_servidor_revit.bat` y el servidor `revit` recargado en Antigravity:
79 herramientas, las 13 `conn_*` presentes; `conn_ping` por el cliente → `ok: true`, `addin_version 0.1.0`,
`backend advancesteel`, `document.title HANGAR_PRUEBA_sondeo`; `conn_get_guide` resumido correctamente (flujo,
lectura de planos, seguridad) y `conn_list_types` → `gusset_node`. Sin llamadas de escritura. **Fase 4 cerrada en todos
los frentes**: Core, add-in, rutas, herramientas y cliente de IA. Siguiente: Fase 5 (guion de punta a punta con
Antigravity y README).
