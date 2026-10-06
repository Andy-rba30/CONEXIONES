# Fase 8: detección de nudos y plan de lote (marcas en el modelo, sin crear nada)

Fecha: 2026-10-04 (ronda 8b: 2026-10-05). Rama: `main`. Add-in **0.8.0** en la Fase 8 y **0.8.1** en la ronda 8b (Core con
`Batch/`; add-in con `Batch/`, tres operaciones nuevas y el botón **Planificar lote** con su ventana); adaptador 0.8.1
(23 rutas) y 21 herramientas `conn_*`. Prompt y alcance: `docs/prompts/fase-8.md`, escrito a partir de las secciones
3.1, 3.2, 3.4, 4 y 6 de `docs/propuestas/catalogo-y-lotes.md` y de las decisiones de su sección 7.1 (P3, P6, P7, P8, P10,
P11, P12, P13).

**Estado: CERRADA el 2026-10-05 (sección 8); ronda 8c probada en el PC y cerrada el 2026-10-05 (secciones 9 y 10, add-in
0.8.4 con la ventana del plan no modal); ronda 8d probada en el PC el 2026-10-05 y cerrada el 2026-10-06 (sección 11, add-in
0.8.5: Cordón… ya no cierra Revit, pinchar con la ventana oculta, Descartar cierra la ventana, sondeo 19 v3); ronda 8e
probada en el PC el 2026-10-05 (puente repetido el 2026-10-06 con Revit abierto) y cerrada el 2026-10-06 (sección 12): la
0.8.5 funciona en Revit, la etiqueta del sondeo 19 se vio y se pinchó, y la Fase 8 queda cerrada del todo. El add-in sigue
en 0.8.5: el cierre de la 8e solo toca el sondeo 19 (v4).** Probada en el PC el 2026-10-04 (`resultados-fase-8.md`: marcas y ventana
bien, 0 nudos `ready` porque las diagonales reales terminan en la cara del cordón); corregida en la ronda 8b (sección 7,
add-in 0.8.1) y **confirmada en el PC el 2026-10-05** (`resultados-fase-8b.md`: 56 barras → 59 nudos, **16 `ready`** con la
plantilla oficial, 8 `same` y 8 `mirror_x`, replan con el mismo token, Editar nudo sobre un `ready`). El cierre corrige
cinco cosas pequeñas que enseñó la 8b (add-in **0.8.2** en el repositorio, **NO PROBADO en Revit**: se comprueba en la
instalación de la Fase 9, sección 8.5). Esta fase **no crea ninguna conexión**: planifica y marca; crear el lote es la
Fase 9. Las secciones 1 a 6 son el informe original de la Fase 8; la 7, la ronda 8b; la 8, el cierre.

---

## 1. Qué se hizo

- **Detección de nudos** (`Core/Batch/NodeDetector.cs`, sección 3.2 de la propuesta): los extremos de las barras se
  agrupan a menos de `node_cluster_mm` (10 mm, `config/catalog.json`); cada grupo es un nudo con punto de trabajo (media
  de los extremos), barras que llegan (un extremo en el grupo) y barras que atraviesan (eje a menos de
  `node_axis_max_distance_mm`, 5 mm, y punto dentro del tramo lejos de los extremos). Cordón = la barra que atraviesa;
  más de una → `ambiguous_chord`; ninguna → la más horizontal de las que llegan y `chord_continuous = false`. Marco
  canónico (`NodeFrame.Compute` con el cordón y la barra menos paralela a él) y ángulo con signo de cada barra; ejes que
  no se cortan → `offset`; una sola barra o todas paralelas → `untyped`. Nombres `N1, N2…` ordenados por la coordenada
  global de mayor extensión de la cercha (X en el Hangar), después las otras dos; estables para la misma selección;
  `NextName` para los nudos añadidos. `BuildNode` reconstruye un nudo con sus barras corregidas (cordón forzado, barras
  quitadas o añadidas) y se usa en todas las correcciones.
- **Correcciones** (`Core/Batch/BatchOverrides.cs`, sección 3.4): `exclude` / `include`, `add_node`, `chord`, `template`
  (nulo = sin plantilla), `remove_member` / `add_member`, `merge`, `split`, `spec` por nudo y `replace_existing` (P8).
  Se leen del JSON de la petición (clave desconocida → `INVALID_REQUEST`), se acumulan en el plan (`MergeWith`: lo nuevo
  manda por clave; `include` quita de `exclude`; `chord: null` olvida la corrección) y son las mismas para el MCP y para
  la ventana.
- **El plan** (`Core/Batch/BatchPlan.cs`): `plan_id`, documento, selección, plantillas, correcciones, nudos (`PlanNode`:
  estado, punto de trabajo, cordón, barras con ángulo y lado, firma, plantilla, orientación, casado, intentos,
  especificación, token, errores, avisos, conexión existente, color, marca) y marcas puestas; JSON de ida y vuelta.
  Paleta fija de 12 colores con nombre en español (`PlanPalette`).
- **Constructor del plan** (`Core/Batch/PlanBuilder.cs`): barras de la selección (y las que mencionan las correcciones)
  desde `IModelFacts` → detección → `merge`, `split`, `add_node`, `chord`/barras → nombres → por nudo: excluido, ya
  conectado (P8), plantilla fijada o todas las pedidas (varias plantillas, propuesta 3.6: gana la que casa todas sus
  ranuras con menor desvío; a igualdad, la primera), `TemplateMatcher` (cuatro orientaciones, P6), `TemplateInstantiator`
  con `source.batch_id` = `plan_id` (P13), y validación con un **delegado** (`PlanValidator`): en Revit,
  `ValidationService` (lo mismo que `conn_validate`); en las pruebas, `SpecValidator` con la cercha sintética. Una
  especificación editada a mano (`overrides.spec`) sustituye a la plantilla solo en ese nudo y se valida igual. Estados:
  `ready`, `invalid`, `no_match` (con `attempts`), `ambiguous_chord`, `offset`, `untyped`, `already_connected`,
  `excluded`. El punto de trabajo de un nudo añadido o separado sale de sus barras (`WorkPointFor`: el extremo que más
  barras comparten; a igualdad, el más cercano al nudo original).
- **Add-in**:
  - `Batch/PlanMarks.cs` (sección 3.4, opción A, P10): en la vista activa, `View.SetElementOverrides` con color de línea y
    de superficie (patrón sólido buscado por `IsSolidFill`), cordón con grosor 10 y barras con 6; marcador `DirectShape`
    de Modelos genéricos en el punto de trabajo (cubo de 160 mm para `same`, octaedro para las orientaciones en espejo,
    P6) con `ApplicationId` `MotorConexiones.Plan`, `ApplicationDataId` = plan y nudo, `Name` y `Marca` = `N…`, y en
    `Comentarios` el plan, la vista y los IDs coloreados. `Remove` restaura y borra; `RemoveAll` limpia marcadores
    huérfanos leyendo sus comentarios (planes que el add-in ya no recuerda). Vista que no admite overrides → aviso
    `PLAN_MARKS_SKIPPED` y plan sin marcas.
  - `Batch/PlanRegistry.cs`: planes en memoria (`plan_id` → plan) hasta descartar o cerrar Revit; `LastFor(documento)`
    para que el botón reabra el último.
  - `Batch/BatchPlanner.cs`: selección → `RevitModelFacts` → `PlanBuilder` → marcas, todo en un `OperationScope`
    (`MotorConexiones: batch_plan <plan_id>`: una entrada de deshacer; replanificar quita las marcas viejas y pone las
    nuevas en la misma); `Discard` y `DiscardAll`; conexiones existentes desde Extensible Storage; plantillas por id o
    nombre (`TEMPLATE_NOT_FOUND` si falta alguna pedida); `PlanToData` / `NodeToData` para las respuestas.
  - Operaciones `batch_plan` (`element_ids` o selección, `template_ids`/`template_id`, `overrides`, `plan_id`, `mark`,
    `reset_overrides`, `replace_existing`, `include_specs`), `batch_plan_get` (`plan_id` o el último, `node`) y
    `batch_plan_discard` (`plan_id` o el último, `all`) registradas en `Bridge` (21 operaciones). Código nuevo
    `PLAN_NOT_FOUND`; aviso `PLAN_MARKS_SKIPPED`.
  - `BatchPlanCommand.cs` + `UI/BatchPlanWindow.xaml(.cs)` + `UI/ChooseDialog.xaml(.cs)`: botón **Planificar lote**
    (cuarto del panel). Sin selección reabre el último plan del documento. Ventana modal con la tabla (nudo, color,
    estado, orientación, cordón, barras con ángulo, plantilla, desvío, avisos, errores, token), detalle del nudo
    (punto de trabajo, barras, casado, intentos, errores y avisos completos) y botones **Ver en Revit**, **Excluir /
    Incluir**, **Cordón…**, **Barras…** (listas con **Pinchar en Revit…**, P11), **Añadir nudo…** (pinchar), **Plantilla…**
    (automática, ninguna o una de las del plan), **Editar nudo** (abre `PreviewWindow` con la especificación; el cambio
    pasa a `overrides.spec`; **Crear** allí no crea nada), **Quitar edición**, **Replanificar**, **Guardar plan JSON**
    (`Documentos\MotorConexiones\plan-<id>.json`), **Descartar plan** y **Cerrar** (deja marcas y plan). Las acciones que
    necesitan la vista de Revit cierran la ventana, el comando las hace (`ShowElements`, `PickObject(s)` con filtro de
    armazón estructural) y la ventana se vuelve a abrir replanificada (bucle en el comando, sin `ExternalEvent`).
  - Versión **0.8.0** en `AddinInfo`, los dos csproj, adaptador, herramientas, simulador, contrato y README.
- **MCP**: `mcp/revit_mcp/conexiones.py` (3 rutas `/conn/batch/plan/...`, 23 en total), `mcp/tools/conn_tools.py`
  (`conn_batch_plan`, `conn_batch_plan_get`, `conn_batch_plan_discard` con su manual; `include_specs` por defecto
  `false` para no volcar 3 KB por nudo), `mcp/pruebas/simulador_revit.py` (imitación mínima del plan sobre el nudo del
  fixture; 45 comprobaciones), `mcp/pruebas/probar_conexiones.py` (pruebas 22 a 24 con `mark: false`; 26 en total, 28
  con `--puente`), `mcp/CONTRATO-conn.md` (rutas, códigos, herramientas, deshacer, pruebas).
- **Scripts y documentación**: `scripts/sondeos/17-marcas-plan.py` (prueba en Revit las API de las marcas y se deshace);
  `scripts/conn-call.ps1` (comentario con las 21 operaciones); `docs/guide.md` (sección 6 "Lotes", la de seguridad pasa
  a 7); README (estado, tabla de garantías, árbol, cuentas, sección 12 nueva "Planificar un lote", errores);
  `CLAUDE.md` (estructura: `Batch/`, `docs/prompts/`); `docs/prompts/fase-8.md`; este informe,
  `docs/instalacion/fase-8.md` y `docs/fases/resumen-fase-8.md`.
- **Pruebas**: `Tests/Fakes/SyntheticTruss.cs` (cercha sintética, sección 5 del prompt) con `SyntheticTrussFacts`
  (`IModelFacts` que mide el ángulo con signo respecto al marco del nudo, como `RevitModelFacts`), `NodeDetectorTests`
  (8) y `BatchPlanTests` (11). De 131 a **150**.

---

## 2. Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` y NuGet sirvió los paquetes de la API 2027, como en la Fase 7: el
proyecto `MotorConexiones.Revit` **compila** en la nube, así que los miembros de la API que usan las marcas
(`View.SetElementOverrides`, `OverrideGraphicSettings.SetSurfaceForegroundPatternId`, `FillPattern.IsSolidFill`,
`DirectShape.ApplicationId`, `TessellatedShapeBuilder`, `Selection.PickObjects`, `UIDocument.ShowElements`) existen; su
efecto visual lo prueba el sondeo 17 en el PC.

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
Build succeeded.  0 Warning(s)  0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   150, Skipped:     0, Total:   150 - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/17-marcas-plan.py   # correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
  [OK] 23 rutas registradas  registradas: 23
  ...
  [OK] POST /conn/batch/plan/ -> plan_id, N1 ready con token y batch_id
  [OK] POST /conn/batch/plan/get/ node N1 -> el mismo token (sin documento también responde)
  [OK] POST /conn/batch/plan/ con plan_id y overrides.exclude -> mismo plan, N1 excluded
  [OK] POST /conn/batch/plan/ con una corrección desconocida -> INVALID_REQUEST
  [OK] POST /conn/batch/plan/discard/ -> ok
  [OK] POST /conn/batch/plan/get/ <descartado> -> PLAN_NOT_FOUND
Autocomprobación: 45/45 correctas

