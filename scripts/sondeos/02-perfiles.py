# -*- coding: utf-8 -*-
# Sondeo 02: tipos de armazon estructural cargados (familia, tipo, forma de seccion, dimensiones) y
# cuantas instancias usa cada uno, para localizar los HSS de la cercha.
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\02-perfiles.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function

MM_POR_PIE = 304.8


def nombre(elemento):
    try:
        return elemento.Name
    except Exception:
        try:
            return DB.Element.Name.__get__(elemento)
        except Exception:
            return "?"


def a_mm(pies):
    try:
        return round(float(pies) * MM_POR_PIE, 2)
    except Exception:
        return "?"


def describir_seccion(simbolo):
    """Forma y medidas de la seccion estructural del tipo (si la familia las define)."""
    try:
        seccion = simbolo.GetStructuralSection()
    except Exception as error:
        return "GetStructuralSection ERROR " + type(error).__name__ + ": " + str(error)
    if seccion is None:
        return "sin seccion estructural"
    partes = []
    try:
        partes.append("forma=" + str(seccion.StructuralSectionShape))
    except Exception:
        partes.append("forma=?")
    try:
        partes.append("clase=" + seccion.GetType().Name)
    except Exception:
        pass
    for propiedad in ("Width", "Height", "WallNominalThickness", "WallDesignThickness", "Diameter", "FlangeThickness", "WebThickness"):
        try:
            valor = getattr(seccion, propiedad)
            partes.append("{0}={1}mm".format(propiedad, a_mm(valor)))
        except Exception:
            pass
    return " ".join(partes)


print("=== 02-perfiles ===")

# 1. Instancias por tipo (para saber que perfiles usa la cercha)
instancias_por_tipo = {}
ids_por_tipo = {}
try:
    coleccion = (DB.FilteredElementCollector(doc)
                 .OfCategory(DB.BuiltInCategory.OST_StructuralFraming)
                 .WhereElementIsNotElementType())
    for instancia in coleccion:
        try:
            tipo_id = int(instancia.GetTypeId().Value)
        except Exception:
            continue
        instancias_por_tipo[tipo_id] = instancias_por_tipo.get(tipo_id, 0) + 1
        lista = ids_por_tipo.setdefault(tipo_id, [])
        if len(lista) < 6:
            lista.append(int(instancia.Id.Value))
except Exception as error:
    print("ERROR recorriendo instancias: " + type(error).__name__ + ": " + str(error))

# 2. Tipos cargados de armazon estructural
try:
    simbolos = (DB.FilteredElementCollector(doc)
                .OfCategory(DB.BuiltInCategory.OST_StructuralFraming)
                .OfClass(DB.FamilySymbol)
                .ToElements())
except Exception as error:
    simbolos = []
    print("ERROR recorriendo tipos: " + type(error).__name__ + ": " + str(error))

print("Tipos de armazon estructural cargados: {0}".format(len(simbolos)))
filas = []
for simbolo in simbolos:
    try:
        familia = simbolo.Family.Name
    except Exception:
        familia = "?"
    tipo_id = int(simbolo.Id.Value)
    filas.append((familia, nombre(simbolo), tipo_id, simbolo))

filas.sort(key=lambda fila: (fila[0], fila[1]))
for familia, tipo, tipo_id, simbolo in filas:
    usos = instancias_por_tipo.get(tipo_id, 0)
    print("- [{0}] familia='{1}' tipo='{2}' instancias={3} ids={4}".format(
        tipo_id, familia, tipo, usos, ids_por_tipo.get(tipo_id, [])))
    if usos > 0 or "HSS" in tipo.upper() or "HSS" in familia.upper():
        print("    seccion: " + describir_seccion(simbolo))

# 3. Detalle de las instancias usadas: material, extremos y retiros
print("Instancias de armazon estructural (maximo 30):")
mostradas = 0
try:
    coleccion = (DB.FilteredElementCollector(doc)
                 .OfCategory(DB.BuiltInCategory.OST_StructuralFraming)
                 .WhereElementIsNotElementType())
    for instancia in coleccion:
        if mostradas >= 30:
            print("  ... (hay mas)")
            break
        mostradas += 1
        try:
            tipo = doc.GetElement(instancia.GetTypeId())
            etiqueta = "{0} : {1}".format(tipo.Family.Name, nombre(tipo))
        except Exception:
            etiqueta = "?"
        try:
            material = doc.GetElement(instancia.StructuralMaterialId)
            material = nombre(material) if material is not None else "sin material"
        except Exception:
            material = "?"
        try:
            curva = instancia.Location.Curve
            p0 = curva.GetEndPoint(0)
            p1 = curva.GetEndPoint(1)
            extremos = "({0}, {1}, {2}) -> ({3}, {4}, {5}) mm".format(
                a_mm(p0.X), a_mm(p0.Y), a_mm(p0.Z), a_mm(p1.X), a_mm(p1.Y), a_mm(p1.Z))
        except Exception:
            extremos = "sin curva de ubicacion"
        retiros = []
        for nombre_param, bip in (("inicio", DB.BuiltInParameter.START_EXTENSION), ("fin", DB.BuiltInParameter.END_EXTENSION)):
            try:
                parametro = instancia.get_Parameter(bip)
                retiros.append("{0}={1}mm".format(nombre_param, a_mm(parametro.AsDouble()) if parametro else "?"))
            except Exception:
                retiros.append(nombre_param + "=?")
        try:
            tipo_estructural = str(instancia.StructuralType)
        except Exception:
            tipo_estructural = "?"
        print("  [{0}] {1} | {2} | material={3} | {4} | retiros {5}".format(
            int(instancia.Id.Value), etiqueta, tipo_estructural, material, extremos, " ".join(retiros)))
except Exception as error:
    print("ERROR recorriendo instancias: " + type(error).__name__ + ": " + str(error))

print("=== fin 02-perfiles ===")
