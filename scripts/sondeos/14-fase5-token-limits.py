# -*- coding: utf-8 -*-
# Sondeo 14 (Fase 5): comprueba, por reflexion sobre Bridge.Handle (como el sondeo 11) y SIN CREAR NADA:
#   1) que la DLL desplegada es la de la Fase 5: conn_ping ya no lista probe_plate_b ni probe_delete_b, y el Core
#      desplegado tiene LimitsConfig.ComputeHash (el hash de config\limits.json que entra en el validation_token);
#   2) que el config\limits.json desplegado junto a la DLL es el del repositorio (mismo hash);
#   3) que validate del fixture confirmado da token (64 hex) y que validar dos veces da el mismo token;
#   4) que create con un token alterado, y con un token calculado con OTRO limits.json, se rechaza con
#      VALIDATION_TOKEN_INVALID sin tocar el modelo (conn_list igual, extensiones de las barras iguales).
# Solo sobre la copia "_sondeo.rvt". Se ejecuta SIN transaccion envolvente (cada operacion abre la suya):
#   .\scripts\revit-exec.ps1 -File scripts\sondeos\14-fase5-token-limits.py -SinTransaccion -TimeoutSec 600
# Disponibles: doc, uidoc, uiapp, DB, UI, revit, clr, System, print.
from __future__ import print_function
import json
import os
import time

RUTA_FIXTURE = r"D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json"
RUTA_LIMITS_REPO = r"D:\Proyectos C#\CONEXIONES\config\limits.json"
MM_POR_PIE = 304.8
HEX = "0123456789abcdef"

comprobaciones = []
no_probadas = []


def comprobar(nombre, ok, detalle=""):
    comprobaciones.append(bool(ok))
    print("[{0}] {1}{2}".format("OK" if ok else "FALLO", nombre, (": " + str(detalle)) if detalle else ""))


def no_probada(nombre, motivo):
    no_probadas.append(nombre)
    print("[NO PROBADO] {0}: {1}".format(nombre, motivo))


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def tipo_clr(t):
    return clr.GetClrType(t)


def es_token(valor):
    return isinstance(valor, basestring) and len(valor) == 64 and all(c in HEX for c in valor.lower())


print("=== 14-fase5-token-limits ===")
if not doc.PathName.lower().endswith("_sondeo.rvt"):
    print("PARADA: el documento abierto no es la copia '_sondeo.rvt' ({0}).".format(doc.PathName))
    raise SystemExit

ens_revit = buscar_ensamblado("MotorConexiones.Revit")
ens_core = buscar_ensamblado("MotorConexiones.Core")
if ens_revit is None or ens_core is None:
    print("PARADA: MotorConexiones.Revit o MotorConexiones.Core no estan cargados. Ejecuta scripts\\deploy.ps1 con Revit cerrado y vuelve a abrir Revit.")
    raise SystemExit
firma = System.Array[System.Type]([tipo_clr(System.String), tipo_clr(System.String), tipo_clr(DB.Document), tipo_clr(UI.UIDocument)])
handle = ens_revit.GetType("MotorConexiones.Revit.Bridge").GetMethod("Handle", firma)
print("Bridge.Handle encontrado: {0} | Revit.dll {1} | Core.dll {2}".format(
    handle is not None, ens_revit.GetName().Version, ens_core.GetName().Version))
print("DLL desplegada: " + str(ens_revit.Location))


def llamar(operacion, cuerpo):
    inicio = time.time()
    argumentos = System.Array[System.Object]([operacion, json.dumps(cuerpo), doc, uidoc])
    try:
        texto = handle.Invoke(None, argumentos)
        respuesta = json.loads(texto)
    except Exception as error:
        interna = getattr(error, "InnerException", None)
        print("--- {0}: EXCEPCION {1}: {2}".format(operacion, type(error).__name__, str(interna or error)[:400]))
        return {"ok": False, "data": None, "errors": [{"code": "EXCEPTION", "message": str(error)[:400]}], "warnings": []}
    errores = respuesta.get("errors") or []
    avisos = respuesta.get("warnings") or []
    print("--- {0}: ok={1} en {2} ms | errores={3} | avisos={4}".format(
        operacion, respuesta.get("ok"), int((time.time() - inicio) * 1000),
        ", ".join(str(e.get("code")) for e in errores) or "-",
        ", ".join(str(w.get("code")) for w in avisos) or "-"))
    for e in errores:
        print("    ERROR {0} [{1}]: {2}".format(e.get("code"), e.get("path"), str(e.get("message"))[:300]))
    return respuesta


