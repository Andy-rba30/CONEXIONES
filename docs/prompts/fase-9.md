# Fase 9: crear por lotes (el plan se convierte en acero, nudo a nudo)

Tercera y última de las fases que salen de `docs/propuestas/catalogo-y-lotes.md` (sección 6: Fase 7 catálogo, Fase 8
detección de nudos y plan, Fase 9 crear por lotes). Se ejecuta en **una sesión**, con el prompt de la sección 1. Lo demás
de este archivo es el alcance que esa sesión debe cumplir, escrito a partir de las secciones 3.5, 4 y 6 de la propuesta,
de las respuestas de su sección 7.1 (P8, P9, P13), de la mejora C3 de `docs/propuestas/flujo-intuitivo.md` (botones en la
columna *Qué hacer*), de lo que la Fase 8 dejó anotado para esta (`docs/fases/fase-8.md`: 8.5, 8.7 P6, 10.2, 11.8, 12.1,
12.5 y 12.7) y de las **dos decisiones de la persona** (sección 2).

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-8.md entero (sobre todo las secciones 8.5, 10.2, 11.8
y 12) y docs/propuestas/catalogo-y-lotes.md completo. Escribe docs/prompts/fase-9.md (crear por lotes: secciones 3.5,
4 y 6 de la propuesta, con las decisiones de 7.1) y ejecuta SOLO la Fase 9: conn_batch_create con el validation_token
del plan, botón "Crear N conexiones" en la ventana del plan, informe por nudo (creado, con aviso, fallido y por qué),
"Borrar el lote" (conn_batch_delete y botón), una operación = un TransactionGroup por nudo con rollback del nudo que
falle sin tirar los demás, y la columna "Qué hacer" con botones (mejora C3 de docs/propuestas/flujo-intuitivo.md),
incluido el consejo del empalme de la sección 12.1 (cuando el cordón se fijó a mano y no pasa de largo, decir "El cordón
termina en este nudo (empalme): ninguna plantilla encaja con dos diagonales; crea esa típica o excluye", nunca
"selecciónalo y replanifica"). Las cartelas fantasma (V2) solo si caben sin recortar lo anterior; si no, déjalas en
la propuesta.

DECISIONES:
- P6, los 14 nudos con cordón HSS4X4: se crean con la cartela del Detalle D y el aviso TEMPLATE_PROFILE_DIFFERS.
- Los 10 nudos del cordón superior sin plantilla y los 7 empalmes: quedan fuera del lote (no_match no se crea).

