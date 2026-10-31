# -*- coding: utf-8 -*-
# Sondeo 32 (ronda de sondeos de la Fase 14; prepara la Fase 15, `base_plate`): PILARES, QUE HAY BAJO SU BASE, LOS ANCLAJES QUE YA
# ESTAN MODELADOS Y LA API DE ANCLAJES DE ADVANCE STEEL. Solo lee; no abre ninguna transaccion (ni de Revit ni FabricationTransaction:
# para listar tipos por reflexion no hace falta, como en el sondeo 06; crear un anclaje de prueba sera otro sondeo, el de la Fase 15).
# Por que: la propuesta de la Fase 14 (docs/propuestas/fase-14-opciones.md, seccion 4 y decision D9 de la seccion 8) deja a este
# sondeo medir lo que `base_plate` necesita saber antes de escribirse: si hay pilares con base en el modelo, que hay debajo (zapata,
# losa, viga, nada) y a que cota, como estan modelados hoy los anclajes (el sondeo 02 de la Fase 0 vio familias de armazon
# 'ANCLAJE_O7-8- 22x22' (la O es el simbolo de diametro) con 72 ejemplares y 'ESPARRAGO 5-8 16x16' con 48) y si la API de Advance Steel tiene objetos de anclaje y grout.
# Que hace:
#   1) PILARES ESTRUCTURALES (OST_StructuralColumns): por pilar, id, familia : tipo, material estructural, inclinado o no, lados del
#      perfil (b/Ht del tipo, la seccion estructural o la designacion), nivel y desfase de base y de cabeza, cota Z de la base (por la
#      caja envolvente, en coordenadas internas como node_info; y por el nivel + desfase), largo, punto en planta, giro, rejillas mas
#      cercanas; y QUE HAY DEBAJO de la base: (i) por cajas envolventes, el elemento de cimentacion, suelo, armazon, muro, terreno o
#      pilar cuya caja contiene el punto de la base en planta y cuya cara superior queda entre 1 000 mm por debajo y 50 por encima
#      (el mas alto) y si la base esta DENTRO de algun elemento; (ii) con ReferenceIntersector hacia abajo desde la base en una vista
#      3D (si la hay), el primer elemento que toca el rayo y a que distancia. Resumen por tipo y cota de base.
#      Si existe %TEMP%\sondeo32-ids.txt (un id por linea), solo esos pilares (los ids se construyen con System.Int64, sondeo 30 v3).
#   2) ANCLAJES MODELADOS COMO ARMAZON: los tipos de armazon cuyo nombre de familia o tipo contiene ANCLA, ANCHOR, ESPARRAGO, PERNO,
#      BOLT, STUD o VARILLA; por tipo, ejemplares, diametro y una muestra; y por pilar, cuantos anclajes tienen su cabeza a menos de
#      300 mm de la cota de la base y a menos de 600 mm en planta, su diametro, su largo, cuanto quedan embebidos bajo la base y
#      cuanto asoman, y su reparto en planta (lo que `base_plate` llamara el patron de anclajes).
#   3) LAS CATEGORIAS DE CONEXION DE REVIT: OST_StructConnections y sus subcategorias (planchas, pernos, ANCLAJES, soldaduras, ...)
#      si existen en la API 2027 (getattr, sin dar por hecho el nombre) y cuantos elementos tiene cada una; los nombres de
#      BuiltInCategory que contienen Anchor o Grout.
#   4) LA API DE ADVANCE STEEL por reflexion: los ensamblados de acero cargados; los archivos de <Revit>\AddIns\SteelConnections con
#      Anchor o Grout en el nombre y las DLL gestionadas AS*Mgd que se cargan por ruta si faltan (como el sondeo 06); los tipos de
#      todos esos ensamblados (y de RevitAPI / RevitAPISteel) cuyo nombre contiene "Anchor" o "Grout": clase o enumeracion, cadena de
#      herencia, CONSTRUCTORES con sus parametros, metodos estaticos, y los metodos y propiedades de instancia con nombre relevante;
#      los valores de las enumeraciones; los tipos *BoltPattern* (para ver donde cuelga el anclaje); y los miembros llamados *Anchor* o
#      *Grout* en cualquier otro tipo de ASObjectsMgd. No se da por hecho ningun nombre: si no sale nada, `base_plate` se programa con
#      pernos largos y aviso (propuesta, 2.B).
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\32-columnas-y-anclajes.py -SinTransaccion -TimeoutSec 900
from __future__ import print_function
import math
import os
import re
import traceback

