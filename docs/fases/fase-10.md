# Fase 10: cercha sin dolor (selección asistida, etiquetas pinchables, cartelas fantasma y el encargo para IA)

Fecha: 2026-10-06. Rama: `main`. Add-in **0.10.0** (Core con `Batch/SelectionAssist`, `Batch/LabelImage` y
`Brief/DesignBriefWriter`; add-in con `Batch/PlanLabels` + `PlanLabelHandler`, las cartelas fantasma en `PlanMarks`, la
selección asistida en `BatchPlanner` / `BatchPlanCommand` / la ventana, y `DesignBriefCommand`), adaptador 0.10.0 (25 rutas,
sin rutas nuevas) y 23 herramientas `conn_*` (`conn_batch_plan` con `expand_selection` y `labels`). Prompt y alcance:
`docs/prompts/fase-10.md`, escrito a partir de las mejoras C6, V3 y V2 de `docs/propuestas/flujo-intuitivo.md`, de la ronda
9b de `docs/propuestas/encargo-ia-externa.md` y de lo que la Fase 9 dejó anotado (`docs/fases/fase-9.md`: 7.2 punto 3, 7.6 y 7.8).

**Estado: programada y probada en la nube; NO PROBADA en Revit.** Se prueba en el PC con `docs/instalacion/fase-10.md`
(unos 90 minutos sobre la copia), que incluye lo pendiente de la Fase 9 (7.6): Borrar el lote desde la ventana, `--puente`
entero, `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta, Ctrl+Z devuelve las marcas, el texto de los
`REVIT_WARNING` y el aviso del borrado seguido.

Decisiones de la persona, fijadas en el prompt: las etiquetas **nunca abren un cuadro** (al pinchar una, la ventana del plan
elige esa fila y escribe en su barra de estado); V2 y la ronda 9b solo si cabían sin recortar lo anterior (**entraron las
dos**); el encargo para IA en **un solo `.md`** copiado al portapapeles, **sin herramienta MCP** y **sin preguntar el tipo**.

---

## 1. Qué se hizo

- **Core, `Batch/SelectionAssist.cs` (C6, selección asistida)**: `SelectionAssist.Expand(selección, candidatas, opciones del
  detector, opciones)` recibe las barras seleccionadas y todas las barras candidatas del documento (como `DetectorBar`) y
  añade, por rondas, las que **tocan** las ya elegidas con la **misma regla del detector** (ronda 8b): `NodeDetector.SnapEnd`
  decide si el extremo de una barra se corta con el eje de la otra a menos del alcance de cara (`chord` si la otra pasa de
  largo, `member` si llega), y dos tramos paralelos pegados por los extremos son un empalme (`splice`). Se queda **en el plano
  de la cercha**: una barra con un extremo a más de 50 mm del plano (correa, riostra) se descarta (`SkippedOutOfPlane`); con
  una sola barra el plano no se conoce y lo **votan** las que la tocan (la normal de cada pareja no paralela; gana la más
  repetida). Tope de 400 barras (`LimitReached`). Filtro rápido por cajas envolventes antes de la geometría fina.
  `SelectionExpansion` lleva las añadidas con su motivo y ronda, las descartadas, los IDs que no son barras, el plano y
  `SummaryText()` en español ("Se añadieron 15 barras que tocan la selección: 3 cordones que pasan de largo y 12 barras que
  llegan. 1 barra fuera del plano de la cercha no se añadió." / "La selección ya está completa: ninguna barra más la toca.");
  `SelectionExpansionSummary` es lo que viaja en el plan y en la respuesta (`selection_expansion`).
- **Core, `Batch/LabelImage.cs` (V3)**: el BMP de **24 bits y 32×32** de la etiqueta, escrito a mano (lo que el sondeo 19 v3 y
  v4 demostró que Revit pinta; sin `System.Drawing`): círculo del color del estado (los RGB de `PlanAdvice`), anillo blanco y
  la cifra del nudo en blanco con una fuente de píxeles de 5×7 (hasta tres cifras; `NumberOf("N4") = "4"`). La versión
  **resaltada** es la inversa (círculo blanco, anillo grueso y cifra en color). `FileName` estable por color, cifra y estado;
  `EnsureFile` escribe una vez y reutiliza; `PixelAt` para las pruebas.
- **Core, `Brief/DesignBriefWriter.cs` (ronda 9b)**: el encargo para una IA externa en un solo Markdown: el prompt de la
  sección 3 de la propuesta, los datos del nudo (`node_info` tal cual), el esquema (`data.example`), las secciones 2 a 4 de
  `docs/guide.md` (`ExtractGuideSections`) y un ejemplo confirmado; `FileNameFor` (`encargo-<documento>-AAAAMMDD-HHMM.md`);
  `EmbeddedExampleJson()` lee `docs/fixtures/detalle-D-confirmado.json`, embebido en el Core como recurso.
- **Core, modelo y configuración**: `BatchPlan.LabelViewId`, `LabelIndices` (nudo → índice del control), `HasLabels`,
  `SelectionExpansion`; `PlanNode.LabelIndex`, `GhostElementId`; `CatalogConfig.PlanLabels` y `PlanGhosts` (`plan_labels`,
  `plan_ghosts` en `config/catalog.json`, `true`); códigos `SELECTION_EXPANDED` y `PLAN_LABELS_SKIPPED` (avisos).
- **Add-in, `Batch/PlanLabels.cs` + `PlanLabelHandler` (V3)**: `Apply` pone una etiqueta por nudo visible en la vista marcada
  (`TemporaryGraphicsManager.AddControl(InCanvasControlData(ruta, punto de trabajo), vista)`, `SetTooltip` con el globo del
  mapa) y guarda los índices en el plan; `Remove`, `RemoveNodes` (los creados) y `Clear` (todas las del documento) las quitan
  (`RemoveControl` solo de las que `GetAll()` dice que existen); `Highlight` cambia la etiqueta del nudo elegido por su versión
  resaltada con `UpdateControl` y devuelve la anterior; `OnClick` busca plan y nudo por el índice y el documento, resalta,
  anota `label_clicked` y avisa a la ventana. La carpeta de los BMP es la primera sin tildes ni espacios de
  `%LOCALAPPDATA%\MotorConexiones\etiquetas`, `%PUBLIC%\MotorConexiones\etiquetas`, `C:\MotorConexiones\etiquetas` o la
  temporal (`label_folder` en el log). `EnsureHandlerRegistered` registra el servidor `3f6c1b2e-…` en
  `TemporaryGraphicsHandlerService` y lo activa **sin desactivar a los demás** (`GetActiveServerIds` + `SetActiveServers`).
  El manejador (`ITemporaryGraphicsHandler.OnClick`) **nunca abre un cuadro** y captura cualquier excepción
  (`label_click_failed`). Nada de aquí lanza: fallos al log y al aviso `PLAN_LABELS_SKIPPED`.
- **Add-in, `Batch/BatchPlanner.cs`**: `ExpandSelection(documento, selección, avisos)` lee como candidatas todas las barras
  de armazón estructural con eje del documento (`RevitModelFacts`) y llama al Core (`selection_expanded` en el log, con la
  duración). `Plan` con `ExpandSelection` amplía la selección antes de detectar (aviso `SELECTION_EXPANDED`; el plan guarda
  `SelectionExpansion`); quita las etiquetas del plan anterior (y de otros planes marcados) **fuera** de la transacción, y
  después de las marcas pone las nuevas (`Mark && Labels && plan_labels`); `Discard` / `DiscardAndClean` / `DiscardAll` quitan
  etiquetas (`removed_labels`, `remaining_labels`; Descartar plan y `all` llaman a `Clear`). `PlanToData` añade
  `selection_expansion`, `labels` {`count`, `view_id`}, `label_view_id`, `label_indices`, `marks.ghost_count`; `NodeToData`,
  `label_index` y `ghost_element_id`.
- **Add-in, `Batch/PlanMarks.cs` (V2, cartelas fantasma)**: en cada nudo con especificación (`ready`, `failed`, `invalid`),
  `CreateGhost` dibuja un `DirectShape` con el contorno de `gusset.outline.points_mm` extruido su espesor y centrado en el
  plano de la cercha, en el marco del nudo leído del modelo (`NodeInspector.ResolveNode`, el mismo que usa la fabricación),
  con el `ApplicationId` de los marcadores y `ApplicationDataId` `<plan>:<nudo>:ghost`, coloreado por estado y transparente
  al 65 %; entra en `MarkerElementIds` (así Remove, RemoveNodes, RemoveAll y `discard all` lo quitan como a un marcador) y en
  `PlanMarkState`. Un fallo de geometría solo avisa (en un nudo que no valida, solo al log).
- **Add-in, `Batch/BatchCreator.cs`**: las etiquetas de los nudos creados se quitan con sus marcas (`RemoveLabelsOfCreated`).
- **Add-in, `BatchPlanCommand.cs` (C6)**: basta **una** barra seleccionada. `AssistSelection` busca las que la tocan y, si hay,
  un `TaskDialog` con dos enlaces de orden y Cancelar: **Planificar con las N barras** / **Planificar solo las M seleccionadas**
  (`ribbon_batch_assist` con `choice`); sin nada que añadir, planifica directamente; con una barra sola y nada que la toque, lo
  dice. Registra el manejador de clics por si el arranque no pudo. El aviso `SELECTION_EXPANDED` no pinta la barra de estado
  en rojo.
- **Add-in, `UI/BatchPlanWindow`**: `OnLabelClicked(plan, nudo)` (lo llama el manejador en el hilo de Revit): elige la fila
  (enseñándola si estaba oculta), pinta el detalle y escribe "Etiqueta 4 pinchada en la vista: N4 · Listo · … Ver en Revit
  encuadra el nudo"; si la etiqueta es de otro plan, solo lo dice. Elegir una fila (tabla o mapa) resalta su etiqueta en Revit
  por el `ExternalEvent`, sin apagar los botones. **Más… > Completar selección** replanifica el plan con la selección ampliada
  (`ReplanNow` admite `elementIds`). La cabecera dice "… (N añadidas por la selección asistida) · marcas puestas en la vista con
  33 etiquetas pinchables y 16 cartelas fantasma"; el detalle del nudo, su etiqueta (y "resaltada") y su cartela fantasma.
- **Add-in, `App.cs`**: botón **Encargo para IA** y registro del manejador de clics en el arranque (`label_handler` en el
  evento `startup`); textos del botón Planificar lote al día.
- **Add-in, `DesignBriefCommand.cs` (ronda 9b)**: con el nudo seleccionado (≥ 2 barras), `Bridge.Handle("node_info")`,
  el ejemplo del esquema, `GuideOperation.ReadGuideMarkdown()` (nuevo, compartido con `conn_get_guide`), el ejemplo confirmado
  (la plantilla del catálogo con la misma cantidad de barras, la más reciente, o el Detalle D embebido), el archivo en
  `%LOCALAPPDATA%\MotorConexiones\encargos\`, `Clipboard.SetText`, el Explorador con el archivo marcado y un `TaskDialog`
  corto. Errores de `node_info` en un cuadro con el `hint`. Log `ribbon_design_brief`.
- **Add-in, `Fabrication/AdvanceSteelBackend.cs`** (`fase-9.md` 7.2, punto 3): `BeginSession(document, name, forDeletion)`;
  al borrar, si no se abre la `FabricationTransaction`, el aviso dice "No se pudo abrir la FabricationTransaction de Advance
  Steel para borrar: … Los elementos se borran directamente (Document.Delete) y las barras se restauran igual." (y
  `for_deletion: true` en el log). `ConnectionCreationService.DeleteConnection` lo pasa.
- **Operaciones del puente**: `batch_plan` admite `expand_selection` y `labels`; `batch_plan_discard` devuelve `removed_labels`
  y `remaining_labels` (también con `all`).
- **MCP**: `mcp/revit_mcp/conexiones.py` 0.10.0 (25 rutas), `mcp/tools/conn_tools.py` 0.10.0 (`conn_batch_plan` con
  `expand_selection` y `labels`, manuales al día; 23 herramientas), `mcp/pruebas/simulador_revit.py` (etiquetas,
  `expand_selection` que añade las otras barras del nudo, `removed_labels`; 66 comprobaciones), `mcp/pruebas/probar_conexiones.py`
  (pruebas 29 y 30: `batch_plan` con `expand_selection` desde una barra y su descartar; las del puente pasan a 31 y 32),
  `mcp/CONTRATO-conn.md`.
- **Sondeo 21** (`scripts/sondeos/21-etiquetas-addin.py`): cuenta los controles del lienzo (`GetAll`), dice si el servidor de
  clics del add-in está registrado y activo, la carpeta de BMP y cuántos hay, los marcadores y las cartelas fantasma del
  modelo; con `LIMPIAR = True`, `Clear()`.
- **Documentación**: `docs/prompts/fase-10.md`, `docs/instalacion/fase-10.md` (con lo pendiente de la Fase 9), `docs/guide.md`
  (sección 6: `expand_selection`, etiquetas y fantasmas, `removed_labels`), README (estado, tabla de garantías, árbol,
  instalación, sección 9 "Encargo para IA", sección 12 "Selección asistida, etiquetas pinchables y cartelas fantasma", 13 y
  14), `CLAUDE.md` (estructura y la regla de los cuadros), `docs/propuestas/flujo-intuitivo.md` (C6, V3 y V2 hechos) y
  `docs/propuestas/encargo-ia-externa.md` (estado), `docs/fases/resumen-fase-10.md`.
- **Versión 0.10.0** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador.
- **Pruebas**: `SelectionAssistTests` (8), `LabelImageTests` (12, con la teoría de `NumberOf`) y `DesignBriefTests` (5). De
  192 a **217**.

---

## 2. Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` y NuGet sirvió los paquetes de la API 2027: `MotorConexiones.Revit` compila
en la nube, así que los miembros usados existen (`TemporaryGraphicsManager.GetTemporaryGraphicsManager / AddControl /
RemoveControl / UpdateControl / SetTooltip / GetAll / Clear`, `InCanvasControlData(string, XYZ)`, `ITemporaryGraphicsHandler`,
`TemporaryGraphicsCommandData.Index / Document`, `ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService`,
`MultiServerService.GetActiveServerIds / SetActiveServers`, `ExternalService.GetRegisteredServerIds / AddServer`,
`OverrideGraphicSettings.SetSurfaceTransparency`, `TaskDialog.AddCommandLink`, `System.Windows.Clipboard`); el sondeo 19 v4
ya ejecutó los de las etiquetas en el PC con IronPython, pero **no desde el add-in** (2.1).

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> .../MotorConexiones.Core.dll
  MotorConexiones.Tests -> .../MotorConexiones.Tests.dll
  MotorConexiones.Revit -> .../MotorConexiones.Revit.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   217, Skipped:     0, Total:   217, Duration: 879 ms - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
