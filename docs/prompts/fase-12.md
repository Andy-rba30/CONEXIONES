# Fase 12: placa de extremo y nudo soldado (excentricidad en el contrato, tipos `end_plate` y `direct_weld`, Conectar y el lote sobre el cordón superior)

Fase que sale de dos propuestas cerradas: `docs/propuestas/nudos-en-k-excentricos.md` (las decisiones P1 a P8 están en su
sección 7) y `docs/propuestas/placa-de-extremo.md` (el tipo nuevo, corregido el 2026-10-12 con los Detalles B, C e I), y de
lo que el cierre de la Fase 11 dejó anotado para esta (`docs/fases/fase-11.md`: 7.2 el arranque sobre el cordón superior da
`MEMBER_NOT_AT_NODE`; 7.5 decisiones del cierre; 7.6 qué sigue). Se ejecuta en **una sesión** con el prompt de la sección 1
y una ronda del instalador; lo demás de este archivo es el alcance que esa sesión debe cumplir.

**Estado: borrador del 2026-10-12, pendiente de las respuestas de la sección 9 antes de programar.** Las decisiones de las
propuestas (sección 2) ya están tomadas; las dudas de la sección 9 son las que aparecieron al escribir el alcance contra el
código y los informes, y cambian lo que se programa. **La cartela automática (`outline.mode = "auto"`) queda para la Fase 13.**

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/prompts/fase-12.md completo (sus secciones 2 y 9 fijan las decisiones), docs/propuestas/nudos-en-k-excentricos.md
(secciones 6 y 7), docs/propuestas/placa-de-extremo.md (secciones 2, 3 y 6), docs/fases/fase-11.md (secciones 7.2, 7.5 y 7.6) y
docs/fases/fase-8.md (sección 10.2). Ejecuta solo la Fase 12 con el add-in 0.12.0 tal como la describe docs/prompts/fase-12.md:
(1) la excentricidad en el contrato y en la validación, opción A: punto de trabajo en el eje del cordón a mitad entre los dos
cortes, node.eccentricity_mm y members[i].axis_offset_mm (los devuelve conn_get_node_info y los escribe Conectar), aviso
MEMBER_ECCENTRIC hasta medio canto del cordón y MEMBER_NOT_AT_NODE por encima, cada barra recortada y ranurada sobre su propio
eje; (2) el tipo end_plate (dos planchas cara a cara perpendiculares al eje, plancha con esquinas recortadas, pernos alrededor
del tubo por cara con paso y borde, tuerca a ambos lados, longitud por la pila de dos planchas) con los Detalles B, C e I de
docs/propuestas/ como fixtures en docs/fixtures/; (3) el tipo direct_weld (la diagonal cortada contra la cara del cordón y
soldada en todo el contorno, sin piezas) para los nudos en K del cordón superior; (4) Conectar y Planificar lote sobre los
nudos en K del cordón superior y los empalmes del cordón (nudo en K sin plantilla → arranque direct_weld; dos tramos
colineales → arranque end_plate con el detalle del perfil), con la detección del empalme como nudo de clase splice;
(5) diagonal_splice_offset_mm en config/choices.json y en el panel, dibujado en el croquis como marca del Detalle B; y, de
paso, probar en el PC el camino de una sola barra y el de la cercha desde Conectar. Respeta el orden de recorte de la
sección 10 si no cabe. Ninguna ventana en las rutas de la IA; el contrato solo añade; versión 0.12.0 en todo. Termina con
compilación sin avisos, pruebas en verde (Core y simulador), docs/fases/fase-12.md, docs/instalacion/fase-12.md con los
sondeos 22, 23 y 24, README, docs/guide.md, mcp/CONTRATO-conn.md, CLAUDE.md (estructura), las propuestas al día, commit, push
y un resumen corto guardado también en docs/fases/resumen-fase-12.md.
```

## 2. Decisiones ya tomadas (propuestas cerradas el 2026-10-12)

| Decisión | Qué dice | Qué hace la Fase 12 con ella |
|---|---|---|
| **P1 → opción A** (nudos en K) | Punto de trabajo en el eje del cordón a mitad entre los dos cortes; `node.eccentricity_mm` y `members[i].axis_offset_mm` en el contrato; cada barra medida sobre su eje real | Sección 3.2. `NodeFrame`, `TemplateNode.FromModelFacts`, `NodeInspector` (`conn_get_node_info`), `SpecStarter` y `PlanBuilder` usan el mismo punto |
| **P2 → medio canto del cordón** | Hasta medio canto, aviso `MEMBER_ECCENTRIC` con los mm; por encima, error `MEMBER_NOT_AT_NODE` | `NodeReach` con dos umbrales: 5 mm (`node_axis_max_distance_mm`, como hoy) y medio canto del perfil del cordón (`node_axis_max_offset_mm` en `limits.json`, 0 = medio canto del perfil) |
| **P3 → soldadura directa** | En el nudo en K no hay cartela ni placas: la diagonal se corta contra la cara del cordón y se suelda en todo el contorno | Tipo nuevo **`direct_weld`** (3.4) |
| **P4 → sí, hay detalles** | Detalles B y C como fixtures de `end_plate`; el alzado del nudo (`cordon-superior-nudo-k.png`) como fixture del nudo soldado | `docs/fixtures/detalle-B.json`, `detalle-C.json`, `detalle-I.json` con sus imágenes; `docs/fixtures/nudo-K-cordon-superior.json` |
| **P5 → 200** | En el Detalle I mandan las cotas de la sección I1: octógono de 200, esquinas de 60, pernos a 150; el rótulo "185 × 185" es error del plano | El fixture I lleva 200 y una nota en `uncertain_fields` ya confirmada |
| **P6 → valor editable** | La distancia del cordón al empalme de la diagonal (Detalle B) no está acotada: `diagonal_splice_offset_mm` en `config/choices.json`, propuesta 300, y en el panel | Sección 3.6 (a escala, en el alzado sale unos 330 mm: la persona confirma el 300 en D6) |
| **P7 → el Detalle I es del cordón inferior** (HSS3X3X1/4) | Es el empalme de ese cordón; el montante vertical del alzado se trata como el Detalle B hasta que se diga otra cosa | Fixture I en el Core. **Ojo**: según `fase-8.md` 10.2 los 7 empalmes que el plan ve son del cordón **superior** (HSS12X8, Detalle C) y el inferior no entró en la selección: **D1** lo aclara antes de programar |
| **P8 → hipótesis de soldadura directa** | El nudo del cordón superior es el Detalle G del plano, no visto a escala; si enseña otra cosa se corrige antes de programar | D7 pide el Detalle G si existe |
| Calendario (propuesta, sección 6) | Fase 12 = placa de extremo y nudo soldado; la **cartela automática pasa a la Fase 13** | Sección 8 |

## 3. Alcance (lo que se entrega)

### 3.1 Flujo completo (lo que cambia para la persona)

```
NUDO EN K DEL CORDÓN SUPERIOR (8 nudos: N5, N12, N19, N26, N33, N40, N47 y N54 en el plan de la 8c)
1. SELECCIONAR  pincha el tramo del cordón superior (HSS12X8X1/2) y las dos diagonales que llegan desde abajo y pulsa [ Conectar ]
2. RECONOCER    el add-in lee el nudo: punto de trabajo a mitad entre los dos cortes, excentricidad 34 mm, cada diagonal con su
                desvío (unos ±17 mm); casa las plantillas del catálogo; si ninguna encaja abre la ventana con el ARRANQUE
                direct_weld: "Ninguna plantilla encaja… Arranque: soldadura directa de 2 diagonales a la cara −Y del cordón
                (valores de choices.json). Excentricidad 34 mm (aviso MEMBER_ECCENTRIC)."
3. AJUSTAR      panel: sección Cordón (perfil del modelo, cara donde llegan las barras), Barra 1 y Barra 2 (corte contra la cara,
                holgura, soldadura [5 mm ▾] ☑ todo el contorno, desvío del eje en mm solo lectura, marca del empalme Detalle B a
                [300] mm). Croquis: las dos diagonales en su sitio real, el gap de 34 mm acotado, la soldadura y la marca del empalme.
