# -*- coding: utf-8 -*-
# Sondeo 31 (ronda de sondeos de la Fase 14): INVENTARIO DE LA CERCHA. Solo lee; no abre ninguna transaccion.
# Por que: la propuesta de la Fase 14 (docs/propuestas/fase-14-opciones.md, seccion 4; decisiones D2, D8 y D14 de la seccion 8)
# deja a este sondeo el alcance real de la fase (`end_plate` contra un apoyo, kind "to_support"). Responde de una vez:
#   (a) si existe el CORDON INFERIOR como barra de armazon (una horizontal HSS3X3 en Z ~ 14 914; pendiente desde la ronda 8d);
#   (b) cuantos MONTANTES (barras verticales) hay, a que cordon llegan (superior Z ~ 19 933, central Z ~ 17 423, inferior) y con
#       que hueco entre su extremo y la cara del cordon;
#   (c) que barras terminan contra PILARES o VIGAS de apoyo (el eje B del plano) y a que rejilla de Revit estan;
#   (d) si alguna barra tiene UNIONES AUTOMATICAS activas (en la 13c las del nudo en K estaban desactivadas).
# Que hace:
#   1) AMBITO: las barras seleccionadas en Revit (armazon estructural y pilares); si no hay seleccion y existe %TEMP%\sondeo31-ids.txt
#      (un id por linea), esos ids hacen de seleccion (es donde los ids se construyen con System.Int64). Con seleccion, se anaden las barras y pilares del
#      modelo que estan en el mismo plano: dentro de la caja de la seleccion ampliada 1 500 mm en el plano de la cercha y con sus
#      dos extremos a menos de 600 mm del plano (asi entran montantes, cordon inferior y apoyos sin seleccionarlos, y quedan fuera
#      las correas que cruzan el plano). Sin seleccion, toda la categoria de armazon estructural (tarda mas; solo resumenes).
#   2) Por barra: id, familia y tipo, lados del perfil (canto x ancho: b y Ht del tipo, la seccion estructural o la designacion), giro
#      de seccion, clase (horizontal < 5 grados, vertical > 85, inclinada), cota Z de los dos extremos, largo, uniones automaticas
#      (IsJoinAllowedAtEnd) y, por extremo, el elemento mas cercano cuya CARA queda a menos de medio canto + 60 mm: id, tipo, cara
#      (+Z/-Z = arriba/abajo, +Y/-Y = los lados, en el marco local de la otra barra) y HUECO en mm (negativo = el extremo entra en
#      ella). Un extremo pegado al extremo de otra barra colineal se marca como "empalme". Los lados se reparten con la convencion
#      de la ronda 12c: sin giro el canto (Ht) va en el plano vertical de la barra; girada ~90 grados, el ancho (b). En un pilar los
#      lados se asignan a X e Y globales sin leer su giro (el hueco puede variar en (canto - ancho) / 2: se imprimen los dos lados).
#   3) Resumenes: horizontales por cota Z (ahi se ve el cordon inferior), montantes y a que llegan, extremos contra pilares o vigas
#      que no son HSS con la rejilla mas cercana, extremos libres, uniones automaticas activas y el recuento por clase y perfil.
# Los ids escritos a mano van como System.Int64 (IronPython: DB.ElementId(entero) es ambiguo; sondeo 30 v3). No se da por hecho nada
# de la API de Advance Steel: este sondeo no la toca. Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\31-inventario-cercha.py -SinTransaccion -TimeoutSec 900
# Sin poder seleccionar en Revit (PowerShell):  Set-Content $env:TEMP\sondeo31-ids.txt "1245533`n1249631`n1249632"   y repetir.
from __future__ import print_function
import math
import os
import re
import traceback

