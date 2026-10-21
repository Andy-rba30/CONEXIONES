# -*- coding: utf-8 -*-
# Sondeo 26 (ronda 12b, direct_weld / end_plate): ESCRIBIR UNA SOLDADURA NATIVA de Advance Steel con la firma que el sondeo 24
# encontro y no intento: WeldLine(Point3d[] polyLineWCS, Vector3d xvec, Vector3d yvec) y WeldPoint(Point3d origin, Vector3d
# xvec, Vector3d yvec). El sondeo 24 solo buscaba WeldLine(Point3d, Point3d) y parecidos, por eso dijo "sin constructor
# utilizable" aunque listo el bueno. Sobre la copia y, por defecto, SIN dejar nada (CancelTransaction al final).
#   1) FabricationTransaction, Plate y los tipos de soldadura, como en el 24.
#   2) El objeto de Advance Steel de la BARRA de Revit (la diagonal), por el puente de RevitAPI: SteelElementProperties.
#      AddFabricationInformationForRevitElements + GetFabricationUniqueID(doc, Reference) y despues un metodo estatico de
#      ASObjectsMgd / CADAccess que reciba ese Guid (se buscan todos por reflexion y se prueban); si ninguno sirve, se recorren
#      los ObjectId del modelo con FilerObject.GetFilerObject(ObjectId) comparando GetUniqueId(). Se anota el camino que funciono.
#   3) Una plancha de apoyo (200 x 200 x 9,525) perpendicular a la barra en su extremo (como en el 24) y:
#      a) WeldLine con las 4 esquinas de la huella de la barra (contorno cerrado), espesor 5 (propiedades con Thickness/Throat),
#         tipo de soldadura con "Fillet" si lo hay, Connect([barra, plancha] o [plancha], kInShop) y WriteToDb;
#      b) WeldPoint en una esquina, con las mismas propiedades, Connect y WriteToDb.
#   4) Elementos nuevos con la transaccion abierta y tras cerrarla (se esperan 0 con CancelTransaction).
# Para VER la soldadura en Revit: crea antes el archivo %TEMP%\sondeo26-commit.txt (PowerShell: New-Item $env:TEMP\sondeo26-commit.txt)
# y repite el sondeo: hace Commit en vez de cancelar; despues captura y Ctrl+Z (o cierra la copia sin guardar); el sondeo 13 limpia.
# Barra por defecto: la diagonal 1249631; con una barra seleccionada, esa.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\26-soldadura-weldline.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import re
import traceback

ID_BARRA = 1249631
MM_POR_PIE = 304.8
SOLDADURA_MM = 5.0
LADO_PLANCHA_MM, ESPESOR_MM = 200.0, 9.525
MARCA_COMMIT = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo26-commit.txt")
CANCELAR_AL_FINAL = not os.path.exists(MARCA_COMMIT)


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


def leer(objeto, props, nombre):
    prop = props.get(nombre)
    if prop is None:
        return "-"
    try:
        return prop.GetValue(objeto, None)
    except Exception as error:
        return "ERROR {0}".format(str(error)[:60])


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


def error_texto(error):
    interna = getattr(error, "InnerException", None)
    return "{0}: {1}{2}".format(type(error).__name__, str(error)[:200], (" | interna: " + str(interna)[:200]) if interna is not None else "")


