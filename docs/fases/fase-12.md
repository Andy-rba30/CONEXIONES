# Fase 12: placa de extremo y nudo soldado (excentricidad en el contrato, tipos `end_plate` y `direct_weld`, Conectar y el lote sobre el cordón superior)

Fecha: 2026-10-12. Rama de trabajo: `claude/fase-12-prompt-placa-nudo-1kj36s` → `main`. Add-in **0.12.0** (Core con la
excentricidad de la opción A en `Geometry3D/NodeFrame`, `NodeReach`, `SpecFrame` y `Catalog/TemplateNode`; los tipos
`Types/DirectWeldType` y `Types/EndPlateType` con sus contratos, esquemas, validadores y croquis; la clase `splice` en
`Batch/NodeDetector`, `PlanBuilder`, `PlanAdvice`, `TrussMap` y `BatchRunner`; los arranques por tipo en `Editing/SpecStarter`
y las listas nuevas de `PanelChoices`; add-in con `Fabrication/DirectWeldBuilder`, `Fabrication/EndPlateBuilder`,
`Node/SelectionNodes`, el despacho por tipo en `ConnectionCreationService`, `Validate`/`Preview`/`NodeInfo` con la clase y la
excentricidad, `ConnectCommand` con las clases de nudo y el empalme + nudo K en el mismo punto, el panel por tipo y el
desplegable **Tipo** en Cambiar…), adaptador 0.12.0 (25 rutas, sin rutas nuevas) y 23 herramientas `conn_*` (sin herramientas
nuevas; el manual de la IA con los tres tipos). Prompt y alcance: `docs/prompts/fase-12.md` (decisiones de las propuestas en su
sección 2.1 y de la persona, D1 a D12, en la 2.2). Commits: `d48e5f3` (Core), `b082cb0` (add-in), `fe40881` (MCP y sondeos) y
el de la documentación.

**Estado: probada en Revit el 2026-10-21** (la 0.12.0, sección 7: los dos tipos se fabrican, el lote de 40 sin fallos);
**ronda 12b cerrada el 2026-10-22** (la 0.12.1 en el PC, los sondeos 25, 26 y 27 y el puente 38/38, sección 8); **ronda 12c
probada en Revit el 2026-10-23 y cerrada el mismo día** (la 0.12.2 en el PC: el perfil leído 203,2 × 304,8 con giro 90 y límite
152,4, las `WeldLine` materializadas en los dos tipos, el 430 en horizontal, puente 38/38; el sondeo 28 no encontró la viga de
Advance Steel, sección 9); **ronda 12d probada en Revit el 2026-10-24 y cerrada el mismo día** (la 0.12.3 en el PC: las advertencias de Revit
leídas en el log, el sondeo 29 sin viga de Advance Steel, puente 38/38, sección 10; el cierre deja el add-in **0.12.4**, que filtra
del aviso la advertencia "solo visible en nivel Fino", y el corte inclinado del nudo en K como retiro por el eje, **definitivo**).
**La Fase 12 queda cerrada.** La última versión desplegada en el PC es la 0.12.3; la 0.12.4 se despliega con la siguiente
instalación.

---

## 1. Qué se hizo

### 1.1 La excentricidad en el contrato y en la validación (opción A; sección 3.2 del prompt; decisiones P1 y P2)

- **Punto de trabajo**: con varias barras cuyos ejes cortan al del cordón en puntos distintos, el origen del marco canónico es
  el punto del eje del cordón **a mitad entre los cortes extremos** (`NodeFrame.Compute(chord, planeMember, others)`); con una
  sola barra, o con todas a menos de 5 mm, es el cruce de siempre. **Regresión exigida y cumplida**: el Detalle D y los 16 nudos
  de `HangarTruss8b` dan el mismo punto, la misma especificación y **los mismos tokens** que antes
  (`EccentricityTests.Regression_TheSixteenNodesOfTheHangar8b_KeepTheirTokensWithTheTopChordSelected`).
- **Una sola regla para el marco** (`Geometry3D/SpecFrame.Compute(spec, facts)`): `end_plate` o primer miembro colineal →
  marco colineal con origen en el punto de empalme; lo demás → marco canónico con todas las barras. La usan el validador, la
  previsualización, la creación, `node_info`, el catálogo y el plan (`TemplateNode.FromModelFacts`): el token es el mismo por
  todos los caminos.
- **Contrato** (solo añade): `node.eccentricity_mm` y `members[i].axis_offset_mm`, opcionales en los tres tipos
  (`[JsonIgnore(WhenWritingNull)]`: una especificación que no las trae serializa igual que antes, así que los tokens de la Fase
  11 no cambian). El validador las mide siempre en el modelo; si la especificación las trae distintas (más de 1 mm) lo dice en
  el aviso. `members[i].chord_element_id` (opcional): el tramo al que llega una barra cuando el nudo está sobre un empalme (D5).
