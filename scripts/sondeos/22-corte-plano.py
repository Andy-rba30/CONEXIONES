# -*- coding: utf-8 -*-
# Sondeo 22 (Fase 12, direct_weld): ¿se puede CORTAR EL EXTREMO de una diagonal contra la cara del cordón (corte plano
# inclinado) con la API, en vez de acortarla por su eje (la reserva del add-in, END_CUT_APPROXIMATED)? Sobre la copia, sin
# dejar nada: todo lo que escribe se deshace (Transaction.RollBack / CancelTransaction). Mira dos caminos y anota cuál funciona:
#   A) API de Revit: Autodesk.Revit.DB.Structure.StructuralFramingUtils. Se LISTAN por reflexión sus métodos (no se da ninguno
#      por supuesto) y, si existen CanSetEndReference / SetEndReference, se intenta fijar como referencia del extremo la cara
#      del cordón a la que llega la diagonal (lo que hace a mano "Editar extremo de la viga > elegir cara"). Se mide el extremo
#      antes y después; después se deshace (RollBack).
#   B) API de Advance Steel (ASObjectsMgd): se LISTAN los tipos Beam*Shortening / *Cut / *Notch / *Cope con sus constructores y
#      propiedades, dentro de la FabricationTransaction, SIN crear ninguno (no hay firma conocida: se anota lo que hay para la
#      ronda 12b). Si la lista trae BeamShortening con un constructor (Beam, Plane) o parecido, se anota literalmente.
# Barras por defecto: la diagonal 1249631 y el cordón superior 1245533 (nudo en K del paso 11-5 de la Fase 11); con selección
# de 2 barras en Revit, la más horizontal es el cordón y la otra la diagonal.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\22-corte-plano.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import math
import re
import traceback

ID_DIAGONAL = 1249631
ID_CORDON = 1245533
MM_POR_PIE = 304.8
PROBAR_REVIT = True       # camino A: SetEndReference dentro de una Transaction que se deshace
PROBAR_ADVANCE = True     # camino B: solo listar tipos y firmas (no crea nada)


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


def barra(eid):
    el = doc.GetElement(DB.ElementId(System.Int64(eid)))
    if el is None or not isinstance(el, DB.FamilyInstance) or not isinstance(el.Location, DB.LocationCurve):
        return None
    return el


def extremos(el):
    c = el.Location.Curve
    return c.GetEndPoint(0), c.GetEndPoint(1)


def extension_mm(el, indice):
    bip = DB.BuiltInParameter.START_EXTENSION if indice == 0 else DB.BuiltInParameter.END_EXTENSION
    p = el.get_Parameter(bip)
    return mm(p.AsDouble()) if p is not None and p.HasValue else None


def longitud_corte_mm(el):
    for nombre in ("Cut Length", "Longitud de corte"):
        p = el.LookupParameter(nombre)
        if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
            return mm(p.AsDouble())
    try:
        p = el.get_Parameter(DB.BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH)
        if p is not None and p.HasValue:
            return mm(p.AsDouble())
    except Exception:
        pass
    return None


