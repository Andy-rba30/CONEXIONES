# MotorConexiones

Add-in de Autodesk Revit 2027 en C# y herramientas MCP (`conn_*`) para crear conexiones de acero (cartelas,
placas cuchilla, pernos, soldaduras y retiros de barras) a partir de una especificación JSON leída de un plano.
Lo maneja una IA a través del servidor MCP `revit-mcp` (repositorio aparte, Python + pyRevit).

Estado (2026-10-06): **Fases 0 a 8 probadas en el PC; Fase 9 (crear por lotes) programada y probada en la nube, add-in 0.9.0 NO PROBADO en Revit (se prueba con `docs/instalacion/fase-9.md`).** Fases 0 a 7:
punta a punta desde la IA, placas y pernos de Advance Steel con las medidas del contrato, ventana de previsualización 2D
con cotas, borrado desde la cinta, panel en la pestaña ARBA, pernos con agarre real y placas centradas en su plano (rondas
6b, 6c y 6d) y el catálogo de conexiones (sección 11; ronda 7b cerrada en `docs/fases/fase-7.md`, secciones 7 y 7.9).
**Fase 8 cerrada** (`docs/fases/fase-8.md`, sección 8): detección de nudos de una cercha entera y plan de lote con marcas en
el modelo, desde la IA (`conn_batch_plan`, `conn_batch_plan_get`, `conn_batch_plan_discard`) y desde la cinta (botón
**Planificar lote**), sin crear nada (sección 12). En la ronda 8b el PC confirmó la corrección del corte de los ejes: 56
barras de la cercha del Hangar → 59 nudos con **16 `ready`** con la plantilla oficial (8 como el Detalle D y 8 en espejo),
replan con el mismo token y **Editar nudo** sobre un nudo listo (`resultados-fase-8b.md`). El cierre (0.8.2) corrigió cinco
cosas pequeñas (`end_gap_mm` en la respuesta, un solo plan marcado por documento, el conteo de `discard all`, `overrides`
sin `IsEmpty`, el paso 9 del sondeo 17). **Ronda 8c** (`fase-8.md`, sección 9; sale de `docs/propuestas/flujo-intuitivo.md`):
la ventana del plan se entiende sin leer este README: cabecera con la decisión ("Se crearán 16 conexiones con … (8 iguales,
8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas."), **mapa de la cercha** con un
círculo por nudo del color de su estado, solo los nudos de verdad en la tabla (barras sueltas y parejas sin cordón ocultas
con contador), estados en español, columna **Qué hacer**, cuatro botones y un menú de clic derecho; en el modelo las
marcas van **por estado** (verde, ámbar, rojo, gris); la IA recibe `summary_text`, `status_text`, `advice` y
`visible_by_default` y la guía le dice que resuma en español sin volcar el JSON; con el catálogo vacío, aviso
`CATALOG_EMPTY` y botón **Abrir catálogo**; y el sondeo 19 prueba las etiquetas pinchables en el lienzo (para la Fase 10).
La ronda 8c **se probó en el PC** el 2026-10-05 (`resultados-fase-8c.md`: 64 barras → los mismos 16 listos del cordón
central, 10 nudos del cordón superior "sin plantilla que encaje" porque el Detalle D no es su típica, mapa y colores por
estado bien, `CATALOG_EMPTY`, `discard all` a cero y `--puente` 28/28). Su **cierre** (`fase-8.md`, sección 10) corrige
lo que la persona vio: la ventana del plan pasa a ser **no modal** (se queda abierta mientras orbitas y pinchas en Revit;
todo lo que toca el modelo va por un `ExternalEvent`), **Ver en Revit** hace zoom al nudo sin ningún cuadro, **Descartar**
desde la ventana quita también los cubos (los de cualquier plan) y el sondeo 19 se repite con BMP buscando el manejador de
clics por reflexión. La **ronda 8d** se probó en el PC el 2026-10-05 (`resultados-fase-8d.md`, 0.8.4): la ventana **se quedó
abierta** mientras la persona orbitaba y pinchaba, Descartar desde la ventana dejó 0 cubos con un plan del puente marcado en
otra vista, el catálogo vacío avisó con la ventana abierta y `--puente` 28/28; pero **Cordón… cerró Revit con "fatal error"**
(el diálogo de elección en modo de una sola elección lanzaba una excepción que, con la ventana no modal, nadie capturaba),
**Descartar plan no cerraba la ventana** y la etiqueta del sondeo 19 v2 **no se vio** aunque Revit aceptó el control. Su
**cierre** (`fase-8.md`, sección 11, add-in **0.8.5**) corrige el diálogo, mete cada manejador de la ventana en una red que
captura cualquier excepción (y una red en el despachador solo para las de este add-in), pincha en Revit con la ventana del
plan **oculta** y la ventana principal de Revit activada, con una línea en el log antes y otra después, cierra la ventana al
descartar y repite el sondeo 19 (v3: BMP de 24 bits de 32×32 en una ruta sin tildes, `SetVisibility`, refresco de la vista
y una segunda etiqueta en el centro de la caja de sección). La **ronda 8e** se probó en el PC el 2026-10-05
(`resultados-fase-8e.md`, 0.8.5; el puente se repitió el 2026-10-06 con Revit abierto): **Cordón… sobre N9 ya no cierra
Revit** (nueve pinchados con la ventana oculta y de vuelta, sin ningún error de ventana en el log), Descartar cierra la
ventana y deja 0 marcadores, **la etiqueta del sondeo 19 se vio y se pinchó** (capturas `fase8e-01-etiqueta` y
`fase8e-01-etiqueta-clic`), sondeos 17, 12 y 13 en cero y `--puente` **28/28**. Su cierre (`fase-8.md`, sección 12) no toca
el add-in: corrige el sondeo 19 (v4: la etiqueta B con `ElementId(Int64)` y el clic **sin cuadro**, porque el cuadro modal del
manejador dejaba a Revit sin atender a pyRevit) y da por decidida V3 (etiquetas pinchables) para la Fase 10. **La Fase 8
queda cerrada del todo.** **Fase 9** (`docs/fases/fase-9.md`, prompt en `docs/prompts/fase-9.md`, add-in **0.9.0**): el plan
se convierte en acero. `conn_batch_create` (y el botón **Crear N conexiones** de la ventana del plan) crea los nudos listos
**nudo a nudo con los tokens del plan**: cada nudo es una operación atómica propia (si uno falla se revierte solo y los demás
se quedan) anidada en un grupo del lote que se asimila al final (**una sola entrada de deshacer**; `batch_single_undo` en
`config/catalog.json` es el plan B). Informe por nudo (creada, creada con aviso, rehecha, fallida y por qué, saltada y por
qué), `conn_batch_delete` y **Borrar el lote** (una a una con las garantías de `conn_delete`), `conn_list` con `batch_id`,
la columna **Qué hacer** con botones (mejora C3) y el consejo del empalme del cordón ("El cordón termina en este nudo
(empalme)…"). Decisiones de la persona: los 14 nudos con cordón HSS4X4 se crean con la cartela del Detalle D y el aviso de
perfil (P6); los 10 nudos del cordón superior sin plantilla y los 7 empalmes quedan fuera (un `no_match` no se crea). Las
cartelas fantasma (V2) no entraron: siguen en `docs/propuestas/flujo-intuitivo.md`. Probado en la nube: 192 pruebas del
Core, simulador 62/62, `probar_conexiones.py` 28/28; **NO PROBADO en Revit** (grupos anidados con Advance Steel, Ctrl+Z del
lote, la ventana): `docs/instalacion/fase-9.md`, que empieza por el sondeo 20.

---

## 1. Qué es MotorConexiones

Un sistema para modelar en Revit el nudo de una cercha tal como lo dibuja el plano de fabricación, sin inventar nada.
La IA lee el detalle, escribe una especificación JSON (`gusset_node`, contrato v1), la valida con el add-in y, con la
confirmación del usuario, la crea. También se puede usar sin IA desde la cinta de Revit: panel **MotorConexiones** en la
pestaña **ARBA** (o en **Conexiones** si ARBA no se pudo usar) con los botones **Ejecutar especificación JSON** (abre la
ventana de previsualización, sección 9), **Conexiones del modelo** (lista y borra, sección 10), **Catálogo** (plantillas
con nombre que se aplican a otro nudo, sección 11) y **Planificar lote** (detecta los nudos de una cercha, planifica y, desde
la Fase 9, crea el lote con **Crear N conexiones** y lo quita con **Borrar el lote**, sección 12).

Lo que garantiza el add-in y dónde está probado:

| Garantía | Qué significa | Probado en |
|---|---|---|
| Validación obligatoria | `conn_create` y `conn_update` exigen el `validation_token` (SHA-256) que solo entrega `conn_validate` sin errores. Cubre la especificación, el documento y las barras del nudo. | `docs/fases/resultados-fase-3.md` y `resultados-fase-4.md` (`create` sin token → `VALIDATION_TOKEN_INVALID`; token determinista: el mismo en las Fases 3 y 4) |
| Token ligado a `config/limits.json` | Desde la Fase 5 el token incluye también el hash de los límites AISC: si editas `limits.json` entre validar y crear, el token deja de valer. | Pruebas del Core en la nube (51). **PENDIENTE DE INSTALADOR** en Revit: sondeo 14 de `docs/instalacion/fase-5.md` |
| Backend nativo Advance Steel | Cartela y placa cuchilla como `SteelProxyElement` de categoría *Plates*, pernos como *Bolts*, **con las medidas del contrato** (Advance Steel trabaja en mm; el add-in convierte desde los pies de Revit). Las soldaduras van como `DirectShape` (reserva prevista en v1) y, si Advance Steel no está disponible, todo sale por `DirectShape`. | `resultados-fase-5.md`, ronda 5b, `5b-4` (cartela 565 × 530 × 9,53 mm, cuchilla 170 × 140 × 10 mm, pernos 5/8" a 60 mm) y capturas `fase5-03/04` |
| Atómico y sin ventanas | Cada operación es un `TransactionGroup`; ante un error, rollback completo. Los diálogos de Revit se cancelan y quedan como avisos en la respuesta. | `resultados-fase-3.md` y `resultados-fase-4.md` ("sin ventanas ni cierres de Revit"; avisos `REVIT_WARNING` en `create`) |
| Reversible | `conn_delete` borra solo lo que creó el add-in y devuelve a las barras sus extensiones originales (guardadas en Extensible Storage). | `resultados-fase-3.md` y `resultados-fase-5.md` (`B-3` y `5b-6`: `extension ... -> 0.0 mm`, `conexiones tras borrar: 0`, `Elementos de acero sueltos encontrados: 0`) |
| Cliente de IA | Las 13 herramientas `conn_*` se ven y funcionan desde Antigravity por el puente HTTP del puerto 8000. | `resultados-fase-4.md`, sección "4-9 cliente de IA" |
| Punta a punta desde la IA | Desde Antigravity: `conn_ping` → guía → tipos → `node_info` → esquema → `validate` → `preview` → confirmación literal → `create` (9 elementos, 2,2 s) → `list` → `get` → confirmación literal → `delete` → `list` = 0, sin ventanas ni cierres de Revit. | `resultados-fase-5.md`, sección "B-1 Antigravity (punta a punta)" |
| Token ligado a `limits.json` | El `validation_token` incluye el hash del `limits.json` desplegado: con un `limits.json` distinto o un token alterado, `create` responde `VALIDATION_TOKEN_INVALID` y el modelo no cambia. | `resultados-fase-5.md`, `A-7` (sondeo 14, 14/14) |
| Ventana de previsualización | El botón de la cinta dibuja el nudo con cotas iguales al plano, la tabla edita el JSON y revalida, Guardar JSON no toca el original, Crear y borrar desde la cinta funcionan igual que `conn_create`/`conn_delete`. | `resultados-fase-6.md` y capturas `fase6-01` a `fase6-07` |
| Catálogo de plantillas (Fase 7) | Una conexión creada se guarda como plantilla sin IDs (`conn_catalog_save`, botón **Guardar en catálogo**); `conn_catalog_apply` la casa por ángulos con las barras de otro nudo (también en espejo), instancia la especificación y la valida con el token de siempre. El marco del nudo es canónico (X hacia +X global, +Y hacia arriba) y los ángulos van con signo. | Pruebas del Core en la nube (131: ida y vuelta del Detalle D, 4 orientaciones, sin encaje, políticas de perfil, almacén) y simulador del MCP (36/36 y 23/23). En Revit, `resultados-fase-7.md`: marco canónico sobre el nudo real (`x_axis [1,0,0]`, `chord_direction_reversed: true`, ángulos 136,9 / 44,4 / −135,6 con `side` +Y / +Y / −Y); el Detalle D creado como en el plano (capturas `fase7-02/03`); `catalog_save` desde la conexión real con copia en `catalog\`; `catalog_apply` al mismo nudo `same`, desvío 0, sin avisos y token nuevo; la misma plantilla en el nudo simétrico `mirror_x`, validada en verde y creada (capturas `fase7-06/07`); `probar_conexiones.py --puente` 25/25; sondeos 12 y 13 en cero. Ronda 7b en el PC (`resultados-fase-7b.md`): `deploy.ps1` y `conn_ping` en `0.7.0`, fixture con `1 aviso(s)` y rótulo `diagonal 1249631` (captura `fase7b-01`), plantilla oficial regenerada con `overwrite: true` (mismo `template_id`, tres `diagonal`), sondeos 12 y 13 en cero |
| Plan de lote (Fase 8, cerrada del todo; rondas 8b a 8e) | Con la cercha seleccionada, `conn_batch_plan` (o el botón **Planificar lote**) lleva cada extremo de barra al corte de su eje con el eje del cordón (las diagonales reales terminan en la cara del cordón, a 15–85 mm de su eje; se admite hasta medio canto de cada barra más 10 mm, `node_face_reach_mm`), agrupa esos puntos en nudos, reconoce el cordón que atraviesa, nombra los nudos `N1…`, casa cada uno con las plantillas (también en espejo), instancia la especificación con `source.batch_id` y la valida con el token de siempre; marca los nudos en la vista (colores y marcadores, sin escribir `Marca`; un solo plan marcado por documento) y admite correcciones acumuladas con el mismo `plan_id`. **No crea nada.** | Pruebas del Core en la nube (162: cercha sintética de 30 nudos, la cercha del Hangar de la Fase 8 reconstruida de los puntos de trabajo, 53 barras y 10 `ready`, y la **cercha de la ronda 8b con sus ejes reales**, 56 barras: 59 nudos, 16 `ready` con la plantilla oficial, 8 `same` y 8 `mirror_x`, 20 `no_match` con `NODE_CHORD_NOT_CONTINUOUS`, 23 `untyped`, el mismo plan nudo a nudo que devolvió el PC, y el plan de solo las cuatro barras del Detalle D con el nudo listo llamado N4) y simulador del MCP (45/45 y 26/26; las pruebas 22 y 23 del puente buscan el nudo listo por su cordón en vez de suponer "N1"). En Revit, ronda 8b (`resultados-fase-8b.md`, add-in 0.8.1): `deploy` y `ping` en `0.8.1`; sondeo 18 midiendo en el modelo que las diagonales del Detalle D terminan a 58,8 / 14,3 / 33,1 mm del eje del cordón con los ejes cortándolo a menos de 4 mm entre sí; `conn_batch_plan` sobre 56 barras → **`ready: 16, no_match: 20, untyped: 23`** en 953 ms, N4 (gemelo del Detalle D) `ready same` con desvío 0 y token de 64, N7 `mirror_x`, marcadores sobre el eje del cordón (capturas `fase8b-02/03`), `TEMPLATE_PROFILE_DIFFERS` en los nudos de cordón HSS4X4; replan con `exclude` e `include` con el mismo `plan_id` y **el mismo token**; ventana del plan con 16 listos y **Editar nudo** abriendo la previsualización de N4 validada (`fase8b-04/05`); `discard all` y sondeos 17, 12 y 13 a cero. Fase 8 (`resultados-fase-8.md`, 0.8.0): marcas, ventana, Ver en Revit, Cordón y Añadir nudo pinchando. **NO PROBADO en Revit (0.8.2, se comprueba en la instalación de la ronda 8c, `fase-8.md` 8.5 y 9.3)**: `end_gap_mm` en la respuesta, `PLAN_MARKS_REPLACED`, el conteo de `removed_markers`, `overrides` sin `IsEmpty`, el paso 9 del sondeo 17, `--puente` 28/28 (la 8b no lo anotó; según el chat del instalador fallaron 22, 23 y 27, y 22 y 23 suponían "N1"). **Ronda 8c (0.8.3, NO PROBADA en Revit; `docs/instalacion/fase-8c.md`)**: ventana del plan que se entiende (cabecera con la decisión, mapa de la cercha, solo nudos de verdad, estados en español con icono y color, columna Qué hacer, 4 botones + Más… + clic derecho), marcas del modelo con color por estado (los ocultos no se marcan), `summary_text` / `status_text` / `advice` / `visible_by_default` en la respuesta, aviso `CATALOG_EMPTY` con botón Abrir catálogo y sondeo 19 (etiquetas en el lienzo). Probado en la nube: 177 pruebas del Core (+15: `PlanAdviceTests` con la cabecera exacta de la cercha de la 8b y `TrussMapTests` con su alzado: 56 segmentos, 59 puntos, 16 visibles, N4 en X = −11870), simulador 47/47 y `probar_conexiones.py` 26/26. **Ronda 8c probada en el PC** (`resultados-fase-8c.md`, 0.8.3): 64 barras → los mismos 16 listos, 10 `no_match` con cordón y 7 `✖ Falta el cordón` en el cordón superior (el Detalle D es la típica del cordón central; `fase-8.md` 10.2), colores por estado y mapa (`fase8c-01/02`), `CATALOG_EMPTY` con Abrir catálogo, replan con el mismo token, `discard all` 66 → 0, sondeo 17 paso 9 y `--puente` **28/28**. **Cierre de la 8c (0.8.4, NO PROBADO en Revit; `docs/instalacion/fase-8d.md`)**: ventana del plan no modal con `ExternalEvent`, Ver en Revit sin cuadro, Descartar que quita todos los marcadores (y estado en memoria restaurado si Revit deshace), sondeo 19 v2 con BMP; en la nube 177 pruebas, simulador 47/47 y `probar_conexiones.py` 26/26. Ronda 8d en el PC (`resultados-fase-8d.md`, 0.8.4): ventana abierta mientras se orbita y se pincha (captura `fase8d-02`), Descartar desde la ventana con un plan del puente marcado en otra vista → `remaining_markers: 0` y sondeo 17 a cero, catálogo vacío con la ventana abierta, sondeo 19 v2 con el BMP aceptado y el manejador de clics registrado (etiqueta no visible), `--puente` 28/28; **Cordón… cerró Revit** (captura `fase8d-crash-revit`) y Descartar no cerró la ventana. **Cierre de la 8d (0.8.5, `fase-8.md` sección 11) probado en el PC en la ronda 8e** (`resultados-fase-8e.md`, 2026-10-05, puente repetido el 2026-10-06; `fase-8.md` sección 12): Cordón… sobre N9 seis veces y Barras… dos veces **sin cerrar Revit**, con la ventana oculta mientras se pincha y de vuelta después (9 `ribbon_batch_pick` / 9 `ribbon_batch_picked`, 0 `ribbon_batch_window_error`; captura `fase8e-02-cordon-n9`), Esc cancela (`cancelado`), Excluir / Incluir y Replanificar con la ventana abierta, Descartar desde la ventana con `closes_window: true` y `closed … discarded: true` (0 marcadores; sondeo 17 a cero), sondeo 19 v3 con **la etiqueta vista y pinchada** (`fase8e-01-etiqueta`, `fase8e-01-etiqueta-clic`; cuatro clics anotados), sondeos 12 y 13 en cero y `--puente` **28/28**. **NO PROBADO en Revit**: el sondeo 19 **v4** (cierre de la 8e: la etiqueta B y el clic sin cuadro; se ejecuta en la instalación de la Fase 9 o de la 10), Plantilla… cancelado, Editar nudo y Planificar lote con la ventana abierta, el cordón inferior y Descartar con un plan marcado en otra vista |
| Crear por lotes (Fase 9, **NO PROBADA en Revit**) | `conn_batch_create` (o **Crear N conexiones**) exige `plan_id` y el `validation_token` del plan **por nudo** (sin token no se crea nada; un nombre suelto es `INVALID_REQUEST`), lo compara con el del plan y lo vuelve a comprobar contra el modelo como `conn_create`; crea cada nudo como una operación propia (`OperationScope`: grupo, `IFailuresPreprocessor`, diálogos cancelados, adopción de los elementos de Advance Steel, registro con `source.batch_id`) anidada en un grupo exterior del lote: **el nudo que falla se revierte solo y los demás se quedan** (P9); el grupo exterior se asimila (una entrada de deshacer) o se revierte entero con `stop_on_error`. Solo se crean `ready` y `failed` (reintento); `invalid`, `no_match`, `excluded`, `untyped`, `already_connected` y los ya creados se saltan con su motivo. `conn_batch_delete` borra las conexiones del lote una a una con las garantías de `conn_delete`, en un solo grupo. | Pruebas del Core en la nube (192: `BatchCreateTests` con la cercha de la 8b y un creador simulado: 16 creadas con 14 avisos, un nudo que falla se revierte solo, `stop_on_error` revierte todo, saltados por estado, token distinto del plan, `replace_existing` → rehecha, informe JSON ida y vuelta, borrado que devuelve los nudos a listos, "ya creada en este lote" tras replanificar; `PlanActionsTests`: botones por estado y el empalme detectado con dos tramos de cordón), simulador 62/62 (crear, saltar la segunda vez, token falso, `stop_on_error`, `list` por lote, borrar) y `probar_conexiones.py` 28/28 (`batch_create` con token falso no toca el modelo; `batch_delete` de un lote inexistente). **NO PROBADO en Revit**: los grupos anidados con la sesión de Advance Steel (sondeo 20 en `docs/instalacion/fase-9.md`, con el plan B `batch_single_undo: false`), Ctrl+Z del lote entero, Crear y Borrar desde la ventana, el informe en pantalla |
| Pernos con agarre real (ronda 6b) | La placa cuchilla apoya sobre una cara de la cartela (`plate.gusset_face`, `+z` por defecto) y los pernos atraviesan cartela + placa: agarre = suma de espesores y longitud calculada de `limits.json` (Detalle D: 19,5 mm y 44,45 mm) o tomada de `bolts.length_mm`. Las cotas del croquis se editan con doble clic. | Pruebas del Core en la nube (99); ronda 6b en el PC: placa apoyada y `Bolt Length 44,45` / `Grip Length 19,53` (`resultados-fase-6b.md`). Ronda 6c: pernos con cabeza en la placa y `Grip 19,52` medidos por el sondeo 16 (`resultados-fase-6c.md`). Ronda 6d (placas centradas en su plano): sondeo 16 en la sesión de la Fase 7, cartela `−4,76 .. 4,76`, placa cuchilla `4,76 .. 14,76` y pernos `−29,69 .. 24,69`, las tres `OK` (`resultados-fase-7.md`, bloque `6d-3 sondeo 16`; `fase-6.md`, sección 9) |

Lo que **no** hace: no diseña ni verifica resistencias; no lee planos PDF completos; v1 solo conoce `gusset_node`.

---

## 2. Estructura del repositorio

```text
CONEXIONES/
├── CLAUDE.md                        Reglas permanentes del repositorio
├── MotorConexiones.sln
├── src/
│   ├── MotorConexiones.Core/        netstandard2.0, sin referencias a Revit
│   │   ├── AddinInfo.cs             Versión del add-in (0.9.0 en la Fase 9; igual que <Version> de los csproj) y spec_version (1.0)
│   │   ├── Batch/                   Fase 8: NodeDetector (segmentos → nudos), BatchOverrides (correcciones), BatchPlan (el plan
│   │   │                            y sus nudos, JSON), PlanBuilder (detección + correcciones + casado + instanciación + validación),
│   │   │                            PlanAdvice (ronda 8c: estado en español, color por estado, qué hacer, cabecera con la decisión;
│   │   │                            Fase 9: created/failed, botones de Qué hacer, empalme), TrussMap (alzado 2D de la cercha para el
│   │   │                            mapa de la ventana) y, Fase 9, BatchCreateRequest (plan_id + nudos con token), BatchReport (informe
│   │   │                            por nudo) y BatchRunner (qué se crea, en qué orden, stop_on_error; el creador lo pone Revit)
│   │   ├── Catalog/                 Fase 7: CatalogTemplate (archivo de plantilla), CatalogConfig (config/catalog.json),
│   │   │                            TemplateNode (el nudo con ángulos con signo), TemplateBuilder (spec → plantilla),
│   │   │                            TemplateMatcher (4 orientaciones), TemplateInstantiator (plantilla → spec con IDs),
│   │   │                            CatalogStore (un JSON por plantilla), TemplateJson
│   │   ├── Contract/                ConnectionSpec, ChordSpec, GussetSpec, GussetOutline, MemberSpec, AttachmentSpec,
│   │   │                            KnifePlateSpec, BoltPatternSpec, WeldSpec, DimensionChain, UncertainField, SourceInfo
│   │   │                            (con template_id y batch_id), NodeRef, ApiResponse/ApiError/ApiMeta (sobre común), JsonOptions
│   │   ├── Editing/SpecEditor.cs    Tabla editable por ruta JSON (campos, valores, contorno, JSON con sangría)
│   │   ├── Geometry2D/              Point2D, Segment2D, Polygon2D, Geometry2DChecks (contorno, cruces, pernos en placa)
│   │   ├── Geometry3D/              Vec3, NodeFrame (sistema local canónico del nudo y ángulos con signo), NodeReach,
│   │   │                            ConnectionGeometry, BoltGrid, BoltPosition, WeldLine2D
│   │   ├── Model/IModelFacts.cs     Lo que el validador necesita del modelo, sin depender de Revit
│   │   ├── Schema/JsonSchemaValidator.cs   JSON Schema de gusset_node y su comprobación
│   │   ├── Sketch/                  Croquis 2D en mm: SketchPrimitives, SketchText, SketchNodeInfo, ISketchProvider,
│   │   │                            GussetNodeSketch, SketchBuilder (la ventana solo dibuja lo que sale de aquí)
│   │   ├── Storage/                 ConnectionRecord, ModifiedMemberRecord (lo que se guarda por conexión)
│   │   ├── Types/                   IConnectionType, ConnectionTypeRegistry, GussetNodeType (también ISketchProvider)
│   │   ├── Units/UnitConverter.cs   ÚNICO sitio donde se convierten mm y grados a pies y radianes
│   │   └── Validation/              SpecValidator (las 10 reglas), ValidationTokenGenerator, LimitsConfig,
│   │                                LabelParser, ProfileMatcher, ErrorCodes
│   ├── MotorConexiones.Revit/       net10.0-windows, add-in de Revit 2027
│   │   ├── App.cs                   IExternalApplication: panel "MotorConexiones" en la pestaña ARBA (reserva "Conexiones")
│   │   ├── RunSpecCommand.cs        Botón "Ejecutar especificación JSON": abre la ventana de previsualización y crea
│   │   ├── ListConnectionsCommand.cs  Botón "Conexiones del modelo": lista y borra
│   │   ├── CatalogCommand.cs        Botón "Catálogo" (Fase 7): aplicar a la selección, guardar desde conexión, eliminar
│   │   ├── BatchPlanCommand.cs      Botón "Planificar lote" (Fase 8): planifica, crea el ExternalEvent y abre la ventana del plan (no modal desde la 0.8.4)
│   │   ├── Batch/                   BatchPlanner (selección → plan → marcas; DiscardAndClean), BatchCreator (Fase 9: grupo exterior + OperationScope por
│   │   │                            nudo, token contra el modelo, marcas de los creados fuera, informe; DeleteBatch), PlanMarks (colores y marcadores), PlanRegistry (planes en memoria),
│   │   │                            PlanEvents (cierre 8c: ExternalEvent y cola de la ventana no modal), PlanSnapshot (plan + mapa + tipos leídos
│   │   │                            en contexto válido), PlanZoom (Ver en Revit sin cuadro) y PlanPicker (pinchar barras desde el evento)
│   │   ├── Catalog/                 CatalogService (Revit → Core: construir, guardar, aplicar y validar), CatalogConfigLoader
│   │   ├── UI/                      WPF, solo en el camino de los botones: SketchCanvas, PreviewSession, PreviewWindow,
│   │   │                            ConnectionsWindow, CatalogWindow, SaveTemplateDialog, BatchPlanWindow, ChooseDialog,
│   │   │                            TrussMapCanvas (ronda 8c: el mapa de la cercha con un círculo por nudo) y RevitMainWindow
│   │   │                            (cierre 8d: activa la ventana principal de Revit antes de pinchar con la ventana del plan oculta)
│   │   ├── Bridge.cs                Bridge.Handle(operation, requestJson, doc, uidoc): punto de entrada del MCP
│   │   ├── LimitsConfigLoader.cs    Lee config\limits.json de la carpeta del add-in desplegado
│   │   ├── Fabrication/             IFabricationBackend, AdvanceSteelBackend, DirectShapeBackend, BackendFactory,
│   │   │                            MemberModifier (retiros de extremo)
│   │   ├── Node/                    NodeInspector, RevitGeometry, RevitModelFacts
│   │   ├── Operations/              IOperation + una clase por operación: Ping, Guide, Types, Schema, NodeInfo,
│   │   │                            FindProfile, Validate, Preview, Create, List, Get, Update, Delete (13) y
│   │   │                            CatalogList, CatalogGet, CatalogSave, CatalogDelete, CatalogApply (Fase 7) y
│   │   │                            BatchPlan, BatchPlanGet, BatchPlanDiscard (Fase 8) y BatchCreate, BatchDelete (Fase 9; List admite batch_id)
│   │   ├── Services/                ConnectionCreationService (crea la conexión completa y adopta los elementos de acero),
│   │   │                            ValidationService (validar contra el modelo, una sola regla), RibbonCreation (crear desde la cinta)
│   │   ├── Storage/ConnectionStorageManager.cs     Extensible Storage: esquema MotorConexionesConnection (GUID fijo, v1)
│   │   ├── Transactions/OperationScope.cs          TransactionGroup + IFailuresPreprocessor + DialogBoxShowing
│   │   └── Logging/JsonLineLogger.cs               Una línea JSON por llamada en %LOCALAPPDATA%\MotorConexiones\log\
│   └── MotorConexiones.Tests/       xUnit (192 pruebas), solo Core, con el fixture del Detalle D, la cercha sintética (Fakes/SyntheticTruss.cs) y las del Hangar (Fakes/HangarTruss.cs, HangarTruss8b.cs)
├── catalog/                         Plantillas oficiales del catálogo (deploy.ps1 copia las que falten al PC); ver catalog/LEEME.md
├── config/limits.json               Tolerancias y mínimos AISC 360 (J3.3, J3.4, J2.4), editable sin recompilar
├── config/catalog.json              Carpetas del catálogo, tolerancia de casado (10°), aviso de desvío (5°), espejo, política de perfil,
│                                    agrupación de extremos (node_cluster_mm 10), "atraviesa el nudo" (node_axis_max_distance_mm 5) y
│                                    alcance de cara (node_face_reach_mm, 0 = medio canto de cada barra + 10; ronda 8b) y batch_single_undo
│                                    (Fase 9: true = el lote es una sola entrada de deshacer; false = una por nudo, plan B)
├── docs/
│   ├── ENCARGO_MOTOR_CONEXIONES.md  El encargo completo, por fases
│   ├── guide.md                     Guía para la IA (la devuelve conn_get_guide), editable sin recompilar
│   ├── fixtures/                    detalle-D.json (con dudas), detalle-D-confirmado.json (dudas resueltas),
│   │                                detalle-D.png, cercha-vista-general.png
│   ├── fases/                       fase-N.md (informe de cada fase), resultados-fase-N.md (salidas del PC), capturas/
│   ├── prompts/                     prompt y alcance de cada fase posterior al encargo (fase-6.md, fase-7.md, fase-7b.md, fase-8.md, fase-8c.md, fase-9.md)
│   ├── propuestas/                  Ideas por aclarar antes de programar (catalogo-y-lotes.md, flujo-intuitivo.md)
│   └── instalacion/                 Instrucciones literales para el agente instalador, una por fase
├── mcp/                             Archivos nuevos para la extensión revit-mcp (no se toca lo existente)
│   ├── revit_mcp/conexiones.py      Adaptador IronPython 2.7: 25 rutas /conn/... -> Bridge.Handle
│   ├── tools/conn_tools.py          23 herramientas @mcp.tool() conn_* (CPython, SDK mcp 2.x)
│   ├── CONTRATO-conn.md             Contrato de las rutas /conn/ (para pegar al final de CONTRATO.md de revit-mcp)
│   ├── instalar-conn.ps1            Copia los dos archivos a la extensión y añade las líneas de registro (idempotente)
│   └── pruebas/                     probar_conexiones.py (28 pruebas + 2 con --puente) y simulador_revit.py (solo nube)
└── scripts/
    ├── deploy.ps1                   Compila en Release y copia DLL, .addin, config\*.json y docs\guide.md a Addins\2027; plantillas de catalog\
    ├── revit-exec.ps1               Ejecuta un sondeo IronPython dentro de Revit (por /execute_code/ o -SinTransaccion)
    ├── conn-call.ps1                Llama a una operación del add-in por HTTP (ping o /conn/op/<operación>/)
    └── sondeos/                     00 a 20, 19b y capturar-nudo.py (sondeos para el instalador; 17 = marcas del plan, 18 = extremos en la cara, 19 y 19b = etiquetas en el lienzo, 20 = grupos anidados con Advance Steel, Fase 9)
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

Se espera `0 Errores` y `Superado: 192`.

### Paso 2: desplegar el add-in (con Revit cerrado)

`deploy.ps1` compila en Release (salvo con `-NoBuild`) y copia `MotorConexiones.Core.dll`, `MotorConexiones.Revit.dll`,
`config\limits.json` y `docs\guide.md` a `%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\`, más el manifiesto
`MotorConexiones.addin`. Revit bloquea la DLL mientras está abierto, así que ciérralo antes.

```powershell
.\scripts\deploy.ps1
```

Se espera `== MotorConexiones 0.9.0.0 desplegado en Revit 2027 ==`.

### Paso 3: instalar las rutas y herramientas del MCP en la extensión

```powershell
.\mcp\instalar-conn.ps1
```

Copia `mcp\revit_mcp\conexiones.py` y `mcp\tools\conn_tools.py` a la extensión y añade, si faltan, las dos líneas de
registro en `startup.py` y las dos en `tools\__init__.py`. Es idempotente: se puede repetir. Se espera
`(25 rutas @api.route)` y `(23 herramientas @mcp.tool)`.

### Paso 4: abrir Revit y comprobar el add-in

1. Abre **Revit 2027** con tu modelo (para las pruebas, la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
   nunca el original). Si Revit pregunta por el add-in sin firmar, pulsa *Always Load*.
2. En la pestaña **ARBA** debe aparecer el panel **MotorConexiones** con los botones **Ejecutar especificación JSON**,
   **Conexiones del modelo** y **Catálogo**. Si ARBA no se pudo usar, el panel está en la pestaña **Conexiones** (el
   motivo queda en el log, evento `ribbon_panel_created`).
3. Espera a que pyRevit cargue (unos 20 s; pyRevit solo lee `conexiones.py` al arrancar Revit) y comprueba:

   ```powershell
   .\scripts\conn-call.ps1 -Operation ping
   ```

   Se espera `ok: true`, `addin_version: 0.9.0`, `backend: advancesteel` y, en `operations`, las 23 operaciones.

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

### Si solo cambió `config/limits.json`, `config/catalog.json`, `docs/guide.md` o `catalog/*.json`

Basta `.\scripts\deploy.ps1 -NoBuild` (copia los archivos de configuración y la guía, y las plantillas de `catalog/` que
falten en el PC). Para `guide.md` y `catalog.json` ni siquiera hace falta reiniciar Revit (el add-in los lee en cada
llamada); para `limits.json`, ver la sección 6.

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
  "plate_outside_gusset_tolerance_mm": 2.0,
  "bolts": {
    "min_spacing_factor": 2.667,
    "edge_distance_mm": { "12.7": 19.0, "15.875": 22.0, "19.05": 25.0, "22.225": 28.0, "25.4": 32.0, "28.575": 38.0, "31.75": 42.0 },
    "length_addition_mm": { "12.7": 17.46, "15.875": 22.23, "19.05": 25.4, "22.225": 28.58, "25.4": 31.75, "28.575": 38.1, "31.75": 41.28, "default": 1.4 },
    "length_increment_mm": 6.35
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
- `bolts.length_addition_mm` y `bolts.length_increment_mm` (ronda 6b): longitud del perno = agarre (espesor de la
  cartela + espesor de la placa cuchilla) + suplemento por diámetro (tuerca, arandela y rosca sobrante, RCSC tabla
  C-2.1), redondeada hacia arriba a múltiplos del incremento (1/4" = 6,35 mm; pon 5 para pernos métricos). `default` es
  un factor sobre el diámetro para diámetros fuera de la tabla. Si el JSON trae `bolts.length_mm`, manda ese valor y el
  validador solo avisa (`BOLT_LENGTH_TOO_SHORT`) si es menor que agarre + suplemento.
- `plate_outside_gusset_tolerance_mm` (Fase 7): cuánto puede asomar una esquina de la placa cuchilla fuera del contorno
  de la cartela sin error `PLATE_OUTSIDE_GUSSET`. Desde la Fase 7 esa regla comprueba la placa donde está la barra de
  verdad (ángulo con signo del modelo); la placa del Detalle D termina justo en el chaflán y asoma unas décimas.
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

5. **Croquis (opcional).** Si el tipo nuevo implementa también `ISketchProvider`
   (`src/MotorConexiones.Core/Sketch/ISketchProvider.cs`, un método `BuildSketch(spec, nodeInfo, limits)` que devuelve
   las primitivas en mm; `limits` es `config/limits.json` por si el croquis muestra valores calculados, como la longitud
   de los pernos), la ventana de previsualización lo dibuja sin cambios; si no, muestra "el tipo no aporta croquis".
   Cada `SketchDimension` lleva la ruta JSON del campo que mide: así el doble clic sobre la cota lo edita.

6. **Guía.** Describe el tipo nuevo en `docs/guide.md` para que la IA sepa cuándo usarlo.

---

## 8. Guion de prueba de punta a punta con la IA

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

## 9. Previsualizar y corregir antes de crear (botón de la cinta)

El botón **Ejecutar especificación JSON** (pestaña ARBA, panel MotorConexiones) ya no muestra un resumen de texto: abre la
ventana **Previsualización de conexión**, que sirve para ver el nudo dibujado con cotas, corregir valores y crear, con o
sin la IA. Funciona así:

1. Pulsa el botón y elige el archivo JSON (por ejemplo `docs\fixtures\detalle-D-confirmado.json`). Si el JSON no trae
   los `element_ids` del nudo y tienes las barras seleccionadas en Revit, se usan las seleccionadas.
2. **Izquierda, croquis 2D** del nudo en el plano de la cercha y en el sistema local (origen en el punto de trabajo, X a
   lo largo del cordón): eje y ancho del cordón y de cada barra, contorno de la cartela, ranuras, placa cuchilla, pernos,
   retiros (el extremo real de cada barra) y soldaduras en rojo. Cotas en mm con una cifra decimal: ancho y alto de la
   cartela, retiro de cada barra, largo de ranura, largo y ancho de la placa cuchilla, paso, borde y primera fila de los
   pernos; el espesor va como etiqueta (`PL 3/8" · 9,5 mm`) y los pernos llevan su propia etiqueta con el agarre y la
   longitud que se crearán (`4 pernos Ø5/8" · agarre 19,5 mm (cartela 9,5 + placa 10,0) · L 44,5 mm · placa en cara
   +z`). Rueda = zoom, botón central (o arrastrar) = encuadre, **Ajustar** = ver todo. Las direcciones y anchos de las
   barras salen del modelo; si el nudo no se puede leer, el croquis avisa de que son aproximadas.
   **Doble clic sobre una cota** (ronda 6b): el cursor se vuelve una mano encima de las cotas; al hacer doble clic
   aparece un cuadro junto a la cota con el nombre del campo, su ruta JSON y el valor actual; escribe el nuevo y pulsa
   Enter (Esc o un clic fuera cancelan). Es lo mismo que editar la fila en la tabla: el croquis se redibuja y se vuelve
   a validar. Las cotas de ancho y alto de la cartela miden el contorno, así que al cambiarlas el contorno se estira en
   ese eje alrededor del punto de trabajo (los puntos nuevos aparecen en el cuadro del contorno) y `width_mm`/`height_mm`
   toman el valor nuevo; la barra de estado lo explica.
3. **Derecha, tabla editable**: cordón, cartela, cada barra (rol, perfil, ángulo, retiro, tipo de unión, ranura o placa
   cuchilla con su cara de la cartela, pernos con su longitud opcional, soldaduras), cadenas de cotas y las **dudas**
   (`uncertain_fields`) con su `user_confirmed_value`.
   Doble clic en un valor, escribir y Enter: el croquis se redibuja y se vuelve a validar. Acepta coma o punto decimal
   (y tras escribir un entero como `9` se puede volver a escribir `12,7`: solo filas, columnas e ids son enteros);
   las listas van separadas por punto y coma (`75; 420; 70`). El contorno de la cartela se edita en el cuadro de abajo
   (un punto por línea, `x; y`) con **Aplicar contorno**. Al seleccionar una fila, su cota se resalta en naranja.
4. **Abajo, estado**: los mismos errores y avisos que ve la IA (código, campo, mensaje y sugerencia), el
   `validation_token` abreviado y los botones:
   - **Recargar**: vuelve a leer el archivo del disco (por si lo corrigió el chat o un editor).
   - **Guardar JSON**: escribe el JSON corregido junto al original con sufijo `-corregido.json`. Nunca escribe encima.
   - **Validar**: repite la validación contra el modelo.
   - **Crear**: solo se activa con la validación en verde. Vuelve a validar, usa el token recién calculado y crea la
     conexión igual que antes: una operación atómica, registro en Extensible Storage y diálogo con el `connection_id`.
   - **Cancelar**: cierra sin tocar el modelo.

Las reglas del contrato siguen mandando: si cambias el espesor de la cartela de 9,525 a 12,7 mm sin cambiar el rótulo
`3/8"`, la validación marca `LABEL_VALUE_MISMATCH` hasta que pongas `1/2"`; si pones un paso de pernos de 10 mm, marca
`BOLT_SPACING_TOO_SMALL` y **Crear** se desactiva. Nada de esto toca las rutas `conn_*`: la IA sigue sin ventanas.

---

## 10. Borrar desde la cinta

El botón **Conexiones del modelo** (mismo panel) abre una ventana con las conexiones creadas por el add-in en el
documento, lo mismo que devuelve `conn_list`: `connection_id`, tipo, fecha, elementos creados, barras modificadas, barras
del nudo y backend. Selecciona una y pulsa **Borrar seleccionada**: tras confirmar, hace lo mismo que `conn_delete`
(borra solo lo que creó el add-in y devuelve a las barras sus extensiones originales) en una operación atómica. Si algo
falla, se deshace todo y la ventana lo dice. **Actualizar** vuelve a leer la lista. El borrado queda en el log como
`ribbon_delete`.

---

## 11. Catálogo de conexiones (Fase 7)

Una **plantilla** es una conexión guardada sin los IDs del nudo (`node`, `chord.element_id`, `members[].element_id`),
con el ángulo real de cada barra medido en el modelo (`member_pattern`: ranura, rol, ángulo con signo, lado, perfil) y
una regla de casado (`matching`: tolerancia de 10° y si se admite espejo). Vive como archivo
`%LOCALAPPDATA%\MotorConexiones\catalogo\<template_id>.json` (carpeta en `config\catalog.json`, `catalog_folder`); la
carpeta `catalog\` del repositorio guarda las "oficiales" y `deploy.ps1` copia al PC las que falten.

**Marco canónico del nudo (decisión P5 de la propuesta).** Desde la Fase 7, `NodeFrame` orienta X hacia +X global y Y
hacia +Z global (hacia arriba en cerchas verticales), Z = X × Y: el mismo nudo da el mismo marco aunque el cordón esté
dibujado al revés, así las plantillas se pueden comparar entre nudos. `conn_get_node_info` devuelve `angle_in_plane_deg`
**con signo** (+45° arriba a la derecha, −135° abajo a la izquierda), `angle_to_chord_deg` (inclinación sin signo, la del
plano) y `side`. `expected_angle_deg` sigue siendo la inclinación del plano: la regla 8.6 compara sin signo. Ojo: en el
Hangar el cordón está dibujado hacia −X, así que el dibujo local se refleja en X respecto a las Fases 3 a 6 (la placa
cuchilla del Detalle D pasa a terminar en el chaflán inferior izquierdo, como en el plano) y `plate.gusset_face` `+z`
pasa a ser la otra cara física; `docs/fases/fase-7.md` lo detalla.

**Desde la IA** (`docs/guide.md`, sección 5): `conn_catalog_save` con el `connection_id` de una conexión creada (o con
`spec`) y un nombre → `template_id`; `conn_catalog_list` y `conn_catalog_get` para ver las plantillas;
`conn_catalog_apply` con `template_id` y las barras del nudo (o la selección) → casa cada barra con una ranura por su
ángulo probando `same`, `mirror_x`, `mirror_y` y `both`, escribe los IDs, transforma la cartela, pone el ángulo real en
`expected_angle_deg`, añade `source.template_id` y **ya valida**: `data.spec` y `data.validation_token` van tal cual a
`conn_preview` y `conn_create`. `TEMPLATE_NO_MATCH` explica en `data.attempts` qué ranura no encontró barra;
`TEMPLATE_ANGLE_DEVIATION` avisa si una barra se desvía más de `angle_deviation_warning_deg` (5°);
`TEMPLATE_PROFILE_DIFFERS` avisa si el perfil es otro (`profile_policy: warn` por defecto; `require` conserva el de la
plantilla y entonces `PROFILE_MISMATCH`; `ignore` escribe el del modelo sin avisar). `conn_catalog_delete` borra el archivo.

**Desde la cinta**: botón **Catálogo** (lista con búsqueda; **Aplicar a la selección** casa la plantilla con las barras
seleccionadas y abre la ventana de previsualización con la especificación instanciada, como archivo virtual cuyo
**Guardar JSON** escribe en `Documentos\MotorConexiones\`; **Crear** hace lo mismo que el botón de la sección 9;
**Guardar desde conexión…** convierte una conexión del modelo en plantilla; **Eliminar**). En la ventana de
previsualización, **Abrir del catálogo** aplica una plantilla al nudo abierto y **Guardar en catálogo** guarda la
especificación actual (solo con la validación en verde). Al guardar se puede marcar "Copiar también a la carpeta
compartida" (`shared_catalog_folder`, por defecto `catalog\` del repositorio).

Lo que **no** hace todavía: calcular la cartela según los ángulos (`outline.mode = "auto"`, Fase 12): la cartela de la
plantilla se copia tal cual y, si una barra se sale, `conn_validate` lo marca. Crear por lotes es la sección 12.

---

## 12. Planificar y crear un lote: los nudos de una cercha entera (Fases 8 y 9)

Con la cercha seleccionada (cordones, diagonales y montantes), el **plan de lote** hace el trabajo repetitivo antes de
crear nada: lleva cada extremo de barra al **corte de su eje con el eje del cordón** (ronda 8b: en la cercha real las
diagonales terminan en la cara del cordón, a 15–85 mm de su eje, y agrupando solo los extremos a 10 mm nada se juntaba;
un extremo se lleva al corte si está a menos de medio canto de cada barra más 10 mm de ese eje, `node_face_reach_mm` de
`config\catalog.json` lo fija a mano), agrupa esos puntos a menos de 10 mm (`node_cluster_mm`), reconoce la barra que
atraviesa cada punto como cordón (5 mm, `node_axis_max_distance_mm`), calcula el marco canónico y el ángulo con signo de
cada barra (y `end_gap_mm`: cuánto se queda corta respecto al punto de trabajo, que es la media de los cortes de las barras
del nudo con el cordón), nombra los nudos `N1, N2…` a lo largo de la cercha, salta los que ya
tienen conexión del add-in
(`replace_existing` para rehacerlos), casa cada nudo con las plantillas del catálogo (también en espejo), instancia la
especificación con `source.template_id` y `source.batch_id` y la valida con las mismas reglas y el mismo token que
`conn_validate`. Estados por nudo: `ready`, `invalid`, `no_match`, `ambiguous_chord`, `offset`, `untyped`,
`already_connected`, `excluded` y, desde la Fase 9, `created` y `failed`. **Planificar no crea ninguna conexión**: crear
es `conn_batch_create` o el botón **Crear N conexiones** (más abajo, "Crear el lote").

**Marcas en el modelo** (opción A de la propuesta): en la vista activa, las barras de cada nudo se colorean **con el color
de su estado** (ronda 8c: verde = se creará, ámbar = se creará con aviso, rojo = falta algo, gris = no se crea; el mismo
color que la ventana; línea y superficie, el cordón con línea gruesa; las barras sueltas y las parejas sin cordón no se
marcan) y en el punto de trabajo se pone un marcador pequeño con el nombre del nudo
(`DirectShape` de Modelos genéricos: cubo = misma orientación que la plantilla, rombo = en espejo; el nombre se ve en
Propiedades > `Comentarios`, que empieza por `N7 · …`; desde la ronda 8b no se escribe `Marca`, porque Revit avisaba
"Elements have duplicate Mark values"). Replanificar renueva las marcas; descartar las quita (y `conn_batch_plan_discard` con
`all: true` limpia marcas de planes que el add-in ya no recuerde y dice cuántos marcadores había). **En un documento solo
hay un plan marcado** (cierre de la Fase 8): marcar un plan nuevo, desde la IA o desde el botón, quita las marcas del
anterior con el aviso `PLAN_MARKS_REPLACED`; ese plan sigue en memoria y se vuelve a ver replanificándolo con su `plan_id`.
**Descartar plan** desde la ventana (0.8.4) hace con los marcadores lo mismo que `all: true`: quita los de cualquier plan y los
huérfanos, sin olvidar los demás planes (siguen en memoria sin marcas); después no queda ningún cubo ni rombo en el documento
y **la ventana se cierra** (cierre de la ronda 8d, 0.8.5: en la 8d se quedaba abierta). Las marcas son cambios de vista y elementos pequeños, nunca acero, y forman una sola entrada de deshacer.

**Desde la IA** (`docs/guide.md`, sección 6): `conn_batch_plan` (selección o `element_ids`, `template_ids` opcional) →
tabla de nudos → correcciones en `overrides` con el mismo `plan_id` (`exclude`/`include`, `chord`, `template`,
`remove_member`/`add_member`, `add_node`, `merge`, `split`, `spec`) → `conn_batch_plan_get` (con `node`, un nudo con su
especificación y su token) → `conn_batch_plan_discard`. `conn_batch_plan` devuelve por defecto los nudos sin la
especificación completa (`include_specs: false`), para no volcar 3 KB por nudo. El `overrides` de una respuesta se puede
devolver tal cual en la petición siguiente (desde 0.8.2 no lleva claves auxiliares). Desde la ronda 8c la respuesta trae
además `summary_text` (la decisión en español, la misma cabecera de la ventana), `visible_count`, `hidden_text` y, por nudo,
`status_text` (estado en español con icono), `advice` (qué hacer) y `visible_by_default`; `color_name` es el del estado. La
guía pide a la IA que enseñe `summary_text` y una tabla corta solo con los nudos visibles, nunca el JSON. Si el catálogo no
tiene plantillas, aviso `CATALOG_EMPTY`.

**Desde la cinta** (ronda 8c): botón **Planificar lote** (sin selección reabre el último plan del documento). La ventana
"MotorConexiones: conectar cercha (plan y lote)" tiene, de arriba abajo:

- **La cabecera con la decisión**: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de
  perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas." Con 0 listos dice "Ningún nudo listo: …" y la causa más
  frecuente. Debajo, el plan, las barras seleccionadas y las plantillas.
- **El mapa de la cercha**: el alzado con las barras en gris (el cordón más grueso) y un círculo por nudo del color de su
  estado con el número dentro. Clic en un círculo = elegir su fila; doble clic = **Ver en Revit**; pasar el ratón = "N4 ·
  Listo con aviso · Nudo tipico Detalle D · igual"; rueda = zoom; arrastrar = mover; **Ajustar** = ver toda la cercha. Los
  ocultos salen como círculos vacíos pequeños solo con **Mostrar ocultos**. El mapa se recalcula al replanificar.
- **La tabla, solo con los nudos de verdad** (los que tienen cordón): Nudo, Estado (● Listo, ▲ Listo con aviso, ✖ No
  valida, ✖ Sin plantilla que encaje, ✖ Falta el cordón, ✖ Dos cordones posibles, ✖ Los ejes no se cortan, ◌ Ya tiene
  conexión, ◌ Excluido, ○ Barra suelta), Espejo (no / sí), Plantilla, Desvío y **Qué hacer** (una frase por caso: "El
  cordón es HSS4X4… y la plantilla HSS3X3…: se creará con la misma cartela; exclúyelo si no quieres", "Falta el cordón en la
  selección: selecciónalo y replanifica, o Cordón…", "Una barra se sale de la cartela: Editar nudo y agrandarla, o
  excluir", "El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o
  excluye"…) **con botones debajo de la frase** (Fase 9, mejora C3: Excluir, Incluir, Incluir (rehacer), Cordón…, Barras…,
  Plantilla…, Editar nudo, Ver en Revit, Abrir catálogo, según el caso; cada uno hace lo mismo que la entrada del menú de
  clic derecho). Las barras sueltas y las parejas sin cordón van **ocultas por defecto**; la línea de encima las cuenta
  ("Ocultos: 20 sin cordón, 23 barras sueltas") y **Mostrar ocultos** las enseña en gris al final. Los nombres `N1…` no
  cambian (decisión P5). El id del cordón, los ids de las barras, cuánto se queda corta cada una (`end_gap_mm`) y el token
  abreviado se ven en el **detalle del nudo** (panel de abajo), no en la tabla.
- **Clic derecho sobre un nudo**: Ver en Revit, Editar nudo, Quitar edición, Excluir/Incluir, Cordón…, Barras…, Plantilla…
  (de una lista o **pinchando en Revit**: mientras pinchas, la ventana del plan se oculta y Revit queda delante; al
  terminar o con Esc vuelve sola, cierre de la ronda 8d). **Más…**: Añadir nudo… (pinchar las barras de
  un nudo que no se detectó), Guardar plan JSON (`Documentos\MotorConexiones\plan-<id>.json`, con el último informe),
  **Borrar el lote** (Fase 9) y Descartar plan (quita todos los colores y marcadores del documento, también los de otros
  planes, y olvida el plan; no toca las conexiones creadas).
- **Botones**: **Replanificar**, **Editar nudo** (abre la previsualización con la especificación del nudo; lo que se cambie
  vale solo para ese nudo; solo con nudos listos o que no validan: el botón en gris lo dice al pasar el ratón), **Ver en
  Revit** (selecciona las barras del nudo y su marcador y hace zoom en la vista activa, **sin ningún cuadro y sin cerrar la
  ventana**; doble clic en el mapa hace lo mismo), **Crear N conexiones** (Fase 9, en negrita; ver "Crear el lote") y
  **Cerrar** (deja las marcas y el plan para seguir después). Leyenda al pie: "Verde = se creará (o creada) · Ámbar = con
  aviso · Rojo = falta algo o falló · Gris = no se crea".
- **Catálogo vacío**: si no hay ninguna plantilla, la cabecera dice "Ningún nudo listo: no hay plantillas en el catálogo…",
  la barra de estado lleva el aviso `CATALOG_EMPTY` y aparece **Abrir catálogo** (al cerrarlo se replanifica).

Todos esos textos salen del Core (`PlanAdvice`), los mismos que recibe la IA. **La ventana se queda abierta** (cierre de la
ronda 8c, 0.8.4; opción B de la decisión P10, mejora C7 adelantada de la Fase 10): orbita, haz zoom y pincha en Revit con la
ventana a un lado. Como fuera de un comando Revit no admite su API, todo lo que toca el modelo (replanificar, marcas,
descartar, Ver en Revit, pinchar, Editar nudo, Abrir catálogo) pasa por un `ExternalEvent` (`Batch/PlanEvents.cs`): mientras
Revit trabaja, los botones se apagan y la barra de estado dice "⏳ …"; al terminar se encienden solos y la ventana se
repinta con lo que devolvió Revit (`PlanSnapshot`). La previsualización (Editar nudo) y el catálogo siguen siendo ventanas
modales cortas, abiertas dentro del evento. Hay una sola ventana del plan por sesión: pulsar **Planificar lote** con otra
selección la actualiza y sin selección la trae delante; si Revit está con una acción de la ventana, el botón lo dice. El
documento activo tiene que ser el del plan. Mientras Revit ejecuta una acción, la ventana no se puede cerrar (Esc cancela
una elección). **Cierre de la ronda 8d (0.8.5)**: en la 8d, **Cordón…** sobre un nudo que ya tenía cordón cerró Revit con
"fatal error"; la causa fue el diálogo de elección (`ChooseDialog`), que en modo de una sola elección marcaba el cordón
actual con `SelectedItems.Add`, cosa que WPF no admite y que, con la ventana no modal, ninguna captura recogía (en la 8c, con
la ventana modal, el mismo fallo solo cerraba la ventana: está en el log de ese día). Ahora el diálogo marca con
`SelectedItem`, **cada manejador de la ventana va dentro de una red** (`Guard`: el error sale en rojo en la barra de estado
y en el log como `ribbon_batch_window_error`, y la ventana sigue), hay otra red en el despachador de WPF que recoge solo las
excepciones de este add-in, **pinchar en Revit** (Cordón…, Barras…, Añadir nudo…) se hace con la ventana del plan **oculta**
y la ventana principal de Revit activada, con una línea en el log antes (`ribbon_batch_pick`) y otra después
(`ribbon_batch_picked` o `ribbon_batch_pick_failed`), y **Descartar plan cierra la ventana** al terminar (en la 8d `Close()`
se llamaba con la ventana todavía ocupada y el propio cierre lo cancelaba). **Todo esto se probó en el PC en la ronda 8e**
(2026-10-05, `resultados-fase-8e.md`; `fase-8.md`, sección 12): Cordón… sobre N9 seis veces y Barras… dos veces con la
ventana oculta y de vuelta, Esc cancela, Excluir / Incluir y Replanificar con la ventana abierta, Descartar cierra la
ventana; Revit siguió vivo y el log no tiene ningún `ribbon_batch_window_error`.

### Crear el lote y borrarlo (Fase 9, add-in 0.9.0, NO PROBADO en Revit)

Con el plan revisado, **Crear N conexiones** (N = nudos listos, más los que fallaron en un intento anterior) abre un cuadro
de confirmación con la cabecera del plan y crea las conexiones **nudo a nudo con los tokens del plan**: cada nudo es la
misma operación atómica que `conn_create` (el token se comprueba contra el modelo; cartela, placas cuchilla, pernos,
soldaduras, retiros, elementos de Advance Steel adoptados, registro con `source.batch_id` = `plan_id` y
`source.template_id`), con su propio `TransactionGroup`, **anidado** en un grupo exterior del lote que se asimila al final:
**si un nudo falla se revierte solo y los demás se quedan** (decisión P9), y en Revit todo el lote es **una sola entrada de
deshacer** (Ctrl+Z lo deshace entero). Mientras Revit trabaja la ventana dice "⏳ Creando N conexiones…" (cada nudo abre
su sesión de Advance Steel: varios segundos por nudo). Al terminar, la cabecera dice "Creadas 15 conexiones (14 con
aviso), 1 falló", la tabla enseña `✔ Creada`, `▲`→`✔ Creada con aviso` y `✖ Falló al crear` con el motivo en *Qué hacer*
("Falló al crear (FABRICATION_FAILED: …): corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye"), la barra
de estado lleva el informe ("Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: …). Una sola entrada de
deshacer (Ctrl+Z). 48,2 s.") y las marcas (color y marcador) de los nudos creados desaparecen; las de los demás siguen.
Los nudos `invalid`, `no_match` (también los empalmes), `excluded`, `untyped` y `already_connected` no se crean nunca
(decisión de la persona: los 10 nudos del cordón superior sin plantilla y los 7 empalmes quedan fuera); los "Listo con
aviso" por perfil distinto **sí** (P6: los 14 nudos con cordón HSS4X4 llevan la cartela del Detalle D). **Borrar el lote**
(botón y **Más…**) pide confirmación y borra todas las conexiones cuyo `source.batch_id` es el del plan, una a una con
las garantías de `conn_delete` (solo lo que creó el add-in; las barras recuperan su extensión), en una sola entrada de
deshacer, y replanifica. Si se replanifica con el lote creado, esos nudos salen `◌ Ya creada en este lote` con los
botones Ver en Revit e Incluir (rehacer). Desde la IA: `conn_batch_create` (`plan_id`, `nodes` con `validation_token`,
`stop_on_error`) devuelve el mismo informe por nudo (`outcome` `created` / `created_with_warnings` / `updated` /
`failed` / `skipped` / `rolled_back`, `summary_text`); `conn_batch_delete` (`batch_id`) borra el lote; `conn_list`
enseña `batch_id` por conexión y filtra por lote (`docs/guide.md`, sección 6). `config/catalog.json` lleva
`batch_single_undo`: `true` (grupo exterior, una entrada de deshacer) o `false` (plan B de la propuesta: una entrada por
nudo, por si el sondeo 20 dijera que los grupos anidados no conviven con Advance Steel). Todo esto está **NO PROBADO en
Revit**: `docs/instalacion/fase-9.md` lo prueba sobre una copia del modelo, primero con el sondeo 20 (grupos anidados:
crear y borrar el Detalle D dentro de un grupo exterior, asimilar y revertir), después con **Crear 16 conexiones**,
Ctrl+Z, **Borrar el lote** y los sondeos 12 y 13 a cero.

---

## 13. Cómo probar sin Revit

Lo que se puede ejecutar en cualquier máquina (Linux, macOS o Windows) sin Revit ni pyRevit:

```bash
dotnet build MotorConexiones.sln -c Release          # Core, Revit y Tests (0 avisos)
dotnet test MotorConexiones.sln -c Release --no-build  # 192 pruebas del Core (croquis, editor, pernos, catálogo, plan de lote, cerchas del Hangar, textos y mapa del plan, crear por lotes)
python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py
```

**`mcp/pruebas/simulador_revit.py`** (solo para desarrollo; nada lo copia a la extensión) carga el `conexiones.py` real
con módulos `pyrevit`, `clr` y `System` simulados, sustituye `Bridge.Handle` por una imitación en Python del add-in
(mismas operaciones y códigos de error, datos del nudo del Detalle D) y sirve las rutas `/revit_mcp/conn/...` en el
puerto 48884 con token, igual que pyRevit Routes. Con `--extension <clon de revit-mcp>` usa el `seguridad.py` real.

```bash
# 1) Autocomprobación en proceso: 25 rutas, token, dev_exec, acentos, catálogo, plan de lote, catálogo vacío, crear y borrar el lote (62 comprobaciones)
python3 mcp/pruebas/simulador_revit.py --autocomprobar

# 2) Servir el simulador y pasarle el script de pruebas (28 pruebas)
python3 mcp/pruebas/simulador_revit.py &        # escribe el token en el archivo literal "%LOCALAPPDATA%\RevitMcp\token"
python3 mcp/pruebas/probar_conexiones.py        #   de la carpeta actual, que es el que abre el script en Linux

# 3) Con el puente real (30 pruebas): clon de revit-mcp con conn_tools.py copiado y registrado en tools/__init__.py,
#    un venv con "mcp[cli]>=2.2,<3" y httpx, y "python main.py --streamable-http" en la carpeta del clon
python3 mcp/pruebas/probar_conexiones.py --puente
```

Lo que prueba de verdad: el adaptador, la forma de las peticiones, el script de pruebas, las 23 herramientas y el puente.
Lo que **no** prueba: nada de lo que pasa dentro de Revit (geometría, Advance Steel, almacenamiento). Eso solo lo prueba
el instalador en el PC con `docs/instalacion/fase-N.md`.

---

## 14. Errores más comunes

| Código | Significado | Qué hacer |
|---|---|---|
| `VALIDATION_TOKEN_INVALID` | Falta el token o no coincide: la especificación, el documento, las barras del nudo o `limits.json` cambiaron desde `conn_validate`. No caduca por tiempo. | Volver a llamar a `conn_validate` con la misma especificación y usar el token nuevo. |
| `UNRESOLVED_UNCERTAINTY` | Hay entradas en `uncertain_fields` sin `user_confirmed_value`. | Preguntar al usuario y poner el valor confirmado. |
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
| `TEMPLATE_NOT_FOUND`, `TEMPLATE_EXISTS`, `TEMPLATE_INVALID` | Catálogo (Fase 7): el `template_id` no existe, ya hay una plantilla con ese nombre (falta `overwrite: true`) o el archivo no se puede leer. | `conn_catalog_list`; otro nombre o `overwrite`; corregir o borrar el archivo. |
| `TEMPLATE_SPEC_INVALID`, `TEMPLATE_HAS_OPEN_UNCERTAINTIES` | Lo que se quiere guardar como plantilla no valida o tiene dudas sin confirmar. | Corregir con `conn_validate`; confirmar las dudas. |
| `TEMPLATE_NO_MATCH` | Ninguna orientación casa todas las ranuras con las barras seleccionadas. | Revisar la selección (`data.attempts` dice qué falta) o usar otra plantilla. |
| `PLAN_NOT_FOUND` | Plan de lote (Fase 8): no hay ningún plan con ese `plan_id` en memoria (se descartó o Revit se reinició). | Volver a `conn_batch_plan` con la cercha seleccionada; `conn_batch_plan_discard` con `all: true` si quedaron marcas. |
| `PLAN_MARKS_SKIPPED` (aviso) | La vista activa no admite colores por elemento (plantilla de vista, plano). | Abrir una vista 3D y replanificar. |
| `PLAN_MARKS_REPLACED` (aviso, 0.8.2) | Al marcar este plan se quitaron las marcas de otro plan del mismo documento (sigue en memoria sin marcas): en un documento solo hay un plan marcado. | Nada; para volver a ver el otro plan, replanificarlo con su `plan_id`; para olvidarlo, `conn_batch_plan_discard`. |
| `CATALOG_EMPTY` (aviso, ronda 8c) | El catálogo no tiene ninguna plantilla: ningún nudo puede casar y todos salen `no_match` ("✖ Sin plantillas en el catálogo"). | Crear primero la conexión de un nudo y guardarla con **Guardar en catálogo** (o `conn_catalog_save`); la ventana ofrece **Abrir catálogo**. |
| `NODE_CHORD_NOT_CONTINUOUS` (aviso por nudo, ronda 8b) | Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan. | Si es un extremo de cercha, nada; si falta el cordón en la selección, seleccionarlo y replanificar (o `overrides.chord`); si es un empalme (otro tramo del cordón sigue por el otro lado), ese nudo no tiene plantilla: crear su típica o excluir. |
| `BATCH_STOPPED` (Fase 9) | `conn_batch_create` con `stop_on_error: true`: un nudo falló y el lote entero se revirtió. El informe va en `data`. | Mirar `data.nodes` (el nudo con `outcome: failed` y sus `errors`), corregir o excluir ese nudo y volver a crear. |
| `BATCH_NODE_FAILED` (aviso, Fase 9) | Un nudo del lote falló y se revirtió solo; los demás siguen. | En el plan queda `failed` con su especificación y su token: corregir y pulsar Crear otra vez (solo crea los que faltan), o excluir. |
| `BATCH_EMPTY` (aviso, Fase 9) | `conn_batch_delete` no encontró ninguna conexión con ese `batch_id`. | `conn_list` enseña el `batch_id` de cada conexión y `batches` cuenta por lote. |
| `BATCH_UNDO_SPLIT` (aviso, Fase 9) | El grupo exterior del lote no se pudo abrir: cada nudo quedó como una entrada de deshacer propia. | Nada que corregir en el modelo; anotarlo (es el plan B de la propuesta, `batch_single_undo: false`). |

La tabla completa, con `path`, `message` y `hint`, está en `mcp/CONTRATO-conn.md` y en `docs/guide.md`.

---

## 15. Licencia

No se ha definido una licencia. Uso interno del autor del repositorio.
