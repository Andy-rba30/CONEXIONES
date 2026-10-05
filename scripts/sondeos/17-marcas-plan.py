# -*- coding: utf-8 -*-
# Sondeo 17 (Fase 8, corregido en la ronda 8b): comprueba en Revit las API que usan las marcas del plan
# (Revit/Batch/PlanMarks.cs), sin pasar por el add-in: override de graficos por elemento en la vista activa
# (View.SetElementOverrides + OverrideGraphicSettings con color de linea, grosor y patron solido de superficie), marcador
# DirectShape (cubo) con ApplicationId, Name y Comentarios (ya sin Marca: Revit avisaba "duplicate Mark values"), lectura
# del override puesto (View.GetElementOverrides) y limpieza (override vacio + borrar el marcador).
# Ronda 8b: en el PC fallaba con "AttributeError: Name" en barra.Symbol.Name (IronPython no resuelve la propiedad Name
# de FamilySymbol/ElementType); ahora los nombres se leen con nombre_de() y se escriben con poner_nombre().
# Todo dentro de un TransactionGroup que se DESHACE al final: no deja nada en el modelo. Entre medias exporta una imagen
# de la vista activa a docs\fases\capturas\fase8-01-sondeo17.png para ver el color y el cubo.
# Usa la barra 1249510 (cordon del Detalle D) si no hay seleccion; si hay barras seleccionadas, la primera de ellas.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import glob
from System.Collections.Generic import List

MM_POR_PIE = 304.8
ID_BARRA_POR_DEFECTO = 1249510
CARPETA = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas"
NOMBRE_CAPTURA = "fase8-01-sondeo17"
APP_ID_SONDEO = "MotorConexiones.Sondeo17"
APP_ID_PLAN = "MotorConexiones.Plan"


def mm(valor_pies):
    return valor_pies * MM_POR_PIE


def pies(valor_mm):
    return valor_mm / MM_POR_PIE


def nombre_de(elemento):
    """Nombre de un elemento sin tropezar con IronPython (Element.Name oculto en ElementType/FamilySymbol)."""
    try:
        return DB.Element.Name.__get__(elemento)
    except Exception:
        pass
    for bip in (DB.BuiltInParameter.SYMBOL_NAME_PARAM, DB.BuiltInParameter.ALL_MODEL_TYPE_NAME):
        try:
            p = elemento.get_Parameter(bip)
            if p is not None and p.HasValue:
                return p.AsString()
        except Exception:
            pass
    return "?"


def poner_nombre(elemento, texto):
    """Escribe Element.Name (lo que hace PlanMarks con shape.Name); devuelve True si se pudo."""
    try:
        DB.Element.Name.__set__(elemento, texto)
        return True
    except Exception:
        pass
    try:
        elemento.Name = texto
        return True
    except Exception:
        return False


print("=== 17-marcas-plan ===")
vista = doc.ActiveView
print("1) Vista activa: {0} ({1}) | plantilla={2} | admite overrides={3}".format(
    vista.Name, vista.ViewType, vista.IsTemplate, vista.AreGraphicsOverridesAllowed()))

