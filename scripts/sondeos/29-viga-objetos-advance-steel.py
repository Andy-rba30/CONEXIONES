# -*- coding: utf-8 -*-
# Sondeo 29 (ronda 12d, direct_weld): LA VIGA DE ADVANCE STEEL recorriendo LOS OBJETOS DEL MODELO de Advance Steel, y el corte BeamShortening.
# Por que: el sondeo 28 (ronda 12c, resultados-fase-12c.md 12c-3) no encontro la viga de la diagonal 1249631 "por ningun camino",
# pero el recorrido de los objetos del modelo NUNCA CORRIO: el 28 solo invocaba metodos estaticos SIN parametros que devolvieran una
# lista de ObjectId, y el metodo real de la API de Advance Steel, DatabaseManager.GetModelObjectIds(ObjectId[]&) (y sus variantes
# con ClassTypeFilter y Database), recibe la lista como PARAMETRO DE SALIDA. El 28 lo listo ("GetModelObjectIds(ObjectId[]&) -> Void")
# y no lo llamo; por eso no imprimio "tipos recorridos". Este sondeo, sobre la copia y cancelando al final:
#   1) el lado de Revit del puente, como el 28: AddFabricationInformationForRevitElements y GetFabricationUniqueID (el Guid);
#   2) dentro de la FabricationTransaction: DocumentManager.GetCurrentDatabase() / CADDocumentManager.GetCurrentDwg() (que tipo
#      devuelven y sus metodos con ObjectId), y DatabaseManager.GetModelObjectIds(ObjectId[]&) con el parametro de salida;
#      despues la variante con ClassTypeFilter (solo Beam) si el filtro se puede construir;
#   3) abre cada ObjectId con DatabaseManager.Open(ObjectId, eOpenMode) o FilerObject.GetFilerObject(ObjectId), cuenta los tipos,
#      imprime las primeras vigas (tipo, Handle, inicio y fin en mm: para comparar el origen de Advance Steel con el de Revit) y
#      busca la de la diagonal por el Guid (cualquier metodo o propiedad del objeto que se llame *UniqueId*, *Guid* o Handle) y,
#      si ninguno coincide, por geometria (inicio y fin a menos de 50 mm de los extremos de la barra de Revit, en cualquier orden);
#   4) con la viga: sus metodos con Feature/Length/Shorten/Cut, medidas ANTES, BeamShortening(eEnd, 60) + AddFeature con
#      AngleOnY = 90 - angulo con el cordon, medidas DESPUES (lo que los sondeos 25 y 28 iban a hacer);
#   5) WeldLine cerrada sobre la huella de la diagonal en la cara del cordon, conectada a la viga (Connect kInShop) y WriteToDb;
#   6) elementos nuevos con la transaccion abierta y tras cerrarla (se esperan 0 con CancelTransaction).
# Para VER el corte: crea antes %TEMP%\sondeo29-commit.txt (PowerShell: New-Item $env:TEMP\sondeo29-commit.txt) y repite: hace Commit
# en vez de cancelar; despues captura y deshaz con Ctrl+Z (o cierra la copia sin guardar; el sondeo 13 limpia restos).
# Barras por defecto: diagonal 1249631 y cordon 1245533 (nudo en K del paso 11-5); con 2 barras seleccionadas, la mas horizontal es el cordon.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\29-viga-objetos-advance-steel.py -SinTransaccion -TimeoutSec 900
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import math
import os
import re
import traceback

ID_DIAGONAL = 1249631
ID_CORDON = 1245533
MM_POR_PIE = 304.8
LONGITUD_MM = 60.0
SOLDADURA_MM = 5.0
TOLERANCIA_GEOMETRIA_MM = 50.0
MAX_OBJETOS = 20000
VIGAS_A_IMPRIMIR = 6
MARCA_COMMIT = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo29-commit.txt")
CANCELAR_AL_FINAL = not os.path.exists(MARCA_COMMIT)


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


def tupla_mm(t):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(*t)


def error_texto(error):
    interna = getattr(error, "InnerException", None)
    return "{0}: {1}{2}".format(type(error).__name__, str(error)[:200], (" | interna: " + str(interna)[:200]) if interna is not None else "")


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
        if baja.startswith("as") and baja.endswith("mgd") or "advancesteel" in baja or "steelconnections" in baja or baja == "revitapisteel":
            lista.append(a)
    return lista


def tipos_de(ensamblado):
    try:
        return list(ensamblado.GetExportedTypes())
    except Exception as error:
        tipos = getattr(getattr(error, "clsException", None), "Types", None)
        return [t for t in tipos if t is not None] if tipos is not None else []


