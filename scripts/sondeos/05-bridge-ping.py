# -*- coding: utf-8 -*-
# Sondeo 05: llamar a MotorConexiones.Revit.Bridge.Handle("ping") por reflexion, exactamente como lo hace
# mcp/revit_mcp/conexiones.py, pero desde /execute_code/ (no necesita tener instalado conexiones.py).
# Demuestra que el add-in esta cargado por Revit y que el mecanismo del puente funciona.
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\05-bridge-ping.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os

NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"
RUTA_DLL = os.path.join(os.environ.get("APPDATA", ""), "Autodesk", "Revit", "Addins", "2027",
                        "MotorConexiones", "MotorConexiones.Revit.dll")


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def contexto_de_carga(ensamblado):
    try:
        try:
            clr.AddReference("System.Runtime.Loader")
        except Exception:
            pass
        from System.Runtime.Loader import AssemblyLoadContext
        ctx = AssemblyLoadContext.GetLoadContext(ensamblado)
        return ctx.Name if ctx is not None else "None"
    except Exception:
        return "?"


print("=== 05-bridge-ping ===")
print("1) DLL desplegada existe: {0} ({1})".format(os.path.isfile(RUTA_DLL), RUTA_DLL))
manifiesto = os.path.join(os.path.dirname(os.path.dirname(RUTA_DLL)), "MotorConexiones.addin")
print("   manifiesto existe: {0} ({1})".format(os.path.isfile(manifiesto), manifiesto))

ensamblado = buscar_ensamblado(NOMBRE_ENSAMBLADO)
if ensamblado is None:
    print("2) MotorConexiones.Revit NO esta cargado en el proceso de Revit.")
    print("   Causas: no se ejecuto scripts\\deploy.ps1, o Revit no se reinicio despues, o el add-in fallo al cargar")
    print("   (mira %LOCALAPPDATA%\\MotorConexiones\\log\\ y el aviso de Revit al arrancar).")
else:
    print("2) MotorConexiones.Revit cargado: {0}".format(ensamblado.FullName))
    print("   Location: {0}".format(ensamblado.Location))
    print("   AssemblyLoadContext: {0}".format(contexto_de_carga(ensamblado)))
    nucleo = buscar_ensamblado("MotorConexiones.Core")
    print("   MotorConexiones.Core cargado: {0}".format(nucleo is not None))

    tipo = ensamblado.GetType("MotorConexiones.Revit.Bridge")
    print("3) Tipo Bridge encontrado: {0}".format(tipo is not None))
    if tipo is not None:
        from Autodesk.Revit.UI import UIDocument
        firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                           clr.GetClrType(DB.Document), clr.GetClrType(UIDocument)])
        metodo = tipo.GetMethod("Handle", firma)
        print("   Metodo Handle(String, String, Document, UIDocument): {0}".format(metodo is not None))
        if metodo is not None:
            for operacion, cuerpo in (("ping", "{}"), ("no_existe", "{}"), ("ping", "esto no es json")):
                try:
                    argumentos = System.Array[System.Object]([operacion, cuerpo, doc, revit.uidoc])
                    respuesta = metodo.Invoke(None, argumentos)
                    print("4) Handle('{0}', '{1}') ->".format(operacion, cuerpo))
                    print("   " + str(respuesta)[:1500])
                except Exception as error:
                    interna = getattr(error, "InnerException", None)
                    print("4) Handle('{0}') ERROR {1}: {2}".format(operacion, type(error).__name__, str(error)[:300]))
                    if interna is not None:
                        print("   interna: " + str(interna)[:300])

print("=== fin 05-bridge-ping ===")
