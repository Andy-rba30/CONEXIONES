# Fase 13: cartela automática (`outline.mode = "auto"`) y partir la diagonal para fabricar el empalme del Detalle B

Fecha: 2026-10-24. Rama de trabajo: `main`. Add-in **0.13.0** (Core con `Geometry2D/AutoOutline` y `Polygon2D.ConvexHull` /
`Simplify`, el contrato y el esquema con `gusset.outline.mode` y los márgenes opcionales, `SpecValidator` resolviendo `auto`
antes de la regla 9 con `OUTLINE_AUTO_LARGE`, `GussetNodeSketch` dibujando los puntos calculados, `SpecEditor` y `SpecStarter`
con el modo, `TemplateBuilder` con la tolerancia de las plantillas `auto`, `Geometry3D/SplitPoint`, `Storage/SplitRecord`,
`Batch/SplitPlanning`, los pasos `split` y los nudos `N…B` en `PlanBuilder`, `BatchPlan`, `PlanAdvice`, `BatchReport` y
`BatchRunner`; add-in con `Fabrication/MemberSplitter`, `Storage/SplitStorageManager`, `Services/SplitService` y
`SplitChainService`, `Operations/SplitMemberOperation` (`split` y `unsplit`), la cartela automática en `ConnectionCreationService`,
`Validate` / `Preview` / `CatalogApply` con `outline_points_mm`, `NodeInfo` con `split_point_mm`, `BatchCreator` con los pasos de
corte y empalme B y el deshacer del lote, `ConnectCommand` y `RunSpecCommand` con la casilla de partir, el panel **Contorno**, el
botón **Partir barra** y la ventana del plan con los pasos en el detalle), adaptador 0.13.0 (**27 rutas**: `/conn/split/` y
`/conn/unsplit/` nuevas), **25 herramientas** `conn_*` (`conn_split_member` y `conn_unsplit` nuevas), simulador 0.13.0 y sondeo 30.
Prompt y alcance: `docs/prompts/fase-13.md` (las decisiones de la persona, D1 a D11, en su sección 2.2: todas "como recomiendas").

**Estado: programada en la nube el 2026-10-24; probada en Revit el 2026-10-31 (`resultados-fase-13.md`) y cerrada el mismo día con
el add-in 0.13.1 (sección 7).** La instalación de la fase desplegó la 0.13.0 con el filtro de la 0.12.4 (comprobado en el paso 13-3):
todo lo nuevo funcionó en Revit (la cartela automática, la plantilla `auto`, la cadena de Conectar, `split` / `unsplit`, el plan con los
cortes, el puente 49/49); el sondeo 30 paró por un error suyo y el mecanismo del corte lo decidió el propio add-in (`copy_element`, sin
parámetros aproximados). **La 0.13.1 del cierre está NO PROBADA en Revit**: se despliega con `docs/instalacion/fase-13b.md` (ronda
corta, unos 25 minutos), que además repite el sondeo 30 corregido y los sondeos 12 y 13 con sus nombres reales.

---

## 1. Qué se hizo

### 1.1 La cartela automática (sección 3.2 del prompt; decisiones D1, D2, D3, D4)

- **Contrato (solo añade).** `gusset.outline.mode` admite `"polygon"` (lo de siempre: `points_mm` obligatorio) y `"auto"`
  (sin `points_mm`: el contorno se **calcula desde las barras**; escribirlos es `SCHEMA_INVALID` en `gusset.outline.points_mm`).
  Claves opcionales de `auto`: `margin_mm` (40), `side_margin_mm` (30) y `beyond_chord_mm` (40); negativas, `SCHEMA_INVALID`.
  Se serializan solo si existen (`JsonIgnore` con `null`), así que una especificación `polygon` da **el mismo JSON canónico y el
  mismo token** que en la 0.12.4 (`Fase13RegressionTests`: 21 tokens volcados de la 0.12.4 para el Detalle D, los fixtures B, C,
  I y K y los 16 nudos de la cercha de la 8b; el hash de `limits.json` solo cambia si `auto_outline_max_vertices` deja de ser 12).
- **El cálculo** (`AutoOutline.Compute`, determinista, Core): por cada barra, el rectángulo que va desde su retiro hasta el final de
  la ranura (o de la placa cuchilla más su inserción) **más el margen**, con medio ancho del perfil **más el margen lateral** a cada
  lado, sobre su eje real (ángulo con signo y `axis_offset_mm` del nudo); una franja del cordón que cubre la extensión en X de esos
  rectángulos y asoma `beyond_chord_mm` más allá del medio canto (a los dos lados con `through_slot` / `both_sides`, a uno con
  las demás interfaces); la **envolvente convexa** (D1, Andrew) de todo, **redondeada a 5 mm hacia fuera**, sin giros menores de
  3° y con **12 vértices como mucho** (`auto_outline_max_vertices`). Con el Detalle D sale una caja de unos 610 × 590 mm (dentro
  del 15 % del 565 × 530 del plano), con los chaflanes donde la diagonal lo pide, y las tres ranuras y la placa cuchilla quedan
  dentro (`CheckKnifePlateInsideGusset`, `CheckBoltsInsidePlate`). El mismo nudo girado ±15° sigue validando (el polígono fijo da
  `PLATE_OUTSIDE_GUSSET` a +15°, como control). Si la caja supera `max_auto_gusset_mm` (1 200) o el área dobla la de los
  rectángulos, aviso **`OUTLINE_AUTO_LARGE`** (una barra lejos o un margen absurdo: Convertir en polígono y recortar).
