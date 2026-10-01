# -*- coding: utf-8 -*-
# Sondeo 10: SOLO si el sondeo 09 creo la placa. Intento "a ciegas" de un grupo de 4 pernos (2x2) de Advance Steel
# en el mismo nudo. Las firmas de FinitRectScrewBoltPattern no estan verificadas: el sondeo primero las vuelca y
# solo intenta crear si encuentra el constructor (Point3d, Point3d, Vector3d, Vector3d); las propiedades Nx, Ny,
# Dx, Dy y el diametro se fijan por reflexion solo si existen. Todo dentro del contexto de acero (como en 09).
# Se ejecuta SIN transaccion envolvente y sobre la copia "_sondeo.rvt":
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\10-pernos-camino-a.py -SinTransaccion
from __future__ import print_function
import os
import re

UNIDAD_AS = "pies"          # la misma que resulto correcta en el sondeo 09
DIAMETRO_MM, SEPARACION_MM = 15.875, 60.0
MM_POR_PIE = 304.8


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


def v_mm(xyz):
    return (xyz.X * MM_POR_PIE, xyz.Y * MM_POR_PIE, xyz.Z * MM_POR_PIE)


def a_unidad_as(mm):
    return mm / MM_POR_PIE if UNIDAD_AS == "pies" else mm


print("=== 10-pernos-camino-a ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt'.")
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
            nombres = nombres_parametros(c)
            if ctor_tx is None and nombres == ["Document", "Boolean", "String"]:
                tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo pernos A"]
            elif ctor_tx is None and nombres == ["Document", "String"]:
                tipo_tx, ctor_tx, args_tx = t, c, [doc, "Sondeo pernos A"]
            elif ctor_tx is None and nombres == ["Document"]:
                tipo_tx, ctor_tx, args_tx = t, c, [doc]
if ctor_tx is None:
    print("PARADA: sin contexto de acero no se crea nada.")
    raise SystemExit
print("1) Transaccion: {0}".format(tipo_tx.FullName))

# 2) Tipos de pernos (geometria tomada de los parametros del constructor: mismo contexto de carga que ASObjectsMgd)
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit.")
    raise SystemExit

nombres_patron = [t for t in tipos_de(obj) if re.search(r"BoltPattern|ScrewBolt|Bolt", t.Name)]
print("2) Tipos de pernos en ASObjectsMgd ({0}):".format(len(nombres_patron)))
for t in nombres_patron[:40]:
    print("   " + t.FullName)
T_Patron = obj.GetType("Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern")
if T_Patron is None:
    print("PARADA: no existe FinitRectScrewBoltPattern.")
    raise SystemExit
print("   constructores de FinitRectScrewBoltPattern:")
for c in T_Patron.GetConstructors():
    print("     ({0})".format(", ".join(nombres_parametros(c))))
from System.Reflection import BindingFlags
flags = BindingFlags.Public | BindingFlags.Instance
props = {}
t = T_Patron
while t is not None and t.FullName.startswith("Autodesk.AdvanceSteel"):
    for p in t.GetProperties(flags):
        props.setdefault(p.Name, p)
    t = t.BaseType
print("   propiedades (con las de las clases base): " + ", ".join(sorted(props.keys())[:80]))
metodos = sorted(set(m.Name for m in T_Patron.GetMethods(flags) if re.search(r"Write|Connect|Set|Add", m.Name)))
print("   metodos Write*/Connect*/Set*/Add*: " + ", ".join(metodos[:40]))

ctor_patron = buscar_ctor(T_Patron, ["Point3d", "Point3d", "Vector3d", "Vector3d"])
if ctor_patron is None:
    print("PARADA: no hay constructor (Point3d, Point3d, Vector3d, Vector3d). Con el volcado anterior se escribe la version correcta en la siguiente sesion.")
    raise SystemExit
parametros = ctor_patron.GetParameters()
T_Point3d = parametros[0].ParameterType
T_Vector3d = parametros[2].ParameterType
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
if ctor_p is None or ctor_v is None:
    print("PARADA: Point3d/Vector3d sin constructor (d,d,d).")
    raise SystemExit

