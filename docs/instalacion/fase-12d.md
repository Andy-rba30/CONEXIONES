# Instalación y prueba de la ronda 12d: add-in 0.12.3 (las advertencias de Revit en el log) y el sondeo 29 (los objetos del modelo de Advance Steel y el corte)

Objetivo: desplegar el add-in **0.12.3** (cierre de la ronda 12c, `docs/fases/fase-12.md`, sección 9) y comprobar sobre la
**copia** del modelo: (1) el **sondeo 29**: la viga de Advance Steel de la diagonal buscada **recorriendo los objetos del modelo**
(`DatabaseManager.GetModelObjectIds`, lo que el sondeo 28 saltó), el corte `BeamShortening` si la encuentra y la `WeldLine`
conectada; (2) **qué dicen las advertencias de Revit** que la 12c devolvió como `REVIT_WARNING` (2 en el nudo en K, 8 en el
empalme): desde la 0.12.3 cada una queda en el log como `revit_failure` con su texto y sus elementos, y se lee creando y
borrando **un** nudo en K; (3) `--puente` 38/38. Unos **20 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. Esta ronda **crea una conexión y la borra**; el sondeo 29 deshace lo que escribe salvo que la persona pida
verlo (paso 12d-4).

Lo demás de la Fase 12 y de las rondas 12b y 12c ya está contrastado (`fase-12.md` 7.1, 8.1 y 9.1). **No se repite nada de eso.**

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`** ni `scripts\sondeos\`); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la
ventana de PowerShell. Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso
12d-1. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados
**(instalador)** van en PowerShell. **Durante toda la ronda solo debe estar abierta la copia.** **Si un sondeo dice FALLO, PARADA
o ERROR, no es un fallo de la instalación**: es lo que viene a averiguar; copia la salida entera y sigue. El sondeo 29 va
**siempre con `-SinTransaccion`** (lee la selección).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 12d-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la 12c terminó con `list` a cero).
- Las plantillas del catálogo: `templates_count: 2` (`Nudo tipico Detalle D` y `Nudo K cordon superior`). Se quedan.
- Las barras de siempre: cordón superior **1245533**, diagonales **1249631** y **1249632** (nudo en K).

### 12d-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-12d.md"
"# Resultados de la ronda 12d`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "12d-1 git" { git log -1 --oneline; git status --short }
Anota "12d-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "12d-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "12d-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "12d-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "12d-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "12d-1 sondeo 29 desplegado" { Get-ChildItem scripts\sondeos\29-*.py | Select-Object Name, Length }
```

Se espera: `0 Advertencia(s)`, `Superado: 291`, `== MotorConexiones 0.12.3.0 desplegado en Revit 2027 ==`, 25 rutas y 23
herramientas, DLL `0.12.3.0`.

### 12d-2. Abrir Revit, ping y list **(la persona abre; el instalador llama)**

**(la persona)** Abre Revit 2027 y la copia `HANGAR_PRUEBA_sondeo.rvt` (solo ella), vista 3D activa. Espera a que cargue la
pestaña **ARBA** y la de **pyRevit**.

```powershell
Anota "12d-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
Anota "12d-2 list" { .\scripts\conn-call.ps1 -Operation list }
```

Se espera: `ping` con `addin_version 0.12.3` y `document.title: HANGAR_PRUEBA_sondeo`; `list` 0.

### 12d-3. Sondeo 29: los objetos del modelo de Advance Steel, la viga y el corte **(instalador; la persona no toca Revit)**

```powershell
Anota "12d-3 sondeo 29 objetos de Advance Steel" { .\scripts\revit-exec.ps1 -File scripts\sondeos\29-viga-objetos-advance-steel.py -SinTransaccion -TimeoutSec 900 }
Anota "12d-3 sondeo 12 tras el sondeo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12d-3 sondeo 13 tras el sondeo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Qué mirar y qué devolver **literalmente** (es la decisión de la ronda):

- Las líneas `puente: …` (el Guid de Revit), `tipos: DatabaseManager=…` y las de `DocumentManager.GetCurrentDatabase() -> …` /
  `CADDocumentManager.GetCurrentDwg() -> …` con sus métodos con `ObjectId`.
- `abrir con: …` y `GetModelObjectIds, variantes: …`; después cada `GetModelObjectIds(...) -> N ObjectId` (o su `ERROR`).
- Las líneas `viga 1: …` a `viga 6: …` (tipo, `Handle` y demás identificadores, inicio y fin en mm) y la línea `extremos de la
  barra de Revit en mm: …`: si las vigas están en otro origen o en otra unidad, se ve aquí.
- `…: tipos recorridos: …; vigas vistas: N` por cada lista y la línea **`viga de Advance Steel: …`** (encontrada por Guid, por
  geometría, o `NO encontrada`).
- Con la viga: `metodos de la viga con …`, `ANTES: …`, `BeamShortening(...) creado; …`, `DESPUES: …`, `feature: …` y
  **`BeamShortening: AddFeature OK…`** o **`FALLO…`**.
