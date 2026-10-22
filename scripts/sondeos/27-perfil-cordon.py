# -*- coding: utf-8 -*-
# Sondeo 27 (ronda 12b): QUE MIDE DE VERDAD el perfil del cordon superior HSS12X8X1/2 y EN QUE SENTIDO esta puesto. Solo lee.
# Por que: en la Fase 12 `node_info` devolvio width_mm = height_mm = 203,2 para el 1245533 (HSS12X8 es 304,8 x 203,2), la
# plancha del empalme se dibujo sobre una huella de 203 x 203 y el limite de la excentricidad salio 101,6 en vez de 152,4; el
# sondeo 22 midio la cara interior del cordon a 89,8 mm del eje (101,6 - 11,8 de pared): el 8" esta en el plano de la cercha.
# Este sondeo dice, para el cordon superior, una diagonal y el cordon central:
#   1) los parametros de SECCION del tipo (STRUCTURAL_SECTION_COMMON_WIDTH / HEIGHT, STRUCTURAL_SECTION_SHAPE y todos los de
#      longitud con nombres cortos: b, d, h, Ht, B, H, t, tdes, tnom, Width, Height, Depth...), con su valor en mm: es lo que
#      lee RevitModelFacts.ReadLengthParamMm;
#   2) los parametros de EJEMPLAR que giran o desplazan la seccion (Cross-Section Rotation, y/z Justification, y/z Offset Value);
#   3) la caja real del solido medida TRANSVERSALMENTE al eje: el canto en el plano de la cercha (direccion eje x Y global) y el
#      ancho perpendicular al plano (Y global). Con eso se sabe cual de los dos numeros del tipo va en el plano.
# Barras por defecto: cordon superior 1245533, diagonal 1249631 y cordon central 1249510 (Detalle D); con seleccion en Revit,
# las seleccionadas.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\27-perfil-cordon.py -SinTransaccion -TimeoutSec 300   (con -SinTransaccion: usa uidoc)
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import re
import traceback

IDS = [1245533, 1249631, 1249510]
MM_POR_PIE = 304.8
NOMBRES_CORTOS = r"^(b|d|h|Ht|B|H|t|tdes|tnom|t_nom|bf|tf|tw|Width|Height|Depth|Ancho|Alto|Espesor|Nominal Wall Thickness|Wall Thickness)$"


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


def valor(p):
    try:
        if p.StorageType == DB.StorageType.Double:
            try:
                tipo = p.Definition.GetDataType()
                if tipo == DB.SpecTypeId.Length:
                    return "{0:.2f} mm".format(mm(p.AsDouble()))
                if tipo == DB.SpecTypeId.Angle:
                    return "{0:.2f} grados".format(p.AsDouble() * 180.0 / 3.141592653589793)
            except Exception:
                pass
            return "{0:.4f} (crudo) | {1}".format(p.AsDouble(), p.AsValueString())
        if p.StorageType == DB.StorageType.Integer:
            return "{0} | {1}".format(p.AsInteger(), p.AsValueString())
        if p.StorageType == DB.StorageType.String:
            return p.AsString()
        return p.AsValueString()
    except Exception as error:
        return "ERROR {0}".format(str(error)[:80])


def builtin(el, nombre):
    try:
        bip = getattr(DB.BuiltInParameter, nombre)
    except Exception:
        return "{0}: no existe en esta API".format(nombre)
    try:
        p = el.get_Parameter(bip)
    except Exception as error:
        return "{0}: ERROR {1}".format(nombre, str(error)[:80])
    if p is None:
        return "{0}: (no lo tiene)".format(nombre)
    if not p.HasValue:
        return "{0}: sin valor".format(nombre)
    return "{0}: {1}".format(nombre, valor(p))


def caja_transversal(el):
    """Extension del solido a lo largo de las dos direcciones transversales al eje: (en el plano de la cercha, perpendicular al plano)."""
    a, b = el.Location.Curve.GetEndPoint(0), el.Location.Curve.GetEndPoint(1)
    eje = (b - a).Normalize()
    fuera = DB.XYZ.BasisY if abs(eje.DotProduct(DB.XYZ.BasisY)) < 0.9 else DB.XYZ.BasisX   # perpendicular al plano de la cercha (XZ)
    fuera = (fuera - eje.Multiply(fuera.DotProduct(eje))).Normalize()
    en_plano = eje.CrossProduct(fuera).Normalize()                                          # en el plano, perpendicular al eje
    opciones = DB.Options()
    opciones.DetailLevel = DB.ViewDetailLevel.Fine
    opciones.IncludeNonVisibleObjects = False
    minimos = {"plano": None, "fuera": None}
    maximos = {"plano": None, "fuera": None}
    caras = 0

    def anotar(p):
        for clave, direccion in (("plano", en_plano), ("fuera", fuera)):
            v = p.DotProduct(direccion)
            if minimos[clave] is None or v < minimos[clave]:
                minimos[clave] = v
            if maximos[clave] is None or v > maximos[clave]:
                maximos[clave] = v

    def recorrer(geometria):
        global_caras = 0
        for g in geometria:
            if isinstance(g, DB.Solid) and g.Volume > 1e-9:
                for cara in g.Faces:
                    global_caras += 1
                    malla = cara.Triangulate()
                    if malla is None:
                        continue
                    for v in malla.Vertices:
                        anotar(v)
            elif isinstance(g, DB.GeometryInstance):
                global_caras += recorrer(g.GetInstanceGeometry())
        return global_caras

    caras = recorrer(el.get_Geometry(opciones))
    if minimos["plano"] is None:
        return "sin solido (caras {0})".format(caras)
    return "canto EN EL PLANO de la cercha {0:.1f} mm | ancho PERPENDICULAR al plano {1:.1f} mm | caras {2} | eje {3} | en plano {4} | fuera {5}".format(
        mm(maximos["plano"] - minimos["plano"]), mm(maximos["fuera"] - minimos["fuera"]), caras, xyz_mm(eje), xyz_mm(en_plano), xyz_mm(fuera))


