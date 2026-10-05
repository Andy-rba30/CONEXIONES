# -*- coding: UTF-8 -*-
"""
Rutas /conn/... de MotorConexiones para pyRevit Routes (IronPython 2.7, dentro de Revit).

Adaptador delgado: localiza el ensamblado MotorConexiones.Revit ya cargado en el proceso de Revit
(AppDomain.CurrentDomain.GetAssemblies(); si no esta, clr.AddReferenceToFileAndPath con la DLL
desplegada), llama a MotorConexiones.Revit.Bridge.Handle(operation, requestJson, doc, uidoc) por
reflexion y devuelve el JSON tal cual. Cero logica de negocio aqui.

Todas las rutas declaran doc y uidoc para correr en contexto de la API de Revit (pyRevit las
ejecuta por ExternalEvent), llevan @requiere_token y responden SIEMPRE 200 con el sobre comun
{ ok, data, errors, warnings, meta } (main.py solo devuelve el JSON como dict si el HTTP es 200).
Se reservan 401 (token) y 500 (excepcion no controlada del adaptador). Sin documento abierto el
add-in responde 200 con ok:false y NO_DOCUMENT (no 503) para que la IA reciba el sobre completo.

Rutas con nombre (seccion 9 del encargo; Fase 4):
  GET  /conn/ping/                     -> ping          (add-in cargado, version, Revit, backend)
  GET  /conn/guide/                    -> guide         (docs/guide.md)
  GET  /conn/types/                    -> types         (tipos de conexion)
  GET  /conn/schema/<connection_type>  -> schema        (JSON Schema + ejemplo de un tipo)
  POST /conn/node_info/                -> node_info     (element_ids opcional; si falta, la seleccion)
  POST /conn/find_profile/             -> find_profile  (query)
  POST /conn/validate/                 -> validate      (spec)
  POST /conn/preview/                  -> preview       (spec)
  POST /conn/create/                   -> create        (spec, validation_token)
  GET  /conn/list/                     -> list
  GET  /conn/get/<connection_id>       -> get
  POST /conn/update/                   -> update        (connection_id, spec, validation_token)
  POST /conn/delete/                   -> delete        (connection_id)

Rutas del catalogo de plantillas (Fase 7; las plantillas son archivos JSON en el PC, apply y save necesitan el modelo):
  GET  /conn/catalog/list/             -> catalog_list
  GET  /conn/catalog/get/<template_id> -> catalog_get
  POST /conn/catalog/save/             -> catalog_save  (connection_id | spec, name, description, tags, overwrite, ...)
  POST /conn/catalog/delete/           -> catalog_delete (template_id)
  POST /conn/catalog/apply/            -> catalog_apply (template_id, element_ids opcional, chord_element_id, orientation)

Rutas del plan de lote (Fase 8; detecta los nudos de la seleccion, casa plantillas y valida nudo a nudo; NO crea nada;
batch_plan pone marcas de color y marcadores en la vista activa, batch_plan_discard las quita):
  POST /conn/batch/plan/            -> batch_plan         (element_ids opcional, template_ids, overrides, plan_id, mark)
  POST /conn/batch/plan/get/        -> batch_plan_get     (plan_id opcional: el ultimo; node opcional)
  POST /conn/batch/plan/discard/    -> batch_plan_discard (plan_id opcional; all)

Rutas de la Fase 1 que se conservan:
  POST /conn/op/<operation>/   -> Bridge.Handle(operation, <cuerpo JSON>, doc, uidoc)   (generica; la usan
                                  scripts/conn-call.ps1 y los sondeos; no tiene herramienta MCP)
  POST /conn/dev_exec/         -> ejecuta IronPython SIN transaccion envolvente (solo desarrollo:
                                  sondeos que abren sus propias transacciones, p. ej. API de acero;
                                  la usa scripts/revit-exec.ps1 -SinTransaccion; no tiene herramienta MCP)

Registro: startup.py hace `from revit_mcp.conexiones import register_conn_routes` y
`register_conn_routes(api)` dentro de register_routes() (lo anade mcp/instalar-conn.ps1).
"""
from pyrevit import routes, revit, DB, UI
from seguridad import requiere_token
import json
import logging
import os
import sys
import time
import traceback
from StringIO import StringIO

