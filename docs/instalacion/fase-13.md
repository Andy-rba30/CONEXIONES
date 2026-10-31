# Instalación y prueba de la Fase 13: add-in 0.13.0, el filtro de la 12d, el sondeo 30, la cartela automática y partir la diagonal

> **Ejecutada el 2026-10-31** (`docs/fases/resultados-fase-13.md`; cierre en `docs/fases/fase-13.md`, sección 7). Erratas corregidas
> después de ejecutarla, para que el archivo quede bien si se repite: los sondeos 12 y 13 se llaman `12-fase3-borrar.py` y
> `13-limpiar-fase1.py` (el instalador recibió "no existe el archivo"); en el paso 13-9 la variable del plan es `$plan` (`$pid` es de
> solo lectura en PowerShell); la cercha de la 8c parte **31** diagonales, no 30 (`Fase13PlanTests`); el log vive en
> `%LOCALAPPDATA%\MotorConexiones\log\motorconexiones-AAAAMMDD.jsonl`; el sondeo 30 de la 0.13.0 paraba en `barra()` (corregido en
> la v2, se repite en `docs/instalacion/fase-13b.md`).

Objetivo: desplegar el add-in **0.13.0** (`docs/fases/fase-13.md`) y comprobar sobre una **copia** del modelo, en este orden: (1) la
comprobación pendiente de la 12d (`fase-12.md` 10.3: el nudo en K sin la casilla da solo `END_CUT_APPROXIMATED`); (2) el **sondeo
30** (cómo se parte una barra en Revit; decide si `MemberSplitter` es el bueno o la reserva; no deja nada en el modelo); (3) la
**cartela automática** en Conectar, Contorno y Convertir en polígono; (4) una plantilla en modo `auto`; (5) **la cadena** de Conectar
en el nudo en K (partir, empalme B y nudo en K de un clic); (6) `conn_split_member` y `conn_unsplit` por el adaptador; (7) el plan con
los pasos de corte (sin crear el lote); (8) el puente. Unos **75 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. **Esta fase crea acero de verdad en la copia** (cuatro conexiones sueltas y dos cortes de barra) y lo borra al
final.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell. Todos los
comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 13-1. **Revit lo abre y lo cierra la
persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en PowerShell. Mientras una ventana
del add-in está abierta, el instalador no ejecuta nada. **Si Revit se cerrara** en algún paso: anota en qué botón o clic, captura del
cuadro, vuelve a abrir Revit y la copia y sigue con el siguiente; el log del paso 13-10 dirá dónde se quedó. **Si el sondeo dice FALLO
o PARADA, no es un fallo de la instalación**: es lo que viene a averiguar; copia la salida entera y sigue. Lo mismo si un corte o una
conexión sale con una reserva (`SPLIT_PARAMETERS_APPROXIMATED`, `SPLIT_UNDO_PARTIAL`, `END_CUT_APPROXIMATED`, `CATEGORY_FALLBACK`):
anótalo tal cual.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 13-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in, sin marcadores ni etiquetas y **sin barras partidas** (la 12d terminó
  con todo a cero). Los sondeos 12 y 13 del paso 13-2 lo dicen y limpian. **No** hay plan marcado.
- Las plantillas del catálogo en su sitio (`Nudo tipico Detalle D` y las guardadas en la Fase 12: `Nudo K cordon superior` y el
  empalme C, si siguen).
- Localizar en la cercha de la 8c: (a) el **nudo en K** del cordón superior del paso 11-5: cordón **1245533** y diagonales **1249631**
  y **1249632** (si los IDs no coinciden, apunta los reales en Propiedades); (b) **dos nudos del cordón central con cartela** que no
  sean el Detalle D (por ejemplo los de 1249510 con sus diagonales, vecinos del Detalle D); (c) las **64 barras** de la cercha.
- Los fixtures `docs\fixtures\detalle-D-confirmado.json` y `docs\fixtures\nudo-K-cordon-superior.json` del repositorio (paso 13-10).

