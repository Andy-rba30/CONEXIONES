# Fase 10: cercha sin dolor (selección asistida, etiquetas pinchables, cartelas fantasma y el encargo para IA)

Fecha: 2026-10-06. Rama: `main`. Add-in **0.10.0** (Core con `Batch/SelectionAssist`, `Batch/LabelImage` y
`Brief/DesignBriefWriter`; add-in con `Batch/PlanLabels` + `PlanLabelHandler`, las cartelas fantasma en `PlanMarks`, la
selección asistida en `BatchPlanner` / `BatchPlanCommand` / la ventana, y `DesignBriefCommand`), adaptador 0.10.0 (25 rutas,
sin rutas nuevas) y 23 herramientas `conn_*` (`conn_batch_plan` con `expand_selection` y `labels`). Prompt y alcance:
`docs/prompts/fase-10.md`, escrito a partir de las mejoras C6, V3 y V2 de `docs/propuestas/flujo-intuitivo.md`, de la ronda
9b de `docs/propuestas/encargo-ia-externa.md` y de lo que la Fase 9 dejó anotado (`docs/fases/fase-9.md`: 7.2 punto 3, 7.6 y 7.8).

**Estado: probada en Revit el 2026-10-07 (sección 7) con el add-in 0.10.0; cerrada con la ronda 10b (add-in 0.10.1).**
La instalación (`docs/instalacion/fase-10.md`, `resultados-fase-10.md`) contrastó la selección asistida por el puente (63
añadidas, 16 listos), las 33 etiquetas y las 16 cartelas fantasma, Crear 16 dos veces, Ctrl+Z, **Borrar el lote desde la
ventana**, el Encargo para IA y los textos de los avisos (7.1). **El botón Planificar lote con una barra murió en el cuadro
de la selección asistida** ("Corresponding button not found: defaultButton"; corregido en la 0.10.1, 7.2) y por eso **las
etiquetas no se llegaron a pinchar**; el `--puente` dio 14/22 porque el documento activo era otro y el servidor del puerto
8000 no estaba. Solo eso se repite en el PC con `docs/instalacion/fase-10b.md` (unos 30 minutos; 7.6).

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

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-10.md`, unos 90 minutos) — **hecho el 2026-10-07, contrastado en 7.1**

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

### 2.2 NO PROBADO en la nube y por qué (comprobado en el PC: 7.1; lo que sigue sin probar: 7.6 y 7.7)

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

## 5. Pendientes, riesgos y preguntas (escritos antes de la instalación; lo que pasó con cada uno, en 7.2 y 7.9)

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

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando (hecho el 2026-10-07; lo que sigue, en 7.10)

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-10.md` entero (unos 90 minutos sobre la copia).
2. Hacer los pasos marcados **(la persona)**: pinchar **una** barra y Planificar lote (10-3), mirar las etiquetas y las cartelas
   fantasma y **pinchar la 4** (10-4), Editar nudo, Crear 16, Ctrl+Z, crear otra vez y **Borrar el lote desde la ventana**
   (10-5), y el botón **Encargo para IA** (10-8). Seis capturas (`fase10-01` a `fase10-06`).
3. Devolver `docs\fases\resultados-fase-10.md` con las anotaciones y abrir la sesión de cierre con el prompt de
   `docs/fases/resumen-fase-10.md` (sección 4).

---

## 7. Cierre de la Fase 10 (2026-10-07): los resultados contrastados y la ronda 10b (add-in 0.10.1)

Instalación hecha el 2026-10-07 (`docs/fases/resultados-fase-10.md`, commit `94f4d18`; 19 900 líneas, casi todas de las tres
respuestas largas del puente) con el add-in 0.10.0 sobre la copia `HANGAR_PRUEBA_sondeo.rvt`. **No llegaron capturas**
(`fase10-01` a `fase10-06` no están en `docs/fases/capturas/`) **ni anotaciones de la persona en el archivo**; de los pasos
de la persona solo se sabe lo que dice el log del día (bloque `10-9 log del dia`) y lo que la persona contó en el chat: al
pinchar una barra y pulsar **Planificar lote**, un cuadro de error "No se pudo planificar el lote: **Corresponding button not
found: defaultButton**". Todo lo que sigue se contrasta con el log y con las respuestas del puente, y se dice qué quedó sin ver.