import clr
import System

logger = logging.getLogger(__name__)

VERSION_ADAPTADOR = "0.8.1"  # Ronda 8b: misma version que el add-in; Fase 8: rutas del plan de lote (Fase 7: catalogo; Fase 4: rutas con nombre)
ADDIN_VERSION_DESCONOCIDA = None
NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"
NOMBRE_TIPO_PUENTE = "MotorConexiones.Revit.Bridge"
NOMBRE_METODO = "Handle"
RUTA_DLL_DESPLEGADA = os.path.join(
    os.environ.get("APPDATA", ""), "Autodesk", "Revit", "Addins", "2027",
    "MotorConexiones", "MotorConexiones.Revit.dll",
)

# Cache del metodo Handle una vez localizado (el ensamblado no cambia sin reiniciar Revit).
_metodo_handle = None


# ---------------------------------------------------------------------------
# Sobre comun
# ---------------------------------------------------------------------------
def _sobre(ok, data, errors, warnings, operation, inicio):
    return {
        "ok": bool(ok),
        "data": data,
        "errors": errors or [],
        "warnings": warnings or [],
        "meta": {
            "operation": operation,
            "duration_ms": int((time.time() - inicio) * 1000),
            "addin_version": ADDIN_VERSION_DESCONOCIDA,
        },
    }


def _error(code, message, hint=None, path=None):
    return {"code": code, "path": path, "message": message, "hint": hint}


def _sobre_error(operation, inicio, code, message, hint=None, data=None):
    return _sobre(False, data, [_error(code, message, hint)], [], operation, inicio)


# ---------------------------------------------------------------------------
# Localizar Bridge.Handle
# ---------------------------------------------------------------------------
def _buscar_ensamblado(nombre):
    for ensamblado in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if ensamblado.GetName().Name == nombre:
                return ensamblado
        except Exception:
            pass
    return None


def _cargar_ensamblado():
    """Devuelve el ensamblado del add-in: primero el ya cargado por Revit, si no, la DLL desplegada."""
    ensamblado = _buscar_ensamblado(NOMBRE_ENSAMBLADO)
    if ensamblado is not None:
        return ensamblado, "AppDomain"
    if os.path.isfile(RUTA_DLL_DESPLEGADA):
        clr.AddReferenceToFileAndPath(RUTA_DLL_DESPLEGADA)
        ensamblado = _buscar_ensamblado(NOMBRE_ENSAMBLADO)
        if ensamblado is not None:
            logger.warning(
                u"MotorConexiones.Revit no estaba cargado por Revit; se cargo desde %s "
                u"(el boton de la cinta no existira hasta reiniciar Revit).",
                RUTA_DLL_DESPLEGADA,
            )
            return ensamblado, "AddReferenceToFileAndPath"
    return None, None


def _obtener_handle():
    """MethodInfo de Bridge.Handle(String, String, Document, UIDocument) o None si el add-in no esta."""
    global _metodo_handle
    if _metodo_handle is not None:
        return _metodo_handle
    ensamblado, origen = _cargar_ensamblado()
    if ensamblado is None:
        return None
    tipo = ensamblado.GetType(NOMBRE_TIPO_PUENTE)
    if tipo is None:
        logger.error(u"El ensamblado %s no contiene el tipo %s", NOMBRE_ENSAMBLADO, NOMBRE_TIPO_PUENTE)
        return None
    firma = System.Array[System.Type]([
        clr.GetClrType(System.String),
        clr.GetClrType(System.String),
        clr.GetClrType(DB.Document),
        clr.GetClrType(UI.UIDocument),
    ])
    metodo = tipo.GetMethod(NOMBRE_METODO, firma)
    if metodo is None:
        logger.error(u"%s no tiene el metodo %s(String, String, Document, UIDocument)", NOMBRE_TIPO_PUENTE, NOMBRE_METODO)
        return None
    _metodo_handle = metodo
    logger.info(u"MotorConexiones.Revit localizado (%s): %s", origen, ensamblado.Location)
    return metodo


