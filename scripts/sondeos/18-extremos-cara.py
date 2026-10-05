# -*- coding: utf-8 -*-
# Sondeo 18 (ronda 8b): mide en Revit por que la deteccion de la Fase 8 no juntaba las diagonales con el cordon. Para las
# barras seleccionadas (o, sin seleccion, las cuatro del Detalle D: 1249510, 1249630, 1249631, 1249636) imprime el tipo,
# el canto de la seccion (b y h como los lee RevitModelFacts: STRUCTURAL_SECTION_COMMON_WIDTH / HEIGHT), los dos extremos
# de la LocationCurve en mm y, para cada extremo, la barra vecina cuyo eje corta al suyo (a menos de 5 mm), la distancia
# del extremo a ese eje (0 = llega al eje; 15-85 mm = termina en la cara del cordon) y el punto de corte. Es la misma
# regla que NodeDetector.SnapEnd del Core (alcance de cara = medio canto + medio canto + 10 mm). Solo lee: no toca nada.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\18-extremos-cara.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import math

MM_POR_PIE = 304.8
IDS_POR_DEFECTO = [1249510, 1249630, 1249631, 1249636]
EJE_MAX_MM = 5.0        # node_axis_max_distance_mm
AGRUPACION_MM = 10.0    # node_cluster_mm
MEDIO_CANTO_SUPUESTO = 40.0
SENO_MINIMO = math.sin(math.radians(5.0))


def mm(v):
    return v * MM_POR_PIE


def nombre_de(elemento):
    try:
        return DB.Element.Name.__get__(elemento)
    except Exception:
        pass
    for bip in (DB.BuiltInParameter.SYMBOL_NAME_PARAM, DB.BuiltInParameter.ALL_MODEL_TYPE_NAME):
        try:
            p = elemento.get_Parameter(bip)
            if p is not None and p.HasValue:
                return p.AsString()
        except Exception:
            pass
    return "?"


def medida_mm(simbolo, bip, nombres):
    try:
        p = simbolo.get_Parameter(bip)
        if p is not None and p.HasValue:
            return mm(p.AsDouble())
    except Exception:
        pass
    for n in nombres:
        try:
            p = simbolo.LookupParameter(n)
            if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
                return mm(p.AsDouble())
        except Exception:
            pass
    return 0.0


class Barra(object):
    def __init__(self, el):
        self.id = el.Id.Value
        self.tipo = nombre_de(el.Symbol)
        self.b = medida_mm(el.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, ["b", "Width", "B", "Ancho"])
        self.h = medida_mm(el.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, ["d", "h", "H", "Height", "Alto"])
        c = el.Location.Curve
        p0, p1 = c.GetEndPoint(0), c.GetEndPoint(1)
        self.p = (mm(p0.X), mm(p0.Y), mm(p0.Z))
        self.q = (mm(p1.X), mm(p1.Y), mm(p1.Z))
        self.canto = max(self.b, self.h)
        ext0 = el.get_Parameter(DB.BuiltInParameter.START_EXTENSION)
        ext1 = el.get_Parameter(DB.BuiltInParameter.END_EXTENSION)
        self.ext = (mm(ext0.AsDouble()) if ext0 is not None and ext0.HasValue else 0.0,
                    mm(ext1.AsDouble()) if ext1 is not None and ext1.HasValue else 0.0)

    def medio_canto(self):
        return self.canto / 2.0 if self.canto > 0 else MEDIO_CANTO_SUPUESTO

    def longitud(self):
        return dist(self.p, self.q)

    def direccion(self):
        return unit(sub(self.q, self.p))


def sub(a, b):
    return (a[0] - b[0], a[1] - b[1], a[2] - b[2])


def add(a, b):
    return (a[0] + b[0], a[1] + b[1], a[2] + b[2])


def mul(a, k):
    return (a[0] * k, a[1] * k, a[2] * k)


def dot(a, b):
    return a[0] * b[0] + a[1] * b[1] + a[2] * b[2]


def cross(a, b):
    return (a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0])


def norm(a):
    return math.sqrt(dot(a, a))


def unit(a):
    n = norm(a)
    return mul(a, 1.0 / n) if n > 1e-9 else a


def dist(a, b):
    return norm(sub(a, b))


def dist_recta(punto, origen, direccion):
    w = sub(punto, origen)
    return dist(punto, add(origen, mul(direccion, dot(w, direccion))))


