Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
Set-Location "D:\Proyectos C#\CONEXIONES"
$salida = "docs\fases\resultados-fase-3.md"
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}

Anota "8 sondeo 11 crear" { .\scripts\revit-exec.ps1 -File scripts\sondeos\11-fase3-crear.py -SinTransaccion -TimeoutSec 900 }
