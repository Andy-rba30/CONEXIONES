# CLAUDE.md — MotorConexiones

Reglas permanentes de este repositorio. El encargo completo está en `docs/ENCARGO_MOTOR_CONEXIONES.md`
y se ejecuta **una fase por sesión** (sección 13 del encargo). Léelo entero antes de trabajar.

## Qué es esto

Add-in de Revit en C# (`MotorConexiones`) que crea conexiones de acero (cartelas, placas, pernos,
soldaduras, cortes) a partir de una especificación JSON. Lo maneja una IA a través del servidor MCP
`revit-mcp` (repositorio aparte, Python + pyRevit), con herramientas nuevas de prefijo `conn_`.

## Cómo trabajar

- El usuario es principiante: explica en español sencillo, con archivo, botón y comando exactos.
- Ejecuta solo la fase pedida. Al terminar: compila, pruebas en verde, commit, `docs/fases/fase-N.md`
  (Anexo B del encargo) y un resumen corto en el chat.
- Nunca informes como probado lo que no ejecutaste. Marca "NO PROBADO" y explica por qué.
- La sesión corre en la nube: **no hay Revit ni MCP**. Lo que necesite Revit se escribe como sondeo
  en `scripts/sondeos/` con instrucciones en `docs/instalacion/fase-N.md`; un agente instalador lo
  ejecuta en el PC del usuario y la salida vuelve en `docs/fases/resultados-fase-N.md` o en el chat.
- No inventes miembros de la API de Revit: compílalos contra la API 2027 o pruébalos con un sondeo.
- El repositorio `revit-mcp` es de solo lectura. Los archivos nuevos del MCP van en `mcp/` de este
  repositorio y se instalan con `mcp/instalar-conn.ps1`. No se modifican herramientas existentes.
- Código, clases, archivos de código y claves JSON en inglés. Mensajes, errores, comentarios
  importantes, informes y documentación en español.
- Commits claros en español. Sin pull requests. Solo la rama de trabajo indicada.

## Comandos

```powershell
dotnet build MotorConexiones.sln -c Release   # nube y PC
dotnet test                                   # nube y PC
.\scripts\deploy.ps1                            # solo PC (instalador)
.\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py   # solo PC: IronPython dentro de Revit
```

## Estructura (ver sección 4 del encargo)

