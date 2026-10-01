# -*- coding: utf-8 -*-
# Sondeo 11: Verificacion completa de la Fase 3 (Add-in de Revit)
# Ejecuta las 12 operaciones del Bridge de MotorConexiones directamente sobre Revit
# mediante reflexion: ping, guide, types, schema, find_profile, node_info, validate,
# preview, create, list, get y delete.
#
# Se ejecuta con:
#   powershell -ExecutionPolicy Bypass -File .\scripts\revit-exec.ps1 -SinTransaccion -File scripts\sondeos\11-fase3-verificacion.py
#
# Disponibles en el contexto IronPython: doc, DB, revit, clr, System, print.
from __future__ import print_function
import os
import json

NOMBRE_ENSAMBLADO = "MotorConexiones.Revit"
RUTA_FIXTURE = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "docs", "fixtures", "detalle-D-confirmado.json"))


def buscar_ensamblado(nombre):
    for a in System.AppDomain.CurrentDomain.GetAssemblies():
        try:
            if a.GetName().Name == nombre:
                return a
        except Exception:
            pass
    return None


def llamar_bridge(metodo, operacion, cuerpo_dict_o_str):
    if isinstance(cuerpo_dict_o_str, dict):
        cuerpo = json.dumps(cuerpo_dict_o_str)
    else:
        cuerpo = str(cuerpo_dict_o_str)

    argumentos = System.Array[System.Object]([operacion, cuerpo, doc, revit.uidoc])
    respuesta_json = metodo.Invoke(None, argumentos)
    return json.loads(respuesta_json)


print("=== 11-fase3-verificacion ===")
ensamblado = buscar_ensamblado(NOMBRE_ENSAMBLADO)
if ensamblado is None:
    print("ERROR: MotorConexiones.Revit no esta cargado en Revit.")
    print("Asegurate de haber ejecutado .\\scripts\\deploy.ps1 y reiniciado Revit.")
    raise SystemExit(1)

tipo_bridge = ensamblado.GetType("MotorConexiones.Revit.Bridge")
if tipo_bridge is None:
    print("ERROR: No se encontro el tipo MotorConexiones.Revit.Bridge.")
    raise SystemExit(1)

from Autodesk.Revit.UI import UIDocument
firma = System.Array[System.Type]([clr.GetClrType(System.String), clr.GetClrType(System.String),
                                   clr.GetClrType(DB.Document), clr.GetClrType(UIDocument)])
metodo_handle = tipo_bridge.GetMethod("Handle", firma)
if metodo_handle is None:
    print("ERROR: No se encontro el metodo Handle en Bridge.")
    raise SystemExit(1)

print("[OK] Bridge encontrado en el proceso de Revit.")

# 1. PING
print("\n--- 1. conn_ping ---")
res_ping = llamar_bridge(metodo_handle, "ping", {})
print("Success: {0}".format(res_ping.get("success")))
data_ping = res_ping.get("data", {})
print("Add-in version: {0} | Active backend: {1} | Revit: {2}".format(
    data_ping.get("addin_version"), data_ping.get("active_backend"), data_ping.get("revit_version")))

# 2. GUIDE
print("\n--- 2. conn_get_guide ---")
res_guide = llamar_bridge(metodo_handle, "guide", {})
print("Success: {0} | Length: {1} chars".format(
    res_guide.get("success"), len(res_guide.get("data", {}).get("guide_markdown", ""))))

# 3. TYPES
print("\n--- 3. conn_list_types ---")
res_types = llamar_bridge(metodo_handle, "types", {})
print("Success: {0}".format(res_types.get("success")))
for t in res_types.get("data", {}).get("connection_types", []):
    print(" - {0}: {1}".format(t.get("type_name"), t.get("description")))

# 4. SCHEMA
print("\n--- 4. conn_get_schema ---")
res_schema = llamar_bridge(metodo_handle, "schema", {"type": "gusset_node"})
print("Success: {0} | Schema type: {1}".format(
    res_schema.get("success"), res_schema.get("data", {}).get("connection_type")))

# 5. FIND_PROFILE
print("\n--- 5. conn_find_profile ---")
res_prof = llamar_bridge(metodo_handle, "find_profile", {"query": "HSS3X3X1/4"})
print("Success: {0} | Matched: {1} | Total in model: {2}".format(
    res_prof.get("success"), res_prof.get("data", {}).get("matched_count"), res_prof.get("data", {}).get("total_profiles_in_model")))

