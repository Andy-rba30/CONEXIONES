# -*- coding: utf-8 -*-
# Sondeo 00: version de Revit, runtime .NET, IronPython, pyRevit y documento abierto.
# Se ejecuta dentro de Revit con: .\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import sys


def linea(etiqueta, valor):
    print("{0}: {1}".format(etiqueta, valor))


def intentar(etiqueta, funcion):
    try:
        linea(etiqueta, funcion())
    except Exception as error:
        linea(etiqueta, "ERROR " + type(error).__name__ + ": " + str(error))


print("=== 00-version ===")

# 1. Aplicacion de Revit (doc.Application es DB.Application)
app = None
try:
    app = doc.Application
except Exception:
    app = None
if app is None:
    try:
        from pyrevit import HOST_APP
        app = HOST_APP.app
    except Exception:
        app = None

if app is None:
    print("No se pudo obtener la aplicacion de Revit (no hay documento abierto?)")
else:
    intentar("VersionNumber", lambda: app.VersionNumber)
    intentar("VersionBuild", lambda: app.VersionBuild)
    intentar("VersionName", lambda: app.VersionName)
    intentar("SubVersionNumber", lambda: app.SubVersionNumber)
    intentar("Product", lambda: str(app.Product))
    intentar("Language", lambda: str(app.Language))

# 2. Ruta de instalacion (ejecutable del proceso)
intentar("Revit.exe", lambda: System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
intentar("Carpeta RevitAPI.dll", lambda: clr.GetClrType(DB.Element).Assembly.Location)

# 3. Runtime .NET e IronPython
intentar("Runtime .NET", lambda: System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription)
intentar("Environment.Version", lambda: str(System.Environment.Version))
intentar("IronPython sys.version", lambda: sys.version.replace("\n", " "))
intentar("Proceso 64 bits", lambda: System.Environment.Is64BitProcess)

# 4. pyRevit
def version_pyrevit():
    from pyrevit import versionmgr
    return str(versionmgr.get_pyrevit_version().get_formatted())
intentar("pyRevit", version_pyrevit)

def motor_pyrevit():
    from pyrevit import HOST_APP
    return "version={0} build={1} motor={2}".format(HOST_APP.version, HOST_APP.build, getattr(HOST_APP, "engine_version", "?"))
intentar("pyRevit HOST_APP", motor_pyrevit)

# 5. Documento abierto y contenido estructural
intentar("Documento", lambda: doc.Title)
intentar("Ruta .rvt", lambda: doc.PathName)
intentar("Es familia", lambda: doc.IsFamilyDocument)
intentar("Unidades de longitud", lambda: str(doc.GetUnits().GetFormatOptions(DB.SpecTypeId.Length).GetUnitTypeId().TypeId))


def contar(categoria):
    return DB.FilteredElementCollector(doc).OfCategory(categoria).WhereElementIsNotElementType().GetElementCount()

intentar("Armazon estructural (instancias)", lambda: contar(DB.BuiltInCategory.OST_StructuralFraming))
intentar("Pilares estructurales (instancias)", lambda: contar(DB.BuiltInCategory.OST_StructuralColumns))
intentar("Cerchas (instancias)", lambda: contar(DB.BuiltInCategory.OST_Truss))
intentar("Conexiones estructurales (instancias)", lambda: contar(DB.BuiltInCategory.OST_StructConnections))

print("=== fin 00-version ===")
