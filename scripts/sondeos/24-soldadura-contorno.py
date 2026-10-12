# -*- coding: utf-8 -*-
# Sondeo 24 (Fase 12): la SOLDADURA EN EL CONTORNO de la huella de un tubo sobre una cara, con Advance Steel, sobre la copia y
# sin dejar nada (FabricationTransaction cancelada al final). Hoy el add-in representa todas las soldaduras con DirectShape
# (CreateWelds del backend: "v1"); el volcado de la Fase 1 lista WeldPattern, WeldLine y WeldPoint en ASObjectsMgd pero no se
# ha usado ninguno. Este sondeo:
#   1) LISTA por reflexion los constructores, propiedades y los valores de eWeldType / eWeldSurface de WeldPattern, WeldLine y
#      WeldPoint (no se da ninguna firma por supuesta).
#   2) Si WeldLine tiene un constructor con dos Point3d (o Point3d[] / Point3d, Vector3d) lo intenta con el PRIMER lado de la
#      huella del tubo elegido sobre la cara del cordon (direct_weld) o sobre una plancha de prueba (end_plate: se crea una Plate
#      de 200 x 200 x 9,525 perpendicular a la barra, la firma del sondeo 09), fija lo que exista (Thickness / WeldSize =
#      5 mm, el tipo de filete) y escribe con WriteToDb; si existe Connect(FilerObject[], ...) une la soldadura a las piezas.
#      Se anota que aparecio en el documento (categoria y tipo) antes de cancelar.
#   3) Si WeldPoint tiene un constructor con un Point3d lo intenta en una esquina de la huella.
# Barra por defecto: la diagonal 1249631 (nudo en K, Fase 11 paso 11-5); con una barra seleccionada, esa. La huella es la del
# perfil leido del tipo (STRUCTURAL_SECTION_COMMON_WIDTH / HEIGHT), 63,5 x 63,5 si no se lee.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\24-soldadura-contorno.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import re
import traceback

ID_BARRA = 1249631
MM_POR_PIE = 304.8
SOLDADURA_MM = 5.0
LADO_PLANCHA_MM, ESPESOR_MM = 200.0, 9.525
CREAR_PLANCHA_DE_APOYO = True     # la soldadura necesita algo contra lo que ir: una plancha perpendicular a la barra en su extremo
CANCELAR_AL_FINAL = True


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
        if prop.PropertyType.IsEnum:
            prop.SetValue(objeto, System.Enum.Parse(prop.PropertyType, str(valor)), None)
        else:
            prop.SetValue(objeto, System.Convert.ChangeType(valor, prop.PropertyType), None)
        return "{0}={1}".format(nombre, valor)
    except Exception as error:
        return "{0}: ERROR {1}".format(nombre, str(error)[:100])


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


def describir_tipo(t):
    print("   --- {0}".format(t.FullName))
    for c in t.GetConstructors():
        print("   ctor {0}({1})".format(t.Name, ", ".join(p.ParameterType.Name + " " + p.Name for p in c.GetParameters())))
    props = propiedades_de(t)
    print("   propiedades: {0}".format(", ".join(sorted(props))[:900]))
    for n, p in sorted(props.items()):
        if p.PropertyType.IsEnum:
            try:
                print("   enum {0} ({1}): {2}".format(n, p.PropertyType.Name, ", ".join(System.Enum.GetNames(p.PropertyType))))
            except Exception:
                pass
    metodos = sorted(set(m.Name for m in t.GetMethods() if m.DeclaringType.FullName.startswith("Autodesk.AdvanceSteel")))
    print("   metodos propios: {0}".format(", ".join(metodos)[:700]))
    return props


