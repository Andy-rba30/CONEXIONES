# -*- coding: utf-8 -*-
# Sondeo 19 (ronda 8c, para la opcion V3 "etiquetas con numero en la vista"): comprueba en Revit 2027 si existen las API de
# controles temporales en el lienzo (Autodesk.Revit.DB.TemporaryGraphicsManager, InCanvasControlData e
# ITemporaryGraphicsHandler; los nombres se comprueban con hasattr/dir, NO se inventan), genera con System.Drawing una
# imagen PNG pequena (un numero sobre fondo verde), la coloca como control en el punto de trabajo del Detalle D en la vista
# activa, exporta una captura, quita el control y comprueba que no queda nada. Sin transaccion: los controles temporales
# no son elementos del modelo. Si algo falla, lo imprime y sigue: la salida decide si la Fase 10 usa etiquetas pinchables
# o se queda con los marcadores DirectShape de hoy. No deja nada en el modelo (si quedara un control, se quita al final).
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import glob
import tempfile
import traceback

MM_POR_PIE = 304.8
PUNTO_MM = (-11867.7, -17195.8, 17423.0)   # punto de trabajo del Detalle D en HANGAR_PRUEBA_sondeo.rvt (N4 de la cercha de la Fase 8)
CARPETA = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas"
NOMBRE_CAPTURA = "fase8c-03-etiqueta"
NUMERO = "4"
VERDE = (46, 160, 67)


def pies(valor_mm):
    return valor_mm / MM_POR_PIE


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


print("=== 19-etiquetas-lienzo ===")
vista = doc.ActiveView
paso(1, "Vista activa: {0} ({1}) | plantilla={2}".format(vista.Name, vista.ViewType, vista.IsTemplate))

# 1) Las API existen? (nombres a confirmar: no se da nada por hecho)
nombres = ["TemporaryGraphicsManager", "InCanvasControlData", "ITemporaryGraphicsHandler"]
existen = {}
for nombre in nombres:
    existen[nombre] = hasattr(DB, nombre)
    paso(2, "DB.{0} existe: {1}".format(nombre, existen[nombre]))
if existen.get("TemporaryGraphicsManager"):
    miembros = [m for m in dir(DB.TemporaryGraphicsManager) if not m.startswith("_")]
    paso(2, "miembros de TemporaryGraphicsManager: " + ", ".join(miembros))
if existen.get("InCanvasControlData"):
    miembros = [m for m in dir(DB.InCanvasControlData) if not m.startswith("_")]
    paso(2, "miembros de InCanvasControlData: " + ", ".join(miembros))
if existen.get("ITemporaryGraphicsHandler"):
    miembros = [m for m in dir(DB.ITemporaryGraphicsHandler) if not m.startswith("_")]
    paso(2, "miembros de ITemporaryGraphicsHandler: " + ", ".join(miembros))
if not (existen.get("TemporaryGraphicsManager") and existen.get("InCanvasControlData")):
    print("PARADA: faltan las API de controles en el lienzo en esta version de Revit. V3 no es posible; se queda el marcador DirectShape.")
    raise SystemExit

# 2) Imagen PNG con el numero sobre fondo verde (System.Drawing viene con .NET en Windows)
ruta_png = None
try:
    clr.AddReference("System.Drawing")
    from System.Drawing import Bitmap, Graphics, Color, SolidBrush, Font, FontStyle, StringFormat, StringAlignment, RectangleF, Pen
    from System.Drawing.Imaging import ImageFormat
    tamano = 48
    bmp = Bitmap(tamano, tamano)
    g = Graphics.FromImage(bmp)
    g.Clear(Color.Transparent)
    g.FillEllipse(SolidBrush(Color.FromArgb(VERDE[0], VERDE[1], VERDE[2])), 1, 1, tamano - 3, tamano - 3)
    g.DrawEllipse(Pen(Color.White, 2.0), 1, 1, tamano - 3, tamano - 3)
    formato = StringFormat()
    formato.Alignment = StringAlignment.Center
    formato.LineAlignment = StringAlignment.Center
    g.DrawString(NUMERO, Font("Segoe UI", 20.0, FontStyle.Bold), SolidBrush(Color.White), RectangleF(0, 0, tamano, tamano), formato)
    g.Dispose()
    ruta_png = os.path.join(tempfile.gettempdir(), "motorconexiones-etiqueta-N{0}.png".format(NUMERO))
    bmp.Save(ruta_png, ImageFormat.Png)
    bmp.Dispose()
    paso(3, "PNG generado: {0} ({1} bytes)".format(ruta_png, os.path.getsize(ruta_png)))
