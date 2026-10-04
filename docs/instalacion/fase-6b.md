# Fase 6, ronda 6b: pernos a través del paquete y edición de cotas con doble clic (instrucciones para el instalador y la persona)

Objetivo: desplegar el add-in **0.2.1** y comprobar en Revit las dos correcciones de la ronda anterior
(`docs/fases/fase-6.md`, sección 7): (1) los pernos de la placa cuchilla atraviesan ahora el paquete cartela + placa
cuchilla (placa cuchilla apoyada sobre la cara +Z de la cartela, cabeza del perno en la cara exterior de la placa cuchilla,
longitud 44,45 mm = 1-3/4" para 5/8"), en vez de salir sueltos del plano medio de la cartela; (2) en la ventana de
previsualización, **doble clic sobre una cota o un rótulo del croquis** abre un editor en sitio (Intro aplica, Esc
cancela), además de la tabla. También se vuelve a probar **Guardar JSON**, que en la ronda 6b no se pulsó. Una ronda de
unos 25 minutos sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. Los
archivos del MCP (`mcp/`) no cambian: no hace falta `instalar-conn.ps1`.

Reglas: **no escribas, edites ni generes ningún archivo de `src\`, `scripts\` ni `mcp\`**; solo compila y despliega lo
que está en la rama. No uses `git stash`, `git reset` ni `git checkout` de archivos. Si un paso falla, no modifiques nada:
copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` en la misma ventana de PowerShell.
**Revit lo abre y lo cierra la persona.** Las capturas las hace la persona con `Windows + Mayús + S` y las guarda con
el nombre indicado en `docs\fases\capturas\`.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 6b-2.
- Tener a mano el plano del Detalle D (`docs\fixtures\detalle-D.png`).

### 6b-0. Estado del repositorio (devolver la salida literal, sin resumir)

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git stash list
git fetch origin
git diff --stat origin/claude/laughing-pascal-tsxvkt
```

Si `git status` muestra archivos modificados dentro de `src\`, **para y devuelve la salida**: no se despliega nada hasta
que la persona decida qué hacer con esos cambios.

### 6b-1. PowerShell, rama y archivo de resultados (se añade al archivo de la ronda 6b)

```powershell
git pull --no-rebase origin claude/laughing-pascal-tsxvkt
$salida = "docs\fases\resultados-fase-6.md"
"`n# Ronda 6b-2 (pernos y doble clic)`n`nFecha: $(Get-Date -Format s)`n" | Add-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "6b-1 git" { git log -1 --oneline; git status --short }
```

La última línea del log debe ser un commit "Fase 6b: ..." y `git status --short` debe estar vacío (salvo el archivo de
resultados).

### 6b-2. Compilar, pasar las pruebas, Revit cerrado y desplegar

```powershell
Anota "6b-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
Anota "6b-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6b-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "6b-2 limits desplegado" { Select-String -Path "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json" -Pattern "length_addition_mm|length_increment_mm" }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 123`, `6b-2 revit cerrado` vacío,
`== MotorConexiones 0.2.1.0 desplegado en Revit 2027 ==` y dos líneas del `limits.json` desplegado con
`length_addition_mm` y `length_increment_mm` (son nuevas: sin ellas el token y la longitud del perno no serían los de
esta ronda). Si el build falla, **para aquí**.

### 6b-3. Abrir Revit con la copia y comprobar la versión

**La persona abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`**. *Always Load* si pregunta.
Espera unos 20 s y:

```powershell
Anota "6b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
Anota "6b-3 preview pernos" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-D-confirmado.json; .\scripts\conn-call.ps1 -Operation preview -Body ('{"spec":' + $spec + '}') }
```

Se espera `addin_version: 0.2.1` y, en `preview`, un bloque `bolt_stacks` con `grip_mm 19.525`, `bolt_length_mm 44.45`,
`knife_plate_z_offset_mm 9.7625`, `head_face_z_mm 14.7625` y `gusset_face "+z"`. Si `addin_version` no es `0.2.1`, Revit
sigue con la DLL anterior: anótalo, cierra Revit y repite 6b-2 y 6b-3.

### 6b-4. Ventana: doble clic en una cota (lo hace la persona)

1. **ARBA > Conexiones > Ejecutar especificación JSON** → `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
2. Bajo el croquis debe verse una línea gris que empieza por "Pernos de la barra 1249636: agarre 19,5 mm (cartela 9,5 +
   placa cuchilla 10,0), longitud 44,5 mm …". Anota si aparece.
3. Pasa el ratón por encima de una cota (por ejemplo `placa 170,0`): el cursor cambia a mano y la cota se resalta.
   **Haz doble clic sobre el texto de la cota.** Debe abrirse un pequeño editor junto al cursor con el nombre del campo
   ("Placa cuchilla: largo (mm)"), su ruta JSON y el valor `170`; la fila correspondiente de la tabla queda seleccionada.
   Captura: `docs\fases\capturas\fase6b-01-editor-cota.png`.
4. Escribe `180` y pulsa **Intro**: el editor se cierra, el croquis muestra `placa 180,0`, la tabla también, y la
   validación se repite (el token cambia). Pulsa **Esc** en otra cota para ver que cancela. Después vuelve a `170`
   (doble clic, `170`, Intro) o pulsa **Recargar**.
