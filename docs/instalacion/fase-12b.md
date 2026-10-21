# Instalación y prueba de la ronda 12b: add-in 0.12.1, los sondeos 25, 26 y 27 (corte y soldadura nativas, el perfil del cordón) y `--puente` 38/38

Objetivo: desplegar el add-in **0.12.1** (cierre de la Fase 12, `docs/fases/fase-12.md`, sección 7) y correr sobre la
**copia** del modelo **solo lo que la Fase 12 dejó sin decidir**: (1) el **sondeo 27** (solo lee): qué guarda el tipo
HSS12X8X1/2 como ancho y alto y cómo está girado, porque el add-in lo leyó como 203 × 203; (2) el **sondeo 26**: escribir una
soldadura nativa de Advance Steel (`WeldLine`) con la firma que el sondeo 24 encontró y no intentó; (3) el **sondeo 25**:
cortar el extremo de la diagonal con `BeamShortening` de Advance Steel (el sondeo 22 solo listó el tipo); (4) por el puente,
`validate` de los fixtures K y C (ahora con `calculated_values.kind` y `eccentricity_mm`) y **`probar_conexiones.py --puente`
38/38** (en la Fase 12 dio 36/38 por eso). Unos **25 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. **Esta ronda no crea ninguna conexión** y los sondeos **deshacen** lo que escriben, salvo que la persona
pida verlo (paso 12b-4).

Lo demás de la Fase 12 ya está contrastado (`fase-12.md` 7.1): Conectar en el nudo en K y en el empalme, el lote de 40, los
borrados, el sondeo 23 (la fila 4 × 1 y `Connect` funcionan). **No se repite nada de eso.**

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`** ni `scripts\sondeos\`); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la
ventana de PowerShell. Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso
12b-1. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados
**(instalador)** van en PowerShell. **Durante toda la ronda solo debe estar abierta la copia** (ningún otro modelo). **Si un
sondeo dice FALLO, PARADA o ERROR, no es un fallo de la instalación**: es lo que viene a averiguar; copia la salida entera y
sigue.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 12b-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la Fase 12 terminó con `list` a cero). Puede quedar el
  marcador gris y la etiqueta de N5 del plan `dd6cf93f…` (no se descartó): el paso 12b-2 lo descarta.
- Las plantillas del catálogo: `templates_count: 2` (`Nudo tipico Detalle D` y `Nudo K cordon superior`). Se quedan.
- Las barras de siempre: cordón superior **1245533**, diagonales **1249631** y **1249632**, cordón central **1249510** (los
  sondeos las usan por defecto; con selección en Revit, usan la selección).

### 12b-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-12b.md"
"# Resultados de la ronda 12b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "12b-1 git" { git log -1 --oneline; git status --short }
Anota "12b-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "12b-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "12b-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "12b-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "12b-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "12b-1 sondeos nuevos desplegados" { Get-ChildItem scripts\sondeos\25-*.py, scripts\sondeos\26-*.py, scripts\sondeos\27-*.py | Select-Object Name, Length }
```

Se espera: `0 Advertencia(s)`, `Superado: 268`, `== MotorConexiones 0.12.1.0 desplegado en Revit 2027 ==`, 25 rutas y 23
herramientas, DLL `0.12.1.0`.

### 12b-2. Abrir Revit, ping y descartar el plan viejo **(la persona abre; el instalador llama)**

**(la persona)** Abre Revit 2027 y la copia `HANGAR_PRUEBA_sondeo.rvt` (solo ella), vista 3D activa. Espera a que cargue la
pestaña **ARBA** y la de **pyRevit**.

```powershell
Anota "12b-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
Anota "12b-2 list" { .\scripts\conn-call.ps1 -Operation list }
Anota "12b-2 sondeo 17 antes" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
```

Si el sondeo 17 dice "Marcadores de plan … en el modelo: 1" (el N5 del plan `dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c`):

```powershell
Anota "12b-2 batch_plan_discard del plan viejo" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"plan_id":"dd6cf93f-76d6-4fe0-81d1-2defaaf4da7c","all":true}' }
Anota "12b-2 sondeo 17 despues" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "12b-2 sondeo 21 despues" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
```

Se espera: `ping` con `addin_version 0.12.1` y `document.title: HANGAR_PRUEBA_sondeo`; `list` con 0 conexiones; tras descartar,
sondeos 17 y 21 a cero (si el `discard` dice que el plan no existe —Revit se reinició—, anótalo: los marcadores se quitan con el
`all: true` igual; si quedara alguno, **(la persona)** Planificar lote → Descartar plan).

