# Instalación y prueba de la ronda 8d: add-in 0.8.4, la ventana del plan que se queda abierta

Objetivo: desplegar el add-in **0.8.4** (cierre de la ronda 8c, `docs/fases/fase-8.md`, sección 10) y comprobar sobre la
cercha de la ronda 8c (la de la 8b **con los cordones superior e inferior**) las cuatro correcciones: (1) la ventana del
plan **no es modal**: se queda abierta mientras la persona orbita y pincha en Revit; (2) **Ver en Revit** hace zoom al nudo
**sin ningún cuadro** y sin cerrar la ventana; (3) **Descartar plan** desde la ventana quita **todos** los cubos y rombos;
(4) el sondeo 19 con imagen BMP y la búsqueda del manejador de clics. Unos 35 minutos sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta ronda no crea ninguna conexión.**

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana
de PowerShell. Todos los comandos van en la misma ventana, en orden; si abres otra ventana, repite las tres primeras
líneas del paso 8d-1. **Revit lo abre y lo cierra la persona**, no el instalador. Los pasos marcados **(la persona)** se
hacen en Revit; los marcados **(instalador)** van en PowerShell. **Novedad de esta ronda: la ventana del plan puede quedarse
abierta mientras el instalador llama al puente** (es lo que se prueba en 8d-4); en los demás pasos, como siempre, el
instalador no ejecuta nada mientras una ventana del add-in esté abierta. La variable del plan se llama **`$plan`**. La salida
de `probar_conexiones.py --puente` y el log del día se anotan en el archivo con `Anota` (esta vez **80 líneas** del log, no
30: en la 8c las 30 últimas no alcanzaron a ver qué dejó los cubos).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 8d-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la 8c terminó con `remaining_markers: 0`).
- Para el paso 8d-3 la persona selecciona **la misma cercha de la 8c**: el cordón central entero en ocho tramos, sus 48
  diagonales y los ocho tramos del cordón superior (64 elementos). **Y esta vez mira si existe el cordón inferior** como
  barra: en la 8c, en el nivel de abajo (Z ≈ 14,9 m) no entró ninguna barra horizontal (`fase-8.md`, 10.2). Si existe y se
  puede seleccionar, selecciónalo también y anota cuántos elementos quedan; si no existe como barra de estructura (es
  otra cosa: una viga de otra categoría, un muro…), anota qué es.