def extensiones(ids):
    """Extension de inicio y fin (mm, redondeadas) de cada barra: es lo que create modifica y delete restaura."""
    salida = {}
    for eid in ids:
        el = doc.GetElement(DB.ElementId(System.Int64(int(eid))))
        if el is None:
            salida[int(eid)] = None
            continue
        p0 = el.get_Parameter(DB.BuiltInParameter.START_EXTENSION)
        p1 = el.get_Parameter(DB.BuiltInParameter.END_EXTENSION)
        salida[int(eid)] = (round(p0.AsDouble() * MM_POR_PIE, 3) if p0 else None,
                            round(p1.AsDouble() * MM_POR_PIE, 3) if p1 else None)
    return salida


# 1) DLL de la Fase 5: ping sin probe_* y Core con LimitsConfig.ComputeHash
r = llamar("ping", {})
datos = r.get("data") or {}
operaciones = datos.get("operations") or []
print("    operations: " + ", ".join(operaciones))
comprobar("ping ok y sin operaciones probe_*", r.get("ok") and not [o for o in operaciones if str(o).startswith("probe_")])
comprobar("ping con 13 operaciones", len(operaciones) == 13, len(operaciones))
comprobar("documento de ping es la copia _sondeo", (datos.get("document") or {}).get("title") == "HANGAR_PRUEBA_sondeo",
          (datos.get("document") or {}).get("title"))
tipo_limits = ens_core.GetType("MotorConexiones.Core.Validation.LimitsConfig")
metodo_hash = tipo_limits.GetMethod("ComputeHash") if tipo_limits is not None else None
comprobar("Core desplegado tiene LimitsConfig.ComputeHash (Fase 5)", metodo_hash is not None)

# 2) limits.json desplegado junto a la DLL = limits.json del repositorio (mismo hash)
ruta_limits_desplegado = os.path.join(os.path.dirname(str(ens_revit.Location)), "config", "limits.json")
cfg_desplegado = None
hash_desplegado = None
hash_repo = None
if metodo_hash is not None:
    try:
        cargar = tipo_limits.GetMethod("LoadFromFile", System.Array[System.Type]([tipo_clr(System.String)]))
        cfg_desplegado = cargar.Invoke(None, System.Array[System.Object]([ruta_limits_desplegado]))
        hash_desplegado = metodo_hash.Invoke(cfg_desplegado, None)
        cfg_repo = cargar.Invoke(None, System.Array[System.Object]([RUTA_LIMITS_REPO]))
        hash_repo = metodo_hash.Invoke(cfg_repo, None)
    except Exception as error:
        print("    reflexion sobre LimitsConfig: {0}: {1}".format(type(error).__name__, str(error)[:300]))
print("    limits.json desplegado: {0} (existe={1})".format(ruta_limits_desplegado, os.path.isfile(ruta_limits_desplegado)))
print("    hash desplegado={0}".format(hash_desplegado))
print("    hash repositorio={0}".format(hash_repo))
comprobar("limits.json desplegado existe y tiene el mismo hash que el del repositorio",
          os.path.isfile(ruta_limits_desplegado) and hash_desplegado is not None and hash_desplegado == hash_repo)

# 3) estado del modelo antes de nada
if not os.path.isfile(RUTA_FIXTURE):
    print("PARADA: no existe el fixture " + RUTA_FIXTURE)
    raise SystemExit
with open(RUTA_FIXTURE, "r") as archivo:
    spec = json.load(archivo)
ids_fixture = [int(i) for i in spec.get("node", {}).get("element_ids", [])]
r = llamar("list", {})
conexiones_antes = (r.get("data") or {}).get("connections_count")
ext_antes = extensiones(ids_fixture)
print("    conexiones antes: {0}".format(conexiones_antes))
for eid in ids_fixture:
    print("    barra {0}: extension inicio/fin (mm) = {1}".format(eid, ext_antes.get(eid)))

# 4) validate dos veces: token de 64 hex y determinista
r1 = llamar("validate", {"spec": spec})
token1 = (r1.get("data") or {}).get("validation_token")
r2 = llamar("validate", {"spec": spec})
token2 = (r2.get("data") or {}).get("validation_token")
print("    token 1: {0}".format(token1))
print("    token 2: {0}".format(token2))
comprobar("validate del fixture confirmado: is_valid con token de 64 hex", r1.get("ok") and es_token(token1))
comprobar("validar dos veces da el mismo token (determinista)", es_token(token1) and token1 == token2)