def _mensaje_excepcion(error):
    """Texto de la excepcion, desenvolviendo TargetInvocationException de la reflexion."""
    interna = getattr(error, "InnerException", None)
    while interna is not None:
        error = interna
        interna = getattr(error, "InnerException", None)
    try:
        return "{0}: {1}".format(error.GetType().Name, error.Message)
    except Exception:
        return "{0}: {1}".format(type(error).__name__, str(error))


# IronPython 2.7: str y unicode son el mismo tipo. basestring/long no existen en CPython 3 (simulador).
try:
    _TIPOS_TEXTO = (basestring,)  # noqa: F821
    _TIPOS_ENTERO = (int, long)   # noqa: F821
except NameError:  # CPython 3 (solo el simulador de la nube)
    _TIPOS_TEXTO = (str,)
    _TIPOS_ENTERO = (int,)


def _escapar_ascii(texto):
    """Cadena JSON con todo lo que no sea ASCII imprimible escapado como \\uXXXX (sin pasar por str.decode)."""
    partes = []
    for caracter in texto:
        codigo = ord(caracter)
        if caracter == '"':
            partes.append('\\"')
        elif caracter == "\\":
            partes.append("\\\\")
        elif caracter == "\n":
            partes.append("\\n")
        elif caracter == "\r":
            partes.append("\\r")
        elif caracter == "\t":
            partes.append("\\t")
        elif codigo > 0xFFFF:
            # CPython 3 entrega el punto de codigo entero; JSON exige el par sustituto UTF-16 (IronPython ya itera por mitades).
            resto = codigo - 0x10000
            partes.append("\\u%04x\\u%04x" % (0xD800 + (resto >> 10), 0xDC00 + (resto & 0x3FF)))
        elif codigo < 0x20 or codigo > 0x7E:
            partes.append("\\u%04x" % codigo)
        else:
            partes.append(caracter)
    return '"' + "".join(partes) + '"'


def _json_ascii(valor):
    """Serializador JSON minimo (dict, list, str, int, float, bool, None) que escapa a mano los no ASCII."""
    if valor is None:
        return "null"
    if valor is True:
        return "true"
    if valor is False:
        return "false"
    if isinstance(valor, _TIPOS_ENTERO):
        return str(valor)
    if isinstance(valor, float):
        if valor != valor or valor in (float("inf"), float("-inf")):
            return "null"
        return repr(valor)
    if isinstance(valor, _TIPOS_TEXTO):
        return _escapar_ascii(valor)
    if isinstance(valor, dict):
        return "{" + ",".join(_escapar_ascii(str(k)) + ":" + _json_ascii(v) for k, v in valor.items()) + "}"
    if isinstance(valor, (list, tuple)):
        return "[" + ",".join(_json_ascii(v) for v in valor) + "]"
    return _escapar_ascii(str(valor))


def _a_json(datos):
    """JSON del cuerpo que se entrega a Bridge.Handle.

    En IronPython 2.7, json.dumps con ensure_ascii=True (el valor por defecto) llama a s.decode('utf-8') sobre
    cualquier texto con caracteres > 127, y eso falla con los acentos que pyRevit ya decodifico bien del cuerpo
    HTTP ("'unknown' codec can't decode byte 0xe1": Fase 4, primera ronda, pruebas 9-13). Con ensure_ascii=False
    el codificador no decodifica nada: el texto llega a C# como System.String y System.Text.Json lo lee tal cual.
    Si aun asi fallara, se serializa a mano escapando los no ASCII como \\uXXXX.
    """
    try:
        return json.dumps(datos, ensure_ascii=False)
    except Exception as error:
        logger.warning(u"json.dumps(ensure_ascii=False) fallo (%s); se escapa a mano", str(error))
        return _json_ascii(datos)