### 7.1 Contraste con lo esperado en 2.1

| Esperado (2.1) | Resultado | ¿Coincide? |
|---|---|---|
| `deploy.ps1` 0.10.0.0, `catalog.json` con `plan_labels` y `plan_ghosts`, `instalar-conn.ps1` 25 rutas y 23 herramientas, `ping` 0.10.0; `startup` con `label_handler: true` y `label_handler_registered` | `== MotorConexiones 0.10.0.0 desplegado ==` con `config\catalog.json` entre los copiados, `"plan_labels": true` y `"plan_ghosts": true` en el desplegado, `25 rutas @api.route`, `23 herramientas @mcp.tool`, DLL `0.10.0.0`, `ping` con `addin_version 0.10.0` y 23 operaciones; log de arranque: `label_handler_registered` (`already_registered: false`, `activation: "multi:2"`) y `startup` con `"label_handler":true, "label_handler_error":null`; en el PC, compilación sin avisos y 217/217 | **Sí** |
| **Sondeo 21** antes de nada: 0 controles; el servidor del add-in registrado y activo | `2) Controles en el lienzo (GetAll): 0`; `3) Servidor del add-in 3f6c1b2e-…: registrado=True \| activo=True` (dos servidores activos: el de Autodesk y el del add-in, "MotorConexiones: etiquetas del plan (ARBA)"); la carpeta de etiquetas aún no existía; `5) Marcadores de plan en el modelo: 0` | **Sí** (el riesgo 1 de la sección 5, el registro en el arranque, queda cerrado) |
| **Selección asistida** desde una diagonal con el botón: cuadro "Selección asistida: 63 barras tocan la selección", Planificar con todas → los mismos 16 listos; Cancelar y Solo la selección | El botón se pulsó **dos veces** con una barra (11:20:33 y 11:21:06): el log tiene los dos `selection_expanded` (`requested: 1, candidates: 964, added: 63, chords: 8, members: 46, splices: 9, skipped_out_of_plane: 0, rounds: 11`, 0,6 s) y **ningún `ribbon_batch_assist` ni ventana abierta después**: el cuadro murió al construirse ("Corresponding button not found: defaultButton", 7.2) y el botón terminó con el cuadro de error. La **búsqueda** de las barras funciona en el modelo real: 63 añadidas, las mismas que por el puente | **No** (el cuadro; corregido en la 0.10.1 y se repite en la 10b). La búsqueda, **sí** |
| **Etiquetas**: se ven, pinchar la 4 sin ningún cuadro, la ventana elige N4, barra de estado, resaltado desde la tabla y el mapa, clic con la ventana cerrada, el globo | Las etiquetas **se pusieron**: `label_folder` `C:\Users\Public\MotorConexiones\etiquetas` (`plain: true`; la primera candidata, `%LOCALAPPDATA%`, tiene la tilde de "Antón" y se saltó, como estaba previsto), `labels_applied` con `count: 33` cinco veces (11:28:53 por el puente; 12:54:22, 12:55:11 al replanificar desde la ventana; …), 33 BMP de 3126 bytes en la carpeta (sondeo 21), `labels: {count: 33, view_id: 1245519}` y `label_indices` de los 33 nudos visibles en la respuesta. **Ningún `label_clicked` ni `ribbon_batch_label_clicked` en todo el día**: nadie pinchó una etiqueta (la persona no llegó a este paso tras el error del cuadro). Que se vean, el clic, el resaltado y el globo quedan **sin ver** | **Parcial**: puestas y quitadas sí; pinchadas no (10b) |
| **Cartelas fantasma** en los 16 listos, en el sitio de la cartela | `marks.ghost_count: 16` y `ghost_element_id` en los 16 listos (N4: `1322528`), `marker_element_ids` 49 (33 marcadores + 16 fantasmas); `discard all` quitó los 49 (`removed_markers: 49`); sin captura ni anotación de **dónde** salieron | **Parcial**: existen y se quitan; si están en el sitio de la cartela, sin ver (se mira de paso en la 10b) |
| Editar nudo con la ventana abierta; `PLAN_MARKS_REPLACED` puente → ventana y ventana → puente | El plan del puente (11:28:53, `e0136b1f`) **no avisó `PLAN_MARKS_REPLACED`**: no había ningún plan de la ventana marcado (el botón había muerto). Después la persona abrió la ventana con **Planificar lote sin selección** (reabre el último plan del documento: el del puente) y todo lo demás lo hizo sobre ese plan; no hubo un segundo plan marcado que lo sustituyera. Editar nudo no deja rastro en el log | **Sin ver** otra vez (como en la Fase 9); no bloquea: `discard all` y los sondeos 17 y 21 a cero dicen que no quedaron marcas dobles |
| Crear 16 → etiquetas y fantasmas de los creados fuera; Ctrl+Z → marcas y fantasmas vuelven, etiquetas no (Replanificar las repone); crear otra vez y **Borrar el lote desde la ventana** → cuadro, barra de estado, replanificado; sondeos 12 y 13 a cero | **Crear 16** dos veces desde la ventana: 12:51:11 (`ribbon_batch_create`, `created 16, failed 0`, 4,0 s) y 12:54:27 (2,1 s), cada una precedida de `labels_removed` `requested 16, removed 16, remaining_in_document 17` (las de los 16 creados fuera; quedan las 17 de los no listos). Entre las dos, 12:54:22: `labels_removed 17` y `labels_applied 33`, es decir, **Replanificar tras el Ctrl+Z** repuso las 33 etiquetas (si las marcas volvieron con Ctrl+Z no está anotado; sí que al replanificar había 16 listos otra vez, porque el segundo lote creó 16). **Borrar el lote desde la ventana**: 12:55:10 `ribbon_batch_delete` con `deleted: 16, failed: 0` (`batch_delete`: 144 elementos, 48 barras restauradas, 1,1 s, "Una sola entrada de deshacer") seguido de `labels_removed 17` y `labels_applied 33` (la replanificación posterior); sondeos 12 y 13 a cero (`conexiones en el modelo: 0`, acero suelto 0, extensiones de las barras del fixture iguales a las de antes). El cuadro de confirmación y el texto de la barra de estado, sin anotar | **Sí** en lo que el log puede decir (lo pendiente de la Fase 9 queda cerrado: 7.5) |
| El texto del aviso del borrado (`for_deletion`) y el de los `REVIT_WARNING` de un `create` en la {3D} | Al borrar el lote, 6 líneas `fabrication_transaction_failed` con `"for_deletion":true` ("cannot start a fabrication transaction while asynchronous fabrication tasks are queued for execution", desde la 11.ª conexión): es la rama nueva de `AdvanceSteelBackend`, cuyo texto es "No se pudo abrir la FabricationTransaction de Advance Steel para borrar: … Los elementos se borran directamente (Document.Delete) y las barras se restauran igual."; el aviso de la ventana no se anotó, pero el borrado fue completo (16/16, 48 barras restauradas). `create` del Detalle D en la {3D} por el puente: `ok: true`, 9 elementos, tres avisos `REVIT_WARNING` con `message` "**Advertencia de Revit: The created elements are only visible in Detail Level: Fine.**" (uno por cartela, placa cuchilla y pernos: lo que la Fase 9 suponía, 7.2 punto 4); `list` 1; `delete` `deleted_elements_count: 9`, `restored_members_count: 3`, **sin aviso** (una sola sesión de Advance Steel se abre bien); sondeo 12 a cero | **Sí** |
| Por el puente: `batch_plan` con `expand_selection` desde una barra, `batch_plan_get` con `labels` / `label_indices` / `ghost_count`, `batch_plan_discard` con `removed_labels`, `discard all` con `remaining_labels: 0`; sondeos 21 y 17 a cero | `batch_plan` desde la diagonal **1251723** con `expand_selection: true` (10-5): `ok`, aviso `SELECTION_EXPANDED` "Se añadieron 63 barras que tocan la selección: 8 cordones que pasan de largo, 46 barras que llegan y 9 tramos de cordón.", `selection_expansion` {`requested_count 1, added_count 63, chord_count 8, member_count 46, splice_count 9, rounds 11, skipped_out_of_plane [], limit_reached false`}, `selection_count 64`, `visible_count 33`, **`ready_count 16`** (25 `no_match`, 18 `untyped`), cabecera "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 empalmes del cordón (sin plantilla). Ocultos: 8 sin cordón, 18 barras sueltas." (**la misma** que con los 64 elementos a mano en las Fases 8 y 9), `labels {count 33, view_id 1245519}`, `label_indices` (N4 → 0), `marks.ghost_count 16`; 1,5 s dentro de Revit. `batch_plan_get` (10-7): el mismo plan con `label_indices` renovados (N4 → 66, tras tres replanificaciones) y `ghost_count 16`. Segundo `batch_plan` sin marcas: `selection_expansion` igual, `is_marked false`, `labels {count 0}`, `ghost_count 0`. `batch_plan_discard` de ese plan: `removed_labels 0`, `remaining_labels 33`, `remaining_markers 49`; `discard all`: `discarded_plans 1`, `removed_markers 49`, `remaining_markers 0`, `removed_labels 33`, **`remaining_labels 0`** (`labels_cleared before 33, after 0`); sondeo 21 `GetAll: 0`, fantasmas 0; sondeo 17 entero a cero | **Sí** (el riesgo 2 de la sección 5, añadir de más o de menos, queda cerrado para esta cercha) |
| **Encargo para IA** sobre el Detalle D: cuadro, archivo, portapapeles, Explorador | `ribbon_design_brief` a las 14:08:08: archivo `encargo-HANGAR_PRUEBA_sondeo-20261007-1408.md` (15 673 caracteres; 15 771 bytes en disco), `selection [1249510, 1249630, 1249631, 1249636]`, `members 4`, `clipboard: true`, `folder_opened: true`, ejemplo = "la plantilla 'PRUEBA probar_conexiones' del catálogo (spec_template: sin IDs, con slot por barra…)" (la más reciente con 3 barras: la que dejó el `--puente` de la Fase 9, 7.2 punto 4). El bloque `10-8 encargos` se ejecutó dos veces antes de pulsar el botón (carpeta inexistente) y la tercera listó el archivo. Sin captura; el cuadro y el contenido pegado, sin anotar | **Sí** en lo que el log puede decir |
| `probar_conexiones.py --puente` **32/32** y el log del día sin `label_click_failed`, `labels_failed`, `plan_event_failed` ni `ribbon_batch_window_error` | **14/22**: la prueba 2 respondió `documento=MODELO CERCO` (otro modelo era el activo, 30 s después de que el sondeo 12 leyera la copia), así que la 7 (`find_profile` sin HSS en ese modelo), 8, 9, 12 y 19 fallaron con `ELEMENT_NOT_FOUND` de los IDs del fixture y las 20 a 28 no se ejecutaron ("no hay template_id"); la 31 y la 32, "no se pudo conectar con el puente: [WinError 10061]" (el servidor del puerto 8000 no estaba). Las 14 que no dependen del modelo, en orden. El log: **ninguna** de las cuatro líneas prohibidas; tampoco `ribbon_batch_plan_failed`, pero el filtro de `10-9 log del dia` no la incluía (la 10b sí la pide) | **No** (se repite en la 10b con la copia activa y el servidor arrancado) |

