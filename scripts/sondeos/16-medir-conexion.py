# -*- coding: utf-8 -*-
# Sondeo 16 (Fase 6b): MIDE la conexion que ya existe en el modelo (creada desde la ventana o desde la IA) sin crear
# ni borrar nada: para cada conexion de conn_list imprime sus elementos y, en los SteelProxyElement, los parametros que
# muestra Revit (Thickness, Length, Width, Diameter, Bolt Length, Grip Length, ...), con el valor en mm. Es la prueba
# objetiva de los pernos de la Fase 6b: Bolt Length 44.45 mm (1-3/4") y Grip Length 19.5 mm (cartela 9.5 + cuchilla 10).
# Solo sobre la copia "_sondeo.rvt". Se ejecuta SIN transaccion envolvente:
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\16-medir-conexion.py -SinTransaccion
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import json
import time

MM_POR_PIE = 304.8
NOMBRES_MEDIDAS = ("Thickness", "Length", "Width", "Diameter", "Bolt Length", "Grip Length", "Length on side 1",
                   "Length on side 2", "Intermediate distance on side 1", "Intermediate distance on side 2",
                   "Number on side 1", "Number on side 2", "Standard", "Grade")


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def medidas_acero(elemento):
    """Parametros de un SteelProxyElement con el texto que muestra Revit y, si es longitud, el valor en mm."""
    lineas = []
    try:
        parametros = list(elemento.Parameters)
    except Exception as error:
        return ["parametros: ERROR " + str(error)[:120]]
    for nombre in NOMBRES_MEDIDAS:
        for p in parametros:
            try:
                if p.Definition.Name != nombre:
                    continue
                texto = p.AsValueString()
                if p.StorageType == DB.StorageType.Double:
                    lineas.append("{0}: {1} = {2} mm".format(nombre, texto, round(p.AsDouble() * MM_POR_PIE, 2)))
                elif p.StorageType == DB.StorageType.Integer:
                    lineas.append("{0}: {1}".format(nombre, p.AsInteger()))
                else:
                    lineas.append("{0}: {1}".format(nombre, texto if texto else p.AsString()))
            except Exception as error:
                lineas.append("{0}: ERROR {1}".format(nombre, str(error)[:80]))
            break
    return lineas or ["(sin parametros con esos nombres; nombres presentes: {0})".format(
        ", ".join(sorted(set(p.Definition.Name for p in parametros))[:40]))]


def caja_mm(elemento):
    """Caja envolvente en mm (los SteelProxyElement suelen devolver None: se dice)."""
    try:
        caja = elemento.get_BoundingBox(None)
        if caja is None:
            return "sin caja"
        return "caja min ({0:.1f}, {1:.1f}, {2:.1f}) max ({3:.1f}, {4:.1f}, {5:.1f}) mm".format(
            caja.Min.X * MM_POR_PIE, caja.Min.Y * MM_POR_PIE, caja.Min.Z * MM_POR_PIE,
            caja.Max.X * MM_POR_PIE, caja.Max.Y * MM_POR_PIE, caja.Max.Z * MM_POR_PIE)
    except Exception as error:
        return "caja: ERROR " + str(error)[:80]


print("=== 16-medir-conexion ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
ensamblado = buscar_ensamblado("MotorConexiones.Revit")
if ensamblado is None:
    print("PARADA: MotorConexiones.Revit no esta cargado.")
    raise SystemExit
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UI.UIDocument)])
handle = ensamblado.GetType("MotorConexiones.Revit.Bridge").GetMethod("Handle", firma)
print("Bridge.Handle encontrado: {0} | version {1}".format(handle is not None, ensamblado.GetName().Version))


def llamar(operacion, cuerpo):
    inicio = time.time()
    texto = handle.Invoke(None, System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc]))
    respuesta = json.loads(texto)
    print("--- {0}: ok={1} en {2} ms | errores={3}".format(
        operacion, respuesta.get("ok"), int((time.time() - inicio) * 1000),
        ", ".join(str(e.get("code")) for e in respuesta.get("errors") or []) or "-"))
    return respuesta


r = llamar("list", {})
conexiones = (r.get("data") or {}).get("connections") or []
print("    conexiones en el modelo: {0}".format(len(conexiones)))
if not conexiones:
    print("    No hay nada que medir: crea la conexion (ventana o conn_create) y repite.")
for c in conexiones:
    cid = c.get("connection_id")
    g = llamar("get", {"connection_id": cid})
    d = g.get("data") or {}
    print("    conexion {0}: backend={1} | elementos={2} | creada={3}".format(cid, d.get("backend"), d.get("created_elements_count"), d.get("created_utc")))
    for eid in (d.get("created_element_ids") or [])[:30]:
        el = doc.GetElement(DB.ElementId(System.Int64(int(eid))))
        if el is None:
            print("    [{0}] no existe".format(eid))
            continue
        try:
            categoria = el.Category.Name if el.Category is not None else "-"
        except Exception:
            categoria = "?"
        print("    [{0}] {1} | {2} | {3}".format(eid, el.GetType().Name, categoria, caja_mm(el)))
        if el.GetType().Name == "SteelProxyElement":
            for linea in medidas_acero(el):
                print("        " + linea)
print("    ESPERADO (Detalle D, Fase 6b): cartela Thickness 9.5, Length/Width 565 x 530; placa cuchilla 10 x 170 x 140;")
print("    pernos Diameter 5/8\" (15.9 mm), Bolt Length 44.45 mm (1-3/4\"), Grip Length 19.5 mm (9.5 + 10), paso 60, 2 y 2.")
print("    Si Grip Length sigue en 80 mm, Connect no se aplico: mira 'connect' en advance_steel_bolts_written del registro.")
print("=== fin 16-medir-conexion ===")
