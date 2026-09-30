# -*- coding: utf-8 -*-
# Sondeo 04: ensamblados cargados en el proceso de Revit relacionados con acero ("Steel", "AdvanceSteel", "ASMgd"),
# archivos de acero en disco, complementos instalados y tipos de conexion estructural cargados en el documento.
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\04-ensamblados.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os

CLAVES = ("steel", "advancesteel", "asmgd")


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


def nombre(elemento):
    try:
        return elemento.Name
    except Exception:
        try:
            return DB.Element.Name.__get__(elemento)
        except Exception:
            return "?"


print("=== 04-ensamblados ===")

# 1) Ensamblados cargados cuyo nombre contiene Steel / AdvanceSteel / ASMgd
print("1) Ensamblados cargados relacionados con acero:")
encontrados = 0
for a in System.AppDomain.CurrentDomain.GetAssemblies():
    try:
        nombre_asm = a.GetName().Name
    except Exception:
        continue
    baja = nombre_asm.lower()
    if any(clave in baja for clave in CLAVES):
        encontrados += 1
        try:
            ubicacion = a.Location if not a.IsDynamic else "(dinamico)"
        except Exception:
            ubicacion = "?"
        print("   {0} | version {1} | {2} | {3}".format(nombre_asm, a.GetName().Version, contexto_de_carga(a), ubicacion))
print("   total: {0}".format(encontrados))

# 2) Archivos de acero en disco
carpeta_revit = os.path.dirname(str(clr.GetClrType(DB.Element).Assembly.Location))
print("2) Archivos en la carpeta de Revit ({0}):".format(carpeta_revit))
vistos = []
for patron in ("*Steel*.dll", "AS*.dll", "*AdvanceSteel*", "*Steel*.addin", "*Connection*.dll"):
    try:
        for ruta in System.IO.Directory.GetFiles(carpeta_revit, patron):
            if ruta not in vistos:
                vistos.append(ruta)
    except Exception as error:
        print("   no se pudo listar " + patron + ": " + str(error)[:100])
for ruta in sorted(vistos):
    try:
        tamano = System.IO.FileInfo(ruta).Length // 1024
    except Exception:
        tamano = "?"
    print("   {0} ({1} KB)".format(os.path.basename(ruta), tamano))
print("   total: {0}".format(len(vistos)))
try:
    subcarpetas = [os.path.basename(c) for c in System.IO.Directory.GetDirectories(carpeta_revit) if "steel" in os.path.basename(c).lower() or c.lower().endswith("addins")]
    print("   subcarpetas con 'steel' o AddIns: {0}".format(subcarpetas))
except Exception:
    pass

# 3) Complementos instalados (ApplicationPlugins y carpetas Addins de 2027)
print("3) Complementos:")
program_data = System.Environment.GetEnvironmentVariable("ProgramData") or r"C:\ProgramData"
app_data = System.Environment.GetEnvironmentVariable("APPDATA") or ""
rutas = [
    os.path.join(program_data, "Autodesk", "ApplicationPlugins"),
    os.path.join(program_data, "Autodesk", "Revit", "Addins", "2027"),
    os.path.join(app_data, "Autodesk", "Revit", "Addins", "2027"),
]
for carpeta in rutas:
    if not os.path.isdir(carpeta):
        print("   (no existe) " + carpeta)
        continue
    try:
        entradas = sorted(os.listdir(carpeta))
    except Exception as error:
        print("   no se pudo listar " + carpeta + ": " + str(error)[:100])
        continue
    print("   {0}: {1} entradas".format(carpeta, len(entradas)))
    for entrada in entradas[:60]:
        marca = " <-- acero/conexiones" if any(clave in entrada.lower() for clave in ("steel", "connection", "conexion", "acero")) else ""
        print("     " + entrada + marca)

# 4) Pestana Steel de la cinta y tipos de conexion estructural en el documento
for pestana in ("Steel", "Acero"):
    try:
        from pyrevit import HOST_APP
        paneles = HOST_APP.uiapp.GetRibbonPanels(pestana)
        print("4) Pestana '{0}' de la cinta: {1} paneles".format(pestana, len(list(paneles))))
    except Exception as error:
        print("4) Pestana '{0}' de la cinta: no accesible ({1}: {2})".format(pestana, type(error).__name__, str(error)[:120]))
try:
    tipos = list(DB.FilteredElementCollector(doc).OfClass(DB.Structure.StructuralConnectionHandlerType).ToElements())
    print("   StructuralConnectionHandlerType cargados en el documento: {0}".format(len(tipos)))
    for t in tipos[:30]:
        print("     [{0}] {1}".format(int(t.Id.Value), nombre(t)))
except Exception as error:
    print("   StructuralConnectionHandlerType: ERROR {0}: {1}".format(type(error).__name__, str(error)[:150]))
try:
    cuenta = DB.FilteredElementCollector(doc).OfClass(DB.Structure.StructuralConnectionHandler).GetElementCount()
    print("   StructuralConnectionHandler (conexiones existentes): {0}".format(cuenta))
except Exception as error:
    print("   StructuralConnectionHandler: ERROR {0}: {1}".format(type(error).__name__, str(error)[:150]))
try:
    cuenta = DB.FilteredElementCollector(doc).OfCategory(DB.BuiltInCategory.OST_StructConnections).WhereElementIsNotElementType().GetElementCount()
    print("   Elementos de la categoria OST_StructConnections: {0}".format(cuenta))
except Exception as error:
    print("   OST_StructConnections: ERROR {0}: {1}".format(type(error).__name__, str(error)[:150]))

print("=== fin 04-ensamblados ===")