MM_POR_PIE = 304.8
ARCHIVO_IDS = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo32-ids.txt")
DEBAJO_HASTA_MM = 1000.0          # hasta cuanto por debajo de la base se busca algo
DEBAJO_SOBRE_MM = 50.0            # cuanto por encima de la base puede quedar la cara superior de lo que hay debajo
MARGEN_PLANTA_MM = 50.0
ANCLAJE_RADIO_PLANTA_MM = 600.0   # un anclaje "es" de un pilar si esta a menos de esto en planta
ANCLAJE_RADIO_Z_MM = 300.0        # ... y su cabeza (Z maxima) a menos de esto de la cota de la base
MAX_PILARES_DETALLE = 160
MAX_MIEMBROS = 45
MAX_TIPOS_PATRON = 40
PATRON_ANCLAJE = re.compile(r"ANCLA|ANCHOR|ESP.RRAGO|PERNO|BOLT|STUD|VARILLA", re.IGNORECASE)
PATRON_TIPOS = re.compile(r"Anchor|Grout", re.IGNORECASE)
PATRON_MIEMBROS = re.compile(r"Anchor|Grout|Length|Diameter|Type|Nut|Washer|Hole|Plate|Connect|Assembly|Standard|Material|Grade|Orientation|Normal|Pattern|Insert|Write|Create|Set|Get|Count|Number|Spacing|Dist|Radius|Height|Top|Bottom|Head|Embed|Project|Invert", re.IGNORECASE)
NOMBRES_ANCHO = ("b", "B", "Width", "Ancho", "bf")
NOMBRES_CANTO = ("d", "h", "H", "Ht", "Height", "Depth", "Alto")
CATEGORIAS_DEBAJO = ("OST_StructuralFoundation", "OST_Floors", "OST_StructuralFraming", "OST_Walls", "OST_Toposolid", "OST_Topography",
                     "OST_StructuralColumns", "OST_GenericModel", "OST_Mass", "OST_Ceilings", "OST_Roofs")
SUBCATEGORIAS_CONEXION = ("OST_StructConnections", "OST_StructConnectionPlates", "OST_StructConnectionBolts", "OST_StructConnectionAnchors",
                          "OST_StructConnectionWelds", "OST_StructConnectionShearStuds", "OST_StructConnectionHoles", "OST_StructConnectionProfiles",
                          "OST_StructConnectionNobleWarning", "OST_StructConnectionModifiers", "OST_StructConnectionOthers")


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


def categoria_de(elemento):
    try:
        return elemento.Category.Name if elemento.Category is not None else "(sin categoria)"
    except Exception:
        return "?"


def tipo_texto(elemento):
    try:
        t = doc.GetElement(elemento.GetTypeId())
        if t is None:
            return "(sin tipo)"
        fam = t.FamilyName if hasattr(t, "FamilyName") else None
        return "{0} : {1}".format(fam, nombre_de(t)) if fam else nombre_de(t)
    except Exception:
        return "?"


def bic(nombre):
    return getattr(DB.BuiltInCategory, nombre, None)


def coleccionar(categoria):
    try:
        return list(DB.FilteredElementCollector(doc).OfCategory(categoria).WhereElementIsNotElementType().ToElements())
    except Exception:
        return []


def longitud_param(elemento, bip, nombres):
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


def lados_mm(tipo):
    """(canto, ancho, diametro, origen) del tipo: parametros, seccion estructural o designacion."""
    if tipo is None:
        return 0.0, 0.0, 0.0, "sin tipo"
    ancho = longitud_param(tipo, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, NOMBRES_ANCHO)
    canto = longitud_param(tipo, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, NOMBRES_CANTO)
    diametro = 0.0
    origen = "tipo"
    seccion = None
    try:
        seccion = tipo.GetStructuralSection()
    except Exception:
        pass
    if seccion is not None:
        try:
            diametro = mm(seccion.Diameter)
        except Exception:
            diametro = 0.0
        if ancho <= 0.0 or canto <= 0.0:
            for nombre, destino in (("Width", "ancho"), ("Height", "canto")):
                try:
                    v = mm(getattr(seccion, nombre))
                except Exception:
                    continue
                if v > 0 and destino == "ancho" and ancho <= 0.0:
                    ancho = v
                if v > 0 and destino == "canto" and canto <= 0.0:
                    canto = v
            origen = "seccion"
    if ancho <= 0.0 or canto <= 0.0:
        d, w = designacion_mm(nombre_de(tipo))
        if canto <= 0.0 and d > 0:
            canto = d
        if ancho <= 0.0 and (w > 0 or d > 0):
            ancho = w if w > 0 else d
        if d > 0 or w > 0:
            origen = "designacion"
    if diametro > 0 and ancho <= 0.0 and canto <= 0.0:
        ancho = canto = diametro
    return canto, ancho, diametro, origen


def parametro(elemento, *nombres_bip):
    for n in nombres_bip:
        b = getattr(DB.BuiltInParameter, n, None)
        if b is None:
            continue
        try:
            p = elemento.get_Parameter(b)
        except Exception:
            p = None
        if p is not None and p.HasValue:
            return p, n
    return None, None