4. CREAR        [Crear] con la validación en verde (el aviso de excentricidad no bloquea): recorte de cada diagonal contra la cara
                del cordón y soldadura en todo el contorno. Sin placas ni pernos. [Guardar en catálogo] como "Nudo K cordón superior".

EMPALME DEL CORDÓN (los 7 puntos donde terminan dos tramos: N9, N16, N23, N30, N37, N44 y N52 en el plan de la 8c)
1. SELECCIONAR  pincha los dos tramos que se encuentran (dos barras colineales) y pulsa [ Conectar ]
2. RECONOCER    nudo de clase "empalme" (splice): sin cordón que atraviese, dos barras a 0° y 180°. Si ninguna plantilla end_plate
                encaja, ARRANQUE end_plate con el detalle del perfil: HSS12X8X1/2 → Detalle C (PL1/2" 430 × 330, 12 pernos Ø3/4");
                HSS3X3X1/4 → Detalle I (200, 4 pernos Ø5/8" a 150); HSS2-1/2X2-1/2X3/16 → Detalle B (185, 4 pernos Ø5/8" a 135).
3. AJUSTAR      panel: Plancha (espesor, ancho, alto, esquina recortada, soldadura al tubo), Pernos (Ø, agujero, por cara: arriba /
                abajo / lados con cantidad, paso y borde; tuerca a ambos lados; longitud calculada), Tramo 1 / Tramo 2 (retiro).
                Croquis: el alzado de la plancha con el tubo dentro y los pernos alrededor, cotas de paso, borde y esquina.
4. CREAR        dos planchas soldadas a los extremos, pernos que atraviesan las dos, tramos retirados lo que mide la pila.

EL LOTE        Planificar lote sobre la cercha de la 8c (64 barras): los 16 del cordón central con el Detalle D como hoy, los 8 nudos
                en K y los 7 empalmes ya no salen en rojo: cada uno con su plantilla (si la persona guardó una) o con su arranque
                validado, y Crear N conexiones los crea en el mismo lote (un grupo por nudo, como en la Fase 9).