MM_POR_PIE = 304.8
AMPLIACION_PLANO_MM = 1500.0      # cuanto se amplia la caja de la seleccion en el plano de la cercha
AMPLIACION_NORMAL_MM = 600.0      # a cuanto del plano pueden estar los extremos de una vecina
HOLGURA_CARA_MM = 60.0            # un extremo "llega" a una cara si el hueco es menor que medio canto + esto
HORIZONTAL_MAX_DEG = 5.0
VERTICAL_MIN_DEG = 85.0
Z_CORDON_INFERIOR_MM = 14914.0    # fase-8.md 10.2: los puntos de abajo de la cercha de la 8c
Z_TOLERANCIA_NIVEL_MM = 400.0
MAX_LINEAS_DETALLE = 400          # por encima de esto solo se imprimen los resumenes
NOMBRES_ANCHO = ("b", "B", "Width", "Ancho", "bf")
NOMBRES_CANTO = ("d", "h", "H", "Ht", "Height", "Depth", "Alto")
ARCHIVO_IDS = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo31-ids.txt")


def mm(v):
    return v * MM_POR_PIE


def pies(v):
    return v / MM_POR_PIE


def fmt(v, dec=1):
    return ("{0:." + str(dec) + "f}").format(v)


def error_texto(error):
    interna = getattr(error, "InnerException", None)
    return "{0}: {1}{2}".format(type(error).__name__, str(error)[:200], (" | interna: " + str(interna)[:200]) if interna is not None else "")


def id64(entero):
    # IronPython: DB.ElementId(1249631) es ambiguo (tres constructores); System.Int64 elige ElementId(Int64), como en el sondeo 30 v3.
    return DB.ElementId(System.Int64(int(entero)))


def nombre_de(elemento):
    try:
        return elemento.Name
    except Exception:
        try:
            return DB.Element.Name.__get__(elemento)
        except Exception:
            return "?"


def es_de_categoria(elemento, bic):
    try:
        return elemento.Category is not None and elemento.Category.Id.Value == int(bic)
    except Exception:
        return False


def longitud_param(elemento, bip, nombres):
    """Un parametro de longitud en mm: primero el BuiltInParameter, despues por nombre (como RevitModelFacts.ReadLengthParamMm)."""
    if bip is not None:
        try:
            p = elemento.get_Parameter(bip)
            if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
                return mm(p.AsDouble())
        except Exception:
            pass
    for n in nombres:
        try:
            p = elemento.LookupParameter(n)
            if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
                return mm(p.AsDouble())
        except Exception:
            pass
    return 0.0


def pulgadas(texto):
    """'2-1/2' -> 2,5; '1/2' -> 0,5; '12' -> 12."""
    texto = texto.replace("-", " ") if "/" in texto else texto
    total = 0.0
    for trozo in texto.split():
        if "/" in trozo:
            a, b = trozo.split("/")
            total += float(a) / float(b)
        else:
            total += float(trozo)
    return total


def designacion_mm(nombre_tipo):
    """(canto, ancho) en mm desde el nombre: HSS12X8X1/2 -> (304,8, 203,2); 'W12X26 165x310' (sufijo ancho x canto) -> (310, 165)."""
    if not nombre_tipo:
        return 0.0, 0.0
    m = re.search(r"HSS\s*(\d+(?:-\d+/\d+|/\d+)?)\s*X\s*(\d+(?:-\d+/\d+|/\d+)?)", nombre_tipo, re.IGNORECASE)
    if m:
        try:
            return pulgadas(m.group(1)) * 25.4, pulgadas(m.group(2)) * 25.4
        except Exception:
            pass
    m = re.search(r"(\d+(?:\.\d+)?)\s*x\s*(\d+(?:\.\d+)?)\s*$", nombre_tipo)
    if m:
        return float(m.group(2)), float(m.group(1))
    return 0.0, 0.0


