# -*- coding: utf-8 -*-
"""Herramientas conn_* de MotorConexiones para el servidor MCP (CPython 3.11+, SDK mcp 2.x).

Una herramienta por fila de la tabla de la sección 9 del encargo, más las cinco del catálogo de plantillas
(docs/prompts/fase-7.md). Cada una llama a una ruta
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

VERSION_HERRAMIENTAS = "0.7.0"  # Fase 7: 18 herramientas (13 de la Fase 4 + 5 del catalogo)

# Tiempos de espera (segundos) por operación. revit_post usa 30 s por defecto; las operaciones
# que abren la sesión de acero de Advance Steel (crear, actualizar, borrar) y la previsualización
# llevan 180 s como dice el encargo (la primera operación de acero de un documento tardó 132 s en la Fase 1).
TIEMPO_RAPIDO = 15.0
TIEMPO_LECTURA = 60.0
TIEMPO_ESCRITURA = 180.0

HERRAMIENTAS_CONN = (
    "conn_ping", "conn_get_guide", "conn_list_types", "conn_get_schema", "conn_get_node_info",
    "conn_find_profile", "conn_validate", "conn_preview", "conn_create", "conn_list", "conn_get",
    "conn_update", "conn_delete",
    "conn_catalog_list", "conn_catalog_get", "conn_catalog_save", "conn_catalog_delete", "conn_catalog_apply",
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
    """Registra las 18 herramientas conn_* en el servidor MCP (13 de la Fase 4 y 5 del catálogo, Fase 7)."""
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
    async def conn_list(ctx: Context = None) -> str:
        """Lista las conexiones creadas por MotorConexiones en el modelo abierto.

        Lee el almacenamiento del add-in (Extensible Storage), no la geometría.
        Úsala para encontrar un connection_id, para comprobar si una creación que
        no respondió llegó a guardarse, o antes de borrar o actualizar.

        Devuelve data: {connections_count, connections: [{connection_id,
        spec_version, connection_type, created_elements_count, backend, created_utc}]}.
        """
        respuesta = await revit_get("/conn/list/", ctx, timeout=TIEMPO_LECTURA)
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
