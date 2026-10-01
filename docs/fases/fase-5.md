# Fase 5: Punta a punta y documentación

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt`. Add-in 0.1.0 (Core con el hash de `config/limits.json` en el
`validation_token`; operaciones `probe_*` retiradas); adaptador y herramientas 0.4.0 sin cambios.

**Estado: pendiente de la prueba de punta a punta en el PC.** El add-in de esta fase todavía **no se ha desplegado en
Revit**: lo probado hasta ahora en el PC (sección 2.2) se hizo contra la DLL de la Fase 4. La fase se cierra cuando lleguen
los resultados de `docs/instalacion/fase-5.md` (partes A y B) en `docs/fases/resultados-fase-5.md`.

---

## 1. Qué se hizo

### 1.1 En el PC (Antigravity, commit `b93df66`)

- **Retiradas las operaciones de prueba `probe_plate_b` y `probe_delete_b`** (R4 de la Fase 4): fuera de `Bridge.cs` y
  borrados `Operations/ProbePlateBOperation.cs` y `ProbeDeleteBOperation.cs`. `conn_ping` debe listar ahora 13 operaciones.
- **`config/limits.json` entra en el `validation_token`** (respuesta a la pregunta 1 de la Fase 4, sección 4):
  `LimitsConfig.ComputeHash()` (SHA-256 determinista de tolerancias, factor de separación, distancias al borde y filetes
  mínimos), parámetro opcional `LimitsConfig? limits` en `ValidationTokenGenerator.GenerateToken` (añade `|LIMITS:<hash>`),
  `SpecValidator` lo pasa, y en el add-in `LimitsConfigLoader` centraliza la lectura de `config\limits.json` para que
  `validate`, `create` y `update` usen la misma configuración. Si se edita `limits.json` entre validar y crear, el token
  deja de valer (`VALIDATION_TOKEN_INVALID`).
- **Prueba nueva** `ValidationToken_IncludesLimitsHash_DeterministicAndChangesOnConfigDifference` (51 pruebas en total).
- Primera versión de `README.md`, de este informe y de `docs/instalacion/fase-5.md`.

### 1.2 En la nube (esta sesión)

- **Limpieza de restos `probe_*`**: `scripts/conn-call.ps1` (ayuda y ejemplos: `ping`, `node_info`, `schema`) y
  `scripts/sondeos/07-nudo-seleccion.py` (la orden final que imprime es ahora `conn-call.ps1 -Operation node_info`).
  Búsqueda en todo el repositorio: fuera de `docs/fases/` y de `docs/instalacion/` de fases anteriores no queda ninguna
  referencia (`PROBE_EXCEPTION` es el código de error de `/conn/dev_exec/` y se conserva).
- **`README.md` corregido**: esquema de Extensible Storage `MotorConexionesConnection` (el real de
  `Storage/ConnectionStorageManager.cs`); sección "agregar un tipo de conexión nuevo" con la interfaz real
  `IConnectionType` (`Name`, `Description`, `GetSchemaJson()`, `GetExampleJson()`), el patrón de `GussetNodeType.cs` y el
  registro en el constructor estático de `Bridge.cs`, diciendo con claridad qué partes de v1 son específicas de
  `gusset_node`; estructura del repositorio con los archivos reales (no existe `GussetNodeSpec`); Antigravity como
  configuración probada y Claude Desktop marcada **NO PROBADA**; sección nueva "Cómo probar sin Revit"; tabla de garantías
  con la referencia al archivo de resultados donde está probada cada una; `VALIDATION_TOKEN_INVALID` sin "caducidad".
- **Sondeo nuevo `scripts/sondeos/14-fase5-token-limits.py`**: por reflexión sobre `Bridge.Handle` (como el sondeo 11) y
  **sin crear nada**, comprueba que la DLL desplegada es la de esta fase (`ping` sin `probe_*`, Core con
  `LimitsConfig.ComputeHash`), que el `limits.json` desplegado tiene el mismo hash que el del repositorio, que `validate`
  del fixture confirmado da un token de 64 hex y que validar dos veces da el mismo, que `create` con el token alterado y
  con un token calculado con **otro** `limits.json` (tolerancia de cotas 2,5 mm, por reflexión sobre el Core desplegado)
  se rechaza con `VALIDATION_TOKEN_INVALID`, y que después `conn_list` y las extensiones de las barras siguen iguales.
- **Sondeo 12** ampliado: al final imprime siempre las extensiones actuales de las barras del fixture, también cuando ya
  no hay conexiones (para ver, tras el `conn_delete` de Antigravity, que quedaron restauradas).
- **`docs/instalacion/fase-5.md` reescrito** en el formato de las fases anteriores (`Set-ExecutionPolicy`, función
  `Anota` que guarda cada salida en `docs/fases/resultados-fase-5.md`, comandos literales, Revit cerrado antes de
  `deploy.ps1`, siempre la copia `_sondeo`), en dos partes: A para el instalador y B para la persona con Antigravity,
  con el prompt literal que exige "SÍ, CREA" y "SÍ, BORRA" y prohíbe `execute_revit_code` sobre lo creado.
- Este informe.

Sin cambios en `src/` en esta sesión (el C# compila con 0 avisos y pasa las 51 pruebas tal como lo dejó el commit
`b93df66`). En `mcp/` solo cambia una frase de la docstring de `conn_create` en `tools/conn_tools.py` (el token no
"caduca": deja de valer si cambian la especificación, el modelo o `limits.json`); A-3 lo reinstala. `/conn/op/` y
`/conn/dev_exec/` se conservan y no se toca nada de revit-mcp.

## 2. Qué se probó y dónde

### 2.1 Nube (esta sesión)

SDK .NET 10.0.112 (`apt`), Python 3.11, venv con `mcp 2.2.0` y `httpx 0.28.1`, clon de solo lectura de `revit-mcp`
(commit `8505508`).

```text
$ dotnet build MotorConexiones.sln -c Release
  MotorConexiones.Core  -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