### 8d-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-8d.md"
"# Resultados de la ronda 8d`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "8d-1 git" { git log -1 --oneline; git status --short }
Anota "8d-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "8d-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "8d-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "8d-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "8d-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Cierre de la ronda 8c: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 177`**, `8d-1 revit cerrado`
vacío, `== MotorConexiones 0.8.4.0 desplegado en Revit 2027 ==`, `copiado revit_mcp\conexiones.py (23 rutas @api.route)`,
`copiado tools\conn_tools.py (21 herramientas @mcp.tool)` y `0.8.4.0` en la versión de la DLL. Si el build falla o sale
`1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 8d-2. Abrir Revit, ping y sondeo 17

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de siempre, donde se ve la cercha entera. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "8d-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "8d-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "8d-2 sondeo 17 marcas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `ping`: `addin_version: 0.8.4` (en `data` y en `meta`) y 21 operaciones. En `catalog_list`,
   `templates_count: 1` con `Nudo tipico Detalle D` (`6abcf116-9b97-485f-b50d-2851ca0018cc`). En el sondeo 17, los diez pasos
   como en la 8c, con `2) Marcadores de plan … : 0`.

### 8d-3. La ventana que se queda abierta **(la persona; el instalador solo mira)**

1. Selecciona la cercha (ver "Antes de empezar") y pulsa ARBA > **Planificar lote**. La ventana del plan se abre **y Revit
   sigue vivo detrás**: la barra de estado (abajo) dice "… La ventana se queda abierta: orbita y pincha en Revit cuando
   quieras." Anota cuántos elementos seleccionaste y la cabecera (`summary_text`).
2. **Orbita**: con la ventana abierta a un lado, gira la vista 3D (Mayús + rueda o el ViewCube), haz zoom con la rueda y
   pincha una barra cualquiera en el modelo. Todo debe funcionar sin cerrar la ventana. Si la ventana se queda detrás de
   Revit al pinchar en el modelo, anótalo (debe quedarse delante: es hija de la ventana de Revit).
3. **Ver en Revit**: elige en la tabla el nudo gemelo del Detalle D (`▲ Listo con aviso` o `● Listo` en el cordón central)
   y pulsa **Ver en Revit**. Revit debe hacer zoom al nudo y dejar seleccionadas sus barras y su marcador, **sin ningún
   cuadro** (ni de Revit ni del add-in) y **sin cerrar la ventana**; la barra de estado dice "Nudo N… encuadrado en la
   vista {3D} (… elementos seleccionados)…". Haz lo mismo con **doble clic** en un círculo del mapa. Anota si salió algún
   cuadro y cuánto tardó.
4. **Mientras Revit trabaja**: pulsa **Replanificar** y mira la barra de estado: durante un instante dice "⏳ Replanificando
   en Revit…" con los botones apagados, y al terminar se encienden solos y dice "Replanificado con la misma selección…".
   Anota si algún botón se quedó apagado.
5. **Pinchar en Revit con la ventana abierta**: clic derecho sobre un nudo `✖ Falta el cordón` del cordón superior (N9, N16,
   N23… en la 8c) > **Cordón…** > botón de pinchar en Revit: la ventana se apaga con "⏳ Pincha en Revit el cordón de N…",
   Revit pide pinchar; **pulsa Esc**: la barra de estado dice "Sin cambios (elección cancelada)." y los botones vuelven.
   Repite y esta vez pincha uno de los dos tramos del cordón superior: replanifica y el nudo cambia a `✖ Sin plantilla que
   encaje` (es un nudo de dos diagonales: el Detalle D no es su típica, `fase-8.md` 10.2). Lee en el detalle del nudo (panel
   de abajo) el consejo y anótalo tal cual.
6. **Más… > Añadir nudo…**: pulsa Esc en Revit; debe decir "Sin cambios (elección cancelada)." sin cerrar la ventana.
7. **Excluir / Incluir** por clic derecho sobre un listo: gris en el modelo y vuelta; **Editar nudo** sobre el gemelo del
   Detalle D: se abre la previsualización (esta sí es modal); cambia el alto de la cartela de 530 a 500 con doble clic y
   cierra con Cancelar: la fila pasa a `(editado)`; **Quitar edición** la devuelve. Anota si la previsualización se abrió
   delante de la ventana del plan y si al cerrarla la ventana del plan seguía viva.
8. **Planificar lote con la ventana abierta**: sin cerrar la ventana, selecciona en el modelo solo las cuatro barras del
   Detalle D (cordón y tres diagonales de la cercha de la Fase 3, Y ≈ −17 196) y pulsa **Planificar lote** otra vez. No debe
   abrirse una segunda ventana: la misma se actualiza con un plan de 1 nudo listo (`Se creará 1 conexión…`) y el aviso
   `PLAN_MARKS_REPLACED`. Después, sin selección (Esc en el modelo), pulsa **Planificar lote**: la ventana solo viene delante.
   Vuelve a seleccionar la cercha entera y **Planificar lote** para dejar el plan grande puesto. Captura de la ventana con
   el modelo orbitado detrás: `docs\fases\capturas\fase8d-02-ventana-abierta.png`.
9. **Cerrar** la ventana con el botón Cerrar (el plan y las marcas se quedan). Anota en el chat todo lo que no haya ido
   como se describe y el texto de cualquier cuadro que haya salido.

### 8d-4. Descartar desde la ventana con marcas de otro plan en otra vista **(la persona y el instalador, a la vez)**

Reproduce a propósito lo que dejó los cubos en la 8c: un plan del puente marcado en **otra vista** y la ventana descartando.

1. **(la persona)** Abre una **vista distinta** de la 3D (por ejemplo la sección o un alzado de la cercha) y deja la cercha
   seleccionada (64 elementos).
2. **(instalador)** Con la persona en esa otra vista:

   ```powershell
   $tid = "6abcf116-9b97-485f-b50d-2851ca0018cc"
   Anota "8d-4 batch_plan por el puente en otra vista" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, `is_marked: true`, `marked_view_id` = la vista en la que está la persona (anótalo) y el aviso
   `PLAN_MARKS_REPLACED` (el plan del botón se desmarca). Copia `plan_id`, `marked_view_id` y `marks.marker_element_ids`
   (cuántos).