Incluye en docs/instalacion/fase-9.md, además de las pruebas de la Fase 9: las comprobaciones pendientes de la sección
8.5 (add-in 0.8.2), las de la 12.5 (Plantilla… cancelado, Editar nudo y Planificar lote con la ventana abierta, Ver en
Revit sin cuadro anotado, el cordón inferior, Descartar con un plan marcado en otra vista) y el sondeo 19 v4 con el
bloque de la 12.7. Crear el lote en el PC se prueba primero sobre una copia del modelo y con deshacer (Ctrl+Z) y
"Borrar el lote" comprobados con los sondeos 12 y 13 a cero. Sube la versión del add-in a 0.9.0 en AddinInfo, los csproj,
el adaptador, las herramientas y el simulador. Termina con compilación sin avisos, pruebas en verde (Core y simulador),
docs/fases/fase-9.md, docs/instalacion/fase-9.md, el README (estado, tabla de garantías, sección 12), docs/guide.md,
commit, push y un resumen corto guardado también en docs/fases/resumen-fase-9.md.
```

## 2. Decisiones de la persona (fijadas antes de empezar)

| Pregunta | Decisión | Qué hace la Fase 9 |
|---|---|---|
| **P6** (`fase-8.md` 8.7): los 14 nudos del cordón central cuyo tramo es HSS4X4 (la plantilla es HSS3X3) | **Se crean** con la cartela del Detalle D y el aviso `TEMPLATE_PROFILE_DIFFERS` | Un nudo `ready` con avisos es "Listo con aviso" y entra en el lote igual que uno verde. La columna *Qué hacer* lo dice y ofrece **Excluir** para quien no quiera. No se cambia `profile_policy`. |
| Los 10 nudos del cordón superior "sin plantilla que encaje" y los 7 empalmes (`fase-8.md` 10.2 y 12.1) | **Quedan fuera del lote** | Un `no_match` no se crea nunca; no hace falta excluirlos. El consejo de los empalmes cambia (sección 3.5) para no mandar a un callejón sin salida. |
| P9 (propuesta 7.1): si un nudo falla | Opción (a): los demás se quedan creados y el informe dice cuál falló y por qué | Un `TransactionGroup` por nudo dentro de un grupo exterior del lote; `stop_on_error: false` por defecto. |
| P8: nudo que ya tiene conexión | Se salta (`replace_existing: false`) | Con `replace_existing: true` el plan lo deja `ready (rehacer)` y el lote lo rehace con `conn_update` (mismo `connection_id`). |
| P13: `source.template_id` y `source.batch_id` en cada conexión | Ya lo escribe el plan (Fase 8) | `conn_list` enseña `batch_id` por conexión y filtra por lote; `conn_batch_delete` borra por `batch_id`. |
| V2 (cartelas fantasma) | Solo si cabe sin recortar lo anterior | Si no cabe, se queda en `docs/propuestas/flujo-intuitivo.md` y el informe lo dice. |

## 3. Alcance (lo que se entrega)

### 3.1 Flujo completo (propuesta 3.1, tercer paso)

```
1. PLANIFICAR   (Fase 8)  selección + plantillas → plan con N1, N2…, estado, especificación y token por nudo, marcas
2. REVISAR      (Fase 8)  correcciones → replanificar (mismo plan_id, mismos nombres)
3. CREAR        (Fase 9)  conn_batch_create / botón "Crear N conexiones": nudo a nudo, con los tokens del plan
                          → informe: creada / creada con aviso / fallida (por qué) / saltada (por qué)
                          → en Revit, una sola entrada de deshacer (Ctrl+Z deshace el lote entero)
                          → las marcas de los nudos creados se quitan (el acero las sustituye); las de los demás siguen
4. BORRAR       (Fase 9)  conn_batch_delete / botón "Borrar el lote": borra todas las conexiones del lote una a una con
                          las garantías de conn_delete (barras restauradas), una sola entrada de deshacer
