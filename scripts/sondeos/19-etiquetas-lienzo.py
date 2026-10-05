# -*- coding: utf-8 -*-
# Sondeo 19, segunda version (cierre de la ronda 8c), para la opcion V3 "etiquetas con numero en la vista" de la Fase 10.
# Lo que dijo el PC en la ronda 8c (docs/fases/resultados-fase-8c.md, 8c-6): existen DB.TemporaryGraphicsManager
# (AddControl, Clear, GetAll, RemoveControl, SetTooltip, SetVisibility, UpdateControl) y DB.InCanvasControlData (ImagePath,
# Position); AddControl rechazo el PNG ("only *.bmp files are supported"); y DB.ITemporaryGraphicsHandler NO existe con ese
# nombre. Esta version corrige las dos cosas:
#   1) la imagen es BMP (System.Drawing la guarda con ImageFormat.Bmp; BMP no tiene transparencia, asi que el fondo va blanco);
#   2) el manejador de clics NO se da por supuesto: se buscan por reflexion, en RevitAPI.dll y RevitAPIUI.dll, todos los tipos
#      publicos cuyo nombre contenga "TemporaryGraphics" o "InCanvasControl", se imprimen sus miembros, y se miran los servicios
#      externos (BuiltInExternalServices) con "Temporary" en el nombre. Si aparece una interfaz ITemporaryGraphicsHandler (lo
#      esperable: en Autodesk.Revit.UI, registrada como servidor del servicio TemporaryGraphicsHandlerService), se registra un
#      servidor de prueba que, al pinchar la etiqueta, abre un cuadro y escribe una linea en
#      %LOCALAPPDATA%\MotorConexiones\log\sondeo19-clics.txt.
# Este sondeo DEJA la etiqueta puesta y el manejador activo para que la persona la pinche en Revit; despues se ejecuta
# scripts/sondeos/19b-etiquetas-quitar.py, que quita el control, desactiva el servidor y ensena los clics anotados. Los
# controles temporales no son elementos del modelo: no hay transaccion y no queda nada en el .rvt.
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
NOMBRE_CAPTURA = "fase8d-01-etiqueta"
NUMERO = "4"
VERDE = (46, 160, 67)
CARPETA_LOG = os.path.join(os.environ.get("LOCALAPPDATA", tempfile.gettempdir()), "MotorConexiones", "log")
ARCHIVO_CLICS = os.path.join(CARPETA_LOG, "sondeo19-clics.txt")
ARCHIVO_ESTADO = os.path.join(CARPETA_LOG, "sondeo19-estado.txt")   # indice del control y vista, para 19b


def pies(valor_mm):
    return valor_mm / MM_POR_PIE


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def miembros_de(tipo):
    try:
        nombres = set()
        for m in tipo.GetMembers():
            if m.DeclaringType is not None and m.DeclaringType.FullName == "System.Object":
                continue
            nombres.add(m.Name)
        return ", ".join(sorted(n for n in nombres if not n.startswith(".")))
    except Exception as error:
        return "(no legibles: {0})".format(error)


def tipos_con(ensamblado, patrones):
    try:
        tipos = list(ensamblado.GetTypes())
    except Exception as error:
        tipos = []
        try:
            tipos = [t for t in error.Types if t is not None]   # ReflectionTypeLoadException
        except Exception:
            pass
    encontrados = []
    for t in tipos:
        try:
            nombre = t.Name.lower()
            if t.IsPublic and any(p.lower() in nombre for p in patrones):
                encontrados.append(t)
        except Exception:
            pass
    return encontrados


def guardar_estado(indice, vista_id):
    try:
        if not os.path.isdir(CARPETA_LOG):
            os.makedirs(CARPETA_LOG)
        with open(ARCHIVO_ESTADO, "w") as f:
            f.write("{0};{1}\n".format(indice, vista_id))
    except Exception as error:
        print("   (no se pudo guardar el estado para 19b: {0})".format(error))


print("=== 19-etiquetas-lienzo (v2: BMP y busqueda del manejador) ===")
vista = doc.ActiveView
paso(1, "Vista activa: {0} ({1}) | plantilla={2} | id={3}".format(vista.Name, vista.ViewType, vista.IsTemplate, vista.Id.Value))

# 1) Las API que ya se vieron en la 8c
existen = {}
for nombre in ("TemporaryGraphicsManager", "InCanvasControlData"):
    existen[nombre] = hasattr(DB, nombre)
    paso(2, "DB.{0} existe: {1}".format(nombre, existen[nombre]))
if not (existen.get("TemporaryGraphicsManager") and existen.get("InCanvasControlData")):
    print("PARADA: faltan las API de controles en el lienzo en esta version de Revit. V3 no es posible; se queda el marcador DirectShape.")
    raise SystemExit

