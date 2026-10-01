<#
.SYNOPSIS
    Instala los archivos conn_* de MotorConexiones en la extension pyRevit revit-mcp (solo en el PC). Idempotente.

.DESCRIPTION
    1. Copia mcp\revit_mcp\conexiones.py  -> <extension>\revit_mcp\conexiones.py
    2. Copia mcp\tools\conn_tools.py      -> <extension>\tools\conn_tools.py
    3. En <extension>\startup.py, dentro de register_routes(), anade si faltan:
           from revit_mcp.conexiones import register_conn_routes
           register_conn_routes(api)
    4. En <extension>\tools\__init__.py anade si faltan:
           from .conn_tools import register_conn_tools
           register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)
    Antes de tocar startup.py y tools\__init__.py guarda una copia .bak-conn (solo la primera vez).
    Nada mas se modifica. Despues: pyRevit > Reload (o reiniciar Revit) y reiniciar el puente main.py.

.PARAMETER Extension
    Carpeta de la extension. Por defecto C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension

.EXAMPLE
    .\mcp\instalar-conn.ps1

.NOTES
    Codigo de salida 0 si todo quedo instalado; 1 si falta un archivo o no se encontro el punto de insercion.
#>
[CmdletBinding()]
param(
    [string]$Extension = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
)

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

$utf8SinBom = New-Object System.Text.UTF8Encoding($false)
$origen = $PSScriptRoot
$cambios = @()

function Salir([int]$codigo, [string]$mensaje) {
    if ($mensaje) { Write-Output $mensaje }
    exit $codigo
}

function Leer([string]$ruta) {
    return [System.IO.File]::ReadAllText($ruta, [System.Text.Encoding]::UTF8)
}

function Escribir([string]$ruta, [string]$texto) {
    [System.IO.File]::WriteAllText($ruta, $texto, $utf8SinBom)
}

function Copia-Seguridad([string]$ruta) {
    $bak = $ruta + ".bak-conn"
    if (-not (Test-Path -LiteralPath $bak)) { Copy-Item -LiteralPath $ruta -Destination $bak -Force }
}

# 0. Comprobaciones
if (-not (Test-Path -LiteralPath $Extension)) { Salir 1 "ERROR: no existe la carpeta de la extension: $Extension" }
$startup = Join-Path $Extension "startup.py"
$toolsInit = Join-Path $Extension "tools\__init__.py"
foreach ($ruta in @($startup, $toolsInit, (Join-Path $origen "revit_mcp\conexiones.py"), (Join-Path $origen "tools\conn_tools.py"))) {
    if (-not (Test-Path -LiteralPath $ruta)) { Salir 1 "ERROR: falta el archivo $ruta" }
}

# 1 y 2. Copiar los modulos nuevos (siempre: asi se actualizan con cada fase)
foreach ($par in @(
        @{ De = "revit_mcp\conexiones.py"; A = "revit_mcp\conexiones.py" },
        @{ De = "tools\conn_tools.py";     A = "tools\conn_tools.py" })) {
    $de = Join-Path $origen $par.De
    $a = Join-Path $Extension $par.A
    Copy-Item -LiteralPath $de -Destination $a -Force
    $cambios += "copiado $($par.A)"
}

# 3. startup.py: registrar las rutas dentro de register_routes()
$texto = Leer $startup
if ($texto -notmatch "register_conn_routes") {
    $ancla = '        logger.info("All MCP routes registered successfully")'
    $indice = $texto.IndexOf($ancla)
    if ($indice -lt 0) {
        Salir 1 "ERROR: en startup.py no se encontro la linea ancla '$ancla'. Anade a mano, dentro de register_routes(): from revit_mcp.conexiones import register_conn_routes / register_conn_routes(api)"
    }
    $nl = if ($texto.Contains("`r`n")) { "`r`n" } else { "`n" }
    $insercion = "        from revit_mcp.conexiones import register_conn_routes" + $nl + $nl + "        register_conn_routes(api)" + $nl + $nl
    Copia-Seguridad $startup
    Escribir $startup ($texto.Substring(0, $indice) + $insercion + $texto.Substring($indice))
    $cambios += "startup.py: anadido register_conn_routes(api)"
} else {
    $cambios += "startup.py: ya tenia register_conn_routes"
}

# 4. tools\__init__.py: importar y registrar las herramientas
$texto = Leer $toolsInit
if ($texto -notmatch "register_conn_tools") {
    $nl = if ($texto.Contains("`r`n")) { "`r`n" } else { "`n" }
    # Anclas: la ultima linea "from .xxx import register_xxx" y la ultima "register_xxx(mcp_server, ..." del archivo.
    # (La extension del PC no es identica al repositorio revit-mcp: no se depende de document_tools.)
    $lineas = $texto -split "`n"
    $iImport = -1
    $iRegistro = -1
    for ($n = 0; $n -lt $lineas.Count; $n++) {
        if ($lineas[$n] -match '^\s+from \.\w+ import register_\w+\s*$') { $iImport = $n }
        if ($lineas[$n] -match '^\s+register_\w+\(') { $iRegistro = $n }
    }
    if ($iImport -lt 0 -or $iRegistro -lt 0) {
        Salir 1 "ERROR: en tools\__init__.py no se encontro ninguna linea 'from .xxx import register_xxx' o 'register_xxx(mcp_server, ...'. Devuelve el contenido del archivo. Anade a mano, dentro de register_tools(): from .conn_tools import register_conn_tools / register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)"
    }
    # Si la llamada de registro ocupa varias lineas, avanzar hasta la que cierra el parentesis.
    while ($iRegistro -lt $lineas.Count - 1 -and ($lineas[$iRegistro] -notmatch '\)\s*$')) { $iRegistro++ }
    $sangriaImport = ([regex]::Match($lineas[$iImport], '^\s+')).Value
    $sangriaRegistro = ([regex]::Match($lineas[$iRegistro], '^\s+')).Value
    if (-not $sangriaRegistro) { $sangriaRegistro = $sangriaImport }
    # Insertar primero la de mas abajo para no mover indices
    $cr = if ($texto.Contains("`r`n")) { "`r" } else { "" }
    $nuevas = New-Object System.Collections.Generic.List[string]
    for ($n = 0; $n -lt $lineas.Count; $n++) {
        $nuevas.Add($lineas[$n])
        if ($n -eq $iImport) { $nuevas.Add($sangriaImport + "from .conn_tools import register_conn_tools" + $cr) }
        if ($n -eq $iRegistro) { $nuevas.Add($sangriaRegistro + "register_conn_tools(mcp_server, revit_get_func, revit_post_func, revit_image_func)" + $cr) }
    }
    $texto = ($nuevas -join "`n")
    Copia-Seguridad $toolsInit
    Escribir $toolsInit $texto
    $cambios += "tools\__init__.py: anadido register_conn_tools(...)"
} else {
    $cambios += "tools\__init__.py: ya tenia register_conn_tools"
}

Write-Output "== MotorConexiones: archivos conn_* instalados en $Extension =="
$cambios | ForEach-Object { Write-Output ("- " + $_) }
Write-Output "Siguiente paso: pyRevit > Reload (o reinicia Revit) y reinicia el puente MCP (main.py) si estaba en marcha."
exit 0
