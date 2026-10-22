# Instalación y prueba de la ronda 12c: add-in 0.12.2 (el perfil del cordón bien leído, soldaduras `WeldLine`) y el sondeo 28 (la viga de Advance Steel)

Objetivo: desplegar el add-in **0.12.2** (cierre de la ronda 12b, `docs/fases/fase-12.md`, sección 8) y comprobar sobre la
**copia** del modelo: (1) que el add-in lee ahora el HSS12X8X1/2 como **203,2 en el plano × 304,8 perpendicular** con el giro de
90° (`node_info`) y que el límite de la excentricidad del `validate` pasa a **152,4**; (2) el **sondeo 28**: la viga de Advance
Steel de la diagonal buscada en todos los ensamblados, el corte `BeamShortening` si la encuentra y una `WeldLine` sin plancha;
(3) **crear y borrar** el nudo en K y el empalme para ver las soldaduras nativas (`WeldLine`) del add-in: cuántos elementos salen
y qué dice el log; (4) `--puente` 38/38. Unos **35 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca
el original**. Esta ronda **crea dos conexiones y las borra**; el sondeo 28 deshace lo que escribe salvo que la persona pida
verlo (paso 12c-4).

Lo demás de la Fase 12 y de la 12b ya está contrastado (`fase-12.md` 7.1 y 8.1). **No se repite nada de eso.**

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`** ni `scripts\sondeos\`); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la
ventana de PowerShell. Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso
12c-1. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados
**(instalador)** van en PowerShell. **Durante toda la ronda solo debe estar abierta la copia.** **Si un sondeo dice FALLO, PARADA
o ERROR, no es un fallo de la instalación**: es lo que viene a averiguar; copia la salida entera y sigue. Los sondeos que leen la
selección van **siempre con `-SinTransaccion`** (el 27 de la 12b cayó sin él).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 12c-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la 12b terminó con `list` a cero).
- Las plantillas del catálogo: `templates_count: 2` (`Nudo tipico Detalle D` y `Nudo K cordon superior`). Se quedan.
- Las barras de siempre: cordón superior **1245533**, diagonales **1249631** y **1249632** (nudo en K); tramos del empalme del
  Detalle C **1245531** y **1245530**.

### 12c-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-12c.md"
"# Resultados de la ronda 12c`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "12c-1 git" { git log -1 --oneline; git status --short }
Anota "12c-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "12c-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "12c-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "12c-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "12c-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "12c-1 sondeo 28 desplegado" { Get-ChildItem scripts\sondeos\28-*.py | Select-Object Name, Length }
```

Se espera: `0 Advertencia(s)`, `Superado: 291`, `== MotorConexiones 0.12.2.0 desplegado en Revit 2027 ==`, 25 rutas y 23
herramientas, DLL `0.12.2.0`.

### 12c-2. Abrir Revit, ping, el perfil del cordón y el límite nuevo **(la persona abre; el instalador llama)**

**(la persona)** Abre Revit 2027 y la copia `HANGAR_PRUEBA_sondeo.rvt` (solo ella), vista 3D activa. Espera a que cargue la
pestaña **ARBA** y la de **pyRevit**.

```powershell
Anota "12c-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
Anota "12c-2 list" { .\scripts\conn-call.ps1 -Operation list }
Anota "12c-2 node_info nudo en K" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1245533,1249631,1249632]}' }
Anota "12c-2 node_info empalme C" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1245531,1245530]}' }
Anota "12c-2 validate fixture K" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\nudo-K-cordon-superior.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "12c-2 validate fixture C" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-C.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
```

Se espera: `ping` con `addin_version 0.12.2` y `document.title: HANGAR_PRUEBA_sondeo`; `list` 0; en `node_info` del nudo en K el
cordón 1245533 con **`width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`** (antes 203,2 × 203,2) y las diagonales
con 63,5 × 63,5 y `section_rotation_deg -180`; en el del empalme los dos tramos con 203,2 × 304,8 y 90; `validate` del K
`is_valid true` con los dos `MEMBER_ECCENTRIC` diciendo **"admitida hasta 152.4 mm"** (antes 101,6); `validate` del C `is_valid
true`, `kind: splice`, 12 pernos. Si algún número es otro, cópialo tal cual: es lo que viene a saber.

### 12c-3. Sondeo 28: la viga de Advance Steel, el corte y la soldadura sin plancha **(instalador; la persona no toca Revit)**

```powershell
Anota "12c-3 sondeo 28 viga de Advance Steel" { .\scripts\revit-exec.ps1 -File scripts\sondeos\28-viga-advance-steel.py -SinTransaccion -TimeoutSec 900 }
Anota "12c-3 sondeo 12 tras el sondeo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12c-3 sondeo 13 tras el sondeo" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Qué mirar y qué devolver **literalmente** (es la decisión de la ronda):

