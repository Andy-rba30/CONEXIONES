# Propuesta: catálogo de conexiones y aplicación por lotes (nudos de un pórtico entero)

Fecha: 2026-10-01. Estado: **propuesta aclarada con la persona (sección 7.1), lista para convertirse en fases**. No hay
código de esta idea en el repositorio. De aquí salen los prompts de las fases 7, 8 y 9
(`docs/prompts/fase-N.md`), una por sesión, como se hizo con la Fase 6.

Las dos ideas, en palabras de la persona:

1. **Catálogo**: guardar las conexiones para no tener que buscar el archivo JSON cada vez.
2. **Por lotes**: tener una conexión típica (creada o sacada del catálogo), seleccionar los elementos de un pórtico
   entero y que esa conexión se adapte a cada nudo. Antes de crear, una previsualización 3D para comprobar que los
   nudos y las barras se identificaron bien; si no, corregir a mano. En acero hay pequeñas variaciones de ángulo o
   desfase, y los pórticos se repiten cada cierta distancia (1 m, 1 m, 2 m, 2 m…) con la misma idea y distinto largo.

---

## 1. Mapa de la situación actual (lo que hay y lo que condiciona el diseño)

### 1.1 Qué hay hoy (Fases 0 a 6, todo probado en el PC)

| Pieza | Estado | Dónde |
|---|---|---|
| Contrato v1 `gusset_node` (cordón HSS continuo + diagonales/montantes ranurados y soldados o con placa cuchilla empernada) | Cerrado y probado | `src/MotorConexiones.Core/Contract/`, `Schema/` |
| Validación (10 reglas, mínimos AISC en `config/limits.json`) y `validation_token` | Cerrado | `Core/Validation/` |
| Nudo: sistema local, "la barra llega al nudo", direcciones 2D | Cerrado | `Core/Geometry3D/NodeFrame.cs`, `NodeReach.cs`, `ConnectionGeometry.cs`; `Revit/Node/NodeInspector.cs` |
| Creación con Advance Steel (placas y pernos nativos en mm), soldaduras `DirectShape`, retiros de barras, Extensible Storage, borrar y actualizar | Cerrado | `Revit/Services/ConnectionCreationService.cs`, `Fabrication/`, `Storage/` |
| 13 herramientas MCP `conn_*` y guía para la IA | Cerrado, 19/19 por el puente | `mcp/`, `docs/guide.md` |
| Ventana de previsualización 2D con cotas y tabla editable; borrado desde la cinta; panel en la pestaña ARBA | Fase 6: probada en el PC (`resultados-fase-6.md`: ventana, crear 9 elementos, borrar, 19/19) | `Revit/UI/`, `RunSpecCommand.cs`, `ListConnectionsCommand.cs` |
| Pernos con agarre real: `plate.gusset_face`, `bolts.length_mm`, longitud calculada de `limits.json`, cotas editables con doble clic | Rondas 6b y 6c probadas en el PC; **ronda 6d (placas centradas en su plano) pendiente de instalador** | `Core/Geometry3D/BoltStack.cs`, `Fabrication/AdvanceSteelBackend.cs`, `docs/instalacion/fase-6d.md` |

Nota de estado: la Fase 6 sigue abierta en `docs/fases/fase-6.md` con las rondas 6b, 6c y 6d (secciones 6 a 8); la 6d
está escrita y pendiente del instalador. Esta propuesta no toca nada de eso: las fases 7 a 10 empiezan cuando la 6d
esté cerrada. Las 99 pruebas del Core y la compilación sin avisos son el punto de partida.

### 1.2 Cómo funciona hoy el flujo de una conexión (y qué le falta para catálogo y lotes)

```
Plano → IA lee el detalle → JSON con element_ids del nudo → conn_validate (token) → conn_preview → conn_create
                                 ↑
Botón de la cinta: elegir archivo JSON → ventana (croquis + tabla + validación) → Crear
```

Hechos del sistema actual que condicionan el diseño:

1. **La especificación está atada a un nudo concreto.** `node.element_ids`, `chord.element_id` y
   `members[i].element_id` son IDs del modelo. El `validation_token` firma la especificación, el documento, los
   límites y, por cada barra, su `UniqueId`, tipo y extremos de eje redondeados a 0,1 mm. Un JSON del Detalle D no se
   puede "aplicar" a otro nudo sin cambiar esos IDs y volver a validar. **Un catálogo guarda plantillas, no
   especificaciones**: lo mismo pero sin IDs y con una regla para casar barras.
2. **Casi toda la geometría ya se adapta sola al nudo.** Las direcciones de las barras salen del modelo
   (`ConnectionGeometry.GetMemberDirection2D`), y a partir de ellas se colocan los retiros, las ranuras, la placa
   cuchilla, los pernos y las soldaduras. Una diagonal a 43° o a 47° recibe la misma unión sin tocar el JSON. Esto es
   lo que hace viable "la típica se adapta": ya ocurre hoy para todo menos la cartela.
3. **La cartela no se adapta: es un polígono fijo** (`gusset.outline.points_mm`, en mm en el sistema local). Con
   variaciones pequeñas de ángulo sigue valiendo (el plano dibuja una cartela "típica" precisamente por eso); con
   variaciones grandes, las barras pueden salirse de la cartela y el validador lo detecta (`PLATE_OUTSIDE_GUSSET`,
   `BOLT_OUTSIDE_PLATE`). El encargo reservó `outline.mode = "auto"` para v2: una cartela calculada a partir de las
   barras. Es la pieza que haría la plantilla independiente de los ángulos.
