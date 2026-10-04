# -*- coding: utf-8 -*-
# Sondeo 16 (rondas 6b/6c): mide si los pernos de la placa cuchilla atraviesan de verdad la cartela y la placa.
# Necesita una conexion del Detalle D ya creada (desde la ventana de la cinta o con el sondeo 11). La especificacion se
# lee del FIXTURE en disco (como el sondeo 11), no de conn_get: en la ronda 6b, reenviar a validate el texto que
# devolvia el Bridge fallo con UnicodeDecodeError por la "a" con tilde de uncertain_fields (json.dumps en IronPython).
# Para cada elemento
# creado proyecta su geometria sobre el eje Z local del nudo (normal al plano de la cercha) y escribe el intervalo
# [z_min, z_max] en mm. Lo esperado con el fixture (cartela 9,525 mm, placa 10 mm, gusset_face +z):
#   cartela        -4.76 .. +4.76
#   placa cuchilla +4.76 .. +14.76   (apoyada en la cara +Z de la cartela, ya no en su mismo plano)
#   pernos         deben cubrir -4.76 .. +14.76 (agarre 19,5 mm) con cabeza por un lado y tuerca por el otro
# Ademas lee los parametros Bolt Length y Grip Length (se esperan 44.45 y 19.53 mm) y exporta una captura de perfil
# de la placa cuchilla (docs\fases\capturas\fase6b-03-pernos-perfil.png). No borra nada: el sondeo 12 lo hace.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\16-pernos-agarre.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import glob
import json
import os
from System.Collections.Generic import List

NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"
MM_POR_PIE = 304.8
CARPETA_CAPTURAS = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas"
NOMBRE_CAPTURA = "fase6b-03-pernos-perfil"
ID_BARRA_CUCHILLA = 1249636   # diagonal inferior del Detalle D (members[2])
RUTA_FIXTURE = r"D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json"


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def v_restar(a, b):
    return (a[0] - b[0], a[1] - b[1], a[2] - b[2])


def v_sumar(a, b):
    return (a[0] + b[0], a[1] + b[1], a[2] + b[2])


def v_escalar(a, k):
    return (a[0] * k, a[1] * k, a[2] * k)


def v_punto(a, b):
    return a[0] * b[0] + a[1] * b[1] + a[2] * b[2]


def v_cruz(a, b):
    return (a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0])


def v_norma(a):
    return (a[0] * a[0] + a[1] * a[1] + a[2] * a[2]) ** 0.5


def v_normalizar(a):
    n = v_norma(a)
    if n < 1e-12:
        raise ValueError("vector nulo")
    return (a[0] / n, a[1] / n, a[2] / n)


def xyz_mm(p):
    return (p.X * MM_POR_PIE, p.Y * MM_POR_PIE, p.Z * MM_POR_PIE)


def xyz_pies(t):
    return DB.XYZ(t[0] / MM_POR_PIE, t[1] / MM_POR_PIE, t[2] / MM_POR_PIE)


print("=== 16-pernos-agarre ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit

ensamblado = buscar_ensamblado(NOMBRE_ENSAMBLADO)
if ensamblado is None:
    print("PARADA: MotorConexiones.Revit no esta cargado.")
    raise SystemExit
tipo = ensamblado.GetType("MotorConexiones.Revit.Bridge")
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UI.UIDocument)])
handle = tipo.GetMethod("Handle", firma)
print("Bridge.Handle encontrado: {0} | version del ensamblado: {1}".format(handle is not None, ensamblado.GetName().Version))


def llamar(operacion, cuerpo):
    argumentos = System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc])
    try:
        return json.loads(handle.Invoke(None, argumentos))
    except Exception as error:
        interna = getattr(error, "InnerException", None)
        print("--- {0}: EXCEPCION {1}: {2}".format(operacion, type(error).__name__, str(interna or error)[:300]))
        return {"ok": False, "data": None, "errors": [], "warnings": []}


# 1) La conexion creada y su especificacion
r = llamar("list", {})
conexiones = (r.get("data") or {}).get("connections") or []
print("1) conexiones en el modelo: {0}".format(len(conexiones)))
if not conexiones:
    print("PARADA: no hay ninguna conexion. Crea el Detalle D desde la ventana (o con el sondeo 11) y repite.")
    raise SystemExit
conexion = conexiones[0].get("connection_id")
r = llamar("get", {"connection_id": conexion})
datos = r.get("data") or {}
ids_creados = datos.get("created_element_ids") or []
if not os.path.isfile(RUTA_FIXTURE):
    print("PARADA: no existe el fixture " + RUTA_FIXTURE)
    raise SystemExit
with open(RUTA_FIXTURE, "r") as archivo:
    spec = json.load(archivo)   # unicode limpio; la conexion se creo desde este mismo archivo