def filer_de(elemento, obj, T_Filer):
    """El FilerObject de Advance Steel de una barra de Revit. Devuelve (objeto o None, lista de intentos)."""
    from System.Reflection import BindingFlags
    intentos = []
    guid = None
    try:
        ids = System.Collections.Generic.List[DB.ElementId]()
        ids.Add(elemento.Id)
        anadidos = DB.Steel.SteelElementProperties.AddFabricationInformationForRevitElements(doc, ids)
        intentos.append("AddFabricationInformationForRevitElements -> {0}".format(anadidos.Count if anadidos is not None else "None"))
    except Exception as error:
        intentos.append("AddFabricationInformationForRevitElements ERROR " + error_texto(error))
    try:
        guid = DB.Steel.SteelElementProperties.GetFabricationUniqueID(doc, DB.Reference(elemento))
        intentos.append("GetFabricationUniqueID -> {0}".format(guid))
    except Exception as error:
        intentos.append("GetFabricationUniqueID ERROR " + error_texto(error))
    if guid is None or str(guid) == "00000000-0000-0000-0000-000000000000":
        return None, intentos
    ensamblados = []
    for e in (T_Filer.Assembly, obj):
        if e is not None and e not in ensamblados:
            ensamblados.append(e)
    # a) metodos estaticos que reciben un Guid
    for e in ensamblados:
        for t in tipos_de(e):
            try:
                metodos = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
            except Exception:
                continue
            for m in metodos:
                ps = m.GetParameters()
                if len(ps) == 1 and ps[0].ParameterType.Name == "Guid":
                    try:
                        r = m.Invoke(None, System.Array[System.Object]([guid]))
                        intentos.append("{0}.{1}(Guid) -> {2}".format(t.Name, m.Name, r.GetType().Name if r is not None else "None"))
                        if r is not None and T_Filer.IsAssignableFrom(r.GetType()):
                            return r, intentos
                    except Exception as error:
                        intentos.append("{0}.{1}(Guid) ERROR {2}".format(t.Name, m.Name, str(error)[:100]))
    # b) recorrer los ObjectId del modelo comparando GetUniqueId()
    get_por_id = None
    for m in T_Filer.GetMethods(BindingFlags.Public | BindingFlags.Static):
        if m.Name == "GetFilerObject" and nombres_parametros(m) == ["ObjectId"]:
            get_por_id = m
    if get_por_id is not None:
        for e in ensamblados:
            for t in tipos_de(e):
                try:
                    metodos = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                except Exception:
                    continue
                for m in metodos:
                    if len(m.GetParameters()) == 0 and re.search(r"ObjectId", m.ReturnType.Name) and re.search(r"\[\]|List|Collection|Enumerable", m.ReturnType.Name):
                        try:
                            lista = m.Invoke(None, None)
                            cuenta = 0
                            for oid in lista:
                                cuenta += 1
                                try:
                                    fo = get_por_id.Invoke(None, System.Array[System.Object]([oid]))
                                    if fo is not None and str(fo.GetUniqueId()) == str(guid):
                                        intentos.append("{0}.{1}() recorrido: {2} objetos, encontrado".format(t.Name, m.Name, cuenta))
                                        return fo, intentos
                                except Exception:
                                    pass
                            intentos.append("{0}.{1}() recorrido: {2} objetos, no encontrado".format(t.Name, m.Name, cuenta))
                        except Exception as error:
                            intentos.append("{0}.{1}() ERROR {2}".format(t.Name, m.Name, str(error)[:100]))
    return None, intentos


print("=== 26-soldadura-weldline (ronda 12b) ===")
print("   modo: {0}".format("CANCELAR al final (nada queda)" if CANCELAR_AL_FINAL else "COMMIT al final (existe {0}): captura y Ctrl+Z".format(MARCA_COMMIT)))
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Transaccion de acero y tipos
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
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 26 soldadura WeldLine"]
if ctor_tx is None:
    print("PARADA: no hay FabricationTransaction(Document, Boolean, String).")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit (Advance Steel no activo).")
    raise SystemExit
T_WeldLine = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldLine")
T_WeldPoint = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldPoint")
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
ctor_plate = buscar_ctor(T_Plate, ["Plane", "Point3d[]", "Double"]) if T_Plate is not None else None
if ctor_plate is None or T_WeldLine is None or T_WeldPoint is None:
    print("PARADA: faltan Plate(Plane, Point3d[], Double), WeldLine o WeldPoint.")
    raise SystemExit
parametros = ctor_plate.GetParameters()
T_Plane = parametros[0].ParameterType
T_Point3d = parametros[1].ParameterType.GetElementType()
T_Vector3d = T_Plane.Assembly.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d")
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"])
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"])
ctor_plane = buscar_ctor(T_Plane, ["Point3d", "Vector3d"])
write_plate = T_Plate.GetMethod("WriteToDb")
ctor_linea = buscar_ctor(T_WeldLine, ["Point3d[]", "Vector3d", "Vector3d"])
ctor_punto = buscar_ctor(T_WeldPoint, ["Point3d", "Vector3d", "Vector3d"])
T_Filer = T_Plate
while T_Filer is not None and T_Filer.Name != "FilerObject":
    T_Filer = T_Filer.BaseType
if None in (ctor_p, ctor_v, ctor_plane, write_plate) or T_Filer is None:
    print("PARADA: firma de geometria, WriteToDb o FilerObject no reconocida.")
    raise SystemExit
paso(1, "Transaccion {0}; WeldLine(Point3d[], Vector3d, Vector3d): {1}; WeldPoint(Point3d, Vector3d, Vector3d): {2}; FilerObject en {3}".format(
    tipo_tx.FullName, ctor_linea is not None, ctor_punto is not None, T_Filer.Assembly.GetName().Name))