class Barra(object):
    """Una barra de armazon o un pilar con su eje, sus lados y su marco local (X = eje, Y = horizontal perpendicular, Z = 'arriba')."""

    def __init__(self, elemento, caja):
        self.e = elemento
        self.id = int(elemento.Id.Value)
        self.caja = caja
        self.es_pilar = es_de_categoria(elemento, DB.BuiltInCategory.OST_StructuralColumns)
        tipo = doc.GetElement(elemento.GetTypeId())
        self.familia = tipo.FamilyName if tipo is not None and hasattr(tipo, "FamilyName") else "?"
        self.tipo = nombre_de(tipo) if tipo is not None else "?"
        self.p0, self.p1 = self._extremos()
        self.largo_mm = mm(self.p0.DistanceTo(self.p1)) if self.p0 is not None else 0.0
        self.dir = (self.p1 - self.p0).Normalize() if self.p0 is not None and self.largo_mm > 1.0 else DB.XYZ.BasisZ
        self.pendiente_deg = math.degrees(math.asin(min(1.0, abs(self.dir.Z))))
        self.clase = "vertical" if self.pendiente_deg > VERTICAL_MIN_DEG else ("horizontal" if self.pendiente_deg < HORIZONTAL_MAX_DEG else "inclinada")
        self.canto_mm, self.ancho_mm, self.origen_lados = self._lados(tipo)
        self.giro_deg = self._giro()
        x = self.dir
        y = DB.XYZ.BasisX if abs(x.Z) > 0.99 else DB.XYZ.BasisZ.CrossProduct(x).Normalize()
        z = x.CrossProduct(y).Normalize()
        self.ejes = (x, y, z)
        girada = abs(math.sin(math.radians(self.giro_deg))) > abs(math.cos(math.radians(self.giro_deg)))
        if girada and self.clase != "vertical":
            self.medio_y_mm, self.medio_z_mm = self.canto_mm / 2.0, self.ancho_mm / 2.0
        else:
            self.medio_y_mm, self.medio_z_mm = self.ancho_mm / 2.0, self.canto_mm / 2.0
        self.medio_canto_mm = max(self.medio_y_mm, self.medio_z_mm, 1.0)

    def _extremos(self):
        loc = self.e.Location
        try:
            if isinstance(loc, DB.LocationCurve) and loc.Curve is not None:
                return loc.Curve.GetEndPoint(0), loc.Curve.GetEndPoint(1)
        except Exception:
            pass
        try:
            # Pilar vertical: LocationPoint; su eje va de la base a la cabeza (cotas de la caja envolvente).
            if isinstance(loc, DB.LocationPoint) and self.caja is not None:
                p = loc.Point
                return DB.XYZ(p.X, p.Y, self.caja.Min.Z), DB.XYZ(p.X, p.Y, self.caja.Max.Z)
        except Exception:
            pass
        return None, None

    def _lados(self, tipo):
        if tipo is None:
            return 0.0, 0.0, "sin tipo"
        ancho = longitud_param(tipo, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, NOMBRES_ANCHO)
        canto = longitud_param(tipo, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, NOMBRES_CANTO)
        origen = "tipo"
        if ancho <= 0.0 or canto <= 0.0:
            seccion = None
            try:
                seccion = tipo.GetStructuralSection()
            except Exception:
                pass
            if seccion is not None:
                for nombre, destino in (("Width", "ancho"), ("Height", "canto"), ("Diameter", "ambos")):
                    try:
                        v = mm(getattr(seccion, nombre))
                    except Exception:
                        continue
                    if v > 0 and destino in ("ancho", "ambos") and ancho <= 0.0:
                        ancho = v
                    if v > 0 and destino in ("canto", "ambos") and canto <= 0.0:
                        canto = v
                origen = "seccion"
        if ancho <= 0.0 or canto <= 0.0:
            d, w = designacion_mm(self.tipo)
            if canto <= 0.0 and d > 0:
                canto = d
            if ancho <= 0.0 and (w > 0 or d > 0):
                ancho = w if w > 0 else d
            origen = "designacion"
        if ancho <= 0.0 and canto > 0.0:
            ancho = canto
        if canto <= 0.0 and ancho > 0.0:
            canto = ancho
        return canto, ancho, origen

    def _giro(self):
        try:
            p = self.e.get_Parameter(DB.BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE)
            if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
                return math.degrees(p.AsDouble())
        except Exception:
            pass
        return 0.0

    def z_media_mm(self):
        return mm((self.p0.Z + self.p1.Z) / 2.0)

    def uniones(self):
        """(inicio, fin) de IsJoinAllowedAtEnd; None si es un pilar o la API no lo da."""
        if self.es_pilar:
            return None
        utils = getattr(DB.Structure, "StructuralFramingUtils", None) if hasattr(DB, "Structure") else None
        if utils is None:
            return None
        try:
            return bool(utils.IsJoinAllowedAtEnd(self.e, 0)), bool(utils.IsJoinAllowedAtEnd(self.e, 1))
        except Exception:
            return None

    def punto_mas_cercano(self, p):
        u = self.p1 - self.p0
        l2 = u.DotProduct(u)
        if l2 < 1e-12:
            return self.p0, 0.0
        t = max(0.0, min(1.0, (p - self.p0).DotProduct(u) / l2))
        return self.p0 + u * t, t

    def cara_y_hueco(self, p):
        """Que cara de esta barra mira al punto p, a que distancia esta (mm; negativa si p entra en el canto) y cuanto se pasa del extremo."""
        q, t = self.punto_mas_cercano(p)
        d = p - q
        x, y, z = self.ejes
        dx, dy, dz = mm(d.DotProduct(x)), mm(d.DotProduct(y)), mm(d.DotProduct(z))
        exceso_y = abs(dy) - self.medio_y_mm
        exceso_z = abs(dz) - self.medio_z_mm
        if exceso_y >= exceso_z:
            cara, hueco = ("+Y" if dy >= 0 else "-Y"), exceso_y
        else:
            cara, hueco = ("+Z" if dz >= 0 else "-Z"), exceso_z
        if self.clase == "horizontal" and not self.es_pilar:
            lectura = {"+Z": "arriba", "-Z": "abajo", "+Y": "lado +Y", "-Y": "lado -Y"}[cara]
        elif self.es_pilar or self.clase == "vertical":
            lectura = {"+Z": "lado +Z", "-Z": "lado -Z", "+Y": "lado +Y", "-Y": "lado -Y"}[cara]
        else:
            lectura = {"+Z": "cara superior", "-Z": "cara inferior", "+Y": "lado +Y", "-Y": "lado -Y"}[cara]
        return cara, lectura, hueco, abs(dx)


