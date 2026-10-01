# Fase 1, tercera ronda: pernos del camino A (instrucciones para el agente instalador)

Objetivo: ejecutar solo el sondeo 10 reescrito (patrón de 4 pernos de Advance Steel) sobre la copia
`HANGAR_PRUEBA_sondeo.rvt`, que ya tiene la placa de la ronda anterior. Cinco minutos.

1. PowerShell en el repositorio, traer el sondeo corregido y preparar `Anota`:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   $salida = "docs\fases\resultados-fase-1.md"
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   ```

2. Revit 2027 abierto con la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (no el original) y pyRevit cargado:

   ```powershell
   Anota "1c-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

   `document.path` debe terminar en `HANGAR_PRUEBA_sondeo.rvt`.

3. **La persona selecciona el nudo** (cordón HSS3X3 + las tres barras), como en las rondas anteriores. Si la placa de
   la ronda 1b queda seleccionada también, no importa: el sondeo ignora lo que no sea armazón estructural.

4. Pernos:

   ```powershell
   Anota "1c-4 camino A sondeo 10 pernos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\10-pernos-camino-a.py -SinTransaccion }
   ```

   Se espera `WriteToDb() OK`, `Commit() OK` y `Elementos nuevos: 1` o más (categoría Bolts o similar). Si crea
   elementos: zoom al nudo en 3D y **captura** `docs\fases\capturas\fase1-04-pernos-a.png`. Si Revit se cierra de golpe,
   anótalo y no repitas.

5. Reiniciar el puente MCP para que aparezca la herramienta `conn_ping` (solo si el puente `main.py` estaba en marcha:
   ciérralo y vuelve a lanzarlo como lo haga la persona habitualmente, o reinicia el cliente de IA que lo lanza). Anota si
   se hizo.

6. Cierra la copia en Revit sin guardar más cambios. (Si la persona lo autoriza) subir resultados:

   ```powershell
   git add docs\fases\resultados-fase-1.md docs\fases\capturas
   git commit -m "Fase 1: resultados de la tercera ronda (pernos camino A) en el PC"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

7. Devuelve la salida de los pasos 2 y 4, la captura si existe, y si hubo ventanas o cierres de Revit.