### 12b-3. Los tres sondeos, en este orden: 27, 26 y 25 **(instalador; la persona no toca Revit)**

El 27 solo lee. El 26 y el 25 abren su `FabricationTransaction` y la **cancelan** al final: no debe quedar nada. Si alguno
imprime `PARADA`, `FALLO` o `ERROR`, copia la salida y sigue: eso es el resultado.

```powershell
Anota "12b-3 sondeo 27 perfil del cordon" { .\scripts\revit-exec.ps1 -File scripts\sondeos\27-perfil-cordon.py -TimeoutSec 300 }
Anota "12b-3 sondeo 26 soldadura WeldLine" { .\scripts\revit-exec.ps1 -File scripts\sondeos\26-soldadura-weldline.py -SinTransaccion -TimeoutSec 600 }
Anota "12b-3 sondeo 25 corte Advance Steel" { .\scripts\revit-exec.ps1 -File scripts\sondeos\25-corte-advance-steel.py -SinTransaccion -TimeoutSec 600 }
Anota "12b-3 sondeo 12 tras los sondeos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12b-3 sondeo 13 tras los sondeos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Qué mirar y qué devolver literalmente (son **las tres decisiones de la ronda**):

- **Sondeo 27**: para la barra **1245533**, las líneas `STRUCTURAL_SECTION_COMMON_WIDTH`, `STRUCTURAL_SECTION_COMMON_HEIGHT`,
  `STRUCTURAL_SECTION_SHAPE`, "con nombre corto: …" **entera**, `STRUCTURAL_BEND_DIR_ANGLE` (Cross-Section Rotation) y las
  justificaciones, y la línea `SOLIDO: canto EN EL PLANO … | ancho PERPENDICULAR …`. Se espera canto ~203 y ancho ~305; si sale
  al revés, también vale: es lo que viene a saber.
- **Sondeo 26**: las líneas `puente: …` (cuál encontró la viga de Advance Steel), `WeldLine creada: …` (qué propiedades aceptó),
  `WeldLine.Connect: …`, `WeldLine: WriteToDb OK…` o `FALLO…`, `WeldPoint: …`, "Elementos nuevos tras cerrar: 0" y el RESUMEN.
- **Sondeo 25**: las líneas `puente: …`, `viga de Advance Steel: …` (si dice `NO encontrada`, copia todos los intentos: es el
  resultado), `metodos de la viga con Feature/…`, `ANTES: …`, `BeamShortening(...) creado; AngleOnY=…`, `DESPUES: …`, `feature:
  …`, `BeamMultiContourNotch: …`, la extensión del extremo al final y el RESUMEN.
- Sondeos 12 y 13 **a cero** después (12: `1249631: inicio 0.0 | fin 69.2`, como antes). Si el 13 encuentra restos, los limpia:
  anótalo (sería que `CancelTransaction` no deshizo algo).

### 12b-4. Ver el corte y la soldadura, si la persona quiere **(opcional; la persona decide, el instalador ejecuta)**

Los sondeos 25 y 26 hacen **`Commit`** en vez de cancelar si existe un archivo marca en `%TEMP%`. Solo si el paso 12b-3 dijo
`WriteToDb OK` (26) o `AddFeature OK` (25) merece la pena; si no, salta este paso.

```powershell
New-Item -ItemType File -Force "$env:TEMP\sondeo26-commit.txt" | Out-Null
Anota "12b-4 sondeo 26 con commit" { .\scripts\revit-exec.ps1 -File scripts\sondeos\26-soldadura-weldline.py -SinTransaccion -TimeoutSec 600 }
Remove-Item -Force "$env:TEMP\sondeo26-commit.txt"
```

**(la persona)** Mira el extremo superior de la diagonal 1249631 (nudo en K del cordón superior, cerca de X = −9355, Z =
19885): debe verse la plancha de apoyo de 200 × 200 y la soldadura en su contorno. Captura `docs\fases\capturas\fase12b-01-weldline.png`
(de cerca). Después **Ctrl+Z** las veces que haga falta hasta que desaparezcan (o anota que no se deshace: entonces el sondeo 13
limpia al final). Lo mismo con el 25:

```powershell
New-Item -ItemType File -Force "$env:TEMP\sondeo25-commit.txt" | Out-Null
Anota "12b-4 sondeo 25 con commit" { .\scripts\revit-exec.ps1 -File scripts\sondeos\25-corte-advance-steel.py -SinTransaccion -TimeoutSec 600 }
Remove-Item -Force "$env:TEMP\sondeo25-commit.txt"
```

**(la persona)** Mira si el extremo de la diagonal quedó **cortado en plano inclinado** (paralelo a la cara del cordón) o
acortado recto; captura `fase12b-02-corte.png`. Después Ctrl+Z. Luego, pase lo que pase:

```powershell
Anota "12b-4 sondeo 12 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12b-4 sondeo 13 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

