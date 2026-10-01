<#
.SYNOPSIS
    Ejecuta un archivo IronPython 2.7 dentro de Revit a traves de pyRevit Routes, sin pasar por el MCP.

.DESCRIPTION
    Lee el token de sesion de %LOCALAPPDATA%\RevitMcp\token (lo escribe la extension revit-mcp al
    arrancar Revit), hace POST a http://127.0.0.1:48884/revit_mcp/execute_code/ con el contenido del
    archivo como "code" y muestra tal cual la salida ("output") o el error y el traceback.

    Dentro de Revit el codigo dispone de: doc, DB, revit, clr, System y print (ver
    revit_mcp/code_execution.py del repositorio revit-mcp). Todo corre dentro de un TransactionGroup
    "IA: <descripcion>": un sondeo que solo lee no deja rastro; uno que modifica se deshace con Ctrl+Z.

    Requisitos: Revit 2027 abierto con un documento, pyRevit con el servidor Routes activo (puerto
    48884) y la extension revit-mcp cargada. Funciona en Windows PowerShell 5.1 y en PowerShell 7.

.PARAMETER File
    Ruta del archivo .py a ejecutar (relativa a la carpeta actual o absoluta).

.PARAMETER Description
    Texto corto (max. 60 caracteres) para el nombre del grupo de deshacer. Por defecto, el nombre del archivo.

.PARAMETER TimeoutSec
    Segundos de espera maxima de la respuesta de Revit. Por defecto 180.

.PARAMETER Json
    Muestra el cuerpo JSON completo de la respuesta en vez de solo la salida.

.PARAMETER SinTransaccion
    Envia el codigo a /revit_mcp/conn/dev_exec/ (ruta de desarrollo de mcp/revit_mcp/conexiones.py) en vez de
    a /execute_code/. Alli el codigo corre en contexto de la API de Revit pero SIN TransactionGroup ni
    Transaction envolventes: el sondeo abre y cierra las suyas (o las de la API de acero). Disponibles ademas:
    uidoc, uiapp y UI. Hace falta haber instalado conexiones.py con mcp\instalar-conn.ps1.

.EXAMPLE
    .\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py

.EXAMPLE
    .\scripts\revit-exec.ps1 -File scripts\sondeos\03-llamar-dll.py -Description "Sondeo DLL" -Json

.EXAMPLE
    .\scripts\revit-exec.ps1 -File scripts\sondeos\09-placa-camino-a.py -SinTransaccion

.NOTES
    Codigo de salida: 0 si Revit respondio 200 y el codigo termino sin excepcion; 1 en cualquier otro caso.
    El texto se imprime en UTF-8 para conservar acentos.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$File,

    [string]$Description = "",

    [int]$TimeoutSec = 180,

    [switch]$Json,

    [switch]$SinTransaccion,

    [string]$Url = "",

    [string]$TokenPath = (Join-Path $env:LOCALAPPDATA "RevitMcp\token")
)

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }
if (-not $Url) {
    if ($SinTransaccion) { $Url = "http://127.0.0.1:48884/revit_mcp/conn/dev_exec/" }
    else { $Url = "http://127.0.0.1:48884/revit_mcp/execute_code/" }
}

function Salir([int]$codigo, [string]$mensaje) {
    if ($mensaje) { Write-Output $mensaje }
    exit $codigo
}

# 1. Archivo de codigo
$rutaArchivo = Resolve-Path -LiteralPath $File -ErrorAction SilentlyContinue
if (-not $rutaArchivo) { Salir 1 "ERROR: no existe el archivo '$File'." }
$codigo = [System.IO.File]::ReadAllText($rutaArchivo.Path, [System.Text.Encoding]::UTF8)
if ([string]::IsNullOrWhiteSpace($codigo)) { Salir 1 "ERROR: el archivo '$File' esta vacio." }
# IronPython recibe siempre finales de linea LF, aunque Git haya convertido el archivo a CRLF en Windows.
$codigo = $codigo -replace "`r`n", "`n" -replace "`r", "`n"

# 2. Token de sesion
if (-not (Test-Path -LiteralPath $TokenPath)) {
    Salir 1 "ERROR: no existe $TokenPath. Revit no esta abierto o la extension revit-mcp no ha arrancado (pyRevit > Routes activo)."
}
$token = ([System.IO.File]::ReadAllText($TokenPath)).Trim()
if (-not $token) { Salir 1 "ERROR: el archivo de token $TokenPath esta vacio. Reinicia Revit." }

