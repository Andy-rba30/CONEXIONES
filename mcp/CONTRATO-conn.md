## Rutas `/conn/` (MotorConexiones)

> Sección para pegar al final de `CONTRATO.md` del repositorio revit-mcp cuando las herramientas `conn_*` se suban
> allí. Mientras tanto vive en `CONEXIONES/mcp/CONTRATO-conn.md`. Versión: Fase 4 (adaptador 0.4.0, add-in 0.1.0); desde la Fase 6 el add-in responde `addin_version: 0.2.0` sin cambios en las rutas.

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
  "meta": { "operation": "validate", "duration_ms": 41, "addin_version": "0.2.0" }
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
| POST | `/conn/validate/` | `spec` (objeto de la especificación) | `is_valid`, `validation_token` (64 hex), `errors_count`, `warnings_count`, `calculated_values` {`origin_mm`, `axis_distance_mm`, `frame_x`, `frame_y`, `frame_z`} | Lee |
| POST | `/conn/preview/` | `spec` | `summary` {`connection_type`, `backend`, `chord_element_id`, `first_member_element_id`, `working_point_mm`, `gusset_plates`, `knife_plates`, `bolts`, `weld_lines`, `members_modified`, `dry_run`}, `elements_to_create[]`, `members_to_modify[]` | Lee |
| POST | `/conn/create/` | `spec`, `validation_token` | `connection_id`, `spec_version`, `connection_type`, `created_element_ids[]`, `created_elements_count`, `created_utc`, `backend` | **Escribe** |
| GET | `/conn/list/` | — | `connections_count`, `connections[]` {`connection_id`, `spec_version`, `connection_type`, `created_elements_count`, `backend`, `created_utc`} | Lee |
| GET | `/conn/get/<connection_id>` | `connection_id` en la ruta | `connection_id`, `spec_version`, `connection_type`, `created_utc`, `created_element_ids[]`, `created_elements_count`, `backend`, `spec` | Lee |
| POST | `/conn/update/` | `connection_id`, `spec`, `validation_token` | `connection_id`, `spec_version`, `connection_type`, `created_element_ids[]`, `created_elements_count`, `updated_utc` | **Escribe** |
| POST | `/conn/delete/` | `connection_id` | `deleted_connection_id`, `deleted_elements_count`, `restored_members_count` | **Escribe** |

Rutas de desarrollo (sin herramienta MCP; las usan `scripts\conn-call.ps1` y `scripts\revit-exec.ps1 -SinTransaccion`
del repositorio CONEXIONES):

| Método | Ruta | Cuerpo | Respuesta |
|---|---|---|---|
| POST | `/conn/op/<operation>/` | cualquier objeto JSON | Sobre común de `Bridge.Handle(<operation>, <cuerpo>)`; `UNKNOWN_OPERATION` si no existe |
| POST | `/conn/dev_exec/` | `code` (IronPython 2.7), `description` | Sobre común con `data.output` (y `data.traceback` + `PROBE_EXCEPTION` si falla). Ejecuta **sin** TransactionGroup ni Transaction envolventes, con `doc`, `uidoc`, `uiapp`, `DB`, `UI`, `revit`, `clr`, `System` y `print`. Mismo token y mismo poder que `/execute_code/`. |

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
`REVIT_WARNING` (aviso de Revit suprimido), `REVIT_DIALOG_SUPPRESSED`, `CATEGORY_FALLBACK`.

### Herramientas MCP

`tools/conn_tools.py` registra 13 herramientas, una por ruta con nombre, que devuelven el sobre como texto JSON
íntegro (`json.dumps(..., ensure_ascii=False, indent=2)`), nunca `format_response`:

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

Las herramientas comprueban en el puente que los argumentos obligatorios no estén vacíos y que `spec` sea un objeto
(o un texto JSON que lo contenga); si no, devuelven `INVALID_REQUEST` sin llamar a Revit. El manual de cada una es
su docstring.

### Deshacer

Cada operación de escritura (`create`, `update`, `delete`) es un `TransactionGroup` llamado
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

No crea nada en el modelo. Diecisiete pruebas contra 48884 (401 sin token; `ping`; guía; tipos; esquema; `find_profile`;
`node_info`; `validate` del Detalle D confirmado con token; `validate` con 420→402 → `DIMENSION_CHAIN_MISMATCH`;
`validate` con dudas sin confirmar → `UNRESOLVED_UNCERTAINTY`; `preview`; `create` sin token → `VALIDATION_TOKEN_INVALID`;
`list`; `get` y `delete` de un ID inexistente → `ELEMENT_NOT_FOUND`; `op/no_existe` → `UNKNOWN_OPERATION`) y, con
`--puente`, dos contra el puente en 8000 (`tools/list` con las 13 `conn_*` y `tools/call conn_ping`). Termina con
`Resultado: N/N pruebas correctas` y código de salida 0 si todas pasan.
