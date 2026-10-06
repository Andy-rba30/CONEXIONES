# Fase 10: cercha sin dolor (selección asistida, etiquetas pinchables y, si cabe, cartelas fantasma y el encargo para IA)

Primera fase que sale de `docs/propuestas/flujo-intuitivo.md` después de la Fase 9 (sección 7 de la propuesta: "10. Cercha
sin dolor"). Se ejecuta en **una sesión**, con el prompt de la sección 1. Lo demás de este archivo es el alcance que esa
sesión debe cumplir, escrito a partir de las mejoras **C6** (selección asistida) y **V3** (etiquetas con número en la vista,
pinchables) de la propuesta, de la mejora **V2** (cartelas fantasma) si cabe, de la ronda **9b** de
`docs/propuestas/encargo-ia-externa.md` si cabe, y de lo que la Fase 9 dejó anotado para esta (`docs/fases/fase-9.md`:
7.2 punto 3, 7.6 y 7.8). C7 (ventana que se queda abierta) ya está hecha y probada desde el cierre de la 8c.

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/propuestas/flujo-intuitivo.md, docs/fases/fase-9.md (secciones 7.6 y 7.8) y
docs/propuestas/encargo-ia-externa.md. Escribe docs/prompts/fase-10.md (C6 selección asistida, V3 etiquetas pinchables
sobre el sondeo 19 v4 con un manejador que nunca abre un cuadro, y V2 cartelas fantasma si cabe sin recortar lo anterior)
y ejecuta solo la Fase 10 con el add-in 0.10.0. Si queda sitio en la sesión, incluye también la ronda 9b de
docs/propuestas/encargo-ia-externa.md (botón Encargo para IA en la cinta, un solo archivo .md copiado al portapapeles;
sin herramienta MCP por ahora; con un solo tipo de conexión no hace falta preguntar el tipo); si no cabe, déjalo
anotado como pendiente con lo que falte. Incluye en docs/instalacion/fase-10.md lo pendiente de fase-9.md 7.6 (Borrar el
lote desde la ventana, --puente 30/30, PLAN_MARKS_REPLACED, Editar nudo con la ventana abierta) y corrige el texto del
aviso del borrado (fase-9.md 7.2, punto 3). Las etiquetas de la vista no abren nunca un cuadro: al pinchar una, la
ventana del plan selecciona esa fila y escribe en su barra de estado. Termina con compilación sin avisos, pruebas en
verde (Core y simulador), docs/fases/fase-10.md, docs/instalacion/fase-10.md, README, docs/guide.md, commit, push y un
resumen corto guardado también en docs/fases/resumen-fase-10.md.
```

## 2. Decisiones de la persona (fijadas antes de empezar)

| Pregunta | Decisión | Qué hace la Fase 10 |
|---|---|---|
| V3: qué pasa al pinchar una etiqueta en la vista | **Nunca un cuadro.** La etiqueta cambia de aspecto, la ventana del plan elige esa fila y escribe en su barra de estado | El manejador de clics (`ITemporaryGraphicsHandler`) solo llama a `UpdateControl`, a la ventana y al log. Sin ventana abierta, solo cambia la etiqueta y anota el clic |
| V3: base técnica | El sondeo 19 **v4** y 19b, que funcionaron enteros en la instalación de la Fase 9 | BMP de 24 bits de 32×32 en una ruta sin tildes ni espacios, `TemporaryGraphicsManager.AddControl` en la vista marcada, `SetTooltip`, `UpdateControl` para el cambio de aspecto, `RemoveControl` al quitar las marcas |
| V2 (cartelas fantasma) | Solo si cabe **sin recortar** C6, V3, la corrección del aviso ni la documentación | Si no cabe, se queda en la propuesta y el informe lo dice |
| Ronda 9b (encargo para IA) | Si queda sitio: **un solo archivo `.md`** (P1), copiado al portapapeles; **sin herramienta MCP** por ahora (P2); con un solo tipo de conexión **no se pregunta el tipo** (P3) | Botón **Encargo para IA** en la cinta; el texto lo escribe el Core (se prueba sin Revit); si no cabe, pendiente anotado con lo que falte |
| Lo pendiente de la Fase 9 (7.6) | Va en la instalación de esta fase | Borrar el lote desde la ventana, `--puente` entero, `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta, Ctrl+Z devuelve las marcas, el texto de los `REVIT_WARNING` |
| El aviso del borrado (7.2, punto 3) | Se corrige con la versión nueva | Al borrar, si Advance Steel no abre su sesión, el aviso dice que los elementos se borran directamente (no "se crea con DirectShape") |

## 3. Alcance (lo que se entrega)

### 3.1 Flujo completo (lo que cambia para la persona)

```
1. SELECCIONAR  (Fase 10, C6)  pincha UNA barra de la cercha (o varias) y pulsa Planificar lote: el add-in añade las
                               barras que la tocan (cordones que pasan de largo, diagonales y montantes que llegan, el
                               otro tramo del cordón en un empalme) dentro del plano de la cercha, dice cuántas añadió
                               y planifica con todas. También desde la ventana (Más… > Completar selección) y desde la
                               IA (conn_batch_plan con expand_selection: true).
2. PLANIFICAR   (Fase 8)       como siempre; además, en la vista, cada nudo visible lleva una ETIQUETA con su número y el
                               color de su estado (Fase 10, V3), pegada al punto de trabajo, que se puede pinchar.
3. REVISAR      (Fase 8/9)     pinchar una etiqueta en Revit = elegir esa fila en la ventana (y la etiqueta se resalta);
                               elegir una fila en la ventana = resaltar su etiqueta en la vista. Ningún cuadro.
4. CREAR        (Fase 9)       igual; las etiquetas de los nudos creados desaparecen con sus marcas.
5. DESCARTAR    (Fase 8)       quita colores, marcadores y etiquetas; no deja ninguna.
```

### 3.2 C6: selección asistida (Core, se prueba sin Revit)

- **`Batch/SelectionAssist.cs`** (Core): `SelectionAssist.Expand(selectedIds, candidates, detectorOptions, options)` recibe
  las barras seleccionadas y **todas las barras candidatas** del documento (como `DetectorBar`, las lee el add-in) y
  devuelve una `SelectionExpansion` con las barras añadidas, por qué cada una (`chord` = cordón que pasa de largo por el
  extremo de una barra de la selección, `member` = barra que llega a una barra de la selección, `splice` = otro tramo del
  mismo cordón que empieza donde termina uno de la selección), las descartadas por estar fuera del plano de la cercha, el
  plano usado, las rondas y un `SummaryText` en español ("Se añadieron 8 barras que tocan la selección: 2 cordones que
  pasan de largo, 5 barras que llegan y 1 tramo de cordón. 3 barras fuera del plano de la cercha no se añadieron." / "La
  selección ya está completa: ninguna barra más la toca.").
- **Regla de "se tocan"**: la misma del detector (ronda 8b): el extremo de una barra se lleva al corte de su eje con el eje
  de la otra cuando ambos ejes se cortan (5 mm, `node_axis_max_distance_mm`) y el extremo queda a menos del **alcance de
  cara** (medio canto de cada barra más `node_cluster_mm`, o `node_face_reach_mm`) de ese eje (`NodeDetector.SnapEnd`). Un
  tramo paralelo cuyo extremo está a menos del alcance del extremo de otro es un empalme. Se repite por rondas hasta que
  no se añade nada (la cercha entera a partir de una barra) o se llega al tope (`max_bars`, 400) con aviso.
- **Plano de la cercha**: solo se añaden barras con los dos extremos a menos de 50 mm del plano de la cercha (correas,
  arriostrados y riostras entre cerchas quedan fuera). Con una sola barra seleccionada el plano no se conoce todavía: se
  decide **por votación** entre las barras que la tocan (el plano que contiene la barra y a más de las que la tocan); los
  tramos paralelos no votan.
- **Add-in** (`BatchPlanner.ExpandSelection`): lee como candidatas todas las barras de armazón estructural con eje del
  documento (`RevitModelFacts`) y llama al Core. Desde el botón **Planificar lote** (con una barra o más seleccionadas): si
  la asistida añade barras, un cuadro de la cinta (el único sitio con diálogos) dice cuántas y por qué y ofrece
  **Planificar con todas**, **Solo la selección** o **Cancelar**; si no añade ninguna, planifica directamente. Con una
  sola barra y nada que añadir, dice que no encontró ninguna barra que la toque. Desde la ventana: **Más… > Completar
  selección** (replanifica el mismo plan con la selección ampliada; la barra de estado dice qué añadió). Desde la IA:
  `conn_batch_plan` con `expand_selection: true` (por defecto `false`: el contrato no cambia) devuelve
  `data.selection_expansion` {`added_count`, `chord_count`, `member_count`, `splice_count`, `added_element_ids`,
  `skipped_out_of_plane`, `limit_reached`, `summary_text`} y el aviso `SELECTION_EXPANDED` con el mismo texto; el plan se
  calcula con la selección ampliada (`selection_count` la cuenta).

### 3.3 V3: etiquetas con número en la vista, pinchables (add-in, sobre el sondeo 19 v4)

- **`Batch/LabelImage.cs`** (Core): escribe un **BMP de 24 bits de 32×32** (sin `System.Drawing`: a mano, como la v3 del
  sondeo) con un círculo del color del estado (los RGB de `PlanAdvice`), un anillo blanco y la cifra del nudo en blanco
  (fuente de píxeles de 5×7, hasta tres cifras). La versión **resaltada** (pinchada o elegida en la ventana) es la inversa:
  círculo blanco, anillo grueso y cifra del color del estado. Nombre de archivo estable por color, cifra y estado
  (`etiqueta-verde-4.bmp`, `etiqueta-verde-4-sel.bmp`): se escribe una vez y se reutiliza. Se prueba en la nube (cabecera,
  tamaño, píxeles).
- **`Batch/PlanLabels.cs`** (add-in): al marcar un plan (`BatchPlanner.Plan` con `mark`), después de la transacción de las
  marcas, pone una etiqueta por nudo visible (`CanBeMarked`) con `TemporaryGraphicsManager.AddControl(InCanvasControlData(ruta,
  punto de trabajo), vista marcada)` y `SetTooltip` con el globo del mapa ("N4 · Listo · Detalle D · igual"); guarda en el
  plan la vista y el índice de cada nudo (`label_view_id`, `label_indices`, `nodes[].label_index`: el contrato solo
  **añade** claves). La carpeta de los BMP es la primera sin tildes ni espacios de: `%LOCALAPPDATA%\MotorConexiones\etiquetas`,
  `%PUBLIC%\MotorConexiones\etiquetas`, `C:\MotorConexiones\etiquetas`, la temporal (anotada en el log). Las etiquetas se
  quitan (`RemoveControl`, solo las que `GetAll()` dice que existen) al replanificar (se renuevan), al crear el lote (las de
  los nudos creados, con sus marcas), al descartar y con `conn_batch_plan_discard`; con `all: true` y con **Descartar plan**
  de la ventana, además, `Clear()` (no queda ninguna etiqueta en el documento, como con los marcadores). Si algo falla al
  poner las etiquetas, el plan sigue sin ellas con el aviso `PLAN_LABELS_SKIPPED` y una línea en el log; nunca impide
  planificar. `config/catalog.json` lleva `plan_labels` (`true`; `false` = sin etiquetas, sin recompilar) y la petición
  admite `labels: false`.
- **Manejador de clics sin cuadro** (`Batch/PlanLabelHandler.cs`): un `ITemporaryGraphicsHandler` registrado una vez en el
  servicio `TemporaryGraphicsHandlerService` (en el arranque del add-in y, por si acaso, en el botón Planificar lote), que
  **nunca abre un cuadro**: con el índice y el documento del clic busca el plan y el nudo, **resalta esa etiqueta** con
  `UpdateControl` (y devuelve a su aspecto la que estaba resaltada), anota `label_clicked` en el log y, si la ventana del plan
  está abierta con ese plan, **elige su fila** (enseñándola si estaba oculta), pinta su detalle y escribe en la barra de
  estado "Etiqueta 4 pinchada en la vista: N4 · Listo · Nudo tipico Detalle D · igual (Ver en Revit encuadra)". Cualquier
  excepción se captura y va al log (`label_click_failed`): el hilo de Revit nunca recibe una excepción del manejador.
- **Ventana → vista**: elegir una fila en la tabla o en el mapa resalta su etiqueta en Revit por el `ExternalEvent` de la
  ventana (sin apagar los botones ni cambiar la barra de estado). La cabecera del plan dice "marcas y etiquetas puestas".

### 3.4 V2: cartelas fantasma (solo si cabe sin recortar lo anterior)

Un `DirectShape` transparente (Modelos genéricos, el mismo `ApplicationId` de los marcadores con `ApplicationDataId`
`<plan>:<nudo>:ghost`) con el contorno de la cartela de la especificación de cada nudo listo (y con aviso), en el marco del
nudo, coloreado por estado, creado en la misma transacción de las marcas; **Crear** lo quita con las marcas del nudo
creado y **Descartar** lo quita siempre (también los huérfanos, por `ApplicationId`). Si no cabe en la sesión, se queda en
`docs/propuestas/flujo-intuitivo.md` y el informe lo dice.

### 3.5 Ronda 9b: botón **Encargo para IA** (solo si queda sitio)

- **`Brief/DesignBriefWriter.cs`** (Core): escribe el encargo en **un solo Markdown**, en este orden: el prompt de la
  sección 3 de la propuesta, los datos del nudo (el JSON de `node_info` tal cual), el esquema (`data.example` del tipo), las
  reglas de lectura (secciones 2, 3 y 4 de `docs/guide.md`, copiadas del archivo desplegado) y un ejemplo confirmado (la
  plantilla del catálogo con la misma cantidad de barras o, si no hay, `detalle-D-confirmado.json` embebido en el Core).
  Nombre de archivo `encargo-<documento>-<fecha-hora>.md`. Se prueba en la nube.
- **`DesignBriefCommand.cs`** (add-in): con el nudo seleccionado (cordón y barras), calcula `node_info` con el mismo código
  que `conn_get_node_info` (`Bridge.Handle`), escribe el archivo en `%LOCALAPPDATA%\MotorConexiones\encargos\`, lo copia al
  portapapeles, abre la carpeta en el Explorador y lo dice en un cuadro corto. Con un solo tipo de conexión no pregunta el
  tipo. Sin herramienta MCP.

### 3.6 La corrección del aviso del borrado (`fase-9.md` 7.2, punto 3)

`AdvanceSteelBackend` sabe si la sesión es para crear o para borrar (`BeginSession(document, name, forDeletion)`): al
borrar, si no puede abrir la `FabricationTransaction` ("cannot start a fabrication transaction while asynchronous
fabrication tasks are queued"), el aviso dice "… Los elementos se borran directamente (Document.Delete); las barras se
restauran igual.", nunca "Toda la conexión se crea con DirectShape".

### 3.7 Herramientas, rutas y guía

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_batch_plan` | Argumentos nuevos `expand_selection` (`false`) y `labels` (`true`). Devuelve `selection_expansion` (o `null`), `label_view_id`, `label_indices` y `labels` {`count`, `view_id`} en el plan; aviso `SELECTION_EXPANDED`; aviso `PLAN_LABELS_SKIPPED`. |
| MCP | `conn_batch_plan_discard` | Quita también las etiquetas (`removed_labels`). |
| Rutas | sin rutas nuevas | 25 rutas; 23 herramientas. Versión 0.10.0 en adaptador, herramientas y simulador. |
| Cinta | **Planificar lote** con selección asistida; **Encargo para IA** (9b) | Sección 3.2 y 3.5. |
| Ventana | **Más… > Completar selección**; clic en etiqueta ↔ fila | Sección 3.2 y 3.3. |
| Guía | `docs/guide.md`, sección 6 | `expand_selection` (pedir una barra basta), las etiquetas, `labels: false`. |

### 3.8 Arquitectura (solo lo de esta fase)

```
src/MotorConexiones.Core/Batch/
├── SelectionAssist.cs          C6: barras que se tocan (regla del detector), plano por votación, rondas, SummaryText
├── LabelImage.cs               V3: BMP de 24 bits de 32×32 con círculo de color y cifra (fuente 5×7); normal y resaltada
├── BatchPlan.cs                label_view_id, label_indices; PlanNode.label_index
└── PlanAdvice.cs               texto de la cabecera con etiquetas (sin cambios de estados)
src/MotorConexiones.Core/Brief/DesignBriefWriter.cs     9b: el encargo en Markdown (si cabe)
src/MotorConexiones.Core/Catalog/CatalogConfig.cs       plan_labels

src/MotorConexiones.Revit/
├── Batch/PlanLabels.cs         AddControl / SetTooltip / UpdateControl / RemoveControl / Clear; carpeta de los BMP; resaltar
├── Batch/PlanLabelHandler.cs   ITemporaryGraphicsHandler sin cuadro: resalta, log, ventana (fila + barra de estado)
├── Batch/BatchPlanner.cs       ExpandSelection (candidatas del documento), etiquetas tras las marcas, quitar al descartar
├── Batch/BatchCreator.cs       quitar las etiquetas de los nudos creados
├── BatchPlanCommand.cs         selección asistida con cuadro de opciones (Planificar con todas / Solo la selección / Cancelar)
├── DesignBriefCommand.cs       9b: Encargo para IA (si cabe)
├── Fabrication/AdvanceSteelBackend.cs   aviso del borrado
└── UI/BatchPlanWindow          Completar selección, OnLabelClicked (fila + barra de estado), resaltar al elegir fila

mcp/revit_mcp/conexiones.py     0.10.0 (25 rutas); tools/conn_tools.py 0.10.0 (23 herramientas; expand_selection, labels);
                                simulador con selection_expansion y etiquetas; probar_conexiones con la prueba de expand_selection
config/catalog.json             plan_labels (true)
scripts/sondeos/21-etiquetas-addin.py   cuenta los controles del lienzo y los servidores del servicio; -Limpiar
src/MotorConexiones.Tests/      SelectionAssistTests, LabelImageTests, DesignBriefTests (si cabe)
```

## 4. Reglas técnicas de esta fase

- **Ninguna ventana en las rutas `conn_*`** y **ningún cuadro desde el manejador de clics** (el cuadro modal del sondeo 19
  v2 dejó a Revit sin atender a pyRevit un cuarto de hora). El único cuadro nuevo es el de la selección asistida en el
  botón de la cinta (y el del encargo, si entra la 9b).
- Las etiquetas **no son elementos del modelo**: no pasan por transacciones, no entran en Deshacer, se van al cerrar el
  documento. Por eso se quitan explícitamente al descartar y con el lote, y `discard all` / Descartar plan llaman a `Clear()`.
- Una excepción al poner o quitar etiquetas nunca tira el plan ni el lote: aviso y log.
- La selección asistida solo **lee** el modelo; planificar sigue siendo la misma operación (marcas, nunca acero).
- El contrato del plan solo **añade**: `selection_expansion`, `label_view_id`, `label_indices`, `labels`, `nodes[].label_index`,
  los avisos `SELECTION_EXPANDED` y `PLAN_LABELS_SKIPPED`, y los argumentos `expand_selection` y `labels`.
- `ElementId.Value`; miembros de la API comprobados compilando contra 2027 (`TemporaryGraphicsManager`, `InCanvasControlData`,
  `ITemporaryGraphicsHandler`, `TemporaryGraphicsCommandData`, `MultiServerService`), que el sondeo 19 v4 ya ejecutó en el PC.
- Unidades solo en `Units/UnitConverter.cs`. Código y claves JSON en inglés; textos en español.
- Versión **0.10.0** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador.

## 5. Pruebas en la nube

- `SelectionAssistTests`: con la cercha sintética, desde **una** diagonal se añaden el cordón central (`chord`), las demás
  diagonales y los cordones superior e inferior, y quedan fuera la barra suelta y la que cruza en Y (fuera del plano); desde
  el cordón central solo, se añaden las diagonales por rondas; con la cercha de la 8b (56 barras) desde una diagonal se
  recuperan las 56 (los tramos del cordón por empalme o por las diagonales) y el plan vuelve a dar 16 listos; una selección
  completa no añade nada ("ya está completa"); el tope de barras avisa; `SummaryText` exacto.
- `LabelImageTests`: 3126 bytes, cabecera `BM`, 32×32, 24 bits; el centro del círculo lleva el color del estado (o blanco en
  la resaltada); las esquinas son blancas; la cifra pinta píxeles blancos en el centro; nombres de archivo estables; "N4" → "4",
  "N12" → "12", "N5-2" → "5".
- `DesignBriefTests` (si entra la 9b): el encargo lleva el prompt, el JSON del nudo, el esquema, las secciones 2 a 4 de la
  guía y el ejemplo embebido; nombre de archivo sin caracteres prohibidos.
- `simulador_revit.py --autocomprobar` con `expand_selection` y `labels`; `probar_conexiones.py` con la prueba de
  `expand_selection: true` desde una sola barra del fixture (el nudo listo igual). `py_compile` de `mcp/` y `scripts/sondeos/`.
- `dotnet build` sin avisos, `dotnet test` en verde.

## 6. Instrucciones para el instalador (`docs/instalacion/fase-10.md`)

Sobre la **copia** del modelo, con `Anota` y resultados en `docs/fases/resultados-fase-10.md`:

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado (`0.10.0.0`), `instalar-conn.ps1` (25 rutas, 23 herramientas);
   `catalog.json` desplegado con `plan_labels: true`.
2. Abrir Revit: `ping` en `0.10.0`; sondeos 12, 13 y 17 a cero; **sondeo 21** (etiquetas del add-in: 0 controles antes).
3. **(la persona) Selección asistida**: pinchar **una** diagonal de la cercha de la 8c y **Planificar lote**: el cuadro dice
   cuántas barras añadió (se esperan las 63 restantes de las 64, o las 56 de la cercha de la 8b si no se modelaron los
   cordones superior e inferior) y **Planificar con todas**; los mismos 16 listos de siempre. Captura del cuadro.
4. **(la persona) Etiquetas**: una etiqueta con el número y el color por cada nudo visible; pinchar la 4: la etiqueta se
   resalta, la ventana elige N4 y la barra de estado lo dice, **sin ningún cuadro**; elegir N7 en la tabla: la 7 se resalta
   y la 4 vuelve. Captura. Orbitar con la ventana abierta y volver a pinchar.
5. Lo pendiente de la Fase 9 (7.6): **Editar nudo** con la ventana abierta; **Planificar lote** desde el puente y después
   desde la ventana → `PLAN_MARKS_REPLACED` en la barra de estado y un solo juego de marcas y etiquetas; **Crear 16
   conexiones** → las etiquetas de los creados desaparecen; **Ctrl+Z** → las marcas vuelven (las etiquetas no: Replanificar
   las repone); crear otra vez y **Borrar el lote desde la ventana** (cuadro de confirmación, barra de estado "N conexiones
   borradas", replanificación), sondeos 12 y 13 a cero; el texto del aviso del borrado seguido (sondeo 12 con 16) y el de
   los `REVIT_WARNING` de un `conn_create` en la {3D}.
6. Por el puente: `batch_plan` con `expand_selection: true` desde **una** barra (`selection_expansion` y el aviso),
   `batch_plan_get` con `label_indices`, `batch_plan_discard` con `removed_labels`; sondeo 21 a cero tras descartar.
7. (9b, si entró) **Encargo para IA** con el Detalle D seleccionado: el archivo en `encargos\`, el portapapeles, la carpeta
   abierta; pegar el encargo en Claude con la imagen del detalle y guardar el JSON; **Ejecutar especificación JSON** con él.
8. `discard all`, sondeos 17, 21, 12 y 13 a cero; `probar_conexiones.py --puente` entero (32/32: dos pruebas nuevas, la 29 y
   la 30, y las del puente pasan a 31 y 32) y el log del día.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; `simulador_revit.py --autocomprobar`;
  `probar_conexiones.py` contra el simulador; `py_compile` de todo `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-10.md` según el Anexo B del encargo, con la lista de lo NO PROBADO (todo lo de Revit: las etiquetas en
  el lienzo desde el add-in, el manejador, la selección asistida sobre el modelo real, el aviso del borrado).
- `docs/instalacion/fase-10.md` literal, con `Anota`, capturas con nombre fijo, lo pendiente de `fase-9.md` 7.6 y qué devolver.
- README (estado, tabla de garantías, árbol, sección 12, errores), `mcp/CONTRATO-conn.md`, `docs/guide.md` (sección 6),
  `CLAUDE.md` (estructura), `docs/propuestas/flujo-intuitivo.md` (C6 y V3 hechos; V2 si entró o no) y
  `docs/propuestas/encargo-ia-externa.md` (9b si entró o no) al día.
- `docs/fases/resumen-fase-10.md` con el resumen del chat.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta fase

- Botón **Conectar** para un nudo suelto, miniaturas (V4), plano al lado (V5): Fase 11. Edición arrastrando (V6): Fase 12.
- Herramienta MCP `conn_design_brief` (P2 de la propuesta del encargo): más adelante, cuando el agente local la necesite.
- `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`, la cartela automática y la placa de extremo
  (`docs/propuestas/placa-de-extremo.md`).
