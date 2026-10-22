# -*- coding: utf-8 -*-
# Sondeo 28 (ronda 12c, direct_weld): LA VIGA DE ADVANCE STEEL de una barra de Revit, el corte BeamShortening y la WeldLine sin plancha.
# Por que: en la ronda 12b los sondeos 25 y 26 NO encontraron el objeto de Advance Steel de la diagonal 1249631: el puente de Revit
# respondio (AddFabricationInformationForRevitElements -> 0, GetFabricationUniqueID -> un Guid) pero la busqueda solo miro en
# ASObjectsMgd (metodos estaticos con un Guid y metodos que devuelven listas de ObjectId) y no encontro ninguno. Sin viga no se pudo
# probar BeamShortening (el corte inclinado del nudo en K sigue siendo la reserva por el eje, END_CUT_APPROXIMATED) y la WeldLine solo
# se escribio conectada a una plancha (el add-in 0.12.2 la escribe tambien sin plancha en direct_weld: hay que saber si se materializa).
# Este sondeo, sobre la copia y cancelando al final:
#   1) lista los metodos estaticos de SteelElementProperties (RevitAPISteel) con sus firmas (el lado de Revit del puente);
#   2) dentro de la FabricationTransaction lista TODOS los ensamblados cargados de Advance Steel (AS*, Autodesk.AdvanceSteel.*,
#      SteelConnections) y, en cada uno, los tipos cuyo nombre contiene DatabaseManager, DocumentManager, ObjectId o Guid y sus
#      metodos estaticos; si ASCADLinkMgd no esta cargado, lo carga de la carpeta SteelConnections (es el "link" CAD-Revit);
#   3) busca la viga por tres caminos, en todos esos ensamblados: (a) cualquier metodo estatico que reciba un Guid (o un String con
#      el Guid); (b) cualquier metodo estatico sin parametros que devuelva una coleccion de ObjectId, recorriendola con
#      FilerObject.GetFilerObject(ObjectId) o DatabaseManager.Open(ObjectId, OpenMode) y comparando GetUniqueId() con el Guid;
#      (c) si ningun Guid coincide, la geometria: un objeto cuyo tipo se llame Beam (o derive) con un punto de inicio y fin a menos
#      de 50 mm de los extremos de la barra de Revit. Se imprimen los intentos y un recuento de los tipos recorridos;
#   4) con la viga: sus metodos con Feature/Length/Extent/Shorten/Cut, medidas ANTES, BeamShortening(eEnd, 60) + AddFeature con
#      AngleOnY = 90 - angulo con el cordon, medidas DESPUES y la extension del extremo (lo que el 25 iba a hacer);
#   5) WeldLine cerrada sobre la huella de la diagonal en la cara del cordon SIN plancha de apoyo: Connect([viga], kInShop) si hay
#      viga y sin Connect si no; WriteToDb; WeldPoint igual;
#   6) elementos nuevos con la transaccion abierta y tras cerrarla (se esperan 0 con CancelTransaction).
# Para VER el corte y la soldadura: crea antes %TEMP%\sondeo28-commit.txt (PowerShell: New-Item $env:TEMP\sondeo28-commit.txt) y repite:
# hace Commit en vez de cancelar; despues captura y deshaz con Ctrl+Z (o cierra la copia sin guardar; el sondeo 13 limpia restos).
# Barras por defecto: diagonal 1249631 y cordon 1245533 (nudo en K del paso 11-5); con 2 barras seleccionadas, la mas horizontal es el cordon.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\28-viga-advance-steel.py -SinTransaccion -TimeoutSec 900
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
CARPETA_STEEL = r"C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections"
MARCA_COMMIT = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo28-commit.txt")
CANCELAR_AL_FINAL = not os.path.exists(MARCA_COMMIT)


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


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
    """Todos los ensamblados cargados que huelen a Advance Steel / Steel Connections, por nombre."""
    lista = []
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            n = a.GetName().Name
        except Exception:
            continue
        baja = n.lower()
        if baja.startswith("as") and baja.endswith("mgd") or "advancesteel" in baja or "steelconnections" in baja or baja == "revitapisteel" or "dotnetroots" in baja:
            lista.append(a)
    return lista


def tipos_de(ensamblado):
    try:
        return list(ensamblado.GetExportedTypes())
    except Exception as error:
        tipos = getattr(getattr(error, "clsException", None), "Types", None)
        return [t for t in tipos if t is not None] if tipos is not None else []


def nombres_parametros(metodo):
    return [p.ParameterType.Name for p in metodo.GetParameters()]


