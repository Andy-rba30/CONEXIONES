# Fase 11: conectar un nudo (botón Conectar sin JSON, panel por secciones, miniaturas en el catálogo y el plano al lado)

Segunda fase que sale de `docs/propuestas/flujo-intuitivo.md` después de la Fase 10 (sección 5, "Y para un nudo suelto";
sección 6, **V4** miniaturas y **V5** plano al lado; sección 7, fila 11 del calendario). Se ejecuta en **una sesión**, con
el prompt de la sección 1. Lo demás de este archivo es el alcance que esa sesión debe cumplir, escrito a partir de la
propuesta y de lo que la Fase 10 dejó anotado para esta (`docs/fases/fase-10.md`: 8.2 puntos 1 a 3 y 8.5, "De paso en la
Fase 11").

**Estado: decisiones fijadas el 2026-10-12; fase programada el 2026-10-12 (add-in 0.11.0, `docs/fases/fase-11.md`;
D9 entró; NO PROBADA en Revit: `docs/instalacion/fase-11.md`).** La persona respondió en el chat las diez preguntas de la
sección 9 (sus respuestas quedan allí como registro) y la sección 2 recoge lo decidido.

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/prompts/fase-11.md completo, docs/propuestas/flujo-intuitivo.md (sección "Y para un nudo suelto", filas
V4 y V5 y la fila 11 del calendario) y docs/fases/fase-10.md (secciones 8.2 y 8.5). Ejecuta solo la Fase 11 con el add-in 0.11.0 tal como la describe docs/prompts/fase-11.md y con las decisiones de su
sección 2: botón Conectar en la cinta (con las barras del nudo seleccionadas reconoce el nudo, casa todas las plantillas
del catálogo y abre la previsualización con la mejor ya aplicada, sin pedir ningún archivo JSON; si ninguna encaja, abre
una especificación de arranque con cartela rectangular y lo dice); el panel por secciones en la ventana de
previsualización (Cartela, Cordón, Barra 1-2-3) con desplegables de espesor, diámetro de perno, tipo de unión y
soldadura que escriben los mm y el rótulo a la vez, y una pestaña Avanzado con la tabla de hoy, el JSON, el contorno
punto a punto, las cadenas de cotas y las dudas; V4: miniaturas del croquis de cada plantilla en la ventana del
catálogo, en Abrir del catálogo y en el selector de plantilla del panel (PNG cacheado junto a la plantilla, dibujado por
el add-in a partir del croquis del Core); V5: el plano al lado del croquis (PNG o JPG desde archivo o pegado del
portapapeles, con zoom y encuadre, recordado en source.drawing_image). Incluye los tres retoques de paso de
fase-10.md 8.2: la expansión de la selección asistida guardada en el plan también desde la cinta, el campo changed y el
highlighted real en el log label_clicked, y los títulos de los TaskDialog sin el prefijo "MotorConexiones - ". La
miniatura de la plantilla se ve también en el detalle del nudo de la ventana del plan (no en el mapa). Los desplegables
de pernos ofrecen por defecto 1/2", 5/8", 3/4" y 7/8" (los demás diámetros de limits.json siguen aceptados desde
Avanzado). Los valores por defecto del arranque salen de la plantilla más reciente con el mismo perfil de cordón y, si no
hay, de choices.json. El contrato solo añade (source.drawing_image); nada cambia en las rutas conn_* salvo, solo si cabe
sin recortar nada del panel, template_id opcional en conn_catalog_apply. Ninguna ventana en las rutas de la IA. Termina con compilación sin avisos,
pruebas en verde (Core y simulador), docs/fases/fase-11.md, docs/instalacion/fase-11.md, README, docs/guide.md,
mcp/CONTRATO-conn.md, CLAUDE.md (estructura), las propuestas al día, commit, push y un resumen corto guardado también en
docs/fases/resumen-fase-11.md.
```

## 2. Decisiones de la persona (fijadas el 2026-10-12 con las respuestas de la sección 9)

| Pregunta (sección 9) | Decisión | Qué hace la Fase 11 con ella |
|---|---|---|
| D1. ¿Un botón o dos? (P6 de la propuesta, aplazado desde la 8c) | **Dos**: botón nuevo **Conectar** para un nudo y **Planificar lote** se queda como está para la cercha | Conectar no toca el lote. Si la selección da más de un nudo con cordón, Conectar lo dice y ofrece abrir Planificar lote |
| D2. Qué hace Conectar si ninguna plantilla encaja | **Especificación de arranque**: cartela rectangular que envuelve los extremos de las barras, unión soldada en ranura por barra, valores por defecto editables; la cabecera avisa de que es un arranque y ofrece Encargo para IA | `Core/Editing/SpecStarter.cs`, probado en la nube. La cartela automática de verdad (`outline.mode = "auto"`) sigue siendo la Fase 12 |
| D3. Cómo se elige el nudo | **Las barras del nudo seleccionadas** (cordón y barras que llegan), como hoy en Catálogo y Encargo para IA; con una sola barra, Conectar pide pinchar el cordón o las barras (`PickObject`, botón de la cinta: puede) | Sin selección asistida de cercha aquí: una barra sola arrastraría la cercha entera |
| D4. El panel sustituye a la tabla | **Sí**: la columna derecha de la previsualización pasa a ser el panel por secciones; la tabla de hoy, el JSON, el contorno, las cadenas y las dudas van a la pestaña **Avanzado** | Nada se pierde; el doble clic en las cotas del croquis sigue igual |
| D5. Listas de los desplegables y dónde viven | Espesores, **las dos listas**: `1/4"`, `5/16"`, `3/8"`, `1/2"`, `5/8"`, `3/4"`, `1"` y `PL6`, `PL8`, `PL10`, `PL12`, `PL16`, `PL20`; diámetros de perno **por defecto solo `1/2"`, `5/8"`, `3/4"` y `7/8"`** (`bolt_diameter_labels` en `choices.json`); los demás de `limits.json` (`1"`, `1-1/8"`, `1-1/4"`) siguen aceptados escribiéndolos en **Avanzado**; soldaduras: 3, 4, 5, 6, 8 y 10 mm; todo en un archivo nuevo **`config/choices.json`** editable sin recompilar | `deploy.ps1` lo copia como `limits.json`; un valor que no está en la lista se enseña como "Otro: 25,4 mm" y no se pierde |
| D6. Palabras del panel (P7 de la propuesta) | Las de hoy, confirmadas: **cartela**, **retiro**, **placa cuchilla**, **soldada en ranura**, **atornillada**, **en espejo**, **cordón**, **barra** | Los textos del panel usan exactamente esas palabras; `PlanAdvice` no cambia |
| D7. Miniaturas (V4): quién las dibuja y dónde se guardan | **El add-in** (WPF, `RenderTargetBitmap` del mismo `SketchCanvas`), a partir del croquis que calcula el Core sin modelo (`SketchNodeInfo.FromSpecAngles` con los ángulos del `member_pattern`); PNG de 240×180 **junto a la plantilla** (`<template_id>.png` en `catalog_folder`), rehecho si la plantilla es más nueva que el PNG. **También en el detalle del nudo de la ventana del plan** (la miniatura de la plantilla que le casó); **en el mapa de la cercha, no** | El Core se prueba en la nube (el croquis de una plantilla); el PNG solo en Revit. `deploy.ps1` copia también los `.png` de `catalog\` que falten |
| D8. Plano al lado (V5): formatos y sitio | La persona tiene los detalles **como imagen** (igual que `detalle-D.png`): **PNG y JPG** desde archivo (**Abrir plano…**) y **pegado del portapapeles** para los PDF (Ctrl+V tras un recorte con Win+Mayús+S); **PDF directo no**. Panel **a la izquierda** del croquis con divisor, se oculta con un botón; zoom y encuadre como el croquis. **Sí** se guarda la ruta en la especificación y en la plantilla | La imagen pegada se guarda en `%LOCALAPPDATA%\MotorConexiones\planos\`; la ruta queda en `source.drawing_image` (clave nueva, opcional) y en la plantilla (`origin.drawing_image`) para que vuelva a abrirse sola |
| D9. `conn_catalog_apply` sin `template_id` | **Sí, solo si cabe sin recortar nada del panel**: `template_id` opcional; sin él, la ruta casa todas las plantillas y aplica la mejor (`data.proposal` con las candidatas) | Un argumento opcional: el contrato solo añade. Es lo primero que se deja fuera si falta sitio (sección 10); entonces, pendiente anotado |
| D10. Valores por defecto del arranque | **Primero la plantilla más reciente con el mismo perfil de cordón** (su espesor, soldaduras, ranura, retiro y unión al cordón); si no hay, los de `choices.json`: cartela `3/8"`, soldaduras 5 mm todo el contorno, ranura 150 mm, retiro 60 mm, `through_slot` como duda confirmada | `SpecStarter` recibe las plantillas del catálogo y elige la fuente de los valores; la cabecera dice de dónde salieron ("valores de la plantilla Nudo tipico Detalle D" / "valores por defecto de choices.json") |
| Los tres retoques de paso (`fase-10.md` 8.4 y 8.5) | **Entran** los tres, ya decididos en el cierre de la 10 | Sección 3.6 |

## 3. Alcance (lo que se entrega)

### 3.1 Flujo completo (lo que cambia para la persona)

```
1. SELECCIONAR  pincha el cordón y las barras del nudo (dos o más barras) y pulsa  [ Conectar ]
                (con una barra sola, Conectar pide pinchar el cordón o las otras barras en Revit)
2. RECONOCER    el add-in lee el nudo (lo mismo que conn_get_node_info), casa TODAS las plantillas del catálogo y abre la
                ventana de previsualización con la mejor ya aplicada:
                "Plantilla propuesta: Nudo tipico Detalle D · en espejo · desvío máx. 1,2° · 3 barras  [Cambiar…]"
                Si ninguna encaja: "Ninguna plantilla encaja (2 intentos): especificación de arranque con cartela
                rectangular. Rellena el panel o pide el Encargo para IA." y la ventana se abre igual.
3. AJUSTAR      panel por secciones a la derecha del croquis:
                ┌ Cartela ───────────────────────────────────────────────────────────┐
                │ Espesor [3/8" · 9,5 mm ▾]  Ancho [565]  Alto [530]                  │
                │ Unión al cordón [atraviesa el cordón ranurado ▾]  Soldadura [5 mm ▾] ☑ todo el contorno │
                ├ Cordón ────────────────────────────────────────────────────────────┤
                │ HSS3X3X1/4 (del modelo) · continuo                                 │
                ├ Barra 1 · diagonal · +45° ─────────────────────────────────────────┤
                │ Unión [soldada en ranura ▾]  Retiro [180]  Ranura [150]  Soldadura [5 mm ▾] │
                ├ Barra 3 · diagonal · −135° ────────────────────────────────────────┤
                │ Unión [placa cuchilla atornillada ▾]  Retiro [260]                 │
                │ Placa: espesor [PL10 ▾] largo [170] ancho [140] inserción [80] cara [+z ▾] │
                │ Pernos: Ø [5/8" · 15,9 mm ▾] filas [2] columnas [2] paso [60] borde [40] 1.ª fila [40] │
                │ Longitud: se calcula del agarre (44,5 mm)                          │
                └ Avanzado (pestaña): tabla de hoy · JSON · contorno punto a punto · cadenas de cotas · dudas ┘
                Cada cambio redibuja el croquis y vuelve a validar (lo mismo que hoy con la tabla).
                [Plano…] abre la imagen del detalle al lado del croquis para comparar.
4. CREAR        [Crear] solo con la validación en verde: la misma operación atómica de siempre (Fase 3/6).
                [Guardar en catálogo] si es una típica nueva: la plantilla sale ya con su miniatura.
```

### 3.2 Botón **Conectar** (add-in, `ConnectCommand.cs`; la elección de plantilla en el Core, se prueba sin Revit)

- **`Catalog/TemplateChooser.cs`** (Core): `TemplateChooser.Propose(node, templates, options)` casa un `TemplateNode` con
  **todas** las plantillas del catálogo con `TemplateMatcher.MatchAll` (las cuatro orientaciones, la misma regla que el
  plan de lote: `PlanBuilder` ya lo hace nudo a nudo y se extrae aquí para compartirlo) y devuelve una `TemplateProposal`
  con las candidatas **completas** ordenadas por desvío (`Score`, después `MaxDeviationDeg`, después la más reciente), la
  elegida, los intentos fallidos con su `Describe()` y un `SummaryText` en español ("Plantilla propuesta: Nudo tipico
  Detalle D · en espejo · desvío máx. 1,2°. Otras que encajan: ninguna." / "Ninguna de las 2 plantillas encaja con 3 barras
  a +45°, −45° y −135°: Detalle D → falta barra para la ranura 2…"). Con el catálogo vacío, el mismo aviso `CATALOG_EMPTY`
  y botón **Abrir catálogo** de la Fase 8 (C9).
- **`Editing/SpecStarter.cs`** (Core, decisión D2): `SpecStarter.FromNodeInfo(nodeInfo, defaults)` escribe una
  especificación válida en esquema con los IDs y perfiles del modelo, una unión **soldada en ranura** por barra con el
  retiro, la ranura y la soldadura por defecto de `choices.json`, y una **cartela rectangular** en el marco canónico que
  envuelve los extremos recortados de las barras (retiro más ranura más un margen, redondeado a 5 mm) y el ancho del cordón;
  `source.drawing` = "Especificación de arranque (Conectar)". Lo que no se sabe (unión al cordón) va a `uncertain_fields`
  con el valor por defecto confirmado, para que el panel lo enseñe como duda. Los valores por defecto se toman **primero** de la
  plantilla más reciente con el mismo perfil de cordón (decisión D10) y, si no hay ninguna, de `choices.json`; la cabecera
  dice de dónde salieron. Es un punto de partida, no
  la cartela automática (Fase 12): la cabecera lo dice y `conn_validate` manda igual (si una barra se sale, lo marca).
- **`ConnectCommand.cs`** (add-in): con dos o más barras seleccionadas, `Bridge.Handle("node_info")` (el mismo código que
  `conn_get_node_info` y que usa `DesignBriefCommand`), `TemplateNode` del nudo, `TemplateChooser.Propose` con el catálogo
  cargado; si hay elegida, `TemplateInstantiator` (el mismo camino que **Aplicar a la selección**) y una `PreviewSession`
  virtual con título "Conectar · N4 · Nudo tipico Detalle D"; si no, `SpecStarter`. Abre `PreviewWindow` (modal, como hoy:
  es el cuadro del botón de la cinta). Con **una** barra seleccionada: `PickObject` con el mensaje "Pincha el cordón y las
  barras del nudo (Esc termina)" hasta que la persona pulse Esc, y después lo mismo. Con cero barras, cuadro corto. Si la
  selección da **más de un nudo con cordón** (`NodeDetector` sobre la selección), cuadro con "Esto parece una cercha (N
  nudos): usa Planificar lote" y un enlace que lo abre (decisión D1). Errores de `node_info` (`NODE_AXES_NOT_INTERSECTING`,
  `ELEMENT_NOT_A_MEMBER`…) en un cuadro con el `hint`, como el encargo. Log `ribbon_connect` con `selection`, `members`,
  `templates`, `proposal` (`template_id`, `orientation`, `max_deviation_deg`) o `starter: true`, duración.