4. **El sistema local depende del sentido del cordón y del primer miembro.** X va del inicio al fin de la curva del
   cordón; Z se normaliza para apuntar hacia +Z global; Y = Z × X. Si en otro nudo el cordón está dibujado en sentido
   contrario, X e Y se invierten y una cartela guardada aparecería reflejada. Además la diagonal inferior del Detalle D
   sale a 135° en el modelo frente a 45° del plano (pendiente P3 de la Fase 6) y +Y local apunta hacia abajo en el
   Hangar (P4). **Para plantillas hace falta una orientación canónica** o probar la plantilla en sus 4 reflexiones
   (sección 3.3).
5. **Dos reglas de validación serían un estorbo en un lote si no se ajustan**: `PROFILE_MISMATCH` es un **error** si
   el perfil escrito no es el del modelo (una plantilla con `HSS2-1/2…` aplicada a un nudo con otro HSS no validaría), y
   `ANGLE_DIFFERS_FROM_MODEL` es solo aviso (bien). La plantilla necesita decir qué es exigencia y qué es "tomar del
   modelo".
6. **Las dudas (`uncertain_fields`) bloquean el token.** Una plantilla debe guardarse ya sin dudas abiertas (o con
   todas confirmadas); si no, cada nudo del lote arrastraría las mismas preguntas.
   Lo mismo vale para los campos que decide el plano y no el modelo (`plate.gusset_face`, `bolts.length_mm` de la
   ronda 6b): van en la plantilla tal cual y se repiten en cada nudo; lo que se calcula del agarre
   (`data.bolt_stacks` de `conn_validate`) se recalcula nudo a nudo con los espesores reales.
7. **Una operación = un `TransactionGroup`; error = rollback completo.** Para 20 nudos hay que decidir la unidad de
   atomicidad (sección 4.1).
8. **Sin ventanas en el camino de la IA.** El catálogo y el lote tienen que existir como herramientas `conn_*` sin
   ventana y, aparte, como botones de la cinta con ventana. La lógica se escribe una vez (Core + servicios del add-in)
   y se usa desde los dos caminos, igual que hoy `ConnectionCreationService`.
9. **La ventana de la Fase 6 ya es la "previsualización 2D con corrección" de un nudo.** Se reutiliza tal cual para
   corregir un nudo concreto del lote; no hace falta otra.
10. **El Core se prueba en la nube sin Revit.** La detección de nudos, el casado de barras y la instanciación de
    plantillas se pueden escribir y probar con cerchas sintéticas (coordenadas inventadas pero realistas) antes de
    tocar Revit, como se hizo con el croquis.

### 1.3 Qué significa "pórtico" aquí (supuesto que hay que confirmar)

v1 solo conoce `gusset_node`: un cordón de **armazón estructural** (Structural Framing) que atraviesa el nudo y
barras que llegan a él. Un pórtico de columnas y vigas (nudo viga-columna, placa base) es otro tipo de conexión, fuera
de v1. En este documento "pórtico" se entiende como **cercha o armadura de barras HSS repetida** (como la del Hangar):
cordones continuos y diagonales/montantes. Si la persona quiere también nudos viga-columna, antes hace falta un tipo
nuevo (`beam_column` o `base_plate`), que la arquitectura admite (README, sección 7) pero que es un trabajo aparte.

**Confirmado por la persona (2026-10-01)**: es una cercha. La referencia es `docs/propuestas/cercha-referencia.png`:
una cercha de cubierta apoyada en columnas, 9 vanos entre los ejes 2 y 10, cordón superior e inferior con diagonales
y montantes en cada vano; el nudo del Detalle D se creó en uno solo de esos vanos y la idea es repetirlo en el resto.
Los nudos donde la cercha apoya en las columnas son otro tipo de conexión y quedan fuera hasta que exista ese tipo.

---

## 2. Idea 1: catálogo de conexiones

### 2.1 Qué es

Una colección de **plantillas** con nombre, que se pueden abrir y aplicar sin buscar archivos. Una plantilla es una
especificación `gusset_node` completa **sin IDs de elementos** y con un **patrón de barras** que dice cómo casar cada
barra de la plantilla con una barra del nudo real.

```json
{
  "catalog_version": "1.0",
  "template_id": "a3f1…",
  "name": "Nudo típico cordón inferior, 3 diagonales (Detalle D)",
  "description": "Cartela PL 3/8\" 565x530, diagonales ranuradas, inferior con placa cuchilla PL10 y 4 pernos 5/8\"",
  "connection_type": "gusset_node",
  "tags": ["hangar", "cercha", "HSS"],
  "created_utc": "2026-10-01T15:00:00Z",
  "origin": { "connection_id": "05f44106-…", "document": "HANGAR_PRUEBA", "drawing": "Detalle D" },
  "member_pattern": [
    { "slot": 0, "role": "diagonal", "angle_deg": 43.1, "side": "+Y", "profile": "HSS2-1/2X2-1/2X3/16", "profile_policy": "warn" },
    { "slot": 1, "role": "vertical", "angle_deg": 90.0, "side": "+Y", "profile": "HSS2-1/2X2-1/2X3/16", "profile_policy": "warn" },
    { "slot": 2, "role": "diagonal", "angle_deg": 135.6, "side": "-Y", "profile": "HSS2-1/2X2-1/2X3/16", "profile_policy": "warn" }
  ],
  "chord_pattern": { "profile": "HSS3X3X1/4", "continuous": true, "profile_policy": "warn" },
  "matching": { "angle_tolerance_deg": 10.0, "allow_mirror": true },
  "spec_template": {
    "spec_version": "1.0",
    "connection_type": "gusset_node",
    "source": { "drawing": "Detalle D", "scale": "1/10" },
    "chord": { "profile": "HSS3X3X1/4", "continuous": true },
    "gusset": { "...": "igual que hoy, contorno en mm en el sistema local canónico" },
    "members": [ { "slot": 0, "role": "diagonal", "end_setback_mm": 180, "attachment": { "...": "igual que hoy" } }, "…" ],
    "dimension_chains": [ "…" ],
    "uncertain_fields": []
  }
}
```

