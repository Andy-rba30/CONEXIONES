# -*- coding: utf-8 -*-
# Sondeo 20 (Fase 9): comprueba en Revit que un TransactionGroup EXTERIOR puede envolver las operaciones del add-in
# (cada una abre su propio TransactionGroup: anidado) con la sesion de fabricacion de Advance Steel dentro, que es lo que
# hace BatchCreator para que el lote entero sea UNA SOLA entrada de deshacer (propuesta 4.1, decision P9; riesgo 8 de
# docs/propuestas/catalogo-y-lotes.md). Dos partes, las dos sobre el Detalle D (docs/fixtures/detalle-D-confirmado.json):
#   A) exterior.Start() -> validate -> create (grupo anidado del add-in) -> delete (otro anidado) -> exterior.Assimilate():
#      list = 0 y las extensiones de las barras como antes.
#   B) exterior.Start() -> validate -> create -> exterior.RollBack(): list = 0, cero placas y pernos de acero sueltos
#      (como el sondeo 13) y extensiones como antes.
# No deja nada en el modelo si las dos partes salen bien. Si A falla (el add-in no puede abrir su grupo dentro del
# exterior, o Assimilate falla), el instalador pone "batch_single_undo": false en config\catalog.json DESPLEGADO
# (%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json) antes de crear el lote: plan B, una entrada
# de deshacer por nudo. Solo sobre la copia "_sondeo.rvt". Se ejecuta SIN transaccion envolvente:
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\20-grupos-anidados.py -SinTransaccion -TimeoutSec 900
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import json
import os
import time

RUTA_FIXTURE = r"D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json"
NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"
MM_POR_PIE = 304.8


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def resumen(valor, maximo=400):
    texto = json.dumps(valor, ensure_ascii=False)
    return texto if len(texto) <= maximo else texto[:maximo] + " ..."


print("=== 20-grupos-anidados ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
ensamblado = buscar_ensamblado(NOMBRE_ENSAMBLADO)
if ensamblado is None:
    print("PARADA: MotorConexiones.Revit no esta cargado. Ejecuta scripts\\deploy.ps1 con Revit cerrado y vuelve a abrir Revit.")
    raise SystemExit
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UI.UIDocument)])
handle = ensamblado.GetType("MotorConexiones.Revit.Bridge").GetMethod("Handle", firma)
print("0) Bridge.Handle encontrado: {0} | version del ensamblado: {1}".format(handle is not None, ensamblado.GetName().Version))


def llamar(operacion, cuerpo):
    inicio = time.time()
    try:
        texto = handle.Invoke(None, System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc]))
        respuesta = json.loads(texto)
    except Exception as error:
        interna = getattr(error, "InnerException", None)
        print("--- {0}: EXCEPCION {1}: {2}".format(operacion, type(error).__name__, str(interna or error)[:400]))
        return {"ok": False, "data": None, "errors": [{"code": "EXCEPTION", "message": str(error)[:400]}], "warnings": []}
    print("--- {0}: ok={1} en {2} ms | errores={3} | avisos={4}".format(
        operacion, respuesta.get("ok"), int((time.time() - inicio) * 1000),
        ", ".join(str(e.get("code")) for e in respuesta.get("errors") or []) or "-",
        ", ".join(str(w.get("code")) for w in respuesta.get("warnings") or []) or "-"))
    for e in respuesta.get("errors") or []:
        print("    ERROR {0} [{1}]: {2}".format(e.get("code"), e.get("path"), e.get("message")))
    return respuesta


if not os.path.isfile(RUTA_FIXTURE):
    print("PARADA: no existe el fixture " + RUTA_FIXTURE)
    raise SystemExit
with open(RUTA_FIXTURE, "r") as archivo:
    spec = json.load(archivo)
ids = spec.get("node", {}).get("element_ids", [])


def extensiones():
    """Extensiones inicio/fin (mm) de las barras del fixture, para ver que vuelven a su valor."""
    salida = {}
    for eid in ids:
        el = doc.GetElement(DB.ElementId(System.Int64(int(eid))))
        if el is None:
            continue
        p0 = el.get_Parameter(DB.BuiltInParameter.START_EXTENSION)
        p1 = el.get_Parameter(DB.BuiltInParameter.END_EXTENSION)
        salida[str(eid)] = (round(p0.AsDouble() * MM_POR_PIE, 2) if p0 else None, round(p1.AsDouble() * MM_POR_PIE, 2) if p1 else None)
    return salida


def acero_suelto():
    """Placas y pernos de Advance Steel en el documento (como el sondeo 13)."""
    n = 0
    for nombre_cat in ("OST_StructConnectionPlates", "OST_StructConnectionBolts"):
        cat = getattr(DB.BuiltInCategory, nombre_cat, None)
        if cat is None:
            continue
        n += len(list(DB.FilteredElementCollector(doc).OfCategory(cat).WhereElementIsNotElementType()))
    return n


