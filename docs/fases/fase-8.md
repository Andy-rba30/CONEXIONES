# Fase 8: detección de nudos y plan de lote (marcas en el modelo, sin crear nada)

Fecha: 2026-10-04 (ronda 8b: 2026-10-05). Rama: `main`. Add-in **0.8.0** en la Fase 8 y **0.8.1** en la ronda 8b (Core con
`Batch/`; add-in con `Batch/`, tres operaciones nuevas y el botón **Planificar lote** con su ventana); adaptador 0.8.1
(23 rutas) y 21 herramientas `conn_*`. Prompt y alcance: `docs/prompts/fase-8.md`, escrito a partir de las secciones
3.1, 3.2, 3.4, 4 y 6 de `docs/propuestas/catalogo-y-lotes.md` y de las decisiones de su sección 7.1 (P3, P6, P7, P8, P10,
P11, P12, P13).

**Estado: CERRADA el 2026-10-05 (sección 8).** Probada en el PC el 2026-10-04 (`resultados-fase-8.md`: marcas y ventana
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

### 4.7 Ventana modal con bucle en el comando (P10 y P11)

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
