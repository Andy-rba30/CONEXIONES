<#
.SYNOPSIS
    Compila MotorConexiones en Release y lo copia a la carpeta de add-ins de Revit 2027 (solo en el PC).

.DESCRIPTION
    1. dotnet build MotorConexiones.sln -c Release (salvo -NoBuild).
    2. Copia MotorConexiones.Revit.dll, MotorConexiones.Core.dll (y .pdb) a
       %APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\
    3. Copia config\limits.json y docs\guide.md a subcarpetas config\ y docs\ de esa carpeta.
    4. Escribe el manifiesto %APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones.addin con la ruta
       absoluta de la DLL (sustituye __ASSEMBLY_PATH__ del manifiesto del proyecto).

    Revit debe estar CERRADO: si esta abierto, la DLL esta bloqueada y la copia falla.
    Con -Uninstall borra el manifiesto y la carpeta del add-in.

.EXAMPLE
    .\scripts\deploy.ps1

.EXAMPLE
    .\scripts\deploy.ps1 -NoBuild

.NOTES
    Codigo de salida 0 si todo se copio; 1 si fallo la compilacion o alguna copia.
#>
[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RevitVersion = "2027",
    [switch]$NoBuild,
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

$raiz = Split-Path -Parent $PSScriptRoot
$solucion = Join-Path $raiz "MotorConexiones.sln"
$salidaBin = Join-Path $raiz "src\MotorConexiones.Revit\bin\$Configuration\net10.0-windows"
$manifiestoOrigen = Join-Path $raiz "src\MotorConexiones.Revit\MotorConexiones.addin"
$carpetaAddins = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
$destino = Join-Path $carpetaAddins "MotorConexiones"
$manifiestoDestino = Join-Path $carpetaAddins "MotorConexiones.addin"

function Salir([int]$codigo, [string]$mensaje) {
    if ($mensaje) { Write-Output $mensaje }
    exit $codigo
}

if ($Uninstall) {
    if (Test-Path -LiteralPath $manifiestoDestino) { Remove-Item -LiteralPath $manifiestoDestino -Force; Write-Output "Borrado $manifiestoDestino" }
    if (Test-Path -LiteralPath $destino) { Remove-Item -LiteralPath $destino -Recurse -Force; Write-Output "Borrada la carpeta $destino" }
    Salir 0 "MotorConexiones desinstalado de Revit $RevitVersion."
}

$procesosRevit = @(Get-Process -Name "Revit" -ErrorAction SilentlyContinue)
if ($procesosRevit.Count -gt 0) {
    Write-Output "AVISO: Revit esta abierto ($($procesosRevit.Count) proceso(s)). Si la DLL ya estaba cargada, la copia fallara: cierra Revit y repite."
}

# 1. Compilar
if (-not $NoBuild) {
    Write-Output "== dotnet build $solucion -c $Configuration =="
    & dotnet build $solucion -c $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { Salir 1 "ERROR: la compilacion fallo (codigo $LASTEXITCODE). No se copia nada." }
}

$dll = Join-Path $salidaBin "MotorConexiones.Revit.dll"
if (-not (Test-Path -LiteralPath $dll)) { Salir 1 "ERROR: no existe $dll. Compila primero (quita -NoBuild)." }

# 2. Copiar binarios
try {
    New-Item -ItemType Directory -Path $destino -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $destino "config") -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $destino "docs") -Force | Out-Null
    $copiados = @()
    foreach ($archivo in Get-ChildItem -LiteralPath $salidaBin -File | Where-Object { $_.Extension -in ".dll", ".pdb" }) {
        Copy-Item -LiteralPath $archivo.FullName -Destination (Join-Path $destino $archivo.Name) -Force
        $copiados += $archivo.Name
    }
    # 3. Configuracion y guia (editables sin recompilar)
    $limits = Join-Path $raiz "config\limits.json"
    $guia = Join-Path $raiz "docs\guide.md"
    if (Test-Path -LiteralPath $limits) { Copy-Item -LiteralPath $limits -Destination (Join-Path $destino "config\limits.json") -Force; $copiados += "config\limits.json" }
    if (Test-Path -LiteralPath $guia) { Copy-Item -LiteralPath $guia -Destination (Join-Path $destino "docs\guide.md") -Force; $copiados += "docs\guide.md" }
} catch {
    Salir 1 ("ERROR al copiar a {0}: {1}. Si Revit esta abierto, cierralo y repite." -f $destino, $_.Exception.Message)
}

# 4. Manifiesto con la ruta absoluta de la DLL
$rutaDll = Join-Path $destino "MotorConexiones.Revit.dll"
$manifiesto = [System.IO.File]::ReadAllText($manifiestoOrigen, [System.Text.Encoding]::UTF8)
$manifiesto = $manifiesto.Replace("<!-- scripts/deploy.ps1 sustituye __ASSEMBLY_PATH__ por la ruta absoluta de la DLL desplegada. -->", "<!-- Ruta escrita por scripts/deploy.ps1 -->")
$manifiesto = $manifiesto.Replace("__ASSEMBLY_PATH__", $rutaDll)
# Con BOM: Revit lo lee igual y Windows PowerShell 5.1 muestra bien los acentos de la ruta.
[System.IO.File]::WriteAllText($manifiestoDestino, $manifiesto, (New-Object System.Text.UTF8Encoding($true)))

$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($rutaDll).FileVersion
Write-Output "== MotorConexiones $version desplegado en Revit $RevitVersion =="
Write-Output ("Carpeta:     " + $destino)
Write-Output ("Manifiesto:  " + $manifiestoDestino)
Write-Output ("Copiados:    " + ($copiados -join ", "))
Write-Output "Siguiente paso: abre Revit $RevitVersion. Debe aparecer el panel 'Conexiones' en la pestana 'ARBA' (o la pestana 'Conexiones' si ARBA falla; mira el log)."
exit 0