Autocomprobación: 66/66 correctas

$ python3 mcp/pruebas/simulador_revit.py &  (token en el archivo literal "%LOCALAPPDATA%\RevitMcp\token" de la carpeta actual)
$ python3 mcp/pruebas/probar_conexiones.py
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> las que la tocan, selection_expansion y aviso SELECTION_EXPANDED  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED', 'CATALOG_EMPTY'], añadidas=3 (0 cordones, 3 barras, 0 tramos) selection_count=4 avisos=['SELECTION_EXPANDED', 'CATALOG_EMPTY'] | Se añadieron 3 barras que tocan la selección: 3 barras que llegan.
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, descartado e8d0323f-… etiquetas quitadas=0
Resultado: 30/30 pruebas correctas
```

Lo que prueban las pruebas nuevas del Core:

| Prueba | Qué comprueba |
|---|---|
| `Expand_FromOneDiagonal_AddsTheWholeSyntheticTruss` | Desde una diagonal de la cercha sintética: 15 añadidas (3 cordones, 12 que llegan), la primera ronda trae los dos cordones por los que pasa y las otras dos barras de su nudo, el plano es XZ, la barra que cruza en Y queda fuera del plano (1 descartada) y el texto exacto |
| `Expand_FromTheChordAlone_VotesThePlaneAndAddsTheDiagonals` | Desde el cordón solo: el plano lo votan las 11 diagonales que terminan en él (la que se queda 85 mm corta no), los cordones superior e inferior entran como cordones en la segunda ronda, el montante desplazado 8,5 mm no |
| `Expand_LeavesOutBarsOutOfThePlane` | Una riostra que llega al nudo desde fuera del plano se descarta ("2 barras fuera del plano de la cercha no se añadieron") |
| `Expand_WithTheWholeTruss_AddsNothing`, `Expand_IgnoresIdsThatAreNotBarsAndSaysSo`, `Expand_StopsAtTheLimitAndWarns` | Selección completa, IDs que no son barras, tope |
| `Expand_FromOneDiagonalOfTheHangar8bTruss_RecoversTheWholeSelectionAndTheSamePlan` | Desde la diagonal 1251053 de la cercha de la 8b se recuperan las **56 barras** (los 8 tramos del cordón, el último por empalme o por sus diagonales) y el plan vuelve a dar **59 nudos y 16 listos** |
| `Touches_FollowsTheDetectorRule` | Cordón ↔ diagonal (`chord` / `member`), la diagonal corta no toca, la suelta no, el empalme sí, dos tramos separados 300 mm no |
| `LabelImageTests` | 3126 bytes, cabecera `BM`, 32×32, 24 bits; relleno, anillo y cifra en los píxeles esperados (normal y resaltada); dos y tres cifras; `NumberOf`; nombres de archivo; `EnsureFile` escribe una vez y repara un archivo roto |
| `DesignBriefTests` | Todo en orden (prompt, nudo, esquema, secciones 2 a 4 sin la 1 ni la 5, ejemplo embebido), los textos cuando falta algo, `ExtractGuideSections`, `FileNameFor`, el recurso embebido es el fixture |

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-10.md`, unos 90 minutos)

