# -*- coding: utf-8 -*-
# Sondeo 10: grupo de 4 pernos (2x2, 60 mm, diam. 5/8") de Advance Steel en el nudo seleccionado, por el camino A.
# Segunda version (ronda 1c): la posicion sale del sistema local del nudo seleccionado (igual que el sondeo 09),
# no de la caja de la placa, porque los SteelProxyElement devuelven BoundingBox nulo. Firmas tomadas del volcado real
# de la ronda 1b: FinitRectScrewBoltPattern(Point3d ptRef, Point3d ptRef2, Vector3d vX, Vector3d vY) con
# propiedades Nx, Ny, Dx, Dy, ScrewDiameter, ScrewLength. Todo dentro de FabricationTransaction, sobre la copia.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\10-pernos-camino-a.py -SinTransaccion
from __future__ import print_function
import re

UNIDAD_AS = "pies"          # confirmado en la ronda 1b: Advance Steel dentro de Revit trabaja en pies
DIAMETRO_MM, SEPARACION_MM, LONGITUD_MM = 15.875, 60.0, 40.0

# --- Sistema local del nudo (seccion 7 del encargo; misma formula que Core/Geometry3D/NodeFrame.cs) ---
MM_POR_PIE = 304.8


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


def v_mm(xyz):
    return (xyz.X * MM_POR_PIE, xyz.Y * MM_POR_PIE, xyz.Z * MM_POR_PIE)


def v_texto(a):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(a[0], a[1], a[2])


def calcular_marco(c0, c1, m0, m1):
    """Origen, X, Y, Z (mm y unitarios) y distancia entre ejes. Lanza ValueError si son paralelos."""
    x = v_normalizar(v_restar(c1, c0))
    d = v_normalizar(v_restar(m1, m0))
    cr = v_cruz(x, d)
    if v_norma(cr) < 1e-6:
        raise ValueError("el cordon y el primer miembro son paralelos")
    w = v_restar(c0, m0)
    b = v_punto(x, d)
    dd = v_punto(x, w)
    e = v_punto(d, w)
    den = 1.0 - b * b
    s = (b * e - dd) / den
    t = (e - b * dd) / den
    p = v_sumar(c0, v_escalar(x, s))
    q = v_sumar(m0, v_escalar(d, t))
    dist = v_norma(v_restar(p, q))
    origen = v_escalar(v_sumar(p, q), 0.5)
    z = v_normalizar(cr)
    if abs(z[2]) > 1e-4:  # misma tolerancia que NodeFrame.VerticalComponentTolerance
        if z[2] < 0:
            z = v_escalar(z, -1.0)
    elif z[1] < 0:
        z = v_escalar(z, -1.0)
    y = v_normalizar(v_cruz(z, x))
    return origen, x, y, z, dist


def leer_miembros_seleccion(doc, uidoc):
    """Miembros de armazon estructural seleccionados: lista de dict con id, familia, tipo, inicio, fin (mm), pendiente."""
    miembros = []
    id_categoria = DB.ElementId(DB.BuiltInCategory.OST_StructuralFraming)
    for eid in uidoc.Selection.GetElementIds():
        el = doc.GetElement(eid)
        if el is None:
            continue
        try:
            es_barra = el.Category is not None and el.Category.Id == id_categoria and isinstance(el, DB.FamilyInstance)
        except Exception:
            es_barra = False
        try:
            curva = el.Location.Curve if es_barra and isinstance(el.Location, DB.LocationCurve) else None
        except Exception:
            curva = None
        if curva is None:
            print("   (se ignora {0}: no es armazon estructural con eje)".format(eid.Value))
            continue
        ini = v_mm(curva.GetEndPoint(0))
        fin = v_mm(curva.GetEndPoint(1))
        direccion = v_normalizar(v_restar(fin, ini))
        import math
        pendiente = math.degrees(math.asin(min(1.0, abs(direccion[2]))))
        miembros.append({
            "id": eid.Value, "familia": el.Symbol.FamilyName, "tipo": el.Symbol.Name if hasattr(el.Symbol, "Name") else DB.Element.Name.__get__(el.Symbol),
            "ini": ini, "fin": fin, "longitud": v_norma(v_restar(fin, ini)), "pendiente": pendiente,
        })
    return miembros


def elegir_cordon(miembros):
    """El mas horizontal (menor pendiente) y, a igualdad, el mas largo. Devuelve (cordon, resto)."""
    ordenados = sorted(miembros, key=lambda m: (round(m["pendiente"], 3), -m["longitud"]))
    cordon = ordenados[0]
    resto = [m for m in miembros if m is not cordon]
    return cordon, resto
# --- fin sistema local ---


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


def nombres_parametros(metodo):
    return [p.ParameterType.Name for p in metodo.GetParameters()]


def buscar_ctor(tipo, nombres):
    for c in tipo.GetConstructors():
        if nombres_parametros(c) == list(nombres):
            return c
    return None


def a_unidad_as(mm):
    return mm / MM_POR_PIE if UNIDAD_AS == "pies" else mm


print("=== 10-pernos-camino-a ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt'.")
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True).")
    raise SystemExit

# 1) Transaccion de acero (mismo criterio que 09)
tipo_tx = ctor_tx = args_tx = None
for nombre_ens in ("RevitAPISteel", "Autodesk.SteelConnectionsDB"):
    ens = buscar_ensamblado(nombre_ens)
    if ens is None:
        continue
    for t in tipos_de(ens):
        if not (t.IsClass and re.search(r"Transaction", t.Name) and re.search(r"Fabrication|Steel", t.FullName, re.IGNORECASE)):
            continue
        for c in t.GetConstructors():
            if ctor_tx is None and nombres_parametros(c) == ["Document", "Boolean", "String"]:
                tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo pernos A"]
