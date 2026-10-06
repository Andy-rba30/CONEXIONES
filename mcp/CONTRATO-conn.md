## Rutas `/conn/` (MotorConexiones)

> Sección para pegar al final de `CONTRATO.md` del repositorio revit-mcp cuando las herramientas `conn_*` se suban
> allí. Mientras tanto vive en `CONEXIONES/mcp/CONTRATO-conn.md`. Versión: cierre de la ronda 8d (adaptador 0.8.5, add-in 0.8.5: solo cambia la versión, sin cambios de rutas ni de claves; en la cinta, la ventana del plan ya no puede cerrar Revit, pincha con la ventana oculta y se cierra al descartar). Cierre de la ronda 8c (0.8.4): sin cambios de rutas ni de claves; la ventana del plan de la cinta pasa a ser no modal y su Descartar quita todos los marcadores del documento, como `discard` con `all`; las
rutas `/conn/catalog/...` y el marco canónico del nudo son de la Fase 7; las rutas `/conn/batch/plan/...` del plan de lote, de la
Fase 8; la ronda 8b añade `members[].end_gap_mm` y el aviso `NODE_CHORD_NOT_CONTINUOUS` en los nudos del plan; el cierre
de la Fase 8 (0.8.2) añade el aviso `PLAN_MARKS_REPLACED`, cuenta bien `removed_markers` en `discard` con `all`, quita la clave
auxiliar `IsEmpty` de `overrides` y hace que el add-in emita `end_gap_mm`, que en 0.8.1 faltaba en la respuesta; la ronda 8c (0.8.3)
solo **añade** claves: `summary_text`, `visible_count` y `hidden_text` en el plan, `status_text`, `advice` y `visible_by_default` por
nudo, y el aviso `CATALOG_EMPTY`; los **valores** de `color_name` / `color_rgb` pasan a ser los del estado: `verde`, `ambar`, `rojo`, `gris`).

Estas rutas las añade el módulo `revit_mcp/conexiones.py` (IronPython 2.7, dentro de Revit) y las usan las
herramientas `conn_*` de `tools/conn_tools.py` (CPython, puente `main.py`). Los dos archivos se escriben en el
repositorio `Andy-rba30/CONEXIONES` (carpeta `mcp/`) y se copian a la extensión con `mcp\instalar-conn.ps1`, que
también añade, si faltan, dos líneas en `startup.py` (`from revit_mcp.conexiones import register_conn_routes` /
`register_conn_routes(api)`) y dos en `tools/__init__.py` (`from .conn_tools import register_conn_tools` /
`register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)`). Nada más se toca.

### Qué hay detrás