def nivel_y_desfase(elemento, nombres_nivel, nombres_desfase):
    """(nombre del nivel, elevacion del nivel en mm o None, desfase en mm, de que parametros salio)."""
    p_nivel, n1 = parametro(elemento, *nombres_nivel)
    p_desf, n2 = parametro(elemento, *nombres_desfase)
    nombre, elev = "-", None
    if p_nivel is not None and p_nivel.StorageType == DB.StorageType.ElementId:
        try:
            nivel = doc.GetElement(p_nivel.AsElementId())
            if nivel is not None:
                nombre = nombre_de(nivel)
                elev = mm(nivel.Elevation)
        except Exception:
            pass
    desfase = mm(p_desf.AsDouble()) if p_desf is not None and p_desf.StorageType == DB.StorageType.Double else 0.0
    return nombre, elev, desfase, "{0}/{1}".format(n1 or "-", n2 or "-")


def punto_planta(elemento, caja):
    loc = elemento.Location
    try:
        if isinstance(loc, DB.LocationPoint):
            return loc.Point, math.degrees(loc.Rotation)
        if isinstance(loc, DB.LocationCurve) and loc.Curve is not None:
            a, b = loc.Curve.GetEndPoint(0), loc.Curve.GetEndPoint(1)
            return (a if a.Z <= b.Z else b), None
    except Exception:
        pass
    if caja is not None:
        return DB.XYZ((caja.Min.X + caja.Max.X) / 2.0, (caja.Min.Y + caja.Max.Y) / 2.0, caja.Min.Z), None
    return None, None


def rejillas_de_revit():
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


def rejillas_cercanas(rejillas, p):
    """Las dos rejillas mas cercanas de direcciones distintas: 'B a 12 mm / 3 a 40 mm'."""
    por_direccion = {}
    for nombre, origen, direccion in rejillas:
        d2 = DB.XYZ(direccion.X, direccion.Y, 0.0)
        if d2.GetLength() < 1e-9:
            continue
        d2 = d2.Normalize()
        w = DB.XYZ(p.X - origen.X, p.Y - origen.Y, 0.0)
        dist = mm((w - d2 * w.DotProduct(d2)).GetLength())
        angulo = int(round(math.degrees(math.atan2(d2.Y, d2.X))) % 180 / 15.0)
        if angulo not in por_direccion or dist < por_direccion[angulo][1]:
            por_direccion[angulo] = (nombre, dist)
    mejores = sorted(por_direccion.values(), key=lambda par: par[1])[:2]
    return " / ".join("{0} a {1} mm".format(n, fmt(d, 0)) for n, d in mejores) or "-"


def caja_de(elemento):
    try:
        return elemento.get_BoundingBox(None)
    except Exception:
        return None


def vista_3d():
    try:
        v = uidoc.ActiveView if uidoc is not None else None
        if isinstance(v, DB.View3D) and not v.IsTemplate and not v.IsPerspective:
            return v
    except Exception:
        pass
    try:
        for v in DB.FilteredElementCollector(doc).OfClass(DB.View3D):
            if not v.IsTemplate and not v.IsPerspective:
                return v
    except Exception:
        pass
    return None