### 13-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-13.md"
"# Resultados de la Fase 13`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "13-1 git" { git log -1 --oneline; git status --short }
Anota "13-1 build" { dotnet build MotorConexiones.sln -c Release --nologo }
Anota "13-1 test" { dotnet test src\MotorConexiones.Tests -c Release --no-build --nologo }
Anota "13-1 deploy" { .\scripts\deploy.ps1 }
Anota "13-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "13-1 config" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\choices.json" | Select-String "outline_mode|gusset_side_margin"; Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json" | Select-String "max_auto_gusset_mm|auto_outline_max_vertices"; Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json" | Select-String "auto_angle_tolerance_deg" }
```

Se espera: `Build succeeded` sin avisos, `Passed! … 311`, `MotorConexiones 0.13.0.0 desplegado en Revit 2027`, `instalar-conn`
sin cambios en `startup.py` si ya estaba registrado, y las cinco claves nuevas en los tres `config\*.json`.

### 13-2. Abrir Revit, ping, lista vacía y sondeos de limpieza **(la persona abre; instalador)**

**(la persona)** Abre Revit 2027 y la copia `HANGAR_PRUEBA_sondeo.rvt`; espera a que pyRevit cargue (ventana de pyRevit Routes en el
log) y avisa.

```powershell
Anota "13-2 ping" { .\scripts\conn-call.ps1 ping }
Anota "13-2 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13-2 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13-2 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "13-2 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera: `addin_version 0.13.0`, `operations` con `split` y `unsplit` (27 nombres), `list` con 0 conexiones, `unsplit` con
`splits_count: 0`, el sondeo 12 con `1249631: inicio 0.0 | fin 69.2` (la diagonal entera) y el 13 sin restos.

### 13-3. La comprobación pendiente de la 12d: el nudo en K sin la casilla **(la persona; instalador)**

**(la persona)** Selecciona el cordón 1245533 y las diagonales 1249631 y 1249632 y pulsa **Conectar**. En la previsualización
(arranque `direct_weld` o la plantilla `Nudo K cordon superior`), **desmarca** la casilla "Partir las 2 diagonales y crear sus
empalmes del Detalle B a 330" (abajo a la izquierda, junto al token) y pulsa **Crear**. Anota el cuadro **literal**: se espera
"Conexión modelada correctamente", 2 elementos (las dos soldaduras), 2 retiros y **solo** `END_CUT_APPROXIMATED` en las advertencias
(**sin `REVIT_WARNING`**). Cierra el cuadro y avisa.

```powershell
Anota "13-3 list" { .\scripts\conn-call.ps1 list '{}' }
$cid = (.\scripts\conn-call.ps1 list '{}' | ConvertFrom-Json).data.connections[0].connection_id
Anota "13-3 delete" { .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $cid + '"}') }
Anota "13-3 log revit_failure" { Get-Content "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "revit_failure" | Select-Object -Last 4 }
```