Lo que **se quita** respecto a una especificación: `node.element_ids`, `chord.element_id`, `members[].element_id`.
Lo que **se añade**: `member_pattern` (ángulo canónico, lado y perfil esperado por barra), `chord_pattern`, `matching`
y los metadatos (`template_id`, `name`, `tags`, `origin`). `uncertain_fields` debe estar vacío o totalmente confirmado.

### 2.2 De dónde sale una plantilla (tres caminos, el mismo código)

1. **De una conexión ya creada** (`conn_catalog_save` con `connection_id`): el camino natural. La IA lee el detalle,
   crea el nudo una vez, se comprueba en Revit, y "guárdala como típica". El add-in lee la especificación guardada en
   Extensible Storage, mide los ángulos reales de las barras y escribe el patrón.
2. **De un JSON validado** (`conn_catalog_save` con `spec`): sin crear nada; los ángulos salen del nudo al que apunta
   el JSON (hace falta el modelo abierto).
3. **Desde la ventana** (botón "Guardar en catálogo" en la ventana de previsualización): lo mismo que 2, con nombre y
   etiquetas en un diálogo pequeño.

### 2.3 Dónde vive el catálogo (recomendación)

**Carpeta en disco del usuario, un archivo JSON por plantilla**:
`%LOCALAPPDATA%\MotorConexiones\catalogo\<template_id>.json` (misma carpeta donde ya está el log). Razones: se edita
y se copia sin recompilar ni abrir Revit, sobrevive a cambiar de modelo, se puede versionar copiándola al repositorio
(`catalog/` con las plantillas "oficiales" de la oficina, que `deploy.ps1` copia si se quiere), y no requiere tocar el
esquema de Extensible Storage. La ruta se declara en `config/catalog.json` para poder apuntar a una carpeta compartida
de red.

Alternativas descartadas por ahora: dentro del modelo `.rvt` (viaja con el proyecto, pero no sirve entre proyectos y
obliga a esquema nuevo de Extensible Storage); solo en el repositorio (obliga a `git` para añadir una plantilla).

Compromiso posible más adelante: `conn_catalog_export` / `import` para pasar plantillas entre PCs o guardar una copia
dentro del modelo.

### 2.4 Herramientas y botones

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_catalog_list` | Nombre, id, tipo, etiquetas, número de barras, fecha. Sin volcar la plantilla entera. |
| MCP | `conn_catalog_get` | La plantilla completa. |
| MCP | `conn_catalog_save` | Desde `connection_id` o desde `spec`; `name`, `description`, `tags`. Devuelve `template_id`. Sobrescribir exige `overwrite: true`. |
| MCP | `conn_catalog_delete` | Borra el archivo (con confirmación del usuario, como `conn_delete`). |
| MCP | `conn_catalog_apply` | `template_id` + `element_ids` (o la selección) de **un** nudo → especificación instanciada + resultado de `conn_validate` + token. Es el primer uso real del casado de barras (sección 3.3) y la base del lote. |
| Cinta | Botón **Catálogo** | Lista con búsqueda; **Aplicar a la selección** abre la ventana de la Fase 6 con la especificación instanciada (archivo virtual: "Guardar JSON" escribe en Documentos); **Guardar desde conexión** (elige una conexión del modelo); **Eliminar**. |
| Ventana Fase 6 | **Guardar en catálogo** y **Abrir del catálogo** | Dos botones más en la barra de abajo. |
| Guía | Sección nueva en `docs/guide.md` | Cuándo guardar, cuándo aplicar, que `conn_catalog_apply` ya valida. |

Trazabilidad: la especificación instanciada lleva `source.template_id` (y más adelante `source.batch_id`). Así la
conexión creada "sabe" de qué plantilla viene **sin cambiar el esquema de Extensible Storage** (basta ampliar
`source` en el JSON Schema del Core). Esto permite en el futuro "cambió la típica, actualiza todos los nudos que
salieron de ella".

---

## 3. Idea 2: aplicación por lotes a un pórtico

### 3.1 Flujo propuesto (tres pasos, con parada obligatoria en medio)

```
1. PLANIFICAR   selección del pórtico + plantilla(s)  →  plan: nudos detectados, barras de cada nudo, cordón,
                                                           plantilla casada, especificación instanciada, validación y
                                                           token por nudo, nudos sin encaje
                                                      →  marcas en el modelo (colores por nudo, etiqueta N1, N2…)
2. REVISAR      la persona orbita en Revit y mira las marcas; corrige: excluir nudo, cambiar cordón, cambiar
                plantilla, quitar o añadir una barra, editar un nudo en la ventana 2D  →  se replanifica