# 3. Cuerpo de la peticion
if (-not $Description) { $Description = [System.IO.Path]::GetFileName($rutaArchivo.Path) }
if ($Description.Length -gt 60) { $Description = $Description.Substring(0, 60) }
$cuerpo = @{ code = $codigo; description = $Description; token = $token } | ConvertTo-Json -Depth 3 -Compress

# 4. POST con HttpClient (disponible en PowerShell 5.1 y 7)
try { Add-Type -AssemblyName System.Net.Http -ErrorAction SilentlyContinue } catch { }
$cliente = New-Object System.Net.Http.HttpClient
$cliente.Timeout = [TimeSpan]::FromSeconds($TimeoutSec)
$contenido = New-Object System.Net.Http.StringContent($cuerpo, [System.Text.Encoding]::UTF8, "application/json")
$cronometro = [System.Diagnostics.Stopwatch]::StartNew()
try {
    $respuesta = $cliente.PostAsync($Url, $contenido).GetAwaiter().GetResult()
    $estado = [int]$respuesta.StatusCode
    $texto = $respuesta.Content.ReadAsStringAsync().GetAwaiter().GetResult()
} catch {
    $detalle = $_.Exception.Message
    if ($_.Exception.InnerException) { $detalle = $_.Exception.InnerException.Message }
    Salir 1 "ERROR: no se pudo hablar con Revit en $Url ($detalle). Comprueba que Revit esta abierto, que pyRevit tiene Routes activo y que la extension revit-mcp esta cargada."
} finally {
    $cronometro.Stop()
    $cliente.Dispose()
}

$nombre = [System.IO.Path]::GetFileName($rutaArchivo.Path)
Write-Output ("== {0} -> HTTP {1} en {2} ms ==" -f $nombre, $estado, $cronometro.ElapsedMilliseconds)

if ($Json) {
    Write-Output $texto
    if ($estado -eq 200) { exit 0 } else { exit 1 }
}

# 5. Interpretar la respuesta
$datos = $null
try { $datos = $texto | ConvertFrom-Json } catch { $datos = $null }

if ($null -eq $datos) {
    Write-Output "Respuesta no JSON:"
    Salir 1 $texto
}

if ($estado -eq 401) {
    Salir 1 "ERROR 401: token ausente o incorrecto. Si Revit se reinicio, el token cambio: vuelve a ejecutar."
}

if ($SinTransaccion) {
    # Sobre comun del add-in: { ok, data: { output, traceback }, errors, warnings, meta }
    if ($estado -eq 404) { Salir 1 "ERROR 404: la ruta $Url no existe. Instala conexiones.py con .\mcp\instalar-conn.ps1 y recarga pyRevit (o reinicia Revit)." }
    if ($datos.data -and $datos.data.output) { Write-Output $datos.data.output }
    if ($estado -eq 200 -and $datos.ok -eq $true) { exit 0 }
    if ($datos.errors) {
        foreach ($fallo in $datos.errors) {
            Write-Output ("ERROR {0}: {1}" -f $fallo.code, $fallo.message)
            if ($fallo.hint) { Write-Output ("PISTA: " + $fallo.hint) }
        }
    }
    if ($datos.data -and $datos.data.traceback) { Write-Output "TRACEBACK:"; Write-Output $datos.data.traceback }
    if ($datos.warnings) { foreach ($aviso in $datos.warnings) { Write-Output ("AVISO {0}: {1}" -f $aviso.code, $aviso.message) } }
    if (-not $datos.errors) { Write-Output $texto }
    exit 1
}

# Respuesta de /execute_code/
if ($estado -eq 200 -and $datos.status -eq "success") {
    Write-Output $datos.output
    exit 0
}

if ($datos.error) { Write-Output ("ERROR: " + $datos.error) }
if ($datos.error_type) { Write-Output ("TIPO: " + $datos.error_type) }
if ($datos.partial_output) { Write-Output "SALIDA PARCIAL:"; Write-Output $datos.partial_output }
if ($datos.traceback) { Write-Output "TRACEBACK:"; Write-Output $datos.traceback }
if ($datos.hints) { Write-Output "PISTAS:"; $datos.hints | ForEach-Object { Write-Output ("- " + $_) } }
if ($datos.open_transaction) { Write-Output "AVISO: quedo una Transaction abierta en Revit (open_transaction=true). Revisala antes de seguir." }
if (-not $datos.error -and -not $datos.traceback) { Write-Output $texto }
exit 1
