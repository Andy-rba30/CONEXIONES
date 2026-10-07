# -*- coding: utf-8 -*-
"""Herramientas conn_* de MotorConexiones para el servidor MCP (CPython 3.11+, SDK mcp 2.x).

Una herramienta por fila de la tabla de la sección 9 del encargo, más las cinco del catálogo de plantillas
(docs/prompts/fase-7.md), las tres del plan de lote (docs/prompts/fase-8.md) y las dos de crear por lotes
(docs/prompts/fase-9.md: conn_batch_create y conn_batch_delete; conn_list filtra por batch_id). Cada una llama a una ruta
/conn/... de Revit (mcp/revit_mcp/conexiones.py) y devuelve el JSON íntegro del sobre común
{ ok, data, errors, warnings, meta } con json.dumps(ensure_ascii=False, indent=2), nunca
format_response, para que la IA reciba la respuesta sin aplanar.

Las docstrings son el manual de la IA: cuándo usar cada herramienta, qué hace falta antes,
qué devuelve y los errores comunes. El flujo completo está en conn_get_guide.

Registro (lo añade mcp/instalar-conn.ps1 en tools/__init__.py):
    from .conn_tools import register_conn_tools
    register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)
"""
import json
from urllib.parse import quote

from mcp.server.mcpserver import Context

VERSION_HERRAMIENTAS = "0.10.1"  # Ronda 10b (0.10.1): correccion del cuadro de la seleccion asistida en la cinta; sin cambios en las herramientas. Fase 10 (0.10.0): conn_batch_plan con expand_selection (seleccion asistida) y labels (etiquetas pinchables); 23 herramientas. Fase 9 (0.9.0): conn_batch_create, conn_batch_delete y conn_list con batch_id (23 herramientas). Cierre de la ronda 8d (0.8.5): correcciones de la ventana del plan; sin cambios en las herramientas. Cierre de la ronda 8c (0.8.4): ventana del plan no modal; sin cambios en las herramientas. Ronda 8c (0.8.3): manual de conn_batch_plan con summary_text, status_text y advice. Fase 8: 21 herramientas (13 de la Fase 4 + 5 del catalogo + 3 del plan de lote)

# Tiempos de espera (segundos) por operación. revit_post usa 30 s por defecto; las operaciones
# que abren la sesión de acero de Advance Steel (crear, actualizar, borrar) y la previsualización
# llevan 180 s como dice el encargo (la primera operación de acero de un documento tardó 132 s en la Fase 1).
TIEMPO_RAPIDO = 15.0
TIEMPO_LECTURA = 60.0
TIEMPO_ESCRITURA = 180.0
# Fase 9: el lote crea un nudo detrás de otro, cada uno con su sesión de Advance Steel (unos segundos cada uno; la primera
# de un documento tardó 132 s en la Fase 1). 16 nudos pueden pasar de los 180 s: media hora de margen.
TIEMPO_LOTE = 1800.0

HERRAMIENTAS_CONN = (
    "conn_ping", "conn_get_guide", "conn_list_types", "conn_get_schema", "conn_get_node_info",
    "conn_find_profile", "conn_validate", "conn_preview", "conn_create", "conn_list", "conn_get",
    "conn_update", "conn_delete",
    "conn_catalog_list", "conn_catalog_get", "conn_catalog_save", "conn_catalog_delete", "conn_catalog_apply",
    "conn_batch_plan", "conn_batch_plan_get", "conn_batch_plan_discard",
    "conn_batch_create", "conn_batch_delete",
)


# ---------------------------------------------------------------------------
# Sobre común desde el lado del puente
# ---------------------------------------------------------------------------
def _sobre_local(operation, code, message, hint=None, path=None):
    """Sobre común generado en el puente (sin llegar a Revit)."""
    return {
        "ok": False,
        "data": None,
        "errors": [{"code": code, "path": path, "message": message, "hint": hint}],
        "warnings": [],
        "meta": {"operation": operation, "duration_ms": 0, "addin_version": None},
    }


def _clasificar_texto(texto):
    """Convierte el texto de error de revit_get/revit_post (main.py) en código + pista accionables."""
    t = (texto or "").strip()
    bajo = t.lower()
    if "token cambió" in bajo or "token cambio" in bajo:
        return ("REVIT_RESTARTED",
                "Revit se reinició y el token de sesión cambió. Espera unos segundos y repite la llamada.")
    if bajo.startswith("error: 404"):
        return ("CONN_ROUTE_NOT_FOUND",
                "La ruta /conn/ no existe en Revit: conexiones.py no está instalado o pyRevit no se recargó. "
                "Ejecuta mcp\\instalar-conn.ps1 y pyRevit > Reload (o reinicia Revit).")
    if bajo.startswith("error: 401"):
        return ("REVIT_UNAUTHORIZED",
                "Token de sesión rechazado. Reinicia Revit y vuelve a intentarlo.")
    if bajo.startswith("error: 5"):
        return ("CONN_ROUTE_EXCEPTION",
                "El adaptador conexiones.py lanzó una excepción dentro de Revit. Mira el registro de pyRevit "
                "y %LOCALAPPDATA%\\MotorConexiones\\log\\.")
    if "timeout" in bajo or "timed out" in bajo:
        return ("REVIT_TIMEOUT",
                "Revit no respondió a tiempo. Si estaba creando la conexión, comprueba con conn_list si quedó "
                "creada antes de repetir; si no, espera a que Revit termine la orden en curso.")
    if bajo.startswith("error:"):
        return ("REVIT_HTTP_ERROR",
                "Revit devolvió un código HTTP inesperado. Comprueba pyRevit > Routes y el registro de pyRevit.")
    return ("REVIT_UNREACHABLE",
            "Comprueba que Revit está abierto con pyRevit Routes activo (puerto 48884) y que conexiones.py "
            "está instalado (mcp\\instalar-conn.ps1) y pyRevit recargado.")


def _a_texto(respuesta, operation):
    """Sobre común como texto JSON. Si revit_get/revit_post devolvieron un texto de error, se envuelve."""
    if isinstance(respuesta, str):
        code, hint = _clasificar_texto(respuesta)
        return json.dumps(_sobre_local(operation, code, respuesta, hint), ensure_ascii=False, indent=2)
    if not isinstance(respuesta, dict):
        return json.dumps(_sobre_local(operation, "BRIDGE_BAD_RESPONSE",
                                       "Revit devolvió algo que no es un objeto JSON: " + repr(respuesta)[:500]),
                          ensure_ascii=False, indent=2)
    return json.dumps(respuesta, ensure_ascii=False, indent=2)


