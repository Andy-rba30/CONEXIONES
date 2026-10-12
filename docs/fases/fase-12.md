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

**Estado: programada y probada en la nube; NO PROBADA en Revit.** Toda la fabricación (el corte contra la cara, las planchas
octogonales, los pernos a través de dos planchas, las filas por cara, la soldadura en el contorno), el retiro real de los
tramos, `node_info` sobre el modelo, Conectar y el lote de punta a punta y los croquis en WPF se prueban en el PC con
`docs/instalacion/fase-12.md` (unos 90 minutos, sobre la copia; los **sondeos 22, 23 y 24 van primero** porque deciden si la
fabricación de la segunda mitad es la buena o la reserva). La última versión desplegada en el PC es la 0.11.0.

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

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-12.md`, unos 90 minutos)

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

## 3. Qué debo mirar yo en Revit cuando el instalador termine

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

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. Dar `docs/instalacion/fase-12.md` al agente instalador en el PC (Revit cerrado al empezar, sobre la copia).
2. Dejar que corra **primero los tres sondeos** (paso 12-3) y mirar su salida antes de seguir: si el 22 corta, el 23 escribe la
   fila 4 × 1 y el 24 escribe una `WeldLine`, la 12b es corta; si no, las reservas ya están y la 12b decide con la tabla de la
   sección 5.
3. Hacer los pasos marcados **(la persona)**: Conectar en el nudo en K (crear, guardar en catálogo), Conectar en el empalme
   (crear), una sola barra y media cercha, Planificar lote con las 64 barras y Crear, con las ocho capturas.
4. Devolver `docs/fases/resultados-fase-12.md` y las capturas; con eso se escribe el cierre de la fase (sección 7) y la ronda
   12b con lo que los sondeos digan (D12).
