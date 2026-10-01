# Fase 4, segunda ronda: acentos en el adaptador (instrucciones para el agente instalador)

Objetivo: reinstalar solo `mcp\revit_mcp\conexiones.py` (corregido: en la primera ronda las cinco pruebas que envían una
especificación fallaban con `'unknown' codec can't decode byte 0xe1` antes de llegar al add-in) y repetir las pruebas 4-6 y
4-7. No cambia el C#, ni la guía, ni `conn_tools.py`: no hace falta `deploy.ps1`. Diez minutos. Sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`. **Ninguna prueba crea ni borra nada en el modelo.** Cada ejecución añade una sección
"Segunda ronda" al archivo de resultados.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue. No crees
scripts nuevos; usa `Anota` en la ventana de PowerShell. Si tu cliente de IA tiene el puente MCP en marcha en el puerto 8000,
ciérralo antes del paso 4 (en la primera ronda el instalador tuvo que parar dos procesos `main.py`).

1. PowerShell, rama y archivo de resultados (se añade al archivo de la primera ronda):

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   $salida = "docs\fases\resultados-fase-4.md"
   "`n# Segunda ronda`n`nFecha: $(Get-Date -Format s)`n" | Add-Content -Encoding UTF8 $salida
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   $ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
   $py = "$ext\.venv\Scripts\python.exe"
   Anota "4b-1 git" { git log -1 --oneline }
   ```

2. Cerrar Revit (pyRevit solo recarga `conexiones.py` al arrancar) y reinstalar los archivos del MCP:

   ```powershell
   Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
   Anota "4b-2 instalar-conn" { .\mcp\instalar-conn.ps1; Select-String -Path "$ext\revit_mcp\conexiones.py" -Pattern "ensure_ascii=False" }
   ```

   Se espera `copiado revit_mcp\conexiones.py (15 rutas @api.route)` y una línea con `ensure_ascii=False` (es la corrección).

3. **Abrir Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`**, esperar a pyRevit (unos 20 s) y comprobar:

   ```powershell
   Anota "4b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

4. Pruebas de humo contra Revit (17) y, con el puente, 19:

   ```powershell
   Anota "4b-4 probar_conexiones" { & $py mcp\pruebas\probar_conexiones.py }
   $puente = Start-Process -FilePath $py -ArgumentList "main.py", "--streamable-http" -WorkingDirectory $ext -PassThru -WindowStyle Hidden -RedirectStandardOutput "$env:TEMP\puente-conn.log" -RedirectStandardError "$env:TEMP\puente-conn.err"
   Start-Sleep -Seconds 10
   Anota "4b-4 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
   Stop-Process -Id $puente.Id -Force -ErrorAction SilentlyContinue
   ```

   Se espera `Resultado: 17/17` y `Resultado: 19/19`. Lo que decide esta ronda son las pruebas 9 a 13: la 9 debe dar
   `is_valid=True` con un token de 64 caracteres y la 12 un resumen con `bolts: 4`. Si vuelven a fallar, copia el bloque
   entero de la prueba 9 (nombre, detalle y cuerpo).

5. Registro del add-in de hoy (ahora sí debe haber líneas `validate`, `preview` y `create` con `VALIDATION_TOKEN_INVALID`):

   ```powershell
   Anota "4b-5 log add-in" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String '"operation":"(validate|preview|create)"' | Select-Object -Last 12 }
   ```

6. Cerrar la copia en Revit **sin guardar** y subir los resultados (autorizado):

   ```powershell
   git add docs\fases\resultados-fase-4.md
   git commit -m "Fase 4: resultados de la segunda ronda (acentos en el adaptador)"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

7. Devuelve la salida de los pasos 4b-2, 4b-4 y 4b-5 tal cual, y si hubo ventanas o cierres de Revit.

Después (lo hace la persona): vuelve a abrir tu cliente de IA para que relance el puente `main.py` con las herramientas
`conn_*`, y haz el paso 4-9 de `docs\instalacion\fase-4.md` (pedirle `conn_ping` y `conn_get_guide`).
