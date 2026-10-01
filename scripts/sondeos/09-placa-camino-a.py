# -*- coding: utf-8 -*-
# Sondeo 09: PRUEBA DE ESCRITURA del camino A: crear UNA placa de Advance Steel (200 x 200 x 10 mm) en el
# nudo seleccionado, dentro del contexto de fabricacion de acero de Revit. Reglas de seguridad (fase-0.md, 8.1):
#   - Solo corre sobre la copia "_sondeo.rvt" (sondeo 08). Si no, no hace nada.
#   - Solo crea objetos de Advance Steel si antes encuentra y abre un tipo *Transaction* de la API de acero
#     (Autodesk.Revit.DB.Steel.FabricationTransaction o equivalente en RevitAPISteel / SteelConnectionsDB).
#     Crear objetos AS fuera de ese contexto MATA el proceso de Revit (lo comprobo el instalador en la Fase 0).
#   - Si falta cualquier tipo, constructor o metodo esperado, imprime lo que hay y termina sin escribir.
# Se ejecuta SIN transaccion envolvente (la API de acero abre la suya):
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\09-placa-camino-a.py -SinTransaccion
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import re

# Unidades con las que se dan las coordenadas a Advance Steel. Se desconoce si dentro de Revit espera pies
# (unidades internas de Revit) o mm: se prueba con PIES y el sondeo mide despues el tamano real del elemento.
UNIDAD_AS = "pies"          # "pies" o "mm"
ANCHO_MM, ALTO_MM, ESPESOR_MM = 200.0, 200.0, 10.0

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
    if abs(z[2]) > 1e-9:
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


def cargar_por_ruta(nombre_dll):
    base = os.path.splitext(nombre_dll)[0]
    ensamblado = buscar_ensamblado(base)
    if ensamblado is not None:
        return ensamblado
    carpeta_revit = os.path.dirname(str(clr.GetClrType(DB.Element).Assembly.Location))
    ruta = os.path.join(carpeta_revit, "AddIns", "SteelConnections", nombre_dll)
    if not os.path.isfile(ruta):
        print("   no existe " + ruta)
        return None
    clr.AddReferenceToFileAndPath(ruta)
    return buscar_ensamblado(base)


def nombres_parametros(metodo):
    return [p.ParameterType.Name for p in metodo.GetParameters()]


def buscar_ctor(tipo, nombres):
    for c in tipo.GetConstructors():
        if nombres_parametros(c) == list(nombres):
            return c
    return None


def a_unidad_as(mm):
    return mm / MM_POR_PIE if UNIDAD_AS == "pies" else mm


def caja_mm(elemento):
    try:
        caja = elemento.get_BoundingBox(None)
        if caja is None:
            return "sin caja"
        d = v_restar(v_mm(caja.Max), v_mm(caja.Min))
        return "caja {0:.1f} x {1:.1f} x {2:.1f} mm, min {3}".format(d[0], d[1], d[2], v_texto(v_mm(caja.Min)))
    except Exception as error:
        return "caja ERROR " + str(error)[:80]


print("=== 09-placa-camino-a ===")

# 0) Guardas
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}). Ejecuta antes el sondeo 08.".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Tipo de transaccion de la API de acero
candidatos = []
for nombre_ens in ("RevitAPISteel", "Autodesk.SteelConnectionsDB"):
    ens = buscar_ensamblado(nombre_ens)
    if ens is None:
        continue
    for t in tipos_de(ens):
        if t.IsClass and re.search(r"Transaction", t.Name) and re.search(r"Fabrication|Steel", t.FullName, re.IGNORECASE):
            candidatos.append(t)
print("1) Tipos *Transaction* de la API de acero: {0}".format(", ".join(t.FullName for t in candidatos) or "NINGUNO"))
if not candidatos:
    print("PARADA: sin contexto de acero no se crea nada (regla de seguridad). Mira la salida del sondeo 06.")
    raise SystemExit
tipo_tx = None
ctor_tx = None
args_tx = None
for t in candidatos:
    for c in t.GetConstructors():
        nombres = nombres_parametros(c)
        print("   {0}({1})".format(t.Name, ", ".join(nombres)))
        if ctor_tx is None and nombres == ["Document", "Boolean", "String"]:
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo placa A"]
        elif ctor_tx is None and nombres == ["Document", "String"]:
            tipo_tx, ctor_tx, args_tx = t, c, [doc, "Sondeo placa A"]
        elif ctor_tx is None and nombres == ["Document"]:
            tipo_tx, ctor_tx, args_tx = t, c, [doc]
if ctor_tx is None:
    print("PARADA: ningun constructor reconocido (Document[, Boolean][, String]). No se crea nada.")
    raise SystemExit
print("   se usara {0}({1})".format(tipo_tx.FullName, ", ".join(nombres_parametros(ctor_tx))))
tiene_commit = tipo_tx.GetMethod("Commit") is not None
print("   Commit(): {0}; RollBack(): {1}; Dispose(): {2}".format(tiene_commit, tipo_tx.GetMethod("RollBack") is not None, tipo_tx.GetMethod("Dispose") is not None))

# 2) Ensamblados de Advance Steel y tipos necesarios
geo = cargar_por_ruta("ASGeometryMgd.dll")
obj = cargar_por_ruta("ASObjectsMgd.dll")
if geo is None or obj is None:
    print("PARADA: faltan ASGeometryMgd.dll / ASObjectsMgd.dll.")
    raise SystemExit