Build succeeded.  0 Warning(s)  0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build
Passed!  - Failed: 0, Passed: 51, Skipped: 0, Total: 51, Duration: 237 ms

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py
(sin errores; incluye los sondeos 07, 12 y 14 modificados o nuevos)

$ python3 mcp/pruebas/simulador_revit.py --autocomprobar                       Autocomprobación: 27/27 correctas
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar --extension <clon>    Autocomprobación: 27/27 correctas (seguridad.py real)

$ python3 mcp/pruebas/simulador_revit.py &  ;  python3 mcp/pruebas/probar_conexiones.py
Resultado: 17/17 pruebas correctas

$ (clon + conn_tools.py registrado + venv mcp 2.2.0) python main.py --streamable-http  ;  probar_conexiones.py --puente
18. tools/list por el puente trae las 13 herramientas conn_*  [OK]  HTTP 200, herramientas=61 conn_*=13
19. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.1.0
Resultado: 19/19 pruebas correctas
```

Lo que esto prueba: el Core (incluido el hash de límites en el token), el adaptador, el script de pruebas, las 13
herramientas y el puente. **No prueba nada de lo que pasa dentro de Revit**: el simulador imita al add-in.

### 2.2 PC (Antigravity, 2026-10-01, commit `b93df66`, con la DLL de la Fase 4 todavía cargada)

Compilación y pruebas en el PC: `0 Advertencia(s)`, `0 Errores`, `Correctas! ... Superado: 51`. Y
`probar_conexiones.py --puente` contra Revit 2027 y el puente real: `Resultado: 19/19 pruebas correctas`
(`herramientas=79 conn_*=13`). **Esa ejecución no prueba el add-in de esta fase**: `deploy.ps1` no se había vuelto a
ejecutar, así que Revit respondía con la DLL de la Fase 4 (la que aún lista `probe_plate_b` y `probe_delete_b`). Sirve como
prueba de que el entorno del PC sigue en orden tras los cambios del MCP, nada más.

De la Fase 4 queda probado desde Antigravity (`resultados-fase-4.md`, "4-9"): 79 herramientas con las 13 `conn_*`,
`conn_ping`, `conn_get_guide` y `conn_list_types` por el cliente. Sin llamadas de escritura.

### 2.3 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-5.md`)

| Qué | Paso |
|---|---|
| Despliegue de la DLL nueva con Revit cerrado y reinstalación de los archivos del MCP | A-3 |
| `conn_ping` con 13 operaciones y **sin** `probe_*` (es la prueba de que la DLL nueva está cargada) | A-4 |
| `probar_conexiones.py --puente` 19/19 contra la DLL nueva | A-6 |
| Token ligado a `limits.json` en Revit: mismo hash desplegado y del repositorio, token determinista, `create` rechazado con token alterado y con token de otro `limits.json`, modelo intacto (sondeo 14) | A-7 |
| Registro del add-in sin ningún `create` con `ok:true` en la parte A | A-8 |
| **Prueba de punta a punta desde Antigravity**: `conn_ping` → guía → tipos → `node_info` → esquema → `validate` → `preview` → confirmación → `create` (9 elementos) → `list` → `get` → confirmación → `delete` → `list` = 0 | B-1 |
| Pendiente de la Fase 3 (sección 8 de `fase-3.md`): ver **a ojo** que las placas y los pernos de Advance Steel aparecen en pantalla tras `create` (la captura exportada no los mostraba) | B-2 |
| Extensiones de las barras restauradas tras el `conn_delete` de Antigravity (sondeo 12) | B-3 |