# ----------------------------------------------------------------------------------------------------------------------------
# 1) Pilares y que hay debajo
# ----------------------------------------------------------------------------------------------------------------------------
def pilares_y_bases():
    pilares = [e for e in coleccionar(DB.BuiltInCategory.OST_StructuralColumns) if isinstance(e, DB.FamilyInstance)]
    print("1) PILARES ESTRUCTURALES en el modelo: {0}".format(len(pilares)))
    if os.path.exists(ARCHIVO_IDS):
        quiero = set()
        for linea in open(ARCHIVO_IDS).read().replace(",", " ").split():
            try:
                e = doc.GetElement(id64(linea))
                if e is not None:
                    quiero.add(int(e.Id.Value))
                else:
                    print("   el id {0} de {1} no existe: se ignora".format(linea, ARCHIVO_IDS))
            except Exception as error:
                print("   id {0} no valido ({1})".format(linea, error_texto(error)))
        pilares = [e for e in pilares if int(e.Id.Value) in quiero]
        print("   solo los {0} pilares de {1}".format(len(pilares), ARCHIVO_IDS))
    if not pilares:
        return []
    rejillas = rejillas_de_revit()
    # Candidatos a "lo que hay debajo": todo lo de esas categorias con su caja (una sola pasada).
    candidatos = []
    for nombre_cat in CATEGORIAS_DEBAJO:
        c = bic(nombre_cat)
        if c is None:
            print("   (la categoria {0} no existe en esta API: se salta)".format(nombre_cat))
            continue
        for e in coleccionar(c):
            caja = caja_de(e)
            if caja is not None:
                candidatos.append((e, caja, nombre_cat))
    print("   candidatos a estar bajo una base (cimentaciones, suelos, armazon, muros, terreno, pilares, genericos, masas, techos, cubiertas): {0}".format(len(candidatos)))
    vista = vista_3d()
    print("   vista 3D para el rayo hacia abajo: {0}".format("'{0}' (id {1})".format(nombre_de(vista), vista.Id.Value) if vista is not None else "NINGUNA (se salta el rayo)"))
    filtro_cats = None
    if vista is not None:
        try:
            from System.Collections.Generic import List
            lista = List[DB.BuiltInCategory]()
            for nombre_cat in CATEGORIAS_DEBAJO:
                c = bic(nombre_cat)
                if c is not None:
                    lista.Add(c)
            filtro_cats = DB.ElementMulticategoryFilter(lista)
        except Exception as error:
            print("   (sin filtro de categorias para el rayo: {0})".format(error_texto(error)))
    detalle = len(pilares) <= MAX_PILARES_DETALLE
    resumen = {}
    filas = []
    print("   Por pilar: [id] familia : tipo | material | lados | base: nivel + desfase = Z (caja Z) | cabeza | largo | planta (X, Y) giro | rejillas | DEBAJO")
    for e in pilares:
        try:
            caja = caja_de(e)
            tipo = doc.GetElement(e.GetTypeId())
            canto, ancho, diametro, origen = lados_mm(tipo)
            try:
                material = str(e.StructuralMaterialType)
            except Exception:
                material = "?"
            try:
                inclinado = bool(e.IsSlantedColumn)
            except Exception:
                inclinado = False
            nb, eb, db_, pb = nivel_y_desfase(e, ("FAMILY_BASE_LEVEL_PARAM", "SCHEDULE_BASE_LEVEL_PARAM"), ("FAMILY_BASE_LEVEL_OFFSET_PARAM", "SCHEDULE_BASE_LEVEL_OFFSET_PARAM"))
            nt, et, dt, pt = nivel_y_desfase(e, ("FAMILY_TOP_LEVEL_PARAM", "SCHEDULE_TOP_LEVEL_PARAM"), ("FAMILY_TOP_LEVEL_OFFSET_PARAM", "SCHEDULE_TOP_LEVEL_OFFSET_PARAM"))
            z_base_nivel = (eb + db_) if eb is not None else None
            z_base_caja = mm(caja.Min.Z) if caja is not None else None
            z_base = z_base_caja if z_base_caja is not None else z_base_nivel
            p, giro = punto_planta(e, caja)
            largo = longitud_param(e, getattr(DB.BuiltInParameter, "INSTANCE_LENGTH_PARAM", None), ("Length", "Longitud"))
            if largo <= 0 and caja is not None:
                largo = mm(caja.Max.Z - caja.Min.Z)
            # Que hay debajo (cajas)
            debajo, dentro = None, []
            if p is not None and z_base is not None:
                for otro, cj, nombre_cat in candidatos:
                    if otro.Id.Value == e.Id.Value:
                        continue
                    if not (cj.Min.X - pies(MARGEN_PLANTA_MM) <= p.X <= cj.Max.X + pies(MARGEN_PLANTA_MM) and cj.Min.Y - pies(MARGEN_PLANTA_MM) <= p.Y <= cj.Max.Y + pies(MARGEN_PLANTA_MM)):
                        continue
                    cima = mm(cj.Max.Z)
                    fondo = mm(cj.Min.Z)
                    if fondo < z_base - 1.0 and cima > z_base + DEBAJO_SOBRE_MM:
                        dentro.append((otro, nombre_cat, cima, fondo))
                        continue
                    if z_base - DEBAJO_HASTA_MM <= cima <= z_base + DEBAJO_SOBRE_MM:
                        if debajo is None or cima > debajo[2]:
                            debajo = (otro, nombre_cat, cima)
            texto_debajo = "nada hasta {0} mm por debajo".format(fmt(DEBAJO_HASTA_MM, 0))
            if debajo is not None:
                texto_debajo = "[{0}] {1} ({2}) cara superior Z {3}: {4} mm bajo la base".format(debajo[0].Id.Value, tipo_texto(debajo[0]), categoria_de(debajo[0]), fmt(debajo[2], 0), fmt(z_base - debajo[2], 0))
            if dentro:
                texto_debajo += " | la base esta DENTRO de: " + "; ".join("[{0}] {1} ({2}) Z {3}..{4}".format(o.Id.Value, tipo_texto(o), categoria_de(o), fmt(f, 0), fmt(c, 0)) for o, n, c, f in dentro[:3])
            # Rayo hacia abajo
            texto_rayo = ""
            if vista is not None and p is not None and z_base is not None:
                try:
                    from System.Collections.Generic import List
                    excluir = List[DB.ElementId]()
                    excluir.Add(e.Id)
                    filtro = DB.LogicalAndFilter(filtro_cats, DB.ExclusionFilter(excluir)) if filtro_cats is not None else DB.ExclusionFilter(excluir)
                    ri = DB.ReferenceIntersector(filtro, DB.FindReferenceTarget.Element, vista)
                    ri.FindReferencesInRevitLinks = True
                    origen_rayo = DB.XYZ(p.X, p.Y, pies(z_base) + pies(5.0))
                    golpe = ri.FindNearest(origen_rayo, DB.XYZ(0, 0, -1))
                    if golpe is None:
                        texto_rayo = " | rayo: nada"
                    else:
                        ref = golpe.GetReference()
                        dist = mm(golpe.Proximity) - 5.0
                        tocado = doc.GetElement(ref.ElementId)
                        if tocado is not None and isinstance(tocado, DB.RevitLinkInstance):
                            texto_rayo = " | rayo: elemento de un vinculo ({0}) a {1} mm".format(nombre_de(tocado), fmt(dist, 0))
                        elif tocado is not None:
                            texto_rayo = " | rayo: [{0}] {1} ({2}) a {3} mm".format(tocado.Id.Value, tipo_texto(tocado), categoria_de(tocado), fmt(dist, 0))
                        else:
                            texto_rayo = " | rayo: id {0} a {1} mm".format(ref.ElementId.Value, fmt(dist, 0))
                except Exception as error:
                    texto_rayo = " | rayo: ERROR {0}".format(error_texto(error))
            clave = (tipo_texto(e), material, round((z_base or 0.0) / 10.0) * 10.0, texto_debajo.split(" (")[0] if debajo is None else categoria_de(debajo[0]))
            resumen[clave] = resumen.get(clave, 0) + 1
            filas.append((e, p, z_base, canto, ancho, diametro, tipo_texto(e)))
            if detalle:
                print("   [{0}] {1} | {2}{3} | {4} x {5}{6} [{7}] | base {8} {9:+} = {10} (caja {11}) | cabeza {12} {13:+} = {14} | {15} mm | planta ({16}, {17}) giro {18} | {19} | DEBAJO: {20}{21}".format(
                    e.Id.Value, tipo_texto(e), material, " INCLINADO" if inclinado else "", fmt(canto), fmt(ancho), (" diam " + fmt(diametro)) if diametro > 0 else "", origen,
                    nb, db_, fmt(z_base_nivel, 0) if z_base_nivel is not None else "?", fmt(z_base_caja, 0) if z_base_caja is not None else "?",
                    nt, dt, fmt(et + dt, 0) if et is not None else "?", fmt(largo, 0),
                    fmt(mm(p.X), 0) if p is not None else "?", fmt(mm(p.Y), 0) if p is not None else "?", fmt(giro, 0) if giro is not None else "-",
                    rejillas_cercanas(rejillas, p) if p is not None else "-", texto_debajo, texto_rayo))
        except Exception as error:
            print("   [{0}] no se pudo leer: {1}".format(e.Id.Value, error_texto(error)))
    print("   RESUMEN de pilares por tipo, material, cota de base (10 mm) y que hay debajo:")
    for (tipo, material, z, debajo), n in sorted(resumen.items()):
        print("   {0:>4} x {1} | {2} | base Z ~ {3} | debajo: {4}".format(n, tipo, material, fmt(z, 0), debajo))
    return filas


