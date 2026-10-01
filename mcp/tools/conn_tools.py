# -*- coding: utf-8 -*-
"""Herramientas conn_* de MotorConexiones para el servidor MCP (CPython 3.11+).

Cada herramienta llama a una ruta /conn/... de Revit y devuelve el JSON integro
(json.dumps con ensure_ascii=False e indent=2), nunca format_response, para que la
IA reciba el sobre comun { ok, data, errors, warnings, meta } sin aplanar.

Fase 1: solo conn_ping. Las demas herramientas de la seccion 9 del encargo llegan
en la Fase 4. Registro: tools/__init__.py hace
    from .conn_tools import register_conn_tools
    register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)
(lo anade mcp/instalar-conn.ps1).
"""
import json

from mcp.server.mcpserver import Context


def _a_texto(respuesta, operation):
    """Sobre comun como texto JSON. Si revit_get/revit_post devolvieron un texto de error, se envuelve."""
    if isinstance(respuesta, str):
        return json.dumps(
            {
                "ok": False,
                "data": None,
                "errors": [
                    {
                        "code": "REVIT_UNREACHABLE",
                        "path": None,
                        "message": respuesta,
                        "hint": "Comprueba que Revit está abierto con pyRevit Routes activo y que "
                                "conexiones.py está instalado (mcp\\instalar-conn.ps1) y pyRevit recargado.",
                    }
                ],
                "warnings": [],
                "meta": {"operation": operation, "duration_ms": 0, "addin_version": None},
            },
            ensure_ascii=False,
            indent=2,
        )
    return json.dumps(respuesta, ensure_ascii=False, indent=2)


def register_conn_tools(mcp, revit_get, revit_post, revit_image=None):
    """Registra las herramientas conn_* en el servidor MCP."""
    _ = revit_image  # se reserva para conn_preview con imagen (fuera de alcance en v1)

    @mcp.tool()
    async def conn_ping(ctx: Context = None) -> str:
        """Comprueba que el add-in MotorConexiones está cargado en Revit.

        Úsala SIEMPRE antes de cualquier otra herramienta conn_*. No necesita nada
        y no toca el modelo.

        Devuelve el sobre común {ok, data, errors, warnings, meta}. Con ok:true,
        data trae: addin_version, spec_version (versión del contrato JSON),
        backend (cómo se crea la geometría), operations (operaciones del puente),
        revit (version_number, version_build, language), dotnet y document
        (título y ruta del modelo abierto, o null si no hay documento).

        Errores comunes:
        - ADDIN_NOT_LOADED: el add-in no está instalado o Revit no se reinició
          tras instalarlo. Díselo al usuario y para.
        - REVIT_UNREACHABLE: Revit no responde (cerrado, Routes apagado o
          conexiones.py sin instalar).
        """
        respuesta = await revit_get("/conn/ping/", ctx, timeout=15.0)
        return _a_texto(respuesta, "ping")
