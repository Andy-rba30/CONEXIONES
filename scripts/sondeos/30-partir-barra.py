# -*- coding: utf-8 -*-
# Sondeo 30 (Fase 13, partir la diagonal): COMO SE PARTE UNA BARRA DE ARMAZON EN DOS ELEMENTOS DE REVIT y como se deshace.
# Por que: la Fase 13 parte la diagonal del nudo en K a la distancia del Detalle B (330 desde la cara del cordon por el eje de la barra
# + espesor de la plancha) para crear el empalme de placa de extremo sobre los dos tramos. El prompt (docs/prompts/fase-13.md, 3.3)
# deja el mecanismo de Revit a este sondeo: (a) ElementTransformUtils.CopyElement + LocationCurve.Curve en los dos elementos, o
# (b) Document.Create.NewFamilyInstance(curva, simbolo, nivel, StructuralType.Beam) copiando a mano los parametros de instancia.
# El add-in 0.13.0 ya lleva (a) con (b) de reserva (Fabrication/MemberSplitter.cs); este sondeo dice cual conserva los parametros y
# si el deshacer deja la barra como estaba. Sobre la copia del modelo, con la diagonal 1249631 y el cordon 1245533 (nudo en K del
# paso 11-5), o con 2 barras seleccionadas (la mas horizontal es el cordon):
#   1) lista los miembros de la API que se van a usar (ElementTransformUtils.CopyElement, StructuralFramingUtils.*JoinAtEnd,
#      Document.Create.NewFamilyInstance con Curve) y los parametros de instancia de la barra ANTES (curva, extensiones,
#      STRUCTURAL_BEND_DIR_ANGLE, justificaciones, desfases, nivel, tipo);
#   2) calcula el punto de corte como SplitPoint.Compute del Core: cara = (ancho del cordon / 2) / |sen theta| desde el corte de los
#      ejes, + 330, + 9,525 (centro de la pila de dos planchas); imprime theta, cara, primera plancha, corte y el punto en mm;
#   3) (a) CopyElement en el sitio, LocationCurve.Curve del original = tramo corto (del lado del cordon, conserva el id) y de la
#      copia = tramo largo; compara TODOS los parametros de instancia de la copia con los del original (lista los que difieren);
#      extensiones 0 en el corte; DisallowJoinAtEnd en los dos extremos del corte; mide los dos tramos;
#   4) si (a) no conserva el giro de seccion o la justificacion, (b) NewFamilyInstance con el tramo largo y copia de parametros
#      (STRUCTURAL_BEND_DIR_ANGLE, Y_JUSTIFICATION, Z_JUSTIFICATION, Y_OFFSET_VALUE, Z_OFFSET_VALUE, START/END_EXTENSION) y
#      vuelve a comparar;
#   5) node_info por el puente (revit_mcp.conexiones.llamar_bridge) de los dos tramos (se espera kind splice, colineal) y del nudo en K
#      (se espera la misma excentricidad que antes, con el tramo corto); si el puente no esta, lo dice y deja las ordenes de
#      conn-call.ps1 para despues;
#   6) deshace: borra el nuevo, devuelve la curva y las extensiones al original y compara con el paso 1 (longitud, extensiones);
#   7) elementos nuevos con la transaccion abierta y tras cerrarla (se esperan 0 con RollBack).
# Por defecto CANCELA al final (RollBack): nada queda en el modelo. Para VER los dos tramos, crea antes %TEMP%\sondeo30-commit.txt
# (PowerShell: New-Item $env:TEMP\sondeo30-commit.txt) y repite: hace Commit del paso 3 (sin deshacer); despues captura y deshaz con
# Ctrl+Z (o cierra la copia sin guardar; el sondeo 13 limpia restos).
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\30-partir-barra.py -SinTransaccion -TimeoutSec 600
# v2 (cierre de la Fase 13, 2026-10-31): en la instalacion de la Fase 13 el sondeo paro en barra() con "Multiple targets could
#   match: ElementId(BuiltInParameter), ElementId(BuiltInCategory), ElementId(Int64)" (IronPython no sabe cual de los tres
#   constructores usar con un entero; lo mismo que el sondeo 19 v3 -> v4): ahora DB.ElementId(System.Int64(...)). Ademas:
#   (1) mide el DESVIO DE EJE de la diagonal original (cuanto cambia Y entre sus extremos, el angulo en planta respecto a X y el
#   angulo respecto a un plano vertical), porque el tramo nuevo del add-in 0.13.0 salio con la advertencia de Revit "Beam or Brace
#   is slightly off axis": si la barra original ya tiene ese desvio, la advertencia es de la barra, no del corte; (2) proyecta el
#   punto de corte sobre la linea real de la diagonal antes de partir (como hace el add-in 0.13.1), e imprime a cuanto estaba.
#   El add-in 0.13.0 ya partio las dos diagonales en Revit con copy_element y sin parametros aproximados (resultados-fase-13.md,
#   pasos 13-7 y 13-8): este sondeo ya no decide el mecanismo; sirve para ver el desvio y los parametros con detalle.
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import math
import os
import traceback