# 3) Posicion: caja de la ultima placa creada (categoria de placas de conexion) -> centro y plano
cat_placas = getattr(DB.BuiltInCategory, "OST_StructConnectionPlates", None)
placa = None
if cat_placas is not None:
    placas = list(DB.FilteredElementCollector(doc).OfCategory(cat_placas).WhereElementIsNotElementType())
    placa = placas[-1] if placas else None
if placa is None:
    print("PARADA: no se encontro ninguna placa de conexion (crea la del sondeo 09 primero).")
    raise SystemExit
caja = placa.get_BoundingBox(None)
cmin, cmax = v_mm(caja.Min), v_mm(caja.Max)
centro = ((cmin[0] + cmax[0]) / 2, (cmin[1] + cmax[1]) / 2, (cmin[2] + cmax[2]) / 2)
lados = (cmax[0] - cmin[0], cmax[1] - cmin[1], cmax[2] - cmin[2])
eje_fino = min(range(3), key=lambda i: lados[i])
normal = [0.0, 0.0, 0.0]
normal[eje_fino] = 1.0
ejes = [i for i in range(3) if i != eje_fino]
ux = [0.0, 0.0, 0.0]
ux[ejes[0]] = 1.0
uy = [0.0, 0.0, 0.0]
uy[ejes[1]] = 1.0
print("3) Placa [{0}] centro {1} mm, lados {2}, normal aproximada {3}".format(placa.Id.Value, centro, lados, normal))
print("   (posicion aproximada por la caja alineada a ejes globales; vale para la prueba)")


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([a_unidad_as(p[0]), a_unidad_as(p[1]), a_unidad_as(p[2])]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v[0], v[1], v[2]]))


ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    p1 = [centro[i] - SEPARACION_MM / 2 * (ux[i] + uy[i]) for i in range(3)]
    p2 = [centro[i] + SEPARACION_MM / 2 * (ux[i] + uy[i]) for i in range(3)]
    patron = ctor_patron.Invoke(System.Array[System.Object]([punto_as(p1), punto_as(p2), vector_as(ux), vector_as(uy)]))
    print("4) Patron creado en memoria: {0}".format(patron.GetType().FullName))
    fijados = []
    for nombre, valor in (("Nx", 2), ("Ny", 2), ("Wx", a_unidad_as(SEPARACION_MM)), ("Wy", a_unidad_as(SEPARACION_MM)),
                          ("Dx", a_unidad_as(SEPARACION_MM)), ("Dy", a_unidad_as(SEPARACION_MM)), ("ScrewDiameter", a_unidad_as(DIAMETRO_MM))):
        prop = props.get(nombre)
        if prop is not None and prop.CanWrite:
            try:
                tipo_valor = prop.PropertyType
                convertido = System.Convert.ChangeType(valor, tipo_valor)
                prop.SetValue(patron, convertido, None)
                fijados.append("{0}={1}".format(nombre, valor))
            except Exception as error:
                print("   no se pudo fijar {0}: {1}".format(nombre, str(error)[:120]))
    print("   propiedades fijadas: " + (", ".join(fijados) or "ninguna"))
    metodo_write = T_Patron.GetMethod("WriteToDb")
    if metodo_write is None:
        raise Exception("FinitRectScrewBoltPattern no tiene WriteToDb")
    metodo_write.Invoke(patron, None)
    print("   WriteToDb() OK")
    commit = tipo_tx.GetMethod("Commit")
    if commit is not None:
        commit.Invoke(tx, None)
        print("   Commit() OK")
    tx = None
except Exception as error:
    interna = getattr(error, "InnerException", None)
    print("   ERROR {0}: {1}".format(type(error).__name__, str(error)[:400]))
    if interna is not None:
        print("   interna: " + str(interna)[:400])
    try:
        if tx is not None and tipo_tx.GetMethod("CancelTransaction") is not None:
            tipo_tx.GetMethod("CancelTransaction").Invoke(tx, None)
            print("   CancelTransaction() hecho")
    except Exception as e2:
        print("   CancelTransaction ERROR " + str(e2)[:200])
finally:
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass

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
