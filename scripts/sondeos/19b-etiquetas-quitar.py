# -*- coding: utf-8 -*-
# Sondeo 19b (cierre de la ronda 8c; cierre de la 8d: varios controles y rutas BMP en el estado; cierre de la 8e: solo quita los
# controles que Revit dice tener, porque tras un reinicio de Revit ya no queda ninguno y RemoveControl avisaba "index is out of
# range"; borra tambien los BMP naranjas de la v4): segunda mitad del sondeo 19.
# Se ejecuta DESPUES de que la persona haya mirado y pinchado las etiquetas que dejo puestas 19-etiquetas-lienzo.py. Ensena los
# clics anotados en %LOCALAPPDATA%\MotorConexiones\log\sondeo19-clics.txt, quita los controles del lienzo (RemoveControl con
# cada indice guardado y Clear por si acaso), desactiva el
# servidor de prueba del servicio externo (un servidor registrado no se puede quitar hasta reiniciar Revit; se deja
# inactivo) y comprueba que no queda nada. Sin transaccion: los controles temporales no son elementos del modelo.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os
import tempfile
import traceback

CARPETA_LOG = os.path.join(os.environ.get("LOCALAPPDATA", tempfile.gettempdir()), "MotorConexiones", "log")
ARCHIVO_CLICS = os.path.join(CARPETA_LOG, "sondeo19-clics.txt")
ARCHIVO_ESTADO = os.path.join(CARPETA_LOG, "sondeo19-estado.txt")
GUID_SERVIDOR = "7f3a2c1e-8d4b-4e6f-9a10-5b2c3d4e5f60"


def paso(numero, texto):
    print("{0}) {1}".format(numero, texto))


print("=== 19b-etiquetas-quitar ===")

# 1) Clics anotados por el manejador del sondeo 19
try:
    if os.path.exists(ARCHIVO_CLICS):
        with open(ARCHIVO_CLICS) as f:
            contenido = f.read().strip()
        paso(1, "clics anotados en {0}:\n{1}".format(ARCHIVO_CLICS, contenido if contenido else "(archivo vacio)"))
    else:
        paso(1, "ningun clic anotado ({0} no existe): o no se pincho la etiqueta, o el manejador no se registro, o Revit no avisa de los clics".format(ARCHIVO_CLICS))
except Exception as error:
    paso(1, "no se pudo leer el archivo de clics: {0}".format(error))

# 2) Quitar los controles del lienzo (v3 deja dos: A en N4 y B en el centro de la caja de seccion)
indices = []
vista_id = None
rutas_bmp = []
try:
    if os.path.exists(ARCHIVO_ESTADO):
        with open(ARCHIVO_ESTADO) as f:
            partes = f.read().strip().split(";")
        indices = [int(i) for i in partes[0].split(",") if i.strip() != ""]
        vista_id = int(partes[1])
        if len(partes) > 2 and partes[2]:
            rutas_bmp = partes[2].split("|")
        paso(2, "estado del sondeo 19: control(es) {0} en la vista {1}".format(", ".join(str(i) for i in indices), vista_id))
    else:
        paso(2, "sin estado guardado ({0}): se intenta solo Clear".format(ARCHIVO_ESTADO))
except Exception as error:
    paso(2, "no se pudo leer el estado: {0}".format(error))

manager = None
try:
    manager = DB.TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)
    presentes = None
    try:
        presentes = [int(i) for i in manager.GetAll()]
        paso(2, "GetAll() antes de quitar: {0} control(es){1}".format(len(presentes), (": " + ", ".join(str(i) for i in presentes)) if presentes else ""))
    except Exception as error:
        paso(2, "GetAll() fallo: {0}".format(error))
    for indice in indices:
        if presentes is not None and indice not in presentes:
            paso(3, "Control {0} ya no estaba (Revit se reinicio o se quito antes): nada que quitar".format(indice))
            continue
        try:
            manager.RemoveControl(indice)
            paso(3, "Control {0} quitado".format(indice))
        except Exception as error:
            paso(3, "RemoveControl({0}) fallo: {1}".format(indice, error))
    try:
        manager.Clear()
        paso(4, "TemporaryGraphicsManager.Clear() llamado")
    except Exception as error:
        paso(4, "Clear no disponible o fallo: {0}".format(error))
    try:
        paso(4, "GetAll() despues de quitar: {0} control(es) (debe ser 0)".format(len(list(manager.GetAll()))))
    except Exception as error:
        paso(4, "GetAll() fallo: {0}".format(error))
    try:
        uidoc.RefreshActiveView()
        uidoc.UpdateAllOpenViews()
        paso(4, "vista refrescada (RefreshActiveView + UpdateAllOpenViews)")
    except Exception as error:
        paso(4, "refresco fallo: {0}".format(error))
except Exception as error:
    paso(3, "no se pudo obtener el TemporaryGraphicsManager: {0}".format(error))
    print(traceback.format_exc())

# 3) Desactivar el servidor de prueba (si se registro)
try:
    builtin = DB.ExternalService.ExternalServices.BuiltInExternalServices
    tipo_builtin = clr.GetClrType(builtin)
    nombres = [p.Name for p in tipo_builtin.GetProperties() if "temporary" in p.Name.lower() or "canvas" in p.Name.lower()]
    if not nombres:
        paso(5, "ningun servicio externo con Temporary/Canvas en el nombre: nada que desactivar")
    for nombre in nombres:
        try:
            servicio = DB.ExternalService.ExternalServiceRegistry.GetService(getattr(builtin, nombre))
            registrados = [str(g) for g in servicio.GetRegisteredServerIds()]
            if GUID_SERVIDOR in registrados:
                if hasattr(servicio, "SetActiveServers"):
                    from System.Collections.Generic import List
                    servicio.SetActiveServers(List[System.Guid]())
                    paso(5, "servidor de prueba {0} desactivado en {1} (sigue registrado hasta reiniciar Revit)".format(GUID_SERVIDOR, servicio.Name))
                elif hasattr(servicio, "SetActiveServer"):
                    paso(5, "servicio {0} de un solo servidor: no se puede dejar sin servidor activo por la API; reinicia Revit al terminar".format(servicio.Name))
                else:
                    paso(5, "servicio {0}: sin SetActiveServers; reinicia Revit al terminar".format(servicio.Name))
            else:
                paso(5, "el servidor de prueba no esta registrado en {0} (registrados: {1})".format(servicio.Name, len(registrados)))
        except Exception as error:
            paso(5, "servicio {0}: no se pudo leer o desactivar ({1})".format(nombre, error))
except Exception as error:
    paso(5, "no se pudieron leer los servicios externos: {0}".format(error))

# 4) Limpiar los archivos auxiliares (estado y los BMP que anoto el sondeo 19) y comprobar
for ruta in [ARCHIVO_ESTADO, os.path.join(tempfile.gettempdir(), "motorconexiones-etiqueta-N4.bmp")] + rutas_bmp:
    try:
        if os.path.exists(ruta):
            os.remove(ruta)
    except Exception:
        pass
paso(6, "Marcadores DirectShape de plan en el modelo (no los toca este sondeo): {0}".format(
    len([e for e in DB.FilteredElementCollector(doc).OfClass(DB.DirectShape) if e.ApplicationId == "MotorConexiones.Plan"])))
print("=== fin 19b-etiquetas-quitar ===")