# ----------------------------------------------------------------------------------------------------------------------------
# 2) Anclajes ya modelados como armazon
# ----------------------------------------------------------------------------------------------------------------------------
def anclajes_modelados(filas_pilares):
    print("2) ANCLAJES MODELADOS COMO ARMAZON ESTRUCTURAL (familia o tipo con ANCLA/ANCHOR/ESPARRAGO/PERNO/BOLT/STUD/VARILLA)")
    ejemplares = []
    por_tipo = {}
    for e in coleccionar(DB.BuiltInCategory.OST_StructuralFraming):
        if not isinstance(e, DB.FamilyInstance):
            continue
        texto = tipo_texto(e)
        if not PATRON_ANCLAJE.search(texto):
            continue
        loc = e.Location
        try:
            c = loc.Curve
            p0, p1 = c.GetEndPoint(0), c.GetEndPoint(1)
        except Exception:
            continue
        canto, ancho, diametro, origen = lados_mm(doc.GetElement(e.GetTypeId()))
        largo = mm(p0.DistanceTo(p1))
        vertical = abs((p1 - p0).Normalize().Z) > 0.97 if largo > 1 else False
        ejemplares.append((e, p0, p1, largo, vertical, diametro if diametro > 0 else max(canto, ancho), texto))
        por_tipo.setdefault(texto, []).append(ejemplares[-1])
    if not ejemplares:
        print("   ninguno")
        return
    for texto, lista in sorted(por_tipo.items()):
        print("   {0}: {1} ejemplares | diametro {2} mm | largos {3}..{4} mm | verticales {5}".format(
            texto, len(lista), fmt(lista[0][5]), fmt(min(l[3] for l in lista), 0), fmt(max(l[3] for l in lista), 0), sum(1 for l in lista if l[4])))
        for e, p0, p1, largo, vertical, diametro, _ in lista[:4]:
            print("      muestra [{0}] de ({1}, {2}, {3}) a ({4}, {5}, {6}) mm | {7} mm | {8}".format(
                e.Id.Value, fmt(mm(p0.X), 0), fmt(mm(p0.Y), 0), fmt(mm(p0.Z), 0), fmt(mm(p1.X), 0), fmt(mm(p1.Y), 0), fmt(mm(p1.Z), 0), fmt(largo, 0), "vertical" if vertical else "no vertical"))
    # Por pilar: los anclajes de su base
    print("   Anclajes por pilar (cabeza a menos de {0} mm de la cota de la base y a menos de {1} mm en planta):".format(fmt(ANCLAJE_RADIO_Z_MM, 0), fmt(ANCLAJE_RADIO_PLANTA_MM, 0)))
    con, sin = 0, 0
    recuento = {}
    for e, p, z_base, canto, ancho, diametro_pilar, texto_pilar in filas_pilares:
        if p is None or z_base is None:
            continue
        mios = []
        for a, p0, p1, largo, vertical, diametro, texto in ejemplares:
            arriba, abajo = max(p0.Z, p1.Z), min(p0.Z, p1.Z)
            centro = (p0 + p1) / 2.0
            dist_planta = mm(math.hypot(centro.X - p.X, centro.Y - p.Y))
            if dist_planta <= ANCLAJE_RADIO_PLANTA_MM and abs(mm(arriba) - z_base) <= ANCLAJE_RADIO_Z_MM:
                mios.append((a, dist_planta, mm(arriba) - z_base, z_base - mm(abajo), largo, diametro, centro, texto))
        if not mios:
            sin += 1
            continue
        con += 1
        xs = sorted(set(round(mm(m[6].X - p.X) / 5.0) * 5.0 for m in mios))
        ys = sorted(set(round(mm(m[6].Y - p.Y) / 5.0) * 5.0 for m in mios))
        diam = mios[0][5]
        recuento[(texto_pilar, len(mios), fmt(diam, 1))] = recuento.get((texto_pilar, len(mios), fmt(diam, 1)), 0) + 1
        print("   [{0}] {1}: {2} anclajes {3} | diam {4} | largo {5} mm | asoman {6} mm sobre la base, embebidos {7} mm | reparto respecto al pilar X {8} / Y {9} mm | ids {10}".format(
            e.Id.Value, texto_pilar, len(mios), mios[0][7], fmt(diam), fmt(mios[0][4], 0), fmt(max(m[2] for m in mios), 0), fmt(max(m[3] for m in mios), 0),
            [int(x) for x in xs], [int(y) for y in ys], [int(m[0].Id.Value) for m in mios][:8]))
    print("   pilares con anclajes: {0}; sin anclajes: {1}".format(con, sin))
    for (tipo, n, diam), veces in sorted(recuento.items()):
        print("   {0:>4} pilar(es) {1} con {2} anclajes de diam {3}".format(veces, tipo, n, diam))