def llamar_bridge(operation, data, doc, uidoc):
    """Llama a Bridge.Handle y devuelve el sobre comun como dict (nunca lanza)."""
    inicio = time.time()
    metodo = _obtener_handle()
    if metodo is None:
        return _sobre_error(
            operation, inicio, "ADDIN_NOT_LOADED",
            u"El add-in MotorConexiones no está cargado en Revit.",
            u"Instálalo con scripts\\deploy.ps1 (con Revit cerrado) y vuelve a abrir Revit. "
            u"Se buscó en el proceso y en " + RUTA_DLL_DESPLEGADA,
        )
    try:
        cuerpo = _a_json(data if isinstance(data, dict) else {})
    except Exception as error:
        return _sobre_error(operation, inicio, "INVALID_REQUEST",
                            u"No se pudo serializar la petición: " + str(error))
    try:
        argumentos = System.Array[System.Object]([operation, cuerpo, doc, uidoc])
        texto = metodo.Invoke(None, argumentos)
    except Exception as error:
        logger.error(u"Bridge.Handle(%s) lanzo: %s", operation, _mensaje_excepcion(error))
        return _sobre_error(operation, inicio, "BRIDGE_CALL_FAILED",
                            u"La llamada a Bridge.Handle falló: " + _mensaje_excepcion(error),
                            u"Mira el registro del add-in en %LOCALAPPDATA%\\MotorConexiones\\log\\")
    try:
        return json.loads(texto)
    except Exception as error:
        return _sobre_error(operation, inicio, "BRIDGE_BAD_RESPONSE",
                            u"El add-in devolvió algo que no es JSON: " + str(error),
                            data={"raw": str(texto)[:2000]})


def _datos_peticion(request):
    """Cuerpo JSON de la peticion como dict (sin la clave token: ya la quito requiere_token)."""
    datos = getattr(request, "data", None)
    if isinstance(datos, dict):
        return datos
    if isinstance(datos, str) and datos.strip():
        try:
            parseado = json.loads(datos)
            if isinstance(parseado, dict):
                return parseado
        except Exception:
            pass
    return {}


def _responder(operation, datos, doc, uidoc):
    """Respuesta HTTP 200 con el sobre comun; 500 solo si el propio adaptador falla."""
    try:
        return routes.make_response(data=llamar_bridge(operation, datos, doc, uidoc))
    except Exception as error:
        logger.error(u"conn %s: %s", operation, str(error))
        return routes.make_response(data={"error": str(error)}, status=500)


# ---------------------------------------------------------------------------
# Ruta de desarrollo: IronPython sin transaccion envolvente
# ---------------------------------------------------------------------------
def _cerrar_transacciones(espacio, doc):
    """Deshace cualquier Transaction/TransactionGroup (o cualquier objeto con RollBack) que dejara vivo el codigo."""
    for nombre, valor in list(espacio.items()):
        try:
            if isinstance(valor, (DB.Transaction, DB.TransactionGroup)):
                if valor.HasStarted() and not valor.HasEnded():
                    valor.RollBack()
            elif hasattr(valor, "RollBack") and not isinstance(valor, type):
                valor.RollBack()
        except Exception:
            pass
    try:
        return bool(doc is not None and doc.IsModifiable)
    except Exception:
        return False


def _ejecutar_sin_transaccion(doc, uidoc, uiapp, request):
    inicio = time.time()
    datos = _datos_peticion(request)
    codigo = datos.get("code", "")
    if not codigo:
        return _sobre_error("dev_exec", inicio, "INVALID_REQUEST", u"Falta la clave 'code' con el código IronPython.")
    descripcion = (datos.get("description") or u"dev_exec")[:60]

    capturada = StringIO()
    salida_previa = sys.stdout
    espacio = {
        "doc": doc, "uidoc": uidoc, "uiapp": uiapp,
        "DB": DB, "UI": UI, "revit": revit, "clr": clr, "System": System,
        "__builtins__": __builtins__,
        "print": lambda *args: capturada.write(" ".join(str(a) for a in args) + "\n"),
    }
    try:
        sys.stdout = capturada
        exec(codigo, espacio)
        sys.stdout = salida_previa
        return _sobre(True, {"output": capturada.getvalue(), "description": descripcion}, [], [], "dev_exec", inicio)
    except SystemExit:
        # raise SystemExit es la forma de los sondeos de parar limpiamente ("PARADA: ..."): no es un error.
        sys.stdout = salida_previa
        return _sobre(True, {"output": capturada.getvalue(), "description": descripcion, "stopped": True}, [], [], "dev_exec", inicio)
    except BaseException as error:
        sys.stdout = salida_previa
        rastro = traceback.format_exc()
        colgada = _cerrar_transacciones(espacio, doc)
        avisos = []
        if colgada:
            avisos.append(_error("OPEN_TRANSACTION",
                                 u"Quedó una transacción abierta que no se pudo cerrar (doc.IsModifiable=True). Revísala en Revit."))
        return _sobre(False,
                      {"output": capturada.getvalue(), "traceback": rastro, "description": descripcion},
                      [_error("PROBE_EXCEPTION", u"{0}: {1}".format(type(error).__name__, str(error)),
                              u"Lee el traceback en data.traceback.")],
                      avisos, "dev_exec", inicio)
    finally:
        sys.stdout = salida_previa
        try:
            capturada.close()
        except Exception:
            pass