$ python3 mcp/pruebas/simulador_revit.py &  &&  python3 mcp/pruebas/probar_conexiones.py
22. POST /conn/batch/plan/ (mark:false) -> plan_id, N1 ready con token  [OK]  ... resumen={'ready': 1} N1=ready same
23. POST /conn/batch/plan/get/ node N1 -> el nudo con su token  [OK]
24. POST /conn/batch/plan/discard/ -> descartado  [OK]
Resultado: 26/26 pruebas correctas
```

Lo que comprueban las pruebas nuevas del Core con la cercha sintética (`SyntheticTruss`: cordón central con cuatro
nudos del tipo del Detalle D, dos a cada lado; cordón superior dibujado al revés e inferior; una barra que cruza en Y
en X = 3000; la diagonal inferior de X = 5000 termina 6 mm por encima del eje; la de X = 7000 se queda 40 mm corta; una
barra suelta; un montante a 8,5 mm fuera del plano en el cordón inferior) y la plantilla del Detalle D (ángulos 135 /
45 / −135):

| Prueba | Qué comprueba |
|---|---|
| `Detect_FindsTheNodesOfTheSyntheticTrussWithTheirStatus` | 30 nudos: 15 `detected`, 13 `untyped`, 1 `ambiguous_chord`, 1 `offset`; los cuatro centrales con el cordón 100 atravesando |
| `Detect_MeasuresSignedAnglesInTheCanonicalFrame` | 135° / 45° / −135° con lados +Y, +Y, −Y en la mitad izquierda; −45° en la derecha (espejo); firma "2 +Y … 1 -Y" |
| `Detect_ReversedChord_StillGivesTheCanonicalFrame` | Los 8 nudos del cordón superior (dibujado al revés) tienen `ChordReversed` y X = +X global; una barra por nudo hacia −Y |
| `Detect_GroupsEndsWithinTheClusterToleranceAndNotBeyond` | 6 mm se agrupa (3 barras); 40 mm no (2 barras y un extremo suelto `untyped`) |
| `Detect_TwoThroughBars_IsAmbiguousAndAForcedChordResolvesIt` | Dos barras atraviesan → `ambiguous_chord` con las dos en `ThroughBarIds`; `BuildNode` con el cordón forzado → `detected` con 3 barras |
| `Detect_OffsetAndLooseBars` | Desfase de 8,5 mm → `offset` con el mensaje de `NodeFrame`; la barra suelta da dos nudos `untyped` |
| `Names_AreStableAndOrderedAlongTheTruss` | Mismos nombres en dos detecciones; N1…N30 por X global; N6, N11, N18, N22 son los centrales; `NextName` = N31 |
| `IsThrough_RequiresTheAxisNearAndThePointInsideTheSpan` | 4 mm sí, 6 mm no, en el extremo no (llega, no atraviesa), fuera del tramo no |
| `Build_PlansTheSyntheticTrussWithTheDetalleDTemplate` | 2 `ready` (N6 `same`, N18 `mirror_x`) con tokens de 64 distintos, `source.batch_id` = `plan_id`, barras en el orden de las ranuras y la placa cuchilla en la de abajo; 13 `no_match` (4 intentos "sin barra" en el de dos barras); 17 colores distintos; barras sin usar |
| `Overrides_ChordExcludeAndNoTemplate` | `chord` resuelve el ambiguo (`ready`, `same`); `exclude` → `excluded` sin spec ni marca; `template: null` → `no_match` con su detalle |
| `Overrides_AddAndRemoveMembers` | `add_member` de la diagonal corta → 3 barras, `mirror_x`, `ready` (su eje pasa por el nudo: retiro, no desfase); `remove_member` → `no_match` |
| `Overrides_MergeSplitAndAddNode` | `merge` del nudo de dos barras con el extremo suelto → `ready`; `split` → N6 con dos barras y N6-2 justo detrás (hereda el cordón que atraviesa); `add_node` "N18" sustituye al detectado y queda `ready` |
| `Overrides_SpecPerNode_ReplacesTheTemplateInstanceAndIsValidated` | 420→402 en la spec editada → `invalid` con `DIMENSION_CHAIN_MISMATCH`, `has_spec_override`, `batch_id` conservado; corregida → `ready` |
| `AlreadyConnected_IsSkippedUnlessReplaceExisting` | Barra en una conexión → `already_connected` sin token ni marca; con `replace_existing` → `ready` y `replaces_existing` |
| `Replan_KeepsThePlanIdAndTheNames` | Mismo `plan_id`, mismos nombres y el mismo token para el nudo que no cambió |
| `Plan_RoundTripsThroughJson` | `ToJson` → `FromJson`: nudos, tokens, specs y correcciones iguales; basura → nulo |
| `Overrides_ParseMergeAndReject` | Todas las claves; `MergeWith` (`include`, `chord: null`, añadir y quitar barras); clave desconocida, tipo malo y no objeto → `INVALID_REQUEST`; `null` → vacío |
| `Overrides_UnknownNodeNames_AreReportedAsPlanWarnings` | Nombres que no existen y barras inexistentes → avisos del plan, no errores |
| `NoTemplates_GivesNoMatchWithAnExplanation` | Sin plantillas, todos `no_match` con "No hay plantillas" |

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-8.md`)

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.8.0.0`; `instalar-conn.ps1` copia 23 rutas y 21 herramientas; `ping` lista `batch_plan*` y `0.8.0` | 8-2, 8-3 |
| Sondeo 17: override de color y patrón sólido sobre una barra, marcador DirectShape con `Marca` y `Comentarios`, lectura del override, limpieza y rollback; captura `fase8-01-sondeo17.png` | 8-3 |
| `conn_batch_plan` sobre la cercha entera del Hangar: nudos con sentido, el Detalle D `ready` `same`, los simétricos `mirror_x`, colores y marcadores en la vista (capturas `fase8-03`, `fase8-04`) | 8-4 |
| `batch_plan_get` de un nudo; replanificar con `exclude`, `include` (mismo token) y `chord` con el mismo `plan_id` | 8-5 |
| Botón **Planificar lote**: ventana, Ver en Revit, Editar nudo (y Quitar edición), Cordón/Barras con Pinchar en Revit, Añadir nudo, Guardar plan JSON, Descartar (capturas `fase8-05`, `fase8-06`) | 8-6 |
| `batch_plan_discard all` deja 0 marcadores; sondeos 17, 12 y 13 limpios; `probar_conexiones.py --puente` 28/28 | 8-7 |

### 2.2 NO PROBADO en la nube y por qué

- **Todo lo de Revit**: el efecto visual de los overrides (color de superficie con patrón sólido en una vista 3D
  sombreada, grosor de línea), la geometría del marcador (cubo por extrusión y octaedro por `TessellatedShapeBuilder`; si
  la teselación no diera un sólido, el código cae a un cubo pequeño), que `Marca` y `Comentarios` se puedan escribir en un
  `DirectShape`, `PickObject(s)` desde el bucle del comando con la ventana cerrada, `ShowElements`, y las ventanas WPF
  nuevas. Compila; no se ha ejecutado.
- **La detección sobre la cercha real**: las pruebas usan una cercha sintética con extremos exactos. En el Hangar las
  barras pueden tener extensiones o retiros (`resultados-fase-7b.md` mostró extensiones de 68 mm en 1249630 y 1249631
  tras borrar la conexión: `LocationCurve` sí llega al eje, pero conviene mirarlo) y las diagonales de la otra mitad
  llegan con otros ángulos. Es exactamente lo que el paso 8-4 debe enseñar: cuántos nudos salen `ready`, cuántos
  `no_match` o `invalid` (cartela fija, propuesta 3.6) y si la agrupación de 10 mm basta.
- **El tiempo**: planificar valida nudo a nudo contra el modelo (`ValidationService` por nudo). Con 20 nudos deberían ser
  unos segundos; la herramienta `conn_batch_plan` lleva 180 s de espera y `conn-call.ps1` en el paso 8-4 va con 600 s.
- **El puente MCP real** con las 21 herramientas (`--puente`): en la nube solo contra el simulador.

---

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. **Captura `fase8-01-sondeo17.png`** y el bloque `8-3 sondeo 17`: una barra en rojo con su superficie coloreada y un cubo
   rojo en su punto medio; `Tras limpiar: color valido=False | marcador existe=False`. Si el color de superficie no se ve
   (solo la línea), el patrón sólido no se encontró: dímelo con la línea `4) Patron solido`.
2. **Bloque `8-4 batch_plan`**: cuenta cuántos nudos hay por estado (`summary`) y compáralo con lo que ves en la cercha.
   El nudo 1249510 + 1249630 / 1249631 / 1249636 debe ser `ready`, `same`, desvío ~0 y token de 64. Los nudos de la otra
   mitad, `mirror_x`. Un nudo `no_match` o `invalid` en una posición donde "debería" casar es la información más útil:
   copia su `status_detail`, sus `members[]` (ángulos) y sus `attempts`.
3. **Captura `fase8-03-marcas.png`**: cada nudo con un color y su marcador en el punto de trabajo; cordón con línea más
   gruesa. Si dos nudos vecinos comparten color es que hay más de 12 (la paleta se repite): no es un fallo. Si un color
   agrupa barras de dos nudos distintos, sí: dímelo con los nombres.
4. **Paso 8-5**: el `plan_id` no cambia, `<N>` pasa a `excluded` y pierde el color, y al incluirlo vuelve con **el mismo
   token** que en 8-4.
5. **Paso 8-6**: la ventana del plan y cada botón; lo que no funcione como está descrito en `fase-8.md` (instalación) va
   al chat con el texto de la barra de estado o del diálogo.
6. **Paso 8-7**: `remaining_markers: 0`, el sondeo 17 con `Marcadores de plan ... : 0`, sondeos 12 y 13 en cero y
   `28/28`.

---

## 4. Decisiones tomadas y por qué

### 4.1 La detección es geometría pura y vive en el Core

`NodeDetector` trabaja con `DetectorBar` (eje en mm y tipo) y no sabe nada de Revit; `PlanBuilder` recibe un
`IModelFacts` y un delegado de validación. Así toda la lógica del plan (detección, correcciones, casado, instanciación,
estados) se prueba en la nube con una cercha sintética, y en Revit solo quedan la lectura de las barras, la validación
de siempre y las marcas. La misma cercha sintética servirá para probar el informe de la Fase 9.

### 4.2 Un nudo = cordón que atraviesa + barras que llegan; lo demás tiene estado propio

Se siguió la sección 3.2 de la propuesta al pie de la letra, con dos precisiones: una barra "atraviesa" solo si el punto
cae dentro de su tramo lejos de los extremos (una barra que llega no cuenta como cordón aunque su eje pase por el punto),
y el cordón que atraviesa se reconoce solo también en los nudos añadidos a mano o separados (`split`), aunque la persona
no lo incluya en la lista. Los nudos que no son nudos de cercha (extremos sueltos, empalmes) se listan como `untyped` y
no se tocan; los cruces de cordones (`ambiguous_chord`) y los desfases (`offset`) piden corrección en vez de adivinar.

### 4.3 Barras con retiro: el plan las admite si se añaden a mano

Un extremo 40 mm corto (retiro a lo largo de la barra) no se agrupa con el nudo (10 mm), así que ese nudo sale con una
barra menos (`no_match`) y la barra, como extremo suelto. La prueba `Overrides_AddAndRemoveMembers` deja escrito qué pasa
al añadirla con `add_member` (o `merge`): como su eje pasa por el punto de trabajo, la regla de `NodeReach` (hasta 500 mm
a lo largo del eje) la da por llegada, el nudo casa y valida (`ready`). Si en el Hangar hubiera muchas barras así, la
corrección sería subir `node_cluster_mm` en `config/catalog.json` o añadir una tolerancia "a lo largo del eje" en la
detección (una línea en `NodeDetector`), no cambiar el contrato.

### 4.4 Varias plantillas: gana la que casa todas sus ranuras con menor desvío

Como dice la propuesta (3.6): se pasan varias plantillas y cada nudo toma la que mejor casa; a igualdad de desvío, la
primera de la lista (o del catálogo, por nombre). `overrides.template` fija una para un nudo o lo deja sin plantilla. No
hay "casado parcial": un nudo con dos barras no recibe una plantilla de tres (`no_match` con los intentos explicados).

### 4.5 Marcas: overrides de vista y marcadores con memoria propia

Se eligió la opción A (P10): `SetElementOverrides` en la vista activa (no toca el modelo más allá de la vista) y un
`DirectShape` pequeño por nudo. El marcador lleva en `Comentarios` el plan, la vista y los IDs coloreados, y en
`ApplicationId` una etiqueta fija: así `batch_plan_discard` con `all: true` y el sondeo 17 pueden limpiar marcas de
planes que el add-in ya no recuerda (Revit reiniciado con el modelo guardado con marcas). Restaurar un override lo pone
en blanco (`new OverrideGraphicSettings()`): si la persona tenía overrides propios en esas barras, se pierden; se avisa
en el README. Los nudos en espejo llevan un octaedro en vez de un cubo (P6: "que se vean remarcados"), además de la
columna `orientation`.

### 4.6 El plan vive en memoria; las marcas, en el modelo

El plan (nudos, tokens, correcciones) se guarda en `PlanRegistry` mientras Revit siga abierto: los tokens dejan de valer
si el modelo cambia, así que no tiene sentido persistirlo en el modelo. **Guardar plan JSON** lo escribe en Documentos
para el registro; la Fase 9 recibirá `plan_id` más la lista `{node, spec, validation_token}`, como dice la propuesta
(3.5), y volverá a comprobar cada token.

### 4.7 Ventana modal con bucle en el comando (P10 y P11) — sustituida en el cierre de la ronda 8c (sección 10)

> Desde la 0.8.4 la ventana es **no modal** con `ExternalEvent` (opción B de P10): se queda abierta mientras la persona
> orbita y pincha, y todo lo que toca el modelo pasa por `PlanEvents`. Lo que sigue describe la 0.8.0 a la 0.8.3.

Las acciones que necesitan la vista de Revit (orbitar, pinchar) no pueden hacerse con la ventana modal abierta. En vez
de `ExternalEvent`, la ventana devuelve una acción al comando, el comando la ejecuta con las ventanas cerradas
(`ShowElements`, `PickObject(s)` con filtro de armazón estructural) y vuelve a abrir la ventana con el plan
replanificado y el mismo nudo seleccionado. Las correcciones que no necesitan la vista (excluir, cordón o barras de una
lista, plantilla, editar nudo) replanifican dentro de la ventana. "Elegir en Revit" (P11) entró en esta fase.

### 4.8 `include_specs: false` por defecto en la herramienta

Una especificación instanciada ocupa unos 3 KB; con 20 nudos la respuesta de `conn_batch_plan` pasaría de 60 KB. La
herramienta MCP pide por defecto los nudos sin `spec` y `conn_batch_plan_get` con `node` trae una completa cuando hace
falta. La ruta y `conn-call.ps1` las incluyen por defecto (`include_specs` solo cambia la respuesta).

---

## 5. Pendientes, riesgos y preguntas

- **P1 (riesgo principal): la agrupación de 10 mm sobre la cercha real.** Si en el Hangar las diagonales no llegan al
  eje del cordón (retiros modelados), saldrán nudos de menos barras y extremos sueltos. El paso 8-4 lo dirá; la
  corrección está en `config\catalog.json` (`node_cluster_mm`) o en una tolerancia a lo largo del eje (4.3). Pregunta: ¿las
  barras del Hangar llegan al eje con sus `LocationCurve` (como las cuatro del Detalle D) o hay retiros?
  **Respuesta (ronda 8b): no llegan.** El paso 8-4 dio 97 nudos de 53 barras, 93 `untyped` y 0 `ready`: las diagonales
  terminan en la **cara** del cordón, a 15–85 mm de su eje (sección 7.1). Se corrigió agrupando por el corte de los ejes,
  no subiendo `node_cluster_mm` (sección 7.3).
- **P2: nudos en espejo con la cartela fija.** La propuesta (3.6) ya avisa: en la otra mitad las diagonales pueden
  llegar con ángulos distintos y la cartela del plano no cubrirlas (`invalid` con `PLATE_OUTSIDE_GUSSET`). Es lo esperado;
  la respuesta es editar ese nudo en la ventana o la cartela automática (Fase 10). Pregunta: ¿cuántos nudos salen
  `invalid` por esto en la cercha del Detalle D?
- **P3: overrides previos de la persona.** Descartar pone el override de cada barra en blanco. Si en esa vista había
  colores propios, se pierden. Alternativa para después: guardar el override anterior en el marcador. ¿Usas overrides de
  elemento en las vistas de trabajo?
- **P4: la paleta se repite cada 12 nudos.** Para cerchas largas dos nudos lejanos pueden compartir color; el marcador con
  el nombre lo distingue. Si molesta, se puede alternar también el grosor o usar 24 colores.
- **P5: tiempo de planificación.** Cada nudo valida contra el modelo; con muchos nudos puede tardar varios segundos. El
  paso 8-4 anota la duración (`meta.duration_ms`).
- **Pendientes anteriores que siguen**: P2 de la Fase 7 (cara de la placa cuchilla, sin anotar en la 7b: se deja `+z`),
  P2 de la Fase 5 (`UNKNOWN_CONNECTION_TYPE`), P3 de la Fase 5 (Claude Desktop), soldaduras nativas y
  `BoltPattern.Connect`, traspaso de `mcp/` a `revit-mcp`, P9 de la 6b.
- **Siguiente fase**: 9, crear por lotes (sondeo de grupos anidados con Advance Steel, `BatchCreator`,
  `conn_batch_create`, `conn_batch_delete`, `conn_list` por lote, botón **Aplicar lote**, informe por nudo), con el prompt
  que se escribirá en `docs/prompts/fase-9.md` a partir de las secciones 3.5, 4 y 6 de la propuesta y de P9.

---

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-8.md` entero (empieza con `git pull` en `main`).
2. **Seleccionar la cercha entera** cuando el paso 8-4 lo pida (es el paso importante: cuantas más barras, mejor se ve la
   detección) y hacer los pasos marcados **(la persona)**, con las seis capturas.
3. **Mirar en Revit lo de la sección 3 de este informe**: el sondeo 17, los colores y marcadores, la tabla de nudos, el
   replan con el mismo `plan_id`, la ventana del plan, y que al descartar no quede nada.
4. **Dejar constancia**: el instalador sube `docs\fases\resultados-fase-8.md` y las capturas. Añade al final tus
   anotaciones (sobre todo P1 y P2 de la sección 5: qué nudos salieron con qué estado y por qué).
5. **Abrir la sesión de cierre** (ronda 8b si hace falta) con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-8.md (secciones 5 y 6) y docs/fases/resultados-fase-8.md. Cierra la Fase 8 con una ronda
   corta 8b: corrige lo que digan los resultados, actualiza el informe y la tabla de garantías del README. No empieces la Fase 9.
   Termina con el informe actualizado, docs/instalacion/fase-8b.md si hace falta otra ronda, commit, push y un resumen corto.
   ```

   **Hecho el 2026-10-05** (sección 7). Después, el instalador 8b (`docs\instalacion\fase-8b.md`) devolvió
   `docs\fases\resultados-fase-8b.md` y la sesión de cierre se hizo **el mismo 2026-10-05** (sección 8) con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-8.md (sección 7) y docs/fases/resultados-fase-8b.md. Cierra la Fase 8: contrasta los
   resultados de la ronda 8b con lo esperado en 7.5, corrige lo que haga falta (ronda 8c solo si es imprescindible),
   actualiza el informe y la tabla de garantías del README. No empieces la Fase 9. Termina con commit, push y un resumen corto.
   ```

6. **Solo con la 8 cerrada**, lanzar la Fase 9 con una sesión nueva:

   ```
   Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-8.md y docs/propuestas/catalogo-y-lotes.md completo.
   Escribe docs/prompts/fase-9.md (crear por lotes: secciones 3.5, 4 y 6 de la propuesta, con las decisiones de 7.1)
   y ejecuta SOLO la Fase 9. Incluye en docs/instalacion/fase-9.md las comprobaciones pendientes de la sección 8.5 de
   docs/fases/fase-8.md (add-in 0.8.2). Termina con docs/fases/fase-9.md, docs/instalacion/fase-9.md, commit, push y un resumen corto.
   ```

---

## 7. Ronda 8b (2026-10-05): resultados del instalador y corrección de la detección

Los pasos 1 a 4 de la sección 6 se hicieron el 2026-10-04: el instalador ejecutó `docs/instalacion/fase-8.md` y subió
`resultados-fase-8.md` con cuatro capturas (`fase8-02` a `fase8-05`; sin `fase8-01-sondeo17` porque el sondeo falló y sin
`fase8-06-nudo-editado` porque no hubo ningún nudo editable). La persona no añadió anotaciones al final del archivo; lo
que no está en los bloques se lee en las capturas y en `8-7 log del dia`. Esta ronda 8b es el paso 5. Add-in **0.8.1**.

### 7.1 Contraste de los resultados con lo esperado

| Qué (sección 2.1 e instalador) | Esperado | Resultado del instalador | |
|---|---|---|---|
| 8-2 build, test, deploy, instalar-conn | 0 avisos, 150/150, `0.8.0.0`, 23 rutas, 21 herramientas | `0 Advertencia(s)`, `0 Errores`, `Superado: 150`, `== MotorConexiones 0.8.0.0 desplegado ==`, `23 rutas`, `21 herramientas`, DLL `0.8.0.0` | OK |
| 8-3 `ping` y `catalog_list` | `0.8.0`, 21 operaciones; la plantilla oficial | `addin_version 0.8.0`, backend `advancesteel`; `templates_count: 1`, `Nudo tipico Detalle D` (`6abcf116…`, patrón `136,9 +Y · 44,4 +Y · −135,6 −Y`) | OK |
| 8-3 sondeo 17 (marcas) | Override rojo leído, cubo de 160 mm con `Marca` y `Comentarios`, captura, limpieza y rollback | `1) Vista activa: {3D} ... admite overrides=True`, `2) Marcadores de plan: 0` y después **`AttributeError: Name`** en la línea 67 (`barra.Symbol.Name`): el sondeo murió antes de poner nada. Lo mismo en 8-7 | **FALLO** del sondeo, no del add-in (las API de marcas las probó el propio `batch_plan`); corregido en 7.3 |
| 8-4 `batch_plan` sobre la cercha | Un nudo por punto de la cercha, el Detalle D `ready same`, los simétricos `mirror_x` | 53 elementos → **97 nudos: 93 `untyped`, 4 `no_match`, 0 `ready`**, `is_marked: true`, 4 marcadores y 8 barras coloreadas, 289 ms. N10 reconoció el cordón 1249510 atravesando con la diagonal 1249633 (`44,4°`), pero 1249630, 1249631 y 1249636 (las del Detalle D) quedaron como extremos sueltos (N1, N4, N6, N7...). Las barras no están en ningún nudo con cordón: `unused_element_ids` con 45 de las 53 | **FALLO del diagnóstico de la fase**: ver 7.2 |
| 8-4 capturas | Colores por nudo y marcadores | `fase8-03-marcas.png` (los cuatro nudos `no_match` coloreados, dos barras cada uno) y `fase8-04-marcador-espejo.png` (los marcadores se ven; en los `no_match` sin plantilla son cubos) | OK: las marcas funcionan |
| 8-5 `batch_plan_get`, excluir, incluir | Mismo `plan_id`, `excluded`, mismo token | Tres veces `PLAN_NOT_FOUND: No hay ningún plan con plan_id '35920'`: el `plan_id` que llegó fue el **número de proceso de PowerShell**, porque las instrucciones usaban `$pid`, variable automática de solo lectura | **FALLO de las instrucciones**; `$plan` en la 8b |
| 8-6 botón Planificar lote | Ventana, Ver en Revit, Editar nudo, Cordón/Barras pinchando, Añadir nudo, Guardar JSON, Descartar | Log: `batch_plan` `0e900ec3…` desde la cinta (53 barras), `ShowInRevit N10` (dos veces), `PickChord N12` → replan con `chord {N12: 1249631}`, `PickChord N13` → `chord {N13: 1249515}`, `PickNewNode` → `add_node {N98: [1250294, 1250297]}`, `ShowInRevit N93` y `N98`, `batch_plan_discard removed_marks 16`. Captura `fase8-05-ventana-plan.png`. **Editar nudo no se pudo probar**: solo se activa con nudos que tienen especificación (`ready` o `invalid`) y no hubo ninguno | OK en lo que se pudo probar; ver 7.2 |
| 8-7 descartar, restos, puente | `PLAN_NOT_FOUND`, `remaining_markers: 0`, sondeos 12 y 13 en cero, 28/28 | `batch_plan_get` devolvió el plan de 8-4 (seguía en memoria), `discard all`: `discarded_plans 1, remaining_markers 0`; sondeo 17 otra vez con el `AttributeError`; sondeo 12: `conexiones en el modelo: 0` (y las extensiones de 68,6 / 69,2 mm en 1249630 / 1249631 siguen ahí desde la 7b); sondeo 13: 0 restos; `--puente` **26/28**: las pruebas 22 y 23 (plan del nudo del fixture con `mark: false`) dieron `nudos=8 resumen={'untyped': 8}` en vez de `N1 ready`: el mismo problema de 8-4 sobre las cuatro barras del Detalle D | OK salvo el puente, que falla por lo mismo que 8-4 |
| Aviso de Revit | Ninguna ventana | La persona vio el aviso **"Elements have duplicate Mark values"**: los marcadores escribían `Marca` = `N1`, `N2`… en cada plan | **FALLO**; corregido en 7.3 |

### 7.2 Lo que no coincidió y qué se hace con ello

- **Las diagonales de la cercha real terminan en la cara del cordón, no en su eje.** Con los puntos de trabajo del plan
  (cada extremo salió como nudo `untyped` propio, así que los 53 ejes se reconstruyen con 0,1 mm) se ve la geometría: el
  cordón 1249510 va de X = −14397,6 a −4437,3 en Z = 17423; la diagonal 1249630 termina en Z = 17481,8 (**58,8 mm por
  encima** del eje), 1249636 en Z = 17389,9 (33,1 mm por debajo) y 1249631 arranca en Z = 17437,3 (14,3 mm por encima).
  Pero sus **ejes** cortan el eje del cordón en X = −11867,7, −11871,1 y −11871,1: a menos de 4 mm entre sí. Lo mismo en
  los otros nueve nudos del cordón central (a −6740, −1621, 3505, 8629, 13755, 49629, 54755, 59880 y 65010: diferencias
  de 0,1 a 3,4 mm entre cortes). Es decir: el modelo está bien hecho (los ejes concurren), pero cada `LocationCurve` se
  detiene en la cara del cordón (o la sobrepasa unos milímetros), con la esquina del HSS tocando la cara: 58,8 mm =
  38,1 (medio cordón) + 32 · cos 45° (media diagonal proyectada). La agrupación de extremos a 10 mm (sección 3.2 de la
  propuesta) no podía juntarlos, y subir `node_cluster_mm` a 90 mm habría juntado también cosas que no son un nudo.
  **Corrección (7.3): agrupar por el corte del eje de cada barra con el eje del cordón, con una tolerancia según el canto.**
- **La selección no tenía todos los cordones.** De los 53 elementos, el cordón central estaba en cinco tramos (1249510,
  1249509, 1249511, 1249515, 1249516) pero **no entre X ≈ 16 y 47 m**, y no estaban los cordones superior (Z ≈ 19916) ni
  inferior (Z ≈ 14955). Por eso, incluso corregida la detección, 6 tríos de diagonales y 20 parejas en K quedan sin
  cordón que los atraviese. El plan ahora lo dice con el aviso `NODE_CHORD_NOT_CONTINUOUS` en esos nudos, y la 8b pide
  seleccionar también esos cordones.
- **Sondeo 17: `AttributeError: Name`.** IronPython no resuelve `FamilySymbol.Name` (la propiedad `Name` de `Element` queda
  oculta en `ElementType`). Se lee con `DB.Element.Name.__get__(simbolo)` (con el parámetro `SYMBOL_NAME_PARAM` de
  reserva) y se escribe con `DB.Element.Name.__set__(...)`. Como el add-in ya no escribe `Marca`, el sondeo tampoco: cuenta
  los modelos genéricos del documento con `Marca` `N<número>` (deberían ser 0) y lee `Comentarios`.
- **`$pid` en el paso 8-5.** `$PID` es una variable automática de PowerShell (el número de proceso) y la asignación no la
  cambia; el cuerpo JSON llevó `"plan_id":"35920"`. Las instrucciones de la 8b usan `$plan`; las de la Fase 8 llevan una
  nota al principio y el paso corregido para que nadie las copie.
- **"Elements have duplicate Mark values".** Revit comprueba que `Marca` no se repita dentro de una categoría y los
  marcadores (Modelos genéricos) la escribían con `N1`, `N2`…: al replanificar en la misma sesión, o con dos planes, se
  repetían. El `FailureCollector` del add-in borra las advertencias de sus propias transacciones, pero Revit vuelve a
  comprobarlas en la siguiente orden de la persona y entonces sí sale la ventana. Los marcadores dejan de escribir `Marca`;
  el nombre va en `Name` del `DirectShape` y al principio de `Comentarios` (`N7 · MotorConexiones plan …`).
