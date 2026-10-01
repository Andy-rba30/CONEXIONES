# -*- coding: utf-8 -*-
# Sondeo 11: Fase 3 de punta a punta por reflexion sobre Bridge.Handle, sin MCP: ping, guide, types, schema,
# find_profile, node_info, validate, preview, create, list y get con docs/fixtures/detalle-D-confirmado.json.
# DEJA LA CONEXION CREADA (para la captura); el sondeo 12 la borra. Solo sobre la copia "_sondeo.rvt".
# Se ejecuta SIN transaccion envolvente (cada operacion abre la suya):
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\11-fase3-crear.py -SinTransaccion -TimeoutSec 900
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import json
import os
import time

RUTA_FIXTURE = r"D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json"
NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def resumen(valor, maximo=600):
    texto = json.dumps(valor, ensure_ascii=False)
    return texto if len(texto) <= maximo else texto[:maximo] + " ..."


print("=== 11-fase3-crear ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit

ensamblado = buscar_ensamblado(NOMBRE_ENSAMBLADO)
if ensamblado is None:
    print("PARADA: MotorConexiones.Revit no esta cargado. Ejecuta scripts\\deploy.ps1 con Revit cerrado y vuelve a abrir Revit.")
    raise SystemExit
tipo = ensamblado.GetType("MotorConexiones.Revit.Bridge")
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UI.UIDocument)])
handle = tipo.GetMethod("Handle", firma)
print("Bridge.Handle encontrado: {0} | version del ensamblado: {1}".format(handle is not None, ensamblado.GetName().Version))


def llamar(operacion, cuerpo):
    inicio = time.time()
    argumentos = System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc])
    try:
        texto = handle.Invoke(None, argumentos)
        respuesta = json.loads(texto)
    except Exception as error:
        interna = getattr(error, "InnerException", None)
        print("--- {0}: EXCEPCION {1}: {2}".format(operacion, type(error).__name__, str(interna or error)[:400]))
        return {"ok": False, "data": None, "errors": [{"code": "EXCEPTION", "message": str(error)[:400]}], "warnings": []}
    ms = int((time.time() - inicio) * 1000)
    errores = respuesta.get("errors") or []
    avisos = respuesta.get("warnings") or []
    print("--- {0}: ok={1} en {2} ms | errores={3} | avisos={4}".format(
        operacion, respuesta.get("ok"), ms,
        ", ".join(str(e.get("code")) for e in errores) or "-",
        ", ".join(str(w.get("code")) for w in avisos) or "-"))
    for e in errores:
        print("    ERROR {0} [{1}]: {2}".format(e.get("code"), e.get("path"), e.get("message")))
        if e.get("hint"):
            print("          pista: " + str(e.get("hint")))
    for w in avisos[:12]:
        print("    aviso {0}: {1}".format(w.get("code"), str(w.get("message"))[:300]))
    return respuesta


# 1-4: operaciones sin modelo
r = llamar("ping", {})
d = r.get("data") or {}
print("    backend={0} | revit={1} | documento={2}".format(d.get("backend"), (d.get("revit") or {}).get("version_build"), (d.get("document") or {}).get("title")))
r = llamar("guide", {})
print("    guia: {0} caracteres".format(len((r.get("data") or {}).get("guide_markdown", ""))))
r = llamar("types", {})
print("    tipos: " + resumen((r.get("data") or {}).get("connection_types")))
r = llamar("schema", {"type": "gusset_node"})
print("    claves de data: " + ", ".join(sorted((r.get("data") or {}).keys())))

# 5-6: modelo
r = llamar("find_profile", {"query": "HSS2-1/2X2-1/2X3/16"})
print("    " + resumen(r.get("data"), 500))
if not os.path.isfile(RUTA_FIXTURE):
    print("PARADA: no existe el fixture " + RUTA_FIXTURE)
    raise SystemExit
with open(RUTA_FIXTURE, "r") as archivo:
    spec = json.load(archivo)
ids = spec.get("node", {}).get("element_ids", [])
r = llamar("node_info", {"element_ids": ids, "chord_element_id": spec.get("chord", {}).get("element_id")})
print("    " + resumen(r.get("data"), 900))

# 7-8: validar y previsualizar
r = llamar("validate", {"spec": spec})
token = (r.get("data") or {}).get("validation_token")
print("    is_valid={0} | token={1}".format((r.get("data") or {}).get("is_valid"), token))
print("    calculados: " + resumen((r.get("data") or {}).get("calculated_values"), 400))
r = llamar("preview", {"spec": spec})
print("    " + resumen((r.get("data") or {}).get("summary"), 600))
for elemento in (r.get("data") or {}).get("members_to_modify") or []:
    print("    retiro: " + resumen(elemento, 300))

# 9: crear (puede tardar varios minutos la primera vez: Advance Steel)
if not token:
    print("PARADA: sin validation_token no se crea nada. Revisa los errores de validate.")
    raise SystemExit
r = llamar("create", {"spec": spec, "validation_token": token})
datos = r.get("data") or {}
conexion = datos.get("connection_id")
print("    connection_id={0} | elementos creados={1} | ids={2}".format(conexion, datos.get("created_elements_count"), datos.get("created_element_ids")))

# 10-11: listar y leer
r = llamar("list", {})
print("    " + resumen(r.get("data"), 500))
if conexion:
    r = llamar("get", {"connection_id": conexion})
    d = r.get("data") or {}
    print("    backend={0} | elementos={1} | creado={2}".format(d.get("backend"), d.get("created_elements_count"), d.get("created_utc")))
    # Categorias de lo creado (para saber si fue Advance Steel o DirectShape)
    for eid in (d.get("created_element_ids") or [])[:20]:
        el = doc.GetElement(DB.ElementId(System.Int64(eid)))
        try:
            print("    [{0}] {1} | {2}".format(eid, el.GetType().Name, el.Category.Name if el is not None and el.Category is not None else "-"))
        except Exception:
            print("    [{0}] ?".format(eid))
    print("CONEXION CREADA: {0}. Haz la captura y despues ejecuta el sondeo 12 para borrarla.".format(conexion))
print("=== fin 11-fase3-crear ===")
