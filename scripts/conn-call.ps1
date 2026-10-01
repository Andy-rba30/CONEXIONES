<#
.SYNOPSIS
    Llama a una operacion del add-in MotorConexiones a traves de las rutas /conn/ de pyRevit Routes (sin MCP).

.DESCRIPTION
    - ping            -> GET  http://127.0.0.1:48884/revit_mcp/conn/ping/
    - cualquier otra  -> POST http://127.0.0.1:48884/revit_mcp/conn/op/<operacion>/ con el cuerpo JSON de -Body
    El token de sesion se lee de %LOCALAPPDATA%\RevitMcp\token y se anade solo. La respuesta es siempre el
    sobre comun del add-in: { ok, data, errors, warnings, meta }. Se muestra con sangria y acentos legibles.

    Requisitos: Revit 2027 abierto, pyRevit con Routes activo, extension revit-mcp con conexiones.py
    instalado (mcp\instalar-conn.ps1) y el add-in MotorConexiones desplegado (scripts\deploy.ps1).

.PARAMETER Operation
    Nombre de la operacion: ping, probe_plate_b, probe_delete_b, ...

.PARAMETER Body
    Objeto JSON con los datos de la operacion (por defecto {}). Ejemplo: '{"element_ids":[2372289,2372418]}'

.EXAMPLE
    .\scripts\conn-call.ps1 -Operation ping

.EXAMPLE
    .\scripts\conn-call.ps1 -Operation probe_plate_b -Body '{"element_ids":[2372289,2372418,2372419]}'

.NOTES
    Codigo de salida: 0 si HTTP 200 y ok:true; 1 en cualquier otro caso.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Operation,

    [Parameter(Position = 1)]
    [string]$Body = "{}",

    [int]$TimeoutSec = 180,

    [switch]$Raw,

    [string]$BaseUrl = "http://127.0.0.1:48884/revit_mcp",

    [string]$TokenPath = (Join-Path $env:LOCALAPPDATA "RevitMcp\token")
)

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

function Salir([int]$codigo, [string]$mensaje) {
    if ($mensaje) { Write-Output $mensaje }
    exit $codigo
}

# 1. Token
if (-not (Test-Path -LiteralPath $TokenPath)) {
    Salir 1 "ERROR: no existe $TokenPath. Revit no esta abierto o la extension revit-mcp no ha arrancado (pyRevit > Routes activo)."
}
$token = ([System.IO.File]::ReadAllText($TokenPath)).Trim()
if (-not $token) { Salir 1 "ERROR: el archivo de token $TokenPath esta vacio. Reinicia Revit." }

# 2. Cuerpo: se inserta el token dentro del objeto JSON recibido
$cuerpoRecortado = $Body.Trim()
if (-not $cuerpoRecortado.StartsWith("{") -or -not $cuerpoRecortado.EndsWith("}")) {
    Salir 1 "ERROR: -Body debe ser un objeto JSON entre llaves, por ejemplo '{""element_ids"":[1,2]}'."
}
$interior = $cuerpoRecortado.Substring(1, $cuerpoRecortado.Length - 2).Trim()
if ($interior) { $cuerpo = '{"token":"' + $token + '",' + $interior + '}' } else { $cuerpo = '{"token":"' + $token + '"}' }

# 3. Peticion
try { Add-Type -AssemblyName System.Net.Http -ErrorAction SilentlyContinue } catch { }
$cliente = New-Object System.Net.Http.HttpClient
$cliente.Timeout = [TimeSpan]::FromSeconds($TimeoutSec)
$cronometro = [System.Diagnostics.Stopwatch]::StartNew()
$esPing = ($Operation.ToLowerInvariant() -eq "ping")
if ($esPing) { $url = "$BaseUrl/conn/ping/" } else { $url = "$BaseUrl/conn/op/$Operation/" }
try {
    if ($esPing) {
        $respuesta = $cliente.GetAsync("$url" + "?token=$token").GetAwaiter().GetResult()
    } else {
        $contenido = New-Object System.Net.Http.StringContent($cuerpo, [System.Text.Encoding]::UTF8, "application/json")
        $respuesta = $cliente.PostAsync($url, $contenido).GetAwaiter().GetResult()
    }
    $estado = [int]$respuesta.StatusCode
    $texto = $respuesta.Content.ReadAsStringAsync().GetAwaiter().GetResult()
} catch {
    $detalle = $_.Exception.Message
    if ($_.Exception.InnerException) { $detalle = $_.Exception.InnerException.Message }
    Salir 1 "ERROR: no se pudo hablar con Revit en $url ($detalle). Comprueba que Revit esta abierto, que pyRevit tiene Routes activo y que la extension revit-mcp esta cargada."
} finally {
    $cronometro.Stop()
    $cliente.Dispose()
}

Write-Output ("== conn/{0} -> HTTP {1} en {2} ms ==" -f $Operation, $estado, $cronometro.ElapsedMilliseconds)

if ($Raw) {
    Write-Output $texto
    if ($estado -eq 200) { exit 0 } else { exit 1 }
}

if ($estado -eq 401) { Salir 1 "ERROR 401: token ausente o incorrecto. Si Revit se reinicio, el token cambio: vuelve a ejecutar." }
if ($estado -eq 404) { Salir 1 "ERROR 404: la ruta $url no existe. Instala conexiones.py con .\mcp\instalar-conn.ps1 y recarga pyRevit (o reinicia Revit)." }

$datos = $null
try { $datos = $texto | ConvertFrom-Json } catch { $datos = $null }
if ($null -eq $datos) {
    Write-Output "Respuesta no JSON:"
    Salir 1 $texto
}

# 4. Mostrar con sangria y acentos legibles (ConvertTo-Json escapa los no ASCII como \uXXXX)
$bonito = $datos | ConvertTo-Json -Depth 30
$bonito = [regex]::Replace($bonito, '\\u([0-9a-fA-F]{4})', { param($m) [string][char][Convert]::ToInt32($m.Groups[1].Value, 16) })
Write-Output $bonito

if ($estado -eq 200 -and $datos.ok -eq $true) { exit 0 }
if ($datos.errors) {
    foreach ($fallo in $datos.errors) { Write-Output ("ERROR {0}: {1}" -f $fallo.code, $fallo.message) }
}
exit 1
