# Instalación y pruebas de la Fase 1 (instrucciones para el agente instalador)

Objetivo: compilar e instalar el add-in MotorConexiones en Revit 2027, instalar las rutas `conn_*` en la
extensión revit-mcp, comprobar `conn_ping` de punta a punta y ejecutar la prueba técnica "placa + 4 pernos"
en el nudo del Detalle D por los dos caminos (B: DirectShape desde C#; A: Advance Steel desde IronPython).
Las pruebas de escritura se hacen **sobre una copia** del modelo (`HANGAR_PRUEBA_sondeo.rvt`), nunca sobre el original.

Devuelve la salida literal de cada paso, incluidos los errores, y las capturas de pantalla indicadas.

## Antes de empezar (lo decide la persona, no el instalador)

- Revit 2027 **cerrado** al empezar (el paso 5 copia la DLL y falla si Revit la tiene cargada).
- pyRevit con el servidor **Routes** activo y la extensión `revit-mcp` cargada (como en la Fase 0).
- La sesión de PowerShell debe ser del mismo usuario de Windows que abre Revit (token en `%LOCALAPPDATA%\RevitMcp\token`).
- Vale Windows PowerShell 5.1 o PowerShell 7. Todos los comandos se escriben en la misma ventana, uno tras otro.
- El paso 12 lo hace la persona: seleccionar en Revit los miembros del nudo del Detalle D (cordón + barras que llegan).

## Pasos

1. Abrir PowerShell, permitir scripts solo en esta ventana, ir al repositorio y preparar el archivo de resultados:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   $salida = "docs\fases\resultados-fase-1.md"
   "# Resultados de la Fase 1`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   ```

   `Anota` muestra la salida en pantalla y la guarda a la vez en `docs\fases\resultados-fase-1.md`. Todos los pasos
   siguientes la usan: **copia los comandos tal cual, con `Anota "..." { ... }` incluido.**

2. Traer la rama de trabajo de esta fase:

   ```powershell
   Anota "2 git" { git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pull origin claude/laughing-pascal-tsxvkt; git log -1 --oneline }
   ```

   La última línea debe mostrar un commit "Fase 1: ...".

3. Compilar y pasar las pruebas del Core en el PC (tarda 1-3 minutos la primera vez; descarga paquetes NuGet):

   ```powershell
   Anota "3 dotnet build y test" { dotnet --list-sdks; dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
   ```

   Se espera `Build succeeded` con `0 Error(s)` y `Passed!` con `Failed: 0`. Si `dotnet build` falla, anótalo y **para aquí**.

4. Comprobar que Revit está cerrado:

   ```powershell
   Anota "4 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
   ```

   Si aparece algún proceso, cierra Revit (guardando si pregunta) y repite el paso.

5. Instalar el add-in en Revit 2027 (copia DLL, manifiesto, `config\limits.json` y `docs\guide.md`):

   ```powershell
   Anota "5 deploy" { .\scripts\deploy.ps1 -NoBuild; Get-ChildItem "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones" -Recurse | Select-Object FullName, Length; Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones.addin" }
   ```

6. Instalar las rutas `conn_*` en la extensión revit-mcp (idempotente; guarda copias `.bak-conn` la primera vez):

   ```powershell
   Anota "6 instalar-conn" { .\mcp\instalar-conn.ps1; Select-String -Path "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\startup.py" -Pattern "conn"; Select-String -Path "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\tools\__init__.py" -Pattern "conn" }
   ```

7. **Abrir Revit 2027** con el modelo `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA.rvt`. Observa y anota:
   - Si Revit muestra una ventana de seguridad sobre el add-in "MotorConexiones" (Unsigned add-in / Complemento sin firmar), pulsa **Always Load** (Cargar siempre) y anótalo.
   - Si aparece cualquier otra ventana de error al arrancar, copia su texto literal.
   - Debe aparecer la pestaña **Conexiones** en la cinta con el botón **Ejecutar especificación JSON**. Púlsalo:
     sale un diálogo "MotorConexiones 0.1.0 está cargado". Haz una **captura de pantalla** de la cinta con ese
     diálogo y guárdala como `docs\fases\capturas\fase1-01-boton.png`. Cierra el diálogo.
   - Espera a que pyRevit termine de cargar (unos 20 segundos más).

8. Comprobar Revit, el token y `conn_ping` (sin MCP):

   ```powershell
   Anota "8 status" { $token = Get-Content "$env:LOCALAPPDATA\RevitMcp\token"; Invoke-RestMethod "http://127.0.0.1:48884/revit_mcp/status/?token=$token" }
   Anota "8 conn ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

   Se espera `ok: true` con `addin_version: 0.1.0` y `revit.version_build: 27.2.0.39`. Si sale `HTTP 404`, pyRevit no cargó
   `conexiones.py`: haz pyRevit > **Reload** y repite una vez. Si sale `ADDIN_NOT_LOADED`, el add-in no se cargó: mira
   `%LOCALAPPDATA%\MotorConexiones\log\` (paso 17) y la ventana de Revit al arrancar.

9. Sondeos de lectura 05 y 06 (el 06 es largo: devuélvelo entero):

   ```powershell
   Anota "9 sondeo 05 bridge ping" { .\scripts\revit-exec.ps1 -File scripts\sondeos\05-bridge-ping.py }
   Anota "9 sondeo 06 steel miembros" { .\scripts\revit-exec.ps1 -File scripts\sondeos\06-steel-miembros.py }
   ```

10. Script de prueba de punta a punta (CPython del `.venv` de la extensión, habla con Revit por HTTP con el token):

    ```powershell
    Anota "10 probar_conexiones" { & "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\.venv\Scripts\python.exe" mcp\pruebas\probar_conexiones.py }
    ```

    Se espera `Resultado: 4/4 pruebas correctas`.

11. (Opcional, solo si la persona lo pide) Herramienta MCP `conn_ping` desde el puente: reinicia el puente `main.py`
    (o el cliente de IA que lo lanza) y pide a la IA que llame a `conn_ping`. Anota si aparece la herramienta y qué devuelve.

12. **La persona selecciona el nudo del Detalle D en Revit**: en una vista 3D o de alzado de la cercha, seleccionar con
    Ctrl el **cordón** (HSS3X3, el horizontal) y **todas las barras que llegan a ese nudo** (dos diagonales superiores,
    el montante y la diagonal inferior; valen las que haya). No seleccionar nada más. Dejarlas seleccionadas y volver a PowerShell.

13. Leer el nudo seleccionado (solo lectura). Copia la línea `.\scripts\conn-call.ps1 -Operation probe_plate_b ...` que imprime al final:

    ```powershell
    Anota "13 sondeo 07 nudo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\07-nudo-seleccion.py }
    ```

    Si dice `Hacen falta al menos 2 miembros`, la selección no llegó: vuelve al paso 12.

14. Guardar una **copia** del modelo y seguir sobre ella (sin transacción envolvente):

    ```powershell
    Anota "14 sondeo 08 copia" { .\scripts\revit-exec.ps1 -File scripts\sondeos\08-guardar-copia.py -SinTransaccion }
    ```

    Se espera `Copia guardada y abierta: ...HANGAR_PRUEBA_sondeo.rvt`. Si dice que el modelo es de trabajo compartido, haz la
    copia a mano (Archivo > Guardar como > Proyecto > `HANGAR_PRUEBA_sondeo.rvt`) y sigue con la copia abierta.
    Comprueba en la barra de título de Revit que el documento abierto es `HANGAR_PRUEBA_sondeo`. Si la selección se perdió, repite el paso 12.

15. **Camino B** (DirectShape desde C#). Pega la línea que imprimió el paso 13 dentro de `Anota` (los números son un ejemplo):

    ```powershell
    Anota "15 camino B probe_plate_b" { .\scripts\conn-call.ps1 -Operation probe_plate_b -Body '{"element_ids":[2372289,2372418,2372419],"chord_element_id":2372289}' }
    ```

    Se espera `ok: true` con `plate.element_id` y 4 `bolts.element_ids`. En Revit, en una vista 3D, haz zoom al nudo:
    debe verse una placa cuadrada de 200 x 200 x 10 mm en el plano de la cercha, centrada en el nudo, con 4 cilindros
    (pernos) atravesándola. **Captura** `docs\fases\capturas\fase1-02-camino-b.png` (lo más cerca posible del nudo).
    Después borra la prueba:

    ```powershell
    Anota "15 camino B probe_delete_b" { .\scripts\conn-call.ps1 -Operation probe_delete_b }
    ```

16. **Camino A** (Advance Steel desde IronPython, dentro del contexto de acero). Comprueba que el documento abierto sigue
    siendo la copia `_sondeo` y que los miembros del nudo siguen seleccionados (si no, repite el paso 12). Después:

    ```powershell
    Anota "16 camino A sondeo 09 placa" { .\scripts\revit-exec.ps1 -File scripts\sondeos\09-placa-camino-a.py -SinTransaccion }
    ```

    Tres resultados posibles; anota cuál:
    - `PARADA: ...`: el sondeo no encontró el contexto de acero o una firma esperada y **no creó nada**. Es un resultado válido.
    - `WriteToDb() OK` y `Elementos nuevos en el documento: N`: haz zoom al nudo y **captura** `docs\fases\capturas\fase1-03-camino-a.png`.
      Después, solo en este caso, intenta los pernos:

      ```powershell
      Anota "16 camino A sondeo 10 pernos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\10-pernos-camino-a.py -SinTransaccion }
      ```

      Si crea pernos, **captura** `docs\fases\capturas\fase1-04-pernos-a.png`.
    - **Revit se cierra de golpe** durante el 09 o el 10: anota en qué sondeo y qué fue lo último impreso (el archivo de
      resultados conserva la salida de los pasos anteriores). Vuelve a abrir Revit con el **original** `HANGAR_PRUEBA.rvt`
      y no repitas el sondeo. Añade al archivo de resultados una línea que diga en qué paso ocurrió:

      ```powershell
      "`nREVIT SE CERRO DURANTE EL PASO 16 (sondeo 09/10)`n" | Add-Content -Encoding UTF8 $salida
      ```

17. Registro del add-in (JSON por línea; todas las llamadas de hoy):

    ```powershell
    Anota "17 log add-in" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\log"; Get-Content "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" }
    ```

18. Cerrar la copia `HANGAR_PRUEBA_sondeo.rvt` en Revit **sin guardar más cambios** (el original no se ha tocado).
    Comprobar el archivo de resultados y las capturas:

    ```powershell
    Get-Content $salida | Measure-Object -Line
    Get-ChildItem docs\fases\capturas
    ```

19. (Opcional, solo si la persona lo autoriza) Subir resultados y capturas a la rama de trabajo:

    ```powershell
    git add docs\fases\resultados-fase-1.md docs\fases\capturas
    git commit -m "Fase 1: resultados de las pruebas en el PC"
    git push origin claude/laughing-pascal-tsxvkt
    ```

20. Devuelve: el contenido íntegro de `docs\fases\resultados-fase-1.md`, las capturas `fase1-01` a `fase1-04` que existan,
    si Revit mostró alguna ventana (y su texto) y si Revit se cerró de golpe en algún paso.
