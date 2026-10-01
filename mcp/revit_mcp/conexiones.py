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
Se reservan 401 (token) y 500 (excepcion no controlada del adaptador).

Rutas de la Fase 1:
  GET  /conn/ping/             -> Bridge.Handle("ping", "{}", doc, uidoc)
  POST /conn/op/<operation>/   -> Bridge.Handle(operation, <cuerpo JSON>, doc, uidoc)   (generica)
  POST /conn/dev_exec/         -> ejecuta IronPython SIN transaccion envolvente (solo desarrollo:
                                  sondeos que abren sus propias transacciones, p. ej. API de acero)
Las rutas con nombre de la seccion 9 del encargo (validate, create, ...) se anaden en la Fase 4.

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
        cuerpo = json.dumps(data if isinstance(data, dict) else {})
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

    @api.route("/conn/ping/", methods=["GET"])
    @requiere_token
    def conn_ping(doc, uidoc):
        """Comprueba que el add-in esta cargado: version, Revit y backend activo."""
        try:
            return routes.make_response(data=llamar_bridge("ping", {}, doc, uidoc))
        except Exception as error:
            logger.error(u"conn_ping: %s", str(error))
            return routes.make_response(data={"error": str(error)}, status=500)

    @api.route("/conn/op/<operation>/", methods=["POST"])
    @requiere_token
    def conn_op(operation, doc, uidoc, request):
        """Ruta generica: cualquier operacion de Bridge.Handle con el cuerpo JSON como peticion."""
        try:
            return routes.make_response(data=llamar_bridge(str(operation), _datos_peticion(request), doc, uidoc))
        except Exception as error:
            logger.error(u"conn_op(%s): %s", operation, str(error))
            return routes.make_response(data={"error": str(error)}, status=500)

    @api.route("/conn/dev_exec/", methods=["POST"])
    @requiere_token
    def conn_dev_exec(doc, uidoc, uiapp, request):
        """Solo desarrollo: ejecuta IronPython en contexto de la API sin transaccion envolvente."""
        try:
            return routes.make_response(data=_ejecutar_sin_transaccion(doc, uidoc, uiapp, request))
        except Exception as error:
            logger.error(u"conn_dev_exec: %s", str(error))
            return routes.make_response(data={"error": str(error)}, status=500)

    logger.info("Rutas /conn/ de MotorConexiones registradas")
