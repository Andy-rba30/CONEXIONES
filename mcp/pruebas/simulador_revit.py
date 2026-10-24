#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Simulador de Revit + pyRevit Routes para probar en la nube, sin Revit, el lado MCP de MotorConexiones.

Carga el adaptador real mcp/revit_mcp/conexiones.py (y el decorador real revit_mcp/seguridad.py de la
extensión, si se indica --extension) con módulos pyrevit/clr/System simulados, sustituye Bridge.Handle
por una imitación en Python del add-in (mismas operaciones, mismos códigos de error, mismo sobre común)
y sirve las rutas /revit_mcp/conn/... por HTTP en el puerto 48884 con el token de sesión, igual que
pyRevit Routes. Con eso se pueden ejecutar en Linux:

  * mcp/pruebas/probar_conexiones.py            (habla con 48884)
  * main.py --streamable-http de revit-mcp       (el puente real, con tools/conn_tools.py instalado)

NO sustituye a las pruebas en el PC: la imitación del add-in es deliberadamente simple (solo lo que
hace falta para comprobar el adaptador, el script de pruebas y el puente). Lo que pasa dentro de
Revit solo lo prueba el instalador.

Uso:
    python3 mcp/pruebas/simulador_revit.py --autocomprobar [--extension <clon de revit-mcp>]
        Comprueba en proceso que conexiones.py registra las 27 rutas (15 de la Fase 4, 5 del catálogo de la Fase 7, 3 del plan de la
        Fase 8, 2 de crear por lotes de la Fase 9 y 2 de partir la diagonal de la Fase 13) y que cada una llega a Bridge.Handle con la operación y el cuerpo correctos;
        desde la Fase 12 también los tres tipos (gusset_node, direct_weld, end_plate), node_info con la excentricidad y el plan con
        la clase splice y los arranques. Código de salida 0 si todo va bien.

    python3 mcp/pruebas/simulador_revit.py [--puerto 48884] [--token-archivo <ruta>] [--extension <clon>]
        Sirve HTTP hasta Ctrl+C. Escribe el token en --token-archivo (por defecto, el nombre literal
        "%LOCALAPPDATA%\\RevitMcp\\token" en la carpeta actual, que es lo que main.py y probar_conexiones.py
        abren cuando corren en Linux, donde expandvars no toca las variables de Windows).

    Opciones: --sin-addin (responde ADDIN_NOT_LOADED), --sin-documento (NO_DOCUMENT en las operaciones
    que necesitan modelo), --conexiones <ruta> (otro conexiones.py, por ejemplo el copiado a la extensión).
