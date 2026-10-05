# Instalación y prueba de la ronda 8c: add-in 0.8.3, la ventana del plan que se entiende

Objetivo: desplegar el add-in **0.8.3** y comprobar sobre la cercha de la ronda 8b (esta vez **con los cordones superior e
inferior**) que la ventana del plan se entiende: cabecera con la decisión, mapa de la cercha, solo nudos de verdad en la
tabla, estados en español, columna *Qué hacer*, colores por estado en el modelo, aviso de catálogo vacío y el sondeo 19 de
etiquetas en el lienzo. También se comprueba lo que la 0.8.2 dejó sin probar (`docs/fases/fase-8.md`, sección 8.5):
`end_gap_mm` en la respuesta, `PLAN_MARKS_REPLACED`, el conteo de `removed_markers`, `overrides` sin `IsEmpty`, el paso 9
del sondeo 17, `--puente` 28/28 y el log. Unos 50 minutos sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. **Esta ronda no crea ninguna conexión.**

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana
de PowerShell. Todos los comandos van en la misma ventana, en orden; si abres otra ventana, repite las tres primeras
líneas del paso 8c-1. **Revit lo abre y lo cierra la persona**, no el instalador. Los pasos marcados **(la persona)** se
hacen en Revit; los marcados **(instalador)** van en PowerShell. **Mientras una ventana del add-in esté abierta en Revit,
el instalador no ejecuta nada.** La variable del plan se llama **`$plan`** (no `$pid`). **Esta vez la salida de
`probar_conexiones.py --puente` y el log del día se anotan en el archivo con `Anota`** (la 8b los perdió) y al final se
comprueba que el archivo tiene esas dos secciones antes del commit.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 8c-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la 8b terminó con `remaining_markers: 0`).
- Para el paso 8c-3 la persona selecciona **la misma cercha de la 8b** (la gemela en Y = +17204: cordón central entero en
  ocho tramos y sus 48 diagonales, 56 barras) **y además los cordones superior e inferior** de esa cercha (todos sus
  tramos). Anota cuántos elementos quedan seleccionados (se esperan más de 56).

### 8c-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-8c.md"
"# Resultados de la ronda 8c`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "8c-1 git" { git log -1 --oneline; git status --short }
Anota "8c-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "8c-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "8c-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "8c-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "8c-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Ronda 8c: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 177`**, `8c-1 revit cerrado` vacío,
`== MotorConexiones 0.8.3.0 desplegado en Revit 2027 ==`, `copiado revit_mcp\conexiones.py (23 rutas @api.route)`,
`copiado tools\conn_tools.py (21 herramientas @mcp.tool)` y `0.8.3.0` en la versión de la DLL. Si el build falla o sale
`1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 8c-2. Abrir Revit, ping y sondeo 17 entero

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de la 8b donde se ve la cercha entera. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "8c-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "8c-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "8c-2 sondeo 17 marcas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `ping`: `addin_version: 0.8.3` (en `data` y en `meta`) y 21 operaciones. En `catalog_list`,
   `templates_count: 1` con `Nudo tipico Detalle D` y `template_id` `6abcf116-9b97-485f-b50d-2851ca0018cc` (es `$tid`).
   En el **sondeo 17**, los pasos 1 a 8 como en la 8b y además, por primera vez, **`9) Tras limpiar: color valido=False |
   marcador existe=False`** y **`10) TransactionGroup deshecho`** sin traceback (en la 8b moría en el paso 9).

### 8c-3. Planificar la cercha con todos sus cordones **(la persona selecciona, el instalador llama)**

1. **(la persona)** Selecciona la cercha de la 8b **con los cordones superior e inferior** (ver "Antes de empezar") y
   **deja la selección puesta**. Anota el número de elementos. No abras ninguna ventana del add-in.
2. **(instalador)**:

   ```powershell
   $tid = "6abcf116-9b97-485f-b50d-2851ca0018cc"
   Anota "8c-3 batch_plan" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, un `plan_id` (**cópialo**: es `$plan`), `is_marked: true` y, nuevo en la 8c, **`summary_text`** en
   `data` ("Se crearán N conexiones con Nudo tipico Detalle D (… iguales, … en espejo). … avisan de perfil distinto. …"),
   `visible_count`, `hidden_text`, y en cada nudo `status_text` (en español, con icono: `● Listo`, `▲ Listo con aviso`,
   `✖ Falta el cordón`…), `advice` (qué hacer) y `visible_by_default`. `color_name` ya no es un color por nudo sino el del
   estado: `verde`, `ambar`, `rojo` o `gris` (`null` en los ocultos). Con los cordones superior e inferior seleccionados se
   esperan **más nudos listos que los 16 de la 8b** (los de arriba y abajo ya tienen cordón) o, si la plantilla no encaja
   arriba y abajo, nudos `no_match` **con** cordón, `status_text` `✖ Sin plantilla que encaje` y `advice` "Ninguna plantilla
   encaja (N barras, ángulos …): crea esa típica o excluye". Comprueba también lo de la 0.8.2: **`members[]` trae
   `end_gap_mm`** (en N4, el gemelo del Detalle D, unos 84,5 / 19,6 / 48,1 mm). **Copia el bloque entero** aunque sea largo y
   anota aparte `summary_text`, `visible_count` y `hidden_text`.