# 5) create con el token alterado (un caracter cambiado) -> VALIDATION_TOKEN_INVALID
if es_token(token1):
    ultimo = token1[-1]
    alterado = token1[:-1] + ("0" if ultimo != "0" else "1")
    r = llamar("create", {"spec": spec, "validation_token": alterado})
    codigos = [str(e.get("code")) for e in r.get("errors") or []]
    comprobar("create con token alterado -> VALIDATION_TOKEN_INVALID", (not r.get("ok")) and codigos == ["VALIDATION_TOKEN_INVALID"], ",".join(codigos))
else:
    no_probada("create con token alterado", "validate no dio token")

# 6) create con un token calculado con OTRO limits.json (tolerancia de cotas 2,5 mm) usando el Core desplegado
nombre6 = "create con token de otro limits.json -> VALIDATION_TOKEN_INVALID"
nombre6b = "token de validate == token calculado con el limits.json desplegado"
if es_token(token1) and cfg_desplegado is not None:
    try:
        tipo_spec = ens_core.GetType("MotorConexiones.Core.Contract.ConnectionSpec")
        tipo_facts_interfaz = ens_core.GetType("MotorConexiones.Core.Model.IModelFacts")
        tipo_generador = ens_core.GetType("MotorConexiones.Core.Validation.ValidationTokenGenerator")
        tipo_facts = ens_revit.GetType("MotorConexiones.Revit.Node.RevitModelFacts")
        de_json = tipo_spec.GetMethod("FromJson", System.Array[System.Type]([tipo_clr(System.String)]))
        spec_obj = de_json.Invoke(None, System.Array[System.Object]([json.dumps(spec)]))
        cargar_json = tipo_limits.GetMethod("LoadFromJson", System.Array[System.Type]([tipo_clr(System.String)]))
        otros_limites = cargar_json.Invoke(None, System.Array[System.Object](['{"schema_version":1,"dimension_chain_tolerance_mm":2.5}']))
        facts = tipo_facts.GetConstructors()[0].Invoke(System.Array[System.Object]([doc, None, None]))
        generar = tipo_generador.GetMethod("GenerateToken", System.Array[System.Type]([tipo_spec, tipo_facts_interfaz, tipo_limits]))
        token_otro = generar.Invoke(None, System.Array[System.Object]([spec_obj, facts, otros_limites]))
        token_mismo = generar.Invoke(None, System.Array[System.Object]([spec_obj, facts, cfg_desplegado]))
        print("    token con otro limits.json: {0}".format(token_otro))
        print("    token con el limits.json desplegado: {0}".format(token_mismo))
        comprobar("el token cambia al cambiar limits.json", es_token(token_otro) and token_otro.lower() != token1.lower())
        comprobar(nombre6b, es_token(token_mismo) and token_mismo.lower() == token1.lower())
        r = llamar("create", {"spec": spec, "validation_token": token_otro})
        codigos = [str(e.get("code")) for e in r.get("errors") or []]
        comprobar(nombre6, (not r.get("ok")) and codigos == ["VALIDATION_TOKEN_INVALID"], ",".join(codigos))
    except Exception as error:
        interna = getattr(error, "InnerException", None)
        no_probada(nombre6, "reflexion fallida: {0}: {1}".format(type(error).__name__, str(interna or error)[:300]))
else:
    no_probada(nombre6, "sin token de validate o sin LimitsConfig desplegado")

# 7) el modelo no cambio
r = llamar("list", {})
conexiones_despues = (r.get("data") or {}).get("connections_count")
ext_despues = extensiones(ids_fixture)
comprobar("conn_list igual que antes (nada creado)", conexiones_antes == conexiones_despues, "{0} -> {1}".format(conexiones_antes, conexiones_despues))
comprobar("extensiones de las barras sin cambios", ext_antes == ext_despues, ext_despues)
comprobar("documento sin transaccion abierta", not doc.IsModifiable)

print("RESULTADO: {0}/{1} comprobaciones correctas{2}".format(
    sum(1 for c in comprobaciones if c), len(comprobaciones),
    " ({0} no probadas: {1})".format(len(no_probadas), "; ".join(no_probadas)) if no_probadas else ""))
print("=== fin 14-fase5-token-limits ===")