# 2) Busqueda por reflexion del manejador de clics (sin inventar nombres): tipos y servicios externos
tipos_encontrados = []
try:
    asm_db = clr.GetClrType(DB.Document).Assembly
    asm_ui = clr.GetClrType(UI.UIDocument).Assembly
    for asm in (asm_db, asm_ui):
        for t in tipos_con(asm, ("TemporaryGraphics", "InCanvasControl")):
            tipos_encontrados.append(t)
            paso(3, "tipo {0} ({1}{2}): {3}".format(t.FullName, "interfaz" if t.IsInterface else "clase",
                                                     ", enum" if t.IsEnum else "", miembros_de(t)))
    if not tipos_encontrados:
        paso(3, "ningun tipo con TemporaryGraphics o InCanvasControl en RevitAPI.dll ni RevitAPIUI.dll")
except Exception as error:
    paso(3, "la busqueda por reflexion fallo: {0}".format(error))
    print(traceback.format_exc())

servicio_id = None
servicio = None
try:
    builtin = DB.ExternalService.ExternalServices.BuiltInExternalServices
    tipo_builtin = clr.GetClrType(builtin)
    nombres_servicios = sorted(p.Name for p in tipo_builtin.GetProperties())
    con_temporary = [n for n in nombres_servicios if "temporary" in n.lower() or "canvas" in n.lower()]
    paso(4, "servicios externos integrados: {0} en total; con Temporary/Canvas en el nombre: {1}".format(
        len(nombres_servicios), ", ".join(con_temporary) if con_temporary else "ninguno"))
    for nombre in con_temporary:
        try:
            sid = getattr(builtin, nombre)
            srv = DB.ExternalService.ExternalServiceRegistry.GetService(sid)
            paso(4, "servicio {0}: {1} ({2}) | servidores registrados: {3}".format(
                nombre, srv.Name, srv.GetType().Name, len(list(srv.GetRegisteredServerIds()))))
            if servicio is None:
                servicio_id = sid
                servicio = srv
        except Exception as error:
            paso(4, "servicio {0}: no se pudo leer ({1})".format(nombre, error))
except Exception as error:
    paso(4, "no se pudieron leer los servicios externos: {0}".format(error))

# 3) Imagen BMP con el numero sobre fondo verde (System.Drawing viene con .NET en Windows). BMP no tiene transparencia.
ruta_bmp = None
try:
    clr.AddReference("System.Drawing")
    from System.Drawing import Bitmap, Graphics, Color, SolidBrush, Font, FontStyle, StringFormat, StringAlignment, RectangleF, Pen
    from System.Drawing.Imaging import ImageFormat
    tamano = 48
    bmp = Bitmap(tamano, tamano)
    g = Graphics.FromImage(bmp)
    g.Clear(Color.White)
    g.FillEllipse(SolidBrush(Color.FromArgb(VERDE[0], VERDE[1], VERDE[2])), 1, 1, tamano - 3, tamano - 3)
    g.DrawEllipse(Pen(Color.White, 2.0), 1, 1, tamano - 3, tamano - 3)
    formato = StringFormat()
    formato.Alignment = StringAlignment.Center
    formato.LineAlignment = StringAlignment.Center
    g.DrawString(NUMERO, Font("Segoe UI", 20.0, FontStyle.Bold), SolidBrush(Color.White), RectangleF(0, 0, tamano, tamano), formato)
    g.Dispose()
    ruta_bmp = os.path.join(tempfile.gettempdir(), "motorconexiones-etiqueta-N{0}.bmp".format(NUMERO))
    bmp.Save(ruta_bmp, ImageFormat.Bmp)
    bmp.Dispose()
    paso(5, "BMP generado: {0} ({1} bytes)".format(ruta_bmp, os.path.getsize(ruta_bmp)))
except Exception as error:
    paso(5, "no se pudo generar el BMP con System.Drawing: {0}".format(error))
    print(traceback.format_exc())
    print("PARADA: sin imagen no hay control. Anota el error.")
    raise SystemExit

# 4) Control en el lienzo en el punto de trabajo (sin transaccion: no es un elemento del modelo)
indice = None
manager = None
try:
    manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
    paso(6, "TemporaryGraphicsManager obtenido: {0}".format(manager is not None))
    punto = DB.XYZ(pies(PUNTO_MM[0]), pies(PUNTO_MM[1]), pies(PUNTO_MM[2]))
    datos = DB.InCanvasControlData(ruta_bmp, punto)
    try:
        paso(6, "InCanvasControlData: ImagePath={0} | Position=({1:.1f}, {2:.1f}, {3:.1f}) mm".format(
            datos.ImagePath, datos.Position.X * MM_POR_PIE, datos.Position.Y * MM_POR_PIE, datos.Position.Z * MM_POR_PIE))
    except Exception as error:
        paso(6, "InCanvasControlData creado (propiedades no legibles: {0})".format(error))
    indice = manager.AddControl(datos, vista.Id)
    paso(7, "Control anadido en la vista {0}: indice {1}".format(vista.Id.Value, indice))
    try:
        manager.SetTooltip(indice, "MotorConexiones: nudo N{0} (sondeo 19)".format(NUMERO))
        paso(7, "SetTooltip puesto")
    except Exception as error:
        paso(7, "SetTooltip no disponible o fallo: {0}".format(error))
    try:
        uidoc.RefreshActiveView()
    except Exception:
        pass
    guardar_estado(indice, vista.Id.Value)