- Desde la ventana del plan nada cambia: **Editar nudo** abre la misma `PreviewWindow` y por tanto estrena el panel sin
  código nuevo.

### 3.3 Panel por secciones (add-in, `UI/SpecPanel.xaml`; las listas y la escritura en el Core)

- **`Editing/PanelChoices.cs`** (Core): lee **`config/choices.json`** (`thickness_labels`, `bolt_diameter_labels` = `1/2"`,
  `5/8"`, `3/4"`, `7/8"`, `weld_sizes_mm`, `slot_length_mm`, `end_setback_mm`, `attachment_types`, `chord_interfaces`,
  `gusset_faces`, `roles`, con sus textos en español) y comprueba que cada diámetro de la lista existe en `limits.json`
  (`bolts.edge_distance_mm`; uno que no esté se descarta con una línea en el log); los demás diámetros de `limits.json` se
  siguen aceptando escribiéndolos en Avanzado, y el validador los trata como hoy; convierte cada rótulo a mm con `LabelParser.TryParseLabelToMm`
  (`3/8"` → 9,525; `PL10` → 10) y, al revés, elige la opción que corresponde a un valor del JSON (tolerancia 0,05 mm,
  `label_value_tolerance_mm`) o devuelve "Otro: 9,2 mm" sin cambiar el JSON. Sin el archivo, listas por defecto embebidas
  (nunca un panel vacío).
