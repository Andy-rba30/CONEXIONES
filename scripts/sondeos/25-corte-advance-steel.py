# -*- coding: utf-8 -*-
# Sondeo 25 (ronda 12b, direct_weld): CORTAR EL EXTREMO de una diagonal con Advance Steel, con los constructores que el
# sondeo 22 listo y no intento ("No se crea ninguno (sin firma conocida)"): BeamShortening(eEnd end, Double length), que tiene
# AngleOnY / AngleOnZ / CutStraight (el "acortar con angulo" de Advance Steel = corte plano inclinado), y como segunda via
# BeamMultiContourNotch(Beam, eEnd, Point3d[] contourWCS, Vector3d normalWCS, Vector3d xvec). El camino de Revit
# (StructuralFramingUtils.SetEndReference) ya se descarto en el 22: CanSetEndReference = False sin union previa.
# Sobre la copia y, por defecto, SIN dejar nada (CancelTransaction al final).
#   1) FabricationTransaction y tipos, como en los sondeos 23 y 24.
#   2) El objeto de Advance Steel de la DIAGONAL de Revit por el puente SteelElementProperties (AddFabricationInformationForRevitElements
#      + GetFabricationUniqueID) y un metodo estatico que reciba el Guid, o el recorrido de los ObjectId del modelo (se anota el camino).
#      Si no se encuentra la viga, el sondeo PARA aqui: sin viga no hay corte posible por esta via.
#   3) Con la viga: se listan sus metodos con Feature/Length/Extents, se leen longitud y GeomExtents ANTES, se crea
#      BeamShortening(eEnd del extremo que llega al cordon, LONGITUD_MM) y se anade con AddFeature; se fijan AngleOnY / AngleOnZ
#      (el angulo entre el corte y la seccion recta: 90 - el angulo de la diagonal con el cordon) y se leen DESPUES longitud,
#      GeomExtents y las propiedades del feature (CutStraight, AngleOnY, AngleOnZ, Length). Si BeamShortening falla, se intenta
#      BeamMultiContourNotch con un rectangulo grande sobre el plano de la cara del cordon.
#   4) Elementos nuevos con la transaccion abierta y tras cerrarla (se esperan 0 con CancelTransaction).
# Para VER el corte en Revit: crea antes %TEMP%\sondeo25-commit.txt (PowerShell: New-Item $env:TEMP\sondeo25-commit.txt) y repite:
# hace Commit en vez de cancelar; despues captura la diagonal y deshaz con Ctrl+Z (o cierra la copia sin guardar).
# Barras por defecto: diagonal 1249631 y cordon 1245533 (nudo en K del paso 11-5); con 2 barras seleccionadas, la mas horizontal es el cordon.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\25-corte-advance-steel.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import math
import os
import re
import traceback

ID_DIAGONAL = 1249631
ID_CORDON = 1245533
MM_POR_PIE = 304.8
LONGITUD_MM = 60.0                 # cuanto se acorta por el eje (el hueco extremo-eje medido en el 22 era 68,7)
MARCA_COMMIT = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo25-commit.txt")
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
        v = prop.GetValue(objeto, None)
        return str(v)[:120]
    except Exception as error:
        return "ERROR {0}".format(str(error)[:60])


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


def barra(eid):
    el = doc.GetElement(DB.ElementId(System.Int64(eid)))
    if el is None or not isinstance(el, DB.FamilyInstance) or not isinstance(el.Location, DB.LocationCurve):
        return None
    return el


def extension_mm(el, indice):
    bip = DB.BuiltInParameter.START_EXTENSION if indice == 0 else DB.BuiltInParameter.END_EXTENSION
    p = el.get_Parameter(bip)
    return mm(p.AsDouble()) if p is not None and p.HasValue else None


def medir_viga(viga, props):
    """Longitud y extension de la viga de Advance Steel por los metodos que tenga (no se da ninguno por supuesto)."""
    datos = []
    for nombre in ("GetSysLength", "GetLength", "GetSystemLength", "GetLengthOfBeam"):
        m = viga.GetType().GetMethod(nombre, System.Type.EmptyTypes)
        if m is not None:
            try:
                datos.append("{0}()={1}".format(nombre, str(m.Invoke(viga, None))[:40]))
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