# 6. NODE_INFO
print("\n--- 6. conn_get_node_info ---")
res_node = llamar_bridge(metodo_handle, "node_info", {
    "element_ids": [1249510, 1249630, 1249631, 1249636],
    "chord_element_id": 1249510
})
print("Success: {0}".format(res_node.get("success")))
if res_node.get("success"):
    nd = res_node.get("data", {})
    print("Chord: {0} ({1}) | Axes distance: {2:.2f} mm".format(
        nd.get("chord_element_id"), nd.get("chord_profile"), nd.get("axes_distance_mm", 0)))
    print("Members connected: {0}".format(len(nd.get("connected_members", []))))
else:
    print("Node info note: {0}".format(res_node.get("error", {}).get("message")))

# 7. VALIDATE
print("\n--- 7. conn_validate ---")
token = None
spec_confirmada = None
if os.path.isfile(RUTA_FIXTURE):
    with open(RUTA_FIXTURE, "r") as f:
        spec_confirmada = json.load(f)
    res_val = llamar_bridge(metodo_handle, "validate", {"spec": spec_confirmada})
    print("Success: {0}".format(res_val.get("success")))
    if res_val.get("success"):
        token = res_val.get("data", {}).get("validation_token")
        print("Validation token: {0}".format(token))
    else:
        print("Validation errors: {0}".format(res_val.get("error", {}).get("message")))
else:
    print("No se encontro el archivo fixture: {0}".format(RUTA_FIXTURE))

# 8. PREVIEW
print("\n--- 8. conn_preview ---")
if spec_confirmada:
    res_prev = llamar_bridge(metodo_handle, "preview", {"spec": spec_confirmada})
    print("Success: {0}".format(res_prev.get("success")))
    if res_prev.get("success"):
        pd = res_prev.get("data", {})
        print("Planned elements: {0} | Setbacks to apply: {1}".format(
            len(pd.get("planned_elements", [])), len(pd.get("planned_setbacks", []))))

# 9. CREATE
conn_id = None
print("\n--- 9. conn_create ---")
if spec_confirmada and token:
    res_create = llamar_bridge(metodo_handle, "create", {
        "spec": spec_confirmada,
        "validation_token": token
    })
    print("Success: {0}".format(res_create.get("success")))
    if res_create.get("success"):
        cd = res_create.get("data", {})
        conn_id = cd.get("connection_id")
        print("Created connection_id: {0} | Backend: {1}".format(conn_id, cd.get("backend")))
        print("Created elements: {0} | Modified members: {1}".format(
            len(cd.get("created_element_ids", [])), len(cd.get("modified_members", []))))
    else:
        print("Creation error: {0}".format(res_create.get("error", {}).get("message")))

# 10. LIST
print("\n--- 10. conn_list ---")
res_list = llamar_bridge(metodo_handle, "list", {})
print("Success: {0} | Connections found: {1}".format(
    res_list.get("success"), res_list.get("data", {}).get("total_connections", 0)))
for c in res_list.get("data", {}).get("connections", []):
    print(" - ID: {0} | Type: {1} | Created: {2}".format(
        c.get("connection_id"), c.get("connection_type"), c.get("created_utc")))

# 11. GET
print("\n--- 11. conn_get ---")
if conn_id:
    res_get = llamar_bridge(metodo_handle, "get", {"connection_id": conn_id})
    print("Success: {0} | ID: {1}".format(
        res_get.get("success"), res_get.get("data", {}).get("connection_id")))

# 12. DELETE
print("\n--- 12. conn_delete ---")
if conn_id:
    res_del = llamar_bridge(metodo_handle, "delete", {"connection_id": conn_id})
    print("Success: {0}".format(res_del.get("success")))
    if res_del.get("success"):
        dd = res_del.get("data", {})
        print("Deleted elements: {0} | Restored members: {1}".format(
            len(dd.get("deleted_element_ids", [])), len(dd.get("restored_members", []))))

# 13. LIST (verificar que quedo limpio)
print("\n--- 13. conn_list (post-delete) ---")
res_list2 = llamar_bridge(metodo_handle, "list", {})
print("Connections in model: {0}".format(res_list2.get("data", {}).get("total_connections", 0)))

print("\n=== fin 11-fase3-verificacion ===")
