# Instalación y prueba de la ronda 10b: add-in 0.10.1, Planificar lote desde la cinta con una barra, etiquetas pinchables y `--puente` 32/32

Objetivo: desplegar el add-in **0.10.1** (cierre de la Fase 10, `docs/fases/fase-10.md`, sección 7) y comprobar sobre la
**copia** del modelo **solo lo que la Fase 10 no pudo probar**: (1) **Planificar lote desde la cinta con una barra**
seleccionada (en la 0.10.0 el cuadro de la selección asistida moría con "Corresponding button not found: defaultButton" y el
botón no planificaba; ya está corregido); (2) **las etiquetas pinchables**: que se ven, **pinchar la 4 sin ningún cuadro**, la
fila y la barra de estado de la ventana, el **resaltado desde la tabla y desde el mapa**, el clic con la ventana cerrada y el
globo; (3) **`probar_conexiones.py --puente` 32/32** con la copia **abierta y activa** y el servidor del puerto 8000
**arrancado**, sin cambiar de modelo a mitad (en la Fase 10 el `ping` respondió desde `MODELO CERCO` y el servidor no estaba:
14/22). Unos **30 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta ronda no
crea ninguna conexión** (no se pulsa Crear).

Lo demás de la Fase 10 ya está contrastado (`fase-10.md` 7.1): la selección asistida por el puente (63 añadidas, 16 listos),
las 33 etiquetas y las 16 cartelas fantasma puestas y quitadas, Crear 16 dos veces, Ctrl+Z, **Borrar el lote desde la ventana**
(16 borradas), `discard all` a cero, el Encargo para IA y los textos de los avisos. **No se repite nada de eso.**

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell.
Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 10b-1. **Revit lo
abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en
PowerShell. Mientras la ventana del plan está trabajando ("⏳ …"), el instalador no ejecuta nada. **Durante toda la ronda
solo debe estar abierta la copia** `HANGAR_PRUEBA_sondeo.rvt` (ningún otro modelo abierto en Revit): el `ping` y
`probar_conexiones.py` hablan con el documento **activo**, y en la Fase 10 el activo era otro. Si las etiquetas no se ven o el
clic no hace nada, **no es un fallo de la instalación**: anótalo tal cual y sigue.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 10b-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in, sin marcadores y sin etiquetas (la Fase 10 terminó con los
  sondeos 12, 13, 17 y 21 a cero). El paso 10b-2 lo comprueba.
- El catálogo tiene **dos** plantillas: `Nudo tipico Detalle D` y `PRUEBA probar_conexiones` (la dejó la prueba 19 del
  `--puente` de la Fase 9, que se cayó antes de la 28 que la borra). El paso 10b-2 la borra para que Planificar lote case
  con una sola plantilla, como siempre.
- Saber **qué barra pinchar**: una diagonal cualquiera de la cercha de la 8c (la de la Fase 10 fue la **1251723**).

### 10b-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-10b.md"
"# Resultados de la ronda 10b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
$env:PYTHONIOENCODING = "utf-8"
Anota "10b-1 git" { git log -1 --oneline; git status --short }
Anota "10b-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "10b-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "10b-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "10b-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "10b-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Cierre de la Fase 10: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 217`**, `10b-1 revit cerrado`
vacío, `== MotorConexiones 0.10.1.0 desplegado en Revit 2027 ==`, `copiado revit_mcp\conexiones.py (25 rutas @api.route)`,
`copiado tools\conn_tools.py (23 herramientas @mcp.tool)` y `0.10.1.0` en la versión de la DLL. Si el build falla o sale
`1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 10b-2. Abrir Revit, ping, la plantilla sobrante y los restos **(la persona abre; el instalador llama)**

1. **(la persona)** Abre Revit 2027 **solo** con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (cierra cualquier otro
   proyecto que Revit abra) y ponte en la **vista 3D** sombreada de siempre, con la cercha de la 8c a media distancia. No
   selecciones nada y no abras ninguna ventana del add-in.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "10b-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "10b-2 catalog_list antes" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "10b-2 catalog_delete de la plantilla sobrante" { .\scripts\conn-call.ps1 -Operation catalog_delete -Body '{"template_id":"023b17e5-3f5b-42da-9efc-5ce0dcc1547a"}' }
   Anota "10b-2 catalog_list despues" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "10b-2 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "10b-2 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "10b-2 sondeo 21 etiquetas antes" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "10b-2 log de arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|label_handler" | Select-Object -Last 4 }
   ```

   Se espera en `ping`: `addin_version: 0.10.1` (en `data` y en `meta`), 23 operaciones y **`document.title:
   HANGAR_PRUEBA_sondeo`** (si dice otro título, cierra ese proyecto en Revit y repite el `ping`). `catalog_list antes`:
   `templates_count: 2`; `catalog_delete`: `ok: true` con `deleted_template_id: 023b17e5-…` (si ya no existe, `TEMPLATE_NOT_FOUND`
   y se sigue); `catalog_list despues`: **`templates_count: 1`** (`Nudo tipico Detalle D`). Sondeos 12 y 13: `conexiones en el
   modelo: 0` y `Elementos de acero sueltos encontrados: 0`. Sondeo 21: `1) Documento: HANGAR_PRUEBA_sondeo`, `2) Controles en
   el lienzo (GetAll): 0`, `3) Servidor del add-in 3f6c1b2e-…: registrado=True | activo=True`, `5) Marcadores de plan en el
   modelo: 0`. En el log: `"event":"startup"` con `"addin_version":"0.10.1"` y `"label_handler":true`.