ID_DIAGONAL = 1249631
ID_CORDON = 1245533
MM_POR_PIE = 304.8
OFFSET_MM = 330.0
ESPESOR_MM = 9.525
MARCA_COMMIT = os.path.join(os.environ.get("TEMP", os.environ.get("TMP", ".")), "sondeo30-commit.txt")
CANCELAR_AL_FINAL = not os.path.exists(MARCA_COMMIT)
PARAMETROS = ["STRUCTURAL_BEND_DIR_ANGLE", "Y_JUSTIFICATION", "Z_JUSTIFICATION", "Y_OFFSET_VALUE", "Z_OFFSET_VALUE",
              "START_EXTENSION", "END_EXTENSION", "STRUCTURAL_MATERIAL_PARAM", "INSTANCE_REFERENCE_LEVEL_PARAM", "ELEM_TYPE_PARAM"]


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def mm(v):
    return v * MM_POR_PIE


def xyz_mm(p):
    return "({0:.1f}, {1:.1f}, {2:.1f})".format(mm(p.X), mm(p.Y), mm(p.Z))


def error_texto(error):
    interna = getattr(error, "InnerException", None)
    return "{0}: {1}{2}".format(type(error).__name__, str(error), (" | interna: " + str(interna)) if interna else "")


def barra(eid):
    # IronPython: DB.ElementId(1249631) es ambiguo (tres constructores); System.Int64 elige ElementId(Int64), como en los demas sondeos.
    e = doc.GetElement(DB.ElementId(System.Int64(int(eid))))
    return e if isinstance(e, DB.FamilyInstance) else None


def desvio_de_eje(nombre, e):
    """Cuanto se sale la barra de un plano vertical y de la direccion X en planta (lo que Revit llama 'slightly off axis')."""
    p0, p1 = extremos(e)
    dx, dy, dz = mm(p1.X - p0.X), mm(p1.Y - p0.Y), mm(p1.Z - p0.Z)
    planta = math.hypot(dx, dy)
    angulo_planta = math.degrees(math.atan2(abs(dy), abs(dx))) if planta > 1e-9 else 0.0
    angulo_vertical = math.degrees(math.atan2(abs(dy), math.hypot(dx, dz))) if (dx or dz) else 0.0
    print("   {0}: dX {1:.6f} | dY {2:.6f} | dZ {3:.6f} mm | en planta {4:.6f} grados respecto a X | {5:.6f} grados fuera del plano vertical XZ".format(
        nombre, dx, dy, dz, angulo_planta, angulo_vertical))
    print("   {0}: inicio ({1:.6f}, {2:.6f}, {3:.6f}) fin ({4:.6f}, {5:.6f}, {6:.6f}) mm".format(nombre, mm(p0.X), mm(p0.Y), mm(p0.Z), mm(p1.X), mm(p1.Y), mm(p1.Z)))


def curva(e):
    return e.Location.Curve


def extremos(e):
    c = curva(e)
    return c.GetEndPoint(0), c.GetEndPoint(1)


def parametro(e, nombre):
    try:
        p = e.get_Parameter(getattr(DB.BuiltInParameter, nombre))
    except Exception:
        return None
    return p