def _spec_a_dict(spec, operation):
    """Acepta la especificación como objeto o como texto JSON. Devuelve (dict, None) o (None, sobre de error en texto)."""
    if isinstance(spec, dict):
        return spec, None
    if isinstance(spec, str):
        try:
            parseado = json.loads(spec)
        except ValueError as error:
            return None, json.dumps(_sobre_local(
                operation, "INVALID_REQUEST",
                "La especificación no es JSON válido: {}".format(error),
                "Pasa 'spec' como objeto JSON (o como texto JSON bien formado).", "spec"),
                ensure_ascii=False, indent=2)
        if isinstance(parseado, dict):
            return parseado, None
    return None, json.dumps(_sobre_local(
        operation, "INVALID_REQUEST",
        "La especificación debe ser un objeto JSON con spec_version, connection_type, node, chord, gusset, members...",
        "Pide el ejemplo con conn_get_schema y rellénalo.", "spec"),
        ensure_ascii=False, indent=2)


def _texto_no_vacio(valor, nombre, operation):
    """Devuelve (valor limpio, None) o (None, sobre de error en texto) si falta."""
    limpio = (valor or "").strip() if isinstance(valor, str) else ""
    if not limpio:
        return None, json.dumps(_sobre_local(
            operation, "INVALID_REQUEST", "Falta '{}'.".format(nombre),
            "Pasa '{}' con un valor no vacío.".format(nombre), nombre), ensure_ascii=False, indent=2)
    return limpio, None