def caja_envolvente(cajas):
    minimo = [1e99, 1e99, 1e99]
    maximo = [-1e99, -1e99, -1e99]
    for c in cajas:
        if c is None:
            continue
        for i, (a, b) in enumerate(((c.Min.X, c.Max.X), (c.Min.Y, c.Max.Y), (c.Min.Z, c.Max.Z))):
            minimo[i] = min(minimo[i], a)
            maximo[i] = max(maximo[i], b)
    return (minimo, maximo) if minimo[0] <= maximo[0] else None


def cajas_se_cortan(caja, lo, hi):
    if caja is None:
        return False
    return not (caja.Max.X < lo[0] or caja.Min.X > hi[0] or caja.Max.Y < lo[1] or caja.Min.Y > hi[1] or caja.Max.Z < lo[2] or caja.Min.Z > hi[2])


def punto_en_caja(p, caja, margen_pies):
    if caja is None:
        return False
    return (caja.Min.X - margen_pies <= p.X <= caja.Max.X + margen_pies and
            caja.Min.Y - margen_pies <= p.Y <= caja.Max.Y + margen_pies and
            caja.Min.Z - margen_pies <= p.Z <= caja.Max.Z + margen_pies)


def coleccionar(bic):
    return [e for e in DB.FilteredElementCollector(doc).OfCategory(bic).WhereElementIsNotElementType().ToElements() if isinstance(e, DB.FamilyInstance)]


def rejillas_de_revit():
    """Las rejillas (DB.Grid) con su nombre y su recta en planta, para decir 'eje B' en vez de una coordenada."""
    rejillas = []
    try:
        for g in DB.FilteredElementCollector(doc).OfClass(DB.Grid):
            try:
                c = g.Curve
                if isinstance(c, DB.Line):
                    rejillas.append((g.Name, c.GetEndPoint(0), c.Direction.Normalize()))
            except Exception:
                pass
    except Exception as error:
        print("   (no se pudieron leer las rejillas: {0})".format(error_texto(error)))
    return rejillas