def valor(p):
    if p is None:
        return "(no existe)"
    try:
        if p.StorageType == DB.StorageType.Double:
            return "{0:.6f}".format(p.AsDouble())
        if p.StorageType == DB.StorageType.Integer:
            return str(p.AsInteger())
        if p.StorageType == DB.StorageType.ElementId:
            return str(p.AsElementId().Value)
        return p.AsString() or p.AsValueString() or ""
    except Exception as error:
        return "(error: {0})".format(error)


def fotografia(e):
    """Todos los parametros de instancia (nombre -> valor) para comparar original y copia."""
    foto = {}
    for p in e.Parameters:
        try:
            foto[p.Definition.Name] = valor(p)
        except Exception:
            pass
    return foto


def ancho_en_plano_mm(e):
    """El lado del perfil en el plano de la cercha (ronda 12c: b o Ht segun el giro de seccion); 203,2 si no se lee."""
    tipo = doc.GetElement(e.GetTypeId())
    lados = {}
    for nombre in ("b", "Ht", "B", "H"):
        p = tipo.LookupParameter(nombre) if tipo is not None else None
        if p is not None and p.StorageType == DB.StorageType.Double:
            lados[nombre] = mm(p.AsDouble())
    giro = parametro(e, "STRUCTURAL_BEND_DIR_ANGLE")
    grados = math.degrees(giro.AsDouble()) if giro is not None and giro.StorageType == DB.StorageType.Double else 0.0
    b = lados.get("b", lados.get("B"))
    h = lados.get("Ht", lados.get("H"))
    print("   perfil del cordon: b={0} Ht={1} giro={2:.1f} grados".format(b, h, grados))
    if b is None and h is None:
        return 203.2
    if abs(abs(grados) - 90.0) < 1.0:
        return h or b
    return b or h


def punto_corte(diagonal, cordon):
    """SplitPoint.Compute del Core: cara a (ancho/2)/|sen theta| del corte de los ejes, + 330 hasta la primera plancha, + espesor."""
    d0, d1 = extremos(diagonal)
    c0, c1 = extremos(cordon)
    u = (d1 - d0).Normalize()
    v = (c1 - c0).Normalize()
    dot = u.DotProduct(v)
    sin = math.sqrt(max(0.0, 1.0 - dot * dot))
    if sin < math.sin(math.radians(5.0)):
        raise Exception("la barra es paralela al cordon")
    theta = math.degrees(math.asin(min(1.0, sin)))
    w0 = d0 - c0
    d = u.DotProduct(w0)
    e = v.DotProduct(w0)
    t = (dot * e - d) / (1.0 - dot * dot)
    corte_ejes = d0 + u * t
    lejos = d1 if d1.DistanceTo(corte_ejes) > d0.DistanceTo(corte_ejes) else d0
    direccion = (lejos - corte_ejes).Normalize()
    half = ancho_en_plano_mm(cordon) / 2.0
    cara = half / sin
    primera = cara + OFFSET_MM
    corte = primera + ESPESOR_MM
    punto = corte_ejes + direccion * (corte / MM_POR_PIE)
    extremo_cerca = 1 if lejos.IsAlmostEqualTo(d0) else 0   # el extremo del lado del cordon: 0 = inicio, 1 = fin
    print("   theta = {0:.1f} grados | cara a {1:.1f} mm | primera plancha a {2:.1f} mm | corte a {3:.1f} mm del corte de los ejes".format(theta, cara, primera, corte))
    print("   corte de los ejes {0} | punto de corte {1} | extremo del lado del cordon: {2}".format(xyz_mm(corte_ejes), xyz_mm(punto), "fin" if extremo_cerca == 1 else "inicio"))
    return punto, extremo_cerca, lejos