5. Haz doble clic sobre el rótulo `PL 3/8" (9,5 mm)` de la cartela: debe abrir el editor del espesor (`9,525`). Cambia a
   `12,7` e Intro: la etiqueta pasa a `PL 1/2" (12,7 mm)` y la línea de pernos cambia a agarre 22,7 y longitud 50,8 mm
   (2"). Captura: `docs\fases\capturas\fase6b-02-espesor-desde-croquis.png`.
6. **Guardar JSON** con el espesor en 12,7: la línea de estado debe decir `Guardado en …detalle-D-confirmado-corregido.json`.
7. **Recargar** para volver al archivo original (espesor 9,525, pernos 44,5 mm). Comprueba "Validación: 0 errores" y
   **Crear** habilitado. Deja la ventana abierta y avisa al instalador.

**El instalador** comprueba el archivo guardado (con la ventana todavía abierta Revit no atiende HTTP: esta orden no
habla con Revit):

```powershell
Anota "6b-4 json corregido" { Get-ChildItem docs\fixtures\detalle-D-confirmado*.json | Select-Object Name, Length, LastWriteTime; git status --short; git diff --stat -- docs\fixtures\detalle-D-confirmado.json; Select-String -Path docs\fixtures\detalle-D-confirmado-corregido.json -Pattern 'thickness' }
```

Se espera el archivo `-corregido.json` con `"thickness_mm": 12.7` y `"thickness_label": "1/2\""`, y el original sin cambios.

### 6b-5. Crear y mirar los pernos (la persona)

1. Pulsa **Crear**. Diálogo "Conexión creada" con `Elementos creados: 9` y `Barras retiradas: 3`. Anota el id.
2. Vista 3D, nivel de detalle **Fino**, estilo **Sombreado**, zoom al nudo y gira la vista para ver la diagonal inferior
   de canto. Lo que debe verse ahora: la placa cuchilla **apoyada sobre una cara de la cartela** (no cruzándola), y los 4
   pernos **atravesando las dos placas**, con la cabeza sobre la placa cuchilla y la tuerca asomando por la otra cara de
   la cartela; nada flotando ni saliendo del plano por el lado equivocado. Captura: `docs\fases\capturas\fase6b-03-pernos.png`
   (de canto) y `fase6b-04-nudo.png` (general). Anota en el chat: "los pernos atraviesan las dos placas: SÍ/NO" y "la
   placa cuchilla está sobre la cara de la cartela: SÍ/NO".

**El instalador** mide las piezas con el sondeo 16 (no crea ni borra nada):

```powershell
Anota "6b-5 sondeo 16 medir" { .\scripts\revit-exec.ps1 -File scripts\sondeos\16-medir-conexion.py -SinTransaccion -TimeoutSec 300 }
Anota "6b-5 log pernos" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String 'advance_steel_plate_written|advance_steel_bolts_written|advance_steel_connect_failed|fabrication_transaction_commit' | Select-Object -Last 8 }
```

Se espera en el sondeo 16: pernos `Bolt Length … = 44.45 mm` y `Grip Length … = 19.5 mm` (en la Fase 5b salían 45 y 80);
cartela y cuchilla con sus medidas de siempre. En el log, `advance_steel_plate_written` de la cuchilla con
`"z_offset_mm":9.7625` y el texto de `portioning`, y `advance_steel_bolts_written` con `"grip_mm":19.525`,
`"bolt_length_mm":44.45`, `"head_face_z_mm":14.7625` y el texto de `connect` (`Connect OK con 2 placas, …` o el motivo).
**Copia esas líneas enteras**: deciden la ronda.

### 6b-6. Borrar desde la cinta, sondeos 12 y 13

1. **ARBA > Conexiones > Conexiones del modelo** → selecciona la fila → **Borrar seleccionada** → **Sí**. La línea de estado
   debe decir ahora `Borrada <id>: 9 elementos eliminados y 3 barras restauradas … No queda ninguna conexión en el documento.`
   (en la ronda 6b salía al revés, con "No hay conexiones…" delante). Captura: `docs\fases\capturas\fase6b-05-borrada.png`.
   **Cerrar**.
2. El instalador:

   ```powershell
   Anota "6b-6 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6b-6 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones en el modelo: 0`, extensiones `1249630: 0.0 | 68.64`, `1249631: 0.0 | 69.2`, `1249636: 0.0 | 0.0`
   y `Elementos de acero sueltos encontrados: 0`.

### 6b-7. La IA no se ve afectada

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 15
Anota "6b-7 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 19/19 pruebas correctas` con `addin=0.2.1`.

### 6b-8. Registro del día (solo la 0.2.1)

```powershell
Anota "6b-8 log" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String '"addin_version":"0.2.1"|preview_|run_spec_ribbon|delete_ribbon|connections_window|_failed' | Select-Object -Last 40 }
```

Importan `preview_edit` con `path` `members[2].attachment.plate.length_mm` y `gusset.thickness_mm` (las ediciones desde
el croquis se registran igual que las de la tabla), `preview_saved`, `run_spec_ribbon_created`, `delete_ribbon`,
`connections_window_closed` con `"deleted":1`, y que no haya `*_failed` (un `advance_steel_connect_failed` no invalida
la ronda: significa que `Connect` no existe así en esta versión; lo decide el Grip Length del sondeo 16).

### 6b-9. Cerrar y subir (autorizado)

1. La persona cierra la copia **sin guardar** y cierra la ventana del puente.
2. Subir (el `-corregido.json` **no** se sube):

   ```powershell
   Remove-Item docs\fixtures\detalle-D-confirmado-corregido.json -ErrorAction SilentlyContinue
   git add docs\fases\resultados-fase-6.md docs\fases\capturas
   git commit -m "Fase 6b: resultados del instalador (pernos a través del paquete, doble clic en cotas)"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

3. Devuelve: la salida literal de 6b-0, 6b-2 deploy, 6b-3, 6b-4, 6b-5 (sondeo 16 y las líneas del log enteras), 6b-6,
   6b-7 y 6b-8; las cinco capturas; los SÍ/NO de la persona en 6b-4 y 6b-5; el texto de cualquier ventana de error; y
   si Revit se cerró de golpe en algún paso.
