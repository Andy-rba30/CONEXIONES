# Instalación y prueba de la Fase 8: detección de nudos y plan de lote (sin crear nada)

Objetivo: desplegar el add-in **0.8.0** y comprobar en Revit lo que la nube no puede probar: que las marcas del plan
(colores por nudo y marcadores `N1…`) se ven y se quitan (sondeo 17 y `conn_batch_plan`), que la detección de nudos
sobre la cercha real del Hangar da nudos con sentido (estados, cordón, barras, orientación `same` / `mirror_x`), que
las correcciones replanifican con el mismo `plan_id`, que el botón **Planificar lote** y su ventana funcionan (Ver en
Revit, Editar nudo, Descartar) y que `probar_conexiones.py` pasa con las rutas nuevas. **Esta fase no crea ninguna
conexión**: el lote se crea en la Fase 9. Una ronda de unos 45 minutos sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana
de PowerShell. Todos los comandos van en la misma ventana, en orden; si abres otra ventana, repite las tres primeras
líneas del paso 8-1. **Revit lo abre y lo cierra la persona**, no el instalador. Los pasos marcados **(la persona)** se
hacen en Revit; los marcados **(instalador)** van en PowerShell. **Mientras una ventana del add-in esté abierta en Revit,
el instalador no ejecuta nada** (`revit-exec.ps1` y `conn-call.ps1` esperarían y darían tiempo agotado).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 8-2.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la ronda 7b terminó con `conexiones en el modelo: 0`).
- El catálogo del PC con la plantilla oficial `Nudo tipico Detalle D` (`6abcf116-9b97-485f-b50d-2851ca0018cc`), que la
  ronda 7b regeneró. Si `catalog_list` del paso 8-3 no la muestra, `deploy.ps1` la copia desde `catalog\`.
- Para el paso 8-4 la persona tiene que **seleccionar la cercha entera** del Detalle D: los dos cordones (o el cordón que
  tenga) y **todas** las diagonales y montantes de esa cercha, sin columnas ni correas. Cuantas más barras, más nudos. Una
  forma rápida: en una vista 3D, seleccionar con ventana la cercha y, en el filtro de selección, dejar solo *Armazón
  estructural*. Anota cuántos elementos quedan seleccionados.

### 8-1. Pull y archivo de resultados **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-8.md"
"# Resultados de la Fase 8`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "8-1 git" { git log -1 --oneline; git status --short }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista) y un
commit "Fase 8: ..." o posterior.

### 8-2. Compilar, pasar las pruebas, Revit cerrado, desplegar e instalar las rutas nuevas **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
Anota "8-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "8-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "8-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "8-2 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "8-2 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 150`, `8-2 revit cerrado` vacío,
`== MotorConexiones 0.8.0.0 desplegado en Revit 2027 ==` con `Catalogo: ... (plantillas copiadas de catalog\: 0, ya
existentes: 1)` (o `copiadas: 1, ya existentes: 0` si la carpeta estaba vacía), en `instalar-conn`
`copiado revit_mcp\conexiones.py (23 rutas @api.route)` y `copiado tools\conn_tools.py (21 herramientas @mcp.tool)`, y
`0.8.0.0` en la versión de la DLL. Si el build falla o sale `1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 8-3. Abrir Revit, ping, catálogo y sondeo 17 (marcas)

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`. En la pestaña **ARBA**, el
   panel **MotorConexiones** debe tener **cuatro** botones: **Ejecutar especificación JSON**, **Conexiones del modelo**,
   **Catálogo** y **Planificar lote**. Captura: `docs\fases\capturas\fase8-02-cinta.png`. Ponte en una **vista 3D**
   sombreada donde se vea la cercha del Detalle D entera (es la vista en la que se pondrán los colores).
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "8-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "8-3 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "8-3 sondeo 17 marcas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `ping`: `addin_version: 0.8.0` (en `data` y en `meta`) y 21 operaciones, con `batch_plan`,
   `batch_plan_get` y `batch_plan_discard` entre ellas. En `catalog_list`, `templates_count: 1` con
   `Nudo tipico Detalle D` y su `template_id` (**cópialo**: es `$tid` del paso 8-4). En el sondeo 17: la vista activa con
   `admite overrides=True`, `Marcadores de plan ... : 0`, el `Patron solido` encontrado, el override leído con el color
   `(230, 25, 75)` y grosor 10, el marcador creado con caja `160 x 160 x 160`, la captura
   `docs\fases\capturas\fase8-01-sondeo17.png` (una barra roja y un cubo rojo en su punto medio), `Tras limpiar: color
   valido=False | marcador existe=False` y `TransactionGroup deshecho`. Si cualquier línea dice `fallo` o hay un
   traceback, copia el bloque entero: es la prueba de las API de marcas.