- El bloque `1) SteelElementProperties` entero (los métodos estáticos del lado de Revit).
- `ensamblados de acero cargados: …`, si `ASCADLinkMgd` se cargó o no, y todas las líneas `tipo …` (los tipos con
  `DatabaseManager`, `DocumentManager`, `ObjectId`, `Guid`, `UniqueId`, `Link` y sus métodos estáticos).
- Todas las líneas `puente: …` (cada intento), `tipos recorridos: …` y la línea `viga de Advance Steel: …` o `NO encontrada…`.
- Con la viga: `metodos de la viga con …`, `ANTES: …`, `BeamShortening(...) creado; …`, `DESPUES: …`, `feature: …` y la línea
  `BeamShortening: AddFeature OK…` o `FALLO…`.
- `WeldLine creada: …`, `WeldLine.Connect: …` (`sin Connect` si no hubo viga), `WeldLine: WriteToDb OK…` o `FALLO…`, `WeldPoint: …`.
- `Elementos nuevos con la transaccion abierta`, `Elementos nuevos tras cerrar: 0` y el `RESUMEN`.
- Sondeos 12 (`1249631: inicio 0.0 | fin 69.2`, como antes) y 13 a cero.

### 12c-4. Ver el corte y la soldadura, si la persona quiere **(opcional)**

Solo si el paso 12c-3 dijo `BeamShortening: AddFeature OK` o `WeldLine: WriteToDb OK`:

```powershell
New-Item -ItemType File -Force "$env:TEMP\sondeo28-commit.txt" | Out-Null
Anota "12c-4 sondeo 28 con commit" { .\scripts\revit-exec.ps1 -File scripts\sondeos\28-viga-advance-steel.py -SinTransaccion -TimeoutSec 900 }
Remove-Item -Force "$env:TEMP\sondeo28-commit.txt"
```

**(la persona)** Mira el extremo superior de la diagonal 1249631 (cerca de X = −9355, Z = 19885): si el corte salió, ¿quedó
**inclinado** (paralelo a la cara del cordón) o recto?; la soldadura, ¿se ve sobre la cara inferior del cordón? Captura
`docs\fases\capturas\fase12c-03-sondeo28.png`. Anota también la última línea `Elementos nuevos tras cerrar: N` y las líneas
`nuevo …` (categoría y tipo). Después **Ctrl+Z** hasta que desaparezcan (o cierra sin guardar al final). Luego:

