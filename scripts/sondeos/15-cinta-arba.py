# -*- coding: utf-8 -*-
# Sondeo 15 (Fase 6): lista las pestanas y paneles de la cinta de Revit con Autodesk.Windows (AdWindows): id, titulo,
# visible y, en las pestanas que interesan (ARBA, Conexiones), sus paneles y botones con su id. Sirve para saber en que
# pestana quedo el panel MotorConexiones y quien creo cada cosa: los ids de la API de Revit empiezan por
# "CustomCtrl_%CustomCtrl_%<pestana>%<panel>%<boton>"; los de pyRevit llevan el nombre de la extension (IA-Tools).
# Solo lee; no modifica el modelo ni la cinta.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\15-cinta-arba.py -SinTransaccion
from __future__ import print_function
import clr

clr.AddReference("AdWindows")
from Autodesk.Windows import ComponentManager  # noqa: E402

INTERES = ("ARBA", "CONEXIONES", "MOTORCONEXIONES")


def seguro(funcion, por_defecto="?"):
    try:
        return funcion()
    except Exception as error:
        return "{0} ({1})".format(por_defecto, type(error).__name__)


def texto(valor):
    return str(valor).replace("\r", " ").replace("\n", " ")


print("=== 15-cinta-arba ===")
cinta = ComponentManager.Ribbon
if cinta is None:
    print("PARADA: ComponentManager.Ribbon es None (sin interfaz de usuario).")
    raise SystemExit

pestanas = list(cinta.Tabs)
print("pestanas en la cinta: {0}".format(len(pestanas)))
titulos = []
donde_motor = []
for pestana in pestanas:
    titulo = seguro(lambda: pestana.Title)
    pid = seguro(lambda: pestana.Id)
    visible = seguro(lambda: pestana.IsVisible)
    paneles = seguro(lambda: list(pestana.Panels), [])
    titulos.append(texto(titulo))
    print("- pestana id='{0}' titulo='{1}' visible={2} paneles={3}".format(texto(pid), texto(titulo), visible, len(paneles)))
    detallar = any(clave in texto(titulo).upper() or clave in texto(pid).upper() for clave in INTERES)
    for panel in paneles:
        fuente = seguro(lambda: panel.Source, None)
        panel_id = seguro(lambda: fuente.Id)
        panel_titulo = seguro(lambda: fuente.Title)
        elementos = seguro(lambda: list(fuente.Items), [])
        if "MOTORCONEXIONES" in texto(panel_id).upper() or texto(panel_titulo) == "MotorConexiones":
            donde_motor.append(texto(titulo))
        if not detallar:
            continue
        print("    panel id='{0}' titulo='{1}' elementos={2}".format(texto(panel_id), texto(panel_titulo), len(elementos)))
        for elemento in elementos:
            print("      elemento id='{0}' texto='{1}' tipo={2} visible={3}".format(
                texto(seguro(lambda: elemento.Id)), texto(seguro(lambda: elemento.Text)),
                texto(seguro(lambda: elemento.GetType().Name)), seguro(lambda: elemento.IsVisible)))

print("pestana ARBA existe: {0}".format("ARBA" in titulos))
print("pestana Conexiones existe: {0}".format("Conexiones" in titulos))
if donde_motor:
    print("panel MotorConexiones encontrado en: {0}".format(", ".join(donde_motor)))
else:
    print("panel MotorConexiones: NO encontrado en ninguna pestana (el add-in no cargo o el id del panel cambio)")
print("=== fin 15-cinta-arba ===")
