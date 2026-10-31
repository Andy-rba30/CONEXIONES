# Instalación y prueba de la ronda 13c (cierre de la ronda 13b): add-in 0.13.2, la advertencia del corte en el log y las uniones restauradas

Objetivo: desplegar el add-in **0.13.2** (`docs/fases/fase-13.md`, sección 8) y comprobar sobre una **copia** del modelo, en este orden:
(0) llevar la rama del cierre a `main`; (1) compilar, probar y desplegar; (2) `ping` con la 0.13.2 y la limpieza; (3) el **sondeo 30 v3**
(el corte sale ya a ≈ 485 mm y dice si la diagonal admitía uniones automáticas en cada extremo); (4) `conn_split_member` de 1249632
**sin** `REVIT_WARNING` (la advertencia "slightly off axis" queda en el log con `filtered: true`), el registro con los campos nuevos,
`conn_unsplit` y, **después de deshacer, `node_info` del nudo en K con sus dos diagonales** (en la 13b la cadena partió una sola
diagonal y hay que saber si fue por la selección o porque 1249632 dejó de entrar en el nudo tras deshacer un corte); (5) la cadena
de Conectar con las **tres** barras seleccionadas; (6) el puente, el log, commit y push. Unos **25 minutos** sobre
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell. Todos los
comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 13c-0 y vuelve a definir `Anota`,
`$salida`, `$ext`, `$py` y `$log`. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit;
los marcados **(instalador)** van en PowerShell. **Los ids que un paso pide copiar de la salida anterior (`split_id`,
`new_element_id`) los escribes tú a mano en el comando**: en la 13b la variable `$s` salió vacía y el `node_info` de los tramos falló
por eso, no por el add-in.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 13c-0.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni barras partidas (la 13b terminó con `list` 0 y `splits_count: 0`).
- El nudo en K del cordón superior: cordón **1245533** y diagonales **1249631** y **1249632**.

### 13c-0. Llevar la rama del cierre a `main` **(instalador, Revit cerrado)**

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

Se espera: el merge sin conflictos y `git log -1` con "Cierre de la ronda 13b (add-in 0.13.2)…". Si hay conflicto, **para** y copia la salida.