- **Editar nudo en gris.** Es el comportamiento previsto: el botón abre la previsualización con la **especificación** del
  nudo, y solo la tienen los nudos que casaron con una plantilla (`ready`, o `invalid` con errores) o los editados a mano.
  Un nudo `no_match`, `untyped`, `offset` o `ambiguous_chord` no tiene especificación que editar: primero hay que
  corregirlo (cordón, barras, plantilla). Con 0 nudos `ready` en el PC no había nada que editar. En la 0.8.1 el botón en
  gris lo explica al pasar el ratón ("solo se activa con nudos que tienen especificación (ready o invalid): N9 está
  no_match"), y la 8b lo prueba sobre el Detalle D.

### 7.3 Qué cambió en el código (0.8.1)

- **`Core/Batch/NodeDetector.cs`: extremos efectivos.** Antes de agrupar, cada extremo de barra se lleva al **corte de su
  eje con el eje de la barra vecina** (`SnapEnd`, `EffectiveEnds`, `BarEnd`): se busca la vecina cuyo eje corta al de la
  barra (a menos de `node_axis_max_distance_mm`, los 5 mm de `NodeFrame`), con el corte sobre la vecina (o como mucho un
  alcance más allá de su extremo) y con el extremo real a menos del **alcance de cara** de ese eje. Gana la vecina que
  **atraviesa** (el cordón) y, a igualdad, la de eje más cercano. El alcance de cara es **medio canto de cada barra más
  `node_cluster_mm`** (HSS3X3 + HSS 2-1/2: 38,1 + 31,75 + 10 = 79,85 mm; 40 mm por barra si el modelo no da medidas), o un
  valor fijo si se pone `node_face_reach_mm` en `config/catalog.json` (nueva clave, 0 = según el canto). Dos ejes con menos
  de 5° entre sí no se cortan (empalmes, cordones con quiebro), y una barra que sigue más de un alcance más allá del corte
  con una vecina que termina ahí pasa de largo (es el cordón que sobresale en el extremo de la cercha), no se lleva.
  Los extremos efectivos se agrupan a `node_cluster_mm` como antes y, además, los que cortan al **mismo cordón que
  atraviesa** se juntan si están a menos de un alcance a lo largo de él (nudos en K con excentricidad, como las parejas de
  34 mm del cordón superior del Hangar: salen como un nudo de dos barras con una de ellas `reaches_node: false`). El punto
  de trabajo es la media de los extremos efectivos (sobre el eje del cordón).
- **`DetectorBar.DepthMm`**: canto del perfil (la mayor de las dos medidas que `RevitModelFacts` ya leía:
  `STRUCTURAL_SECTION_COMMON_WIDTH/HEIGHT` o el nombre AISC). **`DetectedMember.EndGapMm`** y **`PlanMember.end_gap_mm`**:
  distancia del extremo real al punto de trabajo (0 si llega al eje; 85 / 20 / 48 mm en el Detalle D), para que la tabla
  diga cuánto se queda corta cada barra.
- **"Atraviesa"** (`PassesThrough`): una barra de la lista del nudo (llega por su extremo efectivo) solo atraviesa si sigue
  más de un alcance de cara por los dos lados (una diagonal que sobrepasa el eje del cordón unos milímetros llega, no
  atraviesa); una barra que no está en la lista atraviesa con que pase el punto más de `node_cluster_mm` por los dos lados
  (como antes). `WorkPointFor` (nudos añadidos a mano, `merge`, `split`) usa los mismos extremos efectivos, y `merge`
  calcula el punto de trabajo con `WorkPointFor` en vez de la media de los dos nudos.
- **Aviso `NODE_CHORD_NOT_CONTINUOUS`** (nuevo código en `ErrorCodes`, aviso por nudo en el plan): ninguna barra atraviesa
  el nudo y el cordón es la más horizontal de las que llegan (extremo de cercha o cordón que falta en la selección).
- **`Revit/Batch/PlanMarks.cs`**: sin `Marca`; `Comentarios` = `N7 · MotorConexiones plan <id>; view=…; ids=…; estado
  orientación; color` (`CommentsFor`); `RemoveAll` sigue leyendo `view=` e `ids=` igual.
- **`UI/BatchPlanWindow`**: el botón **Editar nudo** muestra el globo también en gris (`ToolTipService.ShowOnDisabled`)
  con el motivo y el estado del nudo elegido.
- **`scripts/sondeos/17-marcas-plan.py`** corregido (7.2) y **`18-extremos-cara.py`** nuevo: para la selección (o las cuatro
  barras del Detalle D) imprime tipo, canto `b × h`, extremos de la `LocationCurve`, extensiones y, por extremo, la vecina
  cuyo eje corta al suyo, la distancia del extremo a ese eje, cuánto se queda del corte y el corte (la misma regla que
  `SnapEnd`). Solo lee.
- **Versión 0.8.1** en `AddinInfo`, los dos csproj, adaptador, herramientas, simulador y contrato; `config/catalog.json`
  con `node_face_reach_mm: 0`; `docs/guide.md` (sección 6), `mcp/CONTRATO-conn.md` (`end_gap_mm`, aviso), `conn_tools.py`
  (manual de `conn_batch_plan`), README (estado, garantías, árbol, sección 12, errores) y `docs/instalacion/fase-8.md`
  (nota y `$plan`).
- **Pruebas**: `Tests/Fakes/HangarTruss.cs`, la **cercha real** del Hangar (las 53 barras del paso 8-4, reconstruidas de los
  puntos de trabajo del plan del PC, con los tipos que devolvió y los supuestos marcados); `SyntheticTruss` con el canto
  por tipo, la diagonal corta de X = 7000 a 120 mm (antes 40: ahora 40 mm se agrupa, porque está dentro del alcance) y los
  cordones superior e inferior sobresaliendo 286 mm; `NodeDetectorTests` +7 y `BatchPlanTests` +2. De 150 a **158**.

### 7.4 Qué se probó en la nube

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build      → Passed! Failed: 0, Passed: 158, Total: 158
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar     → Autocomprobación: 45/45 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas
$ python3 -m py_compile scripts/sondeos/17-marcas-plan.py scripts/sondeos/18-extremos-cara.py ...   → correcto
```

| Prueba nueva | Qué comprueba |
|---|---|
| `Detect_EndsCutAtTheChordFace_AreGroupedAtTheCutWithTheChordAxis` | Cordón HSS3X3 y tres diagonales HSS 2-1/2 a 45° que se quedan a 58,8 / 33 / 14 mm del eje (como el Detalle D): un solo nudo, cordón atravesando, punto de trabajo en el corte (±0,5 mm), ángulos 135 / −135 / 45, `ReachesNode` y `EndGapMm` = 83,2 / 46,7 / 19,8 |
| `Detect_FaceReach_FollowsTheProfileDepthOrTheConfiguredValue` | A 79 mm del eje se agrupa, a 95 no (alcance 79,85); sin canto conocido, 90 mm de alcance; `node_face_reach_mm` 120 agrupa los 95 y 20 solo la diagonal a 14 mm; `FromConfig` |
| `Detect_FaceCut_DoesNotSnapToParallelBarsNorAcrossAnOffset` | Dos tramos de cordón con quiebro de 3° y salto de 20 mm no se cortan (dos extremos sueltos en su sitio); el montante a 8,5 mm fuera del plano no se mueve |
| `Detect_GapKJoint_GroupsBothDiagonalsOnTheSameChordWithinTheFaceReach` | Dos diagonales que cortan el cordón a 34 mm una de otra son un nudo (una con `ReachesNode: false`); a 200 mm, dos nudos |
| `Detect_HangarTruss_FindsTheTenCentralNodesWithTheirThreeDiagonals` | La cercha real: 53 nudos, **10 con cordón atravesando y 3 diagonales** (todos `detected`, en Z = 17423), 6 tríos y 20 parejas sin cordón, el empalme 1249515/1249516 (`untyped`, paralelas) y 16 extremos; el Detalle D en X = −11870 con 136,9 / 44,4 / −135,6 (los ángulos de la Fase 7) y `EndGapMm` 84,5 / 19,6 / 48,1; el simétrico en X = −6740,5 con 135 / 44,4 / −44,4; sin `offset` ni `ambiguous_chord`; nombres N1…N53 estables |
| `Detect_HangarTruss_WithTheOldRuleTheDiagonalsStayedLoose` | Con el alcance anulado (lo que hacía la 0.8.0): 97 nudos y ninguno con dos barras, exactamente lo del PC |
| `Build_PlansTheHangarTrussFromThePcResultsWithTheOfficialTemplate` | El plan con la **plantilla oficial** `catalog/6abcf116….json`: 53 nudos, **10 `ready`** (5 `same`, 5 `mirror_x`, desvío ≤ 2°, 10 tokens distintos), 17 `untyped`, 26 `no_match` (los 6 tríos y las 20 parejas sin cordón, todos con el aviso `NODE_CHORD_NOT_CONTINUOUS`), 0 `invalid`; el Detalle D `same` con desvío 0, `batch_id`, la cuchilla en 1249636 y `end_gap_mm` 84,5; el simétrico `mirror_x` con la cuchilla en 1249637; `end_gap_mm` en el JSON ida y vuelta |
| `Overrides_AddNodeOnTheHangarTruss_UsesTheCutWithTheChordAsWorkPoint` | `add_node` con las cuatro barras del Detalle D: punto de trabajo en el corte con el cordón (X = −11870, Z = 17423), `chord_continuous`, `ready same`; el plan pasa a 11 `ready` |

Las 19 pruebas de la Fase 8 siguen pasando con los mismos números (30 nudos, 2 `ready`, 13 `no_match`...): la cercha
sintética tiene los extremos en el eje y solo cambió la diagonal corta (de 40 a 120 mm) y lo que sobresalen los cordones.

### 7.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-8b.md`, unos 40 minutos) — **hecho el 2026-10-05, contrastado en 8.1**

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.8.1.0` y copia `catalog.json` con `node_face_reach_mm`; `ping` dice `0.8.1` | 8b-2, 8b-3 |
| Sondeo 17 entero (sin `AttributeError`): override leído, marcador con `Name` y `Comentarios`, `Marca N<n>: 0`, captura `fase8b-01-sondeo17.png`, limpieza y rollback | 8b-3, 8b-7 |
| Sondeo 18 sobre el Detalle D: `b`/`h` 76,2 y 63,5, y las tres diagonales cortando el eje de 1249510 a 59 / 14 / 33 mm (85 / 20 / 48 del corte) con cortes a menos de 4 mm entre sí | 8b-3 |
| `conn_batch_plan` sobre los mismos 53 elementos: `ready: 10, no_match: 26, untyped: 17`; N4 (Detalle D) `ready same`, N7 `mirror_x`, `end_gap_mm` 84,5 / 19,6 / 48,1, aviso `NODE_CHORD_NOT_CONTINUOUS` en los sin cordón; marcadores sobre el eje; sin ventana de `Marca` duplicada | 8b-4 |
| `batch_plan_get`, excluir, incluir con `$plan` (mismo token); `chord` a un trío sin cordón | 8b-5 |
| Ventana: **Editar nudo** sobre el Detalle D (`ready (editado)` / `invalid (editado)` y Quitar edición), globo del botón en gris, Añadir nudo con el cordón, Descartar sin aviso de Revit | 8b-6 |
| `discard all` a 0, sondeos 17, 12 y 13 limpios, `--puente` **28/28** (las 22 y 23 vuelven a `N1 ready`) | 8b-7 |

### 7.6 NO PROBADO en la nube y por qué

- **El canto real que lee `RevitModelFacts`** (`STRUCTURAL_SECTION_COMMON_WIDTH/HEIGHT` o el nombre del tipo): las pruebas
  usan 76,2 / 63,5 / 101,6 mm por tipo. Si en el Hangar las familias no tuvieran medidas y el nombre no se pudiera leer,
  el alcance sería 90 mm (40 + 40 + 10) y el Detalle D se agruparía igual (58,8 < 90). El sondeo 18 imprime `b` y `h`.
- **Las coordenadas del fixture del Hangar** son los puntos de trabajo del plan del PC redondeados a 0,1 mm y, en los
  cuatro nudos `no_match`, la media de dos extremos a menos de 10 mm: la geometría real puede diferir unos milímetros. Por
  eso el paso 8b-4 compara el `summary` y los `end_gap_mm` con los de la nube.
- **Los tipos "supuestos"** del fixture (las barras que no salieron en ningún nudo con marco): se les dio el tipo de su
  familia (cordón HSS3X3, diagonal HSS 2-1/2). Si algún tramo del cordón central fuera HSS4X4 (como 1249516), cambia el
  alcance en 12 mm, no el resultado.
- **Que la plantilla oficial valide en Revit en los otros nueve nudos**: en la nube validan con `SpecValidator` y los hechos
  sintéticos (sin choques con barras ajenas); `ValidationService` en Revit comprueba además las colisiones.
- **Lo de siempre**: ventanas, pinchar en Revit, el efecto visual, el puente real.

### 7.7 Decisiones de la ronda 8b

- **Agrupar por el corte de los ejes, no subir `node_cluster_mm`.** Con 90 mm de agrupación se habrían juntado extremos
  que no son un nudo (las parejas en K del cordón superior están a 34 mm; los tramos del cordón central, a 415). El corte
  de los ejes es la geometría que el propio `NodeFrame` exige (ejes a menos de 5 mm) y da el punto de trabajo correcto:
  sobre el eje del cordón, donde la plantilla pone la cartela.
- **La tolerancia va por el canto** (medio canto de cada barra más la agrupación): un HSS3X3 con diagonales de 64 mm
  admite 80 mm; perfiles mayores admiten más sin tocar nada. `node_face_reach_mm` queda para forzar un valor si hiciera
  falta, y 40 mm por barra cuando el modelo no da medidas.
- **Los extremos cortados también se llevan al corte con otra diagonal del mismo nudo** (vecina que termina ahí), para que
  un nudo sin cordón en la selección salga como **un** nudo de dos o tres barras con el aviso `NODE_CHORD_NOT_CONTINUOUS`
  en vez de tres extremos sueltos: la persona ve qué le falta. Un cordón que sobresale más de un alcance en el extremo de
  la cercha sigue atravesando.
- **`end_gap_mm` en el contrato** (campo nuevo, solo añade): con él la tabla del plan y el instalador ven cuánto se queda
  corta cada barra sin abrir Revit.
- **Sin `Marca` en los marcadores.** El nombre ya estaba en `Name` y en `Comentarios`; `Marca` solo daba el aviso.
- **El fixture del Hangar entra en las pruebas.** Es la cercha real tal como la midió el PC; cualquier cambio futuro de la
  detección se comprueba contra ella (`10 ready` con la plantilla oficial).

### 7.8 Pendientes que siguen

- P2 (cartela fija en espejo): en la nube los cinco `mirror_x` validan sin `PLATE_OUTSIDE_GUSSET`; Revit lo confirmará.
- P3 (overrides previos de la persona), P4 (paleta de 12), P5 (tiempo: 289 ms para 53 barras en el PC, sin problema).
- Las extensiones de 68,6 / 69,2 mm que siguen en 1249630 / 1249631 desde la ronda 7b (el sondeo 12 las muestra): no
  afectan a la detección (usa la `LocationCurve`), pero conviene ponerlas a 0 en la copia o anotarlo para la Fase 9.
- Los pendientes anteriores de la sección 5 (Fase 7 P2, Fase 5 P2 y P3, soldaduras nativas, `BoltPattern.Connect`,
  traspaso de `mcp/`, P9 de la 6b) y la Fase 9 con el prompt de la sección 6.

---

## 8. Cierre de la Fase 8 (2026-10-05): la ronda 8b contrastada y cinco correcciones (0.8.2)

El instalador ejecutó `docs/instalacion/fase-8b.md` y subió `resultados-fase-8b.md` (1,7 MB, casi todo JSON) con cinco
capturas (`fase8b-01` a `fase8b-05`). La persona no añadió anotaciones ni hubo bloque del paso 8b-6 (ventana): lo que se
sabe de la ventana se lee en las capturas y en lo que dejó en memoria el plan del botón. Faltan también los dos últimos
bloques del paso 8b-7 (`probar_conexiones --puente` y `log del dia`): el archivo termina en el sondeo 13. Según
`docs/fases/resumen-fase-8b-que-sigue.md` (copia de un chat, subida aparte al remoto), el instalador dijo en su chat que
`--puente` falló en las pruebas **22, 23 y 27**, pero esa salida no está en el repositorio y no se pudo contrastar; lo que
sí se pudo hacer es explicar en la nube por qué 22 y 23 fallan en Revit aunque el plan esté bien (8.2).

### 8.1 Contraste con lo esperado en 7.5

| Qué (7.5) | Esperado | Resultado del instalador | |
|---|---|---|---|
| 8b-2 build, test, deploy, instalar-conn | 0 avisos, 158/158, `0.8.1.0`, `catalog.json` copiado, 23 rutas, 21 herramientas | `0 Advertencia(s)`, `0 Errores`, `Superado: 158`, `== MotorConexiones 0.8.1.0 desplegado ==` con `config\catalog.json`, `23 rutas`, `21 herramientas`, DLL `0.8.1.0` | OK |
| 8b-3 `ping`, `catalog_list` | `0.8.1`, 21 operaciones; la plantilla oficial | `addin_version 0.8.1` (en `data` y `meta`), 21 operaciones; `templates_count: 1`, `Nudo tipico Detalle D` `6abcf116…` | OK |
| 8b-3 y 8b-7 sondeo 17 entero | Override leído, marcador con `Name` y `Comentarios`, `Marca N<n>: 0`, captura, limpieza (paso 9) y rollback | Pasos 1 a 8 y 10 bien: vista `{3D}` admite overrides, `Patron solido: [20]`, override `(230, 25, 75) \| grosor 10 \| patron 20`, `6b) ... Marca N<numero>: 0`, marcador `[1321349] nombre=N1 (Name escrito=True)` con `Comentarios=N1 · MotorConexiones sondeo 17; view=1245519; ids=1249510`, caja 160, captura `fase8b-01`, `TransactionGroup deshecho`. **El paso 9 murió** con `The referenced object is not valid` en la línea 222: leía `marcador.Id` después de `doc.Delete`. Igual en 8b-7 (vista `Section 2`, `Marcadores de plan: 0`) | **FALLO del sondeo**, no del add-in; corregido (8.3) |
| 8b-3 sondeo 18 sobre el Detalle D | `b`/`h` 76,2 y 63,5; las tres diagonales cortan el eje de 1249510 a 59 / 14 / 33 mm (85 / 20 / 48 del corte), cortes a menos de 4 mm | `[1249510] HSS3X3X1/4 \| b=76.2 h=0.0 canto=76.2` (esa familia no da `h`; el canto sale de `b`), diagonales `b=63.5 h=63.5`; 1249630 a **58,8** mm del eje (alcance 79,9, 86,2 del corte, corte X = −11867,7), 1249631 a **14,3** (20,4; −11871,1), 1249636 a **33,1** (47,4; −11871,1); 5 de 8 extremos sin vecino | OK: la hipótesis de la 8b, medida en el modelo |
| 8b-4 `batch_plan` | Con los **mismos 53 elementos**: `ready: 10, no_match: 26, untyped: 17`; N4 `ready same`, N7 `mirror_x`, `end_gap_mm` 84,5 / 19,6 / 48,1, aviso `NODE_CHORD_NOT_CONTINUOUS` en los sin cordón; marcadores sobre el eje; sin ventana de `Marca` | La persona seleccionó **otra cercha: 56 barras** de la gemela en Y = +17204 (cordón central **entero** en ocho tramos, siete HSS4X4 de 101,6 mm y uno HSS3X3, y sus 48 diagonales; sin cordones superior ni inferior). **59 nudos: `ready: 16` (8 `same`, 8 `mirror_x`), `no_match: 20`, `untyped: 23`**, 953 ms, `is_marked`, 36 marcadores, `unused_element_ids: []`. N4 (gemelo del Detalle D: cordón 1250933 con 1251053 / 1251054 / 1251059) `ready same`, `max_deviation_deg 0`, token de 64, firma `2 +Y (137, 44) · 1 -Y (-136)`, punto de trabajo (−11870, 17204,3, **17423**); N7 `mirror_x` (135 / 44,4 / −44,4). Los 20 `no_match` son las parejas en K de los cordones superior e inferior (13 arriba, 7 abajo), dos barras cada una, con `NODE_CHORD_NOT_CONTINUOUS`; los 23 `untyped`, 14 extremos de tramos de cordón, el empalme 1250938/1250939 ("todas las barras son paralelas al cordón") y 8 extremos lejanos de diagonales. Sin errores. Aviso `TEMPLATE_PROFILE_DIFFERS` en los 14 nudos de los tramos HSS4X4 (la plantilla esperaba `HSS3X3X1/4`; se escribe el del modelo) y no en los dos del tramo HSS3X3 (N53, N56). **Pero `members[]` vino sin `end_gap_mm`** (solo `element_id`, `angle_deg`, `type_name`, `reaches_node`, `side`) | **OK la detección y el casado** (la nube reproduce el plan exacto, 8.4); **FALLO del add-in** en `end_gap_mm`, corregido (8.3) |
| 8b-4 capturas | Colores y marcadores en el eje | `fase8b-02-marcas.png` (la cercha entera con sus colores) y `fase8b-03-detalle-d.png`: cubo rojo de N4 **sobre el eje del cordón**, donde se cruzan los ejes de las tres diagonales, y el rombo verde de N7 al lado | OK |
| 8b-5 `batch_plan_get`, excluir, incluir, cordón | Mismo `plan_id`, `excluded`, mismo token; `chord` a un trío sin cordón | `batch_plan_get` N4: token `49fdb823…`; replan con `exclude` → **mismo `plan_id`** `4ef7dd3d…`, N4 `excluded` ("Excluido por la persona"), `ready: 15`, `overrides.exclude: ["N4"]`; replan con `include` → N4 `ready` con **el mismo token** `49fdb823…` y `overrides` vacío. El `replan cordon` no se hizo: no había ningún trío sin cordón (el cordón central estaba entero). El objeto `overrides` de la respuesta traía una clave `"IsEmpty": true` que no es del contrato | OK; `IsEmpty` corregido (8.3) |
| 8b-6 ventana | Tabla, Ver en Revit, **Editar nudo** sobre el Detalle D, globo en gris, Añadir nudo con el cordón, Guardar JSON, Descartar sin marcadores ni aviso | Sin bloque ni anotaciones. `fase8b-04-ventana-plan.png`: plan **`5556de0f…`** (nuevo, del botón, con la misma selección) `56 barras seleccionadas · 59 nudo(s): 23 untyped, 20 no_match, 16 ready. Marcas puestas en la vista`, la tabla con colores, cordón, barras con ángulo, plantilla, desvío (0,0° en N4; 1,3–1,4° en los demás) y tokens, y abajo `16 nudo(s) listos con token`. `fase8b-05-nudo-editado.png`: la previsualización abierta desde **Editar nudo** con la cabecera `Nudo N4 del plan (Nudo tipico Detalle D, same)`, `cordón HSS4X4X3-16 102x102 · cartela PL 3/8" · 3 barras, 1 placas cuchilla, 4 pernos`, `Validación correcta` y `Guardar JSON escribe en …\plan-5556de0f N4.json`. No consta Quitar edición, Añadir nudo, el globo ni Descartar | OK en lo que enseñan las capturas; **lo que falta queda NO PROBADO**. Y un fallo de diseño: el botón marcó el plan `5556de0f…` **mientras el del puente `4ef7dd3d…` seguía marcado** (ver 8.2) |
| 8b-7 descartar, restos, puente, log | `PLAN_NOT_FOUND` (o el plan de 8b-4 y `discard all` lo quita), `remaining_markers: 0`, sondeos 17, 12 y 13 limpios, `--puente` 28/28, log con `0.8.1` | `batch_plan_get` devolvió el plan del puente `4ef7dd3d…` con `is_marked: true` (sus 36 marcadores seguían en el modelo: Descartar en la ventana solo quitó los del plan del botón); `discard all`: `discarded_plans 1`, **`removed_markers: 0`** (quitó los 36 pero solo contaba los huérfanos), `remaining_markers 0`; sondeo 17: `Marcadores de plan: 0` (y el fallo del paso 9); sondeo 12: `conexiones en el modelo: 0` (extensiones 68,6 / 69,2 mm en 1249630 / 1249631 siguen desde la 7b) **pero tardó 387 s** (173 ms en la Fase 8); sondeo 13: 0 restos. **Sin bloque de `--puente` ni de `log del dia`** (según `resumen-fase-8b-que-sigue.md`, el chat del instalador dijo 22, 23 y 27 fallidas) | OK la limpieza; conteo corregido (8.3); puente y log **NO PROBADOS**; las pruebas 22 y 23 tenían un fallo propio, corregido (8.2) |
| Ventana de Revit (`Marca` duplicada) | Ninguna | No consta ninguna; el sondeo 17 cuenta `0` modelos genéricos con `Marca N<n>` | OK (sin confirmación expresa de la persona) |