def tipo_llamado(ensamblados, patron):
    for e in ensamblados:
        for t in tipos_de(e):
            if re.search(patron, t.FullName):
                return t
    return None


def nombres_parametros(metodo):
    return [p.ParameterType.Name for p in metodo.GetParameters()]


def firma(metodo):
    return "{0}({1}) -> {2}".format(metodo.Name, ", ".join(nombres_parametros(metodo)), metodo.ReturnType.Name)


def buscar_ctor(tipo, nombres):
    for c in tipo.GetConstructors():
        if nombres_parametros(c) == list(nombres):
            return c
    return None


def metodos_estaticos(tipo):
    from System.Reflection import BindingFlags
    try:
        return list(tipo.GetMethods(BindingFlags.Public | BindingFlags.Static))
    except Exception:
        return []


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
        return str(prop.GetValue(objeto, None))[:120]
    except Exception as error:
        return "ERROR {0}".format(str(error)[:60])


def barra(eid):
    el = doc.GetElement(DB.ElementId(System.Int64(eid)))
    if el is None or not isinstance(el, DB.FamilyInstance) or not isinstance(el.Location, DB.LocationCurve):
        return None
    return el


def extension_mm(el, indice):
    bip = DB.BuiltInParameter.START_EXTENSION if indice == 0 else DB.BuiltInParameter.END_EXTENSION
    p = el.get_Parameter(bip)
    return mm(p.AsDouble()) if p is not None and p.HasValue else None


def punto_de(objeto, nombre):
    """Un metodo sin parametros del objeto que devuelva un Point3d, como (x, y, z) en mm; None si no existe o falla."""
    try:
        m = objeto.GetType().GetMethod(nombre, System.Type.EmptyTypes)
        if m is None:
            return None
        p = m.Invoke(objeto, None)
        if p is None:
            return None
        try:
            return (float(p.x), float(p.y), float(p.z))
        except Exception:
            return (float(p.X), float(p.Y), float(p.Z))
    except Exception:
        return None


def distancia(a, b):
    return math.sqrt(sum((a[i] - b[i]) ** 2 for i in range(3)))


def extremos_de(objeto):
    """(inicio, fin) en mm de un objeto con GetPointAtStart/GetPointAtEnd (o parecidos); (None, None) si no los tiene."""
    nombres = [m.Name for m in objeto.GetType().GetMethods() if len(m.GetParameters()) == 0 and m.ReturnType.Name == "Point3d"]
    inicio = next((punto_de(objeto, n) for n in nombres if re.search(r"Start", n)), None)
    fin = next((punto_de(objeto, n) for n in nombres if re.search(r"End", n)), None)
    return inicio, fin


def parece_la_barra(objeto, extremos_mm):
    inicio, fin = extremos_de(objeto)
    if inicio is None or fin is None:
        return False
    a, b = extremos_mm
    directo = distancia(inicio, a) <= TOLERANCIA_GEOMETRIA_MM and distancia(fin, b) <= TOLERANCIA_GEOMETRIA_MM
    inverso = distancia(inicio, b) <= TOLERANCIA_GEOMETRIA_MM and distancia(fin, a) <= TOLERANCIA_GEOMETRIA_MM
    return directo or inverso


def identificadores_de(objeto):
    """Lo que el objeto diga de si mismo que parezca un identificador: metodos sin parametros y propiedades *UniqueId*, *Guid*, Handle."""
    datos = {}
    try:
        for m in objeto.GetType().GetMethods():
            if len(m.GetParameters()) == 0 and re.search(r"UniqueId|Guid|Handle", m.Name) and not m.Name.startswith("set_"):
                try:
                    datos[m.Name] = str(m.Invoke(objeto, None))
                except Exception as error:
                    datos[m.Name] = "ERROR " + str(error)[:40]
        for p in objeto.GetType().GetProperties():
            if re.search(r"UniqueId|Guid|Handle", p.Name) and p.CanRead:
                try:
                    datos[p.Name] = str(p.GetValue(objeto, None))
                except Exception as error:
                    datos[p.Name] = "ERROR " + str(error)[:40]
    except Exception:
        pass
    return datos


def normaliza_guid(texto):
    return re.sub(r"[^0-9a-f]", "", (texto or "").lower())


