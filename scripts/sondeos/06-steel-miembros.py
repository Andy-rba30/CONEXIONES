# -*- coding: utf-8 -*-
# Sondeo 06: que ofrece la API de acero de Revit 2027 para crear placas y pernos (camino A), SIN crear nada.
#   1) Todos los tipos de RevitAPISteel.dll (los 24 de Autodesk.Revit.DB.Steel + resto) y los miembros publicos
#      de los que pueden abrir el "contexto de acero": SteelModelManager, SteelConnectionUtil,
#      StructuralConnectionBaseUtil, SteelProxyElement, SteelElementProperties y cualquier *Transaction*.
#   2) Tipos de Autodesk.SteelConnectionsDB.dll relacionados con Transaction/Fabrication/Plate/Bolt.
#   3) Archivos de C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\ (ahi viven AS*Mgd.dll).
#   4) Carga por ruta de ASObjectsMgd.dll y ASGeometryMgd.dll: constructores de Plate, de los patrones de pernos
#      (*BoltPattern*) y de Plane/Point3d/Vector3d, y metodos WriteToDb.
#   5) Si hay SDK de Revit 2027 instalado con ejemplos de acero.
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\06-steel-miembros.py
# Solo lee; no modifica el modelo ni crea objetos de Advance Steel. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os
import re

MAX_MIEMBROS = 45
MAX_TIPOS = 60
CARPETA_REVIT = os.path.dirname(str(clr.GetClrType(DB.Element).Assembly.Location))
CARPETA_STEEL = os.path.join(CARPETA_REVIT, "AddIns", "SteelConnections")


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name.lower() == nombre.lower():
                return a
        except Exception:
            pass
    return None


def tipos_de(ensamblado):
    try:
        return list(ensamblado.GetExportedTypes())
    except Exception as error:
        tipos = getattr(getattr(error, "clsException", None), "Types", None)
        if tipos is not None:
            return [t for t in tipos if t is not None]
        print("    no se pudieron enumerar los tipos: " + str(error)[:200])
        return []


def firma_parametros(metodo):
    try:
        return ", ".join("{0} {1}".format(p.ParameterType.Name, p.Name) for p in metodo.GetParameters())
    except Exception:
        return "?"


def volcar_miembros(tipo, maximo=MAX_MIEMBROS, solo_patron=None):
    """Constructores, metodos y propiedades publicos (declarados en el tipo)."""
    from System.Reflection import BindingFlags
    flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
    print("  --- {0} ({1}{2})".format(tipo.FullName, "clase" if tipo.IsClass else "otro",
                                       ", abstracta/estatica" if tipo.IsAbstract else ""))
    try:
        base = tipo.BaseType
        print("      base: {0}".format(base.FullName if base is not None else "-"))
    except Exception:
        pass
    mostrados = 0
    try:
        for c in tipo.GetConstructors():
            print("      ctor({0})".format(firma_parametros(c)))
            mostrados += 1
    except Exception as error:
        print("      ctors ERROR " + str(error)[:100])
    try:
        for m in sorted(tipo.GetMethods(flags), key=lambda x: x.Name):
            if m.IsSpecialName:
                continue
            if solo_patron is not None and not solo_patron.search(m.Name):
                continue
            if mostrados >= maximo:
                print("      ... (mas miembros)")
                break
            print("      {0}{1} {2}({3})".format("static " if m.IsStatic else "", m.ReturnType.Name, m.Name, firma_parametros(m)))
            mostrados += 1
    except Exception as error:
        print("      metodos ERROR " + str(error)[:100])
    try:
        props = [p.Name + ":" + p.PropertyType.Name for p in tipo.GetProperties(flags)]
        if props:
            print("      props: " + ", ".join(sorted(props)[:30]))
    except Exception:
        pass


print("=== 06-steel-miembros ===")