3. **(la persona)** Mira la vista 3D: cada nudo de verdad lleva ahora el **color de su estado** (verde = se creará, ámbar =
   se creará con aviso de perfil, rojo = falta algo, gris = no se crea) y su marcador (cubo igual, rombo espejo); las barras
   sueltas y las parejas sin cordón **no** llevan color ni marcador. Captura de la cercha entera:
   `docs\fases\capturas\fase8c-01-colores-estado.png`. Anota en el chat cuántos colores distintos ves y si algún nudo con
   cordón quedó sin marcar.
4. **(instalador)** Con `<N>` = el nombre del nudo gemelo del Detalle D (N4 si la numeración no cambió; `status_text` lo
   dice) y el `overrides` **copiado tal cual** de la respuesta anterior (es `{}` o lo que haya):

   ```powershell
   $plan = "<plan_id>"
   Anota "8c-3 batch_plan_get N" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan + '","node":"<N>"}') }
   Anota "8c-3 replan con overrides devuelto" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{},"include_specs":false}') -TimeoutSec 600 }
   Anota "8c-3 replan excluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"exclude":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
   Anota "8c-3 replan incluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"include":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera en `batch_plan_get`: `data.node` con `status_text`, `advice`, la especificación completa y el token. En el
   replan con el `overrides` devuelto: `ok: true` (0.8.2: ya no lleva la clave `IsEmpty`; si saliera `INVALID_REQUEST`,
   cópialo). En el replan con `exclude`: el **mismo** `plan_id`, `<N>` con `status_text` `◌ Excluido`, `color_name` `gris` y
   **en gris en el modelo** (la persona lo mira: en la 8b quedaba sin color, ahora va en gris). En el replan con `include`:
   `<N>` otra vez `● Listo` o `▲ Listo con aviso` con el **mismo token** que en el paso 2. Deja el plan puesto.

### 8c-4. Botón Planificar lote **(la persona)**

1. Con la cercha todavía seleccionada (o vuelve a seleccionarla igual), ARBA > **Planificar lote**. Como el plan del puente
   sigue marcado, la ventana debe decir en la barra de estado (abajo) el aviso **`PLAN_MARKS_REPLACED`** ("Se quitaron las
   marcas del plan …") y en la vista debe quedar **un solo juego** de marcas (0.8.2).
2. Mira la ventana entera y haz la captura `docs\fases\capturas\fase8c-02-mapa.png`. Debe tener: la **cabecera** con la
   decisión (la misma frase que `summary_text`), la línea del plan y las plantillas; la línea "Ocultos: … sin cordón, …
   barras sueltas" con el botón **Mostrar ocultos (N)** y **Ajustar**; el **mapa de la cercha** (barras en gris, el cordón
   más grueso, un círculo por nudo con su número y el color de su estado); la **tabla** con Nudo, Estado (punto de color y
   texto en español), Espejo (no / sí), Plantilla, Desvío y **Qué hacer**; el detalle; la leyenda "Verde = se creará · Ámbar
   = se creará con aviso · Rojo = falta algo · Gris = no se crea"; y los botones **Más…**, **Replanificar**, **Editar nudo**,
   **Ver en Revit** y **Cerrar**. Anota si algo no cabe o se solapa.
3. **Mapa**: pasa el ratón por un círculo (globo "N4 · Listo con aviso · Nudo tipico Detalle D · igual"), haz **clic** en
   uno (su fila se elige en la tabla y el detalle cambia), gira la **rueda** (zoom), **arrastra** (mover), pulsa
   **Ajustar**. Haz **doble clic** en el círculo del Detalle D: la ventana se cierra, Revit selecciona sus barras y el
   marcador y hace zoom, el diálogo dice el nudo en español y qué hacer; al cerrarlo la ventana vuelve con ese nudo elegido.
4. **Mostrar ocultos**: las barras sueltas y las parejas sin cordón aparecen en gris al final de la tabla y como círculos
   vacíos pequeños en el mapa; **Ocultar** las quita.
5. **Clic derecho** sobre un nudo listo: menú con Ver en Revit, Editar nudo, Quitar edición, Excluir, Cordón…, Barras…,
   Plantilla…. Elige **Excluir**: la fila pasa a `◌ Excluido` y el nudo se pone **gris** en el modelo; clic derecho >
   **Incluir** lo devuelve. **Replanificar**: el mapa y la tabla se recalculan sin cambiar los nombres.
6. **Editar nudo** sobre el Detalle D: se abre la previsualización con la cabecera `Nudo N… del plan (Nudo tipico Detalle D,
   igual)`; cambia una cota (el alto de la cartela de 530 a 500 con doble clic) y cierra con **Cancelar**: la fila pasa a
   `● Listo (editado)` o `✖ No valida (editado)` según valide, y clic derecho > **Quitar edición** la devuelve a la plantilla.
   Con un nudo oculto o `✖ Falta el cordón` elegido, el botón **Editar nudo** en gris explica el motivo al pasar el ratón.
7. **Más…** > **Guardar plan JSON** (abajo dice la ruta) y después **Más…** > **Descartar plan** > Sí: colores a la
   normalidad y marcadores fuera. Comprueba que **no queda ningún cubo ni rombo** (0.8.2: antes quedaban los del plan del
   puente) y que Revit no mostró ninguna ventana de `Marca`. Anota en el chat lo que no haya ido como se describe.

### 8c-5. Catálogo vacío **(la persona, con ayuda del instalador para mover archivos)**

1. **(instalador)** Mueve las plantillas a una carpeta aparte (sin borrarlas):

   ```powershell
   New-Item -ItemType Directory -Force "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte" | Out-Null
   Move-Item "$env:LOCALAPPDATA\MotorConexiones\catalogo\*.json" "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte\"
   Anota "8c-5 catalogo vacio" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\catalogo" }
   ```

2. **(la persona)** Con la cercha seleccionada, **Planificar lote**: la cabecera dice "Ningún nudo listo: no hay plantillas
   en el catálogo (crea primero la conexión de un nudo y guárdala con Guardar en catálogo)", la tabla enseña los nudos como
   `✖ Sin plantillas en el catálogo` con el consejo "No hay plantillas: crea primero…", la barra de estado lleva el aviso
   `CATALOG_EMPTY` y abajo a la izquierda aparece el botón **Abrir catálogo**. Púlsalo: se abre la ventana del catálogo
   (vacía); ciérrala y la ventana del plan replanifica. Cierra la ventana del plan con **Cerrar**.
3. **(instalador)** Devuelve las plantillas y comprueba por el puente que el aviso sale también desde la IA:

   ```powershell
   Anota "8c-5 batch_plan catalogo vacio" { .\scripts\conn-call.ps1 -Operation batch_plan -Body '{"include_specs":false}' -TimeoutSec 600 }
   Move-Item "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte\*.json" "$env:LOCALAPPDATA\MotorConexiones\catalogo\"
   Remove-Item "$env:LOCALAPPDATA\MotorConexiones\catalogo-aparte"
   Anota "8c-5 catalogo restaurado" { .\scripts\conn-call.ps1 -Operation catalog_list }
   ```

   Se espera en `batch_plan`: `warnings` con `CATALOG_EMPTY`, `summary_text` "Ningún nudo listo: no hay plantillas…" y
   los nudos `no_match`; en `catalog_list`, otra vez `templates_count: 1`.

### 8c-6. Sondeo 19: etiquetas en el lienzo **(instalador; la persona mira la vista)**

```powershell
Anota "8c-6 sondeo 19 etiquetas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
```

Se espera `2) DB.TemporaryGraphicsManager existe: True` (y lo mismo para `InCanvasControlData` e
`ITemporaryGraphicsHandler`) con la lista de sus miembros, `3) PNG generado`, `5) Control anadido en la vista …: indice 0`,
`6) Captura: …fase8c-03-etiqueta.png`, `7) ITemporaryGraphicsHandler instalado`, `8) Control 0 quitado`, `9) …Clear()
llamado` y `10) Marcadores DirectShape de plan en el modelo … : 0` (o los que haya si quedó un plan marcado). **(la
persona)**: si la etiqueta (un círculo verde con un 4) se ve en el punto del Detalle D mientras corre el sondeo pero no sale
en la captura exportada, haz una captura de pantalla a mano con ese nombre. Si el sondeo dice `PARADA: faltan las API…` o
falla en algún paso, **copia el bloque entero**: con eso se decide la Fase 10 (etiquetas pinchables o marcadores). No bloquea.

### 8c-7. Descartar por el puente, restos **(instalador, ventanas cerradas)**

```powershell
Anota "8c-7 batch_plan_get" { .\scripts\conn-call.ps1 -Operation batch_plan_get }
Anota "8c-7 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
Anota "8c-7 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "8c-7 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "8c-7 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera en `batch_plan_get`: el último plan (el del catálogo vacío o el del botón; si quedó alguno marcado, `is_marked:
true`). En `discard all`: `discarded_plans` = los planes en memoria y **`removed_markers` = los marcadores que había** (0.8.2:
antes decía 0 aunque quitara 36), `remaining_markers: 0`. Sondeo 17 con `Marcadores de plan … : 0` y entero hasta el paso
10. `conexiones en el modelo: 0` (las extensiones de 68,6 / 69,2 mm en 1249630 / 1249631 siguen desde la 7b; si el sondeo
12 tarda más de un minuto, anota qué tenía Revit abierto) y `Elementos de acero sueltos encontrados: 0`.

### 8c-8. Puente y log, anotados en el archivo **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 30
Anota "8c-8 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8c-8 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|startup" | Select-Object -Last 30 }
Select-String -Path $salida -Pattern "^## 8c-8" | Select-Object LineNumber, Line
```