# ---------------------------------------------------------------------------
# Registro
# ---------------------------------------------------------------------------
def register_conn_tools(mcp, revit_get, revit_post, revit_image=None):
    """Registra las 23 herramientas conn_* en el servidor MCP (13 de la Fase 4, 5 del catálogo de la Fase 7, 3 del plan de lote de la Fase 8 y 2 de crear por lotes de la Fase 9)."""
    _ = revit_image  # se reserva para conn_preview con imagen (fuera de alcance en v1)

    # --- Descubrir ---------------------------------------------------------------------------------

    @mcp.tool()
    async def conn_ping(ctx: Context = None) -> str:
        """Comprueba que el add-in MotorConexiones está cargado en Revit.

        Úsala SIEMPRE antes de cualquier otra herramienta conn_*. No necesita nada
        y no toca el modelo.

        Devuelve el sobre común {ok, data, errors, warnings, meta}. Con ok:true,
        data trae: addin_version, spec_version (versión del contrato JSON),
        backend ("advancesteel": placas y pernos nativos de Advance Steel;
        "directshape": sólidos genéricos de reserva), operations (operaciones del
        puente), revit (version_number, version_build, language), dotnet y
        document (título y ruta del modelo abierto, o null si no hay documento).

        Errores comunes:
        - ADDIN_NOT_LOADED: el add-in no está instalado o Revit no se reinició
          tras instalarlo. Díselo al usuario y para.
        - REVIT_UNREACHABLE: Revit no responde (cerrado, Routes apagado).
        - CONN_ROUTE_NOT_FOUND: conexiones.py no está instalado en la extensión
          o pyRevit no se recargó (mcp\\instalar-conn.ps1 + pyRevit > Reload).
        """
        respuesta = await revit_get("/conn/ping/", ctx, timeout=TIEMPO_RAPIDO)
        return _a_texto(respuesta, "ping")

    @mcp.tool()
    async def conn_get_guide(ctx: Context = None) -> str:
        """Devuelve la guía de trabajo para crear conexiones de acero con MotorConexiones.

        Llámala al empezar, justo después de conn_ping, y sigue sus pasos en orden:
        inspeccionar el nudo, leer el detalle, transcribir cotas y dudas, validar,
        confirmar con el usuario, previsualizar y crear. La guía vive en
        docs\\guide.md junto al add-in y el usuario puede editarla sin recompilar.

        Devuelve data.guide_markdown (texto Markdown en español). No toca el modelo.
        """
        respuesta = await revit_get("/conn/guide/", ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "guide")

    @mcp.tool()
    async def conn_list_types(ctx: Context = None) -> str:
        """Lista los tipos de conexión que sabe crear el add-in y cuándo usar cada uno.

        En v1 solo existe "gusset_node": nudo de cercha con cartela plana, cordón
        continuo y diagonales/montantes HSS unidos por ranura soldada (welded_slot)
        o por placa cuchilla empernada (bolted_knife_plate). Para otros tipos
        (placa base, viga-columna, empalmes) di al usuario que no están en v1.

        Devuelve data.connection_types: lista de {type_name, description}. El
        type_name es el que se pasa a conn_get_schema y va en spec.connection_type.
        """
        respuesta = await revit_get("/conn/types/", ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "types")

    @mcp.tool()
    async def conn_get_schema(connection_type: str = "gusset_node", ctx: Context = None) -> str:
        """Devuelve el JSON Schema de un tipo de conexión y un ejemplo completo.

        Úsala antes de escribir la especificación: el esquema dice qué campos son
        obligatorios, sus tipos y rangos, y el ejemplo (data.example) muestra una
        especificación válida del Detalle D que puedes copiar y adaptar. Nunca
        inventes campos: el validador rechaza los desconocidos (SCHEMA_INVALID).

        Reglas del contrato v1 que el esquema no puede expresar del todo:
        - Todos los números van en mm y grados; los campos *_label llevan el
          texto del plano (por ejemplo "3/8\\"") y deben coincidir con el número.
        - Un campo solo puede ser null si su ruta está en uncertain_fields.
        - Las posiciones (outline.points_mm) van en el sistema local del nudo
          que devuelve conn_get_node_info, en mm.
        - end_setback_mm se mide desde el punto de trabajo del nudo.
        - En bolted_knife_plate, insertion_mm es la parte de la placa dentro de
          la ranura del HSS y first_row_from_plate_end_mm se mide desde el
          extremo libre de la placa (el que apoya en la cartela). La placa apoya
          sobre una cara de la cartela (plate.gusset_face: "+z" por defecto o
          "-z") y los pernos atraviesan cartela + placa; su longitud se calcula
          del agarre salvo que el plano la dé en bolts.length_mm (opcional).

        Args:
            connection_type: nombre del tipo (de conn_list_types). Por defecto "gusset_node".

        Devuelve data: {connection_type, description, json_schema, example}.
        """
        tipo, error = _texto_no_vacio(connection_type, "connection_type", "schema")
        if error:
            return error
        respuesta = await revit_get("/conn/schema/" + quote(tipo, safe=""), ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "schema")

    # --- Inspeccionar el modelo --------------------------------------------------------------------

    @mcp.tool()
    async def conn_get_node_info(
        element_ids: list[int] | None = None,
        chord_element_id: int | None = None,
        ctx: Context = None,
    ) -> str:
        """Inspecciona los miembros de un nudo de cercha en el modelo abierto de Revit.

        Antes: pide al usuario que seleccione en Revit el cordón y todas las barras
        que llegan al nudo (al menos 2 elementos de armazón estructural). Si no
        pasas element_ids se usa la selección actual de Revit; también puedes
        obtener los IDs con get_selected_elements.

        Devuelve, en data: origin_mm (punto de trabajo = cruce del eje del cordón
        con el del primer miembro), x_axis/y_axis/z_axis (sistema local del nudo:
        X = eje del cordón, Z = normal al plano de la cercha), axis_distance_mm,
        chord_element_id, members (por barra: element_id, family, type (= perfil
        del modelo), structural_type, start_mm, end_mm, length_mm, slope_deg,
        angle_in_plane_deg, node_end, material, is_chord) y existing_connections
        (conexiones del add-in que ya tocan esas barras). Los ángulos y posiciones
        de la especificación salen de aquí, no del dibujo.

        Args:
            element_ids: IDs de Revit de los miembros del nudo (opcional; si falta, la selección).
            chord_element_id: ID del cordón si no es la barra más horizontal (opcional).

        Errores comunes: INVALID_REQUEST (menos de 2 miembros), ELEMENT_NOT_FOUND,
        ELEMENT_NOT_A_MEMBER (no es armazón estructural), NODE_AXES_NOT_INTERSECTING
        (los ejes no se cruzan a menos de 5 mm: el modelo tiene las barras
        desplazadas; díselo al usuario), NO_DOCUMENT.
        """
        datos = {}
        if element_ids:
            datos["element_ids"] = [int(i) for i in element_ids]
        if chord_element_id is not None:
            datos["chord_element_id"] = int(chord_element_id)
        respuesta = await revit_post("/conn/node_info/", datos, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "node_info")

    @mcp.tool()
    async def conn_find_profile(query: str, ctx: Context = None) -> str:
        """Busca tipos de perfil de armazón estructural cargados en el modelo.

        Úsala cuando el rótulo del plano (por ejemplo "HSS2-1/2X2-1/2X3/16") no
        coincide literalmente con el nombre del tipo en Revit (por ejemplo
        "HSS2-1-2X2-1-2X3-16 64x64") o cuando conn_validate devuelve
        PROFILE_MISMATCH. El validador ya entiende las designaciones AISC
        normalizadas, así que en spec.members[].profile escribe el rótulo del plano.

        Args:
            query: designación o fragmento del nombre del perfil.

        Devuelve data: {query, total_profiles_in_model, matched_count,
        matches: [{type_name, family_name, exact_match}], suggestions} (hasta 3
        nombres parecidos cuando no hay coincidencias).
        """
        texto, error = _texto_no_vacio(query, "query", "find_profile")
        if error:
            return error
        respuesta = await revit_post("/conn/find_profile/", {"query": texto}, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "find_profile")

    # --- Validar, previsualizar, crear -------------------------------------------------------------

    @mcp.tool()
    async def conn_validate(spec: dict | str, ctx: Context = None) -> str:
        """Valida una especificación de conexión contra el contrato, los límites AISC y el modelo.

        Es OBLIGATORIA antes de conn_create y conn_update: solo esta herramienta
        entrega el validation_token, y solo cuando no hay ningún error y todas las
        uncertain_fields tienen user_confirmed_value. El token está ligado a la
        especificación exacta y al estado del modelo: deja de valer si cambia
        una coma de la especificación o se mueven las barras
        (VALIDATION_TOKEN_INVALID): entonces vuelve a validar.

        Args:
            spec: la especificación completa (objeto JSON con spec_version,
                connection_type, source, node, chord, gusset, members,
                dimension_chains, uncertain_fields). Pide el esquema y el
                ejemplo con conn_get_schema.

        Devuelve, con ok:true, data: {is_valid, validation_token, errors_count,
        warnings_count, calculated_values (origen y ejes del nudo), bolt_stacks
        (por placa cuchilla: cara de la cartela, agarre y longitud de perno que
        se crearán)}; con ok:false, errors: lista de {code, path, message, hint}
        en español. warnings nunca bloquea (por ejemplo ANGLE_DIFFERS_FROM_MODEL,
        WELD_BELOW_MINIMUM, BOLT_LENGTH_TOO_SHORT): muéstralas al usuario.

        Códigos frecuentes y qué hacer:
        - SCHEMA_INVALID: campo obligatorio ausente, tipo o rango incorrecto, o
          campo desconocido. Corrígelo tú.
        - UNRESOLVED_UNCERTAINTY: una duda sin user_confirmed_value. Pregunta al
          usuario y rellena el valor confirmado (y el campo correspondiente).
        - DIMENSION_CHAIN_MISMATCH: una cadena de cotas no suma su total. Vuelve
          a leer el plano; si no cierra, va a uncertain_fields.
        - LABEL_VALUE_MISMATCH: el rótulo (3/8", PL10, 5/8") no coincide con los mm.
        - PROFILE_MISMATCH: el perfil escrito no es el del modelo (usa
          conn_find_profile y las sugerencias).
        - BOLT_EDGE_DISTANCE_TOO_SMALL, BOLT_SPACING_TOO_SMALL, BOLT_OUTSIDE_PLATE:
          pernos fuera de los mínimos AISC o de la placa; depende del plano:
          pregunta al usuario.
        - OUTLINE_INVALID, PLATE_OUTSIDE_GUSSET, CLASH_WITH_FOREIGN_MEMBER: geometría.
        - ELEMENT_NOT_FOUND, ELEMENT_NOT_A_MEMBER, MEMBER_NOT_AT_NODE: los IDs no
          son los del nudo seleccionado (repite conn_get_node_info).
        """
        datos, error = _spec_a_dict(spec, "validate")
        if error:
            return error
        respuesta = await revit_post("/conn/validate/", {"spec": datos}, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "validate")

    @mcp.tool()
    async def conn_preview(spec: dict | str, ctx: Context = None) -> str:
        """Simula en texto lo que conn_create crearía y modificaría, sin tocar el modelo.

        Úsala después de validar y antes de crear, para enseñar al usuario un
        resumen y pedir su confirmación explícita. No guarda nada (dry_run) y no
        necesita validation_token. En v1 no devuelve imagen: para ver el
        resultado tras crear usa get_revit_view.

        Args:
            spec: la misma especificación que validaste.

        Devuelve data: summary {connection_type, backend, chord_element_id,
        first_member_element_id, working_point_mm, gusset_plates, knife_plates,
        bolts, weld_lines, members_modified, dry_run}, elements_to_create (cartela,
        placas cuchilla con su cara de la cartela y desplazamiento, grupos de
        pernos con agarre y longitud, interfaces soldadas con sus medidas) y
        members_to_modify (por barra: extremo que se retira, distancia actual al
        punto de trabajo, setback_mm y la extensión nueva que se fijará en Revit).
        """
        datos, error = _spec_a_dict(spec, "preview")
        if error:
            return error
        respuesta = await revit_post("/conn/preview/", {"spec": datos}, ctx, timeout=TIEMPO_ESCRITURA)
        return _a_texto(respuesta, "preview")

    @mcp.tool()
    async def conn_create(spec: dict | str, validation_token: str, ctx: Context = None) -> str:
        """Crea la conexión en el modelo de Revit. Exige el validation_token de conn_validate.

        Antes: conn_validate sin errores (token), conn_preview y la confirmación
        explícita del usuario. La operación es atómica: o se crea todo (cartela,
        placas cuchilla, pernos, soldaduras y retiros de las barras) o no se crea
        nada, y queda como UNA entrada de deshacer en Revit. Puede tardar hasta
        3 minutos la primera vez en un documento (Advance Steel). No la repitas
        si no responde: comprueba antes con conn_list.

        Args:
            spec: la especificación validada, sin cambiar ni una coma.
            validation_token: el token devuelto por conn_validate.

        Devuelve data: {connection_id (GUID estable guardado en el modelo),
        spec_version, connection_type, created_element_ids, created_elements_count,
        created_utc, backend}. Informa el connection_id al usuario: sirve para
        conn_get, conn_update y conn_delete. Las warnings REVIT_WARNING son avisos
        de Revit suprimidos (por ejemplo "solo visible en nivel de detalle Fino").

        Errores comunes: VALIDATION_TOKEN_INVALID (token ausente, o la especificación,
        el modelo o config/limits.json cambiaron desde conn_validate: vuelve a validar;
        el token no caduca por tiempo), REVIT_BUSY (Revit
        tiene una orden o un diálogo abierto: pide al usuario que lo cierre),
        INTERNAL_ERROR (fallo al modelar; se hizo rollback completo; mira hint).
        """
        token, error = _texto_no_vacio(validation_token, "validation_token", "create")
        if error:
            return error
        datos, error = _spec_a_dict(spec, "create")
        if error:
            return error
        respuesta = await revit_post("/conn/create/", {"spec": datos, "validation_token": token}, ctx,
                                     timeout=TIEMPO_ESCRITURA)
        return _a_texto(respuesta, "create")

    # --- Gestionar las conexiones creadas ----------------------------------------------------------

    @mcp.tool()
    async def conn_list(batch_id: str | None = None, ctx: Context = None) -> str:
        """Lista las conexiones creadas por MotorConexiones en el modelo abierto.

        Lee el almacenamiento del add-in (Extensible Storage), no la geometría.
        Úsala para encontrar un connection_id, para comprobar si una creación que
        no respondió llegó a guardarse (también un lote: conn_batch_create), o
        antes de borrar o actualizar.

        Args:
            batch_id: opcional (Fase 9): solo las conexiones de ese lote (el plan_id
                del plan que las creó); el filtro se aplica en el puente.

        Devuelve data: {connections_count, total_count, batch_id, batches {batch_id:
        cuántas}, connections: [{connection_id, spec_version, connection_type,
        created_elements_count, backend, created_utc, template_id, batch_id}]}.
        """
        respuesta = await revit_get("/conn/list/", ctx, timeout=TIEMPO_LECTURA)
        filtro = (batch_id or "").strip() if isinstance(batch_id, str) else ""
        if filtro and isinstance(respuesta, dict) and isinstance(respuesta.get("data"), dict):
            datos = respuesta["data"]
            todas = datos.get("connections") or []
            datos["connections"] = [c for c in todas if str(c.get("batch_id") or "").lower() == filtro.lower()]
            datos["connections_count"] = len(datos["connections"])
            datos["batch_id"] = filtro
            datos.setdefault("total_count", len(todas))
        return _a_texto(respuesta, "list")

    @mcp.tool()
    async def conn_get(connection_id: str, ctx: Context = None) -> str:
        """Devuelve la especificación guardada y los elementos de una conexión.

        Úsala para revisar lo que se creó, para partir de la especificación
        guardada al preparar un conn_update, o para enseñar al usuario los IDs
        de los elementos (puedes pasarlos a get_element_properties).

        Args:
            connection_id: GUID devuelto por conn_create o listado por conn_list.

        Devuelve data: {connection_id, spec_version, connection_type, created_utc,
        created_element_ids, created_elements_count, backend, spec (la
        especificación completa tal como se validó)}. ELEMENT_NOT_FOUND si el ID
        no existe en este modelo.
        """
        cid, error = _texto_no_vacio(connection_id, "connection_id", "get")
        if error:
            return error
        respuesta = await revit_get("/conn/get/" + quote(cid, safe=""), ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "get")

    @mcp.tool()
    async def conn_update(connection_id: str, spec: dict | str, validation_token: str, ctx: Context = None) -> str:
        """Reemplaza una conexión existente por una especificación nueva, conservando su connection_id.

        Flujo: conn_get para partir de la especificación guardada, modifícala,
        conn_validate con la nueva (token nuevo), conn_preview, confirmación del
        usuario y conn_update. Es atómico: borra lo creado, restaura las barras y
        vuelve a crear con la nueva especificación; si algo falla, el modelo
        queda como estaba. Puede tardar hasta 3 minutos.

        Args:
            connection_id: GUID de la conexión a reemplazar.
            spec: la especificación nueva, validada.
            validation_token: token de conn_validate para esa especificación nueva.

        Devuelve data: {connection_id, spec_version, connection_type,
        created_element_ids, created_elements_count, updated_utc}.
        Errores: ELEMENT_NOT_FOUND (el connection_id no existe),
        VALIDATION_TOKEN_INVALID, REVIT_BUSY, INTERNAL_ERROR (rollback completo).
        """
        cid, error = _texto_no_vacio(connection_id, "connection_id", "update")
        if error:
            return error
        token, error = _texto_no_vacio(validation_token, "validation_token", "update")
        if error:
            return error
        datos, error = _spec_a_dict(spec, "update")
        if error:
            return error
        respuesta = await revit_post("/conn/update/",
                                     {"connection_id": cid, "spec": datos, "validation_token": token},
                                     ctx, timeout=TIEMPO_ESCRITURA)
        return _a_texto(respuesta, "update")

    @mcp.tool()
    async def conn_delete(connection_id: str, ctx: Context = None) -> str:
        """Borra una conexión creada por el add-in y deja las barras como estaban.

        Borra solo los elementos que el add-in creó para ese connection_id
        (nunca elementos ajenos) y restaura los retiros de extremo originales de
        las barras. Pide confirmación al usuario antes de llamarla. Queda como
        una entrada de deshacer en Revit.

        Args:
            connection_id: GUID de la conexión (de conn_list o conn_get).

        Devuelve data: {deleted_connection_id, deleted_elements_count,
        restored_members_count}. ELEMENT_NOT_FOUND si el ID no existe;
        REVIT_BUSY si Revit tiene una orden abierta.
        """
        cid, error = _texto_no_vacio(connection_id, "connection_id", "delete")
        if error:
            return error
        respuesta = await revit_post("/conn/delete/", {"connection_id": cid}, ctx, timeout=TIEMPO_ESCRITURA)
        return _a_texto(respuesta, "delete")

    # --- Catálogo de plantillas (Fase 7) -----------------------------------------------------------

    @mcp.tool()
    async def conn_catalog_list(ctx: Context = None) -> str:
        """Lista las plantillas del catálogo de conexiones guardadas en el PC.

        Una plantilla es una conexión "típica" guardada sin los IDs del nudo, con el
        ángulo real de cada barra, para aplicarla a otros nudos de la cercha (también
        en espejo). Úsala para enseñar al usuario qué típicas hay y elegir una antes de
        conn_catalog_apply. No necesita modelo abierto y no toca nada.

        Devuelve data: {catalog_folder, shared_catalog_folder, templates_count,
        templates: [{template_id, name, description, connection_type, tags,
        members_count, chord_profile, pattern (ángulos con signo y lado de cada
        barra), created_utc, origin_drawing, origin_document, file}]}. Un archivo
        ilegible en la carpeta aparece como aviso TEMPLATE_INVALID.
        """
        respuesta = await revit_get("/conn/catalog/list/", ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "catalog_list")

    @mcp.tool()
    async def conn_catalog_get(template_id: str, ctx: Context = None) -> str:
        """Devuelve una plantilla completa del catálogo.

        Úsala para revisar qué guarda una plantilla (spec_template sin IDs, con slot
        por barra; member_pattern con ángulo con signo, lado y perfil esperado por
        ranura; chord_pattern; matching con la tolerancia y si se admite espejo) antes
        de aplicarla o para explicársela al usuario.

        Args:
            template_id: GUID de conn_catalog_list.

        Devuelve data: {template_id, name, file, pattern, template}. TEMPLATE_NOT_FOUND
        si no existe; TEMPLATE_INVALID si el archivo no se puede leer.
        """
        tid, error = _texto_no_vacio(template_id, "template_id", "catalog_get")
        if error:
            return error
        respuesta = await revit_get("/conn/catalog/get/" + quote(tid, safe=""), ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "catalog_get")

    @mcp.tool()
    async def conn_catalog_save(
        name: str,
        connection_id: str | None = None,
        spec: dict | str | None = None,
        description: str | None = None,
        tags: list[str] | None = None,
        overwrite: bool = False,
        profile_policy: str | None = None,
        copy_to_shared: bool = False,
        ctx: Context = None,
    ) -> str:
        """Guarda una conexión como plantilla con nombre en el catálogo del PC.

        El flujo natural: el usuario comprueba en Revit la conexión creada y dice
        "guárdala como típica"; pasas su connection_id (de conn_create o conn_list)
        y un nombre. También acepta spec (una especificación que valide contra el
        modelo abierto) en vez de connection_id. Hace falta el modelo abierto porque
        los ángulos de las barras se miden en él. La especificación debe estar sin
        dudas abiertas (TEMPLATE_HAS_OPEN_UNCERTAINTIES) y validar sin errores
        (TEMPLATE_SPEC_INVALID, con los errores de conn_validate debajo).

        Args:
            name: nombre corto y reconocible ("Nudo típico cordón inferior, 3 diagonales").
            connection_id: conexión creada por el add-in en este modelo (opcional si pasas spec).
            spec: especificación completa (objeto o texto JSON), opcional si pasas connection_id.
            description: texto libre (qué cartela, qué uniones).
            tags: etiquetas para buscar ("hangar", "cercha", "HSS").
            overwrite: true para sustituir una plantilla con el mismo nombre (TEMPLATE_EXISTS si no).
            profile_policy: "warn" (por defecto: avisar y usar el perfil del modelo), "require" o "ignore".
            copy_to_shared: true para copiar también a la carpeta compartida de config/catalog.json.

        Devuelve data: {template_id, name, file, shared_file, members_count,
        chord_profile, member_pattern, matching, origin}. Informa el template_id al
        usuario: es lo que usa conn_catalog_apply.
        """
        nombre, error = _texto_no_vacio(name, "name", "catalog_save")
        if error:
            return error
        datos = {"name": nombre, "overwrite": bool(overwrite), "copy_to_shared": bool(copy_to_shared)}
        if isinstance(connection_id, str) and connection_id.strip():
            datos["connection_id"] = connection_id.strip()
        if spec is not None:
            spec_dict, error = _spec_a_dict(spec, "catalog_save")
            if error:
                return error
            datos["spec"] = spec_dict
        if "connection_id" not in datos and "spec" not in datos:
            return json.dumps(_sobre_local(
                "catalog_save", "INVALID_REQUEST", "Pasa connection_id (una conexión creada) o spec (una especificación).",
                "Usa conn_list para ver los connection_id del modelo.", "connection_id"), ensure_ascii=False, indent=2)
        if isinstance(description, str) and description.strip():
            datos["description"] = description.strip()
        if tags:
            datos["tags"] = [str(t).strip() for t in tags if str(t).strip()]
        if isinstance(profile_policy, str) and profile_policy.strip():
            datos["profile_policy"] = profile_policy.strip().lower()
        respuesta = await revit_post("/conn/catalog/save/", datos, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "catalog_save")

    @mcp.tool()
    async def conn_catalog_delete(template_id: str, ctx: Context = None) -> str:
        """Borra una plantilla del catálogo (su archivo JSON). Pide confirmación al usuario antes.

        Las conexiones ya creadas con esa plantilla no cambian.

        Args:
            template_id: GUID de conn_catalog_list.

        Devuelve data: {deleted_template_id, name, file}. TEMPLATE_NOT_FOUND si no existe.
        """
        tid, error = _texto_no_vacio(template_id, "template_id", "catalog_delete")
        if error:
            return error
        respuesta = await revit_post("/conn/catalog/delete/", {"template_id": tid}, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "catalog_delete")

    @mcp.tool()
    async def conn_catalog_apply(
        template_id: str,
        element_ids: list[int] | None = None,
        chord_element_id: int | None = None,
        orientation: str | None = None,
        ctx: Context = None,
    ) -> str:
        """Aplica una plantilla del catálogo a UN nudo y devuelve la especificación ya validada con su token.

        Antes: pide al usuario que seleccione en Revit el cordón y todas las barras
        del nudo (o pasa element_ids). El add-in elige el cordón (el más horizontal,
        o chord_element_id), casa cada barra con una ranura de la plantilla por su
        ángulo (probando también la plantilla en espejo: same, mirror_x, mirror_y,
        both), escribe los IDs reales, transforma la cartela, pone el ángulo real en
        expected_angle_deg y pasa el resultado por conn_validate. No toca el modelo.

        Args:
            template_id: GUID de conn_catalog_list.
            element_ids: IDs de las barras del nudo (opcional: si falta, la selección de Revit).
            chord_element_id: ID del cordón si no es la barra más horizontal (opcional).
            orientation: "auto" (por defecto, prueba las cuatro), "same", "mirror_x", "mirror_y" o "both".

        Devuelve, con ok:true, data: {template_id, name, node {chord_element_id,
        element_ids}, match {orientation, assignments [slot, element_id,
        template_angle_deg, model_angle_deg, deviation_deg], unassigned_members,
        description}, spec (la especificación instanciada), is_valid,
        validation_token, calculated_values, bolt_stacks}. Con ok:false, errors
        trae los de conn_validate (y data sigue con spec y match para corregir) o
        TEMPLATE_NO_MATCH (data.attempts explica qué ranura no encontró barra en
        cada orientación). Avisos: TEMPLATE_ANGLE_DEVIATION (una barra se desvía
        de la plantilla) y TEMPLATE_PROFILE_DIFFERS (perfil distinto, se usó el del
        modelo). Enseña al usuario la orientación y los avisos; con su visto bueno,
        conn_preview y conn_create con data.spec y data.validation_token tal cual.
        """
        tid, error = _texto_no_vacio(template_id, "template_id", "catalog_apply")
        if error:
            return error
        datos = {"template_id": tid}
        if element_ids:
            datos["element_ids"] = [int(i) for i in element_ids]
        if chord_element_id is not None:
            datos["chord_element_id"] = int(chord_element_id)
        if isinstance(orientation, str) and orientation.strip():
            datos["orientation"] = orientation.strip().lower()
        respuesta = await revit_post("/conn/catalog/apply/", datos, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "catalog_apply")

    # --- Plan de lote (Fase 8): detectar nudos, casar plantillas y validar nudo a nudo; NO crea nada ---------------

    @mcp.tool()
    async def conn_batch_plan(
        element_ids: list[int] | None = None,
        template_ids: list[str] | None = None,
        overrides: dict | str | None = None,
        plan_id: str | None = None,
        mark: bool = True,
        replace_existing: bool = False,
        include_specs: bool = False,
        expand_selection: bool = False,
        labels: bool = True,
        ctx: Context = None,
    ) -> str:
        """Planifica un lote: detecta los nudos de una cercha, casa cada uno con las plantillas y valida nudo a nudo. NO crea nada.

        Antes: pide al usuario que seleccione en Revit TODAS las barras de la cercha
        (cordones, diagonales y montantes) o pasa element_ids. Hace falta al menos
        una plantilla en el catálogo (conn_catalog_list); si no pasas template_ids
        se prueban todas y cada nudo toma la que mejor casa. Fase 10: con
        expand_selection=True basta UNA barra (o varias): el add-in añade las que
        la tocan dentro del plano de la cercha (cordones que pasan de largo, barras
        que llegan, tramos del cordón) y planifica con todas; data.selection_expansion
        dice cuántas y cuáles (summary_text), y llega el aviso SELECTION_EXPANDED.

        Qué hace el add-in: lleva cada extremo de barra al corte de su eje con el eje
        del cordón (las diagonales reales terminan en la cara del cordón, 15-85 mm de
        su eje: se admiten hasta medio canto + medio canto + 10 mm, node_face_reach_mm),
        agrupa esos puntos (10 mm), busca la barra que atraviesa cada grupo (cordón)
        y las que llegan (members[].end_gap_mm dice cuánto se queda corta cada una),
        calcula el marco canónico
        y los ángulos con signo, da nombre a los nudos (N1, N2… ordenados a lo largo
        de la cercha), salta los que ya tienen conexión del add-in, casa cada nudo con
        las plantillas (también en espejo: orientation same/mirror_x/mirror_y/both),
        instancia la especificación con source.batch_id y la pasa por conn_validate
        (token por nudo). Con mark (por defecto) colorea en la vista activa las barras
        de cada nudo y pone un marcador con su nombre en el punto de trabajo (cubo =
        misma orientación, rombo = en espejo); enséñaselo al usuario con
        get_revit_view. Las marcas se quitan con conn_batch_plan_discard. En un
        documento solo hay un plan marcado: marcar otro quita las marcas del
        anterior (aviso PLAN_MARKS_REPLACED; ese plan sigue en memoria). Fase 10:
        con las marcas van también una ETIQUETA pinchable con el número de cada
        nudo visible en la vista (labels, por defecto True; pincharla elige el nudo
        en la ventana del plan de la cinta, nunca abre un cuadro) y una CARTELA
        FANTASMA transparente en cada nudo listo (lo que se va a crear, antes de
        crearlo); data.labels, label_view_id, label_indices y nodes[].label_index /
        ghost_element_id lo reflejan; si no se pudieron poner, aviso PLAN_LABELS_SKIPPED
        (el plan sigue). Se quitan con las marcas.

        Args:
            element_ids: IDs de las barras de la cercha (opcional: si falta, la selección de Revit).
            template_ids: plantillas a probar, en orden de preferencia (opcional: todas las del catálogo).
            overrides: correcciones (objeto JSON, también como texto): {"exclude": ["N3"],
                "chord": {"N4": 1249510}, "template": {"N9": "<template_id>" | null},
                "remove_member": {"N2": [id]}, "add_member": {"N2": [id]}, "add_node": {"N11": [ids]},
                "merge": [["N5", "N6"]], "split": {"N5": [[ids], [ids]]}, "spec": {"N4": {...}},
                "include": ["N3"]}. Se acumulan en el plan.
            plan_id: para replanificar el mismo plan (misma selección, correcciones acumuladas, mismos nombres).
            mark: poner las marcas en la vista activa (True) o solo calcular (False).
            replace_existing: planificar también los nudos que ya tienen conexión (para rehacerlos en la Fase 9).
            include_specs: incluir la especificación completa de cada nudo (larga); por defecto no, usa
                conn_batch_plan_get con node para ver una.
            expand_selection: completar la selección con las barras que la tocan antes de planificar (Fase 10;
                por defecto no). Con True el usuario solo necesita pinchar una barra de la cercha.
            labels: poner las etiquetas pinchables con el número de cada nudo (Fase 10; por defecto sí, junto con mark).

        Devuelve data: {plan_id, summary {ready, invalid, no_match, ambiguous_chord, offset,
        untyped, already_connected, excluded}, summary_text (la decisión en español:
        "Se crearán 16 conexiones con Detalle D (8 iguales, 8 en espejo). 14 avisan de
        perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas."), visible_count,
        description, ready_count, is_marked, nodes[] {name, status, status_text (español,
        con icono: "● Listo", "▲ Listo con aviso", "✖ Falta el cordón"...), advice (qué
        hacer, una frase), visible_by_default (false en las barras sueltas y las parejas
        sin cordón: no son nudos), status_detail, chord_element_id, member_element_ids,
        members [element_id, angle_deg, side, end_gap_mm], template_name, orientation,
        is_mirrored, max_deviation_deg, errors, warnings, validation_token, color_name
        (el del estado: verde = se creará, ambar = con aviso, rojo = falta algo, gris =
        no se crea; null en los ocultos), existing_connection_id}, unused_element_ids,
        overrides}. Estados: ready (casa, valida y tiene token), invalid (errores de
        conn_validate: suele ser la cartela fija que no cubre una barra con otro ángulo),
        no_match (ninguna plantilla casa: falta o sobra una barra, o es otro tipo de nudo;
        con NODE_CHORD_NOT_CONTINUOUS, falta el cordón en la selección), ambiguous_chord
        (dos barras atraviesan: pasa overrides.chord), offset (los ejes no se cortan),
        untyped (una sola barra), already_connected (se salta). Aviso CATALOG_EMPTY si el
        catálogo no tiene plantillas (pide al usuario guardar primero una). NO vuelques el
        JSON al usuario: enséñale summary_text y una tabla corta solo con los nudos
        visible_by_default (name, status_text, espejo, template_name, advice); los
        ocultos, en una línea. Pide sus correcciones antes de dar el plan por bueno.
        Cada nudo trae también actions (los botones de la ventana: exclude, include,
        chord, members, template, edit, show…). Con el visto bueno del usuario, crear
        el lote es conn_batch_create con plan_id y los nudos listos con su
        validation_token.
        """
        datos = {"mark": bool(mark)}
        if element_ids:
            datos["element_ids"] = [int(i) for i in element_ids]
        if template_ids:
            datos["template_ids"] = [str(t).strip() for t in template_ids if str(t).strip()]
        if isinstance(plan_id, str) and plan_id.strip():
            datos["plan_id"] = plan_id.strip()
        if overrides is not None:
            if isinstance(overrides, str):
                try:
                    overrides = json.loads(overrides) if overrides.strip() else None
                except ValueError as error:
                    return json.dumps(_sobre_local("batch_plan", "INVALID_REQUEST",
                                                   "overrides no es JSON válido: {}".format(error),
                                                   "Pasa overrides como objeto JSON.", "overrides"), ensure_ascii=False, indent=2)
            if overrides is not None and not isinstance(overrides, dict):
                return json.dumps(_sobre_local("batch_plan", "INVALID_REQUEST", "overrides debe ser un objeto JSON.",
                                               "Ejemplo: {\"exclude\": [\"N3\"]}.", "overrides"), ensure_ascii=False, indent=2)
            if overrides:
                datos["overrides"] = overrides
        if replace_existing:
            datos["replace_existing"] = True
        if not include_specs:
            datos["include_specs"] = False
        if expand_selection:
            datos["expand_selection"] = True
        if not labels:
            datos["labels"] = False
        respuesta = await revit_post("/conn/batch/plan/", datos, ctx, timeout=TIEMPO_ESCRITURA)
        return _a_texto(respuesta, "batch_plan")

    @mcp.tool()
    async def conn_batch_plan_get(plan_id: str | None = None, node: str | None = None, include_specs: bool = False, ctx: Context = None) -> str:
        """Relee un plan de lote guardado en memoria por el add-in (sin tocar el modelo).

        Args:
            plan_id: el plan (opcional: el último planificado).
            node: nombre de un nudo (N4) para recibir solo ese, con su especificación completa y su token.
            include_specs: incluir la especificación de todos los nudos (larga).

        Devuelve lo mismo que conn_batch_plan (o data.node con un solo nudo). Error
        PLAN_NOT_FOUND si no hay plan (se descartó o Revit se reinició): vuelve a
        conn_batch_plan.
        """
        datos = {}
        if isinstance(plan_id, str) and plan_id.strip():
            datos["plan_id"] = plan_id.strip()
        if isinstance(node, str) and node.strip():
            datos["node"] = node.strip()
        if include_specs:
            datos["include_specs"] = True
        respuesta = await revit_post("/conn/batch/plan/get/", datos, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "batch_plan_get")

    @mcp.tool()
    async def conn_batch_plan_discard(plan_id: str | None = None, all: bool = False, ctx: Context = None) -> str:
        """Quita las marcas del plan en el modelo (colores, marcadores, cartelas fantasma y etiquetas) y olvida el plan. No toca ninguna conexión.

        Args:
            plan_id: el plan (opcional: el último).
            all: True quita todas las marcas de MotorConexiones del documento, también de planes que el
                add-in ya no recuerda (tras reiniciar Revit), y todas las etiquetas del lienzo.

        Llámala cuando el usuario termine de revisar y no quiera seguir (o antes de
        guardar el modelo). Devuelve data: {discarded_plan_id, removed_marks,
        removed_labels, remaining_plans, remaining_markers, remaining_labels}.
        """
        datos = {}
        if isinstance(plan_id, str) and plan_id.strip():
            datos["plan_id"] = plan_id.strip()
        if all:
            datos["all"] = True
        respuesta = await revit_post("/conn/batch/plan/discard/", datos, ctx, timeout=TIEMPO_LECTURA)
        return _a_texto(respuesta, "batch_plan_discard")

    # --- Crear por lotes (Fase 9) -----------------------------------------------------------------------------

    @mcp.tool()
    async def conn_batch_create(
        plan_id: str,
        nodes: list[dict] | str,
        stop_on_error: bool = False,
        include_specs: bool = False,
        ctx: Context = None,
    ) -> str:
        """Crea las conexiones de un plan de lote, nudo a nudo, con los validation_token de conn_batch_plan. ESCRIBE en el modelo.

        Antes: un plan de conn_batch_plan revisado con el usuario (enséñale
        summary_text y la tabla corta) y su CONFIRMACIÓN EXPLÍCITA. Cada nudo va con
        el validation_token que devolvió conn_batch_plan (o conn_batch_plan_get):
        sin token no se crea nada, y si el plan se replanificó hay que pasar los
        tokens nuevos. El add-in vuelve a comprobar cada token contra el modelo como
        hace conn_create.

        Qué hace: cada nudo es una operación atómica propia (como conn_create:
        cartela, placas cuchilla, pernos, soldaduras, retiros, registro) anidada en
        un grupo del lote: si un nudo falla se revierte solo y los demás se quedan;
        en Revit todo el lote es UNA entrada de deshacer. Solo se crean los nudos
        ready (o failed de un intento anterior); invalid, no_match, excluded,
        untyped, already_connected y los ya creados se saltan con su motivo. Las
        marcas (colores y marcadores) de los nudos creados se quitan; las demás
        siguen. Cada conexión creada lleva source.batch_id = plan_id (conn_list lo
        enseña) y source.template_id. Un nudo planificado con replace_existing se
        rehace con su mismo connection_id. Puede tardar varios minutos (cada nudo
        abre su sesión de Advance Steel): si la llamada no responde, NO la repitas a
        ciegas: conn_list con batch_id = plan_id dice qué quedó creado.

        Args:
            plan_id: el plan (de conn_batch_plan).
            nodes: lista de {"node": "N4", "validation_token": "..."} (también como
                texto JSON); "spec" opcional sustituye a la del plan solo en esa
                creación. Normalmente, los nudos ready del plan con su token.
            stop_on_error: True = el primer fallo revierte el lote ENTERO
                (BATCH_STOPPED, ok:false, informe en data). Por defecto False (P9).
            include_specs: incluir la especificación creada de cada nudo (larga).

        Devuelve data: {batch_id (= plan_id), created_count, updated_count,
        with_warnings_count, failed_count, skipped_count, rolled_back_count,
        connection_ids, undo_entries ("one" | "per_node"), duration_ms, summary_text
        ("Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: …), 2
        saltadas. Una sola entrada de deshacer (Ctrl+Z)."), nodes[] {node, outcome
        (created | created_with_warnings | updated | failed | skipped | rolled_back),
        connection_id, elements_count, duration_ms, reason, description, errors,
        warnings}, plan_summary, plan_summary_text}. Enseña al usuario summary_text y
        los nudos failed con su motivo; no vuelques el JSON. Avisos: BATCH_NODE_FAILED
        por cada nudo fallido. Errores: PLAN_NOT_FOUND (planifica otra vez),
        INVALID_REQUEST (falta plan_id, nodes o un token), VALIDATION_TOKEN_INVALID
        por nudo (token distinto del plan o modelo cambiado: replanifica),
        BATCH_STOPPED (con stop_on_error). Para quitar el lote: conn_batch_delete.
        """
        pid, error = _texto_no_vacio(plan_id, "plan_id", "batch_create")
        if error:
            return error
        if isinstance(nodes, str):
            try:
                nodes = json.loads(nodes) if nodes.strip() else None
            except ValueError as err:
                return json.dumps(_sobre_local("batch_create", "INVALID_REQUEST", "nodes no es JSON válido: {}".format(err),
                                               "Pasa nodes como lista de {\"node\": \"N4\", \"validation_token\": \"...\"}.", "nodes"),
                                  ensure_ascii=False, indent=2)
        if not isinstance(nodes, list) or not nodes:
            return json.dumps(_sobre_local("batch_create", "INVALID_REQUEST", "nodes debe ser una lista con al menos un nudo.",
                                           "Cada elemento: {\"node\": \"N4\", \"validation_token\": \"<token de conn_batch_plan>\"}.", "nodes"),
                              ensure_ascii=False, indent=2)
        limpios = []
        for i, n in enumerate(nodes):
            if not isinstance(n, dict):
                return json.dumps(_sobre_local("batch_create", "INVALID_REQUEST", "nodes[{}] debe ser un objeto {{node, validation_token}}.".format(i),
                                               "Un nombre suelto no basta: sin validation_token no se crea nada.", "nodes[{}]".format(i)),
                                  ensure_ascii=False, indent=2)
            nombre = str(n.get("node") or n.get("name") or "").strip()
            token = str(n.get("validation_token") or "").strip()
            if not nombre or not token:
                return json.dumps(_sobre_local("batch_create", "INVALID_REQUEST",
                                               "nodes[{}] necesita 'node' y 'validation_token' (el que devolvió conn_batch_plan para ese nudo).".format(i),
                                               "Sin token no se crea nada.", "nodes[{}]".format(i)), ensure_ascii=False, indent=2)
            item = {"node": nombre, "validation_token": token}
            if isinstance(n.get("spec"), dict):
                item["spec"] = n["spec"]
            limpios.append(item)
        datos = {"plan_id": pid, "nodes": limpios}
        if stop_on_error:
            datos["stop_on_error"] = True
        if include_specs:
            datos["include_specs"] = True
        respuesta = await revit_post("/conn/batch/create/", datos, ctx, timeout=TIEMPO_LOTE)
        return _a_texto(respuesta, "batch_create")

    @mcp.tool()
    async def conn_batch_delete(batch_id: str, ctx: Context = None) -> str:
        """Borra TODAS las conexiones de un lote (las creadas por conn_batch_create con ese plan_id). ESCRIBE en el modelo.

        Antes: pide confirmación explícita al usuario (dile cuántas son: conn_list
        con batch_id). Borra una a una con las garantías de conn_delete (solo lo que
        creó el add-in; las barras recuperan su extensión original), todo en UNA
        entrada de deshacer. Una conexión que no se pueda borrar se anota y se sigue
        con las demás. Si el plan sigue en memoria, sus nudos vuelven a listos.

        Args:
            batch_id: el plan_id del plan que creó el lote (conn_list enseña el
                batch_id de cada conexión y data.batches cuenta por lote).

        Devuelve data: {batch_id, deleted_count, failed_count, deleted_elements_count,
        restored_members_count, summary_text, nodes[] {connection_id, node, outcome
        (deleted | failed), elements_count, restored_members_count, errors}}. Aviso
        BATCH_EMPTY si no hay ninguna conexión de ese lote (deleted_count 0).
        """
        bid, error = _texto_no_vacio(batch_id, "batch_id", "batch_delete")
        if error:
            return error
        respuesta = await revit_post("/conn/batch/delete/", {"batch_id": bid}, ctx, timeout=TIEMPO_LOTE)
        return _a_texto(respuesta, "batch_delete")
