# -*- coding: utf-8 -*-
# Sondeo 23 (Fase 12, end_plate): los PERNOS DE LA PLACA DE EXTREMO con Advance Steel, sobre la copia y SIN dejar nada (la
# FabricationTransaction se cancela al final; el sondeo 13 limpia si algo quedara). Decide tres cosas que el add-in da por
# supuestas con su reserva escrita:
#   1) Dos planchas de prueba cara a cara (Plate(Plane, Point3d[], Double), la firma probada en el sondeo 09): 200 x 200 x 9,525,
#      separadas gap_mm (0 = en contacto), perpendiculares al eje de la barra elegida (el marco del EndPlateBuilder: Z a lo largo
#      de la barra, X perpendicular al plano de la cercha, Y vertical). Se anota si Advance Steel las acepta pegadas.
#   2) Una FILA de 4 pernos como UN patron FinitRectScrewBoltPattern de 4 x 1 (Nx=4, Ny=1), con BindingLength = 2 espesores + gap y
#      ScrewLength de la pila de dos planchas (sin cartela): la firma del sondeo 10 con un patron "rectangular" de una sola fila.
#      Se anota NumberOfScrews, si WriteToDb acepta Ny=1, y la longitud y agarre que quedan. Si falla, la reserva del add-in son
#      pernos sueltos (patrones de 1 x 1), que se prueban a continuacion.
#   3) La TUERCA A AMBOS LADOS (decision D9): se LISTAN por reflexion las propiedades y metodos de ScrewBoltPattern / BoltPattern
#      con Nut, Washer, Assembly, Set, Standard, Inverted, Head (no se da ninguno por supuesto) y, si existe algo que suene a
#      "tuerca en los dos lados", se anota el nombre literal para la 12b. Ademas se intenta Connect(FilerObject[], eAssemblyLocation)
#      con las dos planchas (la firma del volcado de la Fase 1) y se anota si une el patron a las planchas.
# Barras por defecto: el tramo 1245531 del cordon superior (empalme N9E de la ronda 8c); con una barra seleccionada, esa.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\23-pernos-dos-planchas.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import re
import traceback

ID_TRAMO = 1245531
MM_POR_PIE = 304.8
LADO_MM, ESPESOR_MM, GAP_MM = 200.0, 9.525, 0.0
DIAMETRO_MM, PASO_MM, BORDE_MM = 15.875, 50.0, 25.0
SUPLEMENTO_MM = 22.23            # limits.json bolts.length_addition_mm para 5/8"
INCREMENTO_MM = 6.35             # limits.json bolts.length_increment_mm
CANCELAR_AL_FINAL = True         # True = no queda nada en la copia


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


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


def propiedades_de(tipo):
    from System.Reflection import BindingFlags
    props = {}
    t = tipo
    while t is not None and t.FullName.startswith("Autodesk.AdvanceSteel"):
        for p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance):
            props.setdefault(p.Name, p)
        t = t.BaseType
    return props


def fijar(objeto, props, nombre, valor):
    prop = props.get(nombre)
    if prop is None or not prop.CanWrite:
        return "{0}: no existe o es de solo lectura".format(nombre)
    try:
        prop.SetValue(objeto, System.Convert.ChangeType(valor, prop.PropertyType), None)
        return "{0}={1}".format(nombre, valor)
    except Exception as error:
        return "{0}: ERROR {1}".format(nombre, str(error)[:100])


def leer(objeto, props, nombre):
    prop = props.get(nombre)
    if prop is None:
        return "-"
    try:
        return prop.GetValue(objeto, None)
    except Exception as error:
        return "ERROR {0}".format(str(error)[:60])


