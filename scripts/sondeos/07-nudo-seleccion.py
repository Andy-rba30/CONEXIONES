# -*- coding: utf-8 -*-
# Sondeo 07: leer el nudo SELECCIONADO en Revit (cordon + barras que llegan) y calcular su sistema local
# (seccion 7 del encargo) con la misma formula que el add-in. Imprime los IDs y la orden lista para pegar
# de conn_get_node_info por HTTP (conn-call.ps1 -Operation node_info), para comparar con lo calculado aqui.
# Antes de ejecutarlo: en Revit, seleccionar el cordon y las diagonales/montante del nudo del Detalle D.
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\07-nudo-seleccion.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function

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

print("=== 07-nudo-seleccion ===")
uidoc = revit.uidoc
seleccion = list(uidoc.Selection.GetElementIds())
print("1) Elementos seleccionados: {0}".format(len(seleccion)))
miembros = leer_miembros_seleccion(doc, uidoc)
print("2) Miembros de armazon estructural con eje: {0}".format(len(miembros)))
for m in miembros:
    print("   [{0}] {1} : {2} | {3} -> {4} mm | L={5:.0f} mm | pendiente={6:.1f} grados".format(
        m["id"], m["familia"], m["tipo"], v_texto(m["ini"]), v_texto(m["fin"]), m["longitud"], m["pendiente"]))

if len(miembros) < 2:
    print("3) Hacen falta al menos 2 miembros (cordon y una diagonal). Selecciona el nudo y repite.")
else:
    cordon, resto = elegir_cordon(miembros)
    print("3) Cordon elegido (mas horizontal, mas largo): [{0}] {1}".format(cordon["id"], cordon["tipo"]))
    print("   Primer miembro (define el plano): [{0}] {1}".format(resto[0]["id"], resto[0]["tipo"]))
    try:
        origen, x, y, z, dist = calcular_marco(cordon["ini"], cordon["fin"], resto[0]["ini"], resto[0]["fin"])
        print("4) Sistema local del nudo:")
        print("   origen (punto de trabajo) mm: " + v_texto(origen))
        print("   X (eje cordon):  ({0:.6f}, {1:.6f}, {2:.6f})".format(*x))
        print("   Y:               ({0:.6f}, {1:.6f}, {2:.6f})".format(*y))
        print("   Z (normal plano):({0:.6f}, {1:.6f}, {2:.6f})".format(*z))
        print("   distancia entre ejes: {0:.2f} mm ({1})".format(dist, "OK <= 5 mm" if dist <= 5.0 else "ERROR > 5 mm: NODE_AXES_NOT_INTERSECTING"))
        for m in resto[1:]:
            try:
                _, _, _, _, d2 = calcular_marco(cordon["ini"], cordon["fin"], m["ini"], m["fin"])
                print("   miembro [{0}] dista {1:.2f} mm del eje del cordon".format(m["id"], d2))
            except ValueError as error:
                print("   miembro [{0}]: {1}".format(m["id"], error))
    except ValueError as error:
        print("4) ERROR: " + str(error))
    ids = [cordon["id"]] + [m["id"] for m in resto]
    print("5) Orden lista para pedir al add-in el mismo nudo (copiar y pegar en PowerShell):")
    print("   .\\scripts\\conn-call.ps1 -Operation node_info -Body '{{\"element_ids\":[{0}],\"chord_element_id\":{1}}}'".format(
        ",".join(str(i) for i in ids), cordon["id"]))

print("=== fin 07-nudo-seleccion ===")