def listar_api():
    for tipo, nombres in ((DB.ElementTransformUtils, ("CopyElement", "CopyElements")),
                          (DB.StructuralFramingUtils if hasattr(DB, "StructuralFramingUtils") else None, ("DisallowJoinAtEnd", "AllowJoinAtEnd", "IsJoinAllowedAtEnd", "CanFlipEnds")),
                          (DB.Structure.StructuralFramingUtils if hasattr(DB, "Structure") and hasattr(DB.Structure, "StructuralFramingUtils") else None, ("DisallowJoinAtEnd", "AllowJoinAtEnd", "IsJoinAllowedAtEnd"))):
        if tipo is None:
            print("   (clase no disponible con ese nombre)")
            continue
        for n in nombres:
            print("   {0}.{1}: {2}".format(tipo.__name__ if hasattr(tipo, "__name__") else tipo, n, "existe" if hasattr(tipo, n) else "NO existe"))
    creador = doc.Create
    firmas = [m for m in dir(creador) if m == "NewFamilyInstance"]
    print("   Document.Create.NewFamilyInstance: {0}".format("existe" if firmas else "NO existe"))


def node_info(ids):
    try:
        import sys
        for ruta in list(sys.path):
            pass
        from revit_mcp.conexiones import llamar_bridge
        import json
        r = llamar_bridge("node_info", {"element_ids": list(ids)}, doc, uidoc)
        datos = r if isinstance(r, dict) else json.loads(str(r))
        d = (datos.get("data") or {})
        print("   node_info {0}: ok={1} kind={2} eccentricity_mm={3} chord={4} barras={5}".format(
            list(ids), datos.get("ok"), d.get("kind"), d.get("eccentricity_mm"), d.get("chord_element_id"),
            [(m.get("element_id"), m.get("length_mm"), m.get("split_distance_mm")) for m in d.get("members") or []]))
        if not datos.get("ok"):
            print("   errores: {0}".format(datos.get("errors")))
    except Exception as error:
        print("   node_info no disponible desde el sondeo ({0}); despues: .\\scripts\\conn-call.ps1 node_info '{{\"element_ids\": {1}}}'".format(error_texto(error), list(ids)))


def nuevos_desde(antes):
    ahora = set(e.Id.Value for e in DB.FilteredElementCollector(doc).WhereElementIsNotElementType())
    return sorted(ahora - antes)


