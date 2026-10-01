#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Pruebas de humo de las rutas /conn/ de MotorConexiones (CPython 3, solo httpx y stdlib).

Requisitos: Revit 2027 abierto con el modelo, pyRevit con Routes activo (48884), la extensión
revit-mcp con conexiones.py instalado y recargado, y el add-in MotorConexiones desplegado.
No hace falta el puente main.py: habla directamente con Revit por HTTP con el token.

Uso (con el Python del .venv de la extensión):
    C:\\IA\\pyrevit-ext\\mcp-server-for-revit-python.extension\\.venv\\Scripts\\python.exe mcp\\pruebas\\probar_conexiones.py

Fase 1: ping sin token (401), ping con token (200, ok:true), operación inexistente
(200, ok:false, UNKNOWN_OPERATION) y dev_exec (200, ok:true, output "hola").
Termina con "Resultado: N/N pruebas correctas" y código de salida 0 si todas pasan.
"""
import json
import os
import sys

import httpx

REVIT = "http://127.0.0.1:48884/revit_mcp"
RUTA_TOKEN = os.path.expandvars(r"%LOCALAPPDATA%\RevitMcp\token")


def leer_token():
    try:
        with open(RUTA_TOKEN, "r", encoding="utf-8") as archivo:
            return archivo.read().strip()
    except OSError:
        return None


def mostrar(nombre, esperado, respuesta, comprobar=None):
    ok = respuesta.status_code == esperado
    detalle = ""
    cuerpo = None
    if ok and comprobar is not None:
        try:
            cuerpo = respuesta.json()
        except json.JSONDecodeError:
            cuerpo = None
        try:
            ok, detalle = comprobar(cuerpo)
        except Exception as error:  # noqa: BLE001
            ok, detalle = False, "excepción al comprobar: {}".format(error)
    print("=" * 70)
    print("{} -> {}  (esperado {})  [{}]{}".format(
        nombre, respuesta.status_code, esperado, "OK" if ok else "FALLO",
        "  " + detalle if detalle else ""))
    print("Cuerpo:")
    texto = respuesta.text
    print(texto if len(texto) <= 3000 else texto[:3000] + " ...")
    return ok


def main():
    resultados = []
    cliente = httpx.Client(timeout=60.0)

    # 1. GET /conn/ping/ sin token -> 401
    r = cliente.get(REVIT + "/conn/ping/")
    resultados.append(mostrar("1. GET /conn/ping/ sin token", 401, r))

    token = leer_token()
    if token is None:
        print("=" * 70)
        print("No existe {}: Revit no está abierto o la extensión no ha iniciado".format(RUTA_TOKEN))
        resultados.extend([False, False, False])
    else:
        # 2. GET /conn/ping/ con token -> 200 y ok:true
        r = cliente.get(REVIT + "/conn/ping/", params={"token": token})

        def comprobar_ping(cuerpo):
            if not isinstance(cuerpo, dict):
                return False, "la respuesta no es un objeto JSON"
            if cuerpo.get("ok") is not True:
                codigos = [e.get("code") for e in cuerpo.get("errors", [])]
                return False, "ok:false " + ", ".join(str(c) for c in codigos)
            datos = cuerpo.get("data") or {}
            return bool(datos.get("addin_version")), "addin_version={} revit={}".format(
                datos.get("addin_version"), (datos.get("revit") or {}).get("version_build"))

        resultados.append(mostrar("2. GET /conn/ping/ con token", 200, r, comprobar_ping))

        # 3. POST /conn/op/no_existe/ -> 200 con ok:false y UNKNOWN_OPERATION
        r = cliente.post(REVIT + "/conn/op/no_existe/", json={"token": token})

        def comprobar_desconocida(cuerpo):
            if not isinstance(cuerpo, dict) or cuerpo.get("ok") is not False:
                return False, "se esperaba ok:false"
            codigos = [e.get("code") for e in cuerpo.get("errors", [])]
            return "UNKNOWN_OPERATION" in codigos, "códigos: " + ", ".join(str(c) for c in codigos)

        resultados.append(mostrar("3. POST /conn/op/no_existe/", 200, r, comprobar_desconocida))

        # 4. POST /conn/dev_exec/ -> 200, ok:true, output "hola"
        r = cliente.post(REVIT + "/conn/dev_exec/", json={"token": token, "code": 'print("hola")', "description": "prueba"})

        def comprobar_dev_exec(cuerpo):
            if not isinstance(cuerpo, dict) or cuerpo.get("ok") is not True:
                return False, "se esperaba ok:true"
            salida = (cuerpo.get("data") or {}).get("output", "")
            return salida.strip() == "hola", "output={!r}".format(salida.strip())

        resultados.append(mostrar("4. POST /conn/dev_exec/ print('hola')", 200, r, comprobar_dev_exec))

    print("=" * 70)
    print("Resultado: {}/{} pruebas correctas".format(sum(resultados), len(resultados)))
    return 0 if all(resultados) else 1


if __name__ == "__main__":
    sys.exit(main())