- **Dónde se usa.** El validador resuelve `auto` → polígono antes de la regla 9 (fuera de cartela, choques, cruces) y devuelve
  los puntos en `ValidationResult.AutoOutline`; `width_mm` y `height_mm` de `auto` se comparan con la caja calculada
  (`LABEL_VALUE_MISMATCH` como aviso si difieren más de la tolerancia). El croquis dibuja los puntos calculados y lo dice en la
  etiqueta ("cartela … · automática"). `conn_validate`, `conn_preview` y `conn_catalog_apply` devuelven
  **`calculated_values.outline_points_mm`** (más `outline_mode`, `outline_width_mm`, `outline_height_mm`, `outline_vertices`;
  `null` con `polygon`), y la previsualización lista la cartela con `outline_mode` y sus puntos. **La fabricación** usa los mismos
  puntos (`AutoOutline.ResolvePoints` en `ConnectionCreationService`): lo que se valida es lo que se crea.
- **Conectar y el panel.** El arranque sin plantilla (`SpecStarter.FromNode`) escribe `{"mode": "auto"}` en vez del rectángulo de la
  Fase 11 (D4: los márgenes 40 / 30 / 40 de `choices.json` solo se escriben si difieren de los del Core, para que los tokens no
  dependan del PC); `starter.outline_mode: "polygon"` recupera el rectángulo. La sección **Cartela** del panel tiene el desplegable
  **Contorno: Automático / Polígono** (`gusset.outline.mode` en `ListFields`): Polígono **convierte** (escribe los puntos calculados
  en `points_mm` y la caja en ancho y alto); Automático quita los puntos. En **Avanzado**, el cuadro del contorno enseña los puntos
  calculados en solo lectura y el botón **Convertir en polígono** hace lo mismo; con `auto`, editar los puntos o estirar las cotas
  da el error "La cartela es automática…: pulsa Convertir en polígono". El JSON sigue siendo la única fuente de verdad
  (`SpecEditor.TrySetOutlineMode` / `ConvertAutoToPolygon`; nunca se escriben los puntos calculados sin pedirlo).
- **Plantillas (D3).** Una plantilla guardada desde `auto` conserva el modo (sin puntos) y casa con **`auto_angle_tolerance_deg`**
  (25°, `catalog.json`) en vez de los 10° de siempre; al aplicarla, la cartela se recalcula con las barras del nudo
  (`TemplateAutoTests`: 12° de desvío casa, el contorno recalculado valida, la miniatura se dibuja). El Detalle D sigue en `polygon`.
- **Plan (D2).** Los nudos con cartela sin plantilla que encaje salen **"● Listo (arranque)"** / **"▲ Listo con aviso (arranque)"**
  con la cartela automática y el aviso `TEMPLATE_NO_MATCH` en su detalle ("se creará con la cartela automática…"); la cabecera los
  cuenta aparte: "… N cartelas automáticas (arranque) …". Con el catálogo vacío, la cercha de la 8c ya no sale toda `no_match`.

### 1.2 Partir la diagonal (sección 3.3; decisiones D5 a D9)

- **Dónde se corta** (`Geometry3D/SplitPoint`, Core): la diagonal llega al cordón con θ; su eje cruza la **cara** a
  `(ancho del cordón en el plano / 2) / |sen θ|` de su corte con el eje del cordón (medido desde el corte propio de la barra, con el
  desvío de eje); desde ahí, por el eje de la barra, **`offset_from_chord_face_mm`** (330, `diagonal_splice_offset_mm`) hasta la
  **primera plancha** (D7) y el corte de los dos elementos en el **centro de la pila** (+ espesor + `gap`/2). En el nudo en K del
  Hangar (θ 44,4°, cordón 203,2 en el plano): cara a 145,2 mm, corte a **≈ 485 mm** del corte de los ejes (`SplitPointTests`).
  `conn_get_node_info` lo devuelve por barra (`split_point_mm`, `split_distance_mm`, `split_offset_from_chord_face_mm`; `null`
  en el cordón, en un empalme o si la barra es paralela). El **tramo corto** (del lado del cordón) conserva el id (D6): el nudo en
  K escrito antes de partir sigue valiendo; el tramo largo es el elemento nuevo.
- **Cómo se parte en Revit** (`Fabrication/MemberSplitter`, **NO PROBADO**; lo decide el **sondeo 30**, sección 2.1): (a)
  `ElementTransformUtils.CopyElement` en el sitio y `LocationCurve.Curve` en los dos (original = tramo corto, copia = tramo largo),
  extensiones a 0 en el corte, `StructuralFramingUtils.DisallowJoinAtEnd` en los dos extremos del corte, comparación de **todos** los
  parámetros de instancia de la copia con los del original (los que difieren se copian a mano; si no se puede, aviso
  **`SPLIT_PARAMETERS_APPROXIMATED`** con la lista); (b) de reserva, si (a) lanza, `Document.Create.NewFamilyInstance(curva,
  símbolo, nivel, Beam)` copiando `STRUCTURAL_BEND_DIR_ANGLE`, las justificaciones, los desfases y las extensiones. Todo compila
  contra la API 2027 (Nice3point 2027.2.0); **ningún miembro se da por bueno hasta que el sondeo lo liste**. Si el punto de corte
  cae fuera de la barra o Revit no copia, **`SPLIT_FAILED`** con rollback completo (la barra sigue entera).
- **Registro y deshacer (D8).** `SplitRecord` (barra, tramo nuevo, cordón, curva original, extensiones originales, punto y
  distancia del corte, mecanismo, parámetros aproximados, `connection_id` de quien lo pidió o `null`, `batch_id`, fecha) en
  Extensible Storage sobre `DataStorage` propios con un esquema **nuevo** (`MotorConexionesSplit`, GUID fijo; ver 4.1).
  `conn_unsplit` (`split_id` o `element_id` de cualquiera de los dos tramos; sin nada, lista los cortes) borra el tramo nuevo y
  devuelve la curva y las extensiones al original, **solo si ninguna conexión usa los tramos** (`SPLIT_IN_USE` con los ids); si la
  barra no recupera exactamente su curva, aviso `SPLIT_UNDO_PARTIAL` (la original nunca se borra). `conn_delete` de una conexión
  que pidió cortes (la cadena de Conectar) los deshace después, en orden inverso; `conn_update` no (la conexión se rehace sobre
  los mismos tramos). **`SPLIT_NOT_FOUND`** si no hay corte.
