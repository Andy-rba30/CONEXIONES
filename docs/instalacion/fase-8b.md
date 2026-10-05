# Instalación y prueba de la ronda 8b: add-in 0.8.1, la misma cercha con el corte de los ejes

Objetivo: repetir la planificación de la Fase 8 sobre **la misma cercha del Hangar** con el add-in **0.8.1**, que corrige
lo que el PC enseñó el 2026-10-04 (`docs/fases/resultados-fase-8.md`): las diagonales de la cercha real terminan en la
**cara** del cordón y no en su eje (a 15–85 mm), así que la agrupación de extremos a 10 mm no las juntaba y los 53 elementos
dieron 97 nudos sin ninguno `ready`. Ahora cada extremo se lleva al corte de su eje con el eje del cordón (hasta medio
canto de cada barra más 10 mm) y, en la nube, la cercha reconstruida de aquellos resultados da **53 nudos con 10 `ready`**
(5 `same` y 5 `mirror_x`, el Detalle D entre ellos). También se corrigen el sondeo 17 (`AttributeError: Name`), la variable
`$pid` del paso 8-5 (PowerShell la reserva: por eso salió `PLAN_NOT_FOUND` con el `plan_id` `35920`) y el aviso de Revit
"Elements have duplicate Mark values" (los marcadores ya no escriben `Marca`). Unos 40 minutos sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta ronda no crea ninguna conexión.**

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana
de PowerShell. Todos los comandos van en la misma ventana, en orden; si abres otra ventana, repite las tres primeras
líneas del paso 8b-1. **Revit lo abre y lo cierra la persona**, no el instalador. Los pasos marcados **(la persona)** se
hacen en Revit; los marcados **(instalador)** van en PowerShell. **Mientras una ventana del add-in esté abierta en Revit,
el instalador no ejecuta nada.** La variable del plan se llama **`$plan`** (no `$pid`).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 8b-2.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la ronda 8 terminó con
  `remaining_markers: 0`, `conexiones en el modelo: 0` y la copia se cerró sin guardar).
- Para el paso 8b-4 la persona selecciona **la misma cercha** que en la Fase 8 (los 53 elementos: el cordón central en
  sus cinco tramos y las diagonales). Si puede, **añade también los cordones superior e inferior** de esa cercha y los
  tramos del cordón central que faltaron entre las diagonales de la mitad derecha (en la Fase 8 no había cordón entre
  X ≈ 16 y 47 m): cuantos más cordones, más nudos completos. Anota cuántos elementos quedan seleccionados.

### 8b-1. Pull y archivo de resultados **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-8b.md"
"# Resultados de la ronda 8b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "8b-1 git" { git log -1 --oneline; git status --short }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista) y un
commit "Ronda 8b: ..." o posterior.

### 8b-2. Compilar, pasar las pruebas, Revit cerrado, desplegar e instalar **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
Anota "8b-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "8b-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "8b-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "8b-2 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "8b-2 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 158`, `8b-2 revit cerrado` vacío,
`== MotorConexiones 0.8.1.0 desplegado en Revit 2027 ==` (con `config\catalog.json` copiado: ahora trae
`node_face_reach_mm: 0`), `copiado revit_mcp\conexiones.py (23 rutas @api.route)`, `copiado tools\conn_tools.py
(21 herramientas @mcp.tool)` y `0.8.1.0` en la versión de la DLL. Si el build falla o sale `1 Advertencia(s)`, **para
aquí** y devuelve la salida.

### 8b-3. Abrir Revit, ping, sondeo 17 (marcas) y sondeo 18 (extremos en la cara)

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de la Fase 8 donde se ve la cercha del Detalle D entera. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "8b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "8b-3 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "8b-3 sondeo 17 marcas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "8b-3 sondeo 18 extremos Detalle D" { .\scripts\revit-exec.ps1 -File scripts\sondeos\18-extremos-cara.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `ping`: `addin_version: 0.8.1` (en `data` y en `meta`) y 21 operaciones. En `catalog_list`,
   `templates_count: 1` con `Nudo tipico Detalle D` y `template_id` `6abcf116-9b97-485f-b50d-2851ca0018cc` (es `$tid`).
   En el **sondeo 17** (en la Fase 8 moría en la línea 3 con `AttributeError: Name`): ahora pasa entero: `3) Barra de
   prueba: [1249510] HSS3X3X1/4 ...`, `4) Patron solido` encontrado, `5) Override puesto ... color leido (230, 25, 75) |
   grosor 10`, `6b) Modelos genericos con Marca N<numero> en el documento (deberian ser 0): 0`, `7) Marcador creado:
   [...] nombre=N1 (Name escrito=True) | Comentarios=N1 · MotorConexiones sondeo 17; view=... | caja mm 160 x 160 x 160`,
   la captura `docs\fases\capturas\fase8b-01-sondeo17.png` (una barra roja y un cubo rojo en su punto medio),
   `9) Tras limpiar: color valido=False | marcador existe=False` y `10) TransactionGroup deshecho`. Si vuelve a salir un
   traceback, copia el bloque entero con el número de línea.
   En el **sondeo 18** (solo lee; sin selección usa las cuatro barras del Detalle D): `2)` una línea por barra con el tipo,
   `b` y `h` (76,2 para el HSS3X3, 63,5 para los HSS 2-1/2; si salen `0.0` y "sin medidas", dilo: es el dato que decide el
   alcance) y sus extremos en mm; `3)` para cada extremo de las diagonales 1249630, 1249631 y 1249636, una línea como
   `corta el eje de [1249510] HSS3X3X1/4 (atraviesa) | extremo a 58.8 mm de ese eje (alcance 79.9) | se queda a 84.5 mm
   del corte | corte (-11868.x, -17195.8, 17423.0)` (los tres cortes deben estar a menos de 4 mm entre sí; las distancias
   esperadas son unos 59, 14 y 33 mm del eje y 85, 20 y 48 mm del corte); los extremos lejanos y los del cordón, `sin eje
   vecino a alcance`. **Copia el bloque entero**: es la prueba de la hipótesis de la ronda 8b sobre el modelo real.