# 0) Marcadores de plan que ya haya en el modelo (de conn_batch_plan): solo se listan, no se tocan.
marcadores = [e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == APP_ID_PLAN]
print("2) Marcadores de plan (ApplicationId {0}) en el modelo: {1}".format(APP_ID_PLAN, len(marcadores)))
for m in marcadores[:12]:
    comentario = ""
    try:
        comentario = m.get_Parameter(DB.BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() or ""
    except Exception:
        pass
    print("   [{0}] {1} | {2} | {3}".format(m.Id.Value, nombre_de(m), m.ApplicationDataId, comentario[:140]))

# 1) Barra de prueba
barra = None
try:
    for eid in uidoc.Selection.GetElementIds():
        el = doc.GetElement(eid)
        if isinstance(el, DB.FamilyInstance) and isinstance(el.Location, DB.LocationCurve):
            barra = el
            break
except Exception:
    pass
if barra is None:
    barra = doc.GetElement(DB.ElementId(System.Int64(ID_BARRA_POR_DEFECTO)))
if barra is None:
    print("PARADA: no hay barra seleccionada ni existe {0}.".format(ID_BARRA_POR_DEFECTO))
    raise SystemExit
curva = barra.Location.Curve
p0 = curva.GetEndPoint(0)
p1 = curva.GetEndPoint(1)
centro = DB.XYZ((p0.X + p1.X) / 2.0, (p0.Y + p1.Y) / 2.0, (p0.Z + p1.Z) / 2.0)
print("3) Barra de prueba: [{0}] {1} | centro mm ({2:.1f}, {3:.1f}, {4:.1f})".format(
    barra.Id.Value, nombre_de(barra.Symbol), mm(centro.X), mm(centro.Y), mm(centro.Z)))

# 2) Patron solido
patron_solido = DB.ElementId.InvalidElementId
for fp in DB.FilteredElementCollector(doc).OfClass(DB.FillPatternElement):
    try:
        if fp.GetFillPattern().IsSolidFill:
            patron_solido = fp.Id
            print("4) Patron solido: [{0}] {1}".format(fp.Id.Value, fp.Name))
            break
    except Exception:
        pass
if patron_solido == DB.ElementId.InvalidElementId:
    print("4) Patron solido: NO ENCONTRADO (solo se colorearan las lineas)")

grupo = DB.TransactionGroup(doc, "Sondeo 17: marcas del plan")
grupo.Start()
try:
    t = DB.Transaction(doc, "Sondeo 17: override y marcador")
    t.Start()
    color = DB.Color(230, 25, 75)
    ogs = DB.OverrideGraphicSettings()
    ogs.SetProjectionLineColor(color)
    ogs.SetProjectionLineWeight(10)
    ogs.SetCutLineColor(color)
    ogs.SetCutLineWeight(10)
    if patron_solido != DB.ElementId.InvalidElementId:
        ogs.SetSurfaceForegroundPatternVisible(True)
        ogs.SetSurfaceForegroundPatternId(patron_solido)
        ogs.SetSurfaceForegroundPatternColor(color)
        ogs.SetCutForegroundPatternVisible(True)
        ogs.SetCutForegroundPatternId(patron_solido)
        ogs.SetCutForegroundPatternColor(color)
    vista.SetElementOverrides(barra.Id, ogs)
    leido = vista.GetElementOverrides(barra.Id)
    c = leido.ProjectionLineColor
    print("5) Override puesto en [{0}]: color leido ({1}, {2}, {3}) | grosor {4} | patron superficie {5}".format(
        barra.Id.Value, c.Red, c.Green, c.Blue, leido.ProjectionLineWeight, leido.SurfaceForegroundPatternId.Value))

    # Marcador: cubo de 160 mm centrado en el punto medio de la barra, Modelos genericos
    half = pies(80.0)
    esquinas = [centro + DB.XYZ(-half, -half, -half), centro + DB.XYZ(half, -half, -half),
                centro + DB.XYZ(half, half, -half), centro + DB.XYZ(-half, half, -half)]
    lineas = List[DB.Curve]()
    for i in range(4):
        lineas.Add(DB.Line.CreateBound(esquinas[i], esquinas[(i + 1) % 4]))
    lazo = DB.CurveLoop.Create(lineas)
    lazos = List[DB.CurveLoop]()
    lazos.Add(lazo)
    solido = DB.GeometryCreationUtilities.CreateExtrusionGeometry(lazos, DB.XYZ.BasisZ, 2.0 * half)
    categoria = DB.ElementId(DB.BuiltInCategory.OST_GenericModel)
    print("6) DirectShape admite Modelos genericos: {0}".format(DB.DirectShape.IsValidCategoryId(categoria, doc)))
    marcador = DB.DirectShape.CreateElement(doc, categoria)
    marcador.ApplicationId = APP_ID_SONDEO
    marcador.ApplicationDataId = "sondeo17:N1"
    formas = List[DB.GeometryObject]()
    formas.Add(solido)
    marcador.SetShape(formas)
    nombre_puesto = poner_nombre(marcador, "N1")
    try:
        marcador.get_Parameter(DB.BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(
            "N1 · MotorConexiones sondeo 17; view={0}; ids={1}".format(vista.Id.Value, barra.Id.Value))
    except Exception as error:
        print("   aviso: no se pudo escribir Comentarios: {0}".format(error))
    # Ronda 8b: ya no se escribe Marca (ALL_MODEL_MARK). Se cuenta cuantos modelos genericos del documento tienen Marca
    # "N1".."N99" por si el aviso "duplicate Mark values" lo provocaban marcadores de planes anteriores.
    con_marca = 0
    for e in DB.FilteredElementCollector(doc).OfCategory(DB.BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType():
        try:
            p = e.get_Parameter(DB.BuiltInParameter.ALL_MODEL_MARK)
            if p is not None and p.HasValue and (p.AsString() or "").startswith("N") and (p.AsString() or "")[1:].isdigit():
                con_marca += 1
        except Exception:
            pass
    print("6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): {0}".format(con_marca))
    vista.SetElementOverrides(marcador.Id, ogs)
    t.Commit()
    bb = marcador.get_BoundingBox(None)
    comentarios = ""
    try:
        comentarios = marcador.get_Parameter(DB.BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() or ""
    except Exception:
        pass
    print("7) Marcador creado: [{0}] nombre={1} (Name escrito={2}) | Comentarios={3} | caja mm {4:.0f} x {5:.0f} x {6:.0f}".format(
        marcador.Id.Value, nombre_de(marcador), nombre_puesto, comentarios[:60],
        mm(bb.Max.X - bb.Min.X), mm(bb.Max.Y - bb.Min.Y), mm(bb.Max.Z - bb.Min.Z)))

    # Captura de la vista activa con el color y el cubo
    try:
        if not os.path.isdir(CARPETA):
            os.makedirs(CARPETA)
        for viejo in glob.glob(os.path.join(CARPETA, NOMBRE_CAPTURA + "*.png")):
            os.remove(viejo)
        opciones = DB.ImageExportOptions()
        opciones.FilePath = os.path.join(CARPETA, NOMBRE_CAPTURA)
        opciones.ExportRange = DB.ExportRange.VisibleRegionOfCurrentView
        opciones.HLRandWFViewsFileType = DB.ImageFileType.PNG
        opciones.ShadowViewsFileType = DB.ImageFileType.PNG
        opciones.ZoomType = DB.ZoomFitType.FitToPage
        opciones.PixelSize = 1600
        doc.ExportImage(opciones)
        generados = glob.glob(os.path.join(CARPETA, NOMBRE_CAPTURA + "*.png"))
        if generados:
            destino = os.path.join(CARPETA, NOMBRE_CAPTURA + ".png")
            if generados[0] != destino:
                if os.path.exists(destino):
                    os.remove(destino)
                os.rename(generados[0], destino)
            print("8) Captura: {0}".format(destino))
        else:
            print("8) Captura: no se genero ningun PNG")
    except Exception as error:
        print("8) Captura: fallo ({0})".format(error))

    # Limpieza como la hace PlanMarks.Remove: override vacio y borrar el marcador. El Id se guarda ANTES de borrar: en la
    # ronda 8b el paso 9 leia marcador.Id despues del Delete y Revit lanzaba "The referenced object is not valid".
    marcador_id = marcador.Id.Value
    t2 = DB.Transaction(doc, "Sondeo 17: limpiar")
    t2.Start()
    vista.SetElementOverrides(barra.Id, DB.OverrideGraphicSettings())
    doc.Delete(DB.ElementId(System.Int64(marcador_id)))
    t2.Commit()
    limpio = vista.GetElementOverrides(barra.Id)
    print("9) Tras limpiar: color valido={0} | marcador existe={1}".format(
        limpio.ProjectionLineColor.IsValid, doc.GetElement(DB.ElementId(System.Int64(marcador_id))) is not None))
finally:
    if grupo.HasStarted() and not grupo.HasEnded():
        grupo.RollBack()
        print("10) TransactionGroup deshecho: el modelo queda como estaba.")
print("=== fin 17-marcas-plan ===")
