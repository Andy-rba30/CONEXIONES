# Instalación y sondeos de la Fase 0 (instrucciones para el agente instalador)

Objetivo: actualizar el repositorio en el PC, comprobar Git y el SDK de .NET 10, y ejecutar cinco
sondeos IronPython dentro de Revit 2027 con `scripts\revit-exec.ps1`. Los sondeos **solo leen**: no
crean ni modifican nada en el modelo. Devuelve la salida literal de cada paso, incluidos los errores.

## Antes de empezar (lo comprueba la persona, no el instalador)

- Revit 2027 abierto **con el modelo de prueba de la cercha** (el `.rvt` con cordón y diagonales HSS).
- pyRevit cargado, con el servidor **Routes** activo (pyRevit > Settings > Routes > Enable Routes Server).
- La extensión `revit-mcp` cargada (es la que escribe el token en `%LOCALAPPDATA%\RevitMcp\token`).
- La sesión de PowerShell debe ser del **mismo usuario de Windows** que tiene Revit abierto (el archivo
  del token solo lo puede leer ese usuario). Vale Windows PowerShell 5.1 o PowerShell 7.

## Pasos

Todos los comandos se escriben en la misma ventana de PowerShell, uno tras otro. Copia la salida
completa de cada paso.

1. Abrir PowerShell y permitir scripts solo en esta ventana:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   ```

2. Traer la rama de trabajo (captura la salida):

   ```powershell
   git fetch origin
   git checkout claude/awesome-babbage-q4826b
   git pull origin claude/awesome-babbage-q4826b
   git log -1 --oneline
   ```

   La última línea debe mostrar el commit "Fase 0: ...". Si `git` no se reconoce, anótalo y sigue.

3. Versiones de Git y del SDK de .NET (captura la salida; si `dotnet` no se reconoce, anótalo y sigue):

   ```powershell
   git --version
   dotnet --list-sdks
   ```

4. Comprobar que Revit responde por Routes con el token (captura la salida; **no copies el valor del token**):

   ```powershell
   Test-Path "$env:LOCALAPPDATA\RevitMcp\token"
   $token = Get-Content "$env:LOCALAPPDATA\RevitMcp\token"
   Invoke-RestMethod "http://127.0.0.1:48884/revit_mcp/status/?token=$token"
   ```

   Se espera `status: active` y el título del documento abierto. Si falla, reinicia Revit, espera a que
   cargue pyRevit y repite este paso una vez.

5. Ejecutar los cinco sondeos, uno por uno (captura la salida completa de cada uno, aunque diga `HTTP 500`):

   ```powershell
   .\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py
   .\scripts\revit-exec.ps1 -File scripts\sondeos\01-steel-api.py
   .\scripts\revit-exec.ps1 -File scripts\sondeos\02-perfiles.py
   .\scripts\revit-exec.ps1 -File scripts\sondeos\03-llamar-dll.py
   .\scripts\revit-exec.ps1 -File scripts\sondeos\04-ensamblados.py
   ```

   Cada sondeo tarda unos segundos. Si uno falla con `ERROR: no se pudo hablar con Revit`, vuelve al
   paso 4. Si Revit muestra alguna ventana durante un sondeo, ciérrala con "Cancelar" y anota su texto.
   No repitas un sondeo más de dos veces.

6. Guardar todas las salidas en `docs\fases\resultados-fase-0.md` (copia el bloque entero tal cual):

   ```powershell
   $salida = "docs\fases\resultados-fase-0.md"
   "# Resultados de la Fase 0`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
   "## Entorno`n`n``````text" | Add-Content -Encoding UTF8 $salida
   (git log -1 --oneline 2>&1) | Add-Content -Encoding UTF8 $salida
   (git --version 2>&1) | Add-Content -Encoding UTF8 $salida
   (dotnet --list-sdks 2>&1) | Add-Content -Encoding UTF8 $salida
   "``````" | Add-Content -Encoding UTF8 $salida
   foreach ($sondeo in "00-version", "01-steel-api", "02-perfiles", "03-llamar-dll", "04-ensamblados") {
       "`n## $sondeo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       (& .\scripts\revit-exec.ps1 -File "scripts\sondeos\$sondeo.py" 2>&1) | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   Get-Content $salida | Measure-Object -Line
   ```

7. (Opcional, solo si la persona lo autoriza) Subir el archivo de resultados a la rama de trabajo:

   ```powershell
   git add docs\fases\resultados-fase-0.md
   git commit -m "Fase 0: resultados de los sondeos en el PC"
   git push origin claude/awesome-babbage-q4826b
   ```

8. Devuelve la salida completa de los pasos 2, 3, 4 y 5, y el contenido íntegro de
   `docs\fases\resultados-fase-0.md` del paso 6. Indica también la ruta completa del `.rvt` abierto
   (aparece en la línea `Ruta .rvt:` del sondeo 00) y si tuviste que cerrar alguna ventana de Revit.
