#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Pruebas de humo de las rutas /conn/ de MotorConexiones (CPython 3, solo httpx y stdlib).

Requisitos: Revit 2027 abierto con la copia HANGAR_PRUEBA_sondeo.rvt (los IDs del fixture son de ese
modelo), pyRevit con Routes activo (48884), la extensión revit-mcp con conexiones.py instalado y
recargado (mcp\\instalar-conn.ps1) y el add-in MotorConexiones desplegado (scripts\\deploy.ps1).
Habla directamente con Revit por HTTP con el token; no necesita el puente main.py salvo con --puente.

NO CREA NADA EN EL MODELO: las llamadas a create y delete van sin token o con un ID inexistente, y el
add-in las rechaza antes de tocar nada. Se puede ejecutar sobre cualquier modelo sin riesgo.

Uso (con el Python del .venv de la extensión, desde la raíz de CONEXIONES):
    C:\\IA\\pyrevit-ext\\mcp-server-for-revit-python.extension\\.venv\\Scripts\\python.exe mcp\\pruebas\\probar_conexiones.py
    ... --puente        añade dos pruebas contra el puente MCP (http://127.0.0.1:8000/mcp, arrancado con
                        "python main.py --streamable-http" o "--combined"): tools/list debe traer las 18
                        herramientas conn_* y tools/call conn_ping debe devolver ok:true.
    ... --fixtures <carpeta>   carpeta con detalle-D-confirmado.json y detalle-D.json (por defecto docs\\fixtures).