except Exception as error:
    paso(3, "no se pudo generar el PNG con System.Drawing: {0}".format(error))
    print(traceback.format_exc())
    print("PARADA: sin imagen no hay control. Anota el error.")
    raise SystemExit

# 3) Control en el lienzo en el punto de trabajo
indice = None
manager = None
try:
    manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
    paso(4, "TemporaryGraphicsManager obtenido: {0}".format(manager is not None))
    punto = DB.XYZ(pies(PUNTO_MM[0]), pies(PUNTO_MM[1]), pies(PUNTO_MM[2]))
    datos = DB.InCanvasControlData(ruta_png, punto)
    try:
        paso(4, "InCanvasControlData: ImageSize={0} Location=({1:.1f}, {2:.1f}, {3:.1f}) mm".format(
            datos.ImageSize, datos.Location.X * MM_POR_PIE, datos.Location.Y * MM_POR_PIE, datos.Location.Z * MM_POR_PIE))
    except Exception as error:
        paso(4, "InCanvasControlData creado (propiedades no legibles: {0})".format(error))
    indice = manager.AddControl(datos, vista.Id)
    paso(5, "Control anadido en la vista {0}: indice {1}".format(vista.Id.Value, indice))
    try:
        uidoc.RefreshActiveView()
    except Exception:
        pass
except Exception as error:
    paso(5, "no se pudo anadir el control: {0}".format(error))
    print(traceback.format_exc())

# 4) Captura de la vista activa con la etiqueta (si se pudo anadir)
if indice is not None:
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
            paso(6, "Captura: {0} (si la etiqueta no sale en la exportacion, mirala en pantalla y haz una captura a mano)".format(destino))
        else:
            paso(6, "Captura: no se genero ningun PNG")
    except Exception as error:
        paso(6, "Captura: fallo ({0})".format(error))

# 5) Manejador de clic (solo se comprueba si se puede definir e instalar; el clic lo prueba la persona)
if indice is not None and existen.get("ITemporaryGraphicsHandler"):
    try:
        class Manejador(DB.ITemporaryGraphicsHandler):
            def OnClick(self, data):
                print("   clic en el control: {0}".format(data))
        manager.SetTemporaryGraphicsHandler(Manejador())
        paso(7, "ITemporaryGraphicsHandler instalado (SetTemporaryGraphicsHandler): pincha la etiqueta en Revit y mira si Revit la resalta; el clic solo se ve en una sesion con el add-in")
    except Exception as error:
        paso(7, "no se pudo instalar el manejador de clic: {0}".format(error))
else:
    paso(7, "manejador de clic no probado")

# 6) Quitar el control y comprobar que no queda nada
if indice is not None:
    try:
        manager.RemoveControl(indice)
        paso(8, "Control {0} quitado".format(indice))
    except Exception as error:
        paso(8, "no se pudo quitar el control: {0}".format(error))
    try:
        manager.Clear()
        paso(9, "TemporaryGraphicsManager.Clear() llamado: no queda ningun control")
    except Exception as error:
        paso(9, "Clear no disponible o fallo: {0}".format(error))
    try:
        uidoc.RefreshActiveView()
    except Exception:
        pass
if ruta_png and os.path.exists(ruta_png):
    try:
        os.remove(ruta_png)
    except Exception:
        pass
paso(10, "Marcadores DirectShape de plan en el modelo (no los toca este sondeo): {0}".format(
    len([e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == "MotorConexiones.Plan"])))
print("=== fin 19-etiquetas-lienzo ===")