### 8.2 Lo que no coincidió y qué se hace con ello

- **`end_gap_mm` no llegaba al puente.** El Core lo calcula y lo serializa (`PlanMember.end_gap_mm`, probado en la nube), pero
  `BatchPlanner.NodeToData` del add-in construye `members[]` a mano y no lo copiaba. La tabla de la ventana tampoco lo
  usa (muestra el ángulo). Corregido: una propiedad más en `NodeToData`. En la cercha de la 8b, N4 debe traer 84,5 / 19,6 /
  48,1 mm (hasta el punto de trabajo; el sondeo 18 imprime 86,2 / 20,4 / 47,4 porque mide hasta el corte **propio** de cada
  barra con el cordón, y el punto de trabajo es la media de los tres cortes, que distan hasta 3,4 mm). La definición se
  anota en el contrato.
- **Dos planes marcados en el mismo documento.** `conn_batch_plan` dejó el plan `4ef7dd3d…` marcado (36 marcadores) y el
  botón, con la misma selección, creó y marcó otro (`5556de0f…`): sus overrides pisaron los colores y añadió 36 marcadores
  más. **Descartar plan** en la ventana limpió solo el suyo: las barras quedaron sin color y con 36 cubos y rombos del plan
  del puente, y el paso 8b-7 lo confirma (`get` lo devolvió `is_marked: true`). Corregido: **en un documento solo hay un
  plan marcado**: al marcar un plan, el add-in quita las marcas de cualquier otro plan marcado del mismo documento (sigue
  en memoria, sin marcas) y avisa con **`PLAN_MARKS_REPLACED`**. Replanificar el mismo `plan_id` sigue como antes.
- **`removed_markers: 0` en `discard all`.** `DiscardAll` quitaba primero las marcas de los planes en memoria (sin contarlas)
  y devolvía solo los marcadores huérfanos. Ahora devuelve cuántos marcadores había en el documento antes de limpiar.
- **`"IsEmpty": true` dentro de `overrides`.** Es una propiedad auxiliar de `BatchOverrides` que `System.Text.Json`
  serializaba. Lo malo no es estético: la IA lee `overrides` de `conn_batch_plan_get` y lo natural es devolverlo tal cual en
  la petición siguiente, y `IsEmpty` daba `INVALID_REQUEST` por clave desconocida. Ahora lleva `[JsonIgnore]` y una prueba
  comprueba que el `overrides` de una respuesta se puede enviar de vuelta.
- **Sondeo 17, paso 9.** Leía `marcador.Id` después de borrarlo; el Id se guarda antes. El resto del sondeo ya estaba bien
  (la captura se llama `fase8-01-sondeo17.png` en el script; el instalador la copió como `fase8b-01`).
- **La selección fue otra cercha.** No es un fallo: es mejor prueba (cordón central entero, dos perfiles de cordón, 16
  nudos en vez de 10). Sus 56 ejes reales (sondeo 18) entran en las pruebas como `HangarTruss8b` y la nube reproduce el
  plan del PC nudo a nudo (8.4). La cercha de 53 barras de la Fase 8 (`HangarTruss`) sigue como estaba.
- **`TEMPLATE_PROFILE_DIFFERS` en 14 nudos.** Es la política `warn` de la plantilla: el cordón del modelo es HSS4X4 y el
  del Detalle D, HSS3X3; se escribe el del modelo y la cartela de 565 × 530 valida en Revit sobre el cordón mayor
  (`is_valid: true`, pernos y placa cuchilla iguales). Que esa cartela sea la correcta para un cordón de 102 mm es
  decisión de ingeniería, no del add-in: queda como pregunta para la Fase 9 (8.7).
- **Las pruebas 22 y 23 del puente buscaban el nudo "N1" a mano.** `probar_conexiones.py` planifica solo las cuatro barras
  del fixture (cordón 1249510 y sus tres diagonales) y daba por bueno el plan si **`N1`** salía `ready`. Eso vale para el
  simulador (un solo nudo), pero no para Revit: cuatro barras tienen ocho extremos, tres forman el nudo y los otros cinco
  son extremos sueltos, y como los nombres van por la X global, tres de ellos quedan a la izquierda del nudo del Detalle D,
  que se llama **N4** (seis nudos: 1 `ready`, 5 `untyped`). La prueba nueva del Core
  `Build_OnlyTheFourBarsOfTheDetalleD_PlansTheNodeReady` lo demuestra con la geometría real: el plan de las cuatro barras da
  N4 `ready same` con el cordón atravesando. Así que, si en la 8b fallaron 22 y 23 como dice el chat del instalador, lo más
  probable es que fuera por el nombre, no por la detección (en la Fase 8, con 0.8.0, sí era la detección: `untyped: 8`).
  Corregido: las dos pruebas buscan el nudo `ready` que contiene el cordón (N1 en el simulador, N4 en Revit) y la 23 pide
  ese nombre. La prueba 27 (`tools/list` con las 21 herramientas) depende de que el servidor haya arrancado en los 20 s
  que da el paso 8b-7; si vuelve a fallar sola, es tiempo, no código.
- **`--puente` y `log del dia` sin anotar, y el sondeo 12 a 387 s.** Se comprueban en la instalación de la Fase 9 (8.5). Los
  387 s no se explican con los bloques (`list` tardó 11 y 6 ms): seguramente Revit estaba ocupado con una ventana o una
  orden abierta.

### 8.3 Qué cambió en el código (0.8.2)

- **`Revit/Batch/BatchPlanner.cs`**: `NodeToData` emite `end_gap_mm`; `Plan` quita las marcas de los otros planes marcados
  del mismo documento (solo si `mark`), dentro de la misma operación, con el aviso `PLAN_MARKS_REPLACED` por cada uno;
  `DiscardAll` devuelve los marcadores que había (planes en memoria y huérfanos) y registra `markers` y `orphans` en el log.
- **`Core/Batch/BatchOverrides.cs`**: `IsEmpty` con `[JsonIgnore]`.
- **`Core/Validation/ErrorCodes.cs`**: `PlanMarksReplaced` = `PLAN_MARKS_REPLACED` (aviso).
- **`scripts/sondeos/17-marcas-plan.py`**: el Id del marcador se guarda antes de borrarlo (paso 9).
- **`mcp/pruebas/probar_conexiones.py`**: las pruebas 22 y 23 buscan el nudo `ready` que contiene el cordón del fixture en
  vez de "N1" (26/26 contra el simulador, donde sigue siendo N1).
- **Versión 0.8.2** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador (sin cambios funcionales en el MCP:
  solo la versión); `mcp/CONTRATO-conn.md` (versión, aviso nuevo, `removed_markers`, definición de `end_gap_mm`),
  `docs/guide.md` (sección 6), `conn_tools.py` (manual de `conn_batch_plan`), README (estado, garantías, sección 12,
  errores) y este informe.
- **Pruebas**: `Tests/Fakes/HangarTruss8b.cs` (las 56 barras de la 8b con los extremos reales de la `LocationCurve` y los
  cantos que leyó `RevitModelFacts`: 101,6 / 76,2 / 63,5), `Detect_HangarTruss8b_ReproducesTheFiftyNineNodesOfThePc`,
  `Build_PlansTheHangarTruss8bExactlyLikeThePc`, `Build_OnlyTheFourBarsOfTheDetalleD_PlansTheNodeReady` y
  `Overrides_ToJson_HasNoHelperKeysAndCanBeSentBackAsARequest`. De 158 a **162**. La ventana, las marcas y el puente real
  no tienen prueba en la nube (compilan).

