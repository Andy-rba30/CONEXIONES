# -*- coding: utf-8 -*-
# Sondeo 19, tercera version (cierre de la ronda 8d), para la opcion V3 "etiquetas con numero en la vista" de la Fase 10.
# Lo que dijo el PC en la ronda 8d (docs/fases/resultados-fase-8d.md, 8d-6): Revit acepto el BMP, AddControl devolvio el
# indice 0, SetTooltip funciono, existen UI.ITemporaryGraphicsHandler (OnClick) y el servicio TemporaryGraphicsHandlerService,
# y el manejador de clics se registro sin error. PERO la etiqueta no se vio en pantalla, asi que no se pudo pinchar.
# Esta version cambia solo lo que puede explicar que no se dibuje, sin inventar ningun miembro de la API (los nombres
# siguientes se comprobaron contra RevitAPI.dll y RevitAPIUI.dll 2027.2.0 en la nube: TemporaryGraphicsManager.AddControl(
# InCanvasControlData, ElementId), SetVisibility(int, bool), SetTooltip(int, string), UpdateControl, GetAll(), RemoveControl(int),
# Clear(); InCanvasControlData(string, XYZ) con ImagePath y Position; UIDocument.RefreshActiveView() y UpdateAllOpenViews();
# View3D.IsSectionBoxActive y GetSectionBox(); BoundingBoxXYZ.Min, Max y Transform):
#   1) BMP de 24 bits sin canal alfa y de 32x32 (el de la v2 era de 48x48 y 32 bits: 9270 bytes = 48*48*4 + 54). Se comprueba
#      leyendo la cabecera del archivo (bits por pixel en el byte 28) y, si System.Drawing no lo guardo a 24 bits, se escribe
#      a mano.
#   2) La imagen se guarda en una carpeta SIN tildes ni espacios (C:\IA\MotorConexiones-sondeo19). En la 8d la ruta era
#      ...\andy bayona anton\appdata\local\temp\..., y un cargador de imagenes nativo puede fallar en silencio con la tilde.
#   3) Posicion en unidades internas de Revit (pies): el punto de trabajo en mm se divide entre 304.8 y se imprime en pies y
#      en mm para que no quede duda.
#   4) Despues de AddControl: SetVisibility(indice, True), SetTooltip, y refresco explicito de la vista con
#      uidoc.RefreshActiveView() y uidoc.UpdateAllOpenViews(), cada paso con su linea de salida.
#   5) Dos etiquetas en vez de una: la "4" en el nudo N4 (Detalle D) y una "B" en el centro de la caja de seccion de la vista
#      (o del recuadro de recorte, o de la barra 1249510 si no hay ninguna): si la "B" se ve y la "4" no, el problema es el
#      punto (fuera de la caja de seccion o tapado), no el control.
#   6) GetAll() despues de anadir, para contar los controles que Revit dice tener.
# El manejador de clics se registra igual que en la v2 (funciono). Este sondeo DEJA las etiquetas puestas para que la persona
# las pinche; despues se ejecuta scripts/sondeos/19b-etiquetas-quitar.py. Los controles temporales no son elementos del
# modelo: no hay transaccion y no queda nada en el .rvt. La captura exportada con ExportImage seguramente NO ensena los
# controles temporales (son graficos de pantalla): la persona hace la captura a mano.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import glob
import struct
import tempfile
import traceback

MM_POR_PIE = 304.8
PUNTO_MM = (-11867.7, -17195.8, 17423.0)   # punto de trabajo del Detalle D en HANGAR_PRUEBA_sondeo.rvt (N4 de la cercha de la Fase 8)
BARRA_PRUEBA_ID = 1249510                  # cordon del Detalle D (sondeo 17): su centro es el tercer candidato para la etiqueta B
CARPETA = r"D:\Proyectos C#\CONEXIONES\docs\fases\capturas"
NOMBRE_CAPTURA = "fase8e-01-etiqueta"
NUMERO = "4"
TAMANO = 32
VERDE = (46, 160, 67)
AZUL = (31, 78, 154)
CARPETAS_BMP = [r"C:\IA\MotorConexiones-sondeo19", os.path.join(os.environ.get("PUBLIC", r"C:\Users\Public"), "MotorConexiones-sondeo19"), tempfile.gettempdir()]
CARPETA_LOG = os.path.join(os.environ.get("LOCALAPPDATA", tempfile.gettempdir()), "MotorConexiones", "log")
ARCHIVO_CLICS = os.path.join(CARPETA_LOG, "sondeo19-clics.txt")
ARCHIVO_ESTADO = os.path.join(CARPETA_LOG, "sondeo19-estado.txt")   # indices de los controles, vista y rutas BMP, para 19b