```

### 3.2 `conn_batch_create` (propuesta 3.5)

- **Petición**: `plan_id` **y** `nodes: [{node, validation_token, spec?}]` tal como salieron de `conn_batch_plan` o de
  `conn_batch_plan_get`. Sin `validation_token` por nudo no se crea nada (`INVALID_REQUEST` antes de tocar el modelo).
  `spec` es opcional: si falta se usa la del plan; si se pasa, sustituye a la del plan solo para esa creación.
  `stop_on_error` (por defecto `false`): con `true`, el primer fallo revierte el lote entero (`BATCH_STOPPED`, `ok: false`,
  con el informe en `data`).
- **Comprobaciones por nudo**, en este orden: el nudo existe en el plan; su estado permite crearlo (`ready`, o `failed`
  de un intento anterior); el token es **el del plan** para ese nudo (si no, `VALIDATION_TOKEN_INVALID`: el plan se
  replanificó y hay que volver a pasar el token nuevo); y el token vuelve a comprobarse **contra el modelo** como hace
  `conn_create` (barras, documento, `limits.json`): si algo cambió, `VALIDATION_TOKEN_INVALID` y ese nudo falla. Un nudo
  `invalid`, `no_match`, `excluded`, `untyped`, `already_connected` o ya `created` se **salta** con su motivo en español.
- **Atomicidad (propuesta 4.1, P9)**: cada nudo es una operación completa (`OperationScope`: `TransactionGroup` +
  `IFailuresPreprocessor` + `DialogBoxShowing`, lo mismo que `conn_create`) **anidada** en un grupo exterior del lote
  (`MotorConexiones: batch_create <plan_id>`). El nudo que falla se revierte solo (su grupo hace rollback) y el lote sigue;
  al final el grupo exterior se asimila: **una sola entrada de deshacer**. Si `stop_on_error`, el exterior se revierte
  entero. Como los grupos anidados con la sesión de fabricación de Advance Steel no se han probado nunca, el **sondeo 20**
  (sección 3.7) lo comprueba en el PC antes de crear el lote; si no funcionara, `config/catalog.json` lleva la llave
  `batch_single_undo` (plan B de la propuesta 4.1: `false` = un grupo por nudo, N entradas de deshacer, sin recompilar).
- **Nudo que rehace una conexión** (`replace_existing`): `conn_update` con el mismo `connection_id`; en el informe sale
  `updated`.
- **Después de crear**: cada nudo creado pasa a estado `created` (con `created_connection_id`) y pierde sus marcas (color
  y marcador); un nudo fallido pasa a `failed` con el error y conserva su especificación y su token para reintentar con
  **Crear** otra vez (solo crea los que faltan). Cada conexión creada lleva `source.batch_id` = `plan_id` y
  `source.template_id` (P13). Si se replanifica después, los nudos creados salen `already_connected` con el texto "Ya
  creada en este lote".
- **Respuesta** (`data`): el informe (sección 3.4) y el resumen del plan después del lote.
- **Registro**: evento `batch_create` en el log JSON con el `batch_id`, las cuentas, la duración y el modo de deshacer.

### 3.3 `conn_batch_delete` y `conn_list` por lote (propuesta 3.5)

- `conn_batch_delete` recibe `batch_id` (el `plan_id` del plan que creó el lote; también se admite `plan_id`), busca en
  Extensible Storage todas las conexiones cuyo `source.batch_id` coincide y las borra **una a una** con las garantías de
  `conn_delete` (solo lo que creó el add-in; retiros de las barras restaurados), dentro de un solo grupo (**una entrada de
  deshacer**). Una conexión que no se pueda borrar se anota y se sigue con la siguiente. Si el plan sigue en memoria,
  sus nudos `created` vuelven a `ready` (con su token) y la ventana lo replanifica. Sin conexiones de ese lote:
  `ok: true`, `deleted_count: 0` y aviso `BATCH_EMPTY`. La confirmación la pide la IA al usuario (guía) o el botón.
- `conn_list` devuelve `batch_id` por conexión y `batches` (cuántas conexiones hay por lote); la herramienta admite
  `batch_id` para filtrar. `conn_get` ya devuelve la especificación completa con `source.batch_id`.

### 3.4 Informe por nudo (propuesta 3.5; Core, se prueba sin Revit)

`Core/Batch/BatchReport.cs`: por nudo `{node, outcome, connection_id, elements_count, duration_ms, reason, errors[],
warnings[]}` con `outcome` ∈ `created` (sin avisos), `created_with_warnings` (el nudo tenía avisos del plan, por ejemplo
`TEMPLATE_PROFILE_DIFFERS`, o Revit avisó al crear), `updated` (rehecha), `failed` (con `errors[]`: el código y el
mensaje en español), `skipped` (con `reason`: "no está listo: ✖ Sin plantilla que encaje", "ya creada en este lote",
"token distinto del plan"…), `rolled_back` (creada y revertida por `stop_on_error`). Cuentas (`created_count`,
`updated_count`, `failed_count`, `skipped_count`, `with_warnings_count`), `connection_ids[]`, `stop_on_error`, `stopped`,
`undo_entries` (`one` | `per_node`), `duration_ms` y `summary_text` en español ("Lote 4ef7dd3d: 15 conexiones creadas
(14 con aviso), 1 falló (N7: …), 2 saltadas. Una sola entrada de deshacer (Ctrl+Z)."). El mismo informe sirve para
`batch_delete` (`outcome` `deleted` | `failed`). El plan guarda el último informe (`last_report`) y la cabecera de la
ventana y `summary_text` del plan lo reflejan ("Creadas 15 conexiones (14 con aviso), 1 falló. Quedan 0 listas…").

### 3.5 Ventana del plan: Crear, Borrar y los botones de *Qué hacer* (mejora C3)

- **Crear N conexiones** (botón principal, en negrita; N = nudos `ready` + `failed`): cuadro de confirmación con la
  cabecera del plan ("Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil
  distinto.") y la explicación de qué pasa si un nudo falla y de que Ctrl+Z deshace el lote entero; después la creación
  pasa por el `ExternalEvent` de la ventana (`RunInRevit`, botones apagados con "⏳ Creando 16 conexiones…") y, al
  terminar, la cabecera, la tabla y la barra de estado enseñan el informe: filas `✔ Creada`, `▲ Creada con aviso`,
  `✖ Falló al crear` con el motivo en *Qué hacer*, y el resumen del lote. El título de la ventana deja de decir "todavía
  no crea nada".
- **Borrar el lote** (en **Más…** y como botón cuando el lote tiene conexiones): confirmación, borra por `batch_id` =
  `plan_id`, replanifica y lo dice ("Lote borrado: 16 conexiones quitadas, barras restauradas").
- **Columna *Qué hacer* con botones** (C3): el texto de siempre más uno o dos botones por caso, calculados en el Core
  (`PlanAdvice.Actions`) para que se prueben sin Revit: `ready` con aviso de perfil → **Excluir**; `invalid` → **Editar
  nudo**, **Excluir**; `no_match` con cordón → **Excluir**, **Plantilla…**; falta el cordón → **Cordón…**, **Barras…**;
  empalme → **Excluir**; `ambiguous_chord` → **Cordón…**; `offset` → **Ver en Revit**, **Excluir**; `already_connected` →
  **Incluir (rehacer)** (replanifica con `replace_existing`); `excluded` → **Incluir**; `created` → **Ver en Revit**;
  `failed` → **Ver en Revit**, **Excluir**. Cada botón hace lo mismo que la entrada del menú de clic derecho.
- **El consejo del empalme** (`fase-8.md` 12.1): cuando ninguna barra atraviesa el nudo y el cordón elegido (a mano con
  Cordón… o por el detector) termina ahí porque otro tramo del mismo cordón sigue por el otro lado (una barra del nudo
  casi paralela al cordón), el estado es `✖ Empalme del cordón` y *Qué hacer* dice "El cordón termina en este nudo
  (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye". Nunca "selecciónalo y replanifica".
  Sin tramo que siga (extremo de cercha o cordón sin seleccionar) el consejo sigue siendo el de la Fase 8.
- Lo demás de la ventana no cambia (mapa, ocultos, clic derecho, Más…, Descartar, no modal, pinchar con la ventana oculta).

### 3.6 Herramientas, rutas y guía

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_batch_create` | `plan_id`, `nodes[{node, validation_token, spec?}]`, `stop_on_error` (false). Espera larga (30 min): cada nudo abre su sesión de Advance Steel. Devuelve el informe. |
| MCP | `conn_batch_delete` | `batch_id`. Devuelve el informe de borrado. |
| MCP | `conn_list` | Nuevo argumento opcional `batch_id` (filtra en el puente; la respuesta trae `batch_id` por conexión y `batches`). |
| Rutas | `POST /conn/batch/create/`, `POST /conn/batch/delete/` | 25 rutas; `list` admite `batch_id` en el cuerpo por la ruta genérica. |
| Cinta | **Crear N conexiones**, **Borrar el lote**, botones de *Qué hacer* | Sección 3.5. |
| Guía | `docs/guide.md`, sección 6 "Crear el lote" y sección 7 | Confirmación explícita antes de crear y antes de borrar; si `conn_batch_create` no responde, `conn_list` con `batch_id` dice qué quedó creado antes de repetir. |