print("   connection_id={0} | backend={1} | elementos creados={2}".format(conexion, datos.get("backend"), len(ids_creados)))
gusset_t = float(((spec.get("gusset") or {}).get("thickness_mm")) or 9.525)
miembro_cuchilla = None
for m in spec.get("members") or []:
    if ((m.get("attachment") or {}).get("type")) == "bolted_knife_plate":
        miembro_cuchilla = m
        break
placa = ((miembro_cuchilla or {}).get("attachment") or {}).get("plate") or {}
pernos = ((miembro_cuchilla or {}).get("attachment") or {}).get("bolts") or {}
placa_t = float(placa.get("thickness_mm") or 10.0)
cara = placa.get("gusset_face") or "+z"
lado = -1.0 if str(cara).strip().lower() == "-z" else 1.0
agarre = gusset_t + placa_t
z_min_paquete = -gusset_t / 2.0 if lado > 0 else -(gusset_t / 2.0 + placa_t)
z_max_paquete = gusset_t / 2.0 + placa_t if lado > 0 else gusset_t / 2.0
print("   cartela {0} mm | placa cuchilla {1} mm | cara {2} | agarre esperado {3:.2f} mm | paquete Z esperado {4:.2f} .. {5:.2f}".format(
    gusset_t, placa_t, cara, agarre, z_min_paquete, z_max_paquete))

# 2) Sistema local del nudo (el mismo que usa el add-in) y validate para ver bolt_stacks
ids_nudo = (spec.get("node") or {}).get("element_ids") or []
r = llamar("node_info", {"element_ids": ids_nudo, "chord_element_id": (spec.get("chord") or {}).get("element_id")})
info = r.get("data") or {}
origen = tuple(info.get("origin_mm") or (0, 0, 0))
eje_x = tuple(info.get("x_axis") or (1, 0, 0))
eje_y = tuple(info.get("y_axis") or (0, 1, 0))
eje_z = tuple(info.get("z_axis") or v_cruz(eje_x, eje_y))
print("2) origen {0} | Z local (normal a la cercha) = {1}".format(
    tuple(round(c, 1) for c in origen), tuple(round(c, 3) for c in eje_z)))
r = llamar("validate", {"spec": spec})
print("   validate bolt_stacks: " + repr((r.get("data") or {}).get("bolt_stacks")))

# 3) Geometria de cada elemento creado proyectada en Z local
def solidos_de(elemento):
    """Solidos con volumen de la geometria del elemento (detalle fino; si no hay, con la vista activa)."""
    encontrados = []

    def recoger(geometria):
        if geometria is None:
            return
        for g in geometria:
            try:
                if isinstance(g, DB.Solid):
                    if g.Volume > 1e-9:
                        encontrados.append(g)
                elif isinstance(g, DB.GeometryInstance):
                    recoger(g.GetInstanceGeometry())
            except Exception:
                pass

    try:
        op = DB.Options()
        op.DetailLevel = DB.ViewDetailLevel.Fine
        op.ComputeReferences = False
        op.IncludeNonVisibleObjects = True
        recoger(elemento.get_Geometry(op))
    except Exception as error:
        print("      get_Geometry(fino): ERROR " + str(error)[:120])
    if not encontrados:
        try:
            op2 = DB.Options()
            op2.View = doc.ActiveView
            op2.ComputeReferences = False
            recoger(elemento.get_Geometry(op2))
        except Exception as error:
            print("      get_Geometry(vista activa): ERROR " + str(error)[:120])
    return encontrados


def rango_z(solidos):
    """[z_min, z_max] en mm de los vertices de los solidos sobre el eje Z local, y numero de vertices."""
    z_min, z_max, n = None, None, 0
    for s in solidos:
        try:
            for arista in s.Edges:
                for p in arista.AsCurve().Tessellate():
                    z = v_punto(v_restar(xyz_mm(p), origen), eje_z)
                    n += 1
                    z_min = z if z_min is None or z < z_min else z_min
                    z_max = z if z_max is None or z > z_max else z_max
        except Exception:
            pass
    return z_min, z_max, n


NOMBRES_MEDIDAS = ("Thickness", "Length", "Width", "Diameter", "Bolt Length", "Grip Length", "Number on side 1", "Number on side 2")


def medidas_acero(elemento):
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
                if p.StorageType == DB.StorageType.Double:
                    lineas.append("{0}: {1} = {2} mm".format(nombre, p.AsValueString(), round(p.AsDouble() * MM_POR_PIE, 2)))
                elif p.StorageType == DB.StorageType.Integer:
                    lineas.append("{0}: {1}".format(nombre, p.AsInteger()))
                else:
                    lineas.append("{0}: {1}".format(nombre, p.AsValueString() or p.AsString()))
            except Exception as error:
                lineas.append("{0}: ERROR {1}".format(nombre, str(error)[:80]))
            break
    return lineas