### 10b-3. Planificar lote desde la cinta con UNA barra **(la persona)**

1. Pincha **una diagonal** de la cercha de la 8c (solo una) y pulsa ARBA > **Planificar lote**. Ahora debe salir el cuadro
   "**Selección asistida: 63 barras tocan la selección**" con el texto "Se añadieron 63 barras que tocan la selección: 8
   cordones que pasan de largo, 46 barras que llegan y 9 tramos de cordón." y "Seleccionadas: 1. Con las añadidas: 64.", con
   dos opciones grandes (**Planificar con las 64 barras**, **Planificar solo las 1 seleccionadas**) y **Cancelar**. (Los
   números son los que dio el puente en la Fase 10 con la diagonal 1251723; con otra diagonal de la misma cercha deben ser los
   mismos.) **Captura del cuadro**: `docs\fases\capturas\fase10b-01-seleccion-asistida.png`. Anota el texto literal. **Si en
   vez del cuadro sale "No se pudo planificar el lote: …", copia ese texto entero**: es lo que esta ronda corrige.
2. Pulsa **Planificar con las 64 barras**. La ventana se abre con la cabecera "Se crearán 16 conexiones con Nudo tipico
   Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 empalmes del cordón (sin
   plantilla). Ocultos: 8 sin cordón, 18 barras sueltas." y debajo "Plan … · 64 barras seleccionadas (63 añadidas por la
   selección asistida) · plantillas: Nudo tipico Detalle D · marcas puestas en la vista con 33 etiquetas pinchables y 16
   cartelas fantasma". La barra de estado empieza por "Se añadieron 63 barras…" (en azul, no en rojo). Captura de la ventana:
   `docs\fases\capturas\fase10b-02-ventana.png`. Si los listos no son 16 o la cabecera nombra otra plantilla, anótalo.
3. **Cancelar**: pincha otra diagonal, Planificar lote, y en el cuadro pulsa **Cancelar**: no pasa nada y la ventana sigue con
   el plan anterior. **Solo la selección**: otra vez Planificar lote y **Planificar solo las 1 seleccionadas**: sale el cuadro
   "Hay una sola barra seleccionada y no encontré ninguna otra que la toque…" (porque sin añadir nada no hay nudo) y el plan
   anterior sigue. Anótalo.

### 10b-4. Etiquetas pinchables **(la persona)**

1. Mira la cercha en la vista 3D: en cada nudo marcado hay, además del cubo o rombo, una **etiqueta redonda** con el número
   (verde, ámbar, roja o gris según el estado); de paso, en los 16 listos, la **cartela fantasma** (placa transparente del
   contorno del Detalle D). Orbita y haz zoom: las etiquetas mantienen su tamaño en pantalla. Captura general:
   `docs\fases\capturas\fase10b-03-etiquetas.png`. Anota si las etiquetas se ven (todas, algunas, ninguna) y si alguna tapa algo.
2. **Pincha la etiqueta 4** (la de N4, el gemelo del Detalle D). Debe pasar, **sin ningún cuadro**: la etiqueta pasa a círculo
   blanco con el 4 en color (resaltada), la ventana del plan elige la fila N4 y su barra de estado dice "Etiqueta 4 pinchada en
   la vista: N4 · Listo con aviso · Nudo tipico Detalle D · igual. Ver en Revit encuadra el nudo (también doble clic en el
   mapa)." Captura de la ventana y la vista juntas: `docs\fases\capturas\fase10b-04-etiqueta-pinchada.png`. Anota literalmente
   **si salió algún cuadro** (no debe) y si Revit siguió respondiendo (orbita después del clic).
3. En la **tabla** de la ventana elige **N7**: la etiqueta 7 pasa a resaltada y la 4 vuelve a su color. Haz lo mismo con un clic
   en el círculo **12** del **mapa**. Anota si las etiquetas cambian al momento o tardan un poco (cambian cuando Revit queda
   libre), y si alguna se queda resaltada de más.
4. Pincha una etiqueta **roja** (un nudo "Sin plantilla que encaje" del cordón superior): la fila elegida es esa y la barra de
   estado lo dice. Después **Cerrar** la ventana del plan (deja las marcas) y pincha otra etiqueta: se resalta igual y **no sale
   ningún cuadro**; vuelve a abrir la ventana con **Planificar lote sin nada seleccionado** (reabre el último plan) y comprueba
   qué fila está elegida. Anótalo.