- `src/MotorConexiones.Core` — netstandard2.0, sin Revit: contrato, esquema, unidades, validación, catálogo (`Catalog/`)
  y detección de nudos y plan de lote (`Batch/`, Fase 8) con sus textos en español y el alzado de la cercha
  (`PlanAdvice`, `TrussMap`, ronda 8c); crear por lotes (Fase 9: `BatchCreateRequest`, `BatchReport`, `BatchRunner` =
  qué nudos se crean y en qué orden, con el creador como delegado; estados `created` y `failed`; botones de "Qué hacer");
  Fase 10: `SelectionAssist` (selección asistida: barras que se tocan, plano por votación) y `LabelImage` (el BMP de 24 bits
  de la etiqueta del lienzo); `Brief/DesignBriefWriter` (ronda 9b: el encargo para una IA externa en un solo Markdown, con
  el Detalle D confirmado embebido como recurso); Fase 11: `Catalog/TemplateChooser` (casa todas las plantillas y propone la
  mejor; lo usa también `PlanBuilder`), `Catalog/TemplateSketch` (el croquis de una plantilla sin modelo, para las miniaturas),
  `Editing/PanelChoices` (las listas de `config/choices.json`, rótulo ↔ mm, "Otro: 9,2 mm"), `Editing/SpecStarter` (la
  especificación de arranque con cartela rectangular cuando ninguna plantilla encaja) y `SpecEditor.TrySetLabeledValue` /
  `TrySetAttachmentType`; el contrato suma `source.drawing_image` (y `origin.drawing_image` en la plantilla). Fase 12:
  `Types/DirectWeldType` y `Types/EndPlateType` (los dos tipos nuevos, con `Validation/DirectWeldValidator` y
  `EndPlateValidator`, `Sketch/DirectWeldSketch` y `EndPlateSketch`, `Contract/EndPlateSpec`), `Geometry3D/SpecFrame` (la
  única regla del marco: colineal para un empalme, canónico a mitad entre los cortes para un nudo), `NodeReach` con medio
  canto (`MEMBER_ECCENTRIC`), `BoltRing` (pernos por cara), `BoltStack.ForTwoPlates`, `DirectWeldGeometry` (corte contra la
  cara), `Geometry2D/Octagon`; en `Batch/`, la clase `splice` (`NodeKind`), dos nudos en un punto (N9 y N9E), los arranques
  por tipo en `PlanBuilder` y `BatchRunner.OrderForCreation` (el empalme antes que su nudo K); `SpecStarter.ForKNode` /
  `ForSplice`; las claves `node.eccentricity_mm` y `members[].axis_offset_mm`, opcionales en los tres tipos. Ronda 12c:
  `Model/SectionOrientation` (`HssDesignation` = la designación AISC en mm, canto × ancho × pared; `SectionOrientation` = qué lado
  queda en el plano de la cercha según Cross-Section Rotation) y la convención escrita en `MemberModelFacts`: **`WidthMm` es el
  lado en el plano de la cercha y `HeightMm` el perpendicular** (el HSS12X8X1/2 del Hangar girado 90°: 203,2 × 304,8);
  `Geometry3D/WeldChain` (segmentos de soldadura → polilíneas para `WeldLine`). Fase 13: `Geometry2D/AutoOutline` (la cartela
  automática, `outline.mode = "auto"`: rectángulos por barra + franja del cordón → envolvente convexa redondeada a 5 mm, 12 vértices;
  `Resolve` / `ResolvePoints` son lo que valida, dibuja y fabrica) y `Polygon2D.ConvexHull` / `Simplify`; el contrato suma
  `gusset.outline.mode` y los márgenes opcionales (`JsonIgnore` con `null`: el camino `polygon` da los mismos tokens);
  `SpecValidator` resuelve `auto` antes de la regla 9 (`OUTLINE_AUTO_LARGE`); `SpecEditor.TrySetOutlineMode` /
  `ConvertAutoToPolygon`; `SpecStarter` arranca con `auto`; `TemplateBuilder` con `auto_angle_tolerance_deg`; `Geometry3D/SplitPoint`
  (el corte del Detalle B: cara a (ancho/2)/|sen θ|, +330, +espesor+gap/2), `Storage/SplitRecord`, `Batch/SplitPlanning` (pasos
  `split` y nudos `N…B` previstos, estado `after_split`), `BatchRunner.StepsFor` (cortes → empalmes B → N9E → N9) y
  `BatchStepHandlers` (`Split`, `CreateSplice`, `Revalidate`: el token del nudo en K se renueva tras el corte).
