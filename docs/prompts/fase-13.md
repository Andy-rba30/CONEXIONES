# Fase 13: cartela automática (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle B a 330

Fase que sale de tres sitios: el encargo (`docs/ENCARGO_MOTOR_CONEXIONES.md`, sección 7: "`auto` queda para v2, pero el esquema ya
lo reserva"; sección 14: fuera de alcance en v1), la decisión **D6** de la Fase 12 (`docs/prompts/fase-12.md`, 2.2: "la diagonal es
una sola barra: se dibuja y se anota ahora, **se parte en la Fase 13**; distancia 330") y el cierre de la ronda 12d
(`docs/fases/fase-12.md`, 10.5 y 10.6: la Fase 12 cerrada, lo que sigue es esta). La cartela automática venía aplazándose desde la
Fase 11 (`docs/fases/fase-11.md`, 7.2) y la propuesta de nudos en K (`docs/propuestas/nudos-en-k-excentricos.md`, apartado 6) la
dejó para aquí. Se ejecuta en **una sesión** con el prompt de la sección 1 y una ronda del instalador; lo demás de este archivo es
el alcance que esa sesión debe cumplir.

**Estado: dudas abiertas** (sección 9; la persona responde en el chat y las respuestas se copian en negrita al final de cada una
y en la tabla 2.2). **Sin código hasta entonces.** Escrito el 2026-10-24 en la sesión del cierre de la 12d.

**Qué hay hoy y qué no** (para no repetir lo hecho):

- Un nudo con cartela **con plantilla** (los 16 del Detalle D de la cercha de la 8c) se crea desde Conectar y desde el lote con el
  polígono fijo de la plantilla (`gusset.outline.mode = "polygon"`, `points_mm` en el marco canónico). El casado admite hasta
  `angle_tolerance_deg` = **10°** (`CatalogConfig`; editable en `config/catalog.json`); en la cercha del Hangar los 16 casan a 0,0°.
- Un nudo con cartela **sin plantilla**: Conectar escribe el **arranque** de `SpecStarter.FromNode`: una **cartela rectangular**
  que envuelve los extremos ranurados de las barras y el cordón (`gusset_margin_mm` y `gusset_beyond_chord_mm` = 40 en
  `config/choices.json`, redondeo a 5 mm) con el aviso literal "Es un punto de partida, no la cartela automática". **El plan no
  arranca cartelas** (`fase-12.md` 4.6): un nudo con barras a los dos lados del cordón y sin plantilla queda `no_match`.
- El **esquema ya admite `"auto"`** en `outline.mode` (`JsonSchemaValidator`), pero nadie lo calcula: el validador solo mira
  `points_mm` cuando existen (`SpecValidator`, regla 9: `OUTLINE_INVALID`, `PLATE_OUTSIDE_GUSSET`) y `AdvanceSteelBackend.CreatePlate`
  recibe los puntos ya resueltos.
- El **nudo en K** del cordón superior (`direct_weld`) lleva por diagonal `splice_hint` (`detail: "B"`,
  `offset_from_chord_face_mm: 330`, `MemberSpec.SpliceHint`), que el croquis dibuja como marca, el panel edita ("Empalme Detalle B a
  [330] mm") y el Encargo para IA nombra. **Nada se fabrica sobre la barra continua.** La diagonal 1249631 del Hangar va de un
  golpe del cordón central al superior (unos 3 500 mm; sondeo 29, línea 0).
- Las barras solo se **acortan**, nunca se parten: `MemberModifier.ApplySetback` escribe `START_EXTENSION` / `END_EXTENSION` y
  guarda `ModifiedMemberRecord` (elemento, extremo, parámetro, valor original) para que `conn_delete` y Borrar restauren. El
  corte inclinado del nudo en K quedó como retiro por el eje, **definitivo**, con `END_CUT_APPROXIMATED` (`fase-12.md` 10.5):
  **esta fase no lo toca**.

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/prompts/fase-13.md completo (sus secciones 2 y 9 fijan las decisiones), docs/prompts/fase-12.md (secciones
2.2 y 3.6), docs/fases/fase-12.md (secciones 4.6, 10.3 y 10.5), docs/fases/fase-11.md (sección 7.2) y
docs/propuestas/catalogo-y-lotes.md (sección 2, puntos 2 y 3). Ejecuta solo la Fase 13 con el add-in 0.13.0 tal como la describe
docs/prompts/fase-13.md: (1) la cartela automática: outline.mode = "auto" calculado en el Core (AutoOutline) a partir de los
extremos reales de las barras (retiro, ranura o placa cuchilla, ancho) y del cordón, con los márgenes de choices.json, como
envolvente convexa redondeada a 5 mm y con un tope de vértices; los puntos calculados vuelven en calculated_values.outline_points_mm
de validate, preview y catalog_apply, el croquis los dibuja, el panel ofrece Contorno: Automático / Polígono y Convertir en
polígono, el arranque de Conectar sin plantilla usa auto en vez del rectángulo, una plantilla guardada con auto casa con cualquier
ángulo dentro de su tolerancia y el plan arranca los nudos con cartela sin plantilla según la decisión D2; (2) partir la diagonal:
la operación conn_split_member (y su botón en la cinta y su paso en Conectar sobre un nudo en K con splice_hint), que parte una
barra de armazón en dos elementos de Revit a la distancia del Detalle B medida desde la cara del cordón a lo largo de su eje,
conserva el id original en el tramo del cordón, guarda en Extensible Storage lo necesario para deshacer (conn_unsplit y Borrar),
y después el empalme end_plate del Detalle B sobre los dos tramos y el nudo en K sobre el tramo corto, en ese orden, en Conectar
(cadena de una sola pulsación según D9) y en el plan (el corte como paso previo según D5); el mecanismo de Revit para partir lo
decide el sondeo 30 antes de programar la fabricación, con la reserva escrita en 3.4. Respeta el orden de recorte de la sección 10
si no cabe. Ninguna ventana en las rutas de la IA; el contrato solo añade; el camino polygon no cambia (mismos tokens que hoy);
versión 0.13.0 en todo. Termina con compilación sin avisos, pruebas en verde (Core y simulador), docs/fases/fase-13.md,
docs/instalacion/fase-13.md con el sondeo 30 primero y la comprobación del filtro de la advertencia de Revit de la 0.12.4
(fase-12.md 10.3), README, docs/guide.md, mcp/CONTRATO-conn.md, CLAUDE.md (estructura), las propuestas al día, commit, push y un
resumen corto guardado también en docs/fases/resumen-fase-13.md.
```

## 2. Decisiones

### 2.1 Decisiones ya tomadas en fases anteriores (no se reabren)

| Decisión | Dónde | Qué hace la Fase 13 con ella |
|---|---|---|
| `outline.mode = "auto"` existe en el esquema desde v1 y se calcula en v2 | Encargo 7 y 14 | Es (1) de esta fase: se calcula en el Core, nunca en el add-in |
| **D6**: la diagonal es una sola barra; se parte en la Fase 13; distancia **330** de la cara del cordón a las planchas del Detalle B | `docs/prompts/fase-12.md` 2.2 y 3.6 | Es (2) de esta fase; el 330 sigue en `choices.json` (`diagonal_splice_offset_mm`) y en `splice_hint` |
| El Detalle B es un empalme `end_plate` (plancha octogonal 185 × 185 de 3/8", 4 pernos Ø5/8", uno por cara) | `docs/fixtures/detalle-B.json`, P4 de la propuesta | El empalme de la diagonal partida es un nudo de clase `splice` con el arranque `end_plate` **por perfil** (HSS2-1/2X2-1/2X3/16 → B): no hace falta tipo nuevo |
| El plan no arranca cartelas rectangulares "a ojo" | `fase-12.md` 4.6 | Con la cartela automática deja de ser "a ojo": la decisión **D2** dice si el plan arranca cartelas |
| El corte inclinado del nudo en K queda como retiro por el eje, definitivo; no más sondeos sobre la viga de Advance Steel | `fase-12.md` 10.5 | El tramo corto de la diagonal se retira por el eje como hoy (177 mm en vez de 145) y `END_CUT_APPROXIMATED` se queda. **Nada que ver con esta fase** |
| El punto de trabajo y la excentricidad (opción A); cada barra medida sobre su propio eje (`axis_offset_mm`) | Propuesta de nudos en K, P1 y P2 | El contorno automático envuelve los **extremos reales** (con el desvío de eje), como pedía la propuesta en su apartado 4.3 |
| Una operación = un `TransactionGroup`; en el lote, cada nudo anidado en el grupo exterior | `CLAUDE.md` | Partir la barra es una operación propia; la cadena de Conectar (D9) es un mini-lote con el mismo mecanismo de `BatchCreator` |
| La 0.12.4 (filtro de la advertencia "solo visible en nivel Fino") no se ha visto en Revit | `fase-12.md` 10.3 | La instalación de esta fase despliega la 0.13.0 (que la incluye) y lo comprueba en el nudo en K (sección 6) |

### 2.2 Decisiones de la persona (respuestas a la sección 9)

| Pregunta (sección 9) | Decisión | Qué hace la Fase 13 con ella |
|---|---|---|
| D1. Forma del contorno automático | *(pendiente)* | |
| D2. El plan con cartelas sin plantilla | *(pendiente)* | |
| D3. Plantillas en modo `auto` y su tolerancia | *(pendiente)* | |
| D4. Márgenes de la cartela automática | *(pendiente)* | |
| D5. Partir como operación propia y paso previo del plan | *(pendiente)* | |
| D6. Qué tramo conserva el id original | *(pendiente)* | |
| D7. Dónde se mide el 330 | *(pendiente)* | |
| D8. Deshacer el corte | *(pendiente)* | |
| D9. Conectar sobre un nudo en K: cadena de una pulsación | *(pendiente)* | |
| D10. Otras cerchas y el cordón inferior | *(pendiente)* | |
| D11. Alcance y rondas | *(pendiente)* | |

## 3. Alcance (lo que se entrega)

### 3.1 Flujo completo (lo que cambia para la persona)

1. **Conectar sobre un nudo con cartela sin plantilla** (o con el catálogo vacío): la previsualización enseña una **cartela
   calculada** que sigue las barras (chaflanes donde la diagonal lo pide, como el Detalle D), no el rectángulo de la Fase 11. La
   cabecera dice "Especificación de arranque con **cartela automática** (…)". El panel, sección Cartela, tiene un desplegable
   **Contorno: Automático / Polígono**; en Automático las cotas del contorno se ven pero no se editan; **Convertir en polígono**
   copia los puntos calculados a `points_mm` y cambia el modo, y desde ahí se editan como hoy (doble clic en las cotas). Crear
   fabrica la cartela con los puntos calculados.
2. **Guardar como plantilla** una especificación con `auto` guarda el modo `auto`: al aplicarla en otro nudo, la cartela se
   recalcula con los ángulos y los perfiles de ese nudo (es "la pieza que haría la plantilla independiente de los ángulos",
   `catalogo-y-lotes.md` 2.3). El Detalle D sigue en `polygon` (es el dibujo del plano) y nada le cambia.
3. **Planificar lote**: según **D2**, los nudos con cartela sin plantilla que encaje reciben un arranque con cartela automática y
   salen "Listo (arranque)" contados aparte en la cabecera ("… N cartelas automáticas (arranque) …"), o siguen `no_match` como hoy.
4. **Conectar sobre un nudo en K con `splice_hint`** (los 15 del cordón superior): además de la previsualización de siempre, una
   casilla **"Partir la diagonal y crear el empalme del Detalle B a 330"** (marcada por defecto cuando hay `splice_hint`, D9). Con
   ella, Crear hace tres cosas en orden dentro de un grupo: parte cada diagonal, crea el empalme `end_plate` del Detalle B sobre
   los dos tramos y crea el nudo en K sobre el tramo corto. El cuadro final dice las tres. Sin la casilla, como hoy.
5. **Un botón nuevo en la cinta, Partir barra** (y la herramienta `conn_split_member`): seleccionas una barra y el cordón al que
   llega, dices la distancia (330 por defecto) y la parte; sirve para prepararla antes del lote. **Borrar** sobre una conexión que
   partió barras deshace también el corte (D8), y `conn_unsplit` / el mismo botón deshacen un corte suelto mientras ningún nudo use
   los tramos.
6. **Planificar lote sobre los nudos en K**: según **D5**, el plan muestra el corte como **paso previo** de cada nudo en K con
   `splice_hint` ("N5: partir 1249631 y 1249632 a 330, después empalme B y nudo en K"), Crear lo ejecuta en ese orden
   (`BatchRunner.OrderForCreation`: corte → empalme B → nudo en K; el empalme del cordón `N9E` sigue antes que `N9`) y Borrar el
   lote deshace los tres. Replanificar después de partir reconoce los dos tramos (el empalme B aparece como nudo `splice` de la
   diagonal, el nudo del cordón central ve el tramo largo).

### 3.2 Cartela automática (Core: `Geometry2D/AutoOutline.cs`; se prueba en la nube)

- **Entrada**: lo que ya tiene `SpecStarter.FromNode` y el validador: por barra, su dirección unitaria en el marco canónico
  (`ConnectionGeometry.GetMemberDirection2D`), su `axis_offset_mm`, su ancho (`MemberModelFacts.WidthMm`, el lado en el plano de la
  cercha), `end_setback_mm`, y según el `attachment`: `slot_length_mm` (ranura) o la placa cuchilla (`length_mm`, `width_mm`,
  `insertion_mm`) y sus pernos; el cordón: su ancho en el plano y `chord_interface`.
- **Cálculo** (en este orden, determinista):
  1. **Rectángulo de cada barra**: desde `end_setback_mm` hasta `end_setback_mm + slot_length_mm + margin_mm` a lo largo de su eje
     real (con el desvío), y `WidthMm / 2 + side_margin_mm` a cada lado; con placa cuchilla, hasta el extremo libre de la placa más
     `margin_mm` y el ancho de la placa más `side_margin_mm` (los pernos ya exigen su borde en la placa, no en la cartela).
  2. **Franja del cordón**: en X desde el mínimo al máximo de los rectángulos más `margin_mm`; en Y, `± (ChordWidthMm / 2 +
     beyond_chord_mm)` para `through_slot` (la cartela atraviesa el cordón ranurado); con `split_top_bottom` y `side_lap` la
     franja se queda en el lado de las barras (**D1** lo confirma).
  3. **Envolvente convexa** de todos los vértices (`Polygon2D.ConvexHull`, nuevo), **redondeo a 5 mm** hacia fuera y
     **simplificación**: se quitan los vértices con giro menor de 3° y se limita a **12 vértices** (si hay más, se sustituyen los
     chaflanes más cortos por su esquina). El Detalle D tiene 8.
  4. El resultado son `points_mm` en el marco canónico, en el sentido de siempre (antihorario), con `width_mm` y `height_mm` = la
     caja que lo envuelve.
- **Contrato** (solo añade): `gusset.outline.mode: "auto"` con `points_mm` ausente o `null`, y las claves opcionales
  `gusset.outline.margin_mm`, `side_margin_mm` y `beyond_chord_mm` (sin ellas, los valores de `choices.json`, **D4**). Con `auto`
  **y** `points_mm` presentes → `SCHEMA_INVALID` ("auto no lleva puntos: usa polygon"). `width_mm` y `height_mm` en `auto` son
  informativos: el validador los recalcula y avisa `LABEL_VALUE_MISMATCH` si difieren más de 1 mm (o se admiten `null` sin pasar
  por `uncertain_fields`; **D1**).
- **Validación**: con `auto`, `SpecValidator` calcula los puntos **antes** de la regla 9 y aplica las mismas comprobaciones que a un
  polígono (`OUTLINE_INVALID` no debería salir nunca: se cubre con prueba; `PLATE_OUTSIDE_GUSSET`, `BOLT_OUTSIDE_PLATE` y
  `CLASH_WITH_FOREIGN_MEMBER` como hoy). Aviso nuevo **`OUTLINE_AUTO_LARGE`** si la caja supera `max_auto_gusset_mm` de
  `limits.json` (propuesta 1 200) o el área es más del doble de la suma de los rectángulos: señal de que una barra está muy lejos
  o un margen es absurdo. Los puntos calculados van en `calculated_values.outline_points_mm` (`ValidationService`,
  `ValidateOperation`, `CatalogApplyOperation` y el simulador), junto con `outline_width_mm` y `outline_height_mm`.
- **Croquis** (`GussetNodeSketch`): dibuja los puntos calculados igual que un polígono y rotula "cartela automática" junto a las
  cotas; `TemplateSketch` (miniaturas) también.
- **Panel y editor** (`SpecEditor`, `PanelChoices`, `UI/SpecPanel`): el desplegable Contorno; `SpecEditor.SetOutlineMode("auto" |
  "polygon")` y `ConvertAutoToPolygon(points)`; en `auto`, los campos de puntos no son editables y `TrySetLabeledValue` sobre ellos
  devuelve un error claro. El JSON sigue siendo la única fuente de verdad (regla de la Fase 11).
- **Arranque y plantillas**: `SpecStarter.FromNode` escribe `mode: "auto"` (el rectángulo desaparece; `GussetBoundsMm` pasa a ser la
  caja del contorno calculado). `TemplateBuilder` conserva `auto`; `TemplateInstantiator` no toca los puntos (no hay);
  `TemplateMatcher` admite para una plantilla `auto` la tolerancia de **D3** (`angle_tolerance_deg` propia, por defecto la del
  catálogo). `PlanBuilder` según **D2**.
- **Fabricación**: `GussetNodeType` resuelve los puntos antes de llamar a `CreatePlate`; el backend no cambia. La placa de Advance
  Steel ya acepta polígonos de 8 vértices (Detalle D): con 12 no hace falta sondeo.

### 3.3 Partir la diagonal (add-in: `Operations/SplitMemberOperation.cs`, `Fabrication/MemberSplitter.cs`; Core: `Geometry3D/SplitPoint.cs`, `Storage/SplitRecord.cs`)

- **Dónde se corta** (Core, `SplitPoint.Compute`, se prueba en la nube): la diagonal llega al cordón con el ángulo θ en el plano
  de la cercha; su eje cruza la **cara** del cordón a `(ChordWidthMm / 2) / |sen θ|` de su corte con el eje del cordón (medido desde
  el corte propio de la barra, con el desvío de eje). Desde ese punto, a lo largo del eje de la diagonal, **`offset_from_chord_face_mm`**
  (330) hasta la **primera plancha** del Detalle B (D7); el punto de corte de los dos elementos de Revit queda a `330 + espesor +
  gap / 2` (el centro de la pila de dos planchas), de modo que el arranque `SpecStarter.ForSplice` ya existente retire cada tramo lo
  justo (espesor + media holgura menos el hueco que deja el corte). En el nudo en K del Hangar: θ = 44,4°, cara a 101,6 mm →
  145,2 mm por el eje; corte a unos 145 + 330 + 9,5 ≈ **485 mm** del corte con el eje del cordón. Se devuelve en mm y en el marco
  global (`conn_get_node_info` lo añade por diagonal con `splice_hint`: `split_point_mm`).
- **Cómo se parte en Revit** (lo decide el **sondeo 30**, sección 6; la fabricación se programa con lo que diga): sobre la copia
  del modelo, con la diagonal 1249631: (a) **copiar el elemento** (`ElementTransformUtils.CopyElement`) y asignar a cada uno su
  tramo con `LocationCurve.Curve` (conserva tipo, nivel, giro de sección, justificación y desfases); o (b) crear el tramo nuevo con
  `Document.Create.NewFamilyInstance(curva, símbolo, nivel, StructuralType.Beam)` y copiar a mano los parámetros de instancia
  (`STRUCTURAL_BEND_DIR_ANGLE`, justificaciones, desfases). El sondeo prueba (a) y, si no conserva los parámetros, (b); mide los
  dos tramos, comprueba que `START_EXTENSION` / `END_EXTENSION` del original siguen siendo los de antes en el extremo que no se tocó
  y valen 0 en el corte, que las uniones automáticas en el corte están **desactivadas** (`StructuralFramingUtils.DisallowJoinAtEnd`,
  si existe con ese nombre; si no, lo que el sondeo liste), que `node_info` sobre los dos tramos da un nudo `splice` colineal con el
  hueco esperado y que `node_info` del nudo en K sigue viendo el tramo corto con la misma excentricidad; y deshace (borrar el
  nuevo, restaurar la curva). **Ningún miembro de la API se da por bueno hasta que el sondeo lo liste.**
- **Qué tramo conserva el id** (**D6**): el tramo **del lado del cordón** (el corto, donde vive el nudo en K y el Detalle B)
  conserva el elemento original; el tramo largo es el elemento nuevo. Así el nudo en K escrito antes de partir sigue valiendo; el
  nudo del otro extremo (cordón central, Detalle D) debe planificarse **después** de partir (Replanificar) o el plan lo hace solo
  (3.1.6).
- **Registro y deshacer** (**D8**): `SplitRecord` en Extensible Storage (barra original, elemento nuevo, curva original de la
  barra, distancia, cordón, fecha, `connection_id` de quien lo pidió o `null` si fue el botón). `conn_unsplit` / Borrar: borra el
  elemento nuevo y devuelve la curva original, **solo si ningún registro de conexión usa los dos tramos** (si los usa, primero
  borrar esas conexiones; el error `SPLIT_IN_USE` lo dice con los ids). `conn_delete` de una conexión creada por la cadena (3.1.4)
  o por el lote deshace primero sus conexiones y después el corte, en orden inverso al de creación.
- **Herramienta y rutas**: `conn_split_member` (`/conn/split/`: `element_id`, `chord_element_id`, `offset_from_chord_face_mm`
  opcional, `validation_token` **no** exigido porque no es una especificación, pero sí `dry_run` para devolver el punto sin cortar) y
  `conn_unsplit` (`/conn/unsplit/`: `element_id` o `split_id`). 27 rutas y 25 herramientas. En el simulador, `split` crea un
  elemento ficticio nuevo con la misma sección y `node_info` lo ve.
- **Conectar** (**D9**): la casilla de 3.1.4; con ella, Crear corre un `BatchRunner` de tres pasos dentro del grupo exterior
  (`BatchCreator`, como el lote): corte, empalme B (arranque `end_plate` por perfil del tramo; plantilla si hay una que case),
  nudo en K (lo que había en la previsualización, con los mismos ids porque el tramo corto conserva el id). Si un paso falla, se
  revierte **todo** (D9 pregunta si prefieres revertir solo el paso que falló).
- **Plan** (**D5**): `PlanBuilder` añade a cada nudo en K con `splice_hint` un paso `split` por diagonal (no es un nudo: no lleva
  etiqueta ni marca; en la tabla sale como línea indentada "Partir 1249631 a 330 (Detalle B)") y, tras él, un nudo `splice`
  **previsto** de la diagonal (`N5B`, con el arranque B) que se valida con los extremos calculados antes de partir; `OrderForCreation`:
  cortes → empalmes B → `N9E` → `N9`. Borrar el lote deshace los cortes al final.

### 3.4 Reservas si el sondeo 30 no da lo esperado

| Qué | Si funciona | Si falla (la reserva que ya está en 0.13.0) |
|---|---|---|
| Copiar el elemento y cambiar las curvas (a) | `MemberSplitter` lo usa | (b) crear con `NewFamilyInstance` y copiar los parámetros que el sondeo liste |
| Ni (a) ni (b) conservan el giro de sección o la justificación | Se queda | Se escriben a mano desde los valores leídos del original (`RevitModelFacts` ya lee `STRUCTURAL_BEND_DIR_ANGLE`); si tampoco, aviso `SPLIT_PARAMETERS_APPROXIMATED` con la lista |
| `node_info` ve un `splice` entre los dos tramos | El empalme B es un nudo `splice` normal | El detector no cambia; el corte queda como operación suelta y el empalme B se crea con `conn_create` sobre los dos ids con `kind: "splice"` explícito |
| Deshacer (borrar el nuevo y restaurar la curva) deja la barra como estaba (longitud y extensiones) | `conn_unsplit` | Si Revit no restaura la curva, se guarda además el valor de las extensiones y se restauran; si tampoco, `SPLIT_UNDO_PARTIAL` con lo que quedó y la barra se deja partida (nunca se borra la original) |
| Advance Steel ve los dos tramos como vigas distintas al crear el empalme B (planchas y pernos sobre el tramo nuevo) | Como un empalme más | Si la soldadura o la plancha no se puede conectar al tramo nuevo, reserva DirectShape como siempre (`CATEGORY_FALLBACK`) |

### 3.5 Herramientas, rutas, configuración, guía y documentación

- Herramientas: `conn_split_member` y `conn_unsplit` nuevas; `conn_validate`, `conn_preview` y `conn_catalog_apply` devuelven
  `calculated_values.outline_points_mm`; `conn_get_node_info` suma `split_point_mm` por diagonal con `splice_hint`;
  `conn_batch_plan` suma los pasos `split` y los nudos `N…B`; `conn_batch_create` y `conn_batch_delete` los ejecutan y deshacen.
- `config/choices.json`: `starter.gusset_side_margin_mm` (propuesta 30, D4) junto a `gusset_margin_mm` y `gusset_beyond_chord_mm`;
  `starter.outline_mode: "auto"`. `config/limits.json`: `max_auto_gusset_mm` (1 200) y `auto_outline_max_vertices` (12).
- `docs/guide.md`: cómo pedir `auto` (la IA no escribe puntos; si el plano trae la cartela acotada, `polygon` como hasta ahora),
  cómo leer `outline_points_mm`, y la cadena del nudo en K (partir → empalme B → nudo). `mcp/CONTRATO-conn.md`: claves nuevas,
  rutas y códigos (`OUTLINE_AUTO_LARGE`, `SPLIT_IN_USE`, `SPLIT_PARAMETERS_APPROXIMATED`, `SPLIT_UNDO_PARTIAL`, `SPLIT_FAILED`).
- README (estado, garantías, árbol, sección de Conectar con la casilla y el botón Partir barra, errores), `CLAUDE.md` (estructura:
  `AutoOutline`, `SplitPoint`, `SplitRecord`, `MemberSplitter`, `SplitMemberOperation`), las propuestas (`catalogo-y-lotes.md` 2.3
  resuelto; `flujo-intuitivo.md` fila 13; `nudos-en-k-excentricos.md` 4.3).

### 3.6 Arquitectura (solo lo de esta fase)

- Core: `Geometry2D/AutoOutline.cs` (el cálculo), `Polygon2D.ConvexHull` y `Simplify`, `Contract/GussetOutline` (claves opcionales),
  `Validation/SpecValidator` (resolver `auto` antes de la regla 9; `OUTLINE_AUTO_LARGE`), `Editing/SpecEditor` y `SpecStarter`,
  `Catalog/TemplateMatcher` (tolerancia de las plantillas `auto`), `Sketch/GussetNodeSketch`, `Geometry3D/SplitPoint.cs`,
  `Storage/SplitRecord.cs`, `Batch/PlanBuilder` y `BatchRunner` (pasos `split`, nudos `N…B`, orden), `Batch/NodeDetector` (nada: el
  `splice` colineal ya existe).
- Add-in: `Operations/SplitMemberOperation.cs` y `UnsplitOperation.cs`, `Fabrication/MemberSplitter.cs`, `Storage` del `SplitRecord`
  (mismo esquema de Extensible Storage con un campo nuevo, versión del esquema +1 y lectura de los registros antiguos sin cambios),
  `Bridge.Handle` con las dos operaciones, `ConnectCommand` (la casilla y la cadena), `UI/SpecPanel` (Contorno), un botón en la
  cinta (Partir barra, con el único diálogo permitido: la distancia), `BatchCreator` (los pasos `split` y su deshacer).
- MCP: `conexiones.py` (2 rutas), `conn_tools.py` (2 herramientas), simulador (`split`, `unsplit`, `outline_points_mm`), pruebas.

## 4. Reglas técnicas de esta fase

- Las de `CLAUDE.md`, todas. En particular: una operación = un `TransactionGroup` (el corte suelto es uno; la cadena de Conectar
  y el lote anidan como `BatchCreator`); ninguna ventana en las rutas de la IA; `ElementId.Value`; conversión de unidades solo
  en `UnitConverter`; el JSON como única fuente de verdad del panel.
- **El camino `polygon` no cambia**: el Detalle D, los 16 nudos de la 8b y los fixtures dan **los mismos tokens** que en la 0.12.4
  (prueba de regresión); ninguna plantilla existente cambia de comportamiento.
- **Nunca se escriben los puntos calculados en `points_mm`** salvo que la persona pulse Convertir en polígono; así una plantilla
  `auto` sigue siendo `auto`.
- **No se inventan miembros de la API de Revit**: `CopyElement`, `LocationCurve.Curve`, `NewFamilyInstance`,
  `DisallowJoinAtEnd` y los parámetros que copie `MemberSplitter` son los que el sondeo 30 **liste**; hasta tenerlo, la fabricación
  del corte compila contra la API 2027 y lleva la reserva de 3.4.
- **El corte es reversible siempre y nunca borra la barra original**; el registro va en Extensible Storage antes de confirmar.
- El corte inclinado del nudo en K **no se toca** (`fase-12.md` 10.5).

## 5. Pruebas en la nube

- **`AutoOutlineTests`**: con los `Fakes` del Detalle D, el contorno automático contiene las tres ranuras y la placa cuchilla
  (`CheckKnifePlateInsideGusset` y `CheckBoltsInsidePlate` en verde), tiene entre 6 y 12 vértices, es simple y antihorario, está
  redondeado a 5 mm, y su caja queda a menos del **15 %** de 565 × 530 (el del plano); los 16 nudos de `HangarTruss8b` validan con
  `auto` sin errores; el mismo nudo girado ±15° sigue validando (lo que el polígono fijo no hace: `PLATE_OUTSIDE_GUSSET` a +15°
  con el Detalle D en `polygon`, como control); el K sintético de 54,9 mm envuelve los dos extremos reales; `mirror_x` y `mirror_y`
  dan el contorno reflejado; `margin_mm` 0 da `OUTLINE_AUTO_LARGE` en vez de romper; `auto` con `points_mm` → `SCHEMA_INVALID`.
- **Regresión**: los tokens del Detalle D, de los fixtures B, C, I y K y de los 16 nudos de la 8b son los de la 0.12.4.
- **`SpecEditorOutlineTests`**: cambiar a `auto` quita los puntos; Convertir en polígono escribe los calculados y a partir de ahí
  se editan; `ListFields` enseña el modo.
- **`TemplateAutoTests`**: una plantilla guardada desde un `auto` conserva el modo, casa con un nudo a 12° de desvío si su tolerancia
  lo permite (D3) y la especificación aplicada valida con el contorno recalculado; `TemplateSketch` la dibuja.
- **`SplitPointTests`**: con el K del Hangar (θ 44,4°, cordón 203,2 en el plano, 330, planchas de 9,525, `gap` 0), el punto de corte
  a ≈ 485 mm del corte con el eje del cordón y la primera plancha a 330 de la cara; con el desvío de eje de ±27,4 cada diagonal da
  su punto; θ = 90° da cara a medio canto; una barra sin `splice_hint` no tiene punto.
- **`Fase13PlanTests`**: la cercha de la 8c con `UseStarters`: cada nudo en K con `splice_hint` lleva sus pasos `split` y sus nudos
  `N…B` con el arranque B; `OrderForCreation` pone cortes → empalmes B → `N9E` → `N9`; `BatchRunner` con un creador falso que
  "parte" (devuelve un id nuevo) crea en ese orden y, si el empalme B falla, el nudo en K del mismo punto se crea igual sobre el
  tramo corto (o no, según D9) y el informe lo dice; Borrar deshace en orden inverso y los cortes al final. Según D2, los nudos con
  cartela sin plantilla salen "Listo (arranque)" con `auto` y la cabecera los cuenta aparte.
- **Simulador**: `--autocomprobar` con `split`, `unsplit`, `outline_points_mm` y `split_point_mm`; `probar_conexiones.py` con las
  pruebas nuevas (validar `auto`, aplicar una plantilla `auto`, `split` + `node_info` + `unsplit`, el plan con los pasos).

## 6. Instrucciones para el instalador (`docs/instalacion/fase-13.md`; la sesión las escribe literales)

Orden, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, con Revit cerrado al empezar:

1. Pull, build, test, `deploy.ps1` (0.13.0), `instalar-conn.ps1`, ping, list 0.
2. **La comprobación pendiente de la 12d** (`fase-12.md` 10.3), antes de nada nuevo: **(la persona)** Conectar en el nudo en K
   (1245533, 1249631, 1249632) **sin la casilla de partir** → Crear → anotar el cuadro (se espera **solo `END_CUT_APPROXIMATED`**,
   sin `REVIT_WARNING`); **(instalador)** `list`, `delete`, y en el log dos líneas `revit_failure` con `filtered: true`. Es la primera
   vez que el filtro se ve en Revit.
3. **Sondeo 30** (`scripts/sondeos/30-partir-barra.py`, `-SinTransaccion`, cancela al final salvo `%TEMP%\sondeo30-commit.txt`):
   lo de 3.3; devolver las líneas de los métodos listados, los parámetros del original y de la copia, las longitudes, las
   extensiones, `node_info` de los dos tramos y del nudo en K, y los elementos nuevos antes y después de cancelar; sondeos 12 y 13.
4. **(la persona)** Conectar sobre un nudo del cordón central **forzando el arranque** (la sesión dice cómo: Cambiar… > Tipo
   `gusset_node` sin plantilla, o apartar un momento la plantilla del catálogo) → la cartela automática en la previsualización, Contorno: Automático, Convertir en polígono y editar una cota;
   capturas `fase13-01-auto.png` y `fase13-02-poligono.png`; Crear, `list`, `delete`.
5. **(la persona)** Guardar como plantilla esa especificación en `auto`; Conectar en otro nudo del cordón central → la plantilla
   `auto` propuesta y la cartela recalculada; Cancelar.
6. **(la persona)** Conectar en el nudo en K **con la casilla** → Crear → el cuadro con las tres cosas; captura `fase13-03-cadena.png`
   del tramo corto con las dos planchas del Detalle B y la soldadura; **(instalador)** `list` 2 (empalme B y nudo en K), `node_info`
   de los dos tramos, `delete` de las dos y comprobar que la diagonal vuelve a ser una (sondeo 12: `1249631: inicio 0.0 | fin 69.2`).
7. **(instalador)** `conn_split_member` con `dry_run` y sin él sobre 1249632, `node_info`, `conn_unsplit`; sondeos 12 y 13 a cero.
8. Planificar lote sobre la cercha completa: los pasos `split` y los nudos `N…B` en la tabla (captura `fase13-04-plan.png`); **no
   crear el lote entero** en esta ronda salvo que la persona lo pida (los 15 nudos en K partirían 30 diagonales).
9. `--puente`, `catalog_list`, el log del día sin `*_failed` del add-in; commit de `docs/fases/resultados-fase-13.md` y las capturas.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas de la sección 5 y la regresión de tokens;
  `simulador_revit.py --autocomprobar`; `probar_conexiones.py` contra el simulador; `py_compile` de `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-13.md` según el Anexo B del encargo, con la lista de lo NO PROBADO y la tabla de reservas (3.4).
- `docs/instalacion/fase-13.md` literal (sección 6), con la comprobación de la 12d y el sondeo 30 **primero**.
- README, `mcp/CONTRATO-conn.md`, `docs/guide.md`, `CLAUDE.md` (estructura), las propuestas y `docs/fases/resumen-fase-13.md`.
- Versión **0.13.0** en los dos `.csproj`, `AddinInfo.cs`, el adaptador, las herramientas y el simulador.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta fase

- **Contornos cóncavos o con recortes** (una cartela en L): solo la envolvente convexa (D1). La edición arrastrando (V6) sigue
  pendiente; Convertir en polígono y las cotas de hoy bastan.
- **El corte inclinado** del nudo en K (`END_CUT_APPROXIMATED` queda como está, `fase-12.md` 10.5) y cualquier sondeo nuevo sobre la
  viga de Advance Steel.
- **Partir cordones** o barras sin `splice_hint` desde el plan: el botón Partir barra y `conn_split_member` valen para cualquier
  barra de armazón, pero el plan solo parte las diagonales de los nudos en K.
- **Partir una barra que ya tiene conexión** en el extremo que se corta: `SPLIT_IN_USE`; primero se borra la conexión.
- `kind: "to_support"` (el montante contra un apoyo), `base_plate`, `plates.second`, el cordón inferior si no está modelado
  (D10 lo pregunta), comprobación resistente, PDF directo, `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`.

## 9. Dudas que hay que aclarar con la persona antes de programar

Las preguntas se dejan tal como se hacen; la respuesta va al final de cada una en **negrita** y es lo que fija la sección 2.2.

- **D1. ¿Qué forma tiene la cartela automática?** Propongo la **envolvente convexa** de los extremos ranurados y de la franja del
  cordón, redondeada a 5 mm y con chaflanes donde las barras lo piden (así sale el Detalle D: ocho vértices). La alternativa, un
  contorno que siga cada barra (en L o con entrantes), ahorra acero pero da más vértices, esquinas entrantes que Advance Steel
  corta peor y una cartela que ya no se parece a la del plano. Recomiendo **convexa**, y `width_mm` / `height_mm` **recalculados**
  por el validador (si la IA los escribe y no cuadran, aviso `LABEL_VALUE_MISMATCH`, no error). ¿Te vale?
- **D2. ¿El plan arranca los nudos con cartela sin plantilla?** En la Fase 12 el plan no arranca cartelas porque el rectángulo era
  "a ojo". Con la cartela automática ya no lo es. Opciones: (a) seguir `no_match` (el plan solo crea cartelas con plantilla);
  (b) arranque con `auto`, estado "Listo (arranque)", contado aparte en la cabecera ("… 4 cartelas automáticas (arranque) …") y
  con el aviso `TEMPLATE_NO_MATCH` en el detalle; (c) arranque pero **excluido por defecto** (hay que pulsar Incluir). Recomiendo
  **(b)**: en el Hangar no cambia nada (los 16 casan con el Detalle D), y en una cercha nueva se ve de un vistazo cuántos nudos
  salen con la cartela calculada. ¿(a), (b) o (c)?
- **D3. Plantillas en modo `auto`.** Una plantilla guardada desde una especificación `auto` conserva el modo y se adapta a los
  ángulos del nudo; con un polígono fijo el casado se limita a 10° (`angle_tolerance_deg`). Propongo que una plantilla `auto` admita
  por defecto **25°** (clave `angle_tolerance_deg` propia en la plantilla, editable en el catálogo) y que la plantilla "Nudo tipico
  Detalle D" **siga en `polygon`** tal cual (es el dibujo del plano). ¿Te vale 25°, y dejamos el Detalle D como está?
- **D4. Márgenes de la cartela automática** (en `choices.json`, editables): del final de la ranura o de la placa cuchilla al borde,
  `gusset_margin_mm` **40** (el de hoy); del lado de la barra al borde, `gusset_side_margin_mm` **30** (nuevo); más allá del cordón
  en cada cara, `gusset_beyond_chord_mm` **40** (el de hoy). Con el Detalle D (ranuras de 150, retiros 180/60/260) estos valores dan
  una cartela parecida a la de 565 × 530 del plano; la prueba lo mide. ¿Te valen, o prefieres otros? ¿Y el tope de tamaño para el
  aviso `OUTLINE_AUTO_LARGE`, 1 200 mm de caja?
- **D5. ¿Partir como operación propia y como paso previo del plan?** Propongo una operación nueva (`conn_split_member`, botón
  **Partir barra**) que no es una conexión, y que el plan la enseñe como paso previo de cada nudo en K con `splice_hint` (línea
  "Partir 1249631 a 330 (Detalle B)" debajo del nudo), seguida del empalme B como nudo previsto `N5B`. La alternativa es esconder el
  corte dentro de la creación del nudo en K (menos pasos a la vista, pero un `conn_delete` del nudo tendría que deshacer un corte
  del que la IA no sabe nada). Recomiendo **operación propia y visible**. ¿De acuerdo?
- **D6. ¿Qué tramo conserva el id original?** Propongo que el **tramo del lado del cordón superior** (el corto, donde van el
  Detalle B y el nudo en K) sea el elemento original y el tramo largo el nuevo: el nudo en K ya escrito sigue valiendo y el nudo del
  cordón central (Detalle D) se planifica después del corte. Al revés, el nudo en K cambiaría de id y la previsualización habría que
  rehacerla. ¿Te vale? (Si en tu modelo las diagonales del cordón central ya tienen conexión creada, dilo: entonces conviene lo
  contrario.)
- **D7. ¿Dónde se mide el 330?** D6 dice "de la cara del cordón a las planchas". Propongo: desde el punto donde el **eje** de la
  diagonal cruza la **cara** del cordón (a 101,6 mm del eje del cordón, 145 mm por el eje de la diagonal a 44,4°), **a lo largo del
  eje**, hasta la **cara de la primera plancha** (la del tramo corto); el corte de los dos elementos queda en el centro de la pila
  (330 + 9,5 + `gap`/2). ¿O prefieres 330 hasta el **centro** de la pila, o medido en **vertical** desde la cara del cordón?
- **D8. Deshacer el corte.** Propongo que Borrar (y `conn_delete`) de una conexión creada por la cadena o por el lote deshaga también
  el corte que ella pidió, en orden inverso; que `conn_unsplit` / Partir barra deshagan un corte suelto; y que **nunca** se deshaga un
  corte mientras una conexión use los tramos (`SPLIT_IN_USE`). ¿Te vale? ¿Quieres además que Ctrl+Z de Revit lo deshaga solo (ya
  lo hace, porque todo va en un grupo de transacciones)?
- **D9. Conectar sobre un nudo en K.** Propongo la casilla **"Partir la diagonal y crear el empalme del Detalle B a 330"**, marcada
  por defecto cuando hay `splice_hint`, y una sola pulsación de Crear que haga corte → empalme B → nudo en K dentro de un grupo. Si
  un paso falla: (a) se revierte todo (recomendado: la barra nunca queda partida sin su empalme), o (b) se queda lo que se hizo y el
  cuadro lo dice. ¿Casilla por defecto marcada y (a)?
- **D10. Otras cerchas y el cordón inferior.** En la cercha del Hangar, hoy **ningún nudo necesita la cartela automática**: los 16
  del cordón central casan con el Detalle D y los del cordón superior son soldadura directa. Sirve para el catálogo vacío, para
  nudos con otros ángulos (¿hay otras cerchas en el Hangar con diagonales a otro ángulo o con montantes?) y para el **cordón
  inferior** si llega a estar modelado como barra (los 8 puntos "sin cordón" de abajo son parejas diagonal + montante a 91°). ¿Hay
  otras cerchas o nudos que quieras probar en la instalación de esta fase? Si el cordón inferior no existe como barra, ¿sigue
  fuera (recomendado)?
- **D11. Alcance y rondas.** Es una fase grande (dos frentes). ¿Aceptas el orden de recorte de la sección 10 y una ronda 13b con el
  instalador para lo que se recorte, como en la 12?

## 10. Riesgos y orden de recorte

- **El tamaño.** Un cálculo geométrico nuevo con panel, plantillas y plan, más una operación de modelo nueva con su deshacer, su
  cadena en Conectar y sus pasos en el lote, y un sondeo. Si no cabe, se recorta **en este orden** y lo recortado queda anotado como
  pendiente para la 13b:
  1. **Los pasos `split` y los nudos `N…B` en el plan** (3.1.6): el lote sigue como en la 12; la diagonal se parte desde Conectar o
     con Partir barra.
  2. **La cadena de una pulsación en Conectar** (3.1.4, D9): quedan el botón Partir barra y `conn_split_member`; después, Conectar
     sobre los dos tramos (empalme B) y sobre el nudo en K, de uno en uno.
  3. **El arranque con `auto` en el plan** (D2): el plan sigue `no_match` para cartelas sin plantilla.
  4. **Las plantillas en modo `auto`** (D3): una plantilla guardada desde `auto` se guarda convertida a `polygon` (con aviso).
  5. **La fabricación del corte en Revit** (`MemberSplitter`): queda `SplitPoint` en el Core, el `dry_run` que devuelve el punto, la
     ruta y la herramienta creables solo en el simulador; la fabricación va a la 13b con el resultado del sondeo 30.
  **Nunca se recorta**: la cartela automática de punta a punta en Conectar (cálculo, validación, croquis, panel, fabricación con
  los puntos calculados), la regresión de tokens del camino `polygon`, `SplitPoint` con sus pruebas ni el sondeo 30.
- **Partir una barra de armazón no tiene API directa en Revit** (el comando Dividir de la interfaz no está expuesto): copiar y
  cambiar las curvas es lo más probable, y lo que puede perderse por el camino son los parámetros de instancia (giro de sección,
  justificación, desfases) y las uniones automáticas en el corte. El sondeo 30 va **primero** en la instalación y cada mecanismo
  tiene su reserva (3.4) con su aviso. Hasta tenerlo, la sesión programa contra la API 2027 sin dar nada por bueno.
- **Las conexiones existentes en la diagonal.** Si una diagonal ya tiene conexión en el extremo que se corta (o en el otro, si el
  id cambia), partirla rompería el registro: `SPLIT_IN_USE` lo impide, y D6 decide qué extremo conserva el id.
- **La envolvente convexa puede salir grande** en nudos con barras muy abiertas o con una barra muy lejos del cordón (una barra que
  "casi" llega): `OUTLINE_AUTO_LARGE` lo avisa y Convertir en polígono permite recortar a mano. No se intenta nada más listo en esta
  fase (D1).
- **El modo `auto` cambia el arranque de Conectar**: el rectángulo de la Fase 11 desaparece. Quien lo prefiera puede Convertir en
  polígono; la prueba de la 8b asegura que los nudos con plantilla no se tocan.
- **Dos elementos nuevos por nudo en K** (30 en la cercha): el lote de la Fase 9 ya anida bien, pero el deshacer de 15 cortes al
  borrar el lote es nuevo; la instalación no crea el lote entero (6.8) hasta que la cadena de un nudo haya funcionado.