### 8.4 Qué se probó en la nube

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build      → Passed! Failed: 0, Passed: 162, Total: 162
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/17-marcas-plan.py scripts/sondeos/18-extremos-cara.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar     → Autocomprobación: 45/45 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas (addin_version 0.8.2; 22 y 23 con el nudo buscado por el cordón)
```

| Prueba nueva | Qué comprueba |
|---|---|
| `Detect_HangarTruss8b_ReproducesTheFiftyNineNodesOfThePc` | Las 56 barras de la 8b: **59 nudos**, 16 con cordón atravesando y tres diagonales (todos en Z = 17423, todas las barras `ReachesNode`), 20 parejas sin cordón, 23 `untyped` (22 de una barra y el empalme 1250938/1250939), sin `offset` ni `ambiguous_chord`; **N4** = cordón 1250933 con 1251053 / 1251054 / 1251059 a 136,9 / 44,4 / −135,6, punto de trabajo X = −11870 y `EndGapMm` 84,5 / 19,6 / 48,1; **N7** en espejo (135 / 44,4 / −44,4) en X = −6740,5; dos nudos sobre el tramo HSS3X3; nombres N1…N59 estables. Los mismos nombres, estados y ángulos que devolvió el PC |
| `Build_PlansTheHangarTruss8bExactlyLikeThePc` | El plan con la plantilla oficial: **16 `ready`** (8 `same`, 8 `mirror_x`, 16 tokens distintos, desvío ≤ 2°, `batch_id`), **20 `no_match`** de dos barras con `NODE_CHORD_NOT_CONTINUOUS`, **23 `untyped`**, 0 `invalid`, ninguna barra sin usar; `TEMPLATE_PROFILE_DIFFERS` en los 14 nudos de HSS4X4 (y `chord.profile` = el del modelo) y ningún aviso en los dos del tramo HSS3X3; N4 `same` con desvío 0 y la cuchilla en 1251059; N7 `mirror_x` con la cuchilla en 1251060. Es el `summary` exacto del paso 8b-4 |
| `Build_OnlyTheFourBarsOfTheDetalleD_PlansTheNodeReady` | Lo que hacen las pruebas 22 y 23 del puente: solo las cuatro barras del fixture (geometría real de `HangarTruss`) → **6 nudos**, 1 `ready` (cordón 1249510 atravesando, `same`, 3 barras, token) y 5 `untyped`; el nudo listo se llama **N4** y N1, N2 y N3 son extremos sueltos a su izquierda |
| `Overrides_ToJson_HasNoHelperKeysAndCanBeSentBackAsARequest` | El JSON de las correcciones no lleva `IsEmpty` y, devuelto a `FromJson`, conserva `exclude`, `chord` y `template: null`; el de unas correcciones vacías también se acepta |

Las 158 pruebas anteriores pasan sin cambios; `py_compile` y el simulador también (45/45).

### 8.5 NO PROBADO en la nube: se comprueba en la instalación de la Fase 9 (sin ronda 8c)

> Actualización (ronda 8c, 2026-10-05): la persona pidió la ronda 8c antes de la Fase 9, así que estas comprobaciones van en
> `docs/instalacion/fase-8c.md` (ver 9.3).

Nada de esto justifica una ronda de instalador aparte (son cambios de una o dos líneas, sin tocar la detección ni la
validación); la Fase 9 desplegará un add-in nuevo y su `docs/instalacion/fase-9.md` debe incluir:

| Qué | Cómo se ve |
|---|---|
| `deploy.ps1` y `ping` en **0.8.2** | `== MotorConexiones 0.8.2.0 desplegado ==`, `addin_version 0.8.2` |
| `end_gap_mm` en `members[]` de `batch_plan` y de `batch_plan_get` | En la cercha de la 8b, N4 con 84,5 / 19,6 / 48,1 (±1) y 0 en ninguna |
| `overrides` sin `IsEmpty`; devolverlo tal cual en una petición | `batch_plan` con `plan_id` y el `overrides` de la respuesta anterior → `ok: true`, no `INVALID_REQUEST` |
| Un plan marcado por documento | Con un plan del puente marcado, el botón con la misma selección: aviso `PLAN_MARKS_REPLACED` (en la barra de estado de la ventana y en el log), un solo juego de marcas en la vista; **Descartar plan** deja **0** cubos y rombos; `batch_plan_get` del plan del puente devuelve `is_marked: false` |
| `discard all` cuenta bien | Con un plan marcado de N nudos marcados: `removed_markers: N`, `remaining_markers: 0` |
| Sondeo 17 entero | `9) Tras limpiar: color valido=False \| marcador existe=False` y `10) TransactionGroup deshecho`, sin traceback |
| `probar_conexiones.py --puente` | **28/28** (las pruebas 22 y 23 dicen `nudo=N4 ready same`; la 27 necesita el servidor arrancado) |
| `log del dia` | `"addin_version":"0.8.2"`, `batch_plan` con `summary`, `batch_plan_discard_all` con `markers` y `orphans` |
| Lo que la 8b no anotó de la ventana | Quitar edición, Añadir nudo con el cordón, el globo de **Editar nudo** en gris, ninguna ventana de `Marca` |

### 8.6 Decisiones del cierre

- **Un plan marcado por documento.** Las marcas viven en la vista y dos planes a la vez solo confunden (el segundo pisa
  los colores del primero y cada Descartar limpia la mitad). Se descartó la alternativa de que el botón reutilice el plan
  del puente cuando la selección coincide: la IA y la persona pueden querer planes distintos; lo que no puede haber es dos
  juegos de marcas. El plan que pierde las marcas sigue en memoria y se replanifica con su `plan_id` si hace falta.
- **`end_gap_mm` sigue midiéndose hasta el punto de trabajo**, no hasta el corte propio de la barra: es la distancia que
  importa para la cartela (que se pone en el punto de trabajo) y coincide con lo que las pruebas de la 8b ya decían.
- **0.8.2 sin ronda 8c.** Cinco correcciones que compilan y se prueban en la nube (las del Core) o son una línea en el
  add-in; probarlas en Revit cuesta una sesión del instalador y la Fase 9 va a desplegar de todos modos. El README dice
  claramente qué versión está probada en el PC (0.8.1) y cuál en el repositorio (0.8.2).
- **La cercha de la 8b entra en las pruebas con sus ejes reales.** `HangarTruss` (Fase 8) venía de los puntos de trabajo
  del plan; `HangarTruss8b` viene de la `LocationCurve` leída en Revit. Las dos se quedan: una cubre la cercha con huecos
  en la selección (tríos sin cordón) y la otra el cordón entero con dos perfiles.

### 8.7 Pendientes que siguen

- **P2 (cartela fija en espejo)**: confirmado en el PC que los 8 `mirror_x` validan en Revit con la cartela del Detalle D
  (sin `PLATE_OUTSIDE_GUSSET`) en esta cercha. Sigue abierto para cerchas con otros ángulos.
- **P6 (nueva, para la Fase 9): la plantilla del Detalle D sobre un cordón HSS4X4.** El plan la da por `ready` con el aviso
  `TEMPLATE_PROFILE_DIFFERS` (política `warn`). Pregunta a la persona: ¿se crean esos 14 nudos con la misma cartela, o la
  plantilla debe exigir el perfil (`profile_policy: require`) y esos nudos salir `no_match` hasta tener su propia plantilla?
  La Fase 9 (crear el lote) no debería crear nada sobre esa duda sin preguntar.
- P3 (overrides previos de la persona), P4 (la paleta se repitió: con 36 nudos marcados, N4 y N23 van en rojo; el
  marcador con el nombre los distingue), P5 (tiempo: 953 ms para 56 barras y 16 validaciones, sin problema).
- Las extensiones de 68,6 / 69,2 mm en 1249630 / 1249631 desde la ronda 7b (y sus gemelas 1251053 / 1251054): no afectan
  a la detección; anotar para la Fase 9.
- El sondeo 12 a 387 s en la 8b: vigilar en la Fase 9; si se repite, anotar qué tenía Revit abierto.
- Los pendientes anteriores de la sección 5 (Fase 7 P2, Fase 5 P2 y P3, soldaduras nativas, `BoltPattern.Connect`,
  traspaso de `mcp/`, P9 de la 6b) y la Fase 9 con el prompt de la sección 6 (paso 6).

### 8.8 Qué hace la persona ahora

Nada en Revit: la Fase 8 está cerrada. Cuando quiera seguir, **lanza la Fase 9 en una sesión nueva** con el prompt del paso 6
de la sección 6 (ya pide incluir en `docs/instalacion/fase-9.md` las comprobaciones de 8.5). Si prefiere decidir antes la
pregunta P6 (8.7), que lo diga en ese prompt.

**Cambio de orden (2026-10-05, a petición de la persona):** antes de la Fase 9 va la **ronda 8c** (la ventana del plan se
entiende: solo nudos de verdad, estados en español, colores por estado, mapa de la cercha), con el prompt de
`docs/prompts/fase-8c.md`, sección 1. Sale de `docs/propuestas/flujo-intuitivo.md`. La Fase 9 se lanza después, con el
mismo prompt del paso 6 de la sección 6.

---

## 9. Ronda 8c (2026-10-05): la ventana del plan se entiende (mapa, estados en español, colores por estado), 0.8.3

Sale de `docs/propuestas/flujo-intuitivo.md` y del prompt `docs/prompts/fase-8c.md` (decisiones de su sección 9, tomadas
por recomendación). Es una ronda de **presentación**: la detección (`NodeDetector`), el casado, `PlanBuilder` y el contrato
no cambian; el fixture `HangarTruss8b` sigue dando 59 nudos y 16 `ready`. Cambia lo que la persona ve en la ventana, en el
modelo y en el chat. Add-in **0.8.3**, **NO PROBADO en Revit** (la última versión desplegada en el PC es la 0.8.1).

### 9.1 Qué se hizo

- **`Core/Batch/PlanAdvice.cs` (nuevo): una sola fuente de los textos.** `StatusText(nudo)` (estado en español con icono:
  `● Listo`, `▲ Listo con aviso`, `✖ No valida`, `✖ Sin plantilla que encaje`, `✖ Sin plantillas en el catálogo`, `✖ Falta el
  cordón`, `✖ Dos cordones posibles`, `✖ Los ejes no se cortan`, `◌ Ya tiene conexión`, `◌ Excluido`, `○ Barra suelta (no es
  nudo)`, con sufijos `(editado)` y `(rehacer)`), `ColorName(nudo)` (verde = listo, ámbar = listo con aviso, rojo = falta
  algo, gris = no se crea, con un RGB fijo por color), `VisibleByDefault(nudo)` (falso en las barras sueltas y en las parejas
  de dos barras con `NODE_CHORD_NOT_CONTINUOUS`; un trío sin cordón sigue visible porque es un nudo de verdad al que le falta
  el cordón), `MirrorText` (no / sí / sí (Y) / sí (XY)), `Advice(nudo, plan)` (qué hacer, una frase por caso, con el perfil
  del cordón y de la plantilla, la distancia de los ejes o los ángulos de las barras sacados del propio nudo),
  `MapLabel` ("N4 · Listo con aviso · Nudo tipico Detalle D · igual"), `SummaryText(plan)` (la cabecera: "Se crearán 16
  conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23
  barras sueltas."; con 0 listos, "Ningún nudo listo: …" y la causa más frecuente), `HiddenText`, `CatalogEmptyWarning()`
  (aviso `CATALOG_EMPTY`, código nuevo en `ErrorCodes`) y `ApplyStatusColors(plan)` (sustituye la paleta por nudo de la
  Fase 8 por el color del estado en `color_name`, `color_rgb` y `color_index`). La ventana, las marcas, la respuesta del
  MCP y la guía llaman aquí; se prueba sin Revit.
- **`Core/Batch/TrussMap.cs` (nuevo): el alzado de la cercha.** A partir de las barras (ejes en mm) y del plan calcula el
  plano de la cercha por mínimos cuadrados de los extremos (autovector del autovalor menor de la covarianza, por rotaciones
  de Jacobi; exacto cuando los ejes son coplanares; con una sola barra, el plano vertical que la contiene), elige el eje
  horizontal del mapa hacia +X global (o +Y si la cercha va en Y) y el vertical hacia +Z, proyecta cada barra a un segmento
  2D y cada nudo a un punto con su nombre, estado, color, espejo, plantilla y globo, y devuelve el rectángulo envolvente,
  `IsCoplanar` y `MaxPlaneDistanceMm`. Las coordenadas son las globales proyectadas: en la cercha del Hangar (Y constante)
  el mapa es el X–Z del modelo tal cual (N4 en X = −11870, cordón en Y = 17423).
- **`Core/Batch/BatchPlan.cs`**: `CanBeMarked` pasa a ser `PlanAdvice.VisibleByDefault`: se marcan los nudos de verdad,
  también los excluidos y los que ya tienen conexión (en gris, "no se crea"), y no las barras sueltas ni las parejas sin
  cordón. `PlanPalette` se queda (la asigna `PlanBuilder`, que no se toca) y el add-in la sustituye nada más construir el
  plan.
- **`Revit/Batch/BatchPlanner.cs`**: tras `PlanBuilder.Build`, `PlanAdvice.ApplyStatusColors(plan)` y, si no hay ninguna
  plantilla, el aviso `CATALOG_EMPTY` en `plan.Warnings`. `PlanToData` añade `summary_text`, `visible_count` y `hidden_text`;
  `NodeToData` añade `status_text`, `advice` y `visible_by_default`, y `color_name` / `color_rgb` pasan a ser los del estado
  (`null` en los ocultos). Nuevos `BarsOf(doc, plan)` (las barras del plan leídas con `RevitModelFacts`, solo lectura) y
  `MapOf(doc, plan)` para el mapa. **Solo se añaden claves**; `status`, `orientation`, nombres y tokens siguen iguales
  (regla 4 del prompt).
- **`Revit/Batch/PlanMarks.cs`**: el color de cada nudo es el de su estado (`PlanAdvice.Rgb`), el mismo RGB que la ventana;
  cubo y rombo se mantienen; `Comentarios` lleva el nombre del color del estado. Los nudos ocultos no se marcan (ni color ni
  marcador): la cercha de la 8b queda con sus 16 nudos marcados en vez de 36.
- **`Revit/UI/TrussMapCanvas.cs` (nuevo)**: el lienzo del mapa (misma técnica que `SketchCanvas`): barras en gris (el cordón
  de algún nudo más grueso), círculos con el número dentro y el color del estado, ocultos como círculos vacíos pequeños solo
  con *Mostrar ocultos*, anillo naranja en el elegido, globo al pasar el ratón, rueda = zoom, arrastrar = encuadre,
  `Fit()` = ver todo; clic = `NodeClicked`, doble clic = `NodeActivated`.
- **`Revit/UI/BatchPlanWindow`** (rehecha): título "MotorConexiones: conectar cercha (plan, todavía no crea nada)";
  cabecera con `SummaryText` y la línea del plan; contador de ocultos con **Mostrar ocultos / Ocultar (43)** y **Ajustar**;
  el mapa (con un separador para cambiar su alto); la tabla con Nudo, Estado (punto de color + texto), Espejo, Plantilla,
  Desvío y **Qué hacer** (texto con salto de línea); las columnas Token, Cordón, Barras, Color, Avisos y Errores salen de la
  tabla y van al **detalle del nudo** (id del cordón con su tipo, ids de las barras con ángulo y "se queda a 84,5 mm del
  eje", casado, intentos, errores, avisos y token abreviado). Botones: **Más…** (Añadir nudo…, Guardar plan JSON, Descartar
  plan), **Replanificar**, **Editar nudo**, **Ver en Revit**, **Cerrar**; menú de clic derecho sobre el nudo (Ver en Revit,
  Editar nudo, Quitar edición, Excluir/Incluir, Cordón…, Barras…, Plantilla…; el clic derecho elige la fila antes de abrir
  el menú). Leyenda al pie. Clic en un círculo del mapa elige la fila (y enseña los ocultos si hace falta); doble clic =
  Ver en Revit; elegir una fila resalta su círculo. Con el catálogo vacío: cabecera "Ningún nudo listo: no hay plantillas…",
  aviso `CATALOG_EMPTY` en la barra de estado y botón **Abrir catálogo** (abre `CatalogWindow`; al cerrarla replanifica).
  Nada se pierde respecto a la 0.8.2: las doce acciones siguen, repartidas. `BatchPlanCommand` no cambia de flujo; el
  diálogo de **Ver en Revit** habla en español (`MapLabel` y el consejo).
- **MCP**: `conn_tools.py` (manual de `conn_batch_plan` con las claves nuevas y la orden de no volcar el JSON),
  `conexiones.py` (solo la versión), `simulador_revit.py` (las claves nuevas imitadas con `_decorar_nudo` y
  `_resumen_texto`, color por estado, aviso `CATALOG_EMPTY` con el catálogo vacío, dos comprobaciones más en
  `--autocomprobar`), `probar_conexiones.py` (las pruebas 22 y 23 exigen `status_text`, `advice`, `visible_by_default`,
  `summary_text` y `color_name` del estado), `mcp/CONTRATO-conn.md` (claves nuevas, valores de color, `CATALOG_EMPTY`).
- **`docs/guide.md`, sección 6 (C8)**: tras `conn_batch_plan` la IA enseña `summary_text` y una tabla corta solo con los
  nudos `visible_by_default`, resume los ocultos en una línea y no vuelca el JSON; `CATALOG_EMPTY` explicado.
- **`scripts/sondeos/19-etiquetas-lienzo.py` (nuevo, para V3, sin código de producción)**: comprueba con `hasattr`/`dir` si
  existen `TemporaryGraphicsManager`, `InCanvasControlData` e `ITemporaryGraphicsHandler`, genera un PNG con `System.Drawing`
  (un "4" sobre fondo verde), lo pone como control en el punto de trabajo del Detalle D en la vista activa, exporta la
  captura `fase8c-03-etiqueta.png`, instala un manejador de clic de prueba, quita el control y comprueba que no queda nada.
  Si falla, lo anota y no bloquea la ronda.
- **Versión 0.8.3** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador. README (estado, garantías, árbol,
  sección 12, 13 y 14), `CLAUDE.md` (estructura), `docs/propuestas/flujo-intuitivo.md` (qué quedó hecho) y este informe.

### 9.2 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build      → Passed! Failed: 0, Passed: 177, Total: 177
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar     → Autocomprobación: 47/47 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas (addin_version 0.8.3)
```

| Prueba nueva (de 162 a **177**) | Qué comprueba |
|---|---|
| `PlanAdviceTests` (10) | Texto, icono, color, visibilidad y consejo de cada estado de la tabla 3.1 del prompt: `ready` sin avisos (verde, "—"), con `TEMPLATE_PROFILE_DIFFERS` ("El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres", ámbar) y con `TEMPLATE_ANGLE_DEVIATION`; `invalid` con `PLATE_OUTSIDE_GUSSET`; `no_match` con cordón ("Ninguna plantilla encaja (3 barras, ángulos 136.9°, 44.4°, -135.6°)…"), pareja sin cordón (oculta) y trío sin cordón (visible); `ambiguous_chord`, `offset` ("Los ejes se cruzan a 12.3 mm…"), `already_connected`, `excluded`, `untyped`; sufijos `(editado)` `(rehacer)`; `MirrorText`; **la cabecera exacta de la cercha de la 8b**: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas.", 16 visibles, `ApplyStatusColors` → 14 ámbar, 2 verdes, 20 rojos, 23 grises y 16 marcables; la cercha sintética sin palabras internas en la cabecera; **catálogo vacío** → `IsCatalogEmpty`, "Ningún nudo listo: no hay plantillas en el catálogo…", `✖ Sin plantillas en el catálogo`, consejo y aviso `CATALOG_EMPTY`; con 0 listos, la causa más frecuente |
| `TrussMapTests` (5) | Sobre `HangarTruss8b`: **56 segmentos, 59 puntos, 16 visibles**, coplanar, eje X del mapa = +X global y eje Y = +Z (el plano de la cercha, Y = 17204 constante), **N4 en X = −11870** (Y = 17423) y **N7 en X = −6740,5**, rectángulo envolvente (−14536,8 … 67553,1 × 14894,7 … 19960,1), el cordón del Detalle D marcado como cordón, globos "N4 · Listo con aviso · Nudo tipico Detalle D · igual" / "… · en espejo"; sobre `SyntheticTruss`, **el cordón invertido no cambia el mapa**; con tres barras no coplanares, **mínimos cuadrados** (mejor que el plano Y = 0 en suma de cuadrados, `IsCoplanar` falso); una sola barra y ningún punto; una cercha a lo largo de Y |
| `BatchPlanTests` (ajustadas) | `CanBeMarked` con la regla nueva: los excluidos y los ya conectados se marcan en gris; los marcables son los visibles por defecto |

Simulador: dos comprobaciones nuevas (`status_text` "● Listo", `advice` "—", `visible_by_default`, `color_name` "verde",
`summary_text` "Se creará 1 conexión con …"; y con el catálogo vacío `CATALOG_EMPTY`, "✖ Sin plantillas en el catálogo" y
"Ningún nudo listo: no hay plantillas…"). `probar_conexiones.py` sigue 26/26 con las claves nuevas exigidas en 22 y 23.