def pies(valor_mm):
    return valor_mm / MM_POR_PIE


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def xyz_texto(punto):
    return "pies ({0:.4f}, {1:.4f}, {2:.4f}) = mm ({3:.1f}, {4:.1f}, {5:.1f})".format(
        punto.X, punto.Y, punto.Z, punto.X * MM_POR_PIE, punto.Y * MM_POR_PIE, punto.Z * MM_POR_PIE)


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


def carpeta_bmp():
    """Primera carpeta sin tildes ni espacios en la que se pueda escribir."""
    for carpeta in CARPETAS_BMP:
        try:
            if not os.path.isdir(carpeta):
                os.makedirs(carpeta)
            prueba = os.path.join(carpeta, "escritura.tmp")
            with open(prueba, "w") as f:
                f.write("ok")
            os.remove(prueba)
            return carpeta
        except Exception:
            continue
    return tempfile.gettempdir()


def cabecera_bmp(ruta):
    """Devuelve (ancho, alto, bits por pixel, bytes) leyendo la cabecera del archivo BMP."""
    with open(ruta, "rb") as f:
        datos = f.read(54)
    if len(datos) < 54 or datos[:2] != b"BM":
        raise ValueError("no es un BMP")
    ancho = struct.unpack("<i", datos[18:22])[0]
    alto = struct.unpack("<i", datos[22:26])[0]
    bpp = struct.unpack("<H", datos[28:30])[0]
    return ancho, abs(alto), bpp, os.path.getsize(ruta)


def escribir_bmp_24(ruta, pixeles, ancho, alto):
    """Escribe a mano un BMP de 24 bits (sin alfa). pixeles[y][x] = (r, g, b), fila 0 arriba."""
    relleno = (4 - (ancho * 3) % 4) % 4
    tamano_filas = (ancho * 3 + relleno) * alto
    with open(ruta, "wb") as f:
        f.write(b"BM")
        f.write(struct.pack("<IHHI", 54 + tamano_filas, 0, 0, 54))
        f.write(struct.pack("<iiiHHIIiiII", 40, ancho, alto, 1, 24, 0, tamano_filas, 2835, 2835, 0, 0))
        for y in range(alto - 1, -1, -1):
            fila = bytearray()
            for x in range(ancho):
                r, g, b = pixeles[y][x]
                fila.extend(bytearray([b, g, r]))
            fila.extend(bytearray(relleno))
            f.write(bytes(fila))


def generar_bmp(ruta, texto, color):
    """BMP de 24 bits y 32x32 con un circulo de color y el texto en blanco. Devuelve como se genero."""
    clr.AddReference("System.Drawing")
    from System.Drawing import Bitmap, Graphics, Color, SolidBrush, Font, FontStyle, StringFormat, StringAlignment, RectangleF, Pen
    from System.Drawing.Imaging import ImageFormat, PixelFormat
    bmp = Bitmap(TAMANO, TAMANO, PixelFormat.Format24bppRgb)
    g = Graphics.FromImage(bmp)
    g.Clear(Color.White)
    g.FillEllipse(SolidBrush(Color.FromArgb(color[0], color[1], color[2])), 1, 1, TAMANO - 3, TAMANO - 3)
    g.DrawEllipse(Pen(Color.White, 1.0), 1, 1, TAMANO - 3, TAMANO - 3)
    formato = StringFormat()
    formato.Alignment = StringAlignment.Center
    formato.LineAlignment = StringAlignment.Center
    g.DrawString(texto, Font("Segoe UI", 14.0, FontStyle.Bold), SolidBrush(Color.White), RectangleF(0, 0, TAMANO, TAMANO), formato)
    g.Dispose()
    bmp.Save(ruta, ImageFormat.Bmp)
    ancho, alto, bpp, tamano = cabecera_bmp(ruta)
    if bpp == 24:
        bmp.Dispose()
        return "System.Drawing a 24 bits"
    # System.Drawing lo guardo con otro formato: se reescribe a mano a 24 bits con los mismos pixeles.
    pixeles = []
    for y in range(TAMANO):
        fila = []
        for x in range(TAMANO):
            c = bmp.GetPixel(x, y)
            fila.append((c.R, c.G, c.B))
        pixeles.append(fila)
    bmp.Dispose()
    escribir_bmp_24(ruta, pixeles, TAMANO, TAMANO)
    return "System.Drawing dio {0} bits; reescrito a mano a 24 bits".format(bpp)


