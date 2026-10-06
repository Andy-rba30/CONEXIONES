# -*- coding: utf-8 -*-
# Sondeo 21 (Fase 10, V3): mira las ETIQUETAS DEL ADD-IN en el lienzo sin pasar por el add-in: cuantos controles tiene el
# TemporaryGraphicsManager del documento (GetAll), si el servidor de clics de MotorConexiones (PlanLabels.ServerId,
# 3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d) esta registrado y activo en el servicio TemporaryGraphicsHandlerService, que
# carpeta de BMP uso el add-in (la primera sin tildes ni espacios: %LOCALAPPDATA%\MotorConexiones\etiquetas o la siguiente
# candidata) y cuantos BMP hay en ella. No cambia nada, salvo con LIMPIAR = True (quita todos los controles con Clear(), por
# si Descartar no pudo; los planes en memoria del add-in se enteran al replanificar). Sin transaccion: los controles del
# lienzo no son elementos del modelo.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import glob
import tempfile
import traceback

LIMPIAR = False
GUID_ADDIN = "3f6c1b2e-7a8d-4c5b-9e21-0d4f5a6b7c8d"   # PlanLabels.ServerId (el add-in)
GUID_SONDEO19 = "7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60"  # el servidor de prueba del sondeo 19 (si se ejecuto en esta sesion)
APP_ID_PLAN = "MotorConexiones.Plan"


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


def es_plana(ruta):
    return all(ord(c) < 128 for c in ruta) and " " not in ruta


print("=== 21-etiquetas-addin (Fase 10) ===")
paso(1, "Documento: {0} | vista activa: {1} ({2})".format(doc.Title, doc.ActiveView.Name, doc.ActiveView.ViewType))

# 2) Controles del lienzo
presentes = None
try:
    manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
    presentes = [int(i) for i in manager.GetAll()]
    paso(2, "Controles en el lienzo (GetAll): {0}{1}".format(len(presentes), (": " + ", ".join(str(i) for i in presentes)) if presentes else ""))
    for indice in presentes[:40]:
        try:
            visible = manager.IsVisible(indice)
        except Exception as error:
            visible = "? ({0})".format(error)
        print("   control {0}: visible={1}".format(indice, visible))
except Exception as error:
    paso(2, "no se pudo leer el TemporaryGraphicsManager: {0}".format(error))
    print(traceback.format_exc())

# 3) Servidor de clics del add-in
try:
    builtin = DB.ExternalService.ExternalServices.BuiltInExternalServices
    servicio = DB.ExternalService.ExternalServiceRegistry.GetService(builtin.TemporaryGraphicsHandlerService)
    registrados = [str(g).lower() for g in servicio.GetRegisteredServerIds()]
    activos = []
    try:
        activos = [str(g).lower() for g in servicio.GetActiveServerIds()]
    except Exception:
        try:
            activos = [str(servicio.GetActiveServerId()).lower()]
        except Exception as error:
            activos = ["? ({0})".format(error)]
    paso(3, "Servicio {0}: {1} servidor(es) registrados, activos: {2}".format(servicio.Name, len(registrados), ", ".join(activos) if activos else "ninguno"))
    paso(3, "Servidor del add-in {0}: registrado={1} | activo={2}".format(GUID_ADDIN, GUID_ADDIN in registrados, GUID_ADDIN in activos))
    paso(3, "Servidor del sondeo 19 {0}: registrado={1} | activo={2}".format(GUID_SONDEO19, GUID_SONDEO19 in registrados, GUID_SONDEO19 in activos))
    for g in registrados:
        try:
            srv = servicio.GetServer(System.Guid(g))
            print("   {0}: {1} ({2})".format(g, srv.GetName(), srv.GetVendorId()))
        except Exception as error:
            print("   {0}: (no legible: {1})".format(g, error))
except Exception as error:
    paso(3, "no se pudo leer el servicio de clics: {0}".format(error))
    print(traceback.format_exc())

# 4) Carpeta de los BMP del add-in (las mismas candidatas que PlanLabels.Folder, en el mismo orden)
candidatas = [os.path.join(os.environ.get("LOCALAPPDATA", ""), "MotorConexiones", "etiquetas"),
              os.path.join(os.environ.get("PUBLIC", r"C:\Users\Public"), "MotorConexiones", "etiquetas"),
              r"C:\MotorConexiones\etiquetas",
              os.path.join(tempfile.gettempdir(), "MotorConexiones-etiquetas")]
elegida = None
for carpeta in candidatas:
    if carpeta and es_plana(carpeta) and os.path.isdir(carpeta):
        elegida = carpeta
        break
if elegida is None:
    paso(4, "Carpeta de etiquetas: ninguna de las candidatas existe todavia (el add-in la crea al planificar): {0}".format(" | ".join(candidatas)))
else:
    bmps = sorted(glob.glob(os.path.join(elegida, "etiqueta-*.bmp")))
    paso(4, "Carpeta de etiquetas: {0} (sin tildes ni espacios: {1}) | {2} BMP".format(elegida, es_plana(elegida), len(bmps)))
    for ruta in bmps[:12]:
        print("   {0} ({1} bytes{2})".format(os.path.basename(ruta), os.path.getsize(ruta), "" if os.path.getsize(ruta) == 3126 else ", esperado 3126"))

# 5) Marcadores de plan en el modelo, para comparar (un marcador y una etiqueta por nudo visible; con V2, ademas una cartela fantasma)
try:
    marcadores = [e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == APP_ID_PLAN]
    fantasmas = [m for m in marcadores if (m.ApplicationDataId or "").endswith(":ghost")]
    paso(5, "Marcadores de plan en el modelo: {0} (de ellos, cartelas fantasma: {1})".format(len(marcadores), len(fantasmas)))
except Exception as error:
    paso(5, "no se pudieron contar los marcadores: {0}".format(error))

# 6) Limpiar (solo con LIMPIAR = True)
if LIMPIAR:
    try:
        manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
        manager.Clear()
        uidoc.RefreshActiveView()
        paso(6, "Clear() llamado: controles despues = {0}".format(len(list(manager.GetAll()))))
    except Exception as error:
        paso(6, "Clear fallo: {0}".format(error))
else:
    paso(6, "Sin limpiar (LIMPIAR = False). Se espera 0 controles tras Descartar plan o batch_plan_discard; si quedan, pon LIMPIAR = True y repite.")
print("=== fin 21-etiquetas-addin ===")