- **`SpecEditor.TrySetLabeledValue(rawJson, pathMm, pathLabel, option)`** (Core): escribe **a la vez** `*_mm` y `*_label`
  (espesor de la cartela, espesor de la placa cuchilla, diámetro del perno), así el desplegable nunca deja un
  `LABEL_VALUE_MISMATCH` a medias. **`SpecEditor.TrySetAttachmentType`**: al cambiar la unión de una barra de soldada en
  ranura a placa cuchilla atornillada (o al revés) crea el bloque que falta con los valores por defecto de `choices.json`
  y conserva el que ya estaba en el JSON por si la persona vuelve atrás (dentro de la sesión; no se guarda).
- **`UI/SpecPanel.xaml`** (add-in): un `UserControl` con un expansor por sección, construido a partir de `SpecEditor.ListFields`
  (la misma lista que hoy llena la tabla; las secciones ya existen: Cordón, Cartela, Barra N, Cadenas de cotas, Dudas). Cada
  campo pinta el control que le toca: **desplegable** para espesor, diámetro, soldadura, tipo de unión, unión al cordón, cara
  de la cartela y rol; **casilla** para "todo el contorno" y "continuo"; **número** (acepta coma o punto, como hoy) para el
  resto; **solo lectura** para los IDs y el perfil del modelo. Todo cambio pasa por `PreviewSession.TrySetField` o por los
  dos métodos nuevos: **una sola fuente de verdad, el JSON**, y el croquis y la validación se refrescan igual que hoy. Al
  entrar en un campo, su cota se resalta en naranja en el croquis (como al elegir una fila). La **pestaña Avanzado** lleva
  la tabla de hoy tal cual, el JSON (solo lectura, con **Copiar**), el cuadro del contorno con **Aplicar contorno**, las
  cadenas de cotas y las dudas. Los errores de validación se enseñan además **junto al campo** (borde rojo y el mensaje
  abajo) cuando el error trae `path`.