- **Herramientas y rutas.** `conn_split_member` (`/conn/split/`: `element_id`, `chord_element_id`, `offset_from_chord_face_mm`
  opcional, `dry_run` para devolver el punto sin cortar, `connection_id` / `batch_id` opcionales; **sin `validation_token`**
  porque no es una especificación) y `conn_unsplit` (`/conn/unsplit/`). La respuesta de `split` trae `split`, `preview` (θ, cara,
  primera plancha, corte, punto, tramos corto y largo), `description` y `next`. En la cinta, el botón **Partir barra**: con el
  cordón y la diagonal seleccionados, un cuadro con un enlace por diagonal ("Partir la barra 1249631 … corte a 485 mm") y "Otra
  distancia…" (el único diálogo con entrada: la distancia); con un tramo partido seleccionado, deshace el corte.
- **Conectar (D9).** Con una especificación `direct_weld` con `splice_hint`, la previsualización enseña la casilla **"Partir la
  diagonal N y crear el empalme del Detalle B a 330"** (marcada por defecto; también desde Ejecutar especificación JSON). Con ella,
  Crear corre `Services/SplitChainService` dentro de un `TransactionGroup` exterior: por cada diagonal, **partir** (su propio
  `OperationScope`, `connection_id` = el del nudo en K) y crear el **empalme B** sobre los dos tramos (arranque `end_plate` del
  perfil del tramo, o la plantilla del catálogo que case: `SpecStarter.ForSplice`, validado contra el modelo con la misma regla que
  `conn_validate`); después el **nudo en K** sobre el tramo corto, **vuelto a validar** (la diagonal cambió de curva y su token
  con ella). Si cualquier paso falla, se revierte **todo** y el cuadro lo dice; si va bien, el cuadro enseña las tres cosas. Sin la
  casilla, como hoy.