print("=== 23-pernos-dos-planchas (Fase 12) ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Transaccion de la API de acero (la misma busqueda que los sondeos 09 y 10)
candidatos = []
for nombre_ens in ("RevitAPISteel", "Autodesk.SteelConnectionsDB"):
    ens = buscar_ensamblado(nombre_ens)
    if ens is None:
        continue
    for t in tipos_de(ens):
        if t.IsClass and re.search(r"Transaction", t.Name) and re.search(r"Fabrication|Steel", t.FullName, re.IGNORECASE):
            candidatos.append(t)
tipo_tx = ctor_tx = args_tx = None
for t in candidatos:
    for c in t.GetConstructors():
        nombres = nombres_parametros(c)
        if ctor_tx is None and nombres == ["Document", "Boolean", "String"]:
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 23 pernos dos planchas"]
if ctor_tx is None:
    print("PARADA: no hay FabricationTransaction(Document, Boolean, String). Sin contexto de acero no se crea nada.")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
paso(1, "Transaccion: {0}; CancelTransaction: {1}".format(tipo_tx.FullName, metodo_cancelar is not None))

# 2) Tipos de Advance Steel
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit (Advance Steel no activo).")
    raise SystemExit
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
T_Patron = obj.GetType("Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern")
ctor_plate = buscar_ctor(T_Plate, ["Plane", "Point3d[]", "Double"]) if T_Plate is not None else None
ctor_patron = buscar_ctor(T_Patron, ["Point3d", "Point3d", "Vector3d", "Vector3d"]) if T_Patron is not None else None
if ctor_plate is None or ctor_patron is None:
    print("PARADA: faltan Plate(Plane, Point3d[], Double) o FinitRectScrewBoltPattern(Point3d, Point3d, Vector3d, Vector3d).")
    raise SystemExit
parametros = ctor_plate.GetParameters()
T_Plane = parametros[0].ParameterType
T_Point3d = parametros[1].ParameterType.GetElementType()
T_Vector3d = T_Plane.Assembly.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d")
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
ctor_plane = buscar_ctor(T_Plane, ["Point3d", "Vector3d"])
write_plate = T_Plate.GetMethod("WriteToDb")
write_patron = T_Patron.GetMethod("WriteToDb")
if None in (ctor_p, ctor_v, ctor_plane, write_plate, write_patron):
    print("PARADA: firma de geometria o WriteToDb no reconocida.")
    raise SystemExit
props_patron = propiedades_de(T_Patron)
paso(2, "Plate y FinitRectScrewBoltPattern listos. Propiedades del patron: {0}".format(", ".join(sorted(props_patron))[:900]))

# 3) Lo que suena a tuerca a ambos lados / conjunto del perno (decision D9): solo se lista
sospechosas = sorted(n for n in props_patron if re.search(r"Nut|Washer|Assembl|Set|Standard|Invert|Head|Grade|Material|Bolt|Screw", n, re.IGNORECASE))
paso(3, "Propiedades que pueden fijar el conjunto del perno (tuerca, arandela, norma): {0}".format(", ".join(sospechosas) or "ninguna"))
metodos = sorted(set(m.Name for m in T_Patron.GetMethods() if re.search(r"Nut|Washer|Assembl|Set|Connect|Invert|Head", m.Name)))
paso(3, "Metodos con Nut/Washer/Assembly/Set/Connect/Invert/Head: {0}".format(", ".join(metodos) or "ninguno"))
for m in T_Patron.GetMethods():
    if m.Name in ("Connect", "SetNutAndWasher", "SetWasher", "SetNut") or re.search(r"Nut|Washer", m.Name):
        print("   {0}({1}) -> {2}".format(m.Name, ", ".join(p.ParameterType.Name + " " + p.Name for p in m.GetParameters()), m.ReturnType.Name))
tipos_conjunto = [t for t in tipos_de(obj) if re.search(r"Nut|Washer|BoltSet|ScrewSet|Assembly", t.Name)]
paso(3, "Tipos de ASObjectsMgd con Nut/Washer/BoltSet/ScrewSet/Assembly: {0}".format(", ".join(t.Name for t in tipos_conjunto) or "ninguno"))

# 4) Marco de la plancha a partir de la barra: Z a lo largo del eje, Y vertical (o la mas vertical), X = Y x Z
seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
tramo = seleccion[0] if len(seleccion) == 1 else doc.GetElement(DB.ElementId(System.Int64(ID_TRAMO)))
if tramo is None or not isinstance(tramo.Location, DB.LocationCurve):
    print("PARADA: no encuentro el tramo {0}; selecciona una barra y repite.".format(ID_TRAMO))
    raise SystemExit
a, b = tramo.Location.Curve.GetEndPoint(0), tramo.Location.Curve.GetEndPoint(1)
origen = b  # el extremo final del tramo = punto de empalme de prueba
z = (b - a).Normalize()
arriba = DB.XYZ.BasisZ
if abs(z.DotProduct(arriba)) > 0.99:
    arriba = DB.XYZ.BasisY
y = (arriba - z.Multiply(arriba.DotProduct(z))).Normalize()
x = y.CrossProduct(z)
paso(4, "Tramo [{0}]: punto de empalme {1}; Z {2}; Y {3}; X {4}".format(tramo.Id.Value, xyz_mm(origen), xyz_mm(z), xyz_mm(y), xyz_mm(x)))


def punto_local(lx, ly, lz):
    p = origen + x.Multiply(lx / MM_POR_PIE) + y.Multiply(ly / MM_POR_PIE) + z.Multiply(lz / MM_POR_PIE)
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


def esquinas(lz):
    arr = System.Array.CreateInstance(T_Point3d, 4)
    medio = LADO_MM / 2.0
    for i, (lx, ly) in enumerate(((-medio, -medio), (medio, -medio), (medio, medio), (-medio, medio))):
        arr[i] = punto_local(lx, ly, lz)
    return arr


agarre = 2 * ESPESOR_MM + GAP_MM
import math
longitud = math.ceil((agarre + SUPLEMENTO_MM) / INCREMENTO_MM) * INCREMENTO_MM
paso(4, "Pila de dos planchas: agarre {0:.2f} mm, longitud del perno {1:.2f} mm (suplemento 5/8\" {2} redondeado a {3})".format(agarre, longitud, SUPLEMENTO_MM, INCREMENTO_MM))

ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(5, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    # 5a) Dos planchas: plano de cada una en su centro (±(gap/2 + t/2) a lo largo de Z), normal Z
    planchas = []
    for signo, nombre in ((1, "A (lado del tramo)"), (-1, "B (otro tramo)")):
        lz = signo * (GAP_MM / 2.0 + ESPESOR_MM / 2.0)
        plano = ctor_plane.Invoke(System.Array[System.Object]([punto_local(0, 0, lz), vector_as(z)]))
        plancha = ctor_plate.Invoke(System.Array[System.Object]([plano, esquinas(lz), ESPESOR_MM]))
        write_plate.Invoke(plancha, None)
        planchas.append(plancha)
        print("   plancha {0} escrita en z local {1:+.3f} mm".format(nombre, lz))
    resultados["planchas"] = "2 planchas de {0} x {0} x {1} con gap {2}: WriteToDb OK".format(LADO_MM, ESPESOR_MM, GAP_MM)

    # 5b) Una fila de 4 pernos como patron 4 x 1 en la cara exterior de la plancha A (plano del patron = cara superior de la pila)
    z_plano = GAP_MM / 2.0 + ESPESOR_MM            # cara exterior de A
    ly = LADO_MM / 2.0 - BORDE_MM                  # fila de arriba
    ancho_fila = PASO_MM * 3
    p1 = punto_local(-ancho_fila / 2.0, ly, z_plano)
    p2 = punto_local(ancho_fila / 2.0, ly, z_plano)
    patron = ctor_patron.Invoke(System.Array[System.Object]([p1, p2, vector_as(x), vector_as(y)]))
    fijados = [fijar(patron, props_patron, "Nx", 4), fijar(patron, props_patron, "Ny", 1),
               fijar(patron, props_patron, "Dx", PASO_MM), fijar(patron, props_patron, "Dy", PASO_MM),
               fijar(patron, props_patron, "ScrewDiameter", DIAMETRO_MM), fijar(patron, props_patron, "BindingLength", agarre),
               fijar(patron, props_patron, "ScrewLength", longitud)]
    print("   patron 4 x 1: " + "; ".join(fijados))
    print("   NumberOfScrews antes de escribir: {0}".format(leer(patron, props_patron, "NumberOfScrews")))
    try:
        write_patron.Invoke(patron, None)
        resultados["fila 4x1"] = "WriteToDb OK; NumberOfScrews={0}; ScrewLength={1}; BindingLength={2}".format(
            leer(patron, props_patron, "NumberOfScrews"), leer(patron, props_patron, "ScrewLength"), leer(patron, props_patron, "BindingLength"))
    except Exception as error:
        resultados["fila 4x1"] = "FALLO WriteToDb: {0}".format(str(error)[:200])
    print("   fila 4 x 1: " + resultados["fila 4x1"])

    # 5c) Connect(FilerObject[], eAssemblyLocation) con las dos planchas, si existe
    conectar = next((m for m in T_Patron.GetMethods() if m.Name == "Connect"), None)
    if conectar is not None:
        try:
            ps = conectar.GetParameters()
            print("   Connect({0})".format(", ".join(p.ParameterType.Name for p in ps)))
            T_Filer = ps[0].ParameterType.GetElementType()
            arr = System.Array.CreateInstance(T_Filer, 2)
            arr[0], arr[1] = planchas[0], planchas[1]
            T_Loc = ps[1].ParameterType
            valores = list(System.Enum.GetNames(T_Loc))
            print("   eAssemblyLocation valores: {0}".format(", ".join(valores)))
            elegido = next((v for v in valores if re.search(r"Site|OnSite|kOnSite", v)), valores[0])
            conectar.Invoke(patron, System.Array[System.Object]([arr, System.Enum.Parse(T_Loc, elegido)]))
            resultados["Connect"] = "OK con {0}".format(elegido)
        except Exception as error:
            resultados["Connect"] = "FALLO: {0}".format(str(error)[:200])
    else:
        resultados["Connect"] = "no existe el metodo Connect en el patron"
    print("   Connect: " + resultados["Connect"])

    # 5d) Reserva: una fila como 4 pernos sueltos (patrones de 1 x 1) en la fila de abajo
    sueltos_ok = 0
    for k in range(4):
        lx = -ancho_fila / 2.0 + k * PASO_MM
        q = punto_local(lx, -ly, z_plano)
        suelto = ctor_patron.Invoke(System.Array[System.Object]([q, q, vector_as(x), vector_as(y)]))
        for nombre, valor in (("Nx", 1), ("Ny", 1), ("Dx", PASO_MM), ("Dy", PASO_MM), ("ScrewDiameter", DIAMETRO_MM), ("BindingLength", agarre), ("ScrewLength", longitud)):
            fijar(suelto, props_patron, nombre, valor)
        try:
            write_patron.Invoke(suelto, None)
            sueltos_ok += 1
        except Exception as error:
            print("   perno suelto {0}: FALLO {1}".format(k + 1, str(error)[:120]))
    resultados["sueltos 1x1"] = "{0}/4 escritos".format(sueltos_ok)
    print("   pernos sueltos: " + resultados["sueltos 1x1"])

    # 5e) Que aparecio en el documento ANTES de cancelar
    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    nuevos = sorted(ids_durante - ids_antes)
    paso(6, "Elementos nuevos con la transaccion abierta: {0}".format(len(nuevos)))
    for i in nuevos[:30]:
        el = doc.GetElement(DB.ElementId(System.Int64(i)))
        try:
            cat = el.Category.Name if el.Category is not None else "-"
        except Exception:
            cat = "?"
        print("   [{0}] {1} | {2}".format(i, el.GetType().Name, cat))
except Exception as error:
    interna = getattr(error, "InnerException", None)
    print("   ERROR {0}: {1}".format(type(error).__name__, str(error)[:400]))
    if interna is not None:
        print("   interna: " + str(interna)[:400])
    print(traceback.format_exc())
finally:
    try:
        if tx is not None:
            if CANCELAR_AL_FINAL and metodo_cancelar is not None:
                metodo_cancelar.Invoke(tx, None)
                paso(7, "CancelTransaction() hecho: nada debe quedar en la copia.")
            else:
                tipo_tx.GetMethod("Commit").Invoke(tx, None)
                paso(7, "Commit() hecho (CANCELAR_AL_FINAL=False): el sondeo 13 lo limpia.")
    except Exception as error:
        paso(7, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0}".format(doc.IsModifiable))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
paso(8, "Elementos nuevos tras cerrar: {0} (se esperan 0 con CancelTransaction; si no, sondeo 13)".format(len(ids_despues - ids_antes)))
paso(9, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 23-pernos-dos-planchas ===")