### 9.3 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-8c.md`, unos 50 minutos)

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.8.3.0`; `ping` dice `0.8.3`; 177 pruebas | 8c-1, 8c-2 |
| Sondeo 17 entero, con el paso 9 (`color valido=False | marcador existe=False`) y el 10 (NO PROBADO de la 0.8.2) | 8c-2 |
| `conn_batch_plan` con la cercha de la 8b **más los cordones superior e inferior**: `summary_text`, `visible_count`, `hidden_text`, `status_text` y `advice` por nudo, `color_name` del estado, `end_gap_mm` en `members[]` (0.8.2), más nudos listos que 16 o `no_match` con cordón y su consejo; la cercha con colores por estado (captura `fase8c-01-colores-estado.png`) y sin marcas en barras sueltas ni parejas | 8c-3 |
| `batch_plan_get`, replan con el `overrides` devuelto tal cual (sin `IsEmpty`, 0.8.2), excluir (gris en el modelo) e incluir (mismo token), `PLAN_MARKS_REPLACED` al planificar desde el botón con el plan del puente marcado (0.8.2), `discard all` contando bien (0.8.2) | 8c-3, 8c-7 |
| Ventana: cabecera, mapa (`fase8c-02-mapa.png`), clic y doble clic en un círculo, globo, Mostrar ocultos, menú de clic derecho (Excluir → gris), Replanificar, Editar nudo sobre un listo, Más… > Descartar plan sin cubos ni rombos | 8c-4 |
| Catálogo vacío: `CATALOG_EMPTY`, cabecera "Ningún nudo listo: no hay plantillas…", botón Abrir catálogo; devolver las plantillas | 8c-5 |
| Sondeo 19: si existen las API, control con el "4" en verde en la vista, captura `fase8c-03-etiqueta.png`, quitado sin restos | 8c-6 |
| Sondeos 17, 12 y 13 en cero; `probar_conexiones.py --puente` **28/28 anotado en el archivo**; log del día anotado (las dos cosas que la 8b perdió) | 8c-7, 8c-8 |

### 9.4 NO PROBADO en la nube y por qué

- **Todo lo visual**: el mapa en pantalla (tamaño de los círculos, el globo, el separador), los colores por estado en Revit,
  el menú de clic derecho y el botón **Más…**, **Abrir catálogo** desde la ventana del plan (abre `CatalogWindow` con
  `Owner` = esta ventana; se comprueba que no haya dos ventanas modales peleándose). Compila (incluida la compilación del
  XAML) y la lógica de textos y del mapa está probada en el Core, pero no hay WPF ni Revit en la nube.
- **`BatchPlanner.BarsOf`** lee las barras con `RevitModelFacts` fuera de transacción (solo lectura): en la nube se prueba
  `TrussMap.Build` con las barras del fixture, no la lectura.
- **El sondeo 19**: los nombres de la API (`TemporaryGraphicsManager`, `InCanvasControlData`, `ITemporaryGraphicsHandler`,
  `AddControl`, `RemoveControl`, `Clear`, `SetTemporaryGraphicsHandler`) se comprueban en el PC con `hasattr`/`dir` antes de
  usarlos; si alguno no existe el sondeo lo dice y para. Que la etiqueta salga en la exportación de imagen tampoco es
  seguro (son gráficos temporales): el sondeo pide una captura a mano si no.
- **Lo de la 0.8.2** (8.5) sigue sin probar hasta esta instalación.
- **Lo de siempre**: ventanas, pinchar en Revit, el puente real.

### 9.5 Decisiones de la ronda 8c

- **Los textos viven en el Core (`PlanAdvice`), no en la ventana** (3.6 del prompt): la IA, la ventana, las marcas y el
  simulador dicen lo mismo y se prueban sin Revit. La cabecera se fija con el texto exacto de la cercha de la 8b.
- **Color por estado también en el modelo** (P4): verde, ámbar, rojo, gris con el mismo RGB que la ventana. Los excluidos y
  los que ya tienen conexión se marcan en **gris** ("no se crea"), antes no se marcaban: así la cercha enseña de un vistazo
  qué se va a crear y qué no. Las barras sueltas y las parejas sin cordón no se marcan (P2): 16 marcadores en la cercha de
  la 8b en vez de 36.
- **Un trío sin cordón sigue visible** (rojo, "Falta el cordón"): es un nudo de verdad al que le falta el cordón en la
  selección; solo se ocultan las **parejas** de dos barras (nudos en K de los cordones no seleccionados) y los extremos
  sueltos. El contador y **Mostrar ocultos** evitan que un nudo mal detectado se esconda del todo (riesgo de la propuesta).
- **`PlanBuilder` no se toca**: sigue asignando la paleta de la Fase 8 y el add-in la sustituye con `ApplyStatusColors` nada
  más construir el plan; `NodeToData` y `PlanMarks` calculan el color del estado por su cuenta, así que el resultado no
  depende de ese paso. `CanBeMarked` sí cambia (está en `BatchPlan.cs`), y dos pruebas de la Fase 8 se ajustan a ello.
- **El mapa se calcula en la ventana, no viaja en el contrato**: necesita los ejes de las barras, que el plan no guarda;
  `BarsOf` los lee del modelo (56 barras, milisegundos) y el contrato solo añade las cuatro claves de texto.
- **Plano por mínimos cuadrados siempre**: con ejes coplanares es exacto y no hace falta un camino aparte; el plano se
  inclina una millonésima en la cercha del Hangar (Y varía 0,3 mm en 80 m) y las pruebas lo admiten con ±1 mm.
- **`Abrir catálogo` solo abre y replanifica**: si la persona pide *Crear* dentro del catálogo, la ventana del plan no crea
  nada (lo dice) y remite al botón **Catálogo** de la cinta: el plan no crea acero en ninguna de sus rutas.
- **Sin ronda 8d**: el mapa cupo en la sesión (`TrussMap` + `TrussMapCanvas` + pruebas), así que no se escribe
  `docs/prompts/fase-8d.md`.

### 9.6 Pendientes y qué sigue

- Probar la 0.8.3 en el PC con `docs/instalacion/fase-8c.md` (incluye lo NO PROBADO de la 0.8.2) y contrastar en una
  sección 10 de este informe (o en el cierre de la Fase 9). **Hecho el 2026-10-05: sección 10.**
- Con la salida del sondeo 19 se decide V3 (etiquetas pinchables) para la Fase 10.
- **Fase 9** (crear el lote, botón **Crear N conexiones**, C3 con botones, V2 cartelas fantasma): después de la 8c, con el
  prompt del paso 6 de la sección 6 y la pregunta P6 (plantilla del Detalle D sobre cordón HSS4X4) por decidir.
- Los pendientes anteriores (8.7) siguen igual.

---

## 10. Cierre de la ronda 8c (2026-10-05): resultados contrastados y cuatro correcciones (0.8.4)

Sale de `docs/fases/resultados-fase-8c.md` (commits `d666087` y `9628e32` del instalador), de las anotaciones de la
persona recogidas en `docs/fases/resumen-fase-8c-que-sigue.md` y del prompt de cierre de su sección 3. Add-in **0.8.4**,
**NO PROBADO en Revit** (la versión desplegada en el PC es la 0.8.3): se comprueba con `docs/instalacion/fase-8d.md`.
La ventana no modal **sí cupo en la sesión**, así que no hay `docs/prompts/fase-8d.md`: la ronda 8d es solo la instalación.

### 10.1 Contraste con lo esperado en 9.3

| Esperado en 9.3 | Qué devolvió el PC (0.8.3) | Resultado |
|---|---|---|
| `deploy.ps1` 0.8.3.0, `ping` 0.8.3, 177 pruebas | `0.8.3.0` desplegado, `addin_version: 0.8.3`, `Superado: 177` | Bien |
| Sondeo 17 entero con los pasos 9 y 10 | `9) Tras limpiar: color valido=False \| marcador existe=False`, `10) TransactionGroup deshecho` (dos veces, 8c-2 y 8c-7) | Bien (lo NO PROBADO de la 0.8.2 queda probado) |
| `conn_batch_plan` con los cordones superior e inferior: claves nuevas, `end_gap_mm`, más listos que 16 o `no_match` con cordón | **64 elementos** (la persona dijo 56; el plan contó 64). `summary_text` = "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 con el cordón sin seleccionar. Ocultos: 8 sin cordón, 18 barras sueltas.", `visible_count: 33`, `hidden_text` "8 sin cordón, 18 barras sueltas"; `status_text`, `advice`, `visible_by_default` y `color_name` del estado en cada nudo; `end_gap_mm` en `members[]` (N4: 20,8 mm en la diagonal del cordón superior; en el gemelo del Detalle D los tres valores de la 8b); 1414 ms | Bien. Los 16 listos son los mismos de la 8b (el cordón central); arriba salen 10 `no_match` **con** cordón y 7 "Falta el cordón"; se explican en 10.2 |
| Colores por estado en la vista, sin marcas en sueltas ni parejas | Captura `fase8c-01-colores-estado.png`; 33 marcadores (`marker_element_ids`), ninguno en los 26 ocultos | Bien |
| `batch_plan_get`, replan con `overrides` devuelto, excluir (gris), incluir (mismo token), `PLAN_MARKS_REPLACED`, `discard all` | `batch_plan_get N4` completo; replan con `overrides` devuelto `ok: true` y el mismo plan; excluir → 15 listos y N4 `◌ Excluido` gris; incluir → 16 listos con el mismo token; `PLAN_MARKS_REPLACED` al planificar por el puente con el plan del botón marcado (8c-5, log); `discard all`: `discarded_plans: 7`, **`removed_markers: 66`**, `remaining_markers: 0` | Bien (todo lo de la 0.8.2 probado). Los 66 marcadores son dos juegos de 33: uno sobraba, ver 10.3 |
| Ventana: cabecera, mapa, clic y doble clic, globo, Mostrar ocultos, clic derecho, Replanificar, Editar nudo, Más… > Descartar sin cubos | Capturas `fase8c-02-mapa.png` y el resto según la persona: todo como se describe **salvo tres cosas**: (1) **Ver en Revit** sacó un cuadro de Revit; (2) **no se puede orbitar** con la ventana abierta y hay que cerrarla y reabrirla; (3) **Más… > Descartar plan** quitó los colores pero **dejó los cubos, en gris** | Tres fallos, corregidos en este cierre (10.3) |
| Catálogo vacío: `CATALOG_EMPTY`, cabecera "Ningún nudo listo…", Abrir catálogo; devolver las plantillas | Aviso `CATALOG_EMPTY`, cercha en rojo, `ribbon_batch_catalog_opened` dos veces en el log y replan al cerrar; plantillas devueltas (`catalogo restaurado`) | Bien |
| Sondeo 19: control con el "4" en verde, captura, quitado sin restos | `TemporaryGraphicsManager` e `InCanvasControlData` existen (miembros: `AddControl, Clear, GetAll, RemoveControl, SetTooltip, SetVisibility, UpdateControl`; `ImagePath, Position`); `AddControl` rechazó el PNG: **"only *.bmp files are supported"**; `DB.ITemporaryGraphicsHandler` **no existe** con ese nombre; sin captura | A medias: el sondeo se reescribe (10.3) y se repite en la 8d |
| Sondeos 17, 12 y 13 en cero; `--puente` 28/28 anotado; log anotado | 0 marcadores, 0 conexiones, 0 restos de acero; **`Resultado: 28/28 pruebas correctas`** anotado en el archivo; log del día anotado (las últimas 30 líneas de `batch_plan|ribbon_batch|startup`) | Bien |

### 10.2 Los 10 nudos "sin plantilla que encaje" y los 7 "falta el cordón" (los cordones superior e inferior)

La persona seleccionó 64 barras: las 56 de la 8b (el cordón central en ocho tramos y sus 48 diagonales) **más ocho tramos
del cordón superior** (HSS12X8X1/2, Z = 19 933 mm). Según los puntos de trabajo del plan, **el cordón inferior no entró**:
en Z ≈ 14 914 (abajo) no hay ninguna barra horizontal seleccionada; cada punto de abajo es una pareja "diagonal + montante
de 91°" sin cordón, y por eso van ocultos ("8 sin cordón" = esas 7 parejas más el extremo izquierdo N1 del cordón
superior). O el cordón inferior no se marcó al seleccionar, o en ese nivel no está modelado como barra de armazón
estructural; la 8d lo comprueba (paso 8d-3).

Los **10 nudos sin plantilla** están todos en el **cordón superior**: N5, N12, N19, N26, N33, N40, N47 y N54 (nudos en K con
dos diagonales que llegan **desde abajo**, a −135,6° y −44,4° / −45°, ambas en el lado −Y) y N49 y N58 (los dos extremos
derechos, con una sola diagonal). La plantilla *Nudo tipico Detalle D* es la típica del **cordón central**: cordón HSS3X3
con **tres** ranuras, dos diagonales por arriba (+Y, a 136,9° y 44,4°) y una por abajo (−Y, a −135,6°). El casado prueba
las cuatro orientaciones (igual, espejo en X, espejo en Y, ambos) y exige que **todas** las ranuras tengan barra: en el
espejo en Y las dos diagonales de arriba encajan en dos ranuras (desvío 0° y 1,3°), pero la tercera ranura (la diagonal
que en la plantilla sube hacia +Y) se queda sin barra, y en las otras orientaciones encaja una o ninguna (`attempts` lo
dice nudo a nudo: "ranura 2 (diagonal 135.6°) sin barra"). El resultado es correcto: **el Detalle D no es la típica de
los nudos del cordón superior**, que tienen otro cordón (HSS12X8) y dos barras en vez de tres. El consejo de la ventana
("Ninguna plantilla encaja (2 barras, ángulos −135,6°, −44,4°): crea esa típica o excluye") es lo que hay que hacer: si
se quieren conectar en el lote, se crea a mano un nudo del cordón superior, se guarda como plantilla y se replanifica;
si no, se excluyen (o se dejan: un `no_match` no se crea).

Los **7 "falta el cordón"** del cordón superior (N9, N16, N23, N30, N37, N44 y N52) son otra cosa: son los **empalmes**
del cordón superior. Ese cordón está modelado en ocho tramos y en esos siete puntos terminan dos tramos (uno llega a
0° / 180° como barra del nudo y el otro hace de cordón "que llega, no pasa de largo"), así que ninguna barra atraviesa
el nudo y el detector avisa `NODE_CHORD_NOT_CONTINUOUS`. El cordón sí está seleccionado; lo que pasa es que está cortado
ahí. Siguen visibles (son nudos de verdad, con dos diagonales) y en rojo, y su consejo es "Falta el cordón en la selección:
selecciónalo y replanifica, o Cordón…": con **Cordón…** se fija uno de los dos tramos como cordón, pero después tampoco
hay plantilla para ellos (dos diagonales desde abajo), así que, igual que los 10, necesitan su propia típica. El texto del
consejo se mejora en la Fase 9 (C3 con botones), no en este cierre, para no tocar `PlanAdvice` sin un caso probado en el
PC: la 8d pide a la persona que mire uno de esos siete en el detalle del nudo.

Resumen para la persona: **los 16 verdes y ámbar del cordón central son los que se crearán; los 10 rojos "sin plantilla"
y los 7 rojos "falta el cordón" del cordón superior necesitan una plantilla propia (crear uno a mano y guardarlo), y el
cordón inferior no entró en la selección**. Nada de esto es un fallo de la detección.

### 10.3 Qué cambió en el código (0.8.4)

- **(1) La ventana del plan es no modal, con `ExternalEvent`** (opción B de P10, mejora C7, adelantada de la Fase 10).
  - `Revit/Batch/PlanEvents.cs` (nuevo): una cola de trabajos `Action<UIApplication>` y un único `ExternalEvent` creado
    dentro del comando de la cinta (`EnsureCreated`; fuera de un comando `ExternalEvent.Create` lanza). `Run(work)` encola
    y levanta el evento; Revit ejecuta la cola en su hilo, en contexto válido, en cuanto queda libre (nada más soltar el
    ratón si la persona orbitaba). Si Revit no acepta la petición (`Denied`: un cuadro suyo abierto), el trabajo se quita
    de la cola y la ventana lo dice en español.
  - `Revit/Batch/PlanSnapshot.cs` (nuevo): el plan, el mapa (`TrussMap`) y el nombre del tipo de cada barra, leídos **de una
    vez en contexto válido** (`BatchPlanner.SnapshotOf`). La ventana ya no lee el modelo por su cuenta (antes `RefreshMap`
    y `TypeOf` llamaban a la API desde la ventana; con la ventana modal dentro del comando valía, fuera de él no).
  - `Revit/Batch/PlanPicker.cs` (nuevo): `PickObject` / `PickObjects` con el filtro de armazón estructural, sacados del
    comando, para llamarlos desde el evento; Esc cancela sin excepción.
  - `UI/BatchPlanWindow.xaml.cs` (rehecha): `RunInRevit(texto, prefijoError, trabajo)` apaga los botones (Cerrar sigue
    activo), pone "⏳ …" en la barra de estado, encola el trabajo y lo vuelve a encender al terminar; un fallo sale en
    rojo en la barra de estado y en el log (`ribbon_batch_action_failed`). Pasan por ahí: Replanificar, Excluir/Incluir,
    Cordón… y Barras… (la lista es WPF, el replan va por el evento; "pinchar en Revit" también), Plantilla…, Quitar
    edición, **Editar nudo** (la previsualización sigue modal, abierta dentro del evento porque valida contra el modelo),
    **Abrir catálogo** (igual), **Añadir nudo…**, **Ver en Revit** y **Descartar plan**. Guardar plan JSON y Mostrar ocultos
    no tocan Revit y siguen en la ventana. `Update(snapshot, estado)` repinta con lo que devuelve cada acción. Una sola
    ventana por sesión (`BatchPlanWindow.Current`); `Closing` se cancela mientras Revit está con una acción (una elección
    a medias dejaría el evento colgado) y lo dice. El documento activo tiene que ser el del plan (`DocumentOf`), por si
    la persona cambió de documento con la ventana abierta.
  - `BatchPlanCommand.cs` (rehecho): sin bucle. Planifica (o reabre el último plan), calcula el `PlanSnapshot`, abre la
    ventana con `Show()` (dueña: la ventana principal de Revit) y termina. Con la ventana ya abierta: con una selección
    nueva planifica y la actualiza (`ribbon_batch_window_updated`); sin selección la trae delante; si está ocupada, un
    cuadro lo dice (es el diálogo del botón de la cinta, el único permitido).
  - `PlanWindowAction` desaparece: la ventana ya no devuelve acciones al comando.
- **(2) Ver en Revit sin `ShowElements` ni cuadro y sin cerrar la ventana**: `Revit/Batch/PlanZoom.cs` (nuevo).
  `UIDocument.ShowElements` abre siempre el cuadro de Revit ("Revit hace zoom… Cerrar") y el comando añadía otro
  (`TaskDialog` con el consejo). Ahora `PlanZoom.ShowNode` busca la `UIView` de la vista activa
  (`GetOpenUIViews`), encuadra con `ZoomAndCenterRectangle` una caja de ±1,2 m alrededor del punto de trabajo
  (`HalfSizeMm`), selecciona las barras del nudo y su marcador y refresca la vista. Ningún cuadro; el consejo del nudo
  sigue en la barra de estado y en el detalle. Si la vista activa no es gráfica (una tabla de planificación), lo dice en
  rojo y no hace nada. Doble clic en el mapa = lo mismo.
- **(3) Descartar desde la ventana quita también los cubos**: `BatchPlanner.DiscardAndClean` (nuevo). Quita las marcas del
  plan, las de **cualquier otro plan marcado del documento** y los **marcadores huérfanos** (`PlanMarks.RemoveAll`, los que
  ningún plan en memoria reclama, leyendo la vista y los ids de sus Comentarios), olvida el plan y devuelve cuántos
  quedan (debe ser 0). Es lo que hace `conn_batch_plan_discard` con `all: true` por el puente, sin olvidar los demás
  planes (siguen en memoria sin marcas). El cuadro de confirmación lo dice ("también los de otros planes") y pasa a ser
  un `MessageBox` de WPF (no necesita contexto de Revit). Los 33 cubos en gris de la 8c eran marcadores de otro plan que
  el registro ya no tenía por marcado; el log del día (solo las 30 últimas líneas) no deja ver cuál ni por qué. Dos
  candidatos y una defensa:
  - Un plan marcado en **otra vista** (en 8c-3 el puente marcó en la vista 1321647 y el sondeo 17 corría en `{3D}` 1245519):
    los cubos son elementos del modelo y se ven en todas las vistas, pero el color es un override de **una** vista, así
    que desde otra vista se ven grises aunque nadie los haya tocado.
  - Una operación deshecha a medias: `PlanMarks.Remove` y `Apply` escribían en el plan en memoria mientras la transacción
    seguía abierta; si Revit la deshacía después (una excepción, un error de Revit), el modelo conservaba los cubos pero
    el plan ya decía "sin marcas" y nada los encontraba. Ahora `BatchPlanner.Plan`, `Discard` y `DiscardAndClean` guardan
    una foto (`PlanMarks.Capture` → `PlanMarkState`) y la devuelven si la operación falla.
  - La defensa vale para los dos casos: tras Descartar, `remaining_markers` = 0 se compruebe lo que se compruebe
    (`ribbon_batch_discard` lo anota en el log con `removed_marks`, `other_plans_unmarked`, `orphan_markers` y
    `remaining_markers`).
- **(4) Sondeo 19 con BMP y búsqueda del manejador de clics**: `scripts/sondeos/19-etiquetas-lienzo.py` (reescrito) y
  `19b-etiquetas-quitar.py` (nuevo). La imagen se guarda con `ImageFormat.Bmp` (fondo blanco: BMP no tiene
  transparencia); `InCanvasControlData` se lee con `ImagePath` y `Position` (los nombres que devolvió el PC, no `Location`);
  y el manejador de clics **no se da por supuesto**: el sondeo busca por reflexión en `RevitAPI.dll` y `RevitAPIUI.dll`
  todos los tipos públicos con `TemporaryGraphics` o `InCanvasControl` en el nombre (con sus miembros) y los servicios
  externos integrados (`BuiltInExternalServices`) con `Temporary` o `Canvas` en el nombre. Si aparece una interfaz
  `*TemporaryGraphicsHandler` y un servicio donde registrarla (lo esperable en Revit 2023+: `Autodesk.Revit.UI.
  ITemporaryGraphicsHandler` como servidor de `TemporaryGraphicsHandlerService`), registra un servidor de prueba que al
  pinchar la etiqueta abre un cuadro y escribe una línea en `%LOCALAPPDATA%\MotorConexiones\log\sondeo19-clics.txt`. El
  sondeo **deja la etiqueta puesta** para que la persona la pinche; `19b` enseña los clics, quita el control (`RemoveControl`
  y `Clear`), desactiva el servidor (un servidor registrado no se puede quitar hasta reiniciar Revit) y comprueba que no
  queda nada. Sigue sin código de producción: su salida decide V3 en la Fase 10.
- **Versión 0.8.4** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador (sin cambios de rutas ni de
  contrato). README (estado, garantías, árbol, sección 12), `docs/guide.md` (sección 6: la ventana abierta y la IA
  comparten los planes), `docs/propuestas/flujo-intuitivo.md` (C7 hecho), `mcp/CONTRATO-conn.md` (versión), `CLAUDE.md`
  (estructura) y este informe. La decisión 4.7 (ventana modal con bucle) queda sustituida por esta sección.

### 10.4 Qué se probó en la nube y cómo

```text
$ dotnet build MotorConexiones.sln -c Release --nologo      → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build      → Passed! Failed: 0, Passed: 177, Total: 177
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar     → Autocomprobación: 47/47 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas (addin_version 0.8.4)
```

No hay pruebas nuevas del Core: este cierre no toca el Core (`PlanAdvice`, `TrussMap`, `PlanBuilder` y el contrato siguen
iguales; `HangarTruss8b` sigue dando 59 nudos y 16 `ready`). Todo lo nuevo es del add-in (ventana, evento, zoom, descartar)
y del sondeo, y se compila contra la API 2027 (`UIView.ZoomAndCenterRectangle`, `UIDocument.GetOpenUIViews`,
`ExternalEvent`, `IExternalEventHandler`, `ExternalEventRequest`: todos resueltos por el compilador, ninguno inventado).

### 10.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-8d.md`, unos 35 minutos)

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.8.4.0`; `ping` dice `0.8.4`; 177 pruebas | 8d-1, 8d-2 |
| La ventana **se queda abierta**: orbitar, hacer zoom y pinchar barras en Revit con la ventana a un lado; los botones se apagan con "⏳ …" mientras Revit trabaja y se encienden solos | 8d-3 |
| **Ver en Revit** y doble clic en el mapa: zoom al nudo, barras y marcador seleccionados, **sin ningún cuadro** y sin cerrar la ventana | 8d-3 |
| Cordón… > pinchar en Revit, Barras… > pinchar, Más… > Añadir nudo… con la ventana abierta (Esc cancela y lo dice) | 8d-3 |
| Excluir / Incluir, Replanificar, Editar nudo (previsualización modal) y Abrir catálogo (con el catálogo vacío) con la ventana abierta | 8d-3, 8d-5 |
| Planificar lote con la ventana abierta y otra selección: la ventana se actualiza sin abrir otra; sin selección, solo viene delante | 8d-3 |
| **Más… > Descartar plan**: 0 cubos y 0 rombos en el modelo, en cualquier vista; `ribbon_batch_discard` en el log con `remaining_markers` 0 | 8d-4 |
| Con el plan del puente marcado en otra vista y la ventana descartando: tampoco quedan cubos | 8d-4 |
| Sondeo 19 v2: BMP aceptado, etiqueta visible, captura `fase8d-01-etiqueta.png`, tipos y servicios encontrados por reflexión, clic en la etiqueta (cuadro y línea en `sondeo19-clics.txt`), 19b limpia | 8d-6 |
| Sondeos 17, 12 y 13 en cero; `--puente` 28/28; log del día (esta vez las 80 últimas líneas) | 8d-7, 8d-8 |
| Si el cordón inferior existe como barra: seleccionarlo también y anotar cuántos nudos de abajo cambian (10.2) | 8d-3 |

### 10.6 NO PROBADO en la nube y por qué

- **Todo lo de la ventana no modal**: que Revit acepte el `ExternalEvent` en cada acción, que los botones se apaguen y se
  enciendan, que `PickObject` dentro del evento funcione con la ventana abierta a un lado (es el patrón habitual de los
  add-ins con ventanas no modales, pero no se ha ejecutado aquí), que la previsualización y el catálogo (modales dentro
  del evento) no se peleen con la ventana no modal, y que la ventana vuelva delante con `Activate`. No hay WPF ni Revit
  en la nube.
- **`ZoomAndCenterRectangle` en una vista 3D**: Revit proyecta las dos esquinas de la caja sobre la vista; en una vista en
  perspectiva puede lanzar excepción (la ventana la enseñaría en rojo). La 8d lo prueba en la 3D sombreada de siempre.
- **`DiscardAndClean`** con marcadores de otro plan y de otra vista: en la nube no hay modelo. Se prueba a propósito en
  8d-4 (plan del puente marcado en otra vista + Descartar desde la ventana).
- **El sondeo 19 v2**: los nombres del manejador de clics se buscan en el PC; si no existe ninguna interfaz
  `*TemporaryGraphicsHandler`, el sondeo lo dice y V3 se queda sin clics (etiquetas solo visibles).
- **Lo de siempre**: ventanas, pinchar en Revit, el puente real.

### 10.7 Decisiones del cierre

- **La ventana no modal entra ahora, no en la Fase 10**: era lo que más molestaba a la persona ("no se puede orbitar") y
  arrastraba el cuadro de Ver en Revit. La Fase 10 se queda con C6 (selección asistida) y V3 (etiquetas) si el sondeo dice
  que sí.
- **Un solo `ExternalEvent` con cola**, no uno por acción: menos estado, y una acción nunca pisa a otra (la ventana se
  pone ocupada hasta que termina la anterior).
- **La ventana no lee el modelo**: todo lo que necesita del modelo viaja en `PlanSnapshot` desde contexto válido. Es más
  código que llamar a la API desde la ventana, pero es lo único que Revit garantiza fuera de un comando.
- **Previsualización y catálogo siguen modales** (lo permitía el prompt): validan y leen el modelo, así que se abren dentro
  del evento; la persona no orbita con ellas abiertas, pero son ventanas cortas.
- **Descartar desde la ventana limpia todo el documento** (no solo el plan): es lo que espera la persona ("que no quede
  ningún cubo") y lo que ya hacía el puente con `all: true`. `conn_batch_plan_discard` con `plan_id` no cambia (el contrato
  sigue igual); la IA tiene `all: true` para lo mismo.
- **Sin tocar el Core ni los textos** (`PlanAdvice`): la explicación de los 10 nudos va en el informe y en la guía; el
  consejo de los empalmes del cordón superior se afina en la Fase 9 (C3), con un caso probado.
- **El sondeo 19 deja la etiqueta puesta** y se limpia con `19b`: es la única forma de que la persona la pinche (un sondeo
  que la quita al final no deja tiempo).
- **Sin `docs/prompts/fase-8d.md`**: la ventana no modal cupo; la ronda 8d es solo instalación y prueba.

### 10.8 Pendientes y qué sigue

- Probar la 0.8.4 en el PC con `docs/instalacion/fase-8d.md` y contrastar en una sección 11 (o en el cierre de la Fase 9).
- Con la salida del sondeo 19 v2 (y de `19b`) se decide V3 para la Fase 10.
- **Fase 9** (crear el lote, botón **Crear N conexiones**, C3 con botones, V2 cartelas fantasma): después de la 8d, con el
  prompt del paso 6 de la sección 6 y la pregunta P6 (plantilla del Detalle D sobre cordón HSS4X4) por decidir. Para los
  nudos del cordón superior (10.2): crear uno a mano, guardarlo como plantilla y replanificar, o excluirlos.
- Los pendientes anteriores (8.7 y 9.6) siguen igual.

## 11. Cierre de la ronda 8d (2026-10-06): resultados contrastados y tres correcciones (0.8.5)

Sale de `docs/fases/resultados-fase-8d.md` (commit `4c72a2c` del instalador, con las capturas `fase8d-01-etiqueta.png`,
`fase8d-02-ventana-abierta.png` y `fase8d-crash-revit.png`), de las anotaciones de la persona recogidas en
`docs/fases/resumen-fase-8d-que-sigue.md` (commit `5fc6f75`) y del prompt de cierre de su sección 3. Add-in **0.8.5**,
**NO PROBADO en Revit** (la versión desplegada en el PC es la 0.8.4): se comprueba con `docs/instalacion/fase-8e.md`.
Igual que en la 8d, no hay `docs/prompts/fase-8e.md`: la ronda 8e es solo instalación y prueba.

### 11.1 Contraste con lo esperado en 10.5

| Esperado en 10.5 | Qué devolvió el PC (0.8.4) | Resultado |
|---|---|---|
| `deploy.ps1` 0.8.4.0, `ping` 0.8.4, 177 pruebas | `0.8.4.0` desplegado y en la DLL, `addin_version: 0.8.4`, `0 Advertencia(s)`, `Superado: 177`, `templates_count: 1` | Bien |
| La ventana se queda abierta: orbitar, zoom y pinchar barras con la ventana a un lado; botones apagados con "⏳ …" mientras Revit trabaja | **Sí** (captura `fase8d-02-ventana-abierta.png`); en el log, `ribbon_batch_window_opened` con `"modeless":true` y `ribbon_batch_show` repetidos (N54, N25, N6, N9, N4) con la ventana abierta | Bien: era lo que pedía la persona (C7) |
| **Ver en Revit** y doble clic en el mapa: zoom al nudo, sin ningún cuadro, sin cerrar la ventana | El instalador lo dio por bueno sin verlo ("asumido"); el log tiene ocho `ribbon_batch_show` y ningún `ribbon_batch_action_failed`, así que `ZoomAndCenterRectangle` no lanzó en la 3D | A medias: la 8e lo mira a propósito (8e-3) |
| Cordón… > pinchar, Barras… > pinchar, Más… > Añadir nudo… con la ventana abierta (Esc cancela) | **Revit se cerró con "fatal error"** al pulsar clic derecho > **Cordón…** sobre N9 (captura `fase8d-crash-revit.png`); el log tiene tres arranques de la 0.8.4 (18:47, 18:57 y 19:01), es decir, dos reinicios de Revit, y ninguna línea entre el `ribbon_batch_window_opened` y el arranque siguiente. Barras… y Añadir nudo… no se llegaron a probar | **Fallo grave**, corregido (11.2 y 11.3) |
| Excluir / Incluir, Replanificar, Editar nudo, Abrir catálogo con la ventana abierta | Catálogo vacío con la ventana abierta: aviso y cercha en rojo (8d-5), `ribbon_batch_catalog_opened` y replan al cerrar; `templates_count: 1` al devolver las plantillas. De Excluir/Incluir, Replanificar y Editar nudo no hay anotación ni rastro en el log (los cierres de Revit cortaron el paso 8d-3) | Catálogo bien; el resto se repite en la 8e |
| Planificar lote con la ventana abierta y otra selección: la ventana se actualiza; sin selección, viene delante | Sin anotación; en el log solo `ribbon_batch_window_opened` con `"selection":0` (reabrir el último plan), ningún `ribbon_batch_window_updated` ni `_activated` | NO PROBADO; se repite en la 8e |
| **Más… > Descartar plan**: 0 cubos y 0 rombos; `ribbon_batch_discard` con `remaining_markers` 0 | `batch_plan_discard` desde la ventana: `removed_marks: 97`, `other_plans_unmarked: 0`, `orphan_markers: 0`, **`remaining_markers: 0`**; sondeo 17 después: `Marcadores de plan … : 0`; `discard all` por el puente: `removed_markers: 0`. **Pero la ventana no se cerró** (anotación 2; en el log, `closed` 33 s después, a mano, y en 8d-5 ni eso) | Marcadores bien; ventana corregida (11.3) |
| Plan del puente marcado en otra vista + Descartar desde la ventana | El `batch_plan` del puente dio `marked_view_id: 1245519`, que es la **{3D}** (sondeo 17: `{3D} … id=1245519`): la persona no estaba en otra vista, así que el caso "otra vista" no se reprodujo. Sin `PLAN_MARKS_REPLACED` porque Revit se había reabierto tras el cierre y no había plan en memoria | Lo probado (plan del puente + Descartar desde la ventana → 0) vale; "otra vista" sigue sin probarse a propósito |
| Sondeo 19 v2: BMP aceptado, etiqueta visible, clic, 19b limpia | BMP aceptado, `7) Control anadido … indice 0`, `SetTooltip` bien, `UI.ITemporaryGraphicsHandler` (OnClick) y `TemporaryGraphicsHandlerService` encontrados, manejador registrado sin error; **la etiqueta no se vio** (captura `fase8d-01-etiqueta.png`, también mirando la pantalla), así que no se pudo pinchar; 19b quitó el control y desactivó el servidor | A medias: la API funciona, el dibujo no aparece; v3 (11.3) |
| Sondeos 17, 12 y 13 en cero; `--puente` 28/28; log (80 líneas) | 0 marcadores, 0 conexiones, 0 restos; **`Resultado: 28/28 pruebas correctas`**; log anotado (las 80 líneas alcanzan hasta la 8c del día anterior) | Bien |
| Cordón inferior: si existe como barra, seleccionarlo y anotar | Sin anotación; los planes del día siguen siendo de 64 elementos (y uno de 65 en la 8c) | Sigue pendiente (no bloquea) |

### 11.2 Por qué se cerró Revit: no fue el pinchado, fue el diálogo de elección

La persona lo describió como "Cordón… → pinchar el cordón en el modelo", y el prompt pedía proteger el `PickObject`. Pero la
captura `fase8d-crash-revit.png` dice otra cosa: el cuadro "Revit has experienced a fatal error" sale con la ventana del
plan **todavía en reposo** (los botones Más…, Replanificar y Ver en Revit encendidos, la barra de estado con el texto de
antes, nada de "⏳ Pincha en Revit el cordón de N9…"), es decir, **antes** de que Revit pidiera pinchar. El nudo elegido era
**N9** (`✖ Falta el cordón`), cuyo detalle dice "cordón 1245530 HSS12X8X1/2 (llega, no pasa de largo)": tiene
`ChordElementId` distinto de cero. Y `OnChord` construye la lista con ese cordón marcado como "(cordón actual)" y abre
`ChooseDialog` en modo de **una sola elección** (`multiSelect: false`), cuyo constructor hacía `ItemsList.SelectedItems.Add(item)`
para cada opción marcada. WPF no lo admite en ese modo y lanza
`InvalidOperationException: Can only change SelectedItems collection in multiple selection modes. Use SelectedItem in single
select modes.` La prueba está en el propio log del día (`resultados-fase-8d.md`, 8d-8, línea de las 17:32:54 de la sesión 8c,
0.8.3): la **misma** excepción, en `ChooseDialog..ctor … line 40` llamada desde `BatchPlanWindow.OnChord … line 362`. Con la
ventana **modal** de la 8c la excepción subía por `ShowDialog()` hasta el `try/catch` del comando de la cinta, que la anotaba
como `ribbon_batch_window_failed` y cerraba la ventana (nadie se fijó). Con la ventana **no modal** de la 8d el comando ya ha
terminado cuando se pulsa Cordón…, el manejador corre en el despachador de WPF de Revit y una excepción sin capturar ahí es
un error fatal de Revit. Por eso no hay ninguna línea en el log: el fallo ocurría antes de cualquier `JsonLineLogger.Write`.

Consecuencias: (1) Cordón… fallaba en **todos** los nudos con cordón (los 16 listos, los 10 `no_match` y los 7 "falta el
cordón"), no solo en N9; Plantilla… también marca una opción ("automática") en modo de una elección, así que habría cerrado
Revit igual; Barras… (modo múltiple) no. (2) El `PickObject` dentro del `ExternalEvent` con la ventana abierta **no se llegó a
ejecutar nunca** en la 8d: sigue NO PROBADO, y por eso el cierre hace también lo que pedía el prompt (ocultar la ventana,
activar Revit, log antes y después), por si ese paso trae su propio problema.

### 11.3 Qué cambió en el código (0.8.5)

- **(1a) `UI/ChooseDialog.xaml.cs`**: en modo de una sola elección la opción marcada se pone con `SelectedItem` (y se hace
  `ScrollIntoView`); en modo múltiple sigue `SelectedItems.Add`. Es la corrección de la causa.
- **(1b) Ninguna excepción de la ventana llega a Revit** (`UI/BatchPlanWindow.xaml.cs`): cada manejador de la ventana
  (`OnChord`, `OnMembers`, `OnTemplate`, `OnEditNode`, `OnDiscard`, `OnSelectionChanged`, el menú, el mapa, `Loaded`… los
  veinte) pasa por `Guard(acción, trabajo)`: una excepción queda en el log como `ribbon_batch_window_error` (con la acción y la
  pila) y en rojo en la barra de estado, y la ventana sigue. Además la ventana se suscribe a `Dispatcher.UnhandledException`
  mientras está abierta y marca como tratadas **solo** las excepciones cuya pila pasa por `MotorConexiones.Revit.UI` (las de
  Revit o de otros add-ins no se tocan): es la red para lo que no es un manejador (pintado, enlaces de datos).
- **(1c) Pinchar en Revit con la ventana oculta** (lo que pedía el prompt): `PickHidingWindow` envuelve los tres pinchados
  (Cordón…, Barras…, Añadir nudo…). Antes de `PickObject`/`PickObjects`: `Hide()` de la ventana del plan y activación de la
  ventana principal de Revit (`UI/RevitMainWindow.cs`, nuevo: `SetForegroundWindow` de `user32` sobre
  `UIApplication.MainWindowHandle`, que es el único manejador que da la API; nunca lanza, devuelve falso si no pudo). Después,
  pase lo que pase, `Show()` y `Activate()` de la ventana. Cualquier excepción del pinchado se captura, queda como
  `ribbon_batch_pick_failed` y se vuelve a lanzar como `InvalidOperationException` en español, que `RunInRevit` enseña en la
  barra de estado (`ribbon_batch_action_failed`). En el log hay **una línea antes** (`ribbon_batch_pick`: acción, nudo, vista,
  `window_hidden`, `revit_activated`) **y otra después** (`ribbon_batch_picked` con el id o los ids, o "cancelado" con Esc):
  si Revit volviera a cerrarse, la última línea dice en qué punto.
- **(2) Descartar plan cierra la ventana**: `OnDiscard` llamaba a `Close()` dentro del trabajo del evento, con la ventana
  todavía **ocupada** (`_busy`), y `OnClosing` cancelaba el cierre ("Termina primero la acción en Revit…"); al terminar,
  `SetBusy(false)` dejaba la ventana abierta con el plan ya descartado (por eso en la 8d, tras descartar, aún se pudo pulsar
  Ver en Revit sobre N4). Ahora el trabajo deja `_closeWhenFree` y `RunInRevit` cierra la ventana en su `finally`, después de
  liberar los botones; `ribbon_batch_discard` lleva `closes_window: true` y el `closed` del log sale con `discarded: true`
  justo después.
- **(3) Sondeo 19 v3** (`scripts/sondeos/19-etiquetas-lienzo.py` reescrito; `19b-etiquetas-quitar.py` adaptado a varios
  controles). Sin inventar nada: los miembros usados se comprobaron en la nube contra `RevitAPI.dll` y `RevitAPIUI.dll` 2027.2.0
  por reflexión de metadatos (`TemporaryGraphicsManager.AddControl(InCanvasControlData, ElementId)`, `SetVisibility(int, bool)`,
  `SetTooltip`, `UpdateControl`, `GetAll()`, `RemoveControl`, `Clear`; `InCanvasControlData(string, XYZ)`;
  `UIDocument.RefreshActiveView()` y `UpdateAllOpenViews()`; `View3D.IsSectionBoxActive` y `GetSectionBox()`;
  `BoundingBoxXYZ.Min/Max/Transform`). Cambios respecto a la v2, cada uno con su línea de salida:
  - **BMP de 24 bits y 32×32**: el de la v2 era de 48×48 a **32 bits** (lo delata su tamaño: 9270 bytes = 48·48·4 + 54, es decir,
    con canal alfa). Ahora `Bitmap(32, 32, PixelFormat.Format24bppRgb)`; el sondeo lee la cabecera del archivo (ancho, alto,
    bits por píxel en el byte 28, tamaño, esperado 3126 bytes) y, si System.Drawing no lo guardó a 24 bits, lo reescribe a mano
    (escritor probado en la nube con un BMP de prueba: `(32, 32, 24, 3126)`).
  - **Ruta sin tildes ni espacios**: en la 8d el BMP estaba en `c:\users\andy bayona antón\appdata\local\temp\…`; un cargador
    de imágenes nativo puede fallar en silencio con la tilde. Ahora va a `C:\IA\MotorConexiones-sondeo19\` (o a `Public`, o a
    temp si no se puede escribir), y el sondeo dice si la ruta es ASCII.
  - **Posición en unidades internas**: ya lo estaba (mm / 304,8), pero ahora se imprime en pies y en mm, y se comprueba si el
    punto de N4 cae dentro de la caja de sección de la vista.
  - **`SetVisibility(indice, True)`**, `SetTooltip`, **`GetAll()`** (cuántos controles dice Revit que hay) y **refresco
    explícito** con `RefreshActiveView()` y `UpdateAllOpenViews()` después de `AddControl`.
  - **Dos etiquetas**: la "4" verde en N4 y una "B" azul en el centro de la caja de sección (o del recuadro de recorte, o de
    la barra 1249510): si se ve la B y no la 4, el problema es el punto (fuera de la caja o tapado), no el control.
  - La captura exportada con `ExportImage` se guarda como `…-exportada.png` y el sondeo avisa de que lo normal es que **no**
    enseñe los controles (son gráficos de pantalla): la persona hace la captura a mano (`fase8e-01-etiqueta.png`).
  - El manejador de clics se registra igual que en la v2 (funcionó); `19b` quita los índices guardados, llama a `Clear`,
    enseña `GetAll()` antes y después y borra los BMP.
- **Versión 0.8.5** en `AddinInfo`, los dos csproj, adaptador, herramientas y simulador (sin cambios de rutas ni de contrato).
  README (estado, garantías, árbol, sección 12), `docs/guide.md` (sección 6: Descartar cierra la ventana),
  `docs/propuestas/flujo-intuitivo.md` (C7 y V3), `mcp/CONTRATO-conn.md` (versión) y este informe.

### 11.4 Qué se probó en la nube y cómo

```text
$ apt-get install -y dotnet-sdk-10.0                          → SDK 10.0.112 (como en las fases anteriores)
$ dotnet build MotorConexiones.sln -c Release --nologo        → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build       → Passed! Failed: 0, Passed: 177, Total: 177
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar      → Autocomprobación: 47/47 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas (addin_version 0.8.5)
$ (escritor BMP de 24 bits del sondeo 19 v3, ejecutado en la nube)  → cabecera (32, 32, 24, 3126), la esperada
$ (reflexión de metadatos sobre RevitAPI.dll y RevitAPIUI.dll 2027.2.0) → todos los miembros de 11.3 existen con esa firma
```

No hay pruebas nuevas del Core: este cierre no toca el Core ni el contrato. Todo lo nuevo es del add-in (diálogo, ventana,
activación de Revit) y del sondeo.

### 11.5 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-8e.md`, unos 25 minutos)