# 1) RevitAPISteel.dll completo
patron_contexto = re.compile(r"Transaction|SteelModelManager|SteelConnectionUtil|StructuralConnectionBaseUtil|SteelProxyElement|SteelModelInfo|Fabrication", re.IGNORECASE)
steel = buscar_ensamblado("RevitAPISteel")
if steel is None:
    ruta = os.path.join(CARPETA_REVIT, "RevitAPISteel.dll")
    print("1) RevitAPISteel no estaba cargado; se carga desde " + ruta)
    try:
        clr.AddReferenceToFileAndPath(ruta)
        steel = buscar_ensamblado("RevitAPISteel")
    except Exception as error:
        print("   ERROR " + str(error)[:200])
if steel is not None:
    tipos = sorted(tipos_de(steel), key=lambda t: t.FullName)
    print("1) RevitAPISteel.dll: {0} tipos publicos (todos):".format(len(tipos)))
    for t in tipos:
        print("   " + t.FullName + ("  [enum]" if t.IsEnum else "") + ("  [interfaz]" if t.IsInterface else ""))
    print("1b) Miembros de los tipos que pueden abrir el contexto de acero:")
    for t in tipos:
        if patron_contexto.search(t.Name) and not t.IsEnum:
            volcar_miembros(t)
else:
    print("1) RevitAPISteel.dll no disponible")

# 1c) SteelElementProperties (RevitAPI.dll)
try:
    volcar_miembros(clr.GetClrType(DB.Steel.SteelElementProperties))
except Exception as error:
    print("1c) SteelElementProperties ERROR " + str(error)[:150])

# 2) Autodesk.SteelConnectionsDB.dll
patron_interes = re.compile(r"Transaction|Fabrication|Plate|Bolt|Weld|Context|Session|Anchor|Shear|Contour|Cut|Cope", re.IGNORECASE)
scdb = buscar_ensamblado("Autodesk.SteelConnectionsDB")
if scdb is None:
    print("2) Autodesk.SteelConnectionsDB no cargado")
else:
    tipos = sorted(tipos_de(scdb), key=lambda t: t.FullName)
    interesantes = [t for t in tipos if patron_interes.search(t.Name)]
    print("2) Autodesk.SteelConnectionsDB.dll: {0} tipos publicos; {1} de interes (max {2}):".format(len(tipos), len(interesantes), MAX_TIPOS))
    for t in interesantes[:MAX_TIPOS]:
        print("   " + t.FullName)
    print("2b) Miembros de los tipos con Transaction/Fabrication/Context/Session:")
    for t in interesantes:
        if re.search(r"Transaction|Fabrication|Context|Session", t.Name, re.IGNORECASE) and not t.IsEnum:
            volcar_miembros(t, maximo=25)

# 3) Carpeta AddIns\SteelConnections
print("3) Carpeta {0}: existe={1}".format(CARPETA_STEEL, os.path.isdir(CARPETA_STEEL)))
if os.path.isdir(CARPETA_STEEL):
    archivos = sorted(f for f in os.listdir(CARPETA_STEEL) if f.lower().endswith(".dll"))
    print("   DLL ({0}):".format(len(archivos)))
    for f in archivos:
        try:
            tam = os.path.getsize(os.path.join(CARPETA_STEEL, f)) // 1024
        except Exception:
            tam = "?"
        cargado = buscar_ensamblado(os.path.splitext(f)[0]) is not None
        print("     {0} ({1} KB){2}".format(f, tam, "  [cargado]" if cargado else ""))