Pruebas (Fase 4): 401 sin token; ping; guía; tipos; esquema (gusset_node y uno inexistente); find_profile;
node_info del nudo del fixture; validate del Detalle D con dudas confirmadas (sin errores, con token);
validate con 420->402 (DIMENSION_CHAIN_MISMATCH, sin token); validate con dudas sin confirmar
(UNRESOLVED_UNCERTAINTY); preview; create sin token (VALIDATION_TOKEN_INVALID); list; get y delete de un ID
inexistente (ELEMENT_NOT_FOUND); operación inexistente por la ruta genérica (UNKNOWN_OPERATION).
Catálogo (Fase 7, pruebas 18 a 23): catalog_list; catalog_save desde el fixture (crea la plantilla "PRUEBA probar_conexiones"
en la carpeta del catálogo del PC, y la borra al final); catalog_get; catalog_apply al mismo nudo (token, orientación same);
catalog_apply con una plantilla inexistente (TEMPLATE_NOT_FOUND); catalog_delete.
Termina con "Resultado: N/N pruebas correctas" y código de salida 0 si todas pasan.
"""
import argparse
import copy
import json
import os
import re
import sys
from pathlib import Path

import httpx

REVIT_POR_DEFECTO = "http://127.0.0.1:48884/revit_mcp"
PUENTE_POR_DEFECTO = "http://127.0.0.1:8000/mcp"
RUTA_TOKEN_POR_DEFECTO = os.path.expandvars(r"%LOCALAPPDATA%\RevitMcp\token")
FIXTURES_POR_DEFECTO = Path(__file__).resolve().parents[2] / "docs" / "fixtures"
HERRAMIENTAS_CONN = [
    "conn_ping", "conn_get_guide", "conn_list_types", "conn_get_schema", "conn_get_node_info",
    "conn_find_profile", "conn_validate", "conn_preview", "conn_create", "conn_list", "conn_get",
    "conn_update", "conn_delete",
    "conn_catalog_list", "conn_catalog_get", "conn_catalog_save", "conn_catalog_delete", "conn_catalog_apply",
]
NOMBRE_PLANTILLA_PRUEBA = "PRUEBA probar_conexiones"
MAXIMO_CUERPO = 1500


def leer_token(ruta):
    try:
        with open(ruta, "r", encoding="utf-8") as archivo:
            return archivo.read().strip() or None
    except OSError:
        return None


def leer_fixture(carpeta, nombre):
    ruta = Path(carpeta) / nombre
    try:
        with open(ruta, "r", encoding="utf-8") as archivo:
            return json.load(archivo), None
    except (OSError, ValueError) as error:
        return None, "no se pudo leer {}: {}".format(ruta, error)


class Pruebas:
    def __init__(self, revit, token, cliente):
        self.revit = revit
        self.token = token
        self.cliente = cliente
        self.resultados = []

    # --- utilidades ---------------------------------------------------------------------------------
    def get(self, ruta, con_token=True):
        params = {"token": self.token} if con_token and self.token else None
        return self.cliente.get(self.revit + ruta, params=params)

    def post(self, ruta, cuerpo, con_token=True):
        datos = dict(cuerpo)
        if con_token and self.token:
            datos["token"] = self.token
        return self.cliente.post(self.revit + ruta, json=datos)

    def anotar(self, nombre, ok, detalle="", cuerpo=""):
        self.resultados.append(bool(ok))
        print("=" * 70)
        print("{}  [{}]{}".format(nombre, "OK" if ok else "FALLO", "  " + detalle if detalle else ""))
        if cuerpo:
            print("Cuerpo:")
            print(cuerpo if len(cuerpo) <= MAXIMO_CUERPO else cuerpo[:MAXIMO_CUERPO] + " ...")
        return ok

    def comprobar_sobre(self, nombre, respuesta, esperado_ok, comprobar=None, codigo_esperado=None):
        """HTTP 200, sobre común con ok == esperado_ok, y comprobaciones extra sobre data/errors."""
        detalle = "HTTP {}".format(respuesta.status_code)
        cuerpo = None
        ok = respuesta.status_code == 200
        if ok:
            try:
                cuerpo = respuesta.json()
            except ValueError:
                cuerpo = None
            if not isinstance(cuerpo, dict) or not {"ok", "data", "errors", "warnings", "meta"} <= set(cuerpo):
                ok = False
                detalle += ", la respuesta no es el sobre común {ok, data, errors, warnings, meta}"
            elif cuerpo.get("ok") is not esperado_ok:
                ok = False
                codigos = [e.get("code") for e in cuerpo.get("errors") or []]
                detalle += ", ok={} (se esperaba {}) errores={}".format(cuerpo.get("ok"), esperado_ok, codigos)
            else:
                codigos = [e.get("code") for e in cuerpo.get("errors") or []]
                avisos = [w.get("code") for w in cuerpo.get("warnings") or []]
                detalle += ", ok={}".format(cuerpo.get("ok"))
                if codigos:
                    detalle += ", errores={}".format(codigos)
                if avisos:
                    detalle += ", avisos={}".format(avisos)
                if codigo_esperado and codigo_esperado not in codigos:
                    ok = False
                    detalle += ", falta el código {}".format(codigo_esperado)
                if ok and comprobar is not None:
                    try:
                        ok, extra = comprobar(cuerpo)
                        if extra:
                            detalle += ", " + extra
                    except Exception as error:  # noqa: BLE001
                        ok, detalle = False, detalle + ", excepción al comprobar: {}".format(error)
        self.anotar(nombre, ok, detalle, respuesta.text)
        return cuerpo if ok else None

    # --- pruebas ------------------------------------------------------------------------------------
    def ejecutar(self, fixtures):
        # 1. Sin token -> 401
        r = self.get("/conn/ping/", con_token=False)
        self.anotar("1. GET /conn/ping/ sin token -> 401", r.status_code == 401, "HTTP {}".format(r.status_code), r.text)

        if self.token is None:
            self.anotar("2-23. Pruebas con token", False, "no hay token: Revit no está abierto o la extensión no ha iniciado")
            return

        # 2. ping
        def comprobar_ping(c):
            d = c["data"] or {}
            return bool(d.get("addin_version")) and bool(d.get("backend")), "addin={} backend={} revit={} documento={}".format(
                d.get("addin_version"), d.get("backend"), (d.get("revit") or {}).get("version_build"),
                (d.get("document") or {}).get("title"))
        self.comprobar_sobre("2. GET /conn/ping/ con token", self.get("/conn/ping/"), True, comprobar_ping)

        # 3. guía
        def comprobar_guia(c):
            texto = (c["data"] or {}).get("guide_markdown") or ""
            return len(texto) > 200 and "conn_validate" in texto, "{} caracteres".format(len(texto))
        self.comprobar_sobre("3. GET /conn/guide/", self.get("/conn/guide/"), True, comprobar_guia)

        # 4. tipos
        def comprobar_tipos(c):
            nombres = [t.get("type_name") for t in (c["data"] or {}).get("connection_types") or []]
            return "gusset_node" in nombres, "tipos={}".format(nombres)
        self.comprobar_sobre("4. GET /conn/types/", self.get("/conn/types/"), True, comprobar_tipos)

        # 5. esquema
        def comprobar_esquema(c):
            d = c["data"] or {}
            esquema = d.get("json_schema")
            ejemplo = d.get("example") or {}
            return isinstance(esquema, dict) and ejemplo.get("connection_type") == "gusset_node", \
                "claves de data={}, ejemplo.members={}".format(sorted(d.keys()), len(ejemplo.get("members") or []))
        self.comprobar_sobre("5. GET /conn/schema/gusset_node", self.get("/conn/schema/gusset_node"), True, comprobar_esquema)

        # 6. esquema de un tipo inexistente -> ok:false
        self.comprobar_sobre("6. GET /conn/schema/no_existe -> ok:false", self.get("/conn/schema/no_existe"), False)

        # 7. find_profile
        def comprobar_perfil(c):
            d = c["data"] or {}
            return (d.get("matched_count") or 0) >= 1, "coincidencias={} sugerencias={}".format(
                [m.get("type_name") for m in d.get("matches") or []], d.get("suggestions"))
        self.comprobar_sobre("7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16",
                             self.post("/conn/find_profile/", {"query": "HSS2-1/2X2-1/2X3/16"}), True, comprobar_perfil)

        # Fixtures
        confirmado, error1 = leer_fixture(fixtures, "detalle-D-confirmado.json")
        con_dudas, error2 = leer_fixture(fixtures, "detalle-D.json")
        if confirmado is None:
            self.anotar("8-13. Pruebas con el fixture detalle-D-confirmado.json", False, error1)
        else:
            ids = confirmado.get("node", {}).get("element_ids") or []
            cordon = (confirmado.get("chord") or {}).get("element_id")

            # 8. node_info
            def comprobar_nudo(c):
                d = c["data"] or {}
                miembros = d.get("members") or []
                return d.get("chord_element_id") == cordon and len(miembros) == len(ids) and len(d.get("origin_mm") or []) == 3, \
                    "cordón={} miembros={} origen_mm={}".format(d.get("chord_element_id"), len(miembros), d.get("origin_mm"))
            self.comprobar_sobre("8. POST /conn/node_info/ {} miembros".format(len(ids)),
                                 self.post("/conn/node_info/", {"element_ids": ids, "chord_element_id": cordon}), True, comprobar_nudo)

            # 9. validate correcto -> token
            def comprobar_valido(c):
                d = c["data"] or {}
                token = d.get("validation_token") or ""
                return d.get("is_valid") is True and re.fullmatch(r"[0-9a-fA-F]{64}", token) is not None, \
                    "is_valid={} token={}...".format(d.get("is_valid"), token[:12])
            self.comprobar_sobre("9. POST /conn/validate/ Detalle D con dudas confirmadas -> token",
                                 self.post("/conn/validate/", {"spec": confirmado}), True, comprobar_valido)

            # 10. validate con 420 -> 402
            roto = copy.deepcopy(confirmado)
            cadena = None
            for cadena in roto.get("dimension_chains") or []:
                if 420.0 in (cadena.get("values_mm") or []) or 420 in (cadena.get("values_mm") or []):
                    cadena["values_mm"] = [402.0 if v in (420, 420.0) else v for v in cadena["values_mm"]]
                    break
            def comprobar_sin_token(c):
                return (c.get("data") or {}).get("validation_token") in (None, ""), "sin token"
            self.comprobar_sobre("10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH",
                                 self.post("/conn/validate/", {"spec": roto}), False, comprobar_sin_token, "DIMENSION_CHAIN_MISMATCH")

            # 11. validate con dudas sin confirmar
            if con_dudas is None:
                self.anotar("11. POST /conn/validate/ con dudas sin confirmar", False, error2)
            else:
                self.comprobar_sobre("11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY",
                                     self.post("/conn/validate/", {"spec": con_dudas}), False, comprobar_sin_token, "UNRESOLVED_UNCERTAINTY")

            # 12. preview
            def comprobar_preview(c):
                resumen = (c["data"] or {}).get("summary") or {}
                return resumen.get("dry_run") is True and resumen.get("gusset_plates") == 1 and resumen.get("bolts") == 4, \
                    "resumen={}".format(json.dumps(resumen, ensure_ascii=False))
            self.comprobar_sobre("12. POST /conn/preview/ Detalle D", self.post("/conn/preview/", {"spec": confirmado}), True, comprobar_preview)

            # 13. create sin token -> rechazado sin tocar el modelo
            self.comprobar_sobre("13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID",
                                 self.post("/conn/create/", {"spec": confirmado}), False, None, "VALIDATION_TOKEN_INVALID")

        # 14. list
        def comprobar_lista(c):
            d = c["data"] or {}
            return isinstance(d.get("connections_count"), int) and isinstance(d.get("connections"), list), \
                "conexiones en el modelo={}".format(d.get("connections_count"))
        self.comprobar_sobre("14. GET /conn/list/", self.get("/conn/list/"), True, comprobar_lista)

        # 15. get inexistente
        self.comprobar_sobre("15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND",
                             self.get("/conn/get/00000000-0000-0000-0000-000000000000"), False, None, "ELEMENT_NOT_FOUND")

        # 16. delete inexistente (no borra nada)
        self.comprobar_sobre("16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND",
                             self.post("/conn/delete/", {"connection_id": "00000000-0000-0000-0000-000000000000"}), False, None,
                             "ELEMENT_NOT_FOUND")

        # 17. operación inexistente por la ruta genérica
        self.comprobar_sobre("17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION",
                             self.post("/conn/op/no_existe/", {}), False, None, "UNKNOWN_OPERATION")

        # --- Catálogo de plantillas (Fase 7). Escribe y borra un archivo en la carpeta del catálogo del PC; no toca el modelo. ---
        # 18. catalog_list
        def comprobar_catalogo(c):
            d = c["data"] or {}
            return isinstance(d.get("templates_count"), int) and isinstance(d.get("templates"), list) and bool(d.get("catalog_folder")), \
                "plantillas={} carpeta={}".format(d.get("templates_count"), d.get("catalog_folder"))
        self.comprobar_sobre("18. GET /conn/catalog/list/", self.get("/conn/catalog/list/"), True, comprobar_catalogo)

        if confirmado is None:
            self.anotar("19-23. Pruebas del catálogo con el fixture", False, "no hay fixture")
            return
        ids = confirmado.get("node", {}).get("element_ids") or []
        cordon = (confirmado.get("chord") or {}).get("element_id")

        # 19. catalog_save desde el fixture
        plantilla = {}
        def comprobar_guardada(c):
            d = c["data"] or {}
            plantilla.update(d)
            tid = d.get("template_id") or ""
            return re.fullmatch(r"[0-9a-fA-F-]{36}", tid) is not None and d.get("members_count") == len(confirmado.get("members") or []), \
                "template_id={} barras={} archivo={}".format(tid, d.get("members_count"), d.get("file"))
        self.comprobar_sobre("19. POST /conn/catalog/save/ desde el fixture -> template_id",
                             self.post("/conn/catalog/save/", {"spec": confirmado, "name": NOMBRE_PLANTILLA_PRUEBA,
                                                               "tags": ["prueba"], "overwrite": True}), True, comprobar_guardada)
        tid = plantilla.get("template_id")
        if not tid:
            self.anotar("20-23. Pruebas del catálogo", False, "no hay template_id")
            return

        # 20. catalog_get: sin IDs y con slot
        def comprobar_plantilla(c):
            d = c["data"] or {}
            t = d.get("template") or {}
            texto = json.dumps(t.get("spec_template") or {})
            return t.get("name") == NOMBRE_PLANTILLA_PRUEBA and "element_id" not in texto and '"slot"' in texto \
                and len(t.get("member_pattern") or []) == len(confirmado.get("members") or []), \
                "nombre={} patrón={}".format(t.get("name"), [(s.get("slot"), s.get("angle_deg"), s.get("side")) for s in t.get("member_pattern") or []])
        self.comprobar_sobre("20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot",
                             self.get("/conn/catalog/get/" + tid), True, comprobar_plantilla)

        # 21. catalog_apply al mismo nudo -> token y orientación same
        def comprobar_aplicada(c):
            d = c["data"] or {}
            token = d.get("validation_token") or ""
            spec = d.get("spec") or {}
            return d.get("is_valid") is True and re.fullmatch(r"[0-9a-fA-F]{64}", token) is not None \
                and (d.get("match") or {}).get("orientation") == "same" and (spec.get("source") or {}).get("template_id") == tid \
                and (spec.get("chord") or {}).get("element_id") == cordon, \
                "orientación={} desvío_máx={} token={}...".format((d.get("match") or {}).get("orientation"),
                                                                   (d.get("match") or {}).get("max_deviation_deg"), token[:12])
        self.comprobar_sobre("21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same",
                             self.post("/conn/catalog/apply/", {"template_id": tid, "element_ids": ids, "chord_element_id": cordon}),
                             True, comprobar_aplicada)

        # 22. catalog_apply con una plantilla inexistente
        self.comprobar_sobre("22. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND",
                             self.post("/conn/catalog/apply/", {"template_id": "00000000-0000-0000-0000-000000000000", "element_ids": ids}),
                             False, None, "TEMPLATE_NOT_FOUND")

        # 23. catalog_delete (limpieza)
        def comprobar_borrada(c):
            return (c["data"] or {}).get("deleted_template_id") == tid, "borrada {}".format(tid)
        self.comprobar_sobre("23. POST /conn/catalog/delete/ -> borrada", self.post("/conn/catalog/delete/", {"template_id": tid}), True, comprobar_borrada)

    # --- puente MCP ---------------------------------------------------------------------------------
    def ejecutar_puente(self, puente):
        cabeceras = {"Accept": "application/json, text/event-stream", "Content-Type": "application/json"}

        def rpc(id_, metodo, params=None, sesion=None):
            cuerpo = {"jsonrpc": "2.0", "id": id_, "method": metodo, "params": params or {}}
            cab = dict(cabeceras)
            if sesion:
                cab["Mcp-Session-Id"] = sesion
            return self.cliente.post(puente, json=cuerpo, headers=cab)

        def resultado_de(respuesta):
            """Resultado JSON-RPC de una respuesta JSON o SSE."""
            texto = respuesta.text
            if respuesta.headers.get("content-type", "").startswith("text/event-stream"):
                for linea in texto.splitlines():
                    if linea.startswith("data:"):
                        texto = linea[5:].strip()
            return json.loads(texto)

        try:
            r = rpc(1, "initialize", {"protocolVersion": "2025-06-18", "capabilities": {},
                                      "clientInfo": {"name": "probar_conexiones", "version": "4"}})
        except httpx.ConnectError as error:
            self.anotar("24. POST {} initialize".format(puente), False, "no se pudo conectar con el puente: {}".format(error))
            self.anotar("25. tools/call conn_ping por el puente", False, "sin puente")
            return
        sesion = r.headers.get("mcp-session-id")
        if r.status_code != 200:
            self.anotar("24. POST {} initialize".format(puente), False, "HTTP {}".format(r.status_code), r.text)
            self.anotar("25. tools/call conn_ping por el puente", False, "sin initialize")
            return
        try:
            self.cliente.post(puente, json={"jsonrpc": "2.0", "method": "notifications/initialized"},
                              headers=dict(cabeceras, **({"Mcp-Session-Id": sesion} if sesion else {})))
        except httpx.HTTPError:
            pass

        r = rpc(2, "tools/list", {}, sesion)
        nombres = []
        ok = r.status_code == 200
        detalle = "HTTP {}".format(r.status_code)
        if ok:
            try:
                nombres = [t["name"] for t in resultado_de(r)["result"]["tools"]]
            except (ValueError, KeyError, TypeError) as error:
                ok, detalle = False, detalle + ", respuesta inesperada: {}".format(error)
        if ok:
            faltan = [n for n in HERRAMIENTAS_CONN if n not in nombres]
            ok = not faltan
            detalle += ", herramientas={} conn_*={}{}".format(len(nombres), len([n for n in nombres if n.startswith("conn_")]),
                                                              " FALTAN " + str(faltan) if faltan else "")
        self.anotar("24. tools/list por el puente trae las 18 herramientas conn_*", ok, detalle,
                    ", ".join(n for n in nombres if n.startswith("conn_")))

        r = rpc(3, "tools/call", {"name": "conn_ping", "arguments": {}}, sesion)
        ok = r.status_code == 200
        detalle = "HTTP {}".format(r.status_code)
        texto = ""
        if ok:
            try:
                res = resultado_de(r)["result"]
                texto = res["content"][0]["text"]
                sobre = json.loads(texto)
                ok = sobre.get("ok") is True and bool((sobre.get("data") or {}).get("addin_version"))
                detalle += ", isError={} ok={} addin={}".format(res.get("isError"), sobre.get("ok"),
                                                                (sobre.get("data") or {}).get("addin_version"))
            except (ValueError, KeyError, TypeError, IndexError) as error:
                ok, detalle = False, detalle + ", respuesta inesperada: {}".format(error)
        self.anotar("25. tools/call conn_ping por el puente -> ok:true", ok, detalle, texto or r.text)


def main():
    parser = argparse.ArgumentParser(description="Pruebas de humo de las rutas /conn/ de MotorConexiones")
    parser.add_argument("--revit", default=REVIT_POR_DEFECTO, help="base de las rutas de Revit (pyRevit Routes)")
    parser.add_argument("--token-archivo", default=RUTA_TOKEN_POR_DEFECTO, help="archivo con el token de sesión")
    parser.add_argument("--fixtures", default=str(FIXTURES_POR_DEFECTO), help="carpeta con los fixtures del Detalle D")
    parser.add_argument("--puente", action="store_true", help="probar también el puente MCP (tools/list y conn_ping)")
    parser.add_argument("--puente-url", default=PUENTE_POR_DEFECTO)
    args = parser.parse_args()

    token = leer_token(args.token_archivo)
    if token is None:
        print("AVISO: no existe {} o está vacío: Revit no está abierto o la extensión no ha iniciado.".format(args.token_archivo))
    cliente = httpx.Client(timeout=180.0)
    pruebas = Pruebas(args.revit, token, cliente)
    try:
        pruebas.ejecutar(args.fixtures)
        if args.puente:
            pruebas.ejecutar_puente(args.puente_url)
    except httpx.ConnectError as error:
        pruebas.anotar("Conexión con Revit", False, "no se pudo hablar con {}: {}. ¿Revit abierto con pyRevit Routes activo?".format(args.revit, error))
    finally:
        cliente.close()

    print("=" * 70)
    print("Resultado: {}/{} pruebas correctas".format(sum(pruebas.resultados), len(pruebas.resultados)))
    return 0 if pruebas.resultados and all(pruebas.resultados) else 1


if __name__ == "__main__":
    sys.exit(main())
