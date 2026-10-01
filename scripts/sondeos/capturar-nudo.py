# -*- coding: utf-8 -*-
# Captura del nudo desde dentro de Revit (sin tocar la pantalla): crea o reutiliza una vista 3D "Captura_MotorConexiones"
# con caja de seccion alrededor del punto de trabajo, sombreado y detalle fino, y la exporta a PNG en
# docs\fases\capturas\<NOMBRE>.png. Lo escribio el instalador en la Fase 3; generalizado aqui.
# El punto de trabajo sale de los miembros SELECCIONADOS (cordon + barras); si no hay seleccion, usa PUNTO_MM.
# Cambia NOMBRE antes de ejecutar (el archivo se sobrescribe). Deja una vista nueva en el documento: no guardes la copia.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\capturar-nudo.py -SinTransaccion
from __future__ import print_function
import os
import glob
from System.Collections.Generic import List

NOMBRE = "fase3-captura"
CARPETA = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas"
PUNTO_MM = (-11867.7, -17195.8, 17423.0)   # nudo del Detalle D en HANGAR_PRUEBA_sondeo.rvt
RADIO_MM = 1100.0
MM_POR_PIE = 304.8

# 1) Punto de trabajo: media de los extremos mas cercanos entre si de los miembros seleccionados, o PUNTO_MM
punto = PUNTO_MM
try:
    puntos = []
    for eid in uidoc.Selection.GetElementIds():
        el = doc.GetElement(eid)
        if isinstance(el, DB.FamilyInstance) and isinstance(el.Location, DB.LocationCurve):
            c = el.Location.Curve
            puntos.append((c.GetEndPoint(0), c.GetEndPoint(1)))
    if len(puntos) >= 2:
        centro = DB.XYZ(PUNTO_MM[0] / MM_POR_PIE, PUNTO_MM[1] / MM_POR_PIE, PUNTO_MM[2] / MM_POR_PIE)
        extremos = [min(par, key=lambda p: p.DistanceTo(centro)) for par in puntos]
        sx = sum(p.X for p in extremos) / len(extremos)
        sy = sum(p.Y for p in extremos) / len(extremos)
        sz = sum(p.Z for p in extremos) / len(extremos)
        punto = (sx * MM_POR_PIE, sy * MM_POR_PIE, sz * MM_POR_PIE)
        print("punto de trabajo desde la seleccion: ({0:.1f}, {1:.1f}, {2:.1f}) mm".format(*punto))
except Exception as error:
    print("seleccion no usable: " + str(error)[:120])

wp = DB.XYZ(punto[0] / MM_POR_PIE, punto[1] / MM_POR_PIE, punto[2] / MM_POR_PIE)
r = RADIO_MM / MM_POR_PIE
caja = DB.BoundingBoxXYZ()
caja.Min = DB.XYZ(wp.X - r, wp.Y - r * 0.7, wp.Z - r)
caja.Max = DB.XYZ(wp.X + r, wp.Y + r * 0.7, wp.Z + r)

# 2) Vista 3D de captura
tipo_3d = None
for vft in DB.FilteredElementCollector(doc).OfClass(DB.ViewFamilyType):
    if vft.ViewFamily == DB.ViewFamily.ThreeDimensional:
        tipo_3d = vft
        break
t = DB.Transaction(doc, "Vista de captura MotorConexiones")
t.Start()
vista = None
for v in DB.FilteredElementCollector(doc).OfClass(DB.View3D):
    if v.Name == "Captura_MotorConexiones" and not v.IsTemplate:
        vista = v
        break
if vista is None:
    vista = DB.View3D.CreateIsometric(doc, tipo_3d.Id)
    vista.Name = "Captura_MotorConexiones"
ojo = DB.XYZ(wp.X + 5.0, wp.Y - 8.0, wp.Z + 4.0)
adelante = (wp - ojo).Normalize()
derecha = adelante.CrossProduct(DB.XYZ(0, 0, 1)).Normalize()
arriba = derecha.CrossProduct(adelante).Normalize()
try:
    vista.SetOrientation(DB.ViewOrientation3D(ojo, arriba, adelante))
except Exception as error:
    print("SetOrientation: " + str(error)[:120])
vista.IsSectionBoxActive = True
vista.SetSectionBox(caja)
vista.DetailLevel = DB.ViewDetailLevel.Fine
vista.DisplayStyle = DB.DisplayStyle.ShadingWithEdges
t.Commit()

# 3) Exportar
if not os.path.isdir(CARPETA):
    os.makedirs(CARPETA)
base = os.path.join(CARPETA, NOMBRE)
opciones = DB.ImageExportOptions()
opciones.FilePath = base
opciones.ExportRange = DB.ExportRange.SetOfViews
ids = List[DB.ElementId]()
ids.Add(vista.Id)
opciones.SetViewsAndSheets(ids)
opciones.HLRandWFViewsFileType = DB.ImageFileType.PNG
opciones.ShadowViewsFileType = DB.ImageFileType.PNG
opciones.ImageResolution = DB.ImageResolution.DPI_150
opciones.ZoomType = DB.ZoomFitType.FitToPage
opciones.PixelSize = 1920
doc.ExportImage(opciones)
# Revit anade " - 3D View - Captura_MotorConexiones" al nombre: se renombra al nombre pedido
generados = glob.glob(base + "*")
destino = base + ".png"
for g in generados:
    if g != destino and g.lower().endswith(".png"):
        if os.path.isfile(destino):
            os.remove(destino)
        os.rename(g, destino)
    elif not g.lower().endswith(".png"):
        os.remove(g)
print("captura: {0} ({1})".format(destino, "OK" if os.path.isfile(destino) else "NO SE GENERO"))