# ---------------------------------------------------------------------------
# Registro de rutas
# ---------------------------------------------------------------------------
def register_conn_routes(api):
    """Registra las rutas /conn/... (se llama desde startup.py)."""

    # --- Sin modelo (el add-in responde aunque no haya documento abierto) ---------------------------

    @api.route("/conn/ping/", methods=["GET"])
    @requiere_token
    def conn_ping(doc, uidoc):
        """Comprueba que el add-in esta cargado: version, Revit y backend activo."""
        return _responder("ping", {}, doc, uidoc)

    @api.route("/conn/guide/", methods=["GET"])
    @requiere_token
    def conn_guide(doc, uidoc):
        """Devuelve docs/guide.md (la guia para la IA) tal cual esta junto al add-in."""
        return _responder("guide", {}, doc, uidoc)

    @api.route("/conn/types/", methods=["GET"])
    @requiere_token
    def conn_types(doc, uidoc):
        """Tipos de conexion registrados en el add-in y cuando usar cada uno."""
        return _responder("types", {}, doc, uidoc)

    @api.route("/conn/schema/<connection_type>", methods=["GET"])
    @requiere_token
    def conn_schema(connection_type, doc, uidoc):
        """JSON Schema de un tipo de conexion mas un ejemplo lleno."""
        return _responder("schema", {"type": str(connection_type)}, doc, uidoc)

    # --- Lectura del modelo -----------------------------------------------------------------------

    @api.route("/conn/node_info/", methods=["POST"])
    @requiere_token
    def conn_node_info(doc, uidoc, request):
        """Punto de trabajo, sistema local y miembros del nudo (element_ids o la seleccion actual)."""
        return _responder("node_info", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/find_profile/", methods=["POST"])
    @requiere_token
    def conn_find_profile(doc, uidoc, request):
        """Busca tipos de perfil cargados que coincidan con una designacion (query)."""
        return _responder("find_profile", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/validate/", methods=["POST"])
    @requiere_token
    def conn_validate(doc, uidoc, request):
        """Valida la especificacion (spec) y, si no hay errores, emite el validation_token."""
        return _responder("validate", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/preview/", methods=["POST"])
    @requiere_token
    def conn_preview(doc, uidoc, request):
        """Simulacion en texto de lo que se crearia y modificaria (spec), sin tocar el modelo."""
        return _responder("preview", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/list/", methods=["GET"])
    @requiere_token
    def conn_list(doc, uidoc):
        """Conexiones creadas por el add-in en el documento abierto."""
        return _responder("list", {}, doc, uidoc)

    @api.route("/conn/get/<connection_id>", methods=["GET"])
    @requiere_token
    def conn_get(connection_id, doc, uidoc):
        """Especificacion guardada y elementos de una conexion por su connection_id."""
        return _responder("get", {"connection_id": str(connection_id)}, doc, uidoc)

    # --- Escritura en el modelo (el add-in abre su TransactionGroup; una peticion a la vez) --------

    @api.route("/conn/create/", methods=["POST"])
    @requiere_token
    def conn_create(doc, uidoc, request):
        """Crea la conexion (spec + validation_token). Atomica: o se crea todo o nada."""
        return _responder("create", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/update/", methods=["POST"])
    @requiere_token
    def conn_update(doc, uidoc, request):
        """Reemplaza una conexion conservando su connection_id (connection_id, spec, validation_token)."""
        return _responder("update", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/delete/", methods=["POST"])
    @requiere_token
    def conn_delete(doc, uidoc, request):
        """Borra una conexion (connection_id) y restaura los miembros modificados."""
        return _responder("delete", _datos_peticion(request), doc, uidoc)

    # --- Catalogo de plantillas (Fase 7): archivos JSON en el PC; list, get y delete no necesitan modelo ----

    @api.route("/conn/catalog/list/", methods=["GET"])
    @requiere_token
    def conn_catalog_list(doc, uidoc):
        """Plantillas del catalogo (nombre, id, tipo, etiquetas, barras, cordon, fecha), sin volcarlas."""
        return _responder("catalog_list", {}, doc, uidoc)

    @api.route("/conn/catalog/get/<template_id>", methods=["GET"])
    @requiere_token
    def conn_catalog_get(template_id, doc, uidoc):
        """Una plantilla completa por su template_id."""
        return _responder("catalog_get", {"template_id": str(template_id)}, doc, uidoc)

    @api.route("/conn/catalog/save/", methods=["POST"])
    @requiere_token
    def conn_catalog_save(doc, uidoc, request):
        """Guarda una plantilla desde una conexion creada (connection_id) o desde una especificacion (spec)."""
        return _responder("catalog_save", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/catalog/delete/", methods=["POST"])
    @requiere_token
    def conn_catalog_delete(doc, uidoc, request):
        """Borra el archivo de una plantilla (template_id)."""
        return _responder("catalog_delete", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/catalog/apply/", methods=["POST"])
    @requiere_token
    def conn_catalog_apply(doc, uidoc, request):
        """Aplica una plantilla a un nudo (template_id, element_ids o la seleccion): especificacion + validacion + token."""
        return _responder("catalog_apply", _datos_peticion(request), doc, uidoc)

    # --- Plan de lote (Fase 8): no crea conexiones; plan pone marcas en la vista activa y discard las quita ----

    @api.route("/conn/batch/plan/", methods=["POST"])
    @requiere_token
    def conn_batch_plan(doc, uidoc, request):
        """Detecta los nudos de la seleccion (o element_ids), casa las plantillas, valida nudo a nudo y marca el modelo."""
        return _responder("batch_plan", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/batch/plan/get/", methods=["POST"])
    @requiere_token
    def conn_batch_plan_get(doc, uidoc, request):
        """Devuelve el plan en memoria (plan_id o el ultimo) sin tocar el modelo."""
        return _responder("batch_plan_get", _datos_peticion(request), doc, uidoc)

    @api.route("/conn/batch/plan/discard/", methods=["POST"])
    @requiere_token
    def conn_batch_plan_discard(doc, uidoc, request):
        """Quita las marcas del modelo y olvida el plan (plan_id o el ultimo; all: true limpia todo)."""
        return _responder("batch_plan_discard", _datos_peticion(request), doc, uidoc)

    # --- Rutas de la Fase 1 (herramientas de desarrollo; sin herramienta MCP) ---------------------

    @api.route("/conn/op/<operation>/", methods=["POST"])
    @requiere_token
    def conn_op(operation, doc, uidoc, request):
        """Ruta generica: cualquier operacion de Bridge.Handle con el cuerpo JSON como peticion."""
        return _responder(str(operation), _datos_peticion(request), doc, uidoc)

    @api.route("/conn/dev_exec/", methods=["POST"])
    @requiere_token
    def conn_dev_exec(doc, uidoc, uiapp, request):
        """Solo desarrollo: ejecuta IronPython en contexto de la API sin transaccion envolvente."""
        try:
            return routes.make_response(data=_ejecutar_sin_transaccion(doc, uidoc, uiapp, request))
        except Exception as error:
            logger.error(u"conn_dev_exec: %s", str(error))
            return routes.make_response(data={"error": str(error)}, status=500)

    logger.info("Rutas /conn/ de MotorConexiones %s registradas (23 rutas)", VERSION_ADAPTADOR)