# ----------------------------------------------------------------------------------------------------------------------------
# 3) Categorias de conexion de Revit
# ----------------------------------------------------------------------------------------------------------------------------
def categorias_conexion():
    print("3) CATEGORIAS DE CONEXION DE REVIT (getattr: sin dar por hecho que existan)")
    for nombre in SUBCATEGORIAS_CONEXION:
        c = bic(nombre)
        if c is None:
            print("   {0}: no existe en esta API".format(nombre))
            continue
        elementos = coleccionar(c)
        muestra = ""
        if elementos and "Anchor" in nombre:
            muestra = " | muestra: " + "; ".join("[{0}] {1}".format(x.Id.Value, tipo_texto(x)) for x in elementos[:5])
        print("   {0}: {1} elementos{2}".format(nombre, len(elementos), muestra))
    try:
        nombres = [n for n in System.Enum.GetNames(DB.BuiltInCategory) if PATRON_TIPOS.search(n)]
        print("   BuiltInCategory con Anchor/Grout en el nombre: {0}".format(", ".join(nombres) or "ninguna"))
    except Exception as error:
        print("   (no se pudieron listar las BuiltInCategory: {0})".format(error_texto(error)))


# ----------------------------------------------------------------------------------------------------------------------------
# 4) La API de Advance Steel por reflexion
# ----------------------------------------------------------------------------------------------------------------------------
def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name.lower() == nombre.lower():
                return a
        except Exception:
            pass
    return None