print("3) intervalo Z local (mm) de cada elemento creado:")
rangos = {}
for eid in ids_creados:
    el = doc.GetElement(DB.ElementId(System.Int64(eid)))
    if el is None:
        print("   [{0}] no existe".format(eid))
        continue
    categoria = el.Category.Name if el.Category is not None else "-"
    nombre = el.GetType().Name
    solidos = solidos_de(el)
    z_min, z_max, n = rango_z(solidos)
    if z_min is None:
        print("   [{0}] {1} | {2}: SIN GEOMETRIA LEGIBLE ({3} solidos)".format(eid, nombre, categoria, len(solidos)))
    else:
        print("   [{0}] {1} | {2}: z {3:.2f} .. {4:.2f} mm (espesor en Z {5:.2f}; {6} solidos, {7} vertices)".format(
            eid, nombre, categoria, z_min, z_max, z_max - z_min, len(solidos), n))
        rangos[eid] = (categoria, nombre, z_min, z_max)
    if nombre == "SteelProxyElement":
        for linea in medidas_acero(el):
            print("        " + linea)

# 4) Veredicto
placas = [(e, r) for e, r in rangos.items() if r[0] == "Plates" or (r[1] == "DirectShape" and abs((r[3] - r[2]) - gusset_t) < 1.0) or (r[1] == "DirectShape" and abs((r[3] - r[2]) - placa_t) < 1.0)]
pernos_rangos = [(e, r) for e, r in rangos.items() if r[0] == "Bolts" or (r[1] == "DirectShape" and (r[3] - r[2]) > agarre + 5.0)]
print("4) veredicto:")
cartela_ok = placa_ok = None
for e, r in placas:
    esp = r[3] - r[2]
    if abs(esp - gusset_t) < 0.6 and abs(r[2] + gusset_t / 2.0) < 0.6:
        cartela_ok = True
        print("   cartela [{0}]: centrada en el plano de la cercha ({1:.2f} .. {2:.2f}) OK".format(e, r[2], r[3]))
    elif abs(esp - placa_t) < 0.6:
        placa_ok = abs(r[2] - (gusset_t / 2.0 if lado > 0 else -(gusset_t / 2.0 + placa_t))) < 0.6
        print("   placa cuchilla [{0}]: {1:.2f} .. {2:.2f} -> {3}".format(
            e, r[2], r[3], "apoya en la cara {0} de la cartela OK".format(cara) if placa_ok else "NO esta donde se esperaba ({0:.2f} .. {1:.2f})".format(
                gusset_t / 2.0 if lado > 0 else -(gusset_t / 2.0 + placa_t), gusset_t / 2.0 + placa_t if lado > 0 else -gusset_t / 2.0)))
if not pernos_rangos:
    print("   pernos: sin geometria legible; usa los parametros Bolt Length / Grip Length y la captura para juzgar.")
for e, r in pernos_rangos:
    cubre = r[2] <= z_min_paquete + 0.6 and r[3] >= z_max_paquete - 0.6
    print("   pernos [{0}]: z {1:.2f} .. {2:.2f} (largo total {3:.2f} mm) -> {4}".format(
        e, r[2], r[3], r[3] - r[2],
        "atraviesan cartela + placa OK; sobresalen {0:.1f} mm por abajo y {1:.1f} mm por arriba".format(z_min_paquete - r[2], r[3] - z_max_paquete)
        if cubre else "NO cubren el paquete {0:.2f} .. {1:.2f}: falta {2:.1f} mm abajo / {3:.1f} mm arriba".format(
            z_min_paquete, z_max_paquete, max(0.0, r[2] - z_min_paquete), max(0.0, z_max_paquete - r[3]))))
    sobresale = max(0.0, z_min_paquete - r[2]) + max(0.0, r[3] - z_max_paquete)
    if sobresale > 3.0 * agarre:
        print("   AVISO: los pernos sobresalen {0:.1f} mm en total, mas de tres veces el agarre: la longitud no se esta aplicando.".format(sobresale))