def conexiones():
    r = llamar("list", {})
    return (r.get("data") or {}).get("connections_count")


antes_ext = extensiones()
antes_acero = acero_suelto()
antes_conexiones = conexiones()
print("1) Antes: conexiones={0} | acero suelto={1} | extensiones={2}".format(antes_conexiones, antes_acero, antes_ext))
if antes_conexiones:
    print("PARADA: el modelo ya tiene conexiones del add-in; ejecuta antes el sondeo 12.")
    raise SystemExit
print("   doc.IsModifiable antes: {0}".format(doc.IsModifiable))

# ---------------------------------------------------------------- Parte A: anidado + Assimilate
print("2) PARTE A: grupo exterior + create (anidado) + delete (anidado) + Assimilate")
exterior = DB.TransactionGroup(doc, "Sondeo 20 A: lote de prueba")
exterior.Start()
print("   exterior.HasStarted={0} | doc.IsModifiable={1} (debe ser False: un grupo no abre transaccion)".format(exterior.HasStarted(), doc.IsModifiable))
conexion = None
try:
    r = llamar("validate", {"spec": spec})
    token = (r.get("data") or {}).get("validation_token")
    print("   token={0}".format(token))
    r = llamar("create", {"spec": spec, "validation_token": token})
    conexion = (r.get("data") or {}).get("connection_id")
    print("   create dentro del grupo exterior: ok={0} connection_id={1} elementos={2}".format(
        r.get("ok"), conexion, (r.get("data") or {}).get("created_elements_count")))
    print("   despues de create: conexiones={0} | acero suelto={1} | extensiones={2}".format(conexiones(), acero_suelto(), extensiones()))
    if conexion:
        r = llamar("delete", {"connection_id": conexion})
        print("   delete dentro del grupo exterior: ok={0} {1}".format(r.get("ok"), resumen(r.get("data"))))
    estado = exterior.Assimilate()
    print("   exterior.Assimilate() -> {0}".format(estado))
except Exception as error:
    print("   FALLO en la parte A: {0}: {1}".format(type(error).__name__, str(error)[:400]))
    try:
        if exterior.HasStarted() and not exterior.HasEnded():
            print("   exterior.RollBack() -> {0}".format(exterior.RollBack()))
    except Exception as error2:
        print("   ni RollBack: {0}".format(str(error2)[:200]))
finally:
    exterior.Dispose()
print("3) Tras A: conexiones={0} | acero suelto={1} | extensiones={2} | doc.IsModifiable={3}".format(conexiones(), acero_suelto(), extensiones(), doc.IsModifiable))
print("   ESPERADO: conexiones=0, acero suelto={0}, extensiones={1}, IsModifiable=False, y en Deshacer de Revit UNA entrada 'Sondeo 20 A'".format(antes_acero, antes_ext))

# ---------------------------------------------------------------- Parte B: anidado + RollBack del exterior
print("4) PARTE B: grupo exterior + create (anidado) + RollBack del exterior")
exterior = DB.TransactionGroup(doc, "Sondeo 20 B: lote revertido")
exterior.Start()
try:
    r = llamar("validate", {"spec": spec})
    token = (r.get("data") or {}).get("validation_token")
    r = llamar("create", {"spec": spec, "validation_token": token})
    conexion = (r.get("data") or {}).get("connection_id")
    print("   create dentro del grupo exterior: ok={0} connection_id={1} | conexiones ahora={2} | acero suelto={3}".format(
        r.get("ok"), conexion, conexiones(), acero_suelto()))
    estado = exterior.RollBack()
    print("   exterior.RollBack() -> {0}".format(estado))
except Exception as error:
    print("   FALLO en la parte B: {0}: {1}".format(type(error).__name__, str(error)[:400]))
    try:
        if exterior.HasStarted() and not exterior.HasEnded():
            exterior.RollBack()
    except Exception:
        pass
finally:
    exterior.Dispose()
despues_ext = extensiones()
despues_acero = acero_suelto()
despues_conexiones = conexiones()
print("5) Tras B: conexiones={0} | acero suelto={1} | extensiones={2} | doc.IsModifiable={3}".format(despues_conexiones, despues_acero, despues_ext, doc.IsModifiable))
bien = (despues_conexiones == 0 and despues_acero == antes_acero and despues_ext == antes_ext and not doc.IsModifiable)
print("6) RESULTADO: {0}".format("GRUPOS ANIDADOS OK: deja batch_single_undo en true" if bien
                                   else "GRUPOS ANIDADOS CON PROBLEMAS: pon \"batch_single_undo\": false en config\\catalog.json desplegado antes de crear el lote y copia este bloque"))
if despues_conexiones:
    print("   Quedo una conexion: ejecuta el sondeo 12 para borrarla.")
if despues_acero != antes_acero:
    print("   Quedo acero suelto: ejecuta el sondeo 13.")
print("=== fin 20-grupos-anidados ===")