if ctor_tx is None:
    print("PARADA: sin contexto de acero no se crea nada.")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
print("1) Transaccion: {0}".format(tipo_tx.FullName))

# 2) Tipo del patron y geometria desde sus parametros (mismo contexto de carga)
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit.")
    raise SystemExit
T_Patron = obj.GetType("Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern")
ctor_patron = buscar_ctor(T_Patron, ["Point3d", "Point3d", "Vector3d", "Vector3d"]) if T_Patron is not None else None
if ctor_patron is None:
    print("PARADA: no existe FinitRectScrewBoltPattern(Point3d, Point3d, Vector3d, Vector3d).")
    raise SystemExit
parametros = ctor_patron.GetParameters()
T_Point3d = parametros[0].ParameterType
T_Vector3d = parametros[2].ParameterType
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
metodo_write = T_Patron.GetMethod("WriteToDb")
if None in (ctor_p, ctor_v, metodo_write):
    print("PARADA: faltan Point3d/Vector3d(d,d,d) o WriteToDb.")
    raise SystemExit
from System.Reflection import BindingFlags
props = {}
t = T_Patron
while t is not None and t.FullName.startswith("Autodesk.AdvanceSteel"):
    for p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance):
        props.setdefault(p.Name, p)
    t = t.BaseType
print("2) FinitRectScrewBoltPattern listo; propiedades disponibles: Nx={0} Ny={1} Dx={2} Dy={3} ScrewDiameter={4} ScrewLength={5}".format(
    *["Nx" in props, "Ny" in props, "Dx" in props, "Dy" in props, "ScrewDiameter" in props, "ScrewLength" in props]))

# 3) Posicion: sistema local del nudo seleccionado (el mismo que uso el sondeo 09 para la placa)
miembros = leer_miembros_seleccion(doc, uidoc)
if len(miembros) < 2:
    print("PARADA: selecciona en Revit el cordon y al menos una diagonal del nudo ({0} miembros validos).".format(len(miembros)))
    raise SystemExit
cordon, resto = elegir_cordon(miembros)
origen, x, y, z, dist = calcular_marco(cordon["ini"], cordon["fin"], resto[0]["ini"], resto[0]["fin"])
print("3) Nudo: cordon [{0}], origen {1} mm, normal {2}".format(cordon["id"], v_texto(origen), v_texto(z)))
medio = SEPARACION_MM / 2.0
p1 = v_sumar(v_sumar(origen, v_escalar(x, -medio)), v_escalar(y, -medio))
p2 = v_sumar(v_sumar(origen, v_escalar(x, medio)), v_escalar(y, medio))
print("   esquinas del patron (mm): {0} y {1}".format(v_texto(p1), v_texto(p2)))


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([a_unidad_as(p[0]), a_unidad_as(p[1]), a_unidad_as(p[2])]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v[0], v[1], v[2]]))


def fijar(patron, nombre, valor):
    prop = props.get(nombre)
    if prop is None or not prop.CanWrite:
        return "{0}: no existe o es de solo lectura".format(nombre)
    try:
        prop.SetValue(patron, System.Convert.ChangeType(valor, prop.PropertyType), None)
        return "{0}={1}".format(nombre, valor)
    except Exception as error:
        return "{0}: ERROR {1}".format(nombre, str(error)[:100])


ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    print("4) FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    patron = ctor_patron.Invoke(System.Array[System.Object]([punto_as(p1), punto_as(p2), vector_as(x), vector_as(y)]))
    print("   Patron creado en memoria: {0}".format(patron.GetType().FullName))
    resultados = [fijar(patron, "Nx", 2), fijar(patron, "Ny", 2),
                  fijar(patron, "Dx", a_unidad_as(SEPARACION_MM)), fijar(patron, "Dy", a_unidad_as(SEPARACION_MM)),
                  fijar(patron, "ScrewDiameter", a_unidad_as(DIAMETRO_MM)), fijar(patron, "ScrewLength", a_unidad_as(LONGITUD_MM))]
    print("   propiedades: " + "; ".join(resultados))
    try:
        print("   NumberOfScrews antes de escribir: {0}".format(props["NumberOfScrews"].GetValue(patron, None)))
    except Exception:
        pass
    metodo_write.Invoke(patron, None)
    print("   WriteToDb() OK")
    tipo_tx.GetMethod("Commit").Invoke(tx, None)
    print("   Commit() OK")
    tx = None
except Exception as error:
    interna = getattr(error, "InnerException", None)
    print("   ERROR {0}: {1}".format(type(error).__name__, str(error)[:400]))
    if interna is not None:
        print("   interna: " + str(interna)[:400])
    try:
        if tx is not None and metodo_cancelar is not None:
            metodo_cancelar.Invoke(tx, None)
            print("   CancelTransaction() hecho")
    except Exception as e2:
        print("   CancelTransaction ERROR " + str(e2)[:200])
finally:
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0}".format(doc.IsModifiable))

ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
nuevos = sorted(ids_despues - ids_antes)
print("5) Elementos nuevos: {0}".format(len(nuevos)))
for i in nuevos[:20]:
    el = doc.GetElement(DB.ElementId(System.Int64(i)))
    try:
        cat = el.Category.Name if el.Category is not None else "-"
    except Exception:
        cat = "?"
    print("   [{0}] {1} | {2}".format(i, el.GetType().Name, cat))
if nuevos:
    try:
        doc.Save()
        print("6) Copia guardada: {0}".format(doc.PathName))
    except Exception as error:
        print("6) No se pudo guardar: " + str(error)[:200])
print("=== fin 10-pernos-camino-a ===")