### 2.4 NO PROBADO

- **Configuración de Claude Desktop** del README: escrita a partir de la documentación general de MCP (stdio con `uv`);
  no hay Claude Desktop en el PC de prueba ni en la nube. Marcada NO PROBADA en el README.
- **Sondeo 14 en IronPython 2.7**: solo compilado con CPython 3 (`py_compile`). Usa las mismas construcciones que los
  sondeos 11 y 12 (que corrieron en el PC) más tres llamadas por reflexión al Core (`LimitsConfig.LoadFromFile`,
  `LoadFromJson`, `ComputeHash`, `ConnectionSpec.FromJson`, `ValidationTokenGenerator.GenerateToken` y el constructor de
  `RevitModelFacts` con dos argumentos opcionales nulos). Si esa parte fallara, el sondeo lo imprime como `[NO PROBADO]`
  y sigue; las comprobaciones de `create` con el token alterado y del modelo intacto no dependen de ella.
- **Las órdenes nuevas de PowerShell** de A-5 (`Get-CimInstance Win32_Process`, `Get-NetTCPConnection`,
  `Start-Process` del `.bat`): no hay Windows en la nube. Son cmdlets estándar de Windows PowerShell 5.1 y 7.

## 3. Qué debo mirar yo cuando el instalador termine

1. **A-4**: en `operations` solo 13 nombres; si aparece `probe_plate_b` o `probe_delete_b`, Revit sigue con la DLL vieja
   (A-3 se hizo con Revit abierto).
2. **A-6**: `Resultado: 19/19` y `conn_*=13`.
3. **A-7**: `RESULTADO: 14/14`. Las líneas que importan: `hash desplegado=` y `hash repositorio=` iguales; `token 1` y
   `token 2` iguales; `token con otro limits.json` distinto; dos `create ... -> VALIDATION_TOKEN_INVALID` en `[OK]`;
   `conn_list igual que antes`. Un `[NO PROBADO]` en la parte de reflexión no es un fallo del add-in.
4. **B-1, en Antigravity**: `conn_validate` con `is_valid: true` y token de 64 caracteres (debe coincidir con el `token 1`
   de A-7: el token es determinista); `conn_preview` con `bolts: 4` y `members_modified: 3`; `conn_create` con 9 IDs y
   `backend: advancesteel` en 1 a 3 segundos (la primera vez puede tardar hasta 3 minutos); `conn_list` 1 y luego 0.
5. **B-2, en Revit 3D**: cartela, placa cuchilla con 4 pernos en la diagonal inferior (1249636), 6 soldaduras y tres
   barras acortadas. **Lo que decide el pendiente de la Fase 3**: que las placas y los pernos (elementos de Advance Steel)
   se vean en pantalla al cabo de unos segundos. Capturas `fase5-02-pantalla.png` (pantalla) y `fase5-01-conexion.png`
   (exportada). Si en pantalla no se ven, el siguiente paso es añadir a `conn_create` la llamada
   `SteelModelManager.RequestGraphicalUpdateForSteelElements` (vista en el sondeo 06) antes de devolver.
6. **B-3**: `conexiones tras borrar: 0` y extensiones `1249630: 0.0 | 68.64`, `1249631: 0.0 | 69.2`, `1249636: 0.0 | 0.0`,
   igual que tras los borrados de la Fase 3.

## 4. Decisiones tomadas y por qué

- **La fase no se declara cumplida hasta que lleguen los resultados.** El informe anterior daba el encargo por cumplido en
  su totalidad con un 19/19 hecho contra la DLL de la Fase 4 y sin haber ejecutado la prueba de punta a punta. Aquí queda
  separado lo probado en la nube, lo probado en el PC con la DLL anterior y lo pendiente.