def firma(metodo):
    return "{0}({1}) -> {2}".format(metodo.Name, ", ".join(nombres_parametros(metodo)), metodo.ReturnType.Name)


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


def metodos_estaticos(tipo):
    from System.Reflection import BindingFlags
    try:
        return list(tipo.GetMethods(BindingFlags.Public | BindingFlags.Static))
    except Exception:
        return []


def punto_de(objeto, nombre):
    """Un metodo sin parametros del objeto que devuelva un Point3d, leido como (x, y, z) en mm; None si no existe."""
    try:
        m = objeto.GetType().GetMethod(nombre, System.Type.EmptyTypes)
        if m is None:
            return None
        p = m.Invoke(objeto, None)
        return (float(p.x), float(p.y), float(p.z)) if p is not None else None
    except Exception:
        try:
            return (float(p.X), float(p.Y), float(p.Z))
        except Exception:
            return None


def distancia(a, b):
    return math.sqrt(sum((a[i] - b[i]) ** 2 for i in range(3)))


def parece_la_barra(objeto, extremos_mm):
    """True si el objeto tiene un punto de inicio y uno de fin a menos de TOLERANCIA de los extremos de la barra de Revit (en cualquier orden)."""
    nombres = [m.Name for m in objeto.GetType().GetMethods() if len(m.GetParameters()) == 0 and m.ReturnType.Name == "Point3d"]
    inicio = next((punto_de(objeto, n) for n in nombres if re.search(r"Start", n)), None)
    fin = next((punto_de(objeto, n) for n in nombres if re.search(r"End", n)), None)
    if inicio is None or fin is None:
        return False, nombres
    a, b = extremos_mm
    directo = distancia(inicio, a) <= TOLERANCIA_GEOMETRIA_MM and distancia(fin, b) <= TOLERANCIA_GEOMETRIA_MM
    inverso = distancia(inicio, b) <= TOLERANCIA_GEOMETRIA_MM and distancia(fin, a) <= TOLERANCIA_GEOMETRIA_MM
    return directo or inverso, nombres


def filer_de(elemento, ensamblados, T_Filer, extremos_mm):
    """La viga de Advance Steel de una barra de Revit: (objeto o None, intentos, recuento de tipos recorridos)."""
    intentos = []
    recuento = {}
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
    guid_texto = str(guid) if guid is not None else ""
    # (a) estaticos con un Guid (o un String) en todos los ensamblados de acero
    for e in ensamblados:
        for t in tipos_de(e):
            for m in metodos_estaticos(t):
                ps = m.GetParameters()
                if len(ps) != 1 or ps[0].ParameterType.Name not in ("Guid", "String"):
                    continue
                if ps[0].ParameterType.Name == "String" and not re.search(r"Guid|Unique|Handle|Id", m.Name):
                    continue
                if not guid_texto:
                    continue
                try:
                    arg = guid if ps[0].ParameterType.Name == "Guid" else guid_texto
                    r = m.Invoke(None, System.Array[System.Object]([arg]))
                    intentos.append("{0}.{1}.{2}({3}) -> {4}".format(e.GetName().Name, t.Name, m.Name, ps[0].ParameterType.Name, r.GetType().Name if r is not None else "None"))
                    if r is not None and T_Filer.IsAssignableFrom(r.GetType()):
                        return r, intentos, recuento
                    if r is not None and r.GetType().Name == "ObjectId":
                        fo = abrir_objeto(r, ensamblados, T_Filer)
                        if fo is not None:
                            intentos.append("   abierto: {0}".format(fo.GetType().Name))
                            return fo, intentos, recuento
                except Exception as error:
                    intentos.append("{0}.{1}.{2}({3}) ERROR {4}".format(e.GetName().Name, t.Name, m.Name, ps[0].ParameterType.Name, str(error)[:100]))
    # (b) y (c) recorrido de los ObjectId del modelo
    for e in ensamblados:
        for t in tipos_de(e):
            for m in metodos_estaticos(t):
                if len(m.GetParameters()) != 0 or not re.search(r"ObjectId", m.ReturnType.Name) or not re.search(r"\[\]|List|Collection|Enumerable", m.ReturnType.Name):
                    continue
                try:
                    lista = m.Invoke(None, None)
                    cuenta = 0
                    por_geometria = None
                    for oid in lista:
                        cuenta += 1
                        fo = abrir_objeto(oid, ensamblados, T_Filer)
                        if fo is None:
                            recuento["(no abierto)"] = recuento.get("(no abierto)", 0) + 1
                            continue
                        nombre_tipo = fo.GetType().Name
                        recuento[nombre_tipo] = recuento.get(nombre_tipo, 0) + 1
                        try:
                            if guid_texto and str(fo.GetUniqueId()) == guid_texto:
                                intentos.append("{0}.{1}.{2}() recorrido: {3} objetos, encontrado por Guid ({4})".format(e.GetName().Name, t.Name, m.Name, cuenta, nombre_tipo))
                                return fo, intentos, recuento
                        except Exception:
                            pass
                        if por_geometria is None and re.search(r"Beam", nombre_tipo):
                            coincide, nombres = parece_la_barra(fo, extremos_mm)
                            if coincide:
                                por_geometria = fo
                                intentos.append("   candidata por geometria: {0} (metodos Point3d: {1})".format(nombre_tipo, ", ".join(nombres)[:200]))
                    intentos.append("{0}.{1}.{2}() recorrido: {3} objetos, {4}".format(e.GetName().Name, t.Name, m.Name, cuenta, "encontrada por geometria" if por_geometria is not None else "no encontrada"))
                    if por_geometria is not None:
                        try:
                            intentos.append("   su GetUniqueId() = {0} (Revit dio {1})".format(por_geometria.GetUniqueId(), guid_texto))
                        except Exception:
                            pass
                        return por_geometria, intentos, recuento
                except Exception as error:
                    intentos.append("{0}.{1}.{2}() ERROR {3}".format(e.GetName().Name, t.Name, m.Name, str(error)[:100]))
    return None, intentos, recuento


