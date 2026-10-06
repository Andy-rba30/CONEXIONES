# Fase 9: crear por lotes (el plan se convierte en acero, nudo a nudo)

Fecha: 2026-10-06. Rama: `main`. Add-in **0.9.0** (Core con `Batch/BatchCreateRequest`, `BatchReport` y `BatchRunner`;
add-in con `Batch/BatchCreator` y las operaciones `batch_create` y `batch_delete`), adaptador 0.9.0 (25 rutas) y 23
herramientas `conn_*`. Prompt y alcance: `docs/prompts/fase-9.md`, escrito a partir de las secciones 3.5, 4 y 6 de
`docs/propuestas/catalogo-y-lotes.md`, de las decisiones de su sección 7.1 (P8, P9, P13), de la mejora C3 de
`docs/propuestas/flujo-intuitivo.md` y de lo que la Fase 8 dejó anotado (8.5, 8.7 P6, 10.2, 11.8, 12.1, 12.5, 12.7).

**Estado: programada y probada en la nube; NO PROBADA en Revit** (la última versión desplegada en el PC es la 0.8.5). Se
prueba con `docs/instalacion/fase-9.md`, que empieza por el sondeo 20 (grupos de transacción anidados con Advance Steel).

Decisiones de la persona, fijadas en el prompt: **P6**, los 14 nudos con cordón HSS4X4 **se crean** con la cartela del
Detalle D y el aviso `TEMPLATE_PROFILE_DIFFERS`; los 10 nudos del cordón superior sin plantilla y los 7 empalmes **quedan
fuera del lote** (un `no_match` no se crea). Las cartelas fantasma (V2) solo si cabían sin recortar lo anterior: **no
entraron** (sección 4.7).

---

## 1. Qué se hizo

- **Core, `Batch/BatchCreateRequest.cs`**: la petición de `conn_batch_create`: `plan_id` y `nodes[{node,
  validation_token, spec?}]` tal como salieron de `conn_batch_plan`, más `stop_on_error` (por defecto `false`, P9) e
  `include_specs`. Cualquier cosa que falte o sobre (sin `plan_id`, sin `nodes`, un nudo sin token, un nombre suelto, una
  clave desconocida) es `INVALID_REQUEST` **antes de tocar el modelo**. `ForPlan(plan)` construye la petición que hace el
  botón Crear: todos los nudos creables (`ready` y `failed`) con su token.