print("=== 27-perfil-cordon (ronda 12b) ===")
# Ronda 12c: por /execute_code/ (sin -SinTransaccion) no existe uidoc y el sondeo cayo con NameError en la 12b; sin seleccion, las de siempre.
try:
    seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
except NameError:
    seleccion = []
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
barras = seleccion if seleccion else [doc.GetElement(DB.ElementId(System.Int64(i))) for i in IDS]
barras = [b for b in barras if b is not None and isinstance(b, DB.FamilyInstance) and isinstance(b.Location, DB.LocationCurve)]
if not barras:
    print("PARADA: no encuentro ninguna de las barras {0}; selecciona una y repite.".format(IDS))
    raise SystemExit
paso(0, "Documento {0} | barras: {1}".format(doc.Title, ", ".join(str(b.Id.Value) for b in barras)))

n = 0
for el in barras:
    n += 1
    simbolo = el.Symbol
    try:
        nombre_tipo = DB.Element.Name.__get__(simbolo)
    except Exception:
        nombre_tipo = "?"
    print("")
    paso(n, "Barra [{0}] familia '{1}' tipo '{2}'".format(el.Id.Value, simbolo.FamilyName, nombre_tipo))
    try:
        a, b = el.Location.Curve.GetEndPoint(0), el.Location.Curve.GetEndPoint(1)
        print("   eje: {0} -> {1}".format(xyz_mm(a), xyz_mm(b)))
    except Exception as error:
        print("   eje: ERROR {0}".format(str(error)[:100]))
    # 1) Parametros de seccion del tipo (lo que lee RevitModelFacts)
    print("   TIPO (seccion):")
    for nombre in ("STRUCTURAL_SECTION_SHAPE", "STRUCTURAL_SECTION_COMMON_WIDTH", "STRUCTURAL_SECTION_COMMON_HEIGHT",
                   "STRUCTURAL_SECTION_HSS_OUTERFILLET", "STRUCTURAL_SECTION_COMMON_NOMINAL_WEIGHT"):
        print("     " + builtin(simbolo, nombre))
    cortos = []
    try:
        for p in simbolo.Parameters:
            try:
                nombre = p.Definition.Name
            except Exception:
                continue
            if re.match(NOMBRES_CORTOS, nombre):
                cortos.append("{0} = {1}".format(nombre, valor(p)))
    except Exception as error:
        cortos.append("ERROR {0}".format(str(error)[:100]))
    print("     con nombre corto: " + ("; ".join(sorted(cortos)) or "ninguno"))
    print("     cadena de RevitModelFacts: ancho = COMMON_WIDTH o b/Width/B/Ancho; alto = COMMON_HEIGHT o d/h/H/Height/Alto")
    # 2) Parametros de ejemplar que giran o desplazan la seccion
    print("   EJEMPLAR (giro y justificacion):")
    for nombre in ("STRUCTURAL_BEND_DIR_ANGLE", "Y_JUSTIFICATION", "Z_JUSTIFICATION", "Y_OFFSET_VALUE", "Z_OFFSET_VALUE",
                   "STRUCTURAL_BEAM_ORIENTATION", "START_EXTENSION", "END_EXTENSION"):
        print("     " + builtin(el, nombre))
    for nombre in ("Cross-Section Rotation", "y Justification", "z Justification", "y Offset Value", "z Offset Value", "Rotacion de seccion"):
        try:
            p = el.LookupParameter(nombre)
            if p is not None and p.HasValue:
                print("     '{0}' = {1}".format(nombre, valor(p)))
        except Exception:
            pass
    # 3) La caja real del solido, transversal al eje
    try:
        print("   SOLIDO: " + caja_transversal(el))
    except Exception as error:
        print("   SOLIDO: ERROR {0}".format(str(error)[:200]))
        print(traceback.format_exc())

print("")
paso(n + 1, "Como leerlo: si el solido del 1245533 da canto en el plano ~203 y ancho perpendicular ~305, el 8\" esta en el plano de la "
            "cercha (lo que supuso la Fase 12) y el add-in debe tomar como 'ancho' (perpendicular al plano) el numero del tipo que valga "
            "304,8 y como 'canto' el de 203,2, girados si Cross-Section Rotation no es 0. Copia este bloque entero en resultados-fase-12b.md.")
print("=== fin 27-perfil-cordon ===")