props_linea = propiedades_de(T_WeldLine)
props_punto = propiedades_de(T_WeldPoint)
escribibles = sorted(n for n, p in props_linea.items() if p.CanWrite)
print("   WeldLine, propiedades escribibles: {0}".format(", ".join(escribibles)[:900]))
for n, p in sorted(props_linea.items()):
    if p.PropertyType.IsEnum and p.CanWrite:
        try:
            print("   enum escribible {0} ({1}): {2}".format(n, p.PropertyType.Name, ", ".join(System.Enum.GetNames(p.PropertyType))[:300]))
        except Exception:
            pass

# 2) Barra, huella y su objeto de Advance Steel
seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
barra = seleccion[0] if len(seleccion) == 1 else doc.GetElement(DB.ElementId(System.Int64(ID_BARRA)))
if barra is None or not isinstance(barra.Location, DB.LocationCurve):
    print("PARADA: no encuentro la barra {0}; selecciona una y repite.".format(ID_BARRA))
    raise SystemExit
a, b = barra.Location.Curve.GetEndPoint(0), barra.Location.Curve.GetEndPoint(1)
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
paso(2, "Barra [{0}] {1}: extremo {2}, eje {3}, huella {4:.1f} x {5:.1f} mm".format(barra.Id.Value, DB.Element.Name.__get__(barra.Symbol), xyz_mm(extremo), xyz_mm(z), ancho, alto))


def punto_local(lx, ly, lz):
    p = extremo + x.Multiply(lx / MM_POR_PIE) + y.Multiply(ly / MM_POR_PIE) + z.Multiply(lz / MM_POR_PIE)
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


def propiedades_soldadura(objeto, props):
    fijados = []
    for nombre in sorted(props):
        p = props[nombre]
        if not p.CanWrite:
            continue
        if re.search(r"Thickness|Throat", nombre) and p.PropertyType.Name == "Double":
            fijados.append(fijar(objeto, props, nombre, SOLDADURA_MM))
        elif re.search(r"WeldType", nombre) and p.PropertyType.IsEnum:
            valores = list(System.Enum.GetNames(p.PropertyType))
            elegido = next((v for v in valores if re.search(r"Fillet", v, re.IGNORECASE)), None)
            if elegido:
                fijados.append(fijar(objeto, props, nombre, elegido))
            else:
                fijados.append("{0}: sin valor Fillet (queda {1}); valores: {2}".format(nombre, leer(objeto, props, nombre), ", ".join(valores)[:200]))
        elif nombre == "IsClosed" and p.PropertyType.Name == "Boolean":
            fijados.append(fijar(objeto, props, nombre, True))
    return fijados


def conectar(objeto, tipo, piezas):
    metodo = next((m for m in tipo.GetMethods() if m.Name == "Connect" and len(m.GetParameters()) == 2 and m.GetParameters()[0].ParameterType.IsArray), None)
    if metodo is None:
        return "sin Connect(FilerObject[], eAssemblyLocation)"
    ps = metodo.GetParameters()
    T_Elem = ps[0].ParameterType.GetElementType()
    arr = System.Array.CreateInstance(T_Elem, len(piezas))
    for i, pieza in enumerate(piezas):
        arr[i] = pieza
    valores = list(System.Enum.GetNames(ps[1].ParameterType))
    sitio = next((v for v in valores if re.search(r"Shop", v)), valores[0])
    metodo.Invoke(objeto, System.Array[System.Object]([arr, System.Enum.Parse(ps[1].ParameterType, sitio)]))
    return "OK con {0} pieza(s), {1}".format(len(piezas), sitio)