# 5) Captura de perfil de la placa cuchilla (vista a lo largo de la barra, Z local vertical) — mejor esfuerzo
try:
    if miembro_cuchilla is not None:
        u = None
        for m in info.get("members") or []:
            if m.get("element_id") == miembro_cuchilla.get("element_id"):
                ini, fin = tuple(m.get("start_mm")), tuple(m.get("end_mm"))
                d = v_normalizar(v_restar(fin, ini))
                # hacia fuera del punto de trabajo
                if v_norma(v_restar(ini, origen)) > v_norma(v_restar(fin, origen)):
                    d = v_escalar(d, -1.0)
                u = d
        if u is None:
            raise ValueError("la barra de la placa cuchilla no esta en node_info")
        retiro = float(miembro_cuchilla.get("end_setback_mm") or 0.0)
        largo = float(placa.get("length_mm") or 170.0)
        insercion = float(placa.get("insertion_mm") or 80.0)
        primera_fila = float(pernos.get("first_row_from_plate_end_mm") or 40.0)
        a_lo_largo = retiro - (largo - insercion) + primera_fila   # primera fila de pernos (fuera del HSS)
        centro = v_sumar(v_sumar(origen, v_escalar(u, a_lo_largo)), v_escalar(eje_z, (z_min_paquete + z_max_paquete) / 2.0))
        v = v_normalizar(v_cruz(eje_z, u))
        tipo_3d = None
        for vft in DB.FilteredElementCollector(doc).OfClass(DB.ViewFamilyType):
            if vft.ViewFamily == DB.ViewFamily.ThreeDimensional:
                tipo_3d = vft
                break
        t = DB.Transaction(doc, "Vista de captura MotorConexiones (perfil pernos)")
        t.Start()
        vista = None
        for vw in DB.FilteredElementCollector(doc).OfClass(DB.View3D):
            if vw.Name == "Captura_MotorConexiones_pernos" and not vw.IsTemplate:
                vista = vw
                break
        if vista is None:
            vista = DB.View3D.CreateIsometric(doc, tipo_3d.Id)
            vista.Name = "Captura_MotorConexiones_pernos"
        ojo = xyz_pies(v_sumar(centro, v_escalar(v, 900.0)))
        objetivo = xyz_pies(centro)
        adelante = (objetivo - ojo).Normalize()
        arriba = xyz_pies(v_sumar(centro, v_escalar(eje_z, 1000.0))) - objetivo
        arriba = arriba.Normalize()
        # arriba debe ser perpendicular a adelante
        derecha = adelante.CrossProduct(arriba).Normalize()
        arriba = derecha.CrossProduct(adelante).Normalize()
        vista.SetOrientation(DB.ViewOrientation3D(ojo, arriba, adelante))
        caja = DB.BoundingBoxXYZ()
        tr = DB.Transform.Identity
        tr.Origin = objetivo
        tr.BasisX = xyz_pies(u) - DB.XYZ(0, 0, 0)
        tr.BasisY = xyz_pies(v) - DB.XYZ(0, 0, 0)
        tr.BasisZ = xyz_pies(eje_z) - DB.XYZ(0, 0, 0)
        caja.Transform = tr
        caja.Min = DB.XYZ(-110.0 / MM_POR_PIE, -140.0 / MM_POR_PIE, -70.0 / MM_POR_PIE)
        caja.Max = DB.XYZ(110.0 / MM_POR_PIE, 140.0 / MM_POR_PIE, 70.0 / MM_POR_PIE)
        vista.IsSectionBoxActive = True
        vista.SetSectionBox(caja)
        vista.DetailLevel = DB.ViewDetailLevel.Fine
        vista.DisplayStyle = DB.DisplayStyle.ShadingWithEdges
        t.Commit()
        if not os.path.isdir(CARPETA_CAPTURAS):
            os.makedirs(CARPETA_CAPTURAS)
        base = os.path.join(CARPETA_CAPTURAS, NOMBRE_CAPTURA)
        opciones = DB.ImageExportOptions()
        opciones.FilePath = base
        opciones.ExportRange = DB.ExportRange.SetOfViews
        ids = List[DB.ElementId]()
        ids.Add(vista.Id)
        opciones.SetViewsAndSheets(ids)
        opciones.HLRandWFViewsFileType = DB.ImageFileType.PNG
        opciones.ShadowViewsFileType = DB.ImageFileType.PNG
        opciones.ImageResolution = DB.ImageResolution.DPI_150
        opciones.ZoomType = DB.ZoomFitType.FitToPage
        opciones.PixelSize = 1600
        doc.ExportImage(opciones)
        destino = base + ".png"
        for g in glob.glob(base + "*"):
            if g != destino and g.lower().endswith(".png"):
                if os.path.isfile(destino):
                    os.remove(destino)
                os.rename(g, destino)
            elif not g.lower().endswith(".png"):
                os.remove(g)
        print("5) captura de perfil: {0} ({1})".format(destino, "OK" if os.path.isfile(destino) else "NO SE GENERO"))
except Exception as error:
    print("5) captura de perfil: no se pudo ({0}). Haz la captura a mano (paso de la persona).".format(str(error)[:160]))
print("=== fin 16-pernos-agarre ===")