- **Core, `Batch/BatchReport.cs`**: el informe por nudo (`outcome` `created`, `created_with_warnings`, `updated`,
  `failed`, `skipped`, `rolled_back`; al borrar, `deleted`), con `connection_id`, elementos, duración, motivo en español,
  errores y avisos; cuentas, `connection_ids`, `stop_on_error`, `stopped`, `undo_entries` (`one` | `per_node`) y
  `summary_text` ("Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: FABRICATION_FAILED: …), 2 saltadas.
  Una sola entrada de deshacer (Ctrl+Z). 48,2 s."). JSON de ida y vuelta; el plan lo guarda en `last_report`.
- **Core, `Batch/BatchRunner.cs`**: qué nudos se crean y en qué orden, sin Revit. Por cada nudo pedido: existe en el plan,
  no está repetido, su estado permite crearlo (`ready` o `failed`; `created` → "ya creada en este lote"; el resto →
  "no está listo: ✖ … · consejo"), el token es **el del plan** (si no, `VALIDATION_TOKEN_INVALID` en ese nudo, sin llamar
  al creador; con `spec` propia el token lo comprueba el creador contra el modelo), y después llama al **creador** (un
  delegado: en Revit `BatchCreator`, en las pruebas una imitación). Éxito → el nudo pasa a `created` con
  `created_connection_id`; fallo → `failed` con el error (una `CatalogException` conserva su código; cualquier otra es
  `FABRICATION_FAILED`) y conserva especificación y token para reintentar. Con `stop_on_error`, el primer fallo detiene el
  lote: con grupo exterior los creados en esa pasada vuelven a `ready` (`rolled_back`; el exterior lo revierte quien
  llama); sin grupo exterior (plan B) se quedan y el informe lo dice; los siguientes salen `skipped` "no se intentó".
  `ApplyDeletion` devuelve a `ready` los nudos cuya conexión borró `batch_delete`.
- **Core, estados y textos**: `NodeStatus.Created` y `NodeStatus.Failed`; `PlanNode.CreatedConnectionId` y
  `ExistingBatchId`; `BatchPlan.LastReport`, `CreatableCount`, `CreatedCount`, `FailedCount`, `CreatedInBatchCount`,
  `HasBatchConnections`. `PlanAdvice`: `✔ Creada` / `✔ Creada con aviso` (verde / ámbar), `✖ Falló al crear` (rojo, con
  "Falló al crear (CÓDIGO: motivo): corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye"), `◌ Ya creada
  en este lote` (tras replanificar con el lote creado: "Creada en este lote (conexión …): Borrar el lote la quita; para
  rehacerla, Incluir (rehacer)"), **`✖ Empalme del cordón`** (sección 4.4) y la cabecera con lo creado ("Creadas 15
  conexiones (14 con aviso), 1 falló. Quedan 0 listas sin crear." / "Nada que crear: 16 conexiones ya creadas en este lote
  (Borrar el lote las quita)."). `CreateButtonText` ("Crear 16 conexiones", "Crear 1 conexión (1 reintento)"). La leyenda
  pasa a "Verde = se creará (o creada) · Ámbar = con aviso · Rojo = falta algo o falló · Gris = no se crea".
- **Core, botones de *Qué hacer* (C3)**: `PlanAdvice.Actions(nudo, plan)` devuelve uno o dos `PlanAction` {`key`,
  `label`} por caso: `ready` con aviso → Excluir (y Editar nudo si el aviso es de ángulo); `invalid` → Editar nudo,
  Excluir; `no_match` con cordón → Excluir, Plantilla…; plantilla fijada o sin plantilla por decisión → Plantilla…;
  catálogo vacío → Abrir catálogo; falta el cordón → Cordón…, Barras…; empalme → Excluir; `ambiguous_chord` → Cordón…;
  `offset` → Ver en Revit, Excluir; `already_connected` → Incluir (rehacer) (y Ver en Revit si es de este lote);
  `excluded` → Incluir; `created` → Ver en Revit; `failed` → Ver en Revit, Excluir. La respuesta del MCP los lleva en
  `actions`; la ventana los pinta debajo de la frase.
- **Core, "ya conectado" bien definido**: `PlanRequest.ExistingConnections` (lista de `ExistingConnection` con
  `connection_id`, `batch_id`, cordón y barras) sustituye al mapa barra → conexión de la Fase 8. Un nudo está "ya
  conectado" solo si la conexión tiene **su cordón y alguna de sus barras** (`Covers`): antes, tras crear el lote y
  replanificar, los otros extremos de las diagonales creadas (extremos sueltos y parejas en K) también salían
  `already_connected` (56 nudos grises en vez de 16). Solo se aplica a los nudos detectados con cordón y barras.
- **Add-in, `Batch/BatchCreator.cs`**: `Create` abre un `TransactionGroup` exterior (`MotorConexiones: batch_create
  <plan_id>`) si `config\catalog.json` lleva `batch_single_undo: true` (nuevo, por defecto; `false` = plan B de la
  propuesta 4.1, una entrada de deshacer por nudo, sin recompilar; si el exterior no se puede abrir, aviso
  `BATCH_UNDO_SPLIT` y plan B), llama a `BatchRunner.Run` con un creador que hace por nudo **lo mismo que `conn_create`**:
  comprueba el token contra el modelo (`ValidationTokenGenerator` con `RevitModelFacts` y `limits.json`), abre su
  `OperationScope` (grupo anidado, `IFailuresPreprocessor`, diálogos cancelados), `CreateConnection` (o
  `UpdateConnection` con el mismo `connection_id` si el nudo rehace una existente), `AdoptNewElements` y `scope.Commit()`;
  si algo lanza, el ámbito revierte el grupo del nudo y el fallo sube al informe. Al final quita las marcas (color y
  marcador) de los nudos creados (`PlanMarks.RemoveNodes`, nuevo: las barras que siga usando otro nudo marcado conservan
  el color) y asimila el exterior; con `stop_on_error` y un fallo, lo revierte. Avisos `BATCH_NODE_FAILED` por nudo
  fallido. `DeleteBatch` busca en Extensible Storage las conexiones con `source.batch_id` = el lote y las borra una a una
  (`DeleteConnection`, una `Transaction` por conexión dentro de un solo `OperationScope`; la que falle se revierte sola y
  se anota), devuelve el informe y, si el plan sigue en memoria, `ApplyDeletion`. Sin conexiones: aviso `BATCH_EMPTY`.
  Eventos de log `batch_create`, `batch_node_created` / `batch_node_updated`, `batch_create_rolled_back`, `batch_delete`.
- **Add-in, operaciones**: `batch_create` (`BatchCreateOperation`: `PLAN_NOT_FOUND` si el plan no está en memoria;
  `BATCH_STOPPED` con `ok: false` y el informe en `data` cuando `stop_on_error` revierte) y `batch_delete`
  (`batch_id` o `plan_id`); `list` devuelve `batch_id` y `template_id` por conexión, `batches` {lote: cuántas},
  `total_count` y admite `batch_id` en el cuerpo. `Bridge` con 23 operaciones. `BatchPlanner`: `PlanToData` añade
  `creatable_count`, `created_count`, `failed_count`, `created_in_batch_count`, `has_batch_connections` y `last_report`;
  `NodeToData` añade `actions`, `existing_batch_id` y `created_connection_id`; `Plan` conserva `last_report` al
  replanificar. `TrussMap` etiqueta "Ya creada en este lote" con el plan.
- **Add-in, ventana del plan** (`UI/BatchPlanWindow`): título "MotorConexiones: conectar cercha (plan y lote)"; columna
  *Qué hacer* con la frase y los botones (`OnAdviceButton` enlaza cada clave con la acción del menú de clic derecho);
  botón **Crear N conexiones** (en negrita; texto de `CreateButtonText`; en gris explica por qué) con cuadro de
  confirmación (la cabecera del plan, qué pasa si un nudo falla, Ctrl+Z deshace el lote entero, puede tardar) y creación
  por el `ExternalEvent` ("⏳ Creando 16 conexión(es) en Revit (puede tardar varios minutos)…"); al terminar, cabecera,
  tabla, detalle (último lote por nudo) y barra de estado con el informe; **Borrar el lote** (botón cuando el plan tiene
  conexiones y en **Más…**) con confirmación, `DeleteBatch` y replanificación; `Incluir (rehacer)` replanifica con
  `replace_existing`. Log `ribbon_batch_create`, `ribbon_batch_delete`, `ribbon_batch_advice_action`. Los textos "Fase 9"
  de la 0.8.x desaparecen (ventana, previsualización, botón de la cinta).
- **MCP**: `mcp/revit_mcp/conexiones.py` (`POST /conn/batch/create/`, `POST /conn/batch/delete/`; 25 rutas),
  `mcp/tools/conn_tools.py` (`conn_batch_create` con `plan_id`, `nodes` (lista o texto JSON; el puente rechaza un nudo sin
  token), `stop_on_error`, `include_specs`, 1800 s de espera; `conn_batch_delete`; `conn_list` con `batch_id` filtrando en
  el puente; 23 herramientas), `mcp/pruebas/simulador_revit.py` (imitación de crear, saltar, token falso, `stop_on_error`,
  `list` por lote y borrar; 62 comprobaciones), `mcp/pruebas/probar_conexiones.py` (pruebas 24 y 25 nuevas: `batch_create`
  con un token **falso**, que no toca el modelo, y `batch_delete` de un lote inexistente; 28 pruebas, 30 con `--puente`),
  `mcp/CONTRATO-conn.md` (rutas, claves, códigos, deshacer, pruebas).
- **Sondeo 20** (`scripts/sondeos/20-grupos-anidados.py`): grupo exterior + `validate` + `create` del Detalle D por
  `Bridge.Handle` (grupo anidado del add-in) + `delete` + `Assimilate`; y grupo exterior + `create` + `RollBack`. Comprueba
  `list` = 0, acero suelto y extensiones como antes, e imprime "GRUPOS ANIDADOS OK" o "CON PROBLEMAS" (plan B).
- **Documentación**: `docs/prompts/fase-9.md`, `docs/instalacion/fase-9.md` (con lo pendiente de la Fase 8: 8.5, 12.5 y el
  sondeo 19 v4), `docs/guide.md` (sección 6 "Crear el lote" y "Borrar el lote"; sección 7), README (estado, tabla de
  garantías, árbol, instalación, sección 12 "Planificar y crear un lote", 13 y 14), `CLAUDE.md` (estructura y regla de
  transacciones), `docs/propuestas/flujo-intuitivo.md` (C3 hecho, V2 pendiente) y `catalogo-y-lotes.md` (estado),
  `scripts/conn-call.ps1` (23 operaciones), `config/catalog.json` (`batch_single_undo`), `docs/fases/resumen-fase-9.md`.
- **Versión 0.9.0** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador.
- **Pruebas**: `BatchCreateTests` (11) y `PlanActionsTests` (4). De 177 a **192**.

---

## 2. Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` y NuGet sirvió los paquetes de la API 2027: `MotorConexiones.Revit`
compila en la nube, así que los miembros usados existen (`TransactionGroup.Start/Assimilate/RollBack/HasStarted/HasEnded`,
`Transaction.RollBack`, todo lo demás ya se usaba). Que un `TransactionGroup` del add-in pueda abrirse **dentro** de otro
con la sesión de Advance Steel en medio solo lo prueba el sondeo 20 en el PC.

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build      → Passed! Failed: 0, Passed: 192, Total: 192
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar     → Autocomprobación: 62/62 correctas (25 rutas registradas)
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 28/28 pruebas correctas (addin_version 0.9.0)
```

| Prueba nueva | Qué comprueba |
|---|---|
| `Request_ParsesNodesWithTokensAndRejectsWhatIsMissing` | `plan_id`, `nodes` con token y `spec` opcional, `stop_on_error`; `INVALID_REQUEST` con su `path` sin `plan_id`, sin `nodes`, con `nodes` vacío, con un nombre suelto ("N4"), con un nudo sin token (`nodes[1].validation_token`), sin `node` y con una clave desconocida |
| `Request_ForPlan_TakesTheCreatableNodesWithTheirTokens` | La petición del botón sobre la cercha de la 8b: los 16 listos con su token; "Crear 16 conexiones" |
| `Run_CreatesTheSixteenReadyNodesOfTheHangarTruss8b` | 16 creadas (14 con el aviso de perfil → `created_with_warnings`), 16 `connection_ids` distintos, `summary_text` "Lote …: 16 conexiones creadas (16 con aviso). Una sola entrada de deshacer (Ctrl+Z).", el plan con 16 `created`, N4 `✔ Creada con aviso` ámbar con su consejo y el botón Ver en Revit, el nudo del tramo HSS3X3 `✔ Creada` verde, cabecera "Creadas 16 conexiones (14 con aviso). Ocultos: …", "Crear 0 conexiones" |
| `Run_AFailedNodeIsRolledBackAloneAndTheRestAreCreated` | El creador falla en N7: 15 creadas, N7 `failed` con `FABRICATION_FAILED` y el mensaje, conserva token y especificación, `✖ Falló al crear` rojo, consejo "Falló al crear (…): corrige y pulsa Crear otra vez…", botones Ver en Revit y Excluir, "Crear 1 conexión (1 reintento)", cabecera "Creadas 15 conexiones (13 con aviso), 1 falló. Pulsa Crear otra vez…"; el reintento solo crea N7 y deja 16 |
| `Run_StopOnError_RevertsEverythingAndSkipsTheRest` | Falla el tercero con `stop_on_error`: 3 llamadas, `stopped`, 2 `rolled_back`, 1 `failed`, 13 `skipped` "no se intentó", sin `connection_ids`; el plan vuelve a 15 listos y el fallido `failed` |
| `Run_StopOnError_WithoutOuterGroup_KeepsWhatWasCreatedBefore` | Plan B (`per_node`): lo creado antes del fallo se queda (2 creadas, 0 revertidas) y el informe lo dice |
| `Run_SkipsWhatCannotBeCreatedAndRejectsATokenThatIsNotThePlansOne` | Token de otro nudo → `failed` `VALIDATION_TOKEN_INVALID` sin llamar al creador y consejo "replanifica…"; pareja sin cordón y barra suelta → `skipped` "no está listo: …"; nombre inexistente → `skipped` con `INVALID_REQUEST`; repetido; el ya creado → "ya creada en este lote"; con `spec` propia no se exige el token del plan |
| `Run_ReplaceExisting_ReportsUpdatedAndACatalogExceptionKeepsItsCode` | Nudo con `replace_existing` → `updated`, "1 rehecha", "Rehecha en este lote"; una `CatalogException` del creador conserva su código en el informe y en el nudo (`batch:validation_token`) |
| `Report_RoundTripsThroughJsonInsideThePlan` | El informe y el plan (con `last_report`, `created` y `failed`) ida y vuelta por JSON |
| `ApplyDeletion_ReturnsTheCreatedNodesToReadyAndKeepsTheDeleteReport` | 10 de 16 borradas → 10 listos con su token y 6 creados; informe de borrado con 90 elementos y 30 barras; "Crear 10 conexiones"; cabecera "Creadas 6 conexiones… Quedan 10 listas sin crear." |
| `Replan_AfterTheBatch_SaysAlreadyCreatedInThisBatch` | Replanificar con las 16 conexiones en el modelo: 16 `already_connected` con `existing_batch_id` = plan, `◌ Ya creada en este lote`, consejo con Borrar el lote, botones Ver en Revit e Incluir (rehacer), cabecera "Nada que crear: 16 conexiones ya creadas en este lote…"; los extremos sueltos siguen `untyped` (23) y una conexión de otro lote sobre una pareja sale "Ya tiene conexión"; `ApplyDeletion` limpia las existentes |
| `Actions_FollowTheStateOfTheNode` | Los botones de cada estado y aviso (tabla de 3.5 del prompt) |
| `Splice_SaysTheChordEndsHereAndNeverSendsToReselect` | Con un tramo a 180°, 0° o 4,9° entre las barras: `IsSplice`, 2 diagonales, `✖ Empalme del cordón`, el consejo exacto, botón Excluir; sin tramo que siga, el caso de la Fase 8; cabecera "1 empalme del cordón (sin plantilla)" |
| `Splice_IsDetectedOnTwoChordSegmentsThatEndAtTheNode` | Geometría: dos tramos HSS12X8 que terminan en X = 5000 y dos diagonales desde abajo → el detector da un nudo de 4 barras sin cordón que atraviese, con un tramo como barra a 0° / 180°; el plan lo deja `no_match` con `NODE_CHORD_NOT_CONTINUOUS`, `IsSplice` y el consejo del empalme; **con el cordón fijado a mano al otro tramo**, lo mismo (lo que la 8e vio en N9) |
| `CreatedAndFailed_HaveTheirOwnTextsColorsAndHeader` | Textos, iconos, colores, `CanBeCreated`, `WhyNotCreatable`, "Crear 2 conexiones (1 reintento)", cabecera "Creadas 2 conexiones (1 con aviso), 1 falló. Quedan 1 lista sin crear.", `ApplyStatusColors` |

Las 177 pruebas anteriores siguen pasando: `AlreadyConnected_IsSkippedUnlessReplaceExisting` pasa a usar
`ExistingConnections` y comprueba que solo un nudo sale `already_connected`; el resto no cambia (`HangarTruss8b` sigue
dando 59 nudos y 16 `ready`).

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-9.md`, unos 75 minutos)

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.9.0.0` y copia `catalog.json` con `batch_single_undo: true`; `instalar-conn.ps1` 25 rutas y 23 herramientas; `ping` 0.9.0 con 23 operaciones | 9-1, 9-2 |
| **Sondeo 20**: `create` y `delete` del Detalle D dentro de un grupo exterior, `Assimilate` → `Committed`, `RollBack` → `RolledBack`, `list` = 0, acero suelto y extensiones como antes, "GRUPOS ANIDADOS OK"; una sola entrada en Deshacer. Si no: plan B `batch_single_undo: false` en el `catalog.json` desplegado | 9-2 |
| Lo pendiente de la Fase 8 (8.5): `end_gap_mm`, `overrides` devuelto tal cual, `PLAN_MARKS_REPLACED`; y de 12.5: Plantilla… cancelado, Editar nudo y Planificar lote con la ventana abierta, Ver en Revit sin cuadro (anotado), el cordón inferior, Descartar con un plan marcado en otra vista | 9-3, 9-4 |
| Los 7 empalmes del cordón superior como `✖ Empalme del cordón` con el consejo nuevo, también tras Cordón… al otro tramo (N9) | 9-3, 9-4 |
| Botones de *Qué hacer* en la tabla (Excluir / Incluir, Plantilla…, Cordón…) | 9-4 |
| **Crear 16 conexiones** desde la ventana sobre la copia: confirmación, "⏳", informe (cabecera, filas `✔ Creada…`, barra de estado), marcas de los creados fuera, tiempo; capturas `fase9-01` a `fase9-03`; `list` por lote = 16; `batch_plan_get` con `created_count: 16` y `last_report` | 9-4 |
| **Ctrl+Z** una sola vez deshace el lote entero (sondeos 12 y 13 a cero); Replanificar, crear otra vez, **Borrar el lote** (sondeos 12 y 13 a cero, barras restauradas; captura `fase9-04`) | 9-4 |
| Por el puente: `batch_create` de dos nudos (`created_with_warnings`, `undo_entries: one`), `list` por lote, saltados la segunda vez, token alterado → `failed` sin cambios, sin token → `INVALID_REQUEST`, `batch_plan_get` con `created` y `failed`, `batch_delete` → 2 borradas, `BATCH_EMPTY` la segunda vez; una entrada de Deshacer por lote | 9-5 |
| Sondeo 19 v4 (`fase-8.md` 12.7): etiqueta B, clic sin cuadro, la A pasa a naranja; 19b al momento; captura `fase9-05` | 9-6 |
| `discard all`, sondeos 17, 12 y 13 a cero; `--puente` **30/30**; log con `batch_create`, `batch_node_created`, `batch_delete`, `ribbon_batch_create`, `ribbon_batch_advice_action` y sin errores de ventana | 9-7, 9-8 |

### 2.2 NO PROBADO en la nube y por qué

- **Grupos de transacción anidados con Advance Steel**: la API de Revit permite anidar `TransactionGroup` (compila), pero
  el add-in nunca ha abierto su grupo dentro de otro con la `FabricationTransaction` de Advance Steel en medio, ni se ha
  asimilado o revertido un grupo exterior con elementos de acero dentro. Es el riesgo 8 de la propuesta y lo primero que
  mira el instalador (sondeo 20); el plan B (`batch_single_undo: false`) no necesita recompilar.
- **Ctrl+Z del lote entero** y que las marcas de los nudos creados vuelvan con él (se quitan en la última transacción del
  mismo grupo): solo en Revit.
- **La ventana**: botones dentro de las celdas de la tabla (`ItemsControl` en un `DataGridTemplateColumn`), el botón Crear
  apagándose mientras Revit trabaja, el cuadro de confirmación, el informe en pantalla, Borrar el lote y la replanificación
  posterior. Compila (incluido el XAML); no hay WPF en la nube.
- **El tiempo de crear 16 nudos**: 16 sesiones de Advance Steel seguidas (en la Fase 1 la primera tardó 132 s; en la 7,
  unos segundos cada una). La herramienta MCP espera 30 minutos; el instalador anota cuánto tardó.
- **`PlanMarks.RemoveNodes`** (quitar las marcas de unos nudos y dejar las demás): en la nube no hay vista.
- **El cordón inferior** (si existe como barra) y Descartar con un plan marcado en otra vista: pendientes de la 8d, en 9-4.
- **Lo de siempre**: pinchar en Revit, el puente real (en la nube solo contra el simulador).

---

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. **Bloque `9-2 sondeo 20`**: las dos partes con `ok=True`, `Assimilate() -> Committed`, `RollBack() -> RolledBack`,
   `conexiones=0` al final y la línea `6) RESULTADO: GRUPOS ANIDADOS OK`. Si dice "CON PROBLEMAS", el instalador habrá
   puesto el plan B: el lote funciona igual, pero con una entrada de deshacer por nudo (`undo_entries: per_node`); dímelo.
2. **Captura `fase9-01-ventana-lote.png`**: la tabla con los botones debajo de cada consejo, los 7 empalmes como
   `✖ Empalme del cordón` con "El cordón termina en este nudo (empalme)…" y el botón **Crear 16 conexiones** encendido.
3. **Capturas `fase9-02` y `fase9-03`**: la cercha con las 16 cartelas puestas y **sin** color ni cubo en esos nudos (los
   rojos y grises siguen marcados); la ventana con "Creadas 16 conexiones (14 con aviso)" y las filas `✔`. Si alguna falló,
   su fila dice por qué y el botón pasa a "Crear 1 conexión (1 reintento)": anota el motivo.
4. **Tu anotación de Ctrl+Z**: con un solo Ctrl+Z deben desaparecer las 16 a la vez y volver sus marcas; el menú Deshacer
   debe tener **una** entrada "MotorConexiones: batch_create …". Si hicieron falta 16, es el plan B.
5. **Bloques `9-4 sondeo 12 tras ctrl+z` y `tras borrar el lote`**: `conexiones en el modelo: 0` y
   `Elementos de acero sueltos encontrados: 0` las dos veces.
6. **Bloque `9-5`**: `created_count: 2` con `undo_entries: "one"`, la segunda llamada `skipped_count: 2`, el token alterado
   `failed_count: 1` sin cambios, `batch_delete` `deleted_count: 2` y `BATCH_EMPTY` después.
7. **Bloque `9-8`**: `30/30` y el log sin `ribbon_batch_window_error` ni `plan_event_failed`.

---

## 4. Decisiones tomadas y por qué

### 4.1 Un grupo por nudo anidado en el grupo del lote, con plan B sin recompilar

Es la opción (a) de P9 tal como la pidió la persona: el nudo que falla se revierte solo y los demás se quedan. El grupo
exterior asimilado da la "una sola entrada de deshacer" de la propuesta 3.5. Como la convivencia de los grupos anidados
con la sesión de Advance Steel no se ha probado nunca, `batch_single_undo` en `config/catalog.json` permite pasar al plan
B (una entrada por nudo) desde el PC, antes de crear el lote, sin tocar `src/`; el sondeo 20 lo decide y el informe lo
dice (`undo_entries`). Si el grupo exterior no se pudiera abrir en el momento, el add-in avisa (`BATCH_UNDO_SPLIT`) y
sigue en plan B por sí solo.

### 4.2 El token del plan, nudo a nudo, y otra vez contra el modelo

`conn_batch_create` exige el `validation_token` de cada nudo (sin él, `INVALID_REQUEST` antes de tocar nada; un nombre
suelto no basta). El Core comprueba que sea **el del plan** (si el plan se replanificó, los tokens cambian y el nudo falla
con `VALIDATION_TOKEN_INVALID` sin llamar a Revit) y el creador lo vuelve a calcular contra el modelo como `conn_create`
(barras, documento, `limits.json`). Con `spec` propia en la petición solo vale la segunda comprobación. Es la regla 5 del
encargo aplicada al lote.

### 4.3 Qué se crea y qué se salta: lo decide el Core

`BatchRunner` es puro Core con el creador como delegado: todo lo que no necesita Revit (orden, estados, tokens, informe,
`stop_on_error`, reintentos) se prueba en la nube con la cercha real de la 8b. Solo se crean `ready` (y `failed` para
reintentar); `invalid`, `no_match`, `excluded`, `untyped`, `already_connected` y `created` se saltan con su motivo, y la
decisión de la persona (los `no_match` del cordón superior y los empalmes quedan fuera; los "Listo con aviso" por perfil
entran) no necesita ninguna corrección: es lo que el plan ya decía.

### 4.4 El empalme se reconoce por la geometría, no por quién eligió el cordón

El consejo pedido ("El cordón termina en este nudo (empalme)…") vale tanto si el cordón lo fijó la persona con Cordón…
como si lo eligió el detector: lo que define el empalme es que ninguna barra atraviesa el nudo **y** una de sus barras
es casi paralela al cordón (±5°, `SpliceParallelDeg`): el otro tramo, que sale a 0° o a 180°. Con eso el estado es
`✖ Empalme del cordón` y los botones, solo Excluir (no hay plantilla de dos diagonales; crearla es trabajo de la persona
con el nudo a mano y Guardar en catálogo). Sin tramo que siga, el nudo sigue siendo "Falta el cordón" con el consejo y
los botones de la Fase 8 (Cordón…, Barras…).

### 4.5 "Ya conectado" = mismo cordón y alguna barra en común

El mapa barra → conexión de la Fase 8 marcaba como `already_connected` cualquier nudo que compartiera una barra con una
conexión; al replanificar con el lote creado, los otros extremos de las 48 diagonales (barras sueltas y parejas en K,
ocultos hasta entonces) salían como 40 nudos grises visibles. Ahora una conexión "cubre" un nudo solo si tiene su cordón y
alguna de sus barras, y solo se aplica a los nudos con cordón y barras. `conn_batch_plan` no cambia de forma; la
`ExistingConnection` lleva además el `batch_id`, que da "Ya creada en este lote".

### 4.6 Los botones de *Qué hacer* salen del Core y hacen lo mismo que el menú

`PlanAdvice.Actions` decide los botones (y la IA los recibe en `actions`); la ventana solo los pinta y enlaza cada clave
con el manejador del menú de clic derecho que ya existía. "Incluir (rehacer)" activa `replace_existing` para el plan
entero (es una corrección del plan, no por nudo): el texto de estado lo dice.

### 4.7 Las cartelas fantasma (V2) no entraron

El prompt las permitía solo si cabían sin recortar lo anterior. Habrían sido un `DirectShape` transparente por nudo listo
con el contorno de la cartela en el marco del nudo (el marco hay que recalcularlo del modelo al marcar), coloreado por
estado, que Crear sustituye y Descartar quita, y no se pueden probar en la nube. Con el lote, el informe, Borrar el lote,
C3, el empalme, el sondeo 20 y la instalación ya cubiertos, se dejan en `docs/propuestas/flujo-intuitivo.md` (V2) para la
Fase 10 o la 11.

### 4.8 `probar_conexiones.py` sigue sin crear nada

Las dos pruebas nuevas del puente usan un token **falso** (el nudo falla y el modelo no cambia) y un lote inexistente: el
script se puede seguir ejecutando sobre cualquier modelo. Crear de verdad se prueba a propósito, sobre la copia, con
Ctrl+Z y Borrar el lote (instalación 9-4 y 9-5).

---

## 5. Pendientes, riesgos y preguntas

- **P1 (riesgo principal): los grupos anidados con Advance Steel.** Sondeo 20. Si falla la parte A, plan B (una entrada
  de deshacer por nudo) sin recompilar; si falla la parte B (el `RollBack` del exterior deja acero suelto), `stop_on_error`
  no debe usarse y el sondeo 13 limpia. Pregunta: ¿qué dijo el sondeo 20?
- **P2: el tiempo del lote.** 16 sesiones de Advance Steel seguidas. Si tardara más de unos minutos, la herramienta MCP
  (30 min) aguanta, pero la ventana no enseña progreso nudo a nudo (solo "⏳ Creando…"); se puede añadir en el cierre si
  molesta.
- **P3: un nudo que falle en Revit** por una razón real (por ejemplo `PLATE_OUTSIDE_GUSSET` que el plan no vio porque
  `ValidationService` y la creación no coinciden): el informe lo dice y el nudo queda `failed` para reintentar o excluir.
  Pregunta: ¿falló alguno de los 16, y por qué?
- **P4: Ctrl+Z y las marcas.** Las marcas de los nudos creados se quitan en la última transacción del grupo del lote:
  Ctrl+Z las devuelve con el acero. Si Revit no las devolviera, Replanificar las recalcula.
- **P5: "Incluir (rehacer)" activa `replace_existing` para todo el plan**, no solo para el nudo del botón. Si se quiere por
  nudo hace falta una corrección nueva (`replace: ["N4"]`); se decide con el uso.
- **Pendientes anteriores que siguen**: V2 (cartelas fantasma), C6 y V3 (Fase 10; el sondeo 19 v4 se ejecuta en esta
  instalación), el botón Conectar, V4 y V5 (Fase 11), cartela automática (Fase 12), P3 y P4 de la Fase 8 (overrides previos
  de la persona, paleta), `conn_batch_update`, traspaso de `mcp/` a `revit-mcp`.

---

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-9.md` entero (empieza con `git pull` en `main`). El
   paso 9-2 corre el **sondeo 20** antes de nada: decide `batch_single_undo`.