T_Point3d = geo.GetType("Autodesk.AdvanceSteel.Geometry.Point3d")
T_Vector3d = geo.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d")
T_Plane = geo.GetType("Autodesk.AdvanceSteel.Geometry.Plane")
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
print("2) Point3d={0} Vector3d={1} Plane={2} Plate={3}".format(T_Point3d is not None, T_Vector3d is not None, T_Plane is not None, T_Plate is not None))
if None in (T_Point3d, T_Vector3d, T_Plane, T_Plate):
    print("PARADA: falta algun tipo de Advance Steel.")
    raise SystemExit
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
ctor_plane = buscar_ctor(T_Plane, ["Point3d", "Vector3d"])
ctor_plate = buscar_ctor(T_Plate, ["Plane", "Point3d[]", "Double"])
metodo_write = T_Plate.GetMethod("WriteToDb")
print("   ctores: Point3d(d,d,d)={0} Vector3d(d,d,d)={1} Plane(Point3d,Vector3d)={2} Plate(Plane,Point3d[],Double)={3} WriteToDb={4}".format(
    ctor_p is not None, ctor_v is not None, ctor_plane is not None, ctor_plate is not None, metodo_write is not None))
if None in (ctor_p, ctor_v, ctor_plane, ctor_plate, metodo_write):
    print("   constructores de Plate disponibles:")
    for c in T_Plate.GetConstructors():
        print("     Plate({0})".format(", ".join(nombres_parametros(c))))
    print("PARADA: firma no reconocida. No se crea nada.")
    raise SystemExit

# 3) Nudo seleccionado y sistema local
miembros = leer_miembros_seleccion(doc, uidoc)
if len(miembros) < 2:
    print("PARADA: selecciona en Revit el cordon y al menos una diagonal del nudo ({0} miembros validos).".format(len(miembros)))
    raise SystemExit
cordon, resto = elegir_cordon(miembros)
origen, x, y, z, dist = calcular_marco(cordon["ini"], cordon["fin"], resto[0]["ini"], resto[0]["fin"])
print("3) Nudo: cordon [{0}], primer miembro [{1}], origen {2} mm, dist ejes {3:.2f} mm".format(cordon["id"], resto[0]["id"], v_texto(origen), dist))
esquinas_mm = []
for (lx, ly) in ((-ANCHO_MM / 2, -ALTO_MM / 2), (ANCHO_MM / 2, -ALTO_MM / 2), (ANCHO_MM / 2, ALTO_MM / 2), (-ANCHO_MM / 2, ALTO_MM / 2)):
    esquinas_mm.append(v_sumar(v_sumar(origen, v_escalar(x, lx)), v_escalar(y, ly)))
print("   esquinas (mm): " + "; ".join(v_texto(p) for p in esquinas_mm))
print("   unidades pasadas a Advance Steel: {0}".format(UNIDAD_AS))


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([a_unidad_as(p[0]), a_unidad_as(p[1]), a_unidad_as(p[2])]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v[0], v[1], v[2]]))


# 4) Elementos antes, para detectar los nuevos
ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())

# 5) Crear la placa dentro del contexto de acero
tx = None
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    print("4) {0} abierta. doc.IsModifiable={1}".format(tipo_tx.Name, doc.IsModifiable))
    plano = ctor_plane.Invoke(System.Array[System.Object]([punto_as(origen), vector_as(z)]))
    esquinas = System.Array.CreateInstance(T_Point3d, 4)
    for i, p in enumerate(esquinas_mm):
        esquinas[i] = punto_as(p)
    placa = ctor_plate.Invoke(System.Array[System.Object]([plano, esquinas, a_unidad_as(ESPESOR_MM)]))
    print("   Plate creada en memoria: {0}".format(placa.GetType().FullName))
    metodo_write.Invoke(placa, None)
    print("   WriteToDb() OK")
    if tiene_commit:
        tipo_tx.GetMethod("Commit").Invoke(tx, None)
        print("   Commit() OK")
    tx = None
except Exception as error:
    interna = getattr(error, "InnerException", None)
    print("   ERROR {0}: {1}".format(type(error).__name__, str(error)[:400]))
    if interna is not None:
        print("   interna: " + str(interna)[:400])
    try:
        if tx is not None and tipo_tx.GetMethod("RollBack") is not None:
            tipo_tx.GetMethod("RollBack").Invoke(tx, None)
            print("   RollBack() hecho")
    except Exception as e2:
        print("   RollBack ERROR " + str(e2)[:200])
finally:
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0}".format(doc.IsModifiable))

# 6) Que aparecio en el documento
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
nuevos = sorted(ids_despues - ids_antes)
print("5) Elementos nuevos en el documento: {0}".format(len(nuevos)))
for i in nuevos[:20]:
    el = doc.GetElement(DB.ElementId(System.Int64(i)))
    try:
        cat = el.Category.Name if el.Category is not None else "-"
    except Exception:
        cat = "?"
    print("   [{0}] {1} | {2} | {3}".format(i, el.GetType().Name, cat, caja_mm(el)))
if nuevos:
    print("   Compara la caja con 200 x 200 x 10 mm: si sale ~60960 mm, Advance Steel esperaba mm; si sale ~0,66 mm, esperaba pies.")
    try:
        doc.Save()
        print("6) Copia guardada con la placa: {0}".format(doc.PathName))
    except Exception as error:
        print("6) No se pudo guardar: " + str(error)[:200])

print("=== fin 09-placa-camino-a ===")