def abrir_objeto(oid):
    """El FilerObject de un ObjectId: DatabaseManager.Open(ObjectId, eOpenMode) y, si no, FilerObject.GetFilerObject(ObjectId)."""
    if metodo_open is not None:
        try:
            return metodo_open.Invoke(None, System.Array[System.Object]([oid, valor_modo_lectura]))
        except Exception:
            pass
    if metodo_get_filer is not None:
        try:
            return metodo_get_filer.Invoke(None, System.Array[System.Object]([oid]))
        except Exception:
            pass
    return None


def medir_viga(viga, props):
    datos = []
    for nombre in ("GetSysLength", "GetLength", "GetSystemLength", "GetLengthOfBeam", "GetPointAtStart", "GetPointAtEnd"):
        m = viga.GetType().GetMethod(nombre, System.Type.EmptyTypes)
        if m is not None:
            try:
                datos.append("{0}()={1}".format(nombre, str(m.Invoke(viga, None))[:60]))
            except Exception as error:
                datos.append("{0}() ERROR {1}".format(nombre, str(error)[:60]))
    for nombre in ("SysLength", "Length", "GeomExtents"):
        if nombre in props:
            datos.append("{0}={1}".format(nombre, leer(viga, props, nombre)))
    m = viga.GetType().GetMethod("GetFeatures", System.Type.EmptyTypes)
    if m is not None:
        try:
            feats = m.Invoke(viga, None)
            datos.append("GetFeatures()={0}".format(len(list(feats)) if feats is not None else "None"))
        except Exception as error:
            datos.append("GetFeatures() ERROR {0}".format(str(error)[:60]))
    return " | ".join(datos) or "(ningun metodo de longitud reconocido)"


def conectar(objeto, tipo, piezas):
    metodo = next((m for m in tipo.GetMethods() if m.Name == "Connect" and len(m.GetParameters()) == 2 and m.GetParameters()[0].ParameterType.IsArray), None)
    if metodo is None:
        return "sin Connect(FilerObject[], eAssemblyLocation)"
    if not piezas:
        return "sin Connect (no hay pieza de Advance Steel)"
    ps = metodo.GetParameters()
    arr = System.Array.CreateInstance(ps[0].ParameterType.GetElementType(), len(piezas))
    for i, pieza in enumerate(piezas):
        arr[i] = pieza
    valores = list(System.Enum.GetNames(ps[1].ParameterType))
    sitio = next((v for v in valores if re.search(r"Shop", v)), valores[0])
    metodo.Invoke(objeto, System.Array[System.Object]([arr, System.Enum.Parse(ps[1].ParameterType, sitio)]))
    return "OK con {0} pieza(s), {1}".format(len(piezas), sitio)


def lista_ids(args, posicion):
    """La lista de ObjectId que un metodo dejo en su parametro de salida, como lista de Python (vacia si no hay)."""
    try:
        valor = args[posicion]
        return list(valor) if valor is not None else []
    except Exception:
        return []


print("=== 29-viga-objetos-advance-steel (ronda 12d) ===")
print("   modo: {0}".format("CANCELAR al final (nada queda)" if CANCELAR_AL_FINAL else "COMMIT al final (existe {0}): captura y Ctrl+Z".format(MARCA_COMMIT)))
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 0) Barras
try:
    seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
except Exception:
    seleccion = []
seleccion = [e for e in seleccion if isinstance(e, DB.FamilyInstance) and isinstance(e.Location, DB.LocationCurve)]
if len(seleccion) == 2:
    def pendiente(e):
        c = e.Location.Curve
        d = c.GetEndPoint(1) - c.GetEndPoint(0)
        return abs(d.Z) / max(d.GetLength(), 1e-9)
    seleccion.sort(key=pendiente)
    cordon, diagonal = seleccion[0], seleccion[1]
else:
    cordon, diagonal = barra(ID_CORDON), barra(ID_DIAGONAL)
if cordon is None or diagonal is None:
    print("PARADA: no encuentro el cordon {0} o la diagonal {1}; selecciona las dos barras y repite.".format(ID_CORDON, ID_DIAGONAL))
    raise SystemExit
c0, c1 = cordon.Location.Curve.GetEndPoint(0), cordon.Location.Curve.GetEndPoint(1)
d0, d1 = diagonal.Location.Curve.GetEndPoint(0), diagonal.Location.Curve.GetEndPoint(1)
ex = (c1 - c0).Normalize()


def distancia_al_eje(p):
    w = p - c0
    return (w - ex.Multiply(w.DotProduct(ex))).GetLength()


