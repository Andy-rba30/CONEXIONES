# -*- coding: utf-8 -*-
# Sondeo 08: guardar el modelo abierto como copia "<nombre>_sondeo.rvt" en la misma carpeta y seguir
# trabajando sobre la copia (las pruebas de escritura de la Fase 1 nunca tocan el modelo original).
# Se ejecuta SIN transaccion envolvente (SaveAs no admite transacciones abiertas):
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\08-guardar-copia.py -SinTransaccion
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import os

print("=== 08-guardar-copia ===")
ruta = doc.PathName
print("Documento: {0}".format(doc.Title))
print("Ruta actual: {0}".format(ruta))
if not ruta:
    print("ERROR: el documento no esta guardado en disco. Guardalo primero (Ctrl+S) y repite.")
elif doc.IsWorkshared:
    print("ERROR: el modelo es de trabajo compartido (IsWorkshared=True). Haz la copia a mano: Archivo > Guardar como > Proyecto, nombre HANGAR_PRUEBA_sondeo.rvt, y sigue con esa copia abierta.")
elif ruta.lower().endswith("_sondeo.rvt"):
    print("Ya estas trabajando sobre la copia de sondeo. No se hace nada.")
elif doc.IsModifiable:
    print("ERROR: hay una transaccion abierta (IsModifiable=True). Cierra la orden en curso en Revit y repite.")
else:
    carpeta, nombre = os.path.split(ruta)
    base, ext = os.path.splitext(nombre)
    destino = os.path.join(carpeta, base + "_sondeo" + ext)
    opciones = DB.SaveAsOptions()
    opciones.OverwriteExistingFile = True
    try:
        doc.SaveAs(destino, opciones)
        print("Copia guardada y abierta: {0}".format(doc.PathName))
        print("Existe en disco: {0}".format(os.path.isfile(destino)))
    except Exception as error:
        print("ERROR al guardar como {0}: {1}: {2}".format(destino, type(error).__name__, str(error)[:300]))
print("=== fin 08-guardar-copia ===")
