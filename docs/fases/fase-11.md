# Fase 11: conectar un nudo (botón Conectar sin JSON, panel por secciones, miniaturas del catálogo y el plano al lado)

Fecha: 2026-10-12. Rama: `main`. Add-in **0.11.0** (Core con `Catalog/TemplateChooser`, `Catalog/TemplateSketch`,
`Editing/PanelChoices`, `Editing/SpecStarter`, `SpecEditor.TrySetLabeledValue` / `TrySetAttachmentType` y la clave
`source.drawing_image`; add-in con `ConnectCommand`, `UI/SpecPanel`, `UI/DrawingPane`, `Catalog/TemplateThumbnails`,
`PanelChoicesLoader`, la previsualización con pestaña Avanzado y Plano…, miniaturas en el catálogo, en `ChooseDialog` y en el
detalle del nudo del plan, y los tres retoques de paso de la Fase 10), adaptador 0.11.0 (25 rutas, sin rutas nuevas) y 23
herramientas `conn_*` (`conn_catalog_apply` con `template_id` opcional). Prompt y alcance: `docs/prompts/fase-11.md`, escrito a
partir de `docs/propuestas/flujo-intuitivo.md` (sección "Y para un nudo suelto", V4, V5 y la fila 11 del calendario) y de
`docs/fases/fase-10.md` 8.2 y 8.5. Decisiones de la persona (D1 a D10): sección 2 del prompt.

**Estado: programada y probada en la nube; NO PROBADA en Revit.** Todo lo de WPF y Revit (el panel, los PNG de las miniaturas,
el plano al lado y el pegado del portapapeles, el botón Conectar de punta a punta, los títulos de los cuadros y los dos
retoques del lote) se prueba en el PC con `docs/instalacion/fase-11.md` (unos 75 minutos, sobre la copia; crea dos conexiones
y las borra). La última versión desplegada en el PC es la 0.10.1.

---

## 1. Qué se hizo

- **Botón Conectar** (`ConnectCommand.cs`, el primero del panel de la cinta; decisiones D1, D2 y D3). Con el cordón y las
  barras del nudo seleccionadas: filtra lo que no sean barras de armazón con eje; con **una** barra pide pinchar el cordón y
  las demás en Revit (`PickObject` uno a uno hasta Esc, con la misma regla de filtro que la ventana del plan); con cero, un
  cuadro corto. Si la selección forma **más de un nudo con cordón** (el detector del plan de lote sobre la selección), el
  cuadro dice "Esto parece una cercha (N nudos): usa Planificar lote" y un enlace abre Planificar lote con esa selección.
  Lee el nudo con `TemplateNode.FromModelFacts` (la regla de `conn_get_node_info` y de `conn_catalog_apply`: cordón el más
  horizontal, marco canónico, ángulos con signo, anchos del modelo); los errores del nudo (`NODE_AXES_NOT_INTERSECTING`,
  `ELEMENT_NOT_A_MEMBER`…) van en un cuadro con el `hint`. Casa **todas** las plantillas del catálogo con `TemplateChooser`
  y abre `PreviewWindow` (modal, el cuadro del botón) con la mejor ya instanciada (`TemplateInstantiator`, el camino de
  Aplicar a la selección) y una `PreviewSession` virtual ("Conectar · nudo del cordón 1249510 · Nudo tipico Detalle D"); si
  ninguna encaja, con la **especificación de arranque** de `SpecStarter`. Si el nudo ya tiene una conexión del add-in
  (mismo cordón y alguna barra, la regla del plan), la cabecera lo avisa sin bloquear. **No crea nada al pulsarse**: Crear hace
  lo mismo que los demás botones (`RibbonCreation.CreateFromSession`, operación `connect_ribbon`). Log `ribbon_connect`
  (`selection`, `members`, `templates`, `candidates`, `proposal` o `starter`, `mode`, `is_valid`, `duration_ms`),
  `ribbon_connect_pick`, `ribbon_connect_truss`, `ribbon_connect_cancelled` y `ribbon_connect_failed`.
