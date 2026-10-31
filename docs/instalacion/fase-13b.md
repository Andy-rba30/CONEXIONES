# Instalación y prueba de la ronda 13b (cierre de la Fase 13): add-in 0.13.1, el sondeo 30 v2 y el corte sobre la línea exacta

> **Ejecutada el 2026-10-31** (`docs/fases/resultados-fase-13b.md`; cierre en `docs/fases/fase-13.md`, sección 8). Errata: en el
> paso 13b-4 la variable `$s` salió vacía (`ConvertFrom-Json` sobre la salida de `conn-call.ps1`, que empieza con una línea de
> cabecera), así que el `node_info` de los tramos dio HTTP 500 y el `unsplit` con `$s.split_id` listó en vez de deshacer (el
> instalador lo deshizo a mano). La ronda 13c (`fase-13c.md`) pide escribir los ids a mano en los comandos.

Objetivo: desplegar el add-in **0.13.1** (`docs/fases/fase-13.md`, sección 7) y comprobar sobre una **copia** del modelo, en este orden:
(0) llevar la rama del cierre a `main`; (1) compilar, probar y desplegar; (2) `ping` con la 0.13.1 y la limpieza con los sondeos 12 y
13 (**con sus nombres reales**: en la Fase 13 no se ejecutaron porque la instalación los llamaba mal); (3) el **sondeo 30 v2** (ya no
decide nada: mide el desvío de eje de la diagonal original y lista sus parámetros; no deja nada en el modelo); (4) `conn_split_member`
y `conn_unsplit` de la diagonal 1249632 con la 0.13.1, mirando si la advertencia de Revit "Beam or Brace is slightly off axis"
desaparece; (5) opcional, la cadena de Conectar otra vez; (6) el puente, el log, commit y push. Unos **25 minutos** sobre
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. Esta ronda parte una barra de verdad en la copia (paso 4) y
lo deshace al momento.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell. Todos los
comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 13b-1 y vuelve a definir `Anota`,
`$salida`, `$ext`, `$py` y `PYTHONIOENCODING`. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen
en Revit; los marcados **(instalador)** van en PowerShell. Mientras una ventana del add-in está abierta, el instalador no ejecuta
nada. **Si el sondeo dice FALLO o PARADA, no es un fallo de la instalación**: copia la salida entera y sigue. Si un corte sale con un
aviso (`REVIT_WARNING`, `SPLIT_PARAMETERS_APPROXIMATED`, `SPLIT_UNDO_PARTIAL`), anótalo tal cual: es justo lo que se viene a mirar.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 13b-0.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in, sin marcadores ni etiquetas y **sin barras partidas** (la Fase 13
  terminó con todo a cero: `list` 0 y `unsplit` con `splits_count: 0`). Los sondeos 12 y 13 del paso 13b-2 lo confirman y limpian.
- El nudo en K del cordón superior de la Fase 13: cordón **1245533** y diagonales **1249631** y **1249632** (si los IDs no coinciden,
  apunta los reales en Propiedades y cámbialos en los comandos).

### 13b-0. Llevar la rama del cierre a `main` **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git fetch origin
git checkout main
git pull --no-rebase origin main
git merge --no-edit origin/claude/sweet-carson-pqqo3s
git push origin main
git log -3 --oneline
```

Se espera: `git status --short` vacío antes de empezar, el merge sin conflictos y `git log -1` con "Cierre de la Fase 13 (add-in
0.13.1)…". Si el merge diera conflicto, **para** y copia la salida: no resuelvas nada.

### 13b-1. Archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
$salida = "docs\fases\resultados-fase-13b.md"
"# Resultados de la ronda 13b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
$log = "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl"
$env:PYTHONIOENCODING = "utf-8"
Anota "13b-1 git" { git log -1 --oneline; git status --short }
Anota "13b-1 build" { dotnet build MotorConexiones.sln -c Release --nologo }
Anota "13b-1 test" { dotnet test src\MotorConexiones.Tests -c Release --no-build --nologo }
Anota "13b-1 deploy" { .\scripts\deploy.ps1 }
Anota "13b-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
```

Se espera: `Build succeeded` sin avisos, `Passed! … 314` (311 de la Fase 13 más 3 del cierre), `MotorConexiones 0.13.1.0 desplegado
en Revit 2027`, `instalar-conn` copiando `conexiones.py` (27 rutas) y `conn_tools.py`.

### 13b-2. Abrir Revit, ping y limpieza **(la persona abre; instalador)**

**(la persona)** Abre Revit 2027 y la copia `HANGAR_PRUEBA_sondeo.rvt`; espera a que pyRevit cargue y avisa.