2. Hacer los pasos marcados **(la persona)**: 9-3 (seleccionar la cercha de la 8c), 9-4 (botones de *Qué hacer*, el empalme,
   lo pendiente de 12.5, **Crear 16 conexiones**, **Ctrl+Z**, crear otra vez y **Borrar el lote**), 9-5.3 (el menú Deshacer),
   9-6 (sondeo 19 v4) y las cinco capturas.
3. **Mirar en Revit lo de la sección 3 de este informe** y anotar al final de `docs\fases\resultados-fase-9.md` lo que no
   coincida (sobre todo P1 y P3 de la sección 5).
4. **Abrir la sesión de cierre** con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-9.md (secciones 5 y 6) y docs/fases/resultados-fase-9.md. Cierra la Fase 9: contrasta
   los resultados con lo esperado en 2.1 (sondeo 20, Crear 16 conexiones, Ctrl+Z, Borrar el lote, el puente), corrige lo
   que haga falta (ronda 9b solo si es imprescindible), actualiza el informe y la tabla de garantías del README. No
   empieces la Fase 10. Termina con commit, push y un resumen corto.
   ```

5. **Solo con la 9 cerrada**, la Fase 10 (C6 selección asistida, V3 etiquetas pinchables con lo que diga el sondeo 19 v4, y
   V2 si se quiere) con un prompt nuevo en `docs/prompts/fase-10.md`, escrito a partir de `docs/propuestas/flujo-intuitivo.md`.