def ensamblados_acero():
    lista = []
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            n = a.GetName().Name
        except Exception:
            continue
        baja = n.lower()
        if (baja.startswith("as") and baja.endswith("mgd")) or "advancesteel" in baja or "steelconnections" in baja or baja == "revitapisteel":
            lista.append(a)
    return lista


def tipos_de(ensamblado):
    try:
        return list(ensamblado.GetExportedTypes())
    except Exception as error:
        tipos = getattr(getattr(error, "clsException", None), "Types", None)
        return [t for t in tipos if t is not None] if tipos is not None else []


def firma_parametros(metodo):
    try:
        return ", ".join("{0} {1}".format(p.ParameterType.Name, p.Name) for p in metodo.GetParameters())
    except Exception:
        return "?"


def cadena_herencia(tipo):
    partes = []
    t = tipo.BaseType
    while t is not None and len(partes) < 8:
        partes.append(t.Name)
        if t.Name in ("FilerObject", "Object"):
            break
        t = t.BaseType
    return " -> ".join(partes) or "-"


def volcar_tipo(tipo):
    from System.Reflection import BindingFlags
    print("   --- {0} [{1}] en {2}".format(tipo.FullName, "enum" if tipo.IsEnum else ("interfaz" if tipo.IsInterface else ("clase abstracta" if tipo.IsAbstract else "clase")), tipo.Assembly.GetName().Name))
    if tipo.IsEnum:
        try:
            print("       valores: {0}".format(", ".join(System.Enum.GetNames(tipo))))
        except Exception as error:
            print("       valores: ERROR {0}".format(error_texto(error)))
        return
    print("       hereda: {0}".format(cadena_herencia(tipo)))
    try:
        ctors = list(tipo.GetConstructors())
        print("       constructores ({0}):".format(len(ctors)))
        for c in ctors:
            print("         ctor({0})".format(firma_parametros(c)))
    except Exception as error:
        print("       constructores: ERROR {0}".format(error_texto(error)))
    try:
        estaticos = [m for m in tipo.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) if not m.IsSpecialName]
        if estaticos:
            print("       estaticos ({0}): {1}".format(len(estaticos), "; ".join("{0} {1}({2})".format(m.ReturnType.Name, m.Name, firma_parametros(m)) for m in estaticos[:MAX_MIEMBROS])))
    except Exception:
        pass
    try:
        metodos = [m for m in tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) if not m.IsSpecialName]
        relevantes = [m for m in metodos if PATRON_MIEMBROS.search(m.Name)]
        print("       metodos de instancia declarados: {0} (relevantes {1}, se muestran {2}):".format(len(metodos), len(relevantes), min(len(relevantes), MAX_MIEMBROS)))
        for m in sorted(relevantes, key=lambda x: x.Name)[:MAX_MIEMBROS]:
            print("         {0} {1}({2})".format(m.ReturnType.Name, m.Name, firma_parametros(m)))
    except Exception as error:
        print("       metodos: ERROR {0}".format(error_texto(error)))
    try:
        props = []
        t = tipo
        while t is not None and t.Name not in ("FilerObject", "Object") and len(props) < 200:
            for p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly):
                props.append("{0}:{1}{2}".format(p.Name, p.PropertyType.Name, "" if p.CanWrite else " (solo lectura)"))
            t = t.BaseType
        print("       propiedades (propias y heredadas hasta FilerObject, {0}): {1}".format(len(props), ", ".join(sorted(props)[:80])))
    except Exception as error:
        print("       propiedades: ERROR {0}".format(error_texto(error)))