def guardar_estado(indices, vista_id, rutas):
    try:
        if not os.path.isdir(CARPETA_LOG):
            os.makedirs(CARPETA_LOG)
        with open(ARCHIVO_ESTADO, "w") as f:
            f.write("{0};{1};{2}\n".format(",".join(str(i) for i in indices), vista_id, "|".join(rutas)))
    except Exception as error:
        print("   (no se pudo guardar el estado para 19b: {0})".format(error))


print("=== 19-etiquetas-lienzo (v3: BMP 24 bits 32x32, ruta sin tildes, SetVisibility, refresco, dos etiquetas) ===")
vista = doc.ActiveView
paso(1, "Vista activa: {0} ({1}) | plantilla={2} | id={3}".format(vista.Name, vista.ViewType, vista.IsTemplate, vista.Id.Value))
es_3d = isinstance(vista, DB.View3D)
caja_seccion = None
if es_3d:
    try:
        activa = vista.IsSectionBoxActive
        paso(1, "Vista 3D | caja de seccion activa={0} | perspectiva={1}".format(activa, vista.IsPerspective))
        if activa:
            caja_seccion = vista.GetSectionBox()
            minimo = caja_seccion.Transform.OfPoint(caja_seccion.Min)
            maximo = caja_seccion.Transform.OfPoint(caja_seccion.Max)
            paso(1, "Caja de seccion (mm): min ({0:.0f}, {1:.0f}, {2:.0f}) max ({3:.0f}, {4:.0f}, {5:.0f})".format(
                minimo.X * MM_POR_PIE, minimo.Y * MM_POR_PIE, minimo.Z * MM_POR_PIE, maximo.X * MM_POR_PIE, maximo.Y * MM_POR_PIE, maximo.Z * MM_POR_PIE))
            dentro = all(min(a, b) - 1 <= p <= max(a, b) + 1 for p, a, b in zip(PUNTO_MM, (minimo.X * MM_POR_PIE, minimo.Y * MM_POR_PIE, minimo.Z * MM_POR_PIE), (maximo.X * MM_POR_PIE, maximo.Y * MM_POR_PIE, maximo.Z * MM_POR_PIE)))
            paso(1, "El punto de trabajo de N4 esta dentro de la caja de seccion: {0}".format(dentro))
    except Exception as error:
        paso(1, "no se pudo leer la caja de seccion: {0}".format(error))
else:
    paso(1, "AVISO: la vista activa no es 3D; el sondeo sigue, pero ponte en la 3D de siempre para comparar con la 8d")

# 2) Las API que ya se vieron en la 8c y la 8d, y sus miembros (SetVisibility tiene que estar)
existen = {}
for nombre in ("TemporaryGraphicsManager", "InCanvasControlData"):
    existen[nombre] = hasattr(DB, nombre)
    paso(2, "DB.{0} existe: {1}".format(nombre, existen[nombre]))
if not (existen.get("TemporaryGraphicsManager") and existen.get("InCanvasControlData")):
    print("PARADA: faltan las API de controles en el lienzo en esta version de Revit. V3 no es posible; se queda el marcador DirectShape.")
    raise SystemExit
try:
    paso(2, "miembros de TemporaryGraphicsManager: {0}".format(miembros_de(clr.GetClrType(DB.TemporaryGraphicsManager))))
except Exception as error:
    paso(2, "miembros no legibles: {0}".format(error))

# 3) Servicio del manejador de clics (en la 8d: TemporaryGraphicsHandlerService, interfaz UI.ITemporaryGraphicsHandler)
servicio_id = None
servicio = None
interfaz = None
try:
    builtin = DB.ExternalService.ExternalServices.BuiltInExternalServices
    tipo_builtin = clr.GetClrType(builtin)
    for nombre in sorted(p.Name for p in tipo_builtin.GetProperties()):
        if "temporary" in nombre.lower() or "canvas" in nombre.lower():
            sid = getattr(builtin, nombre)
            srv = DB.ExternalService.ExternalServiceRegistry.GetService(sid)
            paso(3, "servicio {0}: {1} | servidores registrados: {2}".format(nombre, srv.Name, len(list(srv.GetRegisteredServerIds()))))
            if servicio is None:
                servicio_id = sid
                servicio = srv
    if servicio is None:
        paso(3, "ningun servicio externo con Temporary/Canvas en el nombre")