### 8-4. Planificar la cercha desde `conn-call` **(la persona selecciona, el instalador llama)**

1. **(la persona)** Selecciona la cercha entera (ver "Antes de empezar") y **deja la selección puesta**. Anota el número de
   elementos seleccionados (abajo a la derecha en Revit). No abras ninguna ventana del add-in.
2. **(instalador)** Sustituye `<template_id>` por el del paso 8-3:

   ```powershell
   $tid = "<template_id>"
   Anota "8-4 batch_plan" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, un `plan_id` (**cópialo**: es `$pid`), `summary` con cuentas por estado (por ejemplo
   `ready: 6, no_match: 4, untyped: 10`), `is_marked: true`, `marks.element_count` > 0 y `nodes[]` con un nudo por
   punto de la cercha: `name` (`N1`, `N2`…), `status`, `chord_element_id`, `member_element_ids`, `members[]` con
   `angle_deg` y `side`, `template_name`, `orientation` (`same` en una mitad, `mirror_x` en la otra), `max_deviation_deg`,
   `warnings`/`errors` y `validation_token` (64 caracteres) en los `ready`. Copia el bloque entero aunque sea largo.
   El nudo del Detalle D (cordón 1249510 con 1249630, 1249631 y 1249636) debe salir `ready`, `same`, desvío ~0.
3. **(la persona)** Mira la vista 3D: cada nudo planificado tiene sus barras de un color y un marcador con su nombre en el
   punto de trabajo (cubo = `same`, rombo = en espejo; el nombre se ve al pasar el ratón o en Propiedades > Marca). Los
   nudos `untyped` (extremos sueltos) y `already_connected` no llevan color. Captura de la cercha entera con los colores:
   `docs\fases\capturas\fase8-03-marcas.png`, y un zoom a un marcador en espejo: `fase8-04-marcador-espejo.png`. Anota
   en el chat si los colores agrupan bien las barras de cada nudo y si algún nudo salió `no_match`, `offset` o
   `ambiguous_chord` que no debería.

### 8-5. Corregir y replanificar con el mismo plan **(instalador)**

Elige en la salida de 8-4 un nudo `ready` distinto del Detalle D (llámalo `<N>`, por ejemplo `N3`) y, si hay alguno
`ambiguous_chord`, también ese (`<NA>`, con el `chord_element_id` que quieras entre `through_element_ids`). Sustituye:

```powershell
$pid = "<plan_id>"
Anota "8-5 batch_plan_get N" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $pid + '","node":"<N>"}') }
Anota "8-5 replan excluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $pid + '","overrides":{"exclude":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
Anota "8-5 replan incluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $pid + '","overrides":{"include":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
```

Se espera en `batch_plan_get`: `data.node` con la especificación completa (`spec`, con `source.template_id` y
`source.batch_id` = `plan_id`) y el mismo token que en 8-4. En el primer replan: el **mismo** `plan_id`, los mismos nombres,
`<N>` con `status: excluded` y sin color en el modelo (la persona lo mira), y `overrides.exclude: ["<N>"]`. En el segundo:
`<N>` otra vez `ready` con el **mismo token** que en 8-4 (misma especificación, mismo modelo). Si hubo un nudo
`ambiguous_chord`:

```powershell
Anota "8-5 replan cordon" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $pid + '","overrides":{"chord":{"<NA>":<id del cordon>}},"include_specs":false}') -TimeoutSec 600 }
```

Se espera `<NA>` ya no `ambiguous_chord` (pasa a `ready`, `invalid` o `no_match` según sus barras). Deja el plan puesto para
el paso siguiente.

### 8-6. Botón Planificar lote **(la persona)**

1. Con la cercha todavía seleccionada (o vuelve a seleccionarla), ARBA > **Planificar lote**. Se abre la ventana del plan
   con la tabla de nudos (color, estado, orientación, cordón, barras, plantilla, desvío, avisos, errores, token) y arriba el
   resumen. Como ya había un plan, este es uno nuevo (las marcas anteriores se quitan y se ponen las nuevas). Captura:
   `docs\fases\capturas\fase8-05-ventana-plan.png`.
2. Elige un nudo `ready` y pulsa **Ver en Revit**: la ventana se cierra, Revit selecciona sus barras y su marcador y hace
   zoom; sale un diálogo con el resumen del nudo; al cerrarlo la ventana del plan vuelve a abrirse con ese nudo marcado.
3. Con un nudo `ready` elegido, **Editar nudo**: se abre la ventana de previsualización de siempre con la especificación
   de ese nudo (cabecera `Nudo N… del plan`). Cambia una cota (por ejemplo el alto de la cartela de 530 a 500 con doble
   clic en la cota) y cierra con **Cancelar** o con **Crear** (crear aquí no crea nada): al volver, el nudo dice
   `ready (editado)` o `invalid (editado)` según valide, y **Quitar edición** lo devuelve a la plantilla. Captura de la
   tabla con el nudo editado: `docs\fases\capturas\fase8-06-nudo-editado.png`.
4. **Cordón…** sobre un nudo: debe listar sus barras con el tipo y ofrecer **Pinchar en Revit…** (prueba pincharlo: la
   ventana se cierra, Revit pide la barra, y la ventana vuelve replanificada). **Barras…** igual. **Añadir nudo…**: pincha
   el cordón y las barras de un nudo que el plan no tenga (por ejemplo uno `no_match` de dos barras, añadiendo la que
   falta) y pulsa Finalizar: aparece `N<siguiente>`.
5. **Guardar plan JSON**: abajo dice la ruta en `Documentos\MotorConexiones\plan-<8 letras>.json`.
6. **Descartar plan** > Sí: la ventana se cierra, los colores vuelven a la normalidad y los marcadores desaparecen.
   Comprueba en la vista 3D que no queda ningún cubo ni rombo. Anota en el chat lo que no haya ido como se describe.

### 8-7. Descartar por el puente, restos y pruebas **(instalador, ventanas cerradas)**

```powershell
Anota "8-7 batch_plan_get tras descartar" { .\scripts\conn-call.ps1 -Operation batch_plan_get }
Anota "8-7 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
Anota "8-7 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "8-7 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "8-7 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 20
Anota "8-7 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8-7 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|startup" | Select-Object -Last 30 }
```

Se espera en `batch_plan_get`: `ok: false` con `PLAN_NOT_FOUND` (el botón Descartar olvidó el plan de la cinta; si el
de 8-4 seguía en memoria, devuelve ese: entonces el `discard all` lo quita). En `batch_plan_discard all`:
`remaining_markers: 0`. En el sondeo 17, `Marcadores de plan ... : 0` (y el resto como en 8-3). `conexiones en el
modelo: 0`, `Elementos de acero sueltos encontrados: 0`. `Resultado: 28/28 pruebas correctas` (las 22 a 24 planifican
el nudo del fixture con `mark: false`, sin tocar la vista; la 27 lista las 21 herramientas). En el log,
`"event":"startup","addin_version":"0.8.0"`, `batch_plan` con `summary`, `ribbon_batch_window` con las acciones y
`batch_plan_discard`.

### 8-8. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "8-8 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-8.md docs\fases\capturas
   git commit -m "Fase 8: resultados del instalador (plan de lote sobre la cercha del Hangar, marcas y ventana del plan)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 8-1 a 8-5 y 8-7; el `git status --short` literal (antes del commit y al
   final, que debe quedar vacío); las anotaciones de la persona (número de elementos seleccionados, si los colores
   agrupan bien, qué nudos salieron con qué estado, qué pasó en cada botón del paso 8-6); las seis capturas; y el texto
   de cualquier ventana de error del add-in o de Revit. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
