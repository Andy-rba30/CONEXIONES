# Fase 8: detección de nudos y plan de lote (marcas en el modelo, sin crear nada)

Segunda de las tres fases que salen de `docs/propuestas/catalogo-y-lotes.md` (sección 6: Fase 7 catálogo, Fase 8
detección de nudos y plan, Fase 9 crear por lotes). Se ejecuta en **una sesión**, con el prompt de la sección 1. Lo demás
de este archivo es el alcance que esa sesión debe cumplir, escrito a partir de las secciones 3.1, 3.2, 3.4, 4 y 6 de la
propuesta y de las respuestas de su sección 7.1 (P3, P6, P7, P8, P10, P11, P12, P13).

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-7.md y docs/propuestas/catalogo-y-lotes.md completo.
Escribe docs/prompts/fase-8.md (deteccion de nudos y plan: secciones 3.1, 3.2, 3.4, 4 y 6 de la propuesta, con las
decisiones de 7.1) y ejecuta SOLO la Fase 8. Termina con docs/fases/fase-8.md, docs/instalacion/fase-8.md, commit,
push y un resumen corto. Guarda ademas el resumen en docs/fases/resumen-fase-8.md por si el chat no se ve.
```

## 2. Por qué esta fase

La Fase 7 dejó el catálogo: una plantilla se aplica a **un** nudo (`conn_catalog_apply`, botón **Catálogo**). La persona
quiere seleccionar la cercha entera y que la típica se adapte a cada nudo. Antes de crear nada hace falta saber **qué
nudos hay y qué barras tiene cada uno**, casar la plantilla con cada nudo, validar nudo a nudo y enseñarlo en Revit para
corregir a mano (propuesta 3.1: planificar → revisar → aplicar, con parada obligatoria en medio). Esta fase hace los dos
primeros pasos y **no crea nada**; crear el lote es la Fase 9.

## 3. Alcance (lo que se entrega)

### 3.1 Flujo (propuesta 3.1)

```
1. PLANIFICAR   selección de la cercha + plantillas  →  plan: nudos detectados (N1, N2…), barras de cada nudo, cordón,
                                                        plantilla casada y orientación, especificación instanciada,
                                                        validación y token por nudo, nudos sin encaje o sin tipo
                                                     →  marcas en el modelo (un color por nudo, cordón con línea gruesa,
                                                        marcador con el nombre en el punto de trabajo)
2. REVISAR      la persona orbita en Revit y mira las marcas; corrige: excluir nudo, cambiar cordón, cambiar plantilla,
                quitar o añadir una barra, añadir un nudo, fundir o separar nudos, editar un nudo en la ventana 2D
                → se replanifica (mismo plan_id, nombres de nudo estables)