def abrir_objeto(oid, ensamblados, T_Filer):
    """El FilerObject de un ObjectId: FilerObject.GetFilerObject(ObjectId) o DatabaseManager.Open(ObjectId, OpenMode)."""
    for m in metodos_estaticos(T_Filer):
        if m.Name == "GetFilerObject" and nombres_parametros(m) == ["ObjectId"]:
            try:
                return m.Invoke(None, System.Array[System.Object]([oid]))
            except Exception:
                pass
    for e in ensamblados:
        for t in tipos_de(e):
            if not re.search(r"DatabaseManager", t.Name):
                continue
            for m in metodos_estaticos(t):
                ps = m.GetParameters()
                if m.Name == "Open" and len(ps) == 2 and ps[0].ParameterType.Name == "ObjectId" and ps[1].ParameterType.IsEnum:
                    try:
                        modo = System.Enum.GetNames(ps[1].ParameterType)
                        lectura = next((v for v in modo if re.search(r"Read", v)), modo[0])
                        return m.Invoke(None, System.Array[System.Object]([oid, System.Enum.Parse(ps[1].ParameterType, lectura)]))
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


print("=== 28-viga-advance-steel (ronda 12c) ===")
print("   modo: {0}".format("CANCELAR al final (nada queda)" if CANCELAR_AL_FINAL else "COMMIT al final (existe {0}): captura y Ctrl+Z".format(MARCA_COMMIT)))
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 0) Barras
seleccion = [doc.GetElement(i) for i in uidoc.Selection.GetElementIds()]
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

# 1) El lado de Revit del puente: SteelElementProperties
try:
    T_SEP = clr.GetClrType(DB.Steel.SteelElementProperties)
    print("1) SteelElementProperties ({0}), metodos estaticos:".format(T_SEP.Assembly.GetName().Name))
    for m in sorted(metodos_estaticos(T_SEP), key=lambda m: m.Name):
        print("   " + firma(m))
except Exception as error:
    print("1) SteelElementProperties no disponible: " + error_texto(error))

# 2) Transaccion y tipos
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
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 28 viga de Advance Steel"]
if ctor_tx is None:
    print("PARADA: no hay FabricationTransaction(Document, Boolean, String).")
    raise SystemExit
metodo_cancelar = tipo_tx.GetMethod("CancelTransaction")
obj = buscar_ensamblado("ASObjectsMgd")
if obj is None:
    print("PARADA: ASObjectsMgd no esta cargado por Revit (Advance Steel no activo).")
    raise SystemExit
T_Plate = obj.GetType("Autodesk.AdvanceSteel.Modelling.Plate")
T_Short = obj.GetType("Autodesk.AdvanceSteel.Modelling.BeamShortening")
T_WeldLine = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldLine")
T_WeldPoint = obj.GetType("Autodesk.AdvanceSteel.Modelling.WeldPoint")
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
ctor_punto = buscar_ctor(T_WeldPoint, ["Point3d", "Vector3d", "Vector3d"]) if T_WeldPoint is not None else None
if None in (ctor_short, ctor_linea, ctor_p, ctor_v) or T_Filer is None:
    print("PARADA: falta BeamShortening(eEnd, Double), WeldLine(Point3d[], Vector3d, Vector3d), la geometria o FilerObject.")
    raise SystemExit