print("=== 22-corte-plano (Fase 12) ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Barras
seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
if len(seleccion) == 2:
    def pendiente(e):
        a, b = extremos(e)
        d = (b - a).Normalize()
        return abs(d.Z)
    seleccion.sort(key=pendiente)
    cordon, diagonal = seleccion[0], seleccion[1]
else:
    cordon, diagonal = barra(ID_CORDON), barra(ID_DIAGONAL)
if cordon is None or diagonal is None:
    print("PARADA: no encuentro el cordon {0} o la diagonal {1}; selecciona las dos barras y repite.".format(ID_CORDON, ID_DIAGONAL))
    raise SystemExit
c0, c1 = extremos(cordon)
d0, d1 = extremos(diagonal)
paso(1, "Cordon [{0}] {1}: {2} -> {3}".format(cordon.Id.Value, DB.Element.Name.__get__(cordon.Symbol), xyz_mm(c0), xyz_mm(c1)))
paso(1, "Diagonal [{0}] {1}: {2} -> {3}".format(diagonal.Id.Value, DB.Element.Name.__get__(diagonal.Symbol), xyz_mm(d0), xyz_mm(d1)))

# 2) Que extremo de la diagonal llega al cordon y donde corta su eje al del cordon (opcion A: el corte propio de la barra)
ex = (c1 - c0).Normalize()
def distancia_al_eje(p):
    w = p - c0
    return (w - ex.Multiply(w.DotProduct(ex))).GetLength()
indice = 0 if distancia_al_eje(d0) <= distancia_al_eje(d1) else 1
cerca, lejos = (d0, d1) if indice == 0 else (d1, d0)
dd = (cerca - lejos).Normalize()
# corte del eje de la diagonal con el eje del cordon (punto mas cercano entre rectas)
w = c0 - lejos
b = ex.DotProduct(dd)
den = 1.0 - b * b
if abs(den) < 1e-9:
    print("PARADA: la diagonal es paralela al cordon.")
    raise SystemExit
s = (b * dd.DotProduct(w) - ex.DotProduct(w)) / den
corte = c0 + ex.Multiply(s)
paso(2, "Extremo que llega: {0} ({1}); corte del eje con el eje del cordon: {2}; hueco extremo-eje: {3:.1f} mm".format(
    "inicio" if indice == 0 else "fin", xyz_mm(cerca), xyz_mm(corte), mm(cerca.DistanceTo(corte))))
paso(2, "Extension actual del extremo: {0} mm | Cut Length: {1} mm".format(extension_mm(diagonal, indice), longitud_corte_mm(diagonal)))

# 3) La cara del cordon a la que llega la diagonal: cara plana del solido del cordon cuyo plano corta el eje de la diagonal
#    entre su extremo y el corte, con la normal mirando hacia la diagonal.
opciones = DB.Options()
opciones.ComputeReferences = True
opciones.DetailLevel = DB.ViewDetailLevel.Fine
cara_elegida, punto_cara, distancia_cara = None, None, None
try:
    geometria = cordon.get_Geometry(opciones)
    solidos = []
    for g in geometria:
        if isinstance(g, DB.Solid) and g.Volume > 1e-9:
            solidos.append(g)
        elif isinstance(g, DB.GeometryInstance):
            for h in g.GetInstanceGeometry():
                if isinstance(h, DB.Solid) and h.Volume > 1e-9:
                    solidos.append(h)
    caras = 0
    for solido in solidos:
        for cara in solido.Faces:
            if not isinstance(cara, DB.PlanarFace):
                continue
            caras += 1
            n = cara.FaceNormal
            denominador = n.DotProduct(dd)
            if abs(denominador) < 1e-6:
                continue
            # punto del eje de la diagonal (desde el corte hacia atras) donde atraviesa el plano de la cara
            t = n.DotProduct(cara.Origin - corte) / denominador
            p = corte + dd.Multiply(t)
            # la cara mira hacia la diagonal (hacia donde esta su extremo lejano)
            if n.DotProduct(lejos - corte) <= 0:
                continue
            try:
                proyeccion = cara.Project(p)
            except Exception:
                proyeccion = None
            if proyeccion is None or proyeccion.Distance > 1e-6:
                continue
            d = mm(p.DistanceTo(corte))
            if distancia_cara is None or d < distancia_cara:
                cara_elegida, punto_cara, distancia_cara = cara, p, d
    paso(3, "Caras planas del cordon: {0}; cara a la que llega la diagonal: {1}".format(
        caras, "normal {0} | punto del eje en la cara {1} | a {2:.1f} mm del corte (medio canto esperado 101,6 en HSS12X8)".format(
            xyz_mm(cara_elegida.FaceNormal) if cara_elegida else "-", xyz_mm(punto_cara) if punto_cara else "-", distancia_cara or 0)
        if cara_elegida is not None else "NO ENCONTRADA"))
    if cara_elegida is not None:
        paso(3, "Reference de la cara: {0}".format("si" if cara_elegida.Reference is not None else "NO (ComputeReferences no dio referencia)"))
except Exception as error:
    paso(3, "no se pudo leer la geometria del cordon: {0}".format(error))
    print(traceback.format_exc())

# 4) Camino A: StructuralFramingUtils por reflexion
tipo_utils = None
try:
    tipo_utils = clr.GetClrType(DB.Structure.StructuralFramingUtils)
except Exception as error:
    paso(4, "Autodesk.Revit.DB.Structure.StructuralFramingUtils no esta disponible: {0}".format(error))
if tipo_utils is not None:
    metodos = sorted(set(m.Name for m in tipo_utils.GetMethods() if m.IsStatic and m.DeclaringType == tipo_utils))
    paso(4, "StructuralFramingUtils, metodos estaticos: {0}".format(", ".join(metodos)))
    for m in tipo_utils.GetMethods():
        if m.IsStatic and re.search(r"EndReference|Cut|Join|Extension", m.Name):
            print("   {0}({1}) -> {2}".format(m.Name, ", ".join(p.ParameterType.Name + " " + p.Name for p in m.GetParameters()), m.ReturnType.Name))
    tiene_set = "SetEndReference" in metodos
    tiene_can = "CanSetEndReference" in metodos
    tiene_get = "GetEndReference" in metodos
    tiene_remove = "RemoveEndReference" in metodos
    if PROBAR_REVIT and tiene_set and cara_elegida is not None and cara_elegida.Reference is not None:
        tx = DB.Transaction(doc, "Sondeo 22: corte plano (se deshace)")
        try:
            tx.Start()
            puede = None
            if tiene_can:
                try:
                    puede = DB.Structure.StructuralFramingUtils.CanSetEndReference(diagonal, indice)
                except Exception as error:
                    puede = "ERROR {0}".format(str(error)[:160])
            paso(5, "CanSetEndReference(diagonal, {0}) = {1}".format(indice, puede))
            antes = extension_mm(diagonal, indice)
            antes_corte = longitud_corte_mm(diagonal)
            DB.Structure.StructuralFramingUtils.SetEndReference(diagonal, indice, cara_elegida.Reference)
            doc.Regenerate()
            despues = extension_mm(diagonal, indice)
            despues_corte = longitud_corte_mm(diagonal)
            d0b, d1b = extremos(diagonal)
            extremo_nuevo = d0b if indice == 0 else d1b
            paso(5, "SetEndReference OK. Extension {0} -> {1} mm | Cut Length {2} -> {3} mm | extremo del eje {4} -> {5}".format(
                antes, despues, antes_corte, despues_corte, xyz_mm(cerca), xyz_mm(extremo_nuevo)))
            if tiene_get:
                try:
                    ref = DB.Structure.StructuralFramingUtils.GetEndReference(diagonal, indice)
                    paso(5, "GetEndReference devuelve: {0}".format("una referencia ({0})".format(ref.ElementId.Value) if ref is not None else "None"))
                except Exception as error:
                    paso(5, "GetEndReference ERROR {0}".format(str(error)[:160]))
            # Geometria del extremo tras el corte: caras del solido de la diagonal cerca del cordon (una cara inclinada = corte plano)
            try:
                geo = diagonal.get_Geometry(opciones)
                inclinadas = 0
                for g in geo:
                    sol = [g] if isinstance(g, DB.Solid) else (list(g.GetInstanceGeometry()) if isinstance(g, DB.GeometryInstance) else [])
                    for sd in sol:
                        if not isinstance(sd, DB.Solid) or sd.Volume < 1e-9:
                            continue
                        for cara in sd.Faces:
                            if isinstance(cara, DB.PlanarFace):
                                n = cara.FaceNormal
                                paralela_al_eje = abs(abs(n.DotProduct(dd)) - 1.0) < 1e-3
                                perpendicular_al_eje = abs(n.DotProduct(dd)) < 1e-3
                                if not paralela_al_eje and not perpendicular_al_eje and cara.Area > 1e-6:
                                    inclinadas += 1
                                    print("   cara inclinada del extremo: normal {0}, area {1:.0f} mm2".format(xyz_mm(n), cara.Area * MM_POR_PIE * MM_POR_PIE))
                paso(5, "Caras del extremo ni paralelas ni perpendiculares al eje (corte plano inclinado): {0}".format(inclinadas))
            except Exception as error:
                paso(5, "no se pudo leer la geometria tras el corte: {0}".format(str(error)[:160]))
            if tiene_remove:
                try:
                    DB.Structure.StructuralFramingUtils.RemoveEndReference(diagonal, indice)
                    doc.Regenerate()
                    paso(5, "RemoveEndReference OK; extension {0} mm".format(extension_mm(diagonal, indice)))
                except Exception as error:
                    paso(5, "RemoveEndReference ERROR {0}".format(str(error)[:160]))
        except Exception as error:
            paso(5, "SetEndReference ERROR {0}: {1}".format(type(error).__name__, str(error)[:300]))
            print(traceback.format_exc())
        finally:
            try:
                tx.RollBack()
                paso(5, "Transaction deshecha (RollBack). Extension ahora: {0} mm".format(extension_mm(diagonal, indice)))
            except Exception as error:
                paso(5, "RollBack ERROR {0}".format(str(error)[:160]))
    else:
        paso(5, "Camino A no probado: PROBAR_REVIT={0}, SetEndReference={1}, cara con Reference={2}".format(
            PROBAR_REVIT, tiene_set, cara_elegida is not None and cara_elegida.Reference is not None))

# 5) Camino B: tipos de Advance Steel que recortan vigas (solo listar; se anota para la 12b)
if PROBAR_ADVANCE:
    def buscar_ensamblado(nombre):
        for a in System.AppDomain.CurrentDomain.GetAssemblies():
            try:
                if a.GetName().Name.lower() == nombre.lower():
                    return a
            except Exception:
                pass
        return None

    def tipos_de(ensamblado):
        try:
            return list(ensamblado.GetExportedTypes())
        except Exception as error:
            tipos = getattr(getattr(error, "clsException", None), "Types", None)
            return [t for t in tipos if t is not None] if tipos is not None else []

    obj = buscar_ensamblado("ASObjectsMgd")
    if obj is None:
        paso(6, "ASObjectsMgd no esta cargado por Revit (Advance Steel no activo en esta sesion): sin camino B.")
    else:
        candidatos = [t for t in tipos_de(obj) if re.search(r"Beam.*(Shorten|Cut|Notch|Cope|Polycut|Contour)|Shortening|PolyCut|Polycut", t.Name)]
        paso(6, "Tipos de ASObjectsMgd que recortan vigas: {0}".format(", ".join(t.FullName for t in candidatos) or "NINGUNO"))
        for t in candidatos:
            for c in t.GetConstructors():
                print("   {0}({1})".format(t.Name, ", ".join(p.ParameterType.Name + " " + p.Name for p in c.GetParameters())))
            props = sorted(set(p.Name for p in t.GetProperties()))
            print("   propiedades de {0}: {1}".format(t.Name, ", ".join(props)[:600]))
        paso(6, "No se crea ninguno (sin firma conocida): copia esta lista en resultados-fase-12.md para la ronda 12b.")

print("=== fin 22-corte-plano ===")