- `src/MotorConexiones.Revit` — add-in: cinta, `Bridge.Handle`, nudo, fabricación, almacenamiento, catálogo, plan y lote
  (`Batch/`: marcas en el modelo, planes en memoria, `PlanEvents` = el `ExternalEvent` de la ventana no modal del plan,
  `BatchCreator` = grupo exterior del lote + un `OperationScope` por nudo, y `DeleteBatch`; Fase 10: `PlanLabels` = las
  etiquetas pinchables del lienzo (`TemporaryGraphicsManager`, fuera de las transacciones) y `PlanLabelHandler` = el
  manejador de clics que **nunca abre un cuadro**; `PlanMarks` dibuja además las cartelas fantasma; `UI/`: ventanas WPF de
  los botones, con el mapa de la cercha `TrussMapCanvas`, los botones Crear N conexiones y Borrar el lote, Completar
  selección y `OnLabelClicked`; `DesignBriefCommand` = botón Encargo para IA). Fase 11: `ConnectCommand` = botón **Conectar**
  (selección → nudo → `TemplateChooser` → previsualización con la mejor plantilla o el arranque; con una barra pide pinchar;
  con una cercha manda a Planificar lote); `UI/SpecPanel` = el panel por secciones (Cartela, Cordón, Barra N) con desplegables
  que escriben los mm y el rótulo a la vez y errores junto al campo; `UI/DrawingPane` = el plano al lado del croquis (PNG/JPG o
  pegado del portapapeles, `%LOCALAPPDATA%\MotorConexiones\planos\`); `Catalog/TemplateThumbnails` = el PNG de 240×180 junto a
  cada plantilla (catálogo, Abrir del catálogo, Cambiar…, detalle del nudo del plan); `PanelChoicesLoader`; títulos de los
  `TaskDialog` sin prefijo en `RibbonCreation`.
  Fase 12: `Fabrication/DirectWeldBuilder` (reserva: retiro por el eje con `END_CUT_APPROXIMATED`; el 22 descartó `SetEndReference`, el 25 no encontró la viga de Advance Steel y el 28 la busca en todos los ensamblados antes de probar `BeamShortening`) y
  `Fabrication/EndPlateBuilder` (dos planchas octogonales, filas de pernos como patrones de N × 1, `BOLT_NUTS_APPROXIMATED`),
  `Node/SelectionNodes` (clases de la selección), el despacho por `connection_type` en `ConnectionCreationService`,
  `Validate`/`Preview`/`NodeInfo` con la clase y la excentricidad, `ConnectCommand` con el empalme y el nudo K del mismo
  punto y el desplegable Tipo en Cambiar…, el panel por tipo en `UI/SpecPanel`. Ronda 12c: `RevitModelFacts.GetMemberFacts` lee
  `Ht`/`b` (y la designación si faltan) y `STRUCTURAL_BEND_DIR_ANGLE` y orienta los lados; `AdvanceSteelBackend.CreateWelds`
  escribe `WeldLine` (sondeo 26: `Connect` a las planchas de la misma sesión, `kInShop`; sin plancha, sin `Connect`) con reserva
  DirectShape por recuento al confirmar; `node_info` suma `section_rotation_deg`. Ronda 12d: `Transactions/OperationScope` escribe
  cada fallo de Revit en el log (`revit_failure`: severidad, texto, elementos) y el aviso `REVIT_WARNING` lleva los ids.
  Fase 13: `Fabrication/MemberSplitter` (partir una barra: `CopyElement` + `LocationCurve.Curve`, extensiones 0 y
  `DisallowJoinAtEnd` en el corte, parámetros comparados; reserva `NewFamilyInstance`; `Unsplit`; **probado en Revit el 2026-10-31**:
  `copy_element` sin parámetros aproximados, la reserva no hizo falta; desde la 0.13.1 (cierre) los dos tramos se construyen sobre la
  `Line` exacta de Revit con el punto de corte proyectado en pies, y el registro guarda `original_start_feet` / `original_end_feet`
  para que `Unsplit` restaure la curva exacta; 0.13.2 (cierre de la 13b): la advertencia de Revit "slightly off axis" del tramo nuevo
  (definición `7cba70c0-…`; es de la geometría del modelo: la diagonal se sale 0,006 mm de un plano vertical) queda en el log con
  `filtered: true` **solo en la transacción del corte** (`OperationScope.StartTransaction(…, informativeDefinitions)`), y el registro
  guarda `original_join_allowed` para que `Unsplit` deje las uniones automáticas como estaban, no "permitidas"; la 0.13.2 **probada en
  Revit el 2026-10-31** (ronda 13c): en el Hangar las uniones están desactivadas en los dos extremos, el nudo en K queda igual tras
  deshacer y la cadena parte las dos diagonales sin `REVIT_WARNING`; **Fase 13 cerrada del todo**),
  `Storage/SplitStorageManager` (esquema de Extensible Storage **propio** `MotorConexionesSplit`: un esquema no admite campos nuevos
  con el mismo GUID), `Services/SplitService` (preview, `SPLIT_IN_USE`, partir y deshacer dentro o fuera de un `OperationScope`,
  `UndoSplitsOfConnection`), `Services/SplitChainService` (la cadena D9 de Conectar: grupo exterior, un `OperationScope` por paso,
  el nudo en K revalidado, rollback de todo), `Operations/SplitMemberOperation` (`split` con `dry_run`, sin `validation_token`) y
  `UnsplitOperation`, `SplitMemberCommand` (botón **Partir barra**: `TaskDialog` con un enlace por diagonal y el único cuadro con
  entrada, la distancia), la casilla "Partir la diagonal…" en `PreviewWindow` (`SplitRequested`), **Contorno** en `UI/SpecPanel` y
  **Convertir en polígono** en Avanzado (`PreviewSession.TrySetOutlineMode` / `TryConvertAutoToPolygon`), la cartela automática en
  `ConnectionCreationService` (`AutoOutline.ResolvePoints`), `calculated_values.outline_points_mm` en Validate / Preview /
  CatalogApply, `split_point_mm` en NodeInfo, `BatchCreator` con los pasos de corte y empalme B (y Borrar el lote deshaciendo los
  cortes), `BatchPlanner` con `split_steps` / `split_of_node` / `split_diagonals`. Cierre de la
  12d (0.12.4): la advertencia "only visible in Detail Level: Fine" (definición `73c984d7-…`) se filtra del aviso y queda en el log
  con `filtered: true`; el corte inclinado del nudo en K queda como retiro por el eje, **definitivo** (el sondeo 29 no encontró la
  viga de Advance Steel: no se abren más sondeos sobre ella).
- `src/MotorConexiones.Tests` — xUnit, solo Core, fixture Detalle D.
- `config/limits.json` (Fase 13: `max_auto_gusset_mm`, `auto_outline_max_vertices`), `config/catalog.json` (Fase 13:
  `auto_angle_tolerance_deg`), `config/choices.json` (Fase 11: listas de los desplegables del panel y valores por defecto del
  arranque; Fase 13: `starter.gusset_side_margin_mm`, `starter.outline_mode`), `docs/guide.md` — editables sin recompilar.
- `catalog/` — plantillas oficiales del catálogo de conexiones (Fase 7) y sus miniaturas `.png` (Fase 11); `deploy.ps1` copia
  al PC las que falten.
- `docs/fases/` — un informe por fase y los resultados devueltos por el instalador.
- `docs/instalacion/` — instrucciones literales para el agente instalador, una por fase.
- `docs/propuestas/` — ideas que se aclaran con el usuario antes de convertirse en fase (sin código hasta entonces).
- `docs/prompts/` — prompt y alcance de cada fase posterior al encargo (`fase-N.md`), escritos a partir de la propuesta.
- `mcp/` — archivos nuevos del MCP (`revit_mcp/conexiones.py`, `tools/conn_tools.py`, pruebas, instalador).
- `scripts/` — `deploy.ps1`, `revit-exec.ps1`, `conn-call.ps1` y `sondeos/` (el 20 prueba los grupos anidados con Advance Steel; el 21, las etiquetas del add-in en el lienzo, Fase 10; 22 a 24, Fase 12; 25 = corte con `BeamShortening`, 26 = `WeldLine`, 27 = el perfil del cordón, ronda 12b; 28 = la viga de Advance Steel en todos los ensamblados + `BeamShortening` + `WeldLine` sin plancha, ronda 12c; 29 = los objetos del modelo de Advance Steel por `DatabaseManager.GetModelObjectIds` (parámetro de salida, lo que el 28 saltó), la viga por Guid o geometría y `BeamShortening`, ronda 12d: los 25, 26, 28 y 29 cancelan salvo archivo marca en `%TEMP%`; los que leen la selección van con `-SinTransaccion`; 30 = cómo se
  parte una barra en dos elementos (`CopyElement` + `LocationCurve.Curve`, o `NewFamilyInstance`), sus parámetros, `DisallowJoinAtEnd`,
  `node_info` de los tramos y el deshacer, Fase 13: cancela salvo `%TEMP%\sondeo30-commit.txt`, con `-SinTransaccion`; v2 en el
  cierre: `ElementId(System.Int64(...))`, mide el desvío de eje de la diagonal original y proyecta el punto de corte sobre su línea;
  v3 (cierre de la 13b): el lado del cordón según la convención de la 12c (485 mm), `IsJoinAllowedAtEnd` antes de partir y tras
  deshacer, ids como `int`; **los sondeos 12 y 13 se llaman `12-fase3-borrar.py` y `13-limpiar-fase1.py`**).

## Reglas técnicas que no se negocian

- Conversión de unidades en un único archivo (`Units/UnitConverter.cs`).
- Una operación = un `TransactionGroup`; error = rollback completo. En el lote (Fase 9) cada nudo es una operación
  anidada en un grupo exterior que se asimila: el nudo que falla se revierte solo; `batch_single_undo: false` es el plan B.
- Ninguna ventana en las rutas que usa la IA; el único diálogo permitido es el del botón de la cinta. El manejador de clics
  de las etiquetas del lienzo (Fase 10) tampoco abre cuadros nunca: un cuadro modal ahí deja a Revit sin atender a pyRevit.
- `ElementId.Value` (long), nunca `IntegerValue`.
- `conn_create` y `conn_update` exigen `validation_token` de `conn_validate`.
- El JSON es la única fuente de verdad de la previsualización (Fase 11): el panel escribe por `SpecEditor` y lee de
  `ListFields`; un desplegable con rótulo escribe `*_mm` y `*_label` en la misma edición. Conectar no crea nada al pulsarse.