| Qué | Paso |
|---|---|
| `deploy.ps1` dice `0.8.5.0`; `ping` dice `0.8.5`; 177 pruebas | 8e-1, 8e-2 |
| **Cordón…** sobre N9 (el nudo del cierre de Revit): el diálogo se abre con el cordón actual marcado; **Revit sigue vivo**; "Pinchar en Revit…" oculta la ventana, Esc la devuelve con "Sin cambios (elección cancelada)."; repetir y pinchar un tramo: replanifica | 8e-3 |
| **Barras…** > pinchar y **Más… > Añadir nudo…** con Esc; **Plantilla…** sobre un listo (abre y se cancela) | 8e-3 |
| **Ver en Revit** y doble clic en el mapa mirados a propósito: zoom, selección, ningún cuadro, ventana abierta | 8e-3 |
| Excluir / Incluir, Replanificar, Editar nudo y Planificar lote con la ventana abierta (lo que la 8d no llegó a probar) | 8e-3 |
| **Más… > Descartar plan**: la ventana **se cierra sola**, 0 cubos; log con `closes_window` y `closed … discarded: true` | 8e-4 |
| Sondeo 19 **v3**: BMP de 24 bits confirmado en la cabecera, ruta ASCII, dos controles, `SetVisibility`, `GetAll`, refresco; si se ve alguna etiqueta, captura a mano y clic; 19b limpia | 8e-5 |
| Sondeos 17, 12 y 13 en cero; `--puente` 28/28; log del día con `ribbon_batch_pick` / `ribbon_batch_picked` y sin `ribbon_batch_window_error` | 8e-6, 8e-7 |

### 11.6 NO PROBADO en la nube y por qué

- **Que el diálogo de elección ya no cierre Revit**: la corrección es de WPF puro y el modo de una elección con `SelectedItem`
  es el documentado, pero no hay WPF ni Revit en la nube. Es lo primero que mira la 8e (8e-3 sobre N9).
- **`PickObject` dentro del `ExternalEvent` con la ventana oculta**: en la 8d no se llegó a ejecutar; `Hide()` / `Show()` y
  `SetForegroundWindow` tampoco. Si Revit no acepta el pinchado en ese contexto, la 8e lo verá y el log dirá en qué línea.
- **La red del despachador**: `Dispatcher.UnhandledException` del hilo de Revit; el filtro por la pila es conservador (solo
  nuestro espacio de nombres), pero no se ha disparado nunca.
- **El sondeo 19 v3**: ninguna de las cinco hipótesis (bits, ruta, visibilidad, refresco, punto) está confirmada; si ninguna
  etiqueta se ve, V3 se queda sin dibujo y la Fase 10 lo da por cerrado.
- **Lo de siempre**: ventanas, pinchar en Revit, el puente real.

### 11.7 Decisiones del cierre

- **Corregir la causa y además blindar**: la causa (el diálogo) es una línea; pero una ventana no modal dentro de Revit no
  puede permitirse ninguna excepción suelta, así que todos los manejadores van protegidos y el pinchado se hace como pedía
  el prompt (ventana oculta, Revit activado, log antes y después). Lo segundo es lo que hará que un fallo futuro salga en
  rojo en la barra de estado en vez de cerrar Revit.
- **Red del despachador solo para este add-in**: marcar como tratada cualquier excepción del despachador de Revit taparía
  fallos de Revit o de otros add-ins; el filtro por `MotorConexiones.Revit.UI` en la pila lo evita.
- **`SetForegroundWindow` y no un miembro de la API de Revit**: la API da `MainWindowHandle` y nada para activarla; la llamada
  a `user32` está aislada en `RevitMainWindow`, nunca lanza y su resultado va al log.
- **Descartar cierra desde `RunInRevit`, no desde el trabajo**: cerrar la ventana mientras está ocupada es justo lo que
  `OnClosing` impide; el cierre se pide y se ejecuta al liberar.
- **El sondeo 19 v3 prueba varias hipótesis a la vez** (bits, ruta, visibilidad, refresco, punto) en vez de una por ronda:
  cada ronda en el PC cuesta una tarde; con dos etiquetas y la cabecera del BMP impresa, una sola pasada dice cuál era.
- **Sin tocar el Core, el contrato ni los textos**: el cierre es del add-in y del sondeo; la versión sube por el despliegue.

### 11.8 Pendientes y qué sigue

- Probar la 0.8.5 en el PC con `docs/instalacion/fase-8e.md` y contrastar en una sección 12 (o en el cierre de la Fase 9).
- Con la salida del sondeo 19 v3 se decide V3 para la Fase 10.
- **Fase 9** (crear el lote): después de la 8e, con el prompt del paso 6 de la sección 6 y las decisiones P6 (los 14 nudos
  con cordón HSS4X4) y los 10 nudos del cordón superior sin plantilla (10.2).