Se espera: 1 conexión, borrada, y en el log **dos** líneas `revit_failure` con `"filtered": true` (la advertencia "only visible in
Detail Level: Fine", una por soldadura). Es la primera vez que el filtro de la 0.12.4 se ve en Revit.

### 13-4. Sondeo 30: cómo se parte una barra **(instalador; la persona mira)**

```powershell
Anota "13-4 sondeo 30" { .\scripts\revit-exec.ps1 -File scripts\sondeos\30-partir-barra.py -SinTransaccion -TimeoutSec 600 }
Anota "13-4 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "13-4 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Devuelve la salida **entera** del sondeo 30: las líneas de los miembros de la API (`existe` / `NO existe`), los parámetros del
original ANTES, el punto de corte (θ, cara, primera plancha, corte y el punto en mm), el resultado de (a) con la lista de parámetros
de la copia distintos del original, `DisallowJoinAtEnd`, las longitudes de los dos tramos, (b) si (a) falló, `node_info` de los dos
tramos (`kind splice`) y del nudo en K (misma excentricidad), el deshacer (longitud y extensiones iguales a las de antes) y los
elementos nuevos antes y después de cancelar (se esperan `[]`). El sondeo 12 debe seguir diciendo `1249631: inicio 0.0 | fin 69.2`.
**Si quieres ver los tramos**: `New-Item $env:TEMP\sondeo30-commit.txt`, repite el sondeo, captura `fase13-00-sondeo30.png`, deshaz
con Ctrl+Z en Revit y borra la marca (`Remove-Item $env:TEMP\sondeo30-commit.txt`).

### 13-5. La cartela automática en Conectar **(la persona; instalador)**

**(la persona)** Selecciona el cordón central y las barras de **un nudo con cartela que no sea el Detalle D** (b) y pulsa **Conectar**.
Si propone una plantilla (`Nudo tipico Detalle D`), pulsa **Cambiar…** y elige **Tipo `gusset_node` sin plantilla** (el arranque):
la cabecera dice "Especificación de arranque con cartela automática" y el croquis enseña la cartela **calculada** (con chaflanes
donde la diagonal lo pide; la etiqueta "cartela … · automática: N vértices calculados"). En la sección **Cartela** del panel mira el
desplegable **Contorno: Automático**. Captura **`fase13-01-auto.png`**. Cambia Contorno a **Polígono** (o pulsa **Convertir en
polígono** en Avanzado): el cuadro del contorno pasa a editable con los puntos calculados; haz doble clic en la cota del ancho y pon
otro valor (por ejemplo 650) → la cartela se estira. Captura **`fase13-02-poligono.png`**. Vuelve a Contorno **Automático**
(los puntos desaparecen y la cartela se recalcula). Pulsa **Validar** y, en verde, **Crear**. Anota el cuadro (elementos creados y
avisos) y avisa.

```powershell
Anota "13-5 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13-5 get" { $c = (.\scripts\conn-call.ps1 list '{}' | ConvertFrom-Json).data.connections[0]; .\scripts\conn-call.ps1 get ('{"connection_id": "' + $c.connection_id + '"}') }
```

Se espera: 1 conexión con `spec.gusset.outline.mode: "auto"` **sin `points_mm`**. **No la borres todavía** (13-6 la guarda como plantilla).

### 13-6. Una plantilla en modo `auto` **(la persona; instalador)**

**(la persona)** Con la conexión de 13-5 creada, abre **Catálogo** > **Guardar desde conexión** (o repite Conectar sobre ese nudo y
pulsa **Guardar en catálogo**): nombre `Cartela automatica cordon central`. Después selecciona el **otro** nudo del cordón central
(b) y pulsa **Conectar**: debe proponer `Cartela automatica cordon central` (o listarla en Cambiar…) y la cartela debe salir
recalculada para ese nudo (Contorno: Automático). **Cancelar**. Avisa.

```powershell
Anota "13-6 catalog_list" { .\scripts\conn-call.ps1 catalog_list '{}' }
$tid = ((.\scripts\conn-call.ps1 catalog_list '{}' | ConvertFrom-Json).data.templates | Where-Object { $_.name -like "Cartela automatica*" }).template_id
Anota "13-6 catalog_get" { .\scripts\conn-call.ps1 catalog_get ('{"template_id": "' + $tid + '"}') }
$cid = (.\scripts\conn-call.ps1 list '{}' | ConvertFrom-Json).data.connections[0].connection_id
Anota "13-6 delete" { .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $cid + '"}') }
Anota "13-6 catalog_delete" { .\scripts\conn-call.ps1 catalog_delete ('{"template_id": "' + $tid + '"}') }
```

Se espera: la plantilla con `spec_template.gusset.outline.mode: "auto"` y `matching.angle_tolerance_deg: 25`; la conexión borrada
(3 barras restauradas); la plantilla borrada.

### 13-7. La cadena de Conectar en el nudo en K **(la persona; instalador)**

**(la persona)** Selecciona 1245533, 1249631 y 1249632 y pulsa **Conectar**. Deja la casilla **marcada** ("Partir las 2 diagonales y
crear sus empalmes del Detalle B a 330") y pulsa **Crear**. Anota el cuadro **literal**: se espera "Diagonal partida, empalme B y
nudo en K modelados", dos líneas "Corte: la barra 12496xx … tramo nuevo N a ≈ 485 mm", dos "Empalme B: conexión …" y "Nudo en K:
conexión …". Captura **`fase13-03-cadena.png`** del tramo corto de una diagonal con las dos planchas del Detalle B y la soldadura
en la cara del cordón. Si el cuadro dice que falló (y en qué paso), anótalo tal cual: se deshizo todo. Avisa.

```powershell
Anota "13-7 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13-7 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
$cortes = (.\scripts\conn-call.ps1 unsplit '{}' | ConvertFrom-Json).data.splits
Anota "13-7 node_info tramos" { .\scripts\conn-call.ps1 node_info ('{"element_ids": [' + $cortes[0].element_id + ', ' + $cortes[0].new_element_id + ']}') }
Anota "13-7 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
$conns = (.\scripts\conn-call.ps1 list '{}' | ConvertFrom-Json).data.connections
Anota "13-7 delete nudo K primero (se espera aviso SPLIT_IN_USE)" { $k = $conns | Where-Object { $_.connection_type -eq "direct_weld" }; .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $k.connection_id + '"}') }
Anota "13-7 delete empalmes B" { foreach ($c in ($conns | Where-Object { $_.connection_type -eq "end_plate" })) { .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $c.connection_id + '"}') } }
Anota "13-7 unsplit restantes" { foreach ($s in ((.\scripts\conn-call.ps1 unsplit '{}' | ConvertFrom-Json).data.splits)) { .\scripts\conn-call.ps1 unsplit ('{"split_id": "' + $s.split_id + '"}') } }
Anota "13-7 sondeo 12 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `list` con **3** conexiones (2 `end_plate`, 1 `direct_weld`), `unsplit` con 2 cortes (`connection_id` = la del nudo en K),
`node_info` de los dos tramos `kind: splice` con `existing_connections` (el empalme B), el sondeo 12 con la diagonal **partida**
(1249631 más corta y el tramo nuevo); al borrar el nudo en K **antes** que los empalmes, aviso `SPLIT_IN_USE` (el corte se queda);
tras borrar los empalmes, `unsplit` deshace los dos cortes (`complete: true`) y el sondeo 12 vuelve a `1249631: inicio 0.0 | fin 69.2`.