- **Cabecera del panel**: la plantilla propuesta con su miniatura (V4) y **Cambiar…** (lista de las candidatas que encajan,
  con miniatura, desvío y orientación; elegir otra vuelve a instanciar); si es un arranque, lo dice y ofrece **Encargo para
  IA** (abre lo mismo que el botón de la cinta sobre la misma selección).
- Los botones de abajo no cambian (Abrir del catálogo, Guardar en catálogo, Recargar, Guardar JSON, Validar, Crear, Cancelar);
  **Recargar** se desactiva en las sesiones virtuales (ya lo hace).

### 3.4 V4: miniaturas del catálogo (Core: el croquis de una plantilla; add-in: el PNG)

- **`Catalog/TemplateSketch.cs`** (Core): `TemplateSketch.Build(template, limits)` construye el croquis de una plantilla
  **sin modelo**: instancia `spec_template` con IDs ficticios (1, 2, 3…) y `SketchNodeInfo.FromSpecAngles` con los ángulos
  con signo y los perfiles del `member_pattern` (los anchos, con `SketchNodeInfo.DefaultMemberWidthMm` si el perfil no se
  reconoce), y devuelve el mismo `Sketch` que pinta `SketchCanvas`. Se prueba en la nube (piezas, cotas, límites).
- **`Catalog/TemplateThumbnails.cs`** (add-in): `EnsurePng(template, folder)` pinta ese croquis en un `SketchCanvas` fuera
  de pantalla (sin cotas ni etiquetas: solo cordón, barras, cartela, placas y pernos, coloreado como el croquis) y lo guarda
  con `RenderTargetBitmap` + `PngBitmapEncoder` como `<template_id>.png` (240×180) **junto al archivo de la plantilla**; lo
  rehace si falta o si el `.json` es más nuevo. Un fallo al dibujar deja la miniatura gris con el nombre y una línea en el
  log (`thumbnail_failed`): nunca impide abrir el catálogo. Se generan al abrir la ventana del catálogo (en el hilo de la
  ventana) y al guardar una plantilla.
- **Dónde se ven**: la **ventana del catálogo** (columna de miniatura delante del nombre y una vista grande de la plantilla
  elegida con su `DescribePattern()` a la derecha), **Abrir del catálogo** de la previsualización (`ChooseDialog` con
  miniatura por fila), **Cambiar…** del panel (3.3), **Plantilla…** de la ventana del plan (solo las que entraron en el plan,
  como hoy) y el **detalle del nudo de la ventana del plan** (la miniatura de la plantilla que le casó, junto al texto de
  hoy; decisión D7). En el **mapa de la cercha, no** (los círculos siguen como están). `conn_catalog_list` **no cambia** (la IA no ve imágenes); si hace falta, `thumbnail_path` se añade más adelante.
- `deploy.ps1` copia también los `.png` de `catalog\` que falten (hoy solo `*.json`); **Guardar en catálogo** con "Copiar
  también a la carpeta compartida" copia el PNG con el JSON.

### 3.5 V5: el plano al lado del croquis (add-in, `UI/DrawingPane.xaml`)

- En la previsualización, botón **Plano…** con tres entradas: **Abrir archivo…** (PNG, JPG), **Pegar del portapapeles**
  (`Clipboard.ContainsImage`: el recorte de un PDF o del plano con Win+Mayús+S; se guarda como
  `%LOCALAPPDATA%\MotorConexiones\planos\plano-<documento>-<fecha-hora>.png`) y **Quitar**. La imagen se abre en un panel a
  la **izquierda** del croquis con un divisor (`GridSplitter`), con rueda = zoom, arrastrar = encuadre y **Ajustar**, como el
  croquis. Un botón **Ocultar plano** lo pliega sin olvidar la imagen.
- **Se recuerda**: la ruta va en `source.drawing_image` (clave **nueva y opcional** del contrato: esquema, `AllowedSourceProperties`
  del validador, `SpecEditor`; la IA puede ignorarla); al guardar en catálogo pasa a `origin.drawing_image` de la plantilla
  y, al aplicar la plantilla (Conectar, Abrir del catálogo, Aplicar a la selección), el plano se abre solo si el archivo
  existe (si no, aviso en la barra de estado, nunca un error). `conn_catalog_apply` copia la clave a `source.drawing_image`.
- PDF directo **no** (decisión D8): haría falta una librería de terceros o la API de Windows (`Windows.Data.Pdf`) y una
  dependencia nueva en el add-in no se mete sin probarla en el PC. El pegado del portapapeles cubre el caso.

### 3.6 Los tres retoques de paso (`fase-10.md` 8.2, puntos 1 a 3)

1. **`SelectionExpansion` desde la cinta**: `BatchPlanCommand` pasa la expansión ya calculada a `BatchPlanner.Plan` (parámetro
   nuevo `presetExpansion`) para que `plan.SelectionExpansion` se rellene igual que por el puente; la línea del plan dice
   "(63 añadidas por la selección asistida)" y `selection_expansion` viaja en `batch_plan_get`. Más… > Completar selección,
   lo mismo.
2. **Log `label_clicked`**: `changed` (lo que hoy se llama `highlighted`: si el clic cambió algo) y `highlighted` con el estado
   real de la etiqueta después del clic (siempre `true` tras un clic sobre una etiqueta del plan). El sondeo 21 no cambia.
3. **Títulos de los `TaskDialog`** (29 usos en el add-in): Revit antepone el nombre del add-in, así que los títulos quedan
   **sin** "MotorConexiones - ": "Planificar lote", "Conectar", "Encargo para IA", "Catálogo", "Conexiones del modelo",
   "Error". Una constante por botón en `RibbonCreation` para no repetirlos.

### 3.7 Herramientas, rutas, configuración y guía

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_catalog_apply` | **Solo si cabe (D9)**: `template_id` opcional; sin él, `TemplateChooser` elige y `data.proposal` lista las candidatas (`template_id`, `name`, `orientation`, `max_deviation_deg`) y `data.proposal.chosen`. Con `template_id`, igual que hoy. |
| MCP | las demás | Sin cambios. 25 rutas, 23 herramientas. Versión **0.11.0** en adaptador, herramientas, simulador y contrato (como en todas las fases). |
| Contrato | `source.drawing_image` | Clave nueva, opcional, cadena o `null`. El esquema, el validador y `docs/guide.md` (sección 3) la nombran; la IA no tiene que escribirla. |
| Configuración | **`config/choices.json`** (nuevo) | Listas de los desplegables y valores por defecto del arranque; `deploy.ps1` lo copia a `config\` del add-in; README sección 6 lo explica. |
| Cinta | **Conectar** (nuevo, primero del panel); los demás botones igual | Sección 3.2. Textos sin el prefijo (3.6). |
| Ventanas | Previsualización con panel, Avanzado, Plano…; Catálogo con miniaturas; `ChooseDialog` con miniatura | Secciones 3.3 a 3.5. |
| Guía | `docs/guide.md`, secciones 3 y 5 | `source.drawing_image`; `conn_catalog_apply` sin `template_id` (si entra). |
| Instalador | `deploy.ps1` | Copia `config\choices.json` y los `.png` de `catalog\`. |

### 3.8 Arquitectura (solo lo de esta fase)

```
src/MotorConexiones.Core/
├── Catalog/TemplateChooser.cs      Propose: casa todas las plantillas (MatchAll), ordena, elige, SummaryText; lo usa PlanBuilder
├── Catalog/TemplateSketch.cs       el croquis de una plantilla sin modelo (FromSpecAngles con el member_pattern)
├── Catalog/CatalogTemplate.cs      origin.drawing_image
├── Editing/SpecStarter.cs          la especificación de arranque (cartela rectangular, soldada en ranura, dudas)
├── Editing/PanelChoices.cs         choices.json + diámetros de limits.json; rótulo ↔ mm; "Otro: …"
├── Editing/SpecEditor.cs           TrySetLabeledValue, TrySetAttachmentType; source.drawing_image en ListFields
├── Contract/SourceInfo.cs          DrawingImage
└── Schema/JsonSchemaValidator.cs   source.drawing_image (opcional)

