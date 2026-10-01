#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Simulador de Revit + pyRevit Routes para probar en la nube, sin Revit, el lado MCP de MotorConexiones.

Carga el adaptador real mcp/revit_mcp/conexiones.py (y el decorador real revit_mcp/seguridad.py de la
extensión, si se indica --extension) con módulos pyrevit/clr/System simulados, sustituye Bridge.Handle
por una imitación en Python del add-in (mismas operaciones, mismos códigos de error, mismo sobre común)
y sirve las rutas /revit_mcp/conn/... por HTTP en el puerto 48884 con el token de sesión, igual que
pyRevit Routes. Con eso se pueden ejecutar en Linux:

  * mcp/pruebas/probar_conexiones.py            (habla con 48884)
  * main.py --streamable-http de revit-mcp       (el puente real, con tools/conn_tools.py instalado)

NO sustituye a las pruebas en el PC: la imitación del add-in es deliberadamente simple (solo lo que
hace falta para comprobar el adaptador, el script de pruebas y el puente). Lo que pasa dentro de
Revit solo lo prueba el instalador.

Uso:
    python3 mcp/pruebas/simulador_revit.py --autocomprobar [--extension <clon de revit-mcp>]
        Comprueba en proceso que conexiones.py registra las 15 rutas y que cada una llega a Bridge.Handle
        con la operación y el cuerpo correctos. Código de salida 0 si todo va bien.

    python3 mcp/pruebas/simulador_revit.py [--puerto 48884] [--token-archivo <ruta>] [--extension <clon>]
        Sirve HTTP hasta Ctrl+C. Escribe el token en --token-archivo (por defecto, el nombre literal
        "%LOCALAPPDATA%\\RevitMcp\\token" en la carpeta actual, que es lo que main.py y probar_conexiones.py
        abren cuando corren en Linux, donde expandvars no toca las variables de Windows).

    Opciones: --sin-addin (responde ADDIN_NOT_LOADED), --sin-documento (NO_DOCUMENT en las operaciones
    que necesitan modelo), --conexiones <ruta> (otro conexiones.py, por ejemplo el copiado a la extensión).