### 3.7 Sondeo 20: grupos anidados con Advance Steel (propuesta, riesgo 8)

`scripts/sondeos/20-grupos-anidados.py`, sobre la copia y sin transacción envolvente: abre un `TransactionGroup`
exterior, dentro llama por reflexión a `Bridge.Handle` `validate` + `create` del Detalle D (el add-in abre su propio
grupo: **anidado**) y después `delete`; asimila el exterior y comprueba `list` = 0 y las extensiones de las barras.
Segunda parte: exterior abierto, `create` dentro y **RollBack** del exterior: `list` = 0, cero placas y pernos sueltos
(sondeo 13) y extensiones como estaban. Si la primera parte falla, el instalador pone `batch_single_undo: false` en
`config\catalog.json` desplegado antes de crear el lote (plan B).

### 3.8 Arquitectura (propuesta 4, solo lo de esta fase)

```
src/MotorConexiones.Core/Batch/
├── BatchCreateRequest.cs       plan_id, nodes[{node, validation_token, spec?}], stop_on_error; INVALID_REQUEST si falta algo
├── BatchReport.cs              informe por nudo (created / created_with_warnings / updated / failed / skipped / rolled_back / deleted), cuentas, summary_text, JSON
├── BatchRunner.cs              qué nudos se crean, en qué orden, con qué comprobaciones; delegado por nudo (Revit crea, las pruebas simulan); stop_on_error; ApplyDeletion
├── BatchPlan.cs / NodeDetector.cs   estados created y failed; created_connection_id, existing_batch_id; last_report
└── PlanAdvice.cs               textos de created / failed / empalme / "ya creada en este lote", botones (Actions), cabecera con lo creado

src/MotorConexiones.Revit/
├── Batch/BatchCreator.cs       grupo exterior + OperationScope por nudo (token contra el modelo, create/update, adopción), quitar las marcas de los creados, informe; DeleteBatch por batch_id
├── Operations/BatchCreateOperation.cs, BatchDeleteOperation.cs; ListOperation con batch_id
├── Bridge.cs                   23 operaciones
└── UI/BatchPlanWindow          Crear N conexiones, Borrar el lote, botones en Qué hacer

mcp/revit_mcp/conexiones.py     25 rutas; tools/conn_tools.py 23 herramientas; simulador con batch_create / batch_delete
config/catalog.json             batch_single_undo (true)
scripts/sondeos/20-grupos-anidados.py
src/MotorConexiones.Tests/      BatchCreateTests, PlanAdviceTests (botones, empalme, cabecera con lo creado)
```