except Exception as error:
    paso(3, "no se pudieron leer los servicios externos: {0}".format(error))
try:
    interfaz = getattr(UI, "ITemporaryGraphicsHandler", None)
    paso(3, "UI.ITemporaryGraphicsHandler existe: {0}".format(interfaz is not None))
except Exception as error:
    paso(3, "no se pudo leer la interfaz: {0}".format(error))

# 4) Dos BMP de 24 bits y 32x32 en una carpeta sin tildes ni espacios
carpeta = carpeta_bmp()
rutas = {}
try:
    for clave, texto, color in (("A", NUMERO, VERDE), ("B", "B", AZUL)):
        ruta = os.path.join(carpeta, "etiqueta-{0}.bmp".format(clave))
        if os.path.exists(ruta):
            os.remove(ruta)
        como = generar_bmp(ruta, texto, color)
        ancho, alto, bpp, tamano = cabecera_bmp(ruta)
        rutas[clave] = ruta
        paso(4, "BMP {0}: {1} | {2}x{3}, {4} bits, {5} bytes (esperado {6}) | {7} | ruta sin tildes ni espacios: {8}".format(
            clave, ruta, ancho, alto, bpp, tamano, 54 + TAMANO * TAMANO * 3, como,
            all(ord(c) < 128 for c in ruta) and " " not in ruta))
except Exception as error:
    paso(4, "no se pudo generar el BMP: {0}".format(error))
    print(traceback.format_exc())
    print("PARADA: sin imagen no hay control. Anota el error.")
    raise SystemExit

# 5) Posiciones en unidades internas (pies): A = punto de trabajo de N4; B = centro de la caja de seccion / recorte / barra
punto_a = DB.XYZ(pies(PUNTO_MM[0]), pies(PUNTO_MM[1]), pies(PUNTO_MM[2]))
paso(5, "Etiqueta A (N4): {0}".format(xyz_texto(punto_a)))
punto_b = None
origen_b = None
try:
    if caja_seccion is not None:
        punto_b = caja_seccion.Transform.OfPoint((caja_seccion.Min + caja_seccion.Max) * 0.5)
        origen_b = "centro de la caja de seccion"
    elif vista.CropBoxActive:
        recorte = vista.CropBox
        punto_b = recorte.Transform.OfPoint((recorte.Min + recorte.Max) * 0.5)
        origen_b = "centro del recuadro de recorte"
    else:
        barra = doc.GetElement(DB.ElementId(BARRA_PRUEBA_ID))
        curva = barra.Location.Curve
        punto_b = (curva.GetEndPoint(0) + curva.GetEndPoint(1)) * 0.5
        origen_b = "centro de la barra {0}".format(BARRA_PRUEBA_ID)
    paso(5, "Etiqueta B ({0}): {1}".format(origen_b, xyz_texto(punto_b)))
except Exception as error:
    paso(5, "sin etiqueta B (no se pudo calcular el punto: {0})".format(error))
    punto_b = None

# 6) Controles en el lienzo (sin transaccion: no son elementos del modelo), SetVisibility, tooltip, GetAll y refresco
indices = []
manager = None
try:
    manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
    paso(6, "TemporaryGraphicsManager obtenido: {0}".format(manager is not None))
    for clave, punto in (("A", punto_a), ("B", punto_b)):
        if punto is None:
            continue
        datos = DB.InCanvasControlData(rutas[clave], punto)
        indice = manager.AddControl(datos, vista.Id)
        indices.append(indice)
        paso(6, "Control {0} anadido en la vista {1}: indice {2} | ImagePath={3} | Position {4}".format(
            clave, vista.Id.Value, indice, datos.ImagePath, xyz_texto(datos.Position)))
        try:
            manager.SetVisibility(indice, True)
            paso(6, "SetVisibility({0}, True) llamado".format(indice))
        except Exception as error:
            paso(6, "SetVisibility({0}, True) fallo: {1}".format(indice, error))
        try:
            manager.SetTooltip(indice, "MotorConexiones: etiqueta {0} (sondeo 19 v3)".format(clave))
            paso(6, "SetTooltip({0}) puesto".format(indice))
        except Exception as error:
            paso(6, "SetTooltip({0}) fallo: {1}".format(indice, error))
    try:
        todos = list(manager.GetAll())
        paso(6, "GetAll(): {0} control(es) en el documento: {1}".format(len(todos), ", ".join(str(t) for t in todos)))
    except Exception as error:
        paso(6, "GetAll() fallo: {0}".format(error))
    try:
        uidoc.RefreshActiveView()
        paso(7, "uidoc.RefreshActiveView() llamado")
    except Exception as error:
        paso(7, "RefreshActiveView fallo: {0}".format(error))
    try:
        uidoc.UpdateAllOpenViews()
        paso(7, "uidoc.UpdateAllOpenViews() llamado")
    except Exception as error:
        paso(7, "UpdateAllOpenViews fallo: {0}".format(error))
    guardar_estado(indices, vista.Id.Value, [rutas[k] for k in sorted(rutas)])