| Qué | Paso | Qué se espera |
|---|---|---|
| `deploy.ps1` 0.10.0.0, `catalog.json` con `plan_labels` y `plan_ghosts`, `instalar-conn.ps1` 25 rutas y 23 herramientas, `ping` 0.10.0 | 10-1, 10-2 | Como en las fases anteriores; `startup` con `label_handler: true` y `label_handler_registered` en el log |
| **Sondeo 21** antes de nada | 10-2 | 0 controles; el servidor del add-in registrado y activo |
| **Selección asistida** desde una diagonal de la cercha de la 8c | 10-3 | Cuadro "Selección asistida: 63 barras tocan la selección" (o las que haya), Planificar con todas → los mismos 16 listos; Cancelar y Solo la selección |
| **Etiquetas**: se ven, pinchar la 4 sin ningún cuadro, la ventana elige N4, barra de estado, resaltado desde la tabla y el mapa, clic con la ventana cerrada, el globo | 10-4 | Captura `fase10-03`; ningún cuadro; Revit sigue respondiendo |
| **Cartelas fantasma** en los 16 listos, en el sitio de la cartela | 10-4 | Captura `fase10-02` |
| Editar nudo con la ventana abierta; `PLAN_MARKS_REPLACED` (puente → ventana y ventana → puente) con un solo juego de marcas y etiquetas (sondeo 21) | 10-5 | Lo pendiente de `fase-9.md` 7.6 |
| Crear 16 → etiquetas y fantasmas de los creados fuera; **Ctrl+Z** → marcas y fantasmas vuelven, etiquetas no (Replanificar las repone); crear otra vez y **Borrar el lote desde la ventana** → cuadro, barra de estado, replanificado, sondeos 12 y 13 a cero | 10-5 | Capturas `fase10-04` y `fase10-05`; `ribbon_batch_delete` con `deleted: 16` |
| El texto del aviso del borrado (`for_deletion`) y el de los `REVIT_WARNING` de un `create` en la {3D} | 10-5, 10-6 | "… para borrar: … se borran directamente"; "The created elements are only visible in Detail Level: Fine" (o el que sea) |
| Por el puente: `batch_plan` con `expand_selection` desde una barra, `batch_plan_get` con `labels` / `label_indices` / `ghost_count`, `batch_plan_discard` con `removed_labels`, `discard all` con `remaining_labels: 0`; sondeos 21 y 17 a cero | 10-7 | Nada queda en el lienzo ni en el modelo |
| **Encargo para IA** sobre el Detalle D: cuadro, archivo, portapapeles, Explorador; opcionalmente pegarlo en Claude y aplicar el JSON | 10-8 | Captura `fase10-06` |
| `probar_conexiones.py --puente` **32/32** y el log del día | 10-9 | Sin `label_click_failed`, `labels_failed`, `plan_event_failed` ni `ribbon_batch_window_error` |

