# Fase 1, segunda ronda: subir resultados y repetir el camino A (instrucciones para el agente instalador)

Objetivo: (1) subir a la rama los resultados y capturas de la primera ronda; (2) traer las correcciones; (3) repetir
solo la prueba del camino A (sondeos 09 y 10) sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, que ya existe; (4) reintentar
`instalar-conn.ps1` y, si vuelve a fallar, devolver `tools\__init__.py` entero. Revit no se cerró en la primera ronda; en
esta tampoco se espera, pero las reglas del paso 16 de `fase-1.md` siguen valiendo.

## Pasos

1. PowerShell en el repositorio (la función `Anota` guarda cada salida en el archivo de resultados):

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   $salida = "docs\fases\resultados-fase-1.md"
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   ```

2. Subir los resultados y capturas de la primera ronda **antes** de traer cambios (si la persona lo autorizó):

   ```powershell
   Anota "1b-2 subir primera ronda" { git add docs\fases\resultados-fase-1.md docs\fases\capturas; git commit -m "Fase 1: resultados y capturas de la primera ronda en el PC"; git pull --no-rebase origin claude/laughing-pascal-tsxvkt; git push origin claude/laughing-pascal-tsxvkt; git log -3 --oneline }
   ```

   Si `git pull` abre un editor de mensaje de fusión, guarda y cierra (en vim: `:wq`). Si no se autorizó el push,
   ejecuta solo `git pull --no-rebase origin claude/laughing-pascal-tsxvkt` y sigue.

3. Volver a instalar las rutas y la herramienta del MCP con el instalador corregido (no hace falta cerrar Revit):

   ```powershell
   Anota "1b-3 instalar-conn" { .\mcp\instalar-conn.ps1; Select-String -Path "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\tools\__init__.py" -Pattern "conn_tools" }
   ```

   Si vuelve a decir `ERROR: en tools\__init__.py ...`, devuelve el archivo entero:

   ```powershell
   Anota "1b-3 tools __init__.py" { Get-Content "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\tools\__init__.py" }
   ```

4. Revit 2027 abierto con la copia **`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si está
   abierto el original, ciérralo y abre la copia. Espera a que cargue pyRevit. Comprueba que responde:

   ```powershell
   Anota "1b-4 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

   `document.path` debe terminar en `HANGAR_PRUEBA_sondeo.rvt`.

5. **La persona selecciona el nudo** (cordón HSS3X3 + las barras que llegan), como en el paso 12 de la primera ronda.
   Después, leer el nudo:

   ```powershell
   Anota "1b-5 sondeo 07 nudo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\07-nudo-seleccion.py }
   ```

   Comprueba que la línea `Z (normal plano)` tiene ahora componente Y **positiva** (`(0, 1, 0)` aproximadamente).

6. Camino A, placa (sondeo corregido):

   ```powershell
   Anota "1b-6 camino A sondeo 09 placa" { .\scripts\revit-exec.ps1 -File scripts\sondeos\09-placa-camino-a.py -SinTransaccion }
   ```

   Fíjate en la línea `ASGeometryMgd en AppDomain es el mismo ensamblado:` (debe ser `True`) y en `Elementos nuevos en el
   documento`. Si creó elementos: zoom al nudo en 3D y **captura** `docs\fases\capturas\fase1-03-camino-a.png`. La línea
   `caja ... mm` dice el tamaño real: compáralo con 200 × 200 × 10 (si sale ~60960, Advance Steel esperaba mm).

7. Solo si el paso 6 creó la placa, los pernos (vuelve a seleccionar el nudo si la selección se perdió):

   ```powershell
   Anota "1b-7 camino A sondeo 10 pernos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\10-pernos-camino-a.py -SinTransaccion }
   ```

   Si crea pernos, **captura** `docs\fases\capturas\fase1-04-pernos-a.png`.

8. Registro del add-in y cierre:

   ```powershell
   Anota "1b-8 log add-in" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" }
   ```

   Cierra la copia en Revit **sin guardar más cambios** (los sondeos ya guardaron lo que crearon).

9. (Si la persona lo autoriza) Subir esta segunda ronda:

   ```powershell
   git add docs\fases\resultados-fase-1.md docs\fases\capturas
   git commit -m "Fase 1: resultados de la segunda ronda (camino A) en el PC"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

10. Devuelve: la salida de los pasos 2 a 8 tal cual, las capturas nuevas, si Revit mostró alguna ventana (y su texto)
    y si Revit se cerró de golpe en algún paso.