### 12b-5. Lo que quedó sin ver de Conectar **(la persona; sin crear nada)**

1. Selecciona **una sola barra** (la diagonal 1249631) y pulsa **Conectar**: debe pedir pinchar las demás barras del nudo
   (`PickObject`); pincha el cordón 1245533 y la otra diagonal 1249632 y termina (Esc o el botón): debe abrirse la
   previsualización del nudo en K con la plantilla "Nudo K cordon superior". **Cancelar.** Captura `fase12b-03-una-barra.png` si
   algo sale distinto.
2. Selecciona los **cinco** elementos del nudo N9 de la cercha de la 8c (los dos tramos del cordón superior que se encuentran
   en N9, **1245533** y **1245532**, y las dos diagonales de N9) y pulsa **Conectar**: debe abrirse primero la ventana del
   **empalme** (arranque Detalle C); al cerrarla con **Cancelar**, un cuadro Sí/No para abrir la del **nudo en K**; Sí → la
   ventana del nudo; **Cancelar**. Anota los dos títulos y si el cuadro apareció.
3. Con la ventana del empalme abierta (vuelve a abrirla si hace falta), en el panel pon **Arriba: borde (mm) = 10**: debe salir
   `BOLT_EDGE_DISTANCE_TOO_SMALL` en la tabla de abajo con el campo `bolts.faces.top.edge_mm` y Crear deshabilitado. Vuelve a 25.
   **Cancelar.**

Si algún cuadro inesperado aparece, captúralo (`fase12b-04-cuadro.png`).

### 12b-6. Puente y log **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Anota "12b-6 validate fixture K" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\nudo-K-cordon-superior.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "12b-6 validate fixture C" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-C.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "12b-6 node_info nudo en K" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1245533,1249631,1249632]}' }
Anota "12b-6 list" { .\scripts\conn-call.ps1 -Operation list }
Anota "12b-6 sondeo 17 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "12b-6 sondeo 21 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "12b-6 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "12b-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "12b-6 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "12b-6 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_connect|ribbon_create|catalog_apply|_failed|ribbon_batch_window_error|plan_event_failed" | Select-Object -Last 120 }
Select-String -Path $salida -Pattern "^## 12b-" | Select-Object LineNumber, Line
```

Se espera: `validate` del K con `is_valid true`, **`calculated_values.kind: "node"`**, **`eccentricity_mm: 54.9`** y los dos
avisos `MEMBER_ECCENTRIC` **sin** la frase "El JSON declara…" (el fixture ya trae ∓27,45); del C con `kind: "splice"` y
`eccentricity_mm: 0`; `node_info` con `eccentricity_mm 54.9`; `list` 0; sondeos 17 y 21 a cero; `servidor 8000 escuchando` con
una fila (si sale vacío, espera 30 s y repite ese `Anota`); **`Resultado: 38/38 pruebas correctas`** (si fallan 30b o 30c, copia
sus bloques); `templates_count: 2` (si el `--puente` dejó `PRUEBA probar_conexiones`, anótalo: se borra con `catalog_delete`); en
el log `"addin_version":"0.12.1"`, los `ribbon_connect` del paso 12b-5 (`starter`/`template`, `is_valid`) y **ninguna** línea
`ribbon_connect_failed`, `ribbon_create_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas
enteras). **La última línea debe listar todas las secciones `## 12b-`**; si falta alguna, repite su `Anota`.

### 12b-7. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "12b-7 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-12b.md docs\fases\capturas
   git commit -m "Ronda 12b: resultados del instalador (0.12.1, sondeos 25-27, puente 38/38)"
   git pull --no-rebase origin main
   git push origin main
   ```

Qué devolver: `docs/fases/resultados-fase-12b.md` (lo escribe `Anota`), las capturas `fase12b-01` a `fase12b-04` si las hubo y,
en el chat, lo que no quedó en el archivo: **los tres sondeos resumidos en una línea cada uno** (27: canto en el plano y ancho
perpendicular del 1245533, y qué parámetro del tipo guarda cada uno; 26: `WeldLine` escribe sí / no y con qué `Connect`; 25:
viga encontrada sí / no y por qué camino, `BeamShortening` corta sí / no y si el corte se vio inclinado), lo visto en 12b-5
(pedir pinchar, el cuadro Sí/No, el error del borde) y el `Resultado:` del puente.