### 2.2 NO PROBADO en la nube y por qué

- **Todo lo de Revit** (2.1): los controles del lienzo puestos por el add-in y el clic desde su propio manejador (el sondeo
  19 v4 probó el mecanismo desde IronPython, con otro servidor y otro BMP), la selección asistida sobre el modelo real (en la
  nube se probó con la cercha de la 8b reconstruida de los ejes reales, que es lo más parecido), las cartelas fantasma (la
  geometría es la misma que la de la cartela de verdad, pero `DirectShape` solo se prueba en Revit), el botón Encargo para IA
  (portapapeles y Explorador), el aviso del borrado y lo pendiente de la Fase 9.
- **La votación del plano con barras fuera del plano que también tocan**: probada con una riostra sintética; en el Hangar no
  hay candidatas de ese tipo en la cercha de la 8c (se verá en 10-3 con `skipped_out_of_plane`).
- **El manejador registrado en el arranque** (`OnStartup`): `ExternalServiceRegistry` suele admitirlo ahí; si no, el botón
  Planificar lote lo reintenta y el log lo dice (`label_handler_error`).
- **Las etiquetas con la ventana del plan cerrada y Revit ocupado** (por ejemplo, un clic durante un `PickObject`): Revit no
  entrega clics del lienzo mientras pincha; sin probar.

