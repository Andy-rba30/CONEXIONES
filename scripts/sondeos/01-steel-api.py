# -*- coding: utf-8 -*-
# Sondeo 01: que API de acero hay disponible en este Revit.
#   a) espacio de nombres Autodesk.Revit.DB.Steel dentro de RevitAPI.dll (si existe)
#   b) DLL de acero en la carpeta de Revit (RevitAPISteel.dll, AS*Mgd.dll, *Steel*.dll): se intentan cargar con
#      clr.AddReferenceToFileAndPath y se listan sus espacios de nombres y algunas clases (placas, pernos, soldaduras...)
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\01-steel-api.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os
import re

PATRON_INTERES = re.compile(r"Plate|Bolt|Weld|Connection|Fabrication|Coping|Cut|Anchor|Shear|Contour|Steel", re.IGNORECASE)
MAX_TIPOS = 40
MAX_ESPACIOS = 20


def tipos_de(ensamblado):
    """Tipos publicos del ensamblado, tolerando los que no se puedan cargar."""
    try:
        return list(ensamblado.GetExportedTypes())
    except Exception as error:
        cls = getattr(error, "clsException", None)
        tipos = getattr(cls, "Types", None)
        if tipos is not None:
            return [t for t in tipos if t is not None]
        print("    no se pudieron enumerar los tipos: " + type(error).__name__ + ": " + str(error)[:200])
        return []


def resumir(ensamblado):
    tipos = tipos_de(ensamblado)
    print("    tipos publicos: {0}".format(len(tipos)))
    espacios = {}
    for t in tipos:
        espacio = t.Namespace or "(sin espacio)"
        espacios[espacio] = espacios.get(espacio, 0) + 1
    ordenados = sorted(espacios.items(), key=lambda par: -par[1])
    print("    espacios de nombres (top {0}):".format(MAX_ESPACIOS))
    for espacio, cuenta in ordenados[:MAX_ESPACIOS]:
        print("      {0}: {1}".format(espacio, cuenta))
    interesantes = sorted(t.FullName for t in tipos if PATRON_INTERES.search(t.Name or ""))
    print("    tipos de interes ({0}, se muestran {1}):".format(len(interesantes), min(len(interesantes), MAX_TIPOS)))
    for nombre in interesantes[:MAX_TIPOS]:
        print("      " + nombre)


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name.lower() == nombre.lower():
                return a
        except Exception:
            pass
    return None


print("=== 01-steel-api ===")

# a) Autodesk.Revit.DB.Steel dentro de RevitAPI.dll
revit_api = clr.GetClrType(DB.Element).Assembly
print("RevitAPI.dll: " + str(revit_api.Location))
tipos_steel = [t for t in tipos_de(revit_api) if (t.Namespace or "").startswith("Autodesk.Revit.DB.Steel")]
print("Tipos en Autodesk.Revit.DB.Steel* (RevitAPI.dll): {0}".format(len(tipos_steel)))
for t in sorted(t.FullName for t in tipos_steel)[:MAX_TIPOS]:
    print("  " + t)
print("hasattr(DB, 'Steel'): {0}".format(hasattr(DB, "Steel")))
tipos_conn = [t for t in tipos_de(revit_api) if "StructuralConnection" in (t.Name or "")]
print("Tipos con 'StructuralConnection' en RevitAPI.dll: {0}".format(len(tipos_conn)))
for t in sorted(t.FullName for t in tipos_conn)[:MAX_TIPOS]:
    print("  " + t)

# b) DLL de acero en la carpeta de Revit
carpeta = os.path.dirname(str(revit_api.Location))
print("Carpeta de Revit: " + carpeta)
candidatos = []
for patron in ("RevitAPISteel.dll", "*Steel*.dll", "AS*Mgd*.dll", "AS*.dll", "*AdvanceSteel*.dll"):
    try:
        for ruta in System.IO.Directory.GetFiles(carpeta, patron):
            if ruta not in candidatos:
                candidatos.append(ruta)
    except Exception as error:
        print("  no se pudo listar " + patron + ": " + str(error)[:120])
print("DLL candidatas en la carpeta de Revit: {0}".format(len(candidatos)))
for ruta in candidatos:
    try:
        tamano = System.IO.FileInfo(ruta).Length // 1024
    except Exception:
        tamano = "?"
    print("  {0} ({1} KB)".format(os.path.basename(ruta), tamano))

print("Intentos de carga (maximo 12 DLL):")
for ruta in candidatos[:12]:
    nombre = os.path.splitext(os.path.basename(ruta))[0]
    print("- " + nombre)
    ya = buscar_ensamblado(nombre)
    if ya is not None:
        print("    ya estaba cargado: " + str(ya.FullName))
        resumir(ya)
        continue
    try:
        clr.AddReferenceToFileAndPath(ruta)
    except Exception as error:
        print("    AddReferenceToFileAndPath ERROR " + type(error).__name__ + ": " + str(error)[:200])
        continue
    cargado = buscar_ensamblado(nombre)
    if cargado is None:
        print("    cargado, pero no aparece en AppDomain con ese nombre")
        continue
    print("    cargado: " + str(cargado.FullName))
    resumir(cargado)

print("=== fin 01-steel-api ===")