T_eEnd = ctor_short.GetParameters()[0].ParameterType
nombres_eEnd = list(System.Enum.GetNames(T_eEnd))
props_short = propiedades_de(T_Short)
props_linea = propiedades_de(T_WeldLine)
props_punto = propiedades_de(T_WeldPoint) if T_WeldPoint is not None else {}
quiero = r"End" if indice == 1 else r"Start"
nombre_extremo = next((n for n in nombres_eEnd if re.search(quiero, n, re.IGNORECASE)), nombres_eEnd[0])
valor_extremo = System.Enum.Parse(T_eEnd, nombre_extremo)
angulo_corte = 90.0 - angulo_con_cordon
paso(2, "Transaccion {0}; BeamShortening(eEnd, Double) y WeldLine(Point3d[], Vector3d, Vector3d): si; FilerObject en {1}; eEnd: {2}".format(
    tipo_tx.FullName, T_Filer.Assembly.GetName().Name, ", ".join(nombres_eEnd)))


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


# La huella de la diagonal sobre la cara del cordon: el marco del nudo (X = eje del cordon, Y = hacia la diagonal en el plano, Z = normal)
normal_plano = DB.XYZ.BasisY if abs(ex.DotProduct(DB.XYZ.BasisY)) < 0.9 else DB.XYZ.BasisX
normal_plano = (normal_plano - ex.Multiply(normal_plano.DotProduct(ex))).Normalize()
ey = normal_plano.CrossProduct(ex).Normalize()
if (cerca - c0).DotProduct(ey) < 0:
    ey = ey.Negate()
