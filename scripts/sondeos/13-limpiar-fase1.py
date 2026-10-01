# -*- coding: utf-8 -*-
# Sondeo 13: borra de la copia "_sondeo.rvt" la placa y los pernos de Advance Steel que dejaron los sondeos 09 y 10
# de la Fase 1 (SteelProxyElement de las categorias Plates y Bolts), dentro de una FabricationTransaction.
# Si no se pueden borrar, no pasa nada: solo se solaparian visualmente con la conexion de la Fase 3.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600
from __future__ import print_function
import re


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name.lower() == nombre.lower():
                return a
        except Exception:
            pass
    return None


print("=== 13-limpiar-fase1 ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt'.")
    raise SystemExit
candidatos = []
for nombre_cat in ("OST_StructConnectionPlates", "OST_StructConnectionBolts"):
    cat = getattr(DB.BuiltInCategory, nombre_cat, None)
    if cat is None:
        print("   no existe la categoria " + nombre_cat)
        continue
    for el in DB.FilteredElementCollector(doc).OfCategory(cat).WhereElementIsNotElementType():
        candidatos.append(el.Id)
        print("   [{0}] {1} | {2}".format(el.Id.Value, el.GetType().Name, el.Category.Name))
print("1) Elementos de acero sueltos encontrados: {0}".format(len(candidatos)))
if not candidatos:
    print("   nada que borrar")
    raise SystemExit
ens = buscar_ensamblado("Autodesk.SteelConnectionsDB")
tipo_tx = ens.GetType("Autodesk.SteelConnectionsDB.FabricationTransaction") if ens is not None else None
ctor = None
if tipo_tx is not None:
    for c in tipo_tx.GetConstructors():
        if [p.ParameterType.Name for p in c.GetParameters()] == ["Document", "Boolean", "String"]:
            ctor = c
if ctor is None:
    print("PARADA: sin FabricationTransaction no se borra nada.")
    raise SystemExit
ids = System.Collections.Generic.List[DB.ElementId](candidatos)
tx = ctor.Invoke(System.Array[System.Object]([doc, False, "Limpiar sondeos Fase 1"]))
try:
    borrados = doc.Delete(ids)
    tipo_tx.GetMethod("Commit").Invoke(tx, None)
    print("2) Borrados {0} elementos. Commit OK".format(borrados.Count))
except Exception as error:
    print("2) ERROR al borrar: " + str(error)[:300])
    try:
        tipo_tx.GetMethod("CancelTransaction").Invoke(tx, None)
    except Exception:
        pass
finally:
    try:
        tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
try:
    doc.Save()
    print("3) Copia guardada")
except Exception as error:
    print("3) No se pudo guardar: " + str(error)[:200])
print("=== fin 13-limpiar-fase1 ===")