except Exception as error:
    paso(6, "no se pudo anadir el control: {0}".format(error))
    print(traceback.format_exc())

# 8) Captura exportada (seguramente sin los controles: son graficos de pantalla) + captura a mano
if indices:
    try:
        if not os.path.isdir(CARPETA):
            os.makedirs(CARPETA)
        for viejo in glob.glob(os.path.join(CARPETA, NOMBRE_CAPTURA + "*.png")):
            os.remove(viejo)
        opciones = DB.ImageExportOptions()
        opciones.FilePath = os.path.join(CARPETA, NOMBRE_CAPTURA + "-exportada")
        opciones.ExportRange = DB.ExportRange.VisibleRegionOfCurrentView
        opciones.HLRandWFViewsFileType = DB.ImageFileType.PNG
        opciones.ShadowViewsFileType = DB.ImageFileType.PNG
        opciones.ZoomType = DB.ZoomFitType.FitToPage
        opciones.PixelSize = 1600
        doc.ExportImage(opciones)
        generados = glob.glob(os.path.join(CARPETA, NOMBRE_CAPTURA + "-exportada*.png"))
        paso(8, "Captura exportada: {0} (lo normal es que NO ensene las etiquetas: haz la captura a mano, {1}.png)".format(
            generados[0] if generados else "no se genero ningun PNG", NOMBRE_CAPTURA))
    except Exception as error:
        paso(8, "Captura exportada: fallo ({0}); haz la captura a mano, {1}.png".format(error, NOMBRE_CAPTURA))

# 9) Manejador de clic (igual que en la v2, que se registro bien): cuadro + linea en sondeo19-clics.txt
if not indices:
    paso(9, "manejador de clic no probado (no hay control)")
elif interfaz is None or servicio is None:
    paso(9, "manejador de clic no probado: falta la interfaz UI.ITemporaryGraphicsHandler o el servicio (paso 3)")
else:
    try:
        def anota_clic(data):
            lineas = []
            try:
                lineas.append("clic en una etiqueta: Index={0} | Document={1}".format(data.Index, data.Document.Title))
            except Exception as error_datos:
                lineas.append("clic en una etiqueta (datos no legibles: {0})".format(error_datos))
            try:
                if not os.path.isdir(CARPETA_LOG):
                    os.makedirs(CARPETA_LOG)
                with open(ARCHIVO_CLICS, "a") as f:
                    f.write("\n".join(lineas) + "\n")
            except Exception:
                pass
            try:
                UI.TaskDialog.Show("MotorConexiones - sondeo 19", "Clic recibido.\n\n{0}".format("\n".join(lineas)))
            except Exception:
                pass

        class ManejadorSondeo19(interfaz):
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
                return "Manejador de prueba de clics en etiquetas del lienzo (sondeo 19 v3). Se desactiva con 19b."

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
        paso(9, "manejador registrado y activo como servidor {0} de {1}: pincha una etiqueta en Revit; debe salir un cuadro y una linea en {2}".format(
            guid, servicio.Name, ARCHIVO_CLICS))
    except Exception as error:
        paso(9, "no se pudo registrar el manejador de clic: {0}".format(error))
        print(traceback.format_exc())

paso(10, "Las etiquetas se quedan puestas (indices {0}, vista {1}) para que la persona las mire y las pinche; despues ejecuta 19b-etiquetas-quitar.py".format(
    ", ".join(str(i) for i in indices) if indices else "ninguno", vista.Id.Value))
paso(11, "Marcadores DirectShape de plan en el modelo (no los toca este sondeo): {0}".format(
    len([e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == "MotorConexiones.Plan"])))
print("=== fin 19-etiquetas-lienzo ===")