```

### 3.2 La excentricidad en el contrato y en la validación (Core y `NodeInspector`; se prueba en la nube)

- **Contrato (solo añade)**: `node.eccentricity_mm` (número ≥ 0, opcional, informativo: distancia a lo largo del cordón entre
  los cortes extremos de los ejes de las barras con el eje del cordón; 0 cuando concurren) y `members[i].axis_offset_mm`
  (número con signo, opcional: dónde corta el eje de esa barra al eje del cordón medido desde el punto de trabajo, positivo
  hacia +X del marco canónico). Entran en `NodeRef`, `MemberSpec`, el esquema de los tres tipos, `SpecEditor.ListFields`
  (solo lectura en el panel: "desvío del eje: −17,0 mm"), `TemplateBuilder` (**no** se guardan en la plantilla: son del nudo,
  no de la típica) y `mcp/CONTRATO-conn.md`.
- **Punto de trabajo**: `NodeFrame.Compute` recibe todas las barras que llegan (no solo la que define el plano) y pone el
  origen en el eje del cordón a mitad entre el corte más a −X y el más a +X. Con barras concurrentes el resultado es el de hoy
  (los 16 nudos del cordón central y el Detalle D no cambian ni de punto ni de especificación: prueba de regresión). Lo usan
  `TemplateNode.FromModelFacts` (Conectar, catálogo), `NodeDetector` (el plan ya hace la media: se unifica), `NodeInspector`
  (`conn_get_node_info` devuelve `eccentricity_mm` y `axis_offset_mm` por barra) y `SpecStarter`.
- **Validación** (`SpecValidator`, regla `NodeReach`): la distancia del eje de cada barra al punto de trabajo se compara con dos
  umbrales. Hasta `node_axis_max_distance_mm` (5 mm) nada; hasta **medio canto del cordón** (del perfil del modelo; si no se
  conoce, `NodeDetectorOptions.DefaultHalfDepthMm`) aviso **`MEMBER_ECCENTRIC`** ("El eje del miembro [1249632] corta el cordón a
  17,0 mm del punto de trabajo (excentricidad del nudo 34,0 mm, admitida hasta 152,4 mm)"); por encima, `MEMBER_NOT_AT_NODE` como
  hoy. `limits.json` suma `node_axis_max_offset_mm` (0 = medio canto del perfil; un número lo fija). Si el JSON trae
  `axis_offset_mm` y el modelo mide otra cosa (más de `dimension_chain_tolerance_mm`), el mismo aviso lo dice con los dos valores;
  nunca error (la IA puede no escribirlo).
- **Geometría**: retiro, ranura y soldadura de cada barra se miden **sobre su eje real** desde su propio corte con el eje del
  cordón, no desde el punto de trabajo: `ConnectionGeometry`, `GussetNodeSketch`, `MemberModifier.ApplySetback` (recibe el corte de
  la barra, no el origen del marco) y los contornos de la placa cuchilla. Prueba: un nudo sintético en K de 34 mm con cartela da
  los extremos de las barras en el mismo sitio que si cada una se dibujara sola.
- `docs/guide.md` sección 3 explica las dos claves y el aviso; sección 4, qué hacer con `MEMBER_ECCENTRIC` (nada: es
  informativo) y con `MEMBER_NOT_AT_NODE` (ahora significa "más de medio canto": revisar el nudo o el modelo).

### 3.3 Tipo `end_plate` (Core: `Types/EndPlateType.cs`; add-in: `Fabrication/EndPlateBuilder.cs`)

Dos planchas cara a cara, perpendiculares al eje de la barra, cada una soldada al extremo de su tramo, unidas por pernos
alrededor del tubo con tuerca a ambos lados. Esta fase cubre el **empalme** (`kind: "splice"`: dos tramos de la misma barra);
la variante contra un apoyo (`kind: "to_support"`: montante o diagonal contra la cara de otra barra, D11) y `base_plate` quedan
previstas en el esquema pero fuera (sección 8).

- **Esquema** (borrador de claves; en inglés): `connection_type: "end_plate"`, `kind: "splice"`, `node.element_ids` (los dos
  tramos), `members[2]` con `element_id`, `profile`, `end_setback_mm` (cuánto retrocede cada tramo desde el punto de empalme:
  normalmente medio paquete, D8), `weld` al tubo (`fillet`, `size_mm`, `all_around`); `plate` común a las dos (`thickness_mm` +
  `thickness_label`, `width_mm`, `height_mm`, `corner_cut_mm`: la esquina recortada a 45°, 0 = rectangular; las dos planchas son
  iguales en B, C e I; una clave `plates.second` opcional para que no lo sean queda prevista, no se programa); `bolts` con
  `diameter_mm` + `diameter_label`, `hole_mm` + `hole_label` (Ø18; Ø13/16"), `nuts: "both_sides"`, `length_mm` opcional y el
  patrón **alrededor del tubo** `faces`: `top`, `bottom`, `left`, `right`, cada una con `count`, `spacing_mm` y `edge_mm`
  (distancia del borde de la plancha a la fila; 0 pernos en una cara = sin fila); `gap_mm` entre planchas (0 = en contacto);
  `dimension_chains` y `uncertain_fields` como siempre. **No** se usan `rows` × `columns`.
- **Los tres fixtures, leídos de las imágenes de `docs/propuestas/`** (la persona confirma la lectura del C en D2):
  - **Detalle B / B1** (diagonal HSS2-1/2X2-1/2X3/16): PL3/8" 185 × 185, esquinas de 60 (60 + 65 + 60 por lado), 4 pernos Ø5/8"
    en agujeros Ø18, **uno centrado en cada cara** a 135 entre opuestos (`faces.*.count = 1`, `edge_mm = 25`).
  - **Detalle C / C1** (cordón superior HSS12X8X1/2, el 12" en horizontal): PL1/2" **430 de ancho × 330 de alto**, esquinas de 60
    (60 + 310 + 60 y 60 + 210 + 60), 12 pernos Ø3/4" en agujeros Ø13/16": **4 arriba y 4 abajo a paso 90** (20 + 90 + 90 + 90 + 20
    dentro de los 310 centrales: ejes a ±45 y ±135) con la fila a **25 del borde**, y **2 por lado a paso 80** (ejes a ±40) a 25 del
    borde. Nota: la propuesta decía "bordes de 60"; los 60 son las esquinas recortadas. Con esa lectura todos los pernos quedan a
    unos 38 mm de la cara del tubo (305 × 203), que es lo que enseña la sección.
  - **Detalle I / I1** (cordón inferior HSS3X3X1/4, decisión P5): PL3/8" 200 × 200, esquinas de 60 (60 + 80 + 60), 4 pernos Ø5/8"
    en Ø18 a 150 entre opuestos, uno por cara, a 25 del borde. `uncertain_fields` con el rótulo "185 × 185" confirmado como 200.
  Cada fixture: `docs/fixtures/detalle-X.json` (especificación completa y válida, con IDs ficticios o los reales si se conocen)
  y `docs/fixtures/detalle-X.png` (copia de la imagen de `docs/propuestas/`), y `source.drawing_image` apuntando a ella.
- **Validaciones propias** (códigos nuevos solo si hacen falta; se reutilizan `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL`,
  `BOLT_OUTSIDE_PLATE`, `BOLT_LENGTH_TOO_SHORT`, `WELD_BELOW_MINIMUM`, `LABEL_VALUE_MISMATCH`, `PROFILE_MISMATCH`, `OUTLINE_INVALID`):
  el perfil del JSON es el del modelo en los dos tramos y los dos tramos son colineales (a menos de 5°, `MinCrossingAngleDeg`) y
  del mismo perfil (si no, aviso); la plancha contiene la huella del tubo con holgura; cada perno a `edge_distance_mm` o más del
  borde (y de la esquina recortada: distancia al polígono, no a la caja) y a `min_spacing_factor` × Ø de los demás; cada perno a
  **`end_plate.min_bolt_to_tube_mm`** (nuevo en `limits.json`, propuesta 20 mm: media tuerca más arandela) o más de la cara del
  tubo, con el código nuevo **`BOLT_TOO_CLOSE_TO_TUBE`**; agujero mayor que el perno (`BOLT_HOLE_TOO_SMALL`, nuevo); longitud del
  perno = dos espesores + `gap_mm` + `length_addition_mm` del diámetro (la pila de la ronda 6b, `BoltStack` generalizada a "dos
  planchas sin cartela"; con tuerca a ambos lados el suplemento se aplica a los dos lados: lo fija D9); `end_setback_mm` de cada
  tramo ≥ lo que mide la pila entre el punto de empalme y su extremo.
- **Geometría (Revit)**: cada tramo se retira con `MemberModifier.ApplySetback` (ya existe) hasta dejar sitio a su plancha; cada
  plancha se crea con `CreatePlate` (contorno octogonal: el `corner_cut_mm` se convierte en polígono en el Core) perpendicular al
  eje, centrada en el eje de la barra; los pernos con el backend: Advance Steel entiende patrones rectangulares (`CreateBoltPattern`),
  así que las filas de cada cara van como **un patrón por cara** (fila de `count` × 1) o como pernos sueltos (`CreateBolts`, el
  camino de la Fase 1) si los patrones de una fila fallan: lo decide el **sondeo 23** (sección 6). Soldadura de cada plancha al
  tubo con `CreateWelds` (representación). Todo dentro del `TransactionGroup` de la operación, como el gusset.
- **Croquis** (`EndPlateSketch`, `ISketchProvider`): el alzado de la plancha mirando al eje de la barra (octógono, huella del
  tubo, pernos por cara) con cotas de ancho, alto, esquina, paso y borde (cada `SketchDimension` con su ruta JSON para el doble
  clic), y a un lado el perfil del empalme (dos planchas, el `gap`, los dos retiros, la longitud del perno). `TemplateSketch` lo
  dibuja sin modelo para la miniatura.
- **Panel**: secciones **Plancha**, **Pernos** (con una fila por cara: arriba, abajo, izquierda, derecha) y **Tramo 1 / Tramo 2**;
  `PanelChoices` suma la lista de agujeros (`hole_labels`: Ø18, Ø13/16", Ø15/16", Ø21) y `SpecEditor` escribe `hole_mm` +
  `hole_label` a la vez, como el diámetro. Palabras del panel: **plancha**, **empalme**, **tramo**, **esquina recortada**,
  **por cara**, **tuerca a ambos lados**.

### 3.4 Tipo `direct_weld` (Core: `Types/DirectWeldType.cs`; add-in: `Fabrication/DirectWeldBuilder.cs`)

La diagonal termina **contra la cara del cordón** (corte plano con el ángulo de la diagonal; la cara del HSS12X8 es plana) y se
suelda en todo el contorno. Sin cartela, sin placas, sin pernos. Es el tipo más pequeño del add-in.

- **Esquema** (borrador): `connection_type: "direct_weld"`, `node` (con `eccentricity_mm`), `chord` (`element_id`, `profile`,
  `continuous`, `face`: `auto` | `+y` | `-y`: la cara donde llegan las barras; `auto` = el lado del marco canónico donde están),
  `members[]` con `element_id`, `role`, `profile`, `expected_angle_deg`, `axis_offset_mm`, `end_cut: "to_chord_face"` (único valor
  en esta fase), `end_gap_mm` (holgura entre el corte y la cara, 0 por defecto), `weld` (`fillet`, `size_mm`, `all_around: true`) y
  `splice_hint` opcional (`detail: "B"`, `offset_from_chord_face_mm`: la marca del empalme de la diagonal, 3.6); `dimension_chains`
  y `uncertain_fields`.
- **Fixture**: `docs/fixtures/nudo-K-cordon-superior.json` con los IDs reales del paso 11-5 (cordón 1245533, diagonales 1249631 a
  −135,6° y 1249632 a −45,0°), excentricidad 34 mm, soldadura 5 mm, e imagen `cordon-superior-nudo-k.png`. Para las pruebas de
  geometría, un nudo sintético en K de 34 mm en `Fakes/` y la cercha del Hangar de la 8c con sus ocho tramos del cordón superior
  reconstruidos de `docs/fases/resultados-fase-8c.md` (como hizo la 8b con `HangarTruss8b`).
- **Validaciones**: las generales (esquema, perfiles, ángulos, `NodeReach` con la excentricidad de 3.2), soldadura ≥ mínimo de
  `limits.json` para el espesor de pared de la diagonal (3/16" = 4,8 mm → 3 mm), `end_gap_mm` ≥ 0 y menor que el canto de la
  barra, y una propia: la diagonal cabe en la cara del cordón (su huella cortada no sobresale del ancho de la cara: aviso
  `MEMBER_WIDER_THAN_CHORD_FACE`, nuevo, no error). Con dos barras en el mismo lado se comprueba que sus huellas cortadas no se
  solapan en la cara (el gap de 34 mm del Hangar lo cumple con margen; si se solapan, aviso `MEMBERS_OVERLAP_ON_FACE`, nuevo).
- **Geometría (Revit)**: el corte plano inclinado del extremo de la barra es el único mecanismo nuevo de la fase: el **sondeo 22**
  comprueba qué miembro de la API de Advance Steel en Revit 2027 lo hace (no se inventa: se prueba, y el nombre va en el informe).
  Si no hay forma fiable, la reserva es acortar la barra por su eje hasta la cara (`ApplySetback` con el retiro que resulta de la
  cara y el ángulo) y dejar el corte inclinado anotado como pendiente con el aviso `END_CUT_APPROXIMATED` (nuevo). La soldadura va
  con `CreateWelds` en el contorno de la huella. `ConnectionCreationService` despacha por `connection_type` (hoy solo conoce
  `gusset_node`: README sección 7, paso 4): `ValidateOperation`, `PreviewOperation`, `CreateOperation`, `UpdateOperation` y
  `BatchCreator` pasan por `IConnectionType.Validate` y por el constructor del tipo.
- **Croquis** (`DirectWeldSketch`): el cordón con su canto real (305), las dos diagonales en su posición real cortadas contra la
  cara, el gap entre ellas acotado, la soldadura marcada, y la marca del empalme Detalle B a `offset_from_chord_face_mm` (línea
  discontinua con su cota, editable por doble clic).
- **Panel**: secciones **Cordón** (perfil, cara) y **Barra N** (corte contra la cara, holgura, soldadura, desvío del eje solo
  lectura, marca del empalme). Sin "Unión" (no hay alternativa en este tipo).

### 3.5 Conectar y el lote sobre los nudos del cordón superior y los empalmes (Core: `NodeDetector`, `PlanBuilder`, `SpecStarter`, `TemplateChooser`; add-in: `ConnectCommand`, `BatchCreator`)

- **Clases de nudo**: `DetectedNode.Kind` nuevo: `node` (lo de hoy: un cordón que atraviesa o llega y barras que concurren) y
  **`splice`** (dos barras colineales a menos de 5° que terminan en el mismo punto, sin ninguna que atraviese). Hoy ese caso sale
  `untyped` ("todas las barras son paralelas al cordón") o, si además llegan diagonales, `no_match` con `NODE_CHORD_NOT_CONTINUOUS`
  y `PlanAdvice.IsSplice`. En los 7 puntos del cordón superior donde terminan dos tramos **también llegan dos diagonales**
  (`fase-8.md` 10.2): el detector devuelve ahí **dos nudos en el mismo punto**, el nudo en K (`kind: node`, cordón = el tramo al que
  llega cada diagonal, `continuous: false` admitido para `direct_weld` con el aviso de hoy rebajado a informativo) y el empalme
  (`kind: splice`, los dos tramos). Nombres: el empalme toma el del nudo con sufijo **E** (N9 y N9E). D5 lo confirma.
- **Plan de lote** (`PlanBuilder`): para cada nudo, primero las plantillas del catálogo del tipo que corresponde (`CatalogTemplate.
  connection_type` ya existe; `TemplateMatcher` casa `direct_weld` por ángulos como el gusset, y `end_plate` por perfil y clase, sin
  ángulos); si ninguna encaja, **el arranque del tipo** (`SpecStarter.ForKNode` → `direct_weld`; `SpecStarter.ForSplice` →
  `end_plate` con el detalle del perfil, 3.3) se valida y, si pasa, el nudo queda **`ready`** con `template_name = "arranque
  direct_weld"` / `"arranque end_plate (Detalle C)"` y su token; si no pasa, `invalid` con los errores. Los nudos de **tres**
  barras del cordón central siguen exactamente igual (regresión: los 16 `ready` de la 8b con los mismos tokens). La cabecera cuenta
  los arranques aparte: "Se crearán 38 conexiones: 16 con Nudo tipico Detalle D, 15 soldaduras directas (arranque), 7 empalmes
  (arranque Detalle C)". `PlanAdvice` suma el texto, el icono y el color de `splice`; `TrussMap` dibuja el empalme como un rombo
  sobre el cordón; `PlanMarks` no dibuja cartela fantasma en `direct_weld` ni en `end_plate` (dibuja la huella de la plancha en el
  empalme si cabe; si no, solo el marcador).