"""
import argparse
import copy
import hashlib
import inspect
import io
import json
import math
import os
import re
import sys
import threading
import time
import types
import uuid
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import parse_qs, unquote, urlsplit

RAIZ = Path(__file__).resolve().parents[2]
RUTA_CONEXIONES = RAIZ / "mcp" / "revit_mcp" / "conexiones.py"
RUTA_GUIA = RAIZ / "docs" / "guide.md"
RUTA_FIXTURES = RAIZ / "docs" / "fixtures"
RUTA_FIXTURE = RUTA_FIXTURES / "detalle-D-confirmado.json"
NOMBRE_TOKEN_LITERAL = r"%LOCALAPPDATA%\RevitMcp\token"

# Nudo del Detalle D en HANGAR_PRUEBA_sondeo.rvt (resultados de la Fase 3).
MIEMBROS = {
    1249510: {"family": "HSS-Hollow Structural Section", "type": "HSS3X3X1/4", "angle": 180.0, "chord": True,
              "start": [-4437.3, -17195.7, 17423.0], "end": [-14397.6, -17195.8, 17423.0]},
    1249630: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 43.1, "chord": False,
              "start": [-14536.8, -17195.8, 19918.8], "end": [-11930.6, -17195.8, 17481.8]},
    1249631: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 135.6, "chord": False,
              "start": [-11880.4, -17195.8, 17441.0], "end": [-9400.0, -17195.8, 19900.0]},
    1249636: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 45.0, "chord": False,
              "start": [-9300.0, -17195.8, 14900.0], "end": [-11832.5, -17195.8, 17387.8]},
}
PERFILES_MODELO = ["HSS3X3X1/4", "HSS2-1-2X2-1-2X3-16 64x64", "HSS4X4X1/4", "W12X26", "L3X3X1/4", "C8X11.5"]
ORIGEN_MM = [-11867.7, -17195.8, 17423.0]
PROYECTO_UNIQUE_ID = "simulador-00000000-0000-0000-0000-000000000001"
ADDIN_VERSION = "0.13.0"  # Fase 13 (0.13.0): rutas split y unsplit (27); la cartela automatica (outline.mode auto) devuelve calculated_values.outline_points_mm en validate, preview y catalog/apply; node_info suma split_point_mm por diagonal; el plan lleva los pasos split y los nudos N...B (after_split) y batch_create / batch_delete los ejecutan y deshacen. Cierre de la ronda 12d (0.12.4): sin cambios en las rutas; el add-in filtra del aviso la advertencia de Revit "solo visible en nivel Fino" (sondeo 29: la viga de Advance Steel no aparecio; el retiro por el eje queda definitivo). Ronda 12d (0.12.3): sin cambios en las rutas; el add-in registra las advertencias de Revit (revit_failure) y el sondeo 29 busca la viga de Advance Steel con DatabaseManager.GetModelObjectIds. Ronda 12c (0.12.2): node_info suma section_rotation_deg por barra (el HSS12X8X1/2 del cordon superior va girado 90: width_mm 203,2 en el plano, height_mm 304,8 perpendicular, sondeo 27); el add-in lee el perfil con Ht/b y el giro, y las soldaduras van con WeldLine. Ronda 12b (0.12.1): validate devuelve calculated_values.kind y eccentricity_mm tambien en el add-in; excentricidad real del nudo en K 54,9. Fase 12 (0.12.0): misma version que AddinInfo.Version del add-in; tres tipos, node_info con kind y excentricidad, plan con splice y arranques. Fase 11 (0.11.0): catalog/apply sin template_id elige la mejor (proposal). Ronda 10b (0.10.1): idem

# Fase 12: barras del cordón superior del Hangar (el nudo en K del paso 11-5 de la Fase 11, 54,9 mm medidos en la Fase 12, y el empalme N9E de la ronda
# 8c) y los tramos ficticios de los Detalles B e I. NO entran en la "selección actual" del simulador (MIEMBROS es el Detalle D y la
# selección asistida solo lo conoce a él), pero node_info, validate, catalog/apply y batch/plan las aceptan. "splice" agrupa los dos
# tramos de un empalme (clase splice del detector); "k_angle" y "k_offset" son el ángulo con signo y el desvío del eje
# (axis_offset_mm, opción A) de cada diagonal en el nudo en K; "width", "height" y "wall" son el perfil: "width" es el lado EN el plano de la
# cercha y "height" el PERPENDICULAR (ronda 12c, sondeo 27: el HSS12X8X1/2 va girado 90°, "rotation"; medio canto = lado mayor / 2).
ORIGEN_K_MM = [-9349.7, -17195.8, 19933.0]   # punto de trabajo del nudo en K: a mitad entre los cortes de las dos diagonales (54,9 mm, medidos en Revit en la Fase 12)
EXCENTRICIDAD_K_MM = 54.9
BARRAS_FASE12 = {
    1245533: {"family": "HSS-Hollow Structural Section", "type": "HSS12X8X1/2", "angle": 0.0, "chord": True, "width": 203.2, "height": 304.8, "wall": 12.7, "rotation": 90.0,
              "start": [-14551.9, -17195.8, 19933.0], "end": [-4180.2, -17195.8, 19933.0]},
    1249632: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": -45.0, "chord": False, "width": 63.5, "height": 63.5, "wall": 4.8,
              "k_angle": -45.0, "k_offset": 27.45, "start": [-9322.3, -17195.8, 19900.0], "end": [-6889.6, -17195.8, 17441.0]},
    1245531: {"family": "HSS-Hollow Structural Section", "type": "HSS12X8X1/2", "angle": 0.0, "chord": True, "width": 203.2, "height": 304.8, "wall": 12.7, "rotation": 90.0, "splice": "C",
              "start": [-14551.9, 17204.0, 19933.0], "end": [-4180.2, 17204.0, 19933.0]},
    1245530: {"family": "HSS-Hollow Structural Section", "type": "HSS12X8X1/2", "angle": 0.0, "chord": True, "width": 203.2, "height": 304.8, "wall": 12.7, "rotation": 90.0, "splice": "C",
              "start": [-4180.2, 17204.0, 19933.0], "end": [6069.8, 17204.0, 19933.0]},
    9101: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 0.0, "chord": True, "width": 63.5, "height": 63.5, "wall": 4.8, "splice": "B",
           "start": [-11880.4, -17195.8, 17441.0], "end": [-11647.1, -17195.8, 17674.3]},
    9102: {"family": "HSS2-1-2X2-1-2X3-16 64x64", "type": "HSS2-1-2X2-1-2X3-16 64x64", "angle": 0.0, "chord": True, "width": 63.5, "height": 63.5, "wall": 4.8, "splice": "B",
           "start": [-11647.1, -17195.8, 17674.3], "end": [-9400.0, -17195.8, 19900.0]},
    9201: {"family": "HSS-Hollow Structural Section", "type": "HSS3X3X1/4", "angle": 0.0, "chord": True, "width": 76.2, "height": 76.2, "wall": 6.35, "splice": "I",
           "start": [-14397.6, -17195.8, 14900.0], "end": [-4180.2, -17195.8, 14900.0]},
    9202: {"family": "HSS-Hollow Structural Section", "type": "HSS3X3X1/4", "angle": 0.0, "chord": True, "width": 76.2, "height": 76.2, "wall": 6.35, "splice": "I",
           "start": [-4180.2, -17195.8, 14900.0], "end": [6069.8, -17195.8, 14900.0]},
}
K_ANGULOS = {1249631: -135.6, 1249632: -45.0}   # la diagonal 1249631 es la misma barra del Detalle D (su otro extremo llega al cordón superior)
K_DESVIOS = {1249631: -27.45, 1249632: 27.45}
TODAS_LAS_BARRAS = dict(MIEMBROS)
TODAS_LAS_BARRAS.update(BARRAS_FASE12)

LLAMADAS = []          # (operation, request dict) que recibe el Bridge simulado
CONEXIONES = {}        # connection_id -> registro
CATALOGO = {}          # template_id -> plantilla (Fase 7; en el add-in real son archivos JSON en %LOCALAPPDATA%)
PLANES = {}            # plan_id -> plan (Fase 8; en el add-in real viven en memoria hasta descartar o cerrar Revit)
ULTIMO_PLAN = [None]
OPCIONES = types.SimpleNamespace(sin_addin=False, sin_documento=False)
# Fase 13: los cortes de barra (SplitRecord del add-in) y el siguiente id ficticio del tramo largo.
CORTES = {}            # split_id -> registro
SIGUIENTE_ID_NUEVO = [9900001]
OFFSET_B_MM = 330.0    # diagonal_splice_offset_mm de choices.json
ESPESOR_B_MM = 9.525   # la plancha del Detalle B (3/8")
MAX_CARTELA_AUTO_MM = 1200.0


# ---------------------------------------------------------------------------
# Imitación del add-in (Bridge.Handle)
# ---------------------------------------------------------------------------
def _sobre(operation, ok, data=None, errors=None, warnings=None):
    return {"ok": ok, "data": data, "errors": errors or [], "warnings": warnings or [],
            "meta": {"operation": operation, "duration_ms": 1, "addin_version": ADDIN_VERSION}}


def _err(code, message, path=None, hint=None):
    return {"code": code, "path": path, "message": message, "hint": hint}


def _canonico(obj):
    return json.dumps(obj, sort_keys=True, separators=(",", ":"), ensure_ascii=False)


def _token_de(spec):
    ids = set(spec.get("node", {}).get("element_ids", []) or [])
    if spec.get("chord", {}).get("element_id"):
        ids.add(spec["chord"]["element_id"])
    partes = [_canonico(spec), "|PROJECT:" + PROYECTO_UNIQUE_ID]
    for i in sorted(ids):
        m = _barra(i)
        if m:
            partes.append("|{}:{}:{}:{}".format(i, m["type"], m["start"], m["end"]))
    return hashlib.sha256("".join(partes).encode("utf-8")).hexdigest()


def _normalizar_perfil(texto):
    return re.sub(r"[\s/\-]", "", (texto or "").upper())


def _spec_de(req):
    spec = req.get("spec") if isinstance(req.get("spec"), dict) else req
    return spec if isinstance(spec, dict) else None


def _barra(i):
    """La barra simulada con ese id (el Detalle D o las de la Fase 12), o None."""
    try:
        return TODAS_LAS_BARRAS.get(int(i))
    except (TypeError, ValueError):
        return None


def _distancia(a, b):
    return sum((x - y) ** 2 for x, y in zip(a, b)) ** 0.5


def _medio_canto(i):
    """Medio canto del cordón (el lado mayor, como el validador del Core); 40 mm si el perfil no se conoce."""
    m = _barra(i) or {}
    lados = [v for v in (m.get("width"), m.get("height")) if v]
    return max(lados) / 2.0 if lados else 40.0


def _nudo_de(ids, cordon=None):
    """Imitación de SpecFrame.Compute (Fase 12, opción A): la clase del nudo, su punto de trabajo, la excentricidad y, por barra, el
    ángulo con signo y el desvío de su eje (axis_offset_mm). Dos tramos del mismo empalme → splice (origen en el punto de empalme);
    el cordón superior 1245533 con sus diagonales → el nudo en K de 54,9 mm; lo demás → el Detalle D, concurrente."""
    barras = [int(i) for i in ids if _barra(i)]
    grupos = {_barra(i).get("splice") for i in barras}
    if len(barras) == 2 and len(grupos) == 1 and None not in grupos:
        a, b = _barra(barras[0]), _barra(barras[1])
        punto = next((p for p in (a["start"], a["end"]) for q in (b["start"], b["end"]) if _distancia(p, q) < 1.0), a["end"])
        cordon = cordon if cordon in barras else barras[0]
        return {"kind": "splice", "origin": list(punto), "eccentricity": 0.0, "chord": cordon,
                "angles": {i: 0.0 for i in barras}, "offsets": {i: 0.0 for i in barras}, "sides": {i: "chord" for i in barras}}
    if cordon not in barras:
        cordon = next((i for i in barras if _barra(i)["chord"]), barras[0] if barras else None)
    origen = ORIGEN_K_MM if cordon == 1245533 else ORIGEN_MM
    # El lado (+Y arriba, -Y abajo) sale de la geometría: el extremo lejano de la barra por encima o por debajo del eje del cordón.
    sides = {}
    for i in barras:
        m = _barra(i)
        lejano = max((m["start"], m["end"]), key=lambda p: _distancia(p, origen))
        sides[i] = "chord" if i == cordon else ("+Y" if lejano[2] >= origen[2] else "-Y")
    if cordon == 1245533:
        return {"kind": "node", "origin": list(ORIGEN_K_MM), "eccentricity": EXCENTRICIDAD_K_MM, "chord": cordon,
                "angles": {i: (0.0 if i == cordon else K_ANGULOS.get(i, _barra(i)["angle"])) for i in barras},
                "offsets": {i: (0.0 if i == cordon else K_DESVIOS.get(i, 0.0)) for i in barras}, "sides": sides}
    return {"kind": "node", "origin": list(ORIGEN_MM), "eccentricity": 0.0, "chord": cordon,
            "angles": {i: (0.0 if i == cordon else _barra(i)["angle"]) for i in barras}, "offsets": {i: 0.0 for i in barras}, "sides": sides}


def _nudo_de_spec(spec):
    ids = (spec.get("node") or {}).get("element_ids") or []
    cordon = (spec.get("chord") or {}).get("element_id")
    conocidos = [i for i in ids if _barra(i)]
    if cordon and _barra(cordon) and cordon not in conocidos:
        conocidos.insert(0, cordon)
    return _nudo_de(conocidos, cordon if _barra(cordon) else None) if len(conocidos) >= 2 else None


def _valores_calculados(nudo, spec=None):
    o = nudo["origin"] if nudo else ORIGEN_MM
    valores = {"kind": nudo["kind"] if nudo else "node", "eccentricity_mm": nudo["eccentricity"] if nudo else 0.0, "origin_mm": o,
               "axis_distance_mm": 0.08, "frame_x": [-1, 0, 0], "frame_y": [0, 0, 1], "frame_z": [0, 1, 0]}
    # Fase 13: con outline.mode auto, los vértices calculados (los que se dibujan y se fabrican); con polygon, nada nuevo.
    contorno = _contorno_auto(spec, nudo) if spec else None
    if contorno:
        valores.update({"outline_mode": "auto", "outline_points_mm": contorno["points"], "outline_width_mm": contorno["width"],
                        "outline_height_mm": contorno["height"], "outline_vertices": len(contorno["points"])})
    return valores


def _unitario(v):
    n = sum(x * x for x in v) ** 0.5
    return [x / n for x in v] if n > 1e-9 else [0.0, 0.0, 0.0]


def _punto_corte(bar_id, chord_id, offset=OFFSET_B_MM):
    """SplitPoint.Compute del Core, imitado (Fase 13, D7): la cara del cordón a (ancho/2)/|sen θ| del corte de los ejes, + offset hasta
    la primera plancha, + espesor (centro de la pila). Devuelve (dict, None) o (None, código de error)."""
    b, c = _barra(bar_id), _barra(chord_id)
    if not b or not c:
        return None, "ELEMENT_NOT_FOUND"
    u = _unitario([e - s for s, e in zip(b["start"], b["end"])])
    v = _unitario([e - s for s, e in zip(c["start"], c["end"])])
    dot = sum(x * y for x, y in zip(u, v))
    sin = max(0.0, 1.0 - dot * dot) ** 0.5
    if sin < math.sin(math.radians(5.0)):
        return None, "NODE_AXES_PARALLEL"
    theta = math.degrees(math.asin(min(1.0, sin)))
    half = (c.get("width") or 76.2) / 2.0
    face = half / sin
    first = face + max(0.0, offset)
    split = first + ESPESOR_B_MM
    # El corte de los ejes: el punto de la recta de la barra más cercano a la del cordón.
    w0 = [p - q for p, q in zip(b["start"], c["start"])]
    d = sum(x * y for x, y in zip(u, w0))
    e = sum(x * y for x, y in zip(v, w0))
    denom = 1.0 - dot * dot
    t = (dot * e - d) / denom
    cut = [p + t * x for p, x in zip(b["start"], u)]
    lejos = max((b["start"], b["end"]), key=lambda p: _distancia(p, cut))
    direccion = _unitario([p - q for p, q in zip(lejos, cut)])
    punto = [round(p + split * x, 1) for p, x in zip(cut, direccion)]
    near_end = 0 if lejos == b["end"] else 1
    cerca = b["start"] if near_end == 0 else b["end"]
    largo = _distancia(b["start"], b["end"])
    if _distancia(cerca, punto) >= largo or _distancia(punto, lejos) > largo:
        return None, "SPLIT_FAILED"
    return {"theta_deg": round(theta, 1), "face_distance_mm": round(face, 1), "first_plate_distance_mm": round(first, 1),
            "split_distance_mm": round(split, 1), "cut_point_mm": [round(x, 1) for x in cut], "split_point_mm": punto, "near_end": near_end,
            "short_mm": round(_distancia(cerca, punto), 1), "long_mm": round(_distancia(punto, lejos), 1),
            "plate_thickness_mm": ESPESOR_B_MM, "gap_mm": 0.0}, None


def _envolvente_convexa(puntos):
    """Andrew (cadena monótona), como Polygon2D.ConvexHull del Core."""
    pts = sorted(set((round(x, 3), round(y, 3)) for x, y in puntos))
    if len(pts) < 3:
        return [list(p) for p in pts]

    def cruz(o, a, b):
        return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])
    inferior, superior = [], []
    for p in pts:
        while len(inferior) >= 2 and cruz(inferior[-2], inferior[-1], p) <= 0:
            inferior.pop()
        inferior.append(p)
    for p in reversed(pts):
        while len(superior) >= 2 and cruz(superior[-2], superior[-1], p) <= 0:
            superior.pop()
        superior.append(p)
    return [list(p) for p in inferior[:-1] + superior[:-1]]


def _contorno_auto(spec, nudo):
    """AutoOutline.Compute del Core, imitado (Fase 13, sección 3.2): rectángulo por barra (retiro → final de la ranura o placa + margen,
    medio ancho + margen lateral), franja del cordón (medio canto + más allá), envolvente convexa redondeada a 5 mm hacia fuera.
    None si la cartela no es automática."""
    gusset = (spec or {}).get("gusset") or {}
    outline = gusset.get("outline") or {}
    if outline.get("mode") != "auto":
        return None
    margen = outline.get("margin_mm", 40.0)
    lateral = outline.get("side_margin_mm", 30.0)
    mas_alla = outline.get("beyond_chord_mm", 40.0)
    cordon = (nudo or {}).get("chord")
    medio_cordon = ((_barra(cordon) or {}).get("width") or 76.2) / 2.0
    puntos = []
    for m in spec.get("members") or []:
        eid = m.get("element_id")
        angulo = (nudo or {}).get("angles", {}).get(eid, m.get("expected_angle_deg") or 45.0)
        ux, uy = math.cos(math.radians(angulo)), math.sin(math.radians(angulo))
        a = m.get("attachment") or {}
        retiro = m.get("end_setback_mm") or 0.0
        if a.get("type") == "bolted_knife_plate":
            placa = a.get("plate") or {}
            fin = retiro + (placa.get("length_mm") or 170.0) - (placa.get("insertion_mm") or 80.0) + margen
        else:
            fin = retiro + (a.get("slot_length_mm") or 150.0) + margen
        medio = ((_barra(eid) or {}).get("width") or 63.5) / 2.0 + lateral
        ox = (nudo or {}).get("offsets", {}).get(eid, 0.0)
        for d in (retiro, fin):
            for lado in (-medio, medio):
                puntos.append((ox + ux * d - uy * lado, uy * d + ux * lado))
    if not puntos:
        return None
    xs = [p[0] for p in puntos]
    h = medio_cordon + mas_alla
    interfaz = gusset.get("chord_interface") or "through_slot"
    ymin = -h if interfaz in ("through_slot", "both_sides") else 0.0
    puntos += [(min(xs), ymin), (max(xs), ymin), (max(xs), h), (min(xs), h)]
    casco = _envolvente_convexa(puntos)
    cx = sum(p[0] for p in casco) / len(casco)
    cy = sum(p[1] for p in casco) / len(casco)
    redondeados = []
    for x, y in casco:
        rx = (math.floor(x / 5.0) if x < cx else math.ceil(x / 5.0)) * 5.0
        ry = (math.floor(y / 5.0) if y < cy else math.ceil(y / 5.0)) * 5.0
        if not redondeados or [rx, ry] != redondeados[-1]:
            redondeados.append([rx, ry])
    xs = [p[0] for p in redondeados]
    ys = [p[1] for p in redondeados]
    return {"points": redondeados, "width": round(max(xs) - min(xs), 1), "height": round(max(ys) - min(ys), 1)}


def _op_ping(req):
    documento = None if OPCIONES.sin_documento else {
        "title": "HANGAR_PRUEBA_sondeo", "path": r"D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt",
        "is_family": False, "is_workshared": False, "is_modifiable": False, "is_read_only": False}
    return _sobre("ping", True, {
        "addin_version": ADDIN_VERSION, "spec_version": "1.0", "backend": "advancesteel",
        "operations": sorted(OPERACIONES.keys()),
        "revit": {"version_number": "2027", "version_build": "27.2.0.39", "version_name": "Autodesk Revit 2027",
                  "sub_version_number": "2027.2", "language": "English_USA"},
        "dotnet": {"framework": ".NET 10.0.12 (simulado)", "assembly_location": "simulador", "load_context": "Default"},
        "document": documento, "has_uidocument": not OPCIONES.sin_documento})


def _op_guide(req):
    texto = RUTA_GUIA.read_text(encoding="utf-8") if RUTA_GUIA.is_file() else "# Guía (no encontrada)"
    return _sobre("guide", True, {"guide_markdown": texto})


# Fase 12: los tres tipos del registro del add-in (Core/Types) con la descripción literal, el fixture que sirve de ejemplo y las
# claves del esquema simulado (el esquema real lo escribe JsonSchemaValidator del Core; aquí solo se imita la forma).
TIPOS = {
    "gusset_node": {
        "description": "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.",
        "example": "detalle-D-confirmado.json",
        "required": ["spec_version", "connection_type", "node", "chord", "gusset", "members"],
        "properties": ["spec_version", "connection_type", "source", "node", "chord", "gusset", "members", "dimension_chains", "uncertain_fields"]},
    "direct_weld": {
        "description": "Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cordón y se suelda en todo el contorno, sin cartela ni "
                       "placas (los nudos del cordón superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cordón.",
        "example": "nudo-K-cordon-superior.json",
        "required": ["spec_version", "connection_type", "node", "chord", "members"],
        "properties": ["spec_version", "connection_type", "source", "node", "chord", "members", "dimension_chains", "uncertain_fields"]},
    "end_plate": {
        "description": "Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, "
                       "unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).",
        "example": "detalle-C.json",
        "required": ["spec_version", "connection_type", "node", "chord", "members", "plate", "bolts"],
        "properties": ["spec_version", "connection_type", "kind", "source", "node", "chord", "members", "plate", "bolts", "gap_mm", "dimension_chains", "uncertain_fields"]},
}


def _fixture(nombre):
    ruta = RUTA_FIXTURES / nombre
    return json.loads(ruta.read_text(encoding="utf-8")) if ruta.is_file() else {}


def _op_types(req):
    return _sobre("types", True, {"connection_types": [{"type_name": t, "description": TIPOS[t]["description"]} for t in TIPOS]})


def _op_schema(req):
    tipo = req.get("type", "gusset_node")
    if tipo not in TIPOS:
        return _sobre("schema", False, errors=[_err("UNKNOWN_OPERATION",
                      "El tipo de conexión '{}' no está registrado.".format(tipo), "type",
                      "Tipos disponibles: " + ", ".join(TIPOS))])
    t = TIPOS[tipo]
    esquema = {"$schema": "http://json-schema.org/draft-07/schema#", "title": tipo + " (simulado)",
               "type": "object", "additionalProperties": False, "required": list(t["required"]),
               "properties": {k: {} for k in t["properties"]}}
    return _sobre("schema", True, {"connection_type": tipo, "description": t["description"], "json_schema": esquema, "example": _fixture(t["example"])})


def _miembro_json(i, m, nudo):
    cordon = i == nudo["chord"]
    angulo = 0.0 if cordon else nudo["angles"].get(i, m["angle"])
    desvio = 0.0 if cordon else nudo["offsets"].get(i, 0.0)
    o = nudo["origin"]
    return {"element_id": i, "family": m["family"], "type": m["type"], "structural_type": "Beam",
            "start_mm": m["start"], "end_mm": m["end"], "length_mm": round(_distancia(m["start"], m["end"]), 1), "slope_deg": 0.0,
            "angle_in_plane_deg": angulo, "angle_to_chord_deg": round(_inclinacion(angulo), 1),
            "side": "chord" if cordon else nudo["sides"].get(i, "+Y"), "node_end": 1, "material": "Steel", "is_chord": cordon,
            # Fase 12 (opción A): dónde corta el eje de la barra al del cordón (con signo a lo largo de +X desde el punto de trabajo), el
            # punto de corte, el hueco del extremo, si llega al nudo con la regla de medio canto, y el perfil (ancho, alto, pared).
            "axis_offset_mm": desvio, "axis_cut_mm": [round(o[0] + desvio, 1), o[1], o[2]], "end_gap_mm": 0.0, "reaches_node": True,
            "width_mm": m.get("width", 76.2), "height_mm": m.get("height", m.get("width", 76.2)), "wall_mm": m.get("wall", 6.35),
            # Ronda 12c: el giro de la sección (Cross-Section Rotation) del ejemplar; 90 en los tramos del cordón superior.
            "section_rotation_deg": m.get("rotation", 0.0)}


def _op_node_info(req):
    ids = req.get("element_ids")
    if ids is None:
        ids = sorted(MIEMBROS.keys())  # "selección actual" simulada
    if len(ids) < 2:
        return _sobre("node_info", False, errors=[_err("INVALID_REQUEST",
                      "Se requieren al menos 2 miembros de armazón estructural para inspeccionar el nudo (recibidos: {}).".format(len(ids)),
                      "element_ids", "Selecciona los miembros del nudo en Revit o pasa sus IDs en 'element_ids'.")])
    for i in ids:
        if not _barra(i):
            return _sobre("node_info", False, errors=[_err("ELEMENT_NOT_FOUND",
                          "No existe ningún elemento con ID {}.".format(i), "element_ids")])
    cordon = req.get("chord_element_id")
    nudo = _nudo_de(ids, cordon if _barra(cordon) else None)
    existentes = [{"connection_id": c["connection_id"], "connection_type": c["connection_type"],
                   "created_utc": c["created_utc"], "created_elements_count": len(c["created_element_ids"])}
                  for c in CONEXIONES.values() if set(c["spec"]["node"]["element_ids"]) & set(ids)]
    miembros = [_miembro_json(i, _barra(i), nudo) for i in ids]
    # Fase 13 (D7): dónde se partiría cada barra para el empalme del Detalle B (null en el cordón, en un empalme o si es paralela).
    for mj in miembros:
        corte, _ = (_punto_corte(mj["element_id"], nudo["chord"]) if (nudo["kind"] == "node" and mj["element_id"] != nudo["chord"]) else (None, None))
        mj["split_point_mm"] = corte["split_point_mm"] if corte else None
        mj["split_distance_mm"] = corte["split_distance_mm"] if corte else None
        mj["split_offset_from_chord_face_mm"] = OFFSET_B_MM if corte else None
    return _sobre("node_info", True, {
        # Fase 12: kind (node | splice) y eccentricity_mm (opción A) delante del marco de siempre; el contrato solo añade.
        "kind": nudo["kind"], "eccentricity_mm": nudo["eccentricity"],
        "origin_mm": nudo["origin"], "x_axis": [-1, 0, 0], "y_axis": [0, 0, 1], "z_axis": [0, 1, 0],
        "axis_distance_mm": 0.08, "chord_direction_reversed": True,
        "frame_rule": "canonical: X hacia +X global, Y hacia +Z global (arriba), Z = X x Y; angulos con signo desde +X en [-180, 180)",
        "chord_element_id": nudo["chord"],
        "members": miembros, "existing_connections": existentes})


def _op_find_profile(req):
    consulta = req.get("query") or req.get("name") or req.get("profile") or ""
    normalizada = _normalizar_perfil(consulta)
    coincidencias = [{"type_name": p, "family_name": p, "exact_match": p.upper() == consulta.upper()}
                     for p in PERFILES_MODELO if normalizada and _normalizar_perfil(p).startswith(normalizada)]
    sugerencias = [] if coincidencias or not consulta else PERFILES_MODELO[:3]
    return _sobre("find_profile", True, {"query": consulta, "total_profiles_in_model": len(PERFILES_MODELO),
                                         "matched_count": len(coincidencias), "matches": coincidencias,
                                         "suggestions": sugerencias})


def _validar(spec):
    errores, avisos = [], []
    tipo = spec.get("connection_type")
    if tipo is not None and tipo not in TIPOS:
        errores.append(_err("SCHEMA_INVALID", "connection_type desconocido.", "connection_type", "Tipos: " + ", ".join(TIPOS) + "."))
    for clave in TIPOS.get(tipo, TIPOS["gusset_node"])["required"]:
        if clave not in spec:
            errores.append(_err("SCHEMA_INVALID", "Falta el campo obligatorio '{}'.".format(clave), clave))
    for n, cadena in enumerate(spec.get("dimension_chains") or []):
        suma = sum(cadena.get("values_mm") or [])
        esperado = cadena.get("expected_total_mm") or 0
        if abs(suma - esperado) > 1.0:
            errores.append(_err("DIMENSION_CHAIN_MISMATCH",
                                "La cadena '{}' suma {} mm y debería dar {} mm.".format(cadena.get("label"), suma, esperado),
                                "dimension_chains[{}].values_mm".format(n), "Vuelve a leer las cotas del plano."))
    for n, duda in enumerate(spec.get("uncertain_fields") or []):
        if duda.get("user_confirmed_value") in (None, ""):
            errores.append(_err("UNRESOLVED_UNCERTAINTY",
                                "La duda en '{}' no tiene user_confirmed_value.".format(duda.get("path")),
                                "uncertain_fields[{}].user_confirmed_value".format(n), "Pregunta al usuario y confirma el valor."))
    for i in (spec.get("node") or {}).get("element_ids") or []:
        if not _barra(i):
            errores.append(_err("ELEMENT_NOT_FOUND", "No existe ningún elemento con ID {}.".format(i), "node.element_ids"))
    nudo = _nudo_de_spec(spec)
    # Fase 13: la cartela automática (outline.mode auto) no lleva puntos escritos ni márgenes negativos; si sale grande, aviso.
    outline = ((spec.get("gusset") or {}).get("outline") or {}) if isinstance(spec.get("gusset"), dict) else {}
    if outline.get("mode") == "auto":
        if outline.get("points_mm") is not None:
            errores.append(_err("SCHEMA_INVALID", "Con outline.mode 'auto' no se escriben points_mm: el contorno se calcula desde las barras.", "gusset.outline.points_mm",
                                "Quita points_mm (o cambia a mode 'polygon' para fijar tus puntos)."))
        for clave in ("margin_mm", "side_margin_mm", "beyond_chord_mm"):
            if outline.get(clave) is not None and outline[clave] < 0:
                errores.append(_err("SCHEMA_INVALID", "gusset.outline.{} no puede ser negativo.".format(clave), "gusset.outline." + clave))
        contorno = _contorno_auto(spec, nudo) if not errores else None
        if contorno and (contorno["width"] > MAX_CARTELA_AUTO_MM or contorno["height"] > MAX_CARTELA_AUTO_MM):
            avisos.append(_err("OUTLINE_AUTO_LARGE", "La cartela automática mide {} × {} mm (máximo {} mm por lado): alguna barra queda lejos o un margen es absurdo.".format(
                contorno["width"], contorno["height"], MAX_CARTELA_AUTO_MM), "gusset.outline", "Revisa los márgenes o convierte el contorno en polígono y recórtalo."))
    elif outline.get("mode") == "polygon" and len(outline.get("points_mm") or []) < 3 and spec.get("connection_type", "gusset_node") == "gusset_node" and "gusset" in spec:
        errores.append(_err("SCHEMA_INVALID", "Con outline.mode 'polygon' hacen falta al menos 3 puntos en points_mm.", "gusset.outline.points_mm"))
    # Fase 12 (opción A, decisión P2): hasta medio canto del cordón el desvío del eje es el aviso MEMBER_ECCENTRIC; por encima,
    # MEMBER_NOT_AT_NODE. Vale para los tres tipos (ValidationCommon del Core).
    if nudo and nudo["kind"] == "node" and nudo["eccentricity"] > 0:
        maximo = _medio_canto(nudo["chord"])
        for n, miembro in enumerate(spec.get("members") or []):
            eid = miembro.get("element_id")
            desvio = nudo["offsets"].get(eid, 0.0)
            if abs(desvio) < 0.05:
                continue
            if abs(desvio) > maximo:
                errores.append(_err("MEMBER_NOT_AT_NODE",
                                    "El eje del miembro [{}] corta el cordón a {:.1f} mm del punto de trabajo, más de medio canto del cordón ({:.1f} mm).".format(eid, abs(desvio), maximo),
                                    "members[{}].element_id".format(n), "Comprueba la selección del nudo o node_axis_max_offset_mm en limits.json."))
            else:
                avisos.append(_err("MEMBER_ECCENTRIC",
                                   "El eje del miembro [{}] corta el cordón a {:.1f} mm del punto de trabajo (nudo en K con excentricidad de {:.1f} mm; admitida hasta {:.1f} mm). Cada barra se recorta y se suelda sobre su propio eje.".format(
                                       eid, abs(desvio), nudo["eccentricity"], maximo),
                                   "members[{}].axis_offset_mm".format(n),
                                   "Es un nudo en K con excentricidad (opción A): no hay que corregir nada; por encima de medio canto del cordón sería MEMBER_NOT_AT_NODE."))
    for n, miembro in enumerate(spec.get("members") or []):
        pernos = ((miembro.get("attachment") or {}).get("bolts")) or {}
        if pernos.get("edge_mm") is not None and pernos["edge_mm"] < 22.0:
            errores.append(_err("BOLT_EDGE_DISTANCE_TOO_SMALL",
                                "La distancia al borde ({} mm) es menor que el mínimo configurado (22 mm).".format(pernos["edge_mm"]),
                                "members[{}].attachment.bolts.edge_mm".format(n), "Usa edge_mm >= 22."))
        eid = miembro.get("element_id")
        modelo = _barra(eid)
        angulo_modelo = nudo["angles"].get(eid, modelo["angle"]) if (nudo and modelo) else (modelo or {}).get("angle")
        esperado = miembro.get("expected_angle_deg")
        # Regla 8.6 desde la Fase 7 (ronda 7b en el simulador): se comparan las inclinaciones sin signo (45° = 135° = -45°).
        if angulo_modelo is not None and esperado is not None and abs(_inclinacion(angulo_modelo) - _inclinacion(esperado)) > 1.0:
            avisos.append(_err("ANGLE_DIFFERS_FROM_MODEL",
                               "El ángulo del plano ({:.1f}°, inclinación {:.1f}° respecto al cordón) difiere del de la barra en el modelo ({:.1f}°, inclinación {:.1f}°) por {:.1f}° > 1°.".format(
                                   esperado, _inclinacion(esperado), angulo_modelo, _inclinacion(angulo_modelo),
                                   abs(_inclinacion(angulo_modelo) - _inclinacion(esperado))),
                               "members[{}].expected_angle_deg".format(n)))
        if tipo == "direct_weld":
            soldadura = miembro.get("weld") or {}
            if soldadura.get("size_mm") is not None and soldadura["size_mm"] < 3.0:
                avisos.append(_err("WELD_BELOW_MINIMUM", "La soldadura de {} mm está por debajo del filete mínimo (3 mm).".format(soldadura["size_mm"]),
                                   "members[{}].weld.size_mm".format(n), "Usa al menos 3 mm (J2.4)."))
            if (miembro.get("end_gap_mm") or 0) < 0:
                errores.append(_err("SCHEMA_INVALID", "end_gap_mm no puede ser negativo.", "members[{}].end_gap_mm".format(n)))
    if tipo == "end_plate":
        _validar_placa_extremo(spec, nudo, errores, avisos)
    return errores, avisos


def _validar_placa_extremo(spec, nudo, errores, avisos):
    """Fase 12: las reglas propias de end_plate que imita el simulador (EndPlateValidator del Core tiene más)."""
    miembros = spec.get("members") or []
    cordon = (spec.get("chord") or {}).get("element_id")
    segundo = miembros[0].get("element_id") if miembros else None
    if nudo and nudo["kind"] != "splice":
        errores.append(_err("SPLICE_NOT_COLLINEAR",
                            "Los dos tramos ({} y {}) no son colineales: no forman un empalme.".format(cordon, segundo), "members[0].element_id",
                            "Una placa de extremo empalma dos tramos de la misma barra; con un quiebro mayor de 5° no es un empalme."))
    a, b = _barra(cordon), _barra(segundo)
    if a and b and a["type"] != b["type"]:
        avisos.append(_err("SPLICE_PROFILES_DIFFER",
                           "Los dos tramos son de perfiles distintos ({} y {}): la plancha se dimensiona con el mayor.".format(a["type"], b["type"]),
                           "members[0].profile", "Comprueba en el plano si el empalme cambia de perfil."))
    pernos = spec.get("bolts") or {}
    diametro = pernos.get("diameter_mm") or 0.0
    agujero = pernos.get("hole_mm")
    if agujero is not None and diametro and agujero < diametro + 1.0:
        errores.append(_err("BOLT_HOLE_TOO_SMALL",
                            "El agujero ({:.2f} mm) debe ser al menos 1.0 mm mayor que el perno ({:.2f} mm).".format(agujero, diametro), "bolts.hole_mm",
                            "Ø18 para 5/8\" y Ø13/16\" para 3/4\" en los detalles del plano."))
    for cara, fila in sorted((pernos.get("faces") or {}).items()):
        borde = (fila or {}).get("edge_mm")
        if borde is not None and borde < 22.0:
            errores.append(_err("BOLT_EDGE_DISTANCE_TOO_SMALL",
                                "La distancia al borde de la fila {} ({} mm) es menor que el mínimo configurado (22 mm).".format(cara, borde),
                                "bolts.faces.{}.edge_mm".format(cara), "Usa edge_mm >= 22."))
    espesor = (spec.get("plate") or {}).get("thickness_mm") or 0.0
    necesario = espesor + (spec.get("gap_mm") or 0.0) / 2.0
    for ruta, retiro, tramo in (("chord.end_setback_mm", (spec.get("chord") or {}).get("end_setback_mm"), "primer"),
                                ("members[0].end_setback_mm", (miembros[0] if miembros else {}).get("end_setback_mm"), "segundo")):
        if retiro is not None and retiro + 0.05 < necesario:
            errores.append(_err("SPLICE_SETBACK_TOO_SMALL",
                                "El retiro del {} tramo ({:.1f} mm) no deja sitio a su plancha ({:.1f} mm = espesor más media holgura).".format(tramo, retiro, necesario),
                                ruta, "Usa end_setback_mm >= {:.1f}.".format(necesario)))


def _op_validate(req):
    spec = _spec_de(req)
    if spec is None:
        return _sobre("validate", False, errors=[_err("SCHEMA_INVALID", "No se pudo deserializar la especificación.", "")])
    errores, avisos = _validar(spec)
    if errores:
        return _sobre("validate", False, errors=errores, warnings=avisos)
    return _sobre("validate", True, {
        "is_valid": True, "validation_token": _token_de(spec), "errors_count": 0, "warnings_count": len(avisos),
        "calculated_values": _valores_calculados(_nudo_de_spec(spec), spec), "bolt_stacks": []}, warnings=avisos)


def _resumen_preview(spec):
    pernos = cuchillas = soldaduras = 0
    modificar = []
    for m in spec.get("members") or []:
        a = m.get("attachment") or {}
        if a.get("type") == "bolted_knife_plate":
            cuchillas += 1
            b = a.get("bolts") or {}
            pernos += (b.get("rows") or 1) * (b.get("columns") or 1)
            soldaduras += 2
        elif a.get("type") == "welded_slot":
            soldaduras += 2
        if (m.get("end_setback_mm") or 0) > 0:
            modificar.append({"element_id": m.get("element_id"), "role": m.get("role"), "end": "end",
                              "current_end_distance_mm": 86.2, "setback_mm": m["end_setback_mm"],
                              "new_extension_mm": round(86.2 - m["end_setback_mm"], 1),
                              "action": "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"})
    return pernos, cuchillas, soldaduras, modificar


def _op_preview(req):
    spec = _spec_de(req)
    if spec is None:
        return _sobre("preview", False, errors=[_err("SCHEMA_INVALID", "Especificación nula.")])
    nudo = _nudo_de_spec(spec)
    origen = nudo["origin"] if nudo else ORIGEN_MM
    tipo = spec.get("connection_type")
    miembros = spec.get("members") or []
    comun = {"connection_type": tipo, "backend": "advancesteel", "chord_element_id": (spec.get("chord") or {}).get("element_id"),
             "first_member_element_id": (miembros[0] if miembros else {}).get("element_id"), "working_point_mm": origen}
    if tipo == "direct_weld":
        # Fase 12: sin cartela ni pernos; una soldadura por barra sobre su huella en la cara y el retiro por el eje (reserva hasta el sondeo 28: el 25 no encontró la viga de Advance Steel).
        modificar = [{"element_id": m.get("element_id"), "role": m.get("role"), "end": "end", "current_end_distance_mm": 38.1, "setback_mm": 38.1,
                      "new_extension_mm": 0.0, "action": "Retiro por el eje desde el corte de la barra con el eje del cordón (END_CUT_APPROXIMATED hasta el sondeo 28)"}
                     for m in miembros]
        resumen = dict(comun, eccentricity_mm=nudo["eccentricity"] if nudo else 0.0, gusset_plates=0, knife_plates=0, bolts=0, weld_lines=len(miembros),
                       members_modified=len(modificar), end_cut="axis_setback (END_CUT_APPROXIMATED hasta el sondeo 28)", dry_run=True)
        elementos = [{"kind": "weld_footprint", "member_element_id": m.get("element_id"), "size_mm": (m.get("weld") or {}).get("size_mm")} for m in miembros]
        return _sobre("preview", True, {"summary": resumen, "elements_to_create": elementos, "members_to_modify": modificar})
    if tipo == "end_plate":
        # Fase 12: dos planchas, los pernos por cara (BoltRing) y los dos tramos retirados desde el punto de empalme.
        pernos = sum((f or {}).get("count") or 0 for f in ((spec.get("bolts") or {}).get("faces") or {}).values())
        espesor = (spec.get("plate") or {}).get("thickness_mm") or 0.0
        modificar = [{"element_id": (spec.get("chord") or {}).get("element_id"), "role": "chord", "end": "end", "current_end_distance_mm": 0.0,
                      "setback_mm": (spec.get("chord") or {}).get("end_setback_mm") or espesor, "new_extension_mm": -((spec.get("chord") or {}).get("end_setback_mm") or espesor),
                      "action": "Retirar el tramo desde el punto de empalme para dejar sitio a su plancha"}]
        if miembros:
            modificar.append({"element_id": miembros[0].get("element_id"), "role": "chord", "end": "start", "current_end_distance_mm": 0.0,
                              "setback_mm": miembros[0].get("end_setback_mm") or espesor, "new_extension_mm": -(miembros[0].get("end_setback_mm") or espesor),
                              "action": "Retirar el tramo desde el punto de empalme para dejar sitio a su plancha"})
        resumen = dict(comun, kind=spec.get("kind") or "splice", gusset_plates=0, end_plates=2, knife_plates=0, bolts=pernos, weld_lines=8,
                       members_modified=len(modificar), gap_mm=spec.get("gap_mm") or 0.0, dry_run=True)
        elementos = [{"kind": "end_plate", "side": "A", "thickness_mm": espesor}, {"kind": "end_plate", "side": "B", "thickness_mm": espesor},
                     {"kind": "bolt_ring", "count": pernos, "nuts": (spec.get("bolts") or {}).get("nuts")}]
        return _sobre("preview", True, {"summary": resumen, "elements_to_create": elementos, "members_to_modify": modificar})
    pernos, cuchillas, soldaduras, modificar = _resumen_preview(spec)
    # Fase 13: la cartela con sus puntos (calculados con outline.mode auto, los escritos con polygon) y los mismos calculated_values que validate.
    contorno = _contorno_auto(spec, nudo)
    outline = (spec.get("gusset") or {}).get("outline") or {}
    puntos = contorno["points"] if contorno else (outline.get("points_mm") or [])
    return _sobre("preview", True, {
        "summary": dict(comun, gusset_plates=1, knife_plates=cuchillas, bolts=pernos, weld_lines=soldaduras, members_modified=len(modificar), dry_run=True),
        "elements_to_create": [{"kind": "gusset_plate", "outline_mode": "auto" if contorno else "polygon", "vertices_count": len(puntos), "outline_points_mm": puntos}],
        "members_to_modify": modificar, "calculated_values": _valores_calculados(nudo, spec)})


def _op_create(req, connection_id=None):
    nombre = "update" if connection_id else "create"
    token = (req.get("validation_token") or "").strip()
    if not token:
        return _sobre(nombre, False, errors=[_err("VALIDATION_TOKEN_INVALID",
                      "validation_token es obligatorio para crear una conexión.", "validation_token",
                      "Llama primero a conn_validate para validar la especificación y obtener el token.")])
    spec = _spec_de({k: v for k, v in req.items() if k != "validation_token"})
    if spec is None:
        return _sobre(nombre, False, errors=[_err("SCHEMA_INVALID", "Especificación nula.")])
    if token.lower() != _token_de(spec):
        return _sobre(nombre, False, errors=[_err("VALIDATION_TOKEN_INVALID",
                      "El validation_token no es válido o el modelo ha cambiado desde que se realizó la validación.",
                      "validation_token", "Vuelve a llamar a conn_validate.")])
    cid = connection_id or str(uuid.uuid4())
    ids = [1321345 + len(CONEXIONES) * 10 + n for n in range(9)]
    CONEXIONES[cid] = {"connection_id": cid, "spec_version": spec.get("spec_version", "1.0"),
                       "connection_type": spec.get("connection_type", "gusset_node"), "created_element_ids": ids,
                       "created_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "backend": "advancesteel",
                       "spec": spec}
    avisos = [_err("REVIT_WARNING", "Advertencia de Revit: The created elements are only visible in Detail Level: Fine.")]
    # Fase 12: las reservas de la fabricación que el add-in anuncia hasta que los sondeos 22 y 23 digan otra cosa.
    if spec.get("connection_type") == "direct_weld":
        avisos.append(_err("END_CUT_APPROXIMATED",
                           "El corte inclinado contra la cara del cordón no está disponible todavía (sondeos 22 y 25): cada barra se acortó por su eje hasta dejar su esquina fuera del cordón, con el retiro medido desde su propio corte con el eje del cordón.",
                           "members", "Informativo: el sondeo 28 de docs/instalacion/fase-12c.md busca la viga de Advance Steel y prueba BeamShortening; mientras, la soldadura va sobre la huella en la cara."))
    elif spec.get("connection_type") == "end_plate":
        avisos.append(_err("BOLT_NUTS_APPROXIMATED",
                           "La tuerca a ambos lados no está en la API de Advance Steel: pernos estándar a través de las dos planchas (cabeza en una, tuerca en la otra).",
                           "bolts.nuts", "Informativo (decisión D9): el sondeo 23 de docs/instalacion/fase-12.md decide si hay otra forma."))
    datos = {"connection_id": cid, "spec_version": CONEXIONES[cid]["spec_version"],
             "connection_type": CONEXIONES[cid]["connection_type"], "created_element_ids": ids,
             "created_elements_count": len(ids)}
    if connection_id:
        datos["updated_utc"] = CONEXIONES[cid]["created_utc"]
    else:
        datos["created_utc"] = CONEXIONES[cid]["created_utc"]
        datos["backend"] = "advancesteel"
    return _sobre(nombre, True, datos, warnings=avisos)


def _op_list(req):
    filtro = (req.get("batch_id") or "").strip() or None
    todas = [{k: c[k] for k in ("connection_id", "spec_version", "connection_type", "backend", "created_utc")}
             | {"created_elements_count": len(c["created_element_ids"]),
                "template_id": (c["spec"].get("source") or {}).get("template_id"),
                "batch_id": (c["spec"].get("source") or {}).get("batch_id")} for c in CONEXIONES.values()]
    lotes = {}
    for c in todas:
        if c["batch_id"]:
            lotes[c["batch_id"]] = lotes.get(c["batch_id"], 0) + 1
    items = [c for c in todas if filtro is None or (c["batch_id"] or "").lower() == filtro.lower()]
    return _sobre("list", True, {"connections_count": len(items), "total_count": len(todas), "batch_id": filtro, "batches": lotes, "connections": items})


def _no_encontrada(nombre, cid):
    return _sobre(nombre, False, errors=[_err("ELEMENT_NOT_FOUND",
                  "No se encontró ninguna conexión con ID '{}'.".format(cid), "connection_id",
                  "Usa conn_list para verificar las conexiones guardadas en el modelo.")])


def _cid(req, nombre):
    cid = (req.get("connection_id") or "").strip()
    if not cid:
        return None, _sobre(nombre, False, errors=[_err("INVALID_REQUEST", "connection_id es requerido.", "connection_id")])
    return cid, None


def _op_get(req):
    cid, error = _cid(req, "get")
    if error:
        return error
    c = CONEXIONES.get(cid)
    if not c:
        return _no_encontrada("get", cid)
    return _sobre("get", True, {k: c[k] for k in ("connection_id", "spec_version", "connection_type", "created_utc",
                                                  "created_element_ids", "backend", "spec")}
                  | {"created_elements_count": len(c["created_element_ids"])})


def _op_update(req):
    cid, error = _cid(req, "update")
    if error:
        return error
    if cid not in CONEXIONES:
        return _no_encontrada("update", cid)
    return _op_create({k: v for k, v in req.items() if k != "connection_id"}, connection_id=cid)


def _op_delete(req):
    cid, error = _cid(req, "delete")
    if error:
        return error
    c = CONEXIONES.pop(cid, None)
    if not c:
        return _no_encontrada("delete", cid)
    # Fase 13 (D8): los cortes que pidió esta conexión se deshacen si ningún otro los usa (si no, aviso SPLIT_IN_USE).
    avisos = []
    for corte in [r for r in list(CORTES.values()) if (r.get("connection_id") or "").lower() == cid.lower()]:
        error = _deshacer_corte(corte)
        if error:
            avisos.append(dict(error, message="El corte de la barra {} pedido por la conexión {} se queda: {}".format(corte["element_id"], cid, error["message"])))
    return _sobre("delete", True, {"deleted_connection_id": cid, "deleted_elements_count": len(c["created_element_ids"]),
                                   "restored_members_count": 3}, warnings=avisos)


# ---------------------------------------------------------------------------
# Partir la diagonal (Fase 13): split / unsplit sobre barras ficticias
# ---------------------------------------------------------------------------
def _corte_de_barra(eid):
    return next((r for r in CORTES.values() if r["element_id"] == eid or r["new_element_id"] == eid), None)


def _conexiones_que_usan(ids):
    return [c["connection_id"] for c in CONEXIONES.values() if set((c["spec"].get("node") or {}).get("element_ids") or []) & set(ids)]


def _registro_corte(r):
    return {k: r[k] for k in ("split_id", "element_id", "new_element_id", "chord_element_id", "split_point_mm", "split_distance_mm",
                              "offset_from_chord_face_mm", "original_start_mm", "original_end_mm", "mechanism", "approximated_parameters",
                              "connection_id", "batch_id", "created_utc")}


def _partir(eid, cid, offset, connection_id=None, batch_id=None):
    """El corte en el modelo ficticio: el original queda como tramo corto (del lado del cordón, conserva el id) y el tramo largo es una
    barra nueva con la misma sección; los dos forman un empalme (clase splice) para node_info. Devuelve (registro, None) o (None, error)."""
    corte, codigo = _punto_corte(eid, cid, offset)
    if corte is None:
        mensajes = {"ELEMENT_NOT_FOUND": "La barra {} o el cordón {} no están en el modelo.".format(eid, cid),
                    "NODE_AXES_PARALLEL": "La barra {} es paralela al cordón {}: su eje no cruza la cara y no hay dónde medir el empalme del Detalle B.".format(eid, cid),
                    "SPLIT_FAILED": "El punto de corte cae fuera de la barra {} (más corta que la distancia del Detalle B más la pila de planchas).".format(eid)}
        return None, _err(codigo, mensajes[codigo], "element_id")
    b = _barra(eid)
    nuevo = SIGUIENTE_ID_NUEVO[0]
    SIGUIENTE_ID_NUEVO[0] += 1
    original = {"start": list(b["start"]), "end": list(b["end"]), "splice": b.get("splice")}
    punto = corte["split_point_mm"]
    if corte["near_end"] == 1:
        largo = {"start": list(b["start"]), "end": list(punto)}
        b["start"] = list(punto)
    else:
        largo = {"start": list(punto), "end": list(b["end"])}
        b["end"] = list(punto)
    grupo = "S{}".format(eid)
    b["splice"] = grupo
    copia = dict(b, start=largo["start"], end=largo["end"], splice=grupo, chord=False)
    TODAS_LAS_BARRAS[nuevo] = copia
    registro = {"split_id": str(uuid.uuid4()), "element_id": eid, "new_element_id": nuevo, "chord_element_id": cid,
                "original_start_mm": original["start"], "original_end_mm": original["end"], "original_splice": original["splice"],
                "split_point_mm": punto, "offset_from_chord_face_mm": offset, "split_distance_mm": corte["split_distance_mm"],
                "mechanism": "copy_element_location_curve (simulado)", "approximated_parameters": [],
                "connection_id": connection_id, "batch_id": batch_id, "created_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "preview": corte}
    CORTES[registro["split_id"]] = registro
    return registro, None


def _deshacer_corte(registro, ignorar=()):
    """Devuelve None si se deshizo; si no, el error (SPLIT_IN_USE)."""
    usan = [c for c in _conexiones_que_usan([registro["element_id"], registro["new_element_id"]]) if c.lower() not in {i.lower() for i in ignorar}]
    if usan:
        return _err("SPLIT_IN_USE", "Los tramos {} y {} los usan las conexiones {}: no se puede deshacer el corte.".format(
            registro["element_id"], registro["new_element_id"], ", ".join(usan)), "element_id", "Borra antes esas conexiones (conn_delete o Borrar) y vuelve a conn_unsplit.")
    b = _barra(registro["element_id"])
    if b is not None:
        b["start"], b["end"] = list(registro["original_start_mm"]), list(registro["original_end_mm"])
        if registro.get("original_splice"):
            b["splice"] = registro["original_splice"]
        else:
            b.pop("splice", None)
    TODAS_LAS_BARRAS.pop(registro["new_element_id"], None)
    CORTES.pop(registro["split_id"], None)
    return None


def _vista_previa_corte(eid, cid, corte):
    return {"element_id": eid, "chord_element_id": cid, "member_type": (_barra(eid) or {}).get("type"), "chord_type": (_barra(cid) or {}).get("type"),
            "theta_deg": corte["theta_deg"], "face_distance_mm": corte["face_distance_mm"], "first_plate_distance_mm": corte["first_plate_distance_mm"],
            "split_distance_mm": corte["split_distance_mm"], "cut_point_mm": corte["cut_point_mm"], "split_point_mm": corte["split_point_mm"],
            "plate_thickness_mm": corte["plate_thickness_mm"], "gap_mm": corte["gap_mm"], "short_mm": corte["short_mm"], "long_mm": corte["long_mm"]}


def _describir_corte(eid, cid, corte):
    return "La barra {} llega al cordón {} con {}°: su eje cruza la cara a {} mm del corte de los ejes; la primera plancha del Detalle B va a {} mm y el corte de los dos elementos a {} mm (tramo corto {} mm, tramo largo {} mm).".format(
        eid, cid, corte["theta_deg"], corte["face_distance_mm"], corte["first_plate_distance_mm"], corte["split_distance_mm"], corte["short_mm"], corte["long_mm"]).replace(".", ",").replace(",,", ".")


def _op_split(req):
    eid, cid = req.get("element_id"), req.get("chord_element_id")
    if not isinstance(eid, int) or eid <= 0:
        return _sobre("split", False, errors=[_err("INVALID_REQUEST", "Falta element_id: la barra que se parte (la diagonal).", "element_id",
                                                    "Pasa element_id (la barra) y chord_element_id (el cordón al que llega); offset_from_chord_face_mm es opcional (330).")])
    if not isinstance(cid, int) or cid <= 0:
        return _sobre("split", False, errors=[_err("INVALID_REQUEST", "Falta chord_element_id: el cordón desde cuya cara se mide la distancia.", "chord_element_id",
                                                    "conn_get_node_info del nudo en K dice cuál es el cordón.")])
    if not _barra(eid) or not _barra(cid):
        return _sobre("split", False, errors=[_err("ELEMENT_NOT_FOUND", "No existe ningún elemento con ID {}.".format(eid if not _barra(eid) else cid), "element_id")])
    offset = req.get("offset_from_chord_face_mm")
    offset = float(offset) if isinstance(offset, (int, float)) and offset > 0 else OFFSET_B_MM
    existente = _corte_de_barra(eid)
    if existente and req.get("dry_run") is not True:
        return _sobre("split", False, errors=[_err("SPLIT_IN_USE", "La barra {} ya está partida (corte {}, tramos {} y {}).".format(
            eid, existente["split_id"], existente["element_id"], existente["new_element_id"]), "element_id", "Deshazlo con conn_unsplit antes de volver a partir.")])
    corte, codigo = _punto_corte(eid, cid, offset)
    if corte is None and existente:
        corte = existente["preview"]   # la barra ya está partida: el punto es el del corte hecho
    if corte is None:
        mensaje = {"NODE_AXES_PARALLEL": "La barra {} es paralela al cordón {}: su eje no cruza la cara y no hay dónde medir el empalme del Detalle B.".format(eid, cid),
                   "SPLIT_FAILED": "El punto de corte cae fuera de la barra {} (más corta que la distancia del Detalle B más la pila de planchas).".format(eid)}.get(codigo, "No se pudo calcular el corte.")
        return _sobre("split", False, errors=[_err(codigo, mensaje, "element_id", "El corte de la diagonal solo vale para barras que llegan al cordón con ángulo y largo suficiente.")])
    if req.get("dry_run") is True:
        return _sobre("split", True, {"dry_run": True, "preview": _vista_previa_corte(eid, cid, corte), "description": _describir_corte(eid, cid, corte),
                                      "already_split": _registro_corte(existente) if existente else None})
    registro, error = _partir(eid, cid, offset, req.get("connection_id"), req.get("batch_id"))
    if error:
        return _sobre("split", False, errors=[error])
    return _sobre("split", True, {"split": _registro_corte(registro), "preview": _vista_previa_corte(eid, cid, corte),
                                  "description": _describir_corte(eid, cid, corte) + " Hecho: tramo nuevo {}.".format(registro["new_element_id"]),
                                  "next": "conn_get_node_info con [{}, {}] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.".format(eid, registro["new_element_id"])})


def _op_unsplit(req):
    registro = None
    if (req.get("split_id") or "").strip():
        registro = CORTES.get(req["split_id"].strip())
        if registro is None:
            return _sobre("unsplit", False, errors=[_err("SPLIT_NOT_FOUND", "No hay ningún corte con split_id '{}'.".format(req["split_id"]), "split_id", "conn_unsplit sin parámetros lista los cortes del modelo.")])
    elif isinstance(req.get("element_id"), int):
        registro = _corte_de_barra(req["element_id"])
        if registro is None:
            return _sobre("unsplit", False, errors=[_err("SPLIT_NOT_FOUND", "La barra {} no está partida por el add-in (ningún corte la tiene como tramo).".format(req["element_id"]), "element_id", "conn_unsplit sin parámetros lista los cortes del modelo.")])
    else:
        return _sobre("unsplit", True, {"splits_count": len(CORTES), "splits": [_registro_corte(r) for r in CORTES.values()]})
    error = _deshacer_corte(registro)
    if error:
        return _sobre("unsplit", False, errors=[error])
    return _sobre("unsplit", True, {"unsplit": _registro_corte(registro), "complete": True,
                                    "description": "Corte deshecho: el tramo {} se borró y la barra {} recuperó su curva.".format(registro["new_element_id"], registro["element_id"])})


# ---------------------------------------------------------------------------
# Catálogo de plantillas (Fase 7), imitación simple: sin orientaciones en espejo, casado en orden
# ---------------------------------------------------------------------------
def _inclinacion(angulo):
    a = abs(((angulo + 180.0) % 360.0) - 180.0)
    return 180.0 - a if a > 90.0 else a


def _plantilla_de(spec, nombre, template_id=None, descripcion=None, tags=None, politica="warn", connection_id=None):
    plantilla_spec = copy.deepcopy(spec)
    ids_origen = [(plantilla_spec.get("chord") or {}).get("element_id")]
    nudo = _nudo_de_spec(spec)   # Fase 12: los ángulos con signo salen del nudo (en el K, los de K_ANGULOS; en un empalme, 0)
    plantilla_spec.pop("node", None)
    (plantilla_spec.get("chord") or {}).pop("element_id", None)
    patron = []
    for i, miembro in enumerate(plantilla_spec.get("members") or []):
        eid = miembro.pop("element_id", None)
        ids_origen.append(eid)
        miembro["slot"] = i
        miembro.pop("axis_offset_mm", None)   # Fase 12: el desvío del eje es del nudo, no de la plantilla
        modelo = _barra(eid) or {}
        angulo = nudo["angles"].get(eid, modelo.get("angle", 0.0)) if nudo else modelo.get("angle", 0.0)
        patron.append({"slot": i, "role": miembro.get("role"), "angle_deg": angulo, "side": "+Y" if angulo >= 0 else "-Y",
                       "profile": miembro.get("profile"), "model_type_name": modelo.get("type"), "profile_policy": politica})
    plantilla_spec["uncertain_fields"] = []
    (plantilla_spec.get("source") or {}).pop("template_id", None)
    (plantilla_spec.get("source") or {}).pop("drawing_image", None)
    return {
        "catalog_version": "1.0", "template_id": template_id or str(uuid.uuid4()), "name": nombre, "description": descripcion,
        "connection_type": spec.get("connection_type", "gusset_node"), "tags": list(tags or []),
        "created_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        "origin": {"connection_id": connection_id, "document": "HANGAR_PRUEBA_sondeo", "drawing": (spec.get("source") or {}).get("drawing"),
                   "drawing_image": (spec.get("source") or {}).get("drawing_image"),
                   "element_ids": [i for i in ids_origen if i is not None]},
        "chord_pattern": {"profile": (spec.get("chord") or {}).get("profile"), "continuous": (spec.get("chord") or {}).get("continuous", True),
                          "profile_policy": politica},
        "member_pattern": patron,
        # Fase 13 (D3): una plantilla con la cartela automática casa con más tolerancia (auto_angle_tolerance_deg de catalog.json).
        "matching": {"angle_tolerance_deg": 25.0 if ((plantilla_spec.get("gusset") or {}).get("outline") or {}).get("mode") == "auto" else 10.0, "allow_mirror": True}, "spec_template": plantilla_spec,
    }


def _entrada_catalogo(p):
    return {"template_id": p["template_id"], "name": p["name"], "description": p.get("description"), "connection_type": p["connection_type"],
            "tags": p.get("tags") or [], "members_count": len(p["member_pattern"]), "chord_profile": p["chord_pattern"].get("profile"),
            "pattern": "{} barra(s)".format(len(p["member_pattern"])), "created_utc": p["created_utc"],
            "origin_drawing": p["origin"].get("drawing"), "origin_document": p["origin"].get("document"),
            "file": "%LOCALAPPDATA%\\MotorConexiones\\catalogo\\" + p["template_id"] + ".json"}


def _op_catalog_list(req):
    plantillas = [_entrada_catalogo(p) for p in sorted(CATALOGO.values(), key=lambda p: p["name"].lower())]
    return _sobre("catalog_list", True, {"catalog_folder": r"%LOCALAPPDATA%\MotorConexiones\catalogo", "shared_catalog_folder": None,
                                         "templates_count": len(plantillas), "templates": plantillas})


def _plantilla_no_encontrada(nombre, tid):
    return _sobre(nombre, False, errors=[_err("TEMPLATE_NOT_FOUND", "No existe la plantilla '{}' en el catálogo.".format(tid), "template_id",
                                              "Usa conn_catalog_list para ver las plantillas disponibles.")])


def _op_catalog_get(req):
    tid = (req.get("template_id") or "").strip()
    if not tid:
        return _sobre("catalog_get", False, errors=[_err("INVALID_REQUEST", "template_id es obligatorio.", "template_id")])
    p = CATALOGO.get(tid)
    if not p:
        return _plantilla_no_encontrada("catalog_get", tid)
    return _sobre("catalog_get", True, {"template_id": tid, "name": p["name"], "file": _entrada_catalogo(p)["file"],
                                        "pattern": _entrada_catalogo(p)["pattern"], "template": p})


def _op_catalog_save(req):
    nombre = (req.get("name") or "").strip()
    if not nombre:
        return _sobre("catalog_save", False, errors=[_err("INVALID_REQUEST", "La plantilla necesita un nombre.", "name")])
    cid = (req.get("connection_id") or "").strip()
    if cid:
        c = CONEXIONES.get(cid)
        if not c:
            return _no_encontrada("catalog_save", cid)
        spec = c["spec"]
    else:
        spec = req.get("spec") if isinstance(req.get("spec"), dict) else None
        if spec is None:
            return _sobre("catalog_save", False, errors=[_err("INVALID_REQUEST", "Pasa connection_id o spec.", "connection_id")])
    for n, duda in enumerate(spec.get("uncertain_fields") or []):
        if duda.get("user_confirmed_value") in (None, ""):
            return _sobre("catalog_save", False, errors=[_err("TEMPLATE_HAS_OPEN_UNCERTAINTIES",
                          "La especificación tiene dudas sin confirmar ({}).".format(duda.get("path")), "uncertain_fields")])
    errores, avisos = _validar(spec)
    if errores:
        return _sobre("catalog_save", False, errors=[_err("TEMPLATE_SPEC_INVALID",
                      "La especificación no valida contra el modelo abierto: no se guarda como plantilla.", "spec")] + errores, warnings=avisos)
    existente = next((p for p in CATALOGO.values() if p["name"].lower() == nombre.lower()), None)
    if existente and not req.get("overwrite"):
        return _sobre("catalog_save", False, errors=[_err("TEMPLATE_EXISTS", "Ya existe la plantilla '{}' ({}).".format(existente["name"], existente["template_id"]),
                                                          "name", "Usa otro nombre o pasa overwrite: true para sustituirla.")])
    plantilla = _plantilla_de(spec, nombre, template_id=existente["template_id"] if existente else req.get("template_id"),
                              descripcion=req.get("description"), tags=req.get("tags"), politica=req.get("profile_policy") or "warn",
                              connection_id=cid or None)
    CATALOGO[plantilla["template_id"]] = plantilla
    return _sobre("catalog_save", True, {
        "template_id": plantilla["template_id"], "name": nombre, "file": _entrada_catalogo(plantilla)["file"], "shared_file": None,
        "members_count": len(plantilla["member_pattern"]), "chord_profile": plantilla["chord_pattern"]["profile"],
        "member_pattern": plantilla["member_pattern"], "matching": plantilla["matching"], "origin": plantilla["origin"]}, warnings=avisos)


def _op_catalog_delete(req):
    tid = (req.get("template_id") or "").strip()
    if not tid:
        return _sobre("catalog_delete", False, errors=[_err("INVALID_REQUEST", "template_id es obligatorio.", "template_id")])
    p = CATALOGO.pop(tid, None)
    if not p:
        return _plantilla_no_encontrada("catalog_delete", tid)
    return _sobre("catalog_delete", True, {"deleted_template_id": tid, "name": p["name"], "file": _entrada_catalogo(p)["file"]})


def _candidata(p, barras, nudo):
    """Fase 11: imitación de TemplateChooser para una plantilla: encaja si tiene tantas ranuras como barras. Fase 12 (ForNode): solo
    cuentan las plantillas de la clase del nudo (end_plate para un empalme; gusset_node o direct_weld para un nudo)."""
    tipo = p.get("connection_type") or "gusset_node"
    if (nudo["kind"] == "splice") != (tipo == "end_plate"):
        return None
    ranuras = sorted(p["member_pattern"], key=lambda r: r["slot"])
    if len(ranuras) != len(barras):
        return None
    desvios = [round(abs(nudo["angles"][eid] - r["angle_deg"]), 2) for r, eid in zip(ranuras, barras)]
    return {"template_id": p["template_id"], "name": p["name"], "connection_type": tipo, "orientation": "same", "is_mirrored": False,
            "max_deviation_deg": max(desvios) if desvios else 0.0, "score_deg": round(sum(desvios), 2),
            "description": "{} · igual · desvío máx. {}°".format(p["name"], str(max(desvios) if desvios else 0.0).replace(".", ","))}


def _op_catalog_apply(req):
    tid = (req.get("template_id") or "").strip()
    nombre = (req.get("template_name") or "").strip()
    ids = req.get("element_ids") or sorted(MIEMBROS.keys())
    for i in ids:
        if not _barra(i):
            return _sobre("catalog_apply", False, errors=[_err("ELEMENT_NOT_FOUND", "No existe ningún elemento con id {}.".format(i), "element_ids")])
    nudo = _nudo_de(ids, req.get("chord_element_id") if _barra(req.get("chord_element_id")) else None)
    cordon = nudo["chord"]
    barras = [i for i in ids if i != cordon]
    propuesta = None
    if not tid and not nombre:
        # Fase 11 (D9): sin template_id, la mejor de todas las plantillas (menor desvío; a igual desvío, la más reciente).
        if not CATALOGO:
            return _sobre("catalog_apply", False, errors=[_err("CATALOG_EMPTY", "El catálogo no tiene ninguna plantilla: no hay nada que aplicar.", "template_id",
                                                               "Crea primero un nudo y guárdalo como plantilla (conn_catalog_save o el botón Catálogo).")])
        candidatas = [c for c in (_candidata(q, barras, nudo) for q in sorted(CATALOGO.values(), key=lambda q: q.get("created_utc") or "", reverse=True)) if c]
        candidatas.sort(key=lambda c: (c["score_deg"], c["max_deviation_deg"]))
        propuesta = {"templates_count": len(CATALOGO), "chosen": candidatas[0] if candidatas else None, "candidates": candidatas,
                     "attempts": [] if candidatas else ["{} → faltan o sobran barras".format(q["name"]) for q in CATALOGO.values()],
                     "member_angles_deg": [nudo["angles"][b] for b in barras],
                     "summary_text": ("Plantilla propuesta: {}. Otras que encajan: {}.".format(candidatas[0]["description"], ", ".join(c["name"] for c in candidatas[1:]) or "ninguna")
                                      if candidatas else "Ninguna de las {} plantillas encaja con {} barras.".format(len(CATALOGO), len(barras)))}
        if not candidatas:
            return _sobre("catalog_apply", False, errors=[_err("TEMPLATE_NO_MATCH", propuesta["summary_text"], "element_ids",
                                                               "Comprueba que seleccionaste el cordón y todas las barras del nudo, crea una plantilla para este nudo o pasa template_id para forzar una.")],
                          data={"template_id": None, "name": None, "proposal": propuesta, "attempts": propuesta["attempts"]})
        tid = candidatas[0]["template_id"]
    p = CATALOGO.get(tid) if tid else next((q for q in CATALOGO.values() if q["name"].lower() == nombre.lower()), None)
    if not p:
        return _plantilla_no_encontrada("catalog_apply", tid or nombre)
    ranuras = sorted(p["member_pattern"], key=lambda r: r["slot"])
    if len(barras) < len(ranuras):
        return _sobre("catalog_apply", False, errors=[_err("TEMPLATE_NO_MATCH",
                      "La plantilla '{}' no casa con el nudo: ranura {} sin barra.".format(p["name"], len(barras)), "element_ids",
                      "Comprueba que seleccionaste el cordón y todas las barras del nudo.")],
                      data={"template_id": p["template_id"], "name": p["name"], "attempts": []})
    spec = copy.deepcopy(p["spec_template"])
    spec["chord"]["element_id"] = cordon
    asignaciones = []
    for ranura, miembro, eid in zip(ranuras, spec["members"], barras):
        miembro.pop("slot", None)
        miembro["element_id"] = eid
        angulo = nudo["angles"][eid]
        miembro["expected_angle_deg"] = round(_inclinacion(angulo), 1)
        if abs(nudo["offsets"].get(eid, 0.0)) >= 0.05:
            miembro["axis_offset_mm"] = nudo["offsets"][eid]   # Fase 12 (opción A): el desvío real del eje de esta barra
        asignaciones.append({"slot": ranura["slot"], "role": ranura["role"], "element_id": eid, "template_angle_deg": ranura["angle_deg"],
                             "model_angle_deg": angulo, "deviation_deg": round(abs(angulo - ranura["angle_deg"]), 2), "side": ranura["side"]})
    spec["node"] = {"element_ids": [cordon] + barras[:len(ranuras)]}
    if nudo["eccentricity"] > 0:
        spec["node"]["eccentricity_mm"] = nudo["eccentricity"]
    spec.setdefault("source", {})["template_id"] = p["template_id"]
    if (p.get("origin") or {}).get("drawing_image"):
        spec["source"]["drawing_image"] = p["origin"]["drawing_image"]  # Fase 11 (V5): el plano vuelve con la plantilla
    spec["uncertain_fields"] = []
    errores, avisos = _validar(spec)
    datos = {"template_id": p["template_id"], "name": p["name"], "proposal": propuesta,
             "node": {"chord_element_id": cordon, "element_ids": [cordon] + barras, "chord_direction_reversed": True, "kind": nudo["kind"],
                      "eccentricity_mm": nudo["eccentricity"]},
             "match": {"orientation": "same", "is_complete": True, "matched_count": len(asignaciones), "score_deg": 0.0, "max_deviation_deg": 0.0,
                       "assignments": asignaciones, "unmatched_slots": [], "unassigned_members": barras[len(ranuras):], "description": "same"},
             "spec": spec, "is_valid": not errores, "validation_token": None if errores else _token_de(spec),
             "errors_count": len(errores), "warnings_count": len(avisos),
             "calculated_values": dict(_valores_calculados(nudo, spec), frame_x=[1, 0, 0], frame_z=[0, -1, 0], chord_direction_reversed=True),
             "bolt_stacks": []}
    return _sobre("catalog_apply", not errores, datos, errors=errores, warnings=avisos)


# ---------------------------------------------------------------------------
# Plan de lote (Fase 8): imitación mínima. El modelo simulado tiene el nudo del Detalle D (la "selección actual"), así que el plan
# detecta un nudo (N1) con el cordón y las tres barras, lo casa con la primera plantilla que encaje y lo valida. Fase 12: también
# conoce las barras de BARRAS_FASE12: dos tramos del mismo empalme forman un nudo de clase splice (arranque end_plate del detalle
# de su perfil si ninguna plantilla end_plate encaja) y el cordón superior 1245533 con sus diagonales forma el nudo en K (arranque
# direct_weld si ninguna plantilla encaja, con el aviso MEMBER_ECCENTRIC). Las marcas no existen aquí (is_marked refleja la petición).
# La detección real la prueban las xUnit del Core.
# ---------------------------------------------------------------------------
DETALLE_POR_PERFIL = {"HSS12X8X1/2": ("C", "detalle-C.json"), "HSS3X3X1/4": ("I", "detalle-I.json"),
                      "HSS2-1-2X2-1-2X3-16 64x64": ("B", "detalle-B.json"), "HSS2-1/2X2-1/2X3/16": ("B", "detalle-B.json")}


def _detectar(ids):
    """Imitación mínima de NodeDetector (Fase 12): cada pareja de tramos del mismo empalme es un nudo de clase splice; las demás
    barras forman un nudo (el Detalle D o el nudo en K). Los nombres van por la X del punto de trabajo, como en el Core."""
    grupos, resto = {}, []
    for i in ids:
        g = (_barra(i) or {}).get("splice")
        if g:
            grupos.setdefault(g, []).append(i)
        else:
            resto.append(i)
    nudos = []
    for g in sorted(grupos):
        tramos = grupos[g]
        if len(tramos) >= 2:
            nudos.append(("splice", tramos[:2]))
        else:
            resto.extend(tramos)
    if len(resto) >= 2:
        nudos.append(("node", resto))
    nudos.sort(key=lambda n: _nudo_de(n[1])["origin"][0])
    return nudos


def _arranque(nudo_geo, cordon, barras):
    """Fase 12 (D4): la especificación de arranque del tipo que le toca al nudo, o None si no hay regla (SpecStarter.TypeFor del Core:
    empalme → end_plate del detalle de su perfil; todas las barras a un lado del cordón → direct_weld; a los dos lados → cartela, que
    el plan NO arranca: sigue siendo no_match como en la Fase 8)."""
    if nudo_geo["kind"] == "splice":
        return _arranque_empalme(cordon, barras[0], _barra(cordon)["type"], _barra(barras[0])["type"])
    lados = {nudo_geo["sides"].get(b, "+Y") for b in barras}
    if barras and len(lados) == 1:
        spec = _fixture("nudo-K-cordon-superior.json")
        if not spec:
            return None
        spec = copy.deepcopy(spec)
        plantilla_miembro = spec["members"][0]
        spec["node"] = {"element_ids": [cordon] + list(barras)}
        if nudo_geo["eccentricity"] > 0:
            spec["node"]["eccentricity_mm"] = nudo_geo["eccentricity"]
        spec["chord"]["element_id"] = cordon
        spec["chord"]["profile"] = _barra(cordon)["type"]
        spec["members"] = []
        for b in barras:
            m = copy.deepcopy(plantilla_miembro)
            m["element_id"] = b
            m["profile"] = _barra(b)["type"]
            m["expected_angle_deg"] = round(_inclinacion(nudo_geo["angles"][b]), 1)
            m.pop("axis_offset_mm", None)
            if abs(nudo_geo["offsets"].get(b, 0.0)) >= 0.05:
                m["axis_offset_mm"] = nudo_geo["offsets"][b]
            spec["members"].append(m)
        spec["source"] = {"drawing": "Arranque direct_weld para el nudo en K de {} (config/choices.json)".format(cordon)}
        spec["uncertain_fields"] = []
        return spec, "arranque direct_weld", "direct_weld"
    # Fase 13 (D2): barras a los dos lados → cartela con el contorno automático (outline.mode auto), como SpecStarter.FromNode del Core.
    spec = _fixture("detalle-D.json")
    if not spec or not barras:
        return None
    spec = copy.deepcopy(spec)
    plantilla_miembro = copy.deepcopy(spec["members"][0])
    spec["node"] = {"element_ids": [cordon] + list(barras)}
    spec["chord"]["element_id"] = cordon
    spec["chord"]["profile"] = _barra(cordon)["type"]
    spec["members"] = []
    for b in barras:
        m = copy.deepcopy(plantilla_miembro)
        m["element_id"] = b
        m["profile"] = _barra(b)["type"]
        m["expected_angle_deg"] = round(_inclinacion(nudo_geo["angles"][b]), 1)
        m.pop("axis_offset_mm", None)
        spec["members"].append(m)
    spec["gusset"]["outline"] = {"mode": "auto"}
    contorno = _contorno_auto(spec, nudo_geo)
    if contorno:
        spec["gusset"]["width_mm"], spec["gusset"]["height_mm"] = contorno["width"], contorno["height"]
    spec["dimension_chains"] = []
    spec["source"] = {"drawing": "Arranque gusset_node (cartela automática) para el nudo de {} (config/choices.json)".format(cordon)}
    spec["uncertain_fields"] = []
    return spec, "arranque gusset_node", "gusset_node"


def _arranque_empalme(cordon, tramo, tipo_cordon, tipo_tramo):
    """El arranque end_plate del detalle del perfil (Fase 12, D4) para un empalme; Fase 13: también el empalme B previsto de una diagonal partida."""
    detalle, archivo = DETALLE_POR_PERFIL.get(tipo_cordon, ("I", "detalle-I.json"))
    spec = _fixture(archivo)
    if not spec:
        return None
    spec = copy.deepcopy(spec)
    spec["node"] = {"element_ids": [cordon, tramo]}
    spec["chord"]["element_id"] = cordon
    spec["chord"]["profile"] = tipo_cordon
    spec["members"][0]["element_id"] = tramo
    spec["members"][0]["profile"] = tipo_tramo
    spec["source"] = {"drawing": "Arranque end_plate (Detalle {}) para el empalme de {} y {} (config/choices.json)".format(detalle, cordon, tramo)}
    spec["uncertain_fields"] = []
    return spec, "arranque end_plate (Detalle {})".format(detalle), "end_plate"


def _pasos_corte(nudo, plan_id, req):
    """Fase 13 (D5): por cada diagonal con splice_hint del nudo en K (direct_weld), el paso de corte y el empalme B previsto (N5B), con el
    id provisional negativo del tramo largo, como SplitPlanning del Core. Devuelve la lista de nudos previstos (los pasos quedan en el nudo)."""
    nudo["split_steps"] = []
    spec = nudo.get("spec") or {}
    if req.get("split_diagonals") is False or spec.get("connection_type") != "direct_weld" or nudo["status"] not in ("ready", "invalid"):
        return []
    previstos = []
    for m in spec.get("members") or []:
        offset = (m.get("splice_hint") or {}).get("offset_from_chord_face_mm") or 0
        if offset <= 0:
            continue
        eid = m.get("element_id")
        cordon = m.get("chord_element_id") or nudo["chord_element_id"]
        corte, codigo = _punto_corte(eid, cordon, offset)
        nombre_b = "{}B{}".format(nudo["name"], "" if not previstos else len(previstos) + 1)
        paso = {"element_id": eid, "chord_element_id": cordon, "offset_from_chord_face_mm": offset, "detail": "B",
                "split_point_mm": corte["split_point_mm"] if corte else None, "split_distance_mm": corte["split_distance_mm"] if corte else 0.0,
                "face_distance_mm": corte["face_distance_mm"] if corte else 0.0, "theta_deg": corte["theta_deg"] if corte else 0.0,
                "splice_node": nombre_b, "error": None if corte else {"NODE_AXES_PARALLEL": "la barra es paralela al cordón", "SPLIT_FAILED": "la barra es más corta que la distancia del Detalle B"}.get(codigo, codigo),
                "new_element_id": None, "split_id": None}
        paso["description"] = "Partir {} a {:g} (Detalle B)".format(eid, offset) + (": corte a {:g} mm del eje del cordón por el eje de la barra".format(paso["split_distance_mm"]) if corte else ": no se puede ({})".format(paso["error"]))
        nudo["split_steps"].append(paso)
        virtual = -abs(eid)
        tipo = (_barra(eid) or {}).get("type")
        arranque = _arranque_empalme(eid, virtual, tipo, tipo) if corte else None
        previsto = {"name": nombre_b, "status": "after_split", "status_detail": None, "work_point_mm": corte["split_point_mm"] if corte else nudo["work_point_mm"],
                    "chord_element_id": eid, "chord_continuous": False, "chord_type_name": tipo, "through_element_ids": [], "member_element_ids": [virtual],
                    "element_ids": [eid, virtual], "members": [{"element_id": virtual, "angle_deg": 0.0, "side": "chord", "type_name": tipo, "reaches_node": True,
                                                                "end_gap_mm": 0.0, "axis_offset_mm": 0.0, "chord_element_id": eid}],
                    "kind": "splice", "splice_element_id": None, "eccentricity_mm": 0.0, "connection_type": "end_plate", "starter": True,
                    "signature": "empalme previsto de la diagonal {}".format(eid), "is_manual": nudo.get("is_manual", False), "template_id": None,
                    "template_name": arranque[1] if arranque else None, "orientation": None, "is_mirrored": False, "max_deviation_deg": None, "match": None,
                    "attempts": [], "spec": None, "has_spec_override": False, "is_valid": False, "validation_token": None, "errors": [], "warnings": [],
                    "errors_count": 0, "warnings_count": 0, "existing_connection_id": None, "existing_batch_id": None, "replaces_existing": False,
                    "created_connection_id": None, "color_name": None, "color_rgb": None, "is_marked": False, "marker_element_id": None,
                    "label_index": None, "ghost_element_id": None, "split_steps": [], "split_of_node": nudo["name"]}
        if arranque is None:
            previsto["errors"].append(_err("SPLIT_FAILED", paso["error"] or "No hay arranque end_plate para el perfil {}.".format(tipo), "nodes[{}].split_steps".format(nudo["name"]),
                                           "Revisa la barra y offset_from_chord_face_mm."))
            previsto["status_detail"] = "No se puede prever el empalme: {}.".format(paso["error"] or "sin arranque")
        else:
            spec_b = arranque[0]
            spec_b.setdefault("source", {})["batch_id"] = plan_id
            spec_b["source"]["drawing"] = "Arranque end_plate · Detalle B tras partir {} a {:g}".format(eid, offset)
            previsto.update({"spec": spec_b, "is_valid": True,
                             "status_detail": "Empalme B de la diagonal {}: se crea después de partirla a {:g} (el tramo largo saldrá con un id nuevo; {} es provisional). config/choices.json.".format(eid, offset, virtual)})
        previsto["errors_count"] = len(previsto["errors"])
        previstos.append(previsto)
    return previstos


def _planificar_nudo(nombre, clase, ids_nudo, overrides, plantillas, anterior, req):
    """Un nudo del plan: detección, correcciones, casado con las plantillas de su clase y, sin plantilla, el arranque de su tipo."""
    forzado = overrides.get("chord", {}).get(nombre)
    nudo_geo = _nudo_de(ids_nudo, forzado if _barra(forzado) else None)
    cordon = nudo_geo["chord"]
    barras = [i for i in ids_nudo if i != cordon]
    es_empalme = clase == "splice"
    nudo = {"name": nombre, "status": "detected", "status_detail": None, "work_point_mm": nudo_geo["origin"], "chord_element_id": cordon,
            "chord_continuous": not es_empalme, "chord_type_name": _barra(cordon)["type"], "through_element_ids": [] if es_empalme else [cordon],
            "member_element_ids": barras, "element_ids": [cordon] + barras,
            "members": [{"element_id": b, "angle_deg": nudo_geo["angles"][b], "side": nudo_geo["sides"].get(b, "+Y"),
                         "type_name": _barra(b)["type"], "reaches_node": True, "end_gap_mm": 0.0,
                         "axis_offset_mm": nudo_geo["offsets"].get(b, 0.0), "chord_element_id": cordon} for b in barras],
            # Fase 12: la clase del nudo (node | splice), el otro tramo del empalme, la excentricidad medida, el tipo de conexión que
            # se creará y si la especificación es un arranque (sin plantilla).
            "kind": nudo_geo["kind"], "splice_element_id": barras[0] if es_empalme else None, "eccentricity_mm": nudo_geo["eccentricity"],
            "connection_type": None, "starter": False,
            "signature": "{} barra(s)".format(len(barras)), "is_manual": False, "template_id": None, "template_name": None,
            "orientation": None, "is_mirrored": False, "max_deviation_deg": None, "match": None, "attempts": [], "spec": None,
            "has_spec_override": False, "is_valid": False, "validation_token": None, "errors": [], "warnings": [],
            "errors_count": 0, "warnings_count": 0, "existing_connection_id": None, "existing_batch_id": None, "replaces_existing": False,
            "created_connection_id": None,
            "color_name": "rojo", "color_rgb": [214, 45, 45], "is_marked": bool(req.get("mark", True)), "marker_element_id": None,
            "label_index": None, "ghost_element_id": None,
            # Fase 13 (D5): los cortes de diagonal del nudo en K y, en un empalme B previsto, de qué nudo viene.
            "split_steps": [], "split_of_node": None}
    avisos = []
    if nombre in overrides.get("exclude", []):
        nudo["status"] = "excluded"
        nudo["status_detail"] = "Excluido por la persona."
        return nudo, avisos
    conexion = next((c for c in CONEXIONES.values() if set(c["spec"].get("node", {}).get("element_ids") or []) & set(ids_nudo)), None)
    if conexion and not overrides.get("replace_existing"):
        nudo["status"] = "already_connected"
        nudo["existing_connection_id"] = conexion["connection_id"]
        nudo["existing_batch_id"] = (conexion["spec"].get("source") or {}).get("batch_id")
        nudo["status_detail"] = "Ya tiene la conexión {} (se salta; replace_existing: true para rehacerla).".format(conexion["connection_id"])
        return nudo, avisos
    forzada = overrides.get("template", {}).get(nombre, "auto")
    if forzada is None:
        nudo["status"] = "no_match"
        nudo["status_detail"] = "Sin plantilla por decisión de la persona (template: null)."
        return nudo, avisos
    candidatas = plantillas if forzada == "auto" else [p for p in plantillas if p["template_id"] == forzada]
    if forzada != "auto" and not candidatas:
        nudo["status"] = "no_match"
        nudo["status_detail"] = "La plantilla '{}' no está entre las del plan.".format(forzada)
        return nudo, avisos
    mejor = None
    for p in candidatas:
        if _candidata(p, barras, nudo_geo) is None:   # Fase 12: ForNode (clase) + ranuras
            nudo["attempts"].append(p["name"] + " → sin encaje")
            continue
        r = _op_catalog_apply({"template_id": p["template_id"], "element_ids": ids_nudo, "chord_element_id": cordon})
        if r["data"] and r["data"].get("match") and r["data"]["match"]["is_complete"]:
            mejor = (p, r)
            break
        nudo["attempts"].append(p["name"] + " → sin encaje")
    batch_id = (anterior["plan_id"] if anterior else None) or "pendiente"
    if mejor is not None:
        p, r = mejor
        d = r["data"]
        spec = d["spec"]
        spec.setdefault("source", {})["batch_id"] = batch_id
        nudo.update({"template_id": p["template_id"], "template_name": p["name"], "connection_type": spec.get("connection_type", "gusset_node"),
                     "orientation": d["match"]["orientation"], "max_deviation_deg": d["match"]["max_deviation_deg"], "match": d["match"], "attempts": [],
                     "spec": spec, "is_valid": d["is_valid"], "validation_token": d["validation_token"], "errors": r["errors"], "warnings": r["warnings"],
                     "errors_count": len(r["errors"]), "warnings_count": len(r["warnings"]), "status": "ready" if d["is_valid"] else "invalid"})
        return nudo, list(r["warnings"])
    arranque = None if forzada != "auto" else _arranque(nudo_geo, cordon, barras)
    if arranque is None:
        nudo["status"] = "no_match"
        nudo["status_detail"] = ("No hay plantillas en el catálogo (o ninguna de las pedidas existe)." if not plantillas
                                 else "Ninguna plantilla casa todas sus ranuras con las barras del nudo.")
        return nudo, avisos
    spec, nombre_arranque, tipo = arranque
    spec.setdefault("source", {})["batch_id"] = batch_id
    errores, avisos_validacion = _validar(spec)
    if tipo == "gusset_node":
        # D2: el nudo con cartela sin plantilla se arranca con la cartela automática y lo dice como aviso TEMPLATE_NO_MATCH.
        avisos_validacion = [_err("TEMPLATE_NO_MATCH", "Ninguna plantilla encaja con este nudo: se creará con la cartela automática (arranque; outline.mode auto, calculada desde las barras). Revísala en Editar nudo o guárdala como plantilla.",
                                  "nodes[{}].template_id".format(nombre), "Si prefieres una cartela dibujada, crea la típica de este nudo o excluye el nudo.")] + avisos_validacion
    nudo.update({"template_id": None, "template_name": nombre_arranque, "connection_type": tipo, "starter": True, "attempts": [], "spec": spec,
                 "is_valid": not errores, "validation_token": None if errores else _token_de(spec), "errors": errores, "warnings": avisos_validacion,
                 "errors_count": len(errores), "warnings_count": len(avisos_validacion), "status": "ready" if not errores else "invalid",
                 "status_detail": "Sin plantilla: {} (config/choices.json).".format(nombre_arranque)})
    return nudo, list(avisos_validacion)


def _op_batch_plan(req):
    plan_id = (req.get("plan_id") or "").strip() or None
    anterior = PLANES.get(plan_id) if plan_id else None
    if plan_id and anterior is None:
        return _sobre("batch_plan", False, errors=[_err("PLAN_NOT_FOUND", "No hay ningún plan con plan_id '{}' en memoria.".format(plan_id), "plan_id",
                                                        "Vuelve a planificar sin plan_id con la selección de la cercha.")])
    ids = req.get("element_ids") or (anterior["selection_ids"] if anterior else sorted(MIEMBROS.keys()))
    # Fase 10 (C6): la selección asistida del simulador añade las demás barras del único nudo (todas "se tocan" en él).
    expansion = None
    if req.get("expand_selection") is True and ids:
        faltan = [i for i in sorted(MIEMBROS.keys()) if i not in ids]
        cordones = sum(1 for i in faltan if MIEMBROS[i]["chord"])
        expansion = {"requested_count": len(ids), "added_count": len(faltan), "chord_count": cordones, "member_count": len(faltan) - cordones,
                     "splice_count": 0, "added_element_ids": faltan, "skipped_out_of_plane": [], "ignored_element_ids": [i for i in ids if not _barra(i)],
                     "limit_reached": False, "rounds": 1 if faltan else 0, "summary_text": _texto_expansion(faltan, cordones, len(faltan) - cordones, 0)}
        ids = [i for i in ids if _barra(i)] + faltan
    if len(ids) < 2:
        return _sobre("batch_plan", False, errors=[_err("INVALID_REQUEST", "Se necesitan al menos 2 barras para planificar (recibidas: {}).".format(len(ids)), "element_ids")])
    for i in ids:
        if not _barra(i):
            return _sobre("batch_plan", False, errors=[_err("ELEMENT_NOT_FOUND", "No existe ningún elemento con id {}.".format(i), "element_ids")])
    overrides = dict(anterior["overrides"]) if anterior else {}
    nuevas = req.get("overrides") or {}
    if not isinstance(nuevas, dict):
        return _sobre("batch_plan", False, errors=[_err("INVALID_REQUEST", "overrides debe ser un objeto JSON.", "overrides")])
    for clave, valor in nuevas.items():
        if clave not in ("exclude", "include", "add_node", "chord", "template", "remove_member", "add_member", "merge", "split", "spec", "replace_existing"):
            return _sobre("batch_plan", False, errors=[_err("INVALID_REQUEST", "overrides.{} no es una corrección conocida.".format(clave), "overrides." + clave)])
        if clave == "include":
            overrides["exclude"] = [n for n in overrides.get("exclude", []) if n not in valor]
        elif clave == "exclude":
            overrides["exclude"] = sorted(set(overrides.get("exclude", [])) | set(valor))
        else:
            overrides[clave] = valor
    tids = req.get("template_ids") or (anterior["template_ids"] if anterior else [])
    plantillas = [CATALOGO[t] for t in tids if t in CATALOGO] if tids else list(CATALOGO.values())
    if tids and len(plantillas) != len(tids):
        return _plantilla_no_encontrada("batch_plan", [t for t in tids if t not in CATALOGO][0])
    avisos = []
    if expansion and expansion["added_count"] > 0:
        avisos.append(_err("SELECTION_EXPANDED", expansion["summary_text"], "element_ids",
                           "El plan se calculó con la selección ampliada (selection_count las cuenta; data.selection_expansion dice cuáles); sin expand_selection se planifica solo lo seleccionado."))
    if not plantillas:
        avisos.append(_err("CATALOG_EMPTY", CONSEJO_CATALOGO_VACIO + " (o con conn_catalog_save). Con el catálogo vacío ningún nudo puede casar: todos salen no_match.",
                           "template_ids", "Botón Catálogo > Guardar en catálogo desde una conexión del modelo, o conn_catalog_save desde la IA."))
    nudos = []
    pid = anterior["plan_id"] if anterior else str(uuid.uuid4())
    for n, (clase, ids_nudo) in enumerate(_detectar(ids)):
        nudo, avisos_nudo = _planificar_nudo("N{}".format(n + 1), clase, ids_nudo, overrides, plantillas, anterior, req)
        avisos = avisos_nudo + avisos
        nudos.append(nudo)
        nudos.extend(_pasos_corte(nudo, pid, req))   # Fase 13 (D5): los empalmes B previstos van detrás de su nudo en K
    marcados = [n for n in nudos if n["is_marked"]]
    plan = {"plan_id": pid, "document": "HANGAR_PRUEBA_sondeo",
            "created_utc": anterior["created_utc"] if anterior else time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
            "updated_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "selection_ids": list(ids), "template_ids": list(tids),
            "templates": {p["template_id"]: p["name"] for p in plantillas}, "overrides": overrides, "nodes": nudos,
            "last_report": anterior.get("last_report") if anterior else None,
            "unused_element_ids": [], "is_marked": bool(marcados), "marked_view_id": 1 if marcados else None,
            "marked_element_ids": sorted({i for n in marcados for i in n["element_ids"]}), "marker_element_ids": [],
            "selection_expansion": expansion, "label_view_id": None, "label_indices": {}}
    # Fase 10 (V3): con las marcas va la etiqueta de cada nudo (labels, por defecto sí); las cartelas fantasma (V2) no se imitan.
    if marcados and req.get("labels", True) is not False:
        plan["label_view_id"] = 1
        for indice, nudo in enumerate(marcados):
            plan["label_indices"][nudo["name"]] = indice
            nudo["label_index"] = indice
    for nudo in nudos:
        if nudo["spec"]:
            nudo["spec"]["source"]["batch_id"] = plan["plan_id"]
            if nudo["validation_token"]:
                nudo["validation_token"] = _token_de(nudo["spec"])
        _decorar_nudo(nudo, plan)
    plan["split_steps_requested"] = req.get("split_diagonals") is not False
    PLANES[plan["plan_id"]] = plan
    ULTIMO_PLAN[0] = plan["plan_id"]
    return _sobre("batch_plan", True, _plan_a_datos(plan, req.get("include_specs", True) is not False), warnings=avisos)


# Ronda 8c: lo que el Core calcula en PlanAdvice (status_text, advice, visible_by_default, color por estado y summary_text),
# imitado para el único nudo del simulador. El contrato solo añade claves; los valores de color_name pasan a ser los del estado.
CONSEJO_CATALOGO_VACIO = "No hay plantillas: crea primero la conexión de un nudo y guárdala con Guardar en catálogo"
COLORES_ESTADO = {"verde": [46, 160, 67], "ambar": [240, 160, 0], "rojo": [214, 45, 45], "gris": [140, 140, 140]}
TEXTO_ESTADO = {"invalid": "No valida", "ambiguous_chord": "Dos cordones posibles", "offset": "Los ejes no se cortan",
                "already_connected": "Ya tiene conexión", "excluded": "Excluido", "untyped": "Barra suelta (no es nudo)"}


def _decorar_nudo(nudo, plan):
    estado = nudo["status"]
    avisos = nudo.get("warnings") or []
    sin_cordon = any(a.get("code") == "NODE_CHORD_NOT_CONTINUOUS" for a in avisos)
    acciones = []
    if estado == "created":
        color, texto = ("ambar", "✔ Creada con aviso") if avisos else ("verde", "✔ Creada")
        consejo = "Creada: conexión {}… · Ver en Revit; Borrar el lote la quita".format((nudo.get("created_connection_id") or "?")[:8])
        acciones = [("show", "Ver en Revit")]
    elif estado == "failed":
        color, texto = "rojo", "✖ Falló al crear"
        error = (nudo.get("errors") or [{}])[-1]
        consejo = "Falló al crear ({}: {}): corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye".format(
            error.get("code"), (error.get("message") or "").rstrip("."))
        acciones = [("show", "Ver en Revit"), ("exclude", "Excluir")]
    elif estado == "ready":
        color, texto = ("ambar", "▲ Listo con aviso") if avisos else ("verde", "● Listo")
        if nudo.get("starter"):
            texto += " (arranque)"   # Fase 13 (D2): el arranque se distingue en el estado
        consejo = "—"
        if avisos:
            acciones = [("exclude", "Excluir")]
        for a in avisos:
            if a.get("code") == "TEMPLATE_PROFILE_DIFFERS":
                consejo = "El cordón tiene otro perfil que la plantilla: se creará con la misma cartela; exclúyelo si no quieres"
            elif a.get("code") == "TEMPLATE_ANGLE_DEVIATION":
                consejo = "Una barra se desvía de la plantilla: se creará con el ángulo real; míralo con Editar nudo o exclúyelo"
            elif a.get("code") == "MEMBER_ECCENTRIC":   # Fase 12
                consejo = "Nudo en K con excentricidad de {} mm: se crea igual, cada barra recortada sobre su propio eje".format(
                    str(nudo.get("eccentricity_mm") or 0).replace(".", ","))
            elif a.get("code") == "TEMPLATE_NO_MATCH":   # Fase 13 (D2)
                consejo = "Cartela automática (calculada desde las barras, outline.mode auto): revísala en Editar nudo o guárdala como plantilla"
            elif a.get("code") == "OUTLINE_AUTO_LARGE":
                consejo = "La cartela automática sale grande (una barra lejos o un margen absurdo): Editar nudo > Convertir en polígono y recortar, o excluir"
            else:
                consejo = "Se creará con el aviso {}: {}".format(a.get("code"), (a.get("message") or "").rstrip("."))
        if nudo.get("starter"):   # Fase 12 (D4): el plan dice que es un arranque y de qué tipo
            tipo = {"direct_weld": "soldadura directa", "end_plate": "placa de extremo"}.get(nudo.get("connection_type"), "cartela")
            prefijo = "Sin plantilla: se creará con el {} ({}); guárdala en el catálogo para reutilizarla".format(nudo.get("template_name") or "arranque", tipo)
            consejo = prefijo if consejo == "—" else prefijo + " · " + consejo
            acciones = [("edit", "Editar nudo"), ("exclude", "Excluir")]
    elif estado == "invalid":
        color, texto = "rojo", "✖ No valida"
        codigos = sorted({e.get("code") for e in (nudo.get("errors") or []) if e.get("code")})
        consejo = ("Una barra se sale de la cartela: Editar nudo y agrandarla, o excluir" if "PLATE_OUTSIDE_GUSSET" in codigos
                   else "No valida ({}): Editar nudo para corregirlo, o excluir".format(", ".join(codigos)))
        acciones = [("edit", "Editar nudo"), ("exclude", "Excluir")]
    elif estado == "no_match":
        color = "rojo"
        if nudo.get("kind") == "splice":   # Fase 12: un empalme sin plantilla end_plate ni arranque para su perfil
            texto = "✖ Empalme sin plantilla"
            consejo = "Empalme de los tramos {} y {}: ninguna plantilla end_plate encaja y no hay arranque para su perfil; crea esa típica, Editar nudo o excluye".format(
                nudo["chord_element_id"], ", ".join(str(i) for i in nudo["member_element_ids"]))
            acciones = [("edit", "Editar nudo"), ("exclude", "Excluir")]
        elif not plan["templates"]:
            texto, consejo = "✖ Sin plantillas en el catálogo", CONSEJO_CATALOGO_VACIO
            acciones = [("catalog", "Abrir catálogo")]
        elif sin_cordon:
            texto, consejo = "✖ Falta el cordón", "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…"
            acciones = [("chord", "Cordón…"), ("members", "Barras…")]
        elif (nudo.get("status_detail") or "").startswith("Sin plantilla por decisión"):
            texto, consejo = "✖ Sin plantilla que encaje", "Sin plantilla por decisión tuya: Plantilla… > automática para volver a casarlo"
            acciones = [("template", "Plantilla…")]
        else:
            texto = "✖ Sin plantilla que encaje"
            consejo = "Ninguna plantilla encaja ({} barras, ángulos {}): crea esa típica o excluye".format(
                len(nudo["members"]), ", ".join("{:g}°".format(m["angle_deg"]) for m in nudo["members"]))
            acciones = [("exclude", "Excluir"), ("template", "Plantilla…")]
    elif estado in ("ambiguous_chord", "offset"):
        color, texto = "rojo", "✖ " + TEXTO_ESTADO[estado]
        consejo = "Elige el cordón con Cordón…" if estado == "ambiguous_chord" else "Los ejes no se cortan: corrige el modelo o excluye"
        acciones = [("chord", "Cordón…")] if estado == "ambiguous_chord" else [("show", "Ver en Revit"), ("exclude", "Excluir")]
    elif estado == "after_split":
        # Fase 13 (D5): el empalme B previsto se crea solo tras partir la diagonal (verde) salvo que su arranque no valide (rojo).
        if nudo.get("errors"):
            color, texto = "rojo", "✖ Empalme B no valida"
            consejo = "El empalme B previsto no valida ({}): revisa el arranque end_plate del Detalle B o quita el splice_hint de {}".format(
                ", ".join(sorted({e.get("code") for e in nudo["errors"] if e.get("code")})), nudo.get("split_of_node") or "su nudo en K")
        else:
            color, texto = ("ambar" if avisos else "verde"), ("▲" if avisos else "●") + " Empalme B (tras partir)"
            consejo = "Se crea solo, después de partir la diagonal (paso previo de {}); nada que hacer".format(nudo.get("split_of_node") or "su nudo en K")
    elif estado == "untyped":
        color, texto, consejo = "gris", "○ Barra suelta (no es nudo)", "No es un nudo: nada que hacer"
    elif estado == "excluded":
        color, texto, consejo = "gris", "◌ Excluido", "Excluido: Incluir para volver a planificarlo"
        acciones = [("include", "Incluir")]
    elif estado == "already_connected" and nudo.get("existing_batch_id") and nudo.get("existing_batch_id") == plan.get("plan_id"):
        color, texto = "gris", "◌ Ya creada en este lote"
        consejo = "Creada en este lote (conexión {}…): Borrar el lote la quita; para rehacerla, Incluir (rehacer)".format((nudo.get("existing_connection_id") or "?")[:8])
        acciones = [("show", "Ver en Revit"), ("include_replace", "Incluir (rehacer)")]
    else:
        color, texto, consejo = "gris", "◌ " + TEXTO_ESTADO.get(estado, estado), "Se salta; Incluir (rehacer) replanifica con 'rehacer existentes' para rehacerla con el lote"
        acciones = [("include_replace", "Incluir (rehacer)")]
    if nudo.get("has_spec_override"):
        texto += " (editado)"
    nudo["actions"] = [{"key": k, "label": l} for k, l in acciones]
    visible = estado != "untyped" and not (estado == "no_match" and sin_cordon and len(nudo["element_ids"]) <= 2)
    nudo["status_text"] = texto
    nudo["advice"] = consejo
    nudo["visible_by_default"] = visible
    nudo["color_name"] = color if visible else None
    nudo["color_rgb"] = COLORES_ESTADO[color] if visible else None
    nudo["is_marked"] = bool(nudo.get("is_marked")) and visible and estado != "after_split"


def _resumen_texto(plan):
    nudos = plan["nodes"]
    listos = [n for n in nudos if n["status"] == "ready"]
    creados = [n for n in nudos if n["status"] == "created"]
    fallidos = [n for n in nudos if n["status"] == "failed"]
    en_lote = [n for n in nudos if n["status"] == "already_connected" and n.get("existing_batch_id") == plan.get("plan_id")]
    frases = []
    if creados or fallidos:
        texto = "Creada 1 conexión" if len(creados) == 1 else "Creadas {} conexiones".format(len(creados))
        con_aviso = sum(1 for n in creados if n.get("warnings"))
        if con_aviso:
            texto += " ({} con aviso)".format(con_aviso)
        if fallidos:
            texto += ", {} {}".format(len(fallidos), "falló" if len(fallidos) == 1 else "fallaron")
        frases.append(texto + ".")
        if listos:
            frases.append("Quedan {} {} sin crear.".format(len(listos), "lista" if len(listos) == 1 else "listas"))
        elif fallidos:
            frases.append("Pulsa Crear otra vez para reintentar {}.".format("la fallida" if len(fallidos) == 1 else "las fallidas"))
    elif listos:
        con_plantilla = [n for n in listos if not n.get("starter")]
        arranques = [n for n in listos if n.get("starter")]
        if not arranques:
            espejo = sum(1 for n in listos if n.get("is_mirrored"))
            plantilla = listos[0].get("template_name") or "especificación editada"
            frases.append("{} {} con {} ({} {}, {} en espejo).".format(
                "Se creará 1 conexión" if len(listos) == 1 else "Se crearán {} conexiones".format(len(listos)), "", plantilla,
                len(listos) - espejo, "igual" if len(listos) - espejo == 1 else "iguales", espejo).replace("  ", " "))
        else:
            # Fase 12: los arranques se cuentan aparte de las plantillas, como PlanAdvice del Core:
            # "Se crearán 3 conexiones: 1 con Nudo tipico Detalle D (1 igual, 0 en espejo), 1 soldadura directa (arranque), 1 empalme (arranque Detalle C)."
            partes = []
            if con_plantilla:
                espejo = sum(1 for n in con_plantilla if n.get("is_mirrored"))
                partes.append("{} con {} ({} {}, {} en espejo)".format(len(con_plantilla), con_plantilla[0].get("template_name") or "especificación editada",
                                                                      len(con_plantilla) - espejo, "igual" if len(con_plantilla) - espejo == 1 else "iguales", espejo))
            soldaduras = [n for n in arranques if n.get("connection_type") == "direct_weld"]
            if soldaduras:
                partes.append("{} {} (arranque)".format(len(soldaduras), "soldadura directa" if len(soldaduras) == 1 else "soldaduras directas"))
            empalmes = [n for n in arranques if n.get("connection_type") == "end_plate"]
            detalles = {}
            for n in empalmes:
                detalle = re.search(r"Detalle (\w+)", n.get("template_name") or "")
                detalles.setdefault(detalle.group(1) if detalle else "", []).append(n)
            for detalle, grupo in sorted(detalles.items(), key=lambda kv: (-len(kv[1]), kv[0])):
                partes.append("{} {} (arranque{})".format(len(grupo), "empalme" if len(grupo) == 1 else "empalmes", " Detalle " + detalle if detalle else ""))
            otros = [n for n in arranques if n.get("connection_type") not in ("direct_weld", "end_plate")]
            if otros:   # Fase 13 (D2): la cartela del arranque es la automática
                partes.append("{} {} (arranque)".format(len(otros), "cartela automática" if len(otros) == 1 else "cartelas automáticas"))
            frases.append("{}: {}.".format("Se creará 1 conexión" if len(listos) == 1 else "Se crearán {} conexiones".format(len(listos)), ", ".join(partes)))
        perfil = sum(1 for n in listos if any(a.get("code") == "TEMPLATE_PROFILE_DIFFERS" for a in n.get("warnings") or []))
        if perfil:
            frases.append("{} {} de perfil distinto.".format(perfil, "avisa" if perfil == 1 else "avisan"))
        excentricos = sum(1 for n in listos if any(a.get("code") == "MEMBER_ECCENTRIC" for a in n.get("warnings") or []))
        if excentricos:
            frases.append("{} {} de excentricidad.".format(excentricos, "avisa" if excentricos == 1 else "avisan"))
        grandes = sum(1 for n in listos if any(a.get("code") == "OUTLINE_AUTO_LARGE" for a in n.get("warnings") or []))
        if grandes:
            frases.append("{} {} (OUTLINE_AUTO_LARGE).".format(grandes, "cartela automática sale grande" if grandes == 1 else "cartelas automáticas salen grandes"))
        # Fase 13 (D5): los cortes de diagonal y los empalmes B que van con los nudos en K.
        cortes = sum(1 for n in listos for s in n.get("split_steps") or [] if s.get("error") is None and not s.get("new_element_id"))
        previstos = [n for n in nudos if n["status"] == "after_split"]
        validos = sum(1 for n in previstos if not n.get("errors"))
        if cortes:
            frase = "Además se {} a 330 (Detalle B) y se {} antes de sus nudos en K".format(
                "parte 1 diagonal" if cortes == 1 else "parten {} diagonales".format(cortes), "crea 1 empalme B" if validos == 1 else "crean {} empalmes B".format(validos))
            if len(previstos) - validos:
                frase += " ({} no {})".format(len(previstos) - validos, "valida" if len(previstos) - validos == 1 else "validan")
            frases.append(frase + ".")
    elif en_lote:
        frases.append("Nada que crear: {} (Borrar el lote las quita).".format(
            "1 conexión ya creada en este lote" if len(en_lote) == 1 else "{} conexiones ya creadas en este lote".format(len(en_lote))))
    elif not plan["templates"]:
        frases.append("Ningún nudo listo: no hay plantillas en el catálogo (crea primero la conexión de un nudo y guárdala con Guardar en catálogo).")
    else:
        causas = {"no_match": "ninguna plantilla encaja en {} nudo(s)", "invalid": "{} nudo(s) no valida(n)", "excluded": "{} nudo(s) excluido(s)",
                  "already_connected": "{} nudo(s) ya tiene(n) conexión"}
        estados = {}
        for n in nudos:
            estados[n["status"]] = estados.get(n["status"], 0) + 1
        estado, cuenta = max(estados.items(), key=lambda kv: kv[1])
        frases.append("Ningún nudo listo: " + causas.get(estado, "{} nudo(s) " + estado).format(cuenta) + ".")
    ocultos = [n for n in nudos if not n.get("visible_by_default", True)]
    if ocultos:
        frases.append("Ocultos: {} barras sueltas.".format(len(ocultos)))
    return " ".join(frases)


def _plan_a_datos(plan, incluir_specs):
    nudos = []
    for n in plan["nodes"]:
        copia = dict(n)
        if not incluir_specs:
            copia["spec"] = None
        nudos.append(copia)
    resumen = {}
    for n in plan["nodes"]:
        resumen[n["status"]] = resumen.get(n["status"], 0) + 1
    visibles = [n for n in plan["nodes"] if n.get("visible_by_default", True)]
    return {"plan_id": plan["plan_id"], "document": plan["document"], "created_utc": plan["created_utc"], "updated_utc": plan["updated_utc"],
            "selection_count": len(plan["selection_ids"]), "templates": plan["templates"], "summary": resumen,
            "description": "{} nudo(s): {}.".format(len(plan["nodes"]), ", ".join("{} {}".format(v, k) for k, v in resumen.items())),
            "ready_count": resumen.get("ready", 0),
            "creatable_count": resumen.get("ready", 0) + resumen.get("failed", 0), "created_count": resumen.get("created", 0),
            # Fase 13 (D5): cuántos cortes de diagonal quedan por hacer y cuántos empalmes B previstos hay.
            "split_step_count": sum(1 for n in plan["nodes"] if n["status"] in ("ready", "failed") for s in n.get("split_steps") or [] if s.get("error") is None and not s.get("new_element_id")),
            "after_split_count": resumen.get("after_split", 0),
            "failed_count": resumen.get("failed", 0),
            "created_in_batch_count": sum(1 for n in plan["nodes"] if n["status"] == "already_connected" and n.get("existing_batch_id") == plan["plan_id"]),
            "has_batch_connections": resumen.get("created", 0) > 0 or bool((plan.get("last_report") or {}).get("connection_ids")),
            "last_report": plan.get("last_report"),
            "summary_text": _resumen_texto(plan), "visible_count": len(visibles),
            "hidden_text": "" if len(visibles) == len(plan["nodes"]) else "{} barras sueltas".format(len(plan["nodes"]) - len(visibles)),
            "selection_expansion": plan.get("selection_expansion"),
            "is_marked": plan["is_marked"], "marked_view_id": plan["marked_view_id"],
            "marks": {"element_count": len(plan["marked_element_ids"]), "marker_element_ids": plan["marker_element_ids"], "ghost_count": 0},
            "labels": {"count": len(plan.get("label_indices") or {}), "view_id": plan.get("label_view_id")},
            "label_view_id": plan.get("label_view_id"), "label_indices": plan.get("label_indices") or {},
            "overrides": plan["overrides"], "unused_element_ids": plan["unused_element_ids"], "nodes": nudos}


def _texto_expansion(anadidas, cordones, barras, tramos):
    """El SummaryText de SelectionExpansion del Core, imitado."""
    if not anadidas:
        return "La selección ya está completa: ninguna barra más la toca."
    partes = []
    if cordones:
        partes.append("{} {}".format(cordones, "cordón que pasa de largo" if cordones == 1 else "cordones que pasan de largo"))
    if barras:
        partes.append("{} {}".format(barras, "barra que llega" if barras == 1 else "barras que llegan"))
    if tramos:
        partes.append("{} {}".format(tramos, "tramo de cordón" if tramos == 1 else "tramos de cordón"))
    lista = partes[0] if len(partes) == 1 else ", ".join(partes[:-1]) + " y " + partes[-1]
    cabeza = "Se añadió 1 barra que toca la selección" if len(anadidas) == 1 else "Se añadieron {} barras que tocan la selección".format(len(anadidas))
    return cabeza + ": " + lista + "."


def _plan_de(req, nombre):
    pid = (req.get("plan_id") or "").strip() or ULTIMO_PLAN[0]
    plan = PLANES.get(pid) if pid else None
    if plan is None:
        return None, _sobre(nombre, False, errors=[_err("PLAN_NOT_FOUND",
                            "No hay ningún plan en memoria." if not (req.get("plan_id") or "").strip() else "No hay ningún plan con plan_id '{}' en memoria.".format(pid),
                            "plan_id", "Planifica con conn_batch_plan (la selección de la cercha y, si quieres, template_ids).")])
    return plan, None


def _op_batch_plan_get(req):
    plan, error = _plan_de(req, "batch_plan_get")
    if error:
        return error
    nombre = (req.get("node") or "").strip()
    if nombre:
        nudo = next((n for n in plan["nodes"] if n["name"].lower() == nombre.lower()), None)
        if nudo is None:
            return _sobre("batch_plan_get", False, errors=[_err("INVALID_REQUEST", "El plan no tiene ningún nudo llamado '{}'.".format(nombre), "node")])
        return _sobre("batch_plan_get", True, {"plan_id": plan["plan_id"], "node": nudo})
    return _sobre("batch_plan_get", True, _plan_a_datos(plan, req.get("include_specs", True) is not False))


def _op_batch_plan_discard(req):
    if req.get("all"):
        n = len(PLANES)
        etiquetas = sum(len(p.get("label_indices") or {}) for p in PLANES.values())
        PLANES.clear()
        ULTIMO_PLAN[0] = None
        return _sobre("batch_plan_discard", True, {"discarded_plans": n, "removed_markers": 0, "remaining_markers": 0, "removed_labels": etiquetas, "remaining_labels": 0})
    plan, error = _plan_de(req, "batch_plan_discard")
    if error:
        return error
    PLANES.pop(plan["plan_id"], None)
    ULTIMO_PLAN[0] = next(iter(PLANES), None)
    return _sobre("batch_plan_discard", True, {"discarded_plan_id": plan["plan_id"], "removed_marks": len(plan["marked_element_ids"]),
                                               "removed_labels": len(plan.get("label_indices") or {}),
                                               "remaining_plans": len(PLANES), "remaining_markers": 0, "remaining_labels": 0})


# ---------------------------------------------------------------------------
# Crear por lotes (Fase 9), imitación: cada nudo del plan se crea con _op_create y su token; informe por nudo
# ---------------------------------------------------------------------------
def _informe_texto(informe):
    cabeza = "Lote {}: ".format(informe["batch_id"][:8])
    nudos = informe["nodes"]
    if informe["operation"] == "batch_delete":
        borradas = [n for n in nudos if n["outcome"] == "deleted"]
        deshechos = sum(1 for n in nudos if n["outcome"] == "unsplit")
        texto = cabeza + ("1 conexión borrada" if len(borradas) == 1 else "{} conexiones borradas".format(len(borradas)))
        if deshechos:
            texto += ", {} {}".format(deshechos, "corte de diagonal deshecho" if deshechos == 1 else "cortes de diagonal deshechos")
        texto += " ({} elementos, {} barras restauradas).".format(sum(n["elements_count"] for n in borradas), sum(n["restored_members_count"] for n in borradas))
        if borradas:
            texto += " Una sola entrada de deshacer (Ctrl+Z)."
        return texto
    creadas = [n for n in nudos if n["outcome"] in ("created", "created_with_warnings")]
    fallidas = [n for n in nudos if n["outcome"] == "failed"]
    saltadas = [n for n in nudos if n["outcome"] == "skipped"]
    if informe.get("stopped"):
        return cabeza + "se detuvo en {} y se revirtió todo (stop_on_error): {} conexión(es) revertida(s), {} fallida(s).".format(
            informe.get("stopped_at"), sum(1 for n in nudos if n["outcome"] == "rolled_back"), len(fallidas))
    texto = cabeza + ("1 conexión creada" if len(creadas) == 1 else "{} conexiones creadas".format(len(creadas)))
    con_aviso = sum(1 for n in creadas if n["outcome"] == "created_with_warnings" or n["warnings"])
    if con_aviso:
        texto += " ({} con aviso)".format(con_aviso)
    partidas = sum(1 for n in nudos if n["outcome"] == "split")
    if partidas:
        texto += ", {} {} (Detalle B)".format(partidas, "diagonal partida" if partidas == 1 else "diagonales partidas")
    if fallidas:
        texto += ", {} {} ({})".format(len(fallidas), "falló" if len(fallidas) == 1 else "fallaron",
                                       "; ".join("{}: {}: {}".format(n["node"], n["errors"][0]["code"], n["errors"][0]["message"].rstrip(".")) for n in fallidas))
    if saltadas:
        texto += ", {} {} ({})".format(len(saltadas), "saltada" if len(saltadas) == 1 else "saltadas", "; ".join("{}: {}".format(n["node"], n["reason"]) for n in saltadas[:4]))
    texto += "."
    if creadas:
        texto += " Una sola entrada de deshacer (Ctrl+Z)."
    return texto


def _informe_datos(informe, plan):
    nudos = informe["nodes"]
    datos = dict(informe)
    datos.update({
        "created_count": sum(1 for n in nudos if n["outcome"] in ("created", "created_with_warnings")),
        "updated_count": sum(1 for n in nudos if n["outcome"] == "updated"),
        "with_warnings_count": sum(1 for n in nudos if n["outcome"] == "created_with_warnings"),
        "failed_count": sum(1 for n in nudos if n["outcome"] == "failed"),
        "skipped_count": sum(1 for n in nudos if n["outcome"] == "skipped"),
        "rolled_back_count": sum(1 for n in nudos if n["outcome"] == "rolled_back"),
        "deleted_count": sum(1 for n in nudos if n["outcome"] == "deleted"),
        "split_count": sum(1 for n in nudos if n["outcome"] == "split"),
        "unsplit_count": sum(1 for n in nudos if n["outcome"] == "unsplit"),
        "deleted_elements_count": sum(n["elements_count"] for n in nudos if n["outcome"] == "deleted"),
        "restored_members_count": sum(n["restored_members_count"] for n in nudos if n["outcome"] == "deleted"),
        "connection_ids": [n["connection_id"] for n in nudos if n["outcome"] in ("created", "created_with_warnings", "updated")],
        "summary_text": _informe_texto(informe),
        "plan_summary": None, "plan_summary_text": None, "plan_creatable_count": None,
    })
    if plan is not None:
        resumen = {}
        for n in plan["nodes"]:
            resumen[n["status"]] = resumen.get(n["status"], 0) + 1
        datos["plan_summary"] = resumen
        datos["plan_summary_text"] = _resumen_texto(plan)
        datos["plan_creatable_count"] = resumen.get("ready", 0) + resumen.get("failed", 0)
    return datos


def _resultado(nombre, outcome, **extra):
    r = {"node": nombre, "kind": "node", "outcome": outcome, "connection_id": None, "template_name": None, "orientation": None, "elements_count": 0,
         "restored_members_count": 0, "duration_ms": 0, "reason": None, "errors": [], "warnings": [], "element_id": None, "new_element_id": None, "split_id": None}
    r.update(extra)
    return r


def _op_batch_create(req):
    for clave in req:
        if clave not in ("plan_id", "nodes", "stop_on_error", "include_specs", "split_members"):
            return _sobre("batch_create", False, errors=[_err("INVALID_REQUEST", "'{}' no es una clave de batch_create.".format(clave), clave)])
    pid = (req.get("plan_id") or "").strip()
    if not pid:
        return _sobre("batch_create", False, errors=[_err("INVALID_REQUEST", "Falta plan_id: el lote se crea a partir de un plan de conn_batch_plan.", "plan_id")])
    nudos = req.get("nodes")
    if not isinstance(nudos, list) or not nudos:
        return _sobre("batch_create", False, errors=[_err("INVALID_REQUEST", "Falta nodes: la lista de nudos a crear, cada uno con su validation_token del plan.", "nodes")])
    for i, n in enumerate(nudos):
        if not isinstance(n, dict) or not (n.get("node") or "").strip():
            return _sobre("batch_create", False, errors=[_err("INVALID_REQUEST", "Cada elemento de nodes debe ser un objeto {node, validation_token}.", "nodes[{}]".format(i))])
        if not (n.get("validation_token") or "").strip():
            return _sobre("batch_create", False, errors=[_err("INVALID_REQUEST", "El nudo {} viene sin validation_token.".format(n.get("node")), "nodes[{}].validation_token".format(i))])
    plan = PLANES.get(pid)
    if plan is None:
        return _sobre("batch_create", False, errors=[_err("PLAN_NOT_FOUND", "No hay ningún plan con plan_id '{}' en memoria (se descartó o Revit se reinició): no se crea nada.".format(pid), "plan_id")])
    parar = bool(req.get("stop_on_error"))
    informe = {"batch_id": pid, "plan_id": pid, "operation": "batch_create", "document": plan["document"],
               "started_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "finished_utc": None, "duration_ms": 1,
               "stop_on_error": parar, "stopped": False, "stopped_at": None, "undo_entries": "one", "nodes": []}
    vistos = set()
    creados = []
    partidos = []     # Fase 13: los cortes hechos en esta pasada (para revertirlos con stop_on_error)
    detenido = False
    partir = req.get("split_members") is not False
    for item in nudos:
        nombre = item["node"].strip()
        r = _resultado(nombre, "skipped")
        informe["nodes"].append(r)
        if detenido:
            r["reason"] = "no se intentó: el lote se detuvo en " + informe["stopped_at"]
            continue
        nudo = next((n for n in plan["nodes"] if n["name"].lower() == nombre.lower()), None)
        if nudo is None:
            r["reason"] = "el nudo no existe en el plan"
            r["errors"].append(_err("INVALID_REQUEST", "El plan {} no tiene ningún nudo llamado '{}'.".format(pid, nombre), "nodes"))
            continue
        r["template_name"], r["orientation"] = nudo.get("template_name"), nudo.get("orientation")
        if nudo["name"].lower() in vistos:
            r["reason"] = "repetido en la petición"
            continue
        vistos.add(nudo["name"].lower())
        if nudo["status"] == "created":
            r["reason"] = "ya creada en este lote (conexión {})".format(nudo.get("created_connection_id"))
            continue
        if nudo["status"] not in ("ready", "failed") or not nudo.get("validation_token"):
            r["reason"] = "no está listo: {} · {}".format(nudo.get("status_text"), nudo.get("advice"))
            continue
        # Fase 13 (D5): antes del nudo en K, sus cortes de diagonal y los empalmes B previstos (cada paso con su resultado en el informe).
        pasos = [s for s in nudo.get("split_steps") or [] if s.get("error") is None]
        if pasos and item["validation_token"].strip().lower() == (nudo["validation_token"] or "").lower():
            informe["nodes"].pop()   # el nudo en K va detrás de sus pasos, como en el Core
            fallo_paso = False
            for paso in pasos:
                rp = _resultado("{} · partir {}".format(nudo["name"], paso["element_id"]), "skipped", kind="split", element_id=paso["element_id"])
                informe["nodes"].append(rp)
                if not partir:
                    rp["reason"] = "sin partir: split_members es false"
                    continue
                if paso.get("new_element_id"):
                    rp["reason"] = "ya partida (tramo nuevo {})".format(paso["new_element_id"])
                    continue
                registro, error = _partir(paso["element_id"], paso["chord_element_id"], paso["offset_from_chord_face_mm"], None, pid)
                if error:
                    rp["outcome"], rp["reason"] = "failed", error["code"]
                    rp["errors"].append(error)
                    fallo_paso = True
                    break
                rp.update({"outcome": "split", "new_element_id": registro["new_element_id"], "split_id": registro["split_id"], "elements_count": 1})
                paso["new_element_id"], paso["split_id"] = registro["new_element_id"], registro["split_id"]
                partidos.append(paso)
            if partir and not fallo_paso:
                for paso in pasos:
                    previsto = next((n for n in plan["nodes"] if n["name"] == paso["splice_node"]), None)
                    rb = _resultado(paso["splice_node"], "skipped", kind="splice_b", element_id=paso["element_id"], new_element_id=paso.get("new_element_id"))
                    informe["nodes"].append(rb)
                    if previsto is None or not previsto.get("spec") or not paso.get("new_element_id"):
                        rb["reason"] = "el plan no tiene el empalme previsto {}".format(paso["splice_node"])
                        continue
                    if previsto["status"] == "created":
                        rb["reason"] = "ya creada en este lote (conexión {})".format(previsto.get("created_connection_id"))
                        continue
                    spec_b = json.loads(json.dumps(previsto["spec"]).replace(str(-abs(paso["element_id"])), str(paso["new_element_id"])))
                    creada_b = _op_create({"spec": spec_b, "validation_token": _token_de(spec_b)})
                    if not creada_b["ok"]:
                        rb["outcome"], rb["reason"] = "failed", creada_b["errors"][0]["code"]
                        rb["errors"].extend(creada_b["errors"])
                        fallo_paso = True
                        break
                    cid_b = creada_b["data"]["connection_id"]
                    rb.update({"outcome": "created", "connection_id": cid_b, "elements_count": creada_b["data"]["created_elements_count"], "template_name": previsto.get("template_name")})
                    previsto.update({"status": "created", "created_connection_id": cid_b, "spec": spec_b, "validation_token": _token_de(spec_b), "member_element_ids": [paso["new_element_id"]],
                                     "element_ids": [paso["element_id"], paso["new_element_id"]],
                                     "status_detail": "Creada en este lote tras partir {}: conexión {} ({} elementos; tramo nuevo {}).".format(paso["element_id"], cid_b, creada_b["data"]["created_elements_count"], paso["new_element_id"])})
                    _decorar_nudo(previsto, plan)
                    creados.append(previsto)
            informe["nodes"].append(r)
            if fallo_paso:
                r["outcome"], r["reason"] = "failed", "no se creó: falló su corte de diagonal"
                error = _err("SPLIT_FAILED", "El nudo {} no se crea: falló el corte de su diagonal.".format(nudo["name"]), "nodes[{}].split_steps".format(nudo["name"]))
                r["errors"].append(error)
                nudo["status"], nudo["errors"] = "failed", [dict(error, path="batch:" + error["path"])]
                nudo["status_detail"] = "Falló al crear (SPLIT_FAILED): " + error["message"].rstrip(".") + "."
                _decorar_nudo(nudo, plan)
                if parar:
                    detenido, informe["stopped"], informe["stopped_at"] = True, True, nudo["name"]
                continue
            if partir and any(p.get("new_element_id") for p in pasos):
                # La diagonal cambió de curva: el token del nudo en K se renueva contra el modelo de ahora (Revalidate del Core).
                nudo["validation_token"] = _token_de(nudo["spec"])
                item = dict(item, validation_token=nudo["validation_token"])
        if not isinstance(item.get("spec"), dict) and item["validation_token"].strip().lower() != (nudo["validation_token"] or "").lower():
            r["outcome"], r["reason"] = "failed", "token distinto del plan"
            error = _err("VALIDATION_TOKEN_INVALID", "El validation_token de {} no es el que tiene el plan: el plan se replanificó (o el token es de otro nudo).".format(nudo["name"]),
                         "nodes[{}].validation_token".format(nudo["name"]), "Relee el plan con conn_batch_plan_get y pasa el validation_token actual.")
            r["errors"].append(error)
            nudo["status"], nudo["errors"] = "failed", [dict(error, path="batch:" + (error["path"] or ""))]
            nudo["status_detail"] = "Falló al crear (VALIDATION_TOKEN_INVALID): " + error["message"].rstrip(".") + "."
            _decorar_nudo(nudo, plan)
            if parar:
                detenido, informe["stopped"], informe["stopped_at"] = True, True, nudo["name"]
            continue
        spec = item.get("spec") if isinstance(item.get("spec"), dict) else nudo["spec"]
        creada = _op_create({"spec": spec, "validation_token": item["validation_token"]})
        if not creada["ok"]:
            r["outcome"], r["reason"] = "failed", creada["errors"][0]["code"]
            r["errors"].extend(creada["errors"])
            nudo["status"], nudo["errors"] = "failed", [dict(e, path="batch:" + (e.get("path") or "")) for e in creada["errors"]]
            nudo["status_detail"] = "Falló al crear ({}): {}.".format(creada["errors"][0]["code"], creada["errors"][0]["message"].rstrip("."))
            _decorar_nudo(nudo, plan)
            if parar:
                detenido, informe["stopped"], informe["stopped_at"] = True, True, nudo["name"]
            continue
        cid = creada["data"]["connection_id"]
        r.update({"outcome": "created_with_warnings" if (nudo.get("warnings") or creada["warnings"]) else "created", "connection_id": cid,
                  "elements_count": creada["data"]["created_elements_count"], "warnings": list(creada["warnings"])})
        nudo.update({"status": "created", "created_connection_id": cid, "errors": [], "is_marked": False,
                     "status_detail": "Creada en este lote: conexión {} ({} elementos).".format(cid, creada["data"]["created_elements_count"])})
        plan["marked_element_ids"] = [i for i in plan["marked_element_ids"] if i not in nudo["element_ids"]]
        # Fase 10: la etiqueta del nudo creado se quita con sus marcas.
        nudo["label_index"] = None
        (plan.get("label_indices") or {}).pop(nudo["name"], None)
        if not plan.get("label_indices"):
            plan["label_view_id"] = None
        _decorar_nudo(nudo, plan)
        creados.append(nudo)
    if detenido:
        for nudo in creados:
            CONEXIONES.pop(nudo["created_connection_id"], None)
            for r in informe["nodes"]:
                if r["node"] == nudo["name"]:
                    r["outcome"] = "rolled_back"
            nudo.update({"status": "after_split" if nudo.get("split_of_node") else "ready", "created_connection_id": None, "status_detail": None})
            _decorar_nudo(nudo, plan)
        for paso in partidos:   # Fase 13: los cortes de esta pasada vuelven atrás con el grupo exterior
            registro = CORTES.get(paso.get("split_id") or "")
            if registro:
                _deshacer_corte(registro)
            for r in informe["nodes"]:
                if r.get("kind") == "split" and r.get("element_id") == paso["element_id"] and r["outcome"] == "split":
                    r["outcome"] = "rolled_back"
            paso["new_element_id"], paso["split_id"] = None, None
    plan["is_marked"] = bool(plan["marked_element_ids"]) or bool(plan["marker_element_ids"])
    informe["finished_utc"] = time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime())
    plan["last_report"] = _informe_datos(informe, None)
    plan["updated_utc"] = informe["finished_utc"]
    datos = _informe_datos(informe, plan)
    avisos = [_err("BATCH_NODE_FAILED", "{}: falló ({}: {})".format(n["node"], n["errors"][0]["code"], n["errors"][0]["message"].rstrip(".")), "nodes[{}]".format(n["node"]))
              for n in informe["nodes"] if n["outcome"] == "failed"]
    if informe["stopped"]:
        return _sobre("batch_create", False, datos, errors=[_err("BATCH_STOPPED", "El lote se detuvo en {} y se revirtió entero (stop_on_error): {}".format(
            informe["stopped_at"], datos["summary_text"]), "nodes[{}]".format(informe["stopped_at"]))], warnings=avisos)
    return _sobre("batch_create", True, datos, warnings=avisos)


def _op_batch_delete(req):
    bid = (req.get("batch_id") or req.get("plan_id") or "").strip()
    if not bid:
        return _sobre("batch_delete", False, errors=[_err("INVALID_REQUEST", "Falta batch_id: el lote que se quiere borrar (es el plan_id del plan que lo creó).", "batch_id")])
    plan = PLANES.get(bid)
    informe = {"batch_id": bid, "plan_id": plan["plan_id"] if plan else None, "operation": "batch_delete", "document": "HANGAR_PRUEBA_sondeo",
               "started_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()), "finished_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
               "duration_ms": 1, "stop_on_error": False, "stopped": False, "stopped_at": None, "undo_entries": "one", "nodes": []}
    del_lote = [c for c in list(CONEXIONES.values()) if ((c["spec"].get("source") or {}).get("batch_id") or "").lower() == bid.lower()]
    cortes_lote = [r for r in list(CORTES.values()) if (r.get("batch_id") or "").lower() == bid.lower()]
    avisos = []
    if not del_lote and not cortes_lote:
        avisos.append(_err("BATCH_EMPTY", "No hay ninguna conexión con batch_id '{}' en el modelo: nada que borrar.".format(bid), "batch_id"))
    for c in del_lote:
        nombre = None
        if plan:
            nombre = next((n["name"] for n in plan["nodes"] if n.get("created_connection_id") == c["connection_id"] or n.get("existing_connection_id") == c["connection_id"]), None)
        borrada = _op_delete({"connection_id": c["connection_id"]})
        informe["nodes"].append(_resultado(nombre, "deleted" if borrada["ok"] else "failed", connection_id=c["connection_id"],
                                           elements_count=len(c["created_element_ids"]), restored_members_count=3, errors=list(borrada["errors"])))
    # Fase 13 (D8): los cortes de diagonal del lote se deshacen después de sus conexiones (los más recientes primero).
    deshechos = set()
    for registro in sorted(cortes_lote, key=lambda r: r["created_utc"], reverse=True):
        nombre = None
        if plan:
            k = next((n for n in plan["nodes"] if any(s.get("element_id") == registro["element_id"] for s in n.get("split_steps") or [])), None)
            nombre = (k["name"] + " · " if k else "") + "partir {}".format(registro["element_id"])
        rc = _resultado(nombre or "partir {}".format(registro["element_id"]), "unsplit", kind="split", element_id=registro["element_id"],
                        new_element_id=registro["new_element_id"], split_id=registro["split_id"], elements_count=1)
        error = _deshacer_corte(registro)
        if error:
            rc["outcome"], rc["errors"] = "failed", [error]
        else:
            deshechos.add(registro["element_id"])
        informe["nodes"].append(rc)
    if plan:
        borradas = {n["connection_id"] for n in informe["nodes"] if n["outcome"] == "deleted"}
        for nudo in plan["nodes"]:
            for paso in nudo.get("split_steps") or []:
                if paso.get("element_id") in deshechos:
                    paso["new_element_id"], paso["split_id"] = None, None
            if nudo["status"] == "created" and nudo.get("created_connection_id") in borradas:
                nudo.update({"status": "after_split" if nudo.get("split_of_node") else "ready", "created_connection_id": None,
                             "status_detail": "Su conexión se borró con el lote: se vuelve a crear con el corte de {}.".format(nudo.get("split_of_node")) if nudo.get("split_of_node") else None})
                _decorar_nudo(nudo, plan)
            elif nudo["status"] == "already_connected" and nudo.get("existing_connection_id") in borradas:
                nudo.update({"existing_connection_id": None, "existing_batch_id": None, "status_detail": "Su conexión se borró con el lote: replanifica para volver a planificarlo."})
                _decorar_nudo(nudo, plan)
        plan["last_report"] = _informe_datos(informe, None)
    return _sobre("batch_delete", True, _informe_datos(informe, plan), warnings=avisos)


OPERACIONES = {
    "ping": (_op_ping, False), "guide": (_op_guide, False), "types": (_op_types, False), "schema": (_op_schema, False),
    "node_info": (_op_node_info, True), "find_profile": (_op_find_profile, True), "validate": (_op_validate, True),
    "preview": (_op_preview, True), "create": (_op_create, True), "list": (_op_list, True), "get": (_op_get, True),
    "update": (_op_update, True), "delete": (_op_delete, True),
    "catalog_list": (_op_catalog_list, False), "catalog_get": (_op_catalog_get, False), "catalog_save": (_op_catalog_save, True),
    "catalog_delete": (_op_catalog_delete, False), "catalog_apply": (_op_catalog_apply, True),
    "batch_plan": (_op_batch_plan, True), "batch_plan_get": (_op_batch_plan_get, False), "batch_plan_discard": (_op_batch_plan_discard, True),
    "batch_create": (_op_batch_create, True), "batch_delete": (_op_batch_delete, True),
    "split": (_op_split, True), "unsplit": (_op_unsplit, True),
}


def bridge_handle(operation, request_json, doc, uidoc):
    """Imitación de MotorConexiones.Revit.Bridge.Handle: devuelve siempre el sobre común serializado."""
    nombre = operation or ""
    try:
        req = json.loads(request_json) if (request_json or "").strip() else {}
    except ValueError as error:
        return json.dumps(_sobre(nombre, False, errors=[_err("INVALID_REQUEST",
                          "La petición no es JSON válido: {}".format(error), None,
                          "Envía un objeto JSON, por ejemplo {\"element_ids\": [111, 222]}.")]), ensure_ascii=False)
    if not isinstance(req, dict):
        return json.dumps(_sobre(nombre, False, errors=[_err("INVALID_REQUEST", "La petición debe ser un objeto JSON.")]),
                          ensure_ascii=False)
    LLAMADAS.append((nombre, req))
    if nombre not in OPERACIONES:
        return json.dumps(_sobre(nombre, False, errors=[_err("UNKNOWN_OPERATION",
                          "La operación '{}' no existe en el add-in.".format(nombre), None,
                          "Operaciones disponibles: " + ", ".join(sorted(OPERACIONES)) + ".")]), ensure_ascii=False)
    funcion, necesita_doc = OPERACIONES[nombre]
    if necesita_doc and doc is None:
        return json.dumps(_sobre(nombre, False, errors=[_err("NO_DOCUMENT", "No hay ningún documento abierto en Revit.",
                          None, "Abre el modelo y vuelve a intentarlo.")]), ensure_ascii=False)
    return json.dumps(funcion(req), ensure_ascii=False)


# ---------------------------------------------------------------------------
# pyRevit, clr y System simulados
# ---------------------------------------------------------------------------
class _Respuesta:
    def __init__(self, data=None, status=200):
        self.data = data
        self.status = status


class _Peticion:
    def __init__(self, metodo, ruta, data, query_params):
        self.method = metodo
        self.path = ruta
        self.data = data
        self.params = data
        self.query_params = query_params
        self._headers = {}


class _API:
    """Imita pyrevit.routes.API: guarda (patrón, métodos, manejador, nombres de parámetros de ruta)."""

    def __init__(self, nombre):
        self.nombre = nombre
        self.rutas = []

    def route(self, patron, methods=("GET",)):
        def decorador(funcion):
            expresion = "^" + re.sub(r"<(\w+)>", r"(?P<\1>[^/]+)", patron).rstrip("/") + "/?$"
            self.rutas.append((patron, [m.upper() for m in methods], re.compile(expresion), funcion))
            return funcion
        return decorador

    def despachar(self, metodo, ruta, data, query_params, doc, uidoc, uiapp):
        """Como pyRevit: elige la ruta, filtra los argumentos por nombre y llama al manejador."""
        for patron, metodos, expresion, funcion in self.rutas:
            m = expresion.match(ruta)
            if not m:
                continue
            if metodo.upper() not in metodos:
                return _Respuesta({"error": "method not allowed"}, 405)
            peticion = _Peticion(metodo, ruta, data, query_params)
            disponibles = {"doc": doc, "uidoc": uidoc, "uiapp": uiapp, "request": peticion}
            disponibles.update({k: unquote(v) for k, v in m.groupdict().items()})
            nombres = inspect.getfullargspec(funcion).args
            kwargs = {n: disponibles[n] for n in nombres if n in disponibles}
            resultado = funcion(**kwargs)
            if isinstance(resultado, _Respuesta):
                return resultado
            return _Respuesta(resultado, 200)
        return _Respuesta({"error": "Route not found: {} {}".format(metodo, ruta)}, 404)


class _Ensamblado:
    class _Nombre:
        Name = "MotorConexiones.Revit"

    Location = "simulador://MotorConexiones.Revit.dll"

    def GetName(self):
        return self._Nombre()

    def GetType(self, nombre):
        return _Tipo() if nombre == "MotorConexiones.Revit.Bridge" else None


class _Tipo:
    def GetMethod(self, nombre, firma):
        return _Metodo() if nombre == "Handle" and len(firma) == 4 else None


class _Metodo:
    def Invoke(self, instancia, argumentos):
        return bridge_handle(argumentos[0], argumentos[1], argumentos[2], argumentos[3])


class _Documento:
    Title = "HANGAR_PRUEBA_sondeo"
    PathName = r"D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt"
    IsModifiable = False


def _instalar_modulos_falsos(extension):
    """Registra pyrevit, seguridad (real si hay extensión), clr, System y StringIO en sys.modules."""
    pyrevit = types.ModuleType("pyrevit")
    routes = types.ModuleType("pyrevit.routes")
    routes.API = _API
    routes.make_response = lambda data=None, status=200: _Respuesta(data, status)
    pyrevit.routes = routes
    pyrevit.revit = types.SimpleNamespace(uidoc=None, doc=None)
    db = types.ModuleType("pyrevit.DB")
    for nombre in ("Document", "Transaction", "TransactionGroup", "ElementId", "BuiltInParameter"):
        setattr(db, nombre, type(nombre, (), {}))
    ui = types.ModuleType("pyrevit.UI")
    ui.UIDocument = type("UIDocument", (), {})
    pyrevit.DB = db
    pyrevit.UI = ui
    sys.modules["pyrevit"] = pyrevit
    sys.modules["pyrevit.routes"] = routes
    sys.modules["pyrevit.DB"] = db
    sys.modules["pyrevit.UI"] = ui

    clr = types.ModuleType("clr")
    clr.AddReferenceToFileAndPath = lambda ruta: None
    clr.AddReference = lambda nombre: None
    clr.GetClrType = lambda t: t
    sys.modules["clr"] = clr

    system = types.ModuleType("System")

    class _Array:
        def __class_getitem__(cls, tipo):
            return lambda valores: list(valores)
    system.Array = _Array
    system.String = str
    system.Object = object
    system.Type = type
    system.Int64 = int
    system.AppDomain = types.SimpleNamespace(CurrentDomain=types.SimpleNamespace(
        GetAssemblies=lambda: ([] if OPCIONES.sin_addin else [_Ensamblado()])))
    sys.modules["System"] = system

    stringio = types.ModuleType("StringIO")
    stringio.StringIO = io.StringIO
    sys.modules["StringIO"] = stringio

    ruta_seguridad = Path(extension) / "revit_mcp" / "seguridad.py" if extension else None
    if ruta_seguridad and ruta_seguridad.is_file():
        import importlib.util
        espec = importlib.util.spec_from_file_location("seguridad", str(ruta_seguridad))
        modulo = importlib.util.module_from_spec(espec)
        espec.loader.exec_module(modulo)
        sys.modules["seguridad"] = modulo
        return "seguridad.py real de " + str(ruta_seguridad)
    # Imitación mínima del decorador (misma semántica: token en data o en query_params; 401 si no coincide)
    seguridad = types.ModuleType("seguridad")
    seguridad._token = None
    seguridad.establecer_token = lambda t: setattr(seguridad, "_token", t)
    seguridad.token_actual = lambda: seguridad._token

    def requiere_token(funcion):
        nombres = list(inspect.getfullargspec(funcion).args)
        firma = nombres + ([] if "request" in nombres else ["request"])

        def envoltura(**kwargs):
            request = kwargs["request"]
            recibido = None
            if isinstance(request.data, dict) and "token" in request.data:
                recibido = request.data.pop("token")
            elif isinstance(request.query_params, dict):
                recibido = request.query_params.get("token")
            if not recibido or recibido != seguridad._token:
                return _Respuesta({"error": "token ausente o incorrecto"}, 401)
            return funcion(**{n: kwargs[n] for n in nombres})
        envoltura.__name__ = funcion.__name__
        envoltura.__doc__ = funcion.__doc__
        envoltura.__wrapped__ = funcion
        envoltura.__signature__ = inspect.Signature([inspect.Parameter(n, inspect.Parameter.POSITIONAL_OR_KEYWORD) for n in firma])
        # getfullargspec lee __signature__ en CPython 3; así el despachador ve "request" aunque el manejador no lo declare.
        return envoltura
    seguridad.requiere_token = requiere_token
    sys.modules["seguridad"] = seguridad
    return "seguridad simulada (no se indicó --extension)"


def cargar_conexiones(ruta, extension):
    """Importa conexiones.py con los módulos simulados y devuelve (módulo, API con las rutas registradas, origen seguridad)."""
    origen = _instalar_modulos_falsos(extension)
    import importlib.util
    espec = importlib.util.spec_from_file_location("conexiones", str(ruta))
    modulo = importlib.util.module_from_spec(espec)
    sys.modules["conexiones"] = modulo
    espec.loader.exec_module(modulo)
    api = _API("revit_mcp")
    modulo.register_conn_routes(api)
    return modulo, api, origen


# ---------------------------------------------------------------------------
# Autocomprobación en proceso
# ---------------------------------------------------------------------------
RUTAS_ESPERADAS = [
    ("GET", "/conn/ping/", "ping", {}),
    ("GET", "/conn/guide/", "guide", {}),
    ("GET", "/conn/types/", "types", {}),
    ("GET", "/conn/schema/gusset_node", "schema", {"type": "gusset_node"}),
    ("POST", "/conn/node_info/", "node_info", {"element_ids": [1249510, 1249630]}),
    ("POST", "/conn/find_profile/", "find_profile", {"query": "HSS3X3X1/4"}),
    ("POST", "/conn/validate/", "validate", {"spec": {"a": 1}}),
    ("POST", "/conn/preview/", "preview", {"spec": {"a": 1}}),
    ("POST", "/conn/create/", "create", {"spec": {"a": 1}, "validation_token": "x"}),
    ("GET", "/conn/list/", "list", {}),
    ("GET", "/conn/get/abc-123", "get", {"connection_id": "abc-123"}),
    ("POST", "/conn/update/", "update", {"connection_id": "abc", "spec": {"a": 1}, "validation_token": "x"}),
    ("POST", "/conn/delete/", "delete", {"connection_id": "abc"}),
    ("GET", "/conn/catalog/list/", "catalog_list", {}),
    ("GET", "/conn/catalog/get/abc-123", "catalog_get", {"template_id": "abc-123"}),
    ("POST", "/conn/catalog/save/", "catalog_save", {"name": "x", "spec": {"a": 1}}),
    ("POST", "/conn/catalog/delete/", "catalog_delete", {"template_id": "abc"}),
    ("POST", "/conn/catalog/apply/", "catalog_apply", {"template_id": "abc", "element_ids": [1249510, 1249630]}),
    ("POST", "/conn/batch/plan/", "batch_plan", {"element_ids": [1249510, 1249630], "mark": False}),
    ("POST", "/conn/batch/plan/get/", "batch_plan_get", {"plan_id": "abc"}),
    ("POST", "/conn/batch/plan/discard/", "batch_plan_discard", {"plan_id": "abc"}),
    ("POST", "/conn/batch/create/", "batch_create", {"plan_id": "abc", "nodes": [{"node": "N1", "validation_token": "x"}]}),
    ("POST", "/conn/batch/delete/", "batch_delete", {"batch_id": "abc"}),
    ("POST", "/conn/split/", "split", {"element_id": 1249632, "chord_element_id": 1245533, "dry_run": True}),
    ("POST", "/conn/unsplit/", "unsplit", {"element_id": 1249632}),
    ("POST", "/conn/op/no_existe/", "no_existe", {"k": 1}),
]


def autocomprobar(api, origen_seguridad, token):
    import seguridad
    seguridad.establecer_token(token)
    resultados = []

    def comprobar(nombre, condicion, detalle=""):
        resultados.append(bool(condicion))
        print("  [{}] {}{}".format("OK" if condicion else "FALLO", nombre, "  " + detalle if detalle else ""))

    print("Autocomprobación de conexiones.py ({})".format(origen_seguridad))
    patrones = [r[0] for r in api.rutas]
    comprobar("27 rutas registradas", len(api.rutas) == 27, "registradas: {}".format(len(api.rutas)))
    for metodo, ruta, operacion, cuerpo in RUTAS_ESPERADAS:
        del LLAMADAS[:]
        datos = dict(cuerpo)
        consulta = {}
        if metodo == "POST":
            datos["token"] = token
        else:
            consulta["token"] = token
        r = api.despachar(metodo, ruta, datos, consulta, _Documento(), object(), object())
        llego = LLAMADAS[-1] if LLAMADAS else (None, None)
        esperado = {k: v for k, v in cuerpo.items()} if metodo == "POST" else cuerpo
        comprobar("{} {} -> Bridge.Handle('{}')".format(metodo, ruta, operacion),
                  r.status == 200 and isinstance(r.data, dict) and "ok" in r.data and llego[0] == operacion
                  and llego[1] == esperado,
                  "HTTP {} op={} cuerpo={}".format(r.status, llego[0], json.dumps(llego[1], ensure_ascii=False)))
    # Sin token -> 401 en GET y en POST
    r = api.despachar("GET", "/conn/ping/", {}, {}, _Documento(), None, None)
    comprobar("GET /conn/ping/ sin token -> 401", r.status == 401, "HTTP {}".format(r.status))
    r = api.despachar("POST", "/conn/validate/", {"spec": {}}, {}, _Documento(), None, None)
    comprobar("POST /conn/validate/ sin token -> 401", r.status == 401, "HTTP {}".format(r.status))
    # Token incorrecto
    r = api.despachar("GET", "/conn/list/", {}, {"token": "0" * 64}, _Documento(), None, None)
    comprobar("GET /conn/list/ token incorrecto -> 401", r.status == 401, "HTTP {}".format(r.status))
    # dev_exec
    r = api.despachar("POST", "/conn/dev_exec/", {"token": token, "code": 'print("hola")', "description": "prueba"}, {},
                      _Documento(), object(), object())
    comprobar("POST /conn/dev_exec/ print('hola')", r.status == 200 and r.data.get("ok") is True
              and r.data["data"]["output"].strip() == "hola", json.dumps(r.data.get("data"), ensure_ascii=False)[:120])
    r = api.despachar("POST", "/conn/dev_exec/", {"token": token, "code": "raise ValueError('x')"}, {},
                      _Documento(), object(), object())
    comprobar("POST /conn/dev_exec/ excepción -> PROBE_EXCEPTION", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "PROBE_EXCEPTION")
    # Ruta inexistente -> 404
    r = api.despachar("GET", "/conn/nada/", {}, {"token": token}, _Documento(), None, None)
    comprobar("GET /conn/nada/ -> 404", r.status == 404, "HTTP {}".format(r.status))
    # Sin add-in -> ADDIN_NOT_LOADED (200)
    OPCIONES.sin_addin = True
    import conexiones as modulo
    modulo._metodo_handle = None
    r = api.despachar("GET", "/conn/ping/", {}, {"token": token}, _Documento(), None, None)
    comprobar("GET /conn/ping/ sin add-in -> 200 ADDIN_NOT_LOADED", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "ADDIN_NOT_LOADED", json.dumps(r.data.get("errors"), ensure_ascii=False)[:160])
    OPCIONES.sin_addin = False
    modulo._metodo_handle = None
    # Sin documento -> NO_DOCUMENT en validate, pero ping y guide responden ok
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": {}}, {}, None, None, None)
    comprobar("POST /conn/validate/ sin documento -> NO_DOCUMENT", r.status == 200 and r.data.get("ok") is False
              and r.data["errors"][0]["code"] == "NO_DOCUMENT")
    r = api.despachar("GET", "/conn/guide/", {}, {"token": token}, None, None, None)
    comprobar("GET /conn/guide/ sin documento -> ok:true", r.status == 200 and r.data.get("ok") is True)
    # Acentos y emoji en el cuerpo (primera ronda del PC: "'unknown' codec can't decode byte 0xe1" en IronPython)
    texto_raro = "La etiqueta del montante est\u00e1 cortada \u2014 \U0001f600 \"comillas\" \\ barra\n"
    cuerpo_raro = {"spec": {"uncertain_fields": [{"reason": texto_raro}], "n": 1.5, "lista": [None, True, 7]}}
    del LLAMADAS[:]
    r = api.despachar("POST", "/conn/validate/", dict(cuerpo_raro, token=token), {}, _Documento(), object(), object())
    comprobar("POST /conn/validate/ con acentos y emoji llega intacto al Bridge",
              r.status == 200 and LLAMADAS and LLAMADAS[-1][1] == cuerpo_raro,
              "recibido: " + json.dumps((LLAMADAS[-1][1] if LLAMADAS else None), ensure_ascii=False)[:90])
    # Serializador de reserva (_json_ascii): ida y vuelta exacta y solo ASCII
    salida = modulo._json_ascii(cuerpo_raro)
    comprobar("_json_ascii: solo ASCII y json.loads devuelve el mismo objeto",
              all(ord(c) < 128 for c in salida) and json.loads(salida) == cuerpo_raro, salida[:100])
    # Forzar el fallo de json.dumps para comprobar que llamar_bridge usa la reserva
    json_original = modulo.json
    def dumps_roto(*args, **kwargs):
        raise UnicodeDecodeError("unknown", b"\xe1", 0, 1, "simulado")
    # Solo se rompe el json que ve conexiones.py (no el del simulador): se sustituye el modulo en su espacio de nombres.
    modulo.json = types.SimpleNamespace(dumps=dumps_roto, loads=json.loads)
    try:
        del LLAMADAS[:]
        r = api.despachar("POST", "/conn/validate/", dict(cuerpo_raro, token=token), {}, _Documento(), object(), object())
    finally:
        modulo.json = json_original
    comprobar("llamar_bridge con json.dumps roto usa _json_ascii y el Bridge recibe lo mismo",
              r.status == 200 and LLAMADAS and LLAMADAS[-1][1] == cuerpo_raro)
    # Catálogo (Fase 7): guardar desde la especificación del fixture, aplicar al mismo nudo y borrar, en proceso
    fixture = json.loads(RUTA_FIXTURE.read_text(encoding="utf-8")) if RUTA_FIXTURE.is_file() else None
    r = api.despachar("POST", "/conn/catalog/save/", {"token": token, "name": "Autocomprobación", "spec": fixture, "overwrite": True}, {},
                      _Documento(), object(), object())
    tid = ((r.data or {}).get("data") or {}).get("template_id") if r.status == 200 else None
    comprobar("POST /conn/catalog/save/ con el fixture -> template_id", r.status == 200 and r.data.get("ok") is True and bool(tid),
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    r = api.despachar("POST", "/conn/catalog/apply/", {"token": token, "template_id": tid, "element_ids": fixture["node"]["element_ids"] if fixture else [],
                                                     "chord_element_id": 1249510}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/catalog/apply/ al mismo nudo -> ok, token y spec con template_id",
              r.status == 200 and r.data.get("ok") is True and d.get("is_valid") is True and len(d.get("validation_token") or "") == 64
              and (d.get("spec") or {}).get("source", {}).get("template_id") == tid and d["match"]["orientation"] == "same",
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    # Fase 11 (D9): catalog/apply SIN template_id elige la mejor y devuelve proposal.
    r = api.despachar("POST", "/conn/catalog/apply/", {"token": token, "element_ids": fixture["node"]["element_ids"] if fixture else [],
                                                     "chord_element_id": 1249510}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/catalog/apply/ sin template_id -> la plantilla elegida, proposal.chosen y summary_text (Fase 11)",
              r.status == 200 and r.data.get("ok") is True and d.get("template_id") == tid and ((d.get("proposal") or {}).get("chosen") or {}).get("template_id") == tid
              and len((d.get("proposal") or {}).get("candidates") or []) == 1 and "Plantilla propuesta" in ((d.get("proposal") or {}).get("summary_text") or ""),
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    # Fase 11 (V5): source.drawing_image pasa por validate y por catalog/save (la clave nueva del contrato).
    con_plano = copy.deepcopy(fixture) if fixture else {}
    con_plano.setdefault("source", {})["drawing_image"] = "C:\\planos\\detalle-D.png"
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": con_plano}, {}, _Documento(), object(), object())
    comprobar("POST /conn/validate/ con source.drawing_image -> ok:true (Fase 11)",
              r.status == 200 and r.data.get("ok") is True and len(((r.data or {}).get("data") or {}).get("validation_token") or "") == 64,
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    r = api.despachar("POST", "/conn/catalog/save/", {"token": token, "name": "Autocomprobación con plano", "spec": con_plano, "overwrite": True}, {},
                      _Documento(), object(), object())
    tid_plano = ((r.data or {}).get("data") or {}).get("template_id") if r.status == 200 else None
    comprobar("POST /conn/catalog/save/ con source.drawing_image -> plantilla con origin.drawing_image (Fase 11)",
              r.status == 200 and r.data.get("ok") is True and bool(tid_plano)
              and (CATALOGO.get(tid_plano or "", {}).get("origin") or {}).get("drawing_image") == "C:\\planos\\detalle-D.png",
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    if tid_plano:
        api.despachar("POST", "/conn/catalog/delete/", {"token": token, "template_id": tid_plano}, {}, _Documento(), object(), object())
    # Plan de lote (Fase 8): planificar sobre el nudo del fixture con esa plantilla, releer, corregir y descartar
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": fixture["node"]["element_ids"] if fixture else [],
                                                   "template_ids": [tid], "mark": False}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    pid = d.get("plan_id")
    n1 = (d.get("nodes") or [{}])[0]
    comprobar("POST /conn/batch/plan/ -> plan_id, N1 ready con token y batch_id",
              r.status == 200 and r.data.get("ok") is True and bool(pid) and n1.get("name") == "N1" and n1.get("status") == "ready"
              and len(n1.get("validation_token") or "") == 64 and (n1.get("spec") or {}).get("source", {}).get("batch_id") == pid
              and d.get("is_marked") is False and d["summary"].get("ready") == 1,
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160])
    # Ronda 8c: claves nuevas en español (status_text, advice, visible_by_default, summary_text) y color por estado.
    comprobar("POST /conn/batch/plan/ -> status_text '● Listo', advice '—', visible_by_default, color_name 'verde' y summary_text",
              n1.get("status_text") == "● Listo" and n1.get("advice") == "—" and n1.get("visible_by_default") is True
              and n1.get("color_name") == "verde" and n1.get("color_rgb") == [46, 160, 67]
              and (d.get("summary_text") or "").startswith("Se creará 1 conexión con ") and d.get("visible_count") == 1,
              "{} | {} | {} | {}".format(n1.get("status_text"), n1.get("advice"), n1.get("color_name"), d.get("summary_text")))
    # Fase 10: etiquetas pinchables (labels) y selección asistida (expand_selection), imitadas para el único nudo.
    comprobar("POST /conn/batch/plan/ con mark:false -> sin etiquetas (labels.count 0, label_indices {}) y sin selection_expansion",
              isinstance(d.get("labels"), dict) and d["labels"].get("count") == 0 and d.get("label_view_id") is None and d.get("label_indices") == {}
              and n1.get("label_index") is None and d.get("selection_expansion") is None and (d.get("marks") or {}).get("ghost_count") == 0)
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": fixture["node"]["element_ids"] if fixture else [], "template_ids": [tid]}, {}, _Documento(), object(), object())
    d2 = (r.data or {}).get("data") or {}
    n2 = (d2.get("nodes") or [{}])[0]
    comprobar("POST /conn/batch/plan/ con mark (por defecto) -> etiqueta del nudo: labels.count 1, label_view_id 1, label_indices {N1: 0}, nodes[0].label_index 0",
              r.data.get("ok") is True and (d2.get("labels") or {}).get("count") == 1 and d2.get("label_view_id") == 1 and d2.get("label_indices") == {"N1": 0}
              and n2.get("label_index") == 0 and n2.get("is_marked") is True)
    r = api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": d2.get("plan_id")}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/plan/discard/ del plan con etiqueta -> removed_labels 1 y remaining_labels 0",
              r.data.get("ok") is True and r.data["data"].get("removed_labels") == 1 and r.data["data"].get("remaining_labels") == 0)
    diagonal = next((i for i in sorted(MIEMBROS) if not MIEMBROS[i]["chord"]), None)
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": [diagonal], "template_ids": [tid], "mark": False, "expand_selection": True}, {}, _Documento(), object(), object())
    d3 = (r.data or {}).get("data") or {}
    exp = d3.get("selection_expansion") or {}
    comprobar("POST /conn/batch/plan/ con expand_selection desde UNA diagonal -> las otras 3 barras del nudo (1 cordón, 2 barras), selection_count 4, aviso SELECTION_EXPANDED, N1 ready",
              r.data.get("ok") is True and exp.get("requested_count") == 1 and exp.get("added_count") == 3 and exp.get("chord_count") == 1 and exp.get("member_count") == 2
              and d3.get("selection_count") == 4 and any(w.get("code") == "SELECTION_EXPANDED" for w in r.data.get("warnings") or [])
              and (d3.get("nodes") or [{}])[0].get("status") == "ready"
              and exp.get("summary_text") == "Se añadieron 3 barras que tocan la selección: 1 cordón que pasa de largo y 2 barras que llegan.",
              "{} | {}".format(json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:120], exp.get("summary_text")))
    api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": d3.get("plan_id")}, {}, _Documento(), object(), object())
    r = api.despachar("POST", "/conn/batch/plan/get/", {"token": token, "plan_id": pid, "node": "N1"}, {}, None, None, None)
    comprobar("POST /conn/batch/plan/get/ node N1 -> el mismo token (sin documento también responde)",
              r.status == 200 and r.data.get("ok") is True and (r.data["data"].get("node") or {}).get("validation_token") == n1.get("validation_token"))
    # Fase 9: crear el lote con el token del plan, saltarlo la segunda vez, token malo, list por lote y borrar el lote.
    comprobar("POST /conn/batch/plan/ -> N1 trae actions (botones de Qué hacer) y el plan creatable_count",
              isinstance(n1.get("actions"), list) and d.get("creatable_count") == 1 and d.get("created_count") == 0 and d.get("has_batch_connections") is False)
    tok = n1.get("validation_token")
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid, "nodes": [{"node": "N1", "validation_token": tok}]}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    lote = (d.get("nodes") or [{}])[0]
    cid = lote.get("connection_id")
    comprobar("POST /conn/batch/create/ N1 con su token -> created, 1 conexión con source.batch_id = plan_id, summary_text",
              r.status == 200 and r.data.get("ok") is True and d.get("created_count") == 1 and d.get("failed_count") == 0 and lote.get("outcome") in ("created", "created_with_warnings")
              and bool(cid) and (CONEXIONES.get(cid, {}).get("spec", {}).get("source") or {}).get("batch_id") == pid
              and (d.get("summary_text") or "").startswith("Lote " + pid[:8] + ": 1 conexión creada") and d.get("undo_entries") == "one"
              and (d.get("plan_summary") or {}).get("created") == 1,
              "{} | {}".format(json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:120], d.get("summary_text")))
    r = api.despachar("POST", "/conn/batch/plan/get/", {"token": token, "plan_id": pid}, {}, None, None, None)
    d = (r.data or {}).get("data") or {}
    n1 = (d.get("nodes") or [{}])[0]
    comprobar("POST /conn/batch/plan/get/ tras crear -> N1 created '✔ Creada', created_connection_id, last_report y cabecera 'Creada 1 conexión'",
              n1.get("status") == "created" and n1.get("status_text", "").startswith("✔ Creada") and n1.get("created_connection_id") == cid
              and n1.get("is_marked") is False and (d.get("last_report") or {}).get("created_count") == 1 and d.get("has_batch_connections") is True
              and (d.get("summary_text") or "").startswith("Creada 1 conexión"), "{} | {}".format(n1.get("status_text"), d.get("summary_text")))
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid, "nodes": [{"node": "N1", "validation_token": tok}]}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/create/ otra vez -> N1 skipped 'ya creada en este lote', nada nuevo",
              r.data.get("ok") is True and d.get("skipped_count") == 1 and d.get("created_count") == 0 and "ya creada en este lote" in ((d.get("nodes") or [{}])[0].get("reason") or "")
              and len(CONEXIONES) == 1)
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid, "nodes": ["N1"]}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/create/ con un nombre suelto (sin token) -> INVALID_REQUEST",
              r.data.get("ok") is False and r.data["errors"][0]["code"] == "INVALID_REQUEST")
    r = api.despachar("GET", "/conn/list/", {}, {"token": token}, _Documento(), None, None)
    d = (r.data or {}).get("data") or {}
    comprobar("GET /conn/list/ -> la conexión lleva batch_id y batches cuenta 1",
              d.get("connections_count") == 1 and (d.get("connections") or [{}])[0].get("batch_id") == pid and (d.get("batches") or {}).get(pid) == 1)
    r = api.despachar("POST", "/conn/op/list/", {"token": token, "batch_id": "otro"}, {}, _Documento(), None, None)
    comprobar("POST /conn/op/list/ con batch_id de otro lote -> 0 de 1", ((r.data or {}).get("data") or {}).get("connections_count") == 0 and ((r.data or {}).get("data") or {}).get("total_count") == 1)
    r = api.despachar("POST", "/conn/batch/delete/", {"token": token, "batch_id": pid}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/delete/ -> 1 borrada, 0 conexiones, N1 vuelve a ready con su token",
              r.data.get("ok") is True and d.get("deleted_count") == 1 and len(CONEXIONES) == 0 and (d.get("summary_text") or "").startswith("Lote " + pid[:8] + ": 1 conexión borrada")
              and (d.get("plan_summary") or {}).get("ready") == 1, "{} | {}".format(json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:120], d.get("summary_text")))
    r = api.despachar("POST", "/conn/batch/delete/", {"token": token, "batch_id": pid}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/delete/ otra vez -> ok con aviso BATCH_EMPTY y deleted_count 0",
              r.data.get("ok") is True and ((r.data or {}).get("data") or {}).get("deleted_count") == 0 and any(w.get("code") == "BATCH_EMPTY" for w in r.data.get("warnings") or []))
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid, "nodes": [{"node": "N1", "validation_token": "0" * 64}]}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/create/ con token falso -> failed VALIDATION_TOKEN_INVALID, aviso BATCH_NODE_FAILED, nada creado",
              r.data.get("ok") is True and d.get("failed_count") == 1 and (d.get("nodes") or [{}])[0].get("errors", [{}])[0].get("code") == "VALIDATION_TOKEN_INVALID"
              and any(w.get("code") == "BATCH_NODE_FAILED" for w in r.data.get("warnings") or []) and len(CONEXIONES) == 0)
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid, "nodes": [{"node": "N1", "validation_token": "0" * 64}], "stop_on_error": True}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/create/ con token falso y stop_on_error -> BATCH_STOPPED (ok:false) con el informe en data",
              r.data.get("ok") is False and r.data["errors"][0]["code"] == "BATCH_STOPPED" and ((r.data or {}).get("data") or {}).get("stopped") is True)
    r = api.despachar("POST", "/conn/batch/plan/get/", {"token": token, "plan_id": pid}, {}, None, None, None)
    n1 = ((((r.data or {}).get("data") or {}).get("nodes")) or [{}])[0]
    comprobar("POST /conn/batch/plan/get/ -> N1 failed '✖ Falló al crear' con consejo y botones",
              n1.get("status") == "failed" and n1.get("status_text") == "✖ Falló al crear" and (n1.get("advice") or "").startswith("Falló al crear (VALIDATION_TOKEN_INVALID")
              and [a.get("key") for a in n1.get("actions") or []] == ["show", "exclude"], "{} | {}".format(n1.get("status_text"), n1.get("advice")))
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": "no-existe", "nodes": [{"node": "N1", "validation_token": tok}]}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/create/ con un plan inexistente -> PLAN_NOT_FOUND", r.data.get("ok") is False and r.data["errors"][0]["code"] == "PLAN_NOT_FOUND")
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "plan_id": pid, "overrides": {"exclude": ["N1"]}}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/plan/ con plan_id y overrides.exclude -> mismo plan, N1 excluded",
              r.status == 200 and r.data.get("ok") is True and d.get("plan_id") == pid and (d.get("nodes") or [{}])[0].get("status") == "excluded"
              and d.get("overrides", {}).get("exclude") == ["N1"])
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "plan_id": pid, "overrides": {"excluir": ["N1"]}}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/plan/ con una corrección desconocida -> INVALID_REQUEST",
              r.status == 200 and r.data.get("ok") is False and r.data["errors"][0]["code"] == "INVALID_REQUEST")
    r = api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": pid}, {}, _Documento(), object(), object())
    comprobar("POST /conn/batch/plan/discard/ -> ok", r.status == 200 and r.data.get("ok") is True and r.data["data"].get("discarded_plan_id") == pid)
    r = api.despachar("POST", "/conn/batch/plan/get/", {"token": token, "plan_id": pid}, {}, None, None, None)
    comprobar("POST /conn/batch/plan/get/ <descartado> -> PLAN_NOT_FOUND",
              r.status == 200 and r.data.get("ok") is False and r.data["errors"][0]["code"] == "PLAN_NOT_FOUND")
    r = api.despachar("POST", "/conn/catalog/delete/", {"token": token, "template_id": tid}, {}, _Documento(), object(), object())
    comprobar("POST /conn/catalog/delete/ -> ok", r.status == 200 and r.data.get("ok") is True)
    # Ronda 8c (C9): con el catálogo vacío el plan avisa CATALOG_EMPTY y lo dice en español en vez de salir todo no_match sin más.
    guardadas = dict(CATALOGO)
    CATALOGO.clear()
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": fixture["node"]["element_ids"] if fixture else [], "mark": False},
                      {}, _Documento(), object(), object())
    CATALOGO.update(guardadas)
    d = (r.data or {}).get("data") or {}
    n1 = (d.get("nodes") or [{}])[0]
    # Fase 13 (D2): con el catálogo vacío el nudo con cartela ya no sale no_match: se arranca con la cartela automática (aviso TEMPLATE_NO_MATCH).
    comprobar("POST /conn/batch/plan/ con el catálogo vacío -> aviso CATALOG_EMPTY, N1 ready '▲ Listo con aviso (arranque)' con la cartela automática (TEMPLATE_NO_MATCH, outline.mode auto) y summary_text (Fase 13, D2)",
              r.status == 200 and r.data.get("ok") is True and any(w.get("code") == "CATALOG_EMPTY" for w in r.data.get("warnings") or [])
              and n1.get("status") == "ready" and n1.get("starter") is True and n1.get("status_text") == "▲ Listo con aviso (arranque)"
              and any(w.get("code") == "TEMPLATE_NO_MATCH" for w in n1.get("warnings") or []) and ((n1.get("spec") or {}).get("gusset") or {}).get("outline") == {"mode": "auto"}
              and len(n1.get("validation_token") or "") == 64 and (n1.get("advice") or "").startswith("Sin plantilla: se creará con el arranque gusset_node (cartela)")
              and (d.get("summary_text") or "").startswith("Se creará 1 conexión: 1 cartela automática (arranque)."),
              "{} | {} | {} | {}".format([w.get("code") for w in r.data.get("warnings") or []], n1.get("status_text"), n1.get("advice"), d.get("summary_text")))
    if d.get("plan_id"):
        api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": d["plan_id"]}, {}, _Documento(), object(), object())
    r = api.despachar("GET", "/conn/catalog/get/" + str(tid), {}, {"token": token}, None, None, None)
    comprobar("GET /conn/catalog/get/<borrada> -> TEMPLATE_NOT_FOUND (sin documento también responde)",
              r.status == 200 and r.data.get("ok") is False and r.data["errors"][0]["code"] == "TEMPLATE_NOT_FOUND")
    # Fase 12: tres tipos, esquema y ejemplo por tipo, validate de los fixtures C (end_plate) y K (direct_weld), node_info con la
    # excentricidad del nudo en K y la clase splice del empalme, y batch_plan con un empalme (arranque Detalle C) y con el nudo en K
    # (arranque direct_weld con el aviso MEMBER_ECCENTRIC).
    r = api.despachar("GET", "/conn/types/", {}, {"token": token}, None, None, None)
    tipos = [t.get("type_name") for t in ((r.data or {}).get("data") or {}).get("connection_types") or []]
    comprobar("GET /conn/types/ -> gusset_node, direct_weld y end_plate (Fase 12)", r.data.get("ok") is True and tipos == ["gusset_node", "direct_weld", "end_plate"], str(tipos))
    for tipo, archivo in (("direct_weld", "nudo-K-cordon-superior.json"), ("end_plate", "detalle-C.json")):
        r = api.despachar("GET", "/conn/schema/" + tipo, {}, {"token": token}, None, None, None)
        d = (r.data or {}).get("data") or {}
        comprobar("GET /conn/schema/{} -> esquema propio y ejemplo del fixture {} (Fase 12)".format(tipo, archivo),
                  r.data.get("ok") is True and d.get("connection_type") == tipo and (d.get("example") or {}).get("connection_type") == tipo
                  and ("plate" in ((d.get("json_schema") or {}).get("required") or [])) == (tipo == "end_plate"),
                  json.dumps((d.get("json_schema") or {}).get("required"), ensure_ascii=False))
    fixture_c = _fixture("detalle-C.json")
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": fixture_c}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/validate/ del fixture C (end_plate, tramos 1245531 y 1245530) -> ok:true, token, kind splice (Fase 12)",
              r.status == 200 and r.data.get("ok") is True and len(d.get("validation_token") or "") == 64 and (d.get("calculated_values") or {}).get("kind") == "splice",
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:200])
    roto = copy.deepcopy(fixture_c)
    roto["bolts"]["faces"]["top"]["edge_mm"] = 10.0
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": roto}, {}, _Documento(), object(), object())
    comprobar("POST /conn/validate/ del fixture C con edge_mm 10 arriba -> BOLT_EDGE_DISTANCE_TOO_SMALL en bolts.faces.top.edge_mm (Fase 12)",
              r.data.get("ok") is False and any(e.get("code") == "BOLT_EDGE_DISTANCE_TOO_SMALL" and e.get("path") == "bolts.faces.top.edge_mm" for e in r.data.get("errors") or []),
              json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:200])
    fixture_k = _fixture("nudo-K-cordon-superior.json")
    r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": fixture_k}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    avisos_k = [w.get("code") for w in r.data.get("warnings") or []]
    comprobar("POST /conn/validate/ del fixture K (direct_weld, 1245533 + 1249631 + 1249632) -> ok:true con el aviso MEMBER_ECCENTRIC y eccentricity_mm 54.9 (Fase 12)",
              r.data.get("ok") is True and len(d.get("validation_token") or "") == 64 and avisos_k.count("MEMBER_ECCENTRIC") == 2
              and (d.get("calculated_values") or {}).get("eccentricity_mm") == 54.9 and (d.get("calculated_values") or {}).get("kind") == "node",
              "{} | {}".format(avisos_k, json.dumps((r.data or {}).get("errors"), ensure_ascii=False)[:160]))
    r = api.despachar("POST", "/conn/preview/", {"token": token, "spec": fixture_k}, {}, _Documento(), object(), object())
    resumen = ((r.data or {}).get("data") or {}).get("summary") or {}
    comprobar("POST /conn/preview/ del fixture K -> 0 cartelas, 2 soldaduras, 2 barras retiradas por el eje, eccentricity_mm 54.9 (Fase 12)",
              r.data.get("ok") is True and resumen.get("gusset_plates") == 0 and resumen.get("weld_lines") == 2 and resumen.get("members_modified") == 2
              and resumen.get("eccentricity_mm") == 54.9, json.dumps(resumen, ensure_ascii=False)[:200])
    r = api.despachar("POST", "/conn/preview/", {"token": token, "spec": fixture_c}, {}, _Documento(), object(), object())
    resumen = ((r.data or {}).get("data") or {}).get("summary") or {}
    comprobar("POST /conn/preview/ del fixture C -> 2 planchas, 12 pernos, 2 tramos retirados (Fase 12)",
              r.data.get("ok") is True and resumen.get("end_plates") == 2 and resumen.get("bolts") == 12 and resumen.get("members_modified") == 2,
              json.dumps(resumen, ensure_ascii=False)[:200])
    r = api.despachar("POST", "/conn/node_info/", {"token": token, "element_ids": [1245533, 1249631, 1249632]}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    desvios = {m.get("element_id"): m.get("axis_offset_mm") for m in d.get("members") or []}
    comprobar("POST /conn/node_info/ del nudo en K -> kind node, eccentricity_mm 54.9, axis_offset_mm -27.45 / +27.45, axis_cut_mm por barra (Fase 12)",
              r.data.get("ok") is True and d.get("kind") == "node" and d.get("eccentricity_mm") == 54.9 and desvios.get(1249631) == -27.45 and desvios.get(1249632) == 27.45
              and all(len(m.get("axis_cut_mm") or []) == 3 and "width_mm" in m for m in d.get("members") or []),
              json.dumps(desvios, ensure_ascii=False))
    r = api.despachar("POST", "/conn/node_info/", {"token": token, "element_ids": [1245531, 1245530]}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/node_info/ de los dos tramos del cordón superior -> kind splice, origen en el punto de empalme (Fase 12)",
              r.data.get("ok") is True and d.get("kind") == "splice" and d.get("eccentricity_mm") == 0.0 and (d.get("origin_mm") or [None])[0] == -4180.2,
              json.dumps(d.get("origin_mm"), ensure_ascii=False))
    r = api.despachar("POST", "/conn/node_info/", {"token": token, "element_ids": fixture["node"]["element_ids"] if fixture else []}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    comprobar("POST /conn/node_info/ del Detalle D -> kind node, eccentricity_mm 0 y el origen de siempre (regresión, Fase 12)",
              r.data.get("ok") is True and d.get("kind") == "node" and d.get("eccentricity_mm") == 0.0 and d.get("origin_mm") == ORIGEN_MM)
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": [1245531, 1245530], "mark": False}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    n1 = (d.get("nodes") or [{}])[0]
    comprobar("POST /conn/batch/plan/ con los dos tramos -> N1 kind splice, ready, starter con 'arranque end_plate (Detalle C)', connection_type end_plate y la cabecera (Fase 12)",
              r.data.get("ok") is True and len(d.get("nodes") or []) == 1 and n1.get("kind") == "splice" and n1.get("status") == "ready" and n1.get("starter") is True
              and n1.get("template_name") == "arranque end_plate (Detalle C)" and n1.get("connection_type") == "end_plate" and n1.get("splice_element_id") == 1245530
              and len(n1.get("validation_token") or "") == 64 and (n1.get("spec") or {}).get("connection_type") == "end_plate"
              and d.get("summary_text") == "Se creará 1 conexión: 1 empalme (arranque Detalle C)." and (n1.get("advice") or "").startswith("Sin plantilla: se creará con el arranque end_plate"),
              "{} | {} | {} | {}".format(n1.get("kind"), n1.get("status"), n1.get("template_name"), d.get("summary_text")))
    if d.get("plan_id"):
        api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": d["plan_id"]}, {}, _Documento(), object(), object())
    r = api.despachar("POST", "/conn/batch/plan/", {"token": token, "element_ids": [1245533, 1249631, 1249632], "mark": False}, {}, _Documento(), object(), object())
    d = (r.data or {}).get("data") or {}
    n1 = (d.get("nodes") or [{}])[0]
    comprobar("POST /conn/batch/plan/ con el nudo en K -> N1 kind node, ready con aviso MEMBER_ECCENTRIC, 'arranque direct_weld', eccentricity_mm 54.9, axis_offset_mm por barra (Fase 12)",
              r.data.get("ok") is True and n1.get("kind") == "node" and n1.get("status") == "ready" and n1.get("starter") is True and n1.get("template_name") == "arranque direct_weld"
              and n1.get("connection_type") == "direct_weld" and n1.get("eccentricity_mm") == 54.9 and n1.get("status_text") == "▲ Listo con aviso (arranque)"
              and any(w.get("code") == "MEMBER_ECCENTRIC" for w in n1.get("warnings") or []) and {m.get("axis_offset_mm") for m in n1.get("members") or []} == {-27.45, 27.45}
              and d.get("summary_text") == "Se creará 1 conexión: 1 soldadura directa (arranque). 1 avisa de excentricidad. Además se parten 2 diagonales a 330 (Detalle B) y se crean 2 empalmes B antes de sus nudos en K.",
              "{} | {} | {} | {}".format(n1.get("kind"), n1.get("status_text"), n1.get("template_name"), d.get("summary_text")))
    # Fase 13 (D5): el nudo en K trae sus dos pasos de corte y detrás los empalmes B previstos N1B y N1B2 (after_split, kind splice, arranque Detalle B).
    pasos = n1.get("split_steps") or []
    previstos = [n for n in d.get("nodes") or [] if n.get("status") == "after_split"]
    comprobar("POST /conn/batch/plan/ con el nudo en K -> 2 split_steps (corte a ~485 mm, splice_node N1B/N1B2), 2 nudos after_split '● Empalme B (tras partir)' con split_of_node N1, split_step_count 2 y after_split_count 2 (Fase 13)",
              len(pasos) == 2 and all(p.get("error") is None and 470 < (p.get("split_distance_mm") or 0) < 500 and len(p.get("split_point_mm") or []) == 3 for p in pasos)
              and [p.get("splice_node") for p in pasos] == ["N1B", "N1B2"] and [n.get("name") for n in previstos] == ["N1B", "N1B2"]
              and all(n.get("kind") == "splice" and n.get("split_of_node") == "N1" and n.get("status_text") == "● Empalme B (tras partir)" and n.get("connection_type") == "end_plate"
                      and n.get("template_name") == "arranque end_plate (Detalle B)" and n.get("is_marked") is False and n.get("validation_token") is None for n in previstos)
              and d.get("split_step_count") == 2 and d.get("after_split_count") == 2 and d.get("creatable_count") == 1,
              "pasos={} previstos={} | {}".format([(p.get("element_id"), p.get("split_distance_mm"), p.get("splice_node")) for p in pasos], [(n.get("name"), n.get("status_text")) for n in previstos], d.get("split_step_count")))
    # Fase 13: crear el lote del nudo en K parte las dos diagonales, crea los dos empalmes B y después el nudo (token renovado); borrar el lote lo deshace todo.
    pid_k = d.get("plan_id")
    r = api.despachar("POST", "/conn/batch/create/", {"token": token, "plan_id": pid_k, "nodes": [{"node": "N1", "validation_token": n1.get("validation_token")}]}, {}, _Documento(), object(), object())
    dc = (r.data or {}).get("data") or {}
    orden = [(n.get("kind"), n.get("node"), n.get("outcome")) for n in dc.get("nodes") or []]
    comprobar("POST /conn/batch/create/ del nudo en K -> 2 cortes (split), 2 empalmes B (created) y el nudo N1 (created_with_warnings) en ese orden; split_count 2; summary_text con '2 diagonales partidas (Detalle B)' (Fase 13)",
              r.data.get("ok") is True and [o[0] for o in orden] == ["split", "split", "splice_b", "splice_b", "node"] and [o[2] for o in orden] == ["split", "split", "created", "created", "created_with_warnings"]
              and dc.get("split_count") == 2 and dc.get("created_count") == 3 and "2 diagonales partidas (Detalle B)" in (dc.get("summary_text") or "")
              and len(CORTES) == 2 and all(_barra(n.get("new_element_id")) for n in (dc.get("nodes") or [])[:2]),
              "{} | {}".format(orden, dc.get("summary_text")))
    nuevo = ((dc.get("nodes") or [{}])[0]).get("new_element_id")
    r = api.despachar("POST", "/conn/node_info/", {"token": token, "element_ids": [1249631, nuevo]}, {}, _Documento(), object(), object())
    dn = (r.data or {}).get("data") or {}
    comprobar("POST /conn/node_info/ de los dos tramos de la diagonal partida -> kind splice, 2 barras, existing_connections con el empalme B (Fase 13)",
              r.data.get("ok") is True and dn.get("kind") == "splice" and len(dn.get("members") or []) == 2 and len(dn.get("existing_connections") or []) >= 1,
              "{} | {}".format(dn.get("kind"), [c.get("connection_type") for c in dn.get("existing_connections") or []]))
    r = api.despachar("POST", "/conn/unsplit/", {"token": token, "element_id": 1249631}, {}, _Documento(), object(), object())
    comprobar("POST /conn/unsplit/ con el empalme B creado -> SPLIT_IN_USE (Fase 13, D8)",
              r.data.get("ok") is False and r.data["errors"][0]["code"] == "SPLIT_IN_USE", json.dumps(r.data.get("errors"), ensure_ascii=False)[:160])
    r = api.despachar("POST", "/conn/batch/delete/", {"token": token, "batch_id": pid_k}, {}, _Documento(), object(), object())
    dd = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/delete/ del lote del nudo en K -> 3 borradas y 2 cortes deshechos (unsplit_count 2); la diagonal 1249631 vuelve a ser una (Fase 13, D8)",
              r.data.get("ok") is True and dd.get("deleted_count") == 3 and dd.get("unsplit_count") == 2 and not CORTES and _barra(nuevo) is None
              and _barra(1249631)["start"] == [-11880.4, -17195.8, 17441.0] and "2 cortes de diagonal deshechos" in (dd.get("summary_text") or ""),
              "{} | {} | {}".format(dd.get("deleted_count"), dd.get("unsplit_count"), dd.get("summary_text")))
    r = api.despachar("POST", "/conn/batch/plan/get/", {"token": token, "plan_id": pid_k}, {}, _Documento(), object(), object())
    dg = (r.data or {}).get("data") or {}
    comprobar("POST /conn/batch/plan/get/ tras borrar el lote -> N1 ready y N1B / N1B2 after_split otra vez, split_step_count 2 (Fase 13)",
              r.data.get("ok") is True and [n.get("status") for n in dg.get("nodes") or []] == ["ready", "after_split", "after_split"] and dg.get("split_step_count") == 2,
              "{}".format([(n.get("name"), n.get("status")) for n in dg.get("nodes") or []]))
    if pid_k:
        api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": pid_k}, {}, _Documento(), object(), object())
    # Fase 13: split suelto (dry_run y real), node_info con split_point_mm, unsplit; la cartela automática en validate y preview.
    r = api.despachar("POST", "/conn/split/", {"token": token, "element_id": 1249632, "chord_element_id": 1245533, "dry_run": True}, {}, _Documento(), object(), object())
    ds = (r.data or {}).get("data") or {}
    comprobar("POST /conn/split/ dry_run de la diagonal 1249632 contra el cordón 1245533 -> preview con θ ≈ 45°, cara ≈ 143, corte ≈ 483 mm, sin tocar nada (Fase 13, D7)",
              r.data.get("ok") is True and ds.get("dry_run") is True and 44.0 < (ds.get("preview") or {}).get("theta_deg", 0) < 46.0 and 470 < (ds.get("preview") or {}).get("split_distance_mm", 0) < 500
              and ds.get("already_split") is None and not CORTES, json.dumps(ds.get("preview"), ensure_ascii=False)[:200])
    r = api.despachar("POST", "/conn/node_info/", {"token": token, "element_ids": [1245533, 1249631, 1249632]}, {}, _Documento(), object(), object())
    dn = (r.data or {}).get("data") or {}
    diag = [m for m in dn.get("members") or [] if not m.get("is_chord")]
    comprobar("POST /conn/node_info/ del nudo en K -> split_point_mm y split_distance_mm (≈ 485) por diagonal, null en el cordón (Fase 13)",
              r.data.get("ok") is True and len(diag) == 2 and all(len(m.get("split_point_mm") or []) == 3 and 470 < (m.get("split_distance_mm") or 0) < 500 and m.get("split_offset_from_chord_face_mm") == 330.0 for m in diag)
              and all(m.get("split_point_mm") is None for m in dn.get("members") or [] if m.get("is_chord")),
              "{}".format([(m.get("element_id"), m.get("split_distance_mm")) for m in dn.get("members") or []]))
    r = api.despachar("POST", "/conn/split/", {"token": token, "element_id": 1249632, "chord_element_id": 1245533}, {}, _Documento(), object(), object())
    ds = (r.data or {}).get("data") or {}
    nuevo = (ds.get("split") or {}).get("new_element_id")
    r2 = api.despachar("POST", "/conn/split/", {"token": token, "element_id": 1249632, "chord_element_id": 1245533}, {}, _Documento(), object(), object())
    comprobar("POST /conn/split/ real -> split_id, new_element_id (barra nueva con la misma sección) y un segundo split -> SPLIT_IN_USE (Fase 13)",
              r.data.get("ok") is True and nuevo is not None and _barra(nuevo) is not None and _barra(nuevo)["type"] == _barra(1249632)["type"]
              and (ds.get("split") or {}).get("element_id") == 1249632 and r2.data.get("ok") is False and r2.data["errors"][0]["code"] == "SPLIT_IN_USE",
              "{} | {}".format(json.dumps(ds.get("split"), ensure_ascii=False)[:160], (r2.data.get("errors") or [{}])[0].get("code")))
    r = api.despachar("POST", "/conn/unsplit/", {"token": token}, {}, _Documento(), object(), object())
    comprobar("POST /conn/unsplit/ sin parámetros -> lista 1 corte", r.data.get("ok") is True and (r.data.get("data") or {}).get("splits_count") == 1)
    r = api.despachar("POST", "/conn/unsplit/", {"token": token, "element_id": nuevo}, {}, _Documento(), object(), object())
    comprobar("POST /conn/unsplit/ por el tramo nuevo -> deshecho, la barra 1249632 recupera su curva y el tramo desaparece (Fase 13, D8)",
              r.data.get("ok") is True and (r.data.get("data") or {}).get("complete") is True and _barra(nuevo) is None and not CORTES
              and _barra(1249632)["start"] == [-9322.3, -17195.8, 19900.0], json.dumps((r.data or {}).get("data"), ensure_ascii=False)[:160])
    r = api.despachar("POST", "/conn/unsplit/", {"token": token, "element_id": 1249632}, {}, _Documento(), object(), object())
    comprobar("POST /conn/unsplit/ de una barra entera -> SPLIT_NOT_FOUND", r.data.get("ok") is False and r.data["errors"][0]["code"] == "SPLIT_NOT_FOUND")
    if fixture:
        auto = copy.deepcopy(fixture)
        auto["gusset"]["outline"] = {"mode": "auto"}
        auto["dimension_chains"] = []
        r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": auto}, {}, _Documento(), object(), object())
        dv = (r.data or {}).get("data") or {}
        cv = dv.get("calculated_values") or {}
        comprobar("POST /conn/validate/ del Detalle D con outline.mode auto -> válido, calculated_values.outline_points_mm (≥ 4 vértices, múltiplos de 5), outline_width_mm/height_mm (Fase 13)",
                  r.data.get("ok") is True and dv.get("is_valid") is True and cv.get("outline_mode") == "auto" and len(cv.get("outline_points_mm") or []) >= 4
                  and all(p[0] % 5 == 0 and p[1] % 5 == 0 for p in cv.get("outline_points_mm") or []) and cv.get("outline_width_mm", 0) > 300 and cv.get("outline_height_mm", 0) > 300,
                  "{} vértices, {} × {}".format(len(cv.get("outline_points_mm") or []), cv.get("outline_width_mm"), cv.get("outline_height_mm")))
        r = api.despachar("POST", "/conn/preview/", {"token": token, "spec": auto}, {}, _Documento(), object(), object())
        dp = (r.data or {}).get("data") or {}
        cartela = (dp.get("elements_to_create") or [{}])[0]
        comprobar("POST /conn/preview/ del Detalle D con outline.mode auto -> gusset_plate con outline_mode auto y los mismos puntos que validate (Fase 13)",
                  r.data.get("ok") is True and cartela.get("outline_mode") == "auto" and cartela.get("outline_points_mm") == cv.get("outline_points_mm")
                  and (dp.get("calculated_values") or {}).get("outline_points_mm") == cv.get("outline_points_mm"), "{} vértices".format(cartela.get("vertices_count")))
        malo = copy.deepcopy(auto)
        malo["gusset"]["outline"]["points_mm"] = [[0, 0], [10, 0], [10, 10]]
        r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": malo}, {}, _Documento(), object(), object())
        comprobar("POST /conn/validate/ con outline.mode auto Y points_mm -> SCHEMA_INVALID en gusset.outline.points_mm (Fase 13)",
                  r.data.get("ok") is False and any(e.get("code") == "SCHEMA_INVALID" and e.get("path") == "gusset.outline.points_mm" for e in r.data.get("errors") or []),
                  json.dumps(r.data.get("errors"), ensure_ascii=False)[:160])
        grande = copy.deepcopy(auto)
        grande["gusset"]["outline"]["beyond_chord_mm"] = 1500
        r = api.despachar("POST", "/conn/validate/", {"token": token, "spec": grande}, {}, _Documento(), object(), object())
        comprobar("POST /conn/validate/ con beyond_chord_mm 1500 -> válido con aviso OUTLINE_AUTO_LARGE (Fase 13)",
                  r.data.get("ok") is True and any(w.get("code") == "OUTLINE_AUTO_LARGE" for w in r.data.get("warnings") or []),
                  "{}".format([w.get("code") for w in r.data.get("warnings") or []]))
        r = api.despachar("POST", "/conn/catalog/save/", {"token": token, "spec": auto, "name": "PRUEBA auto fase 13"}, {}, _Documento(), object(), object())
        tid_auto = ((r.data or {}).get("data") or {}).get("template_id")
        r = api.despachar("POST", "/conn/catalog/apply/", {"token": token, "template_id": tid_auto, "element_ids": fixture["node"]["element_ids"]}, {}, _Documento(), object(), object())
        da = (r.data or {}).get("data") or {}
        comprobar("POST /conn/catalog/save/ + apply de una plantilla con la cartela automática -> matching.angle_tolerance_deg 25 y la cartela recalculada en calculated_values (Fase 13, D3)",
                  tid_auto is not None and r.data.get("ok") is True and (CATALOGO.get(tid_auto) or {}).get("matching", {}).get("angle_tolerance_deg") == 25.0
                  and len((da.get("calculated_values") or {}).get("outline_points_mm") or []) >= 4 and ((da.get("spec") or {}).get("gusset") or {}).get("outline") == {"mode": "auto"},
                  "{} | {}".format((CATALOGO.get(tid_auto) or {}).get("matching"), len((da.get("calculated_values") or {}).get("outline_points_mm") or [])))
        if tid_auto:
            api.despachar("POST", "/conn/catalog/delete/", {"token": token, "template_id": tid_auto}, {}, _Documento(), object(), object())
    if d.get("plan_id"):
        api.despachar("POST", "/conn/batch/plan/discard/", {"token": token, "plan_id": d["plan_id"]}, {}, _Documento(), object(), object())
    print("Autocomprobación: {}/{} correctas".format(sum(resultados), len(resultados)))
    return all(resultados)


# ---------------------------------------------------------------------------
# Servidor HTTP (imita pyRevit Routes en 48884)
# ---------------------------------------------------------------------------
def servir(api, puerto, token, prefijo="/revit_mcp"):
    import seguridad
    seguridad.establecer_token(token)
    doc = None if OPCIONES.sin_documento else _Documento()

    class Manejador(BaseHTTPRequestHandler):
        def _responder(self, respuesta):
            cuerpo = json.dumps(respuesta.data).encode("utf-8")  # pyRevit serializa con ensure_ascii=True
            self.send_response(respuesta.status)
            self.send_header("Content-Type", "application/json")
            self.send_header("Content-Length", str(len(cuerpo)))
            self.end_headers()
            self.wfile.write(cuerpo)

        def _atender(self, metodo):
            partes = urlsplit(self.path)
            ruta = partes.path
            if not ruta.startswith(prefijo + "/"):
                return self._responder(_Respuesta({"error": "not found"}, 404))
            ruta = ruta[len(prefijo):]
            consulta = {k: v[0] for k, v in parse_qs(partes.query).items()}
            datos = {}
            longitud = int(self.headers.get("Content-Length") or 0)
            if longitud:
                crudo = self.rfile.read(longitud).decode("utf-8")
                try:
                    datos = json.loads(crudo)
                except ValueError:
                    datos = crudo
            with CERROJO:
                respuesta = api.despachar(metodo, ruta, datos, consulta, doc, object() if doc else None, object())
            self._responder(respuesta)

        def do_GET(self):
            self._atender("GET")

        def do_POST(self):
            self._atender("POST")

        def log_message(self, formato, *args):
            sys.stderr.write("[simulador] " + (formato % args) + "\n")

    servidor = ThreadingHTTPServer(("127.0.0.1", puerto), Manejador)
    print("Simulador de Revit escuchando en http://127.0.0.1:{}{}/conn/... (token {}...)".format(puerto, prefijo, token[:8]))
    sys.stdout.flush()
    try:
        servidor.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        servidor.server_close()


CERROJO = threading.Lock()


def main():
    parser = argparse.ArgumentParser(description="Simulador de Revit + pyRevit Routes para MotorConexiones")
    parser.add_argument("--autocomprobar", action="store_true", help="comprobar conexiones.py en proceso y salir")
    parser.add_argument("--puerto", type=int, default=48884)
    parser.add_argument("--token", default=None, help="token de sesión (por defecto se genera uno de 64 hex)")
    parser.add_argument("--token-archivo", default=NOMBRE_TOKEN_LITERAL,
                        help="dónde escribir el token (por defecto el nombre literal de Windows en la carpeta actual)")
    parser.add_argument("--extension", default=os.environ.get("REVIT_MCP_EXTENSION"),
                        help="carpeta de revit-mcp para usar su revit_mcp/seguridad.py real")
    parser.add_argument("--conexiones", default=str(RUTA_CONEXIONES), help="ruta de conexiones.py a cargar")
    parser.add_argument("--sin-addin", action="store_true")
    parser.add_argument("--sin-documento", action="store_true")
    args = parser.parse_args()

    OPCIONES.sin_addin = args.sin_addin
    OPCIONES.sin_documento = args.sin_documento
    token = args.token or uuid.uuid4().hex + uuid.uuid4().hex
    modulo, api, origen = cargar_conexiones(args.conexiones, args.extension)

    if args.autocomprobar:
        return 0 if autocomprobar(api, origen, token) else 1

    ruta_token = Path(args.token_archivo)
    if ruta_token.parent != Path("."):
        ruta_token.parent.mkdir(parents=True, exist_ok=True)
    ruta_token.write_text(token, encoding="utf-8")
    print("conexiones.py: {} ({} rutas; {})".format(args.conexiones, len(api.rutas), origen))
    print("token escrito en {}".format(ruta_token))
    servir(api, args.puerto, token)
    return 0


if __name__ == "__main__":
    sys.exit(main())