## 4. Reglas técnicas de esta fase

- Una operación = un `TransactionGroup`: **cada nudo es una operación** (`OperationScope`); el grupo exterior solo agrupa
  el deshacer y se puede desactivar con `batch_single_undo: false` sin recompilar.
- `conn_batch_create` exige el `validation_token` de `conn_batch_plan` **nudo a nudo** y lo vuelve a comprobar contra el
  modelo como `conn_create`. Sin token no se crea nada.
- `conn_batch_delete` nunca borra elementos que el add-in no creó (las garantías de `conn_delete`, una conexión cada vez).
- Sin ventanas en las rutas `conn_*`. Las confirmaciones viven en la ventana de la cinta y en la guía de la IA.
- El contrato del plan solo **añade**: estados `created` y `failed`, claves `created_connection_id`, `existing_batch_id`,
  `last_report`, `actions`, y `batch_id` / `batches` en `list`. Nombres de nudo estables (P5). `HangarTruss8b` sigue dando
  59 nudos y 16 `ready`.
- `ElementId.Value`; nada de miembros de la API sin compilar contra 2027 (`TransactionGroup` anidados se compilan y el
  sondeo 20 los prueba en el PC con Advance Steel).
- Unidades solo en `Units/UnitConverter.cs`. Código y claves JSON en inglés; textos en español.
- Versión **0.9.0** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador (regla `0.N.0` = fase).

## 5. Pruebas en la nube

- `BatchCreateTests`: la petición (válida; sin `plan_id`, sin token, nombres sueltos y claves desconocidas →
  `INVALID_REQUEST`); el lote sobre la cercha de la 8b (`HangarTruss8b`, 16 listos) con un creador simulado: 16 creadas
  (14 con aviso), tokens del plan, estados `created`, marcas de los creados, `summary_text`; un nudo que falla se revierte
  solo y los demás se crean; `stop_on_error` revierte todo (`rolled_back`, `stopped`, los siguientes `skipped`); nudos
  saltados por estado, token distinto del plan (`VALIDATION_TOKEN_INVALID` sin llamar al creador), nombre inexistente y
  nudo ya creado; `replace_existing` → `updated`; el informe ida y vuelta en JSON y guardado en el plan; `ApplyDeletion`
  devuelve los creados a `ready`; "Ya creada en este lote" tras replanificar con las conexiones del lote.
- `PlanAdviceTests`: textos y colores de `created` y `failed`, botones por estado, el empalme (dos tramos de cordón que
  terminan en el nudo y dos diagonales: el consejo del empalme, también con el cordón fijado a mano), la cabecera con lo
  creado y lo que falló.