def main():
    print("Sondeo 30: partir la barra en dos (Fase 13). Documento: {0}. Cancelar al final: {1}".format(doc.Title, CANCELAR_AL_FINAL))
    seleccion = [i.Value for i in uidoc.Selection.GetElementIds()] if uidoc is not None else []
    id_diag, id_cordon = ID_DIAGONAL, ID_CORDON
    if len(seleccion) == 2 and all(barra(i) for i in seleccion):
        a, b = [barra(i) for i in seleccion]
        def pendiente(e):
            p0, p1 = extremos(e)
            return abs(p1.Z - p0.Z) / max(1e-9, p0.DistanceTo(p1))
        id_cordon, id_diag = (a.Id.Value, b.Id.Value) if pendiente(a) <= pendiente(b) else (b.Id.Value, a.Id.Value)
    diagonal, cordon = barra(id_diag), barra(id_cordon)
    if diagonal is None or cordon is None:
        print("No estan las barras {0} (diagonal) y {1} (cordon) en el modelo: selecciona el cordon y la diagonal y repite.".format(id_diag, id_cordon))
        return
    paso(1, "miembros de la API y parametros ANTES de la diagonal {0} (cordon {1})".format(id_diag, id_cordon))
    listar_api()
    d0, d1 = extremos(diagonal)
    largo_antes = mm(d0.DistanceTo(d1))
    foto_antes = fotografia(diagonal)
    print("   curva: inicio {0} fin {1} largo {2:.1f} mm".format(xyz_mm(d0), xyz_mm(d1), largo_antes))
    desvio_de_eje("diagonal {0} ANTES".format(id_diag), diagonal)
    desvio_de_eje("cordon {0}".format(id_cordon), cordon)
    for n in PARAMETROS:
        print("   {0} = {1}".format(n, valor(parametro(diagonal, n))))
    paso(2, "punto de corte (SplitPoint.Compute del Core) y su proyeccion sobre la linea real de la diagonal (add-in 0.13.1)")
    try:
        punto, extremo_cerca, lejos = punto_corte(diagonal, cordon)
    except Exception as error:
        print("   no se pudo calcular: {0}".format(error_texto(error)))
        return
    try:
        proyectado = curva(diagonal).Project(punto).XYZPoint
        print("   el punto del marco estaba a {0:.6f} mm de la linea real de la diagonal; se usa el proyectado {1}".format(mm(punto.DistanceTo(proyectado)), xyz_mm(proyectado)))
        punto = proyectado
    except Exception as error:
        print("   no se pudo proyectar el punto sobre la linea ({0}); se usa el del marco".format(error_texto(error)))
    antes = set(e.Id.Value for e in DB.FilteredElementCollector(doc).WhereElementIsNotElementType())
    tg = DB.TransactionGroup(doc, "Sondeo 30: partir barra")
    tg.Start()
    copia = None
    try:
        paso(3, "(a) CopyElement en el sitio + LocationCurve.Curve en los dos elementos")
        t = DB.Transaction(doc, "Sondeo 30 (a)")
        t.Start()
        try:
            ids = DB.ElementTransformUtils.CopyElement(doc, diagonal.Id, DB.XYZ.Zero)
            copia = doc.GetElement(list(ids)[0])
            print("   CopyElement -> {0} ({1})".format(copia.Id.Value, type(copia).__name__))
            if extremo_cerca == 1:
                corto = DB.Line.CreateBound(punto, d1)
                largo = DB.Line.CreateBound(d0, punto)
            else:
                corto = DB.Line.CreateBound(d0, punto)
                largo = DB.Line.CreateBound(punto, d1)
            diagonal.Location.Curve = corto
            copia.Location.Curve = largo
            doc.Regenerate()
            for e, nombre, extremo_corte in ((diagonal, "original (tramo corto)", 0 if extremo_cerca == 1 else 1), (copia, "copia (tramo largo)", 1 if extremo_cerca == 1 else 0)):
                p0, p1 = extremos(e)
                print("   {0}: {1} -> {2} largo {3:.1f} mm".format(nombre, xyz_mm(p0), xyz_mm(p1), mm(p0.DistanceTo(p1))))
                desvio_de_eje(nombre, e)
                ext = parametro(e, "START_EXTENSION" if extremo_corte == 0 else "END_EXTENSION")
                if ext is not None and not ext.IsReadOnly:
                    ext.Set(0.0)
                    print("   {0}: extension en el corte puesta a 0".format(nombre))
                utils = getattr(DB.Structure, "StructuralFramingUtils", None) if hasattr(DB, "Structure") else None
                if utils is not None and hasattr(utils, "DisallowJoinAtEnd"):
                    try:
                        utils.DisallowJoinAtEnd(e, extremo_corte)
                        print("   {0}: DisallowJoinAtEnd({1}) ok; IsJoinAllowedAtEnd = {2}".format(nombre, extremo_corte, utils.IsJoinAllowedAtEnd(e, extremo_corte)))
                    except Exception as error:
                        print("   {0}: DisallowJoinAtEnd fallo: {1}".format(nombre, error_texto(error)))
            foto_copia = fotografia(copia)
            foto_original = fotografia(diagonal)
            distintos = sorted(k for k in foto_antes if k in foto_copia and foto_copia[k] != foto_antes[k] and k not in ("Length", "Longitud", "Start Extension", "End Extension", "Extensión inicial", "Extensión final", "Volume", "Volumen", "Cut Length", "Longitud de corte"))
            print("   parametros de la copia distintos del original de antes ({0}): {1}".format(len(distintos), ", ".join("{0} [{1} -> {2}]".format(k, foto_antes[k], foto_copia[k]) for k in distintos) or "ninguno"))
            for n in PARAMETROS:
                print("   {0}: original {1} | copia {2}".format(n, valor(parametro(diagonal, n)), valor(parametro(copia, n))))
            t.Commit()
            print("   (a) commit ok")
        except Exception as error:
            print("   (a) FALLO: {0}".format(error_texto(error)))
            t.RollBack()
            copia = None
        if copia is None:
            paso(4, "(b) NewFamilyInstance(curva, simbolo, nivel, Beam) + copia de parametros")
            t = DB.Transaction(doc, "Sondeo 30 (b)")
            t.Start()
            try:
                simbolo = doc.GetElement(diagonal.GetTypeId())
                nivel = doc.GetElement(diagonal.LevelId) if diagonal.LevelId != DB.ElementId.InvalidElementId else None
                if nivel is None:
                    nivel = DB.FilteredElementCollector(doc).OfClass(DB.Level).FirstElement()
                largo = DB.Line.CreateBound(d0, punto) if extremo_cerca == 1 else DB.Line.CreateBound(punto, d1)
                copia = doc.Create.NewFamilyInstance(largo, simbolo, nivel, DB.Structure.StructuralType.Beam)
                print("   NewFamilyInstance -> {0}".format(copia.Id.Value))
                for n in PARAMETROS[:7]:
                    po, pc = parametro(diagonal, n), parametro(copia, n)
                    if po is not None and pc is not None and not pc.IsReadOnly:
                        try:
                            if po.StorageType == DB.StorageType.Double:
                                pc.Set(po.AsDouble())
                            elif po.StorageType == DB.StorageType.Integer:
                                pc.Set(po.AsInteger())
                            print("   {0} copiado: {1}".format(n, valor(pc)))
                        except Exception as error:
                            print("   {0} no se pudo copiar: {1}".format(n, error_texto(error)))
                diagonal.Location.Curve = DB.Line.CreateBound(punto, d1) if extremo_cerca == 1 else DB.Line.CreateBound(d0, punto)
                doc.Regenerate()
                t.Commit()
                print("   (b) commit ok")
            except Exception as error:
                print("   (b) FALLO: {0}".format(error_texto(error)))
                t.RollBack()
                copia = None
        else:
            paso(4, "(b) no hace falta: (a) funciono (si en el paso 3 hay parametros distintos, el add-in los copia a mano: SPLIT_PARAMETERS_APPROXIMATED)")
        paso(5, "node_info de los dos tramos y del nudo en K (por el puente, si esta)")
        if copia is not None:
            node_info([id_diag, copia.Id.Value])
            node_info([id_cordon, id_diag] + [i for i in (1249632,) if barra(i) and i != id_diag])
        paso(6, "deshacer: borrar la copia y devolver la curva al original")
        if copia is not None:
            t = DB.Transaction(doc, "Sondeo 30 deshacer")
            t.Start()
            try:
                doc.Delete(copia.Id)
                diagonal.Location.Curve = DB.Line.CreateBound(d0, d1)
                for n in ("START_EXTENSION", "END_EXTENSION"):
                    p = parametro(diagonal, n)
                    if p is not None and not p.IsReadOnly and n in foto_antes.get("__ext__", {}):
                        pass
                doc.Regenerate()
                p0, p1 = extremos(diagonal)
                print("   original tras deshacer: {0} -> {1} largo {2:.1f} mm (antes {3:.1f}); diferencia {4:.2f} mm".format(xyz_mm(p0), xyz_mm(p1), mm(p0.DistanceTo(p1)), largo_antes, mm(p0.DistanceTo(p1)) - largo_antes))
                for n in PARAMETROS[:7]:
                    print("   {0}: ahora {1} | antes {2}".format(n, valor(parametro(diagonal, n)), foto_antes.get(parametro(diagonal, n).Definition.Name if parametro(diagonal, n) is not None else n, "?")))
                t.Commit()
            except Exception as error:
                print("   deshacer FALLO: {0}".format(error_texto(error)))
                t.RollBack()
        paso(7, "elementos nuevos con el grupo abierto: {0}".format(nuevos_desde(antes)))
    except Exception as error:
        print("   FALLO general: {0}".format(error_texto(error)))
        traceback.print_exc()
    finally:
        if CANCELAR_AL_FINAL:
            tg.RollBack()
            print("   grupo de transacciones REVERTIDO (nada queda en el modelo)")
        else:
            tg.Assimilate()
            print("   grupo de transacciones ASIMILADO (marca {0}): los cambios quedan; deshaz con Ctrl+Z".format(MARCA_COMMIT))
    print("   elementos nuevos tras cerrar: {0}".format(nuevos_desde(antes)))
    print("Fin del sondeo 30.")


main()
