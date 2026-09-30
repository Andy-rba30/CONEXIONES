# -*- coding: utf-8 -*-
# Sondeo 03: probar el mecanismo con el que revit_mcp/conexiones.py llamara a
# MotorConexiones.Revit.Bridge.Handle(string, string, Document, UIDocument):
#   1) localizar un ensamblado ya cargado por su nombre en AppDomain.CurrentDomain.GetAssemblies()
#   2) obtener un tipo y un metodo estatico por reflexion e invocarlo (con parametros de la API de Revit)
#   3) cargar una DLL por ruta con clr.AddReferenceToFileAndPath y llamar a un metodo estatico suyo
#   4) ver en que AssemblyLoadContext viven RevitAPI, pyRevit y los add-ins (aislamiento en .NET 10)
# Se ejecuta con: .\scripts\revit-exec.ps1 -File scripts\sondeos\03-llamar-dll.py
# Solo lee; no modifica el modelo. Disponibles: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name.lower() == nombre.lower():
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
        if ctx is None:
            return "None"
        return "{0} ({1})".format(ctx.Name, ctx.GetType().Name)
    except Exception as error:
        return "ALC no disponible: " + type(error).__name__ + ": " + str(error)[:100]


def tipos_clr(*tipos):
    return System.Array[System.Type]([clr.GetClrType(t) for t in tipos])


def objetos(*valores):
    return System.Array[System.Object](list(valores))


print("=== 03-llamar-dll ===")

# 1) Ensamblado ya cargado por nombre
revit_api = buscar_ensamblado("RevitAPI")
print("1) RevitAPI en AppDomain: " + ("SI " + str(revit_api.FullName) if revit_api else "NO"))
if revit_api is not None:
    print("   Location: " + str(revit_api.Location))
    print("   AssemblyLoadContext: " + contexto_de_carga(revit_api))

# 2) Metodo estatico por reflexion: UnitUtils.ConvertToInternalUnits(double, ForgeTypeId)
try:
    tipo_unidades = revit_api.GetType("Autodesk.Revit.DB.UnitUtils")
    metodo = tipo_unidades.GetMethod("ConvertToInternalUnits", tipos_clr(System.Double, DB.ForgeTypeId))
    resultado = metodo.Invoke(None, objetos(1000.0, DB.UnitTypeId.Millimeters))
    print("2a) UnitUtils.ConvertToInternalUnits(1000 mm) por reflexion = {0} pies (esperado 3.28084)".format(resultado))
except Exception as error:
    print("2a) ERROR " + type(error).__name__ + ": " + str(error)[:300])

# 2b) Metodo estatico que recibe un Document: Document.GetDocumentVersion(Document)
try:
    tipo_doc = clr.GetClrType(DB.Document)
    metodo = tipo_doc.GetMethod("GetDocumentVersion", tipos_clr(DB.Document))
    version = metodo.Invoke(None, objetos(doc))
    print("2b) Document.GetDocumentVersion(doc) por reflexion: GUID={0} guardados={1}".format(version.VersionGUID, version.NumberOfSaves))
except Exception as error:
    print("2b) ERROR " + type(error).__name__ + ": " + str(error)[:300])

# 2c) Metodo estatico con string y Document, como tendra Bridge.Handle (solo se comprueba la busqueda de la firma)
try:
    uidoc = revit.uidoc
    print("2c) revit.uidoc disponible: {0}; tipo: {1}".format(uidoc is not None, uidoc.GetType().FullName if uidoc is not None else "?"))
    firma = tipos_clr(System.String, System.String, DB.Document)
    print("2c) firma (String, String, Document) construida: {0}".format(", ".join(t.Name for t in firma)))
except Exception as error:
    print("2c) ERROR " + type(error).__name__ + ": " + str(error)[:300])

# 3) Cargar una DLL por ruta y llamar a un metodo estatico: RevitAddInUtility.dll
try:
    carpeta = os.path.dirname(str(revit_api.Location))
    ruta = os.path.join(carpeta, "RevitAddInUtility.dll")
    print("3) RevitAddInUtility.dll existe: {0} ({1})".format(os.path.isfile(ruta), ruta))
    previo = buscar_ensamblado("RevitAddInUtility")
    print("   ya cargado antes: {0}".format(previo is not None))
    clr.AddReferenceToFileAndPath(ruta)
    ensamblado = buscar_ensamblado("RevitAddInUtility")
    print("   cargado ahora: {0} en contexto {1}".format(ensamblado is not None, contexto_de_carga(ensamblado) if ensamblado else "?"))
    tipo = ensamblado.GetType("Autodesk.RevitAddIns.RevitProductUtility")
    metodo = tipo.GetMethod("GetAllInstalledRevitProducts")
    productos = metodo.Invoke(None, None)
    print("   RevitProductUtility.GetAllInstalledRevitProducts(): {0} productos".format(productos.Count))
    for producto in productos:
        print("     - {0} | {1} | {2}".format(producto.Name, producto.Version, producto.InstallLocation))
except Exception as error:
    print("3) ERROR " + type(error).__name__ + ": " + str(error)[:300])

# 4) Contextos de carga: RevitAPI, pyRevit, IronPython y ensamblados que vienen de carpetas de add-ins
print("4) Ensamblados fuera de la carpeta de Revit y del runtime (add-ins), con su AssemblyLoadContext:")
carpeta_revit = os.path.dirname(str(revit_api.Location)).lower() if revit_api else ""
mostrados = 0
for a in System.AppDomain.CurrentDomain.GetAssemblies():
    try:
        if a.IsDynamic:
            continue
        ubicacion = str(a.Location)
    except Exception:
        continue
    if not ubicacion:
        continue
    baja = ubicacion.lower()
    nombre = a.GetName().Name
    es_pyrevit = nombre.lower().startswith("pyrevit") or nombre.lower().startswith("ironpython") or "pyrevit" in baja
    fuera = not baja.startswith(carpeta_revit) and "\\dotnet\\" not in baja and "\\microsoft.net\\" not in baja
    if es_pyrevit or fuera:
        if mostrados >= 40:
            print("   ... (hay mas)")
            break
        mostrados += 1
        print("   {0} | {1} | {2}".format(nombre, contexto_de_carga(a), ubicacion))
print("   total ensamblados en AppDomain: {0}".format(len(list(System.AppDomain.CurrentDomain.GetAssemblies()))))

# 5) Extensible Storage: que nombres de esquema acepta Revit (para la Fase 3, sin tocar el documento)
try:
    constructor = DB.ExtensibleStorage.SchemaBuilder(System.Guid("7d5a2f7e-6c8b-4b9e-9c1e-000000000001"))
    for nombre_esquema in ("MotorConexiones.Connection", "MotorConexionesConnection", "MotorConexiones_Connection"):
        print("5) SchemaBuilder.AcceptableName('{0}') = {1}".format(nombre_esquema, constructor.AcceptableName(nombre_esquema)))
    constructor.Dispose()
except Exception as error:
    print("5) ERROR " + type(error).__name__ + ": " + str(error)[:300])

print("=== fin 03-llamar-dll ===")