3. (Fase 9)     APLICAR crea nudo a nudo con los tokens del plan y quita las marcas
```

Para la IA: `conn_batch_plan` → tabla al usuario (y `get_revit_view` para enseñar las marcas) → correcciones como
`overrides` en otra llamada a `conn_batch_plan` con el mismo `plan_id` → `conn_batch_plan_get` para releer el plan →
`conn_batch_plan_discard` quita las marcas y olvida el plan. Para la cinta: botón **Planificar lote** con la ventana del
plan (modal, decisión P10: se cierra para mirar el modelo y se vuelve a abrir). **Descartar plan** quita las marcas.

### 3.2 Detección de nudos (propuesta 3.2; Core, sin Revit)

Entrada: las barras seleccionadas (eje inicio–fin en mm, tipo). Salida: lista de nudos.

1. **Puntos candidatos**: los extremos de todas las barras, agrupados a menos de `node_cluster_mm` (P7: 10 mm,
   `config/catalog.json`). Cada grupo es un nudo candidato con su punto de trabajo (media de los extremos).
2. **Barras que llegan**: las que tienen un extremo en el grupo (diagonales, montantes).
3. **Barras que atraviesan**: las que pasan a menos de `node_axis_max_distance_mm` (5 mm) del punto sin terminar en él:
   candidatas a cordón continuo. Si hay más de una (cruce de cordones), el nudo queda `ambiguous_chord` y pide corrección.
4. **Cordón**: la barra que atraviesa; si ninguna, la más horizontal de las que llegan (como `ChooseChord`) y el nudo
   queda con `chord_continuous = false`.
5. **Marco canónico** del nudo (`NodeFrame.Compute`, Fase 7) y ángulo con signo de cada barra que llega. Si los ejes
   distan más de 5 mm, el nudo queda `offset` (desfase) y no se le aplica nada.
6. **Firma**: número de barras por lado (+Y / −Y) y sus ángulos. Un nudo con una sola barra que llega y sin cordón
   reconocible (empalme simple, extremo suelto) es `untyped` y no se le aplica nada.
7. **Nombre** `N1, N2, …` (P12) estable para la misma selección: ordenados por la coordenada global de mayor extensión de
   la cercha (X en el Hangar), después por las otras dos. Un nudo añadido a mano recibe el siguiente número libre.
8. **Conexiones existentes**: si alguna barra del nudo ya está en una conexión del add-in, el nudo es
   `already_connected` y se salta (P8: `replace_existing: false` por defecto; con `true` se planifica y queda marcado
   para que la Fase 9 lo rehaga con `conn_update`).

Se prueba en la nube con una cercha sintética (sección 5).

### 3.3 Casado, instanciación y validación por nudo (propuesta 3.3, ya en Core desde la Fase 7)

Por cada nudo con cordón y barras: `TemplateMatcher.Match` con cada plantilla pedida (varias plantillas, propuesta 3.6:
cada nudo toma la que casa todas sus ranuras con menor desvío; a igualdad, la primera de la lista), orientación
`same` / `mirror_x` / `mirror_y` / `both` (P6: los nudos en espejo se aplican y se **remarcan**: columna `orientation`
y marcador distinto en el modelo), `TemplateInstantiator.Instantiate` con `source.template_id` y `source.batch_id` =
`plan_id` (P13), y después **`conn_validate` normal** (mismas reglas, mismo token). Estados del nudo en el plan:

| `status` | Significado | Qué hacer |
|---|---|---|
| `ready` | Casa, valida y tiene `validation_token` (puede traer avisos: `warnings_count`, P3) | Nada; la Fase 9 lo creará |
| `invalid` | Casa pero `conn_validate` da errores (por ejemplo `PLATE_OUTSIDE_GUSSET`) | Editar el nudo en la ventana 2D (`overrides.spec`) u otra plantilla |
| `no_match` | Ninguna plantilla casa todas las ranuras (`attempts` lo explica) | Añadir o quitar barras, cambiar cordón, otra plantilla o excluir |
| `ambiguous_chord` | Más de una barra atraviesa el nudo | `overrides.chord` |
| `offset` | Los ejes no se cortan (> 5 mm) | Arreglar el modelo o excluir |
| `untyped` | Una sola barra, sin cordón reconocible | Nada (no se le aplica plantilla) |
| `already_connected` | Ya tiene conexión del add-in | Se salta; `replace_existing: true` lo planifica |
| `excluded` | La persona lo excluyó | — |

### 3.4 Marcas en el modelo y corrección manual (propuesta 3.4, opción A, decisión P10)

- **Marcas**: en la vista activa, cada nudo planificado colorea sus barras (`View.SetElementOverrides` con
  `OverrideGraphicSettings`: color de línea y de superficie, cordón con línea gruesa) con un color de una paleta fija
  (12 colores que se repiten) y coloca un **marcador** `DirectShape` pequeño (Modelos genéricos, `ApplicationId`
  `MotorConexiones.Plan`) en el punto de trabajo con el nombre del nudo (`Name` y `Mark` = `N1`, `Comments` con el plan y
  los IDs de sus barras, para poder limpiar aunque el add-in haya olvidado el plan): cubo para `same`, octaedro
  (rombo) para las orientaciones en espejo (P6). Todo dentro de un `TransactionGroup` del plan (una entrada de
  deshacer). Los nudos excluidos, sin tipo o ya conectados no se marcan. **Descartar** (o replanificar) quita las
  marcas: restaura los overrides y borra los marcadores. Las API de marcas se comprueban con el sondeo 17 en el PC.
- **Correcciones** (`overrides` del MCP y acciones de la ventana, las mismas):
  - `exclude: ["N3", "N7"]`; `add_node: { "N11": [ids] }`; `chord: { "N4": 1249510 }`;
    `template: { "N9": "<template_id>" | null }`; `remove_member` / `add_member: { "N2": [ids] }`;
    `merge: [["N5", "N6"]]`; `split: { "N5": [[ids], [ids]] }`; `spec: { "N4": { ...especificación... } }`;
    `replace_existing: true|false`.
  - Cada corrección replanifica (vuelve a detectar, casar y validar) y devuelve el plan con el mismo `plan_id`; las
    correcciones se acumulan en el plan.
- **Ventana del plan** (solo cinta, modal): tabla de nudos (nombre, estado, orientación, cordón, barras, plantilla,
  desvío máximo, avisos, errores), detalle del nudo elegido, y botones **Ver en Revit** (cierra la ventana, selecciona y
  hace zoom al nudo; se vuelve a abrir sola), **Excluir / Incluir**, **Cordón… / Barras…** (P11: elegir de la lista
  **o pinchar en Revit**: la ventana se cierra, la persona pincha, y se vuelve a abrir replanificado), **Añadir nudo…**
  (pinchar las barras de un nudo que no se detectó), **Plantilla…**, **Editar nudo** (abre la ventana de la Fase 6 con la
  especificación instanciada; lo que se cambie allí sustituye a la instancia solo para ese nudo), **Replanificar**,
  **Guardar plan JSON** (en `Documentos\MotorConexiones\`), **Descartar plan** y **Cerrar** (deja las marcas y el plan
  para seguir después). **Aplicar lote** llega en la Fase 9.

### 3.5 Herramientas y botones

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_batch_plan` | `element_ids` (o la selección), `template_ids` (opcional: si falta, todas las del catálogo), `overrides`, `plan_id` (replanificar el mismo plan), `mark` (por defecto `true`). Devuelve `plan_id`, `nodes[]` con estado, barras, plantilla, orientación, desvío, errores, avisos y `validation_token`, `summary` por estado y `marks`. No crea nada. |
| MCP | `conn_batch_plan_get` | El plan guardado (`plan_id` o el último). Sin tocar el modelo. |
| MCP | `conn_batch_plan_discard` | Quita las marcas y olvida el plan (`plan_id` o el último; `all: true` limpia marcadores huérfanos). |
| Cinta | Botón **Planificar lote** | Ventana del plan (3.4) sobre la selección actual. |
| Guía | Sección nueva en `docs/guide.md` | Cuándo planificar, cómo leer la tabla, cómo corregir, que no crea nada. |