### 13c-1. Archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
$salida = "docs\fases\resultados-fase-13c.md"
"# Resultados de la ronda 13c`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "13c-1 git" { git log -1 --oneline; git status --short }
Anota "13c-1 build" { dotnet build MotorConexiones.sln -c Release --nologo }
Anota "13c-1 test" { dotnet test src\MotorConexiones.Tests -c Release --no-build --nologo }
Anota "13c-1 deploy" { .\scripts\deploy.ps1 }
Anota "13c-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
```

Se espera: `Build succeeded` sin avisos, `Passed! … 314`, `MotorConexiones 0.13.2.0 desplegado en Revit 2027`, `instalar-conn` copiando
`conexiones.py` (27 rutas) y `conn_tools.py`.

### 13c-2. Abrir Revit, ping y limpieza **(la persona abre; instalador)**

**(la persona)** Abre Revit 2027 y la copia; espera a que pyRevit cargue y avisa.

```powershell
Anota "13c-2 ping" { .\scripts\conn-call.ps1 ping }
Anota "13c-2 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13c-2 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13c-2 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "13c-2 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "13c-2 node_info nudo K antes de nada" { .\scripts\conn-call.ps1 node_info '{"element_ids": [1245533, 1249631, 1249632]}' }
```

Se espera: `addin_version 0.13.2`, `list` 0, `splits_count: 0`, los sondeos 12 y 13 a cero, y en `node_info` `kind: node`,
`eccentricity_mm 54,9` y **las dos diagonales** (1249631 y 1249632) con `reaches_node: true` y `split_point_mm`. **Guarda esta salida
para compararla en el 13c-4.**

### 13c-3. Sondeo 30 v3 **(instalador; la persona mira)**

```powershell
Anota "13c-3 sondeo 30 v3" { .\scripts\revit-exec.ps1 -File scripts\sondeos\30-partir-barra.py -SinTransaccion -TimeoutSec 600 }
Anota "13c-3 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
```

Devuelve la salida **entera**. Lo que importa: (1) las líneas `uniones automaticas permitidas: inicio=… fin=…` de la diagonal y del
cordón **antes** de partir (es lo que el add-in 0.13.2 guarda y restaura); (2) `perfil del cordon: b=203.2 Ht=304.8 giro=90.0` y ahora
`cara a 143,7 … corte a 483 mm` (la v2 decía 557 porque tomaba el lado perpendicular); (3) `node_info [1249631, …]: ok=True kind=splice`
(la v2 dio "recibidos: 1" por un `long` de IronPython); (4) el deshacer con `diferencia 0.00 mm` y `elementos nuevos … []`.

### 13c-4. `conn_split_member` de 1249632 sin la advertencia, el registro, `conn_unsplit` y el nudo después **(instalador)**

```powershell
Anota "13c-4 split" { .\scripts\conn-call.ps1 split '{"element_id": 1249632, "chord_element_id": 1245533}' }
Anota "13c-4 log revit_failure del corte" { Get-Content -Encoding UTF8 $log | Select-String "revit_failure" | Select-Object -Last 3 }
Anota "13c-4 registro del corte" { .\scripts\conn-call.ps1 unsplit '{}' }
```

Copia de la salida del `split` el `split_id` y el `new_element_id` y escríbelos en los dos comandos siguientes (sin variables):

```powershell
Anota "13c-4 node_info tramos" { .\scripts\conn-call.ps1 node_info '{"element_ids": [1249632, NEW_ELEMENT_ID]}' }
Anota "13c-4 unsplit" { .\scripts\conn-call.ps1 unsplit '{"split_id": "SPLIT_ID"}' }
Anota "13c-4 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13c-4 node_info nudo K tras deshacer" { .\scripts\conn-call.ps1 node_info '{"element_ids": [1245533, 1249631, 1249632]}' }
Anota "13c-4 log cortes" { Get-Content -Encoding UTF8 $log | Select-String "member_split|member_unsplit|unsplit_member" | Select-Object -Last 4 }
```

Se espera: el `split` con `mechanism: copy_element`, `approximated_parameters: []` y **`warnings` vacío**; en el log la línea
`revit_failure` "Beam or Brace is slightly off axis…" con **`filtered: true`** y `transaction: MotorConexiones: Partir barra 1249632`;
el registro con `original_start_feet`, `original_end_feet` y **`original_join_allowed`** (dos valores); `node_info` de los tramos
`kind: splice`; `unsplit` `complete: true`; `splits_count: 0`; y **`node_info` del nudo en K igual que en el 13c-2** (`kind: node`,
las dos diagonales con `reaches_node: true`, `eccentricity_mm 54,9`). Si 1249632 ya no sale como barra del nudo, copia las dos salidas
enteras: es lo que hay que saber.

### 13c-5. La cadena de Conectar con las tres barras **(la persona; instalador)**

**(la persona)** Selecciona **las tres**: 1245533, 1249631 **y 1249632** (comprueba en Propiedades que son tres elementos) y pulsa
**Conectar**. La casilla debe decir "Partir las **2** diagonales y crear sus empalmes del Detalle B a 330"; si dice "Partir la
diagonal" (una sola), anótalo y mira qué barras lista la previsualización. Déjala marcada y pulsa **Crear**. Anota el cuadro
**literal**: se esperan **dos** líneas "Corte: …", **dos** "Empalme B: …", "Nudo en K: … (2 elementos, 2 retiros)" y en "Advertencias"
**solo** `BOLT_NUTS_APPROXIMATED; END_CUT_APPROXIMATED` (sin `REVIT_WARNING`). Cierra el cuadro y avisa.

```powershell
Anota "13c-5 list" { .\scripts\conn-call.ps1 list '{}' }
Anota "13c-5 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
```

Copia de `list` los `connection_id` de las conexiones `end_plate` (los empalmes B) y el de la `direct_weld` (el nudo en K) y bórralas
**en este orden** (primero los empalmes, al final el nudo en K), un comando por conexión:

```powershell
Anota "13c-5 delete empalme B 1" { .\scripts\conn-call.ps1 delete '{"connection_id": "ID_EMPALME_1"}' }
Anota "13c-5 delete empalme B 2" { .\scripts\conn-call.ps1 delete '{"connection_id": "ID_EMPALME_2"}' }
Anota "13c-5 delete nudo K (deshace los cortes)" { .\scripts\conn-call.ps1 delete '{"connection_id": "ID_NUDO_K"}' }
Anota "13c-5 unsplit lista" { .\scripts\conn-call.ps1 unsplit '{}' }
Anota "13c-5 node_info nudo K final" { .\scripts\conn-call.ps1 node_info '{"element_ids": [1245533, 1249631, 1249632]}' }
Anota "13c-5 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `list` con 3 conexiones y `unsplit` con 2 cortes; los `delete` en orden (si el del nudo en K avisa "No se pudo abrir la
FabricationTransaction… asynchronous fabrication tasks are queued", es conocido desde la Fase 10: Advance Steel aún tiene tareas en
cola del borrado anterior y los elementos se borran igual); después `splits_count: 0`, el `node_info` del nudo con sus dos diagonales
enteras y el sondeo 12 con 0 conexiones.

### 13c-6. Puente, log, commit y push **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "13c-6 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "13c-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "13c-6 log failed" { Get-Content -Encoding UTF8 $log | Select-String "_failed" }
Select-String -Path $salida -Pattern "^## 13c-" | Select-Object LineNumber, Line
git add docs\fases\resultados-fase-13c.md
git commit -m "Ronda 13c: resultados del instalador (0.13.2, advertencia del corte en el log, uniones restauradas, nudo en K tras deshacer)"
git push origin main
git status --short
git log -1 --oneline
```

Se espera: una fila en `servidor 8000 escuchando`, **`Resultado: 49/49 pruebas correctas`** con `addin=0.13.2`, ningún `*_failed` del
add-in en el log (los `BATCH_NODE_FAILED` del token alterado son de la prueba) y las secciones `13c-1` a `13c-6` en el archivo.
**(la persona)** Cierra Revit sin guardar la copia.