3. **(la persona)** Vuelve a la **vista 3D**: los cubos del plan del puente se ven **grises** (su color es de la otra vista).
   Pulsa **Planificar lote** sin selección (reabre el último plan, el del puente). Con la ventana abierta, el instalador
   llama al puente (es lo que la ventana no modal permite):
4. **(instalador)**, con la ventana del plan abierta:

   ```powershell
   Anota "8d-4 batch_plan_get con la ventana abierta" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body '{"include_specs":false}' }
   ```

   Se espera `ok: true` con el mismo `plan_id` (el puente y la ventana comparten el plan).
5. **(la persona)** En la ventana, **Más… > Descartar plan** > Sí. La ventana se cierra. Mira la vista 3D y la otra vista:
   **no debe quedar ningún cubo ni rombo**, ni gris ni de color, y las barras sin color.
6. **(instalador)**:

   ```powershell
   Anota "8d-4 sondeo 17 tras descartar desde la ventana" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "8d-4 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
   ```

   Se espera en el sondeo 17 **`2) Marcadores de plan … : 0`** y en `discard all` **`removed_markers: 0`** y
   `remaining_markers: 0` (ya no había nada que quitar). Si `removed_markers` no es 0, la corrección (3) no funciona: copia el
   bloque y, en 8d-8, el log con `ribbon_batch_discard`.

### 8d-5. Catálogo vacío con la ventana abierta **(la persona, con ayuda del instalador para mover archivos)**

1. **(instalador)** Mueve las plantillas a una carpeta aparte (sin borrarlas):

   ```powershell
   New-Item -ItemType Directory -Force "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte" | Out-Null
   Move-Item "$env:LOCALAPPDATA\MotorConexiones\catalogo\*.json" "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte\"
   Anota "8d-5 catalogo vacio" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\catalogo" }
   ```

2. **(la persona)** Con la cercha seleccionada, **Planificar lote**: cabecera "Ningún nudo listo: no hay plantillas…", aviso
   `CATALOG_EMPTY` y botón **Abrir catálogo**. Púlsalo: el catálogo se abre (modal) y al cerrarlo la ventana del plan
   replanifica ("Catálogo cerrado: replanificado…") sin cerrarse. Después **Más… > Descartar plan** > Sí.
3. **(instalador)** Devuelve las plantillas:

   ```powershell
   Move-Item "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte\*.json" "$env:LOCALAPPDATA\MotorConexiones\catalogo\"
   Anota "8d-5 catalogo restaurado" { .\scripts\conn-call.ps1 -Operation catalog_list }
   ```

   Se espera `templates_count: 1`.

### 8d-6. Sondeo 19 v2: etiqueta BMP y clic **(instalador y la persona)**

1. **(la persona)** Ponte en la vista 3D con el Detalle D de la Fase 3 (Y ≈ −17 196) a la vista. Ninguna ventana del add-in
   abierta.
