# Fase 7: catálogo de conexiones (plantillas con nombre que se aplican a un nudo)

Primera de las tres fases que salen de `docs/propuestas/catalogo-y-lotes.md` (sección 6: Fase 7 catálogo, Fase 8
detección de nudos y plan, Fase 9 crear por lotes). Se ejecuta en **una sesión**, con el prompt de la sección 1. Lo demás
de este archivo es el alcance que esa sesión debe cumplir, escrito a partir de las secciones 2, 3.3, 4 y 6 de la
propuesta y de las respuestas de su sección 7.1.

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-6.md y docs/propuestas/catalogo-y-lotes.md completo.
Escribe docs/prompts/fase-7.md (catálogo de conexiones: secciones 2, 3.3, 4 y 6 de la propuesta, con las decisiones de 7.1)
y ejecuta SOLO la Fase 7. Termina con docs/fases/fase-7.md, docs/instalacion/fase-7.md, commit, push y un resumen corto.
```

## 2. Por qué esta fase

La persona quiere guardar las conexiones que ya funcionan para no buscar el archivo JSON cada vez, y poder aplicar una
"típica" a otro nudo de la misma cercha sin volver a leer el plano. Hoy la especificación está atada a un nudo concreto
(`node.element_ids`, `chord.element_id`, `members[].element_id`) y el `validation_token` firma esos IDs: un JSON del
Detalle D no se puede "aplicar" a otro nudo sin cambiar los IDs y volver a validar. El catálogo guarda **plantillas**
(la misma especificación sin IDs, con un patrón de barras) y `conn_catalog_apply` las convierte en una especificación
validada para un nudo concreto. Es la pieza sobre la que se apoyan los lotes de las Fases 8 y 9: si solo hubiera tiempo
para una fase, sería esta (propuesta, sección 5.6).

## 3. Alcance (lo que se entrega)

### 3.1 Orientación canónica del sistema local (decisión P5, antes de guardar ninguna plantilla)

Hoy X va del inicio al fin de la curva del cordón: si en otro nudo el cordón está dibujado al revés, X e Y se invierten y
una cartela guardada aparecería reflejada (propuesta 1.2, hecho 4). Para que las plantillas no dependan del sentido de
dibujo, `NodeFrame.Compute` pasa a ser **canónico**:

- X = eje del cordón orientado hacia **+X global** (si el cordón va en Y, hacia +Y global; si es vertical, hacia +Z).
- Y = en el plano de la cercha, orientado hacia **+Z global** (cercha vertical: +Y local apunta hacia arriba); si la
  cercha es horizontal, hacia +Y global.
- Z = X × Y. El origen (punto de trabajo) y la regla de los 5 mm no cambian.

Consecuencias que hay que dejar escritas y probadas:

- El ángulo de cada barra pasa a ser **con signo**, medido desde +X en [−180°, 180°) (P3 de la Fase 6): es lo que
  devuelven `conn_get_node_info` (`angle_in_plane_deg`, más `angle_to_chord_deg` sin signo y `side` `+Y`/`−Y`) y lo que
  muestra el croquis. El `expected_angle_deg` del contrato sigue siendo el ángulo "del plano" (inclinación respecto al
  cordón, sin signo): la regla 8.6 compara las inclinaciones (45° = 135° = −45°) y solo avisa si difieren más de 1°.
- En el Hangar el cordón está dibujado hacia −X (resultados de las Fases 3 a 6: `frame_x = [-1, 0, 0]`). Con el marco
  canónico el dibujo local se refleja en X respecto a las rondas anteriores. **El informe debe decir qué cambia en la
  geometría creada del Detalle D** y el instalador debe traer una captura para compararla con `fase6-05-nudo.png` y con
  el plano `docs/fixtures/detalle-D.png`.
- "Voltear Y" (P4 de la Fase 6) deja de hacer falta: +Y local apunta hacia arriba en cerchas verticales.

### 3.2 La plantilla (propuesta 2.1): un archivo JSON por plantilla

Lo que **se quita** respecto a una especificación: `node.element_ids`, `chord.element_id`, `members[].element_id` y las
`uncertain_fields` (la plantilla solo se guarda con todas las dudas confirmadas; el valor confirmado queda escrito en su
campo). Lo que **se añade**: `catalog_version`, `template_id` (GUID, nombre del archivo), `name`, `description`, `tags`,
`created_utc`, `origin` (`connection_id`, `document`, `drawing`, `element_ids`), `chord_pattern` (`profile`,
`continuous`, `profile_policy`), `member_pattern` (por barra: `slot`, `role`, `angle_deg` con signo en el marco
canónico, `side`, `profile`, `model_type_name`, `profile_policy`), `matching` (`angle_tolerance_deg`, `allow_mirror`) y
`spec_template` (la especificación sin IDs, con `slot` en cada barra y el contorno de la cartela en mm en el marco
canónico del nudo de origen).

- `profile_policy` por defecto `warn` (P3): al aplicar a un nudo con otro perfil se avisa (`TEMPLATE_PROFILE_DIFFERS`) y
  se escribe el perfil del modelo; `require` conserva el perfil de la plantilla (y `conn_validate` dará
  `PROFILE_MISMATCH` como hoy); `ignore` escribe el del modelo sin avisar.
- `allow_mirror` por defecto `true` (P6); tolerancia de casado 10° (P7). Los valores por defecto viven en
  `config/catalog.json`, editable sin recompilar, junto con la carpeta del catálogo.

### 3.3 De dónde sale una plantilla (propuesta 2.2): tres caminos, el mismo código

1. De una conexión ya creada: `conn_catalog_save` con `connection_id` (lee la especificación guardada en Extensible
   Storage y mide los ángulos reales de las barras). Es el flujo real: leer el detalle una vez, crear, comprobar en
   Revit, "guárdala como típica".
2. De un JSON validado: `conn_catalog_save` con `spec` (hace falta el modelo abierto para medir los ángulos; la
   especificación se valida antes y, si tiene errores, no se guarda: `TEMPLATE_SPEC_INVALID`).
3. Desde la ventana de la Fase 6: botón **Guardar en catálogo** (nombre, descripción y etiquetas en un diálogo pequeño).

### 3.4 Dónde vive el catálogo (P2)

Carpeta del usuario `%LOCALAPPDATA%\MotorConexiones\catalogo\<template_id>.json` (misma carpeta donde ya está el
log), declarada en `config/catalog.json` (`catalog_folder`) para poder apuntar a una carpeta compartida. Además, copia
opcional en el repositorio: carpeta `catalog/` con las plantillas "oficiales"; `scripts/deploy.ps1` copia a la carpeta
del usuario las que falten, y "Guardar en catálogo" ofrece copiar también a esa carpeta (`shared_catalog_folder` en
`config/catalog.json`).

### 3.5 Casar la plantilla con el nudo (propuesta 3.3, en Core, sin Revit)

1. Marco canónico del nudo con la misma regla de hoy (`NodeFrame.Compute` con el cordón y una barra que llega).
2. Se prueba la plantilla en **4 orientaciones**: `same`, `mirror_x` (reflejada en X), `mirror_y` (reflejada en Y) y
   `both`. En cada una se transforman los ángulos del `member_pattern` y el contorno de la cartela.
3. En cada orientación se asignan las barras del nudo a las ranuras (`slot`) por ángulo más cercano dentro de
   `angle_tolerance_deg`, sin repetir barra ni ranura. Gana la orientación que casa todas las ranuras con menor suma de
   diferencias. Si ninguna casa todas: `TEMPLATE_NO_MATCH` con el detalle (qué ranura no encontró barra, qué barra
   sobró, el mejor intento por orientación). Con `allow_mirror: false` solo se prueba `same`; la petición puede forzar
   una orientación (`orientation`).
4. Con la asignación hecha se **instancia** la especificación: IDs reales en `node`, `chord` y `members[i]` (en el orden
   de las ranuras), contorno transformado, `expected_angle_deg` = inclinación real de cada barra en el modelo (así no
   salen avisos `ANGLE_DIFFERS_FROM_MODEL`; el aviso útil es `TEMPLATE_ANGLE_DEVIATION`, una vez por barra, cuando se
   desvía de la plantilla más de `angle_deviation_warning_deg`), perfil según `profile_policy`, y `source.template_id`
   (P13; `source.batch_id` queda reservado en el esquema para la Fase 9).
5. La especificación instanciada pasa por **`conn_validate` normal** (mismas reglas, mismo token). No hay atajos.

### 3.6 Herramientas y botones (propuesta 2.4)

| Camino | Qué | Detalle |
|---|---|---|
| MCP | `conn_catalog_list` | Nombre, id, tipo, etiquetas, número de barras, perfil del cordón, fecha. Sin volcar la plantilla. |
| MCP | `conn_catalog_get` | La plantilla completa. |
| MCP | `conn_catalog_save` | Desde `connection_id` o desde `spec`; `name`, `description`, `tags`, `profile_policy`, `copy_to_shared`. Devuelve `template_id`. Sobrescribir una plantilla con el mismo nombre exige `overwrite: true` (`TEMPLATE_EXISTS`). |
| MCP | `conn_catalog_delete` | Borra el archivo (la IA pide confirmación al usuario, como con `conn_delete`). |
| MCP | `conn_catalog_apply` | `template_id` + `element_ids` (o la selección) de **un** nudo → especificación instanciada + resultado de `conn_validate` + token. La IA pasa `spec` y `validation_token` tal cual a `conn_create`. |
| Cinta | Botón **Catálogo** | Lista con búsqueda; **Aplicar a la selección** abre la ventana de la Fase 6 con la especificación instanciada (archivo virtual: **Guardar JSON** escribe en Documentos); **Guardar desde conexión** (elige una conexión del modelo); **Eliminar**. |
| Ventana Fase 6 | **Guardar en catálogo** y **Abrir del catálogo** | Dos botones más en la barra de abajo. |
| Guía | Sección nueva en `docs/guide.md` | Cuándo guardar, cuándo aplicar, que `conn_catalog_apply` ya valida. |

### 3.7 Arquitectura (propuesta 4, solo la parte de esta fase)

```
src/MotorConexiones.Core/
├── Catalog/
│   ├── CatalogTemplate.cs          modelo del archivo de plantilla
│   ├── CatalogConfig.cs            config/catalog.json (carpetas, tolerancias, política de perfil)
│   ├── TemplateNode.cs             el nudo visto por el catálogo: cordón y barras con ángulo con signo (desde IModelFacts)
│   ├── TemplateBuilder.cs          spec + nudo → plantilla (quita IDs, mide ángulos, escribe el patrón)
│   ├── TemplateMatcher.cs          nudo + plantilla → asignación o "sin encaje" (4 orientaciones)
│   ├── TemplateInstantiator.cs     plantilla + asignación → especificación con IDs reales y contorno transformado
│   └── CatalogStore.cs             lee y escribe la carpeta del catálogo (un JSON por plantilla)
└── Geometry3D/NodeFrame.cs         orientación canónica (P5) y ángulo con signo