indice = 0 if distancia_al_eje(d0) <= distancia_al_eje(d1) else 1
cerca, lejos = (d0, d1) if indice == 0 else (d1, d0)
dd = (cerca - lejos).Normalize()
angulo_con_cordon = math.degrees(math.acos(min(1.0, abs(dd.DotProduct(ex)))))
extremos_mm = ((mm(d0.X), mm(d0.Y), mm(d0.Z)), (mm(d1.X), mm(d1.Y), mm(d1.Z)))
paso(0, "Cordon [{0}] {1} -> {2}; diagonal [{3}] {4} -> {5}; extremo que llega: {6} {7}; angulo con el cordon {8:.1f}; extension actual {9} mm".format(
    cordon.Id.Value, xyz_mm(c0), xyz_mm(c1), diagonal.Id.Value, xyz_mm(d0), xyz_mm(d1), "inicio" if indice == 0 else "fin", xyz_mm(cerca), angulo_con_cordon, extension_mm(diagonal, indice)))

# 1) Transaccion y tipos de Advance Steel
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
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 29 viga de Advance Steel"]
if ctor_tx is None:
    print("PARADA: no hay FabricationTransaction(Document, Boolean, String).")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit (Advance Steel no activo).")
    raise SystemExit
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
T_Beam = obj.GetType("Autodesk.AdvanceSteel.Modelling.Beam")
T_Short = obj.GetType("Autodesk.AdvanceSteel.Modelling.BeamShortening")
T_WeldLine = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldLine")
if T_Plate is None or T_Short is None or T_WeldLine is None:
    print("PARADA: faltan Plate, BeamShortening o WeldLine en ASObjectsMgd.")
    raise SystemExit
T_Filer = T_Plate
while T_Filer is not None and T_Filer.Name != "FilerObject":
    T_Filer = T_Filer.BaseType
ctor_plate = buscar_ctor(T_Plate, ["Plane", "Point3d[]", "Double"])
T_Point3d = ctor_plate.GetParameters()[1].ParameterType.GetElementType() if ctor_plate is not None else None
T_Vector3d = T_Point3d.Assembly.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d") if T_Point3d is not None else None
ctor_p = buscar_ctor(T_Point3d, ["Double", "Double", "Double"]) if T_Point3d is not None else None
ctor_v = buscar_ctor(T_Vector3d, ["Double", "Double", "Double"]) if T_Vector3d is not None else None
ctor_short = buscar_ctor(T_Short, ["eEnd", "Double"])
ctor_linea = buscar_ctor(T_WeldLine, ["Point3d[]", "Vector3d", "Vector3d"])
if None in (ctor_short, ctor_linea, ctor_p, ctor_v) or T_Filer is None:
    print("PARADA: falta BeamShortening(eEnd, Double), WeldLine(Point3d[], Vector3d, Vector3d), la geometria o FilerObject.")
    raise SystemExit
T_eEnd = ctor_short.GetParameters()[0].ParameterType
nombres_eEnd = list(System.Enum.GetNames(T_eEnd))
props_short = propiedades_de(T_Short)
props_linea = propiedades_de(T_WeldLine)
quiero = r"End" if indice == 1 else r"Start"
nombre_extremo = next((n for n in nombres_eEnd if re.search(quiero, n, re.IGNORECASE)), nombres_eEnd[0])
valor_extremo = System.Enum.Parse(T_eEnd, nombre_extremo)
angulo_corte = 90.0 - angulo_con_cordon
paso(1, "Transaccion {0}; Beam: {1}; BeamShortening(eEnd, Double) y WeldLine(Point3d[], Vector3d, Vector3d): si; FilerObject en {2}; eEnd: {3}".format(
    tipo_tx.FullName, T_Beam.FullName if T_Beam is not None else "NO esta en ASObjectsMgd", T_Filer.Assembly.GetName().Name, ", ".join(nombres_eEnd)))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


# La huella de la diagonal sobre la cara del cordon (como en el 28): X = eje del cordon, Y = hacia la diagonal, Z = normal del plano
normal_plano = DB.XYZ.BasisY if abs(ex.DotProduct(DB.XYZ.BasisY)) < 0.9 else DB.XYZ.BasisX
normal_plano = (normal_plano - ex.Multiply(normal_plano.DotProduct(ex))).Normalize()
ey = normal_plano.CrossProduct(ex).Normalize()
if (cerca - c0).DotProduct(ey) < 0:
    ey = ey.Negate()


def medida_mm(simbolo, bip, nombres):
    try:
        p = simbolo.get_Parameter(bip)
        if p is not None and p.HasValue:
            return mm(p.AsDouble())
    except Exception:
        pass
    for n in nombres:
        p = simbolo.LookupParameter(n)
        if p is not None and p.HasValue and p.StorageType == DB.StorageType.Double:
            return mm(p.AsDouble())
    return None