def puntos_cercanos(oa, u, ob, v):
    w = sub(oa, ob)
    b = dot(u, v)
    d = dot(u, w)
    e = dot(v, w)
    den = 1.0 - b * b
    if den < 1e-9:
        return None
    ta = (b * e - d) / den
    tb = (e - b * d) / den
    return add(oa, mul(u, ta)), add(ob, mul(v, tb)), ta, tb


def corte(barra, extremo, barras):
    """Como NodeDetector.SnapEnd: (vecina, distancia del extremo al eje vecino, punto de corte, atraviesa) o None."""
    u = barra.direccion()
    mejor = None
    for otra in barras:
        if otra.id == barra.id:
            continue
        v = otra.direccion()
        if norm(cross(u, v)) < SENO_MINIMO:
            continue
        pc = puntos_cercanos(barra.p, u, otra.p, v)
        if pc is None:
            continue
        p, q, ta, tb = pc
        if dist(p, q) > EJE_MAX_MM:
            continue
        alcance = barra.medio_canto() + otra.medio_canto() + AGRUPACION_MM
        d = dist_recta(extremo, otra.p, v)
        if d > alcance or tb < -alcance or tb > otra.longitud() + alcance:
            continue
        punto = mul(add(p, q), 0.5)
        if dist(punto, extremo) > 500.0:
            continue
        atraviesa = alcance < tb < otra.longitud() - alcance
        rango = (0 if atraviesa else 1, d)
        if mejor is None or rango < mejor[0]:
            mejor = (rango, otra, d, punto, atraviesa, alcance)
    return None if mejor is None else mejor[1:]


print("=== 18-extremos-cara ===")
elementos = []
try:
    for eid in uidoc.Selection.GetElementIds():
        el = doc.GetElement(eid)
        if isinstance(el, DB.FamilyInstance) and isinstance(el.Location, DB.LocationCurve):
            elementos.append(el)
except Exception:
    pass
origen = "seleccion"
if not elementos:
    origen = "Detalle D por defecto"
    for i in IDS_POR_DEFECTO:
        el = doc.GetElement(DB.ElementId(System.Int64(i)))
        if isinstance(el, DB.FamilyInstance) and isinstance(el.Location, DB.LocationCurve):
            elementos.append(el)
print("1) Barras ({0}): {1}".format(origen, len(elementos)))
barras = [Barra(el) for el in elementos]

print("2) Tipo, canto (b x h mm, como RevitModelFacts) y extremos de la LocationCurve (mm); extensiones inicio/fin:")
for b in barras:
    print("   [{0}] {1} | b={2:.1f} h={3:.1f} canto={4:.1f}{5} | ext {6:.1f}/{7:.1f} | ({8:.1f}, {9:.1f}, {10:.1f}) -> ({11:.1f}, {12:.1f}, {13:.1f})".format(
        b.id, b.tipo, b.b, b.h, b.canto, "" if b.canto > 0 else " (sin medidas: se suponen 80)", b.ext[0], b.ext[1],
        b.p[0], b.p[1], b.p[2], b.q[0], b.q[1], b.q[2]))

print("3) Cada extremo frente al eje vecino (regla de NodeDetector.SnapEnd, alcance = medio canto + medio canto + 10):")
sueltos = 0
for b in barras:
    for etiqueta, extremo in (("inicio", b.p), ("fin", b.q)):
        r = corte(b, extremo, barras)
        if r is None:
            sueltos += 1
            print("   [{0}] {1}: sin eje vecino a alcance (extremo suelto o cordon de otra cercha)".format(b.id, etiqueta))
            continue
        otra, d, punto, atraviesa, alcance = r
        print("   [{0}] {1}: corta el eje de [{2}] {3}{4} | extremo a {5:.1f} mm de ese eje (alcance {6:.1f}) | se queda a {7:.1f} mm del corte | corte ({8:.1f}, {9:.1f}, {10:.1f})".format(
            b.id, etiqueta, otra.id, otra.tipo, " (atraviesa)" if atraviesa else " (tambien termina ahi)", d, alcance,
            dist(extremo, punto), punto[0], punto[1], punto[2]))
print("4) Extremos sin vecino: {0} de {1}".format(sueltos, 2 * len(barras)))
print("=== fin 18-extremos-cara ===")