except Exception as error:
    paso(7, "no se pudo anadir el control: {0}".format(error))
    print(traceback.format_exc())

# 5) Captura de la vista activa con la etiqueta (si se pudo anadir)
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
            paso(8, "Captura: {0} (si la etiqueta no sale en la exportacion, mirala en pantalla y haz una captura a mano)".format(destino))
        else:
            paso(8, "Captura: no se genero ningun PNG")
    except Exception as error:
        paso(8, "Captura: fallo ({0})".format(error))

# 6) Manejador de clic: solo si la reflexion encontro una interfaz *TemporaryGraphicsHandler y un servicio donde registrarla
interfaz = None
for t in tipos_encontrados:
    if t.IsInterface and t.Name.lower().endswith("temporarygraphicshandler"):
        interfaz = t
        break
if indice is None:
    paso(9, "manejador de clic no probado (no hay control)")
elif interfaz is None:
    paso(9, "manejador de clic no probado: ninguna interfaz *TemporaryGraphicsHandler en RevitAPI.dll ni RevitAPIUI.dll (mira el paso 3)")
elif servicio is None:
    paso(9, "manejador de clic no probado: hay interfaz {0} pero ningun servicio externo con Temporary en el nombre donde registrarla (paso 4)".format(interfaz.FullName))
else:
    try:
        # La interfaz se obtiene por su nombre real desde el espacio de nombres donde vive (UI o DB), nunca a mano.
        espacio = UI if interfaz.Namespace == "Autodesk.Revit.UI" else DB
        Interfaz = getattr(espacio, interfaz.Name)

        def anota_clic(data):
            lineas = []
            try:
                lineas.append("clic en la etiqueta: tipo de datos {0}".format(data.GetType().FullName))
                for p in data.GetType().GetProperties():
                    try:
                        valor = p.GetValue(data, None)
                        lineas.append("   {0} = {1}".format(p.Name, valor))
                    except Exception as error_prop:
                        lineas.append("   {0} = (no legible: {1})".format(p.Name, error_prop))
            except Exception as error_datos:
                lineas.append("clic en la etiqueta (datos no legibles: {0})".format(error_datos))
            try:
                if not os.path.isdir(CARPETA_LOG):
                    os.makedirs(CARPETA_LOG)
                with open(ARCHIVO_CLICS, "a") as f:
                    f.write("\n".join(lineas) + "\n")
            except Exception:
                pass
            try:
                UI.TaskDialog.Show("MotorConexiones - sondeo 19", "Clic recibido en la etiqueta N{0}.\n\n{1}".format(NUMERO, "\n".join(lineas)))
            except Exception:
                pass

        class ManejadorSondeo19(Interfaz):
            def __init__(self):
                self._id = System.Guid("7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60")

            def GetServerId(self):
                return self._id

            def GetServiceId(self):
                return servicio_id

            def GetName(self):
                return "MotorConexiones sondeo 19"

            def GetVendorId(self):
                return "ARBA"

            def GetDescription(self):
                return "Manejador de prueba de clics en etiquetas del lienzo (sondeo 19). Se desactiva con 19b."

            def OnClick(self, data):
                anota_clic(data)

        manejador = ManejadorSondeo19()
        ya = [str(g) for g in servicio.GetRegisteredServerIds()]
        if str(manejador.GetServerId()) not in ya:
            servicio.AddServer(manejador)
        guid = manejador.GetServerId()
        if hasattr(servicio, "SetActiveServers"):
            from System.Collections.Generic import List
            activos = List[System.Guid]()
            activos.Add(guid)
            servicio.SetActiveServers(activos)
        elif hasattr(servicio, "SetActiveServer"):
            servicio.SetActiveServer(guid)
        paso(9, "manejador registrado como servidor {0} del servicio {1} (interfaz {2}): pincha la etiqueta en Revit; debe salir un cuadro y una linea en {3}".format(
            guid, servicio.Name, interfaz.FullName, ARCHIVO_CLICS))
    except Exception as error:
        paso(9, "no se pudo registrar el manejador de clic: {0}".format(error))
        print(traceback.format_exc())

paso(10, "La etiqueta se queda puesta (indice {0}, vista {1}) para que la persona la pinche; despues ejecuta 19b-etiquetas-quitar.py".format(indice, vista.Id.Value))
paso(11, "Marcadores DirectShape de plan en el modelo (no los toca este sondeo): {0}".format(
    len([e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == "MotorConexiones.Plan"])))
print("=== fin 19-etiquetas-lienzo ===")
