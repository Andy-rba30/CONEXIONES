# -*- coding: utf-8 -*-
from __future__ import print_function
import os
import clr
import System
from System.Collections.Generic import List

# Working point: [-11867.7, -17195.8, 17423.0] mm
wp_x = -11867.7 / 304.8
wp_y = -17195.8 / 304.8
wp_z = 17423.0 / 304.8

r = 3.5

bbox = DB.BoundingBoxXYZ()
bbox.Min = DB.XYZ(wp_x - r, wp_y - 2.5, wp_z - r)
bbox.Max = DB.XYZ(wp_x + r, wp_y + 2.5, wp_z + r)

collector = DB.FilteredElementCollector(doc).OfClass(DB.ViewFamilyType)
vft3d = None
for vft in collector:
    if vft.ViewFamily == DB.ViewFamily.ThreeDimensional:
        vft3d = vft
        break

t = DB.Transaction(doc, "Crear vista 3D para captura borrado")
t.Start()

vista_captura = None
for v in DB.FilteredElementCollector(doc).OfClass(DB.View3D):
    if v.Name == "Captura_Borrado_Fase3":
        vista_captura = v
        break

if vista_captura is None:
    vista_captura = DB.View3D.CreateIsometric(doc, vft3d.Id)
    vista_captura.Name = "Captura_Borrado_Fase3"

target = DB.XYZ(wp_x, wp_y, wp_z)
eye = DB.XYZ(wp_x + 5.0, wp_y - 8.0, wp_z + 4.0)
forward = (target - eye).Normalize()
right = forward.CrossProduct(DB.XYZ(0, 0, 1)).Normalize()
up = right.CrossProduct(forward).Normalize()
orient = DB.ViewOrientation3D(eye, up, forward)
try:
    vista_captura.SetOrientation(orient)
except Exception as ex:
    print("SetOrientation:", ex)

vista_captura.IsSectionBoxActive = True
vista_captura.SetSectionBox(bbox)
vista_captura.DetailLevel = DB.ViewDetailLevel.Fine
vista_captura.DisplayStyle = DB.DisplayStyle.ShadingWithEdges
t.Commit()

ruta_base = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase3-02-borrado"
opts = DB.ImageExportOptions()
opts.FilePath = ruta_base
opts.ExportRange = DB.ExportRange.SetOfViews
view_ids = List[DB.ElementId]()
view_ids.Add(vista_captura.Id)
opts.SetViewsAndSheets(view_ids)
opts.HLRandWFViewsFileType = DB.ImageFileType.PNG
opts.ShadowViewsFileType = DB.ImageFileType.PNG
opts.ImageResolution = DB.ImageResolution.DPI_150
opts.ZoomType = DB.ZoomFitType.FitToPage
opts.PixelSize = 1920

try:
    doc.ExportImage(opts)
    print("doc.ExportImage OK")
except Exception as ex:
    print("Error exportando imagen:", ex)