---

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. **La selección asistida**: con una sola diagonal pinchada, el cuadro de Planificar lote debe decir cuántas barras añade y
   por qué (captura `fase10-01`), y con **Planificar con todas** la ventana debe enseñar los **mismos 16 listos** de siempre.
   Si enseña menos listos o más nudos, la selección ampliada incluye o excluye algo distinto a tu selección a mano: mira
   `selection_expansion.added_element_ids` del paso 10-7 y compáralo con los 64 IDs de la 8c.
2. **Las etiquetas**: una por nudo marcado, con el número y el color del estado (captura `fase10-02`). Al **pinchar la 4** no
   debe salir ningún cuadro; la etiqueta se vuelve blanca con el 4 en color, la ventana elige N4 y su barra de estado lo dice
   (captura `fase10-03`). Al elegir N7 en la tabla, la 7 se resalta y la 4 vuelve.
3. **Las cartelas fantasma**: en N4 (el gemelo del Detalle D) la placa transparente tiene que estar donde la Fase 7 creó la
   cartela de verdad (compara con `fase7-02`); en los nudos en espejo, reflejada. Tras **Crear 16 conexiones** desaparecen y
   queda el acero; tras Ctrl+Z vuelven.
4. **Borrar el lote desde la ventana** (lo pendiente de la Fase 9): el cuadro de confirmación, la barra de estado "16 conexiones
   borradas …" y la cercha otra vez con sus marcas (captura `fase10-05`).