Además: el `catalog_list` del paso 10-2 devolvió **2** plantillas (se esperaba 1): `Nudo tipico Detalle D` y
`PRUEBA probar_conexiones` (creada el 2026-10-06 a las 17:18 por la prueba 19 del `--puente` de la Fase 9, que se cayó en la
22 antes de llegar a la 28 que la borra). No afectó a los planes de la instalación (todos por el puente con `template_ids`
del Detalle D), pero sí al encargo (fila anterior) y afectaría a **Planificar lote desde la cinta**, que casa con todo el
catálogo: la 10b la borra antes (`catalog_delete`). El sondeo 21 del paso 10-5.3 también respondió desde `MODELO CERCO`
(`Controles: 0`, 48 s), así que la comprobación "un solo juego de etiquetas" se queda sin dato; el de 10-7 (sobre la copia)
sí: `GetAll: 0` tras `discard all`. La captura `fase8-01-sondeo17.png` cambió de tamaño otra vez (el sondeo 17 la exporta en
cada ejecución; sin importancia).

### 7.2 Lo que no coincidió y qué se hace con ello

1. **El cuadro de la selección asistida mataba el botón** (`BatchPlanCommand.AssistSelection`): en la 0.10.0 el
   `TaskDialog` fijaba `DefaultButton = TaskDialogResult.CommandLink1` **en el inicializador**, antes de `AddCommandLink`, y
   Revit lanza `Corresponding button not found: defaultButton` porque ese enlace aún no existe. La excepción saltaba fuera de
   `AssistSelection`, la recogía el `catch` general de `Execute` (`ribbon_batch_plan_failed` en el log y el cuadro "No se pudo
   planificar el lote: …") y el botón no planificaba con una sola barra; con la cercha entera seleccionada (nada que añadir) no
   pasaba por el cuadro y seguía funcionando, igual que `expand_selection` por el puente y Más… > Completar selección (que no
   preguntan). **Corregido en la 0.10.1**: `DefaultButton` se asigna **después** de los dos `AddCommandLink`, y el cuadro va
   en un `try`: si fallara igual, se anota `ribbon_batch_assist_dialog_failed`, se planifica **con todas** (la opción por
   defecto), la barra de estado lo dice con el aviso `SELECTION_EXPANDED` ("… El cuadro para elegir no se pudo abrir (…): se
   planifica con todas; usa Más… > Completar selección o selecciona a mano si no era lo que querías.") y `ribbon_batch_assist`
   lleva `choice: "all_without_dialog"`. Los otros tres `TaskDialog` con `DefaultButton` del add-in (sustituir plantilla,
   eliminar plantilla, borrar conexión) fijan `CommonButtons` antes y usan `Yes`/`No`: no les pasa. En la nube no se podía ver:
   `TaskDialog` solo existe dentro de Revit (2.2).
2. **Las etiquetas no se pincharon** ni se anotó si se veían: consecuencia directa del punto 1 (la persona no pasó del paso
   10-3). Nada que corregir en el código hasta que se prueben: **se repite entero el paso de las etiquetas en la 10b** (ver,
   pinchar la 4, tabla y mapa, ventana cerrada, globo).
3. **`--puente` 14/22**: dos causas ajenas al add-in. (a) El documento activo era `MODELO CERCO` cuando corrió el script (la
   persona tenía abierto otro modelo y Revit cambió de documento activo entre el sondeo 12 y el `ping`): las pruebas con los
   IDs del fixture fallan con `ELEMENT_NOT_FOUND` en cualquier otro modelo, como está previsto. (b) El servidor del puerto
   8000 no estaba escuchando a los 30 s del `Start-Process` (el `.bat` no arrancó o tardó más). Nada que corregir en el
   script: la 10b exige **un solo proyecto abierto** (el `ping` tiene que decir `HANGAR_PRUEBA_sondeo` antes de lanzarlo),
   espera 45 s y comprueba el puerto 8000 con `Get-NetTCPConnection` antes de la prueba; si fallan solo la 31 y la 32, se
   repite el bloque. **El 32/32 en el PC sigue pendiente** (igual que el 30/30 lo estaba en la Fase 9).
4. **La plantilla sobrante del catálogo** (`PRUEBA probar_conexiones`): la dejó el `--puente` de la Fase 9 al caerse en la
   prueba 22 (la 28 es la que la borra); en la Fase 10 la 19 falló (otro modelo) y tampoco llegó a la 28. Hizo que el encargo
   usara esa plantilla como ejemplo en vez de `Nudo tipico Detalle D` (las dos tienen 3 barras y gana la más reciente) y haría
   que Planificar lote desde la cinta casara contra dos plantillas iguales. La 10b la borra con `catalog_delete` en el paso
   10b-2 y comprueba `templates_count: 1` al final (la 28 del `--puente` borra la suya).
5. **Lo que sigue sin anotar** (sin captura ni texto): el cuadro de confirmación y la barra de estado de Borrar el lote, si
   Ctrl+Z devolvió las marcas y los fantasmas, Editar nudo con la ventana abierta, `PLAN_MARKS_REPLACED`, el cuadro y el
   contenido del encargo. Nada en el log en contra y las operaciones de debajo (`batch_delete 16/16`, `labels_applied` tras
   replanificar, `ribbon_design_brief` con `clipboard: true`) sí están; no se repiten en la 10b (`CLAUDE.md`: solo lo que no
   se pudo probar), se anotan como "sin ver" en el README.

### 7.3 Qué cambió en el código (0.10.1)

- `src/MotorConexiones.Revit/BatchPlanCommand.cs`: `DefaultButton` después de `AddCommandLink`; el cuadro en un `try` con la
  opción por defecto si falla (7.2, punto 1) y las dos líneas de log nuevas (`ribbon_batch_assist_dialog_failed`,
  `choice: "all_without_dialog"`). Comentario del porqué junto al cuadro.
- Versión **0.10.1** en `AddinInfo`, los dos csproj, `mcp/revit_mcp/conexiones.py` (`VERSION_ADAPTADOR`),
  `mcp/tools/conn_tools.py` (`VERSION_HERRAMIENTAS`), `mcp/pruebas/simulador_revit.py` (`ADDIN_VERSION`) y `mcp/CONTRATO-conn.md`
  (como en los cierres de la Fase 8: solo cambia la versión; sin cambios de rutas, claves ni herramientas).
- Documentación: este informe (estado, 2.1, 2.2, 5, 6 y esta sección), `docs/instalacion/fase-10.md` (nota de "hecha"),
  **`docs/instalacion/fase-10b.md`** (7.6), README (estado, tabla de garantías, árbol, instalación, secciones 9 y 12),
  `docs/propuestas/flujo-intuitivo.md` (C6, V2, V3 y la fila de la Fase 10), `docs/propuestas/encargo-ia-externa.md` (estado),
  `docs/fases/fase-9.md` (7.6 cerrado salvo el `--puente`) y `docs/fases/resumen-fase-10-cierre.md`.
- Nada cambia en `config/`, `docs/guide.md`, `catalog/` ni en los sondeos.

### 7.4 Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` y NuGet sirvió los paquetes de la API 2027, así que la 0.10.1 compila
entera (también `MotorConexiones.Revit`, con el `TaskDialog` corregido).

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> .../MotorConexiones.Core.dll
  MotorConexiones.Tests -> .../MotorConexiones.Tests.dll
  MotorConexiones.Revit -> .../MotorConexiones.Revit.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   217, Skipped:     0, Total:   217, Duration: 890 ms - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
Autocomprobación: 66/66 correctas

$ python3 mcp/pruebas/simulador_revit.py &  (token en el archivo literal "%LOCALAPPDATA%\RevitMcp\token" de la carpeta actual)
$ python3 mcp/pruebas/probar_conexiones.py
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.10.1 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
29. POST /conn/batch/plan/ (expand_selection:true, mark:false) desde UNA barra -> ...  [OK]  HTTP 200, ok=True, avisos=['SELECTION_EXPANDED', 'CATALOG_EMPTY'], añadidas=3 ...
30. POST /conn/batch/plan/discard/ del plan ampliado -> descartado, removed_labels 0  [OK]  HTTP 200, ok=True, ... etiquetas quitadas=0
Resultado: 30/30 pruebas correctas
```

Sin pruebas nuevas: el cambio está en el único archivo del add-in que abre cuadros y las pruebas del Core no lo alcanzan
(`TaskDialog` solo existe en Revit). La corrección se comprueba en el PC (7.6).

### 7.5 Lo pendiente de la Fase 9 (7.6 de `fase-9.md`), cerrado con la 0.10.0

| Qué (`fase-9.md` 7.6) | Resultado en la instalación de la Fase 10 |
|---|---|
| **Borrar el lote desde la ventana** (cuadro, barra de estado, replanificación; sondeos 12 y 13 a cero) | **Hecho**: `ribbon_batch_delete` `deleted 16, failed 0` (144 elementos, 48 barras restauradas, 1,1 s, una entrada de deshacer), replanificación justo después (`labels_removed 17`, `labels_applied 33`), sondeos 12 y 13 a cero. El texto del cuadro y de la barra de estado, sin anotar |
| `--puente` 30/30 (ahora 32/32) con la corrección de la consola | **Pendiente otra vez** (14/22 por el documento activo y el servidor; 7.2 punto 3). La corrección de la consola sí funcionó: el script llegó al final e imprimió `Resultado:` con la salida en cp1252 (los "●" salieron como `?`) |
| `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta, el cordón inferior, los textos de la ventana | **Sin ver** (7.1); nada en el log en contra; sin marcas dobles (`discard all` y sondeos 17 y 21 a cero) |
| Ctrl+Z devuelve las marcas de los nudos creados | Ctrl+Z se hizo (entre los dos lotes) y **Replanificar repuso las 33 etiquetas y los 16 listos**; si las marcas y los fantasmas volvieron solos con Ctrl+Z, sin anotar |
| El texto de los 3 `REVIT_WARNING` por nudo | **Confirmado**: "Advertencia de Revit: The created elements are only visible in Detail Level: Fine." ×3 en el `create` del Detalle D en la {3D} (uno por cartela, placa cuchilla y pernos) |
| El aviso del borrado seguido con el texto corregido | **La rama nueva se ejecutó** (`fabrication_transaction_failed` con `for_deletion: true` ×6 al borrar el lote de 16, desde la 11.ª); su texto es el de "para borrar: … se borran directamente" (código); el aviso tal cual lo enseñó la ventana, sin anotar. El borrado fue completo |
| `stop_on_error: true` con 16 nudos y el plan B `batch_single_undo: false` | Sin probar (no hizo falta; sigue en la lista de lo de siempre) |

### 7.6 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-10b.md`, unos 30 minutos, sobre la copia; no crea conexiones)

| Qué | Paso | Qué se espera |
|---|---|---|
| `deploy.ps1` 0.10.1.0, `instalar-conn.ps1` 25 rutas y 23 herramientas, `ping` 0.10.1 con `document.title: HANGAR_PRUEBA_sondeo`; `catalog_delete` de la plantilla sobrante → `templates_count: 1`; sondeos 12, 13 y 21 a cero | 10b-1, 10b-2 | Como en las fases anteriores; un solo proyecto abierto |
| **Planificar lote desde la cinta con una barra**: el cuadro "Selección asistida: 63 barras tocan la selección" (texto literal), Planificar con las 64 barras → los mismos 16 listos y la cabecera con "63 añadidas por la selección asistida" y "33 etiquetas pinchables y 16 cartelas fantasma"; Cancelar; Solo la selección | 10b-3 | Capturas `fase10b-01` y `fase10b-02`; en el log `ribbon_batch_assist` con `choice` y **ningún** `ribbon_batch_plan_failed` ni `ribbon_batch_assist_dialog_failed` |
| **Etiquetas**: se ven (y las cartelas fantasma, de paso), **pinchar la 4 sin ningún cuadro**, la ventana elige N4 y su barra de estado lo dice, resaltado desde la **tabla** (N7) y desde el **mapa** (12), una roja, el clic con la ventana **cerrada**, el globo; Descartar plan deja 0 etiquetas | 10b-4 | Capturas `fase10b-03` y `fase10b-04`; `label_clicked` con `node`, `highlighted: true` y `window_open`, `ribbon_batch_label_clicked`; sin `label_click_failed` ni `label_highlight_failed` |
| **`probar_conexiones.py --puente` 32/32** con la copia como único proyecto abierto y el servidor del puerto 8000 escuchando (`Get-NetTCPConnection`), sin cambiar de modelo | 10b-5 | `documento=HANGAR_PRUEBA_sondeo` en la 2; la 19 crea y la 28 borra su plantilla; la 31 (23 herramientas) y la 32 por el puente |
| Catálogo con 1 plantilla, sondeos 12, 13 y 21 a cero y el log del día | 10b-6 | `"addin_version":"0.10.1"`; ninguna línea de error de las listadas |

### 7.7 NO PROBADO en la nube y por qué

- **El cuadro corregido**: `TaskDialog` solo existe dentro de Revit; en la nube solo se comprueba que compila. El orden
  nuevo (`AddCommandLink` antes de `DefaultButton`) es el que exige la API (el botón por defecto tiene que existir), y si aun
  así fallara, el `try` planifica con todas y lo dice.
- **Las etiquetas pinchadas desde el add-in**, el resaltado desde la tabla y el mapa y el globo (2.2 sigue valiendo: el
  sondeo 19 v4 probó el mecanismo desde IronPython; el add-in las **pone y las quita** bien, 7.1).
- **Dónde salen las cartelas fantasma** (si coinciden con la cartela de la Fase 7, `fase7-02`): existen (16, con
  `ghost_element_id`) y se quitan; su posición solo se ve en Revit.
- **La votación del plano con barras fuera del plano que también tocan**: en la cercha de la 8c no hay (`skipped_out_of_plane:
  []` las dos veces); sigue probada solo con la riostra sintética.
- **El clic con Revit ocupado** (durante un `PickObject`): sin probar; Revit no entrega clics del lienzo mientras pincha.

### 7.8 Decisiones del cierre

- **Hay ronda 10b** porque la corrección está en `src/` y es la que impide usar la selección asistida desde la cinta (lo
  que más se iba a usar); la ronda despliega la 0.10.1 y repite **solo** lo que no se pudo probar (`CLAUDE.md`), no la
  instalación entera.
- **Si el cuadro falla, se planifica con todas** (no solo con la selección): es la opción por defecto del cuadro y lo que
  significa pulsar Planificar lote con una barra; con la selección sola, una barra no da ningún nudo. La barra de estado lo
  dice y el log lo distingue (`all_without_dialog`).
- **Adaptador, herramientas y simulador suben a 0.10.1 con el add-in** aunque no cambien, como en los cierres de la Fase 8:
  una sola versión que leer en `ping` y en el manual.
- **No se añaden pruebas**: el cambio no es alcanzable desde el Core y la prueba que vale es la del PC (10b-3).
- **La plantilla sobrante se borra por el puente**, no a mano ni con un script nuevo (`conn-call.ps1 -Operation catalog_delete`
  ya existe); el `--puente` se queda como está: su 28 borra la suya cuando llega.
- **Las dos causas del 14/22 se atajan con el procedimiento** (un solo proyecto abierto, `ping` antes, 45 s y
  `Get-NetTCPConnection`), no con código: `probar_conexiones.py` ya dice el documento en la prueba 2 y la 31 ya explica que
  falta el servidor.

### 7.9 Pendientes y preguntas (los de la sección 5, al día)

- **Riesgo 1** (etiquetas desde el add-in): **puestas y quitadas, sí** (`labels_applied` 33 cinco veces, `labels_cleared`,
  sondeo 21 a cero); **pinchadas, pendiente** (10b).
- **Riesgo 2** (la selección asistida añade de más o de menos): **cerrado** para la cercha de la 8c: 63 añadidas en 11
  rondas, ninguna fuera del plano, el mismo plan de siempre (16 listos, 10 sin plantilla, 7 empalmes, 8 sin cordón, 18
  sueltas).
- **Riesgo 3** (fantasmas en espejo): existen en los 16; dónde salen, pendiente de ver (10b-4.1, de paso).
- **Riesgo 4** (`Clear()` quita todos los controles): se ejecutó (`labels_cleared 33 → 0`) sin nada ajeno en el lienzo.
- **P1** (etiquetas un poco por encima del punto de trabajo), **P3** (la ventana delante al pinchar): se deciden al ver
  `fase10b-03` y `fase10b-04`. **P2** (fantasma de la placa cuchilla) y **P4** (`conn_design_brief`): siguen.
- Lo de siempre: `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`, el botón Conectar, V4 y V5 (Fase 11), la cartela
  automática (Fase 12), la placa de extremo, `stop_on_error` con 16 nudos y el plan B del deshacer.

### 7.10 Qué hace la persona ahora

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-10b.md` entero (unos 30 minutos, sobre la copia, con la copia
   como **único** proyecto abierto).
2. Hacer los pasos marcados **(la persona)**: pinchar **una** barra y Planificar lote (ahora sí debe salir el cuadro de la
   selección asistida), mirar las etiquetas, **pinchar la 4** (sin ningún cuadro), N7 en la tabla y 12 en el mapa, una roja,
   el clic con la ventana cerrada, el globo y Descartar plan. Cuatro capturas (`fase10b-01` a `fase10b-04`).
3. Devolver `docs\fases\resultados-fase-10b.md` con las anotaciones y abrir la sesión de cierre con el prompt de
   `docs/fases/resumen-fase-10-cierre.md` (sección 4). Si todo coincide, la Fase 10 queda cerrada del todo y empieza la 11.