5. Pasa el ratón sobre una etiqueta: el globo dice "N4 · Listo con aviso · Nudo tipico Detalle D · igual · pincha para elegirlo
   en la ventana del plan" (o el nudo que sea). Anótalo.
6. Para terminar, **Más… > Descartar plan** > Sí: la ventana se cierra y en el modelo no queda color, cubo, fantasma ni
   etiqueta. Anota si quedó alguna etiqueta.

### 10b-5. `--puente` 32/32 con la copia activa y el servidor arrancado **(instalador; la persona no toca Revit)**

**(la persona)** Antes de que el instalador empiece: ninguna ventana del add-in abierta, la copia `HANGAR_PRUEBA_sondeo` es el
**único** proyecto abierto y su vista 3D está activa. No cambies de modelo ni de vista hasta que termine el paso.

```powershell
Anota "10b-5 sondeo 21 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
Anota "10b-5 sondeo 17 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "10b-5 ping antes del puente" { .\scripts\conn-call.ps1 -Operation ping }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "10b-5 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "10b-5 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera: sondeo 21 con `GetAll: 0` y `5) … : 0 (… fantasmas: 0)`; sondeo 17 a cero; **`ping` con `document.title:
HANGAR_PRUEBA_sondeo`** (si no, para, que la persona active la copia, y repite el `ping`); `servidor 8000 escuchando` con una
fila (`LocalPort 8000`): si sale vacío, espera otros 30 s y repite ese `Anota`; si sigue vacío, abre a mano
`C:\IA\iniciar_servidor_revit.bat` en otra ventana, copia lo que escriba y sigue. Y **`Resultado: 32/32 pruebas correctas`**:
la 2 dice `documento=HANGAR_PRUEBA_sondeo`; la 7 encuentra `HSS2-1/2X2-1/2X3/16`; la 8 y la 9 (`node_info` y `validate` con
los IDs 1249510, 1249630, 1249631 y 1249636) en `ok`; la 19 guarda la plantilla `PRUEBA probar_conexiones` y la 28 la
borra; la 29 planifica con `expand_selection` desde una barra del fixture (añade la cercha entera que contiene al Detalle D:
`añadidas` será un número grande) y la 30 la descarta; la 31 y la 32 necesitan el servidor (`tools/list` con 23
herramientas y `conn_ping` por el puente). **Si fallan solo la 31 y la 32** ("no se pudo conectar con el puente"), repite el
`Anota` del puente pasados otros 30 s. Si falla cualquier otra, copia su bloque entero.

### 10b-6. Catálogo, restos y log, anotados en el archivo **(instalador)**

```powershell
Anota "10b-6 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "10b-6 sondeo 12 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "10b-6 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "10b-6 sondeo 21 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
Anota "10b-6 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_batch_assist|ribbon_batch_plan_failed|ribbon_batch_window|selection_expanded|label_clicked|ribbon_batch_label_clicked|label_click|label_highlight|labels_applied|labels_removed|labels_cleared|labels_failed|plan_event_failed|ribbon_batch_window_error|ribbon_batch_discard" | Select-Object -Last 120 }
Select-String -Path $salida -Pattern "^## 10b-6" | Select-Object LineNumber, Line
```

Se espera `templates_count: 1` (la 28 borró la de la prueba), sondeos 12, 13 y 21 a cero. En el log: `"addin_version":"0.10.1"`;
por cada Planificar lote con una barra, `selection_expanded` (`added: 63`) seguido de **`ribbon_batch_assist`** con `choice`
(`all`, `cancel` o `selection`) y, con `all`, `ribbon_batch_window_opened` o `ribbon_batch_window_updated`; **`label_clicked`**
por cada etiqueta pinchada (con `node`, `highlighted: true` y `window_open` `true` o `false`) y `ribbon_batch_label_clicked`
cuando la ventana estaba abierta; `labels_applied` con `count: 33`, `labels_cleared` al descartar; y **ninguna** línea
`ribbon_batch_plan_failed`, `ribbon_batch_assist_dialog_failed`, `label_click_failed`, `label_highlight_failed`, `labels_failed`,
`plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas enteras). **La última línea debe listar cinco
secciones `## 10b-6`**; si falta alguna, repite su `Anota`.

### 10b-7. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "10b-7 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-10b.md docs\fases\capturas
   git commit -m "Ronda 10b: resultados del instalador (0.10.1, Planificar lote con una barra, etiquetas pinchadas, puente 32/32)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 10b-1, 10b-2, 10b-5 y 10b-6; el `git status --short` literal (antes del commit y
   al final, que debe quedar vacío); las anotaciones de la persona del paso 10b-3 (el texto del cuadro o, si salió, el del
   error; los 16 listos y la cabecera; Cancelar y Solo la selección) y del 10b-4 (si las etiquetas se ven, **si salió algún
   cuadro al pinchar**, qué fila eligió la ventana y qué dijo la barra de estado, el resaltado desde la tabla y el mapa, el clic
   con la ventana cerrada, el globo, si Descartar dejó alguna etiqueta); las cuatro capturas; y el texto de cualquier ventana de
   error. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