- **`Catalog/TemplateChooser.cs`** (Core): `Propose(node, templates, config, requireAllMembers)` casa cada plantilla en las
  cuatro orientaciones (`TemplateMatcher.Match` / `MatchAll`), se queda con las que casan **todas sus ranuras** y, por
  defecto, **no dejan barras sobrantes** (con 3 barras seleccionadas una plantilla de 2 dejaría una sin conectar), las ordena
  por suma de desvíos, después desvío máximo, después la más reciente (`created_utc`), y escribe el `SummaryText` en español
  ("Plantilla propuesta: Nudo tipico Detalle D · igual · desvío máx. 0,0°. Otras que encajan: ninguna." / "Ninguna de las 2
  plantillas encaja con 3 barras a +135,0°, +45,0° y −135,0°: Dos barras → sobran barras 1249636 [same]…"). Con el catálogo
  vacío, `IsCatalogEmpty` y el texto de C9. **`PlanBuilder` lo usa** con `requireAllMembers: false` (la regla de la Fase 8: la
  barra sobrante queda en `unassigned_members`), así el plan sigue dando los mismos 16 listos de la cercha de la 8b (las
  pruebas `BatchPlanTests` no cambian). A igual desvío gana ahora la más reciente (antes, la primera de la lista).
- **`Editing/SpecStarter.cs`** (Core; D2 y D10): `FromNode(node, choices, templates)` escribe una especificación válida en
  esquema con los IDs y perfiles del modelo, rol por la inclinación (≥ 80° = vertical), una unión **soldada en ranura** por
  barra (retiro, ranura y soldadura), una **cartela rectangular** en el marco canónico que envuelve los extremos recortados
  (del retiro al final de la ranura más `gusset_margin_mm`, con el ancho de cada barra) y asoma por los dos lados del cordón
  (medio canto más `gusset_beyond_chord_mm`), redondeada a 5 mm; `source.drawing` = "Especificación de arranque (Conectar)";
  `gusset.chord_interface` como duda **confirmada** (`through_slot`). Los valores salen **primero de la plantilla más reciente
  con el mismo perfil de cordón** (`ProfileMatcher.Matches`; su espesor y rótulo, unión al cordón, soldadura al cordón y la
  ranura, retiro y soldadura de su primera barra soldada) y, si no hay, de `choices.json`; `DefaultsSource` dice de dónde
  ("valores de la plantilla Nudo tipico Detalle D" / "valores por defecto de choices.json") y `HeaderText` avisa de que es un
  arranque y no la cartela automática (Fase 12).
- **`Editing/PanelChoices.cs`** (Core; D5) y **`config/choices.json`** (nuevo): `thickness_labels` (las dos listas: `1/4"` …
  `1"` y `PL6` … `PL20`), `bolt_diameter_labels` (`1/2"`, `5/8"`, `3/4"`, `7/8"`), `weld_sizes_mm` (3, 4, 5, 6, 8, 10),
  `slot_length_mm`, `end_setback_mm`, `attachment_types`, `chord_interfaces`, `gusset_faces`, `roles` (con sus textos en
  español: las palabras de D6) y `starter` (valores del arranque, placa cuchilla y pernos por defecto). Convierte cada rótulo a
  mm con `LabelParser` (`3/8"` → 9,525; `PL10` → 10) y al revés encuentra la opción de un valor (`OptionFor`, tolerancia
  `label_value_tolerance_mm`) o devuelve "Otro: 9,2 mm" sin cambiar el JSON. `FilterBoltDiameters(limits)` deja solo los
  diámetros que existen en `limits.json` (los descartados van al log `choices_bolt_labels_discarded`); los demás de
  `limits.json` siguen aceptados desde Avanzado. Sin archivo o con un JSON roto, listas embebidas (nunca un panel vacío).
  `PanelChoicesLoader` (add-in) lee `config\choices.json` desplegado y aplica el filtro.
- **`SpecEditor`** (Core): `TrySetLabeledValue(rawJson, pathMm, pathLabel, label)` escribe **a la vez** `*_mm` y `*_label`
  (nunca deja un `LABEL_VALUE_MISMATCH` a medias); `TrySetAttachmentType(rawJson, memberIndex, type, defaults, restored)`
  cambia `welded_slot` ↔ `bolted_knife_plate` creando el bloque que falta con los valores de `choices.json`
  (`DefaultAttachment`) o reponiendo el bloque guardado (`restored`), y devuelve el sustituido para que la sesión lo guarde en
  memoria; `ListFields` enseña la sección **Origen** (`source.drawing` y `source.drawing_image`).
- **El panel por secciones** (`UI/SpecPanel.xaml(.cs)`; D4 y D6). Un `UserControl` con un expansor por sección construido
  desde `SpecEditor.ListFields` (Cartela, Cordón, Barra 1 · diagonal · +45,0° · 1249630…; las cadenas de cotas, las dudas y el
  origen van a Avanzado): **desplegable** para espesor de cartela y de placa (rótulo + mm en uno), diámetro de perno,
  soldadura, tipo de unión, unión al cordón, cara de la cartela y rol; **casilla** para los sí/no; **número o texto** para el
  resto (Enter o salir del campo aplica; Esc vuelve); **solo lectura** para los IDs y el perfil del modelo. Todo cambio pasa
  por `PreviewSession` (`TrySetField`, `TrySetLabeledValue`, `TrySetAttachmentType`): **una sola fuente de verdad, el JSON**;
  la ventana redibuja y valida como hoy. Al entrar en un campo su cota se resalta en naranja; los errores con `path` se
  enseñan junto al campo (borde rojo y el mensaje debajo; `*_label` se asigna al desplegable de su `*_mm`). Si las rutas no
  cambian, el panel actualiza los valores en sitio (sin perder el foco ni el desplegado); si cambian (otra unión), se
  reconstruye. **Cabecera**: el texto de la propuesta o del arranque, la miniatura de la plantilla aplicada (V4), **Cambiar…**
  (`ChooseDialog` con las candidatas: miniatura, orientación y desvío; elegir otra vuelve a instanciar con
  `PreviewSession.TryApplyCandidate`), **Encargo para IA** en los arranques (`DesignBriefCommand.Run`, el mismo código del
  botón de la cinta sobre las barras del nudo) y **Abrir catálogo** con el catálogo vacío.
- **La previsualización** (`UI/PreviewWindow.xaml(.cs)`): la columna derecha es un `TabControl` con **Panel** y **Avanzado**
  (la tabla de siempre, el contorno con Aplicar contorno, el JSON solo lectura con **Copiar**); el botón **Plano…** sobre el
  croquis (Abrir archivo…, Pegar del portapapeles, Mostrar/Ocultar plano, Quitar); `Abrir del catálogo` deja también la
  cabecera y abre el plano de la plantilla si existe. Desde la ventana del plan, **Editar nudo** abre la misma ventana y estrena
  el panel sin código nuevo.
- **V5, el plano al lado** (`UI/DrawingPane.xaml(.cs)`; D8): panel a la **izquierda** del croquis con `GridSplitter` (oculto
  hasta que se abre una imagen), `Image` con rueda = zoom alrededor del cursor, arrastrar = encuadre, **Ajustar** y **Ocultar
  plano** (pliega sin olvidar). `Load(path)` acepta PNG y JPG; `TrySaveClipboardImage` guarda la imagen del portapapeles como
  `%LOCALAPPDATA%\MotorConexiones\planos\plano-<documento>-<fecha-hora>.png` (si hay texto o nada, lo dice y no pasa nada). La
  ruta se recuerda en **`source.drawing_image`** (clave nueva y opcional del contrato: `SourceInfo.DrawingImage`, esquema,
  `AllowedSourceProperties`, `SpecEditor`); `TemplateBuilder` la copia a **`origin.drawing_image`** de la plantilla (y la quita
  de `spec_template`), `TemplateInstantiator` la devuelve a `source.drawing_image` al aplicar: el plano se abre solo si el
  archivo existe (si no, aviso en la barra de estado y `drawing_pane_missing` en el log, nunca un error). PDF directo, no.
- **V4, miniaturas** (`Catalog/TemplateSketch.cs` en el Core, `Catalog/TemplateThumbnails.cs` en el add-in; D7).
  `TemplateSketch.Build(template, limits)` instancia `spec_template` con IDs ficticios (cordón 1, barras 2, 3…) y construye el
  `SketchNodeInfo` con los ángulos con signo del `member_pattern` y los anchos del perfil (`ProfileMatcher.TryParseHssDimensions`;
  `DefaultMemberWidthMm` si no se reconoce): el mismo `Sketch` que pinta la ventana. `TemplateThumbnails.EnsurePng` lo pinta en
  un `SketchCanvas` fuera de pantalla con `ShowAnnotations = false` (solo cordón, barras, cartela, placas, pernos y soldaduras),
  `Measure` / `Arrange` / `FitTo`, `RenderTargetBitmap` + `PngBitmapEncoder`, 240×180, en `<template_id>.png` **junto al
  `.json`**; lo rehace si falta o el `.json` es más nuevo; si el PNG sale en blanco o falla, hueco gris con el nombre
  (`Placeholder`) y `thumbnail_failed` en el log, nunca bloquea. Se generan al abrir la ventana del catálogo (`EnsureAll`) y al
  guardar una plantilla (`CatalogWindow.SaveAskingToOverwrite`; con "copiar a la compartida" el PNG va con el JSON). Se ven en
  la **ventana del catálogo** (columna Croquis y vista grande de la elegida), en **Abrir del catálogo**, en **Cambiar…**, en
  **Plantilla…** (`ChooseDialog` con `ChoiceItem.Image`) y en el **detalle del nudo** de la ventana del plan (`PlanSnapshot.
  ThumbnailPaths`, que `BatchPlanner.SnapshotOf` rellena en contexto válido); en el mapa, no. `deploy.ps1` copia los `.png` de
  `catalog\` que falten. `conn_catalog_list` no cambia.
- **Los tres retoques de paso** (`fase-10.md` 8.2): (1) `BatchPlanner.Plan(…, presetExpansion)`: la selección asistida
  aplicada desde la cinta (`BatchPlanCommand.AssistSelection`, con todas o sin cuadro) y desde Más… > Completar selección se
  guarda en `plan.SelectionExpansion`, así la línea del plan dice "(63 añadidas por la selección asistida)" y
  `selection_expansion` viaja en `batch_plan_get`; (2) el log `label_clicked` lleva `changed` (si el clic cambió algo) y
  `highlighted` (el estado real de la etiqueta después del clic); (3) los títulos de los `TaskDialog` sin "MotorConexiones - ":
  constantes en `RibbonCreation` (`TitleConnect`, `TitleRunSpec`, `TitleCatalog`, `TitleBatchPlan`, `TitleDesignBrief`,
  `TitleConnections`, `TitleCreate`, `TitleError`) en los 29 usos. Los tres `MessageBox` de WPF de la ventana del plan no
  cambian (Revit no les antepone nada).
- **D9, `conn_catalog_apply` sin `template_id`** (entró): `CatalogApplyOperation` carga todas las plantillas, `TemplateChooser.
  Propose` elige, aplica la mejor en su orientación y devuelve `data.proposal` (`templates_count`, `chosen`, `candidates[]`,
  `attempts[]`, `member_angles_deg[]`, `summary_text`); sin encaje, `TEMPLATE_NO_MATCH` con `data.proposal`; catálogo vacío,
  `CATALOG_EMPTY`. Con `template_id`, igual que siempre (`proposal: null`). `conn_tools.py`: `template_id: str | None`.
  Ninguna ventana en las rutas de la IA.
- Versión **0.11.0** en `AddinInfo`, los dos csproj, adaptador, herramientas, simulador y contrato; `deploy.ps1` copia
  `config\choices.json` y los `.png`; contrato, guía (secciones 3 y 5), README, CLAUDE.md y las propuestas al día.

## 2. Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.401) se instaló con `dotnet-install.sh`; NuGet sirvió los paquetes de la API 2027. Todo compila
sin avisos, así que los miembros usados existen (`PickObject` y `Clipboard` ya se usaban; `RenderTargetBitmap`,
`PngBitmapEncoder`, `GridSplitter`, `TabControl`, `Expander` son de WPF, no de Revit).

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   238, Skipped:     0, Total:   238, Duration: 671 ms - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
  [OK] POST /conn/catalog/apply/ sin template_id -> la plantilla elegida, proposal.chosen y summary_text (Fase 11)
  [OK] POST /conn/validate/ con source.drawing_image -> ok:true (Fase 11)
  [OK] POST /conn/catalog/save/ con source.drawing_image -> plantilla con origin.drawing_image (Fase 11)
Autocomprobación: 69/69 correctas

$ python3 mcp/pruebas/simulador_revit.py --token-archivo sim-token &   (y probar_conexiones.py con --token-archivo sim-token)
21b. POST /conn/catalog/apply/ sin template_id -> la mejor plantilla (proposal.chosen = la de la prueba 19)  [OK]  HTTP 200, ok=True, elegida=PRUEBA probar_conexiones
Resultado: 31/31 pruebas correctas
```

Las 21 pruebas nuevas del Core (`src/MotorConexiones.Tests/Fase11Tests.cs`, 217 → 238):

| Prueba | Qué comprueba |
|---|---|
| `TemplateChooserTests` (6) | Con el nudo del Hangar y la plantilla exacta: `same`, desvío 0, el texto exacto de la propuesta; el nudo reflejado: `mirror_x`; dos plantillas (la del repositorio con 1,9° y la exacta con 0°), gana la de menor desvío aunque vaya segunda y la otra queda como candidata; a igual desvío gana la más reciente; una plantilla de 2 barras con 3: sin elección, 4 intentos y el texto "Ninguna de las 1 plantilla encaja con 3 barras a +135,0°, +45,0° y −135,0°: Dos barras → sobran barras 1249636" (y con `requireAllMembers: false`, la regla del plan, sí casa con la barra en `unassigned_members`); catálogo vacío |
| `SpecStarterTests` (4) | Sin plantillas: esquema correcto, los cuatro IDs, perfiles del modelo, `welded_slot`, retiro 60, ranura 150, soldadura 5, `3/8"` = 9,525, `through_slot`, cartela de 4 puntos múltiplos de 5 que **contiene** los extremos recortados de las tres barras (`Polygon2D.ContainsPoint`) y asoma ±75 del cordón, la duda confirmada, y **valida con token** con los hechos del Hangar; con una plantilla del mismo cordón: espesor 12,7 / `1/2"`, retiro 180, ranura 150; con dos del mismo cordón gana la más reciente y una de otro cordón se ignora; un montante se llama `vertical` con 90° |
| `PanelChoicesTests` (4) | `config/choices.json` del repositorio: 13 espesores que convierten, `3/8" · 9,5 mm`, `PL10 · 10,0 mm`, `5 mm`; los cuatro diámetros existen en `limits.json` (12,7 / 15,875 / 19,05 / 22,225), 25,4 sale como "Otro: 25,4 mm", un `2"` se descarta con su nombre; 9,525 → `3/8"`, 9,2 → "Otro: 9,2 mm"; sin archivo y con JSON roto, listas embebidas no vacías; listas vacías del archivo se rellenan |
| `TemplateSketchTests` (2) | El croquis de la plantilla oficial sin modelo: 1 cartela, 3 barras, 1 placa, 2 ranuras, 4 pernos, 2 bordes de cordón, cota de 565, sin nota de "aproximada"; IDs 1, 2, 3, 4, esquema correcto, ángulos 136,92 / 44,37 / −135,63, anchos 63,5 y 76,2 |
| `SpecEditorFase11Tests` (3) | `TrySetLabeledValue` 1/2" → 12,7 + rótulo sin `LABEL_VALUE_MISMATCH`, 3/4" en los pernos valida, un rótulo malo no cambia nada; `TrySetAttachmentType` crea placa y pernos por defecto (PL10, 170, +z, 5/8", 2×2, 40), no cambia nada si ya era ese tipo, repone la ranura de 123 con el bloque guardado y 150 sin él, índice fuera y tipo malo fallan; `ListFields` con `source.drawing_image` |
| `DrawingImageTests` (2) | El esquema acepta `drawing_image` con ruta, con `null` y ausente, y rechaza `source.drawing_pdf`; sin la clave el JSON serializa igual (el token de siempre no cambia); el validador no exige el archivo; `origin.drawing_image` sobrevive a `ToJson`/`FromJson`, a guardar en un `CatalogStore` y vuelve a `source.drawing_image` al instanciar; sin plano no se escribe la clave |

Las 217 pruebas anteriores pasan sin cambios (la extracción de `TemplateChooser` no altera el plan: los mismos 16 listos de la
cercha de la 8b).

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-11.md`, unos 75 minutos)

| Qué | Paso |
|---|---|
| `deploy.ps1` 0.11.0.0 copia `config\choices.json` y dice cuántos `.png` copió; `instalar-conn.ps1` 25 rutas y 23 herramientas; `ping` 0.11.0 | 11-1, 11-2 |
| **Conectar con plantilla** sobre el Detalle D: la ventana sin pedir archivo, "Plantilla propuesta: Nudo tipico Detalle D · igual…", el panel; espesor `1/2"` sin `LABEL_VALUE_MISMATCH`; pernos `3/4"` sin `BOLT_EDGE_DISTANCE_TOO_SMALL` con borde 40; Barra 1 a placa cuchilla y vuelta (ranura 150); Avanzado; **Crear** | 11-3 |
| **Plano al lado**: Abrir archivo con `detalle-D.png`, zoom y encuadre, Ocultar y volver, Pegar del portapapeles (`planos\`) | 11-4 |
| **Conectar sin plantilla** (cordón superior, 2 barras): la cabecera de arranque, la cartela rectangular, la validación (en verde o con los códigos anotados), Crear si valida | 11-5 |
| **Miniaturas**: la fila y la vista grande en el catálogo, el `.png` junto al `.json`, Guardar en catálogo con miniatura, Eliminar | 11-6 |
| **Retoques**: "(63 añadidas por la selección asistida)" desde la cinta, `changed` / `highlighted` en `label_clicked`, la miniatura en el detalle de N4 (el mapa sin miniaturas), los títulos sin "MotorConexiones - MotorConexiones" | 11-7 |
| Por el puente: `catalog_apply` sin `template_id` con `proposal`, `validate` con `drawing_image`, `list` 2, `delete` 2, sondeos a cero, `--puente` 34/34, el log sin `ribbon_connect_failed`, `thumbnail_failed`, `drawing_pane_failed`, `label_click_failed`, `plan_event_failed` ni `ribbon_batch_window_error` | 11-8 |

### 2.2 NO PROBADO en la nube y por qué

- **Todo lo de WPF y Revit**: el panel (controles, errores junto al campo, el refresco en sitio), los PNG de las miniaturas
  (`RenderTargetBitmap` solo pinta con un hilo de WPF y las fuentes del PC; la comprobación "todo blanco" y el hueco gris
  están para eso), el plano al lado y el pegado del portapapeles (`Clipboard.ContainsImage`), el botón Conectar de punta a
  punta (`PickObject`, `TaskDialog` con enlaces, la detección de "parece una cercha"), los títulos de los cuadros y los dos
  retoques del lote en Revit. Compila contra la API 2027; se prueba en el PC (2.1).
- **El arranque sobre nudos reales distintos del Detalle D**: con los hechos del Hangar valida con token; en un nudo con
  ángulos agudos la cartela rectangular puede chocar con una barra ajena (`CLASH_WITH_FOREIGN_MEMBER`) o salirse: es
  esperable, el panel lo enseña y no se intenta resolver en esta fase (Fase 12).
- **La prueba 32b del puente** (`conn_catalog_apply` sin `template_id` por el MCP) necesita el puente del puerto 8000: solo
  en el PC.

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. `resultados-fase-11.md`: `deploy` en `0.11.0.0` con `config\choices.json` entre los copiados y "miniaturas .png copiadas:
   0" (el repositorio no trae PNG: los dibuja el add-in); `ping` 0.11.0; `catalog_list` 1.
2. Captura `fase11-01-conectar`: la ventana abierta sin cuadro de archivo, la cabecera "Plantilla propuesta: Nudo tipico
   Detalle D · igual · desvío máx. 1,2°…" (el desvío real de la cercha frente a la plantilla medida en ella debe ser
   pequeño) con la miniatura, el croquis y el panel con tres expansores de barra.
3. Captura `fase11-02-panel`: el espesor `1/2" · 12,7 mm` en el desplegable y `PL 1/2" · 12,7 mm` en el croquis sin
   `LABEL_VALUE_MISMATCH` en la lista de abajo; la Barra 1 con placa y pernos tras cambiar la unión; el `connection_id` del
   cuadro de Crear y el sondeo 12 con 1.
4. Captura `fase11-03-plano`: `detalle-D.png` a la izquierda del croquis con el divisor; en el log `drawing_pane_opened`
   (`from: archivo` y `from: portapapeles`) y un `plano-HANGAR_PRUEBA_sondeo-<fecha>.png` en `planos\`.
5. Captura `fase11-04-arranque`: la cabecera "Ninguna de las 1 plantilla encaja con 2 barras a …: Nudo tipico Detalle D →
   falta barra para la ranura …" seguida de "Especificación de arranque con cartela rectangular (valores de la plantilla Nudo
   tipico Detalle D)" si el cordón superior es HSS3X3X1/4 (si es otro perfil, "valores por defecto de choices.json"); la
   cartela rectangular con las dos barras soldadas en ranura; los códigos de la validación. Si validó y se creó, el sondeo 12
   con 2.
6. Captura `fase11-05-catalogo`: la columna Croquis con la miniatura del Detalle D y la vista grande; `dir` con el `.png`
   junto al `.json`; la plantilla `PRUEBA fase 11` con su miniatura antes de eliminarla.
7. Captura `fase11-06-titulo`: un cuadro con el título "Planificar lote" o "Conectar" una sola vez bajo el nombre del add-in;
   la línea del plan con "(63 añadidas por la selección asistida)"; en el log `label_clicked` con `changed: true` y luego
   `changed: false`, las dos con `highlighted: true`; el detalle de N4 con la miniatura.
8. El `--puente` 34/34 (o 33/34 con la 32b anotada si el puente no la atendió) y el log del día sin los eventos de fallo de 2.1.

## 4. Decisiones tomadas y por qué

### 4.1 El nudo se lee con `TemplateNode.FromModelFacts`, no con `Bridge.Handle("node_info")`

El prompt decía "lo mismo que `conn_get_node_info`". `TemplateNode.FromModelFacts` es la misma regla (cordón el más
horizontal con `ChooseChord`, marco canónico, ángulos con signo) y es lo que ya usan `conn_catalog_apply` y el plan de lote;
además trae lo que la propuesta y el arranque necesitan (anchos de perfil, nombres de tipo, `ChordWidthMm`, nuevo) sin pasar
por un JSON. Los códigos de error son los mismos (`CatalogException` con `NODE_AXES_NOT_INTERSECTING`, `ELEMENT_NOT_A_MEMBER`…).
`DesignBriefCommand` sigue usando `Bridge.Handle("node_info")` porque su salida es ese JSON.

### 4.2 Una plantilla con menos ranuras que barras no es candidata para Conectar, pero sí para el plan

Con 3 barras seleccionadas, una plantilla de 2 ranuras "casa" en el sentido de la Fase 7 (todas sus ranuras tienen barra) y
deja una barra sobrante: para un nudo suelto eso es una barra sin conectar, así que `TemplateChooser` la descarta por
defecto (`requireAllMembers: true`) y lo dice ("sobran barras 1249636"). El plan de lote conserva la regla de la Fase 8
(`requireAllMembers: false`), porque cambiarla habría alterado los 16 listos de la 8b y es otra decisión. Queda anotado en 5.

### 4.3 A igual desvío gana la más reciente

Antes, en el plan, a igual suma de desvíos ganaba la primera de la lista (el orden alfabético del catálogo). El prompt pide
"la más reciente" y es lo que una persona espera (la última típica guardada). Afecta solo a empates exactos.

### 4.4 El panel actualiza en sitio y solo se reconstruye si cambian las rutas

Reconstruir el panel en cada edición perdía el foco (al salir de un campo con Tab) y el desplegado de las secciones.
`SpecPanel.Refresh` compara la lista de rutas con la construida: si es la misma, relee los valores control a control (con una
bandera que silencia los eventos); si cambió (otro tipo de unión), reconstruye recordando qué secciones estaban desplegadas.

### 4.5 Los desplegables de rótulo escriben los dos campos en una sola edición

`TrySetLabeledValue` convierte el rótulo con `LabelParser` (la misma regla del validador) y escribe `*_mm` y `*_label`
sobre el mismo JSON antes de devolverlo; así el croquis dice `PL 1/2" · 12,7 mm` al momento y nunca aparece
`LABEL_VALUE_MISMATCH` por un desplegable. Un valor que no está en la lista aparece como "Otro: 9,2 mm" (una opción que no
escribe nada) y se cambia en Avanzado.

### 4.6 El bloque de unión sustituido se guarda en memoria, no en el JSON

Al pasar una barra de soldada en ranura a placa cuchilla, el bloque anterior se guarda en `PreviewSession` (por índice de
barra) y se repone al volver: dentro de la sesión, sin ensuciar el contrato. Si no hay bloque guardado, los valores de
`choices.json`.

### 4.7 Las miniaturas se dibujan solo desde las ventanas

`EnsurePng` corre al abrir la ventana del catálogo, al guardar desde las ventanas, al abrir Cambiar… y en `SnapshotOf` del
plan (en el hilo de la ventana o del comando, siempre con WPF). `conn_catalog_save` por el puente **no** las genera: una ruta
de la IA no debe depender de `RenderTargetBitmap`; la siguiente ventana que las necesite las dibuja.

### 4.8 `catalog/` sigue sin `.png`

El PNG de la plantilla oficial no se puede dibujar en la nube (WPF). `deploy.ps1` copia los `.png` que haya en `catalog\`
(hoy ninguno) y el add-in dibuja el que falte al abrir el catálogo. Si la persona quiere la miniatura oficial en el
repositorio, basta copiar `%LOCALAPPDATA%\MotorConexiones\catalogo\6abcf116-….png` a `catalog\` tras la instalación.

## 5. Pendientes, riesgos y preguntas

- **Riesgo 1 (`RenderTargetBitmap` fuera de pantalla)**: puede devolver blanco en Revit. Está la comprobación de "todo
  blanco" (`IsBlank`) con hueco gris y `thumbnail_failed`; si pasa, la alternativa es pintar la miniatura en un `Viewbox`
  dentro de la ventana oculta o generarla desde el Core (como el BMP de las etiquetas). Se sabe en el paso 11-6.
- **Riesgo 2 (el arranque no valida)**: en el nudo del cordón superior puede dar `CLASH_WITH_FOREIGN_MEMBER` o
  `PLATE_OUTSIDE_GUSSET` (no hay placas en el arranque) o avisos de soldadura mínima. Es lo que la fase viene a averiguar
  (paso 11-5); no es un fallo de la instalación.
- **Riesgo 3 (`PickObject` uno a uno)**: Conectar pide pinchar barra a barra hasta Esc, como pedía el prompt; si en Revit
  resulta incómodo, `PickObjects` (Finalizar) es un cambio de una línea.
- **P1 (4.2)**: ¿debe el plan de lote adoptar también "sin barras sobrantes"? Hoy no, para no mover los 16 listos; si la
  persona prefiere que un nudo con una barra de más no se cree, es un cambio de un parámetro y una prueba.
- **P2**: los desplegables de soldadura no admiten escribir un valor fuera de la lista (se usa Avanzado). Si molesta, se
  hacen editables.
- Lo de siempre: `conn_design_brief`, `thumbnail_path` en `conn_catalog_list`, `conn_batch_update`, el traspaso de `mcp/`
  a `revit-mcp`, la placa de extremo, el fantasma de la placa cuchilla, `stop_on_error` con 16 nudos, el plan B del
  deshacer, y la **cartela automática** (Fase 12), que es lo que convierte el arranque en una conexión de verdad en nudos
  con otros ángulos.

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. Dar `docs/instalacion/fase-11.md` al agente instalador en el PC (Revit cerrado al empezar, sobre la copia).
2. Hacer los pasos marcados **(la persona)**: Conectar sobre el Detalle D, el panel, el plano, Conectar sobre el cordón
   superior, el catálogo con miniaturas y los retoques, con las seis capturas.
3. Devolver `docs/fases/resultados-fase-11.md` y las capturas; con eso se escribe el cierre de la fase (sección 7) y, si
   hace falta, una ronda 11b con las correcciones.