```powershell
Anota "13b-2 ping" { .\scripts\conn-call.ps1 ping }
Anota "13b-2 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13b-2 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13b-2 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "13b-2 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera: `addin_version 0.13.1` (y `operations` con `split` y `unsplit`), `list` con 0 conexiones, `unsplit` con `splits_count: 0`,
el sondeo 12 con `conexiones en el modelo: 0` y las extensiones de las barras del Detalle D, el 13 sin restos. **Si el sondeo 12 o
el 13 dicen "no existe el archivo", copia el mensaje tal cual** (son `12-fase3-borrar.py` y `13-limpiar-fase1.py` en `scripts\sondeos\`).

### 13b-3. Sondeo 30 v2: el desvío de eje de la diagonal y sus parámetros **(instalador; la persona mira)**

```powershell
Anota "13b-3 sondeo 30 v2" { .\scripts\revit-exec.ps1 -File scripts\sondeos\30-partir-barra.py -SinTransaccion -TimeoutSec 600 }
Anota "13b-3 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
```

Devuelve la salida **entera**. Lo que importa: (1) ya no para en `barra()` (en la Fase 13 dio `Multiple targets could match:
ElementId(…)`); (2) en el paso 1, las líneas `diagonal 1249631 ANTES: dX … | dY … | dZ … mm | … grados respecto a X | … grados fuera
del plano vertical XZ`: con `dY 0.000000` la diagonal está en un plano vertical y **la advertencia "slightly off axis" de la Fase 13
no era de la barra**; con otro `dY`, la barra original ya se sale del plano y la advertencia es suya, no del corte; (3) en el paso 2,
"el punto del marco estaba a **N** mm de la linea real de la diagonal" (se espera ≈ 0,01); (4) en el paso 3, `parametros de la copia
distintos del original de antes (0): ninguno`, `DisallowJoinAtEnd(…) ok` y los dos tramos con su `dY`; (5) en el paso 6, `diferencia
0.00 mm`; (6) `elementos nuevos … []` dos veces y `grupo de transacciones REVERTIDO`. `unsplit` sigue con `splits_count: 0` (el sondeo
no usa el add-in).

### 13b-4. `conn_split_member` y `conn_unsplit` de 1249632 con la 0.13.1 **(instalador)**

```powershell
Anota "13b-4 split dry_run" { .\scripts\conn-call.ps1 split '{"element_id": 1249632, "chord_element_id": 1245533, "dry_run": true}' }
Anota "13b-4 split" { .\scripts\conn-call.ps1 split '{"element_id": 1249632, "chord_element_id": 1245533}' }
Anota "13b-4 log revit_failure del corte" { Get-Content -Encoding UTF8 $log | Select-String "revit_failure" | Select-Object -Last 4 }
$s = (.\scripts\conn-call.ps1 unsplit '{}' | ConvertFrom-Json).data.splits[0]
Anota "13b-4 registro del corte" { $s | ConvertTo-Json -Depth 5 }
Anota "13b-4 node_info tramos" { .\scripts\conn-call.ps1 node_info ('{"element_ids": [1249632, ' + $s.new_element_id + ']}') }
Anota "13b-4 unsplit" { .\scripts\conn-call.ps1 unsplit ('{"split_id": "' + $s.split_id + '"}') }
Anota "13b-4 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13b-4 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `dry_run` con `split_distance_mm` ≈ 483,2 y `theta_deg` 45; el corte real con `mechanism: copy_element`,
`approximated_parameters` vacío y el registro con **`original_start_feet` y `original_end_feet`** (nuevos en la 0.13.1). **Lo que se
viene a mirar**: si `warnings` del `split` sale **vacío** (en la Fase 13 traía `REVIT_WARNING: Beam or Brace is slightly off axis …`
sobre el tramo nuevo). Si sigue saliendo, copia la línea `revit_failure` del log (trae `definition`, el Guid de la advertencia) y el
`dY` del sondeo 30: con eso se decide en el cierre si se filtra como la de la 12d. `node_info` de los tramos con `kind: splice`;
`unsplit` con `complete: true`; después `splits_count: 0` y el sondeo 12 con las barras del fixture como antes.

### 13b-5. Opcional, si hay cinco minutos: la cadena de Conectar otra vez **(la persona; instalador)**

**(la persona)** Selecciona 1245533, 1249631 y 1249632 y pulsa **Conectar**; deja la casilla **marcada** y pulsa **Crear**. Anota el
cuadro **literal**: se espera "Diagonal partida, empalme B y nudo en K modelados" y en "Advertencias" **solo** `BOLT_NUTS_APPROXIMATED;
END_CUT_APPROXIMATED` (sin `REVIT_WARNING` si el 13b-4 salió limpio). Cierra el cuadro y avisa.

```powershell
Anota "13b-5 list" { .\scripts\conn-call.ps1 list '{}' }
$conns = (.\scripts\conn-call.ps1 list '{}' | ConvertFrom-Json).data.connections
Anota "13b-5 delete empalmes B" { foreach ($c in ($conns | Where-Object { $_.connection_type -eq "end_plate" })) { .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $c.connection_id + '"}') } }
Anota "13b-5 delete nudo K (deshace los cortes)" { $k = $conns | Where-Object { $_.connection_type -eq "direct_weld" }; .\scripts\conn-call.ps1 delete ('{"connection_id": "' + $k.connection_id + '"}') }
Anota "13b-5 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13b-5 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `list` con 3 conexiones; borrados los empalmes B primero, el `delete` del nudo en K deshace los dos cortes (sin
`SPLIT_IN_USE`), `unsplit` con `splits_count: 0` y el sondeo 12 con 0 conexiones.

### 13b-6. Puente, log, commit y push **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "13b-6 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "13b-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "13b-6 log failed" { Get-Content -Encoding UTF8 $log | Select-String "_failed" }
Anota "13b-6 log cortes" { Get-Content -Encoding UTF8 $log | Select-String "member_split|member_unsplit|split_member|unsplit_member|revit_failure" | Select-Object -Last 20 }
Select-String -Path $salida -Pattern "^## 13b-" | Select-Object LineNumber, Line
git add docs\fases\resultados-fase-13b.md
git commit -m "Ronda 13b: resultados del instalador (0.13.1, sondeo 30 v2, corte sobre la linea exacta)"
git push origin main
git status --short
git log -1 --oneline
```

Se espera: `servidor 8000 escuchando` con una fila (en la Fase 13 el puente falló a la primera porque el servidor no estaba
arrancado; este bloque lo arranca), **`Resultado: 49/49 pruebas correctas`** con `addin=0.13.1` y las 25 `conn_*`, ningún `*_failed`
del add-in en el log, y las secciones `13b-1` a `13b-6` en el archivo. **(la persona)** Cierra Revit sin guardar la copia.