- `simulador_revit.py --autocomprobar` con las 25 rutas, `batch_create` (creada, saltada la segunda vez, token malo),
  `list` con `batch_id` y `batch_delete`; `probar_conexiones.py` (28 pruebas; 30 con `--puente`): `batch_create` con un
  token **falso** → `failed` sin tocar el modelo, y `batch_delete` de un lote inexistente → `deleted_count: 0`. El
  script sigue sin crear nada en el modelo.
- `dotnet build` sin avisos, `dotnet test` en verde, `py_compile` de `mcp/` y `scripts/sondeos/`.

## 6. Instrucciones para el instalador (`docs/instalacion/fase-9.md`)

Sobre una **copia** del modelo (`HANGAR_PRUEBA_sondeo.rvt`), con `Anota` y resultados en `docs/fases/resultados-fase-9.md`:

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado (`0.9.0.0`), `instalar-conn.ps1` (25 rutas, 23 herramientas).
2. Abrir Revit: `ping` en `0.9.0` con 23 operaciones; sondeo 17; **sondeo 20** (grupos anidados): decide
   `batch_single_undo`.
3. Lo pendiente de la Fase 8 (8.5 y 12.5): `end_gap_mm`, `overrides` devuelto tal cual, `PLAN_MARKS_REPLACED`, `discard all`
   con cuenta; Plantilla… cancelado, Editar nudo y Planificar lote con la ventana abierta, Ver en Revit sin cuadro
   (anotado), el cordón inferior y Descartar con un plan marcado en otra vista.
4. **(la persona)** Planificar la cercha de la 8c (64 barras), mirar los botones de *Qué hacer* y el consejo del empalme
   en N9 (con Cordón… fijado a un tramo); **Crear 16 conexiones** desde la ventana; captura; **Ctrl+Z** una sola vez y
   comprobar con el sondeo 12 que no queda ninguna conexión y con el 13 que no queda acero suelto; **Ctrl+Y** (o crear
   otra vez) y **Borrar el lote**; sondeos 12 y 13 a cero.
5. Por el puente: `batch_plan` con `mark: false`, `batch_create` de **dos** nudos con sus tokens, `list` con `batch_id`,
   `batch_create` de los mismos otra vez (saltados), `batch_create` con un token alterado (fallido, sin cambios),
   `batch_delete`; sondeos 12 y 13 a cero.
6. Sondeo 19 v4 con el bloque de `fase-8.md` 12.7 (etiqueta B y clic sin cuadro); 19b.
7. `probar_conexiones.py --puente` (30/30) y el log del día, anotados en el archivo.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; `simulador_revit.py --autocomprobar` con
  las 25 rutas; `probar_conexiones.py` contra el simulador; `py_compile` de todo `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-9.md` según el Anexo B del encargo, con la lista de lo NO PROBADO (todo lo de Revit: grupos anidados con
  Advance Steel, Ctrl+Z del lote, la ventana).
- `docs/instalacion/fase-9.md` literal, con `Anota`, capturas con nombre fijo y qué devolver.
- README (estado, tabla de garantías, árbol, sección 12 "Planificar y crear un lote", errores), `mcp/CONTRATO-conn.md`,
  `docs/guide.md` (secciones 6 y 7), `CLAUDE.md` (estructura), `docs/propuestas/flujo-intuitivo.md` (C3 hecho; V2 si
  entró o no) y `docs/propuestas/catalogo-y-lotes.md` (estado) al día.
- `docs/fases/resumen-fase-9.md` con el resumen del chat.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta fase

- `conn_batch_update` (cambiar la típica y rehacer sus nudos): el terreno queda preparado con `batch_id` y `template_id`.
- Selección asistida (C6) y etiquetas pinchables en producción (V3): Fase 10 (el sondeo 19 v4 se ejecuta en esta
  instalación para dejarla decidida).
- Botón **Conectar** para un nudo suelto, miniaturas (V4), plano al lado (V5): Fase 11. Cartela automática: Fase 12.
- Nudos viga-columna o de apoyo en columnas (otro tipo de conexión).