- **Plan y lote (D5).** `PlanBuilder` añade a cada nudo en K con `splice_hint` un paso `split` por diagonal (`split_steps[]`:
  barra, cordón, distancia, punto, θ, `splice_node`, error si no se puede) y, tras él, un nudo **previsto** `N5B` (`N5B2` con dos)
  de clase `splice`, estado **`after_split`** ("● Empalme B (tras partir)" / "✖ Empalme B no valida"), con el arranque B validado
  con los extremos calculados antes de partir y un id provisional negativo para el tramo largo; no lleva etiqueta ni marca ni
  token. `BatchRunner.StepsFor` ordena **cortes → empalmes B → `N9E` → `N9`**; en el lote (`BatchCreator`) cada corte es un
  `OperationScope` anidado con `batch_id` = `plan_id`, el empalme B se crea con el id real del tramo largo y, antes del nudo en K,
  su token se **renueva** contra el modelo (`BatchStepHandlers.Revalidate`; sin él daría `VALIDATION_TOKEN_INVALID`, porque el
  token resume la curva de la barra). Si un corte falla, el nudo en K no se crea en esa pasada y el informe lo dice. **Borrar el
  lote** borra las conexiones y después deshace los cortes del lote (los más recientes primero; `unsplit_count`, "N cortes de
  diagonal deshechos"); en el plan, los empalmes B vuelven a `after_split` y los cortes a no hechos. La cabecera: "Además se parten
  N diagonales a 330 (Detalle B) y se crean N empalmes B antes de sus nudos en K". `conn_batch_plan` admite `split_diagonals: false`
  y `conn_batch_create` `split_members: false` (plan B: el lote como en la 12). El plan y el informe llevan `split_step_count`,
  `after_split_count`, `split_count`, `unsplit_count` y por nudo `split_steps`, `split_of_node`, `kind`, `element_id`,
  `new_element_id`, `split_id`. En la ventana del plan, los pasos salen en el **detalle** del nudo en K y los empalmes B como filas
  propias (ver 4.4).

### 1.3 Configuración, MCP, simulador y sondeo (sección 3.5)

- `config/choices.json`: `starter.gusset_side_margin_mm` (30) y `starter.outline_mode: "auto"`. `config/limits.json`:
  `max_auto_gusset_mm` (1 200) y `auto_outline_max_vertices` (12). `config/catalog.json`: `auto_angle_tolerance_deg` (25).
- `mcp/revit_mcp/conexiones.py` 0.13.0 (27 rutas), `mcp/tools/conn_tools.py` 0.13.0 (25 herramientas, manuales en español),
  `mcp/pruebas/simulador_revit.py` 0.13.0 (imita `split` / `unsplit` con una barra ficticia nueva de la misma sección, el
  contorno automático, `split_point_mm`, los pasos del plan y el lote con cortes; **101 comprobaciones**),
  `mcp/pruebas/probar_conexiones.py` (**46 pruebas**: 31 a 33b nuevas). `mcp/CONTRATO-conn.md` y `docs/guide.md` al día.
- `scripts/sondeos/30-partir-barra.py`: lo de 3.3 del prompt, con `-SinTransaccion`, cancela al final salvo
  `%TEMP%\sondeo30-commit.txt`.

## 2. Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test src/MotorConexiones.Tests -c Release --nologo → Passed! 311/311 (291 de la 0.12.4 + 20 de Fase13Tests.cs)
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py; ast.parse de scripts/sondeos/*.py
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 101/101 correctas
$ python3 mcp/pruebas/probar_conexiones.py (contra el simulador en 48884, fixtures de docs/fixtures) → Resultado: 46/46
```

- **`AutoOutlineTests`** (`Fase13Tests.cs`): la caja del Detalle D a menos del 15 % de 565 × 530, ranuras y placa dentro, 16 nudos
  de la 8b con la plantilla en `auto` sin errores, ±15° (y el control `PLATE_OUTSIDE_GUSSET` del polígono a +15°), el K sintético
  envuelto, espejo, `margin_mm` 0 y `beyond_chord_mm` 1 500 → `OUTLINE_AUTO_LARGE`, `auto` + `points_mm` → `SCHEMA_INVALID`,
  croquis y miniatura, envolvente y simplificación.
- **`Fase13RegressionTests`**: 21 tokens de la 0.12.4 (volcados con el código anterior en un worktree) iguales con la 0.13.0; la
  cercha de la 8c da los mismos tokens con y sin `SplitDiagonals` (mismo `PlanId`).
- **`SpecEditorOutlineTests`**, **`TemplateAutoTests`** (tolerancia 25, 12° casa, `config/catalog.json` del repositorio),
  **`SplitPointTests`** (≈ 485, θ 44,4, cara 145,2), **`Fase13PlanTests`** (31 pasos y nudos previstos en la 8c, el orden de
  `StepsFor`, `Run` con manejadores falsos: todo en orden, rollback con `stop_on_error`, fallo parcial, sin partidor; el plan de la
  Fase 12 sin cambios con `SplitDiagonals = false`). `Fase11Tests` y `Fase12Tests` ajustadas a D2 ("(arranque)", 15 listos con el
  catálogo vacío) y al plan con pasos.
- **Simulador**: rutas `split` / `unsplit` hasta `Bridge.Handle`, `validate` / `preview` / `catalog/apply` con `outline_points_mm`
  (múltiplos de 5), `auto` + puntos → `SCHEMA_INVALID`, `OUTLINE_AUTO_LARGE`, plantilla `auto` con tolerancia 25, `node_info` con
  `split_point_mm`, `split` en seco y real, segundo `split` → `SPLIT_IN_USE`, `unsplit` por tramo nuevo, `SPLIT_NOT_FOUND`, plan
  del nudo en K con 2 pasos y `N1B` / `N1B2`, lote del nudo en K (split, split, splice_b, splice_b, node; token renovado),
  `unsplit` con el empalme creado → `SPLIT_IN_USE`, Borrar el lote con `unsplit_count` 2 y la diagonal entera otra vez, el plan
  tras borrar; con el catálogo vacío el Detalle D sale "▲ Listo con aviso (arranque)" con `outline.mode auto`.

### 2.1 Lo que se probó en Revit con `docs/instalacion/fase-13.md` (hecho el 2026-10-31; el contraste, en 7.1)

| Qué | Dónde | Lo que se espera |
|---|---|---|
| El filtro de la 0.12.4 (`fase-12.md` 10.3) | 13-3: Conectar en el nudo en K **sin** la casilla, Crear | Solo `END_CUT_APPROXIMATED` en el cuadro; dos `revit_failure` con `filtered: true` en el log |
| **Sondeo 30** | 13-4 | (a) `CopyElement` + `LocationCurve.Curve` conserva tipo, nivel, giro, justificación y desfases (la lista de parámetros distintos vacía o solo longitudes); `DisallowJoinAtEnd` existe; `node_info` de los dos tramos `kind: splice`; el del nudo en K con la misma excentricidad; deshacer deja la barra con su longitud; 0 elementos tras cancelar |
| Cartela automática en Conectar | 13-5 | Previsualización con la cartela calculada (chaflanes), Contorno: Automático, Convertir en polígono y editar una cota; Crear con los puntos calculados; `delete` |
| Plantilla `auto` | 13-6 | Guardar en catálogo desde `auto`; en otro nudo del cordón central, propuesta y cartela recalculada |
| La cadena de Conectar (D9) | 13-7 | Crear con la casilla: cuadro con corte, empalme B y nudo en K; `list` 2; `node_info` de los dos tramos; `delete` de las dos y la diagonal vuelve a ser una (`SPLIT_IN_USE` si se borra el nudo en K antes que el empalme) |
| `conn_split_member` y `conn_unsplit` | 13-8 | `dry_run` ≈ 485; real con `new_element_id`; `node_info`; `unsplit` `complete: true`; sondeos 12 y 13 a cero |
| El plan con los pasos | 13-9 | `split_steps` en los 15 nudos en K, 30 nudos `N…B` `after_split`; **no crear el lote entero** |
| Puente | 13-10 | `--puente` con las 25 herramientas |

### 2.2 NO PROBADO en la nube y por qué

- **Todo lo que toca Revit**: `MemberSplitter` (los dos mecanismos, la comparación de parámetros, `DisallowJoinAtEnd`, el
  deshacer), `SplitStorageManager` (el esquema nuevo de Extensible Storage), la cadena de Conectar y sus cuadros, el botón Partir
  barra y su cuadro de distancia, el panel Contorno y Convertir en polígono en WPF, la fabricación de la cartela con los puntos
  calculados en Advance Steel, el lote con cortes y su deshacer, `node_info` con `split_point_mm` sobre el modelo. No hay Revit ni
  MCP en la nube; compilan contra la API 2027 y se prueban con `docs/instalacion/fase-13.md`.
- **El puente MCP** (`conn_tools.py` con el SDK `mcp` en el `.venv` del PC): `py_compile` sí; `--puente`, en el PC.
- **La geometría exacta del Hangar**: los 485 mm del punto de corte salen de los `Fakes` con los ejes del fixture K; en Revit, con
  la geometría real, puede diferir unos milímetros (el 330 desde la cara es lo que fija el plano).

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. En el **sondeo 30**: si (a) conserva los parámetros (la lista de distintos vacía) → `MemberSplitter` queda como está; si
   solo difieren el giro o la justificación → la 0.13.0 ya los copia a mano (aviso `SPLIT_PARAMETERS_APPROXIMATED` si no puede);
   si (a) lanza → mirar (b); si tampoco → ronda 13b con lo que el sondeo liste (3.4 del prompt). Si `DisallowJoinAtEnd` no existe
   con ese nombre, la 0.13.0 lo salta con aviso y hay que mirar las uniones automáticas en el corte.
2. En la **cadena**: que el empalme B salga con las dos planchas y los pernos del Detalle B (185, 4 pernos Ø5/8") sobre el tramo
   corto y el nuevo, y el nudo en K con sus soldaduras sobre el tramo corto; el cuadro final y el log (`ribbon_split_chain`).
3. En **`delete`**: borrar el empalme B y después el nudo en K deshace el corte (el nudo en K es quien lo pidió); al revés,
   `SPLIT_IN_USE` como aviso y el corte se queda hasta borrar el empalme.
4. La **cartela automática** del cordón central: chaflanes razonables, nada de `OUTLINE_AUTO_LARGE`; la caja del Detalle D en
   Revit frente al 565 × 530 del plano.

## 4. Decisiones tomadas y por qué

### 4.1 Un esquema de Extensible Storage nuevo para los cortes (desvío de la sección 3.6 del prompt)

El prompt pedía "el mismo esquema con un campo nuevo y versión +1". Un esquema de Extensible Storage **no admite campos nuevos con
el mismo GUID** (habría que crear otro GUID y migrar), así que los cortes van en un esquema propio (`MotorConexionesSplit`, GUID
`3D7E9A1C-…`) sobre `DataStorage` propios, y el esquema de las conexiones no se toca: los registros de las fases anteriores se leen
sin cambios. El registro va entero en JSON (`RecordJson`) más dos campos de búsqueda (`SplitId`, `ElementIds`).

### 4.2 La cadena de Conectar no reutiliza `BatchRunner`

El prompt decía "Crear corre un `BatchRunner` de tres pasos". `BatchRunner` trabaja sobre un plan con nudos y tokens; Conectar tiene
una sesión de previsualización. `SplitChainService` hace los mismos tres pasos con las mismas garantías (grupo exterior, un
`OperationScope` por paso, rollback completo) y reutiliza `RibbonCreation.CreateValidated`, `SplitService` y `SpecStarter.ForSplice`;
el lote usa `BatchRunner` con los manejadores de `BatchStepHandlers`. Menos código y sin un plan ficticio.

### 4.3 El token del nudo en K se renueva después del corte

El token resume la curva de cada barra (`ValidationTokenGenerator`): al partir la diagonal, el token del nudo en K calculado con la
barra entera deja de valer y `conn_create` lo rechazaría. En el lote, `BatchStepHandlers.Revalidate` vuelve a validar la
especificación tras el corte y `BatchRunner` usa el token nuevo (y restaura el viejo si el lote se revierte); en Conectar, la sesión
se revalida (`PreviewSession.Refresh`) antes de crear el nudo. El token del plan se comprueba igual que antes, pero en el paso de
corte. No estaba en el prompt; sin esto la cadena no puede funcionar.

### 4.4 Los pasos de corte en el detalle del nudo y los empalmes B como filas

El prompt pedía una línea indentada en la tabla por paso. La tabla de la ventana del plan es una lista de nudos; los empalmes B
previstos **son** nudos (filas propias, "● Empalme B (tras partir)", sin marca ni etiqueta) y los pasos de corte salen en el
**detalle** del nudo en K ("· Partir 1249631 a 330 (Detalle B): corte a 485 mm … → empalme N5B"). Es el punto 1 del orden de recorte
de la sección 10 aplicado solo a la presentación; la lógica (orden, creación, deshacer) está entera.

### 4.5 Los márgenes del arranque solo se escriben si no son los del Core

`SpecStarter` escribe `margin_mm`, `side_margin_mm` y `beyond_chord_mm` solo cuando `choices.json` difiere de 40 / 30 / 40; así el
JSON del arranque (y su token) no depende de la copia de `choices.json` del PC, y una especificación `auto` mínima es `{"mode": "auto"}`.

### 4.6 La franja del cordón no añade margen en X

La franja del cordón cubre exactamente la extensión en X de los rectángulos de las barras (más `beyond_chord_mm` en Y). Con margen
en X la caja del Detalle D superaba el 15 % del plano; sin él queda en ≈ 610 × 590 y las barras siguen dentro con su margen propio.

### 4.7 `conn_update` no deshace los cortes

Rehacer una conexión (`update` = borrar + crear con los mismos ids) no debe tocar los tramos: `DeleteConnection(undoSplits: false)`
en ese camino; `conn_delete` y Borrar sí deshacen.

## 5. Pendientes, riesgos y preguntas

- **El sondeo 30 manda** sobre `MemberSplitter`: hasta tenerlo, la fabricación del corte es la reserva programada (3.4 del prompt).
  Lo que diga va a la ronda 13b si hace falta.
- **Dos elementos por nudo en K**: la cercha de la 8c partiría 31 diagonales; la instalación no crea el lote entero (13-9) hasta que
  la cadena de un nudo haya funcionado en Revit.
- **Replanificar después de partir**: el detector ve los dos tramos como `splice` (el empalme B) y el nudo del cordón central ve el
  tramo largo; probado en el simulador, no en Revit.
- **`OUTLINE_AUTO_LARGE` en nudos abiertos**: solo envolvente convexa (D1); Convertir en polígono y recortar a mano.
- **El puente y la IA**: las dos herramientas nuevas tienen su manual; falta verlas por el puente (`--puente`).
- Fuera de alcance (sección 8 del prompt): contornos cóncavos, el corte inclinado, partir cordones desde el plan, `to_support`,
  `base_plate`, el cordón inferior (D10).

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. Dar `docs/instalacion/fase-13.md` al agente instalador (**el filtro de la 12d y el sondeo 30 primero**).
2. Hacer los pasos marcados **(la persona)** cuando el instalador lo pida (Conectar en el cordón central, la plantilla `auto`, la
   cadena en el nudo en K, el plan) y las capturas `fase13-01` a `fase13-04`.
3. Devolver `docs/fases/resultados-fase-13.md` y el log del día; con eso, el cierre de la fase (sección 7 de este informe) y, si el
   sondeo 30 lo pide, la ronda 13b.

---

## 7. Cierre de la Fase 13 (2026-10-31): la 0.13.0 contrastada en Revit y el add-in 0.13.1

El instalador ejecutó `docs/instalacion/fase-13.md` el 2026-10-31 (`docs/fases/resultados-fase-13.md`, capturas `fase13-01-auto`,
`fase13-02-poligono`, `fase13-03-cadena` y `fase13-04-plan`; commit `8dba21d` en `main`). Esta sección contrasta cada paso con lo
esperado en 2.1, dice qué no coincidió y qué se hizo con ello, y deja la 0.13.1 lista para la ronda 13b.

### 7.1 Contraste con lo esperado en 2.1

| Paso | Esperado (2.1) | Resultado en Revit |
|---|---|---|
| 13-1 | Build sin avisos, 311 pruebas, 0.13.0 desplegada, cinco claves nuevas | **Como se esperaba**: `0 Advertencia(s)`, `Superado: 311`, `MotorConexiones 0.13.0.0 desplegado en Revit 2027`, `instalar-conn` sin cambios en `startup.py`, `outline_mode`, `gusset_side_margin_mm`, `max_auto_gusset_mm`, `auto_outline_max_vertices` y `auto_angle_tolerance_deg` en los tres `config\*.json`. |
| 13-2 | `ping` 0.13.0 con `split` y `unsplit`, `list` 0, `splits_count` 0 | **Como se esperaba** (27 operaciones). Los sondeos 12 y 13 **no corrieron**: ver 7.2.3. |
| 13-3 El filtro de la 12d | Solo `END_CUT_APPROXIMATED`; dos `revit_failure` con `filtered: true` | **Como se esperaba**: "Conexión modelada correctamente", 2 elementos, 2 retiros, `Advertencias: END_CUT_APPROXIMATED`; en el log dos `revit_failure` ("The created elements are only visible in Detail Level: Fine.", elementos 1322493 y 1322494, definición `73c984d7-…`, `filtered: true`). **La comprobación pendiente de `fase-12.md` 10.3 queda hecha.** |
| 13-4 Sondeo 30 | (a) conserva los parámetros, `DisallowJoinAtEnd` existe, `node_info` de los tramos, deshacer, `[]` | **No corrió**: `PROBE_EXCEPTION: TypeError: Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64)` en `barra()` (línea 65). Es el mismo tropiezo del sondeo 19 v3: en IronPython `DB.ElementId(1249631)` es ambiguo y hay que escribir `DB.ElementId(System.Int64(...))`. Lo que venía a decidir lo decidieron los pasos 13-7 y 13-8 (ver 7.2.2). |
| 13-5 Cartela automática | Previsualización con la cartela calculada, Contorno, Convertir en polígono, Crear, `outline.mode auto` sin `points_mm` | **Como se esperaba**: la cartela con chaflanes, Contorno Automático → Polígono (cota editada) → Automático, Crear con **7 elementos**; `get` con `spec.gusset.outline.mode: "auto"` y sin `points_mm`. La persona olvidó la captura del cuadro de Crear (las `fase13-01` y `fase13-02` sí están). |
| 13-6 Plantilla `auto` | Guardar desde `auto`; propuesta y cartela recalculada en el nudo vecino | **Como se esperaba**: `Cartela automatica cordon central` guardada; en el otro nudo del cordón central Conectar propuso "Cartela automatica cordon central · en espejo (mirror_x)" con la cartela recalculada (7 vértices) y Contorno: Automático; `catalog_get` con `outline.mode: "auto"` y `matching.angle_tolerance_deg: 25`; conexión y plantilla borradas. |
| 13-7 La cadena (D9) | Cuadro con corte, empalme B y nudo en K; `list` 3; `node_info` de los tramos; `SPLIT_IN_USE`; `unsplit` | **Como se esperaba**: "Diagonal partida, empalme B y nudo en K modelados": 1249631 → tramo nuevo 1322575 a **485 mm**, 1249632 → 1322592 a **483 mm**, dos empalmes B de 8 elementos, nudo en K con 2 elementos y 2 retiros, una sola entrada de deshacer; `list` 3 (2 `end_plate`, 1 `direct_weld`); `unsplit` lista 2 cortes con `connection_id` del nudo en K, `mechanism: copy_element`, `approximated_parameters: []`; `node_info` de los tramos `kind: splice` (416,1 mm el corto, ya con el retiro del nudo en K, y 3 083,9 el nuevo) con `existing_connections`; borrar el nudo en K primero → dos `SPLIT_IN_USE` ("lo usan las conexiones … (end_plate): no se deshace") y 2 barras restauradas; tras borrar los empalmes, `unsplit` × 2 `complete: true` ("la barra … recuperó su curva"). **Distinto**: `Advertencias: REVIT_WARNING; BOLT_NUTS_APPROXIMATED; END_CUT_APPROXIMATED` (el `REVIT_WARNING` es el de 7.2.1). |
| 13-8 `split` / `unsplit` | `dry_run` ≈ 485, real con `new_element_id` y `mechanism`, `node_info`, `unsplit` `complete` | **Como se esperaba**: `dry_run` con `split_distance_mm 483,2`, `theta_deg 45`, cara 143,7, primera plancha 473,7, tramos 521,6 + 3 024,4; el corte real `mechanism: copy_element`, `approximated_parameters: []`, `new_element_id 1322620`; `node_info` de los tramos `kind: splice`; el del nudo en K `eccentricity_mm 54,9` y `split_point_mm` por diagonal; `unsplit` `complete: true`. **Distinto**: `REVIT_WARNING: Advertencia de Revit: Beam or Brace is slightly off axis and may cause inaccuracies. [elementos: 1322620]` (el tramo nuevo; 7.2.1). |
| 13-9 El plan | `split_steps` en los nudos en K, nudos `N…B` `after_split`, "30 diagonales", N1B / N1B2 | **Como se esperaba, con dos erratas de la instalación**: la ventana del plan de la cercha dijo "Además se parten **31** diagonales a 330 (Detalle B) y se crean 31 empalmes B" (el Core calcula 31: `Fase13PlanTests`; el log del plan de 64 barras: `ready 41, after_split 31, no_match 8, untyped 17`); por el adaptador, el plan de tres barras dio `split_step_count 2`, `after_split_count 2`, cortes a 484,8 y 483,2 con `splice_node` **N3B / N3B2** (no N1B: las barras sueltas también numeran, el nudo en K es N3) y el texto "Además se parten 2 diagonales…"; `discard` limpio y **sin pulsar Crear**. |
| 13-10 Puente | `49/49`, 25 herramientas, sin `*_failed` | **Como se esperaba**: `Resultado: 49/49 pruebas correctas`, `tools/list` con las 25 `conn_*` (`conn_split_member` y `conn_unsplit` entre ellas); en el log solo el `batch_create` con `VALIDATION_TOKEN_INVALID` de la prueba del token alterado (previsto). **Distinto**: la primera ejecución de `--puente` falló porque el servidor MCP (`main.py`, puerto 8000) no estaba arrancado; el instalador lo arrancó con `C:\IA\iniciar_servidor_revit.bat` y repitió (7.2.4). |

En resumen: **todo lo que la Fase 13 programó funcionó en Revit a la primera**; lo que no coincidió son el sondeo (roto), dos
advertencias de Revit sobre el tramo nuevo y erratas de la instalación.

### 7.2 Lo que no coincidió y qué se hace con ello

#### 7.2.1 La advertencia "Beam or Brace is slightly off axis" del tramo nuevo (add-in 0.13.1)

En los dos cortes por el adaptador (13-8, elementos 1322620 y 1322779) y en la cadena (13-7) Revit avisó de que el **tramo nuevo**
está "ligeramente fuera de eje". Lo que se sabe: el punto de corte que el Core calcula en el marco del nudo (`SplitPoint.Compute`:
`cut + direction × split`, en el plano de la cercha) quedó **0,011 mm fuera del plano de la diagonal** (`split_point_mm` Y =
−17 195,789 frente a −17 195,8 de sus dos extremos), porque el plano del nudo pasa por el cordón y el cordón del Hangar no es exactamente
paralelo (su Y va de −17 195,8 a −17 195,7 en 10 m). Pero `SplitPoint.Segments` **proyecta** ese punto sobre el eje de la barra antes
de devolver los tramos (la prueba `Segments_PutTheSplitPointOnTheBarAxis_EvenIfTheFramePointIsOffPlane` lo fija con los números reales
de la diagonal 1249632) y `RevitGeometry.ToMm` / `ToFeet` no redondean: la 0.13.0 ya dejaba los dos tramos sobre el eje original salvo
el error de coma flotante del viaje pies → mm → pies. Quedan dos causas posibles: (a) Revit repite sobre el **elemento nuevo** (lo crea
`CopyElement`) una comprobación que la diagonal original ya incumplía (si ella misma se sale un poco de un plano vertical, como el
cordón); (b) ese viaje de unidades. **Decisión**: la 0.13.1 elimina (b): el punto de corte se proyecta con `Line.Project` sobre la
`Line` **real** de Revit y los dos tramos se construyen con los **extremos exactos del original en pies** (`MemberSplitter.Split`), y el
registro guarda `original_start_feet` / `original_end_feet` para que `Unsplit` restaure la curva exacta (los registros de la 0.13.0 se
restauran desde los mm). Una barra que no sea recta da ahora `SPLIT_FAILED` con un mensaje claro en vez de enderezarse. Para (a), el
**sondeo 30 v2** mide el desvío de eje de la diagonal original (`dY` entre sus extremos, el ángulo en planta respecto a X y el ángulo
fuera del plano vertical) y el paso 13b-4 repite el corte: si con la 0.13.1 el `split` sale **sin** `REVIT_WARNING`, era (b) y está
resuelto; si sigue, es (a), la advertencia es de la geometría del modelo (informativa, como la de la 12d) y se decidirá con la persona si
se filtra por el Guid de su definición (la línea `revit_failure` del log lo trae) o se deja pasar. No se filtra nada a ciegas.

#### 7.2.2 El sondeo 30 no corrió; el mecanismo del corte lo decidió el add-in

`barra()` usaba `DB.ElementId(eid)` con un entero, ambiguo en IronPython (tres constructores). Corregido en la **v2**
(`DB.ElementId(System.Int64(int(eid)))`), que además mide el desvío de eje (7.2.1) y proyecta el punto de corte sobre la línea real,
como la 0.13.1. Lo que el sondeo venía a decidir ya está decidido por los pasos 13-7 y 13-8: `CopyElement` + `LocationCurve.Curve`
**conserva los parámetros** (`approximated_parameters: []` en los cuatro cortes), `DisallowJoinAtEnd` existe y no avisó, `node_info` ve
los dos tramos como `splice` con la misma excentricidad del nudo en K (54,9) y el deshacer deja la barra con su curva (`member_unsplit`
con `problems: []`). **`MemberSplitter` se queda con (a)**; la reserva (b) `NewFamilyInstance` sigue en el código por si alguna familia
no se deja copiar, sin coste. El sondeo 30 v2 se ejecuta en la 13b solo como diagnóstico (3 minutos).

#### 7.2.3 Erratas de `docs/instalacion/fase-13.md`

- Los sondeos 12 y 13 se llamaban `12-estado-barras.py` y `13-limpiar-restos.py` y **no existen**: son `12-fase3-borrar.py` y
  `13-limpiar-fase1.py` (como en la 12d). Por eso ninguna "vuelta a cero" de las diagonales se comprobó con el sondeo 12; se sabe por
  `unsplit` (`complete: true`) y porque el `dry_run` posterior sobre 1249632 volvió a dar la barra entera (521,6 + 3 024,4 mm).
- `$pid` en el paso 13-9 es de solo lectura en PowerShell (ya pasó en la 8e): es `$plan`.
- "30 diagonales" → 31; "N1B / N1B2" → `NxB / NxB2` (N3B en el plan de tres barras).
- La ruta del log es `%LOCALAPPDATA%\MotorConexiones\log\motorconexiones-AAAAMMDD.jsonl` (`JsonLineLogger`), no `logs\…-AAAA-MM-DD`.

El archivo queda corregido y anotado arriba como "ejecutada el 2026-10-31"; la 13b usa los nombres y rutas correctos.

#### 7.2.4 El puente no estaba arrancado

El paso 13-10 no traía el bloque que arranca `main.py` (el 12d-6 sí). El instalador lo resolvió por su cuenta con
`C:\IA\iniciar_servidor_revit.bat` y el puente dio 49/49. El 13b-6 lleva el bloque.

### 7.3 Qué cambia en el repositorio (add-in 0.13.1)

- `src/MotorConexiones.Revit/Fabrication/MemberSplitter.cs`: la barra debe ser una `Line` (si no, `SPLIT_FAILED` "no es recta"); el
  punto de corte se proyecta sobre esa línea en pies; los dos tramos se construyen con los extremos exactos del original; el registro
  lleva los extremos en pies; `Unsplit` restaura desde los pies (o desde los mm si el registro es de la 0.13.0).
- `src/MotorConexiones.Core/Storage/SplitRecord.cs`: `original_start_feet` y `original_end_feet` (opcionales). El contrato solo añade
  (`mcp/CONTRATO-conn.md`); el simulador los devuelve también.
- `src/MotorConexiones.Tests/Fase13CierreTests.cs` (3 pruebas): el registro de la 0.13.0 sin pies sigue cargando, el de la 0.13.1 va y
  vuelve con los pies exactos, y `Segments` deja el punto de corte sobre el eje de la barra con los números reales de la diagonal 1249632.
- `scripts/sondeos/30-partir-barra.py` v2 (7.2.2). `docs/instalacion/fase-13.md` corregida y anotada; `docs/instalacion/fase-13b.md` nueva.
- Versión **0.13.1** en el add-in, el adaptador (27 rutas), las herramientas (25), el simulador y el contrato. `CLAUDE.md`, README y
  las propuestas (`nudos-en-k-excentricos.md`, `catalogo-y-lotes.md`, `flujo-intuitivo.md`) al día.

### 7.4 Qué se probó en la nube y cómo

```
$ dotnet build MotorConexiones.sln -c Release     → Build succeeded, 0 Warning(s)
$ dotnet test src/MotorConexiones.Tests -c Release → Passed! 314/314 (311 + 3 de Fase13CierreTests)
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py; ast.parse de scripts/sondeos/*.py
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar    → Autocomprobación: 101/101 correctas
$ python3 mcp/pruebas/probar_conexiones.py (contra el simulador en 48884) → 46/46
```

**NO PROBADO en Revit** (no hay Revit en la nube): el camino nuevo de `MemberSplitter.Split` (`Line.Project`, los extremos exactos) y de
`Unsplit` (desde los pies), el sondeo 30 v2 y si la advertencia "slightly off axis" desaparece. Se prueba con `docs/instalacion/fase-13b.md`.

### 7.5 Decisiones del cierre

- **El corte con `copy_element` es definitivo** (7.2.2); el sondeo 30 deja de decidir y queda como diagnóstico.
- **No se filtra la advertencia "slightly off axis"** hasta saber si es de la barra (13b-4 y el `dY` del sondeo 30 v2). Si lo es, se
  propondrá a la persona filtrarla por el Guid de su definición como la de la 12d (queda en el log con `filtered: true`) o dejarla.
- **La ronda 13b no añade alcance**: solo despliega la 0.13.1, repite el sondeo 30 y los sondeos 12 y 13, corta y deshace una diagonal
  por el adaptador y pasa el puente. Lo que el prompt dejó fuera (sección 8 de `docs/prompts/fase-13.md`: contornos cóncavos, partir
  cordones desde el plan, `to_support`, `base_plate`, el cordón inferior) sigue fuera.
- **La Fase 13 queda cerrada** salvo esa comprobación de la 0.13.1.

### 7.6 Qué sigue

1. Dar `docs/instalacion/fase-13b.md` al instalador (empieza llevando la rama `claude/sweet-carson-pqqo3s` a `main`, paso 13b-0) y
   devolver `docs/fases/resultados-fase-13b.md`.
2. Sesión corta de cierre de la 13b: contrastar con 7.2.1 (¿desapareció el `REVIT_WARNING`? ¿qué `dY` tiene la diagonal?) y, si
   procede, la decisión sobre el filtro; con eso la Fase 13 queda cerrada del todo.
3. Después, la propuesta de la fase siguiente (sin código), como se hizo con la 12 y la 13.