"""
import argparse
import hashlib
import inspect
import io
import json
import os
import re
import sys
import threading
import time
import types
import uuid
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import parse_qs, unquote, urlsplit

RAIZ = Path(__file__).resolve().parents[2]
RUTA_CONEXIONES = RAIZ / "mcp" / "revit_mcp" / "conexiones.py"
RUTA_GUIA = RAIZ / "docs" / "guide.md"
RUTA_FIXTURE = RAIZ / "docs" / "fixtures" / "detalle-D-confirmado.json"
NOMBRE_TOKEN_LITERAL = r"%LOCALAPPDATA%\RevitMcp\token"

# Nudo del Detalle D en HANGAR_PRUEBA_sondeo.rvt (resultados de la Fase 3).
MIEMBROS = {
    1249510: {"family": "HSS-Hollow Structural Section", "type": "HSS3X3X1/4", "angle": 180.0, "chord": True,
              "start": [-4437.3, -17195.7, 17423.0], "end": [-14397.6, -17195.8, 17423.0]},
    1249630: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 43.1, "chord": False,
              "start": [-14536.8, -17195.8, 19918.8], "end": [-11930.6, -17195.8, 17481.8]},
    1249631: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 135.6, "chord": False,
              "start": [-11880.4, -17195.8, 17441.0], "end": [-9400.0, -17195.8, 19900.0]},
    1249636: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 45.0, "chord": False,
              "start": [-9300.0, -17195.8, 14900.0], "end": [-11832.5, -17195.8, 17387.8]},
}
PERFILES_MODELO = ["HSS3X3X1/4", "HSS2-1-2X2-1-2X3-16 64x64", "HSS4X4X1/4", "W12X26", "L3X3X1/4", "C8X11.5"]
ORIGEN_MM = [-11867.7, -17195.8, 17423.0]
PROYECTO_UNIQUE_ID = "simulador-00000000-0000-0000-0000-000000000001"
ADDIN_VERSION = "0.1.0"

LLAMADAS = []          # (operation, request dict) que recibe el Bridge simulado
CONEXIONES = {}        # connection_id -> registro
OPCIONES = types.SimpleNamespace(sin_addin=False, sin_documento=False)


# ---------------------------------------------------------------------------
# Imitación del add-in (Bridge.Handle)
# ---------------------------------------------------------------------------
def _sobre(operation, ok, data=None, errors=None, warnings=None):
    return {"ok": ok, "data": data, "errors": errors or [], "warnings": warnings or [],
            "meta": {"operation": operation, "duration_ms": 1, "addin_version": ADDIN_VERSION}}


def _err(code, message, path=None, hint=None):
    return {"code": code, "path": path, "message": message, "hint": hint}


def _canonico(obj):
    return json.dumps(obj, sort_keys=True, separators=(",", ":"), ensure_ascii=False)


def _token_de(spec):
    ids = set(spec.get("node", {}).get("element_ids", []) or [])
    if spec.get("chord", {}).get("element_id"):
        ids.add(spec["chord"]["element_id"])
    partes = [_canonico(spec), "|PROJECT:" + PROYECTO_UNIQUE_ID]
    for i in sorted(ids):
        m = MIEMBROS.get(i)
        if m:
            partes.append("|{}:{}:{}:{}".format(i, m["type"], m["start"], m["end"]))
    return hashlib.sha256("".join(partes).encode("utf-8")).hexdigest()


def _normalizar_perfil(texto):
    return re.sub(r"[\s/\-]", "", (texto or "").upper())


def _spec_de(req):
    spec = req.get("spec") if isinstance(req.get("spec"), dict) else req
    return spec if isinstance(spec, dict) else None


def _op_ping(req):
    documento = None if OPCIONES.sin_documento else {
        "title": "HANGAR_PRUEBA_sondeo", "path": r"D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt",
        "is_family": False, "is_workshared": False, "is_modifiable": False, "is_read_only": False}
    return _sobre("ping", True, {
        "addin_version": ADDIN_VERSION, "spec_version": "1.0", "backend": "advancesteel",
        "operations": sorted(OPERACIONES.keys()),
        "revit": {"version_number": "2027", "version_build": "27.2.0.39", "version_name": "Autodesk Revit 2027",
                  "sub_version_number": "2027.2", "language": "English_USA"},
        "dotnet": {"framework": ".NET 10.0.12 (simulado)", "assembly_location": "simulador", "load_context": "Default"},
        "document": documento, "has_uidocument": not OPCIONES.sin_documento})


def _op_guide(req):
    texto = RUTA_GUIA.read_text(encoding="utf-8") if RUTA_GUIA.is_file() else "# Guía (no encontrada)"
    return _sobre("guide", True, {"guide_markdown": texto})


def _op_types(req):
    return _sobre("types", True, {"connection_types": [{
        "type_name": "gusset_node",
        "description": "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por "
                       "ranura soldada o placa cuchilla empernada."}]})


def _op_schema(req):
    tipo = req.get("type", "gusset_node")
    if tipo != "gusset_node":
        return _sobre("schema", False, errors=[_err("UNKNOWN_OPERATION",
                      "El tipo de conexión '{}' no está registrado.".format(tipo), "type",
                      "Tipos disponibles: gusset_node")])
    ejemplo = json.loads(RUTA_FIXTURE.read_text(encoding="utf-8")) if RUTA_FIXTURE.is_file() else {}
    esquema = {"$schema": "http://json-schema.org/draft-07/schema#", "title": "gusset_node (simulado)",
               "type": "object", "additionalProperties": False,
               "required": ["spec_version", "connection_type", "node", "chord", "gusset", "members"],
               "properties": {k: {} for k in ["spec_version", "connection_type", "source", "node", "chord", "gusset",
                                               "members", "dimension_chains", "uncertain_fields"]}}
    return _sobre("schema", True, {"connection_type": "gusset_node", "description": "Nudo de cercha con cartela.",
                                   "json_schema": esquema, "example": ejemplo})


def _miembro_json(i, m):
    return {"element_id": i, "family": m["family"], "type": m["type"], "structural_type": "Beam",
            "start_mm": m["start"], "end_mm": m["end"], "length_mm": 1000.0, "slope_deg": 0.0,
            "angle_in_plane_deg": m["angle"], "node_end": 1, "material": "Steel", "is_chord": m["chord"]}


def _op_node_info(req):
    ids = req.get("element_ids")
    if ids is None:
        ids = sorted(MIEMBROS.keys())  # "selección actual" simulada
    if len(ids) < 2:
        return _sobre("node_info", False, errors=[_err("INVALID_REQUEST",
                      "Se requieren al menos 2 miembros de armazón estructural para inspeccionar el nudo (recibidos: {}).".format(len(ids)),
                      "element_ids", "Selecciona los miembros del nudo en Revit o pasa sus IDs en 'element_ids'.")])
    for i in ids:
        if i not in MIEMBROS:
            return _sobre("node_info", False, errors=[_err("ELEMENT_NOT_FOUND",
                          "No existe ningún elemento con ID {}.".format(i), "element_ids")])
    existentes = [{"connection_id": c["connection_id"], "connection_type": c["connection_type"],
                   "created_utc": c["created_utc"], "created_elements_count": len(c["created_element_ids"])}
                  for c in CONEXIONES.values() if set(c["spec"]["node"]["element_ids"]) & set(ids)]
    return _sobre("node_info", True, {
        "origin_mm": ORIGEN_MM, "x_axis": [-1, 0, 0], "y_axis": [0, 0, 1], "z_axis": [0, 1, 0],
        "axis_distance_mm": 0.08, "chord_element_id": 1249510,
        "members": [_miembro_json(i, MIEMBROS[i]) for i in ids], "existing_connections": existentes})


def _op_find_profile(req):
    consulta = req.get("query") or req.get("name") or req.get("profile") or ""
    normalizada = _normalizar_perfil(consulta)
    coincidencias = [{"type_name": p, "family_name": p, "exact_match": p.upper() == consulta.upper()}
                     for p in PERFILES_MODELO if normalizada and _normalizar_perfil(p).startswith(normalizada)]
    sugerencias = [] if coincidencias or not consulta else PERFILES_MODELO[:3]
    return _sobre("find_profile", True, {"query": consulta, "total_profiles_in_model": len(PERFILES_MODELO),
                                         "matched_count": len(coincidencias), "matches": coincidencias,
                                         "suggestions": sugerencias})


def _validar(spec):
    errores, avisos = [], []
    for clave in ("spec_version", "connection_type", "node", "chord", "gusset", "members"):
        if clave not in spec:
            errores.append(_err("SCHEMA_INVALID", "Falta el campo obligatorio '{}'.".format(clave), clave))
    if spec.get("connection_type") not in (None, "gusset_node"):
        errores.append(_err("SCHEMA_INVALID", "connection_type desconocido.", "connection_type"))
    for n, cadena in enumerate(spec.get("dimension_chains") or []):
        suma = sum(cadena.get("values_mm") or [])
        esperado = cadena.get("expected_total_mm") or 0
        if abs(suma - esperado) > 1.0:
            errores.append(_err("DIMENSION_CHAIN_MISMATCH",
                                "La cadena '{}' suma {} mm y debería dar {} mm.".format(cadena.get("label"), suma, esperado),
                                "dimension_chains[{}].values_mm".format(n), "Vuelve a leer las cotas del plano."))
    for n, duda in enumerate(spec.get("uncertain_fields") or []):
        if duda.get("user_confirmed_value") in (None, ""):
            errores.append(_err("UNRESOLVED_UNCERTAINTY",
                                "La duda en '{}' no tiene user_confirmed_value.".format(duda.get("path")),
                                "uncertain_fields[{}].user_confirmed_value".format(n), "Pregunta al usuario y confirma el valor."))
    for i in (spec.get("node") or {}).get("element_ids") or []:
        if i not in MIEMBROS:
            errores.append(_err("ELEMENT_NOT_FOUND", "No existe ningún elemento con ID {}.".format(i), "node.element_ids"))
    for n, miembro in enumerate(spec.get("members") or []):
        pernos = ((miembro.get("attachment") or {}).get("bolts")) or {}
        if pernos.get("edge_mm") is not None and pernos["edge_mm"] < 22.0:
            errores.append(_err("BOLT_EDGE_DISTANCE_TOO_SMALL",
                                "La distancia al borde ({} mm) es menor que el mínimo configurado (22 mm).".format(pernos["edge_mm"]),
                                "members[{}].attachment.bolts.edge_mm".format(n), "Usa edge_mm >= 22."))
        modelo = MIEMBROS.get(miembro.get("element_id"))
        esperado = miembro.get("expected_angle_deg")
        if modelo and esperado is not None and abs(modelo["angle"] - esperado) > 1.0:
            avisos.append(_err("ANGLE_DIFFERS_FROM_MODEL",
                               "El ángulo del plano ({:.1f}°) difiere del ángulo en el modelo ({:.1f}°) por {:.1f}° > 1°.".format(
                                   esperado, modelo["angle"], abs(modelo["angle"] - esperado)),
                               "members[{}].expected_angle_deg".format(n)))
    return errores, avisos


def _op_validate(req):
    spec = _spec_de(req)
    if spec is None:
        return _sobre("validate", False, errors=[_err("SCHEMA_INVALID", "No se pudo deserializar la especificación.", "")])
    errores, avisos = _validar(spec)
    if errores:
        return _sobre("validate", False, errors=errores, warnings=avisos)
    return _sobre("validate", True, {
        "is_valid": True, "validation_token": _token_de(spec), "errors_count": 0, "warnings_count": len(avisos),
        "calculated_values": {"origin_mm": ORIGEN_MM, "axis_distance_mm": 0.08, "frame_x": [-1, 0, 0],
                              "frame_y": [0, 0, 1], "frame_z": [0, 1, 0]}}, warnings=avisos)


def _resumen_preview(spec):
    pernos = cuchillas = soldaduras = 0
    modificar = []
    for m in spec.get("members") or []:
        a = m.get("attachment") or {}
        if a.get("type") == "bolted_knife_plate":
            cuchillas += 1
            b = a.get("bolts") or {}
            pernos += (b.get("rows") or 1) * (b.get("columns") or 1)
            soldaduras += 2
        elif a.get("type") == "welded_slot":
            soldaduras += 2
        if (m.get("end_setback_mm") or 0) > 0:
            modificar.append({"element_id": m.get("element_id"), "role": m.get("role"), "end": "end",
                              "current_end_distance_mm": 86.2, "setback_mm": m["end_setback_mm"],
                              "new_extension_mm": round(86.2 - m["end_setback_mm"], 1),
                              "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"})
    return pernos, cuchillas, soldaduras, modificar


def _op_preview(req):
    spec = _spec_de(req)
    if spec is None:
        return _sobre("preview", False, errors=[_err("SCHEMA_INVALID", "Especificación nula.")])
    pernos, cuchillas, soldaduras, modificar = _resumen_preview(spec)
    return _sobre("preview", True, {
        "summary": {"connection_type": spec.get("connection_type"), "backend": "advancesteel",
                    "chord_element_id": (spec.get("chord") or {}).get("element_id"),
                    "first_member_element_id": ((spec.get("members") or [{}])[0]).get("element_id"),
                    "working_point_mm": ORIGEN_MM, "gusset_plates": 1, "knife_plates": cuchillas, "bolts": pernos,
                    "weld_lines": soldaduras, "members_modified": len(modificar), "dry_run": True},
        "elements_to_create": [{"kind": "gusset_plate"}], "members_to_modify": modificar})


def _op_create(req, connection_id=None):
    nombre = "update" if connection_id else "create"
    token = (req.get("validation_token") or "").strip()
    if not token:
        return _sobre(nombre, False, errors=[_err("VALIDATION_TOKEN_INVALID",
                      "validation_token es obligatorio para crear una conexión.", "validation_token",
                      "Llama primero a conn_validate para validar la especificación y obtener el token.")])
    spec = _spec_de({k: v for k, v in req.items() if k != "validation_token"})
    if spec is None:
        return _sobre(nombre, False, errors=[_err("SCHEMA_INVALID", "Especificación nula.")])
    if token.lower() != _token_de(spec):
        return _sobre(nombre, False, errors=[_err("VALIDATION_TOKEN_INVALID",
                      "El validation_token no es válido o el modelo ha cambiado desde que se realizó la validación.",
                      "validation_token", "Vuelve a llamar a conn_validate.")])
    cid = connection_id or str(uuid.uuid4())
    ids = [1321345 + len(CONEXIONES) * 10 + n for n in range(9)]
    CONEXIONES[cid] = {"connection_id": cid, "spec_version": spec.get("spec_version", "1.0"),
                       "connection_type": spec.get("connection_type", "gusset_node"), "created_element_ids": ids,
                       "created_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "backend": "advancesteel",
                       "spec": spec}
    avisos = [_err("REVIT_WARNING", "Advertencia de Revit: The created elements are only visible in Detail Level: Fine.")]
    datos = {"connection_id": cid, "spec_version": CONEXIONES[cid]["spec_version"],
             "connection_type": CONEXIONES[cid]["connection_type"], "created_element_ids": ids,
             "created_elements_count": len(ids)}
    if connection_id:
        datos["updated_utc"] = CONEXIONES[cid]["created_utc"]
    else:
        datos["created_utc"] = CONEXIONES[cid]["created_utc"]
        datos["backend"] = "advancesteel"
    return _sobre(nombre, True, datos, warnings=avisos)


def _op_list(req):
    items = [{k: c[k] for k in ("connection_id", "spec_version", "connection_type", "backend", "created_utc")}
             | {"created_elements_count": len(c["created_element_ids"])} for c in CONEXIONES.values()]
    return _sobre("list", True, {"connections_count": len(items), "connections": items})


def _no_encontrada(nombre, cid):
    return _sobre(nombre, False, errors=[_err("ELEMENT_NOT_FOUND",
                  "No se encontró ninguna conexión con ID '{}'.".format(cid), "connection_id",
                  "Usa conn_list para verificar las conexiones guardadas en el modelo.")])


def _cid(req, nombre):
    cid = (req.get("connection_id") or "").strip()
    if not cid:
        return None, _sobre(nombre, False, errors=[_err("INVALID_REQUEST", "connection_id es requerido.", "connection_id")])
    return cid, None


def _op_get(req):
    cid, error = _cid(req, "get")
    if error:
        return error
    c = CONEXIONES.get(cid)
    if not c:
        return _no_encontrada("get", cid)
    return _sobre("get", True, {k: c[k] for k in ("connection_id", "spec_version", "connection_type", "created_utc",
                                                  "created_element_ids", "backend", "spec")}
                  | {"created_elements_count": len(c["created_element_ids"])})


def _op_update(req):
    cid, error = _cid(req, "update")
    if error:
        return error
    if cid not in CONEXIONES:
        return _no_encontrada("update", cid)
    return _op_create({k: v for k, v in req.items() if k != "connection_id"}, connection_id=cid)


def _op_delete(req):
    cid, error = _cid(req, "delete")
    if error:
        return error
    c = CONEXIONES.pop(cid, None)
    if not c:
        return _no_encontrada("delete", cid)
    return _sobre("delete", True, {"deleted_connection_id": cid, "deleted_elements_count": len(c["created_element_ids"]),
                                   "restored_members_count": 3})


OPERACIONES = {
    "ping": (_op_ping, False), "guide": (_op_guide, False), "types": (_op_types, False), "schema": (_op_schema, False),
    "node_info": (_op_node_info, True), "find_profile": (_op_find_profile, True), "validate": (_op_validate, True),
    "preview": (_op_preview, True), "create": (_op_create, True), "list": (_op_list, True), "get": (_op_get, True),
    "update": (_op_update, True), "delete": (_op_delete, True),
}


def bridge_handle(operation, request_json, doc, uidoc):
    """Imitación de MotorConexiones.Revit.Bridge.Handle: devuelve siempre el sobre común serializado."""
    nombre = operation or ""
    try:
        req = json.loads(request_json) if (request_json or "").strip() else {}
    except ValueError as error:
        return json.dumps(_sobre(nombre, False, errors=[_err("INVALID_REQUEST",
                          "La petición no es JSON válido: {}".format(error), None,
                          "Envía un objeto JSON, por ejemplo {\"element_ids\": [111, 222]}.")]), ensure_ascii=False)
    if not isinstance(req, dict):
        return json.dumps(_sobre(nombre, False, errors=[_err("INVALID_REQUEST", "La petición debe ser un objeto JSON.")]),
                          ensure_ascii=False)
    LLAMADAS.append((nombre, req))
    if nombre not in OPERACIONES:
        return json.dumps(_sobre(nombre, False, errors=[_err("UNKNOWN_OPERATION",
                          "La operación '{}' no existe en el add-in.".format(nombre), None,
                          "Operaciones disponibles: " + ", ".join(sorted(OPERACIONES)) + ".")]), ensure_ascii=False)
    funcion, necesita_doc = OPERACIONES[nombre]
    if necesita_doc and doc is None:
        return json.dumps(_sobre(nombre, False, errors=[_err("NO_DOCUMENT", "No hay ningún documento abierto en Revit.",
                          None, "Abre el modelo y vuelve a intentarlo.")]), ensure_ascii=False)
    return json.dumps(funcion(req), ensure_ascii=False)


# ---------------------------------------------------------------------------
# pyRevit, clr y System simulados
# ---------------------------------------------------------------------------
class _Respuesta:
    def __init__(self, data=None, status=200):
        self.data = data
        self.status = status


class _Peticion:
    def __init__(self, metodo, ruta, data, query_params):
        self.method = metodo
        self.path = ruta
        self.data = data
        self.params = data
        self.query_params = query_params
        self._headers = {}


class _API:
    """Imita pyrevit.routes.API: guarda (patrón, métodos, manejador, nombres de parámetros de ruta)."""

    def __init__(self, nombre):
        self.nombre = nombre
        self.rutas = []

    def route(self, patron, methods=("GET",)):
        def decorador(funcion):
            expresion = "^" + re.sub(r"<(\w+)>", r"(?P<\1>[^/]+)", patron).rstrip("/") + "/?$"
            self.rutas.append((patron, [m.upper() for m in methods], re.compile(expresion), funcion))
            return funcion
        return decorador

    def despachar(self, metodo, ruta, data, query_params, doc, uidoc, uiapp):
        """Como pyRevit: elige la ruta, filtra los argumentos por nombre y llama al manejador."""
        for patron, metodos, expresion, funcion in self.rutas:
            m = expresion.match(ruta)
            if not m:
                continue
            if metodo.upper() not in metodos:
                return _Respuesta({"error": "method not allowed"}, 405)
            peticion = _Peticion(metodo, ruta, data, query_params)
            disponibles = {"doc": doc, "uidoc": uidoc, "uiapp": uiapp, "request": peticion}
            disponibles.update({k: unquote(v) for k, v in m.groupdict().items()})
            nombres = inspect.getfullargspec(funcion).args
            kwargs = {n: disponibles[n] for n in nombres if n in disponibles}
            resultado = funcion(**kwargs)
            if isinstance(resultado, _Respuesta):
                return resultado
            return _Respuesta(resultado, 200)
        return _Respuesta({"error": "Route not found: {} {}".format(metodo, ruta)}, 404)


class _Ensamblado:
    class _Nombre:
        Name = "MotorConexiones.Revit"

    Location = "simulador://MotorConexiones.Revit.dll"

    def GetName(self):
        return self._Nombre()

    def GetType(self, nombre):
        return _Tipo() if nombre == "MotorConexiones.Revit.Bridge" else None


class _Tipo:
    def GetMethod(self, nombre, firma):
        return _Metodo() if nombre == "Handle" and len(firma) == 4 else None


class _Metodo:
    def Invoke(self, instancia, argumentos):
        return bridge_handle(argumentos[0], argumentos[1], argumentos[2], argumentos[3])


class _Documento:
    Title = "HANGAR_PRUEBA_sondeo"
    PathName = r"D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt"
    IsModifiable = False


def _instalar_modulos_falsos(extension):
    """Registra pyrevit, seguridad (real si hay extensión), clr, System y StringIO en sys.modules."""
    pyrevit = types.ModuleType("pyrevit")
    routes = types.ModuleType("pyrevit.routes")
    routes.API = _API
    routes.make_response = lambda data=None, status=200: _Respuesta(data, status)
    pyrevit.routes = routes
    pyrevit.revit = types.SimpleNamespace(uidoc=None, doc=None)
    db = types.ModuleType("pyrevit.DB")
    for nombre in ("Document", "Transaction", "TransactionGroup", "ElementId", "BuiltInParameter"):
        setattr(db, nombre, type(nombre, (), {}))
    ui = types.ModuleType("pyrevit.UI")
    ui.UIDocument = type("UIDocument", (), {})
    pyrevit.DB = db
    pyrevit.UI = ui
    sys.modules["pyrevit"] = pyrevit
    sys.modules["pyrevit.routes"] = routes
    sys.modules["pyrevit.DB"] = db
    sys.modules["pyrevit.UI"] = ui

    clr = types.ModuleType("clr")
    clr.AddReferenceToFileAndPath = lambda ruta: None
    clr.AddReference = lambda nombre: None
    clr.GetClrType = lambda t: t
    sys.modules["clr"] = clr

    system = types.ModuleType("System")

    class _Array:
        def __class_getitem__(cls, tipo):
            return lambda valores: list(valores)
    system.Array = _Array
    system.String = str
    system.Object = object
    system.Type = type
    system.Int64 = int
    system.AppDomain = types.SimpleNamespace(CurrentDomain=types.SimpleNamespace(
        GetAssemblies=lambda: ([] if OPCIONES.sin_addin else [_Ensamblado()])))
    sys.modules["System"] = system

    stringio = types.ModuleType("StringIO")
    stringio.StringIO = io.StringIO
    sys.modules["StringIO"] = stringio

    ruta_seguridad = Path(extension) / "revit_mcp" / "seguridad.py" if extension else None
    if ruta_seguridad and ruta_seguridad.is_file():
        import importlib.util
        espec = importlib.util.spec_from_file_location("seguridad", str(ruta_seguridad))
        modulo = importlib.util.module_from_spec(espec)
        espec.loader.exec_module(modulo)
        sys.modules["seguridad"] = modulo
        return "seguridad.py real de " + str(ruta_seguridad)
    # Imitación mínima del decorador (misma semántica: token en data o en query_params; 401 si no coincide)
    seguridad = types.ModuleType("seguridad")
    seguridad._token = None
    seguridad.establecer_token = lambda t: setattr(seguridad, "_token", t)
    seguridad.token_actual = lambda: seguridad._token

    def requiere_token(funcion):
        nombres = list(inspect.getfullargspec(funcion).args)
        firma = nombres + ([] if "request" in nombres else ["request"])

        def envoltura(**kwargs):
            request = kwargs["request"]
            recibido = None
            if isinstance(request.data, dict) and "token" in request.data:
                recibido = request.data.pop("token")
            elif isinstance(request.query_params, dict):
                recibido = request.query_params.get("token")
            if not recibido or recibido != seguridad._token:
                return _Respuesta({"error": "token ausente o incorrecto"}, 401)
            return funcion(**{n: kwargs[n] for n in nombres})
        envoltura.__name__ = funcion.__name__
        envoltura.__doc__ = funcion.__doc__
        envoltura.__wrapped__ = funcion
        envoltura.__signature__ = inspect.Signature([inspect.Parameter(n, inspect.Parameter.POSITIONAL_OR_KEYWORD) for n in firma])
        # getfullargspec lee __signature__ en CPython 3; así el despachador ve "request" aunque el manejador no lo declare.
        return envoltura
    seguridad.requiere_token = requiere_token
    sys.modules["seguridad"] = seguridad
    return "seguridad simulada (no se indicó --extension)"


def cargar_conexiones(ruta, extension):
    """Importa conexiones.py con los módulos simulados y devuelve (módulo, API con las rutas registradas, origen seguridad)."""
    origen = _instalar_modulos_falsos(extension)
    import importlib.util
    espec = importlib.util.spec_from_file_location("conexiones", str(ruta))
    modulo = importlib.util.module_from_spec(espec)
    sys.modules["conexiones"] = modulo
    espec.loader.exec_module(modulo)
    api = _API("revit_mcp")
    modulo.register_conn_routes(api)
    return modulo, api, origen


# ---------------------------------------------------------------------------
# Autocomprobación en proceso
# ---------------------------------------------------------------------------
RUTAS_ESPERADAS = [
    ("GET", "/conn/ping/", "ping", {}),
    ("GET", "/conn/guide/", "guide", {}),
    ("GET", "/conn/types/", "types", {}),
    ("GET", "/conn/schema/gusset_node", "schema", {"type": "gusset_node"}),
    ("POST", "/conn/node_info/", "node_info", {"element_ids": [1249510, 1249630]}),
    ("POST", "/conn/find_profile/", "find_profile", {"query": "HSS3X3X1/4"}),
    ("POST", "/conn/validate/", "validate", {"spec": {"a": 1}}),
    ("POST", "/conn/preview/", "preview", {"spec": {"a": 1}}),
    ("POST", "/conn/create/", "create", {"spec": {"a": 1}, "validation_token": "x"}),
    ("GET", "/conn/list/", "list", {}),
    ("GET", "/conn/get/abc-123", "get", {"connection_id": "abc-123"}),
    ("POST", "/conn/update/", "update", {"connection_id": "abc", "spec": {"a": 1}, "validation_token": "x"}),
    ("POST", "/conn/delete/", "delete", {"connection_id": "abc"}),
    ("POST", "/conn/op/no_existe/", "no_existe", {"k": 1}),
]


def autocomprobar(api, origen_seguridad, token):
    import seguridad
    seguridad.establecer_token(token)
    resultados = []

    def comprobar(nombre, condicion, detalle=""):
        resultados.append(bool(condicion))
        print("  [{}] {}{}".format("OK" if condicion else "FALLO", nombre, "  " + detalle if detalle else ""))

    print("Autocomprobación de conexiones.py ({})".format(origen_seguridad))
    patrones = [r[0] for r in api.rutas]
    comprobar("15 rutas registradas", len(api.rutas) == 15, "registradas: {}".format(len(api.rutas)))
    for metodo, ruta, operacion, cuerpo in RUTAS_ESPERADAS:
        del LLAMADAS[:]
        datos = dict(cuerpo)
        consulta = {}
        if metodo == "POST":
            datos["token"] = token
        else:
            consulta["token"] = token
        r = api.despachar(metodo, ruta, datos, consulta, _Documento(), object(), object())
        llego = LLAMADAS[-1] if LLAMADAS else (None, None)
        esperado = {k: v for k, v in cuerpo.items()} if metodo == "POST" else cuerpo
        comprobar("{} {} -> Bridge.Handle('{}')".format(metodo, ruta, operacion),
                  r.status == 200 and isinstance(r.data, dict) and "ok" in r.data and llego[0] == operacion
                  and llego[1] == esperado,
                  "HTTP {} op={} cuerpo={}".format(r.status, llego[0], json.dumps(llego[1], ensure_ascii=False)))
    # Sin token -> 401 en GET y en POST
    r = api.despachar("GET", "/conn/ping/", {}, {}, _Documento(), None, None)
    comprobar("GET /conn/ping/ sin token -> 401", r.status == 401, "HTTP {}".format(r.status))
    r = api.despachar("POST", "/conn/validate/", {"spec": {}}, {}, _Documento(), None, None)
    comprobar("POST /conn/validate/ sin token -> 401", r.status == 401, "HTTP {}".format(r.status))
    # Token incorrecto
    r = api.despachar("GET", "/conn/list/", {}, {"token": "0" * 64}, _Documento(), None, None)
    comprobar("GET /conn/list/ token incorrecto -> 401", r.status == 401, "HTTP {}".format(r.status))
    # dev_exec
    r = api.despachar("POST", "/conn/dev_exec/", {"token": token, "code": 'print("hola")', "description": "prueba"}, {},
                      _Documento(), object(), object())
    comprobar("POST /conn/dev_exec/ print('hola')", r.status == 200 and r.data.get("ok") is True
              and r.data["data"]["output"].strip() == "hola", json.dumps(r.data.get("data"), ensure_ascii=False)[:120])
    r = api.despachar("POST", "/conn/dev_exec/", {"token": token, "code": "raise ValueError('x')"}, {},
                      _Documento(), object(), object())
    comprobar("POST /conn/dev_exec/ excepción -> PROBE_EXCEPTION", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "PROBE_EXCEPTION")
    # Ruta inexistente -> 404
    r = api.despachar("GET", "/conn/nada/", {}, {"token": token}, _Documento(), None, None)
    comprobar("GET /conn/nada/ -> 404", r.status == 404, "HTTP {}".format(r.status))
    # Sin add-in -> ADDIN_NOT_LOADED (200)
    OPCIONES.sin_addin = True
    import conexiones as modulo
    modulo._metodo_handle = None
    r = api.despachar("GET", "/conn/ping/", {}, {"token": token}, _Documento(), None, None)
    comprobar("GET /conn/ping/ sin add-in -> 200 ADDIN_NOT_LOADED", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "ADDIN_NOT_LOADED", json.dumps(r.data.get("errors"), ensure_ascii=False)[:160])
    OPCIONES.sin_addin = False
    modulo._metodo_handle = None
    # Sin documento -> NO_DOCUMENT en validate, pero ping y guide responden ok
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": {}}, {}, None, None, None)
    comprobar("POST /conn/validate/ sin documento -> NO_DOCUMENT", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "NO_DOCUMENT")
    r = api.despachar("GET", "/conn/guide/", {}, {"token": token}, None, None, None)
    comprobar("GET /conn/guide/ sin documento -> ok:true", r.status == 200 and r.data.get("ok") is True)
    print("Autocomprobación: {}/{} correctas".format(sum(resultados), len(resultados)))
    return all(resultados)


# ---------------------------------------------------------------------------
# Servidor HTTP (imita pyRevit Routes en 48884)
# ---------------------------------------------------------------------------
def servir(api, puerto, token, prefijo="/revit_mcp"):
    import seguridad
    seguridad.establecer_token(token)
    doc = None if OPCIONES.sin_documento else _Documento()

    class Manejador(BaseHTTPRequestHandler):
        def _responder(self, respuesta):
            cuerpo = json.dumps(respuesta.data).encode("utf-8")  # pyRevit serializa con ensure_ascii=True
            self.send_response(respuesta.status)
            self.send_header("Content-Type", "application/json")
            self.send_header("Content-Length", str(len(cuerpo)))
            self.end_headers()
            self.wfile.write(cuerpo)

        def _atender(self, metodo):
            partes = urlsplit(self.path)
            ruta = partes.path
            if not ruta.startswith(prefijo + "/"):
                return self._responder(_Respuesta({"error": "not found"}, 404))
            ruta = ruta[len(prefijo):]
            consulta = {k: v[0] for k, v in parse_qs(partes.query).items()}
            datos = {}
            longitud = int(self.headers.get("Content-Length") or 0)
            if longitud:
                crudo = self.rfile.read(longitud).decode("utf-8")
                try:
                    datos = json.loads(crudo)
                except ValueError:
                    datos = crudo
            with CERROJO:
                respuesta = api.despachar(metodo, ruta, datos, consulta, doc, object() if doc else None, object())
            self._responder(respuesta)

        def do_GET(self):
            self._atender("GET")

        def do_POST(self):
            self._atender("POST")

        def log_message(self, formato, *args):
            sys.stderr.write("[simulador] " + (formato % args) + "\n")

    servidor = ThreadingHTTPServer(("127.0.0.1", puerto), Manejador)
    print("Simulador de Revit escuchando en http://127.0.0.1:{}{}/conn/... (token {}...)".format(puerto, prefijo, token[:8]))
    sys.stdout.flush()
    try:
        servidor.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        servidor.server_close()


CERROJO = threading.Lock()


def main():
    parser = argparse.ArgumentParser(description="Simulador de Revit + pyRevit Routes para MotorConexiones")
    parser.add_argument("--autocomprobar", action="store_true", help="comprobar conexiones.py en proceso y salir")
    parser.add_argument("--puerto", type=int, default=48884)
    parser.add_argument("--token", default=None, help="token de sesión (por defecto se genera uno de 64 hex)")
    parser.add_argument("--token-archivo", default=NOMBRE_TOKEN_LITERAL,
                        help="dónde escribir el token (por defecto el nombre literal de Windows en la carpeta actual)")
    parser.add_argument("--extension", default=os.environ.get("REVIT_MCP_EXTENSION"),
                        help="carpeta de revit-mcp para usar su revit_mcp/seguridad.py real")
    parser.add_argument("--conexiones", default=str(RUTA_CONEXIONES), help="ruta de conexiones.py a cargar")
    parser.add_argument("--sin-addin", action="store_true")
    parser.add_argument("--sin-documento", action="store_true")
    args = parser.parse_args()

    OPCIONES.sin_addin = args.sin_addin
    OPCIONES.sin_documento = args.sin_documento
    token = args.token or uuid.uuid4().hex + uuid.uuid4().hex
    modulo, api, origen = cargar_conexiones(args.conexiones, args.extension)

    if args.autocomprobar:
        return 0 if autocomprobar(api, origen, token) else 1

    ruta_token = Path(args.token_archivo)
    if ruta_token.parent != Path("."):
        ruta_token.parent.mkdir(parents=True, exist_ok=True)
    ruta_token.write_text(token, encoding="utf-8")
    print("conexiones.py: {} ({} rutas; {})".format(args.conexiones, len(api.rutas), origen))
    print("token escrito en {}".format(ruta_token))
    servir(api, args.puerto, token)
    return 0


if __name__ == "__main__":
    sys.exit(main())