src/MotorConexiones.Revit/
├── ConnectCommand.cs               botón Conectar: selección → node_info → Propose → PreviewSession (plantilla o arranque)
├── Catalog/TemplateThumbnails.cs   PNG junto a la plantilla (RenderTargetBitmap del SketchCanvas sin cotas)
├── Services/RibbonCreation.cs      títulos de los cuadros sin prefijo; botón Conectar
├── Batch/BatchPlanner.cs           presetExpansion (3.6.1)
├── Batch/PlanLabels.cs             changed / highlighted en label_clicked (3.6.2)
├── BatchPlanCommand.cs             pasa la expansión al plan; títulos
├── UI/SpecPanel.xaml(.cs)          el panel por secciones (expansores, desplegables, errores junto al campo, cabecera con plantilla)
├── UI/DrawingPane.xaml(.cs)        el plano al lado (archivo, portapapeles, zoom, encuadre, ocultar)
├── UI/PreviewWindow.xaml(.cs)      panel + Avanzado + Plano…; divisor del plano
├── UI/PreviewSession.cs            DrawingImagePath; SetLabeledValue; SetAttachmentType
├── UI/CatalogWindow.xaml(.cs)      miniaturas y vista grande
└── UI/ChooseDialog.xaml(.cs)       miniatura por fila (opcional por constructor)

config/choices.json                 listas de los desplegables y valores por defecto del arranque (nuevo)
scripts/deploy.ps1                  copia choices.json y los .png de catalog\
mcp/revit_mcp/conexiones.py         0.11.0 (25 rutas; catalog/apply con template_id opcional si entra)
mcp/tools/conn_tools.py             0.11.0 (23 herramientas; manual de conn_catalog_apply si entra)
mcp/pruebas/simulador_revit.py      proposal en catalog/apply (si entra); drawing_image aceptado
src/MotorConexiones.Tests/          TemplateChooserTests, SpecStarterTests, PanelChoicesTests, TemplateSketchTests,
                                    SpecEditorTests (nuevos casos), SpecValidationTests (drawing_image)
```

## 4. Reglas técnicas de esta fase

- **Ninguna ventana en las rutas `conn_*`**. Los cuadros nuevos son los del botón Conectar (selección insuficiente, parece una
  cercha, errores de `node_info`) y la previsualización, que ya era el cuadro del botón de la cinta. El manejador de clics
  de las etiquetas sigue sin abrir nada.
- **El JSON sigue siendo la única fuente de verdad** de la previsualización: el panel escribe por `SpecEditor` y lee de
  `ListFields`; nada del panel guarda estado propio salvo el bloque de unión que se conserva al cambiar de tipo (en memoria).
- **Un desplegable escribe los dos campos** (`*_mm` y `*_label`) en la misma edición: nunca deja el JSON a medias.
- **Crear no cambia**: la misma `ConnectionCreationService`, un `TransactionGroup`, rollback completo, token de `Validate`
  recién calculado. Conectar **no crea nada** al pulsarse: solo abre la ventana.
- **El contrato solo añade** (`source.drawing_image`, `origin.drawing_image`, `template_id` opcional y `data.proposal` si entra).
  El validador acepta `drawing_image` y no exige que el archivo exista (es del PC de la persona).
- **Las miniaturas nunca bloquean**: un fallo al pintar deja el hueco gris y una línea en el log; el PNG se escribe junto a la
  plantilla con el mismo nombre y nunca pisa un `.json`.
- La especificación de arranque **no es la cartela automática**: una cartela rectangular sencilla, marcada como arranque, que
  pasa por `conn_validate` como cualquier otra; `outline.mode` sigue siendo `polygon`.
- `ElementId.Value`; miembros de la API comprobados compilando contra 2027 (`PickObject` y `Clipboard` ya se usan en el add-in;
  `RenderTargetBitmap` y `PngBitmapEncoder` son de WPF, no de Revit). Unidades solo en `Units/UnitConverter.cs`; los rótulos en pulgadas los
  convierte `LabelParser`, que ya existe. Código y claves JSON en inglés; textos en español.
- Versión **0.11.0** en `AddinInfo`, los dos csproj, adaptador, herramientas, simulador y contrato.

## 5. Pruebas en la nube

- `TemplateChooserTests`: con el nudo del Detalle D (fixture) y la plantilla del Detalle D, la propuesta la elige en `same`
  con desvío 0; el nudo en espejo (ángulos reflejados) la elige en `mirror_x`; con dos plantillas que encajan, gana la de
  menor desvío y la otra queda como candidata; con una plantilla de 2 barras y un nudo de 3, ninguna encaja y `SummaryText`
  dice por qué (los intentos); catálogo vacío → sin candidatas. `PlanBuilder` sigue dando los mismos 16 listos de la cercha
  de la 8b (las pruebas de `BatchPlanTests` no cambian: la extracción no altera el resultado).
- `SpecStarterTests`: desde el `node_info` del Detalle D (3 barras), una especificación que pasa el esquema, con los tres
  IDs, los perfiles del modelo, unión soldada en ranura, cartela rectangular que contiene los extremos recortados de las
  tres barras (comprobación geométrica con `Polygon2D`), `uncertain_fields` con la unión al cordón confirmada por defecto y
  `source.drawing` de arranque; con una plantilla reciente del mismo cordón, toma de ella el espesor, la soldadura, la ranura, el retiro y la unión al
  cordón (y con dos del mismo cordón, la más reciente); sin ninguna, los de `choices.json`;
  `SpecValidator` sobre el arranque no da errores de esquema ni de rótulo (los de geometría dependen del modelo: con los
  `Fakes` del fixture, se comprueba que valida o que solo avisa).
- `PanelChoicesTests`: lee `config/choices.json` del repositorio (los rótulos convierten todos a mm sin error), los cuatro
  diámetros por defecto existen en `limits.json` (`1/2"` → 12,7, `5/8"` → 15,875, `3/4"` → 19,05, `7/8"` → 22,225) y un
  diámetro de `limits.json` que no está en la lista (25,4) se enseña como "Otro: 25,4 mm" sin perderse; 9,525 → `3/8"`,
  9,2 → "Otro: 9,2 mm"; sin archivo → listas embebidas no vacías.