- Siguen pendientes de la 8d: el cordón inferior (si existe como barra) y la defensa de Descartar con un plan marcado en
  **otra** vista (en la 8d el plan del puente se marcó en la {3D}).
- Los pendientes anteriores (8.7, 9.6 y 10.8) siguen igual.

---

## 12. Cierre de la ronda 8e y de la Fase 8 (2026-10-06): la 0.8.5 contrastada en Revit y el sondeo 19 v4 (el add-in sigue en 0.8.5)

La ronda 8e se hizo en el PC el 2026-10-05 por la tarde con `docs/instalacion/fase-8e.md` (`resultados-fase-8e.md`, commit
`e45b0a1`, cuatro capturas `fase8e-*`). La prueba del puente salió `0/1` ese día porque Revit ya estaba cerrado cuando le
tocó, y se repitió el 2026-10-06 por la mañana con Revit abierto (bloques `8e-7b`, commit `ce1a475`): `28/28`. Este cierre
**no toca el add-in** (sigue en **0.8.5**, que es la que está desplegada en el PC): corrige el sondeo 19 (v4) y su 19b,
contrasta los resultados, y actualiza este informe, el README y `docs/propuestas/flujo-intuitivo.md`. Con esto la Fase 8
queda cerrada del todo; lo que sigue es la Fase 9.

### 12.1 Contraste con lo esperado en 11.5

| Esperado en 11.5 | Qué devolvió el PC (0.8.5) | Resultado |
|---|---|---|
| `deploy.ps1` 0.8.5.0, `ping` 0.8.5, 177 pruebas | `== MotorConexiones 0.8.5.0 desplegado ==`, DLL `0.8.5.0`, `addin_version: 0.8.5`, `0 Advertencia(s)`, `Superado: 177`; `instalar-conn` con 23 rutas y 21 herramientas | Bien |
| **Cordón… sobre N9** (el nudo que cerró Revit en la 8d): el diálogo se abre con el cordón actual marcado; **Revit sigue vivo**; "Pinchar en Revit…" oculta la ventana; Esc la devuelve con "Sin cambios (elección cancelada)."; repetir y pinchar un tramo: replanifica | **Revit siguió vivo.** En el log del día hay **9 `ribbon_batch_pick` y 9 `ribbon_batch_picked`**, todos con `window_hidden: true` y `revit_activated: true`, y **ningún** `ribbon_batch_window_error`, `ribbon_batch_pick_failed` ni `ribbon_batch_action_failed`; un solo `startup` de la 0.8.5 en toda la tarde (20:09:15, antes de abrir la ventana a las 20:16:08) y el siguiente a las 07:25 del día después. Cordón… sobre N9 se pulsó seis veces: la primera (20:19:12) terminó en `result: "cancelado"` dos minutos después (el Esc del paso 2) y las otras cinco (20:27 a 20:29) devolvieron `1245531`, `1251056`, `1245530`, `1245531` y `1245531`, cada una seguida de su `batch_plan` con `replan: true` y `ribbon_batch_replan`. Captura `fase8e-02-cordon-n9.png`: la ventana abierta con N9 y la barra de estado "Cordón de N9: 1245531." | **Bien: el fallo grave de la 8d está corregido** |
| **Barras…** > pinchar y **Más… > Añadir nudo…** con Esc; **Plantilla…** sobre un listo (abre y se cancela) | `Barras…` sobre N9 dos veces (20:32), las dos con `picked: 1251056` y replan (`add_member {"N9": [1251056]}`); `Añadir nudo…` (20:33:14) con `result: "cancelado"` once segundos después. Plantilla… cancelado no escribe nada en el log y la persona no lo anotó | Bien lo que deja rastro; Plantilla… sin prueba |
| **Ver en Revit** y doble clic en el mapa mirados a propósito: zoom, selección, ningún cuadro, ventana abierta | 7 `ribbon_batch_show` (N4 y N11) con la ventana abierta, ninguno seguido de `ribbon_batch_action_failed`; la persona no anotó si salió algún cuadro | Bien por el log; "ningún cuadro" sin anotación |
| Excluir / Incluir, Replanificar, Editar nudo y Planificar lote con la ventana abierta | 16 `ribbon_batch_replan`: el `delta` alterna `exclude: ["N4"]` y `exclude: []` (Excluir e Incluir sobre N4) y hay varios con `delta` vacío (Replanificar); en las cuentas acumuladas del `batch_plan` aparece `excluded: 1`. Editar nudo no escribe un evento propio. **Planificar lote con la ventana abierta no se dio**: la segunda ventana (`selection: 7`) se abrió a las 20:38:12, dieciséis segundos **después** de cerrar la primera (`closed … discarded: false` a las 20:37:56), y no hay ningún `ribbon_batch_window_updated` ni `_activated` | Excluir / Incluir y Replanificar bien; Editar nudo y "Planificar lote con la ventana abierta" siguen sin prueba |
| **Más… > Descartar plan**: la ventana se cierra sola, 0 cubos; log con `closes_window` y `closed … discarded: true` | `batch_plan_discard` desde la ventana (20:38:34): `removed_marks: 9`, `remaining_markers: 0`; `ribbon_batch_discard` con **`closes_window: true`** y, 14 ms después, `ribbon_batch_window … "action":"closed","discarded":true`. Sondeo 17 después: `Marcadores de plan … : 0`; `discard all` por el puente: `discarded_plans: 2`, `removed_markers: 0`, `remaining_markers: 0` | Bien |
| Sondeo 19 **v3**: BMP de 24 bits confirmado en la cabecera, ruta ASCII, dos controles, `SetVisibility`, `GetAll`, refresco; si se ve alguna etiqueta, captura a mano y clic; 19b limpia | `4) BMP A: C:\IA\MotorConexiones-sondeo19\etiqueta-A.bmp \| 32x32, 24 bits, 3126 bytes (esperado 3126) \| System.Drawing a 24 bits \| ruta sin tildes ni espacios: True` (y lo mismo para el BMP B); `5) Etiqueta A (N4): pies (-38.9360, -56.4167, 57.1621)`; **un solo control** (`indice 0`), porque la etiqueta B no se pudo calcular: `Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64)` (error del sondeo, 12.3); `SetVisibility(0, True) llamado`, `SetTooltip(0) puesto`, `GetAll(): 1 control(es)`, `RefreshActiveView()` y `UpdateAllOpenViews()` llamados; manejador registrado. **La etiqueta A se vio** (captura `fase8e-01-etiqueta.png`: círculo verde con el 4 sobre el cordón del Detalle D, en la vista {3D}) **y se pinchó** (captura `fase8e-01-etiqueta-clic.png`: cuadro "MotorConexiones - sondeo 19: Clic recibido. clic en una etiqueta: Index=0 \| Document=HANGAR_PRUEBA_sondeo"; en `sondeo19-clics.txt` quedaron **cuatro** clics). 19b no pudo ejecutarse ese día (12.2); repetido en `8e-7b` al día siguiente enseñó los cuatro clics, `GetAll(): 0` (Revit se había reiniciado y los controles temporales no sobreviven) y `RemoveControl(0) fallo: index is out of range` (corregido en 19b, 12.3) | **Bien: la etiqueta se dibuja y recibe el clic. V3 queda decidida que sí (12.6)** |
| Sondeos 17, 12 y 13 en cero; `--puente` 28/28; log del día con `ribbon_batch_pick` / `ribbon_batch_picked` y sin `ribbon_batch_window_error` | Sondeo 17 a cero en 8e-2 y 8e-4; sondeo 12: `conexiones en el modelo: 0`, pero tardó **274 972 ms** (en la 8d, 257 ms; 12.2); sondeo 13: `0` restos. `--puente` **`0/1` el 2026-10-05** (`WinError 10061`: Revit ya cerrado) y **`28/28` el 2026-10-06** en 8e-7b, con `ping` en `0.8.5` y `backend: advancesteel`. Log con 9 `pick` / 9 `picked` y 0 `window_error` | Bien |

Lo que no coincidió con lo escrito en el instalador, aunque el add-in hizo lo previsto en 10.2: el paso 8e-3.3 decía que, tras
pinchar un tramo del cordón superior, N9 pasaría a `✖ Sin plantilla que encaje`. La captura `fase8e-02-cordon-n9.png` enseña
que **se quedó en `✖ Falta el cordón`** con el detalle "cordón 1245531 HSS12X8X1/2 (llega, no pasa de largo)". Es lo que
hace el detector cuando el cordón se fija a mano (`NodeDetector`, paso 4: `ChordContinuous` solo si esa barra atraviesa el
nudo), y en 10.2 ya se dijo que en los siete empalmes del cordón superior "con Cordón… se fija uno de los dos tramos como
cordón, pero después tampoco hay plantilla para ellos". Lo que está mal es el **consejo**: después de que la persona haya
elegido el cordón, la columna *Qué hacer* sigue diciendo "Falta el cordón en la selección: selecciónalo y replanifica, o
Cordón…", que es un callejón sin salida. Va a la Fase 9 con los botones de *Qué hacer* (C3), ahora sí con un caso probado en
el PC (12.7).

### 12.2 Por qué tras el clic en la etiqueta los sondeos no pudieron hablar con Revit

El manejador de clics del sondeo 19 v3 hacía dos cosas en `OnClick`: escribir una línea en `sondeo19-clics.txt` y abrir un
cuadro con `UI.TaskDialog.Show(…)`. Ese cuadro es **modal y vive en el hilo de Revit**: mientras está abierto, Revit está
dentro del bucle de mensajes del cuadro y no vuelve a su bucle principal, así que no dispara `Idling` ni atiende los
`ExternalEvent`, que es por donde pyRevit Routes ejecuta en contexto de la API lo que le llega por HTTP. Las peticiones
siguientes se quedaron esperando: `19b-etiquetas-quitar.py` y el sondeo 17 de 8e-6 agotaron los 300 s de `revit-exec.ps1`
("Se canceló una tarea" es el `TaskCanceledException` del `HttpClient` en español, no un error de Revit), y el sondeo 12
**tardó 274 972 ms** (257 ms en la 8d): es el tiempo que esperó hasta que la persona cerró el cuadro, y entonces se ejecutó
entero y bien. Después el 13 tardó 172 ms, lo normal. Es decir: **Revit no se cerró ni se colgó**; estaba parado en el
cuadro. Lo confirma el log: ningún `startup` entre las 20:09 del 2026-10-05 y las 07:25 del día siguiente. Dos pistas más:
en `sondeo19-clics.txt` hay cuatro clics (la persona pinchó varias veces, cerrando el cuadro cada vez), y el archivo de
estado del sondeo 19 seguía existiendo al día siguiente (8e-7b: `estado del sondeo 19: control(es) 0 en la vista 1245519`),
señal de que las dos peticiones canceladas **no** se ejecutaron más tarde: pyRevit las descartó cuando el cliente se fue.

La lección para la Fase 10 (V3): el manejador de una etiqueta **nunca abre un cuadro**; hace lo suyo (seleccionar el nudo,
cambiar la etiqueta, escribir en el log o en la barra de estado de la ventana del plan) y vuelve. Y para los sondeos: lo que
se ejecute dentro de un manejador de Revit no puede bloquear el hilo de Revit, porque el siguiente sondeo no llega.

### 12.3 Qué cambió (sondeo 19 v4 y 19b; nada en `src/`, `config/` ni `mcp/`)

- **`scripts/sondeos/19-etiquetas-lienzo.py` (v4)**, que es la v3 tal cual (la que funcionó) con tres cambios:
  - **(a) La etiqueta B se pone.** El error `Multiple targets could match: ElementId(BuiltInParameter), ElementId(BuiltInCategory),
    ElementId(Int64)` es de IronPython, no de Revit: `ElementId` tiene exactamente esos tres constructores (comprobado en la
    nube contra `RevitAPI.dll` 2027.2.0) y un `int` de Python encaja en los tres. Se escribe `DB.ElementId(System.Int64(1249510))`,
    como ya hacían los sondeos 09, 10, 11, 12, 14, 16 y 17. Con la B puesta se sabrá si una etiqueta en el centro de la caja
    de sección se ve igual que la de N4 (la v3 lo dejó sin responder, aunque ya no es decisivo: la de N4 se vio).
  - **(b) El clic no abre ningún cuadro.** `OnClick` escribe la línea de siempre en `sondeo19-clics.txt` y, para que la persona
    vea que el clic llegó, **cambia la etiqueta pinchada por su versión naranja** con `UpdateControl(indice,
    InCanvasControlData(ruta_naranja, punto))` (miembro que el propio PC listó en 8e-5 y cuya firma
    `UpdateControl(Int32, InCanvasControlData)` se comprobó en la nube) y refresca la vista; cada paso deja su línea en el
    archivo (también si `UpdateControl` falla, en cuyo caso la etiqueta se queda como estaba). El sondeo genera por eso cuatro
    BMP (A, B y la versión naranja de cada una), todos de 24 bits y 32×32, y guarda las cuatro rutas en el estado para que 19b
    las borre. El manejador recibe `TemporaryGraphicsCommandData`, que solo tiene `Index` y `Document` (comprobado): con el
    índice y el diccionario `controles` del sondeo se sabe qué etiqueta fue.
  - **(c) El archivo de clics se vacía al empezar** (nuevo paso `0)` con cuántas líneas tenía): en 8e-7b, 19b enseñaba los
    cuatro clics del día anterior como si fueran de esa pasada.
- **`scripts/sondeos/19b-etiquetas-quitar.py`**: solo llama a `RemoveControl` con los índices que `GetAll()` dice que existen
  (tras un reinicio de Revit no queda ninguno y `RemoveControl(0)` avisaba `index is out of range`); los que no están se
  anotan como "ya no estaba". Borra también los BMP naranjas.
- **Sin versión nueva ni despliegue**: no cambia nada de `src/`, `config/`, `mcp/` ni del contrato. En el PC basta un
  `git pull` para tener el sondeo v4; la 0.8.5 desplegada es la buena.

### 12.4 Qué se probó en la nube y cómo

```text
$ apt-get install -y dotnet-sdk-10.0                          → SDK 10.0.112 (como en las fases anteriores)
$ dotnet build MotorConexiones.sln -c Release --nologo        → Build succeeded. 0 Warning(s) 0 Error(s)
$ dotnet test MotorConexiones.sln -c Release --no-build       → Passed! Failed: 0, Passed: 177, Total: 177
$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py   → correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar      → Autocomprobación: 47/47 correctas
$ python3 mcp/pruebas/simulador_revit.py & python3 mcp/pruebas/probar_conexiones.py → Resultado: 26/26 pruebas correctas (addin_version 0.8.5)
$ (MetadataLoadContext sobre RevitAPI.dll y RevitAPIUI.dll 2027.2.0 del paquete NuGet)
    TemporaryGraphicsManager: AddControl(InCanvasControlData, ElementId), UpdateControl(Int32, InCanvasControlData), RemoveControl(Int32),
      SetVisibility(Int32, Boolean), SetTooltip(Int32, String), GetAll(), Clear(), GetTemporaryGraphicsManager(Document)
    InCanvasControlData: ctor(String imagePath, XYZ position), ImagePath, Position
    ElementId: ctor(BuiltInParameter), ctor(BuiltInCategory), ctor(Int64)          ← los tres que confundían a IronPython
    ITemporaryGraphicsHandler: OnClick(TemporaryGraphicsCommandData); TemporaryGraphicsCommandData: Document, Index
```

El build y las pruebas no cambian respecto a 11.4 (este cierre no toca C#): se repiten para dejar constancia de que el
repositorio compila y está en verde en el commit del cierre.

### 12.5 NO PROBADO en Revit y por qué

- **El sondeo 19 v4** (la etiqueta B puesta, el clic sin cuadro y `UpdateControl` a naranja): no hay Revit en la nube. Se
  ejecuta en la instalación de la Fase 9 (si cabe) o en la de la Fase 10, con el bloque de 12.7. `UpdateControl` nunca se ha
  llamado desde un `OnClick`; si Revit no lo admite en ese contexto, la línea del archivo lo dirá y la etiqueta se quedará
  verde (el clic seguirá anotado).
- **Plantilla… cancelado**, **Editar nudo con la ventana abierta** y **Planificar lote con la ventana abierta** (la persona
  lo hizo con la ventana cerrada): sin anotación ni rastro en el log. **Ver en Revit "sin ningún cuadro"**: el log no tiene
  fallos, pero la persona no lo anotó. No bloquean: se miran de paso en la instalación de la Fase 9.
- Los pendientes de la 8d que siguen igual: el cordón inferior (si existe como barra) y Descartar con un plan marcado en
  **otra** vista.

### 12.6 Decisiones del cierre

- **La Fase 8 se cierra sin más rondas.** La 0.8.5 hizo en el PC todo lo que la 8d rompió (Cordón… y Barras… con la ventana
  oculta y de vuelta, Descartar que cierra la ventana) y lo que faltaba (Excluir / Incluir y Replanificar con la ventana
  abierta), con el puente en 28/28. Lo que queda sin anotar (12.5) es pequeño y se mira de paso en la Fase 9; abrir una 8f
  costaría una tarde más del PC para no cambiar nada del add-in.
- **V3 (etiquetas pinchables en la vista) va a la Fase 10: sí.** Revit dibuja un `InCanvasControlData` con un BMP de 24 bits
  de 32×32 en una ruta ASCII, tras `SetVisibility` y refresco, y el manejador de `TemporaryGraphicsHandlerService` recibe el
  clic con el índice del control. No se sabe cuál de las cinco hipótesis de la v3 (bits, ruta, visibilidad, refresco, punto)
  era la buena, porque se probaron juntas y a la primera; la Fase 10 hace las cinco, que son baratas. Y su manejador no abre
  nunca un cuadro (12.2): cambia la etiqueta, selecciona el nudo y escribe en la barra de estado de la ventana del plan.
- **El sondeo 19 v4 no abre una ronda**: es un sondeo, no el add-in, y la Fase 10 lo necesita antes que nadie. Se ejecuta con
  la instalación de la Fase 9 si hay tiempo, y si no, con la de la 10.
- **No se toca `PlanAdvice` para el consejo del empalme** (12.1, último párrafo), igual que decidió 10.2: va con los botones
  de *Qué hacer* de la Fase 9, que es donde ese texto se convierte en acciones. Queda anotado con la captura como caso probado.
- **Sin versión nueva**: nada de `src/` cambia, así que no hay 0.8.6 ni despliegue; la 0.8.5 del PC es la del repositorio.

### 12.7 Pendientes y qué sigue

- **Fase 9 (crear el lote)**, en una sesión nueva con el prompt del paso 6 de la sección 6. Antes de lanzarla, la persona
  decide: **P6**, los 14 nudos con cordón HSS4X4 (¿se crean con la cartela del Detalle D, con el aviso de perfil, o se dejan
  fuera?), y qué hacer con los 10 nudos del cordón superior sin plantilla y los 7 empalmes (¿se excluyen, o se crea uno a mano
  y se guarda como plantilla antes?). Dentro de la Fase 9 va también el consejo del empalme (12.1): cuando el cordón se fijó
  a mano y no pasa de largo, *Qué hacer* debe decir algo como "El cordón termina en este nudo (empalme): ninguna plantilla
  encaja con dos diagonales; crea esa típica o excluye", no "selecciónalo y replanifica".
- **Sondeo 19 v4 en el PC** (5 minutos, Revit abierto en la vista 3D con el Detalle D visible y ninguna ventana del add-in):

  ```powershell
  Anota "8e-9 sondeo 19 v4" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
  # (la persona) mira la 4 verde en N4 y la B azul; pincha la 4: NO sale ningún cuadro y la 4 pasa a naranja
  Anota "8e-9 sondeo 19b" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
  ```

  Se espera: `0) archivo de clics vaciado`, `4)` cuatro BMP de 24 bits, `5)` las dos posiciones, `6)` dos controles (índices
  0 y 1) y `GetAll(): 2`, `9)` "NO sale ningun cuadro"; tras el clic, 19b enseña `clic en una etiqueta: Index=0` seguido de
  `UpdateControl(0): la etiqueta A pasa a naranja (el clic llego)`, `GetAll() antes de quitar: 2`, los dos controles quitados y
  `GetAll() despues de quitar: 0`. Los sondeos siguientes tienen que responder al momento (nada de 300 s).
- Lo de 12.5 que se mira de paso en la instalación de la Fase 9: Plantilla… cancelado, Editar nudo y Planificar lote con
  la ventana abierta, Ver en Revit sin cuadro (anotado), el cordón inferior y Descartar con un plan marcado en otra vista.
- Los pendientes anteriores (8.7, 9.6, 10.8 y 11.8) siguen igual salvo lo cerrado aquí.