print("=== 24-soldadura-contorno (Fase 12) ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Transaccion de acero
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
        if ctor_tx is None and nombres_parametros(c) == ["Document", "Boolean", "String"]:
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 24 soldadura contorno"]
if ctor_tx is None:
    print("PARADA: no hay FabricationTransaction(Document, Boolean, String).")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
paso(1, "Transaccion: {0}".format(tipo_tx.FullName))

# 2) Tipos de soldadura de Advance Steel: listar todo
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit (Advance Steel no activo).")
    raise SystemExit
tipos_weld = [t for t in tipos_de(obj) if re.search(r"Weld", t.Name) and not t.IsEnum]
paso(2, "Tipos con Weld en ASObjectsMgd: {0}".format(", ".join(t.Name for t in tipos_weld)))
props_por_tipo = {}
for t in tipos_weld:
    if t.Name in ("WeldPattern", "WeldLine", "WeldPoint"):
        props_por_tipo[t.Name] = describir_tipo(t)
enums = [t for t in tipos_de(obj) if t.IsEnum and re.search(r"Weld", t.FullName)]
for e in enums:
    try:
        print("   enum {0}: {1}".format(e.FullName, ", ".join(System.Enum.GetNames(e))))
    except Exception:
        pass
T_WeldLine = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldLine")
T_WeldPoint = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldPoint")
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
ctor_plate = buscar_ctor(T_Plate, ["Plane", "Point3d[]", "Double"]) if T_Plate is not None else None
if ctor_plate is None:
    print("PARADA: falta Plate(Plane, Point3d[], Double) (la firma del sondeo 09).")
    raise SystemExit
parametros = ctor_plate.GetParameters()
T_Plane = parametros[0].ParameterType
T_Point3d = parametros[1].ParameterType.GetElementType()
T_Vector3d = T_Plane.Assembly.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d")
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
ctor_plane = buscar_ctor(T_Plane, ["Point3d", "Vector3d"])
write_plate = T_Plate.GetMethod("WriteToDb")

# Constructores de WeldLine que se pueden intentar con lo que tenemos (puntos y vectores)
ctor_linea = None
firma_linea = None
if T_WeldLine is not None:
    for nombres in (["Point3d", "Point3d"], ["Point3d[]"], ["Point3d", "Vector3d"], ["Point3d", "Point3d", "Vector3d"]):
        ctor_linea = buscar_ctor(T_WeldLine, nombres)
        if ctor_linea is not None:
            firma_linea = nombres
            break
ctor_punto = buscar_ctor(T_WeldPoint, ["Point3d"]) if T_WeldPoint is not None else None
paso(2, "WeldLine con constructor utilizable: {0}; WeldPoint(Point3d): {1}".format(
    "WeldLine({0})".format(", ".join(firma_linea)) if ctor_linea else "NO (copia la lista de arriba)", ctor_punto is not None))

# 3) Barra y huella
seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
barra = seleccion[0] if len(seleccion) == 1 else doc.GetElement(DB.ElementId(System.Int64(ID_BARRA)))
if barra is None or not isinstance(barra.Location, DB.LocationCurve):
    print("PARADA: no encuentro la barra {0}; selecciona una y repite.".format(ID_BARRA))
    raise SystemExit
a, b = barra.Location.Curve.GetEndPoint(0), barra.Location.Curve.GetEndPoint(1)
# extremo mas alto = el que llega al cordon superior en la diagonal del nudo en K; en otra barra, el final
extremo = b if b.Z >= a.Z else a
otro = a if extremo is b else b
z = (extremo - otro).Normalize()
arriba = DB.XYZ.BasisZ
if abs(z.DotProduct(arriba)) > 0.99:
    arriba = DB.XYZ.BasisY
y = (arriba - z.Multiply(arriba.DotProduct(z))).Normalize()
x = y.CrossProduct(z)
ancho = medida_mm(barra.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, ["b", "Width", "B"]) or 63.5
alto = medida_mm(barra.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, ["h", "Height", "H", "d"]) or ancho
paso(3, "Barra [{0}] {1}: extremo {2}, eje {3}, huella {4:.1f} x {5:.1f} mm".format(barra.Id.Value, DB.Element.Name.__get__(barra.Symbol), xyz_mm(extremo), xyz_mm(z), ancho, alto))


def punto_local(lx, ly, lz):
    p = extremo + x.Multiply(lx / MM_POR_PIE) + y.Multiply(ly / MM_POR_PIE) + z.Multiply(lz / MM_POR_PIE)
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


hw, hh = ancho / 2.0, alto / 2.0
contorno = [(-hw, -hh), (hw, -hh), (hw, hh), (-hw, hh)]
ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(4, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    plancha = None
    if CREAR_PLANCHA_DE_APOYO and None not in (ctor_p, ctor_v, ctor_plane, write_plate):
        arr = System.Array.CreateInstance(T_Point3d, 4)
        medio = LADO_PLANCHA_MM / 2.0
        for i, (lx, ly) in enumerate(((-medio, -medio), (medio, -medio), (medio, medio), (-medio, medio))):
            arr[i] = punto_local(lx, ly, ESPESOR_MM / 2.0)
        plano = ctor_plane.Invoke(System.Array[System.Object]([punto_local(0, 0, ESPESOR_MM / 2.0), vector_as(z)]))
        plancha = ctor_plate.Invoke(System.Array[System.Object]([plano, arr, ESPESOR_MM]))
        write_plate.Invoke(plancha, None)
        resultados["plancha de apoyo"] = "escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)"
        print("   " + resultados["plancha de apoyo"])

    # 4a) Un lado del contorno como WeldLine
    if ctor_linea is not None:
        props = props_por_tipo.get("WeldLine") or propiedades_de(T_WeldLine)
        p1, p2 = punto_local(contorno[0][0], contorno[0][1], 0.0), punto_local(contorno[1][0], contorno[1][1], 0.0)
        try:
            if firma_linea == ["Point3d", "Point3d"]:
                linea = ctor_linea.Invoke(System.Array[System.Object]([p1, p2]))
            elif firma_linea == ["Point3d[]"]:
                arr2 = System.Array.CreateInstance(T_Point3d, 2)
                arr2[0], arr2[1] = p1, p2
                linea = ctor_linea.Invoke(System.Array[System.Object]([arr2]))
            elif firma_linea == ["Point3d", "Vector3d"]:
                linea = ctor_linea.Invoke(System.Array[System.Object]([p1, vector_as(z)]))
            else:
                linea = ctor_linea.Invoke(System.Array[System.Object]([p1, p2, vector_as(z)]))
            fijados = []
            for nombre in ("Thickness", "WeldSize", "Size", "MainThickness", "UpperThickness"):
                if nombre in props:
                    fijados.append(fijar(linea, props, nombre, SOLDADURA_MM))
            for nombre in ("WeldType", "MainWeldType", "UpperWeldType"):
                p = props.get(nombre)
                if p is not None and p.PropertyType.IsEnum:
                    valores = list(System.Enum.GetNames(p.PropertyType))
                    filete = next((v for v in valores if re.search(r"Fillet", v, re.IGNORECASE)), None)
                    if filete:
                        fijados.append(fijar(linea, props, nombre, filete))
            print("   WeldLine propiedades: " + ("; ".join(fijados) or "ninguna reconocida (Thickness/WeldSize/WeldType)"))
            conectar = next((m for m in T_WeldLine.GetMethods() if m.Name == "Connect"), None)
            if conectar is not None and plancha is not None:
                try:
                    ps = conectar.GetParameters()
                    print("   WeldLine.Connect({0})".format(", ".join(p.ParameterType.Name for p in ps)))
                    if len(ps) >= 1 and ps[0].ParameterType.IsArray:
                        T_Filer = ps[0].ParameterType.GetElementType()
                        piezas = System.Array.CreateInstance(T_Filer, 1)
                        piezas[0] = plancha
                        args = [piezas]
                        for p in list(ps)[1:]:
                            if p.ParameterType.IsEnum:
                                valores = list(System.Enum.GetNames(p.ParameterType))
                                args.append(System.Enum.Parse(p.ParameterType, next((v for v in valores if re.search(r"Site", v)), valores[0])))
                            else:
                                args.append(None)
                        conectar.Invoke(linea, System.Array[System.Object](args))
                        resultados["WeldLine.Connect"] = "OK con la plancha de apoyo"
                except Exception as error:
                    resultados["WeldLine.Connect"] = "FALLO: {0}".format(str(error)[:200])
                print("   Connect: " + resultados.get("WeldLine.Connect", "no se intento"))
            write = T_WeldLine.GetMethod("WriteToDb")
            if write is not None:
                write.Invoke(linea, None)
                resultados["WeldLine"] = "WriteToDb OK ({0} mm, lado 1 de la huella)".format(SOLDADURA_MM)
            else:
                resultados["WeldLine"] = "sin WriteToDb"
        except Exception as error:
            interna = getattr(error, "InnerException", None)
            resultados["WeldLine"] = "FALLO {0}: {1}{2}".format(type(error).__name__, str(error)[:200], (" | interna: " + str(interna)[:200]) if interna is not None else "")
        print("   WeldLine: " + resultados["WeldLine"])
    else:
        resultados["WeldLine"] = "sin constructor utilizable (ver la lista del paso 2)"

    # 4b) Un WeldPoint en la primera esquina
    if ctor_punto is not None:
        try:
            punto = ctor_punto.Invoke(System.Array[System.Object]([punto_local(contorno[0][0], contorno[0][1], 0.0)]))
            write = T_WeldPoint.GetMethod("WriteToDb")
            if write is not None:
                write.Invoke(punto, None)
            resultados["WeldPoint"] = "WriteToDb OK" if write is not None else "sin WriteToDb"
        except Exception as error:
            resultados["WeldPoint"] = "FALLO: {0}".format(str(error)[:200])
        print("   WeldPoint: " + resultados["WeldPoint"])

    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    nuevos = sorted(ids_durante - ids_antes)
    paso(5, "Elementos nuevos con la transaccion abierta: {0}".format(len(nuevos)))
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
                paso(6, "CancelTransaction() hecho: nada debe quedar en la copia.")
            else:
                tipo_tx.GetMethod("Commit").Invoke(tx, None)
                paso(6, "Commit() hecho (CANCELAR_AL_FINAL=False): el sondeo 13 lo limpia.")
    except Exception as error:
        paso(6, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0}".format(doc.IsModifiable))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
paso(7, "Elementos nuevos tras cerrar: {0} (se esperan 0; si no, sondeo 13)".format(len(ids_despues - ids_antes)))
paso(8, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 24-soldadura-contorno ===")