- `SpecEditorTests` (casos nuevos): `TrySetLabeledValue` cambia `gusset.thickness_mm` y `thickness_label` a la vez (12,7 y
  `1/2"`) y la validación no da `LABEL_VALUE_MISMATCH`; `TrySetAttachmentType` de `welded_slot` a `bolted_knife_plate` crea
  `plate` y `bolts` con los valores por defecto y vuelve a `welded_slot` con la ranura de antes; `ListFields` enseña
  `source.drawing_image`.
- `TemplateSketchTests`: el croquis de la plantilla del Detalle D sin modelo tiene cordón, 3 barras, cartela, placa y 4 pernos,
  límites no vacíos, y los ángulos de las barras son los del `member_pattern`.
- `SpecValidationTests`: `source.drawing_image` con ruta, con `null` y ausente pasan el esquema; una clave desconocida en
  `source` sigue fallando (`SCHEMA_INVALID`). `CatalogTests`: `origin.drawing_image` sobrevive a `ToJson`/`FromJson` y a guardar
  y aplicar.
- `simulador_revit.py --autocomprobar` (si entra D9: `catalog/apply` sin `template_id` devuelve `proposal`; el `drawing_image`
  pasa por `validate` y `catalog/save`); `probar_conexiones.py` con una prueba nueva (`catalog_apply` sin `template_id` sobre el
  fixture → la plantilla de la prueba 19, `proposal.chosen`) si entra D9; `py_compile` de `mcp/` y `scripts/sondeos/`.
- `dotnet build` sin avisos, `dotnet test` en verde (217 más las nuevas).

**NO PROBADO en la nube, por qué**: todo lo de WPF y Revit: el panel (controles, errores junto al campo), los PNG de las
miniaturas (`RenderTargetBitmap` solo con un hilo de WPF y las fuentes del PC), el plano al lado y el pegado del portapapeles,
el botón Conectar de punta a punta (`PickObject`, `TaskDialog`), los títulos de los cuadros y los dos retoques del lote en
Revit. Se prueban en el PC (sección 6).

## 6. Instrucciones para el instalador (`docs/instalacion/fase-11.md`)

Sobre la **copia** `HANGAR_PRUEBA_sondeo.rvt` como **único** proyecto abierto, con `Anota`, el servidor del puerto 8000 arrancado
antes del `--puente` (`Get-NetTCPConnection -LocalPort 8000`; si no, `C:\IA\iniciar_servidor_revit.bat`) y resultados en
`docs/fases/resultados-fase-11.md`. **Crea acero de verdad** (dos conexiones de un nudo) y lo borra al final. Unos 75 minutos.