2. **(instalador)**:

   ```powershell
   Anota "8d-6 sondeo 19 etiquetas v2" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera: `2) DB.TemporaryGraphicsManager existe: True` e `InCanvasControlData existe: True`; en `3)` la lista de
   **todos los tipos** de `RevitAPI.dll` y `RevitAPIUI.dll` con `TemporaryGraphics` o `InCanvasControl` en el nombre, con sus
   miembros (**copia esas líneas enteras**: son las que deciden la Fase 10); en `4)` los servicios externos integrados con
   `Temporary` o `Canvas` en el nombre y sus servidores registrados; `5) BMP generado`; `6) InCanvasControlData: ImagePath=…
   | Position=(…) mm`; **`7) Control anadido en la vista …: indice 0`** (en la 8c fallaba aquí por el PNG); `8) Captura:
   …fase8d-01-etiqueta.png`; `9)` o bien "manejador registrado como servidor … pincha la etiqueta en Revit" o bien el motivo
   por el que no se pudo (ninguna interfaz, ningún servicio, o el error de registro: **cópialo entero**); `10) La etiqueta se
   queda puesta…`.
3. **(la persona)** Mira la vista: debe verse un cuadrado blanco con un círculo verde y un **4** en el nudo del Detalle D
   (si no se ve, orbita un poco: los controles temporales se redibujan). Si la captura exportada no la enseña, haz una
   captura de pantalla a mano con el nombre `fase8d-01-etiqueta.png`. **Pincha la etiqueta** una vez: si el manejador se
   registró y Revit avisa de los clics, sale un cuadro "MotorConexiones - sondeo 19: Clic recibido…" (ciérralo). Anota si
   salió el cuadro, y si la etiqueta se mueve con el modelo al orbitar o se queda fija en la pantalla.
4. **(instalador)**:

   ```powershell
   Anota "8d-6 sondeo 19b quitar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera `1) clics anotados en …sondeo19-clics.txt:` con una línea por clic y las propiedades de los datos del clic
   (o "ningún clic anotado…" con el motivo), `3) Control 0 quitado`, `4) …Clear() llamado`, `5) servidor de prueba …
   desactivado` (o el motivo) y `6) Marcadores DirectShape de plan en el modelo … : 0`. **(la persona)**: comprueba que la
   etiqueta ya no está. Si el sondeo dice `PARADA` o falla en algún paso, copia el bloque entero. No bloquea.

### 8d-7. Restos **(instalador, ventanas cerradas)**

```powershell
Anota "8d-7 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "8d-7 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "8d-7 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera `Marcadores de plan … : 0`, `conexiones en el modelo: 0` y `Elementos de acero sueltos encontrados: 0`.

### 8d-8. Puente y log, anotados en el archivo **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 30
Anota "8d-8 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8d-8 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|plan_event|startup" | Select-Object -Last 80 }
Select-String -Path $salida -Pattern "^## 8d-8" | Select-Object LineNumber, Line
```

Se espera **`Resultado: 28/28 pruebas correctas`** (la 27 necesita el servidor arrancado: si falla sola, repite el `Anota`
del puente una vez pasados otros 30 s). En el log, `"addin_version":"0.8.4"`, `ribbon_batch_window_opened` con
`"modeless":true`, `ribbon_batch_show` (Ver en Revit), `ribbon_batch_window_updated` (Planificar lote con la ventana abierta),
`ribbon_batch_window_activated` (sin selección), `ribbon_batch_discard` con `remaining_markers 0` y, si algo falló,
`ribbon_batch_action_failed` o `plan_event_failed` (cópialos). **La última línea debe listar dos secciones `## 8d-8`**: si
falta alguna, repite su `Anota` antes de seguir.

### 8d-9. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "8d-9 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-8d.md docs\fases\capturas
   git commit -m "Ronda 8d: resultados del instalador (0.8.4, ventana del plan no modal, Ver en Revit sin cuadro, descartar limpio, sondeo 19 v2)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 8d-1, 8d-2 y 8d-4 a 8d-8; el `git status --short` literal (antes del commit
   y al final, que debe quedar vacío); las anotaciones de la persona del paso 8d-3 (elementos seleccionados, si existe el
   cordón inferior, si se pudo orbitar y pinchar con la ventana abierta, si Ver en Revit sacó algún cuadro, qué dijo la
   barra de estado, el consejo del nudo del paso 5, si la segunda pulsación de Planificar lote abrió otra ventana) y del
   8d-6 (si se vio la etiqueta y si salió el cuadro del clic); las dos capturas; y el texto de cualquier ventana de error.
   No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