def api_advance_steel():
    print("4) LA API DE ADVANCE STEEL (reflexion, sin transaccion)")
    carpeta_revit = os.path.dirname(str(clr.GetClrType(DB.Element).Assembly.Location))
    carpeta_steel = os.path.join(carpeta_revit, "AddIns", "SteelConnections")
    cargados = ensamblados_acero()
    print("   ensamblados de acero ya cargados ({0}): {1}".format(len(cargados), ", ".join(sorted(a.GetName().Name for a in cargados))))
    print("   carpeta {0}: existe={1}".format(carpeta_steel, os.path.isdir(carpeta_steel)))
    if os.path.isdir(carpeta_steel):
        archivos = sorted(f for f in os.listdir(carpeta_steel) if f.lower().endswith(".dll"))
        con_patron = [f for f in archivos if PATRON_TIPOS.search(f)]
        print("   DLL con Anchor/Grout en el nombre ({0}): {1}".format(len(con_patron), ", ".join("{0} ({1} KB)".format(f, os.path.getsize(os.path.join(carpeta_steel, f)) // 1024) for f in con_patron) or "ninguna"))
        gestionadas = [f for f in archivos if re.search(r"^AS.*Mgd.*\.dll$", f, re.IGNORECASE) or (PATRON_TIPOS.search(f) and "mgd" in f.lower())]
        print("   DLL gestionadas candidatas ({0}): {1}".format(len(gestionadas), ", ".join(gestionadas)))
        for f in gestionadas:
            base = os.path.splitext(f)[0]
            if buscar_ensamblado(base) is not None:
                continue
            try:
                clr.AddReferenceToFileAndPath(os.path.join(carpeta_steel, f))
                print("   cargada por ruta: {0} -> {1}".format(f, "si" if buscar_ensamblado(base) is not None else "no aparece en el AppDomain con ese nombre"))
            except Exception as error:
                print("   {0}: no se pudo cargar ({1})".format(f, error_texto(error)))
    ensamblados = ensamblados_acero()
    # 4a) Tipos con Anchor o Grout en el nombre (tambien los anidados: Tipo+eAnchorType)
    hallados = []
    patrones_bolt = []
    for a in ensamblados:
        for t in tipos_de(a):
            if PATRON_TIPOS.search(t.FullName or t.Name or ""):
                hallados.append(t)
            elif re.search(r"BoltPattern|ScrewBoltPattern|Bolt$", t.Name or ""):
                patrones_bolt.append(t)
    hallados.sort(key=lambda t: t.FullName)
    print("4a) Tipos con 'Anchor' o 'Grout' en el nombre en {0} ensamblados: {1}".format(len(ensamblados), len(hallados)))
    for t in hallados[:MAX_TIPOS_PATRON]:
        volcar_tipo(t)
    if len(hallados) > MAX_TIPOS_PATRON:
        print("   ... y {0} tipos mas (solo nombres): {1}".format(len(hallados) - MAX_TIPOS_PATRON, ", ".join(t.FullName for t in hallados[MAX_TIPOS_PATRON:])))
    # 4b) Donde cuelgan los patrones de pernos (para ver si el anclaje es un BoltPattern mas)
    print("4b) Tipos *BoltPattern* / *Bolt ({0}): {1}".format(len(patrones_bolt), "; ".join("{0} <- {1}".format(t.Name, t.BaseType.Name if t.BaseType is not None else "-") for t in sorted(patrones_bolt, key=lambda x: x.FullName)[:40])))
    # 4c) Miembros llamados *Anchor* o *Grout* en cualquier otro tipo de los ensamblados de Advance Steel
    print("4c) Miembros con Anchor/Grout en el nombre en los demas tipos (maximo 60):")
    from System.Reflection import BindingFlags
    flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
    vistos = 0
    for a in ensamblados:
        nombre_a = a.GetName().Name
        for t in tipos_de(a):
            if t in hallados:
                continue
            try:
                miembros = [m for m in t.GetMembers(flags) if PATRON_TIPOS.search(m.Name)]
            except Exception:
                continue
            for m in miembros:
                if vistos >= 60:
                    break
                detalle = ""
                try:
                    if hasattr(m, "GetParameters"):
                        detalle = "({0}) -> {1}".format(firma_parametros(m), m.ReturnType.Name)
                    elif hasattr(m, "PropertyType"):
                        detalle = ": " + m.PropertyType.Name
                except Exception:
                    pass
                print("   {0}.{1}{2} [{3}]".format(t.FullName, m.Name, detalle, nombre_a))
                vistos += 1
    if vistos == 0:
        print("   ninguno")
    # 4d) El lado de Revit: Autodesk.Revit.DB.Steel y las clases de conexion con Anchor
    revit_api = clr.GetClrType(DB.Element).Assembly
    steel_rvt = [t.FullName for t in tipos_de(revit_api) if PATRON_TIPOS.search(t.Name or "")]
    print("4d) Tipos de RevitAPI.dll con Anchor/Grout en el nombre: {0}".format(", ".join(steel_rvt) or "ninguno"))


def main():
    print("Sondeo 32: pilares, bases y anclajes (Fase 14, para la Fase 15). Documento: {0}. Solo lectura.".format(doc.Title))
    filas = []
    try:
        filas = pilares_y_bases()
    except Exception as error:
        print("   FALLO en 1): {0}".format(error_texto(error)))
        print(traceback.format_exc())
    try:
        anclajes_modelados(filas)
    except Exception as error:
        print("   FALLO en 2): {0}".format(error_texto(error)))
        print(traceback.format_exc())
    try:
        categorias_conexion()
    except Exception as error:
        print("   FALLO en 3): {0}".format(error_texto(error)))
        print(traceback.format_exc())
    try:
        api_advance_steel()
    except Exception as error:
        print("   FALLO en 4): {0}".format(error_texto(error)))
        print(traceback.format_exc())
    print("Fin del sondeo 32.")


main()