1. **(instalador, Revit cerrado)** `git pull`, build sin avisos, test, `deploy.ps1` (`0.11.0.0`; copia `config\choices.json` y
   dice cuántos `.png` de `catalog\` copió), `instalar-conn.ps1` (25 rutas, 23 herramientas).
2. **(instalador)** Abrir Revit (la persona): `ping` 0.11.0 con `HANGAR_PRUEBA_sondeo`; `catalog_list` con 1 plantilla; sondeos
   12, 13, 17 y 21 a cero. **No** hay plan marcado.
3. **(la persona) Conectar con plantilla**: seleccionar las cuatro barras del Detalle D (1249510, 1249630, 1249631, 1249636) y
   pulsar **Conectar**: la ventana se abre sin pedir archivo, con "Plantilla propuesta: Nudo tipico Detalle D · igual …", el
   croquis y el **panel por secciones**. Captura `fase11-01-conectar`. En el panel: cambiar el espesor de la cartela a `1/2"`
   (el croquis dice `PL 1/2" · 12,7 mm`, sin `LABEL_VALUE_MISMATCH`), el diámetro de los pernos a `3/4"` (anotar si la
   validación marca `BOLT_EDGE_DISTANCE_TOO_SMALL` con el borde de 40 mm: el mínimo de `3/4"` es 25, así que **no** debe) y
   volver a `3/8"` y `5/8"`; cambiar la unión de la Barra 1 a placa cuchilla atornillada (aparecen placa y pernos con los
   valores por defecto) y volver a soldada en ranura (vuelve la ranura de 150). Pestaña **Avanzado**: la tabla de siempre y
   el JSON con los cambios. **Crear** → `connection_id`; sondeo 12 con 1. Captura `fase11-02-panel`.
4. **(la persona) Plano al lado**: **Conectar** sobre otro nudo igual de la cercha (el siguiente del cordón central, con sus
   3 barras; el Detalle D ya tiene conexión y daría `already_connected`), **Plano… > Abrir archivo…** con
   `docs\fixtures\detalle-D.png`: la imagen a la izquierda del croquis, zoom
   y encuadre; **Ocultar plano** y volver; **Pegar del portapapeles** tras un recorte con Win+Mayús+S de cualquier cosa (se
   guarda en `planos\`). Captura `fase11-03-plano`. Cancelar sin crear.
5. **(la persona) Conectar sin plantilla (arranque)**: seleccionar un nudo del cordón **superior** con su cordón (cordón
   superior y las dos diagonales que llegan: 2 barras, ninguna plantilla de 2 barras en el catálogo) y **Conectar**: la cabecera
   dice que ninguna encaja y que es una especificación de arranque; el croquis enseña la cartela rectangular y las dos barras
   soldadas en ranura; la validación en verde o con avisos (anotar los códigos). Captura `fase11-04-arranque`. **Crear** si
   está en verde → `connection_id`; sondeo 12 con 2. Si no valida, anotar los errores literales y Cancelar (es lo que esta
   fase viene a averiguar; no es un fallo de la instalación).
6. **(la persona) Miniaturas**: botón **Catálogo**: la fila del Detalle D con su miniatura y la vista grande a la derecha; en
   `%LOCALAPPDATA%\MotorConexiones\catalogo\` hay un `.png` junto al `.json` (`dir`). **Guardar en catálogo** desde la ventana
   del paso 5 (si validó) con el nombre `PRUEBA fase 11` → aparece con miniatura; **Eliminar** después. Captura `fase11-05-catalogo`.
7. **(la persona) Los retoques de paso**: pinchar **una** diagonal y **Planificar lote** → Planificar con las 64 → la línea del
   plan dice "(63 añadidas por la selección asistida)"; pinchar la etiqueta 4 dos veces → en el log `label_clicked` con
   `changed: true` y después `changed: false`, `highlighted: true` las dos; el detalle de N4 enseña la **miniatura** del
   Detalle D (y el mapa sigue igual, sin miniaturas); los títulos de los cuadros de esta sesión
   ("Planificar lote", "Conectar", "Catálogo") sin "MotorConexiones - MotorConexiones". **Descartar plan** → sondeos 17 y 21 a cero.
   Captura `fase11-06-titulo`.
8. **(instalador)** Por el puente: `catalog_apply` **sin** `template_id` con las barras del Detalle D (si entró D9: `proposal`
   con la elegida; si no, el error de siempre por falta de `template_id`, anotado); `validate` con `source.drawing_image`
   (`ok: true`); `list` con las 2 conexiones; `delete` de las dos (barras restauradas); sondeos 12, 13, 17 y 21 a cero;
   `probar_conexiones.py --puente` entero (32/32, o 34/34 si entra la prueba nueva y su pareja del puente); el log del día
   sin `ribbon_connect_failed`, `thumbnail_failed`, `drawing_pane_failed`, `label_click_failed`, `plan_event_failed` ni
   `ribbon_batch_window_error`.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; `simulador_revit.py --autocomprobar`;
  `probar_conexiones.py` contra el simulador; `py_compile` de todo `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-11.md` según el Anexo B del encargo, con la lista de lo NO PROBADO (todo lo de WPF y Revit: sección 5).
- `docs/instalacion/fase-11.md` literal, con `Anota`, capturas con nombre fijo, el servidor del puerto 8000 antes del `--puente`
  y qué devolver.
- README (estado, tabla de garantías, árbol, instalación con `choices.json`, sección 6 con el archivo nuevo, sección 9 con
  Conectar, el panel y el plano, sección 11 con las miniaturas, errores), `mcp/CONTRATO-conn.md` (`source.drawing_image`;
  `template_id` opcional si entra), `docs/guide.md` (secciones 3 y 5), `CLAUDE.md` (estructura: `TemplateChooser`,
  `SpecStarter`, `PanelChoices`, `ConnectCommand`, `SpecPanel`, `DrawingPane`, `choices.json`),
  `docs/propuestas/flujo-intuitivo.md` (botón Conectar, V4 y V5 hechos; la fila 11 del calendario) y
  `docs/propuestas/encargo-ia-externa.md` (el encargo desde el panel) al día.
- `docs/fases/resumen-fase-11.md` con el resumen del chat.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta fase

- **Cartela automática** (`outline.mode = "auto"`): Fase 12. El arranque de Conectar es una cartela rectangular sencilla.
- **Edición arrastrando** en el croquis (V6): Fase 12 o posterior. Vista 3D en la ventana (V7): no se recomienda.
- **PDF directo** en el plano al lado: el pegado del portapapeles lo cubre; si hace falta, propuesta aparte con la librería
  elegida y su sondeo.
- **Fusionar Conectar y Planificar lote en un solo botón** (si D1 se responde "dos"): se decide cuando los dos lleven un tiempo
  en uso.
- `conn_design_brief` (P4 del encargo), `thumbnail_path` en `conn_catalog_list`, `conn_batch_update`, el traspaso de `mcp/` a
  `revit-mcp`, la placa de extremo (`docs/propuestas/placa-de-extremo.md`), el fantasma de la placa cuchilla (P2 de la 10),
  `stop_on_error` con 16 nudos y el plan B del deshacer.

## 9. Preguntas para la persona y sus respuestas (chat del 2026-10-12)

Las preguntas se dejan tal como se hicieron; la respuesta va al final de cada una en **negrita** y es lo que fija la sección 2.

- **D1. Un botón o dos** (la P6 de la propuesta, aplazada desde la 8c). **a)** Botón nuevo **Conectar** para un nudo y
  **Planificar lote** se queda para la cercha; si seleccionas una cercha y pulsas Conectar, te manda a Planificar lote.
  **b)** Un solo botón **Conectar** que decide por la selección (un nudo → ventana del nudo; varios → plan de lote) y
  Planificar lote desaparece de la cinta. Recomiendo **a**: Planificar lote está probado y la gente ya sabe dónde está; fusionar
  es otra fase si hace falta. **Respuesta: a.**
- **D2. Si ninguna plantilla encaja.** **a)** Conectar abre igual la ventana con una **especificación de arranque** (cartela
  rectangular que envuelve las barras, uniones soldadas, valores por defecto) para rellenar el panel. **b)** Conectar solo
  avisa y ofrece Encargo para IA o Abrir del catálogo; sin plantilla no abre el panel. Recomiendo **a**: es lo que hace útil el
  panel para un nudo nuevo; la cartela de verdad calculada por ángulos sigue siendo la Fase 12. **Respuesta: a.**
- **D3. Cómo se elige el nudo.** **a)** Seleccionas el cordón y las barras (como hoy en Catálogo y Encargo para IA); con una
  barra sola, Conectar te pide pinchar las demás en Revit. **b)** Pinchas una barra y Conectar busca solas las que la tocan en
  ese extremo (habría que decidir qué extremo: el más cercano a donde pinchaste). Recomiendo **a**: sin ambigüedad y sin
  arrastrar la cercha entera. **Respuesta: a.**
- **D4. El panel y la tabla.** **a)** El panel por secciones **sustituye** a la tabla; la tabla, el JSON, el contorno punto a
  punto, las cadenas de cotas y las dudas pasan a una pestaña **Avanzado**. **b)** Panel y tabla a la vez (dos pestañas,
  "Panel" y "Tabla"). Recomiendo **a**. **Respuesta: a.**
- **D5. Las listas de los desplegables.** Espesores propuestos: `1/4"`, `5/16"`, `3/8"`, `1/2"`, `5/8"`, `3/4"`, `1"` y `PL6`,
  `PL8`, `PL10`, `PL12`, `PL16`, `PL20`; diámetros de perno: los de `limits.json` (`1/2"` a `1-1/4"`); soldaduras: 3, 4, 5, 6,
  8 y 10 mm. ¿Falta o sobra alguno? ¿Los espesores van en pulgadas, en mm o las dos listas como propongo? ¿Te vale un archivo
  nuevo `config/choices.json` (editable sin recompilar, como `limits.json`)? **Respuesta: las dos listas de espesores tal
  como están; en pernos solo `1/2"`, `5/8"`, `3/4"` y `7/8"` por defecto (los demás de `limits.json` siguen aceptados en
  Avanzado); soldaduras 3, 4, 5, 6, 8 y 10 mm; sí a `config/choices.json`.**
- **D6. Palabras** (la P7 de la propuesta, nunca respondida). El panel va a decir **cartela**, **retiro**, **placa cuchilla**,
  **soldada en ranura**, **atornillada**, **en espejo**, **cordón**, **barra**. Dime las que quieras cambiar (por ejemplo
  "plancha de nudo", "corte", "cuchilla", "lado derecho"). **Respuesta: valen todas las propuestas.**
- **D7. Miniaturas (V4).** **a)** Las dibuja el add-in a partir del croquis y guarda un PNG **junto a cada plantilla**
  (`<template_id>.png` en la carpeta del catálogo; `deploy.ps1` copia también los de `catalog\`). **b)** Sin archivo: se dibujan
  en memoria cada vez que se abre el catálogo. Recomiendo **a** (abre rápido y la plantilla compartida lleva su imagen).
  ¿Las quieres también en el **mapa de la cercha** o en la tabla del plan (en el detalle del nudo, la miniatura de su
  plantilla)? Hoy no está previsto; es barato. **Respuesta: a; sí en el detalle del nudo de la ventana del plan, en el mapa
  no.**
- **D8. El plano al lado (V5).** ¿Tienes los detalles como **PNG o JPG**, o solo en **PDF**? Propongo: Abrir archivo (PNG, JPG)
  y **Pegar del portapapeles** (recorte con Win+Mayús+S, también de un PDF abierto). PDF directo queda fuera (librería nueva).
  ¿El plano a la **izquierda** del croquis con divisor (**a**) o en una **pestaña** que alterna con el croquis (**b**)?
  ¿Quieres que la ruta de la imagen se guarde en la especificación y en la plantilla para que se abra sola la próxima vez
  (**sí**, recomendado) o prefieres abrirla a mano cada vez? **Respuesta: PNG y JPG (los detalles están como imagen, igual
  que `detalle-D.png`) y Pegar del portapapeles para los PDF; plano a la izquierda con divisor (a); sí, guardar la ruta en la
  especificación y en la plantilla.**
- **D9. Para la IA.** ¿`conn_catalog_apply` sin `template_id` debe elegir sola la mejor plantilla (**sí**, si cabe en la sesión)
  o lo dejamos como está (la IA ya puede listar y elegir)? **Respuesta: sí, solo si cabe sin recortar nada del panel.**
- **D10. Valores por defecto del arranque** (si D2 es a). Propongo: cartela `3/8"`, soldaduras 5 mm todo el contorno, ranura
  150 mm, retiro 60 mm, unión al cordón `through_slot` como duda confirmada; o, si hay una plantilla con el mismo perfil de
  cordón, sus valores. ¿Te vale? **Respuesta: sí, con la excepción como regla: si hay una plantilla con el mismo perfil de
  cordón, se usan sus valores.**

## 10. Riesgos

- **El panel es la ventana más grande del add-in** (unos 35 campos por nudo, tres tipos de control, errores junto al campo).
  El riesgo es que no quepa entero en la sesión con V4 y V5: el orden de corte, si hace falta, es **D9 primero** (`template_id`
  opcional por el puente), después **V5** (plano al lado), después **V4** (miniaturas) y nunca el botón Conectar ni el panel,
  que son la fase. Lo que se corte se anota como
  pendiente con lo que falte, como hizo la Fase 10 con la 9b.
- **`RenderTargetBitmap` fuera de pantalla**: pinta solo con el hilo de WPF y tras `Measure`/`Arrange`; en Revit a veces
  devuelve blanco si el control no está en un árbol visual. Se pinta dentro de un `Viewbox` sin añadirlo a la ventana y, si el
  PNG sale vacío (todos los píxeles blancos), se deja el hueco gris y se anota (`thumbnail_failed`). En la nube no se puede ver.
- **La especificación de arranque puede no validar** en nudos con ángulos agudos (la cartela rectangular choca con una barra
  ajena o la ranura se sale): es esperable y el panel lo enseña; no se intenta resolver en esta fase (Fase 12).
- **`PickObject` desde un botón de la cinta** con la ventana cerrada funciona (lo usa Cordón… con la ventana oculta desde la
  8e); aquí no hay ventana abierta todavía, así que es el caso sencillo.
- **El pegado del portapapeles** depende de que haya una imagen (`Clipboard.ContainsImage`); si hay texto, se dice y no pasa nada.
- **Las claves nuevas del contrato** (`drawing_image`) tienen que entrar en el esquema, en `AllowedSourceProperties`, en el
  simulador y en el contrato escrito: si falta una, `SCHEMA_INVALID` al validar una especificación con plano. La prueba de
  `SpecValidationTests` lo cubre.