### 3.6 Arquitectura (propuesta 4, solo la parte de esta fase)

```
src/MotorConexiones.Core/
└── Batch/
    ├── NodeDetector.cs             segmentos → nudos (agrupar extremos, barras que llegan, barras que atraviesan, cordón, marco, nombres)
    ├── BatchOverrides.cs           exclusiones, cordón, plantilla, barras, añadir nudo, merge/split, spec por nudo (JSON del MCP y de la ventana)
    ├── BatchPlan.cs                plan: plan_id, nudos con estado, plantilla, spec, validación, token, marcas, resumen; JSON
    └── PlanBuilder.cs              detección + overrides + casado + instanciación + validación (delegada) → BatchPlan

src/MotorConexiones.Revit/
├── Batch/BatchPlanner.cs           selección → RevitModelFacts → PlanBuilder (validación con ValidationService) → marcas
├── Batch/PlanMarks.cs              colorear barras y marcar nudos; quitar marcas (también huérfanas)
├── Batch/PlanRegistry.cs           planes en memoria del add-in (plan_id → plan) hasta descartar o cerrar Revit
├── Operations/                     BatchPlan, BatchPlanGet, BatchPlanDiscard
├── BatchPlanCommand.cs             botón Planificar lote (bucle ventana → acción en Revit → ventana)
└── UI/BatchPlanWindow.xaml         nudos con estado y acciones; Editar nudo abre PreviewWindow

mcp/tools/conn_tools.py             conn_batch_plan, conn_batch_plan_get, conn_batch_plan_discard (21 herramientas)
mcp/revit_mcp/conexiones.py         /conn/batch/plan/, /conn/batch/plan/get/, /conn/batch/plan/discard/ (23 rutas)
config/catalog.json                 node_cluster_mm (10), node_axis_max_distance_mm (5) ya reservadas; plan_* nuevas
scripts/sondeos/17-marcas-plan.py   override de gráficos, patrón sólido, DirectShape marcador y limpieza, en Revit
src/MotorConexiones.Tests/          NodeDetectorTests (cercha sintética), BatchPlanTests (plan, overrides, estados)
```

## 4. Reglas técnicas de esta fase

- Unidades: solo `Units/UnitConverter.cs`. La detección trabaja en mm con `Vec3`.
- Una operación = un `TransactionGroup`: `batch_plan` (marcas) y `batch_plan_discard` son operaciones con su grupo
  asimilado; `batch_plan_get` no toca el modelo. Las marcas son cambios de vista y elementos pequeños, nunca acero.
