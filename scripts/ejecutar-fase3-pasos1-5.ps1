Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
Set-Location "D:\Proyectos C#\CONEXIONES"
$salida = "docs\fases\resultados-fase-3.md"
"# Resultados de la Fase 3`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}

Anota "2 git" { git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pull --no-rebase origin claude/laughing-pascal-tsxvkt; git log -1 --oneline }
Anota "3 dotnet build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "4 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