Se espera **`Resultado: 28/28 pruebas correctas`** (las pruebas 22 y 23 dicen `nudo=N4 ready same | ▲ Listo con aviso |
ambar | Se creará 1 conexión con PRUEBA probar_conexiones …` o `● Listo | verde`; la 27 necesita el servidor arrancado: si
falla sola, repite el `Anota` del puente una vez pasados otros 30 s). En el log, `"addin_version":"0.8.3"`, `batch_plan` con
`summary`, `ribbon_batch_window` con las acciones, `ribbon_batch_show_hidden`, `ribbon_batch_catalog_opened`,
`batch_plan_discard_all` con `markers` y `orphans`. **La última línea debe listar dos secciones `## 8c-8`** en el archivo:
si falta alguna, repite su `Anota` antes de seguir.

### 8c-9. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "8c-9 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-8c.md docs\fases\capturas
   git commit -m "Ronda 8c: resultados del instalador (0.8.3, ventana del plan con mapa y colores por estado, catalogo vacio, sondeo 19)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 8c-1 a 8c-3 y 8c-5 a 8c-8; el `git status --short` literal (antes del commit
   y al final, que debe quedar vacío); las anotaciones de la persona (elementos seleccionados, `summary_text`, qué se vio
   en la ventana y en el mapa, qué pasó con el clic derecho, Editar nudo, Abrir catálogo y el sondeo 19, si apareció alguna
   ventana de Revit); las tres capturas; y el texto de cualquier ventana de error. No toques nada de `src\`, `config\`,
   `mcp\` ni `docs\fixtures\`.
