# Fase 3, segunda ronda: Advance Steel tras el Commit (instrucciones para el agente instalador)

Objetivo: repetir solo la creación y el borrado de la conexión con el add-in corregido, para ver si las placas y los
pernos salen ahora como elementos de Advance Steel (`SteelProxyElement | Plates/Bolts`) o siguen saliendo por DirectShape.
Diez minutos. Sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, nunca el original.

Regla nueva: **si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión**: copia el error y sigue
con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell.

1. PowerShell, rama y archivo de resultados (se añade al archivo de la primera ronda):

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   $salida = "docs\fases\resultados-fase-3.md"
   "`n# Segunda ronda`n`nFecha: $(Get-Date -Format s)`n" | Add-Content -Encoding UTF8 $salida
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   Anota "3b-1 git" { git log -1 --oneline }
   ```

2. Compilar, cerrar Revit si está abierto, desplegar el add-in y reinstalar `conexiones.py` (cambió):

   ```powershell
   Anota "3b-2 build" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
   Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
   Anota "3b-2 deploy" { .\scripts\deploy.ps1 -NoBuild; .\mcp\instalar-conn.ps1 }
   ```

3. Abrir Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, esperar a pyRevit y comprobar:

   ```powershell
   Anota "3b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

4. Crear la conexión (puede tardar minutos si Advance Steel entra de verdad; no lo interrumpas):

   ```powershell
   Anota "3b-4 sondeo 11 crear" { .\scripts\revit-exec.ps1 -File scripts\sondeos\11-fase3-crear.py -SinTransaccion -TimeoutSec 900 }
   ```

   Lo que decide esta ronda está en las últimas líneas: `[id] SteelProxyElement | Plates` / `| Bolts` significa Advance Steel;
   `DirectShape | Structural Connections` significa reserva. Copia también los avisos de `create`.

5. Captura del nudo desde dentro de Revit (no hace falta tocar la pantalla):

   ```powershell
   Anota "3b-5 captura" { .\scripts\revit-exec.ps1 -File scripts\sondeos\capturar-nudo.py -SinTransaccion }
   Rename-Item "docs\fases\capturas\fase3-captura.png" "fase3-04-segunda-ronda.png" -Force
   ```

6. Borrar y comprobar que el nudo queda limpio (sin placas ni pernos de ningún tipo):

   ```powershell
   Anota "3b-6 sondeo 12 borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "3b-6 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   El sondeo 13 debe decir `Elementos de acero sueltos encontrados: 0`. Si encuentra alguno, es que `conn_delete` no borró
   todo lo de Advance Steel: anótalo (el sondeo los borra).

7. Registro del add-in de hoy (importan las líneas `fabrication_transaction_open`, `advance_steel_*_written`,
   `fabrication_transaction_commit`):

   ```powershell
   Anota "3b-7 log" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "fabrication|advance_steel|create|delete" }
   ```

8. Cerrar la copia en Revit **sin guardar** y subir resultados (autorizado):

   ```powershell
   git add docs\fases\resultados-fase-3.md docs\fases\capturas
   git commit -m "Fase 3: resultados de la segunda ronda (Advance Steel tras el Commit)"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

9. Devuelve la salida de los pasos 3b-4, 3b-6 y 3b-7 tal cual, la captura, y si hubo ventanas o cierres de Revit.