`conexiones.py` es un adaptador sin lógica de negocio: localiza el ensamblado `MotorConexiones.Revit` (add-in de
Revit 2027 en C#, cargado por Revit desde `%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\`) y llama por
reflexión a `MotorConexiones.Revit.Bridge.Handle(operation, requestJson, doc, uidoc)`, que devuelve el JSON de la
respuesta. Todos los manejadores declaran `doc` y `uidoc`, así que corren en contexto de la API de Revit
(ExternalEvent de pyRevit) y el add-in abre sus propias transacciones. Las peticiones se atienden de una en una.

### Seguridad

La misma que el resto del conector: cada ruta lleva `@requiere_token` (POST: clave `token` en el cuerpo JSON; GET:
`?token=`), y responde `401 {"error": "token ausente o incorrecto"}` si falta o no coincide. No hay otro mecanismo.

### Códigos HTTP y sobre común

| HTTP | Cuándo | Cuerpo |
|---|---|---|
| `200` | **Siempre que la petición llega al adaptador**, también con errores de validación, add-in no cargado o sin documento | Sobre común (abajo) |
| `401` | Token ausente o incorrecto | `{"error": "token ausente o incorrecto"}` |
| `404` | La ruta no existe: `conexiones.py` no está instalado o pyRevit no se recargó | (pyRevit) |
| `500` | Excepción no controlada **del adaptador** (no del add-in: el add-in nunca lanza, convierte todo en el sobre) | `{"error": "..."}` |

El puente `main.py` solo devuelve el JSON como `dict` cuando el HTTP es 200 (con otro código devuelve el texto
`Error: <código> - <cuerpo>`); por eso las rutas `conn_*` responden 200 con `ok:false` en todos los fallos de
negocio, incluido "sin documento" (`NO_DOCUMENT`) en vez del 503 que usan otras rutas. Así la IA recibe siempre la
estructura completa con código, ruta del campo, mensaje en español y sugerencia.

```json
{
  "ok": false,
  "data": null,
  "errors": [
    {
      "code": "BOLT_EDGE_DISTANCE_TOO_SMALL",
      "path": "members[2].attachment.bolts.edge_mm",
      "message": "La distancia al borde (15 mm) es menor que el mínimo configurado (22 mm) para pernos de 5/8\".",
      "hint": "Revisa la cota en el plano o usa edge_mm >= 22."
    }
  ],
  "warnings": [],
  "meta": { "operation": "validate", "duration_ms": 41, "addin_version": "0.8.0" }
}
```

`warnings` tiene la misma forma que `errors` y nunca bloquea. Los acentos viajan como `\uXXXX` (pyRevit serializa
con `ensure_ascii=True`) y llegan intactos; el puente los devuelve sin escapar.

### Rutas

Todas cuelgan de `http://127.0.0.1:48884/revit_mcp`. Los POST reciben JSON (`Content-Type: application/json`) con
la clave `token` además de los campos indicados; los GET llevan `?token=`.

| Método | Ruta | Cuerpo / parámetros | `data` de la respuesta | Modelo |
|---|---|---|---|---|
| GET | `/conn/ping/` | — | `addin_version`, `spec_version`, `backend` (`advancesteel` / `directshape`), `operations`, `revit` {`version_number`, `version_build`, `version_name`, `sub_version_number`, `language`}, `dotnet`, `document` (o `null`), `has_uidocument` | No necesita |
| GET | `/conn/guide/` | — | `guide_markdown` (contenido de `docs/guide.md` del add-in) | No necesita |
| GET | `/conn/types/` | — | `connection_types`: `[{type_name, description}]` | No necesita |
| GET | `/conn/schema/<type>` | `type` en la ruta (`gusset_node`) | `connection_type`, `description`, `json_schema` (Draft-07), `example` | No necesita |
| POST | `/conn/node_info/` | `element_ids` (lista; opcional: si falta, la selección actual), `chord_element_id` (opcional) | `origin_mm`, `x_axis`, `y_axis`, `z_axis`, `axis_distance_mm`, `chord_element_id`, `members[]` {`element_id`, `family`, `type`, `structural_type`, `start_mm`, `end_mm`, `length_mm`, `slope_deg`, `angle_in_plane_deg`, `node_end`, `material`, `is_chord`}, `existing_connections[]` | Lee |
| POST | `/conn/find_profile/` | `query` | `query`, `total_profiles_in_model`, `matched_count`, `matches[]` {`type_name`, `family_name`, `exact_match`}, `suggestions[]` | Lee |
| POST | `/conn/validate/` | `spec` (objeto de la especificación) | `is_valid`, `validation_token` (64 hex), `errors_count`, `warnings_count`, `calculated_values` {`origin_mm`, `axis_distance_mm`, `frame_x`, `frame_y`, `frame_z`}, `bolt_stacks[]` {`member_element_id`, `gusset_face`, `grip_mm`, `bolt_length_mm`, `length_source`} (ronda 6b) | Lee |
| POST | `/conn/preview/` | `spec` | `summary` {`connection_type`, `backend`, `chord_element_id`, `first_member_element_id`, `working_point_mm`, `gusset_plates`, `knife_plates`, `bolts`, `weld_lines`, `members_modified`, `dry_run`}, `elements_to_create[]` (la placa cuchilla trae `gusset_face` y `offset_from_gusset_plane_mm`; el grupo de pernos, `grip_mm`, `length_mm` y `length_source`), `members_to_modify[]` | Lee |
| POST | `/conn/create/` | `spec`, `validation_token` | `connection_id`, `spec_version`, `connection_type`, `created_element_ids[]`, `created_elements_count`, `created_utc`, `backend` | **Escribe** |
| GET | `/conn/list/` | — | `connections_count`, `connections[]` {`connection_id`, `spec_version`, `connection_type`, `created_elements_count`, `backend`, `created_utc`} | Lee |
| GET | `/conn/get/<connection_id>` | `connection_id` en la ruta | `connection_id`, `spec_version`, `connection_type`, `created_utc`, `created_element_ids[]`, `created_elements_count`, `backend`, `spec` | Lee |
| POST | `/conn/update/` | `connection_id`, `spec`, `validation_token` | `connection_id`, `spec_version`, `connection_type`, `created_element_ids[]`, `created_elements_count`, `updated_utc` | **Escribe** |
| POST | `/conn/delete/` | `connection_id` | `deleted_connection_id`, `deleted_elements_count`, `restored_members_count` | **Escribe** |
| GET | `/conn/catalog/list/` | — | `catalog_folder`, `shared_catalog_folder`, `templates_count`, `templates[]` {`template_id`, `name`, `description`, `connection_type`, `tags`, `members_count`, `chord_profile`, `pattern`, `created_utc`, `origin_drawing`, `origin_document`, `file`} | No necesita (archivos del PC) |
| GET | `/conn/catalog/get/<template_id>` | `template_id` en la ruta | `template_id`, `name`, `file`, `pattern`, `template` (el archivo completo: `catalog_version`, `member_pattern[]` {`slot`, `role`, `angle_deg` con signo, `side`, `profile`, `model_type_name`, `profile_policy`}, `chord_pattern`, `matching` {`angle_tolerance_deg`, `allow_mirror`}, `spec_template` sin IDs y con `slot` por barra) | No necesita |
| POST | `/conn/catalog/save/` | `connection_id` **o** `spec`; `name`; opcionales `description`, `tags[]`, `overwrite`, `template_id`, `profile_policy` (`warn`/`require`/`ignore`), `angle_tolerance_deg`, `allow_mirror`, `copy_to_shared` | `template_id`, `name`, `file`, `shared_file`, `members_count`, `chord_profile`, `member_pattern[]`, `matching`, `origin` | Lee (mide los ángulos reales) |
| POST | `/conn/catalog/delete/` | `template_id` | `deleted_template_id`, `name`, `file` | No necesita |
| POST | `/conn/catalog/apply/` | `template_id` (o `template_name`), `element_ids` (opcional: la selección), `chord_element_id` (opcional), `orientation` (`auto`, `same`, `mirror_x`, `mirror_y`, `both`) | `template_id`, `name`, `node` {`chord_element_id`, `element_ids`, `chord_direction_reversed`}, `match` {`orientation`, `is_complete`, `matched_count`, `score_deg`, `max_deviation_deg`, `assignments[]` {`slot`, `role`, `element_id`, `template_angle_deg`, `model_angle_deg`, `deviation_deg`, `side`, `model_type_name`, `template_profile`, `profile_policy`}, `unmatched_slots`, `unassigned_members`, `description`}, `spec` (instanciada, con `source.template_id`), `is_valid`, `validation_token`, `errors_count`, `warnings_count`, `calculated_values`, `bolt_stacks[]`. `ok` = la especificación valida; con `TEMPLATE_NO_MATCH`, `data.attempts[]` trae el intento de cada orientación | Lee |
| POST | `/conn/batch/plan/` | `element_ids` (opcional: la selección), `template_ids[]` (opcional: todas las del catálogo), `overrides` (objeto: `exclude[]`, `include[]`, `add_node{}`, `chord{}`, `template{}`, `remove_member{}`, `add_member{}`, `merge[][]`, `split{}`, `spec{}`, `replace_existing`), `plan_id` (replanificar), `mark` (por defecto `true`), `reset_overrides`, `include_specs` (por defecto `true`) | `plan_id`, `document`, `created_utc`, `updated_utc`, `selection_count`, `templates` {id: nombre}, `summary` {estado: cuenta}, `description`, `ready_count`, `summary_text` (ronda 8c: la decisión en español, la misma cabecera que la ventana: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas."; con 0 listos, "Ningún nudo listo: …" y la causa más frecuente), `visible_count`, `hidden_text` ("20 sin cordón, 23 barras sueltas"), `is_marked`, `marked_view_id`, `marks` {`element_count`, `marker_element_ids`}, `overrides`, `unused_element_ids`, `nodes[]` {`name`, `status` (`ready`, `invalid`, `no_match`, `ambiguous_chord`, `offset`, `untyped`, `already_connected`, `excluded`), `status_detail`, `status_text` (ronda 8c: el estado en español con icono: `● Listo`, `▲ Listo con aviso`, `✖ No valida`, `✖ Sin plantilla que encaje`, `✖ Sin plantillas en el catálogo`, `✖ Falta el cordón`, `✖ Dos cordones posibles`, `✖ Los ejes no se cortan`, `◌ Ya tiene conexión`, `◌ Excluido`, `○ Barra suelta (no es nudo)`, con sufijos ` (editado)` y ` (rehacer)`), `advice` (qué hacer, una frase: "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…"), `visible_by_default` (`false` en las barras sueltas y en las parejas de dos barras sin cordón: no son nudos, la ventana las oculta por defecto y no se marcan), `work_point_mm`, `chord_element_id`, `chord_continuous`, `chord_type_name`, `through_element_ids`, `member_element_ids`, `element_ids`, `members[]` {`element_id`, `angle_deg` con signo, `side`, `type_name`, `reaches_node`, `end_gap_mm` (ronda 8b: distancia del extremo real de la barra al punto de trabajo, que es la media de los cortes de las barras del nudo con el cordón; 0 si llega al eje, 15–90 mm si termina en la cara del cordón; el add-in lo emite desde 0.8.2)}, `signature`, `is_manual`, `template_id`, `template_name`, `orientation`, `is_mirrored`, `max_deviation_deg`, `match`, `attempts[]`, `spec`, `has_spec_override`, `is_valid`, `validation_token`, `errors[]`, `warnings[]`, `existing_connection_id`, `replaces_existing`, `color_name` y `color_rgb` (desde 0.8.3 el color del **estado**, el mismo en la ventana, en el mapa y en el modelo: `verde` [46,160,67] = se creará, `ambar` [240,160,0] = se creará con aviso, `rojo` [214,45,45] = falta algo, `gris` [140,140,140] = no se crea; `null` en los nudos ocultos), `is_marked`, `marker_element_id`}. Aviso `PLAN_MARKS_SKIPPED` si la vista activa no admite colores; aviso `CATALOG_EMPTY` si el catálogo no tiene ninguna plantilla (todos los nudos salen `no_match` con `status_text` "✖ Sin plantillas en el catálogo") | **Escribe solo marcas** (overrides de la vista y marcadores DirectShape; nunca acero) |
| POST | `/conn/batch/plan/get/` | `plan_id` (opcional: el último), `node` (opcional: un nudo con su `spec`), `include_specs` | Lo mismo que `batch_plan` o `{plan_id, node}`; `PLAN_NOT_FOUND` si no hay plan en memoria | No necesita |
| POST | `/conn/batch/plan/discard/` | `plan_id` (opcional: el último), `all` (quita todas las marcas de MotorConexiones) | `discarded_plan_id`, `removed_marks`, `remaining_plans`, `remaining_markers` (o `discarded_plans`, `removed_markers` = marcadores que había en el documento, de planes en memoria y huérfanos (0.8.2; en 0.8.1 contaba solo los huérfanos), `remaining_markers` con `all`) | **Escribe solo marcas** |

Rutas de desarrollo (sin herramienta MCP; las usan `scripts\conn-call.ps1` y `scripts\revit-exec.ps1 -SinTransaccion`
del repositorio CONEXIONES):

| Método | Ruta | Cuerpo | Respuesta |
|---|---|---|---|
| POST | `/conn/op/<operation>/` | cualquier objeto JSON | Sobre común de `Bridge.Handle(<operation>, <cuerpo>)`; `UNKNOWN_OPERATION` si no existe |
| POST | `/conn/dev_exec/` | `code` (IronPython 2.7), `description` | Sobre común con `data.output` (y `data.traceback` + `PROBE_EXCEPTION` si falla). Ejecuta **sin** TransactionGroup ni Transaction envolventes, con `doc`, `uidoc`, `uiapp`, `DB`, `UI`, `revit`, `clr`, `System` y `print`. Mismo token y mismo poder que `/execute_code/`. |

### Sistema local del nudo y ángulos (Fase 7)

Desde la Fase 7 el marco del nudo es **canónico**: X = eje del cordón hacia +X global (o +Y, o +Z), Y en el plano de la
cercha hacia +Z global (hacia arriba en cerchas verticales), Z = X × Y. `node_info` devuelve además
`chord_direction_reversed` (la curva del cordón va en sentido contrario a +X local) y, por barra, `angle_in_plane_deg`
**con signo** en [−180°, 180°) medido desde +X, `angle_to_chord_deg` (inclinación sin signo, lo que escribe un plano) y
`side` (`+Y`/`-Y`). El contrato no cambia: `expected_angle_deg` sigue siendo la inclinación del plano y la regla 8.6 la
compara sin signo (45° = 135° = −45°). `source.template_id` y `source.batch_id` son campos opcionales nuevos del esquema.

### Códigos de error

Del adaptador (`conexiones.py`): `ADDIN_NOT_LOADED` (el add-in no está en el proceso ni en la carpeta de add-ins),
`BRIDGE_CALL_FAILED` (la reflexión lanzó), `BRIDGE_BAD_RESPONSE` (el add-in no devolvió JSON), `INVALID_REQUEST`.

Del puente (`conn_tools.py`, cuando Revit no contesta 200): `REVIT_UNREACHABLE`, `REVIT_RESTARTED` (token cambiado),
`CONN_ROUTE_NOT_FOUND` (404), `REVIT_UNAUTHORIZED` (401), `CONN_ROUTE_EXCEPTION` (5xx), `REVIT_TIMEOUT`, `REVIT_HTTP_ERROR`.

Del add-in (`Bridge` y validación, sección 8 del encargo): `UNKNOWN_OPERATION`, `INVALID_REQUEST`, `NO_DOCUMENT`,
`REVIT_BUSY`, `INTERNAL_ERROR`, `SCHEMA_INVALID`, `UNRESOLVED_UNCERTAINTY`, `DIMENSION_CHAIN_MISMATCH`,
`LABEL_VALUE_MISMATCH`, `PROFILE_MISMATCH`, `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL`,
`BOLT_OUTSIDE_PLATE`, `OUTLINE_INVALID`, `PLATE_OUTSIDE_GUSSET`, `CLASH_WITH_FOREIGN_MEMBER`, `ELEMENT_NOT_FOUND`,
`ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE`, `NODE_AXES_NOT_INTERSECTING`, `NODE_AXES_PARALLEL`,
`VALIDATION_TOKEN_INVALID`, `FABRICATION_FAILED`. Advertencias: `ANGLE_DIFFERS_FROM_MODEL`, `WELD_BELOW_MINIMUM`,
`BOLT_LENGTH_TOO_SHORT` (ronda 6b: `bolts.length_mm` menor que agarre + suplemento), `REVIT_WARNING` (aviso de Revit
suprimido), `REVIT_DIALOG_SUPPRESSED`, `CATEGORY_FALLBACK`.

Del catálogo (Fase 7): `TEMPLATE_NOT_FOUND`, `TEMPLATE_EXISTS` (mismo nombre sin `overwrite: true`), `TEMPLATE_INVALID`
(archivo ilegible), `TEMPLATE_SPEC_INVALID` (la especificación a guardar no valida; debajo van los errores de `validate`),
`TEMPLATE_HAS_OPEN_UNCERTAINTIES`, `TEMPLATE_NO_MATCH` (ninguna orientación casa todas las ranuras; `data.attempts`),
`CATALOG_FOLDER_UNAVAILABLE`. Advertencias: `TEMPLATE_ANGLE_DEVIATION` (una barra se desvía de la plantilla más de
`angle_deviation_warning_deg`), `TEMPLATE_PROFILE_DIFFERS` (perfil distinto con `profile_policy: warn`; se escribe el del modelo).

Del plan de lote (Fase 8): `PLAN_NOT_FOUND` (no hay plan con ese `plan_id` en memoria: se descartó o Revit se reinició).
Advertencias: `CATALOG_EMPTY` (ronda 8c: el catálogo no tiene ninguna plantilla, así que ningún nudo puede casar; guarda primero
una con `conn_catalog_save` o el botón Catálogo > Guardar en catálogo), `PLAN_MARKS_SKIPPED` (la vista activa no admite colores por elemento; el plan se calculó sin marcas),
`PLAN_MARKS_REPLACED` (0.8.2: al marcar este plan se quitaron las marcas de otro plan del mismo documento, que sigue en
memoria sin marcas; en un documento solo hay un plan marcado) y, por
nudo (ronda 8b), `NODE_CHORD_NOT_CONTINUOUS` (ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que
llegan; extremo de cercha o cordón que falta en la selección). Los
estados de cada nudo (`ready`, `invalid`, `no_match`, `ambiguous_chord`, `offset`, `untyped`, `already_connected`,
`excluded`) no son errores: van en `nodes[].status` con `status_detail`.

Campos opcionales del contrato añadidos en la ronda 6b (el esquema de `/conn/schema/gusset_node` los describe):
`members[].attachment.plate.gusset_face` (`"+z"` | `"-z"`, cara de la cartela sobre la que apoya la placa cuchilla; por
defecto `+z`) y `members[].attachment.bolts.length_mm` (longitud del perno si el plano la indica; si falta se calcula del
agarre con `config/limits.json`).

### Herramientas MCP

`tools/conn_tools.py` registra 21 herramientas (13 de la Fase 4, 5 del catálogo de la Fase 7 y 3 del plan de lote de la Fase 8), una por ruta con nombre, que
devuelven el sobre como texto JSON íntegro (`json.dumps(..., ensure_ascii=False, indent=2)`), nunca `format_response`:

| Herramienta | Ruta | Argumentos | Tiempo de espera |
|---|---|---|---|
| `conn_ping` | GET `/conn/ping/` | — | 15 s |
| `conn_get_guide` | GET `/conn/guide/` | — | 60 s |
| `conn_list_types` | GET `/conn/types/` | — | 60 s |
| `conn_get_schema` | GET `/conn/schema/<type>` | `connection_type` (= `gusset_node`) | 60 s |
| `conn_get_node_info` | POST `/conn/node_info/` | `element_ids?`, `chord_element_id?` | 60 s |
| `conn_find_profile` | POST `/conn/find_profile/` | `query` | 60 s |
| `conn_validate` | POST `/conn/validate/` | `spec` (objeto o texto JSON) | 60 s |
| `conn_preview` | POST `/conn/preview/` | `spec` | 180 s |
| `conn_create` | POST `/conn/create/` | `spec`, `validation_token` | 180 s |
| `conn_list` | GET `/conn/list/` | — | 60 s |
| `conn_get` | GET `/conn/get/<id>` | `connection_id` | 60 s |
| `conn_update` | POST `/conn/update/` | `connection_id`, `spec`, `validation_token` | 180 s |
| `conn_delete` | POST `/conn/delete/` | `connection_id` | 180 s |
| `conn_catalog_list` | GET `/conn/catalog/list/` | — | 60 s |
| `conn_catalog_get` | GET `/conn/catalog/get/<id>` | `template_id` | 60 s |
| `conn_catalog_save` | POST `/conn/catalog/save/` | `name`, `connection_id?`, `spec?`, `description?`, `tags?`, `overwrite?`, `profile_policy?`, `copy_to_shared?` | 60 s |
| `conn_catalog_delete` | POST `/conn/catalog/delete/` | `template_id` | 60 s |
| `conn_catalog_apply` | POST `/conn/catalog/apply/` | `template_id`, `element_ids?`, `chord_element_id?`, `orientation?` | 60 s |
| `conn_batch_plan` | POST `/conn/batch/plan/` | `element_ids?`, `template_ids?`, `overrides?`, `plan_id?`, `mark` (true), `replace_existing` (false), `include_specs` (false) | 180 s |
| `conn_batch_plan_get` | POST `/conn/batch/plan/get/` | `plan_id?`, `node?`, `include_specs` (false) | 60 s |
| `conn_batch_plan_discard` | POST `/conn/batch/plan/discard/` | `plan_id?`, `all` (false) | 60 s |

Las herramientas comprueban en el puente que los argumentos obligatorios no estén vacíos y que `spec` sea un objeto
(o un texto JSON que lo contenga); si no, devuelven `INVALID_REQUEST` sin llamar a Revit. El manual de cada una es
su docstring.

### Deshacer

Cada operación de escritura (`create`, `update`, `delete`, y `batch_plan` / `batch_plan_discard` para las marcas) es un `TransactionGroup` llamado
`MotorConexiones: <operación> <connection_id>` que se asimila al terminar: en Revit aparece como **una sola entrada
de deshacer**. Ante cualquier error el grupo se revierte entero (el modelo queda como estaba) y la respuesta lleva
`ok:false`. Los avisos de Revit se suprimen (`IFailuresPreprocessor`) y vuelven como `REVIT_WARNING`; cualquier
diálogo que intente abrirse se cancela y se anota como `REVIT_DIALOG_SUPPRESSED`. Ninguna ruta abre ventanas.

### Pruebas

`mcp\pruebas\probar_conexiones.py` (CPython, solo `httpx` y stdlib), con Revit abierto sobre
`HANGAR_PRUEBA_sondeo.rvt`, Routes activo, `conexiones.py` instalado y el add-in desplegado:

```bat
C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\.venv\Scripts\python.exe mcp\pruebas\probar_conexiones.py [--puente]
```

No crea nada en el modelo. Veintiséis pruebas contra 48884 (401 sin token; `ping`; guía; tipos; esquema; `find_profile`;
`node_info`; `validate` del Detalle D confirmado con token; `validate` con 420→402 → `DIMENSION_CHAIN_MISMATCH`;
`validate` con dudas sin confirmar → `UNRESOLVED_UNCERTAINTY`; `preview`; `create` sin token → `VALIDATION_TOKEN_INVALID`;
`list`; `get` y `delete` de un ID inexistente → `ELEMENT_NOT_FOUND`; `op/no_existe` → `UNKNOWN_OPERATION`; y las del
catálogo, Fase 7: `catalog/list`, `catalog/save` desde el fixture (escribe la plantilla "PRUEBA probar_conexiones" en la
carpeta del catálogo del PC), `catalog/get`, `catalog/apply` al mismo nudo con token y orientación `same`,
`catalog/apply` con una plantilla inexistente → `TEMPLATE_NOT_FOUND`, `catalog/delete`; y las del plan de lote, Fase 8,
entre el `apply` y el `delete`: `batch/plan` sobre el nudo del fixture con esa plantilla y `mark: false` (sin tocar la
vista; el nudo `ready` que contiene el cordón, con token y `source.batch_id`: N1 en el simulador y N4 en Revit, porque los
nombres van por la X global), `batch/plan/get` de ese nudo, `batch/plan/discard`; desde la ronda 8c la 22 y la 23 comprueban también `status_text`, `advice`,
`visible_by_default`, `summary_text` y que `color_name` sea el del estado) y, con `--puente`,
dos contra el puente en 8000 (`tools/list` con las 21 `conn_*` y `tools/call conn_ping`). Termina con `Resultado: N/N pruebas correctas`
y código de salida 0 si todas pasan.
