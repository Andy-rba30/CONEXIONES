# -*- coding: utf-8 -*-
# Sondeo 12: borra TODAS las conexiones creadas por MotorConexiones en el documento (conn_list + conn_delete) y
# comprueba que los miembros recuperan su extension original. Solo sobre la copia "_sondeo.rvt".
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900
from __future__ import print_function
import json
import time


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


print("=== 12-fase3-borrar ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt'.")
    raise SystemExit
ensamblado = buscar_ensamblado("MotorConexiones.Revit")
if ensamblado is None:
    print("PARADA: MotorConexiones.Revit no esta cargado.")
    raise SystemExit
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UI.UIDocument)])
handle = ensamblado.GetType("MotorConexiones.Revit.Bridge").GetMethod("Handle", firma)


def llamar(operacion, cuerpo):
    inicio = time.time()
    texto = handle.Invoke(None, System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc]))
    respuesta = json.loads(texto)
    print("--- {0}: ok={1} en {2} ms | errores={3} | avisos={4}".format(
        operacion, respuesta.get("ok"), int((time.time() - inicio) * 1000),
        ", ".join(str(e.get("code")) for e in respuesta.get("errors") or []) or "-",
        ", ".join(str(w.get("code")) for w in respuesta.get("warnings") or []) or "-"))
    for e in respuesta.get("errors") or []:
        print("    ERROR {0}: {1}".format(e.get("code"), e.get("message")))
    for w in (respuesta.get("warnings") or [])[:12]:
        print("    aviso {0}: {1}".format(w.get("code"), str(w.get("message"))[:300]))
    return respuesta


r = llamar("list", {})
conexiones = (r.get("data") or {}).get("connections") or []
print("    conexiones en el modelo: {0}".format(len(conexiones)))
for c in conexiones:
    cid = c.get("connection_id")
    g = llamar("get", {"connection_id": cid})
    miembros = []
    try:
        miembros = (((g.get("data") or {}).get("spec") or {}).get("members") or [])
    except Exception:
        pass
    antes = {}
    for m in miembros:
        el = doc.GetElement(DB.ElementId(System.Int64(int(m.get("element_id")))))
        if el is not None:
            p0 = el.get_Parameter(DB.BuiltInParameter.START_EXTENSION)
            p1 = el.get_Parameter(DB.BuiltInParameter.END_EXTENSION)
            antes[m.get("element_id")] = (p0.AsDouble() * 304.8 if p0 else None, p1.AsDouble() * 304.8 if p1 else None)
    d = llamar("delete", {"connection_id": cid})
    print("    " + json.dumps(d.get("data"), ensure_ascii=False))
    for eid, (s0, s1) in antes.items():
        el = doc.GetElement(DB.ElementId(System.Int64(int(eid))))
        p0 = el.get_Parameter(DB.BuiltInParameter.START_EXTENSION)
        p1 = el.get_Parameter(DB.BuiltInParameter.END_EXTENSION)
        print("    miembro {0}: extension inicio {1} -> {2} mm | fin {3} -> {4} mm".format(
            eid, s0, p0.AsDouble() * 304.8 if p0 else None, s1, p1.AsDouble() * 304.8 if p1 else None))
r = llamar("list", {})
print("    conexiones tras borrar: {0}".format((r.get("data") or {}).get("connections_count")))
print("=== fin 12-fase3-borrar ===")