5. **El encargo**: el cuadro, el archivo `.md` en `encargos\` y, si lo pegas en Claude con `detalle-D.png`, si el JSON que
   devuelve valida a la primera en **Ejecutar especificación JSON**.

---

## 4. Decisiones tomadas y por qué

### 4.1 La selección asistida usa la regla del detector, no una distancia a ojo

Dos barras "se tocan" exactamente cuando el detector las agruparía en un nudo (`SnapEnd`: ejes que se cortan a menos de 5 mm,
extremo a menos del alcance de cara, corte dentro del tramo o como mucho un alcance más allá). Así, planificar con la selección
ampliada da los mismos nudos que si la persona hubiera seleccionado a mano (comprobado con la cercha de la 8b: 59 nudos y 16
listos). Un empalme (dos tramos paralelos pegados) se añade aparte, porque los ejes paralelos no se cortan.

### 4.2 Dentro del plano de la cercha, por votación

Sin el filtro del plano, pinchar una diagonal del Hangar arrastraría correas y riostras que terminan en los nudos (y el
detector vería nudos de cuatro barras que no casan con nada). Con una barra sola no hay plano todavía: se vota entre las que
la tocan (en una cercha, casi todas están en su plano) y los tramos paralelos no votan. Tolerancia de 50 mm: holgada para
cerchas dibujadas a mano (en el Hangar los ejes varían 0,2 mm), estrecha para una correa (sus extremos están a metros).

### 4.3 Las etiquetas son controles del lienzo, no elementos del modelo

Es lo que el sondeo 19 demostró que funciona y pinchable. Consecuencias asumidas: no pasan por transacciones ni por
Deshacer (tras Ctrl+Z del lote las marcas vuelven y las etiquetas no: Replanificar las repone), se van al cerrar el documento,
y se quitan explícitamente al replanificar, al crear, al descartar y con `discard`; `Clear()` en Descartar plan y `discard all`
para no dejar ninguna (también de planes olvidados). Un fallo nunca impide planificar (`PLAN_LABELS_SKIPPED`).

### 4.4 El clic nunca abre un cuadro

El `TaskDialog` del sondeo 19 v2 dejó a Revit sin atender a pyRevit un cuarto de hora. El manejador solo llama a
`UpdateControl`, al log y a la ventana del plan (que es WPF en el mismo hilo), y captura cualquier excepción: el hilo de Revit
nunca recibe una excepción del add-in desde ahí.

### 4.5 El BMP se escribe a mano en el Core

El sondeo 19 v3 necesitó un BMP de 24 bits (el de 32 bits de la v2 no se vio) en una ruta sin tildes ni espacios. Escribirlo a
mano con una fuente de píxeles lo hace determinista, probado byte a byte en la nube y sin `System.Drawing`.

### 4.6 Las cartelas fantasma entran, sin la placa cuchilla

La geometría ya existía (el contorno de la especificación y el marco del nudo de la fabricación), así que cabía sin recortar
nada: ~100 líneas en `PlanMarks`. Solo la cartela (la placa cuchilla del miembro empernado se queda para otra ronda si se
quiere). Van en `MarkerElementIds` para que toda la limpieza de marcadores (por plan, por nudo creado, huérfanos) las cubra sin
código nuevo. Se dibujan también en los nudos fallidos y en los que no validan (así se ve por qué una barra se sale de la cartela).

### 4.7 El encargo en un solo Markdown, con el ejemplo del catálogo si lo hay

Decisiones P1 a P3 de la persona. El ejemplo es la plantilla del catálogo con la misma cantidad de barras (su
`spec_template`, sin IDs y con `slot` por barra; el encargo lo dice) o, si no hay, el Detalle D confirmado embebido en el Core
(no depende de que `docs/fixtures/` esté en el PC).

### 4.8 El aviso del borrado dice lo que pasa

`BeginSession` lleva `forDeletion`: el texto distingue crear ("Toda la conexión se crea con DirectShape") de borrar ("Los
elementos se borran directamente (Document.Delete) y las barras se restauran igual").

---

## 5. Pendientes, riesgos y preguntas

- **Riesgo 1: las etiquetas desde el add-in**. El sondeo 19 las puso desde IronPython con un servidor de prueba; el add-in
  las pone desde C# con otro servidor registrado en el arranque. Si no se ven (10-4.1) o el clic no llega (10-4.2), el log
  (`labels_applied`, `label_handler_registered`, `label_clicked`) dirá en qué punto se quedó; `plan_labels: false` las apaga
  sin recompilar y el resto de la fase sigue valiendo.
- **Riesgo 2: la selección asistida añade de más o de menos** en el modelo real (correas que sí terminan en el eje del cordón,
  cordones que no se tocan por un hueco). El cuadro deja elegir "Solo la selección", y `skipped_out_of_plane` /
  `added_element_ids` del puente dicen qué pasó. El tope de 400 avisa.
- **Riesgo 3: las cartelas fantasma en nudos en espejo**: usan el mismo marco y contorno que la fabricación, así que si la
  cartela real salió bien en espejo (Fase 7, `fase7-06/07`), el fantasma también; se mira en 10-4.1.
- **Riesgo 4: `Clear()` quita todos los controles del lienzo del documento**, también los de otro add-in que los usara. En el
  PC de la persona solo MotorConexiones (y el sondeo 19) los usan; está documentado.
- **P1.** ¿Las etiquetas deberían ir **un poco por encima** del punto de trabajo (para no tapar el cubo) o está bien encima?
  Se decide viendo `fase10-02`.
- **P2.** ¿Las cartelas fantasma también de la **placa cuchilla** (otra placa transparente por barra empernada)? Cabe en una ronda.
- **P3.** ¿Quieres que al pinchar una etiqueta la ventana del plan **venga delante** (hoy no roba el foco a Revit)?
- **P4.** La ronda 9b dejó fuera la herramienta MCP `conn_design_brief` (P2 de la propuesta): se añade cuando el agente local
  la necesite.
- Lo de siempre: `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`, el botón Conectar, V4 y V5 (Fase 11), la cartela
  automática (Fase 12), la placa de extremo (`docs/propuestas/placa-de-extremo.md`).

---

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-10.md` entero (unos 90 minutos sobre la copia).
2. Hacer los pasos marcados **(la persona)**: pinchar **una** barra y Planificar lote (10-3), mirar las etiquetas y las cartelas
   fantasma y **pinchar la 4** (10-4), Editar nudo, Crear 16, Ctrl+Z, crear otra vez y **Borrar el lote desde la ventana**
   (10-5), y el botón **Encargo para IA** (10-8). Seis capturas (`fase10-01` a `fase10-06`).
3. Devolver `docs\fases\resultados-fase-10.md` con las anotaciones y abrir la sesión de cierre con el prompt de
   `docs/fases/resumen-fase-10.md` (sección 4).
