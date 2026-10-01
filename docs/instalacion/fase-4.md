# Instalación y pruebas de la Fase 4 (instrucciones para el agente instalador)

Objetivo: instalar en la extensión revit-mcp las 13 rutas `/conn/...` con nombre y las 13 herramientas `conn_*` del MCP,
volver a desplegar el add-in (solo cambia `docs\guide.md`, la guía que devuelve `conn_get_guide`), y comprobar de punta a
punta, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, que todo responde: primero por HTTP directo a Revit
(`mcp\pruebas\probar_conexiones.py`, 17 pruebas) y después a través del puente MCP real (`main.py`, 2 pruebas más).
**Ninguna prueba crea ni borra nada en el modelo.** Veinte minutos.

Regla: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** en el paso 4 (`deploy.ps1` copia la DLL y falla si Revit la tiene cargada).
- Si el cliente de IA (Claude Desktop, Claude Code...) tiene el puente MCP en marcha, ciérralo antes del paso 7 para que
  el puerto 8000 esté libre, y vuelve a abrirlo al final (paso 9) para que cargue las herramientas nuevas.
- Vale Windows PowerShell 5.1 o PowerShell 7. Todos los comandos van en la misma ventana, en orden.

## Pasos

1. PowerShell, rama y archivo de resultados:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   $salida = "docs\fases\resultados-fase-4.md"
   "# Resultados de la Fase 4`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   $ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
   $py = "$ext\.venv\Scripts\python.exe"
   Anota "4-1 git" { git log -1 --oneline }
   ```

   La última línea debe ser un commit "Fase 4: ...".

2. Compilar y pasar las pruebas (no cambió el C#, pero deja constancia):

   ```powershell
   Anota "4-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
   ```

   Se espera `0 Errores` y `Superado: 50`.

3. Comprobar que el Python del `.venv` de la extensión existe y tiene `httpx` y `mcp` (los usan el puente y las pruebas):

   ```powershell
   Anota "4-3 python venv" { & $py --version; & $py -c "import httpx, mcp; print('httpx', httpx.__version__); import importlib.metadata as m; print('mcp', m.version('mcp'))" }
   ```

4. Cerrar Revit, desplegar el add-in (copia la guía nueva) e instalar los archivos del MCP en la extensión:

   ```powershell
   Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
   Anota "4-4 deploy e instalar-conn" { .\scripts\deploy.ps1 -NoBuild; .\mcp\instalar-conn.ps1; Select-String -Path "$ext\startup.py" -Pattern "conn"; Select-String -Path "$ext\tools\__init__.py" -Pattern "conn" }
   ```

   Se espera `copiado revit_mcp\conexiones.py (15 rutas @api.route)` y `copiado tools\conn_tools.py (13 herramientas @mcp.tool)`,
   y en `startup.py` y `tools\__init__.py` las líneas `register_conn_routes` / `register_conn_tools` (ya estaban desde la Fase 1).

5. **Abrir Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si Revit pregunta por el
   add-in sin firmar, pulsa *Always Load*. Espera a que cargue pyRevit (unos 20 s) y comprueba las rutas nuevas:

   ```powershell
   Anota "4-5 ping y guia" { .\scripts\conn-call.ps1 -Operation ping; $token = Get-Content "$env:LOCALAPPDATA\RevitMcp\token"; (Invoke-RestMethod "http://127.0.0.1:48884/revit_mcp/conn/guide/?token=$token").data.guide_markdown.Substring(0, 300) }
   ```

   Se espera `ok: true` con `backend: advancesteel` y las primeras líneas de la guía nueva ("Guía para la IA: crear
   conexiones de acero con MotorConexiones"). Si la segunda orden da **404**, `conexiones.py` no se recargó: anótalo y repite
   el paso 5 tras pyRevit > Reload.

6. **Pruebas de humo contra Revit** (17 pruebas; no tocan el modelo):

   ```powershell
   Anota "4-6 probar_conexiones" { & $py mcp\pruebas\probar_conexiones.py }
   ```

   Se espera `Resultado: 17/17 pruebas correctas`. Si alguna da `[FALLO]`, copia su bloque entero (nombre, detalle y cuerpo).

7. **Puente MCP real**: arrancarlo en segundo plano desde la carpeta de la extensión, repetir las pruebas con `--puente`
   (19 pruebas: las 17 anteriores más `tools/list` y `tools/call conn_ping` en `http://127.0.0.1:8000/mcp`) y pararlo:

   ```powershell
   $puente = Start-Process -FilePath $py -ArgumentList "main.py", "--streamable-http" -WorkingDirectory $ext -PassThru -WindowStyle Hidden -RedirectStandardOutput "$env:TEMP\puente-conn.log" -RedirectStandardError "$env:TEMP\puente-conn.err"
   Start-Sleep -Seconds 10
   Anota "4-7 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
   Stop-Process -Id $puente.Id -Force -ErrorAction SilentlyContinue
   Anota "4-7 log del puente" { Get-Content "$env:TEMP\puente-conn.err" -Tail 30; Get-Content "$env:TEMP\puente-conn.log" -Tail 10 }
   ```

   Se espera `Resultado: 19/19 pruebas correctas` y, en la prueba 18, `herramientas=61 conn_*=13`. Si la 18 dice "no se pudo
   conectar con el puente", mira el log: lo normal es que el puerto 8000 esté ocupado por otro `main.py` (ciérralo y repite el
   paso 7) o que falte un paquete en el `.venv` (copia el error).

8. Registro del add-in de hoy (solo las operaciones que llamaron las pruebas; debe haber `guide`, `types`, `schema`, `node_info`,
   `validate`, `preview`, `create` con `VALIDATION_TOKEN_INVALID`, `list`, `get`, `delete`):

   ```powershell
   Anota "4-8 log add-in" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String '"event":"handle"' | Select-Object -Last 40 }
   ```

9. **Lo hace la persona, no el instalador**: vuelve a abrir el cliente de IA conectado al MCP (reinicia Claude Desktop o
   Claude Code para que relance `main.py`) y comprueba que aparecen las herramientas `conn_ping`, `conn_get_guide`,
   `conn_list_types`, `conn_get_schema`, `conn_get_node_info`, `conn_find_profile`, `conn_validate`, `conn_preview`,
   `conn_create`, `conn_list`, `conn_get`, `conn_update` y `conn_delete`. Pídele: "Llama a conn_ping y después a
   conn_get_guide y resume la guía en tres líneas". Pega su respuesta al final de `docs\fases\resultados-fase-4.md`
   (a mano, bajo un título `## 4-9 cliente de IA`). Si el cliente no muestra las herramientas, copia su configuración del
   servidor MCP (comando y carpeta de trabajo con que lanza `main.py`).

10. Cerrar la copia en Revit **sin guardar** y subir los resultados (autorizado):

    ```powershell
    git add docs\fases\resultados-fase-4.md
    git commit -m "Fase 4: resultados de las pruebas del MCP en el PC"
    git pull --no-rebase origin claude/laughing-pascal-tsxvkt
    git push origin claude/laughing-pascal-tsxvkt
    ```

11. Devuelve la salida completa de los pasos 4-4, 4-5, 4-6, 4-7 y 4-8 tal cual, y si hubo ventanas o cierres de Revit en algún paso.