### 8b-4. Planificar la misma cercha **(la persona selecciona, el instalador llama)**

1. **(la persona)** Selecciona la cercha (ver "Antes de empezar") y **deja la selección puesta**. Anota el número de
   elementos. No abras ninguna ventana del add-in.
2. **(instalador)**:

   ```powershell
   $tid = "6abcf116-9b97-485f-b50d-2851ca0018cc"
   Anota "8b-4 sondeo 18 seleccion" { .\scripts\revit-exec.ps1 -File scripts\sondeos\18-extremos-cara.py -SinTransaccion -TimeoutSec 300 }
   Anota "8b-4 batch_plan" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, un `plan_id` (**cópialo**: es `$plan`), `is_marked: true` y, **con los mismos 53 elementos de la
   Fase 8**, `summary` con `ready: 10, no_match: 26, untyped: 17` (en la nube, con la cercha reconstruida de aquellos
   resultados, sale exactamente eso; con más cordones seleccionados habrá más `ready` y menos `no_match`). El nudo del
   Detalle D (cordón 1249510 con 1249630, 1249631 y 1249636; en la nube se llama **N4**) debe ser `ready`, `same`,
   `max_deviation_deg` ~0, `chord_continuous: true`, token de 64 caracteres y en `members[]` un `end_gap_mm` de unos
   84,5 / 19,6 / 48,1 mm (cuánto se queda corta cada diagonal). El nudo siguiente del mismo cordón (1249632 / 1249633 /
   1249637; en la nube **N7**) debe ser `ready`, `mirror_x`. Los nudos `no_match` donde no se seleccionó el cordón llevan
   el aviso `NODE_CHORD_NOT_CONTINUOUS` ("Ninguna barra atraviesa el nudo..."). Copia el bloque entero aunque sea largo.
   Si Revit muestra alguna ventana (por ejemplo "Elements have duplicate Mark values"), copia su texto: no debería.
3. **(la persona)** Mira la vista 3D: ahora cada nudo del cordón central tiene sus **cuatro** barras del mismo color
   (cordón grueso) y un marcador en el punto de trabajo, sobre el eje del cordón: cubo en los `same` y rombo en los
   `mirror_x`. Pasa el ratón por un marcador: Propiedades > `Comentarios` empieza por su nombre (`N4 · MotorConexiones
   plan ...`); `Marca` está vacía. Capturas: la cercha entera `docs\fases\capturas\fase8b-02-marcas.png` y un zoom al nudo
   del Detalle D con su marcador `fase8b-03-detalle-d.png`. Anota en el chat si algún color agrupa barras de dos nudos
   distintos (nombres) o si algún nudo de tres diagonales con cordón salió `no_match` o `invalid`.

### 8b-5. Corregir y replanificar con el mismo plan **(instalador)**

Con `<N>` = el nombre del nudo del Detalle D en la salida de 8b-4 (N4 si la selección es la misma):

```powershell
$plan = "<plan_id>"
Anota "8b-5 batch_plan_get N" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan + '","node":"<N>"}') }
Anota "8b-5 replan excluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"exclude":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
Anota "8b-5 replan incluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"include":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
```

Se espera en `batch_plan_get`: `data.node` con la especificación completa (`spec` con `source.template_id` =
`6abcf116…` y `source.batch_id` = `plan_id`, `members[]` en el orden de las ranuras con la placa cuchilla en 1249636) y
el mismo token que en 8b-4. En el primer replan: el **mismo** `plan_id`, los mismos nombres, `<N>` con `status: excluded`
y sin color en el modelo (la persona lo mira). En el segundo: `<N>` otra vez `ready` con el **mismo token** que en 8b-4.
Si en 8b-4 hubo un nudo de tres diagonales **sin** cordón (`no_match` con el aviso `NODE_CHORD_NOT_CONTINUOUS`) y el
cordón existe en el modelo pero no estaba seleccionado, prueba a dárselo (`<NC>` = su nombre, `<id>` = el ID del tramo de
cordón, que la persona lee en Revit al seleccionarlo):

```powershell
Anota "8b-5 replan cordon" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"chord":{"<NC>":<id>}},"include_specs":false}') -TimeoutSec 600 }
```

Se espera `<NC>` con `chord_element_id: <id>`, `chord_continuous: true` y `ready` (o `invalid` con sus errores). Deja el
plan puesto para el paso siguiente.

### 8b-6. Botón Planificar lote **(la persona)**

1. Con la cercha todavía seleccionada (o vuelve a seleccionarla), ARBA > **Planificar lote**. La ventana muestra la tabla
   con los nudos `ready` en verde y su resumen (`10 nudo(s) listos con token...` con los 53 elementos). Captura:
   `docs\fases\capturas\fase8b-04-ventana-plan.png`.
2. Elige el nudo del Detalle D y pulsa **Ver en Revit**: la ventana se cierra, Revit selecciona sus cuatro barras y el
   marcador y hace zoom; al cerrar el diálogo del resumen la ventana vuelve con ese nudo marcado.
3. **Editar nudo** (en la Fase 8 estaba en gris en todos los nudos: solo se activa con nudos que tienen especificación,
   `ready` o `invalid`; pasa el ratón por el botón con un nudo `no_match` elegido y verás el motivo en el globo). Con el
   nudo del Detalle D elegido, púlsalo: se abre la previsualización de siempre con la cabecera `Nudo N… del plan`. Cambia
   una cota (el alto de la cartela de 530 a 500 con doble clic) y cierra con **Cancelar** o con **Crear** (crear aquí no
   crea nada): el nudo pasa a `ready (editado)` o `invalid (editado)` según valide, y **Quitar edición** lo devuelve a la
   plantilla. Captura de la tabla con el nudo editado: `docs\fases\capturas\fase8b-05-nudo-editado.png`.
4. **Añadir nudo…** sobre un trío de diagonales `no_match` sin cordón: pincha en Revit sus tres diagonales **y el tramo de
   cordón** que las atraviesa y pulsa Finalizar: aparece `N<siguiente>` con `chord_continuous` y, si casa, `ready`.
   **Cordón…** y **Barras…** como en la Fase 8 (listas con **Pinchar en Revit…**).
5. **Guardar plan JSON** (abajo dice la ruta en `Documentos\MotorConexiones\plan-<8 letras>.json`) y **Descartar plan** >
   Sí: colores a la normalidad y marcadores fuera. Comprueba que no queda ningún cubo ni rombo y que Revit no mostró
   ningún aviso de `Marca` duplicada. Anota en el chat lo que no haya ido como se describe.

### 8b-7. Descartar por el puente, restos y pruebas **(instalador, ventanas cerradas)**

```powershell
Anota "8b-7 batch_plan_get tras descartar" { .\scripts\conn-call.ps1 -Operation batch_plan_get }
Anota "8b-7 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
Anota "8b-7 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "8b-7 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "8b-7 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 20
Anota "8b-7 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8b-7 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|startup" | Select-Object -Last 30 }
```

Se espera en `batch_plan_get`: `PLAN_NOT_FOUND` (el botón Descartar olvidó el plan; si el de 8b-4 seguía en memoria,
devuelve ese y el `discard all` lo quita). `remaining_markers: 0`. Sondeo 17 con `Marcadores de plan ... : 0` y el resto
como en 8b-3. `conexiones en el modelo: 0`, `Elementos de acero sueltos encontrados: 0`. **`Resultado: 28/28 pruebas
correctas`**: en la Fase 8 fueron 26/28 porque las pruebas 22 y 23 planifican el nudo del fixture (las cuatro barras
del Detalle D) y también salió `untyped`; ahora deben dar `N1 ready con token`. En el log, `"addin_version":"0.8.1"`,
`batch_plan` con `summary` y `ready`, `ribbon_batch_window` con las acciones y `batch_plan_discard`.

### 8b-8. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "8b-8 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-8b.md docs\fases\capturas
   git commit -m "Ronda 8b: resultados del instalador (0.8.1 sobre la cercha del Hangar, corte de los ejes, sondeos 17 y 18)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 8b-1 a 8b-5 y 8b-7; el `git status --short` literal (antes del commit y al
   final, que debe quedar vacío); las anotaciones de la persona (elementos seleccionados, si los colores agrupan bien, qué
   nudos salieron con qué estado, qué pasó con **Editar nudo** y **Añadir nudo**, si apareció alguna ventana de Revit); las
   cinco capturas; y el texto de cualquier ventana de error. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