- `WeldLine creada: …`, `WeldLine.Connect: …`, `WeldLine: WriteToDb OK…` o `FALLO…`.
- `Elementos nuevos con la transaccion abierta`, `Elementos nuevos tras cerrar: 0` y el `RESUMEN`.
- Sondeos 12 (`1249631: inicio 0.0 | fin 69.2`, como antes) y 13 a cero.

### 12d-4. Ver el corte, si la persona quiere **(opcional)**

Solo si el paso 12d-3 dijo `BeamShortening: AddFeature OK`:

```powershell
New-Item -ItemType File -Force "$env:TEMP\sondeo29-commit.txt" | Out-Null
Anota "12d-4 sondeo 29 con commit" { .\scripts\revit-exec.ps1 -File scripts\sondeos\29-viga-objetos-advance-steel.py -SinTransaccion -TimeoutSec 900 }
Remove-Item -Force "$env:TEMP\sondeo29-commit.txt"
```

**(la persona)** Mira el extremo superior de la diagonal 1249631 (cerca de X = −9355, Z = 19885), en nivel de detalle **Fino**:
¿quedó cortada **inclinada** (paralela a la cara del cordón) o recta?, ¿cuánto se acortó? Captura
`docs\fases\capturas\fase12d-01-corte.png`. Anota también la línea `extension del extremo: … mm` del final y las líneas
`nuevo …`. Después **Ctrl+Z** hasta que desaparezcan (o cierra sin guardar al final). Luego:

```powershell
Anota "12d-4 sondeo 12 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12d-4 sondeo 13 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

### 12d-5. Las advertencias de Revit: crear y borrar el nudo en K **(la persona crea; el instalador lista y borra)**

1. **(la persona)** Selecciona el cordón **1245533** y las diagonales **1249631** y **1249632**, pulsa **Conectar** (debe
   proponer "Nudo K cordon superior") y pulsa **Crear**. Anota el cuadro entero: **cuántos elementos** (se esperan 2) y **qué
   avisos** (`END_CUT_APPROXIMATED` y los `REVIT_WARNING`; si el cuadro enseña el texto "Advertencia de Revit: … [elementos: …]",
   cópialo). No hace falta captura.
2. **(instalador)**:

   ```powershell
   Anota "12d-5 list tras el nudo K" { .\scripts\conn-call.ps1 -Operation list }
   ```

   Copia el `connection_id` de la conexión listada y bórrala:

   ```powershell
   Anota "12d-5 delete nudo K" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL LIST>"}' }
   Anota "12d-5 sondeo 12 tras borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "12d-5 sondeo 13 tras borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

Se espera: `list` con 1 conexión (sus `warnings` con los `REVIT_WARNING` y su texto) y 0 tras borrar (`delete` con las barras
restauradas), sondeos 12 y 13 a cero.

### 12d-6. Puente y log **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "12d-6 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "12d-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "12d-6 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "12d-6 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|revit_failure|advance_steel_weld_written|fabrication_transaction_commit|fabrication_transaction_open|direct_weld_member|ribbon_create|_failed|ribbon_batch_window_error|plan_event_failed" | Select-Object -Last 120 }
Select-String -Path $salida -Pattern "^## 12d-" | Select-Object LineNumber, Line
```

Se espera: `servidor 8000 escuchando` con una fila; **`Resultado: 38/38 pruebas correctas`**; `templates_count: 2`; en el log
`"addin_version":"0.12.3"`, **las líneas `revit_failure`** del nudo en K (una por advertencia, con `severity`, `text`, `elements`,
`definition` y `transaction`: son lo que viene a leer esta ronda), `advance_steel_weld_written` × 2 y `fabrication_transaction_commit`
con `missing 0`, y **ninguna** línea `advance_steel_weld_failed`, `ribbon_create_failed`, `plan_event_failed` ni
`ribbon_batch_window_error` (si las hay, cópialas enteras). **La última línea debe listar todas las secciones `## 12d-`**; si falta
alguna, repite su `Anota`.

### 12d-7. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "12d-7 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-12d.md docs\fases\capturas
   git commit -m "Ronda 12d: resultados del instalador (0.12.3, sondeo 29, revit_failure)"
   git pull --no-rebase origin main
   git push origin main
   ```

Qué devolver: `docs/fases/resultados-fase-12d.md` (lo escribe `Anota`), la captura `fase12d-01` si hubo paso 12d-4 y, en el chat,
lo que no quedó en el archivo: **el sondeo 29 en dos líneas** (cuántos `ObjectId` dio `GetModelObjectIds`, cuántas vigas vio y si
sus extremos coinciden con los de Revit; viga encontrada sí / no y por qué camino; `BeamShortening` corta sí / no y si el corte se
vio inclinado), **el texto de las `revit_failure`** del nudo en K (dos líneas literales del log) y el `Resultado:` del puente.
