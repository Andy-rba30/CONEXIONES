# Fase 8: detección de nudos y plan de lote (marcas en el modelo, sin crear nada)

Fecha: 2026-10-04. Rama: `main`. Add-in **0.8.0** (Core con `Batch/`; add-in con `Batch/`, tres operaciones nuevas y el
botón **Planificar lote** con su ventana); adaptador 0.8.0 (23 rutas) y 21 herramientas `conn_*`. Prompt y alcance:
`docs/prompts/fase-8.md`, escrito a partir de las secciones 3.1, 3.2, 3.4, 4 y 6 de `docs/propuestas/catalogo-y-lotes.md`
y de las decisiones de su sección 7.1 (P3, P6, P7, P8, P10, P11, P12, P13).

**Estado: escrita y probada en la nube; PENDIENTE DE INSTALADOR** (`docs/instalacion/fase-8.md`, unos 45 minutos). Esta
fase **no crea ninguna conexión**: planifica y marca; crear el lote es la Fase 9.

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

6. **Solo con la 8 cerrada**, lanzar la Fase 9 con una sesión nueva:

   ```
   Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-8.md y docs/propuestas/catalogo-y-lotes.md completo.
   Escribe docs/prompts/fase-9.md (crear por lotes: secciones 3.5, 4 y 6 de la propuesta, con las decisiones de 7.1)
   y ejecuta SOLO la Fase 9. Termina con docs/fases/fase-9.md, docs/instalacion/fase-9.md, commit, push y un resumen corto.
   ```