def rejilla_mas_cercana(rejillas, p):
    mejor = None
    for nombre, origen, direccion in rejillas:
        w = DB.XYZ(p.X - origen.X, p.Y - origen.Y, 0.0)
        d2 = DB.XYZ(direccion.X, direccion.Y, 0.0)
        if d2.GetLength() < 1e-9:
            continue
        d2 = d2.Normalize()
        dist = mm((w - d2 * w.DotProduct(d2)).GetLength())
        if mejor is None or dist < mejor[1]:
            mejor = (nombre, dist)
    return "{0} a {1} mm".format(mejor[0], fmt(mejor[1], 0)) if mejor else "-"


def xyz_texto(p):
    return "({0}, {1}, {2})".format(fmt(mm(p.X), 0), fmt(mm(p.Y), 0), fmt(mm(p.Z), 0))


def main():
    print("Sondeo 31: inventario de la cercha (Fase 14). Documento: {0}. Solo lectura.".format(doc.Title))
    # 1) Ambito --------------------------------------------------------------------------------------------------------------
    seleccion_ids = [int(i.Value) for i in uidoc.Selection.GetElementIds()] if uidoc is not None else []
    origen_seleccion = "seleccion de Revit"
    if not seleccion_ids and os.path.exists(ARCHIVO_IDS):
        origen_seleccion = ARCHIVO_IDS
        for linea in open(ARCHIVO_IDS).read().replace(",", " ").split():
            try:
                e = doc.GetElement(id64(linea))
            except Exception:
                e = None
            if e is None:
                print("   el id {0} de {1} no existe en el modelo: se ignora".format(linea, ARCHIVO_IDS))
            else:
                seleccion_ids.append(int(e.Id.Value))
    armazon = coleccionar(DB.BuiltInCategory.OST_StructuralFraming)
    pilares = coleccionar(DB.BuiltInCategory.OST_StructuralColumns)
    por_id = {}
    cajas = {}
    for e in armazon + pilares:
        i = int(e.Id.Value)
        por_id[i] = e
        try:
            cajas[i] = e.get_BoundingBox(None)
        except Exception:
            cajas[i] = None
    print("1) En el modelo: {0} barras de armazon estructural y {1} pilares estructurales. Seleccionados: {2} elementos ({3}).".format(len(armazon), len(pilares), len(seleccion_ids), origen_seleccion))
    seleccionadas = [i for i in seleccion_ids if i in por_id]
    descartados = [i for i in seleccion_ids if i not in por_id]
    if descartados:
        print("   en la seleccion hay {0} elementos que no son barras ni pilares (se ignoran): {1}".format(len(descartados), descartados[:10]))
    ids_seleccion = set(seleccionadas)
    vecinas = []
    if seleccionadas:
        envolvente = caja_envolvente([cajas[i] for i in seleccionadas])
        if envolvente is not None:
            minimo, maximo = envolvente
            tamanos = [maximo[k] - minimo[k] for k in range(3)]
            normal = 0 if tamanos[0] <= tamanos[1] else 1     # la cercha esta en un plano vertical: la normal es X o Y
            plano = (minimo[normal] + maximo[normal]) / 2.0
            lo = [minimo[k] - pies(AMPLIACION_NORMAL_MM if k == normal else AMPLIACION_PLANO_MM) for k in range(3)]
            hi = [maximo[k] + pies(AMPLIACION_NORMAL_MM if k == normal else AMPLIACION_PLANO_MM) for k in range(3)]
            print("   plano de la cercha: {0} = {1} mm (normal {0}); caja ampliada X {2}..{3} | Y {4}..{5} | Z {6}..{7} mm".format(
                "XY"[normal], fmt(mm(plano), 0), fmt(mm(lo[0]), 0), fmt(mm(hi[0]), 0), fmt(mm(lo[1]), 0), fmt(mm(hi[1]), 0), fmt(mm(lo[2]), 0), fmt(mm(hi[2]), 0)))
            fuera_del_plano = 0
            for i, e in por_id.items():
                if i in ids_seleccion or not cajas_se_cortan(cajas[i], lo, hi):
                    continue
                try:
                    b = Barra(e, cajas[i])
                except Exception:
                    continue
                if b.p0 is None:
                    continue
                coords = [(b.p0.X, b.p0.Y)[normal], (b.p1.X, b.p1.Y)[normal]]
                if b.es_pilar or all(abs(mm(c - plano)) <= AMPLIACION_NORMAL_MM for c in coords):
                    vecinas.append(b)
                else:
                    fuera_del_plano += 1
            print("   AMBITO: {0} seleccionadas + {1} vecinas en el mismo plano = {2} elementos ({3} cruzan la caja sin estar en el plano, p. ej. correas: fuera del ambito, pero cuentan como posibles apoyos)".format(
                len(seleccionadas), len(vecinas), len(seleccionadas) + len(vecinas), fuera_del_plano))
    construidas = {}
    for b in vecinas:
        construidas[b.id] = b

    def barra_de(i):
        if i not in construidas:
            construidas[i] = Barra(por_id[i], cajas[i])
        return construidas[i]

    barras = []
    if seleccionadas:
        for i in seleccionadas:
            try:
                b = barra_de(i)
                if b.p0 is not None:
                    barras.append(b)
            except Exception as error:
                print("   no se pudo leer {0}: {1}".format(i, error_texto(error)))
        barras += vecinas
    else:
        print("   AMBITO: sin seleccion, toda la categoria de armazon estructural ({0} barras); se imprimen solo los resumenes.".format(len(armazon)))
        for e in armazon:
            try:
                b = barra_de(int(e.Id.Value))
                if b.p0 is not None:
                    barras.append(b)
            except Exception as error:
                print("   no se pudo leer {0}: {1}".format(e.Id.Value, error_texto(error)))
    detalle = len(barras) <= MAX_LINEAS_DETALLE
    rejillas = rejillas_de_revit()
    print("   rejillas en el modelo: {0}".format(", ".join(sorted(n for n, _, _ in rejillas)) or "ninguna"))

    # 2) Extremos contra caras ------------------------------------------------------------------------------------------------
    print("2) Por barra: [id] familia : tipo | clase pendiente | canto x ancho [origen] giro | Z inicio -> Z fin | largo | uniones ini/fin | extremos" if detalle else "2) (sin detalle por barra: ambito grande)")
    llegadas = {}      # id -> [(extremo, Barra otra, cara, lectura, hueco, tipo_llegada), ...]
    for b in barras:
        llegadas[b.id] = []
        margen = pies(b.medio_canto_mm + HOLGURA_CARA_MM + 400.0)
        for indice, p in ((0, b.p0), (1, b.p1)):
            mejor = None
            for i, caja in cajas.items():
                if i == b.id or not punto_en_caja(p, caja, margen):
                    continue
                try:
                    otra = barra_de(i)
                except Exception:
                    continue
                if otra.p0 is None:
                    continue
                cara, lectura, hueco, mas_alla = otra.cara_y_hueco(p)
                colineal = abs(abs(otra.dir.DotProduct(b.dir)) - 1.0) < 0.02
                if colineal and hueco < 0 and mas_alla <= HOLGURA_CARA_MM:
                    tipo_llegada = "empalme"
                elif mas_alla > HOLGURA_CARA_MM:
                    continue          # cae mas alla del extremo de la otra barra: no es su cara
                else:
                    tipo_llegada = "cara"
                if hueco > otra.medio_canto_mm + HOLGURA_CARA_MM:
                    continue
                clave = (0 if tipo_llegada == "cara" else 1, hueco)
                if mejor is None or clave < mejor[0]:
                    mejor = (clave, otra, cara, lectura, hueco, tipo_llegada)
            if mejor is not None:
                llegadas[b.id].append((indice, mejor[1], mejor[2], mejor[3], mejor[4], mejor[5]))
        if detalle:
            u = b.uniones()
            texto_u = "{0}/{1}".format(*["si" if v else "no" for v in u]) if u is not None else "-"
            partes = []
            for indice in (0, 1):
                hallada = [l for l in llegadas[b.id] if l[0] == indice]
                if not hallada:
                    partes.append("{0} -> libre".format("inicio" if indice == 0 else "fin"))
                    continue
                _, otra, cara, lectura, hueco, tipo_llegada = hallada[0]
                partes.append("{0} -> [{1}] {2}{3} {4} hueco {5} mm".format(
                    "inicio" if indice == 0 else "fin", otra.id, otra.tipo, " [pilar]" if otra.es_pilar else "",
                    "cara {0} ({1})".format(cara, lectura) if tipo_llegada == "cara" else "empalme (colineal)", fmt(hueco)))
            print("   [{0}]{1} {2} : {3} | {4} {5} | {6} x {7} [{8}] giro {9} | Z {10} -> {11} | {12} mm | uniones {13} | {14}".format(
                b.id, "" if b.id in ids_seleccion else "*", b.familia, b.tipo, "PILAR" if b.es_pilar else b.clase, fmt(b.pendiente_deg) + " grados",
                fmt(b.canto_mm), fmt(b.ancho_mm), b.origen_lados, fmt(b.giro_deg, 0), fmt(mm(b.p0.Z), 0), fmt(mm(b.p1.Z), 0), fmt(b.largo_mm, 0), texto_u, " ; ".join(partes)))
    if detalle:
        print("   (* = vecina no seleccionada; hueco negativo = el extremo entra en la otra barra)")

    # 3) Resumenes ------------------------------------------------------------------------------------------------------------
    print("3) RESUMENES")
    niveles = {}
    for b in barras:
        if b.clase == "horizontal" and not b.es_pilar:
            niveles.setdefault(round(b.z_media_mm() / 50.0) * 50.0, []).append(b)
    print("3a) Barras horizontales por cota Z (redondeada a 50 mm): cuantas, perfiles, largo total, ids")
    for z in sorted(niveles.keys()):
        lista = niveles[z]
        perfiles = {}
        for b in lista:
            perfiles[b.tipo] = perfiles.get(b.tipo, 0) + 1
        print("   Z ~ {0} mm: {1} barras | {2} | largo total {3} mm | ids {4}{5}".format(
            fmt(z, 0), len(lista), ", ".join("{0} x{1}".format(k, v) for k, v in sorted(perfiles.items())), fmt(sum(b.largo_mm for b in lista), 0),
            [b.id for b in lista][:12], " ..." if len(lista) > 12 else ""))
    abajo = [b for b in barras if b.clase == "horizontal" and not b.es_pilar and abs(b.z_media_mm() - Z_CORDON_INFERIOR_MM) <= Z_TOLERANCIA_NIVEL_MM]
    if abajo:
        print("   CORDON INFERIOR (horizontal en Z ~ {0} +- {1}): SI, {2} tramo(s): {3}".format(
            fmt(Z_CORDON_INFERIOR_MM, 0), fmt(Z_TOLERANCIA_NIVEL_MM, 0), len(abajo), ", ".join("[{0}] {1} {2} mm".format(b.id, b.tipo, fmt(b.largo_mm, 0)) for b in abajo)))
    else:
        print("   CORDON INFERIOR (horizontal en Z ~ {0} +- {1}): NO hay ninguna barra de armazon horizontal a esa cota en el ambito.".format(fmt(Z_CORDON_INFERIOR_MM, 0), fmt(Z_TOLERANCIA_NIVEL_MM, 0)))
        clases = {}
        for b in barras:
            if any(abs(mm(p.Z) - Z_CORDON_INFERIOR_MM) <= Z_TOLERANCIA_NIVEL_MM for p in (b.p0, b.p1)):
                clave = "pilar" if b.es_pilar else b.clase
                clases[clave] = clases.get(clave, 0) + 1
        print("   a esa cota terminan {0} elementos del ambito: {1}".format(sum(clases.values()), ", ".join("{0} {1}".format(v, k) for k, v in sorted(clases.items())) or "ninguno"))
    montantes = [b for b in barras if b.clase == "vertical" and not b.es_pilar]
    print("3b) MONTANTES (barras de armazon verticales, pendiente > {0} grados): {1}".format(fmt(VERTICAL_MIN_DEG, 0), len(montantes)))
    por_destino = {}
    for b in montantes:
        destinos = []
        for indice, otra, cara, lectura, hueco, tipo_llegada in llegadas.get(b.id, []):
            etiqueta = "pilar" if otra.es_pilar else ("cordon Z~{0}".format(fmt(round(otra.z_media_mm() / 50.0) * 50.0, 0)) if otra.clase == "horizontal" else "barra " + otra.clase)
            destinos.append("{0} -> [{1}] {2} ({3}) {4} hueco {5} mm".format("inicio" if indice == 0 else "fin", otra.id, otra.tipo, etiqueta, lectura if tipo_llegada == "cara" else "empalme", fmt(hueco)))
            por_destino[etiqueta] = por_destino.get(etiqueta, 0) + 1
        print("   [{0}] {1} | {2} mm | Z {3} -> {4} | {5}".format(b.id, b.tipo, fmt(b.largo_mm, 0), fmt(mm(b.p0.Z), 0), fmt(mm(b.p1.Z), 0), " ; ".join(destinos) or "ningun extremo llega a una cara"))
    if montantes:
        print("   extremos de montante por destino: {0}".format(", ".join("{0}: {1}".format(k, v) for k, v in sorted(por_destino.items()))))
    print("3c) APOYOS: extremos del ambito que llegan a un pilar o a una viga que no es HSS (W, C, L, hormigon), con la rejilla mas cercana")
    hubo = False
    for b in barras:
        for indice, otra, cara, lectura, hueco, tipo_llegada in llegadas.get(b.id, []):
            es_apoyo = otra.es_pilar or ("HSS" not in otra.tipo.upper() and "HSS" not in otra.familia.upper())
            if not es_apoyo or tipo_llegada != "cara":
                continue
            hubo = True
            p = b.p0 if indice == 0 else b.p1
            print("   [{0}] {1} ({2}) {3} -> [{4}] {5} : {6} {7} cara {8} ({9}) hueco {10} mm | punto {11} | rejilla: {12}".format(
                b.id, b.tipo, b.clase, "inicio" if indice == 0 else "fin", otra.id, otra.familia, otra.tipo, "PILAR" if otra.es_pilar else "viga", cara, lectura, fmt(hueco),
                xyz_texto(p), rejilla_mas_cercana(rejillas, p)))
    if not hubo:
        print("   ninguno en el ambito")
    libres = []
    for b in barras:
        if b.es_pilar:
            continue
        for indice in (0, 1):
            if not any(l[0] == indice for l in llegadas.get(b.id, [])):
                libres.append((b, indice))
    print("3d) EXTREMOS LIBRES (no llegan a ninguna cara ni empalme a menos de medio canto + {0} mm): {1}".format(fmt(HOLGURA_CARA_MM, 0), len(libres)))
    for b, indice in libres[:40]:
        p = b.p0 if indice == 0 else b.p1
        print("   [{0}] {1} ({2}) {3} en {4} | rejilla: {5}".format(b.id, b.tipo, b.clase, "inicio" if indice == 0 else "fin", xyz_texto(p), rejilla_mas_cercana(rejillas, p)))
    if len(libres) > 40:
        print("   ... y {0} mas".format(len(libres) - 40))
    activas = []
    sin_dato = 0
    de_armazon = [b for b in barras if not b.es_pilar]
    for b in de_armazon:
        u = b.uniones()
        if u is None:
            sin_dato += 1
        elif u[0] or u[1]:
            activas.append((b, u))
    print("3e) UNIONES AUTOMATICAS activas en algun extremo: {0} barras de {1} (sin dato: {2})".format(len(activas), len(de_armazon), sin_dato))
    for b, u in activas[:40]:
        print("   [{0}] {1} inicio={2} fin={3}".format(b.id, b.tipo, u[0], u[1]))
    if len(activas) > 40:
        print("   ... y {0} mas".format(len(activas) - 40))
    recuento = {}
    for b in barras:
        clave = ("pilar" if b.es_pilar else b.clase, b.tipo)
        recuento[clave] = recuento.get(clave, 0) + 1
    print("3f) RECUENTO del ambito por clase y perfil:")
    for (clase, tipo), n in sorted(recuento.items()):
        print("   {0:<10} {1:<34} {2}".format(clase, tipo, n))
    print("Fin del sondeo 31.")


try:
    main()
except Exception as error:
    print("FALLO general: {0}".format(error_texto(error)))
    print(traceback.format_exc())