ancho_nominal = medida_mm(cordon.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, ["b", "B", "Width"]) or 203.2
canto_nominal = medida_mm(cordon.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, ["d", "h", "H", "Ht", "Height"]) or 304.8
giro = cordon.get_Parameter(DB.BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE)
giro_deg = math.degrees(giro.AsDouble()) if giro is not None and giro.HasValue else 0.0
en_plano = ancho_nominal if abs(math.sin(math.radians(giro_deg))) > abs(math.cos(math.radians(giro_deg))) else canto_nominal
medio_canto = en_plano / 2.0
ancho_diag = medida_mm(diagonal.Symbol, DB.BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, ["b", "B", "Width"]) or 63.5
seno = abs(dd.DotProduct(ey))
huella = ancho_diag / max(seno, 1e-6)
avance = dd.DotProduct(ey)
s_cara = (medio_canto / MM_POR_PIE - (cerca - c0).DotProduct(ey)) / avance if abs(avance) > 1e-6 else 0.0
centro_cara = cerca + dd.Multiply(s_cara)
paso(2, "Cordon: en el plano {0:.1f} (medio canto {1:.1f}, giro {2:.0f}); diagonal ancho {3:.1f}, huella en la cara {4:.1f} mm, centro {5}".format(
    en_plano, medio_canto, giro_deg, ancho_diag, huella, xyz_mm(centro_cara)))

ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
metodo_open = metodo_get_filer = valor_modo_lectura = None
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(3, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    ensamblados = ensamblados_acero()

    # 3a) El lado de Revit del puente: el Guid de la diagonal
    guid = None
    try:
        ids = System.Collections.Generic.List[DB.ElementId]()
        ids.Add(diagonal.Id)
        anadidos = DB.Steel.SteelElementProperties.AddFabricationInformationForRevitElements(doc, ids)
        print("   puente: AddFabricationInformationForRevitElements -> {0}".format(anadidos.Count if anadidos is not None else "None"))
    except Exception as error:
        print("   puente: AddFabricationInformationForRevitElements ERROR " + error_texto(error))
    try:
        guid = DB.Steel.SteelElementProperties.GetFabricationUniqueID(doc, DB.Reference(diagonal))
        print("   puente: GetFabricationUniqueID -> {0}".format(guid))
    except Exception as error:
        print("   puente: GetFabricationUniqueID ERROR " + error_texto(error))
    guid_norm = normaliza_guid(str(guid) if guid is not None else "")

    # 3b) Los gestores de la base de datos de Advance Steel
    T_DbMgr = tipo_llamado(ensamblados, r"\.DatabaseManager$")
    T_DocMgr = tipo_llamado(ensamblados, r"DocumentManagement\.DocumentManager$")
    T_CadDocMgr = tipo_llamado(ensamblados, r"CADDocumentManager$")
    T_Filtro = tipo_llamado(ensamblados, r"ClassTypeFilter$")
    print("   tipos: DatabaseManager={0}; DocumentManager={1}; CADDocumentManager={2}; ClassTypeFilter={3}".format(
        T_DbMgr.FullName if T_DbMgr is not None else "NO", T_DocMgr.FullName if T_DocMgr is not None else "NO",
        T_CadDocMgr.FullName if T_CadDocMgr is not None else "NO", T_Filtro.FullName if T_Filtro is not None else "NO"))
    for T, nombre in ((T_DocMgr, "GetCurrentDatabase"), (T_CadDocMgr, "GetCurrentDatabase"), (T_CadDocMgr, "GetCurrentDwg")):
        if T is None:
            continue
        m = next((x for x in metodos_estaticos(T) if x.Name == nombre and len(x.GetParameters()) == 0), None)
        if m is None:
            continue
        try:
            bd = m.Invoke(None, None)
            if bd is None:
                print("   {0}.{1}() -> None".format(T.Name, nombre))
                continue
            con_ids = sorted(set(firma(x) for x in bd.GetType().GetMethods() if re.search(r"ObjectId", x.ReturnType.Name) or any(re.search(r"ObjectId", n) for n in nombres_parametros(x))))
            print("   {0}.{1}() -> {2}; metodos con ObjectId: {3}".format(T.Name, nombre, bd.GetType().FullName, "; ".join(con_ids)[:600] or "ninguno"))
        except Exception as error:
            print("   {0}.{1}() ERROR {2}".format(T.Name, nombre, error_texto(error)))

    # 3c) Como abrir un ObjectId
    if T_DbMgr is not None:
        for m in metodos_estaticos(T_DbMgr):
            ps = m.GetParameters()
            if m.Name == "Open" and len(ps) == 2 and ps[0].ParameterType.Name == "ObjectId" and ps[1].ParameterType.IsEnum:
                modos = list(System.Enum.GetNames(ps[1].ParameterType))
                lectura = next((v for v in modos if re.search(r"Read", v)), modos[0])
                metodo_open, valor_modo_lectura = m, System.Enum.Parse(ps[1].ParameterType, lectura)
                print("   abrir con: DatabaseManager.Open(ObjectId, {0}.{1}) (modos: {2})".format(ps[1].ParameterType.Name, lectura, ", ".join(modos)))
                break
    for m in metodos_estaticos(T_Filer):
        if m.Name == "GetFilerObject" and nombres_parametros(m) == ["ObjectId"]:
            metodo_get_filer = m
            print("   abrir con (reserva): FilerObject.GetFilerObject(ObjectId)")
            break
    if metodo_open is None and metodo_get_filer is None:
        print("   SIN manera de abrir un ObjectId (ni DatabaseManager.Open ni FilerObject.GetFilerObject)")

    # 3d) GetModelObjectIds con el parametro de salida: sin filtro, con filtro de Beam y con la base de datos actual
    listas = []
    if T_DbMgr is None:
        print("   DatabaseManager no esta en ningun ensamblado cargado: sin recorrido")
    else:
        variantes = [m for m in metodos_estaticos(T_DbMgr) if m.Name == "GetModelObjectIds"]
        print("   GetModelObjectIds, variantes: " + "; ".join(firma(m) for m in variantes))
        filtro = None
        if T_Filtro is not None and T_Beam is not None:
            try:
                ctor_f = buscar_ctor(T_Filtro, [])
                filtro = ctor_f.Invoke(None) if ctor_f is not None else None
                if filtro is not None:
                    nombres_f = sorted(set(m.Name for m in T_Filtro.GetMethods() if re.search(r"Accept|Reject|Append|Add", m.Name)))
                    hechos = []
                    for nombre_m in ("RejectAllFirst",):
                        m = T_Filtro.GetMethod(nombre_m, System.Type.EmptyTypes)
                        if m is not None:
                            m.Invoke(filtro, None)
                            hechos.append(nombre_m + "()")
                    m = next((x for x in T_Filtro.GetMethods() if re.search(r"AppendAcceptedClass|AddAcceptedClass|AcceptClass", x.Name) and len(x.GetParameters()) == 1), None)
                    if m is not None:
                        m.Invoke(filtro, System.Array[System.Object]([T_Beam]))
                        hechos.append("{0}(Beam)".format(m.Name))
                    print("   ClassTypeFilter: metodos {0}; hecho: {1}".format(", ".join(nombres_f)[:300], ", ".join(hechos) or "nada (sin RejectAllFirst/AppendAcceptedClass)"))
            except Exception as error:
                filtro = None
                print("   ClassTypeFilter no se pudo preparar: " + error_texto(error))
        bd_actual = None
        if T_DocMgr is not None:
            m = next((x for x in metodos_estaticos(T_DocMgr) if x.Name == "GetCurrentDatabase" and len(x.GetParameters()) == 0), None)
            try:
                bd_actual = m.Invoke(None, None) if m is not None else None
            except Exception:
                bd_actual = None
        for m in variantes:
            ps = m.GetParameters()
            if len(ps) == 0 or not ps[0].IsOut and not ps[0].ParameterType.IsByRef:
                continue
            args = None
            etiqueta = firma(m)
            if len(ps) == 1:
                args = System.Array[System.Object]([None])
            elif len(ps) == 2 and filtro is not None:
                args = System.Array[System.Object]([None, filtro])
                etiqueta += " [solo Beam]"
            elif len(ps) == 3 and filtro is not None and bd_actual is not None:
                args = System.Array[System.Object]([None, filtro, bd_actual])
                etiqueta += " [solo Beam, base actual]"
            if args is None:
                print("   {0}: no se llama (falta el filtro o la base de datos)".format(etiqueta))
                continue
            try:
                m.Invoke(None, args)
                ids_as = lista_ids(args, 0)
                print("   {0} -> {1} ObjectId".format(etiqueta, len(ids_as)))
                listas.append((etiqueta, ids_as))
            except Exception as error:
                print("   {0} ERROR {1}".format(etiqueta, error_texto(error)))

    # 3e) Recorrido: tipos, las primeras vigas y la de la diagonal (por Guid y por geometria)
    viga = None
    camino = None
    recuento = {}
    vigas_vistas = 0
    for etiqueta, ids_as in listas:
        if viga is not None:
            break
        recuento = {}
        vigas_vistas = 0
        candidata_geometria = None
        for n, oid in enumerate(ids_as):
            if n >= MAX_OBJETOS:
                print("   {0}: parado en {1} objetos (MAX_OBJETOS)".format(etiqueta, MAX_OBJETOS))
                break
            fo = abrir_objeto(oid)
            if fo is None:
                recuento["(no abierto)"] = recuento.get("(no abierto)", 0) + 1
                continue
            nombre_tipo = fo.GetType().Name
            recuento[nombre_tipo] = recuento.get(nombre_tipo, 0) + 1
            es_viga = T_Beam is not None and T_Beam.IsAssignableFrom(fo.GetType()) or re.search(r"Beam", nombre_tipo) is not None
            if not es_viga:
                continue
            idents = identificadores_de(fo)
            inicio, fin = extremos_de(fo)
            if vigas_vistas < VIGAS_A_IMPRIMIR:
                print("   viga {0}: {1}; {2}; inicio {3}, fin {4}".format(vigas_vistas + 1, fo.GetType().FullName,
                      "; ".join("{0}={1}".format(k, v[:40]) for k, v in sorted(idents.items())) or "sin identificadores",
                      tupla_mm(inicio) if inicio else "?", tupla_mm(fin) if fin else "?"))
            vigas_vistas += 1
            if guid_norm and any(normaliza_guid(v) == guid_norm for v in idents.values()):
                viga, camino = fo, "{0}: Guid de Revit coincide con {1}".format(etiqueta, ", ".join(k for k, v in idents.items() if normaliza_guid(v) == guid_norm))
                break
            if candidata_geometria is None and parece_la_barra(fo, extremos_mm):
                candidata_geometria = (fo, "{0}: inicio y fin a menos de {1:.0f} mm de los extremos de la barra de Revit (ningun identificador coincide con el Guid {2})".format(etiqueta, TOLERANCIA_GEOMETRIA_MM, guid))
        print("   {0}: tipos recorridos: {1}; vigas vistas: {2}".format(etiqueta, ", ".join("{0}={1}".format(k, v) for k, v in sorted(recuento.items(), key=lambda kv: -kv[1])[:25]) or "ninguno", vigas_vistas))
        if viga is None and candidata_geometria is not None:
            viga, camino = candidata_geometria
    print("   extremos de la barra de Revit en mm: {0} y {1} (si las vigas de arriba estan en otro origen o en otra unidad, es lo que hay que casar)".format(tupla_mm(extremos_mm[0]), tupla_mm(extremos_mm[1])))
    if viga is None:
        resultados["viga AS"] = "NO encontrada ({0} lista(s) recorrida(s), {1} viga(s) vista(s) en la ultima)".format(len(listas), vigas_vistas)
        print("   viga de Advance Steel: " + resultados["viga AS"])
    else:
        resultados["viga AS"] = "{0} por {1}".format(viga.GetType().FullName, camino)
        print("   viga de Advance Steel: " + resultados["viga AS"])
        props_viga = propiedades_de(viga.GetType())
        metodos_viga = sorted(set(m.Name for m in viga.GetType().GetMethods() if re.search(r"Feature|Length|Extent|Shorten|Cut|Point", m.Name)))
        print("   metodos de la viga con Feature/Length/Extent/Shorten/Cut/Point: {0}".format(", ".join(metodos_viga)[:700]))
        add_feature = next((m for m in viga.GetType().GetMethods() if m.Name == "AddFeature" and len(m.GetParameters()) == 1), None)
        print("   AddFeature({0}): {1}".format(add_feature.GetParameters()[0].ParameterType.Name if add_feature is not None else "-", add_feature is not None))
        antes = medir_viga(viga, props_viga)
        print("   ANTES: " + antes)
        # 4) BeamShortening con angulo
        try:
            corte = ctor_short.Invoke(System.Array[System.Object]([valor_extremo, LONGITUD_MM]))
            fijados = [fijar(corte, props_short, "AngleOnY", angulo_corte), fijar(corte, props_short, "AngleOnZ", 0.0)]
            if "CutStraight" in props_short:
                fijados.append(fijar(corte, props_short, "CutStraight", True))
            print("   BeamShortening({0}, {1}) creado; {2}".format(nombre_extremo, LONGITUD_MM, "; ".join(fijados)))
            if add_feature is None:
                raise Exception("la viga no tiene AddFeature(1 parametro)")
            add_feature.Invoke(viga, System.Array[System.Object]([corte]))
            despues = medir_viga(viga, props_viga)
            print("   DESPUES: " + despues)
            print("   feature: " + "; ".join("{0}={1}".format(n, leer(corte, props_short, n)) for n in ("Length", "AngleOnY", "AngleOnZ", "CutStraight", "CutStraightType", "CutStraightOffset", "InsLength", "End") if n in props_short))
            resultados["BeamShortening"] = "AddFeature OK ({0}, {1} mm, AngleOnY {2:.1f}) | antes: {3} | despues: {4}".format(nombre_extremo, LONGITUD_MM, angulo_corte, antes, despues)
        except Exception as error:
            resultados["BeamShortening"] = "FALLO " + error_texto(error)
        print("   BeamShortening: " + resultados["BeamShortening"])
    # 5) WeldLine cerrada sobre la huella en la cara, conectada a la viga si la hay
    hw = huella / 2.0
    hh = ancho_diag / 2.0
    esquinas = [(-hw, -hh), (hw, -hh), (hw, hh), (-hw, hh)]


    def punto_cara(lx, ly):
        p = centro_cara + ex.Multiply(lx / MM_POR_PIE) + normal_plano.Multiply(ly / MM_POR_PIE)
        return punto_as(p)


    try:
        puntos = System.Array.CreateInstance(T_Point3d, 5)
        for i, (lx, ly) in enumerate(esquinas + [esquinas[0]]):
            puntos[i] = punto_cara(lx, ly)
        linea = ctor_linea.Invoke(System.Array[System.Object]([puntos, vector_as(ex), vector_as(normal_plano)]))
        fijados = [fijar(linea, props_linea, n, SOLDADURA_MM) for n in ("Thickness", "MainEffectiveThroat", "DoubleEffectiveThroat")] + [fijar(linea, props_linea, "IsClosed", True)]
        print("   WeldLine creada: " + "; ".join(fijados))
        piezas = [viga] if viga is not None else []
        try:
            resultados["WeldLine.Connect"] = conectar(linea, T_WeldLine, piezas)
        except Exception as error:
            resultados["WeldLine.Connect"] = "FALLO " + error_texto(error)
        print("   WeldLine.Connect: " + resultados["WeldLine.Connect"])
        T_WeldLine.GetMethod("WriteToDb").Invoke(linea, None)
        resultados["WeldLine"] = "WriteToDb OK ({0}; contorno {1:.0f} x {2:.0f} sobre la cara del cordon, {3} mm); Length={4}".format(
            "conectada a la viga" if piezas else "SIN Connect", huella, ancho_diag, SOLDADURA_MM, leer(linea, props_linea, "Length"))
    except Exception as error:
        resultados["WeldLine"] = "FALLO " + error_texto(error)
    print("   WeldLine: " + resultados["WeldLine"])
    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    paso(6, "Elementos nuevos con la transaccion abierta: {0}; extension del extremo ahora: {1} mm".format(len(ids_durante - ids_antes), extension_mm(diagonal, indice)))
except Exception as error:
    print("   ERROR " + error_texto(error))
    print(traceback.format_exc())
finally:
    try:
        if tx is not None:
            if CANCELAR_AL_FINAL and metodo_cancelar is not None:
                metodo_cancelar.Invoke(tx, None)
                paso(7, "CancelTransaction() hecho: nada debe quedar en la copia.")
            else:
                tipo_tx.GetMethod("Commit").Invoke(tx, None)
                paso(7, "Commit() hecho (existe {0}): mira el extremo de la diagonal, captura y deshaz con Ctrl+Z (o cierra sin guardar).".format(MARCA_COMMIT))
    except Exception as error:
        paso(7, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0} | extension del extremo: {1} mm".format(doc.IsModifiable, extension_mm(diagonal, indice)))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
nuevos = sorted(ids_despues - ids_antes)
paso(8, "Elementos nuevos tras cerrar: {0} ({1})".format(len(nuevos), "se esperan 0" if CANCELAR_AL_FINAL else "con Commit: la WeldLine materializada es 1 de categoria Welds; el corte no crea elementos, cambia la viga"))
for i in nuevos[:10]:
    el = doc.GetElement(DB.ElementId(System.Int64(i)))
    print("   nuevo {0}: {1} | {2}".format(i, el.Category.Name if el is not None and el.Category is not None else "?", type(el).__name__ if el is not None else "?"))
paso(9, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 29-viga-objetos-advance-steel ===")