# donde el eje de la diagonal corta al del cordon y donde corta a la cara (la cara a medio canto en el plano: 101,6 en el HSS12X8 girado; se lee del tipo)


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
# El centro de la huella: el punto del eje de la diagonal a medio canto de la recta del cordon (la cara). P(s) = cerca + dd * s con
# dd apuntando hacia el cordon; y(P) = y(cerca) + s * (dd . ey) y se quiere y = medio canto.
avance = dd.DotProduct(ey)
s_cara = (medio_canto / MM_POR_PIE - (cerca - c0).DotProduct(ey)) / avance if abs(avance) > 1e-6 else 0.0
centro_cara = cerca + dd.Multiply(s_cara)
paso(3, "Cordon: ancho nominal {0:.1f}, canto nominal {1:.1f}, Cross-Section Rotation {2:.0f} -> en el plano {3:.1f} (medio canto {4:.1f}); diagonal ancho {5:.1f}, huella en la cara {6:.1f} mm, centro de la huella {7}".format(
    ancho_nominal, canto_nominal, giro_deg, en_plano, medio_canto, ancho_diag, huella, xyz_mm(centro_cara)))

ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(4, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    # 4a) ensamblados de acero cargados (dentro de la transaccion se cargan mas) y los tipos que interesan
    ensamblados = ensamblados_acero()
    print("   ensamblados de acero cargados: " + ", ".join(sorted(a.GetName().Name for a in ensamblados)))
    if buscar_ensamblado("ASCADLinkMgd") is None:
        ruta = os.path.join(CARPETA_STEEL, "ASCADLinkMgd.dll")
        if os.path.exists(ruta):
            try:
                cargado = System.Reflection.Assembly.LoadFrom(ruta)
                ensamblados.append(cargado)
                print("   ASCADLinkMgd no estaba cargado: cargado desde {0} ({1} tipos)".format(ruta, len(tipos_de(cargado))))
            except Exception as error:
                print("   ASCADLinkMgd no se pudo cargar: " + error_texto(error))
        else:
            print("   ASCADLinkMgd.dll no existe en " + CARPETA_STEEL)
    for e in ensamblados:
        for t in tipos_de(e):
            if re.search(r"DatabaseManager|DocumentManager|ObjectId$|Guid|UniqueId|Converter|Link", t.Name):
                estaticos = [firma(m) for m in metodos_estaticos(t)]
                print("   tipo {0}.{1}: {2} estaticos: {3}".format(e.GetName().Name, t.FullName, len(estaticos), "; ".join(sorted(set(estaticos)))[:700]))
    # 4b) la viga
    viga, intentos, recuento = filer_de(diagonal, ensamblados, T_Filer, extremos_mm)
    for linea in intentos:
        print("   puente: " + linea)
    if recuento:
        print("   tipos recorridos: " + ", ".join("{0}={1}".format(k, v) for k, v in sorted(recuento.items(), key=lambda kv: -kv[1])[:25]))
    if viga is None:
        resultados["viga AS"] = "NO encontrada por ningun camino (ver los intentos y los tipos de arriba)"
        print("   " + resultados["viga AS"])
    else:
        props_viga = propiedades_de(viga.GetType())
        resultados["viga AS"] = "{0} ({1})".format(viga.GetType().FullName, intentos[-1][:100])
        print("   viga de Advance Steel: " + viga.GetType().FullName)
        metodos_viga = sorted(set(m.Name for m in viga.GetType().GetMethods() if re.search(r"Feature|Length|Extent|Shorten|Cut|Point", m.Name)))
        print("   metodos de la viga con Feature/Length/Extent/Shorten/Cut/Point: {0}".format(", ".join(metodos_viga)[:700]))
        add_feature = next((m for m in viga.GetType().GetMethods() if m.Name == "AddFeature" and len(m.GetParameters()) == 1), None)
        print("   AddFeature({0}): {1}".format(add_feature.GetParameters()[0].ParameterType.Name if add_feature is not None else "-", add_feature is not None))
        antes = medir_viga(viga, props_viga)
        print("   ANTES: " + antes)
        # 5) BeamShortening con angulo
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
    # 6) WeldLine cerrada sobre la huella en la cara, SIN plancha: con la viga si la hay, sin Connect si no
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
        resultados["WeldLine"] = "WriteToDb OK ({0}; contorno {1:.0f} x {2:.0f} sobre la cara del cordon, {3} mm); Length={4}; IsClosed={5}".format(
            "conectada a la viga" if piezas else "SIN Connect", huella, ancho_diag, SOLDADURA_MM, leer(linea, props_linea, "Length"), leer(linea, props_linea, "IsClosed"))
    except Exception as error:
        resultados["WeldLine"] = "FALLO " + error_texto(error)
    print("   WeldLine: " + resultados["WeldLine"])
    if ctor_punto is not None:
        try:
            punto = ctor_punto.Invoke(System.Array[System.Object]([punto_cara(esquinas[0][0], esquinas[0][1]), vector_as(ex), vector_as(normal_plano)]))
            for n in ("Thickness", "MainEffectiveThroat", "DoubleEffectiveThroat"):
                fijar(punto, props_punto, n, SOLDADURA_MM)
            try:
                resultados["WeldPoint.Connect"] = conectar(punto, T_WeldPoint, [viga] if viga is not None else [])
            except Exception as error:
                resultados["WeldPoint.Connect"] = "FALLO " + error_texto(error)
            T_WeldPoint.GetMethod("WriteToDb").Invoke(punto, None)
            resultados["WeldPoint"] = "WriteToDb OK"
        except Exception as error:
            resultados["WeldPoint"] = "FALLO " + error_texto(error)
        print("   WeldPoint: " + resultados["WeldPoint"])
    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    paso(7, "Elementos nuevos con la transaccion abierta: {0}; extension del extremo ahora: {1} mm".format(len(ids_durante - ids_antes), extension_mm(diagonal, indice)))
except Exception as error:
    print("   ERROR " + error_texto(error))
    print(traceback.format_exc())
finally:
    try:
        if tx is not None:
            if CANCELAR_AL_FINAL and metodo_cancelar is not None:
                metodo_cancelar.Invoke(tx, None)
                paso(8, "CancelTransaction() hecho: nada debe quedar en la copia.")
            else:
                tipo_tx.GetMethod("Commit").Invoke(tx, None)
                paso(8, "Commit() hecho (existe {0}): mira la diagonal y la soldadura, captura y deshaz con Ctrl+Z (o cierra sin guardar).".format(MARCA_COMMIT))
    except Exception as error:
        paso(8, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0} | extension del extremo: {1} mm".format(doc.IsModifiable, extension_mm(diagonal, indice)))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
nuevos = sorted(ids_despues - ids_antes)
paso(9, "Elementos nuevos tras cerrar: {0} ({1})".format(len(nuevos), "se esperan 0" if CANCELAR_AL_FINAL else "con Commit: una WeldLine materializada seria 1 de categoria Structural Connections"))
for i in nuevos[:10]:
    el = doc.GetElement(DB.ElementId(System.Int64(i)))
    print("   nuevo {0}: {1} | {2}".format(i, el.Category.Name if el is not None and el.Category is not None else "?", type(el).__name__ if el is not None else "?"))
paso(10, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 28-viga-advance-steel ===")