```powershell
Anota "12c-4 sondeo 12 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12c-4 sondeo 13 tras deshacer" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

### 12c-5. Las soldaduras `WeldLine` del add-in: crear y borrar el nudo en K y el empalme **(la persona crea; el instalador lista y borra)**

1. **(la persona)** Selecciona el cordón **1245533** y las diagonales **1249631** y **1249632**, pulsa **Conectar** (debe
   proponer "Nudo K cordon superior") y pulsa **Crear**. Anota el cuadro: **cuántos elementos** y **qué avisos** (se espera
   `END_CUT_APPROXIMATED`, como siempre; si además sale un `REVIT_WARNING` que diga "no materializó las 2 soldadura(s) WeldLine",
   cópialo). Mira la cara inferior del cordón: la soldadura, ¿es un cordón de Advance Steel (se selecciona como soldadura) o el
   cilindro gris de siempre (DirectShape)? Captura **`fase12c-01-soldadura-k.png`** de cerca.
2. **(instalador)**:

   ```powershell
   Anota "12c-5 list tras el nudo K" { .\scripts\conn-call.ps1 -Operation list }
   ```

   Copia el `connection_id` de la conexión listada y bórrala:

   ```powershell
   Anota "12c-5 delete nudo K" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL LIST>"}' }
   ```

3. **(la persona)** Selecciona los tramos **1245531** y **1245530**, pulsa **Conectar** (arranque Detalle C) y **Crear**. Anota
   **cuántos elementos** (**8** = 2 planchas + 4 patrones + 2 soldaduras si las `WeldLine` se materializan; 14 si cayeron a
   DirectShape) y los avisos. Ponte **mirando por el eje del cordón** (de frente a la plancha): el **430 debe ir en horizontal**
   (perpendicular al plano de la cercha) y la soldadura debe rodear el tubo de **305 × 203** sobre la plancha. Captura
   **`fase12c-02-empalme-frente.png`**.
4. **(instalador)**:

   ```powershell
   Anota "12c-5 list tras el empalme" { .\scripts\conn-call.ps1 -Operation list }
   Anota "12c-5 delete empalme" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL LIST>"}' }
   Anota "12c-5 sondeo 12 tras borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "12c-5 sondeo 13 tras borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

Se espera: `list` con 1 conexión cada vez y 0 tras borrar (`delete` con las barras restauradas; el aviso de la
`FabricationTransaction` al borrar es lo de siempre), sondeos 12 y 13 a cero.

### 12c-6. Puente y log **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "12c-6 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "12c-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "12c-6 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "12c-6 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|advance_steel_weld|fabrication_transaction_commit|fabrication_transaction_open|end_plate_built|direct_weld_member|ribbon_create|_failed|ribbon_batch_window_error|plan_event_failed" | Select-Object -Last 120 }
Select-String -Path $salida -Pattern "^## 12c-" | Select-Object LineNumber, Line
```

Se espera: `servidor 8000 escuchando` con una fila; **`Resultado: 38/38 pruebas correctas`**; `templates_count: 2`; en el log
`"addin_version":"0.12.2"`, una línea `advance_steel_weld_support` con `weld_line: true` y `connect: true`, **`advance_steel_weld_written`**
por cada soldadura (en el nudo en K dos con `connect: "sin Connect…"`; en el empalme dos con `connected_to` = la plancha de su
tramo), `fabrication_transaction_commit` con `pending_welds` y **`missing`** (0 si todas se materializaron), `end_plate_built` con
**`tube_footprint_mm [304.8, 203.2]`**, `direct_weld_member` con `face_y_mm -101.6`, y **ninguna** línea `advance_steel_weld_failed`,
`ribbon_create_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas enteras). **La última línea debe
listar todas las secciones `## 12c-`**; si falta alguna, repite su `Anota`.

### 12c-7. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "12c-7 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-12c.md docs\fases\capturas
   git commit -m "Ronda 12c: resultados del instalador (0.12.2, sondeo 28, WeldLine, perfil del cordon)"
   git pull --no-rebase origin main
   git push origin main
   ```

Qué devolver: `docs/fases/resultados-fase-12c.md` (lo escribe `Anota`), las capturas `fase12c-01` y `fase12c-02` (y `fase12c-03`
si hubo paso 12c-4) y, en el chat, lo que no quedó en el archivo: **el sondeo 28 en dos líneas** (viga encontrada sí / no y por qué
camino; `BeamShortening` corta sí / no y si el corte se vio inclinado; la `WeldLine` sin plancha escribe sí / no), **cuántos
elementos** creó cada conexión y si las soldaduras eran de Advance Steel o DirectShape, si el 430 iba en horizontal, y el
`Resultado:` del puente.