3. APLICAR      crea nudo a nudo con los tokens del plan; informe por nudo; quita las marcas
```

Para la IA: `conn_batch_plan` → tabla al usuario (y `get_revit_view` para enseñar las marcas) → correcciones como
`overrides` en una nueva llamada a `conn_batch_plan` → confirmación explícita → `conn_batch_create`. Para la cinta: dos
botones, **Planificar lote** y **Aplicar lote** (y las marcas se quitan al aplicar o con **Descartar plan**).

### 3.2 Detección de nudos (Core, sin Revit)

Entrada: las barras seleccionadas (eje inicio–fin en mm, tipo, categoría). Salida: lista de nudos.

1. **Puntos candidatos**: los extremos de todas las barras. Se agrupan los que están a menos de
   `node_cluster_mm` (propuesta: 10 mm; configurable) entre sí: cada grupo es un nudo candidato.
2. **Barras que llegan**: las que tienen un extremo en el grupo (diagonales, montantes).
3. **Barras que atraviesan**: las que pasan a menos de `node_axis_max_distance_mm` (5 mm, el mismo de hoy) del punto
   sin terminar en él: candidatas a cordón continuo. Si hay más de una (cruce de cordones), el nudo se marca
   "ambiguo" y pide corrección manual.
4. **Cordón**: la barra que atraviesa; si ninguna atraviesa (extremo de cercha, apoyo) se elige la más horizontal que
   llega, como hoy `ChooseChord`, y el nudo queda con `chord_continuous = false` (la plantilla puede exigir
   `continuous: true` y entonces no casa).
5. **Sistema local canónico del nudo** (sección 3.3) y ángulo en el plano de cada barra que llega.
6. **Firma del nudo**: número de barras que llegan por lado (+Y / −Y) y sus ángulos. Un nudo con 1 barra (empalme
   simple) o sin cordón reconocible se lista como "sin tipo" y no se le aplica nada.
7. **Nombre**: `N1, N2, …` ordenados por el cordón y de izquierda a derecha en el plano de la cercha (o por X global si
   hay varias cerchas), para poder decir en el chat "en N4 el cordón es el otro".
8. **Conexiones existentes**: si alguna barra del nudo ya está en una conexión del add-in, el nudo se marca
   `already_connected` y por defecto se salta (sección 7, P8).

Todo esto es geometría de segmentos: se prueba en la nube con cerchas sintéticas (por ejemplo una Pratt de 6 vanos con
2 cordones, 5 montantes y 10 diagonales, más una barra suelta y un cordón en sentido contrario para probar el espejo).

### 3.3 Casar la plantilla con el nudo (Core, sin Revit)

1. Se calcula el sistema local del nudo con la misma regla de hoy (`NodeFrame.Compute` con el cordón y la primera
   barra que llega).
2. Se prueba la plantilla en **4 orientaciones**: tal cual, reflejada en X (cordón al revés), reflejada en Y (cercha
   vista desde el otro lado) y ambas. Para cada orientación se transforman los ángulos del `member_pattern` y el
   contorno de la cartela.
3. En cada orientación se asignan las barras del nudo a las ranuras (`slot`) de la plantilla por **ángulo más cercano**
   dentro de `angle_tolerance_deg` (propuesta: 10°; configurable por plantilla), sin repetir barra ni ranura. La mejor
   orientación es la que casa todas las ranuras con menor suma de diferencias. Si ninguna casa todas, el nudo queda
   "sin encaje" con el detalle (qué ranura no encontró barra, qué barra sobró).
4. Con la asignación hecha, se **instancia** la especificación: IDs reales en `node`, `chord` y `members[i]`, contorno
   de la cartela transformado, `expected_angle_deg` = ángulo real del modelo (así no salen avisos
   `ANGLE_DIFFERS_FROM_MODEL` en cada nudo; el aviso útil aquí es "difiere de la plantilla más de X°", nuevo código
   `TEMPLATE_ANGLE_DEVIATION`), perfil según `profile_policy` (`require` → como hoy, error si no coincide; `warn` →
   aviso y se escribe el perfil del modelo; `ignore`), `source.template_id` y `source.batch_id`.
5. La especificación instanciada pasa por **`conn_validate` normal** (mismas 10 reglas, mismo token). No hay atajos:
   un nudo del lote se crea con exactamente las mismas garantías que uno a mano.

Decisión relacionada: P3 de la Fase 6 (135° frente a 45°). Para que `member_pattern` sea legible conviene fijar una
convención: ángulo **con signo** medido desde +X del sistema canónico, en [−180°, 180°), y lado `+Y`/`−Y` explícito. La
orientación canónica propuesta: X del cordón apuntando hacia +X global (o +Y global si el cordón va en Y), Y hacia
+Z global cuando la cercha es vertical. Es un cambio pequeño en `NodeFrame` que debe quedar cerrado antes de la Fase 7
(sección 7, P5).

### 3.4 Previsualización 3D y corrección manual

Lo que se quiere ver no es la cartela en 3D (eso ya lo enseña el nudo creado y la ventana 2D con cotas) sino **si los
nudos y sus barras se identificaron bien**. Tres opciones, de menos a más compleja:

| Opción | Cómo | Ventajas | Inconvenientes |
|---|---|---|---|
| **A. Marcas en el modelo y ventana modal** (recomendada para empezar) | Planificar colorea las barras de cada nudo (override de gráficos en la vista activa, un color por nudo, cordón con línea gruesa) y coloca una etiqueta `N1…` (texto 3D o `DirectShape` pequeño) en el punto de trabajo; todo dentro de un `TransactionGroup` del plan. La ventana del plan es modal con la lista de nudos, estado y acciones; **Ver en Revit** cierra la ventana, selecciona y hace zoom al nudo; se vuelve a abrir desde el botón. Aplicar o Descartar deshace las marcas. | Sin `ExternalEvent`; reutiliza `OperationScope`; sirve también a la IA (`get_revit_view` enseña las marcas). | No se puede orbitar con la ventana abierta: se cierra, se mira, se reabre. |
| **B. Ventana no modal** | Misma ventana pero modeless, con `ExternalEvent` + `IExternalEventHandler` para cada acción que toque el modelo. | Orbitar y corregir a la vez. | Nueva pieza de infraestructura (cola de eventos, estados), más riesgo; el encargo evitó `ExternalEvent` a propósito. |
| **C. Imagen 3D en la ventana** | `get_revit_view`/`ImageExportOptions` de una vista 3D temporal con las marcas, mostrada dentro de la ventana. | Todo en una ventana. | Imagen fija, sin orbitar; exportar vistas es lento. |

Propuesta: **A en la Fase 8**, y si al usarla se echa de menos orbitar con la ventana abierta, B en una fase posterior
(la lógica no cambia, solo la ventana). Las API concretas para colorear y etiquetar (`View.SetElementOverrides`,
`OverrideGraphicSettings`, `TemporaryGraphicsManager`, `DirectShape` de texto) se comprueban con un sondeo antes de
usarlas, como manda `CLAUDE.md`.

Correcciones manuales que debe admitir el plan (ventana y `overrides` del MCP, las mismas):

- `exclude: [N3, N7]` — no tocar esos nudos.
- `add_node: { N11: [1250010, 1250011, 1250012] }` — un nudo que no se detectó, dado por sus barras (en la cinta,
  seleccionándolas en Revit).
- `chord: { N4: 1249510 }` — el cordón de N4 es esa barra.
- `template: { N9: "<template_id>" }` — otra plantilla para ese nudo (o `null` = sin plantilla).
- `remove_member: { N2: [1249999] }` / `add_member: { N2: [1250001] }` — una barra que no es del nudo o que faltó.
- `merge: [[N5, N6]]` / `split` — dos grupos que son el mismo nudo (tolerancia corta) o al revés.
- **Editar nudo** (solo cinta): abre la ventana de la Fase 6 con la especificación instanciada de ese nudo; lo que se
  cambie ahí sustituye a la instancia de la plantilla solo para ese nudo (`overrides.spec[N4]`).

Cada corrección replanifica (vuelve a casar y validar) y devuelve el plan nuevo con su `plan_id`.

### 3.5 Crear el lote

- `conn_batch_create` recibe el `plan_id` **y** la lista `{node, spec, validation_token}` tal como salió del último
  `conn_batch_plan` (misma regla que hoy: sin token de `validate` no se crea nada). Si alguna barra cambió desde el
  plan, su token deja de valer y ese nudo falla con `VALIDATION_TOKEN_INVALID`.
- Atomicidad (sección 4.1): **un `TransactionGroup` por nudo** (cada nudo o se crea entero o no se crea), dentro de un
  **grupo exterior del lote** que se asimila al final para que en Revit el lote sea **una sola entrada de deshacer**. Con
  `stop_on_error: false` (por defecto) un nudo que falla se anota y se sigue con el siguiente; con `true` se revierte
  el lote entero. Los grupos anidados están previstos por la API de Revit, pero hay que **probar con sondeo** que
  funcionan con la sesión de fabricación de Advance Steel antes de darlo por bueno.
- Cada conexión creada guarda `source.batch_id` y `source.template_id` en su especificación (sin cambiar el esquema de
  Extensible Storage). `conn_list` admite filtrar por `batch_id`; `conn_batch_delete` borra todas las de un lote (con
  confirmación), una a una con las garantías de `conn_delete`.
- Informe: por nudo, `created` / `failed` (con errores) / `skipped` (motivo), tiempos y `connection_id`.
- Registro: eventos `batch_plan`, `batch_create`, `batch_delete` en el log JSON.

### 3.6 Lo que el lote **no** resuelve por sí solo (y cómo se trata)

- **Ángulos muy distintos** entre vanos (vanos de 1 m y de 2 m con la misma altura dan diagonales a 45° y a 63°): la
  misma cartela fija puede no cubrir la barra. Hoy: el nudo no valida (`PLATE_OUTSIDE_GUSSET` o pernos fuera) y se
  corrige a mano en la ventana 2D o con otra plantilla. Más adelante: `outline.mode = "auto"` (sección 5, fase 10).
- **Nudos de tipos distintos** en el mismo pórtico (apoyo, cumbrera, interior): se pasan **varias plantillas** al
  plan y cada nudo toma la que mejor casa; los que no casan con ninguna se listan.
- **Columnas y nudos viga-columna**: fuera de v1 (sección 1.3).
- **Desfases** (ejes que no se cortan por más de 5 mm): hoy es error `NODE_AXES_NOT_INTERSECTING`; en el plan ese nudo
  queda "con desfase" y se excluye o se arregla en el modelo. Si en la práctica hay muchos, se puede plantear admitir
  una excentricidad declarada en el contrato (cambio de contrato, no de esta propuesta).

---

## 4. Arquitectura propuesta (dónde va cada cosa)

```
src/MotorConexiones.Core/
├── Catalog/
│   ├── CatalogTemplate.cs          modelo del archivo de plantilla (sección 2.1)
│   ├── TemplateBuilder.cs          spec + hechos del nudo → plantilla (quita IDs, mide ángulos, escribe el patrón)
│   ├── TemplateMatcher.cs          nudo (barras con ángulo y lado) + plantilla → asignación o "sin encaje" (4 orientaciones)
│   └── TemplateInstantiator.cs     plantilla + asignación → ConnectionSpec con IDs reales y contorno transformado
├── Batch/
│   ├── NodeDetector.cs             segmentos → nudos (agrupar extremos, barras que llegan, barras que atraviesan, cordón)
│   ├── BatchPlan.cs                plan: nudos, estado, plantilla, spec, validación, token, overrides, plan_id
│   └── BatchOverrides.cs           exclusiones, cordón, plantilla, barras, merge/split, spec por nudo
└── Geometry3D/NodeFrame.cs         orientación canónica (decisión P5)