- **Validación** (`Validation/ValidationCommon`, `NodeReach.MemberReachesNodeEccentric`): hasta **medio canto del cordón** (el
  lado mayor del perfil leído del modelo: 152,4 en un HSS12X8; `node_axis_max_offset_mm: 0` en `limits.json`, un número lo
  fija) el desvío del eje es el aviso **`MEMBER_ECCENTRIC`** ("El eje del miembro [1249631] corta el cordón a 17,0 mm del punto
  de trabajo (nudo en K con excentricidad de 34,0 mm; admitida hasta 152,4 mm). Cada barra se recorta y se suelda sobre su
  propio eje."); por encima, `MEMBER_NOT_AT_NODE` como siempre. El hash de `limits.json` **no cambia** con los valores por
  defecto (las claves nuevas solo entran en el hash cuando no son las de defecto): los tokens guardados siguen valiendo.
- **Cada barra sobre su propio corte**: el retiro, la placa cuchilla, los pernos y la soldadura de una barra se miden desde
  donde **su** eje corta al del cordón (`NodeInspector.CutPointOf`, `AxisOffsetMm`; en `ConnectionCreationService` el marco de la
  barra es el del nudo trasladado `axis_offset_mm` a lo largo de X), también en `gusset_node`. Con barras concurrentes es el mismo
  punto de antes.
- **`conn_get_node_info`** suma `kind` (`node` | `splice`), `eccentricity_mm` y, por barra, `axis_offset_mm`, `axis_cut_mm`,
  `end_gap_mm`, `reaches_node` (regla de medio canto), `width_mm`, `height_mm`, `wall_mm`. `conn_validate` devuelve
  `calculated_values.kind` y `eccentricity_mm`. **Conectar** escribe las dos claves en la especificación de arranque solo cuando
  no son cero (`SpecStarter.FromNode`).

### 1.2 Tipo `end_plate` (sección 3.3; Detalles B, C e I; decisiones D2, D3, D8, D9)

- **Contrato** (`Contract/EndPlateSpec.cs`; esquema en `JsonSchemaValidator`): `kind` (`splice`; `to_support` y `base_plate`
  reservados), `node.element_ids` con los dos tramos, `chord` {`element_id`, `profile`, `continuous: false`, `end_setback_mm`},
  `members[0]` {el otro tramo, `role: "chord"`, `end_setback_mm`, `expected_angle_deg: 0`}, `plate` {`thickness_mm` +
  `thickness_label`, `width_mm`, `height_mm`, `corner_cut_mm`, `weld`}, `bolts` {`diameter_mm` + `diameter_label`, `hole_mm` +
  `hole_label` (`LabelParser` entiende `Ø18` y `Ø13/16"`), `nuts: "both_sides"`, `length_mm` opcional, `faces` {`top`,
  `bottom`, `left`, `right`: {`count`, `spacing_mm`, `edge_mm`}}}, `gap_mm`.
- **Geometría** (`Geometry2D/Octagon.Points`: esquinas recortadas → polígono de 8 vértices, rectángulo con `corner_cut_mm` 0;
  `Geometry3D/BoltRing.Rows`: cada cara → una fila centrada en la plancha, a `edge_mm` del borde, `count` pernos a `spacing_mm`;
  `BoltStack.ForTwoPlates`: agarre = 2 espesores + holgura, longitud del perno con el suplemento de `limits.json` redondeada a
  1/4"). En el Detalle C: 12 pernos (±45 / ±135 arriba y abajo, ±40 a los lados), ninguno a menos de 25 del borde ni de 20 de la
  cara del tubo, longitud 2 × 12,7 + 25,4 → 50,8 (`EndPlateTests.DetalleC_HasTwelveBoltsAroundTheTubeAndTheBoltLengthOfTwoPlates`).
- **Validación** (`Validation/EndPlateValidator`): `SPLICE_NOT_COLLINEAR` (los dos tramos forman más de 5°),
  `SPLICE_PROFILES_DIFFER` (aviso; manda el mayor), `PLATE_SMALLER_THAN_TUBE`, `BOLT_EDGE_DISTANCE_TOO_SMALL` /
  `BOLT_SPACING_TOO_SMALL` / `BOLT_OUTSIDE_PLATE` con la ruta `bolts.faces.<cara>.edge_mm`, `BOLT_TOO_CLOSE_TO_TUBE`
  (`end_plate.min_bolt_to_tube_mm`, 20), `BOLT_HOLE_TOO_SMALL` (`end_plate.min_hole_clearance_mm`, 1), `BOLT_LENGTH_TOO_SHORT`,
  `WELD_BELOW_MINIMUM`, `LABEL_VALUE_MISMATCH` en los tres rótulos y **`SPLICE_SETBACK_TOO_SMALL`**: el retiro de cada tramo
  debe dejar sitio a su plancha (espesor + `gap_mm`/2) **menos el hueco que el tramo ya deja en el modelo** (D8; medido con
  `SpecFrame.SplicePoint`, tolerancia 0,05).
- **Croquis** (`Sketch/EndPlateSketch`): alzado de la plancha con su octógono, la huella del tubo, los pernos por cara, las cotas
  con su ruta JSON (ancho, alto, esquina, paso y borde por cara) y la sección con las dos planchas y la holgura.
- **Arranque** (`SpecStarter.ForSplice`, D3 y D8): `starter.end_plate.by_profile` de `choices.json` elige el detalle por el
  perfil del cordón (**C** para HSS12X8X1/2: 430 × 330, 12 pernos Ø3/4"; **I** para HSS3X3X1/4: 200, 4 pernos Ø5/8" a 150; **B**
  para HSS2-1/2X2-1/2X3/16: 185, 4 pernos a 135) y, para cualquier otro, el **Detalle I escalado** (`default`: plancha = tubo +
  2 × (37 + 25) redondeada a 5, esquina 60, borde 25, un perno por cara); el retiro de cada tramo = espesor + holgura/2 − hueco
  medido, redondeado hacia arriba a 0,1 mm (nunca negativo). Si hay una plantilla `end_plate` del mismo perfil, mandan sus
  valores (D10).
- **Fabricación** (`Fabrication/EndPlateBuilder`; **NO PROBADA**): retira los dos tramos desde el punto de empalme
  (`MemberModifier.ApplySetback`), crea las dos planchas octogonales con `CreatePlate` en un marco con Z a lo largo de la barra
  (X perpendicular al plano de la cercha, donde va el canto mayor del tubo: el 12" del Detalle C), a ±(holgura/2 + espesor/2),
  cada fila de pernos como **un patrón de N × 1** (`CreateBoltPattern` con `BoltStack.ForTwoPlates`; la reserva son pernos
  sueltos, que ya funcionaron en la Fase 1), la soldadura de cada plancha al tubo en el contorno de la huella (`CreateWelds`), y
  el aviso **`BOLT_NUTS_APPROXIMATED`** (D9: la tuerca a ambos lados no está en la API que usa el add-in; perno estándar).
- **Fixtures**: `docs/fixtures/detalle-B.json` (IDs ficticios 9101 y 9102: la diagonal es una sola barra), `detalle-C.json`
  (tramos reales 1245531 y 1245530 del plan de la 8c, nudo N9E) y `detalle-I.json` (IDs ficticios 9201 y 9202; la duda 185 → 200
  confirmada, P5), con sus `.png`.

### 1.3 Tipo `direct_weld` (sección 3.4; decisiones P3, D4, D5, D7)

- **Contrato**: `chord` {`element_id`, `profile`, `continuous`, `face: "auto"`}, `members[]` {`element_id`, `role`, `profile`,
  `expected_angle_deg`, `axis_offset_mm`, `chord_element_id` (opcional, D5), `end_cut: "to_chord_face"`, `end_gap_mm` (≥ 0;
  negativo es `SCHEMA_INVALID`), `end_setback_mm` (opcional), `weld` {`type`, `size_mm`, `all_around`}, `splice_hint` opcional
  {`detail`, `offset_from_chord_face_mm`}}. Admite de 1 a 3 barras (D4: N49 y N58, una sola diagonal, salen `ready`).
- **Geometría** (`Geometry3D/DirectWeldGeometry.ComputeEnd`): la barra sale de su corte con el eje del cordón, llega a la cara
  del lado al que va (y = ±medio canto en el plano, el lado menor del perfil: el 8" del HSS12X8), la huella sobre la cara mide
  ancho / |sen|, el retiro ideal es la distancia del corte a la cara más la holgura, y el **retiro por el eje** (la reserva) deja
  la esquina más cercana fuera del cordón.
- **Validación** (`Validation/DirectWeldValidator`): `WELD_BELOW_MINIMUM`, `MEMBER_WIDER_THAN_CHORD_FACE` (aviso: la barra es
  más ancha que la cara), `MEMBERS_OVERLAP_ON_FACE` (aviso: dos huellas se pisan), más las comunes (excentricidad, perfiles,
  ángulos, rótulos). El fixture `nudo-K-cordon-superior.json` (1245533 + 1249631 + 1249632, excentricidad 34, desvíos ∓17)
  valida con **solo** los dos avisos `MEMBER_ECCENTRIC`.
- **Croquis** (`Sketch/DirectWeldSketch`): el cordón con sus dos caras, cada barra cortada contra la cara desde su propio
  corte, la cota del gap entre cortes (34) y la **marca del empalme de la diagonal** (Detalle B) a `splice_hint.offset_from_
  chord_face_mm` (330) medida a lo largo de la diagonal desde la cara: dos trazos y el rótulo "empalme B a 330" (D6; se dibuja y
  se anota, no se fabrica: Fase 13).
- **Arranque** (`SpecStarter.ForKNode`, D7): filete de 5 mm en todo el contorno, `end_gap_mm` 0, `splice_hint` B a 330 en cada
  diagonal, `axis_offset_mm` y `eccentricity_mm` medidos; valores de `starter.direct_weld` de `choices.json` o de una plantilla
  `direct_weld` del mismo cordón.
- **Fabricación** (`Fabrication/DirectWeldBuilder`; **NO PROBADA**): hasta que el sondeo 22 diga qué miembro de la API hace el
  corte plano inclinado, la **reserva** es acortar cada barra por su eje (`MemberModifier.ApplySetback` desde su propio corte)
  lo justo para que su esquina no entre en el cordón, con el aviso **`END_CUT_APPROXIMATED`**, y la soldadura sobre la huella en
  la cara (`CreateWelds`). Log `direct_weld_member` por barra.

### 1.4 Conectar y el lote sobre el cordón superior y los empalmes (sección 3.5; D4, D5)

- **Clase del nudo** (`NodeDetector`, `NodeKind.Node` | `NodeKind.Splice`): dos tramos colineales que se encuentran (ejes a menos
  de 5° y extremos pegados) forman un nudo **`splice`** (antes salía `untyped` o "sin cordón"). Cuando en el mismo punto llegan
  además diagonales, son **dos nudos**: el nudo en K (`N9`, con `SpliceElementId`, aviso `NODE_AT_SPLICE` en vez de
  `NODE_CHORD_NOT_CONTINUOUS`, cada diagonal con `chord_element_id` = el tramo al que llega su eje, D5) y el empalme (`N9E`,
  `Kind: Splice`). Regla para no confundir dos diagonales colineales con un empalme: con otras barras presentes, la pareja solo
  es cordón si es claramente más horizontal que ellas (5°); si no, sigue valiendo la regla "falta el cordón" de la ronda 8b
  (los nudos N25, N30 y N35 de la cercha sintética no cambian). `work_point_mm` es el origen del marco (a mitad entre cortes).
- **Arranques en el plan** (`PlanBuilder`, `PlanRequest.UseStarters`, `SpecStarter.TypeFor`): sin plantilla que encaje, un
  empalme recibe `end_plate` del detalle de su perfil y un nudo con todas las barras **a un lado** del cordón (y cordón continuo
  o sobre un empalme) recibe `direct_weld`; un nudo con barras a los dos lados (cartela) sigue `no_match` como en la Fase 8
  (la cartela automática es la Fase 13). El nudo lleva `IsStarter`, `TemplateName` ("arranque direct_weld", "arranque end_plate
  (Detalle C)"), `ConnectionType` y `StatusDetail`. `TemplateChooser.ForNode` filtra las plantillas por clase (un empalme solo
  casa `end_plate` del mismo perfil) y `Propose` ordena por menos barras sin asignar, más ranuras casadas y después el desvío
  (así una plantilla `direct_weld` de 2 ranuras no gana al Detalle D en un nudo de 3 barras).
- **La cercha de la 8c** (`Fakes/HangarTruss8c`: la 8b más los 8 tramos del cordón superior a Z 19933 con sus 7 empalmes):
  **41 listos** y la cabecera literal "Se crearán 41 conexiones: 16 con Nudo tipico Detalle D (8 iguales, 8 en espejo), 17
  soldaduras directas (arranque), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I). 14 avisan de perfil distinto.
  2 avisan de excentricidad. 7 nudos en K están sobre un empalme." Las 17 soldaduras son los 8 nudos en K, los 7 nudos en K sobre
  empalme y los 2 extremos derechos de una diagonal (N49 y N58); el empalme del Detalle I es el del cordón central HSS3X3X1/4
  (1250938/1250939, que la 8b detectaba y nadie podía conectar). Con una plantilla `direct_weld` guardada desde el fixture, los
  15 nudos en K la usan en vez del arranque (`Plan_HangarTruss8c_WithADirectWeldTemplate_UsesItInsteadOfTheStarter`).
- **Orden de creación** (`BatchRunner.OrderForCreation`): el empalme `XE` se crea antes que su nudo `X`; el fallo de uno no toca
  al otro (`BatchRunner_CreatesTheSpliceBeforeItsKNode`). `PlanMarks` pone al empalme un marcador propio (octaedro pequeño) y el
  mapa un rombo; la ventana del plan enseña la clase, el tipo y la excentricidad en el detalle del nudo.
- **Conectar** (`ConnectCommand`, `Node/SelectionNodes`): la selección se clasifica con el detector (`SelectionNodes.Resolve`);
  dos tramos → empalme; cordón + barras → nudo; **empalme y nudo en K en el mismo punto** → primero la ventana del empalme y,
  al cerrarla, un cuadro Sí/No para abrir la del nudo en K; más de un punto → "parece una cercha: usa Planificar lote". Sin
  plantilla, `SpecStarter.Choose` (la regla de `TypeFor`) y, si no hay regla, el arranque con cartela de la Fase 11. **Cambiar…**
  ofrece además **Tipo: cartela rectangular / soldadura directa / placa de extremo (arranque)** para forzar otro tipo sobre el
  mismo nudo. El `PreviewSession` resume por tipo. Sigue sin crear nada al pulsarse.
- **Panel por tipo** (`SpecEditor.ListFields`, `UI/SpecPanel`): `end_plate` con secciones Plancha, Pernos (una fila por cara con
  `count`, `spacing_mm`, `edge_mm`, el desplegable de agujero `Ø18 · 18,0 mm`, tuercas), Tramo 1, Tramo 2 y Empalme (holgura);
  `direct_weld` con Cordón (cara, excentricidad de solo lectura) y Barra N (desvío de solo lectura, tramo al que llega, corte,
  holgura, soldadura y la marca del empalme con su detalle y los 330). Los tres tipos pasan por `SpecEditor` sobre el JSON.
- **Catálogo**: `TemplateBuilder` guarda plantillas de los tres tipos (quita `axis_offset_mm` y `chord_element_id`, que son del
  nudo), `TemplateSketch` dibuja la miniatura de `end_plate` sin modelo, `catalog_apply` y `Aplicar a la selección` resuelven el
  nudo con `SelectionNodes` (D10: no se recortó).

### 1.5 `diagonal_splice_offset_mm`, configuración, MCP y sondeos (secciones 3.6 y 3.7)

- `config/choices.json`: `hole_labels` (`Ø18`, `Ø13/16"`, `Ø15/16"`, `Ø21`), `diagonal_splice_offset_mm: 330`,
  `starter.direct_weld` (5 mm, todo el contorno, holgura 0, `splice_hint` B) y `starter.end_plate` (`gap_mm`, `by_profile` C / I
  / B, `default` escalado). `config/limits.json`: `node_axis_max_offset_mm: 0` y `end_plate` {`min_bolt_to_tube_mm: 20`,
  `min_hole_clearance_mm: 1`}. `PanelChoices` los lee (embebidos si no hay archivo).
- `Brief/DesignBriefWriter`: el encargo para una IA externa lleva los tres tipos y, sobre un empalme (`node_info.kind: splice`),
  el esquema de `end_plate`.
- **MCP** (0.12.0, sin rutas ni herramientas nuevas): `conn_tools.py` con el manual de los tres tipos, las claves nuevas y los
  códigos; `simulador_revit.py` con las barras del cordón superior y de los fixtures, los tres tipos en `types` y `schema`,
  `validate` por tipo (`MEMBER_ECCENTRIC`, `SPLICE_*`, `BOLT_HOLE_TOO_SMALL`), `node_info` con `kind` / `eccentricity_mm` /
  `axis_offset_mm` / `axis_cut_mm`, `batch_plan` con **varios nudos**, la clase `splice` y los arranques; `probar_conexiones.py`
  con las pruebas 30b a 30e; `mcp/CONTRATO-conn.md` al día.
- **Sondeos** (IronPython, con rollback): `22-corte-plano.py` (lista por reflexión `StructuralFramingUtils` y, si existen
  `CanSetEndReference` / `SetEndReference`, corta la diagonal 1249631 contra la cara del cordón 1245533 dentro de una
  `Transaction` que se deshace; lista los tipos `Beam*` de Advance Steel sin crearlos), `23-pernos-dos-planchas.py` (dos planchas
  cara a cara, una fila de 4 pernos como patrón 4 × 1 con la pila de dos planchas, `Connect` con las planchas, la reserva de
  pernos sueltos y la lista de lo que suene a tuerca a ambos lados; `CancelTransaction` al final) y `24-soldadura-contorno.py`
  (lista `WeldPattern` / `WeldLine` / `WeldPoint` con sus constructores y enumeraciones e intenta una `WeldLine` sobre un lado
  de la huella; cancelado al final). **Ningún miembro de la API se da por supuesto**: todo se busca por reflexión y se anota.

## 2. Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   268, Skipped:     0, Total:   268 - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto (22, 23 y 24 incluidos)
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
  [OK] GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)
  [OK] POST /conn/validate/ del fixture K (direct_weld, 1245533 + 1249631 + 1249632) -> ok:true con el aviso MEMBER_ECCENTRIC y eccentricity_mm 34 (Fase 12)
  [OK] POST /conn/batch/plan/ con los dos tramos -> N1 kind splice, ready, starter con 'arranque end_plate (Detalle C)' ... (Fase 12)
Autocomprobación: 82/82 correctas

$ python3 mcp/pruebas/simulador_revit.py &   (y probar_conexiones.py con --token-archivo el del simulador)
30c. POST /conn/validate/ del fixture nudo-K-cordon-superior.json (direct_weld) -> token con el aviso MEMBER_ECCENTRIC (Fase 12)  [OK]
30d. POST /conn/batch/plan/ (mark:false) con los dos tramos del cordón superior -> un empalme kind splice, ready con el arranque end_plate (Fase 12)  [OK]
Resultado: 35/35 pruebas correctas
```

Las 30 pruebas nuevas del Core (`src/MotorConexiones.Tests/Fase12Tests.cs`, 238 → 268), con los `Fakes` nuevos
`KNode34` (el nudo en K sintético de 34 mm: cordón HSS12X8X1/2 y dos diagonales a −135,6° y −45°) y `HangarTruss8c`:

| Prueba | Qué comprueba |
|---|---|
| `EccentricityTests` (6) | El marco del K de 34 pone el punto de trabajo a mitad, `axis_offset_mm` ∓17 y `eccentricity_mm` 34; un `gusset_node` sobre él da **aviso** `MEMBER_ECCENTRIC` sin error, a 160 mm `MEMBER_NOT_AT_NODE`, con `node_axis_max_offset_mm: 30` error a 34; el hash de `limits.json` no cambia con los valores por defecto; **los 16 nudos de la 8b conservan sus tokens** con el cordón superior seleccionado; cada barra se mide desde su propio corte; el esquema acepta las claves nuevas (presentes, `null`, ausentes) y sigue rechazando las desconocidas |
| `DirectWeldTests` (6) | El fixture K pasa el esquema y valida con solo la excentricidad; soldadura 2 → `WELD_BELOW_MINIMUM`, `end_gap_mm` negativo → `SCHEMA_INVALID`, cuatro barras → `SCHEMA_INVALID`; barra de 150 sobre cara de 203 → `MEMBER_WIDER_THAN_CHORD_FACE`, huellas que se pisan (78°) → `MEMBERS_OVERLAP_ON_FACE`; el croquis con el cordón, las dos barras cortadas, la cota del gap y la marca a 330; el arranque de `ForKNode` valida con los valores de `choices.json`; sobre un K en el empalme cada diagonal lleva `chord_element_id` del tramo al que llega (D5) |
| `EndPlateTests` (7) | Los tres fixtures pasan el esquema y validan sin errores; el C con 12 pernos en las posiciones esperadas, ninguno a menos de 25 del borde ni de 20 del tubo, longitud 50,8; B e I con 4 pernos a 135 / 150; `edge_mm: 10` → `BOLT_EDGE_DISTANCE_TOO_SMALL` en `bolts.faces.top.edge_mm`, el I sobre HSS4X4 pasa con 24 a la cara y con `min_bolt_to_tube_mm: 30` → `BOLT_TOO_CLOSE_TO_TUBE`, agujero 15 → `BOLT_HOLE_TOO_SMALL`, tramos a 8° → `SPLICE_NOT_COLLINEAR`, retiro corto → `SPLICE_SETBACK_TOO_SMALL`, `length_mm` 30 → `BOLT_LENGTH_TOO_SHORT`; el octógono de 185 con 8 vértices y los pernos dentro, `corner_cut_mm` 0 → rectángulo; el croquis con sus cotas y rutas; `ForSplice` elige C / I / B por perfil y el `default` escalado para HSS4X4X3/16; el panel lista las secciones de cada tipo y el agujero con su rótulo |
| `Fase12PlanTests` (11) | Dos tramos colineales → `splice` (ya no `untyped`); la 8c con los 15 nudos en K, los 7 empalmes y sus compañeros `N9E`…; el plan de la 8c: 41 listos y la cabecera literal, 16 Detalle D con los mismos tokens que la 8b, 15 K `direct_weld`, 7 empalmes C, 1 empalme I, N49 y N58 `ready`; con plantilla `direct_weld` los K la usan; sin arranques ni plantillas el detalle dice por qué falta cada tipo; la cercha sintética suelda cada nudo de un lado; la 8b empalma el cordón central con el I escalado (plancha 200); `BatchRunner` crea `N9E` antes que `N9` y el fallo de uno no toca al otro; guardar y aplicar una plantilla `end_plate`; `choices.json` del repositorio con las claves nuevas; `conn_get_schema` de los tres tipos devuelve esquemas distintos y sus ejemplos validan |

Las 238 pruebas anteriores pasan; las que planifican cerchas (`BatchPlanTests`, `BatchCreateTests`, `PlanAdviceTests`,
`TrussMapTests`, `NodeDetectorTests`, `PlanActionsTests`) corren con `UseStarters = false` para seguir midiendo lo de la Fase 8
(los conteos nuevos están en `Fase12PlanTests`), y cambian donde el detector ahora ve empalmes: la pareja 1250938/1250939 de la
8b sale `splice` (antes `no_match` "Empalme del cordón") y `Splice_IsDetectedOnTwoChordSegmentsThatEndAtTheNode` se reescribió
para los dos nudos en el punto.

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-12.md`, unos 90 minutos) — **hecho el 2026-10-21, contrastado en 7.1**

| Qué | Paso |
|---|---|
| `deploy.ps1` 0.12.0.0 con `limits.json` y `choices.json` con las claves nuevas; `instalar-conn.ps1` 25 rutas y 23 herramientas; `ping` 0.12.0; `list_types` con **3** tipos; sondeos 12, 13, 17 y 21 a cero | 12-1, 12-2 |
| **Sondeo 22**: ¿`StructuralFramingUtils.SetEndReference` corta la diagonal contra la cara del cordón? Qué tipos `Beam*` tiene Advance Steel. **Sondeo 23**: dos planchas pegadas, la fila 4 × 1, `Connect`, los pernos sueltos, lo que suene a tuerca doble. **Sondeo 24**: `WeldLine` / `WeldPoint` con sus firmas y si una se escribe. Los tres con rollback y sondeos 12 y 13 a cero después | 12-3 |
| `node_info` sobre 1245533 + 1249631 + 1249632: `kind: node`, `eccentricity_mm` ≈ 34, `axis_offset_mm` ≈ ∓17; sobre 1245531 + 1245530: `kind: splice` | 12-4 |
| **Conectar en el nudo en K**: la cabecera del arranque `direct_weld`, el croquis con el gap y la marca a 330, el panel, 6 mm y vuelta, validación en verde con `MEMBER_ECCENTRIC`; **Crear**: cómo quedó cada extremo (retiro por el eje con `END_CUT_APPROXIMATED`) y la soldadura; Guardar en catálogo | 12-5 |
| **Conectar en el empalme** (1245531 + 1245530): arranque Detalle C, la plancha con 12 pernos, el borde a 10 → `BOLT_EDGE_DISTANCE_TOO_SMALL` junto al campo; **Crear**: planchas, pernos (con `BOLT_NUTS_APPROXIMATED`), tramos retirados | 12-6 |
| Una sola barra desde Conectar (pide pinchar las demás) y media cercha ("parece una cercha") | 12-7 |
| **Planificar lote** con las 64 barras de la 8c: la cabecera con 16 + 15 + 7 (+ 1), dos `already_connected`, el mapa con los rombos, N9 y N9E; **Crear**; el informe y los `failed` con su error literal | 12-8 |
| Por el puente: `validate` de los fixtures C y K, `batch_plan` sobre los 4 elementos de N9 (dos nudos), `list`, Borrar el lote y `delete` de las dos sueltas (¿se deshace el corte?), sondeos a cero, `probar_conexiones.py --puente` 38/38, el log sin fallos | 12-9 |

### 2.2 NO PROBADO en la nube y por qué

- **Toda la fabricación en Revit**: el corte contra la cara (hoy la reserva por el eje), las planchas octogonales
  (`CreatePlate` con 8 vértices), los pernos a través de dos planchas sin cartela, **las filas por cara como patrones de N × 1**
  (Advance Steel puede rechazar un patrón de una sola fila: el backend cae a DirectShape), la soldadura en el contorno
  (DirectShape, como siempre) y el retiro real de los tramos desde el punto de empalme. Compila contra la API 2027 y los tres
  sondeos deciden; la tabla de reservas está en la sección 5.
- **`node_info` sobre el modelo**: la excentricidad medida en el Hangar (34 mm según el paso 11-5 de la Fase 11), la orientación
  del perfil HSS12X8 (el 12" en horizontal según el Detalle C; `RevitModelFacts` lee `b` / `h` del tipo) y por tanto en qué
  sentido sale la plancha de 430 × 330.
- **Conectar y el lote de punta a punta**, los croquis y los paneles nuevos en WPF, el cuadro Sí/No del empalme + nudo K, el
  desplegable Tipo de Cambiar…, el marcador y el rombo del empalme, dos etiquetas en el mismo punto (N9 y N9E).
- **El camino de una sola barra y el de la cercha desde Conectar** (pendiente desde la Fase 11).
- **La prueba 32b del puente** y las 30b a 30e **contra Revit** (en la nube solo contra el simulador).

## 3. Qué debo mirar yo en Revit cuando el instalador termine (hecho el 2026-10-21: lo visto está en 7.1 y 7.2)

1. `resultados-fase-12.md`, paso 12-3: la salida literal de los **tres sondeos**. Del 22, la lista de métodos de
   `StructuralFramingUtils`, si `SetEndReference` dejó "caras inclinadas del extremo: 1" y la extensión antes y después; si
   funcionó, `DirectWeldBuilder` pasa a usarlo en la 12b (sin `END_CUT_APPROXIMATED`). Del 23, si la fila 4 × 1 escribió con
   `NumberOfScrews 4`, si `Connect` unió el patrón a las dos planchas y la lista de propiedades con Nut/Washer/Assembly; si el
   4 × 1 falló, `EndPlateBuilder` pasa a pernos sueltos. Del 24, la firma de `WeldLine` y si se escribió: con eso `CreateWelds`
   deja de ser DirectShape en la 12b.
2. Paso 12-4: `node_info` del nudo en K con `eccentricity_mm` ≈ 34 y `axis_offset_mm` ≈ ∓17 (si sale otro número, el fixture K
   y `KNode34` se corrigen con el real); el empalme con `kind: splice`.
3. Capturas `fase12-03-nudo-k` y `fase12-04-nudo-k-creado`: la cabecera "Especificación de arranque de soldadura directa…", el
   croquis con el gap de 34 y la marca a 330, la validación en verde con un solo tipo de aviso, y en Revit los dos extremos
   acortados (por el eje) con las soldaduras sobre la cara.
4. Capturas `fase12-05-empalme` y `fase12-06-empalme-creado`: los 12 pernos del Detalle C en el croquis, el error junto al
   borde de 10, y en Revit las dos planchas de 430 × 330 **con el 430 en horizontal** (si sale girada, la orientación del
   perfil se lee al revés: se anota y se corrige), los pernos atravesando las dos y los tramos retirados 12,7.
5. Captura `fase12-07-plan`: la cabecera con 16 (dos `already_connected`) + 15 + 7 (+ 1), los rombos de los empalmes, N9 y N9E
   en el detalle; `fase12-08-lote` con el informe y los `failed` literales (el motivo más probable es la fabricación de un
   tipo nuevo: con eso se decide la 12b).
6. Paso 12-9: los dos `delete` con las barras restauradas (si el corte del sondeo 22 se hubiera usado, si se deshace), los
   sondeos a cero, el `--puente` 38/38 y el log del día sin `ribbon_connect_failed`, `ribbon_create_failed`,
   `advance_steel_plate_failed`, `advance_steel_bolts_failed`, `fabrication_transaction_failed`, `plan_event_failed` ni
   `ribbon_batch_window_error`.

## 4. Decisiones tomadas y por qué

### 4.1 Un solo marco por especificación (`SpecFrame.Compute`), elegido por el tipo y la geometría

El prompt pedía que validación, previsualización, creación, `node_info`, catálogo y plan dieran el mismo punto y el mismo
token. La forma segura es una sola función que decide el marco: colineal (origen en el punto de empalme) si el tipo es
`end_plate` o el primer miembro es colineal con el cordón; canónico con todas las barras en lo demás. `NodeInspector`,
`TemplateNode.FromModelFacts` y `SketchNodeInfo` la llaman; las pruebas de regresión de los tokens de la 8b lo vigilan.

### 4.2 El punto de trabajo es la media de los cortes **extremos**, no de todos

Con tres barras (dos diagonales excéntricas y un montante en medio) la media de los tres cortes se movería al añadir o quitar
el montante; la media de los dos extremos no. En los nudos concurrentes (todos a menos de 5 mm) se usa el cruce exacto del
primer miembro, como siempre: por eso los tokens de la 8b no cambian.

### 4.3 Medio canto: el lado mayor para el límite, el lado menor para la cara

`MEMBER_ECCENTRIC` admite el desvío hasta medio canto del cordón con el **lado mayor** del perfil (152,4 en un HSS12X8), como
pedía P2 ("medio canto") y como lo lee `node_info`. La cara contra la que se corta la diagonal está a medio canto **en el plano
de la cercha**, que es el lado menor (101,6 si el 12" va en horizontal): `DirectWeldGeometry` usa ese. Si el modelo tiene el
perfil girado, la plancha y la cara giran con él y el instalador lo anota (paso 12-6).

### 4.4 Las claves nuevas son invisibles para quien no las usa

`eccentricity_mm` y `axis_offset_mm` llevan `[JsonIgnore(WhenWritingNull)]` (y de paso `gusset` y `attachment`, que los tipos
nuevos no tienen: antes `spec.ToJson()` escribía `gusset: null` y el esquema de `direct_weld` lo rechazaba). El token se calcula
sobre el JSON canónico, así que una especificación de la Fase 11 da el mismo token. Las claves nuevas de `limits.json` solo
entran en su hash cuando no valen lo de defecto.

### 4.5 Dos tramos colineales solo son empalme si no hay un cordón más horizontal

En la cercha sintética hay nudos donde dos diagonales son colineales y el cordón no está en la selección (N25, N30, N35):
antes salían "falta el cordón" y con la regla nueva salían `splice`. Con otras barras presentes, la pareja colineal solo cuenta
como cordón empalmado si es más horizontal que ellas (5°); si no, sigue el "falta el cordón" de la 8b. Con solo dos barras
colineales (el caso real de los empalmes), siempre `splice`.

### 4.6 El plan arranca `direct_weld` y `end_plate`, pero no la cartela

`SpecStarter.TypeFor` da `gusset_node` a un nudo con barras a los dos lados, y Conectar lo usa (la cartela rectangular de la
Fase 11); el plan, no: un lote de cartelas rectangulares "a ojo" sobre 16 nudos no es lo que nadie quiere crear sin mirar, y la
cartela automática es la Fase 13. Así los 16 del Detalle D siguen dependiendo de la plantilla y el catálogo vacío sigue diciendo
"Sin plantillas en el catálogo". Las pruebas de la 8 corren con `UseStarters = false` para medir exactamente lo de entonces.

### 4.7 El retiro del empalme se calcula con el hueco medido (D8) y se redondea hacia arriba

El plan de la 8c dice que algunos tramos ya dejan hueco en el empalme (`end_gap_mm`). El arranque escribe `end_setback_mm` =
espesor + holgura/2 − hueco, redondeado **hacia arriba** a 0,1 mm, y el validador comprueba la misma cuenta con 0,05 de
tolerancia: sin el redondeo, 9,5 contra 9,525 daba `SPLICE_SETBACK_TOO_SMALL` en el propio arranque.

### 4.8 Las filas por cara como patrones de N × 1 y la pila sin cartela

`CreateBoltPattern` ya sabe crear un patrón rectangular con una `BoltStack`; una fila es un patrón de N × 1 y la pila de dos
planchas (`ForTwoPlates`) tiene la misma forma que cartela + placa (dos espesores y una holgura). El marco de los pernos se
desplaza −(espesor + holgura)/2 para que la pila quede centrada en el punto de empalme. Si el sondeo 23 dice que Advance Steel
rechaza `Ny = 1`, el backend ya cae solo a DirectShape y en la 12b se cambia a pernos sueltos (la Fase 1 los creó así).

### 4.9 El corte inclinado se decide en el PC, no se inventa

No hay en el repositorio ninguna prueba de que `StructuralFramingUtils.SetEndReference` o algún `Beam*` de Advance Steel corte
el extremo de una barra contra una cara, así que el add-in no lo llama: acorta por el eje con el aviso `END_CUT_APPROXIMATED` y
el sondeo 22 lo busca por reflexión y lo prueba con rollback. Si funciona, la 12b lo pone en `DirectWeldBuilder` sin cambiar el
contrato.

### 4.10 La diagonal 1249631 es la misma barra en el Detalle D y en el nudo en K

Su extremo inferior es una de las tres barras del Detalle D y el superior llega al cordón superior 1245533: el simulador la
conoce con dos ángulos (135,6° en el Detalle D y −135,6° en el K, según el cordón) y el fixture K la usa tal cual. El sondeo 22
la corta por el extremo **alto**.

## 5. Pendientes, riesgos y preguntas

Qué reserva se activa si cada sondeo falla (ya programadas; el instalador solo anota):

| Sondeo | Si funciona | Si falla (la reserva que ya está en 0.12.0) |
|---|---|---|
| 22, corte plano | 12b: `DirectWeldBuilder` corta con el miembro que el sondeo diga (sin `END_CUT_APPROXIMATED`) | Retiro por el eje desde el corte propio de cada barra, con el aviso `END_CUT_APPROXIMATED`; la soldadura sobre la huella |
| 23, pernos 4 × 1 | Se queda como está | 12b: cada fila como pernos sueltos (patrones de 1 × 1), que ya funcionaron en la Fase 1; mientras, el backend cae a DirectShape con `REVIT_WARNING` |
| 23, tuerca a ambos lados | 12b: se fija la propiedad o el conjunto que el sondeo encuentre | Perno estándar con `BOLT_NUTS_APPROXIMATED` (D9) |
| 23, `Connect` con las planchas | 12b: el patrón se une a las dos planchas | Como hoy: el patrón no se une (los pernos de la cartela tampoco se unen) |
| 24, `WeldLine` | 12b: `CreateWelds` con Advance Steel | DirectShape, como desde la Fase 1 |

- **Riesgo 1 (la plancha girada)**: si el modelo tiene el HSS12X8 con el 12" en vertical, la plancha de 430 × 330 sale girada 90°
  respecto al Detalle C. `RevitModelFacts` lee `b` / `h`; se anota en el paso 12-6 y, si hace falta, el arranque intercambia
  ancho y alto según el perfil leído.
- **Riesgo 2 (dos nudos en un punto)**: `PlanMarks`, las etiquetas y el mapa daban por hecho "un punto, un nudo". El empalme
  lleva su marcador y su etiqueta; en Revit dos etiquetas en el mismo punto pueden solaparse (el sondeo 21 cuenta las etiquetas
  tras el plan; se mira en la captura `fase12-07`).
- **Riesgo 3 (el lote de la 8c con los tipos nuevos)**: 41 conexiones con tres fabricaciones distintas; el fallo más probable es
  `FABRICATION_FAILED` en un empalme o un nudo en K, que se revierte solo (Fase 9) y queda `failed` con su error literal. Con eso
  se decide la 12b.
- **Riesgo 4 (`MEMBER_NOT_AT_NODE` relajado)**: una especificación que antes fallaba ahora valida con aviso hasta medio canto. Es
  lo decidido (P2); la guía lo explica y el aviso lleva los mm.
- **P1**: la 12b (D12) con lo que digan los sondeos: corte plano, pernos, soldadura; y la **Fase 13**: cartela automática
  (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle B a 330 (D6).
- **P2**: el cordón inferior (HSS3X3X1/4) sigue sin saberse si está modelado como barra (D1b): el Detalle I entra como fixture y
  arranque por perfil y se crea solo si la persona lo selecciona.
- Lo de siempre: `conn_design_brief`, `thumbnail_path`, `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`, `base_plate`
  y `to_support` (el montante vertical, D11), `plates.second`, V6 (editar arrastrando).

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando (hecho el 2026-10-21; lo que sigue, en 7.7)

1. Dar `docs/instalacion/fase-12.md` al agente instalador en el PC (Revit cerrado al empezar, sobre la copia).
2. Dejar que corra **primero los tres sondeos** (paso 12-3) y mirar su salida antes de seguir: si el 22 corta, el 23 escribe la
   fila 4 × 1 y el 24 escribe una `WeldLine`, la 12b es corta; si no, las reservas ya están y la 12b decide con la tabla de la
   sección 5.
3. Hacer los pasos marcados **(la persona)**: Conectar en el nudo en K (crear, guardar en catálogo), Conectar en el empalme
   (crear), una sola barra y media cercha, Planificar lote con las 64 barras y Crear, con las ocho capturas.
4. Devolver `docs/fases/resultados-fase-12.md` y las capturas; con eso se escribe el cierre de la fase (sección 7) y la ronda
   12b con lo que los sondeos digan (D12).

## 7. Cierre de la Fase 12 (2026-10-21): la 0.12.0 contrastada en Revit y la ronda 12b (add-in 0.12.1)

Resultados: `docs/fases/resultados-fase-12.md` (instalador, commit `ff1e263`, add-in **0.12.0**, 2026-10-21, sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`), las anotaciones de la persona en el chat y las capturas `fase12-03` a `fase12-08`. **Todo lo que
la fase programó funciona en Revit**: los dos tipos nuevos se crean con Advance Steel (las planchas octogonales, las filas de
pernos como patrones de N × 1 atravesando las dos planchas, los retiros), Conectar reconoce las clases, y el lote de la 8c creó
**40 de 40** conexiones de tres tipos en 6,3 s sin un solo `failed`. Lo que no coincidió es de medida y de sondeo, no de
fabricación: la excentricidad real es **54,9 mm** (no 34), el add-in lee el HSS12X8 como **203 × 203**, los sondeos 22 y 24
**listaron las firmas buenas pero no las intentaron**, y el puente dio **36/38** porque `validate` del add-in no devolvía
`kind` ni `eccentricity_mm` (7.2). De ahí la ronda **12b** (0.12.1 + sondeos 25, 26 y 27; 7.3 y 7.5).

### 7.1 Contraste con lo esperado en 2.1

| Qué se esperaba (2.1) | Qué pasó | ¿Coincide? |
|---|---|---|
| 12-1, 12-2: `deploy.ps1` 0.12.0.0 con las claves nuevas de `limits.json` y `choices.json`; 25 rutas y 23 herramientas; `ping` 0.12.0; **3** tipos; sondeos a cero | `== MotorConexiones 0.12.0.0 desplegado en Revit 2027 ==`, DLL `0.12.0.0`, `node_axis_max_offset_mm`, `min_bolt_to_tube_mm` y `min_hole_clearance_mm` en el `limits.json` desplegado; `ping` `addin_version 0.12.0` sobre `HANGAR_PRUEBA_sondeo`; `types` con `direct_weld`, `end_plate` y `gusset_node`; `catalog_list` con 1; sondeos 12, 13, 17 y 21 a cero; `startup` 0.12.0 | **Sí** |
| 12-3 **Sondeo 22**: ¿`SetEndReference` corta contra la cara? Los tipos `Beam*` de Advance Steel | `CanSetEndReference(diagonal, 1) = False` y `SetEndReference` lanza "is not joined at given end and cannot have an end reference set": **el camino de Revit no sirve** sin unión previa (Transaction deshecha, extensión igual). Advance Steel tiene `BeamShortening(eEnd, Double)` con `AngleOnY`, `AngleOnZ`, `CutStraight`, `CutStraightOffset` (el "acortar con ángulo" = corte plano inclinado), `BeamMultiContourNotch(Beam, eEnd, Point3d[], Vector3d, Vector3d)`, `BeamNotch2Ortho`, `BeamNotchEx`, `PlateFeatShortening`. **El sondeo no creó ninguno** ("sin firma conocida", como estaba escrito). De paso midió la cara interior del cordón a 89,8 mm del eje = 101,6 − 11,8 de pared: **el 8" del HSS12X8 está en el plano de la cercha**, como supuso 1.3 | **Parcial**: decide el camino de Revit (no), no el de Advance Steel (sondeo 25) |
| 12-3 **Sondeo 23**: dos planchas pegadas, la fila 4 × 1, `Connect`, los pernos sueltos, la tuerca a ambos lados | `planchas: 2 planchas de 200 x 200 x 9,525 con gap 0: WriteToDb OK`; **`fila 4x1: WriteToDb OK; NumberOfScrews=4; ScrewLength=44.45; BindingLength=19.05`**; **`Connect: OK con kOnSite`** (`Connect(FilerObject[], eAssemblyLocation)`; valores `kOnSite, kSiteDrill, kInShop, kWrong, kUnknown`); `sueltos 1x1: 4/4 escritos`; 0 elementos nuevos con la transacción abierta y tras cancelar. Tuerca doble: **no hay tipo ni método**: solo `BoltAssembly`, `Assembly`, `Standard`, `Grade`, `IsInverted`, `NutDiameter` / `NutHeight` (lectura), `SumTopSetHeight` / `SumBottomSetHeight`; el único tipo con esos nombres es `eAssemblyLocation` | **Sí**: la fila de N × 1 y `Connect` funcionan (la reserva de pernos sueltos no hace falta); la tuerca doble queda como D9 (`BOLT_NUTS_APPROXIMATED`) |
| 12-3 **Sondeo 24**: `WeldLine` / `WeldPoint` con sus firmas y si una se escribe | Lista completa: **`WeldLine(Point3d[] polyLineWCS, Vector3d xvec, Vector3d yvec)`** y **`WeldPoint(Point3d origin, Vector3d xvec, Vector3d yvec)`** existen, con `Connect`, `WriteToDb`, `IsClosed`, `MainEffectiveThroat`, `DoubleEffectiveThroat`, `Length`…; `eWeldType` sin `kFillet` (hay `kDoubleFilletWeld`, `kTWeld`, `kLineWeld`, `kSeamWeld`…), `eSeamPosition`, `eWeldSurface`, `eExecType`. Pero el sondeo buscaba `WeldLine(Point3d, Point3d)`, `(Point3d[])`, `(Point3d, Vector3d)` o `(Point3d, Point3d, Vector3d)`: **"sin constructor utilizable" y no lo intentó**; la plancha de apoyo sí se escribió y se canceló | **Parcial**: la firma está, falta escribirla (sondeo 26) |
| 12-4 `node_info` del nudo en K: `kind: node`, `eccentricity_mm` ≈ 34, `axis_offset_mm` ≈ ∓17; del empalme: `kind: splice` | `kind: node`, **`eccentricity_mm: 54.9`**, `axis_offset_mm` **−27,4** (1249631, `axis_cut_mm` X −9305,5) y **+27,4** (1249632), `origin_mm` X −9278,1, `end_gap_mm` 90,4; Detalle D con `eccentricity_mm 0` y `axis_offset_mm` 1,7 / 0 / −1,7 (regresión bien). Empalme 1245531 + 1245530: `kind: splice`, `eccentricity_mm 0`, origen en el punto de empalme (−4180,5; 17204,2; 19933), `end_gap_mm` 0,4. **Pero** el HSS12X8X1/2 sale con `width_mm 203.2` **y** `height_mm 203.2` (debería ser 304,8 × 203,2) | **Casi**: la clase y la mecánica sí; el número de la excentricidad era otro (7.2.b) y el perfil se lee mal (7.2.c) |
| 12-5 **Conectar en el nudo en K**: cabecera `direct_weld`, croquis con el gap y la marca a 330, validación en verde con `MEMBER_ECCENTRIC`, **Crear** con `END_CUT_APPROXIMATED`, guardar en catálogo | Captura `fase12-03`: cabecera literal "Conectar · nudo del cordón 1245533 · arranque direct_weld …" y "Especificación de arranque (Conectar) · soldadura directa · cordón HSS12X8X1/2 · excentricidad 54.9 mm · 2 barra(s) cortadas contra la cara, soldadas en todo el contorno · empalme Detalle B a 330 mm"; el croquis con la cota "54,9 excentricidad", el ángulo, la soldadura 5,0 y las dos marcas "empalme B 330,0"; el panel Cordón (cara `auto`, excentricidad 54,9 de solo lectura) y Barra 1 (desvío −27,4); "Validación correcta con 2 aviso(s)" `MEMBER_ECCENTRIC` en `members[0/1].axis_offset_mm`. Captura `fase12-04`: cuadro "Conexión modelada correctamente" con **2 elementos**, **2 retiros**, `advancesteel`, `END_CUT_APPROXIMATED`; en el modelo las dos diagonales acortadas por el eje y las dos soldaduras (DirectShape) sobre la cara inferior del cordón. Log `direct_weld_member`: `axis_offset_mm −27.44 / 27.44`, `setback_ideal_mm 145.28 / 143.68`, `axis_setback_mm 177.73 / 175.43`, `face_y_mm −101.6`, `footprint_mm 90.8 / 89.8`. Guardado como **"Nudo K cordon superior"** (`91adac27…`, `direct_weld`, HSS12X8X1/2, 2 barras) | **Sí** |
| 12-6 **Conectar en el empalme**: arranque Detalle C con 12 pernos, el borde a 10 → error junto al campo, **Crear**: planchas, pernos con `BOLT_NUTS_APPROXIMATED`, tramos retirados; ¿el 430 en horizontal? | Captura `fase12-05`: cabecera "Conectar · empalme de los tramos 1245530 y 1245531 · arranque end_plate (Detalle C) …" y "… plancha PL 1/2" · 12,7 mm 430 x 330 con esquinas de 60 · 12 pernos 3/4" alrededor del tubo, tuerca a ambos lados"; el croquis con la plancha vista por el eje (4 + 4 + 2 + 2 pernos) y el perfil del empalme con "112,3 retiro"; el panel Plancha / Pernos / …; "Validación correcta". Captura `fase12-06`: **14 elementos**, 2 retiros, `BOLT_NUTS_APPROXIMATED`; las dos planchas, los pernos atravesándolas y los tramos retirados. Log: `advance_steel_plate_written` × 2 (8 vértices, 12,7, `plane_z` 0 y −12,7), `advance_steel_bolts_written` × 4 (`Nx=4` / `Nx=2`, `Ny=1`, `ScrewDiameter=19.05`, `BindingLength=25.4`, `ScrewLength=50.8`), `end_plate_built` con `setbacks_mm [12.3, 12.3]` (12,7 − 0,4 de hueco medido: D8 bien), `plate_frame_z` ±X. **La plancha**: la persona anotó que el 430 salió **en vertical**; el log dice `tube_footprint_mm [203.2, 203.2]` (7.2.c). Los tramos elegidos (y = +17204) son del empalme de **otra cercha**, no de la 8c: válido igual. El error del borde a 10 no se anotó | **Casi**: la fabricación entera sí; la orientación de la plancha queda en duda hasta el sondeo 27 |
| 12-7: una sola barra desde Conectar (pide pinchar) y media cercha ("parece una cercha") | Sin sección ni captura: **no se anotó** (sigue pendiente desde la Fase 11) | **Sin ver** |
| 12-8 **Planificar lote** con las 64 barras: 16 + 15 + 7 (+ 1), los rombos, N9 y N9E; **Crear**; los `failed` | Captura `fase12-07`: cabecera literal "Se crearán 40 conexiones: 29 con 2 plantillas (Nudo tipico Detalle D: 16, Nudo K cordon superior: 13) (21 iguales, 8 en espejo), 7 empalmes (arranque Detalle C), 1 empalme (arranque Detalle I escalado), 3 soldaduras directas (arranque). 14 avisan de perfil distinto. 1 avisa de excentricidad. 7 nudos en K están sobre un empalme. 1 ya tiene conexión. Ocultos: 8 sin cordón, 17 barras sueltas." = **41 nudos** (16 D + 13 K con la plantilla del 12-5 + 3 K de arranque + el K del 12-5 ya conectado = 17 soldaduras; 7 C + 1 I), "64 barras seleccionadas (63 añadidas por la selección asistida) · … 41 etiquetas pinchables y 16 cartelas fantasma"; el mapa con los rombos de los empalmes; N9 "▲ Listo con aviso · Nudo K cordon superior · Sobre el empalme N9E…" y N9E "● Listo · arranque end_plate (Detalle C)". Captura `fase12-08`: **"Lote dd6cf93f: 40 conexiones creadas (38 con aviso). Una sola entrada de deshacer (Ctrl+Z). 6.3 s."**, `failed 0`, `rolled_back 0`; detalle de N53/N54 con el casado y los avisos. Log: `batch_create` `requested 40, created 40`, 8 `end_plate_built` (7 de 430 × 330 y el del cordón central de 230 × 230 × 9,525 con 4 pernos) y 21 `direct_weld_member` (N49 con `chord_element_id 1245543`: D5 bien). Sondeos 17 y 21 tras crear: queda **1** marcador y 1 control (el de N5 `already_connected`, gris: los de los creados se quitaron) | **Sí** (las dos cifras que cambian respecto a la nube se explican por la conexión del 12-5: 13 + 3 + 1 en vez de 17 arranques, y 1 aviso de excentricidad en vez de 2) |
| 12-9: `validate` C → `kind: splice`; K → 2 `MEMBER_ECCENTRIC` y ≈ 34; `batch_plan` N9 → dos nudos; `batch_delete` y `delete` × 2 con barras restauradas; sondeos a cero; `--puente` 38/38; el log sin fallos | `validate` C: `is_valid true`, token, `bolt_stacks` (12 pernos, agarre 25,4, L 50,8) **pero `calculated_values` sin `kind`**; K: `is_valid true` con los 2 avisos ("corta el cordón a −27.4 mm … excentricidad del nudo 34.0 mm admitida hasta **101.6** mm. El JSON declara axis_offset_mm = −17.0 y el modelo mide −27.4"). `batch_plan` con 4 elementos: N3 `ready` con **"Nudo K cordon superior"** y N3E `kind: splice` `already_connected` (el empalme del lote): **dos nudos en el punto**. `batch_delete`: **40 borradas, 285 elementos**; `delete` K: 2 elementos y 2 barras restauradas; `delete` empalme: 14 y 2; las tres con el aviso `REVIT_WARNING` "No se pudo abrir la FabricationTransaction … asynchronous fabrication tasks are queued … Los elementos se borran directamente (Document.Delete)" (41 `fabrication_transaction_failed` con `for_deletion: true` en el log: 7.2.e). Sondeos 12 (0 conexiones; 1249631 `fin 69.2`, como antes del día) y 13 (0 sueltos) a cero; 17 y 21 con **1** marcador / 1 control (N5: el plan no se descartó; 7.2.f). `ping` sobre la copia; servidor 8000 escuchando; **`--puente` 36/38**: fallan **30b** y **30c** por `kind=None excentricidad=None` (7.2.d), las 30d, 30e, 31, 32 y 32b pasan; `templates_count 2` (Detalle D y Nudo K cordon superior). El log del día **sin** `ribbon_connect_failed`, `ribbon_create_failed`, `advance_steel_plate_failed`, `advance_steel_bolts_failed`, `plan_event_failed` ni `ribbon_batch_window_error` | **Casi**: todo lo que es del add-in, sí; el puente 36/38 es un hueco del add-in que la 12b cierra |

### 7.2 Lo que no coincidió y qué se hace con ello

- **a. Los sondeos 22 y 24 encontraron las firmas y no las probaron.** Estaban escritos para listar sin crear (22, camino B) o
  para buscar otras firmas (24). Lo que hay que probar ahora es concreto: `BeamShortening(eEnd, Double)` + `AddFeature` con
  `AngleOnY` (el corte plano inclinado), `BeamMultiContourNotch(Beam, eEnd, Point3d[], Vector3d, Vector3d)`, `WeldLine(Point3d[],
  Vector3d, Vector3d)` y `WeldPoint(Point3d, Vector3d, Vector3d)` con `Connect` y `WriteToDb`. Para cortar una **viga de Revit**
  con Advance Steel hace falta su objeto `Beam` de Advance Steel, y el puente documentado en la Fase 1 es
  `SteelElementProperties.AddFabricationInformationForRevitElements` + `GetFabricationUniqueID(doc, Reference)` y, del lado de
  Advance Steel, `FilerObject.GetFilerObject(ObjectId)` / `GetUniqueId()`: no hay un método documentado "por Guid", así que el
  sondeo busca por reflexión todos los estáticos que reciban un `Guid` y, si ninguno sirve, recorre los `ObjectId` del modelo.
  → **Sondeos 25 y 26** (7.3), con `CancelTransaction` por defecto y un archivo marca para repetirlos con `Commit` y verlos.
- **b. La excentricidad del nudo en K es 54,9 mm, no 34.** El 34 venía de la ronda 8b (una medida indirecta sobre el plan); el
  12-4 la mide con la regla de la Fase 12 (corte de cada eje con el eje del cordón): −27,4 y +27,4. No cambia nada del
  mecanismo (opción A); cambia el número en el fixture K, el nudo sintético de las pruebas (`KNode34` → **`KNode55`**), el
  simulador, la guía, el manual de `conn_get_node_info` y el contrato. → **Hecho en 0.12.1** (7.3). El fixture K declara ahora
  ∓27,45 y 54,9, así el aviso ya no dirá "declara −17 y el modelo mide −27,4".
- **c. El HSS12X8X1/2 se lee como 203,2 × 203,2.** `RevitModelFacts.GetMemberFacts` lee `STRUCTURAL_SECTION_COMMON_WIDTH` /
  `HEIGHT` (o `b` / `d`, `h`…) del tipo y los dos dieron 203,2. Consecuencias ya vistas: el límite de la excentricidad salió
  **101,6** (medio lado mayor con 203 × 203) en vez de 152,4; la huella del empalme (`tube_footprint_mm [203.2, 203.2]`) hizo que
  `BOLT_TOO_CLOSE_TO_TUBE` y `PLATE_SMALLER_THAN_TUBE` midieran contra un tubo más estrecho de lo real; y la **plancha de 430 ×
  330**: el constructor pone el 430 (`width_mm`) en X del marco de la plancha = perpendicular al plano de la cercha, que es
  donde está el 12" según el sondeo 22, así que **debería** haber salido con el 430 en horizontal; la persona anotó que salió en
  vertical. La captura `fase12-06` no permite decidirlo (se ve la plancha de canto). No se corrige a ciegas (regla: nada de la
  API sin sondeo): el **sondeo 27** lista los parámetros de sección del tipo con sus valores, `Cross-Section Rotation` y la caja
  real del sólido transversal al eje (canto en el plano y ancho perpendicular). Con eso la 12c arregla la lectura (y, si hace
  falta, el giro de la plancha) y el límite pasa a 152,4. **Mientras tanto nada es inseguro**: con los tramos reales (304,8 a lo
  ancho) los pernos quedan a 37,6 mm de la cara del tubo (filas de 2 a ±190) y a 38,4 (filas de 4 a ±140).
- **d. `--puente` 36/38.** Las pruebas 30b y 30c esperan `calculated_values.kind` y `eccentricity_mm` en `validate`, como ya los
  devolvía el simulador (y como dice el contrato); el add-in solo devolvía el marco. → **Hecho en 0.12.1**:
  `ValidationService.CalculatedValues(frame, isSplice)` los añade en `validate` y en `catalog_apply`. Con eso, 38/38 (se
  comprueba en la 12b).
- **e. La `FabricationTransaction` de borrar no se abre justo después de crear.** "cannot start a fabrication transaction while
  asynchronous fabrication tasks are queued for execution" en los 42 borrados (41 en el log, el primero lo absorbe el aviso).
  Es lo mismo que la Fase 9 anotó (`fase-9.md` 7) y la Fase 10 contrastó (`fase-10.md` 7.1): el backend cae a `Document.Delete`
  y restaura las barras igual; sondeos 12 y 13 a cero lo confirman. No se toca.
- **f. Queda un marcador y una etiqueta (N5)** tras borrar el lote: el plan `dd6cf93f` no se descartó (Borrar el lote no descarta
  el plan; el nudo ya conectado conserva su marca gris). No es un fallo; la 12b lo descarta con `batch_plan_discard` al empezar.
- **g. Sin anotar**: el camino de una sola barra y el de media cercha desde Conectar (12-7), el error del borde a 10 junto al
  campo (12-6) y el cuadro Sí/No "empalme + nudo en K" de Conectar (el empalme elegido en el 12-6 era de otra cercha y en la 8c
  el nudo del 12-5 se abrió solo). Ninguno tiene nada en contra en el log; se prueban de paso en la 12b (una barra y el Sí/No).

### 7.3 Qué cambió en el repositorio (ronda 12b, add-in 0.12.1)

- `src/MotorConexiones.Revit/Services/ValidationService.cs`: `CalculatedValues(frame, isSplice)` devuelve `kind` y
  `eccentricity_mm`; `ValidateOperation` guarda el `ResolvedNode` (su `IsSplice`) y `CatalogApplyOperation` pasa
  `result.Node.IsSplice`. Nada más cambia en el código del add-in: **la fabricación sigue siendo la de la 0.12.0** (el corte por
  el eje y las soldaduras DirectShape se cambian en la 12c con lo que digan los sondeos).
- `docs/fixtures/nudo-K-cordon-superior.json`: `eccentricity_mm 54.9`, `axis_offset_mm` ∓27,45 y el texto de `source.drawing`.
  `src/MotorConexiones.Tests/Fakes/KNode55.cs` (antes `KNode34`): 54,9 y los cortes reales; `Fase12Tests` con los números nuevos
  (el límite sigue siendo 152,4 porque el nudo sintético lleva el canto de 304,8; con `node_axis_max_offset_mm: 10` sigue
  fallando: los ejes pasan ahora a unos 19 mm del punto).
- `mcp/pruebas/simulador_revit.py` (54,9, ∓27,45, la diagonal 1249632 desplazada 10,45 mm), `mcp/tools/conn_tools.py` (manual de
  `conn_get_node_info`), `docs/guide.md` (las dos frases del 34), `mcp/CONTRATO-conn.md` (cabecera 0.12.1 y los 54,9).
- Versión **0.12.1** en los dos `.csproj`, `AddinInfo.cs`, el adaptador, las herramientas y el simulador.
- **Sondeos nuevos** (`scripts/sondeos/`): `25-corte-advance-steel.py`, `26-soldadura-weldline.py`, `27-perfil-cordon.py` (7.5).
- Documentación: esta sección, `docs/fases/resumen-fase-12-cierre.md`, nota de "hecha" en `docs/instalacion/fase-12.md`,
  `docs/instalacion/fase-12b.md`, README, `CLAUDE.md`, `docs/propuestas/flujo-intuitivo.md` (fila 12).

### 7.4 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
  Passed!  - Failed: 0, Passed: 268, Skipped: 0, Total: 268
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto (25, 26 y 27 incluidos)
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 82/82 correctas (eccentricity_mm 54.9, axis_offset_mm -27.45 / +27.45)
$ python3 mcp/pruebas/probar_conexiones.py --token-archivo <token del simulador>   → Resultado: 35/35 pruebas correctas (addin=0.12.1; 30b y 30c con kind)
```

Lo que **NO** se puede probar en la nube: `kind` / `eccentricity_mm` en el `validate` **del add-in** (solo compila; lo mide el
`--puente` de la 12b, 38/38) y los tres sondeos (IronPython dentro de Revit).

### 7.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-12b.md`, unos 25 minutos, sobre la copia; no crea conexiones)

| Qué | Para qué |
|---|---|
| `deploy.ps1` 0.12.1.0, `instalar-conn.ps1`, `ping` 0.12.1, `batch_plan_discard` del plan `dd6cf93f` (sondeos 17 y 21 a cero) | Dejar el PC en la 12b y la copia limpia |
| **Sondeo 27** (solo lee): parámetros de sección del tipo HSS12X8X1/2 (y de una diagonal y el cordón central), `Cross-Section Rotation`, justificaciones y la caja real del sólido transversal al eje | Decidir cómo leer ancho y canto (7.2.c) y en qué sentido va la plancha de 430 |
| **Sondeo 26**: `WeldLine(Point3d[], Vector3d, Vector3d)` cerrada sobre la huella de la diagonal con espesor 5, `Connect([viga, plancha], kInShop)`, `WriteToDb`; `WeldPoint`; cancelado. Opcional: repetir con `%TEMP%\sondeo26-commit.txt` para verla y capturar | Si escribe, `CreateWelds` pasa a soldadura nativa en la 12c |
| **Sondeo 25**: la viga de Advance Steel de la diagonal por el puente `SteelElementProperties` (se anota el camino que funcione), `BeamShortening(eEnd, 60)` + `AddFeature` con `AngleOnY` = 90 − 44,4, medidas antes y después, `BeamMultiContourNotch` como segunda vía; cancelado. Opcional: repetir con `%TEMP%\sondeo25-commit.txt` para ver el corte y capturar | Si corta, `DirectWeldBuilder` deja el retiro por el eje y `END_CUT_APPROXIMATED` en la 12c |
| `validate` del fixture K por el puente (`calculated_values.kind: node`, `eccentricity_mm 54.9`, aviso sin "declara"), del C (`kind: splice`) y **`--puente` 38/38** | Cerrar 7.2.b y 7.2.d |
| De paso **(la persona)**: Conectar con **una sola barra** (debe pedir pinchar las demás) y Conectar con el nudo N9 de la 8c completo (cordón, los dos tramos y las dos diagonales: la ventana del empalme y, al cerrarla, el cuadro Sí/No del nudo en K), sin crear nada | Lo que quedó sin ver (7.2.g) |

### 7.6 Decisiones del cierre

1. **La fabricación de la 0.12.0 se queda tal cual hasta la 12c**: funciona en Revit (planchas, patrones de N × 1 conectados,
   retiros, DirectShape para la soldadura) y cambiarla sin los sondeos sería inventar. La 12b es corta: una línea de código útil
   (`kind` / `eccentricity_mm`), el número bueno de la excentricidad y los tres sondeos que faltaban.
2. **No se corrige a ciegas la lectura del perfil** (7.2.c), aunque el síntoma es claro: la regla es compilar o sondear, no
   suponer qué parámetro guarda el 12". El sondeo 27 lo dice en una pasada y la corrección es de una función
   (`RevitModelFacts.GetMemberFacts`).
3. **Los sondeos 25 y 26 cancelan por defecto** y solo hacen `Commit` si existe un archivo marca en `%TEMP%`: así el instalador
   no edita ningún archivo del repositorio y la persona decide si quiere ver el corte o la soldadura (y deshacer con Ctrl+Z o
   cerrar sin guardar).
4. **La tuerca a ambos lados se queda como D9** (`BOLT_NUTS_APPROXIMATED`): el sondeo 23 confirmó que la API no la expone.
5. **`KNode34` pasa a `KNode55`** en vez de conservar el nombre con otro número: el nombre decía la medida y la medida era otra.

### 7.7 Qué sigue (hecho el 2026-10-22: el contraste está en la sección 8)

1. **Ronda 12b en el PC**: `docs/instalacion/fase-12b.md` y devolver `docs/fases/resultados-fase-12b.md` (y las capturas si se
   repitieron los sondeos con `Commit`). **Hecho el 2026-10-21** (commit `2ce9f93`; sección 8).
2. **Ronda 12c (nube)** con lo que digan los sondeos: `RevitModelFacts` con el ancho y el canto bien leídos (y el giro de la
   plancha si hace falta; el límite de la excentricidad a 152,4 en el Hangar), `DirectWeldBuilder` con `BeamShortening` si corta,
   `CreateWelds` con `WeldLine` si escribe (con reserva DirectShape si no), y los avisos `END_CUT_APPROXIMATED` solo cuando toque.
3. Después, la **Fase 13**: cartela automática (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle
   B a 330 (D6).

## 8. Cierre de la ronda 12b (2026-10-22): los sondeos 25, 26 y 27 contrastados y la ronda 12c (add-in 0.12.2)

Resultados: `docs/fases/resultados-fase-12b.md` (instalador, commit `2ce9f93`, add-in **0.12.1**, 2026-10-21, sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`), las capturas `fase12b-03-una-barra`, `fase12b-04-cuadro`, `fase12b-empalme-n9` y
`fase12b-error-borde`, y el log del día. **Los tres sondeos en una línea cada uno**: **27**: el tipo HSS12X8X1/2 no tiene
ningún `STRUCTURAL_SECTION_COMMON_*`; guarda con nombre corto **`Ht` = 304,8**, **`b` = 203,2** y `t` = 11,81, el ejemplar lleva
**Cross-Section Rotation = 90°** y el sólido mide **203,2 en el plano de la cercha y 304,8 perpendicular** (lo que supuso 1.3 y
midió el sondeo 22). **26**: **`WeldLine(Point3d[], Vector3d, Vector3d)` se escribe** (`WriteToDb OK`, `Length` 254 en el
contorno cerrado de 64 × 64, `IsClosed`, espesor y gargantas 5), conectada a la plancha de apoyo con `Connect(…, kInShop)`;
`WeldPoint` también; nada queda tras cancelar. **25**: **la viga de Advance Steel de la diagonal no se encontró** (el puente de
Revit responde: `AddFabricationInformationForRevitElements → 0`, `GetFabricationUniqueID → 19b0eff1-…`; pero en ASObjectsMgd
no hay ningún estático que reciba un `Guid` ni ninguno que devuelva la lista de `ObjectId`), así que **`BeamShortening` no se
intentó**: el corte inclinado sigue sin decidir. **`--puente` 38/38** (30b y 30c pasan con `kind`). Y lo que quedó sin ver en
7.2.g se vio: una sola barra pide pinchar, media cercha manda a Planificar, el cuadro Sí/No del empalme + nudo en K aparece y el
borde a 10 da `BOLT_EDGE_DISTANCE_TOO_SMALL` junto al campo.

### 8.1 Contraste con lo esperado en 7.5

| Qué se esperaba (7.5) | Qué pasó | ¿Coincide? |
|---|---|---|
| `deploy.ps1` 0.12.1.0, `instalar-conn.ps1`, `ping` 0.12.1, `batch_plan_discard` del plan `dd6cf93f` y sondeos 17 y 21 a cero | `== MotorConexiones 0.12.1.0 desplegado en Revit 2027 ==`, 25 rutas y 23 herramientas, DLL `0.12.1.0`, `ping` `addin_version 0.12.1` sobre `HANGAR_PRUEBA_sondeo`, `list` 0; el sondeo 17 ya daba **0 marcadores antes de descartar** (la copia se cerró sin guardar tras la Fase 12 y Revit se reinició: `discarded_plans 0`), 17 y 21 a cero después (0 controles en el lienzo) | **Sí** |
| **Sondeo 27**: parámetros de sección del tipo, `Cross-Section Rotation`, justificaciones y la caja real del sólido | La primera pasada (sin `-SinTransaccion`) cayó con `NameError: name 'uidoc' is not defined` (8.2.a); la segunda, con `-SinTransaccion`: **1245533** `STRUCTURAL_SECTION_SHAPE 0 (Not Defined)`, `COMMON_WIDTH` y `COMMON_HEIGHT` **"(no lo tiene)"**, con nombre corto **`Ht = 304.80`, `b = 203.20`, `t = 11.81`**; `STRUCTURAL_BEND_DIR_ANGLE` = `Cross-Section Rotation` = **90°**, justificaciones Origin / Center, desplazamientos 0; **sólido: canto en el plano 203,2, ancho perpendicular 304,8** (eje −X, en plano −Z, fuera +Y). La diagonal **1249631** (`HSS2-1-2X2-1-2X3-16 64x64`): `COMMON_WIDTH` = `COMMON_HEIGHT` = 2,5" (63,5), giro −180°, sólido 63,5 × 63,5, `END_EXTENSION` 69,2. El cordón central **1249510** (HSS3X3X1/4): `Ht` = `b` = 76,2, `t` 5,92, giro 180°, sólido 76,2 × 76,2 | **Sí**: dice exactamente por qué el add-in leía 203 × 203 (buscaba `d`/`h`/`H`/`Height` y el tipo guarda **`Ht`**; sin canto, lo infería del "8" de la designación) y cómo leerlo (8.3) |
| **Sondeo 26**: `WeldLine` cerrada sobre la huella, `Connect`, `WriteToDb`; `WeldPoint`; cancelado | `WeldLine(Point3d[], Vector3d, Vector3d): True`, propiedades escribibles listadas (`Thickness`, `MainEffectiveThroat`, `DoubleEffectiveThroat`, `IsClosed`, `AssemblyLocation`…), `eAssemblyLocation` con `kInShop`; la plancha de apoyo de 200 × 200 × 9,525 escrita; **`WeldLine creada: DoubleEffectiveThroat=5.0; IsClosed=True; MainEffectiveThroat=5.0; Thickness=5.0`**, **`Connect: OK con 1 pieza(s), kInShop`** (solo la plancha: la barra de Advance Steel no se encontró), **`WriteToDb OK (contorno cerrado de 64 x 64, 5.0 mm); Length=254.0; IsClosed=True`**; `WeldPoint: WriteToDb OK`; 0 elementos nuevos con la transacción abierta y tras cancelar; sondeos 12 y 13 a cero después | **Sí**: la soldadura nativa escribe → `CreateWelds` pasa a `WeldLine` en la 12c (8.3). Queda por ver si se **materializa** al confirmar y si lo hace **sin** una plancha que conectar (8.2.b) |
| **Sondeo 25**: la viga de Advance Steel por el puente `SteelElementProperties`, `BeamShortening(eEnd, 60)` + `AddFeature` con `AngleOnY`, medidas antes y después; `BeamMultiContourNotch` | `BeamShortening(eEnd, Double): si`, `BeamMultiContourNotch(Beam, eEnd, Point3d[], Vector3d, Vector3d): True`, `eEnd` = `kMid, kStart, kEnd, kNoEnd`, propiedades `AngleOnY`, `AngleOnZ`, `CutStraight`, `CutStraightOffset`, `InsLength`, `UseGap`…; `puente: AddFabricationInformationForRevitElements -> 0`, `puente: GetFabricationUniqueID -> 19b0eff1-50ed-42cc-a31c-d2e89d8dfbf3`; **`NO encontrada: sin viga de Advance Steel no hay corte por esta via`** (ningún otro intento impreso: en ASObjectsMgd no hay estáticos con `Guid` ni listas de `ObjectId`); extensión del extremo 69,2 antes y después; 0 elementos nuevos | **No decide**: el corte no se pudo intentar. La fabricación de `direct_weld` sigue con la reserva (retiro por el eje, `END_CUT_APPROXIMATED`) y el **sondeo 28** busca la viga en todos los ensamblados (8.2.b, 8.5) |
| `validate` del fixture K (`kind: node`, 54,9, avisos sin "declara"), del C (`kind: splice`), `--puente` 38/38 | K: `is_valid true`, **`calculated_values.kind: "node"`, `eccentricity_mm: 54.9`**, origen (−9278,1; −17195,7; 19933), los dos `MEMBER_ECCENTRIC` dicen "corta el cordón a −27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta **101.6** mm)" **sin** "El JSON declara" (el fixture ya trae ∓27,45); C: `is_valid true`, **`kind: "splice"`**, `eccentricity_mm 0`, origen (−4180,5; 17204,2; 19933), 12 pernos, agarre 25,4, L 50,8; `node_info` del K: 54,9, ∓27,4 y **`width_mm 203.2` / `height_mm 203.2`** en el cordón (lo esperado en la 0.12.1); **`Resultado: 38/38 pruebas correctas`** (30b `kind=splice`, 30c `MEMBER_ECCENTRIC` × 2); `templates_count 2`; servidor 8000 escuchando; el log del día sin `ribbon_connect_failed`, `ribbon_create_failed`, `advance_steel_*_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (los 41 `fabrication_transaction_failed` son los borrados de la sesión de la Fase 12, 7.2.e) | **Sí**: 7.2.b y 7.2.d cerrados. El límite **101,6** del aviso es el síntoma de 7.2.c, que la 12c cambia a 152,4 |
| **(la persona)** Conectar con una sola barra; Conectar con el nudo N9 completo (la ventana del empalme y el cuadro Sí/No del nudo en K); el borde a 10 | Log: `ribbon_connect_pick` (`already 1249631`, `picked [1249630, 1249510, 1249636]`: pinchó las barras del Detalle D) → `ribbon_connect` `selection 1`, `members 3`, propuesta **"Nudo tipico Detalle D"** `same` 0,0° → cerrada (captura `fase12b-03`: la diagonal sola seleccionada); dos `ribbon_connect_truss` `selection 9, nodes 5` (**media cercha → "parece una cercha" → Planificar lote**, lo pendiente desde la Fase 11); N9: `ribbon_connect` **`kind: splice`** con `chord 1245533`, `connection_type end_plate`, `starter_empty_catalog` (captura `fase12b-empalme-n9`: "Conectar · empalme de los tramos 1245533 y 1245532 · arranque end_plate (Detalle C)", plancha 430 × 330, 12 pernos) → cerrada → **el cuadro Sí/No** (captura `fase12b-04`: "Este punto también tiene un nudo en K con 2 diagonales sobre el cordón 1245533: ¿conectarlo ahora?") → `ribbon_connect` **`kind: node`, `splice_element_id 1245532`**, propuesta **"Nudo K cordon superior"** `same` 0,55° → cerrada; **borde 10** (captura `fase12b-error-borde`): cuatro `BOLT_EDGE_DISTANCE_TOO_SMALL` en `bolts.faces.top.edge_mm` ("queda a 10.0 mm del borde de la plancha (mínimo 25.0 mm…)") **junto al campo en rojo y en la tabla**, barra de estado "Arriba: borde (mm): 25 → 10." (el botón Crear no sale en la captura) | **Sí**: todo lo de 7.2.g, visto |

### 8.2 Lo que no coincidió y qué se hace con ello

- **a. El sondeo 27 cayó a la primera con `NameError: uidoc`.** La instrucción lo lanzaba por `/execute_code/` (sin
  `-SinTransaccion`), donde el espacio de nombres no trae `uidoc`, y el sondeo lee la selección. El instalador lo repitió con
  `-SinTransaccion` y salió entero. → El 27 lleva ahora el `uidoc` protegido (sin selección usa las barras de siempre) y la
  instrucción de la 12c pone `-SinTransaccion` en todos los sondeos que miran la selección.
- **b. La viga de Advance Steel no aparece buscando solo en ASObjectsMgd.** Los sondeos 25 y 26 solo recorrieron ese ensamblado;
  el Guid que da Revit tiene que convertirse en un `ObjectId` por algún tipo de otro ensamblado (`ASCADLinkMgd`, el "link"
  CAD-Revit, estaba en disco y sin cargar en la Fase 1; `DatabaseManager` / `DocumentManager` son los nombres de siempre de la
  API de Advance Steel) o por otro método de `SteelElementProperties`. → **Sondeo 28** (8.5): lista los estáticos de
  `SteelElementProperties`, todos los ensamblados de acero cargados dentro de la `FabricationTransaction` (y carga `ASCADLinkMgd`
  si falta), prueba cualquier estático con `Guid` o `String`, recorre cualquier lista de `ObjectId` abriéndola con
  `FilerObject.GetFilerObject` o `DatabaseManager.Open`, compara `GetUniqueId()` y, si no coincide, la geometría (un `Beam` con
  inicio y fin a menos de 50 mm de los extremos de la barra); con la viga, el `BeamShortening` del 25. **Mientras tanto la
  fabricación de `direct_weld` no cambia** (sigue el retiro por el eje con `END_CUT_APPROXIMATED`; los textos del aviso ya
  apuntan al 28). El mismo sondeo escribe una **`WeldLine` sin plancha** (conectada a la viga si la encuentra; sin `Connect` si
  no) para saber si se materializa: es lo que el add-in 0.12.2 hace en el nudo en K.
- **c. El límite de la excentricidad sigue en 101,6 en la 0.12.1** (esperado: la lectura del perfil era de la 12c). Con la 12c
  (`Ht` + giro) el cordón pasa a 203,2 en el plano × 304,8 perpendicular, el límite a **152,4** (medio lado mayor, 4.3), la huella
  del empalme a **[304,8, 203,2]** y la cara del cordón para `MEMBER_WIDER_THAN_CHORD_FACE` a 304,8. Nada de lo creado en la
  Fase 12 estaba mal por esto (7.2.c): el medio canto en el plano era 101,6 también con la lectura vieja.
- **d. La plancha de 430 no estaba girada.** Con el sondeo 27 (el 12" perpendicular al plano) y el código de `EndPlateBuilder`
  (el marco de la plancha pone X = Y × Z = perpendicular al plano de la cercha, el global ±Y, y el 430 va en X) **el 430 sale
  horizontal**, como debe; la captura `fase12-06` lo confirma al mirarla con calma: la plancha asoma sobre el tubo unos 63 mm
  ((330 − 203)/2, no los 113 que asomarían con el 430 en vertical) y a los lados se ven las columnas de **2** pernos a ±40. Lo
  que la persona vio "en vertical" es el plano de la plancha (perpendicular al eje), que sí es vertical. **No se gira nada**; lo
  que sí estaba mal era la **soldadura** de la plancha al tubo, dibujada sobre una huella de 203 × 203 en vez de 305 × 203: la
  12c la corrige al corregir la lectura. La instalación de la 12c pide una captura de frente para cerrarlo del todo.
- **e. Una sola barra desde Conectar pinchó el Detalle D, no el nudo en K.** La persona pinchó 1249630, 1249510 y 1249636 en vez
  del cordón superior y la otra diagonal; el camino (`PickObject` → previsualización con la mejor plantilla) queda probado igual.
- **f. El marcador de N5 ya no estaba** (7.2.f): la copia se cerró sin guardar tras la Fase 12, así que los `DirectShape` del plan
  se fueron con ella y `batch_plan_discard` del plan viejo devolvió `discarded_plans 0` (el plan vivía en memoria de un Revit
  que ya se había cerrado). Lo esperable; nada que hacer.

### 8.3 Qué cambia en el repositorio (ronda 12c, add-in 0.12.2)

- **Core** (`netstandard2.0`, probado en la nube):
  - `Model/SectionOrientation.cs` (nuevo): `HssDesignation.TryParseMm` (la designación AISC en mm: **canto × ancho × pared**,
    `HSS12X8X1/2` = 304,8 × 203,2 × 12,7; el sufijo métrico "64x64" como reserva) y `SectionOrientation.Oriented(canto, ancho,
    giro)` → (lado en el plano, lado perpendicular): con el giro más cerca de ±90° que de 0° o 180° el canto nominal queda
    perpendicular; vale para cerchas en un plano vertical.
  - `MemberModelFacts`: **`WidthMm` es el lado en el plano de la cercha y `HeightMm` el perpendicular** (documentado; es lo
    que el Core, el simulador y las pruebas ya suponían: 203,2 × 304,8) y `SectionRotationDeg` nuevo.
  - `EndPlateValidator.Footprint`, `EndPlateSketch` y `SpecStarter.ForSplice` toman la huella del tubo de los lados orientados
    (perpendicular a lo ancho de la plancha, en el plano a lo alto) en vez de adivinar con el mayor y el menor; en el Hangar no
    cambia ningún número.
  - `Geometry3D/WeldChain.cs` (nuevo): los segmentos de soldadura encadenados en polilíneas (cerradas si vuelven al principio).
  - `ProfileMatcher.NormalizeDesignation`: de paso, un error de siempre: la segunda sustitución convertía también el "2-1" de
    "2-1/2" y `HSS2-1/2X2-1/2X3/16` quedaba como `HSS2-1/2X2/1/2X3/16`, con lo que `TryParseHssDimensions` nunca leía la
    diagonal (la coincidencia por nombre lo tapaba). Ahora solo convierte la pareja que termina la designación o va seguida de
    `X`. La prueba `ProfilePolicy_WarnWritesTheModelProfile…` cambiaba de perfil solo el nombre (HSS3X3 con medidas de 63,5) y se
    corrige para cambiar también las medidas: con el analizador arreglado el matcher casaba por las medidas reales.
- **Add-in** (compila; se contrasta en el PC):
  - `Node/RevitModelFacts.GetMemberFacts`: lee el ancho nominal (`COMMON_WIDTH`, `b`, `B`, `Width`, `Ancho`) y el canto nominal
    (`COMMON_HEIGHT`, `d`, `h`, `H`, **`Ht`**, `Height`, `Depth`, `Alto`); si faltan, la designación con `HssDesignation`
    (canto primero); lee `STRUCTURAL_BEND_DIR_ANGLE` del ejemplar y orienta con `SectionOrientation`. Para el HSS12X8X1/2
    girado 90°: `width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`.
  - `Fabrication/AdvanceSteelBackend.CreateWelds`: **`WeldLine`** por cadena (polilínea en mm, `xvec`/`yvec` = X e Y del marco,
    `Thickness` = `MainEffectiveThroat` = `DoubleEffectiveThroat` = el filete, `IsClosed`), **`Connect(…, kInShop)`** con las
    planchas escritas en la misma sesión sobre las que cae la soldadura (dentro de su contorno y entre sus caras), `WriteToDb`;
    sin plancha (soldadura directa) se escribe **sin `Connect`**. Al confirmar, si Advance Steel materializa tantos elementos
    menos como soldaduras escritas, se crea la **reserva DirectShape** de las soldaduras con un `REVIT_WARNING`; si no falta
    ninguno, nada; si falta otro número, aviso sin reserva. Cada escritura deja `advance_steel_weld_written` en el log y
    `fabrication_transaction_commit` lleva `pending_welds` y `missing`. Un fallo al escribir una cadena cae a DirectShape con
    aviso (`advance_steel_weld_failed`), como las placas.
  - `Fabrication/DirectWeldBuilder`: el medio canto y la huella de la barra con el lado en el plano (`WidthMm`); los textos de
    `END_CUT_APPROXIMATED` apuntan a los sondeos 22, 25 y 28. **La fabricación no cambia.**
  - `Operations/NodeInfoOperation`: `section_rotation_deg` por barra.
- **MCP** (`mcp/`): versión 0.12.2 en el adaptador, las herramientas y el simulador; el simulador da `section_rotation_deg` (90 en
  los tramos del cordón superior); el manual de `conn_get_node_info` y `CONTRATO-conn.md` dicen qué es `width_mm` y qué
  `height_mm`.
- **Pruebas**: `Fase12cTests.cs` (`SectionOrientationTests`: la designación, el giro, la huella orientada en el validador, el
  croquis y el arranque escalado con un HSS10X6 girado y sin girar, los hechos sintéticos ya con la convención;
  `WeldChainTests`: el contorno cerrado, los dos costados de la ranura, la línea sola, segmentos al revés y los diminutos):
  **291** pruebas.
- **Sondeos**: `28-viga-advance-steel.py` (nuevo, 8.2.b) y el `uidoc` protegido del 27. Instalación: `docs/instalacion/fase-12c.md`.
- Versión **0.12.2** en los dos `.csproj` y `AddinInfo.cs`. Documentación: esta sección, `docs/fases/resumen-fase-12b-cierre.md`,
  la nota de "hecha" en `docs/instalacion/fase-12b.md`, README, `CLAUDE.md`, `docs/guide.md` y las propuestas
  (`flujo-intuitivo.md` fila 12, `placa-de-extremo.md`, `nudos-en-k-excentricos.md`).

### 8.4 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
  Passed!  - Failed: 0, Passed: 291, Skipped: 0, Total: 291
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto (28 incluido)
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 82/82 correctas
$ python3 mcp/pruebas/probar_conexiones.py --token-archivo <token del simulador>   → Resultado: 35/35 pruebas correctas (addin=0.12.2)
```

**NO PROBADO en la nube** (no hay Revit): la lectura nueva de `RevitModelFacts` (solo compila; la 12c la mide con `node_info`:
`width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`, y con el aviso del `validate` del K: "admitida hasta 152.4
mm"), las `WeldLine` del add-in (se ven al crear el nudo en K y el empalme: `advance_steel_weld_written`,
`fabrication_transaction_commit` con `missing 0`, el empalme con **8** elementos en vez de 14 si las dos soldaduras se
materializan) y el sondeo 28.

### 8.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-12c.md`, unos 35 minutos, sobre la copia; crea y borra dos conexiones)

| Qué | Para qué |
|---|---|
| `deploy.ps1` 0.12.2.0, `instalar-conn.ps1`, `ping` 0.12.2, `list` 0 | Dejar el PC en la 12c |
| `node_info` del nudo en K: el cordón con `width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`; las diagonales 63,5 × 63,5; `validate` del fixture K con "admitida hasta **152.4** mm" | Cerrar 7.2.c (la lectura del perfil) |
| **Sondeo 28** (`-SinTransaccion`, cancela al final): los estáticos de `SteelElementProperties`, los ensamblados de acero cargados y sus tipos `DatabaseManager`/`DocumentManager`/`ObjectId`, los intentos del puente y el recuento de tipos recorridos; con la viga, `BeamShortening` y las medidas; la `WeldLine` sin plancha; sondeos 12 y 13 a cero después | Decidir el corte inclinado (si corta: 12d con `BeamShortening`) y si la soldadura sin plancha se materializa |
| **(la persona)** Conectar en el nudo en K → **Crear** → captura de la soldadura; **(instalador)** `list`, `delete` | Las dos `WeldLine` del nudo en K: cuántos elementos crea (2 si se materializan; si no, el aviso "no materializó las 2 soldadura(s)" y la reserva), `advance_steel_weld_written` × 2 con `connect: sin Connect`, `fabrication_transaction_commit` con `missing` |
| **(la persona)** Conectar en el empalme 1245531 + 1245530 → **Crear** → **captura de frente** (mirando por el eje del cordón); **(instalador)** `list`, `delete` | **8 elementos** (2 planchas + 4 patrones + 2 soldaduras) si las `WeldLine` conectadas a las planchas se materializan; `end_plate_built` con `tube_footprint_mm [304.8, 203.2]`; el 430 en horizontal y la soldadura de 305 × 203 sobre la plancha (8.2.d) |
| `--puente` 38/38, `catalog_list` 2, el log del día sin `*_failed` del add-in (salvo `fabrication_transaction_failed` al borrar, lo de siempre) | Cerrar la ronda |

### 8.6 Decisiones del cierre

1. **La convención queda escrita**: `WidthMm` / `width_mm` es el lado del perfil **en el plano de la cercha** y `HeightMm` /
   `height_mm` el **perpendicular**. No se renombran las claves (el contrato solo añade); se documenta en `MemberModelFacts`, el
   manual de `conn_get_node_info`, el contrato y la guía. Era lo que el Core, el simulador y las pruebas ya hacían; solo el
   add-in estaba fuera.
2. **La orientación se decide con `Cross-Section Rotation`, no midiendo el sólido**: la regla es una función pura (canto, ancho,
   giro) probada en el Core, cuesta una lectura de parámetro y el sondeo 27 vio que las dos cosas coinciden. Una cercha en un
   plano horizontal leería los lados al revés; está fuera del alcance (las del Hangar son verticales) y queda anotado.
3. **La plancha de 430 no se gira** (8.2.d): el código ya la pone bien; la 12c solo pide la captura de frente.
4. **`WeldLine` con `Connect` solo a las planchas de la misma sesión** (lo único que el sondeo 26 probó) y **sin `Connect`** en la
   soldadura directa, con la reserva por recuento al confirmar: así el add-in no pierde soldaduras en silencio y la primera
   creación en el PC dice si la soldadura sin plancha se materializa. `kInShop` como ubicación (no está en el contrato; si hace
   falta, `weld.location` sería una clave nueva opcional).
5. **El corte inclinado sigue siendo la reserva** hasta que el sondeo 28 encuentre la viga: no se escribe código contra
   `BeamShortening` sin haberlo ejecutado una vez (regla de no inventar miembros de la API).
6. **El arreglo de `ProfileMatcher` se queda** aunque sea de paso: lo destapó una prueba nueva, la corrección es de una
   expresión regular y la prueba antigua que cambiaba tenía los datos incoherentes.

### 8.7 Qué sigue

1. **Ronda 12c en el PC**: `docs/instalacion/fase-12c.md` y devolver `docs/fases/resultados-fase-12c.md` con las capturas
   `fase12c-01` (la soldadura del nudo en K) y `fase12c-02` (el empalme de frente).
2. **Cierre de la 12c (nube)** con lo que digan: si el sondeo 28 encuentra la viga y `BeamShortening` corta, una ronda **12d**
   pone el corte inclinado en `DirectWeldBuilder` y retira `END_CUT_APPROXIMATED`; si la `WeldLine` sin plancha se materializa,
   se quita la reserva por recuento (o se deja como red); si no se materializa, la soldadura directa vuelve a DirectShape sin
   aviso y la nativa queda para las planchas.
3. Después, la **Fase 13**: cartela automática (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle
   B a 330 (D6).

---

## 9. Cierre de la ronda 12c (2026-10-23): la 0.12.2 contrastada en Revit y la ronda 12d (add-in 0.12.3)

Resultados: `docs/fases/resultados-fase-12c.md` (instalador, commit `49850e3`, add-in **0.12.2**, 2026-10-23, sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`), las capturas `fase12c-01-soldadura-k`, `fase12c-02-empalme-frente` y `fase12c-03-sondeo28`, y el
log del día. **En una línea cada cosa**: **el perfil** del cordón se lee bien (`node_info`: `width_mm 203.2`, `height_mm 304.8`,
`section_rotation_deg 90`; el `validate` del K dice "admitida hasta **152.4** mm"); **las `WeldLine` se materializan** en los dos
tipos (el nudo en K crea **2** elementos `Welds | SteelProxyElement` con `missing 0`, el empalme **8** = 2 planchas + 4 patrones + 2
soldaduras, también con `missing 0`; la reserva por recuento no saltó); **el 430 va en horizontal** (captura de frente,
`end_plate_built` con `tube_footprint_mm [304.8, 203.2]`); **el sondeo 28 no encontró la viga de Advance Steel** (y por un fallo
suyo, 9.2.a, ni siquiera recorrió los objetos del modelo), así que `BeamShortening` sigue sin intentarse; la `WeldLine` sin
plancha del sondeo, con `Commit`, dio 2 elementos `Welds`. **`--puente` 38/38**, `templates_count 2`, ninguna línea `*_failed` del
add-in en el log.

### 9.1 Contraste con lo esperado en 8.5

| Qué se esperaba (8.5) | Qué pasó | ¿Coincide? |
|---|---|---|
| `deploy.ps1` 0.12.2.0, `instalar-conn.ps1`, `ping` 0.12.2, `list` 0 | `0 Advertencia(s)`, `Superado: 291`, `== MotorConexiones 0.12.2.0 desplegado en Revit 2027 ==`, 25 rutas y 23 herramientas, DLL `0.12.2.0`, `ping` `addin_version 0.12.2` sobre `HANGAR_PRUEBA_sondeo`, `list` 0 | **Sí** |
| `node_info` del nudo en K: el cordón 203,2 × 304,8 con giro 90; las diagonales 63,5 × 63,5; `validate` del K con "admitida hasta 152.4 mm" | Cordón **1245533**: `width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`; diagonales 63,5 × 63,5 con `section_rotation_deg -180`; los dos tramos del empalme 203,2 × 304,8 y 90; `validate` del K `is_valid true` con los dos `MEMBER_ECCENTRIC`: "corta el cordón a ∓27.4 mm del punto de trabajo (excentricidad del nudo 54.9 mm admitida hasta **152.4** mm)"; `validate` del C `is_valid true`, `kind: splice`, 12 pernos | **Sí**: 7.2.c cerrado |
| **Sondeo 28**: estáticos de `SteelElementProperties`, ensamblados y tipos, los intentos del puente y el recuento de tipos recorridos; con la viga, `BeamShortening`; la `WeldLine` sin plancha; sondeos 12 y 13 a cero | `SteelElementProperties` con sus 4 estáticos; 15 ensamblados de acero cargados (`ASCADLinkMgd` ya estaba); los tipos `DocumentManager` (9 estáticos: `GetCurrentDatabase`, `GetCurrentDocument`…), `DatabaseManager` (13: `GetModelObjectIds(ObjectId[]&)` y sus variantes con `ClassTypeFilter` y `Database`, `Open(ObjectId, eOpenMode)`, `GetFilerObjectId`…), `CADDocumentManager`, `ObjectId`; **`puente: AddFabricationInformationForRevitElements -> 0`, `GetFabricationUniqueID -> 0c6957d7-…`, `FilerObject.GetFilerObjectByHandle(String) -> None`**; **ninguna línea "tipos recorridos"**; `NO encontrada por ningun camino`; **`WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 sobre la cara del cordon, 5.0 mm); Length=308.6`**, `WeldPoint: WriteToDb OK`; 0 elementos con la transacción abierta y tras cancelar; sondeos 12 (`1249631: inicio 0.0 \| fin 69.2`) y 13 a cero. Con `Commit` (12c-4): **2 elementos nuevos `Welds | SteelProxyElement`** (la `WeldLine` y el `WeldPoint`), la extensión del extremo sigue en 69,2 (no hubo corte); la persona vio el símbolo en nivel Fino, "en la superior sí aparece, en la inferior no, y no tienen cuerpo volumétrico" (captura `fase12c-03`) | **A medias**: la `WeldLine` sin plancha **sí se materializa**; la viga **no se buscó de verdad** (9.2.a): el corte no decide todavía |
| **(la persona)** Conectar en el nudo en K → Crear; `list`, `delete` | "Nudo K cordon superior" propuesto, **`created_elements 2`**, avisos `END_CUT_APPROXIMATED` + **`REVIT_WARNING` × 2**; log: `direct_weld_member` × 2 (`axis_offset_mm ∓27.44`, `face_y_mm -101.6`, `footprint_mm 90.8 / 89.8`), `advance_steel_weld_written` × 2 (`points 2`, `closed false`, `connect: "sin Connect (ninguna plancha de Advance Steel bajo la soldadura)"`), **`fabrication_transaction_commit` con `pending 2`, `pending_welds 2`, `missing 0`, `new_elements` 1322804 y 1322805 `Welds:SteelProxyElement`**; `list` 1, `delete` con las barras restauradas, sondeos a cero (captura `fase12c-01`: el cuadro "Elementos geométricos creados: 2") | **Sí**: las dos soldaduras directas son nativas y la reserva no hizo falta |
| **(la persona)** Conectar en el empalme → Crear → captura de frente; `list`, `delete` | Arranque Detalle C, **`created_elements 8`** (`1322877`–`1322878` `Plates`, `1322879`–`1322882` `Bolts`, `1322883`–`1322884` `Welds`), avisos `BOLT_NUTS_APPROXIMATED` + **`REVIT_WARNING` × 8**; log: `advance_steel_weld_written` × 2 (`points 4`, `closed true`, `size_mm 6`, `connected_to` la plancha de su tramo, `connect: "Connect con 1 pieza(s), kInShop"`), **`end_plate_built` con `plate_mm [430, 330, 12.7]`, `rows 4`, `bolts 12`, `tube_footprint_mm [304.8, 203.2]`**, `fabrication_transaction_commit` con `pending 8`, `pending_welds 2`, `missing 0`; la persona: "los 430 mm van en horizontal, conteniendo el contorno del tubo de 305 × 203 centrado"; captura `fase12c-02`: la plancha octogonal más ancha que alta, los pernos de la fila superior y de la columna derecha, la línea de la soldadura alrededor del tubo | **Sí**: 8 elementos, huella 305 × 203, el 430 en horizontal (8.2.d cerrado) |
| `--puente` 38/38, `catalog_list` 2, el log sin `*_failed` del add-in | **`Resultado: 38/38 pruebas correctas`**; `templates_count 2`; `advance_steel_weld_support` con `weld_line: true`, `connect: true`, `location kInShop` en cada sesión; ninguna línea `advance_steel_weld_failed`, `ribbon_create_failed`, `plan_event_failed` ni `ribbon_batch_window_error`; el `handle batch_create` con `BATCH_NODE_FAILED` del log es la prueba del puente con el token de ceros (esperado); el borrado abrió su `FabricationTransaction` sin fallo (`pending 0`) las dos veces | **Sí** |

### 9.2 Lo que no coincidió y qué se hace con ello

- **a. El sondeo 28 no recorrió los objetos del modelo de Advance Steel.** Sus caminos (b) y (c) solo invocaban **métodos
  estáticos sin parámetros** que devolvieran una colección de `ObjectId`, y el método real de la API, que el propio sondeo listó
  (`DatabaseManager.GetModelObjectIds(ObjectId[]&)`, con las variantes `(ObjectId[]&, ClassTypeFilter)` y `(ObjectId[]&,
  ClassTypeFilter, Database)`), recibe la lista como **parámetro de salida**: el filtro `len(m.GetParameters()) != 0` lo saltó.
  Por eso no hay línea "tipos recorridos" y el "NO encontrada por ningún camino" solo cubre los tres intentos del puente
  (`AddFabricationInformationForRevitElements → 0`, el Guid de `GetFabricationUniqueID` y `GetFilerObjectByHandle(Guid) → None`:
  el Guid de Revit no es el *handle* de Advance Steel). → **Sondeo 29** (`29-viga-objetos-advance-steel.py`, 9.3): llama a
  `GetModelObjectIds` con el parámetro de salida (sin filtro, con un `ClassTypeFilter` que acepte solo `Beam` y con la base de
  datos de `DocumentManager.GetCurrentDatabase()`), abre cada `ObjectId` con `DatabaseManager.Open(ObjectId, eOpenMode)` (o
  `FilerObject.GetFilerObject`), cuenta los tipos, imprime las primeras vigas (tipo, `Handle` y cualquier `*UniqueId*`/`*Guid*`,
  inicio y fin en mm, para ver si Advance Steel usa el mismo origen que Revit) y busca la de la diagonal por el Guid o por la
  geometría; con la viga, el `BeamShortening` + `AddFeature` del 28 y la `WeldLine` conectada. **Es el último intento** (9.6.3).
- **b. `REVIT_WARNING` repetido, uno por elemento de Advance Steel** (2 en el nudo en K, 8 en el empalme), sin texto ni en el
  cuadro de la cinta ni en el log. Viene del `IFailuresPreprocessor` de `OperationScope` ("Advertencia de Revit: …"): Revit
  lanza una advertencia por cada `SteelProxyElement` al confirmar y el add-in la recogía pero no la escribía. En la Fase 12
  (0.12.0) no aparecieron al crear (los dos `REVIT_WARNING` de `resultados-fase-12.md` son los del borrado): o son nuevas con
  `WeldLine`, o las planchas y los pernos ya las daban y el cuadro no las mostraba. No sabemos qué dicen. → La 12d escribe cada
  fallo en el log (`revit_failure`: severidad, texto, elementos, definición, transacción) y el aviso `REVIT_WARNING` /
  `REVIT_ERROR` lleva "[elementos: …]"; la instalación de la 12d crea un nudo en K para leerlas. Hasta leerlas **no se filtran**.
- **c. Las soldaduras nativas "no tienen cuerpo volumétrico"** y solo se ven en nivel de detalle **Fino** (anotación de la persona
  en 12c-4). Es su representación en Revit: un elemento `Welds | SteelProxyElement` se dibuja como símbolo (líneas), no como
  sólido; `created_elements_count` las cuenta y `delete` las borra. → Nada en el código; la guía (sección 7) y el README lo
  dicen para que nadie las dé por perdidas.
- **d. Lo demás de siempre**: el `handle batch_create` con `BATCH_NODE_FAILED` del log es la prueba 31 del puente (token de
  ceros); los `advance_steel_weld_support` repetidos son uno por sesión de fabricación (crear, borrar, el puente). Nada que hacer.

### 9.3 Qué cambia en el repositorio (ronda 12d, add-in 0.12.3)

- **Add-in** (compila; se contrasta en el PC): `Transactions/OperationScope.FailureCollector.PreprocessFailures` escribe cada
  fallo de Revit en el log con `JsonLineLogger` (`event: revit_failure`, `severity`, `text`, `elements` = los ids de
  `GetFailingElementIds()`, `definition` = el Guid de `GetFailureDefinitionId()`, `transaction` = `GetTransactionName()`; cada
  lectura protegida) y añade "[elementos: 1322804, 1322805]" al texto del aviso `REVIT_WARNING` / `REVIT_ERROR`. **Nada más
  cambia en el add-in**: ni la fabricación (las `WeldLine` y la reserva por recuento se quedan como están), ni la lectura del
  perfil, ni el Core (291 pruebas, las mismas).
- **Sondeo 29** (`scripts/sondeos/29-viga-objetos-advance-steel.py`, 9.2.a): dentro de la `FabricationTransaction`, el Guid del
  puente, los gestores (`DocumentManager.GetCurrentDatabase()`, `CADDocumentManager.GetCurrentDwg()`: qué tipo devuelven y sus
  métodos con `ObjectId`), `GetModelObjectIds` por sus tres variantes con el parámetro de salida, el recorrido con `Open` /
  `GetFilerObject`, el recuento de tipos, las 6 primeras vigas con sus identificadores y extremos en mm, la viga de la diagonal por
  Guid o geometría (50 mm), `BeamShortening(eEnd, 60)` + `AddFeature` con `AngleOnY`, la `WeldLine` conectada, elementos nuevos
  antes y después; cancela al final salvo `%TEMP%\sondeo29-commit.txt`. `-SinTransaccion` siempre (lee la selección).
- **MCP** (`mcp/`): versión 0.12.3 en el adaptador, las herramientas y el simulador; sin rutas ni herramientas nuevas.
- Versión **0.12.3** en los dos `.csproj` y `AddinInfo.cs`. Documentación: esta sección, `docs/fases/resumen-fase-12c-cierre.md`,
  la nota de "hecha" en `docs/instalacion/fase-12c.md`, `docs/instalacion/fase-12d.md`, README, `CLAUDE.md`, `docs/guide.md`
  (las soldaduras como símbolos; los `REVIT_WARNING` con elementos) y las propuestas (`flujo-intuitivo.md` fila 12,
  `placa-de-extremo.md`, `nudos-en-k-excentricos.md`).

### 9.4 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
  Passed!  - Failed: 0, Passed: 291, Skipped: 0, Total: 291
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto (29 incluido)
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 82/82 correctas
$ python3 mcp/pruebas/probar_conexiones.py --token-archivo <token del simulador>   → Resultado: 35/35 pruebas correctas (addin=0.12.3)
```

**NO PROBADO en la nube** (no hay Revit): las líneas `revit_failure` del log y el texto "[elementos: …]" del aviso (solo
compilan; la 12d crea un nudo en K para verlas) y el sondeo 29 entero.

### 9.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-12d.md`, unos 20 minutos, sobre la copia; crea y borra una conexión)

| Qué | Para qué |
|---|---|
| `deploy.ps1` 0.12.3.0, `instalar-conn.ps1`, `ping` 0.12.3, `list` 0 | Dejar el PC en la 12d |
| **Sondeo 29** (`-SinTransaccion`, cancela al final): los gestores y sus tipos, `GetModelObjectIds` → N `ObjectId` por variante, "tipos recorridos", las primeras vigas con `Handle`/`*UniqueId*` e inicio y fin en mm frente a los extremos de la barra de Revit, la línea `viga de Advance Steel: …`; con la viga, `ANTES`, `BeamShortening(...) creado`, `DESPUES`, `feature: …`, `BeamShortening: AddFeature OK` o `FALLO`; la `WeldLine`; sondeos 12 y 13 a cero después; opcional con `Commit` para ver el corte (`fase12d-01-corte.png`) | **Decidir el corte inclinado de una vez**: si la viga aparece y `BeamShortening` corta, una 12e lo pone en `DirectWeldBuilder`; si no aparece ni recorriendo todos los objetos, el retiro por el eje queda como definitivo |
| **(la persona)** Conectar en el nudo en K → Crear → anotar el cuadro (los `REVIT_WARNING` ahora con "[elementos: …]"); **(instalador)** `list`, `delete`; sondeos 12 y 13 | Leer en el log las líneas **`revit_failure`** (`severity`, `text`, `elements`): qué advierte Revit por cada soldadura nativa |
| `--puente` 38/38, `catalog_list` 2, el log del día con `revit_failure` y sin `*_failed` del add-in | Cerrar la ronda |

### 9.6 Decisiones del cierre

1. **La lectura del perfil queda cerrada** (7.2.c): `Ht` + `b` + giro, 203,2 en el plano × 304,8 perpendicular, límite 152,4, huella
   305 × 203. La convención (`width_mm` = en el plano, `height_mm` = perpendicular) se queda como está escrita en la 8.6.1.
2. **Las soldaduras nativas se quedan** en los dos tipos, y **la reserva por recuento también**, como red: no saltó en ninguna de
   las dos creaciones ni en el sondeo, no cuesta nada y avisa si algún día Advance Steel deja de materializarlas. No se quita el
   `REVIT_WARNING` que la acompaña porque solo aparece cuando salta.
3. **El corte inclinado: un sondeo más, y es el último.** Merece la pena porque el retiro por el eje no es cosmético (en el nudo
   en K la diagonal se retira 177 mm en vez de los 145 ideales y deja hueco en el lado agudo, que saldría así en los planos de
   taller) y porque el 28 no llegó a mirar donde tenía que mirar. **Si el 29 tampoco encuentra la viga, se cierra el tema**: el
   retiro por el eje con `END_CUT_APPROXIMATED` pasa a definitivo, se anota en `nudos-en-k-excentricos.md` que Revit no expone la
   viga de Advance Steel de sus propias barras, y no se abren más sondeos sobre esto.
4. **Las advertencias de Revit solo se registran** (una línea en el sitio donde ya se recogían). No se filtran del cuadro ni se
   tocan hasta leerlas; si resultan ruido, se filtran en una ronda posterior.
5. **La Fase 13 va después de la 12d pase lo que pase** con el 29; solo si el 29 encuentra la viga y corta se mete antes una
   **12e** con el corte inclinado en `DirectWeldBuilder` (y se retira `END_CUT_APPROXIMATED`).

### 9.7 Qué sigue (hecho el 2026-10-24: el contraste está en la sección 10)

1. **Ronda 12d en el PC**: `docs/instalacion/fase-12d.md` y devolver `docs/fases/resultados-fase-12d.md` (con `fase12d-01-corte.png`
   si hubo corte).
2. **Cierre de la 12d (nube)** con lo que diga: el corte inclinado decidido (12e o definitivo), las advertencias de Revit leídas
   (filtrar o no), informe, README y propuestas al día. El prompt está en `docs/fases/resumen-fase-12c-cierre.md`.
3. Después, la **Fase 13**: cartela automática (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle
   B a 330 (D6).

## 10. Cierre de la ronda 12d (2026-10-24): la 0.12.3 contrastada en Revit y el add-in 0.12.4

Resultados: `docs/fases/resultados-fase-12d.md` (instalador, commit `1ac780c`, add-in **0.12.3**, 2026-10-24, sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`; sin capturas porque no hubo corte) y el log del día. **En una línea cada cosa**: **el sondeo 29 no
encontró la viga de Advance Steel** (`GetModelObjectIds` devolvió **5** `ObjectId`, **ninguno se abrió** y no se vio ninguna viga;
`BeamShortening` no llegó a intentarse) → el retiro por el eje queda como **definitivo**; **las advertencias de Revit** dicen
**"The created elements are only visible in Detail Level: Fine."** (una por soldadura, severidad `Warning`, definición
`73c984d7-da5a-4cd2-b83b-1c2c345d216b`) → se **filtran del aviso** en la 0.12.4; el nudo en K se creó desde Conectar con **2**
elementos y se borró con las barras restauradas; **`--puente` 38/38**, `templates_count 2`, ninguna línea `*_failed` del add-in.

### 10.1 Contraste con lo esperado en 9.5

| Qué se esperaba (9.5) | Qué pasó | ¿Coincide? |
|---|---|---|
| `deploy.ps1` 0.12.3.0, `instalar-conn.ps1`, `ping` 0.12.3, `list` 0 | `0 Advertencia(s)`, `Superado: 291`, `== MotorConexiones 0.12.3.0 desplegado en Revit 2027 ==`, 25 rutas y 23 herramientas, DLL `0.12.3.0`, el sondeo 29 desplegado (36 213 bytes), `ping` `addin_version 0.12.3` sobre `HANGAR_PRUEBA_sondeo`, `list` 0 | **Sí** |
| **Sondeo 29**: los gestores y sus tipos, `GetModelObjectIds` → N `ObjectId` por variante, "tipos recorridos", las primeras vigas, la línea `viga de Advance Steel: …`; con la viga, `BeamShortening`; la `WeldLine`; sondeos 12 y 13 a cero | Corrió entero en 3,2 s: `puente: AddFabricationInformationForRevitElements -> 0`, Guid `dc5e7d5d-…`; los tipos `DatabaseManager`, `DocumentManager`, `CADDocumentManager` y `IClassTypeFilter`; `DocumentManager.GetCurrentDatabase() -> CADLink.Database.Database` (con `GetObjectId(String)`), `CADDocumentManager.GetCurrentDatabase() -> OpenDatabase` (con `getFilerObjectIdFromHandle(String)` y `getObjectIdFromHandle(String)`), `GetCurrentDwg() -> Database`; **`abrir con: DatabaseManager.Open(ObjectId, eOpenMode.kNotOpened)`** (modos `kNotOpened`, `kNormal`, `kToShadow`) y la reserva `FilerObject.GetFilerObject(ObjectId)`; **`GetModelObjectIds(ObjectId[]&) -> 5 ObjectId`**; las variantes con `ClassTypeFilter` y `Database` "no se llama (falta el filtro o la base de datos)"; **`tipos recorridos: (no abierto)=5; vigas vistas: 0`**; **`viga de Advance Steel: NO encontrada`** (1 lista, 0 vigas); `WeldLine: WriteToDb OK (SIN Connect; contorno 91 x 64 …)`; 0 elementos con la transacción abierta y tras cancelar; sondeos 12 (`1249631: inicio 0.0 \| fin 69.2`) y 13 a cero; el paso 12d-4 no se hizo (no hubo corte) | **No aparece la viga**: el sondeo sí recorrió los objetos (lo que el 28 no hizo), pero no pudo abrir ninguno (10.2.a). Con la regla de 9.6.3, **el tema se cierra** |
| **(la persona)** Conectar en el nudo en K → Crear → anotar el cuadro; **(instalador)** `list`, `delete`; sondeos 12 y 13; leer `revit_failure` en el log | Cuadro: "Conexión modelada correctamente… Elementos geométricos creados: 2; Miembros modificados (retiros): 2; Backend: advancesteel; Advertencias: END_CUT_APPROXIMATED; REVIT_WARNING" (sin ventana de error); log: `direct_weld_member` × 2 (`axis_offset_mm ∓27.44`, `setback_ideal_mm 145.28 / 143.68`, `axis_setback_mm 177.73 / 175.43`, `footprint_mm 90.8 / 89.8`), `advance_steel_weld_written` × 2 (`points 2`, sin `Connect`), `fabrication_transaction_commit` con `pending 2`, `pending_welds 2`, `missing 0`, `new_elements` 1322501 y 1322502 `Welds:SteelProxyElement`; **`revit_failure` × 2**: `severity Warning`, **`text "The created elements are only visible in Detail Level: Fine."`**, `elements 1322501` / `1322502`, `definition 73c984d7-da5a-4cd2-b83b-1c2c345d216b`, `transaction "MotorConexiones: Crear Especificación de arranque (Conectar)"`; `ribbon_create` con `warnings [END_CUT_APPROXIMATED, REVIT_WARNING, REVIT_WARNING]`; `list` 1 (`direct_weld`, 2 elementos), `delete` → 2 elementos borrados y 2 barras restauradas; sondeos 12 y 13 a cero | **Sí**: las advertencias se leyeron y son la de "solo visible en nivel Fino", una por soldadura |
| `--puente` 38/38, `catalog_list` 2, el log del día con `revit_failure` y sin `*_failed` del add-in | **`Resultado: 38/38 pruebas correctas`**; `templates_count 2`; el log con el `startup` 0.12.3 (`label_handler true`), las dos `revit_failure` y el `handle batch_create` con `BATCH_NODE_FAILED` de la prueba 31 del puente (token de ceros, esperado); ninguna línea `ribbon_create_failed`, `advance_steel_weld_failed`, `plan_event_failed` ni `ribbon_batch_window_error` | **Sí** |

### 10.2 Lo que no coincidió y qué se hace con ello

- **a. El sondeo 29 listó 5 objetos y no abrió ninguno.** `tipos recorridos: (no abierto)=5` quiere decir que `abrir_objeto`
  devolvió `None` para los cinco `ObjectId`. La causa más probable está en el propio sondeo: elegía el modo de apertura buscando
  uno con "Read" en el nombre y, como `eOpenMode` solo tiene `kNotOpened`, `kNormal` y `kToShadow`, se quedó con el primero,
  **`kNotOpened`**, que seguramente no abre nada; la reserva `FilerObject.GetFilerObject(ObjectId)` tampoco devolvió objeto (o
  lanzó, y el sondeo lo calla). Así que el 29 **no demuestra** que la viga no esté entre esos 5 objetos (podrían ser las tres
  barras convertidas por Advance Steel, u otra cosa). → **Se cierra igual**, por la regla escrita en 9.6.3 ("si el 29 tampoco
  encuentra la viga, se cierra el tema… y no se abren más sondeos"): el retiro por el eje con `END_CUT_APPROXIMATED` es
  **definitivo**, queda anotado en `nudos-en-k-excentricos.md`, y **no se programa la 12e**. Si algún día la persona quiere
  reabrirlo, lo primero es cambiar el modo a `kNormal` en `abrir_objeto` del sondeo 29 (una línea) y, con los objetos abiertos,
  mirar `getFilerObjectIdFromHandle` de `OpenDatabase`; eso queda escrito y nada más.
- **b. La advertencia de Revit es informativa y se repite.** "The created elements are only visible in Detail Level: Fine.": una
  por elemento de Advance Steel al confirmar (2 en el nudo en K; los 8 del empalme de la 12c serían la misma, por las planchas, los
  pernos y las soldaduras). No dice nada que la guía y el README no digan ya desde la 12c, y por repetirse tapa cualquier otro
  `REVIT_WARNING` real. → **0.12.4** (10.3): se filtra del aviso **por el Guid de su definición** (`73c984d7-…`, que no depende del
  idioma de Revit) y solo cuando es `Warning`; queda en el log como `revit_failure` con `filtered: true`. Cualquier otra
  advertencia sigue saliendo como `REVIT_WARNING` con "[elementos: …]" y los errores nunca se filtran.
- **c. El cuadro de la cinta enseña solo los códigos** ("Advertencias: END_CUT_APPROXIMATED; REVIT_WARNING"), no el texto con
  "[elementos: …]" que la 12d-5 pedía copiar: `RibbonCreation` junta los códigos distintos. Es lo previsto desde la Fase 11 (el
  texto va al log y a `conn_*`); con el filtro de (b) el cuadro del nudo en K dirá solo `END_CUT_APPROXIMATED`. Nada que hacer.
- **d. Cosmético**: la transacción del nudo en K se llama "Crear Especificación de arranque (Conectar)" aunque se creó con la
  plantilla "Nudo K cordon superior": la plantilla, guardada desde un arranque, conserva el `source.drawing` del arranque y el
  nombre de la transacción (y del Deshacer de Revit) sale de ahí. Solo afecta a la plantilla del PC de la persona (no está en
  `catalog/`); se anota y no se toca.
- **e.** La cabecera de `resultados-fase-12d.md` dice `2026-10-23T12:53:21` (el instalador creó el archivo con esa fecha) y el log y
  el commit son del 2026-10-24: vale la fecha del log. Nada que hacer.

### 10.3 Qué cambia en el repositorio (add-in 0.12.4)

- **Add-in** (compila; se ve en el PC con la siguiente instalación): `Transactions/OperationScope.FailureCollector` tiene la
  constante `OnlyVisibleInFineDefinition` (`73c984d7-da5a-4cd2-b83b-1c2c345d216b`); cada fallo se escribe en el log como antes y
  además con **`filtered`** (`true` solo para esa definición con severidad `Warning`); el filtrado **no** se añade a los avisos
  (`continue`), así que no sale como `REVIT_WARNING` ni en el cuadro ni en `conn_create` / `conn_batch_create`. **Nada más cambia**:
  ni la fabricación, ni `DirectWeldBuilder` (el retiro por el eje y `END_CUT_APPROXIMATED` se quedan tal cual), ni el Core (291
  pruebas, las mismas).
- **Sin sondeo nuevo y sin instalación propia**: el 29 fue el último sobre la viga (10.2.a) y el filtro se comprueba de paso en la
  instalación de la Fase 13 (crear un nudo en K debe dar solo `END_CUT_APPROXIMATED` y dejar dos `revit_failure` con
  `filtered: true` en el log).
- **MCP** (`mcp/`): versión 0.12.4 en el adaptador, las herramientas y el simulador; `CONTRATO-conn.md` con la nota de la 12d y del
  cierre (la advertencia que deja de llegar). Sin rutas ni herramientas nuevas.
- Versión **0.12.4** en los dos `.csproj` y `AddinInfo.cs`. Documentación: esta sección, `docs/fases/resumen-fase-12d-cierre.md`,
  la nota de "hecha" en `docs/instalacion/fase-12d.md`, README, `CLAUDE.md`, `docs/guide.md` (la advertencia que ya no llega) y las
  propuestas (`nudos-en-k-excentricos.md`: el corte definitivo y la pista del `kNormal`; `flujo-intuitivo.md` fila 12;
  `placa-de-extremo.md`).

### 10.4 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test src/MotorConexiones.Tests -c Release --no-build --nologo
  Passed!  - Failed: 0, Passed: 291, Skipped: 0, Total: 291
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 82/82 correctas
$ python3 mcp/pruebas/probar_conexiones.py --token-archivo <token del simulador>   → Resultado: 35/35 pruebas correctas (addin=0.12.4)
```

**NO PROBADO en la nube** (no hay Revit): el filtro de la advertencia (solo compila; el Guid de la definición es el que el log del
2026-10-24 devolvió dos veces). Se ve en el PC con la instalación de la Fase 13.

### 10.5 Decisiones del cierre

1. **El corte inclinado del nudo en K queda como retiro por el eje, definitivo.** El 29 era el último intento (9.6.3) y no
   encontró la viga; que no la abriera por el modo `kNotOpened` (10.2.a) se deja escrito por honradez, pero no reabre el tema:
   la diagonal se retira 177 mm por su eje en vez de los 145 ideales, con `END_CUT_APPROXIMATED`, y así sale en los planos. No hay
   12e ni más sondeos sobre la viga de Advance Steel.
2. **La advertencia "solo visible en nivel Fino" se filtra del aviso, por el Guid de su definición y solo si es `Warning`**; queda en
   el log con `filtered: true`. Las demás advertencias y todos los errores siguen como estaban. Es un cambio de una condición en
   `OperationScope`, y por eso no merece una ronda propia en el PC: se ve en la instalación de la Fase 13.
3. **La Fase 12 queda cerrada** con la 0.12.4 en el repositorio y la 0.12.3 en el PC. Lo que sigue es la Fase 13.

### 10.6 Qué sigue

1. **Nada para el instalador ahora**: la 0.12.4 se despliega con la instalación de la Fase 13.
2. **Prompt y alcance de la Fase 13** (una sesión corta, sin código): escribir `docs/prompts/fase-13.md` a partir de las
   propuestas y de lo anotado aquí: la **cartela automática** (`outline.mode = "auto"`, fuera de alcance en el encargo v1,
   aplazada en la 11 y la 12) y **partir la diagonal** para fabricar el empalme del Detalle B a 330 (D6 de la Fase 12), con las
   dudas para la persona y lo que cada sondeo debe decidir. El prompt está en `docs/fases/resumen-fase-12d-cierre.md`.
3. Después, la sesión que programe la **Fase 13** con ese prompt, y su instalación (que de paso despliega la 0.12.4 y comprueba el
   filtro de 10.3).