- Sin ventanas en las rutas `conn_*`: las marcas no son ventanas; la ventana del plan solo existe en la cinta.
- Los tokens del plan son los de `conn_validate`; la Fase 9 los exigirá nudo a nudo.
- `ElementId.Value`; nada de miembros de la API sin compilar (la nube compila contra la API 2027) o sondear (el
  efecto visual de las marcas: sondeo 17).
- Código y claves JSON en inglés; mensajes, textos de las ventanas y documentación en español.
- Versión **0.8.0** en add-in, adaptador y herramientas (regla de la ronda 7b: `0.N.0` = última fase).

## 5. Pruebas en la nube

- Cercha sintética en `Tests/Fakes/SyntheticTruss.cs`: cordón central horizontal con 6 nudos del tipo del Detalle D
  (dos diagonales arriba y una abajo), la mitad derecha en espejo (`mirror_x`), cordón superior e inferior (nudos de
  dos barras: sin encaje), un cordón dibujado al revés, una barra suelta (`untyped`), un nudo con desfase de 8 mm
  (`offset`), un cruce de dos cordones (`ambiguous_chord`), dos extremos a 6 mm (se agrupan) y a 40 mm (no).
- `NodeDetectorTests`: número de nudos, nombres estables, cordón y barras por nudo, ángulos con signo, estados.
- `BatchPlanTests`: plan con la plantilla del Detalle D (`ready` en los seis nudos del cordón central, `mirror_x` en
  tres, tokens de 64 caracteres distintos, `source.batch_id`), `no_match` en los cordones exteriores, `exclude`,
  `chord`, `template: null`, `remove_member` + `add_member`, `merge` / `split`, `add_node`, `spec` por nudo,
  `already_connected` con y sin `replace_existing`, JSON del plan ida y vuelta, `overrides` desde JSON.
- `simulador_revit.py --autocomprobar` con las 23 rutas; `probar_conexiones.py` con `batch_plan` sobre el nudo del
  fixture, `batch_plan_get` y `batch_plan_discard` (26 pruebas; 28 con `--puente`).

## 6. Instrucciones para el instalador (`docs/instalacion/fase-8.md`)

Una ronda sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, con `Anota` y resultados en `docs/fases/resultados-fase-8.md`:

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado, `instalar-conn.ps1` (23 rutas, 21 herramientas).
2. Abrir Revit: `conn_ping` con `batch_plan`, `batch_plan_get`, `batch_plan_discard` y `0.8.0`.
3. Sondeo 17 (marcas): override de color sobre una barra, patrón sólido, marcador DirectShape y limpieza; captura.
4. **(la persona)** seleccionar la cercha entera (los cordones y todas las diagonales y montantes de la cercha del
   Detalle D) → `conn_batch_plan` por `conn-call.ps1` con la plantilla oficial: captura con los colores y los marcadores
   `N1…`; devolver la tabla de nudos (estado por nudo, orientación, tokens).
5. Corregir un cordón (`overrides.chord`) y excluir un nudo → replanificar con el mismo `plan_id`.
6. Botón **Planificar lote** con la misma selección: ventana, **Ver en Revit**, **Editar nudo**, **Descartar plan**;
   capturas.
7. `conn_batch_plan_discard`; sondeos 12 y 13 en cero; ningún marcador queda (`batch_plan_discard` con `all: true`).
8. `probar_conexiones.py --puente` (28/28).

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; `simulador_revit.py --autocomprobar`
  con las rutas nuevas; `py_compile` de todo `mcp/` y `scripts/sondeos/`.
- `docs/fases/fase-8.md` según el Anexo B del encargo, con la lista de lo NO PROBADO.
- `docs/instalacion/fase-8.md` literal, con `Anota`, capturas con nombre fijo y qué devolver.
- README, `mcp/CONTRATO-conn.md`, `docs/guide.md` (sección "Lotes: planificar") y `CLAUDE.md` (estructura) al día.
- `docs/fases/resumen-fase-8.md` con el resumen del chat.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta fase

- Crear el lote, `conn_batch_create`, `conn_batch_delete`, `conn_list` por lote, botón **Aplicar lote**, sondeo de
  grupos anidados con Advance Steel (Fase 9).
- Ventana del plan no modal con `ExternalEvent` (opción B de 3.4; queda anotada en la propuesta, sección 5.11).
- `outline.mode = "auto"` (Fase 10) y `conn_batch_update`.
- Nudos viga-columna o de apoyo en columnas (otro tipo de conexión).