# 4) Ensamblados de Advance Steel por ruta (solo reflexion, sin crear objetos)
patron_as = re.compile(r"Plate|Bolt|Screw|Weld|Anchor|Pattern|Feature|Contour|Cut", re.IGNORECASE)
for nombre_dll in ("ASGeometryMgd.dll", "ASObjectsMgd.dll", "ASDocumentMgd.dll", "ASCADLinkMgd.dll", "ASDatabaseMgd.dll"):
    ruta = os.path.join(CARPETA_STEEL, nombre_dll)
    base = os.path.splitext(nombre_dll)[0]
    print("4) {0}: existe={1}".format(nombre_dll, os.path.isfile(ruta)))
    if not os.path.isfile(ruta):
        continue
    ensamblado = buscar_ensamblado(base)
    if ensamblado is None:
        try:
            clr.AddReferenceToFileAndPath(ruta)
            ensamblado = buscar_ensamblado(base)
            print("   cargado ahora: {0}".format(ensamblado is not None))
        except Exception as error:
            print("   AddReferenceToFileAndPath ERROR {0}: {1}".format(type(error).__name__, str(error)[:200]))
            continue
    else:
        print("   ya estaba cargado")
    if ensamblado is None:
        continue
    tipos = sorted(tipos_de(ensamblado), key=lambda t: t.FullName)
    espacios = {}
    for t in tipos:
        espacios[t.Namespace or "-"] = espacios.get(t.Namespace or "-", 0) + 1
    print("   tipos publicos: {0}; espacios: {1}".format(len(tipos), ", ".join("{0}={1}".format(k, v) for k, v in sorted(espacios.items())[:12])))
    if base == "ASGeometryMgd":
        for nombre_tipo in ("Autodesk.AdvanceSteel.Geometry.Point3d", "Autodesk.AdvanceSteel.Geometry.Vector3d",
                            "Autodesk.AdvanceSteel.Geometry.Plane", "Autodesk.AdvanceSteel.Geometry.Matrix3d"):
            t = ensamblado.GetType(nombre_tipo)
            if t is not None:
                volcar_miembros(t, maximo=12, solo_patron=re.compile(r"^(Is|Get|Set|Transform|Normal|Origin)"))
            else:
                print("   no existe " + nombre_tipo)
    elif base == "ASObjectsMgd":
        interesantes = [t for t in tipos if patron_as.search(t.Name)]
        print("   tipos de interes ({0}, max {1}):".format(len(interesantes), MAX_TIPOS))
        for t in interesantes[:MAX_TIPOS]:
            print("     " + t.FullName)
        for nombre_tipo in ("Autodesk.AdvanceSteel.Modelling.Plate", "Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern",
                            "Autodesk.AdvanceSteel.Modelling.InfinitRectScrewBoltPattern", "Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern",
                            "Autodesk.AdvanceSteel.Modelling.BoltPattern", "Autodesk.AdvanceSteel.CADAccess.FilerObject"):
            t = ensamblado.GetType(nombre_tipo)
            if t is not None:
                volcar_miembros(t, maximo=40)
            else:
                print("   no existe " + nombre_tipo)
    else:
        interesantes = [t for t in tipos if re.search(r"Transaction|Document|Session|Lock|Manager|Db", t.Name)]
        print("   tipos con Transaction/Document/Session/Lock/Manager ({0}, max 30):".format(len(interesantes)))
        for t in interesantes[:30]:
            print("     " + t.FullName)
        for t in interesantes:
            if re.search(r"Transaction|DocumentManager|Session", t.Name) and not t.IsEnum:
                volcar_miembros(t, maximo=25)

# 5) SDK de Revit con ejemplos de acero
candidatos = [os.path.join(CARPETA_REVIT, "SDK"), "C:\\Revit 2027 SDK", "C:\\Revit SDK 2027",
              os.path.join(os.environ.get("USERPROFILE", ""), "Documents", "Revit 2027 SDK"),
              os.path.join(os.environ.get("USERPROFILE", ""), "Desktop", "Revit 2027 SDK"),
              os.path.join(os.environ.get("USERPROFILE", ""), "Downloads", "Revit 2027 SDK")]
print("5) SDK de Revit 2027:")
alguno = False
for c in candidatos:
    if os.path.isdir(c):
        alguno = True
        print("   existe " + c)
        for raiz, carpetas, archivos in os.walk(c):
            if raiz.count(os.sep) - c.count(os.sep) > 3:
                continue
            if re.search(r"steel", os.path.basename(raiz), re.IGNORECASE):
                print("     carpeta de acero: " + raiz)
            for f in archivos:
                if f.lower().endswith(".cs") and re.search(r"plate|bolt|steel|fabrication", f, re.IGNORECASE):
                    print("     " + os.path.join(raiz, f))
if not alguno:
    print("   no encontrado en las rutas habituales (opcional: descargar 'Revit 2027 SDK' de Autodesk Developer Network)")

print("=== fin 06-steel-miembros ===")