print("=== 25-corte-advance-steel (ronda 12b) ===")
print("   modo: {0}".format("CANCELAR al final (nada queda)" if CANCELAR_AL_FINAL else "COMMIT al final (existe {0}): captura y Ctrl+Z".format(MARCA_COMMIT)))
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit
if doc.IsModifiable:
    print("PARADA: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
    raise SystemExit

# 1) Barras
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
paso(1, "Cordon [{0}] {1} -> {2}; diagonal [{3}] {4} -> {5}; extremo que llega: {6} ({7}); angulo con el cordon {8:.1f} grados; "
        "extension actual {9} mm".format(cordon.Id.Value, xyz_mm(c0), xyz_mm(c1), diagonal.Id.Value, xyz_mm(d0), xyz_mm(d1),
                                          "inicio" if indice == 0 else "fin", xyz_mm(cerca), angulo_con_cordon, extension_mm(diagonal, indice)))

# 2) Transaccion de acero y tipos
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
            tipo_tx, ctor_tx, args_tx = t, c, [doc, False, "Sondeo 25 corte Advance Steel"]
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
T_Notch = obj.GetType("Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch")
if T_Plate is None or T_Short is None:
    print("PARADA: faltan Plate o BeamShortening en ASObjectsMgd.")
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
ctor_notch = None
if T_Notch is not None:
    for c in T_Notch.GetConstructors():
        if nombres_parametros(c) == ["Beam", "eEnd", "Point3d[]", "Vector3d", "Vector3d"]:
            ctor_notch = c
if ctor_short is None or T_Filer is None:
    print("PARADA: falta BeamShortening(eEnd, Double) o FilerObject.")
    raise SystemExit
T_eEnd = ctor_short.GetParameters()[0].ParameterType
nombres_eEnd = list(System.Enum.GetNames(T_eEnd))
props_short = propiedades_de(T_Short)
paso(2, "Transaccion {0}; BeamShortening(eEnd, Double): si; BeamMultiContourNotch(Beam, eEnd, Point3d[], Vector3d, Vector3d): {1}; "
        "eEnd ({2}): {3}".format(tipo_tx.FullName, ctor_notch is not None, T_eEnd.FullName, ", ".join(nombres_eEnd)))
print("   BeamShortening, propiedades escribibles: {0}".format(", ".join(sorted(n for n, p in props_short.items() if p.CanWrite))[:600]))
for n, p in sorted(props_short.items()):
    if p.PropertyType.IsEnum:
        try:
            print("   enum {0} ({1}): {2}".format(n, p.PropertyType.Name, ", ".join(System.Enum.GetNames(p.PropertyType))[:300]))
        except Exception:
            pass
# el valor de eEnd del extremo que llega al cordon: el que suene a "End" si es el fin de la curva, a "Start" si es el inicio
quiero = r"End" if indice == 1 else r"Start"
nombre_extremo = next((n for n in nombres_eEnd if re.search(quiero, n, re.IGNORECASE)), nombres_eEnd[0])
valor_extremo = System.Enum.Parse(T_eEnd, nombre_extremo)
angulo_corte = 90.0 - angulo_con_cordon   # angulo entre el plano de corte (la cara del cordon) y la seccion recta de la diagonal


def vector_as(v):
    return ctor_v.Invoke(System.Array[System.Object]([v.X, v.Y, v.Z]))


def punto_as(p):
    return ctor_p.Invoke(System.Array[System.Object]([mm(p.X), mm(p.Y), mm(p.Z)]))


ids_antes = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
tx = None
resultados = {}
try:
    tx = ctor_tx.Invoke(System.Array[System.Object](args_tx))
    paso(3, "FabricationTransaction abierta. doc.IsModifiable={0}".format(doc.IsModifiable))
    viga, intentos = filer_de(diagonal, obj, T_Filer)
    for linea in intentos:
        print("   puente: " + linea)
    if viga is None:
        resultados["viga AS"] = "NO encontrada: sin viga de Advance Steel no hay corte por esta via (copia los intentos del puente)"
        print("   " + resultados["viga AS"])
    else:
        props_viga = propiedades_de(viga.GetType())
        resultados["viga AS"] = "{0} por {1}".format(viga.GetType().Name, intentos[-1][:80])
        print("   viga de Advance Steel: " + viga.GetType().FullName)
        metodos_viga = sorted(set(m.Name for m in viga.GetType().GetMethods() if re.search(r"Feature|Length|Extent|Shorten|Cut", m.Name)))
        print("   metodos de la viga con Feature/Length/Extent/Shorten/Cut: {0}".format(", ".join(metodos_viga)[:600]))
        add_feature = next((m for m in viga.GetType().GetMethods() if m.Name == "AddFeature" and len(m.GetParameters()) == 1), None)
        print("   AddFeature({0}): {1}".format(add_feature.GetParameters()[0].ParameterType.Name if add_feature is not None else "-", add_feature is not None))
        antes = medir_viga(viga, props_viga)
        print("   ANTES: " + antes)
        # 3a) BeamShortening con angulo
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
        # 3b) BeamMultiContourNotch con un rectangulo sobre el plano de la cara del cordon (solo si existe el constructor y hay geometria)
        if ctor_notch is not None and None not in (ctor_p, ctor_v):
            try:
                # plano: pasa por el extremo de la diagonal retrasado LONGITUD_MM, normal = eje de la diagonal girado hacia la cara (aprox. la cara del cordon: vertical)
                normal = DB.XYZ.BasisZ if abs(ex.DotProduct(DB.XYZ.BasisZ)) < 0.5 else DB.XYZ.BasisX
                centro = cerca - dd.Multiply(LONGITUD_MM / MM_POR_PIE)
                u = ex
                v = normal.CrossProduct(u).Normalize()
                lado = 150.0 / MM_POR_PIE
                esquinas = System.Array.CreateInstance(T_Point3d, 4)
                for i, (su, sv) in enumerate(((-1, -1), (1, -1), (1, 1), (-1, 1))):
                    esquinas[i] = punto_as(centro + u.Multiply(su * lado) + v.Multiply(sv * lado))
                notch = ctor_notch.Invoke(System.Array[System.Object]([viga, valor_extremo, esquinas, vector_as(normal), vector_as(u)]))
                write = T_Notch.GetMethod("WriteToDb")
                if write is not None:
                    write.Invoke(notch, None)
                resultados["BeamMultiContourNotch"] = "creado y escrito (rectangulo de 300 sobre el plano de la cara, normal {0}) | despues: {1}".format(xyz_mm(normal), medir_viga(viga, props_viga))
            except Exception as error:
                resultados["BeamMultiContourNotch"] = "FALLO " + error_texto(error)
            print("   BeamMultiContourNotch: " + resultados["BeamMultiContourNotch"])
        else:
            resultados["BeamMultiContourNotch"] = "no intentado (sin constructor o sin geometria)"
    ids_durante = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
    paso(4, "Elementos nuevos con la transaccion abierta: {0}; extension del extremo ahora: {1} mm".format(len(ids_durante - ids_antes), extension_mm(diagonal, indice)))
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
                paso(5, "Commit() hecho (existe {0}): mira la diagonal, captura y deshaz con Ctrl+Z (o cierra sin guardar).".format(MARCA_COMMIT))
    except Exception as error:
        paso(5, "cerrar la transaccion ERROR " + str(error)[:200])
    try:
        if tx is not None and tipo_tx.GetMethod("Dispose") is not None:
            tipo_tx.GetMethod("Dispose").Invoke(tx, None)
    except Exception:
        pass
print("   doc.IsModifiable tras la operacion: {0} | extension del extremo: {1} mm".format(doc.IsModifiable, extension_mm(diagonal, indice)))
ids_despues = set(i.Value for i in DB.FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds())
paso(6, "Elementos nuevos tras cerrar: {0} ({1})".format(len(ids_despues - ids_antes), "se esperan 0" if CANCELAR_AL_FINAL else "con Commit puede haber alguno"))
paso(7, "RESUMEN: " + " | ".join("{0}: {1}".format(k, v) for k, v in sorted(resultados.items())))
print("=== fin 25-corte-advance-steel ===")
