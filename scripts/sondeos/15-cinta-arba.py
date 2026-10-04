# -*- coding: utf-8 -*-
# Sondeo 15 (Fase 6): lista las pestanas y paneles de la cinta de Revit con Autodesk.Windows.ComponentManager.Ribbon
# para ver donde quedo el panel "Conexiones" de MotorConexiones (en la pestana ARBA o en la pestana "Conexiones" de
# reserva) y que botones tiene. Solo lectura: no toca el modelo ni la cinta.
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\15-cinta-arba.py -SinTransaccion
from __future__ import print_function
import clr

print("=== 15-cinta-arba ===")
try:
    clr.AddReference("AdWindows")
    import Autodesk.Windows as AW
except Exception as e:
    print("PARADA: no se pudo cargar AdWindows: {}".format(e))
    raise SystemExit


def texto(obj, *nombres):
    for n in nombres:
        try:
            v = getattr(obj, n)
            if v is not None and str(v) != "":
                return str(v)
        except Exception:
            pass
    return ""


ribbon = AW.ComponentManager.Ribbon
if ribbon is None:
    print("PARADA: ComponentManager.Ribbon es None (la cinta no esta lista).")
    raise SystemExit

interes = ("ARBA", "CONEXIONES", "MOTORCONEXIONES")
total = 0
for tab in ribbon.Tabs:
    total += 1
    tid = texto(tab, "Id")
    ttitulo = texto(tab, "Title", "Name")
    detalle = (tid + " " + ttitulo).upper()
    marca = "  <-- interesa" if any(k in detalle for k in interes) else ""
    print("Pestana: id='{}' titulo='{}' visible={}{}".format(tid, ttitulo, texto(tab, "IsVisible"), marca))
    if not marca:
        continue
    try:
        paneles = list(tab.Panels)
    except Exception as e:
        print("   (sin paneles: {})".format(e))
        continue
    for panel in paneles:
        fuente = None
        try:
            fuente = panel.Source
        except Exception:
            pass
        pid = texto(fuente, "Id") if fuente is not None else texto(panel, "Id")
        ptitulo = texto(fuente, "Title", "Name") if fuente is not None else ""
        print("   Panel: id='{}' titulo='{}' visible={}".format(pid, ptitulo, texto(panel, "IsVisible")))
        try:
            items = list(fuente.Items) if fuente is not None else []
        except Exception:
            items = []
        for item in items:
            iid = texto(item, "Id")
            itexto = texto(item, "Text").replace("\n", " / ").replace("\r", "")
            tipo = type(item).__name__
            print("      Boton: tipo={} id='{}' texto='{}'".format(tipo, iid, itexto))
            # Botones apilados o desplegables: un nivel mas.
            try:
                hijos = list(item.Items)
            except Exception:
                hijos = []
            for hijo in hijos:
                print("         - tipo={} id='{}' texto='{}'".format(type(hijo).__name__, texto(hijo, "Id"), texto(hijo, "Text").replace("\n", " / ")))

print("Pestanas en total: {}".format(total))
print("Esperado (Fase 6): pestana ARBA con un panel 'Conexiones' y dos botones 'Ejecutar especificacion JSON' y "
      "'Conexiones del modelo'; sin pestana 'Conexiones' aparte. Si la pestana 'Conexiones' existe, ARBA fallo y el "
      "log del add-in tiene 'ribbon_arba_failed'.")