- **Sin caducidad de 30 minutos del token, pero ligado a `limits.json`** (pregunta 1 de la Fase 4). Una caducidad con reloj
  obliga a revalidar en mitad de una conversación larga sin que nada haya cambiado; lo que protege de verdad es que ni la
  especificación, ni el documento, ni las barras, ni las reglas de validación cambien entre validar y crear, y el hash de
  `LimitsConfig` cubre lo último de forma determinista.
- **Se conservan `/conn/op/<operation>/` y `/conn/dev_exec/`** (pregunta 2 de la Fase 4): las usan `conn-call.ps1`,
  `revit-exec.ps1 -SinTransaccion` y los sondeos 11 a 14 del instalador; no tienen herramienta MCP y no dan más poder
  que `/execute_code/`. Las operaciones `probe_*` sí se retiran: eran código de sondeo de la Fase 1 alcanzable por
  `/conn/op/`.
- **Antigravity como cliente de la prueba de punta a punta** (pregunta 3 de la Fase 4): es el que ya funcionó en el paso
  4-9, con transporte HTTP en el puerto 8000 y el puente arrancado a mano con `C:\IA\iniciar_servidor_revit.bat`.
- **El sondeo 14 no crea nada**: alterar el token y calcular otro con otro `limits.json` basta para demostrar que
  `create` lo rechaza antes de tocar el modelo; crear desde el sondeo duplicaría la parte B y dejaría dos pruebas de
  escritura en la misma sesión.
- **Sondeo 12 ampliado en vez de un sondeo nuevo**: tras el `conn_delete` de Antigravity no queda nada que borrar, y lo que
  hace falta ver son las extensiones actuales de las barras.
- **Confirmaciones literales ("SÍ, CREA", "SÍ, BORRA")** en el prompt: una confirmación ambigua ("vale", "ok") podría
  interpretarse como permiso para el paso siguiente.
- **README con la referencia al archivo de resultados** por cada garantía, para que no vuelva a afirmarse como probado lo
  que no consta en `resultados-fase-3.md` o `resultados-fase-4.md`.

## 5. Pendientes, riesgos y preguntas

### Pendientes y riesgos

- **P1 (Fase 3, sección 8)**: confirmar a ojo que las placas y los pernos de Advance Steel se ven tras `create` (B-2). Si
  no, `RequestGraphicalUpdateForSteelElements` en `conn_create` (cambio pequeño en C#).
- **P2 (R5 de la Fase 4)**: `conn_get_schema` de un tipo inexistente sigue respondiendo `UNKNOWN_OPERATION`; un
  `UNKNOWN_CONNECTION_TYPE` sería más claro. No se ha tocado por no cambiar el C# en una fase de cierre.
- **P3**: la configuración de Claude Desktop del README está NO PROBADA.
- **P4**: la parte de reflexión del sondeo 14 (constructor de `RevitModelFacts` con `NodeFrame?` y `Vec3?` nulos y
  `GetMethod` con el tipo de la interfaz `IModelFacts`) no se ha ejecutado en IronPython; si falla, imprime `[NO PROBADO]`
  y el resto del sondeo sigue valiendo.
- **P5**: el puente arrancado por `iniciar_servidor_revit.bat` queda en una ventana abierta; si ya había otro `main.py`
  en el puerto 8000, A-5 lo cierra antes. Antigravity debe recargar el servidor `revit` después (B-0).
- **P6**: Antigravity debe poder leer `docs\fixtures\detalle-D-confirmado.json`; si no, se pega el JSON bajo el prompt
  (B-0). En cualquier caso la regla 4 del prompt prohíbe modificar la especificación.
- **P7**: `docs/fases/resultados-fase-5.md` no existe todavía; lo crea A-1 y lo completan A-9, B-3 y B-4.

### Preguntas para ti

1. Si en B-2 las placas y los pernos **no** se ven en pantalla, ¿quieres que la siguiente sesión añada
   `RequestGraphicalUpdateForSteelElements` a `conn_create`, o prefieres comprobar antes si basta con orbitar la vista?
2. ¿Cambio `UNKNOWN_OPERATION` por `UNKNOWN_CONNECTION_TYPE` en `conn_get_schema` (P2) en una sesión de cierre, con su
   prueba y la fila correspondiente en `CONTRATO-conn.md` y `guide.md`?
3. Cuando la Fase 5 quede cerrada, los archivos de `mcp/` pasan al repositorio `revit-mcp` (lo haces tú, según el
   encargo). ¿Quieres que la siguiente sesión prepare el texto de ese commit y la sección para pegar en `CONTRATO.md`?