src/MotorConexiones.Revit/
├── Catalog/CatalogStore.cs         lee y escribe %LOCALAPPDATA%\MotorConexiones\catalogo\ (ruta en config\catalog.json)
├── Batch/BatchPlanner.cs           selección → RevitModelFacts → NodeDetector → matcher → validate por nudo → marcas
├── Batch/BatchCreator.cs           grupos anidados, un nudo cada vez con ConnectionCreationService, informe
├── Batch/PlanMarks.cs              colorear barras y etiquetar nudos; quitar marcas
├── Operations/                     CatalogList/Get/Save/Delete/Apply, BatchPlan, BatchCreate, BatchDelete
├── UI/CatalogWindow.xaml           lista, aplicar a la selección, guardar desde conexión, eliminar
├── UI/BatchPlanWindow.xaml         nudos con estado y acciones; Editar nudo abre PreviewWindow
└── (sin cambios) Bridge.cs         solo registra las operaciones nuevas

mcp/tools/conn_tools.py             conn_catalog_* (5), conn_batch_plan, conn_batch_create, conn_batch_delete
mcp/revit_mcp/conexiones.py         una ruta por operación nueva, mismo adaptador
config/catalog.json                 ruta del catálogo, node_cluster_mm, angle_tolerance_deg por defecto
docs/guide.md                       sección "Catálogo" y "Lotes"
src/MotorConexiones.Tests/          CatalogTests (ida y vuelta spec→plantilla→spec), MatcherTests (4 orientaciones,
                                    tolerancias, sin encaje), NodeDetectorTests (cercha sintética), BatchPlanTests