- **Crear el lote** (`BatchRunner` + `BatchCreator`): el creador delegado ya recibe el `PlanNode` con su especificación; solo
  cambia que `ConnectionCreationService` despacha por tipo. Un grupo por nudo, el que falla se revierte solo, `batch_single_undo`
  como hoy. Los dos nudos de un mismo punto (K y empalme) se crean en orden fijo: **primero el empalme** (retira los tramos y pone
  las planchas), después la soldadura directa (recorta las diagonales contra la cara del tramo que les toca).
- **Conectar** (`ConnectCommand`): con las barras seleccionadas, `NodeDetector` sobre la selección decide la clase: nudo en K →
  `TemplateChooser` entre las plantillas `gusset_node` y `direct_weld` y, sin candidata, el arranque `direct_weld` (D4); dos tramos
  colineales → plantillas `end_plate` o el arranque del detalle; si la selección trae un nudo en K **y** su empalme (los 4
  elementos de N9), Conectar abre primero el empalme y al crearlo ofrece abrir el nudo en K ("Este punto también tiene un nudo en K
  con 2 diagonales: ¿conectarlo ahora?"), sin ventanas nuevas fuera del botón. La cabecera dice el tipo y de dónde salieron los
  valores (plantilla / `choices.json` / detalle del perfil). **Cambiar…** lista las candidatas de todos los tipos y, en un arranque,
  un desplegable **Tipo** (cartela / soldadura directa / placa de extremo) para forzar otro arranque. **Guardar en catálogo**
  guarda la plantilla con su `connection_type` y su miniatura. **Editar nudo** desde el plan abre lo mismo.
- **Por el puente**: `conn_get_node_info` devuelve `kind`, `eccentricity_mm` y `axis_offset_mm`; `conn_list_types` anuncia los tres
  tipos (lee el registro: nada que programar salvo registrar); `conn_get_schema` con `connection_type`; `conn_validate`,
  `conn_preview`, `conn_create`, `conn_update` aceptan los tres (despacho por tipo); `conn_batch_plan` devuelve `kind` y los
  arranques; `conn_catalog_save` / `conn_catalog_apply` con los tipos nuevos. **Sin rutas ni herramientas nuevas**: 25 rutas y
  23 herramientas, versión 0.12.0.
- **De paso, en el PC**: el camino de **una sola barra** desde Conectar (`PickObject` hasta Esc, pendiente desde la Fase 11,
  Riesgo 3) y el de **la cercha** (más de un nudo → "usa Planificar lote"), sin código nuevo salvo lo que el instalador encuentre.

### 3.6 `diagonal_splice_offset_mm`, `choices.json` y el panel

- `config/choices.json` suma: **`diagonal_splice_offset_mm: 300`** (decisión P6; la distancia de la cara del cordón a las planchas
  del empalme de la diagonal, medida a lo largo de la diagonal), `hole_labels`, y los bloques de arranque **`starter.direct_weld`**
  (`weld_size_mm: 5`, `weld_all_around: true`, `end_gap_mm: 0`, `splice_hint: true`) y **`starter.end_plate`** con los tres detalles
  por perfil (`by_profile`: `HSS12X8X1/2` → C, `HSS3X3X1/4` → I, `HSS2-1/2X2-1/2X3/16` → B, y `default` = el I escalado al tubo:
  esquina 60, borde 25, un perno por cara a `canto + 2 × 37`) para que el arranque del empalme salga con el detalle del plano sin
  pedir nada. `PanelChoices` los lee y valida (cada diámetro en `limits.json`, cada rótulo convertible); sin el archivo, valores
  embebidos.
- **Dónde se usa el 300 en esta fase**: en el croquis y en el JSON de `direct_weld` (`members[i].splice_hint`), en el panel (campo
  "Empalme Detalle B a [300] mm", editable) y en el Encargo para IA (`DesignBriefWriter` lo nombra). **No se construye** el
  empalme de la diagonal en Revit en esta fase a menos que la diagonal esté modelada en dos tramos (entonces es un empalme más,
  clase `splice`, Detalle B por perfil): partir una barra continua en dos elementos de Revit es una operación nueva y arriesgada
  que se decide en D6 y, si se quiere, va a la Fase 13.

### 3.7 Herramientas, rutas, configuración y guía

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_get_node_info` | Suma `kind`, `eccentricity_mm`, `members[i].axis_offset_mm` y `members[i].axis_cut_mm` (dónde corta cada eje). Solo añade. |
| MCP | `conn_list_types`, `conn_get_schema` | Tres tipos; esquema y ejemplo por tipo (ya leen el registro). |
| MCP | `conn_validate`, `conn_preview`, `conn_create`, `conn_update`, `conn_batch_plan`, `conn_batch_create`, `conn_catalog_*` | Aceptan los tres tipos; `batch_plan` devuelve `kind` y marca los arranques (`starter: true`, `template_name`). Sin rutas nuevas: 25 rutas, 23 herramientas, versión **0.12.0** en adaptador, herramientas, simulador y contrato. |
| Contrato | `node.eccentricity_mm`, `members[i].axis_offset_mm` | Opcionales; `mcp/CONTRATO-conn.md` y `docs/guide.md` sección 3. |
| Contrato | `end_plate`, `direct_weld` | Esquemas y ejemplos nuevos; `docs/guide.md` sección nueva "Cómo leer una placa de extremo y un nudo soldado". |
| Configuración | `config/limits.json` | `node_axis_max_offset_mm` (0 = medio canto), `end_plate.min_bolt_to_tube_mm` (20). |
| Configuración | `config/choices.json` | `diagonal_splice_offset_mm`, `hole_labels`, `starter.direct_weld`, `starter.end_plate.by_profile`. `deploy.ps1` ya lo copia. |
| Fixtures | `docs/fixtures/` | `detalle-B`, `detalle-C`, `detalle-I` (`.json` + `.png`), `nudo-K-cordon-superior.json` (+ `.png`). |
| Cinta | Sin botones nuevos | Conectar y Planificar lote aprenden las clases y los tipos; los cuadros del botón de la cinta son los únicos permitidos. |
| Ventanas | Previsualización | Panel por tipo (secciones de 3.3 y 3.4), Cambiar… con **Tipo**, croquis nuevos; el plan con `splice` en tabla y mapa. |
| Guía | `docs/guide.md` | Secciones 3, 4 (los códigos nuevos) y la nueva de los tipos. |
| Sondeos | `scripts/sondeos/22-corte-plano.py`, `23-pernos-dos-planchas.py`, `24-soldadura-contorno.py` | Sección 6; se escriben en la nube y los ejecuta el instalador **antes** de dar por buena la fabricación. |
| Instalador | `deploy.ps1`, `instalar-conn.ps1` | Sin cambios de mecanismo (copian `choices.json`, `limits.json` y el catálogo). |

### 3.8 Arquitectura (solo lo de esta fase)

```
src/MotorConexiones.Core/
├── Contract/NodeRef.cs, MemberSpec.cs          eccentricity_mm, axis_offset_mm
├── Contract/EndPlateSpec.cs, DirectWeldSpec.cs  contrato de los dos tipos (plate, bolts.faces, splice_hint, end_cut…)
├── Types/EndPlateType.cs, DirectWeldType.cs     IConnectionType + ISketchProvider; esquema, ejemplo, Validate propio
├── Schema/JsonSchemaValidator.cs                dos esquemas nuevos; las dos claves nuevas en gusset_node
├── Validation/SpecValidator.cs, ErrorCodes.cs   MEMBER_ECCENTRIC, BOLT_TOO_CLOSE_TO_TUBE, BOLT_HOLE_TOO_SMALL, MEMBER_WIDER_THAN_CHORD_FACE,
│                                                MEMBERS_OVERLAP_ON_FACE, END_CUT_APPROXIMATED; reglas de end_plate y direct_weld
├── Model/IModelFacts.cs                          ancho, canto y espesor de pared por barra (ya los da RevitModelFacts)
├── Validation/LimitsConfig.cs                   node_axis_max_offset_mm, end_plate.min_bolt_to_tube_mm
├── Geometry3D/NodeFrame.cs, NodeReach.cs        punto medio entre cortes; dos umbrales; corte de cada eje
├── Geometry3D/BoltStack.cs, BoltRing.cs         pila de dos planchas (sin cartela, tuerca a ambos lados); pernos por cara → posiciones
├── Geometry2D/Octagon.cs                        esquinas recortadas → polígono
├── Sketch/EndPlateSketch.cs, DirectWeldSketch.cs
├── Batch/NodeDetector.cs, PlanBuilder.cs, PlanAdvice.cs, TrussMap.cs   clase splice; dos nudos en un punto; arranque por tipo; textos
├── Editing/SpecStarter.cs, PanelChoices.cs, SpecEditor.cs             ForKNode, ForSplice; listas nuevas; hole_mm + hole_label
├── Catalog/TemplateBuilder.cs, TemplateMatcher.cs, TemplateChooser.cs, TemplateSketch.cs   plantillas de los tres tipos
└── Brief/DesignBriefWriter.cs                   los tipos nuevos y la marca del empalme en el encargo

src/MotorConexiones.Revit/
├── Bridge.cs                                    registra los tres tipos
├── Node/NodeInspector.cs                        kind, eccentricity_mm, axis_offset_mm, axis_cut_mm
├── Fabrication/EndPlateBuilder.cs, DirectWeldBuilder.cs, MemberModifier.cs (corte contra la cara o retiro por eje), IFabricationBackend (filas por cara)
├── Services/ConnectionCreationService.cs        despacho por connection_type
├── Operations/Validate|Preview|Create|UpdateOperation.cs   IConnectionType.Validate
├── Batch/BatchCreator.cs, PlanMarks.cs          orden empalme → nudo K; marcas de splice
├── ConnectCommand.cs                            clase del nudo, arranque por tipo, empalme + nudo K en el mismo punto
└── UI/SpecPanel.xaml(.cs), PreviewWindow        secciones por tipo, desplegable Tipo en Cambiar…

config/limits.json, config/choices.json          claves de 3.7
docs/fixtures/                                   detalle-B, detalle-C, detalle-I, nudo-K-cordon-superior (.json + .png)
scripts/sondeos/22-corte-plano.py, 23-pernos-dos-planchas.py, 24-soldadura-contorno.py
mcp/revit_mcp/conexiones.py, mcp/tools/conn_tools.py, mcp/pruebas/simulador_revit.py, mcp/CONTRATO-conn.md   0.12.0; tipos y claves nuevas
src/MotorConexiones.Tests/                       EccentricityTests, EndPlateTests, DirectWeldTests, NodeDetectorTests (splice), BatchPlanTests (Hangar 8c),
                                                 SpecStarterTests, CatalogTests, SketchBuilderTests, PanelChoicesTests, SpecValidationTests (claves nuevas)
```

## 4. Reglas técnicas de esta fase

- **El contrato solo añade**: dos claves opcionales en `gusset_node` y dos tipos nuevos. Una especificación válida hoy sigue
  siendo válida y produce la misma geometría (regresión sobre el Detalle D y los 16 nudos de la 8b).
- **El punto de trabajo es uno y se calcula en un sitio** (`NodeFrame.Compute`): `NodeInspector`, `TemplateNode`, `NodeDetector` y
  `SpecStarter` lo llaman; nadie hace su propia media.
- **Cada barra se mide sobre su eje real.** Retiro, ranura, corte contra la cara y soldadura parten del corte del eje de esa barra
  con el eje del cordón, no del punto de trabajo. Las plantillas no guardan excentricidad: al aplicarlas, el nudo real la pone.
- **`MEMBER_ECCENTRIC` es aviso, nunca error; `MEMBER_NOT_AT_NODE` ahora es "más de medio canto"**. Ningún otro umbral se toca.
- **No se inventan miembros de la API de Revit ni de Advance Steel**: el corte plano inclinado (sondeo 22), los pernos a través de
  dos planchas con tuerca a ambos lados y las filas por cara (sondeo 23) y la soldadura en el contorno de un tubo (sondeo 24) se
  escriben como sondeos y la fabricación se programa contra lo que compile en 2027; lo que el sondeo no confirme queda con su
  reserva (retiro por eje; pernos sueltos; soldadura como línea) y el aviso correspondiente, nunca callado.
- **Una operación = un `TransactionGroup`**; en el lote, un grupo por nudo dentro del grupo exterior; los dos nudos de un mismo
  punto son dos operaciones (empalme y después soldadura directa).
- **Ninguna ventana en las rutas `conn_*`**; los cuadros nuevos son solo los de Conectar (empalme + nudo K en el mismo punto) y la
  previsualización. El manejador de clics de las etiquetas sigue sin abrir nada.
- **El JSON sigue siendo la única fuente de verdad** de la previsualización; un desplegable con rótulo escribe `*_mm` y `*_label`
  a la vez (ahora también `hole_mm` + `hole_label`).
- `ElementId.Value`; unidades solo en `Units/UnitConverter.cs`; rótulos con `LabelParser`; código y claves JSON en inglés;
  textos en español; versión **0.12.0** en `AddinInfo`, los dos csproj, adaptador, herramientas, simulador y contrato.

## 5. Pruebas en la nube

- **`EccentricityTests`**: nudo sintético en K de 34 mm (cordón HSS12X8, dos diagonales a −135,6° y −45°): el punto de trabajo
  queda a mitad; `axis_offset_mm` ±17; `eccentricity_mm` 34; la validación de un `gusset_node` sobre él da **aviso**
  `MEMBER_ECCENTRIC` y ningún error; a 160 mm da `MEMBER_NOT_AT_NODE`; con `node_axis_max_offset_mm: 30` da error a 34; el Detalle
  D y los 16 nudos de `HangarTruss8b` dan el mismo punto, la misma especificación y los mismos tokens que antes (regresión); el
  retiro de cada barra se mide desde su propio corte (los extremos de las barras coinciden con el dibujo de cada una sola).
- **`DirectWeldTests`**: el fixture `nudo-K-cordon-superior.json` pasa el esquema y valida con los `Fakes` del K de 34 mm (solo el
  aviso de excentricidad); soldadura 2 mm → `WELD_BELOW_MINIMUM`; `end_gap_mm` negativo → `SCHEMA_INVALID`; una diagonal de 150 mm
  de ancho sobre una cara de 203 → `MEMBER_WIDER_THAN_CHORD_FACE`; dos huellas que se pisan → `MEMBERS_OVERLAP_ON_FACE`; el croquis
  tiene cordón, dos barras cortadas contra la cara, la cota del gap y la marca del empalme a 300; `SpecStarter.ForKNode` escribe una
  especificación válida con los valores de `choices.json`.
- **`EndPlateTests`**: los tres fixtures pasan el esquema y validan sin errores con sus perfiles; en el C, 12 pernos con las
  posiciones esperadas (±45 / ±135 arriba y abajo, ±40 a los lados), ninguno a menos de 25 del borde ni a menos de 20 de la cara
  del tubo, longitud = 2 × 12,7 + suplemento de Ø3/4" redondeada a 1/4"; en el B y el I, cuatro pernos a 135 / 150 entre opuestos;
  `edge_mm: 10` en el C → `BOLT_EDGE_DISTANCE_TOO_SMALL`; el I sobre un tubo HSS4X4 (101,6) → pasa con 24 mm a la cara (y con
  `min_bolt_to_tube_mm: 30` → `BOLT_TOO_CLOSE_TO_TUBE`); agujero 15 con perno 15,875 → `BOLT_HOLE_TOO_SMALL`; `corner_cut_mm` 0 →
  rectángulo; tramos a 8° → no colineales; el octógono de 185 con esquina 60 tiene ocho vértices y los pernos dentro; el croquis
  con sus cotas y rutas; `SpecStarter.ForSplice` elige el C para HSS12X8X1/2, el I para HSS3X3X1/4, el B para HSS2-1/2X2-1/2X3/16 y
  el `default` escalado para HSS4X4X3/16.
- **`NodeDetectorTests`**: dos barras colineales que se encuentran → un nudo `kind: splice` (ya no `untyped`); con dos diagonales
  además → **dos** nudos en el punto (N9 y N9E); el empalme 1250938/1250939 de la 8b sale `splice`; el resto de la cercha sintética y
  de las dos del Hangar no cambia de números salvo esos (las pruebas de la 8 y la 8b se actualizan con el conteo nuevo y se dice cuál).
- **`BatchPlanTests`**: la cercha del Hangar de la 8c (64 barras, reconstruida de `resultados-fase-8c.md`) con la plantilla
  oficial: **16 `ready` del Detalle D con los mismos tokens que la 8b**, los 8 nudos en K `ready` con arranque `direct_weld`, los 7
  nudos en K sobre empalme `ready`, los 7 empalmes `ready` con arranque Detalle C, N49 y N58 (una diagonal) `ready` o `no_match`
  según D4; la cabecera exacta; con una plantilla `direct_weld` guardada desde el fixture, los 15 K la usan en vez del arranque;
  `BatchRunner` crea en orden empalme → nudo K y el fallo de uno no toca al otro (creador falso).
- **`CatalogTests`**: guardar y aplicar una plantilla `direct_weld` y una `end_plate` (ida y vuelta por JSON, `connection_type`,
  casado por ángulos / por perfil, miniatura del croquis sin modelo).
- **`SpecValidationTests`**: `eccentricity_mm` y `axis_offset_mm` presentes, `null` y ausentes pasan; una clave desconocida sigue
  dando `SCHEMA_INVALID`; `conn_get_schema` de los tres tipos devuelve esquemas distintos y sus ejemplos validan.
- **`PanelChoicesTests`**: `choices.json` del repositorio con las claves nuevas (todos los rótulos convierten, `diagonal_splice_offset_mm`
  300, los tres detalles por perfil); sin archivo, embebidos.
- **MCP**: `simulador_revit.py --autocomprobar` (tipos nuevos en `types`, `schema`, `validate`, `preview`, `create`, `batch_plan` con
  `kind` y arranques; `node_info` con la excentricidad); `probar_conexiones.py` con tres pruebas nuevas (validar el fixture C, el
  fixture K y un `batch_plan` con un empalme); `py_compile` de `mcp/` y `scripts/sondeos/` (incluidos 22, 23 y 24).
- `dotnet build` sin avisos, `dotnet test` en verde (238 más las nuevas).

**NO PROBADO en la nube, por qué**: toda la fabricación en Revit (el corte contra la cara, las planchas octogonales, los pernos a
través de dos planchas con tuerca a ambos lados, las filas por cara, la soldadura en el contorno), el retiro real de los tramos,
el `node_info` sobre el modelo (la excentricidad medida en el Hangar), Conectar y el lote de punta a punta, los croquis en WPF, el
camino de una sola barra y el de la cercha. Se prueba en el PC (sección 6).

## 6. Instrucciones para el instalador (`docs/instalacion/fase-12.md`)

Sobre la **copia** `HANGAR_PRUEBA_sondeo.rvt` como único proyecto abierto, con `Anota`, el servidor del puerto 8000 arrancado antes
del `--puente` y resultados en `docs/fases/resultados-fase-12.md`. **Crea acero de verdad** (dos nudos sueltos y un lote) y lo borra
al final. Unos **90 minutos**, en dos mitades: los sondeos primero, porque deciden si la fabricación de la segunda mitad es la buena
o la reserva.

1. **(instalador, Revit cerrado)** `git pull`, build sin avisos, test, `deploy.ps1` (`0.12.0.0`; `limits.json` y `choices.json` con las
   claves nuevas), `instalar-conn.ps1` (25 rutas, 23 herramientas).
2. **(instalador)** Abrir Revit: `ping` 0.12.0; `list_types` con **3** tipos; sondeos 12, 13, 17 y 21 a cero.
3. **(instalador) Sondeo 22, corte plano**: sobre una diagonal del cordón superior (1249631), intentar el corte inclinado del extremo
   contra la cara del cordón con el miembro de la API que el sondeo prueba (y las alternativas que lista), dentro de una transacción
   que se revierte; anotar literalmente qué funcionó, el nombre del miembro y la captura `fase12-01-corte`. **Sondeo 23, pernos**:
   dos planchas de prueba cara a cara y un perno que las atraviesa con tuerca a ambos lados (si la API lo permite) y una fila de
   4 pernos como patrón de 4 × 1 y como pernos sueltos; anotar qué quedó en Advance Steel (captura `fase12-02-pernos`). **Sondeo 24,
   soldadura**: la representación de una soldadura en el contorno de la huella de un tubo sobre una cara; anotar. Los tres con rollback
   y sondeos 12 y 13 a cero después.
4. **(instalador) `node_info`** sobre 1245533 + 1249631 + 1249632: `kind: node`, `eccentricity_mm` ≈ 34, `axis_offset_mm` ≈ ±17, el
   punto de trabajo a mitad; sobre dos tramos del cordón superior que se encuentran (N9: anotar los IDs del detalle del plan):
   `kind: splice`.
5. **(la persona) Conectar en un nudo en K** (los tres elementos del paso 4): la ventana se abre con el arranque `direct_weld`, la
   cabecera literal, el croquis con el gap de 34 mm y la marca del empalme a 300, el panel; cambiar la soldadura a 6 mm y volver;
   validación **en verde con el aviso** `MEMBER_ECCENTRIC`. Captura `fase12-03-nudo-k`. **Crear** → `connection_id`; mirar en Revit
   cómo quedó el extremo de cada diagonal (corte inclinado contra la cara, o retiro por eje con el aviso `END_CUT_APPROXIMATED`) y la
   soldadura. Captura `fase12-04-nudo-k-creado`. **Guardar en catálogo** como "Nudo K cordón superior" (miniatura).
6. **(la persona) Conectar en un empalme** (los dos tramos del paso 4): arranque `end_plate` Detalle C, croquis de la plancha con los
   12 pernos, panel con una fila por cara; cambiar el borde de arriba a 10 → `BOLT_EDGE_DISTANCE_TOO_SMALL` junto al campo; volver.
   Captura `fase12-05-empalme`. **Crear** → planchas, pernos (con tuerca a ambos lados o con la reserva anotada), tramos retirados;
   captura `fase12-06-empalme-creado`. Sondeo 12 con 2 conexiones.
7. **(la persona) Una sola barra y la cercha desde Conectar**: pinchar **una** diagonal del cordón central y Conectar → pide pinchar
   el cordón y las barras (Esc termina) → el Detalle D con plantilla; Cancelar. Seleccionar media cercha y Conectar → "parece una
   cercha: usa Planificar lote" y el enlace la abre. Anotar (pendiente desde la Fase 11).
8. **(la persona) Planificar lote** con las 64 barras de la 8c: la cabecera con los 16 del Detalle D (dos ya conectados en 5 y 6:
   `already_connected`), los 15 nudos en K con la plantilla guardada en 5 (o el arranque), los 7 empalmes con el arranque C; el mapa
   con los rombos de los empalmes; el detalle de N9 y N9E. Captura `fase12-07-plan`. **Crear N conexiones** → el informe; sondeo 12
   con el total; capturas `fase12-08-lote`. Anotar los nudos `failed` con su error literal.
9. **(instalador)** Por el puente: `validate` del fixture C y del fixture K (`ok: true`, el aviso en el K); `batch_plan` sobre los 4
   elementos de N9 (dos nudos, `kind`); `list` con todas; **Borrar el lote** y `delete` de las dos sueltas (barras restauradas, incluidos
   los cortes inclinados: anotar si el corte **no** se deshace); sondeos 12, 13, 17 y 21 a cero; `probar_conexiones.py --puente` entero;
   el log del día sin `ribbon_connect_failed`, `ribbon_create_failed`, `advance_steel_plate_failed`, `advance_steel_bolts_failed`, `fabrication_transaction_failed`, `plan_event_failed` ni `ribbon_batch_window_error`.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; `simulador_revit.py --autocomprobar`;
  `probar_conexiones.py` contra el simulador; `py_compile` de todo `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-12.md` según el Anexo B del encargo, con la lista de lo NO PROBADO (sección 5) y la tabla de qué reserva se
  activa si cada sondeo falla.
- `docs/instalacion/fase-12.md` literal (sección 6), con los tres sondeos primero y qué devolver.
- README (estado, tabla de garantías, árbol, sección 6 con las claves nuevas de `limits.json` y `choices.json`, sección 7 con el
  despacho por tipo ya hecho, sección 9 con Conectar sobre un nudo en K y un empalme, sección 12 con las clases de nudo, errores),
  `mcp/CONTRATO-conn.md` (claves y tipos), `docs/guide.md` (secciones 3, 4 y la nueva), `CLAUDE.md` (estructura: tipos, `BoltRing`,
  `Octagon`, `EndPlateBuilder`, `DirectWeldBuilder`, clase `splice`), `docs/propuestas/nudos-en-k-excentricos.md` y
  `placa-de-extremo.md` (estado: programadas en la Fase 12; `base_plate` y `to_support` siguen como propuesta) y
  `docs/propuestas/flujo-intuitivo.md` (la fila 12 del calendario) al día.
- `docs/fases/resumen-fase-12.md` con el resumen del chat.
- Commit en español en la rama de trabajo; sin pull requests.

## 8. Fuera de alcance de esta fase

- **Cartela automática** (`outline.mode = "auto"`): Fase 13. En esta cercha ningún nudo la necesita hoy; el arranque de Conectar
  para un nudo con cartela sigue siendo el rectángulo de la Fase 11.
- **Partir una barra continua** en dos elementos de Revit para colocar el empalme de la diagonal (Detalle B) a 300 mm del cordón:
  Fase 13 si D6 lo pide; en esta fase el 300 se dibuja, se escribe y se edita, pero no se fabrica sobre una barra continua.
- **`end_plate` contra un apoyo** (`kind: "to_support"`: el montante vertical del alzado contra la cara del cordón, D11) y
  **`base_plate`** (anclaje): el esquema deja el hueco; se programan en una fase propia cuando el modelo los tenga.
- **Planchas distintas en los dos tramos** de un empalme (`plates.second`): previsto en el esquema, no programado.
- **El cordón inferior** (HSS3X3X1/4) si no está modelado como barra de armazón (pendiente desde la 8d): el Detalle I entra como
  fixture y como arranque por perfil, y se crea en Revit solo si la persona lo selecciona y existe (D1).
- Soldaduras de penetración, chaflanes del corte, comprobación resistente del nudo en K (AISC 360 K / CIDECT): solo geometría.
- `conn_design_brief` (P4 del encargo), `thumbnail_path`, `conn_batch_update`, el traspaso de `mcp/` a `revit-mcp`, PDF directo,
  la edición arrastrando (V6), fusionar Conectar y Planificar lote.

## 9. Dudas que hay que aclarar con la persona antes de programar

Las preguntas se dejan tal como se hacen; la respuesta va al final de cada una en **negrita** cuando la persona la dé, y la
sección 2 se completa con ella.

- **D1. ¿De qué cordón son los 7 empalmes?** El encargo de esta fase dice "los 7 empalmes del cordón inferior" y la decisión P7
  dice que el Detalle I (HSS3X3X1/4) es "el que el plan llama empalme del cordón (7 en la cercha de la 8c)". Pero `fase-8.md` 10.2
  dice otra cosa, con los datos del PC: los 7 "falta el cordón" del plan (N9, N16, N23, N30, N37, N44 y N52) son los empalmes del
  **cordón superior HSS12X8X1/2**, modelado en ocho tramos (cada uno con `chord_type_name: HSS12X8X1/2` en `resultados-fase-8c.md`),
  y **el cordón inferior no entró en la selección** (en Z ≈ 14 914 no había barra horizontal; la 8d lo dejó pendiente y sigue sin
  anotarse). ¿Confirmas que (a) los 7 empalmes que el lote va a conectar son los del cordón superior, con el **Detalle C**, y (b) el
  cordón inferior existe en el modelo como barra de armazón estructural y, si existe, cuántos tramos tiene (sus empalmes serían el
  Detalle I)? Recomiendo programar la fase sobre (a) y tratar (b) como "entra si lo seleccionas": el arranque por perfil ya pone el
  Detalle I en un empalme HSS3X3X1/4.
- **D2. Lectura del Detalle C.** De la imagen `detalle-C-empalme-cordon-superior.png` leo: plancha 430 de ancho (a lo largo del 12")
  × 330 de alto, esquinas de 60, filas de 4 pernos a paso 90 arriba y abajo con los ejes a **25 del borde** (no a 60: los 60 son las
  esquinas), 2 pernos por lado a paso 80 a 25 del borde; agujeros Ø13/16" para Ø3/4". ¿Es correcto? ¿Y la holgura entre planchas
  (`gap_mm`) es cero (en contacto)? Recomiendo cero.
- **D3. Perfiles del modelo frente a los del detalle.** Los tramos del cordón superior son HSS12X8X1/2 como el Detalle C. Si el cordón
  inferior entra (D1) y sus tramos son HSS4X4 como el cordón central de la 8b, el Detalle I (dibujado para HSS3X3) deja 24 mm
  entre la cara del tubo y el eje del perno: ¿se usa igual (plancha 200, pernos a 150) o se escala (pernos a `canto + 74`, plancha
  `canto + 124`)? Recomiendo el detalle tal cual para HSS3X3 y el escalado (`starter.end_plate.by_profile.default`) para cualquier
  otro perfil, con aviso `TEMPLATE_PROFILE_DIFFERS` como hoy.
- **D4. Arranque sin plantilla.** Cuando ninguna plantilla encaja: en un nudo con cordón y barras **todas a un mismo lado** (K)
  propongo abrir el arranque **`direct_weld`**; con barras a los dos lados (como el Detalle D) la cartela rectangular de hoy; en dos
  tramos colineales, `end_plate` con el detalle del perfil; y en los dos casos un desplegable **Tipo** en Cambiar… para forzar otro.
  ¿Vale esa regla? ¿Y los dos nudos del extremo derecho del cordón superior con **una sola** diagonal (N49 y N58): soldadura directa
  de una barra (recomiendo sí: el tipo admite 1 a 3 barras) o quedan `no_match`?
- **D5. Los 7 puntos donde el empalme coincide con el nudo en K.** Propongo dos conexiones en el mismo punto (N9 = soldadura directa
  de las dos diagonales; N9E = empalme Detalle C), creadas en ese orden, cada una con su grupo y su borrado. ¿De acuerdo? Y en esos
  puntos, ¿cada diagonal se suelda al tramo al que llega su eje (puede ser uno distinto para cada una, con la plancha entre medias
  a pocos mm) o el plano manda otra cosa? El alzado del nudo enseña los empalmes a 610 mm a cada lado del nudo, no en el nudo:
  si el modelo los tiene en el nudo, se construye lo que hay en el modelo y se anota la diferencia.
- **D6. El empalme de la diagonal (Detalle B).** ¿Las diagonales están modeladas como **una sola barra** desde el cordón central
  hasta el superior? Si es así, en esta fase el empalme a 300 mm solo se dibuja y se escribe (3.6); fabricarlo exige partir la barra
  (operación nueva) o modelar planchas y pernos sobre la barra continua (acero falso). Recomiendo: dibujar y anotar ahora, partir la
  barra en la Fase 13 si lo quieres. ¿Y el 300 te vale? A escala, en el alzado sale unos 330.
- **D7. El nudo soldado.** ¿Tienes el **Detalle G** (el del nudo del cordón superior) para confirmar la soldadura directa, el tamaño
  del filete y si hay holgura entre el corte y la cara? Sin él propongo filete de **5 mm** en todo el contorno (pared 3/16"), corte
  contra la cara sin holgura. ¿Y si la API no permite el corte inclinado (sondeo 22), aceptas como reserva el retiro por eje con el
  aviso `END_CUT_APPROXIMATED` hasta que haya forma de hacerlo, o prefieres que la fase no cree nada en ese caso?
- **D8. Retiro de los tramos en el empalme.** En el modelo, ¿los dos tramos del cordón superior terminan en el **mismo punto** (hay
  que retirar cada uno medio paquete: 12,7 mm en el C) o ya dejan el hueco de las dos planchas? El plan de la 8c tiene el
  `end_gap_mm` de esos nudos: la sesión lo lee antes de decidir, pero si lo sabes, dilo. Recomiendo que Conectar calcule el retiro
  que falta a partir del hueco medido, como hace con las barras del gusset.
- **D9. Tuerca a ambos lados.** Si Advance Steel no ofrece el perno con tuerca a ambos lados (sondeo 23), ¿aceptas el perno estándar
  (cabeza y tuerca) con la longitud correcta y un aviso en el informe, o quieres pernos sueltos de otra forma? Recomiendo aceptarlo.
- **D10. Plantillas de los tipos nuevos en el catálogo.** ¿Quieres guardar el nudo en K y el empalme como plantillas (Guardar en
  catálogo con `connection_type`) y que el plan las case, o te basta con el arranque por tipo y perfil para el lote? Recomiendo las
  dos cosas; si no cabe, el catálogo de los tipos nuevos es lo primero que se recorta (sección 10) y el lote usa los arranques.
- **D11. El montante vertical.** En el alzado hay un montante en cada nudo del cordón superior con plancha y pernos contra el cordón.
  En el plan de la 8c los nudos en K tienen solo dos diagonales: ¿el montante no está en el modelo, o no se seleccionó? Si existe,
  ¿lo dejamos fuera de la Fase 12 (recomendado: es `end_plate` contra apoyo, otra clase) o entra como tercera barra del nudo?
- **D12. Alcance y rondas.** Es la fase más grande desde la 8. ¿Aceptas el orden de recorte de la sección 10 y que lo recortado vaya
  a una ronda **12b** con el instalador, como pasó con la 9b y la 10b? Recomiendo sí.

## 10. Riesgos y orden de recorte

- **El tamaño.** Dos tipos nuevos, el despacho por tipo en cinco operaciones, el detector con una clase nueva, el plan con arranques,
  dos croquis, dos paneles y tres sondeos. Si no cabe, se recorta **en este orden** y lo recortado queda anotado como pendiente con lo
  que falte, para la ronda 12b:
  1. Las **plantillas de los tipos nuevos en el catálogo** (D10): el lote y Conectar usan los arranques por tipo y perfil.
  2. **La marca del empalme de la diagonal** en el croquis y el panel (`splice_hint`): queda solo el valor en `choices.json`.
  3. **El lote sobre los empalmes** (clase `splice` en el plan y `BatchCreator`): el empalme se conecta de uno en uno con Conectar.
  4. **El lote sobre los nudos en K** (arranque `direct_weld` en el plan): se conectan de uno en uno con Conectar.
  5. **La fabricación de `end_plate` en Revit** (`EndPlateBuilder`): queda el tipo entero en el Core (contrato, validación, fixtures,
     croquis, panel, puente), creable solo en el simulador, y la fabricación va a la 12b con el resultado del sondeo 23.
  **Nunca se recorta**: la excentricidad (3.2), `direct_weld` de punta a punta en un nudo suelto (es lo que desbloquea los 8 nudos),
  el contrato y la validación de `end_plate` con los tres fixtures, ni los tres sondeos.
- **La API de Advance Steel para el corte inclinado y los pernos a través de dos planchas** no está probada (los pernos del gusset
  atraviesan cartela y cuchilla, con cabeza en la placa; el corte de hoy es por el eje). Los sondeos 22 y 23 van **primero** en la
  instalación y cada mecanismo tiene su reserva escrita (3.3 y 3.4) con su aviso.
- **Cambiar el punto de trabajo puede mover los nudos de hoy.** Con barras concurrentes la media de los cortes es el mismo punto
  a menos de 5 mm, pero no idéntico: la prueba de regresión exige los **mismos tokens** en los 16 nudos de la 8b y en el Detalle D;
  si un token cambia, el cálculo se ajusta hasta que no cambie (el punto de trabajo con una sola barra que llega, o con todas a
  menos de 5 mm, es el de hoy).
- **`MEMBER_NOT_AT_NODE` se relaja hasta medio canto**: una especificación que antes fallaba ahora valida con aviso. Es lo decidido
  (P2); la guía lo explica y el aviso lleva siempre los mm.
- **Dos nudos en un punto** rompen la hipótesis "un punto, un nudo" de `PlanMarks`, las etiquetas del lienzo y el mapa: el empalme
  lleva su etiqueta desplazada (N9E) y su marcador propio; el sondeo 21 (etiquetas) se repite después del plan.
- **Las filas por cara como patrones de Advance Steel** pueden no convivir con un patrón de 4 × 1 (un patrón "rectangular" de una
  fila): si el sondeo 23 lo rechaza, pernos sueltos, que ya funcionaron en la Fase 1.
- **El perfil del tramo HSS12X8 orientado**: el 12" va en horizontal en el Detalle C; en el modelo hay que leer la orientación del
  perfil (`RevitModelFacts` ya lee `b` / `h` y el espesor de pared del tipo, o los infiere del nombre AISC) para poner la plancha 430 en el sentido correcto; si el modelo lo tiene girado,
  la plancha gira con él y se anota.