src/MotorConexiones.Revit/
├── Catalog/CatalogService.cs       Revit → IModelFacts → Core; validación de la especificación instanciada
├── Catalog/CatalogConfigLoader.cs  config\catalog.json junto al add-in, carpeta con %LOCALAPPDATA% expandido
├── Operations/                     CatalogList, CatalogGet, CatalogSave, CatalogDelete, CatalogApply
├── CatalogCommand.cs               botón Catálogo
├── UI/CatalogWindow.xaml           lista, aplicar a la selección, guardar desde conexión, eliminar
├── UI/SaveTemplateDialog.xaml      nombre, descripción, etiquetas, copia a la carpeta compartida
└── (sin cambios de contrato) Bridge.cs solo registra las operaciones nuevas

mcp/tools/conn_tools.py             conn_catalog_* (5)       mcp/revit_mcp/conexiones.py   una ruta por operación nueva
config/catalog.json                 carpetas y tolerancias   catalog/                       plantillas oficiales (copia opcional)
src/MotorConexiones.Tests/          CatalogTests (ida y vuelta spec → plantilla → spec con el Detalle D; 4 orientaciones;
                                    sin encaje; política de perfil; almacén en una carpeta temporal)
```

## 4. Reglas técnicas de esta fase

- Unidades: solo `Units/UnitConverter.cs`. El catálogo trabaja en mm y grados.
- Sin ventanas en las rutas `conn_*`: las cinco operaciones nuevas no abren nada; la ventana del catálogo solo existe en
  la cinta.
- `conn_catalog_apply` devuelve el token de `conn_validate` para que `conn_create` lo use tal cual; `conn_create` y
  `conn_update` no cambian.
- `ElementId.Value`; nada de miembros de la API de Revit sin compilar. Nada de esta fase necesita API nueva de Revit.
- Código y claves JSON en inglés; mensajes, textos de las ventanas y documentación en español.
- El Core se prueba en la nube con el fixture del Detalle D y `FakeModelFacts`; lo que pase en Revit se prueba con el
  instalador.

## 5. Instrucciones para el instalador (`docs/instalacion/fase-7.md`)

Una ronda sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, con `Anota` y resultados en `docs/fases/resultados-fase-7.md`:

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado (copia también `config\catalog.json`).
2. Abrir Revit: `conn_ping` debe listar las cinco operaciones `catalog_*`; `node_info` del nudo del Detalle D con el
   marco canónico (`x_axis` hacia +X global) y los ángulos con signo.
3. Crear el Detalle D desde la ventana (captura del nudo para compararla con `fase6-05-nudo.png` y con el plano).
4. `catalog_save` desde esa conexión (`conn-call.ps1`); `catalog_list`; `catalog_apply` al mismo nudo: `is_valid`,
   orientación `same`, desviaciones 0 y un token nuevo. Borrar la conexión.
5. Botón **Catálogo** > **Aplicar a la selección** sobre el mismo nudo: la ventana se abre con la especificación
   instanciada; **Crear**; captura; borrar desde la cinta.
6. Aplicar la misma plantilla a **otro nudo de la cercha** (la persona lo elige y lo selecciona; el nudo simétrico de la
   otra mitad debe salir en `mirror_x`): captura y, si valida, crear y borrar.
7. Copiar la plantilla guardada a `catalog/` del repositorio y subirla (P2).
8. `probar_conexiones.py --puente` con las pruebas nuevas del catálogo.

## 6. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas del catálogo y las actualizadas del marco
  canónico; `simulador_revit.py --autocomprobar` con las rutas nuevas; `py_compile` de todo `mcp/`.
- `docs/fases/fase-7.md` según el Anexo B del encargo, con la lista de lo NO PROBADO y el efecto del marco canónico en
  la geometría del Detalle D.
- `docs/instalacion/fase-7.md` literal, con `Anota`, capturas con nombre fijo y qué devolver.
- README: estado, árbol, cuentas y una sección nueva "Catálogo de conexiones"; `mcp/CONTRATO-conn.md` con las rutas
  nuevas; `docs/guide.md` con la sección "Catálogo".
- Commit en español en la rama de trabajo indicada; sin pull requests.

## 7. Fuera de alcance de esta fase

- Detección de nudos, plan, marcas en el modelo y `conn_batch_*` (Fases 8 y 9). `source.batch_id` solo queda reservado.
- `outline.mode = "auto"` (Fase 10 opcional): la cartela de la plantilla se copia tal cual (P4).
- `conn_batch_update` (cambiar la típica y rehacer sus nudos): solo se prepara el terreno con `source.template_id`.
- Exportar e importar plantillas entre PCs más allá de copiar el archivo JSON.