```

### 4.1 Reglas que no se negocian, aplicadas a esto

- Unidades: solo `UnitConverter.cs`. La detección de nudos trabaja en mm con `Vec3` como todo el Core.
- Una operación = un `TransactionGroup`: en el lote, **cada nudo es una operación**; el grupo exterior solo agrupa el
  deshacer. Si el sondeo muestra que los grupos anidados no conviven con Advance Steel, se cae a "un grupo por nudo,
  N entradas de deshacer" y se dice en el informe.
- Sin ventanas en las rutas `conn_*`: las marcas en el modelo no son ventanas; la ventana del plan solo existe en la
  cinta.
- `conn_batch_create` exige los tokens de `conn_batch_plan`, nudo a nudo. `conn_catalog_apply` devuelve el token de
  `conn_validate` para que `conn_create` lo use tal cual.
- `ElementId.Value`; nada de miembros de la API sin compilar o sondear (colores, etiquetas, grupos anidados).

---

## 5. Sugerencias para mejorar el flujo (más allá de las dos ideas)

1. **Guardar en el catálogo desde una conexión ya creada** (`conn_catalog_save` con `connection_id`) y no solo desde
   un JSON. Es el flujo real: leer el detalle una vez, crear, comprobar en Revit, guardar como típica, aplicar al resto.
2. **Trazabilidad plantilla → nudos** con `source.template_id` y `source.batch_id`. Abre la puerta a
   `conn_batch_update`: cambiar la típica (por ejemplo el espesor de la cartela) y rehacer todos los nudos que salieron
   de ella con `conn_update`, uno a uno y validados. Es la mejora de más valor a medio plazo y no cuesta nada preparar
   el terreno ahora.
3. **Cerrar antes dos pendientes de la Fase 6 que afectan a las plantillas**: P3 (ángulo con signo y convención
   canónica) y P4 ("Voltear Y" deja de hacer falta si la orientación canónica pone +Y hacia arriba en cerchas
   verticales). Mejor resolverlos en la Fase 7, antes de que haya plantillas guardadas con la convención antigua.
4. **`outline.mode = "auto"` como fase propia** (fase 10): cartela calculada a partir de las barras (cobertura de la
   ranura o placa + margen por barra, borde en el cordón según `chord_interface`, esquinas recortadas por regla). Es lo
   que convierte una plantilla en verdaderamente paramétrica. No se necesita para que el catálogo y el lote funcionen en
   cerchas con ángulos parecidos, que es el caso descrito.
5. **Nombres de nudo estables (`N1…`) y `conn_get_node_info` para varios nudos**: ayuda a la IA y a la persona a hablar
   del mismo nudo. Puede salir gratis de la detección de nudos.
6. **Un nudo por vez sigue siendo el camino seguro**: `conn_catalog_apply` (un nudo) llega en la Fase 7 y ya quita
   casi todo el trabajo repetitivo; el lote (Fases 8 y 9) se apoya en él. Si solo hubiera tiempo para una fase, sería
   esa.
11. **Ventana del plan no modal** (opción B de 3.4, decisión P10): cuando la Fase 8 esté en uso, si se echa de
    menos girar el modelo con la ventana abierta, se añade la ventana con `ExternalEvent` sin cambiar la lógica del
    plan. Queda anotada aquí a petición de la persona.
7. **Plantillas "oficiales" en el repositorio** (`catalog/*.json`) copiadas por `deploy.ps1` a la carpeta del
   catálogo si no existen. Así un PC nuevo arranca con las típicas de la oficina.
8. **Pequeños añadidos en la ventana de la Fase 6** que encajan con el catálogo: botón "Abrir del catálogo" en lugar de
   buscar el archivo; "Guardar en catálogo" junto a "Guardar JSON".
9. **Evitar el aviso repetido de ángulo en lotes**: el instanciador pone `expected_angle_deg` = ángulo real, y el aviso
   útil pasa a ser "se desvía de la plantilla X°" (`TEMPLATE_ANGLE_DEVIATION`), una vez por nudo, con el valor.
10. **Cerrar el informe de la Fase 6** con los resultados ya devueltos (sección 1.1).

---

## 6. Plan por fases (una por sesión, como siempre)

| Fase | Entrega | Se prueba en la nube | Se prueba en el PC (instalador) |
|---|---|---|---|
| **7. Catálogo** | Orientación canónica (P3/P4), `Core/Catalog/` (builder, matcher con 4 orientaciones, instanciador), `CatalogStore`, 5 operaciones `catalog_*` + `conn_catalog_apply`, botón Catálogo, botones en la ventana, `config/catalog.json`, guía, pruebas xUnit (ida y vuelta spec → plantilla → spec con el Detalle D; espejo; sin encaje) | Compila, pruebas, simulador 27+ | Guardar el Detalle D como típica desde la conexión creada; aplicarla al mismo nudo (misma geometría, token nuevo) y a otro nudo de la cercha (captura); `probar_conexiones.py` con las rutas nuevas |
| **8. Detección de nudos y plan** | `Core/Batch/NodeDetector`, `BatchPlan`, `BatchPlanner`, marcas en el modelo, `conn_batch_plan` con `overrides`, botón Planificar lote y ventana del plan con Editar nudo; sondeos de override de gráficos y etiquetas | Cercha sintética: N nudos, cordón invertido, barra suelta, nudo ambiguo, merge/split | Seleccionar la cercha del Hangar: captura con colores y `N1…`; corregir un cordón; plan con tokens; sin crear nada |
| **9. Crear por lotes** | Sondeo de grupos anidados con Advance Steel; `BatchCreator`, `conn_batch_create`, `conn_batch_delete`, `conn_list` por lote, botón Aplicar lote, informe por nudo | Lógica de informe y de `stop_on_error` con el simulador | Crear el lote en la cercha, una entrada de deshacer, `conn_list` = N, borrar el lote, sondeos 12 y 13 limpios |
| **10. Cartela automática** (opcional) | `outline.mode = "auto"` con regla documentada y pruebas; la ventana 2D lo dibuja | Pruebas de contorno | Nudos con ángulos distintos del Detalle D |

Cada fase deja `docs/fases/fase-N.md`, `docs/instalacion/fase-N.md` y, antes, su `docs/prompts/fase-N.md` escrito a
partir de este documento y de las respuestas de la sección 7.

---

## 7. Preguntas que hay que responder antes de programar

Con la recomendación en cada una. Las respuestas recibidas el 2026-10-01 van en la sección 7.1; lo que sigue
abierto, reescrito en palabras más simples, en la 7.2.

| # | Pregunta | Recomendación |
|---|---|---|
| P1 | ¿"Pórtico" aquí es una cercha de barras HSS (cordones + diagonales, como el Hangar) o también columnas y vigas (nudo viga-columna, placa base)? | Cerchas con `gusset_node`. Los nudos viga-columna son un tipo nuevo, para después. |
| P2 | ¿Dónde vive el catálogo? | Carpeta del usuario `%LOCALAPPDATA%\MotorConexiones\catalogo\`, ruta configurable; copia opcional en `catalog/` del repositorio. |
| P3 | Cuando la plantilla se aplica a un nudo con un perfil distinto, ¿error (no se crea) o aviso (se toma el perfil del modelo)? | Aviso por defecto (`profile_policy: warn`), configurable por plantilla. |
| P4 | ¿La cartela se mantiene como el polígono del plano (fija) en todos los nudos del lote, o quieres que se calcule sola desde el principio? | Fija primero (es lo que dice el plano típico); `auto` en la fase 10 si hace falta. |
| P5 | Orientación canónica del sistema local (ángulo con signo, X hacia +X global, +Y hacia arriba en cerchas verticales). Cambia lo que muestra `conn_get_node_info` y el croquis, no la geometría creada. | Sí, hacerlo en la Fase 7 antes de guardar plantillas. |
| P6 | ¿Debe una plantilla aplicarse en espejo (nudo simétrico al otro lado de la cercha) automáticamente? | Sí (`allow_mirror: true`), informando qué orientación se usó. |
| P7 | Tolerancias por defecto: ±10° para casar barras, 10 mm para agrupar extremos en un nudo, 5 mm para "atraviesa el nudo". | Esos valores, en `config/catalog.json`, editables. |
| P8 | Nudo que ya tiene conexión del add-in: ¿saltar, reemplazar (`conn_update`) o error? | Saltar por defecto, con `replace_existing: true` para actualizarlo. |
| P9 | Atomicidad del lote: ¿cada nudo por separado (un fallo no deshace los demás) o todo o nada? | Por nudo, con una sola entrada de deshacer y `stop_on_error` opcional (si el sondeo de grupos anidados lo permite). |
| P10 | Previsualización 3D: ¿marcas en el modelo con ventana modal (cerrar para orbitar) es suficiente para empezar, o quieres desde el principio la ventana no modal con `ExternalEvent`? | Modal con marcas (opción A) en la Fase 8; no modal después si se echa de menos. |
| P11 | Para corregir a mano, ¿basta elegir de listas (cordón, barras, plantilla) y editar el nudo en la ventana 2D, o quieres pinchar barras en Revit desde la ventana? | Listas y ventana 2D primero; pinchar en Revit en una ronda posterior. |
| P12 | Nombres de nudo `N1…` ordenados a lo largo del cordón: ¿te sirve así para referirte a ellos en el chat? | Sí; y se muestran como etiqueta en el modelo durante el plan. |
| P13 | ¿Quieres preparar ya el terreno de `conn_batch_update` (cambiar la típica y rehacer sus nudos) guardando `template_id` y `batch_id` en cada conexión? No cuesta nada ahora y evita una migración después. | Sí. |

### 7.1 Respuestas recibidas (2026-10-01)

| # | Respuesta | Qué cambia en el diseño |
|---|---|---|
| P1 | Es una cercha (imagen `cercha-referencia.png`): un nudo creado en un vano, repetirlo en los demás. | Nada: `gusset_node` tal como está. Los apoyos en columnas quedan fuera. |
| P2 | Catálogo en el PC **y** copia opcional en el repositorio, como catálogo personal para usar en cualquier equipo. | `catalog/` en el repositorio; `deploy.ps1` copia a la carpeta del usuario lo que falte; "Guardar en catálogo" ofrece también guardar en `catalog/`. |
| P3 | Perfil distinto: aviso, y elegir entre seguir o corregir. | `profile_policy: warn` por defecto. En la ventana del plan el nudo sale "con aviso" y se decide nudo a nudo; para la IA, el aviso va en la respuesta y el usuario decide en el chat. |
| P7 | Tolerancias propuestas, bien. Además: poder seleccionar más elementos, añadir o quitar nudos. | El plan admite **Añadir nudo** (seleccionar las barras de un nudo que no se detectó), **Quitar nudo**, y añadir o quitar barras de un nudo. Ya estaba en 3.4; queda explícito. |
| P8 | Nudo que ya tiene conexión: saltar. | `replace_existing: false` por defecto. |
| P4 | La cartela es fija, "como una base"; lo que cambia de un nudo a otro son los ángulos con que llegan las barras. | Opción (a): el polígono de la plantilla se copia tal cual en cada nudo; las uniones siguen el ángulo real de cada barra; si una barra se sale de la cartela, el validador lo marca y ese nudo se corrige a mano. La cartela automática queda como fase 10 opcional, no necesaria. |
| P6 | Aplicar también a los nudos en espejo; en la previsualización deben verse remarcados cuáles son, y poder borrarlos si se quiere o dejarlos. | `allow_mirror: true`; cada nudo del plan lleva `orientation` (`same`, `mirror_x`, `mirror_y`, `both`); la ventana y la respuesta del MCP los distinguen (color o marca propia y columna en la tabla) y se excluyen con la misma acción que cualquier otro nudo. |
| P9 | Si un nudo falla, los demás se quedan creados y el informe dice cuál falló y por qué. | Opción (a): un `TransactionGroup` por nudo; `stop_on_error: false` por defecto. |
| P10 | Empezar con la ventana que se cierra para mirar el modelo (a); dejar anotada la ventana que permanece abierta (b). | Fase 8 con la opción A de 3.4. La opción B (ventana no modal con `ExternalEvent`) queda anotada como mejora posterior en la sección 5. |
| P11 | Corregir con listas **y** pinchando barras en Revit. | La ventana del plan tiene "Elegir en Revit" para cordón y barras (se oculta la ventana, se pincha, se vuelve). Es más trabajo que las listas: va en la Fase 8 si cabe, si no en una ronda 8b. |

Sin pregunta en el chat, se toman las recomendaciones: P5 (orientación canónica en la Fase 7), P12 (nombres `N1…`),
P13 (`template_id` y `batch_id` en cada conexión).

**Estado: todas las preguntas respondidas (2026-10-01). La propuesta queda cerrada y de aquí sale
`docs/prompts/fase-7.md` cuando la ronda 6d esté cerrada.**

### 7.2 Las cuatro preguntas que hubo que explicar (ya respondidas arriba)

- **P4, forma de la cartela.** No se trata de calcular resistencia: el add-in no calcula nada de eso. Se trata del
  **dibujo** de la cartela. Hoy la cartela es un polígono fijo, copiado del plano (565 × 530 con sus esquinas
  recortadas). Si en otro nudo las diagonales llegan con un ángulo algo distinto, hay dos opciones: (a) poner la
  **misma cartela del plano** tal cual, y si una barra se sale de ella el validador avisa y ese nudo se corrige a mano;
  (b) que el add-in **redibuje la cartela** en cada nudo para que cubra las barras (una cartela distinta por nudo).
  Recomendación: (a) ahora, porque es lo que dice el plano típico; (b) más adelante como fase opcional.
- **P6, espejo.** En la mitad izquierda de la cercha la diagonal de un nudo sube hacia la derecha; en el nudo
  simétrico de la mitad derecha, sube hacia la izquierda. Es el mismo nudo "visto en un espejo". La pregunta es si la
  plantilla debe aplicarse sola a los dos (reflejando la cartela y las uniones) o solo a los que tienen la misma
  orientación que el nudo original. Recomendación: aplicarse sola a los dos, y decir en el plan cuáles salieron en
  espejo.
- **P9, si un nudo falla.** Sí, es una conexión por nudo. La pregunta es qué pasa si, al crear 10 nudos, el número 7
  falla (por ejemplo una barra que no llega bien). Opciones: (a) los otros 9 se quedan creados y el informe dice cuál
  falló y por qué; (b) se deshace todo y no queda ninguno. Recomendación: (a).
- **P10, la ventana durante la revisión.** Cuando una ventana del add-in está abierta, Revit no deja girar ni hacer
  zoom en la vista. Para revisar las marcas de colores hay dos formas: (a) la ventana se cierra, miras el modelo con
  libertad, y la vuelves a abrir con el botón para corregir; (b) una ventana que se queda abierta mientras giras el
  modelo (más compleja de programar, con más riesgo). Recomendación: (a) para empezar.

---

## 8. Riesgos conocidos

- **Grupos de transacción anidados con Advance Steel**: no probado. Plan B descrito en 4.1.
- **Override de gráficos y etiquetas** para las marcas: API conocida pero no usada en este add-in; sondeo primero.
- **Detección de nudos con modelos reales "sucios"** (barras que no llegan por 20 mm, ejes desplazados): las
  tolerancias de P7 y la corrección manual existen para eso; conviene probar el plan sobre la cercha del Hangar entera
  antes de crear nada (Fase 8 no crea).
- **Reflexión de la plantilla**: una cartela asimétrica reflejada sigue siendo válida geométricamente, pero el plano
  puede querer la "misma" cartela sin reflejar; por eso el plan informa la orientación usada y permite forzarla.
- **Crecimiento del contrato**: `source.template_id`/`batch_id` y `TEMPLATE_ANGLE_DEVIATION` son añadidos
  compatibles (v1.1 del esquema); nada de lo existente cambia de significado.