### 13-8. `conn_split_member` y `conn_unsplit` sueltos **(instalador)**

```powershell
Anota "13-8 split dry_run" { .\scripts\conn-call.ps1 split '{"element_id": 1249632, "chord_element_id": 1245533, "dry_run": true}' }
Anota "13-8 split" { .\scripts\conn-call.ps1 split '{"element_id": 1249632, "chord_element_id": 1245533}' }
$s = (.\scripts\conn-call.ps1 unsplit '{}' | ConvertFrom-Json).data.splits[0]
Anota "13-8 node_info tramos" { .\scripts\conn-call.ps1 node_info ('{"element_ids": [1249632, ' + $s.new_element_id + ']}') }
Anota "13-8 node_info nudo K" { .\scripts\conn-call.ps1 node_info '{"element_ids": [1245533, 1249631, 1249632]}' }
Anota "13-8 unsplit" { .\scripts\conn-call.ps1 unsplit ('{"split_id": "' + $s.split_id + '"}') }
Anota "13-8 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "13-8 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera: `dry_run` con `preview.split_distance_mm` ≈ 485 y `theta_deg` ≈ 45; el corte real con `new_element_id` y `mechanism`
(`copy_element_location_curve` o `new_family_instance`; `approximated_parameters` vacío o la lista); `node_info` de los tramos
`kind: splice`; el del nudo en K con `eccentricity_mm` 54,9 y `split_point_mm` por diagonal; `unsplit` `complete: true`; los
sondeos 12 y 13 a cero.

### 13-9. Planificar lote con los pasos de corte **(la persona; instalador)**

**(la persona)** Pincha una barra de la cercha de la 8c y pulsa **Planificar lote** (Planificar con todas). En la tabla, elige un
nudo en K del cordón superior: en su detalle salen las líneas "· Partir 12496xx a 330 (Detalle B): corte a … → empalme NxB" y debajo
del nudo las filas **NxB** "● Empalme B (tras partir)"; la cabecera dice "… Además se parten 31 diagonales a 330 (Detalle B) y se
crean 31 empalmes B antes de sus nudos en K." Captura **`fase13-04-plan.png`**. **No pulses Crear N conexiones** (partiría 31
diagonales); pulsa **Descartar**. Avisa.

```powershell
Anota "13-9 batch_plan" { .\scripts\conn-call.ps1 batch_plan '{"element_ids": [1245533, 1249631, 1249632], "mark": false, "include_specs": false}' }
$plan = (.\scripts\conn-call.ps1 batch_plan '{"element_ids": [1245533, 1249631, 1249632], "mark": false, "include_specs": false}' | ConvertFrom-Json).data.plan_id
Anota "13-9 discard" { .\scripts\conn-call.ps1 batch_plan_discard ('{"plan_id": "' + $plan + '"}') }
```

Se espera: N1 `ready` con `split_steps` (2, `split_distance_mm` ≈ 485, `splice_node` N1B / N1B2), N1B y N1B2 `after_split`,
`split_step_count: 2`, `after_split_count: 2`, `summary_text` con la frase de los cortes.

### 13-10. Pruebas, puente y log **(instalador)**

```powershell
Anota "13-10 probar_conexiones" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "13-10 catalog_list" { .\scripts\conn-call.ps1 catalog_list '{}' }
Anota "13-10 log failed" { Get-Content "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "_failed" }
Anota "13-10 log fase 13" { Get-Content "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "split|ribbon_split_chain|batch_split" | Select-Object -Last 20 }
git add docs\fases\resultados-fase-13.md docs\fases\capturas\fase13-*.png
git commit -m "Fase 13: resultados del instalador (0.13.0, sondeo 30, cadena del nudo en K)"
git push origin main
```

Se espera: `Resultado: 49/49 pruebas correctas` (46 + 3 del puente; la 32c parte y la 32e une 1249632), `tools/list` con las 25
`conn_*`, ningún `*_failed` del add-in en el log (los `ribbon_split_chain_failed` o `ribbon_split_failed`, si los hay, se anotan
tal cual). **(la persona)** Cierra Revit sin guardar la copia si quedó algo raro; si todo quedó a cero, guarda o no, como prefieras.