hw, hh = ancho / 2.0, alto / 2.0
contorno = [(-hw, -hh), (hw, -hh), (hw, hh), (-hw, hh)]
ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(3, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    # 3a) El objeto de Advance Steel de la barra
    viga, intentos = filer_de(barra, obj, T_Filer)
    for linea in intentos:
        print("   puente: " + linea)
    resultados["barra AS"] = "{0}".format(viga.GetType().Name if viga is not None else "NO encontrada (ver los intentos)")
    print("   barra de Advance Steel: " + resultados["barra AS"])
    # 3b) Plancha de apoyo
    arr = System.Array.CreateInstance(T_Point3d, 4)
    medio = LADO_PLANCHA_MM / 2.0
    for i, (lx, ly) in enumerate(((-medio, -medio), (medio, -medio), (medio, medio), (-medio, medio))):
        arr[i] = punto_local(lx, ly, ESPESOR_MM / 2.0)
    plano = ctor_plane.Invoke(System.Array[System.Object]([punto_local(0, 0, ESPESOR_MM / 2.0), vector_as(z)]))
    plancha = ctor_plate.Invoke(System.Array[System.Object]([plano, arr, ESPESOR_MM]))
    write_plate.Invoke(plancha, None)
    resultados["plancha de apoyo"] = "escrita (200 x 200 x 9,525 perpendicular a la barra en su extremo)"
    print("   " + resultados["plancha de apoyo"])
    piezas = [p for p in (viga, plancha) if p is not None]
    # 3c) WeldLine con el contorno de la huella
    if ctor_linea is not None:
        try:
            puntos = System.Array.CreateInstance(T_Point3d, 5)
            for i, (lx, ly) in enumerate(contorno + [contorno[0]]):
                puntos[i] = punto_local(lx, ly, 0.0)
            linea = ctor_linea.Invoke(System.Array[System.Object]([puntos, vector_as(x), vector_as(y)]))
            print("   WeldLine creada: " + "; ".join(propiedades_soldadura(linea, props_linea)))
            try:
                resultados["WeldLine.Connect"] = conectar(linea, T_WeldLine, piezas)
            except Exception as error:
                resultados["WeldLine.Connect"] = "FALLO " + error_texto(error)
            print("   WeldLine.Connect: " + resultados["WeldLine.Connect"])
            write = T_WeldLine.GetMethod("WriteToDb")
            write.Invoke(linea, None)
            resultados["WeldLine"] = "WriteToDb OK (contorno cerrado de {0:.0f} x {1:.0f}, {2} mm); Length={3}; IsClosed={4}".format(
                ancho, alto, SOLDADURA_MM, leer(linea, props_linea, "Length"), leer(linea, props_linea, "IsClosed"))
        except Exception as error:
            resultados["WeldLine"] = "FALLO " + error_texto(error)
        print("   WeldLine: " + resultados["WeldLine"])
    else:
        resultados["WeldLine"] = "sin el constructor (Point3d[], Vector3d, Vector3d)"
    # 3d) WeldPoint en la primera esquina
    if ctor_punto is not None:
        try:
            punto = ctor_punto.Invoke(System.Array[System.Object]([punto_local(contorno[0][0], contorno[0][1], 0.0), vector_as(x), vector_as(y)]))
            print("   WeldPoint creado: " + "; ".join(propiedades_soldadura(punto, props_punto)))
            try:
                resultados["WeldPoint.Connect"] = conectar(punto, T_WeldPoint, piezas)
            except Exception as error:
                resultados["WeldPoint.Connect"] = "FALLO " + error_texto(error)
            T_WeldPoint.GetMethod("WriteToDb").Invoke(punto, None)
            resultados["WeldPoint"] = "WriteToDb OK"
        except Exception as error:
            resultados["WeldPoint"] = "FALLO " + error_texto(error)
        print("   WeldPoint: " + resultados["WeldPoint"])
    else:
        resultados["WeldPoint"] = "sin el constructor (Point3d, Vector3d, Vector3d)"
    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    paso(4, "Elementos nuevos con la transaccion abierta: {0}".format(len(ids_durante - ids_antes)))
except Exception as error:
    print("   ERROR " + error_texto(error))
    print(traceback.format_exc())
finally:
    try:
        if tx is not None:
            if CANCELAR_AL_FINAL and metodo_cancelar is not None:
                metodo_cancelar.Invoke(tx, None)
                paso(5, "CancelTransaction() hecho: nada debe quedar en la copia.")
            else:
                tipo_tx.GetMethod("Commit").Invoke(tx, None)
                paso(5, "Commit() hecho (existe {0}): mira la soldadura, captura y deshaz con Ctrl+Z; el sondeo 13 limpia lo que quede.".format(MARCA_COMMIT))
    except Exception as error:
        paso(5, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0}".format(doc.IsModifiable))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
nuevos = sorted(ids_despues - ids_antes)
paso(6, "Elementos nuevos tras cerrar: {0} ({1})".format(len(nuevos), "se esperan 0" if CANCELAR_AL_FINAL else "con Commit deben ser la plancha y las soldaduras"))
for i in nuevos[:20]:
    el = doc.GetElement(DB.ElementId(System.Int64(i)))
    try:
        cat = el.Category.Name if el.Category is not None else "-"
    except Exception:
        cat = "?"
    print("   [{0}] {1} | {2}".format(i, el.GetType().Name, cat))
paso(7, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 26-soldadura-weldline ===")
